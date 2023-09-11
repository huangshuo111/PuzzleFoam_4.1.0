public class ParkStageInfo
{
	public class Info
	{
		public int Area;

		public CommonInfo Common;
	}

	public class CommonInfo
	{
		public int StageNo;

		public int EntryStage;

		public int EntryMinilenThanks;

		public MinilenDrop[] MinilenDrops;

		public int Bg;

		public int Bgm;

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

		public StageInfo.ContinueInfo Continue;

		public int[] StarScores = new int[Constant.StarMax];

		public int ItemNum;

		public StageInfo.Item[] Items;

		public int StageItemNum;

		public StageInfo.Item[] StageItems;

		public int SpecialItemNum;

		public StageInfo.Item[] SpecialItems;

		public int FreeItemNum;

		public StageInfo.Item[] FreeItems;

		public float MinilenAllDropRate;

		public int MinilenOtherDropGroup;
	}

	public class MinilenDrop
	{
		public int MinilenDropId;

		public float MinilenDropRate;
	}

	public Info[] Infos;
}
