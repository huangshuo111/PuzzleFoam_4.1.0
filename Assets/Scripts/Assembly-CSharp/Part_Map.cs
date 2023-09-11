using System;
using System.Collections;
using System.Collections.Generic;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using TnkAd;
using Toast.Analytics;
using UnityEngine;

public class Part_Map : PartBase
{
	public class ResultParam
	{
		public bool IsRetry;

		public bool IsExit;

		public bool IsClear;

		public int StageNo;

		public bool IsResultClose;

		public bool IsProgressOpen;

		public bool IsForceSendInactive;

		public bool IsBossOpen;

		public bool IsSendOneStage;

		public bool IsSendMap;

		public int SendStageNo;
	}

	private enum eListItem
	{
		Mail = 0,
		MailPresent = 1,
		Ranking = 2,
		RankingInvite = 3,
		DayRanking = 4,
		Invite = 5,
		Request = 6,
		RankingDummyFriend = 7,
		FriendHelp = 8,
		Max = 9
	}

	public enum MapMoveDirection
	{
		Up = 0,
		Down = 1,
		Init = 2,
		Jump = 3
	}

	private const int GACHA_OPEN_NUM = 6;

	private const int PARK_OPEN_NUM = 10;

	private const float TWEEN_MAP_MOVE_Y = 2600f;

	private MapCamera mapCamera_;

	private Camera uiCamera_;

	private CameraClearFlags clearFlags_ = CameraClearFlags.Nothing;

	private List<GameObject> globalObjList_ = new List<GameObject>();

	private GlobalRoot globalObj_;

	private GameObject[] listItems_ = new GameObject[9];

	private SaveGameData gameData_;

	private SaveOtherData otherData_;

	private StageIconParent stageIconParent_;

	private TreasureBoxParent boxParent_;

	private int boxId_ = -1;

	private TreasureBox openBox_;

	private int openBoxID_ = -1;

	private MainMenu mainMenu_;

	private StageIcon clearStage_;

	private Area[] areas_;

	private Area directArea_;

	private int mailNum_;

	private BagMenu bagMenu_;

	private GameObject map_;

	private EventMenu eventMenu_;

	private CrossMenu crossMenu_;

	private ADMenu adMenu_;

	private OfferwallMenu offerwallMenu_;

	private BossMenu bossMenu_;

	private Transform gachaButton_;

	private CollaborationMenu collaboMenu_;

	private ResultParam resultParam_ = new ResultParam();

	private bool bEventStage_;

	private bool bShowedNextSetup_;

	private bool bNextStage_;

	private bool bSetupDialogOpen_;

	private StageDataTable stageTbl_;

	private StageIconDataTable iconTbl_;

	private bool bButtonEnable_;

	private int mapNo_ = -1;

	private AreaCloud[] areaCloud_;

	private bool bInitialized;

	private List<UserData> friendsData = new List<UserData>();

	public DailyMission dailyMission;

	private GameObject sendOneButtonObjct;

	private UISprite chara_00;

	private UISprite chara_01;

	private UISprite chara_02;

	private UISprite chara_03;

	private bool blowcheck;

	public bool transBonus;

	public bool isBonusGamePlayed;

	private bool bRecommended;

	private bool showPlayerDataEneble = true;

	public bool bInactive;

	private StageIcon[] icons;

	public static bool bTransMap = true;

	private StageSkipData res_;

	private bool isPaymentStageClear;

	public bool bStageSkip_;

	private bool isExecuteNextStage;

	private void Update()
	{
		if (mainMenu_ != null && GameObject.Find("PB_TNKVidioHandler").GetComponent<PBTnkVidioHandler>().getRewardUpdate())
		{
			GameObject.Find("PB_TNKVidioHandler").GetComponent<PBTnkVidioHandler>().setRewardUpadate(false);
			mainMenu_.update();
		}
	}

