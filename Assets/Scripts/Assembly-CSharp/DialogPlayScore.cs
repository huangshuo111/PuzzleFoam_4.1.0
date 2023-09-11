using System;
using System.Collections;
using System.Collections.Generic;
using Bridge;
using LitJson;
using Network;
using UnityEngine;

public class DialogPlayScore : DialogScrollListBase
{
	public enum eConnectStatus
	{
		None = 0,
		Loading = 1,
		Finish = 2
	}

	private RankingItem selectItem_;

	private List<UserData> userDataList_ = new List<UserData>();

	private List<UserData> allUserList_;

	private GameObject dummyFriendItem_;

	private DummyFriendDataTable dummyFriendTbl_;

	private GameObject borderItem_;

	private eConnectStatus connectStatus_;

	private bool bForceQuit_;

	private int closeCount;

	private WWW www_;

	public override void OnCreate()
	{
		base.OnCreate();
		dummyFriendTbl_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<DummyFriendDataTable>();
		NGUIUtilScalableUIRoot.OffsetUI(base.transform.Find("Bottom"), false);
	}

	public override void OnStartClose()
	{
		if (partManager_.currentPart == PartManager.ePart.Map && www_ != null)
		{
			www_ = null;
		}
	}

	public override void OnClose()
	{
		if (partManager_.currentPart == PartManager.ePart.Map)
		{
			clear();
			closeCount++;
			if (closeCount > 15)
			{
				closeCount = 0;
				Resources.UnloadUnusedAssets();
			}
		}
	}

	protected override void findObject()
	{
		dragPanel_ = base.transform.Find("Bottom/Tween/DragPanel").GetComponent<UIDraggablePanel>();
		grid_ = dragPanel_.transform.Find("contents").GetComponent<UIGrid>();
	}

	public void init(GameObject item, GameObject dummyFriendItem, GameObject borderItem)
	{
		base.init(item);
		dummyFriendItem_ = dummyFriendItem;
		borderItem_ = borderItem;
	}

	public IEnumerator show(int stageNo)
	{
		connectStatus_ = eConnectStatus.None;
		Input.enable = false;
		if (www_ != null)
		{
			www_.Dispose();
			www_ = null;
			yield return null;
		}
		bForceQuit_ = false;
		dialogManager_.StartCoroutine(recvScore(stageNo));
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		Input.enable = true;
	}

	public IEnumerator showRankingStage()
	{
		connectStatus_ = eConnectStatus.None;
		Input.enable = false;
		if (www_ != null)
		{
			www_.Dispose();
			www_ = null;
			yield return null;
		}
		bForceQuit_ = false;
		dialogManager_.StartCoroutine(recvRankingStageScore());
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		Input.enable = true;
	}

	private void OnDestroy()
	{
		bForceQuit_ = true;
	}

	public void forceQuitCoroutine()
	{
		bForceQuit_ = true;
	}

