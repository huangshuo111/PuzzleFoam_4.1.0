namespace Network
{
	public class StageData
	{
		public enum eStatus
		{
			NotPlay = 0,
			Begin = 1,
			GameOver = 2,
			Clear = 3
		}

		public int playCount;

		public int clearCount;

		public int area;

		public int star;

		public int gameOverCount;

		public int hiscore;

		public int scoreSum;

		public int stageNo;

		public int stageStatus;

		public int[] usedItemType;

		public bool isOpen;
	}
}
