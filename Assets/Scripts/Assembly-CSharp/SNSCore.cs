using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class SNSCore
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct SNSFriend
	{
		public string nickname { get; set; }

		public string userid { get; set; }

		public string profileImageUrl { get; set; }

		public bool messageBlocked { get; set; }

		public bool supportedDevice { get; set; }
	}

	public struct LocalUserData
	{
		public long id;

		public string name;

		public string ImageUrl;
	}

	public delegate void LoginCallBack_(int result_, bool status);

	public delegate void local_userCB_();

	public delegate void request_friend_callback(List<SNSFriend> list, int friendcount, int resultcode);

	public delegate void request_appfriend_callback(List<long> memberNos, int result);

	public delegate void sendmessagecallback(int resultcode);

	private static SNSCore _instance;

	public static Part_Title.LoginCB _loginCB;

	protected static bool Authorize_;

	private bool mFriendRequest;

	public static LocalUserData local_UserData_;

	private local_userCB_ localCB_;

	private static request_friend_callback friend_callback_;

	public List<SNSFriend> friends = new List<SNSFriend>();

	public List<SNSFriend> appFriends = new List<SNSFriend>();

	private static request_appfriend_callback appfriend_callback_;

	private static sendmessagecallback sendmessage_callback_;

	private Dictionary<int, int> ResultCodeMap_ = new Dictionary<int, int>();

	public static SNSCore Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new SNSCore();
			}
			return _instance;
		}
	}

	public static bool IsAuthorize
	{
		get
		{
			return Authorize_;
		}
		set
		{
			Authorize_ = value;
		}
	}

	public bool InitComplete
	{
		get
		{
			return KakaoCore.Instance.InitComplete;
		}
	}

	private SNSCore()
	{
		ResultCodeMapping();
	}

	public void CreateInstance()
	{
		IsAuthorize = false;
		mFriendRequest = false;
		KakaoCore.Instance.CreateInstance();
	}

	public void onInitComplete()
	{
		KakaoCore.Instance.onInitComplete();
	}

	public void Authorized()
	{
		KakaoCore.Instance.Authorized();
	}

	public void Login(Part_Title.LoginCB LoginCB)
	{
		_loginCB = LoginCB;
		KakaoCore.Instance.Login(LoginCallBack);
	}

	public void LoginCallBack(int result, bool status)
	{
		if (!ResultCodeMap_.ContainsKey(result))
		{
			_loginCB.OnLogin(99999, false);
		}
		else
		{
			_loginCB.OnLogin(ResultCodeMap_[result], status);
		}
	}

	public void Logout()
	{
		KakaoCore.Instance.Logout();
	}

	public void Unregister()
	{
		KakaoCore.Instance.Unregister();
	}

	public void LoadLocalUser(local_userCB_ callback_)
	{
		localCB_ = callback_;
		KakaoCore.Instance.LoadLocalUser(LoadLocalUserComplete);
	}

	public void LoadLocalUserComplete()
	{
		local_UserData_.id = Convert.ToInt64(KakaoLocalUser.Instance.userId);
		local_UserData_.name = KakaoLocalUser.Instance.nickName;
		local_UserData_.ImageUrl = KakaoLocalUser.Instance.profileImageUrl;
		localCB_();
	}

	public void RequestFriends(request_friend_callback callback_)
	{
		friend_callback_ = callback_;
		if (mFriendRequest)
		{
			onFriendsComplete();
		}
		else
		{
			KakaoCore.Instance.RequestFriends(onFriendsComplete, onFriendsError);
		}
	}

	private void onFriendsComplete()
	{
		if (friends != null)
		{
			friends.Clear();
		}
		else
		{
			friends = new List<SNSFriend>();
		}
		foreach (KakaoFriends.Friend friend in KakaoFriends.Instance.friends)
		{
			SNSFriend item = default(SNSFriend);
			item.nickname = friend.nickname;
			item.userid = friend.userid;
			item.profileImageUrl = friend.profileImageUrl;
			item.messageBlocked = friend.messageBlocked;
			item.supportedDevice = friend.supportedDevice;
			friends.Add(item);
		}
		if (appFriends != null)
		{
			appFriends.Clear();
		}
		else
		{
			appFriends = new List<SNSFriend>();
		}
		foreach (KakaoFriends.Friend appFriend in KakaoFriends.Instance.appFriends)
		{
			SNSFriend item2 = default(SNSFriend);
			item2.nickname = appFriend.nickname;
			item2.userid = appFriend.userid;
			item2.profileImageUrl = appFriend.profileImageUrl;
			item2.messageBlocked = appFriend.messageBlocked;
			item2.supportedDevice = appFriend.supportedDevice;
			appFriends.Add(item2);
		}
		if (friend_callback_ != null)
		{
			friend_callback_(friends, friends.Count, 0);
		}
	}

	private void onFriendsError(int status)
	{
		if (friend_callback_ != null)
		{
			friend_callback_(null, 0, ResultCodeMap_[status]);
		}
	}

	public void RequestAppFriends(request_appfriend_callback callback_)
	{
		appfriend_callback_ = callback_;
		if (mFriendRequest)
		{
			onAppFriendsComplete();
		}
		else
		{
			KakaoCore.Instance.RequestAppFriends(onAppFriendsComplete, onFriendsError);
		}
	}

	private void onAppFriendsComplete()
	{
		List<long> list = new List<long>();
		for (int i = 0; i < KakaoFriends.Instance.appFriends.Count; i++)
		{
			KakaoFriends.Friend friend = KakaoFriends.Instance.appFriends[i];
			if (friend != null)
			{
				list.Add(Convert.ToInt64(friend.userid));
			}
		}
		if (appfriend_callback_ != null)
		{
			appfriend_callback_(list, 0);
		}
	}

	public void KakaoSendMessagePresent(string receiver, sendmessagecallback success_callback)
	{
		sendmessage_callback_ = success_callback;
		KakaoCore.Instance.SendMessage(eKakaoMessageTemplateId.MSG_PRESENT_HEART, receiver, new Dictionary<string, string>(), onSendMessageComplete, onSendMessageError);
	}

	public void KakaoSendMessageRequestHeart(string receiver, sendmessagecallback success_callback)
	{
		sendmessage_callback_ = success_callback;
		KakaoCore.Instance.SendMessage(eKakaoMessageTemplateId.MSG_REQUEST_HEART, receiver, new Dictionary<string, string>(), onSendMessageComplete, onSendMessageError);
	}

	public void KakaoSendMessageFriendHelp(string receiver, sendmessagecallback success_callback)
	{
		sendmessage_callback_ = success_callback;
		KakaoCore.Instance.SendMessage(eKakaoMessageTemplateId.MSG_FRIEND_HELP, receiver, new Dictionary<string, string>(), onSendMessageComplete, onSendMessageError);
	}

	public void KakaoSendMessageRankChange(string receiver, string stage, sendmessagecallback success_callback)
	{
		sendmessage_callback_ = success_callback;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("stage", stage);
		KakaoCore.Instance.SendMessage(eKakaoMessageTemplateId.MSG_RANK_CHANGE, receiver, dictionary, onSendMessageComplete, onSendMessageError);
	}

	public void SendMessage(string messagetext, string userid, sendmessagecallback success_callback)
	{
		sendmessage_callback_ = success_callback;
	}

	public void SendMessage(string messagetext, string[] userid, sendmessagecallback success_callback)
	{
		sendmessage_callback_ = success_callback;
	}

	public void SendInviteMessage(string userid, sendmessagecallback success_callback)
	{
		sendmessage_callback_ = success_callback;
		KakaoCore.Instance.SendInviteMessage(userid, onSendMessageComplete, onSendMessageError);
	}

	private void onSendMessageComplete(int resultcode)
	{
		if (sendmessage_callback_ != null)
		{
			sendmessage_callback_(0);
		}
		sendmessage_callback_ = null;
	}

	private void onSendMessageError(int resultcode)
	{
		if (sendmessage_callback_ != null)
		{
			sendmessage_callback_(ResultCodeMap_[resultcode]);
		}
		sendmessage_callback_ = null;
	}

	public void Feed(sendmessagecallback success_callback, string title, string msg, string msg2, string imageurl = "")
	{
		sendmessage_callback_ = success_callback;
	}

	public bool IsBlockMessage(string mid)
	{
		return KakaoCore.Instance.IsBlockMessage(mid);
	}

	public bool IsSupportedDevice(string mid)
	{
		return KakaoCore.Instance.IsSupportedDevice(mid);
	}

	public void ClearCache()
	{
		mFriendRequest = false;
		KakaoCore.Instance.ClearCache();
	}

	public bool hasValidTokenCache()
	{
		return KakaoCore.Instance.hasValidTokenCache();
	}

	public void Init()
	{
		KakaoCore.Instance.Init();
	}

	private void ResultCodeMapping()
	{
		ResultCodeMap_.Add(-1, -1);
		ResultCodeMap_.Add(0, 0);
		ResultCodeMap_.Add(2, 2);
		ResultCodeMap_.Add(8, 8);
		ResultCodeMap_.Add(10, 10);
		ResultCodeMap_.Add(-9788, -9788);
		ResultCodeMap_.Add(-1000, -1000);
		ResultCodeMap_.Add(500, 500);
		ResultCodeMap_.Add(-451, -451);
		ResultCodeMap_.Add(400, 400);
		ResultCodeMap_.Add(-200, -200);
		ResultCodeMap_.Add(-100, -100);
		ResultCodeMap_.Add(-32, -32);
		ResultCodeMap_.Add(-31, -31);
		ResultCodeMap_.Add(-17, -17);
		ResultCodeMap_.Add(-16, -16);
		ResultCodeMap_.Add(-15, -15);
		ResultCodeMap_.Add(-14, -14);
		ResultCodeMap_.Add(-13, -13);
		ResultCodeMap_.Add(-12, -12);
		ResultCodeMap_.Add(-11, -11);
		ResultCodeMap_.Add(-10, -10);
	}
}
