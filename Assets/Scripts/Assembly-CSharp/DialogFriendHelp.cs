using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Network;
using UnityEngine;

public class DialogFriendHelp : DialogScrollListBase
{
	private const int FRIENDUSER_SHOW_LIMIT = 10;

	private MessageResource msgResource_;

	private int numberOfStep;

	private int value;

	private MainMenu mainMenu_;

	private Transform checkTop;

	private Transform checkBottom;

	private int[] dayover_step = new int[4] { 0, 3, 7, 14 };

	public override void OnCreate()
	{
		base.OnCreate();
		msgResource_ = MessageResource.Instance;
		checkTop = base.transform.Find("checkTop");
		checkBottom = base.transform.Find("checkBottom");
	}

	public new virtual void init(GameObject playerItem)
	{
		base.init(playerItem);
		createLine();
		mainMenu_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
	}

	public virtual void setup()
	{
		int num = 0;
		for (int i = 0; i < DummyPlayFriendData.FriendNum; i++)
		{
			UserData userData = null;
			UserDataObject userDataObject = null;
			userData = DummyPlayFriendData.DummyFriends[i];
			if (userData.lastStageClearProgressDay == 0)
			{
				continue;
			}
			long unixTime = Utility.getUnixTime(DateTime.Now);
			long num2 = long.Parse(PlayerPrefs.GetString(Aes.EncryptString("HelpSend" + userData.ID, Aes.eEncodeType.Percent), "0"));
			if (unixTime - num2 < Constant.HelpResendTime)
			{
				continue;
			}
			if (PlayerPrefs.HasKey(Aes.EncryptString("HelpNoSend" + userData.ID, Aes.eEncodeType.Percent)))
			{
				if (unixTime - userData.lastUpdateTime >= Constant.CantHelpSendLoginTime)
				{
					continue;
				}
				num2 = long.Parse(PlayerPrefs.GetString(Aes.EncryptString("HelpNoSend" + userData.ID, Aes.eEncodeType.Percent), "0"));
				if (unixTime - num2 < Constant.HelpResendTimeAfterNotSend)
				{
					continue;
				}
			}
			userDataObject = createItem(DummyPlayFriendData.DummyFriends[i]).AddComponent<UserDataObject>();
			userDataObject.setData(userData);
			itemList_.Add(userDataObject.gameObject);
			if (++num < 10)
			{
				continue;
			}
			break;
		}
		addLine();
		numberOfStep = ActionReward.info_.maxLimit;
		value = GlobalData.Instance.getGameData().helpRewardCount;
		for (int j = 0; j < itemList_.Count; j++)
		{
			GameObject gameObject = itemList_[j];
			UserData data = gameObject.GetComponent<UserDataObject>().getData();
			PlayerItemBase component = gameObject.gameObject.GetComponent<PlayerItemBase>();
			component.setup(data.UserName, data.ID, data.Mid);
			component.setCoinBalloon(isDaillyFinish());
			component.SetCheckPosY(checkTop.position.y, checkBottom.position.y);
			string message = MessageResource.Instance.getMessage(4519);
			Transform transform = component.transform.Find("privilege_icon/Label");
			if (transform != null)
			{
				transform.GetComponent<UILabel>().text = message + ActionReward.info_.coin;
			}
			UILabel component2 = gameObject.transform.Find("UserName_Label").GetComponent<UILabel>();
			component2.text = data.UserName;
			string message2 = Constant.UserName.ReplaceOverStr(component2);
			component2.text = MessageResource.Instance.castCtrlCode(MessageResource.Instance.getMessage(2486), 1, message2);
			int num3 = data.lastStageClearProgressDay;
			if (num3 >= dayover_step[1] && num3 < dayover_step[2])
			{
				num3 = dayover_step[1];
			}
			else if (num3 >= dayover_step[2] && num3 < dayover_step[3])
			{
				num3 = dayover_step[2];
			}
			else if (num3 >= dayover_step[3])
			{
				num3 = dayover_step[3];
			}
			component2.text = MessageResource.Instance.castCtrlCode(component2.text, 2, num3.ToString());
			addItem(gameObject, j);
		}
		repositionItem();
		updateBonusDisp(base.transform.Find("bonus"));
	}

