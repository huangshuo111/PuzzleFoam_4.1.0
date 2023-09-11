using System.Collections;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using TnkAd;
using Toast.Analytics;
using UnityEngine;

public class DialogAvatarLevelup : DialogShortageBase
{
	public enum eBtn
	{
		Close = 0,
		BuyCoin = 1,
		BuyJewel = 2,
		Max = 3
	}

	public enum eLabel
	{
		coin = 0,
		jewel = 1,
		message = 2,
		before_00 = 3,
		before_lv = 4,
		after_00 = 5,
		after_lv = 6,
		Max = 7
	}

	private UILabel[] labels_;

	private int jewelPrice = -1;

	private int coinPrice = -1;

	private Network.Avatar avatar_;

	private Network.Avatar[] avatarList;

	private string avatarName = string.Empty;

	private UISprite rankStar;

	private UILabel jewelBtnLabel_;

	private UILabel coinBtnLabel_;

	private UIButton[] buttons_ = new UIButton[3];

	private UISprite chara_00;

	private UISprite chara_01;

	private UISprite chara_02;

	private UISprite chara_03;

	private MainMenu mainMenu_;

	private GameObject dataTbl;

	private GameObject lv_anim;

	private bool blowcheck;

	private bool bInitialized;

	public override void OnCreate()
	{
		mainMenu_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		createCB();
	}

	public void ObjInit()
	{
		Transform transform = null;
		Transform transform2 = base.transform.Find("window");
		if (labels_ == null)
		{
			labels_ = new UILabel[7];
			transform = transform2.Find("labels");
			labels_[2] = transform.Find("label_message").GetComponent<UILabel>();
			labels_[3] = transform.Find("label_before_00").GetComponent<UILabel>();
			labels_[4] = transform.Find("label_before_lv").GetComponent<UILabel>();
			labels_[5] = transform.Find("label_after_00").GetComponent<UILabel>();
			labels_[6] = transform.Find("label_after_lv").GetComponent<UILabel>();
		}
		if (buttons_[0] == null)
		{
			transform = transform2.Find("CloseButton");
			buttons_[0] = transform.GetComponent<UIButton>();
		}
		if (buttons_[1] == null)
		{
			transform = transform2.Find("CoinButton");
			buttons_[1] = transform.GetComponent<UIButton>();
			labels_[0] = buttons_[1].transform.Find("label_coin").GetComponent<UILabel>();
		}
		if (buttons_[2] == null)
		{
			transform = transform2.Find("JewelButton");
			buttons_[2] = transform.GetComponent<UIButton>();
			labels_[1] = buttons_[2].transform.Find("label_jewel").GetComponent<UILabel>();
		}
		if (chara_00 == null)
		{
			transform = transform2.Find("Avatar/chara_00");
			chara_00 = transform.GetComponent<UISprite>();
		}
		if (chara_01 == null)
		{
			transform = transform2.Find("Avatar/chara_01");
			chara_01 = transform.GetComponent<UISprite>();
		}
		if (chara_02 == null)
		{
			chara_02 = transform2.Find("Avatar/chara_02").GetComponent<UISprite>();
		}
		if (chara_03 == null)
		{
			chara_03 = transform2.Find("Avatar/chara_03").GetComponent<UISprite>();
		}
		if (rankStar == null)
		{
			rankStar = transform2.Find("rank/star_00").GetComponent<UISprite>();
		}
		bInitialized = true;
	}

