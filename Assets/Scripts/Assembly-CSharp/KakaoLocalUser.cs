using SimpleJSON;

public class KakaoLocalUser
{
	private static KakaoLocalUser _instance;

	public static KakaoLocalUser Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new KakaoLocalUser();
			}
			return _instance;
		}
	}

	public string userId { get; private set; }

	public string nickName { get; private set; }

	public string profileImageUrl { get; private set; }

	public string hashedTalkUserId { get; private set; }

	public string countryIso { get; private set; }

	public bool messageBlocked { get; private set; }

	public bool verified { get; private set; }

	public void setLocalUserFromJSON(JSONNode root)
	{
		Debug.Log("Parse LocalUser from JSON");
		userId = root[KakaoStringKeys.Parsers.userId].Value.ToString();
		nickName = root[KakaoStringKeys.Parsers.nickName].Value.ToString();
		profileImageUrl = root[KakaoStringKeys.Parsers.profileImageUrl].Value.ToString();
		hashedTalkUserId = root[KakaoStringKeys.Parsers.hashedTalkUserId].Value.ToString();
		messageBlocked = root[KakaoStringKeys.Parsers.messageBlocked].AsBool;
		verified = root[KakaoStringKeys.Parsers.verified].AsBool;
		countryIso = root[KakaoStringKeys.Parsers.countryIso].Value.ToString();
	}
}
