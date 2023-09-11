using UnityEngine;

[AddComponentMenu("NGUI/UI/Input (Saved)")]
public class UIInputSaved : UIInput
{
	public string playerPrefsField;

	private void Awake()
	{
		onSubmit = SaveToPlayerPrefs;
		if (!string.IsNullOrEmpty(playerPrefsField) && PlayerPrefs.HasKey(playerPrefsField))
		{
			base.text = PlayerPrefs.GetString(playerPrefsField);
		}
	}

	private void SaveToPlayerPrefs(string val)
	{
		if (!string.IsNullOrEmpty(playerPrefsField))
		{
			PlayerPrefs.SetString(playerPrefsField, val);
		}
	}

	private void OnApplicationQuit()
	{
		SaveToPlayerPrefs(base.text);
	}
}
