using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseArea : MonoBehaviour
{
	private bool isDrawing_;

	private int areaID_ = 1;

	private bool isReleased_;

	private Dictionary<ParkStructures.IntegerXY, Obstacle> areaObstacles_ = new Dictionary<ParkStructures.IntegerXY, Obstacle>();

	public int areaID
	{
		get
		{
			return areaID_;
		}
	}

	public static ReleaseArea createObject(Transform parent)
	{
		GameObject gameObject = new GameObject("ReleaseArea");
		gameObject.transform.SetParent(parent, false);
		return gameObject.AddComponent<ReleaseArea>();
	}

	public IEnumerator setup(ParkAreaReleaseInfo.AreaReleaseInfo releaseAreaData, bool isDrawing = false)
	{
		ParkObjectManager objectManager = ParkObjectManager.Instance;
		base.gameObject.name = "ReleaseArea_" + releaseAreaData.areaID.ToString("00");
		areaID_ = releaseAreaData.areaID;
		for (int i = 0; i < releaseAreaData.LockedGrids.Count; i++)
		{
			if (i % 20 == 0)
			{
				yield return null;
			}
			ParkAreaReleaseInfo.AreaReleaseInfo.LockedGrid lockedGrid = releaseAreaData.LockedGrids[i];
			Grid grid = objectManager.getGrid(lockedGrid.Index.x, lockedGrid.Index.y);
			if (!(grid != null))
			{
				continue;
			}
			Obstacle obstacle = null;
			if (lockedGrid.ObstructiveObjectID > 0)
			{
				obstacle = objectManager.createObstacle(lockedGrid.ObstructiveObjectID, base.transform);
				obstacle.horizontalIndex = lockedGrid.Index.x;
				obstacle.verticalIndex = lockedGrid.Index.y;
				obstacle.setRecalculatedOrder();
				obstacle.setPosition(grid.position);
				if (lockedGrid.IsReverse)
				{
					obstacle.direction = ParkObject.eDirection.Reverse;
				}
			}
			areaObstacles_.Add(lockedGrid.Index, obstacle);
			grid.isReleased = false;
		}
		isDrawing_ = isDrawing;
		setVisible(false);
	}

	public void setVisible(bool visible)
	{
		foreach (Obstacle value in areaObstacles_.Values)
		{
			if (value != null)
			{
				value.SetActive(visible);
			}
		}
	}

	public void ForceUnvisible()
	{
		ParkObjectManager instance = ParkObjectManager.Instance;
		foreach (KeyValuePair<ParkStructures.IntegerXY, Obstacle> item in areaObstacles_)
		{
			if (item.Value != null)
			{
				item.Value.SetActive(false);
			}
			instance.getGrid(item.Key.x, item.Key.y).isReleased = true;
		}
		isReleased_ = true;
		base.gameObject.SetActive(false);
	}

	public IEnumerator ReleaseLockedArea()
	{
		if (isReleased_)
		{
			yield break;
		}
		ParkStructures.IntegerXY baseIndex = new ParkStructures.IntegerXY(0, 0);
		List<ParkStructures.IntegerXY> indices = new List<ParkStructures.IntegerXY>(areaObstacles_.Keys);
		int min = 100;
		int max = 0;
		for (int i = 0; i < indices.Count; i++)
		{
			ParkStructures.IntegerXY index = indices[i];
			if (index.x % 2 == 1)
			{
				if (baseIndex.y < index.y + 1)
				{
					baseIndex = index;
				}
			}
			else if (baseIndex.y < index.y)
			{
				baseIndex = index;
			}
			if (max < index.x)
			{
				max = index.x;
			}
			if (min > index.x)
			{
				min = index.x;
			}
		}
		ParkObjectManager objectManager = ParkObjectManager.Instance;
		ParkStructures.Size size = new ParkStructures.Size((max - min) * objectManager.gridSize.width / 2, 0);
		yield return objectManager.StartCoroutine(objectManager.buildingEffects.PlaySequentialEffect(baseIndex, indices, size, true, OnFinishedSlideIn, OnFinishedSmoke));
	}

	public IEnumerator OnFinishedSlideIn()
	{
		if (Sound.Instance.se_clip.Length > 133)
		{
			Sound.Instance.playSe(Sound.eSe.SE_703_park_extend, false);
		}
		yield return new WaitForSeconds(19f / 30f);
		ParkObjectManager objectManager = ParkObjectManager.Instance;
		foreach (KeyValuePair<ParkStructures.IntegerXY, Obstacle> pair in areaObstacles_)
		{
			if (pair.Value != null)
			{
				StartCoroutine(pair.Value.deleteObstacle(0.2f));
			}
			objectManager.getGrid(pair.Key.x, pair.Key.y).isReleased = true;
		}
		yield return new WaitForSeconds(0.2f);
		isReleased_ = true;
		base.gameObject.SetActive(false);
	}

	public IEnumerator OnFinishedSmoke()
	{
		yield return new WaitForSeconds(0.3f);
		Sound.Instance.playSe(Sound.eSe.SE_113_Clap);
	}
}
