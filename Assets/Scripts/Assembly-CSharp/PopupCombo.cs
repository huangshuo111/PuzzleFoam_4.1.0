using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupCombo : MonoBehaviour
{
	private Transform me;

	private Animation anim;

	private List<TweenAlpha> tweenAlphas = new List<TweenAlpha>();

	private GameObject countBase;

	private GameObject countRoot;

	private Transform bgBonus;

	private Vector3 bgBonusBasePos;

	private Color color1_5 = new Color(1f, 1f, 37f / 51f, 0f);

	private Color color6_10 = new Color(1f, 56f / 85f, 16f / 51f, 0f);

	private Color color11_ = new Color(1f, 16f / 51f, 16f / 51f, 0f);

	public StagePause stagePause;

	private GameObject remain;

	private UISprite skill_bg;

	private UISprite skill_num;

	public void startPopup(int count, Vector3 pos)
	{
		if (count < 2)
		{
			return;
		}
		if (countRoot != null)
		{
			Object.Destroy(countRoot);
		}
		base.gameObject.SetActive(true);
		if (base.transform.Find("combo_bonus_bg/Skill_nokori") != null)
		{
			if (remain == null)
			{
				remain = base.transform.Find("combo_bonus_bg/Skill_nokori").gameObject;
			}
			if (remain.activeSelf)
			{
				remain.SetActive(false);
			}
		}
		if (me == null)
		{
			me = base.transform;
			anim = GetComponentInChildren<Animation>();
			countBase = anim.transform.Find("score_number").gameObject;
			bgBonus = anim.transform.Find("bg_bonus");
			bgBonusBasePos = bgBonus.localPosition;
			countBase.SetActive(false);
		}
		countRoot = new GameObject("countRoot");
		Utility.setParent(countRoot, anim.transform, true);
		Vector3 position = me.position;
		position.x = pos.x;
		position.y = pos.y;
		me.position = position;
		Color color = ((count <= 5) ? color1_5 : ((count > 10) ? color11_ : color6_10));
		UISprite component = bgBonus.GetComponent<UISprite>();
		component.color = color;
		tweenAlphas.Clear();
		tweenAlphas.Add(bgBonus.GetComponent<TweenAlpha>());
		int length = count.ToString().Length;
		int num = 1;
		Transform transform = null;
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < length; i++)
		{
			GameObject gameObject = Object.Instantiate(countBase) as GameObject;
			gameObject.name = (i + 1).ToString("00");
			Utility.setParent(gameObject, countRoot.transform, true);
			gameObject.SetActive(true);
			Transform transform2 = gameObject.transform;
			UISprite component2 = transform2.GetComponent<UISprite>();
			component2.spriteName = "combo_num_0" + count % (num * 10) / num;
			component2.color = color;
			component2.MakePixelPerfect();
			num *= 10;
			if (i > 0)
			{
				Vector3 localPosition = transform.localPosition;
				localPosition.x -= (transform.localScale.x + transform2.localScale.x) * 0.5f;
				transform2.localPosition = localPosition;
				num2 += transform2.localScale.x;
				bgBonus.localPosition += new Vector3(transform2.localScale.x, 0f, 0f);
			}
			else
			{
				transform2.localPosition = countBase.transform.localPosition;
				bgBonus.localPosition = bgBonusBasePos;
			}
			num3 += transform2.localScale.x;
			transform = transform2;
			tweenAlphas.Add(gameObject.GetComponent<TweenAlpha>());
		}
		countRoot.transform.localPosition += Vector3.right * num2;
		me.localPosition += new Vector3((0f - (num3 + bgBonus.localScale.x + countBase.transform.localScale.x * 0.5f)) * 0.5f, 40f, 0f);
		position = me.localPosition;
		if (position.x > 64.75f)
		{
			position.x = 64.75f;
		}
		else if (position.x < -315.25f)
		{
			position.x = -315.25f;
		}
		float num4 = 350f + NGUIUtilScalableUIRoot.GetOffsetY(true).y;
		if (position.y > num4)
		{
			position.y = num4;
		}
		me.localPosition = position;
		anim.Play();
		float num5 = anim.clip.length;
		foreach (TweenAlpha tweenAlpha in tweenAlphas)
		{
			tweenAlpha.Reset();
			tweenAlpha.style = UITweener.Style.Once;
			tweenAlpha.delay = 1.3f;
			tweenAlpha.duration = 0.2f;
			tweenAlpha.Play(true);
			if (num5 < tweenAlpha.delay + tweenAlpha.duration)
			{
				num5 = tweenAlpha.delay + tweenAlpha.duration;
			}
		}
		StartCoroutine(waitRoutine(num5));
	}

	private IEnumerator waitRoutine(float waitTime)
	{
		float elapsedTime = 0f;
		while (elapsedTime < waitTime)
		{
			elapsedTime += Time.deltaTime;
			yield return stagePause.sync();
		}
		foreach (TweenAlpha ta in tweenAlphas)
		{
			ta.style = UITweener.Style.Once;
			ta.delay = 1.3f;
			ta.duration = 0.2f;
			yield return 0;
			ta.enabled = false;
		}
		Object.Destroy(countRoot);
		countRoot = null;
		base.gameObject.SetActive(false);
	}

	public void startFlashPopup(int count, int remainCount, Vector3 pos)
	{
		if (count < 2)
		{
			return;
		}
		if (countRoot != null)
		{
			Object.Destroy(countRoot);
		}
		base.gameObject.SetActive(true);
		if (base.transform.Find("combo_bonus_bg/Skill_nokori") != null)
		{
			if (remain == null)
			{
				remain = base.transform.Find("combo_bonus_bg/Skill_nokori").gameObject;
			}
			if (!remain.activeSelf)
			{
				remain.SetActive(true);
			}
			if (skill_bg == null)
			{
				skill_bg = base.transform.Find("combo_bonus_bg/Skill_nokori/nokori/bg").GetComponent<UISprite>();
			}
			if (skill_num == null)
			{
				skill_num = base.transform.Find("combo_bonus_bg/Skill_nokori/score_number").GetComponent<UISprite>();
			}
			skill_num.spriteName = "game_score_number_" + remainCount.ToString("00");
			skill_bg.alpha = 1f;
			skill_num.alpha = 1f;
			skill_bg.GetComponent<TweenAlpha>().enabled = true;
			skill_num.GetComponent<TweenAlpha>().enabled = true;
		}
		if (me == null)
		{
			me = base.transform;
			anim = GetComponentInChildren<Animation>();
			countBase = anim.transform.Find("score_number").gameObject;
			bgBonus = anim.transform.Find("bg_bonus");
			bgBonusBasePos = bgBonus.localPosition;
			countBase.SetActive(false);
		}
		countRoot = new GameObject("countRoot");
		Utility.setParent(countRoot, anim.transform, true);
		Vector3 position = me.position;
		position.x = pos.x;
		position.y = pos.y;
		me.position = position;
		Color color = ((count <= 5) ? color1_5 : ((count > 10) ? color11_ : color6_10));
		UISprite component = bgBonus.GetComponent<UISprite>();
		component.color = color;
		tweenAlphas.Clear();
		tweenAlphas.Add(bgBonus.GetComponent<TweenAlpha>());
		int length = count.ToString().Length;
		int num = 1;
		Transform transform = null;
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < length; i++)
		{
			GameObject gameObject = Object.Instantiate(countBase) as GameObject;
			gameObject.name = (i + 1).ToString("00");
			Utility.setParent(gameObject, countRoot.transform, true);
			gameObject.SetActive(true);
			Transform transform2 = gameObject.transform;
			UISprite component2 = transform2.GetComponent<UISprite>();
			component2.spriteName = "combo_num_0" + count % (num * 10) / num;
			component2.color = color;
			component2.MakePixelPerfect();
			num *= 10;
			if (i > 0)
			{
				Vector3 localPosition = transform.localPosition;
				localPosition.x -= (transform.localScale.x + transform2.localScale.x) * 0.5f;
				transform2.localPosition = localPosition;
				num2 += transform2.localScale.x;
				bgBonus.localPosition += new Vector3(transform2.localScale.x, 0f, 0f);
			}
			else
			{
				transform2.localPosition = countBase.transform.localPosition;
				bgBonus.localPosition = bgBonusBasePos;
			}
			num3 += transform2.localScale.x;
			transform = transform2;
			tweenAlphas.Add(gameObject.GetComponent<TweenAlpha>());
		}
		countRoot.transform.localPosition += Vector3.right * num2;
		me.localPosition += new Vector3((0f - (num3 + bgBonus.localScale.x + countBase.transform.localScale.x * 0.5f)) * 0.5f, 40f, 0f);
		position = me.localPosition;
		if (position.x > 64.75f)
		{
			position.x = 64.75f;
		}
		else if (position.x < -315.25f)
		{
			position.x = -315.25f;
		}
		float num4 = 350f + NGUIUtilScalableUIRoot.GetOffsetY(true).y;
		if (position.y > num4)
		{
			position.y = num4;
		}
		me.localPosition = position;
		anim.Stop();
		foreach (TweenAlpha tweenAlpha in tweenAlphas)
		{
			tweenAlpha.Reset();
			tweenAlpha.style = UITweener.Style.PingPong;
			tweenAlpha.delay = 0.1f;
			tweenAlpha.duration = 0.1f;
			tweenAlpha.Play(true);
		}
		StartCoroutine(waitRoutine(1.6f));
	}
}
