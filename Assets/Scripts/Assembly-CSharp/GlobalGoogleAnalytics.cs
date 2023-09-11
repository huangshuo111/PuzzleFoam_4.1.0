using UnityEngine;

public class GlobalGoogleAnalytics : MonoBehaviour
{
	public GoogleAnalyticsV4 googleAnalytics;

	private static GlobalGoogleAnalytics instance_;

	public static GlobalGoogleAnalytics Instance
	{
		get
		{
			return instance_;
		}
	}

	public static bool IsInstance()
	{
		return (!(instance_ == null)) ? true : false;
	}

	private void Awake()
	{
		if (instance_ == null)
		{
			instance_ = this;
			Object.DontDestroyOnLoad(this);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		googleAnalytics.StopSession();
		instance_ = null;
	}

	private void DispatchHits()
	{
		googleAnalytics.DispatchHits();
	}

	public void StartSession()
	{
		googleAnalytics.StartSession();
	}

	public void StopSession()
	{
		googleAnalytics.StopSession();
	}

	public void LogScreen(string _title)
	{
		googleAnalytics.LogScreen(_title);
	}

	public void LogEvent(string _eventCategory, string _eventAction, string _eventLabel, long _value)
	{
		googleAnalytics.LogEvent(_eventCategory, _eventAction, _eventLabel, _value);
	}

	public void LogException(string _exceptionDescription, bool _isFatal)
	{
		googleAnalytics.LogException(_exceptionDescription, _isFatal);
	}

	public void LogTiming(string _timingCategory, long _timingInterval, string _timingName, string _timingLabel)
	{
		googleAnalytics.LogTiming(_timingCategory, _timingInterval, _timingName, _timingLabel);
	}

	public void LogSocial(string _socialNetwork, string _socialAction, string _socialTarget)
	{
		googleAnalytics.LogSocial(_socialNetwork, _socialAction, _socialTarget);
	}

	public void LogTransaction(string _transID, string _affiliation, double _revenue, double _tax, double _shipping)
	{
		googleAnalytics.LogTransaction(_transID, _affiliation, _revenue, _tax, _shipping);
	}

	public void LogTransaction(string _transID, string _affiliation, double _revenue, double _tax, double _shipping, string _currencyCode)
	{
		googleAnalytics.LogTransaction(_transID, _affiliation, _revenue, _tax, _shipping, _currencyCode);
	}

	public void LogItem(string _transID, string _name, string _SKU, string _category, double _price, long _quantity)
	{
		googleAnalytics.LogItem(_transID, _name, _SKU, _category, _price, _quantity);
	}

	public void LogItem(string _transID, string _name, string _SKU, string _category, double _price, long _quantity, string _currencyCode)
	{
		googleAnalytics.LogItem(_transID, _name, _SKU, _category, _price, _quantity, _currencyCode);
	}
}
