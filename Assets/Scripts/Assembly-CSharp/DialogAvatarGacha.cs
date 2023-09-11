using System.Collections;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using TnkAd;
using Toast.Analytics;
using UnityEngine;

public class DialogAvatarGacha : DialogShortageBase
{
	private enum eGachaMoneyType
	{
		Free = 0,
		Coin = 1,
		Jewel = 2,
		Ticket = 4
	}

	private enum eGachaCategoryType
	{
		Old = 0,
		Normal = 1,
		Premium = 2
	}

	public enum eBtn
	{
		Close = 0,
		BuyCoin = 1,
		BuyJewel = 2,
		BuyFree = 3,
		BuyTicket = 4,
		ArrowR = 5,
		ArrowL = 6,
		Max = 7
	}

	public enum eText
	{
		Retry = 0,
		Confirm = 1
	}

	public enum eGacha
	{
		Premium = 0,
		Normal = 1,
		Max = 2
	}

	private int jewelPrice = -1;

	private int coinPrice = -1;

	private int premiumTicketPrice = -1;

	private int normalTicketPrice = -1;

	private int resultAvatarID;

	private Network.Avatar resultAvatar;

	private bool isNew;

	private bool isLimitOpen;

	private bool isDraw;

	public static eGacha gachaNo;

	private UILabel label_;

	private UILabel chanceLabel_;

	private UILabel jewelBtnLabel_;

	private UILabel coinBtnLabel_;

	private UILabel freeBtnLabel_;

	private UILabel ticketBtnLabel_;

	private UILabel gachaTicketCountLabel_;

	private UIButton[] buttons_ = new UIButton[7];

	private UILabel nameLabel_;

	private UILabel rankLabel_;

	private MainMenu mainMenu_;

	private GameObject gacha;

	private GameObject gacha_clone;

	private GameObject gacha_00;

	private GameObject gacha_01;

	private GameObject[] btnParents;

	private SaveOptionData optionData;

	public Transform gacha_map_button;

	private bool blowcheck;

	private bool bInitialized;

	private GameObject banner_;

	private GameObject banner_parent;

	public override void OnCreate()
	{
		mainMenu_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		createCB();
	}

	private void ObjInit()
	{
		Transform transform = base.transform;
		Transform transform2 = null;
		buttons_[0] = transform.Find("window/CloseButton").GetComponent<UIButton>();
		buttons_[1] = transform.Find("window/CoinButton").GetComponent<UIButton>();
		coinBtnLabel_ = buttons_[1].transform.Find("label_coin").GetComponent<UILabel>();
		buttons_[2] = transform.Find("window/JewelButton").GetComponent<UIButton>();
		jewelBtnLabel_ = buttons_[2].transform.Find("label_jewel").GetComponent<UILabel>();
		buttons_[3] = transform.Find("window/FreeButton").GetComponent<UIButton>();
		freeBtnLabel_ = buttons_[3].transform.Find("label").GetComponent<UILabel>();
		buttons_[4] = transform.Find("window/TicketButton").GetComponent<UIButton>();
		ticketBtnLabel_ = buttons_[4].transform.Find("label_ticket").GetComponent<UILabel>();
		buttons_[5] = transform.Find("window/gacha/Arrow/arrow_r").GetComponent<UIButton>();
		buttons_[6] = transform.Find("window/gacha/Arrow/arrow_l").GetComponent<UIButton>();
		gacha = transform.Find("window/gacha").gameObject;
		gacha_00 = transform.Find("window/gacha/chara_00").gameObject;
		gacha_01 = transform.Find("window/gacha/chara_01").gameObject;
		label_ = transform.Find("window/label_info").GetComponent<UILabel>();
		chanceLabel_ = transform.Find("window/label_Chance").GetComponent<UILabel>();
		gachaTicketCountLabel_ = transform.Find("window/label_ticketcount").GetComponent<UILabel>();
		nameLabel_ = transform.Find("window/label_name").GetComponent<UILabel>();
		rankLabel_ = transform.Find("window/label_rank").GetComponent<UILabel>();
		bInitialized = true;
	}

	public void resetGachaNo()
	{
		gachaNo = eGacha.Premium;
	}

	public void setup(bool isFree)
	{
		blowcheck = ResourceLoader.Instance.isUseLowResource();
		optionData = SaveData.Instance.getSystemData().getOptionData();
		if (!bInitialized)
		{
			ObjInit();
		}
		if (!gacha.activeSelf)
		{
			gacha.SetActive(true);
		}
		jewelPrice = GlobalData.Instance.getGameData().gachaJewelPrice;
		coinPrice = GlobalData.Instance.getGameData().gachaCoinPrice;
		premiumTicketPrice = GlobalData.Instance.getGameData().gachaPremiumTicketPrice;
		normalTicketPrice = GlobalData.Instance.getGameData().gachaNormalTicketPrice;
		Debug.Log("GlobalData.Instance.getGameData().isGachaUpCampaign = " + GlobalData.Instance.getGameData().isGachaUpCampaign);
		string[] message = TalkMessage.Instance.getMessage(TalkMessage.eType.GachaInfo);
		chanceLabel_.text = message[0];
		setActiveCampaignMessage();
		jewelBtnLabel_.text = jewelPrice.ToString();
		coinBtnLabel_.text = coinPrice.ToString();
		if (gachaNo == eGacha.Premium)
		{
			if (premiumTicketPrice < 1)
			{
				ticketBtnLabel_.text = "--";
			}
			else
			{
				ticketBtnLabel_.text = premiumTicketPrice.ToString();
			}
			label_.text = MessageResource.Instance.getMessage(8806);
			nameLabel_.text = MessageResource.Instance.getMessage(8860);
			rankLabel_.text = MessageResource.Instance.getMessage(8861);
			gacha_00.SetActive(false);
			gacha_01.SetActive(true);
		}
		else
		{
			if (normalTicketPrice < 1)
			{
				ticketBtnLabel_.text = "--";
			}
			else
			{
				ticketBtnLabel_.text = normalTicketPrice.ToString();
			}
			label_.text = MessageResource.Instance.getMessage(8839);
			nameLabel_.text = MessageResource.Instance.getMessage(8863);
			rankLabel_.text = MessageResource.Instance.getMessage(8864);
			gacha_00.SetActive(true);
			gacha_01.SetActive(false);
		}
		setButton(isFree);
		isDraw = false;
		dialogManager_.StartCoroutine(showBanner(true));
		if (partManager_.currentPart == PartManager.ePart.Map)
		{
			(partManager_.execPart as Part_Map).checkAvatarCampaign();
		}
		else if (partManager_.currentPart == PartManager.ePart.EventMap)
		{
			(partManager_.execPart as Part_EventMap).checkAvatarCampaign();
		}
		else if (partManager_.currentPart == PartManager.ePart.CollaborationMap)
		{
			(partManager_.execPart as Part_CollaborationMap).checkAvatarCampaign();
		}
		if (optionData.getFlag(SaveOptionData.eFlag.BGM))
		{
			dialogManager_.StartCoroutine(Sound.Instance.reVolumeBgmCoroutine(Sound.Instance.getDefaultBgmVolume() / 4f));
		}
		UpdatePlusButtonUi();
	}

