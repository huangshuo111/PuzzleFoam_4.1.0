public class RankingStageInfo
{
	public class TimeInfo
	{
		public float Sec;

		public int Line;

		public int Color;

		public int ExtraBubblesRate;

		public int ExtraBubbleNum;

		public ExtraBubble[] ExtraBubbles;
	}

	public class ExtraBubble
	{
		public int Type;

		public float Rate;
	}

	public string BeginDate;

	public string EndDate;

	public int Area;

	public StageInfo.CommonInfo Common;

	public int TimeInfoNum;

	public TimeInfo[] TimeInfos;
}
