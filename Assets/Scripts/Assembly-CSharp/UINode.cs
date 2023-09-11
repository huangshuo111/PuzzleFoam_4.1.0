using UnityEngine;

public class UINode
{
	private int mVisibleFlag = -1;

	public Transform trans;

	public UIWidget widget;

	private float mLastAlpha;

	public int changeFlag = -1;

	public int visibleFlag
	{
		get
		{
			return (!(widget != null)) ? mVisibleFlag : widget.visibleFlag;
		}
		set
		{
			if (widget != null)
			{
				widget.visibleFlag = value;
			}
			else
			{
				mVisibleFlag = value;
			}
		}
	}

	public UINode(Transform t)
	{
		trans = t;
	}

	public bool HasChanged()
	{
		if (widget != null && widget.finalAlpha != mLastAlpha)
		{
			mLastAlpha = widget.finalAlpha;
			trans.hasChanged = false;
			return true;
		}
		if (trans.hasChanged)
		{
			trans.hasChanged = false;
			return true;
		}
		return false;
	}
}
