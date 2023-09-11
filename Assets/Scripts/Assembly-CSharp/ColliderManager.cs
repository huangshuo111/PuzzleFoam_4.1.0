using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
	[SerializeField]
	private float LONG_PRESS_TIME = 0.75f;

	[SerializeField]
	private float DRAG_THRESHOLD = 16f;

	private static ColliderManager instance_;

	private List<ColliderBase> colliderList_ = new List<ColliderBase>();

	private Camera checkCamera_;

	private Vector2 position_;

	private Vector2 lastPointerDownPosition_;

	private bool isDragging_;

	private float pressTime_;

	[SerializeField]
	private EventHandler lastEnterObject_;

	[SerializeField]
	private EventHandler lastPressObject_;

	[SerializeField]
	private EventHandler lastLongPressObject_;

	[SerializeField]
	private EventHandler lastClickObject_;

	[SerializeField]
	private EventHandler draggingObject_;

	private List<ColliderBase> _collider_backyard;

	public static ColliderManager Instance
	{
		get
		{
			if (instance_ == null)
			{
				instance_ = Object.FindObjectOfType<ColliderManager>();
				if (instance_ == null)
				{
					GameObject gameObject = new GameObject("ColliderManager");
					instance_ = gameObject.AddComponent<ColliderManager>();
				}
			}
			return instance_;
		}
	}

	public static bool haveInstance
	{
		get
		{
			return instance_ != null;
		}
	}

	public bool dontIntersect { get; set; }

	public bool isDownNGUIObject { get; set; }

	public float longPressTime
	{
		get
		{
			return LONG_PRESS_TIME;
		}
	}

	public float pressTime
	{
		get
		{
			return pressTime_;
		}
	}

	public EventHandler lastPressObject
	{
		get
		{
			return lastPressObject_;
		}
	}

	public EventHandler lastEnterObject
	{
		get
		{
			return lastEnterObject_;
		}
	}

	public EventHandler lastClickObject
	{
		get
		{
			return lastClickObject_;
		}
	}

	public EventHandler lastLongPressObject
	{
		get
		{
			return lastLongPressObject_;
		}
	}

	public EventHandler draggingObject
	{
		get
		{
			return draggingObject_;
		}
	}

	public void setParent(Transform parent)
	{
		base.transform.SetParent(parent, false);
	}

	public void Add(ColliderBase collider)
	{
		if (collider != null)
		{
			colliderList_.Add(collider);
		}
	}

	public void Remove(ColliderBase collider)
	{
		if (collider != null)
		{
			colliderList_.Remove(collider);
		}
	}

	public void setCheckCamera(Camera checkCamera)
	{
		checkCamera_ = checkCamera;
	}

	public void SortBySortingOrder()
	{
		colliderList_.Sort((ColliderBase a, ColliderBase b) => a.priority - b.priority);
	}

	public bool IntersectPoint(Vector3 point)
	{
		bool result = false;
		if (checkCamera_ == null)
		{
			return result;
		}
		Vector3 vector = checkCamera_.ScreenToWorldPoint(point);
		Vector2 worldPoint = new Vector2(vector.x, vector.y);
		bool mouseButtonDown = Input.GetMouseButtonDown(0);
		bool mouseButton = Input.GetMouseButton(0);
		bool mouseButtonUp = Input.GetMouseButtonUp(0);
		if (isDragging_ && draggingObject_ != null && mouseButton)
		{
			if (draggingObject_.enableOnDrag)
			{
				Vector3 delta = point - new Vector3(position_.x, position_.y);
				draggingObject_.OnDrag(point, delta);
			}
			return result;
		}
		List<ColliderBase> list = new List<ColliderBase>();
		result = IntersectWorldPoint(worldPoint, list);
		position_ = point;
		if (result)
		{
			lastEnterObject_ = list[0].eventHandler;
			if (mouseButtonDown)
			{
				lastPressObject_ = lastEnterObject_;
				lastPointerDownPosition_ = point;
				if (lastPressObject_ != null && lastPressObject_.enableOnPress)
				{
					lastPressObject_.OnPress(point);
				}
				return result;
			}
			if (mouseButton)
			{
				if (lastPressObject_ == null)
				{
					return result;
				}
				pressTime_ = Mathf.Clamp(pressTime_ + Time.deltaTime, 0f, float.MaxValue);
				if (pressTime_ >= LONG_PRESS_TIME)
				{
					lastLongPressObject_ = lastPressObject_;
					lastClickObject_ = null;
					if (lastLongPressObject_.enableOnLongPress)
					{
						lastLongPressObject_.OnLongPress(point);
					}
				}
				if (Vector3.Distance(lastPointerDownPosition_, point) > DRAG_THRESHOLD)
				{
					isDragging_ = true;
					pressTime_ = 0f;
					draggingObject_ = lastPressObject_;
					if (draggingObject_.enableOnDragStart)
					{
						draggingObject_.OnDragStart(point);
					}
				}
				return result;
			}
			if (mouseButtonUp)
			{
				if (!isDragging_)
				{
					if (lastPressObject_ == lastEnterObject_ && Vector3.Distance(lastPointerDownPosition_, point) < DRAG_THRESHOLD)
					{
						if (lastEnterObject_.enableOnClick)
						{
							lastEnterObject_.OnClick(point);
						}
						lastClickObject_ = lastEnterObject_;
					}
					if (lastPressObject_ != null && lastPressObject_.enableOnRelease)
					{
						lastPressObject_.OnRelease(point);
					}
				}
				else
				{
					if (draggingObject_.enableOnDragEnd)
					{
						draggingObject_.OnDragEnd(point);
					}
					isDragging_ = false;
				}
				pressTime_ = 0f;
				return result;
			}
		}
		else
		{
			if (mouseButtonDown)
			{
				lastPressObject_ = null;
				lastClickObject_ = null;
				lastLongPressObject_ = null;
			}
			if (mouseButton)
			{
				lastPressObject_ = null;
				if (draggingObject_ != null)
				{
					if (draggingObject_.enableOnDragEnd)
					{
						draggingObject_.OnDragEnd(point);
					}
					draggingObject_ = null;
				}
			}
			if (mouseButtonUp)
			{
				isDragging_ = false;
				pressTime_ = 0f;
			}
			lastEnterObject_ = null;
		}
		return result;
	}

	private bool IntersectWorldPoint(Vector2 worldPoint, List<ColliderBase> results)
	{
		bool result = false;
		int count = colliderList_.Count;
		for (int i = 0; i < count; i++)
		{
			if (colliderList_[i].enabled && colliderList_[i].Contains(worldPoint))
			{
				results.Add(colliderList_[i]);
				result = true;
			}
		}
		return result;
	}

	private void Update()
	{
		if (dontIntersect)
		{
			dontIntersect = false;
			isDragging_ = false;
			lastEnterObject_ = null;
			lastPressObject_ = null;
			lastClickObject_ = null;
			lastLongPressObject_ = null;
			draggingObject_ = null;
			pressTime_ = 0f;
		}
		else if (Input.touchCount != 1)
		{
			pressTime_ = 0f;
		}
		else
		{
			IntersectPoint(Input.mousePosition);
		}
	}

	private void LateUpdate()
	{
		if (isDownNGUIObject)
		{
			isDownNGUIObject = false;
		}
	}

	public void Clear()
	{
		colliderList_.Clear();
	}

	public void TutorialClear()
	{
		_collider_backyard = colliderList_;
		colliderList_ = new List<ColliderBase>();
	}

	public void TutorialRevirt()
	{
		colliderList_ = _collider_backyard;
		_collider_backyard = null;
	}
}
