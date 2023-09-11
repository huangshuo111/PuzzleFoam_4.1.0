using System.Collections.Generic;
using System.Text;
using SimpleJSON;

public class KakaoGameUserInfo
{
	public class Score
	{
		public string leaderboard_key { get; private set; }

		public int season_score { get; private set; }

		public int last_season_score { get; private set; }

		public int best_score { get; private set; }

		public int last_score { get; private set; }

		public Score(JSONNode score)
		{
			string text = score[KakaoStringKeys.Parsers.Leaderboard.leaderboardKey].Value.ToString();
			leaderboard_key = text;
			string text2 = score[KakaoStringKeys.Parsers.Leaderboard.seasonScore].Value.ToString();
			int result = 0;
			if (text2 != null && text2.Length > 0)
			{
				int.TryParse(text2, out result);
			}
			season_score = result;
			string text3 = score[KakaoStringKeys.Parsers.Leaderboard.lastSeasonScore].Value.ToString();
			int result2 = 0;
			if (text3 != null && text3.Length > 0)
			{
				int.TryParse(text3, out result2);
			}
			last_season_score = result2;
			string text4 = score[KakaoStringKeys.Parsers.Leaderboard.bestScore].Value.ToString();
			int result3 = 0;
			if (text4 != null && text4.Length > 0)
			{
				int.TryParse(text4, out result3);
			}
			best_score = result3;
			string text5 = score[KakaoStringKeys.Parsers.Leaderboard.lastScore].Value.ToString();
			int result4 = 0;
			if (text5 != null && text5.Length > 0)
			{
				int.TryParse(text5, out result4);
			}
			last_score = result4;
		}
	}

	private List<Score> scores = new List<Score>();

	private static KakaoGameUserInfo _instance;

	public string user_id { get; private set; }

	public string nickname { get; private set; }

	public string profile_image_url { get; private set; }

	public bool message_blocked { get; private set; }

	public int exp { get; private set; }

	public int heart { get; private set; }

	public double heart_regen_starts_at { get; private set; }

	public double server_time { get; private set; }

	public byte[] publicData { get; private set; }

	public byte[] privateData { get; private set; }

	public int message_count { get; private set; }

	public static KakaoGameUserInfo Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new KakaoGameUserInfo();
			}
			return _instance;
		}
	}

	public void setMessageBlock(JSONNode root)
	{
		Debug.Log("Parse GameUserInfo's message block!");
		Debug.Log(root.ToString());
		string text = root[KakaoStringKeys.Parsers.messageBlocked];
		if (text != null && string.Equals("true", text))
		{
			message_blocked = true;
		}
		else
		{
			message_blocked = false;
		}
	}

	public void setGameUserInfoFromJSON(JSONNode root)
	{
		Debug.Log("Parse \"GameUserInfo in Leaderboard\"!");
		user_id = root[KakaoStringKeys.Parsers.userId];
		nickname = root[KakaoStringKeys.Parsers.nickName];
		profile_image_url = root[KakaoStringKeys.Parsers.profileImageUrl];
		message_blocked = root[KakaoStringKeys.Parsers.messageBlocked].AsBool;
		exp = root[KakaoStringKeys.Parsers.Leaderboard.exp].AsInt;
		heart = root[KakaoStringKeys.Parsers.Leaderboard.heart].AsInt;
		heart_regen_starts_at = root[KakaoStringKeys.Parsers.Leaderboard.heartRegenStartsAt].AsDouble;
		server_time = root[KakaoStringKeys.Parsers.Leaderboard.serverTime].AsDouble;
		message_count = root[KakaoStringKeys.Parsers.Leaderboard.messageCount].AsInt;
		string text = root[KakaoStringKeys.Parsers.Leaderboard.publicData];
		if (text != null && text.Length > 0)
		{
			publicData = Encoding.UTF8.GetBytes(text);
		}
		else
		{
			publicData = null;
		}
		string text2 = root[KakaoStringKeys.Parsers.Leaderboard.privateData];
		if (text2 != null && text2.Length > 0)
		{
			privateData = Encoding.UTF8.GetBytes(text2);
		}
		else
		{
			privateData = null;
		}
		scores.Clear();
		JSONArray asArray = root[KakaoStringKeys.Parsers.Leaderboard.scores].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode jSONNode = asArray[i];
			if (!(jSONNode == null))
			{
				scores.Add(new Score(jSONNode));
			}
		}
	}
}
