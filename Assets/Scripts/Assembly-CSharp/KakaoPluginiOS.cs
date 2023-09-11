using System.Runtime.InteropServices;
using AOT;

public class KakaoPluginiOS : KakaoPluginBase
{
	private delegate void ResponseCallback(bool hasError, string response);

	[DllImport("__Internal")]
	private static extern void kakaoUnityExtension(string action, ResponseCallback callback);

	[MonoPInvokeCallback(typeof(ResponseCallback))]
	private static void handleResponseCallback(bool hasError, string response)
	{
		if (!hasError)
		{
			KakaoResponseHandler.KakaoResonseComplete(response);
		}
		else
		{
			KakaoResponseHandler.KakaoResonseError(response);
		}
	}

	public override void request(KakaoParamBase param)
	{
		kakaoUnityExtension(param.getParamString(), handleResponseCallback);
	}
}
