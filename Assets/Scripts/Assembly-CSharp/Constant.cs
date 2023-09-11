using System;
using System.Collections;
using Bridge;
using LitJson;
using UnityEngine;

public class Constant
{
	public enum eMoney
	{
		Invalid = -1,
		Ticket = 0,
		Coin = 1,
		Jewel = 2,
		Heart = 3,
		GachaTicket = 4,
		Max = 5
	}

	public enum eShop
	{
		Heart = 0,
		Coin = 1,
		Jewel = 2,
		SetShop = 3
	}

	public enum eEventNo
	{
		None = 0,
		NormalEvent = 1,
		Challenge = 2,
		Boss = 3,
		Collaboration = 11,
		Max = 12
	}

	public class PlayerData
	{
		public static void addCoin(int value)
		{
			int coin = Bridge.PlayerData.getCoin();
			Bridge.PlayerData.setCoin(Mathf.Min(coin + value, CoinMax));
		}

		public static void subJewel(int value)
		{
			int buyJewel = Bridge.PlayerData.getBuyJewel();
			int bonusJewel = Bridge.PlayerData.getBonusJewel();
			Bridge.PlayerData.setBonusJewel(Mathf.Max(bonusJewel - value, 0));
			value = Mathf.Max(value - bonusJewel, 0);
			Bridge.PlayerData.setBuyJewel(Mathf.Max(buyJewel - value, 0));
		}
	}

	public class Skill
	{
		public Avatar.eSpecialSkill SkillType;

		public int Level;

		public float LevelEffect;

		public void set(Avatar.eSpecialSkill skillType, int level, float levelEffect)
		{
			SkillType = skillType;
			Level = level;
			LevelEffect = levelEffect;
		}
	}

	public enum eMorganaCharaType
	{
		Gachapin = 0,
		Mukku = 1,
		Max = 2
	}

	public class Reward
	{
		public eMoney RewardType;

		public int Num;

		public Reward()
		{
			RewardType = eMoney.Invalid;
			Num = 0;
		}

		public void set(eMoney rewardType, int num)
		{
			RewardType = rewardType;
			Num = num;
		}

		public static bool isHeartRecovery(Reward reward)
		{
			if (reward.Num == 0 && reward.RewardType == eMoney.Heart)
			{
				return true;
			}
			return false;
		}

		public static bool addReward(Reward reward)
		{
			bool result = false;
			int num = reward.Num;
			if (isHeartRecovery(reward))
			{
				int heart = Bridge.PlayerData.getHeart();
				num = AutoRecoveryHeartMax;
				num = Mathf.Max(num - heart, 0);
			}
			switch (reward.RewardType)
			{
			case eMoney.Coin:
			{
				int num3 = Bridge.PlayerData.getCoin() + num;
				result = num3 > CoinMax;
				break;
			}
			case eMoney.Heart:
			{
				int heart2 = Bridge.PlayerData.getHeart();
				result = heart2 + num > HeartMax;
				break;
			}
			case eMoney.Jewel:
			{
				int num2 = Bridge.PlayerData.getJewel() + num;
				result = num2 > JewelMax;
				break;
			}
			}
			return result;
		}
	}

	public class Event
	{
		public static int BaseEventStageNo = 10000;

		public static bool isEventStage(int stageNo)
		{
			if (stageNo >= BaseEventStageNo)
			{
				return true;
			}
			return false;
		}

		public static int convNoToLevel(int stageNo, int eventNo)
		{
			if (!isEventStage(stageNo))
			{
				return stageNo - eventNo * BaseEventStageNo;
			}
			return stageNo - eventNo * BaseEventStageNo;
		}

		public static int convLevelToNo(EventStageInfo.eLevel level, int eventNo)
		{
			return (int)(level + BaseEventStageNo * eventNo);
		}

		public static int getLeastLevelStageNo(int eventNo)
		{
			return convLevelToNo(EventStageInfo.eLevel.Easy, eventNo) + 1;
		}

		public static int getHighestLevelStageNo(int eventNo)
		{
			return convLevelToNo(EventStageInfo.eLevel.Hard, eventNo);
		}

		public static bool isRewardFreeItem(int itemNo)
		{
			if (itemNo > 10000 && itemNo <= 20000)
			{
				return true;
			}
			return false;
		}

		public static int convItemTypeNoToRewardfreeItem(int itemNo)
		{
			if (itemNo - 10000 > 0)
			{
				return itemNo - 10000;
			}
			return 0;
		}
	}

	public class ParkStage
	{
		public static int MinParkStageNo = 500000;

		public static int MaxParkStageNo = 600000;

		public static bool isParkStage(int stageNo)
		{
			if (stageNo >= MinParkStageNo && stageNo < MaxParkStageNo)
			{
				return true;
			}
			return false;
		}
	}

	public class Boss
	{
		public const int BaseBossStageNo = 30000;

		public const int BaseBossLevelNo = 1000;

		public const int RequireKeyNum = 5;

		public const int MaxLevel = 5;

		public static bool isBossStage(int stageNo)
		{
			if (stageNo >= 30000)
			{
				return true;
			}
			return false;
		}

		public static int convNoToLevel(int stageNo)
		{
			if (!isBossStage(stageNo))
			{
				return stageNo - 30000;
			}
			return stageNo - 30000;
		}

		public static int convLevelToNo(BossStageInfo.eBossType level)
		{
			return (int)(level + 30000 + 1);
		}

		public static int convBossInfoToNo(int type, int level)
		{
			return 30000 + level * 1000 + (type + 1);
		}

		public static int getLeastLevelStageNo()
		{
			return convLevelToNo(BossStageInfo.eBossType.Owl) + 1;
		}

		public static int getHighestLevelStageNo()
		{
			return convLevelToNo(BossStageInfo.eBossType.SkullDragon);
		}
	}

	public class Item
	{
		public enum eType
		{
			Invalid = -1,
			HyperBubble = 1,
			BombBubble = 2,
			ShakeBubble = 3,
			SuperGuide = 4,
			BubblePlus = 5,
			ScoreUp = 6,
			ChangeUp = 7,
			Replay = 8,
			MinusGuard = 9,
			SkullBarrier = 10,
			TimePlus = 11,
			MetalBubble = 12,
			IceBubble = 13,
			FireBubble = 14,
			BubbleSearch = 15,
			BeeBarrier = 16,
			WaterBubble = 17,
			ShineBubble = 18,
			TimeStop = 19,
			Vacuum = 20,
			PowerUp = 21,
			ReWind = 22,
			ScoreUpR = 23,
			DeleteColor = 24,
			LightningG = 101
		}

