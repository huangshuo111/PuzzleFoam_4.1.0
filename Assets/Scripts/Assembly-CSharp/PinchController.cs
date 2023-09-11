using UnityEngine;

public class PinchController : MonoBehaviour
{
	private const float PINCH_SCALE_COEFFICIENT = 1f;

	private float scaleDelta_;

	private float prevDistance_;

	private Vector2 pointsCenter_;

	public bool isPinch { get; private set; }

	public bool isPinchStart { get; private set; }

	public float scaleDelta
	{
		get
		{
			return scaleDelta_;
		}
	}

	public Vector2 center
	{
		get
		{
			return pointsCenter_;
		}
	}

	private void Awake()
	{
		isPinch = false;
		isPinchStart = false;
	}

	private void Update()
	{
		int touchCount = Input.touchCount;
		Touch[] touches = Input.touches;
		if (!isPinch)
		{
			if (touchCount >= 2)
			{
				Touch touch = touches[0];
				Touch touch2 = touches[1];
				prevDistance_ = Vector2.Distance(touch.position, touch2.position);
				pointsCenter_ = (touch.position + touch2.position) / 2f;
				scaleDelta_ = 0f;
				isPinch = true;
				isPinchStart = true;
			}
			return;
		}
		isPinchStart = false;
		if (touchCount == 0)
		{
			isPinch = false;
			scaleDelta_ = 0f;
		}
		else if (touchCount >= 2)
		{
			Touch touch3 = touches[0];
			Touch touch4 = touches[1];
			float num = Vector2.Distance(touch3.position, touch4.position);
			scaleDelta_ = (num / prevDistance_ - 1f) * 1f;
			prevDistance_ = num;
		}
	}
}
