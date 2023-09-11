using System;
using System.Collections;
using Network;
using UnityEngine;

public class BossCrab : BossBase
{
	public enum moveState
	{
		wait = 0,
		center_left = 1,
		center_right = 2,
		left_center = 3,
		right_center = 4
	}

	private const float attackLoopTime = 2f;

	private const float BOSS_OP_ANIMATION_TIME = 5f;

	private const float FEELD_BUBBLE_FADE_TIME = 1.5f;

	private const float wait_kani_jyakin = 0.35f;

	private const float attackEventTime = 0.3f;

	private const float canHitEventTime = 3f;

	private const float REGENERATIVE_TIME = 8f;

	private const float TURN_OFF_HIT_FLAG_TIME = 3f;

	private const float KNOCKOUT_TIME = 6f;

	private const float wait_kani_damage_Time = 0.2f;

	private const float leftX = -0.3f;

	private const float rightX = 0.3f;

	private const float ONE_WAIT_TIME = 2f;

	private const float ONE_MOVE_TIME = 1f;

	private Vector3 basePos;

	private float baseX;

	private float baseY;

	private float baseZ;

	private GameObject claw_L;

	private GameObject claw_R;

	public GameObject[] jointBubble_L;

	public GameObject[] jointBubble_R;

	private tk2dAnimatedSprite[] bubbleSprites_L = new tk2dAnimatedSprite[2];

	private tk2dAnimatedSprite[] bubbleSprites_R = new tk2dAnimatedSprite[2];

	private UISprite[] clawSpriteL;

	private UISprite[] clawSpriteR;

	private GameObject Boss_Anm_root;

	private GameObject Boss_eff01;

	private GameObject eff01_bg00;

	private GameObject[] eff01_knockout;

	private GameObject hand_eff_L;

	private GameObject hand_eff_R;

	private System.Random random = new System.Random();

	private int MOVE_LOOP_COUNT = 8;

	private bool isRegenerative;

	private eState prevState = eState.None;

	public moveState mState = moveState.center_left;

	public moveState nextState = moveState.left_center;

	private float tempMoveTime;

	private float correctionValue = 1f;

	private int moveCount;

	private string hitColliderName;

	private int jointBubbleTypeL;

	private int jointBubbleTypeR;

	private bool existCraw_L = true;

	private bool existCraw_R = true;

	private bool isRegenerativeLeft;

	private bool isRegenerativeRight;

	public override void SetupBoss(int level, float moveTime, int attackSpan)
	{
		BossDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<BossDataTable>();
		BossListData bossData = component.getBossData();
		BossListData.BossLevelData[] bossLevelList = bossData.bossList[1].bossLevelList;
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
		MOVE_LOOP_COUNT = attackSpan;
		bType = BossStageInfo.eBossType.Crab;
		isCanHitJointBubbleL = true;
		isCanHitJointBubbleR = true;
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
		claw_L = GameObject.Find("hand00_L/ scissors_root");
		claw_R = GameObject.Find("hand00_R/ scissors_root");
		Boss_Anm_root = base.transform.Find("Boss_Anm_root").gameObject;
		Boss_eff01 = Boss_Anm_root.transform.Find("Boss_eff01").gameObject;
		eff01_bg00 = Boss_eff01.transform.Find("bg00").gameObject;
		hand_eff_L = claw_L.transform.Find("hand_eff").gameObject;
		hand_eff_R = claw_R.transform.Find("hand_eff").gameObject;
		eff01_knockout = new GameObject[Boss_eff01.transform.childCount - 1];
		for (int i = 0; i < eff01_knockout.Length; i++)
		{
			eff01_knockout[i] = Boss_eff01.transform.Find("bg" + (i + 1).ToString("00")).gameObject;
		}
		clawSpriteL = new UISprite[2];
		clawSpriteR = new UISprite[2];
		clawSpriteL[0] = claw_L.transform.Find("hand01_L/bg00").gameObject.GetComponent<UISprite>();
		clawSpriteL[1] = claw_L.transform.Find("hand02_L/bg00").gameObject.GetComponent<UISprite>();
		clawSpriteR[0] = claw_R.transform.Find("hand01_R/bg00").gameObject.GetComponent<UISprite>();
		clawSpriteR[1] = claw_R.transform.Find("hand02_R/bg00").gameObject.GetComponent<UISprite>();
		usedColorList.Clear();
		jointBubbleL_SpriteNumber = colorList[random.Next(colorList.Count)];
		jointBubbleL_SpriteName = "bubble_" + jointBubbleL_SpriteNumber.ToString("00");
		jointBubbleTypeL = jointBubbleL_SpriteNumber;
		jointBubbleR_SpriteNumber = colorList[random.Next(colorList.Count)];
		jointBubbleR_SpriteName = "bubble_" + jointBubbleR_SpriteNumber.ToString("00");
		jointBubbleTypeR = jointBubbleR_SpriteNumber;
		StartCoroutine(SetJointBubbleL());
		StartCoroutine(SetJointBubbleR());
		animation = GetComponentInChildren<Animation>();
		Boss_Anm_root.SetActive(false);
		bubbleRoot.gameObject.SetActive(false);
		tempMoveTime = 0f;
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
			while (bossPart_.isWaitStageAcction)
			{
				yield return stagePause_.sync();
			}
			isWaitBossAction = true;
			yield return StartCoroutine(bossPart_.freeDropBubbleRoutine(3));
			Sound.Instance.playSe(Sound.eSe.SE_544_kani_jyakin);
			StartCoroutine(waitSE(Sound.eSe.SE_544_kani_jyakin, 0.35f));
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
				prevState = eState.None;
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
		baseX = base.transform.position.x;
		baseY = base.transform.position.y;
		baseZ = base.transform.position.z;
		StartCoroutine(BossHpSet());
		state = eState.moving;
		yield return stagePause_.sync();
	}

