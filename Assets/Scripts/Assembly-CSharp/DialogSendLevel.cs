using System.Collections;
using Network;

public class DialogSendLevel : DialogSendBase
{
	protected string sendMsg_ = string.Empty;

	public void setup(int lv)
	{
		MessageResource instance = MessageResource.Instance;
		leveltext_ = lv.ToString();
		sendMsg_ = lv.ToString();
		string message = instance.getMessage(32);
		message = instance.castCtrlCode(message, 1, lv.ToString());
		setMessage(message);
		ActionReward.setupButton(base.transform.Find("window/SendButtun"), ActionReward.eType.LevelUp);
	}

	protected override IEnumerator send()
	{
		yield return StartCoroutine(registerTimeline(7000044L, sendMsg_, eTimeLineType.LevelUp));
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
