using System;
using System.Collections;
using Bridge;
using Network;
using TapjoyUnity;
using UnityEngine;

public class DialogBossSetup : DialogSetupBase
{
	public class UI
	{
		public UILabel TargetLabel;

		public GameObject RewardRoot;

		public UILabel SubTitleLabel;

		public BoostItem[] Items;

		public Transform ItemsRoot;

		public UIButton PlayButton;

		public UISprite PlayButtonBG;

		public UIButton HelpButton;

		public CheckBoxs EntryTermsCheckBox;

		public Transform Line1;

		public Transform Line2;

		public UILabel LevelLabel;

		public UILabel Item;

		public UI(GameObject window)
		{
			Transform[] componentsInChildren = window.transform.GetComponentsInChildren<Transform>(true);
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				switch (transform.name)
				{
				case "Target_Label":
					TargetLabel = transform.GetComponent<UILabel>();
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
				case "Boss_Label":
					LevelLabel = transform.GetComponent<UILabel>();
					break;
				case "Item":
					Item = transform.GetComponent<UILabel>();
					break;
				}
			}
		}
	}

	private UI ui_;

	private GameObject windowRoot_;

	private GameObject window_;

	private DateTime periodBegin_;

	private DateTime periodEnd_;

	private StageDataTable dataTable_;

	private SaveOtherData otherData_;

	private BossStageInfo.Info bossInfo_;

	private int type_;

	private int level_ = 1;

	private int noPaymentItemType = -1;

	public override void OnCreate()
	{
		base.OnCreate();
		ui_ = new UI(base.gameObject);
		itemLabel_ = ui_.Item;
		otherData_ = SaveData.Instance.getGameData().getOtherData();
		dataTable_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
		windowRoot_ = base.transform.Find("windows").gameObject;
		window_ = windowRoot_.transform.Find("window00").gameObject;
	}

	public override void OnClose()
	{
		base.OnClose();
		BoostItem[] items = ui_.Items;
		foreach (BoostItem boostItem in items)
		{
			if (boostItem.getState() != BoostItem.eState.OFF && boostItem.getPriceType() == Constant.eMoney.Coin)
			{
				setItemLabelText(1462, 0);
				boostItem.setState(BoostItem.eState.OFF);
				Constant.PlayerData.addCoin(boostItem.getPrice());
			}
		}
		mainMenu_.update();
	}

	public IEnumerator show(int type, int level, DialogManager dialogManager)
	{
		Input.enable = false;
		noPaymentItemType = -1;
		dialogManager_ = dialogManager;
		type_ = type;
		level_ = level;
		int clearCount2 = 0;
		int tutorialNo = -1;
		BossDataTable bossDataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<BossDataTable>();
		BossListData.BossLevelData bossData = bossDataTable.getBossData().bossList[type].bossLevelList[level - 1];
		clearCount2 = bossData.clearCount;
		if (bossDataTable.getBossData().bossList[0].bossLevelList[1].clearCount <= 0 || bossDataTable.getBossData().bossList[1].bossLevelList[0].clearCount <= 0)
		{
			int tutorialPlay = PlayerPrefs.GetInt("boss_powerUp_tutorial_Play", 0);
			if (type == 0 && level == 2 && clearCount2 == 0 && (tutorialPlay == 0 || tutorialPlay == 1))
			{
				tutorialNo = -15;
				if (tutorialPlay == 0)
				{
					PlayerPrefs.SetInt("boss_powerUp_tutorial_Play", 1);
				}
			}
			else if (type == 1 && level == 1 && clearCount2 == 0 && (tutorialPlay == 0 || tutorialPlay == 2))
			{
				tutorialNo = -15;
				if (tutorialPlay == 0)
				{
					PlayerPrefs.SetInt("boss_powerUp_tutorial_Play", 2);
				}
			}
		}
		GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		StageDataTable stageTbl = dataTable.GetComponent<StageDataTable>();
		bossInfo_ = stageTbl.getBossInfo(type_);
		setupUI();
		if (tutorialNo != -1)
		{
			if (TutorialManager.Instance.isTutorial(tutorialNo, TutorialDataTable.ePlace.Setup))
			{
				noPaymentItemType = TutorialManager.Instance.getItemType(tutorialNo);
			}
			else
			{
				noPaymentItemType = -1;
			}
		}
		yield return dialogManager_.StartCoroutine(_showBoss(30001 + level));
		Input.enable = true;
		if (noPaymentItemType != -1)
		{
			yield return StartCoroutine(playTutorial(tutorialNo, bossInfo_.Common));
		}
	}

	private bool isMoveProd(BossStageInfo.eBossType currentLevel, Part_Map.ResultParam resultParam)
	{
		if (currentLevel == BossStageInfo.eBossType.Owl)
		{
			return false;
		}
		if (resultParam != null && resultParam.IsRetry && Constant.Boss.isBossStage(resultParam.StageNo))
		{
			return false;
		}
		int num = Constant.Boss.convLevelToNo(currentLevel);
		int prevStageNo = getPrevStageNo(currentLevel);
		if (!Bridge.StageData.isClear(prevStageNo))
		{
			return false;
		}
		if (otherData_.isPlayedEventStageProd(num))
		{
			return false;
		}
		return true;
	}

	private bool isLastStage(int stageNo)
	{
		if (stageNo == Constant.Boss.getHighestLevelStageNo())
		{
			return true;
		}
		return false;
	}

	private int getPrevStageNo(BossStageInfo.eBossType level)
	{
		int num = Constant.Boss.convLevelToNo(level);
		return Mathf.Max(num - 1, Constant.Boss.getLeastLevelStageNo());
	}

	private BossStageInfo.eBossType getPrevLevel(BossStageInfo.eBossType level)
	{
		return (BossStageInfo.eBossType)Mathf.Max((int)(level - 1), 0);
	}

	private void setupUI()
	{
		UI uI = ui_;
		uI.Item.text = msgRes_.getMessage(1462);
		uI.TargetLabel.text = msgRes_.getMessage(8009);
		window_.transform.Find("plate_boss_00").GetComponent<UISprite>().spriteName = "Bos_setup_plate_boss_" + type_.ToString("00");
		bool flag = true;
		setupPlayButton(uI.PlayButton, uI.PlayButtonBG, flag);
		if (flag)
		{
			uI.ItemsRoot.gameObject.SetActive(true);
			if (type_ == 0 && level_ == 1)
			{
				bossInfo_.Common.ItemNum = 0;
			}
			else
			{
				bossInfo_.Common.ItemNum = bossInfo_.Common.Items.Length;
			}
			setItemPos(uI.ItemsRoot, bossInfo_.Common);
			setupItem(uI.Items, bossInfo_.Common);
			judgeNewIconItem();
			for (int i = 0; i < uI.Items.Length; i++)
			{
				BoostItem boostItem = uI.Items[i];
				boostItem.setState(BoostItem.eState.OFF);
			}
		}
		else
		{
			uI.ItemsRoot.gameObject.SetActive(false);
		}
		uI.HelpButton.gameObject.SetActive(flag);
		if (bossInfo_.Common.ItemNum > 0 && flag)
		{
			NGUIUtility.enable(uI.HelpButton, false);
		}
		else
		{
			NGUIUtility.disable(uI.HelpButton, false);
		}
		window_.transform.Find("Labels/Boss_level_numder").GetComponent<UILabel>().text = level_.ToString();
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

	private void setCurrentWindow(int index)
	{
		Vector3 localPosition = windowRoot_.transform.localPosition;
		localPosition.x = getWindowMovePos(index).x;
		windowRoot_.transform.localPosition = localPosition;
	}

	private Vector3 getWindowMovePos(int index)
	{
		return -window_.transform.localPosition;
	}

	public void UpdateItems()
	{
		BoostItem[] items = ui_.Items;
		foreach (BoostItem boostItem in items)
		{
			boostItem.setState(BoostItem.eState.OFF);
		}
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		UI ui = ui_;
		if (trigger.name.Contains("item_Button"))
		{
			Constant.SoundUtil.PlayButtonSE();
			BoostItem item2 = trigger.transform.parent.GetComponent<BoostItem>();
			yield return StartCoroutine(pressItemButton(ui.Items, item2));
			if (item2.getState() != BoostItem.eState.OFF)
			{
				yield break;
			}
			BoostItem[] items = ui.Items;
			foreach (BoostItem temp in items)
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
			StartCoroutine(pressHelpButton(trigger, bossInfo_.Common));
			break;
		case "Play_Button":
		{
			Constant.SoundUtil.PlayDecideSE();
			Input.enable = false;
			bPossible_ = true;
			yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
			yield return StartCoroutine(dataTable_.downloadResourceURLData(true, false));
			if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Cancel)
			{
				Input.enable = true;
				break;
			}
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			if (Bridge.PlayerData.getHeart() < 1)
			{
				Input.enable = true;
				bPossible_ = false;
				yield return StartCoroutine(base.show(eType.Heart));
				break;
			}
			yield return StartCoroutine(playBoss(type_, level_, ui.Items));
			if (!bPossible_)
			{
				Input.enable = true;
				break;
			}
			BoostItem[] items2 = ui.Items;
			foreach (BoostItem item in items2)
			{
				if (item.getState() == BoostItem.eState.ON)
				{
					if (item.getPriceType() == Constant.eMoney.Jewel)
					{
						Tapjoy.TrackEvent("Money", "Expense Jewel", "Item Lobby Jewel", null, item.getPrice());
						Tapjoy.TrackEvent("Game Item", "Lobby Jewel", "Boss Stage", item.getItemType().ToString(), "Use Jewel", item.getPrice(), null, 0L, null, 0L);
						GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Jewel", "Item Lobby Jewel", item.getPrice());
						GlobalGoogleAnalytics.Instance.LogEvent("Item Lobby Jewel", item.getItemType().ToString(), "Boss Stage", item.getPrice());
					}
					else if (item.getPriceType() == Constant.eMoney.Coin)
					{
						Tapjoy.TrackEvent("Money", "Expense Coin", "Item Lobby Coin", null, item.getPrice());
						Tapjoy.TrackEvent("Game Item", "Lobby Coin", "Boss Stage", item.getItemType().ToString(), "Use Coin", item.getPrice(), null, 0L, null, 0L);
						GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Coin", "Item Lobby Coin", item.getPrice());
						GlobalGoogleAnalytics.Instance.LogEvent("Item Lobby Coin", item.getItemType().ToString(), "Boss Stage", item.getPrice());
					}
				}
			}
			HeartEffect heartEff = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.HeartEffect).GetComponent<HeartEffect>();
			heartEff.gameObject.SetActive(true);
			yield return StartCoroutine(heartEff.play(ui.PlayButton.transform.localPosition));
			heartEff.gameObject.SetActive(false);
			while (NetworkMng.Instance.isShowIcon())
			{
				yield return 0;
			}
			KeyBubbleData keyData = GlobalData.Instance.getKeyBubbleData();
			if (keyData != null)
			{
				keyData.keyBubbleCount = 0;
				GlobalData.Instance.setKeyBubbleData(keyData);
			}
			BossDataTable bossTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<BossDataTable>();
			BossListData bossData = bossTbl.getBossData();
			bossData.bossList[type_].bossLevelList[level_ - 1].playCount++;
			bossTbl.setBossData(bossData);
			Hashtable args = createBossArgs(bossInfo_.BossInfo.BossType, level_, ui.Items);
			partManager_.requestTransition(PartManager.ePart.BossStage, args, FadeMng.eType.Cutout, true);
			Input.enable = true;
			break;
		}
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeBossSetDialog(this));
			break;
		}
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
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
		while (dialog.isOpen())
		{
			yield return 0;
		}
	}

	public override IEnumerator changeItem(int index, StageInfo.Item itemInfo, Hashtable args)
	{
		BoostItem item = ui_.Items[index];
		yield return StartCoroutine(item.change(itemInfo, args));
	}

	public int getSetItemCoin()
	{
		return totalAmount(ui_.Items, Constant.eMoney.Coin);
	}

	private IEnumerator playTutorial(int stageNo, StageInfo.CommonInfo stageInfo)
	{
		TutorialManager.Instance.bItemTutorial = false;
		if (stageInfo.FreeItems.Length > 0 && TutorialManager.Instance.isTutorial(stageNo, TutorialDataTable.ePlace.Setup))
		{
			GameObject uiRoot = dialogManager_.getCurrentUiRoot();
			TutorialManager.Instance.load(stageNo, uiRoot);
			yield return StartCoroutine(TutorialManager.Instance.play(stageNo, TutorialDataTable.ePlace.Setup, uiRoot, stageInfo, null));
		}
	}

	private void judgeNewIconItem()
	{
		int[] array = new int[2];
		BoostItem[] items = getItems();
		BossDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<BossDataTable>();
		BossListData.BossLevelData bossLevelData = component.getBossData().bossList[type_].bossLevelList[level_ - 1];
		array = bossLevelData.usedItemType;
		int @int = PlayerPrefs.GetInt("boss_powerUp_tutorial_Play", 0);
		Debug.Log("showStage:" + @int);
		Debug.Log("type_:" + type_);
		for (int i = 0; i < items.Length; i++)
		{
			bool flag = false;
			if (array != null)
			{
				int[] array2 = array;
				foreach (int num in array2)
				{
					if (items[i].getItemType() == (Constant.Item.eType)num)
					{
						flag = true;
						break;
					}
				}
			}
			bool flag2 = false;
			if (@int - 1 == type_ && ((type_ == 0 && level_ == 2) || (type_ == 1 && level_ == 1)))
			{
				Debug.Log("bUsed:" + flag);
				Debug.Log("bossData.clearCount:" + bossLevelData.clearCount);
				flag2 = items[i].getItemType() != Constant.Item.eType.Invalid;
			}
			items[i].setUseNewIconFlg(flag2 && !flag && bossLevelData.clearCount < 1);
		}
	}

	public BoostItem[] getItems()
	{
		return ui_.Items;
	}
}
