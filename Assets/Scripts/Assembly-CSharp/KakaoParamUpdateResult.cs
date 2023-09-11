using System.Text;
using SimpleJSON;

public class KakaoParamUpdateResult : KakaoParamBase
{
	private string leaderboardKey;

	private int score;

	private int exp;

	private byte[] publicData;

	private byte[] privateData;

	public KakaoParamUpdateResult(string _leaderboardKey, int _score, int _exp, byte[] _publicData, byte[] _privateData)
		: base(KakaoAction.UpdateResult)
	{
		leaderboardKey = _leaderboardKey;
		score = _score;
		exp = _exp;
		publicData = _publicData;
		privateData = _privateData;
	}

	public override string getParamString()
	{
		JSONClass jSONClass = makeDefaultParam();
		jSONClass[KakaoStringKeys.Params.Leaderboard.leaderboardKey] = leaderboardKey;
		jSONClass[KakaoStringKeys.Params.Leaderboard.score] = score.ToString();
		jSONClass[KakaoStringKeys.Params.Leaderboard.exp] = exp.ToString();
		if (publicData != null)
		{
			jSONClass[KakaoStringKeys.Params.Leaderboard.publicData] = Encoding.UTF8.GetString(publicData);
		}
		if (privateData != null)
		{
			jSONClass[KakaoStringKeys.Params.Leaderboard.privateData] = Encoding.UTF8.GetString(privateData);
		}
		return jSONClass.ToString();
	}
}
