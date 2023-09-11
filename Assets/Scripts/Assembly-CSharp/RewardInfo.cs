public class RewardInfo
{
	public class Reward
	{
		public int Type;

		public int Num;
	}

	public class ExpInfo
	{
		public int Lv;

		public int Bonus;

		public int Exp;

		public Reward[] Rewards;

		public int BubbleSpeed;

		public float RankingBonus;
	}

	public class Rate
	{
		public int Clear;

		public int Failed;

		public int AlreadyClear;

		public int AlreadyFailed;
	}

	public ExpInfo[] ExpInfos;

	public Rate ExpRate;

	public Rate CoinRate;

	public int[] CoinStarRates;

	public int LvCap;
}
