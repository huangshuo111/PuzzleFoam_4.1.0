namespace Network
{
	public class GameData
	{
		public int resultCode;

		public int bonusJewel;

		public int buyJewel;

		public int lastStageStatus;

		public int lastStageNo;

		public int treasureboxNum;

		public int heart;

		public int coin;

		public int heartRecoverySsRemaining;

		public int exp;

		public int level;

		public int progressStageNo;

		public int allPlayCount;

		public int allClearCount;

		public int allStarSum;

		public int allStageScoreSum;

		public int maxLevel;

		public int continueNum;

		public StageData[] stageList;

		public TreasureData[] treasureList;

		public InformationData[] informationList;

		public int mailUnReadCount;

		public int helpDataSize;

		public int helpMove;

		public int helpTime;

		public int bonusChanceLv;

		public bool isCoinCampaign;

		public bool isJewelCampaign;

		public bool isHeartCampaign;

		public int heartRecoverTime;

		public bool isCoinupCampaign;

		public int coinup;

		public string aBillKey;

		public bool iadFlg;

		public bool iosInvite;

		public string guestId;

		public bool heartRecvFlg;

		public bool market_review;

		public string limited_event;

		public bool ToastPromotion;

		public bool TNKBanner;

		public bool collaboBBActive;

		public MonetizationConfig monetization;

		public int rewardType;

		public int rewardNum;

		public bool isHeartSendCampaign;

		public int heartSendHour;

		public bool isHeartShopCampaign;

		public bool progressStageOpen;

		public int[] saleArea;

		public int areaSalePercent;

		public bool isAreaCampaign;

		public int[] saleStageItemArea;

		public int stageItemAreaSalePercent;

		public bool isStageItemAreaCampaign;

		public long eventTimeSsRemaining;

		public int eventMaxStageNo;

		public long dailyMissionSsRemaining;

		public InviteBasicReward inviteBasicReward;

		public bool isBossOpen;

		public UserItemList[] userItemList;

		public string comeback;

		public bool firstLogin;

		public int beginnerCampaignLastDay;

		public int[] beginnerCampaignPushAlert;

		public bool isFirstGacha;

		public Avatar[] avatarList;

		public bool isAllAvatarLevelMax;

		public bool isAllAvatarLevelMax2;

		public int gachaCoinPrice;

		public int gachaJewelPrice;

		public int gachaPremiumTicketPrice;

		public int gachaNormalTicketPrice;

		public bool isGachaUpCampaign;

		public bool isGachaSaleCampaign;

		public float gachaSalePercent;

		public float gachaUp;

		public int gachaTicket;

		public int helpRewardCount;

		public int skipPriceType;

		public int skipPrice;

		public bool isSkip;

		public bool isPackageShopCampaign;

		public string gacha_banner_url;

		public string gacha_detail_url;

		public string gacha_banner_url2;

		public string gacha_detail_url2;

		public GachaDrawList.AvatarInfo[] gachaList;

		public GachaDrawList.AvatarInfo[] gachaList2;

		public GachaDrawList.RatioList totalRatioList;

		public GachaDrawList.RatioList totalRatioList2;

		public int minilenCount;

		public int minilenTotalCount;

		public int giveNiceTotalCount;

		public int giveNiceMonthlyCount;

		public int tookNiceTotalCount;

		public bool isParkDailyReward;

		public StageData[] parkStageList;

		public MinilenData[] minilenList;

		public MinilenThanksData[] thanksList;

		public BuildingData[] buildings;

		public ParkNiceList niceList;

		public ParkDailyRewardData daily_reward;

		public int mapReleaseNum;

		public int minilenParkTutorialStatus;

		public AttendanceConfig attendance;

		public void setCommonData(CommonData commonData, bool bCampaignCopy)
		{
			bonusJewel = commonData.bonusJewel;
			buyJewel = commonData.buyJewel;
			lastStageStatus = commonData.lastStageStatus;
			lastStageNo = commonData.lastStageNo;
			treasureboxNum = commonData.treasureboxNum;
			heart = commonData.heart;
			coin = commonData.coin;
			exp = commonData.exp;
			level = commonData.level;
			progressStageNo = commonData.progressStageNo;
			allPlayCount = commonData.allPlayCount;
			allClearCount = commonData.allClearCount;
			allStarSum = commonData.allStarSum;
			allStageScoreSum = commonData.allStageScoreSum;
			heartRecoverySsRemaining = commonData.heartRecoverySsRemaining;
			userItemList = commonData.userItemList;
			gachaUp = commonData.gachaUp;
			gachaSalePercent = commonData.gachaSalePercent;
			aBillKey = commonData.aBillKey;
			market_review = commonData.market_review;
			iadFlg = commonData.iadFlg;
			guestId = commonData.guestId;
			limited_event = commonData.limited_event;
			ToastPromotion = commonData.ToastPromotion;
			TNKBanner = commonData.TNKBanner;
			collaboBBActive = commonData.collaboBBActive;
			if (commonData.monetization != null)
			{
				monetization = commonData.monetization;
			}
			rewardType = commonData.rewardType;
			rewardNum = commonData.rewardNum;
			heartRecvFlg = commonData.heartRecvFlg;
			progressStageOpen = commonData.progressStageOpen;
			if (bCampaignCopy)
			{
				isCoinCampaign = commonData.isCoinCampaign;
				isJewelCampaign = commonData.isJewelCampaign;
				isHeartCampaign = commonData.isHeartCampaign;
				heartRecoverTime = commonData.heartRecoverTime;
				isCoinupCampaign = commonData.isCoinupCampaign;
				coinup = commonData.coinup;
				isHeartSendCampaign = commonData.isHeartSendCampaign;
				heartSendHour = commonData.heartSendHour;
				isHeartShopCampaign = commonData.isHeartShopCampaign;
				isBossOpen = commonData.isBossOpen;
				isFirstGacha = commonData.isFirstGacha;
				isGachaSaleCampaign = commonData.isGachaSaleCampaign;
				isGachaUpCampaign = commonData.isGachaUpCampaign;
			}
			minilenCount = commonData.minilenCount;
			minilenTotalCount = commonData.minilenTotalCount;
			giveNiceTotalCount = commonData.giveNiceTotalCount;
			giveNiceMonthlyCount = commonData.giveNiceMonthlyCount;
			tookNiceTotalCount = commonData.tookNiceTotalCount;
			isParkDailyReward = commonData.isParkDailyReward;
			attendance = commonData.attendance;
		}

		public void setEventData(CommonData commonData)
		{
			eventMaxStageNo = commonData.eventMaxStageNo;
			eventTimeSsRemaining = commonData.eventTimeSsRemaining;
		}

		public void setDailyMissionData(CommonData commonData)
		{
			dailyMissionSsRemaining = commonData.dailyMissionSsRemaining;
		}

		public void setGachaTopData(GachaTop resultData)
		{
			gachaCoinPrice = resultData.gachaCoinPrice;
			gachaJewelPrice = resultData.gachaJewelPrice;
			gachaPremiumTicketPrice = resultData.gachaPremiumTicketPrice;
			gachaNormalTicketPrice = resultData.gachaNormalTicketPrice;
			isFirstGacha = resultData.isFirstGacha;
			isGachaUpCampaign = resultData.isGachaUpCampaign;
			gachaUp = resultData.gachaUp;
			isGachaSaleCampaign = resultData.isGachaSaleCampaign;
			gachaSalePercent = resultData.gachaSalePercent;
			gachaTicket = resultData.gachaTicket;
			gacha_banner_url = resultData.gacha_banner_url;
			gacha_detail_url = resultData.gacha_detail_url;
			gacha_banner_url2 = resultData.gacha_banner_url2;
			gacha_detail_url2 = resultData.gacha_detail_url2;
		}

		public void setParkData(StageData[] park_stages, MinilenData[] minilens, MinilenThanksData[] minilen_thanks, BuildingData[] buildings, int mapReleaseNum = -1)
		{
			if (park_stages != null)
			{
				parkStageList = park_stages;
			}
			if (minilens != null)
			{
				minilenList = minilens;
			}
			if (minilen_thanks != null)
			{
				thanksList = minilen_thanks;
			}
			if (buildings != null)
			{
				this.buildings = buildings;
			}
			if (mapReleaseNum >= 0)
			{
				this.mapReleaseNum = mapReleaseNum;
			}
		}
	}
}
