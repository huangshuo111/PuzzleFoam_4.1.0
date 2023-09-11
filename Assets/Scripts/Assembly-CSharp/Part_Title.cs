using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Network;
using TapjoyUnity;
using TnkAd;
using Toast.Analytics;
using UnityEngine;

public class Part_Title : PartBase
{
	private enum eBubbleType
	{
		Red = 0,
		Blue = 1,
		Green = 2
	}

	private enum eNetworkState
	{
		Login = 0,
		NetworkResourceURL = 1,
		NetworkResource = 2
	}

	public class LoginCB
	{
		public int result_ = -1;

		public bool isPlayable_;

		public void InitCB()
		{
			result_ = -1;
			isPlayable_ = false;
		}

		public void OnLogin(int result, bool isPlayable)
		{
			Debug.Log("Login Result = " + result);
			result_ = result;
			isPlayable_ = isPlayable;
		}

		public bool IsSuccess()
		{
			return result_ == 0 && isPlayable_;
		}
	}

	private const int oneday = 86400;

	private const int splitMapLoadCount = 70;

	private const int splitLoginCount = 25;

	private const int splitResourceLoadCount = 28;

	private GlobalRoot globalObj_;

	private GameObject loading_;

	private static GameObject loginbutton_;

	private static GameObject guestloginbutton_;

	private int loadedItemNum_;

	private int loadItemMax_;

	private UILabel progressLabel_;

	private GlobalObjectParam.eObject[] globalObjs_ = new GlobalObjectParam.eObject[16]
	{
		GlobalObjectParam.eObject.MapMainUI,
		GlobalObjectParam.eObject.MapMainUI2,
		GlobalObjectParam.eObject.MapCamera,
		GlobalObjectParam.eObject.HeartEffect,
		GlobalObjectParam.eObject.Friend,
		GlobalObjectParam.eObject.Loading,
		GlobalObjectParam.eObject.HighScoreItem,
		GlobalObjectParam.eObject.Line,
		GlobalObjectParam.eObject.StageOpenEff,
		GlobalObjectParam.eObject.HighScoreDummyItem,
		GlobalObjectParam.eObject.HighScoreBorderItem,
		GlobalObjectParam.eObject.Cloud,
		GlobalObjectParam.eObject.Tips,
		GlobalObjectParam.eObject.Wave,
		GlobalObjectParam.eObject.ExpEffect,
		GlobalObjectParam.eObject.CoinEffect
	};

	private string[] staticObjNames_ = new string[1] { "Sound" };

	private bool bLoading_;

	private GameObject dataTable_;

	private LoadChara chara_;

	private LoadBubble bubble_;

	private ResponceHeaderData headerData_;

	private string serverAddress_;

	private bool encrypted_;

	private GameObject globalUIRoot_;

	public static bool bStartGame;

	private static bool bloginbuttonenable;

	private static bool bguestloginbuttonenable;

	private int loadParAhead_;

	private bool bFinishMove;

	private bool bResume;

	private bool bLineLoginFinished;

	private bool bMapcreated;

	private bool bResourceDownloaded;

	private bool bLoginFinished;

	public bool isEndLogin { get; private set; }

	public static bool bLoginBtnEnabled
	{
		get
		{
			return bloginbuttonenable;
		}
		set
		{
			if ((bool)loginbutton_)
			{
				loginbutton_.SetActive(value);
			}
			bloginbuttonenable = value;
		}
	}

	public static bool bGuestLoginButtonEnable
	{
		get
		{
			return bguestloginbuttonenable;
		}
		set
		{
			if ((bool)guestloginbutton_)
			{
				guestloginbutton_.SetActive(value);
			}
			bguestloginbuttonenable = value;
		}
	}

	public override IEnumerator setup(Hashtable args)
	{
		GKUnityPluginController.Instance.Init();
		SNSCore.Instance.CreateInstance();
		isEndLogin = false;
		SaveData saveData = SaveData.Instance;
		globalObj_ = GlobalRoot.Instance;
		dataTable_ = loadGlobalObj("Prefabs/", GlobalObjectParam.eObject.DataTable, false);
		StageIconDataTable iconData = dataTable_.GetComponent<StageIconDataTable>();
		iconData.load();
		GimmickDataTable gimmickData = dataTable_.GetComponent<GimmickDataTable>();
		gimmickData.load();
		saveData.load();
		MessageResource.Instance.load();
		Aes.Init();
		globalUIRoot_ = GameObject.Find("UI Root");
		yield return StartCoroutine(lowMemoryDialog());
		ResourceLoader.Instance.init(!saveData.getSystemData().getOptionData().getFlag(SaveOptionData.eFlag.HighQuality));
		GameObject title = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Title_Panel")) as GameObject;
		Utility.setParent(title, uiRoot.transform, false);
		title.transform.localPosition = Vector3.back * 100f;
		loading_ = title.transform.Find("loading").gameObject;
		loginbutton_ = title.transform.Find("LoginButton").gameObject;
		guestloginbutton_ = title.transform.Find("GuestLoginButton").gameObject;
		setupButton(title, true);
		bLoginBtnEnabled = false;
		bGuestLoginButtonEnable = false;
		bubble_ = loading_.GetComponent<LoadBubble>();
		progressLabel_ = loading_.transform.Find("loading_anm/progress_Label").GetComponent<UILabel>();
		chara_ = loading_.transform.Find("chara00").GetComponent<LoadChara>();
		setProgressText(0);
		yield return StartCoroutine(dialogManager.loadGlobalDialogs(true, globalUIRoot_, null, null));
		StartCoroutine(execute());
	}

