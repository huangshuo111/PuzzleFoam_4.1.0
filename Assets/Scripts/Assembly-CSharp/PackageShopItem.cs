using Network;
using UnityEngine;

public class PackageShopItem : MonoBehaviour
{
	public class PackageItemElem
	{
		public UILabel itemName;

		public UISprite image;

		public GameObject obj;

		public PackageItemElem()
		{
			itemName = null;
			image = null;
			obj = null;
		}
	}

	[SerializeField]
	private UILabel packageName;

	[SerializeField]
	private UILabel priceLabel;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private GameObject saleIcon;

	private PackageShopData.PackageInfo info;

	private PackageItemElem[] elems;

	[SerializeField]
	private GameObject[] elemObj;

	private string coin_icon = "UI_icon_coin_00";

	private string heart_icon = "UI_icon_heart_00";

	private string item_icon(int num)
	{
		return "item_" + num.ToString("000") + "_00";
	}

	public void Init()
	{
		GameObject[] array = elemObj;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		elems = new PackageItemElem[elemObj.Length];
		for (int j = 0; j < elemObj.Length; j++)
		{
			elems[j] = new PackageItemElem();
			elems[j].itemName = elemObj[j].transform.Find("item_name").GetComponent<UILabel>();
			elems[j].image = elemObj[j].transform.Find("icon").GetComponent<UISprite>();
			elems[j].obj = elemObj[j];
		}
	}

	public void SetPackage(PackageShopData.PackageInfo package, string packageName)
	{
		info = package;
		SetPackageName(packageName);
		SetPriceLabel(package.price);
		SetSaleIcon(GlobalData.Instance.getGameData().isPackageShopCampaign);
		SetPackageItemElem(package);
	}

	public void SetPackageName(string name)
	{
		packageName.text = name;
	}

	public void SetPriceLabel(int price)
	{
		priceLabel.text = price.ToString();
	}

	public void SetIcon(string iconName)
	{
		icon.spriteName = iconName;
	}

	public void SetSaleIcon(bool isCampaign)
	{
		saleIcon.SetActive(isCampaign);
	}

	public int GetPrice()
	{
		if (info.price > 0)
		{
			return info.price;
		}
		return 0;
	}

	public int GetCoin()
	{
		if (info.coin > 0)
		{
			return info.coin;
		}
		return 0;
	}

	public int GetHeart()
	{
		if (info.heart > 0)
		{
			return info.heart;
		}
		return 0;
	}

	public void SetPackageItemElem(PackageShopData.PackageInfo package)
	{
		MessageResource instance = MessageResource.Instance;
		if (package.coin > 0)
		{
			SetElem(instance.getMessage(26), package.coin, coin_icon, elems[0]);
		}
		if (package.heart > 0)
		{
			SetElem(instance.getMessage(27), package.heart, heart_icon, elems[1]);
		}
		if (package.ticketInfos.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < package.ticketInfos.Length; i++)
		{
			PackageShopData.Ticket ticket = package.ticketInfos[i];
			if (ticket.id >= 0)
			{
				SetElem(instance.getMessage(1000 + (ticket.id - 1)), ticket.num, item_icon(ticket.id), elems[2 + i]);
			}
		}
	}

	public void SetElem(string name, int num, string spriteName, PackageItemElem elem)
	{
		elem.itemName.text = name + " x" + num.ToString("N0");
		elem.image.spriteName = spriteName;
		elem.obj.SetActive(true);
	}
}
