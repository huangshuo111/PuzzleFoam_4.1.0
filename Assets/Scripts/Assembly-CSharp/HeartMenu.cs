using System;
using Bridge;
using Network;
using UnityEngine;

public class HeartMenu : MonoBehaviour
{
	[SerializeField]
	private UILabel Label;

	[SerializeField]
	private UILabel HeartLabel;

	private bool bSetuped_;

	private int time_;

	private float startTime_;

	private GameObject campaign;

	private GameData gameData_;

	private void Awake()
	{
	}

	public void init()
	{
		campaign = base.transform.Find("campaign").gameObject;
		campaign.SetActive(false);
		gameData_ = GlobalData.Instance.getGameData();
		campaign.SetActive(gameData_.isHeartCampaign);
		time_ = gameData_.heartRecoverySsRemaining;
		DateTime now = DateTime.Now;
		Debug.Log("date.Hour = " + now.Hour);
		Debug.Log("date.Minute = " + now.Minute);
		int value = now.Hour * 60 * 60 + now.Minute * 60 + now.Second;
		PlayerPrefs.SetInt("HeartNotificationReserveTime", value);
		int num = time_;
		PlayerPrefs.SetInt("HeartSetTime", num);
		NotificationManager.Instance.schedule(num, 1476, 0, 1, 0);
		updateLabel();
		startTime_ = Time.realtimeSinceStartup;
		bSetuped_ = true;
	}

	public void updateRemainingTime()
	{
		campaign.SetActive(gameData_.isHeartCampaign);
		time_ = gameData_.heartRecoverySsRemaining;
		updateLabel();
		startTime_ = Time.realtimeSinceStartup;
		updateTime();
		Debug.Log("gameData_.heartRecoverySsRemaining =" + gameData_.heartRecoverySsRemaining);
		DateTime now = DateTime.Now;
		Debug.Log("date.Hour = " + now.Hour);
		Debug.Log("date.Minute = " + now.Minute);
		int value = now.Hour * 60 * 60 + now.Minute * 60 + now.Second;
		PlayerPrefs.SetInt("HeartNotificationReserveTime", value);
		int num = time_;
		PlayerPrefs.SetInt("HeartSetTime", num);
		NotificationManager.Instance.schedule(num, 1476, 0, 1, 0);
	}

	private void Update()
	{
		if (bSetuped_ && updateTime())
		{
			addHeart();
			resetTime();
		}
	}

	private void addHeart()
	{
		addHeart(1, true);
	}

	public void addHeart(int num)
	{
		addHeart(num, false);
	}

	private void addHeart(int num, bool bAutoRecovary)
	{
		int heart = Bridge.PlayerData.getHeart();
		if (bAutoRecovary && heart > Constant.AutoRecoveryHeartMax)
		{
			updateLabel();
			return;
		}
		heart += num;
		if (bAutoRecovary)
		{
			heart = Mathf.Min(heart, Constant.AutoRecoveryHeartMax);
		}
		int num2 = Mathf.Min(heart, Constant.HeartMax);
		Bridge.PlayerData.setHeart(num2);
		DateTime now = DateTime.Now;
		Debug.Log("date.Hour = " + now.Hour);
		Debug.Log("date.Minute = " + now.Minute);
		int value = now.Hour * 60 * 60 + now.Minute * 60 + now.Second;
		PlayerPrefs.SetInt("HeartNotificationReserveTime", value);
		int num3 = (Constant.HeartMax - num2) * gameData_.heartRecoverTime;
		PlayerPrefs.SetInt("HeartSetTime", num3);
		NotificationManager.Instance.schedule(num3, 1476, 0, 1, 0);
		updateLabel();
	}

	public void subHeart(int num)
	{
		int heart = Bridge.PlayerData.getHeart();
		int num2 = heart;
		heart -= num;
		if (heart < 0)
		{
			heart = 0;
		}
		if (num2 >= Constant.AutoRecoveryHeartMax && heart < Constant.AutoRecoveryHeartMax)
		{
			resetTime();
		}
		gameData_.heart = heart;
		DateTime now = DateTime.Now;
		Debug.Log("date.Hour = " + now.Hour);
		Debug.Log("date.Minute = " + now.Minute);
		int value = now.Hour * 60 * 60 + now.Minute * 60 + now.Second;
		PlayerPrefs.SetInt("HeartNotificationReserveTime", value);
		int num3 = time_ - Mathf.RoundToInt(Time.realtimeSinceStartup - startTime_);
		PlayerPrefs.SetInt("HeartSetTime", num3);
		NotificationManager.Instance.schedule(num3, 1476, 0, 1, 0);
		updateLabel();
	}

	public void updateLabel()
	{
		int heart = Bridge.PlayerData.getHeart();
		if (heart > Constant.AutoRecoveryHeartMax)
		{
			string message = MessageResource.Instance.getMessage(21);
			message = MessageResource.Instance.castCtrlCode(message, 1, (heart - Constant.AutoRecoveryHeartMax).ToString());
			Label.text = message;
		}
		else if (heart == Constant.AutoRecoveryHeartMax)
		{
			Label.text = MessageResource.Instance.getMessage(20);
		}
		else
		{
			updateTime();
		}
		HeartLabel.text = Mathf.Min(heart, Constant.AutoRecoveryHeartMax).ToString();
	}

	private void resetTime()
	{
		int m = 0;
		int s = 0;
		startTime_ = Time.realtimeSinceStartup;
		calcTime(gameData_.heartRecoverTime, ref m, ref s);
		time_ = gameData_.heartRecoverTime;
	}

	private bool updateTime()
	{
		int heart = Bridge.PlayerData.getHeart();
		if (heart >= Constant.AutoRecoveryHeartMax)
		{
			return false;
		}
		float num = (float)time_ - (Time.realtimeSinceStartup - startTime_) + 0.999999f;
		int m = 0;
		int s = 0;
		calcTime(num, ref m, ref s);
		Label.text = m + ":" + s.ToString("D2");
		if (num <= 0f)
		{
			return true;
		}
		return false;
	}

	private void calcTime(float time, ref int m, ref int s)
	{
		m = (int)(time / 60f);
		s = (int)(time % 60f);
	}
}
