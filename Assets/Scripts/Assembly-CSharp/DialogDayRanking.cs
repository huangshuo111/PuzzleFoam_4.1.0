using System.Collections;
using UnityEngine;

public class DialogDayRanking : DialogRanking
{
	private RankingItem selectItem_;

	public UILabel periodLabel_;

	public UILabel daysLabel_;

	private MessageResource msgResource_;

	public override void OnCreate()
	{
		base.OnCreate();
		daysLabel_ = base.transform.Find("window/RemainingDays_Label").GetComponent<UILabel>();
		periodLabel_ = base.transform.Find("window/Period_Label").GetComponent<UILabel>();
		msgResource_ = MessageResource.Instance;
	}

	public override void setup()
	{
		string message = msgResource_.getMessage(13);
		message = msgResource_.castCtrlCode(message, 1, "----");
		message = msgResource_.castCtrlCode(message, 2, "--");
		message = msgResource_.castCtrlCode(message, 3, "--");
		periodLabel_.text = message;
		message = msgResource_.getMessage(14);
		message = msgResource_.castCtrlCode(message, 1, "-");
		message = msgResource_.castCtrlCode(message, 2, "-");
		daysLabel_.text = message;
	}

	public override void ClearLastItem()
	{
		if (reservedClearItemList_ == null || reservedClearItemList_.Count <= 0)
		{
			return;
		}
		foreach (GameObject item in reservedClearItemList_)
		{
			if (item != null)
			{
				Object.Destroy(item);
			}
		}
		reservedClearItemList_.Clear();
	}

	protected override IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "PresentButton":
		{
			Constant.SoundUtil.PlayDecideSE();
			selectItem_ = trigger.transform.parent.GetComponent<RankingItem>();
			string user_name = selectItem_.getUserName();
			DialogCommon presentDialog = dialogManager_.getDialog(DialogManager.eDialog.Present) as DialogCommon;
			string msg2 = msgResource_.getMessage(15);
			msg2 = msgResource_.castCtrlCode(msg2, 1, user_name);
			presentDialog.setup(msg2, OnDecide, null, true);
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(presentDialog));
			break;
		}
		case "BlockButton":
		case "UnblockButton":
			Constant.SoundUtil.PlayDecideSE();
			selectItem_ = trigger.transform.parent.parent.GetComponent<RankingItem>();
			yield return StartCoroutine(selectItem_.SendHeartRecvFlag(dialogManager_));
			break;
		case "Gift":
		{
			Constant.SoundUtil.PlayDecideSE();
			DialogGiftJewelShop dialog = dialogManager_.getDialog(DialogManager.eDialog.GiftJewelShop) as DialogGiftJewelShop;
			selectItem_ = trigger.transform.parent.GetComponent<RankingItem>();
			dialog.TargetUserID = selectItem_.getMid();
			dialog.TargetUserName = selectItem_.getUserName();
			yield return dialogManager_.StartCoroutine(dialog.show());
			Debug.Log("GiftButton Click!!!!!!!");
			break;
		}
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			clearItem();
			bShow_ = false;
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "Invite_Button":
			Constant.SoundUtil.PlayButtonSE();
			yield return dialogManager_.StartCoroutine(openInviteDialog());
			break;
		}
	}

	private IEnumerator OnDecide()
	{
		yield return StartCoroutine(selectItem_.sendHeartMail(dialogManager_));
	}
}