	protected virtual IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Item_Rescue_Button":
		{
			Constant.SoundUtil.PlayDecideSE();
			PlayerItemBase item = trigger.transform.parent.GetComponent<PlayerItemBase>();
			yield return StartCoroutine(sendHelpMail(item));
			break;
		}
		case "allreceive_button":
			Constant.SoundUtil.PlayDecideSE();
			yield return StartCoroutine(sendHelpMailAll());
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayDecideSE();
			foreach (GameObject obj2 in itemList_)
			{
				PlayerItemBase data2 = obj2.GetComponent<PlayerItemBase>();
				data2.setCoinBalloon(false);
				if (!data2.isFinish)
				{
					PlayerPrefs.SetString(Aes.EncryptString("HelpNoSend" + data2.getID(), Aes.eEncodeType.Percent), Utility.getUnixTime(DateTime.Now).ToString());
				}
			}
			PlayerPrefs.Save();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			clear();
			break;
		}
		foreach (GameObject obj in itemList_)
		{
			PlayerItemBase data = obj.GetComponent<PlayerItemBase>();
			if (!data.isFinish)
			{
				data.setCoinBalloon(isDaillyFinish());
			}
		}
	}

	public IEnumerator sendHelpMail(PlayerItemBase data)
	{
		Input.enable = false;
		yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
		List<long> members = new List<long> { data.getID() };
		NetworkMng.Instance.setup(Hash.PlayerStar(members.ToArray()));
		yield return dialogManager_.StartCoroutine(NetworkMng.Instance.download(API.PlayerStar, true, false));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW wwww = NetworkMng.Instance.getWWW();
			RankingData rankingData = JsonMapper.ToObject<RankingData>(wwww.text);
			wwww.Dispose();
			PlayerItemBase data2 = default(PlayerItemBase);
			RankingStarData stardata = rankingData.starList.SingleOrDefault((RankingStarData x) => x.memberNo == data2.getID());
			UserData user = DummyPlayFriendData.DummyFriends.SingleOrDefault((UserData x) => x.Mid == data2.getID().ToString());
			user.IsHeartRecvFlag = stardata.heartRecvFlg;
		}
		if (data.IsMessageBlock || KakaoCore.Instance.IsBlockMessage(data.getMid()))
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
			dialog.setMessage(MessageResource.Instance.getMessage(81));
			dialog.setup(null, null, true);
			dialog.setButtonText(DialogCommon.eText.Confirm);
			while (dialog.isOpen())
			{
				yield return 0;
			}
			PlayerPrefs.SetString(Aes.EncryptString("HelpSend" + data.getID(), Aes.eEncodeType.Percent), Utility.getUnixTime(DateTime.Now).ToString());
			PlayerPrefs.DeleteKey(Aes.EncryptString("HelpNoSend" + data.getID(), Aes.eEncodeType.Percent));
			PlayerPrefs.Save();
			data.setState(true);
			Input.enable = true;
			yield break;
		}
		Hashtable args = new Hashtable { 
		{
			"toMemberNo",
			data.getID().ToString()
		} };
		NetworkMng.Instance.setup(args);
		yield return dialogManager_.StartCoroutine(NetworkMng.Instance.download(OnCreateMailWWW, true, false));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			Input.enable = true;
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		SendHelpMailResponse res = JsonMapper.ToObject<SendHelpMailResponse>(www.text);
		GameData gameData = GlobalData.Instance.getGameData();
		int increased_coin = res.coin - gameData.coin;
		gameData.coin = res.coin;
		value = res.helpRewardCount;
		mainMenu_.update();
		Debug.Log("res.coin" + res.coin + "---res.addCoin" + res.addCoin + "---res.helpRewardCount" + res.helpRewardCount);
		while (!TalkMessage.Instance.isReceived)
		{
			yield return null;
		}
		string[] presentMsg = TalkMessage.Instance.getMessage(TalkMessage.eType.FriendHelp);
		for (int i = 0; i < presentMsg.Length; i++)
		{
			presentMsg[i] = presentMsg[i].Replace("{owner}", DummyPlayerData.Data.UserName);
		}
		List<string> receiverMidList = new List<string>(1) { data.getMid() };
		DialogSendBase.SendAppLinkMessageCB sendAppLinkMessageCB = new DialogSendBase.SendAppLinkMessageCB();
		SNSCore.Instance.KakaoSendMessageFriendHelp(data.getMid(), sendAppLinkMessageCB.OnMessageSend);
		while (sendAppLinkMessageCB.result_ == -1)
		{
			yield return null;
		}
		yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		Input.enable = true;
		PlayerPrefs.SetString(Aes.EncryptString("HelpSend" + data.getID(), Aes.eEncodeType.Percent), Utility.getUnixTime(DateTime.Now).ToString());
		PlayerPrefs.DeleteKey(Aes.EncryptString("HelpNoSend" + data.getID(), Aes.eEncodeType.Percent));
		PlayerPrefs.Save();
		data.setState(true);
		if (res.addCoin > 0)
		{
			DialogCommon getDialog = dialogManager_.getDialog(DialogManager.eDialog.InviteSent) as DialogCommon;
			UISprite sprite = getDialog.transform.Find("window/chara/anm1").GetComponent<UISprite>();
			MessageResource msgRes = MessageResource.Instance;
			string msg3 = msgRes.getMessage(4000);
			sprite.spriteName = "UI_chara_00_008";
			msg3 = msgRes.castCtrlCode(msg3, 1, msgRes.getMessage(2568));
			msg3 = msgRes.castCtrlCode(msg3, 2, msgRes.castCtrlCode(msgRes.getMessage(31), 1, res.addCoin.ToString("N0")));
			if (increased_coin < res.addCoin && gameData.coin == Constant.CoinMax)
			{
				DialogLimitOver limitOverDialog = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
				yield return dialogManager_.StartCoroutine(limitOverDialog.show(Constant.eMoney.Coin));
			}
			else
			{
				getDialog.setup(msg3, null, null, true);
				yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(getDialog));
			}
			while (getDialog.isOpen())
			{
				yield return null;
			}
		}
		bool bEnd = true;
		foreach (GameObject item in itemList_)
		{
			if (item.GetComponent<PlayerItemBase>().isFinish)
			{
				continue;
			}
			bEnd = false;
			break;
		}
		if (bEnd)
		{
			disableAllreceiveButton();
		}
		updateBonusDisp(base.transform.Find("bonus"));
	}

	private WWW OnCreateMailWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("toMemberNo", args["toMemberNo"]);
		return WWWWrap.create("mail/helpsend/");
	}

	public IEnumerator sendHelpMailAll()
	{
		Input.enable = false;
		yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
		NetworkMng.Instance.setup(null);
		yield return dialogManager_.StartCoroutine(NetworkMng.Instance.download(OnCreateMailAllWWW, true, false));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			Input.enable = true;
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		SendHelpMailAllResponse res = JsonMapper.ToObject<SendHelpMailAllResponse>(www.text);
		GameData gameData = GlobalData.Instance.getGameData();
		int increased_coin = res.coin - gameData.coin;
		gameData.coin = res.coin;
		value = res.helpRewardCount;
		mainMenu_.update();
		while (!TalkMessage.Instance.isReceived)
		{
			yield return null;
		}
		string[] presentMsg = TalkMessage.Instance.getMessage(TalkMessage.eType.FriendHelp);
		for (int i = 0; i < presentMsg.Length; i++)
		{
			presentMsg[i] = presentMsg[i].Replace("{owner}", DummyPlayerData.Data.UserName);
		}
		List<string> receiverMidList = new List<string>(1);
		foreach (GameObject item2 in itemList_)
		{
			PlayerItemBase data2 = item2.GetComponent<PlayerItemBase>();
			long[] successList = res.successList;
			foreach (long memberNo in successList)
			{
				if (memberNo == data2.getID())
				{
					receiverMidList.Add(data2.getMid());
					break;
				}
			}
		}
		DialogSendBase.SendAppLinkMessageCB sendAppLinkMessageCB = new DialogSendBase.SendAppLinkMessageCB();
		yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		Input.enable = true;
		foreach (GameObject item in itemList_)
		{
			PlayerItemBase data = item.GetComponent<PlayerItemBase>();
			PlayerPrefs.SetString(Aes.EncryptString("HelpSend" + data.getID(), Aes.eEncodeType.Percent), Utility.getUnixTime(DateTime.Now).ToString());
			PlayerPrefs.DeleteKey(Aes.EncryptString("HelpNoSend" + data.getID(), Aes.eEncodeType.Percent));
			data.setState(true);
		}
		PlayerPrefs.Save();
		if (res.addCoin > 0)
		{
			DialogCommon getDialog = dialogManager_.getDialog(DialogManager.eDialog.InviteSent) as DialogCommon;
			UISprite sprite = getDialog.transform.Find("window/chara/anm1").GetComponent<UISprite>();
			MessageResource msgRes = MessageResource.Instance;
			string msg3 = msgRes.getMessage(4000);
			sprite.spriteName = "UI_chara_00_008";
			msg3 = msgRes.castCtrlCode(msg3, 1, msgRes.getMessage(2568));
			msg3 = msgRes.castCtrlCode(msg3, 2, msgRes.castCtrlCode(msgRes.getMessage(31), 1, res.addCoin.ToString("N0")));
			if (increased_coin < res.addCoin && gameData.coin == Constant.CoinMax)
			{
				DialogLimitOver limitOverDialog = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
				yield return dialogManager_.StartCoroutine(limitOverDialog.show(Constant.eMoney.Coin));
			}
			else
			{
				getDialog.setup(msg3, null, null, true);
				yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(getDialog));
			}
			while (getDialog.isOpen())
			{
				yield return null;
			}
		}
		disableAllreceiveButton();
		updateBonusDisp(base.transform.Find("bonus"));
	}

	private void disableAllreceiveButton()
	{
		UIButton component = base.transform.Find("window/allreceive_button").GetComponent<UIButton>();
		component.setEnable(false);
		UIButtonColor[] components = component.GetComponents<UIButtonColor>();
		UIButtonColor[] array = components;
		foreach (UIButtonColor uIButtonColor in array)
		{
			if (!(uIButtonColor.tweenTarget.name != "Label"))
			{
				uIButtonColor.enabled = false;
				break;
			}
		}
		component.transform.Find("Label").GetComponent<UILabel>().color = component.pressed;
	}

	private WWW OnCreateMailAllWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Post);
		string text = string.Empty;
		foreach (GameObject item in itemList_)
		{
			if (text.Length > 0)
			{
				text += ",";
			}
			text += item.GetComponent<UserDataObject>().getData().ID;
		}
		WWWWrap.addPostParameter("toMemberNos", text);
		return WWWWrap.create("mail/helpsend/");
	}

	private bool isDaillyFinish()
	{
		return numberOfStep != value;
	}

	private void updateBonusDisp(Transform bonus)
	{
		UILabel component = bonus.Find("gauge/Label").GetComponent<UILabel>();
		string message = msgResource_.getMessage(2505);
		message = msgResource_.castCtrlCode(message, 1, numberOfStep.ToString());
		message = msgResource_.castCtrlCode(message, 2, value.ToString());
		component.text = message;
		UISlider component2 = bonus.Find("gauge/Progress Bar").GetComponent<UISlider>();
		component2.numberOfSteps = numberOfStep;
		component2.sliderValue = (float)value / (float)numberOfStep;
		component2.ForceUpdate();
	}
}
