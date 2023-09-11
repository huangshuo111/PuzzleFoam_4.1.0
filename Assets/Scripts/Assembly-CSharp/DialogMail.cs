using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using UnityEngine;

public class DialogMail : DialogScrollListBase
{
	private List<string> sortList_ = new List<string>();

	private UILabel mailNumLabel_;

	private MainMenu mainMenu_;

	public int mailNum_;

	private GameObject presentItem_;

	private MailData mailData_;

	private int heartNum;

	private int coinNum;

	private int jewelNum;

	private int itemNum;

	private List<int> itemNumList = new List<int>();

	public override void OnCreate()
	{
		base.OnCreate();
		mainMenu_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		mailNumLabel_ = base.transform.Find("window/Notice_Label").GetComponent<UILabel>();
	}

	public void init(GameObject item, GameObject present_item)
	{
		presentItem_ = present_item;
		NGUIUtility.setupButton(presentItem_, base.gameObject, true);
		base.init(item);
		createLine();
	}

	public IEnumerator show(Mail[] mails)
	{
		itemList_.Clear();
		sortList_.Clear();
		string msg = MessageResource.Instance.getMessage(11);
		mailNum_ = 0;
		foreach (Mail mail in mails)
		{
			if (mail.isOpen)
			{
				continue;
			}
			if (mail.mailType == 13)
			{
				createParkNiceHeartItem(mail);
				continue;
			}
			if (mail.mailType >= 20)
			{
				createParkRewardItem(mail);
				continue;
			}
			if (mail.mailType >= 3)
			{
				GameObject item3 = UnityEngine.Object.Instantiate(presentItem_) as GameObject;
				Utility.setParent(item3, base.transform, false);
				MailData data3 = item3.AddComponent<MailData>();
				data3.UserName = string.Empty;
				if (mail.mailType >= 100)
				{
					UserData userdata = DummyPlayFriendData.DummyFriends.SingleOrDefault((UserData x) => x.ID == mail.fromMemberNo);
					if (userdata != null)
					{
						data3.UserName = userdata.UserName;
					}
					else
					{
						data3.UserName = MessageResource.Instance.getMessage(1486);
					}
				}
				data3.ID = mail.fromMemberNo;
				data3.Mid = mail.fromMemberNo.ToString();
				data3.Date = mail.recvTime.ToString();
				data3.Mail = mail;
				sortList_.Add(data3.Date);
				itemList_.Add(item3);
				mailNum_++;
				continue;
			}
			bool bUnknown = true;
			UserData[] dummyFriends = DummyPlayFriendData.DummyFriends;
			foreach (UserData friendData in dummyFriends)
			{
				if (friendData.ID == mail.fromMemberNo)
				{
					GameObject item2 = createItem(friendData);
					MailData data2 = item2.AddComponent<MailData>();
					data2.UserName = friendData.UserName;
					data2.ID = friendData.ID;
					data2.Mid = friendData.Mid;
					data2.Date = mail.recvTime.ToString();
					data2.Mail = mail;
					Texture texture = friendData.Texture;
					PlayerIcon icon = data2.gameObject.GetComponent<MailItem>().getIcon();
					icon.createMaterial();
					if (texture != null)
					{
						icon.setTexture(texture);
					}
					else
					{
						dialogManager_.StartCoroutine(icon.loadTexture(friendData.URL, true, friendData));
					}
					sortList_.Add(data2.Date);
					itemList_.Add(item2);
					mailNum_++;
					bUnknown = false;
					break;
				}
			}
			if (bUnknown)
			{
				GameObject item = createItem(null);
				MailData data = item.AddComponent<MailData>();
				data.UserName = MessageResource.Instance.getMessage(1486);
				data.ID = mail.fromMemberNo;
				data.Date = mail.recvTime.ToString();
				data.Mail = mail;
				sortList_.Add(data.Date);
				itemList_.Add(item);
				mailNum_++;
			}
		}
		msg = MessageResource.Instance.castCtrlCode(msg, 1, mailNum_.ToString());
		mailNumLabel_.text = msg;
		sortList_.Sort();
		sortList_.Reverse();
		addLine();
		for (int i = 0; i < itemList_.Count; i++)
		{
			MailData data4 = itemList_[i].GetComponent<MailData>();
			MailItem item4 = data4.gameObject.GetComponent<MailItem>();
			item4.setMessage(10);
			if (data4.Mail != null)
			{
				if (data4.Mail.mailType == 2)
				{
					item4.setMessage(2485);
					item4.setup(data4.UserName, data4.ID, data4.Mid);
				}
				else if (data4.Mail.mailType == 1 && data4.Mail.bonusType != 0)
				{
					item4.setMessage(2494);
					item4.setup(data4.UserName, data4.ID, data4.Mid);
				}
				else if (data4.Mail.mailType >= 20 && data4.Mail.mailType <= 29)
				{
					item4.setParkRewardMessage(data4.Mail.mailType, data4.Mail.count, data4.Mail.itemType, data4.Mail.message);
				}
				else if (data4.Mail.mailType == 13)
				{
					item4.setup(data4.UserName, data4.ID, data4.Mid);
				}
				else if (data4.Mail.mailType >= 3)
				{
					item4.setSupportMessage(data4.Mail.mailType, data4.Mail.count, data4.Mail.itemType, data4.Mail.message, data4.UserName);
				}
				else
				{
					item4.setup(data4.UserName, data4.ID, data4.Mid);
				}
			}
			else
			{
				item4.setup(data4.UserName, data4.ID, data4.Mid);
			}
			addItem(item4.gameObject, i);
			if (data4.Mail != null)
			{
				if (data4.Mail.mailType == 2)
				{
					item4.transform.Find("Item_Button").gameObject.SetActive(false);
					GameObject button2 = item4.transform.Find("Item_Rescue_Button").gameObject;
					button2.SetActive(true);
					item4.changeActiveButton(button2);
				}
				else if (data4.Mail.mailType == 1 && data4.Mail.bonusType != 0)
				{
					item4.transform.Find("Item_Button").gameObject.SetActive(false);
					GameObject button = item4.transform.Find("Item_Bonus_Button").gameObject;
					button.SetActive(true);
					item4.changeActiveButton(button);
				}
			}
		}
		repositionItem();
		UIButton allreceive_button = base.transform.Find("window/allreceive_button").GetComponent<UIButton>();
		allreceive_button.setEnable(true);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
	}

