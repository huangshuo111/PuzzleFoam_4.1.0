using System.Collections;
using UnityEngine;

public class BoostItem : BoostItemBase
{
	public enum eState
	{
		ON = 0,
		OFF = 1
	}

	private const float ChangeTime = 1f;

	[SerializeField]
	private UISprite Coin;

	[SerializeField]
	private UISprite Jewel;

	[SerializeField]
	private UISprite Free;

	[SerializeField]
	private UISprite Ticket;

	[SerializeField]
	private UIButton Button;

	[SerializeField]
	private UISprite Frame;

	[SerializeField]
	private UILabel PriceLabel;

	[SerializeField]
	private UILabel freeLabel;

	[SerializeField]
	private UILabel freeNum;

	[SerializeField]
	private GameObject SetIcon;

	[SerializeField]
	private GameObject NewIcon;

	[SerializeField]
	private GameObject SaleIcon;

	public Constant.eMoney priceType_ = Constant.eMoney.Coin;

	private eState state_ = eState.OFF;

	private int price_;

	private int originalPrice_;

	public bool bUseNewIcon_;

	public bool bSpecial_;

	public bool bAreaSale_;

	public bool bFreeTicket;

	public float count_time;

	public DialogSetupBase setup_;

	public bool bTweenReset = true;

	public StageInfo.Item itemInfo_;

	public int itemListNumber;

	public bool isSpecialPicup;

	public override void setup(Constant.Item.eType type, int num)
	{
		base.setup(type, num);
		if (NewIcon != null)
		{
			NewIcon.SetActive(false);
		}
		count_time = 1f;
		bSpecial_ = false;
	}

	public void setUseNewIconFlg(bool bUseNewIcon)
	{
		bUseNewIcon_ = bUseNewIcon;
	}

	public void setAreaSaleFlg(bool bAreaSale)
	{
		bAreaSale_ = bAreaSale;
	}

	public void enable()
	{
		NGUIUtility.enable(Button, false);
	}

	public void disable()
	{
		NGUIUtility.disable(Button, false);
		Jewel.gameObject.SetActive(false);
		Coin.gameObject.SetActive(false);
		if (Free != null)
		{
			Free.gameObject.SetActive(false);
		}
		if (freeLabel != null)
		{
			freeLabel.gameObject.SetActive(false);
		}
		if (freeNum != null)
		{
			freeNum.gameObject.SetActive(false);
		}
		PriceLabel.gameObject.SetActive(false);
		if (NewIcon != null)
		{
			NewIcon.SetActive(false);
		}
		if (Ticket != null)
		{
			Ticket.gameObject.SetActive(false);
		}
		if (SaleIcon != null)
		{
			SaleIcon.gameObject.SetActive(false);
		}
	}

	public Constant.eMoney getPriceType()
	{
		return priceType_;
	}

	public int getPrice()
	{
		return price_;
	}

	public int getOriginalPrice()
	{
		return originalPrice_;
	}

	public void setPrice(Constant.eMoney type, int price)
	{
		if (priceType_ != 0 || price == 0)
		{
			switch (type)
			{
			case Constant.eMoney.Coin:
				Jewel.gameObject.SetActive(false);
				Coin.gameObject.SetActive(true);
				resetFreeBoostItem();
				originalPrice_ = price;
				break;
			case Constant.eMoney.Jewel:
				Jewel.gameObject.SetActive(true);
				Coin.gameObject.SetActive(false);
				resetFreeBoostItem();
				originalPrice_ = price;
				break;
			}
			PriceLabel.gameObject.SetActive(true);
			PriceLabel.text = price.ToString("N0");
			price_ = price;
			priceType_ = type;
		}
	}

	public void setPriceUsedFreeTicket(Constant.eMoney type, int price, int ticketCount)
	{
		showSaleIcon(false);
		bFreeTicket = true;
		Jewel.gameObject.SetActive(false);
		Coin.gameObject.SetActive(false);
		Free.gameObject.SetActive(true);
		Ticket.gameObject.SetActive(true);
		freeNum.gameObject.SetActive(true);
		freeLabel.gameObject.SetActive(true);
		PriceLabel.gameObject.SetActive(false);
		freeNum.text = ticketCount.ToString("N0");
		price_ = price;
		priceType_ = type;
	}

	public void updateTicketCountLabel(int ticketCount)
	{
		freeNum.text = ticketCount.ToString("N0");
	}

	public eState getState()
	{
		return state_;
	}

	public void setState(eState state)
	{
		state_ = state;
		string empty = string.Empty;
		if (state_ == eState.ON)
		{
			empty = "item_on";
			showSetIcon(true);
			showNewIcon(false);
			Sound.Instance.playSe(Sound.eSe.SE_321_buyitem);
		}
		else
		{
			empty = "item_off";
			showSetIcon(false);
			showNewIcon(true);
			if (bAreaSale_)
			{
				showSaleIcon(false);
			}
		}
		Frame.spriteName = empty;
	}

