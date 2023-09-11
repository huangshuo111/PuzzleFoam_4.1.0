using System.Collections;
using Bridge;
using Network;
using TapjoyUnity;
using UnityEngine;

public class DialogResultClear : DialogResultBase
{
	private const float CountUpTime = 2f;

	private const float CountUpSETime = 0.05f;

	private const int exclusiveMaxComboHeight = 20;

	private const int exclusiveComboBonusHeight = 30;

	private const int defaultMaxComboHeight = 0;

	private const int defaultComboBonusHeight = 10;

	private const int parkMaxComboHeight = 30;

	private const int parkComboBonusHeight = 40;

	private GameObject[] stars_ = new GameObject[Constant.StarMax];

	private GameObject[] starEffcts_ = new GameObject[Constant.StarMax];

	private GameObject starRoot_;

	private GameObject notesRoot_;

	private Animation highScoreIcon_;

	private bool bCountUp_;

	private int star_;

	private string cauntUp_animHead = string.Empty;

	private int eventNumber;

	private bool eventResult;

	public override void OnCreate()
	{
		base.OnCreate();
		bool flag = false;
		Transform transform = base.transform.Find("window/Stars");
		if (transform == null)
		{
			flag = true;
			transform = base.transform.Find("window/flowers");
		}
		starRoot_ = transform.gameObject;
		Transform transform2 = base.transform.Find("window/Notes");
		if ((bool)transform2)
		{
			notesRoot_ = transform2.gameObject;
		}
		Transform transform3 = null;
		transform3 = ((!flag) ? base.transform.Find("window/Star_eff") : base.transform.Find("window/Flower_eff"));
		GameObject gameObject = transform3.gameObject;
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
			if (flag)
			{
				switch (gameObject2.name)
				{
				case "Flower_00_eff":
					starEffcts_[0] = gameObject2;
					break;
				case "Flower_01_eff":
					starEffcts_[1] = gameObject2;
					break;
				case "Flower_02_eff":
					starEffcts_[2] = gameObject2;
					break;
				}
			}
			else
			{
				switch (gameObject2.name)
				{
				case "Star_00_eff":
					starEffcts_[0] = gameObject2;
					break;
				case "Star_01_eff":
					starEffcts_[1] = gameObject2;
					break;
				case "Star_02_eff":
					starEffcts_[2] = gameObject2;
					break;
				}
			}
			gameObject2.gameObject.SetActive(false);
		}
		highScoreIcon_ = base.transform.Find("window/Stamp").GetComponent<Animation>();
		highScoreIcon_.gameObject.SetActive(false);
	}

	public IEnumerator show(StageInfo.CommonInfo stageInfo, int lv, int score, int stageNo, int highScore, Constant.Reward[] rewards, int exp, int star, int bonusScore, int bonusCoin, int bonusJewel, int remainingBonus, int maxCombo, int comboBonusCoin, int campaignCoin, bool isGetJewel, bool isEventStage, bool isGetCollaboReward, int eventNo, int eventCoin, bool isSkillCoinUp, int skillCoin)
	{
		Input.enable = false;
		disableButton();
		eventResult = isEventStage;
		if (eventResult)
		{
			eventNumber = eventNo;
			switch (eventNumber)
			{
			case 2:
				starRoot_.SetActive(false);
				notesRoot_.SetActive(false);
				base.transform.Find("window/chara/bg_challenge").gameObject.SetActive(true);
				base.transform.Find("window/chara/bg_collaboration").gameObject.SetActive(false);
				break;
			case 11:
				starRoot_.SetActive(false);
				notesRoot_.SetActive(false);
				base.transform.Find("window/chara/bg_challenge").gameObject.SetActive(false);
				base.transform.Find("window/chara/bg_collaboration").gameObject.SetActive(true);
				break;
			default:
				starRoot_.SetActive(false);
				notesRoot_.SetActive(true);
				base.transform.Find("window/chara/bg_challenge").gameObject.SetActive(false);
				base.transform.Find("window/chara/bg_collaboration").gameObject.SetActive(false);
				break;
			}
			cauntUp_animHead = "Note";
			for (int i = 0; i < notesRoot_.transform.childCount; i++)
			{
				GameObject child2 = notesRoot_.transform.GetChild(i).gameObject;
				switch (child2.name)
				{
				case "Note_00":
					stars_[0] = child2;
					break;
				case "Note_01":
					stars_[1] = child2;
					break;
				case "Note_02":
					stars_[2] = child2;
					break;
				}
			}
		}
		else
		{
			starRoot_.SetActive(true);
			if ((bool)notesRoot_)
			{
				notesRoot_.SetActive(false);
			}
			base.transform.Find("window/chara/bg_challenge").gameObject.SetActive(false);
			base.transform.Find("window/chara/bg_collaboration").gameObject.SetActive(false);
			cauntUp_animHead = "Star";
			for (int j = 0; j < starRoot_.transform.childCount; j++)
			{
				GameObject child = starRoot_.transform.GetChild(j).gameObject;
				if (Constant.ParkStage.isParkStage(stageNo))
				{
					cauntUp_animHead = "Flower";
					switch (child.name)
					{
					case "flower_00":
						stars_[0] = child;
						break;
					case "flower_01":
						stars_[1] = child;
						break;
					case "flower_02":
						stars_[2] = child;
						break;
					}
				}
				else
				{
					switch (child.name)
					{
					case "Star_00":
						stars_[0] = child;
						break;
					case "Star_01":
						stars_[1] = child;
						break;
					case "Star_02":
						stars_[2] = child;
						break;
					}
				}
			}
		}
		setup(stageNo, 0, rewards[1].Num - bonusCoin - campaignCoin - skillCoin, exp, eventNo, true);
		GameObject dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		RewardDataTable rewardTbl = dataTbl.GetComponent<RewardDataTable>();
		int rate = rewardTbl.getBonus(lv - 1);
		setJewelText(rewards[2].Num - bonusJewel);
		int jewel_num = 0;
		int ticket_num = 0;
		if (isGetCollaboReward && eventNumber == 11)
		{
			StageDataTable stageData2 = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
			EventStageInfo.Info eventStageInfo2 = stageData2.getEventInfo(stageNo, eventNo);
			int[] Rewards = new int[GameObject.Find("_PartManager/Part_Stage").GetComponent<Part_Stage>().rewards.Length];
			int[] RewardNums = new int[GameObject.Find("_PartManager/Part_Stage").GetComponent<Part_Stage>().rewardnum.Length];
			for (int k = 0; k < Rewards.Length; k++)
			{
				Rewards[k] = GameObject.Find("_PartManager/Part_Stage").GetComponent<Part_Stage>().rewards[k];
				RewardNums[k] = GameObject.Find("_PartManager/Part_Stage").GetComponent<Part_Stage>().rewardnum[k];
			}
			if (eventStageInfo2 != null)
			{
				for (int m = 0; m < Rewards.Length; m++)
				{
					if (Rewards[m] == 2)
					{
						jewel_num = RewardNums[m];
					}
					if (Rewards[m] == 30000)
					{
						ticket_num = RewardNums[m];
					}
				}
				setJewelText(rewards[2].Num - bonusJewel + jewel_num);
			}
		}
		setTargetText(stageInfo);
		setHighScoreText(highScore);
		setLabelText(eLabel.MyScoreLabel, 2553);
		if (stageInfo.Time <= 0)
		{
			setLabelText(eLabel.RemainingBonusLabel, 2554);
		}
		else
		{
			setLabelText(eLabel.RemainingBonusLabel, 2555);
		}
		setLabelText(eLabel.LevelBonusLabel, 2556);
		setLabelText(eLabel.MaxComboLabel, 2557);
		setText(eLabel.MyScore, score - remainingBonus - bonusScore);
		setText(eLabel.RemainingBonus, remainingBonus);
		setText(eLabel.Bonus, bonusScore);
		labels_[17].transform.parent.gameObject.SetActive(maxCombo >= 2);
		labels_[17].text = comboBonusCoin.ToString("N0");
		labels_[15].text = ((maxCombo < 2) ? "0" : maxCombo.ToString("N0"));
		if (Constant.ParkStage.isParkStage(stageNo))
		{
			labels_[15].transform.localPosition = new Vector3(labels_[15].transform.localPosition.x, 30f, labels_[15].transform.localPosition.z);
			labels_[16].transform.localPosition = new Vector3(labels_[16].transform.localPosition.x, 30f, labels_[16].transform.localPosition.z);
			Transform comboBonus = labels_[17].transform.parent;
			TweenPosition tween = comboBonus.GetComponent<TweenPosition>();
			tween.from.y = 40f;
			tween.to.y = 42f;
		}
		else if (eventNumber == 11)
		{
			labels_[15].transform.localPosition = new Vector3(labels_[15].transform.localPosition.x, 20f, labels_[15].transform.localPosition.z);
			labels_[16].transform.localPosition = new Vector3(labels_[16].transform.localPosition.x, 20f, labels_[16].transform.localPosition.z);
			Transform comboBonus2 = labels_[17].transform.parent;
			TweenPosition tween2 = comboBonus2.GetComponent<TweenPosition>();
			tween2.from.y = 30f;
			tween2.to.y = 32f;
		}
		else
		{
			labels_[15].transform.localPosition = new Vector3(labels_[15].transform.localPosition.x, 0f, labels_[15].transform.localPosition.z);
			labels_[16].transform.localPosition = new Vector3(labels_[16].transform.localPosition.x, 0f, labels_[16].transform.localPosition.z);
			Transform comboBonus3 = labels_[17].transform.parent;
			TweenPosition tween3 = comboBonus3.GetComponent<TweenPosition>();
			tween3.from.y = 10f;
			tween3.to.y = 12f;
		}
		setBonusText(eLabel.BonusJewel, bonusJewel);
		bool isTotalZero = ((bonusCoin + campaignCoin + skillCoin + eventCoin == 0) ? true : false);
		int[] showCoinLoopList = new int[4] { campaignCoin, eventCoin, skillCoin, bonusCoin };
		coinLabel.SetActive(false);
		coinBalloon.SetActive(false);
		if (!eventResult)
		{
			KeyBubbleData keyData = GlobalData.Instance.getKeyBubbleData();
			if (Bridge.StageData.isClear(keyData.invalidStage) && !Constant.ParkStage.isParkStage(stageNo))
			{
				setKeyTextEnable(true);
				setKeyText(keyData.keyBubbleCount, keyData.keyBubbleMax);
			}
			else
			{
				setKeyTextEnable(false);
			}
		}
		dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		for (int l = 0; l < stars_.Length; l++)
		{
			stars_[l].gameObject.SetActive(false);
		}
		star_ = star;
		Input.enable = true;
		if (!isTotalZero)
		{
			coinLabel.SetActive(true);
			coinBalloon.SetActive(true);
			StartCoroutine(showCoinLoop(showCoinLoopList));
		}
		yield return StartCoroutine(scoreCountUp(score, stageInfo));
		startConfettiEff();
		Input.enable = false;
		if (score > highScore && highScore > 0)
		{
			Sound.Instance.playSe(Sound.eSe.SE_240_hanko);
			highScoreIcon_.gameObject.SetActive(true);
			highScoreIcon_.Play();
		}
		while (highScoreIcon_.isPlaying)
		{
			yield return 0;
		}
		int reward_jewel_num = rewards[2].Num;
		if (isGetJewel)
		{
			MessageResource msgRes = MessageResource.Instance;
			string msg2 = msgRes.getMessage(1488);
			DialogClearBonus clearBonusDialog = dialogManager_.getDialog(DialogManager.eDialog.ClearBonus) as DialogClearBonus;
			clearBonusDialog.setup(msg2, null, null, true, Constant.eMoney.Jewel, reward_jewel_num, eventNo);
			yield return StartCoroutine(clearBonusDialog.open());
			Tapjoy.TrackEvent("Money", "Income Jewel", "Area Clear Bonus", null, reward_jewel_num);
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Jewel", "Area Clear Bonus", reward_jewel_num);
		}
		if (isGetCollaboReward)
		{
			StageDataTable stageData = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
			EventStageInfo.Info eventStageInfo = stageData.getEventInfo(stageNo, eventNo);
			DialogClearBonusCollabo clearBonusCollaboDialog = dialogManager_.getDialog(DialogManager.eDialog.ClearBonusCollabo) as DialogClearBonusCollabo;
			string msg = ((stageNo % (10000 * eventNo) <= 11) ? MessageResource.Instance.getMessage(3715) : MessageResource.Instance.getMessage(3708));
			if (jewel_num > 0)
			{
				clearBonusCollaboDialog.setup(msg, null, null, true, Constant.eMoney.Jewel, jewel_num, eventNo, stageNo);
			}
			else if (ticket_num > 0)
			{
				clearBonusCollaboDialog.setup(msg, null, null, true, Constant.eMoney.GachaTicket, ticket_num, eventNo, stageNo);
			}
			clearBonusCollaboDialog.sysLabelEnable(false);
			yield return StartCoroutine(clearBonusCollaboDialog.open());
		}
		enableButton();
		yield return StartCoroutine(showScoreDialog(stageNo));
		Input.enable = true;
	}

	private IEnumerator countUp(int value, StageInfo.CommonInfo info)
	{
		float max = value;
		float startTime = Time.time;
		float now = 0f;
		if (now == max)
		{
			setScoreText((int)max);
			yield break;
		}
		float seTime_ = Time.time;
		int index = 0;
		bool bStarEffectPlay = eventNumber != 2 && eventNumber != 11;
		while (bCountUp_ && now < max)
		{
			if (Time.time - seTime_ >= 0.05f)
			{
				Sound.Instance.playSe(Sound.eSe.SE_241_score);
				seTime_ = Time.time;
			}
			float rate = (Time.time - startTime) / 2f;
			if (rate > 1f)
			{
				rate = 1f;
			}
			now = Mathf.Lerp(0f, max, rate);
			setScoreText((int)now);
			if (bStarEffectPlay && info.StarScores.Length > index && info.StarScores[index] <= (int)now)
			{
				Sound.Instance.playSe(Sound.eSe.SE_360_resultstar);
				stars_[index].gameObject.SetActive(true);
				starEffcts_[index].SetActive(true);
				string animeName2 = "Get" + cauntUp_animHead + (index + 1) + "_Result_anm";
				stars_[index].GetComponent<Animation>().Play(animeName2);
				index++;
			}
			yield return 0;
		}
		if (!bCountUp_ && bStarEffectPlay)
		{
			for (int i = index; i < star_; i++)
			{
				stars_[i].gameObject.SetActive(true);
				starEffcts_[i].SetActive(true);
				string animeName = "Get" + cauntUp_animHead + (i + 1) + "_Result_anm";
				stars_[i].GetComponent<Animation>().Play(animeName);
			}
		}
		setScoreText((int)max);
	}

	private IEnumerator skipMonitorRoutine()
	{
		while (!Input.GetMouseButtonDown(0))
		{
			if (!bCountUp_)
			{
				yield break;
			}
			yield return 0;
		}
		while (!Input.GetMouseButtonUp(0))
		{
			if (!bCountUp_)
			{
				yield break;
			}
			yield return 0;
		}
		bCountUp_ = false;
	}

	private IEnumerator showCoinLoop(int[] coinList)
	{
		float timer = 1.5f;
		int loopCount2 = 0;
		UILabel label = coinLabel.GetComponent<UILabel>();
		UISprite sprite = coinBalloon.GetComponent<UISprite>();
		while (true)
		{
			timer += Time.deltaTime;
			if (timer >= 1.5f)
			{
				loopCount2++;
				loopCount2 %= coinList.Length;
				if (coinList[loopCount2] == 0)
				{
					continue;
				}
				timer = 0f;
				label.text = "+" + coinList[loopCount2];
				sprite.spriteName = balloonSpriteName[loopCount2];
			}
			yield return null;
		}
	}

	private IEnumerator scoreCountUp(int score, StageInfo.CommonInfo info)
	{
		bCountUp_ = true;
		StartCoroutine(skipMonitorRoutine());
		yield return StartCoroutine(countUp(score, info));
		bCountUp_ = false;
	}

	private void setHighScoreText(int highScore)
	{
		if (highScore > 0)
		{
			labels_[2].text = highScore.ToString("N0");
			return;
		}
		labels_[2].transform.parent.gameObject.SetActive(false);
		Vector3 localPosition = labels_[0].transform.localPosition;
		localPosition.y -= 25f;
		labels_[0].transform.localPosition = localPosition;
	}

	private void setTargetText(StageInfo.CommonInfo info)
	{
		MessageResource instance = MessageResource.Instance;
		if (labels_[4] != null)
		{
			labels_[4].text = Constant.MessageUtil.getTargetMsg(info, instance, Constant.MessageUtil.eTargetType.Clear);
		}
	}

	public void SetMinilenNum(int num)
	{
		Transform transform = base.transform.Find("window/reward/minilen/Label_Minilen");
		if ((bool)transform)
		{
			transform.GetComponent<UILabel>().text = num.ToString();
		}
		transform = base.transform.Find("window/reward_campaign/minilen/Label_Minilen");
		if ((bool)transform)
		{
			transform.GetComponent<UILabel>().text = num.ToString();
		}
	}
}