		public static bool IsAutoUse(eType itemType)
		{
			if (itemType == eType.HyperBubble || itemType == eType.BombBubble || itemType == eType.ShakeBubble || itemType == eType.MetalBubble || itemType == eType.IceBubble || itemType == eType.FireBubble || itemType == eType.BubbleSearch || itemType == eType.WaterBubble || itemType == eType.ShineBubble || itemType == eType.LightningG)
			{
				return false;
			}
			return true;
		}
	}

	public class Avatar
	{
		public enum eRank
		{
			Rank_B = 0,
			Rank_A = 1,
			Rank_S = 2,
			Rank_SS = 3
		}

		public enum eRankMaxLevel
		{
			Rank_A = 10,
			Rank_S = 20,
			Rank_SS = 30
		}

		public enum eBaseRankIndex
		{
			Rank_B = 20000,
			Rank_A = 21000,
			Rank_S = 22000,
			Rank_SS = 23000
		}

		public enum eThrowCharacter
		{
			Bubblen = 0,
			Bubblen_March = 1,
			Kururun = 2,
			Drank = 3,
			Zenchan = 4,
			Maita = 5,
			Chackn = 6,
			Bobblen = 7,
			Bubblen_Pirates = 8,
			Dranko = 9,
			Bubblen_Scornful_Eyes = 10,
			Bubby = 11,
			Bubblen_Rabbit = 12,
			Bobblen_Wolf = 13,
			Cororon_Witch = 14,
			Kururun_Witch = 15
		}

		public enum eSupportCharacter
		{
			Bobblen = 0,
			Bobblen_March = 1,
			Cororon = 2,
			Zenchan_Red = 3,
			Maita_Red = 4,
			Drank = 5,
			Kururun = 6,
			Bobblen_Pirates = 7,
			Bobblen_Scornful_Eyes = 8,
			Bobby = 9,
			Bobblen_Bear = 10,
			Bubblen_Sheep = 11,
			Kururun_blackcat = 12,
			Maita = 13
		}

		public enum eSpecialSkill
		{
			None = 0,
			ComboMaster = 1,
			FriendIncidenceUp = 2,
			Bomb = 3,
			NextChange = 4,
			Coinup = 5,
			GuideStretch = 6,
			NextPlus = 7,
			NextChange_2 = 8,
			BombPlus = 9,
			MetalPlus = 10,
			Water = 11,
			MinorBombPlus = 12,
			WaterPlus = 13,
			Metal = 14,
			CoinupPlus = 15,
			GuidePlus = 16,
			NextChangePlus = 17,
			BombPlusPlus = 18,
			MetalPlusPlus = 19,
			WaterPlusPlus = 20,
			NextPlusPlus = 21,
			CoinupPlusPlus = 22,
			GuideBomb = 30,
			NextPlusNextChange_2 = 31,
			GgideMetalPlus = 32,
			GgideWaterPlus = 33,
			GuidePlusBombPlusPlus = 34,
			GuidePlusMetalPlusPlus = 35,
			GuidePlusCoinupPlusPlus = 36,
			GgidePlusWaterPlusPlus = 37,
			GgidePlusNextChangePlus = 38
		}

		public enum eBaseSkill
		{
			Red = 0,
			Green = 1,
			Blue = 2,
			Yellow = 3,
			Orange = 4,
			Purple = 5,
			White = 6,
			Black = 7
		}

		public static eSpecialSkill[,] SpecialSkills = new eSpecialSkill[9, 2]
		{
			{
				eSpecialSkill.GuideStretch,
				eSpecialSkill.BombPlus
			},
			{
				eSpecialSkill.NextPlus,
				eSpecialSkill.NextChange_2
			},
			{
				eSpecialSkill.GuideStretch,
				eSpecialSkill.MetalPlus
			},
			{
				eSpecialSkill.GuideStretch,
				eSpecialSkill.WaterPlus
			},
			{
				eSpecialSkill.GuidePlus,
				eSpecialSkill.BombPlusPlus
			},
			{
				eSpecialSkill.GuidePlus,
				eSpecialSkill.MetalPlusPlus
			},
			{
				eSpecialSkill.GuidePlus,
				eSpecialSkill.CoinupPlusPlus
			},
			{
				eSpecialSkill.GuidePlus,
				eSpecialSkill.WaterPlusPlus
			},
			{
				eSpecialSkill.GuidePlus,
				eSpecialSkill.NextChangePlus
			}
		};
	}

	public enum eSetShopRewardType
	{
		Coin = 1,
		Heart = 3,
		GachaTicket = 4,
		StageItemBase = 1000
	}

	public class UserName
	{
		public static int DeadWidth = 270;

		public static bool CheckLenOver(UILabel label)
		{
			return (NGUIUtility.GetTextWidthMax(label) > DeadWidth) ? true : false;
		}

		public static string ReplaceOverStr(UILabel label)
		{
			if (!CheckLenOver(label))
			{
				return label.text;
			}
			return NGUIUtility.LeftWidth(label, DeadWidth) + "...";
		}
	}

	public class SoundUtil
	{
		public static void PlayDecideSE()
		{
			if ((bool)Sound.Instance)
			{
				Sound.Instance.playSe(Sound.eSe.SE_201_kettei);
			}
		}

		public static void PlayButtonSE()
		{
			if ((bool)Sound.Instance)
			{
				Sound.Instance.playSe(Sound.eSe.SE_203_cursor2);
			}
		}

		public static void PlayCancelSE()
		{
			if ((bool)Sound.Instance)
			{
				Sound.Instance.playSe(Sound.eSe.SE_202_cancel);
			}
		}
	}

	public class MessageUtil
	{
		public enum eTargetType
		{
			Setup = 0,
			Clear = 1,
			Failed = 2,
			Continue = 3,
			Game = 4
		}

		public static string getLevelMsg(EventStageInfo.eLevel level, MessageResource msgRes)
		{
			string result = string.Empty;
			switch (level)
			{
			case EventStageInfo.eLevel.Easy:
				result = msgRes.getMessage(100);
				break;
			case EventStageInfo.eLevel.Normal:
				result = msgRes.getMessage(101);
				break;
			case EventStageInfo.eLevel.Hard:
				result = msgRes.getMessage(102);
				break;
			}
			return result;
		}

		public static string getLevelMsg(int stageNo, int eventNo, MessageResource msgRes)
		{
			if (!Event.isEventStage(stageNo))
			{
				return string.Empty;
			}
			EventStageInfo.eLevel level = (EventStageInfo.eLevel)Event.convNoToLevel(stageNo, eventNo);
			return getLevelMsg(level, msgRes);
		}

