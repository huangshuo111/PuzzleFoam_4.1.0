using System.Collections.Generic;
using UnityEngine;

public class KakaoMessagesView : KakaoBaseView
{
	private Vector2 scrollPosition = Vector2.zero;

	public KakaoMessagesView()
		: base(KakaoViewType.Messages)
	{
	}

	public override void Render()
	{
		if (GUI.Button(new Rect(0f, 0f, Screen.width, KakaoBaseView.buttonHeight), "Go to Leaderboard"))
		{
			KakaoSample.Instance.moveToView(KakaoViewType.Leaderboard);
		}
		int count = KakaoGameMessages.Instance.gameMessages.Count;
		int num = Screen.width - 30;
		scrollPosition = GUI.BeginScrollView(new Rect(0f, KakaoBaseView.buttonHeight, Screen.width - 10, Screen.height - KakaoBaseView.buttonHeight), scrollPosition, new Rect(0f, 0f, num, count * KakaoBaseView.buttonHeight));
		int num2 = 0;
		KakaoGameMessages.GameMessage gameMessage = null;
		foreach (KeyValuePair<string, KakaoGameMessages.GameMessage> gameMessage2 in KakaoGameMessages.Instance.gameMessages)
		{
			gameMessage = gameMessage2.Value;
			if (gameMessage != null)
			{
				if (GUI.Button(new Rect(0f, num2, num, KakaoBaseView.buttonHeight), string.Format("{0} : {1}", gameMessage.senderNickName, (gameMessage.message != null) ? gameMessage.message : "empty message")))
				{
					KakaoLeaderboardExtension.Instance.acceptGameMessage(gameMessage.messageId, onAcceptGameMessageComplete, onAcceptGameMessageError);
				}
				num2 += KakaoBaseView.buttonHeight;
			}
		}
		GUI.EndScrollView();
	}

	private void onAcceptGameMessageComplete()
	{
		KakaoNativeExtension.Instance.ShowAlertMessage("Completed accept message!");
	}

	private void onAcceptGameMessageError(string status, string message)
	{
		showAlertErrorMessage(status, message);
	}
}
