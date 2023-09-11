using UnityEngine;

public class MailItem : PlayerItemBase
{
	public void setMessage(int msgNum)
	{
		UserNameLabel.text = MessageResource.Instance.getMessage(msgNum);
	}

	public override void setup(string userName, long id, string mid)
	{
		string text = UserNameLabel.text;
		UserNameLabel.text = userName;
		string message = Constant.UserName.ReplaceOverStr(UserNameLabel);
		text = MessageResource.Instance.castCtrlCode(text, 1, message);
		UserNameLabel.text = text;
		Utility.createSysLabel(UserNameLabel, ref UserNameSysFontLabel);
		Object.Destroy(GetComponent<UIPanel>());
	}

	public void setSupportMessage(int mailType, int count, int itemType, string message, string username = "")
	{
		MessageResource instance = MessageResource.Instance;
		string text;
		if (message == null || message == string.Empty)
		{
			text = instance.getMessage(2572);
			switch (mailType)
			{
			case 3:
				text = instance.castCtrlCode(text, 1, instance.getMessage(3737));
				text = instance.castCtrlCode(text, 2, instance.castCtrlCode(instance.getMessage(28), 1, count.ToString("N0")));
				break;
			case 4:
				text = instance.castCtrlCode(text, 1, instance.getMessage(3736));
				text = instance.castCtrlCode(text, 2, instance.castCtrlCode(instance.getMessage(31), 1, count.ToString("N0")));
				break;
			case 5:
				text = instance.castCtrlCode(text, 1, instance.getMessage(3738));
				text = instance.castCtrlCode(text, 2, instance.castCtrlCode(instance.getMessage(28), 1, count.ToString("N0")));
				break;
			case 6:
				text = instance.castCtrlCode(text, 1, instance.getMessage(1100 + itemType - 1));
				text = instance.castCtrlCode(text, 2, instance.castCtrlCode(instance.getMessage(28), 1, count.ToString("N0")));
				break;
			case 101:
				text = instance.getMessage(500020);
				text = instance.castCtrlCode(text, 1, username);
				text = instance.castCtrlCode(text, 2, count.ToString("N0"));
				break;
			case 102:
				text = instance.getMessage(500019);
				text = instance.castCtrlCode(text, 1, username);
				text = instance.castCtrlCode(text, 2, count.ToString("N0"));
				break;
			}
		}
		else
		{
			text = instance.AddLine(message);
			text = instance.castCtrlCode(text, 1, count.ToString("N0"));
		}
		UserNameLabel.text = text;
		Utility.createSysLabel(UserNameLabel, ref UserNameSysFontLabel);
		Object.Destroy(GetComponent<UIPanel>());
	}

	public void setParkRewardMessage(int mailType, int count, int itemType, string message)
	{
		MessageResource instance = MessageResource.Instance;
		string text = instance.getMessage(9167);
		if (!string.IsNullOrEmpty(message) && message.Contains("visited"))
		{
			text = instance.getMessage(9177);
		}
		switch (mailType)
		{
		case 23:
			text = instance.castCtrlCode(text, 1, instance.getMessage(3736));
			text = instance.castCtrlCode(text, 2, instance.castCtrlCode(instance.getMessage(31), 1, count.ToString("N0")));
			break;
		case 24:
			text = instance.castCtrlCode(text, 1, instance.getMessage(3738));
			text = instance.castCtrlCode(text, 2, instance.castCtrlCode(instance.getMessage(28), 1, count.ToString("N0")));
			break;
		case 25:
			text = instance.castCtrlCode(text, 1, instance.getMessage(3737));
			text = instance.castCtrlCode(text, 2, instance.castCtrlCode(instance.getMessage(28), 1, count.ToString("N0")));
			break;
		}
		UserNameLabel.text = text;
		Utility.createSysLabel(UserNameLabel, ref UserNameSysFontLabel);
		Object.Destroy(GetComponent<UIPanel>());
	}
}
