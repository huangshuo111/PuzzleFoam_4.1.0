using UnityEngine;

public class DummyFriendDataTable : MonoBehaviour
{
	private DummyFriendInfo info_;

	private bool bLoaded_;

	public void load()
	{
		if (!bLoaded_)
		{
			TextAsset textAsset = Resources.Load("Parameter/dummy_friend_info", typeof(TextAsset)) as TextAsset;
			info_ = Xml.DeserializeObject<DummyFriendInfo>(textAsset.text) as DummyFriendInfo;
			bLoaded_ = true;
		}
	}

	public int getInfoCount()
	{
		return info_.Infos.Length;
	}

	public DummyFriendInfo.Info getInfo(int id)
	{
		if (id < 0 && id >= info_.Infos.Length)
		{
			return null;
		}
		return info_.Infos[id];
	}

	public bool isLoaded()
	{
		return bLoaded_;
	}
}
