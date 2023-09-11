using System;
using SimpleJSON;

public class KakaoInvitationEvent
{
	private static KakaoInvitationEvent _instance;

	public string eventId { get; private set; }

	public int maxSenderRewardCount { get; private set; }

	public DateTime eventStartsAt { get; private set; }

	public DateTime eventEndsAt { get; private set; }

	public string senderRewardCode { get; private set; }

	public string receiverRewardCode { get; private set; }

	public string invitationUrl { get; private set; }

	public int totalReveiversCount { get; private set; }

	public static KakaoInvitationEvent Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new KakaoInvitationEvent();
			}
			return _instance;
		}
	}

	public void clear()
	{
		eventId = null;
		maxSenderRewardCount = 0;
		senderRewardCode = null;
		receiverRewardCode = null;
		invitationUrl = null;
		totalReveiversCount = 0;
	}

	public void printForDebugging()
	{
		KakaoNativeExtension.Instance.ShowAlertMessage("Event ID:" + eventId + " / URL for Invitation : " + invitationUrl);
	}

	public void setInvitationEventFromJSON(JSONNode root)
	{
		clear();
		Debug.Log("Parse \"Event from Invitation Tracking API\"!");
		Debug.Log(root.ToString());
		JSONNode jSONNode = root["invitation_event"];
		if (jSONNode == null)
		{
			Debug.Log("Invitation event is empty!");
			return;
		}
		eventId = jSONNode["id"];
		maxSenderRewardCount = jSONNode["max_sender_rewards_count"].AsInt;
		string s = jSONNode["starts_at"].Value.ToString();
		DateTime result;
		DateTime.TryParse(s, out result);
		eventStartsAt = result;
		s = jSONNode["ends_at"].Value.ToString();
		DateTime.TryParse(s, out result);
		eventEndsAt = result;
		senderRewardCode = jSONNode["sender_reward"];
		receiverRewardCode = jSONNode["receiver_reward"];
		JSONNode jSONNode2 = jSONNode["invitation_sender"];
		if (jSONNode2 != null)
		{
			invitationUrl = jSONNode2["invitation_url"];
			totalReveiversCount = jSONNode2["total_receivers_count"].AsInt;
		}
	}
}
