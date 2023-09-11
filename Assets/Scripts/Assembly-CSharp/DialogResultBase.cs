using System.Collections;
using UnityEngine;

public abstract class DialogResultBase : DialogBase
{
	public enum eBtn
	{
		None = 0,
		Close = 1,
		Next = 2,
		Retry = 3
	}

	protected enum eLabel
	{
		Score = 0,
		Stage = 1,
		HighScore = 2,
		Bonus = 3,
		Target = 4,
		Coin = 5,
		Jewel = 6,
		Exp = 7,
		BonusCoin = 8,
		BonusJewel = 9,
		MyScore = 10,
		RemainingBonus = 11,
		MyScoreLabel = 12,
		RemainingBonusLabel = 13,
		LevelBonusLabel = 14,
		MaxCombo = 15,
		MaxComboLabel = 16,
		ComboBonus = 17,
		CampaignCoin = 18,
		SkillBonusCoin = 19,
		StageLabel = 20,
		Key = 21,
		Item = 22,
		Multiply = 23,
		LevelBonusRate = 24,
		Max = 25
	}

	protected delegate void OnSetTextCB(int value);

	private EventStageInfo.Info eventStageInfo;

	private int eventNo;

	protected UILabel[] labels_ = new UILabel[25];

	protected UIButton[] buttons_;

	protected DialogPlayScore scoreDialog_;

	protected SaveGameData gameData_;

	private eBtn pressBtn_;

	protected bool isCampaign_;

	protected bool isEvent_;

	protected bool isCollaboration_;

	protected bool isClear;

	protected GameObject coinLabel;

	protected GameObject coinBalloon;

	protected string[] balloonSpriteName = new string[4] { "event_bonus", "coin_bonus", "skill_bonus", "friend_bonus" };

	public override void OnCreate()
	{
		gameData_ = SaveData.Instance.getGameData();
		scoreDialog_ = dialogManager_.getDialog(DialogManager.eDialog.PlayScore) as DialogPlayScore;
		buttons_ = GetComponentsInChildren<UIButton>(true);
		isCampaign_ = GlobalData.Instance.getGameData().isCoinupCampaign;
	}

