using System;
using System.Text;
using UnityEngine;

public class ParkBuildingDataTable : MonoBehaviour
{
	public enum eBuildingType
	{
		Building = 1,
		Fence = 2,
		Road = 3,
		TransitGate = 4,
		Max = 2
	}

	public enum eRewardType
	{
		Coin = 3,
		Heart = 4,
		Jewel = 5,
		Item = 6,
		GatshaTicket = 7,
		BossBattleKey = 8,
		Max = 7
	}

	public const string BUILDING_DATA_PATH = "building_data";

	private ParkBuildingInfo buildingInfo_;

	public void load()
	{
		if (buildingInfo_ == null)
		{
			byte[] bytes = Aes.DecryptFromFile(StageDataTable.getBuildingDataPath());
			Encoding encoding = Encoding.GetEncoding("UTF-8");
			string @string = encoding.GetString(bytes);
			buildingInfo_ = Xml.DeserializeObject<ParkBuildingInfo>(@string) as ParkBuildingInfo;
		}
	}

	public ParkBuildingInfo.BuildingInfo getInfo(int ID)
	{
		if (buildingInfo_ == null)
		{
			return null;
		}
		ParkBuildingInfo.BuildingInfo buildingInfo = Array.Find(buildingInfo_.BuildingInfos, (ParkBuildingInfo.BuildingInfo o) => o.ID == ID);
		if (buildingInfo == null)
		{
			Debug.LogWarning("Building Not Found !!! id = " + ID);
			buildingInfo = buildingInfo_.BuildingInfos[0];
		}
		return buildingInfo;
	}

	public ParkBuildingInfo.BuildingInfo[] getAllInfo()
	{
		if (buildingInfo_ == null)
		{
			return null;
		}
		return buildingInfo_.BuildingInfos;
	}

	public ParkBuildingInfo.BuildingInfo[] getAllBuildingInfo(bool isOnlyBuildable = true)
	{
		if (buildingInfo_ == null)
		{
			return null;
		}
		Predicate<ParkBuildingInfo.BuildingInfo> match = delegate(ParkBuildingInfo.BuildingInfo info)
		{
			bool flag = info.Type == 1 || info.Type == 2;
			return (!isOnlyBuildable || info.IsUserBuildable) && flag;
		};
		return Array.FindAll(buildingInfo_.BuildingInfos, match);
	}

	public ParkBuildingInfo.BuildingInfo[] getAllRoadInfo()
	{
		if (buildingInfo_ == null)
		{
			return null;
		}
		return Array.FindAll(buildingInfo_.BuildingInfos, (ParkBuildingInfo.BuildingInfo m) => m.Type == 3);
	}

	public ParkBuildingInfo.BuildingInfo[] getAllFenceInfo(bool isOnlyBuildable = true)
	{
		if (buildingInfo_ == null)
		{
			return null;
		}
		Predicate<ParkBuildingInfo.BuildingInfo> match = delegate(ParkBuildingInfo.BuildingInfo info)
		{
			bool flag = info.Type == 2;
			return (!isOnlyBuildable || info.IsUserBuildable) && flag;
		};
		return Array.FindAll(buildingInfo_.BuildingInfos, match);
	}
}
