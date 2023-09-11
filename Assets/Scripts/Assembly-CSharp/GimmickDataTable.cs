using UnityEngine;

public class GimmickDataTable : MonoBehaviour
{
	private GimmickPercentInfo info_;

	private bool bLoaded_;

	public void load()
	{
		if (!bLoaded_)
		{
			TextAsset textAsset = Resources.Load("Parameter/gimmick_percent_info", typeof(TextAsset)) as TextAsset;
			info_ = Xml.DeserializeObject<GimmickPercentInfo>(textAsset.text) as GimmickPercentInfo;
			bLoaded_ = true;
		}
	}

	public bool isLoaded()
	{
		return bLoaded_;
	}

	public GimmickPercentInfo getGimmickInfo()
	{
		return info_;
	}
}