	private void showSetIcon(bool bShow)
	{
		if (!(SetIcon == null))
		{
			SetIcon.SetActive(bShow);
		}
	}

	private void showNewIcon(bool bShow)
	{
		if (!(NewIcon == null))
		{
			NewIcon.SetActive(bUseNewIcon_ && bShow);
		}
	}

	private void showSaleIcon(bool bShow)
	{
		if (!(SaleIcon == null) && !bFreeTicket)
		{
			SaleIcon.SetActive(bShow);
		}
	}

	private void OnEnable()
	{
		if (NumLabel != null)
		{
			NumLabel.transform.localScale = new Vector3(32f, 32f, 1f);
		}
		resetTween();
	}

	public void resetTween()
	{
		if (!bTweenReset)
		{
			return;
		}
		UITweener[] componentsInChildren = base.transform.GetComponentsInChildren<UITweener>();
		UITweener[] array = componentsInChildren;
		foreach (UITweener uITweener in array)
		{
			if (uITweener.tweenName == "special_item" || uITweener.tweenName == "special_price")
			{
				uITweener.Sample(0f, true);
				uITweener.enabled = false;
			}
		}
	}

	public IEnumerator change(StageInfo.Item itemInfo, Hashtable args, bool isEffect = true)
	{
		GameObject eff = null;
		if (isEffect)
		{
			itemInfo_ = itemInfo;
			itemListNumber = (int)args["spItemIndex"];
			isSpecialPicup = true;
			setup_.waitItemPress = true;
			eff = Object.Instantiate(GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.GateEffect)) as GameObject;
			Utility.setParent(eff, base.transform.Find("locater"), true);
			eff.SetActive(true);
			eff.GetComponent<Animation>().Play();
			Sound.Instance.playSe(Sound.eSe.SE_245_door);
			yield return new WaitForSeconds(0.5f);
		}
		bool bChangeNum = num_ != itemInfo.Num;
		bool bChangePrice = price_ != itemInfo.Price || priceType_ != (Constant.eMoney)itemInfo.PriceType;
		if (!isEffect)
		{
			bChangeNum = true;
			bChangePrice = true;
		}
		args.Add("bChangeNum", bChangeNum);
		args.Add("bChangePrice", bChangePrice);
		setup((Constant.Item.eType)itemInfo.Type, itemInfo.Num);
		setPrice((Constant.eMoney)itemInfo.PriceType, itemInfo.Price);
		setState(state_);
		setup_.waitItemPress = false;
		bSpecial_ = true;
		if (isEffect && eff != null)
		{
			while (eff.GetComponent<Animation>().isPlaying)
			{
				yield return null;
			}
		}
		UITweener[] ts = base.transform.GetComponentsInChildren<UITweener>();
		UITweener[] array = ts;
		foreach (UITweener t in array)
		{
			if ((bChangeNum && t.tweenName == "special_item") || (bChangePrice && t.tweenName == "special_price"))
			{
				t.Reset();
				t.Play(true);
			}
		}
		if (eff != null)
		{
			Object.Destroy(eff);
		}
	}

	public void saleIconUpdater()
	{
		if (item_ == Constant.Item.eType.Invalid)
		{
			if (SaleIcon.activeSelf)
			{
				showSaleIcon(false);
			}
		}
		else if (bUseNewIcon_ && bAreaSale_ && state_ == eState.OFF)
		{
			count_time += Time.deltaTime;
			if (count_time > 1f)
			{
				count_time = 0f;
				showNewIcon(!NewIcon.activeSelf);
				showSaleIcon(!NewIcon.activeSelf);
			}
		}
		else if (bAreaSale_)
		{
			if (!SaleIcon.activeSelf)
			{
				showSaleIcon(true);
			}
		}
		else if (SaleIcon.activeSelf)
		{
			showSaleIcon(false);
		}
	}

	public void syncSaleIconUpdater(BoostItem item)
	{
		if (bUseNewIcon_ && bAreaSale_ && item_ != Constant.Item.eType.Invalid)
		{
			showNewIcon(item.NewIcon.activeSelf);
			showSaleIcon(item.SaleIcon.activeSelf);
			count_time = item.count_time;
		}
	}

	public void InitSpecialItemInfo()
	{
		itemInfo_ = null;
		itemListNumber = 0;
		isSpecialPicup = false;
	}

	public void resetFreeBoostItem()
	{
		priceType_ = Constant.eMoney.Coin;
		price_ = (originalPrice_ = 0);
		if (Free != null)
		{
			Free.gameObject.SetActive(false);
		}
		bFreeTicket = false;
		if (Ticket != null)
		{
			Ticket.gameObject.SetActive(false);
			freeNum.gameObject.SetActive(false);
			freeLabel.gameObject.SetActive(false);
		}
	}
}
