using System;
using System.Collections;
using Network;
using UnityEngine;

public class BossSpider : BossBase
{
	private const float ANGRY_ANMATION_TIME = 6f;

	private const float CENTER_ANGLE_OFFSET = 1.575f;

	private const float BOSS_OP_ANIMATION_TIME = 5f;

	private const float FEELD_BUBBLE_FADE_TIME = 1.5f;

	private const float attackEventTime = 0.3f;

	private const float attackLoopTime = 3f;

	private const float TURN_OFF_FLAG_TIME = 5f;

	private const float TURN_OFF_HIT_FLAG_TIME = 3f;

	private const float EGG_SE_WAIT_TIME = 0.35f;

	private const float offset = -0.4f;

	private Vector3 basePos;

	private float basePosX;

	private float basePosY;

	private float basePosZ;

	private Vector3 tailBasePos;

	private float tailBaseX;

	private float tailBaseY;

	private float tailBaseZ;

	private float canHitTime;

	private GameObject pelvis;

	private GameObject body_root;

	public GameObject tail;

	public UISprite tailSprite;

	public GameObject[] joints;

	public UISprite[] jointSprites;

	private GameObject Boss_Anm_root;

	private GameObject Boss_eff00;

	private GameObject Boss_eff01;

	private GameObject Boss_eff02;

	private GameObject knockoutEff;

	private float ONE_MOVE_ROOP_TIME = 20f;

	private int MOVE_LOOP_COUNT = 2;

	private Vector3 angryEffOffset = new Vector3(0f, -0.14f, 0f);

	private float tempMoveTime;

	private Vector3 tailRotateAngle = new Vector3(0f, 0f, 180f);

	private float tweenTime = 1.2f;

	public override void SetupBoss(int level, float moveTime, int attackSpan)
	{
		BossDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<BossDataTable>();
		BossListData bossData = component.getBossData();
		BossListData.BossLevelData[] bossLevelList = bossData.bossList[3].bossLevelList;
		foreach (BossListData.BossLevelData bossLevelData in bossLevelList)
		{
			if (bossLevelData.level == level)
			{
				maxHP = bossLevelData.maxHp;
				currentHP = ((bossLevelData.hp != 0) ? bossLevelData.hp : maxHP);
				startHP = currentHP;
				break;
			}
		}
		ONE_MOVE_ROOP_TIME = moveTime * 2f;
		MOVE_LOOP_COUNT = attackSpan / 2;
		bType = BossStageInfo.eBossType.Spider;
		StartCoroutine(Init());
	}

	public override IEnumerator StartAnimation()
	{
		StartCoroutine(BossActRoutine());
		while (state == eState.start)
		{
			yield return stagePause_.sync();
		}
	}

	private IEnumerator Init()
	{
		myTrans = base.transform;
		basePos = base.transform.position;
		basePosX = base.transform.position.x;
		basePosY = base.transform.position.y;
		basePosZ = base.transform.position.z;
		bossGauge = frontUi.Find("Top_ui").Find("Boss_gauge").gameObject;
		Boss_Anm_root = base.transform.Find("Boss_Anm_root").gameObject;
		Boss_eff00 = Boss_Anm_root.transform.Find("Boss_eff00").gameObject;
		Boss_eff01 = Boss_Anm_root.transform.Find("Boss_eff01").gameObject;
		knockoutEff = Boss_Anm_root.transform.Find("Boss_eff02/bg00").gameObject;
		pelvis = Boss_Anm_root.transform.Find("pelvis").gameObject;
		body_root = GameObject.Find("body_root");
		tail = Boss_Anm_root.transform.Find("pelvis/tail/ball/Bos03_tail01").gameObject;
		tailSprite = tail.GetComponentInChildren<UISprite>();
		tailBasePos = tail.transform.position;
		tailBaseX = tail.transform.position.x;
		tailBaseY = tail.transform.position.y;
		tailBaseZ = tail.transform.position.z;
		tail.SetActive(true);
		tailInit();
		animation = GetComponentInChildren<Animation>();
		Boss_Anm_root.SetActive(false);
		BoxCollider[] cols = GetComponentsInChildren<BoxCollider>();
		for (int i = 0; i < cols.Length; i++)
		{
			cols[i].isTrigger = false;
		}
		bubbleRoot.gameObject.SetActive(false);
		yield return null;
	}

	public override IEnumerator Attack()
	{
		while (bossPart_.state != Part_BossStage.eState.Wait && state == eState.attacking)
		{
			yield return stagePause_.sync();
		}
		if (state == eState.attacking)
		{
			isWaitBossAction = true;
			yield return StartCoroutine(bossPart_.BreakStarBubble(1));
			isWaitBossAction = false;
		}
		yield return null;
	}

