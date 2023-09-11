public class GimmickPercentInfo
{
	public class BlackHoleInfo
	{
		public int MinusPercent;

		public int PlusPercent;

		public MinusInfo[] MinusTable;

		public PlusInfo[] PlusTable;
	}

	public class MinusInfo
	{
		public int MinusType;

		public int MinusTypePercent;
	}

	public class PlusInfo
	{
		public int PlusType;

		public int PlusTypePercent;
	}

	public BlackHoleInfo[] BlackHole;
}
