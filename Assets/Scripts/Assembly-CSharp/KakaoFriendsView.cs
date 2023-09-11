using System.Collections.Generic;
using UnityEngine;

public class KakaoFriendsView : KakaoBaseView
{
	private Vector2 scrollPosition = Vector2.zero;

	public KakaoFriendsView()
		: base(KakaoViewType.Friends)
	{
	}

	public override void Render()
	{
		if (GUI.Button(new Rect(0f, 0f, Screen.width, KakaoBaseView.buttonHeight), "Go to Main"))
		{
			KakaoSample.Instance.moveToView(KakaoViewType.Main);
		}
		int count = KakaoFriends.Instance.appFriends.Count;
		int count2 = KakaoFriends.Instance.friends.Count;
		int num = Screen.width - 30;
		scrollPosition = GUI.BeginScrollView(new Rect(0f, KakaoBaseView.buttonHeight, Screen.width - 10, Screen.height - KakaoBaseView.buttonHeight), scrollPosition, new Rect(0f, 0f, num, count * KakaoBaseView.buttonHeight + count2 * KakaoBaseView.buttonHeight + KakaoBaseView.buttonHeight * 2));
		int num2 = 0;
		KakaoFriends.Friend friend = null;
		GUI.Label(new Rect(0f, 0f, num, KakaoBaseView.buttonHeight), "App Friends");
		for (int i = 0; i < count; i++)
		{
			friend = KakaoFriends.Instance.appFriends[i];
			if (friend != null && GUI.Button(new Rect(0f, num2 += KakaoBaseView.buttonHeight, num, KakaoBaseView.buttonHeight), friend.nickname))
			{
				friend = KakaoFriends.Instance.appFriends[i];
				string text = "capture_for_image_message.png";
				string text2 = Application.persistentDataPath + "/" + text;
				KakaoSample.Instance.captureScreenToImage(text2);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("tag_item", "twinkle candy");
				KakaoNativeExtension.Instance.SendLinkMessage("1116", friend.userid, text2, "itemid=01&count=1", dictionary, onSendImageMessageComplete, onSendImageMessageError);
			}
		}
		GUI.Label(new Rect(0f, num2 += KakaoBaseView.buttonHeight, num, KakaoBaseView.buttonHeight), "Friens");
		for (int j = 0; j < count2; j++)
		{
			friend = KakaoFriends.Instance.friends[j];
			if (friend != null && GUI.Button(new Rect(0f, num2 += KakaoBaseView.buttonHeight, num, KakaoBaseView.buttonHeight), friend.nickname))
			{
				friend = KakaoFriends.Instance.friends[j];
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				dictionary2.Add("tag_hello", "Good morning!");
				KakaoNativeExtension.Instance.SendLinkMessage("1117", friend.userid, null, "itemid=01&count=1", dictionary2, onSendImageMessageComplete, onSendImageMessageError);
			}
		}
		GUI.EndScrollView();
	}

	private void onSendMessageComplete()
	{
		KakaoNativeExtension.Instance.ShowAlertMessage("Succeed SendMessage");
	}

	private void onSendMessageError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}

	private void onSendImageMessageComplete()
	{
		KakaoNativeExtension.Instance.ShowAlertMessage("Succeed SendImageMessage");
	}

	private void onSendImageMessageError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}

	private void onSendInviteImageMessageComplete()
	{
		KakaoNativeExtension.Instance.ShowAlertMessage("Succeed SendInviteImageMessage");
	}

	private void onSendInviteImageMessageError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}
}
