using UnityEngine;

public class ColliderBase : MonoBehaviour
{
	public enum eColliderType
	{
		Circle = 0,
		Square = 1,
		Rhombus = 2,
		Polygon = 3,
		Max = 4
	}

	protected Transform cachedTransform_;

	protected eColliderType colliderType_;

	protected EventHandler eventHandler_;

	protected bool worldSpace_;

	protected Vector2 position
	{
		get
		{
			if (cachedTransform_ == null)
			{
				cachedTransform_ = base.transform;
			}
			return new Vector2(cachedTransform_.position.x, cachedTransform_.position.y);
		}
	}

	public EventHandler eventHandler
	{
		get
		{
			return eventHandler_;
		}
		set
		{
			eventHandler_ = value;
		}
	}

	public eColliderType colliderType
	{
		get
		{
			return colliderType_;
		}
	}

	public int priority
	{
		get
		{
			if (eventHandler_ == null)
			{
				return 0;
			}
			return eventHandler_.priority;
		}
	}

	public bool worldSpace
	{
		get
		{
			return worldSpace_;
		}
		set
		{
			worldSpace_ = value;
		}
	}

	protected virtual void Awake()
	{
		ColliderManager.Instance.Add(this);
	}

	public virtual bool Contains(Vector2 point)
	{
		return false;
	}

	public virtual void OnDrawGizmos()
	{
	}
}
