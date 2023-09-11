using UnityEngine;

public class Square : Polygon
{
	[SerializeField]
	private Vector2 center_;

	[SerializeField]
	private Vector2 size_;

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

	public Vector2 size
	{
		get
		{
			return size_;
		}
		set
		{
			size_ = value;
			ResetPoints();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		points_ = new Vector2[4];
		colliderType_ = eColliderType.Square;
		ResetPoints();
	}

	private void ResetPoints()
	{
		if (points_ == null)
		{
			points_ = new Vector2[4];
		}
		Vector2 vector = size_ * 0.5f;
		points_[0] = center + new Vector2(vector.x, vector.y);
		points_[1] = center + new Vector2(vector.x, 0f - vector.y);
		points_[2] = center + new Vector2(0f - vector.x, 0f - vector.y);
		points_[3] = center + new Vector2(0f - vector.x, vector.y);
	}
}
