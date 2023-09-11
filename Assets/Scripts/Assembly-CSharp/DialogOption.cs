using System.Collections;
using LitJson;
using Network;
using UnityEngine;

public class DialogOption : DialogOptionBase
{
	private enum eCheckBox
	{
		BGM = 0,
		SE = 1,
		HighQuality = 2,
		PushNotice = 3,
		Max = 4
	}

	private enum eLabel
	{
		Operation = 0,
		Version = 1,
		Language = 2,
		KakaoNo = 3,
		NewestVersion = 4,
		Avatar = 5,
		Max = 6
	}

	private const string CAPTURE_URL = "http://linegameblog.jp/archives/36928261.html";

	private UICheckbox[] checkBoxs_ = new UICheckbox[4];

	private UILabel[] labels_ = new UILabel[6];

	private Sound sound_;

	private bool bPlayStage;

	public override void OnCreate()
	{
		sound_ = Sound.Instance;
		init();
		UICheckbox[] componentsInChildren = GetComponentsInChildren<UICheckbox>(true);
		UICheckbox[] array = componentsInChildren;
		foreach (UICheckbox uICheckbox in array)
		{
			switch (uICheckbox.name)
			{
			case "00_BGM":
				checkBoxs_[0] = uICheckbox;
				break;
			case "01_SE":
				checkBoxs_[1] = uICheckbox;
				break;
			case "01_recover":
				checkBoxs_[3] = uICheckbox;
				break;
			case "00_highquality_mode":
				checkBoxs_[2] = uICheckbox;
				break;
			}
		}
		labels_[0] = base.transform.Find("window/ETC/04_operation/Label_01").GetComponent<UILabel>();
		labels_[2] = base.transform.Find("window/ETC/05_language/Label_01").GetComponent<UILabel>();
		labels_[1] = base.transform.Find("CurrentVersion_Label").GetComponent<UILabel>();
		labels_[4] = base.transform.Find("NewestVersion_Label").GetComponent<UILabel>();
		labels_[3] = base.transform.Find("KakaoNo_Label").GetComponent<UILabel>();
		string message = msgRes_.getMessage(500012);
		message = msgRes_.castCtrlCode(message, 1, SaveData.Instance.getAppVersion());
		labels_[1].text = message;
		message = msgRes_.getMessage(500013);
		message = msgRes_.castCtrlCode(message, 1, SaveData.Instance.ServerVersion);
		labels_[4].text = message;
	}

	public void setup()
	{
		setActiveCheckBox(checkBoxs_, 0, optionData_.getFlag(SaveOptionData.eFlag.BGM));
		setActiveCheckBox(checkBoxs_, 1, optionData_.getFlag(SaveOptionData.eFlag.SE));
		setActiveCheckBox(checkBoxs_, 3, optionData_.getFlag(SaveOptionData.eFlag.PushNotice));
		setActiveCheckBox(checkBoxs_, 2, optionData_.getFlag(SaveOptionData.eFlag.HighQuality));
		setOperationLabel();
		setLanguageLabel();
		setKakaoNoLabel();
		bPlayStage = partManager_.currentPart == PartManager.ePart.Stage || partManager_.currentPart == PartManager.ePart.BonusStage || partManager_.currentPart == PartManager.ePart.BossStage;
		setLogOutSprite();
	}

	private void setLanguageLabel()
	{
		bool flag = optionData_.isKorean();
		string message = msgRes_.getMessage((!flag) ? 56 : 55);
		labels_[2].text = message;
	}

	private void setKakaoNoLabel()
	{
		string message = msgRes_.getMessage(500011);
		message = ((!SNSCore.IsAuthorize) ? msgRes_.castCtrlCode(message, 1, string.Empty) : msgRes_.castCtrlCode(message, 1, GlobalData.Instance.LineID.ToString()));
		labels_[3].text = message;
	}

	private void setOperationLabel()
	{
		bool flag = !optionData_.getFlag(SaveOptionData.eFlag.ShootButton);
		string message = msgRes_.getMessage((!flag) ? 54 : 53);
		labels_[0].text = message;
	}

	private void setLogOutSprite()
	{
		UISprite component = base.transform.Find("window/ETC/07_logout/Background").GetComponent<UISprite>();
		if (component == null)
		{
			Debug.Log(base.transform.Find("window/ETC/07_logout/Background").name);
		}
		else if (SNSCore.IsAuthorize)
		{
			component.transform.parent.gameObject.SetActive(true);
			component.spriteName = "kakaologout";
		}
		else
		{
			component.spriteName = "guestlogout";
		}
	}

