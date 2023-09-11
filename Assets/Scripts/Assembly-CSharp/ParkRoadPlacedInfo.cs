using System.Collections.Generic;

public class ParkRoadPlacedInfo
{
	public class RoadPlacedInfo
	{
		public enum eData
		{
			HorizontalIndex = 0,
			VerticalIndex = 1,
			AtlasSpriteID = 2
		}

		public bool IsReverse;

		public int SpriteID;

		public int X;

		public int Y;
	}

	public List<RoadPlacedInfo> RoadPlacedInfos = new List<RoadPlacedInfo>();
}
