using UnityEngine;

public class NetworkIcon : MonoBehaviour
{
	private TweenGroup inTween_;

	private TweenGroup outTween_;

	private void Awake()
	{
		TweenGroup[] componentsInChildren = GetComponentsInChildren<TweenGroup>(true);
		TweenGroup[] array = componentsInChildren;
		foreach (TweenGroup tweenGroup in array)
		{
			switch (tweenGroup.getGroupName())
			{
			case "In":
				inTween_ = tweenGroup;
				break;
			case "Out":
				outTween_ = tweenGroup;
				break;
			}
		}
	}

	public TweenGroup getTween(bool bShow)
	{
		if (bShow)
		{
			return inTween_;
		}
		return outTween_;
	}
}
