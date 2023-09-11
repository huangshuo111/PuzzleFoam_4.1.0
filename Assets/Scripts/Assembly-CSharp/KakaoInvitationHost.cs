using SimpleJSON;

public class KakaoInvitationHost
{
	private static KakaoInvitationHost _instance;

	public string eventId { get; private set; }

	public string receiverRewardCode { get; private set; }

	public string invitaionUrl { get; private set; }

	public string hostUserId { get; private set; }

	public string hostProfileImageUrl { get; private set; }

	public string hostNickName { get; private set; }

	public int totalRegisterdUserFromHost { get; private set; }

	public static KakaoInvitationHost Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new KakaoInvitationHost();
			}
			return _instance;
		}
	}

	public void clear()
	{
		eventId = null;
		receiverRewardCode = null;
		invitaionUrl = null;
		hostUserId = null;
		hostProfileImageUrl = null;
		hostNickName = null;
		totalRegisterdUserFromHost = 0;
	}

	public void printForDebugging()
	{
		if (eventId == null || eventId.Length == 0)
		{
			KakaoNativeExtension.Instance.ShowAlertMessage("Host response is null or I have not a host.");
			return;
		}
		Debug.Log(string.Format("Event ID : {0}\n", eventId));
		Debug.Log(string.Format("Host Nick Name : {0}\n", hostNickName));
		Debug.Log(string.Format("Host User ID : {0}\n", hostUserId));
		KakaoNativeExtension.Instance.ShowAlertMessage("Host is " + hostNickName);
	}

	public void setInvitationHostFromJSON(JSONNode root)
	{
		clear();
		JSONNode jSONNode = root["invitation_sender"];
		JSONNode jSONNode2 = null;
		if (jSONNode != null)
		{
			jSONNode2 = jSONNode["invitation_event"];
			if (jSONNode2 != null)
			{
				eventId = jSONNode2["id"];
				receiverRewardCode = jSONNode2["receiver_reward"];
			}
		}
		invitaionUrl = jSONNode["invitation_url"];
		hostUserId = jSONNode["user_id"];
		hostProfileImageUrl = jSONNode["profile_image_url"];
		hostNickName = jSONNode["nickname"];
		totalRegisterdUserFromHost = jSONNode["total_receivers_count"].AsInt;
	}
}
