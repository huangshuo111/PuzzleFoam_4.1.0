using System.Collections;
using Bridge;
using Network;
using TapjoyUnity;
using UnityEngine;

public class DialogSetupPark : DialogSetupBase
{
	private StageIcon stageIcon_;

	private int _area_index = -1;

	private UILabel stageNoLabel_;

	private UILabel targetLabel_;

	private GameObject[] stars_ = new GameObject[Constant.StarMax];

	private StageDataTable stageDatas_;

	private ParkStageInfo.Info parkStageInfo_;

	private StageInfo.Info stageInfo_;

	private BoostItem[] items_;

	private UISprite chara_00;

	[HideInInspector]
	public Transform skipTrans_;

	private Transform playButton_;

	private UIButton helpButton_;

	public Transform itemsRoot_;

	public bool bButtonEnable_ = true;

	public static bool bReload_;

	private int noPaymentItemType = -1;

	private Part_Park part_;

	private bool blowcheck;

	private bool bInitialize;

	public override void OnCreate()
	{
		base.OnCreate();
	}

	public void Init(Part_Park part)
	{
		part_ = part;
		Transform transform = base.transform.Find("Front_Panel/StageNo_Label");
		stageNoLabel_ = transform.GetComponent<UILabel>();
		transform = base.transform.Find("Target_Label");
		targetLabel_ = transform.GetComponent<UILabel>();
		for (int i = 0; i < stars_.Length; i++)
		{
			string text = "Front_Panel/flowers/flower_" + i.ToString("D2");
			Transform transform2 = base.transform.Find(text);
			stars_[i] = transform2.gameObject;
		}
		itemsRoot_ = base.transform.Find("items");
		items_ = itemsRoot_.GetComponentsInChildren<BoostItem>(true);
		BoostItem[] array = items_;
		foreach (BoostItem boostItem in array)
		{
			boostItem.setup_ = this;
		}
		helpButton_ = base.transform.Find("ItemDetail_Button").GetComponent<UIButton>();
		transform = base.transform.Find("Play_Button");
		if (transform != null)
		{
			playButton_ = transform;
		}
		transform = base.transform.Find("avatar_button/avatar/chara_00");
		if (transform != null)
		{
			chara_00 = transform.GetComponent<UISprite>();
		}
		skipTrans_ = base.transform.Find("skip_button");
		skipTrans_.gameObject.SetActive(false);
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
		if (_area_index >= 0)
		{
			DialogParkStageList dialogParkStageList = dialogManager_.getDialog(DialogManager.eDialog.ParkStageList) as DialogParkStageList;
			dialogParkStageList.gameObject.SetActive(true);
			dialogParkStageList.StartCoroutine(dialogParkStageList.show(_area_index));
		}
	}

