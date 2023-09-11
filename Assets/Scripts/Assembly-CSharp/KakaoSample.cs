using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KakaoSample : MonoBehaviour
{
	private static KakaoSample _instance;

	private List<KakaoBaseView> viewList = new List<KakaoBaseView>();

	private KakaoBaseView currentView;

	public Camera currentCamera;

	public static KakaoSample Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType(typeof(KakaoSample)) as KakaoSample;
				if (!_instance)
				{
					GameObject gameObject = new GameObject();
					gameObject.name = "KakaoSample";
					_instance = gameObject.AddComponent(typeof(KakaoSample)) as KakaoSample;
					Object.DontDestroyOnLoad(_instance);
				}
			}
			return _instance;
		}
	}

	public KakaoBaseView getCurrentView()
	{
		return currentView;
	}

	public void moveToView(KakaoViewType type)
	{
		Debug.Log("KakaoSample - moveToView");
		foreach (KakaoBaseView view in viewList)
		{
			if (view.type == type)
			{
				currentView = view;
				return;
			}
		}
		KakaoBaseView kakaoBaseView = null;
		switch (type)
		{
		case KakaoViewType.Login:
			kakaoBaseView = new KakaoLoginView();
			break;
		case KakaoViewType.Friends:
			kakaoBaseView = new KakaoFriendsView();
			break;
		case KakaoViewType.Main:
			kakaoBaseView = new KakaoMainView();
			break;
		case KakaoViewType.Leaderboard:
			kakaoBaseView = new KakaoLeaderboardView();
			break;
		case KakaoViewType.Messages:
			kakaoBaseView = new KakaoMessagesView();
			break;
		case KakaoViewType.GameFriends:
			kakaoBaseView = new KakaoGameFriendsView();
			break;
		case KakaoViewType.InvitationTracking:
			kakaoBaseView = new KakaoInvitationTrackingView();
			break;
		}
		if (kakaoBaseView != null)
		{
			viewList.Add(kakaoBaseView);
			currentView = kakaoBaseView;
		}
	}

	private void Start()
	{
		KakaoNativeExtension.Instance.Init(onInitComplete, onTokens);
	}

	private void OnGUI()
	{
		if (viewList != null && currentView != null)
		{
			currentView.Render();
		}
	}

	private void onAuthorized(bool _authorized)
	{
		if (_authorized)
		{
			KakaoNativeExtension.Instance.ShowAlertMessage("Move to Main, Because Already finished Login Process!");
			Instance.moveToView(KakaoViewType.Main);
		}
		else
		{
			Instance.moveToView(KakaoViewType.Login);
		}
	}

	private void onInitComplete()
	{
		KakaoNativeExtension.Instance.Authorized(onAuthorized);
	}

	private void onTokens()
	{
		if (KakaoNativeExtension.hasValidTokenCache())
		{
			moveToView(KakaoViewType.Main);
		}
		else
		{
			moveToView(KakaoViewType.Login);
		}
	}

	public void captureScreenToImage(string fileNameWithPath)
	{
		if (currentCamera == null)
		{
			Debug.Log("[Kakao Unity Sample] currentCamera is null. please assign camera instance to the currentCamera value.");
			return;
		}
		RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
		currentCamera.targetTexture = renderTexture;
		Texture2D texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		currentCamera.Render();
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
		currentCamera.targetTexture = null;
		RenderTexture.active = null;
		Object.Destroy(renderTexture);
		byte[] bytes = texture2D.EncodeToPNG();
		File.WriteAllBytes(fileNameWithPath, bytes);
	}
}