	private void createParkRewardItem(Mail mail)
	{
		UserData userData = Array.Find(DummyPlayFriendData.DummyFriends, (UserData f) => f.ID == mail.fromMemberNo);
		if (userData != null)
		{
			GameObject gameObject = createItem(userData);
			MailData mailData = gameObject.AddComponent<MailData>();
			mailData.UserName = userData.UserName;
			mailData.ID = userData.ID;
			mailData.Mid = userData.Mid;
			mailData.Date = mail.recvTime.ToString();
			mailData.Mail = mail;
			MailItem component = mailData.gameObject.GetComponent<MailItem>();
			Texture texture = userData.Texture;
			PlayerIcon icon = component.getIcon();
			icon.createMaterial();
			if (texture != null)
			{
				icon.setTexture(texture);
			}
			else
			{
				dialogManager_.StartCoroutine(icon.loadTexture(userData.URL, true, userData));
			}
			GameObject gameObject2 = component.transform.Find("Item_Button").gameObject;
			gameObject2.SetActive(false);
			GameObject gameObject3 = component.transform.Find("Item_ParkRewardButton").gameObject;
			gameObject3.SetActive(true);
			component.changeActiveButton(gameObject3);
			sortList_.Add(mailData.Date);
			itemList_.Add(gameObject);
		}
		else
		{
			GameObject gameObject4 = UnityEngine.Object.Instantiate(presentItem_) as GameObject;
			MailData mailData2 = gameObject4.AddComponent<MailData>();
			Utility.setParent(gameObject4, base.transform, false);
			mailData2.UserName = MessageResource.Instance.getMessage(1486);
			mailData2.ID = mail.fromMemberNo;
			mailData2.Date = mail.recvTime.ToString();
			mailData2.Mail = mail;
			sortList_.Add(mailData2.Date);
			itemList_.Add(gameObject4);
		}
		mailNum_++;
	}

