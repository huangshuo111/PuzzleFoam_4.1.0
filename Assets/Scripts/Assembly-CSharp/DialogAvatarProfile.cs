using System.Collections;
using LitJson;
using Network;
using UnityEngine;

public class DialogAvatarProfile : DialogBase
{
	public enum eBtn
	{
		Close = 0,
		Change = 1,
		Retry = 2,
		Levelup = 3,
		Max = 4
	}

	public enum eText
	{
		AvatarName = 0,
		AvatarLevel = 1,
		AvatarLimitDescription = 2,
		SkillName = 3,
		SkillInfo = 4,
		Max = 5
	}

	private GameObject dataTbl;

	private Network.Avatar choiceAvatar;

	private string avatarName = string.Empty;

	private UISprite rankStar;

	private Transform avatarParent;

	private Vector3 avatarParentPosDiff = new Vector3(0f, -12f, 0f);

	private Vector3[] avatarParentPositionList;

	private UISprite chara_00;

	private UISprite chara_01;

	private UISprite chara_02;

	private UISprite chara_03;

	private UILabel[] labels;

	private int choiceAvatarID;

	private UIButton[] buttons_ = new UIButton[4];

	private MainMenu mainMenu_;

	private bool firstOpenLevelMax;

	private bool blowcheck;

	private bool bInitialized;

	private int levelButtonFirstState;

	private int changeButtonFirstState;

	public override void OnCreate()
	{
		mainMenu_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
	}

	private void ObjInit()
	{
		Transform transform = base.transform.Find("window");
		Transform transform2 = null;
		if (buttons_[0] == null)
		{
			transform2 = base.transform.Find("window/CloseButton");
			buttons_[0] = transform2.GetComponent<UIButton>();
		}
		if (buttons_[1] == null)
		{
			transform2 = transform.Find("ChangeButton");
			buttons_[1] = transform2.GetComponent<UIButton>();
		}
		if (buttons_[3] == null)
		{
			transform2 = transform.Find("LevelupButton");
			buttons_[3] = transform2.GetComponent<UIButton>();
		}
		if (labels == null)
		{
			labels = new UILabel[5];
			labels[0] = transform.Find("labels/label_avatar_name").GetComponent<UILabel>();
			labels[4] = transform.Find("labels/label_skill_info").GetComponent<UILabel>();
			labels[1] = transform.Find("labels/label_avatar_level").GetComponent<UILabel>();
			labels[2] = transform.Find("labels/label_description").GetComponent<UILabel>();
		}
		if (avatarParent == null)
		{
			avatarParent = transform.Find("Avatar");
			avatarParentPositionList = new Vector3[2]
			{
				avatarParent.localPosition,
				avatarParent.localPosition + avatarParentPosDiff
			};
		}
		if (chara_00 == null)
		{
			chara_00 = transform.Find("Avatar/chara_00").GetComponent<UISprite>();
		}
		if (chara_01 == null)
		{
			chara_01 = transform.Find("Avatar/chara_01").GetComponent<UISprite>();
		}
		if (chara_02 == null)
		{
			chara_02 = transform.Find("Avatar/chara_02").GetComponent<UISprite>();
		}
		if (chara_03 == null)
		{
			chara_03 = transform.Find("Avatar/chara_03").GetComponent<UISprite>();
		}
		if (rankStar == null)
		{
			rankStar = transform.Find("rank/star_00").GetComponent<UISprite>();
		}
		bInitialized = true;
	}

