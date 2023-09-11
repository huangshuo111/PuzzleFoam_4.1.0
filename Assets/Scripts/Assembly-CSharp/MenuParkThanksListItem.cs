using System;
using Bridge;
using Network;
using UnityEngine;

public class MenuParkThanksListItem : MonoBehaviour
{
	[SerializeField]
	private GameObject _clear_mark;

	[SerializeField]
	private GameObject _get_button;

	[SerializeField]
	private GameObject _thanks_button;

	[SerializeField]
	private GameObject _incentive_only_icon;

	[SerializeField]
	private GameObject _incentive_with_number;

	[SerializeField]
	private GameObject _buildincentive_with_number;

	[SerializeField]
	private UISprite _incentive_only_icon_sprite;

	[SerializeField]
	private UISprite _incentive_with_number_sprite;

	[SerializeField]
	private UILabel _incentive_with_number_label;

	[SerializeField]
	private UILabel _buildincentive_with_number_label;

	[SerializeField]
	private GameObject _condition_only_label;

	[SerializeField]
	private GameObject _condition_with_icon;

	[SerializeField]
	private UILabel _condition_only_label_label;

	[SerializeField]
	private UILabel _condition_with_icon_label;

	[SerializeField]
	private UISprite _condition_with_icon_icon;

	[SerializeField]
	private SimpleMessageDraw _get_button_label;

	[SerializeField]
	private UILabel _condition_with_icon_label_1;

	[SerializeField]
	private UILabel _condition_with_icon_label_2;

	private MinilenThanks.MinilenThanksInfo _info;

	private bool _can_get;

	public MinilenThanks.MinilenThanksInfo info
	{
		get
		{
			return _info;
		}
	}

	public bool can_get
	{
		get
		{
			return _can_get;
		}
	}

	public bool Setup(MinilenThanks.MinilenThanksInfo info, bool canGet, GameObject data_object)
	{
		_info = info;
		_can_get = canGet;
		_clear_mark.SetActive(canGet);
		_get_button.SetActive(canGet);
		_thanks_button.SetActive(!canGet);
		_get_button.name = _get_button.name + "_" + _info.ID;
		_thanks_button.name = _thanks_button.name + "_" + _info.ID;
		if (!SetConditionData(data_object))
		{
			return false;
		}
		if (!SetIncentiveData(data_object))
		{
			return false;
		}
		return true;
	}

	private bool SetConditionData(GameObject data_object)
	{
		MessageResource instance = MessageResource.Instance;
		if (_can_get && info.MinilenPrice > 0)
		{
			_condition_only_label.SetActive(false);
			_condition_with_icon.SetActive(true);
			string message = instance.getMessage(9105);
			_condition_with_icon_label.text = instance.castCtrlCode(message, 1, _info.MinilenPrice.ToString());
			_get_button_label.MessageID = 9174;
			return true;
		}
		Network.MinilenData minilenData = Array.Find(Bridge.MinilenData.getMinielenData(), (Network.MinilenData m) => m.index == _info.ConditionMinilenId);
		if (minilenData != null)
		{
			_condition_only_label.SetActive(false);
			_condition_with_icon.SetActive(true);
			_condition_with_icon_label.text = instance.getMessage(9106);
			if (!_can_get && minilenData.level <= 0)
			{
				_condition_with_icon_icon.spriteName = "UI_silhouette_mini_" + (minilenData.index % 10000).ToString("000");
			}
			else
			{
				_condition_with_icon_icon.spriteName = "UI_picturebook_mini_" + (minilenData.index % 10000).ToString("000");
			}
			_condition_with_icon_icon.MakePixelPerfect();
			_condition_with_icon_icon.transform.localScale *= 0.5f;
			return true;
		}
		if (_info.ConditionTotalMinilens > 0 && _info.ConditionParkAreaId >= 0)
		{
			_condition_only_label.SetActive(false);
			_condition_with_icon.SetActive(true);
			_condition_with_icon_label.gameObject.SetActive(false);
			_condition_with_icon_label_1.gameObject.SetActive(true);
			string message2 = instance.getMessage(9118);
			_condition_with_icon_label_1.text = instance.castCtrlCode(message2, 1, _info.ConditionTotalMinilens.ToString());
			_condition_with_icon_label_2.gameObject.SetActive(true);
			string message3 = instance.getMessage(9175);
			_condition_with_icon_label_2.text = instance.castCtrlCode(message3, 1, _info.ConditionParkAreaId.ToString());
			return true;
		}
		if (_info.ConditionTotalMinilens > 0)
		{
			_condition_only_label.SetActive(false);
			_condition_with_icon.SetActive(true);
			string message4 = instance.getMessage(9118);
			_condition_with_icon_label.text = instance.castCtrlCode(message4, 1, _info.ConditionTotalMinilens.ToString());
			return true;
		}
		if (_info.ConditionParkAreaId >= 0)
		{
			_condition_with_icon.SetActive(false);
			_condition_only_label.SetActive(true);
			string message5 = instance.getMessage(9103);
			_condition_only_label_label.text = instance.castCtrlCode(message5, 1, MessageResource.Instance.getMessage(9200 + _info.ConditionParkAreaId));
			if (_info.ConditionParkAreaId > 500000)
			{
				int conditionParkAreaId = _info.ConditionParkAreaId;
				conditionParkAreaId -= 500000;
				conditionParkAreaId = conditionParkAreaId / 10 - 1;
				_condition_only_label_label.text = instance.castCtrlCode(message5, 1, MessageResource.Instance.getMessage(9200 + conditionParkAreaId));
			}
			return true;
		}
		if (info.MinilenPrice > 0)
		{
			_condition_only_label.SetActive(false);
			_condition_with_icon.SetActive(true);
			string message6 = instance.getMessage(9105);
			_condition_with_icon_label.text = instance.castCtrlCode(message6, 1, _info.MinilenPrice.ToString());
			if (_can_get)
			{
				_get_button_label.MessageID = 9174;
			}
			return true;
		}
		return false;
	}

