using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Network;
using UnityEngine;

public class FriendUpdater : MonoBehaviour
{
	public class QueryFollowingMembersPlayedGameCB
	{
		public int result_ = -1;

		public void OnMembersReceive(List<long> memberNos, int result)
		{
			result_ = result;
			if (result_ != 0)
			{
				return;
			}
			DummyPlayFriendData.playedMemberList.Clear();
			foreach (long memberNo in memberNos)
			{
				DummyPlayFriendData.playedMemberList.Add(memberNo);
			}
		}
	}

	public class QueryLineFriendsCB
	{
		public int result_ = -1;

		public List<SNSCore.SNSFriend> lineFriends_;

		public int totalLineFriendCount_;

		public void OnFriendsReceive(List<SNSCore.SNSFriend> lineFriends, int totalLineFriendCount, int result)
		{
			result_ = result;
			if (lineFriends_ == null)
			{
				lineFriends_ = new List<SNSCore.SNSFriend>();
			}
			lineFriends_.Clear();
			foreach (SNSCore.SNSFriend lineFriend in lineFriends)
			{
				lineFriends_.Add(lineFriend);
			}
			totalLineFriendCount_ = totalLineFriendCount;
		}
	}

	private static FriendUpdater instance;

	public bool isUpdate;

	private PartManager partManager_;

	private bool bFirstRetry_;

	private DialogCommon dialog_;

	public static FriendUpdater Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void stop()
	{
		StopAllCoroutines();
		if (dialog_ != null)
		{
			DialogManager dialogManager = partManager_.dialogManager;
			dialogManager.StartCoroutine(dialogManager.closeDialog(dialog_));
			dialog_ = null;
			Input.enable = false;
		}
		isUpdate = false;
	}

	public Coroutine requestUpdate(PartManager partManager)
	{
		Debug.Log("requestUpdate");
		partManager_ = partManager;
		return StartCoroutine(updateRoutine());
	}

