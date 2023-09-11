using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Toast.Analytics
{
	public class GameAnalyticsUnityPluginController : MonoBehaviour
	{
		private const string DELIMITER = "##";

		public static CampaignListener listener;

		private static AndroidJavaClass _activityClass;

		public void Awake()
		{
			Debug.Log("GameAnalyticsUnityPluginController Awake");
			Object.DontDestroyOnLoad(base.gameObject);
			GameAnalyticsUnityPlugin.instance.controller = this;
			_activityClass = new AndroidJavaClass("com.toast.android.analytics.unity.UnityActivity");
		}

		private void OnDestroy()
		{
			if (GameAnalyticsUnityPlugin.instance.controller == this)
			{
				GameAnalyticsUnityPlugin.instance.controller = null;
			}
		}

		public static void setDebugMode(bool enable)
		{
			_activityClass.CallStatic("setDebugMode", enable);
		}

		public static int setUserId(string userId, bool useCampaignOrPromotion)
		{
			return _activityClass.CallStatic<int>("setUserId", new object[2] { userId, useCampaignOrPromotion });
		}

		public static string getDeviceInfo(string key)
		{
			return _activityClass.CallStatic<string>("getDeviceInfo", new object[1] { key });
		}

		public static void setCampaignListener(CampaignListener campaignListener)
		{
			listener = campaignListener;
			_activityClass.CallStatic("setCampaignListener", GameAnalyticsUnityPlugin.instance.controller.name);
		}

		public static int setGcmSenderId(string gcmSenderId)
		{
			return _activityClass.CallStatic<int>("setGcmSenderId", new object[1] { gcmSenderId });
		}

		public static int initializeSdk(string appId, string companyId, string appVersion, bool useLoggingUserId)
		{
			return _activityClass.CallStatic<int>("initializeSdk", new object[4] { appId, companyId, appVersion, useLoggingUserId });
		}

		public static int traceActivation()
		{
			return _activityClass.CallStatic<int>("traceActivation", new object[0]);
		}

		public static int traceDeactivation()
		{
			return _activityClass.CallStatic<int>("traceDeactivation", new object[0]);
		}

		public static int traceFriendCount(int friendCount)
		{
			return _activityClass.CallStatic<int>("traceFriendCount", new object[1] { friendCount });
		}

		public static int tracePurchase(string itemCode, float payment, float unitCost, string currency, int level)
		{
			return _activityClass.CallStatic<int>("tracePurchase", new object[5] { itemCode, payment, unitCost, currency, level });
		}

		public static int traceMoneyAcquisition(string usageCode, string type, double acquistionAmount, int level)
		{
			return _activityClass.CallStatic<int>("traceMoneyAcquisition", new object[4] { usageCode, type, acquistionAmount, level });
		}

		public static int traceMoneyConsumption(string usageCode, string type, double consumptionAmount, int level)
		{
			return _activityClass.CallStatic<int>("traceMoneyConsumption", new object[4] { usageCode, type, consumptionAmount, level });
		}

		public static int traceLevelUp(int level)
		{
			return _activityClass.CallStatic<int>("traceLevelUp", new object[1] { level });
		}

		public static int traceEvent(string eventType, string eventCode, string param1, string param2, double value, int level)
		{
			return _activityClass.CallStatic<int>("traceEvent", new object[6] { eventType, eventCode, param1, param2, value, level });
		}

		public static int traceStartSpeed(string intervalName)
		{
			return _activityClass.CallStatic<int>("traceStartSpeed", new object[1] { intervalName });
		}

		public static int traceEndSpeed(string intervalName)
		{
			return _activityClass.CallStatic<int>("traceEndSpeed", new object[1] { intervalName });
		}

		public static int showCampaign(string adspaceName)
		{
			return _activityClass.CallStatic<int>("showCampaign", new object[1] { adspaceName });
		}

		public static int showCampaign(string adspaceName, int animation, int lifeTime)
		{
			return _activityClass.CallStatic<int>("showCampaign", new object[3] { adspaceName, animation, lifeTime });
		}

		public static int hideCampaign(string adspaceName)
		{
			return _activityClass.CallStatic<int>("hideCampaign", new object[1] { adspaceName });
		}

		public static int hideCampaign(string adspaceName, int animation)
		{
			return _activityClass.CallStatic<int>("hideCampaign", new object[2] { adspaceName, animation });
		}

		public static bool isPromotionAvailable()
		{
			return _activityClass.CallStatic<bool>("isPromotionAvailable", new object[0]);
		}

		public static string getPromotionButtonImagePath()
		{
			return _activityClass.CallStatic<string>("getPromotionButtonImagePath", new object[0]);
		}

		public static int launchPromotionPage()
		{
			return _activityClass.CallStatic<int>("launchPromotionPage", new object[0]);
		}

		public void OnCampaignListener_OnCampaignVisibilityChanged(string param)
		{
			string[] array = Regex.Split(param, "##");
			if (array.Length == 2)
			{
				string adspaceName = array[0];
				bool show = ((array[1] == "true") ? true : false);
				if (listener != null)
				{
					listener.OnCampaignVisibilityChanged(adspaceName, show);
				}
			}
		}

		public void OnCampaignListener_OnCampaignLoadSuccess(string param)
		{
			if (listener != null)
			{
				listener.OnCampaignLoadSuccess(param);
			}
		}

		public void OnCampaignListener_OnCampaignLoadFail(string param)
		{
			string[] array = Regex.Split(param, "##");
			if (array.Length == 3)
			{
				string adspaceName = array[0];
				int errorCode = int.Parse(array[1]);
				string errorMessage = array[2];
				if (listener != null)
				{
					listener.OnCampaignLoadFail(adspaceName, errorCode, errorMessage);
				}
			}
		}

		public void OnCampaignListener_OnMissionCompleted(string param)
		{
			string[] array = Regex.Split(param, "##");
			if (array.Length > 0)
			{
				List<string> missionList = new List<string>(array);
				if (listener != null)
				{
					listener.OnMissionComplete(missionList);
				}
			}
		}

		public void OnCampaignListener_OnPromotionVisibilityChanged(string param)
		{
			if (param == "true")
			{
				listener.OnPromotionVisibilityChanged(true);
			}
			else if (param == "false")
			{
				listener.OnPromotionVisibilityChanged(false);
			}
		}

		public void OnCampaignListener_OnCampaignClick(string param)
		{
			if (listener != null)
			{
				listener.OnCampaignClick(param);
			}
		}
	}
}
