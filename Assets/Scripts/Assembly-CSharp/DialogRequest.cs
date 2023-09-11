using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Network;
using UnityEngine;

public class DialogRequest : DialogScrollListBase
{
	private DialogCommon confirmDialog_;

	private PlayerItemBase selectItem_;

	protected DialogCommon commonDialog_;

	private List<PlayerItemBase> SelectItemList = new List<PlayerItemBase>();

	private bool selectall_;

	public override void OnCreate()
	{
		commonDialog_ = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		base.OnCreate();
	}

	public override void init(GameObject item)
	{
		base.init(item);
		createLine();
		confirmDialog_ = dialogManager_.getDialog(DialogManager.eDialog.RequestConfirm) as DialogCommon;
	}

	public IEnumerator show()
	{
		Input.enable = false;
		List<UserData> friendList = new List<UserData>();
		if (DummyPlayFriendData.FriendNum > 0)
		{
			UserData[] dummyFriends = DummyPlayFriendData.DummyFriends;
			foreach (UserData data3 in dummyFriends)
			{
				long now = Utility.getUnixTime(DateTime.Now);
				long prev = long.Parse(PlayerPrefs.GetString(Aes.EncryptString("HeartRequest" + data3.ID, Aes.eEncodeType.Percent), "0"));
				if (now - prev >= 86400)
				{
					friendList.Add(data3);
				}
			}
		}
		if (friendList.Count <= 0)
		{
			DialogConfirm dialog = dialogManager_.getDialog(DialogManager.eDialog.RequestNone) as DialogConfirm;
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
			Input.enable = true;
			yield break;
		}
		foreach (UserData data2 in friendList)
		{
			GameObject item2 = createItem(data2);
			UserDataObject user2 = item2.AddComponent<UserDataObject>();
			user2.setData(data2);
			itemList_.Add(item2);
		}
		NetworkUtility.SortName(ref itemList_);
		addLine();
		for (int i = 0; i < itemList_.Count; i++)
		{
			GameObject user = itemList_[i];
			UserData data = user.GetComponent<UserDataObject>().getData();
			PlayerItemBase item = user.gameObject.GetComponent<PlayerItemBase>();
			item.setup(data.UserName, data.ID, data.Mid);
			addItem(user, i);
		}
		repositionItem();
		SelectItemList.Clear();
		base.transform.Find("DragPanel").GetComponent<UIDraggablePanel>().enabled = itemList_.Count >= 4;
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		Input.enable = true;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "RequestButton":
		case "Checkbox":
		{
			Constant.SoundUtil.PlayButtonSE();
			selectItem_ = trigger.transform.parent.GetComponent<PlayerItemBase>();
			string msg3 = MessageResource.Instance.getMessage(18);
			msg3 = MessageResource.Instance.castCtrlCode(msg3, 1, selectItem_.getUserName());
			confirmDialog_.setup(msg3, OnDecide, OnCancel, false);
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(confirmDialog_));
			break;
		}
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			clear();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "SelectAll_Button":
		{
			Constant.SoundUtil.PlayButtonSE();
			selectall_ = !selectall_;
			SelectItemList.Clear();
			PlayerItemBase[] componentsInChildren = grid_.GetComponentsInChildren<PlayerItemBase>();
			foreach (PlayerItemBase item in componentsInChildren)
			{
				if (item.SetCheckbox(selectall_) && selectall_)
				{
					SelectItemList.Add(item);
				}
			}
			break;
		}
		case "SendRequest_Button":
			Constant.SoundUtil.PlayButtonSE();
			if (SelectItemList.Count != 0)
			{
				string msg3 = MessageResource.Instance.getMessage(500023);
				confirmDialog_.setup(msg3, OnDecide, OnCancel, false);
				yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(confirmDialog_));
			}
			break;
		}
	}

	private IEnumerator OnCancel()
	{
		yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(confirmDialog_));
	}

	private IEnumerator OnDecide()
	{
		bool bCancel = false;
		while (true)
		{
			Input.enable = false;
			yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
			UserData selectuser = DummyPlayFriendData.DummyFriends.SingleOrDefault((UserData x) => x.Mid == selectItem_.getMid());
			if (selectuser == null)
			{
				bCancel = true;
				Input.enable = true;
				break;
			}
			List<long> members = new List<long> { selectItem_.getID() };
			NetworkMng.Instance.setup(Hash.PlayerStar(members.ToArray()));
			yield return dialogManager_.StartCoroutine(NetworkMng.Instance.download(API.PlayerStar, true, false));
			if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
			{
				WWW www = NetworkMng.Instance.getWWW();
				RankingData rankingData = JsonMapper.ToObject<RankingData>(www.text);
				www.Dispose();
				RankingStarData stardata = rankingData.starList.SingleOrDefault((RankingStarData x) => x.memberNo == Convert.ToInt64(selectItem_.getMid()));
				selectuser.IsHeartRecvFlag = stardata.heartRecvFlg;
			}
			while (!TalkMessage.Instance.isReceived)
			{
				yield return null;
			}
			string[] requestMsg = TalkMessage.Instance.getMessage(TalkMessage.eType.Request);
			for (int i = 0; i < requestMsg.Length; i++)
			{
				requestMsg[i] = requestMsg[i].Replace("{owner}", DummyPlayerData.Data.UserName);
			}
			List<string> receiverMidList = new List<string>(1) { selectItem_.getMid() };
			DialogSendBase.SendAppLinkMessageCB sendAppLinkMessageCB = new DialogSendBase.SendAppLinkMessageCB();
			if (!SNSCore.Instance.IsBlockMessage(selectItem_.getMid()) && selectuser.IsHeartRecvFlag)
			{
				SNSCore.Instance.KakaoSendMessageRequestHeart(selectItem_.getMid(), sendAppLinkMessageCB.OnMessageSend);
				while (sendAppLinkMessageCB.result_ == -1)
				{
					yield return null;
				}
				yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
				Input.enable = true;
				if (sendAppLinkMessageCB.result_ == 0)
				{
					break;
				}
				if (sendAppLinkMessageCB.result_ == -17 || sendAppLinkMessageCB.result_ == -16 || sendAppLinkMessageCB.result_ == -14)
				{
					string msg2 = MessageResource.Instance.getMessage(81);
					Input.enable = true;
					yield return StartCoroutine(openCommonDialog(msg2));
					while (commonDialog_.isOpen())
					{
						yield return 0;
					}
					break;
				}
				DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.NetworkError) as DialogCommon;
				dialog.setMessage(MessageResource.Instance.getMessage(35));
				dialog.setup(null, null, true);
				dialog.setButtonText(DialogCommon.eText.Retry);
				yield return StartCoroutine(dialogManager_.openDialog(dialog));
				while (dialog.isOpen())
				{
					yield return 0;
				}
				if (dialog.result_ == DialogCommon.eResult.Cancel)
				{
					bCancel = true;
					break;
				}
				continue;
			}
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			string msg = MessageResource.Instance.getMessage(81);
			Input.enable = true;
			yield return StartCoroutine(openCommonDialog(msg));
			while (commonDialog_.isOpen())
			{
				yield return 0;
			}
			break;
		}
		if (bCancel)
		{
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(confirmDialog_));
			yield break;
		}
		PlayerPrefs.SetString(Aes.EncryptString("HeartRequest" + selectItem_.getID(), Aes.eEncodeType.Percent), Utility.getUnixTime(DateTime.Now).ToString());
		PlayerPrefs.Save();
		selectItem_.setState(true);
		yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(confirmDialog_));
	}

	protected IEnumerator openCommonDialog(string msg)
	{
		commonDialog_.setButtonActive(DialogCommon.eBtn.Decide, false);
		commonDialog_.setMessage(msg);
		commonDialog_.setAutoCloseFlg(true);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(commonDialog_));
	}

	protected IEnumerator closeCommonDialog()
	{
		yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(commonDialog_));
	}

	public IEnumerator AskFriends()
	{
		yield return dialogManager_.StartCoroutine(NetworkMng.Instance.showIcon(true));
		string[] requestMsg = TalkMessage.Instance.getMessage(TalkMessage.eType.Request);
		for (int i = 0; i < requestMsg.Length; i++)
		{
			requestMsg[i] = requestMsg[i].Replace("{owner}", DummyPlayerData.Data.UserName);
		}
		DialogSendBase.SendAppLinkMessageCB sendAppLinkMessageCB = new DialogSendBase.SendAppLinkMessageCB();
		SNSCore.Instance.SendMessage(requestMsg[0], string.Empty, sendAppLinkMessageCB.OnMessageSend);
		while (sendAppLinkMessageCB.result_ == -1)
		{
			yield return null;
		}
		yield return dialogManager_.StartCoroutine(NetworkMng.Instance.showIcon(false));
		int inputcnt = Input.forceEnable();
		switch ((eSNSResultCode)sendAppLinkMessageCB.result_)
		{
		case eSNSResultCode.SUCCESS:
		{
			DialogCommon commondialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
			commondialog.setMessage(MessageResource.Instance.getMessage(500024));
			commondialog.setup(null, null, true);
			commondialog.setButtonText(DialogCommon.eText.Confirm);
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(commondialog));
			while (commondialog.isOpen())
			{
				yield return 0;
			}
			break;
		}
		case eSNSResultCode.CLOSEDIALOG:
			break;
		case eSNSResultCode.INVITE_MESSAGE_BLOCKED:
		case eSNSResultCode.MESSAGE_BLOCK_USER:
		case eSNSResultCode.UNSUPPORTED_DEVICE:
		{
			string msg = MessageResource.Instance.getMessage(81);
			yield return dialogManager_.StartCoroutine(openCommonDialog(msg));
			while (commonDialog_.isOpen())
			{
				yield return 0;
			}
			break;
		}
		default:
		{
			DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.NetworkError) as DialogCommon;
			dialog.setMessage(MessageResource.Instance.getMessage(35));
			dialog.setup(null, null, true);
			dialog.setButtonText(DialogCommon.eText.Confirm);
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
			while (dialog.isOpen())
			{
				yield return 0;
			}
			break;
		}
		}
	}
}
