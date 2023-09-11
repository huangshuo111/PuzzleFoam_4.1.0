using UnityEngine;

public class KakaoLoginView : KakaoBaseView
{
	public KakaoLoginView()
		: base(KakaoViewType.Login)
	{
	}

	public override void Render()
	{
		if (GUI.Button(new Rect(0f, 0f, Screen.width, KakaoBaseView.buttonHeight), "Login with KakaoTalk"))
		{
			KakaoNativeExtension.Instance.Login(onLoginComplete, onLoginError);
		}
	}

	private void onLoginComplete()
	{
		KakaoNativeExtension.Instance.ShowAlertMessage("Login Success!");
	}

	private void onLoginError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}
}
