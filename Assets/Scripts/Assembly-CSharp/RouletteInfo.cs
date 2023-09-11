public class RouletteInfo
{
	public class Reward
	{
		public int RewardType;

		public int Num;

		public int RewardRank;

		public float Ratio;
	}

	public class RewardInfo
	{
		public int Rank;

		public Reward[] Rewards;
	}

	public RewardInfo[] RewardInfos;
}