	private bool SetIncentiveData(GameObject data_object)
	{
		if (_info == null || _info.IncentiveNum <= 0)
		{
			return false;
		}
		switch ((MinilenThanks.eIncentive)_info.IncentiveType)
		{
		case MinilenThanks.eIncentive.Building:
		{
			_incentive_only_icon.SetActive(true);
			_incentive_with_number.SetActive(false);
			ParkBuildingDataTable component = data_object.GetComponent<ParkBuildingDataTable>();
			if (!component)
			{
				return false;
			}
			ParkBuildingInfo.BuildingInfo buildingInfo = component.getInfo(_info.IncentiveId);
			if (buildingInfo == null)
			{
				return false;
			}
			string building_sprite_name = string.Empty;
			if (buildingInfo.ID >= 50000)
			{
				building_sprite_name = "UI_picturebook_road_" + (buildingInfo.ID % 10000).ToString("0000");
			}
			else
			{
				building_sprite_name = "UI_icon_building_" + (buildingInfo.ID % 10000).ToString("0000");
			}
			if (!_incentive_only_icon_sprite.atlas.spriteList.Exists((UIAtlas.Sprite s) => s.name == building_sprite_name))
			{
				return false;
			}
			_incentive_only_icon_sprite.spriteName = building_sprite_name;
			_incentive_only_icon_sprite.MakePixelPerfect();
			if (buildingInfo.ID >= 50000)
			{
				_incentive_only_icon_sprite.transform.localScale *= 0.75f;
			}
			if (_info.IncentiveNum > 1)
			{
				_buildincentive_with_number.SetActive(true);
				_buildincentive_with_number_label.text = "x" + _info.IncentiveNum;
			}
			return true;
		}
		case MinilenThanks.eIncentive.ParkStageArea:
			_incentive_only_icon.SetActive(true);
			_incentive_with_number.SetActive(false);
			_incentive_only_icon_sprite.spriteName = "UI_parkitem_002";
			_incentive_only_icon_sprite.MakePixelPerfect();
			_incentive_only_icon_sprite.transform.localScale *= 0.6f;
			return true;
		case MinilenThanks.eIncentive.Coin:
			_incentive_only_icon.SetActive(false);
			_incentive_with_number.SetActive(true);
			_incentive_with_number_sprite.spriteName = "UI_icon_coin_00";
			_incentive_with_number_sprite.MakePixelPerfect();
			_incentive_with_number_label.text = _info.IncentiveNum.ToString();
			return true;
		case MinilenThanks.eIncentive.Heart:
			_incentive_only_icon.SetActive(false);
			_incentive_with_number.SetActive(true);
			_incentive_with_number_sprite.spriteName = "UI_icon_heart_00";
			_incentive_with_number_sprite.MakePixelPerfect();
			_incentive_with_number_label.text = _info.IncentiveNum.ToString();
			return true;
		case MinilenThanks.eIncentive.Juel:
			_incentive_only_icon.SetActive(false);
			_incentive_with_number.SetActive(true);
			_incentive_with_number_sprite.spriteName = "UI_icon_juwel_00";
			_incentive_with_number_sprite.MakePixelPerfect();
			_incentive_with_number_label.text = _info.IncentiveNum.ToString();
			return true;
		case MinilenThanks.eIncentive.PuzzleItem:
		{
			_incentive_only_icon.SetActive(false);
			_incentive_with_number.SetActive(true);
			string sprite_name = "item_" + (_info.IncentiveId % 1000).ToString("000") + "_00";
			if (!_incentive_with_number_sprite.atlas.spriteList.Exists((UIAtlas.Sprite s) => s.name == sprite_name))
			{
				return false;
			}
			_incentive_with_number_sprite.spriteName = sprite_name;
			_incentive_with_number_sprite.MakePixelPerfect();
			_incentive_with_number_sprite.transform.localScale *= 0.6f;
			_incentive_with_number_label.text = _info.IncentiveNum.ToString();
			return true;
		}
		case MinilenThanks.eIncentive.GachaTicket:
			_incentive_only_icon.SetActive(false);
			_incentive_with_number.SetActive(true);
			_incentive_with_number_sprite.spriteName = "gacha_ticket";
			_incentive_with_number_sprite.MakePixelPerfect();
			_incentive_with_number_sprite.transform.localScale *= 0.8f;
			_incentive_with_number_label.text = _info.IncentiveNum.ToString();
			return true;
		case MinilenThanks.eIncentive.BossKey:
			_incentive_only_icon.SetActive(false);
			_incentive_with_number.SetActive(true);
			_incentive_with_number_sprite.spriteName = "UI_park_icon_key";
			_incentive_with_number_sprite.MakePixelPerfect();
			_incentive_with_number_label.text = _info.IncentiveNum.ToString();
			return true;
		case MinilenThanks.eIncentive.ParkMapArea:
			_incentive_only_icon.SetActive(true);
			_incentive_with_number.SetActive(false);
			_incentive_only_icon_sprite.spriteName = "UI_parkitem_001";
			_incentive_only_icon_sprite.MakePixelPerfect();
			_incentive_only_icon_sprite.transform.localScale *= 0.6f;
			return true;
		default:
			return false;
		}
	}
}
