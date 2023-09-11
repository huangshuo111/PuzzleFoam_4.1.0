using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Object")]
[RequireComponent(typeof(BoxCollider))]
public class UIDragObjectEx : IgnoreTimeScale
{
	public enum DragEffect
	{
		None = 0,
		Momentum = 1,
		MomentumAndSpring = 2
	}

	public Transform target;

	public float space = 50f;

	[HideInInspector]
	public Vector4 MovingRange;

	public Vector3 scale = Vector3.one;

	public float scrollWheelFactor;

	public bool restrictWithinPanel;

	public DragEffect dragEffect = DragEffect.MomentumAndSpring;

	public float momentumAmount = 35f;

	private Plane mPlane;

	private Vector3 mLastPos;

	private bool mPressed;

	private Vector3 mMomentum = Vector3.zero;

	private float mScroll;

	private Bounds mBounds;

	private void OnPress(bool pressed)
	{
		if (!base.enabled || !NGUITools.GetActive(base.gameObject) || !(target != null))
		{
			return;
		}
		mPressed = pressed;
		if (pressed)
		{
			if (restrictWithinPanel)
			{
				mBounds = calcBounds();
			}
			mMomentum = Vector3.zero;
			mScroll = 0f;
			SpringPosition component = target.GetComponent<SpringPosition>();
			if (component != null)
			{
				component.enabled = false;
			}
			mLastPos = UICamera.lastHit.point;
			Transform transform = UICamera.currentCamera.transform;
			mPlane = new Plane(transform.rotation * Vector3.back, mLastPos);
		}
		else if (restrictWithinPanel && dragEffect == DragEffect.MomentumAndSpring)
		{
			ConstrainTargetToBoundsEx(target, ref mBounds, false);
		}
	}

	private Vector3 checkMoveValue(Vector3 move)
	{
		Vector3 result = move;
		float num = MovingRange.z / 2f;
		float num2 = MovingRange.w / 2f;
		Vector2 vector = base.transform.TransformPoint(new Vector2(0f - num + MovingRange.x - space, 0f - num2 + MovingRange.y - space));
		Vector2 vector2 = base.transform.TransformPoint(new Vector2(num + MovingRange.x + space, num2 + MovingRange.y + space));
		Vector3 position = target.position;
		Vector3 vector3 = (target.position += move);
		if (scale.x != 0f)
		{
			if (vector3.x < vector.x)
			{
				result.x -= vector3.x - vector.x;
			}
			if (vector3.x > vector2.x)
			{
				result.x -= vector3.x - vector2.x;
			}
		}
		if (scale.y != 0f)
		{
			if (vector3.y < vector.y)
			{
				result.y -= vector3.y - vector.y;
			}
			if (vector3.y > vector2.y)
			{
				result.y -= vector3.y - vector2.y;
			}
		}
		target.position = position;
		return result;
	}

	private void OnDrag(Vector2 delta)
	{
		if (!base.enabled || !NGUITools.GetActive(base.gameObject) || !(target != null))
		{
			return;
		}
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
		Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
		float enter = 0f;
		if (!mPlane.Raycast(ray, out enter))
		{
			return;
		}
		Vector3 point = ray.GetPoint(enter);
		Vector3 vector = point - mLastPos;
		mLastPos = point;
		if (vector.x != 0f || vector.y != 0f)
		{
			vector = target.InverseTransformDirection(vector);
			vector.Scale(scale);
			vector = target.TransformDirection(vector);
		}
		if (dragEffect != 0)
		{
			mMomentum = Vector3.Lerp(mMomentum, mMomentum + vector * (0.01f * momentumAmount), 0.67f);
		}
		if (restrictWithinPanel)
		{
			Vector3 localPosition = target.localPosition;
			target.position += checkMoveValue(vector);
			mBounds.center += target.localPosition - localPosition;
			if (dragEffect != DragEffect.MomentumAndSpring && ConstrainTargetToBoundsEx(target, ref mBounds, true))
			{
				mMomentum = Vector3.zero;
				mScroll = 0f;
			}
		}
		else
		{
			target.position += checkMoveValue(vector);
		}
	}

	private void LateUpdate()
	{
		float deltaTime = UpdateRealTimeDelta();
		if (target == null)
		{
			return;
		}
		if (mPressed)
		{
			SpringPosition component = target.GetComponent<SpringPosition>();
			if (component != null)
			{
				component.enabled = false;
			}
			mScroll = 0f;
		}
		else
		{
			mMomentum += scale * ((0f - mScroll) * 0.05f);
			mScroll = NGUIMath.SpringLerp(mScroll, 0f, 20f, deltaTime);
			if (mMomentum.magnitude > 0.0001f)
			{
				target.position += checkMoveValue(NGUIMath.SpringDampen(ref mMomentum, 9f, deltaTime));
				if (!restrictWithinPanel)
				{
					return;
				}
				mBounds = calcBounds();
				if (!ConstrainTargetToBoundsEx(target, ref mBounds, dragEffect == DragEffect.None))
				{
					SpringPosition component2 = target.GetComponent<SpringPosition>();
					if (component2 != null)
					{
						component2.enabled = false;
					}
				}
				return;
			}
			mScroll = 0f;
		}
		NGUIMath.SpringDampen(ref mMomentum, 9f, deltaTime);
	}

	private void OnScroll(float delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject))
		{
			if (Mathf.Sign(mScroll) != Mathf.Sign(delta))
			{
				mScroll = 0f;
			}
			mScroll += delta * scrollWheelFactor;
		}
	}

	private Bounds calcBounds()
	{
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		Vector3 localPosition = target.localPosition;
		Vector3 localScale = target.localScale;
		localScale /= 2f;
		zero.x = 0f - (localScale.x - localPosition.x);
		zero.y = 0f - (localScale.y - localPosition.y);
		zero2.x = localScale.x + localPosition.x;
		zero2.y = localScale.y + localPosition.y;
		Bounds result = new Bounds(zero, Vector3.zero);
		result.Encapsulate(zero2);
		return result;
	}

	private Vector3 CalculateConstrainOffsetEx(Vector2 min, Vector2 max)
	{
		float num = MovingRange.z * 0.5f;
		float num2 = MovingRange.w * 0.5f;
		Vector2 minRect = new Vector2(min.x, min.y);
		Vector2 maxRect = new Vector2(max.x, max.y);
		Vector2 minArea = new Vector2(MovingRange.x - num, MovingRange.y - num2);
		Vector2 maxArea = new Vector2(MovingRange.x + num, MovingRange.y + num2);
		return NGUIMath.ConstrainRect(minRect, maxRect, minArea, maxArea);
	}

	private bool ConstrainTargetToBoundsEx(Transform target, ref Bounds targetBounds, bool immediate)
	{
		Vector3 vector = CalculateConstrainOffsetEx(targetBounds.min, targetBounds.max);
		if (vector.magnitude > 0f)
		{
			if (immediate)
			{
				MoveRelative(vector);
				targetBounds.center += vector;
				SpringPosition component = target.GetComponent<SpringPosition>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
			else
			{
				SpringPosition springPosition = SpringPosition.Begin(target.gameObject, target.localPosition + vector, 13f);
				springPosition.ignoreTimeScale = true;
				springPosition.worldSpace = false;
			}
			return true;
		}
		return false;
	}

	private void MoveRelative(Vector3 relative)
	{
		target.localPosition += relative;
	}
}
