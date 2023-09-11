using System.Collections;
using UnityEngine;

public class DialogClearBonus : DialogBase
{
	public enum eBtn
	{
		Close = 0,
		Decide = 1,
		Max = 2
	}

	public enum eText
	{
		Retry = 0,
		Confirm = 1
	}

	public enum eRewardIcons
	{
		Coin = 0,
		Heart = 1,
		Jewel = 2,
		GachaTicket = 3,
		Max = 4
	}

	public enum eResult
	{
		Unfinished = 0,
		Decide = 1,
		Cancel = 2
	}

	public delegate IEnumerator OnDecideButton();

	public delegate IEnumerator OnCancelButton();

	private bool isCollaboflg;

	private int stagenum;

	private UILabel labelMessage_;

	private UILabel labelNum_;

	private UISysFontLabel sysLabel_;

	private UILabel btnLabel_;

	private UIButton[] buttons_ = new UIButton[2];

	private Transform[] rewardIcons_ = new Transform[4];

	private OnDecideButton decideCB_;

	private OnCancelButton cancelCB_;

	private bool bAutoClose_;

	private Constant.eMoney rewardType_;

	private UISprite charaSprite;

	public eResult result_ { get; private set; }

	public override void OnCreate()
	{
		Transform transform = base.transform.Find("window/Label00");
		if (transform != null)
		{
			labelNum_ = transform.GetComponent<UILabel>();
		}
		transform = base.transform.Find("window/Label01");
		if (transform != null)
		{
			labelMessage_ = transform.GetComponent<UILabel>();
		}
		transform = base.transform.Find("window/Close_Button");
		if (transform != null)
		{
			buttons_[0] = transform.GetComponent<UIButton>();
		}
		transform = base.transform.Find("window/ConfirmButton");
		if (transform != null)
		{
			buttons_[1] = transform.GetComponent<UIButton>();
			btnLabel_ = buttons_[1].transform.Find("Label").GetComponent<UILabel>();
		}
		transform = base.transform.Find("window/jewel_icon");
		if (transform != null)
		{
			rewardIcons_[2] = transform;
		}
		transform = base.transform.Find("window/heart_icon");
		if (transform != null)
		{
			rewardIcons_[1] = transform;
		}
		transform = base.transform.Find("window/coin_icon");
		if (transform != null)
		{
			rewardIcons_[0] = transform;
		}
		transform = base.transform.Find("window/ticket_icon");
		if (transform != null)
		{
			rewardIcons_[3] = transform;
		}
		transform = base.transform.Find("window/Chara/anm1");
		if (transform != null)
		{
			charaSprite = transform.GetComponent<UISprite>();
		}
		setButtonText(eText.Confirm);
		result_ = eResult.Unfinished;
		Utility.createSysLabel(labelMessage_, ref sysLabel_);
	}

	public void setMessage(string msg)
	{
		if (labelMessage_ != null)
		{
			labelMessage_.text = msg;
		}
	}

	public void setNumMessage(string msg)
	{
		if (labelNum_ != null)
		{
			labelNum_.text = msg;
		}
	}

	public void setCharaSprite(int eventNo)
	{
		if (charaSprite != null)
		{
			if (eventNo == 11)
			{
				isCollaboflg = true;
			}
			else
			{
				charaSprite.spriteName = "area_clear";
			}
		}
	}

	public void setCharaSpriteCollabo(int eventNo)
	{
		stagenum %= eventNo * 10000;
		if (stagenum < 11)
		{
			charaSprite.spriteName = "areaCollaboration_clear_03";
		}
		else
		{
			charaSprite.spriteName = "areaCollaboration_clear_04";
		}
	}

	public void setDecideCB(OnDecideButton cb)
	{
		decideCB_ = cb;
	}

	public void setCancelCB(OnCancelButton cb)
	{
		cancelCB_ = cb;
	}

	public void setAutoCloseFlg(bool bFlg)
	{
		bAutoClose_ = bFlg;
	}

	public void setButtonText(eText textType)
	{
		if (!(btnLabel_ == null))
		{
			string text = string.Empty;
			switch (textType)
			{
			case eText.Confirm:
				text = MessageResource.Instance.getMessage(2401);
				break;
			case eText.Retry:
				text = MessageResource.Instance.getMessage(2408);
				break;
			}
			btnLabel_.text = text;
		}
	}

	public void setButtonActive(eBtn btn, bool bActive)
	{
		if (!(buttons_[(int)btn] == null))
		{
			buttons_[(int)btn].gameObject.SetActive(bActive);
		}
	}