	private IEnumerator recvScore(int stageNo)
	{
		connectStatus_ = eConnectStatus.Loading;
		clear();
		bool bEvent = Constant.Event.isEventStage(stageNo);
		allUserList_ = new List<UserData>();
		while (DummyPlayFriendData.FriendNum > 0)
		{
			www_ = createWWW((!bEvent) ? (stageNo + 1) : stageNo);
			while (www_ != null && !www_.isDone)
			{
				if (bForceQuit_)
				{
					www_.Dispose();
					www_ = null;
					yield break;
				}
				yield return 0;
			}
			if (www_ == null)
			{
				yield break;
			}
			if (NetworkUtility.getResultCode(www_) != 0)
			{
				continue;
			}
			Network.StageData[] stageDatas = JsonMapper.ToObject<FriendStageData>(www_.text).memberStageDataList;
			for (int i = 0; i < stageDatas.Length; i++)
			{
				UserData friendData = DummyPlayFriendData.DummyFriends[i];
				friendData.StageClearCount = stageDatas[i].clearCount;
				if (stageDatas[i].clearCount < 1)
				{
					friendData.Score = 0;
					continue;
				}
				friendData.Score = stageDatas[i].hiscore;
				allUserList_.Add(friendData);
			}
			www_.Dispose();
			www_ = null;
			break;
		}
		if (dummyFriendTbl_.getInfoCount() > allUserList_.Count)
		{
			int num = dummyFriendTbl_.getInfoCount() - allUserList_.Count;
			for (int j = 0; j < num; j++)
			{
				UserData userData = DummyPlayFriendData.createDummyFriend(stageNo, j);
				allUserList_.Add(userData);
			}
		}
		if (Bridge.StageData.isClear(stageNo))
		{
			UserData playerData = DummyPlayerData.Data;
			playerData.Score = Bridge.StageData.getHighScore(stageNo);
			allUserList_.Add(playerData);
		}
		NetworkUtility.SortScore(ref allUserList_);
		userDataList_.Clear();
		int myRank = allUserList_.Count;
		int rank = 1;
		for (int i6 = 0; i6 < allUserList_.Count; i6++)
		{
			UserData data = allUserList_[i6];
			data.Rank = rank++;
			if (data.ID == DummyPlayerData.Data.ID && Bridge.StageData.isClear(stageNo))
			{
				myRank = data.Rank;
			}
			userDataList_.Add(data);
		}
		if (myRank <= 6)
		{
			for (int i5 = 0; i5 < userDataList_.Count; i5++)
			{
				addScrollList(i5);
				if (i5 >= 5)
				{
					break;
				}
			}
		}
		else if (userDataList_.Count >= 6)
		{
			if (myRank <= 3)
			{
				for (int i4 = 0; i4 < 3; i4++)
				{
					addScrollList(i4);
				}
			}
			else
			{
				for (int i3 = 0; i3 < 3; i3++)
				{
					addScrollList(i3);
				}
			}
			if (userDataList_.Count > myRank + 1)
			{
				for (int i2 = myRank - 1; i2 < myRank + 1; i2++)
				{
					addScrollList(i2);
				}
			}
			else
			{
				for (int n = myRank - 3; n < myRank; n++)
				{
					addScrollList(n);
				}
			}
		}
		else
		{
			for (int m = 0; m < userDataList_.Count; m++)
			{
				addScrollList(m);
			}
		}
		int tmpRank = 0;
		int index = -1;
		for (int l = 0; l < itemList_.Count; l++)
		{
			GameObject item2 = itemList_[l];
			UserData data3 = item2.GetComponent<UserDataObject>().getData();
			if (data3.Rank - tmpRank >= 2)
			{
				index = l;
				break;
			}
			tmpRank = data3.Rank;
		}
		if (index != -1)
		{
			GameObject line = UnityEngine.Object.Instantiate(borderItem_) as GameObject;
			itemList_.Insert(index, line);
		}
		for (int k = 0; k < itemList_.Count; k++)
		{
			GameObject obj = itemList_[k];
			UserDataObject ud = obj.GetComponent<UserDataObject>();
			if (ud == null)
			{
				addItem(obj, k);
				continue;
			}
			UserData data2 = ud.getData();
			RankingItem item = obj.gameObject.GetComponent<RankingItem>();
			item.setup(data2.UserName, data2.ID, data2.Mid, data2.Score, data2.Rank, data2.RankingSortScore, data2.avatarId, data2.throwId, data2.supportId);
			if (data2.IsDummy)
			{
				RankingDummyItem dummyItem = obj.GetComponent<RankingDummyItem>();
				dummyItem.setFriendSprite(dummyFriendTbl_.getInfo((int)data2.ID));
			}
			bool block;
			if (data2.ID == DummyPlayerData.Data.ID)
			{
				item.setActiveButton(false);
				block = !data2.IsHeartRecvFlag || SNSCore.Instance.IsBlockMessage(data2.Mid.ToString());
			}
			else
			{
				long now = Utility.getUnixTime(DateTime.Now);
				long prev = long.Parse(PlayerPrefs.GetString(Aes.EncryptString("HeartSend" + data2.ID, Aes.eEncodeType.Percent), "0"));
				if (now - prev < Constant.ResendTime)
				{
					item.setPrevTime(prev);
					item.setState(true);
				}
				block = !data2.IsHeartRecvFlag || SNSCore.Instance.IsBlockMessage(data2.Mid.ToString());
			}
			addItem(obj, k);
			item.setMessageBlockButton(data2.ID == DummyPlayerData.Data.ID, block);
		}
		repositionItem();
		connectStatus_ = eConnectStatus.Finish;
	}