		public static string getBossMsg(BossStageInfo.eBossType level, MessageResource msgRes)
		{
			string result = string.Empty;
			switch (level)
			{
			case BossStageInfo.eBossType.Owl:
				result = msgRes.getMessage(100);
				break;
			case BossStageInfo.eBossType.Crab:
				result = msgRes.getMessage(101);
				break;
			case BossStageInfo.eBossType.SkullDragon:
				result = msgRes.getMessage(102);
				break;
			}
			return result;
		}

		public static string getBossMsg(int stageNo, MessageResource msgRes)
		{
			if (!Event.isEventStage(stageNo))
			{
				return string.Empty;
			}
			BossStageInfo.eBossType level = (BossStageInfo.eBossType)Boss.convNoToLevel(stageNo);
			return getBossMsg(level, msgRes);
		}

		public static string getTargetMsg(StageInfo.CommonInfo info, MessageResource msgRes, eTargetType type)
		{
			string result = string.Empty;
			switch (type)
			{
			case eTargetType.Setup:
				result = getTargetMsgSetup(info, msgRes);
				break;
			case eTargetType.Clear:
				result = getTargetMsgClear(info, msgRes);
				break;
			case eTargetType.Failed:
				result = getTargetMsgFailed(info, msgRes);
				break;
			case eTargetType.Game:
				result = getTargetMsgGame(info, msgRes);
				break;
			case eTargetType.Continue:
				result = getTargetMsgContinue(info, msgRes);
				break;
			}
			return result;
		}

		private static string getTargetMsgSetup(StageInfo.CommonInfo info, MessageResource msgRes)
		{
			string text = string.Empty;
			if (info.Score > 0)
			{
				text += msgRes.getMessage(2600);
				text = msgRes.castCtrlCode(text, 1, info.Score.ToString("N0"));
			}
			text = ((info.Move <= 0) ? (text + msgRes.getMessage(2602)) : (text + msgRes.getMessage(2601)));
			if (info.IsAllDelete || info.IsFulcrumDelete)
			{
				return text + msgRes.getMessage(2604);
			}
			if (info.IsFriendDelete)
			{
				return text + msgRes.getMessage(2605);
			}
			if (info.IsMinilenDelete)
			{
				return text + msgRes.getMessage(2606);
			}
			return text + msgRes.getMessage(2603);
		}

		private static string getTargetMsgGame(StageInfo.CommonInfo info, MessageResource msgRes)
		{
			string empty = string.Empty;
			empty = ((info.Move <= 0) ? (empty + msgRes.getMessage(2651)) : (empty + msgRes.getMessage(2650)));
			if (info.IsAllDelete || info.IsFulcrumDelete)
			{
				return addNewLine(empty) + msgRes.getMessage(2653);
			}
			if (info.IsFriendDelete)
			{
				return msgRes.getMessage(2654);
			}
			if (info.IsMinilenDelete)
			{
				return msgRes.getMessage(2655);
			}
			return addNewLine(empty) + msgRes.getMessage(2652);
		}

		private static string getTargetMsgContinue(StageInfo.CommonInfo info, MessageResource msgRes)
		{
			string text = string.Empty;
			if (info.Score > 0)
			{
				text += msgRes.getMessage(2700);
				text = msgRes.castCtrlCode(text, 1, info.Score.ToString("N0"));
			}
			if (info.IsAllDelete || info.IsFulcrumDelete)
			{
				text = addNewLine(text) + msgRes.getMessage(2701);
			}
			else if (info.IsFriendDelete)
			{
				text = addNewLine(text) + msgRes.getMessage(2702);
			}
			else if (info.IsMinilenDelete)
			{
				text = addNewLine(text) + msgRes.getMessage(2703);
			}
			return text;
		}

		private static string getTargetMsgClear(StageInfo.CommonInfo info, MessageResource msgRes)
		{
			string empty = string.Empty;
			if (info.IsAllDelete || info.IsFulcrumDelete)
			{
				return empty + msgRes.getMessage(2751);
			}
			if (info.IsFriendDelete)
			{
				return empty + msgRes.getMessage(2752);
			}
			if (info.IsMinilenDelete)
			{
				return empty + msgRes.getMessage(2753);
			}
			return empty + msgRes.getMessage(2750);
		}

		private static string getTargetMsgFailed(StageInfo.CommonInfo info, MessageResource msgRes)
		{
			string text = string.Empty;
			if (info.Score > 0)
			{
				text += msgRes.getMessage(2800);
			}
			if (info.IsAllDelete || info.IsFulcrumDelete)
			{
				text = addNewLine(text) + msgRes.getMessage(2801);
			}
			else if (info.IsFriendDelete)
			{
				text = addNewLine(text) + msgRes.getMessage(2802);
			}
			else if (info.IsMinilenDelete)
			{
				text = addNewLine(text) + msgRes.getMessage(2803);
			}
			return text;
		}

		private static string addNewLine(string msg)
		{
			return msg + ((msg.Length < 1) ? string.Empty : Environment.NewLine);
		}
	}

	public enum eDownloadDataNo
	{
		Main = 0,
		BG = 100,
		WorldMap = 150,
		ThrowChara = 200,
		SupportChara = 300,
		ScenarioChara = 400,
		HowtoPlay_JP = 500,
		HowtoPlay_EN = 600,
		Park = 800,
		Minilen = 900
	}

	public enum eOutsideFile_main
	{
		Main_0 = 0,
		Main_1 = 1,
		Main_2 = 2,
		Main_3 = 3,
		Stage_0 = 4,
		Stage_1 = 5,
		Stage_2 = 6,
		Stage_3 = 7,
		Stage_4 = 8,
		Gacha_0 = 9,
		Scenario = 10,
		Main_4 = 11,
		Main_5 = 12,
		Max = 13
	}

	public enum eOutsideFile_BG
	{
		BG01 = 0,
		BG02 = 1,
		BG03 = 2,
		BG04 = 3,
		BG05 = 4,
		BG06 = 5,
		BG07 = 6,
		BG08 = 7,
		BG09 = 8,
		BG10 = 9,
		BG11 = 10,
		BG12 = 11,
		BG50 = 12,
		BG51 = 13,
		BG52 = 14,
		BG53 = 15,
		Max = 16
	}

	public enum eOutsideFile_worldmap
	{
		worldmap_00 = 0,
		worldmap_01 = 1,
		worldmap_02 = 2,
		worldmap_03 = 3,
		worldmap_04 = 4,
		worldmap_05 = 5,
		Max = 6
	}

