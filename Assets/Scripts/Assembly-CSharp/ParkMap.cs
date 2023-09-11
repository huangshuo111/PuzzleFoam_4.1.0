using System;
using System.Collections;
using System.Collections.Generic;
using Bridge;
using Network;
using UnityEngine;

public class ParkMap : MonoBehaviour
{
	public const int PROCESS_RETURN_COUNT = 20;

	private const int MINILEN_COUNT_MIN = 1;

	private const int MINILEN_COUNT_MAX = 50;

	private const int MINILEN_COUNT_DENOMINATOR = 8;

	private const float MINILEN_ON_BUILDING_PROBABILITY = 40f;

	private const float MINILEN_INVERCE_PROBABILITY = 40f;

	private const int MINILEN_ON_BUILDING_ANIMATION_WAIT_MAX = 5;

	public long userID { get; set; }

	public bool isPlayerMap { get; set; }

	public IEnumerator setup(ParkObjectManager objectManager, ParkStructures.Size gridSize, ParkStructures.Size backgroundSize, int roadID, int areaReleasedCount, SaveParkData.PlacedData[] buildings, bool isDummy = false)
	{
		Vector2 startPosition = new Vector2(-backgroundSize.width / 2, backgroundSize.height / 2);
		ParkObjectManager.Instance.setRootPosition(startPosition);
		yield return StartCoroutine(objectManager.createBackground(-startPosition));
		yield return StartCoroutine(createGrids(objectManager, startPosition, objectManager.backgroundOffset.toVec2()));
		yield return StartCoroutine(objectManager.createReleaseArea());
		objectManager.setVisibleReleaseArea(areaReleasedCount);
		yield return StartCoroutine(createRoads(objectManager, roadID));
		yield return StartCoroutine(createBuilding(objectManager, buildings));
		yield return StartCoroutine(createMinilens(objectManager, buildings, isDummy));
		objectManager.layoutGrid.ForceUnvisible();
		ColliderManager.Instance.SortBySortingOrder();
	}

	private IEnumerator createGrids(ParkObjectManager objectManager, Vector2 startPosition, Vector2 backgroundOffset)
	{
		ParkUnplacableGridDataTable dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ParkUnplacableGridDataTable>();
		List<UnplacableGridInfo.UnplacableGrid> unplacableGridList = new List<UnplacableGridInfo.UnplacableGrid>(dataTable.getGrids());
		int width = objectManager.mapGridCount.width;
		int height = objectManager.mapGridCount.height;
		ParkStructures.Size gridSize = objectManager.gridSize;
		float backgroundHeight = objectManager.backgroundSize.height;
		Vector2 halfGridSize = gridSize.toVec2() * 0.5f;
		for (int i = 0; i < width; i++)
		{
			if (i % 100 == 0)
			{
				yield return null;
			}
			Vector3 position = new Vector3
			{
				x = startPosition.x + halfGridSize.x * (float)i + halfGridSize.x + backgroundOffset.x
			};
			float adjustY = halfGridSize.y * (float)((i > 0 && i % 2 == 1) ? (-1) : 0);
			position.y = startPosition.y + adjustY - backgroundHeight + backgroundOffset.y;
			for (int j = 0; j < height; j++)
			{
				int h = i;
				int v = height - j - 1;
				Grid grid = objectManager.createGrid(position);
				grid.horizontalIndex = i;
				grid.verticalIndex = height - j - 1;
				grid.offset = startPosition;
				position.y += gridSize.height;
				if (unplacableGridList.Exists((UnplacableGridInfo.UnplacableGrid g) => g.X == h && g.Y == v))
				{
					grid.isReleased = false;
				}
			}
		}
	}

	private IEnumerator createRoads(ParkObjectManager objectManager, int roadID)
	{
		ParkRoadPlacedDataTable dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ParkRoadPlacedDataTable>();
		ParkRoadPlacedInfo.RoadPlacedInfo[] roadPlacedList = dataTable.getAllInfo();
		int count = roadPlacedList.Length;
		for (int i = 0; i < count; i++)
		{
			if (i % 20 == 0)
			{
				yield return null;
			}
			Grid grid = objectManager.getGrid(roadPlacedList[i].X, roadPlacedList[i].Y);
			if (!grid.roadExistsOn)
			{
				Road road = objectManager.createRoad(roadID, roadPlacedList[i].SpriteID, roadPlacedList[i].IsReverse);
				road.horizontalIndex = roadPlacedList[i].X;
				road.verticalIndex = roadPlacedList[i].Y;
				grid.AttachObject(road, true);
				if (!grid.isReleased)
				{
					road.gameObject.SetActive(false);
				}
			}
		}
	}

