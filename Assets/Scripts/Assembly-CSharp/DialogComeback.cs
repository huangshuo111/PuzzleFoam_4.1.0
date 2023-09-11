using System.Collections;
using UnityEngine;

public class DialogComeback : DialogBase
{
	private enum eMargin
	{
		Top = 0,
		Bottom = 1,
		Left = 2,
		Right = 3,
		Max = 4
	}

	private const float MAX_Y_SCALE = 550f;

	protected WebViewObject webViewObject_;

	private int[] margins_ = new int[4] { 170, 245, 95, 95 };

	private Texture2D texture;

	private Transform image;

	private UICheckbox checkBox_;

	private bool bPlaySE_;

	public bool bOping;

	private bool loadSuccess;

	public override void OnCreate()
	{
		calcMargine(ref margins_);
		checkBox_ = GetComponentsInChildren<UICheckbox>(true)[0];
		checkBox_.gameObject.SetActive(false);
		base.transform.Find("window/Title_Information").gameObject.SetActive(false);
		base.transform.Find("window/title_plate").gameObject.SetActive(false);
		base.transform.Find("window/Checkbox_Label").gameObject.SetActive(false);
	}

	public void preLoad(string url)
	{
		webViewObject_ = new GameObject("WebViewObject").AddComponent<WebViewObject>();
		webViewObject_.Init();
		webViewObject_.LoadURL(url);
		webViewObject_.SetMargins(margins_[2], margins_[0], margins_[3], margins_[1]);
		webViewObject_.SetVisibility(false);
	}

	public IEnumerator show_webView(string url)
	{
		bPlaySE_ = false;
		checkBox_.isChecked = false;
		bOping = true;
		Sound.Instance.playSe(Sound.eSe.SE_220_count);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		bPlaySE_ = true;
		webViewObject_.SetVisibility(true);
		webViewObject_.EvaluateJS("window.addEventListener('load', function() {\twindow.addEventListener('click', function() {\t\tUnity.call('clicked');\t}, false);}, false);");
		bOping = false;
	}

	public IEnumerator show(string url)
	{
		bPlaySE_ = false;
		bOping = true;
		Sound.Instance.playSe(Sound.eSe.SE_220_count);
		Debug.Log("comeback_dialog : URL = " + url);
		if (loadSuccess && texture != null)
		{
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
			image = base.transform.Find("window/Image");
			UITexture tex = image.Find("image_00").GetComponent<UITexture>();
			tex.mainTexture = texture;
			tex.MakePixelPerfect();
			if (tex.transform.localScale.y > 550f)
			{
				float rate = 550f / tex.transform.localScale.y;
				tex.transform.localScale = new Vector3(tex.transform.localScale.x * rate, tex.transform.localScale.y * rate, tex.transform.localScale.z);
			}
			tex.transform.localPosition += new Vector3(0f, 0f, -5f);
			yield return null;
			image.gameObject.SetActive(true);
		}
		bPlaySE_ = true;
	}

	private void OnCheckBox(bool bActive)
	{
		if (bPlaySE_)
		{
			Constant.SoundUtil.PlayButtonSE();
		}
	}

	public IEnumerator loadTexture(string url)
	{
		int failedCount = 0;
		WWW www;
		while (true)
		{
			www = new WWW(url);
			while (!www.isDone && www.error == null)
			{
				yield return null;
			}
			if (www.error == null)
			{
				break;
			}
			www.Dispose();
			failedCount++;
			if (failedCount >= 2)
			{
				yield break;
			}
		}
		texture = www.textureNonReadable;
		loadSuccess = true;
		www.Dispose();
	}

	protected virtual IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			image.gameObject.SetActive(false);
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "ConfirmButton":
			Constant.SoundUtil.PlayDecideSE();
			image.gameObject.SetActive(false);
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	public new void close()
	{
		Object.Destroy(base.gameObject);
	}

	private void saveInformationDate()
	{
	}

	protected void closeWebView()
	{
		if (webViewObject_ != null)
		{
			Object.Destroy(webViewObject_.gameObject);
			webViewObject_ = null;
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause && !(webViewObject_ == null))
		{
			webViewObject_.SetVisibility(!pause);
		}
	}

	public void showWebView(bool bShow)
	{
		if (webViewObject_ != null)
		{
			webViewObject_.SetVisibility(bShow);
		}
	}

	private void calcMargine(ref int[] margins)
	{
		int num = NGUIUtilScalableUIRoot.manualHeight;
		int num2 = NGUIUtilScalableUIRoot.manualWidth;
		float num3 = (float)(Screen.height * NGUIUtilScalableUIRoot.manualWidth) / (float)(Screen.width * NGUIUtilScalableUIRoot.manualHeight);
		if (num3 > 1f)
		{
			num = (int)((float)num * num3);
		}
		if (num3 < 1f)
		{
			num2 = (int)((float)num2 * num3);
		}
		Vector2 zero = Vector2.zero;
		zero.y = (num - NGUIUtilScalableUIRoot.manualHeight) / 2;
		zero.x = (NGUIUtilScalableUIRoot.manualWidth - num2) / 2;
		Vector2 vector = new Vector2(Screen.width, Screen.height);
		Vector2 vector2 = new Vector2(NGUIUtilScalableUIRoot.manualWidth, NGUIUtilScalableUIRoot.manualHeight);
		vector2.y += zero.y * 2f;
		vector2.x += zero.x * 2f;
		Vector2 vector3 = new Vector2(vector.x / vector2.x, vector.y / vector2.y);
		margins[0] = (int)((float)margins_[0] * vector3.y + zero.y * vector3.y);
		margins[1] = (int)((float)margins_[1] * vector3.y + zero.y * vector3.y);
		margins[2] = (int)((float)margins_[2] * vector3.x + zero.x * vector3.x);
		margins[3] = (int)((float)margins_[3] * vector3.x + zero.x * vector3.x);
	}
}