	public void setup(int index)
	{
		blowcheck = ResourceLoader.Instance.isUseLowResource();
		if (!bInitialized)
		{
			ObjInit();
		}
		Transform transform = base.transform.Find("window");
		avatarList = GlobalData.Instance.getGameData().avatarList;
		Network.Avatar[] array = avatarList;
		foreach (Network.Avatar avatar in array)
		{
			if (avatar.index == index)
			{
				avatar_ = avatar;
				break;
			}
		}
		if (dataTbl == null)
		{
			dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		}
		MessageResource instance = MessageResource.Instance;
		if (avatar_.index >= 23000)
		{
			avatarName = instance.getMessage(8600 + (avatar_.index - 23000));
			rankStar.spriteName = "rank_star_large_03";
		}
		else if (avatar_.index >= 22000)
		{
			avatarName = instance.getMessage(8500 + (avatar_.index - 22000));
			rankStar.spriteName = "rank_star_large_01";
		}
		else if (avatar_.index >= 21000)
		{
			avatarName = instance.getMessage(8400 + (avatar_.index - 21000));
			rankStar.spriteName = "rank_star_large_02";
		}
		else
		{
			avatarName = instance.getMessage(8300 + (avatar_.index - 20000));
			rankStar.spriteName = "rank_star_large_00";
		}
		float chara_img_diminish_rate = GlobalData.Instance.chara_img_diminish_rate;
		string text = ((avatar_.throwCharacter <= 0) ? string.Empty : ("_" + (avatar_.throwCharacter - 1).ToString("00")));
		string text2 = ((avatar_.supportCharacter <= 0) ? string.Empty : ("_" + (avatar_.supportCharacter - 1).ToString("00")));
		if (avatar_.throwCharacter - 1 > 18)
		{
			chara_00.atlas = chara_03.atlas;
		}
		else
		{
			chara_00.atlas = chara_02.atlas;
		}
		if (avatar_.supportCharacter - 1 > 18)
		{
			chara_01.atlas = chara_03.atlas;
		}
		else
		{
			chara_01.atlas = chara_02.atlas;
		}
		chara_00.spriteName = "avatar_00" + text + "_00";
		chara_00.MakePixelPerfect();
		chara_00.transform.localScale = new Vector3(chara_00.transform.localScale.x / chara_img_diminish_rate, chara_00.transform.localScale.y / chara_img_diminish_rate, 1f);
		chara_01.spriteName = "avatar_01" + text2 + "_00";
		chara_01.MakePixelPerfect();
		chara_01.transform.localScale = new Vector3(chara_01.transform.localScale.x / chara_img_diminish_rate, chara_01.transform.localScale.y / chara_img_diminish_rate, 1f);
		chara_00.gameObject.SetActive(true);
		chara_01.gameObject.SetActive(true);
		SetRankStarPosition();
		string message = instance.getMessage(8809);
		message = instance.castCtrlCode(message, 1, avatarName);
		int[] sKILL_SCORE_LIST = Constant.SKILL_SCORE_LIST;
		int[] sKILL_SCORE_LIST2 = Constant.SKILL_SCORE_LIST2;
		bool flag = ((avatar_.level == avatar_.limitLevel) ? true : false);
		string empty = string.Empty;
		string text3 = string.Empty;
		if (avatar_.baseSkill_3 == -1)
		{
			empty = instance.getMessage(8815);
			empty = instance.castCtrlCode(empty, 1, instance.getMessage(8840 + avatar_.baseSkill_1));
			empty = instance.castCtrlCode(empty, 2, instance.getMessage(8840 + avatar_.baseSkill_2));
			empty = instance.castCtrlCode(empty, 3, sKILL_SCORE_LIST[avatar_.level - 1].ToString());
		}
		else
		{
			empty = instance.getMessage(8838);
			empty = instance.castCtrlCode(empty, 1, instance.getMessage(8840 + avatar_.baseSkill_1));
			empty = instance.castCtrlCode(empty, 2, instance.getMessage(8840 + avatar_.baseSkill_2));
			empty = instance.castCtrlCode(empty, 3, instance.getMessage(8840 + avatar_.baseSkill_3));
			empty = instance.castCtrlCode(empty, 4, sKILL_SCORE_LIST2[avatar_.level - 1].ToString());
		}
		string empty2 = string.Empty;
		string text4 = string.Empty;
		if (avatar_.baseSkill_3 == -1)
		{
			empty2 = instance.getMessage(8815);
			empty2 = instance.castCtrlCode(empty2, 1, instance.getMessage(8840 + avatar_.baseSkill_1));
			empty2 = instance.castCtrlCode(empty2, 2, instance.getMessage(8840 + avatar_.baseSkill_2));
			empty2 = instance.castCtrlCode(empty2, 3, sKILL_SCORE_LIST[avatar_.level].ToString());
		}
		else
		{
			empty2 = instance.getMessage(8838);
			empty2 = instance.castCtrlCode(empty2, 1, instance.getMessage(8840 + avatar_.baseSkill_1));
			empty2 = instance.castCtrlCode(empty2, 2, instance.getMessage(8840 + avatar_.baseSkill_2));
			empty2 = instance.castCtrlCode(empty2, 3, instance.getMessage(8840 + avatar_.baseSkill_3));
			empty2 = instance.castCtrlCode(empty2, 4, sKILL_SCORE_LIST2[avatar_.level].ToString());
		}
		if (avatar_.specialSkill >= 30)
		{
			Constant.Avatar.eSpecialSkill[] array2 = new Constant.Avatar.eSpecialSkill[2];
			for (int j = 0; j < 2; j++)
			{
				array2[j] = Constant.Avatar.SpecialSkills[avatar_.specialSkill - 30, j];
			}
			float[] array3 = new float[2];
			float[] array4 = new float[2];
			int level = ((avatar_.level <= 0) ? 1 : avatar_.level);
			for (int k = 0; k < 2; k++)
			{
				array3[k] = dataTbl.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo((int)array2[k], level);
				if (array2[k] == Constant.Avatar.eSpecialSkill.GuideStretch)
				{
					array3[k] -= 4f;
				}
			}
			int level2 = ((!flag) ? (avatar_.level + 1) : avatar_.level);
			for (int l = 0; l < 2; l++)
			{
				array4[l] = dataTbl.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo((int)array2[l], level2);
				if (array2[l] == Constant.Avatar.eSpecialSkill.GuideStretch)
				{
					array4[l] -= 4f;
				}
			}
			for (int m = 0; m < 2; m++)
			{
				text3 = instance.getMessage(7100 + avatar_.specialSkill);
				text3 = instance.castCtrlCode(text3, 1, array3[0].ToString());
				text3 = instance.castCtrlCode(text3, 2, array3[1].ToString());
				text4 = instance.getMessage(7100 + avatar_.specialSkill);
				text4 = instance.castCtrlCode(text4, 1, array4[0].ToString());
				text4 = instance.castCtrlCode(text4, 2, array4[1].ToString());
			}
		}
		else
		{
			float num = 0f;
			float num2 = 0f;
			num = ((avatar_.level <= 0) ? dataTbl.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo(avatar_.specialSkill, 1) : dataTbl.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo(avatar_.specialSkill, avatar_.level));
			num2 = (flag ? dataTbl.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo(avatar_.specialSkill, avatar_.level) : dataTbl.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo(avatar_.specialSkill, avatar_.level + 1));
			if (avatar_.specialSkill == 6)
			{
				num -= 4f;
				num2 -= 4f;
			}
			if (avatar_.specialSkill > 0)
			{
				text3 = instance.getMessage(7100 + avatar_.specialSkill);
				text3 = instance.castCtrlCode(text3, 1, num.ToString());
				text4 = instance.getMessage(7100 + avatar_.specialSkill);
				text4 = instance.castCtrlCode(text4, 1, num2.ToString());
			}
		}
		empty += text3;
		empty2 += text4;
		string message2 = instance.getMessage(8802);
		string message3 = instance.getMessage(8802);
		message2 = instance.castCtrlCode(message2, 1, avatar_.level.ToString());
		message3 = instance.castCtrlCode(message3, 1, ((!flag) ? (avatar_.level + 1) : avatar_.level).ToString());
		jewelPrice = avatar_.jewelLevelup;
		coinPrice = avatar_.coinLevelup;
		labels_[1].text = jewelPrice.ToString();
		labels_[0].text = coinPrice.ToString();
		labels_[2].text = message;
		labels_[3].text = empty;
		labels_[4].text = message2;
		labels_[5].text = empty2;
		labels_[6].text = message3;
		UpdatePlusButtonUi();
	}

