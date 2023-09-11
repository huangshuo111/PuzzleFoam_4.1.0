using System.Collections;
using LitJson;
using Network;
using UnityEngine;

public class BossDataTable : MonoBehaviour
{
	private BossListData bossData_;

	public BossListData getBossData()
	{
		return bossData_;
	}

	public void setBossData(BossListData bossData)
	{
		bossData_ = bossData;
	}

	private WWW OnCreateWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		return WWWWrap.create("boss/list/");
	}

	public IEnumerator download(bool bCancel, bool bShowIcon)
	{
		Hashtable args = new Hashtable();
		args.Clear();
		NetworkMng.Instance.setup(args);
		yield return NetworkMng.Instance.StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, bCancel, bShowIcon));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			bossData_ = JsonMapper.ToObject<BossListData>(www.text);
		}
	}

	public bool isPlayed(int type, int level)
	{
		return bossData_.bossList[type].bossLevelList[level - 1].playCount > 0;
	}
}
