using System.Collections;
using Bridge;
using Network;
using UnityEngine;

public class DialogAvatarLevelupAnim : DialogShortageBase
{
	public enum eBtn
	{
		Profile = 0,
		Profile_center = 1,
		Levelup = 2,
		Retry = 3,
		Close = 4,
		Max = 5
	}

	public enum eLabel
	{
		limit = 0,
		skill = 1,
		level = 2,
		level_in_arrow = 3,
		Max = 4
	}

	private UILabel[] labels_;

	private UISprite chara_00;

	private UISprite chara_01;

	private UISprite chara_02;

	private UISprite chara_03;

	private Network.Avatar avatar_;

	private Network.Avatar[] avatarList;

	private string avatarName = string.Empty;

	private UISprite rankStar;

	private UIButton[] buttons_ = new UIButton[5];

	private GameObject dataTbl;

	private GameObject lvup_img_00;

	private GameObject lvup_img_01;

	private bool blowcheck;

	private SaveOptionData optionData;

	private bool bInitialized;

	private int levelButtonFirstState;

	public override void OnCreate()
	{
		createCB();
	}

	private void ObjInit()
	{
		Transform transform = base.transform;
		Transform transform2 = null;
		if (buttons_[0] == null)
		{
			transform2 = transform.Find("DetailButton");
			if (transform2 != null)
			{
				buttons_[0] = transform2.GetComponent<UIButton>();
			}
		}
		if (buttons_[1] == null)
		{
			transform2 = transform.Find("DetailButton_center");
			if (transform2 != null)
			{
				buttons_[1] = transform2.GetComponent<UIButton>();
			}
		}
		if (buttons_[2] == null)
		{
			transform2 = transform.Find("LvupButton");
			if (transform2 != null)
			{
				buttons_[2] = transform2.GetComponent<UIButton>();
			}
		}
		if (buttons_[3] == null)
		{
			transform2 = transform.Find("RetryButton");
			if (transform2 != null)
			{
				buttons_[3] = transform2.GetComponent<UIButton>();
				transform2.gameObject.SetActive(false);
			}
		}
		if (buttons_[4] == null)
		{
			transform2 = transform.Find("CloseButton");
			if (transform2 != null)
			{
				buttons_[4] = transform2.GetComponent<UIButton>();
			}
		}
		if (chara_00 == null)
		{
			transform2 = transform.Find("avatar/right");
			if (transform2 != null)
			{
				chara_00 = transform2.GetComponent<UISprite>();
			}
		}
		if (chara_01 == null)
		{
			transform2 = transform.Find("avatar/left");
			if (transform2 != null)
			{
				chara_01 = transform2.GetComponent<UISprite>();
			}
		}
		if (chara_02 == null)
		{
			transform2 = transform.Find("avatar/right2");
			if (transform2 != null)
			{
				chara_02 = transform2.GetComponent<UISprite>();
			}
		}
		if (chara_03 == null)
		{
			transform2 = transform.Find("avatar/right3");
			if (transform2 != null)
			{
				chara_03 = transform2.GetComponent<UISprite>();
			}
		}
		if (lvup_img_00 == null)
		{
			transform2 = transform.Find("Lvup_text");
			if (transform2 != null)
			{
				lvup_img_00 = transform2.gameObject;
			}
		}
		if (lvup_img_01 == null)
		{
			transform2 = transform.Find("lvup_text2");
			if (transform2 != null)
			{
				lvup_img_01 = transform2.gameObject;
			}
		}
		if (labels_ == null)
		{
			labels_ = new UILabel[4];
			transform2 = base.transform.Find("skill/skill_detail_Label");
			if (transform2 != null)
			{
				labels_[1] = transform2.GetComponent<UILabel>();
			}
			transform2 = base.transform.Find("level_limit_Label");
			if (transform2 != null)
			{
				labels_[0] = transform2.GetComponent<UILabel>();
				transform2.gameObject.SetActive(false);
			}
			transform2 = base.transform.Find("LvupButton/label_lv");
			if (transform2 != null)
			{
				labels_[2] = transform2.GetComponent<UILabel>();
			}
			transform2 = base.transform.Find("avatar/arrow/Label/Label_Level_value");
			if (transform2 != null)
			{
				labels_[3] = transform2.GetComponent<UILabel>();
			}
		}
		bInitialized = true;
	}

