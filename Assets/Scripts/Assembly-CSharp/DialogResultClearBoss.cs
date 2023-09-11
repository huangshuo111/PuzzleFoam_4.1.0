using System.Collections;
using TapjoyUnity;
using UnityEngine;

public class DialogResultClearBoss : DialogBase
{
	public BossBase bossBase_;

	private Transform confBtn;

	private UILabel coinLabel;

	private UILabel heartLabel;

	private UILabel jewelLabel;

	private UILabel targetLabel;

	private UILabel levelLabel;

	private UISprite[] item = new UISprite[5];

	private UISlicedSprite plateSprite;

	public override void OnCreate()
	{
		base.OnCreate();
	}

	public void Awake()
	{
		confBtn = base.transform.Find("window/ConfirmButton");
		coinLabel = base.transform.Find("window/rewards/coin_Label01").GetComponent<UILabel>();
		heartLabel = base.transform.Find("window/rewards/heart_Label02").GetComponent<UILabel>();
		for (int i = 0; i < item.Length; i++)
		{
			item[i] = base.transform.Find("window/item/item" + i.ToString("00") + "/Background").GetComponent<UISprite>();
		}
		jewelLabel = base.transform.Find("window/rewards/jewel_Label04").GetComponent<UILabel>();
		levelLabel = base.transform.Find("window/plate/BossLevel_label").GetComponent<UILabel>();
		plateSprite = base.transform.Find("window/plate/palte_boss").GetComponent<UISlicedSprite>();
		targetLabel = base.transform.Find("window/txtArea").GetComponent<UILabel>();
	}

	public IEnumerator showBossStage(BossStageInfo.LevelInfo lvlInfo, int type, int level)
	{
		MessageResource msgRes = MessageResource.Instance;
		plateSprite.spriteName = "Bos_setup_plate_boss_" + type.ToString("00");
		plateSprite.MakePixelPerfect();
		string targetStr = MessageResource.Instance.getMessage(3722);
		targetLabel.text = targetStr;
		string levelStr = MessageResource.Instance.getMessage(8005);
		levelStr = msgRes.castCtrlCode(levelStr, 1, level.ToString());
		levelLabel.text = levelStr;
		coinLabel.text = getRewardNum(lvlInfo, 1).ToString("N0");
		heartLabel.text = getRewardNum(lvlInfo, 3).ToString("N0");
		jewelLabel.text = getRewardNum(lvlInfo, 2).ToString("N0");
		Tapjoy.TrackEvent("Money", "Income Coin", "Boss Stage Clear", null, getRewardNum(lvlInfo, 1));
		Tapjoy.TrackEvent("Money", "Income Jewel", "Boss Stage Clear", null, getRewardNum(lvlInfo, 2));
		GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Boss Stage Clear", getRewardNum(lvlInfo, 1));
		GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Jewel", "Boss Stage Clear", getRewardNum(lvlInfo, 2));
		UISprite[] array = item;
		foreach (UISprite spr in array)
		{
			spr.transform.parent.gameObject.SetActive(false);
		}
		string name = "item_" + (lvlInfo.Rewards[3] % 1000).ToString("000") + "_00";
		int rewardNum = getRewardNum(lvlInfo, 0);
		if (rewardNum > 5)
		{
			rewardNum = 5;
		}
		for (int i = 0; i < rewardNum; i++)
		{
			item[i].spriteName = name;
			item[i].transform.parent.gameObject.SetActive(true);
			item[i].MakePixelPerfect();
			yield return null;
		}
		startConfettiEff();
		Sound.Instance.playSe(Sound.eSe.SE_108_Yay);
		Sound.Instance.playSe(Sound.eSe.SE_113_Clap);
		UIButtonMessage mes = confBtn.GetComponent<UIButtonMessage>();
		mes.target = base.gameObject;
		mes.functionName = "Close";
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
	}

	private int getRewardNum(BossStageInfo.LevelInfo levelInfo, int type)
	{
		if (type == 0)
		{
			return levelInfo.RewardNums[3];
		}
		for (int i = 0; i < levelInfo.Rewards.Length; i++)
		{
			if (levelInfo.Rewards[i] == type)
			{
				return levelInfo.RewardNums[i];
			}
		}
		return 0;
	}

	private void Close()
	{
		stopConfettiEff();
		Constant.SoundUtil.PlayDecideSE();
		dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
	}
}
