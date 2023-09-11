using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Network;
using UnityEngine;

public class Part_BossStage : PartBase
{
	private class ReplayData
	{
		public Bubble.eType type;

		public Vector3 pos;

		public int lineFriendIndex;

		public bool isFrozen;
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
		ActionWait = 7,
		Clear = 8,
		Gameover = 9,
		Result = 10,
		End = 11
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
		DeadLineOver = 6
	}

	public enum eClear
	{
		Score = 1,
		AllDel = 2,
		Friend = 4,
		Fulcrum = 8
	}

	private class FulcrumBrokenData
	{
		public bool isBroken;

		public int createIndex;

		public float brokenTime;
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

	private const float DRANK_OJAMA_IN_WAIT_TIME = 1.2f;

	private const int bubbleLineCount = 3;

	private const int COMBO_DISP_COUNT = 2;

	private const float END_WAIT_TIME = 1f;

	private const int SNAKE_SHOW_MAX = 3;

	private const float CHANGE_COLOR_TIME = 10f;

	private const int CHANGE_COLOR_SHOT_COUNT = 5;

	private const string CEILING_BUBBLE_NAME = "99";

	private const int GLOSS_INTERVAL = 30;

	private const float DRAW_HIT_SIZE_PAR = 0.8f;

	private const float DRAW_OFFSET_SIZE = 10f;

	private const int DRAW_COUNT_MAX = 6;

	private const float CEILING_OFFSET_LOCALPOS = 60f;

	private const int BOSS_STAGE_NO_BASE = 30000;

	private const int BOBBLEN_SHOT_ADD_COUNT = 3;

	private const int MAX_BOBBLEN_SHOT_COUNT = 99;

	private const int RANDOM_RESEARCH_LIMIT = 10;

	private const float LIMIT_X_MIN = -0.54f;

	private const float LIMIT_X_MAX = 0.54f;

	private const float LIMIT_Y_MIN = -0.8f;

	private const float Fether_Wait_Time = 0.2f;

	private const float DEF_BOBBLEN_X = -178f;

	private const float DEF_BOBBLEN_Z = -5f;

	private const float SHOT_BOBBLEN_X = -158f;

	private const float SHOT_BOBBLEN_Z = -4f;

	public float uiScale;

	private BossStageInfo.Info bossStageInfo;

	private BossStageInfo.LevelInfo bossLevelInfo;

	private StageInfo.CommonInfo stageInfo;

	private StageData stageData;

	public StagePause_Boss stagePause;

	public GameObject bossObj;

	private GameObject guardEff;

	private GameObject hitEff;

	private GameObject clearAnim;

	private GameObject drank_ojama;

	private GameObject drank_start;

	private UISpriteAnimation drankSprite;

	private GameObject drankLight;

	private Egg egg_;

	public GameObject eggObj;

	private Nest nest_;

	public List<GameObject> nestList;

	private GameObject Boss_02_egg;

	private GameObject Boss_02_nest;

	public GameObject spiderwebObj;

	public List<Spiderweb> spwList = new List<Spiderweb>();

	public List<tk2dAnimatedSprite> spwAnims = new List<tk2dAnimatedSprite>();

	public List<Bubble_Boss> inSpwBubbles = new List<Bubble_Boss>();

	public List<GameObject> recreateInSpwBubbles = new List<GameObject>();

	private GameObject Boss_03_spiderweb;

	public Bubble_Boss pickingTargetBubble;

	private float[] startline_y = new float[3] { -364f, -468f, -572f };

	private int recreateLineNumber;

	private float spiderwebLine_y;

	private GameObject specialbubble_26_eff;

	private Transform bossObjectRoot;

	private Vector3 bossObjectRootPos = new Vector3(-270f, 455f, 0f);

	private int mComboCount;

	private UILabel comboLabel;

	private Animation comboAnim;

	private int maxComboCount;

	public int mTotalCoin;

	public int totalEventCoin;

	public int coinBubbleCount;

	public int nextCorrection = 5;

	public int nextLimitCorrection = 50;

	private GameObject bubbleObject;

	private Transform bubbleRoot;

	private Transform nextBubbleRoot;

	private Vector3 bubbleRootPos = new Vector3(-270f, 455f, 0f);

	private List<Transform> ceilingBubbleList = new List<Transform>(10);

	private List<ReplayData> replayDataList = new List<ReplayData>(256);

	private Dictionary<int, List<Bubble.eType>> replayChainTypeDic = new Dictionary<int, List<Bubble.eType>>();

	private Dictionary<int, List<Vector3>> replayChainPosDic = new Dictionary<int, List<Vector3>>();

	private List<Bubble.eType> replayNextTypeList = new List<Bubble.eType>(3);

	private bool isInvalidReplay = true;

	private Transform launchpad;

	private List<Transform> scoreList = new List<Transform>(8);

	private Transform scoreRoot;

	private Transform coinRoot;

	private GameObject[] nextBubbles = new GameObject[3];

	private Transform[] nextBubblePoses = new Transform[3];

	private Animation stepNextBubbleAnim;

	private string stepNextBubbleClipName = "Next_bubble_00_anm";

	private int nextBubbleCount = 2;

	public List<Bubble_Boss> fieldBubbleList = new List<Bubble_Boss>(256);

	private int fieldObjectCount;

	private int fieldFriendCount;

	private int fieldItemCount;

	public List<Bubble_Boss> fulcrumList = new List<Bubble_Boss>();

	public List<Bubble_Boss> noDropList = new List<Bubble_Boss>();

	public List<Bubble_Boss> growBubbleList = new List<Bubble_Boss>();

	public List<Bubble_Boss> growCandidateList = new List<Bubble_Boss>();

	public Dictionary<int, List<ChainBubble>> chainBubbleDic = new Dictionary<int, List<ChainBubble>>();

	private List<Bubble.eType> searchedBubbleTypeList = new List<Bubble.eType>(32);

	private List<Bubble_Boss> searchedBubbleList = new List<Bubble_Boss>();

	private BubbleNavi bubbleNavi;

	private int normalBubbleCount;

	private List<Bubble_Boss> lineFriendCandidateList = new List<Bubble_Boss>();

	public GameObject lineFriendBase;

	private Texture defaultUserIconTexture;

	private GameObject[] bubbleBonusBases = new GameObject[6];

	private Transform stageBg;

	private Transform stageUi;

	private Transform frontUi;

	private Transform scrollUi;

	public GameObject boundEffL;

	public GameObject boundEffR;

	private GameObject counter_eff;

	private Animation counter_eff_anm;

	private System.Random random = new System.Random();

	private eState mState;

	private eGameType gameType;

	private eGameover gameoverType;

	private bool bResultOpen;

	public int shotCount;

	public float startTime;

	private int TIME_ADD_VALUE = 5;

	private int PLUS_ADD_VALUE = 3;

	private int stageNo;

	private int bossType;

	private int bossLevel;

	private UILabel bossNameLabel;

	private UILabel bossLvLabel;

	private UISlider scoregauge;

	private float starRate1;

	private float starRate2;

	private GameObject[] stars;

	private PopupExcellent popupExcellent;

	private ReadyGo readyGo;

	private GameObject stageClear;

	public GameObject chacknBase;

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

	public string waitPinchAnimName = "chara_00_09_02_0";

	private float glossTime;

	private Bubble.eType glossType;

	private List<Bubble.eType> glossColorList = new List<Bubble.eType>();

	private float cutinWaitTime = 2f;

	private GameObject bubble_trail_eff;

	private Vector3 bubble_trail_offset;

	private GameObject useitem_bg;

	private GameObject shootButton;

	private Arrow_Boss arrow;

	public Guide_Boss guide;

	private GameObject next_tap;

	private List<Constant.Item.eType> useItemList_ = new List<Constant.Item.eType>();

	private StageBoostItemParent itemParent_;

	private StageUseBoostItemParent useItemParent_;

	private int bubblePlusNum;

	private int timePlusNum;

	private int scoreUpNum = 1;

	private bool bOPAnime_;

	public Material fadeMaterial;

	private int stageEnd_ShotCount;

	private float stageEnd_Time;

	private int stageEnd_ContinueCount;

	private long[] helpDataList;

	private List<Bubble.eType> clearChacknList = new List<Bubble.eType>();

	private List<Bubble_Boss> nearCoinBubbleList_ = new List<Bubble_Boss>();

	private Material gameoverMat;

	private float nextBubbleBobllenBefor_Y;

	private GameObject bubble_20_eff;

	public GameObject counter_count;

	private GameObject deadLine;

	public GameObject sweatEff;

	private Vector3 deadLinePos = new Vector3(0f, -246f, 0f);

	private BossBase bossBase_;

	private int lineDownCount;

	public float scrollTime;

	public float scrollTimeBefor;

	private Transform baseBubbleRoot;

	private List<int> fulcrumIndexList = new List<int>();

	private List<Bubble_Boss> baseBubbleList = new List<Bubble_Boss>();

	private List<int> useRandomColorList = new List<int>();

	private FulcrumBrokenData[] fulcrumBrokenDatas;

	private List<Bubble_Boss>[] BubbleAlignmentLineList;

	private bool requestRecreateFieldBubble;

	private float RECREATE_BUBBLE_FADEIN_TIME = 0.6f;

	public float LINE_DOWN_SEC = 15f;

	public int USE_RANDOM_COLOR = 3;

	public float ALL_BREAK_RECREATE_TIME = 7f;

	public float MOVE_SPEED = 8f;

	public int ATTACK_SPAN = 8;

	private GameObject bobblenShotCountObj;

	private UILabel bobblenShotCountLabel;

	private Animation bobblenShotCounterAnm;

	private int bobblenShotCount_;

	private bool bUsingPowerUp;

	public bool isWaitStageAcction;

	private bool isWaitScroll;

	private BossSoundBase bossSound_;

	private Vector3 powerup_offsetZ = new Vector3(0f, 0f, -2f);

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

	private bool bSearch;

	private List<FriendBonus> friendBonusList = new List<FriendBonus>();

	private Vector3 shotBubblePos;

	private List<Bubble_Boss> frozenBreakList = new List<Bubble_Boss>();

	public List<Chackn_Boss> chacknList = new List<Chackn_Boss>();

	public float moveSpan;

	private UILabel lineFriendLabel_;

	private UISysFontLabel lineFriendSysLabel_;

	private bool bMetalShooting;

	private GameObject shineEffectInstance_;

	private int breakLightningCount_;

	public bool isSearching;

	public bool isMoving;

	private Dictionary<int, BubbleBase> searchDict = new Dictionary<int, BubbleBase>();

	private List<KeyValuePair<BubbleBase, float>> ceilingList;

	private float ceilingBaseY;

	public float ceilingWldBaseY;

	private Bubble_Boss upLeftBubble;

	private Bubble_Boss downRightBubble;

	private Dictionary<Bubble.eType, int> prevCreateBubble = new Dictionary<Bubble.eType, int>();

	private int minLine;

	private bool bGameOver;

	private Coroutine tayunCoroutine;

	private bool isDrankEff;

	private int recreateCount;

	private bool bUniqueCreateLine;

	public int nextScore;

	private int dispCoin = 1;

	private int tempCoin;

	public int nextCoin;

	private System.Random rand = new System.Random();

	private int countUpEffectCount;

	private bool bLastPoint;

	public bool lastPoint_chackn;

	private List<Bubble_Boss> changeNativeSpriteAfterBreakList = new List<Bubble_Boss>();

	private string[] charaNames;

	private int snakeCount_;

	private GameObject snakeCounter;

	private UILabel snakeCounterLabel;

	private Animation snakeCounterAnm;

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
			if (mState != eState.ActionWait && mState != eState.Scroll && itemParent_ != null)
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

	private void Start()
	{
		state = eState.Start;
	}

	private void Update()
	{
		if (state < eState.Wait || state >= eState.Result)
		{
			return;
		}
		updateTotalScoreDisp();
		if (state < eState.Clear && glossTime < Time.time)
		{
			fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
			{
				fieldBubble.startGloss(glossType);
			});
			updateGlossTime();
		}
	}

	private void LateUpdate()
	{
		if (state < eState.Wait || state >= eState.Clear)
		{
			return;
		}
		updateTotalScoreDisp();
		if (state >= eState.Clear)
		{
			return;
		}
		updateFulcrumBrokenTime();
		if (stagePause.pause || bossBase_.state == BossBase.eState.dead || state < eState.Wait || state > eState.ActionWait)
		{
			return;
		}
		stageEnd_Time += Time.deltaTime;
		if (scrollTime != 0f)
		{
			scrollTime += Time.deltaTime;
			if (scrollTime - scrollTimeBefor >= LINE_DOWN_SEC && state == eState.Wait)
			{
				StartCoroutine(timeLineDown());
				scrollTimeBefor = scrollTime;
			}
		}
	}

