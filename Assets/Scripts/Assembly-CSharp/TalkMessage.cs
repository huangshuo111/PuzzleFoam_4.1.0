using System.Collections;
using System.Collections.Generic;
using LitJson;
using Network;
using UnityEngine;

public class TalkMessage : MonoBehaviour
{
	public enum eType
	{
		Invitation = 10,
		Present = 11,
		Request = 12,
		RankingChange = 13,
		FriendHelp = 14,
		Title = 20,
		Text = 21,
		EventSendInfo = 30,
		EventRankingChange = 31,
		ChallengeSendInfo = 40,
		GachaInfo = 50,
		CollaboRankingChange = 60,
		ChallengeRankingChange = 65,
		ParkSendNice = 70,
		ParkExpeditionRankingChange = 71,
		TipsStart = 1000,
		TipsEnd = 9999,
		OpenStarDoor = 10000,
		LevelUPTitle = 100001,
		LevelUPText = 100002,
		LevelUPText2 = 100003,
		AreaClearTitle = 100004,
		AreaClearText = 100005,
		AreaClearText2 = 100006,
		HighScoreTitle = 100007,
		HighScoreText = 100008,
		HighScoreText2 = 100009
	}

	private static TalkMessage instance;

	private bool bReceived_;

	private Message[] messageList_;

	private List<string> tipsMessageList;

	public static TalkMessage Instance
	{
		get
		{
			return instance;
		}
	}

	public bool isReceived
	{
		get
		{
			return bReceived_;
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Object.Destroy(this);
		}
	}

	public IEnumerator updateRoutine()
	{
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, false, false));
		WWW www = NetworkMng.Instance.getWWW();
		messageList_ = JsonMapper.ToObject<MessageList>(www.text).messageList;
		bReceived_ = true;
	}

	private WWW OnCreateWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		return WWWWrap.create("message/list/");
	}

	public string[] getMessage(eType type)
	{
		return getMessage(type, true);
	}

	public string[] getMessage(eType type, bool bReplace)
	{
		string[] array = new string[2]
		{
			string.Empty,
			string.Empty
		};
		Message[] array2 = messageList_;
		foreach (Message message in array2)
		{
			if (message.triggerNo != (int)type)
			{
				continue;
			}
			bool flag = ResourceLoader.Instance.isJapanResource();
			if (!bReplace)
			{
				flag = Application.systemLanguage.ToString() == SystemLanguage.Japanese.ToString();
			}
			if (flag)
			{
				if (bReplace)
				{
					array[0] = message.messageTextJa32.Replace("\r\n", "<br>");
					array[1] = message.messageTextJa31;
				}
				else
				{
					array[0] = message.messageTextJa32;
					array[1] = message.messageTextJa31;
				}
			}
			else if (bReplace)
			{
				array[0] = message.messageTextEn32.Replace("\r\n", "<br>");
				array[1] = message.messageTextEn31;
			}
			else
			{
				array[0] = message.messageTextEn32;
				array[1] = message.messageTextEn31;
			}
			break;
		}
		return array;
	}

	public string getTipsMessage()
	{
		if (messageList_ == null)
		{
			return string.Empty;
		}
		if (tipsMessageList == null)
		{
			tipsMessageList = new List<string>();
			bool flag = ResourceLoader.Instance.isJapanResource();
			Message[] array = messageList_;
			foreach (Message message in array)
			{
				if (message.triggerNo >= 1000 && message.triggerNo <= 9999)
				{
					if (flag)
					{
						tipsMessageList.Add(message.messageTextJa32);
					}
					else
					{
						tipsMessageList.Add(message.messageTextEn32);
					}
				}
			}
		}
		if (tipsMessageList.Count == 0)
		{
			return string.Empty;
		}
		return tipsMessageList[Random.Range(0, tipsMessageList.Count)];
	}
}
