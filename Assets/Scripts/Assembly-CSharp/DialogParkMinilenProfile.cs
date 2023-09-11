using System.Collections;
using Bridge;
using LitJson;
using Network;
using UnityEngine;

public class DialogParkMinilenProfile : DialogBase
{
	public enum eBtn
	{
		Close = 0,
		Change = 1,
		Max = 2
	}

	public enum eText
	{
		MinilenName = 0,
		MinilenLevel = 1,
		SkillInfo = 2,
		BuildingName = 3,
		BuildingGridNum = 4,
		BuildingRewardNum = 5,
		Max = 6
	}

	private Transform minilenParent;

	private Vector3 minilenParentPosDiff = new Vector3(0f, -12f, 0f);

	private Vector3[] minilenParentPositionList;

	private GameObject dataTbl;

	private Network.MinilenData choiceMinilen;

	private UISprite chara;

	private UISprite building;

	private UISprite reward_icon;

	private UILabel[] labels;

	private int choiceMinilenID;

	private UIButton[] buttons_ = new UIButton[2];

	private MainMenu mainMenu_;

	private bool bInitialized;

	private int changeButtonFirstState;

	public override void OnCreate()
	{
		mainMenu_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
	}

	public override void OnOpen()
	{
		dialogManager_.addActiveDialogList(DialogManager.eDialog.ParkMinilenProfile);
	}

	public override void OnClose()
	{
		dialogManager_.removeActiveDialogList(DialogManager.eDialog.ParkMinilenProfile);
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
		if (labels == null)
		{
			labels = new UILabel[6];
			labels[0] = transform.Find("labels/label_minilen_name").GetComponent<UILabel>();
			labels[2] = transform.Find("labels/label_skill_info").GetComponent<UILabel>();
			labels[1] = transform.Find("labels/label_skilleff_level").GetComponent<UILabel>();
			labels[3] = transform.Find("labels/label_building_name").GetComponent<UILabel>();
			labels[4] = transform.Find("labels/label_grid").GetComponent<UILabel>();
			labels[5] = transform.Find("labels/label_reward_count").GetComponent<UILabel>();
		}
		if (minilenParent == null)
		{
			minilenParent = transform.Find("Avatar");
			minilenParentPositionList = new Vector3[2]
			{
				minilenParent.localPosition,
				minilenParent.localPosition + minilenParentPosDiff
			};
		}
		if (chara == null)
		{
			chara = transform.Find("Avatar/minilen_00").GetComponent<UISprite>();
		}
		if (building == null)
		{
			building = transform.Find("Avatar/building_01").GetComponent<UISprite>();
		}
		if (reward_icon == null)
		{
			reward_icon = transform.Find("Avatar/reward_icon").GetComponent<UISprite>();
		}
		bInitialized = true;
	}

