using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using TnkAd;
using Toast.Analytics;
using UnityEngine;

public class Part_Stage : PartBase
{
	private class ReplayData
	{
		public ObstacleDefend obstacle;

		public bool isObstacleActive;

		public Bubble.eType type;

		public Vector3 pos;

		public int lineFriendIndex;

		public bool isFrozen;

		public bool isOnChain;

		public bool isLocked;

		public int index;

		public int counterCount;

		public int unknownColor;

		public int chamelleonIndex;

		public int outType;

		public bool isWarpOutUse;

		public bool IsMorganaParent_;

		public Bubble ParentMorgana_;

		public List<Bubble> ChildMorgana_;

		public int MorganaHP_;

		public int CharNum = -1;

		public bool isContainSearchList;

		public int uniqueId;
	}

	private enum eFriendBonus
	{
		CountPlus = 0,
		Score = 1,
		Coin = 2,
		Jewel = 3,
		CountMinus = 4,
		Snake = 5,
		Max = 6
	}

	public enum eState
	{
		Start = 0,
		Wait = 1,
		Shot = 2,
		Search = 3,
		Scroll = 4,
		Next = 5,
		Exchange = 6,
		Clear = 7,
		Gameover = 8,
		Result = 9,
		End = 10
	}

	public enum eGameType
	{
		ShotCount = 0,
		Time = 1
	}

	public enum eGameover
	{
		ShotCountOver = 0,
		TimeOver = 1,
		HitSkull = 2,
		FriendToGrow = 3,
		ScoreNotEnough = 4,
		CounterOver = 5,
		MinilenVanish = 6,
		MinilenToGrow = 7
	}

	public enum eClear
	{
		Score = 1,
		AllDel = 2,
		Friend = 4,
		Fulcrum = 8,
		Minilen = 0x10
	}

	private enum eVoice
	{
		Invalid = -1,
		Bravo = 0,
		Fantastic = 1,
		Wow = 2,
		Yay = 3,
		Max = 4
	}

	private struct FriendBonus
	{
		public int bonusIndex;

		public int num;

		public int friendIndex;

		public FriendBonus(int b, int n, int f)
		{
			bonusIndex = b;
			num = n;
			friendIndex = f;
		}
	}

	private enum eLimit
	{
		Result = 1,
		LvUp = 2,
		EventReward = 4
	}

	private const int COMBO_DISP_COUNT = 2;

	private const int SNAKE_SHOW_MAX = 3;

	private const float CHANGE_COLOR_TIME = 10f;

	private const int CHANGE_COLOR_SHOT_COUNT = 5;

	private const string CEILING_BUBBLE_NAME = "99";

	private const int GLOSS_INTERVAL = 30;

	private const int SNAKE_ADD_COUNT = 3;

	private const int MAX_SNAKE_COUNT = 99;

	private const float DRAW_HIT_SIZE_PAR = 0.8f;

	private const float DRAW_OFFSET_SIZE = 10f;

	private const int DRAW_COUNT_MAX = 6;

	private const float CEILING_OFFSET_LOCALPOS = 20f;

	private const int TIME_STOP_COUNT_MAX = 10;

	private const int VACUUM_COUNT_MAX = 10;

	private const int OBSTACLE_ACTION_COUNT = 1;

	private const int TUNNEL_ACTION_COUNT = 1;

	private const int RANDOM_RESEARCH_LIMIT = 10;

	private const int HONEYCOMB_MAX = 3;

	private const float BEE_EFFECT_TIME = 1f;

	private const float BLACKHOLE_CROSS_FADE_TIME = 0.7f;

	private const float WATER_EFF_TIME_DEV = 400f;

	private const float WATER_EFF_SIZE_OFFSET = 20f;

	private const float DEF_BOBBLEN_X = -178f;

	private const float DEF_BOBBLEN_Z = -5f;

	private const float SHOT_BOBBLEN_X = -158f;

	private const float SHOT_BOBBLEN_Z = -4f;

	public const float ROT_TIME = 0.3f;

	public float uiScale;

	private StageInfo.CommonInfo stageInfo;

	private StageData stageData;

	private SaveOtherData otherData_;

	public StagePause stagePause;

	private int mComboCount;

	private Network.Avatar avatar = GlobalData.Instance.currentAvatar;

	private List<Constant.Skill> skill_ = new List<Constant.Skill>();

	private bool bComboMaster;

	public bool bGuideStretch;

	private bool bCreater;

	private bool bFriendIncidenceUp;

	private bool bShotPlus;

	private bool bCoinUp;

	private bool bCueBubbleChange;

	public bool bBasicSkill;

	private int ComboMasterCount;

	public float GuideStretchMultiple = 1f;

	private int CreaterExTurn = 1;

	private float IncidenceUp = 1f;

	private int shotPlusNum;

	private float CoinUpRate = 1f;

	private int CueBubbleChangeRestNum;

	private int[] SKILL_SCORE_LIST;

	private int[] SKILL_SCORE_LIST2;

	private int SKILL_SCORE;

	private Transform skillButton;

	private GameObject skillNgIcon_;

	public int CreaterTurnCount;

	public int CreaterStockNum;

	private bool isCreaterSkillUse;

	private GameObject CreaterObj;

	private int skillBonusCoin;

	private GameObject cueBubbleChangeObj;

	private Bubble cueBubble;

	private bool isCueBubbleChangeUse;

	public int basicSkillBreakNum;

	private bool bCreaterSkillEffect;

	private bool isHoneyCreater;

	private UILabel CreaterTurnCountDownLabel_;

	private UISlider CreaterSkillSliderBar_;

	private UILabel CreaterStockLabel_;

	private Bubble.eType createrBubbleType = Bubble.eType.Invalid;

	private bool replaySkillBomShot;

	private int replayBombStockNum;

	private int replayBombTurnCount;

	private bool replayIsCueBubbleChange;

	private Bubble.eType replayCueBubbleChangeType;

	private bool replayCueBubbleActive;

	private int replayComboMasterCount;

	private int nContinueCount;

	private GameObject comboObject;

	private TweenScale comboIn;

	private TweenScale comboOut;

	private UILabel comboLabel;

	private Animation comboAnim;

	private int maxComboCount;

	private int BREAK_SCORE = 10;

	private int DROP_SCORE = 50;

	private int COMBO_SCORE = 100;

	private int REFLECT_SCORE = 50;

	private int RESCUE_SCORE = 500;

	private int MINILEN_SCORE = 500;

	private int CLEAR_SCORE_1_5 = 50;

	private int CLEAR_SCORE_6_10 = 100;

	private int CLEAR_SCORE_11_15 = 250;

	private int CLEAR_SCORE_16 = 300;

	private int mTotalScore;

	private int MORGANA_SCORE = 100;

	private int COIN_ADD_VALUE = 50;

	public int mTotalCoin;

	public int totalEventCoin;

	public int nextCorrection = 5;

	public int nextLimitCorrection = 50;

	private GameObject bubbleObject;

	private GameObject chainBubbleObject;

	public Transform bubbleRoot;

	private Transform nextBubbleRoot;

	private Vector3 bubbleRootPos = new Vector3(-270f, 455f, 0f);

	private List<Transform> ceilingBubbleList = new List<Transform>(10);

	private List<ReplayData> replayDataList = new List<ReplayData>(256);

	private Dictionary<int, List<Bubble.eType>> replayChainTypeDic = new Dictionary<int, List<Bubble.eType>>();

	private Dictionary<int, List<Vector3>> replayChainPosDic = new Dictionary<int, List<Vector3>>();

	private List<Bubble.eType> replayNextTypeList = new List<Bubble.eType>(3);

	private Constant.Item.eType replayUseItem = Constant.Item.eType.Invalid;

	private int replayScore;

	private int replayCoin;

	private int replayShotCount;

	private float tempReplayDiffTime;

	private float replayDiffTime;

	private int replayComboCount;

	private int replayBonusJewel;

	private int replayBonusCoin;

	private int replaySnakeCount;

	private int replayTimeStopCount;

	private int replayVacuumCount;

	private int replayKeyCount;

	private int replayObstacleCount;

	private List<Cloud.CloudAreaState> replayCloudArea = new List<Cloud.CloudAreaState>();

	private List<Bubble.eType> replaySearchedBubbleTypeList = new List<Bubble.eType>();

	private Transform launchpad;

	private List<Transform> scoreList = new List<Transform>(8);

	private Transform scoreRoot;

	private GameObject[] nextBubbles = new GameObject[3];

	private Transform[] nextBubblePoses = new Transform[3];

	private Animation stepNextBubbleAnim;

	private string stepNextBubbleClipName = "Next_bubble_00_anm";

	private int nextBubbleCount = 2;

	public List<Bubble> fieldBubbleList = new List<Bubble>(256);

	private int fieldObjectCount;

	private int fieldFriendCount;

	private int fieldItemCount;

	public List<Bubble> fulcrumList = new List<Bubble>();

	public List<Bubble> noDropList = new List<Bubble>();

	public List<Bubble> growBubbleList = new List<Bubble>();

	public List<Bubble> growCandidateList = new List<Bubble>();

	public List<Bubble> chameleonBubbleList = new List<Bubble>();

	public Dictionary<int, List<ChainBubble>> chainBubbleDic = new Dictionary<int, List<ChainBubble>>();

	private List<Bubble.eType> searchedBubbleTypeList = new List<Bubble.eType>(32);

	private List<Bubble> searchedBubbleList = new List<Bubble>();

	private List<Bubble.eType> searchedPreBubbleTypeList = new List<Bubble.eType>(32);

	private BubbleNavi bubbleNavi;

	private List<Bubble> MorganaList_ = new List<Bubble>();

	private List<GameObject> ReplayDesList_ = new List<GameObject>();

	public int breakMorganaCount;

	private int normalBubbleCount;

	private List<Bubble> lineFriendCandidateList = new List<Bubble>();

	public GameObject lineFriendBase;

	private Texture defaultUserIconTexture;

	private bool isGotJewel;

	private GameObject[] bubbleBonusBases = new GameObject[6];

	private int mBonusCoin;

	private int mBonusJewel;

	private int remainingBonus;

	private Transform stageBg;

	private Transform stageUi;

	private Transform frontUi;

	private Transform scrollUi;

	public GameObject boundEffL;

	public GameObject boundEffR;

	private GameObject bubble_08_eff;

	private GameObject bubble_17_eff;

	private GameObject bubble_17_eff_l;

	private GameObject bubble_17_eff_r;

	public GameObject honeycomb_eff;

	public GameObject[] snake_eff;

	private GameObject counter_eff;

	private Animation counter_eff_anm;

	private System.Random random = new System.Random();

	private GameObject clear_condition_stamp_00;

	private GameObject clear_condition_stamp_01;

	private eState mState;

	private eGameType gameType;

	private eGameover gameoverType;

	private bool isTimeOverClear;

	private eVoice prevVoice = eVoice.Invalid;

	public int shotCount;

	public float startTime;

	private int TIME_ADD_VALUE = 5;

	private int PLUS_ADD_VALUE = 3;

	private Color REST_COUNT_DANGER_COLOR = new Color(0.8235294f, 5f / 51f, 0f);

	private Color REST_COUNT_DEFAULT_COLOR;

	private int stageNo;

	private UILabel countLabel;

	private UISlider scoregauge;

	private float starRate1;

	private float starRate2;

	private GameObject[] stars;

	private UISpriteAnimation scoregauge_eff;

	private PopupScore[] popupScoreNormal = new PopupScore[2];

	private int popupScoreIndex;

	private PopupScore popupScoreDrop;

	private PopupScore popupScoreDrop_clear;

	private PopupScore popupScoreReflect;

	private PopupScore popupScoreRescue;

	private PopupScore popupScoreSkill;

	private Vector3 bonusBasePos = new Vector3(-285f, -120f, 0f);

	private Vector3 bonusClearPos = new Vector3(-285f, -70f, 0f);

	private GameObject[] popupCombo = new GameObject[2];

	private int popupComboIndex;

	private PopupExcellent popupExcellent;

	private bool bExcellent;

	private ReadyGo readyGo;

	private GameObject stageClear;

	public GameObject chacknBase;

	public GameObject minilenBase;

	public GameObject scoreParticleBase;

	public GameObject skullBase;

	private GameObject countdown_eff;

	private UITweener[] countdown_eff_tweeners;

	private GameObject countdown_bad_eff;

	private UITweener[] countdown_bad_eff_tweeners;

	private GameObject skullBarrier;

	private tk2dAnimatedSprite[] charaAnims;

	public string[] CHARA_SPRITE_ANIMATION_HEADER = new string[2] { "chara_00_", "chara_01_" };

	public string waitAnimName = "chara_00_08_02_0";

	private float glossTime;

	private Bubble.eType glossType;

	private List<Bubble.eType> glossColorList = new List<Bubble.eType>();

	private float cutinWaitTime = 3f;

	private GameObject bubble_trail_eff;

	private Vector3 bubble_trail_offset;

	private GameObject useitem_bg;

	private GameObject shootButton;

	private Arrow arrow;

	public Guide guide;

	private GameObject next_tap;

	private List<Constant.Item.eType> useItemList_ = new List<Constant.Item.eType>();

	private StageBoostItemParent itemParent_;

	private StageUseBoostItemParent useItemParent_;

	private StageBoostItem itemReplay_;

	private StageBoostItem itemChangeUp_;

	private StageBoostItem itemTimeStop_;

	private StageBoostItem itemVacuum_;

	private GameObject listItem_;

	private int bubblePlusNum;

	private int timePlusNum;

	private int scoreUpNum = 1;

	private GameObject growGameOver_;

	private bool bEventStage_;

	private int eventNo_;

	private bool bParkStage_;

	private bool bOPAnime_;

	public Material fadeMaterial;

	private Bubble.eType noSearchColor;

	private int stageEnd_ShotCount;

	private float stageEnd_Time;

	private int stageEnd_ContinueCount;

	private long[] helpDataList;

	private bool bShowedLastPoint;

	private List<Bubble.eType> clearChacknList = new List<Bubble.eType>();

	private bool isCampaign_;

	private float campaignRate_;

	private List<Bubble> nearCoinBubbleList_ = new List<Bubble>();

	private int ivyDepthOffset;

	private Material normalMat;

	private Material gameoverMat;

	private int snakeCount_;

	private float nextBubbleBobllenBefor_Y;

	private GameObject snakeCounter;

	private UILabel snakeCounterLabel;

	private Animation snakeCounterAnm;

	private GameObject bubble_19_eff;

	private GameObject bubble_20_eff;

	public GameObject counter_count;

	private GameObject drawLineBase_;

	private GameObject drawTouchPoint_;

	private GameObject drawCounter_;

	private GameObject drawHelp_;

	private GameObject drawWaterBase_;

	private UISprite drawCounteSprite_;

	private List<GameObject> drawLineList_ = new List<GameObject>();

	private List<Transform> drawLineBgList_ = new List<Transform>();

	private GameObject beebarrier_eff;

	private GameObject beebarrier_eff_00;

	private Animation bee_barrier_anm;

	private GameObject timestop_eff;

	private GameObject timestop_eff_00;

	private Animation time_stop_anm;

	private TimeStop timestop;

	private GameObject timestop_counter;

	private UILabel timestop_counter_label;

	public bool bUsingTimeStop;

	private int timeStopCount;

	private List<Bubble> honeycombBubbleList_ = new List<Bubble>();

	private List<Bubble> counterList_ = new List<Bubble>();

	private bool bUsingVacuum;

	private GameObject vacuum_eff;

	private GameObject vacuum_eff_00;

	private GameObject vacuum_eff_01;

	private Animation vacuum_anim;

	private GameObject vacuum_bob;

	private UISprite vacuum_bob_01;

	private GameObject vacuum_counter;

	private UILabel vacuum_counter_label;

	private int vacuumCount;

	private int chackunCount;

	private int obstacleCount;

	public List<ObstacleDefend> obstacleList;

	public List<ObstacleDefend> obstacleMoveList = new List<ObstacleDefend>();

	private int inObjectCount;

	private int outObjectCount;

	private bool outObjectExist;

	private List<GameObject> outObjectList;

	public List<GameObject> outObjectMoveList = new List<GameObject>();

	private int missionType = -1;

	private int lineFriendCount;

	private int lineFriendCountBuffer;

	private int useItemCount;

	private int chamelleonIndexCount;

	public bool bInitialized;

	public List<Vector2> cloudHitPosList_ = new List<Vector2>();

	private List<int> bubbleColorList = new List<int>();

	private List<Bubble> dropSnakeList = new List<Bubble>();

	private int minilen_count_all;

	private int minilen_count_current;

	private int minilen_count_pop;

	private int minilen_count_pop_scored;

	private List<UISprite> minilen_count_label;

	private UISprite leader_minilen;

	private List<Bubble.eType> clearMinilenList = new List<Bubble.eType>();

	public StageMinilen _lost_minilen;

	private int _prak_minilen_drop_id;

	private int _prak_minilen_drop_unique_id;

	private List<int> _minilen_bubble_unique_ids = new List<int>();

	private List<int> _droped_minilen_drops_indexes = new List<int>();

	public int old_linghtningGNum;

	public List<Bubble> _droped_lightning_g = new List<Bubble>();

	private KeyBubbleData keyData;

	private int getKeyBubble;

	private GameObject keyIcon;

	private KeyBubbleInfo keyBubbleInfo;

	private int[] keyBubbleTimingValue = new int[2];

	private GameObject keyAnim;

	public int getKeyCount;

	private bool bGetKey;

	private GameObject countdownContinue;

	private float myBubblePower = 1f;

	private Bubble.eType itemCancelBubbleType = Bubble.eType.Invalid;

	private Bubble.eType itemCancelBubbleTypeOld;

	private GameObject OP_event_00;

	private GameObject OP_event_01;

	private Animation opAnim_00;

	private Animation opAnim_01;

	private Animation bgAnim_;

	private GameObject[] charaObjs;

	private Transform launchpad_;

	private int prevShotBubbleIndex;

	private Bubble.eType prevShotBubbleType;

	private Vector3 random_vec;

	private bool bPlus;

	private bool bMinus;

	private int rainbowChainCount;

	private bool isSkullGameOver;

	private bool isHitHoneycom;

	private bool isHitBlackHole;

	private bool isCounterGameOver;

	private bool bSearch;

	private Coroutine chainBreakRoutine;

	private Coroutine lightningEffectRoutine;

	private Coroutine fireEffectRoutine;

	private List<FriendBonus> friendBonusList = new List<FriendBonus>();

	private Vector3 shotBubblePos;

	private List<Bubble> frozenBreakList = new List<Bubble>();

	public List<Chackn> chacknList = new List<Chackn>();

	public List<StageMinilen> minilenList = new List<StageMinilen>();

	public List<Cloud> cloudList = new List<Cloud>();

	public float moveSpan;

	private int honeycomb_num = -1;

	private bool isHoneycombHitWait;

	private GameObject hitHoneycombEff;

	private bool isHitChameleon;

	private List<Bubble> changeChamList = new List<Bubble>();

	private List<Bubble> checkBreakChamList = new List<Bubble>();

	private int chainedChameleonCount;

	private UILabel lineFriendLabel_;

	private UISysFontLabel lineFriendSysLabel_;

	private bool bMetalShooting;

	private bool bDrawing;

	private float drawDiffTime;

	private bool bWaterCancel;

	private List<Bubble> drawPointsBubble = new List<Bubble>();

	private List<GameObject> WaterLineList_ = new List<GameObject>();

	private GameObject shineEffectInstance_;

	private int totalDropCount;

	private int totalSkillBonusCount;

	public int inCloudCnt;

	private int breakLightningCount_;

	public bool isSearching;

	public bool isMoving;

	private Dictionary<int, BubbleBase> searchDict = new Dictionary<int, BubbleBase>();

	private List<KeyValuePair<BubbleBase, float>> ceilingList;

	private float ceilingBaseY;

	private Bubble upLeftBubble;

	private Bubble downRightBubble;

	private int searchCount;

	private Dictionary<Bubble.eType, int> prevCreateBubble = new Dictionary<Bubble.eType, int>();

	private Coroutine tayunCoroutine;

	private bool bCrisis;

	private int dispScore = 1;

	private int tempScore;

	public int nextScore;

	private System.Random rand = new System.Random();

	private float countAnimDiff;

	private bool isCountAnim;

	private bool isTimeStageReplay;

	private bool bReplaying;

	private List<Ivy> ivyList = new List<Ivy>();

	private int realLinkShotCount;

	private Dictionary<int, int> realLinkDic = new Dictionary<int, int>();

	private RealLinkInfo realLinkInfo;

	private int realLinkTimingValue;

	private UILabel chacknNumLabel;

	private bool bMoveCreaterEffect;

	private float moveCreaterDiffTime;

	private int countUpEffectCount;

	private int prevValue;

	private bool bLastPoint;

	public bool lastPoint_chackn;

	private List<Bubble> rightBubbleList = new List<Bubble>();

	private List<Bubble> leftBubbleList = new List<Bubble>();

	private bool snakeAliveAnimation;

	private List<Bubble> enableCounterList = new List<Bubble>();

	private List<Bubble> countOverBubbleList_ = new List<Bubble>();

	private bool bPlayingBeeBarrierEffect;

	private float beeBarrierDiffTime;

	private bool bPlayingTimeStopEffect;

	private float timeStopDiffTime;

	private bool itemTimeStopUseCheckEnable;

	private bool bTimeStopEffectEnded;

	private bool itemVacummeUseCheckEnable;

	private bool bVacummeEffectEnded;

	private List<Bubble.eType> chamelleonPickUpTypeList = new List<Bubble.eType>();

	private List<Bubble.eType> chamelleonPrePickUpTypeList = new List<Bubble.eType>();

	private Dictionary<int, int> chameleonChangeDic = new Dictionary<int, int>();

	private string[] charaNames;

	public int rakka = 500;

	private List<Vector3> _minilen_firework_poss = new List<Vector3>();

	private bool _minilen_fireworks_running;

	private ResponceHeaderData headerData_;

	private bool bLvUp_;

	private bool bClear_;

	private bool bProgressOpen;

	private DialogResultBase.eBtn resultBtn_;

	private int prevScore_;

	private int nowScore_;

	private int bonusScore_;

	private EventStageInfo.Info eventInfo_;

	private int[] limitFlgs_ = new int[5];

	private Constant.Reward[] lvupRewards_;

	private Constant.Reward[] resultRewards_ = new Constant.Reward[5];

	private StageResult resultData_;

	private bool bGetStarJewel_;

	private int comboBonusCoin;

	private int campaignCoin;

	private bool bBossOpen;

	private bool bGetCollaboReward;

	public int[] rewards = new int[3];

	public int[] rewardnum = new int[3];

	private int _got_rare_minilen = -1;

	private int _got_rare_minilen_level = -1;

	private bool bEventFinish;

	private List<Bubble> rotateFulcrumList = new List<Bubble>();

	private List<Bubble> dropRotateList = new List<Bubble>();

	private List<Bubble> rotateList = new List<Bubble>();

	private int comboCount
	{
		get
		{
			return mComboCount;
		}
		set
		{
			mComboCount = Mathf.Min(value, Constant.ComboMax);
			if (mComboCount > maxComboCount)
			{
				maxComboCount = mComboCount;
			}
		}
	}

	public int totalScore
	{
		get
		{
			return mTotalScore;
		}
		set
		{
			mTotalScore = Mathf.Min(value, Constant.UserScoreMax);
		}
	}

	public int totalCoin
	{
		get
		{
			return mTotalCoin;
		}
		set
		{
			mTotalCoin = Mathf.Min(value, Constant.CoinMax);
		}
	}

	public int bonusCoin
	{
		get
		{
			return mBonusCoin;
		}
		set
		{
			mBonusCoin = Mathf.Min(value, Constant.CoinMax);
		}
	}

	public int bonusJewel
	{
		get
		{
			return mBonusJewel;
		}
		set
		{
			mBonusJewel = Mathf.Min(value, Constant.JewelMax);
		}
	}

	public eState state
	{
		get
		{
			return mState;
		}
		private set
		{
			mState = value;
			if (shootButton != null && shootButton.activeSelf)
			{
				shootButton.GetComponent<UIButton>().isEnabled = mState == eState.Wait;
			}
			if (itemParent_ != null)
			{
				if (mState == eState.Wait)
				{
					itemParent_.enable();
				}
				else
				{
					itemParent_.disable();
				}
			}
		}
	}

	public bool isParkStage
	{
		get
		{
			return bParkStage_;
		}
	}

	public int minilenCountCurrent
	{
		get
		{
			return minilen_count_current;
		}
	}

	public void ChangeMyBubblePower(float diff)
	{
		myBubblePower += diff;
		if (myBubblePower > 2f)
		{
			myBubblePower = 2f;
		}
		else if (myBubblePower < 0.5f)
		{
			myBubblePower = 0.5f;
		}
		GameObject[] array = nextBubbles;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null && gameObject.activeSelf)
			{
				gameObject.GetComponent<Bubble>().changeBubblePower(myBubblePower);
			}
		}
	}

	private void Start()
	{
		isCampaign_ = GlobalData.Instance.getGameData().isCoinupCampaign;
		campaignRate_ = (float)GlobalData.Instance.getGameData().coinup * 0.01f;
		state = eState.Start;
		bBasicSkill = GlobalData.Instance.isBasicSkill;
		SKILL_SCORE_LIST = Constant.SKILL_SCORE_LIST;
		SKILL_SCORE_LIST2 = Constant.SKILL_SCORE_LIST2;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			StringBuilder sb = new StringBuilder();
			_droped_minilen_drops_indexes.ForEach(delegate(int d)
			{
				sb.Append(", " + d);
			});
			Debug.Log("Droped Minilen Count = " + minilen_count_current + "\nIndexes Length = " + _droped_minilen_drops_indexes.Count + "\nIndexes = " + sb.ToString());
		}
		if (state < eState.Wait || state >= eState.Result)
		{
			return;
		}
		updateTotalScoreDisp();
		if (state >= eState.Clear)
		{
			return;
		}
		updateRestCountDisp();
		if (!stagePause.pause)
		{
			stageEnd_Time += Time.deltaTime;
		}
		if (gameType == eGameType.Time && !bShowedLastPoint && Time.time - startTime >= (float)(stageInfo.Time - 10) && !stagePause.pause && !bReplaying)
		{
			StartCoroutine(lastPoint());
		}
		if (gameType == eGameType.Time && Time.time - startTime >= (float)stageInfo.Time && state == eState.Wait && !stagePause.pause && !bReplaying)
		{
			if (isTimeOverClear)
			{
				StartCoroutine(clearRoutine());
			}
			else
			{
				gameoverType = eGameover.TimeOver;
				StartCoroutine(gameoverRoutine());
			}
		}
		if (glossTime < Time.time)
		{
			fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
			{
				fieldBubble.startGloss(glossType);
			});
			updateGlossTime();
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!(partManager == null) && !partManager.isTransitioning() && state < eState.Clear)
		{
			if (Sound.Instance.isPlayingSe(Sound.eSe.SE_503_bee))
			{
				Sound.Instance.stopSe(Sound.eSe.SE_503_bee);
			}
			Ivy.setIvySe(false);
			if (bDrawing)
			{
				bWaterCancel = true;
			}
			if (Sound.Instance.isPlayingSe(Sound.eSe.SE_530_vacuum_move))
			{
				Sound.Instance.stopSe(Sound.eSe.SE_530_vacuum_move);
			}
			if (pauseStatus && !stagePause.pause)
			{
				stagePause.pause = true;
				DialogPause dialogPause = (DialogPause)dialogManager.getDialog(DialogManager.eDialog.Pause);
				dialogPause.stagePause = stagePause;
				dialogPause.stageNo = stageNo;
				dialogPause.setup();
				partManager.StartCoroutine(dialogManager.openDialog(dialogPause));
			}
		}
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		if (trigger.name.Contains("sell_boost"))
		{
			Constant.SoundUtil.PlayButtonSE();
			if (!stagePause.pause)
			{
				StartCoroutine(pressBoostItem(trigger.GetComponent<StageBoostItem>()));
			}
			yield break;
		}
		if (trigger.name.Contains("chara_01"))
		{
			if (state == eState.Wait && !stagePause.pause)
			{
				Sound.Instance.playSe(Sound.eSe.SE_207_koukan);
				StartCoroutine(exchangeBobblenRoutine());
			}
			yield break;
		}
		switch (trigger.name)
		{
		case "Gamestop_Button":
			if (drawLineList_.Count < 2)
			{
				if (timestop != null)
				{
					timestop.stopNegaRoutine();
				}
				if (state != eState.Clear && !stagePause.pause)
				{
					Constant.SoundUtil.PlayButtonSE();
				}
				OnApplicationPause(true);
			}
			break;
		case "next_tap":
			if (state == eState.Wait && !stagePause.pause)
			{
				Sound.Instance.playSe(Sound.eSe.SE_207_koukan);
				StartCoroutine(exchangeRoutine());
			}
			break;
		case "shoot_button":
			Constant.SoundUtil.PlayDecideSE();
			shoot(arrow.fireVector);
			break;
		case "skill_button":
		case "skill_button02":
			StartCoroutine(skillButtonTap());
			break;
		}
	}

	private void CueBubbleChangeExecute()
	{
		foreach (Constant.Item.eType item2 in useItemList_)
		{
			if (!Constant.Item.IsAutoUse(item2))
			{
				StageBoostItem item = itemParent_.getItem(item2);
				item.back();
				removeUseItem(item.getItemType(), item.getNum());
				break;
			}
		}
		int num = 0;
		num = ((snakeCount_ > 0) ? 1 : 0);
		Bubble component = nextBubbles[num].GetComponent<Bubble>();
		setNextTap(false);
		if (snakeCount_ > 0)
		{
			setNextTapBobblen(false);
		}
		Bubble.eType type = component.type;
		component.setType(cueBubble.type);
		cueBubble.setType(type);
		cueBubbleChangeObj.SetActive(false);
		isCueBubbleChangeUse = true;
		CueBubbleChangeRestNum--;
		SkillButtonSetting(CueBubbleChangeRestNum);
		bubbleNavi.startNavi(searchedBubbleList, component.type);
	}

	private void CueBubbleChangeCancel()
	{
		int num = 0;
		num = ((snakeCount_ > 0) ? 1 : 0);
		Bubble component = nextBubbles[num].GetComponent<Bubble>();
		if (gameType != 0 || stageInfo.Move - shotCount > 1)
		{
			setNextTap(true);
		}
		if (num != 0)
		{
			updateChangeBubbleBobblen();
		}
		cueBubbleChangeObj.SetActive(true);
		skillNgIcon_.SetActive(false);
		Bubble.eType type = component.type;
		component.setType(cueBubble.type);
		cueBubble.setType(type);
		isCueBubbleChangeUse = false;
		CueBubbleChangeRestNum++;
		SkillButtonSetting(CueBubbleChangeRestNum);
		bubbleNavi.startNavi(searchedBubbleList, component.type);
	}

	private void CueBubbleCreate()
	{
		int num = 0;
		num = ((snakeCount_ > 0) ? 1 : 0);
		List<Bubble.eType> list = new List<Bubble.eType>();
		for (int i = num; i < nextBubbleCount; i++)
		{
			if (nextBubbles[i] == null)
			{
				return;
			}
			if (nextBubbles[i].activeInHierarchy)
			{
				list.Add(nextBubbles[i].GetComponent<Bubble>().type);
			}
		}
		foreach (Constant.Item.eType item in useItemList_)
		{
			if (!Constant.Item.IsAutoUse(item))
			{
				list.Add(itemCancelBubbleType);
			}
		}
		foreach (Bubble.eType searchedBubbleType in searchedBubbleTypeList)
		{
			if (CueBubbleChangeRestNum == 0)
			{
				break;
			}
			if (list.IndexOf(searchedBubbleType) == -1)
			{
				cueBubble.setType(searchedBubbleType);
				cueBubbleChangeObj.SetActive(true);
				skillNgIcon_.SetActive(false);
				setSkillButtonActive(true);
				return;
			}
		}
		cueBubbleChangeObj.SetActive(false);
		skillNgIcon_.SetActive(true);
		setSkillButtonActive(false);
	}

	private IEnumerator pressBoostItem(StageBoostItem item)
	{
		Constant.Item.eType itemType = item.getItemType();
		bool isSetItemUse = false;
		if (item.isBuy())
		{
			if (!Constant.Item.IsAutoUse(itemType))
			{
				if (itemType == Constant.Item.eType.BubbleSearch)
				{
					useItem(itemType, item.getNum());
					yield break;
				}
				foreach (Constant.Item.eType type in useItemList_)
				{
					if (type != itemType && !Constant.Item.IsAutoUse(type))
					{
						StageBoostItem cancelItem = itemParent_.getItem(type);
						cancelItem.back();
						removeUseItem(cancelItem.getItemType(), cancelItem.getNum());
						break;
					}
				}
				if (!isUsedItem(itemType))
				{
					if (state == eState.Wait)
					{
						item.use();
						useItem(itemType, item.getNum());
					}
				}
				else if (state == eState.Wait)
				{
					item.back();
					removeUseItem(itemType, item.getNum());
				}
				yield break;
			}
			if (itemType == Constant.Item.eType.TimeStop || itemType == Constant.Item.eType.Vacuum)
			{
				isSetItemUse = true;
			}
		}
		if (gameType == eGameType.Time && (float)stageInfo.Time - (Time.time - startTime) < 0.5f)
		{
			yield break;
		}
		stagePause.pause = true;
		if (!guide.isShootButton)
		{
			guide.setActive(false);
		}
		DialogStageShop dialog = dialogManager.getDialog(DialogManager.eDialog.StageShop) as DialogStageShop;
		yield return StartCoroutine(dialog.show(stageNo, item, stageInfo, isSetItemUse));
		Ivy.setIvySe(false);
		while (dialog.isOpen())
		{
			yield return 0;
		}
		stagePause.pause = false;
		if (!item.isBuy())
		{
			yield break;
		}
		if ((item.getItemType() == Constant.Item.eType.TimeStop || item.getItemType() == Constant.Item.eType.Vacuum) && item.isSetItemFirst)
		{
			if (!item.bUse)
			{
				yield break;
			}
			useItem(itemType, item.getNum());
			item.isSetItemFirst = false;
		}
		else
		{
			useBuyItem(item, itemType);
		}
		state = eState.Shot;
		switch (itemType)
		{
		case Constant.Item.eType.BeeBarrier:
			yield return StartCoroutine(beeBarrierEffect());
			break;
		case Constant.Item.eType.TimeStop:
			yield return StartCoroutine(timeStopEffect());
			break;
		case Constant.Item.eType.Vacuum:
			yield return StartCoroutine(vacuumEffect());
			break;
		}
		state = eState.Wait;
	}

	public void useBuyItem(StageBoostItem item, Constant.Item.eType itemType)
	{
		useItemCount++;
		if (Constant.Item.IsAutoUse(itemType))
		{
			useItem(itemType, item.getNum());
			if (itemType == Constant.Item.eType.Replay)
			{
				item.disable();
				removeUseItem(itemType, item.getNum());
			}
		}
		else
		{
			if (itemType == Constant.Item.eType.BubbleSearch)
			{
				return;
			}
			foreach (Constant.Item.eType item3 in useItemList_)
			{
				if (!Constant.Item.IsAutoUse(item3))
				{
					StageBoostItem item2 = itemParent_.getItem(item3);
					item2.back();
					removeUseItem(item3, item2.getNum());
					break;
				}
			}
			item.use();
			useItem(itemType, item.getNum());
		}
	}

	public bool isUsedItem(Constant.Item.eType itemType)
	{
		foreach (Constant.Item.eType item in useItemList_)
		{
			if (item == itemType)
			{
				return true;
			}
		}
		return false;
	}

	private void useItem(Constant.Item.eType item, int num)
	{
		if (item == Constant.Item.eType.BubbleSearch)
		{
			StartCoroutine(searchBubbleRoutine());
			return;
		}
		useItemList_.Add(item);
		if (Constant.Item.IsAutoUse(item))
		{
			if (item != Constant.Item.eType.Replay)
			{
				useitem_bg.SetActive(true);
				useItemParent_.setActive(item, num);
			}
			switch (item)
			{
			case Constant.Item.eType.BubblePlus:
				bubblePlusNum = num;
				break;
			case Constant.Item.eType.TimePlus:
				timePlusNum = num;
				break;
			case Constant.Item.eType.ScoreUp:
				scoreUpNum = num;
				break;
			case Constant.Item.eType.ChangeUp:
				setNextBubbleCount(3);
				if (snakeCount_ > 0)
				{
					updateChangeBubbleBobblen();
				}
				if (bCueBubbleChange)
				{
					CueBubbleCreate();
				}
				break;
			case Constant.Item.eType.SuperGuide:
				if (guide.isShootButton)
				{
					guide.lineUpdate();
					guide.setActive(true);
				}
				break;
			case Constant.Item.eType.Replay:
				StartCoroutine(replayRoutine(false));
				break;
			case Constant.Item.eType.BeeBarrier:
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag("Bee");
				GameObject[] array2 = array;
				foreach (GameObject obj in array2)
				{
					UnityEngine.Object.Destroy(obj);
				}
				break;
			}
			case Constant.Item.eType.TimeStop:
				bUsingTimeStop = true;
				timeStopCount = 10;
				replayTimeStopCount = timeStopCount;
				break;
			case Constant.Item.eType.Vacuum:
				bUsingVacuum = true;
				vacuumCount = 10;
				replayVacuumCount = vacuumCount;
				break;
			case Constant.Item.eType.MinusGuard:
			case Constant.Item.eType.SkullBarrier:
			case Constant.Item.eType.MetalBubble:
			case Constant.Item.eType.IceBubble:
			case Constant.Item.eType.FireBubble:
			case Constant.Item.eType.BubbleSearch:
			case Constant.Item.eType.WaterBubble:
			case Constant.Item.eType.ShineBubble:
				break;
			}
		}
		else
		{
			if (isCreaterSkillUse && bCreater)
			{
				CreaterSkillCancel();
			}
			if (isCueBubbleChangeUse && bCueBubbleChange)
			{
				CueBubbleChangeCancel();
			}
			Bubble bubble = ((snakeCount_ <= 0) ? nextBubbles[0].GetComponent<Bubble>() : nextBubbles[1].GetComponent<Bubble>());
			itemCancelBubbleTypeOld = itemCancelBubbleType;
			if (bubble.type <= Bubble.eType.Black)
			{
				itemCancelBubbleType = bubble.type;
			}
			switch (item)
			{
			case Constant.Item.eType.HyperBubble:
				bubble.setType(Bubble.eType.Hyper);
				break;
			case Constant.Item.eType.BombBubble:
				bubble.setType(Bubble.eType.Bomb);
				break;
			case Constant.Item.eType.ShakeBubble:
				bubble.setType(Bubble.eType.Shake);
				break;
			case Constant.Item.eType.MetalBubble:
				bubble.setType(Bubble.eType.Metal);
				break;
			case Constant.Item.eType.IceBubble:
				bubble.setType(Bubble.eType.Ice);
				break;
			case Constant.Item.eType.FireBubble:
				bubble.setType(Bubble.eType.Fire);
				break;
			case Constant.Item.eType.WaterBubble:
				bubble.setType(Bubble.eType.Water);
				break;
			case Constant.Item.eType.ShineBubble:
				bubble.setType(Bubble.eType.Shine);
				break;
			case Constant.Item.eType.LightningG:
				bubble.setType(Bubble.eType.LightningG_Item);
				break;
			}
			setNextTap(false);
			if (snakeCount_ > 0)
			{
				setNextTapBobblen(false);
			}
			bubbleNavi.startNavi(searchedBubbleList, bubble.type);
		}
	}

	private IEnumerator skillButtonTap()
	{
		Debug.Log("skillButtonTap");
		if (state != eState.Wait)
		{
			yield break;
		}
		if (bCreater)
		{
			Constant.SoundUtil.PlayButtonSE();
			if (isCreaterSkillUse)
			{
				CreaterSkillCancel();
			}
			else if (0 < CreaterStockNum)
			{
				CreaterSkillExecute();
			}
		}
		else if (bCueBubbleChange)
		{
			Constant.SoundUtil.PlayButtonSE();
			if (isCueBubbleChangeUse)
			{
				Debug.Log("チェンジ回数残り" + CueBubbleChangeRestNum + "＞＞＞" + (CueBubbleChangeRestNum + 1));
				CueBubbleChangeCancel();
			}
			else if (0 < CueBubbleChangeRestNum)
			{
				Debug.Log("チェンジ回数残り" + CueBubbleChangeRestNum + "＞＞＞" + (CueBubbleChangeRestNum - 1));
				CueBubbleChangeExecute();
			}
		}
		yield return null;
	}

	private void CreaterSkillCancel()
	{
		isCreaterSkillUse = false;
		CreaterStockNum++;
		Debug.Log("++ストック数" + CreaterStockNum);
		int charaIndex = arrow.charaIndex;
		if (nextBubbles[charaIndex] != null)
		{
			Bubble component = nextBubbles[charaIndex].GetComponent<Bubble>();
			if (component.type > Bubble.eType.Black)
			{
				component.setType(itemCancelBubbleType);
			}
		}
		if (gameType != 0 || stageInfo.Move - shotCount > 1)
		{
			setNextTap(true);
		}
		if (charaIndex != 0)
		{
			updateChangeBubbleBobblen();
		}
		bubbleNavi.startNavi(searchedBubbleList, nextBubbles[charaIndex].GetComponent<Bubble>().type);
		SkillButtonSetting(CreaterStockNum);
	}

	private void CreaterSkillExecute()
	{
		isCreaterSkillUse = true;
		CreaterStockNum--;
		Debug.Log("--ストック数" + CreaterStockNum);
		Bubble bubble = ((snakeCount_ <= 0) ? nextBubbles[0].GetComponent<Bubble>() : nextBubbles[1].GetComponent<Bubble>());
		foreach (Constant.Item.eType item2 in useItemList_)
		{
			if (!Constant.Item.IsAutoUse(item2))
			{
				StageBoostItem item = itemParent_.getItem(item2);
				item.back();
				removeUseItem(item.getItemType(), item.getNum());
				break;
			}
		}
		itemCancelBubbleTypeOld = itemCancelBubbleType;
		if (bubble.type <= Bubble.eType.Black)
		{
			itemCancelBubbleType = bubble.type;
		}
		if (shotCount == 0)
		{
			itemCancelBubbleTypeOld = itemCancelBubbleType;
		}
		bubble.setType(createrBubbleType);
		setNextTap(false);
		if (snakeCount_ > 0)
		{
			setNextTapBobblen(false);
		}
		bubbleNavi.startNavi(searchedBubbleList, bubble.type);
		SkillButtonSetting(CreaterStockNum);
	}

	private void removeUseItem(Constant.Item.eType item, int num)
	{
		useItemList_.Remove(item);
		if (item == Constant.Item.eType.Replay)
		{
			return;
		}
		if (Constant.Item.IsAutoUse(item))
		{
			useItemParent_.setDeactive(item, num);
		}
		else
		{
			if (state != eState.Wait)
			{
				return;
			}
			int charaIndex = arrow.charaIndex;
			if (nextBubbles[charaIndex] != null)
			{
				Bubble component = nextBubbles[charaIndex].GetComponent<Bubble>();
				if (component.type > Bubble.eType.Black)
				{
					component.setType(itemCancelBubbleType);
				}
			}
			if (gameType != 0 || stageInfo.Move - shotCount > 1)
			{
				setNextTap(true);
			}
			if (charaIndex != 0)
			{
				updateChangeBubbleBobblen();
			}
			bubbleNavi.startNavi(searchedBubbleList, nextBubbles[charaIndex].GetComponent<Bubble>().type);
		}
	}

	private void updateBuyItem()
	{
		for (int num = useItemList_.Count - 1; num >= 0; num--)
		{
			Constant.Item.eType eType = useItemList_[num];
			StageBoostItem item = itemParent_.getItem(eType);
			if (!Constant.Item.IsAutoUse(eType))
			{
				removeUseItem(eType, item.getNum());
				if (item.getNum() <= 0)
				{
					item.reset();
				}
			}
		}
	}

	public override IEnumerator setup(Hashtable args)
	{
		nContinueCount = 0;
		stageNo = 1;
		if (args.ContainsKey("StageNo"))
		{
			stageNo = int.Parse(args["StageNo"].ToString());
		}
		bParkStage_ = Constant.ParkStage.isParkStage(stageNo);
		Network.MinilenData current_minilen = null;
		if (bParkStage_)
		{
			current_minilen = Bridge.MinilenData.getCurrent();
		}
		if (bParkStage_)
		{
			avatar = GlobalData.Instance.defaultAvatar();
		}
		else
		{
			if (GlobalData.Instance.getGameData().avatarList != null)
			{
				Network.Avatar[] avatarList = GlobalData.Instance.getGameData().avatarList;
				foreach (Network.Avatar av in avatarList)
				{
					if (av.wearFlg == 1)
					{
						GlobalData.Instance.currentAvatar = av;
						break;
					}
				}
			}
			if (GlobalData.Instance.currentAvatar == null)
			{
				GlobalData.Instance.currentAvatar = GlobalData.Instance.defaultAvatar();
			}
			avatar = GlobalData.Instance.currentAvatar;
		}
		ceilingBaseY = Mathf.Ceil(Mathf.Round(NGUIUtilScalableUIRoot.GetOffsetY(true).y) / 52f) * 52f;
		GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		StageDataTable stageTbl = dataTable.GetComponent<StageDataTable>();
		GlobalData.Instance.ignoreLodingIcon = true;
		int downloadCount = 1;
		NetworkMng.Instance.forceIconDisable(true);
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 4, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 5, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 6, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 8, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.ThrowChara, 0, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.SupportChara, 0, 0, downloadCount));
		int throwChara = avatar.throwCharacter;
		int supportChara = avatar.supportCharacter;
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.ThrowChara, 0 + throwChara, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.SupportChara, 0 + supportChara, 0, downloadCount));
		NetworkMng.Instance.forceIconDisable(false);
		GlobalData.Instance.ignoreLodingIcon = false;
		if (bParkStage_)
		{
			int minilen_level = 1;
			if (current_minilen != null && current_minilen.level > 0)
			{
				minilen_level = current_minilen.level;
			}
			SKILL_SCORE = SKILL_SCORE_LIST[minilen_level - 1];
		}
		else if (avatar != null)
		{
			if (avatar.baseSkill_3 >= 0)
			{
				SKILL_SCORE = SKILL_SCORE_LIST2[avatar.level - 1];
			}
			else
			{
				SKILL_SCORE = SKILL_SCORE_LIST[avatar.level - 1];
			}
		}
		else
		{
			SKILL_SCORE = 0;
		}
		Constant.Avatar.eSpecialSkill sp = Constant.Avatar.eSpecialSkill.None;
		GameObject dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		AvatarSkillDataTable skill_table = dataTbl.GetComponent<AvatarSkillDataTable>();
		if (bParkStage_)
		{
			Network.MinilenData minilen = Bridge.MinilenData.getCurrent();
			if (minilen != null)
			{
				minilen.level = Mathf.Max(minilen.level, 1);
				skill_table.getSkill(minilen.specialSkill, minilen.level, ref skill_);
			}
			else
			{
				skill_table.getSkill(0, 1, ref skill_);
			}
		}
		else if (avatar != null)
		{
			skill_table.getSkill(avatar.specialSkill, avatar.level, ref skill_);
		}
		else
		{
			skill_table.getSkill(0, 1, ref skill_);
		}
		foreach (Constant.Skill sk in skill_)
		{
			switch (sk.SkillType)
			{
			case Constant.Avatar.eSpecialSkill.ComboMaster:
				bComboMaster = true;
				ComboMasterCount = (int)sk.LevelEffect;
				break;
			case Constant.Avatar.eSpecialSkill.FriendIncidenceUp:
				bFriendIncidenceUp = true;
				IncidenceUp = sk.LevelEffect;
				break;
			case Constant.Avatar.eSpecialSkill.Bomb:
			case Constant.Avatar.eSpecialSkill.BombPlus:
			case Constant.Avatar.eSpecialSkill.MinorBombPlus:
			case Constant.Avatar.eSpecialSkill.BombPlusPlus:
				bCreater = true;
				CreaterExTurn = (int)sk.LevelEffect;
				createrBubbleType = Bubble.eType.Bomb;
				break;
			case Constant.Avatar.eSpecialSkill.MetalPlus:
			case Constant.Avatar.eSpecialSkill.Metal:
			case Constant.Avatar.eSpecialSkill.MetalPlusPlus:
				bCreater = true;
				CreaterExTurn = (int)sk.LevelEffect;
				createrBubbleType = Bubble.eType.Metal;
				break;
			case Constant.Avatar.eSpecialSkill.NextPlus:
			case Constant.Avatar.eSpecialSkill.NextPlusPlus:
				bShotPlus = true;
				shotPlusNum = (int)sk.LevelEffect;
				break;
			case Constant.Avatar.eSpecialSkill.Coinup:
			case Constant.Avatar.eSpecialSkill.CoinupPlus:
			case Constant.Avatar.eSpecialSkill.CoinupPlusPlus:
				bCoinUp = true;
				CoinUpRate = sk.LevelEffect;
				break;
			case Constant.Avatar.eSpecialSkill.Water:
			case Constant.Avatar.eSpecialSkill.WaterPlus:
			case Constant.Avatar.eSpecialSkill.WaterPlusPlus:
				bCreater = true;
				CreaterExTurn = (int)sk.LevelEffect;
				createrBubbleType = Bubble.eType.Water;
				break;
			case Constant.Avatar.eSpecialSkill.GuideStretch:
			case Constant.Avatar.eSpecialSkill.GuidePlus:
				bGuideStretch = true;
				GuideStretchMultiple = sk.LevelEffect;
				break;
			case Constant.Avatar.eSpecialSkill.NextChange:
			case Constant.Avatar.eSpecialSkill.NextChange_2:
			case Constant.Avatar.eSpecialSkill.NextChangePlus:
				CueBubbleChangeRestNum = (int)sk.LevelEffect;
				bCueBubbleChange = true;
				break;
			}
		}
		bubbleNavi = base.gameObject.AddComponent<BubbleNavi>();
		stageNo = 1;
		if (args.ContainsKey("StageNo"))
		{
			stageNo = int.Parse(args["StageNo"].ToString());
		}
		bParkStage_ = Constant.ParkStage.isParkStage(stageNo);
		if (bParkStage_)
		{
			bEventStage_ = false;
			stageInfo = stageTbl.getInfo(stageNo).Common;
		}
		else
		{
			bEventStage_ = Constant.Event.isEventStage(stageNo);
			if (!bEventStage_)
			{
				stageInfo = stageTbl.getInfo(stageNo).Common;
			}
			else
			{
				EventStageInfo eventData = stageTbl.getEventData();
				eventNo_ = eventData.EventNo;
				stageInfo = stageTbl.getEventInfo(stageNo, eventNo_).Common;
			}
		}
		int bgNo = stageInfo.Bg;
		if (!bParkStage_)
		{
			yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.BG, 0 + (bgNo - 1), 0, downloadCount));
		}
		uiScale = (UnityEngine.Object.FindObjectOfType(typeof(UIRoot)) as UIRoot).transform.localScale.x;
		bonusBasePos *= uiScale;
		bonusClearPos *= uiScale;
		bubble_trail_eff = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "bubble_trail_eff")) as GameObject;
		bubble_trail_eff.name = "bubble_trail_eff";
		bubble_trail_offset = bubble_trail_eff.transform.localPosition;
		Utility.setParent(bubble_trail_eff, uiRoot.transform, false);
		bubble_trail_eff.SetActive(false);
		stagePause = base.gameObject.AddComponent<StagePause>();
		if (args.ContainsKey("StageResponce"))
		{
			StageBeginData responce = (StageBeginData)args["StageResponce"];
			stageInfo.Move = responce.move;
			stageInfo.Time = responce.time;
			stageInfo.Continue.Price = responce.continuePrice;
			stageInfo.Continue.PriceType = responce.continuePriceType;
			stageInfo.Continue.Recovary = responce.continueRecovary;
			helpDataList = responce.helpDataList;
			KeyBubbleData keyData = GlobalData.Instance.getKeyBubbleData();
			if (keyData != null && responce.keyBubble != null)
			{
				keyData.invalidCount = responce.keyBubble.invalidCount;
				keyData.invalidStage = responce.keyBubble.invalidStage;
				keyData.invalidTime = responce.keyBubble.invalidTime;
				keyData.getKeyBubble = responce.keyBubble.getKeyBubble;
				keyData.keyBubbleCount = responce.keyBubble.keyBubbleCount;
				GlobalData.Instance.setKeyBubbleData(keyData);
			}
		}
		if (Constant.Event.isEventStage(stageNo))
		{
			helpDataList = null;
		}
		if (stageInfo.Move > 0)
		{
			gameType = eGameType.ShotCount;
		}
		else
		{
			gameType = eGameType.Time;
		}
		GameObject stageCollider = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "StageCollider")) as GameObject;
		Utility.setParent(stageCollider, uiRoot.transform, true);
		Transform ceilingCollider = stageCollider.transform.Find("Ceiling");
		ceilingCollider.localPosition += NGUIUtilScalableUIRoot.GetOffsetY(true);
		UnityEngine.Object bgResource = ResourceLoader.Instance.loadGameObject("Prefabs/", "Stage_" + bgNo.ToString("00"));
		GameObject bg = UnityEngine.Object.Instantiate(bgResource) as GameObject;
		Utility.setParent(bg, uiRoot.transform, true);
		stageBg = bg.transform;
		GameObject ui = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Stage_ui")) as GameObject;
		setupButton(ui);
		Utility.setParent(ui, uiRoot.transform, true);
		stageUi = ui.transform;
		frontUi = stageUi.Find("Front_ui");
		frontUi.parent = stageUi.parent;
		frontUi.localPosition += Vector3.back;
		if (ResourceLoader.Instance.isUseLowResource())
		{
			Transform score_ui = frontUi.Find("Top_ui/score");
			if (score_ui != null)
			{
				score_ui.gameObject.AddComponent<UIPanel>();
			}
		}
		GameObject pause_button = frontUi.Find("Top_ui/Gamestop_Button").gameObject;
		pause_button.AddComponent<UIPanel>();
		NGUIUtilScalableUIRoot.OffsetUI(frontUi.Find("Top_ui"), true);
		NGUIUtilScalableUIRoot.OffsetUI(frontUi.Find("Bottom_ui"), false);
		frontUi.Find("Top_ui").gameObject.AddComponent<UIPanel>();
		frontUi.Find("Bottom_ui").gameObject.AddComponent<UIPanel>();
		useitem_bg = frontUi.Find("Bottom_ui/useitem_bg").gameObject;
		useitem_bg.SetActive(false);
		shootButton = frontUi.Find("shoot/shoot_button").gameObject;
		shootButton.SetActive(false);
		readyGo = frontUi.Find("ready_go").GetComponent<ReadyGo>();
		readyGo.gameObject.SetActive(false);
		stageClear = frontUi.Find("stage_clear").gameObject;
		if (ResourceLoader.Instance.isUseLowResource())
		{
			Transform[] stageClearStars = stageClear.GetComponentsInChildren<Transform>(true);
			for (int j = 0; j < stageClearStars.Length; j++)
			{
				if (stageClearStars[j].name.StartsWith("star_set"))
				{
					UnityEngine.Object.Destroy(stageClearStars[j].gameObject);
				}
			}
		}
		stageClear.AddComponent<UIPanel>();
		stageClear.SetActive(false);
		scoreRoot = frontUi.Find("Top_ui/score");
		scoreList.Add(scoreRoot.Find("score_number"));
		scoreList[0].name = "01";
		setupPopupScore("normal_erasure", ref popupScoreNormal[0]);
		popupScoreNormal[1] = (UnityEngine.Object.Instantiate(popupScoreNormal[0].gameObject) as GameObject).GetComponent<PopupScore>();
		Utility.setParent(popupScoreNormal[1].gameObject, popupScoreNormal[0].transform.parent, false);
		popupScoreNormal[1].gameObject.SetActive(false);
		setupPopupScore("drop_bonus", ref popupScoreDrop);
		setupPopupScore("drop_clearBonus", ref popupScoreDrop_clear);
		setupPopupScore("Reflect_bonus", ref popupScoreReflect);
		setupPopupScore("Rescue_bonus", ref popupScoreRescue);
		setupPopupScore("skill_bonus", ref popupScoreSkill);
		popupCombo[0] = frontUi.Find("combo_bonus_new").gameObject;
		popupCombo[1] = UnityEngine.Object.Instantiate(popupCombo[0]) as GameObject;
		Utility.setParent(popupCombo[1], popupCombo[0].transform.parent, false);
		GameObject[] array = popupCombo;
		foreach (GameObject pcObj in array)
		{
			PopupCombo pc = pcObj.AddComponent<PopupCombo>();
			pc.stagePause = stagePause;
			pcObj.SetActive(false);
		}
		setupPopupExcellent("excellent", ref popupExcellent);
		if (bCreater || bCueBubbleChange)
		{
			string skillName = ((!bCueBubbleChange) ? "skill_button02" : "skill_button");
			skillButton = frontUi.Find("Bottom_ui/" + skillName);
			skillButton.gameObject.SetActive(true);
			skillNgIcon_ = skillButton.Find("ng_icon").gameObject;
			UIButtonMessage mes = null;
			if (skillButton.GetComponent<UIButtonMessage>() == null)
			{
				skillButton.gameObject.AddComponent<UIButtonMessage>();
			}
			mes = skillButton.GetComponent<UIButtonMessage>();
			mes.target = base.gameObject;
			mes.trigger = UIButtonMessage.Trigger.OnClick;
			mes.functionName = "OnButton";
			if (bCreater)
			{
				string spriteAnimName = string.Empty;
				if (createrBubbleType == Bubble.eType.Bomb)
				{
					spriteAnimName = "bubble_42";
				}
				else if (createrBubbleType == Bubble.eType.Metal)
				{
					spriteAnimName = "bubble_56";
				}
				else if (createrBubbleType == Bubble.eType.Water)
				{
					spriteAnimName = "bubble_75";
				}
				CreaterObj = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "spr_bubble")) as GameObject;
				CreaterObj.GetComponent<Bubble>().setType(createrBubbleType);
				CreaterObj.GetComponentInChildren<tk2dAnimatedSprite>().Play(spriteAnimName);
				CreaterObj.transform.localPosition = new Vector3(-3f, -3f, -1f);
				CreaterObj.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				Utility.setLayer(CreaterObj, "Default");
				Utility.setParent(CreaterObj, skillButton, false);
				skillButton.Find("charge").gameObject.SetActive(true);
				CreaterTurnCountDownLabel_ = skillButton.Find("charge/label_value").gameObject.GetComponent<UILabel>();
				skillButton.Find("charge/label").gameObject.GetComponent<UILabel>().text = MessageResource.Instance.getMessage(8823);
				CreaterSkillSliderBar_ = skillButton.Find("scoregauge").gameObject.GetComponent<UISlider>();
				CreaterStockLabel_ = skillButton.Find("stock_label").gameObject.GetComponent<UILabel>();
				setSkillButtonActive(false);
				SkillButtonSetting(0);
				CreaterUiUpdate();
			}
			if (bCueBubbleChange)
			{
				setSkillButtonActive(false);
				skillButton.Find("bg_add").gameObject.SetActive(false);
				skillNgIcon_.SetActive(false);
			}
		}
		int[] starScores = stageInfo.StarScores;
		starRate1 = (float)starScores[0] / (float)starScores[2];
		starRate2 = (float)starScores[1] / (float)starScores[2];
		Transform sgRoot = frontUi.Find("Bottom_ui/scoregauge");
		Transform clear_line = sgRoot.Find("clear_line");
		GameObject clear_line2 = UnityEngine.Object.Instantiate(clear_line.gameObject) as GameObject;
		clear_line2.transform.parent = clear_line.parent;
		clear_line2.transform.localScale = clear_line.localScale;
		Vector3 pos = clear_line.transform.localPosition;
		pos.y = 109f * starRate1 - 54f;
		clear_line.localPosition = pos;
		pos.y = 109f * starRate2 - 54f;
		clear_line2.transform.localPosition = pos;
		scoregauge = sgRoot.Find("scoregauge").GetComponent<UISlider>();
		scoregauge.sliderValue = 0f;
		scoregauge.GetComponentInChildren<UIFilledSprite>().color = convertColor(140, 145, 140);
		GameObject scoregaugeStar = scoregauge.transform.parent.Find("stars").gameObject;
		scoregaugeStar.transform.localPosition = Vector3.back;
		scoregaugeStar.SetActive(false);
		scoregaugeStar.AddComponent<UIPanel>();
		scoregaugeStar.SetActive(true);
		scoregauge_eff = sgRoot.Find("scoregauge_eff").GetComponent<UISpriteAnimation>();
		if (eventNo_ == 11)
		{
			sgRoot.Find("bg").gameObject.SetActive(false);
			clear_line.gameObject.SetActive(false);
			clear_line2.gameObject.SetActive(false);
			scoregauge.transform.Find("Background").gameObject.SetActive(false);
			scoregauge.transform.Find("Foreground").gameObject.SetActive(false);
		}
		stars = new GameObject[stageInfo.StarScores.Length];
		if (bParkStage_)
		{
			sgRoot.Find("flowers").gameObject.SetActive(true);
			sgRoot.Find("stars").gameObject.SetActive(false);
			for (int k = 0; k < stars.Length; k++)
			{
				stars[k] = sgRoot.Find("flowers/flower_" + k.ToString("00")).gameObject;
				stars[k].SetActive(false);
			}
		}
		else if (bEventStage_)
		{
			if (eventNo_ == 1)
			{
				sgRoot.Find("notes").gameObject.SetActive(true);
				sgRoot.Find("stars").gameObject.SetActive(false);
				for (int l = 0; l < stars.Length; l++)
				{
					stars[l] = sgRoot.Find("notes/note_0" + l).gameObject;
					stars[l].SetActive(false);
				}
			}
			else
			{
				sgRoot.Find("notes").gameObject.SetActive(false);
				sgRoot.Find("stars").gameObject.SetActive(false);
				sgRoot.Find("clear_line").gameObject.SetActive(false);
				sgRoot.Find("clear_line(Clone)").gameObject.SetActive(false);
			}
		}
		else
		{
			sgRoot.Find("notes").gameObject.SetActive(false);
			sgRoot.Find("stars").gameObject.SetActive(true);
			for (int m = 0; m < stars.Length; m++)
			{
				stars[m] = sgRoot.Find("stars/star_0" + m).gameObject;
				stars[m].SetActive(false);
			}
		}
		countdownContinue = frontUi.Find("countdown_start/img_countdown_start").gameObject;
		keyIcon = frontUi.Find("key_eff").gameObject;
		keyIcon.SetActive(false);
		keyAnim = frontUi.Find("key_get").gameObject;
		keyAnim.SetActive(false);
		boundEffL = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "bound_eff_l")) as GameObject;
		boundEffR = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "bound_eff_r")) as GameObject;
		Utility.setParent(boundEffL, uiRoot.transform, true);
		Utility.setParent(boundEffR, uiRoot.transform, true);
		boundEffL.SetActive(false);
		boundEffR.SetActive(false);
		Transform uiTrans = ui.transform;
		launchpad = uiTrans.Find("launchpad");
		nextBubblePoses[0] = launchpad.Find("set_bubble");
		nextBubblePoses[1] = launchpad.Find("next_00");
		nextBubblePoses[2] = launchpad.Find("next_01");
		Transform combo = launchpad.Find("combo");
		comboObject = combo.gameObject;
		TweenScale[] tss = combo.GetComponents<TweenScale>();
		TweenScale[] array2 = tss;
		foreach (TweenScale ts in array2)
		{
			if (ts.tweenName == "In")
			{
				comboIn = ts;
			}
			else
			{
				comboOut = ts;
			}
		}
		comboLabel = combo.GetComponentInChildren<UILabel>();
		comboAnim = combo.GetComponent<Animation>();
		comboAnim.playAutomatically = false;
		comboAnim.enabled = true;
		yield return StartCoroutine(characterInstantiate());
		if (bParkStage_)
		{
			Transform leader_minilen_trans = stageUi.Find("minilen_base");
			if ((bool)leader_minilen_trans)
			{
				leader_minilen = leader_minilen_trans.GetComponent<UISprite>();
				if ((bool)leader_minilen)
				{
					if (current_minilen != null)
					{
						leader_minilen.spriteName = "UI_set_minilen_" + (current_minilen.index % 10000).ToString("000");
					}
					else
					{
						leader_minilen = null;
					}
				}
			}
		}
		nextBubbleBobllenBefor_Y = nextBubblePoses[1].transform.localPosition.y;
		arrow = launchpad.Find("arrow_pivot").GetComponent<Arrow>();
		arrow.part = this;
		arrow.bubblen = stageUi.Find(charaNames[0]).GetComponentInChildren<tk2dAnimatedSprite>();
		stepNextBubbleAnim = launchpad.GetComponent<Animation>();
		stepNextBubbleAnim.enabled = true;
		stepNextBubbleAnim.playAutomatically = false;
		stepNextBubbleAnim.Stop();
		stepNextBubbleAnim[stepNextBubbleClipName].clip.SampleAnimation(stepNextBubbleAnim.gameObject, 0f);
		next_tap = launchpad.Find("next_tap").gameObject;
		setNextTap(false);
		guide = base.gameObject.AddComponent<Guide>();
		guide.part = this;
		guide.guideline_pos = launchpad.Find("arrow_pivot/guideline_pos").gameObject;
		guide.uiScale = uiScale;
		arrow.guide = guide;
		Vector3 temp_ceiling_pos = ceilingCollider.localPosition;
		temp_ceiling_pos.y -= 20f;
		ceilingCollider.localPosition = temp_ceiling_pos;
		guide.setCeilingPos(ceilingCollider.position);
		temp_ceiling_pos = ceilingCollider.localPosition;
		temp_ceiling_pos.y += 20f;
		ceilingCollider.localPosition = temp_ceiling_pos;
		bubble_08_eff = frontUi.Find("bubble_08_eff").gameObject;
		bubble_08_eff.SetActive(false);
		bubble_17_eff = frontUi.Find("bubble_17_eff").gameObject;
		bubble_17_eff.SetActive(false);
		honeycomb_eff = frontUi.Find("bee_eff").gameObject;
		honeycomb_eff.tag = "Bee";
		honeycomb_eff.SetActive(false);
		snake_eff = new GameObject[3];
		GameObject snake_eff_base = frontUi.Find("snake_00").gameObject;
		Vector3 temp_snake_pos = snake_eff_base.transform.localPosition;
		temp_snake_pos.z = 10f;
		snake_eff_base.transform.localPosition = temp_snake_pos;
		for (int i4 = 0; i4 < 3; i4++)
		{
			snake_eff[i4] = UnityEngine.Object.Instantiate(snake_eff_base) as GameObject;
			Utility.setParent(snake_eff[i4], frontUi, false);
			snake_eff[i4].name = "snake_" + i4.ToString("00");
			snake_eff[i4].SetActive(true);
			snake_eff[i4].GetComponentInChildren<UISprite>().depth -= i4;
			snake_eff[i4].SetActive(false);
		}
		UnityEngine.Object.DestroyImmediate(snake_eff_base);
		snakeCounter = frontUi.Find("snake_counter").gameObject;
		if (ResourceLoader.Instance.isUseLowResource())
		{
			snakeCounter.AddComponent<UIPanel>();
		}
		snakeCounterLabel = snakeCounter.transform.Find("snake_count").GetComponent<UILabel>();
		snakeCounterAnm = snakeCounter.GetComponent<Animation>();
		bubble_19_eff = frontUi.Find("bubble_19_eff").gameObject;
		drawLineBase_ = bubble_19_eff.transform.Find("draw_guide").gameObject;
		drawWaterBase_ = bubble_19_eff.transform.Find("draw_water").gameObject;
		drawCounter_ = new GameObject("draw_counter_parent");
		drawCounter_.transform.parent = frontUi;
		drawCounter_.transform.localScale = Vector3.one;
		drawCounter_.transform.localPosition = Vector3.zero;
		drawCounter_.AddComponent<UIPanel>();
		drawCounteSprite_ = bubble_19_eff.transform.Find("draw_counter/draw_count").GetComponent<UISprite>();
		bubble_19_eff.transform.Find("draw_counter").parent = drawCounter_.transform;
		drawCounter_.SetActive(false);
		drawTouchPoint_ = new GameObject("draw_touch_point");
		drawTouchPoint_.transform.parent = frontUi;
		drawTouchPoint_.transform.localScale = Vector3.one;
		drawTouchPoint_.transform.localPosition = Vector3.zero;
		drawHelp_ = bubble_19_eff.transform.Find("draw_help").gameObject;
		drawHelp_.transform.parent = bubble_19_eff.transform.parent;
		drawHelp_.transform.localScale = Vector3.one;
		drawHelp_.AddComponent<UIPanel>();
		drawHelp_.SetActive(false);
		bubble_20_eff = frontUi.Find("bubble_20_eff").gameObject;
		bubble_20_eff.SetActive(false);
		bubble_20_eff.AddComponent<UIPanel>();
		counter_count = frontUi.Find("counter_count").gameObject;
		counter_count.SetActive(false);
		counter_eff = frontUi.Find("counter_eff").gameObject;
		counter_eff.SetActive(false);
		counter_eff_anm = counter_eff.GetComponent<Animation>();
		beebarrier_eff = frontUi.Find("beebarrier_eff").gameObject;
		beebarrier_eff_00 = beebarrier_eff.transform.Find("beebarrier_eff_00").gameObject;
		bee_barrier_anm = beebarrier_eff.GetComponent<Animation>();
		timestop_eff = frontUi.Find("timestop_eff").gameObject;
		timestop_counter = frontUi.Find("timestop_counter").gameObject;
		timestop_counter.SetActive(true);
		timestop_counter_label = timestop_counter.transform.Find("timestop_root/timestop_count").GetComponent<UILabel>();
		timestop_counter.SetActive(false);
		string supportChara_ = string.Empty;
		if (bParkStage_)
		{
			if (avatar.supportCharacter > 0)
			{
				supportChara_ = "_" + (avatar.supportCharacter - 1).ToString("00");
			}
		}
		else if (GlobalData.Instance.currentAvatar.supportCharacter > 0)
		{
			supportChara_ = "_" + (GlobalData.Instance.currentAvatar.supportCharacter - 1).ToString("00");
		}
		if (ResourceLoader.Instance.loadGameObject("Prefabs/", "vacuum_chara" + supportChara_) == null)
		{
			supportChara_ = string.Empty;
		}
		vacuum_eff = frontUi.Find("vacuum_eff").gameObject;
		vacuum_bob = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "vacuum_chara" + supportChara_)) as GameObject;
		Utility.setParent(vacuum_bob, frontUi, false);
		vacuum_bob_01 = vacuum_bob.transform.Find("bg_00_01").GetComponent<UISprite>();
		vacuum_bob.SetActive(false);
		vacuum_counter = frontUi.Find("vacuum_counter").gameObject;
		vacuum_counter.SetActive(true);
		vacuum_counter_label = vacuum_counter.transform.Find("vacuum_root/vacuum_count").GetComponent<UILabel>();
		vacuum_counter.SetActive(false);
		Transform bubble_bonus = frontUi.Find("bubble_bonus");
		if (gameType == eGameType.ShotCount)
		{
			bubbleBonusBases[0] = bubble_bonus.Find("00").gameObject;
		}
		else
		{
			bubbleBonusBases[0] = bubble_bonus.Find("01").gameObject;
		}
		bubbleBonusBases[1] = bubble_bonus.Find("02").gameObject;
		bubbleBonusBases[2] = bubble_bonus.Find("03").gameObject;
		bubbleBonusBases[3] = bubble_bonus.Find("04").gameObject;
		bubbleBonusBases[4] = bubble_bonus.Find("05").gameObject;
		bubbleBonusBases[5] = bubble_bonus.Find("06").gameObject;
		bubble_bonus.gameObject.SetActive(false);
		if (bParkStage_)
		{
			stageData = stageTbl.getParkPlacementData(stageNo);
		}
		else if (bEventStage_)
		{
			string filePath = StageDataTable.getEventStageDataFilePath(level: (EventStageInfo.eLevel)Constant.Event.convNoToLevel(stageNo, eventNo_), eventNo: eventNo_);
			byte[] bytes = Aes.DecryptFromFile(filePath);
			Encoding enc = Encoding.GetEncoding("UTF-8");
			string dataText = enc.GetString(bytes);
			stageData = Xml.DeserializeObject<StageData>(dataText) as StageData;
		}
		else
		{
			stageData = stageTbl.getPlacementData(stageNo);
		}
		int lineNum = stageData.lineNum + 1;
		GameObject bubbleRootObj = new GameObject("BubbleRoot");
		bubbleRootObj.AddComponent<UIPanel>();
		bubbleRoot = bubbleRootObj.transform;
		Utility.setParent(bubbleRootObj, uiRoot.transform, true);
		int lineStart = 10;
		if (lineStart > lineNum)
		{
			lineStart = lineNum;
		}
		bubbleRoot.localPosition = new Vector3(bubbleRootPos.x, bubbleRootPos.y, -0.1f);
		yield return null;
		GameObject nextBubbleRootObj = new GameObject("NextBubbleRoot");
		nextBubbleRoot = nextBubbleRootObj.transform;
		nextBubbleRootObj.AddComponent<UIPanel>();
		Utility.setParent(nextBubbleRootObj, uiRoot.transform, true);
		nextBubbleRoot.localPosition = new Vector3(bubbleRootPos.x, bubbleRootPos.y, -0.1f);
		float offset_y = 52 * lineNum - 52 * lineStart;
		changeBubbleColor();
		bubbleObject = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "spr_bubble")) as GameObject;
		Utility.setParent(bubbleObject, base.transform, false);
		bubbleObject.SetActive(false);
		Bubble b = bubbleObject.GetComponent<Bubble>();
		b.part = this;
		b.stagePause = stagePause;
		chainBubbleObject = ResourceLoader.Instance.loadGameObject("Prefabs/", "spr_chain_bubble");
		ChainBubble c = chainBubbleObject.GetComponent<ChainBubble>();
		c.part = this;
		c.stagePause = stagePause;
		Transform material_root = b.transform.Find("AS_spr_bubble");
		normalMat = new Material(material_root.GetComponent<tk2dAnimatedSprite>().GetComponent<Renderer>().material);
		gameoverMat = new Material(material_root.GetComponent<tk2dAnimatedSprite>().GetComponent<Renderer>().material);
		gameoverMat.shader = Shader.Find("Custom/CustomGrayscale");
		chackunCount = 0;
		nearCoinBubbleList_.Clear();
		chameleonBubbleList.Clear();
		for (int j2 = 0; j2 < lineNum; j2++)
		{
			int offset = 0;
			if (j2 % 2 == 0)
			{
				offset = 30;
			}
			for (int i = 0; i < 10; i++)
			{
				if (j2 % 2 == 0 && i == 9)
				{
					continue;
				}
				bool is_minilen_bubble = false;
				GameObject obj;
				Bubble bubble;
				if (j2 > 0)
				{
					int index = 10 * (j2 - 1) + i;
					string bubble_name = ((stageData.bubbleTypes[index] <= 99) ? stageData.bubbleTypes[index].ToString("00") : stageData.bubbleTypes[index].ToString("000"));
					if (bubble_name == "99")
					{
						continue;
					}
					obj = UnityEngine.Object.Instantiate(bubbleObject) as GameObject;
					obj.SetActive(true);
					obj.name = bubble_name;
					bubble = obj.GetComponent<Bubble>();
					bubble.init();
					bubble.createIndex = index;
					Bubble.eType type7 = bubble.type;
					if (type7 <= Bubble.eType.Black)
					{
						if (!glossColorList.Contains(type7))
						{
							glossColorList.Add(type7);
						}
						if (!bubbleColorList.Contains((int)type7))
						{
							bubbleColorList.Add((int)type7);
						}
					}
					else if (type7 >= Bubble.eType.PlusRed && type7 <= Bubble.eType.PlusBlack)
					{
						type7 -= 13;
						if (!glossColorList.Contains(type7))
						{
							glossColorList.Add(type7);
						}
						if (!bubbleColorList.Contains((int)type7))
						{
							bubbleColorList.Add((int)type7);
						}
					}
					else if (type7 >= Bubble.eType.MinusRed && type7 <= Bubble.eType.MinusBlack)
					{
						type7 -= 21;
						if (!glossColorList.Contains(type7))
						{
							glossColorList.Add(type7);
						}
						if (!bubbleColorList.Contains((int)type7))
						{
							bubbleColorList.Add((int)type7);
						}
					}
					else if (type7 >= Bubble.eType.FriendRed && type7 <= Bubble.eType.FriendBlack)
					{
						type7 -= 31;
						if (!glossColorList.Contains(type7))
						{
							glossColorList.Add(type7);
						}
						if (!bubbleColorList.Contains((int)type7))
						{
							bubbleColorList.Add((int)type7);
						}
						if (!clearChacknList.Contains(type7))
						{
							clearChacknList.Add(type7);
						}
						chackunCount++;
					}
					else if (type7 >= Bubble.eType.SnakeRed && type7 <= Bubble.eType.SnakeBlack)
					{
						type7 -= 67;
						if (!glossColorList.Contains(type7))
						{
							glossColorList.Add(type7);
						}
						if (!bubbleColorList.Contains((int)type7))
						{
							bubbleColorList.Add((int)type7);
						}
					}
					else
					{
						switch (type7)
						{
						case Bubble.eType.FriendRainbow:
							if (!clearChacknList.Contains(Bubble.eType.FriendRainbow))
							{
								clearChacknList.Add(Bubble.eType.FriendRainbow);
							}
							chackunCount++;
							break;
						case Bubble.eType.FriendBox:
							if (!clearChacknList.Contains(Bubble.eType.FriendBox))
							{
								clearChacknList.Add(Bubble.eType.FriendBox);
							}
							chackunCount++;
							break;
						case Bubble.eType.Coin:
							nearCoinBubbleList_.Add(bubble);
							break;
						case Bubble.eType.Counter:
						case Bubble.eType.CounterRed:
						case Bubble.eType.CounterGreen:
						case Bubble.eType.CounterBlue:
						case Bubble.eType.CounterYellow:
						case Bubble.eType.CounterOrange:
						case Bubble.eType.CounterPurple:
						case Bubble.eType.CounterWhite:
						case Bubble.eType.CounterBlack:
						{
							for (int k2 = 0; k2 < stageData.counteIndex.Length; k2++)
							{
								if (index == stageData.counteIndex[k2])
								{
									bubble.setCounterCount(stageData.counteCount[k2]);
									break;
								}
							}
							break;
						}
						default:
							if (type7 >= Bubble.eType.TunnelOutLeftUP && type7 <= Bubble.eType.TunnelOutRightDown)
							{
								for (int k4 = 0; k4 < stageData.outIndex.Length; k4++)
								{
									if (index == stageData.outIndex[k4])
									{
										bubble.outObjectType = stageData.outType[k4];
										switch (stageData.outType[k4])
										{
										case 1:
											obj.transform.Find("AS_spr_bubble").transform.rotation = Quaternion.Euler(0f, 0f, 30f);
											break;
										case 2:
											obj.transform.Find("AS_spr_bubble").transform.rotation = Quaternion.Euler(0f, 0f, 0f);
											break;
										case 3:
											obj.transform.Find("AS_spr_bubble").transform.rotation = Quaternion.Euler(0f, 0f, 330f);
											break;
										case 4:
											obj.transform.Find("AS_spr_bubble").transform.rotation = Quaternion.Euler(0f, 0f, 150f);
											break;
										case 5:
											obj.transform.Find("AS_spr_bubble").transform.rotation = Quaternion.Euler(0f, 0f, 180f);
											break;
										case 6:
											obj.transform.Find("AS_spr_bubble").transform.rotation = Quaternion.Euler(0f, 0f, 210f);
											break;
										}
									}
								}
							}
							else if (type7 >= Bubble.eType.ChameleonRed && type7 <= Bubble.eType.Unknown)
							{
								chameleonBubbleList.Add(bubble);
								if (type7 != Bubble.eType.Unknown)
								{
									bubble.GetComponentInChildren<tk2dAnimatedSprite>().Stop();
									type7 -= 79;
									if (!bubbleColorList.Contains((int)type7))
									{
										bubbleColorList.Add((int)type7);
									}
									break;
								}
								for (int k3 = 0; k3 < stageData.skeltonIndex.Length; k3++)
								{
									if (index == stageData.skeltonIndex[k3])
									{
										bubble.unknownColor = stageData.skeltonColor[k3];
										if (!bubbleColorList.Contains(bubble.unknownColor))
										{
											bubbleColorList.Add(bubble.unknownColor);
										}
										break;
									}
								}
							}
							else if (type7 >= Bubble.eType.MinilenRed && type7 <= Bubble.eType.MinilenBlack)
							{
								is_minilen_bubble = true;
								minilen_count_all++;
								type7 -= 128;
								if (!glossColorList.Contains(type7))
								{
									glossColorList.Add(type7);
								}
								if (!bubbleColorList.Contains((int)type7))
								{
									bubbleColorList.Add((int)type7);
								}
								if (!clearMinilenList.Contains(type7))
								{
									clearMinilenList.Add(type7);
								}
							}
							else if (type7 == Bubble.eType.MinilenRainbow)
							{
								if (!clearMinilenList.Contains(type7))
								{
									clearMinilenList.Add(type7);
								}
								is_minilen_bubble = true;
								minilen_count_all++;
							}
							break;
						case Bubble.eType.Skull:
							break;
						}
					}
				}
				else
				{
					obj = UnityEngine.Object.Instantiate(bubbleObject) as GameObject;
					obj.SetActive(true);
					obj.name = "99";
					bubble = obj.GetComponent<Bubble>();
					bubble.init();
					bubble.setCeiling(i == 0);
				}
				Transform trans = obj.transform;
				trans.parent = bubbleRoot;
				trans.localScale = Vector3.one;
				trans.localPosition = new Vector3(i * 60 + offset, (float)(j2 * -52) + offset_y, 0f);
				bubble.uniqueId = Mathf.RoundToInt(trans.localPosition.x) + 2500 * Mathf.RoundToInt(trans.localPosition.y);
				if (is_minilen_bubble)
				{
					_minilen_bubble_unique_ids.Add(bubble.uniqueId);
				}
				bubble.setFieldState();
				fieldBubbleList.Add(bubble);
				if (j2 == 0)
				{
					ceilingBubbleList.Add(trans);
				}
			}
		}
		bubbleColorList.Sort();
		foreach (Bubble bubble3 in chameleonBubbleList)
		{
			Bubble.eType color = ((bubble3.type != Bubble.eType.Unknown) ? convertColorBubbleFixed(bubble3.type) : ((Bubble.eType)bubble3.unknownColor));
			for (int i6 = 0; i6 < bubbleColorList.Count; i6++)
			{
				if (color == (Bubble.eType)bubbleColorList[i6])
				{
					bubble3.chamelleonIndex = i6;
					break;
				}
			}
		}
		chamelleonIndexCount = bubbleColorList.Count;
		int morganaCount = 0;
		if (stageData.morganaParentRow != null)
		{
			morganaCount = stageData.morganaParentRow.Length;
		}
		Debug.Log("morganaCount = " + morganaCount);
		if (morganaCount > 0)
		{
			for (int i5 = 0; i5 < morganaCount; i5++)
			{
				stageData.morganaParentRow[i5]++;
				int offset_x = 0;
				if (stageData.morganaParentRow[i5] % 2 == 0)
				{
					offset_x = 30;
				}
				Vector3 tempPos = new Vector3(stageData.morganaParentColumn[i5] * 60 + offset_x, (float)(stageData.morganaParentRow[i5] * -52) + offset_y, 0f);
				Debug.Log("morganaPos = " + tempPos);
				foreach (Bubble bubble4 in fieldBubbleList)
				{
					if (!(Vector3.Distance(bubble4.transform.localPosition, tempPos) < 1f))
					{
						continue;
					}
					bubble4.CharaNum_ = stageData.morganaType[i5];
					bubble4.IsMorganaParent_ = true;
					MorganaList_.Add(bubble4);
					bubble4.Setup();
					foreach (Bubble fb in fieldBubbleList)
					{
						if (bubble4.isNearBubble(fb))
						{
							fb.ParentMorgana_ = bubble4;
							bubble4.ChildMorgana_.Add(fb);
							fb.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
						}
					}
					Debug.Log("Vector3.Distance(bubble.transform.localPosition,tempPos) = " + Vector3.Distance(bubble4.transform.localPosition, tempPos));
				}
			}
		}
		createChainBubble(lineNum - 1);
		Ceiling ceiling = frontUi.Find("stage_ceiling").gameObject.AddComponent<Ceiling>();
		ceiling.setTarget(ceilingBubbleList[0]);
		fadeMaterial = new Material(fieldBubbleList[0].transform.Find("AS_spr_bubble").GetComponent<Renderer>().sharedMaterial);
		fadeMaterial.shader = Shader.Find("Unlit/Transparent Colored");
		Transform countdown = stageUi.Find("launchpad/countdown");
		countLabel = countdown.Find("count_label").GetComponent<UILabel>();
		REST_COUNT_DEFAULT_COLOR = countLabel.color;
		countdown.Find("count_time").GetComponent<UILabel>().text = MessageResource.Instance.getMessage((gameType != 0) ? 201 : 200);
		Transform stage_title = frontUi.Find("Top_ui/stage_title");
		stage_title.localPosition = new Vector3(stage_title.localPosition.x, stage_title.localPosition.y, -0.5f);
		if (bParkStage_)
		{
			stage_title.Find("Label_number").GetComponent<UILabel>().text = (stageNo % 10000).ToString();
			stage_title.Find("Label_title").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(202);
		}
		else if (!bEventStage_)
		{
			stage_title.Find("Label_number").GetComponent<UILabel>().text = (stageNo + 1).ToString();
			stage_title.Find("Label_title").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(202);
		}
		else
		{
			Transform stage_num_tran = stage_title.Find("Label_number");
			stage_num_tran.GetComponent<UILabel>().text = (stageNo - eventNo_ * 10000).ToString();
			stage_num_tran.localScale = new Vector3(18f, 24f, 1f);
			stage_num_tran.localPosition = new Vector3(stage_num_tran.localPosition.x + 6f, stage_num_tran.localPosition.y, stage_num_tran.localPosition.z);
			if (stageNo >= 110000 && stageNo <= 110020)
			{
				stage_num_tran.localPosition = new Vector3(stage_num_tran.localPosition.x + 8.5f, stage_num_tran.localPosition.y, stage_num_tran.localPosition.z);
			}
			Transform stage_title_tran = stage_title.Find("Label_title");
			switch (eventNo_)
			{
			case 1:
				stage_title_tran.GetComponent<UILabel>().text = MessageResource.Instance.getMessage(1491);
				break;
			case 2:
				stage_title_tran.GetComponent<UILabel>().text = MessageResource.Instance.getMessage(4100);
				break;
			case 11:
				stage_title_tran.GetComponent<UILabel>().text = MessageResource.Instance.getMessage(3701);
				break;
			}
			stage_title_tran.localScale = new Vector3(16f, 24f, 1f);
			stage_title_tran.localPosition = new Vector3(stage_title_tran.localPosition.x + 8f, stage_title_tran.localPosition.y, stage_title_tran.localPosition.z);
			if (stageNo >= 110000 && stageNo <= 110020)
			{
				stage_title_tran.localPosition = new Vector3(stage_title_tran.localPosition.x + 7f, stage_title_tran.localPosition.y, stage_title_tran.localPosition.z);
			}
		}
		updateTotalScoreDisp();
		updateFieldBubbleList();
		setupIvies();
		setClearConditionDisplay();
		lineFriendBase = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Player_icon")) as GameObject;
		lineFriendBase.transform.Find("frame").localPosition = Vector3.back * 0.03f;
		lineFriendBase.transform.Find("Player_icon").localPosition = Vector3.back * 0.02f;
		Utility.setParent(lineFriendBase, uiRoot.transform, false);
		defaultUserIconTexture = lineFriendBase.GetComponentInChildren<UITexture>().mainTexture;
		lineFriendBase.SetActive(false);
		yield return StartCoroutine(dialogManager.load(PartManager.ePart.Stage));
		listItem_ = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "SendHighscore_item")) as GameObject;
		listItem_.SetActive(false);
		Utility.setParent(listItem_, base.transform, false);
		DialogHighScoreList dialog = dialogManager.getDialog(DialogManager.eDialog.HighScoreList) as DialogHighScoreList;
		dialog.init(listItem_);
		GameObject scrollUiObj = new GameObject("ScrollUI");
		Utility.setParent(scrollUiObj, uiRoot.transform, true);
		scrollUi = scrollUiObj.transform;
		bubbleRoot.parent = scrollUi;
		nextBubbleRoot.parent = scrollUi;
		stageBg.parent = scrollUi;
		stageUi.parent = scrollUi;
		setupCloud();
		yield return StartCoroutine(cloudUpdateCheck());
		if (cloudList.Count == 0)
		{
			yield return StartCoroutine(setupChameleon());
		}
		Vector3 snake_pos2 = new Vector3(snakeCounter.transform.position.x, snakeCounter.transform.position.y, snakeCounter.transform.position.z);
		Utility.setParent(snakeCounter, scrollUi, true);
		snakeCounter.transform.position = snake_pos2;
		for (int i3 = 0; i3 < 3; i3++)
		{
			snake_pos2 = new Vector3(snake_eff[i3].transform.position.x, snake_eff[i3].transform.position.y, snake_eff[i3].transform.position.z);
			Utility.setParent(snake_eff[i3], scrollUi, true);
			snake_eff[i3].transform.position = snake_pos2;
		}
		chacknBase = stageUi.Find("chara_02").gameObject;
		minilenBase = stageUi.Find("chara_03").gameObject;
		skullBase = frontUi.Find("burst_30_eff").gameObject;
		scoreParticleBase = frontUi.Find("erase_eff").gameObject;
		if (ResourceLoader.Instance.isUseLowResource())
		{
			UnityEngine.Object.Destroy(scoreParticleBase.transform.Find("particle").gameObject);
		}
		countdown_eff = stageUi.Find("launchpad/countdown_eff").gameObject;
		countdown_bad_eff = stageUi.Find("launchpad/countdown_bad_eff").gameObject;
		skullBarrier = frontUi.Find("bubble_30_guard").gameObject;
		DialogPause pauseDialog = dialogManager.getDialog(DialogManager.eDialog.Pause) as DialogPause;
		pauseDialog.init(stageInfo);
		useItemParent_ = frontUi.Find("Bottom_ui/show_boosts").GetComponent<StageUseBoostItemParent>();
		useItemParent_.setup();
		growGameOver_ = frontUi.Find("grow_friend").gameObject;
		Dictionary<Constant.Item.eType, int> mapItemList_ = new Dictionary<Constant.Item.eType, int>();
		for (int i2 = 0; i2 < stageInfo.ItemNum; i2++)
		{
			string typeKey = "item_" + i2 + "type";
			string numKey = "item_" + i2 + "num";
			if (args.ContainsKey(typeKey) && args.ContainsKey(numKey))
			{
				Constant.Item.eType item = (Constant.Item.eType)(int)args[typeKey];
				int num = (int)args[numKey];
				mapItemList_.Add(item, num);
				if (Constant.Item.IsAutoUse(item) && item != Constant.Item.eType.TimeStop && item != Constant.Item.eType.Vacuum)
				{
					useItem(item, num);
				}
				useItemCount++;
			}
		}
		itemParent_ = frontUi.Find("Bottom_ui/boost_items").GetComponent<StageBoostItemParent>();
		itemParent_.setup(stageInfo, mapItemList_);
		itemReplay_ = itemParent_.getItem(Constant.Item.eType.Replay);
		itemChangeUp_ = itemParent_.getItem(Constant.Item.eType.ChangeUp);
		itemTimeStop_ = itemParent_.getItem(Constant.Item.eType.TimeStop);
		itemVacuum_ = itemParent_.getItem(Constant.Item.eType.Vacuum);
		if (itemTimeStop_ != null)
		{
			timestop_counter.transform.parent = itemTimeStop_.gameObject.transform;
			timestop_counter.transform.localPosition = new Vector3(0f, 0f, -0.5f);
		}
		if (itemVacuum_ != null)
		{
			vacuum_counter.transform.parent = itemVacuum_.gameObject.transform;
			vacuum_counter.transform.localPosition = new Vector3(0f, 0f, -0.5f);
		}
		itemParent_.disable();
		TutorialManager.Instance.load(stageNo, uiRoot);
		searchNextBubble();
		if (bCueBubbleChange)
		{
			cueBubbleChangeObj = UnityEngine.Object.Instantiate(bubbleObject) as GameObject;
			cueBubbleChangeObj.transform.localPosition = new Vector3(-3f, -3f, -1f);
			cueBubbleChangeObj.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
			Utility.setLayer(cueBubbleChangeObj, "Default");
			Utility.setParent(cueBubbleChangeObj, skillButton, false);
			cueBubbleChangeObj.name = "00";
			cueBubble = cueBubbleChangeObj.GetComponent<Bubble>();
			cueBubble.init();
		}
		obstacleList = new List<ObstacleDefend>();
		if (stageData.obstacleType != null)
		{
			for (int n = 0; n < stageData.obstacleType.Length; n++)
			{
				GameObject od = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "ojama_00")) as GameObject;
				od.GetComponent<ObstacleDefend>().myType = (ObstacleDefend.eType)stageData.obstacleType[n];
				obstacleList.Add(od.GetComponent<ObstacleDefend>());
			}
			int obstacleCount = 0;
			obstacleList.ForEach(delegate(ObstacleDefend obstacleDefend)
			{
				obstacleDefend.Initialize();
				obstacleDefend.partStage_ = this;
				Bubble bubble5 = null;
				stageData.obstacleRow[obstacleCount]++;
				int num5 = 0;
				if (stageData.obstacleRow[obstacleCount] % 2 == 0)
				{
					num5 = 30;
				}
				Vector3 localPosition = new Vector3(stageData.obstacleColumn[obstacleCount] * 60 + num5, (float)(stageData.obstacleRow[obstacleCount] * -52) + offset_y, -5f);
				Utility.setParent(obstacleDefend.gameObject, bubbleRoot.transform, true);
				obstacleDefend.transform.localPosition = localPosition;
				obstacleDefend.parentBubbleFindSet(fieldBubbleList);
				obstacleCount++;
			});
		}
		foreach (Bubble bubble2 in fieldBubbleList)
		{
			bubble2.inCloud = false;
		}
		inOutObjectCountup();
		MoveOutObjectCheck();
		MoveOutObjectDefend();
		int park_minilen_drop_seed = 0;
		if (bParkStage_ && args.ContainsKey("parkMinilenDropId"))
		{
			_prak_minilen_drop_id = (int)args["parkMinilenDropId"];
		}
		if (_prak_minilen_drop_id > 0 && args.ContainsKey("parkMinilenDropSeed"))
		{
			park_minilen_drop_seed = (int)args["parkMinilenDropSeed"];
		}
		if (park_minilen_drop_seed > 0 && _minilen_bubble_unique_ids.Count > 0)
		{
			int prak_minilen_drop_index = park_minilen_drop_seed % _minilen_bubble_unique_ids.Count;
			_prak_minilen_drop_unique_id = _minilen_bubble_unique_ids[prak_minilen_drop_index];
		}
		yield return StartCoroutine(setupRealLink());
		yield return StartCoroutine(setupKeyBubble());
		playBGM(dataTable);
		yield return stagePause.sync();
		startTime = Time.time;
		shotCount = 0;
		updateRestCountDisp();
		Network.DailyMission mission = GlobalData.Instance.getDailyMissionData();
		if (mission != null)
		{
			missionType = mission.type;
		}
		bInitialized = true;
		StartCoroutine(startRoutine(stageInfo, args));
	}

	private void playBGM(GameObject dataTable)
	{
		if (bParkStage_)
		{
			StageDataTable component = dataTable.GetComponent<StageDataTable>();
			if ((bool)component)
			{
				ParkStageInfo.Info parkInfo = component.getParkInfo(stageNo);
				if (parkInfo != null && parkInfo.Common.Bgm >= 0)
				{
					Sound.Instance.playBgm((Sound.eBgm)parkInfo.Common.Bgm, true);
					return;
				}
			}
		}
		if (bEventStage_ && stageNo / 10000 != 2)
		{
			Sound.Instance.playBgm(Sound.eBgm.BGM_000_Title, true);
			return;
		}
		int maxStageIconsNum = dataTable.GetComponent<StageIconDataTable>().getMaxStageIconsNum();
		if (stageNo == maxStageIconsNum - 1)
		{
			Sound.Instance.playBgm(Sound.eBgm.BGM_004_Boss, true);
			return;
		}
		StageInfo.Info info = dataTable.GetComponent<StageDataTable>().getInfo(stageNo);
		StageInfo.Info info2 = dataTable.GetComponent<StageDataTable>().getInfo(stageNo + 1);
		if (info2 != null && info.Area < info2.Area)
		{
			Sound.Instance.playBgm(Sound.eBgm.BGM_004_Boss, true);
			return;
		}
		switch ((stageNo + 1) % 3)
		{
		case 0:
			Sound.Instance.playBgm(Sound.eBgm.BGM_003_Stage3, true);
			break;
		case 1:
			Sound.Instance.playBgm(Sound.eBgm.BGM_001_Stage1, true);
			break;
		case 2:
			Sound.Instance.playBgm(Sound.eBgm.BGM_002_Stage2, true);
			break;
		}
	}

	private void setupPopupScore(string popupName, ref PopupScore popupScore)
	{
		GameObject gameObject = frontUi.Find(popupName).gameObject;
		popupScore = gameObject.AddComponent<PopupScore>();
		popupScore.stagePause = stagePause;
		gameObject.SetActive(false);
	}

	private void setupPopupExcellent(string popupName, ref PopupExcellent popupExcellent)
	{
		GameObject gameObject = frontUi.Find(popupName).gameObject;
		popupExcellent = gameObject.AddComponent<PopupExcellent>();
		popupExcellent.stagePause = stagePause;
		gameObject.SetActive(false);
	}

	private IEnumerator startRoutine(StageInfo.CommonInfo stageInfo, Hashtable args)
	{
		yield return StartCoroutine(playOPAnime());
		setNextTap(true);
		foreach (Bubble b in fieldBubbleList)
		{
			b.SetBasicSkillIcon(true);
		}
		if (gameType == eGameType.Time && createBlitzBubble())
		{
			updateFieldBubbleList();
		}
		yield return StartCoroutine(stepNextBubble());
		if ((bool)leader_minilen)
		{
			leader_minilen.gameObject.SetActive(true);
		}
		if (stageNo == 2)
		{
			nextBubbles[1].GetComponent<Bubble>().setType(Bubble.eType.Blue);
		}
		if (stageNo == 1 || stageNo == 10)
		{
			args.Add("nextBubbleType", (int)nextBubbles[1].GetComponent<Bubble>().type);
			args.Add("clearChacknList", clearChacknList);
		}
		if (TutorialManager.Instance.isTutorial(stageNo, TutorialDataTable.ePlace.StageProdBegin))
		{
			stagePause.pause = true;
			tutorialStart();
			yield return StartCoroutine(TutorialManager.Instance.play(stageNo, TutorialDataTable.ePlace.StageProdBegin, uiRoot, stageInfo, args));
			tutorialEnd();
			DialogBase dialogQuit2 = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
			while (dialogQuit2.isOpen())
			{
				yield return null;
			}
			stagePause.pause = false;
		}
		if (cloudList != null)
		{
			foreach (Cloud cloud in cloudList)
			{
				cloud.gameObject.SetActive(true);
			}
			StartCoroutine(cloudUpdateCheck());
		}
		if (cloudList.Count > 0)
		{
			yield return StartCoroutine(setupChameleon());
		}
		if (cloudList.Count > 0)
		{
			inOutObjectCountup();
			MoveOutObjectCheck();
			MoveOutObjectDefend();
		}
		if (stageNo != 0)
		{
			int iconIndex;
			if (bParkStage_ && stageInfo.IsMinilenDelete)
			{
				iconIndex = ((gameType != eGameType.Time) ? 6 : 7);
			}
			else
			{
				iconIndex = ((!stageInfo.IsFriendDelete) ? ((stageInfo.IsAllDelete || stageInfo.IsFulcrumDelete) ? 1 : 2) : 0);
				if (gameType == eGameType.Time)
				{
					iconIndex += 3;
				}
			}
			Transform clearCondition;
			if (stageInfo.IsFriendDelete && clearChacknList.Count > 8)
			{
				clearCondition = frontUi.Find("clear_condition_02");
			}
			else
			{
				clearCondition = frontUi.Find("clear_condition");
				for (int i = 0; i < 6; i++)
				{
					clearCondition.Find("condition_icon/" + i.ToString("00")).gameObject.SetActive(i == iconIndex);
				}
				int paperIndex = (stageInfo.IsMinilenDelete ? 3 : ((stageInfo.IsAllDelete || stageInfo.IsFulcrumDelete) ? 2 : ((!stageInfo.IsFriendDelete) ? 1 : 0)));
				for (int j = 0; j < 4; j++)
				{
					Transform paper_trans = clearCondition.Find("setup_paper/paper_" + j.ToString("00"));
					if ((bool)paper_trans)
					{
						paper_trans.gameObject.SetActive(j == paperIndex);
					}
				}
				clearCondition.Find("Background").gameObject.SetActive(!stageInfo.IsFriendDelete && !stageInfo.IsMinilenDelete);
				clearCondition.Find("Background_chakkun").gameObject.SetActive(stageInfo.IsFriendDelete || stageInfo.IsMinilenDelete);
			}
			if (ResourceLoader.Instance.isUseLowResource())
			{
				clearCondition.gameObject.AddComponent<UIPanel>();
			}
			clearCondition.gameObject.SetActive(true);
			UILabel clearConditionLabel = clearCondition.GetComponentInChildren<UILabel>();
			clearConditionLabel.text = Constant.MessageUtil.getTargetMsg(stageInfo, MessageResource.Instance, Constant.MessageUtil.eTargetType.Game);
			if (stageInfo.IsMinilenDelete)
			{
				clearCondition.Find("condition_icon").gameObject.SetActive(false);
				clearConditionLabel.pivot = UIWidget.Pivot.Center;
				Transform chakkun = clearCondition.Find("chakkun");
				chakkun.gameObject.SetActive(true);
				Transform spRoot = null;
				for (int k = 0; k < chakkun.childCount; k++)
				{
					Transform child2 = chakkun.GetChild(k);
					if (child2.name == clearMinilenList.Count.ToString())
					{
						child2.gameObject.SetActive(true);
						spRoot = child2;
					}
					else
					{
						child2.gameObject.SetActive(false);
					}
				}
				clearMinilenList.Sort();
				for (int n = 0; n < clearMinilenList.Count; n++)
				{
					UISprite sp2 = spRoot.Find("pos_0" + n).GetComponent<UISprite>();
					switch (clearMinilenList[n])
					{
					case Bubble.eType.Red:
						sp2.spriteName = "goal_minilen_00";
						break;
					case Bubble.eType.Green:
						sp2.spriteName = "goal_minilen_01";
						break;
					case Bubble.eType.Blue:
						sp2.spriteName = "goal_minilen_02";
						break;
					case Bubble.eType.Yellow:
						sp2.spriteName = "goal_minilen_03";
						break;
					case Bubble.eType.Orange:
						sp2.spriteName = "goal_minilen_04";
						break;
					case Bubble.eType.Purple:
						sp2.spriteName = "goal_minilen_05";
						break;
					case Bubble.eType.White:
						sp2.spriteName = "goal_minilen_06";
						break;
					case Bubble.eType.Black:
						sp2.spriteName = "goal_minilen_07";
						break;
					case Bubble.eType.MinilenRainbow:
						sp2.spriteName = "goal_minilen_08";
						break;
					}
					sp2.MakePixelPerfect();
				}
			}
			else if (stageInfo.IsFriendDelete)
			{
				if (clearChacknList.Count <= 8)
				{
					clearCondition.Find("condition_icon").gameObject.SetActive(false);
					clearConditionLabel.pivot = UIWidget.Pivot.Center;
				}
				else
				{
					clearConditionLabel.text = clearConditionLabel.text.Replace("\r", string.Empty).Replace("\n", string.Empty).Trim('\u3000', ' ');
				}
				Transform chakkun2 = clearCondition.Find("chakkun");
				chakkun2.gameObject.SetActive(true);
				Transform spRoot2 = null;
				for (int m = 0; m < chakkun2.childCount; m++)
				{
					Transform child = chakkun2.GetChild(m);
					if (child.name == clearChacknList.Count.ToString())
					{
						child.gameObject.SetActive(true);
						spRoot2 = child;
					}
					else
					{
						child.gameObject.SetActive(false);
					}
				}
				clearChacknList.Sort();
				for (int l = 0; l < clearChacknList.Count; l++)
				{
					UISprite sp = spRoot2.Find("pos_0" + l).GetComponent<UISprite>();
					switch (clearChacknList[l])
					{
					case Bubble.eType.Red:
						sp.spriteName = "goal_chakkun_00";
						break;
					case Bubble.eType.Green:
						sp.spriteName = "goal_chakkun_01";
						break;
					case Bubble.eType.Blue:
						sp.spriteName = "goal_chakkun_02";
						break;
					case Bubble.eType.Yellow:
						sp.spriteName = "goal_chakkun_03";
						break;
					case Bubble.eType.Orange:
						sp.spriteName = "goal_chakkun_04";
						break;
					case Bubble.eType.Purple:
						sp.spriteName = "goal_chakkun_05";
						break;
					case Bubble.eType.White:
						sp.spriteName = "goal_chakkun_06";
						break;
					case Bubble.eType.Black:
						sp.spriteName = "goal_chakkun_07";
						break;
					case Bubble.eType.FriendRainbow:
						sp.spriteName = "goal_chakkun_08";
						break;
					case Bubble.eType.FriendBox:
						sp.spriteName = "goal_chakkun_09";
						break;
					}
					sp.MakePixelPerfect();
				}
			}
			GameObject tapObject = clearCondition.transform.Find("Background").gameObject;
			Animation clearConditionAnim = clearCondition.GetComponentInChildren<Animation>();
			clearConditionAnim.clip = clearConditionAnim.GetClip("Clear_condition_in_anm");
			clearConditionAnim.Play();
			while (clearConditionAnim.isPlaying)
			{
				yield return stagePause.sync();
			}
			if (TutorialManager.Instance.isTutorial(stageNo, TutorialDataTable.ePlace.StageProd))
			{
				stagePause.pause = true;
				tutorialStart();
				yield return StartCoroutine(TutorialManager.Instance.play(stageNo, TutorialDataTable.ePlace.StageProd, uiRoot, stageInfo, args));
				tutorialEnd();
				DialogBase dialogQuit7 = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
				while (dialogQuit7.isOpen())
				{
					yield return null;
				}
				stagePause.pause = false;
			}
			yield return StartCoroutine(tapCroutine());
			Constant.SoundUtil.PlayDecideSE();
			tapObject.SetActive(false);
			clearConditionAnim.clip = clearConditionAnim.GetClip("Clear_condition_out_anm");
			clearConditionAnim.Play();
			while (clearConditionAnim.isPlaying)
			{
				yield return stagePause.sync();
			}
			clearCondition.gameObject.SetActive(false);
		}
		if (TutorialManager.Instance.isTutorial(stageNo, TutorialDataTable.ePlace.StageProdEnd))
		{
			stagePause.pause = true;
			tutorialStart();
			yield return StartCoroutine(TutorialManager.Instance.play(stageNo, TutorialDataTable.ePlace.StageProdEnd, uiRoot, stageInfo, args));
			tutorialEnd();
			DialogBase dialogQuit6 = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
			while (dialogQuit6.isOpen())
			{
				yield return null;
			}
			stagePause.pause = false;
		}
		if (bubblePlusNum > 0 || timePlusNum > 0)
		{
			yield return StartCoroutine(bubblePlusEffect());
		}
		if (bShotPlus)
		{
			yield return StartCoroutine(shotPlusEffect());
		}
		if (helpDataList != null && helpDataList.Length > 0)
		{
			yield return StartCoroutine(helpEffect());
		}
		yield return StartCoroutine(beeBarrierEffect());
		yield return StartCoroutine(timeStopEffect());
		yield return StartCoroutine(vacuumEffect());
		if (bCreater)
		{
			CreaterStockNum++;
			Debug.Log("++クリエイターストック数" + CreaterStockNum);
			yield return StartCoroutine(CreaterSkillEff(CreaterStockNum));
		}
		tempReplayDiffTime -= timePlusNum;
		Debug.Log("<color=red>tempReplayDiffTime = " + tempReplayDiffTime + "</color>");
		readyGo.gameObject.SetActive(true);
		yield return StartCoroutine(readyGo.play(stagePause));
		UnityEngine.Object.Destroy(readyGo);
		foreach (Bubble b2 in fieldBubbleList)
		{
			b2.SetBasicSkillIcon(false);
		}
		if (TutorialManager.Instance.isTutorial(stageNo, TutorialDataTable.ePlace.ReadyGoEnd))
		{
			stagePause.pause = true;
			tutorialStart();
			yield return StartCoroutine(TutorialManager.Instance.play(stageNo, TutorialDataTable.ePlace.ReadyGoEnd, uiRoot, stageInfo, args));
			tutorialEnd();
			DialogBase dialogQuit5 = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
			while (dialogQuit5.isOpen())
			{
				yield return null;
			}
			stagePause.pause = false;
		}
		if (stageNo == 150 && Bridge.StageData.getClearCount(stageNo) <= 0)
		{
			TutorialManager.Instance.load(-10, uiRoot);
			stagePause.pause = true;
			tutorialStartObjects(-10);
			yield return StartCoroutine(TutorialManager.Instance.play(-10, TutorialDataTable.ePlace.ReadyGoEnd, uiRoot, null, null));
			tutorialEndObjects(-10);
			DialogBase dialogQuit4 = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
			while (dialogQuit4.isOpen())
			{
				yield return null;
			}
			stagePause.pause = false;
		}
		if (stageNo == 255 && Bridge.StageData.getClearCount(stageNo) <= 0)
		{
			TutorialManager.Instance.load(-11, uiRoot);
			stagePause.pause = true;
			tutorialStartObjects(-11);
			yield return StartCoroutine(TutorialManager.Instance.play(-11, TutorialDataTable.ePlace.ReadyGoEnd, uiRoot, null, null));
			tutorialEndObjects(-11);
			DialogBase dialogQuit3 = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
			while (dialogQuit3.isOpen())
			{
				yield return null;
			}
			stagePause.pause = false;
		}
		SaveOtherData otherData = SaveData.Instance.getGameData().getOtherData();
		if (nearCoinBubbleList_.Count != 0 && !otherData.isFlag(SaveOtherData.eFlg.TutorialCoinBubble))
		{
			TutorialManager.Instance.load(-8, uiRoot);
			stagePause.pause = true;
			foreach (Bubble coin_bubble2 in nearCoinBubbleList_)
			{
				coin_bubble2.myTrans.localPosition += Vector3.back * 50f;
			}
			yield return StartCoroutine(TutorialManager.Instance.play(-8, TutorialDataTable.ePlace.ReadyGoEnd, uiRoot, null, null));
			foreach (Bubble coin_bubble in nearCoinBubbleList_)
			{
				coin_bubble.myTrans.localPosition += Vector3.forward * 50f;
			}
			DialogBase dialogQuit = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
			while (dialogQuit.isOpen())
			{
				yield return null;
			}
			stagePause.pause = false;
			TutorialManager.Instance.unload();
			otherData.setFlag(SaveOtherData.eFlg.TutorialCoinBubble, true);
			otherData.save();
			PlayerPrefs.Save();
		}
		updateGlossTime();
		startTime = Time.time + (float)timePlusNum;
		shotCount = -bubblePlusNum;
		if (gameType == eGameType.ShotCount && !bShowedLastPoint && shotCount >= stageInfo.Move - 5)
		{
			yield return StartCoroutine(lastPoint());
		}
		state = eState.Wait;
		if (bCueBubbleChange)
		{
			setSkillButtonActive(true);
			SkillButtonSetting(CueBubbleChangeRestNum);
			CueBubbleCreate();
		}
		showShootButton();
	}

	private IEnumerator tapCroutine()
	{
		while (stagePause.pause || !Input.GetMouseButtonDown(0))
		{
			yield return 0;
		}
		while (stagePause.pause || !Input.GetMouseButtonUp(0))
		{
			yield return 0;
		}
	}

	private IEnumerator skipRoutine()
	{
		while (true)
		{
			if (!bOPAnime_)
			{
				yield break;
			}
			if (!stagePause.pause && Input.GetMouseButtonDown(0))
			{
				break;
			}
			yield return 0;
		}
		while (true)
		{
			if (!bOPAnime_)
			{
				yield break;
			}
			if (!stagePause.pause && Input.GetMouseButtonUp(0))
			{
				break;
			}
			yield return 0;
		}
		if (bOPAnime_)
		{
			bOPAnime_ = false;
			StartCoroutine(skipOPAnime());
		}
	}

	private IEnumerator skipOPAnime()
	{
		UnityEngine.Object.Destroy(scrollUi.GetComponent<iTween>());
		scrollUi.localPosition = Vector3.zero;
		UnityEngine.Object.Destroy(bubbleRoot.GetComponent<iTween>());
		Vector3 pos = bubbleRoot.localPosition;
		pos.y = bubbleRootPos.y;
		bubbleRoot.localPosition = pos;
		UnityEngine.Object.Destroy(OP_event_00);
		UnityEngine.Object.Destroy(OP_event_01);
		charaObjs[0].SetActive(true);
		charaObjs[1].SetActive(true);
		charaAnims = new tk2dAnimatedSprite[2];
		charaAnims[0] = charaObjs[0].GetComponentInChildren<tk2dAnimatedSprite>();
		charaAnims[1] = charaObjs[1].GetComponentInChildren<tk2dAnimatedSprite>();
		charaAnims[0].Play(waitAnimName);
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "07");
		bgAnim_["OP_BG_anm"].time = bgAnim_["OP_BG_anm"].length;
		yield return new WaitForEndOfFrame();
		Slave slave2 = launchpad_.gameObject.GetComponent<Slave>();
		if (slave2 != null)
		{
			slave2.forceUpdate();
			UnityEngine.Object.Destroy(slave2);
		}
		for (int i = 1; i < nextBubbleCount; i++)
		{
			Slave slave = nextBubbles[i].GetComponent<Slave>();
			if (slave != null)
			{
				slave.forceUpdate();
				UnityEngine.Object.Destroy(slave);
			}
		}
	}

	private IEnumerator playOPAnime()
	{
		bOPAnime_ = true;
		StartCoroutine(skipRoutine());
		launchpad_ = stageUi.Find("launchpad");
		Transform launch_pos = stageBg.Find("floor/launch_pos");
		AvatarSetup();
		BoxCollider col = charaObjs[1].AddComponent<BoxCollider>();
		col.size = new Vector3(120f, 120f, 1f);
		col.center = new Vector3(0f, 65f, 0f);
		col.enabled = false;
		UIButtonMessage button = charaObjs[1].AddComponent<UIButtonMessage>();
		button.target = base.gameObject;
		button.trigger = UIButtonMessage.Trigger.OnClick;
		button.functionName = "OnButton";
		charaObjs[1].SetActive(false);
		bgAnim_ = stageBg.GetComponent<Animation>();
		bgAnim_.clip.SampleAnimation(stageBg.gameObject, 0f);
		bgAnim_.Stop();
		launchpad_.position = launch_pos.position;
		for (int k = 1; k < nextBubbleCount; k++)
		{
			createNextBubble(k, false);
		}
		if (stageNo == 0)
		{
			nextBubbles[1].GetComponent<Bubble>().setType(Bubble.eType.Yellow);
		}
		else if (stageNo == 2)
		{
			nextBubbles[1].GetComponent<Bubble>().setType(Bubble.eType.Red);
		}
		else if (stageNo == 3)
		{
			nextBubbles[1].GetComponent<Bubble>().setType(Bubble.eType.Green);
		}
		for (int l = 1; l < nextBubbleCount; l++)
		{
			Slave slave = nextBubbles[l].AddComponent<Slave>();
			slave.target = nextBubblePoses[l];
		}
		Slave s = launchpad_.gameObject.AddComponent<Slave>();
		s.target = launch_pos;
		float scrollY = bubbleRootPos.y - bubbleRoot.localPosition.y - ceilingBubbleList[0].localPosition.y + 52f;
		if (scrollY > 0f)
		{
			scrollY = 0f;
		}
		scrollUi.localPosition = Vector3.up * scrollY;
		OP_event_00 = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "OP_event_" + charaNames[0])) as GameObject;
		OP_event_01 = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "OP_event_" + charaNames[1])) as GameObject;
		Utility.setParent(OP_event_00, uiRoot.transform, false);
		Utility.setParent(OP_event_01, uiRoot.transform, false);
		OP_event_00.transform.localPosition = Vector3.back * 6f;
		OP_event_01.transform.localPosition = Vector3.back * 6f;
		opAnim_00 = OP_event_00.GetComponent<Animation>();
		opAnim_01 = OP_event_01.GetComponent<Animation>();
		OP_event_00.SetActive(false);
		OP_event_01.SetActive(false);
		float waitTime2 = 1.5f;
		float elapsedTime3 = 0f;
		if (scrollY < 0f)
		{
			while (bOPAnime_ && elapsedTime3 < waitTime2)
			{
				elapsedTime3 += Time.deltaTime;
				yield return stagePause.sync();
			}
		}
		bgAnim_.Play();
		float moveTime = Mathf.Abs(scrollY) / 52f * 0.4f;
		iTween.MoveTo(scrollUi.gameObject, iTween.Hash("y", 0, "easetype", iTween.EaseType.linear, "time", moveTime, "islocal", true));
		waitTime2 = moveTime - opAnim_00["OP_event_anm_00"].length;
		int loopCount = 0;
		while (waitTime2 > opAnim_00["OP_event_anm_01"].length)
		{
			loopCount++;
			waitTime2 -= opAnim_00["OP_event_anm_01"].length;
		}
		elapsedTime3 = 0f;
		while (bOPAnime_ && elapsedTime3 < waitTime2)
		{
			elapsedTime3 += Time.deltaTime;
			yield return stagePause.sync();
		}
		if (OP_event_00 != null && OP_event_01 != null)
		{
			OP_event_00.SetActive(true);
			OP_event_01.SetActive(true);
			opAnim_00.clip = opAnim_00["OP_event_anm_00"].clip;
			opAnim_01.clip = opAnim_01["OP_event_anm_00"].clip;
			opAnim_00.Play("OP_event_anm_00");
			opAnim_01.Play("OP_event_anm_00");
			yield return stagePause.sync();
			yield return stagePause.sync();
			while (bOPAnime_ && opAnim_00["OP_event_anm_00"].time != 0f)
			{
				yield return stagePause.sync();
			}
		}
		for (int j = 0; j < loopCount; j++)
		{
			if (opAnim_00 != null)
			{
				opAnim_00.wrapMode = WrapMode.Once;
				opAnim_01.wrapMode = WrapMode.Once;
				opAnim_00.clip = opAnim_00["OP_event_anm_01"].clip;
				opAnim_01.clip = opAnim_00["OP_event_anm_01"].clip;
				opAnim_00.Play("OP_event_anm_01");
				opAnim_01.Play("OP_event_anm_01");
				yield return stagePause.sync();
				yield return stagePause.sync();
				while (bOPAnime_ && opAnim_00["OP_event_anm_01"].time != 0f)
				{
					yield return stagePause.sync();
				}
			}
		}
		while (bOPAnime_ && scrollUi.GetComponent<iTween>() != null)
		{
			yield return stagePause.sync();
		}
		if (opAnim_00 != null && opAnim_01 != null)
		{
			opAnim_00.clip = opAnim_00["OP_event_anm_02"].clip;
			opAnim_01.clip = opAnim_01["OP_event_anm_02"].clip;
			opAnim_00.Play("OP_event_anm_02");
			opAnim_01.Play("OP_event_anm_02");
			OP_event_00.transform.Find("bubblen/bubblen2").GetComponent<tk2dAnimatedSprite>().Play();
			OP_event_01.transform.Find("bobblen/bobblen2").GetComponent<tk2dAnimatedSprite>().Play();
			OP_event_00.transform.Find("bubblen/bubble2").GetComponent<tk2dAnimatedSprite>().Play();
			OP_event_01.transform.Find("bobblen/bubble2").GetComponent<tk2dAnimatedSprite>().Play();
			elapsedTime3 = 0f;
			while (bOPAnime_ && opAnim_00 != null && opAnim_01 != null && elapsedTime3 < 1.9f)
			{
				elapsedTime3 += Time.deltaTime;
				yield return stagePause.sync();
			}
			Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
			while (bOPAnime_ && opAnim_00 != null && opAnim_01 != null && elapsedTime3 < 2.5f)
			{
				elapsedTime3 += Time.deltaTime;
				yield return stagePause.sync();
			}
			Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
		}
		while (bOPAnime_ && bgAnim_["OP_BG_anm"].time != 0f)
		{
			yield return stagePause.sync();
		}
		if (!bOPAnime_)
		{
			Sound.Instance.stopSe(Sound.eSe.SE_218_hakai);
		}
		if (bOPAnime_)
		{
			for (int i = 1; i < nextBubbleCount; i++)
			{
				UnityEngine.Object.Destroy(nextBubbles[i].GetComponent<Slave>());
			}
			UnityEngine.Object.Destroy(launchpad_.gameObject.GetComponent<Slave>());
		}
		while (bOPAnime_ && opAnim_00 != null && opAnim_00 != null && opAnim_00["OP_event_anm_02"].time != 0f)
		{
			yield return stagePause.sync();
		}
		if (bOPAnime_)
		{
			UnityEngine.Object.Destroy(OP_event_00);
			UnityEngine.Object.Destroy(OP_event_01);
			charaObjs[0].SetActive(true);
			charaObjs[1].SetActive(true);
			charaAnims = new tk2dAnimatedSprite[2];
			charaAnims[0] = charaObjs[0].GetComponentInChildren<tk2dAnimatedSprite>();
			charaAnims[1] = charaObjs[1].GetComponentInChildren<tk2dAnimatedSprite>();
			charaAnims[0].Play(waitAnimName);
			charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "07");
			while (bOPAnime_ && bubbleRoot.GetComponent<iTween>() != null)
			{
				yield return stagePause.sync();
			}
			bOPAnime_ = false;
		}
	}

	public void showShootButton()
	{
		if (state >= eState.Wait)
		{
			SaveOptionData optionData = SaveData.Instance.getSystemData().getOptionData();
			shootButton.SetActive(optionData.getFlag(SaveOptionData.eFlag.ShootButton));
			guide.setShootButton(shootButton.activeSelf);
			if (shootButton.activeSelf)
			{
				shootButton.GetComponent<UIButton>().isEnabled = state == eState.Wait;
			}
		}
	}

	public void fire(Vector3 fireVector)
	{
		if (!shootButton.activeSelf)
		{
			shoot(fireVector);
		}
	}

	private void shoot(Vector3 shootVector)
	{
		arrow.shootAnim();
		bubbleNavi.stopNavi();
		bExcellent = false;
		dropSnakeList.Clear();
		obstacleMoveList.Clear();
		foreach (Bubble item2 in MorganaList_)
		{
			item2.OnDamage_ = false;
			item2.IsChangeColor_ = false;
			foreach (Bubble item3 in item2.ChildMorgana_)
			{
				item3.IsChangeColor_ = false;
			}
		}
		setReplay();
		StageBoostItem item = itemParent_.getItem(Constant.Item.eType.LightningG);
		if ((bool)item)
		{
			old_linghtningGNum = item.getNum();
		}
		else
		{
			old_linghtningGNum = 0;
		}
		if ((bCreater && CreaterStockNum <= 0) || (bCueBubbleChange && CueBubbleChangeRestNum <= 0))
		{
			setSkillButtonActive(false);
		}
		Bubble component = nextBubbles[0].GetComponent<Bubble>();
		prevShotBubbleIndex = 0;
		if (snakeCount_ > 0)
		{
			component = nextBubbles[1].GetComponent<Bubble>();
			prevShotBubbleIndex = 1;
		}
		Vector3 localPosition = component.transform.localPosition;
		localPosition.z = -5f;
		component.transform.localPosition = localPosition;
		component.guide = guide;
		if (component.type != Bubble.eType.Metal)
		{
			Sound.Instance.playSe(Sound.eSe.SE_205_tedama);
		}
		else
		{
			Sound.Instance.playSe(Sound.eSe.SE_400_metalbubble_shot);
		}
		bubble_trail_eff.SetActive(true);
		Utility.setParent(bubble_trail_eff, component.myTrans, false);
		bubble_trail_eff.transform.localPosition = bubble_trail_offset;
		prevShotBubbleType = component.type;
		component.shot(shootVector);
		state = eState.Shot;
		shotCount++;
		realLinkShotCount++;
		stageEnd_ShotCount++;
		if (bCreater)
		{
			CreaterTurnCount++;
		}
	}

	private void shootRandom()
	{
		bubbleNavi.stopNavi();
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "10");
		charaAnims[0].Play(CHARA_SPRITE_ANIMATION_HEADER[0] + "09");
		bExcellent = false;
		dropSnakeList.Clear();
		foreach (Bubble item in MorganaList_)
		{
			item.OnDamage_ = false;
			item.IsChangeColor_ = false;
			foreach (Bubble item2 in item.ChildMorgana_)
			{
				item2.IsChangeColor_ = false;
			}
		}
		Bubble component = nextBubbles[1].GetComponent<Bubble>();
		prevShotBubbleIndex = 0;
		Vector3 localPosition = component.transform.localPosition;
		localPosition.z = -5f;
		component.myTrans.localPosition = localPosition;
		component.guide = guide;
		component.bNoCountCombo = true;
		Sound.Instance.playSe(Sound.eSe.SE_400_metalbubble_shot);
		bubble_trail_eff.SetActive(false);
		Utility.setParent(bubble_trail_eff, component.myTrans, false);
		bubble_trail_eff.transform.localPosition = bubble_trail_offset;
		bubble_trail_eff.SetActive(true);
		component.shot(random_vec);
		state = eState.Shot;
		shotCount++;
		realLinkShotCount++;
		stageEnd_ShotCount++;
	}

	private IEnumerator setRandomRoots()
	{
		Bubble shotBubble = nextBubbles[1].GetComponent<Bubble>();
		random_vec = Vector3.zero;
		BubbleBase hitBubble = null;
		int resarch_count = 0;
		while (hitBubble == null)
		{
			random_vec = arrow.getRandomFireVector();
			hitBubble = guide.setRandomRoot(random_vec, shotBubble.transform.position);
			if (hitBubble == null)
			{
				if (resarch_count < 10)
				{
					resarch_count++;
					continue;
				}
				break;
			}
			yield return stagePause.sync();
			Bubble temp_b = nextBubbles[1].GetComponent<Bubble>();
			Vector3 tem_vec = new Vector3(nextBubbles[1].transform.localPosition.x, nextBubbles[1].transform.localPosition.y, nextBubbles[1].transform.localPosition.z);
			temp_b.myTrans.position = guide.hitPos;
			List<Bubble.eType> nearBubbleTypeList = new List<Bubble.eType>(6);
			for (int i = 0; i < fieldBubbleList.Count; i++)
			{
				Bubble bubble = fieldBubbleList[i];
				if (bubble.state == Bubble.eState.Field && !bubble.isLocked && !bubble.isFrozen && temp_b.isNearBubble(bubble))
				{
					nearBubbleTypeList.Add(bubble.type);
				}
			}
			temp_b.myTrans.localPosition = tem_vec;
			if (hitBubble.type == Bubble.eType.Skull || hitBubble.type == Bubble.eType.Honeycomb || hitBubble.type == Bubble.eType.TunnelIn || nearBubbleTypeList.Contains(Bubble.eType.Search))
			{
				hitBubble = null;
			}
			else
			{
				if (resarch_count >= 10)
				{
					continue;
				}
				Bubble hit_bubble = hitBubble.gameObject.GetComponent<Bubble>();
				if (hit_bubble != null && !hit_bubble.isColorBubble())
				{
					hitBubble = null;
					resarch_count++;
					continue;
				}
				Vector3 temp = nextBubbles[1].transform.position;
				shotBubble.myTrans.position = guide.hitPos;
				float[] checkedFlags = new float[fieldBubbleList.Count];
				List<Bubble> rainbowList = new List<Bubble>();
				checkSamColor(shotBubble, ref checkedFlags, rainbowList);
				float chainCount = 0f;
				float[] array = checkedFlags;
				foreach (float checkedFlag in array)
				{
					if (checkedFlag > 0f)
					{
						chainCount += checkedFlag;
					}
				}
				if (rainbowChainCount == 0)
				{
					chainCount += shotBubble.bubblePower;
				}
				shotBubble.myTrans.position = temp;
				if (chainCount >= 3f)
				{
					hitBubble = null;
					resarch_count++;
				}
			}
		}
	}

	public void hit(Bubble shotBubble, BubbleBase hitBubble)
	{
		if (shotBubble.type != Bubble.eType.Metal && !isHoneycombHitWait)
		{
			if (snakeCount_ > 0)
			{
				charaAnims[1].Play(waitAnimName);
			}
			else
			{
				charaAnims[0].Play(waitAnimName);
			}
		}
		StartCoroutine(hitRoutine(shotBubble, hitBubble));
	}

	private IEnumerator hitRoutine(Bubble shotBubble, BubbleBase hitBubble)
	{
		friendBonusList.Clear();
		minilen_count_pop = 0;
		minilen_count_pop_scored = 0;
		bool existChackn = false;
		foreach (Chackn c in chacknList)
		{
			if (c != null)
			{
				existChackn = true;
				break;
			}
		}
		if (!existChackn)
		{
			chacknList.Clear();
		}
		checkBreakChamList.Clear();
		moveSpan = 0f;
		int prevComboCount = comboCount;
		rainbowChainCount = 0;
		dropSnakeList.Clear();
		Bubble.eType shotBubbleType = shotBubble.type;
		shotBubblePos = shotBubble.myTrans.position;
		Bubble hit_b = null;
		bool bFrozen = false;
		bool bCloud = false;
		if (shotBubbleType != Bubble.eType.Metal && hitBubble != null)
		{
			hit_b = hitBubble.gameObject.GetComponent<Bubble>();
		}
		if (hit_b != null)
		{
			bFrozen = hit_b.isFrozen;
			bCloud = hit_b.inCloud;
		}
		bool isHitSkull = false;
		bool tunnelTwoHit = false;
		if (hitBubble != null && hitBubble.type == Bubble.eType.Skull && !isUsedItem(Constant.Item.eType.SkullBarrier) && !bFrozen && !bCloud)
		{
			isHitSkull = true;
		}
		isSkullGameOver = false;
		isHitHoneycom = hitBubble != null && hitBubble.type == Bubble.eType.Honeycomb && !bFrozen && !isUsedItem(Constant.Item.eType.BeeBarrier) && !bCloud;
		isHitBlackHole = hitBubble != null && (hitBubble.type == Bubble.eType.BlackHole_A || hitBubble.type == Bubble.eType.BlackHole_B) && !bFrozen && !bCloud;
		if (snakeCount_ > 0)
		{
			snakeCount_--;
		}
		if (hitBubble != null && ((hitBubble.type == Bubble.eType.Skull && isUsedItem(Constant.Item.eType.SkullBarrier)) || (hitBubble.type == Bubble.eType.Honeycomb && isUsedItem(Constant.Item.eType.BeeBarrier))) && shotBubbleType != Bubble.eType.Hyper && shotBubbleType != Bubble.eType.Bomb && shotBubbleType != Bubble.eType.Shake && shotBubbleType != Bubble.eType.Metal && shotBubbleType != Bubble.eType.Ice && shotBubbleType != Bubble.eType.Fire && shotBubbleType != Bubble.eType.Water && shotBubbleType != Bubble.eType.Shine && !bFrozen)
		{
			StartCoroutine(playSkullBarrierEff(hitBubble.myTrans.position));
		}
		chainBreakRoutine = null;
		bool isValidFieldBubble = true;
		bPlus = false;
		bMinus = false;
		frozenBreakList.Clear();
		Bubble shotBubble2 = default(Bubble);
		List<Bubble> frozenBubbleList = fieldBubbleList.FindAll((Bubble fieldBubble) => fieldBubble.isFrozen && shotBubble2.isNearBubble(fieldBubble));
		foreach (Bubble b2 in frozenBubbleList)
		{
			checkFrozen(b2, frozenBreakList);
		}
		if (shotBubble != null && !isHitBlackHole)
		{
			yield return StartCoroutine(chameleonCheckRoutine(shotBubble));
		}
		if (shotBubbleType != Bubble.eType.Metal && (hitBubble == null || hitBubble.GetComponent<Bubble>() != null))
		{
			yield return StartCoroutine(setRotateFulcrum(shotBubble, (Bubble)hitBubble));
		}
		switch (shotBubbleType)
		{
		case Bubble.eType.Hyper:
			isValidFieldBubble = false;
			yield return StartCoroutine(hyperRoutine(shotBubble));
			break;
		case Bubble.eType.Bomb:
			isValidFieldBubble = false;
			yield return StartCoroutine(bombRoutine(shotBubble));
			break;
		case Bubble.eType.Shake:
			isValidFieldBubble = false;
			yield return StartCoroutine(shakeRoutine(shotBubble));
			break;
		case Bubble.eType.Metal:
			isValidFieldBubble = false;
			yield return StartCoroutine(metalRoutine(shotBubble));
			break;
		case Bubble.eType.Ice:
			isValidFieldBubble = false;
			yield return StartCoroutine(iceRoutine(shotBubble));
			break;
		case Bubble.eType.Fire:
			isValidFieldBubble = false;
			yield return StartCoroutine(fireRoutine(shotBubble));
			break;
		case Bubble.eType.Water:
			isValidFieldBubble = false;
			yield return StartCoroutine(waterRoutine(shotBubble));
			break;
		case Bubble.eType.Shine:
			isValidFieldBubble = false;
			yield return StartCoroutine(shineRoutine(shotBubble));
			break;
		case Bubble.eType.LightningG_Item:
			isValidFieldBubble = false;
			yield return StartCoroutine(lightningGRoutine(shotBubble));
			break;
		}
		if (!isHitHoneycom && !isHitSkull && !isHitBlackHole)
		{
			yield return StartCoroutine(chameleonChangeRoutine());
		}
		if (isValidFieldBubble)
		{
			if (isHitHoneycom)
			{
				if (!shotBubble.bNoCountCombo)
				{
					if (bComboMaster && ComboMasterCount > 0 && comboCount >= 2)
					{
						Sound.Instance.playSe(Sound.eSe.SE_357_readygo);
						ComboMasterCount--;
						popupCombo[popupComboIndex].GetComponent<PopupCombo>().startFlashPopup(comboCount, ComboMasterCount, shotBubblePos);
						popupComboIndex = (popupComboIndex + 1) % 2;
					}
					else
					{
						if (comboCount >= 2)
						{
							comboOut.Reset();
							comboOut.Play(true);
						}
						comboCount = 0;
					}
				}
				else if (!bComboMaster)
				{
					if (comboCount >= 2)
					{
						comboOut.Reset();
						comboOut.Play(true);
					}
					comboCount = 0;
				}
				if (shotBubble.isBasicSkillColor && bBasicSkill)
				{
					basicSkillBreakNum--;
				}
				if (gameType != 0 || shotCount < stageInfo.Move)
				{
					totalDropCount = 0;
					totalSkillBonusCount = 0;
					StartCoroutine(honeycombRoutine(shotBubble, hitBubble));
					yield return StartCoroutine(updateTimeStop(true, false));
					yield return StartCoroutine(updateVacuum(true, false));
					inOutObjectCountup();
					MoveOutObjectCheck();
					MoveOutObjectDefend();
					yield break;
				}
				shotBubble.startBreak();
			}
			if (hitBubble != null && hitBubble.type >= Bubble.eType.MorganaRed && hitBubble.type <= Bubble.eType.MorganaBlack && shotBubble.type != hitBubble.type - 109)
			{
				MorganaFieldBubbleColorCheck(hitBubble as Bubble);
			}
			if (isHitBlackHole)
			{
				yield return StartCoroutine(blackHoleRoutine(shotBubble, hitBubble));
			}
			if (shotBubble != null)
			{
				yield return StartCoroutine(chameleonCheckRoutine(shotBubble));
			}
			if (!isSkullGameOver && isHitSkull)
			{
				if (shotBubble.isBasicSkillColor && bBasicSkill)
				{
					basicSkillBreakNum--;
				}
				Sound.Instance.playSe(Sound.eSe.SE_338_skulbubble);
				isSkullGameOver = true;
				shotBubble.startBreak(true, false);
				((Bubble)hitBubble).startBreak(true, false);
			}
			if (hitBubble != null && hitBubble.type == Bubble.eType.TunnelIn)
			{
				shotBubble.startBreak(true, false);
				Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
				tunnelTwoHit = true;
			}
			if (!isSkullGameOver && !isHitHoneycom && !isHitBlackHole && !tunnelTwoHit)
			{
				bSearch = false;
				if (shotBubble == hitBubble)
				{
					shotBubble.startBreak(true, false);
					Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
				}
				else
				{
					yield return StartCoroutine(hitDefault(shotBubble));
				}
				if (bSearch)
				{
					yield return StartCoroutine(searchBubbleRoutine());
				}
			}
		}
		yield return StartCoroutine(checkBreakCameleon(shotBubble, prevComboCount));
		if (shotBubble.bNoCountCombo && comboCount < prevComboCount && bComboMaster)
		{
			comboCount = prevComboCount;
		}
		if (bComboMaster && ComboMasterCount > 0 && comboCount == 0 && prevComboCount >= 2)
		{
			comboCount = prevComboCount;
			Sound.Instance.playSe(Sound.eSe.SE_357_readygo);
			ComboMasterCount--;
			popupCombo[popupComboIndex].GetComponent<PopupCombo>().startFlashPopup(comboCount, ComboMasterCount, shotBubblePos);
			popupComboIndex = (popupComboIndex + 1) % 2;
		}
		if (prevComboCount != comboCount)
		{
			if (comboCount == 0 && prevComboCount >= 2)
			{
				comboOut.Reset();
				comboOut.Play(true);
			}
			else
			{
				comboLabel.text = comboCount.ToString();
				comboAnim.Play();
				if (comboCount == 2)
				{
					comboIn.Reset();
					comboIn.Play(true);
				}
			}
		}
		updateBuyItem();
		bool CheckCounterOver2 = false;
		isCounterGameOver = false;
		if ((cloudList == null || cloudList.Count == 0) && (rotateFulcrumList == null || rotateFulcrumList.Count == 0) && !isHoneycombHitWait && !bUsingTimeStop)
		{
			startCountdownCounters();
			while (isPlayingCounterAnim())
			{
				yield return stagePause.sync();
			}
			isCounterGameOver = isCounterOver();
			CheckCounterOver2 = true;
		}
		totalDropCount = 0;
		totalSkillBonusCount = 0;
		if (isSkullGameOver)
		{
			if (hitBubble != null)
			{
				foreach (ReplayData rd in replayDataList)
				{
					if (rd.type != Bubble.eType.Skull || (rd.pos - hitBubble.myTrans.localPosition).sqrMagnitude > 1f)
					{
						continue;
					}
					rd.type = shotBubble.type;
					break;
				}
			}
			updateFieldBubbleList();
			while (hitBubble != null)
			{
				yield return stagePause.sync();
			}
			gameoverType = eGameover.HitSkull;
			StartCoroutine(gameoverRoutine());
			isHoneycombHitWait = false;
			yield break;
		}
		if (chainBreakRoutine != null)
		{
			yield return chainBreakRoutine;
			updateChainLock();
			updateOnChain();
			chainBreakRoutine = null;
		}
		if (lightningEffectRoutine != null)
		{
			yield return lightningEffectRoutine;
		}
		if (fireEffectRoutine != null)
		{
			yield return fireEffectRoutine;
		}
		yield return StartCoroutine(updateRotateBubble(shotBubble));
		moveIvies();
		if (!checkClear(true) && !isHoneycombHitWait)
		{
			updateShootCharacter(false);
		}
		yield return StartCoroutine(scrollRoutine());
		if (cloudList != null && cloudList.Count > 0 && !isHitBlackHole)
		{
			if (shotBubble.isHitCloud)
			{
				if (shotBubble != null)
				{
					if (shotBubbleType >= Bubble.eType.Red && shotBubbleType <= Bubble.eType.Black && !Sound.Instance.isPlayingSe(Sound.eSe.SE_218_hakai))
					{
						Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
					}
					shotBubble.startBreak();
				}
				yield return StartCoroutine(cloudMoveDiffCheck());
				foreach (Cloud cloud in cloudList)
				{
					cloud.isCloudMove = false;
				}
				yield return StartCoroutine(cloudUpdateCheck());
				cloudHitPosList_.Clear();
			}
			if (!isHoneycombHitWait && !bUsingTimeStop)
			{
				startCountdownCounters();
				while (isPlayingCounterAnim())
				{
					yield return stagePause.sync();
				}
				isCounterGameOver = isCounterOver();
				CheckCounterOver2 = true;
			}
		}
		else if (cloudList != null && cloudList.Count > 0 && isHitBlackHole && !isHoneycombHitWait && !bUsingTimeStop)
		{
			startCountdownCounters();
			while (isPlayingCounterAnim())
			{
				yield return stagePause.sync();
			}
			isCounterGameOver = isCounterOver();
			CheckCounterOver2 = true;
		}
		if (!CheckCounterOver2 && !isHoneycombHitWait && !bUsingTimeStop)
		{
			startCountdownCounters();
			while (isPlayingCounterAnim())
			{
				yield return stagePause.sync();
			}
			isCounterGameOver = isCounterOver();
			CheckCounterOver2 = true;
		}
		if (gameoverType == eGameover.MinilenVanish)
		{
			StartCoroutine(gameoverRoutine());
			isHoneycombHitWait = false;
			yield break;
		}
		if (grow())
		{
			StartCoroutine(gameoverRoutine());
			isHoneycombHitWait = false;
			yield break;
		}
		yield return StartCoroutine(obstacleMoveRight());
		yield return StartCoroutine(obstacleWait());
		if (checkClear(true))
		{
			if (gameType == eGameType.Time)
			{
				if (stageInfo.IsFriendDelete || stageInfo.IsMinilenDelete)
				{
					int mCount3 = 0;
					int MORGANA_CHILD_NUM3 = 6;
					foreach (Bubble b4 in fieldBubbleList)
					{
						if (b4.IsMorganaParent_)
						{
							mCount3++;
						}
					}
					int dropScore3 = (fieldBubbleList.Count - 9 - mCount3 * MORGANA_CHILD_NUM3) * DROP_SCORE;
					popupScoreDrop_clear.startPopup(dropScore3, bonusClearPos, Vector3.zero);
					totalScore += dropScore3;
					nextScore = totalScore;
				}
				stagePause.pause = true;
				Sound.Instance.playSe(Sound.eSe.SE_104_Great);
				Sound.Instance.playSe(Sound.eSe.SE_113_Clap);
				Transform mission_completed = frontUi.Find("mission_completed");
				mission_completed.gameObject.SetActive(true);
				UILabel Clear_Label = mission_completed.transform.Find("Clear_Label").GetComponent<UILabel>();
				Clear_Label.text = Constant.MessageUtil.getTargetMsg(stageInfo, MessageResource.Instance, Constant.MessageUtil.eTargetType.Clear);
				Animation mission_completedAnim = mission_completed.GetComponent<Animation>();
				mission_completedAnim.Play("Clear_condition_in_anm");
				while (mission_completedAnim.isPlaying)
				{
					yield return null;
				}
				float wait = 0f;
				while (wait < cutinWaitTime)
				{
					if (Input.GetMouseButtonUp(0))
					{
						Constant.SoundUtil.PlayDecideSE();
						break;
					}
					wait += Time.deltaTime;
					yield return null;
				}
				mission_completedAnim.Play("Clear_condition_out_anm");
				while (mission_completedAnim.isPlaying)
				{
					yield return null;
				}
				mission_completed.gameObject.SetActive(false);
				DialogBase dialogQuit = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
				while (dialogQuit.isOpen())
				{
					yield return null;
				}
				stagePause.pause = false;
			}
			else
			{
				int mCount2 = 0;
				int MORGANA_CHILD_NUM2 = 6;
				foreach (Bubble b3 in fieldBubbleList)
				{
					if (b3.IsMorganaParent_)
					{
						mCount2++;
					}
				}
				int dropScore2 = (fieldBubbleList.Count - 9 - mCount2 * MORGANA_CHILD_NUM2) * DROP_SCORE;
				popupScoreDrop_clear.startPopup(dropScore2, bonusClearPos, Vector3.zero);
				totalScore += dropScore2;
				nextScore = totalScore;
			}
			StartCoroutine(clearRoutine());
			yield break;
		}
		if (checkClear(false) && totalScore < stageInfo.Score && gameType == eGameType.ShotCount)
		{
			int mCount = 0;
			int MORGANA_CHILD_NUM = 6;
			foreach (Bubble b in fieldBubbleList)
			{
				if (b.IsMorganaParent_)
				{
					mCount++;
				}
			}
			int dropScore = (fieldBubbleList.Count - 9 - mCount * MORGANA_CHILD_NUM) * DROP_SCORE;
			popupScoreDrop_clear.startPopup(dropScore, bonusClearPos, Vector3.zero);
			totalScore += dropScore;
			nextScore = totalScore;
			if (totalScore >= stageInfo.Score)
			{
				StartCoroutine(clearRoutine());
				yield break;
			}
			gameoverType = eGameover.ScoreNotEnough;
			StartCoroutine(gameoverRoutine());
			yield break;
		}
		if (gameType == eGameType.ShotCount && shotCount >= stageInfo.Move)
		{
			gameoverType = eGameover.ShotCountOver;
			yield return StartCoroutine(gameoverRoutine());
			if (state == eState.Result)
			{
				yield break;
			}
			isCounterGameOver = false;
		}
		if (isCounterGameOver)
		{
			gameoverType = eGameover.CounterOver;
			yield return StartCoroutine(countDownOverEff());
			yield return StartCoroutine(gameoverRoutine());
			yield break;
		}
		if (isHoneycombHitWait || bUsingTimeStop)
		{
			if (obstacleUpdate())
			{
				yield return StartCoroutine(obstacleWait());
			}
			yield return StartCoroutine(obstacleMoveRight());
			yield return StartCoroutine(obstacleWait());
		}
		while (_droped_lightning_g.Exists((Bubble l) => l != null))
		{
			yield return 0;
		}
		_droped_lightning_g.Clear();
		if (isHoneycombHitWait)
		{
			isHoneycombHitWait = false;
			yield break;
		}
		for (int i = 0; i < dropSnakeList.Count; i++)
		{
			yield return stagePause.sync();
			if (dropSnakeList[i] != null)
			{
				i = 0;
			}
		}
		updateSnake();
		while (snakeAliveAnimation)
		{
			yield return stagePause.sync();
		}
		if (!bUsingTimeStop)
		{
			if (obstacleUpdate())
			{
				yield return StartCoroutine(obstacleWait());
			}
			yield return StartCoroutine(obstacleMoveRight());
			yield return StartCoroutine(obstacleWait());
			obstacleCountup();
			if (obstacleCount >= 1)
			{
				yield return StartCoroutine(MoveObstacleDefend());
			}
			yield return StartCoroutine(obstacleMoveRight());
			yield return StartCoroutine(obstacleWait());
			yield return StartCoroutine(updateChameleon());
		}
		inOutObjectCountup();
		MoveOutObjectCheck();
		MoveOutObjectDefend();
		yield return StartCoroutine(updateTimeStop(true, true));
		yield return StartCoroutine(updateVacuum(true, true));
		searchNextBubble();
		yield return StartCoroutine(updateLineFriendBubble());
		yield return StartCoroutine(updateKeyBubble());
		yield return StartCoroutine(MinilenBubbleDropTutorial());
		while (dropSnakeList.Count > 0 && arrow.charaIndex != 0 && charaAnims[1].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[1] + "10"))
		{
			yield return stagePause.sync();
		}
		if (bCreater)
		{
			CreaterUiUpdate();
			if (CreaterTurnCount % CreaterExTurn == 0 && !isHoneyCreater)
			{
				CreaterStockNum++;
				Debug.Log("++ボムストック数" + CreaterStockNum);
				bCreaterSkillEffect = true;
				yield return StartCoroutine(CreaterSkillEff(CreaterStockNum));
			}
			isHoneyCreater = false;
			isCreaterSkillUse = false;
		}
		if (snakeCount_ > 0)
		{
			yield return StartCoroutine(stepNextBubbleBobblen());
		}
		else
		{
			yield return StartCoroutine(stepNextBubble());
		}
		if ((bCreater && CreaterStockNum > 0) || bCueBubbleChange)
		{
			setSkillButtonActive(true);
		}
		if (bCueBubbleChange)
		{
			CueBubbleCreate();
			isCueBubbleChangeUse = false;
		}
		yield return stagePause.sync();
		if (tayunCoroutine != null)
		{
			yield return tayunCoroutine;
		}
		tayunCoroutine = null;
		if (guide.isShootButton)
		{
			guide.lineUpdate();
		}
		if (itemReplay_ != null && !isCounterGameOver)
		{
			itemReplay_.setStateFixed(false);
			itemReplay_.enable();
			itemReplay_.reset();
		}
		if (gameType == eGameType.ShotCount && !bShowedLastPoint && shotCount >= stageInfo.Move - 5)
		{
			yield return StartCoroutine(lastPoint());
		}
		checkTimeStopItemEnable();
		checkVacuumEnable();
		state = eState.Wait;
	}

	private IEnumerator honeycombRoutine(Bubble shotBubble, BubbleBase hitBubble)
	{
		shotBubble.startBreak(true, false);
		while (shotBubble != null)
		{
			yield return 0;
		}
		if (bCreater)
		{
			CreaterUiUpdate();
			if (CreaterTurnCount % CreaterExTurn == 0)
			{
				CreaterStockNum++;
				Debug.Log("++ボムストック数" + CreaterStockNum);
				isHoneyCreater = true;
				bCreaterSkillEffect = true;
				yield return StartCoroutine(CreaterSkillEff(CreaterStockNum));
			}
			isCreaterSkillUse = false;
		}
		updateSnake();
		if (!bUsingTimeStop && !timestop_counter.activeSelf)
		{
			startCountdownCounters();
			while (isPlayingCounterAnim())
			{
				yield return stagePause.sync();
			}
			isCounterGameOver = isCounterOver();
			StartCoroutine(updateChameleon());
		}
		if (!isCounterGameOver)
		{
			Sound.Instance.playSe(Sound.eSe.SE_503_bee, false);
			honeycomb_num = 3;
			((Bubble)hitBubble).isHit = true;
			hitHoneycombEff = hitBubble.gameObject.transform.Find("bee_eff(Clone)").gameObject;
			Vector3 eff_temp_pos = new Vector3(hitHoneycombEff.transform.localPosition.x, hitHoneycombEff.transform.localPosition.y, hitHoneycombEff.transform.localPosition.z);
			Transform bee_parent = hitHoneycombEff.transform.parent;
			hitHoneycombEff.transform.parent = hitHoneycombEff.transform.parent.parent.parent;
			GameObject bee_sub = hitHoneycombEff.transform.Find("bee_sub").gameObject;
			if (prevShotBubbleIndex == 1)
			{
				createNextBubble(1, false);
			}
			float bee_eff_offset_y = 0.2f;
			float bee_eff_offset_z = -0.01f;
			Vector3 chare_01_pos = charaObjs[1].transform.localPosition;
			charaObjs[1].transform.localPosition = new Vector3(charaObjs[1].transform.localPosition.x, charaObjs[1].transform.localPosition.y, charaObjs[1].transform.localPosition.z + 1f);
			Vector3 target_pos_ = new Vector3(charaObjs[1].transform.position.x, charaObjs[1].transform.position.y + bee_eff_offset_y, charaObjs[1].transform.position.z + bee_eff_offset_z);
			Vector3 target_pos_0 = new Vector3(charaObjs[0].transform.position.x, charaObjs[0].transform.position.y + bee_eff_offset_y, charaObjs[0].transform.position.z + bee_eff_offset_z);
			Vector3 target_pos = target_pos_ + (target_pos_0 - target_pos_) * 0.5f;
			target_pos.z = target_pos_.z;
			hitHoneycombEff.GetComponent<Animation>().clip = hitHoneycombEff.GetComponent<Animation>()["Bee_anm_00"].clip;
			hitHoneycombEff.GetComponent<Animation>().CrossFade("Bee_anm_00");
			iTween.MoveTo(hitHoneycombEff, iTween.Hash("position", target_pos, "time", 1f, "easetype", iTween.EaseType.easeOutQuad));
			UISprite[] sprites = bee_sub.GetComponentsInChildren<UISprite>(true);
			bee_sub.SetActive(true);
			float alpha = 0f;
			while (alpha < 1f)
			{
				UISprite[] array = sprites;
				foreach (UISprite s in array)
				{
					Color c = s.color;
					c.a = alpha;
					s.color = c;
				}
				yield return stagePause.sync();
				alpha += Time.deltaTime * 2f;
				if (alpha > 1f)
				{
					alpha = 1f;
				}
			}
			UISprite[] array2 = sprites;
			foreach (UISprite s2 in array2)
			{
				Color c2 = s2.color;
				c2.a = alpha;
				s2.color = c2;
			}
			while (hitHoneycombEff != null && hitHoneycombEff.GetComponent<iTween>() != null)
			{
				yield return stagePause.sync();
			}
			hitHoneycombEff.GetComponent<Animation>().clip = hitHoneycombEff.GetComponent<Animation>()["Bee_anm_01"].clip;
			hitHoneycombEff.GetComponent<Animation>().CrossFade("Bee_anm_01");
			while (honeycomb_num > 0)
			{
				yield return StartCoroutine(setRandomRoots());
				isHoneycombHitWait = true;
				shootRandom();
				while (isHoneycombHitWait)
				{
					yield return stagePause.sync();
				}
				if (state == eState.Clear)
				{
					Sound.Instance.stopSe(Sound.eSe.SE_503_bee);
					Ivy.setIvySe(false);
					alpha = 1f;
					while (alpha > 0f)
					{
						UISprite[] array3 = sprites;
						foreach (UISprite s3 in array3)
						{
							Color c3 = s3.color;
							c3.a = alpha;
							s3.color = c3;
							hitHoneycombEff.transform.Find("bee").GetComponent<UISprite>().color = c3;
						}
						yield return stagePause.sync();
						alpha -= Time.deltaTime * 2f;
						if (alpha < 0f)
						{
							alpha = 0f;
						}
					}
					yield break;
				}
				float waitTime = 0f;
				while (waitTime < 0.3f)
				{
					waitTime += Time.deltaTime;
					yield return stagePause.sync();
				}
				createNextBubble(1, false);
				if (honeycomb_num == -1)
				{
					Sound.Instance.stopSe(Sound.eSe.SE_503_bee);
					if (hitBubble != null)
					{
						((Bubble)hitBubble).isHit = false;
					}
					Ivy.setIvySe(false);
					hitHoneycombEff.transform.parent = bee_parent;
					nextBubbles[1].SetActive(false);
					if (hitHoneycombEff != null)
					{
						hitHoneycombEff.transform.localPosition = eff_temp_pos;
					}
					bee_sub.SetActive(false);
					if (!bTimeStopEffectEnded)
					{
						GameObject dataTable2 = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
						playBGM(dataTable2);
					}
					updateTimeStopCount();
					checkTimeStopItemEnable();
					if (!bVacummeEffectEnded)
					{
						foreach (Cloud cloud in cloudList)
						{
							cloud.resurrection();
						}
						yield return StartCoroutine(cloudUpdateCheck());
					}
					updateVacuumCount();
					checkVacuumEnable();
					yield break;
				}
				honeycomb_num--;
				updateSnake();
			}
			arrow.revertPreFireVector();
			if (guide.isShootButton)
			{
				guide.lineUpdate();
			}
			if (hitHoneycombEff != null)
			{
				if (hitBubble != null && ((Bubble)hitBubble).state == Bubble.eState.Field)
				{
					if (hitBubble != null)
					{
						((Bubble)hitBubble).isHit = false;
					}
					hitHoneycombEff.transform.parent = bee_parent;
					hitHoneycombEff.GetComponent<Animation>().wrapMode = WrapMode.Loop;
					hitHoneycombEff.GetComponent<Animation>().clip = hitHoneycombEff.GetComponent<Animation>()["Bee_anm_00"].clip;
					hitHoneycombEff.GetComponent<Animation>().CrossFade("Bee_anm_00");
					iTween.MoveTo(hitHoneycombEff, iTween.Hash("position", hitBubble.gameObject.transform.position, "time", 1f, "easetype", iTween.EaseType.easeOutQuad));
					float waitTime2 = 0f;
					while (waitTime2 < 0.4f)
					{
						waitTime2 += Time.deltaTime;
						yield return stagePause.sync();
					}
					alpha = 1f;
					while (alpha > 0f)
					{
						UISprite[] array4 = sprites;
						foreach (UISprite s5 in array4)
						{
							Color c5 = s5.color;
							c5.a = alpha;
							s5.color = c5;
						}
						yield return stagePause.sync();
						alpha -= Time.deltaTime * 2f;
						if (alpha < 0f)
						{
							alpha = 0f;
						}
					}
					bee_sub.SetActive(false);
					target_pos = hitBubble.gameObject.transform.position;
					while (hitHoneycombEff != null && hitHoneycombEff.GetComponent<iTween>() != null)
					{
						yield return stagePause.sync();
					}
					UnityEngine.Object.Destroy(hitHoneycombEff.GetComponent<iTween>());
					hitHoneycombEff.transform.localPosition = eff_temp_pos;
					hitHoneycombEff.GetComponent<Animation>().clip = hitHoneycombEff.GetComponent<Animation>()["Bee_anm"].clip;
					hitHoneycombEff.GetComponent<Animation>().CrossFade("Bee_anm");
					GameObject[] bees = GameObject.FindGameObjectsWithTag("Bee");
					GameObject[] array5 = bees;
					foreach (GameObject bee in array5)
					{
						if (!(bee == hitHoneycombEff))
						{
							hitHoneycombEff.GetComponent<Animation>()["Bee_anm"].time = bee.GetComponent<Animation>()["Bee_anm"].time;
							break;
						}
					}
				}
				else
				{
					alpha = 1f;
					while (alpha > 0f)
					{
						UISprite[] array6 = sprites;
						foreach (UISprite s4 in array6)
						{
							Color c4 = s4.color;
							c4.a = alpha;
							s4.color = c4;
							hitHoneycombEff.transform.Find("bee").GetComponent<UISprite>().color = c4;
						}
						yield return stagePause.sync();
						alpha -= Time.deltaTime * 2f;
						if (alpha < 0f)
						{
							alpha = 0f;
						}
					}
					UnityEngine.Object.Destroy(hitHoneycombEff);
				}
			}
			if (!bUsingTimeStop && !timestop_counter.activeSelf)
			{
				if (obstacleUpdate())
				{
					yield return StartCoroutine(obstacleWait());
				}
				yield return StartCoroutine(obstacleMoveRight());
				yield return StartCoroutine(obstacleWait());
				obstacleCountup();
				if (obstacleCount >= 1)
				{
					yield return StartCoroutine(MoveObstacleDefend());
				}
				yield return StartCoroutine(obstacleMoveRight());
				yield return StartCoroutine(obstacleWait());
			}
			inOutObjectCountup();
			MoveOutObjectCheck();
			MoveOutObjectDefend();
			if (!bTimeStopEffectEnded)
			{
				yield return StartCoroutine(timeStopEndAnimation());
				GameObject dataTable3 = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
				playBGM(dataTable3);
				setFieldBubbleTimeStop(false);
			}
			updateTimeStopCount();
			if (!bVacummeEffectEnded)
			{
				yield return StartCoroutine(vacuumAnimation_END());
				yield return StartCoroutine(cloudUpdateCheck());
			}
			updateVacuumCount();
			charaObjs[1].transform.localPosition = chare_01_pos;
			updateShootCharacter(true);
			if (arrow.charaIndex != 0 && nextBubbles[0] == null)
			{
				createNextBubble(0, false);
				nextBubbles[0].SetActive(false);
			}
			honeycomb_num = -1;
			Sound.Instance.stopSe(Sound.eSe.SE_503_bee);
		}
		else
		{
			arrow.revertPreFireVector();
			honeycomb_num = -1;
			if (gameType == eGameType.ShotCount && shotCount >= stageInfo.Move)
			{
				gameoverType = eGameover.ShotCountOver;
				yield return StartCoroutine(gameoverRoutine());
				if (state == eState.Result)
				{
					yield break;
				}
				updateTimeStopCount();
				checkTimeStopItemEnable();
				updateVacuumCount();
				checkVacuumEnable();
				isCounterGameOver = false;
			}
			if (isCounterGameOver)
			{
				gameoverType = eGameover.CounterOver;
				updateTimeStopCount();
				checkTimeStopItemEnable();
				updateVacuumCount();
				checkVacuumEnable();
				yield return StartCoroutine(countDownOverEff());
				yield return StartCoroutine(gameoverRoutine());
				yield break;
			}
			if (!bTimeStopEffectEnded)
			{
				yield return StartCoroutine(timeStopEndAnimation());
				GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
				playBGM(dataTable);
				setFieldBubbleTimeStop(false);
			}
			updateTimeStopCount();
			if (!bVacummeEffectEnded)
			{
				yield return StartCoroutine(vacuumAnimation_END());
				yield return StartCoroutine(cloudUpdateCheck());
			}
			updateVacuumCount();
		}
		if (guide.isShootButton)
		{
			guide.lineUpdate();
		}
		searchNextBubble();
		while (snakeAliveAnimation)
		{
			yield return stagePause.sync();
		}
		if (snakeCount_ > 0)
		{
			nextBubbleHideForShotCountBobllen();
		}
		StartCoroutine(cloudUpdateCheck());
		yield return StartCoroutine(updateLineFriendBubble());
		if (arrow.charaIndex == 0)
		{
			prevShotBubbleIndex = 0;
			charaAnims[0].Play(waitAnimName);
			yield return StartCoroutine(stepNextBubble());
		}
		if (bCueBubbleChange)
		{
			CueBubbleCreate();
			isCueBubbleChangeUse = false;
		}
		yield return stagePause.sync();
		if (tayunCoroutine != null)
		{
			yield return tayunCoroutine;
		}
		tayunCoroutine = null;
		if (itemReplay_ != null)
		{
			itemReplay_.setStateFixed(false);
			itemReplay_.enable();
			itemReplay_.reset();
		}
		if (gameType == eGameType.ShotCount && !bShowedLastPoint && shotCount >= stageInfo.Move - 5)
		{
			yield return StartCoroutine(lastPoint());
		}
		checkTimeStopItemEnable();
		checkVacuumEnable();
		state = eState.Wait;
	}

	private IEnumerator blackHoleRoutine(Bubble shotBubble, BubbleBase hitBubble)
	{
		GameObject AS_spr = shotBubble.transform.Find("AS_spr_bubble").gameObject;
		int shot_type = (int)shotBubble.type;
		Sound.Instance.playSe(Sound.eSe.SE_534_blackhole);
		shotBubble.myTrans.localPosition += Vector3.back;
		hitBubble.myTrans.localPosition += Vector3.back;
		iTween.ScaleTo(hitBubble.gameObject, iTween.Hash("x", 1.5f, "y", 1.5f, "time", 0.6f, "easetype", iTween.EaseType.easeOutQuad));
		AS_spr.transform.localPosition = shotBubble.myTrans.localPosition - hitBubble.myTrans.localPosition;
		shotBubble.myTrans.localPosition = new Vector3(hitBubble.myTrans.localPosition.x, hitBubble.myTrans.localPosition.y, hitBubble.myTrans.localPosition.z - 0.5f);
		iTween.ShakePosition(shotBubble.gameObject, iTween.Hash("x", 0.02f, "y", 0.01f, "time", 1f, "easetype", iTween.EaseType.linear));
		iTween.ScaleTo(shotBubble.gameObject, iTween.Hash("x", 0f, "y", 0f, "z", 0f, "time", 1f, "easetype", iTween.EaseType.linear));
		while (shotBubble.GetComponent<iTween>() != null)
		{
			yield return stagePause.sync();
		}
		GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		GimmickDataTable gimmickTbl = dataTable.GetComponent<GimmickDataTable>();
		Bubble bubble_hit = hitBubble.gameObject.GetComponent<Bubble>();
		GimmickPercentInfo.BlackHoleInfo gimmickDataInfo = ((bubble_hit.type != Bubble.eType.BlackHole_A) ? gimmickTbl.getGimmickInfo().BlackHole[1] : gimmickTbl.getGimmickInfo().BlackHole[0]);
		int seed = random.Next(100);
		List<int> targetList = new List<int>();
		targetList.Clear();
		if (seed <= gimmickDataInfo.MinusPercent)
		{
			GimmickPercentInfo.MinusInfo[] minusTable = gimmickDataInfo.MinusTable;
			foreach (GimmickPercentInfo.MinusInfo data2 in minusTable)
			{
				for (int j = 0; j < data2.MinusTypePercent; j++)
				{
					targetList.Add(data2.MinusType);
				}
			}
		}
		else
		{
			GimmickPercentInfo.PlusInfo[] plusTable = gimmickDataInfo.PlusTable;
			foreach (GimmickPercentInfo.PlusInfo data in plusTable)
			{
				for (int i = 0; i < data.PlusTypePercent; i++)
				{
					targetList.Add(data.PlusType);
				}
			}
		}
		seed = random.Next(targetList.Count);
		Bubble.eType randType = (Bubble.eType)targetList[seed];
		if (randType == Bubble.eType.MinusRed || randType == Bubble.eType.PlusRed)
		{
			if (isColorBubble(shot_type))
			{
				randType += shot_type;
			}
			else
			{
				List<Bubble.eType> colorList = new List<Bubble.eType>();
				colorList.Clear();
				foreach (Bubble b in fieldBubbleList)
				{
					if (isColorBubble((int)b.type) && !b.isFrozen)
					{
						Bubble.eType type = convertColorBubble(b.type);
						if (!colorList.Contains(type))
						{
							colorList.Add(type);
						}
					}
				}
				randType = (Bubble.eType)((int)randType + (int)colorList[random.Next(colorList.Count)]);
			}
		}
		Utility.setParent(shotBubble.gameObject, hitBubble.transform.parent, false);
		Vector3 tempPos = hitBubble.transform.localPosition;
		tempPos.z = 0f;
		shotBubble.myTrans.localPosition = tempPos;
		AS_spr.transform.localPosition = Vector3.zero;
		shotBubble.setType(randType);
		if (randType == Bubble.eType.Honeycomb && isUsedItem(Constant.Item.eType.BeeBarrier))
		{
			shotBubble.setGrayScale(gameoverMat);
		}
		tk2dAnimatedSprite shotSprite = shotBubble.GetComponentInChildren<tk2dAnimatedSprite>();
		Color shotCollor = shotSprite.color;
		shotCollor.a = 0f;
		shotSprite.color = shotCollor;
		shotBubble.transform.localScale = Vector3.one;
		tk2dAnimatedSprite hitSprite = hitBubble.GetComponentInChildren<tk2dAnimatedSprite>();
		Color hitColor = hitSprite.color;
		iTween.ScaleTo(hitBubble.gameObject, iTween.Hash("x", 0.6f, "y", 0.6f, "time", 0.7f, "easetype", iTween.EaseType.easeOutQuad));
		float fadeTime = 0f;
		while (fadeTime < 0.7f)
		{
			yield return stagePause.sync();
			fadeTime += Time.deltaTime;
			shotCollor.a = fadeTime / 0.7f;
			shotSprite.color = shotCollor;
			hitColor.a = 1f - fadeTime / 0.7f;
			hitSprite.color = hitColor;
		}
		shotCollor.a = 1f;
		shotSprite.color = shotCollor;
		while (hitBubble.GetComponent<iTween>() != null)
		{
			yield return stagePause.sync();
		}
		if (bubble_hit.onObstacle != null)
		{
			bubble_hit.onObstacle.currentParentBubble = shotBubble;
			shotBubble.onObstacle = bubble_hit.onObstacle;
		}
		fieldBubbleList.Remove(bubble_hit);
		UnityEngine.Object.DestroyImmediate(hitBubble.gameObject);
		fieldBubbleList.Add(shotBubble);
		shotBubble.setFieldState();
		updateFieldBubbleList();
	}

	private IEnumerator chameleonCheckRoutine(Bubble shotBubble)
	{
		isHitChameleon = false;
		changeChamList.Clear();
		foreach (Bubble b2 in chameleonBubbleList)
		{
			if (b2.isNearBubble(shotBubble) && !b2.isFrozen && !b2.isLocked && !b2.inCloud)
			{
				isHitChameleon = true;
				changeChamList.Add(b2);
			}
		}
		foreach (Bubble b in changeChamList)
		{
			checkBreakChamList.Add(b);
		}
		yield break;
	}

	private IEnumerator chameleonChangeRoutine()
	{
		if (isHitChameleon)
		{
			yield return StartCoroutine(changeChameleon(changeChamList));
			float waitTime = 0f;
			while (waitTime < 0.3f)
			{
				waitTime += Time.deltaTime;
				yield return stagePause.sync();
			}
		}
	}

	private IEnumerator checkBreakCameleon(Bubble shotBubble, int precomb)
	{
		List<Bubble> rainbowList = new List<Bubble>();
		int breakNum_ = 0;
		breakLightningCount_ = 0;
		foreach (Bubble b in checkBreakChamList)
		{
			if (b == null || b.state != Bubble.eState.Field)
			{
				continue;
			}
			float[] checkedFlags = new float[fieldBubbleList.Count];
			checkSamColor(b, ref checkedFlags, rainbowList);
			float chainCount = 0f;
			int bubbleCount = 0;
			float[] array = checkedFlags;
			foreach (float checkedFlag in array)
			{
				if (checkedFlag > 0f)
				{
					chainCount += checkedFlag;
					bubbleCount++;
				}
			}
			List<Bubble> tempBreakBubbleList = new List<Bubble>();
			if (!(chainCount >= 3f))
			{
				continue;
			}
			b.startBreak();
			for (int k = 0; k < fieldBubbleList.Count; k++)
			{
				if (!(checkedFlags[k] <= 0f))
				{
					if (rainbowChainCount > 0)
					{
						fieldBubbleList[k].isLineFriend = false;
					}
					tempBreakBubbleList.Add(fieldBubbleList[k]);
					fieldBubbleList[k].startBreak();
					plusMinusEffect(fieldBubbleList[k]);
					breakNum_++;
				}
			}
			for (int j = 0; j < rainbowList.Count; j++)
			{
				Bubble t = rainbowList[j];
				bool bNear = false;
				foreach (Bubble breakBubble_ in tempBreakBubbleList)
				{
					if (t.isNearBubble(breakBubble_))
					{
						bNear = true;
						break;
					}
				}
				if (bNear)
				{
					if (t.type == Bubble.eType.FriendRainbow)
					{
						t.setType(convertColorBubble(b.type) + 31);
					}
					else if (t.type == Bubble.eType.MinilenRainbow)
					{
						t.setType(convertColorBubble(b.type) + 128);
					}
					else
					{
						t.setType(convertColorBubble(b.type));
					}
					breakLightningCount_++;
				}
			}
			chainedChameleonCount += bubbleCount;
		}
		if (breakNum_ <= 0)
		{
			yield break;
		}
		if (comboCount == 0)
		{
			comboCount = precomb;
		}
		if (breakNum_ >= 5)
		{
			Sound.Instance.playSe(Sound.eSe.SE_219_daihakai);
		}
		else
		{
			Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
		}
		yield return StartCoroutine(breakChameleonRoutine(shotBubble, breakLightningCount_));
		if (rainbowList.Count > 0)
		{
			float waitTime = 0f;
			while (waitTime < 0.3f)
			{
				waitTime += Time.deltaTime;
				yield return stagePause.sync();
			}
		}
		for (int i = 0; i < rainbowList.Count; i++)
		{
			if (rainbowList[i] != null && rainbowList[i].state == Bubble.eState.Field)
			{
				yield return StartCoroutine(hitDefault(rainbowList[i]));
			}
		}
	}

	private IEnumerator lineFriendBonusRoutine()
	{
		if (friendBonusList.Count == 0)
		{
			yield break;
		}
		stagePause.pause = true;
		Transform friend_bonus = frontUi.Find("friend_bonus");
		friend_bonus.gameObject.SetActive(true);
		foreach (FriendBonus friendBonus in friendBonusList)
		{
			lineFriendCount++;
			UserData friendData = DummyPlayFriendData.DummyFriends[friendBonus.friendIndex];
			UILabel label = friend_bonus.Find("Bonusget_Label").GetComponent<UILabel>();
			label.text = friendData.UserName;
			string friendName = Constant.UserName.ReplaceOverStr(label);
			MessageResource msgRes = MessageResource.Instance;
			string msg2 = msgRes.getMessage(2004);
			msg2 = msgRes.castCtrlCode(msg2, 1, friendName);
			label.text = msg2;
			lineFriendLabel_ = label;
			Utility.createSysLabel(lineFriendLabel_, ref lineFriendSysLabel_);
			UITexture uiTexture = friend_bonus.Find("UserIcon").GetComponent<UITexture>();
			uiTexture.material = UnityEngine.Object.Instantiate(uiTexture.material) as Material;
			if (friendData.Texture != null)
			{
				uiTexture.mainTexture = friendData.Texture;
			}
			else
			{
				uiTexture.mainTexture = defaultUserIconTexture;
			}
			Transform bonusTrans = null;
			for (int j = 0; j < 5; j++)
			{
				Transform bonus = friend_bonus.Find(j.ToString("00"));
				if (j == friendBonus.bonusIndex)
				{
					bonus.gameObject.SetActive(true);
					bonusTrans = bonus;
				}
				else
				{
					bonus.gameObject.SetActive(false);
				}
			}
			int div = 1;
			int digit = friendBonus.num.ToString().Length;
			string childName = "number_";
			for (int i = 0; i < 6; i++)
			{
				childName += "0";
				Transform child = bonusTrans.Find(childName);
				if (child == null)
				{
					break;
				}
				if (i > digit)
				{
					child.gameObject.SetActive(false);
					continue;
				}
				UISprite sprite = child.GetComponent<UISprite>();
				if (i == digit)
				{
					sprite.spriteName = "friend_bonus_number_10";
				}
				else
				{
					sprite.spriteName = "friend_bonus_number_0" + friendBonus.num % (div * 10) / div;
				}
				sprite.MakePixelPerfect();
				div *= 10;
			}
			if (friendBonus.bonusIndex == 0 || friendBonus.bonusIndex == 1)
			{
				prevValue = 0;
				bubblePlusNum = friendBonus.num;
				timePlusNum = friendBonus.num;
				StartCoroutine(bubblePlusEffect());
			}
			Animation friend_bonusAnim = friend_bonus.GetComponent<Animation>();
			friend_bonusAnim.Play("Clear_condition_in_anm");
			while (friend_bonusAnim.isPlaying)
			{
				yield return null;
			}
			float wait = 0f;
			while (wait < cutinWaitTime)
			{
				if (Input.GetMouseButtonUp(0))
				{
					Constant.SoundUtil.PlayDecideSE();
					break;
				}
				wait += Time.deltaTime;
				yield return null;
			}
			friend_bonusAnim.Play("Clear_condition_out_anm");
			while (friend_bonusAnim.isPlaying)
			{
				yield return null;
			}
			lineFriendLabel_ = null;
			lineFriendSysLabel_ = null;
		}
		friendBonusList.Clear();
		friend_bonus.gameObject.SetActive(false);
		DialogBase dialogQuit = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
		while (dialogQuit.isOpen())
		{
			yield return null;
		}
		stagePause.pause = false;
	}

	private IEnumerator GetkeyBubbleRoutine()
	{
		stagePause.pause = true;
		GameObject keyAnimObj = UnityEngine.Object.Instantiate(keyAnim) as GameObject;
		Animation anim = keyAnimObj.GetComponent<Animation>();
		keyAnimObj.transform.parent = frontUi.transform;
		keyAnimObj.transform.localPosition = keyAnim.transform.localPosition;
		keyAnimObj.transform.localScale = keyAnim.transform.localScale;
		keyAnimObj.SetActive(true);
		anim.Play("Key_get_in_anm");
		if (anim.isPlaying)
		{
			yield return null;
		}
		float elapsedTime = 0f;
		while (elapsedTime < cutinWaitTime)
		{
			if (Input.GetMouseButtonUp(0))
			{
				Constant.SoundUtil.PlayDecideSE();
				break;
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		anim.Play("Key_get_out_anm");
		while (anim.IsPlaying("Key_get_out_anm"))
		{
			yield return null;
		}
		UnityEngine.Object.Destroy(keyAnimObj);
		bGetKey = false;
		stagePause.pause = false;
	}

	private void LateUpdate()
	{
		if (!(lineFriendLabel_ == null) && !(lineFriendSysLabel_ == null))
		{
			lineFriendSysLabel_.transform.localPosition = lineFriendLabel_.transform.localPosition;
		}
	}

	private IEnumerator hyperRoutine(Bubble shotBubble)
	{
		Sound.Instance.playSe(Sound.eSe.SE_339_hyperbubble);
		shotBubble.startBreak();
		bool bSearch = false;
		breakLightningCount_ = 0;
		if (!shotBubble.isHitCloud && breakFrozen(shotBubble))
		{
			bSearch = true;
		}
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble chainBubble in chainBubbleDic[i])
			{
				if (chainBubble.type != Bubble.eType.ChainLock || !shotBubble.isNearBubble(chainBubble) || !unlockChain(chainBubble))
				{
					continue;
				}
				break;
			}
		}
		List<Bubble> nearBubbleList = new List<Bubble>(6);
		Bubble shotBubble2 = default(Bubble);
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (!fieldBubble.isLocked && fieldBubble.type != Bubble.eType.Blank && fieldBubble.type != Bubble.eType.Fulcrum && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fieldBubble.type != Bubble.eType.FriendBox && fieldBubble.type != Bubble.eType.Rock && fieldBubble.type != Bubble.eType.Box && (fieldBubble.type < Bubble.eType.TunnelIn || fieldBubble.type > Bubble.eType.TunnelOutRightDown) && fieldBubble.state == Bubble.eState.Field && shotBubble2.isNearBubble(fieldBubble) && (!shotBubble2.isNearBubble(fieldBubble) || !fieldBubble.inCloud))
			{
				nearBubbleList.Add(fieldBubble);
			}
		});
		foreach (Bubble breakBubble2 in nearBubbleList)
		{
			if (!breakBubble2.isColorBubbleFixed() && (109 > int.Parse(breakBubble2.name) || 116 < int.Parse(breakBubble2.name)) && !breakBubble2.IsSpecialBreak_)
			{
				continue;
			}
			List<Bubble> rainbowList = new List<Bubble>();
			int[] checkedFlags = new int[fieldBubbleList.Count];
			checkSamColorFixed(breakBubble2, ref checkedFlags, rainbowList);
			for (int j = 0; j < fieldBubbleList.Count; j++)
			{
				if (checkedFlags[j] == 1)
				{
					if (109 <= int.Parse(fieldBubbleList[j].name) && 116 >= int.Parse(fieldBubbleList[j].name) && !fieldBubbleList[j].IsSpecialBreak_)
					{
						StartCoroutine(fieldBubbleList[j].SpecialBreak());
						continue;
					}
					fieldBubbleList[j].startBreak();
					plusEffect(fieldBubbleList[j]);
				}
			}
			if (breakBubble2.state == Bubble.eState.Field)
			{
				breakBubble2.startBreak();
				plusEffect(breakBubble2);
			}
		}
		foreach (Bubble breakBubble in nearBubbleList)
		{
			switch (breakBubble.type)
			{
			case Bubble.eType.Lightning:
			case Bubble.eType.LightningG:
				if (lightning(breakBubble))
				{
					bSearch = true;
				}
				break;
			case Bubble.eType.Search:
				bSearch = true;
				break;
			case Bubble.eType.Time:
			case Bubble.eType.Coin:
				specialEffect(breakBubble);
				break;
			case Bubble.eType.FriendRainbow:
			case Bubble.eType.Rainbow:
			case Bubble.eType.MinilenRainbow:
				breakLightningCount_++;
				break;
			case Bubble.eType.Counter:
			case Bubble.eType.BlackHole_A:
			case Bubble.eType.BlackHole_B:
				if (breakBubble.isFrozen)
				{
					breakLightningCount_++;
				}
				break;
			}
			if (Bubble.eType.ChameleonRed <= breakBubble.type && Bubble.eType.Unknown >= breakBubble.type)
			{
				breakLightningCount_++;
			}
			if (Bubble.eType.MorganaRed <= breakBubble.type && Bubble.eType.MorganaBlack >= breakBubble.type && !breakBubble.IsSpecialBreak_)
			{
				StartCoroutine(breakBubble.SpecialBreak());
			}
			if (breakBubble.type == Bubble.eType.Skull)
			{
				breakBubble.setBreak();
				breakBubble.startFadeout();
			}
			else
			{
				breakBubble.startBreak();
			}
		}
		if (chainBreakRoutine != null)
		{
			updateChainLock();
		}
		yield return StartCoroutine(breakRoutine(shotBubble, breakLightningCount_));
		if (bSearch && !isSkullGameOver)
		{
			yield return StartCoroutine(searchBubbleRoutine());
		}
	}

	private IEnumerator bombRoutine(Bubble shotBubble)
	{
		Sound.Instance.playSe(Sound.eSe.SE_340_bombbubble);
		shotBubble.startBreak();
		bool bSearch = false;
		breakLightningCount_ = 0;
		if (!shotBubble.isHitCloud && breakFrozen(shotBubble))
		{
			bSearch = true;
		}
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble chainBubble in chainBubbleDic[i])
			{
				if (chainBubble.type != Bubble.eType.ChainLock || !shotBubble.isBombRangeBubble(chainBubble) || chainBubble.mLocked || !unlockChain(chainBubble))
				{
					continue;
				}
				break;
			}
		}
		List<Bubble> nearBubbleList = new List<Bubble>(6);
		Bubble shotBubble2 = default(Bubble);
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (!fieldBubble.isLocked && fieldBubble.type != Bubble.eType.Blank && fieldBubble.type != Bubble.eType.Fulcrum && (fieldBubble.type < Bubble.eType.TunnelIn || fieldBubble.type > Bubble.eType.TunnelOutRightDown) && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fieldBubble.state == Bubble.eState.Field && shotBubble2.isBombRangeBubble(fieldBubble) && (!shotBubble2.isBombRangeBubble(fieldBubble) || !fieldBubble.inCloud) && (!shotBubble2.isNearBubble(fieldBubble) || !fieldBubble.inCloud))
			{
				nearBubbleList.Add(fieldBubble);
			}
		});
		foreach (Bubble breakBubble in nearBubbleList)
		{
			switch (breakBubble.type)
			{
			case Bubble.eType.Lightning:
			case Bubble.eType.LightningG:
				if (lightning(breakBubble))
				{
					bSearch = true;
				}
				break;
			case Bubble.eType.Search:
				bSearch = true;
				break;
			case Bubble.eType.Time:
			case Bubble.eType.Coin:
				specialEffect(breakBubble);
				break;
			case Bubble.eType.FriendRainbow:
			case Bubble.eType.Rainbow:
			case Bubble.eType.MinilenRainbow:
				breakLightningCount_++;
				break;
			case Bubble.eType.Grow:
			case Bubble.eType.Skull:
			case Bubble.eType.FriendBox:
			case Bubble.eType.Box:
			case Bubble.eType.Rock:
			case Bubble.eType.Honeycomb:
			case Bubble.eType.Counter:
			case Bubble.eType.BlackHole_A:
			case Bubble.eType.BlackHole_B:
				if (breakBubble.isFrozen)
				{
					breakLightningCount_++;
				}
				break;
			}
			if (Bubble.eType.ChameleonRed <= breakBubble.type && Bubble.eType.Unknown >= breakBubble.type)
			{
				breakLightningCount_++;
			}
			if (Bubble.eType.MorganaRed <= breakBubble.type && Bubble.eType.MorganaBlack >= breakBubble.type && !breakBubble.IsSpecialBreak_)
			{
				StartCoroutine(breakBubble.SpecialBreak());
			}
			if (breakBubble.type == Bubble.eType.Skull)
			{
				breakBubble.setBreak();
			}
			else
			{
				breakBubble.startBreak();
			}
			plusEffect(breakBubble);
			if (breakBubble.type != Bubble.eType.Counter)
			{
				if (breakBubble.onObstacle != null)
				{
					StartCoroutine(breakBubble.onObstacle.breakRoutine(true));
				}
				UnityEngine.Object.Destroy(breakBubble.gameObject);
			}
		}
		if (chainBreakRoutine != null)
		{
			updateChainLock();
		}
		yield return StartCoroutine(breakRoutine(shotBubble, breakLightningCount_));
		if (bSearch)
		{
			yield return StartCoroutine(searchBubbleRoutine());
		}
	}

	private IEnumerator shakeRoutine(Bubble shotBubble)
	{
		Sound.Instance.playSe(Sound.eSe.SE_341_shakebubble);
		shotBubble.startBreak(false);
		bool bSearch = false;
		breakLightningCount_ = 0;
		if (!shotBubble.isHitCloud && breakFrozen(shotBubble))
		{
			bSearch = true;
		}
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble chainBubble in chainBubbleDic[i])
			{
				if (chainBubble.type != Bubble.eType.ChainLock || !shotBubble.isNearBubble(chainBubble) || !unlockChain(chainBubble))
				{
					continue;
				}
				break;
			}
		}
		List<Bubble> nearList = new List<Bubble>(6);
		List<Bubble> nearBubbleList = new List<Bubble>(18);
		Bubble shotBubble2 = default(Bubble);
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (!fieldBubble.isLocked && fieldBubble.type != Bubble.eType.Blank && fieldBubble.type != Bubble.eType.Fulcrum && (fieldBubble.type < Bubble.eType.TunnelIn || fieldBubble.type > Bubble.eType.TunnelOutRightDown) && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fieldBubble.state == Bubble.eState.Field && shotBubble2.isShakeRangeBubble(fieldBubble) && (!shotBubble2.isShakeRangeBubble(fieldBubble) || !fieldBubble.inCloud))
			{
				nearBubbleList.Add(fieldBubble);
				if (shotBubble2.isNearBubble(fieldBubble) && (!shotBubble2.isNearBubble(fieldBubble) || !fieldBubble.inCloud))
				{
					nearList.Add(fieldBubble);
				}
			}
		});
		foreach (Bubble breakBubble3 in nearList)
		{
			switch (breakBubble3.type)
			{
			case Bubble.eType.Lightning:
				if (lightning(breakBubble3))
				{
					bSearch = true;
				}
				breakBubble3.startBreak();
				break;
			case Bubble.eType.Search:
				bSearch = true;
				breakBubble3.startBreak();
				break;
			case Bubble.eType.Time:
			case Bubble.eType.Coin:
				specialEffect(breakBubble3);
				breakBubble3.startBreak();
				break;
			}
			if (breakBubble3.type >= Bubble.eType.PlusRed && breakBubble3.type <= Bubble.eType.PlusBlack)
			{
				plusEffect(breakBubble3);
				breakBubble3.startBreak();
			}
		}
		nearBubbleList.RemoveAll((Bubble b) => b.state != Bubble.eState.Field);
		if (nearBubbleList.Count > 0)
		{
			iTween itween = null;
			foreach (Bubble breakBubble2 in nearBubbleList)
			{
				GameObject spriteObj = breakBubble2.sprite.gameObject;
				iTween.ShakePosition(spriteObj, iTween.Hash("amount", new Vector3(5f, 5f, 0f), "islocal", true, "time", 0.5f));
				itween = spriteObj.GetComponent<iTween>();
			}
			while (itween != null)
			{
				yield return stagePause.sync();
			}
			int nearBubbleCount = nearBubbleList.Count - 3;
			if (nearBubbleCount < 0)
			{
				nearBubbleCount = 0;
			}
			int dropCount = 3;
			for (int k = 0; k < nearBubbleCount; k++)
			{
				int p = 20 + random.Next(21);
				if (p > random.Next(100))
				{
					dropCount++;
				}
			}
			int delayCount = 0;
			for (int j = 0; j < dropCount; j++)
			{
				if (nearBubbleList.Count == 0)
				{
					break;
				}
				int index = random.Next(nearBubbleList.Count);
				Bubble breakBubble = nearBubbleList[index];
				switch (breakBubble.type)
				{
				case Bubble.eType.Lightning:
				case Bubble.eType.LightningG:
					if (lightning(breakBubble))
					{
						bSearch = true;
					}
					break;
				case Bubble.eType.Search:
					bSearch = true;
					break;
				case Bubble.eType.Time:
				case Bubble.eType.Coin:
					specialEffect(breakBubble);
					break;
				case Bubble.eType.FriendRainbow:
				case Bubble.eType.Rainbow:
				case Bubble.eType.MinilenRainbow:
					breakLightningCount_++;
					break;
				case Bubble.eType.Grow:
				case Bubble.eType.Skull:
				case Bubble.eType.FriendBox:
				case Bubble.eType.Box:
				case Bubble.eType.Rock:
				case Bubble.eType.Honeycomb:
				case Bubble.eType.Counter:
				case Bubble.eType.BlackHole_A:
				case Bubble.eType.BlackHole_B:
					if (breakBubble.isFrozen)
					{
						breakLightningCount_++;
					}
					break;
				}
				if (breakBubble.type >= Bubble.eType.PlusRed && breakBubble.type <= Bubble.eType.PlusBlack)
				{
					plusEffect(breakBubble);
				}
				if (breakBubble.type >= Bubble.eType.ChameleonRed && breakBubble.type <= Bubble.eType.Unknown)
				{
					breakLightningCount_++;
				}
				basicSkillDropCount(breakBubble);
				breakBubble.startDrop(delayCount);
				delayCount++;
				nearBubbleList.RemoveAt(index);
			}
		}
		if (chainBreakRoutine != null)
		{
			updateChainLock();
		}
		yield return StartCoroutine(breakRoutine(shotBubble, breakLightningCount_));
		nextScore = totalScore;
		if (bSearch)
		{
			yield return StartCoroutine(searchBubbleRoutine());
		}
	}

	private IEnumerator metalRoutine(Bubble shotBubble)
	{
		bMetalShooting = true;
		yield return stagePause.sync();
		yield return stagePause.sync();
		bool bSearch = false;
		List<Vector3> path = guide.metalPath;
		int pathIndex = 0;
		float move = 0f;
		float length = (path[pathIndex + 1] - path[pathIndex]).magnitude;
		Vector3 pos = shotBubble.myTrans.position;
		float distance = 0f;
		breakLightningCount_ = 0;
		shotBubble.boundCount = 0;
		int breakNum = 0;
		while (true)
		{
			float moveValue = Bubble.SPEED * uiScale * Time.deltaTime;
			move += moveValue;
			distance += moveValue;
			float t = move / length;
			if (1f <= t)
			{
				pathIndex++;
				if (path.Count - 1 <= pathIndex)
				{
					break;
				}
				move -= length;
				length = (path[pathIndex + 1] - path[pathIndex]).magnitude;
				t = move / length;
				if (pathIndex != guide.metalPathIndex)
				{
					Vector3 effPos = path[pathIndex];
					effPos.z = shotBubble.myTrans.position.z;
					StartCoroutine(boundEffRoutine(path[pathIndex - 1].x > path[pathIndex].x, effPos));
					Sound.Instance.playSe(Sound.eSe.SE_401_metalbubble_bound);
				}
				else
				{
					Vector3 tmppos2 = path[pathIndex];
					Bubble fieldBubble = guide.tunnelInBubbleMetal;
					yield return StartCoroutine(shotBubble.warpInRutine(tmppos2, fieldBubble));
					pos = path[pathIndex + 1];
					tmppos2 = fieldBubble.OutObject.transform.position;
					yield return StartCoroutine(shotBubble.warpOutRutine(fieldBubble.OutObject.transform.localPosition, fieldBubble));
					t = 1f;
				}
				shotBubble.boundCount++;
			}
			if (pathIndex != guide.metalPathIndex)
			{
				pos = Vector3.Lerp(path[pathIndex], path[pathIndex + 1], t);
			}
			pos.z = shotBubble.myTrans.position.z;
			shotBubble.myTrans.position = pos;
			bPlus = false;
			bMinus = false;
			breakNum = 0;
			foreach (KeyValuePair<Bubble, float> pair2 in guide.metalBreakBubbleDic)
			{
				if (pair2.Value < distance)
				{
					Bubble bubble = pair2.Key;
					if (bubble == null || bubble.state != Bubble.eState.Field)
					{
						continue;
					}
					List<Bubble> breakBubbleList = new List<Bubble>();
					if (bubble.isFrozen)
					{
						checkFrozen(bubble, breakBubbleList);
					}
					else
					{
						breakBubbleList.Add(bubble);
					}
					foreach (Bubble breakBubble in breakBubbleList)
					{
						if (breakBubble == null || breakBubble.state != Bubble.eState.Field)
						{
							continue;
						}
						plusEffect(breakBubble);
						switch (breakBubble.type)
						{
						case Bubble.eType.Lightning:
						case Bubble.eType.LightningG:
							if (lightning(breakBubble))
							{
								bSearch = true;
							}
							break;
						case Bubble.eType.Search:
							bSearch = true;
							break;
						case Bubble.eType.Time:
						case Bubble.eType.Coin:
							specialEffect(breakBubble);
							break;
						case Bubble.eType.FriendRainbow:
						case Bubble.eType.Rainbow:
						case Bubble.eType.MinilenRainbow:
							breakLightningCount_++;
							break;
						case Bubble.eType.Grow:
						case Bubble.eType.Skull:
						case Bubble.eType.FriendBox:
						case Bubble.eType.Box:
						case Bubble.eType.Rock:
						case Bubble.eType.Honeycomb:
						case Bubble.eType.Counter:
						case Bubble.eType.BlackHole_A:
						case Bubble.eType.BlackHole_B:
							if (breakBubble.isFrozen)
							{
								breakLightningCount_++;
							}
							break;
						}
						if (Bubble.eType.ChameleonRed <= breakBubble.type && Bubble.eType.Unknown >= breakBubble.type)
						{
							breakLightningCount_++;
						}
						if (Bubble.eType.MorganaRed <= breakBubble.type && Bubble.eType.MorganaBlack >= breakBubble.type && !breakBubble.IsSpecialBreak_)
						{
							StartCoroutine(breakBubble.SpecialBreak());
						}
						if (breakBubble.type == Bubble.eType.Skull && !breakBubble.isFrozen)
						{
							breakBubble.setBreak();
							breakBubble.startFadeout();
						}
						else
						{
							breakBubble.startBreak();
						}
						breakNum++;
					}
				}
				if (bPlus)
				{
					Sound.Instance.playSe(Sound.eSe.SE_335_plusbubble);
					playCountdownEff(true);
				}
			}
			if (0 < breakNum)
			{
				Sound.Instance.playSe(Sound.eSe.SE_402_metalbubble_break);
			}
			foreach (KeyValuePair<ChainBubble, float> pair in guide.metalBreakRockDic)
			{
				if (pair.Value < distance)
				{
					ChainBubble chain = pair.Key;
					if (chain.type == Bubble.eType.ChainLock)
					{
						unlockChain(chain);
					}
				}
			}
			if (pathIndex != guide.metalPathIndex)
			{
				yield return stagePause.sync();
			}
		}
		pos = path[pathIndex];
		pos.z = shotBubble.myTrans.position.z;
		shotBubble.myTrans.position = pos;
		bPlus = false;
		bMinus = false;
		breakNum = 0;
		foreach (KeyValuePair<Bubble, float> item in guide.metalBreakBubbleDic)
		{
			Bubble breakBubble2 = item.Key;
			if (breakBubble2 == null || breakBubble2.state != Bubble.eState.Field)
			{
				continue;
			}
			plusEffect(breakBubble2);
			switch (breakBubble2.type)
			{
			case Bubble.eType.Lightning:
			case Bubble.eType.LightningG:
				if (lightning(breakBubble2))
				{
					bSearch = true;
				}
				break;
			case Bubble.eType.Search:
				bSearch = true;
				break;
			case Bubble.eType.Time:
			case Bubble.eType.Coin:
				specialEffect(breakBubble2);
				break;
			case Bubble.eType.FriendRainbow:
			case Bubble.eType.Rainbow:
			case Bubble.eType.MinilenRainbow:
				breakLightningCount_++;
				break;
			}
			if (breakBubble2.type == Bubble.eType.Skull)
			{
				breakBubble2.setBreak();
				breakBubble2.startFadeout();
			}
			else
			{
				breakBubble2.startBreak();
			}
			breakNum++;
		}
		if (bPlus)
		{
			Sound.Instance.playSe(Sound.eSe.SE_335_plusbubble);
			playCountdownEff(true);
		}
		if (0 < breakNum)
		{
			Sound.Instance.playSe(Sound.eSe.SE_402_metalbubble_break);
		}
		foreach (KeyValuePair<ChainBubble, float> item2 in guide.metalBreakRockDic)
		{
			ChainBubble chain2 = item2.Key;
			if (chain2.type == Bubble.eType.ChainLock)
			{
				unlockChain(chain2);
			}
		}
		if (chainBreakRoutine != null)
		{
			updateChainLock();
		}
		Bubble cbBubble = guide.counterbalanceBubble;
		if (cbBubble != null)
		{
			if (cbBubble.type >= Bubble.eType.MorganaRed && cbBubble.type <= Bubble.eType.MorganaBlack)
			{
				Debug.Log("spe");
				StartCoroutine(cbBubble.SpecialBreak());
			}
			guide.counterbalanceBubble = null;
		}
		Sound.Instance.playSe(Sound.eSe.SE_405_metalbubble_broken);
		shotBubble.myTrans.Find("bubble_trail_eff").parent = shotBubble.myTrans.parent.parent.parent;
		shotBubble.startBreak();
		if (arrow.charaIndex != 0)
		{
			charaAnims[1].Play(waitAnimName);
		}
		else
		{
			charaAnims[0].Play(waitAnimName);
		}
		shotBubblePos = shotBubble.myTrans.position;
		yield return StartCoroutine(breakRoutine(shotBubble, breakLightningCount_));
		yield return StartCoroutine(updateRotateBubble(shotBubble));
		bMetalShooting = false;
		if (bSearch)
		{
			yield return StartCoroutine(searchBubbleRoutine());
		}
	}

	private IEnumerator iceRoutine(Bubble shotBubble)
	{
		shotBubble.startBreak(false);
		bool bSearch = false;
		breakLightningCount_ = 0;
		if (!shotBubble.isHitCloud && breakFrozen(shotBubble))
		{
			bSearch = true;
		}
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble chainBubble in chainBubbleDic[i])
			{
				if (chainBubble.type != Bubble.eType.ChainLock || !shotBubble.isNearBubble(chainBubble) || !unlockChain(chainBubble))
				{
					continue;
				}
				break;
			}
		}
		List<Bubble> nearBubbleList = new List<Bubble>(6);
		Bubble shotBubble2 = default(Bubble);
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (!fieldBubble.isLocked && !fieldBubble.isFrozen && fieldBubble.state == Bubble.eState.Field && shotBubble2.isNearBubble(fieldBubble) && (!shotBubble2.isNearBubble(fieldBubble) || !fieldBubble.inCloud))
			{
				nearBubbleList.Add(fieldBubble);
			}
		});
		foreach (Bubble breakBubble in nearBubbleList)
		{
			switch (breakBubble.type)
			{
			case Bubble.eType.Lightning:
				if (lightning(breakBubble))
				{
					bSearch = true;
				}
				breakBubble.startBreak();
				break;
			case Bubble.eType.Search:
				bSearch = true;
				breakBubble.startBreak();
				break;
			case Bubble.eType.Time:
			case Bubble.eType.Coin:
				specialEffect(breakBubble);
				breakBubble.startBreak();
				break;
			}
			if (Bubble.eType.MorganaRed <= breakBubble.type && Bubble.eType.MorganaBlack >= breakBubble.type && !breakBubble.IsSpecialBreak_)
			{
				StartCoroutine(breakBubble.SpecialBreak());
			}
		}
		int frozenCount = 0;
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (!fieldBubble.isLocked && !fieldBubble.isFrozen && !fieldBubble.inCloud && fieldBubble.type != Bubble.eType.Blank && fieldBubble.type != Bubble.eType.Fulcrum && (fieldBubble.type < Bubble.eType.TunnelIn || fieldBubble.type > Bubble.eType.TunnelOutRightDown) && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fieldBubble.state == Bubble.eState.Field && shotBubble2.isShakeRangeBubble(fieldBubble) && (Bubble.eType.MorganaRed > fieldBubble.type || Bubble.eType.MorganaBlack < fieldBubble.type))
			{
				frozenCount++;
				fieldBubble.setFrozen(true);
				if (fieldBubble.onObstacle != null)
				{
					StartCoroutine(fieldBubble.onObstacle.breakRoutine(false));
				}
			}
		});
		if (frozenCount >= 5)
		{
			Sound.Instance.playSe(Sound.eSe.SE_507_bubble_freeze_big);
		}
		else
		{
			Sound.Instance.playSe(Sound.eSe.SE_504_bubble_freeze);
		}
		if (chainBreakRoutine != null)
		{
			updateChainLock();
		}
		yield return StartCoroutine(breakRoutine(shotBubble, breakLightningCount_));
		if (bSearch)
		{
			yield return StartCoroutine(searchBubbleRoutine());
		}
	}

	private IEnumerator fireRoutine(Bubble shotBubble)
	{
		Sound.Instance.playSe(Sound.eSe.SE_511_fire_explosion);
		shotBubble.startBreak();
		bool bSearch = false;
		breakLightningCount_ = 0;
		if (!shotBubble.isHitCloud && breakFrozen(shotBubble))
		{
			bSearch = true;
		}
		float hitBubbleY = shotBubble.myTrans.localPosition.y;
		fireEffectRoutine = StartCoroutine(fireEffect(shotBubble.myTrans.position));
		foreach (Ivy iv in ivyList)
		{
			if (!iv.isRemoved_)
			{
				float bubbleHeightHalf = 26f;
				float ivY = iv.transform.localPosition.y;
				if (!(hitBubbleY - bubbleHeightHalf > ivY) && !(hitBubbleY + bubbleHeightHalf < ivY))
				{
					StartCoroutine(ivyFireEffect(iv, iv.ivyType_ == Ivy.eType.Left));
					break;
				}
			}
		}
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble chainBubble in chainBubbleDic[i])
			{
				if (chainBubble.type == Bubble.eType.ChainLock && !chainBubble.mLocked)
				{
					float fieldBubbleY = chainBubble.myTrans.localPosition.y;
					if (((fieldBubbleY > hitBubbleY - 1f && fieldBubbleY < hitBubbleY + 1f) || shotBubble.isNearBubble(chainBubble)) && unlockChain(chainBubble))
					{
						break;
					}
				}
			}
		}
		List<Bubble> breakBubbleList = new List<Bubble>();
		Bubble shotBubble2 = default(Bubble);
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (!fieldBubble.isLocked && fieldBubble.type != Bubble.eType.Blank && fieldBubble.type != Bubble.eType.Fulcrum && (fieldBubble.type < Bubble.eType.TunnelIn || fieldBubble.type > Bubble.eType.TunnelOutRightDown) && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fieldBubble.state == Bubble.eState.Field)
			{
				float y = fieldBubble.myTrans.localPosition.y;
				if (((!shotBubble2.isHitCloud && y > hitBubbleY - 1f && y < hitBubbleY + 1f) || (shotBubble2.isHitCloud && y > hitBubbleY - 26f && y < hitBubbleY + 26f)) && !fieldBubble.inCloud)
				{
					breakBubbleList.Add(fieldBubble);
					if (fieldBubble.type == Bubble.eType.Lightning)
					{
						fieldBubble.isFire = true;
					}
				}
				else if (shotBubble2.isNearBubble(fieldBubble) && !fieldBubble.inCloud && (fieldBubble.type == Bubble.eType.Lightning || fieldBubble.type == Bubble.eType.Search || fieldBubble.type == Bubble.eType.Time || fieldBubble.type == Bubble.eType.Coin || fieldBubble.type == Bubble.eType.Star || (Bubble.eType.MorganaRed <= fieldBubble.type && Bubble.eType.MorganaBlack >= fieldBubble.type)))
				{
					breakBubbleList.Add(fieldBubble);
				}
			}
		});
		foreach (Bubble breakBubble in breakBubbleList)
		{
			switch (breakBubble.type)
			{
			case Bubble.eType.Lightning:
			case Bubble.eType.LightningG:
				if (!breakBubble.isFire)
				{
					if (lightning(breakBubble))
					{
						bSearch = true;
					}
					Debug.Log("lightning");
				}
				break;
			case Bubble.eType.Search:
				bSearch = true;
				break;
			case Bubble.eType.Time:
			case Bubble.eType.Coin:
				specialEffect(breakBubble);
				break;
			case Bubble.eType.FriendRainbow:
			case Bubble.eType.Rainbow:
			case Bubble.eType.MinilenRainbow:
				breakLightningCount_++;
				break;
			case Bubble.eType.Grow:
			case Bubble.eType.Skull:
			case Bubble.eType.FriendBox:
			case Bubble.eType.Box:
			case Bubble.eType.Rock:
			case Bubble.eType.Honeycomb:
			case Bubble.eType.Counter:
			case Bubble.eType.BlackHole_A:
			case Bubble.eType.BlackHole_B:
				if (breakBubble.isFrozen)
				{
					breakLightningCount_++;
				}
				break;
			}
			if (Bubble.eType.ChameleonRed <= breakBubble.type && Bubble.eType.Unknown >= breakBubble.type)
			{
				breakLightningCount_++;
			}
			if (Bubble.eType.MorganaRed <= breakBubble.type && Bubble.eType.MorganaBlack >= breakBubble.type && !breakBubble.IsSpecialBreak_)
			{
				StartCoroutine(breakBubble.SpecialBreak());
			}
			if (breakBubble.type == Bubble.eType.Skull)
			{
				breakBubble.setBreak();
			}
			else
			{
				breakBubble.startBreak();
			}
			plusEffect(breakBubble);
			if (breakBubble.type != Bubble.eType.Counter)
			{
				if (breakBubble.onObstacle != null)
				{
					StartCoroutine(breakBubble.onObstacle.breakRoutine(true));
				}
				UnityEngine.Object.Destroy(breakBubble.gameObject);
			}
		}
		if (chainBreakRoutine != null)
		{
			updateChainLock();
		}
		yield return StartCoroutine(breakRoutine(shotBubble, breakLightningCount_));
		if (bSearch)
		{
			yield return StartCoroutine(searchBubbleRoutine());
		}
	}

	private IEnumerator lightningGRoutine(Bubble shotBubble)
	{
		Sound.Instance.playSe(Sound.eSe.SE_511_fire_explosion);
		shotBubble.startBreak();
		bool bSearch = false;
		breakLightningCount_ = 0;
		if (!shotBubble.isHitCloud && breakFrozen(shotBubble))
		{
			bSearch = true;
		}
		float hitBubbleY = shotBubble.myTrans.localPosition.y;
		fireEffectRoutine = StartCoroutine(lightningEffect(shotBubble.myTrans.position));
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble chainBubble in chainBubbleDic[i])
			{
				if (chainBubble.type == Bubble.eType.ChainLock && !chainBubble.mLocked)
				{
					float fieldBubbleY = chainBubble.myTrans.localPosition.y;
					if (((fieldBubbleY > hitBubbleY - 1f && fieldBubbleY < hitBubbleY + 1f) || shotBubble.isNearBubble(chainBubble)) && unlockChain(chainBubble))
					{
						break;
					}
				}
			}
		}
		List<Bubble> breakBubbleList = new List<Bubble>();
		Bubble shotBubble2 = default(Bubble);
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (!fieldBubble.isLocked && !fieldBubble.inCloud && fieldBubble.state == Bubble.eState.Field && fieldBubble.type != Bubble.eType.Fulcrum && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && (fieldBubble.type < Bubble.eType.TunnelIn || fieldBubble.type > Bubble.eType.TunnelOutRightDown))
			{
				float y = fieldBubble.myTrans.localPosition.y;
				if (((!shotBubble2.isHitCloud && y > hitBubbleY - 1f && y < hitBubbleY + 1f) || (shotBubble2.isHitCloud && y > hitBubbleY - 26f && y < hitBubbleY + 26f)) && !fieldBubble.inCloud)
				{
					breakBubbleList.Add(fieldBubble);
					if (fieldBubble.type == Bubble.eType.Lightning)
					{
						fieldBubble.isFire = true;
					}
				}
				else if (shotBubble2.isNearBubble(fieldBubble) && !fieldBubble.inCloud && (fieldBubble.type == Bubble.eType.Lightning || fieldBubble.type == Bubble.eType.Search || fieldBubble.type == Bubble.eType.Time || fieldBubble.type == Bubble.eType.Coin || fieldBubble.type == Bubble.eType.Star))
				{
					breakBubbleList.Add(fieldBubble);
				}
			}
		});
		foreach (Bubble breakBubble in breakBubbleList)
		{
			switch (breakBubble.type)
			{
			case Bubble.eType.Lightning:
			case Bubble.eType.LightningG:
				if (!breakBubble.isFire)
				{
					if (lightning(breakBubble))
					{
						bSearch = true;
					}
					Debug.Log("lightning");
				}
				break;
			case Bubble.eType.Search:
				bSearch = true;
				break;
			case Bubble.eType.Time:
			case Bubble.eType.Coin:
				specialEffect(breakBubble);
				break;
			case Bubble.eType.FriendRainbow:
			case Bubble.eType.Rainbow:
			case Bubble.eType.MinilenRainbow:
				breakLightningCount_++;
				break;
			case Bubble.eType.Grow:
			case Bubble.eType.Skull:
			case Bubble.eType.FriendBox:
			case Bubble.eType.Box:
			case Bubble.eType.Rock:
			case Bubble.eType.Honeycomb:
			case Bubble.eType.Counter:
			case Bubble.eType.BlackHole_A:
			case Bubble.eType.BlackHole_B:
				if (breakBubble.isFrozen)
				{
					breakLightningCount_++;
				}
				break;
			}
			if (Bubble.eType.ChameleonRed <= breakBubble.type && Bubble.eType.Unknown >= breakBubble.type)
			{
				breakLightningCount_++;
			}
			if (Bubble.eType.MorganaRed <= breakBubble.type && Bubble.eType.MorganaBlack >= breakBubble.type && !breakBubble.IsSpecialBreak_)
			{
				StartCoroutine(breakBubble.SpecialBreak());
			}
			if (breakBubble.type == Bubble.eType.Skull)
			{
				breakBubble.setBreak();
				breakBubble.startFadeout();
			}
			else
			{
				breakBubble.startBreak();
			}
			specialEffect(breakBubble);
			plusMinusEffect(breakBubble);
		}
		if (chainBreakRoutine != null)
		{
			updateChainLock();
		}
		yield return StartCoroutine(breakRoutine(shotBubble, breakLightningCount_));
		if (bSearch)
		{
			yield return StartCoroutine(searchBubbleRoutine());
		}
	}

	private IEnumerator fireEffect(Vector3 target)
	{
		GameObject eff = UnityEngine.Object.Instantiate(bubble_17_eff) as GameObject;
		Transform effTrans = eff.transform;
		Vector3 scale = effTrans.localScale;
		effTrans.parent = scrollUi;
		effTrans.localPosition = Vector3.back * 10f;
		effTrans.localScale = scale;
		Vector3 pos = effTrans.position;
		pos.x = target.x;
		pos.y = target.y;
		effTrans.position = pos;
		eff.SetActive(true);
		eff.GetComponent<Animation>().Play();
		while (eff.GetComponent<Animation>().isPlaying)
		{
			yield return null;
		}
		UnityEngine.Object.Destroy(eff);
	}

	private IEnumerator ivyFireEffect(Ivy iv, bool bLeft)
	{
		iv.setBurn();
		iv.isRemoved_ = true;
		GameObject eff = ((!bLeft) ? bubble_17_eff_r : bubble_17_eff_l);
		float elapsedTime = 0f;
		float waitTime = 0.7f;
		while (elapsedTime < waitTime)
		{
			elapsedTime += Time.deltaTime;
			yield return stagePause.sync();
		}
		Transform effTrans = eff.transform;
		Vector3 pos = effTrans.localPosition;
		pos.x = iv.headBubblePos_.x;
		pos.y = iv.headBubblePos_.y;
		pos.z = -10f;
		effTrans.localPosition = pos;
		Sound.Instance.playSe(Sound.eSe.SE_511_fire_explosion);
		iv.move((iv.ivyType_ != Ivy.eType.Left) ? 640f : (-640f));
		Transform targetTrans = iv.transform;
		eff.SetActive(true);
		eff.GetComponent<Animation>().Play();
		while (eff.GetComponent<Animation>().isPlaying)
		{
			if (targetTrans != null)
			{
				pos.y = targetTrans.localPosition.y;
			}
			effTrans.localPosition = pos;
			yield return stagePause;
		}
		eff.SetActive(false);
	}

	private IEnumerator waterRoutine(Bubble shotBubble)
	{
		if (shotBubble.isHitCloud)
		{
			shotBubble.setSplashBreak(true);
			shotBubble.startBreak();
			Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
			yield break;
		}
		if (gameType == eGameType.Time)
		{
			drawDiffTime = Time.time - startTime;
		}
		bDrawing = true;
		bool bFinish = false;
		bool bSearch = false;
		breakLightningCount_ = 0;
		List<Bubble> drawList = new List<Bubble>();
		drawList.Clear();
		drawList.Add(shotBubble);
		clearDrawLineList();
		bool bTarget = false;
		foreach (Bubble bubble in fieldBubbleList)
		{
			if (shotBubble.isNearBubble(bubble) && isInScreenBubble(bubble) && bubble.type != Bubble.eType.Fulcrum && bubble.type != Bubble.eType.RotateFulcrumL && bubble.type != Bubble.eType.RotateFulcrumR && bubble.type != Bubble.eType.Invalid && bubble.type != Bubble.eType.Blank && (bubble.type < Bubble.eType.TunnelIn || bubble.type > Bubble.eType.TunnelOutRightDown) && (Bubble.eType.MorganaRed > bubble.type || Bubble.eType.MorganaBlack < bubble.type) && bubble.state == Bubble.eState.Field && !bubble.isLocked && !bubble.inCloud)
			{
				bTarget = true;
				break;
			}
		}
		if (bTarget && !guide.bHitCeiling)
		{
			yield return StartCoroutine(setDrawHighLightEnable(true, shotBubble, 0.3f));
			bool bDrawStart = false;
			while (!bFinish)
			{
				if (stagePause.pause)
				{
					yield return StartCoroutine(setDrawHighLightEnable(false, shotBubble, 0f));
					drawList.Clear();
					drawList.Add(shotBubble);
					bDrawStart = false;
					clearDrawLineList();
					if (drawHelp_.activeSelf)
					{
						drawHelp_.SetActive(false);
					}
					if (drawCounter_.activeSelf)
					{
						drawCounter_.SetActive(false);
					}
					yield return stagePause.sync();
					yield return StartCoroutine(setDrawHighLightEnable(true, shotBubble, 0f));
				}
				if (Input.GetMouseButton(0))
				{
					Vector3 touch_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					if (bDrawStart)
					{
						Bubble b3 = checkDrawFieldBubble(touch_pos);
						if (b3 != null)
						{
							if (!drawList.Contains(b3) && drawList[drawList.Count - 1].isNearBubble(b3))
							{
								if (drawList.Count < 6 && isInScreenBubble(b3))
								{
									drawList.Add(b3);
									if (Sound.Instance.isPlayingSe(Sound.eSe.SE_518_water_bubble_allow))
									{
										Sound.Instance.stopSe(Sound.eSe.SE_518_water_bubble_allow);
									}
									Sound.Instance.playSe(Sound.eSe.SE_518_water_bubble_allow);
								}
							}
							else if (drawList.Contains(b3) && drawList[drawList.Count - 2] == b3)
							{
								drawList.Remove(drawList[drawList.Count - 1]);
								if (Sound.Instance.isPlayingSe(Sound.eSe.SE_518_water_bubble_allow))
								{
									Sound.Instance.stopSe(Sound.eSe.SE_518_water_bubble_allow);
								}
								Sound.Instance.playSe(Sound.eSe.SE_518_water_bubble_allow);
							}
						}
						else if (checkDrawBubble(touch_pos, shotBubble) && drawList.Count == 2)
						{
							drawList.Remove(drawList[1]);
							if (Sound.Instance.isPlayingSe(Sound.eSe.SE_518_water_bubble_allow))
							{
								Sound.Instance.stopSe(Sound.eSe.SE_518_water_bubble_allow);
							}
							Sound.Instance.playSe(Sound.eSe.SE_518_water_bubble_allow);
						}
					}
					else
					{
						bDrawStart = true;
					}
					setDrawCounter(touch_pos, drawList);
					updateDrawLine(touch_pos, drawList);
				}
				else
				{
					yield return 0;
					if (drawCounter_.activeSelf)
					{
						drawCounter_.SetActive(false);
					}
					if (bDrawStart && drawList.Count > 1)
					{
						bFinish = true;
						bWaterCancel = false;
						Transform lastDrawLine = drawLineBgList_[drawLineBgList_.Count - 1];
						lastDrawLine.gameObject.SetActive(false);
						yield return StartCoroutine(setDrawHighLightEnable(false, shotBubble, 0.3f));
						yield return 0;
						yield return StartCoroutine(waterPreBreakEffect());
						if (bWaterCancel)
						{
							bFinish = false;
							drawList.Clear();
							drawList.Add(shotBubble);
							bDrawStart = false;
							clearDrawLineList();
							clearDrawWaterLineList();
							yield return StartCoroutine(setDrawHighLightEnable(true, shotBubble, 0f));
							continue;
						}
					}
					else
					{
						bDrawStart = false;
						clearDrawLineList();
					}
				}
				if (drawList.Count == 1 && !Input.GetMouseButton(0))
				{
					if (!drawHelp_.activeSelf)
					{
						drawHelp_.transform.position = shotBubble.transform.position;
						Vector3 help_temp = drawHelp_.transform.localPosition;
						help_temp.z = -800f;
						drawHelp_.transform.localPosition = help_temp;
						if (drawHelp_.transform.localPosition.x > 0f)
						{
							drawHelp_.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
						}
						else
						{
							drawHelp_.transform.localEulerAngles = Vector3.zero;
						}
						drawHelp_.SetActive(true);
					}
				}
				else if (drawHelp_.activeSelf)
				{
					drawHelp_.SetActive(false);
				}
				yield return 0;
			}
		}
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble chainBubble in chainBubbleDic[i])
			{
				if (chainBubble.type != Bubble.eType.ChainLock || !shotBubble.isNearBubble(chainBubble) || !unlockChain(chainBubble))
				{
					continue;
				}
				break;
			}
		}
		Bubble shotBubble2 = default(Bubble);
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (!fieldBubble.isLocked && fieldBubble.type != Bubble.eType.Blank && fieldBubble.type != Bubble.eType.Fulcrum && (fieldBubble.type < Bubble.eType.TunnelIn || fieldBubble.type > Bubble.eType.TunnelOutRightDown) && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fieldBubble.state == Bubble.eState.Field && shotBubble2.isNearBubble(fieldBubble) && !fieldBubble.inCloud && ((!drawList.Contains(fieldBubble) && (fieldBubble.type == Bubble.eType.Lightning || fieldBubble.type == Bubble.eType.LightningG || fieldBubble.type == Bubble.eType.Search || fieldBubble.type == Bubble.eType.Time || fieldBubble.type == Bubble.eType.Coin || fieldBubble.type == Bubble.eType.Star)) || (Bubble.eType.MorganaRed <= fieldBubble.type && Bubble.eType.MorganaBlack >= fieldBubble.type)))
			{
				drawList.Add(fieldBubble);
			}
		});
		foreach (Bubble breakBubble in drawList)
		{
			if (breakBubble.type != Bubble.eType.Water && breakBubble.state != Bubble.eState.Field)
			{
				continue;
			}
			if (breakBubble.type == Bubble.eType.Water || breakBubble.isFrozen)
			{
				frozenBreakList.Clear();
				List<Bubble> frozenBubbleList = fieldBubbleList.FindAll((Bubble fieldBubble) => fieldBubble.isFrozen && breakBubble.isNearBubble(fieldBubble) && !fieldBubble.inCloud);
				foreach (Bubble b2 in frozenBubbleList)
				{
					checkFrozen(b2, frozenBreakList);
				}
			}
			switch (breakBubble.type)
			{
			case Bubble.eType.Lightning:
			case Bubble.eType.LightningG:
				if (lightning(breakBubble))
				{
					bSearch = true;
				}
				break;
			case Bubble.eType.Search:
				bSearch = true;
				break;
			case Bubble.eType.Time:
			case Bubble.eType.Coin:
				specialEffect(breakBubble);
				break;
			case Bubble.eType.FriendRainbow:
			case Bubble.eType.Rainbow:
			case Bubble.eType.MinilenRainbow:
				if (!breakBubble.isFrozen)
				{
					breakLightningCount_++;
				}
				break;
			case Bubble.eType.Grow:
			case Bubble.eType.Skull:
			case Bubble.eType.FriendBox:
			case Bubble.eType.Box:
			case Bubble.eType.Rock:
			case Bubble.eType.Honeycomb:
			case Bubble.eType.Counter:
			case Bubble.eType.BlackHole_A:
			case Bubble.eType.BlackHole_B:
				if (breakBubble.isFrozen && frozenBreakList.Count == 0)
				{
					breakLightningCount_++;
				}
				break;
			}
			if (Bubble.eType.ChameleonRed <= breakBubble.type && Bubble.eType.Unknown >= breakBubble.type)
			{
				breakLightningCount_++;
			}
			if (Bubble.eType.MorganaRed <= breakBubble.type && Bubble.eType.MorganaBlack >= breakBubble.type && !breakBubble.IsSpecialBreak_)
			{
				StartCoroutine(breakBubble.SpecialBreak());
			}
			if (breakBubble.type == Bubble.eType.Time)
			{
				drawDiffTime -= TIME_ADD_VALUE;
			}
			if (breakBubble.type == Bubble.eType.Water || breakBubble.isFrozen)
			{
				foreach (Bubble b in frozenBreakList)
				{
					if (b.type == Bubble.eType.Time && b.state == Bubble.eState.Field)
					{
						drawDiffTime -= TIME_ADD_VALUE;
					}
				}
				if (breakFrozen(shotBubble))
				{
					bSearch = true;
				}
			}
			breakBubble.setSplashBreak(true);
			breakBubble.startBreak();
			plusEffect(breakBubble);
		}
		Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
		if (chainBreakRoutine != null)
		{
			updateChainLock();
		}
		yield return StartCoroutine(breakRoutine(shotBubble, breakLightningCount_));
		clearDrawLineList();
		clearDrawWaterLineList();
		if (bSearch)
		{
			yield return StartCoroutine(searchBubbleRoutine());
		}
		bDrawing = false;
	}

	private bool isInScreenBubble(Bubble b)
	{
		Vector3 vector = Camera.main.WorldToScreenPoint(b.transform.position);
		vector.y += 16f;
		return vector.y < (float)Screen.height;
	}

	private Bubble checkDrawFieldBubble(Vector3 point)
	{
		Vector3 zero = Vector3.zero;
		float num = Bubble.SQR_SIZE;
		Bubble result = null;
		foreach (Bubble fieldBubble in fieldBubbleList)
		{
			if (fieldBubble.type != Bubble.eType.Fulcrum && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fieldBubble.type != Bubble.eType.Invalid && fieldBubble.type != Bubble.eType.Blank && (Bubble.eType.MorganaRed > fieldBubble.type || Bubble.eType.MorganaBlack < fieldBubble.type) && (fieldBubble.type < Bubble.eType.TunnelIn || fieldBubble.type > Bubble.eType.TunnelOutRightDown) && !fieldBubble.isLocked && !fieldBubble.inCloud)
			{
				zero = (point - fieldBubble.myTrans.position) / uiScale;
				zero.z = 0f;
				float sqrMagnitude = zero.sqrMagnitude;
				if (!(sqrMagnitude > Bubble.SQR_SIZE * 0.8f) && sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = fieldBubble;
				}
			}
		}
		return result;
	}

	private bool checkDrawBubble(Vector3 point, Bubble bubble)
	{
		Vector3 zero = Vector3.zero;
		zero = (point - bubble.myTrans.position) / uiScale;
		zero.z = 0f;
		float sqrMagnitude = zero.sqrMagnitude;
		if (sqrMagnitude > Bubble.SQR_SIZE * 0.8f)
		{
			return false;
		}
		return true;
	}

	private IEnumerator setDrawHighLightEnable(bool enable, Bubble shotBubble, float duration)
	{
		FadeMng fadeMng = partManager.fade;
		if (enable)
		{
			shotBubble.myTrans.localPosition += Vector3.back * 355f;
			foreach (Bubble bubble in fieldBubbleList)
			{
				bubble.myTrans.localPosition += Vector3.back * 355f;
			}
			for (int i = 0; i < chainBubbleDic.Count; i++)
			{
				List<ChainBubble> chainList2 = chainBubbleDic[i];
				if (chainList2.Count != 0)
				{
					for (int l = 0; l < chainList2.Count; l++)
					{
						chainList2[l].myTrans.localPosition += Vector3.back * 355f;
					}
				}
			}
			foreach (Ivy iv in ivyList)
			{
				iv.gameObject.transform.localPosition += Vector3.back * 356f;
			}
			foreach (ObstacleDefend obj in obstacleList)
			{
				obj.transform.localPosition += Vector3.back * 356f;
			}
			GameObject pause_button = frontUi.Find("Top_ui/Gamestop_Button").gameObject;
			pause_button.GetComponent<UIButtonOffset>().enabled = false;
			pause_button.transform.localPosition += Vector3.back * 356f;
			fadeMng.setActive(FadeMng.eType.AllMask, true);
			yield return StartCoroutine(fadeMng.startFade(FadeMng.eType.AllMask, 0f, 0.6f, duration));
			yield break;
		}
		yield return StartCoroutine(fadeMng.startFade(FadeMng.eType.AllMask, 0.6f, 0f, duration));
		fadeMng.setActive(FadeMng.eType.AllMask, false);
		shotBubble.myTrans.localPosition += Vector3.forward * 355f;
		foreach (Bubble bubble2 in fieldBubbleList)
		{
			bubble2.myTrans.localPosition += Vector3.forward * 355f;
		}
		for (int j = 0; j < chainBubbleDic.Count; j++)
		{
			List<ChainBubble> chainList = chainBubbleDic[j];
			if (chainList.Count != 0)
			{
				for (int k = 0; k < chainList.Count; k++)
				{
					chainList[k].myTrans.localPosition += Vector3.forward * 355f;
				}
			}
		}
		foreach (Ivy iv2 in ivyList)
		{
			iv2.gameObject.transform.localPosition += Vector3.forward * 356f;
		}
		foreach (ObstacleDefend obj3 in obstacleList)
		{
			obj3.transform.localPosition += Vector3.forward * 356f;
		}
		GameObject pause_button2 = frontUi.Find("Top_ui/Gamestop_Button").gameObject;
		pause_button2.transform.localPosition += Vector3.forward * 356f;
		pause_button2.GetComponent<UIButtonOffset>().enabled = true;
		foreach (GameObject obj2 in drawLineList_)
		{
			Vector3 temp = obj2.transform.localPosition;
			temp.z = 0f;
			obj2.transform.localPosition = temp;
		}
	}

	private void updateDrawLine(Vector3 touch_pos, List<Bubble> drawList)
	{
		drawPointsBubble.Clear();
		for (int i = 0; i < drawList.Count; i++)
		{
			if (i < 2)
			{
				drawPointsBubble.Add(drawList[i]);
				continue;
			}
			Vector3 vector = drawList[i].myTrans.localPosition - drawList[i - 1].myTrans.localPosition;
			Vector3 vector2 = drawList[i - 1].myTrans.localPosition - drawList[i - 2].myTrans.localPosition;
			int num = (int)(Mathf.Atan2(vector.y, vector.x) * 57.29578f - 90f);
			int num2 = (int)(Mathf.Atan2(vector2.y, vector2.x) * 57.29578f - 90f);
			if (num == num2)
			{
				drawPointsBubble.Remove(drawPointsBubble[drawPointsBubble.Count - 1]);
			}
			drawPointsBubble.Add(drawList[i]);
		}
		if (drawLineList_.Count < drawPointsBubble.Count)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(drawLineBase_) as GameObject;
			gameObject.transform.parent = bubble_19_eff.transform.parent;
			gameObject.transform.localScale = Vector3.one;
			drawLineList_.Add(gameObject);
			drawLineBgList_.Add(gameObject.transform.Find("bg_00"));
			drawLineBgList_[drawLineBgList_.Count - 1].GetComponent<UISlicedSprite>().color = new Color(1f, 1f, 1f, 0.3f);
			if (drawLineBgList_.Count != 1)
			{
				drawLineBgList_[drawLineBgList_.Count - 2].GetComponent<UISlicedSprite>().color = new Color(1f, 1f, 1f, 1f);
			}
			gameObject.SetActive(true);
		}
		else if (drawLineList_.Count > drawPointsBubble.Count)
		{
			GameObject gameObject2 = drawLineList_[drawLineList_.Count - 1];
			drawLineList_.Remove(gameObject2);
			drawLineBgList_.Remove(gameObject2.transform.Find("bg_00"));
			UnityEngine.Object.DestroyImmediate(gameObject2);
			if (drawLineBgList_.Count > 0)
			{
				drawLineBgList_[drawLineBgList_.Count - 1].GetComponent<UISlicedSprite>().color = new Color(1f, 1f, 1f, 0.3f);
			}
		}
		for (int j = 0; j < drawLineList_.Count; j++)
		{
			drawLineList_[j].transform.position = drawPointsBubble[j].myTrans.position;
			Vector3 localPosition = drawLineList_[j].transform.localPosition;
			localPosition.z -= 2f;
			drawLineList_[j].transform.localPosition = localPosition;
		}
		for (int k = 0; k < drawLineList_.Count; k++)
		{
			Vector3 one = Vector3.one;
			if (k == drawLineList_.Count - 1)
			{
				if (Input.GetMouseButton(0))
				{
					one = drawTouchPoint_.transform.localPosition - drawLineList_[k].transform.localPosition;
					one.z = 0f;
					float z = Mathf.Atan2(one.y, one.x) * 57.29578f - 90f;
					float magnitude = one.magnitude;
					drawLineList_[k].transform.localEulerAngles = new Vector3(0f, 0f, z);
					drawLineBgList_[k].localScale = new Vector3(drawLineBgList_[k].localScale.x, magnitude, 1f);
				}
			}
			else
			{
				one = drawLineList_[k + 1].transform.localPosition - drawLineList_[k].transform.localPosition;
				one.z = 0f;
				float z2 = Mathf.Atan2(one.y, one.x) * 57.29578f - 90f;
				float magnitude2 = one.magnitude;
				drawLineList_[k].transform.localEulerAngles = new Vector3(0f, 0f, z2);
				drawLineBgList_[k].localScale = new Vector3(drawLineBgList_[k].localScale.x, magnitude2 + 10f, 1f);
			}
		}
		if (drawList.Count == 6)
		{
			drawLineList_[drawLineList_.Count - 1].SetActive(false);
		}
		else if (!drawLineList_[drawLineList_.Count - 1].activeSelf)
		{
			drawLineList_[drawLineList_.Count - 1].SetActive(true);
		}
	}

	private void setDrawCounter(Vector3 touch_pos, List<Bubble> drawList)
	{
		int num = 6 - drawList.Count;
		if (num > 9)
		{
			num = 9;
		}
		if (num != 0)
		{
			drawCounter_.transform.position = touch_pos;
		}
		else
		{
			drawCounter_.transform.position = drawList[drawList.Count - 1].myTrans.position;
		}
		drawTouchPoint_.transform.position = touch_pos;
		Vector3 localPosition = drawCounter_.transform.localPosition;
		localPosition.z = -750f;
		drawCounter_.transform.localPosition = localPosition;
		drawCounteSprite_.spriteName = "game_score_number_" + num.ToString("00");
		if (!drawCounter_.activeSelf)
		{
			drawCounter_.SetActive(true);
		}
	}

	private void clearDrawLineList()
	{
		foreach (GameObject item in drawLineList_)
		{
			UnityEngine.Object.DestroyImmediate(item);
		}
		drawLineList_.Clear();
		drawLineBgList_.Clear();
	}

	private IEnumerator waterPreBreakEffect()
	{
		WaterLineList_.Clear();
		List<GameObject> splash_eff_bg1 = new List<GameObject>();
		List<GameObject> splash_eff_bg2 = new List<GameObject>();
		for (int i = 0; i < drawPointsBubble.Count - 1; i++)
		{
			GameObject obj = UnityEngine.Object.Instantiate(drawWaterBase_) as GameObject;
			obj.transform.parent = drawWaterBase_.transform.parent.parent;
			obj.transform.localScale = Vector3.one;
			obj.transform.position = drawPointsBubble[i].myTrans.position;
			obj.transform.localPosition += Vector3.back * (10f + (float)i * 0.05f);
			WaterLineList_.Add(obj);
			splash_eff_bg1.Add(obj.transform.Find("bg_00").gameObject);
			splash_eff_bg2.Add(obj.transform.Find("bg_02").gameObject);
			obj.SetActive(false);
		}
		for (int j = 0; j < WaterLineList_.Count; j++)
		{
			Vector3 tempVec = drawPointsBubble[j + 1].myTrans.localPosition - drawPointsBubble[j].myTrans.localPosition;
			tempVec.z = 0f;
			float angle = Mathf.Atan2(tempVec.y, tempVec.x) * 57.29578f - 90f;
			float length = tempVec.magnitude + 20f;
			WaterLineList_[j].transform.localEulerAngles = new Vector3(0f, 0f, angle);
			splash_eff_bg1[j].transform.localScale = new Vector3(splash_eff_bg1[j].transform.localScale.x, 0.1f, 1f);
			splash_eff_bg2[j].transform.localScale = new Vector3(splash_eff_bg2[j].transform.localScale.x, 0.1f, 1f);
			WaterLineList_[j].SetActive(true);
			iTween.ScaleTo(splash_eff_bg1[j], iTween.Hash("y", length, "easetype", iTween.EaseType.easeInOutQuad, "time", length / 400f, "islocal", true));
			iTween.ScaleTo(splash_eff_bg2[j], iTween.Hash("y", length, "easetype", iTween.EaseType.easeInOutQuad, "time", length / 400f, "islocal", true));
			if (j == 0)
			{
				Sound.Instance.playSe(Sound.eSe.SE_516_water_bubble_break, false);
			}
			while ((splash_eff_bg1[j].GetComponent<iTween>() != null || splash_eff_bg2[j].GetComponent<iTween>() != null) && !stagePause.pause)
			{
				yield return 0;
			}
			if (stagePause.pause)
			{
				Sound.Instance.stopSe(Sound.eSe.SE_517_water_bubble_waterfall);
				clearDrawLineList();
				clearDrawWaterLineList();
				yield return stagePause.sync();
				yield break;
			}
		}
		float start_time = 0f;
		while (start_time < 1f)
		{
			start_time += Time.deltaTime;
			if (stagePause.pause)
			{
				Sound.Instance.stopSe(Sound.eSe.SE_517_water_bubble_waterfall);
			}
			yield return stagePause.sync();
			if (!Sound.Instance.isPlayingSe(Sound.eSe.SE_517_water_bubble_waterfall))
			{
				Sound.Instance.playSe(Sound.eSe.SE_517_water_bubble_waterfall, true);
			}
		}
		Sound.Instance.stopSe(Sound.eSe.SE_517_water_bubble_waterfall);
	}

	private void clearDrawWaterLineList()
	{
		foreach (GameObject item in WaterLineList_)
		{
			UnityEngine.Object.DestroyImmediate(item);
		}
		WaterLineList_.Clear();
	}

	private IEnumerator shineRoutine(Bubble shotBubble)
	{
		bool bSearch = false;
		float shotBubbleX = shotBubble.myTrans.localPosition.x;
		breakLightningCount_ = 0;
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble chainBubble in chainBubbleDic[i])
			{
				if (chainBubble.type == Bubble.eType.ChainLock && !chainBubble.mLocked)
				{
					float fieldBubbleX = chainBubble.myTrans.localPosition.x;
					float fieldBubbleY = chainBubble.myTrans.localPosition.y;
					if (fieldBubbleX > shotBubbleX - 60f && fieldBubbleX < shotBubbleX + 60f && fieldBubbleY <= 0f && unlockChain(chainBubble))
					{
						break;
					}
				}
			}
		}
		List<Bubble> breakBubbleList = new List<Bubble>();
		Bubble shotBubble2 = default(Bubble);
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (!fieldBubble.isLocked && fieldBubble.state == Bubble.eState.Field && fieldBubble.type != Bubble.eType.Fulcrum && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && (fieldBubble.type < Bubble.eType.TunnelIn || fieldBubble.type > Bubble.eType.TunnelOutRightDown) && fieldBubble.type != Bubble.eType.Blank)
			{
				float x = fieldBubble.myTrans.localPosition.x;
				float y = fieldBubble.myTrans.localPosition.y;
				if (shotBubble2.isNearBubble(fieldBubble) && y <= 0f)
				{
					if (fieldBubble.inCloud)
					{
						return;
					}
					if (fieldBubble.type == Bubble.eType.Lightning || fieldBubble.type == Bubble.eType.LightningG || fieldBubble.type == Bubble.eType.Search || fieldBubble.type == Bubble.eType.Time || fieldBubble.type == Bubble.eType.Coin || fieldBubble.type == Bubble.eType.Star || fieldBubble.isFrozen || (Bubble.eType.MorganaRed <= fieldBubble.type && Bubble.eType.MorganaBlack >= fieldBubble.type))
					{
						breakBubbleList.Add(fieldBubble);
					}
				}
				if (x > shotBubbleX - 60f && x < shotBubbleX + 60f && y <= 0f && !fieldBubble.inCloud)
				{
					breakBubbleList.Add(fieldBubble);
				}
			}
		});
		shotBubble.startBreak(false);
		StartCoroutine(shineEffect(shotBubble.myTrans.position));
		for (float waitTime = 0f; waitTime < 0.55f; waitTime += Time.deltaTime)
		{
			yield return stagePause.sync();
		}
		foreach (Bubble breakBubble in breakBubbleList)
		{
			if (breakBubble.state != Bubble.eState.Field)
			{
				continue;
			}
			switch (breakBubble.type)
			{
			case Bubble.eType.Lightning:
			case Bubble.eType.LightningG:
				if (lightning(breakBubble))
				{
					bSearch = true;
				}
				break;
			case Bubble.eType.Search:
				bSearch = true;
				break;
			case Bubble.eType.Time:
			case Bubble.eType.Coin:
				specialEffect(breakBubble);
				break;
			case Bubble.eType.FriendRainbow:
			case Bubble.eType.Rainbow:
			case Bubble.eType.MinilenRainbow:
				breakLightningCount_++;
				break;
			case Bubble.eType.Grow:
			case Bubble.eType.Skull:
			case Bubble.eType.FriendBox:
			case Bubble.eType.Box:
			case Bubble.eType.Rock:
			case Bubble.eType.Honeycomb:
			case Bubble.eType.Counter:
			case Bubble.eType.BlackHole_A:
			case Bubble.eType.BlackHole_B:
				if (breakBubble.isFrozen)
				{
					breakLightningCount_++;
				}
				break;
			}
			if (Bubble.eType.ChameleonRed <= breakBubble.type && Bubble.eType.Unknown >= breakBubble.type)
			{
				breakLightningCount_++;
			}
			if (Bubble.eType.MorganaRed <= breakBubble.type && Bubble.eType.MorganaBlack >= breakBubble.type && !breakBubble.IsSpecialBreak_)
			{
				StartCoroutine(breakBubble.SpecialBreak());
			}
			if (breakBubble.type == Bubble.eType.Skull)
			{
				breakBubble.setBreak();
			}
			else
			{
				breakBubble.startBreak();
			}
			if (breakBubble.isFrozen)
			{
				frozenBreakList.Clear();
				List<Bubble> frozenBubbleList = fieldBubbleList.FindAll((Bubble fieldBubble) => fieldBubble.isFrozen && breakBubble.isNearBubble(fieldBubble));
				foreach (Bubble b in frozenBubbleList)
				{
					checkFrozen(b, frozenBreakList);
				}
				if (breakFrozen(shotBubble))
				{
					bSearch = true;
				}
			}
			if (!breakBubble.isFrozen)
			{
				plusEffect(breakBubble);
			}
			if (breakBubble.type != Bubble.eType.Counter)
			{
				if (breakBubble.onObstacle != null)
				{
					StartCoroutine(breakBubble.onObstacle.breakRoutine(true));
				}
				UnityEngine.Object.Destroy(breakBubble.gameObject);
			}
		}
		Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
		if (chainBreakRoutine != null)
		{
			updateChainLock();
		}
		yield return StartCoroutine(breakRoutine(shotBubble, breakLightningCount_));
		nextScore = totalScore;
		if (bSearch)
		{
			yield return StartCoroutine(searchBubbleRoutine());
		}
		UnityEngine.Object.Destroy(shotBubble.gameObject);
		while (shineEffectInstance_ != null)
		{
			yield return stagePause.sync();
		}
	}

	private IEnumerator shineEffect(Vector3 target)
	{
		shineEffectInstance_ = UnityEngine.Object.Instantiate(bubble_20_eff) as GameObject;
		Transform effTrans = shineEffectInstance_.transform;
		Vector3 scale = effTrans.localScale;
		effTrans.parent = scrollUi;
		effTrans.localPosition = Vector3.back * 2f;
		effTrans.localScale = scale;
		charaObjs[0].transform.localPosition += Vector3.back * 5f;
		charaObjs[1].transform.localPosition += Vector3.back * 5f;
		if (arrow.charaIndex == 0 && nextBubbles[1] != null && nextBubbles[1].activeSelf)
		{
			nextBubbles[1].transform.localPosition += Vector3.back * 5f;
		}
		Vector3 pos = effTrans.position;
		pos.x = target.x;
		pos.y = 0f;
		effTrans.position = pos;
		shineEffectInstance_.SetActive(true);
		shineEffectInstance_.GetComponent<Animation>().Play();
		while (shineEffectInstance_.GetComponent<Animation>().isPlaying)
		{
			yield return null;
		}
		charaObjs[0].transform.localPosition += Vector3.forward * 5f;
		charaObjs[1].transform.localPosition += Vector3.forward * 5f;
		if (arrow.charaIndex == 0 && nextBubbles[1] != null && nextBubbles[1].activeSelf)
		{
			nextBubbles[1].transform.localPosition += Vector3.forward * 5f;
		}
		UnityEngine.Object.Destroy(shineEffectInstance_);
	}

	private IEnumerator boundEffRoutine(bool isLeft, Vector3 pos)
	{
		GameObject boundEff = ((!isLeft) ? boundEffR : boundEffL);
		Transform effTrans = boundEff.transform;
		effTrans.position = pos;
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

	private IEnumerator breakRoutine(Bubble shotBubble, int bubbleCountOffset)
	{
		int breakScore = 0;
		int dropScore = 0;
		int comboScore = 0;
		int reflectScore = 0;
		int rescueScore = 0;
		int skillScore = 0;
		int morganaScore = 0;
		if (rainbowChainCount == 0)
		{
			comboCount++;
			comboScore = (comboCount - 1) * COMBO_SCORE;
			if (shotBubble != null)
			{
				reflectScore = (shotBubble.boundCount - ((guide.metalPathIndex > -1) ? 2 : 0)) * REFLECT_SCORE;
			}
			bubbleCountOffset++;
		}
		int tempObjectCount = fieldObjectCount;
		int friendCount = fieldFriendCount;
		int bubbleCount2 = fieldBubbleList.Count + bubbleCountOffset;
		updateFieldBubbleList();
		yield return stagePause.sync();
		int breakCount = bubbleCount2 - fieldBubbleList.Count - (tempObjectCount - fieldObjectCount);
		if (bMinus)
		{
			Sound.Instance.playSe(Sound.eSe.SE_336_minusbubble);
			playCountdownEff(false);
		}
		if (bPlus)
		{
			Sound.Instance.playSe(Sound.eSe.SE_335_plusbubble);
			playCountdownEff(true);
		}
		bPlus = false;
		bMinus = false;
		if (breakFulcrum())
		{
			yield return stagePause.sync();
			updateFieldBubbleList();
		}
		yield return stagePause.sync();
		tempObjectCount = fieldObjectCount;
		bubbleCount2 = fieldBubbleList.Count;
		if (drop())
		{
			yield return stagePause.sync();
			updateFieldBubbleList();
		}
		yield return stagePause.sync();
		int dropCount = bubbleCount2 - fieldBubbleList.Count;
		totalDropCount += dropCount;
		if (totalDropCount >= 4)
		{
			bExcellent = true;
		}
		totalCoin += dropCount;
		totalSkillBonusCount += basicSkillBreakNum;
		breakScore = breakCount * BREAK_SCORE;
		dropScore = dropCount * DROP_SCORE;
		rescueScore = (friendCount - fieldFriendCount) * RESCUE_SCORE + minilen_count_pop_scored * MINILEN_SCORE;
		minilen_count_pop_scored = 0;
		skillScore = basicSkillBreakNum * SKILL_SCORE;
		morganaScore = breakMorganaCount * MORGANA_SCORE;
		int popupDrop = totalDropCount * DROP_SCORE * scoreUpNum;
		int popupSkill = totalSkillBonusCount * SKILL_SCORE * scoreUpNum;
		basicSkillBreakNum = 0;
		breakScore *= scoreUpNum;
		dropScore *= scoreUpNum;
		comboScore *= scoreUpNum;
		reflectScore *= scoreUpNum;
		rescueScore *= scoreUpNum;
		skillScore *= scoreUpNum;
		morganaScore *= scoreUpNum;
		int bonusScore = dropScore + comboScore + reflectScore + rescueScore + skillScore + morganaScore;
		totalScore += breakScore + bonusScore;
		Vector3 offset5 = Vector3.zero;
		popupScoreNormal[popupScoreIndex].startPopup(breakScore + comboScore + morganaScore, shotBubblePos, new Vector3(-120f, 0f, 0f));
		popupScoreIndex = (popupScoreIndex + 1) % 2;
		if (rainbowChainCount == 0)
		{
			if (comboCount >= 2)
			{
				popupCombo[popupComboIndex].GetComponent<PopupCombo>().startPopup(comboCount, shotBubblePos);
				popupComboIndex = (popupComboIndex + 1) % 2;
			}
			if (bExcellent)
			{
				popupExcellent.startPopup(shotBubblePos);
			}
		}
		offset5 = popupScoreDrop.startPopup(popupDrop, bonusBasePos, offset5);
		offset5 = popupScoreReflect.startPopup(reflectScore, bonusBasePos, offset5);
		offset5 = popupScoreRescue.startPopup(rescueScore, bonusBasePos, offset5);
		offset5 = popupScoreSkill.startPopup(popupSkill, bonusBasePos, offset5);
		if (bonusScore > 0 && dropSnakeList.Count <= 0)
		{
			if (snakeCount_ == 0)
			{
				StopCoroutine("chara00BonusAnim");
				StartCoroutine("chara00BonusAnim");
			}
			else
			{
				StopCoroutine("chara01BonusAnim");
				StartCoroutine("chara01BonusAnim");
			}
		}
		if (bExcellent)
		{
			int max = ((prevVoice != eVoice.Invalid) ? 3 : 4);
			int voice = random.Next(0, max);
			if (voice == (int)prevVoice)
			{
				voice = 3;
			}
			switch ((eVoice)voice)
			{
			case eVoice.Bravo:
				Sound.Instance.playSe(Sound.eSe.SE_105_Bravo);
				break;
			case eVoice.Fantastic:
				Sound.Instance.playSe(Sound.eSe.SE_106_Fantastic);
				break;
			case eVoice.Wow:
				Sound.Instance.playSe(Sound.eSe.SE_107_Wow);
				break;
			case eVoice.Yay:
				Sound.Instance.playSe(Sound.eSe.SE_108_Yay);
				break;
			}
			prevVoice = (eVoice)voice;
			Sound.Instance.playSe(Sound.eSe.SE_113_Clap);
		}
		rainbowChainCount++;
		breakMorganaCount = 0;
		yield return StartCoroutine(lineFriendBonusRoutine());
		if (bGetKey)
		{
			yield return StartCoroutine(GetkeyBubbleRoutine());
		}
	}

	private IEnumerator breakChameleonRoutine(Bubble shotBubble, int bubbleCountOffset)
	{
		int breakScore = 0;
		int dropScore = 0;
		int comboScore = 0;
		int reflectScore = 0;
		int rescueScore = 0;
		int skillScore = 0;
		int morganaScore = 0;
		if (rainbowChainCount == 0)
		{
			comboCount++;
			comboScore = (comboCount - 1) * COMBO_SCORE;
			if (shotBubble != null)
			{
				reflectScore = shotBubble.boundCount * REFLECT_SCORE;
			}
			bubbleCountOffset++;
		}
		int friendCount = fieldFriendCount;
		int bubbleCount2 = fieldBubbleList.Count + bubbleCountOffset;
		updateFieldBubbleList();
		yield return stagePause.sync();
		int breakCount = chainedChameleonCount;
		if (bMinus)
		{
			Sound.Instance.playSe(Sound.eSe.SE_336_minusbubble);
			playCountdownEff(false);
		}
		if (bPlus)
		{
			Sound.Instance.playSe(Sound.eSe.SE_335_plusbubble);
			playCountdownEff(true);
		}
		bPlus = false;
		bMinus = false;
		if (breakFulcrum())
		{
			yield return stagePause.sync();
			updateFieldBubbleList();
		}
		yield return stagePause.sync();
		bubbleCount2 = fieldBubbleList.Count;
		if (drop())
		{
			yield return stagePause.sync();
			updateFieldBubbleList();
		}
		yield return stagePause.sync();
		int dropCount = bubbleCount2 - fieldBubbleList.Count;
		totalDropCount += dropCount;
		if (totalDropCount >= 4)
		{
			bExcellent = true;
		}
		totalCoin += dropCount;
		totalSkillBonusCount += basicSkillBreakNum;
		breakScore = breakCount * BREAK_SCORE;
		dropScore = dropCount * DROP_SCORE;
		rescueScore = (friendCount - fieldFriendCount) * RESCUE_SCORE + minilen_count_pop_scored * MINILEN_SCORE;
		minilen_count_pop_scored = 0;
		skillScore = basicSkillBreakNum * SKILL_SCORE;
		morganaScore = breakMorganaCount * MORGANA_SCORE;
		basicSkillBreakNum = 0;
		int popupDrop = totalDropCount * DROP_SCORE * scoreUpNum;
		int popupSkill = totalSkillBonusCount * SKILL_SCORE * scoreUpNum;
		breakScore *= scoreUpNum;
		dropScore *= scoreUpNum;
		comboScore *= scoreUpNum;
		reflectScore *= scoreUpNum;
		rescueScore *= scoreUpNum;
		skillScore *= scoreUpNum;
		morganaScore *= scoreUpNum;
		int bonusScore = dropScore + comboScore + reflectScore + rescueScore + skillScore + morganaScore;
		totalScore += breakScore + bonusScore;
		Vector3 offset5 = Vector3.zero;
		popupScoreNormal[popupScoreIndex].startPopup(breakScore + comboScore + morganaScore, shotBubblePos, new Vector3(-120f, 80f, 0f));
		popupScoreIndex = (popupScoreIndex + 1) % 2;
		if (rainbowChainCount == 0)
		{
			if (comboCount >= 2)
			{
				popupCombo[popupComboIndex].GetComponent<PopupCombo>().startPopup(comboCount, shotBubblePos);
				popupComboIndex = (popupComboIndex + 1) % 2;
			}
			if (bExcellent)
			{
				popupExcellent.startPopup(shotBubblePos);
			}
		}
		offset5 = popupScoreDrop.startPopup(popupDrop, bonusBasePos, offset5);
		offset5 = popupScoreReflect.startPopup(reflectScore, bonusBasePos, offset5);
		offset5 = popupScoreRescue.startPopup(rescueScore, bonusBasePos, offset5);
		offset5 = popupScoreSkill.startPopup(popupSkill, bonusBasePos, offset5);
		if (bonusScore > 0 && dropSnakeList.Count <= 0)
		{
			if (snakeCount_ == 0)
			{
				StopCoroutine("chara00BonusAnim");
				StartCoroutine("chara00BonusAnim");
			}
			else
			{
				StopCoroutine("chara01BonusAnim");
				StartCoroutine("chara01BonusAnim");
			}
		}
		if (bExcellent)
		{
			int max = ((prevVoice != eVoice.Invalid) ? 3 : 4);
			int voice = random.Next(0, max);
			if (voice == (int)prevVoice)
			{
				voice = 3;
			}
			switch ((eVoice)voice)
			{
			case eVoice.Bravo:
				Sound.Instance.playSe(Sound.eSe.SE_105_Bravo);
				break;
			case eVoice.Fantastic:
				Sound.Instance.playSe(Sound.eSe.SE_106_Fantastic);
				break;
			case eVoice.Wow:
				Sound.Instance.playSe(Sound.eSe.SE_107_Wow);
				break;
			case eVoice.Yay:
				Sound.Instance.playSe(Sound.eSe.SE_108_Yay);
				break;
			}
			prevVoice = (eVoice)voice;
			Sound.Instance.playSe(Sound.eSe.SE_113_Clap);
		}
		rainbowChainCount++;
		breakMorganaCount = 0;
		yield return StartCoroutine(lineFriendBonusRoutine());
		chainedChameleonCount = 0;
	}

	private IEnumerator chara00BonusAnim()
	{
		if (isHoneycombHitWait || snakeCount_ != 0)
		{
			yield break;
		}
		charaAnims[0].Play(CHARA_SPRITE_ANIMATION_HEADER[0] + "04");
		while (charaAnims[0].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[0] + "04"))
		{
			yield return stagePause.sync();
		}
		if (state != eState.Gameover)
		{
			if (arrow.charaIndex != 0)
			{
				charaAnims[1].Play(waitAnimName);
			}
			else
			{
				charaAnims[0].Play(waitAnimName);
			}
		}
	}

	private IEnumerator chara01BonusAnim()
	{
		if (isHoneycombHitWait || snakeCount_ == 0)
		{
			yield break;
		}
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "04");
		while (charaAnims[1].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[1] + "04"))
		{
			yield return stagePause.sync();
		}
		if (state != eState.Gameover && state != eState.Clear)
		{
			updateShootCharacter(false);
			if (arrow.charaIndex != 0)
			{
				charaAnims[1].Play(waitAnimName);
			}
			else
			{
				charaAnims[0].Play(waitAnimName);
			}
		}
	}

	private void checkSamColor(Bubble me, ref float[] checkedFlags, List<Bubble> rainbowList)
	{
		if (!me.isColorBubble())
		{
			return;
		}
		List<Bubble> list = new List<Bubble>(6);
		List<int> list2 = new List<int>(6);
		for (int i = 0; i < fieldBubbleList.Count; i++)
		{
			Bubble bubble = fieldBubbleList[i];
			if (me.GetInstanceID() != bubble.GetInstanceID() && checkedFlags[i] == 0f && bubble.state == Bubble.eState.Field && !bubble.isLocked && !bubble.isFrozen && me.isNearBubble(bubble) && (!me.isNearBubble(bubble) || !bubble.inCloud))
			{
				list2.Add(i);
				list.Add(bubble);
			}
		}
		List<Bubble> list3 = new List<Bubble>();
		Bubble.eType eType = convertColorBubble(me.type);
		for (int j = 0; j < list.Count; j++)
		{
			Bubble.eType type = list[j].type;
			if (eType == convertColorBubble(type))
			{
				checkedFlags[list2[j]] = list[j].bubblePower;
				list3.Add(list[j]);
				continue;
			}
			if (type == Bubble.eType.Rainbow || type == Bubble.eType.FriendRainbow || type == Bubble.eType.MinilenRainbow)
			{
				rainbowList.Add(list[j]);
			}
			checkedFlags[list2[j]] = -1f;
		}
		for (int k = 0; k < list3.Count; k++)
		{
			checkSamColor(list3[k], ref checkedFlags, rainbowList);
		}
	}

	private void checkSamColorFixed(Bubble me, ref int[] checkedFlags, List<Bubble> rainbowList)
	{
		if (!me.isColorBubbleFixed())
		{
			return;
		}
		List<Bubble> list = new List<Bubble>(6);
		List<int> list2 = new List<int>(6);
		for (int i = 0; i < fieldBubbleList.Count; i++)
		{
			Bubble bubble = fieldBubbleList[i];
			if (me.GetInstanceID() != bubble.GetInstanceID() && checkedFlags[i] == 0 && bubble.state == Bubble.eState.Field && !bubble.isLocked && !bubble.isFrozen && me.isNearBubble(bubble) && (!me.isNearBubble(bubble) || !bubble.inCloud))
			{
				list2.Add(i);
				list.Add(bubble);
			}
		}
		List<Bubble> list3 = new List<Bubble>();
		Bubble.eType eType = convertColorBubbleFixed(me.type);
		for (int j = 0; j < list.Count; j++)
		{
			Bubble.eType type = list[j].type;
			if (eType == convertColorBubbleFixed(type))
			{
				checkedFlags[list2[j]] = 1;
				list3.Add(list[j]);
				continue;
			}
			if (type == Bubble.eType.Rainbow || type == Bubble.eType.FriendRainbow || type == Bubble.eType.MinilenRainbow)
			{
				rainbowList.Add(list[j]);
			}
			checkedFlags[list2[j]] = -1;
		}
		for (int k = 0; k < list3.Count; k++)
		{
			checkSamColorFixed(list3[k], ref checkedFlags, rainbowList);
		}
	}

	private void checkFrozen(Bubble me, List<Bubble> list)
	{
		if (!list.Contains(me))
		{
			list.Add(me);
			List<Bubble> list2 = fieldBubbleList.FindAll((Bubble bubble) => bubble.isFrozen && !list.Contains(bubble) && me.isNearBubble(bubble));
			list2.ForEach(delegate(Bubble bubble)
			{
				checkFrozen(bubble, list);
			});
		}
	}

	private IEnumerator hitDefault(Bubble shotBubble)
	{
		int breakNum_ = 0;
		List<Bubble> rainbowList = new List<Bubble>();
		breakLightningCount_ = 0;
		if (!shotBubble.isHitCloud && breakFrozen(shotBubble))
		{
			bSearch = true;
			shotBubble.startBreak();
		}
		else if (shotBubble.type != Bubble.eType.Shake)
		{
			float[] checkedFlags = new float[fieldBubbleList.Count];
			checkSamColor(shotBubble, ref checkedFlags, rainbowList);
			float chainCount = 0f;
			float[] array = checkedFlags;
			foreach (float checkedFlag in array)
			{
				if (checkedFlag > 0f)
				{
					chainCount += checkedFlag;
				}
			}
			if (rainbowChainCount == 0)
			{
				chainCount += shotBubble.bubblePower;
			}
			if (chainCount < 3f)
			{
				if (frozenBreakList.Count > 0)
				{
					shotBubble.startBreak();
					breakNum_ = 1;
				}
				else if (rainbowChainCount == 0)
				{
					shotBubble.setFieldState();
					shotBubble.myTrans.parent = bubbleRoot;
					Vector3 p = shotBubble.myTrans.localPosition;
					p.z = 0f;
					shotBubble.myTrans.localPosition = p;
					if (!shotBubble.isHitCloud)
					{
						fieldBubbleList.Add(shotBubble);
					}
				}
			}
			else
			{
				shotBubble.startBreak();
				for (int l = 0; l < fieldBubbleList.Count; l++)
				{
					if (!(checkedFlags[l] <= 0f))
					{
						if (rainbowChainCount > 0)
						{
							fieldBubbleList[l].isLineFriend = false;
						}
						fieldBubbleList[l].startBreak();
						plusMinusEffect(fieldBubbleList[l]);
						breakNum_++;
					}
				}
				int boundCount = shotBubble.boundCount;
				if (breakNum_ >= 8 || boundCount >= 2)
				{
					bExcellent = true;
				}
				if (breakNum_ >= 5)
				{
					Sound.Instance.playSe(Sound.eSe.SE_219_daihakai);
				}
				else
				{
					Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
				}
				for (int k = 0; k < rainbowList.Count; k++)
				{
					Bubble t = rainbowList[k];
					if (t.type == Bubble.eType.FriendRainbow)
					{
						t.setType(convertColorBubble(shotBubble.type) + 31);
					}
					else if (t.type == Bubble.eType.MinilenRainbow)
					{
						t.setType(convertColorBubble(shotBubble.type) + 128);
					}
					else
					{
						t.setType(convertColorBubble(shotBubble.type));
					}
					breakLightningCount_++;
				}
			}
		}
		else
		{
			breakNum_ = 1;
		}
		if (shotBubble.isHitCloud)
		{
			breakNum_ = 1;
			if (shotBubble.isBasicSkillColor && bBasicSkill)
			{
				basicSkillBreakNum++;
				shotBubble.isBasicSkillColor = false;
			}
		}
		int breakFrozenCount = frozenBreakList.Count;
		frozenBreakList.Clear();
		if (rainbowChainCount == 0)
		{
			List<Bubble> nearBubbleList = new List<Bubble>(6);
			Bubble shotBubble2 = default(Bubble);
			fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
			{
				if (!fieldBubble.isLocked && fieldBubble.state == Bubble.eState.Field && shotBubble2.isNearBubble(fieldBubble) && (!shotBubble2.isNearBubble(fieldBubble) || !fieldBubble.inCloud))
				{
					nearBubbleList.Add(fieldBubble);
				}
			});
			foreach (Bubble nearBubble in nearBubbleList)
			{
				if (nearBubble.type >= Bubble.eType.MorganaRed && nearBubble.type <= Bubble.eType.MorganaBlack)
				{
					Bubble parent = nearBubble.ParentMorgana_;
					if (shotBubble.type == parent.type - 109 && !nearBubble.IsChangeColor_ && !parent.OnDamage_)
					{
						StartCoroutine(parent.Hit());
						breakNum_++;
						shotBubble.startBreak();
						Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
						fieldBubbleList.Remove(shotBubble);
					}
				}
				bool bBreak = true;
				switch (nearBubble.type)
				{
				case Bubble.eType.Star:
					star(shotBubble);
					break;
				case Bubble.eType.Lightning:
					if (lightning(nearBubble))
					{
						bSearch = true;
					}
					break;
				case Bubble.eType.Search:
					bSearch = true;
					break;
				case Bubble.eType.Time:
				case Bubble.eType.Coin:
					specialEffect(nearBubble);
					break;
				default:
					bBreak = false;
					break;
				}
				if (bBreak)
				{
					nearBubble.startBreak();
					breakNum_++;
					shotBubble.startBreak();
					fieldBubbleList.Remove(shotBubble);
				}
			}
			for (int j = 0; j < chainBubbleDic.Count; j++)
			{
				foreach (ChainBubble chainBubble in chainBubbleDic[j])
				{
					if (chainBubble.type == Bubble.eType.ChainLock && shotBubble.isNearBubble(chainBubble))
					{
						breakNum_++;
						shotBubble.startBreak();
						fieldBubbleList.Remove(shotBubble);
						if (unlockChain(chainBubble))
						{
							break;
						}
					}
				}
			}
		}
		if (chainBreakRoutine != null)
		{
			updateChainLock();
			breakNum_ = 1;
		}
		if (breakNum_ > 0)
		{
			yield return StartCoroutine(breakRoutine(shotBubble, breakLightningCount_));
			if (rainbowList.Count > 0)
			{
				float waitTime = 0f;
				while (waitTime < 0.3f)
				{
					waitTime += Time.deltaTime;
					yield return stagePause.sync();
				}
			}
			for (int i = 0; i < rainbowList.Count; i++)
			{
				if (rainbowList[i] != null && rainbowList[i].state == Bubble.eState.Field)
				{
					yield return StartCoroutine(hitDefault(rainbowList[i]));
				}
			}
		}
		else
		{
			if (breakFrozenCount > 0)
			{
				updateFieldBubbleList();
				breakFulcrum();
				yield return StartCoroutine(breakRoutine(null, breakFrozenCount));
			}
			if (rainbowChainCount == 0)
			{
				comboCount = 0;
			}
			updateFieldBubbleList();
		}
	}

	private void star(Bubble shotBubble)
	{
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (Bubble.eType.MorganaRed <= fieldBubble.type && Bubble.eType.MorganaBlack >= fieldBubble.type && !fieldBubble.IsChangeColor_ && fieldBubble.ParentMorgana_ != null)
			{
				fieldBubble.ParentMorgana_.OnDamage_ = false;
			}
		});
		Sound.Instance.playSe(Sound.eSe.SE_332_starbubble);
		Bubble.eType shotBubbleType = shotBubble.type;
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (!fieldBubble.isLocked && !fieldBubble.isFrozen && !fieldBubble.inCloud)
			{
				Bubble.eType eType = convertColorBubble(fieldBubble.type);
				if (eType >= Bubble.eType.ChameleonRed && eType <= Bubble.eType.ChameleonBlack)
				{
					eType -= 79;
				}
				if (eType >= Bubble.eType.MorganaRed && eType <= Bubble.eType.MorganaBlack)
				{
					eType -= 109;
				}
				if (shotBubbleType == eType)
				{
					if (Bubble.eType.MorganaRed <= fieldBubble.type && Bubble.eType.MorganaBlack >= fieldBubble.type && !fieldBubble.IsChangeColor_)
					{
						if (fieldBubble.ParentMorgana_ != null && !fieldBubble.ParentMorgana_.OnDamage_)
						{
							StartCoroutine(fieldBubble.ParentMorgana_.Hit());
						}
					}
					else
					{
						fieldBubble.startBreak();
					}
				}
			}
		});
	}

	private bool lightning(Bubble hitBubble)
	{
		Sound.Instance.playSe(Sound.eSe.SE_330_thunderbubble);
		bool bSearch = false;
		float hitBubbleY = hitBubble.myTrans.localPosition.y;
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble item in chainBubbleDic[i])
			{
				if (item.type == Bubble.eType.ChainLock)
				{
					float y = item.myTrans.localPosition.y;
					if (y > hitBubbleY - 1f && y < hitBubbleY + 1f && unlockChain(item))
					{
						break;
					}
				}
			}
		}
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (!(hitBubble == fieldBubble) && !fieldBubble.isLocked && !fieldBubble.inCloud && fieldBubble.state == Bubble.eState.Field && fieldBubble.type != Bubble.eType.Fulcrum && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && (fieldBubble.type < Bubble.eType.TunnelIn || fieldBubble.type > Bubble.eType.TunnelOutRightDown))
			{
				float y2 = fieldBubble.myTrans.localPosition.y;
				if (y2 > hitBubbleY - 1f && y2 < hitBubbleY + 1f)
				{
					if (fieldBubble.type == Bubble.eType.Search)
					{
						bSearch = true;
					}
					switch (fieldBubble.type)
					{
					case Bubble.eType.FriendRainbow:
					case Bubble.eType.Rainbow:
					case Bubble.eType.MinilenRainbow:
						breakLightningCount_++;
						break;
					case Bubble.eType.Grow:
					case Bubble.eType.Skull:
					case Bubble.eType.FriendBox:
					case Bubble.eType.Box:
					case Bubble.eType.Rock:
					case Bubble.eType.Honeycomb:
					case Bubble.eType.Counter:
					case Bubble.eType.BlackHole_A:
					case Bubble.eType.BlackHole_B:
						if (fieldBubble.isFrozen)
						{
							breakLightningCount_++;
						}
						break;
					}
					if (Bubble.eType.ChameleonRed <= fieldBubble.type && Bubble.eType.Unknown >= fieldBubble.type)
					{
						breakLightningCount_++;
					}
					if (Bubble.eType.MorganaRed <= fieldBubble.type && Bubble.eType.MorganaBlack >= fieldBubble.type && !fieldBubble.IsSpecialBreak_)
					{
						StartCoroutine(fieldBubble.SpecialBreak());
					}
					specialEffect(fieldBubble);
					plusMinusEffect(fieldBubble);
					if (fieldBubble.type == Bubble.eType.Skull)
					{
						fieldBubble.setBreak();
						fieldBubble.startFadeout();
					}
					else
					{
						fieldBubble.startBreak();
					}
				}
			}
		});
		lightningEffectRoutine = StartCoroutine(lightningEffect(hitBubble.myTrans.position));
		return bSearch;
	}

	private IEnumerator lightningEffect(Vector3 target)
	{
		GameObject eff = UnityEngine.Object.Instantiate(bubble_08_eff) as GameObject;
		Transform effTrans = eff.transform;
		Vector3 scale = effTrans.localScale;
		effTrans.parent = bubble_08_eff.transform.parent;
		effTrans.localPosition = Vector3.zero;
		effTrans.localScale = scale;
		Vector3 pos = effTrans.position;
		pos.x = target.x;
		pos.y = target.y;
		effTrans.position = pos;
		eff.SetActive(true);
		UISpriteAnimationEx[] anims = eff.GetComponentsInChildren<UISpriteAnimationEx>();
		while (true)
		{
			bool bEnd = true;
			UISpriteAnimationEx[] array = anims;
			foreach (UISpriteAnimationEx anim in array)
			{
				if (anim.isPlaying)
				{
					bEnd = false;
					break;
				}
			}
			if (bEnd)
			{
				break;
			}
			yield return stagePause.sync();
		}
		UnityEngine.Object.Destroy(eff);
	}

	private void specialEffect(Bubble bubble)
	{
		if (bubble.state != Bubble.eState.Field)
		{
			return;
		}
		switch (bubble.type)
		{
		case Bubble.eType.Coin:
			Sound.Instance.playSe(Sound.eSe.SE_334_coinbubble);
			totalEventCoin += COIN_ADD_VALUE;
			friendBonusPop(eFriendBonus.Coin, COIN_ADD_VALUE, bubble.myTrans.parent, bubble.myTrans.position);
			break;
		case Bubble.eType.Time:
			Sound.Instance.playSe(Sound.eSe.SE_333_timebubble);
			if (gameType == eGameType.Time)
			{
				if (stagePause.pause)
				{
					stagePause.diffTime -= TIME_ADD_VALUE;
				}
				else
				{
					startTime += TIME_ADD_VALUE;
				}
				tempReplayDiffTime -= TIME_ADD_VALUE;
				int num = (int)((float)stageInfo.Time - (Time.time - startTime) + 0.999999f);
				if ((float)num > 10f)
				{
					bCrisis = false;
					bShowedLastPoint = false;
					countLabel.color = REST_COUNT_DEFAULT_COLOR;
				}
			}
			friendBonusPop(eFriendBonus.CountPlus, TIME_ADD_VALUE, bubble.myTrans.parent, bubble.myTrans.position);
			playCountdownEff(true);
			break;
		}
	}

	private void plusEffect(Bubble bubble)
	{
		if (bubble.type < Bubble.eType.PlusRed || bubble.type > Bubble.eType.PlusBlack)
		{
			return;
		}
		bPlus = true;
		shotCount -= PLUS_ADD_VALUE;
		friendBonusPop(eFriendBonus.CountPlus, PLUS_ADD_VALUE, bubble.myTrans.parent, bubble.myTrans.position);
		if (gameType == eGameType.ShotCount)
		{
			int num = stageInfo.Move - shotCount;
			if (num > 5)
			{
				bCrisis = false;
				bShowedLastPoint = false;
				countLabel.color = REST_COUNT_DEFAULT_COLOR;
			}
		}
	}

	private void minusEffect(Bubble bubble)
	{
		if (!isUsedItem(Constant.Item.eType.MinusGuard) && bubble.type >= Bubble.eType.MinusRed && bubble.type <= Bubble.eType.MinusBlack && !bubble.isFrozen)
		{
			bMinus = true;
			shotCount += PLUS_ADD_VALUE;
			if (shotCount > stageInfo.Move)
			{
				shotCount = stageInfo.Move;
			}
			friendBonusPop(eFriendBonus.CountMinus, PLUS_ADD_VALUE, bubble.myTrans.parent, bubble.myTrans.position);
		}
	}

	private void snakeEffect(Bubble bubble)
	{
		friendBonusPop(eFriendBonus.Snake, 3, bubble.myTrans.parent, bubble.myTrans.position);
	}

	private void plusMinusEffect(Bubble bubble)
	{
		plusEffect(bubble);
		minusEffect(bubble);
	}

	public Transform getScrollUI()
	{
		return scrollUi;
	}

	private IEnumerator searchBubbleRoutine()
	{
		bool isPlaying = false;
		if (checkClear(true))
		{
			isSearching = false;
			{
				foreach (Chackn c4 in chacknList)
				{
					if (c4 != null)
					{
						c4.animStart(stagePause);
					}
				}
				yield break;
			}
		}
		eState prevState = state;
		state = eState.Search;
		float diffTime = Time.time - startTime;
		float maxY = -2000f;
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			float y = fieldBubble.myTrans.localPosition.y;
			if (y > maxY)
			{
				maxY = y;
			}
		});
		if (maxY > -2000f)
		{
			int canDownLine = (int)((maxY + 1f) / 52f);
			if (canDownLine > 0)
			{
				isSearching = true;
				Sound.Instance.playSe(Sound.eSe.SE_331_searchbubble01);
				if (cloudList != null)
				{
					foreach (Cloud cloud2 in cloudList)
					{
						cloud2.transform.parent = frontUi.parent.Find("ScrollUI");
					}
				}
				iTween.MoveTo(scrollUi.gameObject, iTween.Hash("y", -52 * canDownLine, "easetype", iTween.EaseType.linear, "time", 0.1f * (float)canDownLine, "islocal", true));
				moveSpan = -52 * canDownLine;
				isMoving = true;
				while (scrollUi.GetComponent<iTween>() != null)
				{
					yield return stagePause.sync();
					foreach (Chackn c in chacknList)
					{
						if (c != null && c.isAnimationPlaying())
						{
							c.animationPause();
						}
					}
				}
				isMoving = false;
				foreach (Chackn c3 in chacknList)
				{
					if (c3 != null && !c3.isAnimationPlaying())
					{
						c3.transform.parent = stageUi.parent.parent;
						c3.animationReStart();
					}
				}
				float waitTime = 0f;
				while (waitTime < 0.5f)
				{
					waitTime += Time.deltaTime;
					yield return stagePause.sync();
				}
				isPlaying = true;
				while (isPlaying)
				{
					isPlaying = false;
					foreach (Chackn c2 in chacknList)
					{
						if (c2 != null && c2.isAnimationPlaying())
						{
							isPlaying = true;
							break;
						}
					}
					yield return stagePause.sync();
				}
				Sound.Instance.playSe(Sound.eSe.SE_331_searchbubble02);
				iTween.MoveTo(scrollUi.gameObject, iTween.Hash("y", 0, "easetype", iTween.EaseType.linear, "time", 0.1f * (float)canDownLine, "islocal", true));
				while (scrollUi.GetComponent<iTween>() != null)
				{
					yield return stagePause.sync();
				}
				if (cloudList != null)
				{
					foreach (Cloud cloud in cloudList)
					{
						cloud.transform.parent = scrollUi.parent.Find("Front_ui");
					}
				}
			}
			isSearching = false;
		}
		startTime = Time.time - diffTime;
		state = prevState;
	}

	public float getMoveSpan()
	{
		return moveSpan;
	}

	public Transform getTransform()
	{
		return scrollUi.transform;
	}

	private bool grow()
	{
		bool flag = false;
		if (growBubbleList.Count == 0)
		{
			return flag;
		}
		if (isHoneycombHitWait)
		{
			return flag;
		}
		if (bUsingTimeStop)
		{
			return flag;
		}
		List<Bubble> list = new List<Bubble>();
		growBubbleList.ForEach(delegate(Bubble growBubble)
		{
			if (!growBubble.isFrozen)
			{
				growCandidateList.Clear();
				fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
				{
					if (!(fieldBubble.myTrans.localPosition.y > growBubble.myTrans.localPosition.y - 1f) && growBubble.isNearBubble(fieldBubble) && fieldBubble.isColorBubble() && (!fieldBubble.isLocked || !fieldBubble.isOnChain) && !growBubble.isOnChain && !fieldBubble.isFrozen && growBubble.inCloud == fieldBubble.inCloud)
					{
						for (int i = 0; i < fieldBubbleList.Count; i++)
						{
							if (!fieldBubbleList[i].isLocked && (fieldBubbleList[i].type == Bubble.eType.Fulcrum || fieldBubbleList[i].type == Bubble.eType.RotateFulcrumL || fieldBubbleList[i].type == Bubble.eType.RotateFulcrumR || fieldBubbleList[i].type == Bubble.eType.Honeycomb) && fieldBubble.isNearBubble(fieldBubbleList[i]) && (fieldBubbleList[i].type < Bubble.eType.MorganaRed || fieldBubbleList[i].type > Bubble.eType.MorganaBlack))
							{
								return;
							}
						}
						for (int j = 0; j < fieldBubbleList.Count; j++)
						{
							if (fieldBubbleList[j].type == Bubble.eType.Counter && fieldBubble.isNearBubble(fieldBubbleList[j]))
							{
								return;
							}
						}
						for (int k = 0; k < fieldBubbleList.Count; k++)
						{
							if (fieldBubbleList[k].type >= Bubble.eType.TunnelIn && fieldBubbleList[k].type <= Bubble.eType.TunnelOutRightDown && fieldBubble.isNearBubble(fieldBubbleList[k]))
							{
								return;
							}
						}
						growCandidateList.Add(fieldBubble);
					}
				});
				if (growCandidateList.Count != 0)
				{
					list.Add(growCandidateList[rand.Next(growCandidateList.Count)]);
				}
			}
		});
		if (list.Count > 0)
		{
			Bubble bubble = list[rand.Next(list.Count)];
			if (bubble.type >= Bubble.eType.FriendRed && bubble.type <= Bubble.eType.FriendBlack)
			{
				gameoverType = eGameover.FriendToGrow;
				flag = true;
			}
			if (bubble.type >= Bubble.eType.MinilenRed && bubble.type <= Bubble.eType.MinilenBlack)
			{
				gameoverType = eGameover.MinilenToGrow;
				flag = true;
			}
			bubble.setType(Bubble.eType.Grow);
			bubble.setLineFriend(-1);
			Sound.Instance.playSe(Sound.eSe.SE_227_tuika);
			if (bubble.onObstacle != null)
			{
				bubble.onObstacle.breakRoutine(false);
			}
			if (!flag)
			{
				updateFieldBubbleList();
			}
		}
		return flag;
	}

	private void searchNextBubble()
	{
		if (ceilingBaseY > ceilingBubbleList[0].localPosition.y)
		{
			ceilingBaseY = ceilingBubbleList[0].localPosition.y;
		}
		searchedBubbleTypeList.Clear();
		searchedBubbleList.Clear();
		searchedPreBubbleTypeList.Clear();
		bool[] array = new bool[fieldBubbleList.Count];
		if (noDropList.Count > 0)
		{
			int[] checkedFlags = new int[fieldBubbleList.Count];
			List<Bubble> list = new List<Bubble>();
			for (int i = 0; i < fieldBubbleList.Count; i++)
			{
				bool isDrop = true;
				if (checkedFlags[i] > 0)
				{
					continue;
				}
				if (fieldBubbleList[i].isLocked)
				{
					checkedFlags[i] = 2;
					continue;
				}
				checkedFlags[i]++;
				checkDrop(fieldBubbleList[i], ref checkedFlags, ref isDrop, false);
				if (!isDrop)
				{
					for (int j = 0; j < fieldBubbleList.Count; j++)
					{
						if (checkedFlags[j] == 1)
						{
							checkedFlags[j]++;
						}
					}
					continue;
				}
				bool flag = true;
				bool flag2 = false;
				bool flag3 = false;
				list.Clear();
				for (int k = 0; k < fieldBubbleList.Count; k++)
				{
					if (checkedFlags[k] == 1)
					{
						checkedFlags[k]++;
						array[k] = true;
						if (fieldBubbleList[k].myTrans.localPosition.y < 1f)
						{
							flag = false;
						}
						if (fieldBubbleList[k].myTrans.localPosition.x > 539f)
						{
							flag2 = true;
						}
						if (fieldBubbleList[k].myTrans.localPosition.x < 1f)
						{
							flag3 = true;
						}
						list.Add(fieldBubbleList[k]);
					}
				}
				if (flag)
				{
					for (int l = 0; l < fieldBubbleList.Count; l++)
					{
						if (checkedFlags[l] == 1)
						{
							checkedFlags[l]++;
						}
					}
				}
				else
				{
					if (list.Count <= 0)
					{
						continue;
					}
					bool bDefault = flag2 || (!flag2 && !flag3);
					list.Sort(delegate(Bubble b1, Bubble b2)
					{
						Vector3 localPosition3 = b1.myTrans.localPosition;
						Vector3 localPosition4 = b2.myTrans.localPosition;
						if (bDefault)
						{
							if (localPosition3.x.Equals(localPosition4.x))
							{
								return (!(localPosition3.y < localPosition4.y)) ? 1 : (-1);
							}
							return (!(localPosition3.x > localPosition4.x)) ? 1 : (-1);
						}
						return localPosition3.x.Equals(localPosition4.x) ? ((!(localPosition3.y > localPosition4.y)) ? 1 : (-1)) : ((!(localPosition3.x < localPosition4.x)) ? 1 : (-1));
					});
					searchDict.Clear();
					searchNearBubble(list[0], (!bDefault) ? Bubble.eDir.DownRight : Bubble.eDir.UpLeft, list[0]);
					searchCount = 0;
					addSerchedBubbleType();
				}
			}
		}
		BubbleBase bubbleBase = null;
		Bubble.eDir dir = Bubble.eDir.UpLeft;
		Dictionary<BubbleBase, float> dictionary = new Dictionary<BubbleBase, float>();
		Dictionary<BubbleBase, float> dictionary2 = new Dictionary<BubbleBase, float>();
		for (int m = 0; m < fieldBubbleList.Count; m++)
		{
			if (array[m])
			{
				continue;
			}
			Bubble bubble = fieldBubbleList[m];
			if (bubble.isLocked)
			{
				continue;
			}
			Vector3 localPosition = bubble.myTrans.localPosition;
			if (!(localPosition.y > ceilingBaseY + 1f))
			{
				if (localPosition.x > 504f)
				{
					dictionary.Add(bubble, localPosition.y);
				}
				if (localPosition.y > ceilingBaseY - 1f)
				{
					dictionary2.Add(bubble, 0f - localPosition.x);
				}
			}
		}
		for (int n = 0; n < chainBubbleDic.Count; n++)
		{
			foreach (ChainBubble item in chainBubbleDic[n])
			{
				Vector3 localPosition2 = item.myTrans.localPosition;
				if (!(localPosition2.y > ceilingBaseY + 1f))
				{
					if (localPosition2.x > 504f)
					{
						dictionary.Add(item, localPosition2.y);
					}
					if (localPosition2.y > ceilingBaseY - 1f)
					{
						dictionary2.Add(item, 0f - localPosition2.x);
					}
				}
			}
		}
		List<KeyValuePair<BubbleBase, float>> list2 = new List<KeyValuePair<BubbleBase, float>>(dictionary);
		list2.Sort((KeyValuePair<BubbleBase, float> kvp1, KeyValuePair<BubbleBase, float> kvp2) => (int)(kvp1.Value - kvp2.Value));
		ceilingList = new List<KeyValuePair<BubbleBase, float>>(dictionary2);
		ceilingList.Sort((KeyValuePair<BubbleBase, float> kvp1, KeyValuePair<BubbleBase, float> kvp2) => (int)(kvp1.Value - kvp2.Value));
		if (dictionary.Count > 0)
		{
			bubbleBase = list2[0].Key;
		}
		else if (ceilingList.Count > 0)
		{
			bubbleBase = ceilingList[0].Key;
			dir = Bubble.eDir.Left;
		}
		searchDict.Clear();
		if (bubbleBase != null)
		{
			searchNearBubble(bubbleBase, dir, null);
			searchCount = 0;
		}
		addSerchedBubbleType();
	}

	private void addSerchedBubbleType()
	{
		float num = -363f;
		foreach (KeyValuePair<int, BubbleBase> item in searchDict)
		{
			Bubble.eType eType = item.Value.type;
			if (checkSearchExclusion(eType) || (item.Value is Bubble && ((Bubble)item.Value).isFrozen) || (item.Value is Bubble && ((Bubble)item.Value).inCloud))
			{
				continue;
			}
			if (eType >= Bubble.eType.MorganaRed && eType <= Bubble.eType.MorganaBlack)
			{
				eType -= 109;
			}
			if (item.Value.myTrans.localPosition.y > ceilingBaseY + 1f)
			{
				if (item.Value is Bubble && !((Bubble)item.Value).isLocked)
				{
					searchedPreBubbleTypeList.Add(convertColorBubble(eType));
				}
				continue;
			}
			searchedBubbleList.Add((Bubble)item.Value);
			int num2 = 1;
			if (item.Value.myTrans.localPosition.y < num)
			{
				num2 += (int)((num - item.Value.myTrans.localPosition.y) / 52f);
			}
			eType = convertColorBubble(eType);
			for (int i = 0; i < num2; i++)
			{
				searchedBubbleTypeList.Add(eType);
			}
		}
	}

	private void searchNearBubble(BubbleBase me, Bubble.eDir dir, BubbleBase startBubble)
	{
		searchCount++;
		if (searchCount > 100)
		{
			Debug.LogWarning("表面サーチで無限ループに入っている可能性が非常に高いです");
			return;
		}
		if (!searchDict.ContainsKey(me.GetInstanceID()))
		{
			if (me == startBubble)
			{
				upLeftBubble = null;
				downRightBubble = null;
			}
			searchDict.Add(me.GetInstanceID(), me);
		}
		else if (me == startBubble)
		{
			if (upLeftBubble != null && !searchDict.ContainsKey(upLeftBubble.GetInstanceID()))
			{
				searchNearBubble(upLeftBubble, Bubble.eDir.UpLeft, startBubble);
			}
			if (downRightBubble != null && !searchDict.ContainsKey(downRightBubble.GetInstanceID()))
			{
				searchNearBubble(downRightBubble, Bubble.eDir.DownRight, startBubble);
			}
			return;
		}
		Vector3 mePos = me.myTrans.localPosition;
		Dictionary<BubbleBase, Bubble.eDir> nearBubbleDict = new Dictionary<BubbleBase, Bubble.eDir>();
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (!fieldBubble.isLocked && fieldBubble.GetInstanceID() != me.GetInstanceID() && me.isNearBubble(fieldBubble))
			{
				float num3 = fieldBubble.myTrans.localPosition.y - mePos.y;
				Bubble.eDir value2;
				if (fieldBubble.myTrans.localPosition.x > mePos.x)
				{
					if (Mathf.Abs(num3) < 1f)
					{
						value2 = Bubble.eDir.Right;
					}
					else if (num3 > 0f)
					{
						value2 = Bubble.eDir.UpRight;
					}
					else
					{
						value2 = Bubble.eDir.DownRight;
						if (me == startBubble)
						{
							downRightBubble = fieldBubble;
						}
					}
				}
				else if (Mathf.Abs(num3) < 1f)
				{
					value2 = Bubble.eDir.Left;
				}
				else if (num3 > 0f)
				{
					value2 = Bubble.eDir.UpLeft;
					if (me == startBubble)
					{
						upLeftBubble = fieldBubble;
					}
				}
				else
				{
					value2 = Bubble.eDir.DownLeft;
				}
				nearBubbleDict.Add(fieldBubble, value2);
			}
		});
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble item in chainBubbleDic[i])
			{
				if (item.GetInstanceID() != me.GetInstanceID() && me.isNearBubble(item))
				{
					float num = item.myTrans.localPosition.y - mePos.y;
					Bubble.eDir value = ((item.myTrans.localPosition.x > mePos.x) ? ((Mathf.Abs(num) < 1f) ? Bubble.eDir.Right : ((!(num > 0f)) ? Bubble.eDir.DownRight : Bubble.eDir.UpRight)) : ((Mathf.Abs(num) < 1f) ? Bubble.eDir.Left : ((!(num > 0f)) ? Bubble.eDir.DownLeft : Bubble.eDir.UpLeft)));
					if (!nearBubbleDict.ContainsValue(value))
					{
						nearBubbleDict.Add(item, value);
					}
				}
			}
		}
		List<KeyValuePair<BubbleBase, Bubble.eDir>> list = new List<KeyValuePair<BubbleBase, Bubble.eDir>>(nearBubbleDict);
		int num2 = (int)(dir + 4) % 6;
		for (int j = num2; j < num2 + 6; j++)
		{
			Bubble.eDir eDir = (Bubble.eDir)(j % 6);
			if (startBubble == null)
			{
				if (mePos.x < 1f)
				{
					if (eDir == Bubble.eDir.DownLeft)
					{
						break;
					}
				}
				else if (mePos.x < 36f && eDir == Bubble.eDir.Left)
				{
					break;
				}
				if (mePos.y > ceilingBaseY - 1f && mePos.y < ceilingBaseY + 1f && eDir == Bubble.eDir.UpLeft)
				{
					for (int k = 0; k < ceilingList.Count; k++)
					{
						if (ceilingList[k].Key.myTrans.localPosition.x < mePos.x - 1f)
						{
							searchNearBubble(ceilingList[k].Key, Bubble.eDir.Left, startBubble);
							break;
						}
					}
					break;
				}
			}
			else if ((mePos.x < 1f && eDir == Bubble.eDir.DownLeft) || (mePos.x > 539f && eDir == Bubble.eDir.UpRight))
			{
				break;
			}
			for (int l = 0; l < list.Count; l++)
			{
				if (list[l].Value == eDir)
				{
					searchNearBubble(list[l].Key, eDir, startBubble);
					return;
				}
			}
		}
	}

	private bool checkSearchExclusion(Bubble.eType bubbleType)
	{
		switch (bubbleType)
		{
		case Bubble.eType.Blank:
			return true;
		case Bubble.eType.FriendRainbow:
		case Bubble.eType.FriendBox:
		case Bubble.eType.Hyper:
		case Bubble.eType.Bomb:
		case Bubble.eType.Shake:
		case Bubble.eType.Box:
		case Bubble.eType.Rainbow:
		case Bubble.eType.Rock:
		case Bubble.eType.ChainLock:
		case Bubble.eType.ChainHorizontal:
		case Bubble.eType.ChainRightDown:
		case Bubble.eType.ChainLeftDown:
		case Bubble.eType.Fulcrum:
		case Bubble.eType.RotateFulcrumR:
		case Bubble.eType.RotateFulcrumL:
		case (Bubble.eType)54:
		case (Bubble.eType)55:
		case Bubble.eType.Metal:
		case Bubble.eType.Ice:
		case (Bubble.eType)58:
		case (Bubble.eType)59:
		case (Bubble.eType)60:
		case (Bubble.eType)61:
		case (Bubble.eType)62:
		case (Bubble.eType)63:
		case (Bubble.eType)64:
		case Bubble.eType.Honeycomb:
		case Bubble.eType.Fire:
			return true;
		default:
			switch (bubbleType)
			{
			case Bubble.eType.MinilenRainbow:
				return true;
			case Bubble.eType.Grow:
			case Bubble.eType.Skull:
				return true;
			default:
				if (bubbleType >= Bubble.eType.Lightning && bubbleType <= Bubble.eType.Coin)
				{
					return true;
				}
				if (bubbleType >= Bubble.eType.Water && bubbleType <= Bubble.eType.IceKey)
				{
					return true;
				}
				if (bubbleType >= Bubble.eType.TunnelIn && bubbleType <= Bubble.eType.TunnelOutRightDown)
				{
					return true;
				}
				switch (bubbleType)
				{
				case Bubble.eType.LightningG:
					return true;
				case Bubble.eType.MinilenRed:
				case Bubble.eType.MinilenGreen:
				case Bubble.eType.MinilenBlue:
				case Bubble.eType.MinilenYellow:
				case Bubble.eType.MinilenOrange:
				case Bubble.eType.MinilenPurple:
				case Bubble.eType.MinilenWhite:
				case Bubble.eType.MinilenBlack:
					return true;
				default:
					return false;
				}
			}
		}
	}

	public Bubble.eType convertColorBubble(Bubble.eType bubbleType)
	{
		if (bubbleType >= Bubble.eType.PlusRed && bubbleType <= Bubble.eType.PlusBlack)
		{
			return bubbleType - 13;
		}
		if (bubbleType >= Bubble.eType.MinusRed && bubbleType <= Bubble.eType.MinusBlack)
		{
			return bubbleType - 21;
		}
		if (bubbleType >= Bubble.eType.FriendRed && bubbleType <= Bubble.eType.FriendBlack)
		{
			return bubbleType - 31;
		}
		if (bubbleType >= Bubble.eType.SnakeRed && bubbleType <= Bubble.eType.SnakeBlack)
		{
			return bubbleType - 67;
		}
		if (bubbleType >= Bubble.eType.KeyRed && bubbleType <= Bubble.eType.KeyBlack)
		{
			return bubbleType - 91;
		}
		if (bubbleType >= Bubble.eType.CounterRed && bubbleType <= Bubble.eType.CounterBlack)
		{
			return bubbleType - 100;
		}
		switch (bubbleType)
		{
		case Bubble.eType.FriendBox:
			return Bubble.eType.Box;
		case Bubble.eType.MinilenRed:
		case Bubble.eType.MinilenGreen:
		case Bubble.eType.MinilenBlue:
		case Bubble.eType.MinilenYellow:
		case Bubble.eType.MinilenOrange:
		case Bubble.eType.MinilenPurple:
		case Bubble.eType.MinilenWhite:
		case Bubble.eType.MinilenBlack:
			return bubbleType - 128;
		default:
			return bubbleType;
		}
	}

	public Bubble.eType convertColorBubbleFixed(Bubble.eType bubbleType)
	{
		if (bubbleType >= Bubble.eType.PlusRed && bubbleType <= Bubble.eType.PlusBlack)
		{
			return bubbleType - 13;
		}
		if (bubbleType >= Bubble.eType.MinusRed && bubbleType <= Bubble.eType.MinusBlack)
		{
			return bubbleType - 21;
		}
		if (bubbleType >= Bubble.eType.FriendRed && bubbleType <= Bubble.eType.FriendBlack)
		{
			return bubbleType - 31;
		}
		if (bubbleType >= Bubble.eType.SnakeRed && bubbleType <= Bubble.eType.SnakeBlack)
		{
			return bubbleType - 67;
		}
		if (bubbleType >= Bubble.eType.ChameleonRed && bubbleType <= Bubble.eType.ChameleonBlack)
		{
			return bubbleType - 79;
		}
		if (bubbleType >= Bubble.eType.KeyRed && bubbleType <= Bubble.eType.KeyBlack)
		{
			return bubbleType - 91;
		}
		if (bubbleType >= Bubble.eType.CounterRed && bubbleType <= Bubble.eType.CounterBlack)
		{
			return bubbleType - 100;
		}
		if (Bubble.eType.MorganaRed <= bubbleType && bubbleType <= Bubble.eType.MorganaBlack)
		{
			return bubbleType - 109;
		}
		switch (bubbleType)
		{
		case Bubble.eType.FriendBox:
			return Bubble.eType.Box;
		case Bubble.eType.MinilenRed:
		case Bubble.eType.MinilenGreen:
		case Bubble.eType.MinilenBlue:
		case Bubble.eType.MinilenYellow:
		case Bubble.eType.MinilenOrange:
		case Bubble.eType.MinilenPurple:
		case Bubble.eType.MinilenWhite:
		case Bubble.eType.MinilenBlack:
			return bubbleType - 128;
		default:
			return bubbleType;
		}
	}

	private void setNextBubbleCount(int count)
	{
		count = Mathf.Clamp(count, 2, 3);
		if (count == nextBubbleCount)
		{
			return;
		}
		nextBubbleCount = count;
		if (nextBubbleCount == 2)
		{
			stepNextBubbleClipName = "Next_bubble_00_anm";
			UnityEngine.Object.Destroy(nextBubbles[2]);
			return;
		}
		stepNextBubbleClipName = "Next_bubble_01_anm";
		if (nextBubbles[0] != null)
		{
			createNextBubble(2, true);
		}
	}

	private void createNextBubble(int index, bool littleBubble)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(bubbleObject) as GameObject;
		gameObject.SetActive(true);
		Transform transform = gameObject.transform;
		transform.parent = nextBubbleRoot;
		transform.localScale = Vector3.one;
		transform.position = nextBubblePoses[index].position;
		if (index == 0)
		{
			transform.localPosition += Vector3.back * 5f;
		}
		if (searchedBubbleTypeList.Count == 0)
		{
			List<Bubble.eType> colorList = new List<Bubble.eType>();
			List<Bubble.eType> rockList = new List<Bubble.eType>();
			if (searchedPreBubbleTypeList.Count != 0)
			{
				colorList = searchedPreBubbleTypeList;
			}
			else
			{
				fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
				{
					if (!fieldBubble.isFrozen)
					{
						Bubble.eType eType2 = fieldBubble.type;
						if (Bubble.eType.MorganaRed <= fieldBubble.type && fieldBubble.type <= Bubble.eType.MorganaBlack)
						{
							eType2 -= 109;
						}
						eType2 = convertColorBubble(eType2);
						if (eType2 <= Bubble.eType.Black)
						{
							if (!fieldBubble.isLocked)
							{
								colorList.Add(eType2);
							}
							else
							{
								rockList.Add(eType2);
							}
						}
					}
				});
			}
			if (colorList.Count == 0)
			{
				colorList = rockList;
			}
			if (colorList.Count == 0)
			{
				int num = (int)noSearchColor;
				gameObject.name = num.ToString("00");
				for (int i = 0; i < nextBubbleCount; i++)
				{
					if (nextBubbles[i] != null)
					{
						Bubble component = nextBubbles[i].GetComponent<Bubble>();
						if (component.state != Bubble.eState.Field && !isSpecialBubble((Bubble.eType)int.Parse(nextBubbles[i].name)))
						{
							nextBubbles[i].GetComponent<Bubble>().setType(noSearchColor);
						}
					}
				}
			}
			else
			{
				gameObject.name = nextBubbleCorrection(colorList, littleBubble);
			}
		}
		else
		{
			gameObject.name = nextBubbleCorrection(searchedBubbleTypeList, littleBubble);
		}
		Bubble.eType eType = (Bubble.eType)int.Parse(gameObject.name);
		if (prevCreateBubble.ContainsKey(eType))
		{
			Dictionary<Bubble.eType, int> dictionary;
			Dictionary<Bubble.eType, int> dictionary2 = (dictionary = prevCreateBubble);
			Bubble.eType key;
			Bubble.eType key2 = (key = eType);
			int num2 = dictionary[key];
			dictionary2[key2] = num2 + 1;
		}
		else
		{
			prevCreateBubble.Clear();
			prevCreateBubble.Add(eType, 1);
		}
		gameObject.GetComponent<Bubble>().init();
		nextBubbles[index] = gameObject;
	}

	private string nextBubbleCorrection(List<Bubble.eType> searchedList, bool littleBubble)
	{
		Dictionary<Bubble.eType, int> dictionary = new Dictionary<Bubble.eType, int>();
		for (int i = 0; i < searchedList.Count; i++)
		{
			Bubble.eType eType = searchedList[i];
			if (dictionary.ContainsKey(eType))
			{
				Dictionary<Bubble.eType, int> dictionary2;
				Dictionary<Bubble.eType, int> dictionary3 = (dictionary2 = dictionary);
				Bubble.eType key;
				Bubble.eType key2 = (key = eType);
				int num = dictionary2[key];
				dictionary3[key2] = num + 1;
			}
			else
			{
				dictionary.Add(eType, 0);
			}
		}
		if (dictionary.Count == 1)
		{
			return ((int)dictionary.First().Key).ToString("00");
		}
		if (!littleBubble)
		{
			int count = dictionary.Count;
			int num2 = 0;
			foreach (KeyValuePair<Bubble.eType, int> item in dictionary)
			{
				int num3 = Mathf.RoundToInt(100f / (float)count + (float)(item.Value * nextCorrection));
				if (prevCreateBubble.ContainsKey(item.Key))
				{
					num3 = Mathf.Clamp(num3 - prevCreateBubble[item.Key] * nextLimitCorrection, 0, int.MaxValue);
				}
				num2 += num3;
			}
			int num4 = random.Next(num2);
			Bubble.eType key3 = noSearchColor;
			num2 = 0;
			foreach (KeyValuePair<Bubble.eType, int> item2 in dictionary)
			{
				int num5 = Mathf.RoundToInt(100f / (float)count + (float)(item2.Value * nextCorrection));
				if (prevCreateBubble.ContainsKey(item2.Key))
				{
					num5 = Mathf.Clamp(num5 - prevCreateBubble[item2.Key] * nextLimitCorrection, 0, int.MaxValue);
				}
				num2 += num5;
				if (num4 < num2)
				{
					key3 = item2.Key;
					break;
				}
			}
			int num6 = (int)key3;
			return num6.ToString("00");
		}
		List<KeyValuePair<Bubble.eType, int>> list = new List<KeyValuePair<Bubble.eType, int>>(dictionary);
		list.Sort((KeyValuePair<Bubble.eType, int> kvp1, KeyValuePair<Bubble.eType, int> kvp2) => kvp1.Value - kvp2.Value);
		return ((int)list[0].Key).ToString("00");
	}

	private IEnumerator stepNextBubble()
	{
		if (tayunCoroutine != null)
		{
			float elapsedTime = 0f;
			float waitTime = 0.8f - stepNextBubbleAnim[stepNextBubbleClipName].clip.length;
			while (elapsedTime < waitTime)
			{
				elapsedTime += Time.deltaTime;
				yield return stagePause.sync();
			}
		}
		else
		{
			yield return stagePause.sync();
		}
		if (prevShotBubbleIndex != 0)
		{
			createNextBubble(prevShotBubbleIndex, false);
			nextBubbles[prevShotBubbleIndex].transform.position = nextBubblePoses[prevShotBubbleIndex].position;
			if (nextBubbles[0] != null)
			{
				nextBubbles[0].transform.position = nextBubblePoses[0].position;
			}
		}
		for (int j = 0; j < nextBubbleCount - 1; j++)
		{
			bool exist = false;
			Bubble.eType nextBubbleType = nextBubbles[j + 1].GetComponent<Bubble>().type;
			if (searchedBubbleTypeList.Count > 0)
			{
				for (int m = 0; m < fieldBubbleList.Count; m++)
				{
					if (!fieldBubbleList[m].isFrozen && !fieldBubbleList[m].isLocked && !fieldBubbleList[m].inCloud && nextBubbleType == convertColorBubble(fieldBubbleList[m].type) && searchedBubbleTypeList.Contains(nextBubbleType))
					{
						exist = true;
						break;
					}
				}
			}
			else if (searchedPreBubbleTypeList.Count > 0)
			{
				for (int l = 0; l < fieldBubbleList.Count; l++)
				{
					if (!fieldBubbleList[l].isFrozen && !fieldBubbleList[l].isLocked && !fieldBubbleList[l].inCloud && nextBubbleType == convertColorBubble(fieldBubbleList[l].type) && searchedPreBubbleTypeList.Contains(nextBubbleType))
					{
						exist = true;
						break;
					}
				}
			}
			else
			{
				for (int k = 0; k < fieldBubbleList.Count; k++)
				{
					if (!fieldBubbleList[k].isFrozen && !fieldBubbleList[k].isLocked && !fieldBubbleList[k].inCloud && nextBubbleType == convertColorBubble(fieldBubbleList[k].type))
					{
						exist = true;
						break;
					}
				}
			}
			if (exist)
			{
				nextBubbles[j] = nextBubbles[j + 1];
				nextBubbles[j].SetActive(true);
			}
			else
			{
				createNextBubble(j, true);
				nextBubbles[j].transform.position = nextBubbles[j + 1].transform.position;
				UnityEngine.Object.Destroy(nextBubbles[j + 1]);
			}
			Slave slave = nextBubbles[j].AddComponent<Slave>();
			slave.target = nextBubblePoses[j + 1];
		}
		if (isSpecialBubble(prevShotBubbleType))
		{
			Constant.Item.eType itemType = Constant.Item.eType.Invalid;
			switch (prevShotBubbleType)
			{
			case Bubble.eType.Hyper:
				itemType = Constant.Item.eType.HyperBubble;
				break;
			case Bubble.eType.Bomb:
				itemType = Constant.Item.eType.BombBubble;
				break;
			case Bubble.eType.Shake:
				itemType = Constant.Item.eType.ShakeBubble;
				break;
			case Bubble.eType.Metal:
				itemType = Constant.Item.eType.MetalBubble;
				break;
			case Bubble.eType.Ice:
				itemType = Constant.Item.eType.IceBubble;
				break;
			case Bubble.eType.Fire:
				itemType = Constant.Item.eType.FireBubble;
				break;
			case Bubble.eType.Water:
				itemType = Constant.Item.eType.WaterBubble;
				break;
			case Bubble.eType.Shine:
				itemType = Constant.Item.eType.ShineBubble;
				break;
			case Bubble.eType.LightningG_Item:
				itemType = Constant.Item.eType.LightningG;
				break;
			}
			if (bCreater && replaySkillBomShot)
			{
				if (0 < CreaterStockNum)
				{
					CreaterSkillExecute();
				}
			}
			else
			{
				StageBoostItem item = itemParent_.getItem(itemType);
				if (item != null && item.isBuy())
				{
					item.use();
					useItem(item.getItemType(), item.getNum());
				}
			}
		}
		StopCoroutine("chara01ThrowAnim");
		StartCoroutine("chara01ThrowAnim");
		stepNextBubbleAnim.Play(stepNextBubbleClipName);
		while (stepNextBubbleAnim.isPlaying)
		{
			yield return stagePause.sync();
		}
		stepNextBubbleAnim[stepNextBubbleClipName].clip.SampleAnimation(stepNextBubbleAnim.gameObject, 0f);
		for (int i = 0; i < nextBubbleCount - 1; i++)
		{
			UnityEngine.Object.Destroy(nextBubbles[i].GetComponent<Slave>());
		}
		if (itemCancelBubbleType == Bubble.eType.Invalid)
		{
			itemCancelBubbleType = nextBubbles[0].GetComponent<Bubble>().type;
		}
		createNextBubble(nextBubbleCount - 1, false);
		nextBubbleHideForShotCount();
		bubbleNavi.startNavi(searchedBubbleList, nextBubbles[0].GetComponent<Bubble>().type);
	}

	private IEnumerator stepNextBubbleBobblen()
	{
		if (tayunCoroutine != null)
		{
			float elapsedTime = 0f;
			float waitTime = 0.8f - stepNextBubbleAnim["Next_bubble_02_anm"].clip.length;
			while (elapsedTime < waitTime)
			{
				elapsedTime += Time.deltaTime;
				yield return stagePause.sync();
			}
		}
		else
		{
			yield return stagePause.sync();
		}
		for (int j = 1; j < nextBubbleCount - 1; j++)
		{
			if (prevShotBubbleIndex != 0)
			{
				nextBubbles[j] = nextBubbles[j + 1];
			}
			Slave slave = nextBubbles[j].AddComponent<Slave>();
			slave.target = nextBubblePoses[j + 1];
		}
		StopCoroutine("chara01ThrowAnimBobblen");
		if (prevShotBubbleIndex != 0 && nextBubbleCount > 2)
		{
			stepNextBubbleAnim.Play("Next_bubble_02_anm");
			while (stepNextBubbleAnim.isPlaying)
			{
				yield return stagePause.sync();
			}
			stepNextBubbleAnim["Next_bubble_02_anm"].clip.SampleAnimation(stepNextBubbleAnim.gameObject, 0f);
		}
		for (int i = 1; i < nextBubbleCount - 1; i++)
		{
			UnityEngine.Object.Destroy(nextBubbles[i].GetComponent<Slave>());
			nextBubbles[i].transform.position = nextBubblePoses[i].position;
		}
		if (nextBubbleCount > 2)
		{
			if (prevShotBubbleIndex == 0)
			{
				createNextBubble(0, false);
				nextBubbles[0].SetActive(false);
			}
			else
			{
				createNextBubble(2, false);
			}
		}
		else
		{
			createNextBubble(prevShotBubbleIndex, false);
			if (prevShotBubbleIndex == 0)
			{
				nextBubbles[0].SetActive(false);
				bool exist = false;
				Bubble.eType nextBubbleType = nextBubbles[1].GetComponent<Bubble>().type;
				for (int k = 0; k < fieldBubbleList.Count; k++)
				{
					if (!fieldBubbleList[k].isFrozen && ((!fieldBubbleList[k].isLocked && !fieldBubbleList[k].inCloud) || searchedBubbleTypeList.Count <= 0) && nextBubbleType == convertColorBubble(fieldBubbleList[k].type))
					{
						exist = true;
						break;
					}
				}
				if (!exist)
				{
					UnityEngine.Object.Destroy(nextBubbles[1]);
					createNextBubble(1, false);
				}
			}
		}
		checkNextBubbleExistant(1);
		checkNextBubbleExistant(2);
		if (isSpecialBubble(prevShotBubbleType))
		{
			Constant.Item.eType itemType = Constant.Item.eType.Invalid;
			switch (prevShotBubbleType)
			{
			case Bubble.eType.Hyper:
				itemType = Constant.Item.eType.HyperBubble;
				break;
			case Bubble.eType.Bomb:
				itemType = Constant.Item.eType.BombBubble;
				break;
			case Bubble.eType.Shake:
				itemType = Constant.Item.eType.ShakeBubble;
				break;
			case Bubble.eType.Metal:
				itemType = Constant.Item.eType.MetalBubble;
				break;
			case Bubble.eType.Ice:
				itemType = Constant.Item.eType.IceBubble;
				break;
			case Bubble.eType.Fire:
				itemType = Constant.Item.eType.FireBubble;
				break;
			case Bubble.eType.Water:
				itemType = Constant.Item.eType.WaterBubble;
				break;
			case Bubble.eType.Shine:
				itemType = Constant.Item.eType.ShineBubble;
				break;
			}
			if (bCreater && replaySkillBomShot)
			{
				if (0 < CreaterStockNum)
				{
					CreaterSkillExecute();
				}
			}
			else
			{
				StageBoostItem item = itemParent_.getItem(itemType);
				if (item != null && item.isBuy())
				{
					item.use();
					useItem(item.getItemType(), item.getNum());
				}
			}
		}
		nextBubbleHideForShotCountBobllen();
		bubbleNavi.startNavi(searchedBubbleList, nextBubbles[1].GetComponent<Bubble>().type);
	}

	private IEnumerator chara01ThrowAnim()
	{
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "08");
		while (charaAnims[1].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[1] + "08"))
		{
			yield return stagePause.sync();
		}
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "07");
	}

	private IEnumerator chara00ThrowAnim()
	{
		charaAnims[0].Play(CHARA_SPRITE_ANIMATION_HEADER[0] + "08_00_1");
		while (charaAnims[1].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[1] + "08"))
		{
			yield return stagePause.sync();
		}
		charaAnims[0].Play(waitAnimName);
	}

	private IEnumerator chara01ThrowAnimBobblen()
	{
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "11_00_1");
		while (charaAnims[1].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[1] + "11_00_1"))
		{
			yield return stagePause.sync();
		}
		charaAnims[1].Play(waitAnimName);
	}

	private void updateFieldBubbleList()
	{
		fieldObjectCount = 0;
		fieldFriendCount = 0;
		fieldItemCount = 0;
		fulcrumList.Clear();
		noDropList.Clear();
		growBubbleList.Clear();
		chameleonBubbleList.Clear();
		MorganaList_.Clear();
		normalBubbleCount = 0;
		lineFriendCandidateList.Clear();
		List<Bubble> tempFieldBubbleList = new List<Bubble>(fieldBubbleList);
		fieldBubbleList.Clear();
		rightBubbleList.Clear();
		leftBubbleList.Clear();
		honeycombBubbleList_.Clear();
		counterList_.Clear();
		tempFieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (fieldBubble.state == Bubble.eState.Field)
			{
				fieldBubbleList.Add(fieldBubble);
				if (fieldBubble.IsMorganaParent_)
				{
					MorganaList_.Add(fieldBubble);
				}
				Bubble.eType type = fieldBubble.type;
				if ((type >= Bubble.eType.FriendRainbow && type <= Bubble.eType.RotateFulcrumL) || type == Bubble.eType.Rock || type == Bubble.eType.Box || type == Bubble.eType.FriendBox || type == Bubble.eType.Grow || type == Bubble.eType.Skull || type == Bubble.eType.Honeycomb || type == Bubble.eType.Counter || type == Bubble.eType.BlackHole_A || type == Bubble.eType.BlackHole_B || type == Bubble.eType.FriendFulcrum || (type >= Bubble.eType.ChameleonRed && type <= Bubble.eType.Unknown) || (type >= Bubble.eType.MorganaRed && type <= Bubble.eType.MorganaBlack) || type == Bubble.eType.MinilenRainbow)
				{
					fieldObjectCount++;
				}
				if (type >= Bubble.eType.FriendRed && type <= Bubble.eType.FriendBox)
				{
					fieldFriendCount++;
				}
				if ((type >= Bubble.eType.Lightning && type <= Bubble.eType.Coin) || type == Bubble.eType.LightningG)
				{
					fieldItemCount++;
				}
				switch (type)
				{
				case Bubble.eType.Fulcrum:
				case Bubble.eType.RotateFulcrumR:
				case Bubble.eType.RotateFulcrumL:
					fulcrumList.Add(fieldBubble);
					noDropList.Add(fieldBubble);
					break;
				case Bubble.eType.TunnelIn:
				case Bubble.eType.TunnelNotIn:
				case Bubble.eType.TunnelOutLeftUP:
				case Bubble.eType.TunnelOutUP:
				case Bubble.eType.TunnelOutRightUP:
				case Bubble.eType.TunnelOutLeftDown:
				case Bubble.eType.TunnelOutDown:
				case Bubble.eType.TunnelOutRightDown:
					fulcrumList.Add(fieldBubble);
					noDropList.Add(fieldBubble);
					break;
				case Bubble.eType.Rock:
					noDropList.Add(fieldBubble);
					break;
				case Bubble.eType.Grow:
					growBubbleList.Add(fieldBubble);
					break;
				case Bubble.eType.Honeycomb:
					honeycombBubbleList_.Add(fieldBubble);
					break;
				case Bubble.eType.Counter:
				case Bubble.eType.CounterRed:
				case Bubble.eType.CounterGreen:
				case Bubble.eType.CounterBlue:
				case Bubble.eType.CounterYellow:
				case Bubble.eType.CounterOrange:
				case Bubble.eType.CounterPurple:
				case Bubble.eType.CounterWhite:
				case Bubble.eType.CounterBlack:
					counterList_.Add(fieldBubble);
					break;
				}
				if (type >= Bubble.eType.MorganaRed && type <= Bubble.eType.MorganaBlack)
				{
					noDropList.Add(fieldBubble);
				}
				if (type >= Bubble.eType.ChameleonRed && type <= Bubble.eType.Unknown)
				{
					chameleonBubbleList.Add(fieldBubble);
				}
				if (type <= Bubble.eType.Black && !fieldBubble.isLineFriend && !fieldBubble.isLocked && !fieldBubble.isFrozen && !fieldBubble.inCloud)
				{
					normalBubbleCount++;
					if (fieldBubble.myTrans.localPosition.y < -105f)
					{
						bool flag = true;
						foreach (Bubble item in tempFieldBubbleList)
						{
							if (!(item == fieldBubble) && item.state == Bubble.eState.Field && item.type != Bubble.eType.Blank && !item.isFrozen && fieldBubble.isNearBubble(item) && (item.type > Bubble.eType.Black || item.isLocked || item.isLineFriend))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							lineFriendCandidateList.Add(fieldBubble);
						}
					}
				}
				if (ivyList != null && ivyList.Count >= 0)
				{
					bool flag2 = true;
					foreach (Bubble rightBubble in rightBubbleList)
					{
						if (rightBubble.myTrans.localPosition.y - 26f < fieldBubble.myTrans.localPosition.y && rightBubble.myTrans.localPosition.y + 26f > fieldBubble.myTrans.localPosition.y)
						{
							if (rightBubble.myTrans.localPosition.x + 30f > fieldBubble.myTrans.localPosition.x)
							{
								flag2 = false;
							}
							else
							{
								rightBubbleList.Remove(rightBubble);
							}
							break;
						}
					}
					if (flag2)
					{
						rightBubbleList.Add(fieldBubble);
					}
					flag2 = true;
					foreach (Bubble leftBubble in leftBubbleList)
					{
						if (leftBubble.myTrans.localPosition.y - 26f < fieldBubble.myTrans.localPosition.y && leftBubble.myTrans.localPosition.y + 26f > fieldBubble.myTrans.localPosition.y)
						{
							if (leftBubble.myTrans.localPosition.x + 30f < fieldBubble.myTrans.localPosition.x)
							{
								flag2 = false;
							}
							else
							{
								leftBubbleList.Remove(leftBubble);
							}
							break;
						}
					}
					if (flag2)
					{
						leftBubbleList.Add(fieldBubble);
					}
				}
			}
			else if (fieldBubble != null && fieldBubble.myTrans != null)
			{
				fieldBubble.myTrans.parent = fieldBubble.myTrans.parent.parent;
			}
		});
	}

	private bool drop(bool isRotate = false)
	{
		bool result = false;
		int[] checkedFlags = new int[fieldBubbleList.Count];
		for (int i = 0; i < fieldBubbleList.Count; i++)
		{
			bool isDrop = true;
			if (checkedFlags[i] > 0 || fieldBubbleList[i].isLocked)
			{
				continue;
			}
			checkedFlags[i]++;
			checkDrop(fieldBubbleList[i], ref checkedFlags, ref isDrop, true);
			if (!isDrop)
			{
				for (int j = 0; j < fieldBubbleList.Count; j++)
				{
					if (checkedFlags[j] == 1)
					{
						checkedFlags[j]++;
					}
				}
				continue;
			}
			result = true;
			List<Bubble> list = new List<Bubble>();
			for (int k = 0; k < fieldBubbleList.Count; k++)
			{
				if (checkedFlags[k] != 1)
				{
					continue;
				}
				checkedFlags[k]++;
				Bubble.eType type = fieldBubbleList[k].type;
				switch (type)
				{
				case Bubble.eType.Rock:
				case Bubble.eType.Fulcrum:
				case Bubble.eType.MorganaRed:
				case Bubble.eType.MorganaGreen:
				case Bubble.eType.MorganaBlue:
				case Bubble.eType.MorganaYellow:
				case Bubble.eType.MorganaOrange:
				case Bubble.eType.MorganaPurple:
				case Bubble.eType.MorganaWhite:
				case Bubble.eType.MorganaBlack:
					continue;
				}
				if ((type >= Bubble.eType.TunnelIn && type <= Bubble.eType.TunnelOutRightDown) || type == Bubble.eType.RotateFulcrumL || type == Bubble.eType.RotateFulcrumR)
				{
					continue;
				}
				if (type >= Bubble.eType.SnakeRed && type <= Bubble.eType.SnakeBlack && !fieldBubbleList[k].isFrozen && state != eState.Gameover && state != eState.Clear)
				{
					snakeCount_ += 3;
					if (snakeCount_ > 99)
					{
						snakeCount_ = 99;
					}
					Sound.Instance.playSe(Sound.eSe.SE_514_snake_egg_break);
					snakeEffect(fieldBubbleList[k]);
					dropSnakeList.Add(fieldBubbleList[k]);
				}
				list.Add(fieldBubbleList[k]);
			}
			for (int l = 0; l < list.Count; l++)
			{
				if (!isRotate)
				{
					basicSkillDropCount(list[l]);
				}
				if (isKeyBubble((int)list[l].type))
				{
					list[l].startBreak();
				}
				else
				{
					list[l].startDrop(list.Count - l);
				}
			}
		}
		return result;
	}

	private void checkDrop(Bubble me, ref int[] checkedFlags, ref bool isDrop, bool checkFulcrum)
	{
		Bubble.eType type = me.type;
		if (type == Bubble.eType.Blank)
		{
			isDrop = false;
		}
		if (checkFulcrum && (type == Bubble.eType.Fulcrum || type == Bubble.eType.Rock || (type >= Bubble.eType.MorganaRed && type <= Bubble.eType.MorganaBlack) || type == Bubble.eType.RotateFulcrumL || type == Bubble.eType.RotateFulcrumR || (type >= Bubble.eType.TunnelIn && type <= Bubble.eType.TunnelOutRightDown)))
		{
			isDrop = false;
		}
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble item in chainBubbleDic[i])
			{
				if (me.isNearBubble(item))
				{
					isDrop = false;
					break;
				}
			}
		}
		List<Bubble> list = new List<Bubble>();
		for (int j = 0; j < fieldBubbleList.Count; j++)
		{
			if (checkedFlags[j] == 0 && !fieldBubbleList[j].isLocked && me.isNearBubble(fieldBubbleList[j]))
			{
				checkedFlags[j]++;
				list.Add(fieldBubbleList[j]);
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			checkDrop(list[k], ref checkedFlags, ref isDrop, checkFulcrum);
		}
	}

	private void basicSkillDropCount(Bubble b)
	{
		if (b.isBasicSkillColor && bBasicSkill)
		{
			basicSkillBreakNum++;
			b.isBasicSkillColor = false;
		}
	}

	private bool breakFulcrum()
	{
		if (fulcrumList.Count == 0)
		{
			return false;
		}
		bool result = false;
		Dictionary<int, List<Bubble>> dictionary = new Dictionary<int, List<Bubble>>();
		int num = 0;
		foreach (Bubble fulcrum2 in fulcrumList)
		{
			bool flag = true;
			for (int i = 0; i < num; i++)
			{
				foreach (Bubble item in dictionary[i])
				{
					if (item.isNearBubble(fulcrum2))
					{
						dictionary[i].Add(fulcrum2);
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				List<Bubble> list = new List<Bubble>();
				list.Add(fulcrum2);
				dictionary.Add(num, list);
				num++;
			}
		}
		for (int j = 0; j < num; j++)
		{
			int nearBubbleCount = 0;
			foreach (Bubble fulcrum in dictionary[j])
			{
				fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
				{
					if (!(fulcrum == fieldBubble) && fieldBubble.type != Bubble.eType.Blank && fieldBubble.type != Bubble.eType.Fulcrum && fieldBubble.type != Bubble.eType.Rock && fieldBubble.type != Bubble.eType.Box && fieldBubble.type != Bubble.eType.FriendBox && fieldBubble.type != Bubble.eType.Counter && fieldBubble.type != Bubble.eType.BlackHole_A && fieldBubble.type != Bubble.eType.BlackHole_B && fieldBubble.type != Bubble.eType.FriendFulcrum && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && (fieldBubble.type < Bubble.eType.TunnelIn || fieldBubble.type > Bubble.eType.TunnelOutRightDown) && fulcrum.isNearBubble(fieldBubble))
					{
						nearBubbleCount++;
					}
				});
			}
			if (nearBubbleCount != 0)
			{
				continue;
			}
			foreach (Bubble item2 in dictionary[j])
			{
				item2.startBreak();
			}
			result = true;
		}
		return result;
	}

	private IEnumerator scrollRoutine()
	{
		int line = 0;
		for (int i = 0; i < fieldBubbleList.Count; i++)
		{
			if (fieldBubbleList[i].myTrans.localPosition.y < -623f)
			{
				line = 1;
				break;
			}
		}
		if (line == 0)
		{
			float minY = 10000f;
			float maxY = -10000f;
			fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
			{
				float y = fieldBubble.myTrans.localPosition.y;
				if (y < minY)
				{
					minY = y;
				}
				if (y > maxY)
				{
					maxY = y;
				}
			});
			if (chainBubbleDic != null)
			{
				for (int k = 0; k < chainBubbleDic.Count; k++)
				{
					foreach (ChainBubble chainBubble2 in chainBubbleDic[k])
					{
						float posY = chainBubble2.myTrans.localPosition.y;
						if (posY < minY)
						{
							minY = posY;
						}
						if (posY > maxY)
						{
							maxY = posY;
						}
					}
				}
			}
			if (minY > -519f)
			{
				line = (int)((-519f - minY) / 52f);
				int canDownLine = (int)((maxY + 1f) / 52f);
				if (-line > canDownLine)
				{
					line = -canDownLine;
				}
			}
		}
		if (line == 0)
		{
			yield break;
		}
		eState prevState = state;
		state = eState.Scroll;
		float diffTime = Time.time - startTime;
		Vector3 offset = Vector3.up * (52 * line);
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			fieldBubble.myTrans.localPosition += offset;
		});
		for (int j = 0; j < chainBubbleDic.Count; j++)
		{
			chainBubbleDic[j].ForEach(delegate(ChainBubble chainBubble)
			{
				chainBubble.myTrans.localPosition += offset;
			});
		}
		foreach (ObstacleDefend od in obstacleList)
		{
			od.transform.localPosition += offset;
		}
		if (gameType == eGameType.Time)
		{
			createBlitzBubble();
		}
		Vector3 basePos = bubbleRootPos - offset;
		bubbleRoot.localPosition = basePos;
		scrollIvies(offset.y);
		yield return stagePause.sync();
		float moveTime = 0.2f * (float)Mathf.Abs(line) + 0.1f;
		float moveY2 = 52 * line;
		moveY2 = ((line <= 0) ? (moveY2 - 26f) : (moveY2 + 26f));
		float moveY3 = 26f;
		if (line > 0)
		{
			moveY3 = 0f - moveY3;
		}
		iTween.MoveTo(bubbleRoot.gameObject, iTween.Hash("y", basePos.y + moveY2, "easetype", iTween.EaseType.easeInCubic, "time", moveTime, "islocal", true));
		while (bubbleRoot.GetComponent<iTween>() != null)
		{
			yield return stagePause.sync();
		}
		tayunCoroutine = StartCoroutine(tayun(basePos.y + moveY2 + moveY3, 0.8f));
		StartCoroutine(cloudUpdateCheck());
		startTime = Time.time - diffTime;
		state = prevState;
	}

	private IEnumerator tayun(float y, float t)
	{
		iTween.MoveTo(bubbleRoot.gameObject, iTween.Hash("y", y, "easetype", iTween.EaseType.easeOutElastic, "time", t, "islocal", true));
		while (bubbleRoot.GetComponent<iTween>() != null)
		{
			yield return stagePause.sync();
		}
		bubbleRoot.localPosition = bubbleRootPos;
	}

	private IEnumerator exchangeRoutine()
	{
		state = eState.Exchange;
		int count = nextBubbleCount;
		string clipName = stepNextBubbleClipName;
		if (gameType == eGameType.ShotCount)
		{
			int rest = stageInfo.Move - shotCount;
			if (count == 3 && rest == 2)
			{
				count = 2;
				clipName = "Next_bubble_00_anm";
			}
		}
		for (int k = 0; k < count; k++)
		{
			Slave slave = nextBubbles[k].AddComponent<Slave>();
			slave.target = nextBubblePoses[k];
		}
		GameObject temp = nextBubbles[0];
		for (int j = 0; j < count - 1; j++)
		{
			nextBubbles[j] = nextBubbles[j + 1];
		}
		nextBubbles[count - 1] = temp;
		StopCoroutine("chara01ThrowAnim");
		StartCoroutine("chara01ThrowAnim");
		StopCoroutine("chara00ThrowAnim");
		StartCoroutine("chara00ThrowAnim");
		stepNextBubbleAnim.clip = stepNextBubbleAnim[clipName].clip;
		stepNextBubbleAnim.Play();
		while (stepNextBubbleAnim.isPlaying)
		{
			yield return stagePause.sync();
		}
		stepNextBubbleAnim[clipName].clip.SampleAnimation(stepNextBubbleAnim.gameObject, 0f);
		for (int i = 0; i < count; i++)
		{
			UnityEngine.Object.Destroy(nextBubbles[i].GetComponent<Slave>());
		}
		state = eState.Wait;
		bubbleNavi.startNavi(searchedBubbleList, nextBubbles[0].GetComponent<Bubble>().type);
	}

	private IEnumerator exchangeBobblenRoutine()
	{
		state = eState.Exchange;
		int count = nextBubbleCount;
		string clipName = "Next_bubble_02_anm";
		if (gameType == eGameType.ShotCount)
		{
			int rest = stageInfo.Move - shotCount;
			if (count == 3 && rest < 2)
			{
				yield break;
			}
		}
		for (int k = 1; k < count; k++)
		{
			Slave slave = nextBubbles[k].AddComponent<Slave>();
			slave.target = nextBubblePoses[k];
		}
		GameObject temp = nextBubbles[1];
		for (int j = 1; j < count - 1; j++)
		{
			nextBubbles[j] = nextBubbles[j + 1];
		}
		nextBubbles[count - 1] = temp;
		StopCoroutine("chara01ThrowAnimBobblen");
		StartCoroutine("chara01ThrowAnimBobblen");
		stepNextBubbleAnim.clip = stepNextBubbleAnim[clipName].clip;
		stepNextBubbleAnim.Play();
		while (stepNextBubbleAnim.isPlaying)
		{
			yield return stagePause.sync();
		}
		stepNextBubbleAnim[clipName].clip.SampleAnimation(stepNextBubbleAnim.gameObject, 0f);
		for (int i = 1; i < count; i++)
		{
			UnityEngine.Object.Destroy(nextBubbles[i].GetComponent<Slave>());
			nextBubbles[i].transform.position = nextBubblePoses[i].position;
		}
		state = eState.Wait;
		bubbleNavi.startNavi(searchedBubbleList, nextBubbles[1].GetComponent<Bubble>().type);
	}

	private void updateRestCountDisp()
	{
		updateRestCountDisp(false, false);
	}

	private void updateRestCountDisp(bool bFix)
	{
		updateRestCountDisp(bFix, false);
	}

	private void updateRestCountDisp(bool bFix, bool bReplay)
	{
		if (!bFix && (stagePause.pause || state == eState.Search || state == eState.Scroll || isTimeStageReplay))
		{
			return;
		}
		int num;
		if (gameType == eGameType.ShotCount)
		{
			num = stageInfo.Move - shotCount;
			if (state != eState.Clear && !bCrisis && num <= 5)
			{
				countLabel.color = REST_COUNT_DANGER_COLOR;
				bCrisis = true;
				if (bReplay)
				{
					bShowedLastPoint = true;
				}
			}
			if (bCrisis && num > 5)
			{
				bCrisis = false;
				bShowedLastPoint = false;
				countLabel.color = REST_COUNT_DEFAULT_COLOR;
			}
		}
		else
		{
			if (stagePause.pause)
			{
				startTime = Time.time - stagePause.diffTime;
			}
			if (bDrawing)
			{
				startTime = Time.time - drawDiffTime;
			}
			if (bPlayingBeeBarrierEffect)
			{
				startTime = Time.time - beeBarrierDiffTime;
			}
			if (bPlayingTimeStopEffect)
			{
				startTime = Time.time - timeStopDiffTime;
			}
			if (bMoveCreaterEffect)
			{
				startTime = Time.time - moveCreaterDiffTime;
			}
			if (isCountAnim)
			{
				startTime = Time.time - countAnimDiff;
			}
			num = (int)((float)stageInfo.Time - (Time.time - startTime) + 0.9999f);
			if (num < 0)
			{
				num = 0;
			}
			if (!bCrisis && (float)num <= 10f)
			{
				countLabel.color = REST_COUNT_DANGER_COLOR;
				bCrisis = true;
				if (bReplay)
				{
					bShowedLastPoint = true;
				}
			}
			if (bCrisis && (float)num > 10f)
			{
				bCrisis = false;
				bShowedLastPoint = false;
				countLabel.color = REST_COUNT_DEFAULT_COLOR;
			}
		}
		countLabel.text = num.ToString();
	}

	private void updateTotalScoreDisp()
	{
		if (!stagePause.pause)
		{
			scoreUp();
			scoregaugeUp();
		}
	}

	private void scoreUp()
	{
		if (dispScore == nextScore)
		{
			return;
		}
		if (nextScore - dispScore < 0)
		{
			tempScore = (dispScore = nextScore);
		}
		else
		{
			int num = (int)((float)(nextScore - dispScore) * Time.deltaTime * 2f);
			if (num < 1)
			{
				num = 1;
			}
			tempScore += num;
			if (tempScore > nextScore)
			{
				tempScore = (dispScore = nextScore);
			}
		}
		int length = tempScore.ToString().Length;
		int num2 = scoreRoot.childCount - 1;
		if (num2 < length)
		{
			for (int i = num2; i < length; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(scoreList[0].gameObject) as GameObject;
				gameObject.name = (i + 1).ToString("00");
				Utility.setParent(gameObject, scoreRoot, true);
				scoreList.Add(gameObject.transform);
			}
		}
		int num3 = 1;
		Transform transform = null;
		for (int j = 0; j < scoreList.Count; j++)
		{
			Transform transform2 = scoreList[j];
			if (j >= length)
			{
				transform2.gameObject.SetActive(false);
				continue;
			}
			transform2.gameObject.SetActive(true);
			UISprite component = transform2.GetComponent<UISprite>();
			component.spriteName = "game_score_number_0" + tempScore % (num3 * 10) / num3;
			component.MakePixelPerfect();
			num3 *= 10;
			if (j > 0)
			{
				Vector3 localPosition = transform.localPosition;
				localPosition.x -= (transform.localScale.x + transform2.localScale.x - 2f) * 0.5f;
				transform2.localPosition = localPosition;
			}
			transform = transform2;
		}
	}

	private Color convertColor(int r, int g, int b)
	{
		return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
	}

	private void scoregaugeUp()
	{
		float num = 0f;
		num = ((stageInfo.StarScores[2] == -1) ? ((float)nextScore / (float)stageInfo.Score) : ((float)nextScore / (float)stageInfo.StarScores[2]));
		num = Mathf.Clamp(num, 0f, 1f);
		if (num < scoregauge.sliderValue)
		{
			scoregauge.sliderValue = num;
			if (eventNo_ != 2 && eventNo_ != 11)
			{
				if (num < starRate1)
				{
					stars[0].SetActive(false);
					setClearStamp(true, false);
				}
				if (num < starRate2)
				{
					stars[1].SetActive(false);
				}
				if (num < 1f)
				{
					stars[2].SetActive(false);
				}
				if (num >= 1f)
				{
					scoregauge.GetComponentInChildren<UIFilledSprite>().color = convertColor(255, 0, 190);
				}
				else if (num >= starRate2)
				{
					scoregauge.GetComponentInChildren<UIFilledSprite>().color = convertColor(255, 110, 0);
				}
				else if (num >= starRate1)
				{
					scoregauge.GetComponentInChildren<UIFilledSprite>().color = convertColor(0, 130, 255);
				}
				else
				{
					scoregauge.GetComponentInChildren<UIFilledSprite>().color = convertColor(140, 145, 140);
				}
			}
			else
			{
				setClearStamp(true, false);
			}
		}
		else
		{
			if (!(scoregauge.sliderValue < num))
			{
				return;
			}
			scoregauge.sliderValue += Time.deltaTime;
			if (scoregauge.sliderValue > num)
			{
				scoregauge.sliderValue = num;
			}
			if (eventNo_ != 2 && eventNo_ != 11)
			{
				if (!stars[0].activeSelf && scoregauge.sliderValue >= starRate1)
				{
					stars[0].SetActive(true);
					scoregauge.GetComponentInChildren<UIFilledSprite>().color = convertColor(0, 130, 255);
					StopCoroutine("scoregaugeEffRoutine");
					StartCoroutine("scoregaugeEffRoutine", stars[0].transform.position);
					setClearStamp(true, true);
					if (gameType == eGameType.Time && !stageInfo.IsAllDelete && !stageInfo.IsFriendDelete && !stageInfo.IsFulcrumDelete && !stageInfo.IsMinilenDelete)
					{
						setClearStamp(false, true);
					}
				}
				if (!stars[1].activeSelf && scoregauge.sliderValue >= starRate2)
				{
					stars[1].SetActive(true);
					scoregauge.GetComponentInChildren<UIFilledSprite>().color = convertColor(255, 110, 0);
					StopCoroutine("scoregaugeEffRoutine");
					StartCoroutine("scoregaugeEffRoutine", stars[1].transform.position);
				}
				if (!stars[2].activeSelf && scoregauge.sliderValue >= 1f)
				{
					stars[2].SetActive(true);
					scoregauge.GetComponentInChildren<UIFilledSprite>().color = convertColor(255, 0, 190);
					StopCoroutine("scoregaugeEffRoutine");
					StartCoroutine("scoregaugeEffRoutine", stars[2].transform.position);
				}
			}
			else if (scoregauge.sliderValue >= 1f)
			{
				setClearStamp(true, true);
			}
		}
	}

	private IEnumerator scoregaugeEffRoutine(Vector3 pos)
	{
		scoregauge_eff.gameObject.SetActive(true);
		pos.z = scoregauge_eff.transform.position.z;
		scoregauge_eff.transform.position = pos;
		scoregauge_eff.Reset();
		while (scoregauge_eff.isPlaying)
		{
			yield return stagePause.sync();
		}
		scoregauge_eff.gameObject.SetActive(false);
	}

	private int getClearState(int score)
	{
		int num = 0;
		if (stageInfo.Score > 0 && score >= stageInfo.Score)
		{
			num |= 1;
		}
		if (stageInfo.IsAllDelete && fieldBubbleList.Count <= 9 + fieldObjectCount + fieldItemCount)
		{
			num |= 2;
		}
		if (stageInfo.IsFriendDelete && fieldFriendCount <= 0)
		{
			num |= 4;
		}
		if (stageInfo.IsFulcrumDelete && fulcrumList.Count <= 0)
		{
			num |= 8;
		}
		if (stageInfo.IsMinilenDelete && minilen_count_current >= minilen_count_all)
		{
			num |= 0x10;
		}
		return num;
	}

	private bool checkClear(bool bScoreCheck)
	{
		if (bScoreCheck)
		{
			if (stageInfo.Score > 0 && totalScore < stageInfo.Score)
			{
				return false;
			}
		}
		else if (!stageInfo.IsAllDelete && !stageInfo.IsFriendDelete && !stageInfo.IsFulcrumDelete && !stageInfo.IsMinilenDelete)
		{
			return false;
		}
		if (stageInfo.IsAllDelete)
		{
			if (fieldBubbleList.Count > 9 + fieldObjectCount + fieldItemCount)
			{
				setClearStamp(false, false);
				return false;
			}
			setClearStamp(false, true);
		}
		if (stageInfo.IsFriendDelete && fieldFriendCount > 0)
		{
			return false;
		}
		if (stageInfo.IsFulcrumDelete)
		{
			if (fulcrumList.Count > 0)
			{
				setClearStamp(false, false);
				return false;
			}
			setClearStamp(false, true);
		}
		if (stageInfo.IsMinilenDelete && minilen_count_current < minilen_count_all)
		{
			return false;
		}
		return true;
	}

	private IEnumerator clearRoutine()
	{
		state = eState.Clear;
		frontUi.Find("Top_ui/Gamestop_Button").gameObject.SetActive(false);
		yield return stagePause.sync();
		Input.enable = false;
		if (arrow.charaIndex != 0)
		{
			dropSnakeList.Clear();
			snakeCount_ = 0;
			UnityEngine.Object.Destroy(nextBubbles[0]);
			updateShootCharacter(true);
			updateSnakeImmediate();
		}
		honeycomb_num = -1;
		isHoneycombHitWait = false;
		if (hitHoneycombEff != null)
		{
			hitHoneycombEff.SetActive(false);
		}
		if (gameType == eGameType.ShotCount || stageInfo.IsFriendDelete || stageInfo.IsMinilenDelete)
		{
			int index = 0;
			fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
			{
				if (fieldBubble.type >= Bubble.eType.MorganaRed && fieldBubble.type <= Bubble.eType.MorganaBlack)
				{
					StartCoroutine(fieldBubble.SpecialBreak());
				}
				else if (fieldBubble.type != Bubble.eType.Blank)
				{
					fieldBubble.myTrans.parent = fieldBubble.myTrans.parent.parent;
					fieldBubble.startDrop(fieldBubbleList.Count - index);
					index++;
				}
			});
		}
		else
		{
			yield return new WaitForSeconds(0.1f);
			fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
			{
				if (fieldBubble.type >= Bubble.eType.MorganaRed && fieldBubble.type <= Bubble.eType.MorganaBlack && fieldBubble.myTrans.localPosition.y < 105f)
				{
					StartCoroutine(fieldBubble.SpecialBreak());
				}
				else if (fieldBubble.type != Bubble.eType.Blank && fieldBubble.myTrans.localPosition.y < 105f)
				{
					fieldBubble.myTrans.parent = fieldBubble.myTrans.parent.parent;
					if (fieldBubble.isColorBubble() || fieldBubble.type == Bubble.eType.FriendBox)
					{
						fieldBubble.startBreak();
					}
					else
					{
						fieldBubble.startFadeout();
					}
				}
			});
		}
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble chainBubble in chainBubbleDic[i])
			{
				chainBubble.startBreak();
			}
		}
		StartCoroutine(obstacleMoveRight());
		StartCoroutine(obstacleWait());
		StartCoroutine(removeAllIvies());
		StartCoroutine(cloudClearAnimation());
		StartCoroutine(charaClearAnimation());
		Coroutine coroutine = null;
		GameObject[] array = nextBubbles;
		foreach (GameObject nextBubble in array)
		{
			if (nextBubble != null)
			{
				nextBubble.SetActive(false);
			}
		}
		UnityEngine.Object[] fireworks = new UnityEngine.Object[3]
		{
			ResourceLoader.Instance.loadGameObject("Prefabs/", "fireworks_00"),
			ResourceLoader.Instance.loadGameObject("Prefabs/", "fireworks_01"),
			ResourceLoader.Instance.loadGameObject("Prefabs/", "fireworks_02")
		};
		int fireworksIndex = 0;
		int restCount = ((gameType != 0) ? ((int)((float)stageInfo.Time - (Time.time - startTime) + 0.999999f)) : (stageInfo.Move - shotCount));
		for (int j = 0; j < restCount; j++)
		{
			int bonusScore2 = 0;
			bonusScore2 = ((j < 5) ? CLEAR_SCORE_1_5 : ((j < 10) ? CLEAR_SCORE_6_10 : ((j >= 15) ? CLEAR_SCORE_16 : CLEAR_SCORE_11_15)));
			remainingBonus += bonusScore2 * scoreUpNum;
			totalScore += bonusScore2 * scoreUpNum;
			nextScore = totalScore;
			countLabel.text = (restCount - (j + 1)).ToString();
			coroutine = StartCoroutine(fireworksRoutine(fireworks[fireworksIndex], bonusScore2));
			fireworksIndex = (fireworksIndex + 1) % 3;
			yield return new WaitForSeconds(0.1f);
		}
		yield return coroutine;
		Sound.Instance.playBgm(Sound.eBgm.BGM_007_Clear, false);
		stageClear.SetActive(true);
		while (stageClear.GetComponent<Animation>().isPlaying)
		{
			yield return stagePause.sync();
		}
		if (stageNo > 20000 && stageNo < 30000)
		{
			yield return StartCoroutine(drankBeatAnimation());
		}
		Input.enable = true;
		yield return StartCoroutine(resultSetup(true));
	}

	private IEnumerator drankBeatAnimation()
	{
		GameObject drank = frontUi.Find("challenge_clear").gameObject;
		if (drank != null)
		{
			if (!drank.activeSelf)
			{
				drank.SetActive(true);
			}
			drank.GetComponent<Animation>().Play();
			while (drank.GetComponent<Animation>().isPlaying)
			{
				yield return stagePause.sync();
			}
			yield return null;
		}
	}

	private IEnumerator charaClearAnimation()
	{
		while (charaAnims[0].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[0] + "04"))
		{
			yield return stagePause.sync();
		}
		StopCoroutine("chara00BonusAnim");
		charaAnims[0].Play(CHARA_SPRITE_ANIMATION_HEADER[0] + "07");
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "09");
		yield return 0;
	}

	private IEnumerator fireworksRoutine(UnityEngine.Object fireworks_00, int bonusScore)
	{
		GameObject obj = UnityEngine.Object.Instantiate(bubbleObject) as GameObject;
		obj.SetActive(true);
		Transform trans = obj.transform;
		trans.parent = nextBubbleRoot;
		trans.localScale = Vector3.one;
		trans.position = nextBubblePoses[0].position;
		trans.localPosition += Vector3.forward * 3f;
		obj.name = random.Next(8).ToString("00");
		tk2dAnimatedSprite asp = obj.GetComponentInChildren<tk2dAnimatedSprite>();
		asp.Resume();
		asp.Play("bubble_" + obj.name);
		Rigidbody rb = obj.AddComponent<Rigidbody>();
		rb.constraints = (RigidbodyConstraints)120;
		float xForce = -25 + rand.Next(51);
		float yForce = 110 + rand.Next(30);
		rb.AddForce(new Vector3(xForce, yForce, 0f));
		yield return null;
		while (rb.velocity.y > -0.8f)
		{
			yield return null;
		}
		Sound.Instance.playSe(Sound.eSe.SE_225_fuusen);
		GameObject fireworks = UnityEngine.Object.Instantiate(fireworks_00) as GameObject;
		fireworks.transform.parent = frontUi;
		fireworks.transform.localScale = Vector3.one;
		fireworks.transform.position = obj.transform.position;
		fireworks.transform.localPosition += Vector3.back;
		GameObject scoreObj2 = null;
		scoreObj2 = ((bonusScore == CLEAR_SCORE_1_5) ? (UnityEngine.Object.Instantiate(frontUi.Find("fireworks_score/00").gameObject) as GameObject) : ((bonusScore == CLEAR_SCORE_6_10) ? (UnityEngine.Object.Instantiate(frontUi.Find("fireworks_score/01").gameObject) as GameObject) : ((bonusScore != CLEAR_SCORE_11_15) ? (UnityEngine.Object.Instantiate(frontUi.Find("fireworks_score/03").gameObject) as GameObject) : (UnityEngine.Object.Instantiate(frontUi.Find("fireworks_score/02").gameObject) as GameObject))));
		scoreObj2.SetActive(true);
		scoreObj2.transform.parent = frontUi;
		scoreObj2.transform.localScale = Vector3.one;
		scoreObj2.transform.position = fireworks.transform.position;
		scoreObj2.transform.localPosition += Vector3.back;
		UnityEngine.Object.Destroy(obj);
		while (fireworks.GetComponent<TweenAlpha>().enabled)
		{
			yield return null;
		}
		while (scoreObj2.GetComponent<TweenScale>().enabled)
		{
			yield return null;
		}
		UnityEngine.Object.Destroy(fireworks);
		UnityEngine.Object.Destroy(scoreObj2);
	}

	private IEnumerator gameoverRoutine()
	{
		state = eState.Gameover;
		if (!guide.isShootButton)
		{
			guide.setActive(false);
		}
		while (stagePause.pause)
		{
			yield return 0;
		}
		if (arrow.charaIndex != 0)
		{
			dropSnakeList.Clear();
			snakeCount_ = 0;
			replaySnakeCount = 0;
			UnityEngine.Object.Destroy(nextBubbles[0]);
			updateShootCharacter(true);
			updateSnakeImmediate();
			if (gameType == eGameType.Time)
			{
				createNextBubble(0, false);
				setNextTap(true);
				setNextBubblePosition();
			}
		}
		if (honeycomb_num >= 0)
		{
			arrow.revertPreFireVector();
		}
		honeycomb_num = -1;
		isHoneycombHitWait = false;
		charaAnims[0].Play(CHARA_SPRITE_ANIMATION_HEADER[0] + "05");
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "05");
		int index = 0;
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (fieldBubble.type != Bubble.eType.Blank)
			{
				fieldBubble.startGameover(fieldBubbleList.Count - index, gameoverMat);
				index++;
			}
		});
		while ((bool)_lost_minilen)
		{
			yield return 0;
		}
		if (ivyList != null)
		{
			foreach (Ivy iv in ivyList)
			{
				iv.setEnable(false);
			}
		}
		if (gameoverType != eGameover.FriendToGrow && gameoverType != eGameover.MinilenToGrow && gameoverType != eGameover.ScoreNotEnough)
		{
			int restNum = ((gameoverType != eGameover.TimeOver && gameoverType != 0) ? ((gameType != 0) ? ((int)((float)stageInfo.Time - replayDiffTime + 0.9999f)) : (stageInfo.Move - replayShotCount)) : (-1));
			if (gameoverType == eGameover.MinilenVanish)
			{
				yield return StartCoroutine(DispGameOverMessage());
			}
			DialogADContinue ADdialog = dialogManager.getDialog(DialogManager.eDialog.ADContinue) as DialogADContinue;
			int freeContinueOccurRatio = UnityEngine.Random.Range(0, 100);
			bool bADReady = false;
			if (Plugin.Instance.hasVideoAd("PB_Video_AD") || Vungle.isAdvertAvailable())
			{
				UnityEngine.Debug.Log("TNK hasVideoAd = " + Plugin.Instance.hasVideoAd("PB_Video_AD"));
				UnityEngine.Debug.Log("Vungle isAdvertAvailable = " + Vungle.isAdvertAvailable());
				bADReady = true;
			}
			Debug.Log("freeContinueOccurRatio : " + freeContinueOccurRatio + " // nContinueCount : " + nContinueCount);
			if (GlobalData.Instance.getGameData().monetization.freeContinueOccurRatio > freeContinueOccurRatio && nContinueCount == 0 && bADReady)
			{
				yield return StartCoroutine(ADdialog.show(stageInfo, gameType, gameoverType, getClearState(totalScore), eventNo_, restNum));
				Ivy.setIvySe(false);
				while (ADdialog.isOpen())
				{
					yield return 0;
				}
				if (ADdialog.result == DialogADContinue.eResult.Continue)
				{
					nContinueCount++;
					if (stageInfo.Time >= 0)
					{
						isTimeStageReplay = true;
					}
					yield return StartCoroutine(resurrection());
					if (stageInfo.Time >= 0)
					{
						isTimeStageReplay = false;
						state = eState.Next;
						isCountAnim = true;
						startTime = Time.time - replayDiffTime;
						countAnimDiff = Time.time - startTime;
						countdownContinue.SetActive(true);
						TweenScale[] tsList2 = countdownContinue.transform.GetComponentsInChildren<TweenScale>();
						TweenScale[] array = tsList2;
						foreach (TweenScale ts2 in array)
						{
							ts2.Reset();
							ts2.Play(true);
						}
						while (countdownContinue.GetComponent<Animation>().isPlaying)
						{
							yield return null;
						}
						countdownContinue.SetActive(false);
						isCountAnim = false;
						state = eState.Wait;
					}
					yield break;
				}
			}
			if (!ADdialog.GetADContinue() || nContinueCount >= 1)
			{
				DialogContinue dialog = dialogManager.getDialog(DialogManager.eDialog.Continue) as DialogContinue;
				yield return StartCoroutine(dialog.show(stageInfo, gameType, gameoverType, getClearState(totalScore), eventNo_, restNum));
				Ivy.setIvySe(false);
				while (dialog.isOpen())
				{
					yield return 0;
				}
				if (dialog.result == DialogContinue.eResult.Continue)
				{
					nContinueCount++;
					if (stageInfo.Time >= 0)
					{
						isTimeStageReplay = true;
					}
					yield return StartCoroutine(resurrection());
					if (stageInfo.Time >= 0)
					{
						isTimeStageReplay = false;
						state = eState.Next;
						isCountAnim = true;
						startTime = Time.time - replayDiffTime;
						countAnimDiff = Time.time - startTime;
						countdownContinue.SetActive(true);
						TweenScale[] tsList = countdownContinue.transform.GetComponentsInChildren<TweenScale>();
						TweenScale[] array2 = tsList;
						foreach (TweenScale ts in array2)
						{
							ts.Reset();
							ts.Play(true);
						}
						while (countdownContinue.GetComponent<Animation>().isPlaying)
						{
							yield return null;
						}
						countdownContinue.SetActive(false);
						isCountAnim = false;
						state = eState.Wait;
					}
					yield break;
				}
			}
		}
		else
		{
			yield return StartCoroutine(DispGameOverMessage());
		}
		guide.setActive(false);
		yield return StartCoroutine(resultSetup(false));
	}

	private IEnumerator DispGameOverMessage()
	{
		growGameOver_.SetActive(true);
		if (gameoverType == eGameover.ScoreNotEnough)
		{
			growGameOver_.transform.Find("Target_Label").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(2800);
		}
		else if (gameoverType == eGameover.MinilenToGrow)
		{
			growGameOver_.transform.Find("Target_Label").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(9168);
		}
		else if (gameoverType == eGameover.MinilenVanish)
		{
			growGameOver_.transform.Find("Target_Label").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(9107);
		}
		TweenGroup[] tweens = growGameOver_.GetComponentsInChildren<TweenGroup>(true);
		TweenGroup tweenIn = null;
		TweenGroup tweenOut = null;
		TweenGroup[] array = tweens;
		foreach (TweenGroup tween in array)
		{
			switch (tween.getGroupName())
			{
			case "In":
				tweenIn = tween;
				break;
			case "Out":
				tweenOut = tween;
				break;
			}
		}
		tweenIn.Play();
		while (tweenIn.isPlaying())
		{
			yield return 0;
		}
		yield return StartCoroutine(Utility.clickCoroutine());
		tweenOut.Play();
		while (tweenOut.isPlaying())
		{
			yield return 0;
		}
		growGameOver_.SetActive(false);
	}

	private IEnumerator resurrection()
	{
		stageEnd_ContinueCount++;
		if (gameoverType != eGameover.HitSkull)
		{
			if (gameoverType != 0 && gameoverType != eGameover.TimeOver && gameoverType != eGameover.CounterOver)
			{
				state = eState.Wait;
			}
			fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
			{
				if (fieldBubble.type != Bubble.eType.Blank)
				{
					fieldBubble.resurrection(normalMat);
				}
			});
			foreach (Bubble chameleon in chameleonBubbleList)
			{
				if (chameleon.type >= Bubble.eType.ChameleonRed && chameleon.type <= Bubble.eType.ChameleonBlack)
				{
					chameleon.GetComponentInChildren<tk2dAnimatedSprite>().PlayFromFrame(0);
					chameleon.GetComponentInChildren<tk2dAnimatedSprite>().Stop();
				}
			}
			if (bUsingTimeStop)
			{
				setFieldBubbleTimeStop(true);
				yield return StartCoroutine(updateTimeStop(false, false));
				checkTimeStopItemEnable();
			}
			else
			{
				setFieldBubbleTimeStop(false);
			}
			if (bUsingVacuum)
			{
				yield return StartCoroutine(updateVacuum(false, false));
				checkVacuumEnable();
			}
			if (isUsedItem(Constant.Item.eType.BeeBarrier))
			{
				foreach (Bubble b4 in honeycombBubbleList_)
				{
					if (!b4.isFrozen)
					{
						b4.setGrayScale(gameoverMat);
					}
				}
			}
		}
		foreach (Bubble b3 in counterList_)
		{
			if (b3.getCounterCount() <= 0 && !b3.isFrozen)
			{
				b3.setCounterEnable(false);
				if (b3.type == Bubble.eType.Counter)
				{
					b3.setGrayScale(gameoverMat);
				}
			}
		}
		if (isCounterGameOver)
		{
			foreach (ReplayData data in replayDataList)
			{
				if (data.counterCount != 1)
				{
					continue;
				}
				bool isInvoke = false;
				float chainPosX = data.pos.x - 270f;
				float chainPosY = data.pos.y + 455f;
				foreach (Cloud cloud in cloudList)
				{
					if (chainPosX > cloud.cloudMinX && chainPosX < cloud.cloudMaxX && chainPosY > cloud.cloudMinY && chainPosY < cloud.cloudMaxY)
					{
						isInvoke = true;
						break;
					}
				}
				if (!isInvoke)
				{
					data.counterCount = 0;
				}
			}
		}
		switch (gameoverType)
		{
		case eGameover.ShotCountOver:
		{
			shotCount -= stageInfo.Continue.Recovary;
			int dispCount = stageInfo.Move - shotCount;
			if (dispCount > 5)
			{
				bCrisis = false;
				bShowedLastPoint = false;
			}
			replayShotCount -= stageInfo.Continue.Recovary;
			foreach (Bubble b in fieldBubbleList)
			{
				b.inCloud = false;
			}
			yield return StartCoroutine(cloudUpdateCheck());
			break;
		}
		case eGameover.TimeOver:
		{
			startTime = Time.time - (float)(stageInfo.Time - stageInfo.Continue.Recovary);
			int dispCount2 = (int)((float)stageInfo.Time - (Time.time - startTime) + 0.999999f);
			if ((float)dispCount2 > 10f)
			{
				bCrisis = false;
				bShowedLastPoint = false;
			}
			replayDiffTime = Time.time - startTime;
			tempReplayDiffTime = Time.time - startTime;
			foreach (Bubble b2 in fieldBubbleList)
			{
				b2.inCloud = false;
			}
			yield return StartCoroutine(cloudUpdateCheck());
			break;
		}
		case eGameover.HitSkull:
			yield return StartCoroutine(replayRoutine(true));
			state = eState.Wait;
			break;
		case eGameover.CounterOver:
			yield return StartCoroutine(replayRoutine(true));
			state = eState.Wait;
			break;
		case eGameover.MinilenVanish:
			yield return StartCoroutine(replayRoutine(true));
			state = eState.Wait;
			gameoverType = eGameover.ShotCountOver;
			break;
		}
		if (!bCrisis)
		{
			countLabel.color = REST_COUNT_DEFAULT_COLOR;
		}
		waitAnimName = CHARA_SPRITE_ANIMATION_HEADER[0] + "08_02_0";
		charaAnims[0].Play(waitAnimName);
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "07");
		if (ivyList != null)
		{
			foreach (Ivy iv in ivyList)
			{
				iv.setEnable(true);
			}
		}
		if (snakeCount_ <= 0)
		{
			GameObject nextBubble00Obj = nextBubbles[0];
			GameObject nextBubble01Obj = nextBubbles[1];
			if (nextBubble00Obj != null && nextBubble01Obj != null)
			{
				Bubble nextBubble = nextBubble01Obj.GetComponent<Bubble>();
				if (isSpecialBubble(nextBubble.type))
				{
					Bubble nextBubble0 = nextBubbles[0].GetComponent<Bubble>();
					Bubble.eType tempType = nextBubble.type;
					nextBubble.setType(itemCancelBubbleType);
					itemCancelBubbleType = nextBubble0.type;
					nextBubble0.setType(tempType);
					setNextTap(false);
				}
			}
		}
		inOutObjectCountup();
		MoveOutObjectReplayCheck();
		MoveOutObjectDefend();
	}

	private void updateGlossTime()
	{
		glossTime = Time.time + 1f + (float)rand.Next(3000) * 0.01f;
		if (glossColorList.Count != 0)
		{
			glossType = glossColorList[random.Next(glossColorList.Count)];
		}
	}

	public override IEnumerator OnDestroyCB()
	{
		TutorialManager.Instance.unload();
		GameObject[] bee_objs = GameObject.FindGameObjectsWithTag("Bee");
		for (int i = 0; i < bee_objs.Length; i++)
		{
			if ((bool)bee_objs[i])
			{
				UnityEngine.Object.Destroy(bee_objs[i]);
			}
		}
		yield break;
	}

	private IEnumerator replayRoutine(bool isContinue)
	{
		bReplaying = true;
		bubbleNavi.stopNavi();
		Input.enable = false;
		FadeMng fadeMng = partManager.fade;
		Sound.Instance.playSe(Sound.eSe.SE_206_gugugu);
		fadeMng.setActive(FadeMng.eType.Twirl, true);
		yield return StartCoroutine(fadeMng.startFadeOut(FadeMng.eType.Twirl));
		invokeReplay(isContinue);
		foreach (Bubble b in fieldBubbleList)
		{
			b.inCloud = false;
			if ((b.type < Bubble.eType.TunnelOutLeftUP || b.type > Bubble.eType.TunnelOutRightDown) && b.transform.Find("AS_spr_bubble") != null)
			{
				b.transform.Find("AS_spr_bubble").transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
		if (replayCloudArea.Count > 0)
		{
			for (int i = 0; i < cloudList.Count; i++)
			{
				cloudList[i].setAreaState(replayCloudArea[i]);
			}
			yield return StartCoroutine(cloudUpdateCheck());
		}
		inOutObjectCountup();
		MoveOutObjectReplayCheck();
		MoveOutObjectDefend();
		yield return 0;
		if (guide.isShootButton)
		{
			guide.lineUpdate();
		}
		yield return StartCoroutine(fadeMng.startFadeIn(FadeMng.eType.Twirl));
		fadeMng.setActive(FadeMng.eType.Twirl, false);
		Input.enable = true;
		bReplaying = false;
	}

	private void setReplay()
	{
		replayDataList.Clear();
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			ReplayData replayData = new ReplayData
			{
				type = fieldBubble.type,
				pos = fieldBubble.myTrans.localPosition,
				lineFriendIndex = fieldBubble.lineFriendIndex,
				isFrozen = fieldBubble.isFrozen,
				isOnChain = fieldBubble.isOnChain,
				isLocked = fieldBubble.isLocked,
				index = fieldBubble.createIndex,
				counterCount = fieldBubble.getCounterCount(),
				unknownColor = fieldBubble.unknownColor,
				chamelleonIndex = fieldBubble.chamelleonIndex,
				obstacle = fieldBubble.onObstacle,
				outType = fieldBubble.outObjectType,
				isWarpOutUse = fieldBubble.UseOutObject
			};
			if (fieldBubble.onObstacle != null)
			{
				replayData.isObstacleActive = fieldBubble.onObstacle.gameObject.activeSelf;
			}
			replayData.IsMorganaParent_ = fieldBubble.IsMorganaParent_;
			replayData.MorganaHP_ = fieldBubble.MorganaHP_;
			replayData.CharNum = fieldBubble.CharaNum_;
			replayData.isContainSearchList = searchedBubbleList.Contains(fieldBubble);
			replayData.uniqueId = fieldBubble.uniqueId;
			replayDataList.Add(replayData);
		});
		replayChainTypeDic.Clear();
		replayChainPosDic.Clear();
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			List<Bubble.eType> typeList = new List<Bubble.eType>();
			List<Vector3> posList = new List<Vector3>();
			List<ChainBubble> list = chainBubbleDic[i];
			list.ForEach(delegate(ChainBubble chainBubble)
			{
				typeList.Add(chainBubble.type);
				posList.Add(chainBubble.myTrans.localPosition);
			});
			replayChainTypeDic.Add(i, typeList);
			replayChainPosDic.Add(i, posList);
		}
		replayNextTypeList.Clear();
		for (int j = 0; j < nextBubbleCount; j++)
		{
			replayNextTypeList.Add(nextBubbles[j].GetComponent<Bubble>().type);
		}
		replayScore = totalScore;
		replayCoin = totalCoin;
		replayShotCount = shotCount;
		replayDiffTime = tempReplayDiffTime;
		tempReplayDiffTime = Time.time - startTime;
		replayComboCount = comboCount;
		replayBonusCoin = bonusCoin;
		replayBonusJewel = bonusJewel;
		lineFriendCountBuffer = lineFriendCount;
		replaySearchedBubbleTypeList = new List<Bubble.eType>(searchedBubbleTypeList);
		if (obstacleList != null)
		{
			replayObstacleCount = obstacleCount;
		}
		if (bCreater)
		{
			replaySkillBomShot = isCreaterSkillUse;
			replayBombStockNum = CreaterStockNum;
			replayBombTurnCount = CreaterTurnCount;
		}
		if (bCueBubbleChange)
		{
			replayIsCueBubbleChange = isCueBubbleChangeUse;
			replayCueBubbleChangeType = cueBubble.type;
			replayCueBubbleActive = cueBubbleChangeObj.activeInHierarchy;
		}
		if (bComboMaster)
		{
			replayComboMasterCount = ComboMasterCount;
		}
		Debug.Log("replayDiffTime =" + replayDiffTime);
		replayUseItem = Constant.Item.eType.Invalid;
		foreach (Constant.Item.eType item in useItemList_)
		{
			if (!Constant.Item.IsAutoUse(item))
			{
				replayUseItem = item;
				break;
			}
		}
		if (ivyList != null)
		{
			foreach (Ivy ivy in ivyList)
			{
				ivy.setPreMove();
			}
		}
		replayCloudArea.Clear();
		if (cloudList != null && cloudList.Count > 0)
		{
			for (int k = 0; k < cloudList.Count; k++)
			{
				replayCloudArea.Add(cloudList[k].getAreaState());
			}
		}
		replaySnakeCount = snakeCount_;
		replayTimeStopCount = timeStopCount;
		replayVacuumCount = vacuumCount;
		replayKeyCount = getKeyCount;
	}

	private void invokeReplay(bool isContinue)
	{
		if (fieldBubbleList.Count > replayDataList.Count)
		{
			for (int num = fieldBubbleList.Count - 1; num >= replayDataList.Count; num--)
			{
				UnityEngine.Object.Destroy(fieldBubbleList[num].gameObject);
				fieldBubbleList.RemoveAt(num);
			}
		}
		else if (fieldBubbleList.Count < replayDataList.Count)
		{
			for (int i = fieldBubbleList.Count; i < replayDataList.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(bubbleObject) as GameObject;
				gameObject.SetActive(true);
				Transform transform = gameObject.transform;
				transform.parent = bubbleRoot;
				transform.localScale = Vector3.one;
				gameObject.name = "00";
				Bubble component = gameObject.GetComponent<Bubble>();
				component.init();
				fieldBubbleList.Add(component);
			}
		}
		foreach (GameObject item4 in ReplayDesList_)
		{
			if (item4 != null)
			{
				item4.SetActive(false);
			}
		}
		ReplayDesList_.Clear();
		MorganaList_.Clear();
		searchedBubbleList.Clear();
		for (int j = 0; j < replayDataList.Count; j++)
		{
			Bubble bubble = fieldBubbleList[j];
			bubble.transform.localPosition = replayDataList[j].pos;
			if (bubble.type == Bubble.eType.Blank)
			{
				continue;
			}
			bubble.isLocked = false;
			bubble.setType(replayDataList[j].type);
			bubble.setFieldState();
			bubble.setLineFriend(replayDataList[j].lineFriendIndex, false);
			bubble.setFrozen(replayDataList[j].isFrozen);
			bubble.isOnChain = replayDataList[j].isOnChain;
			bubble.isLocked = replayDataList[j].isLocked;
			bubble.createIndex = replayDataList[j].index;
			bubble.onObstacle = replayDataList[j].obstacle;
			if (bubble.onObstacle != null)
			{
				bubble.onObstacle.gameObject.SetActive(replayDataList[j].isObstacleActive);
				bubble.onObstacle.replayUFO();
				Vector3 localPosition = bubble.myTrans.localPosition;
				localPosition = new Vector3(localPosition.x, localPosition.y, -5f);
				bubble.onObstacle.transform.localPosition = localPosition;
				bubble.onObstacle.currentParentBubble = bubble;
			}
			if (replayDataList[j].isContainSearchList)
			{
				searchedBubbleList.Add(bubble);
			}
			bubble.IsMorganaParent_ = false;
			bubble.ParentMorgana_ = null;
			bubble.ChildMorgana_.Clear();
			bubble.gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
			bubble.IsMorganaParent_ = replayDataList[j].IsMorganaParent_;
			bubble.MorganaHP_ = replayDataList[j].MorganaHP_;
			bubble.CharaNum_ = replayDataList[j].CharNum;
			bubble.sprite.transform.localScale = new Vector3(418f, 418f, 1f);
			bubble.sprite.transform.localPosition = Vector3.zero;
			if (bubble.IsMorganaParent_)
			{
				MorganaList_.Add(bubble);
			}
			else if (bubble.CharaObj_ != null)
			{
				UnityEngine.Object.Destroy(bubble.CharaObj_);
				bubble.CharaObj_ = null;
			}
			if (replayDataList[j].type == Bubble.eType.Counter || (replayDataList[j].type >= Bubble.eType.CounterRed && replayDataList[j].type <= Bubble.eType.CounterBlack))
			{
				if (replayDataList[j].counterCount <= 0)
				{
					if (replayDataList[j].type >= Bubble.eType.CounterRed && replayDataList[j].type <= Bubble.eType.CounterBlack)
					{
						bubble.setColorCounterEnable(false);
					}
					else
					{
						bubble.setCounterEnable(false);
						bubble.setGrayScale(gameoverMat);
					}
				}
				else
				{
					bubble.isLocked = replayDataList[j].isLocked;
					bubble.checkCounterExistante();
					bubble.setCounterSpriteEnable(true);
					bubble.setCounterEnable(true);
					bubble.setCounterCount(replayDataList[j].counterCount);
				}
			}
			if (replayDataList[j].type >= Bubble.eType.TunnelOutLeftUP && replayDataList[j].type <= Bubble.eType.TunnelOutRightDown)
			{
				bubble.isLocked = replayDataList[j].isLocked;
				bubble.outObjectType = replayDataList[j].outType;
				switch (replayDataList[j].outType)
				{
				case 1:
					bubble.transform.Find("AS_spr_bubble").transform.rotation = Quaternion.Euler(0f, 0f, 30f);
					break;
				case 2:
					bubble.transform.Find("AS_spr_bubble").transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					break;
				case 3:
					bubble.transform.Find("AS_spr_bubble").transform.rotation = Quaternion.Euler(0f, 0f, 330f);
					break;
				case 4:
					bubble.transform.Find("AS_spr_bubble").transform.rotation = Quaternion.Euler(0f, 0f, 150f);
					break;
				case 5:
					bubble.transform.Find("AS_spr_bubble").transform.rotation = Quaternion.Euler(0f, 0f, 180f);
					break;
				case 6:
					bubble.transform.Find("AS_spr_bubble").transform.rotation = Quaternion.Euler(0f, 0f, 210f);
					break;
				}
				if (!replayDataList[j].isWarpOutUse)
				{
					bubble.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 123);
				}
			}
			bubble.unknownColor = replayDataList[j].unknownColor;
			bubble.chamelleonIndex = replayDataList[j].chamelleonIndex;
			bubble.uniqueId = replayDataList[j].uniqueId;
		}
		replayDataList.Clear();
		foreach (Bubble item5 in MorganaList_)
		{
			foreach (Bubble fieldBubble in fieldBubbleList)
			{
				if (item5.isNearBubble(fieldBubble))
				{
					fieldBubble.ParentMorgana_ = item5;
					item5.ChildMorgana_.Add(fieldBubble);
					fieldBubble.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
				}
			}
			item5.Setup();
		}
		for (int k = 0; k < chainBubbleDic.Count; k++)
		{
			List<Bubble.eType> list = replayChainTypeDic[k];
			List<Vector3> list2 = replayChainPosDic[k];
			List<ChainBubble> list3 = chainBubbleDic[k];
			if (list3.Count < list.Count)
			{
				for (int l = list3.Count; l < list.Count; l++)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(chainBubbleObject) as GameObject;
					ChainBubble component2 = gameObject2.GetComponent<ChainBubble>();
					Transform transform2 = gameObject2.transform;
					transform2.parent = bubbleRoot;
					transform2.localScale = Vector3.one;
					list3.Add(component2);
				}
			}
			for (int m = 0; m < list.Count; m++)
			{
				ChainBubble chainBubble = list3[m];
				chainBubble.setType(k, list[m]);
				chainBubble.transform.localPosition = list2[m];
			}
		}
		replayChainTypeDic.Clear();
		replayChainPosDic.Clear();
		bool flag = false;
		Bubble.eType eType = Bubble.eType.Red;
		foreach (Bubble fieldBubble2 in fieldBubbleList)
		{
			if (fieldBubble2.type >= Bubble.eType.Red && fieldBubble2.type <= Bubble.eType.Black)
			{
				eType = fieldBubble2.type;
			}
			else if (fieldBubble2.type >= Bubble.eType.PlusRed && fieldBubble2.type <= Bubble.eType.PlusBlack)
			{
				eType = fieldBubble2.type - 13;
			}
			else if (fieldBubble2.type >= Bubble.eType.MinusRed && fieldBubble2.type <= Bubble.eType.MinusBlack)
			{
				eType = fieldBubble2.type - 21;
			}
			else if (fieldBubble2.type >= Bubble.eType.FriendRed && fieldBubble2.type <= Bubble.eType.FriendBlack)
			{
				eType = fieldBubble2.type - 31;
			}
			else if (fieldBubble2.type >= Bubble.eType.SnakeRed && fieldBubble2.type <= Bubble.eType.SnakeBlack)
			{
				eType = fieldBubble2.type - 67;
			}
			else if (fieldBubble2.type >= Bubble.eType.CounterRed && fieldBubble2.type <= Bubble.eType.CounterBlack)
			{
				eType = fieldBubble2.type - 100;
			}
			if (fieldBubble2.type >= Bubble.eType.MinilenRed && fieldBubble2.type <= Bubble.eType.MinilenBlack)
			{
				eType = fieldBubble2.type - 128;
				if (eType == itemCancelBubbleType)
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			itemCancelBubbleType = eType;
		}
		else
		{
			flag = false;
		}
		updateChainLock();
		searchedBubbleTypeList = replaySearchedBubbleTypeList;
		if (bCueBubbleChange && isCueBubbleChangeUse && !isContinue)
		{
			CueBubbleChangeCancel();
		}
		if (nextBubbleCount > replayNextTypeList.Count)
		{
			if (nextBubbles[2] == null || nextBubbles[2].GetComponent<Bubble>().state != 0)
			{
				createNextBubble(2, false);
			}
			if (!isSpecialBubble(nextBubbles[1].GetComponent<Bubble>().type))
			{
				nextBubbles[2].GetComponent<Bubble>().setType(nextBubbles[1].GetComponent<Bubble>().type);
			}
		}
		for (int n = 0; n < replayNextTypeList.Count; n++)
		{
			if (nextBubbles[n] == null || nextBubbles[n].GetComponent<Bubble>().state != 0)
			{
				createNextBubble(n, false);
				Vector3 localPosition2 = nextBubbles[n].transform.localPosition;
				localPosition2.z = -7.9f;
				nextBubbles[n].transform.localPosition = localPosition2;
			}
			nextBubbles[n].GetComponent<Bubble>().setType(replayNextTypeList[n]);
		}
		setNextTap(!isSpecialBubble(replayNextTypeList[0]));
		itemCancelBubbleType = itemCancelBubbleTypeOld;
		int num2 = timeStopCount;
		int num3 = vacuumCount;
		if (!isContinue)
		{
			snakeCount_ = replaySnakeCount;
		}
		bonusJewel = replayBonusJewel;
		bonusCoin = replayBonusCoin;
		totalScore = replayScore;
		nextScore = totalScore;
		totalCoin = replayCoin;
		shotCount = replayShotCount;
		startTime = Time.time - replayDiffTime;
		comboCount = replayComboCount;
		comboLabel.text = comboCount.ToString();
		timeStopCount = replayTimeStopCount;
		realLinkShotCount--;
		lineFriendCount = lineFriendCountBuffer;
		vacuumCount = replayVacuumCount;
		tempReplayDiffTime = replayDiffTime;
		Debug.Log("startTime (" + startTime + ") = Time.time(" + Time.time + ") - replayDiffTime (" + replayDiffTime + ")");
		getKeyCount = replayKeyCount;
		if (obstacleList != null)
		{
			obstacleCount = replayObstacleCount;
		}
		if (comboCount >= 2)
		{
			comboIn.Sample(1f, true);
		}
		else
		{
			comboOut.Sample(1f, true);
		}
		updateRestCountDisp(false, true);
		updateFieldBubbleList();
		if (ivyList != null)
		{
			foreach (Ivy ivy in ivyList)
			{
				ivy.recoverPreMove();
			}
		}
		if ((bool)chacknNumLabel && stageInfo.IsFriendDelete)
		{
			chacknNumLabel.text = "x" + fieldFriendCount;
			setClearStamp(false, fieldFriendCount <= 0);
		}
		Constant.Item.eType eType2 = Constant.Item.eType.Invalid;
		foreach (Constant.Item.eType item6 in useItemList_)
		{
			if (!Constant.Item.IsAutoUse(item6))
			{
				eType2 = item6;
				break;
			}
		}
		if (eType2 != Constant.Item.eType.Invalid)
		{
			StageBoostItem item = itemParent_.getItem(eType2);
			item.back();
			removeUseItem(item.getItemType(), item.getNum());
		}
		StageBoostItem item2 = itemParent_.getItem(Constant.Item.eType.LightningG);
		if ((bool)item2)
		{
			item2.setNum(old_linghtningGNum);
			if (old_linghtningGNum < 0)
			{
				item2.gameObject.SetActive(false);
			}
			else if (old_linghtningGNum == 0 && replayUseItem != Constant.Item.eType.LightningG)
			{
				item2.gameObject.SetActive(false);
			}
			else
			{
				item2.gameObject.SetActive(true);
			}
		}
		if (replayUseItem != Constant.Item.eType.Invalid)
		{
			StageBoostItem item3 = itemParent_.getItem(replayUseItem);
			if (!item3.isBuy())
			{
				item3.gameObject.SetActive(true);
				item3.back();
			}
			useItem(item3.getItemType(), item3.getNum());
		}
		if (bCreater)
		{
			isCreaterSkillUse = false;
			isHoneyCreater = false;
			CreaterStockNum = replayBombStockNum;
			CreaterTurnCount = replayBombTurnCount;
			CreaterUiUpdate();
			SkillButtonSetting(CreaterStockNum);
			if (CreaterStockNum <= 0)
			{
				setSkillButtonActive(false);
			}
			if (replaySkillBomShot)
			{
				itemCancelBubbleType = itemCancelBubbleTypeOld;
				int charaIndex = arrow.charaIndex;
				if (snakeCount_ > 0)
				{
					nextBubbles[1].GetComponent<Bubble>().setType(itemCancelBubbleTypeOld);
				}
				else
				{
					nextBubbles[0].GetComponent<Bubble>().setType(itemCancelBubbleTypeOld);
				}
				CreaterSkillCancel();
				setSkillButtonActive(true);
				CreaterSkillExecute();
			}
		}
		if (bCueBubbleChange)
		{
			if (replayIsCueBubbleChange)
			{
				CueBubbleChangeRestNum++;
				SkillButtonSetting(CueBubbleChangeRestNum);
				isCueBubbleChangeUse = false;
				int num4 = 0;
				num4 = ((snakeCount_ > 0) ? 1 : 0);
				cueBubbleChangeObj.SetActive(true);
				skillNgIcon_.SetActive(false);
				Bubble.eType type = nextBubbles[num4].GetComponent<Bubble>().type;
				nextBubbles[num4].GetComponent<Bubble>().setType(replayCueBubbleChangeType);
				cueBubble.setType(type);
				setSkillButtonActive(true);
			}
			else
			{
				cueBubble.setType(replayCueBubbleChangeType);
				cueBubbleChangeObj.SetActive(replayCueBubbleActive);
				skillNgIcon_.SetActive(!replayCueBubbleActive);
				setSkillButtonActive(replayCueBubbleActive);
			}
		}
		if (bComboMaster)
		{
			ComboMasterCount = replayComboMasterCount;
		}
		if (itemReplay_ != null)
		{
			itemReplay_.setStateFixed(true);
			itemReplay_.disable();
		}
		int num5 = 0;
		if (snakeCount_ <= 0)
		{
			dropSnakeList.Clear();
			updateShootCharacter(true);
			updateSnakeImmediate();
			num5 = 0;
			setNextBubblePosition();
			checkNextBubbleExistant(0);
			if (!nextBubbles[0].activeSelf)
			{
				nextBubbles[0].SetActive(true);
			}
			Bubble component3 = nextBubbles[1].GetComponent<Bubble>();
			if (isSpecialBubble(component3.type))
			{
				UnityEngine.Object.DestroyImmediate(component3.gameObject);
				createNextBubble(1, false);
			}
		}
		else
		{
			dropSnakeList.Clear();
			updateShootCharacter(true);
			updateSnakeImmediate();
			nextBubbles[0].SetActive(false);
			num5 = 1;
			setNextBubblePosition();
		}
		if (arrow.charaIndex == 0)
		{
			nextBubbleHideForShotCount();
		}
		else
		{
			nextBubbleHideForShotCountBobllen();
		}
		if (guide.isShootButton)
		{
			guide.lineUpdate();
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Chackn");
		GameObject[] array2 = array;
		foreach (GameObject obj in array2)
		{
			UnityEngine.Object.Destroy(obj);
		}
		minilen_count_current -= minilen_count_pop;
		StageMinilen.SetNum(this);
		updateMinilenCount();
		if (_droped_minilen_drops_indexes.Count > minilenCountCurrent)
		{
			_droped_minilen_drops_indexes.RemoveRange(minilen_count_current, _droped_minilen_drops_indexes.Count - minilen_count_current);
		}
		if ((bool)chacknNumLabel && stageInfo.IsMinilenDelete)
		{
			int num7 = Math.Max(0, minilen_count_all - minilen_count_current);
			chacknNumLabel.text = "x" + num7;
			setClearStamp(false, num7 <= 0);
		}
		if (!isCounterGameOver)
		{
			bubbleNavi.startNavi(searchedBubbleList, nextBubbles[num5].GetComponent<Bubble>().type);
		}
		GameObject[] array3 = GameObject.FindGameObjectsWithTag("Bee");
		GameObject[] array4 = array3;
		foreach (GameObject gameObject3 in array4)
		{
			gameObject3.GetComponent<Animation>()["Bee_anm"].time = 0f;
		}
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (fieldBubble.type != Bubble.eType.Blank)
			{
				fieldBubble.resurrection(normalMat);
			}
		});
		if (isUsedItem(Constant.Item.eType.BeeBarrier))
		{
			foreach (Bubble item7 in honeycombBubbleList_)
			{
				if (!item7.isFrozen)
				{
					item7.setGrayScale(gameoverMat);
				}
			}
		}
		foreach (Bubble item8 in counterList_)
		{
			if (item8.getCounterCount() <= 0 && !item8.isFrozen)
			{
				item8.setCounterEnable(false);
				if (item8.type == Bubble.eType.Counter)
				{
					item8.setGrayScale(gameoverMat);
				}
			}
		}
		chameleonFieldBubbleColorCheck();
		foreach (Bubble chameleonBubble in chameleonBubbleList)
		{
			if (chameleonBubble.type >= Bubble.eType.ChameleonRed && chameleonBubble.type <= Bubble.eType.ChameleonBlack && !chameleonBubble.isFrozen)
			{
				chameleonBubble.GetComponentInChildren<tk2dAnimatedSprite>().PlayFromFrame(0);
				chameleonBubble.GetComponentInChildren<tk2dAnimatedSprite>().Stop();
			}
		}
		bool flag2 = bUsingTimeStop;
		bUsingTimeStop = timeStopCount > 0;
		if (bUsingTimeStop)
		{
			if (num2 == 10)
			{
				bUsingTimeStop = false;
				timeStopCount = 0;
				if (itemTimeStop_ != null)
				{
					itemTimeStop_.reset();
					itemTimeStop_.setup(Constant.Item.eType.TimeStop, -1, -1, true);
					itemTimeStop_.setStateFixed(false);
					itemTimeStop_.enable();
					timestop_counter.SetActive(false);
					useItemList_.Remove(Constant.Item.eType.TimeStop);
					useItemParent_.setup();
					int num9 = 0;
					foreach (Constant.Item.eType item9 in useItemList_)
					{
						if (Constant.Item.IsAutoUse(item9))
						{
							num9++;
							useItemParent_.setActive(item9, -1);
						}
					}
					useitem_bg.SetActive(num9 > 0);
				}
			}
			else if (useItemParent_.getItem(Constant.Item.eType.TimeStop) == null)
			{
				useItemParent_.setActive(Constant.Item.eType.TimeStop, -1);
			}
		}
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		if (flag2 != bUsingTimeStop)
		{
			if (bUsingTimeStop)
			{
				Sound.Instance.stopBgm();
			}
			else
			{
				playBGM(@object);
			}
		}
		if (itemTimeStop_ != null)
		{
			if (bUsingTimeStop)
			{
				itemTimeStop_.setStateFixed(true);
				itemTimeStop_.disable();
			}
			else
			{
				itemTimeStop_.setStateFixed(false);
				itemTimeStop_.enable();
			}
		}
		if (bUsingTimeStop)
		{
			setFieldBubbleTimeStop(true);
			StartCoroutine(updateTimeStop(false, false));
			updateTimeStopCount();
			checkTimeStopItemEnable();
		}
		else
		{
			setFieldBubbleTimeStop(false);
		}
		bUsingVacuum = vacuumCount > 0;
		if (bUsingVacuum)
		{
			if (num3 == 10)
			{
				bUsingVacuum = false;
				vacuumCount = 0;
				if (itemVacuum_ != null)
				{
					itemVacuum_.reset();
					itemVacuum_.setup(Constant.Item.eType.Vacuum, -1, -1, true);
					itemVacuum_.setStateFixed(false);
					itemVacuum_.enable();
					vacuum_counter.SetActive(false);
					foreach (Cloud cloud in cloudList)
					{
						cloud.resurrection();
					}
					StartCoroutine(cloudUpdateCheck());
					useItemList_.Remove(Constant.Item.eType.Vacuum);
					useItemParent_.setup();
					int num10 = 0;
					foreach (Constant.Item.eType item10 in useItemList_)
					{
						if (Constant.Item.IsAutoUse(item10))
						{
							num10++;
							useItemParent_.setActive(item10, -1);
						}
					}
					useitem_bg.SetActive(num10 > 0);
					replayCloudArea.Clear();
					if (cloudList != null && cloudList.Count > 0)
					{
						for (int num11 = 0; num11 < cloudList.Count; num11++)
						{
							replayCloudArea.Add(cloudList[num11].getVacumeBufferArea());
						}
					}
				}
			}
			else if (useItemParent_.getItem(Constant.Item.eType.Vacuum) == null)
			{
				useItemParent_.setActive(Constant.Item.eType.Vacuum, -1);
			}
		}
		if (itemVacuum_ != null)
		{
			if (bUsingVacuum)
			{
				itemVacuum_.setStateFixed(true);
				itemVacuum_.disable();
			}
			else
			{
				itemVacuum_.setStateFixed(false);
				itemVacuum_.enable();
			}
		}
		if (bUsingVacuum)
		{
			StartCoroutine(updateVacuum(false, false));
			updateVacuumCount();
			checkVacuumEnable();
			StartCoroutine(cloudUpdateCheck());
		}
		foreach (Bubble item11 in MorganaList_)
		{
			item11.Setup();
		}
	}

	private bool createBlitzBubble()
	{
		if (stageInfo.LoopLine < 0)
		{
			return false;
		}
		int loopLineNum = stageData.lineNum - stageInfo.LoopLine;
		if (loopLineNum % 2 != 0)
		{
			loopLineNum--;
		}
		if (ceilingBubbleList[0].localPosition.y - (float)(loopLineNum * 52) - 26f > 0f)
		{
			return false;
		}
		ceilingBubbleList.ForEach(delegate(Transform ceiling)
		{
			ceiling.localPosition += Vector3.up * (loopLineNum * 52);
		});
		float y = ceilingBubbleList[0].localPosition.y;
		int num = 0;
		for (int i = 1; i <= loopLineNum; i++)
		{
			int num2 = 0;
			if (i % 2 == 0)
			{
				num2 = 30;
			}
			for (int j = 0; j < 10; j++)
			{
				if (i % 2 != 0 || j != 9)
				{
					int num3 = 10 * (i - 1) + j;
					string text = stageData.bubbleTypes[num3].ToString("00");
					if (!(text == "99"))
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(bubbleObject) as GameObject;
						gameObject.SetActive(true);
						gameObject.name = text;
						Bubble component = gameObject.GetComponent<Bubble>();
						component.init();
						Transform transform = gameObject.transform;
						transform.parent = bubbleRoot;
						transform.localScale = Vector3.one;
						transform.localPosition = new Vector3(j * 60 + num2, (float)(i * -52) + y, 0f);
						component.setFieldState();
						fieldBubbleList.Insert(9 + num, component);
						num++;
					}
				}
			}
		}
		return true;
	}

	private void createChainBubble(int lineNum)
	{
		if (stageData.chainLayerNum == 0)
		{
			return;
		}
		List<int> chainIndexList = new List<int>();
		for (int i = 0; i < stageData.chainLayerNum; i++)
		{
			List<ChainBubble> list = new List<ChainBubble>();
			float y = ceilingBubbleList[0].localPosition.y;
			for (int j = 1; j <= lineNum; j++)
			{
				int num = 0;
				if (j % 2 == 0)
				{
					num = 30;
				}
				for (int k = 0; k < 10; k++)
				{
					int num2 = 10 * (j - 1) + k;
					Bubble.eType eType = (Bubble.eType)stageData.chainTypes[10 * lineNum * i + num2];
					if (eType != Bubble.eType.Blank)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(chainBubbleObject) as GameObject;
						ChainBubble component = gameObject.GetComponent<ChainBubble>();
						component.setType(i, eType);
						Transform transform = gameObject.transform;
						transform.parent = bubbleRoot;
						transform.localScale = Vector3.one;
						float num3 = 0f;
						if (eType == Bubble.eType.ChainLock)
						{
							num3 = -0.1f;
						}
						num3 -= 3.5f;
						transform.localPosition = new Vector3(k * 60 + num, (float)(j * -52) + y, -0.1f - 0.01f * (float)i + num3);
						list.Add(component);
						if (!chainIndexList.Contains(num2))
						{
							chainIndexList.Add(num2);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				chainBubbleDic.Add(i, list);
			}
		}
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (fieldBubble.createIndex == -1)
			{
				return;
			}
			foreach (int item in chainIndexList)
			{
				if (fieldBubble.createIndex == item)
				{
					fieldBubble.isOnChain = true;
					break;
				}
			}
		});
		updateChainLock();
		updateOnChain();
	}

	public void setupIvies()
	{
		if (stageData.ivyTypes == null || stageData.ivyTypes.Count() < 1)
		{
			return;
		}
		bubble_17_eff_l = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "bubble_17_eff_l")) as GameObject;
		bubble_17_eff_r = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "bubble_17_eff_r")) as GameObject;
		Vector3 localScale = bubble_17_eff_l.transform.localScale;
		Vector3 localScale2 = bubble_17_eff_r.transform.localScale;
		bubble_17_eff_l.transform.parent = bubbleRoot;
		bubble_17_eff_r.transform.parent = bubbleRoot;
		bubble_17_eff_l.transform.localScale = localScale;
		bubble_17_eff_r.transform.localScale = localScale2;
		bubble_17_eff_l.SetActive(false);
		bubble_17_eff_r.SetActive(false);
		for (int i = 0; i < stageData.ivyTypes.Count(); i++)
		{
			int row = stageData.ivyRow[i];
			int num = stageData.ivyColumn[i];
			Ivy ivy = null;
			foreach (Ivy ivy2 in ivyList)
			{
				if (ivy2.column_ == num)
				{
					ivy = ivy2;
					break;
				}
			}
			if (ivy == null)
			{
				ivy = createIvy((Ivy.eType)stageData.ivyTypes[i], row, num);
			}
			ivy.setup(stagePause, dialogManager);
			ivy.createBudObj(row, num, 60f);
			ivy.setPreMove();
		}
		foreach (Ivy ivy3 in ivyList)
		{
			int num2 = ivy3.setBudPriority();
			if (num2 > ivyDepthOffset)
			{
				ivyDepthOffset = num2;
			}
		}
	}

	public void scrollIvies(float offset_y)
	{
		if (ivyList == null)
		{
			return;
		}
		foreach (Ivy ivy in ivyList)
		{
			ivy.gameObject.transform.localPosition = new Vector3(ivy.gameObject.transform.localPosition.x, ivy.gameObject.transform.localPosition.y + offset_y, ivy.gameObject.transform.localPosition.z);
		}
	}

	public void moveIvies()
	{
		if (ivyList == null)
		{
			return;
		}
		foreach (Ivy ivy in ivyList)
		{
			if (ivy.isRemoved_)
			{
				continue;
			}
			float x = ivy.headBubblePos_.x;
			GameObject gameObject;
			if (ivy.ivyType_ == Ivy.eType.Right)
			{
				gameObject = getLeftBubble(ivy);
				if (gameObject == null)
				{
					ivy.isRemoved_ = true;
					ivy.move(640f);
					continue;
				}
				x = ivy.toPosX + ivy.getMoveDirection(gameObject.transform.localPosition.x);
			}
			else
			{
				gameObject = getRightBubble(ivy);
				if (gameObject == null)
				{
					ivy.isRemoved_ = true;
					ivy.move(-640f);
					continue;
				}
				x = ivy.toPosX + ivy.getMoveDirection(gameObject.transform.localPosition.x);
			}
			if (gameObject.transform.localPosition.x + 30f < ivy.headBubblePos_.x || gameObject.transform.localPosition.x - 30f > ivy.headBubblePos_.x)
			{
				Bubble component = gameObject.GetComponent<Bubble>();
				ivy.move(x);
				ivy.headBubblePos_ = new Vector3(component.myTrans.localPosition.x, component.myTrans.localPosition.y, component.myTrans.localPosition.z);
			}
		}
	}

	private IEnumerator removeAllIvies()
	{
		if (ivyList == null)
		{
			yield break;
		}
		foreach (Ivy iv in ivyList)
		{
			if (!iv.isRemoved_)
			{
				if (iv.ivyType_ == Ivy.eType.Right)
				{
					iv.isRemoved_ = true;
					iv.move(640f);
				}
				else
				{
					iv.isRemoved_ = true;
					iv.move(-640f);
				}
				yield return 0;
			}
		}
		while (true)
		{
			bool finished = true;
			foreach (Ivy iv2 in ivyList)
			{
				if (!iv2.gameObject.activeSelf)
				{
					continue;
				}
				if (iv2.ivyType_ == Ivy.eType.Right)
				{
					if (iv2.transform.localPosition.x < 600f)
					{
						finished = false;
						break;
					}
				}
				else if (iv2.transform.localPosition.x > -600f)
				{
					finished = false;
					break;
				}
			}
			if (finished)
			{
				break;
			}
			yield return 0;
		}
		Ivy.setIvySe(false);
	}

	private void updateChainLock()
	{
		if (stageData.chainLayerNum == 0)
		{
			return;
		}
		List<int> heightList = new List<int>();
		List<int> list = new List<int>();
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (fieldBubble.type != Bubble.eType.Blank && !(fieldBubble.myTrans == null))
			{
				int item2 = Mathf.RoundToInt(fieldBubble.myTrans.localPosition.y);
				if (!heightList.Contains(item2))
				{
					heightList.Add(item2);
				}
			}
		});
		int count = heightList.Count;
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble item3 in chainBubbleDic[i])
			{
				if (item3.myTrans == null)
				{
					return;
				}
				if (item3.type == Bubble.eType.ChainLock)
				{
					int item = Mathf.RoundToInt(item3.myTrans.localPosition.y);
					if (!list.Contains(item))
					{
						list.Add(item);
					}
				}
			}
		}
		Dictionary<int, int> leftDic = new Dictionary<int, int>(heightList.Count);
		Dictionary<int, int> rightDic = new Dictionary<int, int>(heightList.Count);
		Dictionary<int, int> leftDicChain = new Dictionary<int, int>(list.Count);
		Dictionary<int, int> rightDicChain = new Dictionary<int, int>(list.Count);
		for (int j = 0; j < heightList.Count; j++)
		{
			leftDic.Add(heightList[j], -1000);
			rightDic.Add(heightList[j], 1000);
		}
		for (int k = 0; k < list.Count; k++)
		{
			leftDicChain.Add(list[k], -1000);
			rightDicChain.Add(list[k], 1000);
		}
		for (int l = 0; l < chainBubbleDic.Count; l++)
		{
			List<ChainBubble> list2 = chainBubbleDic[l];
			if (list2.Count == 0)
			{
				continue;
			}
			Bubble.eType chainType = getChainType(list2);
			switch (chainType)
			{
			case Bubble.eType.ChainHorizontal:
				list2.Sort((ChainBubble b1, ChainBubble b2) => (!(b1.myTrans.localPosition.x < b2.myTrans.localPosition.x)) ? 1 : (-1));
				break;
			case Bubble.eType.ChainRightDown:
			case Bubble.eType.ChainLeftDown:
				list2.Sort((ChainBubble b1, ChainBubble b2) => (!(b1.myTrans.localPosition.y > b2.myTrans.localPosition.y)) ? 1 : (-1));
				break;
			}
			list2[0].attachCollider(chainType);
			int num = Mathf.RoundToInt(list2[0].myTrans.localPosition.y);
			switch (chainType)
			{
			case Bubble.eType.ChainHorizontal:
			{
				for (int m = 0; m < heightList.Count; m++)
				{
					if (heightList[m] == num)
					{
						leftDic[heightList[m]] = 1000;
						rightDic[heightList[m]] = -1000;
					}
				}
				break;
			}
			case Bubble.eType.ChainRightDown:
				list2.ForEach(delegate(ChainBubble chain)
				{
					Vector3 localPosition5 = chain.myTrans.localPosition;
					int key5 = Mathf.RoundToInt(localPosition5.y);
					int num11 = Mathf.RoundToInt(localPosition5.x);
					if (rightDic.ContainsKey(key5) && rightDic[key5] > num11)
					{
						rightDic[key5] = num11;
					}
				});
				break;
			case Bubble.eType.ChainLeftDown:
				list2.ForEach(delegate(ChainBubble chain)
				{
					Vector3 localPosition6 = chain.myTrans.localPosition;
					int key6 = Mathf.RoundToInt(localPosition6.y);
					int num12 = Mathf.RoundToInt(localPosition6.x);
					if (leftDic.ContainsKey(key6) && leftDic[key6] < num12)
					{
						leftDic[key6] = num12;
					}
				});
				break;
			}
			for (int n = 0; n < heightList.Count; n++)
			{
				if (heightList[n] > num)
				{
					leftDic[heightList[n]] = 1000;
					rightDic[heightList[n]] = -1000;
				}
			}
		}
		for (int num2 = 0; num2 < chainBubbleDic.Count; num2++)
		{
			List<ChainBubble> list3 = chainBubbleDic[num2];
			if (list3.Count == 0)
			{
				continue;
			}
			Bubble.eType chainType2 = getChainType(list3);
			switch (chainType2)
			{
			case Bubble.eType.ChainHorizontal:
				list3.Sort((ChainBubble b1, ChainBubble b2) => (!(b1.myTrans.localPosition.x < b2.myTrans.localPosition.x)) ? 1 : (-1));
				break;
			case Bubble.eType.ChainRightDown:
			case Bubble.eType.ChainLeftDown:
				list3.Sort((ChainBubble b1, ChainBubble b2) => (!(b1.myTrans.localPosition.y > b2.myTrans.localPosition.y)) ? 1 : (-1));
				break;
			}
			list3[0].attachCollider(chainType2);
			int num3 = Mathf.RoundToInt(list3[0].myTrans.localPosition.y);
			switch (chainType2)
			{
			case Bubble.eType.ChainHorizontal:
			{
				for (int num4 = 0; num4 < list.Count; num4++)
				{
					if (list[num4] == num3)
					{
					}
				}
				break;
			}
			case Bubble.eType.ChainRightDown:
				list3.ForEach(delegate(ChainBubble chain)
				{
					Vector3 localPosition3 = chain.myTrans.localPosition;
					int key3 = Mathf.RoundToInt(localPosition3.y);
					int num9 = Mathf.RoundToInt(localPosition3.x);
					if (rightDicChain.ContainsKey(key3) && rightDicChain[key3] > num9)
					{
						rightDicChain[key3] = num9;
					}
				});
				break;
			case Bubble.eType.ChainLeftDown:
				list3.ForEach(delegate(ChainBubble chain)
				{
					Vector3 localPosition4 = chain.myTrans.localPosition;
					int key4 = Mathf.RoundToInt(localPosition4.y);
					int num10 = Mathf.RoundToInt(localPosition4.x);
					if (leftDicChain.ContainsKey(key4) && leftDicChain[key4] < num10)
					{
						leftDicChain[key4] = num10;
					}
				});
				break;
			}
			for (int num5 = 0; num5 < list.Count; num5++)
			{
				if (list[num5] > num3)
				{
					leftDicChain[list[num5]] = 1000;
					rightDicChain[list[num5]] = -1000;
				}
			}
		}
		for (int num6 = 0; num6 < chainBubbleDic.Count; num6++)
		{
			foreach (ChainBubble item4 in chainBubbleDic[num6])
			{
				bool mLocked = false;
				Vector3 localPosition = item4.myTrans.localPosition;
				int key = Mathf.RoundToInt(localPosition.y);
				int num7 = Mathf.RoundToInt(localPosition.x);
				if (leftDicChain.ContainsKey(key) && num7 < leftDicChain[key])
				{
					mLocked = true;
				}
				if (rightDicChain.ContainsKey(key) && num7 > rightDicChain[key])
				{
					mLocked = true;
				}
				item4.mLocked = mLocked;
			}
		}
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			if (fieldBubble.type != Bubble.eType.Blank && !(fieldBubble.myTrans == null))
			{
				bool isLocked = false;
				Vector3 localPosition2 = fieldBubble.myTrans.localPosition;
				int key2 = Mathf.RoundToInt(localPosition2.y);
				int num8 = Mathf.RoundToInt(localPosition2.x);
				if (num8 <= leftDic[key2])
				{
					isLocked = true;
				}
				if (num8 >= rightDic[key2])
				{
					isLocked = true;
				}
				fieldBubble.isLocked = isLocked;
			}
		});
	}

	private Bubble.eType getChainType(List<ChainBubble> chainList)
	{
		Bubble.eType result = Bubble.eType.ChainHorizontal;
		foreach (ChainBubble chain in chainList)
		{
			if (chain.type != Bubble.eType.ChainLock)
			{
				result = chain.type;
				break;
			}
		}
		return result;
	}

	private bool unlockChain(ChainBubble chainLock)
	{
		float num = chainLock.myTrans.localPosition.x - 270f;
		float num2 = chainLock.myTrans.localPosition.y + 455f;
		foreach (Cloud cloud in cloudList)
		{
			if (num > cloud.cloudMinX && num < cloud.cloudMaxX && num2 > cloud.cloudMinY && num2 < cloud.cloudMaxY && !cloud.isCloudMove)
			{
				return false;
			}
		}
		List<ChainBubble> list = null;
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			if (chainBubbleDic[i].Contains(chainLock))
			{
				list = chainBubbleDic[i];
				chainLock.unlockEffect();
				chainLock.setType(i, getChainType(list));
				Sound.Instance.playSe(Sound.eSe.SE_403_key_broken);
				break;
			}
		}
		bool flag = false;
		foreach (ChainBubble item in list)
		{
			if (item.type == Bubble.eType.ChainLock)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			chainBreakRoutine = StartCoroutine(chainBreak(list, chainLock));
			list.Clear();
			return true;
		}
		return false;
	}

	private IEnumerator chainBreak(List<ChainBubble> chainList, ChainBubble chainLock)
	{
		ChainBubble[] chainArray = chainList.ToArray();
		float range = 30f;
		Vector3 origin = chainLock.myTrans.localPosition;
		while (bMetalShooting)
		{
			yield return stagePause.sync();
		}
		bool bBreak;
		do
		{
			bBreak = false;
			ChainBubble[] array = chainArray;
			foreach (ChainBubble chain in array)
			{
				if (!(chain == null) && (origin - chain.myTrans.localPosition).sqrMagnitude < range * range)
				{
					bBreak = true;
					chain.startBreak();
				}
			}
			range += 60f;
			if (bBreak)
			{
				Sound.Instance.playSe(Sound.eSe.SE_404_chain_broken);
			}
			float waitTime = 0f;
			while (waitTime < 0.05f)
			{
				waitTime += Time.deltaTime;
				yield return stagePause.sync();
			}
		}
		while (bBreak);
	}

	private void updateOnChain()
	{
		foreach (Bubble fieldBubble in fieldBubbleList)
		{
			fieldBubble.isOnChain = false;
			for (int i = 0; i < chainBubbleDic.Count; i++)
			{
				foreach (ChainBubble item in chainBubbleDic[i])
				{
					if (fieldBubble.transform.localPosition.x == item.transform.localPosition.x && fieldBubble.transform.localPosition.y == item.transform.localPosition.y)
					{
						fieldBubble.isOnChain = true;
						break;
					}
				}
				if (fieldBubble.isOnChain)
				{
					break;
				}
			}
		}
	}

	private void setNextTap(bool flag)
	{
		if (next_tap.activeSelf != flag && snakeCount_ <= 0)
		{
			next_tap.SetActive(flag);
		}
	}

	private void setNextTapBobblen(bool flag)
	{
		BoxCollider component = charaObjs[1].GetComponent<BoxCollider>();
		if (!(component == null))
		{
			component.enabled = flag;
		}
	}

	private IEnumerator updateLineFriendBubble()
	{
		if (realLinkDic.Count == 0)
		{
			yield break;
		}
		switch (gameType)
		{
		case eGameType.ShotCount:
			if (stageInfo.Move - shotCount <= realLinkInfo.InvalidCount)
			{
				yield break;
			}
			break;
		case eGameType.Time:
			if ((int)((float)stageInfo.Time - (Time.time - startTime) + 0.999999f) <= realLinkInfo.InvalidTime)
			{
				yield break;
			}
			break;
		}
		int key = -1;
		for (int i = 0; i < realLinkDic.Count; i++)
		{
			if (realLinkDic.Values.ElementAt(i) == realLinkShotCount)
			{
				key = realLinkDic.Keys.ElementAt(i);
				break;
			}
		}
		if (key == -1)
		{
			yield break;
		}
		updateFieldBubbleList();
		if (lineFriendCandidateList.Count == 0)
		{
			if (realLinkDic[key] % realLinkTimingValue == 0)
			{
				realLinkDic.Remove(key);
				yield break;
			}
			Dictionary<int, int> dictionary;
			Dictionary<int, int> dictionary2 = (dictionary = realLinkDic);
			int key2;
			int key3 = (key2 = key);
			key2 = dictionary[key2];
			dictionary2[key3] = key2 + 1;
			yield break;
		}
		Bubble b = lineFriendCandidateList[random.Next(lineFriendCandidateList.Count)];
		realLinkDic.Remove(key);
		if (!b.setLineFriend(key))
		{
			yield break;
		}
		Transform friend_bonus_eff = frontUi.Find("friend_bonus_eff");
		friend_bonus_eff.gameObject.SetActive(true);
		Vector3 pos = friend_bonus_eff.position;
		pos.x = b.myTrans.position.x;
		pos.y = b.myTrans.position.y;
		friend_bonus_eff.position = pos;
		UITweener[] ts = friend_bonus_eff.GetComponentsInChildren<UITweener>();
		float waitTime = 0f;
		UITweener[] array = ts;
		foreach (UITweener t in array)
		{
			t.Reset();
			t.Play(true);
			if (waitTime < t.duration)
			{
				waitTime = t.duration;
			}
		}
		SaveOtherData otherData = SaveData.Instance.getGameData().getOtherData();
		if (!otherData.isFlag(SaveOtherData.eFlg.TutorialLineFriend))
		{
			float elapsedTime = 0f;
			while (elapsedTime < waitTime)
			{
				elapsedTime += Time.deltaTime;
				yield return stagePause.sync();
			}
			TutorialManager.Instance.load(-1, uiRoot);
			stagePause.pause = true;
			b.myTrans.localPosition += Vector3.back * 50f;
			yield return StartCoroutine(TutorialManager.Instance.play(-1, TutorialDataTable.ePlace.ReadyGoEnd, uiRoot, null, null));
			b.myTrans.localPosition += Vector3.forward * 50f;
			DialogBase dialogQuit = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
			while (dialogQuit.isOpen())
			{
				yield return null;
			}
			stagePause.pause = false;
			TutorialManager.Instance.unload();
			otherData.setFlag(SaveOtherData.eFlg.TutorialLineFriend, true);
			otherData.save();
			PlayerPrefs.Save();
		}
	}

	public void lineFriendBonus(GameObject icon)
	{
		float xForce = -5f + (float)rand.Next(11);
		float yForce = 80f + (float)rand.Next(10);
		lineFriendBonus(icon, 0f, xForce, yForce);
	}

	public void lineFriendBonus(GameObject icon, float delay, float xForce, float yForce)
	{
		int[] array = new int[4] { 45, 30, 20, 5 };
		int num = random.Next(100);
		eFriendBonus eFriendBonus = eFriendBonus.CountPlus;
		int num2 = 0;
		if (isGotJewel)
		{
			array[0] += array[3];
			for (int i = 0; i <= 2; i++)
			{
				num2 += array[i];
				if (num < num2)
				{
					eFriendBonus = (eFriendBonus)i;
					break;
				}
			}
		}
		else
		{
			for (int j = 0; j <= 3; j++)
			{
				num2 += array[j];
				if (num < num2)
				{
					eFriendBonus = (eFriendBonus)j;
					break;
				}
			}
		}
		int num3 = 0;
		int b = 0;
		switch (eFriendBonus)
		{
		case eFriendBonus.CountPlus:
			if (gameType == eGameType.ShotCount)
			{
				num3 = 5;
				b = 0;
			}
			else
			{
				num3 = 10;
				b = 1;
			}
			break;
		case eFriendBonus.Score:
		{
			num3 = stageInfo.Score / 3;
			int num4 = num3 % 10;
			num3 -= num4;
			if (num4 >= 5)
			{
				num3 += 10;
			}
			totalScore += num3;
			b = 2;
			break;
		}
		case eFriendBonus.Coin:
			num3 = 250;
			bonusCoin += num3;
			b = 3;
			break;
		case eFriendBonus.Jewel:
			num3 = 1;
			bonusJewel += num3;
			isGotJewel = true;
			b = 4;
			break;
		}
		int num5 = int.Parse(icon.name);
		FriendBonus item = new FriendBonus(b, num3, num5);
		friendBonusList.Add(item);
		Transform parent = icon.transform.parent;
		GameObject gameObject = UnityEngine.Object.Instantiate(lineFriendBase) as GameObject;
		gameObject.name = num5.ToString();
		Utility.setLayer(gameObject, "Default");
		Utility.setParent(gameObject, parent, true);
		float num6 = 1.1f / parent.localScale.x;
		gameObject.transform.localScale = new Vector3(num6, num6, 1f);
		gameObject.SetActive(true);
		UITexture component = gameObject.transform.Find("Player_icon").GetComponent<UITexture>();
		component.material = UnityEngine.Object.Instantiate(component.material) as Material;
		Texture texture = DummyPlayFriendData.DummyFriends[num5].Texture;
		if (texture != null)
		{
			component.mainTexture = texture;
		}
		else
		{
			component.mainTexture = defaultUserIconTexture;
		}
		UnityEngine.Object.Destroy(gameObject.transform.Find("frame").gameObject);
		UnityEngine.Object.Destroy(icon);
		Transform icon2 = gameObject.transform;
		StartCoroutine(friendDropRoutine(icon2, delay, xForce, yForce));
		StartCoroutine(friendDropRoutine2(icon2));
	}

	private void friendBonusPop(eFriendBonus bonus, int num, Transform parent, Vector3 target)
	{
		friendBonusPop(bonus, num, parent, target, 0, false);
	}

	private void friendBonusPop(eFriendBonus bonus, int num, Transform parent, Vector3 target, int depthOffset, bool isLocal)
	{
		GameObject gameObject = new GameObject("bonusParent");
		GameObject gameObject2 = UnityEngine.Object.Instantiate(bubbleBonusBases[(int)bonus]) as GameObject;
		Transform transform = gameObject2.transform;
		depthOffset += ((int)(target.x / uiScale) - 300) / 30 * -2;
		depthOffset += ((int)(target.y / uiScale) - 416) / 52 * -20;
		if (depthOffset < ivyDepthOffset)
		{
			depthOffset = ivyDepthOffset;
		}
		UISprite[] componentsInChildren = gameObject2.GetComponentsInChildren<UISprite>();
		UISprite[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].depth += depthOffset;
		}
		Vector3 localScale = transform.localScale;
		Utility.setParent(gameObject, parent.parent, true);
		if (!isLocal)
		{
			Vector3 position = gameObject.transform.position;
			position.x = target.x;
			position.y = target.y;
			gameObject.transform.position = position;
		}
		else
		{
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.x = target.x;
			localPosition.y = target.y;
			gameObject.transform.localPosition = localPosition;
		}
		gameObject.transform.localPosition += Vector3.back * 10f;
		Utility.setParent(gameObject2, gameObject.transform, true);
		transform.localScale = localScale;
		int num2 = 1;
		int length = num.ToString().Length;
		string text = "number_";
		for (int j = 0; j < 6; j++)
		{
			text += "0";
			Transform transform2 = transform.Find(text);
			if (transform2 == null)
			{
				break;
			}
			if (j > length)
			{
				transform2.gameObject.SetActive(false);
				continue;
			}
			UISprite component = transform2.GetComponent<UISprite>();
			if (j == length)
			{
				component.spriteName = "friend_bonus_number_10";
			}
			else
			{
				component.spriteName = "friend_bonus_number_0" + num % (num2 * 10) / num2;
			}
			component.MakePixelPerfect();
			num2 *= 10;
		}
		UITweener[] componentsInChildren2 = gameObject2.GetComponentsInChildren<UITweener>(true);
		UITweener[] array2 = componentsInChildren2;
		foreach (UITweener uITweener in array2)
		{
			uITweener.Reset();
			uITweener.Play(true);
		}
	}

	private IEnumerator friendDropRoutine2(Transform icon)
	{
		iTween it = icon.parent.GetComponent<iTween>();
		while (it != null)
		{
			yield return stagePause.sync();
		}
		icon.parent = icon.parent.parent.parent.parent;
	}

	private IEnumerator friendDropRoutine(Transform icon, float delay, float xForce, float yForce)
	{
		Rigidbody rb = icon.gameObject.AddComponent<Rigidbody>();
		rb.constraints = RigidbodyConstraints.FreezeAll;
		float waitTime = 0f;
		while (waitTime < 0.01f * delay)
		{
			waitTime += Time.deltaTime;
			yield return stagePause.sync();
		}
		yield return stagePause.sync();
		rb.constraints = (RigidbodyConstraints)120;
		rb.AddForce(new Vector3(xForce, 0f, 0f));
		while (icon.localPosition.y > -420f)
		{
			yield return stagePause.sync();
		}
		Sound.Instance.playSe(Sound.eSe.SE_216_sessyoku);
		Vector3 p = icon.localPosition;
		p.y = -420f;
		icon.localPosition = p;
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
		UITexture[] textures = icon.GetComponentsInChildren<UITexture>();
		while (textures[0].color.a > 0f)
		{
			UITexture[] array = textures;
			foreach (UITexture texture in array)
			{
				Color color = texture.color;
				color.a -= Time.deltaTime * 5f;
				if (color.a < 0f)
				{
					color.a = 0f;
				}
				texture.color = color;
			}
			yield return stagePause.sync();
		}
		UnityEngine.Object.Destroy(icon.gameObject);
	}

	private IEnumerator setupRealLink()
	{
		int friendCount = DummyPlayFriendData.DummyFriends.Length;
		if (friendCount == 0)
		{
			yield break;
		}
		GameObject dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		RealLinkDataTable tbl = dataTbl.GetComponent<RealLinkDataTable>();
		tbl.load();
		realLinkInfo = tbl.getInfo();
		if (stageNo < realLinkInfo.InvalidStage)
		{
			yield break;
		}
		int appearanceMaximum = Mathf.RoundToInt((float)normalBubbleCount / (float)realLinkInfo.Divide);
		int probabilityNumber = PlayerPrefs.GetInt("ProbabilityNumber", 0);
		float probabilityValue = realLinkInfo.Probability[probabilityNumber];
		float appearanceValue = 0f;
		for (int m = 0; m < realLinkInfo.FriendCount.Length; m++)
		{
			if (friendCount <= realLinkInfo.FriendCount[m])
			{
				appearanceValue = realLinkInfo.Appearance[m];
				break;
			}
		}
		int friendSummon2 = 1;
		friendSummon2 = ((!bFriendIncidenceUp) ? 1 : Mathf.RoundToInt((float)friendSummon2 * IncidenceUp));
		int incidence = Mathf.RoundToInt(probabilityValue * appearanceValue * (float)friendSummon2);
		Debug.Log("確率値 " + probabilityValue);
		Debug.Log("出現値 " + appearanceValue);
		Debug.Log("スキル倍率 " + friendSummon2);
		Debug.Log("incidence(出現率) " + incidence);
		int appearanceConfirmed = 0;
		for (int l = 0; l < appearanceMaximum; l++)
		{
			if (incidence > random.Next(100))
			{
				appearanceConfirmed++;
			}
		}
		if (appearanceConfirmed > friendCount)
		{
			appearanceConfirmed = friendCount;
		}
		if (appearanceConfirmed == 0)
		{
			if (probabilityNumber < realLinkInfo.Probability.Length - 1)
			{
				PlayerPrefs.SetInt("ProbabilityNumber", probabilityNumber + 1);
			}
			yield break;
		}
		PlayerPrefs.SetInt("ProbabilityNumber", 0);
		List<int> friendIndexList = new List<int>();
		for (int k = 0; k < friendCount; k++)
		{
			friendIndexList.Add(k);
		}
		for (int j = 0; j < appearanceConfirmed; j++)
		{
			int index = friendIndexList[random.Next(friendIndexList.Count)];
			yield return StartCoroutine(loadFriendProfTexture(index));
			realLinkDic.Add(index, 0);
			friendIndexList.Remove(index);
			if (friendIndexList.Count < 1)
			{
				break;
			}
		}
		if (gameType == eGameType.ShotCount)
		{
			realLinkTimingValue = (int)((float)(stageInfo.Move - realLinkInfo.InvalidCount) / (float)appearanceConfirmed);
		}
		else
		{
			realLinkTimingValue = (int)((float)(stageInfo.Time - realLinkInfo.InvalidTime) / (float)appearanceConfirmed / 2f);
		}
		if (realLinkTimingValue < 1)
		{
			realLinkTimingValue = 1;
		}
		for (int i = 1; i <= appearanceConfirmed; i++)
		{
			int min = 1 + (i - 1) * realLinkTimingValue;
			int max = i * realLinkTimingValue;
			int key = realLinkDic.Keys.ElementAt(i - 1);
			int timing = min + random.Next(max - min + 1);
			realLinkDic[key] = timing;
		}
	}

	private IEnumerator updateKeyBubble()
	{
		if (keyBubbleTimingValue[0] == 0 && keyBubbleTimingValue[1] == 0)
		{
			yield break;
		}
		switch (gameType)
		{
		case eGameType.ShotCount:
			if (stageInfo.Move - shotCount <= keyBubbleInfo.InvalidCount)
			{
				yield break;
			}
			break;
		case eGameType.Time:
			if ((int)((float)stageInfo.Time - (Time.time - startTime) + 0.999999f) <= keyBubbleInfo.InvalidTime)
			{
				yield break;
			}
			break;
		}
		if (keyBubbleTimingValue[0] != shotCount && keyBubbleTimingValue[1] != shotCount)
		{
			yield break;
		}
		updateFieldBubbleList();
		if (lineFriendCandidateList.Count == 0)
		{
			if (keyBubbleTimingValue[0] != 0)
			{
				keyBubbleTimingValue[0]++;
			}
			if (keyBubbleTimingValue[1] != 0)
			{
				keyBubbleTimingValue[1]++;
			}
			yield break;
		}
		if (keyBubbleTimingValue[0] == shotCount)
		{
			if (keyBubbleTimingValue[0] == 0)
			{
				yield break;
			}
			keyBubbleTimingValue[0] = 0;
		}
		else if (keyBubbleTimingValue[1] == shotCount)
		{
			if (keyBubbleTimingValue[1] == 0)
			{
				yield break;
			}
			keyBubbleTimingValue[1] = 0;
		}
		Bubble b = lineFriendCandidateList[random.Next(lineFriendCandidateList.Count)];
		b.setType(b.type + 91);
		Transform friend_bonus_eff = frontUi.Find("friend_bonus_eff");
		friend_bonus_eff.gameObject.SetActive(true);
		Vector3 pos = friend_bonus_eff.position;
		pos.x = b.myTrans.position.x;
		pos.y = b.myTrans.position.y;
		friend_bonus_eff.position = pos;
		UITweener[] ts = friend_bonus_eff.GetComponentsInChildren<UITweener>();
		float waitTime = 0f;
		UITweener[] array = ts;
		foreach (UITweener t in array)
		{
			t.Reset();
			t.Play(true);
			if (waitTime < t.duration)
			{
				waitTime = t.duration;
			}
		}
		SaveOtherData otherData = SaveData.Instance.getGameData().getOtherData();
		if (!otherData.isFlag(SaveOtherData.eFlg.TutorialKeyBubble))
		{
			float elapsedTime = 0f;
			while (elapsedTime < waitTime)
			{
				elapsedTime += Time.deltaTime;
				yield return stagePause.sync();
			}
			TutorialManager.Instance.load(-1, uiRoot);
			stagePause.pause = true;
			b.myTrans.localPosition += Vector3.back * 50f;
			yield return StartCoroutine(TutorialManager.Instance.play(-12, TutorialDataTable.ePlace.ReadyGoEnd, uiRoot, null, null));
			b.myTrans.localPosition += Vector3.forward * 50f;
			DialogBase dialogQuit = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
			while (dialogQuit.isOpen())
			{
				yield return null;
			}
			stagePause.pause = false;
			TutorialManager.Instance.unload();
			otherData.setFlag(SaveOtherData.eFlg.TutorialKeyBubble, true);
			otherData.save();
			PlayerPrefs.Save();
		}
	}

	private IEnumerator MinilenBubbleDropTutorial()
	{
		if (minilen_count_pop <= 0)
		{
			yield break;
		}
		SaveOtherData otherData = SaveData.Instance.getGameData().getOtherData();
		if (!otherData.isFlag(SaveOtherData.eFlg.TutorialMinilenBubbleDrop))
		{
			TutorialManager.Instance.load(-1, uiRoot);
			stagePause.pause = true;
			yield return StartCoroutine(TutorialManager.Instance.play(-500003, TutorialDataTable.ePlace.ReadyGoEnd, uiRoot, null, null));
			DialogBase dialogQuit = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
			while (dialogQuit.isOpen())
			{
				yield return null;
			}
			stagePause.pause = false;
			TutorialManager.Instance.unload();
			otherData.setFlag(SaveOtherData.eFlg.TutorialMinilenBubbleDrop, true);
			otherData.save();
			PlayerPrefs.Save();
		}
	}

	private IEnumerator MinilenFirstGetTutorial()
	{
		SaveOtherData otherData = SaveData.Instance.getGameData().getOtherData();
		if (!otherData.isFlag(SaveOtherData.eFlg.TutorialMinilenGet))
		{
			TutorialManager.Instance.load(-1, uiRoot);
			GameObject tutorial_panel = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.Tutorial);
			if ((bool)tutorial_panel)
			{
				tutorial_panel.transform.localPosition += new Vector3(0f, 0f, -10f);
			}
			stagePause.pause = true;
			yield return StartCoroutine(TutorialManager.Instance.play(-500004, TutorialDataTable.ePlace.ReadyGoEnd, uiRoot, null, null));
			DialogBase dialogQuit = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
			while (dialogQuit.isOpen())
			{
				yield return null;
			}
			stagePause.pause = false;
			TutorialManager.Instance.unload();
			if ((bool)tutorial_panel)
			{
				tutorial_panel.transform.localPosition -= new Vector3(0f, 0f, -10f);
			}
			otherData.setFlag(SaveOtherData.eFlg.TutorialMinilenGet, true);
			otherData.save();
			PlayerPrefs.Save();
		}
	}

	public void keyBubbleBonus(Transform tran)
	{
		float xForce = -5f + (float)rand.Next(11);
		float yForce = 80f + (float)rand.Next(10);
		keyBubbleBonus(tran, 0f, xForce, yForce);
	}

	public void keyBubbleBonus(Transform tran, float delay, float xForce, float yForce)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(keyIcon) as GameObject;
		Utility.setParent(gameObject, keyIcon.transform, true);
		gameObject.transform.position = tran.position;
		gameObject.SetActive(true);
		bGetKey = true;
		StartCoroutine(keyBubbleDropRoutine(gameObject.transform, delay, xForce, yForce));
		StartCoroutine(keyBubbleDropRoutine2(gameObject.transform));
	}

	private IEnumerator keyBubbleDropRoutine2(Transform icon)
	{
		iTween it = icon.parent.GetComponent<iTween>();
		while (it != null)
		{
			yield return stagePause.sync();
		}
		icon.parent = icon.parent.parent.parent;
	}

	private IEnumerator keyBubbleDropRoutine(Transform icon, float delay, float xForce, float yForce)
	{
		Rigidbody rb = icon.gameObject.AddComponent<Rigidbody>();
		rb.constraints = RigidbodyConstraints.FreezeAll;
		float waitTime = 0f;
		while (waitTime < 0.01f * delay)
		{
			waitTime += Time.deltaTime;
			yield return stagePause.sync();
		}
		yield return stagePause.sync();
		rb.constraints = (RigidbodyConstraints)120;
		rb.AddForce(new Vector3(xForce, 0f, 0f));
		while (icon.localPosition.y > -420f)
		{
			yield return stagePause.sync();
		}
		Sound.Instance.playSe(Sound.eSe.SE_216_sessyoku);
		Vector3 p = icon.localPosition;
		p.y = -420f;
		icon.localPosition = p;
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
		UISprite sprite = icon.GetComponentInChildren<UISprite>();
		while (sprite.color.a > 0f)
		{
			Color color = sprite.color;
			color.a -= Time.deltaTime * 5f;
			if (color.a < 0f)
			{
				color.a = 0f;
			}
			sprite.color = color;
			yield return stagePause.sync();
		}
		UnityEngine.Object.Destroy(icon.gameObject);
	}

	private IEnumerator setupKeyBubble()
	{
		GameObject dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		KeyBubbleDataTable tbl = dataTbl.GetComponent<KeyBubbleDataTable>();
		tbl.load();
		keyBubbleInfo = tbl.getInfo();
		keyData = GlobalData.Instance.getKeyBubbleData();
		keyBubbleInfo.InvalidCount = keyData.invalidCount;
		keyBubbleInfo.InvalidTime = keyData.invalidTime;
		getKeyBubble = keyData.getKeyBubble;
		if (getKeyBubble >= 1)
		{
			if (gameType == eGameType.ShotCount)
			{
				if (stageInfo.Move > keyBubbleInfo.InvalidCount)
				{
					keyBubbleTimingValue[0] = random.Next(stageInfo.Move - keyBubbleInfo.InvalidCount);
					if (keyBubbleTimingValue[0] < 1)
					{
						keyBubbleTimingValue[0] = 1;
					}
				}
				else
				{
					keyBubbleTimingValue[0] = 0;
				}
			}
			else if (stageInfo.Time > keyBubbleInfo.InvalidTime)
			{
				keyBubbleTimingValue[0] = random.Next(stageInfo.Time - keyBubbleInfo.InvalidTime) / 4;
				if (keyBubbleTimingValue[0] < 1)
				{
					keyBubbleTimingValue[0] = 1;
				}
			}
			else
			{
				keyBubbleTimingValue[0] = 0;
			}
		}
		if (gameType == eGameType.ShotCount && stageInfo.Move <= keyBubbleInfo.InvalidCount + 2)
		{
			keyBubbleTimingValue[1] = 0;
		}
		else
		{
			if (getKeyBubble != 2)
			{
				yield break;
			}
			do
			{
				if (gameType == eGameType.ShotCount)
				{
					if (stageInfo.Move <= keyBubbleInfo.InvalidCount + 1)
					{
						keyBubbleTimingValue[1] = 0;
						break;
					}
					keyBubbleTimingValue[1] = random.Next(stageInfo.Move - keyBubbleInfo.InvalidCount);
					if (keyBubbleTimingValue[1] < 1)
					{
						keyBubbleTimingValue[1] = 1;
					}
				}
				else
				{
					if (stageInfo.Time <= keyBubbleInfo.InvalidTime * 2)
					{
						keyBubbleTimingValue[1] = 0;
						break;
					}
					keyBubbleTimingValue[1] = random.Next(stageInfo.Time - keyBubbleInfo.InvalidTime) / 4;
					if (keyBubbleTimingValue[1] < 1)
					{
						keyBubbleTimingValue[1] = 1;
					}
				}
				yield return stagePause.sync();
			}
			while (keyBubbleTimingValue[0] == keyBubbleTimingValue[1]);
		}
	}

	private void changeBubbleColor()
	{
		int playCount = Bridge.StageData.getPlayCount(stageNo);
		if (playCount <= 1)
		{
			return;
		}
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		List<int> list = new List<int>(8);
		List<int> list2 = new List<int>(8);
		for (int i = 0; i <= 7; i++)
		{
			list.Add(i);
			list2.Add(i);
		}
		if (stageNo == 0)
		{
			int num = 3;
			dictionary.Add(num, num);
			list.Remove(num);
			list2.Remove(num);
		}
		else if (stageNo == 2)
		{
			int num2 = 0;
			dictionary.Add(num2, num2);
			list.Remove(num2);
			list2.Remove(num2);
			num2 = 2;
			dictionary.Add(num2, num2);
			list.Remove(num2);
			list2.Remove(num2);
		}
		else if (stageNo == 3)
		{
			int num3 = 1;
			dictionary.Add(num3, num3);
			list.Remove(num3);
			list2.Remove(num3);
		}
		while (list.Count > 0)
		{
			int num4 = list[random.Next(list.Count)];
			int num5 = list2[random.Next(list2.Count)];
			dictionary.Add(num4, num5);
			list.Remove(num4);
			list2.Remove(num5);
		}
		for (int j = 0; j < stageData.bubbleTypes.Length; j++)
		{
			int num6 = stageData.bubbleTypes[j];
			if (isColorBubble(num6) || (num6 >= 79 && num6 <= 86) || (num6 >= 109 && num6 <= 116))
			{
				int num7 = num6;
				if (dictionary.ContainsKey(num6))
				{
					num7 = dictionary[num6];
				}
				else if (dictionary.ContainsKey(num6 - 13))
				{
					num7 = dictionary[num6 - 13] + 13;
				}
				else if (dictionary.ContainsKey(num6 - 21))
				{
					num7 = dictionary[num6 - 21] + 21;
				}
				else if (dictionary.ContainsKey(num6 - 67))
				{
					num7 = dictionary[num6 - 67] + 67;
				}
				else if (dictionary.ContainsKey(num6 - 31))
				{
					num7 = dictionary[num6 - 31] + 31;
				}
				else if (dictionary.ContainsKey(num6 - 79))
				{
					num7 = dictionary[num6 - 79] + 79;
				}
				else if (dictionary.ContainsKey(num6 - 100))
				{
					num7 = dictionary[num6 - 100] + 100;
				}
				else if (dictionary.ContainsKey(num6 - 109))
				{
					num7 = dictionary[num6 - 109] + 109;
				}
				else if (dictionary.ContainsKey(num6 - 128))
				{
					num7 = dictionary[num6 - 128] + 128;
				}
				stageData.bubbleTypes[j] = (byte)num7;
			}
		}
		if (stageData.skeltonIndex != null)
		{
			for (int k = 0; k < stageData.skeltonIndex.Length; k++)
			{
				stageData.skeltonColor[k] = dictionary[stageData.skeltonColor[k]];
			}
		}
		noSearchColor = (Bubble.eType)dictionary[0];
	}

	private bool isColorBubble(int t)
	{
		if (t <= 7)
		{
			return true;
		}
		if (t >= 13 && t <= 20)
		{
			return true;
		}
		if (t >= 21 && t <= 28)
		{
			return true;
		}
		if (t >= 31 && t <= 38)
		{
			return true;
		}
		if (t >= 67 && t <= 74)
		{
			return true;
		}
		if (t >= 91 && t <= 98)
		{
			return true;
		}
		if (t >= 100 && t <= 107)
		{
			return true;
		}
		if (t >= 128 && t <= 135)
		{
			return true;
		}
		return false;
	}

	public bool isKeyBubble(int t)
	{
		return t >= 91 && t <= 98;
	}

	private void nextBubbleHideForShotCount(bool bChangeUp = true)
	{
		if (gameType != 0)
		{
			if (nextBubbles[0] != null)
			{
				Bubble component = nextBubbles[0].GetComponent<Bubble>();
				if (component.state == Bubble.eState.Wait && !isSpecialBubble(component.type))
				{
					setNextTap(true);
				}
			}
			return;
		}
		int num = stageInfo.Move - shotCount;
		for (int i = 0; i < nextBubbleCount; i++)
		{
			if (!(nextBubbles[i] == null) && nextBubbles[i].GetComponent<Bubble>().state == Bubble.eState.Wait)
			{
				Vector3 position = nextBubbles[i].transform.position;
				position.x = nextBubblePoses[i].position.x;
				position.y = nextBubblePoses[i].position.y;
				nextBubbles[i].transform.position = position;
				nextBubbles[i].SetActive(i < num);
			}
		}
		if (num <= 1)
		{
			setNextTap(false);
		}
		else if (nextBubbles[0] != null)
		{
			Bubble component2 = nextBubbles[0].GetComponent<Bubble>();
			if (component2.state == Bubble.eState.Wait && !isSpecialBubble(component2.type))
			{
				setNextTap(true);
			}
		}
		if (!(itemChangeUp_ == null) && !itemChangeUp_.isBuy() && state >= eState.Wait && bChangeUp)
		{
			if (num <= 2)
			{
				itemChangeUp_.disable();
				itemChangeUp_.setStateFixed(true);
			}
			else
			{
				itemChangeUp_.setStateFixed(false);
				itemChangeUp_.enable();
			}
		}
	}

	private void nextBubbleHideForShotCountBobllen()
	{
		if (next_tap.activeSelf)
		{
			next_tap.SetActive(false);
		}
		if (nextBubbles[0] == null)
		{
			createNextBubble(0, false);
		}
		nextBubbles[0].SetActive(false);
		updateChangeBubbleBobblen();
		if (gameType != 0)
		{
			return;
		}
		int num = stageInfo.Move - shotCount;
		for (int i = 1; i < nextBubbleCount; i++)
		{
			nextBubbles[i].SetActive(num >= i);
		}
		if (!(itemChangeUp_ == null) && !itemChangeUp_.isBuy() && state >= eState.Wait)
		{
			if (num <= 1)
			{
				itemChangeUp_.disable();
				itemChangeUp_.setStateFixed(true);
			}
			else
			{
				itemChangeUp_.setStateFixed(false);
				itemChangeUp_.enable();
			}
		}
	}

	private void setClearConditionDisplay()
	{
		Transform transform = frontUi.Find("Top_ui/clear_condition_top");
		if (bEventStage_)
		{
			UnityEngine.Object.Destroy(transform.gameObject);
			if (eventNo_ == 1)
			{
				UnityEngine.Object.Destroy(frontUi.Find("Top_ui/clear_condition_collaboration_top").gameObject);
				transform = frontUi.Find("Top_ui/clear_condition_event_top");
			}
			else if (eventNo_ == 2 || eventNo_ == 11)
			{
				UnityEngine.Object.Destroy(frontUi.Find("Top_ui/clear_condition_event_top").gameObject);
				transform = frontUi.Find("Top_ui/clear_condition_collaboration_top");
			}
			transform.gameObject.SetActive(true);
		}
		else
		{
			UnityEngine.Object.Destroy(frontUi.Find("Top_ui/clear_condition_event_top").gameObject);
			UnityEngine.Object.Destroy(frontUi.Find("Top_ui/clear_condition_collaboration_top").gameObject);
		}
		clear_condition_stamp_00 = transform.Find("clear_condition_stamp_00").gameObject;
		clear_condition_stamp_01 = transform.Find("clear_condition_stamp_01").gameObject;
		GameObject gameObject = new GameObject("stamp_top");
		Utility.setParent(gameObject, transform, true);
		gameObject.AddComponent<UIPanel>();
		clear_condition_stamp_00.transform.parent = gameObject.transform;
		clear_condition_stamp_01.transform.parent = gameObject.transform;
		clear_condition_stamp_00.transform.localPosition = new Vector3(clear_condition_stamp_00.transform.localPosition.x, clear_condition_stamp_00.transform.localPosition.y, -2f);
		clear_condition_stamp_01.transform.localPosition = new Vector3(clear_condition_stamp_01.transform.localPosition.x, clear_condition_stamp_01.transform.localPosition.y, -2f);
		int num;
		if (stageInfo.IsMinilenDelete)
		{
			num = ((gameType != eGameType.Time) ? 6 : 7);
		}
		else
		{
			num = ((stageInfo.IsAllDelete || stageInfo.IsFulcrumDelete) ? 1 : ((!stageInfo.IsFriendDelete) ? 2 : 0));
			if (gameType == eGameType.Time)
			{
				num += 3;
			}
		}
		for (int i = 0; i < 8; i++)
		{
			Transform transform2 = transform.Find(i.ToString("00"));
			if ((bool)transform2)
			{
				transform2.gameObject.SetActive(i == num);
			}
		}
		Transform transform3 = transform.Find(num.ToString("00") + "/clearscore");
		if (bParkStage_)
		{
			Transform transform4 = transform3.Find("bg");
			if (transform4 != null)
			{
				UISprite component = transform4.GetComponent<UISprite>();
				if (component != null)
				{
					component.spriteName = "clear_condition_score_02";
					component.MakePixelPerfect();
					transform4.localScale *= 0.5f;
				}
			}
		}
		int score = stageInfo.Score;
		int length = score.ToString().Length;
		Transform transform5 = transform3.Find("score_number");
		transform5.name = "0";
		for (int j = 1; j < length; j++)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(transform5.gameObject) as GameObject;
			gameObject2.name = j.ToString();
			Utility.setParent(gameObject2, transform5.parent, true);
			gameObject2.transform.localPosition = transform5.localPosition;
		}
		int num2 = 1;
		Transform transform6 = null;
		for (int k = 0; k < length; k++)
		{
			Transform transform7 = transform3.Find(k.ToString());
			UISprite component2 = transform7.GetComponent<UISprite>();
			component2.spriteName = "game_score_number_0" + score % (num2 * 10) / num2;
			component2.MakePixelPerfect();
			num2 *= 10;
			if (k > 0)
			{
				Vector3 localPosition = transform6.localPosition;
				localPosition.x -= (transform6.localScale.x + transform7.localScale.x - 2f) * 0.5f;
				transform7.localPosition = localPosition;
			}
			transform6 = transform7;
		}
		switch (num)
		{
		case 0:
			chacknNumLabel = transform.Find("00/rescuefriend/number_label").GetComponent<UILabel>();
			chacknNumLabel.text = "x" + fieldFriendCount;
			break;
		case 3:
			chacknNumLabel = transform.Find("03/rescuefriend/number_label").GetComponent<UILabel>();
			chacknNumLabel.text = "x" + fieldFriendCount;
			break;
		case 6:
			chacknNumLabel = transform.Find("06/rescuefriend/number_label").GetComponent<UILabel>();
			chacknNumLabel.text = "x" + minilen_count_all;
			break;
		case 7:
			chacknNumLabel = transform.Find("07/rescuefriend/number_label").GetComponent<UILabel>();
			chacknNumLabel.text = "x" + minilen_count_all;
			break;
		}
		Transform transform8 = transform.Find(num.ToString("00") + "/Minilen");
		if ((bool)transform8)
		{
			if (bParkStage_)
			{
				minilen_count_label = new List<UISprite>();
				minilen_count_label.Add(transform8.Find("score_number").GetComponent<UISprite>());
				minilen_count_label[0].spriteName = "game_score_number_00";
			}
			else
			{
				transform8.gameObject.SetActive(false);
				minilen_count_label = null;
			}
		}
	}

	private void playCountdownEff(bool bPlus)
	{
		if (bPlus)
		{
			countdown_eff.SetActive(true);
			if (countdown_eff_tweeners == null)
			{
				countdown_eff_tweeners = countdown_eff.GetComponentsInChildren<UITweener>();
			}
			UITweener[] array = countdown_eff_tweeners;
			foreach (UITweener uITweener in array)
			{
				uITweener.Reset();
				uITweener.style = UITweener.Style.Once;
				uITweener.Play(true);
			}
		}
		else
		{
			countdown_bad_eff.SetActive(true);
			if (countdown_bad_eff_tweeners == null)
			{
				countdown_bad_eff_tweeners = countdown_bad_eff.GetComponentsInChildren<UITweener>();
			}
			UITweener[] array2 = countdown_bad_eff_tweeners;
			foreach (UITweener uITweener2 in array2)
			{
				uITweener2.Reset();
				uITweener2.style = UITweener.Style.Once;
				uITweener2.Play(true);
			}
		}
	}

	private IEnumerator playSkullBarrierEff(Vector3 setPos)
	{
		skullBarrier.SetActive(true);
		Sound.Instance.playSe(Sound.eSe.SE_222_kakoumae);
		Vector3 pos = skullBarrier.transform.position;
		pos.x = setPos.x;
		pos.y = setPos.y;
		skullBarrier.transform.position = pos;
		UISpriteAnimationEx anim = skullBarrier.GetComponentInChildren<UISpriteAnimationEx>();
		anim.SetClip(0);
		while (anim.isPlaying)
		{
			yield return stagePause.sync();
		}
		skullBarrier.SetActive(false);
	}

	private void tutorialStart()
	{
		otherData_ = SaveData.Instance.getGameData().getOtherData();
		Bubble.eType type = (Bubble.eType)TutorialManager.Instance.getHighlightBubble(stageNo);
		if (type != Bubble.eType.Invalid)
		{
			type--;
			fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
			{
				switch (type)
				{
				case Bubble.eType.PlusRed:
					if (fieldBubble.type >= Bubble.eType.PlusRed && fieldBubble.type <= Bubble.eType.PlusBlack)
					{
						fieldBubble.myTrans.localPosition += Vector3.back * 46f;
					}
					break;
				case Bubble.eType.MinusRed:
					if (fieldBubble.type >= Bubble.eType.MinusRed && fieldBubble.type <= Bubble.eType.MinusBlack)
					{
						fieldBubble.myTrans.localPosition += Vector3.back * 46f;
					}
					break;
				case Bubble.eType.SnakeRed:
					if (fieldBubble.type >= Bubble.eType.SnakeRed && fieldBubble.type <= Bubble.eType.SnakeBlack)
					{
						fieldBubble.myTrans.localPosition += Vector3.back * 46f;
					}
					break;
				case Bubble.eType.ChameleonRed:
					if (fieldBubble.type >= Bubble.eType.ChameleonRed && fieldBubble.type <= Bubble.eType.ChameleonBlack)
					{
						fieldBubble.myTrans.localPosition += Vector3.back * 46f;
					}
					break;
				case Bubble.eType.BlackHole_A:
					if (fieldBubble.type == Bubble.eType.BlackHole_A || fieldBubble.type == Bubble.eType.BlackHole_B)
					{
						fieldBubble.myTrans.localPosition += Vector3.back * 46f;
					}
					break;
				case Bubble.eType.CounterRed:
					if (fieldBubble.type >= Bubble.eType.CounterRed && fieldBubble.type <= Bubble.eType.CounterBlack)
					{
						fieldBubble.myTrans.localPosition += Vector3.back * 46f;
					}
					break;
				case Bubble.eType.MorganaRed:
					if (fieldBubble.type >= Bubble.eType.MorganaRed && fieldBubble.type <= Bubble.eType.MorganaBlack)
					{
						foreach (Bubble item in MorganaList_)
						{
							item.myTrans.localPosition += Vector3.back * 10f;
							if (item.myTrans.Find("chara_01_19(Clone)") != null)
							{
								item.myTrans.Find("chara_01_19(Clone)").transform.localPosition += Vector3.back * 6f;
							}
						}
						break;
					}
					break;
				case Bubble.eType.TunnelIn:
					if (fieldBubble.type >= Bubble.eType.TunnelIn && fieldBubble.type <= Bubble.eType.TunnelOutRightDown)
					{
						fieldBubble.myTrans.localPosition += Vector3.back * 46f;
					}
					break;
				case Bubble.eType.RotateFulcrumR:
					if (fieldBubble.type >= Bubble.eType.RotateFulcrumR && fieldBubble.type <= Bubble.eType.RotateFulcrumL)
					{
						fieldBubble.myTrans.localPosition += Vector3.back * 46f;
					}
					break;
				case Bubble.eType.MinilenRed:
					if (fieldBubble.type >= Bubble.eType.MinilenRed && fieldBubble.type <= Bubble.eType.MinilenBlack)
					{
						fieldBubble.myTrans.localPosition += Vector3.back * 46f;
					}
					break;
				default:
					if (fieldBubble.type == type)
					{
						fieldBubble.myTrans.localPosition += Vector3.back * 46f;
					}
					break;
				}
			});
			if (type == Bubble.eType.ChainHorizontal || type == Bubble.eType.ChainLeftDown || type == Bubble.eType.ChainRightDown || type == Bubble.eType.ChainLock)
			{
				for (int i = 0; i < chainBubbleDic.Count; i++)
				{
					List<ChainBubble> list = chainBubbleDic[i];
					if (list.Count != 0)
					{
						for (int j = 0; j < list.Count; j++)
						{
							list[j].myTrans.localPosition += Vector3.back * 46f;
						}
					}
				}
			}
		}
		GimmickBase.eGimmickType highlightGimmick = (GimmickBase.eGimmickType)TutorialManager.Instance.getHighlightGimmick(stageNo);
		if (highlightGimmick == GimmickBase.eGimmickType.Invalid)
		{
			return;
		}
		switch (highlightGimmick - 1)
		{
		case GimmickBase.eGimmickType.Ivy:
		{
			foreach (Ivy ivy in ivyList)
			{
				ivy.gameObject.transform.localPosition += Vector3.back * 56f;
			}
			break;
		}
		case GimmickBase.eGimmickType.UFO:
		{
			foreach (ObstacleDefend obstacle in obstacleList)
			{
				obstacle.gameObject.transform.localPosition += Vector3.back * 90f;
			}
			break;
		}
		}
	}

	private void tutorialStartObjects(int spNum)
	{
		GimmickBase.eGimmickType highlightGimmick = (GimmickBase.eGimmickType)TutorialManager.Instance.getHighlightGimmick(spNum);
		if (highlightGimmick == GimmickBase.eGimmickType.Invalid)
		{
			return;
		}
		switch (highlightGimmick - 1)
		{
		case GimmickBase.eGimmickType.Ivy:
		{
			foreach (Ivy ivy in ivyList)
			{
				ivy.gameObject.transform.localPosition += Vector3.back * 56f;
			}
			break;
		}
		case GimmickBase.eGimmickType.Cloud:
		{
			foreach (Cloud cloud in cloudList)
			{
				cloud.gameObject.transform.localPosition += Vector3.back * 56f;
			}
			break;
		}
		}
	}

	private void tutorialEnd()
	{
		Bubble.eType type = (Bubble.eType)TutorialManager.Instance.getHighlightBubble(stageNo);
		if (type != Bubble.eType.Invalid)
		{
			type--;
			fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
			{
				switch (type)
				{
				case Bubble.eType.PlusRed:
					if (fieldBubble.type >= Bubble.eType.PlusRed && fieldBubble.type <= Bubble.eType.PlusBlack)
					{
						fieldBubble.myTrans.localPosition += Vector3.forward * 46f;
					}
					break;
				case Bubble.eType.MinusRed:
					if (fieldBubble.type >= Bubble.eType.MinusRed && fieldBubble.type <= Bubble.eType.MinusBlack)
					{
						fieldBubble.myTrans.localPosition += Vector3.forward * 46f;
					}
					break;
				case Bubble.eType.SnakeRed:
					if (fieldBubble.type >= Bubble.eType.SnakeRed && fieldBubble.type <= Bubble.eType.SnakeBlack)
					{
						fieldBubble.myTrans.localPosition += Vector3.forward * 46f;
					}
					break;
				case Bubble.eType.ChameleonRed:
					if (fieldBubble.type >= Bubble.eType.ChameleonRed && fieldBubble.type <= Bubble.eType.ChameleonBlack)
					{
						fieldBubble.myTrans.localPosition += Vector3.forward * 46f;
					}
					break;
				case Bubble.eType.BlackHole_A:
					if (fieldBubble.type == Bubble.eType.BlackHole_A || fieldBubble.type == Bubble.eType.BlackHole_B)
					{
						fieldBubble.myTrans.localPosition += Vector3.forward * 46f;
					}
					break;
				case Bubble.eType.CounterRed:
					if (fieldBubble.type >= Bubble.eType.CounterRed && fieldBubble.type <= Bubble.eType.CounterBlack)
					{
						fieldBubble.myTrans.localPosition += Vector3.forward * 46f;
					}
					break;
				case Bubble.eType.MorganaRed:
					if (fieldBubble.type >= Bubble.eType.MorganaRed && fieldBubble.type <= Bubble.eType.MorganaBlack)
					{
						foreach (Bubble item in MorganaList_)
						{
							item.myTrans.localPosition += Vector3.forward * 10f;
							if (item.myTrans.Find("chara_01_19(Clone)") != null)
							{
								item.myTrans.Find("chara_01_19(Clone)").transform.localPosition += Vector3.forward * 6f;
							}
						}
						break;
					}
					break;
				case Bubble.eType.TunnelIn:
					if (fieldBubble.type >= Bubble.eType.TunnelIn && fieldBubble.type <= Bubble.eType.TunnelOutRightDown)
					{
						fieldBubble.myTrans.localPosition += Vector3.forward * 46f;
					}
					break;
				case Bubble.eType.RotateFulcrumR:
					if (fieldBubble.type >= Bubble.eType.RotateFulcrumR && fieldBubble.type <= Bubble.eType.RotateFulcrumL)
					{
						fieldBubble.myTrans.localPosition += Vector3.forward * 46f;
					}
					break;
				case Bubble.eType.MinilenRed:
					if (fieldBubble.type >= Bubble.eType.MinilenRed && fieldBubble.type <= Bubble.eType.MinilenBlack)
					{
						fieldBubble.myTrans.localPosition += Vector3.forward * 46f;
					}
					break;
				default:
					if (fieldBubble.type == type)
					{
						fieldBubble.myTrans.localPosition += Vector3.forward * 46f;
					}
					break;
				}
			});
			if (type == Bubble.eType.ChainHorizontal || type == Bubble.eType.ChainLeftDown || type == Bubble.eType.ChainRightDown || type == Bubble.eType.ChainLock)
			{
				for (int i = 0; i < chainBubbleDic.Count; i++)
				{
					List<ChainBubble> list = chainBubbleDic[i];
					if (list.Count != 0)
					{
						for (int j = 0; j < list.Count; j++)
						{
							list[j].myTrans.localPosition += Vector3.forward * 46f;
						}
					}
				}
			}
		}
		switch ((GimmickBase.eGimmickType)TutorialManager.Instance.getHighlightGimmick(stageNo))
		{
		case GimmickBase.eGimmickType.Cloud:
			if (ivyList == null)
			{
				break;
			}
			{
				foreach (Ivy ivy in ivyList)
				{
					ivy.gameObject.transform.localPosition += Vector3.forward * 56f;
				}
				break;
			}
		case GimmickBase.eGimmickType.Egg:
			if (cloudList == null)
			{
				break;
			}
			{
				foreach (Cloud cloud in cloudList)
				{
					cloud.gameObject.transform.localPosition += Vector3.forward * 56f;
				}
				break;
			}
		case GimmickBase.eGimmickType.Max:
			if (obstacleList == null)
			{
				break;
			}
			{
				foreach (ObstacleDefend obstacle in obstacleList)
				{
					obstacle.gameObject.transform.localPosition += Vector3.forward * 90f;
				}
				break;
			}
		}
	}

	private void tutorialEndObjects(int spNum)
	{
		GimmickBase.eGimmickType highlightGimmick = (GimmickBase.eGimmickType)TutorialManager.Instance.getHighlightGimmick(spNum);
		if (highlightGimmick == GimmickBase.eGimmickType.Invalid)
		{
			return;
		}
		switch (highlightGimmick - 1)
		{
		case GimmickBase.eGimmickType.Ivy:
			if (ivyList == null)
			{
				break;
			}
			{
				foreach (Ivy ivy in ivyList)
				{
					ivy.gameObject.transform.localPosition += Vector3.forward * 56f;
				}
				break;
			}
		case GimmickBase.eGimmickType.Cloud:
			if (cloudList == null)
			{
				break;
			}
			{
				foreach (Cloud cloud in cloudList)
				{
					cloud.gameObject.transform.localPosition += Vector3.forward * 56f;
				}
				break;
			}
		}
	}

	public void updateChacknNum()
	{
		if (!(chacknNumLabel == null) && !stageInfo.IsMinilenDelete)
		{
			int num = int.Parse(chacknNumLabel.text.Replace("x", string.Empty)) - 1;
			chacknNumLabel.text = "x" + num;
			Transform transform = chacknNumLabel.transform.parent.parent.Find("effect");
			transform.gameObject.SetActive(true);
			UITweener[] componentsInChildren = transform.GetComponentsInChildren<UITweener>(true);
			UITweener[] array = componentsInChildren;
			foreach (UITweener uITweener in array)
			{
				uITweener.Reset();
				uITweener.Play(true);
			}
			if (num == 0)
			{
				setClearStamp(false, true);
			}
		}
	}

	public void addMinilenNum()
	{
		minilen_count_current = Mathf.Min(minilen_count_current + 1, minilen_count_all);
		minilen_count_pop++;
		minilen_count_pop_scored++;
	}

	private IEnumerator bubblePlusEffect()
	{
		GameObject obj = UnityEngine.Object.Instantiate(bubbleObject) as GameObject;
		obj.SetActive(true);
		if (gameType == eGameType.ShotCount)
		{
			obj.GetComponent<Bubble>().setType(Bubble.eType.PlusRed);
			yield return StartCoroutine(preCountUpEffect(obj));
			countUpEffect(bubblePlusNum);
		}
		else
		{
			obj.GetComponent<Bubble>().setType(Bubble.eType.Time);
			yield return StartCoroutine(preCountUpEffect(obj));
			countUpEffect(timePlusNum);
		}
	}

	private IEnumerator shotPlusEffect()
	{
		if (gameType == eGameType.ShotCount)
		{
			GameObject obj = null;
			if (bParkStage_)
			{
				obj = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "ParkBubblePlusEffect")) as GameObject;
				UISprite minilen_sp = obj.GetComponentInChildren<UISprite>();
				minilen_sp.spriteName = "UI_picturebook_mini_" + (Bridge.MinilenData.getCurrent().index % 10000).ToString("000");
				minilen_sp.MakePixelPerfect();
				Utility.setLayer(obj, "Default");
				obj.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
			}
			else
			{
				obj = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", charaNames[1])) as GameObject;
				obj.GetComponentInChildren<tk2dAnimatedSprite>().playAutomatically = false;
				obj.GetComponentInChildren<tk2dAnimatedSprite>().StopAndResetFrame();
				obj.GetComponentInChildren<tk2dAnimatedSprite>().SetSprite(charaNames[1] + "_07_000");
				Utility.setLayer(obj, "Default");
				obj.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
			}
			yield return StartCoroutine(preCountUpEffect(obj));
			bubblePlusNum += shotPlusNum;
			prevValue = 0;
			countUpEffect(shotPlusNum);
		}
	}

	private IEnumerator CreaterSkillEff(int stock)
	{
		while (state == eState.Scroll || state == eState.Search)
		{
			yield return null;
		}
		if (state == eState.Clear)
		{
			yield break;
		}
		if (state != 0)
		{
			bMoveCreaterEffect = true;
			if (gameType == eGameType.Time)
			{
				moveCreaterDiffTime = Time.time - startTime;
			}
		}
		string spriteAnimName = string.Empty;
		if (createrBubbleType == Bubble.eType.Bomb)
		{
			spriteAnimName = "bubble_42";
		}
		else if (createrBubbleType == Bubble.eType.Metal)
		{
			spriteAnimName = "bubble_56";
		}
		else if (createrBubbleType == Bubble.eType.Water)
		{
			spriteAnimName = "bubble_75";
		}
		GameObject obj = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "spr_bubble")) as GameObject;
		obj.GetComponentInChildren<tk2dAnimatedSprite>().Play(spriteAnimName);
		Utility.setLayer(obj, "Default");
		obj.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
		Utility.setParent(obj, skillButton.parent, false);
		Vector3 moveEndPos = skillButton.transform.localPosition;
		Vector3 moveStartPos = moveEndPos + Vector3.up * 220f;
		obj.transform.localPosition = moveStartPos;
		iTween.MoveTo(obj, iTween.Hash("position", moveEndPos -= Vector3.forward * 5f, "easetype", iTween.EaseType.easeInOutQuad, "time", 0.5f, "islocal", true));
		iTween.ScaleTo(obj, iTween.Hash("x", 0.5f, "y", 0.5f, "easetype", iTween.EaseType.easeInOutQuad, "time", 0.5f, "islocal", true));
		iTween itween = obj.GetComponent<iTween>();
		while (itween != null)
		{
			yield return null;
		}
		UnityEngine.Object.Destroy(obj);
		GameObject bomb_eff = skillButton.Find("bomb_eff").gameObject;
		bomb_eff.SetActive(true);
		Vector3 eff_scale = bomb_eff.transform.localScale;
		bomb_eff.GetComponent<TweenScale>().Reset();
		bomb_eff.GetComponent<TweenAlpha>().Reset();
		bomb_eff.GetComponent<TweenScale>().Play(true);
		bomb_eff.GetComponent<TweenAlpha>().Play(true);
		SkillButtonSetting(stock);
		Sound.Instance.playSe(Sound.eSe.SE_357_readygo);
		while (bomb_eff.GetComponent<TweenScale>().enabled)
		{
			yield return null;
		}
		bomb_eff.GetComponent<UISprite>().alpha = 1f;
		bomb_eff.SetActive(false);
		bomb_eff.transform.localScale = eff_scale;
		if (state != 0)
		{
			bMoveCreaterEffect = false;
		}
		setSkillButtonActive(true);
	}

	private void CreaterUiUpdate()
	{
		int num = CreaterTurnCount % CreaterExTurn;
		float sliderValue = (float)num / (float)CreaterExTurn;
		CreaterSkillSliderBar_.sliderValue = sliderValue;
		string message = MessageResource.Instance.getMessage(8824);
		message = MessageResource.Instance.castCtrlCode(message, 1, (CreaterExTurn - num).ToString());
		CreaterTurnCountDownLabel_.text = message;
	}

	private IEnumerator setSliderValue(UISlider slider, float targetValue, int speed)
	{
		float startValue = slider.sliderValue;
		float stepValue = Mathf.Abs(targetValue - startValue) / (float)speed;
		if (targetValue > startValue)
		{
			while (slider.sliderValue < targetValue)
			{
				slider.sliderValue += stepValue;
				yield return null;
			}
		}
		else
		{
			while (slider.sliderValue > targetValue && !(slider.sliderValue - stepValue < 0f))
			{
				slider.sliderValue -= stepValue;
				yield return null;
			}
		}
		slider.sliderValue = targetValue;
	}

	private void SkillButtonSetting(int stock)
	{
		if (bCreater)
		{
			if (stock == 0)
			{
				CreaterObj.GetComponent<Bubble>().isLocked = true;
				CreaterStockLabel_.gameObject.SetActive(false);
			}
			else
			{
				CreaterObj.GetComponent<Bubble>().isLocked = false;
				CreaterStockLabel_.gameObject.SetActive(true);
			}
			CreaterStockLabel_.text = "×" + stock;
		}
		else if (stock == 0)
		{
			skillButton.Find("numbers/num_01").gameObject.SetActive(false);
			skillButton.Find("numbers/num_10").gameObject.SetActive(false);
			skillButton.Find("numbers/num_01/01").GetComponent<UISprite>().spriteName = "game_score_number_" + stock.ToString("00");
		}
		else if (stock < 10)
		{
			skillButton.Find("numbers/num_01").gameObject.SetActive(true);
			skillButton.Find("numbers/num_10").gameObject.SetActive(false);
			skillButton.Find("numbers/num_01/01").GetComponent<UISprite>().spriteName = "game_score_number_" + stock.ToString("00");
		}
		else
		{
			skillButton.Find("numbers/num_01").gameObject.SetActive(false);
			skillButton.Find("numbers/num_10").gameObject.SetActive(true);
			skillButton.Find("numbers/num_10/01").GetComponent<UISprite>().spriteName = "game_score_number_" + (stock % 10).ToString("00");
			skillButton.Find("numbers/num_10/10").GetComponent<UISprite>().spriteName = "game_score_number_" + Mathf.Floor(stock / 10).ToString("00");
		}
	}

	private void setSkillButtonActive(bool isActive)
	{
		UIButton[] components = skillButton.GetComponents<UIButton>();
		UIButton[] array = components;
		foreach (UIButton uIButton in array)
		{
			if (uIButton.tweenTarget.name == "bg" && bCueBubbleChange && CueBubbleChangeRestNum > 0 && state != 0)
			{
				uIButton.disabledColor = Color.white;
			}
			else if (bCueBubbleChange)
			{
				uIButton.disabledColor = Color.gray;
			}
			if (isActive)
			{
				if ((bCueBubbleChange && CueBubbleChangeRestNum > 0) || (bCreater && CreaterStockNum > 0))
				{
					NGUIUtility.enable(uIButton, false);
				}
			}
			else
			{
				NGUIUtility.disable(uIButton, false);
			}
		}
		if (bCreater && CreaterStockNum > 0)
		{
			skillButton.Find("bg_add").gameObject.SetActive(true);
		}
		else
		{
			skillButton.Find("bg_add").gameObject.SetActive(false);
		}
		if (bCueBubbleChange && CueBubbleChangeRestNum > 0)
		{
			skillButton.Find("bg_add").gameObject.SetActive(!skillNgIcon_.activeInHierarchy);
		}
		else
		{
			skillButton.Find("bg_add").gameObject.SetActive(false);
		}
	}

	private IEnumerator helpEffect()
	{
		GameData gameData = GlobalData.Instance.getGameData();
		long[] array = helpDataList;
		foreach (long id in array)
		{
			GameObject lineFriendIcon = UnityEngine.Object.Instantiate(lineFriendBase) as GameObject;
			UITexture uiTexture = lineFriendIcon.transform.Find("Player_icon").GetComponent<UITexture>();
			uiTexture.material = UnityEngine.Object.Instantiate(uiTexture.material) as Material;
			UserData[] dummyFriends = DummyPlayFriendData.DummyFriends;
			foreach (UserData friend in dummyFriends)
			{
				if (friend.ID == id)
				{
					Texture texture = friend.Texture;
					if (texture != null)
					{
						uiTexture.mainTexture = texture;
					}
					else
					{
						uiTexture.mainTexture = defaultUserIconTexture;
					}
				}
			}
			Utility.setLayer(lineFriendIcon, "Default");
			lineFriendIcon.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
			lineFriendIcon.SetActive(true);
			yield return StartCoroutine(preCountUpEffect(lineFriendIcon));
			if (gameType == eGameType.Time)
			{
				timePlusNum += gameData.helpTime;
				countUpEffect(timePlusNum);
			}
			else
			{
				bubblePlusNum += gameData.helpMove;
				countUpEffect(gameData.helpMove);
			}
		}
	}

	private IEnumerator preCountUpEffect(GameObject obj)
	{
		Utility.setParent(obj, launchpad, false);
		Vector3 moveEndPos = countdown_eff.transform.localPosition;
		Vector3 moveStartPos = moveEndPos + Vector3.up * 220f;
		if (countUpEffectCount != 0)
		{
			moveStartPos = moveEndPos + Quaternion.Euler(0f, 0f, -10 + rand.Next(21)) * Vector3.up * 180f;
		}
		countUpEffectCount++;
		obj.transform.localPosition = moveStartPos;
		iTween.MoveTo(obj, iTween.Hash("position", moveEndPos, "easetype", iTween.EaseType.easeInOutQuad, "time", 0.7f, "islocal", true));
		iTween.ScaleTo(obj, iTween.Hash("x", 0.5f, "y", 0.5f, "easetype", iTween.EaseType.easeInOutQuad, "time", 0.7f, "islocal", true));
		iTween itween = obj.GetComponent<iTween>();
		while (itween != null)
		{
			yield return null;
		}
		UnityEngine.Object.Destroy(obj);
	}

	private void countUpEffect(int value)
	{
		if (gameType == eGameType.ShotCount)
		{
			friendBonusPop(eFriendBonus.CountPlus, value, scrollUi, new Vector3(-115f, -305f, 0f), countUpEffectCount * 2, true);
		}
		else if (gameType == eGameType.Time)
		{
			friendBonusPop(eFriendBonus.CountPlus, value - prevValue, scrollUi, new Vector3(-115f, -305f, 0f), countUpEffectCount * 2, true);
		}
		Sound.Instance.playSe(Sound.eSe.SE_335_plusbubble);
		playCountdownEff(true);
		if (state < eState.Wait)
		{
			if (gameType == eGameType.ShotCount)
			{
				shotCount -= value;
			}
			else
			{
				startTime = Time.time + (float)value;
			}
		}
		else if (gameType == eGameType.ShotCount)
		{
			shotCount -= value;
		}
		else
		{
			stagePause.diffTime -= value;
		}
		prevValue = value;
		updateRestCountDisp(true);
	}

	private IEnumerator lastPoint()
	{
		if (!guide.isShootButton)
		{
			guide.setActive(false);
		}
		stagePause.pause = true;
		lastPoint_chackn = true;
		bool itemButtonEnable = itemParent_.isEnable();
		if (itemButtonEnable)
		{
			itemParent_.disable();
		}
		bShowedLastPoint = true;
		Transform last_point = frontUi.Find("last_point");
		last_point.gameObject.SetActive(true);
		Transform labelTrans = last_point.Find("count_label");
		UILabel label = labelTrans.GetComponent<UILabel>();
		if (!bLastPoint && !ResourceLoader.Instance.isJapanResource())
		{
			labelTrans.localPosition += Vector3.left * 50f;
		}
		int dispCount;
		if (gameType == eGameType.ShotCount)
		{
			dispCount = stageInfo.Move - shotCount;
			last_point.Find("time").gameObject.SetActive(false);
			last_point.Find("number").gameObject.SetActive(true);
		}
		else
		{
			dispCount = (int)((float)stageInfo.Time - (Time.time - startTime) + 0.9999f);
			last_point.Find("time").gameObject.SetActive(true);
			last_point.Find("number").gameObject.SetActive(false);
		}
		label.text = dispCount.ToString();
		TweenPosition last_pointAnim = last_point.GetComponent<TweenPosition>();
		last_pointAnim.Reset();
		last_pointAnim.Play(true);
		while (last_pointAnim.enabled)
		{
			yield return null;
		}
		last_point.gameObject.SetActive(false);
		bLastPoint = true;
		DialogBase dialogQuit = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
		while (dialogQuit.isOpen())
		{
			yield return null;
		}
		if (itemButtonEnable)
		{
			itemParent_.enable();
		}
		stagePause.pause = false;
		lastPoint_chackn = false;
	}

	private void setClearStamp(bool bScore, bool bEnable)
	{
		if (bScore)
		{
			if (!bEnable)
			{
				clear_condition_stamp_00.SetActive(false);
			}
			else if (!clear_condition_stamp_00.activeSelf)
			{
				clear_condition_stamp_00.SetActive(true);
				UITweener[] components = clear_condition_stamp_00.GetComponents<UITweener>();
				foreach (UITweener uITweener in components)
				{
					uITweener.Reset();
					uITweener.Play(true);
				}
			}
		}
		else if (!bEnable)
		{
			clear_condition_stamp_01.SetActive(false);
			if (chacknNumLabel != null)
			{
				chacknNumLabel.gameObject.SetActive(true);
			}
		}
		else if (!clear_condition_stamp_01.activeSelf)
		{
			clear_condition_stamp_01.SetActive(true);
			UITweener[] components2 = clear_condition_stamp_01.GetComponents<UITweener>();
			foreach (UITweener uITweener2 in components2)
			{
				uITweener2.Reset();
				uITweener2.Play(true);
			}
			if (chacknNumLabel != null)
			{
				chacknNumLabel.gameObject.SetActive(false);
			}
		}
	}

	public bool isSpecialBubble(Bubble.eType type)
	{
		switch (type)
		{
		case Bubble.eType.Hyper:
			return true;
		case Bubble.eType.Bomb:
			return true;
		case Bubble.eType.Shake:
			return true;
		case Bubble.eType.Metal:
			return true;
		case Bubble.eType.Ice:
			return true;
		case Bubble.eType.Fire:
			return true;
		case Bubble.eType.Water:
			return true;
		case Bubble.eType.Shine:
			return true;
		case Bubble.eType.LightningG_Item:
			return true;
		default:
			return false;
		}
	}

	private bool breakFrozen(Bubble shotBubble)
	{
		bool result = false;
		foreach (Bubble frozenBreak in frozenBreakList)
		{
			if (frozenBreak.state != Bubble.eState.Field)
			{
				continue;
			}
			switch (frozenBreak.type)
			{
			case Bubble.eType.Star:
				star(shotBubble);
				break;
			case Bubble.eType.Lightning:
			case Bubble.eType.LightningG:
				if (lightning(frozenBreak))
				{
					result = true;
				}
				break;
			case Bubble.eType.Search:
				result = true;
				break;
			case Bubble.eType.Time:
			case Bubble.eType.Coin:
				specialEffect(frozenBreak);
				break;
			case Bubble.eType.Grow:
			case Bubble.eType.Skull:
			case Bubble.eType.FriendRainbow:
			case Bubble.eType.FriendBox:
			case Bubble.eType.Box:
			case Bubble.eType.Rainbow:
			case Bubble.eType.Rock:
			case Bubble.eType.Honeycomb:
			case Bubble.eType.Counter:
			case Bubble.eType.BlackHole_A:
			case Bubble.eType.BlackHole_B:
			case Bubble.eType.MinilenRainbow:
				breakLightningCount_++;
				break;
			}
			if (Bubble.eType.ChameleonRed <= frozenBreak.type && Bubble.eType.Unknown >= frozenBreak.type)
			{
				breakLightningCount_++;
			}
			frozenBreak.startBreak();
			plusEffect(frozenBreak);
		}
		if (frozenBreakList.Count > 0)
		{
			Sound.Instance.playSe(Sound.eSe.SE_505_frozen_bubble_broken);
		}
		Debug.Log("<color=yellow> breakLightningCount_ = " + breakLightningCount_ + "</color>");
		return result;
	}

	public Ivy createIvy(Ivy.eType type, int row, int column)
	{
		GameObject gameObject = ((type != 0) ? (UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Object_16_L")) as GameObject) : (UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Object_16_R")) as GameObject));
		gameObject.transform.parent = bubbleRoot;
		gameObject.transform.localScale = Vector3.one;
		gameObject.GetComponent<Ivy>().setEditScale();
		int lineNum = stageData.lineNum;
		int num = 10;
		if (num > lineNum)
		{
			num = lineNum + 1;
		}
		float num2 = 52 * lineNum - 52 * num;
		gameObject.transform.localPosition = new Vector3(0f, (float)(column * -52) + num2, -2f);
		Ivy component = gameObject.GetComponent<Ivy>();
		component.ivyType_ = type;
		component.column_ = column;
		ivyList.Add(component);
		Vector3 vector = Vector3.zero;
		if (type == Ivy.eType.Right)
		{
			GameObject leftBubble = getLeftBubble(component);
			if (leftBubble != null)
			{
				vector = leftBubble.transform.localPosition;
				component.headBubblePos_ = new Vector3(leftBubble.transform.localPosition.x, leftBubble.transform.localPosition.y, leftBubble.transform.localPosition.z);
			}
		}
		else
		{
			GameObject rightBubble = getRightBubble(component);
			if (rightBubble != null)
			{
				vector = rightBubble.transform.localPosition;
				component.headBubblePos_ = new Vector3(rightBubble.transform.localPosition.x, rightBubble.transform.localPosition.y, rightBubble.transform.localPosition.z);
			}
		}
		component.setScale();
		setupRomPos(component, vector.x, 266f);
		return component;
	}

	public void setupRomPos(Ivy iv, float pos_x, float offset)
	{
		if (iv.ivyType_ == Ivy.eType.Left)
		{
			offset = 0f - offset;
		}
		iv.setIvyPos(pos_x + offset);
	}

	private GameObject getRightBubble(Ivy iv)
	{
		foreach (Bubble rightBubble in rightBubbleList)
		{
			if (rightBubble.myTrans.localPosition.y - 26f < iv.transform.localPosition.y && rightBubble.myTrans.localPosition.y + 26f > iv.transform.localPosition.y)
			{
				return rightBubble.gameObject;
			}
		}
		return null;
	}

	private GameObject getLeftBubble(Ivy iv)
	{
		foreach (Bubble leftBubble in leftBubbleList)
		{
			if (leftBubble.myTrans.localPosition.y - 26f < iv.transform.localPosition.y && leftBubble.myTrans.localPosition.y + 26f > iv.transform.localPosition.y)
			{
				return leftBubble.gameObject;
			}
		}
		return null;
	}

	private void updateShootCharacter(bool force)
	{
		if (snakeCount_ > 0)
		{
			if (arrow.charaIndex == 1 && !force)
			{
				return;
			}
			arrow.charaIndex = 1;
			arrow.bubblen = charaAnims[1];
			Vector3 localPosition = arrow.transform.localPosition;
			localPosition.x = charaObjs[1].transform.localPosition.x;
			arrow.transform.localPosition = localPosition;
			if (dropSnakeList.Count <= 0)
			{
				localPosition = nextBubblePoses[1].position;
				localPosition.y = nextBubblePoses[0].position.y;
				nextBubblePoses[1].position = localPosition;
				if (nextBubbles[1] != null)
				{
					nextBubbles[1].transform.position = nextBubblePoses[1].position;
				}
			}
			localPosition = charaObjs[1].transform.localPosition;
			localPosition.x = -158f;
			localPosition.z = -4f;
			charaObjs[1].transform.localPosition = localPosition;
			waitAnimName = CHARA_SPRITE_ANIMATION_HEADER[1] + "11_00_0";
			charaAnims[0].Play(CHARA_SPRITE_ANIMATION_HEADER[0] + "09");
			if (dropSnakeList.Count > 0)
			{
				charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "10");
			}
			else if (!charaAnims[1].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[1] + "04"))
			{
				arrow.updateWaitAnimationImmediate();
			}
			if (guide.isShootButton)
			{
				guide.lineUpdate();
			}
			localPosition = guide.shootBasePos;
			localPosition.x = charaObjs[1].transform.localPosition.x;
			guide.shootBasePos = localPosition;
		}
		else if (arrow.charaIndex != 0 || force)
		{
			arrow.charaIndex = 0;
			arrow.bubblen = charaAnims[0];
			Vector3 localPosition2 = arrow.transform.localPosition;
			localPosition2.x = charaObjs[0].transform.localPosition.x;
			arrow.transform.localPosition = localPosition2;
			nextBubblePoses[1].localPosition = new Vector3(nextBubblePoses[1].localPosition.x, nextBubbleBobllenBefor_Y, nextBubblePoses[1].localPosition.z);
			localPosition2 = charaObjs[1].transform.localPosition;
			localPosition2.x = -178f;
			localPosition2.z = -5f;
			charaObjs[1].transform.localPosition = localPosition2;
			waitAnimName = CHARA_SPRITE_ANIMATION_HEADER[0] + "08_02_0";
			if (!charaAnims[0].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[0] + "04"))
			{
				arrow.updateWaitAnimationImmediate();
			}
			charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "07");
			setNextTapBobblen(false);
			if (guide.isShootButton)
			{
				guide.lineUpdate();
			}
			guide.shootBasePos = Vector3.zero;
		}
	}

	public void updateSnakeImmediate()
	{
		updateSnakeCoutnerImmediate();
		int num = snakeCount_;
		if (num > 3)
		{
			num = 3;
		}
		for (int i = 0; i < 3; i++)
		{
			if (i < num && !snake_eff[i].activeSelf)
			{
				snake_eff[i].SetActive(true);
				if (i != 0)
				{
					Animation component = snake_eff[i].GetComponent<Animation>();
					Animation component2 = snake_eff[i - 1].GetComponent<Animation>();
					float num2 = component2["snake_anm_00"].time + 0.8f;
					if (component["snake_anm_00"].length < num2)
					{
						num2 -= component["snake_anm_00"].length;
					}
					component["snake_anm_00"].time = num2;
				}
			}
			else if (i >= num && snake_eff[i].activeSelf)
			{
				snake_eff[i].SetActive(false);
			}
		}
	}

	public void updateSnake()
	{
		updateSnakeCoutner();
		int num = snakeCount_;
		if (num >= 3)
		{
			num = 3;
			StartCoroutine(removeSnakeRoutine(2));
		}
		for (int i = 0; i < 3; i++)
		{
			if (i < num && !snake_eff[i].activeSelf)
			{
				StartCoroutine(fadeinSnakeRoutine(i));
			}
			else if (i >= num && snake_eff[i].activeSelf)
			{
				StartCoroutine(removeSnakeRoutine(i));
			}
		}
	}

	private IEnumerator removeSnakeRoutine(int num)
	{
		GameObject removeInst = UnityEngine.Object.Instantiate(snake_eff[num]) as GameObject;
		removeInst.transform.parent = snake_eff[num].transform.parent;
		removeInst.transform.localPosition = snake_eff[num].transform.localPosition;
		removeInst.transform.localScale = snake_eff[num].transform.localScale;
		snake_eff[num].SetActive(false);
		if (!Sound.Instance.isPlayingSe(Sound.eSe.SE_514_snake_egg_break))
		{
			Sound.Instance.playSe(Sound.eSe.SE_513_snake_move_loop);
		}
		Animation anim = removeInst.GetComponent<Animation>();
		anim.clip = anim["snake_anm_01"].clip;
		anim.Play();
		bool bFadeout = false;
		iTween.MoveTo(removeInst, iTween.Hash("x", 800f, "easetype", iTween.EaseType.linear, "time", 2.4f, "islocal", true));
		while (removeInst.GetComponent<iTween>() != null)
		{
			if (!bFadeout && removeInst.transform.localPosition.x > 180f)
			{
				bFadeout = true;
				StartCoroutine(fadeoutSnakeRoutine(removeInst));
			}
			yield return stagePause.sync();
		}
		UnityEngine.Object.DestroyImmediate(removeInst);
	}

	private IEnumerator fadeinSnakeRoutine(int num)
	{
		snake_eff[num].SetActive(true);
		UISprite sprite = snake_eff[num].transform.GetComponentInChildren<UISprite>();
		if (num != 0)
		{
			Animation anim = snake_eff[num].GetComponent<Animation>();
			Animation anim_base = snake_eff[num - 1].GetComponent<Animation>();
			float point = anim_base["snake_anm_00"].time + 0.8f;
			if (anim["snake_anm_00"].length < point)
			{
				point -= anim["snake_anm_00"].length;
			}
			anim["snake_anm_00"].time = point;
		}
		Color color = sprite.color;
		color.a = 0f;
		sprite.color = color;
		while (color.a < 1f)
		{
			color.a += Time.deltaTime / 2f;
			if (color.a > 1f)
			{
				color.a = 1f;
			}
			sprite.color = color;
			yield return stagePause.sync();
		}
	}

	private IEnumerator fadeoutSnakeRoutine(GameObject target)
	{
		UISprite sprite = target.GetComponentInChildren<UISprite>();
		if (sprite == null)
		{
			yield break;
		}
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
	}

	private void updateChangeBubbleBobblen()
	{
		int num = stageInfo.Move - shotCount;
		Bubble component = nextBubbles[1].GetComponent<Bubble>();
		if (isSpecialBubble(component.type))
		{
			setNextTapBobblen(false);
		}
		else if (gameType == eGameType.ShotCount)
		{
			if (nextBubbleCount > 2 && num >= 2)
			{
				setNextTapBobblen(true);
				return;
			}
			setNextTapBobblen(false);
			if (nextBubbles[2] != null && nextBubbles[2].activeSelf)
			{
				nextBubbles[2].SetActive(false);
			}
		}
		else if (nextBubbleCount > 2)
		{
			setNextTapBobblen(true);
		}
		else
		{
			setNextTapBobblen(false);
		}
	}

	private void updateSnakeCoutner()
	{
		if (snakeCount_ <= 0)
		{
			if (snakeCounter.activeSelf)
			{
				snakeCounter.SetActive(false);
			}
		}
		else
		{
			if (!snakeCounter.activeSelf)
			{
				snakeCounter.SetActive(true);
			}
			if (dropSnakeList.Count > 0 && state != eState.Clear && state != eState.Gameover)
			{
				StopCoroutine("snakeCounterAnimation");
				StartCoroutine("snakeCounterAnimation");
			}
		}
		if (dropSnakeList.Count <= 0)
		{
			snakeCounterLabel.text = snakeCount_.ToString("00");
		}
	}

	private void updateSnakeCoutnerImmediate()
	{
		if (snakeCount_ <= 0)
		{
			if (snakeCounter.activeSelf)
			{
				snakeCounter.SetActive(false);
			}
		}
		else if (!snakeCounter.activeSelf)
		{
			snakeCounter.SetActive(true);
		}
		snakeCounterAnm.Stop();
		snakeCounterLabel.text = snakeCount_.ToString("00");
	}

	private IEnumerator snakeCounterAnimation()
	{
		if (honeycomb_num > 0)
		{
			snakeCounterLabel.text = snakeCount_.ToString("00");
			yield break;
		}
		snakeAliveAnimation = true;
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "10");
		charaAnims[0].Play(CHARA_SPRITE_ANIMATION_HEADER[0] + "09");
		while (Sound.Instance.isPlayingSe(Sound.eSe.SE_514_snake_egg_break))
		{
			yield return stagePause.sync();
		}
		snakeCounterAnm.Play("snake_counter_anm_01");
		Sound.Instance.playSe(Sound.eSe.SE_512_snake_voice);
		while (snakeCounterAnm.IsPlaying("snake_counter_anm_01"))
		{
			yield return stagePause.sync();
		}
		snakeCounterLabel.text = snakeCount_.ToString("00");
		snakeCounterAnm.Play("snake_counter_anm_02");
		charaAnims[0].Play(CHARA_SPRITE_ANIMATION_HEADER[0] + "09");
		charaAnims[1].Play(waitAnimName);
		Bubble b = null;
		if (nextBubbles[1] != null)
		{
			b = nextBubbles[1].GetComponent<Bubble>();
		}
		Vector3 temp_pos = nextBubblePoses[1].position;
		temp_pos.y = nextBubblePoses[0].position.y;
		nextBubblePoses[1].position = temp_pos;
		if (b != null && b.state == Bubble.eState.Wait)
		{
			nextBubbles[1].transform.position = nextBubblePoses[1].position;
		}
		snakeAliveAnimation = false;
	}

	private void setNextBubblePosition()
	{
		for (int i = 0; i < nextBubbleCount; i++)
		{
			if (nextBubbles[i] != null)
			{
				nextBubbles[i].transform.position = nextBubblePoses[i].position;
				if (i == 0)
				{
					Vector3 localPosition = nextBubbles[i].transform.localPosition;
					localPosition.z = -7.9f;
					nextBubbles[i].transform.localPosition = localPosition;
				}
			}
		}
	}

	private void startCountdownCounters()
	{
		enableCounterList.Clear();
		bool flag = false;
		foreach (Bubble fieldBubble in fieldBubbleList)
		{
			if ((fieldBubble.type == Bubble.eType.Counter || (fieldBubble.type >= Bubble.eType.CounterRed && fieldBubble.type <= Bubble.eType.CounterBlack)) && fieldBubble.state == Bubble.eState.Field && !fieldBubble.isFrozen && !fieldBubble.isLocked && !fieldBubble.inCloud)
			{
				enableCounterList.Add(fieldBubble);
				bool flag2 = fieldBubble.startCountDown();
				if (!flag)
				{
					flag = flag2;
				}
			}
		}
		if (flag)
		{
			Sound.Instance.playSe(Sound.eSe.SE_520_gas_bubble_countdown);
		}
	}

	private bool isPlayingCounterAnim()
	{
		foreach (Bubble enableCounter in enableCounterList)
		{
			if (enableCounter.isPlayingCounterAnim())
			{
				return true;
			}
		}
		return false;
	}

	private bool isCounterOver()
	{
		bool flag = false;
		countOverBubbleList_.Clear();
		foreach (Bubble enableCounter in enableCounterList)
		{
			bool flag2 = enableCounter.isCountOver();
			if (flag2)
			{
				if (!flag)
				{
					flag = flag2;
				}
				countOverBubbleList_.Add(enableCounter);
				foreach (ReplayData replayData in replayDataList)
				{
					if (replayData.index == enableCounter.createIndex)
					{
						replayData.counterCount = 0;
						break;
					}
				}
			}
			enableCounter.checkCounterEnable();
		}
		return flag;
	}

	private IEnumerator countDownOverEff()
	{
		if (countOverBubbleList_.Count > 0)
		{
			Sound.Instance.playSe(Sound.eSe.SE_521_gas_bubble_smoke01);
			foreach (Bubble bubble in countOverBubbleList_)
			{
				StartCoroutine(bubble.countOverEffect());
			}
		}
		float startTime = Time.time;
		while (Time.time - startTime < 0.2f)
		{
			yield return stagePause.sync();
		}
		Sound.Instance.pauseBgm(true);
		Sound.Instance.playSe(Sound.eSe.SE_522_gas_bubble_smoke02);
		Color gasColor = Constant.bubbleColor[0];
		Bubble tempB = null;
		bool isColorTimer = false;
		foreach (Bubble b in countOverBubbleList_)
		{
			if (tempB == null)
			{
				tempB = b;
			}
			else if (tempB.transform.localPosition.y > b.transform.localPosition.y)
			{
				tempB = b;
			}
		}
		if (tempB.type >= Bubble.eType.CounterRed && tempB.type <= Bubble.eType.CounterBlack)
		{
			gasColor = Constant.bubbleColor[(int)(tempB.type - 100)];
			isColorTimer = true;
		}
		GameObject counter_eff_inst = UnityEngine.Object.Instantiate(counter_eff) as GameObject;
		counter_eff_inst.transform.parent = uiRoot.transform;
		counter_eff_inst.transform.localScale = Vector3.one;
		counter_eff_inst.transform.position = counter_eff.transform.position;
		counter_eff_inst.transform.localPosition += Vector3.back * 5f;
		counter_eff_inst.SetActive(true);
		UISprite[] sprites = counter_eff_inst.transform.Find("gas_root_00").GetComponentsInChildren<UISprite>();
		if (isColorTimer)
		{
			UISprite[] array = sprites;
			foreach (UISprite sp in array)
			{
				sp.color = gasColor;
			}
		}
		counter_eff_anm = counter_eff_inst.GetComponent<Animation>();
		while (counter_eff_anm.IsPlaying("Counter_eff_anm"))
		{
			yield return stagePause.sync();
		}
		counter_eff_inst.SetActive(false);
		Sound.Instance.pauseBgm(false);
	}

	private IEnumerator beeBarrierEffect()
	{
		BoostItemBase temp_item = useItemParent_.getItem(Constant.Item.eType.BeeBarrier);
		if (temp_item == null)
		{
			yield break;
		}
		bPlayingBeeBarrierEffect = true;
		if (gameType == eGameType.Time)
		{
			beeBarrierDiffTime = Time.time - startTime;
		}
		Vector3 temp_vec = Vector3.zero - temp_item.transform.position;
		temp_vec.z = 0f;
		float angle = Mathf.Atan2(temp_vec.y, temp_vec.x) * 57.29578f - 90f;
		beebarrier_eff_00.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
		beebarrier_eff_00.transform.position = temp_item.transform.position;
		Sound.Instance.playSe(Sound.eSe.SE_222_kakoumae);
		beebarrier_eff.SetActive(true);
		while (bee_barrier_anm.isPlaying)
		{
			yield return stagePause.sync();
		}
		foreach (Bubble b in honeycombBubbleList_)
		{
			if (!b.isFrozen)
			{
				b.setGrayScale(gameoverMat);
			}
		}
		bPlayingBeeBarrierEffect = false;
	}

	private IEnumerator timeStopEffect()
	{
		BoostItemBase temp_item = useItemParent_.getItem(Constant.Item.eType.TimeStop);
		if (!(temp_item == null))
		{
			bPlayingTimeStopEffect = true;
			if (gameType == eGameType.Time)
			{
				timeStopDiffTime = Time.time - startTime;
			}
			GameObject inst = UnityEngine.Object.Instantiate(timestop_eff) as GameObject;
			timestop = inst.GetComponent<TimeStop>();
			timestop.setup(stagePause);
			inst.transform.parent = timestop_eff.transform.parent;
			inst.transform.position = timestop_eff.transform.position;
			inst.transform.localScale = timestop_eff.transform.localScale;
			timestop_eff_00 = inst.transform.Find("timestop_eff_00").gameObject;
			time_stop_anm = inst.GetComponent<Animation>();
			Vector3 temp_vec = Vector3.zero - temp_item.transform.position;
			temp_vec.z = 0f;
			float angle = Mathf.Atan2(temp_vec.y, temp_vec.x) * 57.29578f - 90f;
			timestop_eff_00.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
			timestop_eff_00.transform.position = temp_item.transform.position;
			inst.SetActive(true);
			while (time_stop_anm.isPlaying)
			{
				yield return stagePause.sync();
			}
			Sound.Instance.stopBgm();
			setFieldBubbleTimeStop(true);
			updateTimeStopCount();
			bPlayingTimeStopEffect = false;
			UnityEngine.Object.DestroyImmediate(inst);
		}
	}

	private IEnumerator updateTimeStop(bool countdown, bool playEffect)
	{
		bTimeStopEffectEnded = true;
		itemTimeStopUseCheckEnable = bUsingTimeStop;
		if (!bUsingTimeStop)
		{
			yield break;
		}
		if (countdown)
		{
			Sound.Instance.playSe(Sound.eSe.SE_524_tokei_hari);
			timeStopCount--;
		}
		if (timeStopCount <= 0)
		{
			bUsingTimeStop = false;
			if (!checkClear(true))
			{
				bTimeStopEffectEnded = playEffect;
				if (playEffect)
				{
					yield return StartCoroutine(timeStopEndAnimation());
					GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
					playBGM(dataTable);
					setFieldBubbleTimeStop(false);
				}
			}
			else
			{
				bUsingTimeStop = false;
				setFieldBubbleTimeStop(false);
			}
		}
		if (playEffect)
		{
			updateTimeStopCount();
		}
	}

	private void checkTimeStopItemEnable()
	{
		if (bUsingTimeStop || !itemTimeStopUseCheckEnable)
		{
			return;
		}
		useItemList_.Remove(Constant.Item.eType.TimeStop);
		useItemParent_.setup();
		foreach (Constant.Item.eType item in useItemList_)
		{
			if (Constant.Item.IsAutoUse(item))
			{
				useItemParent_.setActive(item, -1);
			}
		}
		if (itemTimeStop_ != null)
		{
			itemTimeStop_.setStateFixed(false);
			itemTimeStop_.enable();
			itemTimeStop_.reset();
		}
	}

	private IEnumerator timeStopEndAnimation()
	{
		timestop_counter_label.gameObject.SetActive(false);
		yield return stagePause.sync();
		iTween.MoveTo(timestop_counter, iTween.Hash("y", 0, "x", 0, "easetype", iTween.EaseType.linear, "time", 0.5f));
		while (timestop_counter.GetComponent<iTween>() != null)
		{
			yield return stagePause.sync();
		}
		Vector3 temp_pos = timestop_counter.transform.localPosition;
		temp_pos.x = (temp_pos.y = 0f);
		timestop_counter.transform.localPosition = temp_pos;
		timestop_counter_label.gameObject.SetActive(true);
		timestop_counter.SetActive(false);
		Sound.Instance.playSe(Sound.eSe.SE_523_time_start);
		GameObject inst = UnityEngine.Object.Instantiate(timestop_eff) as GameObject;
		time_stop_anm = inst.GetComponent<Animation>();
		inst.transform.parent = timestop_eff.transform.parent;
		inst.transform.localScale = Vector3.one;
		inst.transform.localPosition = Vector3.zero;
		inst.SetActive(true);
		yield return stagePause.sync();
		time_stop_anm.Play("TimeStop_end_anm");
		while (time_stop_anm.IsPlaying("TimeStop_end_anm"))
		{
			yield return stagePause.sync();
		}
		UnityEngine.Object.DestroyImmediate(inst);
	}

	private void updateTimeStopCount()
	{
		if (bUsingTimeStop)
		{
			if (itemTimeStop_ != null && !timestop_counter.activeSelf)
			{
				timestop_counter.SetActive(true);
			}
			timestop_counter_label.text = timeStopCount.ToString();
		}
		else if (itemTimeStop_ != null && timestop_counter.activeSelf)
		{
			timestop_counter.SetActive(false);
		}
	}

	private void setFieldBubbleTimeStop(bool enable)
	{
		foreach (Bubble fieldBubble in fieldBubbleList)
		{
			if (fieldBubble.isFrozen || (fieldBubble.type != Bubble.eType.Grow && fieldBubble.type != Bubble.eType.Counter && (fieldBubble.type < Bubble.eType.CounterRed || fieldBubble.type > Bubble.eType.CounterBlack)))
			{
				continue;
			}
			if (enable)
			{
				if (fieldBubble.type >= Bubble.eType.CounterRed && fieldBubble.type <= Bubble.eType.CounterBlack && !fieldBubble.isOnChain && !fieldBubble.isLocked)
				{
					fieldBubble.counterCount.transform.Find("count/number_00").GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 0.5f);
					fieldBubble.counterCount.transform.Find("count/number_0").GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 0.5f);
					fieldBubble.counterCount.setGasEnable(false);
				}
				else if (fieldBubble.type < Bubble.eType.CounterRed || fieldBubble.type > Bubble.eType.CounterBlack || (!fieldBubble.isOnChain && !fieldBubble.isLocked))
				{
					if (fieldBubble.type != Bubble.eType.Grow)
					{
						fieldBubble.counterCount.transform.Find("count/number_00").GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 0.5f);
						fieldBubble.counterCount.transform.Find("count/number_0").GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 0.5f);
					}
					fieldBubble.setGrayScale(gameoverMat);
					fieldBubble.setTimeStop(enable);
				}
			}
			else if ((fieldBubble.type != Bubble.eType.Counter && (fieldBubble.type < Bubble.eType.CounterRed || fieldBubble.type > Bubble.eType.CounterBlack)) || fieldBubble.getCounterEnable())
			{
				fieldBubble.setTimeStop(enable);
				fieldBubble.resurrection(normalMat);
				if ((fieldBubble.type == Bubble.eType.Counter || (fieldBubble.type >= Bubble.eType.CounterRed && fieldBubble.type <= Bubble.eType.CounterBlack)) && !fieldBubble.isOnChain && !fieldBubble.isLocked)
				{
					fieldBubble.counterCount.transform.Find("count/number_00").GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 1f);
					fieldBubble.counterCount.transform.Find("count/number_0").GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 1f);
					fieldBubble.counterCount.setGasEnable(!fieldBubble.isOnChain && !fieldBubble.isLocked);
				}
				if ((fieldBubble.type == Bubble.eType.Counter || (fieldBubble.type >= Bubble.eType.CounterRed && fieldBubble.type <= Bubble.eType.CounterBlack)) && fieldBubble.inCloud)
				{
					fieldBubble.inCloud = false;
				}
			}
		}
		StartCoroutine(cloudUpdateCheck());
	}

	public int getTimeStopCount()
	{
		return timeStopCount;
	}

	public void setupCloud()
	{
		if (stageData.cloudArea != null && stageData.cloudArea.Length >= 1)
		{
			for (int i = 0; i < stageData.cloudArea.Length; i++)
			{
				createCloud(stageData.cloudArea[i]);
			}
		}
	}

	public void createCloud(int _area)
	{
		if (_area != 0)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Object_19_Cloud")) as GameObject;
			gameObject.transform.parent = frontUi;
			gameObject.transform.localScale = Vector3.one;
			Cloud component = gameObject.GetComponent<Cloud>();
			component.cloudInit(_area);
			gameObject.SetActive(false);
			cloudList.Add(component);
		}
	}

	private IEnumerator cloudUpdateCheck(bool bForce = false)
	{
		foreach (Bubble bubble in fieldBubbleList)
		{
			if (MorganaList_ != null && MorganaList_.Contains(bubble))
			{
				continue;
			}
			float bubblePosX2 = bubble.myTrans.localPosition.x - 270f;
			float bubblePosY2 = bubble.myTrans.localPosition.y + 455f;
			foreach (Cloud cloud2 in cloudList)
			{
				if (bubblePosX2 > cloud2.cloudMinX && bubblePosX2 < cloud2.cloudMaxX && bubblePosY2 > cloud2.cloudMinY && bubblePosY2 < cloud2.cloudMaxY && !cloud2.isCloudMove && !bForce)
				{
					bubble.inCloud = true;
					break;
				}
				bubble.inCloud = false;
			}
			if (bubble.type == Bubble.eType.Honeycomb && bubble.inCloud)
			{
				Transform beeTrans = bubble.myTrans.Find("bee_eff(Clone)");
				if (beeTrans != null)
				{
					beeTrans.gameObject.SetActive(false);
				}
			}
		}
		if (MorganaList_ == null || MorganaList_.Count <= 0)
		{
			yield break;
		}
		foreach (Bubble bubble2 in MorganaList_)
		{
			float bubblePosX = bubble2.myTrans.localPosition.x - 270f;
			float bubblePosY = bubble2.myTrans.localPosition.y + 455f;
			foreach (Cloud cloud in cloudList)
			{
				if (bubblePosX > cloud.cloudMinX && bubblePosX < cloud.cloudMaxX && bubblePosY > cloud.cloudMinY && bubblePosY < cloud.cloudMaxY && !cloud.isCloudMove && !bForce)
				{
					bubble2.inCloud = true;
					break;
				}
				bubble2.inCloud = false;
			}
		}
	}

	public Material getGameoverMat()
	{
		return gameoverMat;
	}

	private IEnumerator cloudMoveDiffCheck()
	{
		Cloud[] hitCloud = new Cloud[2];
		int diff2 = 0;
		int[] insertDiff = new int[2];
		int[] aroundCloudArea = new int[2];
		cloudList.Sort((Cloud a, Cloud b) => a.priority - b.priority);
		for (int i = 0; i < cloudList.Count; i++)
		{
			diff2 = cloudList[i].hitDiff;
			if (diff2 != 0)
			{
				insertDiff[i] = diff2;
				hitCloud[i] = cloudList[i];
			}
			else
			{
				insertDiff[i] = 0;
				hitCloud[i] = null;
			}
		}
		for (int j = 0; j < hitCloud.Length; j++)
		{
			if (!(hitCloud[j] != null))
			{
				continue;
			}
			for (int x9 = 0; x9 < cloudList.Count; x9++)
			{
				aroundCloudArea[x9] = (int)cloudList[x9].getAreaState();
			}
			bool bBound = false;
			if (insertDiff[j] == 1)
			{
				hitCloud[j].moveDiff = 0;
				if (hitCloud[j].getAreaState() >= Cloud.CloudAreaState.LeftUp && hitCloud[j].getAreaState() <= Cloud.CloudAreaState.RightUp)
				{
					hitCloud[j].moveDiff = 1;
					bBound = true;
					for (int x8 = 0; x8 < aroundCloudArea.Length; x8++)
					{
						if (aroundCloudArea[x8] == (int)(hitCloud[j].getAreaState() + 3))
						{
							hitCloud[j].moveDiff = -1;
							break;
						}
					}
				}
				else
				{
					for (int x7 = 0; x7 < aroundCloudArea.Length; x7++)
					{
						if (aroundCloudArea[x7] != 0 && hitCloud[j].getAreaState() != (Cloud.CloudAreaState)aroundCloudArea[x7] && aroundCloudArea[x7] == (int)(hitCloud[j].getAreaState() - 3))
						{
							hitCloud[j].moveDiff = -1;
							break;
						}
					}
				}
			}
			else if (insertDiff[j] == 2)
			{
				hitCloud[j].moveDiff = 1;
				if (hitCloud[j].getAreaState() >= Cloud.CloudAreaState.LeftDown && hitCloud[j].getAreaState() <= Cloud.CloudAreaState.RightDown)
				{
					hitCloud[j].moveDiff = 0;
					bBound = true;
					for (int x6 = 0; x6 < aroundCloudArea.Length; x6++)
					{
						if (aroundCloudArea[x6] == (int)(hitCloud[j].getAreaState() - 3))
						{
							hitCloud[j].moveDiff = -1;
							break;
						}
					}
				}
				else
				{
					for (int x5 = 0; x5 < aroundCloudArea.Length; x5++)
					{
						if (aroundCloudArea[x5] != 0 && hitCloud[j].getAreaState() != (Cloud.CloudAreaState)aroundCloudArea[x5] && aroundCloudArea[x5] == (int)(hitCloud[j].getAreaState() + 3))
						{
							hitCloud[j].moveDiff = -1;
							break;
						}
					}
				}
			}
			else if (insertDiff[j] == 3)
			{
				hitCloud[j].moveDiff = 2;
				if (hitCloud[j].getAreaState() == Cloud.CloudAreaState.RightUp || hitCloud[j].getAreaState() == Cloud.CloudAreaState.RightDown)
				{
					hitCloud[j].moveDiff = 3;
					bBound = true;
					for (int x3 = 0; x3 < aroundCloudArea.Length; x3++)
					{
						if (aroundCloudArea[x3] == (int)(hitCloud[j].getAreaState() - 1) && (hitCloud[j].getAreaState() - 1 == Cloud.CloudAreaState.CenterUp || hitCloud[j].getAreaState() - 1 == Cloud.CloudAreaState.CenterDown))
						{
							hitCloud[j].moveDiff = -1;
							break;
						}
					}
				}
				else
				{
					for (int x4 = 0; x4 < aroundCloudArea.Length; x4++)
					{
						if (aroundCloudArea[x4] != 0 && hitCloud[j].getAreaState() != (Cloud.CloudAreaState)aroundCloudArea[x4] && aroundCloudArea[x4] == (int)(hitCloud[j].getAreaState() + 1))
						{
							if (hitCloud[j].getAreaState() + 1 == Cloud.CloudAreaState.CenterUp || hitCloud[j].getAreaState() + 1 == Cloud.CloudAreaState.CenterDown)
							{
								hitCloud[j].moveDiff = -1;
							}
							else
							{
								hitCloud[j].moveDiff = 3;
							}
							break;
						}
					}
				}
			}
			else if (insertDiff[j] == 4)
			{
				hitCloud[j].moveDiff = 3;
				if (hitCloud[j].getAreaState() == Cloud.CloudAreaState.LeftUp || hitCloud[j].getAreaState() == Cloud.CloudAreaState.LeftDown)
				{
					hitCloud[j].moveDiff = 2;
					bBound = true;
					for (int x = 0; x < aroundCloudArea.Length; x++)
					{
						if (aroundCloudArea[x] == (int)(hitCloud[j].getAreaState() + 1) && (hitCloud[j].getAreaState() + 1 == Cloud.CloudAreaState.CenterUp || hitCloud[j].getAreaState() + 1 == Cloud.CloudAreaState.CenterDown))
						{
							hitCloud[j].moveDiff = -1;
							break;
						}
					}
				}
				else
				{
					for (int x2 = 0; x2 < aroundCloudArea.Length; x2++)
					{
						if (aroundCloudArea[x2] != 0 && hitCloud[j].getAreaState() != (Cloud.CloudAreaState)aroundCloudArea[x2] && aroundCloudArea[x2] == (int)(hitCloud[j].getAreaState() - 1))
						{
							if (hitCloud[j].getAreaState() - 1 == Cloud.CloudAreaState.CenterUp || hitCloud[j].getAreaState() - 1 == Cloud.CloudAreaState.CenterDown)
							{
								hitCloud[j].moveDiff = -1;
							}
							else
							{
								hitCloud[j].moveDiff = 2;
							}
							break;
						}
					}
				}
			}
			if (hitCloud[j].moveDiff != -1)
			{
				hitCloud[j].isCloudMove = true;
				yield return StartCoroutine(cloudUpdateCheck());
				yield return StartCoroutine(hitCloud[j].cloudMoveRoutine(stagePause, bBound));
			}
		}
		yield return null;
	}

	private IEnumerator cloudClearAnimation()
	{
		if (cloudList == null)
		{
			yield break;
		}
		foreach (Cloud cloud2 in cloudList)
		{
			StartCoroutine(cloud2.clearAnimationPlay());
		}
		bool isPlaying = true;
		while (isPlaying)
		{
			yield return stagePause.sync();
			isPlaying = false;
			foreach (Cloud cloud in cloudList)
			{
				if (cloud.animationIsPlaying())
				{
					isPlaying = true;
					break;
				}
			}
		}
	}

	private IEnumerator vacuumEffect()
	{
		BoostItemBase temp_item = useItemParent_.getItem(Constant.Item.eType.Vacuum);
		if (temp_item == null)
		{
			yield break;
		}
		yield return StartCoroutine(cloudUpdateCheck(true));
		GameObject bubbleHold = launchpad.Find("next_hold").gameObject;
		charaObjs[1].SetActive(false);
		bubbleHold.SetActive(false);
		nextBubbles[1].SetActive(false);
		if (nextBubbles[2] != null)
		{
			nextBubbles[2].SetActive(false);
		}
		UISpriteAnimationEx animEX = vacuum_bob.transform.Find("bg_00").GetComponent<UISpriteAnimationEx>();
		vacuum_bob.GetComponent<Animation>().clip = vacuum_bob.GetComponent<Animation>()["Vacuum_chara_anm"].clip;
		animEX.SetClip("vacuum_start");
		vacuum_bob.SetActive(true);
		GameObject inst = UnityEngine.Object.Instantiate(vacuum_eff) as GameObject;
		Vector3 pos2 = Vector3.zero;
		inst.transform.parent = vacuum_eff.transform.parent;
		inst.transform.position = vacuum_eff.transform.position;
		inst.transform.localScale = vacuum_eff.transform.localScale;
		pos2 = inst.transform.position;
		vacuum_eff_00 = inst.transform.Find("vacuum_eff_00").gameObject;
		vacuum_anim = inst.GetComponent<Animation>();
		vacuum_eff_00.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		vacuum_eff_00.transform.position = temp_item.transform.position;
		vacuum_eff_01 = inst.transform.Find("vacuum_eff_01").gameObject;
		pos2.y -= 430f;
		vacuum_eff_01.transform.localPosition = pos2;
		Sound.Instance.playSe(Sound.eSe.SE_528_vacuum_light);
		UISprite vacume_sprite = vacuum_eff_01.transform.Find("eff_bg_2").GetComponent<UISprite>();
		inst.SetActive(true);
		bool bStartAppireSe = false;
		while (vacuum_anim.isPlaying)
		{
			if (!bStartAppireSe && vacume_sprite.color.a != 0f)
			{
				bStartAppireSe = true;
				Sound.Instance.playSe(Sound.eSe.SE_530_vacuum_move);
			}
			yield return stagePause.sync();
		}
		if (Sound.Instance.isPlayingSe(Sound.eSe.SE_530_vacuum_move))
		{
			Sound.Instance.stopSe(Sound.eSe.SE_530_vacuum_move);
		}
		float time = 0f;
		while (time < 0.5f)
		{
			time += Time.deltaTime;
			yield return 0;
		}
		UnityEngine.Object.DestroyImmediate(inst);
		yield return StartCoroutine(vacuumAnimation_START());
		vacuum_bob_01.gameObject.SetActive(true);
		if (guide.isShootButton)
		{
			guide.lineUpdate();
		}
	}

	private IEnumerator vacuumAnimation_START()
	{
		vacuum_bob.GetComponent<Animation>().clip = vacuum_bob.GetComponent<Animation>()["Vacuum_chara_end_anm"].clip;
		vacuum_bob.GetComponent<Animation>().Play();
		yield return 0;
		vacuum_bob_01.gameObject.SetActive(false);
		yield return stagePause.sync();
		UISpriteAnimationEx animEX = vacuum_bob.transform.Find("bg_02").GetComponent<UISpriteAnimationEx>();
		animEX.SetClip("vacuum_end");
		GameObject vacuumLocatorIn = vacuum_bob.transform.Find("vacuum_locator_in").gameObject;
		Sound.Instance.playSe(Sound.eSe.SE_529_vacuum);
		foreach (Cloud cloud2 in cloudList)
		{
			StartCoroutine(cloud2.vacuumAnimationPlay_IN(stagePause, vacuumLocatorIn.transform.localPosition));
		}
		bool tweenPlaying = true;
		while (tweenPlaying)
		{
			yield return stagePause.sync();
			tweenPlaying = false;
			foreach (Cloud cloud in cloudList)
			{
				if (cloud.tweenAnimationPlaying())
				{
					tweenPlaying = true;
					break;
				}
			}
		}
		float time = 0f;
		while (time < 1f)
		{
			time += Time.deltaTime;
			yield return stagePause.sync();
		}
		GameObject bubbleHold = launchpad.Find("next_hold").gameObject;
		charaObjs[1].SetActive(true);
		bubbleHold.SetActive(true);
		if (gameType == eGameType.Time || (gameType == eGameType.ShotCount && shotCount <= stageInfo.Move - 2) || arrow.charaIndex == 1)
		{
			nextBubbles[1].SetActive(true);
		}
		if ((gameType == eGameType.Time || (gameType == eGameType.ShotCount && shotCount <= stageInfo.Move - 3)) && nextBubbles[2] != null)
		{
			nextBubbles[2].SetActive(true);
		}
		while (vacuum_bob.GetComponent<Animation>().IsPlaying("Vacuum_chara_end_anm"))
		{
			yield return stagePause.sync();
		}
		vacuum_bob.SetActive(false);
		yield return StartCoroutine(cloudUpdateCheck());
		updateVacuumCount();
		yield return null;
	}

	private IEnumerator vacuumAnimation_END()
	{
		vacuum_counter_label.gameObject.SetActive(false);
		yield return stagePause.sync();
		iTween.MoveTo(vacuum_counter, iTween.Hash("y", -0.4f, "x", 0, "easetype", iTween.EaseType.linear, "time", 0.5f));
		while (vacuum_counter.GetComponent<iTween>() != null)
		{
			yield return stagePause.sync();
		}
		Vector3 temp_pos = vacuum_counter.transform.localPosition;
		temp_pos.x = (temp_pos.y = 0f);
		vacuum_counter.transform.localPosition = temp_pos;
		vacuum_counter_label.gameObject.SetActive(true);
		vacuum_counter.SetActive(false);
		yield return stagePause.sync();
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "10");
		charaAnims[0].Play(CHARA_SPRITE_ANIMATION_HEADER[0] + "09");
		Sound.Instance.playSe(Sound.eSe.SE_530_vacuum_move);
		GameObject inst = UnityEngine.Object.Instantiate(vacuum_eff) as GameObject;
		vacuum_anim = inst.GetComponent<Animation>();
		Vector3 instPos = inst.transform.localPosition;
		instPos.y -= 180f;
		yield return stagePause.sync();
		inst.transform.parent = timestop_eff.transform.parent;
		inst.transform.localScale = Vector3.one;
		inst.transform.localPosition = instPos;
		inst.SetActive(true);
		vacuum_anim.Play("Vacuum_end_anm");
		float time = 0f;
		while (time < 1f)
		{
			time += Time.deltaTime;
			yield return stagePause.sync();
		}
		if (Sound.Instance.isPlayingSe(Sound.eSe.SE_530_vacuum_move))
		{
			Sound.Instance.stopSe(Sound.eSe.SE_530_vacuum_move);
		}
		Sound.Instance.playSe(Sound.eSe.SE_531_back_cloud);
		foreach (Cloud cloud2 in cloudList)
		{
			StartCoroutine(cloud2.vacuumAnimationPlay_OUT(stagePause, instPos));
		}
		bool tweenPlaying = true;
		while (tweenPlaying)
		{
			yield return stagePause.sync();
			tweenPlaying = false;
			foreach (Cloud cloud in cloudList)
			{
				if (cloud.tweenAnimationPlaying())
				{
					tweenPlaying = true;
					break;
				}
			}
		}
		while (vacuum_anim.IsPlaying("Vacuum_end_anm"))
		{
			yield return stagePause.sync();
		}
		if (snakeCount_ > 0)
		{
			charaAnims[1].Play(waitAnimName);
		}
		else
		{
			waitAnimName = CHARA_SPRITE_ANIMATION_HEADER[0] + "08_02_0";
			charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "07");
			charaAnims[0].Play(waitAnimName);
		}
		UnityEngine.Object.DestroyImmediate(inst);
		yield return null;
	}

	private IEnumerator updateVacuum(bool countDown, bool playEffect)
	{
		bVacummeEffectEnded = true;
		itemVacummeUseCheckEnable = bUsingVacuum;
		if (!bUsingVacuum)
		{
			yield break;
		}
		if (countDown)
		{
			Sound.Instance.playSe(Sound.eSe.SE_524_tokei_hari);
			vacuumCount--;
		}
		if (vacuumCount <= 0)
		{
			if (!checkClear(true))
			{
				bVacummeEffectEnded = playEffect;
				if (playEffect)
				{
					yield return StartCoroutine(vacuumAnimation_END());
					yield return StartCoroutine(cloudUpdateCheck());
				}
				bUsingVacuum = false;
			}
			else
			{
				bUsingVacuum = false;
			}
		}
		if (playEffect)
		{
			updateVacuumCount();
		}
	}

	private void checkVacuumEnable()
	{
		if (bUsingVacuum || !itemVacummeUseCheckEnable)
		{
			return;
		}
		useItemList_.Remove(Constant.Item.eType.Vacuum);
		useItemParent_.setup();
		foreach (Constant.Item.eType item in useItemList_)
		{
			if (Constant.Item.IsAutoUse(item))
			{
				useItemParent_.setActive(item, -1);
			}
		}
		if (itemVacuum_ != null)
		{
			itemVacuum_.setStateFixed(false);
			itemVacuum_.enable();
			itemVacuum_.reset();
		}
	}

	private void updateVacuumCount()
	{
		if (bUsingVacuum)
		{
			if (itemVacuum_ != null && !vacuum_counter.activeSelf)
			{
				vacuum_counter.SetActive(true);
			}
			vacuum_counter_label.text = vacuumCount.ToString();
		}
		else if (itemVacuum_ != null && vacuum_counter.activeSelf)
		{
			vacuum_counter.SetActive(false);
		}
	}

	private void createChamelleonPickUpList()
	{
		chamelleonPickUpTypeList.Clear();
		chamelleonPrePickUpTypeList.Clear();
		foreach (Bubble fieldBubble in fieldBubbleList)
		{
			if (fieldBubble.state != Bubble.eState.Field || fieldBubble.isFrozen || fieldBubble.isLocked || fieldBubble.inCloud || !isColorBubble((int)fieldBubble.type))
			{
				continue;
			}
			Bubble.eType item = convertColorBubble(fieldBubble.type);
			if (fieldBubble.myTrans.localPosition.y > ceilingBaseY + 1f)
			{
				if (!chamelleonPrePickUpTypeList.Contains(item))
				{
					chamelleonPrePickUpTypeList.Add(item);
				}
			}
			else if (!chamelleonPickUpTypeList.Contains(item))
			{
				chamelleonPickUpTypeList.Add(item);
			}
		}
	}

	private void chameleonFieldBubbleColorCheck()
	{
		bubbleColorList.Clear();
		createChamelleonPickUpList();
		if (chamelleonPickUpTypeList.Count >= 1)
		{
			foreach (Bubble.eType chamelleonPickUpType in chamelleonPickUpTypeList)
			{
				bubbleColorList.Add((int)chamelleonPickUpType);
			}
		}
		else
		{
			foreach (Bubble.eType chamelleonPrePickUpType in chamelleonPrePickUpTypeList)
			{
				bubbleColorList.Add((int)chamelleonPrePickUpType);
			}
		}
		if (bubbleColorList.Count == 0)
		{
			bubbleColorList.Add(0);
		}
		bubbleColorList.Sort();
		List<int> list = new List<int>(bubbleColorList);
		chameleonChangeDic.Clear();
		for (int i = 0; i < chamelleonIndexCount + 1; i++)
		{
			int num = 0;
			foreach (Bubble chameleonBubble in chameleonBubbleList)
			{
				if (chameleonBubble.chamelleonIndex == i && !chameleonBubble.isLocked && !chameleonBubble.inCloud && !chameleonBubble.isFrozen)
				{
					num = ((chameleonBubble.type != Bubble.eType.Unknown) ? ((int)convertColorBubbleFixed(chameleonBubble.type)) : chameleonBubble.unknownColor);
					break;
				}
			}
			do
			{
				num++;
				if (num > 7)
				{
					num = 0;
				}
			}
			while (!list.Contains(num));
			chameleonChangeDic.Add(i, num);
		}
	}

	private void MorganaFieldBubbleColorCheck(Bubble hitBubble)
	{
		bubbleColorList.Clear();
		createChamelleonPickUpList();
		if (chamelleonPickUpTypeList.Count >= 1)
		{
			foreach (Bubble.eType chamelleonPickUpType in chamelleonPickUpTypeList)
			{
				bubbleColorList.Add((int)chamelleonPickUpType);
			}
		}
		else
		{
			foreach (Bubble.eType chamelleonPrePickUpType in chamelleonPrePickUpTypeList)
			{
				bubbleColorList.Add((int)chamelleonPrePickUpType);
			}
		}
		if (bubbleColorList.Count == 0)
		{
			bubbleColorList.Add(0);
		}
		bubbleColorList.Sort();
		List<int> list = new List<int>(bubbleColorList);
		int num = (int)(hitBubble.type - 109);
		do
		{
			num++;
			if (num > 7)
			{
				num = 0;
			}
		}
		while (!list.Contains(num));
		StartCoroutine(hitBubble.ChangeColor(num));
	}

	private bool checkChameleon(Bubble.eType _type)
	{
		switch (_type)
		{
		case Bubble.eType.Unknown:
			return true;
		case Bubble.eType.ChameleonRed:
		case Bubble.eType.ChameleonGreen:
		case Bubble.eType.ChameleonBlue:
		case Bubble.eType.ChameleonYellow:
		case Bubble.eType.ChameleonOrange:
		case Bubble.eType.ChameleonPurple:
		case Bubble.eType.ChameleonWhite:
		case Bubble.eType.ChameleonBlack:
			return true;
		default:
			return false;
		}
	}

	private IEnumerator setupChameleon()
	{
		foreach (Bubble b in chameleonBubbleList)
		{
			if (b.type != Bubble.eType.Unknown)
			{
				b.setType(b.type);
				b.GetComponentInChildren<tk2dAnimatedSprite>().Stop();
			}
		}
		yield break;
	}

	private IEnumerator updateChameleon()
	{
		if (bUsingTimeStop)
		{
			yield break;
		}
		chameleonFieldBubbleColorCheck();
		List<Bubble> updateChamelleonList = new List<Bubble>();
		List<tk2dAnimatedSprite> changeAnms = new List<tk2dAnimatedSprite>();
		foreach (Bubble b2 in chameleonBubbleList)
		{
			if (b2.isFrozen || b2.isLocked || b2.inCloud)
			{
				continue;
			}
			updateChamelleonList.Add(b2);
			if (b2.type < Bubble.eType.ChameleonRed || b2.type > Bubble.eType.ChameleonBlack)
			{
				continue;
			}
			int type5 = 79;
			type5 += chameleonChangeDic[b2.chamelleonIndex];
			if (b2.type == (Bubble.eType)type5)
			{
				if (chamelleonIndexCount <= b2.chamelleonIndex + 1)
				{
					b2.chamelleonIndex = 0;
				}
				else
				{
					b2.chamelleonIndex++;
				}
				type5 = 79 + chameleonChangeDic[b2.chamelleonIndex];
				if (b2.type == (Bubble.eType)type5)
				{
					continue;
				}
			}
			tk2dAnimatedSprite anm = b2.GetComponentInChildren<tk2dAnimatedSprite>();
			changeAnms.Add(anm);
			anm.PlayFromFrame(0);
		}
		bool bAnimationg = true;
		while (bAnimationg && changeAnms.Count != 0)
		{
			foreach (tk2dAnimatedSprite sp in changeAnms)
			{
				bAnimationg = sp.IsPlaying(sp.CurrentClip);
				if (bAnimationg)
				{
					break;
				}
			}
			yield return stagePause.sync();
		}
		foreach (Bubble b in updateChamelleonList)
		{
			if (b.type == Bubble.eType.Unknown)
			{
				b.unknownColor = chameleonChangeDic[b.chamelleonIndex];
				continue;
			}
			int type2 = 79;
			type2 += chameleonChangeDic[b.chamelleonIndex];
			if (b.type != (Bubble.eType)type2)
			{
				b.setType((Bubble.eType)type2);
				b.GetComponentInChildren<tk2dAnimatedSprite>().Stop();
			}
		}
		chameleonChangeDic.Clear();
	}

	private IEnumerator changeChameleon(List<Bubble> list)
	{
		Bubble.eType baseType = Bubble.eType.Red;
		bool animPlaying = true;
		bool isUnknown = false;
		bool isChameleon = false;
		foreach (Bubble bubble4 in list)
		{
			if (!(bubble4 == null) && bubble4.state == Bubble.eState.Field && !bubble4.isFrozen)
			{
				if (bubble4.type == Bubble.eType.Unknown)
				{
					isUnknown = true;
				}
				if (bubble4.type >= Bubble.eType.ChameleonRed && bubble4.type <= Bubble.eType.ChameleonBlack)
				{
					isChameleon = true;
				}
			}
		}
		if (isUnknown)
		{
			Sound.Instance.playSe(Sound.eSe.SE_533_unknown_bubble);
		}
		if (isChameleon)
		{
			Sound.Instance.playSe(Sound.eSe.SE_532_cameleon_bubble);
		}
		foreach (Bubble bubble3 in list)
		{
			if (!(bubble3 == null) && bubble3.state == Bubble.eState.Field && !bubble3.isFrozen)
			{
				if (bubble3.type == Bubble.eType.Unknown)
				{
					bubble3.GetComponentInChildren<tk2dAnimatedSprite>().Play("burst_unknown_00");
				}
				bubble3.GetComponentInChildren<tk2dAnimatedSprite>().Play();
			}
		}
		while (animPlaying)
		{
			animPlaying = false;
			foreach (Bubble bubble in list)
			{
				if (!(bubble == null) && bubble.state == Bubble.eState.Field && !bubble.isFrozen)
				{
					tk2dAnimatedSprite asp = bubble.GetComponentInChildren<tk2dAnimatedSprite>();
					asp.Resume();
					if (asp.isPlaying())
					{
						animPlaying = true;
						break;
					}
				}
			}
			yield return stagePause.sync();
		}
		foreach (Bubble bubble2 in list)
		{
			if (!(bubble2 == null) && bubble2.state == Bubble.eState.Field && !bubble2.isFrozen)
			{
				if (bubble2.type == Bubble.eType.Unknown)
				{
					Bubble.eType type2 = baseType + bubble2.unknownColor;
					bubble2.setType(type2);
				}
				else if (bubble2.type >= Bubble.eType.ChameleonRed && bubble2.type <= Bubble.eType.ChameleonBlack)
				{
					Bubble.eType type = (Bubble.eType)((int)baseType + (int)(bubble2.type - 79));
					bubble2.setType(type);
				}
				string text;
				if (bubble2.type > Bubble.eType.Blank)
				{
					int type3 = (int)bubble2.type;
					text = type3.ToString("000");
				}
				else
				{
					int type4 = (int)bubble2.type;
					text = type4.ToString("00");
				}
				bubble2.name = text;
				bubble2.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + bubble2.name);
			}
		}
		updateFieldBubbleList();
	}

	private void checkNextBubbleExistant(int index)
	{
		if (nextBubbles[index] == null)
		{
			return;
		}
		bool flag = false;
		Bubble.eType type = nextBubbles[index].GetComponent<Bubble>().type;
		if (isSpecialBubble(type))
		{
			return;
		}
		for (int i = 0; i < fieldBubbleList.Count; i++)
		{
			if (!fieldBubbleList[i].isFrozen && (!fieldBubbleList[i].isLocked || searchedBubbleTypeList.Count <= 0))
			{
				if (fieldBubbleList[i].isMorgana() && type == fieldBubbleList[i].type - 109)
				{
					flag = true;
					break;
				}
				if (type == convertColorBubble(fieldBubbleList[i].type))
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			UnityEngine.Object.DestroyImmediate(nextBubbles[index]);
			createNextBubble(index, false);
		}
	}

	private IEnumerator loadFriendProfTexture(int ID)
	{
		UserData data = null;
		if (DummyPlayFriendData.DummyFriends.Length > ID)
		{
			data = DummyPlayFriendData.DummyFriends[ID];
		}
		if (data == null || string.IsNullOrEmpty(data.URL))
		{
			yield break;
		}
		int failedCount = 0;
		WWW www;
		while (true)
		{
			www = new WWW(data.URL);
			while (!www.isDone && www.error == null)
			{
				yield return null;
			}
			if (www.error == null)
			{
				break;
			}
			www.Dispose();
			failedCount++;
			if (failedCount >= 3)
			{
				yield break;
			}
		}
		Texture2D texture = www.textureNonReadable;
		www.Dispose();
		if (data != null)
		{
			data.Texture = texture;
		}
	}

	private void AvatarSetup()
	{
		charaObjs[0].SetActive(false);
		CHARA_SPRITE_ANIMATION_HEADER[0] = charaObjs[0].name + "_";
		CHARA_SPRITE_ANIMATION_HEADER[1] = charaObjs[1].name + "_";
		arrow.CHARA_SPRITE_ANIMATION_HEADER = new string[2];
		arrow.CHARA_SPRITE_ANIMATION_HEADER[0] = CHARA_SPRITE_ANIMATION_HEADER[0];
		arrow.CHARA_SPRITE_ANIMATION_HEADER[1] = CHARA_SPRITE_ANIMATION_HEADER[1];
		waitAnimName = CHARA_SPRITE_ANIMATION_HEADER[0] + "08_02_0";
		arrow.charaAnimNames = new string[2]
		{
			CHARA_SPRITE_ANIMATION_HEADER[0] + "08_",
			CHARA_SPRITE_ANIMATION_HEADER[1] + "11_"
		};
		arrow.pinchAnimNames = new string[2]
		{
			CHARA_SPRITE_ANIMATION_HEADER[0] + "09_",
			CHARA_SPRITE_ANIMATION_HEADER[1] + "14_"
		};
	}

	private IEnumerator characterInstantiate()
	{
		Network.Avatar avatar = GlobalData.Instance.currentAvatar;
		if (bParkStage_)
		{
			avatar = GlobalData.Instance.defaultAvatar();
		}
		int throwChara = avatar.throwCharacter;
		int supportChara = avatar.supportCharacter;
		charaNames = new string[2]
		{
			"chara_00" + ((throwChara != 0) ? ("_" + (throwChara - 1).ToString("00")) : string.Empty),
			"chara_01" + ((supportChara != 0) ? ("_" + (supportChara - 1).ToString("00")) : string.Empty)
		};
		if (ResourceLoader.Instance.loadGameObject("Prefabs/", charaNames[0]) == null)
		{
			charaNames[0] = "chara_00";
		}
		if (ResourceLoader.Instance.loadGameObject("Prefabs/", charaNames[1]) == null)
		{
			charaNames[1] = "chara_01";
		}
		charaObjs = new GameObject[2];
		charaObjs[0] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", charaNames[0])) as GameObject;
		charaObjs[1] = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", charaNames[1])) as GameObject;
		Utility.setParent(charaObjs[0], stageUi, false);
		Utility.setParent(charaObjs[1], launchpad, false);
		charaObjs[0].name = charaObjs[0].name.Replace("(Clone)", string.Empty);
		charaObjs[1].name = charaObjs[1].name.Replace("(Clone)", string.Empty);
		Debug.Log("throwChara = " + charaNames[0] + " : supportChara = " + charaNames[1]);
		yield break;
	}

	private IEnumerator MoveObstacleDefend()
	{
		if (obstacleList == null || obstacleList.Count <= 0)
		{
			yield break;
		}
		List<ObstacleDefend> tempList = new List<ObstacleDefend>();
		obstacleList.ForEach(delegate(ObstacleDefend obj)
		{
			if (!(obj.currentParentBubble == null))
			{
				obj.UpdateNearBubbleList(fieldBubbleList);
				Bubble currentParentBubble = obj.currentParentBubble;
				if (!currentParentBubble.isFrozen && !currentParentBubble.isOnChain && !currentParentBubble.isLocked && !currentParentBubble.inCloud && obj.currentParentBubble.state != Bubble.eState.Drop && !(currentParentBubble.transform.localPosition.y > ceilingBaseY) && obj != null && obj.gameObject.activeSelf)
				{
					tempList.Add(obj);
				}
			}
		});
		if (tempList.Count <= 0)
		{
			yield break;
		}
		ObstacleDefend od = tempList[random.Next(tempList.Count)];
		if (od.currentParentBubble != null)
		{
			od.currentParentBubble.onObstacle = null;
			od.currentParentBubble = null;
		}
		od.DecideParentBubble(fieldBubbleList);
		if (od.currentParentBubble != null)
		{
			StartCoroutine(od.Move());
			yield return null;
			while (od.isMoving)
			{
				yield return stagePause.sync();
			}
			yield return StartCoroutine(od.AttackRoutine());
			updateFieldBubbleList();
			obstacleCount = 0;
		}
		else
		{
			obstacleMoveList.Add(od);
		}
	}

	private bool obstacleUpdate()
	{
		List<Bubble> tempList = new List<Bubble>();
		fieldBubbleList.ForEach(delegate(Bubble obj)
		{
			if (obj.type >= Bubble.eType.Red && obj.type <= Bubble.eType.Black && !obj.isLineFriend)
			{
				tempList.Add(obj);
			}
		});
		if (tempList.Count <= 1)
		{
			obstacleList.ForEach(delegate(ObstacleDefend obj)
			{
				if (obj.gameObject.activeSelf)
				{
					StartCoroutine(obj.breakRoutine(true));
					obj.currentParentBubble = null;
					StartCoroutine(obj.explosionRoutine());
				}
			});
			return true;
		}
		return false;
	}

	private void isLiveParentBubble()
	{
		if (obstacleMoveList == null || obstacleMoveList.Count <= 0)
		{
			return;
		}
		foreach (ObstacleDefend obstacleMove in obstacleMoveList)
		{
			if (obstacleMove.currentParentBubble == null)
			{
				obstacleMove.gameObject.SetActive(false);
			}
		}
		obstacleMoveList.Clear();
	}

	private IEnumerator obstacleWait()
	{
		if (obstacleMoveList.Count <= 0)
		{
			yield break;
		}
		while (true)
		{
			bool bWait = false;
			foreach (ObstacleDefend obj in obstacleMoveList)
			{
				if (obj.isMoving && !Sound.Instance.isPlayingSe(Sound.eSe.SE_601_obstacle_ufo_move))
				{
					Sound.Instance.playSe(Sound.eSe.SE_601_obstacle_ufo_move);
				}
				if (obj.isMoving || obj.anim.isPlaying)
				{
					bWait = true;
					break;
				}
			}
			if (!bWait)
			{
				break;
			}
			yield return 0;
		}
		if (Sound.Instance.isPlayingSe(Sound.eSe.SE_601_obstacle_ufo_move))
		{
			Sound.Instance.stopSe(Sound.eSe.SE_601_obstacle_ufo_move);
		}
		isLiveParentBubble();
	}

	private void obstacleCountup()
	{
		if (obstacleList != null && obstacleList.Count > 0)
		{
			obstacleCount++;
		}
	}

	private IEnumerator obstacleMoveRight()
	{
		if (obstacleMoveList == null || obstacleMoveList.Count <= 0)
		{
			yield break;
		}
		if (tayunCoroutine != null)
		{
			float elapsedTime = 0f;
			float waitTime = 0.8f;
			while (elapsedTime < waitTime)
			{
				elapsedTime += Time.deltaTime;
				yield return stagePause.sync();
			}
		}
		obstacleMoveList.Sort(delegate(ObstacleDefend obs1, ObstacleDefend obs2)
		{
			Vector3 position3 = obs1.transform.position;
			Vector3 position4 = obs2.transform.position;
			return position3.x.Equals(position4.x) ? ((!(position3.y > position4.y)) ? 1 : (-1)) : ((!(position3.x > position4.x)) ? 1 : (-1));
		});
		foreach (ObstacleDefend obstacle in obstacleMoveList)
		{
			bool isChangeParent = false;
			List<Bubble> parentCandidate = new List<Bubble>();
			foreach (Bubble fieldBubble2 in fieldBubbleList)
			{
				if (obstacle.isExplosion)
				{
					break;
				}
				if (!(fieldBubble2.onObstacle != null) && obstacle.transform.localPosition.y - 30f <= fieldBubble2.transform.localPosition.y && obstacle.transform.localPosition.y + 30f >= fieldBubble2.transform.localPosition.y && obstacle.transform.position.x < fieldBubble2.transform.position.x && fieldBubble2.type >= Bubble.eType.Red && fieldBubble2.type <= Bubble.eType.Black && !fieldBubble2.isFrozen && !fieldBubble2.isOnChain && !fieldBubble2.isLocked && !fieldBubble2.inCloud && !fieldBubble2.isLineFriend)
				{
					parentCandidate.Add(fieldBubble2);
				}
			}
			parentCandidate.Sort(delegate(Bubble b1, Bubble b2)
			{
				Vector3 position = b1.transform.position;
				Vector3 position2 = b2.transform.position;
				return (!(position.x < position2.x)) ? 1 : (-1);
			});
			using (List<Bubble>.Enumerator enumerator3 = parentCandidate.GetEnumerator())
			{
				if (enumerator3.MoveNext())
				{
					Bubble fieldBubble = enumerator3.Current;
					if (obstacle.currentParentBubble != null)
					{
						obstacle.currentParentBubble.onObstacle = null;
					}
					obstacle.currentParentBubble = fieldBubble;
					fieldBubble.onObstacle = obstacle;
					isChangeParent = true;
					parentCandidate.Clear();
				}
			}
			if (!isChangeParent && obstacle.currentParentBubble != null)
			{
				obstacle.currentParentBubble.onObstacle = null;
				obstacle.currentParentBubble = null;
			}
		}
		foreach (ObstacleDefend obj in obstacleMoveList)
		{
			if (obj.currentParentBubble == null || obj.isExplosion)
			{
				StartCoroutine(obj.explosionRoutine());
			}
			else
			{
				StartCoroutine(obj.MoveRight());
			}
		}
	}

	public void MorganaAnimation(GameObject charaObj)
	{
		ReplayDesList_.Add(charaObj);
		StartCoroutine(MorganaCharaAnimation(charaObj));
	}

	public IEnumerator MorganaCharaAnimation(GameObject charaObj)
	{
		charaObj.name = charaObj.name.Substring(0, charaObj.name.IndexOf("(Clone)"));
		tk2dAnimatedSprite anim = charaObj.GetComponentInChildren<tk2dAnimatedSprite>();
		float charaPosz = -8f;
		charaObj.transform.SetParent(charaObj.transform.parent.parent.parent, true);
		charaObj.transform.localPosition = new Vector3(charaObj.transform.localPosition.x, charaObj.transform.localPosition.y, charaPosz);
		charaObj.GetComponentInChildren<tk2dAnimatedSprite>().Stop();
		float time = 0.6f;
		while (0.2f < time)
		{
			time -= Time.deltaTime;
			yield return null;
		}
		charaObj.GetComponentInChildren<tk2dAnimatedSprite>().Play(charaObj.name + "_03");
		Sound.Instance.playSe(Sound.eSe.SE_603_invader_explosion);
		anim.Play(charaObj.name + "_03");
		while (0.01f < time)
		{
			time -= Time.deltaTime;
			yield return null;
		}
		while (0f < time && charaObj.activeInHierarchy)
		{
			charaObj.GetComponentInChildren<tk2dAnimatedSprite>().color = new Color(1f, 1f, 1f, time);
			time -= Time.deltaTime;
			yield return null;
		}
		UnityEngine.Object.Destroy(charaObj);
	}

	private void inOutObjectCountup()
	{
		List<Bubble> list = new List<Bubble>();
		inObjectCount = 0;
		outObjectCount = 0;
		foreach (Bubble fieldBubble in fieldBubbleList)
		{
			if (fieldBubble.type == Bubble.eType.TunnelIn || fieldBubble.type == Bubble.eType.TunnelNotIn)
			{
				if (fieldBubble.transform.localPosition.y <= ceilingBaseY && !fieldBubble.isOnChain && !fieldBubble.inCloud && !fieldBubble.isLocked)
				{
					inObjectCount++;
				}
				else if (fieldBubble.type != Bubble.eType.TunnelNotIn)
				{
					tk2dSpriteAnimationClip currentClip = fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
					if (currentClip.name != "bubble_121")
					{
						fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 121);
					}
					fieldBubble.name = "121";
					if (fieldBubble.myObject.transform.Find("warpInCollider") != null)
					{
						fieldBubble.myObject.transform.Find("warpInCollider").gameObject.name = "FulcrumCollider(Clone)";
					}
					fieldBubble.type = Bubble.eType.TunnelNotIn;
				}
			}
			if (fieldBubble.type < Bubble.eType.TunnelOutLeftUP || fieldBubble.type > Bubble.eType.TunnelOutRightDown)
			{
				continue;
			}
			if (fieldBubble.transform.localPosition.y <= ceilingBaseY && !fieldBubble.isOnChain && !fieldBubble.inCloud && !fieldBubble.isLocked)
			{
				outObjectCount++;
			}
			else if (fieldBubble.UseOutObject)
			{
				fieldBubble.UseOutObject = false;
				tk2dSpriteAnimationClip currentClip2 = fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
				if (currentClip2.name != "bubble_123")
				{
					fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 123);
				}
			}
		}
	}

	private void MoveOutObjectCheck()
	{
		foreach (Bubble fieldBubble in fieldBubbleList)
		{
			if (fieldBubble.type < Bubble.eType.TunnelOutLeftUP || fieldBubble.type > Bubble.eType.TunnelOutRightDown || !(fieldBubble.transform.localPosition.y <= ceilingBaseY))
			{
				continue;
			}
			bool useOutObject = fieldBubble.UseOutObject;
			if (inObjectCount <= 0)
			{
				if (fieldBubble.UseOutObject)
				{
					fieldBubble.UseOutObject = false;
					tk2dSpriteAnimationClip currentClip = fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
					if (currentClip.name != "bubble_123")
					{
						fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 123);
					}
				}
				continue;
			}
			if (fieldBubble.isOnChain || fieldBubble.inCloud || fieldBubble.isLocked)
			{
				fieldBubble.UseOutObject = false;
			}
			else
			{
				Vector3 vector = fieldBubble.transform.localPosition + fieldBubble.transform.Find("AS_spr_bubble").transform.up * Bubble.mHitSize;
				foreach (Bubble fieldBubble2 in fieldBubbleList)
				{
					if (!(fieldBubble == fieldBubble2))
					{
						float magnitude = (vector - fieldBubble2.transform.localPosition).magnitude;
						if (!(magnitude > Bubble.mHitSize))
						{
							fieldBubble.UseOutObject = false;
							break;
						}
						fieldBubble.UseOutObject = true;
					}
				}
			}
			if (fieldBubble.UseOutObject)
			{
				if (useOutObject != fieldBubble.UseOutObject)
				{
					tk2dSpriteAnimationClip currentClip2 = fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
					if (currentClip2.name != "bubble_122")
					{
						fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 122);
					}
				}
			}
			else if (useOutObject != fieldBubble.UseOutObject)
			{
				tk2dSpriteAnimationClip currentClip3 = fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
				if (currentClip3.name != "bubble_123")
				{
					fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 123);
				}
			}
		}
	}

	private void MoveOutObjectReplayCheck()
	{
		foreach (Bubble fieldBubble in fieldBubbleList)
		{
			if (fieldBubble.type >= Bubble.eType.TunnelOutLeftUP && fieldBubble.type <= Bubble.eType.TunnelOutRightDown)
			{
				if (fieldBubble.transform.localPosition.y <= ceilingBaseY)
				{
					fieldBubble.UseOutObject = false;
				}
				if (fieldBubble.isOnChain || fieldBubble.inCloud || fieldBubble.isLocked)
				{
					fieldBubble.UseOutObject = false;
				}
				else
				{
					Vector3 vector = fieldBubble.transform.localPosition + fieldBubble.transform.Find("AS_spr_bubble").transform.up * Bubble.mHitSize;
					foreach (Bubble fieldBubble2 in fieldBubbleList)
					{
						if (!(fieldBubble == fieldBubble2))
						{
							float magnitude = (vector - fieldBubble2.transform.localPosition).magnitude;
							if (!(magnitude > Bubble.mHitSize))
							{
								fieldBubble.UseOutObject = false;
								break;
							}
							fieldBubble.UseOutObject = true;
						}
					}
				}
				if (fieldBubble.UseOutObject)
				{
					tk2dSpriteAnimationClip currentClip = fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
					if (currentClip.name != "bubble_122")
					{
						fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 122);
					}
				}
				else
				{
					tk2dSpriteAnimationClip currentClip2 = fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
					if (currentClip2.name != "bubble_123")
					{
						fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 123);
					}
				}
			}
			if (fieldBubble.type != Bubble.eType.TunnelIn && fieldBubble.type != Bubble.eType.TunnelNotIn)
			{
				continue;
			}
			if (fieldBubble.transform.localPosition.y <= ceilingBaseY && !fieldBubble.isOnChain && !fieldBubble.inCloud && !fieldBubble.isLocked)
			{
				tk2dSpriteAnimationClip currentClip3 = fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
				if (currentClip3.name != "bubble_120")
				{
					fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 120);
				}
				fieldBubble.name = "120";
				if (fieldBubble.myObject.transform.Find("FulcrumCollider(Clone)") != null)
				{
					fieldBubble.myObject.transform.Find("FulcrumCollider(Clone)").gameObject.name = "warpInCollider";
				}
				fieldBubble.type = Bubble.eType.TunnelIn;
			}
			else if (fieldBubble.type != Bubble.eType.TunnelNotIn)
			{
				tk2dSpriteAnimationClip currentClip4 = fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
				if (currentClip4.name != "bubble_121")
				{
					fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 121);
				}
				fieldBubble.name = "121";
				if (fieldBubble.myObject.transform.Find("warpInCollider") != null)
				{
					fieldBubble.myObject.transform.Find("warpInCollider").gameObject.name = "FulcrumCollider(Clone)";
				}
				fieldBubble.type = Bubble.eType.TunnelNotIn;
			}
		}
	}

	private void MoveOutObjectDefend()
	{
		bool flag = false;
		if (outObjectCount <= 0)
		{
			foreach (Bubble fieldBubble in fieldBubbleList)
			{
				if (fieldBubble.transform.localPosition.y <= ceilingBaseY && fieldBubble.type == Bubble.eType.TunnelIn)
				{
					tk2dSpriteAnimationClip currentClip = fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
					if (currentClip.name != "bubble_121")
					{
						fieldBubble.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 121);
					}
					fieldBubble.name = "121";
					if (fieldBubble.myObject.transform.Find("warpInCollider") != null)
					{
						fieldBubble.myObject.transform.Find("warpInCollider").gameObject.name = "FulcrumCollider(Clone)";
					}
					fieldBubble.type = Bubble.eType.TunnelNotIn;
				}
			}
			flag = true;
		}
		if (inObjectCount <= 0)
		{
			foreach (Bubble fieldBubble2 in fieldBubbleList)
			{
				if (fieldBubble2.transform.localPosition.y <= ceilingBaseY && fieldBubble2.type >= Bubble.eType.TunnelOutLeftUP && fieldBubble2.type <= Bubble.eType.TunnelOutRightDown && fieldBubble2.UseOutObject)
				{
					tk2dSpriteAnimationClip currentClip2 = fieldBubble2.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
					if (currentClip2.name != "bubble_123")
					{
						fieldBubble2.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 123);
					}
					fieldBubble2.UseOutObject = false;
				}
			}
			flag = true;
		}
		if (flag)
		{
			return;
		}
		List<Bubble> list = new List<Bubble>();
		foreach (Bubble fieldBubble3 in fieldBubbleList)
		{
			if (fieldBubble3.transform.localPosition.y <= ceilingBaseY && fieldBubble3.type >= Bubble.eType.TunnelOutLeftUP && fieldBubble3.type <= Bubble.eType.TunnelOutRightDown && fieldBubble3.UseOutObject)
			{
				list.Add(fieldBubble3);
			}
		}
		if (list.Count <= 0)
		{
			foreach (Bubble fieldBubble4 in fieldBubbleList)
			{
				if (fieldBubble4.transform.localPosition.y <= ceilingBaseY && fieldBubble4.type == Bubble.eType.TunnelIn)
				{
					tk2dSpriteAnimationClip currentClip3 = fieldBubble4.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
					if (currentClip3.name != "bubble_121")
					{
						fieldBubble4.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 121);
					}
					if (fieldBubble4.myObject.transform.Find("warpInCollider") != null)
					{
						fieldBubble4.myObject.transform.Find("warpInCollider").gameObject.name = "FulcrumCollider(Clone)";
					}
					fieldBubble4.name = "121";
					fieldBubble4.type = Bubble.eType.TunnelNotIn;
				}
			}
			return;
		}
		Bubble bubble = list[random.Next(list.Count)];
		foreach (Bubble fieldBubble5 in fieldBubbleList)
		{
			if (fieldBubble5.transform.localPosition.y <= ceilingBaseY && fieldBubble5.type == Bubble.eType.TunnelNotIn && !fieldBubble5.isOnChain && !fieldBubble5.inCloud && !fieldBubble5.isLocked)
			{
				fieldBubble5.name = "120";
				fieldBubble5.type = Bubble.eType.TunnelIn;
				if (fieldBubble5.myObject.transform.Find("FulcrumCollider(Clone)") != null)
				{
					fieldBubble5.myObject.gameObject.transform.Find("FulcrumCollider(Clone)").gameObject.name = "warpInCollider";
				}
				tk2dSpriteAnimationClip currentClip4 = fieldBubble5.GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip;
				if (currentClip4.name != "bubble_120")
				{
					fieldBubble5.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + 120);
				}
			}
			if (fieldBubble5.type >= Bubble.eType.TunnelIn && fieldBubble5.type <= Bubble.eType.TunnelOutRightDown)
			{
				fieldBubble5.GetComponent<Bubble>().OldOutObject = fieldBubble5.GetComponent<Bubble>().OutObject;
				fieldBubble5.GetComponent<Bubble>().OutObject = bubble.GetComponent<Bubble>().myObject;
			}
		}
	}

	public void debugClear()
	{
		if (state == eState.Wait)
		{
			StartCoroutine(clearRoutine());
		}
	}

	public void debugGameover()
	{
		if (state == eState.Wait)
		{
			if (stageInfo.Move > 0)
			{
				gameoverType = eGameover.ShotCountOver;
			}
			else
			{
				gameoverType = eGameover.TimeOver;
			}
			StartCoroutine(gameoverRoutine());
		}
	}

	public void AddDropedMinilenInstanceId(int unique_id)
	{
		for (int i = 0; i < _minilen_bubble_unique_ids.Count; i++)
		{
			if (unique_id == _minilen_bubble_unique_ids[i] && !_droped_minilen_drops_indexes.Contains(i))
			{
				_droped_minilen_drops_indexes.Add(i);
			}
		}
	}

	public bool isHitMinilen(int unique_id)
	{
		if (_prak_minilen_drop_unique_id == unique_id)
		{
			return true;
		}
		return false;
	}

	public void updateMinilenCount()
	{
		if (!bParkStage_ || minilen_count_label == null)
		{
			return;
		}
		minilen_count_label.ForEach(delegate(UISprite mc)
		{
			mc.gameObject.SetActive(false);
		});
		int num = (int)Math.Log10(minilen_count_current) + 1;
		int num2 = num;
		while (num > minilen_count_label.Count)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(minilen_count_label[0].gameObject);
			Utility.setParent(gameObject, minilen_count_label[0].transform.parent, true);
			minilen_count_label.Add(gameObject.GetComponent<UISprite>());
			num2++;
		}
		int num3 = 0;
		int num4 = minilen_count_current;
		while (true)
		{
			int num5 = num4 % 10;
			minilen_count_label[num3].gameObject.SetActive(true);
			minilen_count_label[num3].spriteName = "game_score_number_" + num5.ToString("00");
			minilen_count_label[num3].MakePixelPerfect();
			if (num3 > 0)
			{
				Vector3 localPosition = minilen_count_label[num3 - 1].transform.localPosition;
				localPosition.x -= (minilen_count_label[num3 - 1].transform.localScale.x + minilen_count_label[num3].transform.localScale.x - 2f) * 0.5f;
				minilen_count_label[num3].transform.localPosition = localPosition;
			}
			num3++;
			if (num4 < 10)
			{
				break;
			}
			num4 /= 10;
		}
		int num6 = Math.Max(0, minilen_count_all - minilenCountCurrent);
		chacknNumLabel.text = "x" + num6;
		if (stageInfo.IsMinilenDelete && num6 <= 0)
		{
			setClearStamp(false, true);
		}
	}

	public void AddLightningG()
	{
		itemParent_.AddLightningG();
	}

	public void MinilenFireworks(Vector3 pos)
	{
		_minilen_firework_poss.Add(pos);
		if (!_minilen_fireworks_running)
		{
			StartCoroutine(MinilenFireworksRoutine());
		}
	}

	private IEnumerator MinilenFireworksRoutine()
	{
		_minilen_fireworks_running = true;
		int firework_color_randam = UnityEngine.Random.Range(0, 2);
		while (_minilen_firework_poss.Count > 0)
		{
			Vector3 pos = _minilen_firework_poss.ElementAt(0) + new Vector3(0.2f * (float)(firework_color_randam % 4 / 2) - 0.1f, 0.2f * (float)((firework_color_randam + 1) % 4 / 2) - 0.1f, 0f);
			_minilen_firework_poss.RemoveAt(0);
			GameObject prefab = ResourceLoader.Instance.loadGameObject("Prefabs/", "fireworks_" + (firework_color_randam % 3).ToString("00"));
			StartCoroutine(MinilenFirework_subroutine(pos, prefab));
			firework_color_randam++;
			yield return new WaitForSeconds(0.1f);
		}
		_minilen_fireworks_running = false;
	}

	private IEnumerator MinilenFirework_subroutine(Vector3 pos, GameObject prefab)
	{
		Sound.Instance.playSe(Sound.eSe.SE_225_fuusen);
		GameObject fireworks = UnityEngine.Object.Instantiate(prefab) as GameObject;
		fireworks.transform.parent = frontUi;
		fireworks.transform.localScale = Vector3.one;
		fireworks.transform.localPosition = pos;
		fireworks.transform.localPosition += Vector3.back;
		while (fireworks.GetComponent<TweenAlpha>().enabled)
		{
			yield return null;
		}
		while (fireworks.GetComponent<TweenScale>().enabled)
		{
			yield return null;
		}
		UnityEngine.Object.Destroy(fireworks);
	}

	public void vanishMinilen()
	{
		if (stageInfo.IsMinilenDelete)
		{
			gameoverType = eGameover.MinilenVanish;
		}
	}

	private IEnumerator resultSetup(bool bClear)
	{
		Debug.Log("result_setupa");
		for (int i = 0; i < 5; i++)
		{
			resultRewards_[i] = new Constant.Reward();
		}
		bClear_ = bClear;
		state = eState.Result;
		GameObject dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		RewardDataTable rewardTbl = dataTbl.GetComponent<RewardDataTable>();
		StageDataTable stageTbl = dataTbl.GetComponent<StageDataTable>();
		EventStageInfo.Info eventStageInfo = stageTbl.getEventInfo(stageNo, eventNo_);
		if (eventStageInfo != null)
		{
			for (int j = 0; j < eventStageInfo.Rewards.Length; j++)
			{
				rewards[j] = eventStageInfo.Rewards[j];
				rewardnum[j] = eventStageInfo.RewardNums[j];
			}
		}
		ComboBonusDataTable comboBonusTbl = dataTbl.GetComponent<ComboBonusDataTable>();
		comboBonusCoin = comboBonusTbl.getComboBonusCoinNum(maxComboCount);
		totalCoin += comboBonusCoin;
		otherData_ = SaveData.Instance.getGameData().getOtherData();
		Debug.Log("result_setupb");
		if (!bClear)
		{
			Sound.Instance.playSe(Sound.eSe.SE_109_OhNo);
			Sound.Instance.playBgm(Sound.eBgm.BGM_008_Over, false);
		}
		if (Bridge.StageData.isClear(stageNo))
		{
			prevScore_ = Bridge.StageData.getHighScore(stageNo);
		}
		if (!bParkStage_ && bEventStage_)
		{
			EventStageInfo eventData = stageTbl.getEventData();
			eventInfo_ = eventData.Infos[Constant.Event.convNoToLevel(stageNo, eventNo_) - 1];
		}
		int beforeStar = Bridge.StageData.getStar(stageNo);
		int beforeLv = Bridge.PlayerData.getLevel();
		bonusScore_ = getBonusScore(rewardTbl, beforeLv, totalScore);
		nowScore_ = totalScore + bonusScore_;
		Debug.Log("result_setupc");
		while (NetworkMng.Instance.isDownloading() || partManager.isLineLogin())
		{
			yield return null;
		}
		Debug.Log("result_setupd");
		partManager.isStageResultDownloading = true;
		yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
		int clearCountOffset = (bClear_ ? 1 : 0);
		int coin = 0;
		if (eventNo_ != 2)
		{
			coin = calcCoin(totalCoin, bClear_, stageNo, clearCountOffset);
		}
		else
		{
			if (bClear_)
			{
				coin = ((Bridge.StageData.getClearCount(stageNo) != 0) ? 700 : 1000);
			}
			coin += calcCoin(totalCoin, bClear_, stageNo, clearCountOffset);
		}
		Debug.Log("result_setupf");
		if (isCampaign_ && !bEventStage_)
		{
			campaignCoin = coin;
			int resultStar = 0;
			if (bClear_)
			{
				for (int k = stageInfo.StarScores.Length - 1; k >= 0; k--)
				{
					if (nowScore_ >= stageInfo.StarScores[k])
					{
						resultStar = k + 1;
						break;
					}
				}
			}
			if (resultStar > beforeStar && resultStar == Constant.StarMax && stageInfo.StarRewardNum > 0)
			{
				if (stageInfo.StarRewardType == 1)
				{
					campaignCoin += stageInfo.StarRewardNum;
				}
				bool bAreaEnd = false;
				int stageNum = stageTbl.GetComponent<StageIconDataTable>().getMaxStageIconsNum();
				if (stageNo == stageNum - 1)
				{
					bAreaEnd = true;
				}
				else
				{
					StageInfo.Info currentInfo = stageTbl.getInfo(stageNo);
					StageInfo.Info nextInfo = stageTbl.getInfo(stageNo + 1);
					if (nextInfo != null && currentInfo.Area < nextInfo.Area)
					{
						bAreaEnd = true;
					}
				}
				if (bAreaEnd)
				{
					campaignCoin += getResultCoinReward(bClear_, Bridge.StageData.getClearCount(stageNo) + clearCountOffset, resultStar);
				}
			}
			else
			{
				campaignCoin += getResultCoinReward(bClear_, Bridge.StageData.getClearCount(stageNo) + clearCountOffset, resultStar);
			}
			campaignCoin = Mathf.RoundToInt((float)campaignCoin * campaignRate_) - campaignCoin;
		}
		Debug.Log("result_setupg");
		if (bCoinUp)
		{
			skillBonusCoin = coin;
			int resultStar2 = 0;
			if (bClear_)
			{
				for (int l = stageInfo.StarScores.Length - 1; l >= 0; l--)
				{
					if (nowScore_ >= stageInfo.StarScores[l])
					{
						resultStar2 = l + 1;
						break;
					}
				}
			}
			if (resultStar2 > beforeStar && resultStar2 == Constant.StarMax && stageInfo.StarRewardNum > 0)
			{
				if (stageInfo.StarRewardType == 1)
				{
					skillBonusCoin += stageInfo.StarRewardNum;
				}
				bool bAreaEnd2 = false;
				int stageNum2 = stageTbl.GetComponent<StageIconDataTable>().getMaxStageIconsNum();
				if (stageNo == stageNum2 - 1)
				{
					bAreaEnd2 = true;
				}
				else
				{
					StageInfo.Info currentInfo2 = stageTbl.getInfo(stageNo);
					StageInfo.Info nextInfo2 = stageTbl.getInfo(stageNo + 1);
					if (nextInfo2 != null && currentInfo2.Area < nextInfo2.Area)
					{
						bAreaEnd2 = true;
					}
				}
				if (bAreaEnd2)
				{
					skillBonusCoin += getResultCoinReward(bClear_, Bridge.StageData.getClearCount(stageNo) + clearCountOffset, resultStar2);
				}
			}
			else
			{
				skillBonusCoin += getResultCoinReward(bClear_, Bridge.StageData.getClearCount(stageNo) + clearCountOffset, resultStar2);
			}
			skillBonusCoin = Mathf.RoundToInt((float)skillBonusCoin * CoinUpRate) - skillBonusCoin;
		}
		int totalcoin = coin + campaignCoin + bonusCoin + skillBonusCoin;
		if (bClear_)
		{
			totalcoin += totalEventCoin;
		}
		int keyCount = (bClear_ ? getKeyCount : 0);
		StringBuilder p14 = new StringBuilder();
		for (int n = 0; n < _droped_minilen_drops_indexes.Count; n++)
		{
			if (n > 0)
			{
				p14.Append(",");
			}
			if (_minilen_bubble_unique_ids[_droped_minilen_drops_indexes[n]] == _prak_minilen_drop_unique_id)
			{
				p14.Append("-");
			}
			p14.Append(_droped_minilen_drops_indexes[n].ToString());
		}
		Hashtable h = Hash.S2((!bEventStage_ && !bParkStage_) ? (stageNo + 1) : stageNo, (!bClear_) ? 2 : 3, (!bClear_) ? (nowScore_ - bonusScore_) : nowScore_, totalcoin, bonusJewel, stageEnd_ShotCount, (int)stageEnd_Time, stageEnd_ContinueCount, (bClear_ && missionType == 6) ? lineFriendCount : 0, (stageInfo.IsFriendDelete && bClear_ && missionType == 4) ? chackunCount : 0, (missionType == 5) ? useItemCount : 0, keyCount, minilen_count_current, _minilen_bubble_unique_ids.Count, p14.ToString());
		Debug.Log("result_setuph");
		NetworkMng.Instance.setup(h);
		yield return StartCoroutine(NetworkMng.Instance.download(API.S2, false, false));
		Debug.Log("result_setupi");
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Cancel)
		{
			Hashtable args = new Hashtable
			{
				{ "IsForceSendInactive", true },
				{ "StageNo", stageNo }
			};
			PartManager.ePart next_part = PartManager.ePart.Map;
			if (bParkStage_)
			{
				next_part = PartManager.ePart.Park;
			}
			else if (bEventStage_)
			{
				if (eventNo_ == 1)
				{
					next_part = PartManager.ePart.EventMap;
				}
				else if (eventNo_ == 2)
				{
					next_part = PartManager.ePart.ChallengeMap;
				}
				else if (eventNo_ == 11)
				{
					next_part = PartManager.ePart.CollaborationMap;
				}
			}
			partManager.requestTransition(next_part, args, true);
			partManager.isStageResultDownloading = false;
			yield break;
		}
		Debug.Log("result_setupj");
		WWW www = NetworkMng.Instance.getWWW();
		resultData_ = JsonMapper.ToObject<StageResult>(www.text);
		if (resultData_.beforeLv < resultData_.afterLv)
		{
			bLvUp_ = true;
		}
		if (DailyMission.isTermClear() || (bClear_ && stageNo == 5))
		{
			Mission respons_mission = JsonMapper.ToObject<Mission>(www.text);
			GlobalData.Instance.setDailyMissionData(respons_mission);
			Network.DailyMission dMission = GlobalData.Instance.getDailyMissionData();
			if (dMission == null)
			{
				NetworkMng.Instance.setup(Hash.DailyMissionCreate());
				yield return StartCoroutine(NetworkMng.Instance.download(API.DailyMissionCreate, false, false));
				WWW www_dMission = NetworkMng.Instance.getWWW();
				respons_mission = JsonMapper.ToObject<Mission>(www_dMission.text);
				GlobalData.Instance.setDailyMissionData(respons_mission);
				DailyMission.bMissionCreate = true;
			}
		}
		Debug.Log("result_setupk");
		GlobalData.Instance.setInviteRewardData(JsonMapper.ToObject<InviteRewardData>(www.text).inviteBasicReward);
		KeyBubbleData keyBubbleData = GlobalData.Instance.getKeyBubbleData();
		if (keyBubbleData != null && resultData_.keyBubble != null)
		{
			keyBubbleData.keyBubbleCount = resultData_.keyBubble.keyBubbleCount;
			GlobalData.Instance.setKeyBubbleData(keyBubbleData);
			bBossOpen = keyCount > 0 && keyBubbleData.keyBubbleMax <= keyBubbleData.keyBubbleCount;
		}
		Debug.Log("result_setupl");
		headerData_ = NetworkUtility.createResponceHeaderData(www);
		Debug.Log("result_setupm");
		GameData gameData = GlobalData.Instance.getGameData();
		Network.StageData stageData = GlobalData.Instance.getStageData(stageNo);
		gameData.maxLevel = resultData_.maxLevel;
		stageData.star = ((resultData_.star <= stageData.star) ? stageData.star : resultData_.star);
		stageData.clearCount = resultData_.clearCount;
		stageData.hiscore = resultData_.hiscore;
		gameData.allStageScoreSum = resultData_.allStageScoreSum;
		gameData.allStarSum = resultData_.allStarSum;
		gameData.allPlayCount = resultData_.allPlayCount;
		gameData.allClearCount = resultData_.allClearCount;
		gameData.bonusJewel = resultData_.bonusJewel;
		gameData.buyJewel = resultData_.buyJewel;
		gameData.level = resultData_.level;
		gameData.exp = resultData_.exp;
		gameData.coin = resultData_.coin;
		gameData.heart = resultData_.heart;
		gameData.treasureboxNum = resultData_.treasureboxNum;
		gameData.progressStageNo = resultData_.progressStageNo;
		gameData.progressStageOpen = resultData_.progressStageOpen;
		gameData.isAreaCampaign = resultData_.isAreaCampaign;
		gameData.areaSalePercent = resultData_.areaSalePercent;
		gameData.saleArea = resultData_.saleArea;
		gameData.saleStageItemArea = resultData_.saleStageItemArea;
		gameData.stageItemAreaSalePercent = resultData_.stageItemAreaSalePercent;
		gameData.isStageItemAreaCampaign = resultData_.isStageItemAreaCampaign;
		gameData.eventTimeSsRemaining = resultData_.eventTimeSsRemaining;
		gameData.eventMaxStageNo = resultData_.eventMaxStageNo;
		EventMenu.updateGetTime();
		ChallengeMenu.updateGetTime();
		CollaborationMenu.updateGetTime();
		gameData.gachaTicket = resultData_.gachaTicket;
		gameData.dailyMissionSsRemaining = resultData_.dailyMissionSsRemaining;
		DailyMission.updateGetTime();
		bProgressOpen = gameData.progressStageOpen;
		gameData.heartRecoverySsRemaining = resultData_.heartRecoverySsRemaining;
		gameData.mailUnReadCount = resultData_.mailUnReadCount;
		gameData.userItemList = resultData_.userItemList;
		gameData.minilenCount = resultData_.minilenCount;
		gameData.minilenTotalCount = resultData_.minilenTotalCount;
		gameData.giveNiceTotalCount = resultData_.giveNiceTotalCount;
		gameData.giveNiceMonthlyCount = resultData_.giveNiceMonthlyCount;
		gameData.tookNiceTotalCount = resultData_.tookNiceTotalCount;
		gameData.isParkDailyReward = resultData_.isParkDailyReward;
		gameData.minilenList = resultData_.minilenList;
		gameData.thanksList = resultData_.thanksList;
		if (bParkStage_)
		{
			_got_rare_minilen = resultData_.gotRareMinilenIndex;
			Network.MinilenData got_rare = Array.Find(gameData.minilenList, (Network.MinilenData m) => m.index == _got_rare_minilen);
			if (got_rare != null)
			{
				_got_rare_minilen_level = got_rare.level;
			}
			else
			{
				_got_rare_minilen = -1;
			}
		}
		Debug.Log("gameData.avatarList.Length = " + gameData.avatarList.Length);
		if (resultData_.avatarList != null)
		{
			Debug.Log("resultData_.avatarList.Length = " + resultData_.avatarList.Length);
			gameData.avatarList = resultData_.avatarList;
			int[] rank_count = new int[4];
			Network.Avatar[] avatarList = GlobalData.Instance.getGameData().avatarList;
			foreach (Network.Avatar av in avatarList)
			{
				if (av.index >= 23000)
				{
					rank_count[3]++;
				}
				else if (av.index >= 22000)
				{
					rank_count[2]++;
				}
				else if (av.index >= 21000)
				{
					rank_count[1]++;
				}
				else
				{
					rank_count[0]++;
				}
			}
			GlobalData.Instance.avatarCount = rank_count;
		}
		Debug.Log("result_setupn");
		MainMenu mainMenu = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		mainMenu.getHeartMenu().updateRemainingTime();
		if (!bEventStage_)
		{
			if (resultData_.clearCount == 1)
			{
				Tapjoy.SetUserLevel(stageNo + 1);
				Tapjoy.TrackEvent(string.Empty, "User Reaches Level X", "Stage No - " + (stageNo + 1), null, 0L);
				Tapjoy.SetUserLevel(gameData.progressStageNo);
				Tapjoy.TrackEvent(string.Empty, "Stage Clear", "Stage No - " + (stageNo + 1), null, 0L);
				GlobalGoogleAnalytics.Instance.LogEvent("Stage Clear", "Stage No - " + (stageNo + 1), "0", 1L);
				GameAnalytics.traceLevelUp(stageNo + 1);
			}
			Tapjoy.TrackEvent(string.Empty, "Stage Clear Total", "Stage No - " + (stageNo + 1), null, 0L);
			GlobalGoogleAnalytics.Instance.LogEvent("Stage Clear Total", "Stage No - " + (stageNo + 1), "0", 1L);
		}
		if (bEventStage_)
		{
			if (resultData_.clearCount == 1)
			{
				Tapjoy.TrackEvent(string.Empty, "Event Stage Clear", "Stage No - " + (stageNo + 1), null, 0L);
				GlobalGoogleAnalytics.Instance.LogEvent("Event Stage Clear", "Event Stage No - " + (stageNo + 1), "0", 1L);
			}
			Tapjoy.TrackEvent(string.Empty, "Event Stage Clear Total", "Stage No - " + (stageNo + 1), null, 0L);
			GlobalGoogleAnalytics.Instance.LogEvent("Event Stage Clear Total", "Stage No - " + (stageNo + 1), "0", 1L);
		}
		if (bParkStage_)
		{
			if (resultData_.clearCount == 1)
			{
				Tapjoy.TrackEvent(string.Empty, "Park Stage Clear", "Stage No - " + stageNo, null, 0L);
				GlobalGoogleAnalytics.Instance.LogEvent("Park Stage Clear", "Park Stage No - " + stageNo, "0", 1L);
			}
			Tapjoy.TrackEvent(string.Empty, "Park Stage Clear Total", "Stage No - " + stageNo, null, 0L);
			GlobalGoogleAnalytics.Instance.LogEvent("Park Stage Clear Total", "Stage No - " + stageNo, "0", 1L);
		}
		if (stageTbl.getUpdateInfo(headerData_) != 0)
		{
			Debug.Log("result_setupn1");
			yield return StartCoroutine(NetworkUtility.downloadPlayerData(false, false));
			EventMenu.updateGetTime();
			ChallengeMenu.updateGetTime();
			CollaborationMenu.updateGetTime();
			Debug.Log("result_setupn2");
			DailyMission.updateGetTime();
			Debug.Log("result_setupn3");
			yield return StartCoroutine(stageTbl.download(headerData_, false, false));
			Debug.Log("reward         :     " + rewards);
			Debug.Log("result_setupn4");
		}
		Debug.Log("result_setupo");
		StartCoroutine(NetworkMng.Instance.showIcon(false));
		partManager.isStageResultDownloading = false;
		while (NetworkMng.Instance.isShowIcon())
		{
			yield return null;
		}
		Debug.Log("result_setupp");
		bool bGainStarReward = false;
		if (eventNo_ == 2)
		{
			if (bClear)
			{
				resultRewards_[1].set(Constant.eMoney.Coin, getResultCoinReward_Challenge(bClear_, Bridge.StageData.getClearCount(stageNo)));
			}
		}
		else if (eventNo_ == 11)
		{
			if (bClear)
			{
				resultRewards_[1].set(Constant.eMoney.Coin, getResultCoinReward_Collaboration(bClear_, Bridge.StageData.getClearCount(stageNo)));
			}
		}
		else
		{
			if (resultData_.star > beforeStar && resultData_.star == Constant.StarMax && stageInfo.StarRewardNum > 0)
			{
				bGainStarReward = true;
			}
			if (bGainStarReward)
			{
				int rewardType = stageInfo.StarRewardType;
				if (rewardType == 2)
				{
					bGetStarJewel_ = true;
				}
				resultRewards_[rewardType].set((Constant.eMoney)rewardType, stageInfo.StarRewardNum);
				bool bAreaEnd3 = false;
				StageInfo.Info currentInfo4 = stageTbl.getInfo(stageNo);
				int stageNum4 = stageTbl.GetComponent<StageIconDataTable>().getMaxStageIconsNum();
				if (stageNo == stageNum4 - 1)
				{
					bAreaEnd3 = true;
				}
				else
				{
					StageInfo.Info nextInfo4 = stageTbl.getInfo(stageNo + 1);
					if (nextInfo4 != null && currentInfo4.Area < nextInfo4.Area)
					{
						bAreaEnd3 = true;
					}
				}
				if (bAreaEnd3)
				{
					resultRewards_[1].set(Constant.eMoney.Coin, getResultCoinReward(bClear_, Bridge.StageData.getClearCount(stageNo), resultData_.star));
				}
			}
			else
			{
				resultRewards_[1].set(Constant.eMoney.Coin, getResultCoinReward(bClear_, Bridge.StageData.getClearCount(stageNo), resultData_.star));
			}
		}
		if (eventNo_ == 11 && GlobalData.Instance.getStageData(stageNo).clearCount == 1)
		{
			StageInfo.Info currentInfo3 = stageTbl.getInfo(stageNo);
			int stageNum3 = stageTbl.GetComponent<StageIconDataTable>().getMaxCollaboStageIconsNum();
			if (resultData_.addAvatar > 1)
			{
				bGetCollaboReward = true;
			}
			if (stageNo % 100 == stageNum3)
			{
				bGetCollaboReward = true;
			}
			if (resultData_.addTicket > 0)
			{
				bGetCollaboReward = true;
			}
			else
			{
				StageInfo.Info nextInfo3 = stageTbl.getInfo(stageNo + 1);
				if (nextInfo3 != null && currentInfo3.Area < nextInfo3.Area)
				{
					bGetCollaboReward = true;
				}
			}
		}
		Debug.Log("bGetCollaboReward = " + bGetCollaboReward);
		resultRewards_[1].RewardType = Constant.eMoney.Coin;
		resultRewards_[1].Num += getGameCoinReward(bClear_, stageNo, 0);
		resultRewards_[2].RewardType = Constant.eMoney.Jewel;
		resultRewards_[2].Num += bonusJewel;
		Constant.Reward[] array = resultRewards_;
		foreach (Constant.Reward reward in array)
		{
			if (reward.RewardType != Constant.eMoney.Invalid && Constant.Reward.addReward(reward))
			{
				setLimitOverFlg(eLimit.Result, reward.RewardType);
			}
		}
		if (bClear_)
		{
		}
		Debug.Log("result_setupz");
		if (bLvUp_)
		{
			int lv = resultData_.afterLv - 1;
			rewardTbl.getRewards(lv, out lvupRewards_);
			if (lvupRewards_ != null)
			{
				Constant.Reward[] array2 = lvupRewards_;
				foreach (Constant.Reward reward2 in array2)
				{
					if (Constant.Reward.addReward(reward2))
					{
						setLimitOverFlg(eLimit.LvUp, reward2.RewardType);
					}
				}
			}
		}
		otherData_.save();
		Debug.Log("result_setup_end");
		StartCoroutine(execute());
	}

	private IEnumerator execute()
	{
		Hashtable args = new Hashtable();
		yield return StartCoroutine(showResult());
		switch (resultBtn_)
		{
		case DialogResultBase.eBtn.Retry:
			args.Add("IsRetry", true);
			break;
		case DialogResultBase.eBtn.Close:
			args.Add("IsClose", true);
			break;
		}
		if (bClear_ && stageNo == 0 && Bridge.StageData.getClearCount(stageNo) == 1)
		{
			DialogCommon clearbonus_dialog = dialogManager.getDialog(DialogManager.eDialog.ClearBonusInfo) as DialogCommon;
			clearbonus_dialog.setup(null, null, true);
			yield return StartCoroutine(clearbonus_dialog.open());
			while (clearbonus_dialog.isOpen())
			{
				yield return 0;
			}
		}
		if (bLvUp_)
		{
			DialogLevelUp dialog = dialogManager.getDialog(DialogManager.eDialog.LevelUp) as DialogLevelUp;
			dialogManager.addActiveDialogList(DialogManager.eDialog.LevelUp);
			int lv = Bridge.PlayerData.getLevel();
			yield return StartCoroutine(dialog.show(lv, lvupRewards_));
			yield return StartCoroutine(showLimitDialog(eLimit.LvUp));
			while (dialogManager.getActiveDialogNum() > 0)
			{
				yield return 0;
			}
			yield return StartCoroutine(luckyChance());
		}
		while (partManager.isLineLogin())
		{
			yield return 0;
		}
		DialogPlayScore scoreDialog = dialogManager.getDialog(DialogManager.eDialog.PlayScore) as DialogPlayScore;
		if (scoreDialog.getConnectStatus() != DialogPlayScore.eConnectStatus.Finish)
		{
			partManager.isStageResultDownloading = true;
			yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
			while (scoreDialog.getConnectStatus() != DialogPlayScore.eConnectStatus.Finish)
			{
				yield return 0;
			}
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			partManager.isStageResultDownloading = false;
		}
		scoreDialog.forceQuitCoroutine();
		yield return StartCoroutine(showChangeRanking(bClear_, nowScore_, prevScore_));
		if (bEventStage_)
		{
			yield return StartCoroutine(showEventSendInformation());
		}
		args.Add("IsClear", bClear_);
		args.Add("StageNo", stageNo);
		args.Add("IsProgressOpen", bProgressOpen);
		if (bBossOpen)
		{
			otherData_.setFlag(SaveOtherData.eFlg.RequestBossOpen, true);
			otherData_.save();
		}
		Debug.Log("stageNo =" + stageNo);
		if (bClear_ && Scenario.Instance.isScenario(stageNo, Scenario.ePlace.End) && eventNo_ != 11)
		{
			args.Add("Place", Scenario.ePlace.End);
			partManager.requestTransition(PartManager.ePart.Scenario, args, FadeMng.eType.Scenario, true);
			yield break;
		}
		if (bClear_ && Scenario.Instance.isScenario_Collaboration(stageNo, Scenario.ePlace.End) && eventNo_ == 11)
		{
			args.Add("Place", Scenario.ePlace.End);
			partManager.requestTransition(PartManager.ePart.Scenario, args, FadeMng.eType.Scenario, true);
			yield break;
		}
		if (bClear_ && Scenario.Instance.isScenario_Park(stageNo, Scenario.ePlace.End) && eventNo_ == 0)
		{
			args.Add("Place", Scenario.ePlace.End);
			partManager.requestTransition(PartManager.ePart.Scenario, args, FadeMng.eType.Scenario, true);
			yield break;
		}
		PartManager.ePart next_part = PartManager.ePart.Map;
		if (bParkStage_)
		{
			next_part = PartManager.ePart.Park;
		}
		else if (bEventStage_ && !bEventFinish)
		{
			if (eventNo_ == 1)
			{
				next_part = PartManager.ePart.EventMap;
			}
			else if (eventNo_ == 2)
			{
				next_part = PartManager.ePart.ChallengeMap;
			}
			else if (eventNo_ == 11)
			{
				next_part = PartManager.ePart.CollaborationMap;
			}
		}
		if (bEventFinish)
		{
			args.Clear();
			args.Add("StageNo", Bridge.PlayerData.getCurrentStage());
			args.Add("IsProgressOpen", true);
		}
		partManager.requestTransition(next_part, args, true);
	}

	private IEnumerator showResult()
	{
		if (bClear_)
		{
			DialogResultClear dialog = null;
			if (bParkStage_)
			{
				dialog = dialogManager.getDialog(DialogManager.eDialog.ParkStageClear) as DialogResultClear;
				dialog.SetMinilenNum(minilen_count_current);
				dialogManager.addActiveDialogList(DialogManager.eDialog.ParkStageClear);
			}
			else
			{
				dialog = dialogManager.getDialog(DialogManager.eDialog.ResultClear) as DialogResultClear;
				dialogManager.addActiveDialogList(DialogManager.eDialog.ResultClear);
			}
			yield return StartCoroutine(dialog.show(comboBonusCoin: calcCoin(comboBonusCoin, bClear_, stageNo, 0), stageInfo: stageInfo, lv: resultData_.beforeLv, score: nowScore_, stageNo: stageNo, highScore: prevScore_, rewards: resultRewards_, exp: resultData_.addExp, star: resultData_.star, bonusScore: bonusScore_, bonusCoin: bonusCoin, bonusJewel: bonusJewel, remainingBonus: remainingBonus, maxCombo: maxComboCount, campaignCoin: campaignCoin, isGetJewel: bGetStarJewel_, isEventStage: bEventStage_ && !bParkStage_, isGetCollaboReward: bGetCollaboReward, eventNo: eventNo_, eventCoin: totalEventCoin, isSkillCoinUp: bCoinUp, skillCoin: skillBonusCoin));
			if (bParkStage_ && _got_rare_minilen > 0 && _got_rare_minilen_level > 0)
			{
				DialogParkMinilenGet dpmg = dialogManager.getDialog(DialogManager.eDialog.ParkStageMinilenGet) as DialogParkMinilenGet;
				yield return StartCoroutine(dpmg.show(_got_rare_minilen));
				yield return StartCoroutine(MinilenFirstGetTutorial());
				while (dpmg.isOpen())
				{
					yield return 0;
				}
			}
			if (bEventStage_)
			{
				Tapjoy.TrackEvent("Money", "Income Coin", "Event Stage Clear", null, resultRewards_[1].Num);
				if (resultRewards_[2].Num > 0)
				{
					Tapjoy.TrackEvent("Money", "Income Jewel", "Event Stage Clear", null, resultRewards_[2].Num);
				}
			}
			else if (bParkStage_)
			{
				Tapjoy.TrackEvent("Money", "Income Coin", "Park Stage Clear", null, resultRewards_[1].Num);
				if (resultRewards_[2].Num > 0)
				{
					Tapjoy.TrackEvent("Money", "Income Jewel", "Park Stage Clear", null, resultRewards_[2].Num);
				}
				if (minilen_count_current > 0)
				{
					Tapjoy.TrackEvent("Money", "Income Minilen", "Park Stage Clear", null, minilen_count_current);
				}
			}
			else
			{
				Tapjoy.TrackEvent("Money", "Income Coin", "Stage Clear", null, resultRewards_[1].Num);
				if (resultRewards_[2].Num > 0)
				{
					Tapjoy.TrackEvent("Money", "Income Jewel", "Stage Clear", null, resultRewards_[2].Num);
				}
			}
			if (bEventStage_)
			{
				GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Event Stage Clear", resultRewards_[1].Num);
				if (resultRewards_[2].Num > 0)
				{
					GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Jewel", "Event Stage Clear", resultRewards_[2].Num);
				}
			}
			else if (bParkStage_)
			{
				GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Park Stage Clear", resultRewards_[1].Num);
				if (resultRewards_[2].Num > 0)
				{
					GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Jewel", "Park Stage Clear", resultRewards_[2].Num);
				}
				if (minilen_count_current > 0)
				{
					GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Minilen", "Park Stage Clear", minilen_count_current);
				}
			}
			else
			{
				GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Stage Clear", resultRewards_[1].Num);
				if (resultRewards_[2].Num > 0)
				{
					GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Jewel", "Stage Clear", resultRewards_[2].Num);
				}
			}
			if (nowScore_ > prevScore_)
			{
				if (Bridge.StageData.getClearCount(stageNo) > 1 && !bEventStage_)
				{
					DialogSendMaxScore scoreDialog = dialogManager.getDialog(DialogManager.eDialog.SendMaxScore) as DialogSendMaxScore;
					yield return StartCoroutine(scoreDialog.show(Bridge.StageData.getHighScore(stageNo), stageNo));
					while (scoreDialog.isOpen())
					{
						yield return 0;
					}
				}
				else if (!bEventStage_)
				{
					StageDataTable dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
					bool bAreaClear = false;
					StageInfo.Info currentInfo = dataTable.getInfo(stageNo);
					int stageNum = dataTable.GetComponent<StageIconDataTable>().getMaxStageIconsNum();
					if (stageNo == stageNum - 1)
					{
						bAreaClear = true;
					}
					else
					{
						StageInfo.Info nextInfo = dataTable.getInfo(stageNo + 1);
						if (nextInfo != null && currentInfo.Area < nextInfo.Area)
						{
							bAreaClear = true;
						}
						if (Constant.ParkStage.isParkStage(stageNo) && nextInfo == null)
						{
							bAreaClear = true;
						}
					}
					if (bAreaClear)
					{
						DialogSendAreaClear areaDialog = dialogManager.getDialog(DialogManager.eDialog.SendAreaClear) as DialogSendAreaClear;
						int area_no = currentInfo.Area;
						if (bParkStage_)
						{
							area_no += 50000;
						}
						yield return StartCoroutine(areaDialog.show(area_no));
						while (areaDialog.isOpen())
						{
							yield return 0;
						}
					}
				}
			}
		}
		else
		{
			dialogManager.addActiveDialogList(DialogManager.eDialog.ResultFailed);
			DialogResultFailed dialog3 = dialogManager.getDialog(DialogManager.eDialog.ResultFailed) as DialogResultFailed;
			totalEventCoin = 0;
			yield return StartCoroutine(dialog3.show(stageInfo, getClearState(totalScore), totalScore, stageNo, resultRewards_[1].Num, resultData_.addExp, bonusCoin, bonusJewel, campaignCoin, bEventStage_, eventNo_, totalEventCoin, bCoinUp, skillBonusCoin));
			if (bEventStage_)
			{
				Tapjoy.TrackEvent("Money", "Income Coin", "Event Stage Failed", null, resultRewards_[1].Num);
				Tapjoy.TrackEvent(string.Empty, "Event Stage Failed", "Stage No - " + (stageNo + 1), null, 0L);
			}
			else if (bParkStage_)
			{
				Tapjoy.TrackEvent("Money", "Income Coin", "Park Stage Failed", null, resultRewards_[1].Num);
				Tapjoy.TrackEvent(string.Empty, "Park Stage Failed", "Stage No - " + (stageNo + 1), null, 0L);
			}
			else
			{
				Tapjoy.TrackEvent("Money", "Income Coin", "Stage Failed", null, resultRewards_[1].Num);
				Tapjoy.TrackEvent(string.Empty, "Stage Failed", "Stage No - " + stageNo, null, 0L);
			}
			if (bEventStage_)
			{
				GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Event Stage Failed", resultRewards_[1].Num);
				GlobalGoogleAnalytics.Instance.LogEvent("Event Stage Failed", "Stage No - " + (stageNo + 1), "0", 1L);
			}
			else if (bParkStage_)
			{
				GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Park Stage Failed", resultRewards_[1].Num);
				GlobalGoogleAnalytics.Instance.LogEvent("Park Stage Failed", "Stage No - " + stageNo, "0", 1L);
			}
			else
			{
				GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Stage Failed", resultRewards_[1].Num);
				GlobalGoogleAnalytics.Instance.LogEvent("Stage Failed", "Stage No - " + (stageNo + 1), "0", 1L);
			}
		}
		yield return StartCoroutine(showLimitDialog(eLimit.Result));
		while (dialogManager.getActiveDialogNum() > 0)
		{
			yield return 0;
		}
		DialogManager.eDialog resultDialogType = DialogManager.eDialog.ResultFailed;
		if (bClear_)
		{
			resultDialogType = ((!bParkStage_) ? DialogManager.eDialog.ResultClear : DialogManager.eDialog.ParkStageClear);
		}
		DialogResultBase resultDialog = dialogManager.getDialog(resultDialogType) as DialogResultBase;
		resultBtn_ = resultDialog.getClickBtn();
	}

	public IEnumerator showEventSendInformation()
	{
		long[] id_list = null;
		partManager.isStageResultDownloading = true;
		yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
		EventSendListResponse res2 = null;
		bEventFinish = false;
		yield return dialogManager.StartCoroutine(NetworkMng.Instance.download(OnCreateEventSendListWWW, false, true));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			if (NetworkMng.Instance.getResultCode() == eResultCode.EventIsNotHolding)
			{
				bEventFinish = true;
			}
			partManager.isStageResultDownloading = false;
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		res2 = JsonMapper.ToObject<EventSendListResponse>(www.text);
		yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		partManager.isStageResultDownloading = false;
		if (res2 != null)
		{
			id_list = res2.memberNoList;
		}
		bool bFriend2 = false;
		for (int i = 0; i < DummyPlayFriendData.FriendNum; i++)
		{
			UserData data = DummyPlayFriendData.DummyFriends[i];
			if (data.StageNo < 18)
			{
				continue;
			}
			if (id_list != null)
			{
				bool sent = false;
				long[] array = id_list;
				foreach (long id in array)
				{
					if (id == data.ID)
					{
						sent = true;
						break;
					}
				}
				if (sent)
				{
					continue;
				}
			}
			bFriend2 = true;
			break;
		}
	}

	private float getCoinRate(bool bClear, int clearCount)
	{
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		RewardDataTable component = @object.GetComponent<RewardDataTable>();
		return getGainRate(bClear, component.getCoinRate(), component, clearCount);
	}

	private float getStarCoinRate(int star)
	{
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		RewardDataTable component = @object.GetComponent<RewardDataTable>();
		int[] coinStarRates = component.getCoinStarRates();
		return (float)coinStarRates[star] / 100f;
	}

	private int getGameCoinReward(bool bClear, int stageNo, int clearCountOffset)
	{
		int coin = totalCoin;
		return calcCoin(coin, bClear, stageNo, clearCountOffset) + campaignCoin + bonusCoin + skillBonusCoin;
	}

	private int calcCoin(int coin, bool bClear, int stageNo, int clearCountOffset)
	{
		int clearCount = Bridge.StageData.getClearCount(stageNo);
		return Mathf.RoundToInt((float)coin * getCoinRate(bClear, clearCount + clearCountOffset));
	}

	private int getResultCoinReward(bool bClear, int clearCount, int star)
	{
		int coin = stageInfo.Coin;
		return Mathf.RoundToInt((float)coin * getStarCoinRate(star) * getCoinRate(bClear_, clearCount));
	}

	private int getResultCoinReward_Challenge(bool bClear, int clearCount)
	{
		int num = stageInfo.Coin;
		if (bClear)
		{
			if (clearCount == 1)
			{
				num += stageInfo.StarRewardNum;
			}
		}
		else
		{
			num = 0;
		}
		return num;
	}

	private int getResultCoinReward_Collaboration(bool bClear, int clearCount)
	{
		int num = stageInfo.Coin;
		if (bClear)
		{
			num = ((clearCount != 1) ? Mathf.RoundToInt((float)num * getCoinRate(bClear_, clearCount)) : stageInfo.StarRewardNum);
		}
		return num;
	}

	private float getGainRate(bool bClear, RewardInfo.Rate rateData, RewardDataTable rewardTbl, int clearCount)
	{
		int num = 0;
		num = (bClear ? ((clearCount != 1) ? rewardTbl.getRate(rateData, RewardDataTable.eRate.AlreadyClear) : rewardTbl.getRate(rateData, RewardDataTable.eRate.Clear)) : ((clearCount <= 0) ? rewardTbl.getRate(rateData, RewardDataTable.eRate.Failed) : rewardTbl.getRate(rateData, RewardDataTable.eRate.AlreadyFailed)));
		return (float)num / 100f;
	}

	private int getBonusScore(RewardDataTable rewardTbl, int lv, int score)
	{
		int bonus = rewardTbl.getBonus(lv - 1);
		float num = (float)bonus / 100f;
		int num2 = (int)Mathf.Ceil((float)score * num);
		if (num2 < 10)
		{
			return 0;
		}
		float f = (float)num2 / 10f;
		return Mathf.RoundToInt(f) * 10;
	}

	private IEnumerator showChangeRanking(bool bClear, int nowScore, int prevScore)
	{
		int myRank = -1;
		int rivalRank = -1;
		List<UserData> friendList = createFriendList();
		if (bClear && checkChangeRank(nowScore, prevScore, ref myRank, ref rivalRank, friendList))
		{
			DialogChangeRanking dialog = dialogManager.getDialog(DialogManager.eDialog.ChangeRanking) as DialogChangeRanking;
			UserData friendData = friendList[myRank];
			dialogManager.addActiveDialogList(DialogManager.eDialog.ChangeRanking);
			yield return StartCoroutine(dialog.show(nowScore, DummyPlayerData.Data, friendData, myRank + 1, rivalRank + 1, stageNo));
			while (dialogManager.getActiveDialogNum() > 0)
			{
				yield return 0;
			}
		}
	}

	private List<UserData> createFriendList()
	{
		List<UserData> userList = new List<UserData>(DummyPlayFriendData.DummyFriends);
		for (int num = userList.Count - 1; num >= 0; num--)
		{
			UserData userData = userList[num];
			if (userData.ID != DummyPlayerData.Data.ID && userData.StageClearCount <= 0)
			{
				userList.Remove(userData);
			}
		}
		DummyFriendDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<DummyFriendDataTable>();
		if (component.getInfoCount() > userList.Count)
		{
			int num2 = component.getInfoCount() - userList.Count;
			for (int i = 0; i < num2; i++)
			{
				UserData item = DummyPlayFriendData.createDummyFriend(stageNo, i);
				userList.Add(item);
			}
		}
		NetworkUtility.SortScore(ref userList);
		return userList;
	}

	private bool checkChangeRank(int nowScore, int prevScore, ref int myRank, ref int rivalRank, List<UserData> friendList)
	{
		if (prevScore >= nowScore)
		{
			return false;
		}
		myRank = -1;
		for (int i = 0; i < friendList.Count; i++)
		{
			UserData userData = friendList[i];
			if (userData.StageClearCount > 0 && prevScore <= userData.Score && nowScore > userData.Score)
			{
				myRank = i;
				break;
			}
		}
		if (myRank == -1)
		{
			return false;
		}
		rivalRank = -1;
		int score = friendList[myRank].Score;
		for (int j = myRank + 1; j < friendList.Count; j++)
		{
			UserData userData2 = friendList[j];
			if (userData2.StageClearCount > 0 && score >= userData2.Score)
			{
				rivalRank = j;
				break;
			}
		}
		if (rivalRank == -1)
		{
			rivalRank = friendList.Count;
		}
		return true;
	}

	private bool isLimitOver(eLimit limit, Constant.eMoney money)
	{
		return (((uint)limitFlgs_[(int)money] & (uint)limit) != 0) ? true : false;
	}

	private void setLimitOverFlg(eLimit limit, Constant.eMoney money)
	{
		limitFlgs_[(int)money] |= (int)limit;
	}

	private IEnumerator showLimitDialog(eLimit limit)
	{
		DialogLimitOver limitOverDialog = dialogManager.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
		for (int i = 0; i < limitFlgs_.Length; i++)
		{
			Constant.eMoney money = (Constant.eMoney)i;
			if (isLimitOver(limit, money))
			{
				yield return StartCoroutine(limitOverDialog.show(money));
				while (limitOverDialog.isOpen())
				{
					yield return 0;
				}
			}
		}
	}

	private List<Constant.eMoney> createLimitOverList(eLimit limit)
	{
		List<Constant.eMoney> list = new List<Constant.eMoney>();
		for (int i = 0; i < 5; i++)
		{
			if (isLimitOver(limit, (Constant.eMoney)i))
			{
				list.Add((Constant.eMoney)i);
			}
		}
		return list;
	}

	private IEnumerator luckyChance()
	{
		int bonusChanceLv2 = 6;
		bonusChanceLv2 = GlobalData.Instance.getGameData().bonusChanceLv;
		if (Bridge.PlayerData.getLevel() >= bonusChanceLv2)
		{
			DialogLuckyChance dialog = dialogManager.getDialog(DialogManager.eDialog.LuckyChance) as DialogLuckyChance;
			yield return StartCoroutine(dialog.show());
			while (dialogManager.getActiveDialogNum() > 0)
			{
				yield return 0;
			}
		}
	}

	private WWW OnCreateEventSendListWWW(Hashtable hash)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		string param = eventNo_.ToString();
		WWWWrap.addGetParameter("eventNo", param);
		return WWWWrap.create("message/eventsendlist/");
	}

	public void clearRotateFulcrumMetal()
	{
		rotateFulcrumList.Clear();
	}

	public void setRotateFulcrumMetal(Bubble hitBubble)
	{
		if (!(hitBubble == null) && !rotateFulcrumList.Contains(hitBubble))
		{
			Debug.Log("Add!!");
			hitBubble.rotateState.isLeft = hitBubble.type == Bubble.eType.RotateFulcrumL;
			rotateFulcrumList.Add(hitBubble);
		}
	}

	private IEnumerator setRotateFulcrum(Bubble shotBubble, Bubble hitBubble)
	{
		rotateFulcrumList.Clear();
		if (shotBubble == null)
		{
			yield break;
		}
		int rad2 = 0;
		for (int i = 0; i < fieldBubbleList.Count; i++)
		{
			if (!fieldBubbleList[i].inCloud && (fieldBubbleList[i].type == Bubble.eType.RotateFulcrumL || fieldBubbleList[i].type == Bubble.eType.RotateFulcrumR) && !rotateFulcrumList.Contains(fieldBubbleList[i]))
			{
				fieldBubbleList[i].rotateState.isLeft = fieldBubbleList[i].type == Bubble.eType.RotateFulcrumL;
				rad2 = shotBubble.GetRotateRad(fieldBubbleList[i].myTrans.localPosition);
				if (fieldBubbleList[i] == hitBubble || rad2 <= 1)
				{
					rotateFulcrumList.Add(fieldBubbleList[i]);
				}
				else if (shotBubble.type != Bubble.eType.Metal && rad2 <= 2)
				{
					rotateFulcrumList.Add(fieldBubbleList[i]);
				}
			}
		}
	}

	private IEnumerator updateRotateBubble(Bubble shotBubble)
	{
		if (rotateFulcrumList.Count <= 0)
		{
			yield break;
		}
		rotateList.Clear();
		for (int i = 0; i < rotateFulcrumList.Count; i++)
		{
			if (rotateFulcrumList[i].state == Bubble.eState.Field)
			{
				StartCoroutine(rotateBubbleAnim(rotateFulcrumList[i]));
				yield return StartCoroutine(setRotateBubbleState(rotateFulcrumList[i]));
			}
		}
		dropRotateList.Clear();
		updateFieldBubbleList();
		yield return StartCoroutine(setRotateBubblePos());
		if (drop(true))
		{
			yield return stagePause.sync();
			updateFieldBubbleList();
		}
		if (breakFulcrum())
		{
			yield return stagePause.sync();
		}
		updateFieldBubbleList();
		if (bGetKey)
		{
			yield return StartCoroutine(GetkeyBubbleRoutine());
		}
		rotateFulcrumList.Clear();
	}

	private IEnumerator setRotateBubbleState(Bubble fulcrum)
	{
		int rad2 = 0;
		for (int i = 0; i < fieldBubbleList.Count; i++)
		{
			rad2 = fulcrum.GetRotateRad(fieldBubbleList[i].myTrans.localPosition);
			if (rad2 <= 2 && rad2 > 0)
			{
				fieldBubbleList[i].SetRoteteFulcrum(fulcrum);
				if (!rotateList.Contains(fieldBubbleList[i]))
				{
					rotateList.Add(fieldBubbleList[i]);
				}
			}
		}
		yield break;
	}

	private IEnumerator setRotateBubblePos()
	{
		List<Bubble> rotateListTmp = new List<Bubble>();
		rotateListTmp.AddRange(rotateList);
		for (int j = 0; j < rotateList.Count; j++)
		{
			StartCoroutine(rotateList[j].SetRotatePos());
		}
		while (rotateListTmp.Count > 0)
		{
			for (int i = 0; i < rotateList.Count; i++)
			{
				if (rotateList[i].rotateState.moveCnt <= 0 && rotateListTmp.Contains(rotateList[i]))
				{
					rotateListTmp.Remove(rotateList[i]);
				}
			}
			yield return stagePause.sync();
		}
		rotateList.Clear();
		foreach (Bubble b in fieldBubbleList)
		{
			b.inCloud = false;
		}
		yield return StartCoroutine(cloudUpdateCheck());
	}

	public IEnumerator dropRotateBubble(Bubble bubble)
	{
		if (state == eState.Gameover || state == eState.Clear)
		{
			yield break;
		}
		dropRotateList.Add(bubble);
		if (bubble.type >= Bubble.eType.SnakeRed && bubble.type <= Bubble.eType.SnakeBlack && !bubble.isFrozen)
		{
			snakeCount_ += 3;
			if (snakeCount_ > 99)
			{
				snakeCount_ = 99;
			}
			Sound.Instance.playSe(Sound.eSe.SE_514_snake_egg_break);
			snakeEffect(bubble);
		}
		bubble.startDrop(0);
	}

	private IEnumerator rotateBubbleAnim(Bubble bubble)
	{
		if (!(bubble == null) && (bubble.type == Bubble.eType.RotateFulcrumR || bubble.type == Bubble.eType.RotateFulcrumL))
		{
			int clipId = bubble.sprite.clipId;
			bubble.sprite.Resume();
			if (bubble.type == Bubble.eType.RotateFulcrumR)
			{
				bubble.sprite.Play("action_52");
			}
			else if (bubble.type == Bubble.eType.RotateFulcrumL)
			{
				bubble.sprite.Play("action_53");
			}
			bubble.sprite.animationCompleteDelegate = RotateBubbleAnimCompleteDelegate;
			Sound.Instance.playSe(Sound.eSe.SE_605_gear);
			while (bubble.sprite.isPlaying() && bubble.sprite.animationCompleteDelegate != null)
			{
				yield return stagePause.sync();
			}
			bubble.sprite.Resume();
			bubble.sprite.Play(clipId);
		}
	}

	private void RotateBubbleAnimCompleteDelegate(tk2dAnimatedSprite sprite, int clipId)
	{
		sprite.animationCompleteDelegate = null;
	}
}
