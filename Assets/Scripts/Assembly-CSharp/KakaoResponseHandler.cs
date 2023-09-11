using SimpleJSON;
using UnityEngine;

public class KakaoResponseHandler : MonoBehaviour
{
	public delegate void CompleteDelegate();

	public delegate void ErrorDelegate(string status, string message);

	public delegate void AuthorizedDelegate(bool authorized);

	private static KakaoResponseHandler _instance;

	public CompleteDelegate showMessageBlockComplete;

	public ErrorDelegate showMessageBlockError;

	public CompleteDelegate sendLinkGameMessageComplete;

	public ErrorDelegate sendLinkGameMessageError;

	public CompleteDelegate sendInviteLinkGameMessageComplete;

	public ErrorDelegate sendInviteLinkGameMessageError;

	public CompleteDelegate initComplete;

	public AuthorizedDelegate authorized;

	public CompleteDelegate loginComplete;

	public ErrorDelegate loginError;

	public CompleteDelegate localUserComplete;

	public ErrorDelegate localUserError;

	public CompleteDelegate friendsComplete;

	public ErrorDelegate friendsError;

	public CompleteDelegate sendLinkMessageComplete;

	public ErrorDelegate sendLinkMessageError;

	public CompleteDelegate postStoryComplete;

	public ErrorDelegate postStoryError;

	public CompleteDelegate logoutComplete;

	public ErrorDelegate logoutError;

	public CompleteDelegate unregisterComplete;

	public ErrorDelegate unregisterError;

	public CompleteDelegate tokens;

	public CompleteDelegate loadGameInfoComplete;

	public ErrorDelegate loadGameInfoError;

	public CompleteDelegate loadGameUserInfoComplete;

	public ErrorDelegate loadGameUserInfoError;

	public CompleteDelegate updateUserComplete;

	public ErrorDelegate updateUserError;

	public CompleteDelegate useHeartComplete;

	public ErrorDelegate useHeartError;

	public CompleteDelegate updateResultComplete;

	public ErrorDelegate updateResultError;

	public CompleteDelegate updateMultipleResultsComplete;

	public ErrorDelegate updateMultipleResultsError;

	public CompleteDelegate loadLeaderboardComplete;

	public ErrorDelegate loadLeaderboardError;

	public CompleteDelegate blockMessageComplete;

	public ErrorDelegate blockMessageError;

	public CompleteDelegate sendGameMessageComplete;

	public ErrorDelegate sendGameMessageError;

	public CompleteDelegate sendInviteGameMessageComplete;

	public ErrorDelegate sendInviteGameMessageError;

	public CompleteDelegate loadGameFriendsComplete;

	public ErrorDelegate loadGameFriendsError;

	public CompleteDelegate loadGameMessagesComplete;

	public ErrorDelegate loadGameMessagesError;

	public CompleteDelegate acceptGameMessageComplete;

	public ErrorDelegate acceptGameMessageError;

	public CompleteDelegate acceptAllGameMessagesComplete;

	public ErrorDelegate acceptAllGameMessagesError;

	public CompleteDelegate deleteUserComplete;

	public ErrorDelegate deleteUserError;

	public CompleteDelegate invitationEventComplete;

	public CompleteDelegate invitationStatesComplete;

	public CompleteDelegate invitationHostComplete;

	public ErrorDelegate invitationEventError;

	public ErrorDelegate invitationStatesError;

	public ErrorDelegate invitationHostError;

