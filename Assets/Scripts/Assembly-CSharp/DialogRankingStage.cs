using System;
using System.Collections;
using System.Collections.Generic;
using Bridge;
using LitJson;
using Network;
using UnityEngine;

public class DialogRankingStage : DialogScrollListBase
{
	private GameObject MailNoticeIcon;

	private UILabel MailNoticeLabel;

	private UILabel InviteRewardLabel;

	private RankingItem selectItem_;

	public UILabel periodLabel_;

	public UILabel daysLabel_;

	private MessageResource msgResource_;

	protected int userRank_;

	protected GameObject inviteItem_;

	protected GameObject dummyFriendItem_;

	protected DummyFriendDataTable dummyFriendTbl_;

	public bool bShow_ = true;

	private Part_Map partMap_;

	private int mailNum_;

	public override void OnCreate()
	{
		base.OnCreate();
		daysLabel_ = base.transform.Find("window/RemainingDays_Label").GetComponent<UILabel>();
		periodLabel_ = base.transform.Find("window/Period_Label").GetComponent<UILabel>();
		MailNoticeIcon = base.transform.Find("bag_menu/06_mail/notice_icon").gameObject;
		MailNoticeLabel = MailNoticeIcon.transform.Find("Label").GetComponent<UILabel>();
		InviteRewardLabel = base.transform.Find("bag_menu/08_invite/privilege_icon/Label").GetComponent<UILabel>();
		msgResource_ = MessageResource.Instance;
	}

	public virtual void init(GameObject playerItem, GameObject inviteItem, GameObject dummyFriendItem)
	{
		base.init(playerItem);
		createLine();
		inviteItem_ = UnityEngine.Object.Instantiate(inviteItem) as GameObject;
		dummyFriendTbl_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<DummyFriendDataTable>();
		dummyFriendItem_ = dummyFriendItem;
		Utility.setParent(inviteItem_, base.transform, false);
		NGUIUtility.setupButton(inviteItem_, base.gameObject, true);
	}

	public void setup(Part_Map part)
	{
		partMap_ = part;
		setup();
	}

	public void setup()
	{
		string message = msgResource_.getMessage(13);
		message = msgResource_.castCtrlCode(message, 1, "----");
		message = msgResource_.castCtrlCode(message, 2, "--");
		message = msgResource_.castCtrlCode(message, 3, "--");
		periodLabel_.text = message;
		message = msgResource_.getMessage(14);
		message = msgResource_.castCtrlCode(message, 1, "-");
		message = msgResource_.castCtrlCode(message, 2, "-");
		daysLabel_.text = message;
		setMailNum(Bridge.PlayerData.getMailUnReadCount());
		setInviteRewardPopup();
	}

