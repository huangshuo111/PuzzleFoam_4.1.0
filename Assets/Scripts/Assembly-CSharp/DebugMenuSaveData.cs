using System.Collections;
using UnityEngine;

public class DebugMenuSaveData : DebugMenuBase
{
	private enum eItem
	{
		Save = 0,
		Load = 1,
		Reset = 2,
		Max = 3
	}

	private SaveData saveData_;

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(3, "SaveData"));
		if (SaveData.IsInstance())
		{
			saveData_ = SaveData.Instance;
		}
	}

	public override void OnDraw()
	{
		if (!(saveData_ == null))
		{
			DrawItem(0, "Save", eItemType.CenterOnly);
			DrawItem(1, "Load", eItemType.CenterOnly);
			DrawItem(2, "Reset", eItemType.CenterOnly);
		}
	}

	public override void OnExecute()
	{
		if (!(saveData_ == null))
		{
			if (IsPressCenterButton(0))
			{
				saveData_.save();
			}
			if (IsPressCenterButton(1))
			{
				saveData_.load();
			}
			if (IsPressCenterButton(2))
			{
				saveData_.reset();
				PlayerPrefs.DeleteAll();
			}
		}
	}
}
