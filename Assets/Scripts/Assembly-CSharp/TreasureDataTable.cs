using UnityEngine;

public class TreasureDataTable : MonoBehaviour
{
	private TreasureInfo info_;

	private bool bLoaded_;

	public void load()
	{
		if (!bLoaded_)
		{
			TextAsset textAsset = Resources.Load("Parameter/treasure_info", typeof(TextAsset)) as TextAsset;
			info_ = Xml.DeserializeObject<TreasureInfo>(textAsset.text) as TreasureInfo;
			bLoaded_ = true;
		}
	}

	public TreasureInfo.BoxInfo getInfo(int id)
	{
		if (id < 0 || id >= info_.BoxInfos.Length)
		{
			return null;
		}
		return info_.BoxInfos[id];
	}

	public bool isLoaded()
	{
		return bLoaded_;
	}

	public int getBoxNum()
	{
		return info_.BoxInfos.Length;
	}
}
