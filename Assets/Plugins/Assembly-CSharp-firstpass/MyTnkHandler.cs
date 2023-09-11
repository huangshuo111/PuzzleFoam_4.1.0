using TnkAd;
using UnityEngine;

public class MyTnkHandler : EventHandler
{
	public bool showInterstitial = true;

	public override void onReturnQueryPoint(int point)
	{
		Debug.Log("##### onReturnQueryPoint " + point);
	}

	public override void onReturnWithdrawPoints(int point)
	{
		Debug.Log("##### onReturnWithdrawPoints " + point);
	}

	public override void onReturnPurchaseItem(long curPoint, long seqId)
	{
		Debug.Log("##### onReturnPurchaseItem point = " + curPoint);
		Debug.Log("##### onReturnPurchaseItem seqId = " + seqId);
	}

	public override void onReturnQueryPublishState(int state)
	{
		Debug.Log("##### onReturnQueryPublishState " + state);
	}

	public override void onClose(int type)
	{
		Debug.Log("##### TnkAd.Listener onClose " + type);
		if (type == 2)
		{
			Application.Quit();
		}
	}

	public override void onFailure(int errCode)
	{
		Debug.Log("##### TnkAd.Listener onFailure " + errCode);
	}

	public override void onLoad()
	{
		Debug.Log("##### TnkAd.Listener onLoad ");
		if (Plugin.Instance.hasVideoAd("intro_video"))
		{
			Plugin.Instance.showVideoAd("intro_video");
		}
		else if (showInterstitial)
		{
			Plugin.Instance.showInterstitialAd();
		}
	}

	public override void onShow()
	{
		Debug.Log("##### TnkAd.Listener onShow ");
	}

	public override void onVideoCompleted(bool skipped)
	{
		Debug.Log("#### onVideoCompleted " + skipped);
	}
}
