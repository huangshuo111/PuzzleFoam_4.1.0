using System;
using UnityEngine;

namespace Toast.Analytics
{
	public class WindowsCollectableInfo : IDeviceCollectableInfo
	{
		public string OS
		{
			get
			{
				return SystemInfo.operatingSystem;
			}
		}

		public string DeviceName
		{
			get
			{
				return SystemInfo.deviceModel;
			}
		}

		public string OSVersion
		{
			get
			{
				return SystemInfo.operatingSystem;
			}
		}

		public string IP
		{
			get
			{
				return Network.player.ipAddress;
			}
		}

		public string DeviceID
		{
			get
			{
				return SystemInfo.deviceUniqueIdentifier;
			}
		}

		public string TimeZone
		{
			get
			{
				return System.TimeZone.CurrentTimeZone.StandardName;
			}
		}

		public string Locale
		{
			get
			{
				return Application.systemLanguage.ToString();
			}
		}

		public string CountryCode
		{
			get
			{
				return string.Empty;
			}
		}

		public string Carrier
		{
			get
			{
				return Application.platform.ToString();
			}
		}
	}
}
