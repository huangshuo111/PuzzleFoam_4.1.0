using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Network;
using UnityEngine;

public class Part_BonusStage : PartBase
{
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

	public enum eGameover
	{
		ShotCountOver = 0,
		TimeOver = 1,
		HitSkull = 2,
		FriendToGrow = 3,
		ScoreNotEnough = 4,
		CounterOver = 5
	}

	public enum eClear
	{
		Score = 1,
		AllDel = 2,
		Friend = 4,
		Fulcrum = 8
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

	private const float END_WAIT_TIME = 1f;

	private const float CHANGE_COLOR_TIME = 10f;

	private const int CHANGE_COLOR_SHOT_COUNT = 5;

	private const string CEILING_BUBBLE_NAME = "99";

	private const int GLOSS_INTERVAL = 30;

	private const float CEILING_OFFSET_LOCALPOS = 20f;

	private const int RANDOM_RESEARCH_LIMIT = 10;

	public float uiScale;

	public StagePause stagePause;

	private int COIN_ADD_VALUE = 50;

	public int mTotalCoin;

	public int totalEventCoin;

	public int coinBubbleCount;

	public int nextCorrection = 5;

	public int nextLimitCorrection = 50;

	private GameObject bubbleObject;

	private GameObject chainBubbleObject;

	private Transform bubbleRoot;

	private Transform nextBubbleRoot;

	private Vector3 bubbleRootPos = new Vector3(-270f, 455f, 0f);

	private List<Transform> ceilingBubbleList = new List<Transform>(10);

	private Transform launchpad;

	private List<Transform> scoreList = new List<Transform>(8);

	private Transform scoreRoot;

	private Transform coinRoot;

	private GameObject[] nextBubbles = new GameObject[3];

	private Transform[] nextBubblePoses = new Transform[3];

	private Animation stepNextBubbleAnim;

	private string stepNextBubbleClipName = "Next_bubble_00_anm";

	private int nextBubbleCount = 2;

	public List<Bubble> fieldBubbleList = new List<Bubble>(256);

	public List<Bubble> fulcrumList = new List<Bubble>();

	public List<Bubble> noDropList = new List<Bubble>();

	public List<Bubble> growBubbleList = new List<Bubble>();

	public List<Bubble> growCandidateList = new List<Bubble>();

	public Dictionary<int, List<ChainBubble>> chainBubbleDic = new Dictionary<int, List<ChainBubble>>();

	private List<Bubble.eType> searchedBubbleTypeList = new List<Bubble.eType>(32);

	private List<Bubble> searchedBubbleList = new List<Bubble>();

	private BubbleNavi bubbleNavi;

	public GameObject lineFriendBase;

	private GameObject[] bubbleBonusBases = new GameObject[6];

	private int mBonusCoin;

	private int mBonusJewel;

	private Transform stageBg;

	private Transform stageUi;

	private Transform frontUi;

	private Transform scrollUi;

	public GameObject boundEffL;

	public GameObject boundEffR;

	private GameObject counter_eff;

	private Animation counter_eff_anm;

	private System.Random random = new System.Random();

	private GameObject clear_condition_stamp_00;

	private GameObject clear_condition_stamp_01;

	private eState mState;

	private eGameover gameoverType;

	public int shotCount;

	public float startTime;

	private int stageNo;

	private UISlider scoregauge;

	private float starRate1;

	private float starRate2;

	private GameObject[] stars;

	private UISpriteAnimation scoregauge_eff;

	private PopupScore popupScoreDrop;

	private PopupScore popupScoreReflect;

	private PopupScore popupScoreRescue;

	private Vector3 bonusBasePos = new Vector3(-285f, -120f, 0f);

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

	private Network.Avatar avatar = GlobalData.Instance.currentAvatar;

	public string[] CHARA_SPRITE_ANIMATION_HEADER = new string[2] { "chara_00_", "chara_01_" };

	public string waitAnimName = "chara_00_08_02_0";

	public string waitPinchAnimName = "chara_00_09_02_0";

	private float glossTime;

	private Bubble.eType glossType;

	private List<Bubble.eType> glossColorList = new List<Bubble.eType>();

