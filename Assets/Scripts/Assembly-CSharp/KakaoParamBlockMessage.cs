using SimpleJSON;

public class KakaoParamBlockMessage : KakaoParamBase
{
	private bool blockMessage;

	public KakaoParamBlockMessage(bool block)
		: base(KakaoAction.BlockMessage)
	{
		blockMessage = block;
	}

	public override string getParamString()
	{
		JSONClass jSONClass = makeDefaultParam();
		jSONClass[KakaoStringKeys.Params.Leaderboard.block] = ((!blockMessage) ? "false" : "true");
		return jSONClass.ToString();
	}
}