	private IEnumerator MovingRoutine()
	{
		animation.clip = animation["Boss_01_move_anm"].clip;
		animation.Play("Boss_01_move_anm");
		while (bossPart_.state == Part_BossStage.eState.Start || bossPart_.state == Part_BossStage.eState.Gameover)
		{
			yield return stagePause_.sync();
		}
		yield return StartCoroutine(MovingPositionRoutine());
		if (state == eState.moving)
		{
			state = eState.attacking;
		}
	}

	private IEnumerator AttackingRoutine()
	{
		if (bossPart_.state != Part_BossStage.eState.Gameover)
		{
			while (bossPart_.state != Part_BossStage.eState.Wait && state == eState.attacking)
			{
				yield return stagePause_.sync();
			}
			bool successedAttack2 = false;
			float elapsedTime = 0f;
			eff01_bg00.SetActive(true);
			hand_eff_L.SetActive(true);
			hand_eff_R.SetActive(true);
			animation.clip = animation["Boss_01_attack_anm"].clip;
			animation.Play("Boss_01_attack_anm");
			if (!successedAttack2)
			{
				yield return StartCoroutine(Attack());
				successedAttack2 = true;
			}
			while (animation.IsPlaying("Boss_01_attack_anm") && state == eState.attacking && elapsedTime <= 2f)
			{
				yield return stagePause_.sync();
			}
			eff01_bg00.SetActive(false);
			hand_eff_L.SetActive(false);
			hand_eff_R.SetActive(false);
		}
		if (state == eState.attacking)
		{
			state = eState.moving;
		}
	}

	private IEnumerator CanHitDamageRoutine()
	{
		moveCount = 0;
		for (int i = 0; i < eff01_knockout.Length; i++)
		{
			eff01_knockout[i].SetActive(true);
		}
		animation.clip = animation["Boss_01_knockout_anm"].clip;
		animation.Play("Boss_01_knockout_anm");
		while (animation.IsPlaying("Boss_01_knockout_anm") && state == eState.canHitDamage)
		{
			yield return stagePause_.sync();
		}
		prevState = eState.canHitDamage;
	}

	private IEnumerator requestRegenerativeFlag()
	{
		float canHitTime = 0f;
		while (canHitTime < 8f && (state == eState.attacking || state == eState.moving))
		{
			canHitTime += Time.deltaTime;
			yield return stagePause_.sync();
		}
		if (state == eState.canHitDamage)
		{
			float elapsedTime3 = 0f;
			while (elapsedTime3 < 6f && state == eState.canHitDamage)
			{
				elapsedTime3 += Time.deltaTime;
				yield return stagePause_.sync();
			}
			if (state == eState.damaged)
			{
				elapsedTime3 = 0f;
				while (elapsedTime3 < 3f)
				{
					elapsedTime3 += Time.deltaTime;
					yield return stagePause_.sync();
				}
			}
			for (int i = 0; i < eff01_knockout.Length; i++)
			{
				eff01_knockout[i].SetActive(false);
			}
			eff01_bg00.SetActive(false);
		}
		else if (state == eState.damaged)
		{
			float elapsedTime = 0f;
			float targetTime = 3f;
			if (8f - canHitTime < 3f)
			{
				targetTime = 8f - canHitTime;
			}
			while (elapsedTime < targetTime)
			{
				elapsedTime += Time.deltaTime;
				yield return stagePause_.sync();
			}
		}
		isCanHit = false;
		isWaitBossAction = false;
		prevState = eState.None;
		yield return StartCoroutine(RegenerativeCraw());
		if (bossPart_.state != Part_BossStage.eState.Gameover)
		{
			state = eState.moving;
		}
		yield return stagePause_.sync();
		isRegenerative = false;
		isCanHitJointBubbleL = true;
		isCanHitJointBubbleR = true;
	}

