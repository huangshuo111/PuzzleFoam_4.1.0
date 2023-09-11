using System.Collections;
using Bridge;
using UnityEngine;

public class DialogRankingResultClear : DialogResultBase
{
	private const float CountUpTime = 2f;

	private const float CountUpSETime = 0.05f;

	private Animation highScoreIcon_;

	private bool bCountUp_;

	private bool bEffect_ = true;

	private int star_;

	private string cauntUp_animHead = string.Empty;

	private int eventNumber;

	private bool eventResult;

	public override void OnCreate()
	{
		UILabel[] componentsInChildren = GetComponentsInChildren<UILabel>(true);
		base.OnCreate();
		UILabel[] componentsInChildren2 = GetComponentsInChildren<UILabel>(true);
		highScoreIcon_ = base.transform.Find("window/Stamp").GetComponent<Animation>();
		highScoreIcon_.gameObject.SetActive(false);
	}

	public IEnumerator show(int lv, int score, int highScore, Constant.Reward[] rewards, int exp, int bonusScore, int bonusCoin, int maxCombo, int comboBonusCoin, float bonusRate)
	{
		Input.enable = false;
		disableButton();
		bEffect_ = true;
		Transform reward = base.transform.Find("window/reward");
		reward.gameObject.SetActive(true);
		Transform getCoinTransform = null;
		Transform totalCoinTransform = null;
		Transform getExpTransform = null;
		Transform totalExpTransform = null;
		totalExpTransform = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).transform.Find("all").transform.Find("03_exp").transform;
		totalCoinTransform = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).transform.Find("all").transform.Find("01_coin").transform.Find("icon_coin").transform;
		HeartMenu Heart2 = null;
		Heart2 = base.transform.Find("all").transform.Find("04_heart").GetComponent<HeartMenu>();
		if ((bool)Heart2)
		{
			Heart2.init();
		}
		UILabel[] labels = GetComponentsInChildren<UILabel>(true);
		UILabel[] array = labels;
		foreach (UILabel label in array)
		{
			switch (label.name)
			{
			case "Label_Target":
				labels_[4] = label;
				break;
			case "Label_LevelBonusRate":
				labels_[24] = label;
				break;
			case "Label_LevelBonus":
				switch (label.transform.parent.name)
				{
				case "My_score":
					labels_[10] = label;
					break;
				case "remaining_Bonus":
					labels_[11] = label;
					break;
				case "Level_Bonus":
					labels_[3] = label;
					break;
				}
				break;
			case "Label":
				switch (label.transform.parent.name)
				{
				case "My_score":
					labels_[12] = label;
					break;
				case "remaining_Bonus":
					labels_[13] = label;
					break;
				case "Level_Bonus":
					labels_[14] = label;
					break;
				}
				break;
			case "Label_maxcombo1":
				labels_[16] = label;
				break;
			case "Label_maxcombo2":
				labels_[15] = label;
				break;
			case "Label_Score":
				labels_[0] = label;
				break;
			case "Label_LastScore":
				labels_[2] = label;
				break;
			case "Label_Number":
				if (label.transform.parent.name == "ComboBonus")
				{
					labels_[17] = label;
				}
				else
				{
					labels_[1] = label;
				}
				break;
			case "Label_Stage":
				labels_[20] = label;
				break;
			case "Label_Reward":
				getCoinTransform = label.transform.parent.transform;
				labels_[5] = label;
				break;
			case "Label_Reward_jewel":
				labels_[6] = label;
				break;
			case "Label_Exp":
				getExpTransform = label.transform.parent.transform;
				labels_[7] = label;
				break;
			case "Label_event":
				labels_[18] = label;
				break;
			}
		}
		setScoreText(score);
		setCoinText(bonusCoin);
		setExpText(exp);
		GameObject dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		RewardDataTable rewardTbl = dataTbl.GetComponent<RewardDataTable>();
		float rate = rewardTbl.getRankingBonus(lv - 1);
		setHighScoreText(highScore);
		setLabelText(eLabel.MyScoreLabel, 2553);
		setLabelText(eLabel.LevelBonusLabel, 2556);
		setLabelText(eLabel.MaxComboLabel, 2557);
		setText(eLabel.MyScore, score - bonusScore);
		setText(eLabel.Bonus, bonusScore);
		labels_[17].transform.parent.gameObject.SetActive(maxCombo >= 2);
		labels_[17].text = comboBonusCoin.ToString("N0");
		labels_[15].text = ((maxCombo < 2) ? "0" : maxCombo.ToString("N0"));
		dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		Input.enable = true;
		yield return StartCoroutine(scoreCountUp(score));
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
		enableButton();
		if (bEffect_)
		{
			CoinEffect expEff = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.ExpEffect).GetComponent<CoinEffect>();
			expEff.gameObject.SetActive(true);
			yield return StartCoroutine(expEff.play(getExpTransform.position, totalExpTransform.position));
			expEff.gameObject.SetActive(false);
		}
		if (bEffect_ && bonusCoin > 0)
		{
			CoinEffect coinEff = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.CoinEffect).GetComponent<CoinEffect>();
			coinEff.gameObject.SetActive(true);
			yield return StartCoroutine(coinEff.play(getCoinTransform.position, totalCoinTransform.position));
			coinEff.gameObject.SetActive(false);
		}
		PlayerData.setCoin(PlayerData.getCoin() + bonusCoin);
		MainMenu mainMenu_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		mainMenu_.update();
		Input.enable = true;
	}

	private IEnumerator countUp(int value)
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
			yield return 0;
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
		bEffect_ = false;
	}

	private IEnumerator scoreCountUp(int score)
	{
		bCountUp_ = true;
		StartCoroutine(skipMonitorRoutine());
		yield return StartCoroutine(countUp(score));
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
}
