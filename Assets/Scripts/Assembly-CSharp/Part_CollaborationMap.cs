using System;
using System.Collections;
using System.Collections.Generic;
using Bridge;
using LitJson;
using Network;
using UnityEngine;

public class Part_CollaborationMap : PartBase
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

	private Area[] areas_;

	private Area directArea_;

	private int mailNum_;

	private BagMenu bagMenu_;

	private GameObject map_;

	private CollaborationMenu collaborationMenu_;

	private BossMenu bossMenu_;

	private Part_Map.ResultParam resultParam_ = new Part_Map.ResultParam();

	private bool bEventStage_;

	private bool bShowedNextSetup_;

	private bool bNextStage_;

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

	private UISprite chara_00;

	private UISprite chara_01;

	private UISprite chara_02;

	private UISprite chara_03;

	private bool blowcheck;

	public bool transBonus;

	public bool isBonusGamePlayed;

	private bool showPlayerDataEneble = true;

	private bool bInactive;

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
		yield return StartCoroutine(setupWorldMap());
		mainMenu_ = appendGlobalObj(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		mainMenu_.update();
		setupButton(mainMenu_.gameObject, true);
		GameObject menu2 = appendGlobalObj(GlobalObjectParam.eObject.MapMainUI2);
		bagMenu_ = menu2.transform.Find("bag_menu").GetComponent<BagMenu>();
		collaborationMenu_ = menu2.transform.Find("collaboration").GetComponent<CollaborationMenu>();
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
		collaborationMenu_.updateEnable(PartManager.ePart.CollaborationMap);
		dailyMissionValueSetting();
		yield return StartCoroutine(dailyMission.dailyMissionInfoSetup());
		if (eventTbl_ == null || !collaborationMenu_.isEventDuration() || eventTbl_.EventNo != 11)
		{
			StartCoroutine(openNoneEventDioalog());
			yield break;
		}
		stageIconParent_ = appendGlobalObj(GlobalObjectParam.eObject.CollaborationStageIcon).GetComponent<StageIconParent>();
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
		yield return StartCoroutine(loadDialog());
		bNextStage_ = isNextStage(currentStage);
		mapCamera_.EventMoveRangeMax = new Vector2(mapCamera_.DEF_EVENT_RANGE_MAX_W, 100 * (getEventStageNum() - 2));
		mapCamera_.EventMoveRange = new Vector4(0f, mapCamera_.DEF_EVENT_RANGE_CENTER_H - 125f + (float)(100 * (getEventStageNum() - 2)) / 2f, mapCamera_.DEF_EVENT_RANGE_W, 100 * (getEventStageNum() - 2));
		float CurrentStageNum3 = convEventStageNoToIndex(currentStage);
		CurrentStageNum3 = ((CurrentStageNum3 != 0f) ? (CurrentStageNum3 - 1f) : 0f);
		CurrentStageNum3 = ((!(CurrentStageNum3 >= (float)(getEventStageNum() - 1))) ? CurrentStageNum3 : ((float)(getEventStageNum() - 2)));
		float par = CurrentStageNum3 / ((float)getEventStageNum() - 2f);
		float min = mapCamera_.EventMoveRange.y - mapCamera_.EventMoveRange.w / 2f;
		float dif = min - mapCamera_.EventMoveRange.y;
		Vector2 range_max = new Vector2(mapCamera_.EventMoveRangeMax.x, mapCamera_.EventMoveRangeMax.y * par);
		Vector4 range = new Vector4(mapCamera_.EventMoveRange.x, min - dif * par, mapCamera_.EventMoveRange.z, mapCamera_.EventMoveRange.w * par);
		mapCamera_.setMoveRange(range_max, range);
		if (bNextStage_)
		{
			if (currentStage % 10000 != 1)
			{
				currentStage--;
			}
		}
		else if (isLastStage(currentStage - 1))
		{
			currentStage--;
		}
		clearStage_ = setupStageIcon(stageIconParent_, currentStage, bNextStage_);
		areas_ = map_.GetComponentsInChildren<Area>(true);
		directArea_ = setupArea(areas_, currentStage, bNextStage_);
		GameObject currentIcon = stageIconParent_.getIcon(currentStage).gameObject;
		showCurrentEffect(currentIcon);
		showPlayer(currentIcon);
		focus(currentStage);
		sendOneButtonObjct = menu2.transform.Find("sendOneStage").gameObject;
		sendOneButtonObjct.GetComponentInChildren<UIButton>().isEnabled = true;
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
		DialogCollaborationSetup collaboSetup = dialogManager.getDialog(DialogManager.eDialog.CollaborationSetup) as DialogCollaborationSetup;
		collaboSetup.setCollaborationMenu(collaborationMenu_);
		if (collaboSetup.isOpen())
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
		Input.enable = false;
		if (bNextStage_)
		{
			bool bAnimation = true;
			if (resultParam_.StageNo != 0 && resultParam_.StageNo != currentStage - 1)
			{
				bAnimation = false;
			}
			yield return StartCoroutine(clearStage_.setup(bAnimation, true));
			if (!isLastStage(currentStage - 1))
			{
				if (directArea_ != null)
				{
					EventStageInfo.Info info = eventTbl_.Infos[convEventStageNoToIndex(currentStage)];
					yield return StartCoroutine(scrollMap(currentStage));
					yield return StartCoroutine(directArea_.openDirect(Area.eMap.Collaboration, true));
				}
				yield return StartCoroutine(moveNextStage(currentStage));
			}
			otherData_.setCollaborationStageNo(currentStage);
		}
		else if (!bEventStage_ && resultParam_.IsClear && resultParam_.StageNo == currentStage && !resultParam_.IsProgressOpen)
		{
			Input.enable = true;
			yield return StartCoroutine(openAreaLockDialog());
			Input.enable = false;
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
		dialog.setup(this, DialogDailyMissionClear.eTargetPart.CollaborationMap);
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
		if (stageNo % 10000 != 1 && !otherData_.isPlayedCollaborationStageProd(stageNo))
		{
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
				if (area2.getAreaNo() != num)
				{
					area2.open();
					area2.setGateEnable(false);
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
			component.setup(currentIcon.transform.localPosition, this, GamePlayer.eTargetPart.CollaborationMap);
			component.gameObject.SetActive(true);
		}
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
				DialogCollaborationSetup setupDialog = dialogManager.getDialog(DialogManager.eDialog.CollaborationSetup) as DialogCollaborationSetup;
				yield return StartCoroutine(setupDialog.show(icon, this));
				yield break;
			}
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			Input.enable = true;
			yield return dialogManager.StartCoroutine(openNoneEventDioalog());
			yield return StartCoroutine(transMap());
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
				gachaDialog.resetGachaNo();
				gachaDialog.setup(GlobalData.Instance.getGameData().isFirstGacha);
				gachaDialog.gacha_map_button = gachaButton_;
				yield return StartCoroutine(dialogManager.openDialog(gachaDialog));
			}
			yield break;
		}
		switch (name)
		{
		case "BackButton":
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(transMap());
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
			DialogInvite dialog5 = dialogManager.getDialog(DialogManager.eDialog.Invite) as DialogInvite;
			yield return StartCoroutine(dialog5.show());
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
		switch (trigger.transform.parent.name)
		{
		case "01_coin":
		{
			DialogAllShop dialog = dialogManager.getDialog(DialogManager.eDialog.AllShop) as DialogAllShop;
			yield return StartCoroutine(dialog.show(DialogAllShop.ePanelType.Coin));
			break;
		}
		case "02_jewel":
		{
			DialogAllShop dialog2 = dialogManager.getDialog(DialogManager.eDialog.AllShop) as DialogAllShop;
			yield return StartCoroutine(dialog2.show(DialogAllShop.ePanelType.Jewel));
			break;
		}
		case "04_heart":
		{
			DialogAllShop dialog3 = dialogManager.getDialog(DialogManager.eDialog.AllShop) as DialogAllShop;
			yield return StartCoroutine(dialog3.show(DialogAllShop.ePanelType.Heart));
			break;
		}
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
		else if (isPlayedStage() && !isLastStage(resultParam_.StageNo) && resultParam_.IsClear && !resultParam_.IsRetry && !isReclear())
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
					if (!resultParam_.IsResultClose)
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
		if (bNextStage_)
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
		DialogCollaborationSetup setupDialog = dialogManager.getDialog(DialogManager.eDialog.CollaborationSetup) as DialogCollaborationSetup;
		yield return StartCoroutine(setupDialog.show(stageIconParent_.getIcon(stageNo), this));
		bButtonEnable_ = true;
	}

	private IEnumerator moveNextStage(int stage)
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
		GameObject currentIcon = stageIconParent_.getIcon(stage).gameObject;
		yield return StartCoroutine(mapCamera_.moveProd(currentIcon.transform.localPosition));
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
		int count = stageIconParent_.getKeys().Count;
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
		yield return StartCoroutine(dialogManager.load(PartManager.ePart.CollaborationMap));
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

	public IEnumerator sendInactive()
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
		GlobalData.Instance.getGameData().gachaTicket = data.gachaTicket;
		Debug.Log("gachaTicket = " + data.gachaTicket);
		CollaborationMenu.updateGetTime();
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
		DialogCollaborationSetup setup = dialogManager.getDialog(DialogManager.eDialog.CollaborationSetup) as DialogCollaborationSetup;
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
			if (collaborationMenu_ != null)
			{
				collaborationMenu_.updateEnable(PartManager.ePart.CollaborationMap);
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
		globalObj_.unload(GlobalObjectParam.eObject.CollaborationStageIcon);
		globalObj_.unload(GlobalObjectParam.eObject.CollaborationMap);
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
		GameObject stageIconRoot = new GameObject(GlobalObjectParam.getName(GlobalObjectParam.eObject.CollaborationStageIcon));
		globalObj_.appendObject(stageIconRoot, GlobalObjectParam.eObject.CollaborationStageIcon, false);
		Utility.setParent(stageIconRoot, worldMap.transform, false);
		GameObject objBase = ResourceLoader.Instance.loadGameObject("Prefabs/", "stageicon");
		for (int i = 0; i < data.IconNum; i++)
		{
			IconData iconData = data.IconDatas[i];
			GameObject stageIcon = UnityEngine.Object.Instantiate(objBase) as GameObject;
			Utility.setParent(stageIcon, stageIconRoot.transform, false);
			stageIcon.GetComponent<StageIcon>().setStageNo(iconData.StageNo);
			Vector3 pos = iconData.Pos;
			stageIcon.transform.localPosition = new Vector3(pos.x, pos.y, stageIcon.transform.localPosition.z);
			stageIcon.SetActive(true);
			yield return 0;
		}
		stageIconRoot.AddComponent<StageIconParent>().setup();
	}

	private IEnumerator setupWorldMap()
	{
		map_ = globalObj_.load("Prefabs/", GlobalObjectParam.eObject.CollaborationMap, false);
		StageIconDataTable iconData = globalObj_.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageIconDataTable>();
		iconData.load();
		yield return StartCoroutine(loadStageIcons(map_, iconData.getCollaborationData()));
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
			if (CollaborationMenu.isPrevLevelClear(info))
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

	private int getEventStageNum()
	{
		if (GlobalData.Instance.getGameData() == null)
		{
			return 10;
		}
		int eventMaxStageNo = GlobalData.Instance.getGameData().eventMaxStageNo;
		if (iconTbl_ != null && iconTbl_.getCollaborationData().IconNum + 10000 < eventMaxStageNo)
		{
			return eventMaxStageNo % 10000;
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
}
