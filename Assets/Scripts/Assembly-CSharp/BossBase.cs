using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBase : MonoBehaviour
{
	public enum eState
	{
		start = 0,
		moving = 1,
		attacking = 2,
		canHitDamage = 3,
		damaged = 4,
		dead = 5,
		angry = 6,
		picking = 7,
		None = 8
	}

	public enum eBossHitState
	{
		None = 0,
		Diffend = 1,
		Hit = 2,
		Bubble = 3
	}

	protected const float CONTINUE_WAIT_TIME = 0.4f;

	private const int HP_METER_STEP = 20;

	protected Transform myTrans;

	public int maxHP;

	public int currentHP;

	public int startHP;

	public int damageValue;

	public Part_BossStage bossPart_;

	public StagePause_Boss stagePause_;

	public Transform stageUi;

	public Transform frontUi;

	public Transform bubbleRoot;

	protected new Animation animation;

	public bool isWaitting;

	protected GameObject bossGauge;

	public UISlider bossHpSlider;

	public bool isCanHitChackn;

	public bool isCanHitJointBubbleL;

	public bool isCanHitJointBubbleR;

	public Egg egg_;

	public List<int> colorList = new List<int>();

	public List<int> usedColorList = new List<int>();

	protected string jointBubbleL_SpriteName;

	protected string jointBubbleR_SpriteName;

	protected int jointBubbleL_SpriteNumber;

	protected int jointBubbleR_SpriteNumber;

	public bool isCanHit;

	public bool isMoving;

	public bool isDead;

	public bool existPickObj;

	public bool isWaitBossAction;

	public eState state;

	public BossStageInfo.eBossType bType;

	public virtual void SetupBoss(int level, float moveTime, int attackSpan)
	{
	}

	public virtual IEnumerator StartAnimation()
	{
		yield break;
	}

	public IEnumerator BossHpSet()
	{
		bossGauge.SetActive(true);
		bossHpSlider = bossGauge.GetComponentInChildren<UISlider>();
		bossHpSlider.sliderValue = 0f;
		bossHpSlider.numberOfSteps = (maxHP + 1) * 20;
		if (bossGauge.GetComponent<Animation>().isPlaying)
		{
			yield return stagePause_.sync();
		}
		while (bossHpSlider.sliderValue < (float)currentHP / (float)maxHP)
		{
			bossHpSlider.sliderValue += Time.deltaTime;
			yield return stagePause_.sync();
		}
		bossHpSlider.sliderValue = (float)currentHP / (float)maxHP;
	}

	public virtual eBossHitState CheckHitChackn(Vector3 start, Vector3 end)
	{
		return eBossHitState.None;
	}

	public virtual eBossHitState CheckHitBoss(Vector3 start, Vector3 end, int bubbleType)
	{
		return eBossHitState.None;
	}

	public virtual IEnumerator Attack()
	{
		yield break;
	}

	public void Damage()
	{
		currentHP -= damageValue;
		if (currentHP < 0)
		{
			currentHP = 0;
		}
	}

	public virtual void HitChackn()
	{
	}

	public virtual IEnumerator Breaking()
	{
		yield break;
	}

	public virtual void BreakedSpiderweb()
	{
	}

	public virtual IEnumerator tailAction(Bubble_Boss bubble)
	{
		yield break;
	}

	public bool isClear()
	{
		return currentHP <= 0;
	}

	public virtual IEnumerator GameoverWait()
	{
		yield break;
	}

	public virtual IEnumerator ContinueSetup()
	{
		yield break;
	}

	public IEnumerator waitSE(Sound.eSe se, float time)
	{
		yield return new WaitForSeconds(time);
		Sound.Instance.playSe(se);
	}
}
