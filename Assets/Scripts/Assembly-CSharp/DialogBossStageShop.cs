using System.Collections;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using UnityEngine;

public class DialogBossStageShop : DialogShortageBase
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

	private int bossNo_;

	private int bossLevel_ = 1;

	private GameObject campaignObject;

	private UIButton[] buttons = new UIButton[2];

	private GameObject price_;

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
			}
		}
		msgRes_ = MessageResource.Instance;
	}

	public IEnumerator show(int bossNo, int bossLevel, StageBoostItem item, StageInfo.CommonInfo commonInfo, bool _timeStop)
	{
		bossNo_ = bossNo;
		bossLevel_ = bossLevel;
		Debug.Log("BossStageShop : show");
		Constant.Item.eType itemType = item.getItemType();
		int num = item.getNum();
		item_.setup(itemType, num);
		buyItem_ = item;
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
		StageInfo.Item itemInfo = getItemInfo(itemType, commonInfo);
		item_.setPrice((Constant.eMoney)itemInfo.PriceType, itemInfo.Price);
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
			dialogManager_.StartCoroutine(buy());
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
			Hashtable args = Hash.BuyItemBoss(true, bossNo_, bossLevel_, item_);
			NetworkMng.Instance.setup(args);
			yield return StartCoroutine(NetworkMng.Instance.download(API.BuyItemBoss, true, true));
			if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
			{
				WWW www = NetworkMng.Instance.getWWW();
				CommonData commonData = JsonMapper.ToObject<CommonData>(www.text);
				GameData gameData = GlobalData.Instance.getGameData();
				gameData.setCommonData(commonData, false);
				buyItem_.buy();
				Tapjoy.TrackEvent("Money", "Expense Jewel", "Item Ingame", null, price);
				Tapjoy.TrackEvent("Game Item", "Ingame", "Boss Stage", buyItem_.getItemType().ToString(), "Use Jewel", price, null, 0L, null, 0L);
				GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Jewel", "Item Ingame", price);
				GlobalGoogleAnalytics.Instance.LogEvent("Item Ingame", buyItem_.getItemType().ToString(), "Boss Stage", price);
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
}
