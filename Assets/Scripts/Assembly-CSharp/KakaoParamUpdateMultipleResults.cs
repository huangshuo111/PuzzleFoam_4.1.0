using System;
using System.Collections.Generic;
using SimpleJSON;

public class KakaoParamUpdateMultipleResults : KakaoParamBase
{
	private Dictionary<string, int> scores;

	private int exp;

	private byte[] publicData;

	private byte[] privateData;

	public KakaoParamUpdateMultipleResults(Dictionary<string, int> _scores, int _exp, byte[] _publicData, byte[] _privateData)
		: base(KakaoAction.UpdateMultipleResults)
	{
		scores = _scores;
		exp = _exp;
		publicData = _publicData;
		privateData = _privateData;
	}

	public override string getParamString()
	{
		JSONClass jSONClass = makeDefaultParam();
		if (scores != null)
		{
			foreach (string key in scores.Keys)
			{
				jSONClass[KakaoStringKeys.Params.Leaderboard.multipleLeaderboards][key] = scores[key].ToString();
			}
		}
		jSONClass[KakaoStringKeys.Params.Leaderboard.exp] = exp.ToString();
		if (publicData != null)
		{
			jSONClass[KakaoStringKeys.Params.Leaderboard.publicData] = Convert.ToBase64String(publicData);
		}
		if (privateData != null)
		{
			jSONClass[KakaoStringKeys.Params.Leaderboard.privateData] = Convert.ToBase64String(privateData);
		}
		return jSONClass.ToString();
	}
}
