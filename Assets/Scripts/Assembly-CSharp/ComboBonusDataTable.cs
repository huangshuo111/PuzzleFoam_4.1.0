using UnityEngine;

public class ComboBonusDataTable : MonoBehaviour
{
	public ComboBonusInfo info_;

	private bool bLoaded_;

	public void load()
	{
		if (!bLoaded_)
		{
			TextAsset textAsset = Resources.Load("Parameter/combo_bonus_info", typeof(TextAsset)) as TextAsset;
			info_ = Xml.DeserializeObject<ComboBonusInfo>(textAsset.text) as ComboBonusInfo;
			bLoaded_ = true;
		}
	}

	public bool isLoaded()
	{
		return bLoaded_;
	}

	public int getComboBonusCoinNum(int maxComboCount)
	{
		ComboBonusInfo.BonusInfo[] bonusInfos = info_.BonusInfos;
		foreach (ComboBonusInfo.BonusInfo bonusInfo in bonusInfos)
		{
			if (maxComboCount >= bonusInfo.CountMin && maxComboCount <= bonusInfo.CountMax)
			{
				return bonusInfo.CoinNum;
			}
		}
		return 0;
	}
}