	public IEnumerator show(StageIcon stageIcon, int area_index)
	{
		Input.enable = false;
		noPaymentItemType = -1;
		bReload_ = false;
		Transform Item = base.transform.Find("Labels/Item");
		itemLabel_ = Item.GetComponent<UILabel>();
		stageIcon_ = stageIcon;
		stageNo = stageIcon_.getStageNo();
		parkStageInfo_ = stageDatas_.getParkInfo(stageNo);
		stageInfo_ = StageDataTable.convertStageInfo(parkStageInfo_);
		_area_index = area_index;
		if (TutorialManager.Instance.isTutorial(stageNo, TutorialDataTable.ePlace.Setup) && !Bridge.StageData.isClear(stageNo))
		{
			noPaymentItemType = TutorialManager.Instance.getItemType(stageNo);
		}
		else
		{
			noPaymentItemType = -1;
		}
		if (parkStageInfo_ == null)
		{
			Input.enable = true;
			yield break;
		}
		stageNoLabel_.text = (stageNo % 100000).ToString();
		targetLabel_.text = Constant.MessageUtil.getTargetMsg(stageInfo_.Common, msgRes_, Constant.MessageUtil.eTargetType.Setup);
		int star = Bridge.StageData.getStar(stageNo);
		for (int j = 0; j < stars_.Length; j++)
		{
			stars_[j].SetActive(j < star);
		}
		UpdateCharaIcon();
		setItemPos(itemsRoot_, stageInfo_.Common);
		resetFreeTicket(items_);
		setupItem(items_, stageInfo_.Common);
		setupPricelessTicket(items_, stageInfo_.Common, noPaymentItemType);
		judgeNewIconItem(stageNo);
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
		setupClearCondition_Park(base.transform, stageInfo_.Common);
		Transform FriendHelp = base.transform.Find("FriendHelp");
		bool bAreaSale = false;
		GameData gameData = GlobalData.Instance.getGameData();
		if (gameData.saleArea != null)
		{
			setAreaSaleFlg(false);
			int[] saleArea = gameData.saleArea;
			foreach (int sale_area in saleArea)
			{
				if (sale_area == parkStageInfo_.Area + 500000)
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
		FriendHelp.gameObject.SetActive(false);
		yield return dialogManager_.StartCoroutine(_show(stageNo));
		if (bAreaSale)
		{
			StartCoroutine("updateSaleIcons");
		}
		Input.enable = true;
	}

	private void setupClearCondition_Park(Transform window, StageInfo.CommonInfo commonInfo)
	{
		int num;
		if (commonInfo.IsMinilenDelete)
		{
			num = ((commonInfo.Time <= 0) ? 6 : 7);
		}
		else
		{
			num = ((!commonInfo.IsFriendDelete) ? ((commonInfo.IsAllDelete || commonInfo.IsFulcrumDelete) ? 1 : 2) : 0);
			if (commonInfo.Time > 0)
			{
				num += 3;
			}
		}
		Transform transform = null;
		for (int i = 0; i < 8; i++)
		{
			transform = window.Find("Front_Panel/condition_icon/" + i.ToString("00"));
			if ((bool)transform)
			{
				transform.gameObject.SetActive(i == num);
			}
		}
		int num2 = (commonInfo.IsMinilenDelete ? 3 : ((commonInfo.IsAllDelete || commonInfo.IsFulcrumDelete) ? 2 : ((!commonInfo.IsFriendDelete) ? 1 : 0)));
		for (int j = 0; j < 4; j++)
		{
			window.Find("setup_paper/paper_0" + j).gameObject.SetActive(j == num2);
		}
	}

	public IEnumerator ShineButtonEffect(Transform targButton)
	{
		Input.enable = false;
		GameObject eff = Object.Instantiate(GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.GateEffect)) as GameObject;
		Utility.setParent(eff, targButton, true);
		Vector3 effPos = new Vector3(5f, -10f, -0.5f);
		eff.transform.localPosition = effPos;
		eff.transform.localScale += Vector3.one;
		eff.SetActive(true);
		eff.GetComponent<Animation>().Play();
		Sound.Instance.playSe(Sound.eSe.SE_245_door);
		while (eff.GetComponent<Animation>().isPlaying)
		{
			yield return 0;
		}
		Input.enable = true;
		Object.Destroy(eff);
	}

	private void judgeNewIconItem(int stageNo)
	{
		int[] usedItemType = Bridge.StageData.getUsedItemType(stageNo);
		for (int i = 0; i < items_.Length; i++)
		{
			bool flag = false;
			if (usedItemType != null)
			{
				int[] array = usedItemType;
				foreach (int num in array)
				{
					if (items_[i].getItemType() == (Constant.Item.eType)num)
					{
						flag = true;
						break;
					}
				}
			}
			int newItemStageNo = stageDatas_.getNewItemStageNo(items_[i].getItemType());
			items_[i].setUseNewIconFlg(newItemStageNo == stageNo && !flag && Bridge.StageData.getClearCount(stageNo) < 1);
		}
	}

	public BoostItem[] getItems()
	{
		return items_;
	}

	public void setItems(bool[] bItemSet)
	{
		for (int i = 0; i < items_.Length; i++)
		{
			if (bItemSet[i])
			{
				Debug.Log("item = " + items_[i].getItemType());
				items_[i].setState(BoostItem.eState.OFF);
				StartCoroutine(pressItemButton(items_, items_[i]));
			}
		}
		GlobalData.Instance.bSetItems = null;
	}

	public void setSpecialItem(int itemListNumber, StageInfo.Item itemInfo, Hashtable args)
	{
		StartCoroutine(changeItem(itemListNumber, itemInfo, args));
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
			if (parkStageInfo_.Common.StageNo < addItemStageNo)
			{
				Constant.SoundUtil.PlayButtonSE();
				yield return StartCoroutine(pressNoItemButton(addItemStageNo, index, stageInfo_.Common));
			}
			yield break;
		}
		if (trigger.name.Contains("item_Button"))
		{
			Constant.SoundUtil.PlayButtonSE();
			BoostItem item3 = trigger.transform.parent.GetComponent<BoostItem>();
			yield return StartCoroutine(pressItemButton(items_, item3));
			if (item3.getState() != BoostItem.eState.OFF)
			{
				yield break;
			}
			BoostItem[] array = items_;
			foreach (BoostItem temp in array)
			{
				if (temp != item3 && temp.getItemType() != Constant.Item.eType.Invalid)
				{
					item3.syncSaleIconUpdater(temp);
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
			int stageNo = stageIcon_.getStageNo();
			Input.enable = false;
			bPossible_ = true;
			int heart = Bridge.PlayerData.getHeart();
			if ((stageNo != 500001 || Bridge.StageData.getClearCount_Park(stageNo) > 0) && heart < 1)
			{
				Input.enable = true;
				bPossible_ = false;
				yield return StartCoroutine(base.show(eType.Heart));
				break;
			}
			yield return StartCoroutine(play(stageNo, items_));
			if (!bPossible_)
			{
				Input.enable = true;
				if (NetworkMng.Instance.getResultCode() == eResultCode.NotExistStageItem)
				{
					bReload_ = true;
					StopCoroutine("updateSaleIcons");
					yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
					Constant.PlayerData.addCoin(getSetItemCoin());
					mainMenu_.update();
					setupItem(items_, stageInfo_.Common);
					setupPricelessTicket(items_, stageInfo_.Common, noPaymentItemType);
					BoostItem[] array2 = items_;
					foreach (BoostItem item2 in array2)
					{
						item2.setState(BoostItem.eState.OFF);
					}
					yield return StartCoroutine(part_.updateSaleAreaData());
					yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
					onUpdateSale();
				}
				break;
			}
			BoostItem[] array3 = items_;
			foreach (BoostItem item in array3)
			{
				if (item.getState() == BoostItem.eState.ON)
				{
					if (item.getPriceType() == Constant.eMoney.Jewel)
					{
						Tapjoy.TrackEvent("Money", "Expense Jewel", "Item Lobby Jewel", null, item.getPrice());
						Tapjoy.TrackEvent("Game Item", "Lobby Jewel", "Park Stage" + stageNo, item.getItemType().ToString(), "Use Jewel", item.getPrice(), null, 0L, null, 0L);
						GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Jewel", "Item Lobby Jewel", item.getPrice());
						GlobalGoogleAnalytics.Instance.LogEvent("Item Lobby Jewel", item.getItemType().ToString(), "Park Stage" + stageNo, item.getPrice());
					}
					else if (item.getPriceType() == Constant.eMoney.Coin)
					{
						Tapjoy.TrackEvent("Money", "Expense Coin", "Item Lobby Coin", null, item.getPrice());
						Tapjoy.TrackEvent("Game Item", "Lobby Coin", "Park Stage" + stageNo, item.getItemType().ToString(), "Use Coin", item.getPrice(), null, 0L, null, 0L);
						GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Coin", "Item Lobby Coin", item.getPrice());
						GlobalGoogleAnalytics.Instance.LogEvent("Item Lobby Coin", item.getItemType().ToString(), "Park Stage" + stageNo, item.getPrice());
					}
				}
			}
			Tapjoy.TrackEvent("Charactor", "Use Event Stage", GlobalData.Instance.currentAvatar.index.ToString(), "Park Stage : " + stageNo, 0L);
			GlobalGoogleAnalytics.Instance.LogEvent("Charactor Use", Bridge.MinilenData.getCurrent().index.ToString(), Bridge.MinilenData.getCurrent().level.ToString(), 1L);
			HeartEffect heartEff = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.HeartEffect).GetComponent<HeartEffect>();
			heartEff.gameObject.SetActive(true);
			yield return StartCoroutine(heartEff.play(playButton_.localPosition));
			heartEff.gameObject.SetActive(false);
			while (NetworkMng.Instance.isShowIcon())
			{
				yield return 0;
			}
			Hashtable args = createArgs(stageNo, items_);
			StopCoroutine("updateSaleIcons");
			dialogManager_.reserveCloseDialog(DialogManager.eDialog.PlayScore);
			Scenario.Instance.configReset();
			if (Scenario.Instance.isScenario_Park(stageNo, Scenario.ePlace.Begin))
			{
				args.Add("Place", Scenario.ePlace.Begin);
				partManager_.requestTransition(PartManager.ePart.Scenario, args, FadeMng.eType.Scenario, true);
			}
			else
			{
				partManager_.requestTransition(PartManager.ePart.Stage, args, FadeMng.eType.Cutout, true);
			}
			Input.enable = true;
			break;
		}
		case "avatar_button":
			Constant.SoundUtil.PlayButtonSE();
			mainMenu_.update();
			GlobalData.Instance.set_acInfo(true, DialogManager.eDialog.ParkStageSetup, part_, stageIcon_);
			GlobalData.Instance.bSetItems = null;
			yield return dialogManager_.StartCoroutine(dialogManager_.OpenMinilenCollection(null));
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			StopCoroutine("updateSaleIcons");
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(scoreDialog_));
			part_.dailyMissionValueSetting();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	public void UpdateItems()
	{
		BoostItem[] array = items_;
		foreach (BoostItem boostItem in array)
		{
			boostItem.resetTween();
		}
		BoostItem[] array2 = items_;
		foreach (BoostItem boostItem2 in array2)
		{
			boostItem2.setState(BoostItem.eState.OFF);
		}
		mainMenu_.update();
		resetFreeTicket(items_);
		setupItem(items_, stageInfo_.Common);
		setupPricelessTicket(items_, stageInfo_.Common, noPaymentItemType);
		judgeNewIconItem(stageNo);
		updateSaleIconsInner();
		dialogManager_.playTutorial(stageIcon_.getStageNo());
		Debug.Log("item.Length = " + items_.Length);
		for (int k = 0; k < items_.Length; k++)
		{
			BoostItem boostItem3 = items_[k];
			if (boostItem3 != null && boostItem3.isSpecialPicup && boostItem3.getPriceType() != 0)
			{
				Debug.Log("change");
				StartCoroutine(boostItem3.change(boostItem3.itemInfo_, new Hashtable(), false));
			}
		}
	}

	private IEnumerator openStageSkipDialog()
	{
		DialogStageSkip dialog = dialogManager_.getDialog(DialogManager.eDialog.StageSkip) as DialogStageSkip;
		int price2 = 50;
		int stageNo = Bridge.PlayerData.getCurrentStage();
		price2 = GlobalData.Instance.getGameData().skipPrice;
		dialog.setPrice(stageNo, price2);
		yield return StartCoroutine(dialog.open());
		dialogManager_.addActiveDialogList(DialogManager.eDialog.StageSkip);
		while (dialog.isOpen())
		{
			yield return 0;
		}
		dialogManager_.removeActiveDialogList(DialogManager.eDialog.StageSkip);
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
				boostItem.setPrice(Constant.eMoney.Coin, boostItem.getOriginalPrice() * gameData.areaSalePercent / 100);
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

	public void UpdateCharaIcon()
	{
		Network.MinilenData current = Bridge.MinilenData.getCurrent();
		if (current != null)
		{
			chara_00.spriteName = "UI_picturebook_mini_" + (current.index % 10000).ToString("000");
			chara_00.MakePixelPerfect();
		}
	}

	private void onUpdateSale()
	{
		GameData gameData = GlobalData.Instance.getGameData();
		if (gameData.saleArea != null)
		{
			setAreaSaleFlg(false);
			int[] saleArea = gameData.saleArea;
			foreach (int num in saleArea)
			{
				if (num == parkStageInfo_.Area + 500000)
				{
					StartCoroutine("updateSaleIcons");
					setAreaSaleFlg(true);
					break;
				}
			}
		}
		else
		{
			setAreaSaleFlg(false);
		}
		setupItem(items_, stageInfo_.Common);
		BoostItem[] array = items_;
		foreach (BoostItem boostItem in array)
		{
			if (boostItem.bAreaSale_)
			{
				boostItem.setPrice(Constant.eMoney.Coin, boostItem.getOriginalPrice() * gameData.areaSalePercent / 100);
			}
		}
		updateSaleIconsInner();
	}
}