	public void setButton(bool isFree)
	{
		bool flag = false;
		bool flag2 = false;
		flag = GlobalData.Instance.getGameData().isAllAvatarLevelMax;
		flag2 = GlobalData.Instance.getGameData().isAllAvatarLevelMax2;
		buttons_[2].setEnable(true);
		buttons_[1].setEnable(true);
		buttons_[3].setEnable(true);
		buttons_[4].setEnable(true);
		jewelBtnLabel_.color = Color.white;
		coinBtnLabel_.color = Color.white;
		freeBtnLabel_.color = Color.white;
		ticketBtnLabel_.color = Color.white;
		buttons_[2].gameObject.SetActive(!isFree);
		buttons_[1].gameObject.SetActive(!isFree);
		buttons_[3].gameObject.SetActive(isFree);
		buttons_[4].gameObject.SetActive(!isFree);
		int num = 0;
		if (isFree)
		{
			bool isFirstGacha = GlobalData.Instance.getGameData().isFirstGacha;
			buttons_[3].transform.Find("icon_ticket").gameObject.SetActive(!isFirstGacha);
			buttons_[3].transform.Find("label_ticket").gameObject.SetActive(!isFirstGacha);
			buttons_[3].transform.Find("label").gameObject.SetActive(isFirstGacha);
		}
		num = GlobalData.Instance.getGameData().gachaTicket;
		string message = MessageResource.Instance.getMessage(2511);
		message = MessageResource.Instance.castCtrlCode(message, 1, num.ToString());
		gachaTicketCountLabel_.text = message;
		if (gachaNo == eGacha.Premium)
		{
			buttons_[1].gameObject.SetActive(false);
			if (GlobalData.Instance.getGameData().isGachaSaleCampaign)
			{
				buttons_[2].transform.Find("campaign").gameObject.SetActive(true);
			}
			else
			{
				buttons_[2].transform.Find("campaign").gameObject.SetActive(false);
			}
			if (premiumTicketPrice < 1 || num < premiumTicketPrice)
			{
				buttons_[4].setEnable(false);
				ticketBtnLabel_.color = Color.grey;
			}
			buttons_[6].gameObject.SetActive(false);
			buttons_[5].gameObject.SetActive(true);
			if (flag2)
			{
				buttons_[2].setEnable(false);
				buttons_[1].setEnable(false);
				buttons_[3].setEnable(false);
				buttons_[4].setEnable(false);
				jewelBtnLabel_.color = Color.grey;
				coinBtnLabel_.color = Color.grey;
				freeBtnLabel_.color = Color.grey;
				ticketBtnLabel_.color = Color.grey;
				label_.text = MessageResource.Instance.getMessage(8812);
			}
		}
		else
		{
			buttons_[2].gameObject.SetActive(false);
			if (normalTicketPrice < 1 || num < normalTicketPrice)
			{
				buttons_[4].setEnable(false);
				ticketBtnLabel_.color = Color.grey;
			}
			buttons_[6].gameObject.SetActive(true);
			buttons_[5].gameObject.SetActive(false);
			if (flag)
			{
				buttons_[2].setEnable(false);
				buttons_[1].setEnable(false);
				buttons_[3].setEnable(false);
				buttons_[4].setEnable(false);
				jewelBtnLabel_.color = Color.grey;
				coinBtnLabel_.color = Color.grey;
				freeBtnLabel_.color = Color.grey;
				ticketBtnLabel_.color = Color.grey;
				label_.text = MessageResource.Instance.getMessage(8812);
			}
		}
	}

