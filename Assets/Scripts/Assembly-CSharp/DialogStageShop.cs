using System.Collections;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using TnkAd;
using Toast.Analytics;
using UnityEngine;

public class DialogStageShop : DialogShortageBase
{
	private enum eLabel
	{
		Help = 0,
		Name = 1,
		Price = 2,
		Jewel = 3,
		Coin = 4,
		Max = 5
	}

	private enum eIcon
	{
		Jewel = 0,
		Coin = 1,
		Max = 2
	}

	private enum eButton
	{
		Buy = 0,
		UseItem = 1,
		Max = 2
	}

	private UILabel[] labels_ = new UILabel[5];

	private GameObject[] plusObjecats_ = new GameObject[2];

	private BoostItem item_;

	private MessageResource msgRes_;

	private StageDataTable stageDatas_;

	private StageInfo.Info stageInfo_;

	private int stageNo_;

	private GameObject campaignObject;

	private GameObject saleObject;

	private UIButton[] buttons = new UIButton[2];

	private GameObject price_;

	private StageInfo.Item itemInfo_;

	public static bool bReload_;

	private StageBoostItem buyItem_;

	public override void OnCreate()
	{
		createCB();
		Transform[] componentsInChildren = base.transform.GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			switch (transform.name)
			{
			case "item":
				item_ = transform.GetComponent<BoostItem>();
				break;
			case "Label_Name":
				labels_[1] = transform.GetComponent<UILabel>();
				break;
			case "Label_Help":
				labels_[0] = transform.GetComponent<UILabel>();
				break;
			case "Label_Jewel":
				labels_[3] = transform.GetComponent<UILabel>();
				break;
			case "Label_coin":
				labels_[4] = transform.GetComponent<UILabel>();
				break;
			case "01_coin":
				plusObjecats_[1] = transform.gameObject;
				break;
			case "02_jewel":
				plusObjecats_[0] = transform.gameObject;
				break;
			case "Buy_Button":
				buttons[0] = transform.GetComponent<UIButton>();
				break;
			case "ItemUse_Button":
				buttons[1] = transform.GetComponent<UIButton>();
				break;
			case "price":
				price_ = transform.gameObject;
				break;
			case "campaign":
				campaignObject = transform.gameObject;
				updateCampaign();
				break;
			case "sale":
				saleObject = transform.gameObject;
				updateSale();
				break;
			}
		}
		msgRes_ = MessageResource.Instance;
	}

	public IEnumerator show(int stageNo, StageBoostItem item, StageInfo.CommonInfo commonInfo, bool _timeStop)
	{
		bReload_ = false;
		stageNo_ = stageNo;
		Constant.Item.eType itemType = item.getItemType();
		int num = item.getNum();
		item_.setup(itemType, num);
		buyItem_ = item;
		stageDatas_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
		stageInfo_ = stageDatas_.getInfo(stageNo);
		GameData gameData = GlobalData.Instance.getGameData();
		labels_[1].text = msgRes_.getMessage((int)(itemType - 1 + 1000));
		string msg = msgRes_.getMessage((int)(itemType - 1 + 1200));
		if (msgRes_.isCtrlCode(msg, 1))
		{
			msg = msgRes_.castCtrlCode(msg, 1, num.ToString());
		}
		labels_[0].text = msg;
		if (Utility.getStringLine(msg) >= 4)
		{
			labels_[0].transform.localScale = new Vector3(24f, 24f, 1f);
		}
		else
		{
			labels_[0].transform.localScale = new Vector3(30f, 30f, 1f);
		}
		updateLabel();
		StageInfo.Item itemInfo = (itemInfo_ = getItemInfo(itemType, commonInfo));
		if (gameData.isStageItemAreaCampaign)
		{
			if (gameData.saleStageItemArea != null)
			{
				updateSale(false);
				int ItemPrice2 = itemInfo.Price;
				ItemPrice2 = itemInfo.Price * gameData.stageItemAreaSalePercent / 100;
				if (ItemPrice2 <= 0)
				{
					ItemPrice2 = 1;
				}
				int[] saleStageItemArea = gameData.saleStageItemArea;
				foreach (int sale_area in saleStageItemArea)
				{
					if (stageNo < 10000)
					{
						if (sale_area == stageInfo_.Area)
						{
							updateSale();
							item_.setPrice((Constant.eMoney)itemInfo.PriceType, ItemPrice2);
							break;
						}
						item_.setPrice((Constant.eMoney)itemInfo.PriceType, itemInfo.Price);
						continue;
					}
					if (stageNo > 10000 && stageNo < 20000 && sale_area == 10000)
					{
						updateSale();
						item_.setPrice((Constant.eMoney)itemInfo.PriceType, ItemPrice2);
						break;
					}
					if (Constant.ParkStage.isParkStage(stageNo) && stageInfo_.Area + 500000 == sale_area)
					{
						updateSale();
						item_.setPrice((Constant.eMoney)itemInfo.PriceType, ItemPrice2);
						break;
					}
					if (stageNo > 30000 && stageNo < 40000 && sale_area == 30000)
					{
						updateSale();
						item_.setPrice((Constant.eMoney)itemInfo.PriceType, ItemPrice2);
						break;
					}
					if (stageNo > 40000 && stageNo < 500000 && sale_area >= 40000 && sale_area < 500000)
					{
						updateSale();
						item_.setPrice((Constant.eMoney)itemInfo.PriceType, ItemPrice2);
						break;
					}
					item_.setPrice((Constant.eMoney)itemInfo.PriceType, itemInfo.Price);
				}
			}
		}
		else
		{
			updateSale(false);
			item_.setPrice((Constant.eMoney)itemInfo.PriceType, itemInfo.Price);
		}
		GameObject[] array = plusObjecats_;
		foreach (GameObject obj in array)
		{
			obj.SetActive(false);
		}
		if (itemInfo.PriceType == 2)
		{
			plusObjecats_[0].gameObject.SetActive(true);
		}
		else
		{
			plusObjecats_[1].gameObject.SetActive(true);
		}
		updateCampaign();
		if (_timeStop)
		{
			plusObjecats_[0].gameObject.SetActive(false);
			buttons[0].gameObject.SetActive(false);
			price_.gameObject.SetActive(false);
			buttons[1].gameObject.SetActive(true);
		}
		else
		{
			plusObjecats_[0].gameObject.SetActive(true);
			buttons[0].gameObject.SetActive(true);
			price_.gameObject.SetActive(true);
			buttons[1].gameObject.SetActive(false);
		}
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
	}

	private StageInfo.Item getItemInfo(Constant.Item.eType itemType, StageInfo.CommonInfo commonInfo)
	{
		StageInfo.Item[] stageItems = commonInfo.StageItems;
		foreach (StageInfo.Item item in stageItems)
		{
			if (item.Type == (int)itemType)
			{
				return item;
			}
		}
		return null;
	}

	public void updateLabel()
	{
		int jewel = Bridge.PlayerData.getJewel();
		int coin = Bridge.PlayerData.getCoin();
		labels_[3].text = jewel.ToString();
		labels_[4].text = coin.ToString("N0");
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		if (trigger.name == "PlusButton")
		{
			StartCoroutine(pressPlusButton(trigger));
			yield break;
		}
		switch (trigger.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "Buy_Button":
			Constant.SoundUtil.PlayDecideSE();
			yield return dialogManager_.StartCoroutine(buy());
			if (NetworkMng.Instance.getResultCode() == eResultCode.NotExistStageItem)
			{
				bReload_ = true;
				yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
				yield return StartCoroutine(updateSaleStageItemData());
				yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			}
			break;
		case "ItemUse_Button":
			Constant.SoundUtil.PlayDecideSE();
			dialogManager_.StartCoroutine(use());
			break;
		}
	}

	private IEnumerator pressPlusButton(GameObject trigger)
	{
		Constant.SoundUtil.PlayButtonSE();
		string parentName = trigger.transform.parent.name;
		if (parentName == "02_jewel")
		{
			DialogJewelShop dialog2 = dialogManager_.getDialog(DialogManager.eDialog.JewelShop) as DialogJewelShop;
			yield return dialogManager_.StartCoroutine(dialog2.show());
		}
		else
		{
			DialogCoinShop dialog = dialogManager_.getDialog(DialogManager.eDialog.CoinShop) as DialogCoinShop;
			yield return dialogManager_.StartCoroutine(dialog.show());
		}
	}

	private IEnumerator buy()
	{
		int price = item_.getPrice();
		Constant.eMoney type = item_.getPriceType();
		int jewel = Bridge.PlayerData.getJewel();
		int coin = Bridge.PlayerData.getCoin();
		bool bNotBuy = false;
		switch (type)
		{
		case Constant.eMoney.Coin:
			if (coin < price)
			{
				yield return StartCoroutine(show(eType.Coin));
				bNotBuy = true;
			}
			break;
		case Constant.eMoney.Jewel:
			if (jewel < price)
			{
				yield return StartCoroutine(show(eType.Jewel));
				bNotBuy = true;
			}
			break;
		}
		if (!bNotBuy)
		{
			Hashtable args = Hash.BuyItem(true, (!Constant.Event.isEventStage(stageNo_)) ? (stageNo_ + 1) : stageNo_, item_);
			NetworkMng.Instance.setup(args);
			yield return StartCoroutine(NetworkMng.Instance.download(API.BuyItem, true, true));
			if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
			{
				WWW www = NetworkMng.Instance.getWWW();
				CommonData commonData = JsonMapper.ToObject<CommonData>(www.text);
				GameData gameData = GlobalData.Instance.getGameData();
				gameData.setCommonData(commonData, false);
				buyItem_.buy();
				Tapjoy.TrackEvent("Money", "Expense Jewel", "Item Ingame", null, price);
				Tapjoy.TrackEvent("Game Item", "Ingame", "Stage No - " + (stageNo_ + 1), buyItem_.getItemType().ToString(), "Use Jewel", price, null, 0L, null, 0L);
				GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Jewel", "Item Ingame", price);
				GlobalGoogleAnalytics.Instance.LogEvent("Item Ingame", buyItem_.getItemType().ToString(), "Stage No - " + (stageNo_ + 1), price);
				GameAnalytics.traceMoneyConsumption("USE_JEWEL(" + buyItem_.getItemType().ToString() + ")", "0", price, Bridge.PlayerData.getCurrentStage());
				Plugin.Instance.buyCompleted("USE_JEWEL_STAGE(" + buyItem_.getItemType().ToString() + ")");
				dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			}
		}
	}

	private IEnumerator use()
	{
		buyItem_.use_setItemFirst();
		dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
		yield return null;
	}

	public IEnumerator updateSaleStageItemData()
	{
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(API.Inactive, true, false));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		InactiveData data = JsonMapper.ToObject<InactiveData>(www.text);
		GlobalData.Instance.getGameData().saleStageItemArea = data.saleStageItemArea;
		GlobalData.Instance.getGameData().stageItemAreaSalePercent = data.stageItemAreaSalePercent;
		GlobalData.Instance.getGameData().isStageItemAreaCampaign = data.isStageItemAreaCampaign;
		GlobalData.Instance.getGameData().setParkData(null, null, data.thanksList, data.buildings, data.mapReleaseNum);
		SaveData.Instance.getParkData().UpdatePlacedData(data.buildings);
		GameData gameData = GlobalData.Instance.getGameData();
		if (gameData.isStageItemAreaCampaign)
		{
			if (gameData.saleStageItemArea == null)
			{
				yield break;
			}
			updateSale(false);
			int ItemPrice2 = 0;
			ItemPrice2 = itemInfo_.Price * gameData.stageItemAreaSalePercent / 100;
			if (ItemPrice2 <= 0)
			{
				ItemPrice2 = 1;
			}
			int[] saleStageItemArea = gameData.saleStageItemArea;
			foreach (int sale_area in saleStageItemArea)
			{
				if (stageNo_ < 10000)
				{
					if (sale_area == stageInfo_.Area)
					{
						updateSale();
						item_.setPrice((Constant.eMoney)itemInfo_.PriceType, ItemPrice2);
						break;
					}
					item_.setPrice((Constant.eMoney)itemInfo_.PriceType, itemInfo_.Price);
					continue;
				}
				if (stageNo_ > 10000 && stageNo_ < 20000 && sale_area == 10000)
				{
					updateSale();
					item_.setPrice((Constant.eMoney)itemInfo_.PriceType, ItemPrice2);
					break;
				}
				if (Constant.ParkStage.isParkStage(stageNo_) && stageInfo_.Area + 500000 == sale_area)
				{
					updateSale();
					item_.setPrice((Constant.eMoney)itemInfo_.PriceType, ItemPrice2);
					break;
				}
				if (stageNo_ > 30000 && stageNo_ < 40000 && sale_area == 30000)
				{
					updateSale();
					item_.setPrice((Constant.eMoney)itemInfo_.PriceType, ItemPrice2);
					break;
				}
				if (stageNo_ > 40000 && stageNo_ < 500000 && sale_area >= 40000 && sale_area < 500000)
				{
					updateSale();
					item_.setPrice((Constant.eMoney)itemInfo_.PriceType, ItemPrice2);
					break;
				}
				item_.setPrice((Constant.eMoney)itemInfo_.PriceType, itemInfo_.Price);
			}
		}
		else
		{
			updateSale(false);
			item_.setPrice((Constant.eMoney)itemInfo_.PriceType, itemInfo_.Price);
		}
	}

	public void updateCampaign()
	{
		if (!(campaignObject == null))
		{
			GameData gameData = GlobalData.Instance.getGameData();
			campaignObject.SetActive(gameData.isJewelCampaign);
		}
	}

	public void updateCampaign(bool isCampaign)
	{
		if (!(campaignObject == null))
		{
			campaignObject.SetActive(isCampaign);
		}
	}

	public void updateSale()
	{
		if (!(saleObject == null))
		{
			GameData gameData = GlobalData.Instance.getGameData();
			saleObject.SetActive(gameData.isStageItemAreaCampaign);
		}
	}

	public void updateSale(bool isSale)
	{
		if (!(saleObject == null))
		{
			saleObject.SetActive(isSale);
		}
	}
}
