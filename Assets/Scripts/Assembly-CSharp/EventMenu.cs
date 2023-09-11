using System;
using Bridge;
using Network;
using UnityEngine;

public class EventMenu : MonoBehaviour
{
	private Vector3 basePos_;

	private Vector3 prevMapPos_;

	private Transform map_;

	private int duration_;

	private UILabel backLimitLabel_;

	private UILabel eventLimitLable_;

	private Vector3 EventButtonPosition;

	private static long getLimitTime_;

	private GameData gameData_;

	private int eventNum;

	private bool BBActive;

	private void Start()
	{
		basePos_ = base.transform.localPosition;
		eventLimitLable_ = base.gameObject.transform.Find("EventButton/energyLabel").GetComponent<UILabel>();
		backLimitLabel_ = base.gameObject.transform.Find("BackButton/energyLabel").GetComponent<UILabel>();
		gameData_ = GlobalData.Instance.getGameData();
	}

	private void Update()
	{
		if (map_ == null)
		{
			map_ = base.transform.parent.parent.Find("DragCamera(Clone)/DragObject/MapCamera");
			if (map_ != null)
			{
				prevMapPos_ = map_.localPosition;
			}
		}
		if (map_ != null)
		{
			base.transform.localPosition += (prevMapPos_ - map_.localPosition) * 0.5f;
			prevMapPos_ = map_.localPosition;
		}
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
			if (num4 < 0 || eventNum != 1)
			{
				UILabel uILabel = eventLimitLable_;
				string text = "00:00";
				backLimitLabel_.text = text;
				uILabel.text = text;
			}
			else
			{
				UILabel uILabel2 = eventLimitLable_;
				string text = num4.ToString("00") + ":" + num5.ToString("00");
				backLimitLabel_.text = text;
				uILabel2.text = text;
			}
		}
	}

	public void updateEnable(PartManager.ePart part)
	{
		SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
		bool flag = networkData.getEventNo() == 1;
		bool flag2 = Bridge.PlayerData.getCurrentStage() >= 18;
		eventNum = networkData.getEventNo();
		base.gameObject.SetActive((flag && flag2 && isEventDuration()) || part == PartManager.ePart.EventMap);
		if (base.gameObject.activeSelf)
		{
			if (part == PartManager.ePart.Map)
			{
				base.gameObject.transform.Find("EventButton").gameObject.SetActive(true);
				base.gameObject.transform.Find("BackButton").gameObject.SetActive(false);
			}
			else
			{
				base.gameObject.transform.Find("EventButton").gameObject.SetActive(false);
				base.gameObject.transform.Find("BackButton").gameObject.SetActive(true);
			}
		}
	}

	public void updateAreaItemEnable(bool isSale)
	{
		SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
		bool flag = networkData.getEventNo() == 1;
		bool flag2 = Bridge.PlayerData.getCurrentStage() >= 18;
		eventNum = networkData.getEventNo();
		if (base.gameObject.activeSelf && base.gameObject.transform.Find("EventButton/icon_sale") != null)
		{
			base.gameObject.transform.Find("EventButton/icon_sale").gameObject.SetActive(isSale);
			if (isSale)
			{
				base.gameObject.transform.Find("EventButton/icon_sale").GetComponent<UISprite>().spriteName = "UI_shop_sale_map";
			}
		}
	}

	public void updateStageItemEnable(bool isSale)
	{
		SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
		bool flag = networkData.getEventNo() == 1;
		bool flag2 = Bridge.PlayerData.getCurrentStage() >= 18;
		eventNum = networkData.getEventNo();
		if (base.gameObject.activeSelf && base.gameObject.transform.Find("EventButton/icon_sale") != null)
		{
			base.gameObject.transform.Find("EventButton/icon_sale").gameObject.SetActive(isSale);
			if (isSale)
			{
				base.gameObject.transform.Find("EventButton/icon_sale").GetComponent<UISprite>().spriteName = "UI_shop_sale2_map";
			}
		}
	}

	public bool getEnable()
	{
		SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
		bool flag = networkData.getEventNo() == 1;
		bool flag2 = Bridge.PlayerData.getCurrentStage() >= 18;
		return flag && flag2 && isEventDuration();
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
		int entryTerms = info.EntryTerms;
		if (entryTerms == -1)
		{
			return true;
		}
		if (!Bridge.StageData.isClear(entryTerms - 1))
		{
			return false;
		}
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

	public void SetCrossBBActive(bool _BBActive, PartManager.ePart part)
	{
		BBActive = _BBActive;
		EventButtonPosition.x = -250f;
		EventButtonPosition.z = 0f;
		if (BBActive && part == PartManager.ePart.Map)
		{
			EventButtonPosition.y = 250f;
		}
		else
		{
			EventButtonPosition.y = 355f;
		}
		base.gameObject.transform.Find("EventButton").gameObject.transform.localPosition = EventButtonPosition;
		basePos_ = base.transform.localPosition;
	}
}
