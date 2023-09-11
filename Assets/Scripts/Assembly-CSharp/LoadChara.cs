using System.Collections;
using UnityEngine;

public class LoadChara : MonoBehaviour
{
	private const float ONE_FRAME_MOVE = 1f;

	private float moveValue_;

	private float left_;

	private float right_;

	private float toPosX_;

	public float distance;

	public Vector3 startPos;

	private float targetPos_x;

	private void Awake()
	{
		toPosX_ = base.transform.localPosition.x;
		startPos = base.transform.position;
	}

	private void LateUpdate()
	{
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = toPosX_;
		base.transform.localPosition = localPosition;
	}

	public void calcMoveValue(Transform target, int diff)
	{
		right_ = target.localPosition.x;
		left_ = base.transform.localPosition.x;
		moveValue_ = (right_ - left_) / 10f * (float)diff;
	}

	public void move(float percent)
	{
		Vector3 localPosition = base.transform.localPosition;
		float num = moveValue_;
		num += left_;
		localPosition.x = num;
		toPosX_ = localPosition.x;
	}

	public IEnumerator waitMove()
	{
		float to_x = toPosX_;
		while (to_x > base.transform.localPosition.x)
		{
			yield return 0;
		}
	}
}