	private IEnumerator createBuilding(ParkObjectManager objectManager, SaveParkData.PlacedData[] buildings)
	{
		if (buildings == null)
		{
			yield break;
		}
		int count = buildings.Length;
		ParkBuildingDataTable dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ParkBuildingDataTable>();
		for (int i = 0; i < count; i++)
		{
			if (i % 20 == 0)
			{
				yield return null;
			}
			if (buildings[i].x >= 0 && buildings[i].ID < 50000)
			{
				ParkBuildingInfo.BuildingInfo info = dataTable.getInfo(buildings[i].ID);
				Building building = null;
				switch ((ParkBuildingDataTable.eBuildingType)info.Type)
				{
				case ParkBuildingDataTable.eBuildingType.Fence:
					building = objectManager.createFence(buildings[i].ID);
					break;
				case ParkBuildingDataTable.eBuildingType.Building:
					building = objectManager.createBuilding(buildings[i].ID);
					break;
				case ParkBuildingDataTable.eBuildingType.TransitGate:
					building = objectManager.createTransitGate(buildings[i].ID);
					break;
				}
				building.horizontalIndex = buildings[i].x;
				building.verticalIndex = buildings[i].y;
				building.direction = (ParkObject.eDirection)buildings[i].direction;
				building.uniqueID = buildings[i].uniqueID;
				building.setRecalculatedOrder();
				objectManager.AddRelationGrid(building);
				objectManager.getGrid(building.horizontalIndex, building.verticalIndex).AttachObject(building, true);
				if (info.Type == 4)
				{
					building.sortingOrder = ParkObject.getOrder(building.horizontalIndex, building.verticalIndex);
				}
			}
		}
		objectManager.ConnectFences();
	}

	private IEnumerator createMinilens(ParkObjectManager objectManager, SaveParkData.PlacedData[] buildings, bool isDummy)
	{
		List<int> minilenIDList = new List<int>();
		AddMinilenIDs(buildings, ref minilenIDList, isDummy);
		List<Building> hasPivotBuildingList = objectManager.buildingList.FindAll((Building b) => b.hasMinilenPivots);
		int placedMinilenCount = 0;
		for (int j = 0; j < hasPivotBuildingList.Count && minilenIDList.Count > placedMinilenCount + hasPivotBuildingList[j].minilenPivotCount; j++)
		{
			if (Utility.decideByProbability(40f))
			{
				float animationWait = UnityEngine.Random.Range(0f, 5f) * 0.1f;
				for (int k = 0; k < hasPivotBuildingList[j].minilenPivotCount; k++)
				{
					Minilen minilen = objectManager.createMinilen(minilenIDList[placedMinilenCount + k]);
					hasPivotBuildingList[j].addMinilen(minilen, animationWait);
				}
				placedMinilenCount += hasPivotBuildingList[j].minilenPivotCount;
			}
		}
		minilenIDList.RemoveRange(0, placedMinilenCount);
		int roadCount = objectManager.roadCount;
		for (int i = 0; i < minilenIDList.Count; i++)
		{
			if (i % 20 == 0)
			{
				yield return null;
			}
			Road road = objectManager.getRoad(UnityEngine.Random.Range(0, roadCount));
			Grid grid = objectManager.getGrid(road.horizontalIndex, road.verticalIndex);
			if (grid.minilenExistsOn || !grid.roadExistsOn || !grid.isReleased)
			{
				i--;
				continue;
			}
			Minilen minilen2 = objectManager.createMinilen(minilenIDList[i]);
			minilen2.horizontalIndex = road.horizontalIndex;
			minilen2.verticalIndex = road.verticalIndex;
			minilen2.setRecalculatedOrder();
			if (Utility.decideByProbability(40f))
			{
				minilen2.direction = ParkObject.eDirection.Reverse;
			}
			grid.AttachObject(minilen2, true);
		}
	}

	private int getPlaceMinilenCount(ParkBuildingDataTable dataTable, SaveParkData.PlacedData[] buildings)
	{
		if (buildings == null)
		{
			return 1;
		}
		int num = 0;
		for (int i = 0; i < buildings.Length; i++)
		{
			if (buildings[i].x >= 0 && buildings[i].ID < 50000)
			{
				ParkBuildingInfo.BuildingInfo info = dataTable.getInfo(buildings[i].ID);
				num += info.GridWidth * info.GridHeight;
			}
		}
		int num2 = num / 8;
		num2 += ((8 * num2 + 1 <= num) ? 1 : 0);
		return Mathf.Clamp(num2, 1, 50);
	}

	private int findRareMinilen(SaveParkData.PlacedData placedData)
	{
		Network.MinilenData minilenData = Array.Find(Bridge.MinilenData.getMinielenData(), (Network.MinilenData m) => m.releaseBuildingID == placedData.ID);
		if (minilenData == null)
		{
			return -1;
		}
		return minilenData.index;
	}

	private void AddMinilenIDs(SaveParkData.PlacedData[] buildings, ref List<int> IDList, bool isDummy)
	{
		ParkBuildingDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ParkBuildingDataTable>();
		int index = Bridge.MinilenData.getMinielenData()[0].index;
		int placeMinilenCount = getPlaceMinilenCount(component, buildings);
		if (isDummy)
		{
			for (int i = 0; i < placeMinilenCount; i++)
			{
				IDList.Add(index);
			}
			return;
		}
		if (buildings != null)
		{
			int num = buildings.Length;
			for (int j = 0; j < num; j++)
			{
				if (buildings[j].x >= 0 && buildings[j].ID < 50000)
				{
					int num2 = findRareMinilen(buildings[j]);
					if (num2 == -1)
					{
						num2 = index;
					}
					IDList.Add(num2);
				}
			}
		}
		if (IDList.Count <= 0)
		{
			IDList.Add(index);
		}
		for (int k = 0; k < IDList.Count; k++)
		{
			int value = IDList[k];
			int index2 = UnityEngine.Random.Range(k, IDList.Count);
			IDList[k] = IDList[index2];
			IDList[index2] = value;
		}
		if (IDList.Count > placeMinilenCount)
		{
			IDList.RemoveRange(0, IDList.Count - placeMinilenCount);
		}
		int num3 = IDList.Count / 4;
		for (int l = 0; l < num3; l++)
		{
			IDList[l] = index;
		}
	}
}
