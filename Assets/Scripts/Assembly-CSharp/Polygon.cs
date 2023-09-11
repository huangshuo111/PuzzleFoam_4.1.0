using UnityEngine;

public class Polygon : ColliderBase
{
	[SerializeField]
	protected Vector2[] points_;

	public Vector2[] points
	{
		get
		{
			return points_;
		}
	}

	private static float Vec2Cross(Vector2 a, Vector2 b)
	{
		return a.x * b.y - a.y * b.x;
	}

	protected override void Awake()
	{
		base.Awake();
		colliderType_ = eColliderType.Polygon;
	}

	public override bool Contains(Vector2 point)
	{
		if (points_ == null)
		{
			return false;
		}
		if (points_.Length <= 2)
		{
			return false;
		}
		bool result = true;
		int num = points_.Length;
		for (int i = 0; i < num; i++)
		{
			int num2 = ((i != num - 1) ? (i + 1) : 0);
			Vector2 vector;
			Vector2 vector2;
			if (worldSpace_)
			{
				vector = points_[i];
				vector2 = points_[num2];
			}
			else
			{
				vector = base.position + points_[i];
				vector2 = base.position + points_[num2];
			}
			Vector2 a = vector2 - vector;
			Vector2 b = point - vector;
			float num3 = Vec2Cross(a, b);
			if (num3 > 0f)
			{
				result = false;
				break;
			}
		}
		return result;
	}
}
