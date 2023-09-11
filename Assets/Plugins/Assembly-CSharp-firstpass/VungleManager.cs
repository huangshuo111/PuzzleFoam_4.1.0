using System;
using System.Collections.Generic;
using MiniJSONV;
using UnityEngine;

public class VungleManager : MonoBehaviour
{
	private AdFinishedEventArgs adFinishedEventArgs;

	private static AdFinishedEventArgs adWinFinishedEventArgs;

	public static event Action OnAdStartEvent;

	[Obsolete("Please use OnAdPlayable instead.")]
	public static event Action OnCachedAdAvailableEvent;

	public static event Action<bool> OnAdPlayableEvent;

	[Obsolete("Please use OnAdFinishedEvent instead.")]
	public static event Action OnAdEndEvent;

	[Obsolete("Please use OnAdFinishedEvent instead.")]
	public static event Action<double, double> OnVideoViewEvent;

	public static event Action<string> OnSDKLogEvent;

	public static event Action<AdFinishedEventArgs> OnAdFinishedEvent;

	static VungleManager()
	{
		try
		{
			GameObject gameObject = new GameObject("VungleManager");
			gameObject.AddComponent<VungleManager>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		catch (UnityException)
		{
			Debug.LogWarning("It looks like you have the VungleManager on a GameObject in your scene. Please remove the script from your scene.");
		}
	}

	public static void noop()
	{
	}

	public static void onEvent(string e, string arg)
	{
		if (e == "OnAdStart")
		{
			VungleManager.OnAdStartEvent();
		}
		if (e == "OnAdEnd")
		{
			bool flag = adWinFinishedEventArgs != null;
			if (!flag)
			{
				adWinFinishedEventArgs = new AdFinishedEventArgs();
			}
			adWinFinishedEventArgs.WasCallToActionClicked = "1".Equals(arg);
			if (flag)
			{
				VungleManager.OnAdFinishedEvent(adWinFinishedEventArgs);
				adWinFinishedEventArgs = null;
			}
			VungleManager.OnAdEndEvent();
		}
		if (e == "OnAdPlayableChanged")
		{
			if ("1".Equals(arg))
			{
				VungleManager.OnCachedAdAvailableEvent();
			}
			VungleManager.OnAdPlayableEvent("1".Equals(arg));
		}
		if (e == "OnVideoView")
		{
			string[] array = arg.Split(':');
			if (array.Length == 3)
			{
				double num = double.Parse(array[1]);
				double num2 = double.Parse(array[2]);
				bool flag2 = adWinFinishedEventArgs != null;
				if (!flag2)
				{
					adWinFinishedEventArgs = new AdFinishedEventArgs();
				}
				adWinFinishedEventArgs.IsCompletedView = bool.Parse(array[0]);
				adWinFinishedEventArgs.TimeWatched = num;
				adWinFinishedEventArgs.TotalDuration = num2;
				if (flag2)
				{
					VungleManager.OnAdFinishedEvent(adWinFinishedEventArgs);
					adWinFinishedEventArgs = null;
				}
				VungleManager.OnVideoViewEvent(num, num2);
			}
		}
		if (e == "Diagnostic")
		{
			VungleManager.OnSDKLogEvent(arg);
		}
	}

	private void OnAdStart(string empty)
	{
		VungleManager.OnAdStartEvent();
	}

	private void OnCachedAdAvailable(string empty)
	{
		VungleManager.OnCachedAdAvailableEvent();
	}

	private void OnAdPlayable(string playable)
	{
		VungleManager.OnAdPlayableEvent("1".Equals(playable));
	}

	private void OnVideoView(string param)
	{
	}

	private void OnCloseProductSheet(string empty)
	{
	}

	private void OnSDKLog(string log)
	{
		VungleManager.OnSDKLogEvent(log);
	}

	private void OnAdEnd(string param)
	{
		if (adFinishedEventArgs == null)
		{
			adFinishedEventArgs = new AdFinishedEventArgs();
		}
		string[] array = param.Split('-');
		if (array.Length == 2)
		{
			adFinishedEventArgs.IsCompletedView = array[0].Equals("1");
			adFinishedEventArgs.WasCallToActionClicked = array[1].Equals("1");
		}
		VungleManager.OnAdFinishedEvent(adFinishedEventArgs);
		VungleManager.OnVideoViewEvent(adFinishedEventArgs.IsCompletedView ? 15 : 0, 15.0);
		adFinishedEventArgs = null;
		VungleManager.OnAdEndEvent();
	}

	private bool extractBoolValue(string json, string key)
	{
		Dictionary<string, object> attrs = (Dictionary<string, object>)Json.Deserialize(json);
		return extractBoolValue(attrs, key);
	}

	private bool extractBoolValue(Dictionary<string, object> attrs, string key)
	{
		return bool.Parse(attrs[key].ToString());
	}
}
