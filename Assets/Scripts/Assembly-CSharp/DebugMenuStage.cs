using System.Collections;
using UnityEngine;

public class DebugMenuStage : DebugMenuBase
{
	private enum eItem
	{
		Clear = 0,
		Gameover = 1,
		Score = 2,
		Coin = 3,
		BonusCoin = 4,
		BonusJewel = 5,
		ShotCount = 6,
		Max = 7
	}

	private Part_Stage part_;

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(7, "Stage"));
		part_ = Object.FindObjectOfType(typeof(Part_Stage)) as Part_Stage;
	}

	public override void OnExecute()
	{
		if (part_ != null)
		{
			if (IsPressCenterButton(0))
			{
				part_.debugClear();
			}
			if (IsPressCenterButton(1))
			{
				part_.debugGameover();
			}
			part_.totalScore = (int)Vary(2, part_.totalScore, 100, 0, Constant.UserScoreMax);
			part_.totalCoin = (int)Vary(3, part_.totalCoin, 10, 0, Constant.CoinMax);
			part_.bonusCoin = (int)Vary(4, part_.bonusCoin, 10, 0, Constant.CoinMax);
			part_.bonusJewel = (int)Vary(5, part_.bonusJewel, 1, 0, Constant.BonusJewelMax);
			part_.shotCount = (int)Vary(6, part_.shotCount, 1, 0, 99);
		}
	}

	public override void OnDraw()
	{
		if (part_ != null)
		{
			DrawItem(0, "Clear", eItemType.CenterOnly);
			DrawItem(1, "Gameover", eItemType.CenterOnly);
			DrawItem(2, "Score : " + part_.totalScore);
			DrawItem(3, "Coin : " + part_.totalCoin);
			DrawItem(4, "BonusCoin : " + part_.bonusCoin);
			DrawItem(5, "BonusJewel : " + part_.bonusJewel);
			DrawItem(6, "ShotCount : " + part_.shotCount);
		}
	}
}
