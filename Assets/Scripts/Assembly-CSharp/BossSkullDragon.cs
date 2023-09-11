using System;
using System.Collections;
using Network;
using UnityEngine;

public class BossSkullDragon : BossBase
{
	private const float ANGRY_ANMATION_TIME = 6f;

	private const float CENTER_ANGLE_OFFSET = 1.575f;

	private const float BOSS_OP_ANIMATION_TIME = 5f;

	private const float FEELD_BUBBLE_FADE_TIME = 1.5f;

	private const float attackEventTime = 0.3f;

	private const float canHitEventTime = 3f;

	private const float attackLoopTime = 3f;

	private const float TURN_OFF_HIT_FLAG_TIME = 3.3f;

	private const float EGG_SE_WAIT_TIME = 0.35f;

	private Vector3 headBasePos;

	private float headBaseX;

	private float headBaseY;

	private Vector3 basePos;

	private float basePosX;

	private GameObject head_root;

	private GameObject tongue;

	private GameObject Boss_Anm_root;

	private GameObject Boss_eff01;

	private float ONE_MOVE_ROOP_TIME = 10f;

	private int MOVE_LOOP_COUNT = 2;

	private int hitCount;

	private bool isRegenerative;

	private bool isSecondShout = true;

	private Vector3 angryEffOffset = new Vector3(0f, -0.14f, 0f);

	private float tempMoveTime;

	private static int se_count;

