using System.Collections;
using UnityEngine;

public class ReadyGo : MonoBehaviour
{
	[SerializeField]
	private GameObject Ready;

	[SerializeField]
	private GameObject Go;

	private float WaitTime = 1.3f;

	private void Awake()
	{
		Ready.SetActive(false);
		Go.SetActive(false);
	}

	public IEnumerator play(StagePause stagePause)
	{
		Sound.Instance.playSe(Sound.eSe.SE_102_Ready);
		Ready.SetActive(true);
		float startTime = Time.time;
		float pauseTime = 0f;
		while (true)
		{
			float pauseStartTime = Time.time;
			while (stagePause.pause)
			{
				yield return 0;
			}
			float diff = Time.time - pauseStartTime;
			pauseTime = ((diff != 0f) ? diff : pauseTime);
			if (!stagePause.pause && Time.time - startTime >= WaitTime + pauseTime)
			{
				break;
			}
			yield return 0;
		}
		Sound.Instance.playSe(Sound.eSe.SE_357_readygo);
		Sound.Instance.playSe(Sound.eSe.SE_103_Go);
		Go.SetActive(true);
		while (Go.GetComponent<Animation>().isPlaying)
		{
			yield return stagePause.sync();
		}
		Object.Destroy(Ready);
		Object.Destroy(Go);
	}

	public IEnumerator play(StagePause_Boss stagePause)
	{
		Sound.Instance.playSe(Sound.eSe.SE_102_Ready);
		Ready.SetActive(true);
		float startTime = Time.time;
		float pauseTime = 0f;
		while (true)
		{
			float pauseStartTime = Time.time;
			while (stagePause.pause)
			{
				yield return 0;
			}
			float diff = Time.time - pauseStartTime;
			pauseTime = ((diff != 0f) ? diff : pauseTime);
			if (!stagePause.pause && Time.time - startTime >= WaitTime + pauseTime)
			{
				break;
			}
			yield return 0;
		}
		Sound.Instance.playSe(Sound.eSe.SE_357_readygo);
		Sound.Instance.playSe(Sound.eSe.SE_103_Go);
		Go.SetActive(true);
		while (Go.GetComponent<Animation>().isPlaying)
		{
			yield return stagePause.sync();
		}
		Object.Destroy(Ready);
		Object.Destroy(Go);
	}
}
