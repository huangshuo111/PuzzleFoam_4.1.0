using Bridge;
using Network;
using UnityEngine;

public class ContinueIcon : MonoBehaviour
{
	private enum eLabel
	{
		Price = 0,
		Recovary = 1,
		Detail = 2,
		Max = 3
	}

	private enum eIcon
	{
		Time = 0,
		Bubble = 1,
		Replay = 2,
		Max = 3
	}

	private enum eMoneyIcon
	{
		Coin = 0,
		Jewel = 1,
		Max = 2
	}

	private GameObject[] icons_ = new GameObject[3];

	private UILabel[] labels_ = new UILabel[3];

	private GameObject[] moneyIcons_ = new GameObject[2];

	private GameObject saleIcon_;

	private int price_;

	private Constant.eMoney priceType_ = Constant.eMoney.Coin;

	private void Awake()
	{
		init(base.transform);
		init(base.transform.parent.Find("add"));
	}

	private void init(Transform root)
	{
		if (root == null)
		{
			return;
		}
		Transform[] componentsInChildren = root.GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			switch (transform.name)
			{
			case "addtime":
				icons_[0] = transform.gameObject;
				break;
			case "addbubbles":
				icons_[1] = transform.gameObject;
				break;
			case "addreplay":
				icons_[2] = transform.gameObject;
				break;
			case "icon_coin":
				moneyIcons_[0] = transform.gameObject;
				break;
			case "icon_jewel":
				moneyIcons_[1] = transform.gameObject;
				break;
			case "Label_Add":
				labels_[1] = transform.GetComponent<UILabel>();
				break;
			case "Label_detail":
				labels_[2] = transform.GetComponent<UILabel>();
				break;
			case "Label_Continue":
				labels_[0] = transform.GetComponent<UILabel>();
				break;
			case "campaign":
				if (!(transform.parent.name != "ContinueButton"))
				{
					saleIcon_ = transform.gameObject;
				}
				break;
			}
		}
	}

	public void setup(StageInfo.CommonInfo stageInfo, Part_Stage.eGameover gameoverType)
	{
		StageInfo.ContinueInfo @continue = stageInfo.Continue;
		priceType_ = (Constant.eMoney)@continue.PriceType;
		price_ = @continue.Price;
		if (stageInfo.StageNo < 40000 || Constant.ParkStage.isParkStage(stageInfo.StageNo))
		{
			updateSale(@continue);
		}
		else
		{
			saleIcon_.SetActive(false);
		}
		updatePriceLabel();
		GameObject[] array = moneyIcons_;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		if (priceType_ == Constant.eMoney.Coin)
		{
			moneyIcons_[0].SetActive(true);
		}
		else
		{
			moneyIcons_[1].SetActive(true);
		}
		MessageResource instance = MessageResource.Instance;
		if (gameoverType == Part_Stage.eGameover.HitSkull || gameoverType == Part_Stage.eGameover.CounterOver || gameoverType == Part_Stage.eGameover.MinilenVanish)
		{
			string message = instance.getMessage(43);
			if (labels_[1] != null)
			{
				labels_[1].text = message;
			}
			if (labels_[2] != null)
			{
				labels_[2].text = instance.getMessage(2563);
			}
		}
		else
		{
			bool flag = gameoverType == Part_Stage.eGameover.TimeOver;
			string message2 = instance.getMessage((!flag) ? 50 : 49);
			message2 = instance.castCtrlCode(message2, 1, @continue.Recovary.ToString("N0"));
			if (labels_[1] != null)
			{
				labels_[1].text = message2;
			}
			message2 = instance.getMessage((!flag) ? 2561 : 2562);
			message2 = instance.castCtrlCode(message2, 1, @continue.Recovary.ToString("N0"));
			if (labels_[2] != null)
			{
				labels_[2].text = message2;
			}
		}
		setupIcon(gameoverType);
	}

	public void setup_AD(StageInfo.CommonInfo stageInfo, Part_Stage.eGameover gameoverType)
	{
		StageInfo.ContinueInfo @continue = stageInfo.Continue;
		MessageResource instance = MessageResource.Instance;
		if (gameoverType == Part_Stage.eGameover.HitSkull || gameoverType == Part_Stage.eGameover.CounterOver)
		{
			string message = instance.getMessage(43);
			if (labels_[1] != null)
			{
				labels_[1].text = message;
			}
			if (labels_[2] != null)
			{
				labels_[2].text = instance.getMessage(600005);
			}
		}
		else
		{
			bool flag = gameoverType == Part_Stage.eGameover.TimeOver;
			string message2 = instance.getMessage((!flag) ? 50 : 49);
			message2 = instance.castCtrlCode(message2, 1, @continue.Recovary.ToString("N0"));
			if (labels_[1] != null)
			{
				labels_[1].text = message2;
			}
			message2 = instance.getMessage((!flag) ? 600006 : 600007);
			message2 = instance.castCtrlCode(message2, 1, @continue.Recovary.ToString("N0"));
			if (labels_[2] != null)
			{
				labels_[2].text = message2;
			}
		}
		setupIcon(gameoverType);
	}

	public void setupBoss(StageInfo.CommonInfo stageInfo, Part_BossStage.eGameover gameoverType)
	{
		StageInfo.ContinueInfo @continue = stageInfo.Continue;
		priceType_ = (Constant.eMoney)@continue.PriceType;
		price_ = @continue.Price;
		updateSale(@continue);
		updatePriceLabel();
		GameObject[] array = moneyIcons_;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		if (priceType_ == Constant.eMoney.Coin)
		{
			moneyIcons_[0].SetActive(true);
		}
		else
		{
			moneyIcons_[1].SetActive(true);
		}
		MessageResource instance = MessageResource.Instance;
		int num;
		switch (gameoverType)
		{
		case Part_BossStage.eGameover.HitSkull:
		case Part_BossStage.eGameover.CounterOver:
		{
			string message = instance.getMessage(43);
			if (labels_[1] != null)
			{
				labels_[1].text = message;
			}
			if (labels_[2] != null)
			{
				labels_[2].text = instance.getMessage(2563);
			}
			return;
		}
		case Part_BossStage.eGameover.TimeOver:
			num = 1;
			break;
		default:
			num = 0;
			break;
		}
		bool flag = (byte)num != 0;
		string message2 = instance.getMessage((!flag) ? 50 : 49);
		message2 = instance.castCtrlCode(message2, 1, @continue.Recovary.ToString("N0"));
		if (labels_[1] != null)
		{
			labels_[1].text = message2;
		}
		message2 = instance.getMessage((!flag) ? 2561 : 2562);
		message2 = instance.castCtrlCode(message2, 1, @continue.Recovary.ToString("N0"));
		if (labels_[2] != null)
		{
			labels_[2].text = message2;
		}
	}

	public void setup(StageInfo.CommonInfo stageInfo)
	{
		StageInfo.ContinueInfo @continue = stageInfo.Continue;
		priceType_ = (Constant.eMoney)@continue.PriceType;
		price_ = @continue.Price;
		updateSale(@continue);
		updatePriceLabel();
		GameObject[] array = moneyIcons_;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		if (priceType_ == Constant.eMoney.Coin)
		{
			moneyIcons_[0].SetActive(true);
		}
		else
		{
			moneyIcons_[1].SetActive(true);
		}
		MessageResource instance = MessageResource.Instance;
		string message = instance.getMessage(43);
		if (labels_[1] != null)
		{
			labels_[1].text = message;
		}
		if (labels_[2] != null)
		{
			labels_[2].text = instance.getMessage(2563);
		}
		GameObject[] array2 = icons_;
		foreach (GameObject gameObject2 in array2)
		{
			if (gameObject2 != null)
			{
				gameObject2.SetActive(false);
			}
		}
		if (labels_[1] != null)
		{
			labels_[1].gameObject.SetActive(true);
		}
		if (icons_[2] != null)
		{
			icons_[2].SetActive(true);
		}
		if (labels_[1] != null)
		{
			labels_[1].gameObject.SetActive(false);
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

	public void setPrice(int price)
	{
		price_ = price;
		updatePriceLabel();
	}

	private void updatePriceLabel()
	{
		labels_[0].text = price_.ToString();
	}

	private void setupIcon(Part_Stage.eGameover gameoverType)
	{
		GameObject[] array = icons_;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
		if (labels_[1] != null)
		{
			labels_[1].gameObject.SetActive(true);
		}
		switch (gameoverType)
		{
		case Part_Stage.eGameover.HitSkull:
		case Part_Stage.eGameover.CounterOver:
		case Part_Stage.eGameover.MinilenVanish:
			if (icons_[2] != null)
			{
				icons_[2].SetActive(true);
			}
			if (labels_[1] != null)
			{
				labels_[1].gameObject.SetActive(false);
			}
			break;
		case Part_Stage.eGameover.ShotCountOver:
			if (icons_[1] != null)
			{
				icons_[1].SetActive(true);
			}
			break;
		case Part_Stage.eGameover.TimeOver:
			if (icons_[0] != null)
			{
				icons_[0].SetActive(true);
			}
			break;
		case Part_Stage.eGameover.FriendToGrow:
		case Part_Stage.eGameover.ScoreNotEnough:
			break;
		}
	}

	public void updateSale(StageInfo.ContinueInfo continuInfo)
	{
		if (saleIcon_ != null)
		{
			if (!isFreeContinue())
			{
				ContinueSaleData continueData = GlobalData.Instance.getContinueData();
				saleIcon_.SetActive(continueData.isContinueCampaign || continueData.isContinueChance);
				price_ = continuInfo.Price;
				if (continueData.isContinueCampaign)
				{
					price_ = price_ * continueData.continueCampaignSale / 100;
				}
				if (continueData.isContinueChance)
				{
					price_ = price_ * continueData.continueChanceSale / 100;
				}
			}
			else
			{
				saleIcon_.SetActive(false);
			}
		}
		updatePriceLabel();
	}

	private bool isFreeContinue()
	{
		int continueNum = Bridge.PlayerData.getContinueNum();
		if (continueNum > 0)
		{
			return false;
		}
		return true;
	}
}
