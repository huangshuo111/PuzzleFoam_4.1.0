using System.Collections.Generic;
using System.Text;
using SimpleJSON;

public class KakaoGameFriends
{
	public class LeaderboardFriend
	{
		public string nickname { get; private set; }

		public string friendNickName { get; private set; }

		public string userid { get; private set; }

		public string profileImageUrl { get; private set; }

		public int exp { get; private set; }

		public double lastMessageSentAt { get; set; }

		public bool messageBlocked { get; private set; }

		public bool supportedDevice { get; private set; }

		public int rank { get; private set; }

		public int bestScore { get; private set; }

		public int seasonScore { get; private set; }

		public int lastSeasonScore { get; private set; }

		public byte[] publicData { get; private set; }

		public LeaderboardFriend(JSONNode friend)
		{
			friendNickName = friend[KakaoStringKeys.Parsers.friendNickName].Value.ToString();
			nickname = friend[KakaoStringKeys.Parsers.nickName].Value.ToString();
			profileImageUrl = friend[KakaoStringKeys.Parsers.profileImageUrl].Value.ToString();
			userid = friend[KakaoStringKeys.Parsers.userId].Value.ToString();
			exp = friend[KakaoStringKeys.Parsers.Leaderboard.exp].AsInt;
			lastMessageSentAt = friend[KakaoStringKeys.Parsers.Leaderboard.lastMessageSentAt].AsDouble;
			messageBlocked = friend[KakaoStringKeys.Parsers.messageBlocked].AsBool;
			supportedDevice = friend[KakaoStringKeys.Parsers.Leaderboard.supportedDevice].AsBool;
			rank = friend[KakaoStringKeys.Parsers.Leaderboard.rank].AsInt;
			bestScore = friend[KakaoStringKeys.Parsers.Leaderboard.bestScore].AsInt;
			seasonScore = friend[KakaoStringKeys.Parsers.Leaderboard.seasonScore].AsInt;
			lastSeasonScore = friend[KakaoStringKeys.Parsers.Leaderboard.lastSeasonScore].AsInt;
			string text = friend[KakaoStringKeys.Parsers.Leaderboard.publicData].Value.ToString();
			if (text != null && text.Length > 0)
			{
				publicData = Encoding.UTF8.GetBytes(text);
			}
		}
	}

	public class KakaotalkFriend
	{
		public string nickname { get; private set; }

		public string friendNickName { get; private set; }

		public string userid { get; private set; }

		public string profileImageUrl { get; private set; }

		public bool supportedDevice { get; private set; }

		public double lastMessageSentAt { get; set; }

		public bool messageBlocked { get; private set; }

		public KakaotalkFriend(JSONNode friend)
		{
			friendNickName = friend[KakaoStringKeys.Parsers.friendNickName].Value.ToString();
			nickname = friend[KakaoStringKeys.Parsers.nickName].Value.ToString();
			profileImageUrl = friend[KakaoStringKeys.Parsers.profileImageUrl].Value.ToString();
			userid = friend[KakaoStringKeys.Parsers.userId].Value.ToString();
			lastMessageSentAt = friend[KakaoStringKeys.Parsers.Leaderboard.lastMessageSentAt].AsDouble;
			string text = friend[KakaoStringKeys.Parsers.messageBlocked].Value.ToString();
			if (text != null && string.Equals(text, "true"))
			{
				messageBlocked = true;
			}
			else
			{
				messageBlocked = false;
			}
			string text2 = friend[KakaoStringKeys.Parsers.supportedDevice].Value.ToString();
			if (text2 != null && string.Equals(text2, "true"))
			{
				supportedDevice = true;
			}
			else
			{
				supportedDevice = false;
			}
		}
	}

	private static KakaoGameFriends _instance;

	public Dictionary<string, LeaderboardFriend> leaderboardFriends = new Dictionary<string, LeaderboardFriend>();

	public Dictionary<string, KakaotalkFriend> kakaotalkFriends = new Dictionary<string, KakaotalkFriend>();