	public enum eOutsideFile_throw_chara
	{
		chara_00 = 0,
		chara_00_00 = 1,
		chara_00_01 = 2,
		chara_00_02 = 3,
		chara_00_03 = 4,
		chara_00_04 = 5,
		chara_00_05 = 6,
		chara_00_06 = 7,
		chara_00_07 = 8,
		chara_00_08 = 9,
		chara_00_09 = 10,
		chara_00_10 = 11,
		chara_00_11 = 12,
		chara_00_12 = 13,
		chara_00_13 = 14,
		chara_00_14 = 15,
		chara_00_15 = 16,
		chara_00_16 = 17,
		chara_00_17 = 18,
		chara_00_18 = 19,
		chara_00_19 = 20,
		chara_00_20 = 21,
		chara_00_21 = 22,
		chara_00_22 = 23,
		chara_00_23 = 24,
		chara_00_24 = 25,
		chara_00_25 = 26,
		chara_00_26 = 27,
		chara_00_27 = 28,
		chara_00_28 = 29,
		chara_00_29 = 30,
		chara_00_30 = 31,
		chara_00_31 = 32,
		chara_00_32 = 33,
		chara_00_33 = 34,
		chara_00_34 = 35,
		Max = 36
	}

	public enum eOutsideFile_support_chara
	{
		chara_01 = 0,
		chara_01_00 = 1,
		chara_01_01 = 2,
		chara_01_02 = 3,
		chara_01_03 = 4,
		chara_01_04 = 5,
		chara_01_05 = 6,
		chara_01_06 = 7,
		chara_01_07 = 8,
		chara_01_08 = 9,
		chara_01_09 = 10,
		chara_01_10 = 11,
		chara_01_11 = 12,
		chara_01_12 = 13,
		chara_01_13 = 14,
		chara_01_14 = 15,
		chara_01_15 = 16,
		chara_01_16 = 17,
		chara_01_17 = 18,
		chara_01_18 = 19,
		chara_01_19 = 20,
		chara_01_20 = 21,
		chara_01_21 = 22,
		chara_01_22 = 23,
		chara_01_23 = 24,
		chara_01_24 = 25,
		chara_01_25 = 26,
		chara_01_26 = 27,
		chara_01_27 = 28,
		chara_01_28 = 29,
		chara_01_29 = 30,
		chara_01_30 = 31,
		chara_01_31 = 32,
		chara_01_32 = 33,
		chara_01_33 = 34,
		chara_01_34 = 35,
		chara_01_35 = 36,
		Max = 37
	}

	public enum eOutsideFile_scenario_chara
	{
		scenario_chara_00 = 0,
		scenario_chara_01 = 1,
		scenario_chara_02 = 2,
		scenario_chara_10 = 3,
		Max = 4
	}

	public enum eOutsideFile_howtoplay
	{
		howToPlay_00 = 0,
		howToPlay_01 = 1,
		howToPlay_02 = 2,
		howToPlay_03 = 3,
		howToPlay_04 = 4,
		howToPlay_05 = 5,
		howToPlay_06 = 6,
		howToPlay_07 = 7,
		howToPlay_08 = 8,
		howToPlay_09 = 9,
		howToPlay_10 = 10,
		howToPlay_11 = 11,
		howToPlay_12 = 12,
		howToPlay_13 = 13,
		howToPlay_14 = 14,
		howToPlay_15 = 15,
		howToPlay_16 = 16,
		howToPlay_17 = 17,
		howToPlay_18 = 18,
		howToPlay_19 = 19,
		howToPlay_20 = 20,
		howToPlay_21 = 21,
		howToPlay_22 = 22,
		howToPlay_23 = 23,
		howToPlay_24 = 24,
		howToPlay_25 = 25,
		howToPlay_26 = 26,
		howToPlay_27 = 27,
		howToPlay_28 = 28,
		howToPlay_29 = 29,
		howToPlay_30 = 30,
		howToPlay_31 = 31,
		howToPlay_32 = 32,
		howToPlay_33 = 33,
		howToPlay_34 = 34,
		howToPlay_35 = 35,
		howToPlay_36 = 36,
		howToPlay_37 = 37,
		howToPlay_38 = 38,
		howToPlay_39 = 39,
		howToPlay_40 = 40,
		howToPlay_41 = 41,
		howToPlay_42 = 42,
		howToPlay_43 = 43,
		howToPlay_44 = 44,
		howToPlay_45 = 45,
		howToPlay_46 = 46,
		howToPlay_47 = 47,
		howToPlay_48 = 48,
		howToPlay_49 = 49,
		howToPlay_50 = 50,
		howToPlay_51 = 51,
		howToPlay_52 = 52,
		howToPlay_53 = 53,
		Max = 54
	}

	public enum eOutsideFile_park
	{
		Buildings_01 = 0,
		Road_01 = 1,
		BG_01 = 2,
		Max = 3
	}

	public enum eOutsideFile_minilen
	{
		minilen_000 = 0,
		minilen_001 = 1,
		minilen_002 = 2,
		minilen_003 = 3,
		minilen_004 = 4,
		minilen_005 = 5,
		minilen_006 = 6,
		minilen_007 = 7,
		minilen_008 = 8,
		minilen_009 = 9,
		minilen_010 = 10,
		minilen_011 = 11,
		minilen_012 = 12,
		minilen_013 = 13,
		minilen_014 = 14,
		minilen_015 = 15,
		minilen_016 = 16,
		minilen_017 = 17,
		minilen_018 = 18,
		minilen_019 = 19,
		minilen_020 = 20,
		minilen_021 = 21,
		minilen_022 = 22,
		minilen_023 = 23,
		minilen_024 = 24,
		minilen_025 = 25,
		Max = 26
	}

	public const int DailyMissionTermStageNo = 5;

	public static int InformationSaveMax = 30;

	public static float FriendPlaceOverlapCount = 3f;

	public static float TimeOutCount = 20f;

	public static int AutoRecoveryHeartMax = 5;

	public static int HeartMax = 999 + AutoRecoveryHeartMax;

	public static int JewelMax = 9999;

	public static int BonusJewelMax = 9999;

	public static int BuyJewelMax = 9999;

	public static int CoinMax = 9999999;

	public static int LevelMax = 99;

	public static int UserScoreMax = 999999;

	public static int ItemMax = 999;

	public static int StarMax = 3;

	public static int SetupItemMax = 4;

	public static int StageItemMax = 5;

