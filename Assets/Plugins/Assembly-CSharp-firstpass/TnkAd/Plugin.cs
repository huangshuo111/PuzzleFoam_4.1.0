using UnityEngine;

namespace TnkAd
{
	public class Plugin
	{
		private static Plugin _instance;

		private AndroidJavaClass pluginClass;

		public static Plugin Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new Plugin();
				}
				return _instance;
			}
		}

		public Plugin()
		{
			pluginClass = new AndroidJavaClass("com.tnkfactory.ad.unity.TnkUnityPlugin");
		}

		public void initInstance()
		{
			pluginClass.CallStatic("initInstance");
		}

		public void initInstance(string appId)
		{
			pluginClass.CallStatic("initInstance", appId);
		}

		public void prepareVideoAd()
		{
			pluginClass.CallStatic("prepareVideoAd");
		}

		public void prepareVideoAd(string logicName)
		{
			pluginClass.CallStatic("prepareVideoAd", logicName);
		}

		public void prepareVideoAdOnce(string logicName, string handlerName)
		{
			pluginClass.CallStatic("prepareVideoAdOnce", logicName, handlerName);
		}

		public void prepareVideoAd(string logicName, string handlerName)
		{
			pluginClass.CallStatic("prepareVideoAd", logicName, handlerName);
		}

		public void showVideoAd()
		{
			pluginClass.CallStatic("showVideoAd");
		}

		public void showVideoCloseButton(bool show)
		{
			if (show)
			{
				pluginClass.CallStatic("showVideoCloseButton");
			}
			else
			{
				pluginClass.CallStatic("hideVideoCloseButton");
			}
		}

		public void showVideoAd(string logicName)
		{
			pluginClass.CallStatic("showVideoAd", logicName);
		}

		public bool hasVideoAd(string logicName)
		{
			return pluginClass.CallStatic<bool>("hasVideoAd", new object[1] { logicName });
		}

		public void prepareInterstitialAdForPPI()
		{
			pluginClass.CallStatic("prepareInterstitialAdForPPI");
		}

		public void prepareInterstitialAdForCPC()
		{
			pluginClass.CallStatic("prepareInterstitialAdForCPC");
		}

		public void prepareInterstitialAd(string logicName)
		{
			pluginClass.CallStatic("prepareInterstitialAd", logicName);
		}

		public void prepareInterstitialAd(string logicName, string handlerName)
		{
			pluginClass.CallStatic("prepareInterstitialAd", logicName, handlerName);
		}

		public void showInterstitialAd()
		{
			pluginClass.CallStatic("showInterstitialAd");
		}

		public void showInterstitialAd(string logicName)
		{
			pluginClass.CallStatic("showInterstitialAd", logicName);
		}

		public void onBackPressed()
		{
			pluginClass.CallStatic("onBackPressed");
		}

		public bool isAdViewVisible()
		{
			return pluginClass.CallStatic<bool>("isAdViewVisible", new object[0]);
		}

		public bool isInterstitialAdVisible(string logicName)
		{
			return pluginClass.CallStatic<bool>("isInterstitialAdVisible", new object[1] { logicName });
		}

		public bool isInterstitialAdVisible()
		{
			return pluginClass.CallStatic<bool>("isInterstitialAdVisible", new object[0]);
		}

		public void showAdList()
		{
			pluginClass.CallStatic("showAdList");
		}

		public void showAdList(string title)
		{
			pluginClass.CallStatic("showAdList", title);
		}

		public void popupAdList()
		{
			pluginClass.CallStatic("popupAdList");
		}

		public void popupAdList(string title)
		{
			pluginClass.CallStatic("popupAdList", title);
		}

		public void popupAdList(string title, string handlerName)
		{
			pluginClass.CallStatic("popupAdList", title, handlerName);
		}

		public void applicationStarted()
		{
			pluginClass.CallStatic("applicationStarted");
		}

		public void actionCompleted()
		{
			pluginClass.CallStatic("actionCompleted");
		}

		public void actionCompleted(string actionName)
		{
			pluginClass.CallStatic("actionCompleted", actionName);
		}

		public void buyCompleted(string itemName)
		{
			pluginClass.CallStatic("buyCompleted", itemName);
		}

		public void queryPoint(string handlerName)
		{
			pluginClass.CallStatic("queryPoint", handlerName);
		}

		public void withdrawPoints(string desc, string handlerName)
		{
			pluginClass.CallStatic("withdrawPoints", desc, handlerName);
		}

		public void purchaseItem(int cost, string itemName, string handlerName)
		{
			pluginClass.CallStatic("purchaseItem", cost, itemName, handlerName);
		}

		public void queryPublishState(string handlerName)
		{
			pluginClass.CallStatic("queryPublishState", handlerName);
		}

		public void setUserName(string userName)
		{
			pluginClass.CallStatic("setUserName", userName);
		}

		public void setUserAge(int age)
		{
			pluginClass.CallStatic("setUserAge", age);
		}

		public void setUserGender(int gender)
		{
			pluginClass.CallStatic("setUserGender", gender);
		}

		public void popupMoreApps()
		{
			pluginClass.CallStatic("popupMoreApps");
		}

		public void popupMoreApps(string title)
		{
			pluginClass.CallStatic("popupMoreApps", title);
		}

		public void popupMoreApps(string title, string handlerName)
		{
			pluginClass.CallStatic("popupMoreApps", title, handlerName);
		}

		public void popupMoreAppsWithButtons(string title, string closeText, string exitText, string handlerName)
		{
			pluginClass.CallStatic("popupMoreAppsWithButtons", title, closeText, exitText, handlerName);
		}
	}
}
