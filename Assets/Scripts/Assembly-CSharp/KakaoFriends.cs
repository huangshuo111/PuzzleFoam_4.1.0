using System.Collections.Generic;
using SimpleJSON;

public class KakaoFriends
{
	public class Friend
	{
		public string nickname { get; private set; }

		public string userid { get; private set; }

		public string profileImageUrl { get; private set; }

		public bool messageBlocked { get; private set; }

		public bool supportedDevice { get; private set; }

		public Friend(JSONNode friend)
		{
			nickname = friend[KakaoStringKeys.Parsers.nickName].Value.ToString();
			userid = friend[KakaoStringKeys.Parsers.userId].Value.ToString();
			profileImageUrl = friend[KakaoStringKeys.Parsers.profileImageUrl].Value.ToString();
			messageBlocked = friend[KakaoStringKeys.Parsers.messageBlocked].AsBool;
			supportedDevice = friend[KakaoStringKeys.Parsers.supportedDevice].AsBool;
		}
	}

	private static KakaoFriends _instance;

	public List<Friend> appFriends = new List<Friend>();

	public List<Friend> friends = new List<Friend>();

	public static KakaoFriends Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new KakaoFriends();
			}
			return _instance;
		}
	}

	public void clear()
	{
		if (appFriends != null)
		{
			appFriends.Clear();
		}
		if (friends != null)
		{
			friends.Clear();
		}
	}

	public bool hasFriends()
	{
		if (appFriends.Count > 0)
		{
			return true;
		}
		if (friends.Count > 0)
		{
			return true;
		}
		return false;
	}

	public void setFriendsFromJSON(JSONNode root)
	{
		appFriends.Clear();
		friends.Clear();
		Debug.Log("Parse \"app_friends_info\"!");
		JSONNode jSONNode = root[KakaoStringKeys.Parsers.appFriendsInfo];
		int count = jSONNode.Count;
		for (int i = 0; i < count; i++)
		{
			JSONNode jSONNode2 = jSONNode[i];
			if (jSONNode2 == null)
			{
				Debug.LogWarning("app_friends_info is null");
				continue;
			}
			Friend item = new Friend(jSONNode2);
			appFriends.Add(item);
		}
		Debug.Log("Parse \"friends_info\"!");
		JSONNode jSONNode3 = root[KakaoStringKeys.Parsers.friendsInfo];
		count = jSONNode3.Count;
		for (int j = 0; j < count; j++)
		{
			JSONNode jSONNode4 = jSONNode3[j];
			if (jSONNode4 == null)
			{
				Debug.LogWarning("friends_info data is null");
				continue;
			}
			Friend item2 = new Friend(jSONNode4);
			friends.Add(item2);
		}
	}
}
