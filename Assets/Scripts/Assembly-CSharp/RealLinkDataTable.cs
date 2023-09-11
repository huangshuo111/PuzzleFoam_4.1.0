using UnityEngine;

public class RealLinkDataTable : MonoBehaviour
{
	private RealLinkInfo info_;

	private bool bLoaded_;

	public void load()
	{
		if (!bLoaded_)
		{
			TextAsset textAsset = Resources.Load("Parameter/real_link_info", typeof(TextAsset)) as TextAsset;
			info_ = Xml.DeserializeObject<RealLinkInfo>(textAsset.text) as RealLinkInfo;
			bLoaded_ = true;
		}
	}

	public RealLinkInfo getInfo()
	{
		return info_;
	}

	public bool isLoaded()
	{
		return bLoaded_;
	}
}
