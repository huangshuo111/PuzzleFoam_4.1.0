using System.Collections;
using UnityEngine;

public class CoinEffect : MonoBehaviour
{
	[SerializeField]
	private GameObject CoinRoot;

	[SerializeField]
	private TweenGroup Tweens;

	[SerializeField]
	private GameObject Coin;

	[SerializeField]
	private float MoveTime = 2f;

	private ParticleSystem particle_;

	private void Awake()
	{
		Tweens.gameObject.SetActive(false);
		particle_ = Coin.GetComponentInChildren<ParticleSystem>();
	}

	public IEnumerator play(Vector3 from, Vector3 target)
	{
		target.z -= 0.1f;
		from.z = target.z;
		CoinRoot.transform.position = from;
		Coin.SetActive(true);
		particle_.Play();
		iTween.MoveTo(CoinRoot, iTween.Hash("x", target.x, "y", target.y, "islocal", false, "time", MoveTime));
		while (CoinRoot.GetComponent<iTween>() != null)
		{
			yield return 0;
		}
		Sound.Instance.playSe(Sound.eSe.SE_326_heart_break);
		particle_.Stop();
		particle_.Clear();
		Coin.SetActive(false);
		Tweens.gameObject.SetActive(true);
		Tweens.Play();
		yield return new WaitForSeconds(Tweens.getEndTime());
		Tweens.gameObject.SetActive(false);
		CoinRoot.transform.position = from;
		while (Sound.Instance.isPlayingSe(Sound.eSe.SE_326_heart_break))
		{
			yield return 0;
		}
	}
}
