using System.Collections;
using UnityEngine;

public class DebugMenuWebView : DebugMenuBase
{
	private enum eItem
	{
		URL = 0,
		Open = 1,
		Close = 2,
		Reload = 3,
		Result = 4,
		MarginTop = 5,
		MarginBottom = 6,
		MarginLeft = 7,
		MarginRight = 8,
		Max = 9
	}

	private enum eMargin
	{
		Top = 0,
		Bottom = 1,
		Left = 2,
		Right = 3,
		Max = 4
	}

	private enum eOS
	{
		iPhone4S = 0,
		iPod5th = 1,
		iPad3 = 2
	}

	private static int[] result_ = new int[4];

	private WebViewObject webViewObject_;

	private static string url_ = "https://www.google.co.jp";

	private static int[] margins_ = new int[4] { 170, 250, 100, 100 };

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(9, "WebView"));
	}

	public override void OnDraw()
	{
		DrawItem(0, string.Empty, eItemType.TextField);
		DrawItem(1, "Open", eItemType.CenterOnly);
		DrawItem(2, "Close", eItemType.CenterOnly);
		DrawItem(3, "Reload", eItemType.CenterOnly);
		for (int i = 5; i < 9; i++)
		{
			int num = i - 5;
			DrawItem(i, string.Empty + ((eMargin)num).ToString() + ":" + result_[num]);
		}
	}

	public override void OnExecute()
	{
		url_ = VaryString(0, url_);
		if (IsPressCenterButton(1))
		{
			close();
			webViewObject_ = new GameObject("WebViewObject").AddComponent<WebViewObject>();
			webViewObject_.Init(delegate(string msg)
			{
				Debug.Log(string.Format("CallFromJS[{0}]", msg));
			});
			webViewObject_.LoadURL(url_);
			webViewObject_.SetVisibility(true);
			webViewObject_.SetMargins(margins_[2], margins_[0], margins_[3], margins_[1]);
			RuntimePlatform platform = Application.platform;
			if (platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer || platform == RuntimePlatform.IPhonePlayer)
			{
				webViewObject_.EvaluateJS("window.addEventListener('load', function() {\twindow.Unity = {\t\tcall:function(msg) {\t\t\tvar iframe = document.createElement('IFRAME');\t\t\tiframe.setAttribute('src', 'unity:' + msg);\t\t\tdocument.documentElement.appendChild(iframe);\t\t\tiframe.parentNode.removeChild(iframe);\t\t\tiframe = null;\t\t}\t}}, false);");
			}
			webViewObject_.EvaluateJS("window.addEventListener('load', function() {\twindow.addEventListener('click', function() {\t\tUnity.call('clicked');\t}, false);}, false);");
		}
		if (IsPressCenterButton(2))
		{
			close();
		}
		if (IsPressCenterButton(3) && webViewObject_ != null)
		{
			webViewObject_.Reload();
		}
		for (int i = 5; i < 9; i++)
		{
			int num = i - 5;
			margins_[num] = (int)Vary(i, margins_[num], 1, 0, 1000);
		}
	}

	private void OnDestroy()
	{
		close();
	}

	private void close()
	{
		if (webViewObject_ != null)
		{
			Object.Destroy(webViewObject_.gameObject);
			webViewObject_ = null;
		}
	}
}
