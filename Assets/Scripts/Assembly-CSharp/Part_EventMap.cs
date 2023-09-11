using System;
using System.Collections;
using System.Collections.Generic;
using Bridge;
using LitJson;
using Network;
using UnityEngine;

public class Part_EventMap : PartBase
{
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

	private enum MapMoveDirection
	{
		Left = 0,
		Right = 1,
		Init = 2
	}

	private const float TWEEN_MAP_MOVE_X = 2600f;

	private MapCamera mapCamera_;

	private Camera uiCamera_;

	private CameraClearFlags clearFlags_ = CameraClearFlags.Nothing;

	private List<GameObject> globalObjList_ = new List<GameObject>();

	private GlobalRoot globalObj_;

	private Transform gachaButton_;

	private GameObject[] listItems_ = new GameObject[9];

	private SaveGameData gameData_;

	private SaveOtherData otherData_;

	private StageIconParent stageIconParent_;

	private MainMenu mainMenu_;

	private StageIcon clearStage_;

	private int mapNo_ = -1;

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

	private Part_Map.ResultParam resultParam_ = new Part_Map.ResultParam();

	private bool bEventStage_;

	private bool bShowedNextSetup_;

	private bool bNextStage_;

	private bool bSetupDialogOpen_;

	private bool bOpenNextKey_;

	private StageDataTable stageTbl_;

	private EventStageInfo eventTbl_;

	private bool bButtonEnable_;

	public DailyMission dailyMission;

	private bool bInitialized;

	private StageIconDataTable iconTbl_;

	private bool isSendOneStageTap;

	private bool isSendMap;

	private int sendStageNo;

	private GameObject sendOneButtonObjct;

	private GameObject sendMap_Event;

	private List<GameObject> sendButtonList = new List<GameObject>();

	private bool bCurrentBack;

	private UISprite chara_00;

	private UISprite chara_01;

	private UISprite chara_02;

	private UISprite chara_03;

	private bool blowcheck;

	public bool transBonus;

	public bool isBonusGamePlayed;

	public static bool bTransMap = true;

	private bool showPlayerDataEneble = true;

	private bool bInactive;

	private StageIcon[] icons;

