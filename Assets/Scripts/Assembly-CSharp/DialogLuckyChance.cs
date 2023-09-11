using System;
using System.Collections;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using TnkAd;
using Toast.Analytics;
using UnityEngine;

public class DialogLuckyChance : DialogBase
{
	private enum eConnect
	{
		Load = 0,
		Success = 1,
		Cancel = 2,
		Failed = 3
	}

	private eConnect connectStatus_;

	private bool bBuying_;

	private Network.JewelShopInfo luckyChanceInfo_;

	private DialogCommon errorDialog_;

	private bool bError;

	public override void OnCreate()
	{
		errorDialog_ = dialogManager_.getDialog(DialogManager.eDialog.NetworkError) as DialogCommon;
	}

	public IEnumerator show()
	{
		Input.enable = false;
		connectStatus_ = eConnect.Load;
		GKUnityPluginController.GK_CheckDevice(CheckDeviceCB);
		if (connectStatus_ == eConnect.Load)
		{
			yield return null;
		}
		Input.enable = true;
		if (connectStatus_ == eConnect.Failed)
		{
			yield return dialogManager_.StartCoroutine(ShowErrorDialog(500008));
			yield break;
		}
		ShopDataTable shopTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ShopDataTable>();
		yield return dialogManager_.StartCoroutine(shopTbl.download(Constant.eShop.Jewel, dialogManager_));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield break;
		}
		JewelShopData shopData = shopTbl.getJewelData();
		Network.JewelShopInfo[] jewelShopList = shopData.jewelShopList;
		Network.JewelShopInfo[] jewelLuckyChanceList = shopData.jewelLuckyChanceList;
		int overCount = 0;
		int jewel = Bridge.PlayerData.getJewel();
		int j = jewelLuckyChanceList.Length - 1;
		while (j >= 0 && jewelLuckyChanceList[j].num + jewel > Constant.JewelMax)
		{
			overCount++;
			j--;
		}
		if (jewelLuckyChanceList.Length - overCount < 1)
		{
			yield break;
		}
		int index = UnityEngine.Random.Range(0, jewelLuckyChanceList.Length - overCount);
		luckyChanceInfo_ = jewelLuckyChanceList[index];
		int shopNum2 = luckyChanceInfo_.num;
		int shopIndex = 0;
		for (int i = 0; i < jewelShopList.Length; i++)
		{
			if (luckyChanceInfo_.price == jewelShopList[i].price)
			{
				shopIndex = i;
				shopNum2 = jewelShopList[i].num;
				break;
			}
		}
		MessageResource msgRes = MessageResource.Instance;
		UILabel Caption2 = base.transform.Find("window/Caption2").GetComponent<UILabel>();
		UILabel Caption3 = base.transform.Find("window/Caption3").GetComponent<UILabel>();
		UILabel Label = base.transform.Find("window/BuyButton/Label").GetComponent<UILabel>();
		if (ResourceLoader.Instance.isJapanResource())
		{
			Caption3.text = string.Empty + luckyChanceInfo_.num + "루비";
		}
		else
		{
			Caption3.text = string.Empty + luckyChanceInfo_.num + "rubies";
		}
		Caption2.text = msgRes.castCtrlCode(msgRes.getMessage(2481), 1, luckyChanceInfo_.bonus.ToString());
		double price = luckyChanceInfo_.price;
		string msg = MessageResource.Instance.getMessage(62);
		Label.text = msgRes.castCtrlCode(message: (price - Math.Floor(price) == 0.0) ? MessageResource.Instance.castCtrlCode(msg, 1, price.ToString("N0")) : MessageResource.Instance.castCtrlCode(msg, 1, price.ToString()), src: msgRes.getMessage(2482), index: 1);
		UISprite icon_jewel = base.transform.Find("window/icons/icon_jewel").GetComponent<UISprite>();
		icon_jewel.spriteName = "jewelshop_icon_00" + Mathf.Clamp(shopIndex, 0, 4);
		icon_jewel.MakePixelPerfect();
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "BuyButton":
			Constant.SoundUtil.PlayDecideSE();
			bError = false;
			yield return dialogManager_.StartCoroutine(buy());
			Input.forceEnable();
			if (!bError)
			{
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			}
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
		Bridge.PlayerData.setBuyJewel(Bridge.PlayerData.getBuyJewel() + luckyChanceInfo_.buy);
		Bridge.PlayerData.setBonusJewel(Bridge.PlayerData.getBonusJewel() + luckyChanceInfo_.bonus);
	}

	private IEnumerator addJewel(string signature = "", string data = "")
	{
		addJewelLocal();
		Partytrack.sendPayment(luckyChanceInfo_.productId, 1, "KRW", luckyChanceInfo_.price);
		Tapjoy.TrackPurchase(luckyChanceInfo_.productId, "KRW", luckyChanceInfo_.price);
		Plugin.Instance.buyCompleted(luckyChanceInfo_.productId);
		GameAnalytics.tracePurchase(luckyChanceInfo_.productId, (float)luckyChanceInfo_.price, (float)luckyChanceInfo_.price / (float)luckyChanceInfo_.num, "KRW", Bridge.PlayerData.getCurrentStage());
		Tapjoy.TrackEvent("Money", "Income Jewel", "Lucky Chance Jewel", null, luckyChanceInfo_.num);
		Tapjoy.TrackEvent("PurchaseJewel", "Lucky Jewel", luckyChanceInfo_.productId, null, "KRW", (long)luckyChanceInfo_.price, "Jewel", luckyChanceInfo_.num, "PlayStore", (long)luckyChanceInfo_.price);
		GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Jewel", "Lucky Chance Jewel", luckyChanceInfo_.num);
		GlobalGoogleAnalytics.Instance.LogEvent("Lucky Jewel", luckyChanceInfo_.productId, "GooglePlayStore", (long)luckyChanceInfo_.price);
		NetworkMng.Instance.setup(Hash.BuyJewel(luckyChanceInfo_, signature, data));
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

	private IEnumerator buy()
	{
		Input.enable = false;
		bBuying_ = true;
		connectStatus_ = eConnect.Load;
		GKUnityPluginController.GK_Payment_Purchase(luckyChanceInfo_.productId, PurchaseFinished);
		while (connectStatus_ == eConnect.Load)
		{
			yield return null;
		}
		if (connectStatus_ != eConnect.Success)
		{
			Input.enable = true;
			bBuying_ = false;
			bError = true;
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
		Input.enable = true;
		MessageResource msgRes = MessageResource.Instance;
		string msg = msgRes.castCtrlCode(msgRes.getMessage(63), 1, luckyChanceInfo_.num.ToString());
		DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Common).GetComponent<DialogCommon>();
		dialog.setup(msg, null, null, true);
		dialog.setButtonActive(DialogCommon.eBtn.Close, false);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
		while (dialog.isOpen())
		{
			yield return 0;
		}
	}

	protected IEnumerator ShowErrorDialog(int index)
	{
		string msg = MessageResource.Instance.getMessage(index);
		errorDialog_.setup(msg + "(" + GKUnityPluginController.Instance.ErrorCode + ")", null, null, true);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(errorDialog_));
		while (errorDialog_.isOpen())
		{
			yield return null;
		}
	}

	private void PurchaseFinished(string strResult)
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

	private void CheckDeviceCB(string result)
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