	public override IEnumerator setup(Hashtable args)
	{
		blowcheck = ResourceLoader.Instance.isUseLowResource();
		bButtonEnable_ = false;
		GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		stageTbl_ = dataTable.GetComponent<StageDataTable>();
		globalObj_ = GlobalRoot.Instance;
		gameData_ = SaveData.Instance.getGameData();
		otherData_ = gameData_.getOtherData();
		analyzeHash(args);
		iconTbl_ = dataTable.GetComponent<StageIconDataTable>();
		StageIconData icon_data = null;
		if (resultParam_ != null)
		{
			bEventStage_ = Constant.Event.isEventStage(resultParam_.StageNo);
		}
		if (resultParam_.StageNo > getLastStageNo())
		{
			resultParam_.StageNo = getLastStageNo();
		}
		int playableCurrentStage = Bridge.PlayerData.getCurrentStage();
		if (resultParam_.IsForceSendInactive || (dailyMission != null && dailyMission.dailyMissionChangeCheck() && DailyMission.isTermClear()))
		{
			yield return StartCoroutine(forceRecover());
			playableCurrentStage = Bridge.PlayerData.getCurrentStage();
		}
		bNextStage_ = isNextStage(playableCurrentStage);
		bool bCurrentBack = false;
		if (isLastStage(playableCurrentStage - 1))
		{
			bNextStage_ = false;
		}
		if (bNextStage_)
		{
			if (!isQualifiedStarTerms(playableCurrentStage))
			{
				bNextStage_ = false;
			}
			playableCurrentStage--;
			bCurrentBack = true;
		}
		else if (isLastStage(playableCurrentStage - 1))
		{
			playableCurrentStage--;
		}
		if (otherData_.isFlag(SaveOtherData.eFlg.AllClear) && !isLastStage(playableCurrentStage))
		{
			playableCurrentStage--;
		}
		bool bLastStageClear = false;
		Network.StageData[] stageList = GlobalData.Instance.getGameData().stageList;
		if (stageList.Length > 0)
		{
			bLastStageClear = stageList[stageList.Length - 1].clearCount > 0;
		}
		int mapCurrentStage = ((!isPlayedStage() || args == null || bEventStage_) ? playableCurrentStage : resultParam_.StageNo);
		GameObject menu2 = appendGlobalObj(GlobalObjectParam.eObject.MapMainUI2);
		dailyMission = menu2.transform.Find("daily").GetComponent<DailyMission>();
		if (!isPlayedStage())
		{
			if (isNextStage(mapCurrentStage) && !bCurrentBack && !bLastStageClear)
			{
				mapCurrentStage--;
			}
			if (resultParam_.IsSendOneStage)
			{
				mapCurrentStage = 0;
			}
			if (resultParam_.IsSendMap)
			{
				mapCurrentStage = resultParam_.SendStageNo;
			}
		}
		else if (args != null && isShowNextSetup(false) && !resultParam_.IsRetry)
		{
			int nextSetupStageNo = getNextSetupStageNo(false);
			if (nextSetupStageNo != -1 && !isNextStage(nextSetupStageNo))
			{
				mapCurrentStage = nextSetupStageNo;
			}
		}
		mapNo_ = iconTbl_.getMapNoByStageNo(mapCurrentStage);
		icon_data = iconTbl_.getData(mapCurrentStage);
		map_ = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "BG_Panel_" + (mapNo_ + 1).ToString("00"))) as GameObject;
		map_.SetActive(false);
		Utility.setParent(map_, uiRoot.transform, false);
		yield return StartCoroutine(setupMapObject(map_, icon_data));
		map_.SetActive(true);
		mainMenu_ = appendGlobalObj(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		mainMenu_.update();
		setupButton(mainMenu_.gameObject, true);
		bagMenu_ = menu2.transform.Find("bag_menu").GetComponent<BagMenu>();
		eventMenu_ = menu2.transform.Find("event").GetComponent<EventMenu>();
		crossMenu_ = menu2.transform.Find("cross").GetComponent<CrossMenu>();
		adMenu_ = menu2.transform.Find("ad").GetComponent<ADMenu>();
		offerwallMenu_ = menu2.transform.Find("Offerwall").GetComponent<OfferwallMenu>();
		bossMenu_ = menu2.transform.Find("boss").GetComponent<BossMenu>();
		gachaButton_ = menu2.transform.Find("gacha");
		collaboMenu_ = menu2.transform.Find("collaboration").GetComponent<CollaborationMenu>();
		chara_00 = menu2.transform.Find("bag_menu/09_avatar/avatar_button/avatar/chara_00").GetComponent<UISprite>();
		chara_01 = menu2.transform.Find("bag_menu/09_avatar/avatar_button/avatar/chara_01").GetComponent<UISprite>();
		chara_02 = menu2.transform.Find("bag_menu/09_avatar/avatar_button/avatar/chara_02").GetComponent<UISprite>();
		chara_03 = menu2.transform.Find("bag_menu/09_avatar/avatar_button/avatar/chara_03").GetComponent<UISprite>();
		UpdateCharaIcon();
		dailyMissionValueSetting();
		if (dailyMission.gameObject.activeSelf)
		{
			yield return StartCoroutine(dailyMission.dailyMissionInfoSetup());
		}
		setMailNum(Bridge.PlayerData.getMailUnReadCount());
		setInviteRewardPopup();
		setupButton(menu2.gameObject, true);
		menu2.transform.Find("sendMap_Event").gameObject.SetActive(false);
		yield return StartCoroutine(getCrossMission(eventMenu_, crossMenu_, adMenu_, offerwallMenu_, PartManager.ePart.Map));
		eventMenu_.updateEnable(PartManager.ePart.Map);
		collaboMenu_.updateEnable(PartManager.ePart.Map);
		crossMenu_.updateEnable(PartManager.ePart.Map);
		adMenu_.updateEnable(PartManager.ePart.Map);
		offerwallMenu_.updateEnable(PartManager.ePart.Map);
		bossMenu_.updateEnable(PartManager.ePart.Map, partManager.fade);
		if (bossMenu_.gameObject.activeSelf && !otherData_.isFlag(SaveOtherData.eFlg.FirstBossOpen))
		{
			NetworkMng.Instance.setup(null);
			yield return StartCoroutine(NetworkMng.Instance.download(OnCreateKeyGetWWW, false, false, false, false));
			if (NetworkMng.Instance.getResultCode() == eResultCode.Success)
			{
				KeyBubbleData keyData = GlobalData.Instance.getKeyBubbleData();
				keyData.keyBubbleCount = keyData.keyBubbleMax;
				GlobalData.Instance.setKeyBubbleData(keyData);
				otherData_.setFlag(SaveOtherData.eFlg.FirstBossOpen, true);
				otherData_.setFlag(SaveOtherData.eFlg.RequestBossOpen, true);
				otherData_.setFlag(SaveOtherData.eFlg.RequestFirstBossOpen, true);
			}
			else if (NetworkMng.Instance.getResultCode() == eResultCode.AddedReward)
			{
				otherData_.setFlag(SaveOtherData.eFlg.FirstBossOpen, true);
			}
		}
		if (!otherData_.isFlag(SaveOtherData.eFlg.RequestBossOpen))
		{
			yield return StartCoroutine(bossMenu_.checkGateOpenEffect(otherData_.isFlag(SaveOtherData.eFlg.RequestBossOpen)));
		}
		if (!globalObj_.isAppend(GlobalObjectParam.eObject.Player))
		{
			globalObj_.load("Prefabs/", GlobalObjectParam.eObject.Player, false);
		}
		if (!globalObj_.isAppend(GlobalObjectParam.eObject.CurrentStageEffect))
		{
			globalObj_.load("Prefabs/", GlobalObjectParam.eObject.CurrentStageEffect, false);
		}
		boxParent_ = appendGlobalObj(GlobalObjectParam.eObject.TreasureBox).GetComponent<TreasureBoxParent>();
		stageIconParent_ = appendGlobalObj(GlobalObjectParam.eObject.StageIcon).GetComponent<StageIconParent>();
		mapCamera_ = appendGlobalObj(GlobalObjectParam.eObject.MapCamera).GetComponentsInChildren<MapCamera>(true)[0];
		mapCamera_.setMoveRange(mapCamera_.DefMoveRangeMax_[mapNo_], mapCamera_.DefMoveRange_[mapNo_]);
		GamePlayer player = appendGlobalObj(GlobalObjectParam.eObject.Player).GetComponent<GamePlayer>();
		StartCoroutine(player.loadTexture());
		if (GlobalData.Instance.getGameData().avatarList != null)
		{
			Network.Avatar[] avatarList = GlobalData.Instance.getGameData().avatarList;
			foreach (Network.Avatar avatar in avatarList)
			{
				if (avatar.wearFlg == 1)
				{
					DummyPlayerData.Data.avatarId = avatar.index;
					DummyPlayerData.Data.throwId = avatar.throwCharacter;
					DummyPlayerData.Data.supportId = avatar.supportCharacter;
				}
			}
		}
		setActiveGlobalObjs(true);
		GameObject main_2_obj = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI2);
		if ((bool)main_2_obj)
		{
			Transform park_button_trans = main_2_obj.transform.Find("sendOneStage/goToPark/park_Button");
			if ((bool)park_button_trans)
			{
				park_button_trans.gameObject.SetActive(true);
			}
		}
		areaCloud_ = new AreaCloud[2];
		for (int i = 0; i < 2; i++)
		{
			GameObject obj = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "BG_Panel_AreaCloud")) as GameObject;
			obj.SetActive(true);
			Utility.setParent(obj, uiRoot.transform, true);
			areaCloud_[i] = obj.GetComponent<AreaCloud>();
			areaCloud_[i].setup(base.gameObject, mapCamera_, i, mapNo_);
		}
		if (mapNo_ == 0)
		{
			areaCloud_[1].gameObject.SetActive(false);
		}
		else if (mapNo_ == iconTbl_.getMaxMapNum() - 1)
		{
			areaCloud_[0].gameObject.SetActive(false);
		}
		uiCamera_ = GameObject.Find("Camera").GetComponent<Camera>();
		clearFlags_ = uiCamera_.clearFlags;
		uiCamera_.clearFlags = CameraClearFlags.Nothing;
		if (!Bridge.StageData.isClear(0))
		{
			GameObject tap = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Tap_Panel")) as GameObject;
			Utility.setParent(tap, uiRoot.transform, false);
			tap.SetActive(true);
		}
		yield return StartCoroutine(loadDialog());
		clearStage_ = setupStageIcon(stageIconParent_, playableCurrentStage, bNextStage_);
		openBox_ = setupTreasureBox(boxParent_);
		boxParent_.gameObject.SetActive(false);
		areas_ = map_.GetComponentsInChildren<Area>(true);
		directArea_ = setupArea(bNextStage: otherData_.isFlag(SaveOtherData.eFlg.AllClear) || bNextStage_, areas: areas_, currentStage: playableCurrentStage);
		if (directArea_ != null)
		{
			directArea_.mapCamera_ = mapCamera_;
		}
		if (stageIconParent_.getIcon(playableCurrentStage) != null)
		{
			GameObject currentIcon = stageIconParent_.getIcon(playableCurrentStage).gameObject;
			showCurrentEffect(currentIcon);
			showPlayer(currentIcon);
		}
		else
		{
			player.gameObject.SetActive(false);
		}
		if (stageIconParent_.getIcon(mapCurrentStage) != null)
		{
			focus(mapCurrentStage);
		}
		sendOneButtonObjct = menu2.transform.Find("sendOneStage").gameObject;
		sendOneButtonObjct.GetComponentInChildren<UIButton>().isEnabled = true;
		checkStageSixClear();
		FriendUpdater.Instance.requestUpdate(partManager);
		while (FriendUpdater.Instance.isUpdate)
		{
			yield return null;
		}
		yield return StartCoroutine(updateFriends());
		StartCoroutine(updatePanelEnable());
		Transform toastPromotion_ = bagMenu_.transform.Find("toastpromotion_button");
		if (toastPromotion_ != null)
		{
			if (GameAnalytics.isPromotionAvailable() && GlobalData.Instance.getGameData().ToastPromotion)
			{
				toastPromotion_.gameObject.SetActive(true);
				toastPromotion_.localPosition = new Vector3(300f, (!dailyMission.gameObject.activeSelf) ? (-50) : 0, 0f);
			}
			else
			{
				toastPromotion_.gameObject.SetActive(false);
			}
		}
		Vungle.onAdFinishedEvent += VungleAdFinishedEvent;
		DialogSetup setupDialog = dialogManager.getDialog(DialogManager.eDialog.Setup) as DialogSetup;
		setupDialog.bButtonEnable_ = false;
		if (!isOpenTreasureEventToMapIn())
		{
			yield return StartCoroutine(showNextSetup(true));
		}
		StartCoroutine(execute(args));
	}

	private IEnumerator execute(Hashtable args)
	{
		while (!Input.enable)
		{
			partManager.loginInputBack();
			yield return 0;
		}
		yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		if (partManager.prevPart == PartManager.ePart.EventMap || partManager.prevPart == PartManager.ePart.CollaborationMap)
		{
			yield return StartCoroutine(playOpenCloudEff());
		}
		else if (partManager.prevPart == PartManager.ePart.ChallengeMap)
		{
			yield return StartCoroutine(playOpenCloudEff());
		}
		partManager.bTransitionMap_ = false;
		showPlayerDataEneble = false;
		Sound.Instance.playBgm(Sound.eBgm.BGM_010_Map, true);
		if (!isPlayedStage() && (partManager.prevPart == PartManager.ePart.Title || partManager.prevPart == PartManager.ePart.Stage || partManager.prevPart == PartManager.ePart.Scenario))
		{
			GameAnalytics.traceFriendCount(SNSCore.Instance.friends.Count);
			if (GlobalData.Instance.getGameData().TNKBanner)
			{
				Plugin.Instance.prepareInterstitialAd("PB_Cross_ad");
				Plugin.Instance.showInterstitialAd("PB_Cross_ad");
			}
			Plugin.Instance.prepareInterstitialAd("PB_Interstitial_AD");
			Plugin.Instance.showInterstitialAd("PB_Interstitial_AD");
			Plugin.Instance.prepareInterstitialAd("PB_EXIT_AD", "PB_TNKExitHandler");
			yield return StartCoroutine(showComeback());
			if (GlobalData.Instance.getGameData().attendance != null)
			{
				while (dialogManager.getActiveDialogNum() > 0)
				{
					yield return 0;
				}
				DialogAttendCheck dialog = dialogManager.getDialog(DialogManager.eDialog.AttendCheck) as DialogAttendCheck;
				dialog.setup(GlobalData.Instance.getGameData().attendance);
				yield return StartCoroutine(dialogManager.openDialog(dialog));
				while (dialog.isOpen())
				{
					yield return 0;
				}
				setMailNum(Bridge.PlayerData.getMailUnReadCount() + 1);
			}
			yield return StartCoroutine(showInformation());
			yield return StartCoroutine(openWeekRanking());
			yield return StartCoroutine(friendHelp());
		}
		if (Bridge.PlayerData.getCurrentStage() > 2)
		{
			yield return StartCoroutine(partManager.OpenMarketReviewDialog());
		}
		int stageNo = getNextSetupStageNo(true);
		yield return StartCoroutine(playTutorial(stageNo));
		yield return StartCoroutine(recommendedItem(stageNo));
		yield return StartCoroutine(specialItem(stageNo));
		DialogSetup setupDialog = dialogManager.getDialog(DialogManager.eDialog.Setup) as DialogSetup;
		setupDialog.bButtonEnable_ = true;
		while (dialogManager.getActiveDialogNum() >= 1 || bStageSkip_)
		{
			yield return 0;
		}
		bButtonEnable_ = false;
		if (bossMenu_ != null)
		{
			if (otherData_.isFlag(SaveOtherData.eFlg.RequestBossOpen))
			{
				if (otherData_.isFlag(SaveOtherData.eFlg.RequestFirstBossOpen))
				{
					otherData_.setFlag(SaveOtherData.eFlg.RequestFirstBossOpen, false);
					yield return StartCoroutine(playBossTutorial(TutorialDataTable.eSPTutorial.FirstBossOpen));
					bossMenu_.updateKeyLabel();
				}
				else if (!otherData_.isFlag(SaveOtherData.eFlg.FirstBossOpenKeyGet))
				{
					otherData_.setFlag(SaveOtherData.eFlg.FirstBossOpenKeyGet, true);
					yield return StartCoroutine(playBossTutorial(TutorialDataTable.eSPTutorial.BossOpenKey));
				}
				yield return StartCoroutine(bossMenu_.checkGateOpenEffect(otherData_.isFlag(SaveOtherData.eFlg.RequestBossOpen)));
				otherData_.setFlag(SaveOtherData.eFlg.RequestBossOpen, false);
				otherData_.save();
			}
			else if (bossMenu_.gameObject.activeSelf && bossMenu_.isKeyShortage() && !otherData_.isFlag(SaveOtherData.eFlg.BossMenu))
			{
				otherData_.setFlag(SaveOtherData.eFlg.BossMenu, true);
				yield return StartCoroutine(playBossTutorial(TutorialDataTable.eSPTutorial.KeyShortage));
				otherData_.save();
			}
		}
		if (!isExecuteNextStage)
		{
			yield return StartCoroutine(nextStage());
		}
		isExecuteNextStage = false;
		int GACHA_FIRST_OPEN_STAGE_NO = 6;
		int cStage = Bridge.PlayerData.getCurrentStage();
		if (!dailyMission.bonusgamePlayed && DailyMission.bMissionCreate)
		{
			yield return StartCoroutine(showDailyMission());
		}
		if (collaboMenu_.gameObject.activeSelf && !otherData_.isFlag(SaveOtherData.eFlg.TutorialGachaMukCollabo))
		{
			bool isInputOperation3 = Input.enableCount < 1;
			if (isInputOperation3)
			{
				Input.enable = true;
			}
			BoxCollider col = collaboMenu_.GetComponentInChildren<BoxCollider>();
			col.enabled = false;
			collaboMenu_.transform.localPosition += Vector3.back * 100f;
			TutorialManager.Instance.load(-17, uiRoot);
			yield return StartCoroutine(TutorialManager.Instance.play(-17, TutorialDataTable.ePlace.ReadyGoEnd, uiRoot, null, null));
			TutorialManager.Instance.unload(-17);
			collaboMenu_.transform.localPosition += Vector3.forward * 100f;
			col.enabled = true;
			if (isInputOperation3)
			{
				Input.enable = false;
			}
			otherData_.setFlag(SaveOtherData.eFlg.TutorialGachaMukCollabo, true);
			otherData_.save();
		}
		if (!otherData_.isFlag(SaveOtherData.eFlg.FirstGachaOpen) && cStage >= GACHA_FIRST_OPEN_STAGE_NO)
		{
			bool isInputOperation2 = Input.enableCount < 1;
			if (isInputOperation2)
			{
				Input.enable = true;
			}
			yield return StartCoroutine(openFirstGachaDialog());
			if (isInputOperation2)
			{
				Input.enable = false;
			}
		}
		while (dialogManager.getActiveDialogNum() > 0)
		{
			yield return 0;
		}
		if (!otherData_.isFlag(SaveOtherData.eFlg.OpenParkAnnounce) && cStage >= 10)
		{
			bool isInputOperation = Input.enableCount < 1;
			if (isInputOperation)
			{
				Input.enable = true;
			}
			yield return StartCoroutine(openParkAnnounce());
			if (isInputOperation)
			{
				Input.enable = false;
			}
		}
		while (dialogManager.getActiveDialogNum() > 0)
		{
			yield return 0;
		}
		if (DailyMission.isTermClear() && !dailyMission.bonusgamePlayed && dailyMission.missionCleared)
		{
			yield return StartCoroutine(showDailyMissionCleared());
		}
		if (!transBonus)
		{
			yield return StartCoroutine(showNextSetup(false));
			yield return StartCoroutine(playTutorial(getNextSetupStageNo(false)));
			bButtonEnable_ = true;
			bInitialized = true;
			showPlayerDataEneble = true;
		}
	}

	public bool isInitialized()
	{
		return bInitialized;
	}

	private IEnumerator openWeekRanking()
	{
		while (dialogManager.getActiveDialogNum() > 0)
		{
			yield return 0;
		}
		if (Bridge.PlayerData.getCurrentStage() <= 0)
		{
			yield break;
		}
		SaveNetworkData netData = SaveData.Instance.getGameData().getNetworkData();
		bool bPrevious = netData.isShowRanking();
		DialogRanking dialog2 = null;
		dialog2 = (bPrevious ? ((DialogRanking)dialogManager.getDialog(DialogManager.eDialog.Ranking)) : ((DialogDayRanking)dialogManager.getDialog(DialogManager.eDialog.DayRanking)));
		dialog2.setup();
		yield return StartCoroutine(dialog2.loadRanking(bPrevious));
		if (dialog2.bShow_)
		{
			yield return StartCoroutine(dialogManager.openDialog(dialog2));
			while (dialogManager.getActiveDialogNum() > 0)
			{
				yield return 0;
			}
			dailyMissionValueSetting();
			if (dailyMission.gameObject.activeSelf)
			{
				yield return StartCoroutine(dailyMission.dailyMissionInfoSetup());
			}
			if (bInitialized)
			{
				yield return StartCoroutine(dailyMissionClearCheck());
			}
		}
	}

	private IEnumerator friendHelp()
	{
		if (Bridge.PlayerData.getCurrentStage() <= 0)
		{
			yield break;
		}
		bool bHelp = false;
		for (int i = 0; i < DummyPlayFriendData.FriendNum; i++)
		{
			UserData data = DummyPlayFriendData.DummyFriends[i];
			if (data.lastStageClearProgressDay <= 0)
			{
				continue;
			}
			long now = Utility.getUnixTime(DateTime.Now);
			long prev2 = long.Parse(PlayerPrefs.GetString(Aes.EncryptString("HelpSend" + data.ID, Aes.eEncodeType.Percent), "0"));
			if (now - prev2 < Constant.HelpResendTime)
			{
				continue;
			}
			if (PlayerPrefs.HasKey(Aes.EncryptString("HelpNoSend" + data.ID, Aes.eEncodeType.Percent)))
			{
				if (now - data.lastUpdateTime >= Constant.CantHelpSendLoginTime)
				{
					continue;
				}
				prev2 = long.Parse(PlayerPrefs.GetString(Aes.EncryptString("HelpNoSend" + data.ID, Aes.eEncodeType.Percent), "0"));
				if (now - prev2 < Constant.HelpResendTimeAfterNotSend)
				{
					continue;
				}
			}
			bHelp = true;
			break;
		}
		if (bHelp)
		{
			while (dialogManager.getActiveDialogNum() > 0)
			{
				yield return null;
			}
			yield return StartCoroutine(ActionReward.getActionRewardInfo(ActionReward.eType.Friend));
			DialogFriendHelp dialog = (DialogFriendHelp)dialogManager.getDialog(DialogManager.eDialog.FriendHelp);
			dialog.setup();
			yield return StartCoroutine(dialogManager.openDialog(dialog));
			while (dialogManager.getActiveDialogNum() > 0)
			{
				yield return null;
			}
		}
	}

	private IEnumerator showComeback()
	{
		string url = GlobalData.Instance.getGameData().comeback;
		if (url == string.Empty || url == null)
		{
			yield break;
		}
		DialogComeback comebackDialog = dialogManager.getDialog(DialogManager.eDialog.Comeback) as DialogComeback;
		if (!(comebackDialog == null))
		{
			yield return StartCoroutine(comebackDialog.loadTexture(url));
			yield return StartCoroutine(comebackDialog.show(url));
			while (comebackDialog.isOpen())
			{
				yield return null;
			}
			comebackDialog.close();
		}
	}

	private IEnumerator showInformation()
	{
		DialogInformation infoDialog = dialogManager.getDialog(DialogManager.eDialog.Information) as DialogInformation;
		if (infoDialog == null)
		{
			yield break;
		}
		InformationData[] informationList = GlobalData.Instance.getGameData().informationList;
		foreach (InformationData info in informationList)
		{
			int id = info.informationId;
			int shopId = info.shopId;
			SaveInformationData data = SaveData.Instance.getGameData().getOtherData().getInfoData(id);
			if (!data.isShow())
			{
				continue;
			}
			string url = info.informationUrl;
			if (!(url == string.Empty))
			{
				yield return StartCoroutine(infoDialog.show(url, id, shopId));
				while (infoDialog.isOpen())
				{
					yield return 0;
				}
			}
		}
	}

	private IEnumerator getMonetizationAD()
	{
		if (SaveData.Instance.getSystemData().getOptionData().getFlag(SaveOptionData.eFlag.BGM))
		{
			Sound.Instance.setBgmMasterVolume(Sound.Instance.getDefaultBgmVolume());
			Sound.Instance.setBgmVolume(Sound.Instance.getDefaultBgmVolume());
		}
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(API.RequestMonetizationFreeAdReward, true, false));
		WWW www = NetworkMng.Instance.getWWW();
		UnityEngine.Debug.Log(" www : text : " + www.text);
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			CommonData data_ = JsonMapper.ToObject<CommonData>(www.text);
			if (data_.monetization != null)
			{
				GlobalData.Instance.getGameData().monetization = data_.monetization;
				GlobalData.Instance.getGameData().rewardNum = data_.rewardNum;
				GlobalData.Instance.getGameData().rewardType = data_.rewardType;
				GlobalData.Instance.getGameData().coin = data_.coin;
				GlobalData.Instance.getGameData().buyJewel = data_.buyJewel;
				GlobalData.Instance.getGameData().heart = data_.heart;
				mainMenu_.update();
				UnityEngine.Debug.Log(" RewardType : " + data_.rewardType + " // RewardNum : " + data_.rewardNum);
				GKUnityPluginController.Instance.ToastMessage(MessageResource.Instance.getMessage(500042));
			}
		}
		else
		{
			UnityEngine.Debug.Log(" getMonetizationAD : NotSuccess! ");
		}
	}

	private IEnumerator getCrossMission(EventMenu _eventmenu, CrossMenu _crossmenu, ADMenu _admenu, OfferwallMenu _offerwallmenu, PartManager.ePart part)
	{
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(API.RequestCollaboBBInfo, true, false));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			CollaboBBInfo BBinfo = JsonMapper.ToObject<CollaboBBInfo>(www.text);
			eResultCode resultCode = NetworkUtility.getResultCode(www);
			if (BBinfo.downloadUrl == null)
			{
				_eventmenu.SetCrossBBActive(false, part);
				_crossmenu.SetCrossBBActive(false, part);
				_admenu.SetCrossBBActive(false, _eventmenu.getEnable(), part);
				_offerwallmenu.SetCrossBBActive(false, _eventmenu.getEnable(), part);
			}
			else
			{
				_eventmenu.SetCrossBBActive(true, part);
				_crossmenu.SetCrossBBActive(true, part);
				_admenu.SetCrossBBActive(true, _eventmenu.getEnable(), part);
				_offerwallmenu.SetCrossBBActive(true, _eventmenu.getEnable(), part);
			}
		}
	}

	private IEnumerator showCrossMission()
	{
		while (dialogManager.getActiveDialogNum() > 0)
		{
			yield return 0;
		}
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(API.RequestCollaboBBInfo, true));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		CollaboBBInfo BBinfo = JsonMapper.ToObject<CollaboBBInfo>(www.text);
		if (BBinfo.downloadUrl != null)
		{
			DialogCrossMission dialog = dialogManager.getDialog(DialogManager.eDialog.CrossMission) as DialogCrossMission;
			dialog.setup(BBinfo);
			yield return StartCoroutine(dialogManager.openDialog(dialog));
			while (dialog.isOpen())
			{
				yield return 0;
			}
			yield return null;
		}
	}

	private IEnumerator showDailyMission()
	{
		while (dialogManager.getActiveDialogNum() > 0)
		{
			yield return 0;
		}
		DailyMission.bMissionCreate = false;
		DialogDailyMission dialog = dialogManager.getDialog(DialogManager.eDialog.DailyMission) as DialogDailyMission;
		dialog.setup(dailyMission.missionNum, dailyMission.mission_target);
		yield return StartCoroutine(dialogManager.openDialog(dialog));
		while (dialog.isOpen())
		{
			yield return 0;
		}
		yield return null;
	}

	private IEnumerator showDailyMissionCleared()
	{
		DialogDailyMissionClear dialog = dialogManager.getDialog(DialogManager.eDialog.DailyMissionClear) as DialogDailyMissionClear;
		if (!dialog.isOpen())
		{
			while (dialogManager.getActiveDialogNum() > 0)
			{
				yield return 0;
			}
			Sound.Instance.playSe(Sound.eSe.SE_108_Yay);
			Sound.Instance.playSe(Sound.eSe.SE_113_Clap);
			dialog.setup(this, DialogDailyMissionClear.eTargetPart.NormalMap);
			yield return StartCoroutine(dialogManager.openDialog(dialog));
			while (dialog.isOpen())
			{
				yield return 0;
			}
			if (dialog.toBonus)
			{
				transBonus = true;
			}
			yield return null;
		}
	}

	public IEnumerator dailyMissionClearCheck()
	{
		if (!dailyMission.bonusgamePlayed && dailyMission.missionCleared)
		{
			yield return StartCoroutine(showDailyMissionCleared());
		}
	}

	public IEnumerator setBonusGamePlayed(int status)
	{
		isBonusGamePlayed = true;
		NetworkMng.Instance.setup(Hash.BonusStageStart(dailyMission.dateKey));
		yield return StartCoroutine(NetworkMng.Instance.download(API.BonusStageStart, false, true));
		WWW www = NetworkMng.Instance.getWWW();
		BonusStartData data = JsonMapper.ToObject<BonusStartData>(www.text);
		GlobalData.Instance.setBonusStartData(data);
		Network.DailyMission missionData = GlobalData.Instance.getDailyMissionData();
		missionData.receiveFlg = ((status != 1) ? 3 : 4);
		if (status == 1)
		{
			NetworkMng.Instance.setup(Hash.B1(dailyMission.dateKey, 0, status));
			yield return StartCoroutine(NetworkMng.Instance.download(API.B1, false, true));
		}
		dailyMissionValueSetting();
		if (dailyMission.gameObject.activeSelf)
		{
			yield return StartCoroutine(dailyMission.dailyMissionInfoSetup());
		}
		isBonusGamePlayed = false;
	}

	public void dailyMissionValueSetting()
	{
		bool flag = dailyMission.updateDailyMissionData();
		dailyMission.gameObject.SetActive(flag);
		GameObject gameObject = appendGlobalObj(GlobalObjectParam.eObject.MapMainUI2);
		Transform transform = gameObject.transform.Find("sendOneStage");
		if (flag)
		{
			transform.localPosition = GlobalData.Instance.mapJumpDefaultPos;
		}
		else
		{
			transform.localPosition = new Vector3(GlobalData.Instance.mapJumpDefaultPos.x, GlobalData.Instance.mapJumpDefaultPos.y - 65f, GlobalData.Instance.mapJumpDefaultPos.z);
		}
	}

	private IEnumerator playTutorial(int stageNo)
	{
		TutorialManager.Instance.bItemTutorial = false;
		DialogSetup setup = dialogManager.getDialog(DialogManager.eDialog.Setup) as DialogSetup;
		if (setup.isOpen())
		{
			if (setup.skipTrans_.gameObject.activeSelf && !otherData_.isFlag(SaveOtherData.eFlg.TutorialStageSkip))
			{
				otherData_.setFlag(SaveOtherData.eFlg.TutorialStageSkip, true);
				yield return StartCoroutine(setup.ShineButtonEffect(setup.skipTrans_));
				yield return StartCoroutine(setup.playStageSkipTutorial(TutorialDataTable.eSPTutorial.StageSkip));
				otherData_.save();
			}
			GameObject dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
			StageInfo.CommonInfo stageInfo = dataTbl.GetComponent<StageDataTable>().getInfo(stageNo).Common;
			if ((stageInfo.FreeItems.Length > 0 || stageNo == 4) && TutorialManager.Instance.isTutorial(stageNo, TutorialDataTable.ePlace.Setup))
			{
				GameObject uiRoot = dialogManager.getCurrentUiRoot();
				TutorialManager.Instance.load(stageNo, uiRoot);
				yield return StartCoroutine(TutorialManager.Instance.play(stageNo, TutorialDataTable.ePlace.Setup, uiRoot, stageInfo, null));
			}
		}
	}

	private IEnumerator specialItem(int stageNo)
	{
		bool bEvent = Constant.Event.isEventStage(stageNo);
		DialogSetupBase setup = (bEvent ? (dialogManager.getDialog(DialogManager.eDialog.EventSetup) as DialogSetupBase) : (dialogManager.getDialog(DialogManager.eDialog.Setup) as DialogSetupBase));
		if (!setup.isOpen() || !resultParam_.IsRetry || resultParam_.IsClear || resultParam_.IsExit || TutorialManager.Instance.bItemTutorial || bRecommended)
		{
			yield break;
		}
		StageDataTable dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
		StageInfo.CommonInfo stageInfo = (bEvent ? dataTbl.getEventData().Infos[stageNo % 10].Common : dataTbl.getInfo(stageNo).Common);
		if (stageInfo.SpecialItemNum == 0)
		{
			yield break;
		}
		GameData gamedata = GlobalData.Instance.getGameData();
		int[] sale_areas = gamedata.saleArea;
		if (sale_areas != null)
		{
			int[] array = sale_areas;
			foreach (int sale_area in array)
			{
				if (sale_area == dataTbl.getInfo(stageNo).Area)
				{
					yield break;
				}
			}
		}
		System.Random random = new System.Random();
		if (random.Next(3) > 0)
		{
			yield break;
		}
		int index = random.Next(stageInfo.SpecialItemNum);
		StageInfo.Item currentItem = stageInfo.Items[index];
		StageInfo.Item specialItem = stageInfo.SpecialItems[index];
		if (currentItem.Type == specialItem.Type && currentItem.Num == specialItem.Num && currentItem.PriceType == specialItem.PriceType && currentItem.Price == specialItem.Price)
		{
			yield break;
		}
		BoostItem[] items = (bEvent ? ((DialogEventSetup)setup).getItems() : ((DialogSetup)setup).getItems());
		if (!(items[index] != null) || items[index].getPriceType() != 0)
		{
			Input.enable = false;
			Hashtable args = new Hashtable { { "spItemIndex", index } };
			if (!bEvent)
			{
				yield return StartCoroutine(((DialogSetup)setup).changeItem(index, specialItem, args));
			}
			else
			{
				yield return StartCoroutine(((DialogEventSetup)setup).changeItem(index, specialItem, args));
			}
			Input.enable = true;
			int tutorialStageNo = -4;
			TutorialManager.Instance.load(tutorialStageNo, uiRoot);
			yield return StartCoroutine(TutorialManager.Instance.play(tutorialStageNo, TutorialDataTable.ePlace.Setup, uiRoot, stageInfo, args));
			TutorialManager.Instance.unload(tutorialStageNo);
		}
	}

	private IEnumerator reviewReminder()
	{
		if (!resultParam_.IsClear || Constant.Event.isEventStage(resultParam_.StageNo) || resultParam_.StageNo + 1 < Constant.CanReviewStageNo)
		{
			yield break;
		}
		DialogReview dialog = dialogManager.getDialog(DialogManager.eDialog.Review) as DialogReview;
		if (!dialog.isCanOpen())
		{
			yield break;
		}
		yield return StartCoroutine(ActionReward.getActionRewardInfo(ActionReward.eType.Review));
		if (ActionReward.info_ == null)
		{
			yield break;
		}
		if (ActionReward.info_.count > 0)
		{
			PlayerPrefs.SetString(Aes.EncryptString(Constant.ReviewEndKey, Aes.eEncodeType.Percent), "1");
			yield break;
		}
		yield return StartCoroutine(dialog.open());
		while (dialog.isOpen())
		{
			yield return null;
		}
	}

	private IEnumerator recommendedItem(int stageNo)
	{
		bRecommended = false;
		bool bEvent = Constant.Event.isEventStage(stageNo);
		DialogSetup setup = (bEvent ? (dialogManager.getDialog(DialogManager.eDialog.EventSetup) as DialogSetup) : (dialogManager.getDialog(DialogManager.eDialog.Setup) as DialogSetup));
		if (!setup.isOpen() || !resultParam_.IsRetry || resultParam_.IsClear || resultParam_.IsExit || TutorialManager.Instance.bItemTutorial)
		{
			yield break;
		}
		StageDataTable dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
		StageInfo.CommonInfo stageInfo = (bEvent ? dataTbl.getEventData().Infos[stageNo % 10].Common : dataTbl.getInfo(stageNo).Common);
		if (stageInfo.SpecialItemNum == 0)
		{
			yield break;
		}
		System.Random rand = new System.Random();
		if (rand.Next(10) > 3)
		{
			yield break;
		}
		int rand_index = rand.Next(stageInfo.SpecialItemNum);
		StageInfo.Item itemInfo = stageInfo.Items[rand_index];
		int sale_per = 100;
		GameData gamedata = GlobalData.Instance.getGameData();
		int[] sale_areas = gamedata.saleArea;
		if (sale_areas != null)
		{
			int[] array = sale_areas;
			foreach (int sale_area in array)
			{
				if (sale_area == dataTbl.getInfo(stageNo).Area)
				{
					sale_per = gamedata.areaSalePercent;
					break;
				}
			}
		}
		DialogRecommended dialog = dialogManager.getDialog(DialogManager.eDialog.RecommendedItem) as DialogRecommended;
		dialog.setup(null, null, true);
		dialog.setItem(itemInfo, sale_per);
		yield return StartCoroutine(dialog.open());
		while (dialog.isOpen())
		{
			yield return 0;
		}
		if (dialog.result_ == DialogRecommended.eResult.Decide)
		{
			yield return StartCoroutine(setup.setItemByIndex(rand_index));
		}
		bRecommended = true;
	}

	private bool isOpenTreasureEventToMapIn()
	{
		if (openBox_ == null)
		{
			return false;
		}
		if (bNextStage_)
		{
			return false;
		}
		return true;
	}

	private WWW OnCreateTreasureWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("treasureBoxId", boxId_);
		return WWWWrap.create("player/treasureBoxOpen/");
	}

	private IEnumerator nextStage()
	{
		int currentStage = Bridge.PlayerData.getCurrentStage();
		bool bAllClear = otherData_.isFlag(SaveOtherData.eFlg.AllClear);
		Input.enable = false;
		if (bNextStage_)
		{
			bool bAnimation = true;
			if (resultParam_.StageNo != 0 && resultParam_.StageNo != currentStage - 1)
			{
				bAnimation = false;
			}
			if (!(clearStage_ != null))
			{
				if (!isLastStage(currentStage - 1))
				{
					string message = MessageResource.Instance.getMessage(4033);
					DialogConfirm dialog = (DialogConfirm)dialogManager.getDialog(DialogManager.eDialog.ToBeContinue);
					dialog.setMessage(message);
					dialog.setTitleEnable(false);
					Input.enable = true;
					yield return StartCoroutine(dialogManager.openDialog(dialog));
					while (dialog.isOpen())
					{
						yield return 0;
					}
					Input.enable = false;
					otherData_.setStageNo(currentStage);
				}
				bNextStage_ = false;
				Input.enable = true;
				yield break;
			}
			if (!isPaymentStageClear)
			{
				yield return StartCoroutine(clearStage_.setup(bAnimation, false));
			}
			if (isLastStage(currentStage - 1))
			{
				Input.enable = true;
				yield return StartCoroutine(openToBeContinueDialog());
				otherData_.setFlag(SaveOtherData.eFlg.AllClear, true);
				Input.enable = false;
			}
			else
			{
				bool showStarReward = false;
				if (directArea_ != null)
				{
					yield return StartCoroutine(directArea_.openDirect(Area.eMap.Normal));
					if (directArea_.getAreaNo() == 1)
					{
						showStarReward = true;
					}
				}
				yield return StartCoroutine(moveNextStage(currentStage));
				if (eventMenu_.gameObject.activeSelf && !otherData_.isFlag(SaveOtherData.eFlg.TutorialEvent))
				{
					BoxCollider col2 = eventMenu_.GetComponentInChildren<BoxCollider>();
					col2.enabled = false;
					eventMenu_.transform.localPosition += Vector3.back * 100f;
					TutorialManager.Instance.load(-2, uiRoot);
					int count2 = Input.forceEnable();
					yield return StartCoroutine(TutorialManager.Instance.play(-2, TutorialDataTable.ePlace.Setup, uiRoot, null, null));
					Input.revertForceEnable(count2);
					TutorialManager.Instance.unload(-2);
					eventMenu_.transform.localPosition += Vector3.forward * 100f;
					col2.enabled = true;
					otherData_.setFlag(SaveOtherData.eFlg.TutorialEvent, true);
				}
				else if (collaboMenu_.gameObject.activeSelf && !otherData_.isFlag(SaveOtherData.eFlg.TutorialGachaMukCollabo))
				{
					BoxCollider col = collaboMenu_.GetComponentInChildren<BoxCollider>();
					col.enabled = false;
					collaboMenu_.transform.localPosition += Vector3.back * 100f;
					TutorialManager.Instance.load(-17, uiRoot);
					int count = Input.forceEnable();
					yield return StartCoroutine(TutorialManager.Instance.play(-17, TutorialDataTable.ePlace.ReadyGoEnd, uiRoot, null, null));
					Input.revertForceEnable(count);
					TutorialManager.Instance.unload(-17);
					collaboMenu_.transform.localPosition += Vector3.forward * 100f;
					col.enabled = true;
					otherData_.setFlag(SaveOtherData.eFlg.TutorialGachaMukCollabo, true);
				}
				if (showStarReward)
				{
					Input.enable = true;
					yield return StartCoroutine(openFirstGachaDialog());
					checkStageSixClear();
					Input.enable = false;
				}
				otherData_.setStageNo(currentStage);
			}
		}
		else if (bAllClear && !isLastStage(currentStage - 1))
		{
			if (directArea_ != null)
			{
				yield return StartCoroutine(directArea_.openDirect(Area.eMap.Normal));
			}
			yield return StartCoroutine(moveNextStage(currentStage));
			otherData_.setFlag(SaveOtherData.eFlg.AllClear, false);
		}
		else if (isLastStage(resultParam_.StageNo) && resultParam_.IsClear)
		{
			Input.enable = true;
			yield return StartCoroutine(openToBeContinueDialog());
			Input.enable = false;
		}
		else if (!bEventStage_ && resultParam_.IsClear && resultParam_.StageNo == currentStage && !resultParam_.IsProgressOpen)
		{
			Input.enable = true;
			Area target_area = null;
			Area[] array = areas_;
			foreach (Area area in array)
			{
				if (area.getAreaNo() == stageTbl_.getInfo(GlobalData.Instance.getGameData().progressStageNo).Area)
				{
					target_area = area;
					break;
				}
			}
			if (target_area != null)
			{
				yield return StartCoroutine(openAreaLockDialog(target_area));
			}
			Input.enable = false;
		}
		else if (resultParam_.IsProgressOpen)
		{
			GameObject currentIcon = stageIconParent_.getIcon(currentStage).gameObject;
			StageIcon icon = currentIcon.GetComponent<StageIcon>();
			icon.enable();
			yield return StartCoroutine(icon.playOpenProduct());
		}
		otherData_.save();
		Input.enable = true;
	}

	private IEnumerator updatePanelEnable()
	{
		while (base.gameObject != null)
		{
			bool bShowDialog = dialogManager.getActiveDialogNum() >= 1;
			bool bUpdate = !bShowDialog;
			if (bUpdate)
			{
				bUpdate = Input.enable;
			}
			mapCamera_.getDragObject().enabled = Input.touchCount <= 1;
			mapCamera_.setUpdateFlg(bUpdate);
			yield return 0;
		}
	}

	private void analyzeHash(Hashtable args, bool IsProgressOpen = false)
	{
		if (args != null && (isPlayedStage() || partManager.prevPart == PartManager.ePart.EventMap || partManager.prevPart == PartManager.ePart.ChallengeMap || partManager.prevPart == PartManager.ePart.CollaborationMap || partManager.prevPart == PartManager.ePart.BonusStage || IsProgressOpen))
		{
			if (args.ContainsKey("StageNo"))
			{
				resultParam_.StageNo = (int)args["StageNo"];
			}
			if (args.ContainsKey("IsRetry"))
			{
				resultParam_.IsRetry = (bool)args["IsRetry"];
			}
			if (args.ContainsKey("IsClear"))
			{
				resultParam_.IsClear = (bool)args["IsClear"];
			}
			if (args.ContainsKey("IsExit"))
			{
				resultParam_.IsExit = (bool)args["IsExit"];
			}
			if (args.ContainsKey("IsClose"))
			{
				resultParam_.IsResultClose = (bool)args["IsClose"];
			}
			if (args.ContainsKey("IsProgressOpen"))
			{
				resultParam_.IsProgressOpen = (bool)args["IsProgressOpen"];
			}
			if (args.ContainsKey("IsForceSendInactive"))
			{
				resultParam_.IsForceSendInactive = (bool)args["IsForceSendInactive"];
			}
			if (args.ContainsKey("IsBossOpen"))
			{
				resultParam_.IsBossOpen = (bool)args["IsBossOpen"];
			}
			if (args.ContainsKey("IsSendOneStage"))
			{
				resultParam_.IsSendOneStage = (bool)args["IsSendOneStage"];
			}
			if (args.ContainsKey("IsSendMap"))
			{
				resultParam_.IsSendMap = (bool)args["IsSendMap"];
				resultParam_.SendStageNo = (int)args["SendStageNo"];
			}
		}
	}

	private StageIcon setupStageIcon(StageIconParent stageIconRoot, int currentStage, bool bNextStage)
	{
		StageIcon result = null;
		foreach (int key in stageIconRoot.getKeys())
		{
			StageIcon icon = stageIconRoot.getIcon(key);
			if (bNextStage && icon.getStageNo() == currentStage)
			{
				result = icon;
			}
			else
			{
				StartCoroutine(icon.setup(false, false));
			}
			icon.setSprite();
			setupButton(icon.gameObject, true);
			if (currentStage < icon.getStageNo())
			{
				icon.disable();
			}
			else
			{
				icon.enable();
			}
		}
		return result;
	}

	private bool isNextStage(int stageNo)
	{
		if (!otherData_.isPlayedStageProd(stageNo) && !bEventStage_)
		{
			return true;
		}
		return false;
	}

	private Area setupArea(Area[] areas, int currentStage, bool bNextStage, bool bUnlockArea = false)
	{
		Area result = null;
		if (stageTbl_.getInfo(currentStage) == null)
		{
			return result;
		}
		int area = stageTbl_.getInfo(currentStage).Area;
		int num = area;
		if (bNextStage && !isLastStage(currentStage))
		{
			num = stageTbl_.getInfo(getNextStageNo(currentStage)).Area;
		}
		updateAreaSale();
		updateStageItemAreaSale();
		int i;
		for (i = currentStage; Bridge.StageData.isClear(i); i++)
		{
		}
		int area2 = stageTbl_.getInfo(getNextStageNo(i)).Area;
		bool flag = Bridge.StageData.isClear(GlobalData.Instance.getGameData().progressStageNo - 1);
		if (!isLastStage(currentStage) && stageTbl_.getInfo(GlobalData.Instance.getGameData().progressStageNo - 1).Area > stageTbl_.getInfo(currentStage).Area)
		{
			bUnlockArea = true;
		}
		foreach (Area area3 in areas)
		{
			if (num != area && area3.getAreaNo() == num && (area3.isCompleateTerm() || bUnlockArea))
			{
				result = area3;
				area3.updateStarsNumLabel(false);
			}
			else if (area3.getAreaNo() <= area)
			{
				area3.open();
				area3.updateStarsNumLabel(false);
				area3.setGateButtonEnable(false);
			}
			else
			{
				area3.updateStarsNumLabel(true);
				area3.setGateButtonEnable(area3.getAreaNo() == area2 && flag);
			}
		}
		return result;
	}

	public void updateAreaSale()
	{
		if (areas_ != null)
		{
			Area[] array = areas_;
			foreach (Area area in array)
			{
				area.updateSale();
			}
		}
	}

	public void updateStageItemAreaSale()
	{
		GameData gameData = GlobalData.Instance.getGameData();
		if (!collaboMenu_.gameObject.activeSelf && !eventMenu_.gameObject.activeSelf)
		{
			return;
		}
		eventMenu_.updateStageItemEnable(false);
		collaboMenu_.updateStageItemEnable(false);
		if (gameData.isStageItemAreaCampaign && gameData.saleStageItemArea != null)
		{
			int[] saleStageItemArea = gameData.saleStageItemArea;
			foreach (int num in saleStageItemArea)
			{
				if (num >= 10000 && num < 20000 && eventMenu_.gameObject.activeSelf)
				{
					eventMenu_.updateStageItemEnable(true);
				}
				else if (num >= 40000 && num < 500000 && collaboMenu_.gameObject.activeSelf)
				{
					collaboMenu_.updateStageItemEnable(true);
				}
			}
		}
		if (!gameData.isAreaCampaign || gameData.saleArea == null)
		{
			return;
		}
		int[] saleArea = gameData.saleArea;
		foreach (int num2 in saleArea)
		{
			if (num2 >= 10000 && num2 < 20000 && eventMenu_.gameObject.activeSelf)
			{
				eventMenu_.updateAreaItemEnable(true);
			}
			else if (num2 >= 40000 && num2 < 500000 && collaboMenu_.gameObject.activeSelf)
			{
				collaboMenu_.updateAreaItemEnable(true);
			}
		}
	}

	private void setActiveGlobalObjs(bool value)
	{
		foreach (GameObject item in globalObjList_)
		{
			if (!(item == null))
			{
				item.SetActive(value);
			}
		}
	}

	private GameObject appendGlobalObj(GlobalObjectParam.eObject objType)
	{
		GameObject @object = globalObj_.getObject(objType);
		globalObjList_.Add(@object);
		return @object;
	}

	private void showCurrentEffect(GameObject currentIcon)
	{
		GameObject gameObject = appendGlobalObj(GlobalObjectParam.eObject.CurrentStageEffect);
		if (gameObject != null)
		{
			gameObject.gameObject.SetActive(false);
			Utility.setParent(gameObject, map_.transform, false);
			gameObject.transform.localPosition = currentIcon.transform.localPosition;
			gameObject.gameObject.SetActive(true);
		}
	}

	private void showPlayer(GameObject currentIcon)
	{
		GamePlayer component = appendGlobalObj(GlobalObjectParam.eObject.Player).GetComponent<GamePlayer>();
		if (component != null)
		{
			component.setup(currentIcon.transform.localPosition, this, GamePlayer.eTargetPart.NormalMap);
			component.gameObject.SetActive(true);
		}
	}

	private TreasureBox setupTreasureBox(TreasureBoxParent boxParent)
	{
		int totalStar = Bridge.StageData.getTotalStar();
		TreasureBox result = null;
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		TreasureDataTable component = @object.GetComponent<TreasureDataTable>();
		for (int i = 0; i < component.getBoxNum(); i++)
		{
			TreasureInfo.BoxInfo info = component.getInfo(i);
			if (info != null && info.Star <= totalStar && info.ID >= Bridge.PlayerData.getTreasureNum() && (openBoxID_ == -1 || openBoxID_ > info.ID))
			{
				openBoxID_ = info.ID;
			}
		}
		foreach (int key in boxParent.getKeys())
		{
			TreasureBox box = boxParent.getBox(key);
			box.setup(totalStar);
			if (!box.checkComplete(totalStar))
			{
				continue;
			}
			if (key >= Bridge.PlayerData.getTreasureNum())
			{
				if (box.getID() == openBoxID_)
				{
					result = box;
				}
			}
			else
			{
				box.open();
			}
		}
		return result;
	}

	private IEnumerator transEventMap()
	{
		Input.enable = false;
		partManager.bTransitionMap_ = true;
		float orth_add_size = 0.7f;
		float orth_sec = 0.5f;
		float orth_temp = mapCamera_.GetComponent<Camera>().orthographicSize;
		float del_count = 0f;
		if (bInactive)
		{
			StopCoroutine("sendInactive");
			bInactive = false;
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		}
		yield return StartCoroutine(playCloseCloudEff());
		while (del_count < orth_sec)
		{
			del_count += Time.deltaTime;
			float def_per = del_count / orth_sec;
			mapCamera_.GetComponent<Camera>().orthographicSize = orth_temp + orth_add_size * def_per;
			yield return 0;
		}
		GameObject cloud_obj = globalObj_.getObject(GlobalObjectParam.eObject.Cloud);
		Animation anim = cloud_obj.GetComponentInChildren<Animation>();
		while (anim.isPlaying)
		{
			yield return 0;
		}
		mapCamera_.GetComponent<Camera>().orthographicSize = orth_temp;
		OnTransition();
		Input.enable = true;
		partManager.requestTransition(PartManager.ePart.EventMap, null, FadeMng.eType.MapChange, true);
	}

	private IEnumerator transChallengeMap()
	{
		Input.enable = false;
		partManager.bTransitionMap_ = true;
		float orth_add_size = 0.7f;
		float orth_sec = 0.5f;
		float orth_temp = mapCamera_.GetComponent<Camera>().orthographicSize;
		float del_count = 0f;
		yield return StartCoroutine(playCloseCloudEff());
		if (bInactive)
		{
			StopCoroutine("sendInactive");
			bInactive = false;
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		}
		while (del_count < orth_sec)
		{
			del_count += Time.deltaTime;
			float def_per = del_count / orth_sec;
			mapCamera_.GetComponent<Camera>().orthographicSize = orth_temp - orth_add_size * def_per;
			yield return 0;
		}
		GameObject cloud_obj = globalObj_.getObject(GlobalObjectParam.eObject.Cloud);
		Animation anim = cloud_obj.GetComponentInChildren<Animation>();
		while (anim.isPlaying)
		{
			yield return 0;
		}
		mapCamera_.GetComponent<Camera>().orthographicSize = orth_temp;
		OnTransition();
		Input.enable = true;
		partManager.requestTransition(PartManager.ePart.ChallengeMap, null, FadeMng.eType.MapChange, true);
	}

	private IEnumerator transCollaborationMap()
	{
		Input.enable = false;
		partManager.bTransitionMap_ = true;
		float orth_add_size = 0.7f;
		float orth_sec = 0.5f;
		float orth_temp = mapCamera_.GetComponent<Camera>().orthographicSize;
		float del_count = 0f;
		yield return StartCoroutine(playCloseCloudEff());
		if (bInactive)
		{
			StopCoroutine("sendInactive");
			bInactive = false;
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		}
		while (del_count < orth_sec)
		{
			del_count += Time.deltaTime;
			float def_per = del_count / orth_sec;
			mapCamera_.GetComponent<Camera>().orthographicSize = orth_temp + orth_add_size * def_per;
			yield return 0;
		}
		GameObject cloud_obj = globalObj_.getObject(GlobalObjectParam.eObject.Cloud);
		Animation anim = cloud_obj.GetComponentInChildren<Animation>();
		while (anim.isPlaying)
		{
			yield return 0;
		}
		mapCamera_.GetComponent<Camera>().orthographicSize = orth_temp;
		OnTransition();
		Input.enable = true;
		partManager.requestTransition(PartManager.ePart.CollaborationMap, null, FadeMng.eType.MapChange, true);
	}

	private IEnumerator playOpenCloudEff()
	{
		GameObject cloud_obj = globalObj_.getObject(GlobalObjectParam.eObject.Cloud);
		cloud_obj.SetActive(true);
		Animation anim = cloud_obj.GetComponentInChildren<Animation>();
		anim.Play("BG_Cloud_Open_anm");
		anim["BG_Cloud_Open_anm"].speed = 1.5f;
		while (anim.isPlaying)
		{
			yield return 0;
		}
		cloud_obj.SetActive(false);
	}

	private IEnumerator playCloseCloudEff()
	{
		GameObject cloud_obj = globalObj_.getObject(GlobalObjectParam.eObject.Cloud);
		cloud_obj.SetActive(true);
		Animation anim = cloud_obj.GetComponentInChildren<Animation>();
		anim.Play("BG_Cloud_Close_anm");
		anim["BG_Cloud_Close_anm"].speed = 1.5f;
		yield break;
	}

	private IEnumerator playOpenWaveEff()
	{
		GameObject wave_obj = globalObj_.getObject(GlobalObjectParam.eObject.Wave);
		wave_obj.SetActive(true);
		Animation anim = wave_obj.GetComponentInChildren<Animation>();
		anim.Play("BG_Wave_Open_anm");
		anim["BG_Wave_Open_anm"].speed = 1.5f;
		while (anim.isPlaying)
		{
			yield return 0;
		}
		wave_obj.SetActive(false);
	}

	private IEnumerator playCloseWaveEff()
	{
		GameObject wave_obj = globalObj_.getObject(GlobalObjectParam.eObject.Wave);
		wave_obj.SetActive(true);
		Animation anim = wave_obj.GetComponentInChildren<Animation>();
		anim.Play("BG_Wave_Close_anm");
		anim["BG_Wave_Close_anm"].speed = 1.5f;
		yield break;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		if (!bButtonEnable_)
		{
			yield break;
		}
		DialogAppQuit da = (DialogAppQuit)dialogManager.getDialog(DialogManager.eDialog.AppQuit);
		if (da.isOpen() || (otherData_.isFlag(SaveOtherData.eFlg.RequestBossOpen) && !(dialogManager.getDialog(DialogManager.eDialog.Setup) as DialogSetup).isOpen()))
		{
			yield break;
		}
		string name = trigger.name;
		if (name == "park_Button")
		{
			Constant.SoundUtil.PlayDecideSE();
			Hashtable args = new Hashtable
			{
				{
					"Place",
					Scenario.ePlace.Begin
				},
				{ "StageNo", 500000 }
			};
			if (!otherData_.isFlag(SaveOtherData.eFlg.FirstGoPark))
			{
				otherData_.setFlag(SaveOtherData.eFlg.FirstGoPark, true);
				otherData_.save();
				partManager.requestTransition(PartManager.ePart.Scenario, args, FadeMng.eType.Scenario, true);
			}
			else
			{
				partManager.requestTransition(PartManager.ePart.Park, null, FadeMng.eType.AllMask, true);
			}
		}
		if (name.Contains("Stage_button"))
		{
			Constant.SoundUtil.PlayButtonSE();
			StageIcon icon = trigger.transform.parent.GetComponent<StageIcon>();
			DialogSetup setupDialog = dialogManager.getDialog(DialogManager.eDialog.Setup) as DialogSetup;
			BoostItem[] items = setupDialog.transform.Find("items").GetComponentsInChildren<BoostItem>(true);
			BoostItem[] array = items;
			foreach (BoostItem bi in array)
			{
				bi.InitSpecialItemInfo();
			}
			yield return StartCoroutine(setupDialog.show(icon, this));
			yield return StartCoroutine(playTutorial(icon.getStageNo()));
			yield break;
		}
		if (name == "PlusButton")
		{
			Constant.SoundUtil.PlayButtonSE();
			yield return StartCoroutine(plusButton(trigger));
			yield break;
		}
		if (trigger.transform.parent.name == "gacya_button")
		{
			Constant.SoundUtil.PlayButtonSE();
			DialogAvatarGacha gachaDialog = dialogManager.getDialog(DialogManager.eDialog.AvatarGacha) as DialogAvatarGacha;
			Hashtable h = Hash.GachaTop();
			NetworkMng.Instance.setup(h);
			yield return StartCoroutine(NetworkMng.Instance.download(API.GachaTop, true, true));
			while (NetworkMng.Instance.isDownloading())
			{
				yield return null;
			}
			if (NetworkMng.Instance.getResultCode() == eResultCode.Success)
			{
				WWW wwww = NetworkMng.Instance.getWWW();
				GachaTop resultData = JsonMapper.ToObject<GachaTop>(wwww.text);
				GlobalData.Instance.getGameData().setGachaTopData(resultData);
				gachaDialog.resetGachaNo();
				gachaDialog.setup(GlobalData.Instance.getGameData().isFirstGacha);
				gachaDialog.gacha_map_button = gachaButton_;
				yield return StartCoroutine(dialogManager.openDialog(gachaDialog));
			}
			yield break;
		}
		switch (name)
		{
		case "EventButton":
			Constant.SoundUtil.PlayButtonSE();
			if (!eventMenu_.isEventDuration())
			{
				yield return StartCoroutine(openNoneEventDioalog());
			}
			else
			{
				StartCoroutine(transEventMap());
			}
			break;
		case "CrossButton":
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(showCrossMission());
			break;
		case "CollaborationButton":
			Constant.SoundUtil.PlayButtonSE();
			if (!collaboMenu_.isEventDuration())
			{
				yield return StartCoroutine(openNoneEventDioalog());
			}
			else
			{
				StartCoroutine(transCollaborationMap());
			}
			break;
		case "BossButton":
			Constant.SoundUtil.PlayButtonSE();
			if (bossMenu_ != null)
			{
				DialogBossSelect setup = dialogManager.getDialog(DialogManager.eDialog.BossSelect) as DialogBossSelect;
				yield return StartCoroutine(setup.show(dialogManager));
			}
			break;
		case "ranking_button":
		{
			Constant.SoundUtil.PlayDecideSE();
			DialogDayRanking dialog = dialogManager.getDialog(DialogManager.eDialog.DayRanking) as DialogDayRanking;
			dialog.setup();
			yield return StartCoroutine(dialog.loadRanking(false));
			if (dialog.bShow_)
			{
				dailyMissionValueSetting();
				if (dailyMission.gameObject.activeSelf)
				{
					yield return StartCoroutine(dailyMission.dailyMissionInfoSetup());
				}
				StartCoroutine(dailyMissionClearCheck());
				yield return StartCoroutine(dialogManager.openDialog(dialog));
				while (dialog.bShow_)
				{
					yield return 0;
				}
				dailyMissionValueSetting();
				if (dailyMission.gameObject.activeSelf)
				{
					yield return StartCoroutine(dailyMission.dailyMissionInfoSetup());
				}
				StartCoroutine(dailyMissionClearCheck());
			}
			break;
		}
		case "mail_button":
		{
			Constant.SoundUtil.PlayDecideSE();
			int mailCount = 0;
			NetworkMng.Instance.setup(null);
			Mail[] mails2 = null;
			yield return StartCoroutine(NetworkMng.Instance.download(OnCreateMailListWWW, true));
			if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
			{
				break;
			}
			WWW www = NetworkMng.Instance.getWWW();
			mails2 = JsonMapper.ToObject<MailList>(www.text).mailList;
			Mail[] array2 = mails2;
			foreach (Mail mail in array2)
			{
				if (!mail.isOpen)
				{
					mailCount++;
				}
			}
			setMailNum(mailCount);
			if (mailNum_ > 0)
			{
				DialogMail dialog3 = dialogManager.getDialog(DialogManager.eDialog.Mail) as DialogMail;
				yield return StartCoroutine(dialog3.show(mails2));
				while (dialog3.isOpen())
				{
					yield return null;
				}
				GlobalData.Instance.getGameData().mailUnReadCount = dialog3.mailNum_;
				setMailNum(dialog3.mailNum_);
			}
			else
			{
				setMailNum(0);
				DialogConfirm dialog2 = dialogManager.getDialog(DialogManager.eDialog.NoMail) as DialogConfirm;
				yield return StartCoroutine(dialogManager.openDialog(dialog2));
			}
			dailyMissionValueSetting();
			if (dailyMission.gameObject.activeSelf)
			{
				yield return StartCoroutine(dailyMission.dailyMissionInfoSetup());
			}
			break;
		}
		case "invite_button":
		{
			Constant.SoundUtil.PlayDecideSE();
			DialogInvite dialog4 = dialogManager.getDialog(DialogManager.eDialog.Invite) as DialogInvite;
			yield return StartCoroutine(dialog4.show());
			break;
		}
		case "avatar_button":
		{
			Constant.SoundUtil.PlayDecideSE();
			GlobalData.Instance.acInfo_.isSetup = false;
			DialogAvatarCollection collection = dialogManager.getDialog(DialogManager.eDialog.AvatarCollection) as DialogAvatarCollection;
			if (!collection.isOpen())
			{
				collection.setup();
				yield return StartCoroutine(collection.open());
			}
			break;
		}
		case "option_button":
		{
			Constant.SoundUtil.PlayDecideSE();
			DialogOption dialog5 = dialogManager.getDialog(DialogManager.eDialog.Option) as DialogOption;
			dialog5.setup();
			yield return StartCoroutine(dialogManager.openDialog(dialog5));
			break;
		}
		case "Exp_Button":
			Constant.SoundUtil.PlayButtonSE();
			mainMenu_.getExpMenu().changeText();
			break;
		case "arrow_00":
			if (bTransMap)
			{
				mapNo_--;
				if (mapNo_ < 0)
				{
					mapNo_ = 0;
				}
				else
				{
					StartCoroutine(transMap(trigger.transform.parent.parent.gameObject, mapNo_, MapMoveDirection.Down, false));
				}
			}
			break;
		case "arrow_01":
			if (bTransMap)
			{
				mapNo_++;
				if (mapNo_ >= iconTbl_.getMaxMapNum())
				{
					mapNo_ = iconTbl_.getMaxMapNum() - 1;
				}
				else
				{
					StartCoroutine(transMap(trigger.transform.parent.parent.gameObject, mapNo_, MapMoveDirection.Up, false));
				}
			}
			break;
		case "sendOne_button":
		{
			Constant.SoundUtil.PlayButtonSE();
			DialogBase baseDialog = dialogManager.getDialog(DialogManager.eDialog.SendMap);
			if (baseDialog != null)
			{
				DialogSendMap dialog6 = baseDialog as DialogSendMap;
				dialog6.setup();
				dialogManager.StartCoroutine(dialogManager.openDialog(dialog6));
			}
			break;
		}
		case "gate":
		{
			Area area = trigger.transform.parent.parent.GetComponent<Area>();
			yield return StartCoroutine(openAreaLockDialog(area));
			yield return StartCoroutine(playTutorial(Bridge.PlayerData.getCurrentStage()));
			break;
		}
		case "popup_root":
			if (!dailyMission.bonusgamePlayed)
			{
				Constant.SoundUtil.PlayButtonSE();
				StartCoroutine(showDailyMission());
			}
			break;
		case "toastpromotion_button":
			yield return StartCoroutine(GKToastListener.Instance.showToastPromotionDialog(partManager));
			break;
		case "AdButton":
			if (adMenu_.ADReady)
			{
				GameObject.Find("PB_TNKVidioHandler").GetComponent<PBTnkVidioHandler>().setContinueUpdate(false);
				yield return StartCoroutine(ShowVideo());
			}
			break;
		}
	}

	private IEnumerator ShowVideo()
	{
		Sound.Instance.setBgmMasterVolume(0f);
		Sound.Instance.setBgmVolume(0f);
		int CheckNum = UnityEngine.Random.Range(0, 2);
		int VidioNum = 0;
		UnityEngine.Debug.Log("TNK hasVideoAd = " + Plugin.Instance.hasVideoAd("PB_Video_AD"));
		UnityEngine.Debug.Log("Vungle isAdvertAvailable = " + Vungle.isAdvertAvailable());
		switch (CheckNum)
		{
		case 0:
			if (Plugin.Instance.hasVideoAd("PB_Video_AD"))
			{
				UnityEngine.Debug.Log("TNK showVideoAd!!");
				Plugin.Instance.showVideoAd("PB_Video_AD");
				VidioNum = 1;
			}
			else if (Vungle.isAdvertAvailable())
			{
				UnityEngine.Debug.Log("Vungle showVideoAd!!");
				Vungle.playAd(false, string.Empty);
				VidioNum = 2;
			}
			else
			{
				GKUnityPluginController.Instance.ToastMessage(MessageResource.Instance.getMessage(500041));
			}
			break;
		case 1:
			if (Vungle.isAdvertAvailable())
			{
				UnityEngine.Debug.Log("Vungle showVideoAd!!");
				Vungle.playAd(false, string.Empty);
				VidioNum = 2;
			}
			else if (Plugin.Instance.hasVideoAd("PB_Video_AD"))
			{
				UnityEngine.Debug.Log("TNK showVideoAd!!");
				Plugin.Instance.showVideoAd("PB_Video_AD");
				VidioNum = 1;
			}
			else
			{
				GKUnityPluginController.Instance.ToastMessage(MessageResource.Instance.getMessage(500041));
			}
			break;
		}
		switch (VidioNum)
		{
		case 1:
			Tapjoy.TrackEvent("AD Vidio", "Reward", "TNK", null, 0L);
			GlobalGoogleAnalytics.Instance.LogEvent("AD Vidio", "Reward", "TNK", 0L);
			break;
		case 2:
			Tapjoy.TrackEvent("AD Vidio", "Reward", "Vungle", null, 0L);
			GlobalGoogleAnalytics.Instance.LogEvent("AD Vidio", "Reward", "Vungle", 0L);
			break;
		case 0:
			Tapjoy.TrackEvent("AD Vidio", "Reward", "None Vidio", null, 0L);
			GlobalGoogleAnalytics.Instance.LogEvent("AD Vidio", "Reward", "None Vidio", 0L);
			break;
		}
		yield break;
	}

	private IEnumerator playBossTutorial(TutorialDataTable.eSPTutorial tutorialNo)
	{
		bossMenu_.setButtonEnable(false);
		bossMenu_.transform.localPosition += Vector3.back * 100f;
		TutorialManager.Instance.load((int)tutorialNo, uiRoot);
		yield return StartCoroutine(TutorialManager.Instance.play((int)tutorialNo, TutorialDataTable.ePlace.ReadyGoEnd, uiRoot, null, null));
		TutorialManager.Instance.unload((int)tutorialNo);
		bossMenu_.transform.localPosition += Vector3.forward * 100f;
		bossMenu_.setButtonEnable(true);
	}

	private IEnumerator playGachaTutorial(TutorialDataTable.eSPTutorial tutorialNo)
	{
		gachaButton_.GetComponentInChildren<BoxCollider>().enabled = false;
		gachaButton_.transform.localPosition += Vector3.back * 100f;
		TutorialManager.Instance.load((int)tutorialNo, uiRoot);
		yield return StartCoroutine(TutorialManager.Instance.play((int)tutorialNo, TutorialDataTable.ePlace.ReadyGoEnd, uiRoot, null, null));
		TutorialManager.Instance.unload((int)tutorialNo);
		gachaButton_.transform.localPosition += Vector3.forward * 100f;
		gachaButton_.GetComponentInChildren<BoxCollider>().enabled = true;
	}

	private IEnumerator plusButton(GameObject trigger)
	{
		DialogAllShop dialog = dialogManager.getDialog(DialogManager.eDialog.AllShop) as DialogAllShop;
		switch (trigger.transform.parent.name)
		{
		case "01_coin":
			yield return StartCoroutine(dialog.show(DialogAllShop.ePanelType.Coin));
			break;
		case "02_jewel":
			yield return StartCoroutine(dialog.show(DialogAllShop.ePanelType.Jewel));
			break;
		case "04_heart":
			yield return StartCoroutine(dialog.show(DialogAllShop.ePanelType.Heart));
			break;
		}
	}

	public IEnumerator showDialog_FriendData(int _num)
	{
		if (showPlayerDataEneble)
		{
			Constant.SoundUtil.PlayDecideSE();
			DialogPlayerInfo dialog = dialogManager.getDialog(DialogManager.eDialog.PlayerInfo) as DialogPlayerInfo;
			dialog.setup_friend(friendsData, friendsData[_num].StageNo, friendsData[_num].UserName);
			yield return StartCoroutine(dialogManager.openDialog(dialog));
		}
	}

	public IEnumerator showDialog_playerData(UserData _data)
	{
		if (showPlayerDataEneble)
		{
			Constant.SoundUtil.PlayDecideSE();
			DialogPlayerInfo dialog = dialogManager.getDialog(DialogManager.eDialog.PlayerInfo) as DialogPlayerInfo;
			dialog.setup_player(_data);
			yield return StartCoroutine(dialogManager.openDialog(dialog));
		}
	}

	private void focus(int stageNo)
	{
		if (!isPlayedStage())
		{
			mapCamera_.focus(stageIconParent_.getIcon(stageNo).transform.localPosition);
		}
		else
		{
			if (bEventStage_)
			{
				return;
			}
			if (resultParam_.IsRetry)
			{
				mapCamera_.focus(stageIconParent_.getIcon(resultParam_.StageNo).transform.localPosition);
				return;
			}
			if (!bNextStage_ && !isLastStage(resultParam_.StageNo) && isReclear())
			{
				StageIcon icon = stageIconParent_.getIcon(getNextStageNo(resultParam_.StageNo));
				if (icon != null)
				{
					if (Bridge.PlayerData.getCurrentStage() >= getNextStageNo(resultParam_.StageNo))
					{
						mapCamera_.focus(icon.transform.localPosition);
						return;
					}
					StageIcon icon2 = stageIconParent_.getIcon(resultParam_.StageNo);
					if (icon2 != null)
					{
						mapCamera_.focus(icon2.transform.localPosition);
					}
					return;
				}
			}
			mapCamera_.focus(stageIconParent_.getIcon(resultParam_.StageNo).transform.localPosition);
		}
	}

	private void setMailNum(int num)
	{
		if (num == 0 || mailNum_ != num)
		{
			mailNum_ = num;
			bagMenu_.setMailNum(mailNum_);
		}
	}

	private int getNextSetupStageNo(bool bFading)
	{
		if (resultParam_.IsRetry)
		{
			return resultParam_.StageNo;
		}
		if (isLastStage(resultParam_.StageNo))
		{
			return -1;
		}
		if (resultParam_.IsExit)
		{
			return -1;
		}
		if (bNextStage_ || bSetupDialogOpen_)
		{
			if (bFading)
			{
				return -1;
			}
			int currentStage = Bridge.PlayerData.getCurrentStage();
			if (isLastStage(currentStage - 1))
			{
				return -1;
			}
			return currentStage;
		}
		if (!resultParam_.IsClear)
		{
			return -1;
		}
		if (resultParam_.IsResultClose)
		{
			return -1;
		}
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		EventStageInfo eventData = @object.GetComponent<StageDataTable>().getEventData();
		return (!bEventStage_) ? getNextStageNo(resultParam_.StageNo) : getNextEventStageNo(resultParam_.StageNo, eventData.EventNo);
	}

	private IEnumerator showNextSetup(bool bFading)
	{
		if (isShowNextSetup(bFading))
		{
			int stageNo = getNextSetupStageNo(bFading);
			bShowedNextSetup_ = true;
			yield return StartCoroutine(showSetup(stageNo));
		}
	}

	private bool isShowNextSetup(bool bFading)
	{
		if (bShowedNextSetup_)
		{
			return false;
		}
		int nextSetupStageNo = getNextSetupStageNo(bFading);
		if (nextSetupStageNo == -1)
		{
			return false;
		}
		if (!isQualifiedStarTerms(nextSetupStageNo))
		{
			return false;
		}
		return true;
	}

	private IEnumerator showSetup(int stageNo)
	{
		if (!Constant.Event.isEventStage(stageNo) && stageIconParent_.getIcon(stageNo) != null)
		{
			DialogSetup setupDialog = dialogManager.getDialog(DialogManager.eDialog.Setup) as DialogSetup;
			if (setupDialog.isOpen())
			{
				yield break;
			}
			yield return StartCoroutine(setupDialog.show(stageIconParent_.getIcon(stageNo), this));
		}
		bButtonEnable_ = true;
	}

	private IEnumerator moveNextStage(int stage)
	{
		if (stageIconParent_.getIcon(stage) == null)
		{
			GamePlayer player2 = appendGlobalObj(GlobalObjectParam.eObject.Player).GetComponent<GamePlayer>();
			StartCoroutine(player2.move(directArea_.transform.localPosition));
			mapNo_++;
			bNextStage_ = false;
			yield return StartCoroutine(transMap(areaCloud_[0].gameObject, mapNo_, MapMoveDirection.Up, true));
			GameObject currentIcon2 = stageIconParent_.getIcon(stage).gameObject;
			player2 = globalObj_.getObject(GlobalObjectParam.eObject.Player).GetComponent<GamePlayer>();
			StartCoroutine(player2.move(currentIcon2.transform.localPosition));
			StartCoroutine(mapCamera_.moveProd(currentIcon2.transform.localPosition));
			while (player2.GetComponent<iTween>() != null)
			{
				yield return 0;
			}
			while (mapCamera_.isMoving())
			{
				yield return 0;
			}
			Input.enable = true;
			Input.enable = false;
		}
		else
		{
			GameObject currentIcon = stageIconParent_.getIcon(stage).gameObject;
			GamePlayer player = appendGlobalObj(GlobalObjectParam.eObject.Player).GetComponent<GamePlayer>();
			StartCoroutine(player.move(currentIcon.transform.localPosition));
			StartCoroutine(mapCamera_.moveProd(currentIcon.transform.localPosition));
			while (player.GetComponent<iTween>() != null)
			{
				yield return 0;
			}
			while (mapCamera_.isMoving())
			{
				yield return 0;
			}
			showCurrentEffect(currentIcon);
			StageIcon icon = currentIcon.GetComponent<StageIcon>();
			icon.enable();
			yield return StartCoroutine(icon.playOpenProduct());
		}
	}

	private IEnumerator openToBeContinueDialog()
	{
		string message = MessageResource.Instance.getMessage(1467);
		Sound.Instance.playBgm(Sound.eBgm.BGM_000_Title, true);
		DialogConfirm dialog = (DialogConfirm)dialogManager.getDialog(DialogManager.eDialog.ToBeContinue);
		dialog.setMessage(message);
		dialog.setTitleEnable(true);
		yield return StartCoroutine(dialogManager.openDialog(dialog));
		while (dialog.isOpen())
		{
			yield return 0;
		}
		Sound.Instance.playBgm(Sound.eBgm.BGM_010_Map, true);
	}

	public IEnumerator closeSetupDialog(DialogBase setupDialog, DialogBase scoreDialog, int stageNo)
	{
		if (setupDialog != null)
		{
			dialogManager.StartCoroutine(dialogManager.closeDialog(setupDialog));
		}
		if (scoreDialog != null)
		{
			dialogManager.StartCoroutine(dialogManager.closeDialog(scoreDialog));
		}
		if (bStageSkip_)
		{
			yield return StartCoroutine(PaymentStageClear(stageNo));
			bStageSkip_ = false;
		}
		yield return 0;
	}

	private IEnumerator openAreaLockDialog(Area area)
	{
		while (Sound.Instance.BGMFading)
		{
			yield return 0;
		}
		Sound.Instance.stopBgmFading();
		Sound.Instance.playBgm(Sound.eBgm.BGM_000_Title, true);
		clearStage_ = setupStageIcon(currentStage: Bridge.PlayerData.getCurrentStage(), stageIconRoot: stageIconParent_, bNextStage: true);
		Sound.Instance.playBgm(Sound.eBgm.BGM_000_Title, true);
		DialogAreaLock dialog = dialogManager.getDialog(DialogManager.eDialog.AreaLock) as DialogAreaLock;
		Hashtable args = new Hashtable();
		dialog.setup(area, ref args);
		yield return StartCoroutine(dialog.show());
		while (dialog.isOpen())
		{
			yield return 0;
		}
		Sound.Instance.playBgm(Sound.eBgm.BGM_010_Map, true);
		if (args.ContainsKey("IsUnlockArea"))
		{
			yield return StartCoroutine(UnlockArea(args));
		}
		Sound.Instance.playBgm(Sound.eBgm.BGM_010_Map, true);
	}

	private IEnumerator UnlockArea(Hashtable args)
	{
		int currentStage = Bridge.PlayerData.getCurrentStage();
		gameData_ = SaveData.Instance.getGameData();
		otherData_ = gameData_.getOtherData();
		analyzeHash(args, true);
		bNextStage_ = isNextStage(currentStage);
		if (bNextStage_)
		{
			currentStage--;
		}
		else if (isLastStage(currentStage - 1))
		{
			currentStage--;
		}
		GameObject currentIcon = stageIconParent_.getIcon(currentStage).gameObject;
		showCurrentEffect(currentIcon);
		showPlayer(currentIcon);
		focus(currentStage);
		int stageNo = getNextSetupStageNo(false);
		directArea_ = setupArea(areas_, currentStage, bNextStage_, true);
		yield return StartCoroutine(nextStage());
	}

	private IEnumerator scrollMap(int stage)
	{
		if (stageIconParent_.getIcon(stage) != null)
		{
			GameObject currentIcon = stageIconParent_.getIcon(stage).gameObject;
			yield return StartCoroutine(mapCamera_.moveProd(currentIcon.transform.localPosition));
		}
	}

	private int getNextEventStageNo(int stageNo, int eventNo)
	{
		return Mathf.Min(stageNo + 1, Constant.Event.getHighestLevelStageNo(eventNo));
	}

	private int getNextStageNo(int stageNo)
	{
		return Mathf.Min(stageNo + 1, getLastStageNo());
	}

	private int getLastStageNo()
	{
		int num = iconTbl_.getMaxStageIconsNum() - 1;
		StageDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
		if (num > component.getStageData().Infos.Length - 1)
		{
			num = component.getStageData().Infos.Length - 1;
		}
		return num;
	}

	public bool isLastStage(int stageNo)
	{
		return getLastStageNo() == stageNo;
	}

	private bool isReclear()
	{
		if (!resultParam_.IsClear)
		{
			return false;
		}
		return Bridge.StageData.getClearCount(resultParam_.StageNo) > 1;
	}

	private bool isReclearEvent()
	{
		if (!bEventStage_)
		{
			return false;
		}
		if (!resultParam_.IsClear)
		{
			return false;
		}
		return Bridge.StageData.getClearCount(resultParam_.StageNo) > 1;
	}

	private bool isPlayedStage()
	{
		PartManager.ePart prevPart = partManager.prevPart;
		return prevPart == PartManager.ePart.Stage || prevPart == PartManager.ePart.Scenario;
	}

	private bool isQualifiedStarTerms(int stageNo)
	{
		if (getLastStageNo() < stageNo)
		{
			return true;
		}
		if (bNextStage_ && stageTbl_.getInfo(stageNo).Area > stageTbl_.getInfo(stageNo - 1).Area)
		{
			return true;
		}
		int areaStar = Bridge.StageData.getAreaStar(stageTbl_.getInfo(stageNo).Area - 1);
		return !bEventStage_ && stageTbl_.getInfo(stageNo).Common.EntryStars <= areaStar;
	}

	private IEnumerator loadDialog()
	{
		listItems_[0] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "mail_item")) as GameObject;
		listItems_[1] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "present_item")) as GameObject;
		listItems_[2] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "rank_player_item")) as GameObject;
		listItems_[3] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "rank_invite_item")) as GameObject;
		listItems_[7] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "rank_dummy_item")) as GameObject;
		listItems_[4] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "dayrank_player_item")) as GameObject;
		listItems_[5] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "invite_item")) as GameObject;
		listItems_[6] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Request_item")) as GameObject;
		listItems_[8] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Rescue_item")) as GameObject;
		GameObject[] array = listItems_;
		foreach (GameObject item in array)
		{
			Utility.setParent(item, base.transform, false);
			item.SetActive(false);
		}
		yield return StartCoroutine(dialogManager.load(PartManager.ePart.Map));
		if (!isPlayedStage())
		{
			DialogRanking dialog6 = dialogManager.getDialog(DialogManager.eDialog.Ranking) as DialogRanking;
			dialog6.init(listItems_[2], listItems_[3], listItems_[7]);
			dialog6.setup();
		}
		DialogDayRanking dialog5 = dialogManager.getDialog(DialogManager.eDialog.DayRanking) as DialogDayRanking;
		dialog5.init(listItems_[4], listItems_[3], listItems_[7]);
		DialogInvite dialog4 = dialogManager.getDialog(DialogManager.eDialog.Invite) as DialogInvite;
		dialog4.init(listItems_[5]);
		DialogRequest dialog3 = dialogManager.getDialog(DialogManager.eDialog.Request) as DialogRequest;
		dialog3.init(listItems_[6]);
		DialogMail dialog2 = dialogManager.getDialog(DialogManager.eDialog.Mail) as DialogMail;
		dialog2.init(listItems_[0], listItems_[1]);
		DialogFriendHelp dialog = dialogManager.getDialog(DialogManager.eDialog.FriendHelp) as DialogFriendHelp;
		dialog.init(listItems_[8]);
	}

	private void OnApplicationPause(bool pause)
	{
		if (partManager.bTransitionMap_)
		{
			return;
		}
		if (!pause)
		{
			StartCoroutine("sendInactive");
			GameAnalytics.traceActivation();
		}
		else
		{
			StopCoroutine("sendInactive");
			if (bInactive)
			{
				StopCoroutine("updateFriends");
				partManager.loginInputBack();
			}
			FriendUpdater.Instance.stop();
			bInactive = false;
			GameAnalytics.traceDeactivation();
		}
		if (pause)
		{
			Vungle.onPause();
		}
		else
		{
			Vungle.onResume();
		}
	}

	private void OnApplicationQuit()
	{
	}

	private void showWebView(bool show)
	{
		DialogInformation dialogInformation = dialogManager.getDialog(DialogManager.eDialog.Information) as DialogInformation;
		if (dialogInformation != null && dialogInformation.isOpen())
		{
			dialogInformation.showWebView(show);
		}
	}

	public IEnumerator sendInactive()
	{
		Debug.Log("sendInactive");
		while (isBonusGamePlayed || GlobalData.Instance.isResourceDownloading)
		{
			yield return 0;
		}
		DialogJewelShop jewelShop = dialogManager.getDialog(DialogManager.eDialog.JewelShop) as DialogJewelShop;
		DialogLuckyChance luckyChance = dialogManager.getDialog(DialogManager.eDialog.LuckyChance) as DialogLuckyChance;
		DialogCoinShop coinShop = dialogManager.getDialog(DialogManager.eDialog.CoinShop) as DialogCoinShop;
		DialogHeartShop heartShop = dialogManager.getDialog(DialogManager.eDialog.HeartShop) as DialogHeartShop;
		DialogAllShop allShop = dialogManager.getDialog(DialogManager.eDialog.AllShop) as DialogAllShop;
		DialogGiftJewelShop GiftShop = dialogManager.getDialog(DialogManager.eDialog.GiftJewelShop) as DialogGiftJewelShop;
		DialogInformation infoDialog = dialogManager.getDialog(DialogManager.eDialog.Information) as DialogInformation;
		if (infoDialog != null && infoDialog.bOping)
		{
			yield return 0;
		}
		showWebView(false);
		while (partManager.isLineLogin())
		{
			yield return 0;
		}
		if ((jewelShop != null && jewelShop.isOpen()) || (coinShop != null && coinShop.isOpen()) || (heartShop != null && heartShop.isOpen()) || (luckyChance != null && luckyChance.isBuying()) || (GiftShop != null && GiftShop.isBuying()) || (allShop != null && allShop.isOpen()))
		{
			while (NetworkMng.Instance.isDownloading())
			{
				yield return 0;
			}
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			showWebView(true);
			partManager.loginInputBack();
			bInactive = false;
		}
		else
		{
			if (!SNSCore.IsAuthorize)
			{
				yield break;
			}
			bInactive = true;
			if (!bInitialized || !bTransMap)
			{
				while (NetworkMng.Instance.isDownloading())
				{
					yield return 0;
				}
				yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
				if (bInactive)
				{
					showWebView(true);
					partManager.loginInputBack();
					bInactive = false;
				}
				yield break;
			}
			DialogCommon dialog = dialogManager.getDialog(DialogManager.eDialog.NetworkError) as DialogCommon;
			if (dialog.isOpen() || NetworkMng.Instance.isDownloading())
			{
				while (NetworkMng.Instance.isDownloading())
				{
					yield return 0;
				}
				yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
				if (bInactive)
				{
					showWebView(true);
					partManager.loginInputBack();
					bInactive = false;
				}
				yield break;
			}
			showWebView(false);
			yield return FriendUpdater.Instance.requestUpdate(partManager);
			if (!bInactive)
			{
				yield break;
			}
			NetworkMng.Instance.setup(null);
			yield return StartCoroutine(NetworkMng.Instance.download(API.Inactive, true, false));
			if (!bInactive)
			{
				yield break;
			}
			if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
			{
				partManager.loginInputBack();
				bInactive = false;
				yield break;
			}
			WWW www = NetworkMng.Instance.getWWW();
			InactiveData data = JsonMapper.ToObject<InactiveData>(www.text);
			GlobalData.Instance.getGameData().setCommonData(data, true);
			GlobalData.Instance.getGameData().setEventData(data);
			GlobalData.Instance.getGameData().setDailyMissionData(data);
			GlobalData.Instance.getGameData().helpDataSize = data.helpDataSize;
			GlobalData.Instance.getGameData().helpMove = data.helpMove;
			GlobalData.Instance.getGameData().helpTime = data.helpTime;
			GlobalData.Instance.getGameData().bonusChanceLv = data.bonusChanceLv;
			GlobalData.Instance.getGameData().saleArea = data.saleArea;
			GlobalData.Instance.getGameData().areaSalePercent = data.areaSalePercent;
			GlobalData.Instance.getGameData().isAreaCampaign = data.isAreaCampaign;
			GlobalData.Instance.getGameData().saleStageItemArea = data.saleStageItemArea;
			GlobalData.Instance.getGameData().stageItemAreaSalePercent = data.stageItemAreaSalePercent;
			GlobalData.Instance.getGameData().isStageItemAreaCampaign = data.isStageItemAreaCampaign;
			updateAreaSale();
			updateStageItemAreaSale();
			GlobalData.Instance.getGameData().gachaTicket = data.gachaTicket;
			Debug.Log("gachaTicket = " + data.gachaTicket);
			EventMenu.updateGetTime();
			ChallengeMenu.updateGetTime();
			CollaborationMenu.updateGetTime();
			GlobalData.Instance.getGameData().setParkData(null, null, data.thanksList, data.buildings, data.mapReleaseNum);
			SaveData.Instance.getParkData().UpdatePlacedData(data.buildings);
			DailyMission.updateGetTime();
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
					dailyMissionValueSetting();
					if (dailyMission.gameObject.activeSelf)
					{
						yield return StartCoroutine(dailyMission.dailyMissionInfoSetup());
					}
				}
			}
			else
			{
				dailyMission.gameObject.SetActive(false);
			}
			if (mainMenu_ != null && mainMenu_.getHeartMenu() != null)
			{
				mainMenu_.update();
				mainMenu_.getHeartMenu().updateRemainingTime();
			}
			if (stageTbl_ != null)
			{
				ResponceHeaderData headerData = NetworkUtility.createResponceHeaderData(www);
				if (stageTbl_.getUpdateInfo(headerData) != 0)
				{
					yield return StartCoroutine(NetworkUtility.downloadPlayerData(true, false));
					EventMenu.updateGetTime();
					ChallengeMenu.updateGetTime();
					CollaborationMenu.updateGetTime();
					DailyMission.updateGetTime();
					if (!bInactive)
					{
						yield break;
					}
					if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
					{
						partManager.loginInputBack();
						bInactive = false;
						yield break;
					}
					yield return StartCoroutine(stageTbl_.download(headerData, true, false));
					if (!bInactive)
					{
						yield break;
					}
					if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
					{
						partManager.loginInputBack();
						bInactive = false;
						yield break;
					}
				}
			}
			DialogSetup setup = dialogManager.getDialog(DialogManager.eDialog.Setup) as DialogSetup;
			if (setup.isOpen())
			{
				setup.OnApplicationResumeSetupDialog();
			}
			yield return StartCoroutine("updateFriends");
			if (!bInactive)
			{
				yield break;
			}
			while (NetworkMng.Instance.isDownloading())
			{
				yield return 0;
			}
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			if (bInactive)
			{
				if (eventMenu_ != null)
				{
					eventMenu_.updateEnable(PartManager.ePart.Map);
				}
				if (collaboMenu_ != null)
				{
					collaboMenu_.updateEnable(PartManager.ePart.Map);
				}
				checkAvatarCampaign();
				DialogAvatarGacha dialog_ag = dialogManager.getDialog(DialogManager.eDialog.AvatarGacha) as DialogAvatarGacha;
				dialog_ag.setActiveCampaignMessage();
				if (bossMenu_ != null)
				{
					bossMenu_.updateEnable(PartManager.ePart.Map, partManager.fade);
				}
				showWebView(true);
				partManager.loginInputBack();
				bInactive = false;
			}
		}
	}

	public IEnumerator updateSaleAreaData()
	{
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(API.Inactive, true, false));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			InactiveData data = JsonMapper.ToObject<InactiveData>(www.text);
			GlobalData.Instance.getGameData().setCommonData(data, true);
			GlobalData.Instance.getGameData().setEventData(data);
			GlobalData.Instance.getGameData().setDailyMissionData(data);
			GlobalData.Instance.getGameData().helpDataSize = data.helpDataSize;
			GlobalData.Instance.getGameData().helpMove = data.helpMove;
			GlobalData.Instance.getGameData().helpTime = data.helpTime;
			GlobalData.Instance.getGameData().bonusChanceLv = data.bonusChanceLv;
			GlobalData.Instance.getGameData().saleArea = data.saleArea;
			GlobalData.Instance.getGameData().areaSalePercent = data.areaSalePercent;
			GlobalData.Instance.getGameData().isAreaCampaign = data.isAreaCampaign;
			GlobalData.Instance.getGameData().saleStageItemArea = data.saleStageItemArea;
			GlobalData.Instance.getGameData().stageItemAreaSalePercent = data.stageItemAreaSalePercent;
			GlobalData.Instance.getGameData().isStageItemAreaCampaign = data.isStageItemAreaCampaign;
			updateAreaSale();
			updateStageItemAreaSale();
			GlobalData.Instance.getGameData().gachaTicket = data.gachaTicket;
			Debug.Log("gachaTicket = " + data.gachaTicket);
			EventMenu.updateGetTime();
			ChallengeMenu.updateGetTime();
			CollaborationMenu.updateGetTime();
			DailyMission.updateGetTime();
			GlobalData.Instance.getGameData().setParkData(null, null, data.thanksList, data.buildings, data.mapReleaseNum);
			SaveData.Instance.getParkData().UpdatePlacedData(data.buildings);
		}
	}

	public override IEnumerator OnDestroyCB()
	{
		Vungle.onAdFinishedEvent -= VungleAdFinishedEvent;
		TutorialManager.Instance.unload();
		OnTransition();
		setActiveGlobalObjs(false);
		if (uiCamera_ != null)
		{
			uiCamera_.clearFlags = clearFlags_;
		}
		yield break;
	}

	private WWW OnCreateMailListWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		return WWWWrap.create("mail/list/");
	}

	private IEnumerator updateFriends()
	{
		if (globalObj_ == null)
		{
			yield break;
		}
		GameObject root = globalObj_.getObject(GlobalObjectParam.eObject.FriendRoot);
		foreach (Transform t in root.transform)
		{
			UnityEngine.Object.Destroy(t.gameObject);
		}
		GameObject friend = globalObj_.getObject(GlobalObjectParam.eObject.Friend);
		StageIconParent iconRoot = globalObj_.getObject(GlobalObjectParam.eObject.StageIcon).GetComponent<StageIconParent>();
		List<int> stageNoList = new List<int>();
		int[] indexArray = new int[DummyPlayFriendData.FriendNum];
		for (int k = 0; k < indexArray.Length; k++)
		{
			indexArray[k] = k;
		}
		System.Random rand = new System.Random();
		for (int j = indexArray.Length - 1; j > 0; j--)
		{
			int l = rand.Next(j + 1);
			int item = indexArray[j];
			indexArray[j] = indexArray[l];
			indexArray[l] = item;
		}
		friendsData.Clear();
		for (int i = 0; i < DummyPlayFriendData.FriendNum; i++)
		{
			UserData data = DummyPlayFriendData.DummyFriends[indexArray[i]];
			int stageNo = data.StageNo;
			if (stageNo > getLastStageNo() + 1)
			{
				stageNo = getLastStageNo() + 1;
			}
			if (iconRoot.getIcon(stageNo - 1) == null)
			{
				friendsData.Add(data);
				continue;
			}
			GameObject obj = UnityEngine.Object.Instantiate(friend) as GameObject;
			Friend f = obj.GetComponent<Friend>();
			Utility.setParent(obj, root.transform, false);
			f.createMaterial();
			Vector3 pos = Vector3.zero;
			if (!((float)getOverlapCount(stageNoList, stageNo) >= Constant.FriendPlaceOverlapCount))
			{
				pos = iconRoot.getIcon(stageNo - 1).transform.localPosition;
			}
			f.setup(pos, stageNo, this, i);
			friendsData.Add(data);
			int lap = getOverlapCount(stageNoList, stageNo);
			f.setPlace(pos, (Friend.ePalce)lap);
			stageNoList.Add(stageNo);
			obj.SetActive(true);
			StartCoroutine(f.loadTexture(data.URL, true, data));
			yield return 0;
		}
	}

	private int getOverlapCount(List<int> list, int stageNo)
	{
		int num = 0;
		foreach (int item in list)
		{
			if (item == stageNo)
			{
				num++;
			}
		}
		return num;
	}

	private IEnumerator openNoneEventDioalog()
	{
		yield return dialogManager.StartCoroutine(openCommonDialog(58));
	}

	private IEnumerator openCommonDialog(int msgID)
	{
		DialogCommon dialog = dialogManager.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		dialog.setup(msgID, null, null, true);
		dialog.setButtonActive(DialogCommon.eBtn.Close, false);
		yield return dialogManager.StartCoroutine(dialogManager.openDialog(dialog));
		while (dialog.isOpen())
		{
			yield return 0;
		}
	}

	private IEnumerator openFirstGachaDialog()
	{
		otherData_.setFlag(SaveOtherData.eFlg.FirstGachaOpen, true);
		otherData_.save();
		DialogCommon dialog = dialogManager.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		dialog.setup(8820, OnDecide, null, true);
		dialog.setButtonActive(DialogCommon.eBtn.Close, false);
		yield return dialogManager.StartCoroutine(dialogManager.openDialog(dialog));
		while (dialog.isOpen())
		{
			yield return 0;
		}
	}

	private IEnumerator openParkAnnounce()
	{
		otherData_.setFlag(SaveOtherData.eFlg.OpenParkAnnounce, true);
		otherData_.save();
		DialogParkRelease dialog = dialogManager.getDialog(DialogManager.eDialog.ParkReleaseNotice) as DialogParkRelease;
		yield return dialogManager.StartCoroutine(dialogManager.openDialog(dialog));
		while (dialog.isOpen())
		{
			yield return 0;
		}
	}

	private void checkStageSixClear()
	{
		bool flag = Bridge.PlayerData.getCurrentStage() >= 6;
		gachaButton_.Find("gacya_button/popup_root").GetComponent<UIButton>().setEnable(flag);
		gachaButton_.Find("gacya_button").GetComponent<TweenRotation>().enabled = flag;
		if (flag)
		{
			gachaButton_.Find("label").GetComponent<UISprite>().color = Color.white;
			checkAvatarCampaign();
		}
		else
		{
			gachaButton_.Find("label").GetComponent<UISprite>().color = Color.gray;
		}
		Transform transform = dialogManager.getDialog(DialogManager.eDialog.Setup).gameObject.transform.Find("avatar_button");
		transform.gameObject.SetActive(flag);
		UIButton[] componentsInChildren = bagMenu_.gameObject.transform.Find("09_avatar/avatar_button").GetComponentsInChildren<UIButton>(true);
		UIButton[] array = componentsInChildren;
		foreach (UIButton uIButton in array)
		{
			uIButton.setEnable(flag);
		}
		GlobalData.Instance.isBasicSkill = flag;
		bool enable = Bridge.PlayerData.getCurrentStage() >= 10;
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI2);
		if (!@object)
		{
			return;
		}
		Transform transform2 = @object.transform.Find("sendOneStage/goToPark/park_Button");
		if ((bool)transform2)
		{
			UIButton component = transform2.GetComponent<UIButton>();
			if ((bool)component)
			{
				component.setEnable(enable);
			}
		}
	}

	private IEnumerator OnDecide()
	{
		DialogAvatarGacha gachaDialog = dialogManager.getDialog(DialogManager.eDialog.AvatarGacha) as DialogAvatarGacha;
		bool isFree2 = false;
		Hashtable h = Hash.GachaTop();
		NetworkMng.Instance.setup(h);
		yield return StartCoroutine(NetworkMng.Instance.download(API.GachaTop, true, true));
		while (NetworkMng.Instance.isDownloading())
		{
			yield return null;
		}
		if (NetworkMng.Instance.getResultCode() == eResultCode.Success)
		{
			WWW wwww = NetworkMng.Instance.getWWW();
			GachaTop resultData = JsonMapper.ToObject<GachaTop>(wwww.text);
			GlobalData.Instance.getGameData().setGachaTopData(resultData);
			isFree2 = GlobalData.Instance.getGameData().isFirstGacha;
			gachaDialog.setup(isFree2);
			yield return StartCoroutine(dialogManager.openDialog(gachaDialog));
		}
	}

	private IEnumerator openStarRewardDialog()
	{
		DialogCommon dialog = dialogManager.getDialog(DialogManager.eDialog.StarReward) as DialogCommon;
		dialog.setup(4020, null, null, true);
		dialog.setButtonActive(DialogCommon.eBtn.Close, false);
		dialog.transform.Find("window/chara/anm1").GetComponent<UISprite>().spriteName = "UI_chara_00_019";
		yield return dialogManager.StartCoroutine(dialogManager.openDialog(dialog));
		while (dialog.isOpen())
		{
			yield return 0;
		}
	}

	private IEnumerator forceRecover()
	{
		StopCoroutine("sendInactive");
		bInactive = true;
		while (partManager.isLineLogin())
		{
			yield return 0;
		}
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(API.Inactive, true, false));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			bInactive = false;
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		InactiveData data = JsonMapper.ToObject<InactiveData>(www.text);
		GlobalData.Instance.getGameData().setCommonData(data, true);
		GlobalData.Instance.getGameData().setEventData(data);
		GlobalData.Instance.getGameData().setDailyMissionData(data);
		GlobalData.Instance.getGameData().helpDataSize = data.helpDataSize;
		GlobalData.Instance.getGameData().helpMove = data.helpMove;
		GlobalData.Instance.getGameData().helpTime = data.helpTime;
		GlobalData.Instance.getGameData().bonusChanceLv = data.bonusChanceLv;
		GlobalData.Instance.getGameData().progressStageNo = data.progressStageNo;
		GlobalData.Instance.getGameData().saleArea = data.saleArea;
		GlobalData.Instance.getGameData().areaSalePercent = data.areaSalePercent;
		GlobalData.Instance.getGameData().isAreaCampaign = data.isAreaCampaign;
		GlobalData.Instance.getGameData().saleStageItemArea = data.saleStageItemArea;
		GlobalData.Instance.getGameData().stageItemAreaSalePercent = data.stageItemAreaSalePercent;
		GlobalData.Instance.getGameData().isStageItemAreaCampaign = data.isStageItemAreaCampaign;
		GlobalData.Instance.getGameData().setParkData(null, null, data.thanksList, data.buildings, data.mapReleaseNum);
		SaveData.Instance.getParkData().UpdatePlacedData(data.buildings);
		EventMenu.updateGetTime();
		ChallengeMenu.updateGetTime();
		CollaborationMenu.updateGetTime();
		DailyMission.updateGetTime();
		if (DailyMission.isTermClear())
		{
			Mission respons_mission2 = JsonMapper.ToObject<Mission>(www.text);
			GlobalData.Instance.setDailyMissionData(respons_mission2);
			Network.DailyMission dMission = GlobalData.Instance.getDailyMissionData();
			if (dMission == null)
			{
				NetworkMng.Instance.setup(Hash.DailyMissionCreate());
				yield return StartCoroutine(NetworkMng.Instance.download(API.DailyMissionCreate, false, false));
				if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
				{
					WWW www_dMission = NetworkMng.Instance.getWWW();
					respons_mission2 = JsonMapper.ToObject<Mission>(www_dMission.text);
					GlobalData.Instance.setDailyMissionData(respons_mission2);
					DailyMission.bMissionCreate = true;
					if (dailyMission != null)
					{
						dailyMissionValueSetting();
						if (dailyMission.gameObject.activeSelf)
						{
							yield return StartCoroutine(dailyMission.dailyMissionInfoSetup());
						}
					}
				}
			}
		}
		if (stageTbl_ != null)
		{
			ResponceHeaderData headerData = NetworkUtility.createResponceHeaderData(www);
			if (stageTbl_.getUpdateInfo(headerData) != 0 || partManager.prevPart == PartManager.ePart.EventMap || partManager.prevPart == PartManager.ePart.ChallengeMap || partManager.prevPart == PartManager.ePart.CollaborationMap)
			{
				yield return StartCoroutine(NetworkUtility.downloadPlayerData(true, false));
				if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
				{
					EventMenu.updateGetTime();
					ChallengeMenu.updateGetTime();
					CollaborationMenu.updateGetTime();
					DailyMission.updateGetTime();
				}
				if (!bInactive)
				{
					yield break;
				}
				if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
				{
					bInactive = false;
					yield break;
				}
				yield return StartCoroutine(stageTbl_.download(headerData, true, false));
				if (!bInactive)
				{
					yield break;
				}
				if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
				{
					bInactive = false;
					yield break;
				}
			}
		}
		yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		bInactive = false;
	}

	private IEnumerator loadStageIcons(GameObject worldMap, StageIconData data)
	{
		if (stageIconParent_ == null)
		{
			yield return StartCoroutine(createStageIcons(worldMap, data));
			yield break;
		}
		GameObject stageIconRoot = stageIconParent_.gameObject;
		Utility.setParent(stageIconRoot, worldMap.transform, false);
		stageIconRoot.transform.localPosition = Vector3.zero;
		stageIconRoot.transform.localScale = Vector3.one;
		for (int i = 0; i < 90; i++)
		{
			icons[i].gameObject.SetActive(true);
			if (i < data.IconNum)
			{
				IconData iconData = data.IconDatas[i];
				icons[i].setStageNo(iconData.StageNo);
				Vector3 pos = iconData.Pos;
				icons[i].gameObject.transform.localPosition = new Vector3(pos.x, pos.y, icons[i].gameObject.transform.localPosition.z);
			}
			else
			{
				icons[i].setStageNo(-1);
				icons[i].gameObject.SetActive(false);
			}
			if (i % 3 == 0)
			{
				yield return 0;
			}
		}
		stageIconParent_.setup(icons);
		stageIconRoot.SetActive(false);
	}

	private IEnumerator createStageIcons(GameObject worldMap, StageIconData data)
	{
		GameObject stageIconRoot = new GameObject(GlobalObjectParam.getName(GlobalObjectParam.eObject.StageIcon));
		globalObj_.appendObject(stageIconRoot, GlobalObjectParam.eObject.StageIcon, false);
		Utility.setParent(stageIconRoot, worldMap.transform, false);
		stageIconRoot.transform.localPosition = Vector3.zero;
		stageIconRoot.transform.localScale = Vector3.one;
		icons = new StageIcon[90];
		GameObject objBase = ResourceLoader.Instance.loadGameObject("Prefabs/", "stageicon");
		for (int i = 0; i < 90; i++)
		{
			GameObject stageIcon = UnityEngine.Object.Instantiate(objBase) as GameObject;
			if (i < data.IconNum)
			{
				IconData iconData = data.IconDatas[i];
				icons[i] = stageIcon.GetComponent<StageIcon>();
				Utility.setParent(stageIcon, stageIconRoot.transform, false);
				icons[i].setStageNo(iconData.StageNo);
				Vector3 pos = iconData.Pos;
				stageIcon.transform.localPosition = new Vector3(pos.x, pos.y, stageIcon.transform.localPosition.z);
				stageIcon.SetActive(true);
			}
			else
			{
				icons[i] = stageIcon.GetComponent<StageIcon>();
				Utility.setParent(stageIcon, stageIconRoot.transform, false);
				icons[i].setStageNo(-1);
				stageIcon.SetActive(false);
			}
			yield return 0;
		}
		stageIconRoot.AddComponent<StageIconParent>().setup(icons);
		stageIconRoot.SetActive(false);
	}

	private IEnumerator loadTreasureBox(GameObject worldMap, StageIconData data, SaveGameData gameData)
	{
		GameObject dataTableObj = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		TreasureDataTable treasureTbl = dataTableObj.GetComponent<TreasureDataTable>();
		treasureTbl.load();
		GameObject boxRoot = new GameObject(GlobalObjectParam.getName(GlobalObjectParam.eObject.TreasureBox));
		globalObj_.appendObject(boxRoot, GlobalObjectParam.eObject.TreasureBox, false);
		Utility.setParent(boxRoot, worldMap.transform, false);
		int totalStar = Bridge.StageData.getTotalStar();
		for (int i = 0; i < data.BoxNum; i++)
		{
			BoxData boxData = data.BoxDatas[i];
			GameObject box = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "TreasureBox")) as GameObject;
			Utility.setParent(box, boxRoot.transform, false);
			box.GetComponent<TreasureBox>().init(totalStar, treasureTbl.getInfo(boxData.ID), boxData);
			yield return 0;
		}
		boxRoot.AddComponent<TreasureBoxParent>().setup();
		boxRoot.SetActive(false);
	}

	private IEnumerator createFriend(GameObject worldMap)
	{
		GameObject root = new GameObject(GlobalObjectParam.getName(GlobalObjectParam.eObject.FriendRoot));
		globalObj_.appendObject(root, GlobalObjectParam.eObject.FriendRoot, false);
		Utility.setParent(root, worldMap.transform, false);
		yield break;
	}

	public void UpdateCharaIcon()
	{
		GameObject gameObject = appendGlobalObj(GlobalObjectParam.eObject.MapMainUI2);
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

	private void initArea(GameObject worldMap, GameObject gateEffect)
	{
		Area[] componentsInChildren = worldMap.GetComponentsInChildren<Area>(true);
		Area[] array = componentsInChildren;
		foreach (Area area in array)
		{
			area.init(gateEffect, base.gameObject);
		}
	}

	private IEnumerator setupMapObject(GameObject worldMap, StageIconData iconData)
	{
		GameObject gateEffect = loadGlobalObj("Prefabs/", GlobalObjectParam.eObject.GateEffect);
		GameObject effect = loadGlobalObj("Prefabs/", GlobalObjectParam.eObject.CurrentStageEffect);
		Utility.setParent(effect, worldMap.transform, false);
		GameObject player = loadGlobalObj("Prefabs/", GlobalObjectParam.eObject.Player);
		Utility.setParent(player, worldMap.transform, false);
		yield return StartCoroutine(loadStageIcons(worldMap, iconData));
		yield return StartCoroutine(loadTreasureBox(worldMap, iconData, SaveData.Instance.getGameData()));
		yield return StartCoroutine(createFriend(worldMap));
		initArea(worldMap, gateEffect);
	}

	public void OnTransition()
	{
		globalObj_.unload(GlobalObjectParam.eObject.GateEffect);
		globalObj_.unload(GlobalObjectParam.eObject.CurrentStageEffect);
		globalObj_.unload(GlobalObjectParam.eObject.Player);
		globalObj_.unload(GlobalObjectParam.eObject.StageIcon);
		globalObj_.unload(GlobalObjectParam.eObject.TreasureBox);
		globalObj_.unload(GlobalObjectParam.eObject.FriendRoot);
	}

	private void OnChangeMap()
	{
		globalObj_.unload(GlobalObjectParam.eObject.GateEffect);
		globalObj_.unload(GlobalObjectParam.eObject.CurrentStageEffect);
		globalObj_.unload(GlobalObjectParam.eObject.Player);
		stageIconParent_.transform.parent = map_.transform.parent;
		globalObj_.unload(GlobalObjectParam.eObject.TreasureBox);
		globalObj_.unload(GlobalObjectParam.eObject.FriendRoot);
	}

	private GameObject loadGlobalObj(string path, GlobalObjectParam.eObject objType)
	{
		GameObject gameObject = globalObj_.load(path, objType, false);
		gameObject.SetActive(false);
		return gameObject;
	}

	public IEnumerator SendMap(int mapNo, MapMoveDirection direction, bool isAreaMove)
	{
		mapNo_ = mapNo;
		yield return StartCoroutine(transMap(base.gameObject, mapNo, direction, isAreaMove));
	}

	public int getMapNo()
	{
		return mapNo_;
	}

	private IEnumerator transMap(GameObject caller, int mapNo, MapMoveDirection direction, bool isAreaMove)
	{
		Input.enable = false;
		bTransMap = false;
		float move_y = 0f;
		switch (direction)
		{
		case MapMoveDirection.Up:
			move_y = -2600f;
			break;
		case MapMoveDirection.Down:
			move_y = 2600f;
			break;
		case MapMoveDirection.Init:
		case MapMoveDirection.Jump:
			yield return StartCoroutine(playCloseCloudEff());
			break;
		}
		AreaCloud ac = caller.GetComponent<AreaCloud>();
		if (direction == MapMoveDirection.Init || direction == MapMoveDirection.Jump)
		{
			GameObject cloud_obj = globalObj_.getObject(GlobalObjectParam.eObject.Cloud);
			Animation anim = cloud_obj.GetComponentInChildren<Animation>();
			while (anim.isPlaying)
			{
				yield return 0;
			}
		}
		else
		{
			iTween.MoveTo(map_, iTween.Hash("y", move_y, "easetype", iTween.EaseType.easeInSine, "time", 1.2f, "islocal", true));
			ac.setArrowEndable(false);
			yield return StartCoroutine(ac.moveToCenter());
			while (map_.GetComponent<iTween>() != null)
			{
				yield return 0;
			}
		}
		if (!Bridge.StageData.isClear(0))
		{
			Transform tran = uiRoot.transform.Find("Tap_Panel(Clone)");
			if (tran != null)
			{
				tran.gameObject.SetActive(mapNo_ == 0);
			}
		}
		GameObject loading = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.Loading);
		loading.SetActive(true);
		loading.transform.Find("bg").gameObject.SetActive(false);
		OnChangeMap();
		UnityEngine.Object.Destroy(map_);
		if (direction == MapMoveDirection.Jump)
		{
			dialogManager.StartCoroutine(dialogManager.closeDialog(DialogManager.eDialog.SendMap));
		}
		map_ = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "BG_Panel_" + (mapNo_ + 1).ToString("00"))) as GameObject;
		map_.SetActive(false);
		Utility.setParent(map_, uiRoot.transform, false);
		yield return StartCoroutine(setupMapObject(map_, iconTbl_.getDataByIndex(mapNo_)));
		map_.SetActive(true);
		boxParent_ = appendGlobalObj(GlobalObjectParam.eObject.TreasureBox).GetComponent<TreasureBoxParent>();
		int currentStage = Bridge.PlayerData.getCurrentStage();
		if (isLastStage(currentStage - 1))
		{
			currentStage--;
		}
		bSetupDialogOpen_ = bNextStage_;
		if (bNextStage_)
		{
			bNextStage_ = false;
		}
		clearStage_ = setupStageIcon(stageIconParent_, currentStage, bNextStage_);
		openBox_ = setupTreasureBox(boxParent_);
		mapCamera_.setMoveRange(mapCamera_.DefMoveRangeMax_[mapNo_], mapCamera_.DefMoveRange_[mapNo_]);
		GamePlayer player = globalObj_.getObject(GlobalObjectParam.eObject.Player).GetComponent<GamePlayer>();
		yield return StartCoroutine(player.loadTexture());
		areas_ = map_.GetComponentsInChildren<Area>(true);
		directArea_ = setupArea(bNextStage: otherData_.isFlag(SaveOtherData.eFlg.AllClear) || bNextStage_, areas: areas_, currentStage: currentStage);
		if (directArea_ != null)
		{
			directArea_.mapCamera_ = mapCamera_;
		}
		if (stageIconParent_.getIcon(currentStage) != null)
		{
			GameObject currentIcon = stageIconParent_.getIcon(currentStage).gameObject;
			showCurrentEffect(currentIcon);
			showPlayer(currentIcon);
		}
		else
		{
			player.gameObject.SetActive(false);
		}
		setActiveGlobalObjs(true);
		boxParent_.gameObject.SetActive(false);
		if (!partManager.isLineLogin())
		{
			if (partManager.currentPart == PartManager.ePart.Title)
			{
				yield return StartCoroutine(partManager.hspLogin(false, false));
			}
			FriendUpdater.Instance.requestUpdate(partManager);
			while (FriendUpdater.Instance.isUpdate)
			{
				yield return null;
			}
			yield return StartCoroutine(updateFriends());
		}
		yield return Resources.UnloadUnusedAssets();
		loading.SetActive(false);
		Vector3 focus_pos = Vector3.zero;
		switch (direction)
		{
		case MapMoveDirection.Up:
			focus_pos = stageIconParent_.getIcon(90 * mapNo_ + 1).transform.localPosition;
			iTween.MoveTo(caller, iTween.Hash("y", ac.getBasePosY(mapNo_, 1), "easetype", iTween.EaseType.easeInSine, "time", 0.6f, "islocal", true));
			break;
		case MapMoveDirection.Down:
			focus_pos = stageIconParent_.getIcon(90 * (mapNo_ + 1) - 1).transform.localPosition;
			iTween.MoveTo(caller, iTween.Hash("y", ac.getBasePosY(mapNo_, 0), "easetype", iTween.EaseType.easeInSine, "time", 0.6f, "islocal", true));
			break;
		case MapMoveDirection.Init:
			focus_pos = stageIconParent_.getIcon(0).transform.localPosition;
			break;
		case MapMoveDirection.Jump:
			focus_pos = stageIconParent_.getIcon(90 * mapNo_).transform.localPosition;
			break;
		}
		if (areaCloud_[0].gameObject.transform.localPosition.y <= areaCloud_[0].GetComponent<AreaCloud>().Base_Pos_Y[mapNo].x)
		{
			Vector3 tmp = new Vector3(0f, 0f, -30f)
			{
				y = areaCloud_[0].GetComponent<AreaCloud>().Base_Pos_Y[mapNo].x
			};
			areaCloud_[0].gameObject.transform.localPosition = tmp;
		}
		if (!isAreaMove)
		{
			if (direction == MapMoveDirection.Jump)
			{
				StageIcon si = stageIconParent_.getIcon(currentStage);
				if (si == null)
				{
					mapCamera_.transform.localPosition = focus_pos;
				}
				else
				{
					mapCamera_.focus(si.transform.localPosition);
				}
			}
			else
			{
				mapCamera_.transform.localPosition = focus_pos;
			}
			if (direction < MapMoveDirection.Init)
			{
				caller.transform.localPosition = (focus_pos += Vector3.back * 100f);
			}
		}
		else
		{
			Transform root = areas_[0].transform.parent;
			GameObject first_area = root.Find("area_first").gameObject;
			focus_pos = first_area.transform.position;
			mapCamera_.transform.position = focus_pos;
			caller.transform.position = focus_pos;
			caller.transform.localPosition += Vector3.back * 100f;
			player.gameObject.transform.position = focus_pos;
		}
		if (eventMenu_ != null)
		{
			eventMenu_.updateEnable(PartManager.ePart.Map);
		}
		if (collaboMenu_ != null)
		{
			collaboMenu_.updateEnable(PartManager.ePart.Map);
		}
		while (caller.GetComponent<iTween>() != null)
		{
			yield return 0;
		}
		if (direction < MapMoveDirection.Init)
		{
			ac.setArrowEndable(true);
		}
		for (int i = 0; i < 2; i++)
		{
			areaCloud_[i].gameObject.SetActive(true);
			areaCloud_[i].setup(base.gameObject, mapCamera_, i, mapNo_);
		}
		if (mapNo_ == 0)
		{
			areaCloud_[1].gameObject.SetActive(false);
		}
		else if (mapNo_ == iconTbl_.getMaxMapNum() - 1)
		{
			areaCloud_[0].gameObject.SetActive(false);
		}
		if (mapNo_ != 0)
		{
		}
		if (direction == MapMoveDirection.Init || direction == MapMoveDirection.Jump)
		{
			AreaCloud[] array = areaCloud_;
			foreach (AreaCloud aCloud in array)
			{
				Vector3 acPos = aCloud.transform.localPosition;
				aCloud.transform.localPosition = new Vector3(mapCamera_.transform.localPosition.x, acPos.y, acPos.z);
			}
			yield return StartCoroutine(playOpenCloudEff());
		}
		bTransMap = true;
		Input.enable = true;
		if (eventMenu_.gameObject.activeSelf && !otherData_.isFlag(SaveOtherData.eFlg.TutorialEvent))
		{
			BoxCollider col2 = eventMenu_.GetComponentInChildren<BoxCollider>();
			col2.enabled = false;
			eventMenu_.transform.localPosition += Vector3.back * 100f;
			TutorialManager.Instance.load(-2, uiRoot);
			int count2 = Input.forceEnable();
			yield return StartCoroutine(TutorialManager.Instance.play(-2, TutorialDataTable.ePlace.Setup, uiRoot, null, null));
			Input.revertForceEnable(count2);
			TutorialManager.Instance.unload(-2);
			eventMenu_.transform.localPosition += Vector3.forward * 100f;
			col2.enabled = true;
			otherData_.setFlag(SaveOtherData.eFlg.TutorialEvent, true);
		}
		else if (collaboMenu_.gameObject.activeSelf && !otherData_.isFlag(SaveOtherData.eFlg.TutorialGachaMukCollabo))
		{
			BoxCollider col = collaboMenu_.GetComponentInChildren<BoxCollider>();
			col.enabled = false;
			collaboMenu_.transform.localPosition += Vector3.back * 100f;
			TutorialManager.Instance.load(-17, uiRoot);
			int count = Input.forceEnable();
			yield return StartCoroutine(TutorialManager.Instance.play(-17, TutorialDataTable.ePlace.ReadyGoEnd, uiRoot, null, null));
			Input.revertForceEnable(count);
			TutorialManager.Instance.unload(-17);
			collaboMenu_.transform.localPosition += Vector3.forward * 100f;
			col.enabled = true;
			otherData_.setFlag(SaveOtherData.eFlg.TutorialGachaMukCollabo, true);
		}
		otherData_.save();
	}

	public void checkAvatarCampaign()
	{
		if (gachaButton_ != null && Bridge.PlayerData.getCurrentStage() >= 6)
		{
			gachaButton_.Find("campaign_02").gameObject.SetActive(GlobalData.Instance.getGameData().isGachaUpCampaign);
			gachaButton_.Find("campaign").gameObject.SetActive(GlobalData.Instance.getGameData().isGachaSaleCampaign);
		}
	}

	private WWW OnCreateAreaLockWWW(Hashtable args)
	{
		int num = Bridge.PlayerData.getCurrentStage() + 2;
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("stageNo", num);
		return WWWWrap.create("stage/unlockcheck/");
	}

	private WWW OnCreateKeyGetWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("type", 8);
		return WWWWrap.create("reward/add/");
	}

	private void setInviteRewardPopup()
	{
		Constant.Reward reward = new Constant.Reward();
		reward.RewardType = Constant.eMoney.Coin;
		reward.Num = 1000;
		InviteBasicReward inviteBasicReward = GlobalData.Instance.getGameData().inviteBasicReward;
		reward.RewardType = (Constant.eMoney)inviteBasicReward.rewardType;
		reward.Num = inviteBasicReward.reward;
		bagMenu_.setInviteRewardPopup(reward);
	}

	public void VungleAdFinishedEvent(AdFinishedEventArgs args)
	{
		UnityEngine.Debug.Log(" args.TimeWatched : " + args.TimeWatched);
		UnityEngine.Debug.Log(" args.TotalDuration : " + args.TotalDuration);
		UnityEngine.Debug.Log(" args.WasCallToActionClicked : " + args.WasCallToActionClicked);
		UnityEngine.Debug.Log(" args.IsCompletedView : " + args.IsCompletedView);
		StartCoroutine(getMonetizationAD());
	}

	public IEnumerator PaymentStageClear(int clearStageNo)
	{
		bShowedNextSetup_ = false;
		bNextStage_ = true;
		if (!isQualifiedStarTerms(clearStageNo + 1))
		{
			bNextStage_ = false;
		}
		resultParam_.StageNo = clearStageNo;
		resultParam_.IsRetry = false;
		resultParam_.IsClear = true;
		resultParam_.IsProgressOpen = bNextStage_;
		resultParam_.IsExit = false;
		clearStage_ = stageIconParent_.getIcon(clearStageNo);
		if (clearStage_ != null)
		{
			yield return StartCoroutine(clearStage_.setup(true, false));
		}
		directArea_ = setupArea(areas_, clearStageNo, bNextStage_);
		bossMenu_.updateEnable(PartManager.ePart.Map, partManager.fade);
		if (bossMenu_.gameObject.activeSelf && !otherData_.isFlag(SaveOtherData.eFlg.FirstBossOpen))
		{
			NetworkMng.Instance.setup(null);
			yield return StartCoroutine(NetworkMng.Instance.download(OnCreateKeyGetWWW, false, false, false, false));
			if (NetworkMng.Instance.getResultCode() == eResultCode.Success)
			{
				KeyBubbleData keyData = GlobalData.Instance.getKeyBubbleData();
				keyData.keyBubbleCount = keyData.keyBubbleMax;
				GlobalData.Instance.setKeyBubbleData(keyData);
				otherData_.setFlag(SaveOtherData.eFlg.FirstBossOpen, true);
				otherData_.setFlag(SaveOtherData.eFlg.RequestBossOpen, true);
				otherData_.setFlag(SaveOtherData.eFlg.RequestFirstBossOpen, true);
			}
			else if (NetworkMng.Instance.getResultCode() == eResultCode.AddedReward)
			{
				otherData_.setFlag(SaveOtherData.eFlg.FirstBossOpen, true);
			}
		}
		if (!otherData_.isFlag(SaveOtherData.eFlg.RequestBossOpen))
		{
			yield return StartCoroutine(bossMenu_.checkGateOpenEffect(otherData_.isFlag(SaveOtherData.eFlg.RequestBossOpen)));
		}
		if (bossMenu_ != null)
		{
			if (otherData_.isFlag(SaveOtherData.eFlg.RequestBossOpen))
			{
				if (otherData_.isFlag(SaveOtherData.eFlg.RequestFirstBossOpen))
				{
					otherData_.setFlag(SaveOtherData.eFlg.RequestFirstBossOpen, false);
					yield return StartCoroutine(playBossTutorial(TutorialDataTable.eSPTutorial.FirstBossOpen));
					bossMenu_.updateKeyLabel();
				}
				else if (!otherData_.isFlag(SaveOtherData.eFlg.FirstBossOpenKeyGet))
				{
					otherData_.setFlag(SaveOtherData.eFlg.FirstBossOpenKeyGet, true);
					yield return StartCoroutine(playBossTutorial(TutorialDataTable.eSPTutorial.BossOpenKey));
				}
				yield return StartCoroutine(bossMenu_.checkGateOpenEffect(otherData_.isFlag(SaveOtherData.eFlg.RequestBossOpen)));
				otherData_.setFlag(SaveOtherData.eFlg.RequestBossOpen, false);
				otherData_.save();
			}
			else if (bossMenu_.gameObject.activeSelf && bossMenu_.isKeyShortage() && !otherData_.isFlag(SaveOtherData.eFlg.BossMenu))
			{
				otherData_.setFlag(SaveOtherData.eFlg.BossMenu, true);
				yield return StartCoroutine(playBossTutorial(TutorialDataTable.eSPTutorial.KeyShortage));
				otherData_.save();
			}
		}
		eventMenu_.updateEnable(PartManager.ePart.Map);
		collaboMenu_.updateEnable(PartManager.ePart.Map);
		isPaymentStageClear = true;
		yield return StartCoroutine(nextStage());
		isPaymentStageClear = false;
		bNextStage_ = false;
		isExecuteNextStage = true;
		bSetupDialogOpen_ = true;
		showPlayerDataEneble = true;
		if (DailyMission.isTermClear())
		{
			Network.DailyMission dMission = GlobalData.Instance.getDailyMissionData();
			if (dMission == null)
			{
				NetworkMng.Instance.setup(Hash.DailyMissionCreate());
				yield return StartCoroutine(NetworkMng.Instance.download(API.DailyMissionCreate, false, false));
				WWW www_dMission = NetworkMng.Instance.getWWW();
				Mission respons_mission = JsonMapper.ToObject<Mission>(www_dMission.text);
				GlobalData.Instance.setDailyMissionData(respons_mission);
				DailyMission.bMissionCreate = true;
			}
		}
		dailyMissionValueSetting();
		if (dailyMission.gameObject.activeSelf)
		{
			yield return StartCoroutine(dailyMission.dailyMissionInfoSetup());
		}
		if (!dailyMission.bonusgamePlayed && DailyMission.bMissionCreate)
		{
			yield return StartCoroutine(showDailyMission());
		}
		int stageNo = getNextSetupStageNo(false);
		while (dialogManager.getActiveDialogNum() >= 1)
		{
			yield return 0;
		}
		if (!otherData_.isFlag(SaveOtherData.eFlg.OpenParkAnnounce) && Bridge.PlayerData.getCurrentStage() >= 10)
		{
			bool isInputOperation = Input.enableCount < 1;
			if (isInputOperation)
			{
				Input.enable = true;
			}
			yield return StartCoroutine(openParkAnnounce());
			if (isInputOperation)
			{
				Input.enable = false;
			}
			GameObject main_2_obj = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI2);
			if ((bool)main_2_obj)
			{
				Transform park_button_trans = main_2_obj.transform.Find("sendOneStage/goToPark/park_Button");
				if ((bool)park_button_trans)
				{
					UIButton park_button = park_button_trans.GetComponent<UIButton>();
					if ((bool)park_button)
					{
						park_button.setEnable(true);
					}
				}
			}
		}
		else if (!bEventStage_ && resultParam_.StageNo != stageNo)
		{
			yield return StartCoroutine(showNextSetup(false));
			yield return StartCoroutine(playTutorial(getNextSetupStageNo(false)));
		}
		yield return 0;
	}

	public IEnumerator Clear(int clearStageNo)
	{
		bStageSkip_ = false;
		Input.enable = false;
		Hashtable h = Hash.StageSkip(clearStageNo + 1);
		NetworkMng.Instance.setup(h);
		yield return StartCoroutine(NetworkMng.Instance.download(API.StageSkip, false, true));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			Input.enable = true;
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		res_ = JsonMapper.ToObject<StageSkipData>(www.text);
		GameData gameData = GlobalData.Instance.getGameData();
		Network.StageData stageData = GlobalData.Instance.getStageData(clearStageNo);
		stageData.star = 0;
		stageData.clearCount = 1;
		gameData.buyJewel = res_.buyJewel;
		gameData.bonusJewel = res_.bonusJewel;
		gameData.level = res_.level;
		gameData.exp = res_.exp;
		gameData.heart = res_.heart;
		gameData.coin = res_.coin;
		gameData.treasureboxNum = res_.treasureboxNum;
		gameData.lastStageNo = res_.lastStageNo;
		gameData.lastStageStatus = res_.lastStageStatus;
		gameData.progressStageNo = res_.progressStageNo;
		gameData.allStarSum = res_.allStarSum;
		gameData.allPlayCount = res_.allPlayCount;
		gameData.allClearCount = res_.allClearCount;
		gameData.allStageScoreSum = res_.allStageScoreSum;
		gameData.eventMaxStageNo = res_.eventMaxStageNo;
		gameData.eventTimeSsRemaining = res_.eventTimeSsRemaining;
		gameData.minilenCount = res_.minilenCount;
		gameData.minilenTotalCount = res_.minilenTotalCount;
		gameData.giveNiceTotalCount = res_.giveNiceTotalCount;
		gameData.giveNiceMonthlyCount = res_.giveNiceMonthlyCount;
		gameData.tookNiceTotalCount = res_.tookNiceTotalCount;
		gameData.isParkDailyReward = res_.isParkDailyReward;
		Input.enable = true;
		Input.enable = false;
		EventMenu.updateGetTime();
		CollaborationMenu.updateGetTime();
		if (stageTbl_ != null)
		{
			ResponceHeaderData headerData = NetworkUtility.createResponceHeaderData(www);
			if (stageTbl_.getUpdateInfo(headerData) != 0)
			{
				yield return StartCoroutine(NetworkUtility.downloadPlayerData(true, false));
				EventMenu.updateGetTime();
				CollaborationMenu.updateGetTime();
				DailyMission.updateGetTime();
				if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
				{
					Input.enable = true;
					bStageSkip_ = true;
					yield break;
				}
				yield return StartCoroutine(stageTbl_.download(headerData, true, false));
				if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
				{
					Input.enable = true;
					bStageSkip_ = true;
					yield break;
				}
			}
		}
		if (eventMenu_ != null)
		{
			eventMenu_.updateEnable(PartManager.ePart.Map);
		}
		if (collaboMenu_ != null)
		{
			collaboMenu_.updateEnable(PartManager.ePart.Map);
		}
		Input.enable = true;
		bStageSkip_ = true;
	}

	private void DisableTapIcon()
	{
		Transform transform = uiRoot.transform.Find("Tap_Panel(Clone)");
		if (transform != null)
		{
			transform.gameObject.SetActive(false);
		}
	}
}