	public static KakaoGameFriends Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new KakaoGameFriends();
			}
			return _instance;
		}
	}

	public void clear()
	{
		leaderboardFriends.Clear();
		kakaotalkFriends.Clear();
	}

	public void setGameFriendFromJSON(JSONNode root)
	{
		clear();
		JSONArray asArray = root[KakaoStringKeys.Parsers.Leaderboard.appFriends].AsArray;
		int count = asArray.Count;
		for (int i = 0; i < count; i++)
		{
			JSONNode jSONNode = asArray[i];
			if (jSONNode == null)
			{
				Debug.LogWarning("app_friends_info is null");
				continue;
			}
			LeaderboardFriend leaderboardFriend = new LeaderboardFriend(jSONNode);
			leaderboardFriends.Add(leaderboardFriend.userid, leaderboardFriend);
		}
		JSONArray asArray2 = root[KakaoStringKeys.Parsers.Leaderboard.friends].AsArray;
		count = asArray2.Count;
		for (int j = 0; j < count; j++)
		{
			JSONNode jSONNode2 = asArray2[j];
			if (jSONNode2 == null)
			{
				Debug.LogWarning("friends_info data is null");
				continue;
			}
			KakaotalkFriend kakaotalkFriend = new KakaotalkFriend(jSONNode2);
			kakaotalkFriends.Add(kakaotalkFriend.userid, kakaotalkFriend);
		}
	}

	public void updateGameFriendsWithJSON(JSONNode node)
	{
		string key = node[KakaoStringKeys.Parsers.Leaderboard.receiverId].Value.ToString();
		double asDouble = node[KakaoStringKeys.Parsers.Leaderboard.messageSentAt].AsDouble;
		if (leaderboardFriends.ContainsKey(key))
		{
			LeaderboardFriend leaderboardFriend = leaderboardFriends[key];
			leaderboardFriend.lastMessageSentAt = asDouble;
			leaderboardFriends[key] = leaderboardFriend;
		}
		else if (kakaotalkFriends.ContainsKey(key))
		{
			KakaotalkFriend kakaotalkFriend = kakaotalkFriends[key];
			kakaotalkFriend.lastMessageSentAt = asDouble;
			kakaotalkFriends[key] = kakaotalkFriend;
		}
	}

	public void printToConsole()
	{
		Debug.Log("Print LeaderboardFriend Information.");
		foreach (KeyValuePair<string, LeaderboardFriend> leaderboardFriend in leaderboardFriends)
		{
			LeaderboardFriend value = leaderboardFriend.Value;
			if (value != null)
			{
				Debug.Log(string.Format("Name:{0} / Rank:{1} / Best Score:{2}", value.nickname, value.rank.ToString(), value.bestScore.ToString()));
			}
		}
		Debug.Log("Print KakaotalkFriend Information.");
		foreach (KeyValuePair<string, KakaotalkFriend> kakaotalkFriend in kakaotalkFriends)
		{
			KakaotalkFriend value2 = kakaotalkFriend.Value;
			if (value2 != null)
			{
				Debug.Log(string.Format("Name:{0} / messageBlocked:{1}", value2.nickname, (!value2.messageBlocked) ? "false" : "true"));
			}
		}
	}

	public LeaderboardFriend getLeaderboardFriend(string key)
	{
		if (leaderboardFriends == null)
		{
			return null;
		}
		if (key == null || key.Length == 0)
		{
			return null;
		}
		if (!leaderboardFriends.ContainsKey(key))
		{
			return null;
		}
		return leaderboardFriends[key];
	}

	public KakaotalkFriend getKakaotalkFriend(string key)
	{
		if (kakaotalkFriends == null)
		{
			return null;
		}
		if (key == null || key.Length == 0)
		{
			return null;
		}
		if (!kakaotalkFriends.ContainsKey(key))
		{
			return null;
		}
		return kakaotalkFriends[key];
	}
}