	public void SetRankStarPosition()
	{
		labels_[2].text = avatarName;
		float num = labels_[2].relativeSize.x * labels_[2].transform.localScale.x;
		rankStar.transform.parent.localPosition = new Vector3(labels_[2].transform.localPosition.x - num / 2f - rankStar.transform.localScale.x * rankStar.transform.parent.localScale.x / 2f, rankStar.transform.parent.localPosition.y, rankStar.transform.parent.localPosition.z);
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
		{
			Constant.SoundUtil.PlayCancelSE();
			DialogAvatarProfile ap = dialogManager_.getDialog(DialogManager.eDialog.AvatarProfile) as DialogAvatarProfile;
			if (!ap.isOpen())
			{
				yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarProfile(this, avatar_.index));
			}
			else
			{
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			}
			break;
		}
		case "JewelButton":
			Constant.SoundUtil.PlayDecideSE();
			result3 = Bridge.PlayerData.getJewel() - jewelPrice;
			if (result3 < 0)
			{
				yield return StartCoroutine(show(eType.Jewel));
				break;
			}
			yield return dialogManager_.StartCoroutine(CloseDialogs());
			yield return StartCoroutine(AvatarLevelupRoutine(Constant.eMoney.Jewel));
			break;
		case "CoinButton":
			Constant.SoundUtil.PlayDecideSE();
			result3 = Bridge.PlayerData.getCoin() - coinPrice;
			if (result3 < 0)
			{
				yield return StartCoroutine(show(eType.Coin));
				break;
			}
			yield return dialogManager_.StartCoroutine(CloseDialogs());
			yield return StartCoroutine(AvatarLevelupRoutine(Constant.eMoney.Coin));
			break;
		}
	}

	public IEnumerator CloseDialogs()
	{
		DialogAvatarCollection ac = dialogManager_.getDialog(DialogManager.eDialog.AvatarCollection) as DialogAvatarCollection;
		DialogAvatarProfile ap = dialogManager_.getDialog(DialogManager.eDialog.AvatarProfile) as DialogAvatarProfile;
		DialogSetup setup = dialogManager_.getDialog(DialogManager.eDialog.Setup) as DialogSetup;
		DialogEventSetup eSetup = dialogManager_.getDialog(DialogManager.eDialog.EventSetup) as DialogEventSetup;
		DialogCollaborationSetup cSetup = dialogManager_.getDialog(DialogManager.eDialog.CollaborationSetup) as DialogCollaborationSetup;
		if (ac != null && ac.isOpen())
		{
			ac.DestroyContents();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(ac));
		}
		if (ap != null && ap.isOpen())
		{
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(ap));
		}
		if (setup != null && setup.isOpen())
		{
			GlobalData.Instance.setItems(setup.getItems());
			DialogPlayScore ps3 = dialogManager_.getDialog(DialogManager.eDialog.PlayScore) as DialogPlayScore;
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(ps3));
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(setup));
		}
		if (eSetup != null && eSetup.isOpen())
		{
			GlobalData.Instance.setItems(eSetup.getItems());
			DialogPlayScore ps2 = dialogManager_.getDialog(DialogManager.eDialog.PlayScore) as DialogPlayScore;
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(ps2));
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(eSetup));
		}
		if (cSetup != null && cSetup.isOpen())
		{
			GlobalData.Instance.setItems(cSetup.getItems());
			DialogPlayScore ps = dialogManager_.getDialog(DialogManager.eDialog.PlayScore) as DialogPlayScore;
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(ps));
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(cSetup));
		}
	}

	private IEnumerator AvatarLevelupRoutine(Constant.eMoney priceType)
	{
		Input.enable = false;
		yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
		int price = 0;
		price = ((priceType != Constant.eMoney.Jewel) ? avatar_.coinLevelup : avatar_.jewelLevelup);
		Hashtable h = Hash.AvatarLevelup((int)priceType, price, avatar_.index);
		NetworkMng.Instance.setup(h);
		yield return StartCoroutine(NetworkMng.Instance.download(API.AvatarLevelup, false, false));
		while (NetworkMng.Instance.isDownloading())
		{
			yield return null;
		}
		if (NetworkMng.Instance.getResultCode() != 0)
		{
			Input.enable = true;
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		AvatarLevelup resultData_ = JsonMapper.ToObject<AvatarLevelup>(www.text);
		GlobalData.Instance.getGameData().setCommonData(resultData_, true);
		GlobalData.Instance.getGameData().inviteBasicReward = resultData_.inviteBasicReward;
		GlobalData.Instance.getGameData().avatarList = resultData_.avatarList;
		GlobalData.Instance.getGameData().continueNum = resultData_.continueNum;
		GlobalData.Instance.getGameData().heartRecoverTime = resultData_.heartRecoverTime;
		GlobalData.Instance.getGameData().isAllAvatarLevelMax = resultData_.isAllAvatarLevelMax;
		GlobalData.Instance.getGameData().gachaTicket = resultData_.gachaTicket;
		www.Dispose();
		www = null;
		if (GlobalData.Instance.getGameData().avatarList != null)
		{
			int[] rank_count = new int[4];
			Network.Avatar[] array = GlobalData.Instance.getGameData().avatarList;
			foreach (Network.Avatar av in array)
			{
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
		yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		Input.enable = true;
		mainMenu_.update();
		Tapjoy.TrackEvent("Charactor", "LvUP", priceType.ToString(), avatar_.index.ToString(), 0L);
		Tapjoy.TrackEvent("CharactorLevel", "Level", avatar_.index.ToString(), (avatar_.level + 1).ToString(), 0L);
		switch (priceType)
		{
		case Constant.eMoney.Jewel:
			Tapjoy.TrackEvent("CharactorLevel", "Resource", "Jewel" + avatar_.jewelLevelup, avatar_.index.ToString(), "Use Jewel", avatar_.jewelLevelup, null, 0L, null, 0L);
			Tapjoy.TrackEvent("Money", "Expense Jewel", "Charactor LvUP", null, avatar_.jewelLevelup);
			break;
		case Constant.eMoney.Coin:
			Tapjoy.TrackEvent("CharactorLevel", "Resource", "Coin" + avatar_.coinLevelup, avatar_.index.ToString(), "Use Coin", avatar_.coinLevelup, null, 0L, null, 0L);
			Tapjoy.TrackEvent("Money", "Expense Coin", "Charactor LvUP", null, avatar_.coinLevelup);
			break;
		}
		GlobalGoogleAnalytics.Instance.LogEvent("Charactor Level", avatar_.index.ToString(), (avatar_.level + 1).ToString(), 1L);
		switch (priceType)
		{
		case Constant.eMoney.Jewel:
			GlobalGoogleAnalytics.Instance.LogEvent("Charactor LvUP", priceType.ToString(), avatar_.index.ToString(), avatar_.jewelLevelup);
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Jewel", "Charactor LvUP", avatar_.jewelLevelup);
			break;
		case Constant.eMoney.Coin:
			GlobalGoogleAnalytics.Instance.LogEvent("Charactor LvUP", priceType.ToString(), avatar_.index.ToString(), avatar_.coinLevelup);
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Coin", "Charactor LvUP", avatar_.coinLevelup);
			break;
		default:
			GlobalGoogleAnalytics.Instance.LogEvent("Charactor LvUP", priceType.ToString(), avatar_.index.ToString(), 1L);
			break;
		}
		switch (priceType)
		{
		case Constant.eMoney.Jewel:
			GameAnalytics.traceMoneyConsumption("BUY_LEVELUP_JEWEL", "0", avatar_.jewelLevelup, Bridge.PlayerData.getCurrentStage());
			break;
		case Constant.eMoney.Coin:
			GameAnalytics.traceMoneyConsumption("BUY_LEVELUP_COIN", "0", avatar_.coinLevelup, Bridge.PlayerData.getCurrentStage());
			break;
		}
		switch (priceType)
		{
		case Constant.eMoney.Jewel:
			Plugin.Instance.buyCompleted("BUY_LEVELUP_JEWEL");
			break;
		case Constant.eMoney.Coin:
			Plugin.Instance.buyCompleted("BUY_LEVELUP_COIN");
			break;
		}
		yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarLevelupAnim(this, avatar_.index, false, false));
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

	public void UpdatePlusButtonUi()
	{
		Transform transform = base.transform.Find("window/01_coin");
		Transform transform2 = base.transform.Find("window/02_jewel");
		transform.Find("Label").GetComponent<UILabel>().text = Bridge.PlayerData.getCoin().ToString("N0");
		transform2.Find("Label").GetComponent<UILabel>().text = Bridge.PlayerData.getJewel().ToString();
		transform.Find("campaign").gameObject.SetActive(GlobalData.Instance.getGameData().isCoinCampaign);
		transform.Find("campaign_02").gameObject.SetActive(GlobalData.Instance.getGameData().isCoinupCampaign);
		transform2.Find("campaign").gameObject.SetActive(GlobalData.Instance.getGameData().isJewelCampaign);
	}
}
