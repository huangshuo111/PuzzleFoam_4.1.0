using System.Collections;
using Bridge;
using Network;
using UnityEngine;

public class DialogChallengeSetup : DialogSetupBase
{
	public class UI
	{
		public UILabel LevelLabel;

		public UILabel TargetLabel;

		public BoostItem[] Items;

		public Transform ItemsRoot;

		public UIButton PlayButton;

		public UISprite PlayButtonBG;

		public UIButton HelpButton;

		public UILabel Item;

		public GameObject FriendHelp;

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
				case "Play_Button":
					PlayButton = transform.GetComponent<UIButton>();
					PlayButtonBG = transform.Find("Background").GetComponent<UISprite>();
					break;
				case "ItemDetail_Button":
					HelpButton = transform.GetComponent<UIButton>();
					break;
				case "items":
					ItemsRoot = transform;
					Items = transform.GetComponentsInChildren<BoostItem>(true);
					break;
				case "Item":
					Item = transform.GetComponent<UILabel>();
					break;
				case "FriendHelp":
					FriendHelp = transform.gameObject;
					break;
				}
			}
		}
	}

	private StageIcon stageIcon_;

	private UILabel stageNoLabel_;

	private UILabel targetLabel_;

	private StageDataTable stageDatas_;

	private EventStageInfo.Info stageInfo_;

	private EventStageInfo eventInfo_;

	private BoostItem[] items_;

	private Transform playButton_;

	private UIButton helpButton_;

	private Transform itemsRoot_;

	private UI ui_;

	private ChallengeMenu challengeMenu_;

	private int noPaymentItemType = -1;

	public static bool bReload_Challenge;

	private Part_ChallengeMap part_;

	public void setChallengeMenu(ChallengeMenu challengemenu)
	{
		challengeMenu_ = challengemenu;
	}

	public override void OnCreate()
	{
		base.OnCreate();
		Transform transform = base.transform.Find("window");
		ui_ = new UI(transform.gameObject);
		itemLabel_ = ui_.Item;
		Transform transform2 = transform.Find("Labels/StageNo_Label");
		stageNoLabel_ = transform2.GetComponent<UILabel>();
		Transform transform3 = transform.Find("Target_Label");
		targetLabel_ = transform3.GetComponent<UILabel>();
		itemsRoot_ = transform.Find("items");
		items_ = itemsRoot_.GetComponentsInChildren<BoostItem>(true);
		helpButton_ = transform.Find("ItemDetail_Button").GetComponent<UIButton>();
		Transform transform4 = transform.Find("Play_Button");
		playButton_ = transform4;
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

	public IEnumerator show(StageIcon stageIcon, Part_ChallengeMap _part)
	{
		Input.enable = false;
		noPaymentItemType = -1;
		bReload_Challenge = false;
		part_ = _part;
		stageIcon_ = stageIcon;
		int stageNo = stageIcon_.getStageNo();
		GameObject dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		eventInfo_ = dataTbl.GetComponent<StageDataTable>().getEventData();
		stageInfo_ = stageDatas_.getEventInfo(stageNo, eventInfo_.EventNo);
		if (stageInfo_ == null)
		{
			Input.enable = true;
			yield break;
		}
		stageNoLabel_.text = (stageNo - eventInfo_.EventNo * 10000).ToString();
		setupTargetLabel(targetLabel_, stageInfo_.Common);
		bool bAreaSale = false;
		setupUI(stageInfo_);
		setItemPos(itemsRoot_, stageInfo_.Common);
		setupItem(items_, stageInfo_.Common);
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
		GameData gameData = GlobalData.Instance.getGameData();
		if (gameData.saleArea != null)
		{
			setAreaSaleFlg(false);
			int[] saleArea = gameData.saleArea;
			foreach (int sale_area in saleArea)
			{
				if (sale_area == 20000)
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
		yield return StartCoroutine(dialogManager_.waitDialogAnimation(this));
		Input.enable = true;
	}

	private void setupUI(EventStageInfo.Info stageInfo)
	{
		UI uI = ui_;
		uI.Item.text = msgRes_.getMessage(1462);
		setupTargetLabel(uI.TargetLabel, stageInfo.Common);
		bool flag = isEntry();
		setupPlayButton(uI.PlayButton, uI.PlayButtonBG, flag);
		if (flag)
		{
			uI.ItemsRoot.gameObject.SetActive(true);
			for (int i = 0; i < uI.Items.Length; i++)
			{
				BoostItem boostItem = uI.Items[i];
				boostItem.setState(BoostItem.eState.OFF);
			}
			setItemPos(uI.ItemsRoot, stageInfo.Common);
			setupItem(uI.Items, stageInfo.Common);
		}
		else
		{
			uI.ItemsRoot.gameObject.SetActive(false);
		}
		uI.HelpButton.gameObject.SetActive(flag);
		if (stageInfo.Common.ItemNum > 0 && flag)
		{
			NGUIUtility.enable(uI.HelpButton, false);
		}
		else
		{
			NGUIUtility.disable(uI.HelpButton, false);
		}
		setupClearCondition(base.transform.Find("window"), stageInfo.Common);
	}

	private IEnumerator showTutorial()
	{
		SaveOtherData otherData = SaveData.Instance.getGameData().getOtherData();
		if (!Bridge.StageData.isClear(10001))
		{
			while (partManager_.isTransitioning())
			{
				yield return 0;
			}
			Input.enable = true;
			GameObject uiRoot = dialogManager_.getCurrentUiRoot();
			TutorialManager.Instance.load(-2, uiRoot);
			yield return StartCoroutine(TutorialManager.Instance.play(-7, TutorialDataTable.ePlace.Setup, uiRoot, null, null));
			TutorialManager.Instance.unload(-7);
			otherData.setFlag(SaveOtherData.eFlg.TutorialEventMap, true);
			Input.enable = false;
		}
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

	private void setupPlayButton(UIButton button, UISprite buttonBG, bool bEnable)
	{
		if (bEnable)
		{
			NGUIUtility.enable(button, false);
			if (buttonBG != null)
			{
				buttonBG.spriteName = "playbutton_00";
			}
		}
		else
		{
			NGUIUtility.disable(button, false);
			if (buttonBG != null)
			{
				buttonBG.spriteName = "event_lock_00";
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
		yield return dialogManager_.StartCoroutine(openCommonDioalog(58));
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

	private IEnumerator transMap()
	{
		Input.enable = false;
		partManager_.bTransitionMap_ = true;
		yield return StartCoroutine(playCloseWaveEff());
		Constant.PlayerData.addCoin(getSetItemCoin());
		dialogManager_.reserveCloseDialog(DialogManager.eDialog.PlayScore);
		Hashtable args = new Hashtable { { "IsForceSendInactive", true } };
		partManager_.requestTransition(PartManager.ePart.Map, args, FadeMng.eType.MapChange, true);
		Input.enable = true;
	}

	private IEnumerator playCloseWaveEff()
	{
		GameObject wave_obj = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.Wave);
		wave_obj.SetActive(true);
		Animation anim = wave_obj.GetComponentInChildren<Animation>();
		anim.Play("BG_Wave_Close_anm");
		anim["BG_Wave_Close_anm"].speed = 1.5f;
		while (anim.isPlaying)
		{
			yield return 0;
		}
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
				boostItem.setPrice(Constant.eMoney.Coin, boostItem.getPrice() * gameData.areaSalePercent / 100);
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

	private void onUpdateSale()
	{
		GameData gameData = GlobalData.Instance.getGameData();
		setAreaSaleFlg(false);
		if (gameData.saleArea != null)
		{
			int[] saleArea = gameData.saleArea;
			foreach (int num in saleArea)
			{
				if (num == 20000)
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
				boostItem.setPrice(Constant.eMoney.Coin, boostItem.getPrice() * gameData.areaSalePercent / 100);
			}
		}
		updateSaleIconsInner();
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
			BoostItem item2 = trigger.transform.parent.GetComponent<BoostItem>();
			yield return StartCoroutine(pressItemButton(items_, item2));
			if (item2.getState() != BoostItem.eState.OFF)
			{
				yield break;
			}
			BoostItem[] array = items_;
			foreach (BoostItem temp in array)
			{
				if (temp != item2 && temp.getItemType() != Constant.Item.eType.Invalid)
				{
					item2.syncSaleIconUpdater(temp);
					break;
				}
			}
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
			if (headerData.EventID != 2 || !challengeMenu_.isEventDuration())
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
					bReload_Challenge = true;
					StopCoroutine("updateSaleIcons");
					Constant.PlayerData.addCoin(getSetItemCoin());
					mainMenu_.update();
					setupItem(items_, stageInfo_.Common);
					BoostItem[] array2 = items_;
					foreach (BoostItem item in array2)
					{
						item.setState(BoostItem.eState.OFF);
					}
					yield return StartCoroutine(part_.sendInactive());
					onUpdateSale();
				}
			}
			else
			{
				HeartEffect heartEff = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.HeartEffect).GetComponent<HeartEffect>();
				heartEff.gameObject.SetActive(true);
				yield return StartCoroutine(heartEff.play(playButton_.localPosition));
				heartEff.gameObject.SetActive(false);
				while (NetworkMng.Instance.isShowIcon())
				{
					yield return 0;
				}
				Hashtable args = createArgs(stageNo, items_);
				dialogManager_.reserveCloseDialog(DialogManager.eDialog.PlayScore);
				Scenario.Instance.configReset();
				if (Scenario.Instance.isScenario_Challenge(stageNo, Scenario.ePlace.Begin))
				{
					args.Add("Place", Scenario.ePlace.Begin);
					partManager_.requestTransition(PartManager.ePart.Scenario, args, FadeMng.eType.Scenario, true);
				}
				else
				{
					partManager_.requestTransition(PartManager.ePart.Stage, args, FadeMng.eType.Cutout, true);
				}
				Input.enable = true;
			}
			break;
		}
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
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
}