	public static int ComboMax = 99;

	public static float timeOutSec = 30f;

	public static int HelpResendTime = 259200;

	public static int HelpResendTimeAfterNotSend = 604800;

	public static int CantHelpSendLoginTime = 2592000;

	public static int ReviewDialogReopenTime = 259200;

	public static int CanReviewStageNo = 15;

	public static string PreReviewKey = "PreReview";

	public static string ReviewCancelTimeKey = "ReviewCancelTime2";

	public static string ReviewEndKey = "ReviewEnd2";

	public static bool isGuestLogin = false;

	public static int[] SKILL_SCORE_LIST = new int[25]
	{
		30, 40, 55, 75, 100, 130, 165, 205, 250, 300,
		333, 369, 408, 450, 495, 543, 594, 648, 705, 765,
		800, 840, 885, 935, 990
	};

	public static int[] SKILL_SCORE_LIST2 = new int[30]
	{
		50, 75, 100, 130, 165, 205, 250, 300, 333, 369,
		408, 450, 495, 543, 594, 648, 705, 765, 800, 840,
		885, 935, 990, 1000, 1050, 1100, 1150, 1200, 1250, 1300
	};

	public static Color[] bubbleColor = new Color[8]
	{
		new Color(1f, 0.671f, 0.647f, 1f),
		new Color(0.404f, 1f, 0.427f, 1f),
		new Color(0.271f, 0.694f, 1f, 1f),
		new Color(0.976f, 0.91f, 0.631f, 1f),
		new Color(1f, 0.721f, 0f, 1f),
		new Color(0.494f, 0.267f, 1f, 1f),
		new Color(1f, 1f, 1f, 1f),
		new Color(0.361f, 0.33f, 0.396f, 1f)
	};

	public static string[] sample = new string[1] { "null" };

	public static string[][] outsideFiles_main = new string[13][]
	{
		new string[1] { "GUI/UI_Main/UI_Main.png" },
		new string[1] { "GUI/UI_Main/UI_Main_2.png" },
		new string[1] { "GUI/UI_Main/UI_Main_localize.png" },
		new string[1] { "GUI/UI_Main/UI_Main_3.png" },
		new string[1] { "GUI/Stage/Stage.png" },
		new string[1] { "GUI/Stage/Stage_localize.png" },
		new string[1] { "GUI/Stage/Stage_localize2.png" },
		new string[1] { "GUI/UI_Result/UI_Result.png" },
		new string[1] { "GUI/Stage/Stage_localize3.png" },
		new string[1] { "GUI/UI_Main/UI_Gacha.png" },
		new string[1] { "GUI/Scenario/Snenario.png" },
		new string[1] { "GUI/UI_Main/UI_Main_4.png" },
		new string[1] { "GUI/UI_Main/UI_Main_5.png" }
	};

	public static string[][] outsideFiles_BG = new string[16][]
	{
		new string[1] { "GUI/Stage/bg_01.png" },
		new string[1] { "GUI/Stage/bg_02.png" },
		new string[1] { "GUI/Stage/bg_03.png" },
		new string[1] { "GUI/Stage/bg_04.png" },
		new string[1] { "GUI/Stage/bg_05.png" },
		new string[1] { "GUI/Stage/bg_06.png" },
		new string[1] { "GUI/Stage/bg_07.png" },
		new string[1] { "GUI/Stage/bg_08.png" },
		new string[1] { "GUI/Stage/bg_09.png" },
		new string[1] { "GUI/Stage/bg_10.png" },
		new string[1] { "GUI/Stage/bg_11.png" },
		new string[1] { "GUI/Stage/bg_12.png" },
		new string[1] { "GUI/Stage/bg_50.png" },
		new string[1] { "GUI/Stage/bg_51.png" },
		new string[1] { "GUI/Stage/bg_52.png" },
		new string[1] { "GUI/Stage/bg_53.png" }
	};

	public static string[][] outsideFiles_worldmap = new string[6][]
	{
		new string[1] { "GUI/Worldmap/worldmap_000.png" },
		new string[1] { "GUI/Worldmap/worldmap_001.png" },
		new string[1] { "GUI/Worldmap/worldmap_002.png" },
		new string[1] { "GUI/Worldmap/worldmap_003.png" },
		new string[1] { "GUI/Worldmap/worldmap_004.png" },
		new string[1] { "GUI/Worldmap/worldmap_005.png" }
	};

	public static string[][] outsideFiles_throw_chara = new string[36][]
	{
		new string[1] { "SA_Data/chara_00 Data/chara_00.png" },
		new string[1] { "SA_Data/chara_00_00 Data/chara_00_00.png" },
		new string[1] { "SA_Data/chara_00_01 Data/chara_00_01.png" },
		new string[1] { "SA_Data/chara_00_02 Data/chara_00_02.png" },
		new string[1] { "SA_Data/chara_00_03 Data/chara_00_03.png" },
		new string[1] { "SA_Data/chara_00_04 Data/chara_00_04.png" },
		new string[1] { "SA_Data/chara_00_05 Data/chara_00_05.png" },
		new string[1] { "SA_Data/chara_00_06 Data/chara_00_06.png" },
		new string[1] { "SA_Data/chara_00_07 Data/chara_00_07.png" },
		new string[1] { "SA_Data/chara_00_08 Data/chara_00_08.png" },
		new string[1] { "SA_Data/chara_00_09 Data/chara_00_09.png" },
		new string[1] { "SA_Data/chara_00_10 Data/chara_00_10.png" },
		new string[1] { "SA_Data/chara_00_11 Data/chara_00_11.png" },
		new string[1] { "SA_Data/chara_00_12 Data/chara_00_12.png" },
		new string[1] { "SA_Data/chara_00_13 Data/chara_00_13.png" },
		new string[1] { "SA_Data/chara_00_14 Data/chara_00_14.png" },
		new string[1] { "SA_Data/chara_00_15 Data/chara_00_15.png" },
		new string[1] { "SA_Data/chara_00_16 Data/chara_00_16.png" },
		new string[1] { "SA_Data/chara_00_17 Data/chara_00_17.png" },
		new string[1] { "SA_Data/chara_00_18 Data/chara_00_18.png" },
		new string[1] { "SA_Data/chara_00_19 Data/chara_00_19.png" },
		new string[1] { "SA_Data/chara_00_20 Data/chara_00_20.png" },
		new string[1] { "SA_Data/chara_00_21 Data/chara_00_21.png" },
		new string[1] { "SA_Data/chara_00_22 Data/chara_00_22.png" },
		new string[1] { "SA_Data/chara_00_23 Data/chara_00_23.png" },
		new string[1] { "SA_Data/chara_00_24 Data/chara_00_24.png" },
		new string[1] { "SA_Data/chara_00_25 Data/chara_00_25.png" },
		new string[1] { "SA_Data/chara_00_26 Data/chara_00_26.png" },
		new string[1] { "SA_Data/chara_00_27 Data/chara_00_27.png" },
		new string[1] { "SA_Data/chara_00_28 Data/chara_00_28.png" },
		new string[1] { "SA_Data/chara_00_29 Data/chara_00_29.png" },
		new string[1] { "SA_Data/chara_00_30 Data/chara_00_30.png" },
		new string[1] { "SA_Data/chara_00_31 Data/chara_00_31.png" },
		new string[1] { "SA_Data/chara_00_32 Data/chara_00_32.png" },
		new string[1] { "SA_Data/chara_00_33 Data/chara_00_33.png" },
		new string[1] { "SA_Data/chara_00_34 Data/chara_00_34.png" }
	};

