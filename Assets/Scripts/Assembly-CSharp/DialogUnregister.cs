using System.Collections;
using LitJson;
using Network;
using UnityEngine;

public class DialogUnregister : DialogBase
{
	private void Awake()
	{
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "ConfirmButton":
			Constant.SoundUtil.PlayDecideSE();
			if (SNSCore.IsAuthorize)
			{
				SNSCore.Instance.Unregister();
				while (SNSCore.IsAuthorize)
				{
					yield return null;
				}
			}
			yield return dialogManager_.StartCoroutine(SendUnregister());
			PlayerPrefs.SetInt("PolicyFlagSkonec", 0);
			while (PlayerPrefs.GetInt("PolicyFlagSkonec") != 0)
			{
				yield return null;
			}
			PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
			GlobalData.Instance.LineID = 0L;
			Part_Title.bStartGame = false;
			KakaoCore.Instance.ClearCache();
			partManager_.gotoTitle();
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	public IEnumerator SendUnregister()
	{
		Input.enable = false;
		yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(API.Unregister, true, false));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			Input.enable = true;
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		UnregisterData data = JsonMapper.ToObject<UnregisterData>(www.text);
		if (data.resultCode != 0)
		{
			Debug.Log("?????");
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			Input.enable = true;
		}
		else
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			Input.enable = true;
		}
	}
}
