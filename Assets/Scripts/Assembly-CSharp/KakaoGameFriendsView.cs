using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class KakaoGameFriendsView : KakaoBaseView
{
	private Vector2 scrollPosition;

	public KakaoGameFriendsView()
		: base(KakaoViewType.GameFriends)
	{
	}

	public override void Render()
	{
		if (GUI.Button(new Rect(0f, 0f, Screen.width, KakaoBaseView.buttonHeight), "Go to Leaderboard Main"))
		{
			KakaoSample.Instance.moveToView(KakaoViewType.Leaderboard);
		}
		int count = KakaoGameFriends.Instance.leaderboardFriends.Count;
		int count2 = KakaoGameFriends.Instance.kakaotalkFriends.Count;
		int num = Screen.width - 30;
		scrollPosition = GUI.BeginScrollView(new Rect(0f, KakaoBaseView.buttonHeight, Screen.width - 10, Screen.height - KakaoBaseView.buttonHeight), scrollPosition, new Rect(0f, 0f, num, count * KakaoBaseView.buttonHeight + count2 * KakaoBaseView.buttonHeight + KakaoBaseView.buttonHeight * 2));
		int num2 = 0;
		GUI.Label(new Rect(0f, 0f, num, KakaoBaseView.buttonHeight), "Leaderboard Friends");
		foreach (KeyValuePair<string, KakaoGameFriends.LeaderboardFriend> leaderboardFriend in KakaoGameFriends.Instance.leaderboardFriends)
		{
			KakaoGameFriends.LeaderboardFriend value = leaderboardFriend.Value;
			if (value != null && GUI.Button(new Rect(0f, num2 += KakaoBaseView.buttonHeight, num, KakaoBaseView.buttonHeight), value.nickname))
			{
				string s = "itemId|0000101";
				string text = "capture_for_image_message.png";
				string text2 = Application.persistentDataPath + "/" + text;
				KakaoSample.Instance.captureScreenToImage(text2);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("tag_item", "twinkle candy");
				KakaoLeaderboardExtension.Instance.sendLinkGameMessage(value.userid, "1116", "This is game message.", 1, Encoding.UTF8.GetBytes(s), text2, dictionary, "custom=blahblah", onSendLinkGameMessageComplete, onSendLinkGameMessageError);
			}
		}
		GUI.Label(new Rect(0f, num2 += KakaoBaseView.buttonHeight, num, KakaoBaseView.buttonHeight), "Friens");
		foreach (KeyValuePair<string, KakaoGameFriends.KakaotalkFriend> kakaotalkFriend in KakaoGameFriends.Instance.kakaotalkFriends)
		{
			KakaoGameFriends.KakaotalkFriend value2 = kakaotalkFriend.Value;
			if (value2 != null && GUI.Button(new Rect(0f, num2 += KakaoBaseView.buttonHeight, num, KakaoBaseView.buttonHeight), value2.nickname))
			{
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				dictionary2.Add("tag_hello", "Hi there~");
				KakaoLeaderboardExtension.Instance.sendInviteLinkGameMessage(value2.userid, "1117", dictionary2, "param=test", onSendInviteLinkGameMessageComplete, onSendInviteLinkGameMessageError);
			}
		}
		GUI.EndScrollView();
	}

	private void onSendLinkGameMessageComplete()
	{
		KakaoNativeExtension.Instance.ShowAlertMessage("Succeed SendLinkGameMessage");
	}

	private void onSendLinkGameMessageError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}

	private void onSendInviteLinkGameMessageComplete()
	{
		KakaoNativeExtension.Instance.ShowAlertMessage("Succeed SendInviteLinkGameMessage");
	}

	private void onSendInviteLinkGameMessageError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}
}
