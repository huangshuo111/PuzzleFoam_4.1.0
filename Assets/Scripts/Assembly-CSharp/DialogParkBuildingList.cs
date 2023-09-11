using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class DialogParkBuildingList : DialogScrollListBase
{
	public class BuildingListData
	{
		public ParkBuildingInfo.BuildingInfo info;

		public string iconSpriteName;

		public List<int> placableUniqueIDList = new List<int>();

		public int placedNum;

		public int havingNum;

		public int placableNum
		{
			get
			{
				return havingNum - placedNum;
			}
		}
	}

	private MinilenThanksDialogManager _thanks_manager;

	private UISprite _building_sprite;

	public override void OnCreate()
	{
		base.OnCreate();
	}

	public override void OnOpen()
	{
		dialogManager_.addActiveDialogList(DialogManager.eDialog.ParkBuildingList);
	}

	public override void OnClose()
	{
		dialogManager_.removeActiveDialogList(DialogManager.eDialog.ParkBuildingList);
	}

	public void init(GameObject item, MinilenThanksDialogManager thanks_manager)
	{
		base.init(item);
		createLine();
		addLine();
		_thanks_manager = thanks_manager;
		_building_sprite = base.transform.Find("Building_Sprite").GetComponent<UISprite>();
	}

	public void show_setup()
	{
		clear();
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		ParkBuildingDataTable component = @object.GetComponent<ParkBuildingDataTable>();
		component.load();
		ParkBuildingInfo.BuildingInfo[] buildingInfos = component.getAllBuildingInfo();
		addLine();
		BuildingData[] array = null;
		GameData gameData = GlobalData.Instance.getGameData();
		if (gameData != null && gameData.buildings != null)
		{
			array = gameData.buildings;
		}
		else
		{
			int num = buildingInfos.Length;
			array = new BuildingData[num];
			for (int j = 0; j < num; j++)
			{
				array[j] = new BuildingData();
				array[j].x = -1;
				array[j].id = buildingInfos[j].ID;
			}
		}
		BuildingListData buildingListData = null;
		List<BuildingListData> list = new List<BuildingListData>();
		string building_sprite_name = string.Empty;
		for (int i = 0; i < buildingInfos.Length; i++)
		{
			BuildingData[] array2 = Array.FindAll(array, (BuildingData b) => b.id == buildingInfos[i].ID);
			if (array2 == null || array2.Length == 0)
			{
				continue;
			}
			buildingListData = new BuildingListData();
			buildingListData.info = buildingInfos[i];
			string spriteName = buildingInfos[i].SpriteName;
			building_sprite_name = "UI_picturebook_" + spriteName;
			buildingListData.iconSpriteName = building_sprite_name;
			if (_building_sprite != null && _building_sprite.atlas.spriteList.Exists((UIAtlas.Sprite s) => s.name == building_sprite_name))
			{
				list.Add(buildingListData);
			}
			buildingListData.havingNum = array2.Length;
			for (int k = 0; k < array2.Length; k++)
			{
				if (array2[k].x >= 0)
				{
					buildingListData.placedNum++;
				}
				else
				{
					buildingListData.placableUniqueIDList.Add(array2[k].uid);
				}
			}
		}
		list.Sort(delegate(BuildingListData a, BuildingListData b)
		{
			if (a.placableNum == 0)
			{
				return 1;
			}
			return (b.placableNum == 0) ? (-1) : (a.info.SortID - b.info.SortID);
		});
		for (int l = 0; l < list.Count; l++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(item_) as GameObject;
			Utility.setParent(gameObject, base.transform, false);
			MenuParkBuildingListItem component2 = gameObject.GetComponent<MenuParkBuildingListItem>();
			component2.Setup(list[l], true, @object);
			itemList_.Add(gameObject);
			addItem(gameObject, l);
		}
		base.gameObject.SetActive(true);
		repositionItem();
		base.gameObject.SetActive(false);
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Close_Button":
			_thanks_manager.StartCoroutine(_thanks_manager.Close());
			break;
		case "Arrangement_Button":
		{
			_thanks_manager.StartCoroutine(_thanks_manager.Close());
			Transform item = trigger.transform.parent;
			MenuParkBuildingListItem menu = item.GetComponent<MenuParkBuildingListItem>();
			int building_id = menu.buildingId;
			int unique_id = menu.uniqueId;
			ParkObjectManager.Instance.createBuildingOnScreenCenter(building_id, unique_id);
			break;
		}
		}
		yield break;
	}
}
