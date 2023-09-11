using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogHighScoreList : DialogScrollListBase
{
	private UserData user1_;

	private UserData user2_;

	private int score_;

	private int stageNo_;

	private PlayerItemBase selectItem_;

	private DialogSendBase.OnSendSuccess sendCB_;

	private List<long> sendedIdList_ = new List<long>();

	public override void OnCreate()
	{
		base.OnCreate();
		sendCB_ = OnSendSuccess;
	}

	public override void init(GameObject item)
	{
		base.init(item);
		createLine();
	}

	public IEnumerator show(UserData user1, UserData user2, int score, int stageNo)
	{
		Input.enable = false;
		user1_ = user1;
		user2_ = user2;
		score_ = score;
		stageNo_ = stageNo;
		if (DummyPlayFriendData.FriendNum <= 0)
		{
			DialogConfirm dialog = dialogManager_.getDialog(DialogManager.eDialog.RequestNone) as DialogConfirm;
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
			Input.enable = true;
			yield break;
		}
		int friendNum = DummyPlayFriendData.FriendNum;
		for (int j = 0; j < friendNum; j++)
		{
			UserData data = DummyPlayFriendData.DummyFriends[j];
			GameObject item = createItem(data);
			UserDataObject user3 = item.AddComponent<UserDataObject>();
			user3.setData(data);
			itemList_.Add(item);
		}
		NetworkUtility.SortName(ref itemList_);
		addLine();
		for (int i = 0; i < itemList_.Count; i++)
		{
			GameObject user4 = itemList_[i];
			UserData data2 = user4.GetComponent<UserDataObject>().getData();
			PlayerItemBase item2 = user4.gameObject.GetComponent<PlayerItemBase>();
			item2.setup(data2.UserName, data2.ID, data2.Mid);
			addItem(user4, i);
			long prev = long.Parse(PlayerPrefs.GetString(Aes.EncryptString("HeartSend" + data2.ID, Aes.eEncodeType.Percent), "0"));
			if (sendedIdList_.Contains(data2.ID))
			{
				item2.setPrevTime(prev);
				item2.setState(true);
			}
		}
		repositionItem();
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		Input.enable = true;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "RequestButton":
		{
			Constant.SoundUtil.PlayButtonSE();
			selectItem_ = trigger.transform.parent.GetComponent<PlayerItemBase>();
			DialogSendHighScore dialog = dialogManager_.getDialog(DialogManager.eDialog.SendHighScore) as DialogSendHighScore;
			dialog.setup(selectItem_.getMid(), user1_, user2_, score_, stageNo_);
			dialog.setCB(sendCB_);
			dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
			break;
		}
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			clear();
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
		yield break;
	}

	private void OnSendSuccess()
	{
		sendedIdList_.Add(selectItem_.getID());
		selectItem_.setPrevTime(Utility.getUnixTime(DateTime.Now));
		selectItem_.setState(true);
	}
}
