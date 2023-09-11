using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Network;
using TnkAd;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
	public enum eDialog
	{
		Common = 0,
		Information = 1,
		BuyLimit = 2,
		PlayScore = 3,
		JewelShop = 4,
		HeartShop = 5,
		GiftJewelShop = 6,
		Setup = 7,
		Option = 8,
		Ranking = 9,
		Award = 10,
		Mail = 11,
		NoMail = 12,
		AppQuit = 13,
		CoinShop = 14,
		DayRanking = 15,
		InviteConfirm = 16,
		InviteSent = 17,
		Invite = 18,
		InviteNone = 19,
		Present = 20,
		RequestNone = 21,
		Request = 22,
		RequestConfirm = 23,
		ItemHelp = 24,
		Pause = 25,
		LevelUp = 26,
		SendLevel = 27,
		SendHighScore = 28,
		SendMaxScore = 29,
		NetworkError = 30,
		ChangeRanking = 31,
		ResultClear = 32,
		ResultFailed = 33,
		Continue = 34,
		ADContinue = 35,
		HighScoreList = 36,
		Reward = 37,
		StageShop = 38,
		Shortage = 39,
		ExitConfirm = 40,
		RetryConfirm = 41,
		LimitOver = 42,
		OptionOperate = 43,
		OptionLangage = 44,
		EventSetup = 45,
		ToBeContinue = 46,
		HowToPlay = 47,
		Maintenance = 48,
		SendAreaClear = 49,
		Nologin = 50,
		LowMemory = 51,
		DataDownload = 52,
		CapacityShortage = 53,
		GameRestart = 54,
		FriendHelp = 55,
		PurchaseInfo = 56,
		LuckyChance = 57,
		HowToPlayIndex = 58,
		SendTreasure = 59,
		ClearBonus = 60,
		ClearBonusCollabo = 61,
		RecommendedItem = 62,
		Logout = 63,
		Unregister = 64,
		Policy = 65,
		AreaLock = 66,
		Credit = 67,
		EventContinued = 68,
		EventSendInfo = 69,
		EventSendPopup = 70,
		Review = 71,
		StarReward = 72,
		ClearBonusInfo = 73,
		PlayerInfo = 74,
		ChallengeSetup = 75,
		ChallengeContinued = 76,
		DailyMission = 77,
		DailyMissionClear = 78,
		DailyMissionCancel = 79,
		BonusRoulette = 80,
		BonusRouletteCancel = 81,
		SendBonusCoin = 82,
		PauseBoss = 83,
		ContinueBoss = 84,
		ResultFailedBoss = 85,
		ResultClearBoss = 86,
		BossSetup = 87,
		BossSelect = 88,
		BossReward = 89,
		BossStageShop = 90,
		CollaborationSetup = 91,
		OptionAvatar = 92,
		SendBoss = 93,
		Comeback = 94,
		BannerInformation = 95,
		AvatarGacha = 96,
		AvatarGachaResult = 97,
		AvatarLevelup = 98,
		AvatarLevelupAnim = 99,
		AvatarProfile = 100,
		AvatarCollection = 101,
		CrossMission = 102,
		AttendCheck = 103,
		SendMap = 104,
		ContinueRanking = 105,
		ClearRanking = 106,
		StageSkip = 107,
		AllShop = 108,
		GachaContents = 109,
		AirArmorRouting = 110,
		AirArmorCheat = 111,
		ParkAreaList = 112,
		ParkStageList = 113,
		ParkStageSetup = 114,
		ParkStageClear = 115,
		ParkStageMinilenGet = 116,
		ParkMinilenThanks = 117,
		ParkMinilenThanksDetail = 118,
		ParkMinilenThanksGet = 119,
		ParkBuildingList = 120,
		ParkRoadList = 121,
		ParkMinilenCollection = 122,
		ParkMinilenProfile = 123,
		ParkObjectChoices = 124,
		ParkRemoveConfirm = 125,
		ParkNiceHistoryList = 126,
		ParkNiceDetail = 127,
		ParkSendNiceConfirm = 128,
		ParkFriendList = 129,
		ParkRewardCheckList = 130,
		ParkReleaseNotice = 131,
		RankingStage = 132,
		RankingSetup = 133,
		RankingContinue = 134,
		RankingResultClear = 135,
		ModeSelect = 136,
		Max = 137
	}

	private class DialogResourceInfo
	{
		public string ResourceName = string.Empty;

		public string ComponentName = string.Empty;

		public bool IsClearTransform;

		public DialogResourceInfo(string name)
		{
			ResourceName = name;
		}

		public DialogResourceInfo(string name, string component)
		{
			ResourceName = name;
			ComponentName = component;
		}

		public DialogResourceInfo(string name, string component, bool bClearTransform)
		{
			ResourceName = name;
			ComponentName = component;
			IsClearTransform = bClearTransform;
		}
	}

	public delegate void OnStartLoad(eDialog type);

	public delegate void OnFinishLoad(eDialog type);

	public static int DialogMax = 137;

	[SerializeField]
	private PartManager partManager;

	[SerializeField]
	private FadeMng fade;

	private static Dictionary<eDialog, DialogResourceInfo> DialogNameTable = new Dictionary<eDialog, DialogResourceInfo>
	{
		{
			eDialog.AppQuit,
			new DialogResourceInfo("BackKey_Panel", "DialogAppQuit")
		},
		{
			eDialog.Award,
			new DialogResourceInfo("Award_Panel", "DialogRoulette")
		},
		{
			eDialog.BuyLimit,
			new DialogResourceInfo("Buylimit_Panel", "DialogConfirm")
		},
		{
			eDialog.Common,
			new DialogResourceInfo("CommonDialog_Panel", "DialogCommon")
		},
		{
			eDialog.CoinShop,
			new DialogResourceInfo("CoinCharge_Panel", "DialogCoinShop")
		},
		{
			eDialog.Continue,
			new DialogResourceInfo("StageContinue_Panel", "DialogContinue")
		},
		{
			eDialog.ADContinue,
			new DialogResourceInfo("AD_StageContinue_Panel", "DialogADContinue")
		},
		{
			eDialog.ChangeRanking,
			new DialogResourceInfo("RankChange_Panel", "DialogChangeRanking")
		},
		{
			eDialog.DayRanking,
			new DialogResourceInfo("PresentRanking_Panel", "DialogDayRanking")
		},
		{
			eDialog.ExitConfirm,
			new DialogResourceInfo("Exit_Panel", "DialogCommon")
		},
		{
			eDialog.EventSetup,
			new DialogResourceInfo("Event_Panel_new", "DialogEventSetup")
		},
		{
			eDialog.Reward,
			new DialogResourceInfo("BoxGet_Panel", "DialogReward")
		},
		{
			eDialog.HeartShop,
			new DialogResourceInfo("HeartShop_Panel", "DialogHeartShop")
		},
		{
			eDialog.HighScoreList,
			new DialogResourceInfo("SendHighscore_Panel", "DialogHighScoreList")
		},
		{
			eDialog.Invite,
			new DialogResourceInfo("Invite_Panel", "DialogInvite")
		},
		{
			eDialog.InviteNone,
			new DialogResourceInfo("Invite_Nofriend_Panel", "DialogConfirm")
		},
		{
			eDialog.InviteConfirm,
			new DialogResourceInfo("Invite2_Panel", "DialogCommon")
		},
		{
			eDialog.InviteSent,
			new DialogResourceInfo("Invite3_Panel", "DialogCommon")
		},
		{
			eDialog.Information,
			new DialogResourceInfo("Information_Panel", "DialogInformation")
		},
		{
			eDialog.ItemHelp,
			new DialogResourceInfo("Item_Panel", "DialogItemHelp")
		},
		{
			eDialog.JewelShop,
			new DialogResourceInfo("JewelShop_Panel", "DialogJewelShop")
		},
		{
			eDialog.GiftJewelShop,
			new DialogResourceInfo("GiftJewelShop_Panel", "DialogGiftJewelShop")
		},
		{
			eDialog.LevelUp,
			new DialogResourceInfo("Levelup_Panel", "DialogLevelUp")
		},
		{
			eDialog.Mail,
			new DialogResourceInfo("Mail_Panel", "DialogMail")
		},
		{
			eDialog.NoMail,
			new DialogResourceInfo("NoMail_Panel", "DialogConfirm")
		},
		{
			eDialog.NetworkError,
			new DialogResourceInfo("NetworkError_Panel", "DialogCommon")
		},
		{
			eDialog.Option,
			new DialogResourceInfo("Option_Panel", "DialogOption")
		},
		{
			eDialog.Pause,
			new DialogResourceInfo("Gamestop_Panel", "DialogPause")
		},
		{
			eDialog.Present,
			new DialogResourceInfo("Present_Panel", "DialogCommon")
		},
		{
			eDialog.Ranking,
			new DialogResourceInfo("Ranking_Panel", "DialogRanking")
		},
		{
			eDialog.RequestNone,
			new DialogResourceInfo("Request3_Panel", "DialogConfirm")
		},
		{
			eDialog.RequestConfirm,
			new DialogResourceInfo("Request2_Panel", "DialogCommon")
		},
		{
			eDialog.Request,
			new DialogResourceInfo("Request_Panel", "DialogRequest")
		},
		{
			eDialog.ResultClear,
			new DialogResourceInfo("StageClear_Panel", "DialogResultClear")
		},
		{
			eDialog.ResultFailed,
			new DialogResourceInfo("StageFailed_Panel", "DialogResultFailed")
		},
		{
			eDialog.SendLevel,
			new DialogResourceInfo("ScoreNotice_Panel", "DialogSendLevel")
		},
		{
			eDialog.SendHighScore,
			new DialogResourceInfo("ScoreNotice_Panel", "DialogSendHighScore")
		},
		{
			eDialog.SendMaxScore,
			new DialogResourceInfo("ScoreNotice_Panel", "DialogSendMaxScore")
		},
		{
			eDialog.StageShop,
			new DialogResourceInfo("StageShop_Panel", "DialogStageShop")
		},
		{
			eDialog.Setup,
			new DialogResourceInfo("Setup_Panel", "DialogSetup")
		},
		{
			eDialog.RetryConfirm,
			new DialogResourceInfo("Retry_Panel", "DialogCommon")
		},
		{
			eDialog.PlayScore,
			new DialogResourceInfo("Highscore_Panel", "DialogPlayScore")
		},
		{
			eDialog.LimitOver,
			new DialogResourceInfo("LimitOver_Panel", "DialogLimitOver")
		},
		{
			eDialog.OptionOperate,
			new DialogResourceInfo("Operationtype_Panel", "DialogOptionOperation")
		},
		{
			eDialog.OptionLangage,
			new DialogResourceInfo("Languageoption_Panel", "DialogOptionLanguage")
		},
		{
			eDialog.ToBeContinue,
			new DialogResourceInfo("To_be_continued_Panel", "DialogConfirm")
		},
		{
			eDialog.HowToPlay,
			new DialogResourceInfo("HowToPlay_Panel", "DialogHowToPlay")
		},
		{
			eDialog.Maintenance,
			new DialogResourceInfo("Maintenance_Panel", "DialogCommon")
		},
		{
			eDialog.SendAreaClear,
			new DialogResourceInfo("ScoreNotice_Panel", "DialogSendAreaClear")
		},
		{
			eDialog.Nologin,
			new DialogResourceInfo("Nologin_Panel", "DialogNologin")
		},
		{
			eDialog.FriendHelp,
			new DialogResourceInfo("Rescue_Panel", "DialogFriendHelp")
		},
		{
			eDialog.PurchaseInfo,
			new DialogResourceInfo("BuyState_Panel", "DialogPurchaseInfo")
		},
		{
			eDialog.LuckyChance,
			new DialogResourceInfo("LuckyChance_Panel", "DialogLuckyChance")
		},
		{
			eDialog.HowToPlayIndex,
			new DialogResourceInfo("HowToPlay_index_Panel", "DialogHowToPlayIndex")
		},
		{
			eDialog.SendTreasure,
			new DialogResourceInfo("ScoreNotice_Panel", "DialogSendTreasure")
		},
		{
			eDialog.ClearBonus,
			new DialogResourceInfo("ClearBonus_Panel", "DialogClearBonus")
		},
		{
			eDialog.ClearBonusCollabo,
			new DialogResourceInfo("ClearBonus_Panel", "DialogClearBonusCollabo")
		},
		{
			eDialog.RecommendedItem,
			new DialogResourceInfo("RecommendItem_Panel", "DialogRecommended")
		},
		{
			eDialog.Logout,
			new DialogResourceInfo("Logout_Panel", "DialogCommon")
		},
		{
			eDialog.Unregister,
			new DialogResourceInfo("Unregister_Panel", "DialogUnregister")
		},
		{
			eDialog.Policy,
			new DialogResourceInfo("Policy_Panel", "DialogPolicy")
		},
		{
			eDialog.AreaLock,
			new DialogResourceInfo("AreaLock_Panel", "DialogAreaLock")
		},
		{
			eDialog.Credit,
			new DialogResourceInfo("Credit_Panel", "DialogCredit")
		},
		{
			eDialog.EventContinued,
			new DialogResourceInfo("Event_Continued_Panel", "DialogConfirm")
		},
		{
			eDialog.EventSendInfo,
			new DialogResourceInfo("SendInform_Panel", "DialogNortificate")
		},
		{
			eDialog.EventSendPopup,
			new DialogResourceInfo("EventInform_Panel", "DialogEventPopup")
		},
		{
			eDialog.Review,
			new DialogResourceInfo("Review_Panel", "DialogCommon")
		},
		{
			eDialog.StarReward,
			new DialogResourceInfo("Invite3_Panel", "DialogCommon")
		},
		{
			eDialog.ClearBonusInfo,
			new DialogResourceInfo("ClearBonusInfo_Panel", "DialogCommon")
		},
		{
			eDialog.PlayerInfo,
			new DialogResourceInfo("PlayerInfo_Panel", "DialogPlayerInfo")
		},
		{
			eDialog.ChallengeSetup,
			new DialogResourceInfo("Challenge_Panel", "DialogChallengeSetup")
		},
		{
			eDialog.ChallengeContinued,
			new DialogResourceInfo("Challenge_Continued_Panel", "DialogConfirm")
		},
		{
			eDialog.DailyMission,
			new DialogResourceInfo("DailyMission_Panel", "DialogDailyMission")
		},
		{
			eDialog.DailyMissionClear,
			new DialogResourceInfo("DailyMissionClear_Panel", "DialogDailyMissionClear")
		},
		{
			eDialog.DailyMissionCancel,
			new DialogResourceInfo("CommonDialog_Panel", "DialogCommon")
		},
		{
			eDialog.BonusRoulette,
			new DialogResourceInfo("BonusChance_Panel", "DialogBonusRoulette")
		},
		{
			eDialog.BonusRouletteCancel,
			new DialogResourceInfo("CommonDialog_Panel", "DialogCommon")
		},
		{
			eDialog.SendBonusCoin,
			new DialogResourceInfo("ScoreNotice_Panel", "DialogSendBonusCoin")
		},
		{
			eDialog.PauseBoss,
			new DialogResourceInfo("Gamestop_Panel", "DialogPauseBoss")
		},
		{
			eDialog.ContinueBoss,
			new DialogResourceInfo("BossStageContinue_Panel", "DialogContinueBoss")
		},
		{
			eDialog.ResultFailedBoss,
			new DialogResourceInfo("BossStageFailed_Panel", "DialogResultFailedBoss")
		},
		{
			eDialog.ResultClearBoss,
			new DialogResourceInfo("BossClear_Panel", "DialogResultClearBoss")
		},
		{
			eDialog.BossSetup,
			new DialogResourceInfo("Boss_Setup_Panel", "DialogBossSetup")
		},
		{
			eDialog.BossSelect,
			new DialogResourceInfo("Boss_Select_Panel", "DialogBossSelect")
		},
		{
			eDialog.BossReward,
			new DialogResourceInfo("BossReward_Panel", "DialogBossReward")
		},
		{
			eDialog.BossStageShop,
			new DialogResourceInfo("StageShop_Panel", "DialogBossStageShop")
		},
		{
			eDialog.CollaborationSetup,
			new DialogResourceInfo("Collaboration_Panel", "DialogCollaborationSetup")
		},
		{
			eDialog.OptionAvatar,
			new DialogResourceInfo("OptionAvatar_Panel", "DialogOptionAvatar")
		},
		{
			eDialog.SendBoss,
			new DialogResourceInfo("ScoreNotice_Panel", "DialogSendBoss")
		},
		{
			eDialog.Comeback,
			new DialogResourceInfo("Information_Panel", "DialogComeback")
		},
		{
			eDialog.BannerInformation,
			new DialogResourceInfo("BannerInformation_Panel", "DialogBannerInformation")
		},
		{
			eDialog.AvatarGacha,
			new DialogResourceInfo("AvatarGacha_Panel", "DialogAvatarGacha")
		},
		{
			eDialog.AvatarGachaResult,
			new DialogResourceInfo("AvatarGachaResult_Panel", "DialogAvatarGachaResult")
		},
		{
			eDialog.AvatarLevelup,
			new DialogResourceInfo("AvatarLevelup_Panel", "DialogAvatarLevelup")
		},
		{
			eDialog.AvatarLevelupAnim,
			new DialogResourceInfo("AvatarLevelup_anm_Panel", "DialogAvatarLevelupAnim")
		},
		{
			eDialog.AvatarProfile,
			new DialogResourceInfo("AvatarProfile_Panel", "DialogAvatarProfile")
		},
		{
			eDialog.AvatarCollection,
			new DialogResourceInfo("AvatarCollection_Panel", "DialogAvatarCollection")
		},
		{
			eDialog.CrossMission,
			new DialogResourceInfo("CrossMission_Panel", "DialogCrossMission")
		},
		{
			eDialog.AttendCheck,
			new DialogResourceInfo("AttendCheck_Panel", "DialogAttendCheck")
		},
		{
			eDialog.SendMap,
			new DialogResourceInfo("SendMap_Panel_new", "DialogSendMap")
		},
		{
			eDialog.StageSkip,
			new DialogResourceInfo("StageSkip_Panel", "DialogStageSkip")
		},
		{
			eDialog.ContinueRanking,
			new DialogResourceInfo("RankingStageContinue_Panel", "DialogRankingContinue")
		},
		{
			eDialog.ClearRanking,
			new DialogResourceInfo("RankingStageClear_Panel", "DialogRankingResultClear")
		},
		{
			eDialog.AllShop,
			new DialogResourceInfo("AllShop_Panel", "DialogAllShop")
		},
		{
			eDialog.GachaContents,
			new DialogResourceInfo("GachaContents_Panel", "DialogGachaContents")
		},
		{
			eDialog.AirArmorRouting,
			new DialogResourceInfo("AirArmor_RoutingPanel", "DialogAirArmorRouting")
		},
		{
			eDialog.AirArmorCheat,
			new DialogResourceInfo("AirArmor_CheatPanel", "DialogAirArmorCheat")
		},
		{
			eDialog.ParkAreaList,
			new DialogResourceInfo("ParkAreaList_Panel", "DialogParkAreaList")
		},
		{
			eDialog.ParkStageList,
			new DialogResourceInfo("ParkStageList_Panel", "DialogParkStageList")
		},
		{
			eDialog.ParkStageSetup,
			new DialogResourceInfo("ParkStageSetup_Panel", "DialogSetupPark")
		},
		{
			eDialog.ParkStageClear,
			new DialogResourceInfo("ParkStageClear_Panel", "DialogResultClear")
		},
		{
			eDialog.ParkStageMinilenGet,
			new DialogResourceInfo("ParkMinilenGet_Panel", "DialogParkMinilenGet")
		},
		{
			eDialog.ParkMinilenThanks,
			new DialogResourceInfo("ParkThanksList_Panel", "DialogParkThanksList")
		},
		{
			eDialog.ParkMinilenThanksDetail,
			new DialogResourceInfo("ParkMinilenThanks_Detail_Panel", "DialogParkThankDetail")
		},
		{
			eDialog.ParkMinilenThanksGet,
			new DialogResourceInfo("ParkMinilenThanks_Get_Panel", "DialogParkThankGet")
		},
		{
			eDialog.ParkBuildingList,
			new DialogResourceInfo("ParkBuildingList_Panel", "DialogParkBuildingList")
		},
		{
			eDialog.ParkRoadList,
			new DialogResourceInfo("ParkRoadList_Panel", "DialogParkRoadList")
		},
		{
			eDialog.ParkMinilenCollection,
			new DialogResourceInfo("ParkMinilenCollection_Panel", "DialogParkMinilenCollection")
		},
		{
			eDialog.ParkMinilenProfile,
			new DialogResourceInfo("ParkMinilenProfile_Panel", "DialogParkMinilenProfile")
		},
		{
			eDialog.ParkObjectChoices,
			new DialogResourceInfo("Select_parkpart", "DialogParkObjectChoices")
		},
		{
			eDialog.ParkRemoveConfirm,
			new DialogResourceInfo("RemoveConfirmation_Panel", "DialogParkRemoveConfirmation")
		},
		{
			eDialog.ParkNiceHistoryList,
			new DialogResourceInfo("NiceHistory_Panel", "DialogParkNiceHistoryList")
		},
		{
			eDialog.ParkNiceDetail,
			new DialogResourceInfo("NiceDetail_Panel", "DialogParkNiceDetail")
		},
		{
			eDialog.ParkSendNiceConfirm,
			new DialogResourceInfo("SendNice_Panel", "DialogCommon")
		},
		{
			eDialog.ParkFriendList,
			new DialogResourceInfo("ParkFriend_Panel", "DialogParkFriendList")
		},
		{
			eDialog.ParkRewardCheckList,
			new DialogResourceInfo("ParkRewardCheck_Panel", "DialogParkRewardCheckList")
		},
		{
			eDialog.ParkReleaseNotice,
			new DialogResourceInfo("ParkRelease_Panel", "DialogParkRelease")
		},
		{
			eDialog.RankingStage,
			new DialogResourceInfo("PresentRankingStage_Panel", "DialogRankingStage")
		},
		{
			eDialog.RankingSetup,
			new DialogResourceInfo("RankingSetup_Panel", "DialogRankingSetup")
		},
		{
			eDialog.RankingContinue,
			new DialogResourceInfo("RankingStageContinue_Panel", "DialogRankingContinue")
		},
		{
			eDialog.RankingResultClear,
			new DialogResourceInfo("RankingStageClear_Panel", "DialogRankingResultClear")
		},
		{
			eDialog.ModeSelect,
			new DialogResourceInfo("ModeSelect_Panel", "DialogModeSelect")
		}
	};

	private static Dictionary<PartManager.ePart, eDialog[]> DialogTable = new Dictionary<PartManager.ePart, eDialog[]>
	{
		{
			PartManager.ePart.Title,
			new eDialog[2]
			{
				eDialog.Policy,
				eDialog.Common
			}
		},
		{
			PartManager.ePart.Stage,
			new eDialog[23]
			{
				eDialog.Pause,
				eDialog.LevelUp,
				eDialog.SendLevel,
				eDialog.SendHighScore,
				eDialog.SendMaxScore,
				eDialog.ChangeRanking,
				eDialog.ResultFailed,
				eDialog.ResultClear,
				eDialog.Continue,
				eDialog.HighScoreList,
				eDialog.StageShop,
				eDialog.ExitConfirm,
				eDialog.RetryConfirm,
				eDialog.SendAreaClear,
				eDialog.LuckyChance,
				eDialog.ClearBonus,
				eDialog.ClearBonusCollabo,
				eDialog.EventSendInfo,
				eDialog.EventSendPopup,
				eDialog.ClearBonusInfo,
				eDialog.ParkStageClear,
				eDialog.ParkStageMinilenGet,
				eDialog.ADContinue
			}
		},
		{
			PartManager.ePart.Map,
			new eDialog[44]
			{
				eDialog.Setup,
				eDialog.EventSetup,
				eDialog.HeartShop,
				eDialog.Ranking,
				eDialog.Mail,
				eDialog.NoMail,
				eDialog.Award,
				eDialog.DayRanking,
				eDialog.Invite,
				eDialog.InviteConfirm,
				eDialog.InviteSent,
				eDialog.InviteNone,
				eDialog.Request,
				eDialog.RequestConfirm,
				eDialog.ToBeContinue,
				eDialog.FriendHelp,
				eDialog.SendTreasure,
				eDialog.RecommendedItem,
				eDialog.AreaLock,
				eDialog.Review,
				eDialog.StarReward,
				eDialog.PlayerInfo,
				eDialog.DailyMission,
				eDialog.DailyMissionCancel,
				eDialog.DailyMissionClear,
				eDialog.BonusRoulette,
				eDialog.BonusRouletteCancel,
				eDialog.BossSetup,
				eDialog.BossSelect,
				eDialog.BossReward,
				eDialog.BannerInformation,
				eDialog.AvatarGacha,
				eDialog.AvatarGachaResult,
				eDialog.AvatarLevelup,
				eDialog.AvatarLevelupAnim,
				eDialog.AvatarProfile,
				eDialog.AvatarCollection,
				eDialog.SendMap,
				eDialog.RankingSetup,
				eDialog.StageSkip,
				eDialog.GachaContents,
				eDialog.ParkReleaseNotice,
				eDialog.CrossMission,
				eDialog.AttendCheck
			}
		},
		{
			PartManager.ePart.EventMap,
			new eDialog[32]
			{
				eDialog.EventSetup,
				eDialog.HeartShop,
				eDialog.Ranking,
				eDialog.Mail,
				eDialog.NoMail,
				eDialog.Award,
				eDialog.DayRanking,
				eDialog.Invite,
				eDialog.InviteConfirm,
				eDialog.InviteNone,
				eDialog.Request,
				eDialog.RequestConfirm,
				eDialog.EventContinued,
				eDialog.FriendHelp,
				eDialog.SendTreasure,
				eDialog.RecommendedItem,
				eDialog.AreaLock,
				eDialog.PlayerInfo,
				eDialog.DailyMission,
				eDialog.DailyMissionCancel,
				eDialog.DailyMissionClear,
				eDialog.BannerInformation,
				eDialog.AvatarGacha,
				eDialog.AvatarGachaResult,
				eDialog.AvatarLevelup,
				eDialog.AvatarLevelupAnim,
				eDialog.AvatarProfile,
				eDialog.AvatarCollection,
				eDialog.SendMap,
				eDialog.StageSkip,
				eDialog.GachaContents,
				eDialog.CrossMission
			}
		},
		{
			PartManager.ePart.ChallengeMap,
			new eDialog[31]
			{
				eDialog.HeartShop,
				eDialog.Ranking,
				eDialog.Mail,
				eDialog.NoMail,
				eDialog.Award,
				eDialog.DayRanking,
				eDialog.Invite,
				eDialog.InviteConfirm,
				eDialog.InviteNone,
				eDialog.Request,
				eDialog.RequestConfirm,
				eDialog.FriendHelp,
				eDialog.SendTreasure,
				eDialog.RecommendedItem,
				eDialog.AreaLock,
				eDialog.PlayerInfo,
				eDialog.ChallengeSetup,
				eDialog.ChallengeContinued,
				eDialog.DailyMission,
				eDialog.DailyMissionCancel,
				eDialog.DailyMissionClear,
				eDialog.BannerInformation,
				eDialog.AvatarGacha,
				eDialog.AvatarGachaResult,
				eDialog.AvatarLevelup,
				eDialog.AvatarLevelupAnim,
				eDialog.AvatarProfile,
				eDialog.AvatarCollection,
				eDialog.SendMap,
				eDialog.StageSkip,
				eDialog.CrossMission
			}
		},
		{
			PartManager.ePart.CollaborationMap,
			new eDialog[32]
			{
				eDialog.CollaborationSetup,
				eDialog.HeartShop,
				eDialog.Ranking,
				eDialog.Mail,
				eDialog.NoMail,
				eDialog.Award,
				eDialog.DayRanking,
				eDialog.Invite,
				eDialog.InviteConfirm,
				eDialog.InviteNone,
				eDialog.Request,
				eDialog.RequestConfirm,
				eDialog.EventContinued,
				eDialog.FriendHelp,
				eDialog.SendTreasure,
				eDialog.RecommendedItem,
				eDialog.AreaLock,
				eDialog.PlayerInfo,
				eDialog.DailyMission,
				eDialog.DailyMissionCancel,
				eDialog.DailyMissionClear,
				eDialog.BannerInformation,
				eDialog.AvatarGacha,
				eDialog.AvatarGachaResult,
				eDialog.AvatarLevelup,
				eDialog.AvatarLevelupAnim,
				eDialog.AvatarProfile,
				eDialog.AvatarCollection,
				eDialog.SendMap,
				eDialog.StageSkip,
				eDialog.GachaContents,
				eDialog.CrossMission
			}
		},
		{
			PartManager.ePart.BonusStage,
			new eDialog[6]
			{
				eDialog.Pause,
				eDialog.ExitConfirm,
				eDialog.RetryConfirm,
				eDialog.BonusRoulette,
				eDialog.BonusRouletteCancel,
				eDialog.SendBonusCoin
			}
		},
		{
			PartManager.ePart.BossStage,
			new eDialog[20]
			{
				eDialog.PauseBoss,
				eDialog.LevelUp,
				eDialog.SendLevel,
				eDialog.SendHighScore,
				eDialog.SendMaxScore,
				eDialog.SendBoss,
				eDialog.ChangeRanking,
				eDialog.ResultFailedBoss,
				eDialog.ResultClearBoss,
				eDialog.ContinueBoss,
				eDialog.HighScoreList,
				eDialog.ExitConfirm,
				eDialog.RetryConfirm,
				eDialog.SendAreaClear,
				eDialog.LuckyChance,
				eDialog.ClearBonus,
				eDialog.EventSendInfo,
				eDialog.EventSendPopup,
				eDialog.ClearBonusInfo,
				eDialog.BossStageShop
			}
		},
		{
			PartManager.ePart.RankingStage,
			new eDialog[14]
			{
				eDialog.Pause,
				eDialog.HeartShop,
				eDialog.LevelUp,
				eDialog.SendLevel,
				eDialog.SendHighScore,
				eDialog.SendMaxScore,
				eDialog.ChangeRanking,
				eDialog.RankingResultClear,
				eDialog.RankingContinue,
				eDialog.HighScoreList,
				eDialog.LuckyChance,
				eDialog.StageShop,
				eDialog.ExitConfirm,
				eDialog.RetryConfirm
			}
		},
		{
			PartManager.ePart.RankingMap,
			new eDialog[16]
			{
				eDialog.HeartShop,
				eDialog.Mail,
				eDialog.NoMail,
				eDialog.Award,
				eDialog.Invite,
				eDialog.InviteConfirm,
				eDialog.InviteNone,
				eDialog.Request,
				eDialog.RequestConfirm,
				eDialog.FriendHelp,
				eDialog.SendTreasure,
				eDialog.RecommendedItem,
				eDialog.Review,
				eDialog.PlayerInfo,
				eDialog.RankingStage,
				eDialog.RankingSetup
			}
		},
		{
			PartManager.ePart.Park,
			new eDialog[41]
			{
				eDialog.Setup,
				eDialog.HeartShop,
				eDialog.Mail,
				eDialog.NoMail,
				eDialog.Award,
				eDialog.Invite,
				eDialog.InviteConfirm,
				eDialog.InviteNone,
				eDialog.Request,
				eDialog.RequestConfirm,
				eDialog.ToBeContinue,
				eDialog.FriendHelp,
				eDialog.RecommendedItem,
				eDialog.StarReward,
				eDialog.PlayerInfo,
				eDialog.DailyMission,
				eDialog.DailyMissionCancel,
				eDialog.DailyMissionClear,
				eDialog.BannerInformation,
				eDialog.AvatarGacha,
				eDialog.AvatarGachaResult,
				eDialog.AvatarLevelup,
				eDialog.AvatarLevelupAnim,
				eDialog.ParkMinilenCollection,
				eDialog.ParkMinilenProfile,
				eDialog.GachaContents,
				eDialog.ParkAreaList,
				eDialog.ParkStageList,
				eDialog.ParkStageSetup,
				eDialog.ParkMinilenThanks,
				eDialog.ParkMinilenThanksDetail,
				eDialog.ParkMinilenThanksGet,
				eDialog.ParkBuildingList,
				eDialog.ParkRoadList,
				eDialog.ParkObjectChoices,
				eDialog.ParkRemoveConfirm,
				eDialog.ParkNiceHistoryList,
				eDialog.ParkNiceDetail,
				eDialog.ParkSendNiceConfirm,
				eDialog.ParkFriendList,
				eDialog.ParkRewardCheckList
			}
		}
	};

	private static eDialog[] FirstGlobalDialogs = new eDialog[9]
	{
		eDialog.Common,
		eDialog.AppQuit,
		eDialog.NetworkError,
		eDialog.Maintenance,
		eDialog.Information,
		eDialog.Policy,
		eDialog.Comeback,
		eDialog.AirArmorRouting,
		eDialog.AirArmorCheat
	};

	public static int FirstGlobalDialogMax = 9;

	private static eDialog[] GlobalDialogs = new eDialog[24]
	{
		eDialog.Common,
		eDialog.BuyLimit,
		eDialog.ItemHelp,
		eDialog.JewelShop,
		eDialog.GiftJewelShop,
		eDialog.CoinShop,
		eDialog.LimitOver,
		eDialog.Option,
		eDialog.OptionOperate,
		eDialog.RequestNone,
		eDialog.OptionLangage,
		eDialog.Present,
		eDialog.PlayScore,
		eDialog.HowToPlay,
		eDialog.Reward,
		eDialog.Nologin,
		eDialog.PurchaseInfo,
		eDialog.HowToPlayIndex,
		eDialog.Logout,
		eDialog.Unregister,
		eDialog.Credit,
		eDialog.InviteSent,
		eDialog.OptionAvatar,
		eDialog.AllShop
	};

	public static int GlobalDialogMax = 24;

	private GameObject currentUiRoot_;

	private bool bLoading_;

	private Dictionary<eDialog, DialogBase> currentDialogDict_ = new Dictionary<eDialog, DialogBase>();

	private Dictionary<eDialog, DialogBase> globalDialogDict_ = new Dictionary<eDialog, DialogBase>();

	private List<eDialog> activeDialogList_ = new List<eDialog>();

	private List<eDialog> closeDialogList_ = new List<eDialog>();

	public int getActiveDialogNum()
	{
		return activeDialogList_.Count;
	}

	public eDialog getActiveDialog(int index)
	{
		if (index < 0 || index >= getActiveDialogNum())
		{
		}
		return activeDialogList_[index];
	}

	public IEnumerator openDialog(eDialog dialog)
	{
		yield return StartCoroutine(openDialog(getDialog(dialog)));
	}

	public IEnumerator closeDialog(eDialog dialog)
	{
		yield return StartCoroutine(closeDialog(getDialog(dialog)));
	}

	public IEnumerator openDialog(DialogBase dialog)
	{
		yield return StartCoroutine(dialog.open());
		addActiveDialogList(dialog.getDialogType());
	}

	public IEnumerator closeDialog(DialogBase dialog)
	{
		yield return StartCoroutine(dialog.close());
		removeActiveDialogList(dialog.getDialogType());
	}

	public IEnumerator closeBossSetDialog(DialogBase closeDialog)
	{
		if (closeDialog != null)
		{
			yield return StartCoroutine(closeDialog.close());
		}
		yield return null;
		DialogBossSelect setup = getDialog(eDialog.BossSelect) as DialogBossSelect;
		yield return StartCoroutine(setup.show(this));
		if (closeDialog != null)
		{
			removeActiveDialogList(closeDialog.getDialogType());
		}
	}

	public IEnumerator openBossSetDialog(DialogBase closeDialog, int type, int level)
	{
		if (closeDialog != null)
		{
			yield return StartCoroutine(closeDialog.close());
		}
		DialogBossSetup setup = getDialog(eDialog.BossSetup) as DialogBossSetup;
		yield return StartCoroutine(setup.show(type, level, this));
		if (closeDialog != null)
		{
			removeActiveDialogList(closeDialog.getDialogType());
		}
	}

	public IEnumerator OpenAvatarLevelup(DialogBase closeDialog, int AvatarID)
	{
		if (closeDialog != null)
		{
			yield return StartCoroutine(closeDialog.close());
		}
		DialogAvatarLevelup alDialog = getDialog(eDialog.AvatarLevelup) as DialogAvatarLevelup;
		alDialog.setup(AvatarID);
		yield return StartCoroutine(openDialog(alDialog));
		if (closeDialog != null)
		{
			removeActiveDialogList(closeDialog.getDialogType());
		}
	}

	public IEnumerator OpenAvatarGacha(DialogBase closeDialog, bool isFree = false)
	{
		Input.enable = false;
		Hashtable h = Hash.GachaTop();
		NetworkMng.Instance.setup(h);
		yield return StartCoroutine(NetworkMng.Instance.download(API.GachaTop, true, false));
		while (NetworkMng.Instance.isDownloading())
		{
			yield return null;
		}
		if (NetworkMng.Instance.getResultCode() != 0)
		{
			Input.enable = true;
			yield break;
		}
		WWW wwww = NetworkMng.Instance.getWWW();
		GachaTop resultData = JsonMapper.ToObject<GachaTop>(wwww.text);
		GlobalData.Instance.getGameData().setGachaTopData(resultData);
		if (closeDialog != null && closeDialog.getDialogType() == eDialog.AvatarCollection)
		{
			(closeDialog as DialogAvatarCollection).DestroyContents();
		}
		if (closeDialog != null)
		{
			yield return StartCoroutine(closeDialog.close());
		}
		DialogAvatarGacha gachaDialog = getDialog(eDialog.AvatarGacha) as DialogAvatarGacha;
		if (closeDialog.getDialogType() != eDialog.AvatarLevelupAnim)
		{
			gachaDialog.resetGachaNo();
		}
		gachaDialog.setup(isFree);
		yield return StartCoroutine(openDialog(gachaDialog));
		if (closeDialog != null)
		{
			removeActiveDialogList(closeDialog.getDialogType());
		}
		Input.enable = true;
	}

	public IEnumerator OpenAvatarLevelupAnim(DialogBase closeDialog, int avatarID, bool bReleaseLimit, bool isGacha)
	{
		if (closeDialog != null)
		{
			yield return StartCoroutine(closeDialog.close());
		}
		DialogAvatarLevelupAnim lvAnim = getDialog(eDialog.AvatarLevelupAnim) as DialogAvatarLevelupAnim;
		lvAnim.setup(avatarID, bReleaseLimit, isGacha);
		yield return StartCoroutine(openDialog(lvAnim));
		Sound.Instance.playSe(Sound.eSe.SE_569_gacha_avatarLevelup_fanfare);
		if (closeDialog != null)
		{
			removeActiveDialogList(closeDialog.getDialogType());
		}
	}

	public IEnumerator OpenAvatarProfile(DialogBase closeDialog, int avatarID)
	{
		if (closeDialog != null)
		{
			yield return StartCoroutine(closeDialog.close());
		}
		DialogAvatarProfile profDialog = getDialog(eDialog.AvatarProfile) as DialogAvatarProfile;
		profDialog.setup(avatarID);
		yield return StartCoroutine(openDialog(profDialog));
		if (closeDialog != null)
		{
			removeActiveDialogList(closeDialog.getDialogType());
		}
	}

	public IEnumerator OpenAvatarCollection(DialogBase closeDialog)
	{
		if (closeDialog != null)
		{
			yield return StartCoroutine(closeDialog.close());
		}
		DialogAvatarCollection collection = getDialog(eDialog.AvatarCollection) as DialogAvatarCollection;
		collection.setup();
		yield return StartCoroutine(openDialog(collection));
		if (closeDialog != null)
		{
			removeActiveDialogList(closeDialog.getDialogType());
		}
	}

	public IEnumerator OpenMinilenCollection(DialogBase closeDialog)
	{
		if (closeDialog != null)
		{
			yield return StartCoroutine(closeDialog.close());
		}
		DialogParkMinilenCollection collection = getDialog(eDialog.ParkMinilenCollection) as DialogParkMinilenCollection;
		collection.setup();
		yield return StartCoroutine(openDialog(collection));
		if (closeDialog != null)
		{
			removeActiveDialogList(closeDialog.getDialogType());
		}
	}

	public IEnumerator OpenInformation(DialogBase closeDialog)
	{
		if (closeDialog != null)
		{
			yield return StartCoroutine(this.closeDialog(closeDialog));
		}
		DialogInformation infoDialog = getDialog(eDialog.Information) as DialogInformation;
		if (!(infoDialog == null))
		{
			InformationData[] informationList = GlobalData.Instance.getGameData().informationList;
			InformationData info = informationList[2];
			int id = 5;
			int shopId = 0;
			string url = "http://125.6.163.34/puzzlebobble/comeback/ja/14_1.png";
			if (!(url == string.Empty))
			{
				infoDialog.preLoad(url, id, shopId);
				yield return StartCoroutine(infoDialog.show(url, id, shopId));
			}
		}
	}

	public IEnumerator OpenSetup(DialogBase closeDialog, eDialog dialog, StageIcon icon, PartBase part)
	{
		if (closeDialog != null)
		{
			yield return StartCoroutine(closeDialog.close());
		}
		switch (dialog)
		{
		case eDialog.Setup:
		{
			DialogSetup setup = getDialog(eDialog.Setup) as DialogSetup;
			if (setup != null && !setup.isOpen())
			{
				yield return StartCoroutine(setup.show(icon, part as Part_Map));
				playTutorial(icon.getStageNo());
			}
			break;
		}
		case eDialog.EventSetup:
		{
			DialogEventSetup eSetup = getDialog(eDialog.EventSetup) as DialogEventSetup;
			if (eSetup != null && !eSetup.isOpen())
			{
				yield return StartCoroutine(eSetup.show(icon, part as Part_EventMap));
			}
			break;
		}
		case eDialog.CollaborationSetup:
		{
			DialogCollaborationSetup cSetup = getDialog(eDialog.CollaborationSetup) as DialogCollaborationSetup;
			if (cSetup != null && !cSetup.isOpen())
			{
				yield return StartCoroutine(cSetup.show(icon, part as Part_CollaborationMap));
			}
			break;
		}
		}
		if (closeDialog != null)
		{
			removeActiveDialogList(closeDialog.getDialogType());
		}
	}

	public void playTutorial(int stageNo)
	{
		TutorialManager.Instance.bItemTutorial = false;
		DialogSetup dialogSetup = getDialog(eDialog.Setup) as DialogSetup;
		if (dialogSetup.isOpen())
		{
			GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
			StageInfo.CommonInfo common = @object.GetComponent<StageDataTable>().getInfo(stageNo).Common;
			if ((common.FreeItems.Length > 0 || stageNo == 4) && TutorialManager.Instance.isTutorial(stageNo, TutorialDataTable.ePlace.Setup))
			{
				GameObject currentUiRoot = getCurrentUiRoot();
				TutorialManager.Instance.load(stageNo, currentUiRoot);
				TutorialManager.Instance.reOpenSetup(stageNo, TutorialDataTable.ePlace.Setup, currentUiRoot, common);
			}
		}
	}

	public IEnumerator waitDialogAnimation(DialogBase dialog)
	{
		while (dialog.isPlayingAnimation())
		{
			yield return 0;
		}
	}

	public void removeActiveDialogList(eDialog dialogType)
	{
		if (activeDialogList_.Contains(dialogType))
		{
			activeDialogList_.Remove(dialogType);
		}
	}

	public void addActiveDialogList(eDialog dialogType)
	{
		if (!activeDialogList_.Contains(dialogType))
		{
			activeDialogList_.Add(dialogType);
		}
	}

	public void clearActiveDialogList()
	{
		activeDialogList_.Clear();
	}

	public List<eDialog> getReserveDialog()
	{
		return closeDialogList_;
	}

	public void reserveCloseDialog(eDialog dialogType)
	{
		closeDialogList_.Add(dialogType);
	}

	public void clearReserveList()
	{
		closeDialogList_.Clear();
	}

	public void setCurrenUiRoot(GameObject uiRoot)
	{
		currentUiRoot_ = uiRoot;
	}

	public GameObject getCurrentUiRoot()
	{
		return currentUiRoot_;
	}

	public void releaseCurrentDialog()
	{
		currentDialogDict_.Clear();
		currentUiRoot_ = null;
	}

	public void releaseGlobalDialog()
	{
		globalDialogDict_.Clear();
	}

	public bool isLoading()
	{
		return bLoading_;
	}

	public IEnumerator load(PartManager.ePart part)
	{
		yield return StartCoroutine(load(part, null, null));
	}

	public IEnumerator load(PartManager.ePart part, OnStartLoad startCB, OnFinishLoad finishCB)
	{
		if (!DialogTable.ContainsKey(part))
		{
			yield break;
		}
		bLoading_ = true;
		for (int i = 0; i < DialogMax; i++)
		{
			if (startCB != null)
			{
				startCB((eDialog)i);
			}
			if (load(currentUiRoot_, DialogTable[part], (eDialog)i, ref currentDialogDict_) && finishCB != null)
			{
				finishCB((eDialog)i);
			}
		}
		bLoading_ = false;
	}

	public IEnumerator loadGlobalDialogs(GameObject uiRoot)
	{
		yield return StartCoroutine(loadGlobalDialogs(false, uiRoot, null, null));
	}

	public IEnumerator loadGlobalDialogs(bool bFirst, GameObject uiRoot, OnStartLoad startCB, OnFinishLoad finishCB)
	{
		bLoading_ = true;
		eDialog[] dialogs = ((!bFirst) ? GlobalDialogs : FirstGlobalDialogs);
		for (int i = 0; i < DialogMax; i++)
		{
			if (startCB != null)
			{
				startCB((eDialog)i);
			}
			if (load(uiRoot, dialogs, (eDialog)i, ref globalDialogDict_) && finishCB != null)
			{
				finishCB((eDialog)i);
			}
		}
		bLoading_ = false;
		yield break;
	}

	public bool isLoaded(eDialog dialog)
	{
		if (globalDialogDict_.ContainsKey(dialog))
		{
			return true;
		}
		if (currentDialogDict_.ContainsKey(dialog))
		{
			return true;
		}
		return false;
	}

	public DialogBase getDialog(eDialog dialog)
	{
		if (globalDialogDict_.ContainsKey(dialog))
		{
			return globalDialogDict_[dialog];
		}
		if (currentDialogDict_.ContainsKey(dialog))
		{
			return currentDialogDict_[dialog];
		}
		return null;
	}

	private bool load(GameObject parent, eDialog[] dialogMask, eDialog dialogType, ref Dictionary<eDialog, DialogBase> dict)
	{
		if (dialogMask == null)
		{
			return false;
		}
		if (Array.IndexOf(dialogMask, dialogType) >= 0)
		{
			dict[dialogType] = load(parent, dialogType);
			return true;
		}
		return false;
	}

	public DialogBase load(GameObject parent, eDialog dialogType)
	{
		DialogResourceInfo dialogResourceInfo = DialogNameTable[dialogType];
		GameObject gameObject = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", dialogResourceInfo.ResourceName)) as GameObject;
		if (dialogResourceInfo.ComponentName.Length > 0)
		{
			UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(gameObject, "Assets/Scripts/Assembly-CSharp/DialogManager.cs (1540,4)", dialogResourceInfo.ComponentName);
		}
		else
		{
			gameObject.AddComponent<DialogBase>();
		}
		Utility.setParent(gameObject, parent.transform, dialogResourceInfo.IsClearTransform);
		DialogBase component = gameObject.GetComponent<DialogBase>();
		component.init(this, partManager, fade, dialogType);
		component.OnCreate();
		NGUIUtility.setupButton(gameObject, gameObject, true);
		gameObject.SetActive(false);
		return component;
	}

	private void OnDestroy()
	{
		releaseGlobalDialog();
		releaseCurrentDialog();
	}

	private void Update()
	{
		if (!Input.GetKeyUp(KeyCode.Escape) || !Input.enable || (partManager.currentPart == PartManager.ePart.Title && !((Part_Title)partManager.execPart).isEndLogin))
		{
			return;
		}
		DialogAppQuit dialogAppQuit = getDialog(eDialog.AppQuit) as DialogAppQuit;
		if (dialogAppQuit == null || dialogAppQuit.isOpen() || dialogAppQuit.isPlayingAnimation())
		{
			return;
		}
		if (partManager.currentPart == PartManager.ePart.Stage)
		{
			Part_Stage part_Stage = (Part_Stage)partManager.execPart;
			dialogAppQuit.previewPause = part_Stage.stagePause.pause;
			if (!part_Stage.stagePause.pause)
			{
				part_Stage.stagePause.pause = true;
			}
		}
		if (partManager.currentPart == PartManager.ePart.BonusStage)
		{
			Part_BonusStage part_BonusStage = (Part_BonusStage)partManager.execPart;
			dialogAppQuit.previewPause = part_BonusStage.stagePause.pause;
			if (!part_BonusStage.stagePause.pause)
			{
				part_BonusStage.stagePause.pause = true;
			}
		}
		if (partManager.currentPart == PartManager.ePart.BossStage)
		{
			Part_BossStage part_BossStage = (Part_BossStage)partManager.execPart;
			dialogAppQuit.previewPause = part_BossStage.stagePause.pause;
			if (!part_BossStage.stagePause.pause)
			{
				part_BossStage.stagePause.pause = true;
			}
		}
		if (partManager.currentPart == PartManager.ePart.Stage || partManager.currentPart == PartManager.ePart.BonusStage || partManager.currentPart == PartManager.ePart.BossStage || partManager.currentPart == PartManager.ePart.RankingStage)
		{
			StartCoroutine(openDialog(dialogAppQuit));
			return;
		}
		if (GameObject.Find("PB_TNKExitHandler").GetComponent<PBTnkExitHandler>().GetReadyAD())
		{
			UnityEngine.Debug.Log(" PBTnkExitHandler.GetReadyAD() == true");
			Plugin.Instance.showInterstitialAd("PB_EXIT_AD");
		}
		else
		{
			UnityEngine.Debug.Log(" PBTnkExitHandler.GetReadyAD() == false");
			StartCoroutine(openDialog(dialogAppQuit));
		}
		Plugin.Instance.prepareInterstitialAd("PB_EXIT_AD", "PB_TNKExitHandler");
	}

	public void InviteDialogChange(bool iADFlg)
	{
		if (!iADFlg)
		{
			DialogNameTable[eDialog.Invite] = new DialogResourceInfo("Invite_Panel_iOS", "DialogInvite");
			DialogNameTable[eDialog.InviteNone] = new DialogResourceInfo("Invite_Nofriend_Panel_iOS", "DialogConfirm");
			GlobalData.Instance.getGameData().iosInvite = false;
		}
	}
}
