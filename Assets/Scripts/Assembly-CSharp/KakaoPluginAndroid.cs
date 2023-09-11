using UnityEngine;

public class KakaoPluginAndroid : KakaoPluginBase
{
	public AndroidJavaObject activity;

	public KakaoPluginAndroid()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		activity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
	}

	public override void request(KakaoParamBase param)
	{
		activity.Call("kakaoUnityExtension", param.getParamString());
	}
}
