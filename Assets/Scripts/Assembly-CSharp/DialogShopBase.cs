using System.Collections;
using Bridge;
using Network;
using UnityEngine;

public abstract class DialogShopBase : DialogShortageBase
{
	protected enum eDialog
	{
		None = -1,
		Buy = 0,
		Transition = 1,
		Max = 2
	}

	protected UIDraggablePanel dragPanel_;

	private Vector3 panelPos_ = Vector3.zero;

	protected ShopItem buyItem_;

	private ShopItem selectItem_;

	protected MainMenu mainMenu_;

	protected DialogCommon commonDialog_;

	private eDialog currentDialog_ = eDialog.None;

	private DialogCommon.OnDecideButton decideCB_;

	private DialogCommon.OnCancelButton cancelCB_;

	protected ShopItem[] shopItems_;

	private Constant.eShop shopType_;

	protected ShopDataTable shopTbl_;

	protected bool isPresentshop_;

	private GameObject banner_;

	protected virtual int getCommonMsgID()
	{
		return -1;
	}

	protected virtual IEnumerator buy()
	{
		yield break;
	}

	protected void init(Constant.eShop shopType)
	{
		shopTbl_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ShopDataTable>();
		Transform transform = base.transform.Find("DragPanel");
		if (transform != null)
		{
			dragPanel_ = transform.GetComponent<UIDraggablePanel>();
			panelPos_ = dragPanel_.transform.localPosition;
		}
		mainMenu_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		if (shopType != Constant.eShop.Jewel)
		{
			decideCB_ = OnDecide;
			cancelCB_ = OnCancel;
		}
		commonDialog_ = dialogManager_.getDialog(DialogManager.eDialog.Common).GetComponent<DialogCommon>();
		shopItems_ = GetComponentsInChildren<ShopItem>(true);
		shopType_ = shopType;
	}

	protected virtual IEnumerator showCB()
	{
		yield break;
	}

	protected virtual void showInit()
	{
	}

