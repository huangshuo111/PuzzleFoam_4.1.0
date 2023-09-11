using System.Collections;
using UnityEngine;

public class PopupScore : MonoBehaviour
{
	private Transform me;

	private Animation anim;

	private TweenAlpha[] tweenAlphas;

	private GameObject scoreBase;

	private GameObject scoreRoot;

	private Animation coinAnim;

	private GameObject coinBase;

	private GameObject coinRoot;

	public StagePause stagePause;

	public Vector3 startPopup(int score, Vector3 pos, Vector3 offset)
	{
		if (score == 0)
		{
			return offset;
		}
		if (scoreRoot != null)
		{
			Object.Destroy(scoreRoot);
		}
		base.gameObject.SetActive(true);
		if (me == null)
		{
			me = base.transform;
			anim = GetComponentInChildren<Animation>();
			scoreBase = anim.transform.Find("score_number").gameObject;
			scoreBase.SetActive(false);
		}
		scoreRoot = new GameObject("scoreRoot");
		Utility.setParent(scoreRoot, anim.transform, true);
		Vector3 position = me.position;
		position.x = pos.x;
		position.y = pos.y;
		me.position = position;
		me.localPosition += offset;
		position = me.localPosition;
		if (position.x > 70f)
		{
			position.x = 70f;
		}
		else if (position.x < -310f)
		{
			position.x = -310f;
		}
		float num = 310f + NGUIUtilScalableUIRoot.GetOffsetY(true).y;
		if (position.y > num)
		{
			position.y = num;
		}
		me.localPosition = position;
		int length = score.ToString().Length;
		int num2 = 1;
		Transform transform = null;
		float num3 = 0f;
		for (int i = 0; i < length; i++)
		{
			GameObject gameObject = Object.Instantiate(scoreBase) as GameObject;
			gameObject.name = (i + 1).ToString("00");
			Utility.setParent(gameObject, scoreRoot.transform, true);
			gameObject.SetActive(true);
			Transform transform2 = gameObject.transform;
			UISprite component = transform2.GetComponent<UISprite>();
			component.spriteName = "game_score_number_0" + score % (num2 * 10) / num2;
			component.MakePixelPerfect();
			num2 *= 10;
			if (i > 0)
			{
				Vector3 localPosition = transform.localPosition;
				localPosition.x -= (transform.localScale.x + transform2.localScale.x) * 0.5f;
				transform2.localPosition = localPosition;
				num3 += transform2.localScale.x;
			}
			else
			{
				transform2.localPosition = scoreBase.transform.localPosition;
			}
			transform = transform2;
		}
		scoreRoot.transform.localPosition += Vector3.right * num3;
		anim.Play();
		tweenAlphas = GetComponentsInChildren<TweenAlpha>();
		TweenAlpha[] array = tweenAlphas;
		foreach (TweenAlpha tweenAlpha in array)
		{
			tweenAlpha.Reset();
			tweenAlpha.Play(true);
		}
		StartCoroutine(waitRoutine());
		offset.y -= 35f;
		return offset;
	}

