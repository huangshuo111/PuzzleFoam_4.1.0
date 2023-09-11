using UnityEngine;

public class Circle : ColliderBase
{
	[SerializeField]
	private Vector2 center_;

	[SerializeField]
	private float radius_;

	public Vector2 center
	{
		get
		{
			return center_;
		}
		set
		{
			center_ = value;
		}
	}

	public float radius
	{
		get
		{
			return radius_;
		}
		set
		{
			radius_ = value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		colliderType_ = eColliderType.Circle;
	}

	public override bool Contains(Vector2 point)
	{
		if (worldSpace_)
		{
			float num = Vector2.Distance(center_, point);
			return radius_ >= num;
		}
		float num2 = Vector2.Distance(base.position + center_, point);
		return radius_ >= num2;
	}
}
