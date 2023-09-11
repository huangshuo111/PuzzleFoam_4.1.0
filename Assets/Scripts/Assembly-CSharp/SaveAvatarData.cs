using System.Collections.Generic;
using Network;
using UnityEngine;

public class SaveAvatarData : MonoBehaviour
{
	private GlobalData.AvatarInfo avatarInfo;

	private GlobalData.GachaInfo gachaInfo;

	public Network.Avatar[] avatarList;

	private List<int> avatarIndexList = new List<int>();

	public void setup()
	{
	}

	public void Init()
	{
	}

	public int LocalGacha()
	{
		return 0;
	}

	public bool isNewAvatar(int resultID)
	{
		return true;
	}

	public void AddNewAvatar(int avatarID)
	{
	}

	public void AvatarLevelup(int avatarID)
	{
	}

	public void AvatarLimitBreak(int avatarID)
	{
	}

	public void AvatarSetWear(int avatarID)
	{
	}

	public void ClearAvatarLocalData()
	{
	}

	public void setCurrentAvatar()
	{
	}

	public Network.Avatar SearchAvatar(int avatarID)
	{
		return null;
	}

	public void AllAvatarComplete()
	{
	}

	public bool isAllLevelMax()
	{
		if (avatarList == null || avatarList.Length <= 0)
		{
			return false;
		}
		Network.Avatar[] array = avatarList;
		foreach (Network.Avatar avatar in array)
		{
			if (avatar.level < 1)
			{
				return false;
			}
			if (avatar.limitover != 1)
			{
				return false;
			}
		}
		return true;
	}

	public bool isAllLevelMax2()
	{
		return true;
	}

	public void AllLevelMax()
	{
	}
}
