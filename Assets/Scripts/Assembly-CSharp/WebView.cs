using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebView
{
	private enum eMargin
	{
		Top = 0,
		Bottom = 1,
		Left = 2,
		Right = 3,
		Max = 4
	}

	public enum eWebParameterType
	{
		game_id = 2,
		app_id = 4,
		nick = 8,
		version = 0x10,
		os = 0x20,
		os_version = 0x40,
		model = 0x80,
		review = 0x100
	}

	public enum eWebType
	{
		Notice = 0,
		Help = 1,
		ContactUs = 2,
		SCTL = 3,
		EFTA = 4,
		Market = 5,
		GumiHub = 6,
		Event = 7
	}

	private static WebView _instance;

	protected WebViewObject webViewObject_;

	private int[] margins_ = new int[4];

	private bool bPlaySE_;

	private int id_;

	private readonly string androidmarketURL_ = "market://details?id=com.gumikorea.games.puzzlebubbleforkakao";

	private readonly string iosmarketURL_ = "https://itunes.apple.com/kr/app/peojeulbeobeul-for-kakao/id825458984?mt=8&uo=4";

	private readonly string gumihubURL = "http://www.naver.com";

	public readonly string noticeurl = "http://14.63.170.51:10080/information";

	public readonly string ios_noticeurl = "http://14.63.170.51:10080/information";

	public readonly string faq_url = "http://14.63.170.51:10080/view/a/pb/faq";

	public readonly string cs_url = "http://14.63.170.51:10080/contact";

	public readonly string event_url = "http://14.63.170.51:10080/event";

	public int param = 36;

	public int helpparam = 502;

	private Dictionary<int, WebViewObject> webViewDic = new Dictionary<int, WebViewObject>();

	private Dictionary<int, string> webViewUrlDic = new Dictionary<int, string>();

	public static WebView Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new WebView();
			}
			return _instance;
		}
	}

	public string AddParameter(int param)
	{
		Func<string, string> func = delegate(string text)
		{
			if (text != "?")
			{
				text += "&";
			}
			return text;
		};
		string text2 = "?";
		if (((uint)param & 2u) != 0)
		{
			text2 = func(text2);
			text2 += "game=pb";
		}
		if (((uint)param & 4u) != 0)
		{
			text2 = func(text2);
			text2 = text2 + "user_id=" + GlobalData.Instance.LineID;
		}
		if (((uint)param & 8u) != 0)
		{
			text2 = func(text2);
			text2 = text2 + "nick=" + DummyPlayerData.Data.UserName;
		}
		if (((uint)param & 0x10u) != 0)
		{
			text2 = func(text2);
			text2 = text2 + "version=" + SaveData.Instance.getAppVersion();
		}
		if (((uint)param & 0x20u) != 0)
		{
			text2 = func(text2);
			text2 += "channel=1";
		}
		if (((uint)param & 0x40u) != 0)
		{
			text2 = func(text2);
			text2 = text2 + "os_version=" + SystemInfo.operatingSystem;
		}
		if (((uint)param & 0x80u) != 0)
		{
			text2 = func(text2);
			text2 = text2 + "model=" + SystemInfo.deviceModel;
		}
		if (GlobalData.Instance.getGameData().iadFlg && ((uint)param & 0x100u) != 0)
		{
			text2 = func(text2);
			text2 = text2 + "review=" + true;
		}
		return text2.Replace(" ", "_");
	}

	private IEnumerator CreateWebVeiw(eWebType id)
	{
		switch (id)
		{
		case eWebType.Notice:
			preLoad(noticeurl + AddParameter(helpparam), id);
			break;
		case eWebType.ContactUs:
			preLoad(cs_url + AddParameter(helpparam), id);
			break;
		case eWebType.SCTL:
			preLoad("http://line.navere.jp/ebiz_rules/en/", id);
			break;
		case eWebType.Help:
			preLoad(faq_url + AddParameter(param), id);
			break;
		case eWebType.GumiHub:
			preLoad(gumihubURL, eWebType.GumiHub);
			break;
		case eWebType.Event:
			preLoad(event_url + AddParameter(helpparam), id);
			break;
		}
		yield break;
	}

	public void preLoad(string url, eWebType id)
	{
		WebViewObject webViewObject;
		if (webViewDic.ContainsKey((int)id) && webViewDic[(int)id] != null)
		{
			webViewObject = webViewDic[(int)id];
			webViewUrlDic[(int)id] = url;
		}
		else
		{
			webViewObject = new GameObject("WebViewObject").AddComponent<WebViewObject>();
			webViewObject.Init(WebViewCallback, true);
			if (webViewDic.ContainsKey((int)id))
			{
				webViewDic[(int)id] = webViewObject;
			}
			else
			{
				webViewDic.Add((int)id, webViewObject);
				webViewUrlDic.Add((int)id, url);
			}
			webViewObject.SetMargins(margins_[2], margins_[0], margins_[3], margins_[1]);
		}
		webViewObject.SetVisibility(false);
	}

	private void ShowMarket()
	{
		bPlaySE_ = false;
		id_ = 5;
		Sound.Instance.playSe(Sound.eSe.SE_220_count);
		bPlaySE_ = true;
		Application.OpenURL(androidmarketURL_);
	}

	public IEnumerator show(eWebType id, DialogManager dialogManager)
	{
		closeWebView();
		switch (id)
		{
		case eWebType.Market:
			ShowMarket();
			yield break;
		case eWebType.GumiHub:
			Application.OpenURL(gumihubURL);
			yield break;
		}
		if (!webViewDic.ContainsKey((int)id) || webViewDic[(int)id] == null)
		{
			yield return dialogManager.StartCoroutine(CreateWebVeiw(id));
		}
		if (!webViewDic.ContainsKey((int)id))
		{
			Debug.LogError(id.ToString() + " Empty");
			yield break;
		}
		bPlaySE_ = false;
		id_ = (int)id;
		Sound.Instance.playSe(Sound.eSe.SE_220_count);
		bPlaySE_ = true;
		webViewObject_ = webViewDic[(int)id];
		webViewObject_.LoadURL(webViewUrlDic[(int)id]);
		webViewObject_.SetVisibility(true);
		webViewObject_.EvaluateJS("window.addEventListener('load', function() {\twindow.addEventListener('click', function() {\t\tUnity.call('clicked');\t}, false);}, false);");
		Input.enable = false;
		yield return dialogManager.StartCoroutine(updateLoadIcon());
		Input.enable = true;
	}

	private void OnCheckBox(bool bActive)
	{
		if (bPlaySE_)
		{
			Constant.SoundUtil.PlayButtonSE();
		}
	}

	protected IEnumerator updateLoadIcon()
	{
		while (webViewObject_ != null)
		{
			yield return null;
		}
	}

	protected void closeWebView()
	{
		if (webViewObject_ != null)
		{
			webViewObject_.SetVisibility(false);
			webViewObject_ = null;
		}
	}

	protected void clearWebView()
	{
		Dictionary<int, WebViewObject>.Enumerator enumerator = webViewDic.GetEnumerator();
		int num = 0;
		while (enumerator.MoveNext())
		{
			webViewObject_ = enumerator.Current.Value;
			if (webViewObject_ != null)
			{
				UnityEngine.Object.Destroy(webViewObject_.gameObject);
				webViewObject_ = null;
			}
		}
		webViewDic.Clear();
		webViewUrlDic.Clear();
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
		webViewObject_.SetVisibility(bShow);
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

	public void WebViewCallback(string result)
	{
		if (result.Equals("Close"))
		{
			closeWebView();
		}
	}

	public static int getResponseCode(WWW request)
	{
		int result = 0;
		if (request.responseHeaders == null)
		{
			Debug.LogError(" no response headers.");
		}
		else if (!request.responseHeaders.ContainsKey("STATUS"))
		{
			foreach (KeyValuePair<string, string> responseHeader in request.responseHeaders)
			{
				if (responseHeader.Value != null && responseHeader.Value.StartsWith("HTTP/1."))
				{
					result = parseResponseCode(responseHeader.Value);
				}
			}
		}
		else
		{
			result = parseResponseCode(request.responseHeaders["STATUS"]);
		}
		return result;
	}

	public static int parseResponseCode(string statusLine)
	{
		int result = 0;
		string[] array = statusLine.Split(' ');
		if (array.Length < 3)
		{
			Debug.LogError(" invalid response status: " + statusLine);
		}
		else if (!int.TryParse(array[1], out result))
		{
			Debug.LogError(" invalid response code: " + array[1]);
		}
		return result;
	}

	public static IEnumerator NoticeRequest(WWW NoticeWWW)
	{
		yield return NoticeWWW;
		if (NoticeWWW.error == null)
		{
			Debug.Log("NoticeWWW Ok!: " + NoticeWWW.text);
		}
		else
		{
			Debug.Log("NoticeWWW Error: " + NoticeWWW.error);
		}
	}
}
