using System.Collections;
using UnityEngine;

public class Chackn : MonoBehaviour
{
	private Transform chara_02_00;

	public float moveSpan;

	private Part_Stage part;

	private Part_RankingStage part_ranking;

	private float waitTime;

	private float elapsedTime;

	private void Start()
	{
		base.tag = "Chackn";
	}

	private void Update()
	{
	}

	public void animStart(StagePause stagePause)
	{
		chara_02_00 = base.transform.Find("chara_02_00");
		StartCoroutine(anim(stagePause));
	}

	private IEnumerator anim(StagePause stagePause)
	{
		float upY = 470f + NGUIUtilScalableUIRoot.GetOffsetY(true).y - base.transform.localPosition.y - part.moveSpan;
		waitTime = upY / 768.8f + 0.58f - base.GetComponent<Animation>()[base.GetComponent<Animation>().clip.name].time;
		if (base.GetComponent<Animation>().clip.name == "Rescue_01_anm")
		{
			waitTime = 1.34f;
		}
		float baseX = base.transform.localPosition.x;
		if (baseX < -110f)
		{
			base.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
		MeshFilter filter = null;
		Color[] colors = null;
		float fadeTime = 0.1f;
		elapsedTime = 0f;
		while (elapsedTime < waitTime)
		{
			if (part.isSearching)
			{
				if (part.isMoving)
				{
					yield break;
				}
				if (part.moveSpan != 0f)
				{
					Transform newParent = base.transform.parent.parent;
					base.transform.parent = newParent;
					animationReStart();
					yield break;
				}
			}
			else if (part.stagePause.pause && part.lastPoint_chackn)
			{
				base.GetComponent<Animation>()[base.GetComponent<Animation>().clip.name].speed = 0f;
				tk2dAnimatedSprite sp2 = base.gameObject.GetComponentInChildren<tk2dAnimatedSprite>();
				sp2.enabled = false;
				while (part.stagePause.pause && part.lastPoint_chackn)
				{
					yield return stagePause.sync();
				}
				base.GetComponent<Animation>()[base.GetComponent<Animation>().clip.name].speed = 1f;
				sp2.enabled = true;
			}
			float rate = elapsedTime / waitTime;
			Vector3 pos = base.transform.localPosition;
			pos.x = baseX + (-110f - baseX) * rate;
			base.transform.localPosition = pos;
			float fadeRate = 1f;
			if (elapsedTime > waitTime - fadeTime)
			{
				float t = elapsedTime - (waitTime - fadeTime);
				fadeRate = 1f - t / fadeTime;
			}
			if (fadeRate < 1f)
			{
				if (filter == null)
				{
					tk2dAnimatedSprite sp = base.gameObject.GetComponentInChildren<tk2dAnimatedSprite>();
					sp.enabled = false;
					filter = base.gameObject.GetComponentInChildren<MeshFilter>();
					filter.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Transparent Colored");
					colors = filter.sharedMesh.colors;
				}
				for (int i = 0; i < colors.Length; i++)
				{
					colors[i].a = fadeRate;
				}
				filter.sharedMesh.colors = colors;
			}
			elapsedTime += Time.deltaTime;
			yield return stagePause.sync();
		}
		stagePause.GetComponent<Part_Stage>().updateChacknNum();
		Object.Destroy(base.gameObject);
	}

	private IEnumerator animScroll(StagePause stagePause)
	{
		if (base.GetComponent<Animation>().clip.name == "Rescue_01_anm")
		{
			waitTime = 1.34f;
		}
		float baseX = base.transform.localPosition.x;
		if (baseX < -110f)
		{
			base.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
		MeshFilter filter = null;
		Color[] colors = null;
		float fadeTime = 0.1f;
		elapsedTime = 0f;
		while (elapsedTime < waitTime)
		{
			float rate = elapsedTime / waitTime;
			Vector3 pos = base.transform.localPosition;
			pos.x = baseX + (-110f - baseX) * rate;
			base.transform.localPosition = pos;
			float fadeRate = 1f;
			if (elapsedTime > waitTime - fadeTime)
			{
				float t = elapsedTime - (waitTime - fadeTime);
				fadeRate = 1f - t / fadeTime;
			}
			if (fadeRate < 1f)
			{
				if (filter == null)
				{
					tk2dAnimatedSprite sp = base.gameObject.GetComponentInChildren<tk2dAnimatedSprite>();
					sp.enabled = false;
					filter = base.gameObject.GetComponentInChildren<MeshFilter>();
					filter.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Transparent Colored");
					colors = filter.sharedMesh.colors;
				}
				for (int i = 0; i < colors.Length; i++)
				{
					colors[i].a = fadeRate;
				}
				filter.sharedMesh.colors = colors;
			}
			elapsedTime += Time.deltaTime;
			yield return stagePause.sync();
		}
		stagePause.GetComponent<Part_Stage>().updateChacknNum();
		Object.Destroy(base.gameObject);
	}

	public void setPartStage(Part_Stage _stage)
	{
		part = _stage;
	}

	public void setPartRanking(Part_RankingStage _stage)
	{
		part_ranking = _stage;
	}

	public void animationReStart()
	{
		bool flag = base.transform.localPosition.y > -450f;
		Vector3 zero = Vector3.zero;
		if (!flag)
		{
			base.GetComponent<Animation>().clip = base.GetComponent<Animation>()["Rescue_02_anm"].clip;
			zero.x = base.transform.localPosition.x;
			zero.y = -510f - NGUIUtilScalableUIRoot.GetOffsetY(true).y;
			zero.z = -13f;
		}
		else
		{
			base.GetComponent<Animation>().clip = base.GetComponent<Animation>()["Rescue_anm"].clip;
			zero.x = base.transform.localPosition.x;
			zero.y = base.transform.localPosition.y - 50f;
			zero.z = -13f;
		}
		base.transform.localPosition = zero;
		float num = 470f + NGUIUtilScalableUIRoot.GetOffsetY(true).y - base.transform.localPosition.y;
		if (!flag)
		{
			waitTime = num / 900f + 0.35f + elapsedTime;
		}
		else
		{
			waitTime = num / 768.8f + 0.58f + elapsedTime;
		}
		base.GetComponent<Animation>().Play();
		tk2dAnimatedSprite componentInChildren = base.gameObject.GetComponentInChildren<tk2dAnimatedSprite>();
		if (!componentInChildren.enabled)
		{
			componentInChildren.enabled = true;
		}
		StartCoroutine(animScroll(part.stagePause));
	}

	public void animationPause()
	{
		if (part.stagePause.pause)
		{
			tk2dAnimatedSprite componentInChildren = base.gameObject.GetComponentInChildren<tk2dAnimatedSprite>();
			componentInChildren.enabled = false;
		}
		base.GetComponent<Animation>().Stop();
	}

	public bool isAnimationPlaying()
	{
		return base.GetComponent<Animation>().isPlaying;
	}

	private void LateUpdate()
	{
		if (!(chara_02_00 == null))
		{
			Vector3 localPosition = chara_02_00.localPosition;
			localPosition.x = 0f;
			chara_02_00.localPosition = localPosition;
		}
	}
}
