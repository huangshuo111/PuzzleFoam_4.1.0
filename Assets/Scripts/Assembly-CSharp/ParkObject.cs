using System.Collections;
using UnityEngine;

public class ParkObject : EventHandler
{
	public enum eType
	{
		Road = 0,
		Fence = 1,
		Building = 2,
		Minilen = 3,
		Effect = 4,
		Obstacle = 5,
		Max = 6
	}

	public enum eDirection
	{
		Default = 0,
		Reverse = 1
	}

	public enum eGridPoints
	{
		Left = 0,
		Right = 1,
		Top = 2,
		Bottom = 3
	}

	public const int SORTING_ORDER_RANGE = 150;

	public const int SORTING_ORDER_HALF_RANGE = 75;

	public const int SORTING_ORDER_MAX = 25000;

	public const int BACKGROUND_SORTING_ORDER_MAX = 150;

	protected int objectID_;

	protected eType objectType_;

	protected ParkStructures.Size gridSize_;

	protected int horizontalIndex_;

	protected int verticalIndex_;

	protected Transform cachedTransform_;

	protected ColliderBase collider_;

	protected eDirection direction_;

	protected int sortingOrder_;

	protected SpriteRenderer[] spriteRenderers_;

	protected int[] originalOrder_;

	public int objectID
	{
		get
		{
			return objectID_;
		}
	}

	public eType objectType
	{
		get
		{
			return objectType_;
		}
	}

	public ParkStructures.Size gridSize
	{
		get
		{
			return gridSize_;
		}
		set
		{
			gridSize_ = value;
		}
	}

	public Transform cachedTransform
	{
		get
		{
			if (cachedTransform_ == null)
			{
				cachedTransform_ = base.transform;
			}
			return cachedTransform_;
		}
	}

	public ColliderBase colliderObject
	{
		get
		{
			return collider_;
		}
	}

	public virtual Vector3 position
	{
		get
		{
			return cachedTransform.localPosition;
		}
	}

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

	public int sortingOrder
	{
		get
		{
			return sortingOrder_;
		}
		set
		{
			setOrder(value);
		}
	}

	public eDirection direction
	{
		get
		{
			return direction_;
		}
		set
		{
			setObjectDirection(value);
			direction_ = value;
		}
	}

	public bool isSquare
	{
		get
		{
			return gridSize_.height == gridSize_.width;
		}
	}

	public ParkStructures.Size spriteSize { get; set; }

	public int spriteCount
	{
		get
		{
			if (spriteRenderers_ == null)
			{
				return 0;
			}
			return spriteRenderers_.Length;
		}
	}

	public float alpha { get; private set; }

	public int longerEdge
	{
		get
		{
			return (gridSize_.width <= gridSize_.height) ? gridSize_.height : gridSize_.width;
		}
	}

	public ParkStructures.IntegerXY normalizeIndex
	{
		get
		{
			int num = verticalIndex_;
			if (horizontalIndex_ % 2 == 1)
			{
				num++;
			}
			return new ParkStructures.IntegerXY(horizontalIndex_, num);
		}
	}

	private void Awake()
	{
		cachedTransform_ = base.transform;
		gridSize_ = new ParkStructures.Size(1, 1);
		alpha = 1f;
	}

	public virtual IEnumerator setup(int id)
	{
		findRenderer();
		yield break;
	}

