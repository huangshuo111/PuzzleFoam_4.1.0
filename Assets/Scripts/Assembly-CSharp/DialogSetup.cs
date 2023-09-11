using System.Collections;
using Bridge;
using Network;
using TapjoyUnity;
using TnkAd;
using Toast.Analytics;
using UnityEngine;

public class DialogSetup : DialogSetupBase
{
	private StageIcon stageIcon_;

	private UILabel stageNoLabel_;

	private UILabel targetLabel_;

	private GameObject[] stars_ = new GameObject[Constant.StarMax];

	private StageDataTable stageDatas_;

	private StageInfo.Info stageInfo_;

	private BoostItem[] items_;

	private UISprite chara_00;

	private UISprite chara_01;

	private UISprite chara_02;

	private UISprite chara_03;

	[HideInInspector]
	public Transform skipTrans_;

	private Transform playButton_;

	private UIButton helpButton_;

	public Transform itemsRoot_;

	public bool bButtonEnable_ = true;

	public static bool bReload_;

	private int noPaymentItemType = -1;

	private Part_Map part_;

	private bool blowcheck;

	private bool bInitialize;

	public override void OnCreate()
	{
		base.OnCreate();
	}

	public override IEnumerator open()
	{
		skipTrans_.GetComponent<UIPanel>().alpha = 1f;
		return base.open();
	}

	public override IEnumerator close()
	{
		TweenAlpha[] components = skipTrans_.GetComponents<TweenAlpha>();
		TweenAlpha[] array = components;
		foreach (TweenAlpha tweenAlpha in array)
		{
			if (tweenAlpha.gameObject.activeInHierarchy && tweenAlpha.tweenName == "Out")
			{
				tweenAlpha.Reset();
				tweenAlpha.Play(true);
			}
		}
		return base.close();
	}

