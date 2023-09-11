using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class DialogParkRoadList : DialogScrollListBase
{
	private MinilenThanksDialogManager _thanks_manager;

	private UISprite _building_sprite;

	public override void OnCreate()
	{
		base.OnCreate();
	}

	public override void OnOpen()
	{
		dialogManager_.addActiveDialogList(DialogManager.eDialog.ParkRoadList);
	}

	public override void OnClose()
	{
		dialogManager_.removeActiveDialogList(DialogManager.eDialog.ParkRoadList);
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
		ParkBuildingInfo.BuildingInfo[] allRoadInfo = component.getAllRoadInfo();
		addLine();
		BuildingData[] buildings = null;
		GameData gameData = GlobalData.Instance.getGameData();
		if (gameData != null && gameData.buildings != null)
		{
			buildings = gameData.buildings;
		}
		else
		{
			int num = 2;
			buildings = new BuildingData[num];
			for (int j = 0; j < num; j++)
			{
				buildings[j] = new BuildingData();
				buildings[j].x = -1;
				buildings[j].id = 50001 + j;
			}
		}
		List<DialogParkBuildingList.BuildingListData> list = new List<DialogParkBuildingList.BuildingListData>();
		for (int i = 0; i < buildings.Length; i++)
		{
			ParkBuildingInfo.BuildingInfo buildingInfo = Array.Find(allRoadInfo, (ParkBuildingInfo.BuildingInfo r) => r.ID == buildings[i].id);
			if (buildingInfo != null)
			{
				bool flag = false;
				UIAtlas uIAtlas = null;
				string spriteName = buildingInfo.SpriteName;
				string building_sprite_name = "UI_picturebook_road_0001";
				if (_building_sprite != null && _building_sprite.atlas.spriteList.Exists((UIAtlas.Sprite s) => s.name == building_sprite_name))
				{
					DialogParkBuildingList.BuildingListData buildingListData = new DialogParkBuildingList.BuildingListData();
					buildingListData.info = buildingInfo;
					buildingListData.iconSpriteName = building_sprite_name;
					buildingListData.placedNum = 1;
					list.Add(buildingListData);
				}
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(item_) as GameObject;
			Utility.setParent(gameObject, base.transform, false);
			MenuParkRoadListItem component2 = gameObject.GetComponent<MenuParkRoadListItem>();
			component2.Setup(list[k], true, @object);
			itemList_.Add(gameObject);
			addItem(gameObject, k + 1);
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
		case "Switching_Button":
		{
			StartCoroutine(_thanks_manager.Close());
			Transform item = trigger.transform.parent;
			int road_id = item.GetComponent<MenuParkRoadListItem>().roadId;
			break;
		}
		}
		yield break;
	}
}
