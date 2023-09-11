using UnityEngine;

public class KakaoMainView : KakaoBaseView
{
	private Vector2 scrollPosition;

	public KakaoMainView()
		: base(KakaoViewType.Main)
	{
	}

	public override void Render()
	{
		int num = 0;
		int num2 = Screen.width - 30;
		scrollPosition = GUI.BeginScrollView(new Rect(0f, 0f, Screen.width - 10, Screen.height), scrollPosition, new Rect(0f, 0f, num2, KakaoBaseView.buttonHeight * 9));
		if (GUI.Button(new Rect(0f, num, num2, KakaoBaseView.buttonHeight), "LocalUser"))
		{
			KakaoNativeExtension.Instance.LocalUser(onLocalUserComplete, onLocalUserError);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, num2, KakaoBaseView.buttonHeight), "Friends"))
		{
			KakaoNativeExtension.Instance.Friends(onFriendsComplete, onFriendsError);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, num2, KakaoBaseView.buttonHeight), "ShowMessageBlockDialog"))
		{
			KakaoNativeExtension.Instance.ShowMessageBlockDialog(onMessageBlockDialogComplete, onMessageBlockDialogError);
		}
		GUI.enabled = KakaoFriends.Instance.hasFriends();
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, num2, KakaoBaseView.buttonHeight), "SendLinkMessage"))
		{
			KakaoSample.Instance.moveToView(KakaoViewType.Friends);
		}
		GUI.enabled = true;
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, num2, KakaoBaseView.buttonHeight), "Post To KakaoStroy"))
		{
			string text = "capture_for_post_story.png";
			string text2 = Application.persistentDataPath + "/" + text;
			KakaoSample.Instance.captureScreenToImage(text2);
			KakaoNativeExtension.Instance.PostToKakaoStory("This is testing", text2, "itemid=03", onPostStoryComplete, onPostStoryError);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, num2, KakaoBaseView.buttonHeight), "Leaderboard API"))
		{
			KakaoSample.Instance.moveToView(KakaoViewType.Leaderboard);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, num2, KakaoBaseView.buttonHeight), "Invitation Tracking API"))
		{
			KakaoSample.Instance.moveToView(KakaoViewType.InvitationTracking);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, num2, KakaoBaseView.buttonHeight), "Logout"))
		{
			KakaoNativeExtension.Instance.Logout(onLogoutComplete, onLogoutError);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, num2, KakaoBaseView.buttonHeight), "Unregister"))
		{
			KakaoNativeExtension.Instance.Unregister(onUnregisterComplete, onUnregisterError);
		}
		GUI.EndScrollView();
	}

	private void onPostStoryComplete()
	{
		KakaoNativeExtension.Instance.ShowAlertMessage("Complete post story!");
	}

	private void onPostStoryError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}

	private void onLocalUserComplete()
	{
		string nickName = KakaoLocalUser.Instance.nickName;
		string hashedTalkUserId = KakaoLocalUser.Instance.hashedTalkUserId;
		string userId = KakaoLocalUser.Instance.userId;
		string profileImageUrl = KakaoLocalUser.Instance.profileImageUrl;
		string countryIso = KakaoLocalUser.Instance.countryIso;
		bool messageBlocked = KakaoLocalUser.Instance.messageBlocked;
		string text = string.Empty;
		if (nickName != null && nickName.Length > 0)
		{
			text += "nickName : ";
			text += nickName;
			text += "\n";
		}
		if (hashedTalkUserId != null && hashedTalkUserId.Length > 0)
		{
			text += "hashedTalkUserId :";
			text += hashedTalkUserId;
			text += "\n";
		}
		if (userId != null && userId.Length > 0)
		{
			text += "userId :";
			text += userId;
			text += "\n";
		}
		if (profileImageUrl != null && profileImageUrl.Length > 0)
		{
			text += "profileImageUrl :";
			text += profileImageUrl;
			text += "\n";
		}
		if (countryIso != null && countryIso.Length > 0)
		{
			text += "countryIso :";
			text += countryIso;
			text += "\n";
		}
		text += ((!messageBlocked) ? "false" : "true");
		KakaoNativeExtension.Instance.ShowAlertMessage(text);
	}

	private void onLocalUserError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}

	private void onFriendsComplete()
	{
	}

	private void onFriendsError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}

	private void onLogoutComplete()
	{
	}

	private void onLogoutError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}

	private void onUnregisterComplete()
	{
	}

	private void onUnregisterError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}

	private void onMessageBlockDialogComplete()
	{
	}

	private void onMessageBlockDialogError(string status, string message)
	{
	}
}
