public class MinilenThanks
{
	public enum eType
	{
		Normal = 1,
		Random = 2
	}

	public enum eIncentive
	{
		Building = 1,
		ParkStageArea = 2,
		Coin = 3,
		Heart = 4,
		Juel = 5,
		PuzzleItem = 6,
		GachaTicket = 7,
		BossKey = 8,
		ParkMapArea = 9
	}

	public class MinilenThanksInfo
	{
		public int ID;

		public int Type;

		public int IncentiveType;

		public int IncentiveId;

		public int IncentiveNum;

		public int MinilenPrice;

		public int ConditionMinilenId;

		public int ConditionThanksId1;

		public int ConditionThanksId2;

		public int ConditionThanksId3;

		public int ConditionParkAreaId;

		public int ConditionTotalMinilens;
	}

	public MinilenThanksInfo[] MinilenThanksInfoList;
}
