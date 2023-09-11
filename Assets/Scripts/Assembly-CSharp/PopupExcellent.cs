using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupExcellent : MonoBehaviour
{
	private Transform me;

	private UITweener[] tweens;

	public StagePause stagePause;

	private List<float> seTimeList = new List<float>();

	public void startPopup(Vector3 pos)
	{
		base.gameObject.SetActive(true);
		if (me == null)
		{
			me = base.transform;
			tweens = GetComponentsInChildren<UITweener>();
		}
		Vector3 position = me.position;
		position.x = pos.x;
		position.y = pos.y;
		me.position = position;
		me.localPosition += Vector3.down * 50f;
		position = me.localPosition;
		if (position.x > 190f)
		{
			position.x = 190f;
		}
		else if (position.x < -190f)
		{
			position.x = -190f;
		}
		float num = 260f + NGUIUtilScalableUIRoot.GetOffsetY(true).y;
		if (position.y > num)
		{
			position.y = num;
		}
		me.localPosition = position;
		seTimeList.Clear();
		float num2 = 0f;
		UITweener[] array = tweens;
		foreach (UITweener uITweener in array)
		{
			uITweener.Reset();
			uITweener.Play(true);
			if (num2 < uITweener.duration + uITweener.delay)
			{
				num2 = uITweener.duration + uITweener.delay;
			}
			if (!seTimeList.Contains(uITweener.delay) && uITweener.name.Contains("excellent_eff"))
			{
				seTimeList.Add(uITweener.delay);
			}
		}
		StartCoroutine(waitRoutine(num2));
	}

	private IEnumerator waitRoutine(float waitTime)
	{
		float elapsedTime = 0f;
		while (elapsedTime < waitTime)
		{
			elapsedTime += Time.deltaTime;
			for (int i = seTimeList.Count - 1; i >= 0; i--)
			{
				if (elapsedTime >= seTimeList[i])
				{
					Sound.Instance.playSe(Sound.eSe.SE_225_fuusen);
					seTimeList.RemoveAt(i);
				}
			}
			yield return stagePause.sync();
		}
		base.gameObject.SetActive(false);
	}
}
