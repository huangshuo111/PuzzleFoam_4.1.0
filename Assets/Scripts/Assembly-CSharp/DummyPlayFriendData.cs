using System.Collections.Generic;
using UnityEngine;

public class DummyPlayFriendData
{
	public static int FriendNum = 0;

	public static UserData[] DummyFriends = new UserData[0];

	public static List<long> playedMemberList = new List<long>();

	public static UserData createDummyFriend(int stageNo, int index)
	{
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		DummyFriendDataTable component = @object.GetComponent<DummyFriendDataTable>();
		DummyFriendInfo.Info info = component.getInfo(index);
		if (info == null)
		{
			return null;
		}
		UserData userData = new UserData();
		userData.IsDummy = true;
		userData.StageNo = stageNo;
		userData.StageClearCount = 1;
		userData.Score = info.StageScore;
		userData.TotalScore = info.StarSum;
		userData.UserName = MessageResource.Instance.getMessage(info.NameMsgID);
		userData.RankingStageScore = info.RankingStageScore;
		userData.ID = index;
		userData.minilenId = 30000;
		userData.avatarId = 20000;
		userData.throwId = 0;
		userData.supportId = 0;
		return userData;
	}

	public static UserData createRankingDummyFriend(int index)
	{
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		DummyFriendDataTable component = @object.GetComponent<DummyFriendDataTable>();
		DummyFriendInfo.Info info = component.getInfo(index);
		if (info == null)
		{
			return null;
		}
		UserData userData = new UserData();
		userData.IsDummy = true;
		userData.StageClearCount = 1;
		userData.Score = info.StageScore;
		userData.TotalScore = info.StarSum;
		userData.UserName = MessageResource.Instance.getMessage(info.NameMsgID);
		userData.ID = index;
		return userData;
	}
}