	public void setButtonActive(eBtn btn, bool bActive)
	{
		if (!(buttons_[(int)btn] == null))
		{
			buttons_[(int)btn].gameObject.SetActive(bActive);
		}
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		if (trigger.transform.parent.parent.name == "buttons")
		{
			if (gacha_clone != null)
			{
				Object.Destroy(gacha_clone);
			}
			if (trigger.name == "DetailButton")
			{
				Constant.SoundUtil.PlayDecideSE();
				if (optionData.getFlag(SaveOptionData.eFlag.BGM))
				{
					Sound.Instance.setBgmMasterVolume(Sound.Instance.getDefaultBgmVolume());
				}
				Sound.Instance.playBgm(Sound.eBgm.BGM_010_Map, true);
				GlobalData.Instance.isGachaAfterOpeningDialog = true;
				yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarProfile(this, resultAvatarID));
			}
			else if (trigger.name == "RetryButton")
			{
				Constant.SoundUtil.PlayDecideSE();
				if (optionData.getFlag(SaveOptionData.eFlag.BGM))
				{
					Sound.Instance.setBgmMasterVolume(Sound.Instance.getDefaultBgmVolume() / 4f);
					dialogManager_.StartCoroutine(Sound.Instance.reVolumeBgmCoroutine(Sound.Instance.getDefaultBgmVolume() / 4f));
				}
				Sound.Instance.playBgm(Sound.eBgm.BGM_010_Map, true);
				Hashtable h = Hash.GachaTop();
				NetworkMng.Instance.setup(h);
				yield return StartCoroutine(NetworkMng.Instance.download(API.GachaTop, true, false));
				while (NetworkMng.Instance.isDownloading())
				{
					yield return null;
				}
				if (NetworkMng.Instance.getResultCode() == eResultCode.Success)
				{
					WWW wwww = NetworkMng.Instance.getWWW();
					GachaTop resultData = JsonMapper.ToObject<GachaTop>(wwww.text);
					GlobalData.Instance.getGameData().setGachaTopData(resultData);
					setup(GlobalData.Instance.getGameData().isFirstGacha);
				}
			}
			else if (trigger.name == "ConfirmButton")
			{
				Constant.SoundUtil.PlayDecideSE();
				GlobalData.Instance.isGachaAfterOpeningDialog = true;
				yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarLevelupAnim(this, resultAvatarID, isLimitOpen, true));
			}
			else if (trigger.name == "CloseButton")
			{
				Constant.SoundUtil.PlayCancelSE();
				if (optionData.getFlag(SaveOptionData.eFlag.BGM))
				{
					Sound.Instance.setBgmMasterVolume(Sound.Instance.getDefaultBgmVolume());
					dialogManager_.StartCoroutine(Sound.Instance.reVolumeBgmCoroutine(Sound.Instance.getDefaultBgmVolume()));
				}
				Sound.Instance.playBgm(Sound.eBgm.BGM_010_Map, true);
				resetGachaNo();
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			}
		}
		else
		{
			if (isDraw)
			{
				yield break;
			}
			int result3 = 0;
			if (trigger.name == "PlusButton")
			{
				Constant.SoundUtil.PlayButtonSE();
				yield return StartCoroutine(plusButton(trigger));
				yield break;
			}
			switch (trigger.name)
			{
			case "CloseButton":
				Constant.SoundUtil.PlayCancelSE();
				if (optionData.getFlag(SaveOptionData.eFlag.BGM))
				{
					dialogManager_.StartCoroutine(Sound.Instance.reVolumeBgmCoroutine(Sound.Instance.getDefaultBgmVolume()));
				}
				resetGachaNo();
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
				break;
			case "FreeButton":
			{
				Constant.SoundUtil.PlayDecideSE();
				UpdatePlusButtonUi();
				yield return dialogManager_.StartCoroutine(CloseDialogs());
				bool isFree2 = true;
				eGachaMoneyType moneyType = ((!GlobalData.Instance.getGameData().isFirstGacha) ? eGachaMoneyType.Ticket : eGachaMoneyType.Free);
				yield return StartCoroutine(DrawGachaRoutine(eGachaCategoryType.Old, moneyType));
				break;
			}
			case "JewelButton":
				Constant.SoundUtil.PlayDecideSE();
				result3 = Bridge.PlayerData.getJewel() - jewelPrice;
				if (result3 < 0)
				{
					yield return StartCoroutine(show(eType.Jewel));
					while (dialogManager_.getDialog(DialogManager.eDialog.JewelShop).isOpen())
					{
						yield return null;
					}
					UpdatePlusButtonUi();
				}
				else
				{
					yield return dialogManager_.StartCoroutine(CloseDialogs());
					yield return StartCoroutine(DrawGachaRoutine(eGachaCategoryType.Premium, eGachaMoneyType.Jewel));
				}
				break;
			case "CoinButton":
				Constant.SoundUtil.PlayDecideSE();
				result3 = Bridge.PlayerData.getCoin() - coinPrice;
				if (result3 < 0)
				{
					yield return StartCoroutine(show(eType.Coin));
					while (dialogManager_.getDialog(DialogManager.eDialog.CoinShop).isOpen())
					{
						yield return null;
					}
					UpdatePlusButtonUi();
				}
				else
				{
					yield return dialogManager_.StartCoroutine(CloseDialogs());
					yield return StartCoroutine(DrawGachaRoutine(eGachaCategoryType.Normal, eGachaMoneyType.Coin));
				}
				break;
			case "TicketButton":
			{
				Constant.SoundUtil.PlayDecideSE();
				UpdatePlusButtonUi();
				eGachaCategoryType categoryType = eGachaCategoryType.Premium;
				if (gachaNo == eGacha.Normal)
				{
					categoryType = eGachaCategoryType.Normal;
				}
				yield return dialogManager_.StartCoroutine(CloseDialogs());
				yield return StartCoroutine(DrawGachaRoutine(categoryType, eGachaMoneyType.Ticket));
				break;
			}
			case "ListButton":
			{
				Constant.SoundUtil.PlayDecideSE();
				Hashtable h2 = Hash.GachaList();
				NetworkMng.Instance.setup(h2);
				yield return StartCoroutine(NetworkMng.Instance.download(API.GachaList, true, true));
				while (NetworkMng.Instance.isDownloading())
				{
					yield return null;
				}
				if (NetworkMng.Instance.getResultCode() == eResultCode.Success)
				{
					WWW www = NetworkMng.Instance.getWWW();
					GachaDrawList resultData_ = JsonMapper.ToObject<GachaDrawList>(www.text);
					GlobalData.Instance.getGameData().gachaList = resultData_.gachaList;
					GlobalData.Instance.getGameData().gachaList2 = resultData_.gachaList2;
					GlobalData.Instance.getGameData().totalRatioList = resultData_.totalRatioList;
					GlobalData.Instance.getGameData().totalRatioList2 = resultData_.totalRatioList2;
					DialogGachaContents dialog = dialogManager_.getDialog(DialogManager.eDialog.GachaContents) as DialogGachaContents;
					dialog.setup((int)gachaNo);
					yield return StartCoroutine(dialogManager_.openDialog(dialog));
					dialog.reposition();
				}
				break;
			}
			case "CharaBoxButton":
				Constant.SoundUtil.PlayDecideSE();
				if (optionData.getFlag(SaveOptionData.eFlag.BGM))
				{
					dialogManager_.StartCoroutine(Sound.Instance.reVolumeBgmCoroutine(Sound.Instance.getDefaultBgmVolume()));
				}
				resetGachaNo();
				yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarCollection(this));
				break;
			case "HelpButton":
				Constant.SoundUtil.PlayDecideSE();
				if (SaveData.Instance.getSystemData().getOptionData().isKorean())
				{
					Application.OpenURL("http://14.63.170.51:10080/view/a/pb/character");
				}
				else
				{
					Application.OpenURL("http://14.63.170.51:10080/view/a/pb/character");
				}
				break;
			case "arrow_r":
			{
				Constant.SoundUtil.PlayButtonSE();
				gachaNo = eGacha.Normal;
				bool isFree2 = GlobalData.Instance.getGameData().isFirstGacha;
				setup(isFree2);
				break;
			}
			case "arrow_l":
			{
				Constant.SoundUtil.PlayButtonSE();
				gachaNo = eGacha.Premium;
				bool isFree2 = GlobalData.Instance.getGameData().isFirstGacha;
				setup(isFree2);
				break;
			}
			case "banner_parent":
			{
				Constant.SoundUtil.PlayDecideSE();
				Input.enable = false;
				string url = GlobalData.Instance.getGameData().gacha_detail_url;
				if (gachaNo == eGacha.Premium)
				{
					url = GlobalData.Instance.getGameData().gacha_detail_url2;
				}
				if (url == string.Empty || url == null)
				{
					Input.enable = true;
					break;
				}
				DialogBannerInformation bannerDialog = dialogManager_.getDialog(DialogManager.eDialog.BannerInformation) as DialogBannerInformation;
				if (bannerDialog == null)
				{
					Input.enable = true;
					break;
				}
				yield return StartCoroutine(bannerDialog.loadTexture(url));
				yield return StartCoroutine(bannerDialog.show(url));
				Input.enable = true;
				break;
			}
			}
		}
	}

	public IEnumerator CloseDialogs()
	{
		DialogAvatarCollection ac = dialogManager_.getDialog(DialogManager.eDialog.AvatarCollection) as DialogAvatarCollection;
		DialogAvatarLevelup al = dialogManager_.getDialog(DialogManager.eDialog.AvatarLevelup) as DialogAvatarLevelup;
		DialogSetup setup = dialogManager_.getDialog(DialogManager.eDialog.Setup) as DialogSetup;
		DialogEventSetup eSetup = dialogManager_.getDialog(DialogManager.eDialog.EventSetup) as DialogEventSetup;
		DialogCollaborationSetup cSetup = dialogManager_.getDialog(DialogManager.eDialog.CollaborationSetup) as DialogCollaborationSetup;
		if (ac != null && ac.isOpen())
		{
			ac.DestroyContents();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(ac));
		}
		if (al != null && al.isOpen())
		{
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(al));
		}
		if (setup != null && setup.isOpen())
		{
			DialogPlayScore ps3 = dialogManager_.getDialog(DialogManager.eDialog.PlayScore) as DialogPlayScore;
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(ps3));
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(setup));
		}
		if (eSetup != null && eSetup.isOpen())
		{
			DialogPlayScore ps2 = dialogManager_.getDialog(DialogManager.eDialog.PlayScore) as DialogPlayScore;
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(ps2));
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(eSetup));
		}
		if (cSetup != null && cSetup.isOpen())
		{
			DialogPlayScore ps = dialogManager_.getDialog(DialogManager.eDialog.PlayScore) as DialogPlayScore;
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(ps));
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(cSetup));
		}
	}

	private IEnumerator DrawGachaRoutine(eGachaCategoryType categoryType, eGachaMoneyType moneyType)
	{
		if ((gachaNo == eGacha.Normal && GlobalData.Instance.getGameData().isAllAvatarLevelMax) || (gachaNo == eGacha.Premium && GlobalData.Instance.getGameData().isAllAvatarLevelMax2))
		{
			yield break;
		}
		DialogAvatarCollection ac = dialogManager_.getDialog(DialogManager.eDialog.AvatarCollection) as DialogAvatarCollection;
		if (ac.isOpen())
		{
			ac.DestroyContents();
			yield return dialogManager_.StartCoroutine(ac.close());
		}
		isDraw = true;
		Input.enable = false;
		yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
		int campaignType = 0;
		if (GlobalData.Instance.getGameData().isGachaUpCampaign)
		{
			campaignType = 1;
		}
		else if (GlobalData.Instance.getGameData().isGachaSaleCampaign)
		{
			campaignType = 2;
		}
		Debug.Log("categoryType = " + (int)categoryType);
		Debug.Log("moneyType = " + (int)moneyType);
		Debug.Log("campaignType = " + campaignType);
		Hashtable h = Hash.AvatarGacha((int)categoryType, (int)moneyType, campaignType);
		NetworkMng.Instance.setup(h);
		yield return StartCoroutine(NetworkMng.Instance.download(API.AvatarGacha, false, false));
		while (NetworkMng.Instance.isDownloading())
		{
			yield return null;
		}
		if (NetworkMng.Instance.getResultCode() != 0)
		{
			if (NetworkMng.Instance.getResultCode() == eResultCode.NotExistDrawAvatar)
			{
				if (gachaNo == eGacha.Normal)
				{
					GlobalData.Instance.getGameData().isAllAvatarLevelMax = true;
				}
				else
				{
					GlobalData.Instance.getGameData().isAllAvatarLevelMax2 = true;
				}
			}
			GlobalData.Instance.getGameData().isFirstGacha = false;
			yield return StartCoroutine(Inactive());
			Input.enable = true;
			checkAvatarCampaign();
			setActiveCampaignMessage();
			bool isFree = GlobalData.Instance.getGameData().isFirstGacha;
			yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarGacha(this, isFree));
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		AvatarGacha resultData_ = JsonMapper.ToObject<AvatarGacha>(www.text);
		GlobalData.Instance.getGameData().setCommonData(resultData_, true);
		GlobalData.Instance.getGameData().inviteBasicReward = resultData_.inviteBasicReward;
		GlobalData.Instance.getGameData().avatarList = resultData_.avatarList;
		GlobalData.Instance.getGameData().continueNum = resultData_.continueNum;
		GlobalData.Instance.getGameData().heartRecoverTime = resultData_.heartRecoverTime;
		GlobalData.Instance.getGameData().isAllAvatarLevelMax = resultData_.isAllAvatarLevelMax;
		GlobalData.Instance.getGameData().isAllAvatarLevelMax2 = resultData_.isAllAvatarLevelMax2;
		GlobalData.Instance.getGameData().gachaTicket = resultData_.gachaTicket;
		resultAvatarID = resultData_.drawResultAvatarIndex;
		isNew = resultData_.isNewAvatar;
		isLimitOpen = resultData_.isLimitOpen;
		www.Dispose();
		www = null;
		if (GlobalData.Instance.getGameData().avatarList != null)
		{
			int[] rank_count = new int[4];
			Network.Avatar[] avatarList = GlobalData.Instance.getGameData().avatarList;
			foreach (Network.Avatar av in avatarList)
			{
				if (resultAvatarID == av.index)
				{
					resultAvatar = av;
				}
				if (av.wearFlg == 1)
				{
					GlobalData.Instance.currentAvatar = av;
				}
				if (av.index >= 23000)
				{
					rank_count[3]++;
				}
				else if (av.index >= 22000)
				{
					rank_count[2]++;
				}
				else if (av.index >= 21000)
				{
					rank_count[1]++;
				}
				else
				{
					rank_count[0]++;
				}
			}
			GlobalData.Instance.avatarCount = rank_count;
		}
		switch (moneyType)
		{
		case eGachaMoneyType.Jewel:
			Tapjoy.TrackEvent("Charactor", "Gacha", "Premium Jewel", resultAvatarID.ToString(), 0L);
			Tapjoy.TrackEvent("Money", "Expense Jewel", "Charactor Gacha", null, jewelPrice);
			break;
		case eGachaMoneyType.Coin:
			Tapjoy.TrackEvent("Charactor", "Gacha", "Normal Coin", resultAvatarID.ToString(), 0L);
			Tapjoy.TrackEvent("Money", "Expense Coin", "Charactor Gacha", null, coinPrice);
			break;
		default:
			if (categoryType == eGachaCategoryType.Premium)
			{
				Tapjoy.TrackEvent("Charactor", "Gacha", "Premium " + moneyType, resultAvatarID.ToString(), 0L);
			}
			else
			{
				Tapjoy.TrackEvent("Charactor", "Gacha", "Normal " + moneyType, resultAvatarID.ToString(), 0L);
			}
			break;
		}
		switch (moneyType)
		{
		case eGachaMoneyType.Jewel:
			GlobalGoogleAnalytics.Instance.LogEvent("Charactor Gacha", "Premium Jewel", resultAvatarID.ToString(), jewelPrice);
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Jewel", "Charactor Gacha", jewelPrice);
			break;
		case eGachaMoneyType.Coin:
			GlobalGoogleAnalytics.Instance.LogEvent("Charactor Gacha", "Normal Coin", resultAvatarID.ToString(), coinPrice);
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Coin", "Charactor Gacha", coinPrice);
			break;
		default:
			if (categoryType == eGachaCategoryType.Premium)
			{
				GlobalGoogleAnalytics.Instance.LogEvent("Charactor Gacha", "Premium " + moneyType, resultAvatarID.ToString(), 1L);
			}
			else
			{
				GlobalGoogleAnalytics.Instance.LogEvent("Charactor Gacha", "Normal " + moneyType, resultAvatarID.ToString(), 1L);
			}
			break;
		}
		switch (moneyType)
		{
		case eGachaMoneyType.Jewel:
			GameAnalytics.traceMoneyConsumption("BUY_GACHA_JEWEL", "0", jewelPrice, Bridge.PlayerData.getCurrentStage());
			break;
		case eGachaMoneyType.Coin:
			GameAnalytics.traceMoneyConsumption("BUY_GACHA_COIN", "1", coinPrice, Bridge.PlayerData.getCurrentStage());
			break;
		}
		switch (moneyType)
		{
		case eGachaMoneyType.Jewel:
			Plugin.Instance.buyCompleted("BUY_GACHA_JEWEL");
			break;
		case eGachaMoneyType.Coin:
			Plugin.Instance.buyCompleted("BUY_GACHA_COIN");
			break;
		}
		Tapjoy.TrackEvent("Charactor", "LvUP", "Gacha" + moneyType, resultAvatarID.ToString(), 0L);
		Tapjoy.TrackEvent("CharactorLevel", "Level", resultAvatarID.ToString(), resultAvatar.level.ToString(), 0L);
		GlobalGoogleAnalytics.Instance.LogEvent("Charactor LvUP", "Gacha" + moneyType, resultAvatarID.ToString(), 1L);
		GlobalGoogleAnalytics.Instance.LogEvent("Charactor Level", resultAvatarID.ToString(), resultAvatar.level.ToString(), 1L);
		switch (moneyType)
		{
		case eGachaMoneyType.Jewel:
			GameAnalytics.traceMoneyConsumption("BUY_GACHA_LVUP_JEWEL", "0", jewelPrice, Bridge.PlayerData.getCurrentStage());
			break;
		case eGachaMoneyType.Coin:
			GameAnalytics.traceMoneyConsumption("BUY_GACHA_LVUP_COIN", "1", coinPrice, Bridge.PlayerData.getCurrentStage());
			break;
		}
		switch (moneyType)
		{
		case eGachaMoneyType.Jewel:
			Plugin.Instance.buyCompleted("BUY_GACHA_LVUP_JEWEL");
			break;
		case eGachaMoneyType.Coin:
			Plugin.Instance.buyCompleted("BUY_GACHA_LVUP_COIN");
			break;
		}
		yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		Sound.Instance.stopBgm();
		buttons_[6].gameObject.SetActive(false);
		buttons_[5].gameObject.SetActive(false);
		MessageResource msg = MessageResource.Instance;
		gacha_clone = Object.Instantiate(gacha.gameObject) as GameObject;
		Utility.setParent(gacha_clone, gacha.transform.parent, false);
		gacha.SetActive(false);
		UILabel nameLabel = gacha_clone.transform.Find("rarity/name_label").GetComponent<UILabel>();
		UISprite rankStar = gacha_clone.transform.Find("rarity/1/center").GetComponent<UISprite>();
		UISprite chara_0 = gacha_clone.transform.Find("avatar/right").GetComponent<UISprite>();
		UISprite chara_ = gacha_clone.transform.Find("avatar/left").GetComponent<UISprite>();
		UISprite chara_2 = gacha_clone.transform.Find("avatar/right2").GetComponent<UISprite>();
		UISprite chara_3 = gacha_clone.transform.Find("avatar/right3").GetComponent<UISprite>();
		if (resultAvatar.throwCharacter - 1 > 18)
		{
			chara_0.atlas = chara_3.atlas;
		}
		else
		{
			chara_0.atlas = chara_2.atlas;
		}
		if (resultAvatar.supportCharacter - 1 > 18)
		{
			chara_.atlas = chara_3.atlas;
		}
		else
		{
			chara_.atlas = chara_2.atlas;
		}
		string st0 = ((resultAvatar.throwCharacter <= 0) ? string.Empty : ("_" + (resultAvatar.throwCharacter - 1).ToString("00")));
		string st = ((resultAvatar.supportCharacter <= 0) ? string.Empty : ("_" + (resultAvatar.supportCharacter - 1).ToString("00")));
		float rate = GlobalData.Instance.chara_img_expand_rate;
		chara_0.spriteName = "avatar_00" + st0 + "_00";
		chara_0.MakePixelPerfect();
		chara_0.transform.localScale = new Vector3(chara_0.transform.localScale.x * rate, chara_0.transform.localScale.y * rate, 1f);
		chara_.spriteName = "avatar_01" + st + "_00";
		chara_.MakePixelPerfect();
		chara_.transform.localScale = new Vector3(chara_.transform.localScale.x * rate, chara_.transform.localScale.y * rate, 1f);
		rankStar.transform.localPosition = new Vector3(-10f, rankStar.transform.localPosition.y, rankStar.transform.localPosition.z);
		string name = string.Empty;
		if (resultAvatarID >= 23000)
		{
			name = msg.getMessage(8600 + (resultAvatarID - 23000));
			rankStar.spriteName = "rank_star_large_03";
		}
		else if (resultAvatarID >= 22000)
		{
			name = msg.getMessage(8500 + (resultAvatarID - 22000));
			rankStar.spriteName = "rank_star_large_01";
		}
		else if (resultAvatarID >= 21000)
		{
			name = msg.getMessage(8400 + (resultAvatarID - 21000));
			rankStar.spriteName = "rank_star_large_02";
		}
		else
		{
			name = msg.getMessage(8300 + (resultAvatarID - 20000));
			rankStar.spriteName = "rank_star_large_00";
		}
		nameLabel.text = name;
		float size_x = nameLabel.relativeSize.x * nameLabel.transform.localScale.x;
		rankStar.transform.parent.localPosition = new Vector3(nameLabel.transform.localPosition.x - size_x / 2f - rankStar.transform.parent.localScale.x * rankStar.transform.localScale.x / 2f, rankStar.transform.parent.localPosition.y, rankStar.transform.parent.localPosition.z);
		gacha_clone.transform.Find("fade").GetComponent<BoxCollider>().enabled = true;
		Input.enable = true;
		UISpriteAnimationEx anim2 = null;
		anim2 = ((gachaNo != 0) ? gacha_clone.transform.Find("chara_00").GetComponent<UISpriteAnimationEx>() : gacha_clone.transform.Find("chara_01").GetComponent<UISpriteAnimationEx>());
		anim2.enabled = true;
		bool bDown = false;
		UISpriteAnimationEx bAnim = gacha_clone.transform.Find("bubble_00/bubble").GetComponent<UISpriteAnimationEx>();
		float elapsedTime = 0f;
		bool bMakeSound_lever = false;
		bool bMakeSound_flash_eye = false;
		bool bMakeSound_vibrate = false;
		bool bMakeSound_through_chackn = false;
		bool bMakeSound_mouth_open = false;
		bool bMakeSound_out_bubble = false;
		bool bMakeSound_bubble_crush = false;
		bool bMakeSound_fanfale = false;
		string gachaAnim3 = string.Empty;
		if (resultAvatar.rank == 3 || resultAvatar.rank == 2)
		{
			anim2.clipIndex = 1;
			if (resultAvatar.rank == 3)
			{
				bAnim.clipIndex = 2;
			}
			else
			{
				bAnim.clipIndex = 1;
			}
			gacha_clone.transform.Find("bubble_00/light").gameObject.SetActive(true);
			gachaAnim3 = ((gachaNo != 0) ? "Gacha_01_anm" : "Gacha_01_01_anm");
			gacha_clone.GetComponent<Animation>().clip = gacha_clone.GetComponent<Animation>()[gachaAnim3].clip;
			gacha_clone.GetComponent<Animation>().Play(gachaAnim3);
			while (gacha_clone.GetComponent<Animation>().IsPlaying(gachaAnim3))
			{
				if (!bMakeSound_lever && elapsedTime >= 0.3f)
				{
					Sound.Instance.playSe(Sound.eSe.SE_560_gacha_lever);
					bMakeSound_lever = true;
				}
				else if (!bMakeSound_flash_eye && elapsedTime >= 0.95f)
				{
					Sound.Instance.playSe(Sound.eSe.SE_563_gacha_flash_eye);
					bMakeSound_flash_eye = true;
				}
				else if (!bMakeSound_vibrate && elapsedTime >= 1.25f)
				{
					Sound.Instance.playSe(Sound.eSe.SE_561_gacha_vibrate);
					bMakeSound_vibrate = true;
				}
				else if (!bMakeSound_mouth_open && elapsedTime >= 2.4f)
				{
					Sound.Instance.playSeVolumeControl(Sound.eSe.SE_564_gacha_mouth_open, false, 0.6f);
					bMakeSound_mouth_open = true;
				}
				else if (!bMakeSound_through_chackn && elapsedTime >= 2.5f)
				{
					Sound.Instance.playSe(Sound.eSe.SE_562_gacha_through_chackn);
					bMakeSound_through_chackn = true;
				}
				else if (!bMakeSound_out_bubble && elapsedTime >= 3.2f)
				{
					Sound.Instance.playSe(Sound.eSe.SE_565_gacha_bubble_out);
					bMakeSound_out_bubble = true;
				}
				else if (!bMakeSound_bubble_crush && elapsedTime >= 6.5f)
				{
					Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
					bMakeSound_bubble_crush = true;
				}
				else if (!bMakeSound_fanfale && elapsedTime >= 6.7f)
				{
					PlayFanfale(resultAvatar);
					bMakeSound_fanfale = true;
				}
				if (!bDown && Input.GetMouseButtonDown(0))
				{
					bDown = true;
				}
				if (bDown && Input.GetMouseButtonUp(0))
				{
					gacha_clone.GetComponent<Animation>()[gachaAnim3].time = gacha_clone.GetComponent<Animation>()[gachaAnim3].length;
				}
				elapsedTime += Time.deltaTime;
				yield return null;
			}
		}
		else
		{
			if (resultAvatar.rank == 1)
			{
				bAnim.clipIndex = 1;
				gacha_clone.transform.Find("bubble_00/light").gameObject.SetActive(true);
			}
			else
			{
				bAnim.clipIndex = 0;
			}
			gachaAnim3 = ((gachaNo != 0) ? "Gacha_00_anm" : "Gacha_01_00_anm");
			gacha_clone.GetComponent<Animation>().Play(gachaAnim3);
			while (gacha_clone.GetComponent<Animation>().IsPlaying(gachaAnim3))
			{
				if (!bMakeSound_lever && elapsedTime >= 0.3f)
				{
					Sound.Instance.playSe(Sound.eSe.SE_560_gacha_lever);
					bMakeSound_lever = true;
				}
				else if (!bMakeSound_vibrate && elapsedTime >= 1.25f)
				{
					Sound.Instance.playSe(Sound.eSe.SE_561_gacha_vibrate);
					bMakeSound_vibrate = true;
				}
				else if (!bMakeSound_mouth_open && elapsedTime >= 2.3f)
				{
					Sound.Instance.playSe(Sound.eSe.SE_564_gacha_mouth_open);
					bMakeSound_mouth_open = true;
				}
				else if (!bMakeSound_out_bubble && elapsedTime >= 2.45f)
				{
					Sound.Instance.playSe(Sound.eSe.SE_565_gacha_bubble_out);
					bMakeSound_out_bubble = true;
				}
				else if (!bMakeSound_bubble_crush && elapsedTime >= 5.5f)
				{
					Sound.Instance.playSe(Sound.eSe.SE_218_hakai);
					bMakeSound_bubble_crush = true;
				}
				else if (!bMakeSound_fanfale && elapsedTime >= 5.7f)
				{
					PlayFanfale(resultAvatar);
					bMakeSound_fanfale = true;
				}
				if (!bDown && Input.GetMouseButtonDown(0))
				{
					bDown = true;
				}
				if (bDown && Input.GetMouseButtonUp(0))
				{
					gacha_clone.GetComponent<Animation>()[gachaAnim3].time = gacha_clone.GetComponent<Animation>()[gachaAnim3].length;
				}
				elapsedTime += Time.deltaTime;
				yield return null;
			}
		}
		Sound.Instance.stopSe(Sound.eSe.SE_565_gacha_bubble_out);
		if (!bMakeSound_fanfale)
		{
			switch (resultAvatar.rank)
			{
			case 3:
				Sound.Instance.playSe(Sound.eSe.SE_606_gacha_fanfare_ss);
				break;
			case 2:
				Sound.Instance.playSe(Sound.eSe.SE_566_gacha_fanfare_s);
				break;
			case 1:
				Sound.Instance.playSe(Sound.eSe.SE_567_gacha_fanfare_a);
				break;
			case 0:
				Sound.Instance.playSe(Sound.eSe.SE_568_gacha_fanfare_b);
				break;
			}
		}
		gacha_clone.transform.Find("buttons/button_first").gameObject.SetActive(isNew);
		gacha_clone.transform.Find("buttons/button_second").gameObject.SetActive(!isNew);
		checkAvatarCampaign();
		mainMenu_.update();
		UpdatePlusButtonUi();
	}

	private IEnumerator plusButton(GameObject trigger)
	{
		DialogAllShop dialog = dialogManager_.getDialog(DialogManager.eDialog.AllShop) as DialogAllShop;
		switch (trigger.transform.parent.name)
		{
		case "01_coin":
			yield return StartCoroutine(dialog.show(DialogAllShop.ePanelType.Coin));
			break;
		case "02_jewel":
			yield return StartCoroutine(dialog.show(DialogAllShop.ePanelType.Jewel));
			break;
		case "04_heart":
			yield return StartCoroutine(dialog.show(DialogAllShop.ePanelType.Heart));
			break;
		}
	}

	private void PlayFanfale(Network.Avatar avatar)
	{
		switch (avatar.rank)
		{
		case 3:
			Sound.Instance.playSe(Sound.eSe.SE_606_gacha_fanfare_ss);
			break;
		case 2:
			Sound.Instance.playSe(Sound.eSe.SE_566_gacha_fanfare_s);
			break;
		case 1:
			Sound.Instance.playSe(Sound.eSe.SE_567_gacha_fanfare_a);
			break;
		case 0:
			Sound.Instance.playSe(Sound.eSe.SE_568_gacha_fanfare_b);
			break;
		}
	}

	public void UpdatePlusButtonUi()
	{
		Transform transform = base.transform.Find("window/01_coin");
		Transform transform2 = base.transform.Find("window/02_jewel");
		if (transform != null)
		{
			transform.Find("Label").GetComponent<UILabel>().text = Bridge.PlayerData.getCoin().ToString("N0");
		}
		if (transform2 != null)
		{
			transform2.Find("Label").GetComponent<UILabel>().text = Bridge.PlayerData.getJewel().ToString();
		}
		transform.Find("campaign").gameObject.SetActive(GlobalData.Instance.getGameData().isCoinCampaign);
		transform.Find("campaign_02").gameObject.SetActive(GlobalData.Instance.getGameData().isCoinupCampaign);
		transform2.Find("campaign").gameObject.SetActive(GlobalData.Instance.getGameData().isJewelCampaign);
	}

	public IEnumerator Inactive()
	{
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(API.Inactive, true, false));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			InactiveData data = JsonMapper.ToObject<InactiveData>(www.text);
			GlobalData.Instance.getGameData().setCommonData(data, true);
			GlobalData.Instance.getGameData().setEventData(data);
			GlobalData.Instance.getGameData().setDailyMissionData(data);
			GlobalData.Instance.getGameData().helpDataSize = data.helpDataSize;
			GlobalData.Instance.getGameData().helpMove = data.helpMove;
			GlobalData.Instance.getGameData().helpTime = data.helpTime;
			GlobalData.Instance.getGameData().bonusChanceLv = data.bonusChanceLv;
			GlobalData.Instance.getGameData().saleArea = data.saleArea;
			GlobalData.Instance.getGameData().areaSalePercent = data.areaSalePercent;
			GlobalData.Instance.getGameData().isAreaCampaign = data.isAreaCampaign;
			GlobalData.Instance.getGameData().gachaTicket = data.gachaTicket;
			EventMenu.updateGetTime();
			ChallengeMenu.updateGetTime();
			CollaborationMenu.updateGetTime();
			DailyMission.updateGetTime();
			GlobalData.Instance.getGameData().setParkData(null, null, data.thanksList, data.buildings, data.mapReleaseNum);
			SaveData.Instance.getParkData().UpdatePlacedData(data.buildings);
		}
	}

	private void checkAvatarCampaign()
	{
		if (gacha_map_button != null)
		{
			gacha_map_button.Find("campaign_02").gameObject.SetActive(GlobalData.Instance.getGameData().isGachaUpCampaign);
			gacha_map_button.Find("campaign").gameObject.SetActive(GlobalData.Instance.getGameData().isGachaSaleCampaign);
		}
	}

	public void setActiveCampaignMessage()
	{
		if (!(chanceLabel_ == null))
		{
			if (GlobalData.Instance.getGameData().isGachaUpCampaign && gachaNo == eGacha.Premium)
			{
				chanceLabel_.gameObject.SetActive(true);
			}
			else
			{
				chanceLabel_.gameObject.SetActive(false);
			}
		}
	}

	protected IEnumerator showBanner(bool isCampaign)
	{
		banner_parent = base.transform.Find("window/banner_parent").gameObject;
		banner_parent.SetActive(false);
		banner_ = base.transform.Find("window/banner_parent/banner_texture").gameObject;
		banner_.SetActive(false);
		if (!isCampaign)
		{
			yield break;
		}
		string url = GlobalData.Instance.getGameData().gacha_banner_url;
		if (gachaNo == eGacha.Premium)
		{
			url = GlobalData.Instance.getGameData().gacha_banner_url2;
		}
		if (!(url == string.Empty) && url != null)
		{
			banner_parent.SetActive(true);
			WWW www = new WWW(url);
			yield return www;
			Texture tex = www.texture;
			if (tex != null && tex.width != 8 && !isPlayingCloseAnime())
			{
				banner_.SetActive(true);
				banner_.GetComponent<UITexture>().mainTexture = tex;
				banner_.GetComponent<UITexture>().MakePixelPerfect();
				banner_.transform.localScale /= 2f;
				banner_parent.GetComponent<BoxCollider>().size = banner_.transform.localScale;
				banner_.transform.localPosition = new Vector3(0f, 0f, -1.1f);
			}
			www.Dispose();
		}
	}
}
