using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Network;
using UnityEngine;

public class DialogParkFriendList : DialogScrollListBase
{
	private List<DialogParkNiceHistoryList.NiceHistoryListData> _list_datas;

	public override void OnCreate()
	{
		base.OnCreate();
	}

	public override void OnOpen()
	{
		dialogManager_.addActiveDialogList(DialogManager.eDialog.ParkFriendList);
	}

	public override void OnClose()
	{
		dialogManager_.removeActiveDialogList(DialogManager.eDialog.ParkFriendList);
	}

	public override void init(GameObject item)
	{
		base.init(item);
		createLine();
		addLine();
		_list_datas = new List<DialogParkNiceHistoryList.NiceHistoryListData>();
	}

	public IEnumerator show(long currentFriendID = -1)
	{
		Input.enable = false;
		clear();
		addLine();
		DialogParkNiceHistoryList.NiceHistoryListData data2 = null;
		_list_datas.Clear();
		NetworkMng.Instance.setup(null);
		yield return dialogManager_.StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, false, true));
		WWW www = NetworkMng.Instance.getWWW();
		MemberData[] player_datas = JsonMapper.ToObject<FriendData>(www.text).memberDataList;
		UserData[] dummy_user_datas = DummyPlayFriendData.DummyFriends;
		UserData[] array = dummy_user_datas;
		foreach (UserData user in array)
		{
			data2 = new DialogParkNiceHistoryList.NiceHistoryListData();
			data2.user = new UserData();
			data2.user.minilenId = user.minilenId;
			data2.user.TotalMinilenNum = user.TotalMinilenNum;
			data2.user.lastUpdateTime = user.lastUpdateTime;
			data2.user.UserName = user.UserName;
			data2.user.Texture = user.Texture;
			data2.user.URL = user.URL;
			data2.user.ID = user.ID;
			data2.user.Mid = user.Mid;
			MemberData member_data = Array.Find(player_datas, (MemberData pd) => pd.memberNo == user.ID);
			if (member_data != null)
			{
				data2.user.ID = member_data.memberNo;
				data2.user.TotalMinilenNum = member_data.minilenTotalCount;
				data2.user.minilenId = member_data.minilenId;
				data2.is_open_park = member_data.isParkOpen;
				_list_datas.Add(data2);
			}
		}
		_list_datas.Sort((DialogParkNiceHistoryList.NiceHistoryListData a, DialogParkNiceHistoryList.NiceHistoryListData b) => b.user.TotalMinilenNum - a.user.TotalMinilenNum);
		DialogParkNiceHistoryList.NiceHistoryListData dummy_friend_data = new DialogParkNiceHistoryList.NiceHistoryListData();
		dummy_friend_data.user = DummyPlayFriendData.createDummyFriend(-1, 0);
		dummy_friend_data.TakableNice = false;
		dummy_friend_data.user.minilenId = 30000;
		dummy_friend_data.user.TotalMinilenNum = 37;
		SaveParkData.DummyParkData dummy_park_data = SaveData.Instance.getParkData().dummyParkData;
		if (dummy_park_data != null)
		{
			dummy_friend_data.user.minilenId = dummy_park_data.UseMinilenID;
			dummy_friend_data.user.TotalMinilenNum = dummy_park_data.TotalMinilenCount;
		}
		if (ParkTutorialUtility.isPlaying)
		{
			_list_datas.Insert(0, dummy_friend_data);
		}
		else
		{
			_list_datas.Add(dummy_friend_data);
		}
		for (int i = 0; i < _list_datas.Count; i++)
		{
			GameObject addObject = UnityEngine.Object.Instantiate(item_) as GameObject;
			Utility.setParent(addObject, base.transform, false);
			MenuParkNiceHistoryItem niceHistoryItem = addObject.GetComponent<MenuParkNiceHistoryItem>();
			yield return dialogManager_.StartCoroutine(niceHistoryItem.Setup(_list_datas[i], false));
			itemList_.Add(addObject);
			addItem(addObject, i + 1);
		}
		base.gameObject.SetActive(true);
		repositionItem();
		setCurrentVisitFriend(currentFriendID);
		yield return StartCoroutine(base.open());
		Input.enable = true;
	}

	private WWW OnCreateWWWForDummy(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Post);
		string param = "0";
		WWWWrap.addPostParameter("memberNos", param);
		return WWWWrap.create("player/data/");
	}

	private WWW OnCreateWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Post);
		string text = string.Empty;
		UserData[] dummyFriends = DummyPlayFriendData.DummyFriends;
		foreach (UserData userData in dummyFriends)
		{
			if (text.Length > 0)
			{
				text += ",";
			}
			text += userData.ID;
		}
		WWWWrap.addPostParameter("memberNos", text);
		return WWWWrap.create("player/data/");
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			StartCoroutine(base.close());
			break;
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
			DialogParkNiceHistoryList.NiceHistoryListData get_data2 = null;
			get_data2 = _list_datas.Find((DialogParkNiceHistoryList.NiceHistoryListData d) => d.user.ID == playerID);
			if (get_data2 != null)
			{
				yield return ParkObjectManager.Instance.StartCoroutine(ParkObjectManager.Instance.createParkMapForFriend(get_data2));
			}
		}
	}

	public void setCurrentVisitFriend(long playerID)
	{
		int num = _list_datas.FindIndex((DialogParkNiceHistoryList.NiceHistoryListData d) => d.user.ID == playerID);
		if (num >= 0)
		{
			MenuParkNiceHistoryItem component = itemList_[num].GetComponent<MenuParkNiceHistoryItem>();
			if (component != null)
			{
				component.setEnableGoToParkButton(false);
			}
		}
	}
}
