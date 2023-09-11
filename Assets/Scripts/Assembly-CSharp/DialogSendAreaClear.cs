using System.Collections;
using Network;

public class DialogSendAreaClear : DialogSendBase
{
	protected string sendMsg_ = string.Empty;

	public IEnumerator show(int area)
	{
		MessageResource msgRes = MessageResource.Instance;
		int area_name_index = 1300 + area;
		if (area >= 50000)
		{
			area_name_index = 9200 + area % 10000;
		}
		sendMsg_ = msgRes.getMessageDeviceLanguage(area_name_index);
		string msg2 = msgRes.getMessage(67);
		msg2 = msgRes.castCtrlCode(msg2, 1, msgRes.getMessage(area_name_index));
		setMessage(msg2);
		yield return dialogManager_.StartCoroutine(ActionReward.getActionRewardInfo(ActionReward.eType.AreaClear));
		ActionReward.setupButton(base.transform.Find("window/SendButtun"), ActionReward.eType.AreaClear);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
	}

	protected override IEnumerator send()
	{
		yield return StartCoroutine(registerTimeline(7000050L, sendMsg_, eTimeLineType.AreaClear));
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
