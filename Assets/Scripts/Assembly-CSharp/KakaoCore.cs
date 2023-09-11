using System;
using System.Collections.Generic;
using UnityEngine;

public class KakaoCore
{
	public delegate void request_friend_callback();

	public delegate void request_friend_error_callback(int status);

	public delegate void sendmessagecallback(int resultcode);

	public delegate void sendmessage_error_callback(int resultcode);

	private static KakaoCore _instance;

	public static SNSCore.LoginCallBack_ _loginCB;

	protected static bool Authorize_;

	private bool mFriendRequest;

	private bool mInitState;

	private bool mInitComplete;

	private static request_friend_callback friend_callback_;

	private static request_friend_error_callback friend_error_callback_;

	private static sendmessagecallback sendmessage_callback_;

	private static sendmessage_error_callback sendmessage_error_callback_;

	public static KakaoCore Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new KakaoCore();
			}
			return _instance;
		}
	}

	public bool InitState
	{
		get
		{
			return mInitState;
		}
	}

	public bool InitComplete
	{
		get
		{
			return mInitComplete;
		}
	}

	public void Init()
	{
	}

	public void CreateInstance()
	{
		mFriendRequest = false;
		if (!mInitState)
		{
			KakaoNativeExtension.Instance.Init(Instance.onInitComplete, Instance.onTokens);
			mInitState = true;
		}
	}

	public void onInitComplete()
	{
		UnityEngine.Debug.Log("onInitComplete");
		mInitComplete = true;
	}

	public void Authorized()
	{
		UnityEngine.Debug.Log("Authorized");
		KakaoNativeExtension.Instance.Authorized(onAuthorized);
	}

	private void onAuthorized(bool _authorized)
	{
		SNSCore.IsAuthorize = _authorized;
		UnityEngine.Debug.Log("onAuthorized" + _authorized);
	}

	public void Login(SNSCore.LoginCallBack_ LoginCB)
	{
		_loginCB = LoginCB;
		if (SNSCore.IsAuthorize)
		{
			_loginCB(0, true);
		}
		else
		{
			KakaoNativeExtension.Instance.Login(onLoginComplete, onLoginError);
		}
	}

	private void onTokens()
	{
		UnityEngine.Debug.Log("onTokens");
		mInitComplete = true;
	}

	private static void onLoginComplete()
	{
		SNSCore.IsAuthorize = true;
		_loginCB(0, true);
	}

	private static void onLoginError(string status, string message)
	{
		SNSCore.IsAuthorize = false;
		_loginCB(Convert.ToInt32(status), false);
	}

	public void Logout()
	{
		KakaoNativeExtension.Instance.Logout(onLogoutComplete, onLogoutError);
	}

	private void onLogoutComplete()
	{
		SNSCore.IsAuthorize = false;
	}

	private void onLogoutError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}

	public void Unregister()
	{
		KakaoNativeExtension.Instance.Unregister(onUnregisterComplete, onUnregisterError);
	}

	private void onUnregisterComplete()
	{
		SNSCore.IsAuthorize = false;
	}

	private void onUnregisterError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}

	public void LoadLocalUser(KakaoResponseHandler.CompleteDelegate callback_)
	{
		KakaoNativeExtension.Instance.LocalUser(callback_, onLocalUserError);
	}

	private void onLocalUserError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}

	public void RequestFriends(request_friend_callback callback_, request_friend_error_callback error_callback_)
	{
		friend_callback_ = callback_;
		friend_error_callback_ = error_callback_;
		if (mFriendRequest)
		{
			onFriendsComplete();
		}
		else
		{
			KakaoNativeExtension.Instance.Friends(onFriendsComplete, onFriendsError);
		}
	}

	private void onFriendsComplete()
	{
		if (friend_callback_ != null)
		{
			friend_callback_();
		}
		friend_callback_ = null;
	}

	private void onFriendsError(string status, string message)
	{
		showAlertErrorMessage(status, message);
		if (friend_error_callback_ != null)
		{
			friend_error_callback_(Convert.ToInt32(status));
		}
		friend_error_callback_ = null;
	}

	public void RequestAppFriends(request_friend_callback callback_, request_friend_error_callback error_callback_)
	{
		friend_callback_ = callback_;
		friend_error_callback_ = error_callback_;
		if (mFriendRequest)
		{
			onFriendsComplete();
		}
		else
		{
			KakaoNativeExtension.Instance.Friends(onFriendsComplete, onFriendsError);
		}
	}

	public void SendMessage(eKakaoMessageTemplateId templateId, string userid, Dictionary<string, string> metaInfo, sendmessagecallback success_callback, sendmessage_error_callback error_callback)
	{
		int num = (int)templateId;
		string templateId2 = num.ToString();
		sendmessage_callback_ = success_callback;
		sendmessage_error_callback_ = error_callback;
		metaInfo.Add("sender_nick", KakaoLocalUser.Instance.nickName);
		KakaoNativeExtension.Instance.SendLinkMessage(templateId2, userid, null, "kakao91509898332478929://exec", metaInfo, onSendMessageComplete, onSendMessageError);
	}

	public void SendInviteMessage(string userid, sendmessagecallback success_callback, sendmessage_error_callback error_callback)
	{
		sendmessage_callback_ = success_callback;
		sendmessage_error_callback_ = error_callback;
		KakaoNativeExtension.Instance.SendLinkMessage("1368", userid, null, "kakao91509898332478929://exec", null, onSendMessageComplete, onSendMessageError);
	}

	private void onSendMessageComplete()
	{
		if (sendmessage_callback_ != null)
		{
			sendmessage_callback_(0);
		}
		sendmessage_callback_ = null;
	}

	private void onSendMessageError(string status, string message)
	{
		showAlertErrorMessage(status, "SendMessageError");
		if (sendmessage_error_callback_ != null)
		{
			sendmessage_error_callback_(Convert.ToInt32(status));
		}
		sendmessage_error_callback_ = null;
	}

	public bool IsBlockMessage(string mid)
	{
		if (mid.Equals(KakaoLocalUser.Instance.userId))
		{
			return KakaoLocalUser.Instance.messageBlocked;
		}
		KakaoFriends.Friend friend = null;
		friend = KakaoFriends.Instance.appFriends.Find((KakaoFriends.Friend x) => x.userid.Equals(mid));
		if (friend == null)
		{
			friend = KakaoFriends.Instance.friends.Find((KakaoFriends.Friend x) => x.userid.Equals(mid));
		}
		if (friend != null)
		{
			return friend.messageBlocked;
		}
		return true;
	}

	public bool IsSupportedDevice(string mid)
	{
		if (mid == KakaoLocalUser.Instance.userId)
		{
			return true;
		}
		KakaoFriends.Friend friend = null;
		friend = KakaoFriends.Instance.appFriends.Find((KakaoFriends.Friend x) => x.userid == mid);
		if (friend == null)
		{
			friend = KakaoFriends.Instance.friends.Find((KakaoFriends.Friend x) => x.userid == mid);
		}
		if (friend != null)
		{
			return friend.supportedDevice;
		}
		return true;
	}

	public void showAlertErrorMessage(string code, string message)
	{
		string text = string.Empty;
		if (code != null)
		{
			text = text + "Error Code : " + code;
		}
		if (message != null)
		{
			text += "\n";
			text = text + "Error Message : " + message;
		}
	}

	public void ClearCache()
	{
		mFriendRequest = false;
		KakaoNativeExtension.updateTokenCache(null, null);
	}

	public bool hasValidTokenCache()
	{
		return KakaoNativeExtension.hasValidTokenCache();
	}
}
