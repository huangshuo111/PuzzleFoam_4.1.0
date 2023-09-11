using Network;
using UnityEngine;

namespace Bridge
{
	public class PlayerData
	{
		public static bool isInstance()
		{
			if (!GlobalData.IsInstance())
			{
				return false;
			}
			if (GlobalData.Instance.getGameData() == null)
			{
				return false;
			}
			return true;
		}

		public static int getLevel()
		{
			return GlobalData.Instance.getGameData().level;
		}

		public static int getExp()
		{
			return GlobalData.Instance.getGameData().exp;
		}

		public static int getCoin()
		{
			return GlobalData.Instance.getGameData().coin;
		}

		public static void setCoin(int coin)
		{
			GlobalData.Instance.getGameData().coin = coin;
		}

		public static int getContinueNum()
		{
			return GlobalData.Instance.getGameData().continueNum;
		}

		public static int getHeart()
		{
			return GlobalData.Instance.getGameData().heart;
		}

		public static void setHeart(int heart)
		{
			GlobalData.Instance.getGameData().heart = heart;
		}

		public static int getJewel()
		{
			GameData gameData = GlobalData.Instance.getGameData();
			return gameData.bonusJewel + gameData.buyJewel;
		}

		public static int getBonusJewel()
		{
			return GlobalData.Instance.getGameData().bonusJewel;
		}

		public static int getBuyJewel()
		{
			return GlobalData.Instance.getGameData().buyJewel;
		}

		public static void setBonusJewel(int bonusJewel)
		{
			int bonusJewel2 = Mathf.Min(bonusJewel, Constant.BonusJewelMax);
			GlobalData.Instance.getGameData().bonusJewel = bonusJewel2;
		}

		public static void setBuyJewel(int buyJewel)
		{
			int buyJewel2 = Mathf.Min(buyJewel, Constant.BuyJewelMax);
			GlobalData.Instance.getGameData().buyJewel = buyJewel2;
		}

		public static int getCurrentStage()
		{
			GameData gameData = GlobalData.Instance.getGameData();
			StageDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
			if (component != null)
			{
				int maxStageIconsNum = component.GetComponent<StageIconDataTable>().getMaxStageIconsNum();
				if (gameData.progressStageNo == maxStageIconsNum && StageData.getClearCount(gameData.progressStageNo - 1) > 0)
				{
					gameData.progressStageNo++;
				}
			}
			return gameData.progressStageNo - 1;
		}

		public static int getTreasureNum()
		{
			return GlobalData.Instance.getGameData().treasureboxNum;
		}

		public static int getMailUnReadCount()
		{
			return GlobalData.Instance.getGameData().mailUnReadCount;
		}

		public static int getMinilenCount()
		{
			return GlobalData.Instance.getGameData().minilenCount;
		}

		public static int getMinilenTotalCount()
		{
			return GlobalData.Instance.getGameData().minilenTotalCount;
		}

		public static int getGiveNiceTotalCount()
		{
			return GlobalData.Instance.getGameData().giveNiceTotalCount;
		}

		public static int getGiveNiceMonthlyCount()
		{
			return GlobalData.Instance.getGameData().giveNiceMonthlyCount;
		}

		public static int getTookNiceTotalCount()
		{
			return GlobalData.Instance.getGameData().tookNiceTotalCount;
		}
	}
}
