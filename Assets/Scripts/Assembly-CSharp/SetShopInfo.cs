public class SetShopInfo
{
	public class GoodsInfo
	{
		public class Reward
		{
			public int Type;

			public int Num;
		}

		public string ProductID;

		public bool IsHot;

		public int Num;

		public double Price;

		public double PriceDol;

		public double SetPrice;

		public double SetPriceDol;

		public int Buy;

		public int Bonus;

		public int Percent;

		public int SalePercent;

		public Reward[] Rewards;
	}

	public GoodsInfo[] iOSGoodsInfos;

	public GoodsInfo[] AndroidGoodsInfos;

	public GoodsInfo[] iOSSaleInfos;

	public GoodsInfo[] AndroidSaleInfos;
}
