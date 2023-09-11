using UnityEngine;

public class KakaoInvitationTrackingExtension : KakaoNativeExtension
{
	private static KakaoInvitationTrackingExtension _instance;

	public new static KakaoInvitationTrackingExtension Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType(typeof(KakaoInvitationTrackingExtension)) as KakaoInvitationTrackingExtension;
				if (!_instance)
				{
					GameObject gameObject = new GameObject();
					gameObject.name = "KakaoInvitationTrackingExtension";
					_instance = gameObject.AddComponent(typeof(KakaoInvitationTrackingExtension)) as KakaoInvitationTrackingExtension;
					Object.DontDestroyOnLoad(_instance);
				}
			}
			return _instance;
		}
	}

	public void invitationEvent(KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		KakaoParamBase param = new KakaoParamBase(KakaoAction.InvitationEvent);
		KakaoResponseHandler.Instance.invitationEventComplete = complete;
		KakaoResponseHandler.Instance.invitationEventError = error;
		KakaoNativeExtension.plugin.request(param);
	}

	public void invitationStates(KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		KakaoParamBase param = new KakaoParamBase(KakaoAction.InvitationStates);
		KakaoResponseHandler.Instance.invitationStatesComplete = complete;
		KakaoResponseHandler.Instance.invitationStatesError = error;
		KakaoNativeExtension.plugin.request(param);
	}

	public void invitationHost(KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		KakaoParamBase param = new KakaoParamBase(KakaoAction.InvitationHost);
		KakaoResponseHandler.Instance.invitationHostComplete = complete;
		KakaoResponseHandler.Instance.invitationHostError = error;
		KakaoNativeExtension.plugin.request(param);
	}
}