	private IEnumerator BossActRoutine()
	{
		while (true)
		{
			switch (state)
			{
			case eState.start:
				yield return StartCoroutine(StartRoutine());
				break;
			case eState.moving:
				yield return StartCoroutine(MovingRoutine());
				break;
			case eState.attacking:
				yield return StartCoroutine(AttackingRoutine());
				break;
			case eState.canHitDamage:
				yield return StartCoroutine(CanHitDamageRoutine());
				break;
			case eState.damaged:
				yield return StartCoroutine(DamagedRoutine());
				break;
			case eState.dead:
				yield return StartCoroutine(DeadRoutine());
				break;
			case eState.picking:
				yield return StartCoroutine(PickingRoutine());
				break;
			case eState.None:
				if (isWaitting)
				{
					yield return StartCoroutine(WaitRoutine());
					break;
				}
				if (!isCanHit)
				{
					state = eState.moving;
				}
				isWaitBossAction = false;
				break;
			}
			if (isDead)
			{
				break;
			}
			yield return stagePause_.sync();
		}
	}

	private IEnumerator StartRoutine()
	{
		if (state != 0)
		{
			yield break;
		}
		Sound.Instance.playSe(Sound.eSe.SE_536_boss_shutsugen);
		tail.SetActive(true);
		GameObject bossObj = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Boss_03")) as GameObject;
		bossObj.transform.parent = base.transform.parent;
		bossObj.transform.localPosition = base.transform.localPosition;
		bossObj.transform.localScale = new Vector3(0.01f, 0.01f, 1f);
		iTween.ScaleTo(bossObj, iTween.Hash("x", 1f, "y", 1f, "easetype", iTween.EaseType.easeInOutCubic, "time", 5f, "islocal", true));
		iTween.ShakePosition(bossObj, iTween.Hash("x", 0f, "y", 0.01f, "time", 5f, "easetype", iTween.EaseType.linear));
		while (bossObj.GetComponent<iTween>() != null)
		{
			yield return stagePause_.sync();
		}
		UnityEngine.Object.Destroy(bossObj);
		Boss_Anm_root.SetActive(true);
		jointSprites = new UISprite[joints.Length];
		for (int i = 0; i < joints.Length; i++)
		{
			joints[i].SetActive(true);
			jointSprites[i] = joints[i].GetComponentInChildren<UISprite>();
			yield return stagePause_.sync();
		}
		foreach (Bubble_Boss bubble in bossPart_.fieldBubbleList)
		{
			if (!(bubble.sprite == null))
			{
				Color tempColor2 = new Color(bubble.sprite.color.r, bubble.sprite.color.g, bubble.sprite.color.b, 0f);
				bubble.sprite.color = tempColor2;
			}
		}
		bubbleRoot.gameObject.SetActive(true);
		float alpha_time = 0f;
		while (alpha_time < 1.5f)
		{
			foreach (Bubble_Boss bubble2 in bossPart_.fieldBubbleList)
			{
				if (!(bubble2.sprite == null))
				{
					Color tempColor2 = new Color(bubble2.sprite.color.r, bubble2.sprite.color.g, bubble2.sprite.color.b, alpha_time / 1.5f);
					bubble2.sprite.color = tempColor2;
				}
			}
			alpha_time += Time.deltaTime;
			yield return stagePause_.sync();
		}
		foreach (Bubble_Boss bubble3 in bossPart_.fieldBubbleList)
		{
			if (!(bubble3.sprite == null))
			{
				Color tempColor2 = new Color(bubble3.sprite.color.r, bubble3.sprite.color.g, bubble3.sprite.color.b, 1f);
				bubble3.sprite.color = tempColor2;
			}
		}
		while (base.gameObject.GetComponent<iTween>() != null)
		{
			yield return stagePause_.sync();
		}
		StartCoroutine(BossHpSet());
		state = eState.moving;
		yield return stagePause_.sync();
	}

	private IEnumerator MovingRoutine()
	{
		animation.clip = animation["Boss_03_move_anm"].clip;
		animation.Play("Boss_03_move_anm");
		while (bossPart_.state == Part_BossStage.eState.Start || bossPart_.state == Part_BossStage.eState.Gameover)
		{
			yield return stagePause_.sync();
		}
		yield return StartCoroutine(MovingPositionRoutine());
		if (state == eState.moving)
		{
			state = eState.attacking;
			yield return stagePause_.sync();
		}
	}

