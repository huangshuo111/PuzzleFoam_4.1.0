using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using Network;
using Toast.Analytics;
using UnityEngine;

public class GKToastListener : CampaignListener
{
	private static GKToastListener instance_;

	private PartManager partManager_;

	private bool isOpen;

	public static GKToastListener Instance
	{
		get
		{
			if (instance_ == null)
			{
				instance_ = new GKToastListener();
			}
			return instance_;
		}
	}

	private GKToastListener()
	{
		Debug.Log("ToastListener Init");
	}

	public void OnCampaignVisibilityChanged(string adspaceName, bool show)
	{
	}

	public void OnCampaignLoadSuccess(string adspaceName)
	{
	}

	public void OnCampaignLoadFail(string adspaceName, int errorCode, string errorMessage)
	{
	}

	public void OnCampaignClick(string callbackInfo)
	{
	}

	public void OnMissionComplete(List<string> missionList)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ OnMissionComplete " + missionList.Count);
		if (missionList.Count >= 1)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{\"list\":[\"" + missionList[0] + "\"");
			for (int i = 1; i < missionList.Count; i++)
			{
				stringBuilder.Append(",\"" + missionList[i] + "\"");
			}
			stringBuilder.Append("]}");
			Debug.Log(stringBuilder.ToString());
			Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ Dictionary ======");
			partManager_.StartCoroutine(RequestToastPromotionData(stringBuilder.ToString()));
		}
	}

	public IEnumerator showToastPromotionDialog(PartManager partManager)
	{
		partManager_ = partManager;
		if (GameAnalytics.isPromotionAvailable())
		{
			GameAnalytics.launchPromotionPage();
			Debug.Log("Open ToastPromotion Dialog");
			while (isOpen)
			{
				yield return null;
			}
			Debug.Log("Close ToastPromotion Dialog");
			yield return null;
		}
	}

	private IEnumerator RequestToastPromotionData(string data)
	{
		NetworkMng.Instance.setup(Hash.RequestToastPromotionReward(GlobalData.Instance.LineID.ToString(), "5afa6bf52890b2e236089c8134ab00dd6eb7202e782299234741956073971775", data));
		yield return partManager_.StartCoroutine(NetworkMng.Instance.download(API.RequestToastPromotionReward, false, false));
		WWW www = NetworkMng.Instance.getWWW();
		ToastPromotionResult resultData = JsonMapper.ToObject<ToastPromotionResult>(www.text);
		DialogCommon dialog = partManager_.dialogManager.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		if (resultData.resultcode == 0)
		{
			dialog.setup(MessageResource.Instance.AddLine(resultData.message), null, null, true);
			yield return partManager_.StartCoroutine(dialog.open());
			while (dialog.isOpen())
			{
				yield return null;
			}
			Debug.Log("@@@@@@@@@@@@@@@@ Success");
		}
	}

	public void OnPromotionVisibilityChanged(bool show)
	{
		isOpen = show;
	}
}
