using System.Collections;
using UnityEngine;

public class Spiderweb : GimmickBase
{
	private const float EXCLUSIVE_SQR_SIZE = 3600f;

	public int row;

	public int column;

	public tk2dAnimatedSprite sprite;

	public StagePause_Boss stagePause_;

	public Part_BossStage part_;

	public bool existNearBubble;

	public bool isActive = true;

	public IEnumerator SpiderwebInit()
	{
		yield return stagePause_.sync();
	}

	public void seachNear()
	{
	}

	public IEnumerator HitSpiderweb()
	{
		yield return stagePause_.sync();
	}

	public void PlayBubble()
	{
		isActive = true;
		base.gameObject.SetActive(true);
		sprite.Play("bubble_51");
		StartCoroutine(FadeIn());
	}

	public IEnumerator PlayBurst()
	{
		isActive = false;
		sprite.Play("burst_51");
		while (sprite.IsPlaying(sprite.CurrentClip))
		{
			yield return stagePause_.sync();
		}
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);
		base.gameObject.SetActive(false);
		yield return stagePause_.sync();
	}

	public IEnumerator FadeIn()
	{
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);
		float alpha = 0f;
		while (alpha >= 1f)
		{
			alpha += Time.deltaTime / 5f;
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
			yield return stagePause_.sync();
		}
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
	}

	public IEnumerator FadeOut()
	{
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
		float alpha = 0f;
		while (alpha <= 0f)
		{
			alpha -= Time.deltaTime;
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
			yield return stagePause_.sync();
		}
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);
	}
}
