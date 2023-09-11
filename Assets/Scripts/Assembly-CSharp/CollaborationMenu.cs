using System;
using Bridge;
using Network;
using UnityEngine;

public class CollaborationMenu : MonoBehaviour
{
	private Vector3 basePos_;

	private Vector3 prevMapPos_;

	private Transform map_;

	private UILabel backLimitLabel_;

	private UILabel collaborationLimitLabel_;

	private static long getLimitTime_;

	private GameData gameData_;

	private int eventNum;

	private void Start()
	{
		basePos_ = base.transform.localPosition;
		base.transform.localPosition = basePos_;
		backLimitLabel_ = base.gameObject.transform.Find("BackButton/energyLabel").GetComponent<UILabel>();
		collaborationLimitLabel_ = base.gameObject.transform.Find("CollaborationButton/energyLabel").GetComponent<UILabel>();
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
			if (num4 < 0 || eventNum != 11)
			{
				UILabel uILabel = collaborationLimitLabel_;
				string text = "00:00";
				backLimitLabel_.text = text;
				uILabel.text = text;
			}
			else
			{
				UILabel uILabel2 = collaborationLimitLabel_;
				string text = num4.ToString("00") + ":" + num5.ToString("00");
				backLimitLabel_.text = text;
				uILabel2.text = text;
			}
		}
	}

	public void updateEnable(PartManager.ePart part)
	{
		SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
		bool flag = networkData.getEventNo() == 11;
		bool flag2 = Bridge.PlayerData.getCurrentStage() >= 6;
		eventNum = networkData.getEventNo();
		Debug.Log("isEventDuration():" + isEventDuration());
		bool flag3 = ResourceLoader.Instance.isJapanResource();
		base.gameObject.SetActive((flag && flag2 && isEventDuration()) || part == PartManager.ePart.CollaborationMap);
		if (base.gameObject.activeSelf)
		{
			if (part == PartManager.ePart.Map)
			{
				base.gameObject.transform.Find("CollaborationButton").gameObject.SetActive(true);
				base.gameObject.transform.Find("BackButton").gameObject.SetActive(false);
			}
			else
			{
				base.gameObject.transform.Find("CollaborationButton").gameObject.SetActive(false);
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
		if (base.gameObject.activeSelf && base.gameObject.transform.Find("CollaborationButton/icon_sale") != null)
		{
			base.gameObject.transform.Find("CollaborationButton/icon_sale").gameObject.SetActive(isSale);
			if (isSale)
			{
				base.gameObject.transform.Find("CollaborationButton/icon_sale").GetComponent<UISprite>().spriteName = "UI_shop_sale_map";
			}
		}
	}

	public void updateStageItemEnable(bool isSale)
	{
		SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
		bool flag = networkData.getEventNo() == 1;
		bool flag2 = Bridge.PlayerData.getCurrentStage() >= 18;
		eventNum = networkData.getEventNo();
		if (base.gameObject.activeSelf && base.gameObject.transform.Find("CollaborationButton/icon_sale") != null)
		{
			base.gameObject.transform.Find("CollaborationButton/icon_sale").gameObject.SetActive(isSale);
			if (isSale)
			{
				base.gameObject.transform.Find("CollaborationButton/icon_sale").GetComponent<UISprite>().spriteName = "UI_shop_sale2_map";
			}
		}
	}

	public void CollaborationSaleCheck(PartManager.ePart part)
	{
		if (GlobalData.Instance.getGameData().saleArea == null)
		{
			return;
		}
		bool flag = false;
		UISprite component = base.gameObject.transform.Find("CollaborationButton/icon_sale").GetComponent<UISprite>();
		int[] saleArea = GlobalData.Instance.getGameData().saleArea;
		foreach (int num in saleArea)
		{
			if (num == 11 * Constant.Event.BaseEventStageNo)
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