	private GameObject bubble_trail_eff;

	private Vector3 bubble_trail_offset;

	private GameObject useitem_bg;

	private GameObject shootButton;

	private Arrow arrow;

	public Guide guide;

	private GameObject next_tap;

	private bool bOPAnime_;

	public Material fadeMaterial;

	private Bubble.eType noSearchColor;

	private List<Bubble> nearCoinBubbleList_ = new List<Bubble>();

	private Material gameoverMat;

	private int downLineCount;

	private float[] lineDownSec = new float[9] { 8f, 7.5f, 7f, 6.5f, 6f, 5.5f, 5f, 4.5f, 4f };

	private float timeA;

	private float timeB;

	private GameObject deadLine;

	public GameObject sweatEff;

	private System.Random rand = new System.Random();

	private bool isReturnPark_;

	private List<int> coinBubblePoint = new List<int>();

	private int[] bubbleColorArray = new int[5];

	private GameObject OP_event_00;

	private GameObject OP_event_01;

	private Animation opAnim_00;

	private Animation opAnim_01;

	private Animation bgAnim_;

	private GameObject[] charaObjs;

	private Transform launchpad_;

	private Vector3 random_vec;

	private int rainbowChainCount;

	private List<Bubble> frozenBreakList = new List<Bubble>();

	public List<Chackn> chacknList = new List<Chackn>();

	public float moveSpan;

	public bool isSearching;

	public bool isMoving;

	private Dictionary<int, BubbleBase> searchDict = new Dictionary<int, BubbleBase>();

	private List<KeyValuePair<BubbleBase, float>> ceilingList;

	private float ceilingBaseY;

	private Bubble upLeftBubble;

	private Bubble downRightBubble;

	private Dictionary<Bubble.eType, int> prevCreateBubble = new Dictionary<Bubble.eType, int>();

	private int minLine;

	private bool bGameOver;

	private Coroutine tayunCoroutine;

	public int nextScore;

	private int dispCoin = 1;

	private int tempCoin;

	public int nextCoin;

