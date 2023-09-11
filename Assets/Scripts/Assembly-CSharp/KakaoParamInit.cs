using SimpleJSON;

public class KakaoParamInit : KakaoParamBase
{
	public string accessToken;

	public string refreshToken;

	public KakaoParamInit(string _accessToken, string _refreshToken)
		: base(KakaoAction.Init)
	{
		accessToken = _accessToken;
		refreshToken = _refreshToken;
	}

	public override string getParamString()
	{
		JSONClass jSONClass = makeDefaultParam();
		if (accessToken != null && accessToken.Length > 0)
		{
			jSONClass[KakaoStringKeys.Params.access_token] = accessToken;
		}
		if (refreshToken != null && refreshToken.Length > 0)
		{
			jSONClass[KakaoStringKeys.Params.refresh_token] = refreshToken;
		}
		return jSONClass.ToString();
	}
}
