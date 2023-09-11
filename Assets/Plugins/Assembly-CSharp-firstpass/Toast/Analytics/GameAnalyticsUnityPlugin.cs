using UnityEngine;

namespace Toast.Analytics
{
	public class GameAnalyticsUnityPlugin
	{
		public static GameAnalyticsUnityPlugin _instance;

		public GameAnalyticsUnityPluginController _controller;

		public static GameAnalyticsUnityPlugin instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GameAnalyticsUnityPlugin();
				}
				return _instance;
			}
		}

		public GameAnalyticsUnityPluginController controller
		{
			get
			{
				if (_controller == null)
				{
					((GameAnalyticsUnityPluginController)Object.FindObjectOfType(typeof(GameAnalyticsUnityPluginController))).Awake();
				}
				return _controller;
			}
			set
			{
				if (!(_controller == value))
				{
					_controller = value;
				}
			}
		}
	}
}