	public override void SetupBoss(int level, float moveTime, int attackSpan)
	{
		BossDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<BossDataTable>();
		BossListData bossData = component.getBossData();
		BossListData.BossLevelData[] bossLevelList = bossData.bossList[2].bossLevelList;
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
		bType = BossStageInfo.eBossType.SkullDragon;
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
		bossGauge = frontUi.Find("Top_ui").Find("Boss_gauge").gameObject;
		head_root = GameObject.Find("head_root");
		tongue = GameObject.Find("tongue");
		Boss_Anm_root = base.transform.Find("Boss_Anm_root").gameObject;
		Boss_eff01 = Boss_Anm_root.transform.Find("Boss_eff01").gameObject;
		headBasePos = head_root.transform.position;
		headBaseX = head_root.transform.position.x;
		headBaseY = head_root.transform.position.y;
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
			Sound.Instance.playSe(Sound.eSe.SE_550_skelton_attack);
			isWaitBossAction = true;
			yield return StartCoroutine(bossPart_.BubblenKnockout());
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
			case eState.damaged:
				yield return StartCoroutine(DamagedRoutine());
				break;
			case eState.dead:
				yield return StartCoroutine(DeadRoutine());
				break;
			case eState.angry:
				yield return StartCoroutine(AngryRoutine());
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
		base.gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 1f);
		Boss_Anm_root.SetActive(true);
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
		yield return stagePause_.sync();
	}

	private IEnumerator MovingRoutine()
	{
		animation.clip = animation["Boss_02_move_anm"].clip;
		animation.Play("Boss_02_move_anm");
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
			tongue.SetActive(true);
			UISpriteAnimationEx tongueAnim = tongue.transform.Find("bg02").GetComponent<UISpriteAnimationEx>();
			tongueAnim.enabled = true;
			tongueAnim.SetClip("tongue");
			animation.clip = animation["Boss_02_attack_anm"].clip;
			animation.Play("Boss_02_attack_anm");
			if (!successedAttack2)
			{
				yield return StartCoroutine(Attack());
				successedAttack2 = true;
			}
			float elapsedTime = 0f;
			while (animation.IsPlaying("Boss_02_attack_anm") && elapsedTime <= 3f)
			{
				elapsedTime += Time.deltaTime;
				yield return stagePause_.sync();
			}
			while (tongueAnim.isPlaying)
			{
				yield return stagePause_.sync();
			}
			tongueAnim.enabled = false;
			tongue.SetActive(false);
		}
		if (state == eState.attacking)
		{
			state = eState.moving;
		}
	}

	private IEnumerator requestTurnOffHitFlag()
	{
		isRegenerative = true;
		float canHitTime = 0f;
		while (canHitTime < 6f && state == eState.angry)
		{
			canHitTime += Time.deltaTime;
			yield return stagePause_.sync();
		}
		if (state == eState.damaged)
		{
			isSecondShout = 6f - canHitTime >= 3.3f;
			float elapsedTime = 0f;
			float targetTime3 = 3.3f;
			if (isSecondShout)
			{
				targetTime3 = 6f - canHitTime + 1.3f;
				while (elapsedTime < targetTime3)
				{
					elapsedTime += Time.deltaTime;
					yield return stagePause_.sync();
				}
			}
			else
			{
				targetTime3 = 1.3f;
				while (elapsedTime < targetTime3)
				{
					elapsedTime += Time.deltaTime;
					yield return stagePause_.sync();
				}
			}
		}
		isCanHit = false;
		isWaitBossAction = false;
		StartCoroutine(egg_.RegenerativeEgg());
		state = eState.moving;
		isRegenerative = false;
		isSecondShout = true;
		Boss_eff01.SetActive(false);
		yield return stagePause_.sync();
	}

	private IEnumerator DamagedRoutine()
	{
		hitCount++;
		Damage();
		Sound.Instance.playSe(Sound.eSe.SE_548_skelton_damage);
		if (currentHP > 0)
		{
			PlayerPrefs.SetInt("current_Skull", currentHP);
		}
		else if (currentHP <= 0)
		{
			PlayerPrefs.DeleteKey("current_Skull");
		}
		animation.clip = animation["Boss_02_damage_anm"].clip;
		animation.Play("Boss_02_damage_anm");
		float i = bossHpSlider.sliderValue;
		while (i > (float)currentHP / (float)maxHP)
		{
			i -= Time.deltaTime;
			bossHpSlider.sliderValue = i;
			yield return stagePause_.sync();
		}
		while (animation.IsPlaying("Boss_02_damage_anm"))
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
		else if (isCanHit && hitCount < 2)
		{
			state = eState.angry;
		}
		else
		{
			state = eState.moving;
			isCanHit = false;
			hitCount = 0;
		}
		yield return stagePause_.sync();
	}

	private IEnumerator DeadRoutine()
	{
		animation.clip = animation["Boss_02_dead_anm"].clip;
		animation.Play("Boss_02_dead_anm");
		while (animation.IsPlaying("Boss_02_dead_anm"))
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

	private IEnumerator AngryRoutine()
	{
		if (!isRegenerative)
		{
			StartCoroutine(requestTurnOffHitFlag());
		}
		if (isSecondShout)
		{
			StartCoroutine(waitSE(Sound.eSe.SE_551_skelton_sakebi, 0.35f));
		}
		Boss_eff01.transform.position = head_root.transform.Find("head").transform.position + angryEffOffset;
		Boss_eff01.SetActive(true);
		animation.GetClip("Boss_02_angry_anm").wrapMode = WrapMode.Loop;
		animation.clip = animation["Boss_02_angry_anm"].clip;
		animation.Play("Boss_02_angry_anm");
		while (state == eState.angry)
		{
			yield return stagePause_.sync();
		}
		Sound.Instance.stopSe(Sound.eSe.SE_551_skelton_sakebi);
	}

	private IEnumerator WaitRoutine()
	{
		base.transform.position = basePos;
		head_root.transform.position = headBasePos;
		animation.clip = animation["Boss_02_move_anm"].clip;
		animation.Play("Boss_02_move_anm");
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
			float x = headBaseX + 0.3f * Mathf.Cos(angle);
			float y = headBaseY + 0.04f * Mathf.Sin(angle * 6f);
			float bodyX = basePosX + 0.1f * Mathf.Cos(angle);
			head_root.transform.position = new Vector3(x, y, myTrans.position.z);
			myTrans.position = new Vector3(bodyX, base.transform.position.y, myTrans.position.z);
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
			case "chin_collider":
				if (isCanHit)
				{
					return eBossHitState.Hit;
				}
				return eBossHitState.Diffend;
			default:
				return eBossHitState.Diffend;
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
		egg_.ImmediatelyRegenerativeEgg();
		yield return 0;
	}

	public override IEnumerator ContinueSetup()
	{
		state = eState.None;
		isMoving = false;
		isCanHit = false;
		tempMoveTime = 0f;
		base.transform.position = basePos;
		float elapsedTime = 0f;
		while (elapsedTime < 0.4f)
		{
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		isWaitting = false;
		state = eState.moving;
	}

	public static void playMovingSE()
	{
		se_count++;
		if (se_count > 10)
		{
			Sound.Instance.playSe(Sound.eSe.SE_549_skelton_glowl);
			se_count = 0;
		}
	}
}
