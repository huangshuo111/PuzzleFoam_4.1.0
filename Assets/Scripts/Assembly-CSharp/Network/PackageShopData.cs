namespace Network
{
	public class PackageShopData
	{
		public class Ticket
		{
			public int id;

			public int num;
		}

		public class PackageInfo
		{
			public int price;

			public int coin;

			public int heart;

			public Ticket[] ticketInfos;
		}

		public int resultCode;

		public bool isPackageShopCampaign;

		public PackageInfo[] packageInfos;
	}
}
