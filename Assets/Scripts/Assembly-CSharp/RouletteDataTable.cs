using UnityEngine;

public class RouletteDataTable : MonoBehaviour
{
	private RouletteInfo info_;

	private bool bLoaded_;

	public void load()
	{
		if (!bLoaded_)
		{
			TextAsset textAsset = Resources.Load("Parameter/roulette_info", typeof(TextAsset)) as TextAsset;
			info_ = Xml.DeserializeObject<RouletteInfo>(textAsset.text) as RouletteInfo;
			bLoaded_ = true;
		}
	}

	public RouletteInfo.Reward[] getRewards(int rankIndex)
	{
		if (rankIndex < 0 && rankIndex >= info_.RewardInfos.Length)
		{
			return null;
		}
		return info_.RewardInfos[rankIndex].Rewards;
	}

	public bool isLoaded()
	{
		return bLoaded_;
	}
}
