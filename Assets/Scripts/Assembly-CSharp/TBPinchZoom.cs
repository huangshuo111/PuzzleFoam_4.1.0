using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("FingerGestures/Toolbox/Camera/Pinch-Zoom")]
[RequireComponent(typeof(PinchRecognizer))]
public class TBPinchZoom : MonoBehaviour
{
	public enum ZoomMethod
	{
		Position = 0,
		FOV = 1
	}

	public ZoomMethod zoomMethod;

	public float zoomSpeed = 1.5f;

	public float minZoomAmount;

	public float maxZoomAmount = 50f;

	private Vector3 defaultPos = Vector3.zero;

	private float defaultFov;

	private float defaultOrthoSize;

	private float zoomAmount;

	public Vector3 DefaultPos
	{
		get
		{
			return defaultPos;
		}
		set
		{
			defaultPos = value;
		}
	}

	public float DefaultFov
	{
		get
		{
			return defaultFov;
		}
		set
		{
			defaultFov = value;
		}
	}

	public float DefaultOrthoSize
	{
		get
		{
			return defaultOrthoSize;
		}
		set
		{
			defaultOrthoSize = value;
		}
	}

	public float ZoomAmount
	{
		get
		{
			return zoomAmount;
		}
		set
		{
			zoomAmount = Mathf.Clamp(value, minZoomAmount, maxZoomAmount);
			switch (zoomMethod)
			{
			case ZoomMethod.Position:
				base.transform.position = defaultPos + zoomAmount * base.transform.forward;
				break;
			case ZoomMethod.FOV:
				if (base.GetComponent<Camera>().orthographic)
				{
					base.GetComponent<Camera>().orthographicSize = Mathf.Max(defaultOrthoSize - zoomAmount, 0.1f);
				}
				else
				{
					base.GetComponent<Camera>().fov = Mathf.Max(defaultFov - zoomAmount, 0.1f);
				}
				break;
			}
		}
	}

	public float ZoomPercent
	{
		get
		{
			return (ZoomAmount - minZoomAmount) / (maxZoomAmount - minZoomAmount);
		}
	}

	private void Start()
	{
		if (!GetComponent<PinchRecognizer>())
		{
			Debug.LogWarning("No pinch recognizer found on " + base.name + ". Disabling TBPinchZoom.");
			base.enabled = false;
		}
		SetDefaults();
	}

	public void SetDefaults()
	{
		DefaultPos = base.transform.position;
		DefaultFov = base.GetComponent<Camera>().fov;
		DefaultOrthoSize = base.GetComponent<Camera>().orthographicSize;
	}

	private void OnPinch(PinchGesture gesture)
	{
		ZoomAmount += zoomSpeed * gesture.Delta;
	}
}
