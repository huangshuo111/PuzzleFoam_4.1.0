using System;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
	public class ShopData
	{
		public double Price;

		public int Num;

		public string ProductID;

		public Constant.eShop ShopType;
	}

	public bool isBuy = true;

	[SerializeField]
	private UIButton Button;

	[SerializeField]
	private UILabel NumberLabel;

	[SerializeField]
	private UILabel PercentLabel;

	[SerializeField]
	private UILabel PercentLabelAdd;

	[SerializeField]
	private UILabel PriceLabel;

	[SerializeField]
	private UILabel listPriceLabel;

	[SerializeField]
	private UILabel specialPriceLabel;

	[SerializeField]
	private UILabel DiscountLabel;

	[SerializeField]
	private UILabel ReceiveHeartLabel;

	private int ReceiveHeartNum_;

	private ShopData data_ = new ShopData();

	[SerializeField]
	private UILabel bonusLabel;

	protected virtual void OnAwake()
	{
	}

	private void Awake()
	{
		OnAwake();
	}

	public void setNum(int num, bool isSetShop = false)
	{
		data_.Num = num;
		if (isSetShop && ResourceLoader.Instance.isJapanResource())
		{
			NumberLabel.text = data_.Num + MessageResource.Instance.getMessage(3737);
		}
		else if (isSetShop)
		{
			NumberLabel.text = data_.Num.ToString();
		}
		else
		{
			NumberLabel.text = data_.Num.ToString("N0");
		}
	}

	public void setDefaultLabel(int num)
	{
		NumberLabel.text = num.ToString("N0");
	}

	public int getNum()
	{
		return data_.Num;
	}

	public void setReceiveHeartNum(int num)
	{
		ReceiveHeartNum_ = num;
		if (ReceiveHeartLabel != null)
		{
			ReceiveHeartLabel.gameObject.SetActive(ReceiveHeartNum_ != 0);
			if (ReceiveHeartNum_ != 0)
			{
				string message = MessageResource.Instance.getMessage(500016);
				message = MessageResource.Instance.castCtrlCode(message, 1, num.ToString("N0"));
				ReceiveHeartLabel.text = message;
			}
		}
	}

	public int getReceiveHeartNum()
	{
		return ReceiveHeartNum_;
	}

	public void setPrice(double price, Constant.eShop shopType)
	{
		data_.Price = price;
		data_.ShopType = shopType;
		if (shopType == Constant.eShop.Jewel || shopType == Constant.eShop.SetShop)
		{
			string message = MessageResource.Instance.getMessage(62);
			if (data_.Price - Math.Floor(data_.Price) != 0.0)
			{
				message = MessageResource.Instance.getMessage(80);
				message = MessageResource.Instance.castCtrlCode(message, 1, data_.Price.ToString());
			}
			else
			{
				message = MessageResource.Instance.getMessage(62);
				message = MessageResource.Instance.castCtrlCode(message, 1, data_.Price.ToString("N0"));
			}
			PriceLabel.text = message;
		}
		else
		{
			PriceLabel.text = data_.Price.ToString("N0");
		}
	}

	public ShopData getShopData()
	{
		return data_;
	}

	public Constant.eShop getShopType()
	{
		return data_.ShopType;
	}

	public double getPrice()
	{
		return data_.Price;
	}

	public void setProductID(string id)
	{
		data_.ProductID = id;
	}

	public string getProductID()
	{
		return data_.ProductID;
	}

	public void setRate(int rate, Constant.eShop shopType)
	{
		if (!(PercentLabel == null))
		{
			Vector3 localPosition = NumberLabel.transform.localPosition;
			if (rate == 0)
			{
				localPosition.y = -14f;
				PercentLabel.transform.parent.gameObject.SetActive(false);
			}
			else
			{
				localPosition.y = 5f;
				PercentLabel.transform.parent.gameObject.SetActive(true);
			}
			string message = MessageResource.Instance.getMessage(2552);
			switch (shopType)
			{
			case Constant.eShop.Coin:
				message = MessageResource.Instance.getMessage(2575);
				break;
			case Constant.eShop.Jewel:
				message = MessageResource.Instance.getMessage(2552);
				break;
			}
			message = MessageResource.Instance.castCtrlCode(message, 1, rate.ToString());
			PercentLabel.text = message;
			NumberLabel.transform.localPosition = localPosition;
		}
	}

	public void setBonus(int bonus)
	{
		if (!(bonusLabel == null))
		{
			bonusLabel.gameObject.SetActive(bonus > 0);
			string message = MessageResource.Instance.getMessage(2513);
			message = MessageResource.Instance.castCtrlCode(message, 1, bonus.ToString());
			bonusLabel.text = message;
		}
	}

	public void setFixedPrice(double fixedPrice, double specialPrice, int salePercent)
	{
		if (!(listPriceLabel == null) && !(specialPriceLabel == null) && !(DiscountLabel == null))
		{
			MessageResource instance = MessageResource.Instance;
			string message = instance.getMessage(62);
			message = ((fixedPrice - Math.Floor(fixedPrice) == 0.0) ? instance.castCtrlCode(message, 1, fixedPrice.ToString("N0")) : instance.castCtrlCode(message, 1, fixedPrice.ToString()));
			listPriceLabel.text = message;
			string message2 = instance.getMessage(62);
			message2 = ((specialPrice - Math.Floor(specialPrice) == 0.0) ? instance.castCtrlCode(message2, 1, specialPrice.ToString("N0")) : instance.castCtrlCode(message2, 1, specialPrice.ToString()));
			specialPriceLabel.text = message2;
			string message3 = instance.getMessage(2514);
			DiscountLabel.text = instance.castCtrlCode(message3, 1, salePercent.ToString());
		}
	}

	public void setSale(int rate, bool bSale, Constant.eShop shopType, bool isSetShop = false)
	{
		if (PercentLabel == null || PercentLabelAdd == null || isSetShop)
		{
			return;
		}
		base.transform.Find("sale_pos").gameObject.SetActive(bSale);
		if (!bSale)
		{
			NumberLabel.color = new Color(0.99607843f, 0.80784315f, 0.19215687f);
			NumberLabel.effectColor = new Color(0.54509807f, 23f / 85f, 7f / 51f);
			PercentLabelAdd.transform.parent.gameObject.SetActive(false);
			return;
		}
		Vector3 localPosition = NumberLabel.transform.localPosition;
		localPosition.y = 5f;
		PercentLabel.transform.parent.gameObject.SetActive(true);
		string message = MessageResource.Instance.getMessage(2574);
		switch (shopType)
		{
		case Constant.eShop.Coin:
			message = MessageResource.Instance.getMessage(2576);
			break;
		case Constant.eShop.Jewel:
			message = MessageResource.Instance.getMessage(2574);
			break;
		}
		message = MessageResource.Instance.castCtrlCode(message, 1, rate.ToString());
		PercentLabel.text = message;
		NumberLabel.transform.localPosition = localPosition;
		if (!isSetShop)
		{
			NumberLabel.color = new Color(81f / 85f, 0.3372549f, 0.40392157f);
			NumberLabel.effectColor = new Color(0.4117647f, 7f / 85f, 0f);
		}
	}

	public void disable()
	{
		NGUIUtility.disable(Button, false);
	}

	public void enable()
	{
		NGUIUtility.enable(Button, false);
	}
}
