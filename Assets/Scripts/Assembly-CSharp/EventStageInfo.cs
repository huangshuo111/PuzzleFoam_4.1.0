public class EventStageInfo
{
	public enum eLevel
	{
		Easy = 0,
		Normal = 1,
		Hard = 2,
		Max = 10
	}

	public class TimeTable
	{
		public class TimeInfo
		{
			public int ChallengeStartHour;

			public int ChallengeEndHour;
		}

		public TimeInfo[] TimeInfos;
	}

	public class Info
	{
		public int Level;

		public int[] Rewards = new int[3];

		public int[] RewardNums = new int[3];

		public int EntryTerms;

		public StageInfo.CommonInfo Common;
	}

	public string BeginDate;

	public string EndDate;

	public int EventNo;

	public TimeTable TimeDetail;

	public Info[] Infos;
}