	private IEnumerator DamagedRoutine()
	{
		Damage();
		StartCoroutine(waitSE(Sound.eSe.SE_543_kani_damage, 0.2f));
		if (currentHP > 0)
		{
			isWaitBossAction = false;
		}
		animation.clip = animation["Boss_01_damage_anm"].clip;
		animation.Play("Boss_01_damage_anm");
		float i = bossHpSlider.sliderValue;
		while (i > (float)currentHP / (float)maxHP)
		{
			i -= Time.deltaTime;
			bossHpSlider.sliderValue = i;
			yield return stagePause_.sync();
		}
		while (animation.IsPlaying("Boss_01_damage_anm"))
		{
			yield return stagePause_.sync();
		}
		if (currentHP <= 0)
		{
			state = eState.dead;
		}
		else if (isWaitBossAction)
		{
			state = eState.damaged;
		}
		else if (isCanHit && prevState == eState.canHitDamage)
		{
			state = eState.canHitDamage;
		}
		else
		{
			state = eState.moving;
		}
		while (isRegenerativeLeft || (isRegenerativeRight && isRegenerative))
		{
			yield return stagePause_.sync();
		}
		yield return stagePause_.sync();
	}

	private IEnumerator DeadRoutine()
	{
		animation.clip = animation["Boss_01_dead_anm"].clip;
		animation.Play("Boss_01_dead_anm");
		while (animation.IsPlaying("Boss_01_dead_anm"))
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
		base.transform.position = basePos;
		animation.clip = animation["Boss_01_move_anm"].clip;
		animation.Play("Boss_01_move_anm");
		while (isWaitting)
		{
			yield return stagePause_.sync();
		}
	}

	private IEnumerator MovingPositionRoutine()
	{
		isMoving = true;
		while (isMoving && moveCount < MOVE_LOOP_COUNT)
		{
			float oneMoveTime = 1f * correctionValue;
			float move_time = tempMoveTime;
			float oneWaitTime = 2f * correctionValue;
			switch (mState)
			{
			case moveState.center_left:
				while (isMoving && move_time < oneMoveTime)
				{
					myTrans.position = new Vector3(-0.3f * (move_time / oneMoveTime), baseY, baseZ);
					move_time += Time.deltaTime;
					yield return stagePause_.sync();
				}
				if (isMoving)
				{
					myTrans.position = new Vector3(-0.3f, baseY, baseZ);
					nextState = moveState.left_center;
					mState = moveState.wait;
				}
				break;
			case moveState.left_center:
				while (isMoving && move_time < oneMoveTime)
				{
					myTrans.position = new Vector3(-0.3f * (1f - move_time / oneMoveTime), baseY, baseZ);
					move_time += Time.deltaTime;
					yield return stagePause_.sync();
				}
				if (isMoving)
				{
					myTrans.position = new Vector3(baseX, baseY, baseZ);
					nextState = moveState.center_right;
					mState = moveState.wait;
				}
				break;
			case moveState.center_right:
				while (isMoving && move_time < oneMoveTime)
				{
					myTrans.position = new Vector3(0.3f * (move_time / oneMoveTime), baseY, baseZ);
					move_time += Time.deltaTime;
					yield return stagePause_.sync();
				}
				if (isMoving)
				{
					myTrans.position = new Vector3(0.3f, baseY, baseZ);
					nextState = moveState.right_center;
					mState = moveState.wait;
				}
				break;
			case moveState.right_center:
				while (isMoving && move_time < oneMoveTime)
				{
					myTrans.position = new Vector3(0.3f * (1f - move_time / oneMoveTime), baseY, baseZ);
					move_time += Time.deltaTime;
					yield return stagePause_.sync();
				}
				if (isMoving)
				{
					myTrans.position = new Vector3(baseX, baseY, baseZ);
					nextState = moveState.center_left;
					mState = moveState.wait;
				}
				break;
			case moveState.wait:
				while (isMoving && move_time < oneWaitTime)
				{
					move_time += Time.deltaTime;
					yield return stagePause_.sync();
				}
				if (isMoving)
				{
					mState = nextState;
					nextState = moveState.wait;
				}
				break;
			}
			if (isMoving)
			{
				tempMoveTime = 0f;
				moveCount++;
			}
			else
			{
				tempMoveTime = move_time;
			}
			yield return stagePause_.sync();
		}
		if (moveCount >= MOVE_LOOP_COUNT)
		{
			moveCount = 0;
		}
	}

