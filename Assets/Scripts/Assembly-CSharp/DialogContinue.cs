using System.Collections;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using TnkAd;
using UnityEngine;

public class DialogContinue : DialogShortageBase
{
	private enum eLabel
	{
		Target = 0,
		Jewel = 1,
		Coin = 2,
		Max = 3
	}

	private enum eMoneyIcon
	{
		Coin = 0,
		Jewel = 1,
		Max = 2
	}

	public enum eResult
	{
		Continue = 0,
		Close = 1
	}

	private UILabel restLabel;

	private UILabel restNumLabel;

	private UILabel[] labels_ = new UILabel[3];

	private GameObject[] plusObjects_ = new GameObject[2];

	private ContinueIcon continueIcon_;

	private Part_Stage.eGameover gameoverType_ = Part_Stage.eGameover.TimeOver;

	private StageInfo.CommonInfo commonInfo_;

	private ClearCheckBox checkBox_;

	private bool bShowChanceTutorial = true;

	public eResult result { get; private set; }

	public override void OnCreate()
	{
		createCB();
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			switch (transform.name)
			{
			case "Label_Condition":
				labels_[0] = transform.GetComponent<UILabel>();
				break;
			case "Checkboxs":
				checkBox_ = transform.GetComponent<ClearCheckBox>();
				break;
			case "Label_Jewel":
				labels_[1] = transform.GetComponent<UILabel>();
				break;
			case "Label_coin":
				labels_[2] = transform.GetComponent<UILabel>();
				break;
			case "01_jewel":
				plusObjects_[1] = transform.gameObject;
				break;
			case "02_coin":
				plusObjects_[0] = transform.gameObject;
				break;
			case "ContinueIcon":
				continueIcon_ = transform.GetComponent<ContinueIcon>();
				break;
			case "Label_num":
				restNumLabel = transform.GetComponent<UILabel>();
				break;
			case "Label_rest":
				restLabel = transform.GetComponent<UILabel>();
				break;
			case "campaign":
				if (!(transform.parent.name != "01_jewel") && !GlobalData.Instance.getGameData().isJewelCampaign)
				{
					transform.gameObject.SetActive(false);
				}
				break;
			}
		}
	}

	public IEnumerator show(StageInfo.CommonInfo stageInfo, Part_Stage.eGameType gameType, Part_Stage.eGameover gameoverType, int clearState, int eventNo, int restNum)
	{
		gameoverType_ = gameoverType;
		commonInfo_ = stageInfo;
		Sound.Instance.playSe(Sound.eSe.SE_236_slow);
		MessageResource msgRes = MessageResource.Instance;
		continueIcon_.setup(stageInfo, gameoverType);
		if (eventNo == 2)
		{
			base.transform.Find("window/chara/bg_normal").gameObject.SetActive(false);
			base.transform.Find("window/chara/bg_challenge").gameObject.SetActive(true);
		}
		else
		{
			base.transform.Find("window/chara/bg_normal").gameObject.SetActive(true);
			base.transform.Find("window/chara/bg_challenge").gameObject.SetActive(false);
		}
		labels_[0].text = Constant.MessageUtil.getTargetMsg(stageInfo, msgRes, Constant.MessageUtil.eTargetType.Continue);
		if (restNum >= 0)
		{
			restLabel.gameObject.SetActive(true);
			restNumLabel.gameObject.SetActive(true);
			switch (gameType)
			{
			case Part_Stage.eGameType.ShotCount:
				restLabel.text = MessageResource.Instance.getMessage(3730);
				break;
			case Part_Stage.eGameType.Time:
				restLabel.text = MessageResource.Instance.getMessage(3731);
				break;
			}
			restNumLabel.text = restNum.ToString();
		}
		else
		{
			restLabel.gameObject.SetActive(false);
			restNumLabel.gameObject.SetActive(false);
		}
		GameObject[] array = plusObjects_;
		foreach (GameObject obj in array)
		{
			if (obj != null)
			{
				obj.SetActive(false);
			}
		}
		if (continueIcon_.getPriceType() == Constant.eMoney.Coin)
		{
			plusObjects_[0].SetActive(true);
		}
		else
		{
			plusObjects_[1].SetActive(true);
		}
		updateLabel();
		checkBox_.setup(stageInfo, clearState, labels_[0]);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		if (isFreeContinue())
		{
			Hashtable args2 = new Hashtable();
			args2["GameOverType"] = gameoverType;
			TutorialManager.Instance.load(-3, dialogManager_.getCurrentUiRoot());
			yield return StartCoroutine(TutorialManager.Instance.play(-3, TutorialDataTable.ePlace.Continue, dialogManager_.getCurrentUiRoot(), stageInfo, args2));
			yield break;
		}
		ContinueSaleData continueData = GlobalData.Instance.getContinueData();
		if (continueData.isContinueChance && bShowChanceTutorial)
		{
			bShowChanceTutorial = false;
			Hashtable args = new Hashtable();
			args["GameOverType"] = gameoverType;
			TutorialManager.Instance.load(-9, dialogManager_.getCurrentUiRoot());
			yield return StartCoroutine(TutorialManager.Instance.play(-9, TutorialDataTable.ePlace.Continue, dialogManager_.getCurrentUiRoot(), stageInfo, args));
		}
	}

	public IEnumerator showBossStage(StageInfo.CommonInfo stageInfo, Part_BossStage.eGameover gameoverType, int clearState)
	{
		Sound.Instance.playSe(Sound.eSe.SE_236_slow);
		MessageResource msgRes = MessageResource.Instance;
		base.transform.Find("window/chara/bg_normal").gameObject.SetActive(true);
		base.transform.Find("window/chara/bg_challenge").gameObject.SetActive(false);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		if (isFreeContinue())
		{
			Hashtable args2 = new Hashtable();
			args2["GameOverType"] = gameoverType;
			TutorialManager.Instance.load(-3, dialogManager_.getCurrentUiRoot());
			yield return StartCoroutine(TutorialManager.Instance.play(-3, TutorialDataTable.ePlace.Continue, dialogManager_.getCurrentUiRoot(), stageInfo, args2));
			yield break;
		}
		ContinueSaleData continueData = GlobalData.Instance.getContinueData();
		if (continueData.isContinueChance && bShowChanceTutorial)
		{
			bShowChanceTutorial = false;
			Hashtable args = new Hashtable();
			args["GameOverType"] = gameoverType;
			TutorialManager.Instance.load(-9, dialogManager_.getCurrentUiRoot());
			yield return StartCoroutine(TutorialManager.Instance.play(-9, TutorialDataTable.ePlace.Continue, dialogManager_.getCurrentUiRoot(), stageInfo, args));
		}
	}

	public ContinueIcon getContinueIcon()
	{
		return continueIcon_;
	}

	public void updateLabel()
	{
		int jewel = Bridge.PlayerData.getJewel();
		int coin = Bridge.PlayerData.getCoin();
		labels_[1].text = jewel.ToString();
		if (labels_[2] != null)
		{
			labels_[2].text = coin.ToString("N0");
		}
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
		case "ContinueButton":
		{
			Constant.SoundUtil.PlayDecideSE();
			DialogCommon dialog2 = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
			dialog2.setup(MessageResource.Instance.getMessage(500026), isContinuePlay, null, true);
			yield return StartCoroutine(dialog2.open());
			while (dialog2.isOpen())
			{
				yield return null;
			}
			break;
		}
		case "Close_Button":
		{
			Constant.SoundUtil.PlayCancelSE();
			ContinueSaleData continueData = GlobalData.Instance.getContinueData();
			if (continueData.isContinueChance && !isFreeContinue())
			{
				DialogCommon dialog2 = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
				dialog2.setup(null, null, true);
				dialog2.setMessage(MessageResource.Instance.getMessage(2504));
				yield return StartCoroutine(dialog2.open());
				while (dialog2.isOpen())
				{
					yield return 0;
				}
				if (dialog2.result_ == DialogCommon.eResult.Cancel)
				{
					break;
				}
			}
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			result = eResult.Close;
			break;
		}
		}
	}

	private IEnumerator isContinuePlay()
	{
		if (continueIcon_.getPriceType() == Constant.eMoney.Jewel)
		{
			int jewel = Bridge.PlayerData.getJewel();
			if (jewel < continueIcon_.getPrice())
			{
				yield return StartCoroutine(base.show(eType.Jewel));
				yield break;
			}
		}
		else
		{
			int coin = Bridge.PlayerData.getCoin();
			if (coin < continueIcon_.getPrice())
			{
				yield return StartCoroutine(base.show(eType.Coin));
				yield break;
			}
		}
		int stageNo = commonInfo_.StageNo;
		ContinueSaleData cntData = GlobalData.Instance.getContinueData();
		int cntType = 1;
		if (isFreeContinue())
		{
			cntType = 0;
		}
		else if (cntData.isContinueCampaign && (stageNo < 40000 || Constant.ParkStage.isParkStage(stageNo)))
		{
			cntType = 2;
		}
		if (gameoverType_ == Part_Stage.eGameover.HitSkull)
		{
			NetworkMng.Instance.setup(Hash.StageReplay(stageNo, cntType, cntData.isContinueChance));
			yield return StartCoroutine(NetworkMng.Instance.download(API.StageReplay, true, true));
		}
		else
		{
			NetworkMng.Instance.setup(Hash.StageContinue(stageNo, cntType, cntData.isContinueChance));
			yield return StartCoroutine(NetworkMng.Instance.download(API.StageContinue, true, true));
		}
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			if (NetworkMng.Instance.getResultCode() == eResultCode.ContinueSaleHadFinished)
			{
				cntData.isContinueCampaign = !cntData.isContinueCampaign;
				GlobalData.Instance.setContinueData(cntData);
				continueIcon_.setup(commonInfo_, gameoverType_);
			}
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		JsonData json = JsonMapper.ToObject(www.text);
		GameData gameData = GlobalData.Instance.getGameData();
		CommonData commonData = JsonMapper.ToObject<CommonData>(www.text);
		gameData.setCommonData(commonData, false);
		gameData.continueNum = (int)json["continueNum"];
		Plugin.Instance.buyCompleted("USE_JEWEL_CONTINUE");
		Tapjoy.TrackEvent("Money", "Expense Jewel", "CONTINUE", null, continueIcon_.getPrice());
		Tapjoy.TrackEvent("Game Item", "Ingame", "Stage No - " + (stageNo + 1), "CONTINUE", 0L);
		GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Jewel", "CONTINUE", continueIcon_.getPrice());
		GlobalGoogleAnalytics.Instance.LogEvent("Item Ingame", "CONTINUE", "Stage No - " + (stageNo + 1), continueIcon_.getPrice());
		dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
		result = eResult.Continue;
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

	private IEnumerator pressPlusButton(GameObject trigger)
	{
		Constant.SoundUtil.PlayButtonSE();
		string parentName = trigger.transform.parent.name;
		if (parentName == "01_jewel")
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

	public void updateCampaign(bool isCampaign)
	{
		Transform transform = base.transform.Find("window/01_jewel/campaign");
		if (!(transform == null))
		{
			transform.gameObject.SetActive(isCampaign);
		}
	}
}
