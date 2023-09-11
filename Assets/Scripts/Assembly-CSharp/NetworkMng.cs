using System.Collections;
using System.Collections.Generic;
using LitJson;
using Network;
using UnityEngine;

public class NetworkMng : MonoBehaviour
{
	public enum eStatus
	{
		None = 0,
		Load = 1,
		Error = 2,
		Success = 3,
		Retry = 4,
		Cancel = 5,
		InvalidSessionID = 6
	}

	public class ResultCodeInfo
	{
		public class Info
		{
			public int ResultCode;

			public int MsgID;
		}

		public Info[] Infos;
	}

	public delegate WWW OnCreateWWW(Hashtable args);

	private static NetworkMng instance_;

	private eStatus status_;

	private Hashtable args_;

	private WWW www_;

	private NetworkIcon icon_;

	[SerializeField]
	private DialogManager DialogMng;

	[SerializeField]
	private PartManager PartMng;

	private bool bIconForceDisable_;

	private eStatus recovaryStatus_;

	private eResultCode resultCode_;

	private Dictionary<eResultCode, ResultCodeInfo.Info> resultCodeDict_ = new Dictionary<eResultCode, ResultCodeInfo.Info>();

	private bool bShowTween_;

	private bool bHideTween_;

	private bool bDownloading;

	public static NetworkMng Instance
	{
		get
		{
			return instance_;
		}
	}

	public static bool IsInstance()
	{
		return (!(instance_ == null)) ? true : false;
	}

