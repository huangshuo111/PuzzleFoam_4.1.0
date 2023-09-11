using System.Collections;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using UnityEngine;

public class DialogRoulette : DialogBase
{
	private enum eLabel
	{
		result_type_label = 0,
		result_number_label = 1,
		result_lot_label = 2,
		by_label = 3,
		Label00 = 4,
		Label01 = 5,
		Label = 6,
		Max = 7
	}

	private enum eObj
	{
		cell_00 = 0,
		cell_01 = 1,
		cell_02 = 2,
		cell_03 = 3,
		cell_04 = 4,
		cell_05 = 5,
		cell_06 = 6,
		rotate = 7,
		result = 8,
		icon_coin = 9,
		icon_heart = 10,
		icon_jewel = 11,
		UI_chara_00_002_00 = 12,
		UI_chara_00_002_01 = 13,
		ConfirmButton = 14,
		Close_Button = 15,
		eff_down = 16,
		base_00 = 17,
		base_01 = 18,
		base_02 = 19,
		base_03 = 20,
		center = 21,
		Max = 22
	}

	private enum eRank
	{
		First = 0,
		Second = 1,
		Third = 2,
		Other = 3,
		Max = 4
	}

	private UILabel[] labels_ = new UILabel[7];

	private Transform[] objs_ = new Transform[22];

	private int rank_;

	private RouletteInfo.Reward[] rewards_;

	private float bgmTime;

