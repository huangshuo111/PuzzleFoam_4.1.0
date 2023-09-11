using TnkAd;
using UnityEngine;

public class PBTnkExitHandler : TnkAd.EventHandler
{
	private bool bReadyAD;

	public override void onReturnQueryPoint(int point)
	{
		Debug.Log("##### PBTnkExitHandler - onReturnQueryPoint " + point);
	}

	public override void onReturnPurchaseItem(long curPoint, long seqId)
	{
		Debug.Log("##### PBTnkExitHandler - onReturnPurchaseItem point = " + curPoint);
		Debug.Log("##### PBTnkExitHandler - onReturnPurchaseItem seqId = " + seqId);
	}

	public override void onReturnQueryPublishState(int state)
	{
		Debug.Log("##### PBTnkExitHandler - onReturnQueryPublishState " + state);
	}

	public override void onClose(int type)
	{
		UnityEngine.Debug.Log("##### PBTnkExitHandler - TnkAd.Listener onClose " + type);
		switch (type)
		{
		case 0:
			UnityEngine.Debug.Log("##### PBTnkExitHandler - TnkAd.Listener onClose // CLOSE_SIMPLE");
			break;
		case 1:
			UnityEngine.Debug.Log("##### PBTnkExitHandler - TnkAd.Listener onClose // CLOSE_CLICK");
			break;
		case 2:
			GlobalGoogleAnalytics.Instance.StopSession();
			Application.Quit();
			break;
		}
	}

	public override void onFailure(int errCode)
	{
		bReadyAD = false;
		UnityEngine.Debug.Log("##### PBTnkExitHandler - TnkAd.Listener onFailure " + errCode);
	}

	public override void onLoad()
	{
		bReadyAD = true;
		UnityEngine.Debug.Log("##### PBTnkExitHandler - TnkAd.Listener onLoad ");
	}

	public override void onShow()
	{
		UnityEngine.Debug.Log("##### PBTnkExitHandler - TnkAd.Listener onShow ");
	}

	public override void onVideoCompleted(bool skipped)
	{
		UnityEngine.Debug.Log("##### PBTnkExitHandler - TnkAd.onVideoCompleted " + skipped);
		if (skipped)
		{
			UnityEngine.Debug.Log("##### PBTnkExitHandler - TnkAd.onVideoCompleted // true");
		}
		else
		{
			UnityEngine.Debug.Log("##### PBTnkExitHandler - TnkAd.onVideoCompleted // false");
		}
	}

	public bool GetReadyAD()
	{
		return bReadyAD;
	}

	public void SetReadyAD(bool _bReadyAD)
	{
		bReadyAD = _bReadyAD;
	}
}
