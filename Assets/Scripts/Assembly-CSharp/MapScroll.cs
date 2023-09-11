using System;
using System.Collections;
using UnityEngine;

public class MapScroll : MonoBehaviour
{
	[Serializable]
	public class ScrollRange
	{
		public float minX;

		public float minY;

		public float maxX;

		public float maxY;

		public ScrollRange()
		{
		}

		public ScrollRange(float minX, float minY, float maxX, float maxY)
		{
			this.minX = minX;
			this.minY = minY;
			this.maxX = maxX;
			this.maxY = maxY;
		}
	}

	private const float DEFAULT_ZOOM = 1f;

	private const float OBJECT_DRAGGING_SCROLL_ZOOM_CORFFICIENT = 0.4f;

	private const float OBJECT_DRAGGING_SCROLL_ADJUST = 0.04f;

	private const float INERTIA_MAGNITUDE_MIN = 0.03f;

	[SerializeField]
	private ScrollRange AUTOSCROLL_RANGE = new ScrollRange(0.2f, 0.3f, 0.2f, 0.25f);

	[SerializeField]
	private float DRAG_THRESHOLD = 8f;

	private float dragCoefficient_ = 1.3f;

	private float slidingCoefficient_ = 0.5f;

	private float frictionCoefficient_ = 0.85f;

	private float zoomCoefficient_ = 2f;

	private Camera mapCamera_;

	private Transform cachedTransform_;

	private Vector2 backgroundHalfSize_;

	private float aspectRatio_;

	private Vector3 mouseFirstPressPosition_;

	private Vector3 mousePreviousPosition_;

	private Vector2 moveBoundary_;

	private Vector3 inertia_;

	private PinchController pinchController_;

	private float zoom_;

	private float zoomMin_ = 1f;

	private float zoomMax_ = 2.75f;

	private ParkObject trackingObject_;

	public bool enableScroll { get; set; }

	public float currentZoom
	{
		get
		{
			return zoom_;
		}
	}

	public bool isTracking { get; private set; }

	public bool isAutoScrolling { get; private set; }

	public bool isPinch
	{
		get
		{
			return pinchController_.isPinch;
		}
	}

	public float dragCoefficient
	{
		get
		{
			return dragCoefficient_;
		}
		set
		{
			dragCoefficient_ = value;
		}
	}

	public float zoomCoefficient
	{
		get
		{
			return zoomCoefficient_;
		}
		set
		{
			zoomCoefficient_ = value;
		}
	}

	public bool isScrolling { get; set; }

	public IEnumerator setup(ParkStructures.Size backgroundSize)
	{
		mapCamera_ = GetComponent<Camera>();
		cachedTransform_ = base.transform;
		zoom_ = 1f;
		backgroundHalfSize_ = backgroundSize.toVec2() * 0.5f;
		aspectRatio_ = (float)Screen.width / (float)Screen.height;
		cachedTransform_.localPosition = new Vector3(0f, 0f, 0f);
		setNewOrthographicSize();
		pinchController_ = base.gameObject.AddComponent<PinchController>();
		enableScroll = true;
		yield break;
	}

	public void ResetZoom()
	{
		zoom_ = 1f;
		setNewOrthographicSize();
		mousePreviousPosition_ = Vector3.zero;
		inertia_ = Vector3.zero;
	}

	private void LateUpdate()
	{
		if (isTracking)
		{
			Track();
			mousePreviousPosition_ = Vector3.zero;
			inertia_ = Vector3.zero;
			return;
		}
		if (!Input.enable)
		{
			mousePreviousPosition_ = Vector3.zero;
			inertia_ = Vector3.zero;
			return;
		}
		Touch[] touches = Input.touches;
		bool isDown = false;
		bool isUp = false;
		Vector3 currentPosition = Vector3.zero;
		if (touches != null && Input.touchCount == 1)
		{
			isDown = Input.touches[0].phase == TouchPhase.Stationary || Input.touches[0].phase == TouchPhase.Moved;
			isUp = Input.touches[0].phase == TouchPhase.Ended;
			currentPosition = Input.mousePosition;
		}
		float num = pinchController_.scaleDelta * zoomCoefficient_;
		if (!enableScroll)
		{
			mousePreviousPosition_ = Vector3.zero;
			inertia_ = Vector3.zero;
			return;
		}
		if (Mathf.Abs(num) > 0f)
		{
			float num2 = zoom_;
			zoom_ = Mathf.Clamp(zoom_ + num, zoomMin_, zoomMax_);
			if (zoom_ != num2)
			{
				setNewOrthographicSize();
				Vector3 newCameraZoomCenter = getNewCameraZoomCenter(cachedTransform_.localPosition);
				newCameraZoomCenter.z = 0f;
				cachedTransform_.localPosition = newCameraZoomCenter;
			}
		}
		Scroll(currentPosition, isDown, isUp);
	}

