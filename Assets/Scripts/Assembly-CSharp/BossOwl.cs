using System;
using System.Collections;
using Network;
using UnityEngine;

public class BossOwl : BossBase
{
	private const float attackLoopTime = 2f;

	private const float CENTER_ANGLE_OFFSET = 1.575f;

	private const float BOSS_OP_ANIMATION_TIME = 5f;

	private const float FEELD_BUBBLE_FADE_TIME = 1.5f;

	private const float attackEventTime = 0.3f;

	private const float TURN_OFF_FLAG_TIME = 5f;

	private const float TURN_OFF_HIT_FLAG_TIME = 2.7f;

	private Vector3 basePos;

	private float baseX;

	private float baseY;

	private GameObject pelvis;

	private GameObject Boss_Anm_root;

	private GameObject knockoutEff;

	private GameObject attackEff;

	private GameObject featherEff;

	private Vector3 turnRotate = new Vector3(0f, 180f, 0f);

	private float canHitTime;

	public float ONE_MOVE_ROOP_TIME = 2f;

	public int MOVE_LOOP_COUNT = 8;

	private bool bTurnOff;

	private float tempMoveTime;

	public override void SetupBoss(int level, float moveTime, int attackSpan)
	{
		BossDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<BossDataTable>();
		BossListData bossData = component.getBossData();
		BossListData.BossLevelData[] bossLevelList = bossData.bossList[0].bossLevelList;
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
		ONE_MOVE_ROOP_TIME = moveTime;
		MOVE_LOOP_COUNT = attackSpan;
		bType = BossStageInfo.eBossType.Owl;
		isCanHitChackn = true;
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
		bossGauge = frontUi.Find("Top_ui").Find("Boss_gauge").gameObject;
		Boss_Anm_root = base.transform.Find("Boss_Anm_root").gameObject;
		knockoutEff = Boss_Anm_root.transform.Find("Boss_eff02/bg00").gameObject;
		attackEff = Boss_Anm_root.transform.Find("Boss_eff01").gameObject;
		featherEff = Boss_Anm_root.transform.Find("Boss_eff00").gameObject;
		pelvis = GameObject.Find("pelvis");
		animation = GetComponentInChildren<Animation>();
		baseX = base.gameObject.transform.position.x;
		baseY = base.gameObject.transform.position.y;
		tempMoveTime = 0f;
		Boss_Anm_root.SetActive(false);
		bubbleRoot.gameObject.SetActive(false);
		yield return null;
	}