	private void OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "00_BGM":
		{
			Constant.SoundUtil.PlayButtonSE();
			saveFlag(SaveOptionData.eFlag.BGM, checkBoxs_, 0);
			bool flag = optionData_.getFlag(SaveOptionData.eFlag.BGM);
			sound_.setBgmMasterVolume((!flag) ? 0f : sound_.getDefaultBgmVolume());
			sound_.setBgmVolume((!flag) ? 0f : sound_.getDefaultBgmVolume());
			break;
		}
		case "01_SE":
		{
			saveFlag(SaveOptionData.eFlag.SE, checkBoxs_, 1);
			bool flag2 = optionData_.getFlag(SaveOptionData.eFlag.SE);
			sound_.setSeMasterVolume((!flag2) ? 0f : sound_.getDefaultSeVolume());
			if (flag2)
			{
				PlayerPrefs.SetInt("KakaoIntroSound", 0);
			}
			else
			{
				PlayerPrefs.SetInt("KakaoIntroSound", 1);
			}
			if (!flag2)
			{
				sound_.stopSe();
			}
			else
			{
				Constant.SoundUtil.PlayButtonSE();
			}
			break;
		}
		case "00_highquality_mode":
		{
			Constant.SoundUtil.PlayButtonSE();
			bool isChecked = checkBoxs_[2].isChecked;
			if (isChecked)
			{
				PlayerPrefs.SetInt("SetHighQuality", 1);
			}
			saveFlag(SaveOptionData.eFlag.HighQuality, checkBoxs_, 2);
			StartCoroutine(openCommonDialog(isChecked));
			break;
		}
		case "01_recover":
			Constant.SoundUtil.PlayButtonSE();
			saveFlag(SaveOptionData.eFlag.PushNotice, checkBoxs_, 3);
			break;
		case "03_notice":
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(WebView.Instance.show(WebView.eWebType.Notice, dialogManager_));
			break;
		case "01_help":
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(WebView.Instance.show(WebView.eWebType.Help, dialogManager_));
			break;
		case "00_event":
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(WebView.Instance.show(WebView.eWebType.Event, dialogManager_));
			break;
		case "02_tutorial":
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(OpenHowtoPlayIndex());
			break;
		case "10_cs":
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(WebView.Instance.show(WebView.eWebType.ContactUs, dialogManager_));
			break;
		case "04_operation":
		{
			Constant.SoundUtil.PlayButtonSE();
			DialogOptionOperation dialogOptionOperation = dialogManager_.getDialog(DialogManager.eDialog.OptionOperate) as DialogOptionOperation;
			dialogOptionOperation.setup();
			StartCoroutine(dialogManager_.openDialog(dialogOptionOperation));
			break;
		}
		case "05_language":
		{
			Constant.SoundUtil.PlayButtonSE();
			DialogOptionLanguage dialogOptionLanguage = dialogManager_.getDialog(DialogManager.eDialog.OptionLangage) as DialogOptionLanguage;
			dialogOptionLanguage.setup();
			StartCoroutine(dialogManager_.openDialog(dialogOptionLanguage));
			break;
		}
		case "06_buystate":
		{
			Constant.SoundUtil.PlayButtonSE();
			DialogPurchaseInfo dialogPurchaseInfo = dialogManager_.getDialog(DialogManager.eDialog.PurchaseInfo) as DialogPurchaseInfo;
			StartCoroutine(dialogPurchaseInfo.show());
			break;
		}
		case "07_logout":
		{
			Constant.SoundUtil.PlayButtonSE();
			DialogCommon dialogCommon = dialogManager_.getDialog(DialogManager.eDialog.Logout) as DialogCommon;
			dialogCommon.setup(500003, SNSLogout_CB, null, true);
			StartCoroutine(dialogManager_.openDialog(dialogCommon));
			break;
		}
		case "08_unregister":
		{
			Constant.SoundUtil.PlayButtonSE();
			DialogUnregister dialogUnregister = dialogManager_.getDialog(DialogManager.eDialog.Unregister) as DialogUnregister;
			StartCoroutine(dialogUnregister.open());
			break;
		}
		case "09_credit":
		{
			DialogCredit dialogCredit = dialogManager_.getDialog(DialogManager.eDialog.Credit) as DialogCredit;
			StartCoroutine(dialogCredit.show());
			break;
		}
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	private IEnumerator SNSLogout_CB()
	{
		if (SNSCore.IsAuthorize)
		{
			SNSCore.Instance.Logout();
			while (SNSCore.IsAuthorize)
			{
				yield return null;
			}
		}
		PlayerPrefs.SetInt("PolicyFlagSkonec", 0);
		while (PlayerPrefs.GetInt("PolicyFlagSkonec") != 0)
		{
			yield return null;
		}
		SNSCore.Instance.ClearCache();
		GlobalData.Instance.LineID = 0L;
		Part_Title.bStartGame = false;
		partManager_.gotoTitle();
	}

	private IEnumerator OpenHowtoPlayIndex()
	{
		Input.enable = false;
		GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		StageDataTable stageTbl = dataTable.GetComponent<StageDataTable>();
		Constant.eDownloadDataNo downloadData = Constant.eDownloadDataNo.HowtoPlay_JP;
		int file_num = Constant.outsideFiles_howtoplay_JP.Length;
		if (!ResourceLoader.Instance.isJapanResource())
		{
			downloadData = Constant.eDownloadDataNo.HowtoPlay_EN;
			file_num = Constant.outsideFiles_howtoplay_EN.Length;
		}
		GlobalData.Instance.isResourceDownloading = true;
		ResourceURLData resourceURLData2 = JsonMapper.ToObject<ResourceURLData>(stageTbl.getResourceURLData().text);
		for (int i = 0; i < file_num; i++)
		{
			bool isLast = ((i == file_num - 1) ? true : false);
			if (!stageTbl.isNewestGameResourceData(downloadData, i, resourceURLData2))
			{
				yield return StartCoroutine(stageTbl.downloadGameResource(downloadData, i, isLast, i, file_num));
			}
		}
		resourceURLData2 = null;
		GlobalData.Instance.isResourceDownloading = false;
		DialogHowToPlayIndex dialog = dialogManager_.getDialog(DialogManager.eDialog.HowToPlayIndex) as DialogHowToPlayIndex;
		dialog.isChallenge = false;
		dialog.setup();
		StartCoroutine(dialogManager_.openDialog(dialog));
		Input.enable = true;
	}
}
