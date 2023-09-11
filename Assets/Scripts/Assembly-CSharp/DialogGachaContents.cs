using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class DialogGachaContents : DialogScrollListBase
{
	public class TabButton
	{
		public GameObject button;

		public string backgroundBaseSpriteName;

		public UISprite background;

		public TabButton(GameObject button_)
		{
			button = button_;
			background = button.transform.Find("background").GetComponent<UISprite>();
			backgroundBaseSpriteName = background.spriteName;
		}
	}

	public enum eTabType
	{
		Premium = 0,
		Normal = 1,
		Max = 2
	}

	private const float ITEM_BASE_SCALE = 140f;

	private const float RANK_LINE_Y = 60f;

	private const float HEADER_Y = 340f;

	private const int ONE_ROW_AVATAR_COUNT = 1;

	public List<int> newAvatarIdList = new List<int> { 23004 };

	private List<int> indexList_ = new List<int>();

	public bool isChallenge;

	private Vector3 baselocalPos;

	private Vector3 item_interval;

	private Vector3 rank_line_interval;

	private Vector3 header_interval;

	public Vector3 gridBasePos;

	private GameObject item_01;

	private bool blowcheck;

	private TabButton premiumButton;

	private TabButton normalButton;

	private eTabType activeTab;

	private GachaDrawList.AvatarInfo[] avatarList;

	private GachaDrawList.RatioList totalRatio;

	private GachaDrawList.RatioList totalRatio2;

	private List<GameObject> contentsList = new List<GameObject>();

	public override void OnCreate()
	{
		base.OnCreate();
		GameObject gameObject = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "GachaContents_item_00")) as GameObject;
		Utility.setParent(gameObject, base.transform, false);
		gameObject.SetActive(false);
		item_01 = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "GachaContents_item_01")) as GameObject;
		Utility.setParent(item_01, base.transform, false);
		item_01.SetActive(false);
		item_interval = new Vector3(0f, -140f, 0f);
		rank_line_interval = new Vector3(0f, -60f, 0f);
		header_interval = new Vector3(0f, -340f, 0f);
		gridBasePos = grid_.transform.localPosition + rank_line_interval / 1.4f;
		premiumButton = new TabButton(base.transform.Find("tabs/PremiumButton").gameObject);
		normalButton = new TabButton(base.transform.Find("tabs/NormalButton").gameObject);
		activeTab = (eTabType)DialogAvatarGacha.gachaNo;
		base.init(gameObject);
	}

	public virtual void setup()
	{
		blowcheck = ResourceLoader.Instance.isUseLowResource();
		baselocalPos = Vector3.zero;
		indexList_.Clear();
		int num = GlobalData.Instance.avatarCount[3];
		int num2 = GlobalData.Instance.avatarCount[2];
		int num3 = GlobalData.Instance.avatarCount[1];
		int num4 = GlobalData.Instance.avatarCount[0];
		if (activeTab == eTabType.Premium)
		{
			avatarList = GlobalData.Instance.getGameData().gachaList2;
		}
		else
		{
			avatarList = GlobalData.Instance.getGameData().gachaList;
		}
		num = 0;
		num2 = 0;
		num3 = 0;
		num4 = 0;
		GachaDrawList.AvatarInfo[] array = avatarList;
		foreach (GachaDrawList.AvatarInfo avatarInfo in array)
		{
			if (avatarInfo.rank == 3)
			{
				num++;
			}
			else if (avatarInfo.rank == 2)
			{
				num2++;
			}
			else if (avatarInfo.rank == 1)
			{
				num3++;
			}
			else
			{
				num4++;
			}
		}
		totalRatio = GlobalData.Instance.getGameData().totalRatioList;
		totalRatio2 = GlobalData.Instance.getGameData().totalRatioList2;
		baselocalPos = header_interval / 2f;
		SetTab(activeTab);
		GameObject gameObject = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "GachaContents_header")) as GameObject;
		Utility.setParent(gameObject, base.transform.Find("DragPanel/contents"), false);
		gameObject.transform.localPosition = baselocalPos;
		baselocalPos += rank_line_interval / 2f + header_interval / 2f;
		if (activeTab == eTabType.Premium)
		{
			gameObject.transform.Find("infos/label_message_4").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(8862);
		}
		else
		{
			gameObject.transform.Find("infos/label_message_4").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(8865);
		}
		if (num > 0)
		{
			createRankContents(num, Constant.Avatar.eRank.Rank_SS);
			itemList_.Clear();
			baselocalPos += rank_line_interval / 2f + item_interval / 2f;
		}
		if (num2 > 0)
		{
			createRankContents(num2, Constant.Avatar.eRank.Rank_S);
			itemList_.Clear();
			baselocalPos += rank_line_interval / 2f + item_interval / 2f;
		}
		if (num3 > 0)
		{
			createRankContents(num3, Constant.Avatar.eRank.Rank_A);
			itemList_.Clear();
			baselocalPos += rank_line_interval / 2f + item_interval / 2f;
		}
		if (num4 > 0)
		{
			createRankContents(num4, Constant.Avatar.eRank.Rank_B);
			itemList_.Clear();
		}
		reposition();
		grid_.transform.localPosition = gridBasePos;
	}

	public virtual void setup(int type)
	{
		activeTab = (eTabType)type;
		blowcheck = ResourceLoader.Instance.isUseLowResource();
		baselocalPos = Vector3.zero;
		indexList_.Clear();
		int num = GlobalData.Instance.avatarCount[3];
		int num2 = GlobalData.Instance.avatarCount[2];
		int num3 = GlobalData.Instance.avatarCount[1];
		int num4 = GlobalData.Instance.avatarCount[0];
		if (activeTab == eTabType.Premium)
		{
			avatarList = GlobalData.Instance.getGameData().gachaList2;
		}
		else
		{
			avatarList = GlobalData.Instance.getGameData().gachaList;
		}
		num = 0;
		num2 = 0;
		num3 = 0;
		num4 = 0;
		GachaDrawList.AvatarInfo[] array = avatarList;
		foreach (GachaDrawList.AvatarInfo avatarInfo in array)
		{
			if (avatarInfo.rank == 3)
			{
				num++;
			}
			else if (avatarInfo.rank == 2)
			{
				num2++;
			}
			else if (avatarInfo.rank == 1)
			{
				num3++;
			}
			else
			{
				num4++;
			}
		}
		totalRatio = GlobalData.Instance.getGameData().totalRatioList;
		totalRatio2 = GlobalData.Instance.getGameData().totalRatioList2;
		baselocalPos = header_interval / 2f;
		SetTab(activeTab);
		GameObject gameObject = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "GachaContents_header")) as GameObject;
		Utility.setParent(gameObject, base.transform.Find("DragPanel/contents"), false);
		gameObject.transform.localPosition = baselocalPos;
		baselocalPos += rank_line_interval / 2f + header_interval / 2f;
		string text = string.Empty;
		if (activeTab == eTabType.Premium)
		{
			if (totalRatio2 != null)
			{
				text = MessageResource.Instance.getMessage(8862);
				text = MessageResource.Instance.castCtrlCode(text, 1, totalRatio2.ratio_ss);
				text = MessageResource.Instance.castCtrlCode(text, 2, totalRatio2.ratio_s);
				text = MessageResource.Instance.castCtrlCode(text, 3, totalRatio2.ratio_a);
			}
		}
		else if (totalRatio != null)
		{
			text = MessageResource.Instance.getMessage(8865);
			text = MessageResource.Instance.castCtrlCode(text, 1, totalRatio.ratio_a);
			text = MessageResource.Instance.castCtrlCode(text, 2, totalRatio.ratio_b);
		}
		gameObject.transform.Find("infos/label_message_4").GetComponent<UILabel>().text = text;
		if (num > 0)
		{
			createRankContents(num, Constant.Avatar.eRank.Rank_SS);
			itemList_.Clear();
			baselocalPos += rank_line_interval / 2f + item_interval / 2f;
		}
		if (num2 > 0)
		{
			createRankContents(num2, Constant.Avatar.eRank.Rank_S);
			itemList_.Clear();
			baselocalPos += rank_line_interval / 2f + item_interval / 2f;
		}
		if (num3 > 0)
		{
			createRankContents(num3, Constant.Avatar.eRank.Rank_A);
			itemList_.Clear();
			baselocalPos += rank_line_interval / 2f + item_interval / 2f;
		}
		if (num4 > 0)
		{
			createRankContents(num4, Constant.Avatar.eRank.Rank_B);
			itemList_.Clear();
		}
		reposition();
		grid_.transform.localPosition = gridBasePos;
	}

	public void reposition()
	{
		repositionItem();
		dragPanel_.ResetPosition();
	}

	private void addProfItem(int index, bool isRankB = false)
	{
		GameObject gameObject = null;
		gameObject = (isRankB ? (Object.Instantiate(item_01) as GameObject) : createItem(null));
		itemList_.Add(gameObject);
		indexList_.Add(index);
	}

	private void createRankContents(int avatarCount, Constant.Avatar.eRank rank)
	{
		bool flag = rank == Constant.Avatar.eRank.Rank_B;
		contentsList.Clear();
		int num = avatarCount - 1;
		GameObject gameObject = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "AvatarProfile_subtitle")) as GameObject;
		Utility.setParent(gameObject, base.transform.Find("DragPanel/contents"), false);
		gameObject.transform.localPosition = baselocalPos;
		baselocalPos += rank_line_interval / 2f + item_interval / 2f;
		gameObject.transform.Find("label_message").GetComponent<UILabel>().text = MessageResource.Instance.getMessage((int)(8850 + rank));
		UISprite component = gameObject.transform.Find("Star/BG").GetComponent<UISprite>();
		float chara_img_diminish_rate = GlobalData.Instance.chara_img_diminish_rate;
		for (int i = 0; ((!flag) ? i : (i * 3)) < avatarCount; i++)
		{
			addProfItem(i, flag);
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
		GachaDrawList.AvatarInfo[] array = avatarList;
		foreach (GachaDrawList.AvatarInfo avatarInfo in array)
		{
			if (avatarInfo.rank == (int)rank && num3 < avatarInfo.index)
			{
				num3 = avatarInfo.index;
			}
			if (avatarInfo.rank == (int)rank)
			{
				list.Add(avatarInfo.index);
			}
		}
		GameObject gameObject2 = null;
		for (int k = 0; k < itemList_.Count; k++)
		{
			addItem(itemList_[k], k);
			itemList_[k].name = string.Concat(rank, itemList_[k].name);
			int num4 = ((!flag) ? 1 : 3);
			for (int l = 0; l < num4; l++)
			{
				if (num < avatarCount && num >= 0)
				{
					gameObject2 = itemList_[k].transform.Find("avatar_item_" + l.ToString("00")).gameObject;
					gameObject2.name = "profile_" + list[num];
					gameObject2.SetActive(true);
					GachaDrawList.AvatarInfo[] array2 = avatarList;
					foreach (GachaDrawList.AvatarInfo avatarInfo2 in array2)
					{
						if (avatarInfo2.index == list[num])
						{
							if (avatarInfo2.index == num3)
							{
								itemList_[k].transform.Find("underline").gameObject.SetActive(false);
							}
							UISprite component2 = gameObject2.transform.Find("Detail_Button/chara1").GetComponent<UISprite>();
							UISprite component3 = gameObject2.transform.Find("Detail_Button/chara2").GetComponent<UISprite>();
							UISprite component4 = gameObject2.transform.Find("Detail_Button/chara3").GetComponent<UISprite>();
							UISprite component5 = gameObject2.transform.Find("Detail_Button/chara4").GetComponent<UISprite>();
							string text = ((avatarInfo2.throwCharacter <= 0) ? string.Empty : ("_" + (avatarInfo2.throwCharacter - 1).ToString("00")));
							string text2 = ((avatarInfo2.supportCharacter <= 0) ? string.Empty : ("_" + (avatarInfo2.supportCharacter - 1).ToString("00")));
							string empty = string.Empty;
							if (avatarInfo2.throwCharacter - 1 > 18)
							{
								component2.atlas = component5.atlas;
							}
							else
							{
								component2.atlas = component4.atlas;
							}
							if (avatarInfo2.supportCharacter - 1 > 18)
							{
								component3.atlas = component5.atlas;
							}
							else
							{
								component3.atlas = component4.atlas;
							}
							component2.spriteName = "avatar_00" + text + "_00" + empty;
							component2.MakePixelPerfect();
							component2.transform.localScale = new Vector3(component2.transform.localScale.x * 8f / 10f, component2.transform.localScale.y * 8f / 10f, 1f);
							component3.spriteName = "avatar_01" + text2 + "_00" + empty;
							component3.MakePixelPerfect();
							component3.transform.localScale = new Vector3(component3.transform.localScale.x * 8f / 10f, component3.transform.localScale.y * 8f / 10f, 1f);
							component2.gameObject.SetActive(true);
							component3.gameObject.SetActive(true);
							gameObject2.transform.Find("Detail_Button").gameObject.SetActive(true);
							string empty2 = string.Empty;
							empty2 = ((avatarInfo2.rank == 3) ? MessageResource.Instance.getMessage(8600 + avatarInfo2.index - num2) : ((avatarInfo2.rank == 2) ? MessageResource.Instance.getMessage(8500 + avatarInfo2.index - num2) : ((avatarInfo2.rank != 1) ? MessageResource.Instance.getMessage(8300 + avatarInfo2.index - num2) : MessageResource.Instance.getMessage(8400 + avatarInfo2.index - num2))));
							gameObject2.transform.Find("name_Label").GetComponent<UILabel>().text = empty2;
							if (gameObject2.transform.Find("information_Label") != null)
							{
								gameObject2.transform.Find("information_Label").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(7200 + avatarInfo2.specialSkill);
							}
							gameObject2.transform.Find("new").gameObject.SetActive(newAvatarIdList.Contains(avatarInfo2.index));
						}
					}
					num--;
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
		UISprite[] array = new UISprite[2];
		bool flag = false;
		for (int i = 0; i < GlobalData.Instance.avatarCount[3]; i++)
		{
			GameObject gameObject = GameObject.Find("profile_" + (23000 + i));
			Debug.Log("find to " + (23000 + i));
			if (gameObject != null)
			{
				flag = 23000 + i == id;
				gameObject.transform.Find("set").gameObject.SetActive(flag);
				array[0] = gameObject.transform.Find("Detail_Button/chara1").GetComponent<UISprite>();
				array[1] = gameObject.transform.Find("Detail_Button/chara2").GetComponent<UISprite>();
				ChangeCharaSpriteColor(array, !flag);
			}
			else
			{
				Debug.Log("id = " + id);
			}
		}
		for (int j = 0; j < GlobalData.Instance.avatarCount[2]; j++)
		{
			GameObject gameObject2 = GameObject.Find("profile_" + (22000 + j));
			Debug.Log("find to " + (22000 + j));
			if (gameObject2 != null)
			{
				flag = 22000 + j == id;
				gameObject2.transform.Find("set").gameObject.SetActive(flag);
				array[0] = gameObject2.transform.Find("Detail_Button/chara1").GetComponent<UISprite>();
				array[1] = gameObject2.transform.Find("Detail_Button/chara2").GetComponent<UISprite>();
				ChangeCharaSpriteColor(array, !flag);
			}
			else
			{
				Debug.Log("id = " + id);
			}
		}
		for (int k = 0; k < GlobalData.Instance.avatarCount[1]; k++)
		{
			GameObject gameObject3 = GameObject.Find("profile_" + (21000 + k));
			if (gameObject3 != null)
			{
				flag = 21000 + k == id;
				gameObject3.transform.Find("set").gameObject.SetActive(flag);
				array[0] = gameObject3.transform.Find("Detail_Button/chara1").GetComponent<UISprite>();
				array[1] = gameObject3.transform.Find("Detail_Button/chara2").GetComponent<UISprite>();
				ChangeCharaSpriteColor(array, !flag);
			}
		}
		for (int l = 0; l < GlobalData.Instance.avatarCount[0]; l++)
		{
			GameObject gameObject4 = GameObject.Find("profile_" + (20000 + l));
			if (gameObject4 != null)
			{
				flag = 20000 + l == id;
				gameObject4.transform.Find("set").gameObject.SetActive(flag);
				array[0] = gameObject4.transform.Find("Detail_Button/chara1").GetComponent<UISprite>();
				array[1] = gameObject4.transform.Find("Detail_Button/chara2").GetComponent<UISprite>();
			}
		}
	}

	private void ChangeCharaSpriteColor(UISprite[] sprites, bool bDefault)
	{
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
		switch (trigger.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			DestroyContents();
			GlobalData.Instance.acInfo_.isSetup = false;
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "gachaButton":
			Constant.SoundUtil.PlayCancelSE();
			DestroyContents();
			GlobalData.Instance.acInfo_.isSetup = false;
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "PremiumButton":
			if (activeTab != 0)
			{
				Constant.SoundUtil.PlayButtonSE();
				activeTab = eTabType.Premium;
				DestroyContents();
				setup((int)activeTab);
			}
			break;
		case "NormalButton":
			if (activeTab != eTabType.Normal)
			{
				Constant.SoundUtil.PlayButtonSE();
				activeTab = eTabType.Normal;
				DestroyContents();
				setup((int)activeTab);
			}
			break;
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
		Network.Avatar[] array = GlobalData.Instance.getGameData().avatarList;
		foreach (Network.Avatar avatar in array)
		{
			if (avatar.index == index && avatar.level > 0)
			{
				PlayerPrefs.SetInt(index + "_isNew", 1);
				PlayerPrefs.Save();
				break;
			}
		}
	}

	public void SetTab(eTabType type)
	{
		premiumButton.background.spriteName = premiumButton.backgroundBaseSpriteName;
		normalButton.background.spriteName = normalButton.backgroundBaseSpriteName;
		switch (type)
		{
		case eTabType.Premium:
			premiumButton.background.spriteName += "_on";
			break;
		case eTabType.Normal:
			normalButton.background.spriteName += "_on";
			break;
		}
	}
}
