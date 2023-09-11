using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogSendBase : DialogBase
{
	public class SendAppLinkMessageCB
	{
		public int result_ = -1;

		public void OnMessageSend(int result)
		{
			result_ = result;
		}
	}

	public enum eTimeLineType
	{
		HighScore = 1,
		LevelUp = 2,
		AreaClear = 3,
		Treasure = 4,
		BonusCoin = 5,
		Boss = 6
	}

	public class RegisterLineTimelineActivityCB
	{
	}

	public delegate void OnSendSuccess();

	private OnSendSuccess successCB_;

	protected UILabel label_;

	private UISysFontLabel sysLabel_;

	private UIButton sendButton_;

	protected DialogCommon errorDialog_;

	protected DialogCommon commonDialog_;

	public static bool internalError;

	protected SendAppLinkMessageCB sendMessageCB_;

	protected string leveltext_;

	protected string scoretext_;

	protected string stageNo_;

	protected string islandName_;

	private RegisterLineTimelineActivityCB registerTimelineCB_;

	protected eSNSResultCode GetMessageResult
	{
		get
		{
			return (eSNSResultCode)sendMessageCB_.result_;
		}
	}

	public override void OnCreate()
	{
		label_ = base.transform.Find("window/Label").GetComponent<UILabel>();
		errorDialog_ = dialogManager_.getDialog(DialogManager.eDialog.NetworkError) as DialogCommon;
		commonDialog_ = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		Utility.createSysLabel(label_, ref sysLabel_);
	}

	public void callCB()
	{
		if (successCB_ != null)
		{
			successCB_();
		}
	}

	public void setCB(OnSendSuccess cb)
	{
		successCB_ = cb;
	}

	protected void setMessage(string msg)
	{
		label_.text = msg;
	}

	protected void enableSendButton()
	{
		NGUIUtility.enable(sendButton_, false);
	}

	protected void disableSendButton()
	{
		NGUIUtility.enable(sendButton_, false);
	}

	protected abstract IEnumerator send();

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "SendButtun":
			Constant.SoundUtil.PlayDecideSE();
			yield return dialogManager_.StartCoroutine(send());
			break;
		}
	}

	private IEnumerator OnResend()
	{
		if (errorDialog_.isOpen())
		{
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(errorDialog_));
		}
		yield return dialogManager_.StartCoroutine(send());
	}

	private IEnumerator OnCancel()
	{
		yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(errorDialog_));
	}

	protected IEnumerator openErrorDialog(string msg)
	{
		errorDialog_.setup(msg, OnResend, OnCancel, false);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(errorDialog_));
	}

	protected IEnumerator closeErrorDialog()
	{
		yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(errorDialog_));
	}

	protected IEnumerator openCommonDialog(string msg)
	{
		commonDialog_.setButtonActive(DialogCommon.eBtn.Decide, false);
		commonDialog_.setMessage(msg);
		commonDialog_.setAutoCloseFlg(true);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(commonDialog_));
	}

	protected IEnumerator closeCommonDialog()
	{
		yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(commonDialog_));
	}

	protected IEnumerator sendMessage(string mid, string[] msg)
	{
		List<string> receiverMidList = new List<string>(1) { mid };
		sendMessageCB_ = new SendAppLinkMessageCB();
		SNSCore.Instance.SendMessage(msg[0], mid, sendMessageCB_.OnMessageSend);
		while (sendMessageCB_.result_ == -1)
		{
			yield return null;
		}
	}

	protected IEnumerator sendRankChangeMessage(string mid, string stage)
	{
		List<string> receiverMidList = new List<string>(1) { mid };
		sendMessageCB_ = new SendAppLinkMessageCB();
		SNSCore.Instance.KakaoSendMessageRankChange(mid, stage, sendMessageCB_.OnMessageSend);
		while (sendMessageCB_.result_ == -1)
		{
			yield return null;
		}
	}

	protected bool isSuccessSendMessage()
	{
		if (sendMessageCB_ == null)
		{
			return false;
		}
		if (sendMessageCB_.result_ == -1)
		{
			return false;
		}
		return sendMessageCB_.result_ == 0;
	}

	public IEnumerator registerTimeline(long templateId, string replace, eTimeLineType tlType)
	{
		yield break;
	}

	public bool isSuccessRegisterTimeline()
	{
		return true;
	}

	private void LateUpdate()
	{
		if (sysLabel_ != null && label_ != null)
		{
			sysLabel_.Text = label_.text;
		}
	}
}
