using System;
using System.Collections.Generic;
using System.Text;

public class Vungle
{
	private const string PLUGIN_VERSION = "3.1.21";

	private const string IOS_SDK_VERSION = "4.0.6";

	private const string WIN_SDK_VERSION = "1.3.15";

	private const string ANDROID_SDK_VERSION = "4.0.2";

	public static string VersionInfo
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder("unity-");
			return stringBuilder.Append("3.1.21").Append("/android-").Append("4.0.2")
				.ToString();
		}
	}

	public static event Action onAdStartedEvent;

	[Obsolete("Please use onAdFinishedEvent event instead this and onAdViewedEvent event.")]
	public static event Action onAdEndedEvent;

	public static event Action<bool> adPlayableEvent;

	[Obsolete("Please use adPlayableEvent event instead this and onCachedAdAvailableEvent event.")]
	public static event Action onCachedAdAvailableEvent;

	[Obsolete("Please use onAdFinishedEvent event instead this and onAdEndedEvent event.")]
	public static event Action<double, double> onAdViewedEvent;

	public static event Action<string> onLogEvent;

	public static event Action<AdFinishedEventArgs> onAdFinishedEvent;

	static Vungle()
	{
		VungleManager.OnAdStartEvent += adStarted;
		VungleManager.OnAdEndEvent += adEnded;
		VungleManager.OnCachedAdAvailableEvent += cachedAdAvailable;
		VungleManager.OnAdPlayableEvent += adPlayable;
		VungleManager.OnVideoViewEvent += videoViewed;
		VungleManager.OnSDKLogEvent += onLog;
		VungleManager.OnAdFinishedEvent += adFinished;
	}

	private static void adStarted()
	{
		if (Vungle.onAdStartedEvent != null)
		{
			Vungle.onAdStartedEvent();
		}
	}

	private static void adEnded()
	{
		if (Vungle.onAdEndedEvent != null)
		{
			Vungle.onAdEndedEvent();
		}
	}

	private static void videoViewed(double timeWatched, double totalDuration)
	{
		if (Vungle.onAdViewedEvent != null)
		{
			Vungle.onAdViewedEvent(timeWatched, totalDuration);
		}
	}

	private static void cachedAdAvailable()
	{
		if (Vungle.onCachedAdAvailableEvent != null)
		{
			Vungle.onCachedAdAvailableEvent();
		}
	}

	private static void adPlayable(bool playable)
	{
		if (Vungle.adPlayableEvent != null)
		{
			Vungle.adPlayableEvent(playable);
		}
	}

	private static void onLog(string log)
	{
		if (Vungle.onLogEvent != null)
		{
			Vungle.onLogEvent(log);
		}
	}

	private static void adFinished(AdFinishedEventArgs args)
	{
		if (Vungle.onAdFinishedEvent != null)
		{
			Vungle.onAdFinishedEvent(args);
		}
	}

	public static void init(string androidAppId, string iosAppId, string winAppId = "")
	{
		VungleAndroid.init(androidAppId, "3.1.21");
	}

	public static void setSoundEnabled(bool isEnabled)
	{
		VungleAndroid.setSoundEnabled(isEnabled);
	}

	public static bool isAdvertAvailable()
	{
		return VungleAndroid.isVideoAvailable();
	}

	[Obsolete("This method is deprecated. Please use playAdWithOptions( Dictionary<string,object> ) method instead.")]
	public static void playAd(bool incentivized = false, string user = "", int orientation = 6)
	{
		VungleAndroid.playAd(incentivized, user);
	}

	public static void playAdWithOptions(Dictionary<string, object> options)
	{
		if (options == null)
		{
			throw new ArgumentException("You can not call this method with null parameter");
		}
		VungleAndroid.playAdEx(options);
	}

	public static void clearCache()
	{
	}

	public static void clearSleep()
	{
	}

	public static void setEndPoint(string endPoint)
	{
	}

	public static void setLogEnable(bool enable)
	{
	}

	public static string getEndPoint()
	{
		return string.Empty;
	}

	public static void onResume()
	{
		VungleAndroid.onResume();
	}

	public static void onPause()
	{
		VungleAndroid.onPause();
	}
}
