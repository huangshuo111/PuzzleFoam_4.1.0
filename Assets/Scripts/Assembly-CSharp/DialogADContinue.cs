using System.Collections;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using TnkAd;
using UnityEngine;

public class DialogADContinue : DialogShortageBase
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

	private bool bADContinue;

	private bool biOSADCheck;

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
			case "ContinueIcon":
				continueIcon_ = transform.GetComponent<ContinueIcon>();
				break;
			case "Label_num":
				restNumLabel = transform.GetComponent<UILabel>();
				break;
			case "Label_rest":
				restLabel = transform.GetComponent<UILabel>();
				break;
			}
		}
	}

	public IEnumerator show(StageInfo.CommonInfo stageInfo, Part_Stage.eGameType gameType, Part_Stage.eGameover gameoverType, int clearState, int eventNo, int restNum)
	{
		bADContinue = false;
		gameoverType_ = gameoverType;
		commonInfo_ = stageInfo;
		Sound.Instance.playSe(Sound.eSe.SE_236_slow);
		MessageResource msgRes = MessageResource.Instance;
		continueIcon_.setup_AD(stageInfo, gameoverType);
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
		checkBox_.setup(stageInfo, clearState, labels_[0]);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		while (isOpen())
		{
			yield return null;
		}
		base.close();
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

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "ContinueButton":
			Constant.SoundUtil.PlayDecideSE();
			yield return StartCoroutine(ShowVideo());
			GameObject.Find("PB_TNKVidioHandler").GetComponent<PBTnkVidioHandler>().setContinueUpdate(true);
			if (SaveData.Instance.getSystemData().getOptionData().getFlag(SaveOptionData.eFlag.BGM))
			{
				Sound.Instance.setBgmMasterVolume(Sound.Instance.getDefaultBgmVolume());
				Sound.Instance.setBgmVolume(Sound.Instance.getDefaultBgmVolume());
			}
			bADContinue = true;
			yield return StartCoroutine(isContinuePlay());
			break;
		case "Close_Button":
		{
			Constant.SoundUtil.PlayCancelSE();
			DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
			dialog.setup(null, null, true);
			dialog.setMessage(MessageResource.Instance.getMessage(600008));
			yield return StartCoroutine(dialog.open());
			while (dialog.isOpen())
			{
				yield return 0;
			}
			if (dialog.result_ != DialogCommon.eResult.Cancel)
			{
				bADContinue = false;
				dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
				result = eResult.Close;
			}
			break;
		}
		}
	}

	private IEnumerator isContinuePlay()
	{
		int stageNo = commonInfo_.StageNo;
		if (gameoverType_ == Part_Stage.eGameover.HitSkull)
		{
			NetworkMng.Instance.setup(Hash.StageContinueByAD(stageNo, 1));
		}
		else
		{
			NetworkMng.Instance.setup(Hash.StageContinueByAD(stageNo, 0));
		}
		yield return StartCoroutine(NetworkMng.Instance.download(API.StageContinueByAd, true, true));
		WWW www = NetworkMng.Instance.getWWW();
		Debug.Log("www : URL : " + www.url);
		JsonData json = JsonMapper.ToObject(www.text);
		GameData gameData = GlobalData.Instance.getGameData();
		CommonData commonData = JsonMapper.ToObject<CommonData>(www.text);
		gameData.setCommonData(commonData, false);
		gameData.continueNum = (int)json["continueNum"];
		Vungle.onAdFinishedEvent -= VungleAdCheckEvent;
		Plugin.Instance.buyCompleted("USE_ADVIDIO_CONTINUE");
		Tapjoy.TrackEvent("Game Item", "Ingame", "Stage No - " + (stageNo + 1), "ADVIDIO_CONTINUE", 0L);
		GlobalGoogleAnalytics.Instance.LogEvent("Item Ingame", "ADVIDIO_CONTINUE", "Stage No - " + (stageNo + 1), continueIcon_.getPrice());
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

	public bool GetADContinue()
	{
		return bADContinue;
	}

	private IEnumerator ShowVideo()
	{
		Sound.Instance.setBgmMasterVolume(0f);
		Sound.Instance.setBgmVolume(0f);
		int CheckNum = Random.Range(0, 2);
		int VidioNum = 0;
		switch (CheckNum)
		{
		case 0:
			if (Plugin.Instance.hasVideoAd("PB_Video_AD"))
			{
				UnityEngine.Debug.Log("TNK showVideoAd!!");
				Plugin.Instance.showVideoAd("PB_Video_AD");
				VidioNum = 1;
			}
			else if (Vungle.isAdvertAvailable())
			{
				UnityEngine.Debug.Log("Vungle showVideoAd!!");
				Vungle.playAd(false, string.Empty);
				VidioNum = 2;
			}
			else
			{
				GKUnityPluginController.Instance.ToastMessage(MessageResource.Instance.getMessage(500041));
			}
			break;
		case 1:
			if (Vungle.isAdvertAvailable())
			{
				UnityEngine.Debug.Log("Vungle showVideoAd!!");
				Vungle.playAd(false, string.Empty);
				VidioNum = 2;
			}
			else if (Plugin.Instance.hasVideoAd("PB_Video_AD"))
			{
				UnityEngine.Debug.Log("TNK showVideoAd!!");
				Plugin.Instance.showVideoAd("PB_Video_AD");
				VidioNum = 1;
			}
			else
			{
				GKUnityPluginController.Instance.ToastMessage(MessageResource.Instance.getMessage(500041));
			}
			break;
		}
		switch (VidioNum)
		{
		case 1:
			Tapjoy.TrackEvent("AD Vidio", "Continue", "TNK", null, 0L);
			GlobalGoogleAnalytics.Instance.LogEvent("AD Vidio", "Continue", "TNK", 0L);
			break;
		case 2:
			Tapjoy.TrackEvent("AD Vidio", "Continue", "Vungle", null, 0L);
			GlobalGoogleAnalytics.Instance.LogEvent("AD Vidio", "Continue", "Vungle", 0L);
			break;
		case 0:
			Tapjoy.TrackEvent("AD Vidio", "Continue", "None Vidio", null, 0L);
			GlobalGoogleAnalytics.Instance.LogEvent("AD Vidio", "Continue", "None Vidio", 0L);
			break;
		}
		yield break;
	}

	private IEnumerator CheckVidioView()
	{
		do
		{
			yield return null;
		}
		while (!biOSADCheck);
	}

	public void VungleAdCheckEvent(AdFinishedEventArgs args)
	{
		UnityEngine.Debug.Log(" args.TimeWatched : " + args.TimeWatched);
		UnityEngine.Debug.Log(" args.TotalDuration : " + args.TotalDuration);
		UnityEngine.Debug.Log(" args.WasCallToActionClicked : " + args.WasCallToActionClicked);
		UnityEngine.Debug.Log(" args.IsCompletedView : " + args.IsCompletedView);
		biOSADCheck = true;
	}
}