	private IEnumerator recvRankingStageScore()
	{
		connectStatus_ = eConnectStatus.Loading;
		clear();
		allUserList_ = new List<UserData>();
		while (DummyPlayFriendData.FriendNum > 0)
		{
			www_ = createWWW(5000);
			while (www_ != null && !www_.isDone)
			{
				if (bForceQuit_)
				{
					www_.Dispose();
					www_ = null;
					yield break;
				}
				yield return 0;
			}
			if (www_ == null)
			{
				yield break;
			}
			if (NetworkUtility.getResultCode(www_) != 0)
			{
				continue;
			}
			Network.StageData[] stageDatas = JsonMapper.ToObject<FriendStageData>(www_.text).memberStageDataList;
			for (int i = 0; i < stageDatas.Length; i++)
			{
				UserData friendData = DummyPlayFriendData.DummyFriends[i];
				friendData.StageClearCount = stageDatas[i].clearCount;
				if (stageDatas[i].clearCount < 1)
				{
					friendData.Score = 0;
					continue;
				}
				friendData.Score = stageDatas[i].hiscore;
				allUserList_.Add(friendData);
			}
			www_.Dispose();
			www_ = null;
			break;
		}
		if (dummyFriendTbl_.getInfoCount() > allUserList_.Count)
		{
			int num = dummyFriendTbl_.getInfoCount() - allUserList_.Count;
			for (int j = 0; j < num; j++)
			{
				UserData userData = DummyPlayFriendData.createRankingDummyFriend(j);
				allUserList_.Add(userData);
			}
		}
		if (Bridge.StageData.isRankingClear())
		{
			UserData playerData = DummyPlayerData.Data;
			playerData.RankingStageScore = Bridge.StageData.getRankingStageHighScore();
			allUserList_.Add(playerData);
		}
		NetworkUtility.SortRankingStageScore(ref allUserList_);
		userDataList_.Clear();
		int myRank = allUserList_.Count;
		int rank = 1;
		for (int i6 = 0; i6 < allUserList_.Count; i6++)
		{
			UserData data = allUserList_[i6];
			data.Rank = rank++;
			if (data.ID == DummyPlayerData.Data.ID && Bridge.StageData.isRankingClear())
			{
				myRank = data.Rank;
			}
			userDataList_.Add(data);
		}
		if (myRank <= 6)
		{
			for (int i5 = 0; i5 < userDataList_.Count; i5++)
			{
				addScrollList(i5);
				if (i5 >= 5)
				{
					break;
				}
			}
		}
		else if (userDataList_.Count >= 6)
		{
			if (myRank <= 3)
			{
				for (int i4 = 0; i4 < 3; i4++)
				{
					addScrollList(i4);
				}
			}
			else
			{
				for (int i3 = 0; i3 < 3; i3++)
				{
					addScrollList(i3);
				}
			}
			if (userDataList_.Count > myRank + 1)
			{
				for (int i2 = myRank - 1; i2 < myRank + 1; i2++)
				{
					addScrollList(i2);
				}
			}
			else
			{
				for (int n = myRank - 3; n < myRank; n++)
				{
					addScrollList(n);
				}
			}
		}
		else
		{
			for (int m = 0; m < userDataList_.Count; m++)
			{
				addScrollList(m);
			}
		}
		int tmpRank = 0;
		int index = -1;
		for (int l = 0; l < itemList_.Count; l++)
		{
			GameObject item2 = itemList_[l];
			UserData data3 = item2.GetComponent<UserDataObject>().getData();
			if (data3.Rank - tmpRank >= 2)
			{
				index = l;
				break;
			}
			tmpRank = data3.Rank;
		}
		if (index != -1)
		{
			GameObject line = UnityEngine.Object.Instantiate(borderItem_) as GameObject;
			itemList_.Insert(index, line);
		}
		for (int k = 0; k < itemList_.Count; k++)
		{
			GameObject obj = itemList_[k];
			UserDataObject ud = obj.GetComponent<UserDataObject>();
			if (ud == null)
			{
				addItem(obj, k);
				continue;
			}
			UserData data2 = ud.getData();
			RankingItem item = obj.gameObject.GetComponent<RankingItem>();
			item.setup(data2.UserName, data2.ID, data2.Mid, data2.RankingStageScore, data2.Rank, data2.RankingSortScore, data2.avatarId, data2.throwId, data2.supportId);
			if (data2.IsDummy)
			{
				RankingDummyItem dummyItem = obj.GetComponent<RankingDummyItem>();
				dummyItem.setFriendSprite(dummyFriendTbl_.getInfo((int)data2.ID));
			}
			bool block;
			if (data2.ID == DummyPlayerData.Data.ID)
			{
				item.setActiveButton(false);
				block = !data2.IsHeartRecvFlag || SNSCore.Instance.IsBlockMessage(data2.Mid.ToString());
			}
			else
			{
				long now = Utility.getUnixTime(DateTime.Now);
				long prev = long.Parse(PlayerPrefs.GetString(Aes.EncryptString("HeartSend" + data2.ID, Aes.eEncodeType.Percent), "0"));
				if (now - prev < Constant.ResendTime)
				{
					item.setPrevTime(prev);
					item.setState(true);
				}
				block = !data2.IsHeartRecvFlag || SNSCore.Instance.IsBlockMessage(data2.Mid.ToString());
			}
			addItem(obj, k);
			item.setMessageBlockButton(data2.ID == DummyPlayerData.Data.ID, block);
		}
		repositionItem();
		connectStatus_ = eConnectStatus.Finish;
	}

