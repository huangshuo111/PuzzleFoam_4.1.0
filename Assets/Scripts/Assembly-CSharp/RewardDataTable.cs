using UnityEngine;

public class RewardDataTable : MonoBehaviour
{
	public enum eRate
	{
		Clear = 0,
		Failed = 1,
		AlreadyClear = 2,
		AlreadyFailed = 3
	}

	private RewardInfo data_;

	private bool bLoaded_;

	public void load()
	{
		if (!bLoaded_)
		{
			TextAsset textAsset = Resources.Load("Parameter/reward_info", typeof(TextAsset)) as TextAsset;
			data_ = Xml.DeserializeObject<RewardInfo>(textAsset.text) as RewardInfo;
			bLoaded_ = true;
		}
	}

	public bool isLoaded()
	{
		return bLoaded_;
	}

	public int getLvCap()
	{
		int num = 0;
		return GlobalData.Instance.getGameData().maxLevel;
	}

	public int getExp(int lv)
	{
		if (overLevelCap(lv))
		{
			return 0;
		}
		return data_.ExpInfos[lv].Exp;
	}

	public int getBonus(int lv)
	{
		return data_.ExpInfos[lv].Bonus;
	}

	public int getBubbleSpeed(int lv)
	{
		return data_.ExpInfos[lv].BubbleSpeed;
	}

	public float getRankingBonus(int lv)
	{
		return data_.ExpInfos[lv].RankingBonus;
	}

	public bool getRewards(int lv, out Constant.Reward[] reward)
	{
		if (overLevelCap(lv))
		{
			reward = null;
			return false;
		}
		RewardInfo.Reward[] rewards = data_.ExpInfos[lv].Rewards;
		reward = new Constant.Reward[rewards.Length];
		for (int i = 0; i < rewards.Length; i++)
		{
			reward[i] = new Constant.Reward();
			reward[i].set((Constant.eMoney)rewards[i].Type, rewards[i].Num);
		}
		return false;
	}

	private bool overLevelCap(int lv)
	{
		if (lv <= 0 || lv > getLvCap())
		{
			return true;
		}
		return false;
	}

	public RewardInfo.Rate getCoinRate()
	{
		return data_.CoinRate;
	}

	public RewardInfo.Rate getExpRate()
	{
		return data_.ExpRate;
	}

	public int[] getCoinStarRates()
	{
		return data_.CoinStarRates;
	}

	public int getRate(RewardInfo.Rate data, eRate type)
	{
		int result = 0;
		switch (type)
		{
		case eRate.Clear:
			result = data.Clear;
			break;
		case eRate.Failed:
			result = data.Failed;
			break;
		case eRate.AlreadyClear:
			result = data.AlreadyClear;
			break;
		case eRate.AlreadyFailed:
			result = data.AlreadyFailed;
			break;
		}
		return result;
	}
}