	private void createParkNiceHeartItem(Mail mail)
	{
		UserData userData = Array.Find(DummyPlayFriendData.DummyFriends, (UserData f) => f.ID == mail.fromMemberNo);
		if (userData != null)
		{
			GameObject gameObject = createItem(userData);
			MailData mailData = gameObject.AddComponent<MailData>();
			mailData.UserName = userData.UserName;
			mailData.ID = userData.ID;
			mailData.Mid = userData.Mid;
			mailData.Date = mail.recvTime.ToString();
			mailData.Mail = mail;
			MailItem component = mailData.gameObject.GetComponent<MailItem>();
			Texture texture = userData.Texture;
			PlayerIcon icon = component.getIcon();
			icon.createMaterial();
			if (texture != null)
			{
				icon.setTexture(texture);
			}
			else
			{
				dialogManager_.StartCoroutine(icon.loadTexture(userData.URL, true, userData));
			}
			sortList_.Add(mailData.Date);
			itemList_.Add(gameObject);
		}
		else
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(presentItem_) as GameObject;
			MailData mailData2 = gameObject2.AddComponent<MailData>();
			Utility.setParent(gameObject2, base.transform, false);
			if (mail.fromMemberNo == 0L)
			{
				UserData userData2 = DummyPlayFriendData.createDummyFriend(-1, 0);
				mailData2.UserName = userData2.UserName;
				mailData2.ID = userData2.ID;
				mailData2.Mid = string.Empty;
			}
			else
			{
				mailData2.UserName = MessageResource.Instance.getMessage(1486);
				mailData2.ID = mail.fromMemberNo;
			}
			mailData2.Date = mail.recvTime.ToString();
			mailData2.Mail = mail;
			sortList_.Add(mailData2.Date);
			itemList_.Add(gameObject2);
		}
		mailNum_++;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Item_Button":
		case "Item_Bonus_Button":
		case "Item_Rescue_Button":
		case "Item_ParkRewardButton":
		{
			Constant.SoundUtil.PlayDecideSE();
			MailItem item = trigger.transform.parent.GetComponent<MailItem>();
			MailData data = item.GetComponent<MailData>();
			Mail mail = data.Mail;
			GameData gameData = GlobalData.Instance.getGameData();
			if (mail != null && mail.mailType == 2 && gameData.helpDataSize >= 5)
			{
				DialogCommon dialog6 = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
				dialog6.setup(2571, null, null, true);
				dialog6.setButtonActive(DialogCommon.eBtn.Close, false);
				dialog6.sysLabelEnable(false);
				yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog6));
				while (dialog6.isOpen())
				{
					yield return 0;
				}
				dialog6.sysLabelEnable(true);
				break;
			}
			heartNum = Bridge.PlayerData.getHeart();
			coinNum = Bridge.PlayerData.getCoin();
			jewelNum = Bridge.PlayerData.getJewel();
			int itemType = data.Mail.itemType;
			UserItemList[] userItemList = gameData.userItemList;
			foreach (UserItemList uItem in userItemList)
			{
				if (uItem.itemType == itemType)
				{
					itemNum = uItem.count + data.Mail.count;
					break;
				}
			}
			yield return StartCoroutine(receiveMail(item.gameObject));
			if (!item.isFinish || mail == null)
			{
				break;
			}
			if (mail.mailType == 1)
			{
				int msgNum = 2487;
				if (mail.bonusType != 0)
				{
					msgNum = 2488;
				}
				string msg2 = MessageResource.Instance.getMessage(msgNum);
				msg2 = MessageResource.Instance.castCtrlCode(msg2, 1, (Constant.ResendTime / 60 / 60).ToString());
				DialogCommon dialog5 = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
				dialog5.setup(msg2, null, null, true);
				dialog5.sysLabelEnable(false);
				yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog5));
				yield return StartCoroutine(limitOver());
				while (dialog5.isOpen())
				{
					yield return 0;
				}
				dialog5.sysLabelEnable(true);
				if (dialog5.result_ != DialogCommon.eResult.Cancel && !string.IsNullOrEmpty(data.Mid))
				{
					yield return StartCoroutine(sendHeartMail(data));
				}
			}
			else if (mail.mailType == 2)
			{
				DialogCommon dialog4 = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
				dialog4.setup(2490, null, null, true);
				dialog4.setButtonActive(DialogCommon.eBtn.Close, false);
				dialog4.sysLabelEnable(false);
				dialog4.setMessageSize(30f);
				yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog4));
				yield return StartCoroutine(limitOver());
				while (dialog4.isOpen())
				{
					yield return 0;
				}
				dialog4.sysLabelEnable(true);
				dialog4.setMessageSize(32f);
			}
			else if (mail.mailType >= 20)
			{
				DialogCommon dialog3 = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
				dialog3.setup(2573, null, null, true);
				dialog3.setButtonActive(DialogCommon.eBtn.Close, false);
				dialog3.sysLabelEnable(false);
				yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog3));
				yield return StartCoroutine(limitOver());
				while (dialog3.isOpen())
				{
					yield return 0;
				}
				dialog3.sysLabelEnable(true);
			}
			else if (mail.mailType == 13)
			{
				DialogCommon dialog2 = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
				dialog2.setup(9169, null, null, true);
				dialog2.setButtonActive(DialogCommon.eBtn.Close, false);
				dialog2.sysLabelEnable(false);
				yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog2));
				yield return StartCoroutine(limitOver());
				while (dialog2.isOpen())
				{
					yield return 0;
				}
				dialog2.sysLabelEnable(true);
			}
			else if (mail.mailType >= 3)
			{
				DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
				dialog.setup(2573, null, null, true);
				dialog.setButtonActive(DialogCommon.eBtn.Close, false);
				dialog.sysLabelEnable(false);
				yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
				yield return StartCoroutine(limitOver());
				while (dialog.isOpen())
				{
					yield return 0;
				}
				dialog.sysLabelEnable(true);
			}
			break;
		}
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			clear();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "allreceive_button":
		{
			Constant.SoundUtil.PlayDecideSE();
			trigger.GetComponent<Collider>().enabled = false;
			DialogCommon dialog_ = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
			dialog_.setup(2489, null, null, true);
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog_));
			while (dialog_.isOpen())
			{
				yield return 0;
			}
			if (dialog_.result_ == DialogCommon.eResult.Cancel)
			{
				trigger.GetComponent<Collider>().enabled = true;
				break;
			}
			heartNum = Bridge.PlayerData.getHeart();
			coinNum = Bridge.PlayerData.getCoin();
			jewelNum = Bridge.PlayerData.getJewel();
			int prevMailNum = mailNum_;
			yield return StartCoroutine(receiveMailAll());
			if (mailNum_ > 0)
			{
				trigger.GetComponent<Collider>().enabled = true;
			}
			if (prevMailNum != mailNum_)
			{
				dialog_.setup(1475, null, null, true);
				dialog_.setButtonActive(DialogCommon.eBtn.Close, false);
				yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog_));
			}
			if (heartNum > Constant.HeartMax)
			{
				DialogLimitOver limitOverDialog = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
				yield return StartCoroutine(limitOverDialog.show(Constant.eMoney.Heart));
			}
			if (coinNum > Constant.CoinMax)
			{
				DialogLimitOver limitOverDialog4 = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
				while (limitOverDialog4.isOpen())
				{
					yield return 0;
				}
				yield return StartCoroutine(limitOverDialog4.show(Constant.eMoney.Coin));
			}
			if (jewelNum > Constant.JewelMax)
			{
				DialogLimitOver limitOverDialog3 = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
				while (limitOverDialog3.isOpen())
				{
					yield return 0;
				}
				yield return StartCoroutine(limitOverDialog3.show(Constant.eMoney.Jewel));
			}
			foreach (int i in itemNumList)
			{
				if (i > Constant.ItemMax)
				{
					DialogLimitOver limitOverDialog2 = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
					while (limitOverDialog2.isOpen())
					{
						yield return 0;
					}
					yield return StartCoroutine(limitOverDialog2.show(Constant.eMoney.Ticket));
					break;
				}
			}
			while (dialog_.isOpen())
			{
				yield return 0;
			}
			clear();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
		}
	}

	private IEnumerator limitOver()
	{
		if (heartNum > Constant.HeartMax)
		{
			DialogLimitOver limitOverDialog4 = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
			yield return StartCoroutine(limitOverDialog4.show(Constant.eMoney.Heart));
		}
		if (coinNum > Constant.CoinMax)
		{
			DialogLimitOver limitOverDialog3 = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
			while (limitOverDialog3.isOpen())
			{
				yield return 0;
			}
			yield return StartCoroutine(limitOverDialog3.show(Constant.eMoney.Coin));
		}
		if (jewelNum > Constant.JewelMax)
		{
			DialogLimitOver limitOverDialog2 = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
			while (limitOverDialog2.isOpen())
			{
				yield return 0;
			}
			yield return StartCoroutine(limitOverDialog2.show(Constant.eMoney.Jewel));
		}
		if (itemNum > Constant.ItemMax)
		{
			DialogLimitOver limitOverDialog = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
			while (limitOverDialog.isOpen())
			{
				yield return 0;
			}
			yield return StartCoroutine(limitOverDialog.show(Constant.eMoney.Ticket));
		}
	}

	private WWW OnCreateWWW(Hashtable args)
	{
		if (mailData_.Mail.mailType == 13)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(mailData_.Mail.mailType.ToString());
			stringBuilder.Append(":");
			stringBuilder.Append(mailData_.Mail.fromMemberNo.ToString());
			WWWWrap.addPostParameter("fromMemberNos", stringBuilder.ToString());
			return WWWWrap.create("mail/allrecv/");
		}
		if (mailData_.Mail.mailType >= 3)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append(mailData_.Mail.mailType.ToString());
			stringBuilder2.Append(":");
			stringBuilder2.Append(mailData_.Mail.id.ToString());
			WWWWrap.addPostParameter("fromMemberNos", stringBuilder2.ToString());
			return WWWWrap.create("mail/allrecv/");
		}
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("fromMemberNo", mailData_.ID.ToString());
		WWW result = null;
		switch (mailData_.Mail.mailType)
		{
		case 1:
			result = WWWWrap.create("mail/heartrecv/");
			break;
		case 2:
			result = WWWWrap.create("mail/helprecv/");
			break;
		}
		return result;
	}

	private WWW OnCreateWWW_All(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Post);
		GameData gameData = GlobalData.Instance.getGameData();
		int num = gameData.helpDataSize;
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < itemList_.Count; i++)
		{
			MailData component = itemList_[i].GetComponent<MailData>();
			MailItem component2 = component.gameObject.GetComponent<MailItem>();
			if (component2.isFinish)
			{
				continue;
			}
			if (component.Mail.mailType == 2)
			{
				num++;
				if (num > 5)
				{
					continue;
				}
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(",");
			}
			stringBuilder.Append(component.Mail.mailType.ToString());
			stringBuilder.Append(":");
			if (component.Mail.mailType < 3 || component.Mail.mailType == 13)
			{
				stringBuilder.Append(component.Mail.fromMemberNo.ToString());
			}
			else
			{
				stringBuilder.Append(component.Mail.id.ToString());
			}
		}
		WWWWrap.addPostParameter("fromMemberNos", stringBuilder.ToString());
		return WWWWrap.create("mail/allrecv/");
	}

	private IEnumerator receiveMail(GameObject obj)
	{
		MailItem item = obj.GetComponent<MailItem>();
		if (item.isFinish)
		{
			yield break;
		}
		mailData_ = item.GetComponent<MailData>();
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, true));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		GameData json = JsonMapper.ToObject<GameData>(www.text);
		GameData gameData = GlobalData.Instance.getGameData();
		gameData.heart = json.heart;
		gameData.coin = json.coin;
		gameData.bonusJewel = json.bonusJewel;
		gameData.buyJewel = json.buyJewel;
		switch (mailData_.Mail.mailType)
		{
		case 1:
			heartNum++;
			if (mailData_.Mail.bonusType != 0)
			{
				coinNum += mailData_.Mail.bonusNum;
			}
			break;
		case 2:
			gameData.helpDataSize = json.helpDataSize;
			gameData.helpMove = json.helpMove;
			gameData.helpTime = json.helpTime;
			break;
		case 3:
			jewelNum += mailData_.Mail.count;
			Tapjoy.TrackEvent("Money", "Income Jewel", "Mail", null, mailData_.Mail.count);
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Jewel", "Mail", mailData_.Mail.count);
			break;
		case 4:
			coinNum += mailData_.Mail.count;
			Tapjoy.TrackEvent("Money", "Income Coin", "Mail", null, mailData_.Mail.count);
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Mail", mailData_.Mail.count);
			break;
		case 5:
			heartNum += mailData_.Mail.count;
			break;
		case 6:
			gameData.userItemList = json.userItemList;
			break;
		case 13:
			heartNum += mailData_.Mail.count;
			break;
		case 22:
			coinNum += mailData_.Mail.count;
			break;
		case 23:
			heartNum += mailData_.Mail.count;
			break;
		case 24:
			jewelNum += mailData_.Mail.count;
			break;
		}
		mainMenu_.update();
		item.setState(true);
		mailNum_--;
		if (mailNum_ == 0)
		{
			UIButton allreceive_button = base.transform.Find("window/allreceive_button").GetComponent<UIButton>();
			allreceive_button.setEnable(false);
			allreceive_button.transform.Find("Label").GetComponent<UILabel>().color = allreceive_button.pressed;
		}
	}

	private IEnumerator receiveMailAll()
	{
		GameData gameData = GlobalData.Instance.getGameData();
		int helpDataSize = gameData.helpDataSize;
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(OnCreateWWW_All, true));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		GameData json = JsonMapper.ToObject<GameData>(www.text);
		gameData.heart = json.heart;
		gameData.coin = json.coin;
		gameData.bonusJewel = json.bonusJewel;
		gameData.buyJewel = json.buyJewel;
		gameData.helpDataSize = json.helpDataSize;
		gameData.helpMove = json.helpMove;
		gameData.helpTime = json.helpTime;
		for (int i = 0; i < itemList_.Count; i++)
		{
			MailData data = itemList_[i].GetComponent<MailData>();
			MailItem item = data.gameObject.GetComponent<MailItem>();
			if (item.isFinish)
			{
				continue;
			}
			switch (data.Mail.mailType)
			{
			case 1:
				heartNum++;
				if (data.Mail.bonusType != 0)
				{
					coinNum += data.Mail.bonusNum;
				}
				break;
			case 2:
				helpDataSize++;
				if (helpDataSize > 5)
				{
					continue;
				}
				break;
			case 3:
				jewelNum += data.Mail.count;
				Tapjoy.TrackEvent("Money", "Income Jewel", "Mail", null, data.Mail.count);
				GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Jewel", "Mail", data.Mail.count);
				break;
			case 4:
				coinNum += data.Mail.count;
				Tapjoy.TrackEvent("Money", "Income Coin", "Mail", null, data.Mail.count);
				GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Mail", data.Mail.count);
				break;
			case 5:
				heartNum += data.Mail.count;
				break;
			case 6:
			{
				int itemType = data.Mail.itemType;
				UserItemList[] userItemList = gameData.userItemList;
				foreach (UserItemList uItem in userItemList)
				{
					if (uItem.itemType == itemType)
					{
						itemNumList.Add(uItem.count + data.Mail.count);
						break;
					}
				}
				gameData.userItemList = json.userItemList;
				break;
			}
			case 13:
				heartNum += data.Mail.count;
				break;
			case 22:
				coinNum += data.Mail.count;
				break;
			case 23:
				heartNum += data.Mail.count;
				break;
			case 24:
				jewelNum += data.Mail.count;
				break;
			}
			item.setState(true);
		}
		mainMenu_.update();
		if (helpDataSize > 5)
		{
			mailNum_ = helpDataSize - 5;
			DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
			dialog.setup(2571, null, null, true);
			dialog.setButtonActive(DialogCommon.eBtn.Close, false);
			dialog.sysLabelEnable(false);
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
			while (dialog.isOpen())
			{
				yield return 0;
			}
			dialog.sysLabelEnable(true);
		}
		else
		{
			mailNum_ = 0;
		}
		if (mailNum_ == 0)
		{
			UIButton allreceive_button = base.transform.Find("window/allreceive_button").GetComponent<UIButton>();
			allreceive_button.setEnable(false);
			allreceive_button.transform.Find("Label").GetComponent<UILabel>().color = allreceive_button.pressed;
		}
	}

	public IEnumerator sendHeartMail(MailData data)
	{
		Input.enable = false;
		yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
		Hashtable args = new Hashtable { 
		{
			"toMemberNo",
			data.ID.ToString()
		} };
		NetworkMng.Instance.setup(args);
		yield return dialogManager_.StartCoroutine(NetworkMng.Instance.download(OnCreateMailWWW, true, false));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			Input.enable = true;
			yield break;
		}
		if (DailyMission.isTermClear())
		{
			WWW www = NetworkMng.Instance.getWWW();
			Mission respons_mission2 = JsonMapper.ToObject<Mission>(www.text);
			GlobalData.Instance.setDailyMissionData(respons_mission2);
			Network.DailyMission dMission = GlobalData.Instance.getDailyMissionData();
			if (dMission == null)
			{
				NetworkMng.Instance.setup(Hash.DailyMissionCreate());
				yield return StartCoroutine(NetworkMng.Instance.download(API.DailyMissionCreate, false, false));
				WWW www_dMission = NetworkMng.Instance.getWWW();
				respons_mission2 = JsonMapper.ToObject<Mission>(www_dMission.text);
				GlobalData.Instance.setDailyMissionData(respons_mission2);
				DailyMission.bMissionCreate = true;
			}
		}
		while (!TalkMessage.Instance.isReceived)
		{
			yield return null;
		}
		string[] presentMsg = TalkMessage.Instance.getMessage(TalkMessage.eType.Present);
		for (int i = 0; i < presentMsg.Length; i++)
		{
			presentMsg[i] = presentMsg[i].Replace("{owner}", DummyPlayerData.Data.UserName);
		}
		List<string> receiverMidList = new List<string>(1) { data.Mid };
		DialogSendBase.SendAppLinkMessageCB sendAppLinkMessageCB = new DialogSendBase.SendAppLinkMessageCB();
		SNSCore.Instance.KakaoSendMessagePresent(data.Mid, sendAppLinkMessageCB.OnMessageSend);
		while (sendAppLinkMessageCB.result_ == -1)
		{
			yield return null;
		}
		yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		Input.enable = true;
		PlayerPrefs.SetString(Aes.EncryptString("HeartSend" + data.ID, Aes.eEncodeType.Percent), Utility.getUnixTime(DateTime.Now).ToString());
		PlayerPrefs.Save();
	}

	private WWW OnCreateMailWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("toMemberNo", args["toMemberNo"]);
		return WWWWrap.create("mail/heartsend/");
	}
}
