using System;
using System.Collections;
using System.Collections.Generic;
using Bridge;
using Network;
using UnityEngine;

public class Bubble : BubbleBase
{
	public enum eState
	{
		Wait = 0,
		Shot = 1,
		Field = 2,
		Break = 3,
		Drop = 4,
		Gameover = 5
	}

	public enum eDir
	{
		UpRight = 0,
		Right = 1,
		DownRight = 2,
		DownLeft = 3,
		Left = 4,
		UpLeft = 5,
		Max = 6
	}

	public enum eType
	{
		Red = 0,
		Green = 1,
		Blue = 2,
		Yellow = 3,
		Orange = 4,
		Purple = 5,
		White = 6,
		Black = 7,
		Lightning = 8,
		Search = 9,
		Star = 10,
		Time = 11,
		Coin = 12,
		PlusRed = 13,
		PlusGreen = 14,
		PlusBlue = 15,
		PlusYellow = 16,
		PlusOrange = 17,
		PlusPurple = 18,
		PlusWhite = 19,
		PlusBlack = 20,
		MinusRed = 21,
		MinusGreen = 22,
		MinusBlue = 23,
		MinusYellow = 24,
		MinusOrange = 25,
		MinusPurple = 26,
		MinusWhite = 27,
		MinusBlack = 28,
		Grow = 29,
		Skull = 30,
		FriendRed = 31,
		FriendGreen = 32,
		FriendBlue = 33,
		FriendYellow = 34,
		FriendOrange = 35,
		FriendPurple = 36,
		FriendWhite = 37,
		FriendBlack = 38,
		FriendRainbow = 39,
		FriendBox = 40,
		Hyper = 41,
		Bomb = 42,
		Shake = 43,
		Box = 44,
		Rainbow = 45,
		Rock = 46,
		ChainLock = 47,
		ChainHorizontal = 48,
		ChainRightDown = 49,
		ChainLeftDown = 50,
		Fulcrum = 51,
		RotateFulcrumR = 52,
		RotateFulcrumL = 53,
		Metal = 56,
		Ice = 57,
		Honeycomb = 65,
		Fire = 66,
		SnakeRed = 67,
		SnakeGreen = 68,
		SnakeBlue = 69,
		SnakeYellow = 70,
		SnakeOrange = 71,
		SnakePurple = 72,
		SnakeWhite = 73,
		SnakeBlack = 74,
		Water = 75,
		Shine = 76,
		Counter = 77,
		BlackHole_A = 78,
		ChameleonRed = 79,
		ChameleonGreen = 80,
		ChameleonBlue = 81,
		ChameleonYellow = 82,
		ChameleonOrange = 83,
		ChameleonPurple = 84,
		ChameleonWhite = 85,
		ChameleonBlack = 86,
		Unknown = 87,
		BlackHole_B = 88,
		FriendFulcrum = 89,
		IceKey = 90,
		KeyRed = 91,
		KeyGreen = 92,
		KeyBlue = 93,
		KeyYellow = 94,
		KeyOrange = 95,
		KeyPurple = 96,
		KeyWhite = 97,
		KeyBlack = 98,
		FieldMax = 41,
		ObjectBegin = 39,
		ObjectEnd = 53,
		FriendBegin = 31,
		FriendEnd = 40,
		Blank = 99,
		CounterRed = 100,
		CounterGreen = 101,
		CounterBlue = 102,
		CounterYellow = 103,
		CounterOrange = 104,
		CounterPurple = 105,
		CounterWhite = 106,
		CounterBlack = 107,
		MorganaRed = 109,
		MorganaGreen = 110,
		MorganaBlue = 111,
		MorganaYellow = 112,
		MorganaOrange = 113,
		MorganaPurple = 114,
		MorganaWhite = 115,
		MorganaBlack = 116,
		TunnelIn = 120,
		TunnelNotIn = 121,
		TunnelOutLeftUP = 122,
		TunnelOutUP = 123,
		TunnelOutRightUP = 124,
		TunnelOutLeftDown = 125,
		TunnelOutDown = 126,
		TunnelOutRightDown = 127,
		MinilenRed = 128,
		MinilenGreen = 129,
		MinilenBlue = 130,
		MinilenYellow = 131,
		MinilenOrange = 132,
		MinilenPurple = 133,
		MinilenWhite = 134,
		MinilenBlack = 135,
		MinilenDancing = 136,
		IcedMinilen = 137,
		LightningG = 138,
		MinilenRainbow = 139,
		LightningG_Item = 140,
		Invalid = -1
	}

	public class RotateState
	{
		public Bubble fulcrum;

		public int rad;

		public bool isLeft;

		public Vector2 diff;

		public int moveCnt;

		public RotateState()
		{
			init();
		}

		public void init()
		{
			fulcrum = null;
			rad = 0;
			diff = Vector2.zero;
			moveCnt = 0;
		}
	}

	public const float LIMIT_X_MIN = 0f;

	public const float LIMIT_X_MAX = 540f;

	public const int OFFSET_W = 60;

	public const int OFFSET_H = 52;

	public const int HALF_SIZE = 30;

	public const float SQR_SAME_RANGE = 900f;

	public const float SQR_NEAR_RANGE = 8100f;

	private const float SHOCK_RANGE = 210f;

	public const float SQR_SHOCK_RANGE = 44100f;

	private const float SHAKE_RANGE = 150f;

	private const float SQR_SHAKE_RANGE = 22500f;

	public const float SQR_BOMB_RANGE = 22500f;

	private const float PRECHECK_RANGE = 90f;

	public const float SQR_PRECHECK_RANGE = 8100f;

	public const int BREAK_THRESHOLD = 3;

	public const int LINE_DEFAULT = 10;

	public const int LINE_LIMIT = 12;

	public const int LINE_BONUS_LIMIT = 15;

	public const float DROP_JUMP_HEIGHT = -420f;

	public const int dropForceX_Min = -5;

	public const int dropForceX_Rand = 11;

	public const int dropForceY_Min = 80;

	public const int dropForceY_Rand = 10;

	public const float DROP_JUMP_SNAKE_HEIGHT = -360f;

	private const int GLOSS_INTERVAL = 120;

	private const float morganaAlpha_ = 0.8f;

	public const int ROTATE_RAD_MAX = 2;

	public Part_Stage part;

	public Part_BonusStage bonusPart;

	public Part_RankingStage rankingPart;

	public StagePause stagePause;

	public List<eType> basicSkillColorType = new List<eType>();

	public bool isBasicSkillColor;

	[SerializeField]
	private GameObject Score_up;

	public GameObject OldOutObject;

	public GameObject OutObject;

	public GameObject myObject;

	public int outObjectType;

	public bool UseOutObject = true;

	public bool isHit;

	public tk2dAnimatedSprite sprite;

	public tk2dAnimatedSprite gloss;

	public float bubblePower = 1f;

	public UILabel powerLabel_;

	public int hideCount;

	public Counter counterCount;

	public GameObject TunnelOut;

	public Vector3 TunnelOutPos;

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

	public bool bNoCountCombo;

	private float glossTime;

	private string glossClipName;

	public int unknownColor = -1;

	public int chamelleonIndex = -1;

	public ObstacleDefend onObstacle;

	public bool isFire;

	private bool mLocked;

	public bool mCloud;

	public bool isHitCloud;

	public Guide guide;

	public bool isLineFriend;

	public int lineFriendIndex = -1;

	private GameObject lineFriendIcon;

	public int createIndex = -1;

	public bool isOnChain;

	private Vector3 BEE_DEFAULT_POS = new Vector3(0f, 0f, -0.02f);

	private Vector3 COUNTER_DEFALUT_POS = new Vector3(0f, 0f, -3f);

	public int uniqueId = int.MaxValue;

	private static GameObject fulcrumColliderBase_ = null;

	private GameObject fulcrumCollider_;

	private static System.Random rand = new System.Random();

	public bool IsMorganaParent_;

	public Bubble ParentMorgana_;

	public List<Bubble> ChildMorgana_;

	public int MorganaHP_ = 3;

	public bool OnDamage_;

	public bool IsSpecialBreak_;

	public bool IsChangeColor_;

	public int CharaNum_ = -1;

	public GameObject CharaObj_;

	public bool AnimStopFlg;

	public string[] CharaNames_ = new string[1] { "chara_01_19" };

	public string[,] CharaAnimNo_ = new string[4, 2]
	{
		{ "_03", "_03" },
		{ "_02", "_02" },
		{ "_01", "_01" },
		{ "_00", "_00" }
	};

	public RotateState rotateState = new RotateState();

	public iTween.EaseType easeType = iTween.EaseType.easeInOutQuad;

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

