using System.Collections;
using UnityEngine;

public class ScoreParticle : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void animStart(StagePause stagePause)
	{
		StartCoroutine(anim(stagePause));
	}

	public void bonusAnimStart(StagePause stagePause)
	{
		StartCoroutine(bonusAnim(stagePause));
	}

	public void rankingAnimStart(StagePause stagePause)
	{
		StartCoroutine(rankingAnim(stagePause));
	}

	private IEnumerator anim(StagePause stagePause)
	{
		float waitTime = 1f;
		float move = 750f;
		Vector3 target = new Vector3(220f, 440f, -15f) + NGUIUtilScalableUIRoot.GetOffsetY(true);
		Vector3 diff = base.transform.localPosition - target;
		float distance = diff.magnitude;
		if (distance > move)
		{
			distance = move;
			base.transform.localPosition = target + diff.normalized * move;
		}
		waitTime *= distance / move;
		iTween.MoveTo(base.gameObject, iTween.Hash("position", target, "easetype", iTween.EaseType.easeInQuad, "islocal", true, "time", waitTime));
		ParticleSystem particle = base.gameObject.GetComponentInChildren<ParticleSystem>();
		UISprite sp = base.transform.Find("bg").GetComponent<UISprite>();
		int waitCount = 7;
		float elapsedTime2 = 0f;
		while (elapsedTime2 < waitTime)
		{
			elapsedTime2 += Time.deltaTime;
			if (waitTime - elapsedTime2 < waitTime * 0.1f)
			{
				Color c = sp.color;
				c.a = 1f - (waitTime - elapsedTime2) / (waitTime * 0.1f);
				sp.color = c;
			}
			waitCount--;
			yield return stagePause.sync();
		}
		if (particle != null)
		{
			particle.enableEmission = false;
		}
		sp.enabled = false;
		while (waitCount > 0)
		{
			waitCount--;
			yield return stagePause.sync();
		}
		Part_Stage part = stagePause.GetComponent<Part_Stage>();
		part.nextScore = part.totalScore;
		waitTime = ((!(particle != null)) ? 0.7f : particle.startLifetime);
		elapsedTime2 = 0f;
		while (elapsedTime2 < waitTime)
		{
			elapsedTime2 += Time.deltaTime;
			yield return stagePause.sync();
		}
		Object.Destroy(base.gameObject);
	}

	private IEnumerator bonusAnim(StagePause stagePause)
	{
		float waitTime = 1f;
		float move = 750f;
		Vector3 target = new Vector3(-200f, 440f, -15f) + NGUIUtilScalableUIRoot.GetOffsetY(true);
		Vector3 diff = base.transform.localPosition - target;
		float distance = diff.magnitude;
		if (distance > move)
		{
			distance = move;
			base.transform.localPosition = target + diff.normalized * move;
		}
		waitTime *= distance / move;
		iTween.MoveTo(base.gameObject, iTween.Hash("position", target, "easetype", iTween.EaseType.easeInQuad, "islocal", true, "time", waitTime));
		ParticleSystem particle = base.gameObject.GetComponentInChildren<ParticleSystem>();
		UISprite sp = base.transform.Find("bg").GetComponent<UISprite>();
		int waitCount = 7;
		float elapsedTime2 = 0f;
		while (elapsedTime2 < waitTime)
		{
			elapsedTime2 += Time.deltaTime;
			if (waitTime - elapsedTime2 < waitTime * 0.1f)
			{
				Color c = sp.color;
				c.a = 1f - (waitTime - elapsedTime2) / (waitTime * 0.1f);
				sp.color = c;
			}
			waitCount--;
			yield return stagePause.sync();
		}
		if (particle != null)
		{
			particle.enableEmission = false;
		}
		sp.enabled = false;
		while (waitCount > 0)
		{
			waitCount--;
			yield return stagePause.sync();
		}
		Part_BonusStage part = stagePause.GetComponent<Part_BonusStage>();
		waitTime = ((!(particle != null)) ? 0.7f : particle.startLifetime);
		elapsedTime2 = 0f;
		while (elapsedTime2 < waitTime)
		{
			elapsedTime2 += Time.deltaTime;
			yield return stagePause.sync();
		}
		Object.Destroy(base.gameObject);
	}

	private IEnumerator rankingAnim(StagePause stagePause)
	{
		float waitTime = 1f;
		float move = 750f;
		Vector3 target = new Vector3(-200f, 440f, -15f) + NGUIUtilScalableUIRoot.GetOffsetY(true);
		Vector3 diff = base.transform.localPosition - target;
		float distance = diff.magnitude;
		if (distance > move)
		{
			distance = move;
			base.transform.localPosition = target + diff.normalized * move;
		}
		waitTime *= distance / move;
		iTween.MoveTo(base.gameObject, iTween.Hash("position", target, "easetype", iTween.EaseType.easeInQuad, "islocal", true, "time", waitTime));
		ParticleSystem particle = base.gameObject.GetComponentInChildren<ParticleSystem>();
		UISprite sp = base.transform.Find("bg").GetComponent<UISprite>();
		int waitCount = 7;
		float elapsedTime2 = 0f;
		while (elapsedTime2 < waitTime)
		{
			elapsedTime2 += Time.deltaTime;
			if (waitTime - elapsedTime2 < waitTime * 0.1f)
			{
				Color c = sp.color;
				c.a = 1f - (waitTime - elapsedTime2) / (waitTime * 0.1f);
				sp.color = c;
			}
			waitCount--;
			yield return stagePause.sync();
		}
		if (particle != null)
		{
			particle.enableEmission = false;
		}
		sp.enabled = false;
		while (waitCount > 0)
		{
			waitCount--;
			yield return stagePause.sync();
		}
		Part_RankingStage part = stagePause.GetComponent<Part_RankingStage>();
		part.nextScore = part.totalScore;
		waitTime = ((!(particle != null)) ? 0.7f : particle.startLifetime);
		elapsedTime2 = 0f;
		while (elapsedTime2 < waitTime)
		{
			elapsedTime2 += Time.deltaTime;
			yield return stagePause.sync();
		}
		Object.Destroy(base.gameObject);
	}
}
