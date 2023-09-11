using System.Collections;
using Bridge;
using Network;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	private enum eLabel
	{
		Coin = 0,
		Jewel = 1,
		Max = 2
	}

	[SerializeField]
	private HeartMenu Heart;

	[SerializeField]
	private ExpMenu Exp;

	private UILabel[] labels_ = new UILabel[2];

	public GameObject coinCampaign_;

	public GameObject coinUpCampaign_;

	public GameObject jewelCampaign_;

	public GameObject heartShopCampaign_;

	private void Awake()
	{
		UILabel[] componentsInChildren = GetComponentsInChildren<UILabel>();
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			switch (uILabel.transform.parent.name)
			{
			case "01_coin":
				labels_[0] = uILabel;
				break;
			case "02_jewel":
				labels_[1] = uILabel;
				break;
			}
		}
		coinCampaign_ = base.transform.Find("all/01_coin/campaign").gameObject;
		coinUpCampaign_ = base.transform.Find("all/01_coin/campaign_02").gameObject;
		jewelCampaign_ = base.transform.Find("all/02_jewel/campaign").gameObject;
		heartShopCampaign_ = base.transform.Find("all/04_heart/sale_pos").gameObject;
		coinCampaign_.SetActive(false);
		coinUpCampaign_.SetActive(false);
		jewelCampaign_.SetActive(false);
		heartShopCampaign_.SetActive(false);
	}

	public void init()
	{
		Heart.init();
		update();
	}

	public void update()
	{
		if (base.gameObject.activeSelf)
		{
			StopAllCoroutines();
			StartCoroutine(changeVelue());
		}
		else
		{
			labels_[0].text = Bridge.PlayerData.getCoin().ToString("N0");
			labels_[1].text = Bridge.PlayerData.getJewel().ToString();
		}
		Exp.update();
		getHeartMenu().updateLabel();
		GameData gameData = GlobalData.Instance.getGameData();
		coinCampaign_.SetActive(gameData.isCoinCampaign);
		coinUpCampaign_.SetActive(gameData.isCoinupCampaign);
		jewelCampaign_.SetActive(gameData.isJewelCampaign);
		heartShopCampaign_.SetActive(gameData.isHeartShopCampaign);
	}

	public HeartMenu getHeartMenu()
	{
		return Heart;
	}

	public ExpMenu getExpMenu()
	{
		return Exp;
	}

	private IEnumerator changeVelue()
	{
		int toCoin = Bridge.PlayerData.getCoin();
		int toJewel = Bridge.PlayerData.getJewel();
		int fromCoin = int.Parse(labels_[0].text.Replace(",", string.Empty));
		int fromJewel = int.Parse(labels_[1].text);
		float time = 0f;
		while (time < 1f)
		{
			time += Time.deltaTime * 2f;
			int dispCoin = (int)Mathf.Lerp(fromCoin, toCoin, time);
			int dispJewel = (int)Mathf.Lerp(fromJewel, toJewel, time);
			labels_[0].text = dispCoin.ToString("N0");
			labels_[1].text = dispJewel.ToString();
			yield return null;
		}
		labels_[0].text = toCoin.ToString("N0");
		labels_[1].text = toJewel.ToString();
	}
}
