using System;
using System.Collections;
using Bridge;
using Network;
using UnityEngine;

public class DialogParkThanksList : DialogScrollListBase
{
	private class MinilenThanksData
	{
		public Network.MinilenThanksData net_data;

		public MinilenThanks.MinilenThanksInfo info;

		public bool can_get;
	}

	private MinilenThanksDialogManager _thanks_manager;

	private DialogParkThankDetail _detail_dialog;

	public override void OnCreate()
	{
		base.OnCreate();
	}

	public override void OnOpen()
	{
		dialogManager_.addActiveDialogList(DialogManager.eDialog.ParkMinilenThanks);
	}

	public override void OnClose()
	{
		dialogManager_.removeActiveDialogList(DialogManager.eDialog.ParkMinilenThanks);
	}

	public void init(GameObject item, MinilenThanksDialogManager thanks_manager)
	{
		base.init(item);
		createLine();
		addLine();
		_thanks_manager = thanks_manager;
		_detail_dialog = dialogManager_.getDialog(DialogManager.eDialog.ParkMinilenThanksDetail) as DialogParkThankDetail;
		if (!_detail_dialog)
		{
			Debug.LogError("Cannot Found Dialog : DialogParkThankDetail");
		}
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		MinilenThanksDataTable component = @object.GetComponent<MinilenThanksDataTable>();
		component.load();
	}

	public void show_setup()
	{
		clear();
		Transform transform = base.transform.Find("Minilen_num/count");
		if ((bool)transform)
		{
			UILabel component = transform.GetComponent<UILabel>();
			if ((bool)component)
			{
				component.text = Bridge.PlayerData.getMinilenCount().ToString();
			}
			else
			{
				component.text = "-";
			}
		}
		Transform transform2 = base.transform.Find("Total_Minilen_num/count");
		if ((bool)transform2)
		{
			UILabel component2 = transform2.GetComponent<UILabel>();
			if ((bool)component2)
			{
				component2.text = Bridge.PlayerData.getMinilenTotalCount().ToString();
			}
			else
			{
				component2.text = "-";
			}
		}
		Setup();
		base.gameObject.SetActive(true);
		repositionItem();
		base.gameObject.SetActive(false);
	}

	private void Setup()
	{
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		MinilenThanksDataTable component = @object.GetComponent<MinilenThanksDataTable>();
		MinilenThanks.MinilenThanksInfo[] allInfo = component.getAllInfo();
		Network.MinilenThanksData[] thanks_list = null;
		thanks_list = GlobalData.Instance.getGameData().thanksList;
		MinilenThanksData[] array = new MinilenThanksData[thanks_list.Length];
		int minilenCount = Bridge.PlayerData.getMinilenCount();
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new MinilenThanksData();
			array[i].net_data = thanks_list[i];
			array[i].info = Array.Find(allInfo, (MinilenThanks.MinilenThanksInfo thank) => thank.ID == thanks_list[i].ID);
			if (array[i].info != null && thanks_list[i].available && array[i].info.MinilenPrice <= minilenCount)
			{
				array[i].can_get = true;
			}
		}
		Array.Sort(array, delegate(MinilenThanksData a, MinilenThanksData b)
		{
			if (a.can_get && !b.can_get)
			{
				return -1;
			}
			if (!a.can_get && b.can_get)
			{
				return 1;
			}
			if (a.info == null)
			{
				return 1;
			}
			if (b.info == null)
			{
				return -1;
			}
			if (a.info.IncentiveType < b.info.IncentiveType)
			{
				return -1;
			}
			if (a.info.IncentiveType > b.info.IncentiveType)
			{
				return 1;
			}
			if (a.net_data.ID < b.net_data.ID)
			{
				return -1;
			}
			return (a.net_data.ID > b.net_data.ID) ? 1 : 0;
		});
		addLine();
		for (int j = 0; j < array.Length; j++)
		{
			MinilenThanksData minilenThanksData = array[j];
			if (minilenThanksData.info != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(item_) as GameObject;
				Utility.setParent(gameObject, base.transform, false);
				MenuParkThanksListItem component2 = gameObject.GetComponent<MenuParkThanksListItem>();
				if (!component2.Setup(minilenThanksData.info, minilenThanksData.can_get, @object))
				{
					Debug.LogWarning("Thanks List Missed Creating List-Item. " + minilenThanksData.info.ID);
					UnityEngine.Object.Destroy(gameObject);
				}
				else
				{
					itemList_.Add(gameObject);
					addItem(gameObject, j);
				}
			}
		}
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		if (trigger.name.Contains("Close_Button"))
		{
			_thanks_manager.StartCoroutine(_thanks_manager.Close());
		}
		if (trigger.name.Contains("Thanks_Button") || trigger.name.Contains("Get_Button"))
		{
			MenuParkThanksListItem menu_item = trigger.transform.parent.GetComponent<MenuParkThanksListItem>();
			if ((bool)menu_item)
			{
				Constant.SoundUtil.PlayButtonSE();
				_detail_dialog.gameObject.SetActive(true);
				StartCoroutine(_detail_dialog.show(menu_item.info, menu_item.can_get));
			}
		}
		yield break;
	}
}