	public void Init()
	{
		Transform transform = base.transform.Find("Labels/StageNo_Label");
		stageNoLabel_ = transform.GetComponent<UILabel>();
		transform = base.transform.Find("Target_Label");
		targetLabel_ = transform.GetComponent<UILabel>();
		for (int i = 0; i < stars_.Length; i++)
		{
			string text = "Star_" + i.ToString("D2");
			Transform transform2 = base.transform.Find("Stars/" + text);
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
		transform = base.transform.Find("avatar_button/avatar/chara_01");
		if (transform != null)
		{
			chara_01 = transform.GetComponent<UISprite>();
		}
		transform = base.transform.Find("avatar_button/avatar/chara_02");
		if (transform != null)
		{
			chara_02 = transform.GetComponent<UISprite>();
		}
		transform = base.transform.Find("avatar_button/avatar/chara_03");
		if (transform != null)
		{
			chara_03 = transform.GetComponent<UISprite>();
		}
		skipTrans_ = base.transform.Find("skip_button");
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

	public IEnumerator show(StageIcon stageIcon, Part_Map part)
	{
		Input.enable = false;
		if (!bInitialize)
		{
			Init();
		}
		noPaymentItemType = -1;
		bReload_ = false;
		part_ = part;
		Transform Item = base.transform.Find("Labels/Item");
		itemLabel_ = Item.GetComponent<UILabel>();
		stageIcon_ = stageIcon;
		stageNo = stageIcon_.getStageNo();
		stageInfo_ = stageDatas_.getInfo(stageNo);
		if (TutorialManager.Instance.isTutorial(stageNo, TutorialDataTable.ePlace.Setup) && !Bridge.StageData.isClear(stageNo))
		{
			noPaymentItemType = TutorialManager.Instance.getItemType(stageNo);
		}
		else
		{
			noPaymentItemType = -1;
		}
		if (stageInfo_ == null)
		{
			Input.enable = true;
			yield break;
		}
		stageNoLabel_.text = (stageNo + 1).ToString();
		setupTargetLabel(targetLabel_, stageInfo_.Common);
		int star = Bridge.StageData.getStar(stageNo);
		for (int i = 0; i < stars_.Length; i++)
		{
			stars_[i].SetActive(i < star);
		}
		UpdateCharaIcon();
		setItemPos(itemsRoot_, stageInfo_.Common);
		resetFreeTicket(items_);
		setupItem(items_, stageInfo_.Common);
		setupPricelessTicket(items_, stageInfo_.Common, noPaymentItemType);
		judgeNewIconItem(stageNo);
		for (int j = 0; j < items_.Length; j++)
		{
			BoostItem item = items_[j];
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
		setupClearCondition(base.transform, stageInfo_.Common);
		Transform FriendHelp = base.transform.Find("FriendHelp");
		bool bAreaSale = false;
		if (skipTrans_ != null)
		{
			Debug.Log("PlayCount = " + Bridge.StageData.getPlayCount(stageNo) + "     isClear = " + Bridge.StageData.isClear(stageNo) + "    LastStage = " + part_.isLastStage(stageNo));
			bool bSkipActive = Bridge.StageData.getPlayCount(stageNo) >= 1 && GlobalData.Instance.getGameData().isSkip && !Bridge.StageData.isClear(stageNo) && !part_.isLastStage(stageNo);
			skipTrans_.gameObject.SetActive(bSkipActive);
		}
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
		if (gameData.helpDataSize > 0)
		{
			FriendHelp.gameObject.SetActive(true);
			bool bTimeStage = stageInfo_.Common.Time > 0;
			FriendHelp.Find("fonts/time").gameObject.SetActive(bTimeStage);
			FriendHelp.Find("fonts/bubble").gameObject.SetActive(!bTimeStage);
			FriendHelp.Find("icons/time").gameObject.SetActive(bTimeStage);
			FriendHelp.Find("icons/bubble").gameObject.SetActive(!bTimeStage);
			MessageResource msgRes = MessageResource.Instance;
			UILabel Label_number = FriendHelp.Find("Label_number").GetComponent<UILabel>();
			if (bTimeStage)
			{
				Label_number.text = msgRes.castCtrlCode(msgRes.getMessage(49), 1, (gameData.helpDataSize * gameData.helpTime).ToString());
			}
			else
			{
				Label_number.text = msgRes.castCtrlCode(msgRes.getMessage(50), 1, (gameData.helpDataSize * gameData.helpMove).ToString());
			}
		}
		else
		{
			FriendHelp.gameObject.SetActive(false);
		}
		yield return dialogManager_.StartCoroutine(_show(stageNo));
		if (bAreaSale)
		{
			StartCoroutine("updateSaleIcons");
		}
		Input.enable = true;
	}

	public IEnumerator playStageSkipTutorial(TutorialDataTable.eSPTutorial tutorialNo)
	{
		skipTrans_.GetComponentInChildren<BoxCollider>().enabled = false;
		skipTrans_.localPosition = new Vector3(skipTrans_.localPosition.x, skipTrans_.localPosition.y, -100f);
		TutorialManager.Instance.load((int)tutorialNo, part_.uiRoot);
		yield return StartCoroutine(TutorialManager.Instance.play((int)tutorialNo, TutorialDataTable.ePlace.Setup, part_.uiRoot, null, null));
		TutorialManager.Instance.unload((int)tutorialNo);
		skipTrans_.localPosition = new Vector3(skipTrans_.localPosition.x, skipTrans_.localPosition.y, 0f);
		skipTrans_.GetComponentInChildren<BoxCollider>().enabled = true;
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
			BoostItem item = trigger.transform.parent.GetComponent<BoostItem>();
			yield return StartCoroutine(pressItemButton(items_, item));
			if (item.getState() != BoostItem.eState.OFF)
			{
				yield break;
			}
			BoostItem[] array = items_;
			foreach (BoostItem temp in array)
			{
				if (temp != item && temp.getItemType() != Constant.Item.eType.Invalid)
				{
					item.syncSaleIconUpdater(temp);
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
			if (heart < 1)
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
					BoostItem[] array3 = items_;
					foreach (BoostItem item3 in array3)
					{
						item3.setState(BoostItem.eState.OFF);
					}
					yield return StartCoroutine(part_.updateSaleAreaData());
					yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
					onUpdateSale();
				}
				break;
			}
			BoostItem[] array4 = items_;
			foreach (BoostItem item4 in array4)
			{
				if (item4.getState() == BoostItem.eState.ON)
				{
					if (item4.getPriceType() == Constant.eMoney.Jewel)
					{
						Tapjoy.TrackEvent("Money", "Expense Jewel", "Item Lobby Jewel", null, item4.getPrice());
						Tapjoy.TrackEvent("Game Item", "Lobby Jewel", "Stage No - " + (stageNo + 1), item4.getItemType().ToString(), "Use Jewel", item4.getPrice(), null, 0L, null, 0L);
						GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Jewel", "Item Lobby Jewel", item4.getPrice());
						GlobalGoogleAnalytics.Instance.LogEvent("Item Lobby Jewel", item4.getItemType().ToString(), "Stage No - " + (stageNo + 1), item4.getPrice());
						GameAnalytics.traceMoneyConsumption("USE_JEWEL(" + item4.getItemType().ToString() + ")", "0", item4.getPrice(), Bridge.PlayerData.getCurrentStage());
						Plugin.Instance.buyCompleted("USE_JEWEL(" + item4.getItemType().ToString() + ")");
					}
					else if (item4.getPriceType() == Constant.eMoney.Coin)
					{
						Tapjoy.TrackEvent("Money", "Expense Coin", "Item Lobby Coin", null, item4.getPrice());
						Tapjoy.TrackEvent("Game Item", "Lobby Coin", "Stage No - " + (stageNo + 1), item4.getItemType().ToString(), "Use Coin", item4.getPrice(), null, 0L, null, 0L);
						GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Coin", "Item Lobby Coin", item4.getPrice());
						GlobalGoogleAnalytics.Instance.LogEvent("Item Lobby Coin", item4.getItemType().ToString(), "Stage No - " + (stageNo + 1), item4.getPrice());
						GameAnalytics.traceMoneyConsumption("USE_COIN(" + item4.getItemType().ToString() + ")", "1", item4.getPrice(), Bridge.PlayerData.getCurrentStage());
						Plugin.Instance.buyCompleted("USE_COIN(" + item4.getItemType().ToString() + ")");
					}
				}
			}
			Tapjoy.TrackEvent("Charactor", "Use", GlobalData.Instance.currentAvatar.index.ToString(), "Stage : " + (stageNo + 1), 0L);
			GlobalGoogleAnalytics.Instance.LogEvent("Charactor Use", GlobalData.Instance.currentAvatar.index.ToString(), GlobalData.Instance.currentAvatar.level.ToString(), 1L);
			Plugin.Instance.buyCompleted("USE_CHARACTOR(" + GlobalData.Instance.currentAvatar.index + ")");
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
			if (Scenario.Instance.isScenario(stageNo, Scenario.ePlace.Begin))
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
			GlobalData.Instance.set_acInfo(true, DialogManager.eDialog.Setup, part_, stageIcon_);
			GlobalData.Instance.bSetItems = null;
			yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarCollection(null));
			break;
		case "skip_button":
			Constant.SoundUtil.PlayButtonSE();
			yield return StartCoroutine(openStageSkipDialog());
			if (part_.bStageSkip_)
			{
				BoostItem[] array2 = items_;
				foreach (BoostItem item2 in array2)
				{
					item2.setState(BoostItem.eState.OFF);
				}
				yield return StartCoroutine(part_.closeSetupDialog(this, scoreDialog_, stageIcon_.getStageNo()));
			}
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			StopCoroutine("updateSaleIcons");
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(scoreDialog_));
			part_.dailyMissionValueSetting();
			if (part_.dailyMission.gameObject.activeSelf)
			{
				yield return StartCoroutine(part_.dailyMission.dailyMissionInfoSetup());
			}
			if (part_.isInitialized())
			{
				StartCoroutine(part_.dailyMissionClearCheck());
			}
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

	public int getSetItemJewel()
	{
		return totalAmount(items_, Constant.eMoney.Jewel);
	}

	public void OnApplicationResumeSetupDialog()
	{
		Constant.PlayerData.addCoin(-getSetItemCoin());
		Constant.PlayerData.subJewel(getSetItemJewel());
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
				boostItem.setPrice(boostItem.getPriceType(), boostItem.getOriginalPrice() * gameData.areaSalePercent / 100);
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
		blowcheck = ResourceLoader.Instance.isUseLowResource();
		float chara_img_expand_rate = GlobalData.Instance.chara_img_expand_rate;
		string text = ((GlobalData.Instance.currentAvatar.throwCharacter <= 0) ? string.Empty : ("_" + (GlobalData.Instance.currentAvatar.throwCharacter - 1).ToString("00")));
		string text2 = ((GlobalData.Instance.currentAvatar.supportCharacter <= 0) ? string.Empty : ("_" + (GlobalData.Instance.currentAvatar.supportCharacter - 1).ToString("00")));
		if (GlobalData.Instance.currentAvatar.throwCharacter - 1 > 18)
		{
			chara_00.atlas = chara_03.atlas;
		}
		else
		{
			chara_00.atlas = chara_02.atlas;
		}
		if (GlobalData.Instance.currentAvatar.supportCharacter - 1 > 18)
		{
			chara_01.atlas = chara_03.atlas;
		}
		else
		{
			chara_01.atlas = chara_02.atlas;
		}
		chara_00.spriteName = "avatar_00" + text + "_00";
		chara_01.spriteName = "avatar_01" + text2 + "_00";
		chara_00.MakePixelPerfect();
		chara_01.MakePixelPerfect();
		chara_00.transform.localScale = new Vector3(chara_00.transform.localScale.x * chara_img_expand_rate, chara_00.transform.localScale.y * chara_img_expand_rate, chara_00.transform.localScale.z);
		chara_01.transform.localScale = new Vector3(chara_01.transform.localScale.x * chara_img_expand_rate, chara_01.transform.localScale.y * chara_img_expand_rate, chara_01.transform.localScale.z);
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
				boostItem.setPrice(Constant.eMoney.Coin, boostItem.getOriginalPrice() * gameData.areaSalePercent / 100);
			}
		}
		updateSaleIconsInner();
	}
}
