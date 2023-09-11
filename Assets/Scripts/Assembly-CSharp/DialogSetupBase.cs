using System.Collections;
using System.Collections.Generic;
using Bridge;
using LitJson;
using Network;
using UnityEngine;

public abstract class DialogSetupBase : DialogShortageBase
{
	private const float NoneItemPosY = -110f;

	private const float DefaultItemPosY = -100f;

	protected SaveGameData gameData_;

	protected MainMenu mainMenu_;

	protected MessageResource msgRes_;

	protected bool bPossible_;

	protected DialogPlayScore scoreDialog_;

	protected DialogBossReward bossReward_;

	protected int stageNo;

	public bool waitItemPress;

	private List<BoostItem> buyItemList_ = new List<BoostItem>(4);

	private StageBeginData stageResponce_ = new StageBeginData();

	protected UILabel itemLabel_;

	protected int _drop_minilen_id = -1;

	protected int _drop_minilen_rand = -1;

	protected Dictionary<Constant.Item.eType, int> ticketTable = new Dictionary<Constant.Item.eType, int>();

	public override void OnCreate()
	{
		createCB();
		scoreDialog_ = dialogManager_.getDialog(DialogManager.eDialog.PlayScore) as DialogPlayScore;
		gameData_ = SaveData.Instance.getGameData();
		msgRes_ = MessageResource.Instance;
		mainMenu_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
	}

	protected void setupTargetLabel(UILabel label, StageInfo.CommonInfo info)
	{
		label.text = Constant.MessageUtil.getTargetMsg(info, msgRes_, Constant.MessageUtil.eTargetType.Setup);
	}