	public static string[][] outsideFiles_support_chara = new string[37][]
	{
		new string[1] { "SA_Data/chara_01 Data/chara_01.png" },
		new string[1] { "SA_Data/chara_01_00 Data/chara_01_00.png" },
		new string[1] { "SA_Data/chara_01_01 Data/chara_01_01.png" },
		new string[1] { "SA_Data/chara_01_02 Data/chara_01_02.png" },
		new string[1] { "SA_Data/chara_01_03 Data/chara_01_03.png" },
		new string[1] { "SA_Data/chara_01_04 Data/chara_01_04.png" },
		new string[1] { "SA_Data/chara_01_05 Data/chara_01_05.png" },
		new string[1] { "SA_Data/chara_01_06 Data/chara_01_06.png" },
		new string[1] { "SA_Data/chara_01_07 Data/chara_01_07.png" },
		new string[1] { "SA_Data/chara_01_08 Data/chara_01_08.png" },
		new string[1] { "SA_Data/chara_01_09 Data/chara_01_09.png" },
		new string[1] { "SA_Data/chara_01_10 Data/chara_01_10.png" },
		new string[1] { "SA_Data/chara_01_11 Data/chara_01_11.png" },
		new string[1] { "SA_Data/chara_01_12 Data/chara_01_12.png" },
		new string[1] { "SA_Data/chara_01_13 Data/chara_01_13.png" },
		new string[1] { "SA_Data/chara_01_14 Data/chara_01_14.png" },
		new string[1] { "SA_Data/chara_01_15 Data/chara_01_15.png" },
		new string[1] { "SA_Data/chara_01_16 Data/chara_01_16.png" },
		new string[1] { "SA_Data/chara_01_17 Data/chara_01_17.png" },
		new string[1] { "SA_Data/chara_01_18 Data/chara_01_18.png" },
		new string[1] { "SA_Data/chara_01_19 Data/chara_01_19.png" },
		new string[1] { "SA_Data/chara_01_20 Data/chara_01_20.png" },
		new string[1] { "SA_Data/chara_01_21 Data/chara_01_21.png" },
		new string[1] { "SA_Data/chara_01_22 Data/chara_01_22.png" },
		new string[1] { "SA_Data/chara_01_23 Data/chara_01_23.png" },
		new string[1] { "SA_Data/chara_01_24 Data/chara_01_24.png" },
		new string[1] { "SA_Data/chara_01_25 Data/chara_01_25.png" },
		new string[1] { "SA_Data/chara_01_26 Data/chara_01_26.png" },
		new string[1] { "SA_Data/chara_01_27 Data/chara_01_27.png" },
		new string[1] { "SA_Data/chara_01_28 Data/chara_01_28.png" },
		new string[1] { "SA_Data/chara_01_29 Data/chara_01_29.png" },
		new string[1] { "SA_Data/chara_01_30 Data/chara_01_30.png" },
		new string[1] { "SA_Data/chara_01_31 Data/chara_01_31.png" },
		new string[1] { "SA_Data/chara_01_32 Data/chara_01_32.png" },
		new string[1] { "SA_Data/chara_01_33 Data/chara_01_33.png" },
		new string[1] { "SA_Data/chara_01_34 Data/chara_01_34.png" },
		new string[1] { "SA_Data/chara_01_35 Data/chara_01_35.png" }
	};

	public static string[][] outsideFiles_scenario_chara = new string[4][]
	{
		new string[1] { "SA_Data/Scenario_chara_total01 Data/Scenario_chara_total01.png" },
		new string[1] { "SA_Data/Scenario_chara_total02 Data/Scenario_chara_total02.png" },
		new string[1] { "SA_Data/Scenario_chara_total03 Data/Scenario_chara_total03.png" },
		new string[1] { "SA_Data/Scenario_chara_total10 Data/Scenario_chara_total10.png" }
	};

	public static string[][] outsideFilesLow_main = new string[13][]
	{
		new string[1] { "GUI_Low/UI_Main/UI_Main_Low.png" },
		new string[1] { "GUI_Low/UI_Main/UI_Main_2_Low.png" },
		new string[1] { "GUI_Low/UI_Main/UI_Main_localize_Low.png" },
		new string[1] { "GUI_Low/UI_Main/UI_Main_3_Low.png" },
		new string[1] { "GUI_Low/Stage/Stage_Low.png" },
		new string[1] { "GUI_Low/Stage/Stage_localize_Low.png" },
		new string[1] { "GUI_Low/Stage/Stage_localize2_Low.png" },
		new string[1] { "GUI_Low/UI_Result/UI_Result_Low.png" },
		new string[1] { "GUI_Low/Stage/Stage_localize3_Low.png" },
		new string[1] { "GUI_Low/UI_Main/UI_Gacha_Low.png" },
		new string[1] { "GUI_Low/Scenario/Snenario_Low.png" },
		new string[1] { "GUI_Low/UI_Main/UI_Main_4_Low.png" },
		new string[1] { "GUI_Low/UI_Main/UI_Main_5_Low.png" }
	};

