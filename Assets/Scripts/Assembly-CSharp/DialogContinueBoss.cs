using System.Collections;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using TnkAd;
using UnityEngine;

public class DialogContinueBoss : DialogShortageBase
{
	private enum eLabel
	{
		Target = 0,
		Jewel = 1,
		Coin = 2,
		BossHP = 3,
		addTime = 4,
		Detail = 5,
		Max = 6
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

	private UILabel[] labels_ = new UILabel[6];

	private GameObject[] plusObjecats_ = new GameObject[2];

	private ContinueIcon continueIcon_;

	public BossBase bossBase_;

	private UISlider bossHPSlider;

	private Part_Stage.eGameover gameoverType_ = Part_Stage.eGameover.TimeOver;

	private StageInfo.CommonInfo commonInfo_;

	private ClearCheckBox checkBox_;

	private int bossType_;

	private int bossLevel_;

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
				plusObjecats_[1] = transform.gameObject;
				break;
			case "02_coin":
				plusObjecats_[0] = transform.gameObject;
				break;
			case "ContinueIcon":
				continueIcon_ = transform.GetComponent<ContinueIcon>();
				break;
			case "Label_BossHP":
				labels_[3] = transform.GetComponent<UILabel>();
				break;
			case "Bossgauge":
				bossHPSlider = transform.gameObject.GetComponent<UISlider>();
				break;
			case "Label_Add":
				labels_[4] = transform.GetComponent<UILabel>();
				break;
			case "Label_detail":
				labels_[5] = transform.GetComponent<UILabel>();
				break;
			case "addbubbles":
				transform.gameObject.SetActive(false);
				break;
			case "addreplay":
				transform.gameObject.SetActive(true);
				break;
			case "addtime":
				transform.gameObject.SetActive(false);
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

	public IEnumerator showBossStage(int bossType, int bossLevel, StageInfo.CommonInfo stageInfo, Part_BossStage.eGameover gameoverType, int clearState)
	{
		commonInfo_ = stageInfo;
		bossType_ = bossType;
		bossLevel_ = bossLevel;
		MessageResource msgRes = MessageResource.Instance;
		continueIcon_.setupBoss(stageInfo, gameoverType);
		labels_[0].text = string.Empty;
		labels_[5].text = MessageResource.Instance.getMessage(2563);
		labels_[4].text = string.Empty;
		bossHPSlider.numberOfSteps = bossBase_.bossHpSlider.numberOfSteps;
		bossHPSlider.sliderValue = (float)bossBase_.currentHP / (float)bossBase_.maxHP;
		GameObject[] array = plusObjecats_;
		foreach (GameObject obj in array)
		{
			if (obj != null)
			{
				obj.SetActive(false);
			}
		}
		if (continueIcon_.getPriceType() == Constant.eMoney.Coin)
		{
			plusObjecats_[0].SetActive(true);
		}
		else
		{
			plusObjecats_[1].SetActive(true);
		}
		updateLabel();
		checkBox_.gameObject.SetActive(false);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
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
			if (continueIcon_.getPriceType() == Constant.eMoney.Jewel)
			{
				int jewel = Bridge.PlayerData.getJewel();
				if (jewel < continueIcon_.getPrice())
				{
					yield return StartCoroutine(base.show(eType.Jewel));
					break;
				}
			}
			else
			{
				int coin = Bridge.PlayerData.getCoin();
				if (coin < continueIcon_.getPrice())
				{
					yield return StartCoroutine(base.show(eType.Coin));
					break;
				}
			}
			int stageNo = commonInfo_.StageNo;
			ContinueSaleData cntData = GlobalData.Instance.getContinueData();
			int cntType = 1;
			if (cntData.isContinueCampaign)
			{
				cntType = 2;
			}
			NetworkMng.Instance.setup(Hash.BossStageContinue(Constant.Boss.convBossInfoToNo(bossType_, 0), bossLevel_, cntType, cntData.isContinueChance));
			yield return StartCoroutine(NetworkMng.Instance.download(API.BossStageContinue, true, true));
			if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
			{
				if (NetworkMng.Instance.getResultCode() == eResultCode.ContinueSaleHadFinished)
				{
					cntData.isContinueCampaign = !cntData.isContinueCampaign;
					GlobalData.Instance.setContinueData(cntData);
					continueIcon_.setup(commonInfo_, gameoverType_);
				}
				break;
			}
			WWW www = NetworkMng.Instance.getWWW();
			JsonData json = JsonMapper.ToObject(www.text);
			GameData gameData = GlobalData.Instance.getGameData();
			CommonData commonData = JsonMapper.ToObject<CommonData>(www.text);
			gameData.setCommonData(commonData, false);
			gameData.continueNum = (int)json["continueNum"];
			Plugin.Instance.buyCompleted("USE_JEWEL_BOSS_CONTINUE");
			Tapjoy.TrackEvent("Money", "Expense Jewel", "CONTINUE", null, continueIcon_.getPrice());
			Tapjoy.TrackEvent("Game Item", "Ingame", "Boss Stage", "BOSS_CONTINUE", 0L);
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Jewel", "CONTINUE", continueIcon_.getPrice());
			GlobalGoogleAnalytics.Instance.LogEvent("Item Ingame", "BOSS_CONTINUE", "Boss Stage", continueIcon_.getPrice());
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			result = eResult.Continue;
			break;
		}
		case "Close_Button":
		{
			Constant.SoundUtil.PlayCancelSE();
			ContinueSaleData continueData = GlobalData.Instance.getContinueData();
			if (continueData.isContinueChance && !isFreeContinue())
			{
				DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
				dialog.setup(null, null, true);
				dialog.setMessage(MessageResource.Instance.getMessage(2504));
				yield return StartCoroutine(dialog.open());
				while (dialog.isOpen())
				{
					yield return 0;
				}
				if (dialog.result_ == DialogCommon.eResult.Cancel)
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

	private void Start()
	{
	}

	private void Update()
	{
	}
}
