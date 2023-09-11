using System.Collections;
using UnityEngine;

public class DialogChangeRanking : DialogBase
{
	private enum eUser
	{
		Me = 0,
		Rival = 1,
		Max = 2
	}

	private enum eBtn
	{
		Close = 0,
		Send = 1,
		Max = 2
	}

	private class UserUI
	{
		public PlayerIcon Icon;

		public UILabel NameLabel;

		public UILabel ScoreLabel;

		public UILabel RankLabel;

		public DummyFriendIcon DummyIcon;

		public TweenPosition Tween;

		public void setup(GameObject userRoot)
		{
			Tween = userRoot.GetComponent<TweenPosition>();
			for (int i = 0; i < userRoot.transform.childCount; i++)
			{
				GameObject gameObject = userRoot.transform.GetChild(i).gameObject;
				switch (gameObject.name)
				{
				case "Name":
					NameLabel = gameObject.GetComponent<UILabel>();
					break;
				case "rank":
					RankLabel = gameObject.GetComponent<UILabel>();
					break;
				case "Score":
					ScoreLabel = gameObject.GetComponent<UILabel>();
					break;
				case "UserIcon":
					Icon = gameObject.GetComponent<PlayerIcon>();
					Icon.createMaterial();
					break;
				case "dummy_icon":
					DummyIcon = gameObject.GetComponent<DummyFriendIcon>();
					break;
				}
			}
		}
	}

	private UserUI[] userUIs_ = new UserUI[2];

	private UISysFontLabel[] NameSysLabels_ = new UISysFontLabel[2];

	private UserData user1_;

	private UserData user2_;

	private int score_;

	private int stageNo_;

	private DummyFriendDataTable dummyFriendTbl_;

	private UIButton[] buttons_ = new UIButton[2];

	private DialogSendBase.OnSendSuccess sendCB_;

	public override void OnCreate()
	{
		base.OnCreate();
		UIButton[] componentsInChildren = GetComponentsInChildren<UIButton>(true);
		UIButton[] array = componentsInChildren;
		foreach (UIButton uIButton in array)
		{
			switch (uIButton.name)
			{
			case "CloseButtun":
				buttons_[0] = uIButton;
				break;
			case "SendButton":
				buttons_[1] = uIButton;
				break;
			}
		}
		dummyFriendTbl_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<DummyFriendDataTable>();
		sendCB_ = OnSendSuccess;
	}

	public IEnumerator show(int score, UserData user1, UserData user2, int currentRank, int prevRank, int stageNo)
	{
		user1_ = user1;
		user2_ = user2;
		score_ = score;
		stageNo_ = stageNo;
		Input.enable = false;
		for (int i = 0; i < userUIs_.Length; i++)
		{
			userUIs_[i] = new UserUI();
			UserUI ui = userUIs_[i];
			GameObject userRoot = base.transform.Find("window/User" + (i + 1)).gameObject;
			ui.setup(userRoot);
			if (i == 0)
			{
				ui.NameLabel.text = user1.UserName;
				ui.NameLabel.text = Constant.UserName.ReplaceOverStr(ui.NameLabel);
				ui.ScoreLabel.text = score.ToString("N0");
				ui.RankLabel.text = currentRank.ToString();
				dialogManager_.StartCoroutine(loadPlayerIcon(user1, ui.Icon));
			}
			else
			{
				ui.NameLabel.text = user2.UserName;
				ui.NameLabel.text = Constant.UserName.ReplaceOverStr(ui.NameLabel);
				ui.ScoreLabel.text = user2.Score.ToString("N0");
				ui.RankLabel.text = prevRank.ToString();
				ui.DummyIcon.gameObject.SetActive(user2.IsDummy);
				ui.Icon.gameObject.SetActive(!user2.IsDummy);
				if (user2.IsDummy)
				{
					ui.DummyIcon.setFriendSprite(dummyFriendTbl_.getInfo((int)user2.ID));
				}
				else
				{
					dialogManager_.StartCoroutine(loadPlayerIcon(user2, ui.Icon));
				}
			}
			Utility.createSysLabel(ui.NameLabel, ref NameSysLabels_[i]);
		}
		if (user2_.IsDummy)
		{
			ActionReward.setupButton(buttons_[1].transform, ActionReward.eType.Invalid);
			NGUIUtility.disable(buttons_[1], false);
		}
		else
		{
			yield return dialogManager_.StartCoroutine(ActionReward.getActionRewardInfo(ActionReward.eType.HiscoreChange));
			ActionReward.setupButton(buttons_[1].transform, ActionReward.eType.HiscoreChange);
		}
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		float waitTime = 0f;
		Sound.Instance.playSe(Sound.eSe.SE_235_target);
		for (int j = 0; j < userUIs_.Length; j++)
		{
			UserUI ui2 = userUIs_[j];
			ui2.Tween.Reset();
			ui2.Tween.Play(true);
			waitTime = Mathf.Max(ui2.Tween.duration + ui2.Tween.delay, waitTime);
		}
		Input.enable = true;
	}

