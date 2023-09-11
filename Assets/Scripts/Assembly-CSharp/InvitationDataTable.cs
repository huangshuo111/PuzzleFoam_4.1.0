using UnityEngine;

public class InvitationDataTable : MonoBehaviour
{
	public InvitationInfo info_;

	private bool bLoaded_;

	public void load()
	{
		if (!bLoaded_)
		{
			TextAsset textAsset = Resources.Load("Parameter/invitation_info", typeof(TextAsset)) as TextAsset;
			info_ = Xml.DeserializeObject<InvitationInfo>(textAsset.text) as InvitationInfo;
			bLoaded_ = true;
		}
	}

	public bool getReward(ref Constant.Reward reward, ref Constant.Reward reward2)
	{
		if (reward == null)
		{
			return false;
		}
		reward.Num = info_.Reward;
		reward.RewardType = (Constant.eMoney)info_.RewardType;
		reward2.Num = info_.Reward2;
		reward2.RewardType = (Constant.eMoney)info_.RewardType2;
		return true;
	}

	public bool isLoaded()
	{
		return bLoaded_;
	}
}
