using System.Collections;
using UnityEngine;

public class logo : MonoBehaviour
{
	[SerializeField]
	private FadeMng FadeManager;

	[SerializeField]
	private bool PlayerPrefabsReset_;

	[SerializeField]
	private GameObject Skonec_logo_;

	[SerializeField]
	private float Skonec_WaitTime = 0.5f;

	[SerializeField]
	private GameObject Taito_logo_;

	[SerializeField]
	private float Taito_WaitTime = 0.5f;

	private void Awake()
	{
		GKUnityPluginController.Instance.GK_RegistGCM();
	}

	private IEnumerator Start()
	{
		PlayerPrefs.SetInt("KakaoIntroSound", PlayerPrefs.GetInt("KakaoIntroSound"));
		if (PlayerPrefs.GetInt("KakaoIntroSound") == 0)
		{
			Handheld.PlayFullScreenMovie("android/Kaoaogame_H_720_1280.mp4", new Color(255f, 217f, 0f), FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.Fill);
		}
		else
		{
			Handheld.PlayFullScreenMovie("android/Kaoaogame_H_720_1280_Mute.mp4", new Color(255f, 217f, 0f), FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.Fill);
		}
		GKUnityPluginController.CallAndroidFunc("com.gumikorea.framework.GCM.LocalNotificationReceiver", "delete");
		SNSCore.Instance.Init();
		Skonec_logo_.SetActive(true);
		Taito_logo_.SetActive(false);
		yield return StartCoroutine(FadeManager.startFadeIn(FadeMng.eType.AllMask));
		while (FadeManager.isFade())
		{
			yield return 0;
		}
		yield return new WaitForSeconds(Skonec_WaitTime);
		yield return StartCoroutine(FadeManager.startFadeOut(FadeMng.eType.AllMask));
		while (FadeManager.isFade())
		{
			yield return 0;
		}
		Skonec_logo_.SetActive(false);
		Taito_logo_.SetActive(true);
		yield return StartCoroutine(FadeManager.startFadeIn(FadeMng.eType.AllMask));
		while (FadeManager.isFade())
		{
			yield return 0;
		}
		yield return new WaitForSeconds(Taito_WaitTime);
		yield return StartCoroutine(FadeManager.startFadeOut(FadeMng.eType.AllMask));
		while (FadeManager.isFade())
		{
			yield return 0;
		}
		Application.LoadLevel("main");
	}
}
