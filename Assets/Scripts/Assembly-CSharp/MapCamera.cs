using System.Collections;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
	[SerializeField]
	private float PinchScale = 0.02f;

	[SerializeField]
	private Vector2 MoveRangeMax = new Vector2(1400f, 2200f);

	[SerializeField]
	private float PinchMin = 1f;

	[SerializeField]
	private float PinchMax = 1.5f;

	[SerializeField]
	private float PinchMoveScale = 3f;

	[SerializeField]
	private float MoveTime = 2f;

	[SerializeField]
	private iTween.EaseType EaseType = iTween.EaseType.easeInBack;

	[SerializeField]
	private Vector2 DragScaleMax = Vector2.zero;

	[HideInInspector]
	private Vector3 pinchPos_ = Vector3.zero;

	private bool bMoving_;

	private Vector2 moveRangeScale_ = Vector2.zero;

	private bool bUpdate_ = true;

	private Vector4 initMoveRange_ = Vector4.zero;

	public UIDragObjectEx dragObject_;

	private Transform target_;

	private Camera uiCamera_;

	private Vector2 min_ = Vector2.zero;

	private Vector2 max_ = Vector2.zero;

	private Vector2 initDragScale_ = Vector2.zero;

	private Vector2 dragScale_ = Vector2.zero;

	private Vector3 offset_ = Vector3.zero;

	public Vector2[] DefMoveRangeMax_;

	public Vector4[] DefMoveRange_;

	[HideInInspector]
	public Vector2 EventMoveRangeMax = new Vector2(320f, 2000f);

	[HideInInspector]
	public Vector4 EventMoveRange = new Vector4(0f, 450f, 380f, 2200f);

	[SerializeField]
	public float DEF_EVENT_RANGE_MAX_W = 320f;

	[SerializeField]
	public float DEF_EVENT_RANGE_W = 380f;

	[SerializeField]
	public float DEF_EVENT_RANGE_MAX_H_ONE = 100f;

	[SerializeField]
	public float DEF_EVENT_RANGE_H_ONE = 110f;

	[SerializeField]
	public float DEF_EVENT_RANGE_CENTER_H = -650f;

	public float DebugPinchScaleMin
	{
		get
		{
			return PinchMin;
		}
		set
		{
			PinchMin = value;
		}
	}

	public float DebugPinchScaleMax
	{
		get
		{
			return PinchMax;
		}
		set
		{
			PinchMax = value;
		}
	}

	public float DebugPinchScaleFactor
	{
		get
		{
			return PinchScale;
		}
		set
		{
			PinchScale = value;
		}
	}

	public void init(Camera camera)
	{
		uiCamera_ = camera;
		dragObject_ = base.transform.parent.GetComponent<UIDragObjectEx>();
		target_ = dragObject_.target;
		initMoveRange_ = dragObject_.MovingRange;
		moveRangeScale_.x = MoveRangeMax.x - initMoveRange_.z;
		moveRangeScale_.y = MoveRangeMax.y - initMoveRange_.w;
		initDragScale_ = dragObject_.scale;
		dragScale_.x -= Mathf.Abs(DragScaleMax.x) - Mathf.Abs(initDragScale_.x);
		dragScale_.y -= Mathf.Abs(DragScaleMax.y) - Mathf.Abs(initDragScale_.y);
	}

	private void updateDragObject()
	{
		float orthographicSize = base.GetComponent<Camera>().orthographicSize;
		float num = (orthographicSize - PinchMin) * 2f;
		Vector4 movingRange = initMoveRange_;
		movingRange.z += num * moveRangeScale_.x;
		movingRange.w += num * moveRangeScale_.y;
		dragObject_.MovingRange = movingRange;
		Vector2 vector = initDragScale_;
		vector.x += num * dragScale_.x;
		vector.y += num * dragScale_.y;
		dragObject_.scale = vector;
	}

	public bool isUpdate()
	{
		return bUpdate_;
	}

	public void setUpdateFlg(bool bUpdate)
	{
		bUpdate_ = bUpdate;
	}

	public UIDragObjectEx getDragObject()
	{
		return dragObject_;
	}

	private void OnPinch(PinchGesture gesture)
	{
		if (bUpdate_)
		{
			if (gesture.Phase == ContinuousGesturePhase.Started)
			{
				pinchPos_ = gesture.Position;
			}
			float num = base.GetComponent<Camera>().orthographicSize - gesture.Delta * PinchScale;
			if (!(num <= PinchMin) && !(num >= PinchMax))
			{
				Vector3 vector = -uiCamera_.ScreenToWorldPoint(pinchPos_);
				vector.x = Mathf.Clamp(vector.x, -1f, 1f);
				vector.y = Mathf.Clamp(vector.y, -1f, 1f);
				base.GetComponent<Camera>().orthographicSize = Mathf.Clamp(base.GetComponent<Camera>().orthographicSize - gesture.Delta * PinchScale, PinchMin, PinchMax);
				updateDragObject();
				Vector3 vector2 = vector * (PinchMoveScale * gesture.Delta);
				move(-vector2);
			}
		}
	}

	private void calcDeadArea()
	{
		Vector4 movingRange = dragObject_.MovingRange;
		float num = movingRange.z / 2f;
		float num2 = movingRange.w / 2f;
		min_ = new Vector2(0f - num + movingRange.x, 0f - num2 + movingRange.y);
		max_ = new Vector2(num + movingRange.x, num2 + movingRange.y);
	}

	public void focus(Vector3 target)
	{
		target_.localPosition = clampDeadArea(target);
	}

	private Vector3 clampDeadArea(Vector3 pos)
	{
		Vector3 result = pos;
		calcDeadArea();
		result.x = Mathf.Clamp(result.x, min_.x, max_.x);
		result.y = Mathf.Clamp(result.y, min_.y, max_.y);
		return result;
	}

	public void move(Vector3 value)
	{
		calcDeadArea();
		Vector3 vector = value;
		Vector3 vector2 = target_.localPosition + value;
		if (vector2.x < min_.x)
		{
			vector.x -= vector2.x - min_.x;
		}
		if (vector2.y < min_.y)
		{
			vector.y -= vector2.y - min_.y;
		}
		if (vector2.x > max_.x)
		{
			vector.x -= vector2.x - max_.x;
		}
		if (vector2.y > max_.y)
		{
			vector.y -= vector2.y - max_.y;
		}
		target_.localPosition += vector + offset_;
	}

	public IEnumerator moveProd(Vector3 to)
	{
		Vector3 from = target_.localPosition;
		Vector3 t = clampDeadArea(to);
		if (!(from == t))
		{
			bMoving_ = true;
			iTween.MoveTo(target_.gameObject, iTween.Hash("x", t.x, "y", t.y, "islocal", true, "time", MoveTime, "easeType", EaseType));
			while (target_.GetComponent<iTween>() != null)
			{
				yield return 0;
			}
			bMoving_ = false;
		}
	}

	public bool isMoving()
	{
		return bMoving_;
	}

	public void setMoveRange(Vector2 rangeMax, Vector4 range)
	{
		MoveRangeMax = rangeMax;
		dragObject_.MovingRange = range;
		initMoveRange_ = dragObject_.MovingRange;
		moveRangeScale_.x = MoveRangeMax.x - initMoveRange_.z;
		moveRangeScale_.y = MoveRangeMax.y - initMoveRange_.w;
		updateDragObject();
	}
}
