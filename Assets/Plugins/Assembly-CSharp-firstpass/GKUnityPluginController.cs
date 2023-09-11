using UnityEngine;

public class GKUnityPluginController : MonoBehaviour
{
	public delegate void purchasecallback(string strresult);

	public delegate void checkdevicecallback(string strresult);

	private static GKUnityPluginController m_Instance;

	public static string m_AdvertisingId = string.Empty;

	public static string deviceToken_ = string.Empty;

	private static purchasecallback purchaseCB_;

	private static checkdevicecallback checkdeviceCB_;

	private int m_ErrorCode;

	private static AndroidJavaObject m_Activity;

	public static GKUnityPluginController Instance
	{
		get
		{
			if (!m_Instance)
			{
				m_Instance = Object.FindObjectOfType(typeof(GKUnityPluginController)) as GKUnityPluginController;
				if (!m_Instance)
				{
					GameObject gameObject = new GameObject();
					gameObject.name = "GKUnityPluginController";
					m_Instance = gameObject.AddComponent(typeof(GKUnityPluginController)) as GKUnityPluginController;
					Object.DontDestroyOnLoad(m_Instance);
				}
			}
			return m_Instance;
		}
	}

	public int ErrorCode
	{
		get
		{
			return m_ErrorCode;
		}
	}

	public static AndroidJavaObject Activity
	{
		get
		{
			if (m_Activity == null)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				m_Activity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
			return m_Activity;
		}
	}

	public void Init()
	{
	}

	public void GK_RegistGCM()
	{
		Activity.Call("RegistGCM");
	}

	public static void GK_Payment_Initialize(string publicKey)
	{
		Activity.CallStatic("GK_Payment_Initialize", publicKey);
	}

	public static void GK_Payment_Purchase(string productId, purchasecallback cb)
	{
		purchaseCB_ = cb;
		Activity.CallStatic("GK_Payment_Purchase", productId, Instance.gameObject.name);
	}

	public static void GK_CheckDevice(checkdevicecallback cb)
	{
		checkdeviceCB_ = cb;
		Activity.CallStatic("GK_CheckDevice");
	}

	public void SetLocalNotification(int delay, string title, string msg, int code, int rate, int diff)
	{
		Activity.Call("SetLocalNotification", delay, title, msg, code, rate, diff);
	}

	public static void CallAndroidFunc(string funcnName, int code)
	{
		Activity.Call(funcnName, code);
	}

	public void AdvertisingId(string id)
	{
		m_AdvertisingId = id;
	}

	public void getAdvertisingId()
	{
		m_AdvertisingId = string.Empty;
		Activity.Call("getAdvertisingId");
	}

	public static void CallAndroidFunc(string className, string funcnName)
	{
		using (AndroidJavaObject androidJavaObject = new AndroidJavaObject(className))
		{
			androidJavaObject.Call(funcnName);
		}
	}

	public bool InstalledPackage(string _packageName)
	{
		return Activity.Call<bool>("InstalledPackage", new object[1] { _packageName });
	}

	public void LaunchPackage(string _packageName)
	{
		Activity.Call("LaunchPackage", _packageName);
	}

	public void ToastMessage(string _szMessage)
	{
		Activity.Call("ToastMessageMakeText", _szMessage);
	}

	public void BuyKakaoShopItem(string _accessToken, string _itemCode, string _developerPayload, string _userId, string _phase, purchasecallback cb)
	{
		purchaseCB_ = cb;
		Activity.Call("BuyKakaoShopItem", _accessToken, _itemCode, _developerPayload, _userId, _phase, Instance.gameObject.name);
	}

	public void BuyCulturelandItem(string _TestMode, string _userId, string _itemCode, purchasecallback cb)
	{
		purchaseCB_ = cb;
		Activity.Call("BuyCulturelandItem", _TestMode, _userId, _itemCode);
	}

	public void PurchaseFinished(string strResult)
	{
		if (purchaseCB_ != null)
		{
			purchaseCB_(strResult);
		}
	}

	public void CheckDeviceResult(string strResult)
	{
		if (checkdeviceCB_ != null)
		{
			checkdeviceCB_(strResult);
		}
	}

	public void RegisterDeviceToken(string deviceToken)
	{
		deviceToken_ = deviceToken;
	}
}