	public IEnumerator recreateImmediateRoutine()
	{
		setRecreateAllFulcrumImmediate(true);
		yield return StartCoroutine(recreateCheck());
		setRecreateAllFulcrumImmediate(false);
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!(partManager == null) && !partManager.isTransitioning() && state < eState.Clear && pauseStatus && !stagePause.pause)
		{
			stagePause.pause = true;
			partManager.StartCoroutine(dialogManager.openDialog(DialogManager.eDialog.PauseBoss));
			DialogPauseBoss dialogPauseBoss = (DialogPauseBoss)dialogManager.getDialog(DialogManager.eDialog.PauseBoss);
			dialogPauseBoss.init(bossStageInfo.BossInfo);
			dialogPauseBoss.stagePause = stagePause;
			dialogPauseBoss.stageNo = stageNo;
			dialogPauseBoss.setup();
			partManager.StartCoroutine(dialogManager.openDialog(dialogPauseBoss));
		}
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		if (trigger.name.Contains("sell_boost"))
		{
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(pressBoostItem(trigger.GetComponent<StageBoostItem>()));
			yield break;
		}
		switch (trigger.name)
		{
		case "Gamestop_Button":
			if (state != eState.Clear && !stagePause.pause)
			{
				Constant.SoundUtil.PlayButtonSE();
			}
			OnApplicationPause(true);
			break;
		case "next_tap":
			if (state == eState.Wait && !stagePause.pause)
			{
				Sound.Instance.playSe(Sound.eSe.SE_207_koukan);
				StartCoroutine(exchangeRoutine());
			}
			break;
		case "shoot_button":
			if (state == eState.Wait)
			{
				Constant.SoundUtil.PlayDecideSE();
				guide.lineUpdate();
				shoot(arrow.fireVector);
			}
			break;
		case "chara_01":
			if (state == eState.Wait && !stagePause.pause)
			{
				Sound.Instance.playSe(Sound.eSe.SE_207_koukan);
				StartCoroutine(exchangeBobblenRoutine());
			}
			break;
		}
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
					item.use();
					useItem(itemType, item.getNum());
				}
				else
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
		DialogBossStageShop dialog = dialogManager.getDialog(DialogManager.eDialog.BossStageShop) as DialogBossStageShop;
		yield return StartCoroutine(dialog.show(Constant.Boss.convBossInfoToNo(bossType, 0), bossLevel, item, stageInfo, isSetItemUse));
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
		state = eState.Wait;
	}

	public void useBuyItem(StageBoostItem item, Constant.Item.eType itemType)
	{
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
			Constant.Item.eType eType = item;
			if (eType == Constant.Item.eType.PowerUp)
			{
				bUsingPowerUp = true;
				if (nextBubbles[arrow.charaIndex] != null)
				{
					setPowerupBubble(nextBubbles[arrow.charaIndex].GetComponent<Bubble_Boss>());
				}
			}
			return;
		}
		Bubble_Boss bubble_Boss = ((bobblenShotCount_ <= 0) ? nextBubbles[0].GetComponent<Bubble_Boss>() : nextBubbles[1].GetComponent<Bubble_Boss>());
		switch (item)
		{
		case Constant.Item.eType.HyperBubble:
			bubble_Boss.setType(Bubble.eType.Hyper);
			break;
		case Constant.Item.eType.BombBubble:
			bubble_Boss.setType(Bubble.eType.Bomb);
			break;
		case Constant.Item.eType.ShakeBubble:
			bubble_Boss.setType(Bubble.eType.Shake);
			break;
		case Constant.Item.eType.MetalBubble:
			bubble_Boss.setType(Bubble.eType.Metal);
			break;
		case Constant.Item.eType.IceBubble:
			bubble_Boss.setType(Bubble.eType.Ice);
			break;
		case Constant.Item.eType.FireBubble:
			bubble_Boss.setType(Bubble.eType.Fire);
			break;
		case Constant.Item.eType.WaterBubble:
			bubble_Boss.setType(Bubble.eType.Water);
			break;
		case Constant.Item.eType.ShineBubble:
			bubble_Boss.setType(Bubble.eType.Shine);
			break;
		}
		setNextTap(false);
		if (snakeCount_ > 0)
		{
			setNextTapBobblen(false);
		}
		bubbleNavi.startNavi(searchedBubbleList, bubble_Boss.type);
	}

	private void setPowerupBubble(Bubble_Boss currentBubble)
	{
		if (currentBubble != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(specialbubble_26_eff) as GameObject;
			Utility.setParent(gameObject, currentBubble.transform, true);
			gameObject.transform.localPosition += powerup_offsetZ;
			gameObject.transform.Find("loop").gameObject.SetActive(true);
			if (currentBubble.powerUpEffect == null)
			{
				currentBubble.powerUpEffect = gameObject.transform;
			}
			currentBubble.isPowerUp = true;
		}
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
		else if (state == eState.Wait)
		{
			int charaIndex = arrow.charaIndex;
			if (bobblenShotCount_ <= 0)
			{
				setNextTap(true);
			}
			if (charaIndex != 0)
			{
				updateChangeBubbleBobblen();
			}
			bubbleNavi.startNavi(searchedBubbleList, nextBubbles[charaIndex].GetComponent<Bubble_Boss>().type);
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
		Network.Avatar avatar = GlobalData.Instance.currentAvatar;
		ceilingBaseY = Mathf.Ceil(Mathf.Round(NGUIUtilScalableUIRoot.GetOffsetY(true).y) / 52f) * 52f;
		GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		StageDataTable stageTbl = dataTable.GetComponent<StageDataTable>();
		GlobalData.Instance.ignoreLodingIcon = true;
		NetworkMng.Instance.forceIconDisable(true);
		int downloadCount = 1;
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 4, true, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 5, true, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 6, true, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 8, true, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.BG, 7, true, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.BG, 8, true, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.ThrowChara, 0, true, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.SupportChara, 0, true, 0, downloadCount));
		int throwChara = avatar.throwCharacter;
		int supportChara = avatar.supportCharacter;
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.ThrowChara, 0 + throwChara, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.SupportChara, 0 + supportChara, 0, downloadCount));
		NetworkMng.Instance.forceIconDisable(false);
		GlobalData.Instance.ignoreLodingIcon = false;
		bubbleNavi = base.gameObject.AddComponent<BubbleNavi>();
		if (args.ContainsKey("BossType"))
		{
			bossType = int.Parse(args["BossType"].ToString());
		}
		if (args.ContainsKey("BossLevel"))
		{
			bossLevel = int.Parse(args["BossLevel"].ToString());
		}
		BossStageInfo.LevelInfo lvInfo = stageTbl.getBossLevelData(bossType, bossLevel);
		LINE_DOWN_SEC = lvInfo.LineDownSec;
		USE_RANDOM_COLOR = lvInfo.UseColorNum;
		ALL_BREAK_RECREATE_TIME = lvInfo.RecreateBubbleSec;
		MOVE_SPEED = (float)lvInfo.MoveSpeed / 1000f;
		ATTACK_SPAN = lvInfo.AttackSpan;
		stageNo = Constant.Boss.convBossInfoToNo(bossType, 0);
		bossStageInfo = stageTbl.getBossInfo(bossType);
		bossLevelInfo = stageTbl.getBossLevelData(bossType, bossLevel);
		stageInfo = bossStageInfo.Common;
		if (bossType == 0 && bossLevel == 1)
		{
			stageInfo.StageItemNum = 0;
		}
		else
		{
			stageInfo.StageItemNum = stageInfo.StageItems.Length;
		}
		int bgNo = stageInfo.Bg;
		uiScale = (UnityEngine.Object.FindObjectOfType(typeof(UIRoot)) as UIRoot).transform.localScale.x;
		bubble_trail_eff = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "bubble_trail_eff")) as GameObject;
		bubble_trail_eff.name = "bubble_trail_eff";
		bubble_trail_offset = bubble_trail_eff.transform.localPosition;
		Utility.setParent(bubble_trail_eff, uiRoot.transform, false);
		bubble_trail_eff.SetActive(false);
		stagePause = base.gameObject.AddComponent<StagePause_Boss>();
		helpDataList = null;
		GameObject stageCollider = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "StageCollider")) as GameObject;
		Utility.setParent(stageCollider, uiRoot.transform, true);
		Transform ceilingCollider = stageCollider.transform.Find("Ceiling");
		ceilingCollider.localPosition += NGUIUtilScalableUIRoot.GetOffsetY(true);
		UnityEngine.Object bgResource2 = null;
		bgResource2 = ResourceLoader.Instance.loadGameObject("Prefabs/", "Stage_Boss_" + bgNo.ToString("00"));
		GameObject bg = UnityEngine.Object.Instantiate(bgResource2) as GameObject;
		Utility.setParent(bg, uiRoot.transform, true);
		stageBg = bg.transform;
		GameObject ui = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "BossStage_ui")) as GameObject;
		setupButton(ui, true);
		Utility.setParent(ui, uiRoot.transform, true);
		stageUi = ui.transform;
		frontUi = stageUi.Find("Front_ui");
		frontUi.parent = stageUi.parent;
		frontUi.localPosition += Vector3.back;
		deadLine = frontUi.Find("deadline_line_eff").gameObject;
		deadLine.transform.localPosition = deadLinePos;
		sweatEff = frontUi.Find("deadline_chara_eff").gameObject;
		sweatEff.SetActive(false);
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
			for (int n = 0; n < stageClearStars.Length; n++)
			{
				if (stageClearStars[n].name.StartsWith("star_set"))
				{
					UnityEngine.Object.Destroy(stageClearStars[n].gameObject);
				}
			}
		}
		stageClear.AddComponent<UIPanel>();
		stageClear.SetActive(false);
		coinRoot = frontUi.Find("Top_ui/clear_condition_top/00/clearscore");
		scoreList.Add(coinRoot.Find("score_number"));
		scoreList[0].name = "01";
		setupPopupExcellent("excellent", ref popupExcellent);
		specialbubble_26_eff = frontUi.Find("specialbubble_26_eff").gameObject;
		Boss_02_egg = frontUi.Find("Boss_02_egg").gameObject;
		Boss_02_nest = frontUi.Find("Boss_02_nest").gameObject;
		if (bossType == 3)
		{
			Boss_03_spiderweb = frontUi.Find("Boss_03_spiderweb").gameObject;
			Spiderweb spw = Boss_03_spiderweb.GetComponent<Spiderweb>();
			spw.stagePause_ = stagePause;
			spw.part_ = this;
		}
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
		yield return StartCoroutine(characterInstantiate());
		nextBubbleBobllenBefor_Y = nextBubblePoses[1].transform.localPosition.y;
		arrow = launchpad.Find("arrow_pivot").GetComponent<Arrow_Boss>();
		arrow.bossPart = this;
		arrow.bubblen = stageUi.Find(charaNames[0]).GetComponentInChildren<tk2dAnimatedSprite>();
		stepNextBubbleAnim = launchpad.GetComponent<Animation>();
		stepNextBubbleAnim.enabled = true;
		stepNextBubbleAnim.playAutomatically = false;
		stepNextBubbleAnim.Stop();
		stepNextBubbleAnim[stepNextBubbleClipName].clip.SampleAnimation(stepNextBubbleAnim.gameObject, 0f);
		next_tap = launchpad.Find("next_tap").gameObject;
		setNextTap(false);
		guide = base.gameObject.AddComponent<Guide_Boss>();
		guide.bossPart = this;
		guide.guideline_pos = launchpad.Find("arrow_pivot/guideline_pos").gameObject;
		arrow.guide = guide;
		Vector3 temp_ceiling_pos = ceilingCollider.localPosition;
		temp_ceiling_pos.y -= 90f;
		ceilingCollider.localPosition = temp_ceiling_pos;
		guide.setCeilingPos(ceilingCollider.position);
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
		TextAsset dataText2 = null;
		dataText2 = Resources.Load("Parameter/BossStageData/" + Constant.Boss.convBossInfoToNo(bossType, bossLevel - 1), typeof(TextAsset)) as TextAsset;
		if (dataText2 == null)
		{
			dataText2 = Resources.Load("Parameter/BossStageData/30001", typeof(TextAsset)) as TextAsset;
		}
		stageData = Xml.DeserializeObject<StageData>(dataText2.text) as StageData;
		int lineNum = stageData.lineNum + 1;
		GameObject bubbleRootObj = new GameObject("BubbleRoot");
		bubbleRootObj.AddComponent<UIPanel>();
		bubbleRoot = bubbleRootObj.transform;
		Utility.setParent(bubbleRootObj, uiRoot.transform, true);
		int lineStart = 6;
		if (lineStart > lineNum)
		{
			lineStart = lineNum;
		}
		bubbleRoot.localPosition = new Vector3(bubbleRootPos.x, bubbleRootPos.y, -0.1f);
		GameObject baseBubbleRootObj = new GameObject("BaseBubbleRoot");
		baseBubbleRootObj.AddComponent<UIPanel>();
		baseBubbleRoot = baseBubbleRootObj.transform;
		Utility.setParent(baseBubbleRootObj, uiRoot.transform, true);
		baseBubbleRoot.localPosition = bubbleRoot.localPosition;
		yield return null;
		GameObject nextBubbleRootObj = new GameObject("NextBubbleRoot");
		nextBubbleRoot = nextBubbleRootObj.transform;
		Utility.setParent(nextBubbleRootObj, uiRoot.transform, true);
		nextBubbleRoot.localPosition = new Vector3(bubbleRootPos.x, bubbleRootPos.y, -0.1f);
		float offset_y = 52 * lineNum - 52 * lineStart - 208;
		GameObject bossObjectRootObj = new GameObject("BossObjectRoot");
		bossObjectRoot = bossObjectRootObj.transform;
		Utility.setParent(bossObjectRootObj, uiRoot.transform, true);
		bossObjectRoot.localPosition = new Vector3(bossObjectRootPos.x, bossObjectRootPos.y, -0.1f);
		bubbleObject = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "spr_bubble_boss")) as GameObject;
		Utility.setParent(bubbleObject, base.transform, false);
		bubbleObject.SetActive(false);
		Bubble_Boss b = bubbleObject.GetComponent<Bubble_Boss>();
		b.bossPart_ = this;
		b.stagePause_ = stagePause;
		bossObj = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Boss_" + bossType.ToString("00"))) as GameObject;
		Utility.setParent(bossObj, uiRoot.transform, false);
		bossBase_ = bossObj.GetComponent<BossBase>();
		bossBase_.stagePause_ = stagePause;
		bossBase_.stageUi = stageUi;
		bossBase_.frontUi = frontUi;
		bossBase_.bubbleRoot = bubbleRoot;
		bossBase_.bossPart_ = this;
		bossSound_ = bossObj.GetComponentInChildren<BossSoundBase>();
		if (bossSound_ != null)
		{
			bossSound_.bPlay = false;
		}
		hitEff = frontUi.Find("Boss_eff_hit").gameObject;
		guardEff = frontUi.Find("Boss_eff_guard").gameObject;
		clearAnim = frontUi.Find("stage_clear").gameObject;
		drank_ojama = frontUi.Find("Boss_drank_ojama").gameObject;
		drank_start = frontUi.Find("Boss_drank_start").gameObject;
		drankSprite = drank_ojama.transform.Find("root/drank/bg00").gameObject.GetComponent<UISpriteAnimation>();
		drankSprite.loop = false;
		drankLight = drank_ojama.transform.Find("root/light").gameObject;
		drankLight.transform.Find("bg01").gameObject.GetComponent<TweenAlpha>().style = UITweener.Style.Once;
		hitEff.AddComponent<UIPanel>();
		guardEff.AddComponent<UIPanel>();
		chacknBase = stageUi.Find("chara_02").gameObject;
		Transform material_root = b.transform.Find("AS_spr_bubble");
		gameoverMat = new Material(material_root.GetComponent<tk2dAnimatedSprite>().GetComponent<Renderer>().material);
		gameoverMat.shader = Shader.Find("Custom/CustomGrayscale");
		nearCoinBubbleList_.Clear();
		fulcrumIndexList.Clear();
		baseBubbleList.Clear();
		List<int> preColorList = new List<int>();
		preColorList.Clear();
		for (int m = 0; m < 7; m++)
		{
			preColorList.Add(m);
		}
		useRandomColorList.Clear();
		for (int l = 0; l < USE_RANDOM_COLOR; l++)
		{
			int type = preColorList[random.Next(preColorList.Count)];
			useRandomColorList.Add(type);
			preColorList.Remove(type);
		}
		bossBase_.colorList = useRandomColorList;
		bossBase_.SetupBoss(bossLevel, MOVE_SPEED, ATTACK_SPAN);
		for (int j2 = 0; j2 < lineNum; j2++)
		{
			int offset = ((j2 % 2 == 0) ? 30 : 0);
			for (int i = 0; i < 10; i++)
			{
				if (j2 % 2 == 0 && i == 9)
				{
					continue;
				}
				if (stageData.eggRow != null)
				{
					for (int k4 = 0; k4 < stageData.eggRow.Length; k4++)
					{
						if (stageData.eggRow[k4] == j2 - 1 && stageData.eggColumn[k4] == i)
						{
							GameObject egg = UnityEngine.Object.Instantiate(Boss_02_egg) as GameObject;
							Utility.setParent(egg, bubbleRoot.parent, true);
							Vector3 tempPos3 = new Vector3(i * 60 + offset, (float)(j2 * -52) + offset_y, 0f);
							egg.transform.localPosition = tempPos3 + bubbleRoot.transform.localPosition;
							egg.SetActive(true);
							eggObj = egg;
							egg_ = egg.GetComponent<Egg>();
							egg_.part_ = this;
							egg_.stagePause_ = stagePause;
							egg_.bossBase_ = bossBase_;
							StartCoroutine(egg_.EggInit());
							b.egg_ = egg_;
							bossBase_.egg_ = egg_;
						}
					}
				}
				if (stageData.nestRow != null)
				{
					for (int k3 = 0; k3 < stageData.nestRow.Length; k3++)
					{
						if (stageData.nestRow[k3] == j2 - 1 && stageData.nestColumn[k3] == i)
						{
							GameObject nest = UnityEngine.Object.Instantiate(Boss_02_nest) as GameObject;
							Utility.setParent(nest, bubbleRoot.parent, true);
							Vector3 tempPos2 = new Vector3(i * 60 + offset, (float)(j2 * -52) + offset_y, 0f);
							nest.transform.localPosition = tempPos2 + bubbleRoot.transform.localPosition;
							nest.SetActive(true);
							nest_ = nest.GetComponent<Nest>();
							nest_.stagePause_ = stagePause;
							nest_.part_ = this;
							StartCoroutine(nest_.NestInit());
							b.nest_ = nest_;
						}
					}
				}
				if (isSpiderStage())
				{
					for (int k2 = 0; k2 < stageData.spiderwebRow.Length; k2++)
					{
						if (stageData.spiderwebRow[k2] == j2 - 1 && stageData.spiderwebColumn[k2] == i)
						{
							GameObject spiderweb = UnityEngine.Object.Instantiate(Boss_03_spiderweb) as GameObject;
							Utility.setParent(spiderweb, bossObjectRoot, true);
							Vector3 tempPos = new Vector3(i * 60 + offset, (float)(j2 * -52) + offset_y, 0f);
							spiderweb.transform.localPosition = tempPos;
							spiderweb.SetActive(true);
							StartCoroutine(spiderweb.GetComponent<Spiderweb>().SpiderwebInit());
							spwList.Add(spiderweb.GetComponent<Spiderweb>());
						}
					}
					if (spwList.Count > 0)
					{
						spiderwebLine_y = spwList[0].transform.localPosition.y;
					}
				}
				GameObject obj;
				Bubble_Boss bubble;
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
					bubble = obj.GetComponent<Bubble_Boss>();
					bubble.init();
					bubble.createIndex = index;
					bubble.bossBase = bossBase_;
					Bubble.eType type6 = bubble.type;
					if (type6 <= Bubble.eType.Black)
					{
						if (!glossColorList.Contains(type6))
						{
							glossColorList.Add(type6);
						}
					}
					else if (type6 >= Bubble.eType.PlusRed && type6 <= Bubble.eType.PlusBlack)
					{
						type6 -= 13;
						if (!glossColorList.Contains(type6))
						{
							glossColorList.Add(type6);
						}
					}
					else if (type6 >= Bubble.eType.MinusRed && type6 <= Bubble.eType.MinusBlack)
					{
						type6 -= 21;
						if (!glossColorList.Contains(type6))
						{
							glossColorList.Add(type6);
						}
					}
					else if (type6 >= Bubble.eType.FriendRed && type6 <= Bubble.eType.FriendBlack)
					{
						type6 -= 31;
						if (!glossColorList.Contains(type6))
						{
							glossColorList.Add(type6);
						}
						if (!clearChacknList.Contains(type6))
						{
							clearChacknList.Add(type6);
						}
					}
					else if (type6 >= Bubble.eType.SnakeRed && type6 <= Bubble.eType.SnakeBlack)
					{
						type6 -= 67;
						if (!glossColorList.Contains(type6))
						{
							glossColorList.Add(type6);
						}
					}
					else
					{
						switch (type6)
						{
						case Bubble.eType.Skull:
							isInvalidReplay = false;
							break;
						case Bubble.eType.FriendRainbow:
							if (!clearChacknList.Contains(Bubble.eType.FriendRainbow))
							{
								clearChacknList.Add(Bubble.eType.FriendRainbow);
							}
							break;
						case Bubble.eType.FriendBox:
							if (!clearChacknList.Contains(Bubble.eType.FriendBox))
							{
								clearChacknList.Add(Bubble.eType.FriendBox);
							}
							break;
						case Bubble.eType.Coin:
							nearCoinBubbleList_.Add(bubble);
							break;
						case Bubble.eType.Unknown:
						{
							for (int x = 0; x < stageData.skeltonIndex.Length; x++)
							{
								if (index == stageData.skeltonIndex[x])
								{
									bubble.unknownColor = stageData.skeltonColor[x];
									break;
								}
							}
							break;
						}
						case Bubble.eType.Fulcrum:
						case Bubble.eType.RotateFulcrumR:
						case Bubble.eType.RotateFulcrumL:
						case Bubble.eType.FriendFulcrum:
							if (isSpiderStage())
							{
								bubble.createIndex = j2;
								fulcrumIndexList.Add(bubble.createIndex);
							}
							else
							{
								fulcrumIndexList.Add(bubble.createIndex);
							}
							break;
						}
					}
				}
				else
				{
					obj = UnityEngine.Object.Instantiate(bubbleObject) as GameObject;
					obj.SetActive(true);
					obj.name = "99";
					bubble = obj.GetComponent<Bubble_Boss>();
					bubble.init();
					bubble.setCeiling(i == 0);
				}
				Transform trans = obj.transform;
				trans.parent = bubbleRoot;
				trans.localScale = Vector3.one;
				trans.localPosition = new Vector3(i * 60 + offset, (float)(j2 * -52) + offset_y, 0f);
				bubble.setFieldState();
				fieldBubbleList.Add(bubble);
				if (j2 > 0)
				{
					GameObject temp_obj = UnityEngine.Object.Instantiate(obj) as GameObject;
					Utility.setParent(temp_obj, baseBubbleRoot, false);
					Bubble_Boss basebubble = temp_obj.GetComponent<Bubble_Boss>();
					if (isSpiderStage())
					{
						basebubble.createIndex = j2;
					}
					baseBubbleList.Add(basebubble);
					temp_obj.SetActive(false);
				}
				if (j2 == 0)
				{
					ceilingBubbleList.Add(trans);
				}
			}
		}
		List<int> temp = new List<int>();
		temp.Clear();
		foreach (int ID in fulcrumIndexList)
		{
			if (!temp.Contains(ID))
			{
				temp.Add(ID);
			}
		}
		int count = temp.Count;
		fulcrumBrokenDatas = new FulcrumBrokenData[count];
		for (int k = 0; k < fulcrumBrokenDatas.Length; k++)
		{
			fulcrumBrokenDatas[k] = new FulcrumBrokenData();
			fulcrumBrokenDatas[k].isBroken = false;
			fulcrumBrokenDatas[k].createIndex = temp[k];
			fulcrumBrokenDatas[k].brokenTime = -1f;
		}
		Ceiling ceiling = frontUi.Find("stage_ceiling").gameObject.AddComponent<Ceiling>();
		ceiling.transform.position = ceilingCollider.position;
		temp_ceiling_pos.y += 60f;
		ceiling.transform.localPosition = temp_ceiling_pos;
		fadeMaterial = new Material(fieldBubbleList[0].transform.Find("AS_spr_bubble").GetComponent<Renderer>().sharedMaterial);
		fadeMaterial.shader = Shader.Find("Unlit/Transparent Colored");
		Transform bossInfoBoard = frontUi.Find("Top_ui/Boss_gauge/board");
		bossInfoBoard.Find("BossName").GetComponent<UILabel>().text = string.Empty;
		bossInfoBoard.Find("Lv").GetComponent<UILabel>().text = string.Empty;
		bossInfoBoard.Find("level").GetComponent<UILabel>().text = string.Empty;
		bossInfoBoard.Find("BossName").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(8200 + bossType);
		bossInfoBoard.Find("Lv").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(22);
		bossInfoBoard.Find("level").GetComponent<UILabel>().text = bossLevel.ToString();
		updateTotalScoreDisp();
		updateFieldBubbleList();
		if (isSpiderStage())
		{
			shuffleBubbleType(fieldBubbleList);
		}
		else
		{
			foreach (Bubble_Boss temp_bubble in fieldBubbleList)
			{
				if (temp_bubble.type == Bubble.eType.Fulcrum || temp_bubble.type == Bubble.eType.RotateFulcrumR || temp_bubble.type == Bubble.eType.RotateFulcrumL || temp_bubble.type == Bubble.eType.FriendFulcrum)
				{
					List<Bubble_Boss> list2 = new List<Bubble_Boss>();
					list2.Clear();
					checkConnect(temp_bubble, list2, fieldBubbleList);
					shuffleBubbleType(list2);
				}
			}
		}
		if (isSpiderStage())
		{
			foreach (List<Bubble_Boss> list in sortLine(selectForBossSpider()))
			{
				changeStarBubble(list);
			}
		}
		lineFriendBase = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Player_icon")) as GameObject;
		lineFriendBase.transform.Find("frame").localPosition = Vector3.back * 0.03f;
		lineFriendBase.transform.Find("Player_icon").localPosition = Vector3.back * 0.02f;
		Utility.setParent(lineFriendBase, uiRoot.transform, false);
		defaultUserIconTexture = lineFriendBase.GetComponentInChildren<UITexture>().mainTexture;
		lineFriendBase.SetActive(false);
		yield return StartCoroutine(dialogManager.load(PartManager.ePart.BossStage));
		GameObject scrollUiObj = new GameObject("ScrollUI");
		Utility.setParent(scrollUiObj, uiRoot.transform, true);
		scrollUi = scrollUiObj.transform;
		bubbleRoot.parent = scrollUi;
		baseBubbleRoot.parent = scrollUi;
		nextBubbleRoot.parent = scrollUi;
		stageBg.parent = scrollUi;
		stageUi.parent = scrollUi;
		useItemParent_ = frontUi.Find("Bottom_ui/show_boosts").GetComponent<StageUseBoostItemParent>();
		useItemParent_.setup();
		Dictionary<Constant.Item.eType, int> mapItemList_ = new Dictionary<Constant.Item.eType, int>();
		for (int j = 0; j < stageInfo.ItemNum; j++)
		{
			string typeKey = "item_" + j + "type";
			string numKey = "item_" + j + "num";
			if (args.ContainsKey(typeKey) && args.ContainsKey(numKey))
			{
				Constant.Item.eType item = (Constant.Item.eType)(int)args[typeKey];
				int num = (int)args[numKey];
				mapItemList_.Add(item, num);
				if (Constant.Item.IsAutoUse(item) && item != Constant.Item.eType.TimeStop && item != Constant.Item.eType.Vacuum)
				{
					useItem(item, num);
				}
			}
		}
		if (mapItemList_ != null)
		{
			Debug.Log(mapItemList_.Count);
		}
		Transform bottom = frontUi.Find("Bottom_ui");
		bottom.gameObject.SetActive(true);
		bottom.Find("boost_items").gameObject.SetActive(true);
		bottom.Find("combo").gameObject.SetActive(false);
		bottom.Find("scoregauge").gameObject.SetActive(false);
		bottom.Find("useitem_bg").gameObject.SetActive(false);
		itemParent_ = bottom.Find("boost_items").GetComponent<StageBoostItemParent>();
		if (itemParent_ != null && stageInfo != null)
		{
			itemParent_.setup(stageInfo, mapItemList_);
		}
		itemParent_.disable();
		scoreParticleBase = frontUi.Find("erase_eff").gameObject;
		if (ResourceLoader.Instance.isUseLowResource())
		{
			UnityEngine.Object.Destroy(scoreParticleBase.transform.Find("particle").gameObject);
		}
		Debug.Log("stageNo = " + stageNo);
		TutorialManager.Instance.load(stageNo - 1, uiRoot);
		searchNextBubble();
		if (isSpiderStage())
		{
			InitInSpwBubbles();
			yield return StartCoroutine(SetSpiderStage());
		}
		Sound.Instance.playBgm(Sound.eBgm.BGM_004_Boss, true);
		yield return stagePause.sync();
		startTime = Time.time;
		shotCount = 0;
		StartCoroutine(startRoutine(stageInfo, args));
	}

	private void setupPopupScore(string popupName, ref PopupScore popupScore)
	{
		GameObject gameObject = frontUi.Find(popupName).gameObject;
		popupScore = gameObject.AddComponent<PopupScore>();
		gameObject.SetActive(false);
	}

	private void setupPopupExcellent(string popupName, ref PopupExcellent popupExcellent)
	{
		GameObject gameObject = frontUi.Find(popupName).gameObject;
		popupExcellent = gameObject.AddComponent<PopupExcellent>();
		gameObject.SetActive(false);
	}

	private IEnumerator startRoutine(StageInfo.CommonInfo stageInfo, Hashtable args)
	{
		yield return StartCoroutine(playOPAnime());
		Transform bossInfoBoard = stageUi.Find("launchpad/board");
		bossInfoBoard.Find("BossName").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(8200 + bossType);
		bossInfoBoard.Find("Lv").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(22);
		bossInfoBoard.Find("level").GetComponent<UILabel>().text = bossLevel.ToString();
		setNextTap(true);
		if (gameType == eGameType.Time && createBlitzBubble())
		{
			updateFieldBubbleList();
		}
		yield return StartCoroutine(stepNextBubble());
		yield return StartCoroutine(StartInDrankEff(drank_start));
		StartCoroutine(StartOutDrankEff(drank_start));
		yield return StartCoroutine(bossBase_.StartAnimation());
		readyGo.gameObject.SetActive(true);
		yield return StartCoroutine(readyGo.play(stagePause));
		UnityEngine.Object.Destroy(readyGo);
		BossDataTable bossTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<BossDataTable>();
		BossListData bossListData = bossTbl.getBossData();
		if (bossListData.bossList[bossStageInfo.BossInfo.BossType].bossLevelList[0].playCount == 1 && TutorialManager.Instance.isTutorial(stageNo - 1, TutorialDataTable.ePlace.ReadyGoEnd))
		{
			stagePause.pause = true;
			tutorialStart();
			yield return StartCoroutine(TutorialManager.Instance.play(stageNo - 1, TutorialDataTable.ePlace.ReadyGoEnd, uiRoot, stageInfo, args));
			tutorialEnd();
			DialogBase dialogQuit2 = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
			while (dialogQuit2.isOpen())
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
			foreach (Bubble_Boss coin_bubble2 in nearCoinBubbleList_)
			{
				coin_bubble2.myTrans.localPosition += Vector3.back * 50f;
			}
			yield return StartCoroutine(TutorialManager.Instance.play(-8, TutorialDataTable.ePlace.ReadyGoEnd, uiRoot, null, null));
			foreach (Bubble_Boss coin_bubble in nearCoinBubbleList_)
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
		state = eState.Wait;
		if (bossSound_ != null)
		{
			bossSound_.bPlay = true;
		}
		showShootButton();
		scrollTime = Time.time;
		scrollTimeBefor = Time.time;
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
		for (int l = 1; l < nextBubbleCount; l++)
		{
			createNextBubble(l, false);
		}
		for (int k = 1; k < nextBubbleCount; k++)
		{
			Slave slave = nextBubbles[k].AddComponent<Slave>();
			slave.target = nextBubblePoses[k];
		}
		Slave s = launchpad_.gameObject.AddComponent<Slave>();
		s.target = launch_pos;
		float scrollY = 0f;
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
		while (bOPAnime_ && elapsedTime3 < waitTime2)
		{
			elapsedTime3 += Time.deltaTime;
			yield return stagePause.sync();
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
		if (!bossBase_.isWaitBossAction && !isWaitScroll)
		{
			arrow.shootAnim();
			bubbleNavi.stopNavi();
			Bubble_Boss component = nextBubbles[0].GetComponent<Bubble_Boss>();
			prevShotBubbleIndex = 0;
			if (bobblenShotCount_ > 0)
			{
				component = nextBubbles[1].GetComponent<Bubble_Boss>();
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
			component.bossBase = bossBase_;
			component.shot(shootVector);
			state = eState.Shot;
		}
	}

	private void shootRandom()
	{
		if (!bossBase_.isWaitBossAction)
		{
			bubbleNavi.stopNavi();
			charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "10");
			charaAnims[0].Play(CHARA_SPRITE_ANIMATION_HEADER[0] + "09");
			Bubble_Boss component = nextBubbles[1].GetComponent<Bubble_Boss>();
			prevShotBubbleIndex = 0;
			Vector3 localPosition = component.transform.localPosition;
			localPosition.z = -5f;
			component.myTrans.localPosition = localPosition;
			component.guide = guide;
			Sound.Instance.playSe(Sound.eSe.SE_400_metalbubble_shot);
			bubble_trail_eff.SetActive(false);
			Utility.setParent(bubble_trail_eff, component.myTrans, false);
			bubble_trail_eff.transform.localPosition = bubble_trail_offset;
			bubble_trail_eff.SetActive(true);
			component.shot(random_vec);
			state = eState.Shot;
			shotCount++;
			stageEnd_ShotCount++;
		}
	}

	private IEnumerator setRandomRoots()
	{
		Bubble_Boss shotBubble = nextBubbles[1].GetComponent<Bubble_Boss>();
		random_vec = Vector3.zero;
		BubbleBase hitBubble = null;
		int resarch_count = 0;
		while (hitBubble == null)
		{
			random_vec = arrow.getRandomFireVector();
			hitBubble = guide.setRandomRoot(random_vec, shotBubble.transform.position);
			yield return stagePause.sync();
			Bubble_Boss temp_b = nextBubbles[1].GetComponent<Bubble_Boss>();
			Vector3 tem_vec = new Vector3(nextBubbles[1].transform.localPosition.x, nextBubbles[1].transform.localPosition.y, nextBubbles[1].transform.localPosition.z);
			temp_b.myTrans.position = guide.hitPos;
			List<Bubble.eType> nearBubbleTypeList = new List<Bubble.eType>(6);
			for (int i = 0; i < fieldBubbleList.Count; i++)
			{
				Bubble_Boss bubble = fieldBubbleList[i];
				if (bubble.state == Bubble.eState.Field && !bubble.isLocked && !bubble.isFrozen && temp_b.isNearBubble(bubble))
				{
					nearBubbleTypeList.Add(bubble.type);
				}
			}
			temp_b.myTrans.localPosition = tem_vec;
			if (hitBubble.type == Bubble.eType.Skull || hitBubble.type == Bubble.eType.Honeycomb || nearBubbleTypeList.Contains(Bubble.eType.Search))
			{
				hitBubble = null;
			}
			else
			{
				if (resarch_count >= 10)
				{
					continue;
				}
				Bubble_Boss hit_bubble = hitBubble.gameObject.GetComponent<Bubble_Boss>();
				if (hit_bubble != null && !hit_bubble.isColorBubble())
				{
					hitBubble = null;
					resarch_count++;
					continue;
				}
				Vector3 temp = nextBubbles[1].transform.position;
				shotBubble.myTrans.position = guide.hitPos;
				int[] checkedFlags = new int[fieldBubbleList.Count];
				List<Bubble_Boss> rainbowList = new List<Bubble_Boss>();
				checkSamColor(shotBubble, ref checkedFlags, rainbowList);
				int chainCount = 0;
				int[] array = checkedFlags;
				foreach (int checkedFlag in array)
				{
					if (checkedFlag == 1)
					{
						chainCount++;
					}
				}
				if (rainbowChainCount == 0)
				{
					chainCount++;
				}
				shotBubble.myTrans.position = temp;
				if (chainCount >= 3)
				{
					hitBubble = null;
					resarch_count++;
				}
			}
		}
	}

	public void hit(Bubble_Boss shotBubble, BubbleBase hitBubble)
	{
		if (shotBubble.type != Bubble.eType.Metal)
		{
			if (bobblenShotCount_ > 0)
			{
				if (sweatEff.activeSelf)
				{
					charaAnims[1].Play(waitPinchAnimName);
				}
				else
				{
					charaAnims[1].Play(waitAnimName);
				}
			}
			else if (sweatEff.activeSelf)
			{
				charaAnims[0].Play(waitPinchAnimName);
			}
			else
			{
				charaAnims[0].Play(waitAnimName);
			}
		}
		StartCoroutine(hitRoutine(shotBubble, hitBubble));
	}

	private IEnumerator hitRoutine(Bubble_Boss shotBubble, BubbleBase hitBubble)
	{
		moveSpan = 0f;
		rainbowChainCount = 0;
		if (bossStageInfo.BossInfo.BossType == 0)
		{
			yield return StartCoroutine(SetNativeSpriteRoutine(shotBubble));
		}
		shotBubblePos = shotBubble.myTrans.position;
		bSearch = false;
		if (hitBubble.myTrans.localPosition.y >= spiderwebLine_y)
		{
			shotBubble.isStay = true;
		}
		if (hitBubble != shotBubble)
		{
			if (!isSpiderStage())
			{
				yield return StartCoroutine(hitDefault(shotBubble));
			}
			else if (!isNearSpiderweb(hitBubble))
			{
				yield return StartCoroutine(hitDefault(shotBubble));
			}
			else
			{
				shotBubble.startBreak();
			}
		}
		else if (guide.isSpw && shotBubble.isDiffendBoss)
		{
			Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
			shotBubble.startBreak(true);
			StartCoroutine(GuardEffPlay());
			float trail_wait = 0f;
			while (trail_wait < 0.5f)
			{
				trail_wait += Time.deltaTime;
				yield return stagePause.sync();
			}
		}
		else if (guide.isSpw && !shotBubble.isHitBoss)
		{
			Debug.Log("isSpw = true!");
			if (bossBase_.state == BossBase.eState.moving)
			{
				pickingTargetBubble = shotBubble;
				bossBase_.state = BossBase.eState.picking;
				bossBase_.isMoving = false;
				yield return StartCoroutine(hitSpiderweb(shotBubble));
			}
			else
			{
				shotBubble.startBreak(true);
			}
		}
		else if (shotBubble != null)
		{
			bool isHitBoss = shotBubble.isHitBoss;
			bool isDiffendBoss = shotBubble.isDiffendBoss;
			bool isHitBossJointBubble = shotBubble.isBossJointBubble;
			bool isHitEgg = guide.isEgg;
			bool isHitNest = guide.isNest;
			Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
			shotBubble.startBreak(true);
			if (isHitBoss)
			{
				StartCoroutine(HitEffPlay());
			}
			else if (isHitBossJointBubble)
			{
				StartCoroutine(bossBase_.Breaking());
			}
			else if (isDiffendBoss)
			{
				StartCoroutine(GuardEffPlay());
			}
			else if (isHitEgg)
			{
				StartCoroutine(egg_.HitEgg());
			}
			float trail_wait2 = 0f;
			while (trail_wait2 < 0.5f)
			{
				trail_wait2 += Time.deltaTime;
				yield return stagePause.sync();
			}
		}
		if (shotBubble != null && nest_ != null && nest_.childList.Count > 0)
		{
			Vector3 diff_2 = Vector3.zero;
			float near = 7200f;
			foreach (GameObject nest in nest_.childList)
			{
				diff_2 = (shotBubble.myTrans.position - nest.transform.position) / uiScale;
				diff_2.z = 0f;
				float sqrMagnitude = diff_2.sqrMagnitude;
				if (sqrMagnitude > 3600f || !(sqrMagnitude < near))
				{
					continue;
				}
				shotBubble.startBreak();
				updateFieldBubbleList();
				break;
			}
		}
		if (bobblenShotCount_ > 0)
		{
			bobblenShotCount_--;
			if (bobblenShotCount_ <= 0)
			{
				setNextTap(true);
				UnityEngine.Object.Destroy(nextBubbles[0]);
			}
		}
		updateShootCharacter(false);
		if (isSpiderStage())
		{
			yield return StartCoroutine(updateSpiderweb());
		}
		if (bSearch)
		{
			yield return StartCoroutine(searchBubbleRoutine());
		}
		searchNextBubble();
		updateBuyItem();
		if (arrow.charaIndex == 0)
		{
			yield return StartCoroutine(stepNextBubble());
		}
		else
		{
			yield return StartCoroutine(stepNextBubbleBobblen());
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
		sweatCheck();
		if (bGameOver)
		{
			gameoverType = eGameover.DeadLineOver;
			Debug.Log("lineover");
			StartCoroutine(gameoverRoutine());
			yield break;
		}
		while (bossBase_.isWaitBossAction)
		{
			yield return stagePause.sync();
		}
		if (bossBase_.currentHP <= 0)
		{
			StartCoroutine(clearRoutine());
		}
		else
		{
			state = eState.Wait;
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

	private IEnumerator hyperRoutine(Bubble_Boss shotBubble)
	{
		Sound.Instance.playSe(Sound.eSe.SE_339_hyperbubble);
		shotBubble.startBreak();
		bool bSearch = false;
		breakLightningCount_ = 0;
		if (breakFrozen(shotBubble))
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
		List<Bubble_Boss> nearBubbleList = new List<Bubble_Boss>(6);
		Bubble_Boss shotBubble2 = default(Bubble_Boss);
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (!fieldBubble.isLocked && fieldBubble.type != Bubble.eType.Blank && fieldBubble.type != Bubble.eType.Fulcrum && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fieldBubble.type != Bubble.eType.FriendFulcrum && fieldBubble.type != Bubble.eType.FriendBox && fieldBubble.type != Bubble.eType.Rock && fieldBubble.type != Bubble.eType.Box && fieldBubble.state == Bubble.eState.Field && shotBubble2.isNearBubble(fieldBubble))
			{
				nearBubbleList.Add(fieldBubble);
			}
		});
		foreach (Bubble_Boss breakBubble2 in nearBubbleList)
		{
			if (!breakBubble2.isColorBubble())
			{
				continue;
			}
			List<Bubble_Boss> rainbowList = new List<Bubble_Boss>();
			int[] checkedFlags = new int[fieldBubbleList.Count];
			checkSamColor(breakBubble2, ref checkedFlags, rainbowList);
			for (int j = 0; j < fieldBubbleList.Count; j++)
			{
				if (checkedFlags[j] == 1)
				{
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
		foreach (Bubble_Boss breakBubble in nearBubbleList)
		{
			switch (breakBubble.type)
			{
			case Bubble.eType.Lightning:
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
				breakLightningCount_++;
				break;
			case Bubble.eType.Counter:
				if (breakBubble.isFrozen)
				{
					breakLightningCount_++;
				}
				break;
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
		yield return StartCoroutine(breakRoutine(shotBubble, breakLightningCount_));
		if (bSearch)
		{
			yield return StartCoroutine(searchBubbleRoutine());
		}
	}

	private IEnumerator bombRoutine(Bubble_Boss shotBubble)
	{
		Sound.Instance.playSe(Sound.eSe.SE_340_bombbubble);
		shotBubble.startBreak();
		bool bSearch = false;
		breakLightningCount_ = 0;
		if (breakFrozen(shotBubble))
		{
			bSearch = true;
		}
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble chainBubble in chainBubbleDic[i])
			{
				if (chainBubble.type != Bubble.eType.ChainLock || !shotBubble.isBombRangeBubble(chainBubble) || !unlockChain(chainBubble))
				{
					continue;
				}
				break;
			}
		}
		List<Bubble_Boss> nearBubbleList = new List<Bubble_Boss>(6);
		Bubble_Boss shotBubble2 = default(Bubble_Boss);
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (!fieldBubble.isLocked && fieldBubble.type != Bubble.eType.Blank && fieldBubble.type != Bubble.eType.Fulcrum && fieldBubble.type != Bubble.eType.FriendFulcrum && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fieldBubble.state == Bubble.eState.Field && shotBubble2.isBombRangeBubble(fieldBubble))
			{
				nearBubbleList.Add(fieldBubble);
			}
		});
		foreach (Bubble_Boss breakBubble in nearBubbleList)
		{
			switch (breakBubble.type)
			{
			case Bubble.eType.Lightning:
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
				breakLightningCount_++;
				break;
			case Bubble.eType.Grow:
			case Bubble.eType.Skull:
			case Bubble.eType.FriendBox:
			case Bubble.eType.Box:
			case Bubble.eType.Rock:
			case Bubble.eType.Honeycomb:
			case Bubble.eType.Counter:
				if (breakBubble.isFrozen)
				{
					breakLightningCount_++;
				}
				break;
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
				UnityEngine.Object.Destroy(breakBubble.gameObject);
			}
		}
		yield return StartCoroutine(breakRoutine(shotBubble, breakLightningCount_));
		if (bSearch)
		{
			yield return StartCoroutine(searchBubbleRoutine());
		}
	}

	private IEnumerator shakeRoutine(Bubble_Boss shotBubble)
	{
		Sound.Instance.playSe(Sound.eSe.SE_341_shakebubble);
		shotBubble.startBreak(false);
		bool bSearch = false;
		breakLightningCount_ = 0;
		if (breakFrozen(shotBubble))
		{
			bSearch = true;
		}
		for (int k = 0; k < chainBubbleDic.Count; k++)
		{
			foreach (ChainBubble chainBubble in chainBubbleDic[k])
			{
				if (chainBubble.type != Bubble.eType.ChainLock || !shotBubble.isNearBubble(chainBubble) || !unlockChain(chainBubble))
				{
					continue;
				}
				break;
			}
		}
		List<Bubble_Boss> nearList = new List<Bubble_Boss>(6);
		List<Bubble_Boss> nearBubbleList = new List<Bubble_Boss>(18);
		Bubble_Boss shotBubble2 = default(Bubble_Boss);
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (!fieldBubble.isLocked && fieldBubble.type != Bubble.eType.Blank && fieldBubble.type != Bubble.eType.Fulcrum && fieldBubble.type != Bubble.eType.FriendFulcrum && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fieldBubble.state == Bubble.eState.Field && shotBubble2.isShakeRangeBubble(fieldBubble))
			{
				nearBubbleList.Add(fieldBubble);
				if (shotBubble2.isNearBubble(fieldBubble))
				{
					nearList.Add(fieldBubble);
				}
			}
		});
		foreach (Bubble_Boss breakBubble3 in nearList)
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
		}
		nearBubbleList.RemoveAll((Bubble_Boss b) => b.state != Bubble.eState.Field);
		if (nearBubbleList.Count > 0)
		{
			iTween itween = null;
			foreach (Bubble_Boss breakBubble2 in nearBubbleList)
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
			for (int j = 0; j < nearBubbleCount; j++)
			{
				int p = 20 + random.Next(21);
				if (p > random.Next(100))
				{
					dropCount++;
				}
			}
			int delayCount = 0;
			for (int i = 0; i < dropCount; i++)
			{
				if (nearBubbleList.Count == 0)
				{
					break;
				}
				int index = random.Next(nearBubbleList.Count);
				Bubble_Boss breakBubble = nearBubbleList[index];
				breakBubble.startDrop(delayCount);
				delayCount++;
				nearBubbleList.RemoveAt(index);
			}
		}
		yield return StartCoroutine(breakRoutine(shotBubble, breakLightningCount_));
		if (bSearch)
		{
			yield return StartCoroutine(searchBubbleRoutine());
		}
	}

	private IEnumerator metalRoutine(Bubble_Boss shotBubble)
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
			if (t >= 1f)
			{
				pathIndex++;
				if (pathIndex >= path.Count - 1)
				{
					break;
				}
				move -= length;
				length = (path[pathIndex + 1] - path[pathIndex]).magnitude;
				t = move / length;
				Vector3 effPos = path[pathIndex];
				effPos.z = shotBubble.myTrans.position.z;
				StartCoroutine(boundEffRoutine(path[pathIndex - 1].x > path[pathIndex].x, effPos));
				Sound.Instance.playSe(Sound.eSe.SE_401_metalbubble_bound);
				shotBubble.boundCount++;
			}
			pos = Vector3.Lerp(path[pathIndex], path[pathIndex + 1], t);
			pos.z = shotBubble.myTrans.position.z;
			shotBubble.myTrans.position = pos;
			bPlus = false;
			bMinus = false;
			breakNum = 0;
			foreach (KeyValuePair<Bubble_Boss, float> pair2 in guide.metalBreakBubbleDic)
			{
				if (pair2.Value < distance)
				{
					Bubble_Boss bubble = pair2.Key;
					if (bubble == null || bubble.state != Bubble.eState.Field)
					{
						continue;
					}
					List<Bubble_Boss> breakBubbleList = new List<Bubble_Boss>();
					if (bubble.isFrozen)
					{
						checkFrozen(bubble, breakBubbleList);
					}
					else
					{
						breakBubbleList.Add(bubble);
					}
					foreach (Bubble_Boss breakBubble in breakBubbleList)
					{
						if (breakBubble == null || breakBubble.state != Bubble.eState.Field)
						{
							continue;
						}
						plusEffect(breakBubble);
						switch (breakBubble.type)
						{
						case Bubble.eType.Lightning:
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
							breakLightningCount_++;
							break;
						case Bubble.eType.Grow:
						case Bubble.eType.Skull:
						case Bubble.eType.FriendBox:
						case Bubble.eType.Box:
						case Bubble.eType.Rock:
						case Bubble.eType.Honeycomb:
						case Bubble.eType.Counter:
							if (breakBubble.isFrozen)
							{
								breakLightningCount_++;
							}
							break;
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
			if (breakNum > 0)
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
			yield return stagePause.sync();
		}
		pos = path[pathIndex];
		pos.z = shotBubble.myTrans.position.z;
		shotBubble.myTrans.position = pos;
		bPlus = false;
		bMinus = false;
		breakNum = 0;
		foreach (KeyValuePair<Bubble_Boss, float> item in guide.metalBreakBubbleDic)
		{
			Bubble_Boss breakBubble2 = item.Key;
			if (breakBubble2 == null || breakBubble2.state != Bubble.eState.Field)
			{
				continue;
			}
			plusEffect(breakBubble2);
			switch (breakBubble2.type)
			{
			case Bubble.eType.Lightning:
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
		if (breakNum > 0)
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
		bMetalShooting = false;
		if (bSearch)
		{
			yield return StartCoroutine(searchBubbleRoutine());
		}
	}

	private IEnumerator iceRoutine(Bubble_Boss shotBubble)
	{
		shotBubble.startBreak(false);
		bool bSearch = false;
		breakLightningCount_ = 0;
		if (breakFrozen(shotBubble))
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
		List<Bubble_Boss> nearBubbleList = new List<Bubble_Boss>(6);
		Bubble_Boss shotBubble2 = default(Bubble_Boss);
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (!fieldBubble.isLocked && !fieldBubble.isFrozen && fieldBubble.state == Bubble.eState.Field && shotBubble2.isNearBubble(fieldBubble))
			{
				nearBubbleList.Add(fieldBubble);
			}
		});
		foreach (Bubble_Boss breakBubble in nearBubbleList)
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
		}
		int frozenCount = 0;
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (!fieldBubble.isLocked && !fieldBubble.isFrozen && fieldBubble.type != Bubble.eType.Blank && fieldBubble.type != Bubble.eType.Fulcrum && fieldBubble.type != Bubble.eType.FriendFulcrum && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fieldBubble.state == Bubble.eState.Field && shotBubble2.isShakeRangeBubble(fieldBubble))
			{
				frozenCount++;
				fieldBubble.setFrozen(true);
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
		yield return StartCoroutine(breakRoutine(shotBubble, breakLightningCount_));
		if (bSearch)
		{
			yield return StartCoroutine(searchBubbleRoutine());
		}
	}

	private IEnumerator fireRoutine(Bubble_Boss shotBubble)
	{
		Sound.Instance.playSe(Sound.eSe.SE_511_fire_explosion);
		shotBubble.startBreak();
		bool bSearch = false;
		breakLightningCount_ = 0;
		if (breakFrozen(shotBubble))
		{
			bSearch = true;
		}
		float hitBubbleY = shotBubble.myTrans.localPosition.y;
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble chainBubble in chainBubbleDic[i])
			{
				if (chainBubble.type == Bubble.eType.ChainLock)
				{
					float fieldBubbleY = chainBubble.myTrans.localPosition.y;
					if (((fieldBubbleY > hitBubbleY - 1f && fieldBubbleY < hitBubbleY + 1f) || shotBubble.isNearBubble(chainBubble)) && unlockChain(chainBubble))
					{
						break;
					}
				}
			}
		}
		List<Bubble_Boss> breakBubbleList = new List<Bubble_Boss>();
		Bubble_Boss shotBubble2 = default(Bubble_Boss);
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (!fieldBubble.isLocked && fieldBubble.type != Bubble.eType.Blank && fieldBubble.type != Bubble.eType.Fulcrum && fieldBubble.type != Bubble.eType.FriendFulcrum && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fieldBubble.state == Bubble.eState.Field)
			{
				float y = fieldBubble.myTrans.localPosition.y;
				if (y > hitBubbleY - 1f && y < hitBubbleY + 1f)
				{
					breakBubbleList.Add(fieldBubble);
				}
				else if (shotBubble2.isNearBubble(fieldBubble) && (fieldBubble.type == Bubble.eType.Lightning || fieldBubble.type == Bubble.eType.Search || fieldBubble.type == Bubble.eType.Time || fieldBubble.type == Bubble.eType.Coin || fieldBubble.type == Bubble.eType.Star))
				{
					breakBubbleList.Add(fieldBubble);
				}
			}
		});
		foreach (Bubble_Boss breakBubble in breakBubbleList)
		{
			switch (breakBubble.type)
			{
			case Bubble.eType.Lightning:
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
				breakLightningCount_++;
				break;
			case Bubble.eType.Grow:
			case Bubble.eType.Skull:
			case Bubble.eType.FriendBox:
			case Bubble.eType.Box:
			case Bubble.eType.Rock:
			case Bubble.eType.Honeycomb:
			case Bubble.eType.Counter:
				if (breakBubble.isFrozen)
				{
					breakLightningCount_++;
				}
				break;
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
				UnityEngine.Object.Destroy(breakBubble.gameObject);
			}
		}
		yield return StartCoroutine(breakRoutine(shotBubble, breakLightningCount_));
		if (bSearch)
		{
			yield return StartCoroutine(searchBubbleRoutine());
		}
	}

	private bool isInScreenBubble(Bubble_Boss b)
	{
		Vector3 vector = Camera.main.WorldToScreenPoint(b.transform.position);
		vector.y += 16f;
		return vector.y < (float)Screen.height;
	}

	private IEnumerator shineRoutine(Bubble_Boss shotBubble)
	{
		bool bSearch = false;
		float shotBubbleX = shotBubble.myTrans.localPosition.x;
		breakLightningCount_ = 0;
		for (int i = 0; i < chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble chainBubble in chainBubbleDic[i])
			{
				if (chainBubble.type == Bubble.eType.ChainLock)
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
		List<Bubble_Boss> breakBubbleList = new List<Bubble_Boss>();
		Bubble_Boss shotBubble2 = default(Bubble_Boss);
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (!fieldBubble.isLocked && fieldBubble.state == Bubble.eState.Field && fieldBubble.type != Bubble.eType.Fulcrum && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fieldBubble.type != Bubble.eType.FriendFulcrum && fieldBubble.type != Bubble.eType.Blank)
			{
				float x = fieldBubble.myTrans.localPosition.x;
				float y = fieldBubble.myTrans.localPosition.y;
				if (shotBubble2.isNearBubble(fieldBubble) && y <= 0f && (fieldBubble.type == Bubble.eType.Lightning || fieldBubble.type == Bubble.eType.Search || fieldBubble.type == Bubble.eType.Time || fieldBubble.type == Bubble.eType.Coin || fieldBubble.type == Bubble.eType.Star || fieldBubble.isFrozen))
				{
					breakBubbleList.Add(fieldBubble);
				}
				if (x > shotBubbleX - 60f && x < shotBubbleX + 60f && y <= 0f)
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
		foreach (Bubble_Boss breakBubble in breakBubbleList)
		{
			if (breakBubble.state != Bubble.eState.Field)
			{
				continue;
			}
			switch (breakBubble.type)
			{
			case Bubble.eType.Lightning:
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
				breakLightningCount_++;
				break;
			case Bubble.eType.Grow:
			case Bubble.eType.Skull:
			case Bubble.eType.FriendBox:
			case Bubble.eType.Box:
			case Bubble.eType.Rock:
			case Bubble.eType.Honeycomb:
			case Bubble.eType.Counter:
				if (breakBubble.isFrozen)
				{
					breakLightningCount_++;
				}
				break;
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
				List<Bubble_Boss> frozenBubbleList = fieldBubbleList.FindAll((Bubble_Boss fieldBubble) => fieldBubble.isFrozen && breakBubble.isNearBubble(fieldBubble));
				foreach (Bubble_Boss b in frozenBubbleList)
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
				UnityEngine.Object.Destroy(breakBubble.gameObject);
			}
		}
		Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
		yield return StartCoroutine(breakRoutine(shotBubble, breakLightningCount_));
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

	private IEnumerator breakRoutine(Bubble_Boss shotBubble, int bubbleCountOffset)
	{
		int breakScore = 0;
		int dropScore2 = 0;
		int comboScore2 = 0;
		int reflectScore2 = 0;
		int rescueScore2 = 0;
		int bubbleCount2 = fieldBubbleList.Count + bubbleCountOffset;
		updateFieldBubbleList();
		if (isSpiderStage())
		{
			UpdateInSpwBubble();
		}
		yield return stagePause.sync();
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
		totalCoin += dropCount;
		breakScore *= scoreUpNum;
		dropScore2 *= scoreUpNum;
		comboScore2 *= scoreUpNum;
		reflectScore2 *= scoreUpNum;
		rescueScore2 *= scoreUpNum;
		updateTotalScoreDisp();
		yield return null;
	}

	private IEnumerator chara00BonusAnim()
	{
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
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "04");
		while (charaAnims[1].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[1] + "04"))
		{
			yield return stagePause.sync();
		}
		if (state != eState.Gameover && state != eState.Clear)
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

	public void checkSamColor(Bubble_Boss me, ref int[] checkedFlags, List<Bubble_Boss> rainbowList)
	{
		if (!me.isColorBubble())
		{
			return;
		}
		List<Bubble_Boss> list = new List<Bubble_Boss>(6);
		List<int> list2 = new List<int>(6);
		for (int i = 0; i < fieldBubbleList.Count; i++)
		{
			Bubble_Boss bubble_Boss = fieldBubbleList[i];
			if (me.GetInstanceID() != bubble_Boss.GetInstanceID() && checkedFlags[i] == 0 && bubble_Boss.state == Bubble.eState.Field && !bubble_Boss.isLocked && !bubble_Boss.isFrozen && me.isNearBubble(bubble_Boss))
			{
				list2.Add(i);
				list.Add(bubble_Boss);
			}
		}
		List<Bubble_Boss> list3 = new List<Bubble_Boss>();
		Bubble.eType eType = convertColorBubble(me.type);
		for (int j = 0; j < list.Count; j++)
		{
			Bubble.eType type = list[j].type;
			if (eType == convertColorBubble(type))
			{
				checkedFlags[list2[j]] = 1;
				list3.Add(list[j]);
				continue;
			}
			if (type == Bubble.eType.Rainbow || type == Bubble.eType.FriendRainbow)
			{
				rainbowList.Add(list[j]);
			}
			checkedFlags[list2[j]] = -1;
		}
		for (int k = 0; k < list3.Count; k++)
		{
			checkSamColor(list3[k], ref checkedFlags, rainbowList);
		}
	}

	private void checkConnect(Bubble_Boss me, List<Bubble_Boss> list, List<Bubble_Boss> checkList)
	{
		if (!list.Contains(me))
		{
			list.Add(me);
			List<Bubble_Boss> list2 = checkList.FindAll((Bubble_Boss bubble) => !list.Contains(bubble) && me.isNearBubble(bubble));
			list2.ForEach(delegate(Bubble_Boss bubble)
			{
				checkConnect(bubble, list, checkList);
			});
		}
	}

	private void checkConnectSamColor(Bubble_Boss me, List<Bubble_Boss> list)
	{
		if (!list.Contains(me))
		{
			list.Add(me);
			List<Bubble_Boss> list2 = fieldBubbleList.FindAll((Bubble_Boss bubble) => !list.Contains(bubble) && me.isNearBubble(bubble) && me.isSamColor(bubble));
			list2.ForEach(delegate(Bubble_Boss bubble)
			{
				checkConnectSamColor(bubble, list);
			});
		}
	}

	private void checkFrozen(Bubble_Boss me, List<Bubble_Boss> list)
	{
		if (!list.Contains(me))
		{
			list.Add(me);
			List<Bubble_Boss> list2 = fieldBubbleList.FindAll((Bubble_Boss bubble) => bubble.isFrozen && !list.Contains(bubble) && me.isNearBubble(bubble));
			list2.ForEach(delegate(Bubble_Boss bubble)
			{
				checkFrozen(bubble, list);
			});
		}
	}

	private IEnumerator hitSpiderweb(Bubble_Boss shotBubble)
	{
		shotBubble.isStay = true;
		shotBubble.isPowerUp = false;
		yield return stagePause.sync();
	}

	private IEnumerator hitDefault(Bubble_Boss shotBubble)
	{
		int breakNum_ = 0;
		List<Bubble_Boss> rainbowList = new List<Bubble_Boss>();
		breakLightningCount_ = 0;
		if (breakFrozen(shotBubble))
		{
			bSearch = true;
		}
		if (shotBubble.type != Bubble.eType.Shake)
		{
			int[] checkedFlags = new int[fieldBubbleList.Count];
			checkSamColor(shotBubble, ref checkedFlags, rainbowList);
			int chainCount = 0;
			int[] array = checkedFlags;
			foreach (int checkedFlag in array)
			{
				if (checkedFlag == 1)
				{
					chainCount++;
				}
			}
			if (rainbowChainCount == 0)
			{
				chainCount++;
			}
			if (chainCount < 3)
			{
				if (frozenBreakList.Count > 0)
				{
					shotBubble.startBreak();
					breakNum_ = 1;
				}
				else if (rainbowChainCount == 0)
				{
					shotBubble.isPowerUp = false;
					shotBubble.setFieldState();
					shotBubble.myTrans.parent = bubbleRoot;
					Vector3 p = shotBubble.myTrans.localPosition;
					p.z = 0f;
					shotBubble.myTrans.localPosition = p;
					fieldBubbleList.Add(shotBubble);
				}
			}
			else
			{
				shotBubble.startBreak();
				for (int l = 0; l < fieldBubbleList.Count; l++)
				{
					if (checkedFlags[l] == 1)
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
					Bubble_Boss t = rainbowList[k];
					if (t.type == Bubble.eType.FriendRainbow)
					{
						t.type = convertColorBubble(shotBubble.type) + 31;
					}
					else
					{
						t.type = convertColorBubble(shotBubble.type);
					}
					string text;
					if (t.type > Bubble.eType.Blank)
					{
						int type = (int)t.type;
						text = type.ToString("000");
					}
					else
					{
						int type2 = (int)t.type;
						text = type2.ToString("00");
					}
					t.name = text;
					t.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + t.name);
					breakLightningCount_++;
				}
			}
		}
		else
		{
			breakNum_ = 1;
		}
		frozenBreakList.Clear();
		if (rainbowChainCount == 0)
		{
			List<Bubble_Boss> nearBubbleList = new List<Bubble_Boss>(6);
			Bubble_Boss shotBubble2 = default(Bubble_Boss);
			fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
			{
				if (!fieldBubble.isLocked && fieldBubble.state == Bubble.eState.Field && shotBubble2.isNearBubble(fieldBubble))
				{
					nearBubbleList.Add(fieldBubble);
				}
			});
			foreach (Bubble_Boss nearBubble in nearBubbleList)
			{
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
			yield return StartCoroutine(SetNativeSpriteAfterBreakRoutine());
		}
		else
		{
			if (rainbowChainCount == 0)
			{
				comboCount = 0;
			}
			updateFieldBubbleList();
		}
	}

	private void star(Bubble_Boss shotBubble)
	{
		Sound.Instance.playSe(Sound.eSe.SE_332_starbubble);
		Bubble.eType shotBubbleType = shotBubble.type;
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (!fieldBubble.isLocked && !fieldBubble.isFrozen)
			{
				Bubble.eType eType = convertColorBubble(fieldBubble.type);
				if (shotBubbleType == eType)
				{
					fieldBubble.isLineFriend = false;
					fieldBubble.startBreak();
				}
			}
		});
	}

	private bool lightning(Bubble_Boss hitBubble)
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
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (!(hitBubble == fieldBubble) && !fieldBubble.isLocked && fieldBubble.state == Bubble.eState.Field && fieldBubble.type != Bubble.eType.Fulcrum && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fieldBubble.type != Bubble.eType.FriendFulcrum)
			{
				float y2 = fieldBubble.myTrans.localPosition.y;
				if (y2 > hitBubbleY - 1f && y2 < hitBubbleY + 1f)
				{
					fieldBubble.isLineFriend = false;
					if (fieldBubble.type == Bubble.eType.Skull)
					{
						fieldBubble.setBreak();
						fieldBubble.startFadeout();
					}
					else
					{
						fieldBubble.startBreak();
					}
					specialEffect(fieldBubble);
					plusMinusEffect(fieldBubble);
					if (fieldBubble.type == Bubble.eType.Search)
					{
						bSearch = true;
					}
					if (fieldBubble.type == Bubble.eType.Rainbow || fieldBubble.type == Bubble.eType.FriendRainbow)
					{
						breakLightningCount_++;
					}
				}
			}
		});
		return bSearch;
	}

	private void specialEffect(Bubble_Boss bubble)
	{
		if (bubble.state != Bubble.eState.Field)
		{
			return;
		}
		switch (bubble.type)
		{
		case Bubble.eType.Coin:
			coinBubbleCount++;
			Sound.Instance.playSe(Sound.eSe.SE_334_coinbubble);
			nextCoin = totalEventCoin;
			break;
		case Bubble.eType.Time:
			Sound.Instance.playSe(Sound.eSe.SE_333_timebubble);
			if (gameType == eGameType.Time)
			{
				startTime += TIME_ADD_VALUE;
				int num = (int)((float)stageInfo.Time - (Time.time - startTime) + 0.999999f);
				if (!((float)num > 10f))
				{
				}
			}
			playCountdownEff(true);
			break;
		}
	}

	private void plusEffect(Bubble_Boss bubble)
	{
		if (bubble.type < Bubble.eType.PlusRed || bubble.type > Bubble.eType.PlusBlack)
		{
			return;
		}
		bPlus = true;
		shotCount -= PLUS_ADD_VALUE;
		if (gameType == eGameType.ShotCount)
		{
			int num = stageInfo.Move - shotCount;
			if (num <= 5)
			{
			}
		}
	}

	private void minusEffect(Bubble_Boss bubble)
	{
		if (!isUsedItem(Constant.Item.eType.MinusGuard) && bubble.type >= Bubble.eType.MinusRed && bubble.type <= Bubble.eType.MinusBlack)
		{
			bMinus = true;
			shotCount += PLUS_ADD_VALUE;
			if (shotCount > stageInfo.Move)
			{
				shotCount = stageInfo.Move;
			}
		}
	}

	private void snakeEffect(Bubble_Boss bubble)
	{
	}

	private void plusMinusEffect(Bubble_Boss bubble)
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
		if (checkClear())
		{
			isSearching = false;
			yield break;
		}
		state = eState.Search;
		float diffTime = Time.time - startTime;
		float maxY = -2000f;
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
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
				iTween.MoveTo(scrollUi.gameObject, iTween.Hash("y", -52 * canDownLine, "easetype", iTween.EaseType.linear, "time", 0.1f * (float)canDownLine, "islocal", true));
				moveSpan = -52 * canDownLine;
				isMoving = true;
				while (scrollUi.GetComponent<iTween>() != null)
				{
					yield return stagePause.sync();
					foreach (Chackn_Boss c in chacknList)
					{
						if (c != null && c.isAnimationPlaying())
						{
							c.animationPause();
						}
					}
				}
				isMoving = false;
				foreach (Chackn_Boss c3 in chacknList)
				{
					if (c3 != null && !c3.isAnimationPlaying())
					{
						c3.transform.parent = stageUi.parent.parent;
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
					foreach (Chackn_Boss c2 in chacknList)
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
			}
			isSearching = false;
		}
		startTime = Time.time - diffTime;
		state = eState.Wait;
	}

	public float getMoveSpan()
	{
		return moveSpan;
	}

	public Transform getTransform()
	{
		return scrollUi.transform;
	}

	private void searchNextBubble()
	{
		if (ceilingBaseY > ceilingBubbleList[0].localPosition.y)
		{
			ceilingBaseY = ceilingBubbleList[0].localPosition.y;
		}
		ceilingWldBaseY = ceilingBubbleList[0].position.y;
		searchedBubbleTypeList.Clear();
		searchedBubbleList.Clear();
		bool[] array = new bool[fieldBubbleList.Count];
		if (noDropList.Count > 0)
		{
			int[] checkedFlags = new int[fieldBubbleList.Count];
			List<Bubble_Boss> list = new List<Bubble_Boss>();
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
					list.Sort(delegate(Bubble_Boss b1, Bubble_Boss b2)
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
			Bubble_Boss bubble_Boss = fieldBubbleList[m];
			if (bubble_Boss.isLocked || bubble_Boss.isNearSpw)
			{
				continue;
			}
			Vector3 localPosition = bubble_Boss.myTrans.localPosition;
			if (!(localPosition.y > ceilingBaseY + 1f))
			{
				if (localPosition.x > 504f)
				{
					dictionary.Add(bubble_Boss, localPosition.y);
				}
				if (localPosition.y > ceilingBaseY - 1f)
				{
					dictionary2.Add(bubble_Boss, 0f - localPosition.x);
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
		}
		addSerchedBubbleType();
	}

	private void addSerchedBubbleType()
	{
		float num = -363f;
		foreach (KeyValuePair<int, BubbleBase> item in searchDict)
		{
			Bubble.eType type = item.Value.type;
			if (!checkSearchExclusion(type) && !(item.Value.myTrans.localPosition.y > ceilingBaseY + 1f) && (!(item.Value is Bubble_Boss) || !((Bubble_Boss)item.Value).isFrozen))
			{
				searchedBubbleList.Add((Bubble_Boss)item.Value);
				int num2 = 1;
				if (item.Value.myTrans.localPosition.y < num)
				{
					num2 += (int)((num - item.Value.myTrans.localPosition.y) / 52f);
				}
				type = convertColorBubble(type);
				for (int i = 0; i < num2; i++)
				{
					searchedBubbleTypeList.Add(type);
				}
			}
		}
	}

	private void searchNearBubble(BubbleBase me, Bubble.eDir dir, BubbleBase startBubble)
	{
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
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
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
			if (bubbleType >= Bubble.eType.Grow && bubbleType <= Bubble.eType.Skull)
			{
				return true;
			}
			if (bubbleType >= Bubble.eType.Lightning && bubbleType <= Bubble.eType.Coin)
			{
				return true;
			}
			if (bubbleType >= Bubble.eType.Water && bubbleType <= Bubble.eType.Counter)
			{
				return true;
			}
			if (bubbleType == Bubble.eType.FriendFulcrum)
			{
				return true;
			}
			return false;
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
		if (bubbleType == Bubble.eType.FriendBox)
		{
			return Bubble.eType.Box;
		}
		return bubbleType;
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
			foreach (int useRandomColor in useRandomColorList)
			{
				searchedBubbleTypeList.Add((Bubble.eType)useRandomColor);
			}
		}
		if (searchedBubbleTypeList.Count == 0)
		{
			List<Bubble.eType> colorList = new List<Bubble.eType>();
			List<Bubble.eType> rockList = new List<Bubble.eType>();
			fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
			{
				if (!fieldBubble.isFrozen)
				{
					Bubble.eType eType2 = convertColorBubble(fieldBubble.type);
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
			if (colorList.Count == 0)
			{
				colorList = rockList;
			}
			if (colorList.Count == 0)
			{
				gameObject.name = useRandomColorList[random.Next(useRandomColorList.Count)].ToString("00");
				for (int i = 0; i < nextBubbleCount; i++)
				{
					if (nextBubbles[i] != null)
					{
						Bubble_Boss component = nextBubbles[i].GetComponent<Bubble_Boss>();
						if (component.state != Bubble.eState.Field && !isSpecialBubble((Bubble.eType)int.Parse(nextBubbles[i].name)))
						{
							nextBubbles[i].GetComponent<Bubble_Boss>().setType((Bubble.eType)useRandomColorList[random.Next(useRandomColorList.Count)]);
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
			if (bossBase_.usedColorList.Count > 0)
			{
				foreach (int usedColor in bossBase_.usedColorList)
				{
					searchedBubbleTypeList.Add((Bubble.eType)usedColor);
				}
			}
			gameObject.name = nextBubbleCorrection(searchedBubbleTypeList, littleBubble);
		}
		Bubble.eType eType = (Bubble.eType)int.Parse(gameObject.name);
		if (prevCreateBubble.ContainsKey(eType))
		{
			Dictionary<Bubble.eType, int> dictionary;
			Dictionary<Bubble.eType, int> dictionary2 = (dictionary = prevCreateBubble);
			Bubble.eType key;
			Bubble.eType key2 = (key = eType);
			int num = dictionary[key];
			dictionary2[key2] = num + 1;
		}
		else
		{
			prevCreateBubble.Clear();
			prevCreateBubble.Add(eType, 1);
		}
		gameObject.GetComponent<Bubble_Boss>().init();
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
			Bubble.eType eType2 = (Bubble.eType)useRandomColorList[random.Next(useRandomColorList.Count)];
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
					eType2 = item2.Key;
					break;
				}
			}
			int num6 = (int)eType2;
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
			nextBubbles[j] = nextBubbles[j + 1];
			if (nextBubbles[j] != null)
			{
				Bubble_Boss b = nextBubbles[j].GetComponent<Bubble_Boss>();
				if (b.state != Bubble.eState.Field && b.isPowerUp)
				{
					b.isPowerUp = false;
				}
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
			}
			StageBoostItem item = itemParent_.getItem(itemType);
			if (item.isBuy())
			{
				item.use();
				useItem(item.getItemType(), item.getNum());
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
		if (bUsingPowerUp)
		{
			setPowerupBubble(nextBubbles[0].GetComponent<Bubble_Boss>());
		}
		createNextBubble(nextBubbleCount - 1, false);
		bubbleNavi.startNavi(searchedBubbleList, nextBubbles[0].GetComponent<Bubble_Boss>().type);
	}

	private IEnumerator stepNextBubbleBobblen()
	{
		Debug.Log("stepNextBubbleBobblen");
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
		if (prevShotBubbleIndex != 0)
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
				Bubble.eType nextBubbleType = nextBubbles[1].GetComponent<Bubble_Boss>().type;
				for (int k = 0; k < fieldBubbleList.Count; k++)
				{
					if (!fieldBubbleList[k].isFrozen && (!fieldBubbleList[k].isLocked || searchedBubbleTypeList.Count <= 0) && nextBubbleType == convertColorBubble(fieldBubbleList[k].type))
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
			StageBoostItem item = itemParent_.getItem(itemType);
			if (item.isBuy())
			{
				item.use();
				useItem(item.getItemType(), item.getNum());
			}
		}
		if (bUsingPowerUp)
		{
			setPowerupBubble(nextBubbles[1].GetComponent<Bubble_Boss>());
		}
		bubbleNavi.startNavi(searchedBubbleList, nextBubbles[1].GetComponent<Bubble_Boss>().type);
	}

	private IEnumerator chara01ThrowAnim()
	{
		if (sweatEff.activeSelf)
		{
			charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "13");
			while (charaAnims[1].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[1] + "13"))
			{
				yield return stagePause.sync();
			}
			charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "12");
		}
		else
		{
			charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "08");
			while (charaAnims[1].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[1] + "08"))
			{
				yield return stagePause.sync();
			}
			charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "07");
		}
	}

	private IEnumerator chara00ThrowAnim()
	{
		if (sweatEff.activeSelf)
		{
			charaAnims[0].Play(CHARA_SPRITE_ANIMATION_HEADER[0] + "09_00_1");
			while (charaAnims[1].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[1] + "13"))
			{
				yield return stagePause.sync();
			}
			charaAnims[0].Play(waitPinchAnimName);
		}
		else
		{
			charaAnims[0].Play(CHARA_SPRITE_ANIMATION_HEADER[0] + "08_00_1");
			while (charaAnims[1].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[1] + "08"))
			{
				yield return stagePause.sync();
			}
			charaAnims[0].Play(waitAnimName);
		}
	}

	private IEnumerator chara01ThrowAnimBobblen()
	{
		if (sweatEff.activeSelf)
		{
			charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "14_00_1");
			while (charaAnims[1].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[1] + "14_00_1"))
			{
				yield return stagePause.sync();
			}
			charaAnims[1].Play(waitPinchAnimName);
		}
		else
		{
			charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "11_00_1");
			while (charaAnims[1].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[1] + "11_00_1"))
			{
				yield return stagePause.sync();
			}
			charaAnims[1].Play(waitAnimName);
		}
	}

	private void updateFieldBubbleList()
	{
		if (state == eState.Clear)
		{
			return;
		}
		fieldObjectCount = 0;
		fieldFriendCount = 0;
		fieldItemCount = 0;
		fulcrumList.Clear();
		noDropList.Clear();
		growBubbleList.Clear();
		normalBubbleCount = 0;
		lineFriendCandidateList.Clear();
		List<Bubble_Boss> tempFieldBubbleList = new List<Bubble_Boss>(fieldBubbleList);
		fieldBubbleList.Clear();
		minLine = 0;
		tempFieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (fieldBubble.state == Bubble.eState.Field)
			{
				fieldBubbleList.Add(fieldBubble);
				Bubble.eType type = fieldBubble.type;
				if ((type >= Bubble.eType.FriendRainbow && type <= Bubble.eType.RotateFulcrumL) || type == Bubble.eType.Grow || type == Bubble.eType.Skull || type == Bubble.eType.Honeycomb || type == Bubble.eType.Counter || type == Bubble.eType.BlackHole_A || type == Bubble.eType.BlackHole_B || (type >= Bubble.eType.ChameleonRed && type <= Bubble.eType.Unknown))
				{
					fieldObjectCount++;
				}
				if (type >= Bubble.eType.FriendRed && type <= Bubble.eType.FriendBox)
				{
					fieldFriendCount++;
				}
				if (type >= Bubble.eType.Lightning && type <= Bubble.eType.Coin)
				{
					fieldItemCount++;
				}
				switch (type)
				{
				case Bubble.eType.Fulcrum:
				case Bubble.eType.RotateFulcrumR:
				case Bubble.eType.RotateFulcrumL:
				case Bubble.eType.FriendFulcrum:
					fulcrumList.Add(fieldBubble);
					noDropList.Add(fieldBubble);
					break;
				}
				if (spwList != null && spwList.Count > 0)
				{
					fieldBubble.isNearSpw = isNearSpiderweb(fieldBubble);
				}
				if (fieldBubble.isNearSpw)
				{
					fieldBubble.isStay = true;
				}
				if (type <= Bubble.eType.Black && !fieldBubble.isLineFriend && !fieldBubble.isLocked && !fieldBubble.isFrozen && !fieldBubble.inCloud)
				{
					normalBubbleCount++;
					if (fieldBubble.myTrans.localPosition.y < -105f)
					{
						bool flag = true;
						foreach (Bubble_Boss item in tempFieldBubbleList)
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
				if (fieldBubble.myTrans.localPosition.y / 52f < (float)minLine)
				{
					minLine = (int)((fieldBubble.myTrans.localPosition.y - 30f) / 52f);
				}
			}
			else if (fieldBubble != null && fieldBubble.myTrans != null)
			{
				fieldBubble.myTrans.parent = fieldBubble.myTrans.parent.parent;
			}
		});
		if (minLine < -13)
		{
			bGameOver = true;
		}
		if (!requestRecreateFieldBubble && fulcrumList.Count == 0)
		{
			requestRecreateFieldBubble = true;
		}
	}

	private bool drop()
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
			List<Bubble_Boss> list = new List<Bubble_Boss>();
			for (int k = 0; k < fieldBubbleList.Count; k++)
			{
				if (checkedFlags[k] == 1)
				{
					checkedFlags[k]++;
					Bubble.eType type = fieldBubbleList[k].type;
					if (type != Bubble.eType.Fulcrum && type != Bubble.eType.RotateFulcrumL && type != Bubble.eType.RotateFulcrumR && type != Bubble.eType.FriendFulcrum && type != Bubble.eType.Rock && !fieldBubbleList[k].isNearSpw)
					{
						list.Add(fieldBubbleList[k]);
					}
				}
			}
			for (int l = 0; l < list.Count; l++)
			{
				list[l].startDrop(list.Count - l);
			}
		}
		return result;
	}

	private void checkDrop(Bubble_Boss me, ref int[] checkedFlags, ref bool isDrop, bool checkFulcrum)
	{
		Bubble.eType type = me.type;
		if (type == Bubble.eType.Blank)
		{
			isDrop = false;
		}
		if (checkFulcrum && (type == Bubble.eType.Fulcrum || type == Bubble.eType.RotateFulcrumR || type == Bubble.eType.RotateFulcrumL || type == Bubble.eType.FriendFulcrum || type == Bubble.eType.Rock))
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
		List<Bubble_Boss> list = new List<Bubble_Boss>();
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

	private bool breakFulcrum()
	{
		if (fulcrumList.Count == 0)
		{
			return false;
		}
		bool result = false;
		Dictionary<int, List<Bubble_Boss>> dictionary = new Dictionary<int, List<Bubble_Boss>>();
		int num = 0;
		foreach (Bubble_Boss fulcrum2 in fulcrumList)
		{
			bool flag = true;
			for (int i = 0; i < num; i++)
			{
				foreach (Bubble_Boss item in dictionary[i])
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
				List<Bubble_Boss> list = new List<Bubble_Boss>();
				list.Add(fulcrum2);
				dictionary.Add(num, list);
				num++;
			}
		}
		for (int j = 0; j < num; j++)
		{
			int nearBubbleCount = 0;
			foreach (Bubble_Boss fulcrum in dictionary[j])
			{
				fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
				{
					if (!(fulcrum == fieldBubble) && fieldBubble.type != Bubble.eType.Blank && fieldBubble.type != Bubble.eType.Fulcrum && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fieldBubble.type != Bubble.eType.FriendFulcrum && fieldBubble.type != Bubble.eType.Rock && fieldBubble.type != Bubble.eType.Box && fieldBubble.type != Bubble.eType.FriendBox && fieldBubble.type != Bubble.eType.Counter && fulcrum.isNearBubble(fieldBubble))
					{
						nearBubbleCount++;
					}
				});
			}
			if (nearBubbleCount != 0)
			{
				continue;
			}
			foreach (Bubble_Boss item2 in dictionary[j])
			{
				setFulcrumBrokenData(item2.createIndex, true);
				item2.startBreak();
			}
			result = true;
		}
		return result;
	}

	private void setRecreateAllFulcrumImmediate(bool isBroken)
	{
		FulcrumBrokenData[] array = fulcrumBrokenDatas;
		foreach (FulcrumBrokenData fulcrumBrokenData in array)
		{
			fulcrumBrokenData.isBroken = isBroken;
			fulcrumBrokenData.brokenTime = ((!isBroken) ? (-1f) : ALL_BREAK_RECREATE_TIME);
		}
	}

	private void setFulcrumBrokenData(int createIndex, bool isBroken)
	{
		FulcrumBrokenData[] array = fulcrumBrokenDatas;
		foreach (FulcrumBrokenData fulcrumBrokenData in array)
		{
			if (createIndex == fulcrumBrokenData.createIndex)
			{
				fulcrumBrokenData.isBroken = isBroken;
				fulcrumBrokenData.brokenTime = ((!isBroken) ? (-1f) : 0f);
				break;
			}
		}
	}

	private void updateFulcrumBrokenTime()
	{
		if (stagePause.pause)
		{
			return;
		}
		FulcrumBrokenData[] array = fulcrumBrokenDatas;
		foreach (FulcrumBrokenData fulcrumBrokenData in array)
		{
			if (fulcrumBrokenData.isBroken)
			{
				fulcrumBrokenData.brokenTime += Time.deltaTime;
			}
		}
	}

	private FulcrumBrokenData getFulcrumBrokenData(int createIndex)
	{
		FulcrumBrokenData[] array = fulcrumBrokenDatas;
		foreach (FulcrumBrokenData fulcrumBrokenData in array)
		{
			if (createIndex == fulcrumBrokenData.createIndex)
			{
				return fulcrumBrokenData;
			}
		}
		return null;
	}

	private IEnumerator timeLineDown()
	{
		while (state != eState.Wait || bossBase_.isWaitBossAction)
		{
			yield return stagePause.sync();
		}
		isWaitStageAcction = true;
		isWaitScroll = true;
		int line = -1;
		foreach (Bubble_Boss bubble2 in fieldBubbleList)
		{
			if (bubble2.isStay)
			{
				Utility.setParent(bubble2.gameObject, bossObjectRoot, false);
			}
		}
		state = eState.Scroll;
		float diffTime = Time.time - startTime;
		Vector3 offset = Vector3.up * (52 * line);
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (fieldBubble.type != Bubble.eType.Blank && !fieldBubble.isStay)
			{
				fieldBubble.myTrans.localPosition += offset;
			}
		});
		Vector3 basePos = bubbleRootPos - offset;
		bubbleRoot.localPosition = basePos;
		yield return stagePause.sync();
		float moveTime = 0.2f * (float)Mathf.Abs(line) + 0.1f;
		float moveY2 = 52 * line;
		moveY2 = ((line <= 0) ? (moveY2 - 26f) : (moveY2 + 26f));
		float moveY3 = 26f;
		if (line > 0)
		{
			moveY3 = 0f - moveY3;
		}
		bool existHitBubble = false;
		Vector3 offsetVec = new Vector3(0f, -52f, 0f);
		List<Bubble_Boss> hitNestBubbleList = new List<Bubble_Boss>();
		hitNestBubbleList.Clear();
		if (nestList != null && nestList.Count > 0)
		{
			Vector3 diff = Vector3.zero;
			float near = 5408f;
			fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
			{
				if (fieldBubble.type != Bubble.eType.Blank && !fieldBubble.isStay)
				{
					foreach (GameObject nest in nestList)
					{
						Vector3 vector = fieldBubble.myTrans.position + offsetVec * uiScale;
						diff = (vector - nest.transform.position) / uiScale;
						diff.z = 0f;
						float sqrMagnitude = diff.sqrMagnitude;
						if (!(sqrMagnitude > 2704f) && sqrMagnitude < near)
						{
							hitNestBubbleList.Add(fieldBubble);
							break;
						}
					}
				}
			});
			if (hitNestBubbleList.Count > 0)
			{
				existHitBubble = true;
			}
		}
		float elapsedTime = 0f;
		iTween.MoveTo(bubbleRoot.gameObject, iTween.Hash("y", basePos.y + moveY2, "easetype", iTween.EaseType.easeInCubic, "time", moveTime, "islocal", true));
		while (bubbleRoot.GetComponent<iTween>() != null)
		{
			if (elapsedTime > moveTime / 2f && existHitBubble)
			{
				foreach (Bubble_Boss bubble in hitNestBubbleList)
				{
					bubble.startBreak();
				}
				existHitBubble = false;
			}
			elapsedTime += Time.deltaTime;
			yield return stagePause.sync();
		}
		tayunCoroutine = StartCoroutine(tayun(basePos.y + moveY2 + moveY3, 0.8f));
		startTime = Time.time - diffTime;
		updateFieldBubbleList();
		sweatCheck();
		if (bGameOver)
		{
			gameoverType = eGameover.DeadLineOver;
			Debug.Log("lineover");
			StartCoroutine(gameoverRoutine());
			isWaitStageAcction = false;
			isWaitScroll = false;
			yield break;
		}
		if (guide.isShootButton)
		{
			guide.lineUpdate();
		}
		StartCoroutine(ReturnParentAfterScroll());
		isWaitStageAcction = false;
		isWaitScroll = false;
		lineDownCount++;
		if (lineDownCount % 2 == 0)
		{
			if (isSpiderStage())
			{
				StartCoroutine(recreateCheckSpiderStage());
			}
			else
			{
				StartCoroutine(recreateCheck());
			}
		}
		state = eState.Wait;
		yield return 0;
	}

	private IEnumerator recreateCheck()
	{
		while (bossBase_.isWaitBossAction)
		{
			yield return stagePause.sync();
		}
		isWaitStageAcction = true;
		List<int> tempFulcrumList = new List<int>();
		tempFulcrumList.Clear();
		foreach (Bubble_Boss bubble2 in fieldBubbleList)
		{
			if ((bubble2.type == Bubble.eType.Fulcrum || bubble2.type == Bubble.eType.RotateFulcrumL || bubble2.type == Bubble.eType.RotateFulcrumR || bubble2.type == Bubble.eType.FriendFulcrum) && !tempFulcrumList.Contains(bubble2.createIndex))
			{
				tempFulcrumList.Add(bubble2.createIndex);
			}
		}
		foreach (int index in fulcrumIndexList)
		{
			if (!tempFulcrumList.Contains(index))
			{
				foreach (Bubble_Boss bubble in baseBubbleList)
				{
					if (index != bubble.createIndex)
					{
						continue;
					}
					FulcrumBrokenData data = getFulcrumBrokenData(index);
					if (data.brokenTime >= ALL_BREAK_RECREATE_TIME)
					{
						if (!isDrankEff)
						{
							isDrankEff = true;
							yield return StartCoroutine(OjamaInDrankEff(drank_ojama));
							yield return StartCoroutine(recreate(bubble));
							StartCoroutine(OjamaOutDrankEff(drank_ojama));
						}
						else
						{
							yield return StartCoroutine(recreate(bubble));
						}
						setFulcrumBrokenData(index, false);
					}
				}
			}
			isDrankEff = false;
		}
		isWaitStageAcction = false;
	}

	private IEnumerator recreateCheckSpiderStage()
	{
		while (bossBase_.isWaitBossAction)
		{
			yield return stagePause.sync();
		}
		isWaitStageAcction = true;
		List<int> tempFulcrumList = new List<int>();
		tempFulcrumList.Clear();
		foreach (Bubble_Boss bubble2 in fieldBubbleList)
		{
			if ((bubble2.type == Bubble.eType.Fulcrum || bubble2.type == Bubble.eType.RotateFulcrumL || bubble2.type == Bubble.eType.RotateFulcrumR || bubble2.type == Bubble.eType.FriendFulcrum) && !tempFulcrumList.Contains(bubble2.createIndex))
			{
				tempFulcrumList.Add(bubble2.createIndex);
			}
		}
		recreateCount = 0;
		bool isTimePassed = false;
		FulcrumBrokenData[] array = fulcrumBrokenDatas;
		foreach (FulcrumBrokenData data in array)
		{
			if (data.isBroken)
			{
				recreateCount++;
			}
			if (data.brokenTime >= ALL_BREAK_RECREATE_TIME)
			{
				isTimePassed = true;
			}
		}
		Debug.Log("recreateCount = " + recreateCount);
		if (recreateCount > recreateLineCount())
		{
			recreateCount = recreateLineCount();
		}
		Debug.Log("recreateLineCount() = " + recreateLineCount());
		int compCount = 0;
		recreateLineNumber = 0;
		if (recreateCount == 0 || !isTimePassed)
		{
			isWaitStageAcction = false;
			yield break;
		}
		List<int> tempList = new List<int>();
		List<int> sortCreateIndexList = new List<int>();
		tempList.Clear();
		sortCreateIndexList.Clear();
		foreach (int index3 in fulcrumIndexList)
		{
			if (!tempList.Contains(index3))
			{
				tempList.Add(index3);
			}
		}
		int[] tempArray = new int[tempList.Count];
		int arryCount = 0;
		foreach (int index2 in tempList)
		{
			tempArray[arryCount] = index2;
			arryCount++;
		}
		for (int k = 0; k < tempArray.Length - 1; k++)
		{
			for (int i = 0; i < tempArray.Length - 1; i++)
			{
				if (tempArray[i] > tempArray[i + 1])
				{
					int temp = tempArray[i + 1];
					tempArray[i + 1] = tempArray[i];
					tempArray[i] = temp;
				}
			}
		}
		if (tempFulcrumList.Count == 1 && recreateCount == 2 && tempFulcrumList.Contains(tempArray[1]))
		{
			bUniqueCreateLine = true;
		}
		else
		{
			bUniqueCreateLine = false;
		}
		for (int j = 0; j < tempArray.Length; j++)
		{
			sortCreateIndexList.Add(tempArray[j]);
		}
		foreach (int index in sortCreateIndexList)
		{
			if (!tempFulcrumList.Contains(index))
			{
				foreach (Bubble_Boss bubble in baseBubbleList)
				{
					if (index == bubble.createIndex)
					{
						if (!isDrankEff)
						{
							isDrankEff = true;
							yield return StartCoroutine(OjamaInDrankEff(drank_ojama));
							yield return StartCoroutine(recreate(bubble));
							StartCoroutine(OjamaOutDrankEff(drank_ojama));
						}
						else
						{
							yield return StartCoroutine(recreate(bubble));
						}
						setFulcrumBrokenData(index, false);
						compCount++;
						break;
					}
				}
			}
			isDrankEff = false;
			if (compCount == recreateCount)
			{
				isWaitStageAcction = false;
				bUniqueCreateLine = false;
				yield break;
			}
		}
		isWaitStageAcction = false;
		bUniqueCreateLine = false;
	}

	private IEnumerator recreate(Bubble_Boss me)
	{
		if (state == eState.Clear)
		{
			yield break;
		}
		Sound.Instance.playSe(Sound.eSe.SE_537_bubble_flow);
		List<Bubble_Boss> createList = new List<Bubble_Boss>();
		createList.Clear();
		if (isSpiderStage())
		{
			CheckLine(me, createList, baseBubbleList);
		}
		else
		{
			checkConnect(me, createList, baseBubbleList);
		}
		List<Bubble_Boss> createdList = new List<Bubble_Boss>();
		createdList.Clear();
		foreach (Bubble_Boss bubble in createList)
		{
			GameObject obj = UnityEngine.Object.Instantiate(bubble.gameObject) as GameObject;
			obj.name = obj.name.Replace("(Clone)", string.Empty);
			obj.SetActive(true);
			Utility.setParent(obj, bubbleRoot, false);
			Bubble_Boss inst_bubble = obj.GetComponent<Bubble_Boss>();
			inst_bubble.setFieldState();
			fieldBubbleList.Add(inst_bubble);
			createdList.Add(inst_bubble);
		}
		updateFieldBubbleList();
		shuffleBubbleType(createdList);
		if (isSpiderStage())
		{
			Debug.Log("bUniqueCreateLine = " + bUniqueCreateLine);
			setline(createdList, recreateLineNumber % 3);
			changeStarBubble(createdList);
			recreateLineNumber++;
		}
		StartCoroutine(fadeInCreatedList(createdList));
	}

	private IEnumerator ReturnParentAfterScroll()
	{
		while (bubbleRoot.GetComponent<iTween>() != null)
		{
			yield return stagePause.sync();
		}
		bubbleRoot.localPosition = bubbleRootPos;
		foreach (Bubble_Boss bubble in fieldBubbleList)
		{
			if (bubble.isStay)
			{
				Utility.setParent(bubble.gameObject, bubbleRoot, false);
			}
		}
	}

	private IEnumerator StartInDrankEff(GameObject drank)
	{
		Sound.Instance.playSe(Sound.eSe.SE_546_mahoutsukai_shutsugen);
		drank.SetActive(true);
		while (drank.GetComponent<Animation>().IsPlaying("Boss_drank_start_in_anm"))
		{
			yield return stagePause.sync();
		}
	}

	private IEnumerator StartOutDrankEff(GameObject drank)
	{
		drank.GetComponent<Animation>().Play("Boss_drank_start_out_anm");
		while (drank.GetComponent<Animation>().IsPlaying("Boss_drank_start_out_anm"))
		{
			yield return stagePause.sync();
		}
		drank.SetActive(false);
	}

	private IEnumerator OjamaInDrankEff(GameObject drank)
	{
		bool spritePlay = false;
		float elapsedTime = 0f;
		drank.SetActive(true);
		Sound.Instance.playSe(Sound.eSe.SE_547_mahoutsukai_side);
		while (drank.GetComponent<Animation>().IsPlaying("Boss_drank_ojama_in_anm") || elapsedTime < 1.2f)
		{
			if (!drank.GetComponent<Animation>().IsPlaying("Boss_drank_ojama_in_anm") && !spritePlay)
			{
				drankSprite.Reset();
				drankLight.SetActive(true);
				TweenAlpha twnAlpha = drankLight.transform.Find("bg01").gameObject.GetComponent<TweenAlpha>();
				twnAlpha.enabled = true;
				twnAlpha.Reset();
				twnAlpha.Play(true);
				spritePlay = true;
			}
			elapsedTime += Time.deltaTime;
			yield return stagePause.sync();
		}
	}

	private IEnumerator OjamaOutDrankEff(GameObject drank)
	{
		float elapsedTime = 0f;
		while (elapsedTime < 0.7f)
		{
			elapsedTime += Time.deltaTime;
			yield return stagePause.sync();
		}
		Sound.Instance.playSe(Sound.eSe.SE_545_mahoutsukai_mahou);
		drank.GetComponent<Animation>().Play("Boss_drank_ojama_out_anm");
		while (drank.GetComponent<Animation>().IsPlaying("Boss_drank_ojama_out_anm"))
		{
			yield return stagePause.sync();
		}
		drank.SetActive(false);
	}

	private void shuffleBubbleType(List<Bubble_Boss> list)
	{
		List<int> list2 = new List<int>();
		list2.Clear();
		foreach (Bubble_Boss item2 in list)
		{
			if (isColorBubble((int)item2.type))
			{
				if (isFriendBubble((int)item2.type))
				{
					int item = 31 + useRandomColorList[random.Next(useRandomColorList.Count)];
					list2.Add(item);
				}
				else
				{
					list2.Add(useRandomColorList[random.Next(useRandomColorList.Count)]);
				}
			}
		}
		foreach (Bubble_Boss item3 in list)
		{
			if (isColorBubble((int)item3.type))
			{
				int num = list2[random.Next(list2.Count)];
				item3.setType((Bubble.eType)num);
				list2.Remove(num);
			}
		}
	}

	private IEnumerator fadeInCreatedList(List<Bubble_Boss> createdList)
	{
		foreach (Bubble_Boss bubble in createdList)
		{
			if (!(bubble.sprite == null))
			{
				Color tempColor2 = new Color(bubble.sprite.color.r, bubble.sprite.color.g, bubble.sprite.color.b, 0f);
				bubble.sprite.color = tempColor2;
			}
		}
		bubbleRoot.gameObject.SetActive(true);
		float alpha_time = 0f;
		while (alpha_time < RECREATE_BUBBLE_FADEIN_TIME)
		{
			foreach (Bubble_Boss bubble2 in createdList)
			{
				if (!(bubble2.sprite == null))
				{
					Color tempColor2 = new Color(bubble2.sprite.color.r, bubble2.sprite.color.g, bubble2.sprite.color.b, alpha_time / RECREATE_BUBBLE_FADEIN_TIME);
					bubble2.sprite.color = tempColor2;
				}
			}
			alpha_time += Time.deltaTime;
			yield return stagePause.sync();
		}
		foreach (Bubble_Boss bubble3 in createdList)
		{
			if (!(bubble3.sprite == null))
			{
				Color tempColor2 = new Color(bubble3.sprite.color.r, bubble3.sprite.color.g, bubble3.sprite.color.b, 1f);
				bubble3.sprite.color = tempColor2;
			}
		}
	}

	private void sweatCheck()
	{
		sweatEff.SetActive(minLine < -11);
		waitAnimChange();
	}

	private void waitAnimChange()
	{
		arrow.updateWaitAnimationImmediate();
		if (arrow.charaIndex == 0)
		{
			if (sweatEff.activeSelf)
			{
				charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "12");
			}
			else
			{
				charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "07");
			}
		}
	}

	private bool bonusOverCheck()
	{
		bool result = false;
		for (int i = 0; i < fieldBubbleList.Count; i++)
		{
			if (fieldBubbleList[i].myTrans.localPosition.y < -624f)
			{
				result = true;
				break;
			}
		}
		return result;
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
		if (bUsingPowerUp)
		{
			for (int l = 0; l < nextBubbleCount - 1; l++)
			{
				Bubble_Boss b = nextBubbles[l].GetComponent<Bubble_Boss>();
				if (b.state != Bubble.eState.Field && b.isPowerUp)
				{
					b.isPowerUp = false;
				}
			}
		}
		int count = nextBubbleCount;
		string clipName = stepNextBubbleClipName;
		if (gameType == eGameType.ShotCount && count == 3)
		{
			count = 2;
			clipName = "Next_bubble_00_anm";
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
		if (bUsingPowerUp)
		{
			setPowerupBubble(nextBubbles[arrow.charaIndex].GetComponent<Bubble_Boss>());
		}
		state = eState.Wait;
		bubbleNavi.startNavi(searchedBubbleList, nextBubbles[0].GetComponent<Bubble_Boss>().type);
	}

	private IEnumerator exchangeBobblenRoutine()
	{
		state = eState.Exchange;
		int count = nextBubbleCount;
		string clipName = "Next_bubble_02_anm";
		if (gameType != 0 || count != 3)
		{
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
			bubbleNavi.startNavi(searchedBubbleList, nextBubbles[1].GetComponent<Bubble_Boss>().type);
		}
	}

	private void updateTotalScoreDisp()
	{
		if (!stagePause.pause)
		{
			scoreUp();
		}
	}

	private void scoreUp()
	{
		if (dispCoin == nextCoin)
		{
			return;
		}
		if (nextCoin - dispCoin < 0)
		{
			tempCoin = dispCoin;
		}
		else
		{
			int num = (int)((float)(nextCoin - dispCoin) * Time.deltaTime * 2f);
			if (num < 1)
			{
				num = 1;
			}
			tempCoin += num;
			if (tempCoin > nextCoin)
			{
				tempCoin = (dispCoin = nextCoin);
			}
		}
		int length = tempCoin.ToString().Length;
		int num2 = coinRoot.childCount - 2;
		if (num2 < length)
		{
			for (int i = num2; i <= length; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(scoreList[0].gameObject) as GameObject;
				gameObject.name = (i + 1).ToString("00");
				Utility.setParent(gameObject, coinRoot, true);
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
			component.spriteName = "game_score_number_0" + totalEventCoin % (num3 * 10) / num3;
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
		return num;
	}

	private bool checkClear()
	{
		return bossBase_.isClear();
	}

	public IEnumerator clearRoutine()
	{
		state = eState.Clear;
		frontUi.Find("Top_ui/Gamestop_Button").gameObject.SetActive(false);
		yield return stagePause.sync();
		Input.enable = false;
		float x_min = -0.54f;
		float x_max = 0.54f;
		float elapsedTime = 0f;
		GameObject boss_died_eff = frontUi.Find("Boss_died_eff").gameObject;
		Vector3 objScale = boss_died_eff.transform.localScale;
		GameObject[] bound_obj = new GameObject[18];
		GameObject[] straight_obj = new GameObject[18];
		Vector3[] vec = new Vector3[18];
		Vector3[] vec_s = new Vector3[18];
		int[] maxCount = new int[18];
		int[] bCount = new int[18];
		for (int n = 0; n < 18; n++)
		{
			bound_obj[n] = UnityEngine.Object.Instantiate(boss_died_eff) as GameObject;
			bound_obj[n].transform.parent = bossBase_.transform.parent;
			bound_obj[n].transform.position = bossBase_.gameObject.transform.position;
			bound_obj[n].transform.localScale = objScale;
			bound_obj[n].SetActive(true);
			float value = n * 20;
			float SPEED = UnityEngine.Random.Range(3.5f, 5f);
			vec[n] = new Vector3(Mathf.Sin(value * ((float)Math.PI / 180f)), Mathf.Cos(value * ((float)Math.PI / 180f)), 0f) * SPEED;
			maxCount[n] = UnityEngine.Random.Range(0, 4);
			bCount[n] = 0;
		}
		for (int m = 0; m < 18; m++)
		{
			straight_obj[m] = UnityEngine.Object.Instantiate(boss_died_eff) as GameObject;
			straight_obj[m].transform.parent = bossBase_.transform.parent;
			straight_obj[m].transform.position = bossBase_.gameObject.transform.position;
			straight_obj[m].transform.localScale = objScale;
			straight_obj[m].SetActive(true);
			float value2 = (float)m * (20f * UnityEngine.Random.Range(0.6f, 1.4f));
			float SPEED2 = UnityEngine.Random.Range(2.5f, 5f);
			vec_s[m] = new Vector3(Mathf.Sin(value2 * ((float)Math.PI / 180f)), Mathf.Cos(value2 * ((float)Math.PI / 180f)), 0f) * SPEED2;
		}
		Sound.Instance.playSe(Sound.eSe.SE_230_bossbakushi);
		UnityEngine.Object.Destroy(bossBase_.gameObject);
		while (elapsedTime <= 2f)
		{
			for (int j = 0; j < 18; j++)
			{
				bound_obj[j].transform.position += vec[j] * Time.deltaTime;
				if (((bound_obj[j].transform.position.x < x_min && vec[j].x <= 0f) || (bound_obj[j].transform.position.x > x_max && vec[j].x >= 0f)) && bCount[j] < maxCount[j])
				{
					vec[j].x = 0f - vec[j].x;
					bCount[j]++;
				}
				if (guide.isHitCeiling(bound_obj[j].transform.position) && bCount[j] < maxCount[j])
				{
					vec[j].y = 0f - vec[j].y;
					bCount[j]++;
				}
				if (bound_obj[j].transform.position.y <= -0.8f && bCount[j] < maxCount[j])
				{
					vec[j].y = 0f - vec[j].y;
					bCount[j]++;
				}
			}
			for (int i = 0; i < 18; i++)
			{
				straight_obj[i].transform.position += vec_s[i] * Time.deltaTime * 0.5f;
			}
			elapsedTime += Time.deltaTime;
			yield return stagePause.sync();
		}
		for (int l = 0; l < bound_obj.Length; l++)
		{
			UnityEngine.Object.Destroy(bound_obj[l]);
		}
		for (int k = 0; k < straight_obj.Length; k++)
		{
			UnityEngine.Object.Destroy(straight_obj[k]);
		}
		GameObject[] array = nextBubbles;
		foreach (GameObject nextBubble in array)
		{
			if (nextBubble != null)
			{
				nextBubble.SetActive(false);
			}
		}
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (fieldBubble.type != Bubble.eType.Blank && fieldBubble.myTrans.localPosition.y < 105f)
			{
				fieldBubble.myTrans.parent = fieldBubble.myTrans.parent.parent;
				if (fieldBubble.isColorBubble() || fieldBubble.type == Bubble.eType.FriendBox)
				{
					fieldBubble.startBreak(false);
				}
				else
				{
					fieldBubble.startFadeout();
				}
			}
		});
		StartCoroutine(startFirework());
		yield return StartCoroutine(WaitTimeCoroutine(0.8f));
		sweatEff.SetActive(false);
		StartCoroutine(charaClearAnimation());
		clearAnim.SetActive(true);
		Sound.Instance.playBgm(Sound.eBgm.BGM_007_Clear, false);
		while (clearAnim.GetComponent<Animation>().isPlaying)
		{
			yield return stagePause.sync();
		}
		while (stageClear.GetComponent<Animation>().isPlaying)
		{
			yield return stagePause.sync();
		}
		Input.enable = true;
		yield return StartCoroutine(resultSetup(true));
	}

	private IEnumerator resultSetup(bool bClear)
	{
		Debug.Log("Constant.Boss.convBossInfoToNo( bossType, 0 ):" + Constant.Boss.convBossInfoToNo(bossType, 0));
		Input.enable = false;
		Hashtable h = Hash.BS2(p2: (bossBase_.currentHP >= 0) ? bossBase_.currentHP : 0, p3: (bossBase_.currentHP < 0) ? bossBase_.startHP : (bossBase_.startHP - bossBase_.currentHP), p0: Constant.Boss.convBossInfoToNo(bossType, 0), p1: bossLevel);
		NetworkMng.Instance.setup(h);
		yield return StartCoroutine(NetworkMng.Instance.download(API.BS2, false, true));
		while (NetworkMng.Instance.isDownloading())
		{
			yield return stagePause.sync();
		}
		WWW www = NetworkMng.Instance.getWWW();
		BossStageResult resultData_ = JsonMapper.ToObject<BossStageResult>(www.text);
		GameData gameData = GlobalData.Instance.getGameData();
		gameData.maxLevel = resultData_.maxLevel;
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
		gameData.dailyMissionSsRemaining = resultData_.dailyMissionSsRemaining;
		DailyMission.updateGetTime();
		gameData.heartRecoverySsRemaining = resultData_.heartRecoverySsRemaining;
		gameData.mailUnReadCount = resultData_.mailUnReadCount;
		gameData.userItemList = resultData_.userItemList;
		gameData.minilenCount = resultData_.minilenCount;
		gameData.minilenTotalCount = resultData_.minilenTotalCount;
		gameData.giveNiceTotalCount = resultData_.giveNiceTotalCount;
		gameData.giveNiceMonthlyCount = resultData_.giveNiceMonthlyCount;
		gameData.tookNiceTotalCount = resultData_.tookNiceTotalCount;
		gameData.isParkDailyReward = resultData_.isParkDailyReward;
		UserItemList[] userItemList = gameData.userItemList;
		foreach (UserItemList data in userItemList)
		{
			Debug.Log("itemType:" + data.itemType + "   count:" + data.count);
		}
		MainMenu mainMenu = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		mainMenu.getHeartMenu().updateRemainingTime();
		Input.enable = true;
		bResultOpen = true;
		if (bClear)
		{
			DialogResultClearBoss c_dialog = dialogManager.getDialog(DialogManager.eDialog.ResultClearBoss) as DialogResultClearBoss;
			c_dialog.bossBase_ = bossBase_;
			yield return StartCoroutine(c_dialog.showBossStage(bossLevelInfo, bossType, bossLevel));
			while (c_dialog.isOpen())
			{
				yield return null;
			}
			yield return dialogManager.StartCoroutine(ActionReward.getActionRewardInfo(ActionReward.eType.Boss));
			partManager.requestTransition(PartManager.ePart.Map, null, true);
			yield return null;
			yield break;
		}
		Sound.Instance.playSe(Sound.eSe.SE_109_OhNo);
		Sound.Instance.playBgm(Sound.eBgm.BGM_008_Over, false);
		DialogResultFailedBoss r_dialog = dialogManager.getDialog(DialogManager.eDialog.ResultFailedBoss) as DialogResultFailedBoss;
		r_dialog.bossBase_ = bossBase_;
		yield return StartCoroutine(r_dialog.showBossStage(stageInfo, bossType, bossLevel));
		while (r_dialog.isOpen())
		{
			yield return null;
		}
		partManager.requestTransition(PartManager.ePart.Map, null, true);
		yield return null;
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
		charaAnims[0].Play(CHARA_SPRITE_ANIMATION_HEADER[0] + "07");
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "09");
	}

	private IEnumerator startFirework()
	{
		UnityEngine.Object[] fireworks = new UnityEngine.Object[3]
		{
			ResourceLoader.Instance.loadGameObject("Prefabs/", "fireworks_00"),
			ResourceLoader.Instance.loadGameObject("Prefabs/", "fireworks_01"),
			ResourceLoader.Instance.loadGameObject("Prefabs/", "fireworks_02")
		};
		int fireworksIndex2 = 0;
		while (true)
		{
			StartCoroutine(fireworksRoutine(fireworks[fireworksIndex2]));
			fireworksIndex2++;
			fireworksIndex2 %= 3;
			yield return new WaitForSeconds(0.1f);
		}
	}

	private IEnumerator fireworksRoutine(UnityEngine.Object fireworks_00)
	{
		GameObject obj = UnityEngine.Object.Instantiate(bubbleObject) as GameObject;
		obj.SetActive(true);
		obj.transform.Find("AS_spr_bubble").gameObject.SetActive(false);
		Transform trans = obj.transform;
		trans.parent = nextBubbleRoot;
		trans.localScale = Vector3.one;
		trans.position = nextBubblePoses[0].position;
		trans.localPosition += Vector3.forward * 3f;
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
		if (!bResultOpen)
		{
			Sound.Instance.playSe(Sound.eSe.SE_225_fuusen);
		}
		else
		{
			Sound.Instance.playSeVolumeControl(Sound.eSe.SE_225_fuusen, false, 0.3f);
		}
		GameObject fireworks = UnityEngine.Object.Instantiate(fireworks_00) as GameObject;
		fireworks.transform.parent = frontUi;
		fireworks.transform.localScale = Vector3.one;
		fireworks.transform.position = obj.transform.position;
		fireworks.transform.localPosition += Vector3.back;
		UnityEngine.Object.Destroy(obj);
		while (fireworks.GetComponent<TweenAlpha>().enabled)
		{
			yield return null;
		}
		UnityEngine.Object.Destroy(fireworks);
	}

	private IEnumerator gameoverRoutine()
	{
		Sound.Instance.playSe(Sound.eSe.SE_236_slow);
		state = eState.Gameover;
		if (!guide.isShootButton)
		{
			guide.setActive(false);
		}
		while (stagePause.pause)
		{
			yield return 0;
		}
		sweatEff.SetActive(false);
		charaAnims[0].Play(CHARA_SPRITE_ANIMATION_HEADER[0] + "05");
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "05");
		while (bossBase_.isWaitBossAction)
		{
			yield return stagePause.sync();
		}
		int index = 0;
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (fieldBubble.type != Bubble.eType.Blank)
			{
				fieldBubble.startGameover(fieldBubbleList.Count - index, gameoverMat);
				index++;
			}
		});
		float time = 0f;
		while (time <= 1f)
		{
			time += Time.deltaTime;
			yield return stagePause.sync();
		}
		if (gameoverType == eGameover.DeadLineOver)
		{
			DialogContinueBoss dialog = dialogManager.getDialog(DialogManager.eDialog.ContinueBoss) as DialogContinueBoss;
			dialog.bossBase_ = bossBase_;
			yield return StartCoroutine(dialog.showBossStage(bossType, bossLevel, stageInfo, gameoverType, 2));
			StartCoroutine(bossBase_.GameoverWait());
			List<Bubble_Boss> tempDeleteList = new List<Bubble_Boss>();
			foreach (Bubble_Boss b2 in fieldBubbleList)
			{
				if (b2.type != Bubble.eType.Blank)
				{
					tempDeleteList.Add(b2);
				}
			}
			foreach (Bubble_Boss b in tempDeleteList)
			{
				fieldBubbleList.Remove(b);
				UnityEngine.Object.DestroyImmediate(b.gameObject);
			}
			while (dialog.isOpen())
			{
				yield return 0;
			}
			if (dialog.result == DialogContinueBoss.eResult.Continue)
			{
				yield return StartCoroutine(resurrection());
				yield break;
			}
		}
		Sound.Instance.stopBgm();
		guide.setActive(false);
		yield return StartCoroutine(resultSetup(false));
	}

	private IEnumerator resurrection()
	{
		stageEnd_ContinueCount++;
		bGameOver = false;
		setNextTap(true);
		if (bobblenShotCount_ > 0)
		{
			bobblenShotCount_ = 0;
			prevShotBubbleIndex = 1;
			updateShootCharacter(false);
			for (int i = 0; i < nextBubbles.Length; i++)
			{
				if (i != 2)
				{
					UnityEngine.Object.Destroy(nextBubbles[i]);
				}
			}
			yield return StartCoroutine(stepNextBubble());
		}
		eGameover eGameover = gameoverType;
		if (eGameover == eGameover.TimeOver)
		{
			startTime = Time.time - (float)(stageInfo.Time - stageInfo.Continue.Recovary);
			int dispCount = (int)((float)stageInfo.Time - (Time.time - startTime) + 0.999999f);
			if (!((float)dispCount > 10f))
			{
			}
		}
		waitAnimName = CHARA_SPRITE_ANIMATION_HEADER[0] + "08_02_0";
		waitPinchAnimName = CHARA_SPRITE_ANIMATION_HEADER[0] + "09_02_0";
		charaAnims[0].Play(waitAnimName);
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "07");
		scrollTimeBefor = scrollTime;
		setRecreateAllFulcrumImmediate(true);
		yield return StartCoroutine(recreateCheck());
		setRecreateAllFulcrumImmediate(false);
		if (gameoverType != eGameover.HitSkull)
		{
			state = eState.Wait;
		}
		lineDownCount = 0;
		requestRecreateFieldBubble = false;
		scrollTime = (scrollTimeBefor = Time.time);
		StartCoroutine(bossBase_.ContinueSetup());
		yield return null;
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
		yield break;
	}

	private void setReplay()
	{
		if (isInvalidReplay)
		{
			return;
		}
		replayDataList.Clear();
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			ReplayData item = new ReplayData
			{
				type = fieldBubble.type,
				pos = fieldBubble.myTrans.localPosition,
				lineFriendIndex = fieldBubble.lineFriendIndex,
				isFrozen = fieldBubble.isFrozen
			};
			replayDataList.Add(item);
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
			replayNextTypeList.Add(nextBubbles[j].GetComponent<Bubble_Boss>().type);
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
					string text = ((stageData.bubbleTypes[num3] <= 99) ? stageData.bubbleTypes[num3].ToString("00") : stageData.bubbleTypes[num3].ToString("000"));
					if (!(text == "99"))
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(bubbleObject) as GameObject;
						gameObject.SetActive(true);
						gameObject.name = text;
						Bubble_Boss component = gameObject.GetComponent<Bubble_Boss>();
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

	private void updateChainLock()
	{
		if (stageData.chainLayerNum == 0)
		{
			return;
		}
		List<int> heightList = new List<int>();
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (fieldBubble.type != Bubble.eType.Blank && !(fieldBubble.myTrans == null))
			{
				int item = Mathf.RoundToInt(fieldBubble.myTrans.localPosition.y);
				if (!heightList.Contains(item))
				{
					heightList.Add(item);
				}
			}
		});
		Dictionary<int, int> leftDic = new Dictionary<int, int>(heightList.Count);
		Dictionary<int, int> rightDic = new Dictionary<int, int>(heightList.Count);
		for (int i = 0; i < heightList.Count; i++)
		{
			leftDic.Add(heightList[i], -1000);
			rightDic.Add(heightList[i], 1000);
		}
		for (int j = 0; j < chainBubbleDic.Count; j++)
		{
			List<ChainBubble> list = chainBubbleDic[j];
			if (list.Count == 0)
			{
				continue;
			}
			Bubble.eType chainType = getChainType(list);
			switch (chainType)
			{
			case Bubble.eType.ChainHorizontal:
				list.Sort((ChainBubble b1, ChainBubble b2) => (!(b1.myTrans.localPosition.x < b2.myTrans.localPosition.x)) ? 1 : (-1));
				break;
			case Bubble.eType.ChainRightDown:
			case Bubble.eType.ChainLeftDown:
				list.Sort((ChainBubble b1, ChainBubble b2) => (!(b1.myTrans.localPosition.y > b2.myTrans.localPosition.y)) ? 1 : (-1));
				break;
			}
			list[0].attachCollider(chainType);
			int num = Mathf.RoundToInt(list[0].myTrans.localPosition.y);
			switch (chainType)
			{
			case Bubble.eType.ChainHorizontal:
			{
				for (int k = 0; k < heightList.Count; k++)
				{
					if (heightList[k] == num)
					{
						leftDic[heightList[k]] = 1000;
						rightDic[heightList[k]] = -1000;
					}
				}
				break;
			}
			case Bubble.eType.ChainRightDown:
				list.ForEach(delegate(ChainBubble chain)
				{
					Vector3 localPosition2 = chain.myTrans.localPosition;
					int key2 = Mathf.RoundToInt(localPosition2.y);
					int num3 = Mathf.RoundToInt(localPosition2.x);
					if (rightDic.ContainsKey(key2) && rightDic[key2] > num3)
					{
						rightDic[key2] = num3;
					}
				});
				break;
			case Bubble.eType.ChainLeftDown:
				list.ForEach(delegate(ChainBubble chain)
				{
					Vector3 localPosition3 = chain.myTrans.localPosition;
					int key3 = Mathf.RoundToInt(localPosition3.y);
					int num4 = Mathf.RoundToInt(localPosition3.x);
					if (leftDic.ContainsKey(key3) && leftDic[key3] < num4)
					{
						leftDic[key3] = num4;
					}
				});
				break;
			}
			for (int l = 0; l < heightList.Count; l++)
			{
				if (heightList[l] > num)
				{
					leftDic[heightList[l]] = 1000;
					rightDic[heightList[l]] = -1000;
				}
			}
		}
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (fieldBubble.type != Bubble.eType.Blank && !(fieldBubble.myTrans == null))
			{
				bool isLocked = false;
				Vector3 localPosition = fieldBubble.myTrans.localPosition;
				int key = Mathf.RoundToInt(localPosition.y);
				int num2 = Mathf.RoundToInt(localPosition.x);
				if (num2 <= leftDic[key])
				{
					isLocked = true;
				}
				if (num2 >= rightDic[key])
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

	private void setNextTap(bool flag)
	{
		if (next_tap.activeSelf != flag)
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
		return false;
	}

	private bool isFriendBubble(int t)
	{
		if (t >= 31 && t <= 38)
		{
			return true;
		}
		return false;
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
		Bubble.eType type = (Bubble.eType)TutorialManager.Instance.getHighlightBubble(stageNo - 1);
		if (type != Bubble.eType.Invalid)
		{
			type--;
			fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
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
		GimmickBase.eGimmickType[] array = new GimmickBase.eGimmickType[2]
		{
			(GimmickBase.eGimmickType)(TutorialManager.Instance.getHighlightGimmick(stageNo - 1) - 1),
			(GimmickBase.eGimmickType)(TutorialManager.Instance.getHighlightGimmick2(stageNo - 1) - 1)
		};
		for (int k = 0; k < array.Length; k++)
		{
			switch (array[k])
			{
			case GimmickBase.eGimmickType.Egg:
				if (egg_ != null)
				{
					egg_.gameObject.transform.localPosition += Vector3.back * 56f;
				}
				break;
			case GimmickBase.eGimmickType.SpikeRock:
				if (nest_ != null)
				{
					nest_.gameObject.transform.localPosition += Vector3.back * 56f;
				}
				break;
			}
		}
		bossBase_.gameObject.transform.localPosition += Vector3.back * 56f;
	}

	private void tutorialEnd()
	{
		Bubble.eType type = (Bubble.eType)TutorialManager.Instance.getHighlightBubble(stageNo - 1);
		if (type != Bubble.eType.Invalid)
		{
			type--;
			fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
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
		GimmickBase.eGimmickType[] array = new GimmickBase.eGimmickType[2]
		{
			(GimmickBase.eGimmickType)(TutorialManager.Instance.getHighlightGimmick(stageNo - 1) - 1),
			(GimmickBase.eGimmickType)(TutorialManager.Instance.getHighlightGimmick2(stageNo - 1) - 1)
		};
		for (int k = 0; k < array.Length; k++)
		{
			switch (array[k])
			{
			case GimmickBase.eGimmickType.Egg:
				if (egg_ != null)
				{
					egg_.gameObject.transform.localPosition += Vector3.forward * 56f;
				}
				break;
			case GimmickBase.eGimmickType.SpikeRock:
				if (nest_ != null)
				{
					nest_.gameObject.transform.localPosition += Vector3.forward * 56f;
				}
				break;
			}
		}
		bossBase_.gameObject.transform.localPosition += Vector3.forward * 56f;
	}

	private IEnumerator bubblePlusEffect()
	{
		GameObject obj = UnityEngine.Object.Instantiate(bubbleObject) as GameObject;
		obj.SetActive(true);
		if (gameType == eGameType.ShotCount)
		{
			obj.GetComponent<Bubble_Boss>().setType(Bubble.eType.PlusRed);
			yield return StartCoroutine(preCountUpEffect(obj));
			countUpEffect(bubblePlusNum);
		}
		else
		{
			obj.GetComponent<Bubble_Boss>().setType(Bubble.eType.Time);
			yield return StartCoroutine(preCountUpEffect(obj));
			countUpEffect(timePlusNum);
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
				countUpEffect(bubblePlusNum);
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
		Sound.Instance.playSe(Sound.eSe.SE_335_plusbubble);
		playCountdownEff(true);
		if (state < eState.Wait)
		{
			if (gameType == eGameType.ShotCount)
			{
				shotCount = -value;
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
	}

	private IEnumerator lastPoint()
	{
		Debug.Log("lastPoint");
		if (!guide.isShootButton)
		{
			guide.setActive(false);
		}
		stagePause.pause = true;
		lastPoint_chackn = true;
		Transform last_point = frontUi.Find("last_point");
		last_point.gameObject.SetActive(true);
		Transform labelTrans = last_point.Find("count_label");
		UILabel label = labelTrans.GetComponent<UILabel>();
		if (!bLastPoint && !ResourceLoader.Instance.isJapanResource())
		{
			labelTrans.localPosition += Vector3.left * 50f;
		}
		int dispCount = (int)((float)stageInfo.Time - (Time.time - startTime) + 0.9999f);
		last_point.Find("time").gameObject.SetActive(true);
		last_point.Find("number").gameObject.SetActive(false);
		label.text = dispCount.ToString();
		TweenPosition last_pointAnim = last_point.GetComponent<TweenPosition>();
		last_pointAnim.Reset();
		last_pointAnim.Play(true);
		while (last_pointAnim.enabled)
		{
			if (stagePause.pause)
			{
				foreach (Chackn_Boss c in chacknList)
				{
					if (c != null)
					{
						c.animationPause();
					}
				}
			}
			yield return null;
		}
		last_point.gameObject.SetActive(false);
		bLastPoint = true;
		DialogBase dialogQuit = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
		while (dialogQuit.isOpen())
		{
			yield return null;
		}
		stagePause.pause = false;
		lastPoint_chackn = false;
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
		default:
			return false;
		}
	}

	private bool breakFrozen(Bubble_Boss shotBubble)
	{
		bool result = false;
		foreach (Bubble_Boss frozenBreak in frozenBreakList)
		{
			switch (frozenBreak.type)
			{
			case Bubble.eType.Star:
				star(shotBubble);
				break;
			case Bubble.eType.Lightning:
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
				breakLightningCount_++;
				break;
			}
			frozenBreak.startBreak();
			plusEffect(frozenBreak);
		}
		if (frozenBreakList.Count > 0)
		{
			Sound.Instance.playSe(Sound.eSe.SE_505_frozen_bubble_broken);
		}
		return result;
	}

	private void updateChangeBubbleBobblen()
	{
		int num = stageInfo.Move - shotCount;
		Bubble_Boss component = nextBubbles[1].GetComponent<Bubble_Boss>();
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

	private IEnumerator SetNativeSpriteRoutine(Bubble_Boss shotBubble)
	{
		Bubble_Boss shotBubble2 = default(Bubble_Boss);
		List<Bubble_Boss> nearSecretBubble = fieldBubbleList.FindAll((Bubble_Boss fieldBubble) => shotBubble2.isNearBubble(fieldBubble) && fieldBubble.isSecret);
		changeNativeSpriteAfterBreakList.Clear();
		List<Bubble_Boss> tempSamList = new List<Bubble_Boss>();
		tempSamList.Clear();
		checkConnectSamColor(shotBubble, tempSamList);
		if (tempSamList.Count >= 3)
		{
			foreach (Bubble_Boss b in tempSamList)
			{
				if (b.isSecret)
				{
					nearSecretBubble.Add(b);
				}
				foreach (Bubble_Boss bubble4 in fieldBubbleList)
				{
					if (!tempSamList.Contains(bubble4) && !nearSecretBubble.Contains(bubble4) && isColorBubble((int)bubble4.type) && b.isNearBubble(bubble4) && bubble4.isSecret)
					{
						changeNativeSpriteAfterBreakList.Add(bubble4);
					}
				}
			}
		}
		if (nearSecretBubble.Count > 0)
		{
			foreach (Bubble_Boss bubble3 in nearSecretBubble)
			{
				bubble3.sprite.Play("burst_fether_00");
			}
			foreach (Bubble_Boss bubble2 in nearSecretBubble)
			{
				while (bubble2.sprite.IsPlaying("burst_fether_00"))
				{
					yield return stagePause.sync();
				}
			}
			foreach (Bubble_Boss bubble in nearSecretBubble)
			{
				bubble.setType(bubble.type);
				bubble.isSecret = false;
			}
			float waitTime = 0f;
			while (waitTime < 0.2f)
			{
				waitTime += Time.deltaTime;
				yield return stagePause.sync();
			}
		}
		yield return null;
	}

	private IEnumerator SetNativeSpriteAfterBreakRoutine()
	{
		if (changeNativeSpriteAfterBreakList.Count > 0)
		{
			foreach (Bubble_Boss bubble3 in changeNativeSpriteAfterBreakList)
			{
				if (!(bubble3 == null) && bubble3.state == Bubble.eState.Field)
				{
					bubble3.sprite.Play("burst_fether_00");
				}
			}
			foreach (Bubble_Boss bubble2 in changeNativeSpriteAfterBreakList)
			{
				if (!(bubble2 == null) && bubble2.state == Bubble.eState.Field)
				{
					while (bubble2.sprite.IsPlaying("burst_fether_00"))
					{
						yield return stagePause.sync();
					}
				}
			}
			foreach (Bubble_Boss bubble in changeNativeSpriteAfterBreakList)
			{
				if (!(bubble == null) && bubble.state == Bubble.eState.Field)
				{
					bubble.setType(bubble.type);
					bubble.isSecret = false;
				}
			}
			float waitTime = 0f;
			while (waitTime < 0.2f)
			{
				waitTime += Time.deltaTime;
				yield return stagePause.sync();
			}
		}
		changeNativeSpriteAfterBreakList.Clear();
	}

	private void checkNextBubbleExistant(int index)
	{
		if (nextBubbles[index] == null)
		{
			return;
		}
		bool flag = false;
		Bubble.eType type = nextBubbles[index].GetComponent<Bubble_Boss>().type;
		if (isSpecialBubble(type))
		{
			return;
		}
		for (int i = 0; i < fieldBubbleList.Count; i++)
		{
			if (!fieldBubbleList[i].isFrozen && (!fieldBubbleList[i].isLocked || searchedBubbleTypeList.Count <= 0) && type == convertColorBubble(fieldBubbleList[i].type))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			UnityEngine.Object.DestroyImmediate(nextBubbles[index]);
			createNextBubble(index, false);
		}
	}

	public IEnumerator freeDropBubbleRoutine(int dropnum)
	{
		GameObject[] bubbleObjs = new GameObject[dropnum];
		Bubble_Boss[] dropBubbles = new Bubble_Boss[dropnum];
		Bubble_Boss[] hitBubbles = new Bubble_Boss[dropnum];
		Vector3[] hitDiffs = new Vector3[dropnum];
		Vector3[] hitPoses = new Vector3[dropnum];
		bool[] isEndDrop = new bool[dropnum];
		for (int l = 0; l < isEndDrop.Length; l++)
		{
			isEndDrop[l] = false;
		}
		for (int k = 0; k < dropnum; k++)
		{
			if (bubbleObjs[k] == null)
			{
				Bubble.eType rand_type = (Bubble.eType)useRandomColorList[random.Next(useRandomColorList.Count)];
				bubbleObjs[k] = UnityEngine.Object.Instantiate(bubbleObject) as GameObject;
				GameObject obj = bubbleObjs[k];
				int num = (int)rand_type;
				obj.name = num.ToString("00");
				dropBubbles[k] = bubbleObjs[k].GetComponent<Bubble_Boss>();
				dropBubbles[k].init();
				dropBubbles[k].createIndex = 999;
				dropBubbles[k].bossBase = bossBase_;
				float random_x = 0f;
				Transform temp_tran = bubbleObjs[k].transform;
				if (k != 0)
				{
					bool bContinue = true;
					while (bContinue)
					{
						random_x = random.Next(540);
						bContinue = false;
						for (int m = 0; m < k; m++)
						{
							if ((int)Mathf.Abs(random_x - bubbleObjs[m].transform.localPosition.x) < 120)
							{
								bContinue = true;
								break;
							}
						}
					}
				}
				else
				{
					random_x = random.Next(540);
				}
				temp_tran.parent = bubbleRoot;
				temp_tran.localScale = Vector3.one;
				temp_tran.localPosition = new Vector3(random_x, ceilingBaseY + 52f, 0f);
				bubbleObjs[k].SetActive(true);
			}
			setDrop(bubbleObjs[k], ref hitBubbles[k], ref hitPoses[k], ref hitDiffs[k]);
			Sound.Instance.playSe(Sound.eSe.SE_535_bubblefall);
			iTween.MoveTo(bubbleObjs[k], iTween.Hash("position", hitPoses[k], "easetype", iTween.EaseType.easeInQuad, "time", (bubbleObjs[k].transform.position.y - hitPoses[k].y) * 0.5f, "islocal", false));
		}
		bool bDropping2 = true;
		while (bDropping2)
		{
			bDropping2 = false;
			for (int i = 0; i < bubbleObjs.Length; i++)
			{
				if (bubbleObjs[i].GetComponent<iTween>() != null)
				{
					bDropping2 = true;
				}
				else
				{
					if (isEndDrop[i])
					{
						continue;
					}
					isEndDrop[i] = true;
					if (hitBubbles[i] == null)
					{
						dropBubbles[i].startBreak(true);
						continue;
					}
					Vector3 correctpos = hitBubbles[i].myTrans.localPosition + dropBubbles[i].getCorrectOffset(hitDiffs[i]);
					Vector3 diff2 = dropBubbles[i].myTrans.localPosition - correctpos;
					iTween.MoveTo(bubbleObjs[i], iTween.Hash("position", correctpos, "time", 0.05f, "islocal", true, "easetype", iTween.EaseType.linear));
					dropBubbles[i].shock(diff2);
					foreach (Bubble_Boss fieldBubble in fieldBubbleList)
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
					dropBubbles[i].setFieldState();
					fieldBubbleList.Add(dropBubbles[i]);
				}
			}
			yield return stagePause.sync();
		}
		updateFieldBubbleList();
		bDropping2 = true;
		while (bDropping2)
		{
			bDropping2 = false;
			for (int j = 0; j < bubbleObjs.Length; j++)
			{
				if (bubbleObjs[j] != null && bubbleObjs[j].GetComponent<iTween>() != null)
				{
					bDropping2 = true;
					break;
				}
			}
			yield return stagePause.sync();
		}
	}

	public void setDrop(GameObject baseObj, ref Bubble_Boss hitBubble, ref Vector3 hitPos, ref Vector3 hitDiff)
	{
		GameObject gameObject = new GameObject("temp_obj");
		Utility.setParent(gameObject, baseObj.transform.parent, false);
		gameObject.transform.localScale = baseObj.transform.localScale;
		gameObject.transform.position = baseObj.transform.position;
		hitBubble = null;
		Vector3 vector = -base.transform.up;
		Vector3 nearDiff = Vector3.zero;
		int num = 0;
		bool flag = false;
		while (hitBubble == null)
		{
			for (int i = 1; i <= 10; i++)
			{
				Vector3 localPosition = gameObject.transform.localPosition + vector;
				if (localPosition.x < 0f || localPosition.x > 540f)
				{
					float num2 = ((!(vector.x < 0f)) ? ((localPosition.x - 540f) / vector.x) : (localPosition.x / vector.x));
					localPosition = gameObject.transform.localPosition + vector * (1f - num2);
					vector.x = 0f - vector.x;
					localPosition += vector * num2;
				}
				gameObject.transform.localPosition = localPosition;
				checkFieldBubble(gameObject.transform, ref nearDiff, ref hitBubble);
				if (hitBubble != null)
				{
					hitPos = gameObject.transform.position;
					break;
				}
				if (arrow.transform.position.y > gameObject.transform.position.y)
				{
					flag = true;
					hitPos = gameObject.transform.position;
					break;
				}
				num++;
			}
			if (flag)
			{
				break;
			}
		}
		hitDiff = nearDiff;
		UnityEngine.Object.DestroyImmediate(gameObject);
	}

	private void checkFieldBubble(Transform baseTrans, ref Vector3 nearDiff, ref Bubble_Boss hitBubble)
	{
		Vector3 zero = Vector3.zero;
		float num = Bubble.SQR_SIZE * 2f;
		foreach (Bubble_Boss fieldBubble in fieldBubbleList)
		{
			if (fieldBubble.type != Bubble.eType.Blank)
			{
				zero = (baseTrans.position - fieldBubble.myTrans.position) / uiScale;
				zero.z = 0f;
				float sqrMagnitude = zero.sqrMagnitude;
				if (!(sqrMagnitude > Bubble.SQR_SIZE) && sqrMagnitude < num)
				{
					num = sqrMagnitude;
					nearDiff = zero;
					hitBubble = fieldBubble;
				}
			}
		}
	}

	private void updateShootCharacter(bool force)
	{
		int num = ((bobblenShotCount_ > 0) ? 1 : 0);
		if (!force && arrow.charaIndex == num)
		{
			return;
		}
		arrow.charaIndex = num;
		arrow.bubblen = charaAnims[num];
		Vector3 localPosition = arrow.transform.localPosition;
		localPosition.x = charaObjs[num].transform.localPosition.x;
		arrow.transform.localPosition = localPosition;
		if (num == 0)
		{
			nextBubblePoses[1].localPosition = new Vector3(nextBubblePoses[1].localPosition.x, nextBubbleBobllenBefor_Y, nextBubblePoses[1].localPosition.z);
			localPosition = charaObjs[1].transform.localPosition;
			localPosition.x = -178f;
			localPosition.z = -5f;
			charaObjs[1].transform.localPosition = localPosition;
			guide.shootBasePos = Vector3.zero;
			waitAnimName = CHARA_SPRITE_ANIMATION_HEADER[0] + "08_02_0";
			waitPinchAnimName = CHARA_SPRITE_ANIMATION_HEADER[0] + "09_02_0";
			charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "07");
			arrow.updateWaitAnimationImmediate();
		}
		else
		{
			localPosition = nextBubblePoses[1].position;
			localPosition.y = nextBubblePoses[0].position.y;
			nextBubblePoses[1].position = localPosition;
			if (nextBubbles[1] != null)
			{
				nextBubbles[1].transform.position = nextBubblePoses[1].position;
			}
			localPosition = charaObjs[1].transform.localPosition;
			localPosition.x = -158f;
			localPosition.z = -4f;
			charaObjs[1].transform.localPosition = localPosition;
			localPosition = guide.shootBasePos;
			localPosition.x = charaObjs[1].transform.localPosition.x;
			guide.shootBasePos = localPosition;
			waitAnimName = CHARA_SPRITE_ANIMATION_HEADER[1] + "11_00_0";
			waitPinchAnimName = CHARA_SPRITE_ANIMATION_HEADER[1] + "14_00_0";
			charaAnims[0].Play(CHARA_SPRITE_ANIMATION_HEADER[0] + "09");
			arrow.updateWaitAnimationImmediate();
		}
		if (guide.isShootButton)
		{
			guide.lineUpdate();
		}
	}

	public IEnumerator BubblenKnockout()
	{
		bobblenShotCount_ += 3;
		if (bobblenShotCount_ > 99)
		{
			bobblenShotCount_ = 99;
		}
		if (arrow.charaIndex == 0)
		{
			Bubble_Boss b = nextBubbles[0].GetComponent<Bubble_Boss>();
			if (b.isPowerUp)
			{
				StageBoostItem item = itemParent_.getItem(Constant.Item.eType.PowerUp);
				if (isUsedItem(Constant.Item.eType.PowerUp))
				{
					if (item.getItemType() == Constant.Item.eType.PowerUp)
					{
						nextBubbles[0].GetComponent<Bubble_Boss>().isPowerUp = false;
					}
					setPowerupBubble(nextBubbles[1].GetComponent<Bubble_Boss>());
				}
			}
		}
		setNextTap(false);
		setNextTapBobblen(false);
		nextBubbles[0].SetActive(false);
		arrow.guide.setActive(false);
		arrow.isButtonTouch = true;
		arrow.isValidTouch = true;
		updateShootCharacter(true);
		yield return stagePause.sync();
	}

	public IEnumerator updateSpiderweb()
	{
		Vector3 diff_2 = Vector3.zero;
		float uiScale_ = uiScale;
		foreach (Spiderweb spw in spwList)
		{
			bool isnear = false;
			if (!spw.gameObject.activeSelf)
			{
				continue;
			}
			foreach (Bubble_Boss bubble in inSpwBubbles)
			{
				diff_2 = (spw.transform.position - bubble.transform.position) / uiScale_;
				diff_2.z = 0f;
				float sqrMagnitude = diff_2.sqrMagnitude;
				if (sqrMagnitude < 3610f)
				{
					isnear = true;
					break;
				}
			}
			if (!isnear)
			{
				StartCoroutine(spw.PlayBurst());
			}
		}
		if (activeSpiderwebCount() == 0)
		{
			bossBase_.BreakedSpiderweb();
		}
		yield break;
	}

	public IEnumerator turnupSpiderweb()
	{
		foreach (Spiderweb spw in spwList)
		{
			spw.PlayBubble();
		}
		yield return StartCoroutine(RecreateSpwBubble());
		UpdateInSpwBubble();
		yield return stagePause.sync();
	}

	public int activeSpiderwebCount()
	{
		if (spwList.Count == 0)
		{
			return 0;
		}
		int num = 0;
		foreach (Spiderweb spw in spwList)
		{
			if (spw.isActive)
			{
				num++;
			}
		}
		return num;
	}

	public bool isNearSpiderweb(BubbleBase bubble)
	{
		float num = Bubble.SQR_SIZE * 2f;
		foreach (Spiderweb spw in spwList)
		{
			if (spw.gameObject.activeSelf)
			{
				Vector3 vector = (spw.transform.position - bubble.myTrans.position) / uiScale;
				vector.z = 0f;
				float sqrMagnitude = vector.sqrMagnitude;
				if (!(sqrMagnitude > 8100f) && sqrMagnitude < num)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void InitInSpwBubbles()
	{
		inSpwBubbles.Clear();
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (fieldBubble.isNearSpw)
			{
				fieldBubble.isStay = true;
				inSpwBubbles.Add(fieldBubble);
			}
		});
		foreach (Bubble_Boss inSpwBubble in inSpwBubbles)
		{
			foreach (Bubble_Boss baseBubble in baseBubbleList)
			{
				if (inSpwBubble.myTrans.localPosition.x == baseBubble.myTrans.localPosition.x && inSpwBubble.myTrans.localPosition.y == baseBubble.myTrans.localPosition.y)
				{
					baseBubble.isStay = true;
					baseBubble.isNearSpw = true;
				}
			}
		}
	}

	public void UpdateInSpwBubble()
	{
		inSpwBubbles.Clear();
		fieldBubbleList.ForEach(delegate(Bubble_Boss fieldBubble)
		{
			if (fieldBubble.isNearSpw)
			{
				fieldBubble.isStay = true;
				inSpwBubbles.Add(fieldBubble);
			}
		});
	}

	public void CheckLine(Bubble_Boss bubble, List<Bubble_Boss> list, List<Bubble_Boss> checkList)
	{
		foreach (Bubble_Boss check in checkList)
		{
			if (check.transform.localPosition.y == bubble.transform.localPosition.y)
			{
				list.Add(check);
			}
		}
	}

	public List<List<Bubble_Boss>> sortLine(List<Bubble_Boss> selectList)
	{
		List<List<Bubble_Boss>> list = new List<List<Bubble_Boss>>();
		list.Clear();
		List<Bubble_Boss> list2 = new List<Bubble_Boss>();
		list2.Clear();
		list2 = selectList;
		List<Bubble_Boss> list3 = new List<Bubble_Boss>();
		while (list2.Count > 0)
		{
			Debug.Log("temp.Count = " + list2.Count);
			float y = list2[0].myTrans.localPosition.y;
			List<Bubble_Boss> list4 = new List<Bubble_Boss>();
			list4.Clear();
			foreach (Bubble_Boss item in list2)
			{
				if (y == item.myTrans.localPosition.y)
				{
					list4.Add(item);
				}
				else
				{
					list3.Add(item);
				}
			}
			list.Add(list4);
			list2.Clear();
			for (int i = 0; i < list3.Count; i++)
			{
				list2.Add(list3[i]);
			}
			list3.Clear();
		}
		return list;
	}

	private List<Bubble_Boss> selectForBossSpider()
	{
		List<Bubble_Boss> list = new List<Bubble_Boss>();
		list.Clear();
		foreach (Bubble_Boss fieldBubble in fieldBubbleList)
		{
			if (!fieldBubble.isNearSpw && fieldBubble.type != Bubble.eType.Fulcrum && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && !(fieldBubble.sprite == null))
			{
				list.Add(fieldBubble);
			}
		}
		return list;
	}

	public IEnumerator BreakStarBubble(int breakCount)
	{
		List<Bubble_Boss> starList = new List<Bubble_Boss>();
		starList.Clear();
		fieldBubbleList.ForEach(delegate(Bubble_Boss bubble)
		{
			if (bubble.type == Bubble.eType.Star)
			{
				starList.Add(bubble);
			}
		});
		for (int i = 0; i < breakCount; i++)
		{
			if (starList.Count > 0)
			{
				Bubble_Boss bubble2 = starList[random.Next(starList.Count)];
				starList.Remove(bubble2);
				fieldBubbleList.Remove(bubble2);
				yield return StartCoroutine(bossBase_.tailAction(bubble2));
				Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
				bubble2.startBreak();
			}
		}
		fulcrumList.ForEach(delegate(Bubble_Boss fulcrum)
		{
			bool bNear = false;
			selectForBossSpider().ForEach(delegate(Bubble_Boss bubble)
			{
				if (fulcrum.isNearBubble(bubble))
				{
					bNear = true;
				}
			});
			if (!bNear)
			{
				fulcrum.startBreak();
			}
		});
		updateFieldBubbleList();
		yield return stagePause.sync();
	}

	public IEnumerator BreakPickingTarget()
	{
		if (!(pickingTargetBubble == null))
		{
			yield return StartCoroutine(bossBase_.tailAction(pickingTargetBubble));
			Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
			pickingTargetBubble.startBreak();
			updateFieldBubbleList();
		}
	}

	public void changeStarBubble(List<Bubble_Boss> list)
	{
		Bubble_Boss bubble_Boss = list[random.Next(list.Count)];
		while (bubble_Boss.isNearSpw || bubble_Boss.type == Bubble.eType.Fulcrum || bubble_Boss.type == Bubble.eType.RotateFulcrumR || bubble_Boss.type == Bubble.eType.RotateFulcrumL || bubble_Boss.sprite == null)
		{
			bubble_Boss = list[random.Next(list.Count)];
		}
		bubble_Boss.setType(Bubble.eType.Star);
	}

	private IEnumerator SetSpiderStage()
	{
		Vector3 offsetVec = new Vector3(0f, -104f, 0f);
		foreach (Bubble_Boss bubble in fieldBubbleList)
		{
			if (bubble.sprite != null)
			{
				bubble.transform.localPosition += offsetVec;
			}
		}
		foreach (Bubble_Boss bubble2 in baseBubbleList)
		{
			if (bubble2.sprite != null)
			{
				bubble2.myTrans.localPosition += offsetVec;
			}
		}
		foreach (Spiderweb spw in spwList)
		{
			spw.gameObject.transform.localPosition += offsetVec;
		}
		yield return null;
	}

	private int recreateLineCount()
	{
		List<Bubble_Boss> list = new List<Bubble_Boss>();
		list.Clear();
		foreach (Bubble_Boss fieldBubble in fieldBubbleList)
		{
			if (!fieldBubble.isNearSpw && fieldBubble.sprite != null)
			{
				list.Add(fieldBubble);
			}
		}
		int num = 3;
		foreach (Bubble_Boss item in list)
		{
			if (item.myTrans.localPosition.y >= startline_y[0])
			{
				num = 0;
				break;
			}
			if (num >= 2 && item.myTrans.localPosition.y >= startline_y[1])
			{
				num = 1;
			}
			else if (num >= 3 && item.myTrans.localPosition.y >= startline_y[2])
			{
				num = 2;
			}
		}
		return num;
	}

	private IEnumerator RecreateSpwBubble()
	{
		Sound.Instance.playSe(Sound.eSe.SE_537_bubble_flow);
		List<Bubble_Boss> createList = new List<Bubble_Boss>();
		createList.Clear();
		foreach (Bubble_Boss b in baseBubbleList)
		{
			if (b.isNearSpw)
			{
				createList.Add(b);
			}
		}
		List<Bubble_Boss> createdList = new List<Bubble_Boss>();
		createdList.Clear();
		foreach (Bubble_Boss bubble in createList)
		{
			GameObject obj = UnityEngine.Object.Instantiate(bubble.gameObject) as GameObject;
			obj.name = obj.name.Replace("(Clone)", string.Empty);
			obj.SetActive(true);
			Utility.setParent(obj, bubbleRoot, false);
			Bubble_Boss inst_bubble = obj.GetComponent<Bubble_Boss>();
			inst_bubble.setFieldState();
			fieldBubbleList.Add(inst_bubble);
			createdList.Add(inst_bubble);
		}
		updateFieldBubbleList();
		shuffleBubbleType(createdList);
		StartCoroutine(fadeInCreatedList(createdList));
		yield break;
	}

	private void setline(List<Bubble_Boss> list, int number)
	{
		foreach (Bubble_Boss item in list)
		{
			item.myTrans.localPosition = new Vector3(item.myTrans.localPosition.x, startline_y[number], item.myTrans.localPosition.z);
		}
	}

	private bool isSpiderStage()
	{
		return bossStageInfo.BossInfo.BossType == 3;
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
		Debug.Log("waitAnimName = " + waitAnimName);
		arrow.charaAnimNames = new string[2]
		{
			CHARA_SPRITE_ANIMATION_HEADER[0] + "08_",
			CHARA_SPRITE_ANIMATION_HEADER[1] + "11_"
		};
		arrow.pinchCharaAnimNames = new string[2]
		{
			CHARA_SPRITE_ANIMATION_HEADER[0] + "09_",
			CHARA_SPRITE_ANIMATION_HEADER[1] + "14_"
		};
	}

	private IEnumerator characterInstantiate()
	{
		GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		StageDataTable stageTbl = dataTable.GetComponent<StageDataTable>();
		Network.Avatar avatar = GlobalData.Instance.currentAvatar;
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

	private IEnumerator WaitTimeCoroutine(float time)
	{
		float elapsedTime = 0f;
		while (elapsedTime < time)
		{
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}

	private void SnakeSet()
	{
		snakeCounter = frontUi.Find("snake_counter").gameObject;
		if (ResourceLoader.Instance.isUseLowResource())
		{
			snakeCounter.AddComponent<UIPanel>();
		}
		snakeCounterLabel = snakeCounter.transform.Find("snake_count").GetComponent<UILabel>();
		snakeCounterAnm = snakeCounter.GetComponent<Animation>();
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

	public IEnumerator HitEffPlay()
	{
		hitEff.SetActive(true);
		hitEff.transform.position = shotBubblePos;
		while (hitEff.GetComponent<Animation>().isPlaying)
		{
			yield return stagePause.sync();
		}
		hitEff.SetActive(false);
	}

	public IEnumerator GuardEffPlay()
	{
		guardEff.SetActive(true);
		guardEff.transform.position = shotBubblePos;
		while (guardEff.GetComponent<Animation>().isPlaying)
		{
			yield return stagePause.sync();
		}
		guardEff.SetActive(false);
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
			gameoverType = eGameover.TimeOver;
			StartCoroutine(gameoverRoutine());
		}
	}
}
