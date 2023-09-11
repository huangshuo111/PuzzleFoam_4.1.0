using System.Collections;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using TnkAd;
using Toast.Analytics;
using UnityEngine;

public class DialogHeartShop : DialogShopBase
{
	private GameObject heartSaleDisp;

	private UILabel saleHeartCount;

	public bool isSale_;

	protected override int getCommonMsgID()
	{
		return 7;
	}

	public override void OnCreate()
	{
		init(Constant.eShop.Heart);
	}

	protected override void showInit()
	{
		heartSaleDisp = base.transform.Find("window/item000/sale_pos").gameObject;
		HeartShopData heartData = shopTbl_.getHeartData();
		isSale_ = heartData.isHeartShopCampaign;
		heartSaleDisp.SetActive(isSale_);
		if (partManager_.currentPart == PartManager.ePart.Map || partManager_.currentPart == PartManager.ePart.EventMap || partManager_.currentPart == PartManager.ePart.CollaborationMap)
		{
			GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>().heartShopCampaign_.SetActive(isSale_);
		}
		GameData gameData = GlobalData.Instance.getGameData();
		gameData.isHeartShopCampaign = isSale_;
		if (heartSaleDisp.activeSelf)
		{
			saleHeartCount = heartSaleDisp.transform.Find("Number_Label_sale").GetComponent<UILabel>();
			saleHeartCount.text = shopItems_[0].getNum().ToString();
		}
	}

	protected override IEnumerator OnButton(GameObject trigger)
	{
		if (trigger.name == "Request_Button")
		{
			Constant.SoundUtil.PlayButtonSE();
			DialogRequest dialog = dialogManager_.getDialog(DialogManager.eDialog.Request) as DialogRequest;
			yield return dialogManager_.StartCoroutine(dialog.show());
		}
		else
		{
			yield return dialogManager_.StartCoroutine(base.OnButton(trigger));
		}
	}

	protected override IEnumerator buy()
	{
		int heart = Bridge.PlayerData.getHeart();
		NetworkMng.Instance.setup(Hash.BuyHeart(buyItem_));
		yield return StartCoroutine(NetworkMng.Instance.download(API.BuyHeart, true));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			JsonData json = JsonMapper.ToObject(www.text);
			GameData gameData = GlobalData.Instance.getGameData();
			gameData.bonusJewel = (int)json["bonusJewel"];
			gameData.buyJewel = (int)json["buyJewel"];
			gameData.heart = (int)json["heart"];
			Tapjoy.TrackEvent("PurchaseHeart", "Heart", "Heart", null, 0L);
			Tapjoy.TrackEvent("Money", "Expense Jewel", "Buy Heart", null, (int)buyItem_.getPrice());
			GameAnalytics.traceMoneyConsumption("USE_JEWEL", "0", buyItem_.getPrice(), Bridge.PlayerData.getCurrentStage());
			GameAnalytics.traceMoneyAcquisition("BUY_HEART", "1", buyItem_.getPrice(), Bridge.PlayerData.getCurrentStage());
			Plugin.Instance.buyCompleted("BUY_HEART" + buyItem_.getPrice());
			GlobalGoogleAnalytics.Instance.LogEvent("Buy Heart", "Heart", "5 Heart", (long)buyItem_.getPrice());
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Jewel", "Buy Heart", (int)buyItem_.getPrice());
			mainMenu_.update();
			yield return dialogManager_.StartCoroutine(base.openFinishDialog(buyItem_.getShopType(), buyItem_.getNum()));
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
		}
	}
}
