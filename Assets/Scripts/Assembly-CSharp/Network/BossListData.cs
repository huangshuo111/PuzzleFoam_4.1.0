namespace Network
{
	public class BossListData
	{
		public class BossData
		{
			public BossLevelData[] bossLevelList;
		}

		public class BossLevelData
		{
			public int hp;

			public int level;

			public int playCount;

			public int status;

			public int clearCount;

			public int maxHp;

			public long lastPlayTime;

			public int[] usedItemType;
		}

		public BossData[] bossList;
	}
}
