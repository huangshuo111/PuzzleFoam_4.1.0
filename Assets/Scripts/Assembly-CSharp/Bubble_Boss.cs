using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble_Boss : BubbleBase
{
	public const float LIMIT_X_MIN = 0f;

	public const float LIMIT_X_MAX = 540f;

	public const int OFFSET_W = 60;

	public const int OFFSET_H = 52;

	public const int HALF_SIZE = 30;

	public const float SQR_NEAR_RANGE = 8100f;

	private const float SHOCK_RANGE = 210f;

	private const float SQR_SHOCK_RANGE = 44100f;

	private const float SHAKE_RANGE = 150f;

	private const float SQR_SHAKE_RANGE = 22500f;

	public const float SQR_BOMB_RANGE = 22500f;

	private const float PRECHECK_RANGE = 90f;

	public const float SQR_PRECHECK_RANGE = 8100f;

	public const int BREAK_THRESHOLD = 3;

	public const int LINE_DEFAULT = 10;

	public const int LINE_LIMIT = 13;

	public const int LINE_BONUS_LIMIT = 15;

	public const float DROP_JUMP_HEIGHT = -420f;

	public const int dropForceX_Min = -5;

	public const int dropForceX_Rand = 11;

	public const int dropForceY_Min = 80;

	public const int dropForceY_Rand = 10;

	public const float DROP_JUMP_SNAKE_HEIGHT = -360f;

	private const int GLOSS_INTERVAL = 120;

	private const float OK_HIT_BOSS_Y = 0.35f;

	public Guide_Boss guide;

	public StagePause_Boss stagePause_;

	public Part_BossStage bossPart_;

	public Egg egg_;

	public Nest nest_;

	public List<Spiderweb> spiderwebList;

	public bool isNearSpw;

	public bool isStay;

	public tk2dAnimatedSprite sprite;

	public tk2dAnimatedSprite gloss;

	public BossBase bossBase;

	public int nativeSpriteID;

	public tk2dAnimatedSprite childAnimSp;

	public bool isSecret;

	private Counter counterCount;

	public static float SPEED = 1800f;

	public static float mHitSize = 46.5f;

	public static float SQR_SIZE = mHitSize * mHitSize;

	public static float SQR_LARGE_SIZE = mHitSize * 1.3f * (mHitSize * 1.3f);

	private Vector3 offsetUpRight = new Vector3(30f, 52f);

	private Vector3 offsetRight = new Vector3(60f, 0f);

	private Vector3 offsetDownRight = new Vector3(30f, -52f);

	private Vector3 offsetDownLeft = new Vector3(-30f, -52f);

	private Vector3 offsetLeft = new Vector3(-60f, 0f);

	private Vector3 offsetUpLeft = new Vector3(-30f, 52f);

	public int boundCount;

	private float glossTime;

	private string glossClipName;

	public int unknownColor = -1;

	private bool mLocked;

	public bool mCloud;

	private bool mPowerUp;

	public Transform powerUpEffect;

	private UISpriteAnimation powerUpEffectLoop;

	private UISpriteAnimation powerUpEffectBurst;

	public bool isHitCloud;

	public bool isLineFriend;

	public int lineFriendIndex = -1;

	private GameObject lineFriendIcon;

	public int createIndex = -1;

	public bool isOnChain;

	private static GameObject fulcrumColliderBase_ = null;

	private GameObject fulcrumCollider_;

	public bool isHitBoss;

	public bool isDiffendBoss;

	public bool isBossJointBubble;

	public bool isHitEgg;

	public bool isHitNest;

	private Vector3 rayStartPoint;

	private Vector3 offsetVec = new Vector3(0f, 0f, -10f);

	private static System.Random rand = new System.Random();

	public static float hitSize
	{
		get
		{
			return mHitSize;
		}
		set
		{
			mHitSize = value;
			SQR_SIZE = mHitSize * mHitSize;
			SQR_LARGE_SIZE = mHitSize * 1.3f * (mHitSize * 1.3f);
		}
	}

	public Bubble.eState state { get; private set; }

	public bool isLocked
	{
		get
		{
			return mLocked;
		}
		set
		{
			if (mLocked == value)
			{
				return;
			}
			mLocked = value;
			setLockColor();
			if (type == Bubble.eType.Honeycomb)
			{
				Transform transform = myTrans.Find("bee_eff(Clone)");
				if (transform != null)
				{
					transform.gameObject.SetActive(!mLocked);
				}
			}
		}
	}

	public bool inCloud
	{
		get
		{
			return mCloud;
		}
		set
		{
			if (mCloud == value)
			{
				return;
			}
			mCloud = value;
			setLockColor();
			if (type == Bubble.eType.Honeycomb)
			{
				Transform transform = myTrans.Find("bee_eff(Clone)");
				if (transform != null)
				{
					transform.gameObject.SetActive(!mCloud);
				}
			}
			if (type == Bubble.eType.Counter && counterCount != null)
			{
				counterCount.setCounterSpriteFade(isLocked || inCloud);
			}
		}
	}

	public bool isPowerUp
	{
		get
		{
			return mPowerUp;
		}
		set
		{
			if (mPowerUp != value)
			{
				mPowerUp = value;
				powerUpEffect.gameObject.SetActive(mPowerUp);
				if (mPowerUp)
				{
					powerUpEffect.gameObject.SetActive(true);
					powerUpEffectLoop = powerUpEffect.Find("loop").GetComponent<UISpriteAnimation>();
					powerUpEffectLoop.gameObject.SetActive(true);
					powerUpEffectBurst = powerUpEffect.Find("burst").GetComponent<UISpriteAnimation>();
					powerUpEffectBurst.gameObject.SetActive(false);
				}
				else if (powerUpEffect != null)
				{
					UnityEngine.Object.DestroyImmediate(powerUpEffect.gameObject);
				}
			}
		}
	}

	public bool isFrozen { get; private set; }

	public bool isSplash { get; private set; }

	private void setLockColor()
	{
		Mesh sharedMesh = sprite.GetComponent<MeshFilter>().sharedMesh;
		Color[] colors = sharedMesh.colors;
		if (isLocked || inCloud)
		{
			if (type != Bubble.eType.Grow)
			{
				for (int i = 0; i < colors.Length; i++)
				{
					colors[i] = Color.gray;
				}
			}
			if (sprite != null && type != Bubble.eType.Grow)
			{
				sprite.Stop();
				sprite.SetFrame(0);
			}
			if (gloss != null)
			{
				StopCoroutine("glossRoutine");
				gloss.gameObject.SetActive(false);
			}
		}
		else
		{
			for (int j = 0; j < colors.Length; j++)
			{
				colors[j] = Color.white;
			}
		}
		sharedMesh.colors = colors;
	}

	public void init()
	{
		isFrozen = false;
		myTrans = base.transform;
		setType((Bubble.eType)int.Parse(base.name));
		sprite.animationEventDelegate = eventDelegate;
		if (base.gameObject.GetComponentInChildren<tk2dAnimatedSprite>() != null)
		{
			childAnimSp = GetComponentInChildren<tk2dAnimatedSprite>();
			nativeSpriteID = childAnimSp.spriteId;
		}
	}

	private void Update()
	{
		if (gloss != null && glossTime < Time.time)
		{
			startGloss(type);
			glossTime = Time.time + 1f + (float)rand.Next(12000) * 0.01f;
		}
	}

	public void setFieldState()
	{
		state = Bubble.eState.Field;
	}

	public void setCeiling(bool bAttachCollider)
	{
		UnityEngine.Object.Destroy(sprite.gameObject);
		sprite = null;
		gloss = null;
		if (bAttachCollider)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "ChainColliderHorizon")) as GameObject;
			gameObject.transform.parent = myTrans;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
		}
	}

	private void setGloss()
	{
		if (gloss != null)
		{
			StopCoroutine("glossRoutine");
			gloss.gameObject.SetActive(false);
		}
		gloss = null;
		glossClipName = null;
		if (type <= Bubble.eType.Black || (type >= Bubble.eType.PlusRed && type <= Bubble.eType.MinusBlack) || (type >= Bubble.eType.SnakeRed && type <= Bubble.eType.SnakeBlack))
		{
			glossClipName = "bubble_gloss";
		}
		if (glossClipName != null)
		{
			gloss = sprite.transform.Find("gloss").GetComponent<tk2dAnimatedSprite>();
			gloss.Play(glossClipName);
			glossTime = Time.time + 1f + (float)rand.Next(12000) * 0.01f;
		}
	}

	public void setFrozen(bool bFrozen)
	{
		isFrozen = bFrozen;
		if (!isFrozen)
		{
			sprite.Play("bubble_" + base.name);
			return;
		}
		switch (type)
		{
		case Bubble.eType.Lightning:
			sprite.Play("bubble_59");
			break;
		case Bubble.eType.Time:
			sprite.Play("bubble_60");
			break;
		case Bubble.eType.Search:
			sprite.Play("bubble_61");
			break;
		case Bubble.eType.Coin:
			sprite.Play("bubble_62");
			break;
		case Bubble.eType.Star:
			sprite.Play("bubble_108");
			break;
		case Bubble.eType.PlusRed:
		case Bubble.eType.PlusGreen:
		case Bubble.eType.PlusBlue:
		case Bubble.eType.PlusYellow:
		case Bubble.eType.PlusOrange:
		case Bubble.eType.PlusPurple:
		case Bubble.eType.PlusWhite:
		case Bubble.eType.PlusBlack:
			sprite.Play("bubble_63");
			break;
		case Bubble.eType.FriendRed:
		case Bubble.eType.FriendGreen:
		case Bubble.eType.FriendBlue:
		case Bubble.eType.FriendYellow:
		case Bubble.eType.FriendOrange:
		case Bubble.eType.FriendPurple:
		case Bubble.eType.FriendWhite:
		case Bubble.eType.FriendBlack:
		case Bubble.eType.FriendRainbow:
		case Bubble.eType.FriendBox:
			sprite.Play("bubble_64");
			break;
		case Bubble.eType.Honeycomb:
		{
			sprite.Play("bubble_58");
			Transform transform = myTrans.Find("bee_eff(Clone)");
			if (transform != null)
			{
				UnityEngine.Object.DestroyImmediate(transform.gameObject);
			}
			break;
		}
		case Bubble.eType.Counter:
			if (counterCount != null)
			{
				counterCount.setSpriteEnable(false, false);
			}
			sprite.Play("bubble_58");
			break;
		default:
			sprite.Play("bubble_58");
			break;
		}
	}

	public void setSplashBreak(bool bSplash)
	{
		isSplash = bSplash;
	}

	public bool setLineFriend(int friendIndex)
	{
		return setLineFriend(friendIndex, true);
	}

	public bool setLineFriend(int friendIndex, bool bScaling)
	{
		if (!isLineFriend && friendIndex < 0)
		{
			return false;
		}
		if (isLineFriend && friendIndex >= 0)
		{
			lineFriendIndex = friendIndex;
			lineFriendIcon.name = friendIndex.ToString();
			return true;
		}
		if (isLineFriend && friendIndex < 0)
		{
			isLineFriend = false;
			lineFriendIndex = -1;
			UnityEngine.Object.Destroy(lineFriendIcon);
			return false;
		}
		if (gloss == null)
		{
			return false;
		}
		isLineFriend = true;
		lineFriendIndex = friendIndex;
		lineFriendIcon = UnityEngine.Object.Instantiate(gloss.gameObject) as GameObject;
		lineFriendIcon.name = friendIndex.ToString();
		Utility.setParent(lineFriendIcon, sprite.transform, true);
		lineFriendIcon.transform.localPosition = Vector3.back * 0.005f;
		lineFriendIcon.SetActive(true);
		lineFriendIcon.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_55");
		if (bScaling)
		{
			lineFriendIcon.transform.localScale = new Vector3(1E-06f, 1E-06f, 1f);
			iTween.ScaleTo(lineFriendIcon, iTween.Hash("x", 1, "y", 1, "easetype", iTween.EaseType.easeOutElastic, "time", 1));
		}
		else
		{
			lineFriendIcon.transform.localScale = Vector3.one;
		}
		return true;
	}

	public void setType(Bubble.eType newType)
	{
		type = newType;
		int num = (int)type;
		base.name = num.ToString("00");
		sprite.Play("bubble_" + base.name);
		setGloss();
		setFrozen(isFrozen);
		if (fulcrumCollider_ != null)
		{
			UnityEngine.Object.Destroy(fulcrumCollider_);
			fulcrumCollider_ = null;
		}
		if (type == Bubble.eType.Fulcrum)
		{
			if (fulcrumColliderBase_ == null)
			{
				fulcrumColliderBase_ = ResourceLoader.Instance.loadGameObject("Prefabs/", "FulcrumCollider");
			}
			fulcrumCollider_ = UnityEngine.Object.Instantiate(fulcrumColliderBase_) as GameObject;
			fulcrumCollider_.transform.parent = myTrans;
			fulcrumCollider_.transform.localPosition = Vector3.zero;
			fulcrumCollider_.transform.localScale = Vector3.one;
		}
		else if (type == Bubble.eType.RotateFulcrumL || type == Bubble.eType.RotateFulcrumR)
		{
			if (fulcrumColliderBase_ == null)
			{
				fulcrumColliderBase_ = ResourceLoader.Instance.loadGameObject("Prefabs/", "FulcrumCollider");
			}
			fulcrumCollider_ = UnityEngine.Object.Instantiate(fulcrumColliderBase_) as GameObject;
			fulcrumCollider_.transform.parent = myTrans;
			fulcrumCollider_.transform.localPosition = Vector3.zero;
			fulcrumCollider_.transform.localScale = Vector3.one;
		}
	}

	public void setBreak()
	{
		state = Bubble.eState.Break;
	}

	public void shot(Vector3 fireVector)
	{
		rayStartPoint = myTrans.position;
		state = Bubble.eState.Shot;
		if (type != Bubble.eType.Metal)
		{
			StartCoroutine(shotMoveRoutine(fireVector));
			return;
		}
		isHitCloud = guide.inCloud;
		bossPart_.hit(this, null);
	}

	private IEnumerator shotMoveRoutine(Vector3 fireVector)
	{
		Bubble_Boss[] fieldBubbles = null;
		fieldBubbles = bossPart_.fieldBubbleList.ToArray();
		isHitBoss = false;
		isDiffendBoss = false;
		isBossJointBubble = false;
		base.transform.localPosition += offsetVec;
		fireVector.z = 0f;
		fireVector.Normalize();
		Vector3 moveVector = fireVector * SPEED;
		boundCount = 0;
		float x_min = 0.1f;
		float x_max = 539.9f;
		yield return stagePause_.sync();
		yield return stagePause_.sync();
		bool boundCeiling = false;
		float moved = 0f;
		while (state == Bubble.eState.Shot)
		{
			Vector3 vec = moveVector * Time.deltaTime;
			Vector3 pos2 = myTrans.localPosition + vec;
			if (pos2.x < x_min || pos2.x > x_max)
			{
				float rate;
				if (vec.x < 0f)
				{
					rate = (pos2.x - x_min) / vec.x;
					StartCoroutine(boundEffRoutine(bossPart_.boundEffL, true));
				}
				else
				{
					rate = (pos2.x - x_max) / vec.x;
					StartCoroutine(boundEffRoutine(bossPart_.boundEffR, true));
				}
				pos2 = myTrans.localPosition + vec * (1f - rate);
				vec.x = 0f - vec.x;
				pos2 += vec * rate;
				rayStartPoint = myTrans.position;
				boundCount++;
				moveVector.x = 0f - moveVector.x;
				switch (boundCount)
				{
				case 1:
					Sound.Instance.playSe(Sound.eSe.SE_211_bound1);
					break;
				case 2:
					Sound.Instance.playSe(Sound.eSe.SE_212_bound2);
					break;
				case 3:
					Sound.Instance.playSe(Sound.eSe.SE_213_bound3);
					break;
				case 4:
					Sound.Instance.playSe(Sound.eSe.SE_214_bound4);
					break;
				default:
					Sound.Instance.playSe(Sound.eSe.SE_215_bound5);
					break;
				}
			}
			if (guide.isHitCeiling(myTrans.position) && !boundCeiling)
			{
				boundCeiling = true;
				StartCoroutine(boundEffRoutine(bossPart_.boundEffL, true));
				vec.y = 0f - vec.y;
				boundCount++;
				moveVector.y = 0f - moveVector.y;
				rayStartPoint = myTrans.position;
				switch (boundCount)
				{
				case 1:
					Sound.Instance.playSe(Sound.eSe.SE_211_bound1);
					break;
				case 2:
					Sound.Instance.playSe(Sound.eSe.SE_212_bound2);
					break;
				case 3:
					Sound.Instance.playSe(Sound.eSe.SE_213_bound3);
					break;
				case 4:
					Sound.Instance.playSe(Sound.eSe.SE_214_bound4);
					break;
				default:
					Sound.Instance.playSe(Sound.eSe.SE_215_bound5);
					break;
				}
			}
			myTrans.localPosition = pos2;
			if (myTrans.position.y > 0.35f)
			{
				switch (bossBase.CheckHitBoss(rayStartPoint, myTrans.position, (int)type))
				{
				case BossBase.eBossHitState.Hit:
					yield return stagePause_.sync();
					bossBase.damageValue = ((!isPowerUp) ? 1 : 2);
					bossBase.state = BossBase.eState.damaged;
					isHitBoss = true;
					bossBase.isWaitBossAction = true;
					bossBase.isMoving = false;
					break;
				case BossBase.eBossHitState.Diffend:
					yield return stagePause_.sync();
					Sound.Instance.playSe(Sound.eSe.SE_246_bossnodmg);
					isDiffendBoss = true;
					break;
				case BossBase.eBossHitState.Bubble:
					yield return stagePause_.sync();
					Debug.Log((int)type);
					isBossJointBubble = true;
					break;
				}
			}
			moved += vec.magnitude;
			if (moved < guide.hitMoveLength && !isHitBoss && !isDiffendBoss && !isBossJointBubble && !isHitEgg && !isHitNest)
			{
				yield return stagePause_.sync();
				continue;
			}
			if (isHitBoss)
			{
				myTrans.Find("bubble_trail_eff").parent = myTrans.parent.parent.parent;
				guide.hitBubble = null;
				if (type == Bubble.eType.Water)
				{
					Sound.Instance.playSe(Sound.eSe.SE_515_water_bubble_hit);
				}
				else if (type == Bubble.eType.Shine)
				{
					Sound.Instance.playSe(Sound.eSe.SE_519_shine_bubble_hit);
				}
				bossPart_.hit(this, this);
				break;
			}
			if (isBossJointBubble)
			{
				myTrans.Find("bubble_trail_eff").parent = myTrans.parent.parent.parent;
				guide.hitBubble = null;
				bossPart_.hit(this, this);
				break;
			}
			if (isDiffendBoss)
			{
				myTrans.Find("bubble_trail_eff").parent = myTrans.parent.parent.parent;
				guide.hitBubble = null;
				bossPart_.hit(this, this);
				break;
			}
			if (isHitEgg)
			{
				myTrans.Find("bubble_trail_eff").parent = myTrans.parent.parent.parent;
				guide.hitBubble = null;
				bossPart_.hit(this, this);
				break;
			}
			pos2 = myTrans.position;
			pos2.x = guide.hitPos.x;
			pos2.y = guide.hitPos.y;
			myTrans.position = pos2;
			BubbleBase hitBubble = guide.hitBubble;
			Vector3 diff2 = guide.hitDiff;
			myTrans.Find("bubble_trail_eff").parent = myTrans.parent.parent.parent;
			if (guide.isNest || guide.isEgg)
			{
				if (type == Bubble.eType.Water)
				{
					Sound.Instance.playSe(Sound.eSe.SE_515_water_bubble_hit);
				}
				else if (type == Bubble.eType.Shine)
				{
					Sound.Instance.playSe(Sound.eSe.SE_519_shine_bubble_hit);
				}
				bossPart_.hit(this, this);
				break;
			}
			if (guide.inCloud)
			{
				if (type == Bubble.eType.Water)
				{
					Sound.Instance.playSe(Sound.eSe.SE_515_water_bubble_hit);
				}
				else if (type == Bubble.eType.Shine)
				{
					Sound.Instance.playSe(Sound.eSe.SE_519_shine_bubble_hit);
				}
				isHitCloud = true;
				bossPart_.hit(this, null);
				break;
			}
			if (guide.isSpw)
			{
				bossPart_.hit(this, this);
				Vector3 c_pos = guide.hitSpw.transform.localPosition + getCorrectOffset(diff2);
				c_pos.z = 0f;
				iTween.MoveTo(base.gameObject, iTween.Hash("position", c_pos, "time", 0.05f, "islocal", true, "easetype", iTween.EaseType.linear));
				shock(-diff2 * 0.8f);
				break;
			}
			if (hitBubble == null)
			{
				if (type == Bubble.eType.Water)
				{
					Sound.Instance.playSe(Sound.eSe.SE_515_water_bubble_hit);
				}
				else if (type == Bubble.eType.Shine)
				{
					Sound.Instance.playSe(Sound.eSe.SE_519_shine_bubble_hit);
				}
				bossPart_.hit(this, this);
				break;
			}
			Vector3 correctpos = hitBubble.myTrans.localPosition + getCorrectOffset(diff2);
			correctpos.z = 0f;
			iTween.MoveTo(base.gameObject, iTween.Hash("position", correctpos, "time", 0.05f, "islocal", true, "easetype", iTween.EaseType.linear));
			shock(-diff2 * 0.8f);
			Bubble_Boss[] array = fieldBubbles;
			foreach (Bubble_Boss fieldBubble in array)
			{
				if (!fieldBubble.isLocked && !fieldBubble.inCloud && fieldBubble.myTrans.childCount != 0 && fieldBubble.type != Bubble.eType.Blank)
				{
					diff2 = fieldBubble.myTrans.localPosition - correctpos;
					if (!(diff2.sqrMagnitude > 44100f))
					{
						fieldBubble.shock(diff2);
					}
				}
			}
			if (type == Bubble.eType.Water)
			{
				Sound.Instance.playSe(Sound.eSe.SE_515_water_bubble_hit);
			}
			else if (type == Bubble.eType.Shine)
			{
				Sound.Instance.playSe(Sound.eSe.SE_519_shine_bubble_hit);
			}
			else
			{
				Sound.Instance.playSe(Sound.eSe.SE_216_sessyoku);
			}
			while (base.gameObject.GetComponentInChildren<iTween>() != null)
			{
				yield return stagePause_.sync();
			}
			myTrans.localPosition = correctpos;
			bossPart_.hit(this, hitBubble);
			break;
		}
	}

	private IEnumerator boundEffRoutine(GameObject boundEff, bool isLeft)
	{
		Transform effTrans = boundEff.transform;
		effTrans.position = myTrans.position;
		if (isLeft)
		{
			effTrans.localPosition += Vector3.left * 30f;
		}
		else
		{
			effTrans.localPosition += Vector3.right * 30f;
		}
		boundEff.SetActive(true);
		UITweener[] tweeners = boundEff.GetComponentsInChildren<UITweener>();
		UITweener[] array = tweeners;
		foreach (UITweener tweener in array)
		{
			tweener.style = UITweener.Style.Once;
			tweener.Reset();
			tweener.Play(true);
		}
		while (tweeners[0].enabled)
		{
			yield return stagePause_.sync();
		}
		boundEff.SetActive(false);
	}

	public Vector3 getCorrectOffset(Vector3 diff)
	{
		float num = Mathf.Atan2(diff.y, diff.x) * 57.29578f - 90f;
		if (num <= -180f)
		{
			num += 360f;
		}
		if (num <= 0f && num > -60f)
		{
			return offsetUpRight;
		}
		if (num <= -60f && num > -120f)
		{
			return offsetRight;
		}
		if (num <= -120f && num > -180f)
		{
			return offsetDownRight;
		}
		if (num <= 180f && num > 120f)
		{
			return offsetDownLeft;
		}
		if (num <= 120f && num > 60f)
		{
			return offsetLeft;
		}
		return offsetUpLeft;
	}

	public void startBreak()
	{
		startBreak(true, true);
	}

	public void startBreak(bool bDestroy)
	{
		startBreak(bDestroy, true);
	}

	public void startBreak(bool bDestroy, bool bParticle)
	{
		if (state == Bubble.eState.Break)
		{
			return;
		}
		if (type == Bubble.eType.Search)
		{
			bossPart_.isSearching = true;
		}
		if (!sprite.enabled)
		{
			sprite.enabled = true;
		}
		float x = myTrans.localPosition.x;
		float y = myTrans.localPosition.y;
		float num = ((float)(int)x - 540f) / 30f * 0.005f;
		num += (float)((int)y / 52) * 0.05f;
		myTrans.localPosition += Vector3.forward * num;
		rescue();
		if (bParticle && type != Bubble.eType.Grow)
		{
			scoreParticle();
		}
		state = Bubble.eState.Break;
		if (myTrans.localPosition.y > 155f && type != Bubble.eType.Skull)
		{
			if (bDestroy)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				base.gameObject.SetActive(false);
			}
			return;
		}
		if (gloss != null)
		{
			StopCoroutine("glossRoutine");
			gloss.gameObject.SetActive(false);
			gloss = null;
		}
		StartCoroutine(breakRoutine(bDestroy));
	}

	private IEnumerator breakRoutine(bool bDestroy)
	{
		UnityEngine.Object.Destroy(sprite.GetComponent<iTween>());
		myTrans.localPosition += Vector3.back * 0.01f;
		Bubble.eType convertedType = Bubble.eType.Invalid;
		convertedType = bossPart_.convertColorBubble(type);
		if (convertedType == Bubble.eType.FriendRainbow)
		{
			convertedType = Bubble.eType.Rainbow;
		}
		if (counterCount != null)
		{
			counterCount.setSpriteEnable(false, false);
		}
		if (isFrozen)
		{
			sprite.Play("burst_57");
			while (sprite.IsPlaying(sprite.CurrentClip))
			{
				yield return stagePause_.sync();
			}
		}
		else if (isSplash && type != Bubble.eType.Counter)
		{
			sprite.Play("burst_67");
			while (sprite.IsPlaying(sprite.CurrentClip))
			{
				yield return stagePause_.sync();
			}
		}
		else if (type == Bubble.eType.Skull)
		{
			GameObject skull = UnityEngine.Object.Instantiate(bossPart_.skullBase) as GameObject;
			Utility.setParent(skull, myTrans.parent.parent, true);
			skull.transform.position = myTrans.position;
			Vector3 pos = skull.transform.localPosition;
			pos.z = -17f;
			skull.transform.localPosition = pos;
			skull.SetActive(true);
			sprite.gameObject.SetActive(false);
			while (skull.GetComponent<Animation>().isPlaying)
			{
				yield return stagePause_.sync();
			}
			UnityEngine.Object.Destroy(skull);
		}
		else if (type >= Bubble.eType.ChameleonRed && type <= Bubble.eType.ChameleonBlack)
		{
			sprite.Play("burst_" + ((int)(type - 79)).ToString("00"));
			while (sprite.IsPlaying(sprite.CurrentClip))
			{
				yield return stagePause_.sync();
			}
		}
		else
		{
			tk2dAnimatedSprite obj = sprite;
			int num = (int)convertedType;
			obj.Play("burst_" + num.ToString("00"));
			while (sprite.IsPlaying(sprite.CurrentClip))
			{
				yield return stagePause_.sync();
			}
		}
		if (isPowerUp)
		{
			powerUpEffectLoop.gameObject.SetActive(false);
			powerUpEffectBurst.gameObject.SetActive(true);
			powerUpEffectBurst.Reset();
			while (powerUpEffectBurst.isPlaying)
			{
				yield return stagePause_.sync();
			}
		}
		if (bDestroy)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	public void startDrop(int delay)
	{
		state = Bubble.eState.Drop;
		StartCoroutine(dropRoutine(delay));
	}

	private IEnumerator dropRoutine(int delay)
	{
		Rigidbody rb = base.gameObject.AddComponent<Rigidbody>();
		rb.isKinematic = false;
		rb.constraints = RigidbodyConstraints.FreezeAll;
		float xForce = -5f + (float)rand.Next(11);
		float yForce = 80f + (float)rand.Next(10);
		float waitTime = 0f;
		while (waitTime < 0.01f * (float)delay)
		{
			waitTime += Time.deltaTime;
			yield return stagePause_.sync();
		}
		yield return stagePause_.sync();
		myTrans.localPosition += Vector3.forward * 3f;
		if (type == Bubble.eType.Honeycomb)
		{
			Transform beeTrans = myTrans.Find("bee_eff(Clone)");
			if (beeTrans != null)
			{
				beeTrans.localPosition += Vector3.back * 3f;
			}
		}
		if (counterCount != null)
		{
			counterCount.setSpriteEnable(false, false);
		}
		rb.isKinematic = false;
		rb.constraints = (RigidbodyConstraints)120;
		rb.AddForce(new Vector3(xForce, 0f, 0f));
		while (myTrans.localPosition.y > -420f)
		{
			yield return stagePause_.sync();
		}
		Sound.Instance.playSe(Sound.eSe.SE_216_sessyoku);
		Vector3 p = myTrans.localPosition;
		p.y = -420f;
		myTrans.localPosition = p;
		rb.isKinematic = false;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		rb.AddForce(new Vector3(xForce, yForce, 0f));
		yield return stagePause_.sync();
		yield return stagePause_.sync();
		while (rb.velocity.y > 0f)
		{
			yield return stagePause_.sync();
		}
		UnityEngine.Object.Destroy(rb);
		if (isColorBubble() || type == Bubble.eType.FriendBox || type == Bubble.eType.FriendRainbow || type == Bubble.eType.FriendFulcrum)
		{
			rescue();
			StartCoroutine(breakRoutine(true));
		}
		else
		{
			StartCoroutine(fadeoutRoutine());
		}
	}

	private void rescue()
	{
		if ((type >= Bubble.eType.FriendRed && type <= Bubble.eType.FriendBox) || type == Bubble.eType.FriendFulcrum)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(bossPart_.chacknBase) as GameObject;
			Utility.setParent(gameObject, bossPart_.getScrollUI(), true);
			gameObject.transform.position = myTrans.position;
			gameObject.GetComponent<Animation>().enabled = false;
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.z = -13f;
			float num = 480f + NGUIUtilScalableUIRoot.GetOffsetY(true).y;
			if (localPosition.y > num)
			{
				gameObject.GetComponent<Animation>().clip = gameObject.GetComponent<Animation>()["Rescue_01_anm"].clip;
				localPosition.y = num;
			}
			gameObject.transform.localPosition = localPosition;
			gameObject.SetActive(true);
			Chackn_Boss chackn_Boss = gameObject.AddComponent<Chackn_Boss>();
			chackn_Boss.bossBase_ = bossBase;
			chackn_Boss.setPartStage(bossPart_);
			chackn_Boss.animStart(stagePause_);
			Sound.Instance.playSe((Sound.eSe)(40 + rand.Next(3)));
		}
	}

	private void scoreParticle()
	{
		if (bossPart_.state != Part_BossStage.eState.Clear && type >= Bubble.eType.FriendRainbow && !isFrozen && bossPart_ != null && bossPart_.isSpecialBubble(type))
		{
		}
	}

	public void startFadeout()
	{
		StartCoroutine(fadeoutRoutine());
	}

	private IEnumerator fadeoutRoutine()
	{
		sprite.Stop();
		MeshFilter filter = sprite.GetComponent<MeshFilter>();
		if (type != Bubble.eType.Honeycomb || !bossPart_.isUsedItem(Constant.Item.eType.BeeBarrier) || type != Bubble.eType.Counter || !(counterCount != null) || counterCount.getCount() > 0)
		{
			filter.GetComponent<Renderer>().sharedMaterial = bossPart_.fadeMaterial;
		}
		Mesh mesh = filter.sharedMesh;
		Color[] colors = mesh.colors;
		while (colors[0].a > 0f)
		{
			for (int i = 0; i < colors.Length; i++)
			{
				colors[i].a -= Time.deltaTime * 5f;
				if (colors[i].a < 0f)
				{
					colors[i].a = 0f;
				}
			}
			mesh.colors = colors;
			yield return stagePause_.sync();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private IEnumerator fadeoutSnakeRoutine(UISprite sprite)
	{
		Color color = sprite.color;
		while (color.a > 0f)
		{
			color.a -= Time.deltaTime * 5f;
			if (color.a < 0f)
			{
				color.a = 0f;
			}
			sprite.color = color;
			yield return stagePause_.sync();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void setGrayScale(Material mat)
	{
		Transform transform = myTrans.Find("bee_eff(Clone)");
		if (transform != null)
		{
			UnityEngine.Object.DestroyImmediate(transform.gameObject);
		}
		sprite.GetComponent<Renderer>().material = mat;
	}

	public void startGameover(int delay, Material mat)
	{
		state = Bubble.eState.Gameover;
		StartCoroutine("gameoverRoutine", delay);
		Transform transform = myTrans.Find("bee_eff(Clone)");
		if (transform != null)
		{
			UnityEngine.Object.DestroyImmediate(transform.gameObject);
		}
		sprite.enabled = false;
		sprite.GetComponent<Renderer>().material = mat;
	}

	private IEnumerator gameoverRoutine(int delay)
	{
		float waitTime = 0f;
		while (waitTime < 0.01f * (float)delay)
		{
			if (state == Bubble.eState.Field)
			{
				yield break;
			}
			waitTime += Time.deltaTime;
			yield return stagePause_.sync();
		}
		if (state != Bubble.eState.Field)
		{
		}
	}

	public void resurrection(Material mat)
	{
		StopCoroutine("gameoverRoutine");
		state = Bubble.eState.Field;
		setType(type);
		sprite.enabled = true;
		sprite.GetComponent<Renderer>().material = mat;
		if (type == Bubble.eType.Grow)
		{
			sprite.PlayFromFrame(1);
		}
		Color color = sprite.color;
		color.a = 1f;
		sprite.color = color;
	}

	public void shock(Vector3 force)
	{
		StartCoroutine(shockRoutine(force));
	}

	private IEnumerator shockRoutine(Vector3 force)
	{
		float power = 0.2f;
		Vector3 max = force;
		max.Normalize();
		max *= 210f;
		Vector3 pos = (max - force) * power;
		GameObject child = sprite.gameObject;
		iTween.MoveTo(child, iTween.Hash("position", pos, "time", 0.05f, "islocal", true, "easetype", iTween.EaseType.linear));
		while (child.GetComponent<iTween>() != null)
		{
			yield return stagePause_.sync();
		}
		iTween.MoveTo(child, iTween.Hash("position", Vector3.zero, "time", 0.1f, "islocal", true, "easetype", iTween.EaseType.linear));
		while (child.GetComponent<iTween>() != null)
		{
			yield return stagePause_.sync();
		}
	}

	public void startGloss(Bubble.eType glossType)
	{
		if ((!(bossPart_ != null) || !bossPart_.stagePause.pause) && (state != Bubble.eState.Field || !(myTrans.localPosition.y > 105f)) && glossType == type && !(gloss == null))
		{
			StopCoroutine("glossRoutine");
			StartCoroutine("glossRoutine");
		}
	}

	private IEnumerator glossRoutine()
	{
		if (!mLocked && !mCloud)
		{
			gloss.gameObject.SetActive(true);
			gloss.Play();
			while (gloss != null && gloss.IsPlaying(glossClipName))
			{
				yield return bossPart_.stagePause.sync();
			}
			if (gloss != null)
			{
				gloss.gameObject.SetActive(false);
			}
		}
	}

	public bool isShakeRangeBubble(Bubble_Boss target)
	{
		float sqrMagnitude = (myTrans.localPosition - target.myTrans.localPosition).sqrMagnitude;
		if (sqrMagnitude > 22500f)
		{
			return false;
		}
		return true;
	}

	public bool isColorBubble()
	{
		if (type <= Bubble.eType.Black)
		{
			return true;
		}
		if (type >= Bubble.eType.PlusRed && type <= Bubble.eType.PlusBlack)
		{
			return true;
		}
		if (type >= Bubble.eType.MinusRed && type <= Bubble.eType.MinusBlack)
		{
			return true;
		}
		if (type >= Bubble.eType.FriendRed && type <= Bubble.eType.FriendBlack)
		{
			return true;
		}
		if (type >= Bubble.eType.SnakeRed && type <= Bubble.eType.SnakeBlack)
		{
			return true;
		}
		return false;
	}

	public void Bubble_Boss_Init()
	{
	}

	public bool isSamColor(Bubble_Boss b)
	{
		if (!isColorBubble() || !b.isColorBubble())
		{
			return false;
		}
		return bossPart_.convertColorBubble(type) == bossPart_.convertColorBubble(b.type);
	}

	private void eventDelegate(tk2dAnimatedSprite sp, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame, int frameNum)
	{
		float num = 418f * ((float)frame.eventInt / 100f);
		sp.transform.localScale = new Vector3(num, num, 1f);
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, frame.eventFloat);
	}
}
