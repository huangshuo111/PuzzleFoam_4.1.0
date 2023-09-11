using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class DialogParkNiceHistoryList : DialogScrollListBase
{
	public class NiceHistoryListData
	{
		public UserData user;

		public bool TakableNice;

		public int GiveNiceElapsedTimes;

		public bool is_open_park = true;

		public void CloneUserData(out NiceHistoryListData output)
		{
			output = new NiceHistoryListData();
			user.Clone(out output.user);
			output.TakableNice = TakableNice;
			output.is_open_park = is_open_park;
		}
	}

	private List<NiceHistoryListData> _list_datas = new List<NiceHistoryListData>();

	public override void OnOpen()
	{
		dialogManager_.addActiveDialogList(DialogManager.eDialog.ParkNiceHistoryList);
	}

	public override void OnClose()
	{
		dialogManager_.removeActiveDialogList(DialogManager.eDialog.ParkNiceHistoryList);
	}

	public override void init(GameObject item)
	{
		base.init(item);
		createLine();
		addLine();
	}

	public IEnumerator show()
	{
		Input.enable = false;
		clear();
		addLine();
		_list_datas.Clear();
		ParkNiceList nice_list = GlobalData.Instance.getGameData().niceList;
		UserData[] friend_datas = DummyPlayFriendData.DummyFriends;
		ParkNiceListData[] niceList = nice_list.niceList;
		foreach (ParkNiceListData park_nice_list in niceList)
		{
			NiceHistoryListData history_data = new NiceHistoryListData();
			if (park_nice_list.memberNo == 0L)
			{
				SaveParkData.DummyParkData dummy_park_data = SaveData.Instance.getParkData().dummyParkData;
				history_data.user = DummyPlayFriendData.createDummyFriend(-1, 0);
				history_data.user.minilenId = dummy_park_data.UseMinilenID;
				history_data.TakableNice = park_nice_list.takableNice;
				history_data.user.TotalMinilenNum = dummy_park_data.TotalMinilenCount;
			}
			else
			{
				UserData user = Array.Find(friend_datas, (UserData f) => f.ID == park_nice_list.memberNo);
				if (user == null)
				{
					continue;
				}
				history_data.user = user;
				history_data.user.minilenId = park_nice_list.minilenId;
				history_data.TakableNice = park_nice_list.takableNice;
				history_data.user.TotalMinilenNum = park_nice_list.minilenTotalCount;
			}
			for (int i = 0; i < park_nice_list.giveNiceElapsedTimes.Length; i++)
			{
				NiceHistoryListData new_data = null;
				history_data.CloneUserData(out new_data);
				new_data.GiveNiceElapsedTimes = park_nice_list.giveNiceElapsedTimes[i];
				_list_datas.Add(new_data);
			}
		}
		_list_datas.Sort(delegate(NiceHistoryListData a, NiceHistoryListData b)
		{
			if (a.GiveNiceElapsedTimes > b.GiveNiceElapsedTimes)
			{
				return 1;
			}
			return (a.GiveNiceElapsedTimes < b.GiveNiceElapsedTimes) ? (-1) : 0;
		});
		bool load_texture = _list_datas.Exists((NiceHistoryListData l) => l.user.Texture == null);
		if (load_texture)
		{
			yield return NetworkMng.Instance.StartCoroutine(NetworkMng.Instance.showIcon(true));
		}
		for (int j = 0; j < _list_datas.Count; j++)
		{
			GameObject addObject = UnityEngine.Object.Instantiate(item_) as GameObject;
			Utility.setParent(addObject, base.transform, false);
			MenuParkNiceHistoryItem niceHistoryItem = addObject.GetComponent<MenuParkNiceHistoryItem>();
			yield return dialogManager_.StartCoroutine(niceHistoryItem.Setup(_list_datas[j], true));
			itemList_.Add(addObject);
			addItem(addObject, j);
		}
		if (load_texture)
		{
			yield return NetworkMng.Instance.StartCoroutine(NetworkMng.Instance.showIcon(false));
		}
		base.gameObject.SetActive(true);
		repositionItem();
		yield return StartCoroutine(base.open());
		Input.enable = true;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Close_Button":
		{
			Constant.SoundUtil.PlayCancelSE();
			StartCoroutine(base.close());
			DialogParkNiceDetail dialog = dialogManager_.getDialog(DialogManager.eDialog.ParkNiceDetail) as DialogParkNiceDetail;
			yield return StartCoroutine(dialog.Setup());
			dialog.setCommentMessage();
			dialogManager_.StartCoroutine(dialog.open());
			break;
		}
		}
		if (!trigger.name.Contains("GoToPark_Button"))
		{
			yield break;
		}
		Constant.SoundUtil.PlayButtonSE();
		long playerID = -1L;
		if (long.TryParse(trigger.name.Substring("GoToPark_Button_".Length), out playerID))
		{
			StartCoroutine(close());
			NiceHistoryListData get_data2 = null;
			get_data2 = _list_datas.Find((NiceHistoryListData d) => d.user.ID == playerID);
			if (get_data2 != null)
			{
				yield return ParkObjectManager.Instance.StartCoroutine(ParkObjectManager.Instance.createParkMapForFriend(get_data2));
			}
		}
	}
}
