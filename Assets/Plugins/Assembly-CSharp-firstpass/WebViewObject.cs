using System;
using UnityEngine;

public class WebViewObject : MonoBehaviour
{
	public enum eResult
	{
		None = 0,
		Begin = 1,
		End = 2
	}

	private Action<string> callback;

	private bool visibility;

	private AndroidJavaObject webView;

	public void Init()
	{
		Init(null);
	}

	public void Init(Action<string> cb, bool isClosedBtn = false)
	{
		callback = cb;
		webView = new AndroidJavaObject("net.gree.unitywebview.WebViewPlugin");
		webView.Call("Init", base.name, isClosedBtn);
	}

	private void OnDestroy()
	{
		if (webView != null)
		{
			webView.Call("Destroy");
		}
	}

	public void SetMargins(int left, int top, int right, int bottom)
	{
		if (webView != null)
		{
			webView.Call("SetMargins", left, top, right, bottom);
		}
	}

	public void SetVisibility(bool v)
	{
		if (webView != null)
		{
			webView.Call("SetVisibility", v);
			visibility = v;
		}
	}

	public void LoadURL(string url)
	{
		if (webView != null)
		{
			webView.Call("LoadURL", url);
		}
	}

	public void EvaluateJS(string js)
	{
		if (webView != null)
		{
			webView.Call("LoadURL", "javascript:" + js);
		}
	}

	public void CallFromJS(string message)
	{
		Debug.Log("CallJS -> " + message);
		if (callback != null)
		{
			callback(message);
		}
		else
		{
			Debug.Log("callback enough");
		}
	}

	public void Reload()
	{
		if (webView != null)
		{
			webView.Call("Reload");
		}
	}

	public eResult GetResult()
	{
		if (webView == null)
		{
			return eResult.None;
		}
		return (eResult)webView.Call<int>("GetResult", new object[0]);
	}

	private void Update()
	{
		if (visibility && Input.GetKeyUp(KeyCode.Escape))
		{
			Debug.Log("Key Escape");
			webView.Call("onBackPressed");
		}
	}
}
