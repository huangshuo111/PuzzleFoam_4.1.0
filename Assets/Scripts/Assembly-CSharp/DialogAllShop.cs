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

public class DialogAllShop : DialogShortageBase
{
	public enum ePanelType
	{
		Invalid = 0,
		Jewel = 1,
		SetShop = 2,
		Coin = 3,
		Heart = 4,
		Max = 5
	}

	protected enum eDialog
	{
		None = -1,
		Buy = 0,
		Transition = 1,
		Max = 2
	}

	private enum eConnect
	{
		Load = 0,
		Success = 1,
		Failed = 2,
		Cancel = 3
	}

	public class ShopButton
	{
		public GameObject button;

		public string backgroundBaseSpriteName;

		public UISprite background;

		public ShopButton(GameObject button_)
		{
			button = button_;
			background = button.transform.Find("background").GetComponent<UISprite>();
			backgroundBaseSpriteName = background.spriteName;
		}
	}

	private const int ELEM_COUNT = 8;

	private const int SET_ITEM_NEED_DRAG_PANEL_NUMBER = 3;

	private DialogCommon commonDialog_;

	private ShopItem selectItem_;

	private PackageShopItem selectPackageItem_;

	private GameObject[] panels;

	private ePanelType activePanel;

	protected ShopDataTable shopTbl_;

	protected UIDraggablePanel[] dragPanel_;

	private Vector3[] panelPos_;

	protected MainMenu mainMenu_;

	private eDialog currentDialog_ = eDialog.None;

	private DialogCommon.OnDecideButton decideCB_;

	private DialogCommon.OnCancelButton cancelCB_;

	protected ShopItem[][] shopItems_;

	private Constant.eShop shopType_;

	private ShopItem buyItem_;

	public bool isSale_;

	private Network.JewelShopInfo[] shopInfo_jewel;

	private Network.SetShopInfo[] shopInfo_set;

	private int setupItemCoin_;

	[NonSerialized]
	public bool isJewelCampaign_;

	[NonSerialized]
	public bool isCoinCampaign_;

	[NonSerialized]
	public bool isPackageShopCampaign_;

	private GameObject hotIcon_;

	private List<Transform> hotIconLocator_ = new List<Transform>();

	private string SetItemSignature = string.Empty;

	private string SetItemData = string.Empty;

	private eConnect connectStatus_;

	private bool bBuying_;

	public TJPlacement offerwallPlacement;

	public string TapjoyOutput = string.Empty;

	private GameObject heartSaleDisp;

	private UILabel saleHeartCount;

	private UIButton requestButton;

	private GameObject hotIcon_SetShop;

	private List<Transform> hotIconLocator_SetShop = new List<Transform>();

	public bool isBuyPackage;

	private ShopButton jewelButton;

	private ShopButton packageButton;

	private ShopButton coinButton;

	private ShopButton heartButton;

	private GameObject window_base_01;

	private GameObject window_base_01_heart;

	private GameObject window_base_02;

	private GameObject banner_;

	private string checkKey = string.Empty;

	public override void OnCreate()
	{
		panels = new GameObject[5];
		panels[1] = base.transform.Find("JewelShop_Panel").gameObject;
		panels[3] = base.transform.Find("CoinCharge_Panel").gameObject;
		panels[4] = base.transform.Find("HeartShop_Panel").gameObject;
		panels[2] = base.transform.Find("SetShop_Panel").gameObject;
		dragPanel_ = new UIDraggablePanel[5];
		panelPos_ = new Vector3[5];
		shopItems_ = new ShopItem[5][];
		jewelButton = new ShopButton(base.transform.Find("tabs/JewelButton").gameObject);
		packageButton = new ShopButton(base.transform.Find("tabs/PackageButton").gameObject);
		coinButton = new ShopButton(base.transform.Find("tabs/CoinButton").gameObject);
		heartButton = new ShopButton(base.transform.Find("tabs/HeartButton").gameObject);
		window_base_01 = base.transform.Find("window/window_base_01").gameObject;
		window_base_01_heart = base.transform.Find("window/window_base_01_heart").gameObject;
		window_base_02 = base.transform.Find("window/window_base_02").gameObject;
		shopTbl_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ShopDataTable>();
		commonDialog_ = dialogManager_.getDialog(DialogManager.eDialog.Common).GetComponent<DialogCommon>();
		hotIcon_ = panels[1].transform.Find("icon_hot").gameObject;
		Transform transform = panels[1].transform.Find("DragPanel/contents");
		for (int i = 0; i < transform.childCount; i++)
		{
			hotIconLocator_.Add(transform.GetChild(i).Find("hot_pos"));
		}
		hotIcon_SetShop = panels[2].transform.Find("icon_hot").gameObject;
		transform = panels[2].transform.Find("DragPanel/contents");
		for (int j = 0; j < transform.childCount; j++)
		{
			hotIconLocator_SetShop.Add(transform.GetChild(j).Find("hot_pos"));
		}
		requestButton = panels[4].transform.Find("window/Request_Button").GetComponent<UIButton>();
		mainMenu_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		init(ePanelType.Jewel);
		init(ePanelType.Coin);
		init(ePanelType.Heart);
		init(ePanelType.SetShop);
		decideCB_ = OnDecide;
		cancelCB_ = OnCancel;
		dragPanel_[3].enabled = false;
		dragPanel_[3] = null;
		if (offerwallPlacement == null)
		{
			offerwallPlacement = TJPlacement.CreatePlacement("pb_offerwall_placement");
		}
		TJPlacement.OnRequestSuccess += HandlePlacementRequestSuccess;
		TJPlacement.OnRequestFailure += HandlePlacementRequestFailure;
		TJPlacement.OnContentReady += HandlePlacementContentReady;
		TJPlacement.OnContentShow += HandlePlacementContentShow;
		TJPlacement.OnContentDismiss += HandlePlacementContentDismiss;
		TJPlacement.OnPurchaseRequest += HandleOnPurchaseRequest;
		TJPlacement.OnRewardRequest += HandleOnRewardRequest;
		Tapjoy.OnAwardCurrencyResponse += HandleAwardCurrencyResponse;
		Tapjoy.OnAwardCurrencyResponseFailure += HandleAwardCurrencyResponseFailure;
		Tapjoy.OnSpendCurrencyResponse += HandleSpendCurrencyResponse;
		Tapjoy.OnSpendCurrencyResponseFailure += HandleSpendCurrencyResponseFailure;
		Tapjoy.OnGetCurrencyBalanceResponse += HandleGetCurrencyBalanceResponse;
		Tapjoy.OnGetCurrencyBalanceResponseFailure += HandleGetCurrencyBalanceResponseFailure;
		Tapjoy.OnEarnedCurrency += HandleEarnedCurrency;
		Tapjoy.OnVideoStart += HandleVideoStart;
		Tapjoy.OnVideoError += HandleVideoError;
		Tapjoy.OnVideoComplete += HandleVideoComplete;
	}

