using System;
using UnityEngine;

public class PlayerItemBase : MonoBehaviour
{
	[SerializeField]
	protected UILabel UserNameLabel;

	[SerializeField]
	protected GameObject ActiveButton;

	[SerializeField]
	protected GameObject DeactiveButton;

	[SerializeField]
	protected PlayerIcon Icon;

	[SerializeField]
	protected UILabel DeactiveTimeLabel;

	[SerializeField]
	protected GameObject CoinBalloon;

	[SerializeField]
	protected GameObject MessageBlock;

	[SerializeField]
	protected GameObject BlockButton;

	[SerializeField]
	protected GameObject UnBlockButton;

	[SerializeField]
	protected GameObject Checkbox;

	protected GameObject CheckMark_;

	protected bool IsMine;

	protected UISysFontLabel UserNameSysFontLabel;

	protected string userName_ = string.Empty;

	protected long id_;

	protected string mid_ = string.Empty;

	protected long prev_time = -1L;

	private bool bLiveCoinBalloon = true;

	private Transform coinBalloon;

	private bool bFinish_;

	public float checkTopY;

	public float checkBottomY;

	public bool IsMessageBlock { get; protected set; }

	public bool IsChecked { get; set; }

	public bool isFinish
	{
		get
		{
			return bFinish_;
		}
	}

	private void Awake()
	{
		IsMessageBlock = false;
		coinBalloon = base.transform.Find("privilege_icon");
		if (Checkbox != null)
		{
			Checkbox.SetActive(false);
		}
		if (ActiveButton != null)
		{
			UILabel componentInChildren = ActiveButton.GetComponentInChildren<UILabel>();
			if (!componentInChildren.name.Contains("+_Label"))
			{
				componentInChildren.text = MessageResource.Instance.getMessage(2405);
			}
		}
	}

	private void Update()
	{
		if (bLiveCoinBalloon && coinBalloon != null)
		{
			coinBalloon.gameObject.SetActive(checkActivePosition(base.transform.position));
		}
	}

	private bool checkActivePosition(Vector3 pos)
	{
		return pos.y < checkTopY && pos.y > checkBottomY;
	}

	public virtual void setup(string userName, long id, string mid)
	{
		if (UserNameLabel != null)
		{
			UserNameLabel.text = userName;
			userName_ = Constant.UserName.ReplaceOverStr(UserNameLabel);
			UserNameLabel.text = userName_;
		}
		id_ = id;
		mid_ = mid;
		setState(false);
		Utility.createSysLabel(UserNameLabel, ref UserNameSysFontLabel);
		UnityEngine.Object.Destroy(GetComponent<UIPanel>());
	}

	public void setState(bool bFinish)
	{
		if ((bool)DeactiveButton)
		{
			DeactiveButton.SetActive(bFinish);
		}
		if ((bool)ActiveButton)
		{
			ActiveButton.SetActive(!bFinish);
		}
		setCoinBalloon(!bFinish);
		bFinish_ = bFinish;
		if (bFinish_)
		{
			return;
		}
		if (IsMine || IsMessageBlock)
		{
			if ((bool)ActiveButton)
			{
				ActiveButton.SetActive(false);
			}
			setMessageBlockButtonState(IsMessageBlock);
		}
		else if ((bool)MessageBlock)
		{
			MessageBlock.SetActive(false);
		}
	}

	private void LateUpdate()
	{
		if (UserNameSysFontLabel != null && UserNameLabel != null)
		{
			UserNameSysFontLabel.Text = UserNameLabel.text;
		}
		if (bFinish_ && DeactiveTimeLabel != null)
		{
			long num = Constant.ResendTime - (Utility.getUnixTime(DateTime.Now) - prev_time);
			if (num >= 0)
			{
				int num2 = (int)num / 3600;
				int num3 = (int)(num - num2 * 3600) / 60;
				int num4 = (int)(num - num2 * 3600 - num3 * 60);
				string text = num2.ToString("00") + ":" + num3.ToString("00") + ":" + num4.ToString("00");
				DeactiveTimeLabel.text = text;
			}
			else
			{
				setState(false);
			}
		}
	}

	public void setActiveButton(bool bActive)
	{
		if ((bool)DeactiveButton)
		{
			DeactiveButton.SetActive(bActive);
		}
		if ((bool)ActiveButton)
		{
			ActiveButton.SetActive(bActive);
		}
		if ((bool)MessageBlock)
		{
			MessageBlock.SetActive(bActive);
		}
	}

	public void setMessageBlockButton(bool bMine, bool bBlock)
	{
		IsMessageBlock = bBlock;
		IsMine = bMine;
		setState(bFinish_);
	}

	protected void setMessageBlockButtonState(bool bBlock)
	{
		if (!MessageBlock)
		{
			return;
		}
		MessageBlock.SetActive(true);
		BoxCollider boxCollider = null;
		if ((bool)BlockButton)
		{
			BlockButton.SetActive(!bBlock);
			boxCollider = BlockButton.GetComponent<BoxCollider>();
			if ((bool)boxCollider && !bBlock)
			{
				boxCollider.enabled = IsMine;
			}
		}
		if ((bool)UnBlockButton)
		{
			UnBlockButton.SetActive(bBlock);
			UnBlockButton.GetComponent<BoxCollider>().enabled = IsMine;
			boxCollider = UnBlockButton.GetComponent<BoxCollider>();
			if ((bool)boxCollider && bBlock)
			{
				boxCollider.enabled = IsMine;
			}
		}
	}

	public void setCoinBalloon(bool bActive)
	{
		if (CoinBalloon != null && bLiveCoinBalloon)
		{
			bLiveCoinBalloon = bActive;
			if (bActive && checkActivePosition(base.transform.position))
			{
				CoinBalloon.SetActive(true);
			}
			else if (!bActive)
			{
				CoinBalloon.SetActive(false);
			}
		}
	}

	public void setPrevTime(long time)
	{
		prev_time = time;
	}

	public PlayerIcon getIcon()
	{
		return Icon;
	}

	public string getUserName()
	{
		return userName_;
	}

	public long getID()
	{
		return id_;
	}

	public string getMid()
	{
		return mid_;
	}

	public void changeActiveButton(GameObject obj)
	{
		ActiveButton = obj;
	}

	public void SetCheckPosY(float topY, float bottomY)
	{
		checkTopY = topY;
		checkBottomY = bottomY;
	}

	public bool SetCheckbox(bool check)
	{
		if (CheckMark_ != null)
		{
			IsChecked = check;
			CheckMark_.SetActive(check);
			return true;
		}
		return false;
	}
}