	public void setup(int avatarID)
	{
		blowcheck = ResourceLoader.Instance.isUseLowResource();
		if (!bInitialized)
		{
			ObjInit();
		}
		if (dataTbl == null)
		{
			dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		}
		Transform transform = base.transform.Find("window");
		Debug.Log("avatarID = " + avatarID);
		choiceAvatarID = avatarID;
		Network.Avatar[] array = null;
		array = GlobalData.Instance.getGameData().avatarList;
		Network.Avatar[] array2 = array;
		foreach (Network.Avatar avatar in array2)
		{
			if (avatar.index == avatarID)
			{
				choiceAvatar = avatar;
				break;
			}
		}
		LevelButtonSwitch(choiceAvatar.level != choiceAvatar.limitLevel && choiceAvatar.level > 0);
		int limitLevelMax = choiceAvatar.limitLevelMax;
		labels[2].gameObject.SetActive(choiceAvatar.level == choiceAvatar.limitLevel && choiceAvatar.level != limitLevelMax);
		if (choiceAvatar.wearFlg == 0)
		{
			ChangeButtonSwitch(choiceAvatar.level > 0);
		}
		else
		{
			ChangeButtonSwitch(false);
		}
		labels[1].gameObject.SetActive(choiceAvatar.level > 0);
		MessageResource instance = MessageResource.Instance;
		int[] sKILL_SCORE_LIST = Constant.SKILL_SCORE_LIST;
		int[] sKILL_SCORE_LIST2 = Constant.SKILL_SCORE_LIST2;
		string empty = string.Empty;
		string empty2 = string.Empty;
		empty2 = ((choiceAvatar.baseSkill_3 < 0) ? ((choiceAvatar.level <= 0) ? sKILL_SCORE_LIST[0].ToString() : sKILL_SCORE_LIST[choiceAvatar.level - 1].ToString()) : ((choiceAvatar.level <= 0) ? sKILL_SCORE_LIST2[0].ToString() : sKILL_SCORE_LIST2[choiceAvatar.level - 1].ToString()));
		if (choiceAvatar.baseSkill_3 == -1)
		{
			empty = instance.getMessage(8814);
			empty = instance.castCtrlCode(empty, 1, instance.getMessage(8840 + choiceAvatar.baseSkill_1));
			empty = instance.castCtrlCode(empty, 2, instance.getMessage(8840 + choiceAvatar.baseSkill_2));
			empty = instance.castCtrlCode(empty, 3, empty2);
		}
		else
		{
			empty = instance.getMessage(8837);
			empty = instance.castCtrlCode(empty, 1, instance.getMessage(8840 + choiceAvatar.baseSkill_1));
			empty = instance.castCtrlCode(empty, 2, instance.getMessage(8840 + choiceAvatar.baseSkill_2));
			empty = instance.castCtrlCode(empty, 3, instance.getMessage(8840 + choiceAvatar.baseSkill_3));
			empty = instance.castCtrlCode(empty, 4, empty2);
		}
		if (choiceAvatar.specialSkill >= 30)
		{
			Constant.Avatar.eSpecialSkill[] array3 = new Constant.Avatar.eSpecialSkill[2];
			for (int j = 0; j < 2; j++)
			{
				array3[j] = Constant.Avatar.SpecialSkills[choiceAvatar.specialSkill - 30, j];
			}
			float[] array4 = new float[2];
			int level = ((choiceAvatar.level <= 0) ? 1 : choiceAvatar.level);
			for (int k = 0; k < 2; k++)
			{
				array4[k] = dataTbl.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo((int)array3[k], level);
				if (array3[k] == Constant.Avatar.eSpecialSkill.GuideStretch)
				{
					array4[k] -= 4f;
				}
			}
			if (choiceAvatar.specialSkill > 0)
			{
				string message = instance.getMessage(7000 + choiceAvatar.specialSkill);
				message = instance.castCtrlCode(message, 1, array4[0].ToString());
				message = instance.castCtrlCode(message, 2, array4[1].ToString());
				empty += message;
			}
			else
			{
				empty = empty.Remove(empty.Length - 1, 1);
			}
		}
		else
		{
			float num = 0f;
			num = ((choiceAvatar.level <= 0) ? dataTbl.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo(choiceAvatar.specialSkill, 1) : dataTbl.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo(choiceAvatar.specialSkill, choiceAvatar.level));
			if (choiceAvatar.specialSkill == 6)
			{
				num -= 4f;
			}
			if (choiceAvatar.specialSkill > 0)
			{
				string message2 = instance.getMessage(7000 + choiceAvatar.specialSkill);
				message2 = instance.castCtrlCode(message2, 1, num.ToString());
				empty += message2;
			}
			else
			{
				empty = empty.Remove(empty.Length - 1, 1);
			}
		}
		labels[4].text = empty;
		if (avatarID >= 23000)
		{
			avatarName = instance.getMessage(8600 + (avatarID - 23000));
			rankStar.spriteName = "rank_star_large_03";
		}
		else if (avatarID >= 22000)
		{
			avatarName = instance.getMessage(8500 + (avatarID - 22000));
			rankStar.spriteName = "rank_star_large_01";
		}
		else if (avatarID >= 21000)
		{
			avatarName = instance.getMessage(8400 + (avatarID - 21000));
			rankStar.spriteName = "rank_star_large_02";
		}
		else
		{
			avatarName = instance.getMessage(8300 + (avatarID - 20000));
			rankStar.spriteName = "rank_star_large_00";
		}
		if (choiceAvatar.level <= 0)
		{
			avatarName = "  " + instance.getMessage(2581) + "  ";
		}
		string text = string.Empty;
		string text2 = avatarName + " ";
		float chara_img_diminish_rate = GlobalData.Instance.chara_img_diminish_rate;
		string text3 = ((choiceAvatar.throwCharacter <= 0) ? string.Empty : ("_" + (choiceAvatar.throwCharacter - 1).ToString("00")));
		string text4 = ((choiceAvatar.supportCharacter <= 0) ? string.Empty : ("_" + (choiceAvatar.supportCharacter - 1).ToString("00")));
		string text5 = string.Empty;
		if (choiceAvatar.level <= 0)
		{
			text5 = "_sh";
		}
		if (choiceAvatar.throwCharacter - 1 > 18)
		{
			chara_00.atlas = chara_03.atlas;
		}
		else
		{
			chara_00.atlas = chara_02.atlas;
		}
		if (choiceAvatar.supportCharacter - 1 > 18)
		{
			chara_01.atlas = chara_03.atlas;
		}
		else
		{
			chara_01.atlas = chara_02.atlas;
		}
		chara_00.spriteName = "avatar_00" + text3 + "_00" + text5;
		chara_00.MakePixelPerfect();
		chara_01.spriteName = "avatar_01" + text4 + "_00" + text5;
		chara_01.MakePixelPerfect();
		chara_00.gameObject.SetActive(true);
		chara_01.gameObject.SetActive(true);
		avatarParent.localPosition = avatarParentPositionList[(avatarID == 22009) ? 1u : 0u];
		if (choiceAvatar.level > 0)
		{
			chara_00.transform.localScale = new Vector3(chara_00.transform.localScale.x / chara_img_diminish_rate, chara_00.transform.localScale.y / chara_img_diminish_rate, 1f);
			chara_01.transform.localScale = new Vector3(chara_01.transform.localScale.x / chara_img_diminish_rate, chara_01.transform.localScale.y / chara_img_diminish_rate, 1f);
			text = instance.getMessage(8830);
			text = instance.castCtrlCode(text, 1, choiceAvatar.limitLevel.ToString());
			text = instance.castCtrlCode(text, 2, limitLevelMax.ToString());
			text2 += instance.getMessage(8802);
			text2 = instance.castCtrlCode(text2, 1, choiceAvatar.level.ToString());
			string message3 = instance.getMessage(7100 + choiceAvatar.specialSkill);
		}
		labels[0].text = text2;
		labels[1].text = text;
		SetRankStarPosition();
		if (buttons_[3].tweenTarget.GetComponent<TweenColor>() != null)
		{
			buttons_[3].tweenTarget.GetComponent<TweenColor>().enabled = true;
		}
	}