	public override IEnumerator setup(Hashtable args)
	{
		blowcheck = ResourceLoader.Instance.isUseLowResource();
		bButtonEnable_ = false;
		GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		stageTbl_ = dataTable.GetComponent<StageDataTable>();
		eventTbl_ = stageTbl_.getEventData();
		iconTbl_ = dataTable.GetComponent<StageIconDataTable>();
		globalObj_ = GlobalRoot.Instance;
		gameData_ = SaveData.Instance.getGameData();
		otherData_ = gameData_.getOtherData();
		analyzeHash(args);
		if (resultParam_.IsForceSendInactive)
		{
			yield return StartCoroutine(forceRecover());
		}
		int currentStage = getCurrentEventStage();
		gameData_ = SaveData.Instance.getGameData();
		otherData_ = gameData_.getOtherData();
		if (!globalObj_.isAppend(GlobalObjectParam.eObject.Player))
		{
			globalObj_.load("Prefabs/", GlobalObjectParam.eObject.Player, false);
		}
		if (!globalObj_.isAppend(GlobalObjectParam.eObject.CurrentStageEffect))
		{
			globalObj_.load("Prefabs/", GlobalObjectParam.eObject.CurrentStageEffect, false);
		}
		if (!globalObj_.isAppend(GlobalObjectParam.eObject.GateEffect))
		{
			globalObj_.load("Prefabs/", GlobalObjectParam.eObject.GateEffect, false);
		}
		mainMenu_ = appendGlobalObj(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		mainMenu_.update();
		setupButton(mainMenu_.gameObject, true);
		GameObject menu2 = appendGlobalObj(GlobalObjectParam.eObject.MapMainUI2);
		bagMenu_ = menu2.transform.Find("bag_menu").GetComponent<BagMenu>();
		eventMenu_ = menu2.transform.Find("event").GetComponent<EventMenu>();
		crossMenu_ = menu2.transform.Find("cross").GetComponent<CrossMenu>();
		adMenu_ = menu2.transform.Find("ad").GetComponent<ADMenu>();
		offerwallMenu_ = menu2.transform.Find("Offerwall").GetComponent<OfferwallMenu>();
		sendMap_Event = menu2.transform.Find("sendMap_Event").gameObject;
		sendMap_Event.SetActive(true);
		bossMenu_ = menu2.transform.Find("boss").GetComponent<BossMenu>();
		bossMenu_.gameObject.SetActive(false);
		dailyMission = menu2.transform.Find("daily").GetComponent<DailyMission>();
		gachaButton_ = menu2.transform.Find("gacha");
		chara_00 = menu2.transform.Find("bag_menu/09_avatar/avatar_button/avatar/chara_00").GetComponent<UISprite>();
		chara_01 = menu2.transform.Find("bag_menu/09_avatar/avatar_button/avatar/chara_01").GetComponent<UISprite>();
		chara_02 = menu2.transform.Find("bag_menu/09_avatar/avatar_button/avatar/chara_02").GetComponent<UISprite>();
		chara_03 = menu2.transform.Find("bag_menu/09_avatar/avatar_button/avatar/chara_03").GetComponent<UISprite>();
		UpdateCharaIcon();
		checkAvatarCampaign();
		setMailNum(Bridge.PlayerData.getMailUnReadCount());
		setupButton(menu2.gameObject, true);
		eventMenu_.updateEnable(PartManager.ePart.EventMap);
		crossMenu_.updateEnable(PartManager.ePart.EventMap);
		adMenu_.updateEnable(PartManager.ePart.EventMap);
		offerwallMenu_.updateEnable(PartManager.ePart.EventMap);
		dailyMissionValueSetting();
		yield return StartCoroutine(dailyMission.dailyMissionInfoSetup());
		if (eventTbl_ == null || !eventMenu_.isEventDuration() || eventTbl_.EventNo != 1)
		{
			StartCoroutine(openNoneEventDioalog());
			yield break;
		}
		bCurrentBack = false;
		bNextStage_ = isNextStage(currentStage);
		if (isLastStage(currentStage - 1))
		{
			bNextStage_ = false;
		}
		if (bNextStage_)
		{
			currentStage--;
			bCurrentBack = true;
		}
		else if (isLastStage(currentStage - 1))
		{
			currentStage--;
		}
		if (otherData_.isFlag(SaveOtherData.eFlg.AllClearEvent) && !isLastStage(currentStage))
		{
			currentStage--;
		}
		int mapCurrentStage = ((!isPlayedStage() || args == null) ? currentStage : resultParam_.StageNo);
		if (!bNextStage_ && !isLastStage(mapCurrentStage) && isReclear() && !resultParam_.IsRetry)
		{
			mapCurrentStage = (((mapCurrentStage - 10000) % 59 != 0) ? mapCurrentStage : (mapCurrentStage + 1));
		}
		yield return StartCoroutine(setupWorldMap(mapCurrentStage));
		stageIconParent_ = appendGlobalObj(GlobalObjectParam.eObject.EventStageIcon).GetComponent<StageIconParent>();
		mapCamera_ = appendGlobalObj(GlobalObjectParam.eObject.MapCamera).GetComponentsInChildren<MapCamera>(true)[0];
		GamePlayer player = appendGlobalObj(GlobalObjectParam.eObject.Player).GetComponent<GamePlayer>();
		StartCoroutine(player.loadTexture());
		setActiveGlobalObjs(true);
		GameObject main_2_obj = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI2);
		if ((bool)main_2_obj)
		{
			Transform park_button_trans = main_2_obj.transform.Find("sendOneStage/goToPark/park_Button");
			if ((bool)park_button_trans)
			{
				park_button_trans.gameObject.SetActive(false);
			}
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
		StageIconData sid = iconTbl_.getEventDataByIndex(mapNo_);
		yield return StartCoroutine(loadDialog());
		int stageno = currentStage;
		if (bCurrentBack)
		{
			stageno++;
		}
		int divideCurrentStage3 = ((stageno < sid.IconDatas[sid.IconDatas.Length - 1].StageNo) ? (convEventStageNoToIndex(stageno + 1) % 59) : sid.IconNum);
		divideCurrentStage3 = ((divideCurrentStage3 < 59) ? divideCurrentStage3 : (divideCurrentStage3 - 1));
		divideCurrentStage3 = ((divideCurrentStage3 > 3) ? (divideCurrentStage3 - 3) : 0);
		mapCamera_.EventMoveRangeMax = new Vector2(mapCamera_.DEF_EVENT_RANGE_MAX_W, mapCamera_.DEF_EVENT_RANGE_MAX_H_ONE * (float)sid.IconNum);
		mapCamera_.EventMoveRange = new Vector4(0f, mapCamera_.DEF_EVENT_RANGE_CENTER_H + mapCamera_.DEF_EVENT_RANGE_H_ONE * (float)sid.IconNum / 2f, mapCamera_.DEF_EVENT_RANGE_W, mapCamera_.DEF_EVENT_RANGE_H_ONE * (float)sid.IconNum);
		float par = (float)divideCurrentStage3 / (float)sid.IconNum;
		float min = mapCamera_.EventMoveRange.y - mapCamera_.EventMoveRange.w / 2f;
		float dif = min - mapCamera_.EventMoveRange.y;
		Vector2 range_max = new Vector2(mapCamera_.EventMoveRangeMax.x, mapCamera_.EventMoveRangeMax.y * par);
		Vector4 range = new Vector4(mapCamera_.EventMoveRange.x, min - dif * par, mapCamera_.EventMoveRange.z, mapCamera_.EventMoveRange.w * par + mapCamera_.DEF_EVENT_RANGE_H_ONE * 2f);
		mapCamera_.setMoveRange(range_max, range);
		clearStage_ = setupStageIcon(stageIconParent_, currentStage, bNextStage_);
		areas_ = map_.GetComponentsInChildren<Area>(true);
		directArea_ = setupArea(bNextStage: otherData_.isFlag(SaveOtherData.eFlg.AllClearEvent) || bNextStage_, areas: areas_, currentStage: currentStage);
		if (stageIconParent_.getIcon(currentStage) != null)
		{
			GameObject currentIcon = stageIconParent_.getIcon(currentStage).gameObject;
			showCurrentEffect(currentIcon);
			showPlayer(currentIcon);
		}
		else
		{
			globalObj_.unload(GlobalObjectParam.eObject.CurrentStageEffect);
			player.gameObject.SetActive(false);
		}
		if (stageIconParent_.getIcon(mapCurrentStage) != null)
		{
			focus(mapCurrentStage);
		}
		sendOneButtonObjct = menu2.transform.Find("sendOneStage").gameObject;
		sendOneButtonObjct.GetComponentInChildren<UIButton>().isEnabled = true;
		Transform[] golist = sendMap_Event.GetComponentsInChildren<Transform>(true);
		Transform[] array = golist;
		foreach (Transform sendButton in array)
		{
			if (sendButton.name == "foward" || sendButton.name == "back")
			{
				sendButton.gameObject.SetActive(true);
				sendButtonList.Add(sendButton.gameObject);
			}
		}
		setMapSendButtonEnable(currentStage);
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
		}
		StartCoroutine(updatePanelEnable());
		Transform toastPromotion_ = bagMenu_.transform.Find("toastpromotion_button");
		if (toastPromotion_ != null)
		{
			toastPromotion_.gameObject.SetActive(false);
		}
		yield return StartCoroutine(showNextSetup(true));
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
		if (partManager.prevPart == PartManager.ePart.Map)
		{
			yield return StartCoroutine(playOpenCloudEff());
		}
		partManager.bTransitionMap_ = false;
		showPlayerDataEneble = false;
		Sound.Instance.playBgm(Sound.eBgm.BGM_010_Map, true);
		DialogEventSetup eventSetup = dialogManager.getDialog(DialogManager.eDialog.EventSetup) as DialogEventSetup;
		eventSetup.setEventMenu(eventMenu_);
		if (eventSetup.isOpen())
		{
		}
		while (dialogManager.getActiveDialogNum() >= 1)
		{
			yield return 0;
		}
		DialogDailyMissionClear mc = dialogManager.getDialog(DialogManager.eDialog.DailyMissionClear) as DialogDailyMissionClear;
		if (!mc.toBonus)
		{
			yield return StartCoroutine(nextStage());
			bOpenNextKey_ = isOpenStageKey(getCurrentEventStage());
			yield return StartCoroutine(nextKeyOpen());
			if (!dailyMission.bonusgamePlayed && dailyMission.missionCleared)
			{
				yield return StartCoroutine(showDailyMissionCleared());
			}
			if (!transBonus)
			{
				yield return StartCoroutine(showNextSetup(false));
				bButtonEnable_ = true;
				bInitialized = true;
				showPlayerDataEneble = true;
			}
		}
	}

