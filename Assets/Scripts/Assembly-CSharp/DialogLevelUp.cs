using System.Collections;
using UnityEngine;

public class DialogLevelUp : DialogBase
{
	private class UIReward
	{
		private enum eLabel
		{
			BonusNum = 0,
			RewardNum = 1,
			RewardText = 2,
			Max = 3
		}

		private enum eIcon
		{
			Coin = 0,
			Heart = 1,
			Jewel = 2,
			Max = 3
		}

		public UILabel[] Labels = new UILabel[3];

		public GameObject[] Icons = new GameObject[3];

		public GameObject Root;

		public void init(GameObject rewardRoot)
		{
			Transform[] componentsInChildren = rewardRoot.GetComponentsInChildren<Transform>(true);
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				switch (transform.name)
				{
				case "bonus_label":
					Labels[0] = transform.GetComponent<UILabel>();
					break;
				case "reward_num_Label":
					Labels[1] = transform.GetComponent<UILabel>();
					break;
				case "reward_bonus_Label":
					Labels[2] = transform.GetComponent<UILabel>();
					break;
				case "icon_coin":
					Icons[0] = transform.gameObject;
					break;
				case "icon_jewel":
					Icons[2] = transform.gameObject;
					break;
				case "icon_heart":
					Icons[1] = transform.gameObject;
					break;
				}
			}
			Root = rewardRoot;
		}

		public void setup(Constant.Reward reward)
		{
			GameObject[] icons = Icons;
			foreach (GameObject gameObject in icons)
			{
				gameObject.SetActive(false);
			}
			MessageResource instance = MessageResource.Instance;
			string src = string.Empty;
			switch (reward.RewardType)
			{
			case Constant.eMoney.Coin:
				src = instance.getMessage(31);
				Labels[2].text = instance.getMessage(300);
				Icons[0].SetActive(true);
				break;
			case Constant.eMoney.Jewel:
				src = instance.getMessage(28);
				Labels[2].text = instance.getMessage(301);
				Icons[2].SetActive(true);
				break;
			case Constant.eMoney.Heart:
				if (reward.Num == 0)
				{
					Labels[2].text = instance.getMessage(1478);
					break;
				}
				src = instance.getMessage(28);
				Labels[2].text = instance.getMessage(302);
				Icons[1].SetActive(true);
				break;
			}
			if (reward.Num > 0)
			{
				Labels[1].gameObject.SetActive(true);
				src = instance.castCtrlCode(src, 1, reward.Num.ToString("N0"));
				Labels[1].text = src;
			}
			else
			{
				Labels[1].gameObject.SetActive(false);
			}
		}
	}

	private int lv_;

	private LevelUpSprite levelSprite_;

	private UILabel levelBonusLabel_;

	private UIButton sendButton_;

	private UIReward[] uiRewards_ = new UIReward[2];

	private DialogSendBase.OnSendSuccess cb_;

	public override void OnCreate()
	{
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			switch (transform.name)
			{
			case "Level":
				levelSprite_ = transform.GetComponent<LevelUpSprite>();
				break;
			case "SendButtun":
				sendButton_ = transform.GetComponent<UIButton>();
				break;
			case "Reward_Bonus01":
				uiRewards_[0] = new UIReward();
				uiRewards_[0].init(transform.gameObject);
				break;
			case "Reward_Bonus02":
				uiRewards_[1] = new UIReward();
				uiRewards_[1].init(transform.gameObject);
				break;
			case "bonus_label":
				levelBonusLabel_ = transform.GetComponent<UILabel>();
				break;
			}
		}
		cb_ = OnSendSuccess;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "CloseButtun":
			Constant.SoundUtil.PlayDecideSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "SendButtun":
		{
			Constant.SoundUtil.PlayDecideSE();
			DialogSendLevel dialog = dialogManager_.getDialog(DialogManager.eDialog.SendLevel) as DialogSendLevel;
			dialog.setup(lv_);
			dialog.setCB(cb_);
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
			break;
		}
		}
	}

	public IEnumerator show(int lv, Constant.Reward[] rewards)
	{
		Input.enable = false;
		yield return dialogManager_.StartCoroutine(ActionReward.getActionRewardInfo(ActionReward.eType.LevelUp));
		ActionReward.setupButton(sendButton_.transform, ActionReward.eType.LevelUp);
		Sound.Instance.playSe(Sound.eSe.SE_239_tassei);
		startConfettiEff();
		lv_ = lv;
		GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		RewardDataTable rewardTbl = dataTable.GetComponent<RewardDataTable>();
		int bonus = rewardTbl.getBonus(lv - 1);
		levelSprite_.setup(lv);
		for (int j = 0; j < uiRewards_.Length; j++)
		{
			uiRewards_[j].Root.SetActive(false);
		}
		for (int i = uiRewards_.Length - 1; i >= 0; i--)
		{
			if (i < rewards.Length)
			{
				uiRewards_[i].Root.SetActive(true);
				uiRewards_[i].setup(rewards[i]);
			}
		}
		MessageResource msgRes = MessageResource.Instance;
		string msg2 = msgRes.getMessage(30);
		msg2 = msgRes.castCtrlCode(msg2, 1, bonus.ToString());
		levelBonusLabel_.text = msg2;
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		Input.enable = true;
	}

	private void OnSendSuccess()
	{
		sendButton_.transform.Find("Label_reward").GetComponent<UILabel>().color = Color.gray;
		sendButton_.transform.Find("icon_coin").GetComponent<UISprite>().color = Color.gray;
		NGUIUtility.disable(sendButton_, false);
	}
}
