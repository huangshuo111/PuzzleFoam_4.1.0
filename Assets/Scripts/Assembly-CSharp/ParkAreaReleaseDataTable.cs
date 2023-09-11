using System.Collections.Generic;
using UnityEngine;

public class ParkAreaReleaseDataTable : MonoBehaviour
{
	private ParkAreaReleaseInfo releaseAreaInfo_;

	public int dataCount
	{
		get
		{
			if (releaseAreaInfo_ == null)
			{
				return 0;
			}
			return releaseAreaInfo_.ReleaseAreaInfos.Count;
		}
	}

	public void load()
	{
		if (releaseAreaInfo_ != null)
		{
			return;
		}
		TextAsset textAsset = Resources.Load<TextAsset>("Parameter/release_area_list");
		if (textAsset == null)
		{
			return;
		}
		releaseAreaInfo_ = new ParkAreaReleaseInfo();
		releaseAreaInfo_.ReleaseAreaInfos = new List<ParkAreaReleaseInfo.AreaReleaseInfo>();
		string text = textAsset.text.Replace("\n", string.Empty);
		text = text.Replace("\r", string.Empty);
		string[] array = text.Split(';');
		foreach (string text2 in array)
		{
			if (string.IsNullOrEmpty(text2))
			{
				continue;
			}
			string[] array2 = text2.Split(':');
			ParkAreaReleaseInfo.AreaReleaseInfo areaReleaseInfo = new ParkAreaReleaseInfo.AreaReleaseInfo();
			areaReleaseInfo.LockedGrids = new List<ParkAreaReleaseInfo.AreaReleaseInfo.LockedGrid>();
			areaReleaseInfo.AreaName = array2[0];
			string[] array3 = array2[1].Split('|');
			for (int j = 0; j < array3.Length; j++)
			{
				if (!string.IsNullOrEmpty(array3[j]))
				{
					string text3 = array3[j];
					text3 = text3.Replace("{", string.Empty);
					text3 = text3.Replace("}", string.Empty);
					string[] array4 = text3.Split(',');
					ParkAreaReleaseInfo.AreaReleaseInfo.LockedGrid lockedGrid = new ParkAreaReleaseInfo.AreaReleaseInfo.LockedGrid();
					lockedGrid.Index.x = int.Parse(array4[0]);
					lockedGrid.Index.y = int.Parse(array4[1]);
					int num = int.Parse(array4[2]);
					lockedGrid.ObstructiveObjectID = Mathf.Abs(num);
					lockedGrid.IsReverse = num < 0;
					areaReleaseInfo.LockedGrids.Add(lockedGrid);
				}
			}
			releaseAreaInfo_.ReleaseAreaInfos.Add(areaReleaseInfo);
			releaseAreaInfo_.ReleaseAreaInfos.Sort((ParkAreaReleaseInfo.AreaReleaseInfo a, ParkAreaReleaseInfo.AreaReleaseInfo b) => a.areaID - b.areaID);
		}
	}

	public ParkAreaReleaseInfo.AreaReleaseInfo getInfo(int ID)
	{
		if (releaseAreaInfo_ == null)
		{
			return null;
		}
		return releaseAreaInfo_.ReleaseAreaInfos.Find((ParkAreaReleaseInfo.AreaReleaseInfo m) => m.areaID == ID);
	}

	public ParkAreaReleaseInfo.AreaReleaseInfo[] getAllInfo()
	{
		if (releaseAreaInfo_ == null)
		{
			return null;
		}
		return releaseAreaInfo_.ReleaseAreaInfos.ToArray();
	}
}
