using UnityEngine;

public class SaveOptionData : SaveDataBase
{
	public enum eFlag
	{
		BGM = 0,
		SE = 1,
		PushNotice = 2,
		HeartNotice = 3,
		HighQuality = 4,
		ShootButton = 5
	}

	private int bitmap_;

	private string language_ = string.Empty;

	private string avatar_ = string.Empty;

	private string version_ = string.Empty;

	protected override void OnSetup()
	{
		if (!PlayerPrefs.HasKey(SaveKeys.getOptionFlgDataKey()))
		{
			saveDefault();
		}
		if (!PlayerPrefs.HasKey(SaveKeys.getOptionCharacterAvatarKey()))
		{
			avatar_ = string.Empty;
		}
	}

	protected override void OnLoad()
	{
		bitmap_ = PlayerPrefs.GetInt(SaveKeys.getOptionFlgDataKey());
		language_ = PlayerPrefs.GetString(SaveKeys.getOptionLanguageKey());
		avatar_ = PlayerPrefs.GetString(SaveKeys.getOptionCharacterAvatarKey());
		version_ = PlayerPrefs.GetString(SaveKeys.getOptionDataVersionKey());
		if (version_ != SaveData.Instance.getAppVersion())
		{
			language_ = Application.systemLanguage.ToString();
			version_ = SaveData.Instance.getAppVersion();
			OnSave();
			save();
		}
	}

	protected override void OnReset()
	{
		saveDefault();
		save();
	}

	protected override void OnSave()
	{
		PlayerPrefs.SetInt(SaveKeys.getOptionFlgDataKey(), bitmap_);
		PlayerPrefs.SetString(SaveKeys.getOptionLanguageKey(), language_);
		PlayerPrefs.SetString(SaveKeys.getOptionCharacterAvatarKey(), avatar_);
		PlayerPrefs.SetString(SaveKeys.getOptionDataVersionKey(), version_);
	}

	public bool getFlag(eFlag flagType)
	{
		return getFlag(bitmap_, (int)flagType);
	}

	public void setFlag(eFlag flagType, bool value)
	{
		setFlag(ref bitmap_, (int)flagType, value);
	}

	public void setLanguage(string language)
	{
		language_ = language;
	}

	public string getLanguage()
	{
		return language_;
	}

	public bool isKorean()
	{
		Debug.Log("Language = " + getLanguage());
		return true;
	}

	public void setAvatar(string avatar)
	{
		avatar_ = avatar;
	}

	public string getAvatar()
	{
		return avatar_;
	}

	public void PrefsDefault()
	{
		saveDefault();
		OnLoad();
	}

	public void PrefsDefaultException()
	{
		saveDefaultExceptionQuality();
		OnLoad();
	}

	public void avatarReset()
	{
		avatar_ = string.Empty;
	}

	private void saveDefault()
	{
		bitmap_ = 0;
		setFlag(eFlag.BGM, true);
		setFlag(eFlag.SE, true);
		setFlag(eFlag.PushNotice, true);
		setFlag(eFlag.HeartNotice, true);
		setFlag(eFlag.HighQuality, true);
		setFlag(eFlag.ShootButton, false);
		avatar_ = string.Empty;
		language_ = "Korean";
		PlayerPrefs.SetString(SaveKeys.getOptionCharacterAvatarKey(), avatar_);
		PlayerPrefs.SetString(SaveKeys.getOptionLanguageKey(), language_);
		PlayerPrefs.SetInt(SaveKeys.getOptionFlgDataKey(), bitmap_);
	}

	private void saveDefaultExceptionQuality()
	{
		bitmap_ = 0;
		setFlag(eFlag.BGM, true);
		setFlag(eFlag.SE, true);
		setFlag(eFlag.PushNotice, true);
		setFlag(eFlag.HeartNotice, true);
		setFlag(eFlag.ShootButton, false);
		avatar_ = string.Empty;
		language_ = "Korean";
		PlayerPrefs.SetString(SaveKeys.getOptionCharacterAvatarKey(), avatar_);
		PlayerPrefs.SetString(SaveKeys.getOptionLanguageKey(), language_);
		PlayerPrefs.SetInt(SaveKeys.getOptionFlgDataKey(), bitmap_);
	}
}