	public void setup(int minilenID)
	{
		MessageResource instance = MessageResource.Instance;
		if (!bInitialized)
		{
			ObjInit();
		}
		if (dataTbl == null)
		{
			dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		}
		Transform transform = base.transform.Find("window");
		choiceMinilenID = minilenID;
		Network.MinilenData[] minielenData = Bridge.MinilenData.getMinielenData();
		foreach (Network.MinilenData minilenData in minielenData)
		{
			if (minilenData.index == minilenID)
			{
				choiceMinilen = minilenData;
				break;
			}
		}
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		ParkBuildingDataTable component = @object.GetComponent<ParkBuildingDataTable>();
		component.load();
		if (choiceMinilen.releaseBuildingID != -1)
		{
			building.gameObject.SetActive(true);
			reward_icon.gameObject.SetActive(true);
			ParkBuildingInfo.BuildingInfo info = component.getInfo(choiceMinilen.releaseBuildingID);
			string message = instance.getMessage(info.NameID);
			labels[3].text = message;
			string spriteName = info.SpriteName;
			string building_sprite_name = "UI_picturebook_" + spriteName;
			if (building.atlas.spriteList.Exists((UIAtlas.Sprite s) => s.name == building_sprite_name))
			{
				building.gameObject.SetActive(true);
				building.spriteName = building_sprite_name;
				building.MakePixelPerfect();
			}
			else
			{
				building.gameObject.SetActive(false);
			}
			string text = info.GridWidth.ToString();
			string text2 = info.GridHeight.ToString();
			string text3 = text + "x" + text2;
			labels[4].text = text3;
			string text4 = info.RewardNum.ToString();
			if (text4 == "0")
			{
				text4 = instance.getMessage(9113);
				reward_icon.gameObject.SetActive(false);
			}
			else
			{
				reward_icon.gameObject.SetActive(true);
			}
			labels[5].text = text4;
			if (info.RewardType != 0)
			{
				switch (info.RewardType)
				{
				case 3:
					reward_icon.spriteName = "UI_icon_coin_00";
					reward_icon.MakePixelPerfect();
					reward_icon.transform.localScale *= 0.75f;
					break;
				case 4:
					reward_icon.spriteName = "UI_icon_heart_00";
					reward_icon.MakePixelPerfect();
					reward_icon.transform.localScale *= 0.75f;
					break;
				case 6:
					reward_icon.spriteName = "item_" + (info.RewardType % 1000).ToString("000") + "_00";
					reward_icon.MakePixelPerfect();
					reward_icon.transform.localScale *= 0.35f;
					break;
				}
			}
			int[] sKILL_SCORE_LIST = Constant.SKILL_SCORE_LIST;
			int[] sKILL_SCORE_LIST2 = Constant.SKILL_SCORE_LIST2;
			string empty = string.Empty;
			string empty2 = string.Empty;
			empty2 = ((choiceMinilen.level <= 0) ? sKILL_SCORE_LIST[0].ToString() : sKILL_SCORE_LIST[choiceMinilen.level - 1].ToString());
			empty = instance.getMessage(8814);
			empty = instance.castCtrlCode(empty, 1, instance.getMessage(8840 + choiceMinilen.baseSkill_1));
			empty = instance.castCtrlCode(empty, 2, instance.getMessage(8840 + choiceMinilen.baseSkill_2));
			empty = instance.castCtrlCode(empty, 3, empty2);
			if (choiceMinilen.specialSkill >= 30)
			{
				Constant.Avatar.eSpecialSkill[] array = new Constant.Avatar.eSpecialSkill[2];
				for (int j = 0; j < 2; j++)
				{
					array[j] = Constant.Avatar.SpecialSkills[choiceMinilen.specialSkill - 30, j];
				}
				float[] array2 = new float[2];
				int level = ((choiceMinilen.level <= 0) ? 1 : choiceMinilen.level);
				for (int k = 0; k < 2; k++)
				{
					array2[k] = dataTbl.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo((int)array[k], level);
					if (array[k] == Constant.Avatar.eSpecialSkill.GuideStretch)
					{
						array2[k] -= 4f;
					}
				}
				if (choiceMinilen.specialSkill > 0)
				{
					string message2 = instance.getMessage(8600 + choiceMinilen.specialSkill);
					message2 = instance.castCtrlCode(message2, 1, array2[0].ToString());
					message2 = instance.castCtrlCode(message2, 2, array2[1].ToString());
					empty += message2;
				}
				else
				{
					empty = empty.Remove(empty.Length - 1, 1);
				}
			}
			else
			{
				float num = 0f;
				num = ((choiceMinilen.level <= 0) ? dataTbl.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo(choiceMinilen.specialSkill, 1) : dataTbl.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo(choiceMinilen.specialSkill, choiceMinilen.level));
				if (choiceMinilen.specialSkill == 6)
				{
					num -= 4f;
				}
				if (choiceMinilen.specialSkill > 0)
				{
					string message3 = instance.getMessage(7000 + choiceMinilen.specialSkill);
					message3 = instance.castCtrlCode(message3, 1, num.ToString());
					empty += message3;
				}
				else
				{
					empty = empty.Remove(empty.Length - 1, 1);
				}
			}
			labels[2].text = empty;
		}
		else
		{
			string message4 = MessageResource.Instance.getMessage(9113);
			building.gameObject.SetActive(false);
			reward_icon.gameObject.SetActive(false);
			labels[3].text = message4;
			labels[4].text = "-  ";
			labels[5].text = message4;
			labels[2].text = message4;
		}
		if (choiceMinilen.wearFlg == 0)
		{
			ChangeButtonSwitch(choiceMinilen.level > 0);
		}
		else
		{
			ChangeButtonSwitch(false);
		}
		string text5 = "UI_";
		string text6 = ((choiceMinilen.level > 0) ? "picturebook_mini_" : "silhouette_mini_");
		text5 = text5 + text6 + (choiceMinilen.index % 10000).ToString("000");
		chara.spriteName = text5;
		chara.MakePixelPerfect();
		chara.gameObject.SetActive(true);
		minilenParent.localPosition = minilenParentPositionList[(minilenID == 22009) ? 1u : 0u];
		int messageID;
		if (choiceMinilen.level > 0)
		{
			labels[1].gameObject.SetActive(true);
			string text7 = string.Format("Lv.{0}/{1}", choiceMinilen.level, choiceMinilen.maxLevel);
			int num2 = -1;
			if (choiceMinilen.releaseBuildingID == num2)
			{
				text7 = string.Empty;
			}
			labels[1].text = text7;
			messageID = choiceMinilen.nameID;
		}
		else
		{
			messageID = 2581;
			labels[1].gameObject.SetActive(false);
		}
		string empty3 = string.Empty;
		empty3 = instance.getMessage(messageID);
		labels[0].text = empty3;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "CloseButton":
		{
			if (GlobalData.Instance.isParkControlAfterOpeningDialog)
			{
				Constant.SoundUtil.PlayCancelSE();
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
				GlobalData.Instance.isParkControlAfterOpeningDialog = false;
				break;
			}
			Constant.SoundUtil.PlayCancelSE();
			DialogParkMinilenCollection ac = dialogManager_.getDialog(DialogManager.eDialog.ParkMinilenCollection) as DialogParkMinilenCollection;
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
			yield return StartCoroutine(MinilenChange(choiceMinilen));
			break;
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

	private IEnumerator MinilenChange(Network.MinilenData minilen)
	{
		Input.enable = false;
		Hashtable h = new Hashtable { 
		{
			"minilenId",
			minilen.index.ToString()
		} };
		NetworkMng.Instance.setup(h);
		yield return StartCoroutine(NetworkMng.Instance.download(API.MinilenSetwear, false, true));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield return dialogManager_.StartCoroutine(NetworkMng.Instance.showIcon(false));
			yield break;
		}
		WWW www__ = NetworkMng.Instance.getWWW();
		Network.PlayerData player_data = JsonMapper.ToObject<Network.PlayerData>(www__.text);
		GameData game_data = GlobalData.Instance.getGameData();
		game_data.bonusJewel = player_data.bonusJewel;
		game_data.buyJewel = player_data.buyJewel;
		game_data.treasureboxNum = player_data.treasureboxNum;
		game_data.heart = player_data.heart;
		game_data.coin = player_data.coin;
		game_data.exp = player_data.exp;
		game_data.level = player_data.level;
		game_data.allPlayCount = player_data.allPlayCount;
		game_data.allClearCount = player_data.allClearCount;
		game_data.allStageScoreSum = player_data.allStageScoreSum;
		game_data.minilenCount = player_data.minilenCount;
		game_data.minilenTotalCount = player_data.minilenTotalCount;
		game_data.giveNiceTotalCount = player_data.giveNiceTotalCount;
		game_data.giveNiceMonthlyCount = player_data.giveNiceMonthlyCount;
		game_data.tookNiceTotalCount = player_data.tookNiceTotalCount;
		game_data.minilenList = player_data.minilenList;
		DialogParkMinilenCollection c = dialogManager_.getDialog(DialogManager.eDialog.ParkMinilenCollection) as DialogParkMinilenCollection;
		if (c.isOpen())
		{
			c.setIconChange(choiceMinilenID);
		}
		DialogSetupPark setup_park = dialogManager_.getDialog(DialogManager.eDialog.ParkStageSetup) as DialogSetupPark;
		if (setup_park != null && setup_park.isOpen())
		{
			setup_park.UpdateCharaIcon();
		}
		if (partManager_.currentPart == PartManager.ePart.Park)
		{
			partManager_.transform.Find("Part_Park").GetComponent<Part_Park>().UpdateMinilenIcon();
		}
		int setupItemCoin_ = 0;
		if (partManager_.currentPart == PartManager.ePart.Park && setup_park.isOpen())
		{
			setupItemCoin_ = setup_park.getSetItemCoin();
		}
		GlobalData.Instance.getGameData().coin -= setupItemCoin_;
		mainMenu_.update();
		MessageResource msg = MessageResource.Instance;
		string showText3 = msg.getMessage(8808);
		showText3 = msg.castCtrlCode(showText3, 1, msg.getMessage(minilen.nameID));
		showText3 = msg.castCtrlCode(showText3, 2, Mathf.Max(minilen.level, 1).ToString());
		Input.enable = true;
		DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		dialog.setup(showText3, null, null, true);
		dialog.setButtonActive(DialogCommon.eBtn.Close, false);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
		ChangeButtonSwitch(false);
	}
}
