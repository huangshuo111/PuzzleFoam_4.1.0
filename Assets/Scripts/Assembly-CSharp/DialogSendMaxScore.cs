using System.Collections;
using Network;

public class DialogSendMaxScore : DialogSendBase
{
	protected string sendMsg_ = string.Empty;

	public IEnumerator show(int score, int stageno)
	{
		MessageResource msgRes = MessageResource.Instance;
		sendMsg_ = score.ToString();
		scoretext_ = score.ToString();
		stageNo_ = stageno.ToString();
		string msg2 = msgRes.getMessage(34);
		msg2 = msgRes.castCtrlCode(msg2, 1, score.ToString());
		setMessage(msg2);
		yield return dialogManager_.StartCoroutine(ActionReward.getActionRewardInfo(ActionReward.eType.BestScore));
		ActionReward.setupButton(base.transform.Find("window/SendButtun"), ActionReward.eType.BestScore);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
	}

	protected override IEnumerator send()
	{
		yield return StartCoroutine(registerTimeline(7000054L, sendMsg_, eTimeLineType.HighScore));
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