	public static KakaoResponseHandler Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(KakaoResponseHandler)) as KakaoResponseHandler;
				if (!_instance)
				{
					GameObject gameObject = new GameObject();
					gameObject.name = "KakaoResponseHandler";
					_instance = gameObject.AddComponent(typeof(KakaoResponseHandler)) as KakaoResponseHandler;
					Object.DontDestroyOnLoad(_instance);
				}
			}
			return _instance;
		}
	}

	public static void KakaoResonseComplete(string result)
	{
		_instance.onKakaoResonseComplete(result);
	}

	public void onKakaoResonseComplete(string result)
	{
		JSONNode jSONNode = JSON.Parse(result);
		string a = jSONNode[KakaoStringKeys.Params.action];
		JSONNode jSONNode2 = jSONNode[KakaoStringKeys.Params.result];
		if (string.Equals(a, KakaoAction.Init.ToString()))
		{
			if (initComplete != null)
			{
				initComplete();
			}
			initComplete = null;
		}
		else if (string.Equals(a, KakaoAction.Authorized.ToString()) && jSONNode2 != null)
		{
			string text = jSONNode2[KakaoStringKeys.Params.authorized];
			if (authorized != null && text != null)
			{
				authorized(string.Equals(text, "true"));
			}
			authorized = null;
		}
		else if (string.Equals(a, KakaoAction.Login.ToString()))
		{
			if (loginComplete != null)
			{
				loginComplete();
			}
			loginComplete = null;
		}
		else if (string.Equals(a, KakaoAction.LocalUser.ToString()) && jSONNode2 != null)
		{
			KakaoLocalUser.Instance.setLocalUserFromJSON(jSONNode2);
			if (localUserComplete != null)
			{
				localUserComplete();
			}
			localUserComplete = null;
		}
		else if (string.Equals(a, KakaoAction.Friends.ToString()) && jSONNode2 != null)
		{
			KakaoFriends.Instance.setFriendsFromJSON(jSONNode2);
			if (friendsComplete != null)
			{
				friendsComplete();
			}
			friendsComplete = null;
		}
		else if (string.Equals(a, KakaoAction.ShowMessageBlockDialog.ToString()))
		{
			if (showMessageBlockComplete != null)
			{
				showMessageBlockComplete();
			}
			showMessageBlockComplete = null;
		}
		else if (string.Equals(a, KakaoAction.SendLinkMessage.ToString()))
		{
			if (sendLinkMessageComplete != null)
			{
				sendLinkMessageComplete();
			}
			sendLinkMessageComplete = null;
		}
		else if (string.Equals(a, KakaoAction.PostToKakaoStory.ToString()))
		{
			if (postStoryComplete != null)
			{
				postStoryComplete();
			}
			postStoryComplete = null;
		}
		else if (string.Equals(a, KakaoAction.Logout.ToString()))
		{
			if (logoutComplete != null)
			{
				logoutComplete();
			}
			logoutComplete = null;
		}
		else if (string.Equals(a, KakaoAction.Unregister.ToString()))
		{
			if (unregisterComplete != null)
			{
				unregisterComplete();
			}
			unregisterComplete = null;
		}
		else if (string.Equals(a, KakaoAction.Token.ToString()))
		{
			string text2 = jSONNode2[KakaoStringKeys.Params.access_token];
			string text3 = jSONNode2[KakaoStringKeys.Params.refresh_token];
			KakaoNativeExtension.updateTokenCache(text2, text3);
			if (text2 == null || text3 == null || text2.Length == 0 || text3.Length == 0)
			{
				clearCache();
			}
			if (tokens != null)
			{
				tokens();
			}
		}
		else if (string.Equals(a, KakaoAction.LoadGameInfo.ToString()) && jSONNode2 != null)
		{
			KakaoGameInfo.Instance.setGameInfoFromJSON(jSONNode2);
			if (loadGameInfoComplete != null)
			{
				loadGameInfoComplete();
			}
			loadGameInfoComplete = null;
		}
		else if (string.Equals(a, KakaoAction.LoadGameUserInfo.ToString()) && jSONNode2 != null)
		{
			KakaoGameUserInfo.Instance.setGameUserInfoFromJSON(jSONNode2);
			if (loadGameUserInfoComplete != null)
			{
				loadGameUserInfoComplete();
			}
			loadGameUserInfoComplete = null;
		}
		else if (string.Equals(a, KakaoAction.UpdateUser.ToString()))
		{
			if (updateUserComplete != null)
			{
				updateUserComplete();
			}
			updateUserComplete = null;
		}
		else if (string.Equals(a, KakaoAction.UseHeart.ToString()))
		{
			if (useHeartComplete != null)
			{
				useHeartComplete();
			}
			useHeartComplete = null;
		}
		else if (string.Equals(a, KakaoAction.UpdateResult.ToString()))
		{
			if (updateResultComplete != null)
			{
				updateResultComplete();
			}
			updateResultComplete = null;
		}
		else if (string.Equals(a, KakaoAction.UpdateMultipleResults.ToString()))
		{
			if (updateMultipleResultsComplete != null)
			{
				updateMultipleResultsComplete();
			}
			updateMultipleResultsComplete = null;
		}
		else if (string.Equals(a, KakaoAction.LoadLeaderboard.ToString()))
		{
			KakaoLeaderboards.Instance.setLeaderboardsFromJSON(jSONNode2);
			if (loadLeaderboardComplete != null)
			{
				loadLeaderboardComplete();
			}
			loadLeaderboardComplete = null;
		}
		else if (string.Equals(a, KakaoAction.BlockMessage.ToString()))
		{
			KakaoGameUserInfo.Instance.setMessageBlock(jSONNode2);
			if (blockMessageComplete != null)
			{
				blockMessageComplete();
			}
			blockMessageComplete = null;
		}
		else if (string.Equals(a, KakaoAction.SendLinkGameMessage.ToString()))
		{
			if (sendLinkGameMessageComplete != null)
			{
				sendLinkGameMessageComplete();
			}
			sendLinkGameMessageComplete = null;
		}
		else if (string.Equals(a, KakaoAction.SendInviteLinkGameMessage.ToString()))
		{
			if (sendInviteLinkGameMessageComplete != null)
			{
				sendInviteLinkGameMessageComplete();
			}
			sendInviteLinkGameMessageComplete = null;
		}
		else if (string.Equals(a, KakaoAction.LoadGameFriends.ToString()))
		{
			KakaoGameFriends.Instance.setGameFriendFromJSON(jSONNode2);
			if (loadGameFriendsComplete != null)
			{
				loadGameFriendsComplete();
			}
			loadGameFriendsComplete = null;
		}
		else if (string.Equals(a, KakaoAction.LoadGameMessages.ToString()))
		{
			KakaoGameMessages.Instance.setGameMessagesFromJSON(jSONNode2);
			if (loadGameMessagesComplete != null)
			{
				loadGameMessagesComplete();
			}
			loadGameMessagesComplete = null;
		}
		else if (string.Equals(a, KakaoAction.AcceptGameMessage.ToString()))
		{
			KakaoGameMessages.Instance.updateGameMessagesFromJSON(jSONNode2);
			if (acceptGameMessageComplete != null)
			{
				acceptGameMessageComplete();
			}
			acceptGameMessageComplete = null;
		}
		else if (string.Equals(a, KakaoAction.AcceptAllGameMessages.ToString()))
		{
			KakaoGameMessages.Instance.clear();
			if (acceptAllGameMessagesComplete != null)
			{
				acceptAllGameMessagesComplete();
			}
			acceptAllGameMessagesComplete = null;
		}
		else if (string.Equals(a, KakaoAction.DeleteUser.ToString()))
		{
			if (deleteUserComplete != null)
			{
				deleteUserComplete();
			}
			deleteUserComplete = null;
		}
		else if (string.Equals(a, KakaoAction.InvitationEvent.ToString()))
		{
			KakaoInvitationEvent.Instance.setInvitationEventFromJSON(jSONNode2);
			if (invitationEventComplete != null)
			{
				invitationEventComplete();
			}
			invitationEventComplete = null;
		}
		else if (string.Equals(a, KakaoAction.InvitationStates.ToString()))
		{
			KakaoInvitationStates.Instance.setInvitationStatesFromJSON(jSONNode2);
			if (invitationStatesComplete != null)
			{
				invitationStatesComplete();
			}
			invitationStatesComplete = null;
		}
		else if (string.Equals(a, KakaoAction.InvitationHost.ToString()))
		{
			KakaoInvitationHost.Instance.setInvitationHostFromJSON(jSONNode2);
			if (invitationHostComplete != null)
			{
				invitationHostComplete();
			}
			invitationHostComplete = null;
		}
	}

	private void clearCache()
	{
		KakaoFriends.Instance.clear();
		KakaoGameFriends.Instance.clear();
		KakaoGameMessages.Instance.clear();
		KakaoInvitationEvent.Instance.clear();
		KakaoInvitationStates.Instance.clear();
		KakaoInvitationHost.Instance.clear();
	}

	public static void KakaoResonseError(string error)
	{
		_instance.onKakaoResonseError(error);
	}

	public void onKakaoResonseError(string error)
	{
		JSONNode jSONNode = JSON.Parse(error);
		string a = jSONNode[KakaoStringKeys.Params.action];
		string status = null;
		string message = null;
		JSONNode jSONNode2 = jSONNode[KakaoStringKeys.Params.error];
		if (jSONNode2 != null)
		{
			status = jSONNode2["status"];
			message = jSONNode2["message"];
		}
		if (string.Equals(a, KakaoAction.Login.ToString()))
		{
			if (loginError != null)
			{
				loginError(status, message);
			}
			loginError = null;
		}
		else if (string.Equals(a, KakaoAction.LocalUser.ToString()))
		{
			if (localUserError != null)
			{
				localUserError(status, message);
			}
			localUserError = null;
		}
		else if (string.Equals(a, KakaoAction.Friends.ToString()))
		{
			if (friendsError != null)
			{
				friendsError(status, message);
			}
			friendsError = null;
		}
		else if (string.Equals(a, KakaoAction.ShowMessageBlockDialog.ToString()))
		{
			if (showMessageBlockError != null)
			{
				showMessageBlockError(status, message);
			}
			showMessageBlockError = null;
		}
		else if (string.Equals(a, KakaoAction.SendLinkMessage.ToString()))
		{
			if (sendLinkMessageError != null)
			{
				sendLinkMessageError(status, message);
			}
			sendLinkMessageError = null;
		}
		else if (string.Equals(a, KakaoAction.PostToKakaoStory.ToString()))
		{
			if (postStoryError != null)
			{
				postStoryError(status, message);
			}
			postStoryError = null;
		}
		else if (string.Equals(a, KakaoAction.Logout.ToString()))
		{
			if (logoutError != null)
			{
				logoutError(status, message);
			}
			logoutError = null;
		}
		else if (string.Equals(a, KakaoAction.Unregister.ToString()))
		{
			if (unregisterError != null)
			{
				unregisterError(status, message);
			}
			unregisterError = null;
		}
		else if (string.Equals(a, KakaoAction.LoadGameInfo.ToString()))
		{
			if (loadGameInfoError != null)
			{
				loadGameInfoError(status, message);
			}
			loadGameInfoError = null;
		}
		else if (string.Equals(a, KakaoAction.LoadGameUserInfo.ToString()))
		{
			if (loadGameUserInfoError != null)
			{
				loadGameUserInfoError(status, message);
			}
			loadGameUserInfoError = null;
		}
		else if (string.Equals(a, KakaoAction.UpdateUser.ToString()))
		{
			if (updateUserError != null)
			{
				updateUserError(status, message);
			}
			updateUserError = null;
		}
		else if (string.Equals(a, KakaoAction.UseHeart.ToString()))
		{
			if (useHeartError != null)
			{
				useHeartError(status, message);
			}
			useHeartError = null;
		}
		else if (string.Equals(a, KakaoAction.UpdateResult.ToString()))
		{
			if (updateResultError != null)
			{
				updateResultError(status, message);
			}
			updateResultError = null;
		}
		else if (string.Equals(a, KakaoAction.UpdateMultipleResults.ToString()))
		{
			if (updateMultipleResultsError != null)
			{
				updateMultipleResultsError(status, message);
			}
			updateMultipleResultsError = null;
		}
		else if (string.Equals(a, KakaoAction.LoadLeaderboard.ToString()))
		{
			if (loadLeaderboardError != null)
			{
				loadLeaderboardError(status, message);
			}
			loadLeaderboardError = null;
		}
		else if (string.Equals(a, KakaoAction.BlockMessage.ToString()))
		{
			if (blockMessageError != null)
			{
				blockMessageError(status, message);
			}
			blockMessageError = null;
		}
		else if (string.Equals(a, KakaoAction.SendLinkGameMessage.ToString()))
		{
			if (sendLinkGameMessageError != null)
			{
				sendLinkGameMessageError(status, message);
			}
			sendLinkGameMessageError = null;
		}
		else if (string.Equals(a, KakaoAction.SendInviteLinkGameMessage.ToString()))
		{
			if (sendInviteLinkGameMessageError != null)
			{
				sendInviteLinkGameMessageError(status, message);
			}
			sendInviteLinkGameMessageError = null;
		}
		else if (string.Equals(a, KakaoAction.LoadGameFriends.ToString()))
		{
			if (loadGameFriendsError != null)
			{
				loadGameFriendsError(status, message);
			}
			loadGameFriendsError = null;
		}
		else if (string.Equals(a, KakaoAction.LoadGameMessages.ToString()))
		{
			if (loadGameMessagesError != null)
			{
				loadGameMessagesError(status, message);
			}
			loadGameMessagesError = null;
		}
		else if (string.Equals(a, KakaoAction.AcceptGameMessage.ToString()))
		{
			if (acceptGameMessageError != null)
			{
				acceptGameMessageError(status, message);
			}
			acceptGameMessageError = null;
		}
		else if (string.Equals(a, KakaoAction.AcceptAllGameMessages.ToString()))
		{
			if (acceptAllGameMessagesError != null)
			{
				acceptAllGameMessagesError(status, message);
			}
			acceptAllGameMessagesError = null;
		}
		else if (string.Equals(a, KakaoAction.DeleteUser.ToString()))
		{
			if (deleteUserError != null)
			{
				deleteUserError(status, message);
			}
			deleteUserError = null;
		}
	}
}
