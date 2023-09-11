using System;
using Bridge;
using Network;
using UnityEngine;

public class MenuParkAreaListItem : MonoBehaviour
{
	[SerializeField]
	private GameObject _open_obj;

	[SerializeField]
	private UILabel _open_area_name_label;

	[SerializeField]
	private GameObject _open_button;

	[SerializeField]
	private UISprite[] _open_minilen_sprites = new UISprite[0];

	[SerializeField]
	private GameObject _unlock_obj;

	[SerializeField]
	private UILabel _unlock_area_name_label;

	[SerializeField]
	private GameObject _unlock_area_clear;

	[SerializeField]
	private UILabel _unlock_area_clear_label;

	[SerializeField]
	private GameObject _unlock_minilen;

	[SerializeField]
	private UILabel _unlock_minilen_label;

	public void Setup(ParkAreaInfo area_info, bool is_open)
	{
		MessageResource instance = MessageResource.Instance;
		_open_obj.SetActive(is_open);
		_unlock_obj.SetActive(!is_open);
		if (is_open)
		{
			_open_button.name = "Item_Button" + area_info.area_id;
			_open_area_name_label.text = MessageResource.Instance.getMessage(9200 + area_info.area_id);
			for (int i = 0; i < _open_minilen_sprites.Length; i++)
			{
				if (i >= area_info.gettable_minilens.Length)
				{
					_open_minilen_sprites[i].gameObject.SetActive(false);
					continue;
				}
				Network.MinilenData minilenData = Array.Find(Bridge.MinilenData.getMinielenData(), (Network.MinilenData m) => m.index == area_info.gettable_minilens[i]);
				if (minilenData == null)
				{
					_open_minilen_sprites[i].gameObject.SetActive(false);
					continue;
				}
				_open_minilen_sprites[i].gameObject.SetActive(true);
				if (minilenData.level <= 0)
				{
					_open_minilen_sprites[i].spriteName = "UI_silhouette_mini_" + (area_info.gettable_minilens[i] % 10000).ToString("000");
					_open_minilen_sprites[i].MakePixelPerfect();
					_open_minilen_sprites[i].transform.localScale *= 0.35f;
				}
				else
				{
					_open_minilen_sprites[i].spriteName = "UI_icon_mini_" + (area_info.gettable_minilens[i] % 10000).ToString("000");
					_open_minilen_sprites[i].MakePixelPerfect();
				}
			}
			Transform transform = base.transform.Find("Open/SaleIcon");
			Transform transform2 = base.transform.Find("Open/StageItemSaleIcon");
			if ((bool)transform && (bool)transform2)
			{
				bool flag = false;
				bool flag2 = false;
				int[] saleArea = GlobalData.Instance.getGameData().saleArea;
				if (saleArea != null && Array.Exists(saleArea, (int sale_area) => sale_area == area_info.area_id + 500000))
				{
					flag = true;
				}
				int[] saleStageItemArea = GlobalData.Instance.getGameData().saleStageItemArea;
				if (saleStageItemArea != null && Array.Exists(saleStageItemArea, (int sale_stage) => sale_stage == area_info.area_id + 500000))
				{
					flag2 = true;
				}
				if (flag)
				{
					transform.gameObject.SetActive(true);
				}
				else if (flag2)
				{
					transform2.gameObject.SetActive(true);
				}
			}
			return;
		}
		_unlock_minilen.SetActive(false);
		_unlock_area_clear.SetActive(true);
		_unlock_area_name_label.text = MessageResource.Instance.getMessage(9200 + area_info.area_id);
		StageDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
		ParkStageInfo parkStageData = component.getParkStageData();
		int num = -1;
		if (area_info.first_info.Common.EntryStage <= 0)
		{
			ParkStageInfo.Info info = Array.Find(parkStageData.Infos, (ParkStageInfo.Info ps) => ps.Common.StageNo == area_info.first_info.Common.EntryStage);
			if (info != null && info.Area != area_info.area_id)
			{
				num = info.Area;
			}
		}
		if (num < 0)
		{
			num = Mathf.Max(0, area_info.area_id - 1);
		}
		string message = instance.getMessage(9200 + num);
		string message2 = instance.getMessage(9103);
		_unlock_area_clear_label.text = instance.castCtrlCode(message2, 1, message);
		if (area_info.first_info.Common.EntryMinilenThanks > 0)
		{
			UILabel unlock_area_clear_label = _unlock_area_clear_label;
			unlock_area_clear_label.text = unlock_area_clear_label.text + "\n    OR\n" + instance.getMessage(9104);
		}
	}
}
