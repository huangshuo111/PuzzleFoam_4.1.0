using System.Collections;
using UnityEngine;

public class DialogGumiHub : DialogAppQuit
{
	private enum eMargin
	{
		Top = 0,
		Bottom = 1,
		Left = 2,
		Right = 3,
		Max = 4
	}

	protected WebViewObject webViewObject1_;

	private int[] margins_ = new int[4] { 10, 232, 18, 99 };

	private bool bPlaySE_;

	private readonly string GumiHubBannerURL = "http://www.naver.com";

	public new bool previewPause;

	public override void OnCreate()
	{
		calcMargine(ref margins_);
		preLoad();
	}

	public void preLoad()
	{
		webViewObject1_ = new GameObject("WebViewObject_GumiHub").AddComponent<WebViewObject>();
		webViewObject1_.Init();
		webViewObject1_.LoadURL(GumiHubBannerURL);
		webViewObject1_.SetMargins(margins_[2], margins_[0], margins_[3], margins_[1]);
		webViewObject1_.SetVisibility(false);
	}

	public override IEnumerator open()
	{
		partManager_.inhibitTips(true);
		DialogInformation infoDialog = dialogManager_.getDialog(DialogManager.eDialog.Information) as DialogInformation;
		if (infoDialog != null && infoDialog.isOpen())
		{
			infoDialog.showWebView(false);
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(infoDialog));
		}
		yield return dialogManager_.StartCoroutine(base.open());
		webViewObject1_.SetVisibility(true);
		webViewObject1_.EvaluateJS("window.addEventListener('load', function() {\twindow.addEventListener('click', function() {\t\tUnity.call('clicked');\t}, false);}, false);");
	}

	protected override IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "ConfirmButton":
			Constant.SoundUtil.PlayDecideSE();
			closeWebView();
			fadeManager_.setActive(FadeMng.eType.AllMask, true);
			yield return StartCoroutine(fadeManager_.startFade(FadeMng.eType.AllMask, 0f, 1f, 0.1f));
			yield return new WaitForEndOfFrame();
			Application.Quit();
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			showWebView(false);
			if (partManager_.currentPart == PartManager.ePart.Stage)
			{
				((Part_Stage)partManager_.execPart).stagePause.pause = previewPause;
			}
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			partManager_.inhibitTips(false);
			break;
		case "ShowOtherGame":
			showWebView(false);
			StartCoroutine(WebView.Instance.show(WebView.eWebType.GumiHub, dialogManager_));
			break;
		}
	}

	protected void closeWebView()
	{
		if (webViewObject1_ != null)
		{
			Object.Destroy(webViewObject1_.gameObject);
			webViewObject1_ = null;
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (isOpen() && webViewObject1_ != null)
		{
			webViewObject1_.SetVisibility(!pause);
		}
	}

	public void showWebView(bool bShow)
	{
		webViewObject1_.SetVisibility(bShow);
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
		margins[0] = (int)((float)margins[0] * vector3.y + zero.y * vector3.y);
		margins[1] = (int)((float)margins[1] * vector3.y + zero.y * vector3.y);
		margins[2] = (int)((float)margins[2] * vector3.x + zero.x * vector3.x);
		margins[3] = (int)((float)margins[3] * vector3.x + zero.x * vector3.x);
	}
}
