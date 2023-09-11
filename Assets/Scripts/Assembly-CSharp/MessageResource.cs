using System.Collections.Generic;
using UnityEngine;

public class MessageResource : MonoBehaviour
{
	private static string CTRL_CODE = "CTRL";

	private static MessageResource instance_;

	private Dictionary<int, string> msgDicJp_ = new Dictionary<int, string>();

	private Dictionary<int, string> msgDicEn_ = new Dictionary<int, string>();

	private Dictionary<int, string> msgDicKr_ = new Dictionary<int, string>();

	private Dictionary<int, string> msgDic_;

	private bool bLoaded_;

	public static MessageResource Instance
	{
		get
		{
			return instance_;
		}
	}

	private void Awake()
	{
		if (instance_ == null)
		{
			instance_ = this;
			Object.DontDestroyOnLoad(this);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		instance_ = null;
	}

	public string getMessage(int messageID)
	{
		if (!bLoaded_)
		{
			return string.Empty;
		}
		if (msgDic_.ContainsKey(messageID))
		{
			return msgDic_[messageID];
		}
		return string.Empty;
	}

	public string getMessageDeviceLanguage(int messageID)
	{
		if (!bLoaded_)
		{
			return string.Empty;
		}
		if (msgDicKr_.ContainsKey(messageID))
		{
			return msgDicKr_[messageID];
		}
		if (Application.systemLanguage.ToString() == SystemLanguage.Japanese.ToString())
		{
			if (msgDicJp_.ContainsKey(messageID))
			{
				return msgDicJp_[messageID];
			}
		}
		else if (msgDicEn_.ContainsKey(messageID))
		{
			return msgDicEn_[messageID];
		}
		return string.Empty;
	}

	public string castCtrlCode(string src, int index, string message)
	{
		if (!isCtrlCode(src, index))
		{
			return src;
		}
		string ctrlCode = getCtrlCode(index);
		return src.Replace(ctrlCode, message);
	}

	public bool isCtrlCode(string src, int index)
	{
		return src.IndexOf(getCtrlCode(index)) > -1;
	}

	private string getCtrlCode(int index)
	{
		return "%" + CTRL_CODE + index.ToString("D2") + "%";
	}

	public int getMessageID(string message)
	{
		if (!bLoaded_)
		{
			return -1;
		}
		foreach (int key in msgDic_.Keys)
		{
			if (msgDic_[key] == message)
			{
				return key;
			}
		}
		return -1;
	}

	public bool isExistMessageID(int messageID)
	{
		if (msgDic_.ContainsKey(messageID))
		{
			return true;
		}
		return false;
	}

	public void load()
	{
		if (!bLoaded_)
		{
			load(eLanguageType.Japanese);
			load(eLanguageType.English);
			load(eLanguageType.Korean);
			SaveOptionData optionData = SaveData.Instance.getSystemData().getOptionData();
			if (optionData.isKorean())
			{
				msgDic_ = msgDicKr_;
			}
			else
			{
				msgDic_ = msgDicEn_;
			}
			bLoaded_ = true;
		}
	}

	private void load(eLanguageType type)
	{
		TextAsset textAsset;
		Dictionary<int, string> dictionary;
		switch (type)
		{
		case eLanguageType.Japanese:
			textAsset = Object.Instantiate(Resources.Load("Parameter/Message/message_JPN", typeof(TextAsset))) as TextAsset;
			dictionary = msgDicJp_;
			break;
		case eLanguageType.English:
			textAsset = Object.Instantiate(Resources.Load("Parameter/Message/message_ENG", typeof(TextAsset))) as TextAsset;
			dictionary = msgDicEn_;
			break;
		case eLanguageType.Korean:
			textAsset = Object.Instantiate(Resources.Load("Parameter/Message/message_KOR", typeof(TextAsset))) as TextAsset;
			dictionary = msgDicKr_;
			break;
		default:
			Debug.LogError(type);
			return;
		}
		string[] array = textAsset.text.Split('\n');
		foreach (string text in array)
		{
			string[] array2 = text.Split('|');
			if (array2.Length < 2)
			{
				continue;
			}
			int key = int.Parse(array2[0]);
			string text2 = array2[1];
			if (array2.Length > 2)
			{
				for (int j = 2; j < array2.Length; j++)
				{
					text2 += "\n";
					text2 += array2[j];
				}
			}
			text2 = removeNewLine(text2);
			text2 = text2.Substring(0, text2.Length - 1);
			dictionary[key] = text2;
		}
	}

	public string AddLine(string line)
	{
		string[] array = line.Split('|');
		if (array.Length < 2)
		{
			return line;
		}
		string text = array[0];
		for (int i = 1; i < array.Length; i++)
		{
			text += "\n";
			text += array[i];
		}
		text = removeNewLine(text);
		return text.Substring(0, text.Length - 1);
	}

	private string removeNewLine(string message)
	{
		string text = message;
		int num = text.LastIndexOf("\n");
		if (num != -1 && text.Length - 1 == num)
		{
			text = text.Remove(num, 1);
		}
		return text;
	}
}