	protected IEnumerator pressHelpButton(GameObject trigger, StageInfo.CommonInfo commonInfo)
	{
		if (trigger.name == "ItemDetail_Button")
		{
			Constant.SoundUtil.PlayButtonSE();
			DialogItemHelp itemHelp = dialogManager_.getDialog(DialogManager.eDialog.ItemHelp) as DialogItemHelp;
			itemHelp.setup(commonInfo);
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(itemHelp));
		}
	}

	protected Hashtable createArgs(int stageNo, BoostItem[] items)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("StageResponce", stageResponce_);
		hashtable.Add("StageNo", stageNo);
		for (int i = 0; i < buyItemList_.Count; i++)
		{
			hashtable.Add("item_" + i + "type", buyItemList_[i].getItemType());
			hashtable.Add("item_" + i + "num", buyItemList_[i].getNum());
		}
		hashtable.Add("parkMinilenDropId", _drop_minilen_id);
		hashtable.Add("parkMinilenDropSeed", _drop_minilen_rand);
		return hashtable;
	}

	protected Hashtable createBossArgs(int type, int level, BoostItem[] items)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("StageResponce", stageResponce_);
		hashtable.Add("BossType", type);
		hashtable.Add("BossLevel", level);
		for (int i = 0; i < buyItemList_.Count; i++)
		{
			hashtable.Add("item_" + i + "type", buyItemList_[i].getItemType());
			hashtable.Add("item_" + i + "num", buyItemList_[i].getNum());
		}
		return hashtable;
	}

	protected IEnumerator pressItemButton(BoostItem[] items, BoostItem item)
	{
		if (item.getState() == BoostItem.eState.ON)
		{
			setItemLabelText(1462, 0);
			item.setState(BoostItem.eState.OFF);
			if (item.getPriceType() == Constant.eMoney.Coin)
			{
				Constant.PlayerData.addCoin(item.getPrice());
			}
			else if (item.getPriceType() == Constant.eMoney.Ticket && ticketTable.ContainsKey(item.getItemType()))
			{
				item.updateTicketCountLabel(ticketTable[item.getItemType()]);
			}
		}
		else
		{
			while (waitItemPress)
			{
				yield return null;
			}
			setItemLabelText((int)(item.getItemType() + 1250 - 1), item.getNum());
			item.setState(BoostItem.eState.ON);
			Constant.eMoney type = item.getPriceType();
			int total = item.getPrice();
			int result = 0;
			switch (type)
			{
			case Constant.eMoney.Jewel:
				result = Bridge.PlayerData.getJewel() - total;
				break;
			case Constant.eMoney.Coin:
				result = Bridge.PlayerData.getCoin() - total;
				break;
			}
			if (result < 0)
			{
				setItemLabelText(1462, 0);
				item.setState(BoostItem.eState.OFF);
				if (type == Constant.eMoney.Coin)
				{
					yield return StartCoroutine(show(eType.Coin));
				}
				else
				{
					yield return StartCoroutine(show(eType.Jewel));
				}
				yield break;
			}
			if (item.getPriceType() == Constant.eMoney.Coin)
			{
				Constant.PlayerData.addCoin(-item.getPrice());
			}
			else if (item.getPriceType() == Constant.eMoney.Ticket && ticketTable.ContainsKey(item.getItemType()))
			{
				item.updateTicketCountLabel(ticketTable[item.getItemType()] - 1);
			}
		}
		mainMenu_.update();
	}

	protected IEnumerator pressNoItemButton(int addItemStageNo, int index, StageInfo.CommonInfo commonInfo)
	{
		Hashtable args = new Hashtable();
		args["AddItemStageNo"] = addItemStageNo;
		args["AddItemIndex"] = index;
		GameObject uiRoot = dialogManager_.getCurrentUiRoot();
		TutorialManager.Instance.load(-6, uiRoot);
		int tutorialStageNo = -6;
		yield return StartCoroutine(TutorialManager.Instance.play(tutorialStageNo, TutorialDataTable.ePlace.Setup, uiRoot, commonInfo, args));
		TutorialManager.Instance.unload(-6);
	}

	protected void setItemPos(Transform itemsRoot, StageInfo.CommonInfo commonInfo)
	{
		Vector3 localPosition = itemsRoot.transform.localPosition;
		if (commonInfo.ItemNum > 0)
		{
			localPosition.y = -100f;
		}
		else
		{
			localPosition.y = -110f;
		}
		itemsRoot.transform.localPosition = localPosition;
	}

	protected void setupItem(BoostItem[] items, StageInfo.CommonInfo commonInfo)
	{
		setItemLabelText(1462, 0);
		for (int i = 0; i < items.Length; i++)
		{
			BoostItem boostItem = items[i];
			if (i >= commonInfo.ItemNum)
			{
				boostItem.setup(Constant.Item.eType.Invalid, 0);
				boostItem.noneItem();
				boostItem.disable();
			}
			else
			{
				boostItem.enable();
				StageInfo.Item item = commonInfo.Items[i];
				boostItem.setup((Constant.Item.eType)item.Type, item.Num);
				boostItem.setPrice((Constant.eMoney)item.PriceType, item.Price);
			}
		}
	}

	protected void setupPricelessTicket(BoostItem[] items, StageInfo.CommonInfo commonInfo, int noPaymentNo)
	{
		ticketTable.Clear();
		GameData gameData = GlobalData.Instance.getGameData();
		if (gameData.userItemList != null)
		{
			UserItemList[] userItemList = gameData.userItemList;
			foreach (UserItemList userItemList2 in userItemList)
			{
				ticketTable.Add((Constant.Item.eType)userItemList2.itemType, userItemList2.count);
			}
		}
		foreach (BoostItem boostItem in items)
		{
			if (ticketTable.ContainsKey(boostItem.getItemType()) && noPaymentNo != (int)boostItem.getItemType() && ticketTable[boostItem.getItemType()] > 0)
			{
				boostItem.setPriceUsedFreeTicket(Constant.eMoney.Ticket, 0, ticketTable[boostItem.getItemType()]);
			}
		}
	}

	protected void setupClearCondition(Transform window, StageInfo.CommonInfo commonInfo)
	{
		int num;
		if (commonInfo.IsMinilenDelete)
		{
			num = ((commonInfo.Time <= 0) ? 6 : 7);
		}
		else
		{
			num = ((!commonInfo.IsFriendDelete) ? ((commonInfo.IsAllDelete || commonInfo.IsFulcrumDelete) ? 1 : 2) : 0);
			if (commonInfo.Time > 0)
			{
				num += 3;
			}
		}
		Transform transform = null;
		for (int i = 0; i < 8; i++)
		{
			transform = window.Find("condition_icon/" + i.ToString("00"));
			if ((bool)transform)
			{
				transform.gameObject.SetActive(i == num);
			}
		}
		int num2 = ((commonInfo.IsAllDelete || commonInfo.IsFulcrumDelete) ? 2 : ((!commonInfo.IsFriendDelete) ? 1 : 0));
		for (int j = 0; j < 3; j++)
		{
			window.Find("setup_paper/paper_0" + j).gameObject.SetActive(j == num2);
		}
	}

	protected int totalAmount(BoostItem[] items, Constant.eMoney type)
	{
		int num = 0;
		foreach (BoostItem boostItem in items)
		{
			if (boostItem.getState() != BoostItem.eState.OFF && boostItem.getPriceType() == type)
			{
				num += boostItem.getPrice();
			}
		}
		return num;
	}

	protected IEnumerator play(int stageNo, BoostItem[] items)
	{
		buyItemList_.Clear();
		foreach (BoostItem item in items)
		{
			if (item.getState() != BoostItem.eState.OFF)
			{
				buyItemList_.Add(item);
			}
		}
		int no = ((!Constant.Event.isEventStage(stageNo)) ? (stageNo + 1) : stageNo);
		NetworkMng.Instance.setup(Hash.StagePlay(no, buyItemList_));
		yield return StartCoroutine(NetworkMng.Instance.download(API.StagePlay, true, !Constant.Event.isEventStage(stageNo)));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			bPossible_ = false;
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		CommonData commonData = JsonMapper.ToObject<CommonData>(www.text);
		JsonData json = JsonMapper.ToObject(www.text);
		if (DailyMission.isTermClear())
		{
			Mission respons_mission2 = JsonMapper.ToObject<Mission>(www.text);
			GlobalData.Instance.setDailyMissionData(respons_mission2);
			Network.DailyMission dMission = GlobalData.Instance.getDailyMissionData();
			if (dMission == null)
			{
				NetworkMng.Instance.setup(Hash.DailyMissionCreate());
				yield return StartCoroutine(NetworkMng.Instance.download(API.DailyMissionCreate, false, false));
				WWW www_dMission = NetworkMng.Instance.getWWW();
				respons_mission2 = JsonMapper.ToObject<Mission>(www_dMission.text);
				GlobalData.Instance.setDailyMissionData(respons_mission2);
				DailyMission.bMissionCreate = true;
			}
		}
		GameData gameData = GlobalData.Instance.getGameData();
		gameData.setCommonData(commonData, true);
		gameData.setEventData(commonData);
		gameData.setDailyMissionData(commonData);
		gameData.continueNum = (int)json["continueNum"];
		gameData.helpMove = (int)json["helpMove"];
		gameData.helpTime = (int)json["helpTime"];
		gameData.gachaTicket = (int)json["gachaTicket"];
		EventMenu.updateGetTime();
		ChallengeMenu.updateGetTime();
		CollaborationMenu.updateGetTime();
		DailyMission.updateGetTime();
		ContinueSaleData continueData = GlobalData.Instance.getContinueData();
		continueData.isContinueCampaign = (bool)json["isContinueCampaign"];
		if (continueData.isContinueCampaign)
		{
			continueData.continueCampaignSale = (int)json["continueCampaignSale"];
		}
		continueData.isContinueChance = (bool)json["isContinueChance"];
		if (continueData.isContinueChance)
		{
			continueData.continueChanceSale = (int)json["continueChanceSale"];
		}
		ComboBonusDataTable comboBonusTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ComboBonusDataTable>();
		ComboBonusDataList comboBonusDataList = JsonMapper.ToObject<ComboBonusDataList>(www.text);
		comboBonusTbl.info_.BonusInfos = new ComboBonusInfo.BonusInfo[comboBonusDataList.comboBonusList.Length];
		for (int i = 0; i < comboBonusDataList.comboBonusList.Length; i++)
		{
			ComboBonusData data = comboBonusDataList.comboBonusList[i];
			comboBonusTbl.info_.BonusInfos[i] = new ComboBonusInfo.BonusInfo();
			comboBonusTbl.info_.BonusInfos[i].CountMin = data.countMin;
			comboBonusTbl.info_.BonusInfos[i].CountMax = data.countMax;
			comboBonusTbl.info_.BonusInfos[i].CoinNum = data.coinNum;
		}
		if (!Constant.Event.isEventStage(stageNo))
		{
			gameData.helpDataSize = 0;
		}
		stageResponce_ = JsonMapper.ToObject<StageBeginData>(www.text);
		KeyBubbleData keyData = GlobalData.Instance.getKeyBubbleData();
		Network.StageData stageData = GlobalData.Instance.getStageData(stageNo);
		gameData.userItemList = stageResponce_.userItemList;
		if (stageData.usedItemType == null)
		{
			stageData.usedItemType = new int[0];
		}
		List<int> usedItemList = new List<int>(stageData.usedItemType);
		foreach (BoostItem buyItem in buyItemList_)
		{
			int itemType = (int)buyItem.getItemType();
			if (!usedItemList.Contains(itemType))
			{
				usedItemList.Add(itemType);
			}
		}
		stageData.usedItemType = usedItemList.ToArray();
		stageData.playCount++;
		mainMenu_.getHeartMenu().updateRemainingTime();
		mainMenu_.update();
		_drop_minilen_id = 0;
		_drop_minilen_rand = 0;
		if (Constant.ParkStage.isParkStage(stageNo))
		{
			_drop_minilen_id = (int)json["parkMinilenDropId"];
			_drop_minilen_rand = (int)json["parkMinilenDropSeed"];
		}
	}

	protected IEnumerator playBoss(int bossType, int bossLevel, BoostItem[] items)
	{
		buyItemList_.Clear();
		foreach (BoostItem item in items)
		{
			if (item.getState() != BoostItem.eState.OFF)
			{
				buyItemList_.Add(item);
			}
		}
		NetworkMng.Instance.setup(Hash.BossStagePlay(Constant.Boss.convBossInfoToNo(bossType, 0), bossLevel, buyItemList_));
		yield return StartCoroutine(NetworkMng.Instance.download(API.BossStagePlay, true, false));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			bPossible_ = false;
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		CommonData commonData = JsonMapper.ToObject<CommonData>(www.text);
		JsonData json = JsonMapper.ToObject(www.text);
		GameData gameData = GlobalData.Instance.getGameData();
		gameData.bonusJewel = commonData.bonusJewel;
		gameData.buyJewel = commonData.buyJewel;
		gameData.lastStageStatus = commonData.lastStageStatus;
		gameData.lastStageNo = commonData.lastStageNo;
		gameData.treasureboxNum = commonData.treasureboxNum;
		gameData.heart = commonData.heart;
		gameData.coin = commonData.coin;
		gameData.exp = commonData.exp;
		gameData.level = commonData.level;
		gameData.progressStageNo = commonData.progressStageNo;
		gameData.allPlayCount = commonData.allPlayCount;
		gameData.allClearCount = commonData.allClearCount;
		gameData.allStarSum = commonData.allStarSum;
		gameData.allStageScoreSum = commonData.allStageScoreSum;
		gameData.heartRecoverySsRemaining = commonData.heartRecoverySsRemaining;
		gameData.isCoinCampaign = commonData.isCoinCampaign;
		gameData.isJewelCampaign = commonData.isJewelCampaign;
		gameData.isHeartCampaign = commonData.isHeartCampaign;
		gameData.heartRecoverTime = commonData.heartRecoverTime;
		gameData.isCoinupCampaign = commonData.isCoinupCampaign;
		gameData.isHeartSendCampaign = commonData.isHeartSendCampaign;
		gameData.heartSendHour = commonData.heartSendHour;
		gameData.isHeartShopCampaign = commonData.isHeartShopCampaign;
		gameData.isBossOpen = commonData.isBossOpen;
		gameData.saleStageItemArea = commonData.saleStageItemArea;
		gameData.stageItemAreaSalePercent = commonData.stageItemAreaSalePercent;
		gameData.isStageItemAreaCampaign = commonData.isStageItemAreaCampaign;
		gameData.setEventData(commonData);
		gameData.setDailyMissionData(commonData);
		gameData.continueNum = (int)json["continueNum"];
		ContinueSaleData continueData = GlobalData.Instance.getContinueData();
		continueData.isContinueCampaign = (bool)json["isContinueCampaign"];
		if (continueData.isContinueCampaign)
		{
			continueData.continueCampaignSale = (int)json["continueCampaignSale"];
		}
		continueData.isContinueChance = (bool)json["isContinueChance"];
		if (continueData.isContinueChance)
		{
			continueData.continueChanceSale = (int)json["continueChanceSale"];
		}
		EventMenu.updateGetTime();
		ChallengeMenu.updateGetTime();
		CollaborationMenu.updateGetTime();
		DailyMission.updateGetTime();
		mainMenu_.getHeartMenu().updateRemainingTime();
		mainMenu_.update();
	}

	protected IEnumerator _show(int stageNo)
	{
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(getDialogType()));
		dialogManager_.StartCoroutine(scoreDialog_.show(stageNo));
		yield return StartCoroutine(dialogManager_.waitDialogAnimation(this));
		yield return StartCoroutine(dialogManager_.waitDialogAnimation(scoreDialog_));
	}

	protected IEnumerator _showBoss(int stageNo)
	{
		dialogManager_.StartCoroutine(dialogManager_.openDialog(getDialogType()));
		yield return StartCoroutine(dialogManager_.waitDialogAnimation(this));
		yield return null;
	}

	protected IEnumerator _showRankingStage()
	{
		dialogManager_.StartCoroutine(dialogManager_.openDialog(getDialogType()));
		yield return dialogManager_.StartCoroutine(scoreDialog_.showRankingStage());
		yield return StartCoroutine(dialogManager_.waitDialogAnimation(this));
	}

	public virtual IEnumerator changeItem(int index, StageInfo.Item itemInfo, Hashtable args)
	{
		yield break;
	}

	public void setItemLabelText(int msgNum, int num)
	{
		if (itemLabel_ != null)
		{
			itemLabel_.text = msgRes_.getMessage(msgNum);
			if (msgRes_.isCtrlCode(itemLabel_.text, 1))
			{
				itemLabel_.text = msgRes_.castCtrlCode(itemLabel_.text, 1, num.ToString());
			}
		}
	}

	protected void resetFreeTicket(BoostItem[] items)
	{
		for (int i = 0; i < items.Length; i++)
		{
			items[i].resetFreeBoostItem();
		}
	}
}
