using UnityEngine;

public class KakaoInvitationTrackingView : KakaoBaseView
{
	public KakaoInvitationTrackingView()
		: base(KakaoViewType.InvitationTracking)
	{
	}

	public override void Render()
	{
		int num = 0;
		if (GUI.Button(new Rect(0f, num, Screen.width, KakaoBaseView.buttonHeight), "Go to Main"))
		{
			KakaoSample.Instance.moveToView(KakaoViewType.Main);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, Screen.width, KakaoBaseView.buttonHeight), "Invitation event"))
		{
			KakaoInvitationTrackingExtension.Instance.invitationEvent(onInvitationEventComplete, onInvitationEventError);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, Screen.width, KakaoBaseView.buttonHeight), "Invitation states "))
		{
			KakaoInvitationTrackingExtension.Instance.invitationStates(onInvitationStatesComplete, onInvitationStatesError);
		}
		if (GUI.Button(new Rect(0f, num += KakaoBaseView.buttonHeight, Screen.width, KakaoBaseView.buttonHeight), "Invited host"))
		{
			KakaoInvitationTrackingExtension.Instance.invitationHost(onInvitationHostComplete, onInvitationHostError);
		}
	}

	public void onInvitationEventComplete()
	{
		KakaoInvitationEvent.Instance.printForDebugging();
	}

	public void onInvitationEventError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}

	public void onInvitationStatesComplete()
	{
		KakaoInvitationStates.Instance.printForDebugging();
	}

	public void onInvitationStatesError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}

	public void onInvitationHostComplete()
	{
		KakaoInvitationHost.Instance.printForDebugging();
	}

	public void onInvitationHostError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}
}
