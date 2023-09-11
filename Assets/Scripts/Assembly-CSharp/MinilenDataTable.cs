using System;
using UnityEngine;

public class MinilenDataTable : MonoBehaviour
{
	private MinilenData _data;

	private bool bLoaded_;

	public void load()
	{
		if (!bLoaded_)
		{
			TextAsset textAsset = Resources.Load("Parameter/minilen_data", typeof(TextAsset)) as TextAsset;
			_data = Xml.DeserializeObject<MinilenData>(textAsset.text) as MinilenData;
			bLoaded_ = true;
		}
	}

	public MinilenData.MinilenInfo getInfo(int id)
	{
		if (_data == null)
		{
			return null;
		}
		return Array.Find(_data.MinilenInfoList, (MinilenData.MinilenInfo info) => info.ID == id);
	}

	public MinilenData.MinilenInfo getInfoByReleaseBuildingID(int buildingID)
	{
		if (_data == null)
		{
			return null;
		}
		return Array.Find(_data.MinilenInfoList, (MinilenData.MinilenInfo info) => info.ReleaseBuildingID == buildingID);
	}

	public MinilenData.MinilenInfo[] getAllMinilenData()
	{
		return null;
	}
}
