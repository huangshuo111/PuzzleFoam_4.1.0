using System.Collections;
using Network;

public class DialogSendBoss : DialogSendBase
{
	protected string sendMsg_ = string.Empty;

	public void setup(int bossType, int bossLevel)
	{
		MessageResource instance = MessageResource.Instance;
		string message = MessageResource.Instance.getMessage(8200 + bossType).ToString();
		sendMsg_ = MessageResource.Instance.getMessage(8200 + bossType).ToString() + "(Lv" + bossLevel + ")";
		string message2 = instance.getMessage(3721);
		message2 = instance.castCtrlCode(message2, 1, message);
		message2 = instance.castCtrlCode(message2, 2, bossLevel.ToString());
		setMessage(message2);
		Debug.Log("sendMsg_ = " + sendMsg_);
		ActionReward.setupButton(base.transform.Find("window/SendButtun"), ActionReward.eType.Boss);
	}

	protected override IEnumerator send()
	{
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