	public virtual IEnumerator show()
	{
		Input.enable = false;
		yield return dialogManager_.StartCoroutine(shopTbl_.download(shopType_, dialogManager_));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			Input.enable = true;
			yield break;
		}
		showInit();
		setItemPrice(shopTbl_, shopType_);
		if (shopType_ == Constant.eShop.Heart)
		{
			showInit();
		}
		int num = 0;
		int max = 0;
		switch (shopType_)
		{
		case Constant.eShop.Coin:
			num = Bridge.PlayerData.getCoin();
			max = Constant.CoinMax;
			break;
		case Constant.eShop.Jewel:
			num = Bridge.PlayerData.getJewel();
			max = Constant.JewelMax;
			break;
		case Constant.eShop.Heart:
			num = Bridge.PlayerData.getHeart();
			max = Constant.HeartMax;
			break;
		}
		if (!isPresentshop_ && !isCanBuy(num, max))
		{
			yield return dialogManager_.StartCoroutine(showLimitOverDialog());
			Input.enable = true;
			yield break;
		}
		updateShopItem(num, max);
		yield return dialogManager_.StartCoroutine(showCB());
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		Input.enable = true;
	}

	protected virtual IEnumerator OnButton(GameObject trigger)
	{
		if (trigger.name.Contains("Item"))
		{
			Constant.SoundUtil.PlayDecideSE();
			selectItem_ = trigger.transform.parent.GetComponent<ShopItem>();
			yield return dialogManager_.StartCoroutine(openCommonDialog(eDialog.Buy));
		}
		switch (trigger.name)
		{
		case "Button_1":
			Constant.SoundUtil.PlayDecideSE();
			break;
		case "Button_2":
			Constant.SoundUtil.PlayDecideSE();
			break;
		case "Request_Button":
			Constant.SoundUtil.PlayDecideSE();
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
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
			yield return dialogManager_.StartCoroutine(buy());
			buyItem_ = null;
		}
		else
		{
			currentDialog_ = eDialog.Transition;
			yield return dialogManager_.StartCoroutine(base.show(eType.Jewel, decideCB_, cancelCB_));
		}
	}

	protected virtual IEnumerator openFinishDialog(Constant.eShop shopType, int num)
	{
		int msgID = 0;
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
		string msg2 = MessageResource.Instance.getMessage(msgID);
		msg2 = MessageResource.Instance.castCtrlCode(msg2, 1, num.ToString());
		commonDialog_.setup(msg2, null, null, true);
		commonDialog_.setButtonActive(DialogCommon.eBtn.Close, false);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(commonDialog_));
		while (commonDialog_.isOpen())
		{
			yield return 0;
		}
	}

	protected IEnumerator ShowErrorDialog(int msgnum)
	{
		string msg = MessageResource.Instance.getMessage(msgnum);
		commonDialog_.setup(msg, null, null, true);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(commonDialog_));
		while (commonDialog_.isOpen())
		{
			yield return null;
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

	private IEnumerator openCommonDialog(eDialog dialog)
	{
		int msgID = getCommonMsgID();
		currentDialog_ = dialog;
		commonDialog_.setup(msgID, decideCB_, cancelCB_, false);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(commonDialog_));
	}

	protected int getLimitOverItemCount(int num, int max)
	{
		int num2 = 0;
		ShopItem[] array = shopItems_;
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

	protected bool isCanBuy(int num, int max)
	{
		int limitOverItemCount = getLimitOverItemCount(num, max);
		if (limitOverItemCount == shopItems_.Length)
		{
			return false;
		}
		return true;
	}

	protected bool isCanBuyCheck(int num, int max, int addNum)
	{
		return max >= num + addNum;
	}

	protected void updateShopItem(int num, int max)
	{
		ShopItem[] array = shopItems_;
		foreach (ShopItem shopItem in array)
		{
			int num2 = num + shopItem.getNum();
			if (num2 > max)
			{
				shopItem.disable();
			}
			else
			{
				shopItem.enable();
			}
		}
	}

	protected void setItemPrice(ShopDataTable shopTbl, Constant.eShop shopType)
	{
		for (int i = 0; i < shopItems_.Length; i++)
		{
			ShopItem item = shopItems_[i];
			setItemData(shopType, shopTbl, i, ref item);
		}
	}

	private void setItemData(Constant.eShop shopType, ShopDataTable shopTbl, int index, ref ShopItem item)
	{
		int num = 0;
		double price = 0.0;
		string productID = string.Empty;
		int rate = 0;
		bool flag = false;
		int rate2 = 0;
		int num2 = 0;
		int receiveHeartNum = 0;
		int defaultLabel = 0;
		switch (shopType)
		{
		case Constant.eShop.Jewel:
		{
			JewelShopData jewelData = shopTbl.getJewelData();
			DialogJewelShop dialogJewelShop = (DialogJewelShop)this;
			Network.JewelShopInfo[] jewelShopList = jewelData.jewelShopList;
			Network.JewelShopInfo[] jewelSaleList = jewelData.jewelSaleList;
			Network.JewelShopInfo[] jewelPresentList = jewelData.jewelPresentList;
			if (isPresentshop_)
			{
				if (index >= jewelPresentList.Length)
				{
					item.gameObject.SetActive(false);
					return;
				}
				price = jewelPresentList[index].price;
				num = jewelPresentList[index].num;
				productID = jewelPresentList[index].productId;
				rate = jewelPresentList[index].bonus;
				receiveHeartNum = jewelPresentList[index].BonusHeart;
				break;
			}
			if (!dialogJewelShop.isJewelCampaign_)
			{
				if (index >= jewelShopList.Length)
				{
					item.gameObject.SetActive(false);
					return;
				}
				price = jewelShopList[index].price;
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
			price = jewelShopList[index].price;
			num = jewelSaleList[index].num;
			productID = jewelSaleList[index].productId;
			rate = jewelShopList[index].bonus;
			flag = jewelShopList[index].productId != jewelSaleList[index].productId;
			rate2 = jewelSaleList[index].bonus;
			break;
		}
		case Constant.eShop.Coin:
		{
			CoinShopData coinData = shopTbl.getCoinData();
			DialogCoinShop dialogCoinShop = (DialogCoinShop)this;
			Network.ShopInfo[] coinShopList = coinData.coinShopList;
			Network.ShopInfo[] coinSaleList = coinData.coinSaleList;
			if (!dialogCoinShop.isCoinCampaign_)
			{
				price = coinShopList[index].price;
				num = coinShopList[index].num;
				rate = coinShopList[index].bonus;
			}
			else
			{
				price = coinSaleList[index].price;
				num = coinSaleList[index].num;
				rate = coinShopList[index].bonus;
				flag = coinShopList[index].num != coinSaleList[index].num || coinShopList[index].price != coinSaleList[index].price;
				rate2 = coinSaleList[index].bonus;
			}
			break;
		}
		case Constant.eShop.Heart:
		{
			HeartShopData heartData = shopTbl.getHeartData();
			DialogHeartShop dialogHeartShop = (DialogHeartShop)this;
			flag = dialogHeartShop.isSale_;
			if (flag)
			{
				price = heartData.heartShopList[heartData.heartShopItem].price;
				num = heartData.heartShopList[heartData.heartShopItem].num;
				defaultLabel = heartData.originalnum;
			}
			else
			{
				price = heartData.heartShopList[index].price;
				num = heartData.heartShopList[index].num;
			}
			break;
		}
		}
		item.setRate(rate, shopType);
		item.setSale(rate2, flag, shopType);
		item.setNum(num);
		item.setPrice(price, shopType);
		item.setProductID(productID);
		if (shopType == Constant.eShop.Heart && flag)
		{
			item.setDefaultLabel(defaultLabel);
		}
		item.setReceiveHeartNum(receiveHeartNum);
	}

	protected virtual void OnDisable()
	{
		if ((bool)dragPanel_)
		{
			dragPanel_.MoveRelative(new Vector3(0f - dragPanel_.transform.localPosition.x, 0f - dragPanel_.transform.localPosition.y, 0f));
			dragPanel_.MoveRelative(new Vector3(panelPos_.x, panelPos_.y, 0f));
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

	protected IEnumerator showBanner(bool isCampaign)
	{
		banner_ = base.transform.Find("banner_locator/banner").gameObject;
		banner_.SetActive(false);
		yield break;
	}

	public override void OnStartClose()
	{
		if (banner_ != null)
		{
			banner_.SetActive(false);
		}
	}
}