	public static string[][] outsideFilesLow_BG = new string[16][]
	{
		new string[1] { "GUI_Low/Stage/bg_01_Low.png" },
		new string[1] { "GUI_Low/Stage/bg_02_Low.png" },
		new string[1] { "GUI_Low/Stage/bg_03_Low.png" },
		new string[1] { "GUI_Low/Stage/bg_04_Low.png" },
		new string[1] { "GUI_Low/Stage/bg_05_Low.png" },
		new string[1] { "GUI_Low/Stage/bg_06_Low.png" },
		new string[1] { "GUI_Low/Stage/bg_07_Low.png" },
		new string[1] { "GUI_Low/Stage/bg_08_Low.png" },
		new string[1] { "GUI_Low/Stage/bg_09_Low.png" },
		new string[1] { "GUI_Low/Stage/bg_10_Low.png" },
		new string[1] { "GUI_Low/Stage/bg_11_Low.png" },
		new string[1] { "GUI_Low/Stage/bg_12_Low.png" },
		new string[1] { "GUI_Low/Stage/bg_50_Low.png" },
		new string[1] { "GUI_Low/Stage/bg_51_Low.png" },
		new string[1] { "GUI_Low/Stage/bg_52_Low.png" },
		new string[1] { "GUI_Low/Stage/bg_53_Low.png" }
	};

	public static string[][] outsideFilesLow_worldmap = new string[6][]
	{
		new string[1] { "GUI_Low/Worldmap/worldmap_000_Low.png" },
		new string[1] { "GUI_Low/Worldmap/worldmap_001_Low.png" },
		new string[1] { "GUI_Low/Worldmap/worldmap_002_Low.png" },
		new string[1] { "GUI_Low/Worldmap/worldmap_003_Low.png" },
		new string[1] { "GUI_Low/Worldmap/worldmap_004_Low.png" },
		new string[1] { "GUI_Low/Worldmap/worldmap_005_Low.png" }
	};

	public static string[][] outsideFilesLow_throw_chara = new string[36][]
	{
		new string[1] { "SA_Data_Low/chara_00 Data/chara_00_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_00 Data/chara_00_00_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_01 Data/chara_00_01_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_02 Data/chara_00_02_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_03 Data/chara_00_03_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_04 Data/chara_00_04_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_05 Data/chara_00_05_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_06 Data/chara_00_06_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_07 Data/chara_00_07_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_08 Data/chara_00_08_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_09 Data/chara_00_09_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_10 Data/chara_00_10_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_11 Data/chara_00_11_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_12 Data/chara_00_12_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_13 Data/chara_00_13_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_14 Data/chara_00_14_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_15 Data/chara_00_15_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_16 Data/chara_00_16_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_17 Data/chara_00_17_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_18 Data/chara_00_18_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_19 Data/chara_00_19_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_20 Data/chara_00_20_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_21 Data/chara_00_21_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_22 Data/chara_00_22_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_23 Data/chara_00_23_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_24 Data/chara_00_24_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_25 Data/chara_00_25_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_26 Data/chara_00_26_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_27 Data/chara_00_27_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_28 Data/chara_00_28_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_29 Data/chara_00_29_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_30 Data/chara_00_30_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_31 Data/chara_00_31_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_32 Data/chara_00_32_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_33 Data/chara_00_33_Low.png" },
		new string[1] { "SA_Data_Low/chara_00_34 Data/chara_00_34_Low.png" }
	};

	public static string[][] outsideFilesLow_support_chara = new string[37][]
	{
		new string[1] { "SA_Data_Low/chara_01 Data/chara_01_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_00 Data/chara_01_00_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_01 Data/chara_01_01_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_02 Data/chara_01_02_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_03 Data/chara_01_03_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_04 Data/chara_01_04_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_05 Data/chara_01_05_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_06 Data/chara_01_06_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_07 Data/chara_01_07_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_08 Data/chara_01_08_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_09 Data/chara_01_09_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_10 Data/chara_01_10_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_11 Data/chara_01_11_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_12 Data/chara_01_12_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_13 Data/chara_01_13_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_14 Data/chara_01_14_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_15 Data/chara_01_15_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_16 Data/chara_01_16_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_17 Data/chara_01_17_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_18 Data/chara_01_18_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_19 Data/chara_01_19_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_20 Data/chara_01_20_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_21 Data/chara_01_21_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_22 Data/chara_01_22_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_23 Data/chara_01_23_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_24 Data/chara_01_24_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_25 Data/chara_01_25_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_26 Data/chara_01_26_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_27 Data/chara_01_27_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_28 Data/chara_01_28_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_29 Data/chara_01_29_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_30 Data/chara_01_30_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_31 Data/chara_01_31_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_32 Data/chara_01_32_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_33 Data/chara_01_33_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_34 Data/chara_01_34_Low.png" },
		new string[1] { "SA_Data_Low/chara_01_35 Data/chara_01_35_Low.png" }
	};

	public static string[][] outsideFilesLow_scenario_chara = new string[4][]
	{
		new string[1] { "SA_Data_Low/Scenario_chara_total01 Data/Scenario_chara_total01_Low.png" },
		new string[1] { "SA_Data_Low/Scenario_chara_total02 Data/Scenario_chara_total02_Low.png" },
		new string[1] { "SA_Data_Low/Scenario_chara_total03 Data/Scenario_chara_total03_Low.png" },
		new string[1] { "SA_Data_Low/Scenario_chara_total10 Data/Scenario_chara_total10_Low.png" }
	};

