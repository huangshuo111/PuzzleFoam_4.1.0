using SimpleJSON;

public class KakaoParamShowAlertMessage : KakaoParamBase
{
	private string message;

	public KakaoParamShowAlertMessage(string _message)
		: base(KakaoAction.ShowAlertMessage)
	{
		message = _message;
	}

	public override string getParamString()
	{
		JSONClass jSONClass = makeDefaultParam();
		if (message != null)
		{
			jSONClass[KakaoStringKeys.Params.message] = message;
		}
		return jSONClass.ToString();
	}
}
