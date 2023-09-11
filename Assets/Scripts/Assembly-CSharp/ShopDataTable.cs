using System.Collections;
using LitJson;
using Network;
using UnityEngine;

public class ShopDataTable : MonoBehaviour
{
	private JewelShopData jewelShopData_;

	private CoinShopData coinShopData_;

	private HeartShopData heartShopData_;

	private SetShopData setShopData_;

	private ShopAllInfo shopAllInfo_;

	private Hashtable args_ = new Hashtable();

	public JewelShopData getJewelData()
	{
		return jewelShopData_;
	}

	public CoinShopData getCoinData()
	{
		return coinShopData_;
	}

	public HeartShopData getHeartData()
	{
		return heartShopData_;
	}

	public SetShopData getSetShopData()
	{
		return setShopData_;
	}

	private WWW OnCreateWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		return WWWWrap.create(getURL((Constant.eShop)(int)args["type"]));
	}

	public IEnumerator DownloadAllInfo()
	{
		args_.Clear();
		NetworkMng.Instance.setup(args_);
		yield return NetworkMng.Instance.StartCoroutine(NetworkMng.Instance.download(API.ShopAll, true));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			shopAllInfo_ = JsonMapper.ToObject<ShopAllInfo>(www.text);
		}
	}

	public void ReplaceData()
	{
		JewelShopData jewelShopData = new JewelShopData();
		jewelShopData.isJewelCampaign = shopAllInfo_.isJewelCampaign;
		jewelShopData.jewelShopList = shopAllInfo_.jewelShopList;
		jewelShopData.jewelSaleList = shopAllInfo_.jewelSaleList;
		jewelShopData.jewelLuckyChanceList = shopAllInfo_.jewelLuckyChanceList;
		CoinShopData coinShopData = new CoinShopData();
		coinShopData.isCoinCampaign = shopAllInfo_.isCoinCampaign;
		coinShopData.coinShopList = shopAllInfo_.coinShopList;
		coinShopData.coinSaleList = shopAllInfo_.coinSaleList;
		HeartShopData heartShopData = new HeartShopData();
		heartShopData.isHeartShopCampaign = shopAllInfo_.isHeartShopCampaign;
		heartShopData.heartShopItem = shopAllInfo_.heartShopItem;
		heartShopData.heartShopList = shopAllInfo_.heartShopList;
		SetShopData setShopData = new SetShopData();
		setShopData.setShopList = shopAllInfo_.setShopList;
		setShopData.setSaleList = shopAllInfo_.setSaleList;
		setShopData.setRookieList = shopAllInfo_.setRookieList;
		jewelShopData_ = jewelShopData;
		coinShopData_ = coinShopData;
		heartShopData_ = heartShopData;
		setShopData_ = setShopData;
	}

	public IEnumerator download(Constant.eShop shopType, DialogManager dialogMng)
	{
		args_.Clear();
		args_["type"] = shopType;
		NetworkMng.Instance.setup(args_);
		yield return NetworkMng.Instance.StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, true));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			switch (shopType)
			{
			case Constant.eShop.Coin:
				coinShopData_ = JsonMapper.ToObject<CoinShopData>(www.text);
				break;
			case Constant.eShop.Heart:
				heartShopData_ = JsonMapper.ToObject<HeartShopData>(www.text);
				break;
			case Constant.eShop.Jewel:
				jewelShopData_ = JsonMapper.ToObject<JewelShopData>(www.text);
				break;
			}
		}
	}

	private string getURL(Constant.eShop shopType)
	{
		string result = string.Empty;
		switch (shopType)
		{
		case Constant.eShop.Coin:
			result = "charge/shop/coin/";
			break;
		case Constant.eShop.Heart:
			result = "charge/shop/heart/";
			break;
		case Constant.eShop.Jewel:
			result = "jewelShop/";
			break;
		}
		return result;
	}
}
