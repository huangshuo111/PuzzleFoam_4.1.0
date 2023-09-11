using System.Collections;
using System.Collections.Generic;
using Bridge;
using Network;
using UnityEngine;

public class DebugMenuNetwork : DebugMenuBase
{
	private enum eItem
	{
		Reset = 0,
		MyData = 1,
		SetLevel = 2,
		SetExp = 3,
		SetJewel = 4,
		SetCoin = 5,
		SetHeart = 6,
		AddStar = 7,
		ResultCode = 8,
		Max = 9
	}

	private int jewel_;

	private int coin_;

	private int heart_;

	private int star_;

	private int exp_;

	private int level_;

	private int resultIdx_;

	private Dictionary<int, eResultCode> resultCodeDict_ = new Dictionary<int, eResultCode>
	{
		{
			0,
			eResultCode.Success
		},
		{
			1,
			eResultCode.NotExistRecord
		},
		{
			2,
			eResultCode.InvalidRequestMethod
		},
		{
			3,
			eResultCode.InvalidRequestParameter
		},
		{
			4,
			eResultCode.InvalidRequestHeader
		},
		{
			5,
			eResultCode.ErrorAesDecrypt
		},
		{
			6,
			eResultCode.Lock
		},
		{
			7,
			eResultCode.NotExistUserData
		},
		{
			8,
			eResultCode.InvalidJewel
		},
		{
			9,
			eResultCode.InvalidCoin
		},
		{
			10,
			eResultCode.InvalidHeart
		},
		{
			11,
			eResultCode.ShortageJewel
		},
		{
			12,
			eResultCode.ShortageCoin
		},
		{
			13,
			eResultCode.ShortageHeart
		},
		{
			14,
			eResultCode.NotStageClear
		},
		{
			15,
			eResultCode.NotExistStageData
		},
		{
			16,
			eResultCode.NotStageStart
		},
		{
			17,
			eResultCode.NotExistJewelItem
		},
		{
			18,
			eResultCode.NotExistCoinItem
		},
		{
			19,
			eResultCode.NotExistHeartItem
		},
		{
			20,
			eResultCode.NotExistStageItem
		},
		{
			21,
			eResultCode.InvalidTreasure
		},
		{
			22,
			eResultCode.OpendTreasure
		},
		{
			23,
			eResultCode.InvalidHeartMail
		},
		{
			24,
			eResultCode.NotExistsMaile
		},
		{
			25,
			eResultCode.InvalidRankingReward
		},
		{
			26,
			eResultCode.AlreadyInvite
		},
		{
			27,
			eResultCode.NotExistsContinue
		},
		{
			28,
			eResultCode.NotExistsReplay
		},
		{
			29,
			eResultCode.ErrorUnknown
		},
		{
			30,
			eResultCode.StopPlaying
		},
		{
			31,
			eResultCode.ErrorHSPServer
		},
		{
			32,
			eResultCode.NotSupportOS
		},
		{
			33,
			eResultCode.InvalidSessionID
		},
		{
			34,
			eResultCode.AddedReward
		},
		{
			35,
			eResultCode.NotExistReward
		},
		{
			36,
			eResultCode.LimitOverReward
		},
		{
			37,
			eResultCode.NotExistsReplay
		},
		{
			38,
			eResultCode.EventAlreadySent
		},
		{
			39,
			eResultCode.EventAlreadySentAll
		},
		{
			40,
			eResultCode.DailyLimitOverInvite
		},
		{
			41,
			eResultCode.LimitOverInvite
		}
	};

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(9, "Network"));
		if (Bridge.PlayerData.isInstance())
		{
			exp_ = Bridge.PlayerData.getExp();
			level_ = Bridge.PlayerData.getLevel();
			jewel_ = Bridge.PlayerData.getBonusJewel();
			coin_ = Bridge.PlayerData.getCoin();
			heart_ = Bridge.PlayerData.getHeart();
			star_ = Bridge.StageData.getTotalStar();
		}
	}

	public override void OnDraw()
	{
		DrawItem(0, "Reset", eItemType.CenterOnly);
		DrawItem(1, "MyData", eItemType.CenterOnly);
		DrawItem(4, "Jewel : " + jewel_);
		DrawItem(5, "Coin : " + coin_);
		DrawItem(6, "Heart : " + heart_);
		DrawItem(2, "Level: " + level_);
		DrawItem(3, "Exp : " + exp_);
		DrawItem(7, "Star : " + star_);
		DrawItem(8, "Result : " + resultCodeDict_[resultIdx_]);
	}

	public override void OnExecute()
	{
		if (IsPressCenterButton(0))
		{
			PlayerPrefs.DeleteAll();
			StartCoroutine(NetworkMng.Instance.download(Reset, true, false));
		}
		if (IsPressCenterButton(1))
		{
		}
		jewel_ = (int)Vary(4, jewel_, 1, 0, Constant.JewelMax);
		if (IsPressCenterButton(4))
		{
			StartCoroutine(NetworkMng.Instance.download(SetJewel, true, false));
		}
		coin_ = (int)Vary(5, coin_, 1000, 0, Constant.CoinMax);
		if (IsPressCenterButton(5))
		{
			StartCoroutine(NetworkMng.Instance.download(SetCoin, true, false));
		}
		heart_ = (int)Vary(6, heart_, 1, 0, Constant.HeartMax);
		if (IsPressCenterButton(6))
		{
			StartCoroutine(NetworkMng.Instance.download(SetHeart, true, false));
		}
		level_ = (int)Vary(2, level_, 1, 0, 99);
		if (IsPressCenterButton(2))
		{
			StartCoroutine(NetworkMng.Instance.download(SetLv, true, false));
		}
		exp_ = (int)Vary(3, exp_, 1, 0, 999999);
		if (IsPressCenterButton(3))
		{
			StartCoroutine(NetworkMng.Instance.download(SetExp, true, false));
		}
		star_ = (int)Vary(7, star_, 1, 0, Constant.StarMax * GlobalData.Instance.getNormalStageNum());
		if (IsPressCenterButton(7))
		{
			StartCoroutine(NetworkMng.Instance.download(AddStar, true, false));
		}
		resultIdx_ = (int)Vary(8, resultIdx_, 1, 0, resultCodeDict_.Keys.Count - 1);
		if (IsPressCenterButton(8))
		{
			NetworkMng.Instance.setup(null);
			StartCoroutine(NetworkMng.Instance.download(SendResultCode, true, false));
		}
	}

	private WWW Reset(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		return WWWWrap.create("debug/reset/");
	}

	private WWW AddStar(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("star", star_);
		return WWWWrap.create("debug/staradd/");
	}

	private WWW SetExp(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("exp", exp_);
		return WWWWrap.create("debug/expset/");
	}

	private WWW SetJewel(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("jewel", jewel_);
		return WWWWrap.create("debug/jewelset/");
	}

	private WWW SetHeart(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("heart", heart_);
		return WWWWrap.create("debug/heartset/");
	}

	private WWW SetCoin(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("coin", coin_);
		return WWWWrap.create("debug/coinset/");
	}

	private WWW SetLv(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("lv", level_);
		return WWWWrap.create("debug/lvset/");
	}

	private WWW SendResultCode(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("resultCode", (int)resultCodeDict_[resultIdx_]);
		return WWWWrap.create("debug/resultcodetest/");
	}
}
