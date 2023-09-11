using UnityEngine;

public class Rhombus : Polygon
{
	[SerializeField]
	private Vector2 center_;

	[SerializeField]
	private Vector2 extents_;

	public Vector2 center
	{
		get
		{
			return center_;
		}
		set
		{
			center_ = value;
			ResetPoints();
		}
	}

	public Vector2 extents
	{
		get
		{
			return extents_;
		}
		set
		{
			extents_ = value;
			ResetPoints();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		points_ = new Vector2[4];
		colliderType_ = eColliderType.Rhombus;
	}

	private void ResetPoints()
	{
		if (points_ == null)
		{
			points_ = new Vector2[4];
		}
		points_[0] = center + new Vector2(0f, extents_.y);
		points_[1] = center + new Vector2(extents_.x, 0f);
		points_[2] = center + new Vector2(0f, 0f - extents_.y);
		points_[3] = center + new Vector2(0f - extents_.x, 0f);
	}
}
