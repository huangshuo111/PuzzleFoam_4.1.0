using System;
using System.Collections.Generic;
using LitJson;
using Network;
using UnityEngine;

public class SaveParkData
{
	public class MapSaveData
	{
		public int roadID;

		public int releasedAreaCount;

		public PlacedData[] buildings;
	}

	public class PlacedData
	{
		public int uniqueID;

		public int ID;

		public int direction;

		public int x;

		public int y;

		public bool isSameBuilding(int uniqueID, int ID)
		{
			return this.uniqueID == uniqueID && this.ID == ID;
		}

		public bool isSameBuilding(ParkStructures.IntegerXY index)
		{
			return x == index.x && y == index.y;
		}
	}

	public class DummyParkData
	{
		public int UseMinilenID;

		public int TotalMinilenCount;

		public int ReleaseAreaCount;

		public PlacedData[] PlacedDatas;

		public static DummyParkData parse(string text)
		{
			return Xml.DeserializeObject<DummyParkData>(text) as DummyParkData;
		}
	}

	private const string PARK_PREFS_KEY = "ParkMap.PlacedData";

	private const string DEFAULT_PLACED_DATA_PATH = "Parameter/default_placed_data";

	private MapSaveData mapSaveData_;

	private List<PlacedData> buildings_ = new List<PlacedData>();

	private DummyParkData dummyParkData_;

	public int roadID { get; set; }

	public int areaReleasedCount { get; set; }

	public List<PlacedData> buildings
	{
		get
		{
			return buildings_;
		}
	}

	public DummyParkData dummyParkData
	{
		get
		{
			return dummyParkData_;
		}
	}

	private MapSaveData createSaveData(ParkObjectManager objectManager)
	{
		MapSaveData mapSaveData = new MapSaveData();
		mapSaveData.roadID = roadID;
		mapSaveData.releasedAreaCount = areaReleasedCount;
		mapSaveData.buildings = buildings_.ToArray();
		return mapSaveData;
	}

	public void save()
	{
		MapSaveData mapSaveData = new MapSaveData();
		mapSaveData.roadID = roadID;
		mapSaveData.releasedAreaCount = areaReleasedCount;
		mapSaveData.buildings = buildings_.ToArray();
		string value = JsonMapper.ToJson(mapSaveData);
		PlayerPrefs.SetString("ParkMap.PlacedData", value);
		PlayerPrefs.Save();
	}

	public void load()
	{
		if (mapSaveData_ == null)
		{
			string @string = PlayerPrefs.GetString("ParkMap.PlacedData", string.Empty);
			if (string.IsNullOrEmpty(@string))
			{
				mapSaveData_ = getInitialData();
				string value = JsonMapper.ToJson(mapSaveData_);
				PlayerPrefs.SetString("ParkMap.PlacedData", value);
				PlayerPrefs.Save();
			}
			else
			{
				mapSaveData_ = JsonMapper.ToObject<MapSaveData>(@string);
			}
			roadID = mapSaveData_.roadID;
			areaReleasedCount = mapSaveData_.releasedAreaCount;
			buildings_.AddRange(mapSaveData_.buildings);
		}
	}

	public void delete()
	{
		PlayerPrefs.DeleteKey("ParkMap.PlacedData");
		PlayerPrefs.Save();
	}

	public void reset()
	{
		PlayerPrefs.SetString("ParkMap.PlacedData", JsonMapper.ToJson(getInitialData()));
		PlayerPrefs.Save();
	}

	public void AddDisplacedBuildingData(int uniqueID, int ID)
	{
		PlacedData placedData = new PlacedData();
		placedData.uniqueID = uniqueID;
		placedData.ID = ID;
		placedData.x = -1;
		placedData.y = -1;
		placedData.direction = 0;
		buildings_.Add(placedData);
	}

	public void UpdateBuildingData(Building building, ParkStructures.IntegerXY initialIndex)
	{
		int num = buildings_.FindIndex((PlacedData p) => p.isSameBuilding(building.uniqueID, building.objectID));
		if (num >= 0)
		{
			buildings_[num].uniqueID = building.uniqueID;
			buildings_[num].ID = building.objectID;
			buildings_[num].x = building.horizontalIndex;
			buildings_[num].y = building.verticalIndex;
			buildings_[num].direction = (int)building.direction;
		}
	}

	public void RemoveBuildingData(Building building)
	{
		int num = buildings_.FindIndex((PlacedData p) => p.isSameBuilding(building.uniqueID, building.objectID));
		if (num >= 0)
		{
			buildings_[num].x = -1;
			buildings_[num].y = -1;
		}
	}