	public void SetRankStarPosition()
	{
		float num = labels[0].relativeSize.x * labels[0].transform.localScale.x;
		Debug.Log("size_x = " + num);
		Debug.Log(labels[0].relativeSize.x);
		Debug.Log(labels[0].transform.localScale.x);
		rankStar.transform.parent.localPosition = new Vector3(labels[0].transform.localPosition.x - num / 2f - rankStar.transform.localScale.x / 2f, rankStar.transform.parent.localPosition.y, rankStar.transform.parent.localPosition.z);
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "CloseButton":
		{
			Constant.SoundUtil.PlayCancelSE();
			DialogAvatarCollection ac = dialogManager_.getDialog(DialogManager.eDialog.AvatarCollection) as DialogAvatarCollection;
			if (ac != null && !ac.isOpen() && !GlobalData.Instance.isGachaAfterOpeningDialog)
			{
				yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarCollection(this));
				break;
			}
			GlobalData.Instance.isGachaAfterOpeningDialog = false;
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
		case "ChangeButton":
			Constant.SoundUtil.PlayDecideSE();
			yield return StartCoroutine(AvatarChange(choiceAvatar));
			break;
		case "GachaButton":
		{
			Constant.SoundUtil.PlayButtonSE();
			bool isFree2 = false;
			isFree2 = GlobalData.Instance.getGameData().isFirstGacha;
			yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarGacha(null, isFree2));
			break;
		}
		case "LevelupButton":
			Constant.SoundUtil.PlayDecideSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarLevelup(null, choiceAvatarID));
			break;
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
			buttons_[3].setEnable(isEnable);
		}
		else
		{
			buttons_[3].isEnabled = isEnable;
		}
		UILabel component = buttons_[3].transform.Find("label_message").GetComponent<UILabel>();
		if (isEnable)
		{
			component.color = Color.white;
		}
		else
		{
			component.color = Color.grey;
		}
	}

	private void ChangeButtonSwitch(bool isEnable)
	{
		if (changeButtonFirstState == 0)
		{
			changeButtonFirstState = (isEnable ? 1 : 2);
		}
		if (changeButtonFirstState == 1)
		{
			buttons_[1].setEnable(isEnable);
		}
		else
		{
			buttons_[1].isEnabled = isEnable;
		}
		UILabel component = buttons_[1].transform.Find("label_message").GetComponent<UILabel>();
		if (isEnable)
		{
			component.color = Color.white;
		}
		else
		{
			component.color = Color.grey;
		}
	}

	private IEnumerator AvatarChange(Network.Avatar avatar)
	{
		Input.enable = false;
		Hashtable h = Hash.AvatarSetWear(avatar.index);
		NetworkMng.Instance.setup(h);
		yield return StartCoroutine(NetworkMng.Instance.download(API.AvatarSetwear, false, true));
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
		AvatarSetWear resultData_ = JsonMapper.ToObject<AvatarSetWear>(www.text);
		GlobalData.Instance.getGameData().setCommonData(resultData_, true);
		GlobalData.Instance.getGameData().inviteBasicReward = resultData_.inviteBasicReward;
		GlobalData.Instance.getGameData().avatarList = resultData_.avatarList;
		GlobalData.Instance.getGameData().continueNum = resultData_.continueNum;
		GlobalData.Instance.getGameData().heartRecoverTime = resultData_.heartRecoverTime;
		GlobalData.Instance.getGameData().isAllAvatarLevelMax = resultData_.isAllAvatarLevelMax;
		www.Dispose();
		www = null;
		if (GlobalData.Instance.getGameData().avatarList != null)
		{
			int[] rank_count = new int[4];
			Network.Avatar[] avatarList = GlobalData.Instance.getGameData().avatarList;
			foreach (Network.Avatar av in avatarList)
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
		DummyPlayerData.Data.avatarId = avatar.index;
		DummyPlayerData.Data.throwId = avatar.throwCharacter;
		DummyPlayerData.Data.supportId = avatar.supportCharacter;
		DialogAvatarCollection c = dialogManager_.getDialog(DialogManager.eDialog.AvatarCollection) as DialogAvatarCollection;
		if (c.isOpen())
		{
			c.setIconChange(choiceAvatarID);
		}
		DialogSetup setup = dialogManager_.getDialog(DialogManager.eDialog.Setup) as DialogSetup;
		if (setup != null && setup.isOpen())
		{
			setup.UpdateCharaIcon();
		}
		DialogEventSetup eSetup = dialogManager_.getDialog(DialogManager.eDialog.EventSetup) as DialogEventSetup;
		if (eSetup != null && eSetup.isOpen())
		{
			eSetup.UpdateCharaIcon();
		}
		DialogCollaborationSetup cSetup = dialogManager_.getDialog(DialogManager.eDialog.CollaborationSetup) as DialogCollaborationSetup;
		if (cSetup != null && cSetup.isOpen())
		{
			cSetup.UpdateCharaIcon();
		}
		if (partManager_.currentPart == PartManager.ePart.Map)
		{
			partManager_.transform.Find("Part_Map").GetComponent<Part_Map>().UpdateCharaIcon();
		}
		else if (partManager_.currentPart == PartManager.ePart.EventMap)
		{
			partManager_.transform.Find("Part_EventMap").GetComponent<Part_EventMap>().UpdateCharaIcon();
		}
		else if (partManager_.currentPart == PartManager.ePart.CollaborationMap)
		{
			partManager_.transform.Find("Part_CollaborationMap").GetComponent<Part_CollaborationMap>().UpdateCharaIcon();
		}
		int setupItemCoin_ = 0;
		if (partManager_.currentPart == PartManager.ePart.Map)
		{
			if (setup.isOpen())
			{
				setupItemCoin_ = setup.getSetItemCoin();
			}
		}
		else if (partManager_.currentPart == PartManager.ePart.CollaborationMap)
		{
			if (cSetup != null && cSetup.isOpen())
			{
				setupItemCoin_ = cSetup.getSetItemCoin();
			}
		}
		else if (partManager_.currentPart == PartManager.ePart.EventMap && eSetup != null && eSetup.isOpen())
		{
			setupItemCoin_ = eSetup.getSetItemCoin();
		}
		GlobalData.Instance.getGameData().coin -= setupItemCoin_;
		mainMenu_.update();
		MessageResource msg = MessageResource.Instance;
		string showText3 = msg.getMessage(8808);
		showText3 = msg.castCtrlCode(showText3, 1, avatarName);
		showText3 = msg.castCtrlCode(showText3, 2, avatar.level.ToString());
		Input.enable = true;
		DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		dialog.setup(showText3, null, null, true);
		dialog.setButtonActive(DialogCommon.eBtn.Close, false);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
		ChangeButtonSwitch(false);
	}
}
