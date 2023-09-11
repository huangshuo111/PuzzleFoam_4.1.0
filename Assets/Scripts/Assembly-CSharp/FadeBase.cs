using System.Collections;
using UnityEngine;

public abstract class FadeBase : MonoBehaviour
{
	public float FadeTime = 0.3f;

	protected GameObject fadeObject_;

	protected Material fadeMaterial_;

	protected float fromAlpha_;

	protected float toAlpha_;

	protected float fadeDuration_;

	protected float startTime_;

	protected bool bFade_;

	protected virtual void Awake()
	{
		UITexture componentInChildren = base.gameObject.GetComponentInChildren<UITexture>();
		fadeMaterial_ = new Material(componentInChildren.material);
		componentInChildren.material = fadeMaterial_;
		fadeObject_ = componentInChildren.gameObject;
	}

	protected abstract IEnumerator updateFade();

	public IEnumerator startFade(float from, float to, float duration)
	{
		if (!bFade_)
		{
			fromAlpha_ = from;
			toAlpha_ = to;
			fadeDuration_ = duration;
			startTime_ = Time.time;
			yield return StartCoroutine(updateFade());
		}
	}

	public IEnumerator startFadeIn()
	{
		yield return StartCoroutine(startFade(1f, 0f, FadeTime));
	}

	public IEnumerator startFadeOut()
	{
		yield return StartCoroutine(startFade(0f, 1f, FadeTime));
	}

	public bool isFade()
	{
		return bFade_;
	}
}
