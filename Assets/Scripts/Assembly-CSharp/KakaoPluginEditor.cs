public class KakaoPluginEditor : KakaoPluginBase
{
	public override void request(KakaoParamBase param)
	{
		Debug.Log(param.getParamString());
		switch (param.getAction())
		{
		case KakaoAction.Init:
			if (KakaoResponseHandler.Instance.initComplete != null)
			{
				KakaoResponseHandler.Instance.initComplete();
			}
			break;
		case KakaoAction.Authorized:
			if (KakaoResponseHandler.Instance.authorized != null)
			{
				KakaoResponseHandler.Instance.authorized(KakaoNativeExtension.hasValidTokenCache());
			}
			break;
		case KakaoAction.Login:
			if (KakaoResponseHandler.Instance.tokens != null)
			{
				KakaoNativeExtension.updateTokenCache("test_access_token", "test_refresh_token");
				KakaoResponseHandler.Instance.tokens();
				if (KakaoResponseHandler.Instance.loginComplete != null)
				{
					KakaoResponseHandler.Instance.loginComplete();
				}
			}
			break;
		case KakaoAction.LocalUser:
			if (KakaoResponseHandler.Instance.localUserComplete != null)
			{
				KakaoResponseHandler.Instance.localUserComplete();
			}
			break;
		case KakaoAction.Friends:
			if (KakaoResponseHandler.Instance.friendsComplete != null)
			{
				KakaoResponseHandler.Instance.friendsComplete();
			}
			break;
		case KakaoAction.SendLinkMessage:
			break;
		case KakaoAction.PostToKakaoStory:
			if (KakaoResponseHandler.Instance.postStoryComplete != null)
			{
				KakaoResponseHandler.Instance.postStoryComplete();
			}
			break;
		case KakaoAction.Logout:
			if (KakaoResponseHandler.Instance.tokens != null)
			{
				KakaoNativeExtension.updateTokenCache(null, null);
				KakaoResponseHandler.Instance.tokens();
				if (KakaoResponseHandler.Instance.logoutComplete != null)
				{
					KakaoResponseHandler.Instance.logoutComplete();
				}
			}
			break;
		case KakaoAction.Unregister:
			if (KakaoResponseHandler.Instance.tokens != null)
			{
				KakaoNativeExtension.updateTokenCache(null, null);
				KakaoResponseHandler.Instance.tokens();
				if (KakaoResponseHandler.Instance.unregisterComplete != null)
				{
					KakaoResponseHandler.Instance.unregisterComplete();
				}
			}
			break;
		case KakaoAction.ShowAlertMessage:
			Debug.Log("KakaoAction.ShowAlertMessage");
			break;
		case KakaoAction.LoadGameInfo:
			break;
		case KakaoAction.LoadGameUserInfo:
			break;
		case KakaoAction.UpdateUser:
			break;
		case KakaoAction.UseHeart:
			break;
		case KakaoAction.UpdateResult:
			break;
		case KakaoAction.UpdateMultipleResults:
			break;
		case KakaoAction.LoadLeaderboard:
			break;
		case KakaoAction.BlockMessage:
			break;
		case KakaoAction.LoadGameFriends:
			break;
		case KakaoAction.LoadGameMessages:
			break;
		case KakaoAction.AcceptGameMessage:
			break;
		case KakaoAction.AcceptAllGameMessages:
			break;
		case KakaoAction.DeleteUser:
			break;
		case KakaoAction.ShowMessageBlockDialog:
		case KakaoAction.Token:
		case KakaoAction.InvitationEvent:
		case KakaoAction.InvitationStates:
		case KakaoAction.InvitationHost:
		case KakaoAction.SendLinkGameMessage:
		case KakaoAction.SendInviteLinkGameMessage:
			break;
		}
	}
}
