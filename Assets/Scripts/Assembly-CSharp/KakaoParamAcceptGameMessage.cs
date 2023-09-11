using SimpleJSON;

public class KakaoParamAcceptGameMessage : KakaoParamBase
{
	private string id;

	public KakaoParamAcceptGameMessage(string _id)
		: base(KakaoAction.AcceptGameMessage)
	{
		id = _id;
	}

	public override string getParamString()
	{
		JSONClass jSONClass = makeDefaultParam();
		if (id != null)
		{
			jSONClass[KakaoStringKeys.Params.Leaderboard.messageId] = id;
		}
		return jSONClass.ToString();
	}
}
