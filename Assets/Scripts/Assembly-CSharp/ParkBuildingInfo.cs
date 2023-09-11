public class ParkBuildingInfo
{
	public class BuildingInfo
	{
		public int ID;

		public int SortID;

		public int NameID;

		public string SpriteName;

		public string PrefabName;

		public int Type;

		public int GridWidth;

		public int GridHeight;

		public int RewardType;

		public int RewardId;

		public int RewardNum;

		public int MinilenAction;

		public int MinilenDirection;

		public bool IsUserBuildable;

		public bool UseBuildingEffect;
	}

	public BuildingInfo[] BuildingInfos;
}
