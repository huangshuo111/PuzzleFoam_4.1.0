using SimpleJSON;

public class KakaoParamLoadLeaderboard : KakaoParamBase
{
	private string leaderboardKey;

	public KakaoParamLoadLeaderboard(string _leaderboardKey)
		: base(KakaoAction.LoadLeaderboard)
	{
		leaderboardKey = _leaderboardKey;
	}

	public override string getParamString()
	{
		JSONClass jSONClass = makeDefaultParam();
		if (leaderboardKey != null)
		{
			jSONClass[KakaoStringKeys.Params.Leaderboard.leaderboardKey] = leaderboardKey;
		}
		return jSONClass.ToString();
	}
}