	public static string[][] outsideFiles_howtoplay_JP = new string[54][]
	{
		new string[1] { "Howtoplay/howtoplay_00.png" },
		new string[1] { "Howtoplay/howtoplay_01.png" },
		new string[1] { "Howtoplay/howtoplay_02.png" },
		new string[1] { "Howtoplay/howtoplay_03.png" },
		new string[1] { "Howtoplay/howtoplay_04.png" },
		new string[1] { "Howtoplay/howtoplay_05.png" },
		new string[1] { "Howtoplay/howtoplay_06.png" },
		new string[1] { "Howtoplay/howtoplay_07.png" },
		new string[1] { "Howtoplay/howtoplay_08.png" },
		new string[1] { "Howtoplay/howtoplay_09.png" },
		new string[1] { "Howtoplay/howtoplay_10.png" },
		new string[1] { "Howtoplay/howtoplay_11.png" },
		new string[1] { "Howtoplay/howtoplay_12.png" },
		new string[1] { "Howtoplay/howtoplay_13.png" },
		new string[1] { "Howtoplay/howtoplay_14.png" },
		new string[1] { "Howtoplay/howtoplay_15.png" },
		new string[1] { "Howtoplay/howtoplay_16.png" },
		new string[1] { "Howtoplay/howtoplay_17.png" },
		new string[1] { "Howtoplay/howtoplay_18.png" },
		new string[1] { "Howtoplay/howtoplay_19.png" },
		new string[1] { "Howtoplay/howtoplay_20.png" },
		new string[1] { "Howtoplay/howtoplay_21.png" },
		new string[1] { "Howtoplay/howtoplay_22.png" },
		new string[1] { "Howtoplay/howtoplay_23.png" },
		new string[1] { "Howtoplay/howtoplay_24.png" },
		new string[1] { "Howtoplay/howtoplay_25.png" },
		new string[1] { "Howtoplay/howtoplay_26.png" },
		new string[1] { "Howtoplay/howtoplay_27.png" },
		new string[1] { "Howtoplay/howtoplay_28.png" },
		new string[1] { "Howtoplay/howtoplay_29.png" },
		new string[1] { "Howtoplay/howtoplay_30.png" },
		new string[1] { "Howtoplay/howtoplay_31.png" },
		new string[1] { "Howtoplay/howtoplay_32.png" },
		new string[1] { "Howtoplay/howtoplay_33.png" },
		new string[1] { "Howtoplay/howtoplay_34.png" },
		new string[1] { "Howtoplay/howtoplay_35.png" },
		new string[1] { "Howtoplay/howtoplay_36.png" },
		new string[1] { "Howtoplay/howtoplay_37.png" },
		new string[1] { "Howtoplay/howtoplay_38.png" },
		new string[1] { "Howtoplay/howtoplay_39.png" },
		new string[1] { "Howtoplay/howtoplay_40.png" },
		new string[1] { "Howtoplay/howtoplay_41.png" },
		new string[1] { "Howtoplay/howtoplay_42.png" },
		new string[1] { "Howtoplay/howtoplay_43.png" },
		new string[1] { "Howtoplay/howtoplay_44.png" },
		new string[1] { "Howtoplay/howtoplay_45.png" },
		new string[1] { "Howtoplay/howtoplay_46.png" },
		new string[1] { "Howtoplay/howtoplay_47.png" },
		new string[1] { "Howtoplay/howtoplay_48.png" },
		new string[1] { "Howtoplay/howtoplay_49.png" },
		new string[1] { "Howtoplay/howtoplay_50.png" },
		new string[1] { "Howtoplay/howtoplay_51.png" },
		new string[1] { "Howtoplay/howtoplay_52.png" },
		new string[1] { "Howtoplay/howtoplay_53.png" }
	};

	public static string[][] outsideFiles_howtoplay_EN = new string[54][]
	{
		new string[1] { "Howtoplay_EN/howtoplay_00_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_01_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_02_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_03_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_04_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_05_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_06_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_07_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_08_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_09_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_10_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_11_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_12_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_13_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_14_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_15_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_16_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_17_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_18_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_19_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_20_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_21_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_22_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_23_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_24_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_25_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_26_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_27_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_28_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_29_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_30_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_31_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_32_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_33_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_34_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_35_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_36_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_37_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_38_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_39_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_40_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_41_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_42_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_43_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_44_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_45_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_46_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_47_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_48_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_49_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_50_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_51_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_52_en.png" },
		new string[1] { "Howtoplay_EN/howtoplay_53_en.png" }
	};

	public static string[][] outsideFiles_park = new string[3][]
	{
		new string[1] { "ParkResources_Additions/Parkparts/Atlas/A_Building_0000.png" },
		new string[1] { "ParkResources_Additions/Parkparts/Atlas/A_Road_0001.png" },
		new string[1] { "ParkResources_Additions/Parkparts/BG/A_BG_001.png" }
	};

	public static string[][] outsideFiles_minilen = new string[26][]
	{
		new string[1] { "ParkResources_Additions/Minilen/Mini_000/A_Mini_000.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_001/A_Mini_001.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_002/A_Mini_002.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_003/A_Mini_003.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_004/A_Mini_004.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_005/A_Mini_005.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_006/A_Mini_006.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_007/A_Mini_007.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_008/A_Mini_008.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_009/A_Mini_009.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_010/A_Mini_010.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_011/A_Mini_011.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_012/A_Mini_012.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_013/A_Mini_013.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_014/A_Mini_014.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_015/A_Mini_015.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_016/A_Mini_016.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_017/A_Mini_017.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_018/A_Mini_018.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_019/A_Mini_019.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_020/A_Mini_020.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_021/A_Mini_021.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_022/A_Mini_022.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_023/A_Mini_023.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_024/A_Mini_024.png" },
		new string[1] { "ParkResources_Additions/Minilen/Mini_025/A_Mini_025.png" }
	};

	public static string[] delete_Directories = new string[4] { "Assets/NGUI_Sources/Minilen", "Assets/NGUI_Sources/Parkparts/BG", "Assets/NGUI_Sources/Parkparts/Building", "Assets/NGUI_Sources/Parkparts/Garden" };

	public static int ResendTime
	{
		get
		{
			return GlobalData.Instance.getGameData().heartSendHour * 60 * 60;
		}
	}

	public static float iOSVersion()
	{
		return 0f;
	}

	public static string GetDocumentsPath()
	{
		if (!Application.isEditor)
		{
			return Application.persistentDataPath;
		}
		string dataPath = Application.dataPath;
		return dataPath.Substring(0, dataPath.LastIndexOf('/')) + "/Documents";
	}

	public static string JsonGetValue(string jsonString, string property)
	{
		JsonReader jsonReader = new JsonReader(jsonString);
		while (jsonReader.Read())
		{
			if (jsonReader.Token.ToString() == "PropertyName" && jsonReader.Value.ToString() == property)
			{
				jsonReader.Read();
				if (jsonReader.Token.ToString() == "Double" || jsonReader.Token.ToString() == "Int" || jsonReader.Token.ToString() == "String")
				{
					return jsonReader.Value.ToString();
				}
			}
		}
		return string.Empty;
	}

	public static ArrayList JsonGetArrayList(string jsonString, string property)
	{
		ArrayList arrayList = new ArrayList();
		arrayList.Clear();
		JsonReader jsonReader = new JsonReader(jsonString);
		while (jsonReader.Read())
		{
			if (!(jsonReader.Token.ToString() == "PropertyName") || !(jsonReader.Value.ToString() == property))
			{
				continue;
			}
			jsonReader.Read();
			if (!(jsonReader.Token.ToString() == "ArrayStart"))
			{
				continue;
			}
			while (jsonReader.Token.ToString() != "ArrayEnd")
			{
				jsonReader.Read();
				if (jsonReader.Token.ToString() == "Double" || jsonReader.Token.ToString() == "Int" || jsonReader.Token.ToString() == "String")
				{
					arrayList.Add(jsonReader.Value.ToString());
				}
			}
		}
		return arrayList;
	}
}
