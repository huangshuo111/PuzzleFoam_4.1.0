using System;
using Network;

namespace Bridge
{
	public class MinilenData
	{
		public static void Setup()
		{
		}

		public static Network.MinilenData[] getMinielenData()
		{
			return GlobalData.Instance.getGameData().minilenList;
		}

		public static void Wear(int id)
		{
		}

		public static void setAvarable(int id)
		{
		}

		public static void setLevel(int id, int level)
		{
		}

		public static Network.MinilenData getCurrent()
		{
			Network.MinilenData minilenData = Array.Find(GlobalData.Instance.getGameData().minilenList, (Network.MinilenData m) => m.wearFlg > 0);
			if (minilenData == null)
			{
				minilenData = GlobalData.Instance.getGameData().minilenList[0];
			}
			return minilenData;
		}

		public static int Drop(int stage_id, out int drop_seed)
		{
			if (!Constant.ParkStage.isParkStage(stage_id))
			{
				drop_seed = 0;
				return -1;
			}
			drop_seed = 0;
			return -1;
		}
	}
}