	private IEnumerator AttackingRoutine()
	{
		if (bossPart_.state != Part_BossStage.eState.Gameover)
		{
			bool successedAttack2 = false;
			animation.clip = animation["Boss_03_attack_anm"].clip;
			animation.Play("Boss_03_attack_anm");
			if (!successedAttack2)
			{
				yield return StartCoroutine(Attack());
				successedAttack2 = true;
			}
			float elapsedTime = 0f;
			while (animation.IsPlaying("Boss_03_attack_anm") && elapsedTime <= 3f)
			{
				elapsedTime += Time.deltaTime;
				yield return stagePause_.sync();
			}
		}
		if (state == eState.attacking)
		{
			state = eState.moving;
		}
	}

	private IEnumerator CanHitDamageRoutine()
	{
		tempMoveTime %= ONE_MOVE_ROOP_TIME * 2f;
		if (isCanHit)
		{
			animation.clip = animation["Boss_03_knockout_anm"].clip;
			animation.Play("Boss_03_knockout_anm");
		}
		while (animation.IsPlaying("Boss_03_knockout_anm") && state == eState.canHitDamage && isCanHit)
		{
			yield return stagePause_.sync();
		}
	}

	private IEnumerator requestTurnOffHitFlag()
	{
		while (state == eState.canHitDamage && canHitTime < 5f)
		{
			canHitTime += Time.deltaTime;
			yield return stagePause_.sync();
		}
		if (state != eState.canHitDamage)
		{
			float elapsedTime = 0f;
			float targetTime = 3f;
			if (5f - canHitTime <= 3f)
			{
				targetTime = 5f - canHitTime;
			}
			while (elapsedTime < targetTime)
			{
				elapsedTime += Time.deltaTime;
				yield return stagePause_.sync();
			}
		}
		if (currentHP > 0)
		{
			isCanHit = false;
			knockoutEff.SetActive(false);
			isWaitBossAction = false;
		}
		while (bossPart_.isWaitStageAcction)
		{
			yield return stagePause_.sync();
		}
		yield return StartCoroutine(bossPart_.turnupSpiderweb());
		tail.GetComponentInChildren<UISprite>().enabled = true;
		for (int i = 0; i < jointSprites.Length - 2; i++)
		{
			jointSprites[i].enabled = true;
		}
		state = eState.moving;
		yield return stagePause_.sync();
	}

	private IEnumerator DamagedRoutine()
	{
		Sound.Instance.playSe(Sound.eSe.SE_540_fukurou_damage);
		Damage();
		Sound.Instance.playSe(Sound.eSe.SE_228_bossdmg);
		animation.clip = animation["Boss_03_damage_anm"].clip;
		animation.Play("Boss_03_damage_anm");
		float i = bossHpSlider.sliderValue;
		while (i > (float)currentHP / (float)maxHP)
		{
			i -= Time.deltaTime;
			bossHpSlider.sliderValue = i;
			yield return stagePause_.sync();
		}
		while (animation.IsPlaying("Boss_03_damage_anm"))
		{
			yield return stagePause_.sync();
		}
		isWaitBossAction = false;
		if (currentHP <= 0)
		{
			state = eState.dead;
		}
		else if (isWaitBossAction)
		{
			state = eState.damaged;
		}
		else if (isCanHit)
		{
			state = eState.canHitDamage;
		}
		else
		{
			state = eState.moving;
		}
	}

	private IEnumerator DeadRoutine()
	{
		animation.clip = animation["Boss_03_dead_anm"].clip;
		animation.Play("Boss_03_dead_anm");
		while (animation.IsPlaying("Boss_03_dead_anm"))
		{
			yield return stagePause_.sync();
		}
		iTween.ShakePosition(base.gameObject, iTween.Hash("x", 0.03f, "y", 0.02f, "time", 0.9f, "easetype", iTween.EaseType.linear));
		float elapsedTime = 0f;
		while (elapsedTime < 0.7f)
		{
			elapsedTime += Time.deltaTime;
			yield return stagePause_.sync();
		}
		isWaitBossAction = false;
		isDead = true;
		yield return stagePause_.sync();
	}

	private IEnumerator PickingRoutine()
	{
		if (!(bossPart_.pickingTargetBubble == null))
		{
			animation.clip = animation["Boss_03_attack_anm"].clip;
			animation.Play("Boss_03_attack_anm");
			if (state == eState.picking)
			{
				isWaitBossAction = true;
				yield return StartCoroutine(bossPart_.BreakPickingTarget());
				isWaitBossAction = false;
			}
			while (bossPart_.state != Part_BossStage.eState.Wait && state == eState.picking)
			{
				yield return stagePause_.sync();
			}
			state = eState.moving;
			yield return stagePause_.sync();
		}
	}

	private IEnumerator WaitRoutine()
	{
		base.transform.position = basePos;
		animation.clip = animation["Boss_03_move_anm"].clip;
		animation.Play("Boss_03_move_anm");
		while (isWaitting)
		{
			yield return stagePause_.sync();
		}
	}

