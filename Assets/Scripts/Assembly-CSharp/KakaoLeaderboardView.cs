using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class KakaoLeaderboardView : KakaoBaseView
{
	private Vector2 scrollPosition;

	public KakaoLeaderboardView()
		: base(KakaoViewType.Leaderboard)
	{
	}

	public override void Render()
	{
		int num = 0;
		int num2 = Screen.width - 30;
		scrollPosition = GUI.BeginScrollView(new Rect(0f, 0f, Screen.width - 10, Screen.height), scrollPosition, new Rect(0f, 0f, num2, KakaoBaseView.buttonHeight * 14));
		if (GUI.Button(new Rect(0f, 0f, Screen.width, KakaoBaseView.buttonHeight), "Go to Main"))
		{
			KakaoSample.Instance.moveToView(KakaoViewType.Main);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, Screen.width, KakaoBaseView.buttonHeight), "Load Game Info"))
		{
			KakaoLeaderboardExtension.Instance.loadGameInfo(onGameInfoComplete, onGameInfoError);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, Screen.width, KakaoBaseView.buttonHeight), "Load Game User Info"))
		{
			KakaoLeaderboardExtension.Instance.loadGameUserInfo(onGameUserInfoComplete, onGameUserInfoError);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, Screen.width, KakaoBaseView.buttonHeight), "Update User"))
		{
			byte[] bytes = Encoding.UTF8.GetBytes("level|001");
			byte[] bytes2 = Encoding.UTF8.GetBytes("money|1200");
			KakaoLeaderboardExtension.Instance.updateUser(1, bytes, bytes2, onUpdateUserComplete, onUpdateUserError);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, Screen.width, KakaoBaseView.buttonHeight), "Use Heart"))
		{
			KakaoLeaderboardExtension.Instance.useHeart(1, onUseHeartComplete, onUseHeartError);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, Screen.width, KakaoBaseView.buttonHeight), "Update Result"))
		{
			byte[] bytes3 = Encoding.UTF8.GetBytes("level|001");
			byte[] bytes4 = Encoding.UTF8.GetBytes("money|1200");
			KakaoLeaderboardExtension.Instance.updateResult("DEFAULT", 1000, 1000, bytes3, bytes4, onUpdateResultComplete, onUpdateResultError);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, Screen.width, KakaoBaseView.buttonHeight), "Update Multiple Results"))
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary.Add("DEFAULT", 1000);
			byte[] bytes5 = Encoding.UTF8.GetBytes("level|001");
			byte[] bytes6 = Encoding.UTF8.GetBytes("money|1200");
			KakaoLeaderboardExtension.Instance.updateMultipleResults(dictionary, 1000, bytes5, bytes6, onUpdateMultipleResultComplete, onUpdateMultipleResultError);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, Screen.width, KakaoBaseView.buttonHeight), "Load Leaderboard"))
		{
			KakaoLeaderboardExtension.Instance.loadLeaderboard("DEFAULT", onLoadLeaderboardComplete, onLoadLeaderboardError);
		}
		GUI.enabled = KakaoGameUserInfo.Instance.user_id != null && KakaoGameUserInfo.Instance.user_id.Length > 0;
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, Screen.width, KakaoBaseView.buttonHeight), "Load Game Friends"))
		{
			KakaoLeaderboardExtension.Instance.loadGameFriends(onLoadGameFriendsComplete, onLoadGameFriendsError);
		}
		GUI.enabled = true;
		GUI.enabled = KakaoGameFriends.Instance.kakaotalkFriends.Count > 0 || KakaoGameFriends.Instance.leaderboardFriends.Count > 0;
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, Screen.width, KakaoBaseView.buttonHeight), "Send Link Game Message"))
		{
			KakaoSample.Instance.moveToView(KakaoViewType.GameFriends);
		}
		GUI.enabled = true;
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, Screen.width, KakaoBaseView.buttonHeight), "Load Game Messages"))
		{
			KakaoLeaderboardExtension.Instance.loadGameMessages(onLoadGameMessagesComplete, onLoadGameMessagesError);
		}
		GUI.enabled = KakaoGameMessages.Instance.gameMessages.Count > 0;
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, Screen.width, KakaoBaseView.buttonHeight), "Accept All Game Messages"))
		{
			KakaoLeaderboardExtension.Instance.acceptAllGameMessages(onAcceptAllGameMessagesComplete, onAcceptAllGameMessagesError);
		}
		GUI.enabled = true;
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, Screen.width, KakaoBaseView.buttonHeight), "Block/Unblock Message"))
		{
			KakaoLeaderboardExtension.Instance.blockMessage(!KakaoGameUserInfo.Instance.message_blocked, onBlockMessageComplete, onBlockMessageError);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, Screen.width, KakaoBaseView.buttonHeight), "Delete User Info"))
		{
			KakaoLeaderboardExtension.Instance.deleteUser(onDeleteUserInfoComplete, onDeleteUserInfoError);
		}
		GUI.EndScrollView();
	}

	private void onGameInfoComplete()
	{
		Debug.Log("onGameInfoComplete");
		string text = KakaoGameInfo.Instance.max_heart.ToString();
		string text2 = KakaoGameInfo.Instance.rechargeable_heart.ToString();
		string text3 = KakaoGameInfo.Instance.heart_regen_interval.ToString();
		string text4 = KakaoGameInfo.Instance.game_message_interval.ToString();
		string text5 = KakaoGameInfo.Instance.invitation_interval.ToString();
		string text6 = KakaoGameInfo.Instance.next_score_reset_time.ToString();
		string text7 = KakaoGameInfo.Instance.last_score_reset_time.ToString();
		string text8 = KakaoGameInfo.Instance.last_score_reset_timestamp.ToString();
		string text9 = KakaoGameInfo.Instance.next_score_reset_timestamp.ToString();
		string min_version_for_ios = KakaoGameInfo.Instance.min_version_for_ios;
		string current_version_for_ios = KakaoGameInfo.Instance.current_version_for_ios;
		string min_version_for_android = KakaoGameInfo.Instance.min_version_for_android;
		string current_version_for_android = KakaoGameInfo.Instance.current_version_for_android;
		string notice = KakaoGameInfo.Instance.notice;
		string text10 = string.Empty;
		if (text != null && text.Length > 0)
		{
			text10 += "maxHeart : ";
			text10 += text;
			text10 += "\n";
		}
		if (text3 != null && text3.Length > 0)
		{
			text10 += "heartRegenInterval :";
			text10 += text3;
			text10 += "\n";
		}
		if (text2 != null && text2.Length > 0)
		{
			text10 += "rechageableHeart :";
			text10 += text2;
			text10 += "\n";
		}
		if (text4 != null && text4.Length > 0)
		{
			text10 += "gameMessageInterval :";
			text10 += text4;
			text10 += "\n";
		}
		if (text5 != null && text5.Length > 0)
		{
			text10 += "invitationInterval :";
			text10 += text5;
			text10 += "\n";
		}
		if (text6 != null && text6.Length > 0)
		{
			text10 += "nextScoreResetTime :";
			text10 += text6;
			text10 += "\n";
		}
		if (text7 != null && text7.Length > 0)
		{
			text10 += "lastScoreResetTime :";
			text10 += text7;
			text10 += "\n";
		}
		if (text9 != null && text9.Length > 0)
		{
			text10 += "nextScoreResetTimeStamp :";
			text10 += text9;
			text10 += "\n";
		}
		if (text8 != null && text8.Length > 0)
		{
			text10 += "lastScoreResetTimeStamp :";
			text10 += text8;
			text10 += "\n";
		}
		if (min_version_for_ios != null && min_version_for_ios.Length > 0)
		{
			text10 += "minVersionForiOS :";
			text10 += min_version_for_ios;
			text10 += "\n";
		}
		if (current_version_for_ios != null && current_version_for_ios.Length > 0)
		{
			text10 += "currentVersionForiOS :";
			text10 += current_version_for_ios;
			text10 += "\n";
		}
		if (min_version_for_android != null && min_version_for_android.Length > 0)
		{
			text10 += "minVersionForAndroid :";
			text10 += min_version_for_android;
			text10 += "\n";
		}
		if (current_version_for_android != null && current_version_for_android.Length > 0)
		{
			text10 += "currentVersionForAndroid :";
			text10 += current_version_for_android;
			text10 += "\n";
		}
		if (notice != null && notice.Length > 0)
		{
			text10 += "notice :";
			text10 += notice;
			text10 += "\n";
		}
		KakaoNativeExtension.Instance.ShowAlertMessage(text10);
	}

	private void onGameInfoError(string status, string message)
	{
		Debug.Log("onGameInfoError");
		showAlertErrorMessage(status, message);
	}

	private void onGameUserInfoComplete()
	{
		Debug.Log("onGameUserInfoComplete");
		string text = string.Empty;
		string user_id = KakaoGameUserInfo.Instance.user_id;
		string nickname = KakaoGameUserInfo.Instance.nickname;
		string profile_image_url = KakaoGameUserInfo.Instance.profile_image_url;
		string text2 = ((!KakaoGameUserInfo.Instance.message_blocked) ? "false" : "true");
		string text3 = KakaoGameUserInfo.Instance.exp.ToString();
		string text4 = KakaoGameUserInfo.Instance.heart.ToString();
		string text5 = KakaoGameUserInfo.Instance.heart_regen_starts_at.ToString();
		string text6 = ((KakaoGameUserInfo.Instance.publicData != null) ? Encoding.UTF8.GetString(KakaoGameUserInfo.Instance.publicData) : null);
		string text7 = ((KakaoGameUserInfo.Instance.privateData != null) ? Encoding.UTF8.GetString(KakaoGameUserInfo.Instance.privateData) : null);
		string text8 = KakaoGameUserInfo.Instance.message_count.ToString();
		if (user_id != null && user_id.Length > 0)
		{
			text += "user_id : ";
			text += user_id;
			text += "\n";
		}
		if (nickname != null && nickname.Length > 0)
		{
			text += "nickname : ";
			text += nickname;
			text += "\n";
		}
		if (profile_image_url != null && profile_image_url.Length > 0)
		{
			text += "profile_image_url : ";
			text += profile_image_url;
			text += "\n";
		}
		if (text3 != null && text3.Length > 0)
		{
			text += "exp : ";
			text += text3;
			text += "\n";
		}
		if (text4 != null && text4.Length > 0)
		{
			text += "heart : ";
			text += text4;
			text += "\n";
		}
		if (text2 != null && text2.Length > 0)
		{
			text += "message_blocked : ";
			text += text2;
			text += "\n";
		}
		if (text5 != null && text5.Length > 0)
		{
			text += "heart_regen_starts_at : ";
			text += text5;
			text += "\n";
		}
		if (text6 != null && text6.Length > 0)
		{
			text += "publicData : ";
			text += text6;
			text += "\n";
		}
		if (text7 != null && text7.Length > 0)
		{
			text += "privateData : ";
			text += text7;
			text += "\n";
		}
		if (text8 != null && text8.Length > 0)
		{
			text += "message_count : ";
			text += text8;
			text += "\n";
		}
		KakaoNativeExtension.Instance.ShowAlertMessage(text);
	}

	private void onGameUserInfoError(string status, string message)
	{
		Debug.Log("onGameUserInfoError");
		showAlertErrorMessage(status, message);
	}

	private void onUpdateUserComplete()
	{
		Debug.Log("onUpdateUserComplete");
		KakaoLeaderboardExtension.Instance.loadGameUserInfo(onGameUserInfoComplete, onGameUserInfoError);
	}

	private void onUpdateUserError(string status, string message)
	{
		Debug.Log("onUpdateUserError");
		showAlertErrorMessage(status, message);
	}

	private void onUseHeartComplete()
	{
		Debug.Log("onUseHeartComplete");
		KakaoLeaderboardExtension.Instance.loadGameUserInfo(onGameUserInfoComplete, onGameUserInfoError);
	}

	private void onUseHeartError(string status, string message)
	{
		Debug.Log("onUseHeartError");
		showAlertErrorMessage(status, message);
	}

	private void onUpdateResultComplete()
	{
		Debug.Log("onUpdateResultComplete");
		KakaoLeaderboardExtension.Instance.loadGameUserInfo(onGameUserInfoComplete, onGameUserInfoError);
	}

	private void onUpdateResultError(string status, string message)
	{
		Debug.Log("onUpdateResultError");
		showAlertErrorMessage(status, message);
	}

	private void onUpdateMultipleResultComplete()
	{
		Debug.Log("onUpdateMultipleResultComplete");
		KakaoLeaderboardExtension.Instance.loadGameUserInfo(onGameUserInfoComplete, onGameUserInfoError);
	}

	private void onUpdateMultipleResultError(string status, string message)
	{
		Debug.Log("onUpdateMultipleResultError");
		showAlertErrorMessage(status, message);
	}

	private void onLoadLeaderboardComplete()
	{
		Debug.Log("onLoadLeaderboardComplete");
		KakaoLeaderboards.Instance.printToConsole();
	}

	private void onLoadLeaderboardError(string status, string message)
	{
		Debug.Log("onLoadLeaderboardError");
		showAlertErrorMessage(status, message);
	}

	private void onLoadGameFriendsComplete()
	{
		Debug.Log("onLoadGameFriendsComplete");
		KakaoGameFriends.Instance.printToConsole();
	}

	private void onLoadGameFriendsError(string status, string message)
	{
		Debug.Log("onLoadGameFriendsError");
		showAlertErrorMessage(status, message);
	}

	private void onLoadGameMessagesComplete()
	{
		Debug.Log("onLoadGameMessagesComplete");
		KakaoSample.Instance.moveToView(KakaoViewType.Messages);
	}

	private void onLoadGameMessagesError(string status, string message)
	{
		Debug.Log("onLoadGameMessagesError");
		showAlertErrorMessage(status, message);
	}

	private void onDeleteUserInfoComplete()
	{
		Debug.Log("onDeleteUserInfoComplete");
	}

	private void onDeleteUserInfoError(string status, string message)
	{
		Debug.Log("onDeleteUserInfoError");
		showAlertErrorMessage(status, message);
	}

	private void onAcceptAllGameMessagesComplete()
	{
		Debug.Log("onAcceptAllGameMessagesComplete");
	}

	private void onAcceptAllGameMessagesError(string status, string message)
	{
		Debug.Log("onAcceptAllGameMessagesError");
		showAlertErrorMessage(status, message);
	}

	private void onBlockMessageComplete()
	{
		Debug.Log("onBlockMessageComplete");
		KakaoNativeExtension.Instance.ShowAlertMessage("onBlockMessageComplete- current state : " + KakaoGameUserInfo.Instance.message_blocked);
	}

	private void onBlockMessageError(string status, string message)
	{
		Debug.Log("onBlockMessageError");
		showAlertErrorMessage(status, message);
	}
}
