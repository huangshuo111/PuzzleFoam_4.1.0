using System.Collections;
using System.Collections.Generic;
using Bridge;
using Network;
using UnityEngine;

public class DialogParkMinilenCollection : DialogScrollListBase
{
	private const float ITEM_BASE_SCALE = 145f;

	private const float RANK_LINE_Y = 60f;

	private const int ONE_ROW_MINILEN_COUNT = 3;

	public Vector3 gridBasePos;

	private List<int> indexList_ = new List<int>();

	private Vector3 baselocalPos;

	private Vector3 item_interval;

	private Vector3 area_line_interval;

	private List<GameObject> contentsList = new List<GameObject>();

	public override void OnCreate()
	{
		base.OnCreate();
		GameObject gameObject = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "MinilenProfile_item")) as GameObject;
		Utility.setParent(gameObject, base.transform, false);
		gameObject.SetActive(false);
		item_interval = new Vector3(0f, -145f, 0f);
		area_line_interval = new Vector3(0f, -60f, 0f);
		gridBasePos = grid_.transform.localPosition + area_line_interval / 1.4f;
		base.init(gameObject);
	}

	public override void OnOpen()
	{
		dialogManager_.addActiveDialogList(DialogManager.eDialog.ParkMinilenCollection);
	}

	public virtual void setup()
	{
		baselocalPos = Vector3.zero;
		indexList_.Clear();
		baselocalPos = item_interval / 2f;
		StageDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
		ParkStageInfo stage_info = component.getParkStageData();
		List<ParkAreaInfo> list = new List<ParkAreaInfo>();
		for (int i = 0; i < stage_info.Infos.Length; i++)
		{
			if (!list.Exists((ParkAreaInfo area) => area.area_id == stage_info.Infos[i].Area))
			{
				ParkAreaInfo parkAreaInfo = component.getParkAreaInfo(stage_info.Infos[i].Area);
				if (parkAreaInfo != null)
				{
					list.Add(parkAreaInfo);
				}
			}
		}
		int num = 0;
		Network.MinilenData[] array = new Network.MinilenData[10];
		int[] array2 = new int[1] { 30000 };
		createAreaContents(array2.Length, list[0].area_id, array2);
		itemList_[itemList_.Count - 1].transform.Find("underline").gameObject.SetActive(false);
		itemList_.Clear();
		baselocalPos += area_line_interval / 2f + item_interval / 2f;
		for (int j = 0; j < list.Count; j++)
		{
			array2 = list[j].gettable_minilens;
			num = array2.Length;
			if (num > 0)
			{
				createSubtitle(list[j].area_id);
				createAreaContents(num, list[j].area_id, array2);
				itemList_[itemList_.Count - 1].transform.Find("underline").gameObject.SetActive(false);
				itemList_.Clear();
				baselocalPos += area_line_interval / 2f + item_interval / 2f;
			}
		}
		repositionItem();
		dragPanel_.ResetPosition();
		grid_.transform.localPosition = gridBasePos;
	}

	private void addProfItem(int index)
	{
		GameObject item = createItem(null);
		itemList_.Add(item);
		indexList_.Add(index);
	}

	private void createAreaContents(int minilen_count, int area_id, int[] minilen_ids)
	{
		contentsList.Clear();
		int num = 0;
		float chara_img_diminish_rate = GlobalData.Instance.chara_img_diminish_rate;
		for (int i = 0; i * 3 < minilen_count; i++)
		{
			addProfItem(i);
		}
		List<int> list = new List<int>();
		list.Clear();
		Network.MinilenData[] minielenData = Bridge.MinilenData.getMinielenData();
		foreach (Network.MinilenData minilenData in minielenData)
		{
			for (int k = 0; k < minilen_ids.Length; k++)
			{
				if (minilen_ids[k] == minilenData.index)
				{
					list.Add(minilenData.index);
				}
			}
		}
		GameObject gameObject = null;
		for (int l = 0; l < itemList_.Count; l++)
		{
			addItem(itemList_[l], l);
			itemList_[l].name = area_id + itemList_[l].name;
			for (int m = 0; m < 3; m++)
			{
				if (num < minilen_count)
				{
					gameObject = itemList_[l].transform.Find("avatar_item_" + m.ToString("00")).gameObject;
					gameObject.name = "profile_" + list[num];
					gameObject.SetActive(true);
					Network.MinilenData[] minielenData2 = Bridge.MinilenData.getMinielenData();
					foreach (Network.MinilenData minilenData2 in minielenData2)
					{
						if (minilenData2.index == list[num])
						{
							UISprite component = gameObject.transform.Find("Detail_Button/chara1").GetComponent<UISprite>();
							string text = "UI_";
							string text2 = ((minilenData2.level > 0) ? "picturebook_mini_" : "silhouette_mini_");
							text = text + text2 + (minilenData2.index % 10000).ToString("000");
							component.spriteName = text;
							component.MakePixelPerfect();
							component.gameObject.SetActive(true);
							if (minilenData2.wearFlg == 1)
							{
								gameObject.transform.Find("set").gameObject.SetActive(true);
							}
							string empty = string.Empty;
							empty = MessageResource.Instance.getMessage(minilenData2.nameID);
							if (minilenData2.level <= 0)
							{
								component.transform.localScale = new Vector3(component.transform.localScale.x / chara_img_diminish_rate, component.transform.localScale.y / chara_img_diminish_rate, 1f);
								gameObject.transform.Find("name_Label").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(2581);
								gameObject.transform.Find("level_Label").GetComponent<UILabel>().text = string.Empty;
							}
							else
							{
								component.transform.localScale = new Vector3(component.transform.localScale.x / chara_img_diminish_rate, component.transform.localScale.y / chara_img_diminish_rate, 1f);
								gameObject.transform.Find("name_Label").GetComponent<UILabel>().text = empty;
								gameObject.transform.Find("level_Label").GetComponent<UILabel>().text = MessageResource.Instance.castCtrlCode(MessageResource.Instance.getMessage(8802), 1, minilenData2.level.ToString());
								gameObject.transform.Find("new").gameObject.SetActive(PlayerPrefs.GetInt(minilenData2.index + "_isNew", 0) == 0);
							}
							if (minilenData2.index == 30000)
							{
								gameObject.transform.Find("level_Label").GetComponent<UILabel>().text = string.Empty;
							}
						}
					}
					num++;
				}
				else
				{
					itemList_[l].transform.Find("avatar_item_" + m.ToString("00")).gameObject.SetActive(false);
				}
			}
			if (l > 0)
			{
				baselocalPos += item_interval;
			}
			itemList_[l].transform.localPosition = baselocalPos;
		}
	}

	public void createSubtitle(int area_id)
	{
		GameObject gameObject = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "MinilenProfile_subtitle")) as GameObject;
		Utility.setParent(gameObject, base.transform.Find("DragPanel/contents"), false);
		gameObject.transform.localPosition = baselocalPos;
		baselocalPos += area_line_interval / 2f + item_interval / 2f;
		gameObject.transform.Find("label_message").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(9200 + area_id);
	}

	public void setIconChange(int id)
	{
		bool flag = false;
		Network.MinilenData[] minielenData = Bridge.MinilenData.getMinielenData();
		foreach (Network.MinilenData minilenData in minielenData)
		{
			GameObject gameObject = GameObject.Find("profile_" + minilenData.index);
			if (gameObject != null)
			{
				gameObject.transform.Find("set").gameObject.SetActive(false);
				if (id == minilenData.index)
				{
					gameObject.transform.Find("set").gameObject.SetActive(true);
				}
			}
		}
	}

	public void DestroyContents()
	{
		Transform[] componentsInChildren = base.transform.Find("DragPanel/contents").GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name != "contents")
			{
				Object.Destroy(transform.gameObject);
			}
		}
	}

	protected virtual IEnumerator OnButton(GameObject trigger)
	{
		if (trigger.name.Contains("profile_"))
		{
			Constant.SoundUtil.PlayButtonSE();
			DialogParkMinilenProfile profDialog = dialogManager_.getDialog(DialogManager.eDialog.ParkMinilenProfile) as DialogParkMinilenProfile;
			int minilenID = int.Parse(trigger.name.Replace("profile_", string.Empty));
			profDialog.setup(minilenID);
			disableNew(minilenID);
			trigger.transform.Find("new").gameObject.SetActive(false);
			yield return dialogManager_.StartCoroutine(profDialog.open());
			yield break;
		}
		switch (trigger.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			DestroyContents();
			if (GlobalData.Instance.acInfo_.isSetup)
			{
				GlobalData.openAvatarCollectionInfo acInfo_ = GlobalData.Instance.acInfo_;
				yield return dialogManager_.StartCoroutine(OpenSetup(acInfo_));
			}
			GlobalData.Instance.acInfo_.isSetup = false;
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	public override void OnClose()
	{
		clear();
		dialogManager_.removeActiveDialogList(DialogManager.eDialog.ParkMinilenCollection);
	}

	public IEnumerator CloseDialogs()
	{
		DialogSetup setup = dialogManager_.getDialog(DialogManager.eDialog.Setup) as DialogSetup;
		DialogEventSetup eSetup = dialogManager_.getDialog(DialogManager.eDialog.EventSetup) as DialogEventSetup;
		DialogCollaborationSetup cSetup = dialogManager_.getDialog(DialogManager.eDialog.CollaborationSetup) as DialogCollaborationSetup;
		DialogPlayScore ps = dialogManager_.getDialog(DialogManager.eDialog.PlayScore) as DialogPlayScore;
		if (ps != null && ps.isOpen())
		{
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(ps));
		}
		if (setup != null && setup.isOpen())
		{
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(setup));
		}
		if (eSetup != null && eSetup.isOpen())
		{
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(eSetup));
		}
		if (cSetup != null && cSetup.isOpen())
		{
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(cSetup));
		}
	}

	public IEnumerator OpenSetup(GlobalData.openAvatarCollectionInfo acInfo_)
	{
		if (acInfo_ != null && acInfo_.isSetup && acInfo_.stageIcon != null)
		{
			acInfo_.isSetup = false;
		}
		yield return dialogManager_.StartCoroutine(dialogManager_.OpenSetup(this, acInfo_.dialog, acInfo_.stageIcon, acInfo_.part_));
		if (acInfo_.dialog == DialogManager.eDialog.Setup)
		{
			DialogSetup setup = dialogManager_.getDialog(DialogManager.eDialog.Setup) as DialogSetup;
			if (GlobalData.Instance.getSetedItems() == null)
			{
				yield break;
			}
			BoostItem[] boostItem = GlobalData.Instance.getBoostItem();
			foreach (BoostItem bi in boostItem)
			{
				if (bi.isSpecialPicup)
				{
					setup.setSpecialItem(args: new Hashtable { { "spItemIndex", bi.itemListNumber } }, itemListNumber: bi.itemListNumber, itemInfo: bi.itemInfo_);
				}
			}
			setup.setItems(GlobalData.Instance.getSetedItems());
		}
		else if (acInfo_.dialog == DialogManager.eDialog.EventSetup)
		{
			DialogEventSetup eSetup = dialogManager_.getDialog(DialogManager.eDialog.EventSetup) as DialogEventSetup;
			if (GlobalData.Instance.getSetedItems() != null)
			{
				eSetup.setItems(GlobalData.Instance.getSetedItems());
			}
		}
		else if (acInfo_.dialog == DialogManager.eDialog.CollaborationSetup)
		{
			DialogCollaborationSetup cSetup = dialogManager_.getDialog(DialogManager.eDialog.CollaborationSetup) as DialogCollaborationSetup;
			if (GlobalData.Instance.getSetedItems() != null)
			{
				cSetup.setItems(GlobalData.Instance.getSetedItems());
			}
		}
	}

	public void disableNew(int index)
	{
		Network.MinilenData[] minielenData = Bridge.MinilenData.getMinielenData();
		foreach (Network.MinilenData minilenData in minielenData)
		{
			if (minilenData.index == index && minilenData.level > 0)
			{
				PlayerPrefs.SetInt(index + "_isNew", 1);
				PlayerPrefs.Save();
				break;
			}
		}
	}
}
