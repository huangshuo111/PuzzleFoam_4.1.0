using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class GlobalData : MonoBehaviour
{
	public class openAvatarCollectionInfo
	{
		public bool isSetup;

		public DialogManager.eDialog dialog = DialogManager.eDialog.Setup;

		public StageIcon stageIcon;

		public PartBase part_;
	}

	public class AvatarInfo
	{
		public Avatar[] AvatarList;
	}

	public class Avatar
	{
		public int AvatarIndex;

		public int Rank;

		public string StartTime;

		public int ThrowCharacter;

		public int SupportCharacter;

		public int LevelLimit_1;

		public int LevelLimit_2;

		public int BaseSkill_1;

		public int BaseSkill_2;

		public int BaseSkill_3;

		public int SpecialSkill;

		public LevelupInfo[] LevelupInfos;
	}

	public class LevelupInfo
	{
		public int Level;

		public int CoinLevelup;

		public int JewelLevelup;
	}

	public class GachaInfo
	{
		public AvatarRate[] AvatarRates;
	}

	public class AvatarRate
	{
		public int Index;

		public string StartTime;

		public int Rank;

		public int Rate;
	}

	public enum GKEvent
	{
		Halloween = 2
	}

	private static GlobalData instance_;

	public long LineID;

	private GameData gameData_;

	private ContinueSaleData continueData_ = new ContinueSaleData();

	private Mission dailyMission_;

	private BonusStartData bonusStageData_;

	private Network.StageData rankingStageData_;

	private KeyBubbleData keyBubbleData_ = new KeyBubbleData();

	private Dictionary<int, Network.StageData> stageDataDict_ = new Dictionary<int, Network.StageData>();

	private int normalStageNum;

	public bool ignoreLodingIcon;

	public bool isResourceDownloading;

	public int GachaPriceJewel = 10;

	public int GachaPriceCoin = 10000;

	public int GachaPricePremiumTicket = 5;

	public int GachaPriceNormalTicket = 1;

	public int RatioPremiumSS = 3;

	public int RatioPremiumS = 17;

	public int RatioPremiumA = 80;

	public int RatioPremiumB;

	public int RatioNormalSS;

	public int RatioNormalS;

	public int RatioNormalA = 20;

	public int RatioNormalB = 80;

	public Network.Avatar currentAvatar;

	[HideInInspector]
	public bool isBasicSkill = true;

	public int[] avatarCount;

	public Vector3 mapJumpDefaultPos;

	public int COLLABO_STAGE_NUM = 20;

	public openAvatarCollectionInfo acInfo_ = new openAvatarCollectionInfo();

	public bool[] bSetItems;

	public BoostItem[] boostItem;

	public bool isGachaAfterOpeningDialog;

	public bool isParkControlAfterOpeningDialog;

	public SaveAvatarData avatarData;

	public float chara_img_expand_rate = 1.25f;

	public float chara_img_diminish_rate = 1.6f;

	public GameObject sysfont_label;

	public int[] unlockedStages;

	private bool isThai_;

	private AchievementData[] achievementData_;

	private int GKEventFlag_;

	public static GlobalData Instance
	{
		get
		{
			return instance_;
		}
	}

	public void set_acInfo(bool isSetup, DialogManager.eDialog dialog, PartBase part, StageIcon icon)
	{
		if (acInfo_ == null)
		{
			acInfo_ = new openAvatarCollectionInfo();
		}
		acInfo_.isSetup = isSetup;
		acInfo_.dialog = dialog;
		acInfo_.part_ = part;
		acInfo_.stageIcon = icon;
	}

	public void setItems(BoostItem[] items_)
	{
		boostItem = items_;
		if (bSetItems == null)
		{
			bSetItems = new bool[items_.Length];
		}
		for (int i = 0; i < items_.Length; i++)
		{
			if (items_[i].getState() == BoostItem.eState.ON)
			{
				bSetItems[i] = true;
			}
			else
			{
				bSetItems[i] = false;
			}
		}
	}

	public bool[] getSetedItems()
	{
		return bSetItems;
	}

	public BoostItem[] getBoostItem()
	{
		return boostItem;
	}

	public Network.Avatar defaultAvatar()
	{
		Network.Avatar avatar = new Network.Avatar();
		avatar.index = 20000;
		avatar.rank = 0;
		avatar.throwCharacter = 0;
		avatar.supportCharacter = 0;
		avatar.baseSkill_1 = 1;
		avatar.baseSkill_2 = 2;
		avatar.baseSkill_3 = 3;
		avatar.specialSkill = 0;
		avatar.level = 1;
		avatar.limitLevel = 5;
		avatar.limitLevelMax = 10;
		avatar.coinLevelup = 15000;
		avatar.jewelLevelup = 30;
		avatar.wearFlg = 1;
		avatar.limitover = 0;
		return avatar;
	}

	public void setAvatarData()
	{
		avatarData = new SaveAvatarData();
		avatarData.setup();
	}

	public IEnumerator createSysfontTexture()
	{
		sysfont_label.SetActive(true);
		yield return null;
		sysfont_label.SetActive(false);
	}

	public void setRankingStageData(Network.StageData data)
	{
		rankingStageData_ = data;
	}

	public Network.StageData getRankingStageData()
	{
		return rankingStageData_;
	}

	public static bool IsInstance()
	{
		return (!(instance_ == null)) ? true : false;
	}

	private void Awake()
	{
		if (instance_ == null)
		{
			instance_ = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		instance_ = null;
	}

	public GameData getGameData()
	{
		if (gameData_ != null)
		{
			gameData_.collaboBBActive = true;
		}
		return gameData_;
	}

	public void setGameData(GameData data)
	{
		gameData_ = data;
		setStageDictionary(gameData_.stageList);
	}

	public void setAchievememtData(AchievementData[] data)
	{
		achievementData_ = data;
	}

	public AchievementData[] getAchievementData()
	{
		return achievementData_;
	}

	public void setPlayerData(PlayerData playerData)
	{
		gameData_.setCommonData(playerData, true);
		gameData_.treasureList = playerData.treasureList;
		gameData_.mailUnReadCount = playerData.mailUnReadCount;
		gameData_.setParkData(playerData.parkStageList, playerData.minilenList, playerData.minilenThanksList, playerData.buildings, playerData.mapReleaseNum);
		gameData_.heartRecvFlg = playerData.heartRecvFlg;
		setStageDictionary(playerData.stageList);
	}

	public void setMarketReviewData(bool flag)
	{
		gameData_.market_review = flag;
	}

	public void setStageDictionary(Network.StageData[] stageDatas)
	{
		stageDataDict_.Clear();
		normalStageNum = 0;
		foreach (Network.StageData stageData in stageDatas)
		{
			if (Constant.Event.isEventStage(stageData.stageNo) || Constant.ParkStage.isParkStage(stageData.stageNo))
			{
				stageDataDict_[stageData.stageNo] = stageData;
				continue;
			}
			stageDataDict_[stageData.stageNo - 1] = stageData;
			normalStageNum++;
		}
	}

	public bool isStageData(int stageNo)
	{
		if (Constant.ParkStage.isParkStage(stageNo))
		{
			return Array.Exists(getGameData().parkStageList, (Network.StageData ps) => ps.stageNo == stageNo);
		}
		return stageDataDict_.ContainsKey(stageNo);
	}

	public Network.StageData getStageData(int stageNo)
	{
		if (Constant.ParkStage.isParkStage(stageNo))
		{
			return Array.Find(gameData_.parkStageList, (Network.StageData ps) => ps.stageNo == stageNo);
		}
		if (!isStageData(stageNo))
		{
			return null;
		}
		return stageDataDict_[stageNo];
	}

	public int getNormalStageNum()
	{
		return normalStageNum;
	}

	public ContinueSaleData getContinueData()
	{
		return continueData_;
	}

	public void setContinueData(ContinueSaleData data)
	{
		continueData_ = data;
	}

	public void setInviteRewardData(InviteBasicReward data)
	{
		gameData_.inviteBasicReward = data;
	}

	public void setDailyMissionData(Mission data)
	{
		dailyMission_ = data;
	}

	public Network.DailyMission getDailyMissionData()
	{
		if (dailyMission_ == null)
		{
			return null;
		}
		return dailyMission_.dailyMission;
	}

	public void setBonusStartData(BonusStartData data)
	{
		bonusStageData_ = data;
	}

	public BonusStartData getBonusStartData()
	{
		return bonusStageData_;
	}

	public void setMapJumpDefaultPos(Vector3 pos)
	{
		mapJumpDefaultPos = pos;
	}

	public Vector3 getMapJumpDefaultPos()
	{
		return mapJumpDefaultPos;
	}

	public void setKeyBubbleData(KeyBubbleData data)
	{
		keyBubbleData_ = data;
	}

	public KeyBubbleData getKeyBubbleData()
	{
		return keyBubbleData_;
	}

	public void SetGKEventFlag()
	{
		GKEventFlag_ = 0;
		if (gameData_.limited_event == null || gameData_.limited_event.Equals("None"))
		{
			return;
		}
		string[] array = gameData_.limited_event.Split(',');
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.Equals("halloween"))
			{
				GKEventFlag_ += 2;
			}
		}
	}

	public bool IsGKEvent(GKEvent type)
	{
		return ((uint)GKEventFlag_ & (uint)type) != 0;
	}

	public int getStageCount()
	{
		return stageDataDict_.Count;
	}
}
