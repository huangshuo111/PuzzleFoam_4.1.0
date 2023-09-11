using System.Collections;
using UnityEngine;

public class TimeStop : MonoBehaviour
{
	private const int timeStopNegaCount = 3;

	private const float oneNegaWaitTime = 0.2f;

	private NegaEffect negaEffect_;

	public StagePause stagePause_;

	private void OnApplicationPause(bool pause)
	{
		Debug.Log("OnApplicationPause:TimeStop");
		stopNegaRoutine();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void setup(StagePause stagePause)
	{
		stagePause_ = stagePause;
		negaEffect_ = Camera.main.GetComponent<NegaEffect>();
	}

	public void stopNegaRoutine()
	{
		StopAllCoroutines();
		negaEffect_.enabled = false;
	}

	private IEnumerator negaRoutine()
	{
		Sound.Instance.playSe(Sound.eSe.SE_525_tokeishutsugen);
		negaEffect_.enabled = false;
		base.GetComponent<Animation>()["TimeStop_anm"].speed = 0f;
		for (int i = 0; i < 3; i++)
		{
			yield return new WaitForSeconds(0.2f);
			yield return stagePause_.sync();
			negaEffect_.enabled = true;
			yield return new WaitForSeconds(0.2f);
			yield return stagePause_.sync();
			negaEffect_.enabled = false;
		}
		negaEffect_.enabled = false;
		base.GetComponent<Animation>()["TimeStop_anm"].speed = 1f;
	}
}
