using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class ProjectSettings : ScriptableObject
{
	public enum eSNSType
	{
		KakaoTalk = 0,
		FaceBook = 1
	}

	public enum ePurchaseType
	{
		GooglePlayStore = 0,
		KakaoShop = 1,
		CultureLand = 2,
		OneStore = 3
	}

	public enum ServerType_KR
	{
		Local = 0,
		Dev = 1,
		Staging = 2,
		Live = 3,
		Temp = 4
	}

	public enum ServerType_NA
	{
		STG = 0,
		LIVE = 1
	}

	public enum eSplashType
	{
		iPhoneSplashScreen = 0,
		iPhoneHighResSplashScreen = 1,
		iPhoneTallHighResSplashScreen = 2,
		iPhone47inSplashScreen = 3,
		iPhone55inPortraitSplashScreen = 4,
		iPhone55inLandscapeSplashScreen = 5,
		iPadPortraitSplashScreen = 6,
		iPadHighResPortraitSplashScreen = 7,
		iPadLandscapeSplashScreen = 8,
		iPadHighResLandscapeSplashScreen = 9,
		Max = 10
	}

	[Serializable]
	public class GlobalDefine : ISerializable
	{
		public string define;

		public bool enabled;

		public GlobalDefine()
		{
		}

		protected GlobalDefine(SerializationInfo info, StreamingContext context)
		{
			define = info.GetString("define");
			enabled = info.GetBoolean("enabled");
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("define", define);
			info.AddValue("enabled", enabled);
		}
	}

	private const string m_BuildSettingsAssetName = "ProjectSettings";

	private const string m_BuildSettingsPath = "SNS/Resources";

	private const string m_BuildSettingsAssetExtension = ".asset";

	private static ProjectSettings instance;

	[SerializeField]
	private eSNSType m_eSNSType;

	[SerializeField]
	private ePurchaseType m_ePurchaseType;

	[SerializeField]
	private ServerType_NA m_eServerType_NA;

	[SerializeField]
	private ServerType_KR m_eServerType_KR;

	[SerializeField]
	private bool m_IPhoneSimulator;

	[SerializeField]
	private string m_ProductName_NA;

	[SerializeField]
	private string m_ProductName_KR;

	[SerializeField]
	private string m_Android_NA_FB_AppID;

	[SerializeField]
	private string m_Android_KR_FB_AppID;

	[SerializeField]
	private string m_IPhone_NA_FB_AppID;

	[SerializeField]
	private string m_IPhone_KR_FB_AppID;

	[SerializeField]
	private string m_IPhone_NA_PartyTrack_AppID;

	[SerializeField]
	private string m_IPhone_NA_PartyTrack_AppSignature;

	[SerializeField]
	private string m_Android_NA_PartyTrack_AppID;

	[SerializeField]
	private string m_Android_NA_PartyTrack_AppSignature;

	[SerializeField]
	private string m_IPhone_KR_PartyTrack_AppID;

	[SerializeField]
	private string m_IPhone_KR_PartyTrack_AppSignature;

	[SerializeField]
	private string m_Android_KR_PartyTrack_AppID;

	[SerializeField]
	private string m_Android_KR_PartyTrack_AppSignature;

	[SerializeField]
	private string m_IPhone_NA_5Rocks_AppID;

	[SerializeField]
	private string m_IPhone_NA_5Rocks_AppKey;

	[SerializeField]
	private string m_Android_NA_5Rocks_AppID;

	[SerializeField]
	private string m_Android_NA_5Rocks_AppKey;

	[SerializeField]
	private string m_IPhone_KR_5Rocks_AppID;

	[SerializeField]
	private string m_IPhone_KR_5Rocks_AppKey;

	[SerializeField]
	private string m_Android_KR_5Rocks_AppID;

	[SerializeField]
	private string m_Android_KR_5Rocks_AppKey;

	[SerializeField]
	private string m_Toast_AppKey;

	[SerializeField]
	private string m_Toast_CompanyID;

	[SerializeField]
	private Texture2D[] m_AndroidFacebookIcon = new Texture2D[6];

	[SerializeField]
	private Texture2D[] m_AndroidKakaoIcon = new Texture2D[6];

	[SerializeField]
	private Texture2D[] m_AndroidKakaoShopIcon = new Texture2D[6];

	[SerializeField]
	private Texture2D[] m_IOSFacebookIcon = new Texture2D[7];

	[SerializeField]
	private Texture2D[] m_IOSKakaoIcon = new Texture2D[7];

	public Texture2D[] Android_KakaoSplash = new Texture2D[10];

	public Texture2D[] Android_FBSplash = new Texture2D[10];

	public Texture2D[] IOS_KakaoSplash = new Texture2D[10];

	public Texture2D[] IOS_FBSplash = new Texture2D[10];

	[SerializeField]
	private List<GlobalDefine> m_ListGlobalDefine = new List<GlobalDefine>();

	[SerializeField]
	private string m_Android_NA_Version;

	[SerializeField]
	private string m_Android_KR_Version;

	[SerializeField]
	private string m_IPhone_NA_Version;

	[SerializeField]
	private string m_IPhone_KR_Version;

	[SerializeField]
	private string m_Android_NA_VersionCode;

	[SerializeField]
	private string m_Android_KR_VersionCode;

	[SerializeField]
	private string m_IPhone_NA_VersionCode;

	[SerializeField]
	private string m_IPhone_KR_VersionCode;

	public static ProjectSettings Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Resources.Load("ProjectSettings") as ProjectSettings;
				if (instance == null)
				{
					instance = ScriptableObject.CreateInstance<ProjectSettings>();
				}
			}
			return instance;
		}
	}

	public eSNSType SNSType
	{
		get
		{
			return m_eSNSType;
		}
		set
		{
			m_eSNSType = value;
		}
	}

	public ePurchaseType PurchaseType
	{
		get
		{
			return m_ePurchaseType;
		}
		set
		{
			m_ePurchaseType = value;
		}
	}

	public bool IPhoneSimulator
	{
		get
		{
			return m_IPhoneSimulator;
		}
		set
		{
			m_IPhoneSimulator = value;
		}
	}

	public ServerType_NA ConnectServer_NA
	{
		get
		{
			return m_eServerType_NA;
		}
		set
		{
			m_eServerType_NA = value;
		}
	}

	public ServerType_KR ConnectServer_KR
	{
		get
		{
			return m_eServerType_KR;
		}
		set
		{
			m_eServerType_KR = value;
		}
	}

	public string ConnectServerIP
	{
		get
		{
			switch (m_eServerType_KR)
			{
			case ServerType_KR.Local:
				return "http://192.168.1.211:10080/";
			case ServerType_KR.Dev:
				return "http://14.63.164.247:30001/";
			case ServerType_KR.Live:
				return "http://14.49.41.124/";
			case ServerType_KR.Staging:
				return "http://14.63.164.247:20001/";
			case ServerType_KR.Temp:
				return "http://14.63.164.247:30002/";
			default:
				Debug.LogError("Server Not Found");
				return string.Empty;
			}
		}
	}

	public string AssetBundleServerIP
	{
		get
		{
			switch (m_eServerType_KR)
			{
			case ServerType_KR.Local:
			case ServerType_KR.Dev:
			case ServerType_KR.Staging:
			case ServerType_KR.Temp:
				return "http://14.63.164.247:30003/static/download/resource/";
			case ServerType_KR.Live:
				return "http://rw01-ie3097.ktics.co.kr/pb/download/resource/";
			default:
				Debug.LogError("Server Not Found");
				return string.Empty;
			}
		}
	}

	public bool IsEncrypt
	{
		get
		{
			switch (m_eServerType_KR)
			{
			case ServerType_KR.Local:
				return false;
			case ServerType_KR.Dev:
				return false;
			case ServerType_KR.Live:
				return true;
			case ServerType_KR.Staging:
				return true;
			case ServerType_KR.Temp:
				return false;
			default:
				Debug.LogError("Server Not Found");
				return false;
			}
		}
	}

	public string ResponceHeaderKeysRegion
	{
		get
		{
			Debug.LogError("Server Not Found");
			return string.Empty;
		}
	}

	public bool IsLive
	{
		get
		{
			switch (m_eServerType_KR)
			{
			case ServerType_KR.Local:
				return false;
			case ServerType_KR.Dev:
				return false;
			case ServerType_KR.Live:
				return true;
			case ServerType_KR.Staging:
				return false;
			case ServerType_KR.Temp:
				return false;
			default:
				return false;
			}
		}
	}

	public string ProductName_NA
	{
		get
		{
			return m_ProductName_NA;
		}
		set
		{
			m_ProductName_NA = value;
			DirtyEditor();
		}
	}

	public string ProductName_KR
	{
		get
		{
			return m_ProductName_KR;
		}
		set
		{
			m_ProductName_KR = value;
			DirtyEditor();
		}
	}

	public string IPhone_NA_FB_AppID
	{
		get
		{
			return m_IPhone_NA_FB_AppID;
		}
		set
		{
			m_IPhone_NA_FB_AppID = value;
			DirtyEditor();
		}
	}

	public string Android_NA_FB_AppID
	{
		get
		{
			return m_Android_NA_FB_AppID;
		}
		set
		{
			m_Android_NA_FB_AppID = value;
			DirtyEditor();
		}
	}

	public string IPhone_KR_FB_AppID
	{
		get
		{
			return m_IPhone_KR_FB_AppID;
		}
		set
		{
			m_IPhone_KR_FB_AppID = value;
			DirtyEditor();
		}
	}

	public string Android_KR_FB_AppID
	{
		get
		{
			return m_Android_KR_FB_AppID;
		}
		set
		{
			m_Android_KR_FB_AppID = value;
			DirtyEditor();
		}
	}

	public string IPhone_NA_PartyTrack_AppID
	{
		get
		{
			return m_IPhone_NA_PartyTrack_AppID;
		}
		set
		{
			m_IPhone_NA_PartyTrack_AppID = value;
			DirtyEditor();
		}
	}

	public string Android_NA_PartyTrack_AppID
	{
		get
		{
			return m_Android_NA_PartyTrack_AppID;
		}
		set
		{
			m_Android_NA_PartyTrack_AppID = value;
			DirtyEditor();
		}
	}

	public string IPhone_KR_PartyTrack_AppID
	{
		get
		{
			return m_IPhone_KR_PartyTrack_AppID;
		}
		set
		{
			m_IPhone_KR_PartyTrack_AppID = value;
			DirtyEditor();
		}
	}

	public string Android_KR_PartyTrack_AppID
	{
		get
		{
			return m_Android_KR_PartyTrack_AppID;
		}
		set
		{
			m_Android_KR_PartyTrack_AppID = value;
			DirtyEditor();
		}
	}

	public string IPhone_NA_PartyTrack_AppSignature
	{
		get
		{
			return m_IPhone_NA_PartyTrack_AppSignature;
		}
		set
		{
			m_IPhone_NA_PartyTrack_AppSignature = value;
			DirtyEditor();
		}
	}

	public string Android_NA_PartyTrack_AppSignature
	{
		get
		{
			return m_Android_NA_PartyTrack_AppSignature;
		}
		set
		{
			m_Android_NA_PartyTrack_AppSignature = value;
			DirtyEditor();
		}
	}

	public string IPhone_KR_PartyTrack_AppSignature
	{
		get
		{
			return m_IPhone_KR_PartyTrack_AppSignature;
		}
		set
		{
			m_IPhone_KR_PartyTrack_AppSignature = value;
			DirtyEditor();
		}
	}

	public string Android_KR_PartyTrack_AppSignature
	{
		get
		{
			return m_Android_KR_PartyTrack_AppSignature;
		}
		set
		{
			m_Android_KR_PartyTrack_AppSignature = value;
			DirtyEditor();
		}
	}

	public string IPhone_NA_5Rocks_AppID
	{
		get
		{
			return m_IPhone_NA_5Rocks_AppID;
		}
		set
		{
			m_IPhone_NA_5Rocks_AppID = value;
			DirtyEditor();
		}
	}

	public string Android_NA_5Rocks_AppID
	{
		get
		{
			return m_Android_NA_5Rocks_AppID;
		}
		set
		{
			m_Android_NA_5Rocks_AppID = value;
			DirtyEditor();
		}
	}

	public string IPhone_KR_5Rocks_AppID
	{
		get
		{
			return m_IPhone_KR_5Rocks_AppID;
		}
		set
		{
			m_IPhone_KR_5Rocks_AppID = value;
			DirtyEditor();
		}
	}

	public string Android_KR_5Rocks_AppID
	{
		get
		{
			return m_Android_KR_5Rocks_AppID;
		}
		set
		{
			m_Android_KR_5Rocks_AppID = value;
			DirtyEditor();
		}
	}

	public string IPhone_NA_5Rocks_AppKey
	{
		get
		{
			return m_IPhone_NA_5Rocks_AppKey;
		}
		set
		{
			m_IPhone_NA_5Rocks_AppKey = value;
			DirtyEditor();
		}
	}

	public string Android_NA_5Rocks_AppKey
	{
		get
		{
			return m_Android_NA_5Rocks_AppKey;
		}
		set
		{
			m_Android_NA_5Rocks_AppKey = value;
			DirtyEditor();
		}
	}

	public string IPhone_KR_5Rocks_AppKey
	{
		get
		{
			return m_IPhone_KR_5Rocks_AppKey;
		}
		set
		{
			m_IPhone_KR_5Rocks_AppKey = value;
			DirtyEditor();
		}
	}

	public string Android_KR_5Rocks_AppKey
	{
		get
		{
			return m_Android_KR_5Rocks_AppKey;
		}
		set
		{
			m_Android_KR_5Rocks_AppKey = value;
			DirtyEditor();
		}
	}

	public string ToastAppKey
	{
		get
		{
			return m_Toast_AppKey;
		}
		set
		{
			m_Toast_AppKey = value;
			DirtyEditor();
		}
	}

	public string ToastCompanyID
	{
		get
		{
			return m_Toast_CompanyID;
		}
		set
		{
			m_Toast_CompanyID = value;
			DirtyEditor();
		}
	}

	public Texture2D[] AndroidFacebookIconGroup
	{
		get
		{
			return m_AndroidFacebookIcon;
		}
	}

	public Texture2D[] AndroidKakaoIconGroup
	{
		get
		{
			return m_AndroidKakaoIcon;
		}
	}

	public Texture2D[] AndroidKakaoShopIconGroup
	{
		get
		{
			return m_AndroidKakaoShopIcon;
		}
	}

	public Texture2D[] IOSFacebookIconGroup
	{
		get
		{
			return m_IOSFacebookIcon;
		}
	}

	public Texture2D[] IOSKakaoIconGroup
	{
		get
		{
			return m_IOSKakaoIcon;
		}
	}

	public Texture2D AndroidIconFB_192x192
	{
		get
		{
			return m_AndroidFacebookIcon[0];
		}
		set
		{
			if (m_AndroidFacebookIcon[0] != value)
			{
				m_AndroidFacebookIcon[0] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconFB_144x144
	{
		get
		{
			return m_AndroidFacebookIcon[1];
		}
		set
		{
			if (m_AndroidFacebookIcon[1] != value)
			{
				m_AndroidFacebookIcon[1] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconFB_96x96
	{
		get
		{
			return m_AndroidFacebookIcon[2];
		}
		set
		{
			if (m_AndroidFacebookIcon[2] != value)
			{
				m_AndroidFacebookIcon[2] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconFB_72x72
	{
		get
		{
			return m_AndroidFacebookIcon[3];
		}
		set
		{
			if (m_AndroidFacebookIcon[3] != value)
			{
				m_AndroidFacebookIcon[3] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconFB_48x48
	{
		get
		{
			return m_AndroidFacebookIcon[4];
		}
		set
		{
			if (m_AndroidFacebookIcon[4] != value)
			{
				m_AndroidFacebookIcon[4] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconFB_36x36
	{
		get
		{
			return m_AndroidFacebookIcon[5];
		}
		set
		{
			if (m_AndroidFacebookIcon[5] != value)
			{
				m_AndroidFacebookIcon[5] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconKakao_192x192
	{
		get
		{
			return m_AndroidKakaoIcon[0];
		}
		set
		{
			if (m_AndroidKakaoIcon[0] != value)
			{
				m_AndroidKakaoIcon[0] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconKakao_144x144
	{
		get
		{
			return m_AndroidKakaoIcon[1];
		}
		set
		{
			if (m_AndroidKakaoIcon[1] != value)
			{
				m_AndroidKakaoIcon[1] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconKakao_96x96
	{
		get
		{
			return m_AndroidKakaoIcon[2];
		}
		set
		{
			if (m_AndroidKakaoIcon[2] != value)
			{
				m_AndroidKakaoIcon[2] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconKakao_72x72
	{
		get
		{
			return m_AndroidKakaoIcon[3];
		}
		set
		{
			if (m_AndroidKakaoIcon[3] != value)
			{
				m_AndroidKakaoIcon[3] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconKakao_48x48
	{
		get
		{
			return m_AndroidKakaoIcon[4];
		}
		set
		{
			if (m_AndroidKakaoIcon[4] != value)
			{
				m_AndroidKakaoIcon[4] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconKakao_36x36
	{
		get
		{
			return m_AndroidKakaoIcon[5];
		}
		set
		{
			if (m_AndroidKakaoIcon[5] != value)
			{
				m_AndroidKakaoIcon[5] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconKakaoShop_192x192
	{
		get
		{
			return m_AndroidKakaoShopIcon[0];
		}
		set
		{
			if (m_AndroidKakaoShopIcon[0] != value)
			{
				m_AndroidKakaoShopIcon[0] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconKakaoShop_144x144
	{
		get
		{
			return m_AndroidKakaoShopIcon[1];
		}
		set
		{
			if (m_AndroidKakaoShopIcon[1] != value)
			{
				m_AndroidKakaoShopIcon[1] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconKakaoShop_96x96
	{
		get
		{
			return m_AndroidKakaoShopIcon[2];
		}
		set
		{
			if (m_AndroidKakaoShopIcon[2] != value)
			{
				m_AndroidKakaoShopIcon[2] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconKakaoShop_72x72
	{
		get
		{
			return m_AndroidKakaoShopIcon[3];
		}
		set
		{
			if (m_AndroidKakaoShopIcon[3] != value)
			{
				m_AndroidKakaoShopIcon[3] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconKakaoShop_48x48
	{
		get
		{
			return m_AndroidKakaoShopIcon[4];
		}
		set
		{
			if (m_AndroidKakaoShopIcon[4] != value)
			{
				m_AndroidKakaoShopIcon[4] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D AndroidIconKakaoShop_36x36
	{
		get
		{
			return m_AndroidKakaoShopIcon[5];
		}
		set
		{
			if (m_AndroidKakaoShopIcon[5] != value)
			{
				m_AndroidKakaoShopIcon[5] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D IOSIconFB_152x152
	{
		get
		{
			return m_IOSFacebookIcon[0];
		}
		set
		{
			if (m_IOSFacebookIcon[0] != value)
			{
				m_IOSFacebookIcon[0] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D IOSIconFB_144x144
	{
		get
		{
			return m_IOSFacebookIcon[1];
		}
		set
		{
			if (m_IOSFacebookIcon[1] != value)
			{
				m_IOSFacebookIcon[1] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D IOSIconFB_120x120
	{
		get
		{
			return m_IOSFacebookIcon[2];
		}
		set
		{
			if (m_IOSFacebookIcon[2] != value)
			{
				m_IOSFacebookIcon[2] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D IOSIconFB_114x114
	{
		get
		{
			return m_IOSFacebookIcon[3];
		}
		set
		{
			if (m_IOSFacebookIcon[3] != value)
			{
				m_IOSFacebookIcon[3] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D IOSIconFB_76x76
	{
		get
		{
			return m_IOSFacebookIcon[4];
		}
		set
		{
			if (m_IOSFacebookIcon[4] != value)
			{
				m_IOSFacebookIcon[4] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D IOSIconFB_72x72
	{
		get
		{
			return m_IOSFacebookIcon[5];
		}
		set
		{
			if (m_IOSFacebookIcon[5] != value)
			{
				m_IOSFacebookIcon[5] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D IOSIconFB_57x57
	{
		get
		{
			return m_IOSFacebookIcon[6];
		}
		set
		{
			if (m_IOSFacebookIcon[6] != value)
			{
				m_IOSFacebookIcon[6] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D IOSIconKakao_152x152
	{
		get
		{
			return m_IOSKakaoIcon[0];
		}
		set
		{
			if (m_IOSKakaoIcon[0] != value)
			{
				m_IOSKakaoIcon[0] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D IOSIconKakao_144x144
	{
		get
		{
			return m_IOSKakaoIcon[1];
		}
		set
		{
			if (m_IOSKakaoIcon[1] != value)
			{
				m_IOSKakaoIcon[1] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D IOSIconKakao_120x120
	{
		get
		{
			return m_IOSKakaoIcon[2];
		}
		set
		{
			if (m_IOSKakaoIcon[2] != value)
			{
				m_IOSKakaoIcon[2] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D IOSIconKakao_114x114
	{
		get
		{
			return m_IOSKakaoIcon[3];
		}
		set
		{
			if (m_IOSKakaoIcon[3] != value)
			{
				m_IOSKakaoIcon[3] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D IOSIconKakao_76x76
	{
		get
		{
			return m_IOSKakaoIcon[4];
		}
		set
		{
			if (m_IOSKakaoIcon[4] != value)
			{
				m_IOSKakaoIcon[4] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D IOSIconKakao_72x72
	{
		get
		{
			return m_IOSKakaoIcon[5];
		}
		set
		{
			if (m_IOSKakaoIcon[5] != value)
			{
				m_IOSKakaoIcon[5] = value;
				DirtyEditor();
			}
		}
	}

	public Texture2D IOSIconKakao_57x57
	{
		get
		{
			return m_IOSKakaoIcon[6];
		}
		set
		{
			if (m_IOSKakaoIcon[6] != value)
			{
				m_IOSKakaoIcon[6] = value;
				DirtyEditor();
			}
		}
	}

	public List<GlobalDefine> GlobalDefines
	{
		get
		{
			return m_ListGlobalDefine;
		}
		set
		{
			m_ListGlobalDefine = value;
			DirtyEditor();
		}
	}

	public string Android_NA_Version
	{
		get
		{
			return m_Android_NA_Version;
		}
		set
		{
			m_Android_NA_Version = value;
		}
	}

	public string Android_KR_Version
	{
		get
		{
			return m_Android_KR_Version;
		}
		set
		{
			m_Android_KR_Version = value;
		}
	}

	public string IPhone_NA_Version
	{
		get
		{
			return m_IPhone_NA_Version;
		}
		set
		{
			m_IPhone_NA_Version = value;
		}
	}

	public string IPhone_KR_Version
	{
		get
		{
			return m_IPhone_KR_Version;
		}
		set
		{
			m_IPhone_KR_Version = value;
		}
	}

	public string Android_NA_VersionCode
	{
		get
		{
			return m_Android_NA_VersionCode;
		}
		set
		{
			m_Android_NA_VersionCode = value;
		}
	}

	public string Android_KR_VersionCode
	{
		get
		{
			return m_Android_KR_VersionCode;
		}
		set
		{
			m_Android_KR_VersionCode = value;
		}
	}

	public string IPhone_NA_VersionCode
	{
		get
		{
			return m_IPhone_NA_VersionCode;
		}
		set
		{
			m_IPhone_NA_VersionCode = value;
		}
	}

	public string IPhone_KR_VersionCode
	{
		get
		{
			return m_IPhone_KR_VersionCode;
		}
		set
		{
			m_IPhone_KR_VersionCode = value;
		}
	}

	private static void DirtyEditor()
	{
	}
}
