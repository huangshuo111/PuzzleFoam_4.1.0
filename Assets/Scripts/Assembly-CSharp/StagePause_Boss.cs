using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePause_Boss : MonoBehaviour
{
	private bool isPause;

	private Animation[] pauseAnimations;

	private tk2dAnimatedSprite[] pauseAnimatedSprits;

	private UITweener[] pauseTweeners;

	private Rigidbody[] rigidbodies;

	private Vector3[] rigidbodyVelocities;

	private ParticleSystem[] particleSystems;

	private UISpriteAnimationEx[] spriteAnimations;

	public float diffTime;

	private Part_BossStage partBoss;

	public bool pause
	{
		get
		{
			return isPause;
		}
		set
		{
			if ((partBoss.state < Part_BossStage.eState.Clear || !value) && isPause != value)
			{
				pauseAnimation(value);
				isPause = value;
			}
		}
	}

	private void Awake()
	{
		partBoss = GetComponent<Part_BossStage>();
	}

	private void pauseAnimation(bool pauseValue)
	{
		if (pauseValue)
		{
			pauseAnimations = Object.FindObjectsOfType(typeof(Animation)) as Animation[];
			Animation[] array = pauseAnimations;
			foreach (Animation animation in array)
			{
				animation[animation.clip.name].speed = 0f;
			}
			pauseAnimatedSprits = Object.FindObjectsOfType(typeof(tk2dAnimatedSprite)) as tk2dAnimatedSprite[];
			tk2dAnimatedSprite[] array2 = pauseAnimatedSprits;
			foreach (tk2dAnimatedSprite tk2dAnimatedSprite2 in array2)
			{
				tk2dAnimatedSprite2.Pause();
			}
			iTween.Pause();
			pauseTweeners = Object.FindObjectsOfType(typeof(UITweener)) as UITweener[];
			List<UITweener> list = new List<UITweener>();
			UITweener[] array3 = pauseTweeners;
			foreach (UITweener uITweener in array3)
			{
				if (uITweener.enabled)
				{
					list.Add(uITweener);
					uITweener.enabled = false;
				}
			}
			pauseTweeners = list.ToArray();
			rigidbodies = Object.FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
			rigidbodyVelocities = new Vector3[rigidbodies.Length];
			for (int l = 0; l < rigidbodies.Length; l++)
			{
			}
			particleSystems = Object.FindObjectsOfType(typeof(ParticleSystem)) as ParticleSystem[];
			ParticleSystem[] array4 = particleSystems;
			foreach (ParticleSystem particleSystem in array4)
			{
				particleSystem.Pause();
			}
			spriteAnimations = Object.FindObjectsOfType(typeof(UISpriteAnimationEx)) as UISpriteAnimationEx[];
			UISpriteAnimationEx[] array5 = spriteAnimations;
			foreach (UISpriteAnimationEx uISpriteAnimationEx in array5)
			{
				uISpriteAnimationEx.enabled = false;
			}
			if (partBoss != null)
			{
				diffTime = Time.time - partBoss.startTime;
			}
			return;
		}
		Animation[] array6 = pauseAnimations;
		foreach (Animation animation2 in array6)
		{
			if (animation2 != null)
			{
				animation2[animation2.clip.name].speed = 1f;
			}
		}
		tk2dAnimatedSprite[] array7 = pauseAnimatedSprits;
		foreach (tk2dAnimatedSprite tk2dAnimatedSprite3 in array7)
		{
			if (tk2dAnimatedSprite3 != null)
			{
				tk2dAnimatedSprite3.Resume();
			}
		}
		iTween.Resume();
		UITweener[] array8 = pauseTweeners;
		foreach (UITweener uITweener2 in array8)
		{
			if (uITweener2 != null)
			{
				uITweener2.enabled = true;
			}
		}
		for (int num4 = 0; num4 < rigidbodies.Length; num4++)
		{
			if (rigidbodies[num4] != null)
			{
				rigidbodies[num4].WakeUp();
			}
		}
		ParticleSystem[] array9 = particleSystems;
		foreach (ParticleSystem particleSystem2 in array9)
		{
			if (particleSystem2 != null)
			{
				particleSystem2.Play();
			}
		}
		UISpriteAnimationEx[] array10 = spriteAnimations;
		foreach (UISpriteAnimationEx uISpriteAnimationEx2 in array10)
		{
			if (uISpriteAnimationEx2 != null)
			{
				uISpriteAnimationEx2.enabled = true;
			}
		}
		if (partBoss != null)
		{
			partBoss.startTime = Time.time - diffTime;
		}
	}

	public Coroutine sync()
	{
		return StartCoroutine(pauseRoutine());
	}

	public IEnumerator pauseRoutine()
	{
		while (isPause)
		{
			yield return null;
		}
		yield return null;
	}
}
