using System.Collections;
using Bridge;
using Network;
using UnityEngine;

public class DialogRankingSetup : DialogSetupBase
{
	private StageIcon stageIcon_;

	private UILabel stageNoLabel_;

	private UILabel targetLabel_;

	private GameObject[] stars_ = new GameObject[Constant.StarMax];

	private StageDataTable stageDatas_;

	private RankingStageInfo stageInfo_;

	private BoostItem[] items_;

	private Transform playButton_;

	private UIButton helpButton_;

	private Transform itemsRoot_;

	public bool bButtonEnable_ = true;

	public static bool bReload_;

	private int noPaymentItemType = -1;

	private Part_Map partMap_;

	public override void OnCreate()
	{
		base.OnCreate();
		itemsRoot_ = base.transform.Find("items");
		items_ = itemsRoot_.GetComponentsInChildren<BoostItem>(true);
		helpButton_ = base.transform.Find("ItemDetail_Button").GetComponent<UIButton>();
		Transform transform = base.transform.Find("Play_Button");
		playButton_ = transform;
		stageDatas_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
	}

	public override void OnClose()
	{
		base.OnClose();
		BoostItem[] array = items_;
		foreach (BoostItem boostItem in array)
		{
			if (boostItem.getState() != BoostItem.eState.OFF && boostItem.getPriceType() == Constant.eMoney.Coin)
			{
				Constant.PlayerData.addCoin(boostItem.getPrice());
			}
		}
		mainMenu_.update();
	}

	public IEnumerator show(PartBase part, bool isRankinngMap)
	{
		Input.enable = false;
		bReload_ = false;
		Transform Item = base.transform.Find("Labels/Item");
		itemLabel_ = Item.GetComponent<UILabel>();
		stageInfo_ = stageDatas_.getRankingData();
		noPaymentItemType = -1;
		if (stageInfo_ == null)
		{
			Input.enable = true;
			yield break;
		}
		StageInfo.CommonInfo commonInfo = stageInfo_.Common;
		setItemPos(itemsRoot_, commonInfo);
		setupItem(items_, commonInfo);
		for (int i = 0; i < items_.Length; i++)
		{
			BoostItem item = items_[i];
			item.setState(BoostItem.eState.OFF);
		}
		if (stageInfo_.Common.ItemNum > 0)
		{
			NGUIUtility.enable(helpButton_, false);
		}
		else
		{
			NGUIUtility.disable(helpButton_, false);
		}
		bool bAreaSale = false;
		GameData gameData = GlobalData.Instance.getGameData();
		if (gameData.saleArea != null)
		{
			setAreaSaleFlg(false);
			int[] saleArea = gameData.saleArea;
			foreach (int sale_area in saleArea)
			{
				if (sale_area == stageInfo_.Area)
				{
					setSalePrice();
					setAreaSaleFlg(true);
					bAreaSale = true;
					break;
				}
			}
		}
		else
		{
			setAreaSaleFlg(false);
		}
		updateSaleIconsInner();
		yield return dialogManager_.StartCoroutine(_showRankingStage());
		if (bAreaSale)
		{
			StartCoroutine("updateSaleIcons");
		}
		Input.enable = true;
	}