	private void init(bool isEvent, bool isCollaboration)
	{
		isEvent_ = isEvent;
		isCollaboration_ = isCollaboration;
		Transform transform = base.transform.Find("window/reward");
		Transform transform2 = base.transform.Find("window/reward_campaign");
		Transform transform3 = base.transform.Find("window/reward_event");
		Transform transform4 = base.transform.Find("window/reward_collaboration");
		string[] array = null;
		if (isEvent_)
		{
			array = new string[3] { "reward", "reward_campaign", null };
			Object.Destroy(transform.gameObject);
			Object.Destroy(transform2.gameObject);
			if (isCollaboration_)
			{
				array[2] = "reward_event";
				Object.Destroy(transform3.gameObject);
				transform4.gameObject.SetActive(true);
				coinLabel = transform4.Find("reward_coin/Label_bonus").gameObject;
				coinBalloon = transform4.Find("reward_coin/icon_bonus").gameObject;
			}
			else
			{
				array[2] = "reward_collaboration";
				Object.Destroy(transform4.gameObject);
				transform3.gameObject.SetActive(true);
				coinLabel = transform3.Find("reward_coin/Label_bonus").gameObject;
				coinBalloon = transform3.Find("reward_coin/icon_bonus").gameObject;
				transform3.Find("reward_coin/icon_event").gameObject.SetActive(false);
				transform3.Find("reward_coin/Label_event").gameObject.SetActive(false);
			}
		}
		else if (isCampaign_)
		{
			array = new string[3] { "reward", "reward_event", "reward_collaboration" };
			Object.Destroy(transform.gameObject);
			Object.Destroy(transform3.gameObject);
			Object.Destroy(transform4.gameObject);
			transform2.gameObject.SetActive(true);
			coinLabel = transform2.Find("reward_coin/Label_bonus").gameObject;
			coinBalloon = transform2.Find("reward_coin/icon_bonus").gameObject;
			transform2.Find("reward_coin/icon_event").gameObject.SetActive(false);
			transform2.Find("reward_coin/Label_event").gameObject.SetActive(false);
		}
		else
		{
			array = new string[3] { "reward_event", "reward_campaign", "reward_collaboration" };
			if ((bool)transform2)
			{
				Object.Destroy(transform2.gameObject);
			}
			if ((bool)transform3)
			{
				Object.Destroy(transform3.gameObject);
			}
			if ((bool)transform4)
			{
				Object.Destroy(transform4.gameObject);
			}
			transform.gameObject.SetActive(true);
			coinLabel = transform.Find("reward_coin/Label_bonus").gameObject;
			coinBalloon = transform.Find("reward_coin/icon_bonus").gameObject;
		}
		if (SaveData.Instance.getSystemData().getOptionData().isKorean())
		{
			balloonSpriteName = new string[4] { "event_bonus", "coin_bonus", "skill_bonus", "friend_bonus" };
		}
		else
		{
			balloonSpriteName = new string[4] { "event_bonus_en", "coin_bonus_en", "skill_bonus_en", "friend_bonus_en" };
		}
		UILabel[] componentsInChildren = GetComponentsInChildren<UILabel>(true);
		UILabel[] array2 = componentsInChildren;
		foreach (UILabel uILabel in array2)
		{
			bool flag = true;
			for (int j = 0; j < array.Length; j++)
			{
				if (uILabel.transform.parent.parent.name == array[j])
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				continue;
			}
			switch (uILabel.name)
			{
			case "Label_Target":
				labels_[4] = uILabel;
				break;
			case "Label_LevelBonus":
				switch (uILabel.transform.parent.name)
				{
				case "My_schore":
					labels_[10] = uILabel;
					break;
				case "remaining_Bonus":
					labels_[11] = uILabel;
					break;
				case "Level_Bonus":
					labels_[3] = uILabel;
					break;
				}
				break;
			case "Label":
				switch (uILabel.transform.parent.name)
				{
				case "My_schore":
					labels_[12] = uILabel;
					break;
				case "remaining_Bonus":
					labels_[13] = uILabel;
					break;
				case "Level_Bonus":
					labels_[14] = uILabel;
					break;
				}
				break;
			case "Label_maxcombo1":
				labels_[16] = uILabel;
				break;
			case "Label_maxcombo2":
				labels_[15] = uILabel;
				break;
			case "Label_Score":
				labels_[0] = uILabel;
				break;
			case "Label_LastScore":
				labels_[2] = uILabel;
				break;
			case "Label_Number":
				if (uILabel.transform.parent.name == "ComboBonus")
				{
					labels_[17] = uILabel;
				}
				else
				{
					labels_[1] = uILabel;
				}
				break;
			case "Label_Stage":
				labels_[20] = uILabel;
				break;
			case "Label_Reward":
				labels_[5] = uILabel;
				break;
			case "Label_Reward_jewel":
				labels_[6] = uILabel;
				break;
			case "Label_Exp":
				labels_[7] = uILabel;
				break;
			case "Label_bonus":
				labels_[8] = uILabel;
				break;
			case "Label_bonus_jewel":
				labels_[9] = uILabel;
				break;
			case "Label_event":
				labels_[18] = uILabel;
				break;
			case "Label_key":
				labels_[21] = uILabel;
				break;
			case "Label_item":
				labels_[22] = uILabel;
				break;
			case "Label_multiply":
				labels_[23] = uILabel;
				break;
			}
		}
	}

	protected void enableButton()
	{
		NGUIUtility.enable(buttons_, true);
	}

	protected void disableButton()
	{
		NGUIUtility.disable(buttons_, true);
	}

