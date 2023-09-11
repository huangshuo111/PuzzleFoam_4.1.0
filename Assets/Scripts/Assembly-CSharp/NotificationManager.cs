using Network;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
	public enum NotificationCode
	{
		Heart = 0,
		ComebackThreeDay = 1,
		ComebackOneWeek = 2,
		ComebackFortnight = 3,
		ComebackThirty = 4,
		Comeback45 = 1001,
		Comeback60 = 1002,
		Comeback75 = 1003,
		Comeback90 = 1004,
		BeginnerLogin = 5,
		BeginnerLoginOneDay = 6,
		BeginnerLoginTwoDay = 7,
		BeginnerLoginThreeDay = 8,
		BeginnerLoginFourDay = 9,
		BeginnerLoginFiveDay = 10,
		BeginnerLoginSixDay = 11,
		BeginnerLoginSevenDay = 12,
		MAX = 13
	}

	private float cancelTime = -1f;

	private static NotificationManager instance;

	public static NotificationManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void Start()
	{
		cancel(0);
	}

	private void Update()
	{
		if (!(cancelTime < 0f) && Time.realtimeSinceStartup > cancelTime)
		{
			cancel(0);
		}
	}

	public void schedule(int time, int messageID, int code, int rate, int diff)
	{
		cancel(code);
		if (!SaveData.Instance.getSystemData().getOptionData().getFlag(SaveOptionData.eFlag.PushNotice) || time * rate < diff)
		{
			return;
		}
		GameData gameData = GlobalData.Instance.getGameData();
		int num;
		if (code == 0)
		{
			if (gameData.heart >= Constant.AutoRecoveryHeartMax)
			{
				PlayerPrefs.DeleteKey("HeartNotificationReserveTime");
				PlayerPrefs.DeleteKey("HeartSetTime");
				return;
			}
			num = time + gameData.heartRecoverTime * (Constant.AutoRecoveryHeartMax - gameData.heart - 1);
			Debug.Log("delay = " + num);
		}
		else
		{
			num = time;
		}
		Debug.Log("Notification_date :  time = " + time + " : diff = " + diff + " : rate = " + rate);
		MessageResource messageResource = MessageResource.Instance;
		GKUnityPluginController.Instance.SetLocalNotification(num, messageResource.getMessage(500), messageResource.getMessage(messageID), code, rate, diff);
		cancelTime = Time.realtimeSinceStartup + (float)num - 3f;
	}

	public void cancel(int code)
	{
		GKUnityPluginController.CallAndroidFunc("CancelLocalNotification", code);
		cancelTime = -1f;
	}

	public void delete()
	{
		GKUnityPluginController.CallAndroidFunc("com.gumikorea.framework.GCM.LocalNotificationReceiver", "delete");
	}
}
