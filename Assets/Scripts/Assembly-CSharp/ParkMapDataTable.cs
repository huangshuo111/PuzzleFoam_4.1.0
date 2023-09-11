using UnityEngine;

public class ParkMapDataTable : MonoBehaviour
{
	public const string MAP_DATA_PATH = "map_data";

	private ParkMapInfo mapInfo_;

	public void load()
	{
		if (mapInfo_ == null)
		{
			TextAsset textAsset = Resources.Load("Parameter/map_data", typeof(TextAsset)) as TextAsset;
			mapInfo_ = Xml.DeserializeObject<ParkMapInfo>(textAsset.text) as ParkMapInfo;
		}
	}

	public ParkMapInfo.MapInfo getInfo(int id)
	{
		if (mapInfo_ == null)
		{
			return null;
		}
		return mapInfo_.MapInfos[id];
	}
}
