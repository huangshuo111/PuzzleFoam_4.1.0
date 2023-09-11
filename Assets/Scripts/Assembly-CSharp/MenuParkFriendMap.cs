using System;
using System.Collections;
using LitJson;
using Network;
using UnityEngine;

public class MenuParkFriendMap : MonoBehaviour
{
	[SerializeField]
	private UILabel nameLabel_;

	[SerializeField]
	private UILabel minilenCountLabel_;

	[SerializeField]
	private UIButton niceButton_;

	private DialogManager dialogManager_;

	private bool isDummy_;

	private long friendID_;

	private string userName_;

	private string mid_;

	private bool takableNice_;

	private DialogCommon confirmDialog_;

	public void init(DialogManager dialogManager, Transform parent)
	{
		dialogManager_ = dialogManager;
		base.transform.SetParent(parent, false);
		NGUIUtility.setupButton(base.gameObject, base.gameObject, true);
		confirmDialog_ = dialogManager_.getDialog(DialogManager.eDialog.ParkSendNiceConfirm) as DialogCommon;
	}

	public void setup(DialogParkNiceHistoryList.NiceHistoryListData userData, bool takableNice)
	{
		friendID_ = userData.user.ID;
		mid_ = userData.user.Mid;
		isDummy_ = userData.user.IsDummy;
		nameLabel_.text = userData.user.UserName;
		string text = Constant.UserName.ReplaceOverStr(nameLabel_);
		nameLabel_.text = text;
		userName_ = text;
		minilenCountLabel_.text = userData.user.TotalMinilenNum.ToString();
		niceButton_.setEnable(takableNice);
		takableNice_ = takableNice;
	}

	public void setActive(bool active)
	{
		base.gameObject.SetActive(active);
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "nice_button":
			Constant.SoundUtil.PlayDecideSE();
			if (takableNice_)
			{
				string msg2 = MessageResource.Instance.getMessage(9161);
				msg2 = MessageResource.Instance.castCtrlCode(msg2, 1, userName_);
				confirmDialog_.setup(msg2, OnDecide, OnCancel, false);
				yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(confirmDialog_));
			}
			break;
		case "back_button":
		{
			Constant.SoundUtil.PlayCancelSE();
			SaveParkData saveData = SaveData.Instance.getParkData();
			yield return dialogManager_.StartCoroutine(ParkObjectManager.Instance.createParkMapForPlayer(saveData.roadID, saveData.areaReleasedCount, saveData.buildings.ToArray()));
			break;
		}
		case "friend_button":
			if (dialogManager_ != null)
			{
				Constant.SoundUtil.PlayDecideSE();
				DialogParkFriendList dialog = dialogManager_.getDialog(DialogManager.eDialog.ParkFriendList) as DialogParkFriendList;
				StartCoroutine(dialog.show(friendID_));
			}
			break;
		}
	}

	private IEnumerator OnDecide()
	{
		bool sendSucceed = true;
		yield return StartCoroutine(SendNice(friendID_, delegate(bool ret)
		{
			sendSucceed = ret;
		}));
		if (sendSucceed)
		{
			takableNice_ = false;
			niceButton_.setEnable(takableNice_);
		}
		if (confirmDialog_.isOpen())
		{
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(confirmDialog_));
		}
	}

	private IEnumerator OnCancel()
	{
		yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(confirmDialog_));
	}

	private IEnumerator SendNice(long friendID, Action<bool> result)
	{
		result(false);
		NetworkMng netManager = NetworkMng.Instance;
		netManager.setup(new Hashtable { 
		{
			"memberNo",
			friendID.ToString()
		} });
		yield return StartCoroutine(netManager.download(API.ParkSendNice, true, true));
		if (netManager.getStatus() != NetworkMng.eStatus.Success)
		{
			yield return StartCoroutine(netManager.showIcon(false));
			yield break;
		}
		result(true);
		WWW www = netManager.getWWW();
		FriendParkData friendData = JsonMapper.ToObject<FriendParkData>(www.text);
		GameData game_data = GlobalData.Instance.getGameData();
		game_data.bonusJewel = friendData.bonusJewel;
		game_data.buyJewel = friendData.buyJewel;
		game_data.treasureboxNum = friendData.treasureboxNum;
		game_data.heart = friendData.heart;
		game_data.coin = friendData.coin;
		game_data.exp = friendData.exp;
		game_data.level = friendData.level;
		game_data.allPlayCount = friendData.allPlayCount;
		game_data.allClearCount = friendData.allClearCount;
		game_data.allStarSum = friendData.allStarSum;
		game_data.allStageScoreSum = friendData.allStageScoreSum;
		game_data.userItemList = friendData.userItemList;
		game_data.minilenCount = friendData.minilenCount;
		game_data.minilenTotalCount = friendData.minilenTotalCount;
		game_data.giveNiceTotalCount = friendData.giveNiceTotalCount;
		game_data.giveNiceMonthlyCount = friendData.giveNiceMonthlyCount;
		game_data.tookNiceTotalCount = friendData.tookNiceTotalCount;
		game_data.isParkDailyReward = friendData.isParkDailyReward;
		if (!isDummy_ && SaveData.Instance.getSystemData().getOptionData().getFlag(SaveOptionData.eFlag.PushNotice))
		{
			yield return StartCoroutine(SendNiceNotice());
		}
	}

	private IEnumerator SendNiceNotice()
	{
		yield break;
	}
}
