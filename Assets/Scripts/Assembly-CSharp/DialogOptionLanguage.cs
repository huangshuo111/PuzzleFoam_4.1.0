using System.Collections;
using UnityEngine;

public class DialogOptionLanguage : DialogOptionBase
{
	private enum eCheckBox
	{
		JP = 0,
		EN = 1,
		Max = 2
	}

	private UICheckbox[] checkBoxs_ = new UICheckbox[2];

	private string language_ = string.Empty;

	public override void OnCreate()
	{
		init();
		UICheckbox[] componentsInChildren = GetComponentsInChildren<UICheckbox>(true);
		UICheckbox[] array = componentsInChildren;
		foreach (UICheckbox uICheckbox in array)
		{
			switch (uICheckbox.name)
			{
			case "JP":
				checkBoxs_[0] = uICheckbox;
				break;
			case "EN":
				checkBoxs_[1] = uICheckbox;
				break;
			}
		}
	}

	public void setup()
	{
		language_ = optionData_.getLanguage();
		setActiveCheckBox(checkBoxs_, 0, SystemLanguage.Japanese.ToString() == language_);
		setActiveCheckBox(checkBoxs_, 1, SystemLanguage.Japanese.ToString() != language_);
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		UICheckbox checkBox = trigger.GetComponent<UICheckbox>();
		if (checkBox != null)
		{
			Constant.SoundUtil.PlayButtonSE();
			pressCheckBox(checkBox);
			yield break;
		}
		switch (trigger.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "ConfirmButton":
			Constant.SoundUtil.PlayDecideSE();
			dialogManager_.StartCoroutine(confirm());
			break;
		}
	}

	private IEnumerator confirm()
	{
		bool bDialog = language_ != optionData_.getLanguage();
		optionData_.setLanguage(language_);
		save();
		updateOptionDialog();
		yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
		if (bDialog)
		{
			dialogManager_.StartCoroutine(openCommonDialog(false));
		}
	}

	private void pressCheckBox(UICheckbox checkBox)
	{
		eCheckBox eCheckBox = eCheckBox.JP;
		for (int i = 0; i < checkBoxs_.Length; i++)
		{
			if (checkBoxs_[i] == checkBox)
			{
				eCheckBox = (eCheckBox)i;
			}
			checkBoxs_[i].isChecked = false;
		}
		checkBox.isChecked = true;
		language_ = SystemLanguage.Korean.ToString();
	}

	private void setActiveCheckBox(eCheckBox type, bool bActive)
	{
		checkBoxs_[(int)type].isChecked = bActive;
	}
}
