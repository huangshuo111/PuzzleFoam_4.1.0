using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using UnityEngine;

public class DialogInvite : DialogScrollListBase
{
	protected enum eDialog
	{
		None = -1,
		Confirm = 0,
		Sent = 1,
		Max = 2
	}

	private DialogCommon confirmDialog_;

	private DialogCommon sentDialog_;

	private MainMenu mainMenu_;

	private eDialog currentDialog_ = eDialog.None;

	private PlayerItemBase selectItem_;

	private MessageResource msgResource_;

	private InvitationDataTable dataTable_;

	private string rewardMsg_;

	private string rewardMsg2_;

	private int currentPeopleNum_;

	private Color defaultBgColor;

	private Color currentBgColor;

	private bool bAddItemFlag = true;

	private InvitationInfo.BonusInfo currentBonusInfo_;

	public override void OnCreate()
	{
		base.OnCreate();
		mainMenu_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		msgResource_ = MessageResource.Instance;
		dataTable_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<InvitationDataTable>();
		Transform transform = base.transform.Find("bonus");
		currentBgColor = transform.Find("1/bg").GetComponent<UISprite>().color;
		defaultBgColor = transform.Find("2/bg").GetComponent<UISprite>().color;
	}

	public override void init(GameObject item)
	{
		base.init(item);
		createLine(-40f);
		confirmDialog_ = dialogManager_.getDialog(DialogManager.eDialog.InviteConfirm) as DialogCommon;
		sentDialog_ = dialogManager_.getDialog(DialogManager.eDialog.InviteSent) as DialogCommon;
	}

