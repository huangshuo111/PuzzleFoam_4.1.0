using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Network;
using UnityEngine;

public class DialogRanking : DialogScrollListBase
{
	protected int userRank_;

	protected GameObject inviteItem_;

	protected GameObject dummyFriendItem_;

	protected DummyFriendDataTable dummyFriendTbl_;

	public bool bShow_ = true;

	public override void OnCreate()
	{
		base.OnCreate();
	}

	public virtual void ClearLastItem()
	{
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

	public virtual void setup()
	{
	}

	public IEnumerator loadRanking(bool bPrevious)
	{
		bShow_ = true;
		Input.enable = false;
		ClearLastItem();
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
			RankingStarData[] starList = rankingData.starList;
			foreach (RankingStarData rankData in starList)
			{
				if (DummyPlayerData.Data.ID == rankData.memberNo && rankData.lastWeekStarSum < 1)
				{
					DateTime nextDate = rankingData.getRankingStartDate();
					netData.setRankingDate(nextDate);
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
			bool bHeartflag = false;
			RankingStarData[] starList2 = rankingData.starList;
			foreach (RankingStarData rankData2 in starList2)
			{
				if (data2.ID == rankData2.memberNo)
				{
					score = ((!bPrevious) ? rankData2.allStarSum : rankData2.lastWeekStarSum);
					rankingSortScore = rankData2.allStageScoreSum;
					bFound = true;
					bHeartflag = rankData2.heartRecvFlg;
					break;
				}
			}
			if (bFound)
			{
				data2.TotalScore = score;
				data2.RankingSortScore = rankingSortScore;
				data2.IsHeartRecvFlag = bHeartflag;
				UserDataObject obj2 = createItem(data2).AddComponent<UserDataObject>();
				obj2.setData(data2);
				itemList_.Add(obj2.gameObject);
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
		UserData ud = new UserData("dummy", 0, 0, string.Empty, 0, 0L);
		UserDataObject obj_ = createItem(ud).AddComponent<UserDataObject>();
		obj_.setData(ud);
		int insertCount = 10;
		List<GameObject> tempList = new List<GameObject>();
		int tempcount_ = 0;
		foreach (GameObject obj in itemList_)
		{
			if (tempcount_ != insertCount)
			{
				tempList.Add(obj);
			}
			else
			{
				tempList.Add(obj_.gameObject);
				tempList.Add(obj);
			}
			tempcount_++;
		}
		itemList_ = tempList;
		for (int i = 0; i < itemList_.Count; i++)
		{
			GameObject item = itemList_[i];
			UserDataObject player = item.GetComponent<UserDataObject>();
			UserData data = player.getData();
			int rankValue = ((i <= insertCount) ? (i + 1) : i);
			if (data.ID == DummyPlayerData.Data.ID)
			{
				userRank_ = rankValue;
			}
			RankingItem rankItem = player.GetComponent<RankingItem>();
			if (i == insertCount)
			{
				rankItem.ShowInfomation();
			}
			else
			{
				rankItem.setup(data.UserName, data.ID, data.Mid, data.TotalScore, rankValue, data.RankingSortScore, data.avatarId, data.throwId, data.supportId);
				if (data.IsDummy)
				{
					RankingDummyItem dummyItem = player.GetComponent<RankingDummyItem>();
					dummyItem.setFriendSprite(dummyFriendTbl_.getInfo((int)data.ID));
				}
				bool block;
				if (data.Mid == DummyPlayerData.Data.Mid)
				{
					rankItem.setActiveButton(false);
					block = !data.IsHeartRecvFlag || SNSCore.Instance.IsBlockMessage(data.Mid);
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
					block = !data.IsHeartRecvFlag || SNSCore.Instance.IsBlockMessage(data.Mid);
				}
				rankItem.setMessageBlockButton(data.ID == DummyPlayerData.Data.ID, block);
				rankItem.SetGiftButtonEnable(data.ID != DummyPlayerData.Data.ID && SaveData.Instance.PresentFlag);
				addItem(item, i);
			}
			addItem(item, i);
		}
		if (!bPrevious)
		{
			MessageResource mr = MessageResource.Instance;
			DateTime nextDate3 = rankingData.getRankingStartDate();
			string msg7 = mr.getMessage(13);
			msg7 = mr.castCtrlCode(msg7, 1, nextDate3.Year.ToString());
			msg7 = mr.castCtrlCode(msg7, 2, nextDate3.Month.ToString());
			msg7 = mr.castCtrlCode(msg7, 3, nextDate3.Day.ToString());
			base.gameObject.GetComponent<DialogDayRanking>().periodLabel_.text = msg7;
			msg7 = mr.getMessage(14);
			msg7 = mr.castCtrlCode(msg7, 1, rankingData.lastWeekStarRankingRemainingDD.ToString());
			msg7 = mr.castCtrlCode(msg7, 2, rankingData.lastWeekStarRankingRemainingHH.ToString());
			base.gameObject.GetComponent<DialogDayRanking>().daysLabel_.text = msg7;
		}
		else
		{
			DateTime nextDate2 = rankingData.getRankingStartDate();
			netData.setRankingDate(nextDate2);
			netData.setRankingUniqueID(rankingData.lastWeekStarRankingStartDate2);
		}
		addItem(inviteItem_, itemList_.Count);
		repositionItem();
		Input.enable = true;
	}

	protected virtual IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "confirm_button":
			Constant.SoundUtil.PlayDecideSE();
			yield return dialogManager_.StartCoroutine(openAwardDialog());
			break;
		case "Invite_Button":
			Constant.SoundUtil.PlayButtonSE();
			yield return dialogManager_.StartCoroutine(openInviteDialog());
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
}