	private void Awake()
	{
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			switch (transform.name)
			{
			case "Label00":
				labels_[4] = transform.GetComponent<UILabel>();
				break;
			case "Label01":
				labels_[5] = transform.GetComponent<UILabel>();
				break;
			case "result_number_label":
				labels_[1] = transform.GetComponent<UILabel>();
				break;
			case "result_lot_label":
				labels_[2] = transform.GetComponent<UILabel>();
				break;
			case "result_type_label":
				labels_[0] = transform.GetComponent<UILabel>();
				break;
			case "by_label":
				labels_[3] = transform.GetComponent<UILabel>();
				break;
			case "Label":
				labels_[6] = transform.GetComponent<UILabel>();
				break;
			case "cell_00":
				objs_[0] = transform;
				break;
			case "cell_01":
				objs_[1] = transform;
				break;
			case "cell_02":
				objs_[2] = transform;
				break;
			case "cell_03":
				objs_[3] = transform;
				break;
			case "cell_04":
				objs_[4] = transform;
				break;
			case "cell_05":
				objs_[5] = transform;
				break;
			case "cell_06":
				objs_[6] = transform;
				break;
			case "rotate":
				objs_[7] = transform;
				break;
			case "roulette":
				objs_[8] = transform;
				break;
			case "icon_coin":
				if (transform.parent.name == "result")
				{
					objs_[9] = transform;
				}
				break;
			case "icon_heart":
				if (transform.parent.name == "result")
				{
					objs_[10] = transform;
				}
				break;
			case "icon_jewel":
				if (transform.parent.name == "result")
				{
					objs_[11] = transform;
				}
				break;
			case "UI_chara_00_002_00":
				objs_[12] = transform;
				break;
			case "UI_chara_00_002_01":
				objs_[13] = transform;
				break;
			case "ConfirmButton":
				objs_[14] = transform;
				break;
			case "Close_Button":
				objs_[15] = transform;
				break;
			case "eff_down":
				objs_[16] = transform;
				break;
			case "base_00":
				objs_[17] = transform;
				break;
			case "base_01":
				objs_[18] = transform;
				break;
			case "base_02":
				objs_[19] = transform;
				break;
			case "base_03":
				objs_[20] = transform;
				break;
			case "center":
				objs_[21] = transform;
				break;
			}
		}
	}

	public IEnumerator show(int rank)
	{
		Input.enable = false;
		int index = Mathf.Clamp(rank - 1, 0, 3);
		rank_ = rank;
		for (int k = 0; k < 4; k++)
		{
			objs_[17 + k].gameObject.SetActive(k == index);
		}
		UISprite sp = objs_[21].GetComponent<UISprite>();
		switch ((eRank)index)
		{
		case eRank.First:
			sp.spriteName = "UI_roulette_center";
			break;
		case eRank.Second:
			sp.spriteName = "UI_roulette_center_silver";
			break;
		case eRank.Third:
			sp.spriteName = "UI_roulette_center_bronze";
			break;
		default:
			sp.spriteName = "UI_roulette_center_wood";
			break;
		}
		NetworkMng.Instance.setup(Hash.Roulette(rank_));
		yield return dialogManager_.StartCoroutine(NetworkMng.Instance.download(API.Roulette, true));
		SaveNetworkData netData = SaveData.Instance.getGameData().getNetworkData();
		if (NetworkMng.Instance.getResultCode() == eResultCode.InvalidRankingReward)
		{
			netData.save();
			Input.enable = true;
			yield break;
		}
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			netData.resetRankingDate();
			netData.resetRankingUniqueID();
			Input.enable = true;
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		RouletteDataList data = JsonMapper.ToObject<RouletteDataList>(www.text);
		rewards_ = new RouletteInfo.Reward[data.rouletteList.Length];
		for (int j = 0; j < rewards_.Length; j++)
		{
			rewards_[j] = new RouletteInfo.Reward();
			rewards_[j].RewardType = data.rouletteList[j].rewardType;
			rewards_[j].Num = data.rouletteList[j].num;
			rewards_[j].RewardRank = data.rouletteList[j].rewardRank;
		}
		labels_[5].gameObject.SetActive(false);
		labels_[1].gameObject.SetActive(false);
		labels_[2].gameObject.SetActive(false);
		labels_[3].gameObject.SetActive(false);
		objs_[9].gameObject.SetActive(false);
		objs_[10].gameObject.SetActive(false);
		objs_[11].gameObject.SetActive(false);
		for (int i = 0; i < rewards_.Length; i++)
		{
			RouletteInfo.Reward reward = rewards_[i];
			Transform cell = objs_[i];
			cell.Find("bg_red").gameObject.SetActive(reward.RewardRank == 1);
			cell.Find("bg_blue").gameObject.SetActive(reward.RewardRank == 3);
			cell.Find("icon_coin").gameObject.SetActive(reward.RewardType == 1);
			cell.Find("icon_heart").gameObject.SetActive(reward.RewardType == 3);
			cell.Find("icon_jewel").gameObject.SetActive(reward.RewardType == 2);
			cell.Find("icon_miss").gameObject.SetActive(reward.RewardType == -1);
			if (reward.Num < 0)
			{
				cell.Find("number_Label").GetComponent<UILabel>().text = string.Empty;
			}
			else
			{
				cell.Find("number_Label").GetComponent<UILabel>().text = reward.Num.ToString("N0");
			}
		}
		MessageResource msgRes = MessageResource.Instance;
		labels_[4].text = msgRes.castCtrlCode(msgRes.getMessage(2580), 1, rank.ToString());
		labels_[0].text = msgRes.getMessage(2581);
		labels_[6].text = msgRes.getMessage(2587);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		Input.enable = true;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Close_Button":
		{
			Constant.SoundUtil.PlayCancelSE();
			DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
			dialog.setup(2582, null, null, true);
			dialog.sysLabelEnable(false);
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
			while (dialog.isOpen())
			{
				yield return 0;
			}
			dialog.sysLabelEnable(true);
			if (dialog.result_ != DialogCommon.eResult.Cancel)
			{
				SaveNetworkData netData = SaveData.Instance.getGameData().getNetworkData();
				NetworkMng.Instance.setup(Hash.R1(rank_, netData.getRankingUniqueID(), 1));
				yield return StartCoroutine(NetworkMng.Instance.download(API.R1, true));
				netData.save();
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			}
			break;
		}
		case "ConfirmButton":
			Constant.SoundUtil.PlayDecideSE();
			if (labels_[6].text != MessageResource.Instance.getMessage(2401))
			{
				yield return StartCoroutine(sendRankingRoulette(rank_));
			}
			else
			{
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			}
			break;
		}
	}

	private IEnumerator sendRankingRoulette(int p0)
	{
		Input.enable = false;
		objs_[15].gameObject.SetActive(false);
		UIButtonColor labelColor = null;
		UIButtonColor[] uiButtonColors = GetComponentsInChildren<UIButtonColor>();
		UIButtonColor[] array = uiButtonColors;
		foreach (UIButtonColor uiButtonColor in array)
		{
			if (uiButtonColor.tweenTarget != null && uiButtonColor.tweenTarget.name == "Label")
			{
				labelColor = uiButtonColor;
				break;
			}
		}
		labels_[6].color = Color.gray;
		labelColor.defaultColor = Color.gray;
		objs_[14].GetComponent<UIButton>().setEnable(false);
		int heartNum = Bridge.PlayerData.getHeart();
		int coinNum = Bridge.PlayerData.getCoin();
		int jewelNum = Bridge.PlayerData.getJewel();
		int rewardIndex = Random.Range(0, 6);
		SaveNetworkData netData = SaveData.Instance.getGameData().getNetworkData();
		NetworkMng.Instance.setup(Hash.R1(p0, netData.getRankingUniqueID(), 0));
		yield return StartCoroutine(NetworkMng.Instance.download(API.R1, true));
		if (NetworkMng.Instance.getResultCode() == eResultCode.InvalidRankingReward)
		{
			netData.save();
			Input.enable = true;
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			yield break;
		}
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			netData.resetRankingDate();
			netData.resetRankingUniqueID();
			Input.enable = true;
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			yield break;
		}
		netData.save();
		WWW www2 = NetworkMng.Instance.getWWW();
		CommonData commonData = JsonMapper.ToObject<CommonData>(www2.text);
		GlobalData.Instance.getGameData().setCommonData(commonData, false);
		RouletteRewardData rewardData = JsonMapper.ToObject<RouletteRewardData>(www2.text);
		for (int i = 0; i < rewards_.Length; i++)
		{
			if (rewards_[i].RewardType == rewardData.rewardType && rewards_[i].Num == rewardData.rewardNum)
			{
				rewardIndex = i;
				break;
			}
		}
		www2.Dispose();
		www2 = null;
		int prevIndex = currentRewardIndex(true);
		float rot = 2880f + 51.42857f * (float)rewardIndex - 15f + (float)Random.Range(0, 31);
		iTween.RotateAdd(objs_[7].gameObject, iTween.Hash("z", 0f - rot, "easetype", iTween.EaseType.easeOutCubic, "time", 7, "islocal", true));
		while (objs_[7].GetComponent<iTween>() != null)
		{
			rouletteSe(ref prevIndex);
			yield return null;
		}
		RouletteInfo.Reward reward = rewards_[rewardIndex];
		MessageResource msgRes = MessageResource.Instance;
		labels_[5].gameObject.SetActive(true);
		labels_[5].text = msgRes.getMessage(2586);
		if (reward.RewardRank != 3)
		{
			objs_[12].GetComponent<UISpriteAnimationEx>().clipIndex = 1;
			objs_[13].GetComponent<UISpriteAnimationEx>().clipIndex = 1;
			startConfettiEff();
			objs_[9].gameObject.SetActive(reward.RewardType == 1);
			objs_[10].gameObject.SetActive(reward.RewardType == 3);
			objs_[11].gameObject.SetActive(reward.RewardType == 2);
			labels_[0].gameObject.SetActive(false);
			labels_[3].gameObject.SetActive(true);
			labels_[2].gameObject.SetActive(true);
			labels_[1].gameObject.SetActive(true);
			labels_[2].text = msgRes.getMessage((reward.RewardRank != 1) ? 2584 : 2583);
			labels_[1].text = reward.Num.ToString("N0");
		}
		else
		{
			objs_[12].GetComponent<UISpriteAnimationEx>().clipIndex = 2;
			objs_[13].GetComponent<UISpriteAnimationEx>().clipIndex = 2;
			objs_[16].gameObject.SetActive(true);
			labels_[0].text = msgRes.getMessage(2585);
		}
		MainMenu menu = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		menu.update();
		labels_[6].text = msgRes.getMessage(2401);
		labels_[6].color = Color.white;
		labelColor.defaultColor = Color.white;
		objs_[14].GetComponent<UIButton>().setEnable(true);
		Input.enable = true;
		StartCoroutine(playResultBgm(reward.RewardRank));
		if (reward.RewardType == 1)
		{
			Tapjoy.TrackEvent("Money", "Income Coin", "Ranking Roulett", null, reward.Num);
		}
		else if (reward.RewardType == 2)
		{
			Tapjoy.TrackEvent("Money", "Income Jewel", "Ranking Roulett", null, reward.Num);
		}
		if (reward.RewardType == 1)
		{
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Ranking Roulett", reward.Num);
		}
		else if (reward.RewardType == 2)
		{
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Jewel", "Ranking Roulett", reward.Num);
		}
		switch ((Constant.eMoney)reward.RewardType)
		{
		case Constant.eMoney.Coin:
			if (coinNum + reward.Num > Constant.CoinMax)
			{
				DialogLimitOver limitOverDialog = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
				yield return StartCoroutine(limitOverDialog.show(Constant.eMoney.Coin));
			}
			break;
		case Constant.eMoney.Heart:
			if (heartNum + reward.Num > Constant.HeartMax)
			{
				DialogLimitOver limitOverDialog2 = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
				yield return StartCoroutine(limitOverDialog2.show(Constant.eMoney.Heart));
			}
			break;
		case Constant.eMoney.Jewel:
			if (jewelNum + reward.Num > Constant.JewelMax)
			{
				DialogLimitOver limitOverDialog3 = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
				yield return StartCoroutine(limitOverDialog3.show(Constant.eMoney.Jewel));
			}
			break;
		}
	}

	private int currentRewardIndex(bool bForSE)
	{
		float num = 51.42857f;
		float num2 = (bForSE ? (num * 0.6f) : (num * 0.2f));
		float num3;
		for (num3 = objs_[7].localEulerAngles.z; num3 > num2; num3 -= 360f)
		{
		}
		for (; num3 < -360f + num2; num3 += 360f)
		{
		}
		num3 -= num2;
		return Mathf.Clamp((int)(num3 / (0f - num)), 0, 6);
	}

	private IEnumerator playResultBgm(int rewardRank)
	{
		bgmTime = Sound.Instance.timeBgm();
		switch (rewardRank)
		{
		case 1:
			Sound.Instance.playBgm(Sound.eBgm.BGM_501_roulette_special_win, false);
			break;
		case 2:
			Sound.Instance.playBgm(Sound.eBgm.BGM_500_roulette_normal_win, false);
			break;
		case 3:
			Sound.Instance.playBgm(Sound.eBgm.BGM_502_roulette_lose, false);
			break;
		}
		while (Sound.Instance.isPlayingBgm())
		{
			yield return null;
		}
		Sound.Instance.playBgm(Sound.eBgm.BGM_010_Map, true);
		Sound.Instance.setBGMTime(bgmTime);
	}

	public override void OnClose()
	{
		if (Sound.Instance.currentBgm != Sound.eBgm.BGM_010_Map)
		{
			Sound.Instance.playBgm(Sound.eBgm.BGM_010_Map, true);
			Sound.Instance.setBGMTime(bgmTime);
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
}
