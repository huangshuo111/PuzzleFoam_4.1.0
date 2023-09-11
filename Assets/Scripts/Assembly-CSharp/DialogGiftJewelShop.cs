using System;
using System.Collections;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using TnkAd;
using Toast.Analytics;
using UnityEngine;

public class DialogGiftJewelShop : DialogJewelShop
{
	private ShopItem selectItem_;

	public string TargetUserID { get; set; }

	public string TargetUserName { get; set; }

	protected override void showInit()
	{
		JewelShopData jewelData = shopTbl_.getJewelData();
		isJewelCampaign_ = jewelData.isJewelCampaign;
		if (partManager_.currentPart == PartManager.ePart.Map)
		{
			GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>().jewelCampaign_.SetActive(isJewelCampaign_);
		}
		shopInfo_ = jewelData.jewelPresentList;
		isPresentshop_ = true;
	}

	public override IEnumerator show()
	{
		Input.enable = false;
		connectStatus_ = eConnect.Load;
		GKUnityPluginController.GK_CheckDevice(base.CheckDeviceCB);
		while (connectStatus_ == eConnect.Load)
		{
			yield return null;
		}
		Input.enable = true;
		if (connectStatus_ == eConnect.Failed)
		{
			yield return dialogManager_.StartCoroutine(ShowErrorDialog(500008));
			yield break;
		}
		DialogJewelShop.bBuyState = false;
		yield return dialogManager_.StartCoroutine(base.show());
	}

	protected override IEnumerator OnButton(GameObject trigger)
	{
		if (trigger.name.Contains("Item"))
		{
			if (!DialogJewelShop.bBuyState)
			{
				Constant.SoundUtil.PlayDecideSE();
				ShopItem item = (selectItem_ = trigger.transform.parent.GetComponent<ShopItem>());
				if (item.getPrice() - Math.Floor(item.getPrice()) != 0.0)
				{
					string price = MessageResource.Instance.getMessage(80);
					price = MessageResource.Instance.castCtrlCode(price, 1, item.getPrice().ToString());
				}
				else
				{
					string price = MessageResource.Instance.getMessage(62);
					price = MessageResource.Instance.castCtrlCode(price, 1, item.getPrice().ToString("N0"));
				}
				DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Present) as DialogCommon;
				string msg3 = MessageResource.Instance.getMessage(500017);
				msg3 = MessageResource.Instance.castCtrlCode(msg3, 1, TargetUserName);
				msg3 = MessageResource.Instance.castCtrlCode(msg3, 2, item.getNum().ToString("N0"));
				msg3 = MessageResource.Instance.castCtrlCode(msg3, 3, item.getReceiveHeartNum().ToString("N0"));
				Debug.Log("TargetID = " + TargetUserID);
				Debug.Log("TargetName = " + TargetUserName);
				dialog.setup(msg3, OnDecide_Present, null, true);
				yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
			}
		}
		else
		{
			switch (trigger.name)
			{
			case "Close_Button":
				Constant.SoundUtil.PlayCancelSE();
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
				break;
			}
		}
	}

	private IEnumerator OnDecide_Present()
	{
		DialogJewelShop.bBuyState = true;
		yield return dialogManager_.StartCoroutine(buy_giftjewel(selectItem_));
		DialogJewelShop.bBuyState = false;
	}

	private IEnumerator buy_giftjewel(ShopItem item)
	{
		Input.enable = false;
		buyItem_ = item;
		bBuying_ = true;
		connectStatus_ = eConnect.Load;
		Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
		GKUnityPluginController.GK_Payment_Purchase(buyItem_.getProductID(), base.PurchaseFinished);
		Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
		while (connectStatus_ == eConnect.Load)
		{
			yield return null;
		}
		Debug.Log("#############################");
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
		}
		else
		{
			Debug.Log("??????????");
			bBuying_ = false;
			Input.enable = true;
			yield return dialogManager_.StartCoroutine(openFinishDialog());
		}
	}

	protected IEnumerator openFinishDialog()
	{
		int msgID = 500018;
		string msg2 = MessageResource.Instance.getMessage(msgID);
		msg2 = MessageResource.Instance.castCtrlCode(msg2, 1, TargetUserName);
		msg2 = MessageResource.Instance.castCtrlCode(msg2, 2, selectItem_.getNum().ToString());
		commonDialog_.setup(msg2, null, null, true);
		commonDialog_.setButtonActive(DialogCommon.eBtn.Close, false);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(commonDialog_));
		while (commonDialog_.isOpen())
		{
			yield return 0;
		}
	}

	protected override IEnumerator addJewel(string signature = "", string data = "")
	{
		Partytrack.sendPayment(buyItem_.getProductID(), 1, "KRW", buyItem_.getPrice());
		Tapjoy.TrackPurchase(buyItem_.getProductID(), "KRW", buyItem_.getPrice());
		Plugin.Instance.buyCompleted(buyItem_.getProductID());
		GameAnalytics.tracePurchase(buyItem_.getProductID(), (float)buyItem_.getPrice(), (float)buyItem_.getPrice() / (float)buyItem_.getNum(), "KRW", Bridge.PlayerData.getCurrentStage());
		Tapjoy.TrackEvent("PurchaseJewel", "Gift Jewel", buyItem_.getProductID(), null, "KRW", (long)buyItem_.getPrice(), "Jewel", buyItem_.getNum(), "PlayStore", (long)buyItem_.getPrice());
		GlobalGoogleAnalytics.Instance.LogEvent("Gift Jewel", buyItem_.getProductID(), "GooglePlayStore", (long)buyItem_.getPrice());
		Debug.Log("TargetUserID!!! = " + TargetUserID);
		NetworkMng.Instance.setup(Hash.PresentJewel(buyItem_, TargetUserID, signature, data));
		Debug.Log("NetWork HashSetting - Hash.PresentJewel");
		yield return StartCoroutine(NetworkMng.Instance.download(API.PresentJewel, true, true, true));
		Debug.Log("NetWork SendMessage!!! API.PresentJewel");
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			JsonData json = JsonMapper.ToObject(www.text);
			connectStatus_ = eConnect.Success;
		}
		else
		{
			connectStatus_ = eConnect.Failed;
		}
		Debug.Log("AddJewel End!!!");
	}
}
