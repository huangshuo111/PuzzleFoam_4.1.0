using System;
using System.Collections;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using TnkAd;
using Toast.Analytics;
using UnityEngine;

public class DialogCoinShop : DialogShopBase
{
	private int setupItemCoin_;

	[NonSerialized]
	public bool isCoinCampaign_;

	protected override int getCommonMsgID()
	{
		return 8;
	}

	public override void OnCreate()
	{
		init(Constant.eShop.Coin);
		UIDraggablePanel componentInChildren = GetComponentInChildren<UIDraggablePanel>();
		componentInChildren.enabled = false;
	}

	protected override void showInit()
	{
		CoinShopData coinData = shopTbl_.getCoinData();
		isCoinCampaign_ = coinData.isCoinCampaign;
		if (partManager_.currentPart == PartManager.ePart.Map || partManager_.currentPart == PartManager.ePart.EventMap || partManager_.currentPart == PartManager.ePart.CollaborationMap)
		{
			GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>().coinCampaign_.SetActive(isCoinCampaign_);
		}
		GameData gameData = GlobalData.Instance.getGameData();
		gameData.isCoinCampaign = isCoinCampaign_;
	}

	protected override IEnumerator showCB()
	{
		dialogManager_.StartCoroutine(showBanner(isCoinCampaign_));
		yield break;
	}

	protected override IEnumerator buy()
	{
		int coin = Bridge.PlayerData.getCoin();
		setupItemCoin_ = 0;
		if (partManager_.currentPart == PartManager.ePart.Map)
		{
			DialogSetup setup = dialogManager_.getDialog(DialogManager.eDialog.Setup) as DialogSetup;
			if (setup.isOpen())
			{
				setupItemCoin_ = setup.getSetItemCoin();
			}
		}
		else if (partManager_.currentPart == PartManager.ePart.CollaborationMap)
		{
			DialogCollaborationSetup setup2 = dialogManager_.getDialog(DialogManager.eDialog.CollaborationSetup) as DialogCollaborationSetup;
			if (setup2 != null && setup2.isOpen())
			{
				setupItemCoin_ = setup2.getSetItemCoin();
			}
		}
		else if (partManager_.currentPart == PartManager.ePart.EventMap)
		{
			DialogEventSetup eventSetup = dialogManager_.getDialog(DialogManager.eDialog.EventSetup) as DialogEventSetup;
			if (eventSetup != null && eventSetup.isOpen())
			{
				setupItemCoin_ = eventSetup.getSetItemCoin();
			}
		}
		NetworkMng.Instance.setup(Hash.BuyCoin(buyItem_));
		yield return StartCoroutine(NetworkMng.Instance.download(API.BuyCoin, true));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		JsonData json = JsonMapper.ToObject(www.text);
		GameData gameData = GlobalData.Instance.getGameData();
		gameData.bonusJewel = (int)json["bonusJewel"];
		gameData.buyJewel = (int)json["buyJewel"];
		gameData.coin = (int)json["coin"] - setupItemCoin_;
		Tapjoy.TrackEvent("Money", "Income Coin", "Coin Shop", null, buyItem_.getNum());
		Tapjoy.TrackEvent("Money", "Expense Jewel", "Coin Shop", null, (int)buyItem_.getPrice());
		Tapjoy.TrackEvent("PurchaseCoin", "Coin", "COIN" + buyItem_.getNum(), null, "Use Jewel", (long)buyItem_.getPrice(), "Get Coin", buyItem_.getNum(), null, 0L);
		GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Coin Shop", buyItem_.getNum());
		GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Jewel", "Coin Shop", (int)buyItem_.getPrice());
		GlobalGoogleAnalytics.Instance.LogEvent("Buy Coin", buyItem_.getProductID(), buyItem_.getNum() + "Coin", (long)buyItem_.getPrice());
		GameAnalytics.traceMoneyConsumption("USE_JEWEL", "0", buyItem_.getPrice(), Bridge.PlayerData.getCurrentStage());
		GameAnalytics.traceMoneyAcquisition("BUY_COIN", "1", buyItem_.getNum(), Bridge.PlayerData.getCurrentStage());
		Plugin.Instance.buyCompleted("BUY_COIN" + buyItem_.getNum());
		updateOtherLabel();
		mainMenu_.update();
		yield return dialogManager_.StartCoroutine(base.openFinishDialog(buyItem_.getShopType(), buyItem_.getNum()));
		coin = Bridge.PlayerData.getCoin();
		coin += setupItemCoin_;
		DialogAvatarGacha gacha = dialogManager_.getDialog(DialogManager.eDialog.AvatarGacha) as DialogAvatarGacha;
		DialogAvatarLevelup levelup = dialogManager_.getDialog(DialogManager.eDialog.AvatarLevelup) as DialogAvatarLevelup;
		if (gacha != null && levelup != null)
		{
			if (gacha.isOpen())
			{
				gacha.UpdatePlusButtonUi();
			}
			if (levelup.isOpen())
			{
				levelup.UpdatePlusButtonUi();
			}
		}
	}
}
