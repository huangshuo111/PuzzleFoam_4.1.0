using UnityEngine;

public abstract class KakaoPluginBase : ScriptableObject
{
	public abstract void request(KakaoParamBase param);
}
