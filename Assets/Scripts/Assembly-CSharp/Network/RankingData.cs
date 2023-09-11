using System;

namespace Network
{
	public class RankingData
	{
		public int resultCode;

		public string lastWeekStarRankingStartDate;

		public string lastWeekStarRankingStartDate2;

		public int lastWeekStarRankingRemainingDD;

		public int lastWeekStarRankingRemainingHH;

		public int lastWeekStarRankingRemainingMM;

		public RankingStarData[] starList;

		public RankingStageScoreData[] rankingStageScoreList = new RankingStageScoreData[0];

		public DateTime getRankingStartDate()
		{
			string format = "yyyy-MM-dd HH:mm";
			return DateTime.ParseExact(lastWeekStarRankingStartDate, format, null);
		}
	}
}