	private void Scroll(Vector3 currentPosition, bool isDown, bool isUp)
	{
		if (Input.touchCount >= 2)
		{
			mousePreviousPosition_ = Vector3.zero;
			inertia_ = Vector3.zero;
			return;
		}
		if (isDown)
		{
			if (mousePreviousPosition_ == Vector3.zero)
			{
				mousePreviousPosition_ = currentPosition;
				mouseFirstPressPosition_ = currentPosition;
				return;
			}
			if (Vector3.Distance(currentPosition, mouseFirstPressPosition_) > DRAG_THRESHOLD)
			{
				isScrolling = true;
				Vector3 offset = (currentPosition - mousePreviousPosition_) * dragCoefficient_ / zoom_;
				Vector3 localPosition = cachedTransform_.localPosition - getNewOffset(cachedTransform_.localPosition, offset);
				localPosition.z = 0f;
				cachedTransform_.localPosition = localPosition;
				mousePreviousPosition_ = currentPosition;
			}
		}
		else if (inertia_.magnitude > 0.03f)
		{
			Vector3 newOffset = getNewOffset(cachedTransform_.localPosition, inertia_);
			Vector3 localPosition2 = cachedTransform_.localPosition - newOffset;
			localPosition2.z = 0f;
			cachedTransform_.localPosition = localPosition2;
			inertia_ = newOffset * frictionCoefficient_;
		}
		else
		{
			inertia_ = Vector3.zero;
		}
		if (isUp)
		{
			if (mousePreviousPosition_ != Vector3.zero)
			{
				Vector3 vector = currentPosition - mousePreviousPosition_;
				inertia_ = vector * slidingCoefficient_;
				inertia_.z = 0f;
			}
			isScrolling = false;
			mouseFirstPressPosition_ = Vector3.zero;
			mousePreviousPosition_ = Vector3.zero;
		}
	}

	private bool ForceScroll(Vector2 delta)
	{
		Vector3 vector = cachedTransform_.localPosition - getNewOffset(cachedTransform_.localPosition, delta);
		vector.z = 0f;
		if (vector == cachedTransform_.localPosition)
		{
			return true;
		}
		cachedTransform_.localPosition = vector;
		return false;
	}

	private void ResetScrollParameter()
	{
		mousePreviousPosition_ = Vector3.zero;
		mouseFirstPressPosition_ = Vector3.zero;
		inertia_ = Vector3.zero;
	}

	public void StartAutoScroll()
	{
		isAutoScrolling = true;
	}

	public void EndAutoScroll()
	{
		isAutoScrolling = false;
		ResetScrollParameter();
	}

	public void ScrollByObjectDragging()
	{
		Vector3 mousePosition = Input.mousePosition;
		float min = AUTOSCROLL_RANGE.minX * (float)Screen.width;
		float num = AUTOSCROLL_RANGE.maxX * (float)Screen.width;
		float min2 = AUTOSCROLL_RANGE.minY * (float)Screen.height;
		float num2 = AUTOSCROLL_RANGE.maxY * (float)Screen.height;
		float num3 = 0.04f / zoom_;
		if (!isInside(mousePosition.x, min, (float)Screen.width - num))
		{
			Vector2 vector = new Vector2((float)(Screen.width / 2) - mousePosition.x, 0f);
			ForceScroll(vector * num3);
		}
		if (!isInside(mousePosition.y, min2, (float)Screen.height - num2))
		{
			Vector2 vector2 = new Vector2(0f, (float)(Screen.height / 2) - mousePosition.y);
			ForceScroll(vector2 * num3);
		}
	}

	public void StartTracking(ParkObject parkObject)
	{
		trackingObject_ = parkObject;
		isTracking = true;
	}

	public void EndTracking()
	{
		trackingObject_ = null;
		isTracking = false;
		ResetScrollParameter();
	}

	private void Track()
	{
		Vector3 vector = trackingObject_.cachedTransform.localPosition + new Vector3(0f, trackingObject_.spriteSize.height / 2);
		Vector3 vector2 = cachedTransform_.localPosition + new Vector3(backgroundHalfSize_.x, 0f - backgroundHalfSize_.y) - vector;
		if (vector2.magnitude > 0.05f)
		{
			if (ForceScroll(vector2 * 0.1f))
			{
				EndTracking();
			}
		}
		else
		{
			EndTracking();
		}
	}

	private bool isInside(float value, float min, float max)
	{
		return min <= value && value <= max;
	}

	private Vector3 getNewOffset(Vector3 position, Vector3 offset)
	{
		Vector3 vector = position - offset;
		Vector3 result = offset;
		if (!isInside(vector.x, 0f - moveBoundary_.x, moveBoundary_.x))
		{
			result.x = 0f;
		}
		if (!isInside(vector.y, 0f - moveBoundary_.y, moveBoundary_.y))
		{
			result.y = 0f;
		}
		return result;
	}

	private Vector3 getNewCameraZoomCenter(Vector3 center)
	{
		Vector3 result = center;
		if (!isInside(center.x, 0f - moveBoundary_.x, moveBoundary_.x))
		{
			if (center.x < 0f)
			{
				result.x = 0f - moveBoundary_.x;
			}
			else
			{
				result.x = moveBoundary_.x;
			}
		}
		if (!isInside(center.y, 0f - moveBoundary_.y, moveBoundary_.y))
		{
			if (center.y < 0f)
			{
				result.y = 0f - moveBoundary_.y;
			}
			else
			{
				result.y = moveBoundary_.y;
			}
		}
		return result;
	}

	private void setNewOrthographicSize()
	{
		mapCamera_.orthographicSize = backgroundHalfSize_.y / zoom_;
		moveBoundary_.x = backgroundHalfSize_.x - mapCamera_.orthographicSize * aspectRatio_;
		moveBoundary_.y = backgroundHalfSize_.y - mapCamera_.orthographicSize;
	}
}
