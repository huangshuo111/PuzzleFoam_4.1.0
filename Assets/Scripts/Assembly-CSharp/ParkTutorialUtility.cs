using System;
using System.Collections;
using System.Collections.Generic;
using Bridge;
using Network;
using UnityEngine;

public class ParkTutorialUtility : MonoBehaviour
{
	public enum eTutorial
	{
		None = 0,
		TryStage = 1,
		ReceiveReward = 2,
		BuildPark = 3,
		BubblenPark = 4,
		Max = 5
	}

	public enum eTalker
	{
		Bubblen = 0,
		Minilen = 1,
		Max = 2
	}

	private static ParkTutorialUtility _instance;

	private Transform _panel;

	private TweenGroup _panel_in;

	private TweenGroup _panel_out;

	private UISprite _fade;

	private UILabel _message_label;

	private UIButtonMessage _message_close_button;

	private UISprite _message_dialog;

	private GameObject _pickup_arrow;

	private UIButtonMessage _message_next_arrow;

	private Transform _tap_marker;

	public bool _is_press_tutorial_button;

	private bool _is_playing;

	public GameObject _old_target_object;

	public string _old_function_name = string.Empty;

	private Dictionary<Collider, bool> _old_button_enables;

	public static ParkTutorialUtility instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = GameObject.Find("TutorialManager");
				if (!gameObject)
				{
					gameObject = new GameObject("TutorialManager");
				}
				_instance = gameObject.AddComponent<ParkTutorialUtility>();
			}
			return _instance;
		}
	}

	public static bool isPlaying
	{
		get
		{
			if (!_instance)
			{
				return false;
			}
			return _instance._is_playing;
		}
	}

	private void Awake()
	{
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.Tutorial);
		if (!@object)
		{
			Debug.LogError("Tutorial GameObject Not Found !!", GlobalRoot.Instance.gameObject);
		}
		_panel = @object.transform;
		TweenGroup[] components = _panel.GetComponents<TweenGroup>();
		_panel_in = Array.Find(components, (TweenGroup tw) => tw.getGroupName() == "In");
		_panel_out = Array.Find(components, (TweenGroup tw) => tw.getGroupName() == "Out");
		Transform transform = @object.transform;
		_fade = transform.Find("fade").GetComponent<UISprite>();
		_message_label = transform.Find("Label").GetComponent<UILabel>();
		_message_close_button = transform.Find("Button").GetComponent<UIButtonMessage>();
		_message_dialog = transform.Find("window").GetComponent<UISprite>();
		_pickup_arrow = transform.Find("pickup_arrow").gameObject;
		_message_next_arrow = transform.Find("NextButton").GetComponent<UIButtonMessage>();
		_tap_marker = transform.Find("TapMarker").GetComponent<Transform>();
	}

	public bool IsPlayable(eTutorial tutorial)
	{
		GameData gameData = GlobalData.Instance.getGameData();
		if (gameData.minilenParkTutorialStatus >= 4)
		{
			return false;
		}
		switch (tutorial)
		{
		case eTutorial.TryStage:
			if (gameData.minilenParkTutorialStatus >= 1)
			{
				return false;
			}
			if (Bridge.StageData.getClearCount_Park(500001) <= 0)
			{
				return true;
			}
			return false;
		case eTutorial.ReceiveReward:
		{
			if (gameData.minilenParkTutorialStatus >= 2)
			{
				return false;
			}
			if (Bridge.StageData.getClearCount_Park(500001) != 1)
			{
				return false;
			}
			MinilenThanksDataTable thanks_table = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<MinilenThanksDataTable>();
			Network.MinilenThanksData[] array = null;
			array = GlobalData.Instance.getGameData().thanksList;
			if (!Array.Exists(array, delegate(Network.MinilenThanksData thank)
			{
				if (!thank.available)
				{
					return false;
				}
				MinilenThanks.MinilenThanksInfo info = thanks_table.getInfo(thank.ID);
				if (info == null)
				{
					return false;
				}
				return info.IncentiveType == 1;
			}))
			{
				return false;
			}
			return true;
		}
		case eTutorial.BuildPark:
		{
			if (Bridge.StageData.getClearCount_Park(500001) != 1)
			{
				return false;
			}
			if (gameData.minilenParkTutorialStatus >= 3)
			{
				return false;
			}
			BuildingData[] buildings = GlobalData.Instance.getGameData().buildings;
			if (!Array.Exists(buildings, (BuildingData building) => building.id >= 40000 && building.id < 49999 && building.x < 0))
			{
				return false;
			}
			return true;
		}
		default:
			if (Bridge.StageData.getPlayCount_Park(500002) > 0)
			{
				return false;
			}
			return true;
		}
	}

	public IEnumerator Play()
	{
		if (IsPlayable(eTutorial.Max))
		{
			Input.enable = false;
			if (IsPlayable(eTutorial.TryStage))
			{
				yield return StartCoroutine(PlayTryStage());
			}
			else if (IsPlayable(eTutorial.ReceiveReward))
			{
				yield return StartCoroutine(PlayReceiveReward());
			}
			else if (IsPlayable(eTutorial.BuildPark))
			{
				yield return StartCoroutine(PlayBuildPark(false));
			}
			else if (IsPlayable(eTutorial.BubblenPark))
			{
				yield return StartCoroutine(PlayBubblenPark());
			}
			Input.enable = true;
			ResetDialogEnable();
		}
	}

	private void ResetDialogEnable()
	{
		_fade.gameObject.SetActive(true);
		_fade.enabled = true;
		_tap_marker.gameObject.SetActive(false);
		_message_dialog.gameObject.SetActive(true);
		_message_close_button.gameObject.SetActive(true);
		_message_next_arrow.gameObject.SetActive(false);
		_message_label.gameObject.SetActive(true);
		_panel.gameObject.SetActive(false);
	}

	public IEnumerator PlayTryStage()
	{
		_is_playing = true;
		ParkObjectManager.Instance.mapScroll.enableScroll = false;
		StopCoroutine("Message");
		yield return StartCoroutine(Message(161001, eTalker.Minilen, false));
		StopCoroutine("Message");
		yield return StartCoroutine(Message(161002, eTalker.Bubblen, false));
		StopCoroutine("Message");
		yield return StartCoroutine(Message(161003, eTalker.Bubblen, false));
		StopCoroutine("Message");
		yield return StartCoroutine(Message(161004, eTalker.Bubblen, false));
		StopCoroutine("Message");
		GameObject main_ui2 = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI2);
		Transform park_stage_button = main_ui2.transform.Find("park_stage/ParkStageListButton");
		IEnumerator park_stage_button_keeper = KeepTransformLocalPostion(park_stage_button, park_stage_button.localPosition += 41f * Vector3.back);
		StartCoroutine(park_stage_button_keeper);
		StartCoroutine(Message(161005, eTalker.Bubblen, true, false, park_stage_button.localPosition));
		SetJustOneButton(park_stage_button.gameObject);
		while (!FinishedJustOneButton())
		{
			yield return 0;
		}
		StopCoroutine(park_stage_button_keeper);
		park_stage_button.localPosition += 41f * Vector3.forward;
		_is_press_tutorial_button = true;
		yield return 0;
		while (_panel_out.isPlaying())
		{
			yield return 0;
		}
		_panel.gameObject.SetActive(false);
		StopCoroutine("Message");
		DialogParkAreaList area_list = null;
		while (!area_list)
		{
			area_list = UnityEngine.Object.FindObjectOfType<DialogParkAreaList>();
			yield return 0;
		}
		yield return 0;
		yield return 0;
		yield return 0;
		Transform area_list_trans = area_list.transform;
		Vector3 area_1_button_pos = area_list_trans.localPosition;
		Transform area_drag_trans = area_list_trans.Find("DragPanel");
		area_drag_trans.GetComponent<UIDraggablePanel>().enabled = false;
		area_1_button_pos += area_drag_trans.localPosition;
		Transform area_container_trans = area_drag_trans.Find("contents");
		area_1_button_pos += area_container_trans.localPosition;
		Transform area_1_containt_trans = area_container_trans.Find("2");
		area_1_button_pos += area_1_containt_trans.localPosition;
		Transform area_1_button = area_1_containt_trans.Find("Open/Item_Button0");
		area_1_button_pos += area_1_button.localPosition;
		StartCoroutine(Message(-1, eTalker.Bubblen, false, false, area_1_button_pos));
		SetJustOneButton(area_1_button.gameObject);
		while (!FinishedJustOneButton())
		{
			_fade.gameObject.SetActive(false);
			yield return 0;
		}
		_is_press_tutorial_button = true;
		area_drag_trans.GetComponent<UIDraggablePanel>().enabled = true;
		_fade.gameObject.SetActive(true);
		yield return 0;
		while (_panel_out.isPlaying())
		{
			yield return 0;
		}
		_panel.gameObject.SetActive(false);
		StopCoroutine("Message");
		DialogParkStageList stage_list = null;
		while (!stage_list)
		{
			stage_list = UnityEngine.Object.FindObjectOfType<DialogParkStageList>();
			yield return 0;
		}
		yield return 0;
		yield return 0;
		yield return 0;
		Transform stage_list_trans = stage_list.transform;
		Vector3 stage_1_button_pos = stage_list_trans.localPosition;
		Transform stage_drag_trans = stage_list_trans.Find("DragPanel");
		stage_drag_trans.GetComponent<UIDraggablePanel>().enabled = false;
		stage_1_button_pos += stage_drag_trans.localPosition;
		Transform stage_container_trans = stage_drag_trans.Find("contents");
		stage_1_button_pos += stage_container_trans.localPosition;
		Transform stage_1_containt_trans = stage_container_trans.Find("2");
		stage_1_button_pos += stage_1_containt_trans.localPosition;
		Transform stage_1_button = stage_1_containt_trans.Find("Item_Button500001");
		stage_1_button_pos += stage_1_button.localPosition;
		StartCoroutine(Message(-1, eTalker.Bubblen, false, false, stage_1_button_pos));
		SetJustOneButton(stage_1_button.gameObject);
		while (!FinishedJustOneButton())
		{
			_fade.gameObject.SetActive(false);
			yield return 0;
		}
		_is_press_tutorial_button = true;
		stage_drag_trans.GetComponent<UIDraggablePanel>().enabled = true;
		_fade.gameObject.SetActive(true);
		yield return 0;
		while (_panel_out.isPlaying())
		{
			yield return 0;
		}
		_panel.gameObject.SetActive(false);
		StopCoroutine("Message");
		DialogSetupPark park_setup = null;
		while (!park_setup)
		{
			park_setup = UnityEngine.Object.FindObjectOfType<DialogSetupPark>();
			yield return 0;
		}
		Transform setup_button = park_setup.transform.Find("Play_Button");
		StartCoroutine(Message(-1, eTalker.Bubblen, false, false, setup_button.localPosition));
		SetJustOneButton(setup_button.gameObject);
		while (!FinishedJustOneButton())
		{
			_fade.gameObject.SetActive(false);
			yield return 0;
		}
		_is_press_tutorial_button = true;
		yield return 0;
		while (_panel_out.isPlaying())
		{
			yield return 0;
		}
		GlobalData.Instance.getGameData().minilenParkTutorialStatus = 1;
		_is_playing = false;
	}

	public IEnumerator PlayReceiveReward()
	{
		_is_playing = true;
		ParkObjectManager.Instance.mapScroll.enableScroll = false;
		StopCoroutine("Message");
		yield return StartCoroutine(Message(161006, eTalker.Bubblen, false));
		StopCoroutine("Message");
		yield return StartCoroutine(Message(161007, eTalker.Minilen, false));
		StopCoroutine("Message");
		yield return StartCoroutine(Message(161008, eTalker.Bubblen, false));
		StopCoroutine("Message");
		GameObject main_ui2 = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI2);
		Transform minilen_thanks_button_parent = main_ui2.transform.Find("minilen");
		minilen_thanks_button_parent.gameObject.AddComponent<UIPanel>();
		Transform minilen_thanks_button = minilen_thanks_button_parent.Find("minilen_button");
		IEnumerator minilen_thanks_button_keeper = KeepTransformLocalPostion(minilen_thanks_button, minilen_thanks_button.localPosition += 41f * Vector3.back);
		minilen_thanks_button_parent.gameObject.SetActive(false);
		minilen_thanks_button_parent.gameObject.SetActive(true);
		StartCoroutine(minilen_thanks_button_keeper);
		StartCoroutine(Message(161009, eTalker.Bubblen, true, false, minilen_thanks_button.localPosition));
		SetJustOneButton(minilen_thanks_button.gameObject);
		while (!FinishedJustOneButton())
		{
			yield return 0;
		}
		StopCoroutine(minilen_thanks_button_keeper);
		minilen_thanks_button.localPosition += 41f * Vector3.forward;
		_is_press_tutorial_button = true;
		yield return 0;
		while (_panel_out.isPlaying())
		{
			yield return 0;
		}
		_panel.gameObject.SetActive(false);
		StopCoroutine("Message");
		DialogParkThanksList thanks_list = null;
		while (!thanks_list)
		{
			thanks_list = UnityEngine.Object.FindObjectOfType<DialogParkThanksList>();
			yield return 0;
		}
		yield return 0;
		yield return 0;
		yield return 0;
		Transform thanks_list_trans = thanks_list.transform;
		Vector3 thanks_1_button_pos = thanks_list_trans.parent.localPosition + thanks_list_trans.localPosition;
		Transform thanks_drag_trans = thanks_list_trans.Find("DragPanel");
		thanks_drag_trans.GetComponent<UIDraggablePanel>().enabled = false;
		thanks_1_button_pos += thanks_drag_trans.localPosition;
		Transform thanks_container_trans = thanks_drag_trans.Find("contents");
		thanks_1_button_pos += thanks_container_trans.localPosition;
		Transform thanks_1_containt_trans = thanks_container_trans.Find("1");
		thanks_1_button_pos += thanks_1_containt_trans.localPosition;
		Transform thanks_1_button = Array.Find(thanks_1_containt_trans.GetComponentsInChildren<UIButton>(true), (UIButton btn) => btn.name.Contains("Get_Button")).transform;
		thanks_1_button_pos += thanks_1_button.localPosition;
		StartCoroutine(Message(-1, eTalker.Bubblen, false, false, thanks_1_button_pos));
		SetJustOneButton(thanks_1_button.gameObject);
		while (!FinishedJustOneButton())
		{
			_fade.gameObject.SetActive(false);
			yield return 0;
		}
		_is_press_tutorial_button = true;
		thanks_drag_trans.GetComponent<UIDraggablePanel>().enabled = true;
		_fade.gameObject.SetActive(true);
		yield return 0;
		while (_panel_out.isPlaying())
		{
			yield return 0;
		}
		_panel.gameObject.SetActive(false);
		StopCoroutine("Message");
		DialogParkThankDetail thanks_detail = null;
		while (!thanks_detail)
		{
			thanks_detail = UnityEngine.Object.FindObjectOfType<DialogParkThankDetail>();
			yield return 0;
		}
		yield return 0;
		yield return 0;
		yield return 0;
		Transform thanks_detail_trans = thanks_detail.transform;
		Vector3 thanks_detail_button_pos = thanks_detail_trans.localPosition;
		Transform detail_window_trans = thanks_detail_trans.Find("window");
		thanks_detail_button_pos += detail_window_trans.localPosition;
		Transform detail_reward_trans = detail_window_trans.Find("reward_building");
		if (!detail_reward_trans.gameObject.activeSelf)
		{
			detail_reward_trans = detail_window_trans.Find("reward_area");
		}
		if (!detail_reward_trans.gameObject.activeSelf)
		{
			detail_reward_trans = detail_window_trans.Find("reward_item");
		}
		thanks_detail_button_pos += detail_reward_trans.localPosition;
		Transform thanks_detail_button = detail_reward_trans.GetComponentInChildren<UIButton>().transform;
		thanks_detail_button_pos += thanks_detail_button.localPosition;
		StartCoroutine(Message(-1, eTalker.Bubblen, false, false, thanks_detail_button_pos));
		SetJustOneButton(thanks_detail_button.gameObject);
		while (!FinishedJustOneButton())
		{
			_fade.gameObject.SetActive(false);
			yield return 0;
		}
		_is_press_tutorial_button = true;
		_fade.gameObject.SetActive(true);
		yield return 0;
		while (_panel_out.isPlaying())
		{
			yield return 0;
		}
		_panel.gameObject.SetActive(false);
		StopCoroutine("Message");
		DialogParkThankGet thanks_get = null;
		while (!thanks_get)
		{
			thanks_get = UnityEngine.Object.FindObjectOfType<DialogParkThankGet>();
			yield return 0;
		}
		Transform check_button = thanks_get.transform.Find("window/closeButton");
		StartCoroutine(Message(-1, eTalker.Bubblen, false, false, check_button.localPosition));
		SetJustOneButton(check_button.gameObject);
		while (!FinishedJustOneButton())
		{
			_fade.gameObject.SetActive(false);
			yield return 0;
		}
		_is_press_tutorial_button = true;
		_fade.gameObject.SetActive(true);
		yield return 0;
		while (_panel_out.isPlaying())
		{
			yield return 0;
		}
		_panel.gameObject.SetActive(false);
		GlobalData.Instance.getGameData().minilenParkTutorialStatus = 2;
		_is_playing = false;
		if (IsPlayable(eTutorial.BuildPark))
		{
			ResetDialogEnable();
			yield return 0;
			yield return StartCoroutine(PlayBuildPark(true));
		}
	}

	public IEnumerator PlayBuildPark(bool from_dialog)
	{
		_is_playing = true;
		ParkObjectManager.Instance.mapScroll.enableScroll = false;
		if (from_dialog)
		{
			StopCoroutine("Message");
			yield return StartCoroutine(Message(161010, eTalker.Bubblen, false));
		}
		else
		{
			StopCoroutine("Message");
			GameObject main_ui2 = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI2);
			Transform minilen_thanks_button_parent = main_ui2.transform.Find("minilen");
			minilen_thanks_button_parent.gameObject.AddComponent<UIPanel>();
			Transform minilen_thanks_button = minilen_thanks_button_parent.Find("minilen_button");
			IEnumerator minilen_thanks_button_keeper = KeepTransformLocalPostion(minilen_thanks_button, minilen_thanks_button.localPosition += 41f * Vector3.back);
			minilen_thanks_button_parent.gameObject.SetActive(false);
			minilen_thanks_button_parent.gameObject.SetActive(true);
			StartCoroutine(minilen_thanks_button_keeper);
			StartCoroutine(Message(161010, eTalker.Bubblen, true, false, minilen_thanks_button.localPosition));
			SetJustOneButton(minilen_thanks_button.gameObject);
			while (!FinishedJustOneButton())
			{
				yield return 0;
			}
			StopCoroutine(minilen_thanks_button_keeper);
			minilen_thanks_button.localPosition += 41f * Vector3.forward;
			_is_press_tutorial_button = true;
			yield return 0;
			while (_panel_out.isPlaying())
			{
				yield return 0;
			}
			_panel.gameObject.SetActive(false);
		}
		StopCoroutine("Message");
		MinilenThanksDialogManager thanks_manager = null;
		while (!thanks_manager)
		{
			thanks_manager = UnityEngine.Object.FindObjectOfType<MinilenThanksDialogManager>();
			yield return 0;
		}
		Transform thanks_manager_trans = thanks_manager.transform;
		Vector3 left_button_pos = thanks_manager_trans.localPosition;
		Transform button_panel_trans = thanks_manager_trans.Find("ButtonPanel");
		left_button_pos += button_panel_trans.localPosition;
		Transform left_button_trans = button_panel_trans.Find("Left_Button");
		left_button_pos += left_button_trans.localPosition;
		Transform building_trans = left_button_trans.Find("label_building");
		left_button_pos += building_trans.localPosition;
		StartCoroutine(Message(-1, eTalker.Bubblen, false, false, left_button_pos));
		_tap_marker.localScale = new Vector3(-1f, 1f, 1f);
		SetJustOneButton(left_button_trans.gameObject);
		while (!FinishedJustOneButton())
		{
			_fade.gameObject.SetActive(false);
			yield return 0;
		}
		_is_press_tutorial_button = true;
		_fade.gameObject.SetActive(true);
		_tap_marker.localScale = new Vector3(1f, 1f, 1f);
		yield return 0;
		while (_panel_out.isPlaying())
		{
			yield return 0;
		}
		_panel.gameObject.SetActive(false);
		yield return 0;
		while (thanks_manager.isMoving)
		{
			yield return 0;
		}
		StopCoroutine("Message");
		DialogParkBuildingList building_list = null;
		while (!building_list)
		{
			building_list = UnityEngine.Object.FindObjectOfType<DialogParkBuildingList>();
			yield return 0;
		}
		Transform building_list_trans = building_list.transform;
		Vector3 building_1_button_pos = building_list_trans.parent.localPosition;
		building_1_button_pos.x = 0f;
		Transform building_drag_trans = building_list_trans.Find("DragPanel");
		building_drag_trans.GetComponent<UIDraggablePanel>().enabled = false;
		building_1_button_pos += building_drag_trans.localPosition;
		Transform building_container_trans = building_drag_trans.Find("contents");
		building_1_button_pos += building_container_trans.localPosition;
		Transform building_1_containt_trans = building_container_trans.Find("1");
		building_1_button_pos += building_1_containt_trans.localPosition;
		Transform building_1_button = building_1_containt_trans.Find("Arrangement_Button");
		building_1_button_pos += building_1_button.localPosition;
		StartCoroutine(Message(-1, eTalker.Bubblen, false, false, building_1_button_pos));
		SetJustOneButton(building_1_button.gameObject);
		building_1_button.gameObject.SetActive(false);
		building_1_button.gameObject.SetActive(true);
		while (!FinishedJustOneButton())
		{
			_fade.gameObject.SetActive(false);
			yield return 0;
		}
		_is_press_tutorial_button = true;
		building_drag_trans.GetComponent<UIDraggablePanel>().enabled = true;
		_fade.gameObject.SetActive(true);
		yield return 0;
		while (_panel_out.isPlaying())
		{
			yield return 0;
		}
		_panel.gameObject.SetActive(false);
		StopCoroutine("Message");
		while (building_list.gameObject.activeSelf)
		{
			yield return 0;
		}
		DialogParkObjectChoices chooser = UnityEngine.Object.FindObjectOfType<DialogParkObjectChoices>();
		chooser.canOtherClickDecision = false;
		GameObject choooser_cancel_button = chooser.transform.Find("First/cancel_button").gameObject;
		while (!choooser_cancel_button.activeSelf)
		{
			yield return 0;
		}
		choooser_cancel_button.SetActive(false);
		Transform chooser_trans = chooser.transform;
		Transform chooser_container_trans = chooser_trans.Find("First");
		Vector3 ok_button_localPosition = chooser_container_trans.Find("ok_button").localPosition;
		ColliderManager.Instance.enabled = false;
		IEnumerator message_routine = Message(161011, eTalker.Bubblen, false, true, chooser_trans.localPosition + chooser_container_trans.localScale.x * ok_button_localPosition);
		while (message_routine.MoveNext())
		{
			_tap_marker.localPosition = chooser_trans.localPosition + chooser_container_trans.localScale.x * ok_button_localPosition + new Vector3(0f, 200f, -1f);
			yield return 0;
		}
		while (_panel_out.isPlaying())
		{
			yield return 0;
		}
		_tap_marker.gameObject.SetActive(false);
		ColliderManager.Instance.enabled = true;
		Input.enable = true;
		BuildingEffects building_efffect = ParkObjectManager.Instance.buildingEffects;
		while (!building_efffect.isPlaying)
		{
			yield return 0;
		}
		Input.enable = false;
		while (building_efffect.isPlaying)
		{
			yield return 0;
		}
		StopCoroutine("Message");
		yield return StartCoroutine(Message(161012, eTalker.Bubblen, false));
		choooser_cancel_button.SetActive(true);
		GlobalData.Instance.getGameData().minilenParkTutorialStatus = 3;
		_is_playing = false;
		if (IsPlayable(eTutorial.BubblenPark))
		{
			ResetDialogEnable();
			yield return 0;
			yield return StartCoroutine(PlayBubblenPark());
		}
	}

	public IEnumerator PlayBubblenPark()
	{
		TransitGate gate = UnityEngine.Object.FindObjectOfType<TransitGate>();
		if (gate == null)
		{
			Debug.LogError("No Gate Found !!!");
			yield break;
		}
		_is_playing = true;
		ParkObjectManager.Instance.mapScroll.enableScroll = false;
		StopCoroutine("Message");
		Transform target_trans = null;
		ColliderManager collider_manager = ColliderManager.Instance;
		collider_manager.TutorialClear();
		collider_manager.Add(gate.cachedTransform.GetComponentInChildren<Circle>());
		StartCoroutine(Message(161013, eTalker.Bubblen, false, false, new Vector3(1f, 0f, 0f)));
		_fade.gameObject.SetActive(false);
		UIButtonMessage[] all_buttons = UnityEngine.Object.FindObjectsOfType<UIButtonMessage>();
		Dictionary<Collider, bool> all_button_old_enabled = new Dictionary<Collider, bool>();
		Array.ForEach(all_buttons, delegate(UIButtonMessage button)
		{
			Collider collider = button.GetComponent<Collider>();
			if ((bool)collider && !all_button_old_enabled.ContainsKey(collider))
			{
				all_button_old_enabled.Add(collider, collider.enabled);
				collider.enabled = false;
			}
		});
		DialogManager dialog_manager = UnityEngine.Object.FindObjectOfType<DialogManager>();
		MapScroll park_scroll = ParkObjectManager.Instance.mapScroll;
		Camera park_camera = park_scroll.GetComponent<Camera>();
		Transform park_camera_trans = park_scroll.transform;
		DialogParkFriendList friend_list = dialog_manager.getDialog(DialogManager.eDialog.ParkFriendList) as DialogParkFriendList;
		while (!friend_list.isOpen())
		{
			park_scroll.StartTracking(gate);
			park_camera.orthographicSize += 0.1f * (400f - park_camera.orthographicSize);
			Vector3 park_camera_pos = park_camera_trans.localPosition;
			park_camera_pos.x += 0.1f * (-1400f - park_camera_pos.x);
			park_camera_pos.y += 0.1f * (-600f - park_camera_pos.y);
			park_camera_trans.localPosition = park_camera_pos;
			yield return 0;
		}
		foreach (Collider coll in all_button_old_enabled.Keys)
		{
			coll.enabled = all_button_old_enabled[coll];
		}
		_is_press_tutorial_button = true;
		ParkObjectManager.Instance.mapScroll.EndTracking();
		collider_manager.TutorialRevirt();
		while (_panel_out.isPlaying())
		{
			yield return 0;
		}
		StopCoroutine("Message");
		yield return 0;
		yield return 0;
		yield return 0;
		Transform friend_list_trans = friend_list.transform;
		Vector3 friend_1_button_pos = friend_list_trans.localPosition;
		Transform frined_drag_trans = friend_list_trans.Find("DragPanel");
		frined_drag_trans.GetComponent<UIDraggablePanel>().enabled = false;
		friend_1_button_pos += frined_drag_trans.localPosition;
		Transform friend_container_trans = frined_drag_trans.Find("contents");
		friend_1_button_pos += friend_container_trans.localPosition;
		Transform friend_1_containt_trans = friend_container_trans.Find("2");
		friend_1_button_pos += friend_1_containt_trans.localPosition;
		Transform friend_1_button = friend_1_containt_trans.GetComponentInChildren<UIButton>().transform;
		friend_1_button_pos += friend_1_button.localPosition;
		StartCoroutine(Message(-1, eTalker.Bubblen, false, false, friend_1_button_pos));
		SetJustOneButton(friend_1_button.gameObject);
		while (!FinishedJustOneButton())
		{
			_fade.gameObject.SetActive(false);
			yield return 0;
		}
		_is_press_tutorial_button = true;
		frined_drag_trans.GetComponent<UIDraggablePanel>().enabled = true;
		_fade.gameObject.SetActive(true);
		yield return 0;
		while (_panel_out.isPlaying())
		{
			yield return 0;
		}
		_panel.gameObject.SetActive(false);
		StopCoroutine("Message");
		yield return StartCoroutine(Message(161014, eTalker.Bubblen, false));
		StopCoroutine("Message");
		yield return StartCoroutine(Message(161015, eTalker.Bubblen, false));
		StopCoroutine("Message");
		MenuParkFriendMap friend_menu = null;
		while (!friend_menu)
		{
			friend_menu = UnityEngine.Object.FindObjectOfType<MenuParkFriendMap>();
			yield return 0;
		}
		yield return 0;
		yield return 0;
		yield return 0;
		Transform frined_back_button = friend_menu.transform.Find("back_button");
		StartCoroutine(Message(-1, eTalker.Bubblen, false, false, frined_back_button.localPosition));
		SetJustOneButton(frined_back_button.gameObject);
		while (!FinishedJustOneButton())
		{
			_fade.gameObject.SetActive(false);
			yield return 0;
		}
		_is_press_tutorial_button = true;
		_fade.gameObject.SetActive(true);
		yield return 0;
		while (_panel_out.isPlaying())
		{
			yield return 0;
		}
		_panel.gameObject.SetActive(false);
		StopCoroutine("Message");
		yield return StartCoroutine(Message(161016, eTalker.Bubblen, false));
		StopCoroutine("Message");
		yield return StartCoroutine(Message(161017, eTalker.Bubblen, false));
		ParkObjectManager.Instance.mapScroll.enableScroll = true;
		GlobalData.Instance.getGameData().minilenParkTutorialStatus = 4;
		_is_playing = false;
		Part_Park part_park = UnityEngine.Object.FindObjectOfType<Part_Park>();
		if ((bool)part_park)
		{
			part_park.StartCoroutine(part_park.dailyMissionClearCheck());
		}
	}

	private IEnumerator KeepTransformLocalPostion(Transform keep_transform, Vector3 keep_position)
	{
		while (true)
		{
			keep_transform.localPosition = keep_position;
			yield return 0;
		}
	}

	public IEnumerator Message(int message_id, eTalker talker, bool is_fade)
	{
		yield return StartCoroutine(Message(message_id, talker, is_fade, true, Vector3.zero));
	}

	public IEnumerator Message(int message_id, eTalker talker, bool is_fade, bool is_close_button, Vector3 target_local_pos)
	{
		_pickup_arrow.SetActive(false);
		SetMessageText(message_id, talker, is_close_button);
		_fade.enabled = is_fade;
		Vector3 panel_pos = _panel.localPosition;
		panel_pos.x = 0f;
		panel_pos.y = -200f;
		_panel.localPosition = panel_pos;
		_tap_marker.gameObject.SetActive(target_local_pos != Vector3.zero);
		_tap_marker.localPosition = target_local_pos + new Vector3(0f, 200f, -1f);
		_is_press_tutorial_button = false;
		_panel.gameObject.SetActive(true);
		_panel_in.Play();
		while (_panel_in.isPlaying())
		{
			yield return 0;
		}
		Input.enable = true;
		while (!_is_press_tutorial_button)
		{
			yield return 0;
		}
		Input.enable = false;
		_is_press_tutorial_button = false;
		_panel_out.Play();
	}

	private void SetMessageText(int message_id, eTalker talker, bool is_close_button)
	{
		if (message_id < 0)
		{
			_message_label.gameObject.SetActive(false);
			_message_dialog.gameObject.SetActive(false);
			_message_close_button.gameObject.SetActive(false);
			return;
		}
		_message_label.text = MessageResource.Instance.getMessage(message_id);
		_message_next_arrow.gameObject.SetActive(false);
		if (talker == eTalker.Minilen)
		{
			_message_label.gameObject.SetActive(true);
			_message_dialog.gameObject.SetActive(true);
			_message_close_button.gameObject.SetActive(is_close_button);
			_message_dialog.spriteName = "tutorial_window_park";
		}
		else
		{
			_message_label.gameObject.SetActive(true);
			_message_dialog.gameObject.SetActive(true);
			_message_close_button.gameObject.SetActive(is_close_button);
			_message_dialog.spriteName = "tutorial_window";
		}
	}

	private void OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Button":
		case "NextButton":
			Constant.SoundUtil.PlayDecideSE();
			_is_press_tutorial_button = true;
			break;
		}
	}

	public void SetJustOneButton(GameObject one_enable_button_obj)
	{
		UIButtonMessage component = one_enable_button_obj.GetComponent<UIButtonMessage>();
		if (!component)
		{
			Debug.LogError(one_enable_button_obj.name + " doesn't have <UIButtonMessage> Component !!", one_enable_button_obj);
			return;
		}
		_old_target_object = component.target;
		_old_function_name = component.functionName;
		component.target = base.gameObject;
		component.functionName = "OnTargetButton";
		_old_button_enables = ButtonActivateJustOne(one_enable_button_obj);
	}

	public bool FinishedJustOneButton()
	{
		if ((bool)_old_target_object)
		{
			return false;
		}
		RevirtButtonActivateJustOne(_old_button_enables);
		return true;
	}

	public Dictionary<Collider, bool> ButtonActivateJustOne(GameObject one_enbale_button_obj)
	{
		UIButton one_enable_button = one_enbale_button_obj.GetComponent<UIButton>();
		if (!one_enable_button)
		{
			Debug.LogError(one_enbale_button_obj.name + " doesn't have <UIButton> Component !!", one_enbale_button_obj);
			return new Dictionary<Collider, bool>();
		}
		Dictionary<Collider, bool> old_button_enabled = new Dictionary<Collider, bool>();
		UIButton[] array = UnityEngine.Object.FindObjectsOfType<UIButton>();
		Array.ForEach(array, delegate(UIButton button)
		{
			Collider component = button.GetComponent<Collider>();
			if ((bool)component)
			{
				if (old_button_enabled.ContainsKey(component))
				{
					Debug.Log("Already Contains " + component.name, component.gameObject);
				}
				else
				{
					old_button_enabled.Add(component, button.GetComponent<Collider>().enabled);
					component.enabled = button.gameObject == one_enable_button.gameObject;
				}
			}
		});
		return old_button_enabled;
	}

	public void RevirtButtonActivateJustOne(Dictionary<Collider, bool> old_button_enabled)
	{
		foreach (KeyValuePair<Collider, bool> item in old_button_enabled)
		{
			if ((bool)item.Key)
			{
				item.Key.enabled = item.Value;
			}
		}
	}

	public void OnTargetButton(GameObject button_object)
	{
		if (!_old_target_object)
		{
			Debug.LogError("Missing Target Called back UIButton !! ");
			return;
		}
		if (string.IsNullOrEmpty(_old_function_name))
		{
			Debug.LogError("Missing FunctionName Called back UIButton !! ");
			return;
		}
		UIButtonMessage component = button_object.GetComponent<UIButtonMessage>();
		if (!component)
		{
			Debug.LogError(button_object.name + " doesn't have <UIButtonMessage> Component !!", button_object);
			return;
		}
		_old_target_object.SendMessage(_old_function_name, button_object);
		component.target = _old_target_object;
		component.functionName = _old_function_name;
		_old_target_object = null;
		_old_function_name = string.Empty;
	}
}