	private IEnumerator playTutorial(int stageNo)
	{
		TutorialManager.Instance.bItemTutorial = false;
		if (TutorialManager.Instance.isTutorial(stageNo, TutorialDataTable.ePlace.Setup))
		{
			GameObject uiRoot = dialogManager.getCurrentUiRoot();
			TutorialManager.Instance.load(stageNo, uiRoot);
			GameObject dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
			StageInfo.CommonInfo stageInfo = dataTbl.GetComponent<StageDataTable>().getInfo(0).Common;
			yield return StartCoroutine(TutorialManager.Instance.play(stageNo, TutorialDataTable.ePlace.Setup, uiRoot, stageInfo, null));
		}
	}

	private IEnumerator nextStage()
	{
		if (eventTbl_ == null)
		{
			yield break;
		}
		int currentStage = getCurrentEventStage();
		bool bAllClear = otherData_.isFlag(SaveOtherData.eFlg.AllClearEvent);
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
			yield return StartCoroutine(clearStage_.setup(bAnimation, false));
			if (isLastStage(currentStage - 1))
			{
				Input.enable = true;
				yield return StartCoroutine(openToBeContinueDialog());
				otherData_.setFlag(SaveOtherData.eFlg.AllClearEvent, true);
				Input.enable = false;
			}
			else
			{
				if (directArea_ != null)
				{
					EventStageInfo.Info info2 = eventTbl_.Infos[convEventStageNoToIndex(currentStage)];
					yield return StartCoroutine(scrollMap(currentStage));
					yield return StartCoroutine(directArea_.openDirect(Area.eMap.Event, EventMenu.isTermsStageClear(info2)));
					if (EventMenu.isTermsStageClear(info2))
					{
						otherData_.setEventStageKeyNo(currentStage);
					}
				}
				yield return StartCoroutine(moveNextStage(currentStage));
			}
			otherData_.setEventStageNo(currentStage);
		}
		else if (bAllClear && !isLastStage(currentStage - 1))
		{
			if (directArea_ != null)
			{
				EventStageInfo.Info info = eventTbl_.Infos[convEventStageNoToIndex(currentStage)];
				yield return StartCoroutine(scrollMap(currentStage));
				yield return StartCoroutine(directArea_.openDirect(Area.eMap.Event, EventMenu.isTermsStageClear(info)));
				if (EventMenu.isTermsStageClear(info))
				{
					otherData_.setEventStageKeyNo(currentStage);
				}
			}
			yield return StartCoroutine(moveNextStage(currentStage));
			otherData_.setFlag(SaveOtherData.eFlg.AllClearEvent, false);
		}
		else if (!bEventStage_ && resultParam_.IsClear && resultParam_.StageNo == currentStage && !resultParam_.IsProgressOpen)
		{
			Input.enable = true;
			yield return StartCoroutine(openAreaLockDialog());
			Input.enable = false;
		}
		else if (isLastStage(resultParam_.StageNo) && resultParam_.IsClear)
		{
			Input.enable = true;
			yield return StartCoroutine(openToBeContinueDialog());
			Input.enable = false;
		}
		otherData_.save();
		Input.enable = true;
	}

	private IEnumerator nextKeyOpen()
	{
		if (eventTbl_ == null)
		{
			yield break;
		}
		int currentStage = getCurrentEventStage();
		bool bAllClear = otherData_.isFlag(SaveOtherData.eFlg.AllClearEvent);
		Input.enable = false;
		if (bOpenNextKey_)
		{
			if (isLastStage(currentStage - 1))
			{
				EventStageInfo.Info info3 = eventTbl_.Infos[convEventStageNoToIndex(currentStage - 1)];
				if (EventMenu.isTermsStageClear(info3) && otherData_.getEventStageKeyNo() < currentStage - 1)
				{
					GameObject effect3 = UnityEngine.Object.Instantiate(globalObj_.getObject(GlobalObjectParam.eObject.GateEffect)) as GameObject;
					Area area_3 = null;
					Area[] array = areas_;
					foreach (Area area3 in array)
					{
						if (convEventStageNoToIndex(currentStage - 1) == area3.getAreaNo())
						{
							area_3 = area3;
							break;
						}
					}
					area_3.setEffect(effect3);
					Input.enable = false;
					yield return StartCoroutine(area_3.openKeyDirect());
					while (effect3.GetComponent<Animation>().isPlaying)
					{
						yield return null;
					}
					UnityEngine.Object.Destroy(effect3.gameObject);
					Input.enable = true;
				}
			}
			else
			{
				EventStageInfo.Info info2 = eventTbl_.Infos[convEventStageNoToIndex(currentStage)];
				if (EventMenu.isTermsStageClear(info2) && otherData_.getEventStageKeyNo() < currentStage)
				{
					GameObject effect2 = UnityEngine.Object.Instantiate(globalObj_.getObject(GlobalObjectParam.eObject.GateEffect)) as GameObject;
					Area area_2 = null;
					Area[] array2 = areas_;
					foreach (Area area2 in array2)
					{
						if (convEventStageNoToIndex(currentStage) == area2.getAreaNo())
						{
							area_2 = area2;
							break;
						}
					}
					area_2.setEffect(effect2);
					Input.enable = false;
					yield return StartCoroutine(area_2.openKeyDirect());
					while (effect2.GetComponent<Animation>().isPlaying)
					{
						yield return null;
					}
					UnityEngine.Object.Destroy(effect2.gameObject);
					Input.enable = true;
				}
			}
			otherData_.setEventStageKeyNo(currentStage);
		}
		else if (bAllClear && !isLastStage(currentStage - 1))
		{
			if (directArea_ != null)
			{
				EventStageInfo.Info info = eventTbl_.Infos[convEventStageNoToIndex(currentStage)];
				if (EventMenu.isTermsStageClear(info) && otherData_.getEventStageKeyNo() < currentStage)
				{
					GameObject effect = UnityEngine.Object.Instantiate(globalObj_.getObject(GlobalObjectParam.eObject.GateEffect)) as GameObject;
					Area area_ = null;
					Area[] array3 = areas_;
					foreach (Area area in array3)
					{
						if (convEventStageNoToIndex(currentStage) == area.getAreaNo())
						{
							area_ = area;
							break;
						}
					}
					area_.setEffect(effect);
					Input.enable = false;
					yield return StartCoroutine(area_.openKeyDirect());
					while (effect.GetComponent<Animation>().isPlaying)
					{
						yield return null;
					}
					UnityEngine.Object.Destroy(effect.gameObject);
					Input.enable = true;
				}
			}
			otherData_.setFlag(SaveOtherData.eFlg.AllClearEvent, false);
			otherData_.setEventStageKeyNo(currentStage);
		}
		otherData_.save();
		Input.enable = true;
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

	private IEnumerator showDailyMission()
	{
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
		Sound.Instance.playSe(Sound.eSe.SE_108_Yay);
		Sound.Instance.playSe(Sound.eSe.SE_113_Clap);
		DialogDailyMissionClear dialog = dialogManager.getDialog(DialogManager.eDialog.DailyMissionClear) as DialogDailyMissionClear;
		dialog.setup(this, DialogDailyMissionClear.eTargetPart.EventMap);
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

	private void analyzeHash(Hashtable args)
	{
		if (args != null && isPlayedStage())
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
				StartCoroutine(icon.setup(false, true));
			}
			setupButton(icon.gameObject, true);
			if (currentStage < icon.getStageNo())
			{
				icon.gameObject.SetActive(false);
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
		if (stageNo % 10000 != 1 && !otherData_.isPlayedEventStageProd(stageNo))
		{
			return true;
		}
		return false;
	}

	private bool isOpenStageKey(int stageNo)
	{
		if (eventTbl_ == null)
		{
			return false;
		}
		int num = convEventStageNoToIndex(stageNo);
		if (num >= getEventStageNum())
		{
			num = getEventStageNum() - 1;
		}
		if (stageNo % 10000 != 1 && EventMenu.isTermsStageClear(eventTbl_.Infos[num]) && !otherData_.isPlayedEventStageKeyProd(stageNo))
		{
			Debug.Log("return true");
			return true;
		}
		return false;
	}

	private Area setupArea(Area[] areas, int currentStage, bool bNextStage)
	{
		Area area = null;
		int currentEventArea = getCurrentEventArea(currentStage);
		int num = currentEventArea;
		if (bNextStage && !isLastStage(currentStage))
		{
			num = currentEventArea + 1;
		}
		foreach (Area area2 in areas)
		{
			if (num != currentEventArea && area2.getAreaNo() == num)
			{
				area = area2;
				area.gameObject.SetActive(false);
			}
			else if (area2.getAreaNo() <= currentEventArea)
			{
				if (area2.getAreaNo() != num || otherData_.isPlayedEventStageKeyProd(currentStage))
				{
					area2.open();
					StageIconData eventDataByIndex = iconTbl_.getEventDataByIndex(mapNo_);
					int stageNo = eventDataByIndex.IconDatas[eventDataByIndex.IconNum - 1].StageNo;
					int num2 = currentStage;
					if (bCurrentBack)
					{
						num2++;
					}
					if (!bNextStage && num2 > stageNo && area2.root_2 != null)
					{
						area2.root_2.SetActive(true);
					}
				}
			}
			else
			{
				area2.gameObject.SetActive(false);
			}
		}
		return area;
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
			Utility.setParent(component.gameObject, map_.transform, false);
			component.setup(currentIcon.transform.localPosition, this, GamePlayer.eTargetPart.EventMap);
			component.gameObject.SetActive(true);
		}
	}

	private IEnumerator transMap(GameObject caller, int mapNo, MapMoveDirection direction, bool isAreaMove)
	{
		Debug.Log("transMap:" + mapNo);
		Input.enable = false;
		bTransMap = false;
		yield return StartCoroutine(playCloseCloudEff());
		GameObject loading = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.Loading);
		loading.SetActive(true);
		loading.transform.Find("bg").gameObject.SetActive(false);
		OnChangeMap();
		UnityEngine.Object.Destroy(map_);
		int currentStage = getCurrentEventStage();
		yield return StartCoroutine(setupWorldMap(currentStage, mapNo));
		GameObject gateEffect = loadGlobalObj("Prefabs/", GlobalObjectParam.eObject.GateEffect);
		GameObject effectLoad = loadGlobalObj("Prefabs/", GlobalObjectParam.eObject.CurrentStageEffect);
		Utility.setParent(effectLoad, map_.transform, false);
		GameObject playerLoad = loadGlobalObj("Prefabs/", GlobalObjectParam.eObject.Player);
		Utility.setParent(playerLoad, map_.transform, false);
		setMapSendButtonEnable(currentStage);
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
		StageIconData sid = iconTbl_.getEventDataByIndex(mapNo_);
		int divideCurrentStage = ((currentStage <= sid.IconDatas[sid.IconDatas.Length - 1].StageNo) ? (convEventStageNoToIndex(currentStage) % 59) : sid.IconNum);
		divideCurrentStage = ((divideCurrentStage < 59) ? divideCurrentStage : (divideCurrentStage - 1));
		divideCurrentStage = ((divideCurrentStage > 3) ? (divideCurrentStage - 3) : 0);
		mapCamera_.EventMoveRangeMax = new Vector2(mapCamera_.DEF_EVENT_RANGE_MAX_W, mapCamera_.DEF_EVENT_RANGE_MAX_H_ONE * (float)sid.IconNum);
		mapCamera_.EventMoveRange = new Vector4(0f, mapCamera_.DEF_EVENT_RANGE_CENTER_H + mapCamera_.DEF_EVENT_RANGE_H_ONE * (float)sid.IconNum / 2f, mapCamera_.DEF_EVENT_RANGE_W, mapCamera_.DEF_EVENT_RANGE_H_ONE * (float)sid.IconNum);
		float par = (float)divideCurrentStage / (float)sid.IconNum;
		float min = mapCamera_.EventMoveRange.y - mapCamera_.EventMoveRange.w / 2f;
		float dif = min - mapCamera_.EventMoveRange.y;
		Vector2 range_max = new Vector2(mapCamera_.EventMoveRangeMax.x, mapCamera_.EventMoveRangeMax.y * par);
		Vector4 range = new Vector4(mapCamera_.EventMoveRange.x, min - dif * par, mapCamera_.EventMoveRange.z, mapCamera_.EventMoveRange.w * par + mapCamera_.DEF_EVENT_RANGE_H_ONE * 2f);
		mapCamera_.setMoveRange(range_max, range);
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
			focus(currentStage);
		}
		else
		{
			globalObj_.unload(GlobalObjectParam.eObject.CurrentStageEffect);
			player.gameObject.SetActive(false);
			focus((mapNo_ + 1) * 59 + 10000);
		}
		setActiveGlobalObjs(true);
		yield return Resources.UnloadUnusedAssets();
		loading.SetActive(false);
		if (isAreaMove)
		{
			player.transform.localPosition += Vector3.left * 350f;
		}
		yield return StartCoroutine(playOpenCloudEff());
		bTransMap = true;
		Input.enable = true;
	}

	private void OnChangeMap()
	{
		globalObj_.unload(GlobalObjectParam.eObject.GateEffect);
		globalObj_.unload(GlobalObjectParam.eObject.CurrentStageEffect);
		globalObj_.unload(GlobalObjectParam.eObject.Player);
		stageIconParent_.transform.parent = map_.transform.parent;
	}

	private GameObject loadGlobalObj(string path, GlobalObjectParam.eObject objType)
	{
		GameObject gameObject = globalObj_.load(path, objType, false);
		gameObject.SetActive(false);
		return gameObject;
	}

	public void SendMap(int focusNo)
	{
		isSendMap = true;
		sendStageNo = focusNo;
		StartCoroutine(transMap());
	}

	private IEnumerator transMap()
	{
		Input.enable = false;
		partManager.bTransitionMap_ = true;
		if (bInactive)
		{
			StopCoroutine("sendInactive");
			bInactive = false;
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		}
		yield return StartCoroutine(playCloseCloudEff());
		foreach (GameObject message in sendButtonList)
		{
			message.SetActive(true);
		}
		sendMap_Event.SetActive(false);
		Hashtable args = new Hashtable { { "IsForceSendInactive", true } };
		if (isSendOneStageTap)
		{
			args.Add("IsSendOneStage", true);
		}
		if (isSendMap)
		{
			args.Add("IsSendMap", true);
			args.Add("SendStageNo", sendStageNo);
		}
		Input.enable = true;
		partManager.requestTransition(PartManager.ePart.Map, args, FadeMng.eType.MapChange, true);
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
		while (anim.isPlaying)
		{
			yield return 0;
		}
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		if (!bButtonEnable_)
		{
			yield break;
		}
		DialogAppQuit da = (DialogAppQuit)dialogManager.getDialog(DialogManager.eDialog.AppQuit);
		if (da.isOpen())
		{
			yield break;
		}
		string name = trigger.name;
		if (name.Contains("Stage_button"))
		{
			Constant.SoundUtil.PlayButtonSE();
			StageIcon icon = trigger.transform.parent.GetComponent<StageIcon>();
			if (stageAliveCheck(icon))
			{
				DialogEventSetup setupDialog = dialogManager.getDialog(DialogManager.eDialog.EventSetup) as DialogEventSetup;
				yield return StartCoroutine(setupDialog.show(icon, this));
				yield break;
			}
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			Input.enable = true;
			yield return dialogManager.StartCoroutine(openNoneEventDioalog());
			yield return StartCoroutine(transMap());
			yield break;
		}
		if (trigger.transform.parent.name == "gacya_button")
		{
			Constant.SoundUtil.PlayButtonSE();
			DialogAvatarGacha gachaDialog = dialogManager.getDialog(DialogManager.eDialog.AvatarGacha) as DialogAvatarGacha;
			bool isFree2 = false;
			Hashtable h = Hash.GachaTop();
			NetworkMng.Instance.setup(h);
			yield return StartCoroutine(NetworkMng.Instance.download(API.GachaTop, true, false));
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
				gachaDialog.resetGachaNo();
				gachaDialog.setup(isFree2);
				yield return StartCoroutine(dialogManager.openDialog(gachaDialog));
			}
			yield break;
		}
		if (name == "PlusButton")
		{
			Constant.SoundUtil.PlayButtonSE();
			yield return StartCoroutine(plusButton(trigger));
			yield break;
		}
		switch (name)
		{
		case "BackButton":
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(transMap());
			break;
		case "back":
			if (bTransMap)
			{
				mapNo_--;
				if (mapNo_ < 0)
				{
					mapNo_ = 0;
				}
				else
				{
					StartCoroutine(transMap(trigger.transform.parent.parent.gameObject, mapNo_, MapMoveDirection.Left, false));
				}
			}
			break;
		case "foward":
			if (bTransMap)
			{
				mapNo_++;
				if (mapNo_ >= iconTbl_.getMaxMapNum())
				{
					mapNo_ = iconTbl_.getMaxMapNum() - 1;
				}
				else
				{
					StartCoroutine(transMap(trigger.transform.parent.parent.gameObject, mapNo_, MapMoveDirection.Right, false));
				}
			}
			break;
		case "sendOne_button":
		{
			Constant.SoundUtil.PlayButtonSE();
			DialogBase baseDialog = dialogManager.getDialog(DialogManager.eDialog.SendMap);
			if (baseDialog != null)
			{
				DialogSendMap dialog = baseDialog as DialogSendMap;
				dialog.setup();
				dialogManager.StartCoroutine(dialogManager.openDialog(dialog));
			}
			break;
		}
		case "ranking_button":
		{
			Constant.SoundUtil.PlayDecideSE();
			DialogDayRanking dialog2 = dialogManager.getDialog(DialogManager.eDialog.DayRanking) as DialogDayRanking;
			dialog2.setup();
			yield return StartCoroutine(dialog2.loadRanking(false));
			if (dialog2.bShow_)
			{
				dailyMissionValueSetting();
				yield return StartCoroutine(dailyMission.dailyMissionInfoSetup());
				StartCoroutine(dailyMissionClearCheck());
				yield return StartCoroutine(dialogManager.openDialog(dialog2));
				while (dialog2.bShow_)
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
			Mail[] array = mails2;
			foreach (Mail mail in array)
			{
				if (!mail.isOpen)
				{
					mailCount++;
				}
			}
			setMailNum(mailCount);
			if (mailNum_ > 0)
			{
				DialogMail dialog4 = dialogManager.getDialog(DialogManager.eDialog.Mail) as DialogMail;
				yield return StartCoroutine(dialog4.show(mails2));
				while (dialog4.isOpen())
				{
					yield return null;
				}
				GlobalData.Instance.getGameData().mailUnReadCount = dialog4.mailNum_;
				setMailNum(dialog4.mailNum_);
			}
			else
			{
				setMailNum(0);
				DialogConfirm dialog3 = dialogManager.getDialog(DialogManager.eDialog.NoMail) as DialogConfirm;
				yield return StartCoroutine(dialogManager.openDialog(dialog3));
			}
			break;
		}
		case "invite_button":
		{
			Constant.SoundUtil.PlayDecideSE();
			DialogInvite dialog5 = dialogManager.getDialog(DialogManager.eDialog.Invite) as DialogInvite;
			yield return StartCoroutine(dialog5.show());
			break;
		}
		case "avatar_button":
		{
			Constant.SoundUtil.PlayDecideSE();
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
			DialogOption dialog6 = dialogManager.getDialog(DialogManager.eDialog.Option) as DialogOption;
			dialog6.setup();
			yield return StartCoroutine(dialogManager.openDialog(dialog6));
			break;
		}
		case "Exp_Button":
			Constant.SoundUtil.PlayButtonSE();
			mainMenu_.getExpMenu().changeText();
			break;
		case "popup_root":
			if (!dailyMission.bonusgamePlayed)
			{
				Constant.SoundUtil.PlayButtonSE();
				StartCoroutine(showDailyMission());
			}
			break;
		}
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
		if (!bTransMap)
		{
			mapCamera_.focus(stageIconParent_.getIcon(stageNo).transform.localPosition);
		}
		else if (!isPlayedStage())
		{
			mapCamera_.focus(stageIconParent_.getIcon(stageNo).transform.localPosition);
		}
		else if (!bEventStage_)
		{
			if (resultParam_.IsRetry)
			{
				mapCamera_.focus(stageIconParent_.getIcon(resultParam_.StageNo).transform.localPosition);
			}
			else if (!bNextStage_ && !isLastStage(resultParam_.StageNo) && isReclear())
			{
				mapCamera_.focus(stageIconParent_.getIcon(getNextStageNo(resultParam_.StageNo)).transform.localPosition);
			}
			else
			{
				mapCamera_.focus(stageIconParent_.getIcon(resultParam_.StageNo).transform.localPosition);
			}
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
			int currentEventStage = getCurrentEventStage();
			if (isLastStage(currentEventStage - 1))
			{
				return -1;
			}
			return currentEventStage;
		}
		if (!resultParam_.IsClear)
		{
			return -1;
		}
		if (resultParam_.IsResultClose)
		{
			return -1;
		}
		return (!bEventStage_) ? getNextStageNo(resultParam_.StageNo) : getNextEventStageNo(resultParam_.StageNo, eventTbl_.EventNo);
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
		return true;
	}

	private IEnumerator showSetup(int stageNo)
	{
		DialogEventSetup setupDialog = dialogManager.getDialog(DialogManager.eDialog.EventSetup) as DialogEventSetup;
		yield return StartCoroutine(setupDialog.show(stageIconParent_.getIcon(stageNo), this));
		bButtonEnable_ = true;
	}

	private IEnumerator moveNextStage(int stage)
	{
		if (stageIconParent_.getIcon(stage) == null)
		{
			Area area59 = areas_[areas_.Length - 1];
			if (area59 != null)
			{
				yield return StartCoroutine(area59.showRoot2());
				while (area59.root_2.GetComponentInChildren<iTween>() != null)
				{
					yield return 0;
				}
			}
			GamePlayer player2 = appendGlobalObj(GlobalObjectParam.eObject.Player).GetComponent<GamePlayer>();
			StartCoroutine(player2.move(player2.transform.localPosition + Vector3.right * 350f));
			while (player2.GetComponent<iTween>() != null)
			{
				yield return 0;
			}
			mapNo_++;
			yield return StartCoroutine(transMap(base.gameObject, mapNo_, MapMoveDirection.Right, true));
			GameObject currentIcon2 = stageIconParent_.getIcon(stage).gameObject;
			player2 = globalObj_.getObject(GlobalObjectParam.eObject.Player).GetComponent<GamePlayer>();
			player2.transform.localPosition += Vector3.left * 350f;
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
		}
		else
		{
			GameObject currentIcon = stageIconParent_.getIcon(stage).gameObject;
			GamePlayer player = appendGlobalObj(GlobalObjectParam.eObject.Player).GetComponent<GamePlayer>();
			StartCoroutine(player.move(currentIcon.transform.localPosition));
			while (player.GetComponent<iTween>() != null)
			{
				yield return 0;
			}
			StageIcon icon = currentIcon.GetComponent<StageIcon>();
			currentIcon.SetActive(true);
			icon.enable();
			showCurrentEffect(currentIcon);
			yield return StartCoroutine(icon.playOpenProduct());
		}
	}

	private IEnumerator openToBeContinueDialog()
	{
		Sound.Instance.playBgm(Sound.eBgm.BGM_000_Title, true);
		DialogBase dialog = dialogManager.getDialog(DialogManager.eDialog.EventContinued);
		yield return StartCoroutine(dialogManager.openDialog(dialog));
		while (dialog.isOpen())
		{
			yield return 0;
		}
		Sound.Instance.playBgm(Sound.eBgm.BGM_010_Map, true);
	}

	private IEnumerator openAreaLockDialog()
	{
		Sound.Instance.playBgm(Sound.eBgm.BGM_000_Title, true);
		DialogCommon dialog = dialogManager.getDialog(DialogManager.eDialog.AreaLock) as DialogCommon;
		dialog.setup(null, null, true);
		yield return StartCoroutine(dialog.open());
		dialogManager.addActiveDialogList(DialogManager.eDialog.AreaLock);
		while (dialog.isOpen())
		{
			yield return 0;
		}
		Sound.Instance.playBgm(Sound.eBgm.BGM_010_Map, true);
	}

	private IEnumerator scrollMap(int stage)
	{
		GameObject currentIcon = null;
		Debug.Log("stageIconParent_ = " + stageIconParent_);
		Debug.Log(" stageIconParent_.getIcon(stage) = " + stageIconParent_.getIcon(stage));
		if (stageIconParent_ != null && stageIconParent_.getIcon(stage) != null)
		{
			currentIcon = stageIconParent_.getIcon(stage).gameObject;
		}
		if (currentIcon != null)
		{
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
		if (eventTbl_ == null)
		{
			return 10010;
		}
		return getEventStageNum() + eventTbl_.EventNo * 10000;
	}

	private bool isLastStage(int stageNo)
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

	private bool isPlayedStage()
	{
		PartManager.ePart prevPart = partManager.prevPart;
		return prevPart == PartManager.ePart.Stage || prevPart == PartManager.ePart.Scenario;
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
		yield return StartCoroutine(dialogManager.load(PartManager.ePart.EventMap));
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
			return;
		}
		StopCoroutine("sendInactive");
		if (bInactive)
		{
			StopCoroutine("updateFriends");
			partManager.loginInputBack();
		}
		FriendUpdater.Instance.stop();
		bInactive = false;
	}

	private void OnApplicationQuit()
	{
	}

	private IEnumerator sendInactive()
	{
		while (isBonusGamePlayed || GlobalData.Instance.isResourceDownloading)
		{
			yield return 0;
		}
		DialogJewelShop JewelShop = dialogManager.getDialog(DialogManager.eDialog.JewelShop) as DialogJewelShop;
		DialogLuckyChance LuckyChance = dialogManager.getDialog(DialogManager.eDialog.LuckyChance) as DialogLuckyChance;
		DialogCoinShop CoinShop = dialogManager.getDialog(DialogManager.eDialog.CoinShop) as DialogCoinShop;
		DialogHeartShop HeartShop = dialogManager.getDialog(DialogManager.eDialog.HeartShop) as DialogHeartShop;
		DialogGiftJewelShop GiftShop = dialogManager.getDialog(DialogManager.eDialog.GiftJewelShop) as DialogGiftJewelShop;
		DialogAllShop allShop = dialogManager.getDialog(DialogManager.eDialog.AllShop) as DialogAllShop;
		while (partManager.isLineLogin())
		{
			yield return 0;
		}
		if ((JewelShop != null && JewelShop.isOpen()) || (CoinShop != null && CoinShop.isOpen()) || (HeartShop != null && HeartShop.isOpen()) || (LuckyChance != null && LuckyChance.isBuying()) || (GiftShop != null && GiftShop.isOpen()) || (allShop != null && allShop.isOpen()))
		{
			while (NetworkMng.Instance.isDownloading())
			{
				yield return 0;
			}
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			partManager.loginInputBack();
			bInactive = false;
			yield break;
		}
		bInactive = true;
		if (!bInitialized)
		{
			while (NetworkMng.Instance.isDownloading())
			{
				yield return 0;
			}
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			if (bInactive)
			{
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
				partManager.loginInputBack();
				bInactive = false;
			}
			yield break;
		}
		yield return FriendUpdater.Instance.getFriends();
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
		GlobalData.Instance.getGameData().setParkData(null, null, data.thanksList, data.buildings, data.mapReleaseNum);
		SaveData.Instance.getParkData().UpdatePlacedData(data.buildings);
		EventMenu.updateGetTime();
		GlobalData.Instance.getGameData().gachaTicket = data.gachaTicket;
		Debug.Log("gachaTicket = " + data.gachaTicket);
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
				dailyMission.dailyMissionInfoSetup();
			}
		}
		else
		{
			dailyMission.gameObject.SetActive(false);
		}
		MemberData[] playerDatas = JsonMapper.ToObject<FriendData>(www.text).memberDataList;
		FriendUpdater.Instance.setProgress(playerDatas);
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
		DialogEventSetup setup = dialogManager.getDialog(DialogManager.eDialog.EventSetup) as DialogEventSetup;
		if (setup != null && setup.isOpen())
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
				eventMenu_.updateEnable(PartManager.ePart.EventMap);
			}
			if (crossMenu_ != null)
			{
				crossMenu_.updateEnable(PartManager.ePart.EventMap);
			}
			if (adMenu_ != null)
			{
				adMenu_.updateEnable(PartManager.ePart.EventMap);
			}
			if (offerwallMenu_ != null)
			{
				offerwallMenu_.updateEnable(PartManager.ePart.EventMap);
			}
			partManager.loginInputBack();
			bInactive = false;
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
		TutorialManager.Instance.unload();
		globalObj_.unload(GlobalObjectParam.eObject.EventStageIcon);
		globalObj_.unload(GlobalObjectParam.eObject.EventMap);
		globalObj_.unload(GlobalObjectParam.eObject.Player);
		globalObj_.unload(GlobalObjectParam.eObject.CurrentStageEffect);
		globalObj_.unload(GlobalObjectParam.eObject.GateEffect);
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
		for (int i = 0; i < DummyPlayFriendData.FriendNum; i++)
		{
			UserData data = DummyPlayFriendData.DummyFriends[indexArray[i]];
			int stageNo2 = data.StageNo;
			if (!((float)getOverlapCount(stageNoList, stageNo2) >= Constant.FriendPlaceOverlapCount))
			{
				if (stageNo2 > getLastStageNo() + 1)
				{
					stageNo2 = getLastStageNo() + 1;
				}
				yield return 0;
			}
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

	private IEnumerator loadStageIcons(GameObject worldMap, StageIconData data)
	{
		GameObject stageIconRoot;
		if (stageIconParent_ == null)
		{
			stageIconRoot = new GameObject(GlobalObjectParam.getName(GlobalObjectParam.eObject.EventStageIcon));
			globalObj_.appendObject(stageIconRoot, GlobalObjectParam.eObject.EventStageIcon, false);
			Utility.setParent(stageIconRoot, worldMap.transform, false);
			icons = new StageIcon[59];
			GameObject objBase = ResourceLoader.Instance.loadGameObject("Prefabs/", "stageicon");
			for (int i = 0; i < 59; i++)
			{
				GameObject stageIcon = UnityEngine.Object.Instantiate(objBase) as GameObject;
				if (i < data.IconNum)
				{
					IconData iconData2 = data.IconDatas[i];
					icons[i] = stageIcon.GetComponent<StageIcon>();
					Utility.setParent(stageIcon, stageIconRoot.transform, false);
					icons[i].setStageNo(iconData2.StageNo);
					Vector3 pos2 = iconData2.Pos;
					stageIcon.transform.localPosition = new Vector3(pos2.x, pos2.y, stageIcon.transform.localPosition.z);
					stageIcon.SetActive(true);
					yield return 0;
				}
				else
				{
					icons[i] = stageIcon.GetComponent<StageIcon>();
					Utility.setParent(stageIcon, stageIconRoot.transform, false);
					icons[i].setStageNo(-1);
					stageIcon.SetActive(false);
				}
			}
			stageIconRoot.AddComponent<StageIconParent>().setup(icons);
			stageIconRoot.SetActive(false);
			yield break;
		}
		stageIconRoot = stageIconParent_.gameObject;
		Utility.setParent(stageIconRoot, worldMap.transform, false);
		stageIconRoot.transform.localPosition = Vector3.zero;
		stageIconRoot.transform.localScale = Vector3.one;
		for (int j = 0; j < 59; j++)
		{
			icons[j].gameObject.SetActive(true);
			if (j < data.IconNum)
			{
				IconData iconData = data.IconDatas[j];
				icons[j].setStageNo(iconData.StageNo);
				Vector3 pos = iconData.Pos;
				icons[j].gameObject.transform.localPosition = new Vector3(pos.x, pos.y, icons[j].gameObject.transform.localPosition.z);
			}
			else
			{
				icons[j].setStageNo(-1);
				icons[j].gameObject.SetActive(false);
			}
			if (j % 10 == 0)
			{
				yield return 0;
			}
		}
		stageIconParent_.setup(icons);
	}

	private IEnumerator setupWorldMap(int currentStage)
	{
		int mapno = iconTbl_.getEventMapNoByStageNo(currentStage);
		yield return StartCoroutine(setupWorldMap(currentStage, mapno));
	}

	private IEnumerator setupWorldMap(int currentStage, int mapno)
	{
		mapNo_ = mapno;
		map_ = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "BG_Panel_Event_" + (mapNo_ + 1).ToString("00"))) as GameObject;
		Utility.setParent(map_, uiRoot.transform, false);
		StageIconDataTable iconData = globalObj_.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageIconDataTable>();
		iconData.load();
		yield return StartCoroutine(loadStageIcons(map_, iconData.getEventDataByIndex(mapNo_)));
	}

	private int getCurrentEventArea(int current_stage)
	{
		return current_stage % 10000 - 1;
	}

	private int getCurrentEventStage()
	{
		if (eventTbl_ == null)
		{
			return 10001;
		}
		int num = eventTbl_.EventNo * 10000 + 1;
		for (int i = 0; i < getEventStageNum(); i++)
		{
			EventStageInfo.Info info = eventTbl_.Infos[i];
			if (EventMenu.isPrevLevelClear(info))
			{
				num = info.Common.StageNo;
				continue;
			}
			break;
		}
		if (Bridge.StageData.isClear(num))
		{
			num++;
		}
		Debug.Log("currentstage:" + num);
		return num;
	}

	private void setMapSendButtonEnable(int currentStage)
	{
		foreach (GameObject sendButton in sendButtonList)
		{
			sendButton.gameObject.SetActive(true);
			sendButton.GetComponent<UIButtonMessage>().target = base.gameObject;
			if (mapNo_ <= 0 && sendButton.name == "back")
			{
				sendButton.SetActive(false);
				continue;
			}
			if (mapNo_ + 1 >= iconTbl_.getMaxEventMapNum() && sendButton.name == "foward")
			{
				sendButton.SetActive(false);
				continue;
			}
			int num = currentStage % 10000;
			int num2 = (mapNo_ + 1) * 59;
			if (num <= num2 && sendButton.name == "foward")
			{
				sendButton.SetActive(false);
			}
		}
	}

	private int getEventStageNum()
	{
		if (GlobalData.Instance.getGameData() == null)
		{
			return 10;
		}
		int eventMaxStageNo = GlobalData.Instance.getGameData().eventMaxStageNo;
		if (iconTbl_ != null && iconTbl_.getMaxEventIconsNum() + 10000 < eventMaxStageNo)
		{
			return iconTbl_.getMaxEventIconsNum();
		}
		return eventMaxStageNo % 10000;
	}

	private int convEventStageNoToIndex(int stageNo)
	{
		return stageNo % 10000 - 1;
	}

	private IEnumerator openNoneEventDioalog()
	{
		Input.enable = true;
		partManager.bTransitionMap_ = true;
		yield return StartCoroutine(openCommonDialog(58));
		Hashtable args = new Hashtable { { "IsForceSendInactive", true } };
		partManager.requestTransition(PartManager.ePart.Map, args, FadeMng.eType.MapChange, true);
		Input.enable = true;
	}

	public void UpdateCharaIcon()
	{
		GameObject gameObject = appendGlobalObj(GlobalObjectParam.eObject.MapMainUI2);
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
	}

	public void checkAvatarCampaign()
	{
		if (GlobalData.Instance.getGameData().isGachaUpCampaign)
		{
			gachaButton_.Find("campaign_02").gameObject.SetActive(true);
			return;
		}
		gachaButton_.Find("campaign_02").gameObject.SetActive(false);
		if (GlobalData.Instance.getGameData().isGachaSaleCampaign)
		{
			gachaButton_.Find("campaign").gameObject.SetActive(true);
		}
		else
		{
			gachaButton_.Find("campaign").gameObject.SetActive(false);
		}
	}

	private IEnumerator openCommonDialog(int msgID)
	{
		DialogCommon dialog = dialogManager.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		Vector3 temp = dialog.gameObject.transform.localPosition;
		temp.z -= 300f;
		dialog.gameObject.transform.localPosition = temp;
		dialog.setup(msgID, null, null, true);
		dialog.setButtonActive(DialogCommon.eBtn.Close, false);
		yield return dialogManager.StartCoroutine(dialogManager.openDialog(dialog));
		while (dialog.isOpen())
		{
			yield return 0;
		}
		temp.z += 300f;
		dialog.gameObject.transform.localPosition = temp;
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
		GlobalData.Instance.getGameData().saleArea = data.saleArea;
		GlobalData.Instance.getGameData().areaSalePercent = data.areaSalePercent;
		GlobalData.Instance.getGameData().isAreaCampaign = data.isAreaCampaign;
		EventMenu.updateGetTime();
		DailyMission.updateGetTime();
		GlobalData.Instance.getGameData().setParkData(null, null, data.thanksList, data.buildings, data.mapReleaseNum);
		SaveData.Instance.getParkData().UpdatePlacedData(data.buildings);
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
				dailyMission.dailyMissionInfoSetup();
			}
		}
		else
		{
			dailyMission.gameObject.SetActive(false);
		}
		yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		bInactive = false;
	}

	private bool stageAliveCheck(StageIcon stageIcon)
	{
		int stageNo = stageIcon.getStageNo();
		StageDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		EventStageInfo eventData = @object.GetComponent<StageDataTable>().getEventData();
		EventStageInfo.Info eventInfo = component.getEventInfo(stageNo, eventData.EventNo);
		if (eventInfo != null)
		{
			return true;
		}
		return false;
	}
}