	public void setup(int index, bool bReleaseLimit, bool isGacha)
	{
		optionData = SaveData.Instance.getSystemData().getOptionData();
		blowcheck = ResourceLoader.Instance.isUseLowResource();
		if (!bInitialized)
		{
			ObjInit();
		}
		Transform transform = base.transform;
		MessageResource instance = MessageResource.Instance;
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
		bool flag = ((avatar_.level == avatar_.limitLevelMax) ? true : false);
		Debug.Log("limit" + avatar_.limitLevelMax);
		if (dataTbl == null)
		{
			dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		}
		if (isGacha)
		{
			buttons_[2].gameObject.SetActive(false);
			buttons_[3].gameObject.SetActive(true);
			buttons_[0].gameObject.SetActive(true);
			buttons_[1].gameObject.SetActive(false);
		}
		else
		{
			buttons_[3].gameObject.SetActive(false);
			if (flag)
			{
				buttons_[2].gameObject.SetActive(false);
				buttons_[0].gameObject.SetActive(false);
				buttons_[1].gameObject.SetActive(true);
			}
			else
			{
				buttons_[2].gameObject.SetActive(true);
				buttons_[0].gameObject.SetActive(true);
				buttons_[1].gameObject.SetActive(false);
				LevelButtonSwitch(avatar_.level != avatar_.limitLevel && avatar_.level > 0);
				string message = instance.getMessage(8802);
				message = instance.castCtrlCode(message, 1, (avatar_.level + 1).ToString());
				labels_[2].text = message;
			}
		}
		labels_[3].transform.parent.gameObject.SetActive(true);
		labels_[3].text = avatar_.level.ToString();
		float chara_img_expand_rate = GlobalData.Instance.chara_img_expand_rate;
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
		chara_00.spriteName = "avatarLvUP_00" + text + "_00";
		chara_00.MakePixelPerfect();
		chara_00.transform.localScale = new Vector3(chara_00.transform.localScale.x * chara_img_expand_rate, chara_00.transform.localScale.y * chara_img_expand_rate, 1f);
		chara_01.spriteName = "avatarLvUP_01" + text2 + "_00";
		chara_01.MakePixelPerfect();
		chara_01.transform.localScale = new Vector3(chara_01.transform.localScale.x * chara_img_expand_rate, chara_01.transform.localScale.y * chara_img_expand_rate, 1f);
		chara_00.gameObject.SetActive(true);
		chara_01.gameObject.SetActive(true);
		chara_00.transform.localRotation = new Quaternion(0f, 180f, 0f, 0f);
		int[] sKILL_SCORE_LIST = Constant.SKILL_SCORE_LIST;
		int[] sKILL_SCORE_LIST2 = Constant.SKILL_SCORE_LIST2;
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
		if (avatar_.specialSkill >= 30)
		{
			Constant.Avatar.eSpecialSkill[] array2 = new Constant.Avatar.eSpecialSkill[2];
			for (int j = 0; j < 2; j++)
			{
				array2[j] = Constant.Avatar.SpecialSkills[avatar_.specialSkill - 30, j];
			}
			float[] array3 = new float[2];
			int level = ((avatar_.level <= 0) ? 1 : avatar_.level);
			for (int k = 0; k < 2; k++)
			{
				array3[k] = dataTbl.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo((int)array2[k], level);
				if (array2[k] == Constant.Avatar.eSpecialSkill.GuideStretch)
				{
					array3[k] -= 4f;
				}
			}
			text3 = instance.getMessage(7100 + avatar_.specialSkill);
			text3 = instance.castCtrlCode(text3, 1, array3[0].ToString());
			text3 = instance.castCtrlCode(text3, 2, array3[1].ToString());
		}
		else
		{
			float num = 0f;
			num = dataTbl.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo(avatar_.specialSkill, avatar_.level);
			if (avatar_.specialSkill == 6)
			{
				num -= 4f;
			}
			if (avatar_.specialSkill > 0)
			{
				text3 = instance.getMessage(7100 + avatar_.specialSkill);
				text3 = instance.castCtrlCode(text3, 1, num.ToString());
			}
			else
			{
				empty = empty.Remove(empty.Length - 1, 1);
			}
		}
		empty += text3;
		lvup_img_00.SetActive(!bReleaseLimit);
		lvup_img_01.SetActive(bReleaseLimit);
		labels_[0].gameObject.SetActive(bReleaseLimit);
		string text4 = string.Empty;
		if (bReleaseLimit)
		{
			text4 = instance.getMessage(8817);
			text4 = instance.castCtrlCode(text4, 1, (avatar_.limitLevel - 1).ToString());
			text4 = instance.castCtrlCode(text4, 2, avatar_.limitLevel.ToString());
		}
		labels_[0].text = text4;
		labels_[1].text = empty;
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
		switch (trigger.name)
		{
		case "CloseButton":
			Constant.SoundUtil.PlayCancelSE();
			if (GlobalData.Instance.isGachaAfterOpeningDialog)
			{
				if (optionData.getFlag(SaveOptionData.eFlag.BGM))
				{
					Sound.Instance.setBgmMasterVolume(Sound.Instance.getDefaultBgmVolume());
				}
				Sound.Instance.playBgm(Sound.eBgm.BGM_010_Map, true);
			}
			GlobalData.Instance.acInfo_.isSetup = false;
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "LvupButton":
			Constant.SoundUtil.PlayDecideSE();
			if (GlobalData.Instance.isGachaAfterOpeningDialog)
			{
				if (optionData.getFlag(SaveOptionData.eFlag.BGM))
				{
					Sound.Instance.setBgmMasterVolume(Sound.Instance.getDefaultBgmVolume());
				}
				Sound.Instance.playBgm(Sound.eBgm.BGM_010_Map, true);
			}
			yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarLevelup(this, avatar_.index));
			break;
		case "DetailButton":
		case "DetailButton_center":
			Constant.SoundUtil.PlayDecideSE();
			if (GlobalData.Instance.isGachaAfterOpeningDialog)
			{
				if (optionData.getFlag(SaveOptionData.eFlag.BGM))
				{
					Sound.Instance.setBgmMasterVolume(Sound.Instance.getDefaultBgmVolume());
				}
				Sound.Instance.playBgm(Sound.eBgm.BGM_010_Map, true);
			}
			yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarProfile(this, avatar_.index));
			break;
		case "RetryButton":
		{
			Constant.SoundUtil.PlayDecideSE();
			if (GlobalData.Instance.isGachaAfterOpeningDialog)
			{
				if (optionData.getFlag(SaveOptionData.eFlag.BGM))
				{
					Sound.Instance.setBgmMasterVolume(Sound.Instance.getDefaultBgmVolume() / 4f);
				}
				Sound.Instance.playBgm(Sound.eBgm.BGM_010_Map, true);
			}
			bool isFree = GlobalData.Instance.getGameData().isFirstGacha;
			yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarGacha(this, isFree));
			break;
		}
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

	private IEnumerator plusButton(GameObject trigger)
	{
		switch (trigger.transform.parent.name)
		{
		case "01_coin":
		{
			DialogAllShop dialog = dialogManager_.getDialog(DialogManager.eDialog.AllShop) as DialogAllShop;
			yield return dialogManager_.StartCoroutine(dialog.show(DialogAllShop.ePanelType.Coin));
			break;
		}
		case "02_jewel":
		{
			DialogAllShop dialog2 = dialogManager_.getDialog(DialogManager.eDialog.AllShop) as DialogAllShop;
			yield return dialogManager_.StartCoroutine(dialog2.show(DialogAllShop.ePanelType.Jewel));
			break;
		}
		}
	}

	private void LevelButtonSwitch(bool isEnable)
	{
		if (levelButtonFirstState == 0)
		{
			levelButtonFirstState = (isEnable ? 1 : 2);
		}
		if (levelButtonFirstState == 1)
		{
			buttons_[2].setEnable(isEnable);
		}
		else
		{
			buttons_[2].isEnabled = isEnable;
		}
		UILabel component = buttons_[2].transform.Find("label_lvup").GetComponent<UILabel>();
		UILabel uILabel = labels_[2];
		if (isEnable)
		{
			component.color = Color.white;
			uILabel.color = Color.yellow;
		}
		else
		{
			component.color = Color.grey;
			uILabel.color = Color.grey;
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
}
