using System;
using System.Collections;
using System.Collections.Generic;
using Bridge;
using LitJson;
using Network;
using UnityEngine;

public class Part_Park : PartBase
{
	private struct TopMenuUI
	{
		public GameObject all_01_coin;

		public GameObject all_02_jewel;

		public GameObject all_03_exp;

		public GameObject all_04_heart;

		public void findObject(GameObject mainMenu)
		{
			all_01_coin = getChild(mainMenu, "all/01_coin");
			all_02_jewel = getChild(mainMenu, "all/02_jewel");
			all_03_exp = getChild(mainMenu, "all/03_exp");
			all_04_heart = getChild(mainMenu, "all/04_heart");
		}
	}

	private struct BottomMenuUI
	{
		public GameObject sendOne_button;

		public GameObject sendOne_button_park;

		public BagMenu bag_menu;

		public GameObject bag_menu_05_ranking;

		public GameObject bag_menu_06_mail;

		public GameObject bag_menu_07_option;

		public GameObject bag_menu_08_invite;

		public GameObject bag_menu_09_avatar;

		public GameObject bag_menu_10_estimation;

		public GameObject bag_menu_11_minilen;

		public GameObject gacha;

		public DailyMission dailyMission;

		public BossMenu bossMenu;

		public EventMenu eventMenu;

		public CollaborationMenu collaborationMenu;

		public MenuParkStage parkStage;

		public MenuMinilenThanks minilenMenu;

		public GameObject goToPark;

		public CrossMenu crossMenu_;

		public ADMenu adMenu_;

		public OfferwallMenu offerwallMenu_;

		public void findObject(GameObject mainMenu)
		{
			sendOne_button = getChild(mainMenu, "sendOneStage/sendOne_button");
			sendOne_button_park = getChild(mainMenu, "sendOneStage/sendOne_button_park");
			bag_menu = getChild(mainMenu, "bag_menu").GetComponent<BagMenu>();
			bag_menu_05_ranking = getChild(bag_menu.gameObject, "05_ranking");
			bag_menu_06_mail = getChild(bag_menu.gameObject, "06_mail");
			bag_menu_07_option = getChild(bag_menu.gameObject, "07_option");
			bag_menu_08_invite = getChild(bag_menu.gameObject, "08_invite");
			bag_menu_09_avatar = getChild(bag_menu.gameObject, "09_avatar");
			bag_menu_10_estimation = getChild(bag_menu.gameObject, "10_estimation");
			bag_menu_11_minilen = getChild(bag_menu.gameObject, "11_minilen");
			gacha = getChild(mainMenu, "gacha");
			dailyMission = getChild(mainMenu, "daily").GetComponent<DailyMission>();
			bossMenu = getChild(mainMenu, "boss").GetComponent<BossMenu>();
			eventMenu = getChild(mainMenu, "event").GetComponent<EventMenu>();
			collaborationMenu = getChild(mainMenu, "collaboration").GetComponent<CollaborationMenu>();
			parkStage = getChild(mainMenu, "park_stage").GetComponent<MenuParkStage>();
			minilenMenu = getChild(mainMenu, "minilen").GetComponent<MenuMinilenThanks>();
			goToPark = getChild(mainMenu, "sendOneStage/goToPark");
			crossMenu_ = getChild(mainMenu, "cross").GetComponent<CrossMenu>();
			adMenu_ = getChild(mainMenu, "ad").GetComponent<ADMenu>();
			offerwallMenu_ = getChild(mainMenu, "Offerwall").GetComponent<OfferwallMenu>();
		}

		public void setActiveForPark()
		{
			gacha.SetActive(false);
			bossMenu.gameObject.SetActive(false);
			eventMenu.gameObject.SetActive(false);
			collaborationMenu.gameObject.SetActive(false);
			parkStage.gameObject.SetActive(true);
			goToPark.SetActive(false);
			bag_menu_05_ranking.SetActive(false);
			bag_menu_10_estimation.SetActive(true);
			bag_menu_09_avatar.SetActive(false);
			bag_menu_11_minilen.SetActive(true);
			sendOne_button.SetActive(false);
			sendOne_button_park.SetActive(true);
			minilenMenu.gameObject.SetActive(true);
			getChild(minilenMenu.gameObject, "minilen_button").SetActive(true);
			crossMenu_.gameObject.SetActive(false);
			adMenu_.gameObject.SetActive(false);
			offerwallMenu_.gameObject.SetActive(false);
		}

		public void setActiveForTransitionOtherScene()
		{
			gacha.SetActive(true);
			bag_menu_05_ranking.SetActive(true);
			bossMenu.gameObject.SetActive(true);
			eventMenu.gameObject.SetActive(true);
			collaborationMenu.gameObject.SetActive(true);
			parkStage.gameObject.SetActive(false);
			goToPark.SetActive(true);
			bag_menu_05_ranking.SetActive(true);
			bag_menu_10_estimation.SetActive(false);
			bag_menu_09_avatar.SetActive(true);
			bag_menu_11_minilen.SetActive(false);
			sendOne_button.SetActive(true);
			sendOne_button_park.SetActive(false);
			getChild(minilenMenu.gameObject, "minilen_button").SetActive(true);
			minilenMenu.gameObject.SetActive(false);
			crossMenu_.gameObject.SetActive(true);
			adMenu_.gameObject.SetActive(true);
			offerwallMenu_.gameObject.SetActive(true);
		}
	}

