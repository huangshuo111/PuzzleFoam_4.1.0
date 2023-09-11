public class DummyFriendInfo
{
	public enum eDummyFriendType
	{
		Bubblen = 1,
		Bobblen = 2,
		Chucken = 3
	}

	public class Info
	{
		public int Type;

		public int NameMsgID;

		public int StarSum;

		public int StageScore;

		public int RankingStageScore;
	}

	public Info[] Infos;
}