	protected void setup(int stageNo, int score, int coin, int exp, int _eventNo, bool bClear)
	{
		eventNo = _eventNo;
		StageDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
		eventStageInfo = component.getEventInfo(stageNo, _eventNo);
		int[] array = new int[3];
		int[] array2 = new int[3];
		if (_eventNo == 11)
		{
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = GameObject.Find("_PartManager/Part_Stage").GetComponent<Part_Stage>().rewards[i];
				array[i] = GameObject.Find("_PartManager/Part_Stage").GetComponent<Part_Stage>().rewardnum[i];
			}
		}
		isClear = bClear;
		if (Constant.ParkStage.isParkStage(stageNo))
		{
			init(false, false);
		}
		else
		{
			init(stageNo >= 10000, _eventNo == 11);
		}
		setStageNoText(stageNo, _eventNo);
		setScoreText(score);
		setCoinText(coin);
		setExpText(exp);
		int type = -1;
		int no = 0;
		if (_eventNo != 11 || eventStageInfo == null)
		{
			return;
		}
		for (int j = 0; j < array2.Length; j++)
		{
			if (Constant.Event.isRewardFreeItem(array2[j]) && bClear)
			{
				type = Constant.Event.convItemTypeNoToRewardfreeItem(array2[j]);
				no = array[j];
				break;
			}
		}
		setFreeItem(stageNo, type, no, isClear);
	}

	protected IEnumerator _show(int stageNo)
	{
		pressBtn_ = eBtn.None;
		dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		if (stageNo <= Constant.Event.BaseEventStageNo || stageNo / Constant.Event.BaseEventStageNo != 3)
		{
			dialogManager_.StartCoroutine(showScoreDialog(stageNo));
		}
		yield return StartCoroutine(dialogManager_.waitDialogAnimation(this));
		yield return StartCoroutine(dialogManager_.waitDialogAnimation(scoreDialog_));
	}

	protected IEnumerator showScoreDialog(int stageNo)
	{
		yield return dialogManager_.StartCoroutine(scoreDialog_.show(stageNo));
	}

	protected void setStageNoText(int stage, int _event)
	{
		if (!Constant.Event.isEventStage(stage))
		{
			labels_[1].text = (stage + 1).ToString("N0");
			return;
		}
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		EventStageInfo eventData = @object.GetComponent<StageDataTable>().getEventData();
		switch (_event)
		{
		case 1:
			labels_[20].text = MessageResource.Instance.getMessage(1491);
			break;
		case 2:
			labels_[20].text = MessageResource.Instance.getMessage(4100);
			break;
		case 11:
			labels_[20].text = MessageResource.Instance.getMessage(3701);
			break;
		}
		labels_[20].gameObject.transform.localScale = new Vector3(30f, 30f, 1f);
		labels_[20].gameObject.transform.localPosition = new Vector3(-20f, 6f, -1f);
		if (Constant.ParkStage.isParkStage(stage))
		{
			labels_[1].text = (stage % 100000).ToString("N0");
		}
		else
		{
			labels_[1].text = (stage - _event * 10000).ToString("N0");
		}
		labels_[1].gameObject.transform.localScale = new Vector3(32f, 32f, 1f);
		labels_[1].gameObject.transform.localPosition = new Vector3(110f, 6f, -1f);
	}

	protected void setScoreText(int score)
	{
		labels_[0].text = score.ToString("N0");
	}

	protected void setCoinText(int coin)
	{
		labels_[5].text = coin.ToString("N0");
	}

	protected void setJewelText(int jewel)
	{
		labels_[6].text = jewel.ToString("N0");
	}

	protected void setExpText(int exp)
	{
		labels_[7].text = exp.ToString("N0");
	}

	protected void setFreeItem(int stageNumber, int Type, int No, bool bClear)
	{
		UISprite component = base.transform.Find("window/reward_collaboration/item/itemIcon/Background").GetComponent<UISprite>();
		if (component != null)
		{
			component.gameObject.SetActive(true);
		}
		if (labels_[23] != null)
		{
			labels_[23].gameObject.SetActive(true);
		}
		if (GlobalData.Instance.getStageData(stageNumber).clearCount == 1)
		{
			if (bClear && Type > 0)
			{
				labels_[22].text = No.ToString("N0");
				component.spriteName = "item_" + Type.ToString("000") + "_00";
				component.MakePixelPerfect();
				Transform transform = base.transform.Find("window/reward_collaboration/item/itemIcon");
				transform.transform.localScale = new Vector3(0.45f, 0.45f, 1f);
			}
			if (No <= 0)
			{
				labels_[22].text = "0";
				if (component != null)
				{
					component.gameObject.SetActive(false);
				}
				if (labels_[23] != null)
				{
					labels_[23].gameObject.SetActive(false);
				}
			}
		}
		else
		{
			labels_[22].text = "0";
			if (component != null)
			{
				component.gameObject.SetActive(false);
			}
			if (labels_[23] != null)
			{
				labels_[23].gameObject.SetActive(false);
			}
		}
	}

	protected void setActiveItemIcon(bool active)
	{
		GameObject gameObject = base.transform.Find("window/reward_collaboration/item/itemIcon/Background").gameObject;
		if (gameObject != null)
		{
			gameObject.SetActive(active);
		}
	}

	protected void setPlusText(eLabel label, int value)
	{
		if (!(labels_[(int)label] == null))
		{
			MessageResource instance = MessageResource.Instance;
			string message = instance.getMessage(21);
			message = instance.castCtrlCode(message, 1, value.ToString("N0"));
			labels_[(int)label].text = message;
		}
	}

	protected void setBonusText(eLabel label, int value)
	{
		if (value == 0)
		{
			if (labels_[(int)label] != null)
			{
				labels_[(int)label].gameObject.SetActive(false);
				labels_[(int)label].transform.parent.Find("icon_bonus").gameObject.SetActive(false);
			}
		}
		else
		{
			setPlusText(label, value);
		}
	}

	protected void setCampaignText(int campaignValue, int bonusValue)
	{
		UILabel uILabel = labels_[18];
		if (uILabel == null)
		{
			return;
		}
		if (campaignValue == 0)
		{
			uILabel.gameObject.SetActive(false);
			uILabel.transform.parent.Find("icon_event").gameObject.SetActive(false);
			return;
		}
		UILabel uILabel2 = labels_[8];
		if (bonusValue == 0 && uILabel2 != null)
		{
			uILabel.transform.localPosition = uILabel2.transform.localPosition;
			Transform parent = uILabel.transform.parent;
			TweenPosition component = parent.Find("icon_event").GetComponent<TweenPosition>();
			TweenPosition component2 = parent.Find("icon_bonus").GetComponent<TweenPosition>();
			component.from = component2.from;
			component.to = component2.to;
		}
		setPlusText(eLabel.CampaignCoin, campaignValue);
	}

	protected void setText(eLabel label, int value)
	{
		labels_[(int)label].text = value.ToString("N0");
	}

	protected void setLabelText(eLabel label, int msgNum)
	{
		labels_[(int)label].text = MessageResource.Instance.getMessage(msgNum);
	}

	protected void setKeyText(int keyCount, int maxKeyCount)
	{
		labels_[21].text = keyCount.ToString("N0") + "/" + maxKeyCount.ToString("N0");
	}

	protected void setKeyTextEnable(bool enable)
	{
		labels_[21].transform.parent.gameObject.SetActive(enable);
	}

	private void OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Close_Button":
			pressBtn_ = eBtn.Close;
			Constant.SoundUtil.PlayCancelSE();
			_close();
			break;
		case "NextButton":
			pressBtn_ = eBtn.Next;
			Constant.SoundUtil.PlayDecideSE();
			_close();
			break;
		case "RetryButton":
			pressBtn_ = eBtn.Retry;
			Constant.SoundUtil.PlayDecideSE();
			_close();
			break;
		}
	}

	private void _close()
	{
		if (eventNo == 11)
		{
			setActiveItemIcon(false);
		}
		dialogManager_.StartCoroutine(dialogManager_.closeDialog(scoreDialog_));
		dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
	}

	public eBtn getClickBtn()
	{
		return pressBtn_;
	}
}
