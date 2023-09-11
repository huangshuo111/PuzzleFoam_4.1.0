using System;
using System.Collections;
using System.Collections.Generic;
using Bridge;
using LitJson;
using Network;
using UnityEngine;

public class DialogParkRewardCheckList : DialogScrollListBase
{
	public class RewardData
	{
		public int type;

		public int id;

		public int num;
	}

	private bool _openState;

	public bool openState
	{
		get
		{
			return _openState;
		}
	}

	public override void OnCreate()
	{
		base.OnCreate();
	}

	public override void OnOpen()
	{
		dialogManager_.addActiveDialogList(DialogManager.eDialog.ParkRewardCheckList);
	}

	public override void OnClose()
	{
		dialogManager_.removeActiveDialogList(DialogManager.eDialog.ParkRewardCheckList);
	}

	public new void init(GameObject item)
	{
		base.init(item);
		createLine();
	}

	public IEnumerator show()
	{
		clear();
		List<RewardData> reward_datas = new List<RewardData>();
		RewardData reward_data = new RewardData();
		GameObject data_object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		ParkBuildingDataTable data_table = data_object.GetComponent<ParkBuildingDataTable>();
		data_table.load();
		ParkBuildingInfo.BuildingInfo[] buildingInfos = data_table.getAllBuildingInfo();
		ParkObjectManager park_object_manager = ParkObjectManager.Instance;
		SaveParkData save_data = SaveData.Instance.getParkData();
		SaveParkData.PlacedData[] placed_buildings = save_data.buildings.ToArray();
		List<ParkBuildingInfo.BuildingInfo> infos = new List<ParkBuildingInfo.BuildingInfo>();
		ParkBuildingInfo.BuildingInfo building = null;
		int heart_num = Bridge.PlayerData.getHeart();
		int coin_num = Bridge.PlayerData.getCoin();
		int item_max = 0;
		List<UserItemList> item_list = new List<UserItemList>();
		GameData game_data = GlobalData.Instance.getGameData();
		for (int l = 0; l < game_data.userItemList.Length; l++)
		{
			item_list.Add(new UserItemList
			{
				itemType = game_data.userItemList[l].itemType,
				count = game_data.userItemList[l].count
			});
		}
		NetworkMng.Instance.setup(null);
		yield return dialogManager_.StartCoroutine(NetworkMng.Instance.download(API.ParkGetBuildingDailyReward, true, true));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield return dialogManager_.StartCoroutine(NetworkMng.Instance.showIcon(false));
			yield break;
		}
		WWW www__ = NetworkMng.Instance.getWWW();
		ParkDailyRewardData daily_reward = JsonMapper.ToObject<ParkDailyRewardData>(www__.text);
		game_data.bonusJewel = daily_reward.bonusJewel;
		game_data.buyJewel = daily_reward.buyJewel;
		game_data.treasureboxNum = daily_reward.treasureboxNum;
		game_data.heart = daily_reward.heart;
		game_data.coin = daily_reward.coin;
		game_data.exp = daily_reward.exp;
		game_data.level = daily_reward.level;
		game_data.allPlayCount = daily_reward.allPlayCount;
		game_data.allClearCount = daily_reward.allClearCount;
		game_data.allStarSum = daily_reward.allStarSum;
		game_data.allStageScoreSum = daily_reward.allStageScoreSum;
		game_data.minilenCount = daily_reward.minilenCount;
		game_data.minilenTotalCount = daily_reward.minilenTotalCount;
		game_data.giveNiceTotalCount = daily_reward.giveNiceTotalCount;
		game_data.giveNiceMonthlyCount = daily_reward.giveNiceMonthlyCount;
		game_data.tookNiceTotalCount = daily_reward.tookNiceTotalCount;
		game_data.isParkDailyReward = daily_reward.isParkDailyReward;
		game_data.daily_reward = daily_reward;
		if (daily_reward != null && daily_reward.reward != null)
		{
			if (daily_reward.reward.coin > 0)
			{
				reward_data.id = 0;
				reward_data.type = 3;
				reward_data.num = daily_reward.reward.coin;
				reward_datas.Add(reward_data);
				coin_num += daily_reward.reward.coin;
				GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Park Daily Reward", daily_reward.reward.coin);
				reward_data = new RewardData();
			}
			if (daily_reward.reward.heart > 0)
			{
				reward_data.id = 0;
				reward_data.type = 4;
				reward_data.num = daily_reward.reward.heart;
				reward_datas.Add(reward_data);
				heart_num += daily_reward.reward.heart;
				reward_data = new RewardData();
			}
			if (daily_reward.reward.userItem != null && daily_reward.reward.userItem.Length != 0)
			{
				for (int k = 0; k < daily_reward.reward.userItem.Length; k++)
				{
					UserItemList game_data_item = Array.Find(game_data.userItemList, (UserItemList ui) => ui.itemType == daily_reward.reward.userItem[k].itemType);
					if (game_data_item == null)
					{
						List<UserItemList> new_game_data_items = new List<UserItemList>(game_data.userItemList);
						game_data_item = new UserItemList
						{
							itemType = daily_reward.reward.userItem[k].itemType
						};
						new_game_data_items.Add(game_data_item);
						game_data.userItemList = new_game_data_items.ToArray();
					}
					game_data_item.count = Mathf.Clamp(game_data_item.count + daily_reward.reward.userItem[k].count, 0, Constant.ItemMax);
					reward_data.id = daily_reward.reward.userItem[k].itemType;
					reward_data.type = 6;
					reward_data.num = daily_reward.reward.userItem[k].count;
					reward_datas.Add(reward_data);
					UserItemList user_item = item_list.Find((UserItemList item) => item.itemType == reward_data.id);
					if (user_item != null)
					{
						user_item.count += reward_data.num;
					}
					reward_data = new RewardData();
				}
			}
		}
		if (reward_data != null && reward_datas.Count == 0)
		{
			_openState = true;
			yield break;
		}
		if (item_list.Count > 0)
		{
			for (int j = 0; j < item_list.Count; j++)
			{
				if (item_list[j].count > item_max)
				{
					item_max = item_list[j].count;
				}
			}
		}
		for (int i = 0; i < reward_datas.Count; i++)
		{
			RewardData left_reward_data2 = null;
			RewardData right_reward_data = null;
			if (i * 2 < reward_datas.Count)
			{
				left_reward_data2 = reward_datas[i * 2];
				if (i * 2 + 1 < reward_datas.Count)
				{
					right_reward_data = reward_datas[i * 2 + 1];
				}
				GameObject addObject = UnityEngine.Object.Instantiate(item_) as GameObject;
				Utility.setParent(addObject, base.transform, false);
				MenuParkRewardCheckListItem rewardItem = addObject.GetComponent<MenuParkRewardCheckListItem>();
				rewardItem.Setup(left_reward_data2, right_reward_data);
				itemList_.Add(addObject);
				addItem(addObject, i);
			}
		}
		GameObject main_ui_obj = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI);
		if ((bool)main_ui_obj)
		{
			MainMenu main_ui = main_ui_obj.GetComponent<MainMenu>();
			if ((bool)main_ui)
			{
				main_ui.update();
			}
		}
		base.gameObject.SetActive(true);
		repositionItem();
		yield return StartCoroutine(base.open());
		yield return dialogManager_.StartCoroutine(CheckLimitOver(heart_num, coin_num, item_max));
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "CloseButton":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	private IEnumerator CheckLimitOver(int heart_num, int coin_num, int item_num)
	{
		if (heart_num > Constant.HeartMax)
		{
			DialogLimitOver limitOverDialog3 = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
			yield return dialogManager_.StartCoroutine(limitOverDialog3.show(Constant.eMoney.Heart));
		}
		if (coin_num > Constant.CoinMax)
		{
			DialogLimitOver limitOverDialog2 = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
			while (limitOverDialog2.isOpen())
			{
				yield return 0;
			}
			yield return dialogManager_.StartCoroutine(limitOverDialog2.show(Constant.eMoney.Coin));
		}
		if (item_num > Constant.ItemMax)
		{
			DialogLimitOver limitOverDialog = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
			while (limitOverDialog.isOpen())
			{
				yield return 0;
			}
			yield return dialogManager_.StartCoroutine(limitOverDialog.show(Constant.eMoney.Ticket));
		}
	}
}
