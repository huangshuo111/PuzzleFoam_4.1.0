public class JewelShopInfo
{
	public class GoodsInfo
	{
		public string ProductID;

		public bool IsHot;

		public int Num;

		public double Price;

		public double PriceDol;

		public int Buy;

		public int Bonus;

		public int Percent;
	}

	public GoodsInfo[] iOSGoodsInfos;

	public GoodsInfo[] AndroidGoodsInfos;

	public GoodsInfo[] iOSLuckyChanceInfos;

	public GoodsInfo[] AndroidLuckyChanceInfos;
}
