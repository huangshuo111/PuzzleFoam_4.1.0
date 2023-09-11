using System.Collections;
using Bridge;
using LitJson;
using Network;
using UnityEngine;

public class DialogRankingContinue : DialogShortageBase
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

	private UILabel[] labels_ = new UILabel[3];

	private GameObject[] plusObjecats_ = new GameObject[2];

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
				plusObjecats_[1] = transform.gameObject;
				break;
			case "02_coin":
				plusObjecats_[0] = transform.gameObject;
				break;
			case "ContinueIcon":
				continueIcon_ = transform.GetComponent<ContinueIcon>();
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

	public IEnumerator show(StageInfo.CommonInfo stageInfo)
	{
		commonInfo_ = stageInfo;
		Sound.Instance.playSe(Sound.eSe.SE_236_slow);
		MessageResource msgRes = MessageResource.Instance;
		continueIcon_.setup(stageInfo);
		base.transform.Find("window/chara/bg_normal").gameObject.SetActive(true);
		if (continueIcon_.getPriceType() == Constant.eMoney.Coin)
		{
			plusObjecats_[0].SetActive(true);
		}
		else
		{
			plusObjecats_[1].SetActive(true);
		}
		updateLabel();
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		ContinueSaleData continueData = GlobalData.Instance.getContinueData();
		if (continueData.isContinueChance && bShowChanceTutorial)
		{
			bShowChanceTutorial = false;
			Hashtable args = new Hashtable();
			args["GameOverType"] = gameoverType_;
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
			if (isFreeContinue())
			{
				cntType = 0;
			}
			else if (cntData.isContinueCampaign)
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
			}
			else
			{
				WWW www = NetworkMng.Instance.getWWW();
				JsonData json = JsonMapper.ToObject(www.text);
				GameData gameData = GlobalData.Instance.getGameData();
				CommonData commonData = JsonMapper.ToObject<CommonData>(www.text);
				gameData.setCommonData(commonData, false);
				gameData.continueNum = (int)json["continueNum"];
				dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
				result = eResult.Continue;
			}
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
			DialogAllShop dialog2 = dialogManager_.getDialog(DialogManager.eDialog.AllShop) as DialogAllShop;
			dialogManager_.StartCoroutine(dialog2.show(DialogAllShop.ePanelType.Jewel));
		}
		else
		{
			DialogAllShop dialog = dialogManager_.getDialog(DialogManager.eDialog.AllShop) as DialogAllShop;
			dialogManager_.StartCoroutine(dialog.show(DialogAllShop.ePanelType.Coin));
		}
		yield break;
	}

	public void updateCampaign(bool isCampaign)
	{
		Transform transform = base.transform.Find("window/01_jewel/campaign");
		if (!(transform == null))
		{
			transform.gameObject.SetActive(isCampaign);
		}
	}

	private void LateUpdate()
	{
		updateLabel();
	}
}