	public eConnectStatus getConnectStatus()
	{
		return connectStatus_;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "PresentButton":
		{
			Constant.SoundUtil.PlayButtonSE();
			selectItem_ = trigger.transform.parent.GetComponent<RankingItem>();
			string user_name = selectItem_.getUserName();
			DialogCommon presentDialog = dialogManager_.getDialog(DialogManager.eDialog.Present) as DialogCommon;
			string msg2 = MessageResource.Instance.getMessage(15);
			msg2 = MessageResource.Instance.castCtrlCode(msg2, 1, user_name);
			presentDialog.setup(msg2, OnDecide, null, true);
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(presentDialog));
			break;
		}
		}
	}

	private IEnumerator OnDecide()
	{
		yield return dialogManager_.StartCoroutine(selectItem_.sendHeartMail(dialogManager_));
	}

	private void addScrollList(int index)
	{
		if (DummyPlayerData.Data.ID == userDataList_[index].ID)
		{
			addScrollList(DummyPlayerData.Data, null, -1);
		}
		else
		{
			addScrollList(userDataList_[index], allUserList_.ToArray(), index);
		}
	}

	private void addScrollList(UserData data, UserData[] datas, int index)
	{
		if (data.IsDummy)
		{
			addDummyFriend(data, dummyFriendItem_);
			return;
		}
		GameObject gameObject = null;
		gameObject = ((datas != null) ? createItem(data, datas, index) : createItem(data));
		UserDataObject userDataObject = gameObject.AddComponent<UserDataObject>();
		userDataObject.setData(data);
		itemList_.Add(gameObject);
	}

	private WWW createWWW(int stageNo)
	{
		WWWWrap.setup(WWWWrap.eMethod.Post);
		WWWWrap.addGetParameter("stageNo", stageNo);
		string text = string.Empty;
		UserData[] dummyFriends = DummyPlayFriendData.DummyFriends;
		foreach (UserData userData in dummyFriends)
		{
			if (text.Length > 0)
			{
				text += ",";
			}
			text += userData.ID;
		}
		WWWWrap.addPostParameter("memberNos", text);
		return WWWWrap.create("player/stagedata/");
	}
}
