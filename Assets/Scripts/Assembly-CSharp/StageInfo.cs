public class StageInfo
{
	public class Info
	{
		public int Area;

		public CommonInfo Common;
	}

	public class CommonInfo
	{
		public int StageNo;

		public int EntryStars;

		public int UnlockPrice;

		public int Bg;

		public int LoopLine;

		public int Exp;

		public int Coin;

		public int StarRewardType;

		public int StarRewardNum;

		public int Move;

		public int Score;

		public int Time;

		public bool IsAllDelete;

		public bool IsFriendDelete;

		public bool IsFulcrumDelete;

		public bool IsMinilenDelete;

		public ContinueInfo Continue;

		public int[] StarScores = new int[Constant.StarMax];

		public int ItemNum;

		public Item[] Items;

		public int StageItemNum;

		public Item[] StageItems;

		public int SpecialItemNum;

		public Item[] SpecialItems;

		public int FreeItemNum;

		public Item[] FreeItems;
	}

	public class ContinueInfo
	{
		public int Recovary;

		public int PriceType;

		public int Price;

		public int ReplayPriceType;

		public int ReplayPrice;
	}

	public class Item
	{
		public int Type;

		public int Num;

		public int PriceType;

		public int Price;
	}

	public Info[] Infos;
}