	private void init(ePanelType shop)
	{
		Transform transform = panels[(int)shop].transform.Find("DragPanel");
		if (transform != null)
		{
			dragPanel_[(int)shop] = transform.GetComponent<UIDraggablePanel>();
			panelPos_[(int)shop] = dragPanel_[(int)shop].transform.localPosition;
		}
		shopItems_[(int)shop] = panels[(int)shop].transform.GetComponentsInChildren<ShopItem>(true);
	}

	public IEnumerator show(ePanelType panelType)
	{
		Input.enable = false;
		yield return dialogManager_.StartCoroutine(shopTbl_.DownloadAllInfo());
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			Input.enable = true;
			yield break;
		}
		shopTbl_.ReplaceData();
		showInit_Jewel();
		showInit_Coin();
		showInit_Heart();
		showInit_SetShop();
		PartManager.ePart currentPart = partManager_.currentPart;
		if (currentPart == PartManager.ePart.Map || currentPart == PartManager.ePart.EventMap || currentPart == PartManager.ePart.CollaborationMap || currentPart == PartManager.ePart.Park)
		{
			GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>().heartShopCampaign_.SetActive(isSale_);
			GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>().jewelCampaign_.SetActive(isJewelCampaign_);
			GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>().coinCampaign_.SetActive(isCoinCampaign_);
		}
		setItemPrice(shopTbl_, Constant.eShop.Jewel, ePanelType.Jewel);
		setItemPrice(shopTbl_, Constant.eShop.Coin, ePanelType.Coin);
		setItemPrice(shopTbl_, Constant.eShop.Heart, ePanelType.Heart);
		setItemPrice(shopTbl_, Constant.eShop.SetShop, ePanelType.SetShop);
		showInit_Heart();
		SetShopData shopData = shopTbl_.getSetShopData();
		dragPanel_[2].enabled = shopData.setShopList.Length >= 3;
		SetPanel(panelType);
		yield return dialogManager_.StartCoroutine(showJewelCB());
		yield return dialogManager_.StartCoroutine(showCoinCB());
		yield return dialogManager_.StartCoroutine(showSetShopCB());
		UpdateSalePopupInTabs();
		if (!isOpen())
		{
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		}
		Input.enable = true;
	}

	private void showInit_Jewel()
	{
		JewelShopData jewelData = shopTbl_.getJewelData();
		isJewelCampaign_ = jewelData.isJewelCampaign;
		GameData gameData = GlobalData.Instance.getGameData();
		gameData.isJewelCampaign = isJewelCampaign_;
		if (partManager_.currentPart == PartManager.ePart.Stage)
		{
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
			shopInfo_jewel = jewelData.jewelShopList;
		}
		else
		{
			shopInfo_jewel = jewelData.jewelSaleList;
		}
	}

	private IEnumerator showJewelCB()
	{
		hotIcon_.SetActive(false);
		for (int i = 0; i < shopInfo_jewel.Length; i++)
		{
			if (shopInfo_jewel[i].isHot)
			{
				hotIcon_.SetActive(true);
				Utility.setParent(hotIcon_, hotIconLocator_[i], false);
				break;
			}
		}
		yield break;
	}

	private void showInit_Coin()
	{
		CoinShopData coinData = shopTbl_.getCoinData();
		isCoinCampaign_ = coinData.isCoinCampaign;
		GameData gameData = GlobalData.Instance.getGameData();
		gameData.isCoinCampaign = isCoinCampaign_;
	}

	private IEnumerator showCoinCB()
	{
		yield break;
	}

	private void showInit_Heart()
	{
		heartSaleDisp = panels[4].transform.Find("window/item000/sale_pos").gameObject;
		HeartShopData heartData = shopTbl_.getHeartData();
		isSale_ = heartData.isHeartShopCampaign;
		heartSaleDisp.SetActive(isSale_);
		GameData gameData = GlobalData.Instance.getGameData();
		gameData.isHeartShopCampaign = isSale_;
		if (heartSaleDisp.activeSelf)
		{
			saleHeartCount = heartSaleDisp.transform.Find("Number_Label_sale").GetComponent<UILabel>();
			saleHeartCount.text = shopItems_[4][0].getNum().ToString();
		}
	}

	private void showInit_SetShop()
	{
		SetShopData setShopData = shopTbl_.getSetShopData();
		GameData gameData = GlobalData.Instance.getGameData();
		shopInfo_set = setShopData.setShopList;
	}

	private IEnumerator showSetShopCB()
	{
		hotIcon_SetShop.SetActive(false);
		for (int i = 0; i < shopInfo_set.Length; i++)
		{
			if (shopInfo_set[i].isHot)
			{
				hotIcon_SetShop.SetActive(true);
				Utility.setParent(hotIcon_SetShop, hotIconLocator_SetShop[i], false);
				break;
			}
		}
		yield break;
	}

	protected bool isCanBuy(int num, int max, ePanelType type)
	{
		int limitOverItemCount = getLimitOverItemCount(num, max, type);
		if (limitOverItemCount == shopItems_[(int)type].Length)
		{
			return false;
		}
		return true;
	}

	protected bool isCanBuyCheck(int num, int max, int addNum)
	{
		return max >= num + addNum;
	}

	protected int getLimitOverItemCount(int num, int max, ePanelType type)
	{
		int num2 = 0;
		ShopItem[] array = shopItems_[(int)type];
		foreach (ShopItem shopItem in array)
		{
			int num3 = num + shopItem.getNum();
			if (num3 > max || !shopItem.gameObject.activeSelf)
			{
				num2++;
			}
		}
		return num2;
	}

	protected IEnumerator showLimitOverDialog()
	{
		DialogConfirm dialog = dialogManager_.getDialog(DialogManager.eDialog.BuyLimit) as DialogConfirm;
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
	}

	protected IEnumerator showBanner(bool isCampaign)
	{
		banner_ = panels[1].transform.Find("banner_locator/banner").gameObject;
		banner_.SetActive(false);
		if (isCampaign)
		{
			string imgUrl = ((shopType_ != Constant.eShop.Coin) ? shopTbl_.getJewelData().imgUrl : shopTbl_.getCoinData().imgUrl);
			WWW www = new WWW(imgUrl);
			yield return www;
			Texture tex = www.texture;
			if (tex != null && tex.width != 8 && !isPlayingCloseAnime())
			{
				banner_.SetActive(true);
				banner_.GetComponent<UITexture>().mainTexture = tex;
			}
			www.Dispose();
		}
	}

	protected void updateShopItem(int num, int max, ePanelType type)
	{
	}

	protected void setItemPrice(ShopDataTable shopTbl, Constant.eShop shopType, ePanelType panelType)
	{
		for (int i = 0; i < shopItems_[(int)panelType].Length; i++)
		{
			ShopItem item = shopItems_[(int)panelType][i];
			setItemData(shopType, shopTbl, i, ref item);
		}
	}

	private void setItemData(Constant.eShop shopType, ShopDataTable shopTbl, int index, ref ShopItem item)
	{
		int num = 0;
		double num2 = 0.0;
		string productID = string.Empty;
		int rate = 0;
		int bonus = 0;
		bool flag = false;
		int num3 = 0;
		int defaultLabel = 0;
		bool isBuy = true;
		double fixedPrice = 0.0;
		Network.SetShopInfo.Reward[] array = new Network.SetShopInfo.Reward[2];
		switch (shopType)
		{
		case Constant.eShop.Jewel:
		{
			JewelShopData jewelData = shopTbl.getJewelData();
			Network.JewelShopInfo[] jewelShopList = jewelData.jewelShopList;
			Network.JewelShopInfo[] jewelSaleList = jewelData.jewelSaleList;
			if (!isJewelCampaign_)
			{
				if (index >= jewelShopList.Length)
				{
					item.gameObject.SetActive(false);
					return;
				}
				num2 = jewelShopList[index].price;
				num = jewelShopList[index].num;
				productID = jewelShopList[index].productId;
				rate = jewelShopList[index].bonus;
				break;
			}
			if (index >= jewelSaleList.Length)
			{
				item.gameObject.SetActive(false);
				return;
			}
			num2 = jewelSaleList[index].price;
			num = jewelSaleList[index].num;
			productID = jewelSaleList[index].productId;
			rate = jewelShopList[index].bonus;
			flag = jewelShopList[index].productId != jewelSaleList[index].productId;
			num3 = jewelSaleList[index].bonus;
			break;
		}
		case Constant.eShop.Coin:
		{
			CoinShopData coinData = shopTbl.getCoinData();
			Network.ShopInfo[] coinShopList = coinData.coinShopList;
			Network.ShopInfo[] coinSaleList = coinData.coinSaleList;
			if (!isCoinCampaign_)
			{
				num2 = coinShopList[index].price;
				num = coinShopList[index].num;
				rate = coinShopList[index].bonus;
			}
			else
			{
				num2 = coinSaleList[index].price;
				num = coinSaleList[index].num;
				rate = coinSaleList[index].bonus;
				flag = coinShopList[index].num != coinSaleList[index].num || coinShopList[index].price != coinSaleList[index].price;
				num3 = coinSaleList[index].bonus;
			}
			break;
		}
		case Constant.eShop.Heart:
		{
			HeartShopData heartData = shopTbl.getHeartData();
			flag = isSale_;
			if (flag)
			{
				num2 = heartData.heartShopList[heartData.heartShopItem].price;
				num = heartData.heartShopList[heartData.heartShopItem].num;
				defaultLabel = heartData.heartShopList[0].num;
			}
			else
			{
				num2 = heartData.heartShopList[index].price;
				num = heartData.heartShopList[index].num;
			}
			break;
		}
		case Constant.eShop.SetShop:
		{
			SetShopData setShopData = shopTbl.getSetShopData();
			Network.SetShopInfo[] setShopList = setShopData.setShopList;
			Network.SetShopInfo[] setShopList2 = setShopData.setShopList;
			if (index >= setShopList.Length)
			{
				item.gameObject.SetActive(false);
				return;
			}
			num2 = ((!WWWWrap.isJapan()) ? setShopList[index].priceDol : setShopList[index].price);
			fixedPrice = ((!WWWWrap.isJapan()) ? setShopList[index].setPriceDol : setShopList[index].setPrice);
			num = setShopList[index].num;
			productID = setShopList[index].productId;
			rate = 0;
			num3 = setShopList[index].salePercent;
			array = setShopList[index].rewards;
			isBuy = setShopList[index].isBuy;
			bonus = setShopList[index].bonus;
			break;
		}
		}
		bool flag2 = shopType == Constant.eShop.SetShop;
		if (!flag2)
		{
			item.setRate(rate, shopType);
		}
		item.setSale(num3, flag, shopType, flag2);
		item.setNum(num, flag2);
		item.setPrice(num2, shopType);
		item.setProductID(productID);
		if (flag2)
		{
			item.isBuy = isBuy;
			item.setBonus(bonus);
			item.setFixedPrice(fixedPrice, num2, num3);
			for (int i = 0; i < 8; i++)
			{
				if (i < array.Length)
				{
					int messageID = 26;
					string spriteName = "UI_icon_coin_00";
					if (array[i].type == 3)
					{
						messageID = 27;
						spriteName = "UI_icon_heart_00";
					}
					else if (array[i].type == 4)
					{
						messageID = 2510;
						spriteName = "gacha_ticket";
					}
					else if (array[i].type >= 1000)
					{
						messageID = array[i].type - 1;
						spriteName = "item_" + (array[i].type - 1000).ToString("000") + "_00";
					}
					string message = MessageResource.Instance.getMessage(messageID);
					Transform transform = item.transform.Find("elem_" + i.ToString("00"));
					transform.Find("item_name").GetComponent<UILabel>().text = message;
					transform.Find("item_number").GetComponent<UILabel>().text = array[i].num.ToString();
					transform.gameObject.SetActive(true);
					transform.Find("icon").GetComponent<UISprite>().spriteName = spriteName;
					transform.Find("icon").GetComponent<UISprite>().MakePixelPerfect();
					ReSizeLocalScale(transform.Find("icon").transform, 30f);
				}
				else
				{
					if (!(item.transform.Find("elem_" + i.ToString("00")) != null))
					{
						break;
					}
					item.transform.Find("elem_" + i.ToString("00")).gameObject.SetActive(false);
				}
			}
		}
		if (shopType == Constant.eShop.Heart && flag)
		{
			item.setDefaultLabel(defaultLabel);
		}
	}

	private void ReSizeLocalScale(Transform image, float height)
	{
		if (height != 0f)
		{
			float num = height / image.localScale.y;
			image.localScale = new Vector3(image.localScale.x * num, image.localScale.y * num, image.localScale.z);
		}
	}

	protected virtual void OnDisable()
	{
		for (int i = 0; i < dragPanel_.Length; i++)
		{
			if (dragPanel_[i] != null)
			{
				Debug.Log("dragPanel_ = " + dragPanel_[i].transform.parent.name);
				dragPanel_[i].MoveRelative(new Vector3(0f - dragPanel_[i].transform.localPosition.x, 0f - dragPanel_[i].transform.localPosition.y, 0f));
				dragPanel_[i].MoveRelative(new Vector3(panelPos_[i].x, panelPos_[i].y, 0f));
			}
		}
	}

	protected override IEnumerator OnCancel()
	{
		yield return dialogManager_.StartCoroutine(closeCommonDialog());
	}

	protected override IEnumerator OnDecide()
	{
		if (currentDialog_ == eDialog.Transition)
		{
			yield return dialogManager_.StartCoroutine(transition());
			yield break;
		}
		int jewel = Bridge.PlayerData.getJewel();
		if ((double)jewel >= selectItem_.getPrice())
		{
			yield return dialogManager_.StartCoroutine(closeCommonDialog());
			buyItem_ = selectItem_;
			if (activePanel == ePanelType.Coin)
			{
				yield return dialogManager_.StartCoroutine(buy_Coin());
			}
			else if (activePanel == ePanelType.Heart)
			{
				yield return dialogManager_.StartCoroutine(buy_Heart());
			}
			buyItem_ = null;
		}
		else
		{
			currentDialog_ = eDialog.Transition;
			yield return dialogManager_.StartCoroutine(base.show(eType.Jewel, decideCB_, cancelCB_));
		}
	}

	private IEnumerator closeCommonDialog()
	{
		if (currentDialog_ != eDialog.None)
		{
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(commonDialog_));
		}
		currentDialog_ = eDialog.None;
	}

	private IEnumerator openCommonDialog(eDialog dialog, int msgID)
	{
		currentDialog_ = dialog;
		commonDialog_.setup(msgID, decideCB_, cancelCB_, false);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(commonDialog_));
	}

	private void UpdateSetupDialog()
	{
		DialogSetup dialogSetup = dialogManager_.getDialog(DialogManager.eDialog.Setup) as DialogSetup;
		DialogEventSetup dialogEventSetup = dialogManager_.getDialog(DialogManager.eDialog.EventSetup) as DialogEventSetup;
		DialogCollaborationSetup dialogCollaborationSetup = dialogManager_.getDialog(DialogManager.eDialog.CollaborationSetup) as DialogCollaborationSetup;
		DialogBossSetup dialogBossSetup = dialogManager_.getDialog(DialogManager.eDialog.BossSetup) as DialogBossSetup;
		DialogSetupPark dialogSetupPark = dialogManager_.getDialog(DialogManager.eDialog.ParkStageSetup) as DialogSetupPark;
		if (dialogSetup != null && dialogSetup.isOpen())
		{
			dialogSetup.UpdateItems();
		}
		else if (dialogEventSetup != null && dialogEventSetup.isOpen())
		{
			dialogEventSetup.UpdateItems();
		}
		else if (dialogCollaborationSetup != null && dialogCollaborationSetup.isOpen())
		{
			dialogCollaborationSetup.UpdateItems();
		}
		else if (dialogBossSetup != null && dialogBossSetup.isOpen())
		{
			dialogBossSetup.UpdateItems();
		}
		else if (dialogSetupPark != null && dialogSetupPark.isOpen())
		{
			dialogSetupPark.UpdateItems();
		}
		mainMenu_.update();
	}

	private void UpdateAvatarGachaDialog()
	{
		DialogAvatarGacha dialogAvatarGacha = dialogManager_.getDialog(DialogManager.eDialog.AvatarGacha) as DialogAvatarGacha;
		if (dialogAvatarGacha.isOpen())
		{
			dialogAvatarGacha.setButton(GlobalData.Instance.getGameData().isFirstGacha);
		}
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.G))
		{
			isBuyPackage = true;
			GlobalData.Instance.getGameData().gachaTicket++;
			UserItemList[] userItemList = GlobalData.Instance.getGameData().userItemList;
			foreach (UserItemList userItemList2 in userItemList)
			{
				userItemList2.count++;
			}
			mainMenu_.update();
		}
	}

	protected virtual IEnumerator OnButton(GameObject trigger)
	{
		if (trigger.name == "Request_Button")
		{
			Constant.SoundUtil.PlayButtonSE();
			yield return StartCoroutine(partManager_.nologin());
			if (partManager_.isNologinCancel)
			{
				yield break;
			}
			DialogRequest dialog = dialogManager_.getDialog(DialogManager.eDialog.Request) as DialogRequest;
			yield return dialogManager_.StartCoroutine(dialog.show());
		}
		switch (trigger.name)
		{
		case "Button_1":
			Constant.SoundUtil.PlayButtonSE();
			Debug.Log(string.Format("trigger.name = {0}", trigger.name));
			partManager_.showBoard(WebView.eWebType.SCTL);
			break;
		case "Button_2":
			Constant.SoundUtil.PlayButtonSE();
			Debug.Log(string.Format("trigger.name = {0}", trigger.name));
			partManager_.showBoard(WebView.eWebType.EFTA);
			break;
		}
		switch (trigger.name)
		{
		case "JewelButton":
			if (activePanel != ePanelType.Jewel)
			{
				Constant.SoundUtil.PlayButtonSE();
			}
			SetPanel(ePanelType.Jewel);
			break;
		case "CoinButton":
			if (activePanel != ePanelType.Coin)
			{
				Constant.SoundUtil.PlayButtonSE();
			}
			SetPanel(ePanelType.Coin);
			break;
		case "PackageButton":
			if (activePanel != ePanelType.SetShop)
			{
				Constant.SoundUtil.PlayButtonSE();
			}
			SetPanel(ePanelType.SetShop);
			break;
		case "HeartButton":
			if (activePanel != ePanelType.Heart)
			{
				Constant.SoundUtil.PlayButtonSE();
			}
			SetPanel(ePanelType.Heart);
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			SetPanel(ePanelType.Invalid);
			if (isBuyPackage)
			{
				UpdateSetupDialog();
				UpdateAvatarGachaDialog();
				isBuyPackage = false;
			}
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "ADButton1":
			Constant.SoundUtil.PlayCancelSE();
			Plugin.Instance.showAdList();
			break;
		case "ADButton2":
			Constant.SoundUtil.PlayCancelSE();
			if (offerwallPlacement != null)
			{
				offerwallPlacement.RequestContent();
			}
			break;
		}
		string parentName = string.Empty;
		if (trigger.transform.parent.parent.parent != null && activePanel == ePanelType.Heart)
		{
			parentName = trigger.transform.parent.parent.parent.name;
		}
		else if (trigger.transform.parent.parent.parent.parent != null)
		{
			parentName = trigger.transform.parent.parent.parent.parent.name;
		}
		switch (parentName)
		{
		case "JewelShop_Panel":
			yield return StartCoroutine(OnButton_Jewel(trigger));
			break;
		case "CoinCharge_Panel":
			yield return StartCoroutine(OnButton_Coin(trigger));
			break;
		case "Package_Panel":
			yield return StartCoroutine(OnButton_Package(trigger));
			break;
		case "HeartShop_Panel":
			yield return StartCoroutine(OnButton_Heart(trigger));
			break;
		case "SetShop_Panel":
			yield return StartCoroutine(OnButton_SetShop(trigger));
			break;
		}
	}

	private IEnumerator OnButton_Jewel(GameObject trigger)
	{
		if (trigger.name.Contains("Item"))
		{
			Constant.SoundUtil.PlayDecideSE();
			dialogManager_.StartCoroutine(buy_Jewel(trigger));
		}
		yield break;
	}

	private IEnumerator OnButton_SetShop(GameObject trigger)
	{
		if (trigger.name.Contains("Item"))
		{
			Constant.SoundUtil.PlayDecideSE();
			dialogManager_.StartCoroutine(buy_Set(trigger));
		}
		yield break;
	}

	private IEnumerator OnButton_Coin(GameObject trigger)
	{
		if (trigger.name.Contains("Item"))
		{
			Constant.SoundUtil.PlayDecideSE();
			ShopItem item = trigger.transform.parent.GetComponent<ShopItem>();
			int num = Bridge.PlayerData.getCoin();
			int max = Constant.CoinMax;
			if (!isCanBuyCheck(num, max, item.getNum()))
			{
				yield return dialogManager_.StartCoroutine(showLimitOverDialog());
				yield break;
			}
			selectItem_ = trigger.transform.parent.GetComponent<ShopItem>();
			yield return dialogManager_.StartCoroutine(openCommonDialog(eDialog.Buy, 8));
		}
	}

	private IEnumerator OnButton_Package(GameObject trigger)
	{
		if (trigger.name.Contains("Item"))
		{
			Constant.SoundUtil.PlayDecideSE();
			selectPackageItem_ = trigger.transform.parent.GetComponent<PackageShopItem>();
			yield return dialogManager_.StartCoroutine(openCommonDialog(eDialog.Buy, 8));
		}
	}

	private IEnumerator OnButton_Heart(GameObject trigger)
	{
		if (trigger.name.Contains("Item"))
		{
			Constant.SoundUtil.PlayDecideSE();
			ShopItem item = trigger.transform.parent.GetComponent<ShopItem>();
			int num = Bridge.PlayerData.getHeart();
			int max = Constant.HeartMax;
			if (!isCanBuyCheck(num, max, item.getNum()))
			{
				yield return dialogManager_.StartCoroutine(showLimitOverDialog());
				yield break;
			}
			selectItem_ = trigger.transform.parent.GetComponent<ShopItem>();
			yield return dialogManager_.StartCoroutine(openCommonDialog(eDialog.Buy, 7));
		}
	}

	private IEnumerator connectStateChange()
	{
		connectStatus_ = eConnect.Success;
		yield break;
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

	private void addJewelLocal()
	{
		int num = 0;
		int num2 = 0;
		string productID = buyItem_.getProductID();
		JewelShopData jewelData = shopTbl_.getJewelData();
		for (int i = 0; i < shopInfo_jewel.Length; i++)
		{
			Network.JewelShopInfo jewelShopInfo = shopInfo_jewel[i];
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

	private void addSetShopLocal()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		List<int> list = new List<int>();
		string productID = buyItem_.getProductID();
		JewelShopData jewelData = shopTbl_.getJewelData();
		for (int i = 0; i < shopInfo_jewel.Length; i++)
		{
			Network.SetShopInfo setShopInfo = shopInfo_set[i];
			if (!(productID == setShopInfo.productId))
			{
				continue;
			}
			num = setShopInfo.bonus;
			num2 = setShopInfo.buy;
			for (int j = 0; j < setShopInfo.rewards.Length; j++)
			{
				if (setShopInfo.rewards[j].type == 1)
				{
					num3 += setShopInfo.rewards[j].num;
				}
				if (setShopInfo.rewards[j].type == 3)
				{
					num4 += setShopInfo.rewards[j].num;
				}
				if (setShopInfo.rewards[j].type == 4)
				{
					num5 += setShopInfo.rewards[j].num;
				}
				if (setShopInfo.rewards[j].type > 1000)
				{
					list.Add(setShopInfo.rewards[j].num);
				}
			}
			break;
		}
		Bridge.PlayerData.setBuyJewel(Bridge.PlayerData.getBuyJewel() + num2);
		Bridge.PlayerData.setBonusJewel(Bridge.PlayerData.getBonusJewel() + num);
		Bridge.PlayerData.setHeart(Bridge.PlayerData.getHeart() + num4);
		Bridge.PlayerData.setCoin(Bridge.PlayerData.getCoin() + num3);
		GlobalData.Instance.getGameData().gachaTicket += num5;
	}

	private IEnumerator buy_Jewel(GameObject trigger)
	{
		Input.enable = false;
		ShopItem item = (buyItem_ = trigger.transform.parent.GetComponent<ShopItem>());
		int num = Bridge.PlayerData.getJewel();
		int max = Constant.JewelMax;
		if (!isCanBuyCheck(num, max, item.getNum()))
		{
			yield return dialogManager_.StartCoroutine(showLimitOverDialog());
			Input.enable = true;
			yield break;
		}
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
			yield break;
		}
		bBuying_ = false;
		mainMenu_.update();
		Input.enable = true;
		yield return dialogManager_.StartCoroutine(openFinishDialog(Constant.eShop.Jewel, buyItem_.getNum()));
		updateOtherLabel();
		yield return dialogManager_.StartCoroutine(show(ePanelType.Jewel));
		UpdateAvatarDialog();
	}

	public bool isBuying()
	{
		return bBuying_;
	}

	private IEnumerator buy_Coin()
	{
		int coin3 = Bridge.PlayerData.getCoin();
		setupItemCoin_ = 0;
		if (!isBuyPackage)
		{
			if (partManager_.currentPart == PartManager.ePart.Map)
			{
				DialogSetup setup = dialogManager_.getDialog(DialogManager.eDialog.Setup) as DialogSetup;
				if (setup.isOpen())
				{
					setupItemCoin_ = setup.getSetItemCoin();
				}
				DialogBossSetup bSetup = dialogManager_.getDialog(DialogManager.eDialog.BossSetup) as DialogBossSetup;
				if (bSetup.isOpen())
				{
					setupItemCoin_ = bSetup.getSetItemCoin();
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
			else if (partManager_.currentPart == PartManager.ePart.Park)
			{
				DialogSetupPark setup3 = dialogManager_.getDialog(DialogManager.eDialog.ParkStageSetup) as DialogSetupPark;
				if (setup3 != null && setup3.isOpen())
				{
					setupItemCoin_ = setup3.getSetItemCoin();
				}
			}
		}
		NetworkMng.Instance.setup(Hash.BuyCoin(buyItem_));
		yield return StartCoroutine(NetworkMng.Instance.download(API.BuyCoin, true));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			JsonData json = JsonMapper.ToObject(www.text);
			GameData gameData = GlobalData.Instance.getGameData();
			gameData.bonusJewel = (int)json["bonusJewel"];
			gameData.buyJewel = (int)json["buyJewel"];
			gameData.coin = (int)json["coin"] - setupItemCoin_;
			Tapjoy.TrackEvent("Money", "Income Coin", "Coin Shop", null, buyItem_.getNum());
			Tapjoy.TrackEvent("Money", "Expense Jewel", "Coin Shop", null, (int)buyItem_.getPrice());
			Tapjoy.TrackEvent("PurchaseCoin", "Coin", "COIN" + buyItem_.getNum(), null, "Use Jewel", (long)buyItem_.getPrice(), "Get Coin", buyItem_.getNum(), null, 0L);
			GameAnalytics.traceMoneyConsumption("USE_JEWEL", "0", buyItem_.getPrice(), Bridge.PlayerData.getCurrentStage());
			GameAnalytics.traceMoneyAcquisition("BUY_COIN", "1", buyItem_.getNum(), Bridge.PlayerData.getCurrentStage());
			Plugin.Instance.buyCompleted("BUY_COIN" + buyItem_.getNum());
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Coin Shop", buyItem_.getNum());
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Jewel", "Coin Shop", (int)buyItem_.getPrice());
			GlobalGoogleAnalytics.Instance.LogEvent("Buy Coin", buyItem_.getProductID(), buyItem_.getNum() + "Coin", (long)buyItem_.getPrice());
			updateOtherLabel();
			mainMenu_.update();
			yield return dialogManager_.StartCoroutine(openFinishDialog(buyItem_.getShopType(), buyItem_.getNum()));
			yield return dialogManager_.StartCoroutine(show(ePanelType.Coin));
			coin3 = Bridge.PlayerData.getCoin();
			coin3 += setupItemCoin_;
			UpdateAvatarDialog();
		}
	}

	private IEnumerator buy_Heart()
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
			Tapjoy.TrackEvent("Money", "Expense Jewel", "Buy Heart", null, (int)buyItem_.getPrice());
			Tapjoy.TrackEvent("PurchaseHeart", "Heart", "Heart", null, 0L);
			GameAnalytics.traceMoneyConsumption("USE_JEWEL", "0", buyItem_.getPrice(), Bridge.PlayerData.getCurrentStage());
			GameAnalytics.traceMoneyAcquisition("BUY_HEART", "1", buyItem_.getPrice(), Bridge.PlayerData.getCurrentStage());
			Plugin.Instance.buyCompleted("BUY_HEART" + buyItem_.getPrice());
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Jewel", "Buy Heart", (int)buyItem_.getPrice());
			GlobalGoogleAnalytics.Instance.LogEvent("Buy Heart", "Heart", "5 Heart", (long)buyItem_.getPrice());
			updateOtherLabel();
			mainMenu_.update();
			UpdateAvatarDialog();
			yield return dialogManager_.StartCoroutine(openFinishDialog(buyItem_.getShopType(), buyItem_.getNum()));
			if (isBuyPackage)
			{
				UpdateSetupDialog();
				UpdateAvatarGachaDialog();
				isBuyPackage = false;
			}
			SetPanel(ePanelType.Invalid);
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
		}
	}

	private IEnumerator buy_Set(GameObject trigger)
	{
		SetItemSignature = string.Empty;
		SetItemData = string.Empty;
		Input.enable = false;
		if (!(buyItem_ = trigger.transform.parent.GetComponent<ShopItem>()).isBuy)
		{
			yield return dialogManager_.StartCoroutine(showLimitOverDialog());
			Input.enable = true;
			yield break;
		}
		NetworkMng.Instance.setup(Hash.CheckKey(buyItem_.getProductID(), buyItem_.getNum()));
		yield return StartCoroutine(NetworkMng.Instance.download(API.CheckKey, true));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			Input.enable = true;
			bBuying_ = false;
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		JsonData checkResult = JsonMapper.ToObject(www.text);
		checkKey = (string)checkResult["checkKey"];
		Debug.Log("<color=red>checkKey = " + checkKey + "</color>");
		bBuying_ = true;
		connectStatus_ = eConnect.Load;
		GKUnityPluginController.GK_Payment_Purchase(buyItem_.getProductID(), PurchaseFinished_Set);
		while (connectStatus_ == eConnect.Load)
		{
			yield return null;
		}
		if (connectStatus_ != eConnect.Success)
		{
			bBuying_ = false;
			NetworkMng.Instance.setup(Hash.CancelPayment(checkKey));
			yield return StartCoroutine(NetworkMng.Instance.download(API.CancelPayment, true));
			if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
			{
				Input.enable = true;
				bBuying_ = false;
				yield break;
			}
			WWW www_cancelKey = NetworkMng.Instance.getWWW();
			mainMenu_.update();
			bBuying_ = false;
			Input.enable = true;
			yield break;
		}
		bBuying_ = false;
		NetworkMng.Instance.setup(Hash.SetPayment(buyItem_.getProductID(), buyItem_.getNum(), checkKey, SetItemSignature, SetItemData));
		yield return StartCoroutine(NetworkMng.Instance.download(API.SetPayment, false));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			Input.enable = true;
			yield break;
		}
		WWW www_setPayment = NetworkMng.Instance.getWWW();
		CommonData common = JsonMapper.ToObject<CommonData>(www_setPayment.text);
		GameData gameData = GlobalData.Instance.getGameData();
		gameData.bonusJewel = common.bonusJewel;
		gameData.buyJewel = common.buyJewel;
		gameData.coin = common.coin;
		gameData.heart = common.heart;
		gameData.userItemList = common.userItemList;
		gameData.gachaTicket = common.gachaTicket;
		isBuyPackage = true;
		mainMenu_.update();
		Input.enable = true;
		yield return dialogManager_.StartCoroutine(openFinishDialog(Constant.eShop.Jewel, buyItem_.getNum()));
		updateOtherLabel();
		yield return dialogManager_.StartCoroutine(show(ePanelType.SetShop));
		UpdateAvatarDialog();
	}

	public void SetPanel(ePanelType type)
	{
		for (int i = 0; i < panels.Length; i++)
		{
			if (panels[i] != null)
			{
				panels[i].SetActive(type == (ePanelType)i);
			}
		}
		SetTab(type);
		window_base_01.SetActive(type != ePanelType.Heart);
		window_base_01_heart.SetActive(type == ePanelType.Heart);
		window_base_02.SetActive(type == ePanelType.Heart);
		activePanel = type;
	}

	private void SetTab(ePanelType type)
	{
		jewelButton.background.spriteName = jewelButton.backgroundBaseSpriteName;
		packageButton.background.spriteName = packageButton.backgroundBaseSpriteName;
		coinButton.background.spriteName = coinButton.backgroundBaseSpriteName;
		heartButton.background.spriteName = heartButton.backgroundBaseSpriteName;
		switch (type)
		{
		case ePanelType.Jewel:
			jewelButton.background.spriteName += "_on";
			break;
		case ePanelType.SetShop:
			packageButton.background.spriteName += "_on";
			break;
		case ePanelType.Coin:
			coinButton.background.spriteName += "_on";
			break;
		case ePanelType.Heart:
			heartButton.background.spriteName += "_on";
			break;
		}
	}

	private void UpdateSalePopupInTabs()
	{
		base.transform.Find("tabs/JewelButton/sale").gameObject.SetActive(isJewelCampaign_);
		base.transform.Find("tabs/CoinButton/sale").gameObject.SetActive(isCoinCampaign_);
		base.transform.Find("tabs/HeartButton/sale").gameObject.SetActive(isSale_);
		base.transform.Find("tabs/PackageButton/sale").gameObject.SetActive(isPackageShopCampaign_);
	}

	protected IEnumerator openFinishDialog(Constant.eShop shopType, int num, bool bSuccess = true)
	{
		string msg2 = string.Empty;
		int msgID = 0;
		if (bSuccess)
		{
			switch (shopType)
			{
			case Constant.eShop.Coin:
				msgID = 65;
				break;
			case Constant.eShop.Heart:
				msgID = 64;
				break;
			case Constant.eShop.Jewel:
				msgID = 63;
				break;
			}
			msg2 = MessageResource.Instance.getMessage(msgID);
			msg2 = MessageResource.Instance.castCtrlCode(msg2, 1, num.ToString());
		}
		commonDialog_.setup(msg2, null, null, true);
		commonDialog_.setButtonActive(DialogCommon.eBtn.Close, false);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(commonDialog_));
		while (commonDialog_.isOpen())
		{
			yield return 0;
		}
	}

	protected void updateOtherLabel()
	{
		if (dialogManager_.isLoaded(DialogManager.eDialog.StageShop) && dialogManager_.getDialog(DialogManager.eDialog.StageShop).isOpen())
		{
			DialogBase dialog = dialogManager_.getDialog(DialogManager.eDialog.StageShop);
			DialogStageShop dialogStageShop = dialog as DialogStageShop;
			dialogStageShop.updateLabel();
		}
		if (dialogManager_.isLoaded(DialogManager.eDialog.BossStageShop) && dialogManager_.getDialog(DialogManager.eDialog.BossStageShop).isOpen())
		{
			DialogBase dialog2 = dialogManager_.getDialog(DialogManager.eDialog.BossStageShop);
			DialogBossStageShop dialogBossStageShop = dialog2 as DialogBossStageShop;
			dialogBossStageShop.updateLabel();
		}
		if (dialogManager_.isLoaded(DialogManager.eDialog.Continue) && dialogManager_.getDialog(DialogManager.eDialog.Continue).isOpen())
		{
			DialogBase dialog3 = dialogManager_.getDialog(DialogManager.eDialog.Continue);
			DialogContinue dialogContinue = dialog3 as DialogContinue;
			dialogContinue.updateLabel();
		}
		if (dialogManager_.isLoaded(DialogManager.eDialog.ContinueBoss) && dialogManager_.getDialog(DialogManager.eDialog.ContinueBoss).isOpen())
		{
			DialogBase dialog4 = dialogManager_.getDialog(DialogManager.eDialog.ContinueBoss);
			DialogContinueBoss dialogContinueBoss = dialog4 as DialogContinueBoss;
			dialogContinueBoss.updateLabel();
		}
	}

	private void UpdateAvatarDialog()
	{
		DialogAvatarGacha dialogAvatarGacha = dialogManager_.getDialog(DialogManager.eDialog.AvatarGacha) as DialogAvatarGacha;
		DialogAvatarLevelup dialogAvatarLevelup = dialogManager_.getDialog(DialogManager.eDialog.AvatarLevelup) as DialogAvatarLevelup;
		if (dialogAvatarGacha != null && dialogAvatarLevelup != null)
		{
			if (dialogAvatarGacha.isOpen())
			{
				dialogAvatarGacha.UpdatePlusButtonUi();
			}
			if (dialogAvatarLevelup.isOpen())
			{
				dialogAvatarLevelup.UpdatePlusButtonUi();
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

	protected void PurchaseFinished_Set(string strResult)
	{
		Debug.Log("PurchaseFinished = " + strResult);
		if (strResult.Equals(string.Empty))
		{
			connectStatus_ = eConnect.Failed;
			return;
		}
		SetItemSignature = string.Empty;
		SetItemData = string.Empty;
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
			SetItemSignature = Constant.JsonGetValue(strResult, "purchase-info");
			SetItemData = Constant.JsonGetValue(strResult, "signature");
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

	public void HandlePlacementRequestSuccess(TJPlacement placement)
	{
		UnityEngine.Debug.Log("Lasy-HandlePlacementRequestSuccess");
		UnityEngine.Debug.Log("Lasy-GetName : " + placement.GetName());
		UnityEngine.Debug.Log("Lasy-IsContentAvailable : " + placement.IsContentAvailable());
		if (placement.IsContentAvailable())
		{
			if (placement.GetName() == "pb_offerwall_placement")
			{
				placement.ShowContent();
			}
		}
		else
		{
			TapjoyOutput = "No content available for " + placement.GetName();
			UnityEngine.Debug.Log("C#: No content available for " + placement.GetName());
		}
	}

	public void HandlePlacementRequestFailure(TJPlacement placement, string error)
	{
		UnityEngine.Debug.Log("Lasy-HandlePlacementRequestFailure");
		Debug.Log("C#: HandlePlacementRequestFailure");
		Debug.Log("C#: Request for " + placement.GetName() + " has failed because: " + error);
		TapjoyOutput = "Request for " + placement.GetName() + " has failed because: " + error;
	}

	public void HandlePlacementContentReady(TJPlacement placement)
	{
		UnityEngine.Debug.Log("Lasy-HandlePlacementContentReady");
		Debug.Log("C#: HandlePlacementContentReady");
		TapjoyOutput = "HandlePlacementContentReady";
		if (!placement.IsContentAvailable())
		{
			Debug.Log("C#: no content");
		}
	}

	public void HandlePlacementContentShow(TJPlacement placement)
	{
		UnityEngine.Debug.Log("Lasy-HandlePlacementContentShow");
		Debug.Log("C#: HandlePlacementContentShow");
	}

	public void HandlePlacementContentDismiss(TJPlacement placement)
	{
		UnityEngine.Debug.Log("Lasy-HandlePlacementContentDismiss");
		Debug.Log("C#: HandlePlacementContentDismiss");
		TapjoyOutput = "TJPlacement " + placement.GetName() + " has been dismissed";
	}

	private void HandleOnPurchaseRequest(TJPlacement placement, TJActionRequest request, string productId)
	{
		UnityEngine.Debug.Log("Lasy-HandleOnPurchaseRequest");
		Debug.Log("C#: HandleOnPurchaseRequest");
		request.Completed();
	}

	private void HandleOnRewardRequest(TJPlacement placement, TJActionRequest request, string itemId, int quantity)
	{
		UnityEngine.Debug.Log("Lasy-HandleOnRewardRequest");
		Debug.Log("C#: HandleOnRewardRequest");
		request.Completed();
	}

	public void HandleAwardCurrencyResponse(string currencyName, int balance)
	{
		UnityEngine.Debug.Log("Lasy-HandleAwardCurrencyResponse");
		Debug.Log("C#: HandleAwardCurrencySucceeded: currencyName: " + currencyName + ", balance: " + balance);
		TapjoyOutput = "Awarded Currency -- " + currencyName + " Balance: " + balance;
	}

	public void HandleAwardCurrencyResponseFailure(string error)
	{
		UnityEngine.Debug.Log("Lasy-HandleAwardCurrencyResponseFailure");
		Debug.Log("C#: HandleAwardCurrencyResponseFailure: " + error);
	}

	public void HandleGetCurrencyBalanceResponse(string currencyName, int balance)
	{
		UnityEngine.Debug.Log("Lasy-HandleGetCurrencyBalanceResponse");
		Debug.Log("C#: HandleGetCurrencyBalanceResponse: currencyName: " + currencyName + ", balance: " + balance);
		TapjoyOutput = currencyName + " Balance: " + balance;
	}

	public void HandleGetCurrencyBalanceResponseFailure(string error)
	{
		UnityEngine.Debug.Log("Lasy-HandleGetCurrencyBalanceResponseFailure");
		Debug.Log("C#: HandleGetCurrencyBalanceResponseFailure: " + error);
	}

	public void HandleSpendCurrencyResponse(string currencyName, int balance)
	{
		UnityEngine.Debug.Log("Lasy-HandleSpendCurrencyResponse");
		Debug.Log("C#: HandleSpendCurrencyResponse: currencyName: " + currencyName + ", balance: " + balance);
		TapjoyOutput = currencyName + " Balance: " + balance;
	}

	public void HandleSpendCurrencyResponseFailure(string error)
	{
		UnityEngine.Debug.Log("Lasy-HandleSpendCurrencyResponseFailure");
		Debug.Log("C#: HandleSpendCurrencyResponseFailure: " + error);
	}

	public void HandleEarnedCurrency(string currencyName, int amount)
	{
		UnityEngine.Debug.Log("Lasy-HandleEarnedCurrency");
		Debug.Log("C#: HandleEarnedCurrency: currencyName: " + currencyName + ", amount: " + amount);
		TapjoyOutput = currencyName + " Earned: " + amount;
		Tapjoy.ShowDefaultEarnedCurrencyAlert();
	}

	public void HandleVideoStart()
	{
		UnityEngine.Debug.Log("Lasy-HandleVideoStart");
		Debug.Log("C#: HandleVideoStarted");
	}

	public void HandleVideoError(string status)
	{
		UnityEngine.Debug.Log("Lasy-HandleVideoError");
		Debug.Log("C#: HandleVideoError, status: " + status);
	}

	public void HandleVideoComplete()
	{
		UnityEngine.Debug.Log("Lasy-HandleVideoComplete");
		Debug.Log("C#: HandleVideoComplete");
	}
}
