using System.Collections.Generic;
using UnityEngine;

public class StageIconParent : MonoBehaviour
{
	private Dictionary<int, StageIcon> iconDict_ = new Dictionary<int, StageIcon>();

	public void setup()
	{
		StageIcon[] componentsInChildren = GetComponentsInChildren<StageIcon>(true);
		StageIcon[] array = componentsInChildren;
		foreach (StageIcon stageIcon in array)
		{
			iconDict_[stageIcon.getStageNo()] = stageIcon;
		}
	}

	public void setup(StageIcon[] icons)
	{
		iconDict_.Clear();
		foreach (StageIcon stageIcon in icons)
		{
			iconDict_[stageIcon.getStageNo()] = stageIcon;
		}
	}

	public Dictionary<int, StageIcon>.KeyCollection getKeys()
	{
		return iconDict_.Keys;
	}

	public StageIcon getIcon(int stageNo)
	{
		if (!iconDict_.ContainsKey(stageNo))
		{
			return null;
		}
		return iconDict_[stageNo];
	}
}
