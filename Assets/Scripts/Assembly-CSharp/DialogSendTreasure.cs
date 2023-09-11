using System.Collections;
using Network;

public class DialogSendTreasure : DialogSendBase
{
	protected string sendMsg_ = string.Empty;

	public IEnumerator show(Constant.Reward reward)
	{
		MessageResource msgRes = MessageResource.Instance;
		ActionReward.setupButton(base.transform.Find("window/SendButtun"), ActionReward.eType.TreasureBox);
		sendMsg_ = msgRes.castCtrlCode(msgRes.getMessageDeviceLanguage((int)(2565 + reward.RewardType - 1)), 1, reward.Num.ToString("N0"));
		setMessage(msgRes.getMessage(2550));
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
	}

	protected override IEnumerator send()
	{
		yield return StartCoroutine(registerTimeline(7000048L, sendMsg_, eTimeLineType.Treasure));
		bool bError = !isSuccessRegisterTimeline();
		if (DialogSendBase.internalError)
		{
			yield return StartCoroutine(NetworkMng.Instance.openErrorDialog(false, eResultCode.ErrorUnknown));
			DialogSendBase.internalError = false;
		}
		else if (bError)
		{
			yield return StartCoroutine(NetworkMng.Instance.openErrorDialog(false, eResultCode.ErrorUnknown));
			DialogSendBase.internalError = false;
		}
		else
		{
			yield return dialogManager_.StartCoroutine(sendSuccess());
		}
	}

	private IEnumerator sendSuccess()
	{
		yield return dialogManager_.StartCoroutine(ActionReward.addActionReward(dialogManager_));
		yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
		callCB();
	}
}
