using System.Collections;
using UnityEngine;

public class DialogOptionAvatar : DialogOptionBase
{
	private enum eCheckBox
	{
		Default = 0,
		BubblenMarch = 1,
		Max = 2
	}

	private UICheckbox[] checkBoxs_ = new UICheckbox[2];

	private UISprite previewImg;

	private string avatar_ = string.Empty;

	public override void OnCreate()
	{
		init();
		Transform transform = base.transform.Find("window/Avatar_Button");
		UICheckbox[] componentsInChildren = transform.GetComponentsInChildren<UICheckbox>(true);
		UICheckbox[] array = componentsInChildren;
		foreach (UICheckbox uICheckbox in array)
		{
			switch (uICheckbox.name)
			{
			case "default":
				checkBoxs_[0] = uICheckbox;
				break;
			case "collabo_01":
				checkBoxs_[1] = uICheckbox;
				break;
			}
		}
		previewImg = base.transform.Find("window/img/preview_img").GetComponent<UISprite>();
	}

	public void setup()
	{
		avatar_ = PlayerPrefs.GetString(SaveKeys.getOptionCharacterAvatarKey());
		setActiveCheckBox(checkBoxs_, 0, avatar_ == string.Empty);
		setActiveCheckBox(checkBoxs_, 1, avatar_ == "_00");
		if (avatar_ == string.Empty)
		{
			previewImg.spriteName = "avatarImg_default";
		}
		else
		{
			previewImg.spriteName = "avatarImg_collaboration";
		}
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
		bool bDialog = avatar_ != optionData_.getAvatar();
		optionData_.setAvatar(avatar_);
		save();
		updateOptionDialog();
		yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
	}

	private void pressCheckBox(UICheckbox checkBox)
	{
		eCheckBox eCheckBox = eCheckBox.Default;
		for (int i = 0; i < checkBoxs_.Length; i++)
		{
			if (checkBoxs_[i] == checkBox)
			{
				eCheckBox = (eCheckBox)i;
			}
			checkBoxs_[i].isChecked = false;
		}
		checkBox.isChecked = true;
		if (eCheckBox == eCheckBox.BubblenMarch)
		{
			avatar_ = "_00";
			previewImg.spriteName = "avatarImg_collaboration";
		}
		else
		{
			avatar_ = string.Empty;
			previewImg.spriteName = "avatarImg_default";
		}
	}

	private void setActiveCheckBox(eCheckBox type, bool bActive)
	{
		checkBoxs_[(int)type].isChecked = bActive;
	}
}