	public Vector3 startPopupWithCoin(int score, int coin, Vector3 pos, Vector3 offset)
	{
		if (score == 0)
		{
			return offset;
		}
		if (scoreRoot != null)
		{
			Object.Destroy(scoreRoot);
		}
		if (coinRoot != null)
		{
			Object.Destroy(coinRoot);
		}
		base.gameObject.SetActive(true);
		if (me == null)
		{
			me = base.transform;
			anim = GetComponentInChildren<Animation>();
			scoreBase = anim.transform.Find("score_number").gameObject;
			scoreBase.SetActive(false);
		}
		scoreRoot = new GameObject("scoreRoot");
		Utility.setParent(scoreRoot, anim.transform, true);
		Vector3 position = me.position;
		position.x = pos.x;
		position.y = pos.y;
		me.position = position;
		me.localPosition += offset;
		position = me.localPosition;
		if (position.x > 70f)
		{
			position.x = 70f;
		}
		else if (position.x < -310f)
		{
			position.x = -310f;
		}
		float num = 310f + NGUIUtilScalableUIRoot.GetOffsetY(true).y;
		if (position.y > num)
		{
			position.y = num;
		}
		me.localPosition = position;
		int length = score.ToString().Length;
		int num2 = 1;
		Transform transform = null;
		float num3 = 0f;
		for (int i = 0; i < length; i++)
		{
			GameObject gameObject = Object.Instantiate(scoreBase) as GameObject;
			gameObject.name = (i + 1).ToString("00");
			Utility.setParent(gameObject, scoreRoot.transform, true);
			gameObject.SetActive(true);
			Transform transform2 = gameObject.transform;
			UISprite component = transform2.GetComponent<UISprite>();
			component.spriteName = "game_score_number_0" + score % (num2 * 10) / num2;
			component.MakePixelPerfect();
			num2 *= 10;
			if (i > 0)
			{
				Vector3 localPosition = transform.localPosition;
				localPosition.x -= (transform.localScale.x + transform2.localScale.x) * 0.5f;
				transform2.localPosition = localPosition;
				num3 += transform2.localScale.x;
			}
			else
			{
				transform2.localPosition = scoreBase.transform.localPosition;
			}
			transform = transform2;
		}
		scoreRoot.transform.localPosition += Vector3.right * num3;
		anim.Play();
		tweenAlphas = GetComponentsInChildren<TweenAlpha>();
		TweenAlpha[] array = tweenAlphas;
		foreach (TweenAlpha tweenAlpha in array)
		{
			tweenAlpha.Reset();
			tweenAlpha.Play(true);
		}
		coinAnim = me.Find("erasure_coin_bg").GetComponent<Animation>();
		coinBase = coinAnim.transform.Find("score_number").gameObject;
		coinBase.SetActive(false);
		coinRoot = new GameObject("scoreRoot");
		Utility.setParent(coinRoot, coinAnim.transform, true);
		int length2 = coin.ToString().Length;
		int num4 = 1;
		Transform transform3 = null;
		float num5 = 0f;
		for (int k = 0; k < length2; k++)
		{
			GameObject gameObject2 = Object.Instantiate(coinBase) as GameObject;
			gameObject2.name = (k + 1).ToString("00");
			Utility.setParent(gameObject2, coinRoot.transform, true);
			gameObject2.SetActive(true);
			Transform transform4 = gameObject2.transform;
			UISprite component2 = transform4.GetComponent<UISprite>();
			component2.spriteName = "game_score_number_0" + coin % (num4 * 10) / num4;
			component2.MakePixelPerfect();
			num4 *= 10;
			if (k > 0)
			{
				Vector3 localPosition2 = transform3.localPosition;
				localPosition2.x -= (transform3.localScale.x + transform4.localScale.x) * 0.5f;
				transform4.localPosition = localPosition2;
				num5 += transform4.localScale.x;
			}
			else
			{
				transform4.localPosition = coinBase.transform.localPosition;
				Vector3 localPosition3 = transform4.localPosition;
				localPosition3.x += transform4.localScale.x * (float)length2 * 0.5f;
				transform4.localPosition = localPosition3;
			}
			transform3 = transform4;
		}
		coinAnim.Play();
		tweenAlphas = me.Find("erasure_coin_bg").GetComponentsInChildren<TweenAlpha>();
		TweenAlpha[] array2 = tweenAlphas;
		foreach (TweenAlpha tweenAlpha2 in array2)
		{
			tweenAlpha2.Reset();
			tweenAlpha2.Play(true);
		}
		StartCoroutine(waitRoutine());
		offset.y -= 35f;
		return offset;
	}

	private IEnumerator waitRoutine()
	{
		while (anim.isPlaying)
		{
			yield return stagePause.sync();
		}
		Object.Destroy(scoreRoot);
		scoreRoot = null;
		base.gameObject.SetActive(false);
	}
}
