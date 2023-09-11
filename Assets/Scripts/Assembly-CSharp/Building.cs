using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : ParkObject
{
	private const int MULTISPRITE_ORDER_RANGE = 20;

	private const float BRING_TIMESPAN = 1.5f;

	private const float BRING_MINIMUM_ALPHA = 0.5f;

	private const float BRING_MAXIMUM_ALPHA = 0.8f;

	private const float DRAGGING_HEIGHT_UP = 4f;

	private static readonly Color UNPLACABLE_COLOR = new Color(0.781f, 0f, 0f);

	protected int uniqueID_;

	protected bool isSetupFinished_;

	protected bool isDragMove_;

	protected bool canReplace_;

	protected Vector3 prevPosition_ = Vector3.zero;

	protected Vector2 colliderOffset_ = Vector2.zero;

	protected bool useMultipleSprites_;

	protected ParkStructures.Size originalColliderSize_ = new ParkStructures.Size(0, 0);

	protected List<Transform> minilenPivots_ = new List<Transform>();

	protected List<Minilen> inPivotMinilens_ = new List<Minilen>();

	public int uniqueID
	{
		get
		{
			return uniqueID_;
		}
		set
		{
			uniqueID_ = value;
		}
	}

	public bool canReplace
	{
		get
		{
			return canReplace_;
		}
		set
		{
			canReplace_ = value;
		}
	}

	public bool useMultipleSprites
	{
		get
		{
			return useMultipleSprites_;
		}
	}

	public bool hasMinilenPivots { get; private set; }

	public int minilenPivotCount
	{
		get
		{
			return minilenPivots_.Count;
		}
	}

	public bool isSelected { get; private set; }

	public Vector3 objectCenter
	{
		get
		{
			Vector3 vector = new Vector3(position.x + colliderOffset_.x, position.y + colliderOffset_.y);
			if (base.isSquare)
			{
				return vector;
			}
			if (direction_ == eDirection.Default)
			{
				return vector + collider_.transform.localPosition;
			}
			return vector - collider_.transform.localPosition;
		}
	}

	public override bool isSame(ParkObject parkObject)
	{
		if (base.isSame(parkObject) && objectID_ == parkObject.objectID)
		{
			return true;
		}
		return false;
	}

	public bool isSameBuilding(Building other)
	{
		if (objectID_ == other.objectID && uniqueID_ == other.uniqueID)
		{
			return true;
		}
		return false;
	}

	public override IEnumerator setup(int id)
	{
		yield return StartCoroutine(base.setup(id));
		objectID_ = id;
		objectType_ = eType.Building;
		ParkObjectManager objectManager = ParkObjectManager.Instance;
		ParkBuildingDataTable dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ParkBuildingDataTable>();
		ParkBuildingInfo.BuildingInfo info = dataTable.getInfo(id);
		gridSize_ = new ParkStructures.Size(info.GridWidth, info.GridHeight);
		collider_ = base.gameObject.GetComponent<ColliderBase>();
		collider_.eventHandler = this;
		colliderOffset_ = getColliderOffset();
		originalColliderSize_ = getColliderSize();
		base.spriteSize = getColliderSize();
		base.enableOnPress = true;
		base.enableOnRelease = true;
		base.enableOnDragStart = true;
		base.enableOnDrag = true;
		base.enableOnDragEnd = true;
		checkMultipleSprites();
		isSetupFinished_ = true;
	}

	public override void setupImmediate(int id)
	{
		base.setupImmediate(id);
		objectID_ = id;
		objectType_ = eType.Building;
		ParkObjectManager instance = ParkObjectManager.Instance;
		ParkBuildingDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ParkBuildingDataTable>();
		ParkBuildingInfo.BuildingInfo info = component.getInfo(id);
		gridSize_ = new ParkStructures.Size(info.GridWidth, info.GridHeight);
		collider_ = base.gameObject.GetComponentInChildren<ColliderBase>();
		collider_.eventHandler = this;
		colliderOffset_ = getColliderOffset();
		originalColliderSize_ = getColliderSize();
		base.spriteSize = getColliderSize();
		base.enableOnPress = true;
		base.enableOnRelease = true;
		base.enableOnDragStart = true;
		base.enableOnDrag = true;
		base.enableOnDragEnd = true;
		checkMultipleSprites();
		isSetupFinished_ = true;
	}

	private void Drag(Vector3 inputPosition, Vector3 delta)
	{
		if (Input.touchCount >= 2)
		{
			prevPosition_ = prevPosition_;
			return;
		}
		ParkObjectManager instance = ParkObjectManager.Instance;
		instance.ScrollMapCameraByObjectDragging();
		Vector3 vector = instance.mapCamera.ScreenToWorldPoint(inputPosition);
		Grid candidateGrid = null;
		List<ParkStructures.IntegerXY> list = new List<ParkStructures.IntegerXY>();
		ParkObjectManager.ePlantCheckResult ePlantCheckResult = instance.canPlantBuilding(vector, this, ref candidateGrid);
		if (candidateGrid != null)
		{
			if (ePlantCheckResult == ParkObjectManager.ePlantCheckResult.Succeeded)
			{
				OnSucceedToReplace(instance, candidateGrid);
			}
			else
			{
				OnFailedToReplace(instance, candidateGrid, ePlantCheckResult);
			}
		}
	}

	protected void OnSucceedToReplace(ParkObjectManager objectManager, Grid candidateGrid)
	{
		if (base.index != candidateGrid.index)
		{
			horizontalIndex_ = candidateGrid.horizontalIndex;
			verticalIndex_ = candidateGrid.verticalIndex;
			base.sortingOrder = 25000;
			priority_ = 0;
			setPosition(candidateGrid.position + new Vector3(0f, 4f));
			objectManager.layoutGrid.setPosition(candidateGrid.position);
			objectManager.layoutGrid.setColor(true);
			setColor(Color.white);
			OnMoveGrid();
		}
		canReplace_ = true;
	}

	protected void OnFailedToReplace(ParkObjectManager objectManager, Grid candidateGrid, ParkObjectManager.ePlantCheckResult result)
	{
		if (base.index != candidateGrid.index)
		{
			horizontalIndex_ = candidateGrid.horizontalIndex;
			verticalIndex_ = candidateGrid.verticalIndex;
			base.sortingOrder = 25000;
			priority_ = 0;
			objectManager.layoutGrid.setPosition(candidateGrid.position);
			setPosition(candidateGrid.position + new Vector3(0f, 4f));
			switch (result)
			{
			case ParkObjectManager.ePlantCheckResult.GridNotYetReleased:
			case ParkObjectManager.ePlantCheckResult.GridShortage:
			case ParkObjectManager.ePlantCheckResult.ExistsObject:
				objectManager.layoutGrid.setUnplaceableGrid(this);
				setColor(UNPLACABLE_COLOR);
				canReplace_ = false;
				break;
			}
			OnMoveGrid();
		}
	}

	protected override void setObjectDirection(eDirection newDirection)
	{
		if (newDirection == direction_)
		{
			return;
		}
		ParkObjectManager instance = ParkObjectManager.Instance;
		switch (newDirection)
		{
		case eDirection.Default:
			cachedTransform_.localScale = new Vector3(1f, 1f, 1f);
			if (!base.isSquare)
			{
				instance.layoutGrid.setVisible(gridSize_);
			}
			break;
		case eDirection.Reverse:
			cachedTransform_.localScale = new Vector3(-1f, 1f, 1f);
			if (!base.isSquare)
			{
				instance.layoutGrid.setVisible(new ParkStructures.Size(gridSize_.height, gridSize_.width));
			}
			break;
		}
		direction_ = newDirection;
		if (!base.isSquare)
		{
			if (instance.canPlantBuilding(this) == ParkObjectManager.ePlantCheckResult.Succeeded)
			{
				instance.layoutGrid.setColor(true);
				setColor(Color.white);
				canReplace_ = true;
			}
			else
			{
				instance.layoutGrid.setUnplaceableGrid(this);
				setColor(UNPLACABLE_COLOR);
				canReplace_ = false;
			}
		}
		setColliderOffset();
		if (hasMinilenPivots)
		{
			for (int i = 0; i < inPivotMinilens_.Count; i++)
			{
				inPivotMinilens_[i].RepositionEffect();
			}
		}
	}

	protected override void setOrder(int order)
	{
		base.setOrder(order);
		if (hasMinilenPivots)
		{
			for (int i = 0; i < inPivotMinilens_.Count; i++)
			{
				Minilen minilen = inPivotMinilens_[i];
				minilen.horizontalIndex = horizontalIndex_;
				minilen.verticalIndex = verticalIndex_;
				minilen.sortingOrder = order + i * 20 + 1;
			}
		}
	}

	protected virtual void setColliderOffset()
	{
		switch (collider_.colliderType)
		{
		case ColliderBase.eColliderType.Circle:
		{
			Circle circle = collider_ as Circle;
			circle.center = new Vector2(circle.center.x * -1f, circle.center.y);
			colliderOffset_.x *= -1f;
			break;
		}
		case ColliderBase.eColliderType.Square:
		{
			Square square = collider_ as Square;
			square.center = new Vector2(square.center.x * -1f, square.center.y);
			colliderOffset_.x *= -1f;
			break;
		}
		}
	}

	protected Vector2 getColliderOffset()
	{
		Vector2 result = Vector2.zero;
		switch (collider_.colliderType)
		{
		case ColliderBase.eColliderType.Circle:
		{
			Circle circle = collider_ as Circle;
			result = circle.center;
			break;
		}
		case ColliderBase.eColliderType.Square:
		{
			Square square = collider_ as Square;
			result = square.center;
			break;
		}
		}
		return result;
	}

	public ParkStructures.Size getColliderSize()
	{
		ParkStructures.Size result = new ParkStructures.Size(0, 0);
		switch (collider_.colliderType)
		{
		case ColliderBase.eColliderType.Circle:
		{
			Circle circle = collider_ as Circle;
			result = new ParkStructures.Size(circle.radius, circle.radius);
			break;
		}
		case ColliderBase.eColliderType.Square:
		{
			Square square = collider_ as Square;
			result = new ParkStructures.Size(square.size.x, square.size.y);
			break;
		}
		}
		return result;
	}

	public void setDraggableCollider()
	{
		ParkStructures.Size size = ParkObjectManager.Instance.gridSize;
		switch (collider_.colliderType)
		{
		case ColliderBase.eColliderType.Circle:
		{
			Circle circle = collider_ as Circle;
			circle.radius = Mathf.Max(originalColliderSize_.width, size.width);
			break;
		}
		case ColliderBase.eColliderType.Square:
		{
			Square square = collider_ as Square;
			square.size = new Vector2(Mathf.Max(originalColliderSize_.width, size.width), originalColliderSize_.height);
			square.size *= 2f;
			break;
		}
		}
	}

	public void returnOriginalCollider()
	{
		switch (collider_.colliderType)
		{
		case ColliderBase.eColliderType.Circle:
		{
			Circle circle = collider_ as Circle;
			circle.center = colliderOffset_;
			circle.radius = originalColliderSize_.width;
			break;
		}
		case ColliderBase.eColliderType.Square:
		{
			Square square = collider_ as Square;
			square.center = colliderOffset_;
			square.size = originalColliderSize_.toVec2();
			break;
		}
		}
	}

	public virtual void OnSelect()
	{
		ParkObjectManager instance = ParkObjectManager.Instance;
		setDraggableCollider();
		StartCoroutine(Bring(1.5f, 0.5f, 0.8f));
		StartDragMoving();
		instance.mapScroll.StartAutoScroll();
		instance.layoutGrid.setColor(true);
		instance.selectedObject = this;
		base.sortingOrder = 25000;
		priority_ = 0;
		ColliderManager.Instance.SortBySortingOrder();
		isSelected = true;
	}

	public virtual void OnDeselect(bool resetColor = true)
	{
		if (resetColor)
		{
			setColor(Color.white);
			setAlpha(1f);
			StopAllCoroutines();
		}
		ParkObjectManager instance = ParkObjectManager.Instance;
		instance.mapScroll.EndAutoScroll();
		instance.mapScroll.enableScroll = true;
		instance.selectedObject = null;
		returnOriginalCollider();
		instance.AddRelationGrid(this);
		setRecalculatedOrder();
		ColliderManager.Instance.SortBySortingOrder();
		isSelected = false;
	}

	public virtual void OnMoveGrid()
	{
		if (hasMinilenPivots)
		{
			for (int i = 0; i < inPivotMinilens_.Count; i++)
			{
				inPivotMinilens_[i].RepositionEffect();
			}
		}
	}

	public override void OnRemove()
	{
		base.OnRemove();
		ParkObjectManager instance = ParkObjectManager.Instance;
		if (hasMinilenPivots)
		{
			for (int i = 0; i < inPivotMinilens_.Count; i++)
			{
				instance.Remove(inPivotMinilens_[i]);
				inPivotMinilens_[i].OnRemove();
				Object.Destroy(inPivotMinilens_[i].gameObject);
			}
		}
		inPivotMinilens_.Clear();
	}

	public override void OnPress(Vector3 inputPosition)
	{
		if (!isSelected)
		{
			ParkObjectManager.Instance.StartWatchingLongPress(this);
		}
	}

	public override void OnRelease(Vector3 inputPosition)
	{
		if (!isSelected)
		{
			ParkObjectManager.Instance.EndWatchingLongPress();
		}
	}

	public override void OnDragStart(Vector3 inputPosition)
	{
		if (!isSelected)
		{
			ParkObjectManager.Instance.EndWatchingLongPress(false);
		}
	}

	public override void OnDragEnd(Vector3 inputPosition)
	{
		ParkObjectManager.Instance.mapScroll.enableScroll = true;
	}

	public override void OnDrag(Vector3 inputPosition, Vector3 delta)
	{
		if (isDragMove_)
		{
			ParkObjectManager.Instance.mapScroll.enableScroll = false;
			Drag(inputPosition, delta);
		}
	}

	public void StartDragMoving()
	{
		ParkObjectManager instance = ParkObjectManager.Instance;
		isDragMove_ = true;
		canReplace_ = true;
		Grid grid = instance.getGrid(horizontalIndex_, verticalIndex_);
		LayoutGrid layoutGrid = instance.layoutGrid;
		layoutGrid.setPosition(grid.position);
		if (direction_ == eDirection.Reverse)
		{
			layoutGrid.setVisible(new ParkStructures.Size(gridSize_.height, gridSize_.width));
		}
		else
		{
			layoutGrid.setVisible(gridSize_);
		}
		layoutGrid.setColor(true);
		setPosition(grid.position + new Vector3(0f, 4f));
	}

	public void EndDragMoving()
	{
		ParkObjectManager instance = ParkObjectManager.Instance;
		isDragMove_ = false;
		canReplace_ = true;
		Grid grid = instance.getGrid(horizontalIndex_, verticalIndex_);
		setPosition(grid.position);
		instance.layoutGrid.ForceUnvisible();
		OnMoveGrid();
		prevPosition_ = Vector3.zero;
	}

	public IEnumerator fadeInAlpha(float waitTime = 2f, float fadeTime = 0.45f)
	{
		float minAlpha = base.alpha;
		if (waitTime > 0f)
		{
			yield return new WaitForSeconds(waitTime);
		}
		float time = 0f;
		while (time < fadeTime)
		{
			float nextAlpha = minAlpha + time / fadeTime;
			setAlpha(Mathf.Clamp01(nextAlpha));
			if (nextAlpha >= 1f)
			{
				break;
			}
			time += Time.deltaTime;
			yield return null;
		}
		setAlpha(1f);
	}

	public IEnumerator fadeOutAlpha(float waitTime = 2f, float fadeTime = 0.45f)
	{
		setAlpha(1f);
		if (waitTime > 0f)
		{
			yield return new WaitForSeconds(waitTime);
		}
		float time = 0f;
		while (time < fadeTime)
		{
			setAlpha(1f - time / fadeTime);
			time += Time.deltaTime;
			yield return null;
		}
		setAlpha(0f);
	}

	private void checkMultipleSprites()
	{
		hasMinilenPivots = false;
		int num = spriteRenderers_.Length;
		if (num > 1)
		{
			useMultipleSprites_ = true;
		}
		if (cachedTransform_.childCount <= 0)
		{
			return;
		}
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].name.Contains("Pivot"))
			{
				minilenPivots_.Add(componentsInChildren[i]);
				hasMinilenPivots = true;
			}
		}
	}

	public void setMultipleSprites(Sprite[] sprites)
	{
		if (spriteRenderers_ == null)
		{
			return;
		}
		int num = sprites.Length;
		if (num <= spriteRenderers_.Length)
		{
			for (int i = 0; i < num; i++)
			{
				spriteRenderers_[i].sprite = sprites[i];
			}
		}
	}

	public void addMinilen(Minilen minilen, float animationChangeWait = 0f)
	{
		if (!hasMinilenPivots)
		{
			return;
		}
		for (int i = 0; i < minilenPivots_.Count; i++)
		{
			if (minilenPivots_[i].transform.childCount == 0)
			{
				minilen.cachedTransform.SetParent(minilenPivots_[i], false);
				minilen.horizontalIndex = horizontalIndex_;
				minilen.verticalIndex = verticalIndex_;
				minilen.sortingOrder = base.sortingOrder + i * 20 + 1;
				ParkBuildingDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ParkBuildingDataTable>();
				ParkBuildingInfo.BuildingInfo info = component.getInfo(base.objectID);
				minilen.canAction = false;
				if (info.MinilenAction >= 0)
				{
					StartCoroutine(minilen.PlayAnimationWaitTime((Minilen.eAnimationState)info.MinilenAction, animationChangeWait, true));
				}
				if (info.MinilenDirection >= 0)
				{
					eDirection minilenDirection = (eDirection)info.MinilenDirection;
					minilen.direction = minilenDirection;
				}
				inPivotMinilens_.Add(minilen);
				break;
			}
		}
	}
}
