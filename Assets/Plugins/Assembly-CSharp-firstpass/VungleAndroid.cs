using System.Collections.Generic;
using MiniJSONV;
using UnityEngine;

public class VungleAndroid
{
	private static AndroidJavaObject _plugin;

	static VungleAndroid()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		VungleManager.noop();
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.vungle.VunglePlugin"))
		{
			_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
		}
	}

	public static void init(string appId, string pluginVersion)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("init", appId, pluginVersion);
		}
	}

	public static void onPause()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("onPause");
		}
	}

	public static void onResume()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("onResume");
		}
	}

	public static bool isVideoAvailable()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		return _plugin.Call<bool>("isVideoAvailable", new object[0]);
	}

	public static void setSoundEnabled(bool isEnabled)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("setSoundEnabled", isEnabled);
		}
	}

	public static void setAdOrientation(VungleAdOrientation orientation)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("setAdOrientation", (int)orientation);
		}
	}

	public static bool isSoundEnabled()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return true;
		}
		return _plugin.Call<bool>("isSoundEnabled", new object[0]);
	}

	public static void playAd(bool incentivized = false, string user = "")
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			if (user == null)
			{
				user = string.Empty;
			}
			_plugin.Call("playAd", incentivized, user);
		}
	}

	public static void playAdEx(bool incentivized = false, int orientation = 5, bool large = false, string user = "", string alerTitle = "", string alertText = "", string alertClose = "", string alertContinue = "")
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			if (user == null)
			{
				user = string.Empty;
			}
			if (alerTitle == null)
			{
				alerTitle = string.Empty;
			}
			if (alertText == null)
			{
				alertText = string.Empty;
			}
			if (alertClose == null)
			{
				alertClose = string.Empty;
			}
			if (alertContinue == null)
			{
				alertContinue = string.Empty;
			}
			_plugin.Call("playAdEx", incentivized, orientation, large, user, alerTitle, alertText, alertClose, alertContinue);
		}
	}

	public static void playAdEx(Dictionary<string, object> options)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("playAdEx", Json.Serialize(options));
		}
	}
}