	public BoostItem[] getItems()
	{
		return items_;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		if (!bButtonEnable_)
		{
			yield break;
		}
		if (trigger.name.Contains("Noitem_Button"))
		{
			int index = int.Parse(trigger.transform.parent.name.Replace("item_", string.Empty));
			StageDataTable dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
			int addItemStageNo = dataTbl.getAddItemStageNo(index);
			if (stageInfo_.Common.StageNo < addItemStageNo)
			{
				Constant.SoundUtil.PlayButtonSE();
				yield return StartCoroutine(pressNoItemButton(addItemStageNo, index, stageInfo_.Common));
			}
			yield break;
		}
		if (trigger.name.Contains("item_Button"))
		{
			Constant.SoundUtil.PlayButtonSE();
			BoostItem item2 = trigger.transform.parent.GetComponent<BoostItem>();
			yield return StartCoroutine(pressItemButton(items_, item2));
			if (item2.getState() != BoostItem.eState.OFF)
			{
				yield break;
			}
			BoostItem[] array = items_;
			foreach (BoostItem temp in array)
			{
				if (temp != item2 && temp.getItemType() != Constant.Item.eType.Invalid)
				{
					item2.syncSaleIconUpdater(temp);
					break;
				}
			}
			yield break;
		}
		switch (trigger.name)
		{
		case "ItemDetail_Button":
			StartCoroutine(pressHelpButton(trigger, stageInfo_.Common));
			break;
		case "Play_Button":
		{
			Constant.SoundUtil.PlayDecideSE();
			Input.enable = false;
			bPossible_ = true;
			int heart = Bridge.PlayerData.getHeart();
			if (heart < 1)
			{
				Input.enable = true;
				bPossible_ = false;
				yield return StartCoroutine(base.show(eType.Heart));
				break;
			}
			yield return StartCoroutine(play(0, items_));
			if (!bPossible_)
			{
				Input.enable = true;
				if (NetworkMng.Instance.getResultCode() == eResultCode.NotExistStageItem)
				{
					bReload_ = true;
					StopCoroutine("updateSaleIcons");
					Constant.PlayerData.addCoin(getSetItemCoin());
					mainMenu_.update();
					setupItem(items_, stageInfo_.Common);
					BoostItem[] array2 = items_;
					foreach (BoostItem item in array2)
					{
						item.setState(BoostItem.eState.OFF);
					}
					if ((bool)partMap_)
					{
						yield return StartCoroutine(partMap_.sendInactive());
					}
					onUpdateSale();
				}
			}
			else
			{
				HeartEffect heartEff = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.HeartEffect).GetComponent<HeartEffect>();
				heartEff.gameObject.SetActive(true);
				yield return StartCoroutine(heartEff.play(playButton_.localPosition));
				heartEff.gameObject.SetActive(false);
				while (NetworkMng.Instance.isShowIcon())
				{
					yield return 0;
				}
				bButtonEnable_ = true;
				Hashtable args = createArgs(1, items_);
				dialogManager_.StartCoroutine(dialogManager_.closeDialog(scoreDialog_));
				partManager_.requestTransition(PartManager.ePart.RankingStage, args, FadeMng.eType.Cutout, true);
				Input.enable = true;
			}
			break;
		}
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			StopCoroutine("updateSaleIcons");
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(scoreDialog_));
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	public override IEnumerator changeItem(int index, StageInfo.Item itemInfo, Hashtable args)
	{
		BoostItem item = items_[index];
		yield return StartCoroutine(item.change(itemInfo, args));
	}

	public int getSetItemCoin()
	{
		return totalAmount(items_, Constant.eMoney.Coin);
	}

	public void OnApplicationResumeSetupDialog()
	{
		Constant.PlayerData.addCoin(-getSetItemCoin());
	}

	public IEnumerator setItemByIndex(int item_index)
	{
		yield return StartCoroutine(pressItemButton(items_, base.gameObject.transform.Find("items/item_" + item_index.ToString("00")).GetComponent<BoostItem>()));
	}

	private void setAreaSaleFlg(bool flg)
	{
		BoostItem[] array = items_;
		foreach (BoostItem boostItem in array)
		{
			if (boostItem.getItemType() != Constant.Item.eType.Invalid && boostItem.getItemType() != (Constant.Item.eType)noPaymentItemType)
			{
				boostItem.setAreaSaleFlg(flg);
			}
		}
	}

	private void setSalePrice()
	{
		GameData gameData = GlobalData.Instance.getGameData();
		BoostItem[] array = items_;
		foreach (BoostItem boostItem in array)
		{
			if (boostItem.getItemType() != Constant.Item.eType.Invalid)
			{
				boostItem.setPrice(Constant.eMoney.Coin, boostItem.getPrice() * gameData.areaSalePercent / 100);
			}
		}
	}

	private IEnumerator updateSaleIcons()
	{
		while (true)
		{
			updateSaleIconsInner();
			yield return 0;
		}
	}

	private void updateSaleIconsInner()
	{
		BoostItem[] array = items_;
		foreach (BoostItem boostItem in array)
		{
			if (boostItem.getItemType() == (Constant.Item.eType)noPaymentItemType)
			{
				boostItem.setAreaSaleFlg(false);
			}
			boostItem.saleIconUpdater();
		}
	}

	private void onUpdateSale()
	{
		GameData gameData = GlobalData.Instance.getGameData();
		setAreaSaleFlg(false);
		if (gameData.saleArea != null)
		{
			int[] saleArea = gameData.saleArea;
			foreach (int num in saleArea)
			{
				if (num == stageInfo_.Area)
				{
					setAreaSaleFlg(true);
					StartCoroutine("updateSaleIcons");
					break;
				}
			}
		}
		setupItem(items_, stageInfo_.Common);
		BoostItem[] array = items_;
		foreach (BoostItem boostItem in array)
		{
			if (boostItem.bAreaSale_)
			{
				boostItem.setPrice(Constant.eMoney.Coin, boostItem.getPrice() * gameData.areaSalePercent / 100);
			}
		}
		updateSaleIconsInner();
	}
}
