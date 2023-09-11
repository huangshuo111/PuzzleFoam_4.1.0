using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogReward : DialogBase
{
	private GameObject[] items_ = new GameObject[4];

	private UILabel priceLabel_;

	private UILabel textLabel_;

	private MainMenu mainMenu_;

	private MessageResource msgRes_;

	private UIButton sendButton_;

	private GameObject confirmButton_;

	private GameObject eventConfirmButton_;

	private DialogSendBase.OnSendSuccess cb_;

	private int inputEnableCount_;

	private Constant.Reward sendReward_;

	public override void OnCreate()
	{
		msgRes_ = MessageResource.Instance;
		mainMenu_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		Transform[] componentsInChildren = base.transform.GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			switch (transform.name)
			{
			case "coin_icon":
				items_[0] = transform.gameObject;
				break;
			case "heart_icon":
				items_[2] = transform.gameObject;
				break;
			case "jewel_icon":
				items_[1] = transform.gameObject;
				break;
			case "ticket_icon":
				items_[3] = transform.gameObject;
				break;
			case "Label00":
				priceLabel_ = transform.GetComponent<UILabel>();
				break;
			case "Label01":
				textLabel_ = transform.GetComponent<UILabel>();
				break;
			case "NoticeButton":
				sendButton_ = transform.GetComponent<UIButton>();
				break;
			case "ConfirmButton":
				confirmButton_ = transform.gameObject;
				break;
			case "ConfirmButton_Event":
				eventConfirmButton_ = transform.gameObject;
				break;
			}
		}
		cb_ = OnSendSuccess;
	}

	public override void OnClose()
	{
		Sound.Instance.pauseBgm(false);
	}

	public IEnumerator show(Constant.Reward reward, List<Constant.eMoney> limitOverMoneys, string textMessage)
	{
		Input.enable = false;
		bool bTreasure = partManager_.currentPart == PartManager.ePart.Map;
		sendButton_.gameObject.SetActive(bTreasure);
		confirmButton_.SetActive(bTreasure);
		eventConfirmButton_.SetActive(!bTreasure);
		if (bTreasure)
		{
			yield return dialogManager_.StartCoroutine(ActionReward.getActionRewardInfo(ActionReward.eType.TreasureBox));
			ActionReward.setupButton(sendButton_.transform, ActionReward.eType.TreasureBox);
		}
		setupItems(reward.RewardType);
		sendReward_ = reward;
		switch (reward.RewardType)
		{
		case Constant.eMoney.Jewel:
			base.transform.Find("window/Chara/anm1").GetComponent<UISprite>().spriteName = "UI_chara_00_019";
			break;
		case Constant.eMoney.Coin:
			base.transform.Find("window/Chara/anm1").GetComponent<UISprite>().spriteName = "UI_chara_00_008";
			break;
		case Constant.eMoney.Heart:
			base.transform.Find("window/Chara/anm1").GetComponent<UISprite>().spriteName = "UI_chara_00_020";
			break;
		}
		string msg = string.Empty;
		msg = msgRes_.getMessage((reward.RewardType != Constant.eMoney.Coin) ? 28 : 31);
		msg = msgRes_.castCtrlCode(msg, 1, reward.Num.ToString("N0"));
		priceLabel_.text = msg;
		textLabel_.text = textMessage;
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		if (bTreasure)
		{
			SaveOtherData otherData = SaveData.Instance.getGameData().getOtherData();
			if (!otherData.isFlag(SaveOtherData.eFlg.TutorialTreasure))
			{
				GameObject uiRoot = dialogManager_.getCurrentUiRoot();
				TutorialManager.Instance.load(-5, uiRoot);
				Input.enable = true;
				yield return StartCoroutine(TutorialManager.Instance.play(-5, TutorialDataTable.ePlace.Setup, uiRoot, null, null));
				Input.enable = false;
				TutorialManager.Instance.unload(-5);
				otherData.setFlag(SaveOtherData.eFlg.TutorialTreasure, true);
				otherData.save();
			}
		}
		startConfettiEff();
		Sound.Instance.playSe(Sound.eSe.SE_113_Clap);
		StartCoroutine(bgmReplay());
		mainMenu_.update();
		if (limitOverMoneys != null && limitOverMoneys.Count > 0)
		{
			yield return StartCoroutine(showLimitOverDialog(limitOverMoneys));
		}
		else
		{
			Input.enable = true;
		}
		inputEnableCount_ = Input.forceEnable();
	}

	private IEnumerator bgmReplay()
	{
		while (Sound.Instance.isPlayingSe(Sound.eSe.SE_113_Clap))
		{
			yield return null;
		}
		Sound.Instance.pauseBgm(false);
	}

	private IEnumerator showLimitOverDialog(List<Constant.eMoney> limitOverMoneys)
	{
		Input.enable = true;
		DialogLimitOver limitOverDialog = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
		foreach (Constant.eMoney money in limitOverMoneys)
		{
			Input.enable = false;
			yield return StartCoroutine(limitOverDialog.show(money));
			Input.enable = true;
			while (limitOverDialog.isOpen())
			{
				yield return 0;
			}
		}
	}

	private void setupItems(Constant.eMoney gainMoney)
	{
		for (int i = 0; i < items_.Length; i++)
		{
			items_[i].SetActive((i + 1 == (int)gainMoney) ? true : false);
		}
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			Input.revertForceEnable(inputEnableCount_);
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			NGUIUtility.enable(sendButton_, false);
			break;
		case "ConfirmButton":
		case "ConfirmButton_Event":
			Constant.SoundUtil.PlayDecideSE();
			Input.revertForceEnable(inputEnableCount_);
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			NGUIUtility.enable(sendButton_, false);
			break;
		case "NoticeButton":
		{
			Constant.SoundUtil.PlayDecideSE();
			DialogSendTreasure dialog = dialogManager_.getDialog(DialogManager.eDialog.SendTreasure) as DialogSendTreasure;
			dialog.setCB(cb_);
			yield return dialogManager_.StartCoroutine(dialog.show(sendReward_));
			break;
		}
		}
	}

	private void OnSendSuccess()
	{
		sendButton_.transform.Find("Label").GetComponent<UILabel>().color = Color.gray;
		sendButton_.transform.Find("Label_reward").GetComponent<UILabel>().color = Color.gray;
		sendButton_.transform.Find("icon_coin").GetComponent<UISprite>().color = Color.gray;
		NGUIUtility.disable(sendButton_, false);
	}
}
