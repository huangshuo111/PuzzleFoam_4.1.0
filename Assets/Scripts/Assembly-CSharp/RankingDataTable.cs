using UnityEngine;

public class RankingDataTable : MonoBehaviour
{
	private RankingInfo info_;

	private bool bLoaded_;

	public void load()
	{
		if (!bLoaded_)
		{
			TextAsset textAsset = Resources.Load("Parameter/ranking_info", typeof(TextAsset)) as TextAsset;
			info_ = Xml.DeserializeObject<RankingInfo>(textAsset.text) as RankingInfo;
			bLoaded_ = true;
		}
	}

	public bool getReward(int idx, ref Constant.Reward reward)
	{
		if (idx < 0 && idx >= info_.RewardInfos.Length)
		{
			return false;
		}
		reward.set((Constant.eMoney)info_.RewardInfos[idx].RewardType, info_.RewardInfos[idx].Num);
		return true;
	}

	public bool isLoaded()
	{
		return bLoaded_;
	}
}
