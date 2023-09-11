public abstract class KakaoBaseView
{
	public KakaoViewType type;

	protected static readonly int buttonHeight = 80;

	public KakaoBaseView(KakaoViewType _type)
	{
		type = _type;
	}

	protected void showAlertErrorMessage(string code, string message)
	{
		string text = string.Empty;
		if (code != null)
		{
			text = text + "Error Code : " + code;
		}
		if (message != null)
		{
			text += "\n";
			text = text + "Error Message : " + message;
		}
		KakaoNativeExtension.Instance.ShowAlertMessage(text);
	}

	public abstract void Render();
}
