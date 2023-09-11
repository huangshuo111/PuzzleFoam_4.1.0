public class BossStageInfo
{
	public enum eBossType
	{
		Invalid = -1,
		Owl = 0,
		Crab = 1,
		SkullDragon = 2,
		Spider = 3,
		Max = 4
	}

	public class Info
	{
		public BossData BossInfo;

		public int EntryTerms;

		public StageInfo.CommonInfo Common;
	}

	public class BossData
	{
		public int BossType;

		public int Active;

		public LevelInfo[] LevelInfos;
	}

	public class LevelInfo
	{
		public int Level;

		public int HitPoint;

		public int MoveSpeed;

		public int AttackSpan;

		public int LineDownSec;

		public int UseColorNum;

		public int RecreateBubbleSec;

		public int[] Rewards;

		public int[] RewardNums;
	}

	public string BeginDate;

	public string EndDate;

	public Info[] Infos;
}