	public IEnumerator loadRanking(bool bPrevious)
	{
		bShow_ = true;
		Input.enable = false;
		SaveNetworkData netData = SaveData.Instance.getGameData().getNetworkData();
		if (bPrevious && !netData.isShowRanking())
		{
			bShow_ = false;
			Input.enable = true;
			yield break;
		}
		List<long> members = new List<long>(DummyPlayFriendData.FriendNum) { DummyPlayerData.Data.ID };
		UserData[] dummyFriends = DummyPlayFriendData.DummyFriends;
		foreach (UserData user in dummyFriends)
		{
			members.Add(user.ID);
		}
		NetworkMng.Instance.setup(Hash.PlayerStar(members.ToArray()));
		yield return dialogManager_.StartCoroutine(NetworkMng.Instance.download(API.PlayerStar, true));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			bShow_ = false;
			Input.enable = true;
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		RankingData rankingData = JsonMapper.ToObject<RankingData>(www.text);
		www.Dispose();
		if (bPrevious)
		{
			RankingStageScoreData[] rankingStageScoreList = rankingData.rankingStageScoreList;
			foreach (RankingStageScoreData rankData2 in rankingStageScoreList)
			{
				if (DummyPlayerData.Data.ID == rankData2.memberNo && rankData2.lastWeekRankingStageScore < 1)
				{
					DateTime nextDate3 = rankingData.getRankingStartDate();
					netData.setRankingDate(nextDate3);
					netData.setRankingUniqueID(rankingData.lastWeekStarRankingStartDate2);
					netData.save();
					bShow_ = false;
					Input.enable = true;
					yield break;
				}
			}
		}
		addLine();
		foreach (UserData data2 in new List<UserData>(DummyPlayFriendData.DummyFriends) { DummyPlayerData.Data })
		{
			int score = 0;
			int rankingSortScore = 0;
			bool bFound = false;
			RankingStageScoreData[] rankingStageScoreList2 = rankingData.rankingStageScoreList;
			foreach (RankingStageScoreData rankData in rankingStageScoreList2)
			{
				if (data2.ID == rankData.memberNo)
				{
					score = ((!bPrevious) ? rankData.rankingStageScore : rankData.lastWeekRankingStageScore);
					rankingSortScore = rankData.rankingStageScore;
					bFound = true;
					break;
				}
			}
			if (bFound)
			{
				data2.RankingStageScore = score;
				data2.RankingSortScore = rankingSortScore;
				UserDataObject obj = createItem(data2).AddComponent<UserDataObject>();
				obj.setData(data2);
				itemList_.Add(obj.gameObject);
			}
		}
		int friendCount = itemList_.Count - 1;
		if (dummyFriendTbl_.getInfoCount() > friendCount)
		{
			int num = dummyFriendTbl_.getInfoCount() - friendCount;
			for (int j = 0; j < num; j++)
			{
				UserData userData = DummyPlayFriendData.createDummyFriend(-1, j);
				addDummyFriend(userData, dummyFriendItem_);
			}
		}
		NetworkUtility.SortTotalScore(ref itemList_);
		for (int i = 0; i < itemList_.Count; i++)
		{
			GameObject item = itemList_[i];
			UserDataObject player = item.GetComponent<UserDataObject>();
			UserData data = player.getData();
			if (data.ID == DummyPlayerData.Data.ID)
			{
				userRank_ = i + 1;
				data.RankingStageScore = Bridge.StageData.getRankingStageHighScore();
			}
			RankingItem rankItem = player.GetComponent<RankingItem>();
			rankItem.setup(data.UserName, data.ID, data.Mid, data.RankingStageScore, i + 1, data.allStarSum, data.avatarId, data.throwId, data.supportId);
			if (data.IsDummy)
			{
				RankingDummyItem dummyItem = player.GetComponent<RankingDummyItem>();
				dummyItem.setFriendSprite(dummyFriendTbl_.getInfo((int)data.ID));
			}
			if (data.ID == DummyPlayerData.Data.ID)
			{
				rankItem.setActiveButton(false);
			}
			else
			{
				long now = Utility.getUnixTime(DateTime.Now);
				long prev = long.Parse(PlayerPrefs.GetString(Aes.EncryptString("HeartSend" + data.ID, Aes.eEncodeType.Percent), "0"));
				if (now - prev < Constant.ResendTime)
				{
					rankItem.setPrevTime(prev);
					rankItem.setState(true);
				}
			}
			addItem(item, i);
		}
		if (!bPrevious)
		{
			MessageResource mr = MessageResource.Instance;
			DateTime nextDate2 = rankingData.getRankingStartDate();
			string msg7 = mr.getMessage(13);
			msg7 = mr.castCtrlCode(msg7, 1, nextDate2.Year.ToString());
			msg7 = mr.castCtrlCode(msg7, 2, nextDate2.Month.ToString());
			msg7 = mr.castCtrlCode(msg7, 3, nextDate2.Day.ToString());
			base.gameObject.GetComponent<DialogDayRanking>().periodLabel_.text = msg7;
			msg7 = mr.getMessage(14);
			msg7 = mr.castCtrlCode(msg7, 1, rankingData.lastWeekStarRankingRemainingDD.ToString());
			msg7 = mr.castCtrlCode(msg7, 2, rankingData.lastWeekStarRankingRemainingHH.ToString());
			base.gameObject.GetComponent<DialogDayRanking>().daysLabel_.text = msg7;
		}
		else
		{
			DateTime nextDate = rankingData.getRankingStartDate();
			netData.setRankingDate(nextDate);
			netData.setRankingUniqueID(rankingData.lastWeekStarRankingStartDate2);
		}
		addItem(inviteItem_, itemList_.Count);
		repositionItem();
		Input.enable = true;
	}