	public override eBossHitState CheckHitBoss(Vector3 start, Vector3 end, int bubbleType)
	{
		Vector3 direction = end - start;
		float radius = Bubble.hitSize * bossPart_.uiScale * 0.5f;
		int layerMask = 1 << LayerMask.NameToLayer("BossCollider");
		RaycastHit hitInfo;
		if (Physics.SphereCast(start, radius, direction, out hitInfo, direction.magnitude, layerMask))
		{
			Debug.Log("hit.collider.name = " + hitInfo.collider.name);
			switch (hitInfo.collider.name)
			{
			case "eye_L_collider":
				if (isCanHit)
				{
					return eBossHitState.Hit;
				}
				return eBossHitState.Diffend;
			case "eye_R_collider":
				if (isCanHit)
				{
					return eBossHitState.Hit;
				}
				return eBossHitState.Diffend;
			case "body_collider":
				return eBossHitState.Diffend;
			case "bubble_R_collider":
				if (jointBubbleTypeR == bubbleType && isCanHitJointBubbleR)
				{
					hitColliderName = "bubble_R_collider";
					return eBossHitState.Bubble;
				}
				return eBossHitState.Diffend;
			case "bubble_L_collider":
				if (jointBubbleTypeL == bubbleType && isCanHitJointBubbleL)
				{
					hitColliderName = "bubble_L_collider";
					return eBossHitState.Bubble;
				}
				return eBossHitState.Diffend;
			default:
				return eBossHitState.Diffend;
			}
		}
		return eBossHitState.None;
	}

	public IEnumerator SetJointBubbleL()
	{
		jointBubbleL_SpriteNumber = colorList[random.Next(colorList.Count)];
		jointBubbleL_SpriteName = "bubble_" + jointBubbleL_SpriteNumber.ToString("00");
		jointBubbleTypeL = jointBubbleL_SpriteNumber;
		for (int i = 0; i < jointBubble_L.Length; i++)
		{
			bubbleSprites_L[i] = jointBubble_L[i].transform.Find("AS_spr_bubble").GetComponent<tk2dAnimatedSprite>();
			bubbleSprites_L[i].Play(jointBubbleL_SpriteName);
			yield return stagePause_.sync();
		}
		UpdateUsedColorList();
	}

	public IEnumerator SetJointBubbleR()
	{
		jointBubbleR_SpriteNumber = colorList[random.Next(colorList.Count)];
		jointBubbleR_SpriteName = "bubble_" + jointBubbleR_SpriteNumber.ToString("00");
		jointBubbleTypeR = jointBubbleR_SpriteNumber;
		for (int i = 0; i < jointBubble_R.Length; i++)
		{
			bubbleSprites_R[i] = jointBubble_R[i].transform.Find("AS_spr_bubble").GetComponent<tk2dAnimatedSprite>();
			bubbleSprites_R[i].Play(jointBubbleR_SpriteName);
			yield return stagePause_.sync();
		}
		UpdateUsedColorList();
	}

	private void UpdateUsedColorList()
	{
		usedColorList.Clear();
		if (isCanHitJointBubbleL)
		{
			usedColorList.Add(jointBubbleL_SpriteNumber);
		}
		if (isCanHitJointBubbleR)
		{
			usedColorList.Add(jointBubbleR_SpriteNumber);
		}
	}

	public override IEnumerator Breaking()
	{
		Sound.Instance.playSe(Sound.eSe.SE_542_kani_arm_broken);
		StartCoroutine(BurstJointBubble());
		StartCoroutine(BreakClaw());
		isCanHit = true;
		if (!isRegenerative)
		{
			StartCoroutine(requestRegenerativeFlag());
		}
		yield return null;
	}

