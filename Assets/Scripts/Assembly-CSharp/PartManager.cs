using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class PartManager : MonoBehaviour
{
	public enum ePart
	{
		Title = 0,
		Map = 1,
		Stage = 2,
		Scenario = 3,
		EventMap = 4,
		ChallengeMap = 5,
		BonusStage = 6,
		BossStage = 7,
		RankingStage = 8,
		CollaborationMap = 9,
		ModeSelect = 10,
		RankingMap = 11,
		Park = 12
	}

	public enum eConnectStatus
	{
		None = 0,
		Load = 1,
		Cancel = 2,
		Success = 3
	}

	[SerializeField]
	private ePart startPart;

	public PartBase preCreatePart;

	public Transform uiParent;

	public DialogManager dialogManager;

	[SerializeField]
	public FadeMng fade;

	[SerializeField]
	private FadeMng.eType DefaultFadeType;

	[HideInInspector]
	public int notInputBackCount;

	public bool isNologinCancel;

	private bool bSuspend;

	private bool bGotoTitle_;

	private bool bKakaoLeave;

	private bool bLineLogin_;

	private Sound.eBgm replayBgmNum = Sound.eBgm.Max;

	private float replayBgmTime;

	private bool replayBgmLoop;

	public bool bTransitionMap_;

	public LoginListener loginListener = new LoginListener();

	private DateTime suspendDateTime_;

	public bool isStageResultDownloading;

	public GameObject UIRoot;

	private Part_Title.LoginCB loginCB;

	private eConnectStatus connectStatus_;

	private bool isguestenter;

	private bool marketreview_flag_;

	private bool devicetoken_breakflag;

	private string devicetoken;

	private bool isTipsInhibit;

	public PartBase execPart { get; private set; }

	public ePart currentPart { get; private set; }

	public ePart prevPart { get; private set; }

	public void loginInputBack()
	{
		while (notInputBackCount > 0)
		{
			Debug.Log("notInputBackCount = " + notInputBackCount);
			Input.enable = true;
			notInputBackCount--;
		}
	}

	private void Start()
	{
		suspendDateTime_ = DateTime.Now;
		NotificationManager.Instance.cancel(0);
		Application.targetFrameRate = 60;
		ResourceLoader.Instance.loadList();
		Screen.sleepTimeout = -1;
		Hashtable args = new Hashtable();
		StartCoroutine(transition(startPart, args, DefaultFadeType, false));
	}

	private void Update()
	{
	}

	public void requestTransition(ePart nextPart, Hashtable args)
	{
		requestTransition(nextPart, args, DefaultFadeType, false);
	}

	public void requestTransition(ePart nextPart, Hashtable args, bool bLoadIcon)
	{
		requestTransition(nextPart, args, DefaultFadeType, bLoadIcon);
	}

	public void requestTransition(ePart nextPart, Hashtable args, FadeMng.eType fadeType, bool bLoadIcon)
	{
		if (!(execPart == null))
		{
			StartCoroutine(transition(nextPart, args, fadeType, bLoadIcon));
		}
	}

	private IEnumerator transition(ePart nextPart, Hashtable args, FadeMng.eType fadeType, bool bLoadIcon)
	{
		while (isLineLogin())
		{
			yield return null;
		}
		if (NetworkMng.IsInstance())
		{
			while (NetworkMng.Instance.isDownloading())
			{
				yield return null;
			}
		}
		PartBase old_part2 = execPart;
		execPart = null;
		if (Sound.Instance != null)
		{
			Sound.Instance.stopBgm();
		}
		Debug.Log("transition Input.enable = false");
		Input.enable = false;
		if (old_part2 != null)
		{
			fade.setActive(fadeType, true);
			yield return StartCoroutine(fade.startFadeOut(fadeType));
			WWW NoticeWWW = new WWW(WebView.Instance.noticeurl + "/check_exist" + WebView.Instance.AddParameter(WebView.Instance.helpparam));
			yield return StartCoroutine(WebView.NoticeRequest(NoticeWWW));
			if (WebView.getResponseCode(NoticeWWW) == 200)
			{
				UnityEngine.Debug.Log(" if (WebView.getResponseCode(NoticeWWW) == 200) ");
				if (currentPart == ePart.Title)
				{
					yield return StartCoroutine(WebView.Instance.show(WebView.eWebType.Notice, dialogManager));
				}
			}
		}
		yield return null;
		GameObject loading = null;
		if (bLoadIcon)
		{
			loading = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.Loading);
			loading.SetActive(true);
			loading.transform.Find("bg").gameObject.SetActive(fadeType != FadeMng.eType.MapChange);
			if (isDispTips(nextPart))
			{
				startTips();
			}
		}
		if (old_part2 != null)
		{
			if (currentPart != 0)
			{
				dialogManager.releaseCurrentDialog();
			}
			if (nextPart == ePart.Stage || nextPart == ePart.BossStage || nextPart == ePart.BonusStage || nextPart == ePart.RankingStage)
			{
				GlobalRoot glbObj_2 = GlobalRoot.Instance;
				glbObj_2.unload(GlobalObjectParam.eObject.Cloud);
				glbObj_2.unload(GlobalObjectParam.eObject.Wave);
			}
			if (currentPart == ePart.Stage || currentPart == ePart.BossStage || currentPart == ePart.BonusStage || currentPart == ePart.RankingStage)
			{
				GlobalRoot glbObj_ = GlobalRoot.Instance;
				glbObj_.load("Prefabs/", GlobalObjectParam.eObject.Cloud, false).SetActive(false);
				glbObj_.load("Prefabs/", GlobalObjectParam.eObject.Wave, false).SetActive(false);
			}
			yield return StartCoroutine(old_part2.OnDestroyCB());
			UnityEngine.Object.Destroy(old_part2.gameObject);
			old_part2 = null;
			yield return null;
		}
		yield return Resources.UnloadUnusedAssets();
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();
		prevPart = currentPart;
		currentPart = nextPart;
		PartBase newPart;
		if (preCreatePart != null)
		{
			newPart = preCreatePart;
			preCreatePart = null;
		}
		else
		{
			string part_name = "Part_" + nextPart;
			GameObject part_obj = new GameObject(part_name);
			newPart = (PartBase)UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(part_obj, "Assets/Scripts/Assembly-CSharp/PartManager.cs (228,24)", part_name);
			List<DialogManager.eDialog> dialogList = dialogManager.getReserveDialog();
			foreach (DialogManager.eDialog dialog in dialogList)
			{
				yield return dialogManager.StartCoroutine(dialogManager.closeDialog(dialog));
			}
			dialogManager.clearReserveList();
			dialogManager.clearActiveDialogList();
			newPart.init(this, dialogManager);
			yield return StartCoroutine(newPart.setup(args));
		}
		yield return null;
		if (bLoadIcon)
		{
			stopTips();
			loading.SetActive(false);
		}
		fade.setActive(fadeType, true);
		yield return StartCoroutine(fade.startFadeIn(fadeType));
		fade.setActive(fadeType, false);
		execPart = newPart;
		Debug.Log("transition Input.enable = true");
		Input.enable = true;
	}

	public bool isTransitioning()
	{
		return execPart == null;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		bSuspend = pauseStatus;
		replayBgm(pauseStatus);
		if (!pauseStatus)
		{
			if (NotificationManager.Instance != null)
			{
				NotificationManager.Instance.delete();
			}
			TimeSpan timeSpan = DateTime.Now - suspendDateTime_;
			if (currentPart != 0 && timeSpan.TotalHours >= 1.0)
			{
				bGotoTitle_ = true;
			}
			else if (loginCB == null && currentPart != 0 && SNSCore.IsAuthorize && !bTransitionMap_)
			{
				StartCoroutine(hspLogin(true, false));
			}
		}
		else
		{
			suspendDateTime_ = DateTime.Now;
		}
	}

	private void LateUpdate()
	{
		if (bGotoTitle_)
		{
			bGotoTitle_ = false;
			GlobalData.Instance.LineID = 0L;
			Part_Title.bStartGame = false;
			gotoTitle();
		}
	}

	public eConnectStatus getConnectStatus()
	{
		return connectStatus_;
	}

	public IEnumerator hspLogin(bool bShowConnectIcon, bool bCancel)
	{
		if (bLineLogin_)
		{
			yield break;
		}
		while (isStageResultDownloading)
		{
			yield return 0;
		}
		GameObject dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		StageDataTable stageTbl = dataTbl.GetComponent<StageDataTable>();
		while (stageTbl.bGameResourceDownload || GlobalData.Instance.isResourceDownloading)
		{
			yield return 0;
		}
		Debug.Log("login_222");
		bLineLogin_ = true;
		DialogCommon d = dialogManager.getDialog(DialogManager.eDialog.NetworkError) as DialogCommon;
		if (d.isOpen())
		{
			yield return 0;
		}
		while (NetworkMng.Instance.isDownloading())
		{
			yield return 0;
		}
		Debug.Log("login_333");
		GameObject loading = null;
		if (GlobalRoot.IsInstance())
		{
			loading = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.Loading);
		}
		if (loading != null)
		{
			while (loading.activeSelf && !GlobalData.Instance.ignoreLodingIcon)
			{
				yield return 0;
			}
		}
		Debug.Log("login_444");
		DialogBase dialog2 = dialogManager.getDialog(DialogManager.eDialog.AllShop);
		if (dialog2 != null)
		{
			DialogAllShop shopDialog = dialog2 as DialogAllShop;
			while (shopDialog.isBuying())
			{
				yield return 0;
			}
		}
		DialogBase dialog_ = dialogManager.getDialog(DialogManager.eDialog.JewelShop);
		if (dialog2 != null)
		{
			DialogJewelShop jewel_dialog = dialog_ as DialogJewelShop;
			while (jewel_dialog.isBuying())
			{
				yield return 0;
			}
		}
		dialog2 = dialogManager.getDialog(DialogManager.eDialog.LuckyChance);
		if (dialog2 != null)
		{
			DialogLuckyChance lcDialog = dialog2 as DialogLuckyChance;
			while (lcDialog.isBuying())
			{
				yield return 0;
			}
		}
		if (d.isOpen())
		{
			yield return 0;
		}
		while (NetworkMng.Instance.isDownloading())
		{
			yield return 0;
		}
		Debug.Log("login_555");
		int retryCount = 0;
		int count = Input.forceDisable();
		Input.enable = false;
		connectStatus_ = eConnectStatus.Load;
		loginCB = new Part_Title.LoginCB();
		while (true)
		{
			if (bShowConnectIcon)
			{
				yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
			}
			loginCB.InitCB();
			loginListener.reset();
			while (!SNSCore.Instance.InitComplete)
			{
				Debug.Log("Kakao = " + KakaoCore.Instance.InitComplete);
				yield return null;
			}
			Debug.Log("SNSCore.IsAuthorize - " + SNSCore.IsAuthorize);
			if (SNSCore.Instance.hasValidTokenCache() && !SNSCore.IsAuthorize)
			{
				SNSCore.Instance.Authorized();
				while (!SNSCore.IsAuthorize)
				{
					yield return null;
				}
			}
			Part_Title.bLoginBtnEnabled = !SNSCore.IsAuthorize && !SNSCore.Instance.hasValidTokenCache();
			Debug.Log("LoginState = " + SNSCore.IsAuthorize);
			Part_Title.bGuestLoginButtonEnable = false;
			int logininputctn = Input.forceEnable();
			while (Part_Title.bLoginBtnEnabled)
			{
				yield return null;
			}
			Input.revertForceEnable(logininputctn);
			bKakaoLeave = false;
			SNSCore.Instance.Login(loginCB);
			Part_Title.bLoginBtnEnabled = false;
			Part_Title.bGuestLoginButtonEnable = false;
			Debug.Log("Waiting for Login");
			while (loginCB.result_ == -1 && !bKakaoLeave)
			{
				yield return null;
			}
			if (bKakaoLeave)
			{
				yield return new WaitForSeconds(2f);
			}
			if (loginCB.IsSuccess() && loginCB.isPlayable_)
			{
				break;
			}
			int resultCode = loginCB.result_;
		}
		long newID = Convert.ToInt64(SNSCore.local_UserData_.id);
		long currentID = GlobalData.Instance.LineID;
		if (currentID != 0L && currentID != newID)
		{
			Input.revertForceDisable(count);
			Input.enable = true;
			Part_Title.bStartGame = false;
			bLineLogin_ = false;
			gotoTitle();
		}
		else
		{
			GlobalData.Instance.LineID = newID;
			loginCB = null;
		}
		if (bShowConnectIcon && ((currentPart != ePart.Map && currentPart != ePart.EventMap && currentPart != ePart.CollaborationMap && currentPart != ePart.ChallengeMap) || bCancel))
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		}
		Input.revertForceDisable(count);
		if ((currentPart != ePart.Map && currentPart != ePart.EventMap && currentPart != ePart.CollaborationMap && currentPart != ePart.ChallengeMap) || bCancel)
		{
			Input.enable = true;
		}
		else
		{
			notInputBackCount++;
		}
		Debug.Log("partManager notInputBackCount = " + notInputBackCount);
		connectStatus_ = eConnectStatus.Success;
		bLineLogin_ = false;
		Debug.Log("login_end");
	}

	public IEnumerator OnGuestModeCancel()
	{
		isguestenter = false;
		yield break;
	}

	public void SetMarketViewFlag(bool flag)
	{
		marketreview_flag_ = flag;
	}

	public IEnumerator OpenMarketReviewDialog()
	{
		if (!marketreview_flag_)
		{
			DialogCommon reviewdialog = dialogManager.getDialog(DialogManager.eDialog.Review) as DialogCommon;
			reviewdialog.setup(OnMarketReview, null, true);
			yield return StartCoroutine(dialogManager.openDialog(reviewdialog));
			Debug.Log("OnMarketReview Open");
			while (reviewdialog.isOpen())
			{
				yield return null;
			}
			Debug.Log("MarketReview Send");
			SetMarketViewFlag(true);
		}
	}

	public IEnumerator OnMarketReview()
	{
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(API.RequestMarketReview, true, true));
		yield return StartCoroutine(WebView.Instance.show(WebView.eWebType.Market, dialogManager));
		Debug.Log("OnMarketReview End");
	}

	private IEnumerator RegisterDeviceToken()
	{
		devicetoken_breakflag = false;
		devicetoken = null;
		StartCoroutine("DevicetokenBreakFlag");
		while (GKUnityPluginController.deviceToken_ == string.Empty)
		{
			if (devicetoken_breakflag)
			{
				yield break;
			}
			yield return null;
		}
		string token = GKUnityPluginController.deviceToken_;
		devicetoken = token;
		PlayerPrefs.SetString("DeviceToken", devicetoken);
		PlayerPrefs.Save();
	}

	public IEnumerator SendDeviceToken(Part_Title partitle)
	{
		while (!devicetoken_breakflag)
		{
			yield return null;
		}
		if ((devicetoken == null || devicetoken == string.Empty) && (PlayerPrefs.GetString("DeviceToken") == null || PlayerPrefs.GetString("DeviceToken") == string.Empty))
		{
			Debug.Log("DeviceToken notfound#################################################################################");
			yield break;
		}
		Debug.Log("devicetoken = " + devicetoken);
		Debug.Log("Prefs = " + PlayerPrefs.GetString("DeviceToken"));
		if (devicetoken == null)
		{
			devicetoken = PlayerPrefs.GetString("DeviceToken");
		}
		Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
		Debug.Log("SendDeviceToken = " + devicetoken);
		NetworkMng.Instance.setup(Hash.RegisterDeviceToken(devicetoken));
		yield return partitle.StartCoroutine(NetworkMng.Instance.download(API.RegisterDeviceToken, false, false));
		Debug.Log("SendDeviceTokenComplete!!!");
	}

	private IEnumerator DevicetokenBreakFlag()
	{
		yield return new WaitForSeconds(3f);
		devicetoken_breakflag = true;
	}

	public bool isLineLogin()
	{
		return bLineLogin_ || bSuspend;
	}

	private IEnumerator OnRetryCB()
	{
		Input.enable = false;
		yield break;
	}

	public IEnumerator retryDialog(bool bShowConnectIcon, bool bCancel, int resultCode)
	{
		int count = Input.forceEnable();
		DialogCommon dialog = dialogManager.getDialog(DialogManager.eDialog.NetworkError) as DialogCommon;
		dialog.setMessage(MessageResource.Instance.getMessage(35));
		dialog.setup(OnRetryCB, null, true);
		dialog.setButtonActive(DialogCommon.eBtn.Close, false);
		yield return StartCoroutine(dialogManager.openDialog(dialog));
		while (dialog.isOpen())
		{
			yield return 0;
		}
		Input.revertForceEnable(count);
		Input.enable = true;
	}

	public IEnumerator restartDialog(int resultCode)
	{
		yield break;
	}

	public IEnumerator nologin(bool gototitle = false, bool isjewel = false)
	{
		isNologinCancel = false;
		if (!SNSCore.IsAuthorize)
		{
			DialogNologin dialog = dialogManager.getDialog(DialogManager.eDialog.Nologin) as DialogNologin;
			dialog.GoToTitle = gototitle;
			dialog.IsJewelShop = isjewel;
			yield return dialogManager.StartCoroutine(dialogManager.openDialog(dialog));
			while (dialog.isOpen())
			{
				yield return null;
			}
			isNologinCancel = true;
		}
	}

	public void gotoTitle()
	{
		UnityEngine.Object.Destroy(GameObject.Find("DebugMenu"));
		UnityEngine.Object.Destroy(GameObject.Find("DebugMenuFactroy"));
		UnityEngine.Object.Destroy(GameObject.Find("GlobalData"));
		UnityEngine.Object.Destroy(GameObject.Find("MessageResource"));
		UnityEngine.Object.Destroy(GameObject.Find("Network"));
		UnityEngine.Object.Destroy(GameObject.Find("ResourceLoader"));
		UnityEngine.Object.Destroy(GameObject.Find("SaveData"));
		UnityEngine.Object.Destroy(GameObject.Find("Scenario"));
		UnityEngine.Object.Destroy(GameObject.Find("TutorialManager"));
		DummyPlayerData.Data = new UserData(string.Empty, 0, 0, string.Empty, 0, 100L);
		DummyPlayerData.result_ = -1;
		DummyPlayFriendData.FriendNum = 0;
		DummyPlayFriendData.DummyFriends = new UserData[0];
		DummyPlayFriendData.playedMemberList = new List<long>();
		DummyLineFriendData.FriendNum = 0;
		DummyLineFriendData.DummyFriends = new UserData[0];
		Application.LoadLevel("main");
	}

	private void replayBgm(bool pauseStatus)
	{
		if (Sound.Instance == null)
		{
			return;
		}
		if (pauseStatus)
		{
			if (Sound.Instance.isPlayingBgm())
			{
				replayBgmNum = Sound.Instance.currentBgm;
				replayBgmTime = Sound.Instance.timeBgm();
				replayBgmLoop = Sound.Instance.isBgmLoop();
				Sound.Instance.stopBgm(false);
			}
			else
			{
				replayBgmNum = Sound.eBgm.Max;
			}
		}
		else if (replayBgmNum != Sound.eBgm.Max)
		{
			Sound.Instance.playBgm(replayBgmNum, replayBgmLoop);
			Sound.Instance.setBGMTime(replayBgmTime);
		}
	}

	private bool isDispTips(ePart nextPart)
	{
		if ((currentPart == ePart.Map && nextPart == ePart.EventMap) || (currentPart == ePart.EventMap && nextPart == ePart.Map))
		{
			return false;
		}
		if ((currentPart == ePart.Map && nextPart == ePart.CollaborationMap) || (currentPart == ePart.CollaborationMap && nextPart == ePart.Map))
		{
			return false;
		}
		return true;
	}

	public void startTips()
	{
		StartCoroutine("tipsRoutine");
	}

	public void stopTips()
	{
		StopCoroutine("tipsRoutine");
		if (GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.Tips) != null)
		{
			GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.Tips).SetActive(false);
		}
	}

	public void inhibitTips(bool bInhibit)
	{
		isTipsInhibit = bInhibit;
	}

	private IEnumerator tipsRoutine()
	{
		TalkMessage talkMsg = TalkMessage.Instance;
		GameObject tips = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.Tips);
		GameObject labelObj = tips.transform.Find("Label").gameObject;
		if (!labelObj.activeSelf)
		{
			labelObj.SetActive(true);
		}
		tips.SetActive(true);
		UILabel label = tips.GetComponentInChildren<UILabel>();
		label.text = string.Empty;
		TweenGroup openTween = null;
		TweenGroup closeTween = null;
		TweenGroup[] groups = tips.GetComponents<TweenGroup>();
		TweenGroup[] array = groups;
		foreach (TweenGroup group in array)
		{
			switch (group.getGroupName())
			{
			case "In":
				openTween = group;
				break;
			case "Out":
				closeTween = group;
				break;
			}
		}
		tips.SetActive(false);
		while (true)
		{
			if (isTipsInhibit)
			{
				yield return null;
				continue;
			}
			tips.SetActive(true);
			label.text = talkMsg.getTipsMessage();
			openTween.Play();
			float elapsedTime = 0f;
			while (elapsedTime < 10f && !isTipsInhibit && tips.activeSelf)
			{
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			if (!tips.activeSelf)
			{
				break;
			}
			closeTween.Play();
			while (closeTween.isPlaying())
			{
				yield return null;
			}
			if (tips.activeSelf)
			{
				continue;
			}
			break;
		}
	}

	public void showBoard(WebView.eWebType webType)
	{
		string value = null;
		string text = null;
		int num = 0;
		switch (webType)
		{
		case WebView.eWebType.Notice:
			value = "notice";
			break;
		case WebView.eWebType.Help:
			value = "help";
			break;
		case WebView.eWebType.SCTL:
			value = "terms";
			text = "LGAPP_ebiz_rules";
			num = 1413;
			break;
		case WebView.eWebType.EFTA:
			value = "terms";
			text = "LGPB_sikin";
			num = 1414;
			break;
		}
		if (!string.IsNullOrEmpty(value))
		{
		}
	}
}
