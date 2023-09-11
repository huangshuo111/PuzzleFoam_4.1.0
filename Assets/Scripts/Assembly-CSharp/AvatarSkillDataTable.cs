using System.Collections.Generic;
using UnityEngine;

public class AvatarSkillDataTable : MonoBehaviour
{
	private SkillInfo info_;

	private bool bLoaded_;

	public void load()
	{
		if (!bLoaded_)
		{
			TextAsset textAsset = Resources.Load("Parameter/skill_info", typeof(TextAsset)) as TextAsset;
			info_ = Xml.DeserializeObject<SkillInfo>(textAsset.text) as SkillInfo;
			bLoaded_ = true;
		}
	}

	public bool getSkill(int skillTypeNum, int idx, ref List<Constant.Skill> skills)
	{
		idx--;
		if (skillTypeNum >= 30)
		{
			int num = skillTypeNum - 30;
			for (int i = 0; i < 2; i++)
			{
				Constant.Skill skill = new Constant.Skill();
				skill.SkillType = Constant.Avatar.SpecialSkills[num, i];
				skill.Level = info_.SkillList[(int)skill.SkillType].LevelInfos[idx].Level;
				skill.LevelEffect = info_.SkillList[(int)skill.SkillType].LevelInfos[idx].LevelEffect;
				skills.Add(skill);
			}
		}
		else
		{
			Constant.Skill skill = new Constant.Skill();
			skill.SkillType = (Constant.Avatar.eSpecialSkill)skillTypeNum;
			skill.Level = info_.SkillList[skillTypeNum].LevelInfos[idx].Level;
			skill.LevelEffect = info_.SkillList[skillTypeNum].LevelInfos[idx].LevelEffect;
			skills.Add(skill);
		}
		return true;
	}

	public bool isLoaded()
	{
		return bLoaded_;
	}

	public float getSkillLevelInfo(int skill, int level)
	{
		if (info_ != null)
		{
			return info_.SkillList[skill].LevelInfos[level - 1].LevelEffect;
		}
		return -1f;
	}
}