	public IEnumerator BurstJointBubble()
	{
		if (hitColliderName == "bubble_L_collider")
		{
			isCanHitJointBubbleL = false;
			UpdateUsedColorList();
			for (int l = 0; l < bubbleSprites_L.Length; l++)
			{
				bubbleSprites_L[l].Play(jointBubbleL_SpriteName.Replace("bubble_", "burst_"));
			}
			Color[] color2 = new Color[bubbleSprites_L.Length];
			for (int k = 0; k < bubbleSprites_L.Length; k++)
			{
				while (bubbleSprites_L[k].IsPlaying(jointBubbleR_SpriteName.Replace("bubble_", "burst_")))
				{
					yield return stagePause_.sync();
				}
				color2[k].a = 0f;
				bubbleSprites_L[k].color = color2[k];
				jointBubble_L[k].SetActive(false);
			}
		}
		else if (hitColliderName == "bubble_R_collider")
		{
			isCanHitJointBubbleR = false;
			UpdateUsedColorList();
			for (int j = 0; j < bubbleSprites_R.Length; j++)
			{
				bubbleSprites_R[j].Play(jointBubbleL_SpriteName.Replace("bubble_", "burst_"));
			}
			Color[] color = new Color[bubbleSprites_R.Length];
			for (int i = 0; i < bubbleSprites_R.Length; i++)
			{
				while (bubbleSprites_R[i].IsPlaying(jointBubbleR_SpriteName.Replace("bubble_", "burst_")))
				{
					yield return stagePause_.sync();
				}
				color[i].a = 0f;
				bubbleSprites_R[i].color = color[i];
				jointBubble_R[i].SetActive(false);
			}
		}
		yield return stagePause_.sync();
	}

	public IEnumerator BreakClaw()
	{
		isCanHit = true;
		if (hitColliderName == "bubble_L_collider")
		{
			existCraw_L = false;
			if (!existCraw_L && !existCraw_R && !isRegenerativeRight)
			{
				state = eState.canHitDamage;
				Sound.Instance.playSe(Sound.eSe.SE_247_bosspiyo);
				isMoving = false;
			}
			else
			{
				correctionValue = 0.5f;
			}
			Vector3 basePos = claw_L.transform.localPosition;
			Vector3 vec = new Vector3(0.5f, 1.3f, 0f);
			while (claw_L.transform.position.y < 2f)
			{
				claw_L.transform.position += vec * Time.deltaTime;
				yield return null;
			}
			for (int j = 0; j < clawSpriteL.Length; j++)
			{
				Color[] color2 = new Color[clawSpriteL.Length];
				color2[j].a = 0f;
				clawSpriteL[j].color = color2[j];
			}
			claw_L.SetActive(false);
			claw_L.transform.localPosition = basePos;
		}
		else if (hitColliderName == "bubble_R_collider")
		{
			existCraw_R = false;
			if (!existCraw_L && !existCraw_R && !isRegenerativeLeft)
			{
				state = eState.canHitDamage;
				Sound.Instance.playSe(Sound.eSe.SE_247_bosspiyo);
				isMoving = false;
			}
			else
			{
				correctionValue = 0.5f;
			}
			Vector3 basePos2 = claw_R.transform.localPosition;
			Vector3 vec2 = new Vector3(-0.5f, 1.3f, 0f);
			while (claw_R.transform.position.y < 2f)
			{
				claw_R.transform.position += vec2 * Time.deltaTime;
				yield return null;
			}
			for (int i = 0; i < clawSpriteR.Length; i++)
			{
				Color[] color = new Color[clawSpriteR.Length];
				color[i].a = 0f;
				clawSpriteR[i].color = color[i];
			}
			claw_R.SetActive(false);
			claw_R.transform.localPosition = basePos2;
		}
	}

	public IEnumerator RegenerativeCraw()
	{
		correctionValue = 1f;
		isRegenerativeLeft = true;
		isRegenerativeRight = true;
		StartCoroutine(RegenerativeLeftCraw());
		StartCoroutine(RegenerativeRigthCraw());
		while (isRegenerativeLeft || isRegenerativeRight)
		{
			yield return stagePause_.sync();
		}
	}

