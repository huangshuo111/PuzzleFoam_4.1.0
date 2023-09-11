using System;
using System.Collections;
using UnityEngine;

public class MinilenThanksDialogManager : MonoBehaviour
{
	private static readonly float PANEL_WIDTH_DELTA = 740f;

	private DialogParkThanksList _thanks_list;

	private DialogParkBuildingList _building_list;

	private DialogParkRoadList _road_list;

	private UIPanel _fade_panel;

	private UIPanel _button_panel;

	private TweenGroup _in_tweens;

	private TweenGroup _out_tweens;

	private bool _is_moving;

	private int position;

	public bool isMoving
	{
		get
		{
			return _is_moving;
		}
	}

	public static MinilenThanksDialogManager Init(GameObject thanks_item, GameObject building_item, GameObject road_item)
	{
		DialogManager dialogManager = UnityEngine.Object.FindObjectOfType<DialogManager>();
		DialogParkThanksList dialogParkThanksList = dialogManager.getDialog(DialogManager.eDialog.ParkMinilenThanks) as DialogParkThanksList;
		DialogParkBuildingList dialogParkBuildingList = dialogManager.getDialog(DialogManager.eDialog.ParkBuildingList) as DialogParkBuildingList;
		DialogParkRoadList dialogParkRoadList = dialogManager.getDialog(DialogManager.eDialog.ParkRoadList) as DialogParkRoadList;
		MinilenThanksDialogManager component = dialogParkThanksList.GetComponent<MinilenThanksDialogManager>();
		if ((bool)component)
		{
			dialogParkThanksList.init(thanks_item, component);
			dialogParkBuildingList.init(building_item, component);
			dialogParkRoadList.init(road_item, component);
			return component;
		}
		GameObject original = ResourceLoader.Instance.loadGameObject("Prefabs/", "ParkMinilenThanksDialogSub");
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original);
		MinilenThanksDialogManager minilenThanksDialogManager = gameObject.AddComponent<MinilenThanksDialogManager>();
		Transform transform = minilenThanksDialogManager.transform;
		transform.SetParent(dialogParkThanksList.transform.parent, false);
		transform.localPosition = new Vector3(0f, dialogParkThanksList.transform.localPosition.y, 0f);
		transform.localScale = new Vector3(1f, 1f, 1f);
		minilenThanksDialogManager._in_tweens = Array.Find(minilenThanksDialogManager.GetComponents<TweenGroup>(), (TweenGroup tg) => tg.getGroupName() == "In");
		minilenThanksDialogManager._out_tweens = Array.Find(minilenThanksDialogManager.GetComponents<TweenGroup>(), (TweenGroup tg) => tg.getGroupName() == "Out");
		Transform transform2 = transform.Find("FadePanel");
		minilenThanksDialogManager._fade_panel = transform2.GetComponent<UIPanel>();
		Transform transform3 = transform.Find("ButtonPanel");
		minilenThanksDialogManager._button_panel = transform3.GetComponent<UIPanel>();
		Transform transform4 = transform3.Find("Right_Button");
		Vector3 localPosition = transform4.localPosition;
		localPosition.x = 1.5f * PANEL_WIDTH_DELTA;
		transform4.localPosition = localPosition;
		Transform transform5 = transform3.Find("Left_Button");
		Vector3 localPosition2 = transform5.localPosition;
		localPosition2.x = 0.5f * PANEL_WIDTH_DELTA;
		transform5.localPosition = localPosition2;
		if (!ResourceLoader.Instance.isJapanResource())
		{
			Array.ForEach(transform3.GetComponentsInChildren<UILabel>(), delegate(UILabel lbl)
			{
				lbl.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
			});
		}
		dialogParkThanksList.transform.SetParent(transform, true);
		dialogParkBuildingList.transform.SetParent(transform, true);
		dialogParkRoadList.transform.SetParent(transform, true);
		dialogParkBuildingList.transform.localPosition = dialogParkThanksList.transform.localPosition + new Vector3(PANEL_WIDTH_DELTA, 0f, -1f);
		dialogParkRoadList.transform.localPosition = dialogParkThanksList.transform.localPosition + new Vector3(2f * PANEL_WIDTH_DELTA, 0f, -1f);
		minilenThanksDialogManager._thanks_list = dialogParkThanksList;
		minilenThanksDialogManager._building_list = dialogParkBuildingList;
		minilenThanksDialogManager._road_list = dialogParkRoadList;
		NGUIUtility.setupButton(transform3.gameObject, minilenThanksDialogManager.gameObject, true);
		minilenThanksDialogManager.gameObject.SetActive(false);
		dialogParkThanksList.init(thanks_item, minilenThanksDialogManager);
		dialogParkBuildingList.init(building_item, minilenThanksDialogManager);
		dialogParkRoadList.init(road_item, minilenThanksDialogManager);
		return minilenThanksDialogManager;
	}

	public IEnumerator Show()
	{
		Input.enable = false;
		_fade_panel.alpha = 0f;
		_button_panel.alpha = 0f;
		base.gameObject.SetActive(true);
		_building_list.show_setup();
		_road_list.show_setup();
		_thanks_list.show_setup();
		yield return 0;
		_building_list.gameObject.SetActive(true);
		StartCoroutine(_building_list.open());
		_road_list.gameObject.SetActive(true);
		StartCoroutine(_road_list.open());
		_thanks_list.gameObject.SetActive(true);
		StartCoroutine(_thanks_list.open());
		_in_tweens.Play();
		while (_in_tweens.isPlaying())
		{
			yield return 0;
		}
		Input.enable = true;
	}

	public IEnumerator Close()
	{
		Constant.SoundUtil.PlayCancelSE();
		StartCoroutine(_building_list.close());
		StartCoroutine(_road_list.close());
		StartCoroutine(_thanks_list.close());
		_out_tweens.Play();
		while (_out_tweens.isPlaying())
		{
			yield return 0;
		}
		while (_building_list.isOpen() || _road_list.isOpen() || _thanks_list.isOpen())
		{
			yield return 0;
		}
		base.gameObject.SetActive(false);
	}

	public void OnButton(GameObject obj)
	{
		if (obj.name.Contains("Left"))
		{
			StartCoroutine(OnPressMoveButton(true));
		}
		else
		{
			StartCoroutine(OnPressMoveButton(false));
		}
	}

	public IEnumerator OnPressMoveButton(bool left)
	{
		if (left)
		{
			if (position == 0)
			{
				position = -1;
			}
			else if (position == -1)
			{
				position = 0;
			}
		}
		else if (position == -1)
		{
			position = -2;
		}
		else if (position == -2)
		{
			position = -1;
		}
		Constant.SoundUtil.PlayButtonSE();
		_is_moving = true;
		Input.enable = false;
		iTween.MoveTo(base.gameObject, iTween.Hash("x", (float)position * PANEL_WIDTH_DELTA, "time", 0.3f, "islocal", true, "easeType", iTween.EaseType.easeInOutCubic));
		yield return 0;
		while ((bool)_thanks_list.GetComponent<iTween>())
		{
			yield return 0;
		}
		_is_moving = false;
		Input.enable = true;
	}

	public IEnumerator Move(bool left)
	{
		if (left)
		{
			if (position == -2)
			{
				yield break;
			}
			position--;
		}
		else
		{
			if (position == 0)
			{
				yield break;
			}
			position++;
		}
		Constant.SoundUtil.PlayButtonSE();
		_is_moving = true;
		Input.enable = false;
		iTween.MoveTo(base.gameObject, iTween.Hash("x", (float)position * PANEL_WIDTH_DELTA, "time", 0.3f, "islocal", true, "easeType", iTween.EaseType.easeInOutCubic));
		yield return 0;
		while ((bool)_thanks_list.GetComponent<iTween>())
		{
			yield return 0;
		}
		_is_moving = false;
		Input.enable = true;
	}
}
