using System;
using System.Collections;
using Bridge;
using Network;
using UnityEngine;

public class DailyMission : MonoBehaviour
{
	public bool missionCleared;

	public bool bonusgamePlayed;

	public int missionNum;

	public int mission_target;

	public int mission_now;

	public string dateKey = string.Empty;

	public string missionShort = string.Empty;

	public string missionLong = string.Empty;

	private UILabel missionLabel;

	private UILabel missionCondition;

	private UISlider missionSlider;

	public bool buffer;

	public UIButton popupBtn;

	private int updateDay = -1;

	public static bool bMissionCreate;

	private static long getLimitTime_;

	private GameData gameData_;

	private void Start()
	{
		gameData_ = GlobalData.Instance.getGameData();
	}

	private void Update()
	{
		if (bonusgamePlayed && gameData_ != null)
		{
			int num = (int)((getLimitTime_ + gameData_.dailyMissionSsRemaining) / 3600);
			int num2 = (int)((getLimitTime_ + gameData_.dailyMissionSsRemaining) % 3600 / 60);
			int num3 = num - DateTime.Now.Hour;
			int num4 = num2 - DateTime.Now.Minute;
			if (num4 < 0)
			{
				num3--;
				num4 = 60 + num4;
			}
			if (num3 < 0 || updateDay != DateTime.Now.Day)
			{
				missionCondition.text = "00:00";
			}
			else
			{
				missionCondition.text = num3.ToString("00") + ":" + num4.ToString("00");
			}
		}
	}

	public IEnumerator dailyMissionInfoSetup()
	{
		missionLabel = base.transform.Find("Label_txt").GetComponent<UILabel>();
		missionCondition = base.transform.Find("Label_num").GetComponent<UILabel>();
		missionSlider = base.transform.Find("bg_root/daily_Bar").GetComponent<UISlider>();
		missionSlider.sliderValue = 0f;
		popupBtn = base.transform.Find("popup_button/popup_root").GetComponent<UIButton>();
		if (bonusgamePlayed)
		{
			missionLabel.text = string.Empty + MessageResource.Instance.getMessage(4517);
			missionSlider.sliderValue = 1f;
			popupBtn.setEnable(false);
			updateDay = DateTime.Now.Day;
		}
		else
		{
			int missionNumber = 4700 + (missionNum - 1);
			missionLabel.text = string.Empty + MessageResource.Instance.getMessage(missionNumber);
			if (mission_now >= mission_target)
			{
				missionCleared = true;
			}
			missionCondition.text = string.Empty + mission_now + "/" + mission_target;
			missionSlider.sliderValue = (float)mission_now / (float)mission_target;
			popupBtn.setEnable(true);
		}
		yield return null;
	}

	public bool updateDailyMissionData()
	{
		Network.DailyMission dailyMissionData = GlobalData.Instance.getDailyMissionData();
		if (dailyMissionData == null)
		{
			return false;
		}
		missionNum = dailyMissionData.type;
		dateKey = dailyMissionData.dateKey;
		mission_target = dailyMissionData.clearCount;
		mission_now = dailyMissionData.nowCount;
		if (dailyMissionData.receiveFlg == 1)
		{
			missionCleared = false;
			bonusgamePlayed = false;
		}
		else if (dailyMissionData.receiveFlg == 2)
		{
			missionCleared = true;
			bonusgamePlayed = false;
		}
		else if (dailyMissionData.receiveFlg >= 3)
		{
			bonusgamePlayed = true;
		}
		return true;
	}

	public bool dailyMissionChangeCheck()
	{
		if (gameData_ != null)
		{
			int num = (int)((getLimitTime_ + gameData_.dailyMissionSsRemaining) / 3600);
			int num2 = (int)((getLimitTime_ + gameData_.dailyMissionSsRemaining) % 3600 / 60);
			int num3 = num - DateTime.Now.Hour;
			int num4 = num2 - DateTime.Now.Minute;
			if (num4 < 0)
			{
				num3--;
				num4 = 60 + num4;
			}
			return num3 <= 0 || updateDay != DateTime.Now.Day;
		}
		return false;
	}

	public static bool isTermClear()
	{
		return Bridge.StageData.isClear(5);
	}

	public static void updateGetTime()
	{
		getLimitTime_ = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
	}
}
