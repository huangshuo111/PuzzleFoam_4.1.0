using UnityEngine;

public class ParkUnplacableGridDataTable : MonoBehaviour
{
	private const string DATA_PATH = "Parameter/unplacable_grids";

	private UnplacableGridInfo _info;

	public void load()
	{
		if (_info == null)
		{
			TextAsset textAsset = Resources.Load<TextAsset>("Parameter/unplacable_grids");
			_info = Xml.DeserializeObject<UnplacableGridInfo>(textAsset.text) as UnplacableGridInfo;
		}
	}

	public UnplacableGridInfo.UnplacableGrid[] getGrids()
	{
		if (_info == null)
		{
			return null;
		}
		return _info.UnplacableGrids;
	}
}
