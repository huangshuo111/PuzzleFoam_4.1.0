using System.Collections;
using UnityEngine;

public class HeartEffect : MonoBehaviour
{
	[SerializeField]
	private GameObject HeartRoot;

	[SerializeField]
	private TweenGroup Tweens;

	[SerializeField]
	private GameObject Heart;

	[SerializeField]
	private float MoveTime = 2f;

	private ParticleSystem particle_;

	private void Awake()
	{
		Tweens.gameObject.SetActive(false);
		particle_ = Heart.GetComponentInChildren<ParticleSystem>();
	}

	public IEnumerator play(Vector3 target)
	{
		Vector3 from = HeartRoot.transform.localPosition;
		Heart.SetActive(true);
		particle_.Play();
		iTween.MoveTo(HeartRoot, iTween.Hash("x", target.x, "y", target.y, "islocal", true, "time", MoveTime));
		while (HeartRoot.GetComponent<iTween>() != null)
		{
			yield return 0;
		}
		Sound.Instance.playSe(Sound.eSe.SE_326_heart_break);
		particle_.Stop();
		particle_.Clear();
		Heart.SetActive(false);
		Tweens.gameObject.SetActive(true);
		Tweens.Play();
		yield return new WaitForSeconds(Tweens.getEndTime());
		Tweens.gameObject.SetActive(false);
		HeartRoot.transform.localPosition = from;
		while (Sound.Instance.isPlayingSe(Sound.eSe.SE_326_heart_break))
		{
			yield return 0;
		}
	}
}
