using System;
using UnityEngine;

public class GoogleAnalyticsAndroidV4 : IDisposable
{
	private string trackingCode;

	private string appVersion;

	private string appName;

	private string bundleIdentifier;

	private int dispatchPeriod;

	private int sampleFrequency;

	private bool anonymizeIP;

	private bool adIdCollection;

	private bool dryRun;

	private int sessionTimeout;

	private AndroidJavaObject tracker;

	private AndroidJavaObject logger;

	private AndroidJavaObject currentActivityObject;

	private AndroidJavaObject googleAnalyticsSingleton;

	internal void InitializeTracker()
	{
		Debug.Log("Initializing Google Analytics Android Tracker.");
		using (AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.google.android.gms.analytics.GoogleAnalytics"))
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				currentActivityObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				googleAnalyticsSingleton = androidJavaObject.CallStatic<AndroidJavaObject>("getInstance", new object[1] { currentActivityObject });
				tracker = googleAnalyticsSingleton.Call<AndroidJavaObject>("newTracker", new object[1] { trackingCode });
				googleAnalyticsSingleton.Call("setLocalDispatchPeriod", dispatchPeriod);
				googleAnalyticsSingleton.Call("setDryRun", dryRun);
				tracker.Call("setSampleRate", (double)sampleFrequency);
				tracker.Call("setAppName", appName);
				tracker.Call("setAppId", bundleIdentifier);
				tracker.Call("setAppVersion", appVersion);
				tracker.Call("setAnonymizeIp", anonymizeIP);
				tracker.Call("enableAdvertisingIdCollection", adIdCollection);
			}
		}
	}

	internal void SetTrackerVal(Field fieldName, object value)
	{
		object[] args = new object[2]
		{
			fieldName.ToString(),
			value
		};
		tracker.Call("set", args);
	}

	private void SetSessionOnBuilder(AndroidJavaObject hitBuilder)
	{
	}

	internal void StartSession()
	{
	}

	internal void StopSession()
	{
	}

	public void SetOptOut(bool optOut)
	{
		googleAnalyticsSingleton.Call("setAppOptOut", optOut);
	}

	internal void LogScreen(AppViewHitBuilder builder)
	{
		tracker.Call("setScreenName", builder.GetScreenName());
		AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.google.android.gms.analytics.HitBuilders$ScreenViewBuilder");
		object[] args = new object[1] { androidJavaObject.Call<AndroidJavaObject>("build", new object[0]) };
		tracker.Call("send", args);
	}

	internal void LogEvent(EventHitBuilder builder)
	{
		AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.google.android.gms.analytics.HitBuilders$EventBuilder");
		androidJavaObject.Call<AndroidJavaObject>("setCategory", new object[1] { builder.GetEventCategory() });
		androidJavaObject.Call<AndroidJavaObject>("setAction", new object[1] { builder.GetEventAction() });
		androidJavaObject.Call<AndroidJavaObject>("setLabel", new object[1] { builder.GetEventLabel() });
		androidJavaObject.Call<AndroidJavaObject>("setValue", new object[1] { builder.GetEventValue() });
		object[] args = new object[1] { androidJavaObject.Call<AndroidJavaObject>("build", new object[0]) };
		tracker.Call("send", args);
	}

	internal void LogTransaction(TransactionHitBuilder builder)
	{
	}

	internal void LogItem(ItemHitBuilder builder)
	{
	}

	public void LogException(ExceptionHitBuilder builder)
	{
	}

	public void LogSocial(SocialHitBuilder builder)
	{
	}

	public void LogTiming(TimingHitBuilder builder)
	{
	}

	public void DispatchHits()
	{
	}

	public void SetSampleFrequency(int sampleFrequency)
	{
		this.sampleFrequency = sampleFrequency;
	}

	public void ClearUserIDOverride()
	{
		SetTrackerVal(Fields.USER_ID, null);
	}

	public void SetTrackingCode(string trackingCode)
	{
		this.trackingCode = trackingCode;
	}

	public void SetAppName(string appName)
	{
		this.appName = appName;
	}

	public void SetBundleIdentifier(string bundleIdentifier)
	{
		this.bundleIdentifier = bundleIdentifier;
	}

	public void SetAppVersion(string appVersion)
	{
		this.appVersion = appVersion;
	}

	public void SetDispatchPeriod(int dispatchPeriod)
	{
		this.dispatchPeriod = dispatchPeriod;
	}

	public void SetLogLevelValue(GoogleAnalyticsV4.DebugMode logLevel)
	{
	}

	public void SetAnonymizeIP(bool anonymizeIP)
	{
		this.anonymizeIP = anonymizeIP;
	}

	public void SetAdIdCollection(bool adIdCollection)
	{
		this.adIdCollection = adIdCollection;
	}

	public void SetDryRun(bool dryRun)
	{
		this.dryRun = dryRun;
	}

	public void Dispose()
	{
		googleAnalyticsSingleton.Dispose();
		tracker.Dispose();
	}
}
