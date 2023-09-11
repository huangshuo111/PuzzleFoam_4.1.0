using SimpleJSON;

public class KakaoParamUseHeart : KakaoParamBase
{
	private int useHeart;

	public KakaoParamUseHeart(int _useHeart)
		: base(KakaoAction.UseHeart)
	{
		useHeart = _useHeart;
	}

	public override string getParamString()
	{
		JSONClass jSONClass = makeDefaultParam();
		jSONClass[KakaoStringKeys.Params.Leaderboard.useHeart] = useHeart.ToString();
		return jSONClass.ToString();
	}
}
