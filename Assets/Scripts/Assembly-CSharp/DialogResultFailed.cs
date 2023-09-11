using System.Collections;
using UnityEngine;

public class DialogResultFailed : DialogResultBase
{
	private ClearCheckBox checkBox_;

	public override void OnCreate()
	{
		base.OnCreate();
		checkBox_ = base.transform.Find("window/Checkboxs").GetComponent<ClearCheckBox>();
	}

	public IEnumerator show(StageInfo.CommonInfo stageInfo, int clearState, int score, int stageNo, int coin, int exp, int bonusCoin, int bonusJewel, int campaignCoin, bool isEventStage, int eventNo, int eventCoin, bool isSkillCoinUp, int skillCoin)
	{
		Input.enable = false;
		setup(stageNo, score, coin - bonusCoin - campaignCoin - skillCoin, exp, eventNo, false);
		if (Constant.ParkStage.isParkStage(stageNo))
		{
			base.transform.Find("window/flowers").gameObject.SetActive(true);
			base.transform.Find("window/Stars").gameObject.SetActive(false);
			base.transform.Find("window/Notes").gameObject.SetActive(false);
			base.transform.Find("window/chara/bg_normal").gameObject.SetActive(true);
			base.transform.Find("window/chara/bg_challenge").gameObject.SetActive(false);
			base.transform.Find("window/chara/bg_collaboration").gameObject.SetActive(false);
			labels_[20].text = MessageResource.Instance.getMessage(9102);
		}
		else if (isEventStage)
		{
			switch (eventNo)
			{
			case 1:
				base.transform.Find("window/Stars").gameObject.SetActive(false);
				base.transform.Find("window/Notes").gameObject.SetActive(true);
				base.transform.Find("window/chara/bg_normal").gameObject.SetActive(true);
				base.transform.Find("window/chara/bg_challenge").gameObject.SetActive(false);
				base.transform.Find("window/chara/bg_collaboration").gameObject.SetActive(false);
				labels_[20].text = MessageResource.Instance.getMessage(1491);
				break;
			case 2:
				base.transform.Find("window/Stars").gameObject.SetActive(false);
				base.transform.Find("window/Notes").gameObject.SetActive(false);
				base.transform.Find("window/chara/bg_normal").gameObject.SetActive(false);
				base.transform.Find("window/chara/bg_challenge").gameObject.SetActive(true);
				base.transform.Find("window/chara/bg_collaboration").gameObject.SetActive(false);
				labels_[20].text = MessageResource.Instance.getMessage(4100);
				break;
			case 11:
				base.transform.Find("window/Stars").gameObject.SetActive(false);
				base.transform.Find("window/Notes").gameObject.SetActive(false);
				base.transform.Find("window/chara/bg_normal").gameObject.SetActive(true);
				base.transform.Find("window/chara/bg_challenge").gameObject.SetActive(false);
				base.transform.Find("window/chara/bg_collaboration").gameObject.SetActive(true);
				labels_[20].text = MessageResource.Instance.getMessage(3701);
				break;
			}
		}
		else
		{
			base.transform.Find("window/Stars").gameObject.SetActive(true);
			base.transform.Find("window/Notes").gameObject.SetActive(false);
			base.transform.Find("window/chara/bg_normal").gameObject.SetActive(true);
			base.transform.Find("window/chara/bg_challenge").gameObject.SetActive(false);
			base.transform.Find("window/chara/bg_collaboration").gameObject.SetActive(false);
			labels_[20].text = MessageResource.Instance.getMessage(202);
		}
		setTargetText(stageInfo);
		checkBox_.setup(stageInfo, clearState, labels_[4]);
		setJewelText(0);
		setBonusText(eLabel.BonusJewel, bonusJewel);
		bool isTotalZero = ((bonusCoin + campaignCoin + skillCoin + eventCoin == 0) ? true : false);
		int[] showCoinLoopList = new int[4] { campaignCoin, eventCoin, skillCoin, bonusCoin };
		coinLabel.SetActive(false);
		coinBalloon.SetActive(false);
		yield return dialogManager_.StartCoroutine(_show(stageNo));
		if (!isTotalZero)
		{
			coinLabel.SetActive(true);
			coinBalloon.SetActive(true);
			StartCoroutine(showCoinLoop(showCoinLoopList));
		}
		Input.enable = true;
	}

	private void setTargetText(StageInfo.CommonInfo info)
	{
		MessageResource instance = MessageResource.Instance;
		labels_[4].text = Constant.MessageUtil.getTargetMsg(info, instance, Constant.MessageUtil.eTargetType.Continue);
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
}
