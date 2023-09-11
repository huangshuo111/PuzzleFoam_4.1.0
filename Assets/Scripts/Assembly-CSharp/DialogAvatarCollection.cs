using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class DialogAvatarCollection : DialogScrollListBase
{
	private const float ITEM_BASE_SCALE = 145f;

	private const float RANK_LINE_Y = 60f;

	private const int ONE_ROW_AVATAR_COUNT = 3;

	private List<int> indexList_ = new List<int>();

	public bool isChallenge;

	private Vector3 baselocalPos;

	private Vector3 item_interval;

	private Vector3 rank_line_interval;

	public Vector3 gridBasePos;

	private bool blowcheck;

	private List<GameObject> contentsList = new List<GameObject>();

	public override void OnCreate()
	{
		base.OnCreate();
		GameObject gameObject = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "AvatarProfile_item")) as GameObject;
		Utility.setParent(gameObject, base.transform, false);
		gameObject.SetActive(false);
		item_interval = new Vector3(0f, -145f, 0f);
		rank_line_interval = new Vector3(0f, -60f, 0f);
		gridBasePos = grid_.transform.localPosition + rank_line_interval / 1.4f;
		base.init(gameObject);
	}

	public virtual void setup()
	{
		baselocalPos = Vector3.zero;
		indexList_.Clear();
		blowcheck = GameObject.Find("ResourceLoader").GetComponent<ResourceLoader>().isUseLowResource();
		baselocalPos = rank_line_interval / 2f;
		if (GlobalData.Instance.avatarCount[3] > 0)
		{
			createRankContents(GlobalData.Instance.avatarCount[3], Constant.Avatar.eRank.Rank_SS);
			itemList_[itemList_.Count - 1].transform.Find("underline").gameObject.SetActive(false);
			itemList_.Clear();
			baselocalPos += rank_line_interval / 2f + item_interval / 2f;
		}
		if (GlobalData.Instance.avatarCount[2] > 0)
		{
			createRankContents(GlobalData.Instance.avatarCount[2], Constant.Avatar.eRank.Rank_S);
			itemList_[itemList_.Count - 1].transform.Find("underline").gameObject.SetActive(false);
			itemList_.Clear();
			baselocalPos += rank_line_interval / 2f + item_interval / 2f;
		}
		if (GlobalData.Instance.avatarCount[1] > 0)
		{
			createRankContents(GlobalData.Instance.avatarCount[1], Constant.Avatar.eRank.Rank_A);
			itemList_[itemList_.Count - 1].transform.Find("underline").gameObject.SetActive(false);
			itemList_.Clear();
			baselocalPos += rank_line_interval / 2f + item_interval / 2f;
		}
		if (GlobalData.Instance.avatarCount[0] > 0)
		{
			createRankContents(GlobalData.Instance.avatarCount[0], Constant.Avatar.eRank.Rank_B);
			itemList_[itemList_.Count - 1].transform.Find("underline").gameObject.SetActive(false);
			itemList_.Clear();
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

	private void createRankContents(int avatarCount, Constant.Avatar.eRank rank)
	{
		contentsList.Clear();
		int num = 0;
		GameObject gameObject = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "AvatarProfile_subtitle")) as GameObject;
		Utility.setParent(gameObject, base.transform.Find("DragPanel/contents"), false);
		gameObject.transform.localPosition = baselocalPos;
		baselocalPos += rank_line_interval / 2f + item_interval / 2f;
		gameObject.transform.Find("label_message").GetComponent<UILabel>().text = MessageResource.Instance.getMessage((int)(8850 + rank));
		UISprite component = gameObject.transform.Find("Star/BG").GetComponent<UISprite>();
		float chara_img_diminish_rate = GlobalData.Instance.chara_img_diminish_rate;
		for (int i = 0; i * 3 < avatarCount; i++)
		{
			addProfItem(i);
		}
		int num2 = 0;
		switch (rank)
		{
		case Constant.Avatar.eRank.Rank_SS:
			num2 = 23000;
			component.spriteName = "rank_star_large_03";
			break;
		case Constant.Avatar.eRank.Rank_S:
			num2 = 22000;
			component.spriteName = "rank_star_large_01";
			break;
		case Constant.Avatar.eRank.Rank_A:
			num2 = 21000;
			component.spriteName = "rank_star_large_02";
			break;
		case Constant.Avatar.eRank.Rank_B:
			num2 = 20000;
			component.spriteName = "rank_star_large_00";
			break;
		}
		List<int> list = new List<int>();
		list.Clear();
		int num3 = 0;
		Network.Avatar[] avatarList = GlobalData.Instance.getGameData().avatarList;
		foreach (Network.Avatar avatar in avatarList)
		{
			if (avatar.rank == (int)rank)
			{
				list.Add(avatar.index);
			}
		}
		GameObject gameObject2 = null;
		for (int k = 0; k < itemList_.Count; k++)
		{
			addItem(itemList_[k], k);
			itemList_[k].name = string.Concat(rank, itemList_[k].name);
			for (int l = 0; l < 3; l++)
			{
				if (num < avatarCount)
				{
					gameObject2 = itemList_[k].transform.Find("avatar_item_" + l.ToString("00")).gameObject;
					gameObject2.name = "profile_" + list[num];
					gameObject2.SetActive(true);
					Network.Avatar[] avatarList2 = GlobalData.Instance.getGameData().avatarList;
					foreach (Network.Avatar avatar2 in avatarList2)
					{
						if (avatar2.index == list[num])
						{
							UISprite component2 = gameObject2.transform.Find("Detail_Button/chara1").GetComponent<UISprite>();
							UISprite component3 = gameObject2.transform.Find("Detail_Button/chara2").GetComponent<UISprite>();
							UISprite component4 = gameObject2.transform.Find("Detail_Button/chara3").GetComponent<UISprite>();
							UISprite component5 = gameObject2.transform.Find("Detail_Button/chara4").GetComponent<UISprite>();
							string text = ((avatar2.throwCharacter <= 0) ? string.Empty : ("_" + (avatar2.throwCharacter - 1).ToString("00")));
							string text2 = ((avatar2.supportCharacter <= 0) ? string.Empty : ("_" + (avatar2.supportCharacter - 1).ToString("00")));
							if (avatar2.throwCharacter - 1 > 18)
							{
								component2.atlas = component5.atlas;
							}
							else
							{
								component2.atlas = component4.atlas;
							}
							if (avatar2.supportCharacter - 1 > 18)
							{
								component3.atlas = component5.atlas;
							}
							else
							{
								component3.atlas = component4.atlas;
							}
							string text3 = ((avatar2.level > 0) ? string.Empty : "_sh");
							component2.spriteName = "avatar_00" + text + "_00" + text3;
							component2.MakePixelPerfect();
							component3.spriteName = "avatar_01" + text2 + "_00" + text3;
							component3.MakePixelPerfect();
							component2.gameObject.SetActive(true);
							component3.gameObject.SetActive(true);
							if (avatar2.wearFlg == 1)
							{
								gameObject2.transform.Find("set").gameObject.SetActive(true);
							}
							string empty = string.Empty;
							empty = ((avatar2.rank == 3) ? MessageResource.Instance.getMessage(8600 + avatar2.index - num2) : ((avatar2.rank == 2) ? MessageResource.Instance.getMessage(8500 + avatar2.index - num2) : ((avatar2.rank != 1) ? MessageResource.Instance.getMessage(8300 + avatar2.index - num2) : MessageResource.Instance.getMessage(8400 + avatar2.index - num2))));
							if (avatar2.level <= 0)
							{
								gameObject2.transform.Find("name_Label").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(2581);
								gameObject2.transform.Find("level_Label").GetComponent<UILabel>().text = string.Empty;
								continue;
							}
							component2.transform.localScale = new Vector3(component2.transform.localScale.x / chara_img_diminish_rate, component2.transform.localScale.y / chara_img_diminish_rate, 1f);
							component3.transform.localScale = new Vector3(component3.transform.localScale.x / chara_img_diminish_rate, component3.transform.localScale.y / chara_img_diminish_rate, 1f);
							gameObject2.transform.Find("name_Label").GetComponent<UILabel>().text = empty;
							gameObject2.transform.Find("level_Label").GetComponent<UILabel>().text = MessageResource.Instance.castCtrlCode(MessageResource.Instance.getMessage(8802), 1, avatar2.level.ToString());
							gameObject2.transform.Find("new").gameObject.SetActive(PlayerPrefs.GetInt(avatar2.index + "_isNew", 0) == 0);
						}
					}
					num++;
				}
				else
				{
					itemList_[k].transform.Find("avatar_item_" + l.ToString("00")).gameObject.SetActive(false);
				}
			}
			if (k > 0)
			{
				baselocalPos += item_interval;
			}
			itemList_[k].transform.localPosition = baselocalPos;
		}
	}

	public void createSubtitle(float y)
	{
		GameObject original = ResourceLoader.Instance.loadGameObject("Prefabs/", "AvatarProfile_subtitle");
		line_ = Object.Instantiate(original) as GameObject;
		Utility.setParent(line_, base.transform, false);
		GameObject gameObject = line_.transform.Find("line").gameObject;
		Vector3 localPosition = gameObject.transform.localPosition;
		localPosition.y = y;
		gameObject.transform.localPosition = localPosition;
	}

	public void setIconChange(int id)
	{
		bool flag = false;
		Network.Avatar[] avatarList = GlobalData.Instance.getGameData().avatarList;
		foreach (Network.Avatar avatar in avatarList)
		{
			GameObject gameObject = GameObject.Find("profile_" + avatar.index);
			if (gameObject != null)
			{
				gameObject.transform.Find("set").gameObject.SetActive(false);
				if (id == avatar.index)
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
			DialogAvatarProfile profDialog = dialogManager_.getDialog(DialogManager.eDialog.AvatarProfile) as DialogAvatarProfile;
			int avatarID = int.Parse(trigger.name.Replace("profile_", string.Empty));
			profDialog.setup(avatarID);
			disableNew(avatarID);
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
		case "gachaButton":
		{
			Constant.SoundUtil.PlayDecideSE();
			GlobalData.Instance.acInfo_.isSetup = false;
			yield return dialogManager_.StartCoroutine(CloseDialogs());
			bool isFree = GlobalData.Instance.getGameData().isFirstGacha;
			yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarGacha(this, isFree));
			break;
		}
		}
	}

	public override void OnClose()
	{
		clear();
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
		Network.Avatar[] avatarList = GlobalData.Instance.getGameData().avatarList;
		foreach (Network.Avatar avatar in avatarList)
		{
			if (avatar.index == index && avatar.level > 0)
			{
				PlayerPrefs.SetInt(index + "_isNew", 1);
				PlayerPrefs.Save();
				break;
			}
		}
	}
}
