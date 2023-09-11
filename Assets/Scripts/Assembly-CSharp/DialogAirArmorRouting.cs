using System.Collections;
using UnityEngine;

public class DialogAirArmorRouting : DialogBase
{
	public bool previewPause;

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Permit":
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			partManager_.inhibitTips(false);
			break;
		case "NotPermit":
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

	public void RoutingOpen()
	{
		StartCoroutine(RoutingCoroutine());
	}

	private IEnumerator RoutingCoroutine()
	{
		DialogAirArmorRouting dialog = dialogManager_.getDialog(DialogManager.eDialog.AirArmorRouting) as DialogAirArmorRouting;
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
		while (dialog.isOpen())
		{
			yield return 0;
		}
	}
}
