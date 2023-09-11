using System;
using System.Collections;
using System.Collections.Generic;
using Bridge;
using UnityEngine;

public class DialogParkAreaList : DialogScrollListBase
{
	private int _select_area_inedex = -1;

	public override void OnCreate()
	{
		base.OnCreate();
	}

	public override void init(GameObject item)
	{
		base.init(item);
		createLine();
		addLine();
		StageDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
		component.loadParkStageData();
	}

	public IEnumerator show()
	{
		Array.ForEach(GetComponentsInChildren<UIPanel>(), delegate(UIPanel p)
		{
			p.alpha = 0f;
		});
		Input.enable = false;
		clear();
		addLine();
		StageDataTable stage_datas = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
		ParkStageInfo park_stage_datas = stage_datas.getParkStageData();
		List<ParkAreaInfo> area_infos = new List<ParkAreaInfo>();
		for (int j = 0; j < park_stage_datas.Infos.Length; j++)
		{
			if (!area_infos.Exists((ParkAreaInfo area) => area.area_id == park_stage_datas.Infos[j].Area))
			{
				ParkAreaInfo area_info = stage_datas.getParkAreaInfo(park_stage_datas.Infos[j].Area);
				if (area_info != null)
				{
					area_infos.Add(area_info);
				}
			}
		}
		yield return 0;
		for (int i = 0; i < area_infos.Count; i++)
		{
			GameObject add_obj = UnityEngine.Object.Instantiate(item_) as GameObject;
			Utility.setParent(add_obj, base.transform, false);
			MenuParkAreaListItem area_item = add_obj.GetComponent<MenuParkAreaListItem>();
			area_item.Setup(area_infos[i], Bridge.StageData.isOpen_Park(area_infos[i].first_info.Common.StageNo, park_stage_datas));
			itemList_.Add(add_obj);
			addItem(add_obj, i + 1);
		}
		yield return 0;
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
			_select_area_inedex = -1;
			StartCoroutine(base.close());
			break;
		}
		if (trigger.name.Contains("Item_Button"))
		{
			int.TryParse(trigger.name.Substring("Item_Button".Length), out _select_area_inedex);
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(base.close());
		}
		yield break;
	}

	public override void OnOpen()
	{
		dialogManager_.addActiveDialogList(DialogManager.eDialog.ParkAreaList);
	}

	public override void OnClose()
	{
		base.OnClose();
		dialogManager_.removeActiveDialogList(DialogManager.eDialog.ParkAreaList);
		if (_select_area_inedex >= 0)
		{
			DialogParkStageList dialogParkStageList = dialogManager_.getDialog(DialogManager.eDialog.ParkStageList) as DialogParkStageList;
			dialogParkStageList.gameObject.SetActive(true);
			dialogParkStageList.StartCoroutine(dialogParkStageList.show(_select_area_inedex));
		}
	}
}
