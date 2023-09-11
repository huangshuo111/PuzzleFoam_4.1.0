using System.Collections;

public abstract class DialogOptionBase : DialogBase
{
	protected SaveOptionData optionData_;

	protected MessageResource msgRes_;

	protected DialogCommon commonDialog_;

	protected void init()
	{
		commonDialog_ = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		msgRes_ = MessageResource.Instance;
		SaveSystemData systemData = SaveData.Instance.getSystemData();
		optionData_ = systemData.getOptionData();
	}

	protected void setActiveCheckBox(UICheckbox[] boxs, int index, bool bActive)
	{
		boxs[index].isChecked = bActive;
	}

	protected void saveFlag(SaveOptionData.eFlag flag, UICheckbox[] boxs, int index)
	{
		optionData_.setFlag(flag, boxs[index].isChecked);
		optionData_.save();
	}

	protected IEnumerator openCommonDialog(bool bWarning)
	{
		string msg = MessageResource.Instance.getMessage(57);
		if (bWarning)
		{
			msg += MessageResource.Instance.getMessage(4013);
		}
		commonDialog_.setup(msg, null, null, true);
		commonDialog_.setButtonActive(DialogCommon.eBtn.Close, false);
		commonDialog_.sysLabelEnable(false);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(commonDialog_));
		while (commonDialog_.isOpen())
		{
			yield return 0;
		}
		commonDialog_.sysLabelEnable(true);
	}

	protected void updateOptionDialog()
	{
		DialogOption dialogOption = dialogManager_.getDialog(DialogManager.eDialog.Option) as DialogOption;
		dialogOption.setup();
	}

	protected void save()
	{
		optionData_.save();
	}
}