	public eState state { get; private set; }

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
			if (type == eType.Honeycomb)
			{
				Transform transform = myTrans.Find("bee_eff(Clone)");
				if (transform != null)
				{
					transform.gameObject.SetActive(!mLocked);
				}
			}
			if (isCounter(type) && counterCount != null)
			{
				counterCount.setCounterSpriteFade(isLocked || inCloud || part.bUsingTimeStop, part.getTimeStopCount() <= 0);
			}
			if (type >= eType.MorganaRed && type <= eType.MorganaBlack && IsMorganaParent_)
			{
				if (mLocked)
				{
					CharaObj_.GetComponentInChildren<tk2dAnimatedSprite>().Play(CharaNames_[CharaNum_] + "_04");
				}
				else
				{
					CharaObj_.GetComponentInChildren<tk2dAnimatedSprite>().Play(CharaNames_[CharaNum_] + CharaAnimNo_[MorganaHP_, CharaNum_]);
				}
			}
			if (!(sprite != null) || type < eType.TunnelIn || type > eType.TunnelOutLeftUP)
			{
				return;
			}
			Debug.Log("  オブジェクト名  " + myObject.name);
			if (type == eType.TunnelIn && mLocked)
			{
				myObject.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 121);
				myObject.name = "121";
				if (fulcrumCollider_.name == "warpInCollider")
				{
					fulcrumCollider_.name = "FulcrumCollider(Clone)";
				}
				type = eType.TunnelNotIn;
			}
			else if (type == eType.TunnelNotIn && !mLocked)
			{
				myObject.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 120);
				myObject.name = "120";
				if (fulcrumCollider_.name == "FulcrumCollider(Clone)")
				{
					fulcrumCollider_.name = "warpInCollider";
				}
				type = eType.TunnelIn;
			}
			if (type == eType.TunnelOutLeftUP && mLocked && UseOutObject)
			{
				UseOutObject = false;
				myObject.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 123);
			}
			else if (type == eType.TunnelOutLeftUP && !mLocked && !UseOutObject)
			{
				UseOutObject = true;
				myObject.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 122);
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
			if (type >= eType.MorganaRed && type <= eType.MorganaBlack && IsMorganaParent_)
			{
				if (inCloud)
				{
					if (AnimStopFlg)
					{
						CharaObj_.GetComponentInChildren<tk2dAnimatedSprite>().Play(CharaNames_[CharaNum_] + "_04");
					}
					else
					{
						CharaObj_.GetComponentInChildren<tk2dAnimatedSprite>().Play(CharaNames_[CharaNum_] + CharaAnimNo_[MorganaHP_, CharaNum_]);
					}
				}
				else
				{
					CharaObj_.GetComponentInChildren<tk2dAnimatedSprite>().Play(CharaNames_[CharaNum_] + CharaAnimNo_[MorganaHP_, CharaNum_]);
				}
			}
			if (part.bInitialized)
			{
				setLockColor();
			}
			if (type == eType.Honeycomb)
			{
				Transform transform = myTrans.Find("bee_eff(Clone)");
				if (transform != null)
				{
					transform.gameObject.SetActive(!mCloud);
				}
			}
			if (isCounter(type) && counterCount != null)
			{
				counterCount.setCounterSpriteFade(isLocked || inCloud || part.bUsingTimeStop);
			}
			if (!(sprite != null) || type < eType.TunnelIn || type > eType.TunnelOutLeftUP)
			{
				return;
			}
			if (type == eType.TunnelIn && mCloud)
			{
				tk2dSpriteAnimationClip currentClip = myObject.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
				if (currentClip.name != "bubble_121")
				{
					myObject.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 121);
				}
				myObject.name = "121";
				if (fulcrumCollider_.name == "warpInCollider")
				{
					fulcrumCollider_.name = "FulcrumCollider(Clone)";
				}
				type = eType.TunnelNotIn;
			}
			else if (type == eType.TunnelNotIn && !mCloud)
			{
				tk2dSpriteAnimationClip currentClip2 = myObject.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
				if (currentClip2.name != "bubble_120")
				{
					myObject.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 120);
				}
				myObject.name = "120";
				if (fulcrumCollider_.name == "FulcrumCollider(Clone)")
				{
					fulcrumCollider_.name = "warpInCollider";
				}
				type = eType.TunnelIn;
			}
			if (type == eType.TunnelOutLeftUP && mCloud)
			{
				UseOutObject = false;
				tk2dSpriteAnimationClip currentClip3 = myObject.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
				if (currentClip3.name != "bubble_123")
				{
					myObject.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 123);
				}
			}
			else if (type == eType.TunnelOutLeftUP && !mCloud)
			{
				UseOutObject = true;
				tk2dSpriteAnimationClip currentClip4 = myObject.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
				if (currentClip4.name != "bubble_122")
				{
					myObject.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 122);
				}
			}
		}
	}

	public bool isFrozen { get; private set; }

	public bool isSplash { get; private set; }

	public GameObject OutObjectData
	{
		get
		{
			return base.gameObject;
		}
		set
		{
			OutObject = value;
		}
	}

	public void changeBubblePower(float power)
	{
		if (power >= 0.5f && power <= 2f)
		{
			bubblePower = power;
			if (powerLabel_ == null)
			{
				powerLabel_ = base.transform.Find("AS_spr_bubble/power/power").GetComponent<UILabel>();
			}
			powerLabel_.transform.parent.gameObject.SetActive(power != 1f);
			powerLabel_.gameObject.SetActive(power != 1f);
			powerLabel_.text = power.ToString();
		}
	}

	private void setLockColor()
	{
		Mesh sharedMesh = sprite.GetComponent<MeshFilter>().sharedMesh;
		Color[] colors = sharedMesh.colors;
		if (isMorgana())
		{
			bool flag = true;
			int num = 0;
			Mesh mesh = null;
			Color[] array = null;
			if (ParentMorgana_ != null)
			{
				mesh = ParentMorgana_.sprite.GetComponent<MeshFilter>().sharedMesh;
				array = mesh.colors;
				foreach (Bubble item in ParentMorgana_.ChildMorgana_)
				{
					if (mLocked)
					{
						break;
					}
					if (item.inCloud)
					{
						num++;
					}
				}
			}
			if (mLocked || num >= 6)
			{
				if (array != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = Color.gray;
						array[i].a = 0.8f;
					}
					ParentMorgana_.AnimStopFlg = true;
					ParentMorgana_.sprite.GetComponent<MeshFilter>().sharedMesh.colors = array;
				}
			}
			else
			{
				if (array == null)
				{
					return;
				}
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = Color.white;
					if (isMorgana())
					{
						float num2 = 0.8f;
						array[j] = new Color(num2, num2, num2, num2);
					}
				}
				ParentMorgana_.AnimStopFlg = false;
				ParentMorgana_.sprite.GetComponent<MeshFilter>().sharedMesh.colors = array;
			}
			return;
		}
		if (isLocked || inCloud)
		{
			if (isMorgana() && IsMorganaParent_)
			{
				bool flag2 = true;
				foreach (Bubble item2 in ChildMorgana_)
				{
					if (mLocked)
					{
						break;
					}
					if (!item2.inCloud && !item2.isLocked)
					{
						flag2 = false;
						break;
					}
				}
				if (!flag2)
				{
					return;
				}
			}
			if (type != eType.Grow)
			{
				for (int k = 0; k < colors.Length; k++)
				{
					colors[k] = Color.gray;
					if (IsMorganaParent_)
					{
						colors[k].a = 0.8f;
					}
				}
			}
			if (sprite != null && type != eType.Grow && (type < eType.TunnelIn || type > eType.TunnelOutLeftUP))
			{
				sprite.Stop();
				sprite.SetFrame(0);
				if (onObstacle != null && onObstacle.gameObject.activeSelf)
				{
					onObstacle.setLockColor(true);
				}
			}
			if (gloss != null)
			{
				StopCoroutine("glossRoutine");
				gloss.gameObject.SetActive(false);
			}
		}
		else
		{
			for (int l = 0; l < colors.Length; l++)
			{
				colors[l] = Color.white;
				if (isMorgana() && IsMorganaParent_)
				{
					float num3 = 0.8f;
					colors[l] = new Color(num3, num3, num3, num3);
				}
			}
			if (onObstacle != null && onObstacle.gameObject.activeSelf)
			{
				onObstacle.setLockColor(false);
			}
			if ((!isCounter(type) || part.getTimeStopCount() <= 0) && sprite != null && type != eType.Grow && (type < eType.ChameleonRed || type > eType.ChameleonBlack))
			{
				setType(type);
			}
		}
		sharedMesh.colors = colors;
	}

	public void init()
	{
		if (part != null && part.bBasicSkill)
		{
			if (part.isParkStage)
			{
				Network.MinilenData current = Bridge.MinilenData.getCurrent();
				if (current.baseSkill_1 >= 0)
				{
					basicSkillColorType.Add((eType)current.baseSkill_1);
				}
				if (current.baseSkill_2 >= 0)
				{
					basicSkillColorType.Add((eType)current.baseSkill_2);
				}
			}
			else
			{
				basicSkillColorType.Add((eType)GlobalData.Instance.currentAvatar.baseSkill_1);
				basicSkillColorType.Add((eType)GlobalData.Instance.currentAvatar.baseSkill_2);
				if (GlobalData.Instance.currentAvatar.baseSkill_3 != -1)
				{
					basicSkillColorType.Add((eType)GlobalData.Instance.currentAvatar.baseSkill_3);
				}
			}
		}
		isFrozen = false;
		myTrans = base.transform;
		setType((eType)int.Parse(base.name));
		sprite.animationEventDelegate = eventDelegate;
		myObject = base.gameObject;
	}

	private void Start()
	{
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
		state = eState.Field;
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
		if (type <= eType.Black || (type >= eType.PlusRed && type <= eType.MinusBlack) || (type >= eType.SnakeRed && type <= eType.SnakeBlack))
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
			if (!isLocked && !isOnChain)
			{
				sprite.Resume();
				sprite.Play("bubble_" + base.name);
			}
			return;
		}
		switch (type)
		{
		case eType.Lightning:
		case eType.LightningG:
		case eType.LightningG_Item:
			sprite.Resume();
			sprite.Play("bubble_59");
			sprite.Resume();
			break;
		case eType.Time:
			sprite.Resume();
			sprite.Play("bubble_60");
			break;
		case eType.Search:
			sprite.Resume();
			sprite.Play("bubble_61");
			break;
		case eType.Coin:
			sprite.Resume();
			sprite.Play("bubble_62");
			break;
		case eType.Star:
			sprite.Resume();
			sprite.Play("bubble_108");
			break;
		case eType.PlusRed:
		case eType.PlusGreen:
		case eType.PlusBlue:
		case eType.PlusYellow:
		case eType.PlusOrange:
		case eType.PlusPurple:
		case eType.PlusWhite:
		case eType.PlusBlack:
			sprite.Resume();
			sprite.Play("bubble_63");
			break;
		case eType.FriendRed:
		case eType.FriendGreen:
		case eType.FriendBlue:
		case eType.FriendYellow:
		case eType.FriendOrange:
		case eType.FriendPurple:
		case eType.FriendWhite:
		case eType.FriendBlack:
		case eType.FriendRainbow:
		case eType.FriendBox:
			sprite.Resume();
			sprite.Play("bubble_64");
			break;
		case eType.Honeycomb:
		{
			sprite.Resume();
			sprite.Play("bubble_58");
			Transform transform = myTrans.Find("bee_eff(Clone)");
			if (transform != null)
			{
				UnityEngine.Object.DestroyImmediate(transform.gameObject);
			}
			break;
		}
		case eType.Counter:
		case eType.CounterRed:
		case eType.CounterGreen:
		case eType.CounterBlue:
		case eType.CounterYellow:
		case eType.CounterOrange:
		case eType.CounterPurple:
		case eType.CounterWhite:
		case eType.CounterBlack:
			if (counterCount != null)
			{
				counterCount.setSpriteEnable(false, false);
			}
			sprite.Resume();
			sprite.Play("bubble_58");
			break;
		case eType.KeyRed:
		case eType.KeyGreen:
		case eType.KeyBlue:
		case eType.KeyYellow:
		case eType.KeyOrange:
		case eType.KeyPurple:
		case eType.KeyWhite:
		case eType.KeyBlack:
			sprite.Resume();
			sprite.Play("bubble_90");
			break;
		case eType.MinilenRed:
		case eType.MinilenGreen:
		case eType.MinilenBlue:
		case eType.MinilenYellow:
		case eType.MinilenOrange:
		case eType.MinilenPurple:
		case eType.MinilenWhite:
		case eType.MinilenBlack:
		case eType.MinilenRainbow:
			sprite.Resume();
			sprite.Play("bubble_137");
			break;
		default:
			sprite.Resume();
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

	public void setType(eType newType)
	{
		if (!sprite.enabled)
		{
			sprite.enabled = true;
		}
		eType eType = type;
		type = newType;
		string text;
		if (type > eType.Blank)
		{
			int num = (int)type;
			text = num.ToString("000");
		}
		else
		{
			int num2 = (int)type;
			text = num2.ToString("00");
		}
		base.name = text;
		if (!isLocked || type == eType.Grow || (type >= eType.TunnelIn && type <= eType.TunnelOutLeftUP))
		{
			sprite.Resume();
			sprite.Play("bubble_" + base.name);
		}
		else if (isLocked)
		{
			sprite.Play("bubble_" + base.name);
			sprite.Stop();
		}
		setGloss();
		setFrozen(isFrozen);
		if (fulcrumCollider_ != null)
		{
			UnityEngine.Object.Destroy(fulcrumCollider_);
			fulcrumCollider_ = null;
		}
		if (type == eType.Fulcrum || type == eType.RotateFulcrumL || type == eType.RotateFulcrumR || (type >= eType.MorganaRed && type <= eType.MorganaBlack) || (type >= eType.TunnelIn && type <= eType.TunnelOutRightDown))
		{
			if (fulcrumColliderBase_ == null)
			{
				fulcrumColliderBase_ = ResourceLoader.Instance.loadGameObject("Prefabs/", "FulcrumCollider");
			}
			fulcrumCollider_ = UnityEngine.Object.Instantiate(fulcrumColliderBase_) as GameObject;
			fulcrumCollider_.transform.parent = myTrans;
			fulcrumCollider_.transform.localPosition = Vector3.zero;
			fulcrumCollider_.transform.localScale = Vector3.one;
			if (type == eType.RotateFulcrumL || type == eType.RotateFulcrumR)
			{
				Transform transform = fulcrumCollider_.transform.Find("RotateRange");
				if (transform != null)
				{
					transform.gameObject.SetActive(true);
				}
			}
			if (type >= eType.MorganaRed && type <= eType.MorganaBlack)
			{
				fulcrumCollider_.name = "MorganaCollider";
			}
			if (type == eType.TunnelIn)
			{
				fulcrumCollider_.name = "warpInCollider";
			}
		}
		if (type == eType.Honeycomb && !isFrozen && !part.isUsedItem(Constant.Item.eType.BeeBarrier) && !inCloud && !isHit)
		{
			Transform transform2 = myTrans.Find("bee_eff(Clone)");
			if (transform2 == null)
			{
				part.honeycomb_eff.SetActive(true);
				GameObject gameObject = UnityEngine.Object.Instantiate(part.honeycomb_eff) as GameObject;
				Utility.setParent(gameObject, myTrans, true);
				part.honeycomb_eff.SetActive(false);
				transform2 = gameObject.transform;
			}
			transform2.localPosition = BEE_DEFAULT_POS;
		}
		else if (eType == eType.Honeycomb)
		{
			Transform transform3 = myTrans.Find("bee_eff(Clone)");
			if (transform3 != null)
			{
				UnityEngine.Object.DestroyImmediate(transform3.gameObject);
			}
		}
		if (type == eType.Honeycomb)
		{
			Transform transform4 = myTrans.Find("bee_eff(Clone)");
			if (transform4 != null)
			{
				if (mLocked || mCloud)
				{
					transform4.gameObject.SetActive(false);
				}
				Animation animation = transform4.GetComponent<Animation>();
				animation.clip = animation["Bee_anm"].clip;
				animation.Play();
			}
		}
		if (isCounter(type) && !isFrozen)
		{
			if (counterCount == null)
			{
				createCounter();
			}
			counterCount.gameObject.transform.localPosition = COUNTER_DEFALUT_POS;
		}
		else if (isCounter(eType) && counterCount != null)
		{
			UnityEngine.Object.DestroyImmediate(counterCount.gameObject);
		}
		if (part != null && part.bBasicSkill)
		{
			SetBasicSkill(!isLocked && !mCloud, true);
		}
	}

	private void showBasicSkillIcon()
	{
		eType eType = part.convertColorBubble(type);
		foreach (eType item in basicSkillColorType)
		{
			if (item == eType && !isFrozen)
			{
				isBasicSkillColor = true;
				break;
			}
			isBasicSkillColor = false;
		}
	}

	public void SetBasicSkill(bool bShow, bool bFlag)
	{
		eType eType = part.convertColorBubble(type);
		foreach (eType item in basicSkillColorType)
		{
			if (item == eType && !isFrozen)
			{
				if (Score_up.activeSelf != bShow || isBasicSkillColor != bFlag)
				{
					isBasicSkillColor = bFlag;
				}
				return;
			}
		}
		isBasicSkillColor = false;
	}

	public void SetBasicSkillIcon(bool bShow)
	{
		if (part == null)
		{
			return;
		}
		eType eType = part.convertColorBubble(type);
		foreach (eType item in basicSkillColorType)
		{
			if (item == eType && Score_up.activeSelf != bShow && !isFrozen)
			{
				Score_up.SetActive(bShow);
				break;
			}
		}
	}

	public void setBreak()
	{
		state = eState.Break;
	}

	public void shot(Vector3 fireVector)
	{
		state = eState.Shot;
		if (type != eType.Metal)
		{
			StartCoroutine(shotMoveRoutine(fireVector));
			return;
		}
		isHitCloud = guide.inCloud;
		if (part != null)
		{
			part.hit(this, null);
		}
		else if (rankingPart != null)
		{
			rankingPart.hit(this, null);
		}
	}

	private IEnumerator shotMoveRoutine(Vector3 fireVector)
	{
		Bubble[] fieldBubbles = null;
		if (part != null)
		{
			fieldBubbles = part.fieldBubbleList.ToArray();
		}
		else if (bonusPart != null)
		{
			fieldBubbles = bonusPart.fieldBubbleList.ToArray();
		}
		else if (rankingPart != null)
		{
			fieldBubbles = rankingPart.fieldBubbleList.ToArray();
		}
		fireVector.z = 0f;
		fireVector.Normalize();
		Vector3 moveVector = fireVector * SPEED;
		boundCount = 0;
		float x_min = 0.1f;
		float x_max = 539.9f;
		yield return stagePause.sync();
		yield return stagePause.sync();
		float moved = 0f;
		while (state == eState.Shot)
		{
			Vector3 vec = moveVector * Time.deltaTime;
			Vector3 pos2 = myTrans.localPosition + vec;
			if (pos2.x < x_min || pos2.x > x_max)
			{
				float rate;
				if (vec.x < 0f)
				{
					rate = (pos2.x - x_min) / vec.x;
					if (part != null)
					{
						StartCoroutine(boundEffRoutine(part.boundEffL, true));
					}
					else if (bonusPart != null)
					{
						StartCoroutine(boundEffRoutine(bonusPart.boundEffL, true));
					}
					else if (rankingPart != null)
					{
						StartCoroutine(boundEffRoutine(rankingPart.boundEffL, true));
					}
				}
				else
				{
					rate = (pos2.x - x_max) / vec.x;
					if (part != null)
					{
						StartCoroutine(boundEffRoutine(part.boundEffR, false));
					}
					else if (bonusPart != null)
					{
						StartCoroutine(boundEffRoutine(bonusPart.boundEffR, true));
					}
					else if (rankingPart != null)
					{
						StartCoroutine(boundEffRoutine(rankingPart.boundEffR, true));
					}
				}
				pos2 = myTrans.localPosition + vec * (1f - rate);
				vec.x = 0f - vec.x;
				pos2 += vec * rate;
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
			Bubble[] array = fieldBubbles;
			foreach (Bubble fieldBubble in array)
			{
				if (fieldBubble.type != eType.TunnelIn || guide.tunnelInBubble == null)
				{
					continue;
				}
				float hitCheckLength = (fieldBubble.myTrans.localPosition - pos2).magnitude;
				if (mHitSize >= hitCheckLength)
				{
					yield return StartCoroutine(warpInRutine(pos2, fieldBubble));
					pos2 = fieldBubble.OutObject.transform.localPosition;
					moveVector = fieldBubble.OutObject.transform.Find("AS_spr_bubble").transform.up * SPEED;
					yield return StartCoroutine(warpOutRutine(pos2, fieldBubble));
					if (fieldBubble.OutObject.GetComponent<Bubble>().outObjectType == 1 || fieldBubble.OutObject.GetComponent<Bubble>().outObjectType == 2 || fieldBubble.OutObject.GetComponent<Bubble>().outObjectType == 4)
					{
						StartCoroutine(warpOutEffRutine(part.boundEffR, fieldBubble.OutObject));
					}
					else
					{
						StartCoroutine(warpOutEffRutine(part.boundEffL, fieldBubble.OutObject));
					}
				}
			}
			myTrans.localPosition = pos2;
			moved += vec.magnitude;
			if (moved < guide.hitMoveLength)
			{
				yield return stagePause.sync();
				continue;
			}
			pos2 = myTrans.position;
			pos2.x = guide.hitPos.x;
			pos2.y = guide.hitPos.y;
			myTrans.position = pos2;
			BubbleBase hitBubble = guide.hitBubble;
			Vector3 diff2 = guide.hitDiff;
			myTrans.Find("bubble_trail_eff").parent = myTrans.parent.parent.parent;
			if (guide.inCloud)
			{
				if (type == eType.Water)
				{
					Sound.Instance.playSe(Sound.eSe.SE_515_water_bubble_hit);
				}
				else if (type == eType.Shine)
				{
					Sound.Instance.playSe(Sound.eSe.SE_519_shine_bubble_hit);
				}
				isHitCloud = true;
				if (part != null)
				{
					part.hit(this, null);
				}
				break;
			}
			if (hitBubble == null)
			{
				if (type == eType.Water)
				{
					Sound.Instance.playSe(Sound.eSe.SE_515_water_bubble_hit);
				}
				else if (type == eType.Shine)
				{
					Sound.Instance.playSe(Sound.eSe.SE_519_shine_bubble_hit);
				}
				if (part != null)
				{
					part.hit(this, this);
				}
				break;
			}
			Vector3 correctpos = hitBubble.myTrans.localPosition + getCorrectOffset(diff2);
			correctpos.z = 0f;
			iTween.MoveTo(base.gameObject, iTween.Hash("position", correctpos, "time", 0.05f, "islocal", true, "easetype", iTween.EaseType.linear));
			shock(-diff2 * 0.8f);
			Bubble[] array2 = fieldBubbles;
			foreach (Bubble fieldBubble2 in array2)
			{
				if (!fieldBubble2.isLocked && !fieldBubble2.inCloud && fieldBubble2.myTrans.childCount != 0 && fieldBubble2.type != eType.Blank)
				{
					diff2 = fieldBubble2.myTrans.localPosition - correctpos;
					if (!(diff2.sqrMagnitude > 44100f))
					{
						fieldBubble2.shock(diff2);
					}
				}
			}
			if (type == eType.Water)
			{
				Sound.Instance.playSe(Sound.eSe.SE_515_water_bubble_hit);
			}
			else if (type == eType.Shine)
			{
				Sound.Instance.playSe(Sound.eSe.SE_519_shine_bubble_hit);
			}
			else
			{
				Sound.Instance.playSe(Sound.eSe.SE_216_sessyoku);
			}
			while (base.gameObject.GetComponentInChildren<iTween>() != null)
			{
				yield return stagePause.sync();
			}
			myTrans.localPosition = correctpos;
			if (part != null)
			{
				part.hit(this, hitBubble);
			}
			else if (bonusPart != null)
			{
				bonusPart.hit(this, hitBubble);
			}
			else if (rankingPart != null)
			{
				rankingPart.hit(this, hitBubble);
			}
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
			yield return stagePause.sync();
		}
		boundEff.SetActive(false);
	}

	private IEnumerator warpOutEffRutine(GameObject boundEff, GameObject warpOutPos)
	{
		Transform effTrans = boundEff.transform;
		Quaternion rot = warpOutPos.transform.Find("AS_spr_bubble").transform.rotation;
		rot = ((warpOutPos.GetComponent<Bubble>().outObjectType != 1 && warpOutPos.GetComponent<Bubble>().outObjectType != 2 && warpOutPos.GetComponent<Bubble>().outObjectType != 4) ? Quaternion.Euler(0f, 0f, rot.eulerAngles.z + 30f) : Quaternion.Euler(0f, 0f, rot.eulerAngles.z - 30f));
		effTrans.rotation = rot;
		effTrans.position = warpOutPos.transform.position + warpOutPos.transform.Find("AS_spr_bubble").transform.up * (30f * guide.uiScale);
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
			yield return stagePause.sync();
		}
		boundEff.SetActive(false);
	}

	private Vector3 getCorrectOffset(Vector3 diff)
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
		if (state == eState.Break)
		{
			return;
		}
		if (onObstacle != null && onObstacle.gameObject.activeSelf)
		{
			part.StartCoroutine(onObstacle.breakRoutine(true));
		}
		if (type == eType.Search)
		{
			part.isSearching = true;
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
		if (isLineFriend && part != null && part.state < Part_Stage.eState.Clear)
		{
			isLineFriend = false;
			lineFriendIndex = -1;
			part.lineFriendBonus(lineFriendIcon);
		}
		if (isKeyBubble() && part != null && part.state < Part_Stage.eState.Clear)
		{
			part.getKeyCount++;
			part.keyBubbleBonus(myTrans);
		}
		if (part != null && isBasicSkillColor && part.bBasicSkill && !isFrozen)
		{
			part.basicSkillBreakNum++;
			isBasicSkillColor = false;
		}
		rescue();
		vanishMinilen();
		if (bParticle && type != eType.Grow)
		{
			scoreParticle();
		}
		state = eState.Break;
		if (myTrans.localPosition.y > 155f && type != eType.Skull)
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
		Score_up.SetActive(false);
		UnityEngine.Object.Destroy(sprite.GetComponent<iTween>());
		sprite.enabled = true;
		myTrans.localPosition += Vector3.back * 0.01f;
		eType convertedType = eType.Invalid;
		if (part != null)
		{
			convertedType = part.convertColorBubble(type);
		}
		else if (bonusPart != null)
		{
			convertedType = bonusPart.convertColorBubble(type);
		}
		else if (rankingPart != null)
		{
			convertedType = rankingPart.convertColorBubble(type);
		}
		if (convertedType == eType.FriendRainbow || convertedType == eType.MinilenRainbow)
		{
			convertedType = eType.Rainbow;
		}
		if (counterCount != null)
		{
			counterCount.setSpriteEnable(false, false);
		}
		if (isFrozen)
		{
			sprite.Resume();
			sprite.Play("burst_57");
			while (sprite.IsPlaying(sprite.CurrentClip))
			{
				yield return stagePause.sync();
			}
		}
		else if (isSplash && !isCounter(type))
		{
			sprite.Resume();
			sprite.Play("burst_67");
			while (sprite.IsPlaying(sprite.CurrentClip))
			{
				yield return stagePause.sync();
			}
		}
		else if (type == eType.Skull)
		{
			GameObject skull = UnityEngine.Object.Instantiate(part.skullBase) as GameObject;
			Utility.setParent(skull, myTrans.parent.parent, true);
			skull.transform.position = myTrans.position;
			Vector3 pos = skull.transform.localPosition;
			pos.z = -17f;
			skull.transform.localPosition = pos;
			skull.SetActive(true);
			sprite.gameObject.SetActive(false);
			while (skull.GetComponent<Animation>().isPlaying)
			{
				yield return stagePause.sync();
			}
			UnityEngine.Object.Destroy(skull);
		}
		else if (type >= eType.ChameleonRed && type <= eType.ChameleonBlack)
		{
			sprite.Resume();
			sprite.Play("burst_" + ((int)(type - 79)).ToString("00"));
			while (sprite.IsPlaying(sprite.CurrentClip))
			{
				yield return stagePause.sync();
			}
		}
		else if (type >= eType.CounterRed && type <= eType.CounterBlack)
		{
			sprite.Resume();
			sprite.Play("burst_" + ((int)(type - 100)).ToString("00"));
			while (sprite.IsPlaying(sprite.CurrentClip))
			{
				yield return stagePause.sync();
			}
		}
		else if (type >= eType.TunnelIn && type <= eType.TunnelOutRightDown)
		{
			sprite.Resume();
			sprite.Play("burst_51");
			while (sprite.IsPlaying(sprite.CurrentClip))
			{
				yield return stagePause.sync();
			}
		}
		else if (type == eType.LightningG || type == eType.LightningG_Item)
		{
			sprite.Resume();
			sprite.Play("burst_08");
			while (sprite.IsPlaying(sprite.CurrentClip))
			{
				yield return stagePause.sync();
			}
		}
		else
		{
			sprite.Resume();
			string text;
			if (convertedType > eType.Blank)
			{
				int num = (int)convertedType;
				text = num.ToString("000");
			}
			else
			{
				int num2 = (int)convertedType;
				text = num2.ToString("00");
			}
			string spriteNum = text;
			sprite.Play("burst_" + spriteNum);
			while (sprite.IsPlaying(sprite.CurrentClip))
			{
				yield return stagePause.sync();
			}
		}
		if (onObstacle != null && onObstacle.gameObject.activeSelf)
		{
			part.StartCoroutine(onObstacle.breakRoutine(true));
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
		if (onObstacle != null && onObstacle.gameObject.activeSelf)
		{
			part.StartCoroutine(onObstacle.breakRoutine(true));
		}
		state = eState.Drop;
		StartCoroutine(dropRoutine(delay));
	}

	private IEnumerator dropRoutine(int delay)
	{
		if (isKeyBubble() && part != null && part.state < Part_Stage.eState.Clear)
		{
			startBreak();
			yield break;
		}
		if (type == eType.LightningG)
		{
			part._droped_lightning_g.Add(this);
		}
		dropMinilen();
		Rigidbody rb = base.gameObject.AddComponent<Rigidbody>();
		rb.isKinematic = false;
		rb.constraints = RigidbodyConstraints.FreezeAll;
		float xForce = -5f + (float)rand.Next(11);
		float yForce = 80f + (float)rand.Next(10);
		float waitTime = 0f;
		while (waitTime < 0.01f * (float)delay)
		{
			waitTime += Time.deltaTime;
			yield return stagePause.sync();
		}
		yield return stagePause.sync();
		myTrans.localPosition += Vector3.forward * 3f;
		if (type == eType.Honeycomb)
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
		if (!isFrozen && type >= eType.SnakeRed && type <= eType.SnakeBlack)
		{
			GameObject snake_eff = null;
			bool bCreate = false;
			float changeTime = 0f;
			while (myTrans.localPosition.y > -360f)
			{
				changeTime += Time.deltaTime;
				if (!bCreate && changeTime > 0.3f)
				{
					bCreate = true;
					snake_eff = createSnakeEffect();
				}
				yield return stagePause.sync();
			}
			UnityEngine.Object.Destroy(rb);
			if (snake_eff != null)
			{
				StartCoroutine(fadeoutSnakeRoutine(snake_eff.GetComponentInChildren<UISprite>()));
			}
			StartCoroutine(fadeoutRoutine());
			yield break;
		}
		while (myTrans.localPosition.y > -420f)
		{
			yield return stagePause.sync();
		}
		Sound.Instance.playSe(Sound.eSe.SE_216_sessyoku);
		Vector3 p = myTrans.localPosition;
		p.y = -420f;
		myTrans.localPosition = p;
		rb.isKinematic = false;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		rb.AddForce(new Vector3(xForce, yForce, 0f));
		yield return stagePause.sync();
		yield return stagePause.sync();
		while (rb.velocity.y > 0f)
		{
			yield return stagePause.sync();
		}
		UnityEngine.Object.Destroy(rb);
		if (type == eType.LightningG)
		{
			sprite.Resume();
			sprite.Play("burst_08");
			Sound.Instance.stopSe(Sound.eSe.SE_013_park_gift);
			Sound.Instance.playSe(Sound.eSe.SE_013_park_gift);
			while (sprite.IsPlaying(sprite.CurrentClip))
			{
				yield return stagePause.sync();
			}
			part.AddLightningG();
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (isColorBubble() || type == eType.FriendBox || type == eType.FriendRainbow || type == eType.FriendFulcrum || type == eType.MinilenRainbow)
		{
			rescue();
			rescueMinilen();
			StartCoroutine(breakRoutine(true));
		}
		else
		{
			StartCoroutine(fadeoutRoutine());
		}
	}

	private void rescue()
	{
		if ((type >= eType.FriendRed && type <= eType.FriendBox) || type == eType.FriendFulcrum)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(part.chacknBase) as GameObject;
			Utility.setParent(gameObject, part.getScrollUI(), true);
			gameObject.transform.position = myTrans.position;
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
			Chackn chackn = gameObject.AddComponent<Chackn>();
			chackn.setPartStage(part);
			part.chacknList.Add(chackn);
			chackn.animStart(stagePause);
			Sound.Instance.playSe((Sound.eSe)(40 + rand.Next(3)));
		}
	}

	private void rescueMinilen()
	{
		if ((type >= eType.MinilenRed && type <= eType.MinilenBlack) || type == eType.MinilenRainbow)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(part.minilenBase) as GameObject;
			Utility.setParent(gameObject, part.getScrollUI(), true);
			gameObject.transform.position = myTrans.position;
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.z = 1f;
			gameObject.transform.localPosition = localPosition;
			gameObject.SetActive(true);
			StageMinilen stageMinilen = gameObject.AddComponent<StageMinilen>();
			part.AddDropedMinilenInstanceId(uniqueId);
			if (part.isHitMinilen(uniqueId))
			{
				stageMinilen.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_141");
			}
			part.minilenList.Add(stageMinilen);
			stageMinilen.animStart(part);
		}
	}

	private void vanishMinilen()
	{
		if (((type >= eType.MinilenRed && type <= eType.MinilenBlack) || type == eType.MinilenRainbow) && part.state != Part_Stage.eState.Clear)
		{
			part.vanishMinilen();
			GameObject gameObject = UnityEngine.Object.Instantiate(part.minilenBase) as GameObject;
			Utility.setParent(gameObject, part.getScrollUI(), true);
			gameObject.transform.position = myTrans.position;
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.z = 1f;
			gameObject.transform.localPosition = localPosition;
			gameObject.SetActive(true);
			StageMinilen stageMinilen = gameObject.AddComponent<StageMinilen>();
			stageMinilen.Escape(part);
			Sound.Instance.playSe(Sound.eSe.SE_109_OhNo);
		}
	}

	private void dropMinilen()
	{
		if ((type >= eType.MinilenRed && type <= eType.MinilenBlack) || type == eType.MinilenRainbow)
		{
			part.addMinilenNum();
		}
	}

	private void scoreParticle()
	{
		if (bonusPart != null)
		{
			bonusScoreParticle();
		}
		else if (rankingPart != null)
		{
			rankingScoreParticle();
		}
		else if ((!(part != null) || part.state != Part_Stage.eState.Clear) && (type < eType.FriendRainbow || isFrozen || !(part != null) || part.isSpecialBubble(type)))
		{
			GameObject gameObject = null;
			if (part != null)
			{
				gameObject = UnityEngine.Object.Instantiate(part.scoreParticleBase) as GameObject;
				Utility.setParent(gameObject, myTrans.parent.parent.parent.Find("Front_ui"), true);
				gameObject.transform.position = myTrans.position;
				Vector3 localPosition = gameObject.transform.localPosition;
				localPosition.z = -1f;
				gameObject.transform.localPosition = localPosition;
				gameObject.SetActive(true);
				ScoreParticle scoreParticle = gameObject.AddComponent<ScoreParticle>();
				scoreParticle.animStart(stagePause);
			}
		}
	}

	private void bonusScoreParticle()
	{
		if (bonusPart.state != Part_BonusStage.eState.Clear && bonusPart.state != Part_BonusStage.eState.End && type == eType.Coin)
		{
			GameObject gameObject = null;
			if (bonusPart != null)
			{
				gameObject = UnityEngine.Object.Instantiate(bonusPart.scoreParticleBase) as GameObject;
				Utility.setParent(gameObject, myTrans.parent.parent.parent.Find("Front_ui"), true);
				gameObject.transform.position = myTrans.position;
				Vector3 localPosition = gameObject.transform.localPosition;
				localPosition.z = -1f;
				gameObject.transform.localPosition = localPosition;
				gameObject.SetActive(true);
				ScoreParticle scoreParticle = gameObject.AddComponent<ScoreParticle>();
				scoreParticle.bonusAnimStart(stagePause);
			}
		}
	}

	private void rankingScoreParticle()
	{
		if (rankingPart.state != Part_RankingStage.eState.Clear && rankingPart.state != Part_RankingStage.eState.End && type == eType.Coin)
		{
			GameObject gameObject = null;
			if (rankingPart != null)
			{
				gameObject = UnityEngine.Object.Instantiate(rankingPart.scoreParticleBase) as GameObject;
				Utility.setParent(gameObject, myTrans.parent.parent.parent.Find("Front_ui"), true);
				gameObject.transform.position = myTrans.position;
				Vector3 localPosition = gameObject.transform.localPosition;
				localPosition.z = -1f;
				gameObject.transform.localPosition = localPosition;
				gameObject.SetActive(true);
				ScoreParticle scoreParticle = gameObject.AddComponent<ScoreParticle>();
				scoreParticle.rankingAnimStart(stagePause);
			}
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
		if (type != eType.Honeycomb || !part.isUsedItem(Constant.Item.eType.BeeBarrier) || !isCounter(type) || !(counterCount != null) || counterCount.getCount() > 0)
		{
			if (part != null)
			{
				filter.GetComponent<Renderer>().sharedMaterial = part.fadeMaterial;
			}
			else if (bonusPart != null)
			{
				filter.GetComponent<Renderer>().sharedMaterial = bonusPart.fadeMaterial;
			}
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
			yield return stagePause.sync();
		}
		if (onObstacle != null && onObstacle.gameObject.activeSelf)
		{
			StartCoroutine(onObstacle.breakRoutine(true));
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
			yield return stagePause.sync();
		}
		if (onObstacle != null && onObstacle.gameObject.activeSelf)
		{
			StartCoroutine(onObstacle.breakRoutine(true));
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
		state = eState.Gameover;
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
			if (state == eState.Field)
			{
				yield break;
			}
			waitTime += Time.deltaTime;
			yield return stagePause.sync();
		}
		if (state != eState.Field)
		{
		}
	}

	public void resurrection(Material mat)
	{
		StopCoroutine("gameoverRoutine");
		state = eState.Field;
		setType(type);
		sprite.enabled = true;
		sprite.GetComponent<Renderer>().material = mat;
		if (type == eType.Grow)
		{
			sprite.Resume();
			sprite.PlayFromFrame(1);
		}
		if (type < eType.MorganaRed || type > eType.MorganaBlack)
		{
			Color color = sprite.color;
			color.a = 1f;
			sprite.color = color;
		}
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
		eType type = child.transform.parent.GetComponent<Bubble>().type;
		if (eType.MorganaRed <= type && eType.MorganaBlack >= type)
		{
			iTween.MoveTo(child, iTween.Hash("position", pos + Vector3.back, "time", 0.05f, "islocal", true, "easetype", iTween.EaseType.linear));
		}
		else
		{
			iTween.MoveTo(child, iTween.Hash("position", pos, "time", 0.05f, "islocal", true, "easetype", iTween.EaseType.linear));
		}
		if (counterCount != null)
		{
			iTween.MoveTo(counterCount.gameObject, iTween.Hash("position", pos + Vector3.back * 3f, "time", 0.05f, "islocal", true, "easetype", iTween.EaseType.linear));
		}
		if (IsMorganaParent_)
		{
			iTween.MoveTo(CharaObj_, iTween.Hash("position", CharaObj_.transform.localPosition + pos, "time", 0.05f, "islocal", true, "easetype", iTween.EaseType.linear));
		}
		while (child.GetComponent<iTween>() != null)
		{
			yield return stagePause.sync();
		}
		if (eType.MorganaRed <= type && eType.MorganaBlack >= type)
		{
			iTween.MoveTo(child, iTween.Hash("position", Vector3.back, "time", 0.1f, "islocal", true, "easetype", iTween.EaseType.linear));
		}
		else
		{
			iTween.MoveTo(child, iTween.Hash("position", Vector3.zero, "time", 0.1f, "islocal", true, "easetype", iTween.EaseType.linear));
		}
		if (counterCount != null)
		{
			iTween.MoveTo(counterCount.gameObject, iTween.Hash("position", Vector3.back * 3f, "time", 0.1f, "islocal", true, "easetype", iTween.EaseType.linear));
		}
		if (IsMorganaParent_)
		{
			iTween.MoveTo(CharaObj_, iTween.Hash("position", CharaObj_.transform.localPosition - pos, "time", 0.1f, "islocal", true, "easetype", iTween.EaseType.linear));
		}
		while (child.GetComponent<iTween>() != null)
		{
			yield return stagePause.sync();
		}
	}

	public void startGloss(eType glossType)
	{
		if ((!(part != null) || !part.stagePause.pause) && (!(bonusPart != null) || !bonusPart.stagePause.pause) && (state != eState.Field || !(myTrans.localPosition.y > 105f)) && glossType == type && !(gloss == null))
		{
			StopCoroutine("glossRoutine");
			StartCoroutine("glossRoutine");
		}
	}

	private IEnumerator glossRoutine()
	{
		if (mLocked || mCloud)
		{
			yield break;
		}
		gloss.gameObject.SetActive(true);
		gloss.Play();
		while (gloss != null && gloss.IsPlaying(glossClipName))
		{
			if (part != null)
			{
				yield return part.stagePause.sync();
			}
			else if (bonusPart != null)
			{
				yield return bonusPart.stagePause.sync();
			}
		}
		if (gloss != null)
		{
			gloss.gameObject.SetActive(false);
		}
	}

	private GameObject createSnakeEffect()
	{
		GameObject gameObject = null;
		base.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
		gameObject = UnityEngine.Object.Instantiate(part.snake_eff[0]) as GameObject;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		gameObject.SetActive(true);
		Animation component = gameObject.GetComponent<Animation>();
		component.clip = component["snake_anm_01"].clip;
		component.Play();
		UISprite componentInChildren = gameObject.GetComponentInChildren<UISprite>();
		Color color = componentInChildren.color;
		color.a = 1f;
		componentInChildren.color = color;
		return gameObject;
	}

	public void setCounterCount(int count)
	{
		if (counterCount == null && !isFrozen)
		{
			createCounter();
			counterCount.gameObject.transform.localPosition = COUNTER_DEFALUT_POS;
		}
		if (counterCount != null)
		{
			counterCount.setCount(count, part.bUsingTimeStop);
			counterCount.changeAnim(part.bUsingTimeStop);
		}
	}

	public bool startCountDown()
	{
		if (!isCounter(type) || isFrozen || isLocked || inCloud)
		{
			return false;
		}
		if (counterCount != null)
		{
			return counterCount.countDown();
		}
		return false;
	}

	public bool isPlayingCounterAnim()
	{
		if (!isCounter(type) || isFrozen || isLocked || inCloud)
		{
			return false;
		}
		if (counterCount != null)
		{
			return counterCount.isPlayingCountdown();
		}
		return false;
	}

	public bool isCountOver()
	{
		if (!isCounter(type) || isFrozen || isLocked || inCloud)
		{
			return false;
		}
		if (counterCount != null && counterCount.isCountOver())
		{
			counterCount.setCountEnable(type, false, false);
			return true;
		}
		return false;
	}

	public void setCounterEnable(bool enable)
	{
		if (!(counterCount == null))
		{
			counterCount.setCountEnable(type, enable, enable && !isOnChain && !isLocked);
			sprite.enabled = enable;
			if (!enable)
			{
				counterCount.setSpriteEnable(false, false);
			}
		}
	}

	public void setColorCounterEnable(bool enable)
	{
		if (!(counterCount == null))
		{
			counterCount.setCountEnable(type, enable, enable && !isOnChain && !isLocked);
			if (!enable)
			{
				counterCount.setSpriteEnable(false, false);
			}
		}
	}

	public int getCounterCount()
	{
		if (counterCount == null)
		{
			return -1;
		}
		return counterCount.getCount();
	}

	public void checkCounterEnable()
	{
		if (!(counterCount == null))
		{
			counterCount.setCountEnable(type, counterCount.getCount() > 0, counterCount.getCount() > 0 && !isOnChain && !isLocked);
		}
	}

	public IEnumerator countOverEffect()
	{
		if (!(counterCount == null))
		{
			yield return StartCoroutine(counterCount.playOverEffect(type));
		}
	}

	public bool isCountOverEffectPlaying()
	{
		if (counterCount == null)
		{
			return false;
		}
		return counterCount.isPlayingOverEffect();
	}

	public IEnumerator warpInRutine(Vector3 movePos, Bubble hitBubble)
	{
		GameObject AS_spr = base.transform.Find("AS_spr_bubble").gameObject;
		Sound.Instance.playSe(Sound.eSe.SE_534_blackhole);
		movePos += Vector3.back;
		AS_spr.transform.localPosition = myTrans.localPosition - hitBubble.myTrans.localPosition;
		myTrans.localPosition = new Vector3(hitBubble.myTrans.localPosition.x, hitBubble.myTrans.localPosition.y, hitBubble.myTrans.localPosition.z - 0.5f);
		iTween.ShakePosition(base.gameObject, iTween.Hash("x", 0.02f, "y", 0.01f, "time", 1f, "easetype", iTween.EaseType.linear));
		iTween.ScaleTo(base.gameObject, iTween.Hash("x", 0f, "y", 0f, "z", 0f, "time", 1f, "easetype", iTween.EaseType.linear));
		while (GetComponent<iTween>() != null)
		{
			yield return stagePause.sync();
		}
	}

	public IEnumerator warpOutRutine(Vector3 outPos, Bubble hitBubble)
	{
		GameObject AS_spr = base.transform.Find("AS_spr_bubble").gameObject;
		outPos += Vector3.back;
		AS_spr.transform.localPosition = hitBubble.OutObject.transform.localPosition + hitBubble.OutObject.transform.Find("AS_spr_bubble").transform.up * mHitSize - outPos;
		myTrans.localPosition = outPos;
		hitBubble.OutObject.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_124");
		iTween.ScaleTo(base.gameObject, iTween.Hash("x", 1f, "y", 1f, "z", 1f, "time", 0.2f, "easetype", iTween.EaseType.linear));
		Sound.Instance.playSe(Sound.eSe.SE_604_cannon);
		while (GetComponent<iTween>() != null)
		{
			yield return stagePause.sync();
		}
	}

	public IEnumerator warpOutRutineMetal(Vector3 outPos, Bubble hitBubble)
	{
		GameObject AS_spr = base.transform.Find("AS_spr_bubble").gameObject;
		outPos += Vector3.back;
		AS_spr.transform.localPosition = hitBubble.OutObject.transform.localPosition - outPos;
		myTrans.localPosition = outPos;
		hitBubble.OutObject.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_124");
		iTween.ScaleTo(base.gameObject, iTween.Hash("x", 1f, "y", 1f, "z", 1f, "time", 0.2f, "easetype", iTween.EaseType.linear));
		Sound.Instance.playSe(Sound.eSe.SE_604_cannon);
		while (GetComponent<iTween>() != null)
		{
			yield return stagePause.sync();
		}
	}

	public void createCounter()
	{
		part.counter_count.SetActive(true);
		GameObject gameObject = UnityEngine.Object.Instantiate(part.counter_count) as GameObject;
		counterCount = gameObject.GetComponent<Counter>();
		Utility.setParent(gameObject, myTrans, true);
		counterCount.part_ = part;
		if (type >= eType.CounterRed && type <= eType.CounterBlack)
		{
			counterCount.setupGasColor(type);
		}
		part.counter_count.SetActive(false);
	}

	public void setCounterSpriteEnable(bool enabled)
	{
		counterCount.setSpriteEnable(enabled, enabled && !isOnChain && !isLocked);
	}

	public void checkCounterExistante()
	{
		if (counterCount == null)
		{
			createCounter();
		}
	}

	public bool getCounterEnable()
	{
		if (counterCount == null)
		{
			return false;
		}
		return counterCount.getCounterEnable();
	}

	public void setTimeStop(bool enable)
	{
		sprite.enabled = !enable;
		if (counterCount != null)
		{
			if (isOnChain || isLocked)
			{
				counterCount.setGasEnable(false);
			}
			else
			{
				counterCount.setGasEnable(!enable);
			}
		}
	}

	public bool isShakeRangeBubble(Bubble target)
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
		if (type <= eType.Black)
		{
			return true;
		}
		if (type >= eType.PlusRed && type <= eType.PlusBlack)
		{
			return true;
		}
		if (type >= eType.MinusRed && type <= eType.MinusBlack)
		{
			return true;
		}
		if (type >= eType.FriendRed && type <= eType.FriendBlack)
		{
			return true;
		}
		if (type >= eType.SnakeRed && type <= eType.SnakeBlack)
		{
			return true;
		}
		if (type >= eType.KeyRed && type <= eType.KeyBlack)
		{
			return true;
		}
		if (type >= eType.CounterRed && type <= eType.CounterBlack)
		{
			return true;
		}
		if (type >= eType.MinilenRed && type <= eType.MinilenBlack)
		{
			return true;
		}
		return false;
	}

	public bool isColorBubbleFixed()
	{
		if (type <= eType.Black)
		{
			return true;
		}
		if (type >= eType.PlusRed && type <= eType.PlusBlack)
		{
			return true;
		}
		if (type >= eType.MinusRed && type <= eType.MinusBlack)
		{
			return true;
		}
		if (type >= eType.FriendRed && type <= eType.FriendBlack)
		{
			return true;
		}
		if (type >= eType.SnakeRed && type <= eType.SnakeBlack)
		{
			return true;
		}
		if (type >= eType.ChameleonRed && type <= eType.ChameleonBlack)
		{
			return true;
		}
		if (type >= eType.KeyRed && type <= eType.KeyBlack)
		{
			return true;
		}
		if (type >= eType.CounterRed && type <= eType.CounterBlack)
		{
			return true;
		}
		if (type >= eType.MorganaRed && type <= eType.MorganaBlack)
		{
			return true;
		}
		if (type >= eType.MinilenRed && type <= eType.MinilenBlack)
		{
			return true;
		}
		return false;
	}

	public bool isKeyBubble()
	{
		return type >= eType.KeyRed && type <= eType.KeyBlack;
	}

	public bool isCounter(eType bubbleType)
	{
		return bubbleType == eType.Counter || (bubbleType >= eType.CounterRed && bubbleType <= eType.CounterBlack);
	}

	private void eventDelegate(tk2dAnimatedSprite sp, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame, int frameNum)
	{
		float num = 418f * ((float)frame.eventInt / 100f);
		sp.transform.localScale = new Vector3(num, num, 1f);
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, frame.eventFloat);
	}

	public bool isMorgana()
	{
		return type >= eType.MorganaRed && type <= eType.MorganaBlack;
	}

	public void Setup()
	{
		if (CharaObj_ == null)
		{
			CharaObj_ = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", CharaNames_[CharaNum_])) as GameObject;
		}
		else if (CharaNames_[CharaNum_] != CharaObj_.name.Replace("(Clone)", string.Empty))
		{
			GameObject charaObj_ = CharaObj_;
			CharaObj_ = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", CharaNames_[CharaNum_])) as GameObject;
			UnityEngine.Object.Destroy(charaObj_);
		}
		CharaObj_.transform.SetParent(base.transform, false);
		CharaObj_.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
		CharaObj_.transform.localPosition = new Vector3(0f, -46f, 2f);
		CharaObj_.GetComponentInChildren<tk2dAnimatedSprite>().Play(CharaNames_[CharaNum_] + CharaAnimNo_[MorganaHP_, CharaNum_]);
		switch (MorganaHP_)
		{
		case 3:
			sprite.gameObject.transform.localScale = new Vector3(370f, 370f, sprite.gameObject.transform.localScale.z);
			break;
		case 2:
			sprite.gameObject.transform.localScale = new Vector3(460f, 460f, sprite.gameObject.transform.localScale.z);
			break;
		case 1:
			sprite.gameObject.transform.localScale = new Vector3(522f, 522f, sprite.gameObject.transform.localScale.z);
			break;
		}
		sprite.color = new Color(1f, 1f, 1f, 0.8f);
		sprite.transform.localPosition = new Vector3(sprite.transform.localPosition.x, sprite.transform.localPosition.y, -1f);
	}

	public IEnumerator Hit()
	{
		OnDamage_ = true;
		MorganaHP_--;
		switch (MorganaHP_)
		{
		case 2:
			iTween.ScaleTo(sprite.gameObject, iTween.Hash("x", 460f, "y", 460f, "time", 1f, "easetype", iTween.EaseType.linear));
			Sound.Instance.playSe(Sound.eSe.SE_331_searchbubble01);
			yield return StartCoroutine(DamageAnimation());
			break;
		case 1:
			iTween.ScaleTo(sprite.gameObject, iTween.Hash("x", 522f, "y", 522f, "time", 1f, "easetype", iTween.EaseType.linear));
			Sound.Instance.playSe(Sound.eSe.SE_331_searchbubble01);
			yield return StartCoroutine(DamageAnimation());
			break;
		case 0:
			Dead();
			break;
		}
	}

	private IEnumerator DamageAnimation()
	{
		while (Sound.Instance.isPlayingSe(Sound.eSe.SE_331_searchbubble01))
		{
			yield return null;
		}
		CharaObj_.transform.localPosition = new Vector3(0f, -46f, 2f);
		CharaObj_.GetComponentInChildren<tk2dAnimatedSprite>().Play(CharaNames_[CharaNum_] + CharaAnimNo_[MorganaHP_, CharaNum_]);
	}

	public IEnumerator ChangeColor(int color)
	{
		eType setColor = (eType)(color + 109);
		ParentMorgana_.setType(setColor);
		ParentMorgana_.IsChangeColor_ = true;
		foreach (Bubble child in ParentMorgana_.ChildMorgana_)
		{
			child.IsChangeColor_ = true;
			child.setType(setColor);
		}
		yield break;
	}

	public IEnumerator SpecialBreak()
	{
		if (IsSpecialBreak_)
		{
			yield break;
		}
		Bubble pBubble = null;
		if (IsMorganaParent_)
		{
			pBubble = this;
		}
		else if (ParentMorgana_ != null)
		{
			pBubble = ParentMorgana_;
		}
		pBubble.IsSpecialBreak_ = true;
		foreach (Bubble child in pBubble.ChildMorgana_)
		{
			child.IsSpecialBreak_ = true;
		}
		pBubble.Dead();
	}

	private void Dead()
	{
		Debug.Log("morgana is dead");
		Sound.Instance.playSe(Sound.eSe.SE_219_daihakai);
		part.breakMorganaCount++;
		part.MorganaAnimation(CharaObj_);
		foreach (Bubble item in ChildMorgana_)
		{
			item.startBreak();
		}
		startBreak();
	}

	public bool IsRotateBubble()
	{
		return rotateState.fulcrum != null && rotateState.rad > 0 && rotateState.rad <= 2;
	}

	public bool IsRotateBubble(Bubble fulcrum)
	{
		return fulcrum != null && rotateState.fulcrum == fulcrum;
	}

	public void ResetRoteteFulcrum()
	{
		rotateState.init();
	}

	public void SetRoteteFulcrum(Bubble fulcrum)
	{
		if (fulcrum == null)
		{
			rotateState.init();
			return;
		}
		if (!isRotateType())
		{
			rotateState.init();
			return;
		}
		int rotateRad = GetRotateRad(fulcrum.myTrans.localPosition);
		if (!(rotateState.fulcrum != null) || rotateRad < rotateState.rad)
		{
			if (rotateRad > 2)
			{
				rotateState.init();
				return;
			}
			rotateState.fulcrum = fulcrum;
			rotateState.diff.x = Mathf.Abs(rotateState.fulcrum.myTrans.localPosition.x - myTrans.localPosition.x);
			rotateState.diff.y = Mathf.Abs(rotateState.fulcrum.myTrans.localPosition.y - myTrans.localPosition.y);
			rotateState.rad = rotateRad;
			rotateState.moveCnt = rotateRad;
			rotateState.isLeft = fulcrum.rotateState.isLeft;
		}
	}

	public int GetRotateRad(Vector3 fulcrumPos)
	{
		Vector2 zero = Vector2.zero;
		zero.x = Mathf.Abs(fulcrumPos.x - myTrans.localPosition.x);
		zero.y = Mathf.Abs(fulcrumPos.y - myTrans.localPosition.y);
		zero.x /= 60f;
		zero.y /= 52f;
		zero.x = (float)(int)(zero.x * 100f) * 0.01f;
		zero.y = (float)(int)(zero.y * 100f) * 0.01f;
		int num = Mathf.CeilToInt(zero.x);
		int num2 = Mathf.CeilToInt(zero.y);
		if (num + num2 == 0)
		{
			return 0;
		}
		if (zero.x > zero.y + 0.25f)
		{
			return num;
		}
		if (zero.x + 0.25f < zero.y)
		{
			return num2;
		}
		return num + 1;
	}

	public IEnumerator SetRotatePos()
	{
		if (!IsRotateBubble())
		{
			rotateState.init();
			yield break;
		}
		if (rotateState.moveCnt <= 0)
		{
			rotateState.init();
			yield break;
		}
		Vector3 moveVec = Vector3.zero;
		Vector3 fulcrumPos = rotateState.fulcrum.myTrans.localPosition;
		int def = 30 * rotateState.rad;
		while (rotateState.moveCnt > 0)
		{
			if (rotateState.isLeft)
			{
				if (Mathf.Abs(myTrans.localPosition.y - fulcrumPos.y) < 30f)
				{
					moveVec = (((int)myTrans.localPosition.x <= (int)fulcrumPos.x) ? offsetDownRight : offsetUpLeft);
				}
				else if (myTrans.localPosition.y > fulcrumPos.y)
				{
					moveVec = ((fulcrumPos.x - myTrans.localPosition.x >= (float)(def - 15)) ? offsetDownLeft : ((!(fulcrumPos.x - myTrans.localPosition.x < (float)(-def - 15))) ? offsetLeft : offsetUpLeft));
				}
				else if (myTrans.localPosition.y < fulcrumPos.y)
				{
					moveVec = ((fulcrumPos.x - myTrans.localPosition.x <= (float)(-def + 15)) ? offsetUpRight : ((!(fulcrumPos.x - myTrans.localPosition.x > (float)(def + 15))) ? offsetRight : offsetDownRight));
				}
			}
			else if (Mathf.Abs(myTrans.localPosition.y - fulcrumPos.y) < 30f)
			{
				moveVec = (((int)myTrans.localPosition.x <= (int)fulcrumPos.x) ? offsetUpRight : offsetDownLeft);
			}
			else if (myTrans.localPosition.y > fulcrumPos.y)
			{
				moveVec = ((fulcrumPos.x - myTrans.localPosition.x >= (float)(def + 15)) ? offsetUpRight : ((!(fulcrumPos.x - myTrans.localPosition.x < (float)(-def + 15))) ? offsetRight : offsetDownRight));
			}
			else if (myTrans.localPosition.y < fulcrumPos.y)
			{
				moveVec = ((fulcrumPos.x - myTrans.localPosition.x <= (float)(-def - 15)) ? offsetDownLeft : ((!(fulcrumPos.x - myTrans.localPosition.x > (float)(def - 15))) ? offsetLeft : offsetUpLeft));
			}
			ObstacleDefend ob = null;
			if (part.obstacleList != null && part.obstacleList.Count > 0)
			{
				for (int i = 0; i < part.obstacleList.Count; i++)
				{
					if (part.obstacleList[i].currentParentBubble == this)
					{
						ob = part.obstacleList[i];
						break;
					}
				}
			}
			Vector3 moveto = myTrans.localPosition + moveVec;
			float moveTime = 0.3f / (float)(rotateState.rad * rotateState.rad);
			iTween.MoveTo(base.gameObject, iTween.Hash("position", moveto, "easetype", rotateState.fulcrum.easeType, "time", moveTime, "islocal", true));
			while (GetComponent<iTween>() != null)
			{
				if (ob != null)
				{
					ob.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, ob.transform.localPosition.z);
				}
				yield return stagePause.sync();
			}
			base.gameObject.transform.localPosition = moveto;
			myTrans.localPosition = moveto;
			rotateState.moveCnt--;
			if (ob != null)
			{
				ob.transform.localPosition = new Vector3(moveto.x, moveto.y, ob.transform.localPosition.z);
			}
			Bubble sameBubble = GetSamePosBubble();
			bool bDrop = false;
			if (myTrans.localPosition.x < -1f)
			{
				bDrop = true;
			}
			else if (myTrans.localPosition.x >= 570f)
			{
				bDrop = true;
			}
			else if (sameBubble != null)
			{
				bDrop = true;
			}
			if (bDrop)
			{
				if ((bool)part)
				{
					yield return StartCoroutine(part.dropRotateBubble(this));
				}
				rotateState.moveCnt = 0;
				yield break;
			}
		}
		rotateState.init();
	}

	private bool isRotateType()
	{
		return isRotateType(this);
	}

	private bool isRotateType(Bubble b)
	{
		switch (b.type)
		{
		case eType.Fulcrum:
		case eType.RotateFulcrumR:
		case eType.RotateFulcrumL:
		case eType.FriendFulcrum:
		case eType.Blank:
			return false;
		default:
			return true;
		}
	}

	public Bubble GetSamePosBubble()
	{
		Bubble[] array = null;
		if (part != null)
		{
			array = part.fieldBubbleList.ToArray();
		}
		else if (bonusPart != null)
		{
			array = bonusPart.fieldBubbleList.ToArray();
		}
		else if (rankingPart != null)
		{
			array = rankingPart.fieldBubbleList.ToArray();
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (isSamePosBubble(array[i]) && array[i].rotateState.fulcrum == null)
			{
				return array[i];
			}
		}
		return null;
	}
}