	private string[] charaNames;

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
			fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
			{
				fieldBubble.startGloss(glossType);
			});
			updateGlossTime();
		}
	}

	private void LateUpdate()
	{
		if (state < eState.Wait || state >= eState.Result)
		{
			return;
		}
		updateTotalScoreDisp();
		if (state < eState.Clear && !stagePause.pause && timeB != 0f)
		{
			timeB += Time.deltaTime;
			if ((downLineCount <= 8 && timeB - timeA >= lineDownSec[downLineCount]) || (downLineCount >= 9 && timeB - timeA >= lineDownSec[8]))
			{
				StartCoroutine(timeLineDown());
				timeA = timeB;
			}
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!(partManager == null) && !partManager.isTransitioning() && state < eState.Clear && pauseStatus && !stagePause.pause)
		{
			stagePause.pause = true;
			DialogPause dialogPause = (DialogPause)dialogManager.getDialog(DialogManager.eDialog.Pause);
			dialogPause.stagePause = stagePause;
			dialogPause.stageNo = stageNo;
			dialogPause.setup();
			partManager.StartCoroutine(dialogManager.openDialog(dialogPause));
		}
	}

	private IEnumerator OnButton(GameObject trigger)
	{
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
				shoot(arrow.fireVector);
			}
			break;
		}
		yield break;
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
		avatar = GlobalData.Instance.currentAvatar;
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
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.ThrowChara, 0, true, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.SupportChara, 0, true, 0, downloadCount));
		int throwChara = avatar.throwCharacter;
		int supportChara = avatar.supportCharacter;
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.ThrowChara, 0 + throwChara, 0, downloadCount));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.SupportChara, 0 + supportChara, 0, downloadCount));
		NetworkMng.Instance.forceIconDisable(false);
		GlobalData.Instance.ignoreLodingIcon = false;
		bubbleNavi = base.gameObject.AddComponent<BubbleNavi>();
		stageNo = 1;
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.BG, 9, 0, downloadCount));
		uiScale = (UnityEngine.Object.FindObjectOfType(typeof(UIRoot)) as UIRoot).transform.localScale.x;
		bonusBasePos *= uiScale;
		bubble_trail_eff = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "bubble_trail_eff")) as GameObject;
		bubble_trail_eff.name = "bubble_trail_eff";
		bubble_trail_offset = bubble_trail_eff.transform.localPosition;
		Utility.setParent(bubble_trail_eff, uiRoot.transform, false);
		bubble_trail_eff.SetActive(false);
		stagePause = base.gameObject.AddComponent<StagePause>();
		GameObject stageCollider = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "StageCollider")) as GameObject;
		Utility.setParent(stageCollider, uiRoot.transform, true);
		Transform ceilingCollider = stageCollider.transform.Find("Ceiling");
		ceilingCollider.localPosition += NGUIUtilScalableUIRoot.GetOffsetY(true);
		UnityEngine.Object bgResource = null;
		bgResource = ResourceLoader.Instance.loadGameObject("Prefabs/", "Stage_10");
		GameObject bg = UnityEngine.Object.Instantiate(bgResource) as GameObject;
		Utility.setParent(bg, uiRoot.transform, true);
		stageBg = bg.transform;
		GameObject ui = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "BonusStage_ui")) as GameObject;
		setupButton(ui);
		Utility.setParent(ui, uiRoot.transform, true);
		stageUi = ui.transform;
		frontUi = stageUi.Find("Front_ui");
		frontUi.parent = stageUi.parent;
		frontUi.localPosition += Vector3.back;
		deadLine = frontUi.Find("deadline_line_eff").gameObject;
		Vector3 linePos = new Vector3(0f, -200f, 0f);
		deadLine.transform.localPosition = linePos;
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
		coinRoot = frontUi.Find("Top_ui/clear_condition_top/00/clearscore");
		scoreList.Add(coinRoot.Find("score_number"));
		scoreList[0].name = "01";
		setupPopupExcellent("excellent", ref popupExcellent);
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
		arrow = launchpad.Find("arrow_pivot").GetComponent<Arrow>();
		arrow.bonusPart = this;
		arrow.bubblen = stageUi.Find(charaNames[0]).GetComponentInChildren<tk2dAnimatedSprite>();
		stepNextBubbleAnim = launchpad.GetComponent<Animation>();
		stepNextBubbleAnim.enabled = true;
		stepNextBubbleAnim.playAutomatically = false;
		stepNextBubbleAnim.Stop();
		stepNextBubbleAnim[stepNextBubbleClipName].clip.SampleAnimation(stepNextBubbleAnim.gameObject, 0f);
		next_tap = launchpad.Find("next_tap").gameObject;
		setNextTap(false);
		guide = base.gameObject.AddComponent<Guide>();
		guide.bonusPart = this;
		guide.guideline_pos = launchpad.Find("arrow_pivot/guideline_pos").gameObject;
		guide.uiScale = uiScale;
		arrow.guide = guide;
		Vector3 temp_ceiling_pos2 = ceilingCollider.localPosition;
		temp_ceiling_pos2.y -= 20f;
		ceilingCollider.localPosition = temp_ceiling_pos2;
		guide.setCeilingPos(ceilingCollider.position);
		temp_ceiling_pos2 = ceilingCollider.localPosition;
		temp_ceiling_pos2.y += 20f;
		ceilingCollider.localPosition = temp_ceiling_pos2;
		Transform bubble_bonus = frontUi.Find("bubble_bonus");
		bubbleBonusBases[0] = bubble_bonus.Find("00").gameObject;
		bubbleBonusBases[1] = bubble_bonus.Find("02").gameObject;
		bubbleBonusBases[2] = bubble_bonus.Find("03").gameObject;
		bubbleBonusBases[3] = bubble_bonus.Find("04").gameObject;
		bubbleBonusBases[4] = bubble_bonus.Find("05").gameObject;
		bubbleBonusBases[5] = bubble_bonus.Find("06").gameObject;
		bubble_bonus.gameObject.SetActive(false);
		int lineNum = 81;
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
		yield return null;
		GameObject nextBubbleRootObj = new GameObject("NextBubbleRoot");
		nextBubbleRoot = nextBubbleRootObj.transform;
		Utility.setParent(nextBubbleRootObj, uiRoot.transform, true);
		nextBubbleRoot.localPosition = new Vector3(bubbleRootPos.x, bubbleRootPos.y, -0.1f);
		float offset_y = 52 * lineNum - 52 * lineStart;
		bubbleObject = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "spr_bubble")) as GameObject;
		Utility.setParent(bubbleObject, base.transform, false);
		bubbleObject.SetActive(false);
		Bubble b = bubbleObject.GetComponent<Bubble>();
		b.bonusPart = this;
		b.stagePause = stagePause;
		Transform material_root = b.transform.Find("AS_spr_bubble");
		gameoverMat = new Material(material_root.GetComponent<tk2dAnimatedSprite>().GetComponent<Renderer>().material);
		gameoverMat.shader = Shader.Find("Custom/CustomGrayscale");
		nearCoinBubbleList_.Clear();
		List<int> bubbleColor = new List<int>();
		for (int x = 0; x < 5; x++)
		{
			int color = UnityEngine.Random.Range(0, 8);
			if (!bubbleColor.Contains(color))
			{
				bubbleColor.Add(color);
			}
			else
			{
				x--;
			}
		}
		bubbleColorArray = bubbleColor.ToArray();
		for (int k = 0; k < lineNum; k++)
		{
			coinBubblePoint.Clear();
			int offset = 30;
			int bubbleNum = 10;
			if (k % 2 == 1)
			{
				offset = 0;
			}
			for (int y = 0; y < 3; y++)
			{
				int point2 = 0;
				point2 = ((k % 2 != 0) ? UnityEngine.Random.Range(0, bubbleNum) : UnityEngine.Random.Range(0, bubbleNum - 1));
				if (!coinBubblePoint.Contains(point2))
				{
					coinBubblePoint.Add(point2);
				}
				else
				{
					y--;
				}
			}
			for (int i = 0; i < bubbleNum; i++)
			{
				if (k % 2 == 0 && i == bubbleNum - 1)
				{
					continue;
				}
				GameObject obj;
				Bubble bubble;
				if (k > 0)
				{
					int index = bubbleNum * (k - 1) + i;
					string bubble_name = string.Empty;
					if (coinBubblePoint.Contains(i))
					{
						bubble_name = "12";
					}
					else if (k >= 0 && k <= 8)
					{
						bubble_name = bubbleColorArray[UnityEngine.Random.Range(0, 3)].ToString("00");
					}
					else if (k >= 9 && k <= 14)
					{
						bubble_name = bubbleColorArray[UnityEngine.Random.Range(0, 4)].ToString("00");
					}
					else if (k >= 15)
					{
						bubble_name = bubbleColorArray[UnityEngine.Random.Range(0, 5)].ToString("00");
					}
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
					Bubble.eType type = bubble.type;
					if (type <= Bubble.eType.Black)
					{
						if (!glossColorList.Contains(type))
						{
							glossColorList.Add(type);
						}
					}
					else if (type == Bubble.eType.Coin)
					{
						nearCoinBubbleList_.Add(bubble);
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
				if (k == 0)
				{
					trans.localPosition = new Vector3(i * 60 + offset, (float)(k * -52) + offset_y, 0f);
				}
				else
				{
					trans.localPosition = new Vector3(i * 60 + offset, (float)((lineNum - k) * -52) + offset_y, 0f);
				}
				bubble.setFieldState();
				fieldBubbleList.Add(bubble);
				if (k == 0)
				{
					ceilingBubbleList.Add(trans);
				}
			}
		}
		Ceiling ceiling = frontUi.Find("stage_ceiling").gameObject.AddComponent<Ceiling>();
		ceiling.setTarget(ceilingBubbleList[0]);
		fadeMaterial = new Material(fieldBubbleList[0].transform.Find("AS_spr_bubble").GetComponent<Renderer>().sharedMaterial);
		fadeMaterial.shader = Shader.Find("Unlit/Transparent Colored");
		Transform stage_title = frontUi.Find("Top_ui/stage_title");
		stage_title.localPosition = new Vector3(stage_title.localPosition.x, stage_title.localPosition.y, -0.5f);
		updateTotalScoreDisp();
		updateFieldBubbleList();
		lineFriendBase = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Player_icon")) as GameObject;
		lineFriendBase.transform.Find("frame").localPosition = Vector3.back * 0.03f;
		lineFriendBase.transform.Find("Player_icon").localPosition = Vector3.back * 0.02f;
		Utility.setParent(lineFriendBase, uiRoot.transform, false);
		lineFriendBase.SetActive(false);
		yield return StartCoroutine(dialogManager.load(PartManager.ePart.BonusStage));
		GameObject scrollUiObj = new GameObject("ScrollUI");
		Utility.setParent(scrollUiObj, uiRoot.transform, true);
		scrollUi = scrollUiObj.transform;
		bubbleRoot.parent = scrollUi;
		nextBubbleRoot.parent = scrollUi;
		stageBg.parent = scrollUi;
		stageUi.parent = scrollUi;
		scoreParticleBase = frontUi.Find("erase_eff").gameObject;
		if (ResourceLoader.Instance.isUseLowResource())
		{
			UnityEngine.Object.Destroy(scoreParticleBase.transform.Find("particle").gameObject);
		}
		TutorialManager.Instance.load(stageNo, uiRoot);
		searchNextBubble();
		Sound.Instance.playBgm(Sound.eBgm.BGM_music_bonus, true);
		yield return stagePause.sync();
		startTime = Time.time;
		shotCount = 0;
		if (args.ContainsKey("isReturnPark"))
		{
			isReturnPark_ = (bool)args["isReturnPark"];
		}
		StartCoroutine(startRoutine(args));
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

	private IEnumerator startRoutine(Hashtable args)
	{
		yield return StartCoroutine(playOPAnime());
		setNextTap(true);
		yield return StartCoroutine(stepNextBubble());
		readyGo.gameObject.SetActive(true);
		yield return StartCoroutine(readyGo.play(stagePause));
		UnityEngine.Object.Destroy(readyGo);
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
		state = eState.Wait;
		showShootButton();
		timeA = Time.time;
		timeB = Time.time;
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
		}
		while (bOPAnime_ && bubbleRoot.GetComponent<iTween>() != null)
		{
			yield return stagePause.sync();
		}
		bOPAnime_ = false;
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
		Bubble component = nextBubbles[0].GetComponent<Bubble>();
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
		component.shot(shootVector);
		state = eState.Shot;
	}

	private void shootRandom()
	{
		bubbleNavi.stopNavi();
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "10");
		charaAnims[0].Play(CHARA_SPRITE_ANIMATION_HEADER[0] + "09");
		Bubble component = nextBubbles[1].GetComponent<Bubble>();
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
				Bubble hit_bubble = hitBubble.gameObject.GetComponent<Bubble>();
				if (hit_bubble != null && !hit_bubble.isColorBubble())
				{
					hitBubble = null;
					resarch_count++;
					continue;
				}
				Vector3 temp = nextBubbles[1].transform.position;
				shotBubble.myTrans.position = guide.hitPos;
				int[] checkedFlags = new int[fieldBubbleList.Count];
				List<Bubble> rainbowList = new List<Bubble>();
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

	public void hit(Bubble shotBubble, BubbleBase hitBubble)
	{
		if (shotBubble.type != Bubble.eType.Metal)
		{
			if (sweatEff.activeSelf)
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

	private IEnumerator hitRoutine(Bubble shotBubble, BubbleBase hitBubble)
	{
		moveSpan = 0f;
		rainbowChainCount = 0;
		yield return StartCoroutine(hitDefault(shotBubble));
		searchNextBubble();
		yield return StartCoroutine(stepNextBubble());
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
			StartCoroutine(gameoverRoutine());
		}
		else
		{
			state = eState.Wait;
		}
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
		if (rainbowChainCount == 0)
		{
			bubbleCountOffset++;
		}
		int bubbleCount2 = fieldBubbleList.Count + bubbleCountOffset;
		updateFieldBubbleList();
		yield return stagePause.sync();
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

	private void checkSamColor(Bubble me, ref int[] checkedFlags, List<Bubble> rainbowList)
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
			if (me.GetInstanceID() != bubble.GetInstanceID() && checkedFlags[i] == 0 && bubble.state == Bubble.eState.Field && !bubble.isLocked && !bubble.isFrozen && me.isNearBubble(bubble))
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
				for (int k = 0; k < fieldBubbleList.Count; k++)
				{
					if (checkedFlags[k] == 1)
					{
						if (rainbowChainCount > 0)
						{
							fieldBubbleList[k].isLineFriend = false;
						}
						fieldBubbleList[k].startBreak();
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
				for (int j = 0; j < rainbowList.Count; j++)
				{
					Bubble t = rainbowList[j];
					if (t.type == Bubble.eType.FriendRainbow)
					{
						t.type = convertColorBubble(shotBubble.type) + 31;
					}
					else
					{
						t.type = convertColorBubble(shotBubble.type);
					}
					int type = (int)t.type;
					t.name = type.ToString("00");
					t.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_" + t.name);
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
			List<Bubble> nearBubbleList = new List<Bubble>(6);
			Bubble shotBubble2 = default(Bubble);
			fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
			{
				if (!fieldBubble.isLocked && fieldBubble.state == Bubble.eState.Field && shotBubble2.isNearBubble(fieldBubble))
				{
					nearBubbleList.Add(fieldBubble);
				}
			});
			foreach (Bubble nearBubble in nearBubbleList)
			{
				bool bBreak = true;
				Bubble.eType type2 = nearBubble.type;
				if (type2 == Bubble.eType.Coin)
				{
					specialEffect(nearBubble);
				}
				else
				{
					bBreak = false;
				}
				if (bBreak)
				{
					nearBubble.startBreak();
					breakNum_++;
					shotBubble.startBreak();
					fieldBubbleList.Remove(shotBubble);
				}
			}
		}
		if (breakNum_ > 0)
		{
			yield return StartCoroutine(breakRoutine(shotBubble, 0));
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
			updateFieldBubbleList();
		}
	}

	private void specialEffect(Bubble bubble)
	{
		if (bubble.state == Bubble.eState.Field)
		{
			Bubble.eType type = bubble.type;
			if (type == Bubble.eType.Coin)
			{
				coinBubbleCount++;
				Sound.Instance.playSe(Sound.eSe.SE_334_coinbubble);
				totalEventCoin += COIN_ADD_VALUE;
				nextCoin = totalEventCoin;
				friendBonusPop(eFriendBonus.Coin, COIN_ADD_VALUE, bubble.myTrans.parent, bubble.myTrans.position);
			}
		}
	}

	public Transform getScrollUI()
	{
		return scrollUi;
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
		searchedBubbleTypeList.Clear();
		searchedBubbleList.Clear();
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
		}
		addSerchedBubbleType();
	}

	private void addSerchedBubbleType()
	{
		float num = -363f;
		foreach (KeyValuePair<int, BubbleBase> item in searchDict)
		{
			Bubble.eType type = item.Value.type;
			if (!checkSearchExclusion(type) && !(item.Value.myTrans.localPosition.y > ceilingBaseY + 1f) && (!(item.Value is Bubble) || !((Bubble)item.Value).isFrozen))
			{
				searchedBubbleList.Add((Bubble)item.Value);
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
			List<Bubble.eType> colorList = new List<Bubble.eType>();
			List<Bubble.eType> rockList = new List<Bubble.eType>();
			fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
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
		for (int j = 0; j < nextBubbleCount - 1; j++)
		{
			bool exist = false;
			Bubble.eType nextBubbleType = nextBubbles[j + 1].GetComponent<Bubble>().type;
			for (int k = 0; k < fieldBubbleList.Count; k++)
			{
				if (!fieldBubbleList[k].isFrozen && (!fieldBubbleList[k].isLocked || searchedBubbleTypeList.Count <= 0) && nextBubbleType == convertColorBubble(fieldBubbleList[k].type))
				{
					exist = true;
					break;
				}
			}
			if (exist)
			{
				nextBubbles[j] = nextBubbles[j + 1];
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
		createNextBubble(nextBubbleCount - 1, false);
		bubbleNavi.startNavi(searchedBubbleList, nextBubbles[0].GetComponent<Bubble>().type);
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
		charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "11_00_1");
		while (charaAnims[1].IsPlaying(CHARA_SPRITE_ANIMATION_HEADER[1] + "11_00_1"))
		{
			yield return stagePause.sync();
		}
		charaAnims[1].Play(waitAnimName);
	}

	private void updateFieldBubbleList()
	{
		fulcrumList.Clear();
		noDropList.Clear();
		List<Bubble> list = new List<Bubble>(fieldBubbleList);
		fieldBubbleList.Clear();
		minLine = 0;
		list.ForEach(delegate(Bubble fieldBubble)
		{
			if (fieldBubble.state == Bubble.eState.Field)
			{
				fieldBubbleList.Add(fieldBubble);
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
		if (minLine < -12)
		{
			bGameOver = true;
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
			List<Bubble> list = new List<Bubble>();
			for (int k = 0; k < fieldBubbleList.Count; k++)
			{
				if (checkedFlags[k] == 1)
				{
					checkedFlags[k]++;
					Bubble.eType type = fieldBubbleList[k].type;
					if (type != Bubble.eType.Fulcrum && type != Bubble.eType.Rock && type != Bubble.eType.RotateFulcrumL && type != Bubble.eType.RotateFulcrumR)
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

	private void checkDrop(Bubble me, ref int[] checkedFlags, ref bool isDrop, bool checkFulcrum)
	{
		Bubble.eType type = me.type;
		if (type == Bubble.eType.Blank)
		{
			isDrop = false;
		}
		if (checkFulcrum && (type == Bubble.eType.Fulcrum || type == Bubble.eType.Rock || type == Bubble.eType.RotateFulcrumL || type == Bubble.eType.RotateFulcrumR))
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
					if (!(fulcrum == fieldBubble) && fieldBubble.type != Bubble.eType.Blank && fieldBubble.type != Bubble.eType.Fulcrum && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fieldBubble.type != Bubble.eType.Rock && fieldBubble.type != Bubble.eType.Box && fieldBubble.type != Bubble.eType.FriendBox && fieldBubble.type != Bubble.eType.Counter && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR && fulcrum.isNearBubble(fieldBubble))
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

	private IEnumerator timeLineDown()
	{
		while (state != eState.Wait)
		{
			yield return 0;
		}
		int line = -1;
		eState prevState = state;
		state = eState.Scroll;
		float diffTime = Time.time - startTime;
		Vector3 offset = Vector3.up * (52 * line);
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
		{
			fieldBubble.myTrans.localPosition += offset;
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
		iTween.MoveTo(bubbleRoot.gameObject, iTween.Hash("y", basePos.y + moveY2, "easetype", iTween.EaseType.easeInCubic, "time", moveTime, "islocal", true));
		while (bubbleRoot.GetComponent<iTween>() != null)
		{
			yield return stagePause.sync();
		}
		tayunCoroutine = StartCoroutine(tayun(basePos.y + moveY2 + moveY3, 0.8f));
		startTime = Time.time - diffTime;
		downLineCount++;
		if (downLineCount >= lineDownSec.Length)
		{
			downLineCount = lineDownSec.Length - 1;
		}
		updateFieldBubbleList();
		sweatCheck();
		if (bGameOver)
		{
			StartCoroutine(gameoverRoutine());
			yield break;
		}
		if (guide.isShootButton)
		{
			guide.lineUpdate();
		}
		state = prevState;
		yield return 0;
	}

	private void sweatCheck()
	{
		sweatEff.SetActive(minLine < -10);
		waitAnimChange();
	}

	private void waitAnimChange()
	{
		arrow.updateWaitAnimationImmediate();
		if (sweatEff.activeSelf)
		{
			charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "12");
		}
		else
		{
			charaAnims[1].Play(CHARA_SPRITE_ANIMATION_HEADER[1] + "07");
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
				for (int j = 0; j < chainBubbleDic.Count; j++)
				{
					foreach (ChainBubble chainBubble in chainBubbleDic[j])
					{
						float posY = chainBubble.myTrans.localPosition.y;
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
		if (line != 0)
		{
			eState prevState = state;
			state = eState.Scroll;
			float diffTime = Time.time - startTime;
			Vector3 offset = Vector3.up * (52 * line);
			fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
			{
				fieldBubble.myTrans.localPosition += offset;
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
			iTween.MoveTo(bubbleRoot.gameObject, iTween.Hash("y", basePos.y + moveY2, "easetype", iTween.EaseType.easeInCubic, "time", moveTime, "islocal", true));
			while (bubbleRoot.GetComponent<iTween>() != null)
			{
				yield return stagePause.sync();
			}
			tayunCoroutine = StartCoroutine(tayun(basePos.y + moveY2 + moveY3, 0.8f));
			startTime = Time.time - diffTime;
			state = prevState;
		}
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

	private IEnumerator clearRoutine()
	{
		state = eState.Clear;
		Input.enable = false;
		frontUi.Find("Top_ui/Gamestop_Button").gameObject.SetActive(false);
		Sound.Instance.playBgm(Sound.eBgm.BGM_007_Clear, false);
		stageClear.SetActive(true);
		while (stageClear.GetComponent<Animation>().isPlaying)
		{
			yield return stagePause.sync();
		}
		Input.enable = true;
		yield return StartCoroutine(resultSetup(true));
		yield return null;
	}

	private IEnumerator resultSetup(bool hoge)
	{
		DialogBonusRoulette dialog = dialogManager.getDialog(DialogManager.eDialog.BonusRoulette) as DialogBonusRoulette;
		dialog.setup(totalEventCoin, coinBubbleCount);
		Sound.Instance.playSe(Sound.eSe.SE_239_tassei);
		yield return StartCoroutine(dialogManager.openDialog(dialog));
		while (dialog.isOpen())
		{
			yield return 0;
		}
		Hashtable args = new Hashtable { { "IsForceSendInactive", true } };
		if (isReturnPark_)
		{
			partManager.requestTransition(PartManager.ePart.Park, args, true);
			yield break;
		}
		partManager.bTransitionMap_ = true;
		partManager.requestTransition(PartManager.ePart.Map, args, true);
	}

	private IEnumerator gameoverRoutine()
	{
		Sound.Instance.stopBgm();
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
		int index = 0;
		fieldBubbleList.ForEach(delegate(Bubble fieldBubble)
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
		guide.setActive(false);
		yield return StartCoroutine(resultSetup(false));
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
		for (int i = 1; i < nextBubbleCount; i++)
		{
			nextBubbles[i].SetActive(true);
		}
	}

	private void tutorialStart()
	{
		Bubble.eType type = (Bubble.eType)TutorialManager.Instance.getHighlightBubble(stageNo);
		if (type == Bubble.eType.Invalid)
		{
			return;
		}
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
			default:
				if (fieldBubble.type == type)
				{
					fieldBubble.myTrans.localPosition += Vector3.back * 46f;
				}
				break;
			}
		});
		if (type != Bubble.eType.ChainHorizontal && type != Bubble.eType.ChainLeftDown && type != Bubble.eType.ChainRightDown && type != Bubble.eType.ChainLock)
		{
			return;
		}
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

	private void tutorialStartObjects(int spNum)
	{
	}

	private void tutorialEnd()
	{
		Bubble.eType type = (Bubble.eType)TutorialManager.Instance.getHighlightBubble(stageNo);
		if (type == Bubble.eType.Invalid)
		{
			return;
		}
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
			default:
				if (fieldBubble.type == type)
				{
					fieldBubble.myTrans.localPosition += Vector3.forward * 46f;
				}
				break;
			}
		});
		if (type != Bubble.eType.ChainHorizontal && type != Bubble.eType.ChainLeftDown && type != Bubble.eType.ChainRightDown && type != Bubble.eType.ChainLock)
		{
			return;
		}
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

	private void tutorialEndObjects(int spNum)
	{
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
		arrow.pinchAnimNames = new string[2]
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
}
