using System;
using System.Collections;
using Bridge;
using Network;
using TapjoyUnity;
using UnityEngine;

public class DialogParkMinilenGet : DialogShortageBase
{
	public IEnumerator show(int minilen_id)
	{
		Input.enable = false;
		int minilen_id2 = default(int);
		Network.MinilenData minilen = Array.Find(Bridge.MinilenData.getMinielenData(), (Network.MinilenData m) => m.index == minilen_id2);
		bool is_level_up = minilen.level > 1;
		Transform window_trans = base.transform.Find("window/window_base_00");
		if ((bool)window_trans)
		{
			window_trans.localPosition += Vector3.back;
		}
		Transform new_trans = null;
		if (is_level_up)
		{
			Transform lv_lbl_trans = base.transform.Find("window/title_Lvup");
			if ((bool)lv_lbl_trans)
			{
				lv_lbl_trans.gameObject.SetActive(true);
			}
			Transform get_lbl_trans = base.transform.Find("window/title_rareminilen");
			if ((bool)get_lbl_trans)
			{
				get_lbl_trans.gameObject.SetActive(false);
			}
			Transform level_lbl_trans = base.transform.Find("window/avatar/arrow/Label/Label_Level_value");
			UILabel level_lbl = null;
			if ((bool)level_lbl_trans)
			{
				level_lbl = level_lbl_trans.GetComponent<UILabel>();
			}
			if ((bool)level_lbl)
			{
				level_lbl.text = minilen.level.ToString();
			}
			new_trans = base.transform.Find("window/new");
			if ((bool)new_trans)
			{
				new_trans.gameObject.SetActive(false);
			}
		}
		else
		{
			Transform lv_lbl_trans2 = base.transform.Find("window/title_Lvup");
			if ((bool)lv_lbl_trans2)
			{
				lv_lbl_trans2.gameObject.SetActive(false);
			}
			Transform get_lbl_trans2 = base.transform.Find("window/title_rareminilen");
			if ((bool)get_lbl_trans2)
			{
				get_lbl_trans2.gameObject.SetActive(true);
			}
			Transform notice_lbl_trans = base.transform.Find("window/levelup_label");
			if ((bool)notice_lbl_trans)
			{
				notice_lbl_trans.gameObject.SetActive(false);
			}
			Transform lvl_up_arrow = base.transform.Find("window/avatar/arrow");
			if ((bool)lvl_up_arrow)
			{
				lvl_up_arrow.gameObject.SetActive(false);
			}
			new_trans = base.transform.Find("window/new");
		}
		Transform name_lbl_trans = base.transform.Find("window/name_label");
		UILabel name_lbl = null;
		if ((bool)name_lbl_trans)
		{
			name_lbl = name_lbl_trans.GetComponent<UILabel>();
		}
		if ((bool)name_lbl)
		{
			name_lbl.text = MessageResource.Instance.getMessage(minilen.nameID);
			if ((bool)new_trans)
			{
				Vector3 new_pos = new_trans.localPosition;
				new_pos.x = -0.5f * (name_lbl.transform.localScale.x * name_lbl.relativeSize.x + new_trans.localScale.x);
				new_trans.localPosition = new_pos;
			}
		}
		Transform sprite_trans = base.transform.Find("window/avatar/minilen");
		UISprite minilen_sprite = null;
		if ((bool)sprite_trans)
		{
			minilen_sprite = sprite_trans.GetComponent<UISprite>();
		}
		if ((bool)minilen_sprite)
		{
			minilen_sprite.spriteName = "UI_picturebook_mini_" + (minilen_id % 10000).ToString("000");
			minilen_sprite.MakePixelPerfect();
			sprite_trans.localScale *= 1.4f;
		}
		Transform building_trans = base.transform.Find("window/Label/buildingname_label");
		UILabel building_lbl = null;
		if ((bool)building_trans)
		{
			building_lbl = building_trans.GetComponent<UILabel>();
		}
		if ((bool)building_lbl)
		{
			GameObject global_root = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
			ParkBuildingDataTable building_table = null;
			if ((bool)global_root)
			{
				building_table = global_root.GetComponent<ParkBuildingDataTable>();
			}
			ParkBuildingInfo.BuildingInfo building = null;
			if ((bool)building_table)
			{
				building = Array.Find(building_table.getAllInfo(), (ParkBuildingInfo.BuildingInfo b) => b.ID == minilen.releaseBuildingID);
			}
			if (building != null)
			{
				building_lbl.text = MessageResource.Instance.getMessage(building.NameID);
			}
			else
			{
				building_lbl.gameObject.SetActive(false);
			}
		}
		Transform skill_trans = base.transform.Find("window/Label/skill_label");
		UILabel skill_lbl = null;
		if ((bool)skill_trans)
		{
			skill_lbl = skill_trans.GetComponent<UILabel>();
		}
		if ((bool)skill_lbl)
		{
			skill_lbl.text = getSkillString(minilen);
		}
		Tapjoy.TrackEvent("MinilenLevel", "Level", minilen.index.ToString(), minilen.level.ToString(), 0L);
		GlobalGoogleAnalytics.Instance.LogEvent("Minilen Level", minilen.index.ToString(), minilen.level.ToString(), 1L);
		base.gameObject.SetActive(true);
		Sound.Instance.playSe(Sound.eSe.SE_012_park_discover);
		yield return StartCoroutine(base.open());
		Input.enable = true;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "CloseButton":
			Constant.SoundUtil.PlayCancelSE();
			StartCoroutine(base.close());
			break;
		}
		yield break;
	}

	private string getSkillString(Network.MinilenData minilen)
	{
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		MessageResource instance = MessageResource.Instance;
		int[] sKILL_SCORE_LIST = Constant.SKILL_SCORE_LIST;
		int[] sKILL_SCORE_LIST2 = Constant.SKILL_SCORE_LIST2;
		string empty = string.Empty;
		string empty2 = string.Empty;
		empty2 = ((minilen.level <= 0) ? sKILL_SCORE_LIST[0].ToString() : sKILL_SCORE_LIST[minilen.level - 1].ToString());
		empty = instance.getMessage(8814);
		empty = instance.castCtrlCode(empty, 1, instance.getMessage(8840 + minilen.baseSkill_1));
		empty = instance.castCtrlCode(empty, 2, instance.getMessage(8840 + minilen.baseSkill_2));
		empty = instance.castCtrlCode(empty, 3, empty2);
		if (minilen.specialSkill >= 30)
		{
			Constant.Avatar.eSpecialSkill[] array = new Constant.Avatar.eSpecialSkill[2];
			for (int i = 0; i < 2; i++)
			{
				array[i] = Constant.Avatar.SpecialSkills[minilen.specialSkill - 30, i];
			}
			float[] array2 = new float[2];
			int level = ((minilen.level <= 0) ? 1 : minilen.level);
			for (int j = 0; j < 2; j++)
			{
				array2[j] = @object.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo((int)array[j], level);
				if (array[j] == Constant.Avatar.eSpecialSkill.GuideStretch)
				{
					array2[j] -= 4f;
				}
			}
			if (minilen.specialSkill > 0)
			{
				string message = instance.getMessage(7000 + minilen.specialSkill);
				message = instance.castCtrlCode(message, 1, array2[0].ToString());
				message = instance.castCtrlCode(message, 2, array2[1].ToString());
				return empty + message;
			}
			return empty.Remove(empty.Length - 1, 1);
		}
		float num = 0f;
		num = ((minilen.level <= 0) ? @object.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo(minilen.specialSkill, 1) : @object.GetComponent<AvatarSkillDataTable>().getSkillLevelInfo(minilen.specialSkill, minilen.level));
		if (minilen.specialSkill == 6)
		{
			num -= 4f;
		}
		if (minilen.specialSkill > 0)
		{
			string message2 = instance.getMessage(7000 + minilen.specialSkill);
			message2 = instance.castCtrlCode(message2, 1, num.ToString());
			return empty + message2;
		}
		return empty.Remove(empty.Length - 1, 1);
	}
}
