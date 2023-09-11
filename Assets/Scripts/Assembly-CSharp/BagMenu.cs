using UnityEngine;

public class BagMenu : MonoBehaviour
{
	[SerializeField]
	private GameObject MailNoticeIcon;

	[SerializeField]
	private UILabel MailNoticeLabel;

	[SerializeField]
	private UILabel InviteRewardLabel;

	private int mailNum_;

	public void setMailNum(int num)
	{
		mailNum_ = num;
		if (mailNum_ > 0)
		{
			MailNoticeIcon.SetActive(true);
			MailNoticeLabel.text = mailNum_.ToString();
		}
		else
		{
			MailNoticeIcon.SetActive(false);
		}
	}

	public void setInviteRewardPopup(Constant.Reward reward)
	{
		if (!(InviteRewardLabel == null))
		{
			MessageResource instance = MessageResource.Instance;
			InviteRewardLabel.text = instance.castCtrlCode(instance.getMessage(45), 1, reward.Num.ToString("N0"));
		}
	}

	public void OnEnable()
	{
	}
}
