using System.Collections.Generic;
using UnityEngine;

public class KakaoNativeExtension : MonoBehaviour
{
	protected static KakaoPluginBase plugin;

	private static KakaoNativeExtension _instance;

	public static KakaoNativeExtension Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType(typeof(KakaoNativeExtension)) as KakaoNativeExtension;
				if (!_instance)
				{
					GameObject gameObject = new GameObject();
					gameObject.name = "KakaoNativeExtension";
					_instance = gameObject.AddComponent(typeof(KakaoNativeExtension)) as KakaoNativeExtension;
					Object.DontDestroyOnLoad(_instance);
				}
			}
			return _instance;
		}
	}

	public void Init(KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.CompleteDelegate tokens)
	{
		string @string = PlayerPrefs.GetString(KakaoStringKeys.Commons.accessTokenKeyForPlayerPrefs);
		string string2 = PlayerPrefs.GetString(KakaoStringKeys.Commons.refreshTokenKeyForPlayerPrefs);
		UnityEngine.Debug.Log("!!!!" + @string);
		UnityEngine.Debug.Log("!!!!" + string2);
		KakaoResponseHandler.Instance.initComplete = complete;
		KakaoResponseHandler.Instance.tokens = tokens;
		if (plugin == null)
		{
			plugin = ScriptableObject.CreateInstance<KakaoPluginAndroid>();
		}
		KakaoParamBase param = new KakaoParamInit(@string, string2);
		plugin.request(param);
		updateTokenCache(@string, string2);
	}

	public void Authorized(KakaoResponseHandler.AuthorizedDelegate complete)
	{
		UnityEngine.Debug.Log("Authorized");
		KakaoResponseHandler.Instance.authorized = complete;
		KakaoParamBase param = new KakaoParamBase(KakaoAction.Authorized);
		plugin.request(param);
	}

	public void Login(KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		UnityEngine.Debug.Log("Login");
		KakaoResponseHandler.Instance.loginComplete = complete;
		KakaoResponseHandler.Instance.loginError = error;
		KakaoParamBase param = new KakaoParamBase(KakaoAction.Login);
		plugin.request(param);
	}

	public void LocalUser(KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		UnityEngine.Debug.Log("LocalUser");
		KakaoResponseHandler.Instance.localUserComplete = complete;
		KakaoResponseHandler.Instance.localUserError = error;
		KakaoParamBase param = new KakaoParamBase(KakaoAction.LocalUser);
		plugin.request(param);
	}

	public void ShowMessageBlockDialog(KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		UnityEngine.Debug.Log("ShowMessageBlockDialog");
		KakaoResponseHandler.Instance.showMessageBlockComplete = complete;
		KakaoResponseHandler.Instance.showMessageBlockError = error;
		KakaoParamBase param = new KakaoParamBase(KakaoAction.ShowMessageBlockDialog);
		plugin.request(param);
	}

	public void Friends(KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		UnityEngine.Debug.Log("Friends");
		KakaoResponseHandler.Instance.friendsComplete = complete;
		KakaoResponseHandler.Instance.friendsError = error;
		KakaoParamBase param = new KakaoParamBase(KakaoAction.Friends);
		plugin.request(param);
	}

	public void SendLinkMessage(string templateId, string receiverId, string imagePath, string executeUrl, Dictionary<string, string> metaInfo, KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		UnityEngine.Debug.Log("SendLinkMessage");
		KakaoResponseHandler.Instance.sendLinkMessageComplete = complete;
		KakaoResponseHandler.Instance.sendLinkMessageError = error;
		KakaoParamBase param = new KakaoParamLinkMessage(templateId, receiverId, imagePath, executeUrl, metaInfo);
		plugin.request(param);
	}

	public void PostToKakaoStory(string message, string imagePath, string executeUrl, KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		UnityEngine.Debug.Log("PostToKakaoStory");
		KakaoResponseHandler.Instance.postStoryComplete = complete;
		KakaoResponseHandler.Instance.postStoryError = error;
		KakaoParamBase param = new KakaoParamStory(message, imagePath, executeUrl);
		plugin.request(param);
	}

	public void Logout(KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		UnityEngine.Debug.Log("Logout");
		KakaoResponseHandler.Instance.logoutComplete = complete;
		KakaoResponseHandler.Instance.logoutError = error;
		KakaoParamBase param = new KakaoParamBase(KakaoAction.Logout);
		plugin.request(param);
	}

	public void Unregister(KakaoResponseHandler.CompleteDelegate complete, KakaoResponseHandler.ErrorDelegate error)
	{
		UnityEngine.Debug.Log("Unregister");
		KakaoResponseHandler.Instance.unregisterComplete = complete;
		KakaoResponseHandler.Instance.unregisterError = error;
		KakaoParamBase param = new KakaoParamBase(KakaoAction.Unregister);
		plugin.request(param);
	}

	public void ShowAlertMessage(string message)
	{
		UnityEngine.Debug.Log("ShowAlertMessage");
		KakaoParamBase param = new KakaoParamShowAlertMessage(message);
		plugin.request(param);
	}

	public static void updateTokenCache(string accessToken, string refreshToken)
	{
		if (accessToken != null && refreshToken != null && accessToken.Length > 0 && refreshToken.Length > 0)
		{
			PlayerPrefs.SetString(KakaoStringKeys.Commons.accessTokenKeyForPlayerPrefs, accessToken);
			PlayerPrefs.SetString(KakaoStringKeys.Commons.refreshTokenKeyForPlayerPrefs, refreshToken);
			UnityEngine.Debug.Log("Archived tokens.");
		}
		else
		{
			PlayerPrefs.DeleteKey(KakaoStringKeys.Commons.accessTokenKeyForPlayerPrefs);
			PlayerPrefs.DeleteKey(KakaoStringKeys.Commons.refreshTokenKeyForPlayerPrefs);
			UnityEngine.Debug.Log("Token is invaldate, Because logout or unregister or expired token.");
		}
		PlayerPrefs.Save();
	}

	public static bool hasValidTokenCache()
	{
		string @string = PlayerPrefs.GetString(KakaoStringKeys.Commons.accessTokenKeyForPlayerPrefs, null);
		string string2 = PlayerPrefs.GetString(KakaoStringKeys.Commons.refreshTokenKeyForPlayerPrefs, null);
		if (@string != null && string2 != null && @string.Length > 0 && string2.Length > 0)
		{
			return true;
		}
		return false;
	}

	public static string getAccessToken()
	{
		return PlayerPrefs.GetString(KakaoStringKeys.Commons.accessTokenKeyForPlayerPrefs, null);
	}
}
