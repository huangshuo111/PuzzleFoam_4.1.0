using System.Collections;

public class DialogSendBonusCoin : DialogSendBase
{
	protected string sendMsg_ = string.Empty;

	private bool bCoinSend;

	public bool isSend;

	public IEnumerator show(int coin)
	{
		isSend = false;
		MessageResource msgRes = MessageResource.Instance;
		sendMsg_ = msgRes.getMessageDeviceLanguage(4526);
		sendMsg_ = msgRes.castCtrlCode(sendMsg_, 1, coin.ToString());
		string msg2 = msgRes.getMessage(4516);
		msg2 = msgRes.castCtrlCode(msg2, 1, coin.ToString());
		setMessage(msg2);
		yield return dialogManager_.StartCoroutine(ActionReward.getActionRewardInfo(ActionReward.eType.AreaClear));
		ActionReward.setupButton(base.transform.Find("window/SendButtun"), ActionReward.eType.AreaClear);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
	}

	protected override IEnumerator send()
	{
		yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
	}

	private IEnumerator sendSuccess()
	{
		yield return dialogManager_.StartCoroutine(ActionReward.addActionReward(dialogManager_));
		bCoinSend = false;
		yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
		callCB();
	}
}