	public IEnumerator RegenerativeLeftCraw()
	{
		float speed = 1f;
		if (!existCraw_L)
		{
			existCraw_L = true;
			yield return StartCoroutine(SetJointBubbleL());
			claw_L.SetActive(true);
			Color[] color = new Color[clawSpriteL.Length];
			for (int k = 0; k < clawSpriteL.Length; k++)
			{
				color[k] = clawSpriteL[k].color;
				color[k].r = 1f;
				color[k].g = 1f;
				color[k].b = 1f;
				clawSpriteL[k].color = color[k];
			}
			bool setColor = false;
			while (!setColor)
			{
				for (int i = 0; i < clawSpriteL.Length; i++)
				{
					color[i].a += Time.deltaTime * speed;
					clawSpriteL[i].color = color[i];
					if (color[i].a >= 1f)
					{
						setColor = true;
					}
				}
				yield return stagePause_.sync();
			}
			Color[] bubbleColor = new Color[bubbleSprites_L.Length];
			for (int l = 0; l < bubbleSprites_L.Length; l++)
			{
				bubbleColor[l] = bubbleSprites_L[l].color;
				bubbleColor[l].r = 1f;
				bubbleColor[l].g = 1f;
				bubbleColor[l].b = 1f;
				bubbleSprites_L[l].Play(jointBubbleL_SpriteName);
				jointBubble_L[l].SetActive(true);
			}
			bool setBubbleColor = false;
			while (!setBubbleColor)
			{
				for (int j = 0; j < bubbleSprites_L.Length; j++)
				{
					bubbleColor[j].a += Time.deltaTime * speed;
					bubbleSprites_L[j].color = bubbleColor[j];
					if (bubbleColor[j].a >= 1f)
					{
						setBubbleColor = true;
					}
				}
				yield return stagePause_.sync();
			}
		}
		hitColliderName = string.Empty;
		isRegenerativeLeft = false;
		yield return stagePause_.sync();
	}

	public IEnumerator RegenerativeRigthCraw()
	{
		float speed = 1f;
		if (!existCraw_R)
		{
			existCraw_R = true;
			yield return StartCoroutine(SetJointBubbleR());
			claw_R.SetActive(true);
			Color[] color = new Color[clawSpriteR.Length];
			for (int k = 0; k < clawSpriteR.Length; k++)
			{
				color[k] = clawSpriteR[k].color;
				color[k].r = 1f;
				color[k].g = 1f;
				color[k].b = 1f;
				clawSpriteR[k].color = color[k];
			}
			bool setColor = false;
			while (!setColor)
			{
				for (int i = 0; i < clawSpriteR.Length; i++)
				{
					color[i].a += Time.deltaTime * speed;
					clawSpriteR[i].color = color[i];
					if (color[i].a >= 1f)
					{
						setColor = true;
					}
				}
				yield return stagePause_.sync();
			}
			Color[] bubbleColor = new Color[bubbleSprites_R.Length];
			for (int l = 0; l < bubbleSprites_R.Length; l++)
			{
				bubbleColor[l] = bubbleSprites_R[l].color;
				bubbleColor[l].r = 1f;
				bubbleColor[l].g = 1f;
				bubbleColor[l].b = 1f;
				bubbleSprites_R[l].Play(jointBubbleR_SpriteName);
				jointBubble_R[l].SetActive(true);
			}
			bool setBubbleColor = false;
			while (!setBubbleColor)
			{
				for (int j = 0; j < bubbleSprites_R.Length; j++)
				{
					bubbleColor[j].a += Time.deltaTime * speed;
					bubbleSprites_R[j].color = bubbleColor[j];
					if (bubbleColor[j].a >= 1f)
					{
						setBubbleColor = true;
					}
				}
				yield return stagePause_.sync();
			}
		}
		hitColliderName = string.Empty;
		isRegenerativeRight = false;
		yield return stagePause_.sync();
	}

	public override IEnumerator GameoverWait()
	{
		isWaitting = true;
		state = eState.None;
		isMoving = false;
		isCanHit = false;
		prevState = eState.None;
		StartCoroutine(RegenerativeCraw());
		yield return 0;
	}

	public override IEnumerator ContinueSetup()
	{
		base.transform.position = basePos;
		isRegenerative = false;
		isMoving = false;
		isCanHit = false;
		isCanHitJointBubbleL = true;
		isCanHitJointBubbleR = true;
		existCraw_L = true;
		existCraw_R = true;
		moveCount = 0;
		mState = moveState.center_left;
		tempMoveTime = 0f;
		float elapsedTime = 0f;
		while (elapsedTime < 0.4f)
		{
			elapsedTime += Time.deltaTime;
			yield return stagePause_.sync();
		}
		isWaitting = false;
		state = eState.moving;
	}
}
