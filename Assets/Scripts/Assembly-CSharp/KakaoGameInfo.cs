using System;
using SimpleJSON;

public class KakaoGameInfo
{
	private static KakaoGameInfo _instance;

	public int max_heart { get; private set; }

	public int rechargeable_heart { get; private set; }

	public int heart_regen_interval { get; private set; }

	public int game_message_interval { get; private set; }

	public int invitation_interval { get; private set; }

	public DateTime next_score_reset_time { get; private set; }

	public DateTime last_score_reset_time { get; private set; }

	public double last_score_reset_timestamp { get; private set; }

	public double next_score_reset_timestamp { get; private set; }

	public string min_version_for_ios { get; private set; }

	public string current_version_for_ios { get; private set; }

	public string min_version_for_android { get; private set; }

	public string current_version_for_android { get; private set; }

	public string notice { get; private set; }

	public static KakaoGameInfo Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new KakaoGameInfo();
			}
			return _instance;
		}
	}

	private void clear()
	{
		max_heart = 0;
		rechargeable_heart = 0;
		heart_regen_interval = 0;
		game_message_interval = 0;
		invitation_interval = 0;
		next_score_reset_time = DateTime.MinValue;
		last_score_reset_time = DateTime.MinValue;
		last_score_reset_timestamp = 0.0;
		next_score_reset_timestamp = 0.0;
		min_version_for_ios = null;
		current_version_for_ios = null;
		min_version_for_android = null;
		current_version_for_android = null;
		notice = null;
	}

	public void setGameInfoFromJSON(JSONNode root)
	{
		clear();
		Debug.Log("Parse \"GameInfo in Leaderboard\"!");
		max_heart = root["max_heart"].AsInt;
		rechargeable_heart = root["rechargeable_heart"].AsInt;
		heart_regen_interval = root["heart_regen_interval"].AsInt;
		game_message_interval = root["game_message_interval"].AsInt;
		invitation_interval = root["invitation_interval"].AsInt;
		string s = root["next_score_reset_time"].Value.ToString();
		DateTime result;
		DateTime.TryParse(s, out result);
		next_score_reset_time = result;
		s = root["last_score_reset_time"].Value.ToString();
		DateTime.TryParse(s, out result);
		last_score_reset_time = result;
		last_score_reset_timestamp = root["last_score_reset_timestamp"].AsDouble;
		next_score_reset_timestamp = root["next_score_reset_timestamp"].AsDouble;
		min_version_for_ios = root["min_version_for_ios"].Value.ToString();
		current_version_for_ios = root["current_version_for_ios"].Value.ToString();
		min_version_for_android = root["min_version_for_android"].Value.ToString();
		current_version_for_android = root["current_version_for_android"].Value.ToString();
		notice = root["notice"].Value.ToString();
		JSONNode jSONNode = root["leaderboards"];
		int count = jSONNode.Count;
		for (int i = 0; i < count; i++)
		{
			JSONNode jSONNode2 = jSONNode[i];
			if (!(jSONNode2 == null))
			{
				KakaoLeaderboards.Instance.setLeaderboard(jSONNode2["key"]);
			}
		}
	}
}
