using System;
using System.Collections;
using UnityEngine;

public class DialogEventPopup : DialogBase
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

	private GameObject listItem_;

	private DialogSendBase.OnSendSuccess cb_;

	private UIButton sendButton_;

	private int bossType;

	private int bossLevel;

	private bool isBossReview;

	private long[] sendIdList_;

	public eResult result_ { get; private set; }

	public override void OnCreate()
	{
		Transform transform = base.transform.Find("window/Inform_Label");
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
		transform = base.transform.Find("window/SendButton");
		if (transform != null)
		{
			sendButton_ = transform.GetComponent<UIButton>();
		}
		listItem_ = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "SendInform_item")) as GameObject;
		Utility.setParent(listItem_, base.transform, false);
		listItem_.SetActive(false);
		setButtonText(eText.Confirm);
		result_ = eResult.Unfinished;
		cb_ = OnSendSuccess;
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

	public void setup(long[] sendIdList, OnDecideButton decideCB, OnCancelButton cancelCB, bool bAutoClose, int eventNo)
	{
		isBossReview = false;
		setButtonText(eText.Confirm);
		setAutoCloseFlg(bAutoClose);
		setDecideCB(decideCB);
		setCancelCB(cancelCB);
		sendIdList_ = sendIdList;
		result_ = eResult.Unfinished;
		switch (eventNo)
		{
		case 1:
			base.transform.Find("window/chara/bg_normal").gameObject.SetActive(true);
			base.transform.Find("window/chara/bg_challenge").gameObject.SetActive(false);
			label_.text = MessageResource.Instance.getMessage(2588);
			break;
		case 2:
			base.transform.Find("window/chara/bg_normal").gameObject.SetActive(false);
			base.transform.Find("window/chara/bg_challenge").gameObject.SetActive(true);
			label_.text = MessageResource.Instance.getMessage(4102);
			break;
		}
		if (eventNo == 1)
		{
			PlayerPrefs.SetInt(Aes.EncryptString("SendDate", Aes.eEncodeType.Percent), DateTime.Now.Month * 100 + DateTime.Now.Day);
			PlayerPrefs.Save();
		}
	}

	public void setupBoss(ActionReward.eType actionRewardType, OnDecideButton decideCB, OnCancelButton cancelCB, bool bAutoClose, int eventNo, int type, int level)
	{
		if (eventNo == 3)
		{
			ActionReward.setupButton(base.transform.Find("window/SendButton"), ActionReward.eType.Boss);
			isBossReview = true;
			bossType = type;
			bossLevel = level;
			setButtonText(eText.Confirm);
			setAutoCloseFlg(bAutoClose);
			setDecideCB(decideCB);
			setCancelCB(cancelCB);
			result_ = eResult.Unfinished;
			base.transform.Find("window/chara/bg_normal").gameObject.SetActive(false);
			base.transform.Find("window/chara/bg_challenge").gameObject.SetActive(true);
			MessageResource instance = MessageResource.Instance;
			string message = MessageResource.Instance.getMessage(3720);
			message = instance.castCtrlCode(message, 1, instance.getMessage(8200 + type));
			message = instance.castCtrlCode(message, 2, level.ToString());
			label_.text = message;
		}
	}

	private void SetActionRewardButton()
	{
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "CloseButtun":
			Constant.SoundUtil.PlayDecideSE();
			result_ = eResult.Cancel;
			if (bAutoClose_)
			{
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			}
			break;
		case "SendButton":
			if (!isBossReview)
			{
				Constant.SoundUtil.PlayDecideSE();
				DialogNortificate inform_dialog = dialogManager_.getDialog(DialogManager.eDialog.EventSendInfo) as DialogNortificate;
				inform_dialog.init(listItem_);
				inform_dialog.setup(sendIdList_);
				yield return StartCoroutine(dialogManager_.openDialog(inform_dialog));
				while (inform_dialog.isOpen())
				{
					yield return 0;
				}
				if (bAutoClose_)
				{
					yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
				}
			}
			else
			{
				Constant.SoundUtil.PlayDecideSE();
				DialogSendBoss dialog = dialogManager_.getDialog(DialogManager.eDialog.SendBoss) as DialogSendBoss;
				dialog.setup(bossType, bossLevel);
				dialog.setCB(cb_);
				yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
			}
			break;
		}
	}

	private void OnSendSuccess()
	{
		sendButton_.transform.Find("Label_reward").GetComponent<UILabel>().color = Color.gray;
		sendButton_.transform.Find("icon_coin").GetComponent<UISprite>().color = Color.gray;
		NGUIUtility.disable(sendButton_, false);
	}

	public void setMessageSize(float size)
	{
		if (label_ != null)
		{
			label_.transform.localScale = new Vector3(size, size, 1f);
			if (sysLabel_ != null)
			{
				sysLabel_.FontSize = (int)(label_.transform.localScale.y * 0.87f);
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
		sysLabel_.gameObject.SetActive(bEnable);
	}
}
