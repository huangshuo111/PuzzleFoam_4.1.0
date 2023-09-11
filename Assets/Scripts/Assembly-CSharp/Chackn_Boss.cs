using System.Collections;
using UnityEngine;

public class Chackn_Boss : MonoBehaviour
{
	private const float MOVE_SPEED = 1f;

	private const float DESTOROY_OFFSET_Y = 0.3f;

	private const float CHACKN_MOVE_TIME_DEV_OFFSET = 0.72f;

	public float moveSpan;

	private Transform trans;

	private Part_BossStage part;

	public BossBase bossBase_;

	private StagePause stagePause;

	private Vector3 moveVec;

	private float dPosY;

	private Vector3 offset_rescR = new Vector3(-0.2f, -0.05f, 0f);

	private Vector3 offset_rescL = new Vector3(0.2f, -0.05f, 0f);

	private Vector3 offset_upR = new Vector3(-0.2f, 0f, 0f);

	private Vector3 offset_upL = new Vector3(0.2f, 0f, 0f);

	private Vector3 offsetVec_hit = new Vector3(0f, -0.05f, 0f);

	private Vector3 startHitPos;

	private float startY;

	private float startX;

	private void Start()
	{
		base.tag = "Chackn";
	}

	public void animStart(StagePause_Boss stagePause)
	{
		startY = base.transform.position.y;
		dPosY = part.ceilingWldBaseY + 0.3f;
		if (base.transform.position.x < 0f)
		{
			base.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
		StartCoroutine(anim(stagePause));
	}

	private IEnumerator anim(StagePause_Boss stagePause)
	{
		bool hitBoss = false;
		Vector3 currentPos2 = base.transform.position;
		Vector3 targetPos = new Vector3(base.transform.position.x, dPosY, base.transform.position.z);
		Vector3[] path = new Vector3[2];
		if (base.transform.position.x > 0f)
		{
			path[0] = base.transform.position + offset_rescR;
			path[1] = targetPos + offset_upR;
		}
		else
		{
			path[0] = base.transform.position + offset_rescL;
			path[1] = targetPos + offset_upL;
		}
		startX = path[0].x;
		startHitPos = new Vector3(startX, startY, base.transform.position.z);
		float tweenTime = (dPosY - currentPos2.y) / 0.72f;
		iTween.MoveTo(base.gameObject, iTween.Hash("path", path, "time", tweenTime, "easetype", iTween.EaseType.easeOutCubic));
		while (base.gameObject.GetComponent<iTween>() != null)
		{
			currentPos2 = base.transform.position;
			switch (bossBase_.CheckHitChackn(startHitPos, currentPos2))
			{
			case BossBase.eBossHitState.Hit:
				hitBoss = true;
				bossBase_.isMoving = false;
				bossBase_.HitChackn();
				break;
			}
			if (hitBoss && base.gameObject.GetComponent<iTween>() != null)
			{
				iTween.Stop(base.gameObject);
			}
			yield return stagePause.sync();
		}
		if (hitBoss)
		{
			currentPos2 = base.transform.position;
			targetPos = base.transform.position + offsetVec_hit;
			moveVec = new Vector3(0f, -0.1f, 0f);
			while (base.transform.position.y > targetPos.y)
			{
				Vector3 vec = moveVec * Time.deltaTime;
				base.transform.position += vec;
				yield return stagePause.sync();
			}
			moveVec = new Vector3(0f, 0.6f, 0f);
			while (base.transform.position.y < dPosY)
			{
				Vector3 vec = moveVec * (Time.deltaTime * 1f);
				base.transform.position += vec;
				yield return stagePause.sync();
			}
			Object.Destroy(base.gameObject);
		}
		Object.Destroy(base.gameObject);
	}

	public void setPartStage(Part_BossStage _stage)
	{
		part = _stage;
	}

	public void animationReStart()
	{
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
}
