using System;
using System.Collections;
using Bridge;
using Network;
using TapjoyUnity;
using TnkAd;
using UnityEngine;

public class DialogEventSetup : DialogSetupBase
{
	public class UI
	{
		public UILabel LevelLabel;

		public UILabel TargetLabel;

		public GameObject RewardRoot;

		public GameObject Jewel;

		public GameObject Coin;

		public GameObject Heart;

		public UILabel RewardLabel;

		public UILabel SubTitleLabel;

		public GameObject[] Stars = new GameObject[Constant.StarMax];

		public BoostItem[] Items;

		public Transform ItemsRoot;

		public UIButton PlayButton;

		public UISprite PlayButtonBG;

		public UIButton HelpButton;

		public CheckBoxs EntryTermsCheckBox;

		public Transform Line1;

		public Transform Line2;

		public UILabel Item;

		public UI(GameObject window)
		{
			Transform[] componentsInChildren = window.transform.GetComponentsInChildren<Transform>(true);
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				switch (transform.name)
				{
				case "StageNo_Label":
					LevelLabel = transform.GetComponent<UILabel>();
					break;
				case "Target_Label":
					TargetLabel = transform.GetComponent<UILabel>();
					break;
				case "Clearbonus_Label":
					RewardLabel = transform.GetComponent<UILabel>();
					break;
				case "jewel":
					Jewel = transform.gameObject;
					break;
				case "coin":
					Coin = transform.gameObject;
					break;
				case "heart":
					Heart = transform.gameObject;
					break;
				case "Note_00":
					Stars[0] = transform.gameObject;
					break;
				case "Note_01":
					Stars[1] = transform.gameObject;
					break;
				case "Note_02":
					Stars[2] = transform.gameObject;
					break;
				case "Play_Button":
					PlayButton = transform.GetComponent<UIButton>();
					PlayButtonBG = transform.GetComponentInChildren<UISprite>();
					break;
				case "ItemDetail_Button":
					HelpButton = transform.GetComponent<UIButton>();
					break;
				case "items":
					ItemsRoot = transform;
					Items = transform.GetComponentsInChildren<BoostItem>(true);
					break;
				case "Clearbonus":
					RewardRoot = transform.gameObject;
					break;
				case "Checkboxs":
					EntryTermsCheckBox = transform.GetComponent<CheckBoxs>();
					break;
				case "open_condition_Label":
					SubTitleLabel = transform.GetComponent<UILabel>();
					break;
				case "line1":
					Line1 = transform;
					break;
				case "line2":
					Line2 = transform;
					break;
				case "Item":
					Item = transform.GetComponent<UILabel>();
					break;
				}
			}
		}
	}

	private StageIcon stageIcon_;

	private UILabel stageNoLabel_;

	private UILabel targetLabel_;

	private GameObject[] stars_ = new GameObject[Constant.StarMax];

	private GameObject[] plateStars_ = new GameObject[Constant.StarMax];

	private StageDataTable stageDatas_;

	private EventStageInfo.Info stageInfo_;

	private EventStageInfo eventInfo_;

	private BoostItem[] items_;

	private Transform playButton_;

	private UIButton helpButton_;

	private Transform itemsRoot_;

	private UI ui_;

	private DateTime periodBegin_;

	private DateTime periodEnd_;

	private int noPaymentItemType = -1;

	private EventMenu eventMenu_;

	private Part_EventMap part_;

	private UISprite chara_00;

	private UISprite chara_01;

	private UISprite chara_02;

	private UISprite chara_03;

	private bool blowchange;

	public static bool bReload_;

	public override void OnCreate()
	{
		base.OnCreate();
		Transform transform = base.transform.Find("windows");
		ui_ = new UI(transform.gameObject);
		chara_00 = base.transform.Find("windows/avatar_button/avatar/chara_00").GetComponent<UISprite>();
		chara_01 = base.transform.Find("windows/avatar_button/avatar/chara_01").GetComponent<UISprite>();
		chara_02 = base.transform.Find("windows/avatar_button/avatar/chara_02").GetComponent<UISprite>();
		chara_03 = base.transform.Find("windows/avatar_button/avatar/chara_03").GetComponent<UISprite>();
		UpdateCharaIcon();
		itemLabel_ = ui_.Item;
		Transform transform2 = transform.Find("Labels/StageNo_Label");
		stageNoLabel_ = transform2.GetComponent<UILabel>();
		Transform transform3 = transform.Find("Target_Label");
		targetLabel_ = transform3.GetComponent<UILabel>();
		for (int i = 0; i < stars_.Length; i++)
		{
			string text = "Note_" + i.ToString("D2");
			Transform transform4 = transform.Find("Notes/" + text);
			stars_[i] = transform4.gameObject;
		}
		for (int j = 0; j < plateStars_.Length; j++)
		{
			string text2 = "plate_star_" + j.ToString("D2");
			Transform transform5 = transform.Find(text2);
			plateStars_[j] = transform5.gameObject;
		}
		itemsRoot_ = transform.Find("items");
		items_ = itemsRoot_.GetComponentsInChildren<BoostItem>(true);
		helpButton_ = transform.Find("ItemDetail_Button").GetComponent<UIButton>();
		Transform transform6 = transform.Find("Play_Button");
		playButton_ = transform6;
		stageDatas_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
	}

	public override void OnClose()
	{
		base.OnClose();
		BoostItem[] array = items_;
		foreach (BoostItem boostItem in array)
		{
			if (boostItem.getState() != BoostItem.eState.OFF && boostItem.getPriceType() == Constant.eMoney.Coin)
			{
				Constant.PlayerData.addCoin(boostItem.getPrice());
			}
		}
		mainMenu_.update();
	}

	public void setEventMenu(EventMenu eventmenu)
	{
		eventMenu_ = eventmenu;
	}

	public IEnumerator show(StageIcon stageIcon, Part_EventMap evePart)
	{
		Input.enable = false;
		noPaymentItemType = -1;
		stageIcon_ = stageIcon;
		part_ = evePart;
		stageNo = stageIcon_.getStageNo();
		GameObject dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		eventInfo_ = dataTbl.GetComponent<StageDataTable>().getEventData();
		stageInfo_ = stageDatas_.getEventInfo(stageNo, eventInfo_.EventNo);
		periodBegin_ = DateTime.Parse(eventInfo_.BeginDate);
		periodEnd_ = DateTime.Parse(eventInfo_.EndDate);
		if (stageInfo_ == null)
		{
			Input.enable = true;
			yield break;
		}
		stageNoLabel_.text = (stageNo - eventInfo_.EventNo * 10000).ToString();
		setupTargetLabel(targetLabel_, stageInfo_.Common);
		int star = Bridge.StageData.getStar(stageNo);
		for (int j = 0; j < stars_.Length; j++)
		{
			stars_[j].SetActive(j < star);
		}
		setupUI(stageInfo_);
		setItemPos(itemsRoot_, stageInfo_.Common);
		UpdateCharaIcon();
		resetFreeTicket(items_);
		setupItem(items_, stageInfo_.Common);
		setupPricelessTicket(items_, stageInfo_.Common, noPaymentItemType);
		judgeNewIconItem(stageNo);
		for (int i = 0; i < items_.Length; i++)
		{
			BoostItem item = items_[i];
			item.setState(BoostItem.eState.OFF);
		}
		if (stageInfo_.Common.ItemNum > 0)
		{
			NGUIUtility.enable(helpButton_, false);
		}
		else
		{
			NGUIUtility.disable(helpButton_, false);
		}
		bool bAreaSale = false;
		GameData gameData = GlobalData.Instance.getGameData();
		if (gameData.saleArea != null)
		{
			setAreaSaleFlg(false);
			int[] saleArea = gameData.saleArea;
			foreach (int sale_area in saleArea)
			{
				if (sale_area == 10000)
				{
					setSalePrice();
					setAreaSaleFlg(true);
					bAreaSale = true;
					break;
				}
			}
		}
		else
		{
			setAreaSaleFlg(false);
		}
		updateSaleIconsInner();
		yield return dialogManager_.StartCoroutine(_show(stageNo));
		if (bAreaSale)
		{
			StartCoroutine("updateSaleIcons");
		}
		StartCoroutine(showTutorial());
		yield return StartCoroutine(dialogManager_.waitDialogAnimation(this));
		Input.enable = true;
	}

	private IEnumerator showTutorial()
	{
		SaveOtherData otherData = SaveData.Instance.getGameData().getOtherData();
		if (Bridge.StageData.isClear(10001))
		{
			yield break;
		}
		while (partManager_.isTransitioning())
		{
			yield return 0;
		}
		Input.enable = true;
		GameObject uiRoot = dialogManager_.getCurrentUiRoot();
		TutorialManager.Instance.load(-2, uiRoot);
		GameObject[] plate_inst = new GameObject[plateStars_.Length];
		for (int j = 0; j < plateStars_.Length; j++)
		{
			plate_inst[j] = UnityEngine.Object.Instantiate(plateStars_[j]) as GameObject;
			Utility.setParent(plate_inst[j], uiRoot.transform, false);
			plate_inst[j].transform.position = stars_[j].transform.position;
			plate_inst[j].transform.localPosition += new Vector3(0f, 0f, -100f);
		}
		yield return StartCoroutine(TutorialManager.Instance.play(-7, TutorialDataTable.ePlace.Setup, uiRoot, null, null));
		for (int i = 0; i < plate_inst.Length; i++)
		{
			if (plate_inst[i] != null)
			{
				UnityEngine.Object.DestroyImmediate(plate_inst[i]);
			}
		}
		TutorialManager.Instance.unload(-7);
		otherData.setFlag(SaveOtherData.eFlg.TutorialEventMap, true);
		Input.enable = false;
	}

	private void setupUI(EventStageInfo.Info stageInfo)
	{
		int num = stageIcon_.getStageNo();
		UI uI = ui_;
		uI.Item.text = msgRes_.getMessage(1462);
		setupTargetLabel(uI.TargetLabel, stageInfo.Common);
		setupCheckBox(uI.SubTitleLabel, uI.EntryTermsCheckBox);
		bool flag = isEntry();
		setupPlayButton(uI.PlayButton, uI.PlayButtonBG, flag);
		int star = Bridge.StageData.getStar(num);
		for (int i = 0; i < uI.Stars.Length; i++)
		{
			uI.Stars[i].SetActive(i < star);
		}
		if (flag)
		{
			uI.ItemsRoot.gameObject.SetActive(true);
			for (int j = 0; j < uI.Items.Length; j++)
			{
				BoostItem boostItem = uI.Items[j];
				boostItem.setState(BoostItem.eState.OFF);
			}
			setItemPos(uI.ItemsRoot, stageInfo.Common);
			setupItem(uI.Items, stageInfo.Common);
		}
		else
		{
			uI.ItemsRoot.gameObject.SetActive(false);
		}
		uI.SubTitleLabel.gameObject.SetActive(!flag);
		uI.Line1.gameObject.SetActive(!flag);
		uI.Line2.gameObject.SetActive(!flag);
		uI.HelpButton.gameObject.SetActive(flag);
		if (stageInfo.Common.ItemNum > 0 && flag)
		{
			NGUIUtility.enable(uI.HelpButton, false);
		}
		else
		{
			NGUIUtility.disable(uI.HelpButton, false);
		}
		setupClearCondition(base.transform.Find("windows"), stageInfo.Common);
	}

	private EventStageInfo.eLevel getPrevLevel()
	{
		return (EventStageInfo.eLevel)Mathf.Max(stageInfo_.Level - 1, 0);
	}

	private bool isEntry()
	{
		if (!EventMenu.isPrevLevelClear(stageInfo_))
		{
			return false;
		}
		if (!EventMenu.isTermsStageClear(stageInfo_))
		{
			return false;
		}
		return true;
	}

	private void setupCheckBox(UILabel subTitle, CheckBoxs checkBox)
	{
		if (isEntry())
		{
			checkBox.gameObject.SetActive(false);
			subTitle.text = msgRes_.getMessage(1462);
			return;
		}
		checkBox.gameObject.SetActive(true);
		subTitle.text = msgRes_.getMessage(600);
		int num = 0;
		if (stageInfo_.EntryTerms != -1)
		{
			checkBox.setCheck(num, EventMenu.isTermsStageClear(stageInfo_));
			string message = msgRes_.getMessage(601);
			message = msgRes_.castCtrlCode(message, 1, stageInfo_.EntryTerms.ToString());
			checkBox.setText(num, message);
			num++;
		}
		checkBox.setup(num);
	}

	private void setupPlayButton(UIButton button, UISprite buttonBG, bool bEnable)
	{
		if (bEnable)
		{
			NGUIUtility.enable(button, false);
			buttonBG.spriteName = "playbutton_00";
		}
		else
		{
			NGUIUtility.disable(button, false);
			buttonBG.spriteName = "event_lock_00";
		}
	}

	private void setupPeriodLabel(UILabel label, DateTime begin, DateTime end)
	{
		MessageResource instance = MessageResource.Instance;
		string message = instance.getMessage(12);
		message = instance.castCtrlCode(message, 1, begin.Year.ToString());
		message = instance.castCtrlCode(message, 2, begin.Month.ToString());
		message = instance.castCtrlCode(message, 3, begin.Day.ToString());
		message = instance.castCtrlCode(message, 4, end.Month.ToString());
		message = instance.castCtrlCode(message, 5, end.Day.ToString());
		label.text = message;
	}

	private void judgeNewIconItem(int stageNo)
	{
		int[] usedItemType = Bridge.StageData.getUsedItemType(stageNo);
		for (int i = 0; i < items_.Length; i++)
		{
			bool flag = false;
			if (usedItemType != null)
			{
				int[] array = usedItemType;
				foreach (int num in array)
				{
					if (items_[i].getItemType() == (Constant.Item.eType)num)
					{
						flag = true;
						break;
					}
				}
			}
			int newItemStageNo = stageDatas_.getNewItemStageNo(items_[i].getItemType());
			items_[i].setUseNewIconFlg(newItemStageNo == stageNo && !flag && Bridge.StageData.getClearCount(stageNo) < 1);
		}
	}

	public BoostItem[] getItems()
	{
		return items_;
	}

	public void setItems(bool[] bItemSet)
	{
		for (int i = 0; i < items_.Length; i++)
		{
			if (bItemSet[i])
			{
				Debug.Log("item = " + items_[i].getItemType());
				items_[i].setState(BoostItem.eState.OFF);
				StartCoroutine(pressItemButton(items_, items_[i]));
			}
		}
		GlobalData.Instance.bSetItems = null;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		if (trigger.name.Contains("Noitem_Button"))
		{
			int index = int.Parse(trigger.transform.parent.name.Replace("item_", string.Empty));
			StageDataTable dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
			int addItemStageNo = dataTbl.getAddItemStageNo(index);
			if (stageInfo_.Common.StageNo < addItemStageNo)
			{
				Constant.SoundUtil.PlayButtonSE();
				yield return StartCoroutine(pressNoItemButton(addItemStageNo, index, stageInfo_.Common));
			}
			yield break;
		}
		if (trigger.name.Contains("item_Button"))
		{
			Constant.SoundUtil.PlayButtonSE();
			yield return StartCoroutine(pressItemButton(items_, trigger.transform.parent.GetComponent<BoostItem>()));
			yield break;
		}
		switch (trigger.name)
		{
		case "ItemDetail_Button":
			StartCoroutine(pressHelpButton(trigger, stageInfo_.Common));
			break;
		case "Play_Button":
		{
			Constant.SoundUtil.PlayDecideSE();
			int stageNo = stageIcon_.getStageNo();
			Input.enable = false;
			bPossible_ = true;
			yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
			StageDataTable dataTable_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
			yield return StartCoroutine(dataTable_.downloadResourceURLData(true, false));
			if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Cancel)
			{
				Input.enable = true;
				break;
			}
			WWW www = dataTable_.getResourceURLData();
			ResponceHeaderData headerData = NetworkUtility.createResponceHeaderData(www);
			if (headerData.EventID != 1 || !eventMenu_.isEventDuration())
			{
				yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
				Input.enable = true;
				yield return dialogManager_.StartCoroutine(openNoneEventDioalog());
				yield return StartCoroutine(transMap());
				break;
			}
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			int heart = Bridge.PlayerData.getHeart();
			if (heart < 1)
			{
				Input.enable = true;
				bPossible_ = false;
				yield return StartCoroutine(base.show(eType.Heart));
				break;
			}
			yield return StartCoroutine(play(stageNo, items_));
			if (!bPossible_)
			{
				Input.enable = true;
				if (NetworkMng.Instance.getResultCode() == eResultCode.NotExistStageItem)
				{
					bReload_ = true;
					yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
					Constant.PlayerData.addCoin(getSetItemCoin());
					mainMenu_.update();
					setupItem(items_, stageInfo_.Common);
					setupPricelessTicket(items_, stageInfo_.Common, noPaymentItemType);
					BoostItem[] array = items_;
					foreach (BoostItem item in array)
					{
						item.setState(BoostItem.eState.OFF);
					}
					yield return StartCoroutine(part_.updateSaleAreaData());
					yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
					onUpdateSale();
				}
				break;
			}
			BoostItem[] array2 = items_;
			foreach (BoostItem item2 in array2)
			{
				if (item2.getState() == BoostItem.eState.ON)
				{
					if (item2.getPriceType() == Constant.eMoney.Jewel)
					{
						Tapjoy.TrackEvent("Money", "Expense Jewel", "Item Lobby Jewel", null, item2.getPrice());
						Tapjoy.TrackEvent("Game Item", "Lobby Jewel", "Event Stage" + (stageNo + 1), item2.getItemType().ToString(), "Use Jewel", item2.getPrice(), null, 0L, null, 0L);
						GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Jewel", "Item Lobby Jewel", item2.getPrice());
						GlobalGoogleAnalytics.Instance.LogEvent("Item Lobby Jewel", item2.getItemType().ToString(), "Event Stage" + (stageNo + 1), item2.getPrice());
					}
					else if (item2.getPriceType() == Constant.eMoney.Coin)
					{
						Tapjoy.TrackEvent("Money", "Expense Coin", "Item Lobby Coin", null, item2.getPrice());
						Tapjoy.TrackEvent("Game Item", "Lobby Coin", "Event Stage" + (stageNo + 1), item2.getItemType().ToString(), "Use Coin", item2.getPrice(), null, 0L, null, 0L);
						GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Coin", "Item Lobby Coin", item2.getPrice());
						GlobalGoogleAnalytics.Instance.LogEvent("Item Lobby Coin", item2.getItemType().ToString(), "Event Stage" + (stageNo + 1), item2.getPrice());
					}
				}
			}
			Tapjoy.TrackEvent("Charactor", "Use Event Stage", GlobalData.Instance.currentAvatar.index.ToString(), "Event Stage : " + (stageNo + 1), 0L);
			GlobalGoogleAnalytics.Instance.LogEvent("Charactor Use", GlobalData.Instance.currentAvatar.index.ToString(), GlobalData.Instance.currentAvatar.level.ToString(), 1L);
			Plugin.Instance.buyCompleted("USE_CHARACTOR(" + GlobalData.Instance.currentAvatar.index + ")");
			HeartEffect heartEff = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.HeartEffect).GetComponent<HeartEffect>();
			heartEff.gameObject.SetActive(true);
			yield return StartCoroutine(heartEff.play(playButton_.localPosition));
			heartEff.gameObject.SetActive(false);
			while (NetworkMng.Instance.isShowIcon())
			{
				yield return 0;
			}
			Hashtable args = createArgs(stageNo, items_);
			StopCoroutine("updateSaleIcons");
			dialogManager_.reserveCloseDialog(DialogManager.eDialog.PlayScore);
			if (Scenario.Instance.isScenario(stageNo, Scenario.ePlace.Begin))
			{
				args.Add("Place", Scenario.ePlace.Begin);
				partManager_.requestTransition(PartManager.ePart.Scenario, args, FadeMng.eType.Scenario, true);
			}
			else
			{
				partManager_.requestTransition(PartManager.ePart.Stage, args, FadeMng.eType.Cutout, true);
			}
			Input.enable = true;
			break;
		}
		case "avatar_button":
			Constant.SoundUtil.PlayButtonSE();
			mainMenu_.update();
			GlobalData.Instance.set_acInfo(true, DialogManager.eDialog.EventSetup, part_, stageIcon_);
			GlobalData.Instance.bSetItems = null;
			yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarCollection(null));
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			StopCoroutine("updateSaleIcons");
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(scoreDialog_));
			part_.dailyMissionValueSetting();
			if (part_.dailyMission.gameObject.activeSelf)
			{
				yield return StartCoroutine(part_.dailyMission.dailyMissionInfoSetup());
			}
			StartCoroutine(part_.dailyMissionClearCheck());
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	public void UpdateItems()
	{
		BoostItem[] array = items_;
		foreach (BoostItem boostItem in array)
		{
			boostItem.resetTween();
		}
		BoostItem[] array2 = items_;
		foreach (BoostItem boostItem2 in array2)
		{
			boostItem2.setState(BoostItem.eState.OFF);
		}
		mainMenu_.update();
		resetFreeTicket(items_);
		setupItem(items_, stageInfo_.Common);
		setupPricelessTicket(items_, stageInfo_.Common, noPaymentItemType);
		judgeNewIconItem(stageNo);
		Debug.Log("item.Length = " + items_.Length);
		for (int k = 0; k < items_.Length; k++)
		{
			BoostItem boostItem3 = items_[k];
			Debug.Log("item = " + boostItem3);
			if (boostItem3 != null && boostItem3.isSpecialPicup && boostItem3.getPriceType() != 0)
			{
				Debug.Log("change");
				StartCoroutine(boostItem3.change(boostItem3.itemInfo_, new Hashtable(), false));
			}
		}
	}

	private bool isLastStage(int stageNo)
	{
		if (stageNo == Constant.Event.getHighestLevelStageNo(eventInfo_.EventNo))
		{
			return true;
		}
		return false;
	}

	public override IEnumerator close()
	{
		yield return dialogManager_.StartCoroutine(base.close());
	}

	private IEnumerator openNoneEventDioalog()
	{
		yield return StartCoroutine(NetworkMng.Instance.openErrorDialog(false, eResultCode.EventIsNotHolding));
	}

	private IEnumerator openUpdateEventDioalog()
	{
		yield return dialogManager_.StartCoroutine(openCommonDioalog(61));
	}

	private IEnumerator openCommonDioalog(int msgID)
	{
		DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		dialog.setup(msgID, null, null, true);
		dialog.setButtonActive(DialogCommon.eBtn.Close, false);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
		while (dialog.isOpen())
		{
			yield return 0;
		}
	}

	public void UpdateCharaIcon()
	{
		blowchange = ResourceLoader.Instance.isUseLowResource();
		float chara_img_expand_rate = GlobalData.Instance.chara_img_expand_rate;
		string text = ((GlobalData.Instance.currentAvatar.throwCharacter <= 0) ? string.Empty : ("_" + (GlobalData.Instance.currentAvatar.throwCharacter - 1).ToString("00")));
		string text2 = ((GlobalData.Instance.currentAvatar.supportCharacter <= 0) ? string.Empty : ("_" + (GlobalData.Instance.currentAvatar.supportCharacter - 1).ToString("00")));
		if (GlobalData.Instance.currentAvatar.throwCharacter - 1 > 18)
		{
			chara_00.atlas = chara_03.atlas;
		}
		else
		{
			chara_00.atlas = chara_02.atlas;
		}
		if (GlobalData.Instance.currentAvatar.supportCharacter - 1 > 18)
		{
			chara_01.atlas = chara_03.atlas;
		}
		else
		{
			chara_01.atlas = chara_02.atlas;
		}
		chara_00.spriteName = "avatar_00" + text + "_00";
		chara_01.spriteName = "avatar_01" + text2 + "_00";
		chara_00.MakePixelPerfect();
		chara_01.MakePixelPerfect();
		chara_00.transform.localScale = new Vector3(chara_00.transform.localScale.x * chara_img_expand_rate, chara_00.transform.localScale.y * chara_img_expand_rate, chara_00.transform.localScale.z);
		chara_01.transform.localScale = new Vector3(chara_01.transform.localScale.x * chara_img_expand_rate, chara_01.transform.localScale.y * chara_img_expand_rate, chara_01.transform.localScale.z);
	}

	public int getSetItemCoin()
	{
		return totalAmount(items_, Constant.eMoney.Coin);
	}

	public void OnApplicationResumeSetupDialog()
	{
		Constant.PlayerData.addCoin(-getSetItemCoin());
	}

	public IEnumerator setItemByIndex(int item_index)
	{
		yield return StartCoroutine(pressItemButton(items_, base.gameObject.transform.Find("items/item_" + item_index.ToString("00")).GetComponent<BoostItem>()));
	}

	private void setAreaSaleFlg(bool flg)
	{
		BoostItem[] array = items_;
		foreach (BoostItem boostItem in array)
		{
			if (boostItem.getItemType() != Constant.Item.eType.Invalid && boostItem.getItemType() != (Constant.Item.eType)noPaymentItemType)
			{
				boostItem.setAreaSaleFlg(flg);
			}
		}
	}

	private void setSalePrice()
	{
		GameData gameData = GlobalData.Instance.getGameData();
		BoostItem[] array = items_;
		foreach (BoostItem boostItem in array)
		{
			if (boostItem.getItemType() != Constant.Item.eType.Invalid)
			{
				boostItem.setPrice(Constant.eMoney.Coin, boostItem.getOriginalPrice() * gameData.areaSalePercent / 100);
			}
		}
	}

	private IEnumerator updateSaleIcons()
	{
		while (true)
		{
			updateSaleIconsInner();
			yield return 0;
		}
	}

	private void updateSaleIconsInner()
	{
		BoostItem[] array = items_;
		foreach (BoostItem boostItem in array)
		{
			if (boostItem.getItemType() == (Constant.Item.eType)noPaymentItemType)
			{
				boostItem.setAreaSaleFlg(false);
			}
			boostItem.saleIconUpdater();
		}
	}

	private IEnumerator transMap()
	{
		Input.enable = false;
		partManager_.bTransitionMap_ = true;
		yield return StartCoroutine(playCloseCloudEff());
		Constant.PlayerData.addCoin(getSetItemCoin());
		dialogManager_.reserveCloseDialog(DialogManager.eDialog.PlayScore);
		Hashtable args = new Hashtable { { "IsForceSendInactive", true } };
		partManager_.requestTransition(PartManager.ePart.Map, args, FadeMng.eType.MapChange, true);
		Input.enable = true;
	}

	private IEnumerator playCloseCloudEff()
	{
		GameObject cloud_obj = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.Cloud);
		cloud_obj.SetActive(true);
		Animation anim = cloud_obj.GetComponentInChildren<Animation>();
		anim.Play("BG_Cloud_Close_anm");
		while (anim.isPlaying)
		{
			yield return 0;
		}
	}

	private void onUpdateSale()
	{
		GameData gameData = GlobalData.Instance.getGameData();
		setAreaSaleFlg(false);
		if (gameData.saleArea != null)
		{
			int[] saleArea = gameData.saleArea;
			foreach (int num in saleArea)
			{
				if (num == 10000)
				{
					setAreaSaleFlg(true);
					StartCoroutine("updateSaleIcons");
					break;
				}
			}
		}
		setupItem(items_, stageInfo_.Common);
		BoostItem[] array = items_;
		foreach (BoostItem boostItem in array)
		{
			if (boostItem.bAreaSale_)
			{
				boostItem.setPrice(Constant.eMoney.Coin, boostItem.getOriginalPrice() * gameData.areaSalePercent / 100);
			}
		}
		updateSaleIconsInner();
	}
}
