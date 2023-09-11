using System;
using System.Collections;
using System.Collections.Generic;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using TnkAd;
using Toast.Analytics;
using UnityEngine;

public class DialogJewelShop : DialogShopBase
{
	protected enum eConnect
	{
		Load = 0,
		Success = 1,
		Failed = 2,
		Cancel = 3
	}

	protected eConnect connectStatus_;

	protected bool bBuying_;

	private GameObject hotIcon_;

	private List<Transform> hotIconLocator_ = new List<Transform>();

	protected Network.JewelShopInfo[] shopInfo_;

	[NonSerialized]
	public bool isJewelCampaign_;

	protected static bool bBuyState;

	public override void OnCreate()
	{
		hotIcon_ = base.transform.Find("icon_hot").gameObject;
		Transform transform = base.transform.Find("DragPanel/contents");
		for (int i = 0; i < transform.childCount; i++)
		{
			hotIconLocator_.Add(transform.GetChild(i).Find("hot_pos"));
		}
		base.transform.Find("Button_1").gameObject.SetActive(false);
		base.transform.Find("Button_2").gameObject.SetActive(false);
		init(Constant.eShop.Jewel);
	}

	protected override void showInit()
	{
		JewelShopData jewelData = shopTbl_.getJewelData();
		isJewelCampaign_ = jewelData.isJewelCampaign;
		if (partManager_.currentPart == PartManager.ePart.Map)
		{
			GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>().jewelCampaign_.SetActive(isJewelCampaign_);
		}
		if (partManager_.currentPart == PartManager.ePart.Stage)
		{
			GameData gameData = GlobalData.Instance.getGameData();
			gameData.isJewelCampaign = isJewelCampaign_;
			DialogStageShop dialogStageShop = dialogManager_.getDialog(DialogManager.eDialog.StageShop) as DialogStageShop;
			if (dialogStageShop.isOpen())
			{
				dialogStageShop.updateCampaign(gameData.isJewelCampaign);
			}
			DialogContinue dialogContinue = dialogManager_.getDialog(DialogManager.eDialog.Continue) as DialogContinue;
			if (dialogContinue.isOpen())
			{
				dialogContinue.updateCampaign(gameData.isJewelCampaign);
			}
		}
		if (!isJewelCampaign_)
		{
			shopInfo_ = jewelData.jewelShopList;
		}
		else
		{
			shopInfo_ = jewelData.jewelSaleList;
		}
	}

	protected override IEnumerator showCB()
	{
		hotIcon_.SetActive(false);
		for (int i = 0; i < shopInfo_.Length; i++)
		{
			if (shopInfo_[i].isHot)
			{
				hotIcon_.SetActive(true);
				Utility.setParent(hotIcon_, hotIconLocator_[i], false);
				break;
			}
		}
		dialogManager_.StartCoroutine(showBanner(isJewelCampaign_));
		yield break;
	}

