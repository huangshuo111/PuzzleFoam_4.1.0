using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace Toast.Analytics
{
	public class GameAnalyticsWebPlayer : MonoBehaviour
	{
		public class TraceLogSerializer
		{
			private Dictionary<string, string> _basicFields = new Dictionary<string, string>();

			private Dictionary<string, decimal> _basicNumberFields = new Dictionary<string, decimal>();

			private Dictionary<string, string> _extendedFields = new Dictionary<string, string>();

			private Dictionary<string, decimal> _extendedNumberFields = new Dictionary<string, decimal>();

			public bool IsDebug { get; set; }

			public void AddExtendedLevelField(int level)
			{
				_extendedNumberFields.Add("lv", level);
			}

			public void AddMandatoryFields(TraceLogMandatoryField mandatory)
			{
				_basicFields.Add("pv", mandatory.ProtocolVersion);
				_basicFields.Add("kv", mandatory.SDKVersion);
				_basicFields.Add("cid", mandatory.CompanyId);
				_basicFields.Add("aid", mandatory.AppId);
				_basicFields.Add("av", mandatory.AppVersion);
				_basicFields.Add("uid", mandatory.UserId);
				_basicFields.Add("did", mandatory.DeviceId);
				_basicFields.Add("ip", mandatory.ClientIP);
				_basicFields.Add("t", mandatory.ActionType);
				_basicNumberFields.Add("ts", mandatory.ClientTimeStamp);
			}

			public void AddOptionalISFields(TraceLogOptionalISField optional)
			{
				_basicFields.Add("dnm", optional.DeviceName);
				_basicFields.Add("cr", optional.Carrier);
				_basicFields.Add("os", optional.OS);
				_basicFields.Add("osv", optional.OSVerions);
				_basicFields.Add("tz", optional.TimeZone);
				_basicFields.Add("cc", optional.CountryCode);
				_basicFields.Add("lc", optional.Locale);
			}

			public void AddOptionalCountryCodeField(string countryCode)
			{
				_basicFields.Add("cc", countryCode);
			}

			public void AddTraceInstallField(string store)
			{
				_basicFields.Add("store", store);
			}

			public void AddTraceDeactivateField(decimal duration)
			{
				_basicNumberFields.Add("du", duration);
			}

			public void AddTraceFinishFields(decimal duration)
			{
				_extendedNumberFields.Add("du", duration);
			}

			public void AddTracePurchaseFields(string itemCode, decimal unitCost, string currency, decimal payment)
			{
				_basicFields.Add("icd", itemCode);
				_basicNumberFields.Add("cost", unitCost);
				_basicFields.Add("curr", currency);
				_basicNumberFields.Add("pay", payment);
			}

			public void AddTraceGoodsFields(string usageCode, string type, string @do, decimal amount, decimal level)
			{
				_basicFields.Add("ucd", usageCode);
				_basicFields.Add("ty", type);
				_basicFields.Add("do", @do);
				_basicNumberFields.Add("am", amount);
				_basicNumberFields.Add("lv", level);
			}

			public void AddTraceLevelUpField(decimal level)
			{
				_basicNumberFields.Add("lv", level);
			}

			public void AddTraceEventFields(string eventType, string eventCode, string param1, string param2, decimal value)
			{
				_basicFields.Add("evt", eventType);
				_basicFields.Add("ev", eventCode);
				_basicFields.Add("prm1", param1);
				_basicFields.Add("prm2", param2);
				_basicNumberFields.Add("val", value);
			}

			public void AddTraceSpeedFields(string intervalName, decimal loadingTime)
			{
				_basicFields.Add("ivn", intervalName);
				_basicNumberFields.Add("ldt", loadingTime);
			}

			public void AddTraceFriendField(string friends)
			{
				_basicFields.Add("friends", friends);
			}

			public string BuildJSON()
			{
				if (IsDebug)
				{
					_basicFields.Add("debug", "on");
				}
				bool flag = true;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("{");
				foreach (KeyValuePair<string, string> basicField in _basicFields)
				{
					stringBuilder.Append((!flag) ? "," : string.Empty);
					flag = false;
					stringBuilder.AppendFormat("\"{0}\":\"{1}\"", basicField.Key, basicField.Value);
				}
				foreach (KeyValuePair<string, decimal> basicNumberField in _basicNumberFields)
				{
					stringBuilder.Append((!flag) ? "," : string.Empty);
					flag = false;
					stringBuilder.AppendFormat("\"{0}\":{1}", basicNumberField.Key, basicNumberField.Value);
				}
				if (_extendedFields.Count != 0 || _extendedNumberFields.Count != 0)
				{
					stringBuilder.Append(",");
					stringBuilder.AppendFormat("\"ex\":{0}", BuildExtendedJSON());
				}
				stringBuilder.Append("}");
				return stringBuilder.ToString();
			}

			private string BuildExtendedJSON()
			{
				bool flag = true;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("{");
				foreach (KeyValuePair<string, string> extendedField in _extendedFields)
				{
					stringBuilder.Append((!flag) ? "," : string.Empty);
					flag = false;
					stringBuilder.AppendFormat("\"{0}\":\"{1}\"", extendedField.Key, extendedField.Value);
				}
				foreach (KeyValuePair<string, decimal> extendedNumberField in _extendedNumberFields)
				{
					stringBuilder.Append((!flag) ? "," : string.Empty);
					flag = false;
					stringBuilder.AppendFormat("\"{0}\":{1}", extendedNumberField.Key, extendedNumberField.Value);
				}
				stringBuilder.Append("}");
				return stringBuilder.ToString();
			}
		}

		public class TraceLogMandatoryField
		{
			public string ProtocolVersion { get; set; }

			public string SDKVersion { get; set; }

			public string CompanyId { get; set; }

			public string AppId { get; set; }

			public string AppVersion { get; set; }

			public string UserId { get; set; }

			public string DeviceId { get; set; }

			public string ClientIP { get; set; }

			public int ClientTimeStamp { get; set; }

			public string ActionType { get; set; }
		}

		public class TraceLogOptionalISField
		{
			public string DeviceName { get; set; }

			public string Carrier { get; set; }

			public string OS { get; set; }

			public string OSVerions { get; set; }

			public string TimeZone { get; set; }

			public string CountryCode { get; set; }

			public string Locale { get; set; }
		}

		public static class ActionType
		{
			public const string TRACE_ACTIVATION = "a";

			public const string TRACE_DEACTIVATION = "d";

			public const string TRACE_PURCHASE = "p";

			public const string TRACE_MONEY = "g";

			public const string TRACE_LEVELUP = "l";

			public const string TRACE_EVENT = "e";

			public const string TRACE_FRIENDS_COUNT = "n";

			public const string TRACE_SPEED = "v";

			public const string TRACE_INSTALL = "i";

			public const string TRACE_START = "s";

			public const string TRACE_FINISH = "f";
		}

		public static class ReturnCode
		{
			public const int S_SUCCESS = 0;

			public const int W_ALREADY_INITIALIZED = 4096;

			public const int E_NOT_INITIALIZED = 32768;

			public const int E_SESSION_CLOSED = 32769;

			public const int E_INVALID_PARAMS = 32770;

			public const int E_ALREADY_EXISTS = 32771;

			public const int E_INTERNAL_ERROR = 32772;

			public const int E_INSUFFICIENT_OPERATION = 32773;

			public const int E_APP_ID_IS_EMPTY = 32774;

			public const int E_ENTERPRISE_ID_IS_EMPTY = 32775;

			public const int E_APP_VERSION_IS_EMPTY = 32776;

			public const int E_TOKEN_EMPTY = 32777;

			public const int E_PARENT_EMPTY = 32778;

			public const int E_LOGGING_USER_ID_IS_EMPTY = 32779;

			public const int E_CAMPAIGN_SHOW_EXPIRED = 28672;

			public const int E_CAMPAIGN_SHOW_ALREADY = 28673;

			public const int E_CAMPAIGN_SHOW_PENDING = 28674;

			public const int E_CAMPAIGN_SHOW_FAIL = 28675;

			public const int E_CAMPAIGN_SHOW_BLOCKED = 28676;

			public const int E_CAMPAIGN_NOTEXIST = 28677;

			public const int E_CAMPAIGN_DISABLED = 28678;

			public const int E_CAMPAIGN_USER_ID_IS_EMPTY = 28679;

			internal static readonly Dictionary<int, string> RESULT_STRING = new Dictionary<int, string>
			{
				{ 0, "S_SUCCESS" },
				{ 4096, "W_ALREADY_INITIALIZED" },
				{ 32768, "E_NOT_INITIALIZED" },
				{ 32769, "E_SESSION_CLOSED" },
				{ 32770, "E_INVALID_PARAMS" },
				{ 32771, "E_ALREADY_EXISTS" },
				{ 32772, "E_INTERNAL_ERROR" },
				{ 32773, "E_INSUFFICIENT_OPERATION" },
				{ 32774, "E_APP_ID_IS_EMPTY" },
				{ 32775, "E_ENTERPRISE_ID_IS_EMPTY" },
				{ 32776, "E_APP_VERSION_IS_EMPTY" },
				{ 32777, "E_TOKEN_EMPTY" },
				{ 32778, "E_PARENT_EMPTY" },
				{ 32779, "E_LOGGING_USER_ID_IS_EMPTY" },
				{ 28672, "E_CAMPAIGN_SHOW_EXPIRED" },
				{ 28673, "E_CAMPAIGN_SHOW_ALREADY" },
				{ 28674, "E_CAMPAIGN_SHOW_PENDING" },
				{ 28675, "E_CAMPAIGN_SHOW_FAIL" },
				{ 28676, "E_CAMPAIGN_SHOW_BLOCKED" },
				{ 28677, "E_CAMPAIGN_NOTEXIST" },
				{ 28678, "E_CAMPAIGN_DISABLED" },
				{ 28679, "E_CAMPAIGN_USER_ID_IS_EMPTY" }
			};
		}

		private const string SDK_VERSION = "1.3.2";

		private const string PROTOCOL_VERSION = "1.0";

		private const string AFLAT_SERVER_URL = "https://api-log-analytics.cloud.toast.com/am";

		private bool _useLoggingUserId;

		private Stopwatch _appStopwatch;

		private Stopwatch _activationStopwatch;

		private Dictionary<string, Stopwatch> _speedStopwatches;

		private string _appId;

		private string _companyId;

		private string _appVersion;

		public static GameAnalyticsWebPlayer UniqueInstance { get; private set; }

		public bool IsInitialized { get; private set; }

		public bool IsTraceStartLogged { get; private set; }

		public IDeviceCollectableInfo DeviceInfo { get; private set; }

		public bool DebugFlag { get; private set; }

		public string UserId { get; private set; }

		public bool UseLoggingUserId
		{
			get
			{
				return _useLoggingUserId;
			}
		}

		private GameAnalyticsWebPlayer()
		{
		}

		private void Start()
		{
			UniqueInstance = this;
		}

		public static string getVersion()
		{
			return "1.3.2";
		}

		public static void setDebugMode(bool enable)
		{
			UniqueInstance.DebugFlag = enable;
		}

		public static bool getDebugMode()
		{
			return UniqueInstance.DebugFlag;
		}

		public static string getDeviceInfo(string key)
		{
			return string.Empty;
		}

		public static int setUserId(string userId, bool useCampaignOrPromotion)
		{
			UniqueInstance.UserId = userId;
			if (UniqueInstance.UseLoggingUserId && UniqueInstance.IsInitialized && !UniqueInstance.IsTraceStartLogged)
			{
				UniqueInstance.TraceStart();
			}
			return 0;
		}

		public static string getResultMessage(int errorCode)
		{
			try
			{
				return ReturnCode.RESULT_STRING[errorCode];
			}
			catch (KeyNotFoundException)
			{
				return "ErrorCode doest NOT exist";
			}
		}

		public static int initializeSdk(string appId, string companyId, string appVersion, bool useLoggingUserId)
		{
			if (UniqueInstance.IsInitialized)
			{
				return 4096;
			}
			try
			{
				UniqueInstance.Initialize(appId, companyId, appVersion, useLoggingUserId);
				if (SystemInfo.operatingSystem.Contains("Windows"))
				{
					UniqueInstance.DeviceInfo = new WindowsCollectableInfo();
				}
				else
				{
					UniqueInstance.DeviceInfo = new MacCollaectableInfo();
				}
				if (!useLoggingUserId || UniqueInstance.UserId != null)
				{
					UniqueInstance.TraceStart();
				}
				return 0;
			}
			catch (Exception)
			{
				return 32772;
			}
		}

		public static int traceActivation()
		{
			return CommonTraceTask(delegate
			{
				UniqueInstance.TraceActivation();
				return 0;
			});
		}

		public static int traceDeactivation()
		{
			return CommonTraceTask(delegate
			{
				UniqueInstance.TraceDeactivation();
				return 0;
			});
		}

		public static int traceFriendCount(int friendCount)
		{
			return CommonTraceTask(delegate
			{
				UniqueInstance.TraceFriendCount(friendCount);
				return 0;
			});
		}

		public static int tracePurchase(string itemCode, float payment, float unitCost, string currency, int level)
		{
			return CommonTraceTask(delegate
			{
				UniqueInstance.TracePurchase(itemCode, payment, unitCost, currency, level);
				return 0;
			});
		}

		public static int traceMoneyAcquisition(string usageCode, string type, double acquistionAmount, int level)
		{
			return CommonTraceTask(delegate
			{
				UniqueInstance.TraceMoneyAcquisition(usageCode, type, acquistionAmount, level);
				return 0;
			});
		}

		public static int traceMoneyConsumption(string usageCode, string type, double consumptionAmount, int level)
		{
			return CommonTraceTask(delegate
			{
				UniqueInstance.TraceMoneyConsumption(usageCode, type, consumptionAmount, level);
				return 0;
			});
		}

		public static int traceLevelUp(int level)
		{
			return CommonTraceTask(delegate
			{
				UniqueInstance.TraceLevelUp(level);
				return 0;
			});
		}

		public static int traceEvent(string eventType, string eventCode, string param1, string param2, double value, int level)
		{
			return CommonTraceTask(delegate
			{
				UniqueInstance.TraceEvent(eventType, eventCode, param1, param2, value, level);
				return 0;
			});
		}

		public static int traceStartSpeed(string intervalName)
		{
			return CommonTraceTask(() => UniqueInstance.TraceStartSpeed(intervalName));
		}

		public static int traceEndSpeed(string intervalName)
		{
			return CommonTraceTask(() => UniqueInstance.TraceEndSpeed(intervalName));
		}

		private void Initialize(string appId, string companyId, string appVersion, bool useLoggingUserId)
		{
			_appId = appId;
			_companyId = companyId;
			_appVersion = appVersion;
			_useLoggingUserId = useLoggingUserId;
			IsInitialized = true;
			_appStopwatch = new Stopwatch();
			_activationStopwatch = new Stopwatch();
			_speedStopwatches = new Dictionary<string, Stopwatch>();
		}

		private static int CommonTraceTask(Func<int> predicate)
		{
			if (UniqueInstance == null || !UniqueInstance.IsInitialized)
			{
				return 32768;
			}
			if (!UniqueInstance.IsTraceStartLogged)
			{
				return 32769;
			}
			return predicate();
		}

		private int TraceStart()
		{
			SendTraceStart();
			IsTraceStartLogged = true;
			_appStopwatch.Start();
			return 0;
		}

		private void SendTraceStart()
		{
			TraceLogSerializer traceLogSerializer = new TraceLogSerializer();
			AssignDefaultMandatoryAndDebugFields(traceLogSerializer, "s", DebugFlag);
			AssignDefaultOptionalISFields(traceLogSerializer);
			SendTraceLog(traceLogSerializer);
		}

		public void TraceActivation()
		{
			_activationStopwatch.Reset();
			_activationStopwatch.Start();
			SendTraceActivation();
		}

		private void SendTraceActivation()
		{
			TraceLogSerializer traceLogSerializer = new TraceLogSerializer();
			AssignDefaultMandatoryAndDebugFields(traceLogSerializer, "a", DebugFlag);
			traceLogSerializer.AddOptionalCountryCodeField(DeviceInfo.CountryCode);
			SendTraceLog(traceLogSerializer);
		}

		private void TraceDeactivation()
		{
			_activationStopwatch.Stop();
			int duration = (int)_activationStopwatch.Elapsed.TotalSeconds;
			SendTraceDeactivation(duration);
		}

		private void SendTraceDeactivation(int duration)
		{
			TraceLogSerializer traceLogSerializer = new TraceLogSerializer();
			AssignDefaultMandatoryAndDebugFields(traceLogSerializer, "d", DebugFlag);
			traceLogSerializer.AddTraceDeactivateField(duration);
			SendTraceLog(traceLogSerializer);
		}

		private void TraceFriendCount(int friendCount)
		{
			SendTraceFriendCount(friendCount);
		}

		private void SendTraceFriendCount(int friendCount)
		{
			TraceLogSerializer traceLogSerializer = new TraceLogSerializer();
			AssignDefaultMandatoryAndDebugFields(traceLogSerializer, "n", DebugFlag);
			traceLogSerializer.AddTraceFriendField(friendCount.ToString());
			SendTraceLog(traceLogSerializer);
		}

		private void TracePurchase(string itemCode, float payment, float unitCost, string currency, int level)
		{
			SendTracePurchase(itemCode, (decimal)unitCost, currency, (decimal)payment, level);
		}

		private void SendTracePurchase(string itemCode, decimal unitCost, string currency, decimal payment, int level)
		{
			TraceLogSerializer traceLogSerializer = new TraceLogSerializer();
			AssignDefaultMandatoryAndDebugFields(traceLogSerializer, "p", DebugFlag);
			traceLogSerializer.AddTracePurchaseFields(itemCode, unitCost, currency, payment);
			traceLogSerializer.AddExtendedLevelField(level);
			traceLogSerializer.AddOptionalCountryCodeField(DeviceInfo.CountryCode);
			SendTraceLog(traceLogSerializer);
		}

		private void TraceMoneyAcquisition(string usageCode, string type, double acquistionAmount, int level)
		{
			SendTraceMoney(usageCode, type, "0", (decimal)acquistionAmount, level);
		}

		private void TraceMoneyConsumption(string usageCode, string type, double consumptionAmount, int level)
		{
			SendTraceMoney(usageCode, type, "1", (decimal)consumptionAmount, level);
		}

		private void SendTraceMoney(string usageCode, string type, string @do, decimal amount, decimal level)
		{
			TraceLogSerializer traceLogSerializer = new TraceLogSerializer();
			AssignDefaultMandatoryAndDebugFields(traceLogSerializer, "g", DebugFlag);
			traceLogSerializer.AddTraceGoodsFields(usageCode, type, @do, amount, level);
			SendTraceLog(traceLogSerializer);
		}

		private void TraceLevelUp(int level)
		{
			SendTraceLevelUp(level);
		}

		private void SendTraceLevelUp(decimal level)
		{
			TraceLogSerializer traceLogSerializer = new TraceLogSerializer();
			AssignDefaultMandatoryAndDebugFields(traceLogSerializer, "l", DebugFlag);
			traceLogSerializer.AddTraceLevelUpField(level);
			SendTraceLog(traceLogSerializer);
		}

		private void TraceEvent(string eventType, string eventCode, string param1, string param2, double value, int level)
		{
			SendTraceEvent(eventType, eventCode, param1, param2, (decimal)value, level);
		}

		private void SendTraceEvent(string eventType, string eventCode, string param1, string param2, decimal value, int level)
		{
			TraceLogSerializer traceLogSerializer = new TraceLogSerializer();
			AssignDefaultMandatoryAndDebugFields(traceLogSerializer, "e", DebugFlag);
			traceLogSerializer.AddTraceEventFields(eventType, eventCode, param1, param2, value);
			traceLogSerializer.AddExtendedLevelField(level);
			SendTraceLog(traceLogSerializer);
		}

		private int TraceStartSpeed(string intervalName)
		{
			if (_speedStopwatches.ContainsKey(intervalName))
			{
				return 32771;
			}
			_speedStopwatches.Add(intervalName, new Stopwatch());
			_speedStopwatches[intervalName].Start();
			return 0;
		}

		private int TraceEndSpeed(string intervalName)
		{
			if (!_speedStopwatches.ContainsKey(intervalName))
			{
				return 32773;
			}
			_speedStopwatches[intervalName].Stop();
			long elapsedMilliseconds = _speedStopwatches[intervalName].ElapsedMilliseconds;
			_speedStopwatches.Remove(intervalName);
			SendTraceSpeed(intervalName, elapsedMilliseconds);
			return 0;
		}

		private void SendTraceSpeed(string intervalName, decimal loadingTime)
		{
			TraceLogSerializer traceLogSerializer = new TraceLogSerializer();
			AssignDefaultMandatoryAndDebugFields(traceLogSerializer, "v", DebugFlag);
			traceLogSerializer.AddTraceSpeedFields(intervalName, loadingTime);
			SendTraceLog(traceLogSerializer);
		}

		private void SendTraceLog(TraceLogSerializer traceLog)
		{
			string s = traceLog.BuildJSON();
			string s2 = WWW.EscapeURL(s);
			WWW www = new WWW("https://api-log-analytics.cloud.toast.com/am", Encoding.UTF8.GetBytes(s2));
			StartCoroutine(WaitForRequest(www));
		}

		private TraceLogSerializer AssignDefaultMandatoryAndDebugFields(TraceLogSerializer logSerializer, string actionType, bool isDebug)
		{
			TraceLogMandatoryField traceLogMandatoryField = new TraceLogMandatoryField();
			traceLogMandatoryField.ProtocolVersion = "1.0";
			traceLogMandatoryField.SDKVersion = "1.3.2";
			traceLogMandatoryField.CompanyId = _companyId;
			traceLogMandatoryField.AppId = _appId;
			traceLogMandatoryField.AppVersion = _appVersion;
			traceLogMandatoryField.UserId = ((!_useLoggingUserId) ? DeviceInfo.DeviceID : UserId);
			traceLogMandatoryField.DeviceId = DeviceInfo.DeviceID;
			traceLogMandatoryField.ClientIP = DeviceInfo.IP;
			traceLogMandatoryField.ClientTimeStamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
			traceLogMandatoryField.ActionType = actionType;
			TraceLogMandatoryField mandatory = traceLogMandatoryField;
			logSerializer.AddMandatoryFields(mandatory);
			logSerializer.IsDebug = isDebug;
			return logSerializer;
		}

		public TraceLogSerializer AssignDefaultOptionalISFields(TraceLogSerializer logSerializaer)
		{
			TraceLogOptionalISField traceLogOptionalISField = new TraceLogOptionalISField();
			traceLogOptionalISField.DeviceName = DeviceInfo.DeviceName;
			traceLogOptionalISField.Carrier = DeviceInfo.Carrier;
			traceLogOptionalISField.OS = DeviceInfo.OS;
			traceLogOptionalISField.OSVerions = DeviceInfo.OSVersion;
			traceLogOptionalISField.TimeZone = DeviceInfo.TimeZone;
			traceLogOptionalISField.CountryCode = DeviceInfo.CountryCode;
			traceLogOptionalISField.Locale = DeviceInfo.Locale;
			TraceLogOptionalISField optional = traceLogOptionalISField;
			logSerializaer.AddOptionalISFields(optional);
			return logSerializaer;
		}

		private IEnumerator WaitForRequest(WWW www)
		{
			yield return www;
			if (www.error == null)
			{
				string headerStr = string.Empty;
				foreach (KeyValuePair<string, string> entry in www.responseHeaders)
				{
					string text = headerStr;
					headerStr = text + entry.Key + " : " + entry.Value + "\n";
				}
				UnityEngine.Debug.Log("HTTP Response Header\n" + headerStr);
			}
			else
			{
				UnityEngine.Debug.Log("HTTP Request Failed. Reason : " + www.error);
			}
		}
	}
}
