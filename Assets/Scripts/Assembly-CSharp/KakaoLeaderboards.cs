using System;
using System.Collections.Generic;
using SimpleJSON;

public class KakaoLeaderboards
{
	public class Leaderboard
	{
		public class Score
		{
			public int best_score;

			public int season_score;

			public int last_season_score;

			public Score(int bestScore, int seasonScore, int lastSeasonScore)
			{
				best_score = bestScore;
				season_score = seasonScore;
				last_season_score = lastSeasonScore;
			}
		}

		public Dictionary<string, Score> scores = new Dictionary<string, Score>();

		public void setScore(string userId, int bestScore, int seasonScore, int lastSeasonScore)
		{
			if (scores.ContainsKey(userId))
			{
				scores.Remove(userId);
			}
			scores.Add(userId, new Score(bestScore, seasonScore, lastSeasonScore));
		}
	}

	private static KakaoLeaderboards _instance;

	public Dictionary<string, Leaderboard> leaderboards = new Dictionary<string, Leaderboard>(StringComparer.OrdinalIgnoreCase);

	public static KakaoLeaderboards Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new KakaoLeaderboards();
			}
			return _instance;
		}
	}

	public void setLeaderboard(string key)
	{
		if (leaderboards.ContainsKey(key))
		{
			leaderboards.Remove(key);
		}
		Leaderboard value = new Leaderboard();
		leaderboards.Add(key, value);
	}

	public void clear(string leaderboardKey)
	{
		if (leaderboards.ContainsKey(leaderboardKey))
		{
			leaderboards.Remove(leaderboardKey);
		}
	}

	public void setLeaderboardsFromJSON(JSONNode root)
	{
		string text = root[KakaoStringKeys.Parsers.Leaderboard.leaderboardKey].Value.ToString();
		Leaderboard leaderboard = null;
		leaderboard = (leaderboards.ContainsKey(text) ? leaderboards[text] : new Leaderboard());
		if (text != null && text.Length > 0)
		{
			clear(text);
		}
		JSONArray asArray = root[KakaoStringKeys.Parsers.Leaderboard.appFriends].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode jSONNode = asArray[i];
			if (!(jSONNode == null))
			{
				string userId = jSONNode[KakaoStringKeys.Parsers.userId].Value.ToString();
				int asInt = jSONNode[KakaoStringKeys.Parsers.Leaderboard.bestScore].AsInt;
				int asInt2 = jSONNode[KakaoStringKeys.Parsers.Leaderboard.lastSeasonScore].AsInt;
				int asInt3 = jSONNode[KakaoStringKeys.Parsers.Leaderboard.seasonScore].AsInt;
				leaderboard.setScore(userId, asInt, asInt3, asInt2);
			}
		}
		leaderboards.Add(text, leaderboard);
	}

	public void printToConsole()
	{
		foreach (string key in leaderboards.Keys)
		{
			Debug.Log("***********************************************");
			Debug.Log(string.Format("Leaderboard Key : {0}", key));
			Leaderboard leaderboard = leaderboards[key];
			if (leaderboard == null)
			{
				continue;
			}
			Debug.Log("Scores in Leaderboard");
			foreach (string key2 in leaderboard.scores.Keys)
			{
				if (key2 != null || key2.Length != 0)
				{
					Leaderboard.Score score = leaderboard.scores[key2];
					Debug.Log(string.Format("{0}'s scores : {1}/{2}/{3}", key2, score.season_score, score.best_score, score.last_season_score));
				}
			}
		}
	}
}
