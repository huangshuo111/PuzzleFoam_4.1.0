using System;
using System.Collections;
using UnityEngine;

public class DialogReview : DialogBase
{
	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "ReviewButton":
		{
			Constant.SoundUtil.PlayDecideSE();
			PlayerPrefs.SetInt(Aes.EncryptString(Constant.PreReviewKey, Aes.eEncodeType.Percent), 1);
			PlayerPrefs.Save();
			yield return null;
			Application.OpenURL("https://play.google.com/store/apps/details?id=jp.naver.SJLGPB");
			while (!partManager_.isLineLogin())
			{
				yield return null;
			}
			while (partManager_.isLineLogin())
			{
				yield return null;
			}
			yield return null;
			yield return null;
			while (partManager_.currentPart == PartManager.ePart.Map && ((Part_Map)partManager_.execPart).bInactive)
			{
				yield return null;
			}
			DialogCommon d = dialogManager_.getDialog(DialogManager.eDialog.NetworkError) as DialogCommon;
			while (d.isOpen() || NetworkMng.Instance.isDownloading())
			{
				yield return null;
			}
			PlayerPrefs.SetInt(Aes.EncryptString(Constant.PreReviewKey, Aes.eEncodeType.Percent), 0);
			PlayerPrefs.Save();
			yield return StartCoroutine(ActionReward.addActionReward(dialogManager_));
			PlayerPrefs.SetString(Aes.EncryptString(Constant.ReviewEndKey, Aes.eEncodeType.Percent), "1");
			PlayerPrefs.Save();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			PlayerPrefs.SetString(Aes.EncryptString(Constant.ReviewCancelTimeKey, Aes.eEncodeType.Percent), Utility.getUnixTime(DateTime.Now).ToString());
			PlayerPrefs.Save();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	public override IEnumerator open()
	{
		MessageResource msgRes = MessageResource.Instance;
		Constant.eMoney RewardType = ActionReward.rewardType();
		int Num = ActionReward.rewardNum();
		UILabel Inform_Label = base.transform.Find("window/Inform_Label").GetComponent<UILabel>();
		string msg4 = msgRes.getMessage(4010);
		switch (RewardType)
		{
		case Constant.eMoney.Jewel:
			msg4 = msgRes.castCtrlCode(msg4, 1, msgRes.getMessage(2569));
			msg4 = msgRes.castCtrlCode(msg4, 2, msgRes.castCtrlCode(msgRes.getMessage(28), 1, Num.ToString("N0")));
			break;
		case Constant.eMoney.Coin:
			msg4 = msgRes.castCtrlCode(msg4, 1, msgRes.getMessage(2568));
			msg4 = msgRes.castCtrlCode(msg4, 2, msgRes.castCtrlCode(msgRes.getMessage(31), 1, Num.ToString("N0")));
			break;
		case Constant.eMoney.Heart:
			msg4 = msgRes.castCtrlCode(msg4, 1, msgRes.getMessage(2570));
			msg4 = msgRes.castCtrlCode(msg4, 2, msgRes.castCtrlCode(msgRes.getMessage(28), 1, Num.ToString("N0")));
			break;
		}
		Inform_Label.text = msg4;
		yield return dialogManager_.StartCoroutine(base.open());
	}

	public bool isCanOpen()
	{
		int num = int.Parse(PlayerPrefs.GetString(Aes.EncryptString(Constant.ReviewEndKey, Aes.eEncodeType.Percent), "0"));
		if (num > 0)
		{
			return false;
		}
		long unixTime = Utility.getUnixTime(DateTime.Now);
		long num2 = long.Parse(PlayerPrefs.GetString(Aes.EncryptString(Constant.ReviewCancelTimeKey, Aes.eEncodeType.Percent), "0"));
		if (unixTime - num2 < Constant.ReviewDialogReopenTime)
		{
			return false;
		}
		return true;
	}
}
