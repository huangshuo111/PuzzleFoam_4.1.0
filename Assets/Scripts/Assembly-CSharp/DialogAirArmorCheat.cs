using System.Collections;
using UnityEngine;

public class DialogAirArmorCheat : DialogBase
{
	public bool previewPause;

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "OKButton":
			fadeManager_.setActive(FadeMng.eType.AllMask, true);
			yield return StartCoroutine(fadeManager_.startFade(FadeMng.eType.AllMask, 0f, 1f, 0.1f));
			yield return new WaitForEndOfFrame();
			Application.Quit();
			break;
		}
	}

	public override IEnumerator open()
	{
		partManager_.inhibitTips(true);
		yield return null;
	}

	public void CheatOpen()
	{
		StartCoroutine(CheatCoroutine());
	}

	private IEnumerator CheatCoroutine()
	{
		DialogAirArmorCheat dialog = dialogManager_.getDialog(DialogManager.eDialog.AirArmorCheat) as DialogAirArmorCheat;
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
		while (dialog.isOpen())
		{
			yield return 0;
		}
	}
}
