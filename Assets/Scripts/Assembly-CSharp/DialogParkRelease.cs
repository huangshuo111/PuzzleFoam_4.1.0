using System.Collections;
using UnityEngine;

public class DialogParkRelease : DialogBase
{
	private IEnumerator OnButton(GameObject trigger)
	{
		Constant.SoundUtil.PlayDecideSE();
		dialogManager_.StartCoroutine(dialogManager_.closeDialog(DialogManager.eDialog.ParkReleaseNotice));
		yield break;
	}
}
