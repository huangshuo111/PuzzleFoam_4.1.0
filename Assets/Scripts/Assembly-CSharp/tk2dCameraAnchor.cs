using UnityEngine;

[AddComponentMenu("2D Toolkit/Camera/tk2dCameraAnchor")]
[ExecuteInEditMode]
public class tk2dCameraAnchor : MonoBehaviour
{
	public enum Anchor
	{
		UpperLeft = 0,
		UpperCenter = 1,
		UpperRight = 2,
		MiddleLeft = 3,
		MiddleCenter = 4,
		MiddleRight = 5,
		LowerLeft = 6,
		LowerCenter = 7,
		LowerRight = 8
	}

	public Anchor anchor;

	public Vector2 offset = Vector2.zero;

	public tk2dCamera tk2dCamera;

	private Transform __transform;

	private Transform _transform
	{
		get
		{
			if (__transform == null)
			{
				__transform = base.transform;
			}
			return __transform;
		}
	}

	private void Start()
	{
		UpdateTransform();
	}

	private void UpdateTransform()
	{
		if (tk2dCamera != null)
		{
			Rect screenExtents = tk2dCamera.ScreenExtents;
			float yMin = screenExtents.yMin;
			float yMax = screenExtents.yMax;
			float y = (yMax + yMin) * 0.5f;
			float xMin = screenExtents.xMin;
			float xMax = screenExtents.xMax;
			float x = (xMin + xMax) * 0.5f;
			Vector3 localPosition = _transform.localPosition;
			Vector3 vector = Vector3.zero;
			switch (anchor)
			{
			case Anchor.UpperLeft:
				vector = new Vector3(xMin, yMin, localPosition.z);
				break;
			case Anchor.UpperCenter:
				vector = new Vector3(x, yMin, localPosition.z);
				break;
			case Anchor.UpperRight:
				vector = new Vector3(xMax, yMin, localPosition.z);
				break;
			case Anchor.MiddleLeft:
				vector = new Vector3(xMin, y, localPosition.z);
				break;
			case Anchor.MiddleCenter:
				vector = new Vector3(x, y, localPosition.z);
				break;
			case Anchor.MiddleRight:
				vector = new Vector3(xMax, y, localPosition.z);
				break;
			case Anchor.LowerLeft:
				vector = new Vector3(xMin, yMax, localPosition.z);
				break;
			case Anchor.LowerCenter:
				vector = new Vector3(x, yMax, localPosition.z);
				break;
			case Anchor.LowerRight:
				vector = new Vector3(xMax, yMax, localPosition.z);
				break;
			}
			Vector3 vector2 = vector + new Vector3(offset.x, offset.y, 0f);
			Vector3 localPosition2 = _transform.localPosition;
			if (localPosition2 != vector2)
			{
				_transform.localPosition = vector2;
			}
		}
	}

	public void ForceUpdateTransform()
	{
		UpdateTransform();
	}

	private void LateUpdate()
	{
		UpdateTransform();
	}
}
