using System.Collections;
using UnityEngine;

public class Grid : EventHandler
{
	private bool isReleased_ = true;

	private Road road_;

	private Building building_;

	private Minilen minilen_;

	private ParkStructures.Size objectSize_;

	private Rhombus collider_;

	protected int horizontalIndex_;

	protected int verticalIndex_;

	public bool isReleased
	{
		get
		{
			return isReleased_;
		}
		set
		{
			isReleased_ = value;
			CheckReleasedState();
		}
	}

	public bool roadExistsOn { get; private set; }

	public bool buildingExistsOn { get; private set; }

	public bool minilenExistsOn { get; private set; }

	public int horizontalIndex
	{
		get
		{
			return horizontalIndex_;
		}
		set
		{
			horizontalIndex_ = value;
		}
	}

	public int verticalIndex
	{
		get
		{
			return verticalIndex_;
		}
		set
		{
			verticalIndex_ = value;
		}
	}

	public ParkStructures.IntegerXY index
	{
		get
		{
			return new ParkStructures.IntegerXY(horizontalIndex_, verticalIndex_);
		}
	}

	public Vector3 position
	{
		get
		{
			return collider_.points[2] - offset;
		}
	}

	public Vector2 offset { get; set; }

	public ColliderBase colliderObject
	{
		get
		{
			return collider_;
		}
	}

	public bool isSameObject(Building building)
	{
		if (building_ == null)
		{
			return false;
		}
		return building_.isSameBuilding(building);
	}

	public bool isSameObject(Minilen minilen)
	{
		if (minilen_ == null)
		{
			return false;
		}
		return minilen_.isSame(minilen);
	}

	public IEnumerator setup(Vector3 positionm, ParkStructures.Size gridSize)
	{
		objectSize_ = gridSize;
		collider_ = base.gameObject.AddComponent<Rhombus>();
		collider_.worldSpace = true;
		collider_.center = position + new Vector3(0f, objectSize_.height / 2);
		collider_.extents = new Vector2(objectSize_.width / 2, objectSize_.height / 2);
		collider_.eventHandler = this;
		collider_.enabled = false;
		base.priority = 100;
		yield break;
	}

	public void setupImmediate(Vector3 position, ParkStructures.Size gridSize)
	{
		objectSize_ = gridSize;
		collider_ = base.gameObject.AddComponent<Rhombus>();
		collider_.worldSpace = true;
		collider_.center = position + new Vector3(0f, objectSize_.height / 2);
		collider_.extents = new Vector2(objectSize_.width / 2, objectSize_.height / 2);
		collider_.eventHandler = this;
		collider_.enabled = false;
		base.priority = 100;
	}

	public void Destroy()
	{
		if (collider_ != null)
		{
			ColliderManager.Instance.Remove(collider_);
			Object.Destroy(collider_);
			collider_ = null;
		}
	}

	public void AttachObject(Building building, bool positionRecalculate = false)
	{
		building_ = building;
		buildingExistsOn = true;
		building_.setRecalculatedOrder();
		if (positionRecalculate)
		{
			building_.setPosition(position);
		}
	}

	public void AttachObject(Road road, bool positionRecalculate = false)
	{
		road_ = road;
		roadExistsOn = true;
		road_.setRecalculatedOrder();
		if (positionRecalculate)
		{
			road_.setPosition(position);
		}
	}

	public void AttachObject(Minilen minilen, bool positionRecalculate = false)
	{
		minilen_ = minilen;
		minilenExistsOn = true;
		if (positionRecalculate)
		{
			minilen_.setPosition(position);
			minilen.setRecalculatedOrder();
		}
	}

	public void DetachObject(ParkObject.eType objectType)
	{
		switch (objectType)
		{
		case ParkObject.eType.Road:
			road_ = null;
			roadExistsOn = false;
			break;
		case ParkObject.eType.Fence:
		case ParkObject.eType.Building:
			building_ = null;
			buildingExistsOn = false;
			break;
		case ParkObject.eType.Minilen:
			minilen_ = null;
			minilenExistsOn = false;
			break;
		}
	}

	private void CheckReleasedState()
	{
		if (isReleased_)
		{
			if (road_ != null && !road_.gameObject.activeSelf)
			{
				road_.gameObject.SetActive(true);
			}
		}
		else if (road_ != null && road_.gameObject.activeSelf)
		{
			road_.gameObject.SetActive(false);
		}
	}

	public int getMinilenOrder()
	{
		if (buildingExistsOn)
		{
			return building_.sortingOrder - 60;
		}
		return 0;
	}
}