	public void UpdatePlacedData(BuildingData[] buildingDatas)
	{
		if (buildingDatas != null)
		{
			buildings_.Clear();
			for (int i = 0; i < buildingDatas.Length; i++)
			{
				PlacedData placedData = new PlacedData();
				placedData.uniqueID = buildingDatas[i].uid;
				placedData.ID = buildingDatas[i].id;
				placedData.x = buildingDatas[i].x;
				placedData.y = buildingDatas[i].y;
				placedData.direction = buildingDatas[i].d;
				buildings_.Add(placedData);
			}
		}
	}

	public void getNetworkPostParameter(out ParkObjectManager.PostBuildingList parameter)
	{
		if (buildings_.Count == 0)
		{
			parameter = null;
		}
		parameter = new ParkObjectManager.PostBuildingList();
		parameter.buildings = new BuildingData[buildings_.Count];
		for (int i = 0; i < buildings_.Count; i++)
		{
			parameter.buildings[i] = new BuildingData();
			parameter.buildings[i].uid = buildings_[i].uniqueID;
			parameter.buildings[i].id = buildings_[i].ID;
			parameter.buildings[i].x = buildings_[i].x;
			parameter.buildings[i].y = buildings_[i].y;
			parameter.buildings[i].d = buildings_[i].direction;
		}
	}

	public void AddReleasedAreaCount()
	{
		areaReleasedCount++;
		ParkAreaReleaseDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ParkAreaReleaseDataTable>();
		if (component != null)
		{
			areaReleasedCount = Mathf.Clamp(areaReleasedCount, 0, component.dataCount);
		}
	}

	public void setDummyFriendData(string text)
	{
		if (dummyParkData_ == null)
		{
			if (string.IsNullOrEmpty(text))
			{
				TextAsset textAsset = Resources.Load<TextAsset>("Parameter/dummy_park_data");
				dummyParkData_ = DummyParkData.parse(textAsset.text);
			}
			else
			{
				dummyParkData_ = DummyParkData.parse(text);
			}
		}
	}

	public static MapSaveData getDummyData()
	{
		MapSaveData dummyData = new MapSaveData();
		dummyData.roadID = 50001;
		dummyData.buildings = new PlacedData[9];
		Action<int, int, int, ParkStructures.IntegerXY> action = delegate(int index, int id, int dir, ParkStructures.IntegerXY pos)
		{
			dummyData.buildings[index] = new PlacedData();
			dummyData.buildings[index].uniqueID = index;
			dummyData.buildings[index].ID = id;
			dummyData.buildings[index].direction = dir;
			dummyData.buildings[index].x = pos.x;
			dummyData.buildings[index].y = pos.y;
		};
		action(0, 49999, 0, new ParkStructures.IntegerXY(2, 23));
		action(1, 40000, 1, new ParkStructures.IntegerXY(10, 8));
		action(2, 40022, 1, new ParkStructures.IntegerXY(7, 10));
		action(3, 40002, 0, new ParkStructures.IntegerXY(5, 17));
		action(4, 40015, 0, new ParkStructures.IntegerXY(14, 14));
		action(5, 41007, 0, new ParkStructures.IntegerXY(11, 21));
		action(6, 41008, 0, new ParkStructures.IntegerXY(12, 19));
		action(7, 41008, 0, new ParkStructures.IntegerXY(14, 20));
		action(8, 50001, 0, new ParkStructures.IntegerXY(0, 0));
		return dummyData;
	}

	public static MapSaveData getDummyData2()
	{
		MapSaveData dummyData = new MapSaveData();
		dummyData.roadID = 50001;
		dummyData.buildings = new PlacedData[9];
		Action<int, int, int, ParkStructures.IntegerXY> action = delegate(int index, int id, int dir, ParkStructures.IntegerXY pos)
		{
			dummyData.buildings[index] = new PlacedData();
			dummyData.buildings[index].uniqueID = index;
			dummyData.buildings[index].ID = id;
			dummyData.buildings[index].direction = dir;
			dummyData.buildings[index].x = pos.x;
			dummyData.buildings[index].y = pos.y;
		};
		action(0, 49999, 0, new ParkStructures.IntegerXY(2, 23));
		action(1, 40000, 1, new ParkStructures.IntegerXY(5, 15));
		action(2, 40000, 1, new ParkStructures.IntegerXY(15, 13));
		action(3, 40001, 0, new ParkStructures.IntegerXY(11, 8));
		action(4, 40013, 0, new ParkStructures.IntegerXY(9, 11));
		action(5, 40020, 1, new ParkStructures.IntegerXY(1, 8));
		action(6, 41005, 0, new ParkStructures.IntegerXY(1, 10));
		action(7, 41005, 0, new ParkStructures.IntegerXY(2, 11));
		action(8, 50001, 0, new ParkStructures.IntegerXY(0, 0));
		return dummyData;
	}

	public static MapSaveData getInitialData()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Parameter/default_placed_data");
		return JsonMapper.ToObject<MapSaveData>(textAsset.text);
	}
}