	public override IEnumerator Attack()
	{
		float elapsedTime = 0f;
		while (elapsedTime < 0.3f)
		{
			elapsedTime += Time.deltaTime;
			yield return stagePause_.sync();
		}
		Sound.Instance.playSe(Sound.eSe.SE_228_bossdmg);
		if (bossPart_.fieldBubbleList != null)
		{
			foreach (Bubble_Boss bubble in bossPart_.fieldBubbleList)
			{
				if (bubble.type != Bubble.eType.Fulcrum && bubble.type != Bubble.eType.FriendFulcrum && bubble.type != Bubble.eType.RotateFulcrumL && bubble.type != Bubble.eType.RotateFulcrumR && bubble.GetComponentInChildren<tk2dAnimatedSprite>() != null)
				{
					bubble.sprite.Stop();
					bubble.sprite.Play("bubble_fether_00");
					bubble.isSecret = true;
				}
			}
		}
		yield return stagePause_.sync();
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
				pelvis.transform.localRotation = Quaternion.identity;
				while (bTurnOff)
				{
					yield return null;
				}
				isCanHitChackn = true;
				isWaitBossAction = false;
				yield return StartCoroutine(movingRoutine());
				break;
			case eState.attacking:
				isCanHitChackn = true;
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
			case eState.None:
				if (isWaitting)
				{
					yield return StartCoroutine(WaitRoutine());
				}
				else if (!isCanHit)
				{
					state = eState.moving;
				}
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
		base.gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 1f);
		Boss_Anm_root.SetActive(true);
		animation.clip = animation["Boss_00_start_anm"].clip;
		animation.Play("Boss_00_start_anm");
		iTween.ScaleTo(base.gameObject, iTween.Hash("x", 1f, "y", 1f, "easetype", iTween.EaseType.easeInOutCubic, "time", 5f, "islocal", true));
		iTween.ShakePosition(base.gameObject, iTween.Hash("x", 0f, "y", 0.01f, "time", 5f, "easetype", iTween.EaseType.linear));
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
			foreach (Bubble_Boss bubble3 in bossPart_.fieldBubbleList)
			{
				if (!(bubble3.sprite == null))
				{
					Color tempColor2 = new Color(bubble3.sprite.color.r, bubble3.sprite.color.g, bubble3.sprite.color.b, alpha_time / 1.5f);
					bubble3.sprite.color = tempColor2;
				}
			}
			alpha_time += Time.deltaTime;
			yield return stagePause_.sync();
		}
		foreach (Bubble_Boss bubble2 in bossPart_.fieldBubbleList)
		{
			if (!(bubble2.sprite == null))
			{
				Color tempColor2 = new Color(bubble2.sprite.color.r, bubble2.sprite.color.g, bubble2.sprite.color.b, 1f);
				bubble2.sprite.color = tempColor2;
			}
		}
		while (base.gameObject.GetComponent<iTween>() != null)
		{
			yield return stagePause_.sync();
		}
		StartCoroutine(BossHpSet());
		state = eState.moving;
	}

	private IEnumerator movingRoutine()
	{
		animation.clip = animation["Boss_00_move_anm"].clip;
		animation.Play("Boss_00_move_anm");
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
			bool successedAttack = false;
			float elapsedTime = 0f;
			attackEff.SetActive(true);
			featherEff.SetActive(true);
			animation.clip = animation["Boss_00_attack_anm"].clip;
			animation.Play("Boss_00_attack_anm");
			Sound.Instance.playSe(Sound.eSe.SE_538_fukurou_attack);
			while (animation.IsPlaying("Boss_00_attack_anm") && state == eState.attacking && elapsedTime <= 2f)
			{
				elapsedTime += Time.deltaTime;
				if (!successedAttack)
				{
					yield return StartCoroutine(Attack());
					successedAttack = true;
					yield return stagePause_.sync();
				}
				else
				{
					yield return stagePause_.sync();
				}
			}
			animation.clip = animation["Boss_00_attack_end_anm"].clip;
			animation.Play("Boss_00_attack_end_anm");
			while (animation.IsPlaying("Boss_00_attack_end_anm") && state == eState.attacking)
			{
				yield return stagePause_.sync();
			}
			attackEff.SetActive(false);
			featherEff.SetActive(false);
		}
		if (state == eState.attacking)
		{
			state = eState.moving;
			yield return stagePause_.sync();
		}
	}

	private IEnumerator CanHitDamageRoutine()
	{
		float elapsedTime = 0f;
		tempMoveTime %= ONE_MOVE_ROOP_TIME * 2f;
		if (isCanHit)
		{
			animation.clip = animation["Boss_00_knockout_anm"].clip;
			animation.Play("Boss_00_knockout_anm");
		}
		while (animation.IsPlaying("Boss_00_knockout_anm") && state == eState.canHitDamage && isCanHit)
		{
			elapsedTime += Time.deltaTime;
			yield return stagePause_.sync();
		}
		while (animation.IsPlaying("Boss_00_damage_end_anm"))
		{
			yield return stagePause_.sync();
		}
	}

	private IEnumerator requestTurnOffHitFlag()
	{
		bTurnOff = true;
		while (state == eState.canHitDamage && canHitTime < 5f)
		{
			canHitTime += Time.deltaTime;
			yield return stagePause_.sync();
		}
		if (state != eState.canHitDamage)
		{
			float elapsedTime = 0f;
			float targetTime = 2.7f;
			if (5f - canHitTime <= 3f)
			{
				targetTime = 5f - canHitTime;
			}
			while (elapsedTime < targetTime && state != eState.moving)
			{
				elapsedTime += Time.deltaTime;
				yield return stagePause_.sync();
			}
		}
		if (currentHP > 0)
		{
			animation.clip = animation["Boss_00_damage_end_anm"].clip;
			animation.Play("Boss_00_damage_end_anm");
			isCanHit = false;
			knockoutEff.SetActive(false);
			isWaitBossAction = false;
			while (animation.IsPlaying("Boss_00_damage_end_anm") && !isWaitBossAction)
			{
				yield return stagePause_.sync();
			}
		}
		while (bossPart_.isWaitStageAcction)
		{
			yield return stagePause_.sync();
		}
		if (bossPart_.fulcrumList.Count == 0 && currentHP > 0)
		{
			bossPart_.scrollTime = (bossPart_.scrollTimeBefor = Time.time);
			yield return StartCoroutine(bossPart_.recreateImmediateRoutine());
		}
		if (base.transform.position.x > 0f && currentHP > 0)
		{
			base.transform.eulerAngles = Vector3.zero;
		}
		state = eState.moving;
		bTurnOff = false;
		yield return stagePause_.sync();
	}

	private IEnumerator DamagedRoutine()
	{
		Sound.Instance.playSe(Sound.eSe.SE_540_fukurou_damage);
		Damage();
		Sound.Instance.playSe(Sound.eSe.SE_228_bossdmg);
		animation.clip = animation["Boss_00_damage_anm"].clip;
		animation.Play("Boss_00_damage_anm");
		float hp = bossHpSlider.sliderValue;
		while (hp > (float)currentHP / (float)maxHP)
		{
			hp -= Time.deltaTime;
			bossHpSlider.sliderValue = hp;
			yield return stagePause_.sync();
		}
		while (animation.IsPlaying("Boss_00_damage_anm"))
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
		while (animation.IsPlaying("Boss_00_damage_end_anm"))
		{
			yield return stagePause_.sync();
		}
	}

	private IEnumerator DeadRoutine()
	{
		animation.clip = animation["Boss_00_dead_anm"].clip;
		animation.Play("Boss_00_dead_anm");
		while (animation.IsPlaying("Boss_00_dead_anm"))
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

	private IEnumerator WaitRoutine()
	{
		pelvis.transform.localRotation = Quaternion.identity;
		base.transform.position = basePos;
		animation.clip = animation["Boss_00_move_anm"].clip;
		animation.Play("Boss_00_move_anm");
		while (isWaitting)
		{
			yield return stagePause_.sync();
		}
	}

	public override void HitChackn()
	{
		if (state == eState.moving || state == eState.attacking)
		{
			canHitTime = 0f;
			state = eState.canHitDamage;
			isMoving = false;
			isCanHit = true;
			isCanHitChackn = false;
			Sound.Instance.playSe(Sound.eSe.SE_247_bosspiyo);
			knockoutEff.SetActive(true);
			if (base.transform.position.x > 0f)
			{
				base.transform.eulerAngles = turnRotate;
			}
			Sound.Instance.playSe(Sound.eSe.SE_247_bosspiyo);
			StartCoroutine(requestTurnOffHitFlag());
		}
	}

	private IEnumerator MovingPositionRoutine()
	{
		isMoving = true;
		float move_time = tempMoveTime;
		while (isMoving && move_time < ONE_MOVE_ROOP_TIME * (float)MOVE_LOOP_COUNT)
		{
			float angle = move_time / ONE_MOVE_ROOP_TIME * (float)Math.PI - 1.575f;
			float x = baseX + 0.3f * Mathf.Cos(angle);
			float y = baseY + 0.075f * Mathf.Sin(angle * 2f);
			base.transform.position = new Vector3(x, y, base.transform.position.z);
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
			switch (hitInfo.collider.name)
			{
			case "body_collider":
				if (isCanHit)
				{
					return eBossHitState.Hit;
				}
				return eBossHitState.Diffend;
			default:
				if (isCanHit)
				{
					return eBossHitState.Hit;
				}
				return eBossHitState.Diffend;
			}
		}
		return eBossHitState.None;
	}

	public override eBossHitState CheckHitChackn(Vector3 start, Vector3 end)
	{
		Vector3 direction = end - start;
		float radius = Bubble.hitSize * bossPart_.uiScale * 0.5f;
		int layerMask = 1 << LayerMask.NameToLayer("BossCollider");
		RaycastHit hitInfo;
		if (Physics.SphereCast(start, radius, direction, out hitInfo, direction.magnitude, layerMask))
		{
			switch (hitInfo.collider.name)
			{
			case "body_collider":
				if (isCanHitChackn)
				{
					return eBossHitState.Hit;
				}
				return eBossHitState.None;
			default:
				if (isCanHitChackn)
				{
					return eBossHitState.Hit;
				}
				return eBossHitState.None;
			}
		}
		return eBossHitState.None;
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
		base.transform.position = basePos;
		tempMoveTime = 0f;
		float elapsedTime = 0f;
		while (elapsedTime < 0.4f)
		{
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		isWaitting = false;
		state = eState.moving;
	}
}
