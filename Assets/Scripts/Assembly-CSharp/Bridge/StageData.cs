using System;
using Network;

namespace Bridge
{
	public class StageData
	{
		private static Network.StageData getStageData(int stageNo)
		{
			return GlobalData.Instance.getStageData(stageNo);
		}

		private static Network.StageData getRankingStageData()
		{
			return GlobalData.Instance.getRankingStageData();
		}

		public static int getClearCount(int stageNo)
		{
			if (!GlobalData.Instance.isStageData(stageNo))
			{
				return 0;
			}
			return getStageData(stageNo).clearCount;
		}

		public static int getPlayCount_Park(int stageNo)
		{
			if (!Constant.ParkStage.isParkStage(stageNo))
			{
				return 0;
			}
			return getStageData(stageNo).playCount;
		}

		public static int getClearCount_Park(int stageNo)
		{
			if (!Constant.ParkStage.isParkStage(stageNo))
			{
				return 0;
			}
			return getStageData(stageNo).clearCount;
		}

		public static int getRankingClearCount()
		{
			return getRankingStageData().clearCount;
		}

		public static int getPlayCount(int stageNo)
		{
			if (!GlobalData.Instance.isStageData(stageNo))
			{
				return 0;
			}
			return getStageData(stageNo).playCount;
		}

		public static int getRankingPlayCount()
		{
			return getRankingStageData().playCount;
		}

		public static bool isClear(int stageNo)
		{
			if (!GlobalData.Instance.isStageData(stageNo))
			{
				return false;
			}
			return getStageData(stageNo).clearCount > 0;
		}

		public static bool isRankingClear()
		{
			return getRankingStageData().clearCount > 0;
		}

		public static int getHighScore(int stageNo)
		{
			if (!GlobalData.Instance.isStageData(stageNo))
			{
				return 0;
			}
			return getStageData(stageNo).hiscore;
		}

		public static int getRankingStageHighScore()
		{
			return 0;
		}

		public static int getStar(int stageNo)
		{
			if (!GlobalData.Instance.isStageData(stageNo))
			{
				return 0;
			}
			return getStageData(stageNo).star;
		}

		public static int[] getUsedItemType(int stageNo)
		{
			if (!GlobalData.Instance.isStageData(stageNo))
			{
				return null;
			}
			if (getStageData(stageNo).usedItemType == null)
			{
				return null;
			}
			return getStageData(stageNo).usedItemType;
		}

		public static int getTotalScore()
		{
			return GlobalData.Instance.getGameData().allStageScoreSum;
		}

		public static int getTotalStar()
		{
			return GlobalData.Instance.getGameData().allStarSum;
		}

		public static bool isOpen_Park(int stage_no, ParkStageInfo info)
		{
			Network.StageData stageData = Array.Find(GlobalData.Instance.getGameData().parkStageList, (Network.StageData ps) => ps.stageNo == stage_no);
			if (stageData == null)
			{
				return false;
			}
			if (!stageData.isOpen)
			{
				return false;
			}
			return true;
		}

		public static int getHighScore_Park(int stage_no)
		{
			if (!Constant.ParkStage.isParkStage(stage_no))
			{
				return 0;
			}
			Network.StageData stageData = Array.Find(GlobalData.Instance.getGameData().parkStageList, (Network.StageData ps) => ps.stageNo == stage_no);
			if (stageData == null)
			{
				return 0;
			}
			return stageData.hiscore;
		}

		public static int getAreaStar(int areano)
		{
			int num = 0;
			Network.StageData stageData = null;
			for (int i = 0; i < GlobalData.Instance.getStageCount(); i++)
			{
				stageData = GlobalData.Instance.getStageData(i);
				if (stageData != null && areano == stageData.area)
				{
					num += stageData.star;
				}
			}
			return num;
		}

		public static bool IsAreaClear(int areano)
		{
			Network.StageData stageData = null;
			bool result = true;
			for (int i = 0; i < GlobalData.Instance.getStageCount(); i++)
			{
				stageData = GlobalData.Instance.getStageData(i);
				if (stageData != null && areano == stageData.area && !isClear(i))
				{
					result = false;
					break;
				}
			}
			return result;
		}
	}
}