	private IEnumerator MovingPositionRoutine()
	{
		isMoving = true;
		float move_time = tempMoveTime;
		while (isMoving && move_time < ONE_MOVE_ROOP_TIME * (float)MOVE_LOOP_COUNT)
		{
			float angle = move_time / ONE_MOVE_ROOP_TIME * (float)Math.PI - 1.575f;
			float x = basePosX + 0f * Mathf.Cos(angle);
			float y = basePosY - Mathf.Abs(0.24f * Mathf.Sin(angle * 2f));
			myTrans.position = new Vector3(x, y, myTrans.position.z);
			float tx = tailBaseX + 0.1f * Mathf.Cos(angle * 3f);
			float ty = myTrans.position.y + -0.4f + 0.01f * Mathf.Sin(angle);
			tail.transform.position = new Vector3(tx, ty, tail.transform.position.z);
			tailSprite.gameObject.transform.eulerAngles = new Vector3(base.transform.localRotation.x, base.transform.localRotation.y, tailRotateAngle.z + 45f * Mathf.Cos(angle * 3f));
			move_time += Time.deltaTime;
			yield return stagePause_.sync();
		}
		if (isMoving)
		{
			tempMoveTime = 0f;
		}
		else
		{
			tempMoveTime = move_time;
		}
		yield return stagePause_.sync();
	}

	public override eBossHitState CheckHitBoss(Vector3 start, Vector3 end, int bubbleType)
	{
		Vector3 direction = end - start;
		float radius = Bubble.hitSize * bossPart_.uiScale * 0.5f;
		int layerMask = 1 << LayerMask.NameToLayer("BossCollider");
		RaycastHit hitInfo;
		if (Physics.SphereCast(start, radius, direction, out hitInfo, direction.magnitude, layerMask))
		{
			if (isCanHit)
			{
				return eBossHitState.Hit;
			}
			return eBossHitState.Diffend;
		}
		return eBossHitState.None;
	}

	public override void BreakedSpiderweb()
	{
		if (state == eState.moving || state == eState.attacking)
		{
			canHitTime = 0f;
			state = eState.canHitDamage;
			isMoving = false;
			isCanHit = true;
			Sound.Instance.playSe(Sound.eSe.SE_247_bosspiyo);
			knockoutEff.SetActive(true);
			tail.GetComponentInChildren<UISprite>().enabled = false;
			for (int i = 0; i < jointSprites.Length - 2; i++)
			{
				jointSprites[i].enabled = false;
			}
			Sound.Instance.playSe(Sound.eSe.SE_247_bosspiyo);
			StartCoroutine(requestTurnOffHitFlag());
		}
	}

	private void tailInit()
	{
		tail.transform.position = tailBasePos;
		tail.transform.eulerAngles = tailRotateAngle;
	}

	private IEnumerator tailSetPos()
	{
		Vector3 pos = new Vector3(0f, tail.transform.position.y + -0.4f, 0f);
		float tweenTime = 1f;
		iTween.MoveTo(tail, iTween.Hash("position", pos, "time", tweenTime, "easetype", iTween.EaseType.easeInOutQuad));
		while (tail.GetComponent<iTween>() != null)
		{
			yield return stagePause_.sync();
		}
	}

	public override IEnumerator tailAction(Bubble_Boss bubble)
	{
		Vector3 temp = tail.transform.eulerAngles;
		tail.transform.eulerAngles = tailRotateAngle;
		Vector3[] path = new Vector3[3]
		{
			new Vector3(myTrans.position.x, myTrans.position.y - 0.1f, tail.transform.position.z),
			bubble.myTrans.position,
			tail.transform.position
		};
		iTween.MoveTo(tail, iTween.Hash("path", path, "time", tweenTime, "easetype", iTween.EaseType.easeInSine));
		tail.transform.eulerAngles = temp;
		yield return null;
		while (tail.GetComponent<iTween>() != null)
		{
			yield return stagePause_.sync();
		}
	}

	public override IEnumerator GameoverWait()
	{
		isWaitting = true;
		state = eState.None;
		isMoving = false;
		isCanHit = false;
		yield return 0;
	}

	public override IEnumerator ContinueSetup()
	{
		state = eState.None;
		isMoving = false;
		isCanHit = false;
		existPickObj = false;
		tempMoveTime = 0f;
		base.transform.position = basePos;
		yield return StartCoroutine(bossPart_.turnupSpiderweb());
		float elapsedTime = 0f;
		while (elapsedTime < 0.4f)
		{
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		isWaitting = false;
		state = eState.moving;
	}

	private void OnDestroy()
	{
		for (int i = 0; i < joints.Length; i++)
		{
			joints[i] = null;
		}
		tail = null;
	}
}
