using System.Collections;
using LitJson;
using Network;
using UnityEngine;

public class DialogAvatarGachaResult : DialogBase
{
	public enum eBtn
	{
		Close = 0,
		Profile = 1,
		Retry = 2,
		Levelup = 3,
		Max = 4
	}

	public enum eText
	{
		AvatarName = 0,
		AvatarLevel = 1,
		AvatarDescription = 2,
		SkillName = 3,
		SkillInfo = 4,
		Max = 5
	}

	private UISprite rankStar;

	private GameObject newObj;

	private UILabel[] labels;

	private Network.Avatar resultAvatar;

	private int resultAvatarID;

	private string avatarName = string.Empty;

	private UIButton[] buttons_ = new UIButton[4];

	private MainMenu mainMenu_;

	private GameObject dataTbl;

	private int levelButtonFirstState;

	public override void OnCreate()
	{
	}

	private void ObjInit()
	{
		mainMenu_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		Transform transform = null;
		if (buttons_[0] == null)
		{
			transform = base.transform.Find("window/CloseButton");
			buttons_[0] = transform.GetComponent<UIButton>();
		}
		if (buttons_[1] == null)
		{
			transform = base.transform.Find("window/ChangeButton");
			buttons_[1] = transform.GetComponent<UIButton>();
		}
		if (buttons_[2] == null)
		{
			transform = base.transform.Find("window/RetryButton");
			buttons_[2] = transform.GetComponent<UIButton>();
		}
		if (labels == null)
		{
			labels = new UILabel[5];
			labels[0] = base.transform.Find("window/labels/label_avatar_name").GetComponent<UILabel>();
			labels[1] = base.transform.Find("window/labels/label_avatar_level").GetComponent<UILabel>();
		}
		if (rankStar == null)
		{
			rankStar = base.transform.Find("window/rank/star_00").GetComponent<UISprite>();
		}
	}

