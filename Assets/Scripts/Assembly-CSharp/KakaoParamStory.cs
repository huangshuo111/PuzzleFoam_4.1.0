using SimpleJSON;

public class KakaoParamStory : KakaoParamBase
{
	private string message;

	private string imagePath;

	private string executeUrl;

	public KakaoParamStory(string _message, string _imagePath, string _executeUrl)
		: base(KakaoAction.PostToKakaoStory)
	{
		message = _message;
		imagePath = _imagePath;
		executeUrl = _executeUrl;
	}

	public override string getParamString()
	{
		JSONClass jSONClass = makeDefaultParam();
		jSONClass[KakaoStringKeys.Params.message] = message;
		jSONClass[KakaoStringKeys.Params.imagePath] = imagePath;
		jSONClass[KakaoStringKeys.Params.executeUrl] = executeUrl;
		return jSONClass.ToString();
	}
}
