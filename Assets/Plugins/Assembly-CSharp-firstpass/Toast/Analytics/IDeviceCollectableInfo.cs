namespace Toast.Analytics
{
	public interface IDeviceCollectableInfo
	{
		string OS { get; }

		string DeviceName { get; }

		string OSVersion { get; }

		string IP { get; }

		string DeviceID { get; }

		string TimeZone { get; }

		string Locale { get; }

		string CountryCode { get; }

		string Carrier { get; }
	}
}
