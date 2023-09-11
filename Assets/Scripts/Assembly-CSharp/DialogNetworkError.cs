using System.Collections;
using Network;
using UnityEngine;

public class DialogNetworkError : DialogBase
{
	private eStatus status_;

	public IEnumerator show(WWW www, bool bCancel)
	{
		status_ = eStatus.None;
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
	}

	public eStatus getStatus()
	{
		return status_;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "ConfirmButton":
			Constant.SoundUtil.PlayDecideSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			status_ = eStatus.Cancel;
			break;
		}
	}
}
