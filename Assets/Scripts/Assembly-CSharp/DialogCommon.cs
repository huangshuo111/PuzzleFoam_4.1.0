using System.Collections;
using UnityEngine;

public class DialogCommon : DialogBase
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

	public enum eResult
	{
		Unfinished = 0,
		Decide = 1,
		Cancel = 2
	}

	public delegate IEnumerator OnDecideButton();

	public delegate IEnumerator OnCancelButton();

	private UILabel label_;

	private UISysFontLabel sysLabel_;

	private UILabel btnLabel_;

	private UIButton[] buttons_ = new UIButton[2];

	private OnDecideButton decideCB_;

	private OnCancelButton cancelCB_;

	private bool bAutoClose_;

	public eResult result_ { get; private set; }

	public override void OnCreate()
	{
		Transform transform = base.transform.Find("window/Dialog_Label");
		if (transform != null)
		{
			label_ = transform.GetComponent<UILabel>();
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
		setButtonText(eText.Confirm);
		result_ = eResult.Unfinished;
		Utility.createSysLabel(label_, ref sysLabel_);
	}

	public void setMessage(string msg)
	{
		if (label_ != null)
		{
			label_.text = msg;
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

	public void setup(int msgID, OnDecideButton decideCB, OnCancelButton cancelCB, bool bAutoClose)
	{
		setup(MessageResource.Instance.getMessage(msgID), decideCB, cancelCB, bAutoClose);
	}

	public void setup(string msg, OnDecideButton decideCB, OnCancelButton cancelCB, bool bAutoClose)
	{
		setup(decideCB, cancelCB, bAutoClose);
		setMessage(msg);
	}

	public void setup(OnDecideButton decideCB, OnCancelButton cancelCB, bool bAutoClose)
	{
		setButtonText(eText.Confirm);
		setAutoCloseFlg(bAutoClose);
		setDecideCB(decideCB);
		setCancelCB(cancelCB);
		result_ = eResult.Unfinished;
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
		sysLabelEnable(true);
	}

	public void setMessageSize(float size)
	{
		if (label_ != null)
		{
			label_.transform.localScale = new Vector3(size, size, 1f);
			if (sysLabel_ != null)
			{
				sysLabel_.FontSize = (int)(label_.transform.localScale.y * 0.8f);
			}
		}
	}

	private void LateUpdate()
	{
		if (sysLabel_ != null && label_ != null)
		{
			sysLabel_.Text = label_.text;
		}
	}

	public void sysLabelEnable(bool bEnable)
	{
		label_.gameObject.SetActive(!bEnable);
		if (sysLabel_ != null)
		{
			sysLabel_.gameObject.SetActive(bEnable);
		}
	}
}
