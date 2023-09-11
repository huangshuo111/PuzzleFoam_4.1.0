using UnityEngine;

public class BubbleBase : MonoBehaviour
{
	public Transform myTrans;

	public Bubble.eType type;

	public bool isNearBubble(BubbleBase target)
	{
		if (target == null)
		{
			return false;
		}
		float sqrMagnitude = (myTrans.localPosition - target.myTrans.localPosition).sqrMagnitude;
		if (sqrMagnitude > 8100f)
		{
			return false;
		}
		if (sqrMagnitude < 1f)
		{
			return false;
		}
		return true;
	}

	public bool isBombRangeBubble(BubbleBase target)
	{
		float sqrMagnitude = (myTrans.localPosition - target.myTrans.localPosition).sqrMagnitude;
		if (sqrMagnitude > 22500f)
		{
			return false;
		}
		return true;
	}

	public bool isSamePosBubble(BubbleBase target)
	{
		if (target == null || target == this)
		{
			return false;
		}
		float sqrMagnitude = (myTrans.localPosition - target.myTrans.localPosition).sqrMagnitude;
		if (sqrMagnitude > 900f)
		{
			return false;
		}
		return true;
	}
}