	private void Awake()
	{
		if (instance_ == null)
		{
			instance_ = this;
			Object.DontDestroyOnLoad(this);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void init(NetworkIcon icon)
	{
		icon_ = icon;
		TextAsset textAsset = Resources.Load("Parameter/resultcode_info", typeof(TextAsset)) as TextAsset;
		ResultCodeInfo resultCodeInfo = Xml.DeserializeObject<ResultCodeInfo>(textAsset.text) as ResultCodeInfo;
		ResultCodeInfo.Info[] infos = resultCodeInfo.Infos;
		ResultCodeInfo.Info[] array = infos;
		foreach (ResultCodeInfo.Info info in array)
		{
			resultCodeDict_[(eResultCode)info.ResultCode] = info;
		}
	}

	private void OnDestroy()
	{
		instance_ = null;
	}

	public void setup(Hashtable args)
	{
		status_ = eStatus.None;
		args_ = args;
		www_ = null;
	}

	public void forceIconDisable(bool bDisable)
	{
		bIconForceDisable_ = bDisable;
	}

	private IEnumerator playIconTween(TweenGroup tween)
	{
		tween.Play();
		while (icon_.gameObject.activeSelf && tween.isPlaying())
		{
			yield return 0;
		}
	}

	public IEnumerator showIcon(bool bShow)
	{
		if (bShow && bShowTween_)
		{
			while (bShowTween_)
			{
				yield return null;
			}
			yield break;
		}
		if (!bShow && bHideTween_)
		{
			while (bHideTween_)
			{
				yield return null;
			}
			yield break;
		}
		if (bShow && bHideTween_)
		{
			while (bHideTween_)
			{
				yield return null;
			}
		}
		if (!(icon_ == null))
		{
			if (bShow)
			{
				bShowTween_ = true;
				icon_.gameObject.SetActive(true);
				icon_.StopAllCoroutines();
				bHideTween_ = false;
				yield return icon_.StartCoroutine(playIconTween(icon_.getTween(true)));
				bShowTween_ = false;
			}
			else if (icon_.gameObject.activeSelf)
			{
				bHideTween_ = true;
				icon_.StopAllCoroutines();
				bShowTween_ = false;
				yield return icon_.StartCoroutine(playIconTween(icon_.getTween(false)));
				icon_.gameObject.SetActive(false);
				bHideTween_ = false;
			}
		}
	}

	public IEnumerator download(OnCreateWWW createWWWCB, bool bCancel)
	{
		yield return StartCoroutine(download(createWWWCB, bCancel, true));
	}

	public IEnumerator download(OnCreateWWW createWWWCB, bool bCancel, bool bShowIcon)
	{
		yield return StartCoroutine(download(createWWWCB, bCancel, bShowIcon, false, true));
	}

	public IEnumerator download(OnCreateWWW createWWWCB, bool bCancel, bool bShowIcon, bool bForce)
	{
		yield return StartCoroutine(download(createWWWCB, bCancel, bShowIcon, bForce, true));
	}

	public bool isDownloading()
	{
		return bDownloading;
	}

	public bool isShowIcon()
	{
		return icon_ != null && icon_.gameObject.activeSelf;
	}

	public IEnumerator download(OnCreateWWW createWWWCB, bool bCancel, bool bShowIcon, bool bForce, bool bShowError)
	{
		while (PartMng.isLineLogin() && !bForce)
		{
			yield return 0;
		}
		while (bDownloading)
		{
			yield return null;
		}
		bDownloading = true;
		Input.enable = false;
		int retryCount = 0;
		do
		{
			bool bRetry = isRetry();
			if ((icon_ != null && bShowIcon) || (bRetry && !bIconForceDisable_))
			{
				yield return StartCoroutine(showIcon(true));
			}
			yield return StartCoroutine(download(createWWWCB));
			resultCode_ = NetworkUtility.getResultCode(www_);
			if (resultCode_ == eResultCode.MONETIZATION_FREE_AD_NOT_ACTIVE)
			{
				GKUnityPluginController.Instance.ToastMessage(MessageResource.Instance.getMessage(500040));
				Debug.Log(" Network.eResultCode.MONETIZATION_FREE_AD_NOT_ACTIVE");
			}
			else if (resultCode_ == eResultCode.MONETIZATION_FREE_AD_NOT_CHARGED)
			{
				GKUnityPluginController.Instance.ToastMessage(MessageResource.Instance.getMessage(500039));
				Debug.Log(" Network.eResultCode.MONETIZATION_FREE_AD_NOT_CHARGED");
			}
			else if (resultCode_ != 0 && bShowError)
			{
				status_ = getErrorStatus(resultCode_);
				if (!isRecovary() && isRecovaryCheck())
				{
					saveRecovaryDate();
				}
				saveSessionID(www_);
				if (icon_ != null && icon_.gameObject.activeSelf)
				{
					yield return StartCoroutine(showIcon(false));
				}
				int count = Input.forceEnable();
				if (resultCode_ == eResultCode.InvalidSessionID)
				{
					updateCommonData(www_);
				}
				if (retryCount == 0 && canRetryError(bCancel, resultCode_) && !invalidAutoRetry(resultCode_))
				{
					status_ = eStatus.Retry;
				}
				else
				{
					yield return StartCoroutine(openErrorDialog(bCancel, resultCode_));
				}
				Input.revertForceEnable(count);
				if (isCancel())
				{
					break;
				}
				retryCount++;
				continue;
			}
			if ((icon_ != null && bShowIcon) || (bRetry && !bIconForceDisable_))
			{
				yield return StartCoroutine(showIcon(false));
			}
			status_ = eStatus.Success;
		}
		while (status_ != eStatus.Success);
		saveSessionID(www_);
		Input.enable = true;
		bDownloading = false;
	}

	private void saveSessionID(WWW www)
	{
		ResponceHeaderData responceHeaderData = NetworkUtility.createResponceHeaderData(www);
		if (responceHeaderData != null && responceHeaderData.SessionID != null)
		{
			SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
			networkData.setSessionID(responceHeaderData.SessionID, true);
		}
	}

	private IEnumerator download(OnCreateWWW createWWWCB)
	{
		status_ = eStatus.Load;
		if (isRecovary())
		{
			yield return StartCoroutine(sendRecovary());
			if (recovaryStatus_ == eStatus.Error)
			{
				yield break;
			}
		}
		www_ = createWWWCB(args_);
		float waitTime = 25f;
		float reachabilityCheckTime = Time.realtimeSinceStartup + waitTime;
		while (www_ != null && !www_.isDone && string.IsNullOrEmpty(www_.error))
		{
			if (reachabilityCheckTime < Time.realtimeSinceStartup)
			{
				if (Application.internetReachability == NetworkReachability.NotReachable)
				{
					www_.Dispose();
					www_ = null;
					break;
				}
				reachabilityCheckTime = Time.realtimeSinceStartup + waitTime;
			}
			yield return null;
		}
	}

	private IEnumerator sendRecovary()
	{
		SaveNetworkData netData = SaveData.Instance.getGameData().getNetworkData();
		recovaryStatus_ = eStatus.Load;
		Hashtable args = Hash.Recovary(netData);
		www_ = API.Recovary(args);
		yield return www_;
		resultCode_ = NetworkUtility.getResultCode(www_);
		if (resultCode_ != 0)
		{
			recovaryStatus_ = getErrorStatus(resultCode_);
			yield break;
		}
		saveSessionID(www_);
		netData.resetRecoverySaveDate();
		netData.save();
		recovaryStatus_ = eStatus.Success;
		if (GlobalData.IsInstance() && GlobalData.Instance.getGameData() != null)
		{
			CommonData commonData = JsonMapper.ToObject<CommonData>(www_.text);
			GlobalData.Instance.getGameData().setCommonData(commonData, true);
		}
	}

	public bool isError()
	{
		if (status_ == eStatus.Error)
		{
			return true;
		}
		return false;
	}

	private bool isCancel()
	{
		if (status_ == eStatus.Cancel || status_ == eStatus.InvalidSessionID)
		{
			return true;
		}
		return false;
	}

	private bool isRetry()
	{
		if (status_ == eStatus.Retry)
		{
			return true;
		}
		return false;
	}

	public WWW getWWW()
	{
		return www_;
	}

	public eStatus getStatus()
	{
		return status_;
	}

	public eResultCode getResultCode()
	{
		return resultCode_;
	}

	private IEnumerator OnCancelCB()
	{
		status_ = eStatus.Cancel;
		yield break;
	}

	private IEnumerator OnRetryCB()
	{
		status_ = eStatus.Retry;
		yield break;
	}

	private IEnumerator OnInvalidSessionCB()
	{
		status_ = eStatus.InvalidSessionID;
		yield break;
	}

	private IEnumerator OnUpdateApp()
	{
		yield return DialogMng.StartCoroutine(WebView.Instance.show(WebView.eWebType.Market, DialogMng));
	}

	public IEnumerator OnMaintenaceQuitApp()
	{
		Constant.SoundUtil.PlayDecideSE();
		yield return new WaitForEndOfFrame();
		Application.Quit();
	}

	public IEnumerator openErrorDialog(bool bCancel, eResultCode resultCode)
	{
		PartMng.stopTips();
		DialogCommon dialog2 = null;
		switch (resultCode)
		{
		case eResultCode.Maintenance:
			dialog2 = DialogMng.getDialog(DialogManager.eDialog.Maintenance) as DialogCommon;
			break;
		case eResultCode.VersionUpdate:
		case eResultCode.NotExistStageItem:
		case eResultCode.InvalidHeartMail:
		case eResultCode.InvalidRankingReward:
		case eResultCode.ContinueSaleHadFinished:
		case eResultCode.NotExistCampaign:
			dialog2 = DialogMng.getDialog(DialogManager.eDialog.Common) as DialogCommon;
			break;
		default:
			dialog2 = DialogMng.getDialog(DialogManager.eDialog.NetworkError) as DialogCommon;
			break;
		}
		dialog2.sysLabelEnable(true);
		dialog2.setMessageSize(32f);
		setupErrorDialog(dialog2, bCancel, resultCode);
		if (!isCloseValidResultCode(resultCode))
		{
			dialog2.setButtonActive(DialogCommon.eBtn.Close, false);
		}
		yield return StartCoroutine(DialogMng.openDialog(dialog2));
		while (dialog2.isOpen())
		{
			yield return 0;
		}
	}

	private void setupErrorDialog(DialogCommon dialog, bool bCancel, eResultCode resultCode)
	{
		eResultCode key = resultCode;
		if (!resultCodeDict_.ContainsKey(resultCode))
		{
			key = eResultCode.ErrorUnknown;
		}
		if (isCancelResultCode(resultCode))
		{
			if (resultCode == eResultCode.InvalidHeartMail && GlobalData.Instance.getGameData().isHeartSendCampaign)
			{
				MessageResource instance = MessageResource.Instance;
				string src = instance.getMessage(resultCodeDict_[key].MsgID) + instance.getMessage(2591);
				src = instance.castCtrlCode(src, 1, GlobalData.Instance.getGameData().heartSendHour.ToString());
				dialog.setMessageSize(30f);
				dialog.sysLabelEnable(false);
				dialog.setup(src, OnCancelCB, OnCancelCB, true);
			}
			else
			{
				dialog.setup(resultCodeDict_[key].MsgID, OnCancelCB, OnCancelCB, true);
			}
			dialog.setButtonText(DialogCommon.eText.Confirm);
			return;
		}
		switch (resultCode)
		{
		case eResultCode.VersionUpdate:
			dialog.setup(resultCodeDict_[key].MsgID, OnUpdateApp, null, true);
			dialog.setButtonActive(DialogCommon.eBtn.Close, false);
			dialog.setButtonText(DialogCommon.eText.Confirm);
			return;
		case eResultCode.Maintenance:
			dialog.setup(resultCodeDict_[key].MsgID, null, OnMaintenaceQuitApp, true);
			dialog.setButtonActive(DialogCommon.eBtn.Close, true);
			return;
		}
		if (!bCancel)
		{
			dialog.setButtonActive(DialogCommon.eBtn.Close, false);
			dialog.setup(resultCodeDict_[key].MsgID, OnRetryCB, null, true);
			dialog.setButtonText(DialogCommon.eText.Retry);
		}
		else if (resultCode == eResultCode.InvalidSessionID)
		{
			dialog.setup(resultCodeDict_[key].MsgID, OnInvalidSessionCB, OnInvalidSessionCB, true);
			dialog.setButtonText(DialogCommon.eText.Confirm);
		}
		else
		{
			dialog.setup(resultCodeDict_[key].MsgID, OnRetryCB, OnCancelCB, true);
			dialog.setButtonText(DialogCommon.eText.Retry);
		}
	}

	private bool canRetryError(bool bCancel, eResultCode resultCode)
	{
		if (isCancelResultCode(resultCode))
		{
			return false;
		}
		if (resultCode == eResultCode.VersionUpdate)
		{
			return false;
		}
		if (!bCancel)
		{
			return true;
		}
		if (resultCode == eResultCode.InvalidSessionID)
		{
			return false;
		}
		return true;
	}

	private bool invalidAutoRetry(eResultCode resultCode)
	{
		if (resultCode == eResultCode.NotStageStart)
		{
			return true;
		}
		return false;
	}

	private eAPI getAPIType(Hashtable args)
	{
		if (args == null)
		{
			return eAPI.Invalid;
		}
		if (!args.ContainsKey("API"))
		{
			return eAPI.Invalid;
		}
		return (eAPI)(int)args["API"];
	}

	private bool isMatchAPI(eAPI api)
	{
		if (getAPIType(args_) == api)
		{
			return true;
		}
		return false;
	}

	private bool isRecovaryCheck()
	{
		if (isMatchAPI(eAPI.StageBegin) || isMatchAPI(eAPI.BuyItem) || isMatchAPI(eAPI.StageContinue) || isMatchAPI(eAPI.StageReplay))
		{
			return true;
		}
		return false;
	}

	private void updateCommonData(WWW www)
	{
		if (www == null || !string.IsNullOrEmpty(www.error) || string.IsNullOrEmpty(www.text) || isMatchAPI(eAPI.Login) || !GlobalData.IsInstance())
		{
			return;
		}
		GameData gameData = GlobalData.Instance.getGameData();
		if (GlobalData.Instance.getGameData() == null)
		{
			return;
		}
		JsonData jsonData = JsonMapper.ToObject(www.text);
		gameData.continueNum = (int)jsonData["continueNum"];
		CommonData commonData = JsonMapper.ToObject<CommonData>(www_.text);
		gameData.setCommonData(commonData, false);
		if (GlobalRoot.IsInstance())
		{
			GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI);
			if (!(@object == null))
			{
				MainMenu component = @object.GetComponent<MainMenu>();
				component.update();
			}
		}
	}

	private eStatus getErrorStatus(eResultCode resultCode)
	{
		return eStatus.Error;
	}

	private bool isRecovary()
	{
		SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
		return networkData.isRecovery();
	}

	private void saveRecovaryDate()
	{
		SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
		networkData.setRecoveryID(networkData.getSessionID());
		networkData.save();
	}

	public static bool isCancelResultCode(eResultCode resultCode)
	{
		if (resultCode == eResultCode.ShortageJewel || resultCode == eResultCode.ShortageCoin || resultCode == eResultCode.ShortageHeart || resultCode == eResultCode.NotStageClear || resultCode == eResultCode.NotExistStageData || resultCode == eResultCode.NotStageStart || resultCode == eResultCode.NotSkipStage || resultCode == eResultCode.NotExistStageItem || resultCode == eResultCode.InvalidTreasure || resultCode == eResultCode.OpendTreasure || resultCode == eResultCode.InvalidHeartMail || resultCode == eResultCode.NotExistsMaile || resultCode == eResultCode.InvalidHelpMail || resultCode == eResultCode.CantHelpMail || resultCode == eResultCode.InvalidRankingReward || resultCode == eResultCode.EventIsNotHolding || resultCode == eResultCode.EventAlreadySent || resultCode == eResultCode.EventAlreadySentAll || resultCode == eResultCode.AlreadyInvite || resultCode == eResultCode.DailyLimitOverInvite || resultCode == eResultCode.LimitOverInvite || resultCode == eResultCode.AddedReward || resultCode == eResultCode.NotExistReward || resultCode == eResultCode.LimitOverReward || resultCode == eResultCode.NotExistsGachaTicket || resultCode == eResultCode.NotExistDrawAvatar || resultCode == eResultCode.ContinueSaleHadFinished || resultCode == eResultCode.NotExistCampaign)
		{
			return true;
		}
		return false;
	}

	public static bool isCloseValidResultCode(eResultCode resultCode)
	{
		if (resultCode == eResultCode.InvalidRequestMethod || resultCode == eResultCode.InvalidRequestParameter || resultCode == eResultCode.InvalidRequestHeader || resultCode == eResultCode.ErrorAesDecrypt || resultCode == eResultCode.Lock || resultCode == eResultCode.NotSupportOS || resultCode == eResultCode.InvalidSessionID || resultCode == eResultCode.ShortageJewel || resultCode == eResultCode.ShortageCoin || resultCode == eResultCode.ShortageHeart || resultCode == eResultCode.NotStageClear || resultCode == eResultCode.NotExistStageData || resultCode == eResultCode.NotStageStart || resultCode == eResultCode.NotExistStageItem || resultCode == eResultCode.InvalidTreasure || resultCode == eResultCode.OpendTreasure || resultCode == eResultCode.InvalidHeartMail || resultCode == eResultCode.NotExistsMaile || resultCode == eResultCode.InvalidHelpMail || resultCode == eResultCode.CantHelpMail || resultCode == eResultCode.InvalidRankingReward || resultCode == eResultCode.AlreadyInvite || resultCode == eResultCode.DailyLimitOverInvite || resultCode == eResultCode.LimitOverInvite || resultCode == eResultCode.EventIsNotHolding || resultCode == eResultCode.EventAlreadySent || resultCode == eResultCode.EventAlreadySentAll || resultCode == eResultCode.AddedReward || resultCode == eResultCode.NotExistReward || resultCode == eResultCode.NotExistDrawAvatar || resultCode == eResultCode.LimitOverReward || resultCode == eResultCode.ContinueSaleHadFinished || resultCode == eResultCode.NotExistCampaign)
		{
			return false;
		}
		return true;
	}

	public void statusChangeLoad()
	{
		status_ = eStatus.Load;
	}

	public void statusChangeSuccess()
	{
		status_ = eStatus.Success;
	}
}