	public IEnumerator showRankingStage(int score, UserData user1, UserData user2, int currentRank, int prevRank)
	{
		user1_ = user1;
		user2_ = user2;
		score_ = score;
		Input.enable = false;
		for (int i = 0; i < userUIs_.Length; i++)
		{
			userUIs_[i] = new UserUI();
			UserUI ui = userUIs_[i];
			GameObject userRoot = base.transform.Find("window/User" + (i + 1)).gameObject;
			ui.setup(userRoot);
			if (i == 0)
			{
				ui.NameLabel.text = user1.UserName;
				ui.NameLabel.text = Constant.UserName.ReplaceOverStr(ui.NameLabel);
				ui.ScoreLabel.text = score.ToString("N0");
				ui.RankLabel.text = currentRank.ToString();
				dialogManager_.StartCoroutine(loadPlayerIcon(user1, ui.Icon));
			}
			else
			{
				ui.NameLabel.text = user2.UserName;
				ui.NameLabel.text = Constant.UserName.ReplaceOverStr(ui.NameLabel);
				ui.ScoreLabel.text = user2.Score.ToString("N0");
				ui.RankLabel.text = prevRank.ToString();
				ui.DummyIcon.gameObject.SetActive(user2.IsDummy);
				ui.Icon.gameObject.SetActive(!user2.IsDummy);
				if (user2.IsDummy)
				{
					ui.DummyIcon.setFriendSprite(dummyFriendTbl_.getInfo((int)user2.ID));
				}
				else
				{
					partManager_.StartCoroutine(loadPlayerIcon(user2, ui.Icon));
				}
			}
			Utility.createSysLabel(ui.NameLabel, ref NameSysLabels_[i]);
		}
		if (user2_.IsDummy)
		{
			ActionReward.setupButton(buttons_[1].transform, ActionReward.eType.Invalid);
			NGUIUtility.disable(buttons_[1], false);
		}
		else
		{
			yield return dialogManager_.StartCoroutine(ActionReward.getActionRewardInfo(ActionReward.eType.HiscoreChange));
			ActionReward.setupButton(buttons_[1].transform, ActionReward.eType.HiscoreChange);
		}
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		float waitTime = 0f;
		Sound.Instance.playSe(Sound.eSe.SE_235_target);
		for (int j = 0; j < userUIs_.Length; j++)
		{
			UserUI ui2 = userUIs_[j];
			ui2.Tween.Reset();
			ui2.Tween.Play(true);
			waitTime = Mathf.Max(ui2.Tween.duration + ui2.Tween.delay, waitTime);
		}
		Input.enable = true;
	}

	private IEnumerator loadPlayerIcon(UserData user, PlayerIcon icon)
	{
		if (user.Texture == null)
		{
			yield return dialogManager_.StartCoroutine(icon.loadTexture(user.URL, true, null, -1));
			user.Texture = icon.getLoadTexture();
		}
		else
		{
			icon.setTexture(user.Texture);
		}
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "CloseButtun":
			Constant.SoundUtil.PlayDecideSE();
			Object.Destroy(base.transform.Find("window/User1").GetComponent<UIPanel>());
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "SendButton":
		{
			Constant.SoundUtil.PlayDecideSE();
			DialogSendHighScore dialog = dialogManager_.getDialog(DialogManager.eDialog.SendHighScore) as DialogSendHighScore;
			dialog.setup(user2_.Mid, user1_, user2_, score_, stageNo_);
			dialog.setCB(sendCB_);
			dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
			break;
		}
		}
	}

	private void OnSendSuccess()
	{
		buttons_[1].transform.Find("Label_reward").GetComponent<UILabel>().color = Color.gray;
		buttons_[1].transform.Find("icon_coin").GetComponent<UISprite>().color = Color.gray;
		NGUIUtility.disable(buttons_[1], false);
	}
}