	protected IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "PresentButton":
		{
			Constant.SoundUtil.PlayDecideSE();
			selectItem_ = trigger.transform.parent.GetComponent<RankingItem>();
			string user_name = selectItem_.getUserName();
			DialogCommon presentDialog = dialogManager_.getDialog(DialogManager.eDialog.Present) as DialogCommon;
			string msg2 = msgResource_.getMessage(15);
			msg2 = msgResource_.castCtrlCode(msg2, 1, user_name);
			presentDialog.setup(msg2, OnDecide, null, true);
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(presentDialog));
			break;
		}
		case "mail_button":
		{
			Constant.SoundUtil.PlayDecideSE();
			yield return StartCoroutine(partManager_.nologin());
			if (partManager_.isNologinCancel)
			{
				break;
			}
			int mailCount = 0;
			NetworkMng.Instance.setup(null);
			Mail[] mails2 = null;
			if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
			{
				break;
			}
			WWW www = NetworkMng.Instance.getWWW();
			mails2 = JsonMapper.ToObject<MailList>(www.text).mailList;
			Mail[] array = mails2;
			foreach (Mail mail in array)
			{
				if (!mail.isOpen)
				{
					mailCount++;
				}
			}
			setMailNum(mailCount);
			if (getMailNum() > 0)
			{
				DialogMail dialog2 = dialogManager_.getDialog(DialogManager.eDialog.Mail) as DialogMail;
				yield return StartCoroutine(dialog2.show(mails2));
				while (dialog2.isOpen())
				{
					yield return null;
				}
				GlobalData.Instance.getGameData().mailUnReadCount = dialog2.mailNum_;
				setMailNum(dialog2.mailNum_);
			}
			else
			{
				setMailNum(0);
				DialogConfirm dialog = dialogManager_.getDialog(DialogManager.eDialog.NoMail) as DialogConfirm;
				yield return StartCoroutine(dialogManager_.openDialog(dialog));
			}
			break;
		}
		case "option_button":
		{
			Constant.SoundUtil.PlayDecideSE();
			DialogOption dialog3 = dialogManager_.getDialog(DialogManager.eDialog.Option) as DialogOption;
			dialog3.setup();
			yield return StartCoroutine(dialogManager_.openDialog(dialog3));
			break;
		}
		case "invite_button":
			Constant.SoundUtil.PlayButtonSE();
			yield return StartCoroutine(partManager_.nologin());
			if (!partManager_.isNologinCancel)
			{
				yield return dialogManager_.StartCoroutine(openInviteDialog());
			}
			break;
		case "Play_Button":
		{
			Constant.SoundUtil.PlayButtonSE();
			StageIcon icon = trigger.transform.parent.GetComponent<StageIcon>();
			break;
		}
		case "StoryMode_Button":
			break;
		}
	}

	private IEnumerator openAwardDialog()
	{
		DialogRoulette award = dialogManager_.getDialog(DialogManager.eDialog.Award) as DialogRoulette;
		dialogManager_.addActiveDialogList(DialogManager.eDialog.Award);
		yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
		yield return dialogManager_.StartCoroutine(award.show(userRank_));
	}

	protected IEnumerator openInviteDialog()
	{
		DialogInvite dialog = dialogManager_.getDialog(DialogManager.eDialog.Invite) as DialogInvite;
		yield return dialogManager_.StartCoroutine(dialog.show());
	}

	private IEnumerator OnDecide()
	{
		yield return StartCoroutine(selectItem_.sendHeartMail(dialogManager_));
	}

	private void setMailNum(int num)
	{
		mailNum_ = num;
		if (mailNum_ > 0)
		{
			MailNoticeIcon.SetActive(true);
			MailNoticeLabel.text = mailNum_.ToString();
		}
		else
		{
			MailNoticeIcon.SetActive(false);
		}
	}

	private int getMailNum()
	{
		return mailNum_;
	}

	private void setInviteRewardPopup()
	{
		Constant.Reward reward = new Constant.Reward();
		reward.RewardType = Constant.eMoney.Coin;
		reward.Num = 1000;
		InviteBasicReward inviteBasicReward = GlobalData.Instance.getGameData().inviteBasicReward;
		reward.RewardType = (Constant.eMoney)inviteBasicReward.rewardType;
		reward.Num = inviteBasicReward.reward;
		if (!(InviteRewardLabel == null))
		{
			MessageResource instance = MessageResource.Instance;
			InviteRewardLabel.text = instance.castCtrlCode(instance.getMessage(45), 1, reward.Num.ToString("N0"));
		}
	}
}
