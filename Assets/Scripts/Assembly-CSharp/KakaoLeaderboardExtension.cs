using System.Collections.Generic;
using UnityEngine;

public class KakaoLeaderboardExtension : KakaoNativeExtension
{
	private static KakaoLeaderboardExtension _instance;

	public new static KakaoLeaderboardExtension Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType(typeof(KakaoLeaderboardExtension)) as KakaoLeaderboardExtension;
				if (!_instance)
				{
					GameObject gameObject = new GameObject();
					gameObject.name = "KakaoLeaderboardExtension";
					_instance = gameObject.AddComponent(typeof(KakaoLeaderboardExtension)) as KakaoLeaderboardExtension;
					Object.DontDestroyOnLoad(_instance);
				}
			}
			return _instance;
		}
	}

	public void loadGameInfo(KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		Debug.Log("loadGameInfo");
		KakaoResponseHandler.Instance.loadGameInfoComplete = complete;
		KakaoResponseHandler.Instance.loadGameInfoError = error;
		KakaoParamBase param = new KakaoParamBase(KakaoAction.LoadGameInfo);
		KakaoNativeExtension.plugin.request(param);
	}

	public void loadGameUserInfo(KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		Debug.Log("loadGameUserInfo");
		KakaoResponseHandler.Instance.loadGameUserInfoComplete = complete;
		KakaoResponseHandler.Instance.loadGameUserInfoError = error;
		KakaoParamBase param = new KakaoParamBase(KakaoAction.LoadGameUserInfo);
		KakaoNativeExtension.plugin.request(param);
	}

	public void updateUser(int additionalHeart, byte[] publicData, byte[] privateData, KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		Debug.Log("updateUser");
		KakaoResponseHandler.Instance.updateUserComplete = complete;
		KakaoResponseHandler.Instance.updateUserError = error;
		KakaoParamBase param = new KakaoParamUpdateUser(additionalHeart, publicData, privateData);
		KakaoNativeExtension.plugin.request(param);
	}

	public void useHeart(int useHeart, KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		Debug.Log("useHeart");
		KakaoResponseHandler.Instance.useHeartComplete = complete;
		KakaoResponseHandler.Instance.useHeartError = error;
		KakaoParamBase param = new KakaoParamUseHeart(useHeart);
		KakaoNativeExtension.plugin.request(param);
	}

	public void updateResult(string leaderboardKey, int score, int exp, byte[] publicData, byte[] privateData, KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		Debug.Log("updateResult");
		KakaoResponseHandler.Instance.updateResultComplete = complete;
		KakaoResponseHandler.Instance.updateResultError = error;
		KakaoParamBase param = new KakaoParamUpdateResult(leaderboardKey, score, exp, publicData, privateData);
		KakaoNativeExtension.plugin.request(param);
	}

	public void updateMultipleResults(Dictionary<string, int> scores, int exp, byte[] publicData, byte[] privateData, KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		Debug.Log("updateMultipleResults");
		KakaoResponseHandler.Instance.updateMultipleResultsComplete = complete;
		KakaoResponseHandler.Instance.updateMultipleResultsError = error;
		KakaoParamBase param = new KakaoParamUpdateMultipleResults(scores, exp, publicData, privateData);
		KakaoNativeExtension.plugin.request(param);
	}

	public void loadLeaderboard(string leaderboardKey, KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		Debug.Log("loadLeaderboard");
		KakaoResponseHandler.Instance.loadLeaderboardComplete = complete;
		KakaoResponseHandler.Instance.loadLeaderboardError = error;
		KakaoParamBase param = new KakaoParamLoadLeaderboard(leaderboardKey);
		KakaoNativeExtension.plugin.request(param);
	}

	public void blockMessage(bool block, KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		Debug.Log("blockMessage");
		KakaoResponseHandler.Instance.blockMessageComplete = complete;
		KakaoResponseHandler.Instance.blockMessageError = error;
		KakaoParamBase param = new KakaoParamBlockMessage(block);
		KakaoNativeExtension.plugin.request(param);
	}

	public void sendLinkGameMessage(string receiverId, string templateId, string gameMessage, int heart, byte[] data, string imagePath, Dictionary<string, string> metaInfo, string executeUrl, KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		Debug.Log("sendLinkGameMessage");
		KakaoResponseHandler.Instance.sendLinkGameMessageComplete = complete;
		KakaoResponseHandler.Instance.sendLinkGameMessageError = error;
		KakaoParamLinkGameMessage param = new KakaoParamLinkGameMessage(receiverId, templateId, gameMessage, heart, data, imagePath, metaInfo, executeUrl);
		KakaoNativeExtension.plugin.request(param);
	}

	public void sendInviteLinkGameMessage(string receiverId, string templateId, Dictionary<string, string> metaInfo, string executeUrl, KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		Debug.Log("sendInviteLinkGameMessage");
		KakaoResponseHandler.Instance.sendInviteLinkGameMessageComplete = complete;
		KakaoResponseHandler.Instance.sendInviteLinkGameMessageError = error;
		KakaoParamInviteLinkGameMessage param = new KakaoParamInviteLinkGameMessage(receiverId, templateId, metaInfo, executeUrl);
		KakaoNativeExtension.plugin.request(param);
	}

	public void acceptGameMessage(string id, KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		Debug.Log("acceptGameMessage");
		KakaoResponseHandler.Instance.acceptGameMessageComplete = complete;
		KakaoResponseHandler.Instance.acceptGameMessageError = error;
		KakaoParamBase param = new KakaoParamAcceptGameMessage(id);
		KakaoNativeExtension.plugin.request(param);
	}

	public void loadGameFriends(KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		Debug.Log("loadGameFriends");
		KakaoResponseHandler.Instance.loadGameFriendsComplete = complete;
		KakaoResponseHandler.Instance.loadGameFriendsError = error;
		KakaoParamBase param = new KakaoParamBase(KakaoAction.LoadGameFriends);
		KakaoNativeExtension.plugin.request(param);
	}

	public void loadGameMessages(KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		Debug.Log("loadGameMessages");
		KakaoResponseHandler.Instance.loadGameMessagesComplete = complete;
		KakaoResponseHandler.Instance.loadGameMessagesError = error;
		KakaoParamBase param = new KakaoParamBase(KakaoAction.LoadGameMessages);
		KakaoNativeExtension.plugin.request(param);
	}

	public void acceptAllGameMessages(KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		Debug.Log("acceptAllGameMessages");
		KakaoResponseHandler.Instance.acceptAllGameMessagesComplete = complete;
		KakaoResponseHandler.Instance.acceptAllGameMessagesError = error;
		KakaoParamBase param = new KakaoParamBase(KakaoAction.AcceptAllGameMessages);
		KakaoNativeExtension.plugin.request(param);
	}

	public void deleteUser(KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		Debug.Log("deleteUser");
		KakaoResponseHandler.Instance.deleteUserComplete = complete;
		KakaoResponseHandler.Instance.deleteUserError = error;
		KakaoParamBase param = new KakaoParamBase(KakaoAction.DeleteUser);
		KakaoNativeExtension.plugin.request(param);
	}
}
