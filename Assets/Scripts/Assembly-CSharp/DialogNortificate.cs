using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Network;
using UnityEngine;

public class DialogNortificate : DialogScrollListBase
{
	private const int MAX_SEND_NUM = 10;

	private const float OFFSET_Y = -1f;

	private const int ONE_PAGE_NUM = 4;

	private EventStageInfo eventData_;

	private List<long> sendIdList_ = new List<long>();

	public override void OnCreate()
	{
		base.OnCreate();
	}

	public new virtual void init(GameObject playerItem)
	{
		base.init(playerItem);
		createLine();
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		StageDataTable component = @object.GetComponent<StageDataTable>();
		eventData_ = component.getEventData();
		sendIdList_.Clear();
	}

	public virtual void setup(long[] sendIdList)
	{
		List<UserData> list = new List<UserData>();
		for (int i = 0; i < DummyPlayFriendData.FriendNum; i++)
		{
			UserData userData = DummyPlayFriendData.DummyFriends[i];
			if (isEventStagePlayable(userData.StageNo))
			{
				continue;
			}
			if (sendIdList != null)
			{
				bool flag = false;
				foreach (long num in sendIdList)
				{
					if (num == userData.ID)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
			}
			list.Add(userData);
		}
		for (int k = 0; k < 10; k++)
		{
			if (list.Count <= 0)
			{
				break;
			}
			UserData userData2 = null;
			UserDataObject userDataObject = null;
			System.Random random = new System.Random();
			int index = random.Next(list.Count - 1);
			userData2 = list[index];
			userDataObject = createItem(userData2).AddComponent<UserDataObject>();
			userDataObject.setData(userData2);
			itemList_.Add(userDataObject.gameObject);
			list.Remove(userData2);
			if (itemList_.Count >= 10)
			{
				break;
			}
		}
		if (itemList_.Count > 0)
		{
			addLine();
			for (int l = 0; l < itemList_.Count; l++)
			{
				GameObject gameObject = itemList_[l];
				UserData data = gameObject.GetComponent<UserDataObject>().getData();
				PlayerItemBase component = gameObject.gameObject.GetComponent<PlayerItemBase>();
				component.setup(data.UserName, data.ID, data.Mid);
				UILabel component2 = gameObject.transform.Find("name_Label").GetComponent<UILabel>();
				component2.text = data.UserName;
				string text = Constant.UserName.ReplaceOverStr(component2);
				component2.text = text;
				addItem(gameObject, l);
			}
			repositionItem();
			if (itemList_.Count <= 4)
			{
				dragPanel_.enabled = false;
				dragPanel_.gameObject.transform.localPosition = new Vector3(dragPanel_.gameObject.transform.localPosition.x, -1f, dragPanel_.gameObject.transform.localPosition.z);
			}
		}
	}

	protected virtual IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "InformButton":
		{
			Constant.SoundUtil.PlayDecideSE();
			PlayerItemBase item = trigger.transform.parent.GetComponent<PlayerItemBase>();
			yield return StartCoroutine(sendInformation(item));
			break;
		}
		case "allreceive_button":
			Constant.SoundUtil.PlayDecideSE();
			yield return StartCoroutine(sendInformationAll());
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayDecideSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			clear();
			break;
		}
	}

	public IEnumerator sendInformation(PlayerItemBase data)
	{
		int success_count = 0;
		int userCoin = GlobalData.Instance.getGameData().coin;
		Input.enable = false;
		yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
		Hashtable args = new Hashtable
		{
			{
				"eventNo",
				eventData_.EventNo.ToString()
			},
			{
				"toMemberNo",
				data.getID().ToString()
			}
		};
		NetworkMng.Instance.setup(args);
		yield return dialogManager_.StartCoroutine(NetworkMng.Instance.download(OnCreateInformationWWW, true, false));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			Input.enable = true;
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		EventMessageResponse res = JsonMapper.ToObject<EventMessageResponse>(www.text);
		GameData gameData = GlobalData.Instance.getGameData();
		int increased_coin = res.coin - gameData.coin;
		gameData.bonusJewel = res.bonusJewel;
		gameData.buyJewel = res.buyJewel;
		gameData.level = res.level;
		gameData.exp = res.exp;
		gameData.coin = res.coin;
		gameData.heart = res.heart;
		gameData.treasureboxNum = res.treasureboxNum;
		gameData.progressStageNo = res.progressStageNo;
		gameData.allStageScoreSum = res.allStageScoreSum;
		gameData.allStarSum = res.allStarSum;
		gameData.allPlayCount = res.allPlayCount;
		gameData.allClearCount = res.allClearCount;
		success_count = res.successCount;
		userCoin = gameData.coin;
		while (!TalkMessage.Instance.isReceived)
		{
			yield return null;
		}
		string[] presentMsg = TalkMessage.Instance.getMessage(TalkMessage.eType.EventSendInfo);
		if (eventData_.EventNo == 2)
		{
			presentMsg = TalkMessage.Instance.getMessage(TalkMessage.eType.ChallengeSendInfo);
		}
		for (int i = 0; i < presentMsg.Length; i++)
		{
			presentMsg[i] = presentMsg[i].Replace("{owner}", DummyPlayerData.Data.UserName);
		}
		DialogSendBase.SendAppLinkMessageCB sendAppLinkMessageCB = new DialogSendBase.SendAppLinkMessageCB();
		SNSCore.Instance.SendMessage(presentMsg[0], data.getMid(), sendAppLinkMessageCB.OnMessageSend);
		while (sendAppLinkMessageCB.result_ == -1)
		{
			yield return null;
		}
		yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		Input.enable = true;
		if (success_count > 0)
		{
			DialogCommon getDialog = dialogManager_.getDialog(DialogManager.eDialog.InviteSent) as DialogCommon;
			UISprite sprite = getDialog.transform.Find("window/chara/anm1").GetComponent<UISprite>();
			MessageResource msgRes = MessageResource.Instance;
			string msg3 = msgRes.getMessage(4000);
			sprite.spriteName = "UI_chara_00_008";
			msg3 = msgRes.castCtrlCode(msg3, 1, msgRes.getMessage(2568));
			msg3 = msgRes.castCtrlCode(msg3, 2, msgRes.castCtrlCode(msgRes.getMessage(31), 1, (success_count * 50).ToString("N0")));
			getDialog.setup(msg3, null, null, true);
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(getDialog));
			if (increased_coin < success_count * 50 && userCoin == Constant.CoinMax)
			{
				DialogLimitOver limitOverDialog = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
				yield return dialogManager_.StartCoroutine(limitOverDialog.show(Constant.eMoney.Coin));
			}
			while (getDialog.isOpen())
			{
				yield return null;
			}
		}
		data.setState(true);
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
		sendIdList_.Add(data.getID());
	}

	private WWW OnCreateInformationWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("eventNo", args["eventNo"]);
		WWWWrap.addGetParameter("toMemberNo", args["toMemberNo"]);
		return WWWWrap.create("message/event/");
	}

	public IEnumerator sendInformationAll()
	{
		disableAllreceiveButton();
		yield break;
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

	private WWW OnCreateInformationAllWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Post);
		string text = string.Empty;
		string param = eventData_.EventNo.ToString();
		foreach (GameObject item in itemList_)
		{
			long iD = item.GetComponent<UserDataObject>().getData().ID;
			if (!sendIdList_.Contains(iD))
			{
				if (text.Length > 0)
				{
					text += ",";
				}
				text += iD;
			}
		}
		WWWWrap.addGetParameter("eventNo", param);
		WWWWrap.addPostParameter("toMemberNos", text);
		return WWWWrap.create("message/event/");
	}

	private bool isEventStagePlayable(int stageNo)
	{
		return stageNo <= 18;
	}
}
