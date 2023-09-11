using System;
using System.Collections;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using UnityEngine;

public class DialogBonusRoulette : DialogShortageBase
{
	public class ExpressionType
	{
		public const int FirstRoulette = 0;

		public const int SecondRoulette = 1;

		public const int SecondResult = 2;

		public const int Max = 3;
	}

	public class ExpressionLabel
	{
		public const int Explanation = 0;

		public const int BonusStageGetCoin = 1;

		public const int LevelBonus = 2;

		public const int Ratio = 3;

		public const int totalCoin_ = 4;

		public const int Max = 5;
	}

	public class WindowPanel
	{
		public GameObject window;

		public UIButton button;

		public UILabel[] labels = new UILabel[5];
	}

	private const int CellMax = 7;

	private WindowPanel[] panels_;

	private int currentPanel_;

	private UIButton cancelButton_;

	private UIButton receiveButton_;

	private UILabel levelBonus_;

	private UILabel plusLabel_;

	private Transform[] chara = new Transform[2];

	private Transform rotate;

	private int stageCoin_;

	private int levelBonusCoin_;

	private int totalCoin_;

	private int bufferGetCoin;

	private int bufferBonusCoin;

	private int multipleNum;

	private int freeRouletteResult;

	private int[] pattern = new int[7] { 2, 2, 2, 2, 2, 2, 2 };

	private UILabel[] rouletteLabel = new UILabel[7];

	private GameObject[] rouletteCell = new GameObject[7];

	private int maxMaginification;

	private int rubbyNum;

	private UILabel jewelLabel_;

	private BonuseRouletteData[] rouletteList_;

	private BonusStartData startData_;

	private BonuseStageResult resultData_;

	private StageResult Result;

	private bool bRoulette;