	public void setup(int avatarID, bool isNew)
	{
		ObjInit();
		resultAvatarID = avatarID;
		if (dataTbl == null)
		{
			dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		}
		Network.Avatar[] avatarList = GlobalData.Instance.getGameData().avatarList;
		Network.Avatar[] array = avatarList;
		foreach (Network.Avatar avatar in array)
		{
			if (avatar.index == avatarID)
			{
				resultAvatar = avatar;
				break;
			}
		}
		LevelButtonSwitch(resultAvatar.level != resultAvatar.limitLevel && resultAvatar.level > 0);
		MessageResource instance = MessageResource.Instance;
		int[] sKILL_SCORE_LIST = Constant.SKILL_SCORE_LIST;
		int[] sKILL_SCORE_LIST2 = Constant.SKILL_SCORE_LIST2;
		float num = 0f;
		num = ((resultAvatar.level <= 0) ? dataTbl.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo(resultAvatar.specialSkill, 1) : dataTbl.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo(resultAvatar.specialSkill, resultAvatar.level));
		string message = instance.getMessage(8840 + resultAvatar.baseSkill_1);
		string message2 = instance.getMessage(8840 + resultAvatar.baseSkill_2);
		string message3 = string.Empty;
		if (resultAvatar.baseSkill_3 != -1)
		{
			message3 = instance.getMessage(8840 + resultAvatar.baseSkill_3);
		}
		string empty = string.Empty;
		empty = ((resultAvatar.baseSkill_3 < 0) ? ((resultAvatar.level <= 0) ? string.Empty : sKILL_SCORE_LIST[resultAvatar.level - 1].ToString()) : ((resultAvatar.level <= 0) ? string.Empty : sKILL_SCORE_LIST2[resultAvatar.level - 1].ToString()));
		string message4;
		if (resultAvatar.baseSkill_3 == -1)
		{
			message4 = instance.getMessage(8814);
			message4 = instance.castCtrlCode(message4, 1, message);
			message4 = instance.castCtrlCode(message4, 2, message2);
			message4 = instance.castCtrlCode(message4, 3, empty);
		}
		else
		{
			message4 = instance.getMessage(8837);
			message4 = instance.castCtrlCode(message4, 1, message);
			message4 = instance.castCtrlCode(message4, 2, message2);
			message4 = instance.castCtrlCode(message4, 3, message3);
			message4 = instance.castCtrlCode(message4, 4, empty);
		}
		if (resultAvatar.specialSkill > 0)
		{
			string message5 = instance.getMessage(7000 + resultAvatar.specialSkill);
			message5 = instance.castCtrlCode(message5, 1, num.ToString());
			message4 += message5;
		}
		if (avatarID >= 23000)
		{
			avatarName = instance.getMessage(8600 + (avatarID - 23000));
			rankStar.spriteName = "stage_star_large_03";
		}
		else if (avatarID >= 22000)
		{
			avatarName = instance.getMessage(8500 + (avatarID - 22000));
			rankStar.spriteName = "stage_star_large_01";
		}
		else if (avatarID >= 21000)
		{
			avatarName = instance.getMessage(8400 + (avatarID - 21000));
			rankStar.spriteName = "stage_star_large_02";
		}
		else
		{
			avatarName = instance.getMessage(8300 + (avatarID - 20000));
			rankStar.spriteName = "stage_star_large_00";
		}
		string message6 = instance.getMessage(8802);
		message6 = instance.castCtrlCode(message6, 1, resultAvatar.level.ToString());
		labels[1].text = message6;
		labels[0].text = avatarName + "  " + message6;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "CloseButton":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "ProfileButton":
			Constant.SoundUtil.PlayDecideSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarProfile(this, resultAvatar.index));
			break;
		case "RetryButton":
		{
			Constant.SoundUtil.PlayButtonSE();
			bool isFree = GlobalData.Instance.getGameData().isFirstGacha;
			yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarGacha(this, isFree));
			break;
		}
		case "LevelupButton":
			Constant.SoundUtil.PlayDecideSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.OpenAvatarLevelup(this, resultAvatarID));
			break;
		}
	}

	private void LevelButtonSwitch(bool isEnable)
	{
		if (levelButtonFirstState == 0)
		{
			levelButtonFirstState = (isEnable ? 1 : 2);
		}
	}

	private IEnumerator AvatarChange(Network.Avatar avatar)
	{
		Hashtable h = Hash.AvatarSetWear(avatar.index);
		NetworkMng.Instance.setup(h);
		yield return StartCoroutine(NetworkMng.Instance.download(API.AvatarSetwear, false, true));
		while (NetworkMng.Instance.isDownloading())
		{
			yield return null;
		}
		WWW www2 = NetworkMng.Instance.getWWW();
		AvatarSetWear resultData_ = JsonMapper.ToObject<AvatarSetWear>(www2.text);
		GlobalData.Instance.getGameData().setCommonData(resultData_, true);
		GlobalData.Instance.getGameData().inviteBasicReward = resultData_.inviteBasicReward;
		GlobalData.Instance.getGameData().avatarList = resultData_.avatarList;
		GlobalData.Instance.getGameData().continueNum = resultData_.continueNum;
		GlobalData.Instance.getGameData().heartRecoverTime = resultData_.heartRecoverTime;
		GlobalData.Instance.getGameData().isAllAvatarLevelMax = resultData_.isAllAvatarLevelMax;
		www2.Dispose();
		www2 = null;
		mainMenu_.update();
		GlobalData.Instance.currentAvatar = avatar;
		MessageResource msgRes = MessageResource.Instance;
		string showText3 = msgRes.getMessage(8808);
		showText3 = msgRes.castCtrlCode(showText3, 1, avatarName);
		showText3 = msgRes.castCtrlCode(showText3, 2, avatar.level.ToString());
		DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		dialog.setup(showText3, null, null, true);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
	}
}
