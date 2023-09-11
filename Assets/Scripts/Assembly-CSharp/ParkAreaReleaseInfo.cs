using System.Collections.Generic;

public class ParkAreaReleaseInfo
{
	public class AreaReleaseInfo
	{
		public class LockedGrid
		{
			public ParkStructures.IntegerXY Index;

			public int ObstructiveObjectID;

			public bool IsReverse;
		}

		public string AreaName;

		public List<LockedGrid> LockedGrids;

		public int areaID
		{
			get
			{
				return int.Parse(AreaName.Replace("エリア", string.Empty));
			}
		}
	}

	public List<AreaReleaseInfo> ReleaseAreaInfos;
}