	private void Awake()
	{
		panels_ = new WindowPanel[3];
		for (int i = 0; i < panels_.Length; i++)
		{
			panels_[i] = new WindowPanel();
		}
		Transform transform = base.transform.Find("window/Close_Button");
		if (transform != null)
		{
			cancelButton_ = transform.gameObject.GetComponent<UIButton>();
		}
		for (int j = 0; j < 3; j++)
		{
			transform = base.transform.Find("window/BonusChance_" + j.ToString("00"));
			if (transform != null)
			{
				panels_[j].window = transform.gameObject;
			}
			transform = base.transform.Find("window/BonusChance_" + j.ToString("00") + "/ConfirmButton");
			if (transform != null)
			{
				panels_[j].button = transform.gameObject.GetComponent<UIButton>();
			}
			transform = base.transform.Find("window/BonusChance_" + j.ToString("00") + "/Label_00");
			if (transform != null)
			{
				panels_[j].labels[0] = transform.gameObject.GetComponent<UILabel>();
			}
			for (int k = 1; k < 5; k++)
			{
				transform = base.transform.Find("window/BonusChance_" + j.ToString("00") + "/result/number_label_" + (k - 1).ToString("00"));
				if (transform != null)
				{
					panels_[j].labels[k] = transform.gameObject.GetComponent<UILabel>();
				}
			}
		}
		transform = base.transform.Find("window/BonusChance_01/ConfirmButton_00");
		if (transform != null)
		{
			receiveButton_ = transform.gameObject.GetComponent<UIButton>();
		}
		transform = base.transform.Find("window/BonusChance_01/result/lv_bonus_label");
		if (transform != null)
		{
			levelBonus_ = transform.gameObject.GetComponent<UILabel>();
		}
		transform = base.transform.Find("window/BonusChance_01/result/plus_label");
		if (transform != null)
		{
			plusLabel_ = transform.gameObject.GetComponent<UILabel>();
		}
		transform = base.transform.Find("window/roulette/rotate");
		if (transform != null)
		{
			rotate = transform;
		}
		for (int l = 0; l < 7; l++)
		{
			transform = rotate.Find("cell_" + l.ToString("00"));
			if (transform != null)
			{
				rouletteCell[l] = transform.gameObject;
				transform = rouletteCell[l].transform.Find("number_Label");
				rouletteLabel[l] = transform.gameObject.GetComponent<UILabel>();
			}
		}
		transform = base.transform.Find("window/roulette/chara_1/UI_chara_00_002_00");
		if (transform != null)
		{
			chara[0] = transform;
		}
		transform = base.transform.Find("window/roulette/chara_1/UI_chara_00_002_00");
		if (transform != null)
		{
			chara[1] = transform;
		}
		transform = panels_[1].window.transform.Find("jewel/Label");
		if (transform != null)
		{
			jewelLabel_ = transform.GetComponent<UILabel>();
		}
		transform = panels_[1].window.transform.Find("jewel/campaign");
		if (transform != null && !GlobalData.Instance.getGameData().isJewelCampaign)
		{
			transform.gameObject.SetActive(false);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		updateJewelLabel();
	}

	public override void OnCreate()
	{
	}

	public void setup(int stageCoin, int breakCount)
	{
		startData_ = GlobalData.Instance.getBonusStartData();
		changeExpression(0);
		int level = Bridge.PlayerData.getLevel();
		levelBonusCoin_ = (breakCount + 1) * level / 2;
		stageCoin_ = stageCoin;
		totalCoin_ = stageCoin + levelBonusCoin_;
		bufferGetCoin = stageCoin_;
		bufferBonusCoin = levelBonusCoin_;
		updateRouletteCels(true);
		string message = MessageResource.Instance.getMessage(4514);
		message = MessageResource.Instance.castCtrlCode(message, 1, totalCoin_.ToString());
		message = MessageResource.Instance.castCtrlCode(message, 2, (totalCoin_ * maxMaginification).ToString());
		setExpressionMsg(currentPanel_, message, stageCoin_.ToString(), levelBonusCoin_.ToString(), "?", "?");
	}

	public IEnumerator show()
	{
		Sound.Instance.playSe(Sound.eSe.SE_239_tassei);
		yield return StartCoroutine(base.open());
	}

	private void RotateStart()
	{
		StartCoroutine(RouletteRoll(totalCoin_));
	}

	private IEnumerator getCoinSend()
	{
		Input.enable = false;
		int sendCoin2 = totalCoin_;
		if (currentPanel_ == 2)
		{
			sendCoin2 -= freeRouletteResult;
		}
		Input.enable = true;
		yield return null;
	}

	private IEnumerator RouletteRoll(int _coin)
	{
		if (!bRoulette)
		{
			updateRouletteCels(currentPanel_ == 0);
			bRoulette = true;
			Input.enable = false;
			multipleNum = getRandomMulValue();
			Debug.Log("multipleNum:" + multipleNum);
			int index = getMulIndex(multipleNum);
			if (currentPanel_ == 1)
			{
				bufferGetCoin = totalCoin_;
			}
			totalCoin_ *= multipleNum;
			yield return StartCoroutine(sendBonusCoin(false));
			Input.enable = false;
			stopConfettiEff();
			if (currentPanel_ == 1)
			{
				string msg = MessageResource.Instance.getMessage(4514);
				msg = MessageResource.Instance.castCtrlCode(msg, 1, string.Empty + bufferGetCoin);
				msg = MessageResource.Instance.castCtrlCode(msg, 2, string.Empty + bufferGetCoin * maxMaginification);
				bufferBonusCoin = 0;
				Vector3 pos = panels_[currentPanel_].labels[1].transform.localPosition;
				setExpressionMsg(currentPanel_, msg, bufferGetCoin.ToString(), bufferBonusCoin.ToString(), "?", "?");
				pos.y = panels_[currentPanel_].labels[4].transform.localPosition.y;
				panels_[currentPanel_].labels[1].transform.localPosition = pos;
				panels_[currentPanel_].labels[2].gameObject.SetActive(false);
				levelBonus_.gameObject.SetActive(false);
				plusLabel_.gameObject.SetActive(false);
			}
			int coinNum = Bridge.PlayerData.getCoin();
			cancelButton_.gameObject.SetActive(false);
			panels_[currentPanel_].button.setEnable(false);
			if (currentPanel_ == 1)
			{
				receiveButton_.setEnable(false);
			}
			rotate.transform.localEulerAngles = Vector3.zero;
			int prevIndex = currentRewardIndex(true);
			float rot = 2880f + 51.42857f * (float)index - 15f + (float)UnityEngine.Random.Range(0, 31);
			iTween.RotateAdd(rotate.gameObject, iTween.Hash("z", 0f - rot, "easetype", iTween.EaseType.easeOutCubic, "time", 7, "islocal", true));
			while (rotate.GetComponent<iTween>() != null)
			{
				rouletteSe(ref prevIndex);
				yield return null;
			}
			startConfettiEff();
			if (currentPanel_ == 0)
			{
				freeRouletteResult = totalCoin_;
			}
			Input.enable = true;
			StartCoroutine(playResultBgm());
			if (coinNum + totalCoin_ > Constant.CoinMax)
			{
				DialogLimitOver limitOverDialog = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
				yield return StartCoroutine(limitOverDialog.show(Constant.eMoney.Coin));
			}
			changePanelExpression();
			Input.enable = true;
			bRoulette = false;
		}
	}

	private void rouletteSe(ref int prevIndex)
	{
		int num = currentRewardIndex(true);
		if (prevIndex != num)
		{
			prevIndex = num;
			Sound.Instance.playSe(Sound.eSe.SE_506_turn_roulette);
		}
	}

	private void userDataSetting()
	{
		GameData gameData = GlobalData.Instance.getGameData();
		gameData.allStageScoreSum = Result.allStageScoreSum;
		gameData.allStarSum = Result.allStarSum;
		gameData.allPlayCount = Result.allPlayCount;
		gameData.allClearCount = Result.allClearCount;
		gameData.bonusJewel = Result.bonusJewel;
		gameData.buyJewel = Result.buyJewel;
		gameData.level = Result.level;
		gameData.exp = Result.exp;
		gameData.coin = Result.coin;
		gameData.heart = Result.heart;
		gameData.treasureboxNum = Result.treasureboxNum;
		gameData.progressStageNo = Result.progressStageNo;
		gameData.progressStageOpen = Result.progressStageOpen;
		gameData.isAreaCampaign = Result.isAreaCampaign;
		gameData.areaSalePercent = Result.areaSalePercent;
		gameData.saleArea = Result.saleArea;
		gameData.saleStageItemArea = Result.saleStageItemArea;
		gameData.stageItemAreaSalePercent = Result.stageItemAreaSalePercent;
		gameData.isStageItemAreaCampaign = Result.isStageItemAreaCampaign;
	}

	private void changePanelExpression()
	{
		changeExpression(currentPanel_ + 1);
		if (currentPanel_ == 1)
		{
			cancelButton_.gameObject.SetActive(false);
			string message = MessageResource.Instance.getMessage(4514);
			message = MessageResource.Instance.castCtrlCode(message, 1, totalCoin_.ToString());
			message = MessageResource.Instance.castCtrlCode(message, 2, (totalCoin_ * (maxMaginification + 1)).ToString());
			setExpressionMsg(currentPanel_, message, bufferGetCoin.ToString(), bufferBonusCoin.ToString(), multipleNum.ToString(), totalCoin_.ToString());
			UILabel component = panels_[1].button.transform.Find("Label").GetComponent<UILabel>();
			string message2 = MessageResource.Instance.getMessage(4523);
			message2 = MessageResource.Instance.castCtrlCode(message2, 1, rubbyNum.ToString());
			component.text = message2;
		}
		else if (currentPanel_ == 2)
		{
			cancelButton_.gameObject.SetActive(false);
			setExpressionMsg(currentPanel_, null, bufferGetCoin.ToString(), bufferBonusCoin.ToString(), multipleNum.ToString(), totalCoin_.ToString());
		}
	}

	private int currentRewardIndex(bool bForSE)
	{
		float num = 51.42857f;
		float num2 = (bForSE ? (num * 0.6f) : (num * 0.2f));
		float num3;
		for (num3 = rotate.localEulerAngles.z; num3 > num2; num3 -= 360f)
		{
		}
		for (; num3 < -360f + num2; num3 += 360f)
		{
		}
		num3 -= num2;
		return Mathf.Clamp((int)(num3 / (0f - num)), 0, 6);
	}

	private IEnumerator playResultBgm()
	{
		Sound.Instance.playBgm(Sound.eBgm.BGM_500_roulette_normal_win, false);
		while (Sound.Instance.isPlayingBgm())
		{
			yield return null;
		}
	}

	private IEnumerator sendBonusCoin(bool cancel)
	{
		Network.DailyMission mission = GlobalData.Instance.getDailyMissionData();
		DialogLimitOver limitOverDialog = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
		WWW www = null;
		if (currentPanel_ == 0 || (currentPanel_ == 1 && cancel))
		{
			if (currentPanel_ == 0 && cancel && Bridge.PlayerData.getCoin() + totalCoin_ > Constant.CoinMax)
			{
				yield return StartCoroutine(limitOverDialog.show(Constant.eMoney.Coin));
				while (limitOverDialog.isOpen())
				{
					yield return 0;
				}
			}
			NetworkMng.Instance.setup(Hash.B1(mission.dateKey, totalCoin_, 0));
			yield return StartCoroutine(NetworkMng.Instance.download(API.B1, false, true));
			www = NetworkMng.Instance.getWWW();
			resultData_ = JsonMapper.ToObject<BonuseStageResult>(www.text);
			rubbyNum = resultData_.roulettePrice;
			Tapjoy.TrackEvent("Money", "Income Coin", "Bonus Stage", null, totalCoin_);
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Bonus Stage", totalCoin_);
		}
		else if (currentPanel_ == 1 && !cancel)
		{
			NetworkMng.Instance.setup(Hash.B2(mission.dateKey, totalCoin_ - freeRouletteResult));
			yield return StartCoroutine(NetworkMng.Instance.download(API.B2, false, true));
			www = NetworkMng.Instance.getWWW();
			Tapjoy.TrackEvent("Money", "Income Coin", "Bonus Stage Roulett", null, totalCoin_ - freeRouletteResult);
			Tapjoy.TrackEvent("Money", "Expense Jewel", "Bonus Stage Roulett", null, 10L);
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Bonus Stage Roulett", totalCoin_ - freeRouletteResult);
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Jewel", "Bonus Stage Roulett", 10L);
		}
		Result = JsonMapper.ToObject<StageResult>(www.text);
		userDataSetting();
	}

	private IEnumerator OnButton(GameObject trig)
	{
		if (trig.name == "PlusButton")
		{
			StartCoroutine(pressPlusButton(trig));
			yield break;
		}
		switch (trig.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			if (currentPanel_ == 0)
			{
				if (currentPanel_ != 2)
				{
					DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
					string msg2 = MessageResource.Instance.getMessage(4515);
					msg2 = MessageResource.Instance.castCtrlCode(msg2, 1, totalCoin_.ToString());
					dialog.setup(msg2, null, null, true);
					dialog.sysLabelEnable(false);
					yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
					while (dialog.isOpen())
					{
						yield return 0;
					}
					dialog.sysLabelEnable(true);
					if (dialog.result_ == DialogCommon.eResult.Cancel)
					{
						break;
					}
				}
				yield return StartCoroutine(sendBonusCoin(true));
			}
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "ConfirmButton":
			Constant.SoundUtil.PlayDecideSE();
			if (currentPanel_ == 0)
			{
				RotateStart();
			}
			else if (currentPanel_ == 1)
			{
				DialogCommon dialog2 = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
				string msg4 = MessageResource.Instance.getMessage(500027);
				msg4 = MessageResource.Instance.castCtrlCode(msg4, 1, rubbyNum.ToString());
				dialog2.setup(msg4, SecondRouletteCheck, null, true);
				dialog2.sysLabelEnable(false);
				yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog2));
				while (dialog2.isOpen())
				{
					yield return 0;
				}
				dialog2.sysLabelEnable(true);
			}
			break;
		case "ConfirmButton_00":
			Constant.SoundUtil.PlayDecideSE();
			if (currentPanel_ == 1)
			{
				DialogSendBonusCoin dialog3 = dialogManager_.getDialog(DialogManager.eDialog.SendBonusCoin) as DialogSendBonusCoin;
				yield return StartCoroutine(dialog3.show(totalCoin_));
				while (dialog3.isOpen())
				{
					yield return 0;
				}
			}
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "ConfirmButton_01":
			Constant.SoundUtil.PlayDecideSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	private IEnumerator SecondRouletteCheck()
	{
		if (Bridge.PlayerData.getJewel() < rubbyNum)
		{
			createCB();
			yield return StartCoroutine(base.show(eType.Jewel));
		}
		else
		{
			RotateStart();
		}
	}

	private void updateRouletteCels(bool isFree)
	{
		maxMaginification = 0;
		rouletteList_ = ((!isFree) ? resultData_.rouletteList : startData_.rouletteList);
		for (int i = 0; i < rouletteList_.Length; i++)
		{
			pattern[i] = rouletteList_[i].magnification;
		}
		for (int j = 0; j < 7; j++)
		{
			if (maxMaginification < pattern[j])
			{
				maxMaginification = pattern[j];
			}
			rouletteLabel[j].text = "×" + pattern[j];
		}
	}

	private int getRandomMulValue()
	{
		System.Random random = new System.Random();
		int num = random.Next(99);
		int num2 = 0;
		BonuseRouletteData[] array = rouletteList_;
		foreach (BonuseRouletteData bonuseRouletteData in array)
		{
			if (num2 <= num && num < num2 + bonuseRouletteData.ratio)
			{
				return bonuseRouletteData.magnification;
			}
			num2 += bonuseRouletteData.ratio;
		}
		return -1;
	}

	private int getMulIndex(int mul)
	{
		for (int i = 0; i < rouletteList_.Length; i++)
		{
			if (rouletteList_[i].magnification == mul)
			{
				return i;
			}
		}
		return 0;
	}

	public void changeExpression(int type)
	{
		currentPanel_ = type;
		for (int i = 0; i < 3; i++)
		{
			panels_[i].window.SetActive(i == currentPanel_);
		}
	}

	public void setExpressionMsg(int type, string message, string coin, string levelcoin, string ratio, string totalCoin_)
	{
		if (panels_[type].labels[0] != null && message != null)
		{
			panels_[type].labels[0].text = message;
		}
		if (panels_[type].labels[1] != null && coin != null)
		{
			panels_[type].labels[1].text = coin;
		}
		if (panels_[type].labels[2] != null && levelcoin != null)
		{
			panels_[type].labels[2].text = levelcoin;
		}
		if (panels_[type].labels[3] != null && ratio != null)
		{
			panels_[type].labels[3].text = ratio;
		}
		if (panels_[type].labels[4] != null && totalCoin_ != null)
		{
			panels_[type].labels[4].text = totalCoin_;
		}
	}

	private IEnumerator pressPlusButton(GameObject trigger)
	{
		Constant.SoundUtil.PlayButtonSE();
		DialogJewelShop dialog = dialogManager_.getDialog(DialogManager.eDialog.JewelShop) as DialogJewelShop;
		yield return dialogManager_.StartCoroutine(dialog.show());
	}

	public void updateJewelLabel()
	{
		if (panels_[1].window.activeSelf && jewelLabel_ != null)
		{
			int jewel = Bridge.PlayerData.getJewel();
			if (jewelLabel_.text != jewel.ToString())
			{
				jewelLabel_.text = jewel.ToString();
			}
		}
	}
}
