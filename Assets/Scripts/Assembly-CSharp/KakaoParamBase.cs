using SimpleJSON;

public class KakaoParamBase
{
	protected KakaoAction action;

	public KakaoParamBase(KakaoAction _action)
	{
		action = _action;
	}

	public KakaoAction getAction()
	{
		return action;
	}

	protected JSONClass makeDefaultParam()
	{
		JSONClass jSONClass = new JSONClass();
		jSONClass[KakaoStringKeys.Params.action] = action.ToString();
		return jSONClass;
	}

	public virtual string getParamString()
	{
		JSONClass jSONClass = makeDefaultParam();
		return jSONClass.ToString();
	}
}
