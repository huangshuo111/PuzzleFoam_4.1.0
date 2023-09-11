using System.Collections;
using LitJson;
using Network;
using TnkAd;
using UnityEngine;

public class PBTnkVidioHandler : TnkAd.EventHandler
{
	private bool bReward;

	private static bool bRewardUpdate;

	private static bool bContinue;

	public override void onReturnQueryPoint(int point)
	{
		Debug.Log("##### PBTnkVidioHandler - onReturnQueryPoint " + point);
	}

	public override void onReturnPurchaseItem(long curPoint, long seqId)
	{
		Debug.Log("##### PBTnkVidioHandler - onReturnPurchaseItem point = " + curPoint);
		Debug.Log("##### PBTnkVidioHandler - onReturnPurchaseItem seqId = " + seqId);
	}

	public override void onReturnQueryPublishState(int state)
	{
		Debug.Log("##### PBTnkVidioHandler - onReturnQueryPublishState " + state);
	}

	public override void onClose(int type)
	{
		UnityEngine.Debug.Log("##### PBTnkVidioHandler - TnkAd.Listener onClose " + type);
		switch (type)
		{
		case 0:
			UnityEngine.Debug.Log("##### PBTnkVidioHandler - TnkAd.Listener onClose // CLOSE_SIMPLE");
			if (!bReward)
			{
				GKUnityPluginController.Instance.ToastMessage(MessageResource.Instance.getMessage(500038));
			}
			break;
		case 1:
			UnityEngine.Debug.Log("##### PBTnkVidioHandler - TnkAd.Listener onClose // CLOSE_CLICK");
			break;
		}
	}

	public override void onFailure(int errCode)
	{
		UnityEngine.Debug.Log("##### PBTnkVidioHandler - TnkAd.Listener onFailure " + errCode);
	}

	public override void onLoad()
	{
		UnityEngine.Debug.Log("##### PBTnkVidioHandler - TnkAd.Listener onLoad ");
		bReward = false;
	}

	public override void onShow()
	{
		UnityEngine.Debug.Log("##### PBTnkVidioHandler - TnkAd.Listener onShow ");
	}

	public override void onVideoCompleted(bool skipped)
	{
		UnityEngine.Debug.Log("##### PBTnkVidioHandler - TnkAd.onVideoCompleted " + skipped);
		if (skipped)
		{
			UnityEngine.Debug.Log("##### PBTnkVidioHandler - TnkAd.onVideoCompleted // true");
			return;
		}
		UnityEngine.Debug.Log("##### PBTnkVidioHandler - TnkAd.onVideoCompleted // false");
		bReward = true;
		if (!bContinue)
		{
			StartCoroutine(getMonetizationAD());
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
		Debug.Log(" www : text : " + www.text);
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			CommonData data_ = JsonMapper.ToObject<CommonData>(www.text);
			GlobalData.Instance.getGameData().monetization = data_.monetization;
			GlobalData.Instance.getGameData().rewardNum = data_.rewardNum;
			GlobalData.Instance.getGameData().rewardType = data_.rewardType;
			GlobalData.Instance.getGameData().coin = data_.coin;
			GlobalData.Instance.getGameData().buyJewel = data_.buyJewel;
			GlobalData.Instance.getGameData().heart = data_.heart;
			GKUnityPluginController.Instance.ToastMessage(MessageResource.Instance.getMessage(500042));
			setRewardUpadate(true);
		}
		else
		{
			UnityEngine.Debug.Log(" getMonetizationAD : NotSuccess! ");
		}
	}

	public bool getRewardUpdate()
	{
		return bRewardUpdate;
	}

	public void setRewardUpadate(bool _bRewardUpdate)
	{
		bRewardUpdate = _bRewardUpdate;
	}

	public void setContinueUpdate(bool _bContinue)
	{
		Debug.Log(" setContinueUpdate : " + bContinue);
		bContinue = _bContinue;
	}
}