	protected override IEnumerator OnButton(GameObject trigger)
	{
		if (trigger.name.Contains("Item"))
		{
			if (!bBuyState)
			{
				bBuyState = true;
				Constant.SoundUtil.PlayDecideSE();
				yield return dialogManager_.StartCoroutine(buy(trigger));
				bBuyState = false;
			}
			yield break;
		}
		switch (trigger.name)
		{
		case "Button_1":
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(WebView.Instance.show(WebView.eWebType.SCTL, dialogManager_));
			break;
		case "Button_2":
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(WebView.Instance.show(WebView.eWebType.EFTA, dialogManager_));
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	public bool isBuying()
	{
		return bBuying_;
	}

	private void addJewelLocal()
	{
		int num = 0;
		int num2 = 0;
		string productID = buyItem_.getProductID();
		JewelShopData jewelData = shopTbl_.getJewelData();
		for (int i = 0; i < shopInfo_.Length; i++)
		{
			Network.JewelShopInfo jewelShopInfo = shopInfo_[i];
			if (productID == jewelShopInfo.productId)
			{
				num = jewelShopInfo.bonus;
				num2 = jewelShopInfo.buy;
				break;
			}
		}
		Bridge.PlayerData.setBuyJewel(Bridge.PlayerData.getBuyJewel() + num2);
		Bridge.PlayerData.setBonusJewel(Bridge.PlayerData.getBonusJewel() + num);
	}

	protected virtual IEnumerator addJewel(string signature = "", string data = "")
	{
		addJewelLocal();
		Partytrack.sendPayment(buyItem_.getProductID(), 1, "KRW", buyItem_.getPrice());
		Tapjoy.TrackPurchase(buyItem_.getProductID(), "KRW", buyItem_.getPrice());
		Plugin.Instance.buyCompleted(buyItem_.getProductID());
		GameAnalytics.tracePurchase(buyItem_.getProductID(), (float)buyItem_.getPrice(), (float)buyItem_.getPrice() / (float)buyItem_.getNum(), "KRW", Bridge.PlayerData.getCurrentStage());
		Tapjoy.TrackEvent("Money", "Income Jewel", "Jewel Shop", null, buyItem_.getNum());
		Tapjoy.TrackEvent("PurchaseJewel", "Jewel", buyItem_.getProductID(), null, "KRW", (long)buyItem_.getPrice(), "Jewel", buyItem_.getNum(), "PlayStore", (long)buyItem_.getPrice());
		GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Jewel", "Jewel Shop", buyItem_.getNum());
		GlobalGoogleAnalytics.Instance.LogEvent("Buy Jewel", buyItem_.getProductID(), "GooglePlayStore", (long)buyItem_.getPrice());
		NetworkMng.Instance.setup(Hash.BuyJewel(buyItem_, signature, data));
		yield return StartCoroutine(NetworkMng.Instance.download(API.BuyJewel, true, true, true));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			JsonData json = JsonMapper.ToObject(www.text);
			GameData gameData = GlobalData.Instance.getGameData();
			gameData.bonusJewel = (int)json["bonusJewel"];
			gameData.buyJewel = (int)json["buyJewel"];
			connectStatus_ = eConnect.Success;
		}
		else
		{
			connectStatus_ = eConnect.Failed;
		}
	}

	protected virtual IEnumerator buy(GameObject trigger)
	{
		Input.enable = false;
		ShopItem item = trigger.transform.parent.GetComponent<ShopItem>();
		buyItem_ = item;
		bBuying_ = true;
		connectStatus_ = eConnect.Load;
		GKUnityPluginController.GK_Payment_Purchase(buyItem_.getProductID(), PurchaseFinished);
		while (connectStatus_ == eConnect.Load)
		{
			yield return null;
		}
		if (connectStatus_ != eConnect.Success)
		{
			Input.enable = true;
			bBuying_ = false;
			if (connectStatus_ == eConnect.Failed)
			{
				yield return partManager_.StartCoroutine(ShowErrorDialog(500008));
			}
			if (connectStatus_ == eConnect.Cancel)
			{
				yield return partManager_.StartCoroutine(ShowErrorDialog(305));
			}
			yield break;
		}
		bBuying_ = false;
		mainMenu_.update();
		Input.enable = true;
		yield return dialogManager_.StartCoroutine(base.openFinishDialog(buyItem_.getShopType(), buyItem_.getNum()));
		updateOtherLabel();
		int jewel = Bridge.PlayerData.getJewel();
		if (!isCanBuy(jewel, Constant.JewelMax))
		{
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			yield return dialogManager_.StartCoroutine(showLimitOverDialog());
			yield break;
		}
		updateShopItem(jewel, Constant.JewelMax);
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

	protected void PurchaseFinished(string strResult)
	{
		if (strResult.Equals(string.Empty))
		{
			connectStatus_ = eConnect.Failed;
			return;
		}
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (!Constant.JsonGetValue(strResult, "signing-status").Equals("0"))
		{
			if (Constant.JsonGetValue(strResult, "signing-status").Equals("-1005"))
			{
				connectStatus_ = eConnect.Cancel;
			}
			else
			{
				connectStatus_ = eConnect.Failed;
			}
		}
		else
		{
			empty = Constant.JsonGetValue(strResult, "purchase-info");
			empty2 = Constant.JsonGetValue(strResult, "signature");
			StartCoroutine(addJewel(empty2, empty));
		}
	}

	protected void CheckDeviceCB(string result)
	{
		if (result.Equals(string.Empty))
		{
			connectStatus_ = eConnect.Failed;
		}
		else if (Constant.JsonGetValue(result, "signing-status").Equals("0"))
		{
			connectStatus_ = eConnect.Success;
		}
		else
		{
			connectStatus_ = eConnect.Failed;
		}
	}

	protected void CulturelandPurchaseFinished(string strResult)
	{
		if (strResult.Equals(string.Empty))
		{
			connectStatus_ = eConnect.Failed;
			return;
		}
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (!Constant.JsonGetValue(strResult, "signing-status").Equals("0000"))
		{
			if (Constant.JsonGetValue(strResult, "signing-status").Equals("-1005"))
			{
				connectStatus_ = eConnect.Cancel;
			}
			else
			{
				connectStatus_ = eConnect.Failed;
			}
		}
		else
		{
			empty = Constant.JsonGetValue(strResult, "purchase-info");
			empty2 = Constant.JsonGetValue(strResult, "signature");
			StartCoroutine(addJewel(empty2, empty));
		}
	}
}