	public override void OnClose()
	{
		for (int i = 0; i < 2; i++)
		{
			setButtonActive((eBtn)i, true);
		}
		setButtonText(eText.Confirm);
	}

	public void setup(int msgID, OnDecideButton decideCB, OnCancelButton cancelCB, bool bAutoClose, Constant.eMoney rewardType, int rewardNum, int eventNo)
	{
		setup(MessageResource.Instance.getMessage(msgID), decideCB, cancelCB, bAutoClose, rewardType, rewardNum, eventNo);
	}

	public void setup(string msg, OnDecideButton decideCB, OnCancelButton cancelCB, bool bAutoClose, Constant.eMoney rewardType, int rewardNum, int eventNo)
	{
		setup(decideCB, cancelCB, bAutoClose, rewardType, rewardNum, eventNo);
		setMessage(msg);
	}

	public void setup(OnDecideButton decideCB, OnCancelButton cancelCB, bool bAutoClose, Constant.eMoney rewardType, int rewardNum, int eventNo)
	{
		setButtonText(eText.Confirm);
		setAutoCloseFlg(bAutoClose);
		setDecideCB(decideCB);
		setCancelCB(cancelCB);
		result_ = eResult.Unfinished;
		setNumMessage(rewardNum.ToString());
		setCharaSprite(eventNo);
		if (isCollaboflg)
		{
			setCharaSpriteCollabo(eventNo);
		}
		rewardType_ = rewardType;
	}

	public override IEnumerator open()
	{
		Sound.Instance.playSe(Sound.eSe.SE_108_Yay);
		Sound.Instance.playSe(Sound.eSe.SE_113_Clap);
		switch (rewardType_)
		{
		case Constant.eMoney.Coin:
			if ((bool)rewardIcons_[1])
			{
				rewardIcons_[1].gameObject.SetActive(false);
			}
			if ((bool)rewardIcons_[2])
			{
				rewardIcons_[2].gameObject.SetActive(false);
			}
			if ((bool)rewardIcons_[3])
			{
				rewardIcons_[3].gameObject.SetActive(false);
			}
			break;
		case Constant.eMoney.Heart:
			if ((bool)rewardIcons_[0])
			{
				rewardIcons_[0].gameObject.SetActive(false);
			}
			if ((bool)rewardIcons_[2])
			{
				rewardIcons_[2].gameObject.SetActive(false);
			}
			if ((bool)rewardIcons_[3])
			{
				rewardIcons_[3].gameObject.SetActive(false);
			}
			break;
		case Constant.eMoney.Jewel:
			if ((bool)rewardIcons_[1])
			{
				rewardIcons_[1].gameObject.SetActive(false);
			}
			if ((bool)rewardIcons_[0])
			{
				rewardIcons_[0].gameObject.SetActive(false);
			}
			if ((bool)rewardIcons_[3])
			{
				rewardIcons_[3].gameObject.SetActive(false);
			}
			break;
		case Constant.eMoney.GachaTicket:
			if ((bool)rewardIcons_[1])
			{
				rewardIcons_[1].gameObject.SetActive(false);
			}
			if ((bool)rewardIcons_[0])
			{
				rewardIcons_[0].gameObject.SetActive(false);
			}
			if ((bool)rewardIcons_[2])
			{
				rewardIcons_[2].gameObject.SetActive(false);
			}
			break;
		}
		yield return dialogManager_.StartCoroutine(base.open());
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			result_ = eResult.Cancel;
			if (cancelCB_ != null)
			{
				yield return dialogManager_.StartCoroutine(cancelCB_());
			}
			if (bAutoClose_)
			{
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			}
			break;
		case "ConfirmButton":
			Constant.SoundUtil.PlayDecideSE();
			result_ = eResult.Decide;
			if (decideCB_ != null)
			{
				yield return dialogManager_.StartCoroutine(decideCB_());
			}
			if (bAutoClose_)
			{
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			}
			break;
		}
	}

	public void setMessageSize(float size)
	{
		if (labelMessage_ != null)
		{
			labelMessage_.transform.localScale = new Vector3(size, size, 1f);
			if (sysLabel_ != null)
			{
				sysLabel_.FontSize = (int)(labelMessage_.transform.localScale.y * 0.87f);
			}
		}
	}

	private void LateUpdate()
	{
		if (sysLabel_ != null && labelMessage_ != null)
		{
			sysLabel_.Text = labelMessage_.text;
		}
	}

	public void sysLabelEnable(bool bEnable)
	{
		labelMessage_.gameObject.SetActive(!bEnable);
		sysLabel_.gameObject.SetActive(bEnable);
	}
}
