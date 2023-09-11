using System.Collections;
using UnityEngine;

public class DebugMenuDialog : DebugMenuBase
{
	private enum eItem
	{
		Dialog = 0,
		Open = 1,
		Max = 2
	}

	private DialogManager dialogMng_;

	private static int dialogType_;

	private static DialogBase currentDialog_;

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(2, "Dialog"));
		GameObject obj = GameObject.Find("DialogManager");
		if (!(obj == null))
		{
			dialogMng_ = obj.GetComponent<DialogManager>();
			currentDialog_ = getDialog();
		}
	}

	public override void OnDraw()
	{
		if (!(dialogMng_ == null))
		{
			if (currentDialog_ == null)
			{
				DrawItem(0, "Not Load");
				return;
			}
			DrawItem(0, currentDialog_.getDialogType().ToString());
			DrawItem(1, "Open : " + currentDialog_.isOpen());
		}
	}

	public override void OnExecute()
	{
		if (dialogMng_ == null)
		{
			return;
		}
		dialogType_ = (int)Vary(0, dialogType_, 1, 0, DialogManager.DialogMax);
		currentDialog_ = getDialog();
		if (!(currentDialog_ == null) && IsPressCenterButton(1))
		{
			if (currentDialog_.isOpen())
			{
				StartCoroutine(dialogMng_.closeDialog(currentDialog_));
			}
			else
			{
				StartCoroutine(dialogMng_.openDialog(currentDialog_));
			}
		}
	}

	private DialogBase getDialog()
	{
		DialogManager.eDialog dialog = (DialogManager.eDialog)(1 << dialogType_);
		if (!dialogMng_.isLoaded(dialog))
		{
			return null;
		}
		return dialogMng_.getDialog(dialog);
	}
}