	private IEnumerator ServerStausCheck()
	{
		yield return StartCoroutine(NetworkMng.Instance.download(API.ServerStatusCheck, false, false));
		WWW www = NetworkMng.Instance.getWWW();
		ServerStatusInfo info = JsonMapper.ToObject<ServerStatusInfo>(www.text);
		serverAddress_ = info.apiURL;
		encrypted_ = info.encrypted;
		SaveData.Instance.ServerVersion = info.latestVersion;
		SaveData.Instance.PresentFlag = info.PresentFlag;
		if (info.status > 1)
		{
			DialogCommon dialog = dialogManager.getDialog(DialogManager.eDialog.Maintenance) as DialogCommon;
			int status = info.status;
			if (status == 2)
			{
				MessageResource msgRes = MessageResource.Instance;
				dialog.setMessage(msgRes.AddLine(info.message));
			}
			dialog.sysLabelEnable(true);
			dialog.setup(null, NetworkMng.Instance.OnMaintenaceQuitApp, true);
			dialog.setButtonActive(DialogCommon.eBtn.Close, true);
			yield return StartCoroutine(dialogManager.openDialog(dialog));
			while (dialog.isOpen())
			{
				yield return null;
			}
		}
	}

	private IEnumerator login()
	{
		if (!Tapjoy.IsConnected)
		{
			Tapjoy.Connect();
		}
		GKUnityPluginController.Instance.getAdvertisingId();
		while (GKUnityPluginController.m_AdvertisingId == string.Empty)
		{
			yield return null;
		}
		Debug.Log("Recv AdvertisingId = " + GKUnityPluginController.m_AdvertisingId);
		Plugin.Instance.applicationStarted();
		Plugin.Instance.initInstance();
		Plugin.Instance.showVideoCloseButton(false);
		Plugin.Instance.prepareVideoAd("PB_Video_AD", "PB_TNKVidioHandler");
		Plugin.Instance.prepareInterstitialAd("PB_Video_AD", "PB_TNKVidioHandler");
		Vungle.init("57873ff8890aa8ad7b00003d", "578740343c87924209000036", string.Empty);
		Partytrack.start(Convert.ToInt32(ProjectSettings.Instance.Android_KR_PartyTrack_AppID), ProjectSettings.Instance.Android_KR_PartyTrack_AppSignature);
		int result = GameAnalytics.initializeSdk(ProjectSettings.Instance.ToastAppKey, ProjectSettings.Instance.ToastCompanyID, SaveData.Instance.getAppVersion(), true);
		GameAnalytics.setCampaignListener(GKToastListener.Instance);
		GameAnalytics.setDebugMode(false);
		if (result != 0)
		{
			Debug.LogError("GameAnalytics initializeSdk false");
		}
		GlobalGoogleAnalytics.Instance.StartSession();
		if (PlayerPrefs.GetInt("NewInstallCount") == 0)
		{
			Tapjoy.TrackEvent("Install", "New Install Count", "PlayStore", null, "PlayStore", 1L, null, 0L, null, 0L);
			GlobalGoogleAnalytics.Instance.LogEvent("New Install Count", "PlayStore", SaveData.Instance.getAppVersion().ToString(), 1L);
			PlayerPrefs.SetInt("NewInstallCount", 1);
			PlayerPrefs.Save();
		}
		if (!PlayerPrefs.GetString("UpdateVersion").Equals(SaveData.Instance.getAppVersion()))
		{
			Tapjoy.TrackEvent("Install", "Update Version Count", "PlayStore", SaveData.Instance.getAppVersion(), "PlayStore", 1L, SaveData.Instance.getAppVersion(), 1L, null, 0L);
			GlobalGoogleAnalytics.Instance.LogEvent("Update Version Count", "PlayStore", SaveData.Instance.getAppVersion().ToString(), 1L);
			PlayerPrefs.SetString("UpdateVersion", SaveData.Instance.getAppVersion());
			PlayerPrefs.Save();
		}
		NetworkMng.Instance.setup(Hash.Login(!SNSCore.IsAuthorize));
		yield return StartCoroutine(NetworkMng.Instance.download(API.Login, false, false));
		WWW www2 = NetworkMng.Instance.getWWW();
		LoginData response = JsonMapper.ToObject<LoginData>(www2.text);
		GlobalData.Instance.setGameData(response);
		GlobalData.Instance.getGameData().minilenParkTutorialStatus = response.minilenParkTutorialStatus;
		if (GlobalData.Instance.getGameData().TNKBanner)
		{
			Plugin.Instance.actionCompleted();
		}
		Plugin.Instance.setUserName(GlobalData.Instance.LineID.ToString());
		GlobalData.Instance.getGameData().iosInvite = true;
		if (GlobalData.Instance.getGameData().avatarList != null)
		{
			int[] rank_count = new int[4];
			Network.Avatar[] avatarList = GlobalData.Instance.getGameData().avatarList;
			foreach (Network.Avatar av in avatarList)
			{
				if (av.wearFlg == 1)
				{
					GlobalData.Instance.currentAvatar = av;
				}
				if (av.index >= 23000)
				{
					rank_count[3]++;
				}
				else if (av.index >= 22000)
				{
					rank_count[2]++;
				}
				else if (av.index >= 21000)
				{
					rank_count[1]++;
				}
				else
				{
					rank_count[0]++;
				}
			}
			GlobalData.Instance.avatarCount = rank_count;
		}
		headerData_ = NetworkUtility.createResponceHeaderData(www2);
		DummyPlayerData.Data.IsHeartRecvFlag = GlobalData.Instance.getGameData().heartRecvFlg;
		GlobalData.Instance.SetGKEventFlag();
		GKUnityPluginController.GK_Payment_Initialize(GlobalData.Instance.getGameData().aBillKey);
		partManager.SetMarketViewFlag(GlobalData.Instance.getGameData().market_review);
		GameAnalytics.setUserId(GlobalData.Instance.LineID.ToString(), true);
		GameAnalytics.traceActivation();
		EventMenu.updateGetTime();
		ChallengeMenu.updateGetTime();
		CollaborationMenu.updateGetTime();
		DailyMission.updateGetTime();
		if (DailyMission.isTermClear())
		{
			Mission respons_mission2 = JsonMapper.ToObject<Mission>(www2.text);
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
		www2.Dispose();
		www2 = null;
		if (response.keyBubble != null)
		{
			KeyBubbleData keyData = GlobalData.Instance.getKeyBubbleData();
			keyData.keyBubbleCount = response.keyBubble.keyBubbleCount;
			keyData.keyBubbleMax = response.keyBubble.keyBubbleMax;
			GlobalData.Instance.setKeyBubbleData(keyData);
		}
	}

	private IEnumerator execute()
	{
		while (partManager.isTransitioning())
		{
			yield return 0;
		}
		bLoading_ = true;
		dataTable_.GetComponent<RewardDataTable>().load();
		dataTable_.GetComponent<RankingDataTable>().load();
		dataTable_.GetComponent<RouletteDataTable>().load();
		dataTable_.GetComponent<InvitationDataTable>().load();
		dataTable_.GetComponent<ComboBonusDataTable>().load();
		dataTable_.GetComponent<DummyFriendDataTable>().load();
		dataTable_.GetComponent<AvatarSkillDataTable>().load();
		TutorialDataTable tutorialTbl = dataTable_.GetComponent<TutorialDataTable>();
		tutorialTbl.load();
		loadItemMax_ = globalObjs_.Length;
		loadItemMax_ += DialogManager.GlobalDialogMax;
		loadItemMax_ += staticObjNames_.Length;
		loadItemMax_ += 70;
		loadItemMax_++;
		loadItemMax_++;
		loadItemMax_++;
		loadItemMax_ += 25;
		loadItemMax_ += 28;
		StartCoroutine(progressUpdate());
		SaveOptionData optionData = SaveData.Instance.getSystemData().getOptionData();
		StageDataTable stageTbl = dataTable_.GetComponent<StageDataTable>();
		SaveNetworkData netData = SaveData.Instance.getGameData().getNetworkData();
		if (PlayerPrefs.GetInt("PolicyFlagSkonec") == 0)
		{
			DialogPolicy policydialog = dialogManager.getDialog(DialogManager.eDialog.Policy) as DialogPolicy;
			yield return StartCoroutine(policydialog.show());
			while (policydialog.isOpen())
			{
				yield return null;
			}
			PlayerPrefs.SetInt("PolicyFlagSkonec", 1);
			PlayerPrefs.Save();
		}
		Input.resetCount();
		bool bKorean = optionData.isKorean();
		Hashtable header = new Hashtable();
		header[NetworkUtility.ResponceHeaderKeys.MemberNo] = 3000000000000000000L;
		header[NetworkUtility.ResponceHeaderKeys.AppVersion] = SaveData.Instance.getAppVersion();
		header[NetworkUtility.ResponceHeaderKeys.DeviceInfo] = SystemInfo.deviceModel + "/Android " + SystemInfo.operatingSystem.Split(' ')[2];
		Debug.Log(string.Concat(ProjectSettings.Instance.ConnectServer_KR, " Ver. ", SaveData.Instance.getAppVersion()));
		WWWWrap.init(ProjectSettings.Instance.ConnectServerIP, header, ProjectSettings.Instance.IsEncrypt);
		GameObject connectIcon = loadGlobalObj("Prefabs/", GlobalObjectParam.eObject.Connecting, false);
		NetworkMng.Instance.init(connectIcon.GetComponent<NetworkIcon>());
		yield return StartCoroutine(ServerStausCheck());
		Tapjoy.TrackEvent("Social Connect", "Start", 0L);
		yield return StartCoroutine(lineLogin());
		Input.resetCount();
		progressLabel_.gameObject.SetActive(true);
		if (!ProjectSettings.Instance.IsLive)
		{
			progressLabel_.color = Color.blue;
		}
		loadedItemNum_++;
		header[NetworkUtility.ResponceHeaderKeys.MemberNo] = GlobalData.Instance.LineID;
		bool bJapan = optionData.isKorean();
		header[NetworkUtility.ResponceHeaderKeys.Region] = ((!bJapan) ? "EN" : "JA");
		WWWWrap.init(serverAddress_, header, encrypted_);
		NetworkMng.Instance.forceIconDisable(true);
		StartCoroutine(loginLoadedItemNumCountUp(25));
		yield return StartCoroutine(login());
		bLoginFinished = true;
		Tapjoy.TrackEvent("Social Connect", "End", 0L);
		Tapjoy.SetUserID(GlobalData.Instance.LineID.ToString());
		GameData gameData = GlobalData.Instance.getGameData();
		Tapjoy.SetUserLevel(gameData.progressStageNo);
		float startLoadingTime = Time.realtimeSinceStartup;
		Tapjoy.TrackEvent("Loading", "Start", 0L);
		yield return partManager.StartCoroutine("RegisterDeviceToken");
		loadedItemNum_++;
		stageTbl.gameObject.SetActive(true);
		do
		{
			yield return StartCoroutine(stageTbl.AssetBundleDownLoad());
		}
		while (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Retry);
		NetworkMng.Instance.statusChangeSuccess();
		yield return StartCoroutine(createStaticObject());
		try
		{
			Sound sound_ = Sound.Instance;
			sound_.setBgmMasterVolume((!optionData.getFlag(SaveOptionData.eFlag.BGM)) ? 0f : sound_.getDefaultBgmVolume());
			sound_.setSeMasterVolume((!optionData.getFlag(SaveOptionData.eFlag.SE)) ? 0f : sound_.getDefaultSeVolume());
		}
		catch (Exception e)
		{
			Debug.Log(e.Message);
		}
		StartCoroutine(resourceLoadedItemNumCountUp(28));
		yield return StartCoroutine(stageTbl.download(headerData_, false, false));
		bResourceDownloaded = true;
		Tapjoy.TrackEvent("DLC", "DownloadScript-Start", 0L);
		float dlcloadingtime = Time.realtimeSinceStartup;
		if (stageTbl.getStageData() == null)
		{
			stageTbl.loadStageData();
		}
		if (netData.getEventNo() != 0 && stageTbl.getEventData() == null)
		{
			stageTbl.loadEventData();
		}
		if (stageTbl.getBossData() == null)
		{
			stageTbl.loadBossData();
		}
		Tapjoy.TrackEvent("DLC", "DownloadScript-End", ((long)(Time.realtimeSinceStartup - dlcloadingtime)).ToString(), null, 0L);
		Debug.Log("DLC DownloadScript-End - " + (long)(Time.realtimeSinceStartup - dlcloadingtime));
		Tapjoy.TrackEvent("DLC", "DownloadGameResources-Start", 0L);
		dlcloadingtime = Time.realtimeSinceStartup;
		GlobalData.Instance.ignoreLodingIcon = true;
		int downloadResourceCount = 17;
		int BgCount = 6;
		int downloadCount = downloadResourceCount + BgCount;
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 0, false, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 1, false, 1, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 2, false, 2, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 3, false, 3, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 4, false, 4, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 5, false, 5, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 6, false, 6, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 7, false, 7, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 8, false, 8, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 9, false, 9, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.WorldMap, 0, false, 10, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.WorldMap, 1, false, 11, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.WorldMap, 2, false, 12, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.WorldMap, 3, false, 13, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.WorldMap, 4, false, 14, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.WorldMap, 5, false, 15, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.ThrowChara, 16, false, 16, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.SupportChara, 20, false, 17, downloadCount));
		List<int> bgList = new List<int>(BgCount);
		for (int i = 0; i < BgCount; i++)
		{
			int bg = stageTbl.getInfo(i).Common.Bg;
			if (!bgList.Contains(bg))
			{
				bgList.Add(bg);
			}
		}
		for (int j = 0; j < bgList.Count; j++)
		{
			yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.BG, 0 + (bgList[j] - 1), j == bgList.Count - 1, j + downloadResourceCount, downloadCount));
		}
		GlobalData.Instance.ignoreLodingIcon = false;
		yield return StartCoroutine(TalkMessage.Instance.updateRoutine());
		Tapjoy.TrackEvent("DLC", "DownloadGameResources-End", ((long)(Time.realtimeSinceStartup - dlcloadingtime)).ToString(), null, 0L);
		Debug.Log("DownloadGameResources-End : " + (long)(Time.time - dlcloadingtime));
		Debug.Log("GlobalData.Instance.getGameData().firstLogin = " + GlobalData.Instance.getGameData().firstLogin);
		Debug.Log("SNSCore.IsAuthorize = " + SNSCore.IsAuthorize);
		if (GlobalData.Instance.getGameData().firstLogin && SNSCore.IsAuthorize)
		{
			DateTime date = DateTime.Now;
			Debug.Log("date.Hour = " + date.Hour);
			Debug.Log("date.Minute = " + date.Minute);
			Debug.Log("date.Second = " + date.Second);
			int FirstLoginTime = date.Hour * 60 * 60 + date.Minute * 60 + date.Second;
			PlayerPrefs.SetInt("FirstLoginTime", FirstLoginTime);
		}
		if (GlobalData.Instance.getGameData().firstLogin && SNSCore.IsAuthorize)
		{
			NotificationManager.Instance.schedule(86400, 3732, 1, 3, 0);
			NotificationManager.Instance.schedule(86400, 3733, 2, 7, 0);
			NotificationManager.Instance.schedule(86400, 3734, 3, 14, 0);
			NotificationManager.Instance.schedule(86400, 3735, 4, 30, 0);
			NotificationManager.Instance.schedule(86400, 3735, 1001, 45, 0);
			NotificationManager.Instance.schedule(86400, 3735, 1002, 60, 0);
			NotificationManager.Instance.schedule(86400, 3735, 1003, 75, 0);
			NotificationManager.Instance.schedule(86400, 3735, 1004, 90, 0);
		}
		for (int k = 0; k < 7; k++)
		{
			if (GlobalData.Instance.getGameData().beginnerCampaignPushAlert[k] > 0)
			{
				int beginnerLoginTime = GlobalData.Instance.getGameData().beginnerCampaignPushAlert[k];
				Debug.Log(" i : " + k + "  time : " + beginnerLoginTime);
				NotificationManager.Instance.schedule(1, 3741 + k, 6 + k, beginnerLoginTime, 0);
			}
		}
		if (GlobalData.Instance.getGameData().beginnerCampaignLastDay > 0)
		{
			DateTime date2 = DateTime.Now;
			int beginnerLoginTime2 = GlobalData.Instance.getGameData().beginnerCampaignLastDay;
			PlayerPrefs.SetInt("BeginnerLoginTime", beginnerLoginTime2);
			NotificationManager.Instance.schedule(1, 3740, 5, beginnerLoginTime2, 0);
		}
		DialogInformation infoDialog = dialogManager.getDialog(DialogManager.eDialog.Information) as DialogInformation;
		InformationData[] informationList = GlobalData.Instance.getGameData().informationList;
		foreach (InformationData info in informationList)
		{
			int id = info.informationId;
			int shopId = info.shopId;
			SaveInformationData data = SaveData.Instance.getGameData().getOtherData().getInfoData(id);
			if (data.isShow())
			{
				string url = info.informationUrl;
				if (!(url == string.Empty))
				{
					infoDialog.preLoad(url, id, shopId);
				}
			}
		}
		TutorialManager.Instance.init(tutorialTbl.getData(), loadGlobalObj("Prefabs/", GlobalObjectParam.eObject.Tutorial, false));
		for (int m = 0; m < globalObjs_.Length; m++)
		{
			GlobalObjectParam.eObject type = globalObjs_[m];
			GameObject obj = loadGlobalObj("Prefabs/", type, true);
			switch (type)
			{
			case GlobalObjectParam.eObject.MapMainUI:
				NGUIUtilScalableUIRoot.OffsetUI(obj.transform.Find("all"), true);
				break;
			case GlobalObjectParam.eObject.MapMainUI2:
				NGUIUtilScalableUIRoot.OffsetUI(obj.transform.Find("bag_menu"), false);
				NGUIUtilScalableUIRoot.OffsetUI(obj.transform.Find("event"), true);
				NGUIUtilScalableUIRoot.OffsetUI(obj.transform.Find("cross"), true);
				NGUIUtilScalableUIRoot.OffsetUI(obj.transform.Find("ad"), true);
				NGUIUtilScalableUIRoot.OffsetUI(obj.transform.Find("Offerwall"), true);
				NGUIUtilScalableUIRoot.OffsetUI(obj.transform.Find("boss"), true);
				NGUIUtilScalableUIRoot.OffsetUI(obj.transform.Find("collaboration"), true);
				NGUIUtilScalableUIRoot.OffsetUI(obj.transform.Find("minilen"), true);
				NGUIUtilScalableUIRoot.OffsetUI(obj.transform.Find("park_stage"), true);
				NGUIUtilScalableUIRoot.OffsetUI(obj.transform.Find("daily"), false);
				NGUIUtilScalableUIRoot.OffsetUI(obj.transform.Find("sendMap_Event"), false);
				NGUIUtilScalableUIRoot.OffsetUI(obj.transform.Find("sendOneStage"), false);
				NGUIUtilScalableUIRoot.OffsetUI(obj.transform.Find("gacha"), false);
				GlobalData.Instance.mapJumpDefaultPos = obj.transform.Find("sendOneStage").localPosition;
				break;
			case GlobalObjectParam.eObject.MapCamera:
				obj.GetComponentsInChildren<MapCamera>(true)[0].init(GameObject.Find("Camera").GetComponent<Camera>());
				break;
			}
			yield return 0;
		}
		isEndLogin = true;
		partManager.startTips();
		yield return StartCoroutine(dialogManager.loadGlobalDialogs(false, globalUIRoot_, null, OnGlobalDialogFinish));
		GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>().init();
		DialogPlayScore dialog = dialogManager.getDialog(DialogManager.eDialog.PlayScore) as DialogPlayScore;
		dialog.init(globalObj_.getObject(GlobalObjectParam.eObject.HighScoreItem), globalObj_.getObject(GlobalObjectParam.eObject.HighScoreDummyItem), globalObj_.getObject(GlobalObjectParam.eObject.HighScoreBorderItem));
		DialogAppQuit quitdialog = (DialogAppQuit)dialogManager.getDialog(DialogManager.eDialog.AppQuit);
		while (quitdialog != null && quitdialog.isOpen())
		{
			yield return 0;
		}
		yield return StartCoroutine(partManager.SendDeviceToken(this));
		Input.enable = false;
		StartCoroutine(mapLoadedItemNumCountUp(70));
		yield return StartCoroutine(createMapPart());
		bMapcreated = true;
		while (!bFinishMove)
		{
			yield return 0;
		}
		Debug.Log("<color=red>Time = " + Time.time + "</color>");
		Input.enable = true;
		NetworkMng.Instance.forceIconDisable(false);
		partManager.stopTips();
		BossDataTable bossDataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<BossDataTable>();
		if (bossDataTable != null)
		{
			yield return dialogManager.StartCoroutine(bossDataTable.download(false, false));
		}
		bLoading_ = false;
		bStartGame = true;
		partManager.requestTransition(PartManager.ePart.Map, null, false);
		Tapjoy.TrackEvent("Loading", "End", ((long)(Time.realtimeSinceStartup - startLoadingTime)).ToString(), null, 0L);
	}

	private void OnGlobalDialogFinish(DialogManager.eDialog type)
	{
		loadedItemNum_++;
	}

	private IEnumerator progressUpdate()
	{
		int bubbleIdx = 1;
		int section = 0;
		float loadItemMax = loadItemMax_;
		float average = loadItemMax / 9f;
		StartCoroutine(lateUpdateParDisp());
		int prevPercent = 0;
		while (bLoading_)
		{
			float loadedItemNum = loadedItemNum_;
			float percent = loadedItemNum / loadItemMax;
			int dispPercent = (loadParAhead_ = (int)(percent * 100f));
			if (dispPercent >= prevPercent && bubbleIdx <= 10)
			{
				chara_.calcMoveValue(bubble_.getBubble(bubbleIdx - 1).transform, dispPercent - prevPercent);
				prevPercent = dispPercent;
				chara_.move(percent);
			}
			if (dispPercent >= 10 && dispPercent >= bubbleIdx * 10)
			{
				StartCoroutine(lateBreakBubble(bubbleIdx - 1));
				bubbleIdx++;
			}
			yield return 0;
		}
		loadParAhead_ = 100;
		yield return 0;
		loading_.SetActive(false);
	}

	private IEnumerator lateUpdateParDisp()
	{
		int par = 0;
		while (par != 100)
		{
			if (par < loadParAhead_)
			{
				par++;
				setProgressText(par);
			}
			yield return 0;
		}
	}

	private IEnumerator lateBreakBubble(int index)
	{
		yield return StartCoroutine(chara_.waitMove());
		bubble_.burst(index);
		if (index == 9)
		{
			bFinishMove = true;
		}
	}

	private void setProgressText(int percent)
	{
		MessageResource instance = MessageResource.Instance;
		string message = instance.getMessage(24);
		message = instance.castCtrlCode(message, 1, percent.ToString());
		progressLabel_.text = message;
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

	private IEnumerator createStaticObject()
	{
		for (int i = 0; i < staticObjNames_.Length; i++)
		{
			UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", staticObjNames_[i]));
			loadedItemNum_++;
			yield return 0;
		}
	}

	private GameObject loadGlobalObj(string path, GlobalObjectParam.eObject objType, bool bCount)
	{
		GameObject gameObject = globalObj_.load(path, objType, false);
		gameObject.SetActive(false);
		if (bCount)
		{
			loadedItemNum_++;
		}
		return gameObject;
	}

	private IEnumerator lineLogin()
	{
		yield return StartCoroutine(partManager.hspLogin(false, false));
		if (SNSCore.IsAuthorize)
		{
			DummyPlayerData.result_ = -1;
			DummyPlayerData cb = new DummyPlayerData();
			SNSCore.Instance.LoadLocalUser(cb.OnMyProfileLoad);
			while (DummyPlayerData.result_ == -1)
			{
				yield return null;
			}
			long newID = Convert.ToInt64(SNSCore.local_UserData_.id);
			GlobalData.Instance.LineID = newID;
		}
		else
		{
			GlobalData.Instance.LineID = 3000000000000000000L;
			DummyPlayerData.Data.UserName = "Player";
		}
	}

	private IEnumerator gestsLoginMapping()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer && bStartGame && !SNSCore.IsAuthorize)
		{
		}
		yield break;
	}

	public void OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "LoginButton":
			bLoginBtnEnabled = false;
			break;
		case "GuestLoginButton":
			Debug.Log("GuestLoginButton");
			bGuestLoginButtonEnable = false;
			break;
		}
	}

	private IEnumerator createMapPart()
	{
		string part_name = "Part_Map";
		GameObject part_obj = new GameObject(part_name);
		PartBase newPart = (PartBase)UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(part_obj, "Assets/Scripts/Assembly-CSharp/Part_Title.cs (774,32)", part_name);
		newPart.init(partManager, dialogManager);
		yield return StartCoroutine(newPart.setup(null));
		partManager.preCreatePart = newPart;
		loadedItemNum_++;
		yield return null;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus)
		{
			bResume = true;
		}
	}

	private IEnumerator mapLoadedItemNumCountUp(int repeatCount)
	{
		int i;
		for (i = 0; i < repeatCount; i++)
		{
			if (bMapcreated)
			{
				break;
			}
			float time2 = 0f;
			while (time2 < 0.15f)
			{
				time2 += Time.deltaTime;
				yield return null;
			}
			loadedItemNum_++;
		}
		for (; i < repeatCount; i += 2)
		{
			float time = 0f;
			while (time < 0.006f)
			{
				time += Time.deltaTime;
				yield return null;
			}
			loadedItemNum_ += 2;
		}
		loadedItemNum_ += repeatCount - i;
	}

	private IEnumerator loginLoadedItemNumCountUp(int repeatCount)
	{
		int i;
		for (i = 0; i < repeatCount; i++)
		{
			if (bLoginFinished)
			{
				break;
			}
			float time2 = 0f;
			while (time2 < 0.3f)
			{
				time2 += Time.deltaTime;
				yield return null;
			}
			loadedItemNum_++;
		}
		for (; i < repeatCount; i++)
		{
			float time = 0f;
			while (time < 0.1f)
			{
				time += Time.deltaTime;
				yield return null;
			}
			loadedItemNum_++;
		}
		loadedItemNum_ += repeatCount - i;
	}

	private IEnumerator resourceLoadedItemNumCountUp(int repeatCount)
	{
		int i;
		for (i = 0; i < repeatCount; i++)
		{
			if (bResourceDownloaded)
			{
				break;
			}
			float time2 = 0f;
			while (time2 < 0.2f)
			{
				time2 += Time.deltaTime;
				yield return null;
			}
			loadedItemNum_++;
		}
		for (; i < repeatCount; i++)
		{
			float time = 0f;
			while (time < 0.05f)
			{
				time += Time.deltaTime;
				yield return null;
			}
			loadedItemNum_++;
		}
		loadedItemNum_ += repeatCount - i;
	}

	private void DeleteLocalSaveData()
	{
		PlayerPrefs.DeleteKey("boss_powerUp_tutorial_Play");
		PlayerPrefs.DeleteKey(Aes.EncryptString("SendDate", Aes.eEncodeType.Percent));
		PlayerPrefs.DeleteKey(Aes.EncryptString(Constant.PreReviewKey, Aes.eEncodeType.Percent));
		for (int i = 0; i < Constant.InformationSaveMax; i++)
		{
			PlayerPrefs.DeleteKey(SaveKeys.getInformationDataIDKey(i));
			PlayerPrefs.DeleteKey(SaveKeys.getInformationDataDateKey(i));
		}
		for (int j = 0; j < DummyPlayFriendData.FriendNum; j++)
		{
			UserData userData = null;
			userData = DummyPlayFriendData.DummyFriends[j];
			PlayerPrefs.DeleteKey(Aes.EncryptString("HelpSend" + userData.ID, Aes.eEncodeType.Percent));
			PlayerPrefs.DeleteKey(Aes.EncryptString("HelpNoSend" + userData.ID, Aes.eEncodeType.Percent));
			PlayerPrefs.DeleteKey(Aes.EncryptString("HeartSend" + userData.ID, Aes.eEncodeType.Percent));
			PlayerPrefs.DeleteKey(Aes.EncryptString("HeartRequest" + userData.ID, Aes.eEncodeType.Percent));
		}
		for (int k = 0; k <= 100; k++)
		{
			if (PlayerPrefs.HasKey("ShowHowToPlay" + k.ToString("000")))
			{
				PlayerPrefs.DeleteKey("ShowHowToPlay" + k.ToString("000"));
			}
		}
		for (int l = 400; l <= 500; l++)
		{
			if (PlayerPrefs.HasKey("ShowHowToPlay" + l.ToString("000")))
			{
				PlayerPrefs.DeleteKey("ShowHowToPlay" + l.ToString("000"));
			}
		}
		for (int m = 600; m <= 650; m++)
		{
			if (PlayerPrefs.HasKey("ShowHowToPlay" + m.ToString("000")))
			{
				PlayerPrefs.DeleteKey("ShowHowToPlay" + m.ToString("000"));
			}
		}
		PlayerPrefs.DeleteKey("FirstLoginTime");
		PlayerPrefs.DeleteKey("HeartNotificationReserveTime");
		PlayerPrefs.DeleteKey("HeartSetTime");
		Network.Avatar[] avatarList = GlobalData.Instance.getGameData().avatarList;
		foreach (Network.Avatar avatar in avatarList)
		{
			PlayerPrefs.DeleteKey(avatar.index + "_isNew");
		}
	}

	private IEnumerator lowMemoryDialog()
	{
		if (!SaveData.Instance.getSystemData().getOptionData().getFlag(SaveOptionData.eFlag.HighQuality) || PlayerPrefs.GetInt("SetHighQuality", 0) > 0)
		{
			yield break;
		}
		int limitSize2 = -1;
		limitSize2 = 600;
		if (limitSize2 < 0 || SystemInfo.systemMemorySize > limitSize2)
		{
			yield break;
		}
		int osVersion = -1;
		string versionString = SystemInfo.operatingSystem.Replace("Android OS ", string.Empty);
		int.TryParse(versionString.Substring(0, 1), out osVersion);
		if (osVersion < 4)
		{
			GameObject obj = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "CommonDialog_Panel")) as GameObject;
			DialogLowMemory dialog = obj.AddComponent<DialogLowMemory>();
			Utility.setParent(obj, globalUIRoot_.transform, true);
			dialog.init(partManager.dialogManager, partManager, partManager.fade, DialogManager.eDialog.LowMemory);
			dialog.OnCreate();
			NGUIUtility.setupButton(obj, obj, true);
			yield return StartCoroutine(dialog.open());
			Input.enable = true;
			while (dialog.isOpen())
			{
				yield return null;
			}
			Input.enable = false;
			UnityEngine.Object.Destroy(obj);
			SaveData.Instance.getSystemData().getOptionData().setFlag(SaveOptionData.eFlag.HighQuality, false);
			SaveData.Instance.getSystemData().getOptionData().save();
			PlayerPrefs.Save();
		}
	}
}
