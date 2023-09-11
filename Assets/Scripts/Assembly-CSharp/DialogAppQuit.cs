using System.Collections;
using UnityEngine;

public class DialogAppQuit : DialogBase
{
	public bool previewPause;

	protected virtual IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "ConfirmButton":
			GlobalGoogleAnalytics.Instance.StopSession();
			Constant.SoundUtil.PlayDecideSE();
			fadeManager_.setActive(FadeMng.eType.AllMask, true);
			yield return StartCoroutine(fadeManager_.startFade(FadeMng.eType.AllMask, 0f, 1f, 0.1f));
			yield return new WaitForEndOfFrame();
			Application.Quit();
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			if (partManager_.currentPart == PartManager.ePart.Stage)
			{
				((Part_Stage)partManager_.execPart).stagePause.pause = previewPause;
			}
			if (partManager_.currentPart == PartManager.ePart.BonusStage)
			{
				((Part_BonusStage)partManager_.execPart).stagePause.pause = previewPause;
			}
			if (partManager_.currentPart == PartManager.ePart.BossStage)
			{
				((Part_BossStage)partManager_.execPart).stagePause.pause = previewPause;
			}
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			partManager_.inhibitTips(false);
			break;
		}
	}

	public override IEnumerator open()
	{
		partManager_.inhibitTips(true);
		DialogInformation infoDialog = dialogManager_.getDialog(DialogManager.eDialog.Information) as DialogInformation;
		if (infoDialog != null && infoDialog.isOpen())
		{
			infoDialog.showWebView(false);
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(infoDialog));
		}
		yield return dialogManager_.StartCoroutine(base.open());
	}
}
