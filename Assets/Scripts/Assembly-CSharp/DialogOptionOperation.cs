using System.Collections;
using UnityEngine;

public class DialogOptionOperation : DialogOptionBase
{
	private enum eCheckBox
	{
		TypeA = 0,
		TypeB = 1,
		Max = 2
	}

	private UICheckbox[] checkBoxs_ = new UICheckbox[2];

	private eCheckBox currentType_;

	public override void OnCreate()
	{
		init();
		UICheckbox[] componentsInChildren = GetComponentsInChildren<UICheckbox>(true);
		UICheckbox[] array = componentsInChildren;
		foreach (UICheckbox uICheckbox in array)
		{
			switch (uICheckbox.name)
			{
			case "type_A":
				checkBoxs_[0] = uICheckbox;
				break;
			case "type_B":
				checkBoxs_[1] = uICheckbox;
				break;
			}
		}
	}

	public void setup()
	{
		currentType_ = (optionData_.getFlag(SaveOptionData.eFlag.ShootButton) ? eCheckBox.TypeB : eCheckBox.TypeA);
		setActiveCheckBox(checkBoxs_, 0, !optionData_.getFlag(SaveOptionData.eFlag.ShootButton));
		setActiveCheckBox(checkBoxs_, 1, optionData_.getFlag(SaveOptionData.eFlag.ShootButton));
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
		{
			Constant.SoundUtil.PlayDecideSE();
			optionData_.setFlag(SaveOptionData.eFlag.ShootButton, currentType_ == eCheckBox.TypeB);
			PartBase part = partManager_.execPart;
			if (part is Part_Stage)
			{
				((Part_Stage)part).showShootButton();
			}
			else if (part is Part_BonusStage)
			{
				((Part_BonusStage)part).showShootButton();
			}
			else if (part is Part_BossStage)
			{
				((Part_BossStage)part).showShootButton();
			}
			save();
			updateOptionDialog();
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
		}
	}

	private void pressCheckBox(UICheckbox checkBox)
	{
		for (int i = 0; i < checkBoxs_.Length; i++)
		{
			if (checkBoxs_[i] == checkBox)
			{
				currentType_ = (eCheckBox)i;
			}
			checkBoxs_[i].isChecked = false;
		}
		checkBox.isChecked = true;
	}
}
