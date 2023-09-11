using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class DialogParkNiceDetail : DialogBase
{
	private enum eMessageID
	{
		BuildingCountBetween1And10 = 9129,
		BuildingCountBetween11And20 = 9130,
		RareBuildingCountBetween1And5 = 9131,
		BuildingCountBetween21And30 = 9132,
		RareBuildingCountBetween6And10 = 9133
	}

	private const int NICE_COUNT_MAX = 9999999;

	public override void OnOpen()
	{
		dialogManager_.addActiveDialogList(DialogManager.eDialog.ParkNiceDetail);
	}

	public override void OnClose()
	{
		dialogManager_.removeActiveDialogList(DialogManager.eDialog.ParkNiceDetail);
	}

	public IEnumerator Setup(bool isAfterHistory = true)
	{
		UILabel totalNiceCountLabel = base.transform.Find("window/total_nice_count/count_label").GetComponent<UILabel>();
		UILabel currentMonthNiceCountLabel = base.transform.Find("window/current_month_nice_count/count_label").GetComponent<UILabel>();
		UILabel playerNiceCountLabel = base.transform.Find("window/player_nice_count/count_label").GetComponent<UILabel>();
		GameData gameData = GlobalData.Instance.getGameData();
		if (gameData != null)
		{
			totalNiceCountLabel.text = Mathf.Clamp(gameData.giveNiceTotalCount, 0, 9999999).ToString();
			currentMonthNiceCountLabel.text = Mathf.Clamp(gameData.giveNiceMonthlyCount, 0, 9999999).ToString();
			playerNiceCountLabel.text = Mathf.Clamp(gameData.tookNiceTotalCount, 0, 9999999).ToString();
		}
		else
		{
			totalNiceCountLabel.text = "10";
			currentMonthNiceCountLabel.text = "1";
			playerNiceCountLabel.text = "5";
		}
		setCommentMessage();
		yield break;
	}

	public void setCommentMessage()
	{
		ParkObjectManager instance = ParkObjectManager.Instance;
		int num = instance.buildingCount + instance.fenceCount;
		int rareBuildingCount = instance.getRareBuildingCount();
		List<eMessageID> list = new List<eMessageID>();
		if (num > 20)
		{
			list.Add(eMessageID.BuildingCountBetween21And30);
		}
		else if (num > 10)
		{
			list.Add(eMessageID.BuildingCountBetween11And20);
		}
		else if (num > 0)
		{
			list.Add(eMessageID.BuildingCountBetween1And10);
		}
		if (rareBuildingCount > 5)
		{
			list.Add(eMessageID.RareBuildingCountBetween6And10);
		}
		else if (rareBuildingCount >= 1)
		{
			list.Add(eMessageID.RareBuildingCountBetween1And5);
		}
		if (list.Count == 0)
		{
			list.Add(eMessageID.BuildingCountBetween1And10);
		}
		int messageID = (int)list[Random.Range(0, list.Count)];
		UILabel component = base.transform.Find("window/comment/balloon/comment_label").GetComponent<UILabel>();
		component.text = MessageResource.Instance.getMessage(messageID);
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "NiceHistory_Button":
		{
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(close());
			DialogParkNiceHistoryList dialog = dialogManager_.getDialog(DialogManager.eDialog.ParkNiceHistoryList) as DialogParkNiceHistoryList;
			dialogManager_.StartCoroutine(dialog.show());
			break;
		}
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			StartCoroutine(close());
			break;
		}
		yield break;
	}
}