	private IEnumerator updateRoutine()
	{
		isUpdate = true;
		yield return getFriends();
		if (DummyPlayFriendData.FriendNum < 1)
		{
			isUpdate = false;
			yield break;
		}
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, false, false));
		WWW www = NetworkMng.Instance.getWWW();
		MemberData[] playerDatas = JsonMapper.ToObject<FriendData>(www.text).memberDataList;
		www.Dispose();
		setProgress(playerDatas);
		isUpdate = false;
	}

	private WWW OnCreateWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Post);
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
		return WWWWrap.create("player/data/");
	}

	private IEnumerator retryDialog()
	{
		if (bFirstRetry_)
		{
			bFirstRetry_ = false;
			yield break;
		}
		bool bShowIcon = NetworkMng.Instance.isShowIcon();
		if (bShowIcon)
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		}
		DialogManager dialogManager = partManager_.dialogManager;
		dialog_ = dialogManager.getDialog(DialogManager.eDialog.NetworkError) as DialogCommon;
		dialog_.setMessage(MessageResource.Instance.getMessage(35));
		dialog_.setup(null, null, true);
		dialog_.setButtonText(DialogCommon.eText.Retry);
		Input.enable = true;
		yield return dialogManager.StartCoroutine(dialogManager.openDialog(dialog_));
		while (dialog_.isOpen())
		{
			yield return null;
		}
		Input.enable = false;
		dialog_ = null;
		if (bShowIcon)
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
		}
	}

	public Coroutine getFriends()
	{
		return StartCoroutine(getFriendsRoutine());
	}

	private IEnumerator getFriendsRoutine()
	{
		if (!SNSCore.IsAuthorize)
		{
			yield break;
		}
		DummyPlayFriendData.FriendNum = 0;
		DummyPlayFriendData.DummyFriends = new UserData[0];
		DummyPlayFriendData.playedMemberList.Clear();
		DummyLineFriendData.FriendNum = 0;
		DummyLineFriendData.DummyFriends = new UserData[0];
		bFirstRetry_ = true;
		while (true)
		{
			QueryFollowingMembersPlayedGameCB queryFollowingMembersPlayedGameCB = new QueryFollowingMembersPlayedGameCB();
			SNSCore.Instance.RequestAppFriends(queryFollowingMembersPlayedGameCB.OnMembersReceive);
			while (queryFollowingMembersPlayedGameCB.result_ == -1)
			{
				yield return null;
			}
			if (queryFollowingMembersPlayedGameCB.result_ == 0)
			{
				break;
			}
			yield return StartCoroutine(retryDialog());
		}
		int index = 0;
		bFirstRetry_ = true;
		while (true)
		{
			QueryLineFriendsCB queryLineFriendsCB = new QueryLineFriendsCB();
			SNSCore.Instance.RequestFriends(queryLineFriendsCB.OnFriendsReceive);
			while (queryLineFriendsCB.result_ == -1)
			{
				yield return null;
			}
			if (queryLineFriendsCB.result_ == 0)
			{
				if (queryLineFriendsCB.lineFriends_ == null || queryLineFriendsCB.totalLineFriendCount_ == -1)
				{
					break;
				}
				List<UserData> playedFriends = new List<UserData>();
				List<UserData> notPlayedFriends = new List<UserData>();
				foreach (SNSCore.SNSFriend friend2 in SNSCore.Instance.appFriends)
				{
					playedFriends.Add(new UserData
					{
						ID = Convert.ToInt64(friend2.userid),
						Mid = friend2.userid,
						UserName = friend2.nickname,
						URL = friend2.profileImageUrl
					});
				}
				foreach (SNSCore.SNSFriend friend in SNSCore.Instance.friends)
				{
					notPlayedFriends.Add(new UserData
					{
						ID = Convert.ToInt64(friend.userid),
						Mid = friend.userid,
						UserName = friend.nickname,
						URL = friend.profileImageUrl
					});
				}
				Array.Resize(ref DummyPlayFriendData.DummyFriends, DummyPlayFriendData.DummyFriends.Length + playedFriends.Count);
				Array.Copy(playedFriends.ToArray(), 0, DummyPlayFriendData.DummyFriends, DummyPlayFriendData.FriendNum, playedFriends.Count);
				DummyPlayFriendData.FriendNum += playedFriends.Count;
				Array.Resize(ref DummyLineFriendData.DummyFriends, DummyLineFriendData.DummyFriends.Length + notPlayedFriends.Count);
				Array.Copy(notPlayedFriends.ToArray(), 0, DummyLineFriendData.DummyFriends, DummyLineFriendData.FriendNum, notPlayedFriends.Count);
				DummyLineFriendData.FriendNum += notPlayedFriends.Count;
				break;
			}
			if (queryLineFriendsCB.result_ == 0)
			{
				break;
			}
			yield return StartCoroutine(retryDialog());
		}
	}

	public void setProgress(MemberData[] playerDatas, bool IsUpdateHeartFlag = true)
	{
		DummyPlayFriendData.DummyFriends = DummyPlayFriendData.DummyFriends.Where(delegate(UserData userData)
		{
			MemberData[] array2 = playerDatas;
			foreach (MemberData memberData2 in array2)
			{
				if (userData.ID == memberData2.memberNo)
				{
					return true;
				}
			}
			return false;
		}).ToArray();
		DummyPlayFriendData.FriendNum = DummyPlayFriendData.DummyFriends.Length;
		MemberData[] array = playerDatas;
		foreach (MemberData memberData in array)
		{
			UserData[] dummyFriends = DummyPlayFriendData.DummyFriends;
			foreach (UserData userData2 in dummyFriends)
			{
				if (memberData.memberNo == userData2.ID)
				{
					userData2.StageNo = memberData.progressStageNo;
					if (IsUpdateHeartFlag)
					{
						userData2.IsHeartRecvFlag = memberData.heartRecvFlg;
					}
					string text = memberData.lastUpdateTime.ToString();
					if (text.Length > 10)
					{
						text = text.Substring(0, 10);
					}
					userData2.lastUpdateTime = long.Parse(text);
					userData2.lastStageClearProgressDay = 0;
					if (memberData.isHelp)
					{
						userData2.lastStageClearProgressDay = memberData.lastStageClearProgressDay;
					}
					userData2.treasureboxNum = memberData.treasureboxNum;
					userData2.allStarSum = memberData.allStarSum;
					userData2.minilenId = memberData.minilenId;
					userData2.TotalMinilenNum = memberData.minilenTotalCount;
					break;
				}
			}
		}
	}
}
