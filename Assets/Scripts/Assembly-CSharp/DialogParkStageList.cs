using System;
using System.Collections;
using System.Collections.Generic;
using Bridge;
using Network;
using UnityEngine;

public class DialogParkStageList : DialogScrollListBase
{
	private int _area_index = -1;

	private int _stage_index = -1;

	private UILabel _subtitle_label;

	private List<UISprite> _subtitle_minilens = new List<UISprite>();

	public override void OnCreate()
	{
		base.OnCreate();
	}

	public override void init(GameObject item)
	{
		base.init(item);
		createLine();
		addLine();
		_subtitle_label = base.transform.Find("AreaName_Label").GetComponent<UILabel>();
		int num = 1;
		while (true)
		{
			Transform transform = base.transform.Find(num.ToString("Minilen_pos/00"));
			if (transform == null)
			{
				break;
			}
			UISprite component = transform.GetComponent<UISprite>();
			_subtitle_minilens.Add(component);
			num++;
		}
	}

	public IEnumerator show(int area_id)
	{
		_area_index = area_id;
		StageDataTable stage_datas = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
		ParkAreaInfo area_info = stage_datas.getParkAreaInfo(_area_index);
		_subtitle_label.text = MessageResource.Instance.getMessage(9200 + _area_index);
		for (int i = 0; i < _subtitle_minilens.Count; i++)
		{
			if (i >= area_info.gettable_minilens.Length)
			{
				_subtitle_minilens[i].gameObject.SetActive(false);
				continue;
			}
			Network.MinilenData minilen = Array.Find(Bridge.MinilenData.getMinielenData(), (Network.MinilenData m) => m.index == area_info.gettable_minilens[i]);
			if (minilen == null)
			{
				_subtitle_minilens[i].gameObject.SetActive(false);
				continue;
			}
			_subtitle_minilens[i].gameObject.SetActive(true);
			if (minilen.level <= 0)
			{
				_subtitle_minilens[i].spriteName = "UI_silhouette_mini_" + (area_info.gettable_minilens[i] % 10000).ToString("000");
				_subtitle_minilens[i].color = Color.white;
				_subtitle_minilens[i].MakePixelPerfect();
				_subtitle_minilens[i].transform.localScale *= 0.35f;
			}
			else
			{
				_subtitle_minilens[i].spriteName = "UI_icon_mini_" + (area_info.gettable_minilens[i] % 10000).ToString("000");
				_subtitle_minilens[i].color = Color.white;
				_subtitle_minilens[i].MakePixelPerfect();
			}
		}
		Transform sale_icon_trans = base.transform.Find("SaleIcon");
		Transform stage_item_sale_icon_trans = base.transform.Find("StageItemSaleIcon");
		if ((bool)sale_icon_trans && (bool)stage_item_sale_icon_trans)
		{
			bool sale_hit = false;
			bool sale_hit2 = false;
			int[] sale_areas = GlobalData.Instance.getGameData().saleArea;
			if (sale_areas != null && Array.Exists(sale_areas, (int sale_area) => sale_area == area_info.area_id + 500000))
			{
				sale_hit = true;
			}
			int[] sale_stage_areas = GlobalData.Instance.getGameData().saleStageItemArea;
			if (sale_stage_areas != null && Array.Exists(sale_stage_areas, (int sale_stage) => sale_stage == area_info.area_id + 500000))
			{
				sale_hit2 = true;
			}
			Debug.Log("   normalsale " + sale_hit);
			Debug.Log("    sale " + sale_hit2);
			if (sale_hit)
			{
				sale_icon_trans.gameObject.SetActive(true);
			}
			else if (sale_hit2)
			{
				stage_item_sale_icon_trans.gameObject.SetActive(true);
			}
		}
		Input.enable = false;
		clear();
		addLine();
		for (int j = 0; j < area_info.stage_infos.Length; j++)
		{
			if (Bridge.StageData.isOpen_Park(area_info.stage_infos[j].Common.StageNo, stage_datas.getParkStageData()))
			{
				GameObject add_obj = UnityEngine.Object.Instantiate(item_) as GameObject;
				Utility.setParent(add_obj, base.transform, false);
				MenuParkStageListItem stage_item = add_obj.GetComponent<MenuParkStageListItem>();
				stage_item.Setup(area_info.stage_infos[j], Bridge.StageData.getHighScore_Park(area_info.stage_infos[j].Common.StageNo));
				itemList_.Add(add_obj);
				addItem(add_obj, j + 1);
			}
		}
		base.gameObject.SetActive(true);
		repositionItem();
		yield return StartCoroutine(base.open());
		yield return 0;
		Input.enable = true;
	}

	protected virtual IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			_stage_index = -1;
			_area_index = -1;
			StartCoroutine(base.close());
			break;
		}
		if (trigger.name.Contains("Item_Button"))
		{
			int.TryParse(trigger.name.Substring("Item_Button".Length), out _stage_index);
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(base.close());
		}
		yield break;
	}

	public override void OnOpen()
	{
		dialogManager_.addActiveDialogList(DialogManager.eDialog.ParkStageList);
	}

	public override void OnClose()
	{
		base.OnClose();
		dialogManager_.removeActiveDialogList(DialogManager.eDialog.ParkStageList);
		if (_stage_index < 0)
		{
			DialogParkAreaList dialogParkAreaList = dialogManager_.getDialog(DialogManager.eDialog.ParkAreaList) as DialogParkAreaList;
			dialogParkAreaList.gameObject.SetActive(true);
			dialogParkAreaList.StartCoroutine(dialogParkAreaList.show());
		}
		else
		{
			StageIcon stageIcon = base.gameObject.AddComponent<StageIcon>();
			stageIcon.setStageNum_Park(_stage_index);
			DialogSetupPark dialogSetupPark = dialogManager_.getDialog(DialogManager.eDialog.ParkStageSetup) as DialogSetupPark;
			dialogSetupPark.gameObject.SetActive(true);
			dialogSetupPark.StartCoroutine(dialogSetupPark.show(stageIcon, _area_index));
		}
	}
}