	private enum eListItem
	{
		Mail = 0,
		MailPresent = 1,
		Invite = 2,
		Request = 3,
		FriendHelp = 4,
		ParkArea = 5,
		ParkStage = 6,
		ParkThanks = 7,
		ParkBuilding = 8,
		ParkRoad = 9,
		ParkNiceHistory = 10,
		ParkFriendList = 11,
		ParkRewardCheck = 12,
		Max = 13
	}

	private class DownloadInfo
	{
		public Constant.eDownloadDataNo downloadNo;

		public int downloadDataIndex;

		public DownloadInfo(Constant.eDownloadDataNo no, int index)
		{
			downloadNo = no;
			downloadDataIndex = index;
		}
	}

	private static readonly DownloadInfo[] DOWNLOAD_INFOS = new DownloadInfo[35]
	{
		new DownloadInfo(Constant.eDownloadDataNo.Park, 0),
		new DownloadInfo(Constant.eDownloadDataNo.Park, 1),
		new DownloadInfo(Constant.eDownloadDataNo.Park, 2),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 0),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 1),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 2),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 3),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 4),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 5),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 6),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 7),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 8),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 9),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 10),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 11),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 12),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 13),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 14),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 15),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 16),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 17),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 18),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 19),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 20),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 21),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 22),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 23),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 24),
		new DownloadInfo(Constant.eDownloadDataNo.Minilen, 25),
		new DownloadInfo(Constant.eDownloadDataNo.BG, 12),
		new DownloadInfo(Constant.eDownloadDataNo.BG, 13),
		new DownloadInfo(Constant.eDownloadDataNo.BG, 14),
		new DownloadInfo(Constant.eDownloadDataNo.BG, 15),
		new DownloadInfo(Constant.eDownloadDataNo.Main, 11),
		new DownloadInfo(Constant.eDownloadDataNo.Main, 12)
	};

	private List<GameObject> globalObjectList_ = new List<GameObject>();

	private GlobalRoot globalRoot_;

	private MainMenu mainMenu_;

	private TopMenuUI topMenuUI_;

	private GameObject mainMenu2_;

	private BottomMenuUI bottomMenuUI_;

	private bool buttonEnable_ = true;

	private int unopenedMailCount_;

	private GameObject[] listItems_ = new GameObject[13];

	private MenuParkFriendMap friendParkMenu_;

	private SaveGameData gameData_;

	private SaveOtherData otherData_;

	private UISprite minilen_00;

	private GlobalRoot globalObj_;

	private MinilenThanksDialogManager _thanke_dialogs;

	private DailyMission dailyMission_;

	private bool transitBonusStage_;

	private bool isBonusGamePlayed_;

	private bool wasInitialized_;

	private bool isInactive_;

	private Part_Map.ResultParam resultParam_ = new Part_Map.ResultParam();

	private static bool _get_local_daily_reward = false;

	private bool isTransitingMap_;

	private bool _is_execute_wait;

	private static GameObject getChild(GameObject parent, string path)
	{
		Transform transform = parent.transform.Find(path);
		if (transform == null)
		{
			return null;
		}
		return transform.gameObject;
	}

	public override IEnumerator setup(Hashtable args)
	{
		buttonEnable_ = false;
		globalRoot_ = GlobalRoot.Instance;
		gameData_ = SaveData.Instance.getGameData();
		otherData_ = gameData_.getOtherData();
		analyzeHash(args);
		GameObject dataTable = globalRoot_.getObject(GlobalObjectParam.eObject.DataTable);
		Bridge.MinilenData.Setup();
		dataTable.GetComponent<ParkBuildingDataTable>().load();
		dataTable.GetComponent<ParkAreaReleaseDataTable>().load();
		dataTable.GetComponent<ParkRoadPlacedDataTable>().load();
		dataTable.GetComponent<ParkMapDataTable>().load();
		dataTable.GetComponent<ParkUnplacableGridDataTable>().load();
		if (resultParam_.IsForceSendInactive || (dailyMission_ != null && dailyMission_.dailyMissionChangeCheck() && DailyMission.isTermClear()))
		{
			yield return StartCoroutine(forceRecover());
		}
		SaveParkData parkData = SaveData.Instance.getParkData();
		GameData gameData = GlobalData.Instance.getGameData();
		BuildingData[] roadArray = Array.FindAll(gameData.buildings, (BuildingData b) => b.id >= 50000);
		int currentRoadID = 0;
		if (roadArray == null || roadArray.Length == 0)
		{
			currentRoadID = parkData.roadID;
		}
		else
		{
			for (int i = 0; i < roadArray.Length; i++)
			{
				if (roadArray[i].x >= 0)
				{
					currentRoadID = roadArray[i].id;
					break;
				}
			}
		}
		parkData.roadID = currentRoadID;
		parkData.areaReleasedCount = gameData.mapReleaseNum;
		parkData.UpdatePlacedData(gameData.buildings);
		parkData.save();
		StageDataTable stageTable = dataTable.GetComponent<StageDataTable>();
		string text = string.Empty;
		yield return StartCoroutine(stageTable.downloadParkDummyData(false, true, delegate(string ret)
		{
			text = ret;
		}));
		parkData.setDummyFriendData(text);
		yield return StartCoroutine(downloadGameResources());
		GameObject obj = globalRoot_.load("Prefabs/", GlobalObjectParam.eObject.FriendParkUI, false);
		obj.SetActive(false);
		mainMenu_ = appendGlobalObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		mainMenu_.gameObject.SetActive(true);
		mainMenu_.update();
		setupButton(mainMenu_.gameObject, true);
		topMenuUI_.findObject(mainMenu_.gameObject);
		mainMenu2_ = appendGlobalObject(GlobalObjectParam.eObject.MapMainUI2);
		mainMenu2_.SetActive(true);
		setupButton(mainMenu2_.gameObject, true);
		minilen_00 = mainMenu2_.transform.Find("bag_menu/11_minilen/minilen_avatar_button/avatar/chara_00").GetComponent<UISprite>();
		UpdateMinilenIcon();
		bottomMenuUI_.findObject(mainMenu2_);
		dailyMission_ = bottomMenuUI_.dailyMission;
		bottomMenuUI_.setActiveForPark();
		yield return StartCoroutine(loadDialog());
		bottomMenuUI_.parkStage.Reposition();
		bottomMenuUI_.minilenMenu.Reposition();
		GameObject menuObject = appendGlobalObject(GlobalObjectParam.eObject.FriendParkUI);
		friendParkMenu_ = menuObject.GetComponent<MenuParkFriendMap>();
		friendParkMenu_.init(dialogManager, uiRoot.transform);
		wasInitialized_ = true;
		yield return StartCoroutine(sendInactive());
		int newNiceCount = getNewNiceCount(GlobalData.Instance.getGameData().niceList, false);
		setNewNiceCount(newNiceCount);
		dailyMissionValueSetting();
		if (dailyMission_.gameObject.activeSelf)
		{
			yield return StartCoroutine(dailyMission_.dailyMissionInfoSetup());
		}
		setMailCount(Bridge.PlayerData.getMailUnReadCount());
		bottomMenuUI_.minilenMenu.UpdateCount();
		ColliderManager colliderManager = ColliderManager.Instance;
		colliderManager.setParent(base.transform);
		ParkObjectManager objectManager = ParkObjectManager.Instance;
		objectManager.setParent(base.transform);
		yield return StartCoroutine(objectManager.setup(base.transform, dialogManager));
		yield return StartCoroutine(objectManager.createParkMapForPlayer(parkData.roadID, parkData.areaReleasedCount, parkData.buildings.ToArray()));
		StartCoroutine(execute(args));
	}

	public void UpdateMinilenIcon()
	{
		Network.MinilenData current = Bridge.MinilenData.getCurrent();
		if (current != null)
		{
			minilen_00.spriteName = "UI_picturebook_mini_" + (current.index % 10000).ToString("000");
			minilen_00.MakePixelPerfect();
		}
	}

	private bool isPlayedStage()
	{
		PartManager.ePart prevPart = partManager.prevPart;
		return prevPart == PartManager.ePart.Stage || prevPart == PartManager.ePart.Scenario;
	}

	private void analyzeHash(Hashtable args)
	{
		if (args != null && (isPlayedStage() || partManager.prevPart == PartManager.ePart.EventMap || partManager.prevPart == PartManager.ePart.ChallengeMap || partManager.prevPart == PartManager.ePart.CollaborationMap || partManager.prevPart == PartManager.ePart.BonusStage))
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

	private IEnumerator loadDialog()
	{
		listItems_[0] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "mail_item")) as GameObject;
		listItems_[1] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "present_item")) as GameObject;
		listItems_[2] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "invite_item")) as GameObject;
		listItems_[3] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Request_item")) as GameObject;
		listItems_[4] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Rescue_item")) as GameObject;
		listItems_[5] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "park_area_item")) as GameObject;
		listItems_[6] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "park_stage_item")) as GameObject;
		listItems_[7] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "park_thanks_item")) as GameObject;
		listItems_[8] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "park_building_item")) as GameObject;
		listItems_[9] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "park_road_item")) as GameObject;
		listItems_[10] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "park_nice_history_item")) as GameObject;
		listItems_[11] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "park_nice_history_item")) as GameObject;
		listItems_[12] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Park_RewardCheck_item")) as GameObject;
		GameObject[] array = listItems_;
		foreach (GameObject item in array)
		{
			Utility.setParent(item, base.transform, false);
			item.SetActive(false);
		}
		yield return StartCoroutine(dialogManager.load(PartManager.ePart.Park));
		DialogInvite dialog12 = dialogManager.getDialog(DialogManager.eDialog.Invite) as DialogInvite;
		dialog12.init(listItems_[2]);
		DialogRequest dialog11 = dialogManager.getDialog(DialogManager.eDialog.Request) as DialogRequest;
		dialog11.init(listItems_[3]);
		DialogMail dialog10 = dialogManager.getDialog(DialogManager.eDialog.Mail) as DialogMail;
		dialog10.init(listItems_[0], listItems_[1]);
		DialogFriendHelp dialog9 = dialogManager.getDialog(DialogManager.eDialog.FriendHelp) as DialogFriendHelp;
		dialog9.init(listItems_[4]);
		DialogParkAreaList dialog8 = dialogManager.getDialog(DialogManager.eDialog.ParkAreaList) as DialogParkAreaList;
		dialog8.init(listItems_[5]);
		DialogParkStageList dialog7 = dialogManager.getDialog(DialogManager.eDialog.ParkStageList) as DialogParkStageList;
		dialog7.init(listItems_[6]);
		DialogParkRemoveConfirmation dialog6 = dialogManager.getDialog(DialogManager.eDialog.ParkRemoveConfirm) as DialogParkRemoveConfirmation;
		DialogParkObjectChoices dialog5 = dialogManager.getDialog(DialogManager.eDialog.ParkObjectChoices) as DialogParkObjectChoices;
		yield return StartCoroutine(dialog5.setup());
		dialog5.gameObject.SetActive(true);
		_thanke_dialogs = MinilenThanksDialogManager.Init(listItems_[7], listItems_[8], listItems_[9]);
		DialogParkNiceDetail dialog4 = dialogManager.getDialog(DialogManager.eDialog.ParkNiceDetail) as DialogParkNiceDetail;
		DialogParkNiceHistoryList dialog3 = dialogManager.getDialog(DialogManager.eDialog.ParkNiceHistoryList) as DialogParkNiceHistoryList;
		dialog3.init(listItems_[10]);
		DialogParkFriendList dialog2 = dialogManager.getDialog(DialogManager.eDialog.ParkFriendList) as DialogParkFriendList;
		dialog2.init(listItems_[11]);
		DialogParkRewardCheckList dialog = dialogManager.getDialog(DialogManager.eDialog.ParkRewardCheckList) as DialogParkRewardCheckList;
		dialog.init(listItems_[12]);
		DialogSetupPark dialog_setup = dialogManager.getDialog(DialogManager.eDialog.ParkStageSetup) as DialogSetupPark;
		dialog_setup.Init(this);
	}

	private IEnumerator downloadGameResources()
	{
		GlobalData.Instance.ignoreLodingIcon = true;
		StageDataTable stageTable = globalRoot_.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
		int dlCount = 0;
		for (int i = 0; i < DOWNLOAD_INFOS.Length; i++)
		{
			bool close = false;
			if (DOWNLOAD_INFOS.Length - 1 <= i)
			{
				close = true;
			}
			yield return StartCoroutine(stageTable.downloadGameResource(DOWNLOAD_INFOS[i].downloadNo, DOWNLOAD_INFOS[i].downloadDataIndex, close, i, DOWNLOAD_INFOS.Length));
		}
		GlobalData.Instance.ignoreLodingIcon = false;
	}

	private IEnumerator openCommonDialog(int messageID)
	{
		DialogCommon dialogCommon = dialogManager.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		dialogCommon.setup(messageID, null, null, true);
		dialogCommon.setButtonActive(DialogCommon.eBtn.Close, false);
		yield return StartCoroutine(dialogManager.openDialog(dialogCommon));
		while (dialogCommon.isOpen())
		{
			yield return null;
		}
	}

	private void LateUpdate()
	{
		if (_is_execute_wait && Input.enable)
		{
			Input.enable = false;
			_is_execute_wait = false;
		}
	}

	private IEnumerator execute(Hashtable args)
	{
		Sound.Instance.playBgm(Sound.eBgm.BGM_011_ParkMap, true);
		UICamera.currentCamera.clearFlags = CameraClearFlags.Depth;
		_is_execute_wait = true;
		while (_is_execute_wait)
		{
			yield return null;
		}
		Input.enable = true;
		wasInitialized_ = true;
		FadeMng fade = partManager.fade;
		if (Constant.ParkStage.isParkStage(resultParam_.StageNo) && resultParam_.IsClear && Bridge.StageData.getClearCount_Park(resultParam_.StageNo) == 1)
		{
			int next_stag_no = resultParam_.StageNo + 1;
			StageDataTable dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
			if (dataTable.getInfo(next_stag_no) == null)
			{
				DialogConfirm tobecontinue = (DialogConfirm)dialogManager.getDialog(DialogManager.eDialog.ToBeContinue);
				yield return StartCoroutine(dialogManager.openDialog(DialogManager.eDialog.ToBeContinue));
				while (tobecontinue.isOpen())
				{
					yield return 0;
				}
			}
		}
		if (!ParkTutorialUtility.instance.IsPlayable(ParkTutorialUtility.eTutorial.Max))
		{
			yield return StartCoroutine(showRewardCheck());
			if (DailyMission.isTermClear() && !dailyMission_.bonusgamePlayed && dailyMission_.missionCleared)
			{
				yield return StartCoroutine(showDailyMissionCleared());
			}
			if (transitBonusStage_)
			{
				buttonEnable_ = true;
				yield break;
			}
			yield return StartCoroutine(setup_OpenStatgeSetup(args));
		}
		else
		{
			StartCoroutine(ParkTutorialUtility.instance.Play());
		}
		buttonEnable_ = true;
	}

	private IEnumerator showRewardCheck()
	{
		GameData gameData = GlobalData.Instance.getGameData();
		if (gameData == null || !gameData.isParkDailyReward)
		{
			yield break;
		}
		DialogParkRewardCheckList rewardCheckList = dialogManager.getDialog(DialogManager.eDialog.ParkRewardCheckList) as DialogParkRewardCheckList;
		if (!(rewardCheckList == null))
		{
			yield return StartCoroutine(rewardCheckList.show());
			while (rewardCheckList.isOpen())
			{
				yield return null;
			}
			yield return null;
		}
	}

	public override IEnumerator OnDestroyCB()
	{
		bottomMenuUI_.setActiveForTransitionOtherScene();
		UICamera.currentCamera.clearFlags = CameraClearFlags.Color;
		TutorialManager.Instance.unload();
		GlobalRoot instance = GlobalRoot.Instance;
		instance.unload(GlobalObjectParam.eObject.GateEffect);
		instance.unload(GlobalObjectParam.eObject.CurrentStageEffect);
		instance.unload(GlobalObjectParam.eObject.Player);
		instance.unload(GlobalObjectParam.eObject.StageIcon);
		instance.unload(GlobalObjectParam.eObject.TreasureBox);
		instance.unload(GlobalObjectParam.eObject.FriendRoot);
		instance.unload(GlobalObjectParam.eObject.FriendParkUI);
		instance.getObject(GlobalObjectParam.eObject.MapMainUI).SetActive(false);
		instance.getObject(GlobalObjectParam.eObject.MapMainUI2).SetActive(false);
		return base.OnDestroyCB();
	}

	private GameObject appendGlobalObject(GlobalObjectParam.eObject objectType)
	{
		GameObject @object = globalRoot_.getObject(objectType);
		globalObjectList_.Add(@object);
		return @object;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		if (!buttonEnable_ || isTransitingMap_ || (ParkObjectManager.haveInstance && ParkObjectManager.Instance.mapScroll.isAutoScrolling))
		{
			yield break;
		}
		DialogAppQuit appQuit = dialogManager.getDialog(DialogManager.eDialog.AppQuit) as DialogAppQuit;
		if (appQuit.isOpen())
		{
			yield break;
		}
		bool will_transit = false;
		switch (trigger.name)
		{
		case "PlusButton":
			Constant.SoundUtil.PlayButtonSE();
			yield return StartCoroutine(OnClickPlusButton(trigger));
			break;
		case "Exp_Button":
			Constant.SoundUtil.PlayButtonSE();
			yield return StartCoroutine(OnClickExpButton(trigger));
			break;
		case "minilen_button":
			Constant.SoundUtil.PlayButtonSE();
			yield return StartCoroutine(OnClickMinilenThanksButton(trigger));
			break;
		case "sendOne_button_park":
			if (!ParkObjectManager.haveInstance || !ParkObjectManager.Instance.mapScroll.isPinch)
			{
				isTransitingMap_ = true;
				if (ColliderManager.haveInstance)
				{
					ColliderManager.Instance.Clear();
				}
				Constant.SoundUtil.PlayButtonSE();
				yield return StartCoroutine(OnClickBackButton(trigger));
			}
			break;
		case "invite_button":
			Constant.SoundUtil.PlayDecideSE();
			yield return StartCoroutine(OnClickInviteButton(trigger));
			break;
		case "estimation_button":
			Constant.SoundUtil.PlayDecideSE();
			yield return StartCoroutine(OnClickEstimationButton(trigger));
			break;
		case "mail_button":
			Constant.SoundUtil.PlayDecideSE();
			yield return StartCoroutine(OnClickMailButton(trigger));
			break;
		case "option_button":
			Constant.SoundUtil.PlayDecideSE();
			yield return StartCoroutine(OnClickOptionButton(trigger));
			break;
		case "minilen_avatar_button":
			Constant.SoundUtil.PlayDecideSE();
			yield return StartCoroutine(OnClickMinilenButton(trigger));
			break;
		case "ParkStageListButton":
		{
			Constant.SoundUtil.PlayButtonSE();
			DialogParkAreaList area_dialog = dialogManager.getDialog(DialogManager.eDialog.ParkAreaList) as DialogParkAreaList;
			yield return StartCoroutine(area_dialog.show());
			yield break;
		}
		case "popup_root":
			if (!dailyMission_.bonusgamePlayed)
			{
				Constant.SoundUtil.PlayButtonSE();
				StartCoroutine(showDailyMission());
			}
			break;
		}
		if (!will_transit)
		{
		}
	}

	private IEnumerator OnClickPlusButton(GameObject trigger)
	{
		DialogAllShop dialogAllShop = dialogManager.getDialog(DialogManager.eDialog.AllShop) as DialogAllShop;
		switch (trigger.transform.parent.name)
		{
		case "01_coin":
			yield return StartCoroutine(dialogAllShop.show(DialogAllShop.ePanelType.Coin));
			break;
		case "02_jewel":
			yield return StartCoroutine(dialogAllShop.show(DialogAllShop.ePanelType.Jewel));
			break;
		case "04_heart":
			yield return StartCoroutine(dialogAllShop.show(DialogAllShop.ePanelType.Heart));
			break;
		}
	}

	private IEnumerator OnClickExpButton(GameObject trigger)
	{
		mainMenu_.getExpMenu().changeText();
		yield break;
	}

	private IEnumerator OnClickMinilenThanksButton(GameObject trigger)
	{
		yield return StartCoroutine(_thanke_dialogs.Show());
	}

	private IEnumerator OnClickBackButton(GameObject trigger)
	{
		isTransitingMap_ = true;
		yield return StartCoroutine(sendInactive());
		SaveData.Instance.getParkData().save();
		partManager.requestTransition(PartManager.ePart.Map, null, FadeMng.eType.AllMask, true);
	}

	private IEnumerator OnClickInviteButton(GameObject trigger)
	{
		yield return StartCoroutine(partManager.nologin());
		if (!partManager.isNologinCancel)
		{
			DialogInvite dialogInvite = dialogManager.getDialog(DialogManager.eDialog.Invite) as DialogInvite;
			yield return StartCoroutine(dialogInvite.show());
		}
	}

	private IEnumerator OnClickEstimationButton(GameObject trigger)
	{
		int newNiceCount2 = 0;
		NetworkMng.Instance.setup(null);
		yield return NetworkMng.Instance.StartCoroutine(NetworkMng.Instance.download(API.ParkGetNiceList, true, true));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield return NetworkMng.Instance.StartCoroutine(NetworkMng.Instance.showIcon(false));
			yield break;
		}
		WWW www__ = NetworkMng.Instance.getWWW();
		ParkNiceList nice_list = JsonMapper.ToObject<ParkNiceList>(www__.text);
		GameData game_data = GlobalData.Instance.getGameData();
		game_data.niceList = nice_list;
		newNiceCount2 = getNewNiceCount(nice_list, true);
		if (newNiceCount2 >= 1)
		{
			DialogParkNiceHistoryList dialog2 = dialogManager.getDialog(DialogManager.eDialog.ParkNiceHistoryList) as DialogParkNiceHistoryList;
			StartCoroutine(dialog2.show());
			setNewNiceCount(0);
		}
		else
		{
			DialogParkNiceDetail dialog = dialogManager.getDialog(DialogManager.eDialog.ParkNiceDetail) as DialogParkNiceDetail;
			yield return StartCoroutine(dialog.Setup(false));
			StartCoroutine(dialog.open());
		}
	}

	private int getNewNiceCount(ParkNiceList niceList, bool needSave)
	{
		int num = 0;
		long ticks = DateTime.Now.Ticks;
		string @string = PlayerPrefs.GetString("NiceTime");
		if (PlayerPrefs.HasKey("NiceTime") && @string.Length != 0)
		{
			long num2 = long.Parse(@string);
			long ticks2 = ticks - num2;
			TimeSpan timeSpan = new TimeSpan(ticks2);
			ParkNiceListData[] niceList2 = niceList.niceList;
			foreach (ParkNiceListData parkNiceListData in niceList2)
			{
				int[] giveNiceElapsedTimes = parkNiceListData.giveNiceElapsedTimes;
				foreach (int num3 in giveNiceElapsedTimes)
				{
					if (num3 < (int)timeSpan.TotalSeconds)
					{
						num++;
					}
				}
			}
		}
		else
		{
			if (niceList.niceList != null)
			{
				num = niceList.niceList.Length;
			}
			TimeSpan timeSpan2 = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
			float num4 = (float)timeSpan2.TotalSeconds;
		}
		if (needSave)
		{
			PlayerPrefs.SetString("NiceTime", ticks.ToString());
			PlayerPrefs.Save();
		}
		return num;
	}

	private IEnumerator OnClickMailButton(GameObject trigger)
	{
		int resultUnopenedMailCount = 0;
		NetworkMng manager = NetworkMng.Instance;
		manager.setup(null);
		yield return StartCoroutine(manager.download(OnCreateMailListWWW, true));
		if (manager.getStatus() != NetworkMng.eStatus.Success)
		{
			yield break;
		}
		WWW www = manager.getWWW();
		Mail[] mails = JsonMapper.ToObject<MailList>(www.text).mailList;
		int responcedMailNum = mails.Length;
		for (int i = 0; i < responcedMailNum; i++)
		{
			if (!mails[i].isOpen)
			{
				resultUnopenedMailCount++;
			}
		}
		setMailCount(resultUnopenedMailCount);
		if (unopenedMailCount_ > 0)
		{
			DialogMail dialogMail = dialogManager.getDialog(DialogManager.eDialog.Mail) as DialogMail;
			yield return StartCoroutine(dialogMail.show(mails));
			while (dialogMail.isOpen())
			{
				yield return null;
			}
			GlobalData.Instance.getGameData().mailUnReadCount = dialogMail.mailNum_;
			setMailCount(dialogMail.mailNum_);
		}
		else
		{
			setMailCount(0);
			DialogConfirm dialogConfirm = dialogManager.getDialog(DialogManager.eDialog.NoMail) as DialogConfirm;
			yield return StartCoroutine(dialogManager.openDialog(dialogConfirm));
		}
		dailyMissionValueSetting();
		if (bottomMenuUI_.dailyMission.gameObject.activeSelf)
		{
			yield return StartCoroutine(bottomMenuUI_.dailyMission.dailyMissionInfoSetup());
		}
	}

	private WWW OnCreateMailListWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		return WWWWrap.create("mail/list");
	}

	private void setMailCount(int count)
	{
		unopenedMailCount_ = count;
		bottomMenuUI_.bag_menu.setMailNum(count);
	}

	private void setNewNiceCount(int count)
	{
		GameObject gameObject = bottomMenuUI_.bag_menu_10_estimation.transform.Find("notice_icon").gameObject;
		if (count > 0)
		{
			gameObject.SetActive(true);
			UILabel componentInChildren = bottomMenuUI_.bag_menu_10_estimation.GetComponentInChildren<UILabel>();
			if (componentInChildren != null)
			{
				componentInChildren.text = count.ToString();
			}
		}
		else
		{
			gameObject.SetActive(false);
		}
	}

	private IEnumerator OnClickOptionButton(GameObject trigger)
	{
		DialogOption dialogOption = dialogManager.getDialog(DialogManager.eDialog.Option) as DialogOption;
		dialogOption.setup();
		yield return StartCoroutine(dialogManager.openDialog(dialogOption));
	}

	private IEnumerator OnClickAvatarButton(GameObject trigger)
	{
		GlobalData.Instance.acInfo_.isSetup = false;
		DialogParkMinilenCollection dialogCollection = dialogManager.getDialog(DialogManager.eDialog.ParkMinilenCollection) as DialogParkMinilenCollection;
		if (!dialogCollection.isOpen())
		{
			dialogCollection.setup();
			yield return StartCoroutine(dialogCollection.open());
		}
	}

	private IEnumerator OnClickMinilenButton(GameObject trigger)
	{
		GlobalData.Instance.acInfo_.isSetup = false;
		DialogParkMinilenCollection dialogCollection = dialogManager.getDialog(DialogManager.eDialog.ParkMinilenCollection) as DialogParkMinilenCollection;
		if (!dialogCollection.isOpen())
		{
			dialogCollection.setup();
			yield return StartCoroutine(dialogCollection.open());
		}
	}

	private int getLastStageNo()
	{
		int num = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageIconDataTable>().getMaxStageIconsNum() - 1;
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

	private IEnumerator setup_OpenStatgeSetup(Hashtable args)
	{
		DialogSetupPark dialog_setup = dialogManager.getDialog(DialogManager.eDialog.ParkStageSetup) as DialogSetupPark;
		int open_stage_no = -1;
		if (args == null || args.ContainsKey("IsClose"))
		{
			yield break;
		}
		if (args.Contains("IsRetry"))
		{
			if (args.ContainsKey("StageNo"))
			{
				open_stage_no = (int)args["StageNo"];
			}
		}
		else
		{
			if (args.ContainsKey("IsExit"))
			{
				yield break;
			}
			if (args.ContainsKey("StageNo"))
			{
				open_stage_no = (int)args["StageNo"] + 1;
				if (!Bridge.StageData.isOpen_Park(open_stage_no, GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>().getParkStageData()))
				{
					open_stage_no = -1;
				}
			}
		}
		if (open_stage_no >= 0)
		{
			StageIcon stage_icon = base.gameObject.AddComponent<StageIcon>();
			stage_icon.setStageNum_Park(open_stage_no);
			StageDataTable dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
			int area_id = dataTbl.getParkInfo(open_stage_no).Area;
			dialog_setup.gameObject.SetActive(true);
			dialog_setup.StartCoroutine(dialog_setup.show(stage_icon, area_id));
		}
		else
		{
			DialogParkAreaList area_dialog = dialogManager.getDialog(DialogManager.eDialog.ParkAreaList) as DialogParkAreaList;
			yield return StartCoroutine(area_dialog.show());
		}
	}

	public void dailyMissionValueSetting()
	{
		bool flag = bottomMenuUI_.dailyMission.updateDailyMissionData();
		bottomMenuUI_.dailyMission.gameObject.SetActive(flag);
		Transform transform = mainMenu2_.transform.Find("sendOneStage");
		if (flag)
		{
			transform.localPosition = GlobalData.Instance.mapJumpDefaultPos;
		}
		else
		{
			transform.localPosition = GlobalData.Instance.mapJumpDefaultPos - new Vector3(0f, 65f, 0f);
		}
	}

	private IEnumerator showDailyMission()
	{
		while (dialogManager.getActiveDialogNum() > 0)
		{
			yield return null;
		}
		DailyMission.bMissionCreate = false;
		DialogDailyMission dialog = dialogManager.getDialog(DialogManager.eDialog.DailyMission) as DialogDailyMission;
		dialog.setup(dailyMission_.missionNum, dailyMission_.mission_target);
		yield return StartCoroutine(dialogManager.openDialog(dialog));
		while (dialog.isOpen())
		{
			yield return null;
		}
	}

	private IEnumerator showDailyMissionCleared()
	{
		DialogDailyMissionClear dialog = dialogManager.getDialog(DialogManager.eDialog.DailyMissionClear) as DialogDailyMissionClear;
		if (!dialog.isOpen())
		{
			while (dialogManager.getActiveDialogNum() > 0)
			{
				yield return null;
			}
			Sound.Instance.playSe(Sound.eSe.SE_108_Yay);
			Sound.Instance.playSe(Sound.eSe.SE_113_Clap);
			dialog.setup(this, DialogDailyMissionClear.eTargetPart.ParkMap);
			yield return StartCoroutine(dialogManager.openDialog(dialog));
			while (dialog.isOpen())
			{
				yield return 0;
			}
			if (dialog.toBonus)
			{
				transitBonusStage_ = true;
			}
			yield return null;
		}
	}

	public IEnumerator dailyMissionClearCheck()
	{
		if (!dailyMission_.bonusgamePlayed && dailyMission_.missionCleared)
		{
			yield return StartCoroutine(showDailyMissionCleared());
		}
	}

	public IEnumerator setBonusGamePlayed(int status)
	{
		isBonusGamePlayed_ = true;
		NetworkMng.Instance.setup(Hash.BonusStageStart(dailyMission_.dateKey));
		yield return StartCoroutine(NetworkMng.Instance.download(API.BonusStageStart, false, true));
		WWW www = NetworkMng.Instance.getWWW();
		BonusStartData data = JsonMapper.ToObject<BonusStartData>(www.text);
		GlobalData.Instance.setBonusStartData(data);
		Network.DailyMission missionData = GlobalData.Instance.getDailyMissionData();
		missionData.receiveFlg = ((status != 1) ? 3 : 4);
		if (status == 1)
		{
			NetworkMng.Instance.setup(Hash.B1(dailyMission_.dateKey, 0, status));
			yield return StartCoroutine(NetworkMng.Instance.download(API.B1, false, true));
		}
		dailyMissionValueSetting();
		if (dailyMission_.gameObject.activeSelf)
		{
			yield return StartCoroutine(dailyMission_.dailyMissionInfoSetup());
		}
		isBonusGamePlayed_ = false;
	}

	public IEnumerator sendInactive()
	{
		Debug.Log("sendInactive");
		while (isBonusGamePlayed_ || GlobalData.Instance.isResourceDownloading)
		{
			yield return null;
		}
		DialogJewelShop jewelShop = dialogManager.getDialog(DialogManager.eDialog.JewelShop) as DialogJewelShop;
		DialogLuckyChance luckyChance = dialogManager.getDialog(DialogManager.eDialog.LuckyChance) as DialogLuckyChance;
		DialogCoinShop coinShop = dialogManager.getDialog(DialogManager.eDialog.CoinShop) as DialogCoinShop;
		DialogHeartShop heartShop = dialogManager.getDialog(DialogManager.eDialog.HeartShop) as DialogHeartShop;
		DialogAllShop allShop = dialogManager.getDialog(DialogManager.eDialog.AllShop) as DialogAllShop;
		DialogInformation infoDialog = dialogManager.getDialog(DialogManager.eDialog.Information) as DialogInformation;
		if (infoDialog != null && infoDialog.bOping)
		{
			yield return 0;
		}
		while (partManager.isLineLogin())
		{
			yield return 0;
		}
		if ((jewelShop != null && jewelShop.isOpen()) || (coinShop != null && coinShop.isOpen()) || (heartShop != null && heartShop.isOpen()) || (luckyChance != null && luckyChance.isBuying()) || (allShop != null && allShop.isOpen()))
		{
			while (NetworkMng.Instance.isDownloading())
			{
				yield return 0;
			}
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			partManager.loginInputBack();
			isInactive_ = false;
			yield break;
		}
		isInactive_ = true;
		if (!wasInitialized_)
		{
			while (NetworkMng.Instance.isDownloading())
			{
				yield return 0;
			}
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			if (isInactive_)
			{
				partManager.loginInputBack();
				isInactive_ = false;
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
			if (isInactive_)
			{
				partManager.loginInputBack();
				isInactive_ = false;
			}
			yield break;
		}
		yield return FriendUpdater.Instance.requestUpdate(partManager);
		if (!isInactive_)
		{
			yield break;
		}
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(API.Inactive, true, false));
		if (!isInactive_)
		{
			yield break;
		}
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			partManager.loginInputBack();
			isInactive_ = false;
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		InactiveData data = JsonMapper.ToObject<InactiveData>(www.text);
		GameData gameData = GlobalData.Instance.getGameData();
		gameData.setCommonData(data, true);
		gameData.setEventData(data);
		gameData.setDailyMissionData(data);
		gameData.helpDataSize = data.helpDataSize;
		gameData.helpMove = data.helpMove;
		gameData.helpTime = data.helpTime;
		gameData.bonusChanceLv = data.bonusChanceLv;
		gameData.saleArea = data.saleArea;
		gameData.areaSalePercent = data.areaSalePercent;
		gameData.isAreaCampaign = data.isAreaCampaign;
		gameData.saleStageItemArea = data.saleStageItemArea;
		gameData.stageItemAreaSalePercent = data.stageItemAreaSalePercent;
		gameData.isStageItemAreaCampaign = data.isStageItemAreaCampaign;
		gameData.gachaTicket = data.gachaTicket;
		Debug.Log("gachaTicket = " + data.gachaTicket);
		EventMenu.updateGetTime();
		ChallengeMenu.updateGetTime();
		CollaborationMenu.updateGetTime();
		gameData.setParkData(null, null, data.thanksList, data.buildings, data.mapReleaseNum);
		SaveData.Instance.getParkData().UpdatePlacedData(data.buildings);
		gameData.niceList = new ParkNiceList();
		gameData.niceList.resultCode = data.resultCode;
		gameData.niceList.niceList = data.niceList;
		gameData.tookNiceTotalCount = data.tookNiceTotalCount;
		gameData.giveNiceMonthlyCount = data.giveNiceMonthlyCount;
		gameData.giveNiceTotalCount = data.giveNiceTotalCount;
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
				if (dailyMission_.gameObject.activeSelf)
				{
					yield return StartCoroutine(dailyMission_.dailyMissionInfoSetup());
				}
			}
		}
		else
		{
			dailyMission_.gameObject.SetActive(false);
		}
		if (mainMenu_ != null && mainMenu_.getHeartMenu() != null)
		{
			mainMenu_.update();
			mainMenu_.getHeartMenu().updateRemainingTime();
		}
		DialogSetupPark setup = dialogManager.getDialog(DialogManager.eDialog.ParkStageSetup) as DialogSetupPark;
		if (setup.isOpen())
		{
			setup.OnApplicationResumeSetupDialog();
		}
		if (isInactive_)
		{
			while (NetworkMng.Instance.isDownloading())
			{
				yield return 0;
			}
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			if (isInactive_)
			{
				partManager.loginInputBack();
				isInactive_ = false;
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

	private IEnumerator forceRecover()
	{
		StopCoroutine("sendInactive");
		isInactive_ = true;
		while (partManager.isLineLogin())
		{
			yield return null;
		}
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(API.Inactive, true, false));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			isInactive_ = false;
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
					if (dailyMission_ != null)
					{
						dailyMissionValueSetting();
						if (dailyMission_.gameObject.activeSelf)
						{
							yield return StartCoroutine(dailyMission_.dailyMissionInfoSetup());
						}
					}
				}
			}
		}
		yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		isInactive_ = false;
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
		if (isInactive_)
		{
			partManager.loginInputBack();
		}
		FriendUpdater.Instance.stop();
		isInactive_ = false;
	}

	public IEnumerator updatePlayerInfo()
	{
		yield return StartCoroutine(sendInactive());
		int newNiceCount = getNewNiceCount(GlobalData.Instance.getGameData().niceList, false);
		setNewNiceCount(newNiceCount);
		int resultUnopenedMailCount = 0;
		NetworkMng manager = NetworkMng.Instance;
		manager.setup(null);
		yield return StartCoroutine(manager.download(OnCreateMailListWWW, true));
		if (manager.getStatus() != NetworkMng.eStatus.Success)
		{
			yield break;
		}
		WWW www = manager.getWWW();
		Mail[] mails = JsonMapper.ToObject<MailList>(www.text).mailList;
		int responcedMailNum = mails.Length;
		for (int i = 0; i < responcedMailNum; i++)
		{
			if (!mails[i].isOpen)
			{
				resultUnopenedMailCount++;
			}
		}
		setMailCount(resultUnopenedMailCount);
		bottomMenuUI_.minilenMenu.UpdateCount();
	}
}