	public virtual void setupImmediate(int id)
	{
		findRenderer();
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

	public virtual void OnRemove()
	{
		if (collider_ != null)
		{
			ColliderManager.Instance.Remove(collider_);
			Object.Destroy(collider_);
			collider_ = null;
		}
	}

	public virtual bool isSame(ParkObject parkObject)
	{
		if (objectType_ != parkObject.objectType)
		{
			return false;
		}
		return index == parkObject.index;
	}

	public void findRenderer()
	{
		spriteRenderers_ = GetComponentsInChildren<SpriteRenderer>(false);
		if (spriteRenderers_ != null)
		{
			int num = spriteRenderers_.Length;
			originalOrder_ = new int[num];
			for (int i = 0; i < num; i++)
			{
				spriteRenderers_[i].sortingLayerName = "ParkMap";
				originalOrder_[i] = spriteRenderers_[i].sortingOrder;
			}
		}
	}

	public static int getOrder(int horizontal, int vertical)
	{
		int num = vertical * 150;
		if (horizontal % 2 == 1)
		{
			num += 150;
		}
		return num + vertical - horizontal;
	}

	public int calculateOrder()
	{
		int num = -32768;
		switch (objectType_)
		{
		case eType.Road:
			num = 0;
			break;
		case eType.Fence:
		case eType.Building:
		case eType.Obstacle:
		{
			num = getOrder(horizontalIndex_, verticalIndex_);
			int num2 = Mathf.Max(1, (gridSize_.width + gridSize_.height) / 2);
			if (num2 >= 1)
			{
				num -= num2 * 75;
			}
			if (horizontalIndex_ % 2 == 0)
			{
				num++;
			}
			break;
		}
		case eType.Minilen:
			num = getOrder(horizontalIndex_, verticalIndex_) + horizontalIndex_;
			if (ParkObjectManager.Instance.existsFront(index))
			{
				num -= 75;
			}
			if (horizontalIndex_ % 2 == 1)
			{
				num -= 37;
			}
			break;
		case eType.Effect:
			num = getOrder(horizontalIndex_, verticalIndex_);
			num += 60;
			break;
		}
		return num + 150;
	}

	protected virtual void setOrder(int order)
	{
		if (spriteRenderers_ != null && originalOrder_ != null)
		{
			int num = spriteRenderers_.Length;
			for (int i = 0; i < num; i++)
			{
				spriteRenderers_[i].sortingOrder = order + originalOrder_[i];
			}
		}
		sortingOrder_ = order;
		int num2 = (int)objectType_ * 1000;
		priority_ = num2 + verticalIndex_ * 10 + longerEdge;
	}

	public void setRecalculatedOrder()
	{
		sortingOrder = calculateOrder();
	}

	protected virtual void setObjectDirection(eDirection newDirection)
	{
	}

	public virtual void InvertDirection()
	{
		switch (direction_)
		{
		case eDirection.Default:
			setObjectDirection(eDirection.Reverse);
			direction_ = eDirection.Reverse;
			break;
		case eDirection.Reverse:
			setObjectDirection(eDirection.Default);
			direction_ = eDirection.Default;
			break;
		}
	}

	public virtual void setPosition(Vector3 localPosition)
	{
		cachedTransform_.localPosition = localPosition;
	}

	public void setAlpha(float newAlpha)
	{
		int num = spriteRenderers_.Length;
		for (int i = 0; i < num; i++)
		{
			Color color = spriteRenderers_[i].color;
			color.a = newAlpha;
			spriteRenderers_[i].color = color;
		}
		alpha = newAlpha;
	}

	public void setColor(Color color)
	{
		Color color2 = color;
		color2.a = alpha;
		int num = spriteRenderers_.Length;
		for (int i = 0; i < num; i++)
		{
			spriteRenderers_[i].color = color2;
		}
	}

	public void resetColor()
	{
		int num = spriteRenderers_.Length;
		for (int i = 0; i < num; i++)
		{
			spriteRenderers_[i].color = Color.white;
		}
	}

	protected IEnumerator Bring(float timeSpan, float minAlpha = 0f, float maxAlpha = 1f)
	{
		float time2 = 0f;
		float halfTimeSpan = timeSpan * 0.5f;
		float alphaDelta = maxAlpha - minAlpha;
		while (true)
		{
			if (time2 < halfTimeSpan)
			{
				setAlpha(maxAlpha - alphaDelta * (time2 / halfTimeSpan));
				time2 += Time.deltaTime;
				yield return null;
				continue;
			}
			time2 = 0f;
			while (time2 < halfTimeSpan)
			{
				setAlpha(minAlpha + alphaDelta * (time2 / halfTimeSpan));
				time2 += Time.deltaTime;
				yield return null;
			}
			time2 = 0f;
		}
	}
}
