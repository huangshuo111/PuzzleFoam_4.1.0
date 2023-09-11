using System;
using Bridge;
using Network;
using UnityEngine;

public class ChallengeMenu : MonoBehaviour
{
	private Vector3 basePos_;

	private Vector3 prevMapPos_;

	private Transform map_;

	private int duration_;

	private UILabel backLimitLabel_;

	private UILabel challengeLimitLable_;

	private static long getLimitTime_;

	private GameData gameData_;

	private int eventNum;

	private void Start()
	{
		basePos_ = base.transform.localPosition;
		base.transform.localPosition = basePos_;
		backLimitLabel_ = base.gameObject.transform.Find("BackButton/energyLabel").GetComponent<UILabel>();
		challengeLimitLable_ = base.gameObject.transform.Find("ChallengeButton/energyLabel").GetComponent<UILabel>();
		gameData_ = GlobalData.Instance.getGameData();
	}

	private void Update()
	{
		if (map_ == null)
		{
			map_ = base.transform.parent.parent.Find("DragCamera(Clone)/DragObject/MapCamera");
			prevMapPos_ = map_.localPosition;
		}
		base.transform.localPosition += (prevMapPos_ - map_.localPosition) * 0.5f;
		prevMapPos_ = map_.localPosition;
		Vector3 vector = basePos_ - base.transform.localPosition;
		vector.z = 0f;
		float num = Time.deltaTime;
		if (num > 1f)
		{
			num = 1f;
		}
		base.transform.localPosition += vector * num;
		if (gameData_ != null)
		{
			int num2 = (int)((getLimitTime_ + gameData_.eventTimeSsRemaining) / 3600);
			int num3 = (int)((getLimitTime_ + gameData_.eventTimeSsRemaining) % 3600 / 60);
			int num4 = num2 - DateTime.Now.Hour;
			int num5 = num3 - DateTime.Now.Minute;
			if (num5 < 0)
			{
				num4--;
				num5 = 60 + num5;
			}
			if (num4 < 0 || eventNum != 2)
			{
				UILabel uILabel = challengeLimitLable_;
				string text = "00:00";
				backLimitLabel_.text = text;
				uILabel.text = text;
			}
			else
			{
				UILabel uILabel2 = challengeLimitLable_;
				string text = num4.ToString("00") + ":" + num5.ToString("00");
				backLimitLabel_.text = text;
				uILabel2.text = text;
			}
		}
	}

	public void updateEnable(PartManager.ePart part)
	{
		SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
		bool flag = networkData.getEventNo() == 2;
		bool flag2 = Bridge.PlayerData.getCurrentStage() >= 30;
		eventNum = networkData.getEventNo();
		base.gameObject.SetActive((flag && flag2 && isEventDuration()) || part == PartManager.ePart.ChallengeMap);
		if (base.gameObject.activeSelf)
		{
			if (part == PartManager.ePart.Map)
			{
				base.gameObject.transform.Find("ChallengeButton").gameObject.SetActive(true);
				base.gameObject.transform.Find("BackButton").gameObject.SetActive(false);
			}
			else
			{
				base.gameObject.transform.Find("ChallengeButton").gameObject.SetActive(false);
				base.gameObject.transform.Find("BackButton").gameObject.SetActive(true);
			}
		}
	}

	public void challengeSaleCheck(PartManager.ePart part)
	{
		if (GlobalData.Instance.getGameData().saleArea == null)
		{
			return;
		}
		bool flag = false;
		UISprite component = base.gameObject.transform.Find("ChallengeButton/icon_sale").GetComponent<UISprite>();
		int[] saleArea = GlobalData.Instance.getGameData().saleArea;
		foreach (int num in saleArea)
		{
			if (num == 2 * Constant.Event.BaseEventStageNo)
			{
				flag = true;
				break;
			}
		}
		if (part == PartManager.ePart.Map && flag)
		{
			component.gameObject.SetActive(true);
		}
		else
		{
			component.gameObject.SetActive(false);
		}
	}

	public bool isEventDuration()
	{
		if (gameData_ == null)
		{
			gameData_ = GlobalData.Instance.getGameData();
		}
		if (getLimitTime_ + gameData_.eventTimeSsRemaining < getNowTimeBySeconds())
		{
			return false;
		}
		return true;
	}

	public static bool isTermsStageClear(EventStageInfo.Info info)
	{
		return true;
	}

	public static bool isPrevLevelClear(EventStageInfo.Info info)
	{
		if (info.Level != 1)
		{
			int stageNo = info.Common.StageNo - 1;
			if (!Bridge.StageData.isClear(stageNo))
			{
				return false;
			}
		}
		return true;
	}

	private int getNowTimeBySeconds()
	{
		return DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
	}

	public static void updateGetTime()
	{
		getLimitTime_ = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
	}
}