	public IEnumerator show()
	{
		Input.enable = false;
		yield return dialogManager_.StartCoroutine(NetworkMng.Instance.showIcon(true));
		currentPeopleNum_ = 9;
		List<UserData> notInvitedList = new List<UserData>();
		List<string> invitedList = new List<string>();
		InvitationListData invitationListData2 = null;
		NetworkMng.Instance.setup(null);
		yield return dialogManager_.StartCoroutine(NetworkMng.Instance.download(OnCreateWWW_List, true, false));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			invitationListData2 = JsonMapper.ToObject<InvitationListData>(www.text);
			currentPeopleNum_ = invitationListData2.invitationSize;
			dataTable_.info_.BonusInfos = new InvitationInfo.BonusInfo[invitationListData2.bonusList.Length];
			for (int j = 0; j < invitationListData2.bonusList.Length; j++)
			{
				dataTable_.info_.BonusInfos[j] = new InvitationInfo.BonusInfo();
				dataTable_.info_.BonusInfos[j].PeopleNum = invitationListData2.bonusList[j].peopleNum;
				dataTable_.info_.BonusInfos[j].RewardType = invitationListData2.bonusList[j].rewardType;
				dataTable_.info_.BonusInfos[j].Reward = invitationListData2.bonusList[j].reward;
			}
			InvitationData[] list = invitationListData2.invitationList;
			if (list != null)
			{
				InvitationData[] array = list;
				foreach (InvitationData data2 in array)
				{
					invitedList.Add(data2.toMid);
				}
			}
			if (GlobalData.Instance.getGameData().iosInvite && DummyLineFriendData.FriendNum > 0)
			{
				UserData[] dummyFriends = DummyLineFriendData.DummyFriends;
				foreach (UserData data in dummyFriends)
				{
					bool bInvited = false;
					foreach (string mid in invitedList)
					{
						if (data.Mid != mid)
						{
							continue;
						}
						bInvited = true;
						break;
					}
					if (!bInvited)
					{
						notInvitedList.Add(data);
					}
				}
			}
			bool isFriend = notInvitedList.Count > 0;
			Transform Period_Label = base.transform.Find("window/Period_Label");
			Object.Destroy(Period_Label.GetComponent<SimpleMessageDraw>());
			if (GlobalData.Instance.getGameData().iosInvite)
			{
				string msg5 = msgResource_.getMessage(1412);
				Constant.Reward reward = new Constant.Reward();
				Constant.Reward reward2 = new Constant.Reward();
				dataTable_.getReward(ref reward, ref reward2);
				rewardMsg_ = string.Format("{0:#,0}", reward.Num);
				rewardMsg2_ = string.Format("{0:#,0}", reward2.Num);
				Period_Label.GetComponent<UILabel>().text = msgResource_.castCtrlCode(msg5, 1, rewardMsg_);
			}
			else
			{
				string msg4 = msgResource_.getMessage(500014);
				Period_Label.GetComponent<UILabel>().text = msg4;
			}
			Transform bonus = base.transform.Find("bonus");
			if (!isFriend)
			{
				DialogConfirm dialog2 = dialogManager_.getDialog(DialogManager.eDialog.InviteNone) as DialogConfirm;
				Transform Dialog1_Label = dialog2.transform.Find("window/Dialog1_Label");
				Object.Destroy(Dialog1_Label.GetComponent<SimpleMessageDraw>());
				if (GlobalData.Instance.getGameData().iosInvite)
				{
					string msg3 = msgResource_.getMessage(1410);
					Dialog1_Label.GetComponent<UILabel>().text = msgResource_.castCtrlCode(msg3, 1, rewardMsg_);
				}
				else
				{
					string msg2 = msgResource_.getMessage(500014);
					Dialog1_Label.GetComponent<UILabel>().text = msg2;
				}
				bonus = dialog2.transform.Find("bonus");
			}
			if (GlobalData.Instance.getGameData().iosInvite)
			{
				for (int i = 0; i < dataTable_.info_.BonusInfos.Length; i++)
				{
					InvitationInfo.BonusInfo bonusInfo = dataTable_.info_.BonusInfos[i];
					Transform bonusTrans = bonus.Find((i + 1).ToString());
					bonusTrans.Find("number/Label").GetComponent<UILabel>().text = msgResource_.getMessage(500010);
					bonusTrans.Find("number/Label_number").GetComponent<UILabel>().text = bonusInfo.PeopleNum.ToString();
					string msg = string.Format("{0:#,0}", bonusInfo.Reward);
					if (!ResourceLoader.Instance.isJapanResource())
					{
						msg += " ";
					}
					switch ((Constant.eMoney)bonusInfo.RewardType)
					{
					case Constant.eMoney.Jewel:
						msg += msgResource_.getMessage(2569);
						break;
					case Constant.eMoney.Coin:
						msg += msgResource_.getMessage(2568);
						break;
					case Constant.eMoney.Heart:
						msg += msgResource_.getMessage(2570);
						break;
					}
					bonusTrans.Find("Label2").GetComponent<UILabel>().text = msg;
					bonusTrans.Find("icon_coin").gameObject.SetActive(bonusInfo.RewardType == 1);
					bonusTrans.Find("icon_jewel").gameObject.SetActive(bonusInfo.RewardType == 2);
					bonusTrans.Find("icon_heart").gameObject.SetActive(bonusInfo.RewardType == 3);
				}
				updateBonusDisp(bonus);
			}
			if (!isFriend)
			{
				DialogConfirm dialog = dialogManager_.getDialog(DialogManager.eDialog.InviteNone) as DialogConfirm;
				yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
				Input.enable = true;
				yield return dialogManager_.StartCoroutine(NetworkMng.Instance.showIcon(false));
				yield break;
			}
			bAddItemFlag = true;
			dialogManager_.StartCoroutine(AddInviteItem(notInvitedList));
			while (bAddItemFlag)
			{
				yield return null;
			}
			Input.enable = true;
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
			yield return dialogManager_.StartCoroutine(NetworkMng.Instance.showIcon(false));
		}
		else
		{
			Input.enable = true;
			yield return dialogManager_.StartCoroutine(NetworkMng.Instance.showIcon(false));
		}
	}

	private IEnumerator AddInviteItem()
	{
		yield return dialogManager_.StartCoroutine(AddInviteItem(new List<UserData>(DummyLineFriendData.DummyFriends)));
	}

	private IEnumerator AddInviteItem(List<UserData> notinvitelist)
	{
		addLine();
		bool notinvite = false;
		int i = 0;
		UserData[] dummyFriends = DummyLineFriendData.DummyFriends;
		foreach (UserData data in dummyFriends)
		{
			if (!bAddItemFlag)
			{
				yield break;
			}
			GameObject item = createItem(data);
			addItem(item, i++);
			UserDataObject obj = item.AddComponent<UserDataObject>();
			obj.setData(data);
			itemList_.Add(item);
			notinvite = notinvitelist.SingleOrDefault((UserData x) => x.ID == data.ID) == null;
			if (GlobalData.Instance.getGameData().iosInvite)
			{
				item.transform.Find("InviteButton").gameObject.SetActive(true);
				item.transform.Find("InviteIOSButton").gameObject.SetActive(false);
				string rewardString = msgResource_.castCtrlCode(msgResource_.getMessage(45), 1, rewardMsg_);
				string rewardString2 = msgResource_.castCtrlCode(msgResource_.getMessage(45), 1, rewardMsg2_);
				item.transform.Find("InviteButton/Coin_Label").GetComponent<UILabel>().text = rewardString;
				item.transform.Find("InviteButton/Heart_Label").GetComponent<UILabel>().text = rewardString2;
				PlayerItemBase playeritem2 = item.GetComponent<PlayerItemBase>();
				playeritem2.setup(data.UserName, data.ID, data.Mid);
				playeritem2.setState(notinvite);
			}
			else
			{
				item.transform.Find("InviteIOSButton").gameObject.SetActive(!notinvite);
				PlayerItemBase playeritem = item.GetComponent<PlayerItemBase>();
				playeritem.setup(data.UserName, data.ID, data.Mid);
				playeritem.setState(notinvite);
				item.transform.Find("InviteButton").gameObject.SetActive(false);
			}
			if (itemList_.Count >= 10)
			{
				yield return new WaitForSeconds(0.01f);
			}
			else
			{
				yield return null;
			}
		}
		repositionItem();
		dragPanel_.ResetPosition();
		bAddItemFlag = false;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "InviteButton":
		case "InviteIOSButton":
		{
			Constant.SoundUtil.PlayDecideSE();
			selectItem_ = trigger.transform.parent.GetComponent<PlayerItemBase>();
			string msg2 = msgResource_.getMessage(16);
			msg2 = msgResource_.castCtrlCode(msg2, 1, selectItem_.getUserName());
			yield return dialogManager_.StartCoroutine(openCommonDialog(msg2, eDialog.Confirm));
			break;
		}
		case "Close_Button":
			if (bAddItemFlag)
			{
				bAddItemFlag = false;
			}
			Constant.SoundUtil.PlayCancelSE();
			clear();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	private IEnumerator OnCancel()
	{
		yield return dialogManager_.StartCoroutine(closeCommonDialog());
	}

	private IEnumerator OnDecide()
	{
		if (currentDialog_ == eDialog.Confirm)
		{
			yield return dialogManager_.StartCoroutine(closeCommonDialog());
			int gainCoin2 = 1000;
			Constant.Reward reward = new Constant.Reward();
			Constant.Reward reward2 = new Constant.Reward();
			dataTable_.getReward(ref reward, ref reward2);
			gainCoin2 = reward.Num;
			int prevPeopleNum = currentPeopleNum_;
			int current = 0;
			int max = 0;
			bool bRewardLimitOver3 = false;
			bool bRewardLimitOver6 = false;
			bool bBonusLimitOver3 = false;
			if (GlobalData.Instance.getGameData().iosInvite)
			{
				switch (reward.RewardType)
				{
				case Constant.eMoney.Coin:
					current = Bridge.PlayerData.getCoin();
					max = Constant.CoinMax;
					break;
				case Constant.eMoney.Heart:
					current = Bridge.PlayerData.getHeart();
					max = Constant.HeartMax;
					break;
				case Constant.eMoney.Jewel:
					current = Bridge.PlayerData.getJewel();
					max = Constant.JewelMax;
					break;
				}
				bRewardLimitOver3 = false;
				bRewardLimitOver3 = current + reward.Num > max;
				bRewardLimitOver6 = false;
				switch (reward2.RewardType)
				{
				case Constant.eMoney.Coin:
					current = Bridge.PlayerData.getCoin();
					max = Constant.CoinMax;
					break;
				case Constant.eMoney.Heart:
					current = Bridge.PlayerData.getHeart();
					max = Constant.HeartMax;
					break;
				case Constant.eMoney.Jewel:
					current = Bridge.PlayerData.getJewel();
					max = Constant.JewelMax;
					break;
				}
				bRewardLimitOver6 = current + reward2.Num > max;
				bBonusLimitOver3 = false;
				switch ((Constant.eMoney)currentBonusInfo_.RewardType)
				{
				case Constant.eMoney.Coin:
					current = Bridge.PlayerData.getCoin();
					max = Constant.CoinMax;
					break;
				case Constant.eMoney.Heart:
					current = Bridge.PlayerData.getHeart();
					max = Constant.HeartMax;
					break;
				case Constant.eMoney.Jewel:
					current = Bridge.PlayerData.getJewel();
					max = Constant.JewelMax;
					break;
				}
				bBonusLimitOver3 = current + currentBonusInfo_.Reward > max;
			}
			else
			{
				bRewardLimitOver3 = false;
				bRewardLimitOver6 = false;
				bBonusLimitOver3 = false;
			}
			bool bCancel = false;
			Input.enable = false;
			yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
			while (!TalkMessage.Instance.isReceived)
			{
				yield return null;
			}
			if (!SNSCore.Instance.IsSupportedDevice(selectItem_.getMid()))
			{
				yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
				Input.enable = true;
				DialogCommon unsupported_dialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
				unsupported_dialog.setMessage(msgResource_.getMessage(308));
				unsupported_dialog.setup(null, null, true);
				unsupported_dialog.setButtonText(DialogCommon.eText.Confirm);
				yield return StartCoroutine(dialogManager_.openDialog(unsupported_dialog));
				while (unsupported_dialog.isOpen())
				{
					yield return 0;
				}
				yield break;
			}
			string[] inviteMsg = TalkMessage.Instance.getMessage(TalkMessage.eType.Invitation);
			if (inviteMsg.Length >= 2)
			{
				List<string> receiverMidList = new List<string>(1) { selectItem_.getMid() };
				DialogSendBase.SendAppLinkMessageCB sendAppLinkMessageCB = new DialogSendBase.SendAppLinkMessageCB();
				SNSCore.Instance.SendInviteMessage(selectItem_.getMid(), sendAppLinkMessageCB.OnMessageSend);
				while (sendAppLinkMessageCB.result_ == -1)
				{
					yield return null;
				}
				yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
				Input.enable = true;
				if (sendAppLinkMessageCB.result_ != 0)
				{
					if (sendAppLinkMessageCB.result_ == -32)
					{
						DialogCommon dialog3 = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
						dialog3.setMessage(msgResource_.getMessage(309));
						dialog3.setup(null, null, true);
						dialog3.setButtonText(DialogCommon.eText.Confirm);
						yield return StartCoroutine(dialogManager_.openDialog(dialog3));
						while (dialog3.isOpen())
						{
							yield return 0;
						}
					}
					else if (sendAppLinkMessageCB.result_ == -17 || sendAppLinkMessageCB.result_ == -16 || sendAppLinkMessageCB.result_ == -14)
					{
						DialogCommon dialog3 = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
						dialog3.setMessage(msgResource_.getMessage(81));
						dialog3.setup(null, null, true);
						dialog3.setButtonText(DialogCommon.eText.Confirm);
						yield return StartCoroutine(dialogManager_.openDialog(dialog3));
						while (dialog3.isOpen())
						{
							yield return 0;
						}
					}
					else if (sendAppLinkMessageCB.result_ == -31)
					{
						DialogCommon dialog3 = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
						dialog3.setMessage(msgResource_.getMessage(1411));
						dialog3.setup(null, null, true);
						dialog3.setButtonText(DialogCommon.eText.Confirm);
						yield return StartCoroutine(dialogManager_.openDialog(dialog3));
						while (dialog3.isOpen())
						{
							yield return 0;
						}
					}
					else
					{
						yield return StartCoroutine(NetworkMng.Instance.openErrorDialog(false, eResultCode.ErrorUnknown));
					}
				}
				else
				{
					if (bCancel)
					{
						yield break;
					}
					NetworkMng.Instance.setup(null);
					yield return StartCoroutine(NetworkMng.Instance.download(OnCreateWWW_To, true));
					if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
					{
						yield break;
					}
					WWW www = NetworkMng.Instance.getWWW();
					InvitationResultData resultData = JsonMapper.ToObject<InvitationResultData>(www.text);
					GameData gameData = GlobalData.Instance.getGameData();
					gameData.coin = resultData.coin;
					gameData.bonusJewel = resultData.bonusJewel;
					gameData.buyJewel = resultData.buyJewel;
					gameData.heart = resultData.heart;
					currentPeopleNum_ = resultData.invitationSize;
					mainMenu_.update();
					InvitationInfo.BonusInfo prevBonusInfo = currentBonusInfo_;
					updateBonusDisp(base.transform.Find("bonus"));
					string msg10 = msgResource_.getMessage(17);
					if (GlobalData.Instance.getGameData().iosInvite)
					{
						msg10 = msgResource_.getMessage(17);
						msg10 = msgResource_.castCtrlCode(msg10, 1, selectItem_.getUserName());
						msg10 = msgResource_.castCtrlCode(msg10, 2, rewardMsg_);
						Tapjoy.TrackEvent("Money", "Income Coin", "Invite", null, reward.Num);
						GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Invite", reward.Num);
					}
					else
					{
						msg10 = msgResource_.getMessage(500015);
						msg10 = msgResource_.castCtrlCode(msg10, 1, selectItem_.getUserName());
					}
					setSentDialogIcon(reward.RewardType);
					yield return dialogManager_.StartCoroutine(openCommonDialog(msg10, eDialog.Sent));
					if (bRewardLimitOver3)
					{
						DialogLimitOver limitOverDialog3 = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
						yield return StartCoroutine(limitOverDialog3.show(reward.RewardType));
					}
					if (bRewardLimitOver6)
					{
						DialogLimitOver limitOverDialog2 = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
						while (limitOverDialog2.isOpen())
						{
							yield return null;
						}
						yield return StartCoroutine(limitOverDialog2.show(reward2.RewardType));
					}
					InvitationInfo.BonusInfo[] BonusInfos = dataTable_.info_.BonusInfos;
					int maxPeopleNum = BonusInfos[BonusInfos.Length - 1].PeopleNum;
					int peopleNum = currentPeopleNum_;
					if (prevPeopleNum > maxPeopleNum)
					{
						prevPeopleNum = maxPeopleNum;
					}
					if (peopleNum > maxPeopleNum)
					{
						peopleNum = maxPeopleNum;
					}
					if (GlobalData.Instance.getGameData().iosInvite && prevPeopleNum != peopleNum && prevBonusInfo.PeopleNum == currentPeopleNum_)
					{
						while (sentDialog_.isOpen())
						{
							yield return 0;
						}
						msg10 = msgResource_.getMessage(2560);
						msg10 = msgResource_.castCtrlCode(msg10, 1, prevBonusInfo.PeopleNum.ToString());
						switch ((Constant.eMoney)prevBonusInfo.RewardType)
						{
						case Constant.eMoney.Jewel:
							msg10 = msgResource_.castCtrlCode(msg10, 2, msgResource_.getMessage(2569));
							msg10 = msgResource_.castCtrlCode(msg10, 3, msgResource_.castCtrlCode(msgResource_.getMessage(28), 1, string.Format("{0:#,0}", prevBonusInfo.Reward)));
							Tapjoy.TrackEvent("Money", "Income Jewel", "Invitations", null, prevBonusInfo.Reward);
							GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Jewel", "Invitations", prevBonusInfo.Reward);
							break;
						case Constant.eMoney.Coin:
							msg10 = msgResource_.castCtrlCode(msg10, 2, msgResource_.getMessage(2568));
							msg10 = msgResource_.castCtrlCode(msg10, 3, msgResource_.castCtrlCode(msgResource_.getMessage(31), 1, string.Format("{0:#,0}", prevBonusInfo.Reward)));
							Tapjoy.TrackEvent("Money", "Income Coin", "Invitations", null, prevBonusInfo.Reward);
							GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Invitations", prevBonusInfo.Reward);
							break;
						case Constant.eMoney.Heart:
							msg10 = msgResource_.castCtrlCode(msg10, 2, msgResource_.getMessage(2570));
							msg10 = msgResource_.castCtrlCode(msg10, 3, msgResource_.castCtrlCode(msgResource_.getMessage(28), 1, string.Format("{0:#,0}", prevBonusInfo.Reward)));
							break;
						}
						Sound.Instance.pauseBgm(true);
						Sound.Instance.playSe(Sound.eSe.SE_242_kirakira);
						setSentDialogIcon((Constant.eMoney)prevBonusInfo.RewardType);
						yield return dialogManager_.StartCoroutine(openCommonDialog(msg10, eDialog.Sent));
						if (bBonusLimitOver3)
						{
							DialogLimitOver limitOverDialog = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
							yield return StartCoroutine(limitOverDialog.show((Constant.eMoney)prevBonusInfo.RewardType));
						}
						startConfettiEff();
						Sound.Instance.playSe(Sound.eSe.SE_113_Clap);
						StartCoroutine(bgmReplay());
					}
					selectItem_.setState(true);
				}
			}
			else
			{
				yield return StartCoroutine(NetworkMng.Instance.openErrorDialog(false, eResultCode.ErrorUnknown));
			}
		}
		else
		{
			yield return dialogManager_.StartCoroutine(closeCommonDialog());
		}
	}

	private IEnumerator bgmReplay()
	{
		while (Sound.Instance.isPlayingSe(Sound.eSe.SE_113_Clap))
		{
			yield return null;
		}
		while (Sound.Instance.isPlayingSe(Sound.eSe.SE_242_kirakira))
		{
			yield return null;
		}
		Sound.Instance.pauseBgm(false);
	}

	public override void OnClose()
	{
		base.OnClose();
		Sound.Instance.pauseBgm(false);
	}

	private void setSentDialogIcon(Constant.eMoney rewardType)
	{
		switch (rewardType)
		{
		case Constant.eMoney.Jewel:
			sentDialog_.transform.Find("window/chara/anm1").GetComponent<UISprite>().spriteName = "UI_chara_00_019";
			break;
		case Constant.eMoney.Coin:
			sentDialog_.transform.Find("window/chara/anm1").GetComponent<UISprite>().spriteName = "UI_chara_00_008";
			break;
		case Constant.eMoney.Heart:
			sentDialog_.transform.Find("window/chara/anm1").GetComponent<UISprite>().spriteName = "UI_chara_00_020";
			break;
		}
	}

	private IEnumerator closeCommonDialog()
	{
		if (currentDialog_ != eDialog.None)
		{
			DialogCommon commonDialog2 = null;
			switch (currentDialog_)
			{
			default:
				yield break;
			case eDialog.Confirm:
				commonDialog2 = confirmDialog_;
				break;
			case eDialog.Sent:
				commonDialog2 = sentDialog_;
				break;
			}
			stopConfettiEff();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(commonDialog2));
		}
		currentDialog_ = eDialog.None;
	}

	private IEnumerator openCommonDialog(string msg, eDialog dialog)
	{
		currentDialog_ = dialog;
		DialogCommon commonDialog2 = null;
		switch (dialog)
		{
		default:
			yield break;
		case eDialog.Confirm:
			commonDialog2 = confirmDialog_;
			break;
		case eDialog.Sent:
			commonDialog2 = sentDialog_;
			break;
		}
		commonDialog2.setup(msg, OnDecide, OnCancel, false);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(commonDialog2));
	}

	private WWW OnCreateWWW_List(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		return WWWWrap.create("invitation/list/");
	}

	private WWW OnCreateWWW_To(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("toMid", selectItem_.getMid());
		return WWWWrap.create("invitation/");
	}

	private void updateBonusDisp(Transform bonus)
	{
		if (!GlobalData.Instance.getGameData().iosInvite)
		{
			return;
		}
		InvitationInfo.BonusInfo[] bonusInfos = dataTable_.info_.BonusInfos;
		int num = -1;
		for (int i = 0; i < bonusInfos.Length; i++)
		{
			UISprite component = bonus.Find(i + 1 + "/bg").GetComponent<UISprite>();
			if (num != -1 || (currentPeopleNum_ >= bonusInfos[i].PeopleNum && i != bonusInfos.Length - 1))
			{
				component.color = defaultBgColor;
				continue;
			}
			component.color = currentBgColor;
			num = i;
		}
		for (int j = 0; j < bonusInfos.Length; j++)
		{
			bonus.Find(j + 1 + "/complete").gameObject.SetActive(j < num);
		}
		currentBonusInfo_ = bonusInfos[num];
		UILabel component2 = bonus.Find("gauge/Label").GetComponent<UILabel>();
		if (currentPeopleNum_ >= bonusInfos[bonusInfos.Length - 1].PeopleNum)
		{
			component2.text = msgResource_.getMessage(2559);
			bonus.Find(bonusInfos.Length + "/complete").gameObject.SetActive(true);
		}
		else
		{
			string message = msgResource_.getMessage(2558);
			message = msgResource_.castCtrlCode(message, 1, currentPeopleNum_.ToString());
			message = msgResource_.castCtrlCode(message, 2, currentBonusInfo_.PeopleNum.ToString());
			switch ((Constant.eMoney)currentBonusInfo_.RewardType)
			{
			case Constant.eMoney.Jewel:
				message = msgResource_.castCtrlCode(message, 3, msgResource_.getMessage(2569));
				break;
			case Constant.eMoney.Coin:
				message = msgResource_.castCtrlCode(message, 3, msgResource_.getMessage(2568));
				break;
			case Constant.eMoney.Heart:
				message = msgResource_.castCtrlCode(message, 3, msgResource_.getMessage(2570));
				break;
			}
			component2.text = message;
		}
		UISlider component3 = bonus.Find("gauge/Progress Bar").GetComponent<UISlider>();
		component3.numberOfSteps = currentBonusInfo_.PeopleNum + 1;
		component3.sliderValue = (float)currentPeopleNum_ / (float)currentBonusInfo_.PeopleNum;
		component3.ForceUpdate();
	}
}
