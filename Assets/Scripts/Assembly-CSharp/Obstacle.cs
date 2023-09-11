using System.Collections;
using UnityEngine;

public class Obstacle : ParkObject
{
	private bool isSetupFinished_;

	public override IEnumerator setup(int id)
	{
		yield return StartCoroutine(base.setup(id));
		objectID_ = id;
		objectType_ = eType.Obstacle;
		ParkObjectManager objectManager = ParkObjectManager.Instance;
		ParkBuildingDataTable dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ParkBuildingDataTable>();
		ParkBuildingInfo.BuildingInfo info = dataTable.getInfo(id);
		gridSize_ = new ParkStructures.Size(info.GridWidth, info.GridHeight);
		collider_ = GetComponentInChildren<ColliderBase>();
		if (collider_ != null)
		{
			ColliderManager.Instance.Remove(collider_);
			Object.Destroy(collider_);
		}
		isSetupFinished_ = true;
	}

	public override void setupImmediate(int id)
	{
		base.setupImmediate(id);
		objectID_ = id;
		objectType_ = eType.Obstacle;
		ParkObjectManager instance = ParkObjectManager.Instance;
		ParkBuildingDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ParkBuildingDataTable>();
		ParkBuildingInfo.BuildingInfo info = component.getInfo(id);
		gridSize_ = new ParkStructures.Size(info.GridWidth, info.GridHeight);
		collider_ = GetComponentInChildren<ColliderBase>();
		if (collider_ != null)
		{
			ColliderManager.Instance.Remove(collider_);
			Object.Destroy(collider_);
		}
		isSetupFinished_ = true;
	}

	protected override void setObjectDirection(eDirection newDirection)
	{
		if (newDirection != direction_)
		{
			ParkObjectManager instance = ParkObjectManager.Instance;
			switch (newDirection)
			{
			case eDirection.Default:
				cachedTransform_.localScale = new Vector3(1f, 1f, 1f);
				break;
			case eDirection.Reverse:
				cachedTransform_.localScale = new Vector3(-1f, 1f, 1f);
				break;
			}
			direction_ = newDirection;
		}
	}

	public void SetActive(bool active)
	{
		base.gameObject.SetActive(active);
	}

	public IEnumerator deleteObstacle(float scaleTime)
	{
		cachedTransform_.localScale = Vector3.one;
		setAlpha(1f);
		float time = 0f;
		while (time < scaleTime)
		{
			float scale = 1f - time / scaleTime;
			setAlpha(scale);
			time += Time.deltaTime;
			yield return null;
		}
		cachedTransform_.localScale = Vector3.zero;
		SetActive(false);
	}
}
