using System;
using System.Collections;
using Bridge;
using LitJson;
using Network;
using TapjoyUnity;
using UnityEngine;

public class DialogParkThankDetail : DialogBase
{
	private const float TOTAL_MINILEN_TITLE_HEIGHT_SCALE = 30f;

	private const float TOTAL_MINILEN_TITLE_ENGLISH_WIDTH_SCALE = 25f;

	private const float TOTAL_MINILEN_TITLE_JAPANESE_WIDTH_SCALE = 30f;

	private UIPanel _panel;

	private UILabel _minilen_num_label;

	private UILabel _minilen_num_title_label;

	private Transform _close_button;

	private Transform _building_pattern;

	private Transform _area_pattern;

	private Transform _item_pattern;

	private Transform _exchange_notice;

	private Transform _build_reward;

	private UISprite _sp_condition_window;

	private UILabel _sp_condition_title;

	private UILabel _sp_condition_label;

	private UILabel _condition_only_label_label;

	private MinilenThanks.MinilenThanksInfo _info;

	private bool _is_available;

	private bool _open_get_dialog;

	private bool _is_pressed_button;

	public override void OnCreate()
	{
		base.OnCreate();
	}

	public override void OnOpen()
	{
		dialogManager_.addActiveDialogList(DialogManager.eDialog.ParkMinilenThanksDetail);
	}

	public override void OnClose()
	{
		dialogManager_.removeActiveDialogList(DialogManager.eDialog.ParkMinilenThanksDetail);
		if (_open_get_dialog)
		{
			DialogParkThankGet dialogParkThankGet = dialogManager_.getDialog(DialogManager.eDialog.ParkMinilenThanksGet) as DialogParkThankGet;
			dialogParkThankGet.gameObject.SetActive(true);
			dialogManager_.StartCoroutine(dialogParkThankGet.show(_info));
		}
	}

	public void init()
	{
		Transform transform = base.transform;
		_panel = GetComponent<UIPanel>();
		_minilen_num_label = FindChildComponent<UILabel>("Minilen_num/label_count");
		_minilen_num_title_label = FindChildComponent<UILabel>("Minilen_num/title");
		_close_button = FindChildComponent<Transform>("Close_Button");
		_building_pattern = FindChildComponent<Transform>("window/reward_building");
		_area_pattern = FindChildComponent<Transform>("window/reward_area");
		_item_pattern = FindChildComponent<Transform>("window/reward_item");
		_exchange_notice = FindChildComponent<Transform>("window/exchange_notice");
		_build_reward = FindChildComponent<Transform>("window/reward_building/avatar/reward_count");
		_sp_condition_window = FindChildComponent<UISprite>("window/reward_area/base_01");
		_sp_condition_title = FindChildComponent<UILabel>("window/reward_area/labels/misson2");
		_sp_condition_label = FindChildComponent<UILabel>("window/reward_area/labels/found2");
		_condition_only_label_label = FindChildComponent<UILabel>("window/reward_item/labels/found");
	}

	public IEnumerator show(MinilenThanks.MinilenThanksInfo info, bool is_available)
	{
		_info = info;
		_is_available = is_available;
		_open_get_dialog = false;
		_is_pressed_button = false;
		Input.enable = false;
		if (!_panel)
		{
			init();
		}
		_sp_condition_window.gameObject.SetActive(false);
		_sp_condition_title.gameObject.SetActive(false);
		_sp_condition_label.gameObject.SetActive(false);
		_panel.alpha = 0f;
		Network.MinilenData minilen_data = Array.Find(Bridge.MinilenData.getMinielenData(), (Network.MinilenData m) => m.index == _info.ConditionMinilenId);
		MessageResource message = MessageResource.Instance;
		_minilen_num_title_label.transform.localScale = new Vector3(30f, 30f, 1f);
		if (info.MinilenPrice > 0 || minilen_data != null)
		{
			_minilen_num_label.text = Bridge.PlayerData.getMinilenCount().ToString();
			_minilen_num_title_label.text = message.getMessage(9122);
		}
		else if (info.ConditionTotalMinilens > 0)
		{
			_minilen_num_label.text = Bridge.PlayerData.getMinilenTotalCount().ToString();
			_minilen_num_title_label.text = message.getMessage(9171);
			if (!SaveData.Instance.getSystemData().getOptionData().isKorean())
			{
				_minilen_num_title_label.transform.localScale = new Vector3(25f, 30f, 1f);
			}
		}
		if (_info.IncentiveType == 1)
		{
			if (!SetupBuildingReward())
			{
				Input.enable = true;
				base.gameObject.SetActive(false);
				yield break;
			}
		}
		else if (_info.IncentiveType == 2 || _info.IncentiveType == 9)
		{
			if (!SetupAreaReward())
			{
				Input.enable = true;
				base.gameObject.SetActive(false);
				yield break;
			}
		}
		else if (!SetupItemReward())
		{
			Input.enable = true;
			base.gameObject.SetActive(false);
			yield break;
		}
		Input.enable = true;
		yield return StartCoroutine(base.open());
	}

	private bool SetupBuildingReward()
	{
		MessageResource instance = MessageResource.Instance;
		_building_pattern.gameObject.SetActive(true);
		_area_pattern.gameObject.SetActive(false);
		_item_pattern.gameObject.SetActive(false);
		if (_info.IncentiveNum <= 1)
		{
			_build_reward.gameObject.SetActive(false);
		}
		else
		{
			_build_reward.gameObject.SetActive(true);
		}
		FindChildComponent<UILabel>("getButton/label_message", _building_pattern).text = ((!_is_available) ? instance.getMessage(9142) : instance.getMessage(9141));
		ParkBuildingDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ParkBuildingDataTable>();
		if (!component)
		{
			Debug.LogError("GlobalRoot Don't Have ParkBuildingDataTable component.");
			return false;
		}
		ParkBuildingInfo.BuildingInfo info = component.getInfo(_info.IncentiveId);
		if (info == null)
		{
			Debug.LogError("Building Master Data Don’t Have Id = " + _info.IncentiveId);
			return false;
		}
		string empty = string.Empty;
		Network.MinilenData minilenData = Array.Find(Bridge.MinilenData.getMinielenData(), (Network.MinilenData m) => m.index == _info.ConditionMinilenId);
		empty = ((minilenData == null) ? instance.getMessage(9170) : instance.getMessage(9135));
		FindChildComponent<UILabel>("labels/specialbuilding", _building_pattern).text = empty;
		FindChildComponent<UILabel>("labels/label_building_name", _building_pattern).text = instance.getMessage(info.NameID);
		FindChildComponent<UILabel>("labels/label_grid", _building_pattern).text = info.GridWidth + "x" + info.GridHeight;
		UISprite uISprite = FindChildComponent<UISprite>("avatar/building", _building_pattern);
		if (info.ID >= 50000)
		{
			uISprite.spriteName = "UI_picturebook_road_" + (info.ID % 10000).ToString("0000");
		}
		else
		{
			uISprite.spriteName = "UI_picturebook_building_" + (info.ID % 10000).ToString("0000");
		}
		uISprite.MakePixelPerfect();
		uISprite.transform.localScale *= 1.5f;
		string building_reward_icon_name = string.Empty;
		float num = 1f;
		switch (info.RewardType)
		{
		case 8:
			building_reward_icon_name = "UI_park_icon_key";
			break;
		case 3:
			building_reward_icon_name = "UI_icon_coin_00";
			break;
		case 7:
			building_reward_icon_name = "gacha_ticket";
			num = 0.8f;
			break;
		case 4:
			building_reward_icon_name = "UI_icon_heart_00";
			break;
		case 5:
			building_reward_icon_name = "UI_icon_juwel_00";
			break;
		case 6:
			building_reward_icon_name = "item_" + (info.RewardId % 1000).ToString("000") + "_00";
			num = 0.6f;
			break;
		}
		UISprite uISprite2 = FindChildComponent<UISprite>("avatar/coin", _building_pattern);
		if (string.IsNullOrEmpty(building_reward_icon_name))
		{
			uISprite2.enabled = false;
			FindChildComponent<UILabel>("labels/label_rewardcount", _building_pattern).text = "-";
		}
		else
		{
			uISprite2.enabled = true;
			if (!uISprite2.atlas.spriteList.Exists((UIAtlas.Sprite s) => s.name == building_reward_icon_name))
			{
				return false;
			}
			uISprite2.spriteName = building_reward_icon_name;
			uISprite2.MakePixelPerfect();
			uISprite2.transform.localScale *= num;
			FindChildComponent<UILabel>("labels/label_rewardcount", _building_pattern).text = info.RewardNum.ToString();
		}
		if (_info.IncentiveNum > 1)
		{
			_build_reward.GetComponent<UILabel>().text = "x" + _info.IncentiveNum;
		}
		if (!SetConditionData(FindChildComponent<UISprite>("avatar/minilen", _building_pattern), FindChildComponent<UILabel>("labels/rescue", _building_pattern), FindChildComponent<Transform>("getButton", _building_pattern)))
		{
			return false;
		}
		return true;
	}

	private bool SetupAreaReward()
	{
		MessageResource instance = MessageResource.Instance;
		_building_pattern.gameObject.SetActive(false);
		_area_pattern.gameObject.SetActive(true);
		_item_pattern.gameObject.SetActive(false);
		FindChildComponent<UILabel>("checkButton/label_message", _area_pattern).text = ((!_is_available) ? instance.getMessage(9142) : instance.getMessage(9141));
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (_info.IncentiveType == 2)
		{
			StageDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
			if (!component)
			{
				Debug.LogError("GlobalRoot Don't Have StageDataTable component.");
				return false;
			}
			ParkAreaInfo parkAreaInfo = component.getParkAreaInfo(_info.IncentiveId);
			if (parkAreaInfo == null)
			{
				Debug.LogError("Park Stage Master Data Don’t Have Id = " + _info.IncentiveId);
				return false;
			}
			empty = instance.getMessage(parkAreaInfo.area_id + 9200);
			empty2 = "UI_parkitem_002";
			FindChildComponent<UILabel>("labels/area", _area_pattern).text = instance.getMessage(9143);
		}
		else
		{
			empty = instance.getMessage(9148);
			empty2 = "UI_parkitem_001";
			FindChildComponent<UILabel>("labels/area", _area_pattern).text = instance.getMessage(9147);
		}
		FindChildComponent<UILabel>("labels/label_area_name", _area_pattern).text = empty;
		UISprite uISprite = FindChildComponent<UISprite>("avatar/item", _area_pattern);
		uISprite.spriteName = empty2;
		uISprite.MakePixelPerfect();
		uISprite.transform.localScale *= 1.5f;
		if (!SetConditionData(FindChildComponent<UISprite>("avatar/minilen", _area_pattern), FindChildComponent<UILabel>("labels/found", _area_pattern), FindChildComponent<Transform>("checkButton", _area_pattern)))
		{
			return false;
		}
		return true;
	}

	private bool SetupItemReward()
	{
		MessageResource instance = MessageResource.Instance;
		_building_pattern.gameObject.SetActive(false);
		_area_pattern.gameObject.SetActive(false);
		_item_pattern.gameObject.SetActive(true);
		FindChildComponent<UILabel>("checkButton/label_message", _item_pattern).text = ((!_is_available) ? instance.getMessage(9142) : instance.getMessage(9141));
		UISprite uISprite = FindChildComponent<UISprite>("avatar/item", _item_pattern);
		UISprite uISprite2 = FindChildComponent<UISprite>("avatar/jem", _item_pattern);
		string icon_name = string.Empty;
		switch (_info.IncentiveType)
		{
		case 8:
			icon_name = "UI_park_icon_key";
			break;
		case 3:
			icon_name = "UI_icon_coin_00";
			break;
		case 7:
			icon_name = "gacha_ticket";
			break;
		case 4:
			icon_name = "UI_icon_heart_00";
			break;
		case 5:
			icon_name = "UI_icon_juwel_00";
			break;
		case 6:
			icon_name = "item_" + (_info.IncentiveId % 1000).ToString("000") + "_00";
			if (!uISprite.atlas.spriteList.Exists((UIAtlas.Sprite s) => s.name == icon_name))
			{
				return false;
			}
			break;
		default:
			return false;
		}
		uISprite.spriteName = icon_name;
		uISprite.MakePixelPerfect();
		uISprite2.spriteName = icon_name;
		uISprite2.MakePixelPerfect();
		if (_info.IncentiveType == 6)
		{
			uISprite2.transform.localScale *= 0.5f;
		}
		else
		{
			uISprite.transform.localScale *= 1.5f;
		}
		FindChildComponent<UILabel>("labels/label_count", _item_pattern).text = _info.IncentiveNum.ToString();
		if (!SetConditionData(FindChildComponent<UISprite>("avatar/minilen", _item_pattern), FindChildComponent<UILabel>("labels/found", _item_pattern), FindChildComponent<Transform>("checkButton", _item_pattern)))
		{
			return false;
		}
		return true;
	}

	private bool SetConditionData(UISprite minilen_icon_sprite, UILabel condition_label, Transform button_trans)
	{
		MessageResource instance = MessageResource.Instance;
		minilen_icon_sprite.color = Color.white;
		if (_is_available && _info.MinilenPrice > 0)
		{
			condition_label.gameObject.SetActive(true);
			string message = instance.getMessage(9105);
			message = message.Replace("\r", string.Empty).Replace("\n", string.Empty);
			condition_label.text = instance.castCtrlCode(message, 1, _info.MinilenPrice.ToString());
			minilen_icon_sprite.gameObject.SetActive(true);
			minilen_icon_sprite.spriteName = "UI_icon_mini_000";
			minilen_icon_sprite.MakePixelPerfect();
			Transform transform = minilen_icon_sprite.transform;
			Transform transform2 = condition_label.transform;
			Vector3 localPosition = transform.localPosition;
			localPosition.x = condition_label.transform.localPosition.x;
			localPosition.x -= Math.Abs(transform2.localScale.x) * condition_label.relativeSize.x;
			localPosition.x -= 0.5f * Math.Abs(transform.localScale.x) * minilen_icon_sprite.relativeSize.x;
			transform.localPosition = localPosition;
			if ((bool)button_trans)
			{
				FindChildComponent<UISprite>("Background", button_trans).MakePixelPerfect();
				Vector3 localPosition2 = button_trans.localPosition;
				localPosition2.y = -280f;
				button_trans.localPosition = localPosition2;
				_exchange_notice.gameObject.SetActive(true);
				FindChildComponent<UILabel>("label_message", button_trans).text = instance.getMessage(9174);
			}
			return true;
		}
		if ((bool)button_trans)
		{
			UISprite uISprite = FindChildComponent<UISprite>("Background", button_trans);
			uISprite.MakePixelPerfect();
			uISprite.transform.localScale *= 1.22f;
			Vector3 localPosition3 = button_trans.localPosition;
			localPosition3.y = -270f;
			button_trans.localPosition = localPosition3;
			_exchange_notice.gameObject.SetActive(false);
		}
		Network.MinilenData minilenData = Array.Find(Bridge.MinilenData.getMinielenData(), (Network.MinilenData m) => m.index == _info.ConditionMinilenId);
		if (minilenData != null)
		{
			condition_label.gameObject.SetActive(true);
			condition_label.text = instance.getMessage(9106);
			minilen_icon_sprite.gameObject.SetActive(true);
			minilen_icon_sprite.spriteName = "UI_icon_mini_" + (_info.ConditionMinilenId % 1000).ToString("000");
			minilen_icon_sprite.MakePixelPerfect();
			if (!_is_available && minilenData.level <= 0)
			{
				minilen_icon_sprite.color = Color.black;
			}
			else
			{
				minilen_icon_sprite.color = Color.white;
			}
			Transform transform3 = minilen_icon_sprite.transform;
			Transform transform4 = condition_label.transform;
			Vector3 localPosition4 = transform3.localPosition;
			localPosition4.x = condition_label.transform.localPosition.x;
			localPosition4.x -= Math.Abs(transform4.localScale.x) * condition_label.relativeSize.x;
			localPosition4.x -= 0.5f * Math.Abs(transform3.localScale.x) * minilen_icon_sprite.relativeSize.x;
			transform3.localPosition = localPosition4;
			return true;
		}
		if (_info.ConditionTotalMinilens > 0 && _info.ConditionParkAreaId >= 0)
		{
			condition_label.gameObject.SetActive(true);
			string message2 = instance.getMessage(9118);
			message2 = instance.castCtrlCode(message2, 1, _info.ConditionTotalMinilens.ToString());
			message2 = message2.Replace("\r", string.Empty).Replace("\n", string.Empty);
			condition_label.text = message2;
			minilen_icon_sprite.gameObject.SetActive(true);
			minilen_icon_sprite.spriteName = "UI_icon_mini_000";
			minilen_icon_sprite.MakePixelPerfect();
			Transform transform5 = minilen_icon_sprite.transform;
			Transform transform6 = condition_label.transform;
			Vector3 localPosition5 = transform5.localPosition;
			localPosition5.x = condition_label.transform.localPosition.x;
			localPosition5.x -= Math.Abs(transform6.localScale.x) * condition_label.relativeSize.x;
			localPosition5.x -= 0.5f * Math.Abs(transform5.localScale.x) * minilen_icon_sprite.relativeSize.x;
			transform5.localPosition = localPosition5;
			_sp_condition_window.gameObject.SetActive(true);
			_sp_condition_title.gameObject.SetActive(true);
			_sp_condition_label.gameObject.SetActive(true);
			string message3 = instance.getMessage(9175);
			message3 = instance.castCtrlCode(message3, 1, _info.ConditionParkAreaId.ToString());
			message3 = message3.Replace("\r", string.Empty).Replace("\n", string.Empty);
			_sp_condition_label.text = message3;
			return true;
		}
		if (_info.ConditionTotalMinilens > 0)
		{
			condition_label.gameObject.SetActive(true);
			string message4 = instance.getMessage(9118);
			message4 = instance.castCtrlCode(message4, 1, _info.ConditionTotalMinilens.ToString());
			message4 = message4.Replace("\r", string.Empty).Replace("\n", string.Empty);
			condition_label.text = message4;
			minilen_icon_sprite.gameObject.SetActive(true);
			minilen_icon_sprite.spriteName = "UI_icon_mini_000";
			minilen_icon_sprite.MakePixelPerfect();
			Transform transform7 = minilen_icon_sprite.transform;
			Transform transform8 = condition_label.transform;
			Vector3 localPosition6 = transform7.localPosition;
			localPosition6.x = condition_label.transform.localPosition.x;
			localPosition6.x -= Math.Abs(transform8.localScale.x) * condition_label.relativeSize.x;
			localPosition6.x -= 0.5f * Math.Abs(transform7.localScale.x) * minilen_icon_sprite.relativeSize.x;
			transform7.localPosition = localPosition6;
			return true;
		}
		if (_info.ConditionParkAreaId >= 0)
		{
			condition_label.gameObject.SetActive(true);
			minilen_icon_sprite.gameObject.SetActive(false);
			string message5 = instance.getMessage(9103);
			message5 = message5.Replace("\r", string.Empty).Replace("\n", string.Empty);
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
		if (_info.MinilenPrice > 0)
		{
			condition_label.gameObject.SetActive(true);
			string message6 = instance.getMessage(9105);
			message6 = message6.Replace("\r", string.Empty).Replace("\n", string.Empty);
			condition_label.text = instance.castCtrlCode(message6, 1, _info.MinilenPrice.ToString());
			minilen_icon_sprite.gameObject.SetActive(true);
			minilen_icon_sprite.spriteName = "UI_icon_mini_000";
			minilen_icon_sprite.MakePixelPerfect();
			Transform transform9 = minilen_icon_sprite.transform;
			Transform transform10 = condition_label.transform;
			Vector3 localPosition7 = transform9.localPosition;
			localPosition7.x = condition_label.transform.localPosition.x;
			localPosition7.x -= Math.Abs(transform10.localScale.x) * condition_label.relativeSize.x;
			localPosition7.x -= 0.5f * Math.Abs(transform9.localScale.x) * minilen_icon_sprite.relativeSize.x;
			transform9.localPosition = localPosition7;
			if ((bool)button_trans && _is_available)
			{
				FindChildComponent<UISprite>("Background", button_trans).MakePixelPerfect();
				Vector3 localPosition8 = button_trans.localPosition;
				localPosition8.y = -280f;
				button_trans.localPosition = localPosition8;
				_exchange_notice.gameObject.SetActive(true);
				FindChildComponent<UILabel>("Background", button_trans).text = instance.getMessage(9174);
			}
			return true;
		}
		return false;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		if (_is_pressed_button)
		{
			yield break;
		}
		_open_get_dialog = false;
		if (trigger.name.Contains("Close_Button"))
		{
			Constant.SoundUtil.PlayCancelSE();
			StartCoroutine(base.close());
		}
		if (!trigger.name.Contains("checkButton") && !trigger.name.Contains("getButton"))
		{
			yield break;
		}
		if (_is_available)
		{
			Constant.SoundUtil.PlayDecideSE();
			bool is_success2 = false;
			_is_pressed_button = true;
			int heart_num = Bridge.PlayerData.getHeart();
			int coin_num = Bridge.PlayerData.getCoin();
			int jewel_num = Bridge.PlayerData.getJewel();
			int item_num = 0;
			int gacha_ticket_num = 0;
			GameData game_data = GlobalData.Instance.getGameData();
			switch ((MinilenThanks.eIncentive)_info.IncentiveType)
			{
			case MinilenThanks.eIncentive.Heart:
				heart_num += _info.IncentiveNum;
				break;
			case MinilenThanks.eIncentive.Coin:
				coin_num += _info.IncentiveNum;
				break;
			case MinilenThanks.eIncentive.Juel:
				jewel_num += _info.IncentiveNum;
				break;
			case MinilenThanks.eIncentive.PuzzleItem:
			{
				UserItemList[] userItemList = game_data.userItemList;
				foreach (UserItemList uItem in userItemList)
				{
					if (uItem.itemType == _info.IncentiveId)
					{
						item_num = uItem.count + _info.IncentiveNum;
						break;
					}
				}
				break;
			}
			case MinilenThanks.eIncentive.GachaTicket:
				gacha_ticket_num = game_data.gachaTicket + _info.IncentiveNum;
				break;
			}
			ParkObjectManager.PostBuildingList postParameter = null;
			SaveData.Instance.getParkData().getNetworkPostParameter(out postParameter);
			Hashtable network_args = new Hashtable
			{
				{ "minilnThanksData_id", _info.ID },
				{
					"buildings",
					JsonMapper.ToJson(postParameter)
				}
			};
			NetworkMng.Instance.setup(network_args);
			yield return StartCoroutine(NetworkMng.Instance.download(API.ParkMinilenThanksClear, false, true));
			if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
			{
				yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
				is_success2 = false;
			}
			else
			{
				WWW www = NetworkMng.Instance.getWWW();
				MinilenThanksRecieveData receive = JsonMapper.ToObject<MinilenThanksRecieveData>(www.text);
				game_data.bonusJewel = receive.bonusJewel;
				game_data.buyJewel = receive.buyJewel;
				game_data.treasureboxNum = receive.treasureboxNum;
				game_data.heart = receive.heart;
				game_data.coin = receive.coin;
				game_data.exp = receive.exp;
				game_data.level = receive.level;
				game_data.allPlayCount = receive.allPlayCount;
				game_data.allClearCount = receive.allClearCount;
				game_data.allStarSum = receive.allStarSum;
				game_data.allStageScoreSum = receive.allStageScoreSum;
				game_data.userItemList = receive.userItemList;
				game_data.minilenCount = receive.minilenCount;
				game_data.minilenTotalCount = receive.minilenTotalCount;
				game_data.giveNiceTotalCount = receive.giveNiceTotalCount;
				game_data.giveNiceMonthlyCount = receive.giveNiceMonthlyCount;
				game_data.tookNiceTotalCount = receive.tookNiceTotalCount;
				game_data.isParkDailyReward = receive.isParkDailyReward;
				game_data.setParkData(receive.parkStageList, receive.minilenList, receive.thanksList, receive.buildings, receive.mapReleaseNum);
				SaveData.Instance.getParkData().UpdatePlacedData(receive.buildings);
				SaveData.Instance.getParkData().save();
				is_success2 = true;
				Tapjoy.TrackEvent("Money", "Expense Minilen", "Minilen Thanks", null, _info.MinilenPrice);
				Tapjoy.TrackEvent("Minilen", "Minilen Thanks", _info.ID.ToString(), null, 0L);
				switch ((MinilenThanks.eIncentive)_info.IncentiveType)
				{
				case MinilenThanks.eIncentive.Coin:
					Tapjoy.TrackEvent("Money", "Income Coin", "Minilen Thanks", null, _info.IncentiveNum);
					break;
				case MinilenThanks.eIncentive.Juel:
					Tapjoy.TrackEvent("Money", "Income Jewel", "Minilen Thanks", null, _info.IncentiveNum);
					break;
				}
				GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Minilen", "Minilen Thanks", _info.MinilenPrice);
				GlobalGoogleAnalytics.Instance.LogEvent("Minilen", "Minilen Thanks", _info.ID.ToString(), 1L);
				switch ((MinilenThanks.eIncentive)_info.IncentiveType)
				{
				case MinilenThanks.eIncentive.Coin:
					GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Coin", "Minilen Thanks", _info.IncentiveNum);
					break;
				case MinilenThanks.eIncentive.Juel:
					GlobalGoogleAnalytics.Instance.LogEvent("Money", "Income Jewel", "Minilen Thanks", _info.IncentiveNum);
					break;
				}
			}
			if (!is_success2)
			{
				StartCoroutine(base.close());
				yield break;
			}
			yield return StartCoroutine(CheckLimitOver(heart_num, coin_num, jewel_num, item_num, gacha_ticket_num));
			GameObject main_ui2_obj = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI2);
			if ((bool)main_ui2_obj)
			{
				MenuMinilenThanks thanks_ui = main_ui2_obj.GetComponentInChildren<MenuMinilenThanks>();
				if ((bool)thanks_ui)
				{
					thanks_ui.UpdateCount();
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
			_open_get_dialog = true;
		}
		else
		{
			Constant.SoundUtil.PlayButtonSE();
		}
		StartCoroutine(base.close());
	}

	private IEnumerator CheckLimitOver(int heart_num, int coin_num, int jewel_num, int item_num, int gacha_ticket_num)
	{
		if (heart_num > Constant.HeartMax)
		{
			DialogLimitOver limitOverDialog4 = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
			yield return StartCoroutine(limitOverDialog4.show(Constant.eMoney.Heart));
		}
		if (coin_num > Constant.CoinMax)
		{
			DialogLimitOver limitOverDialog3 = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
			while (limitOverDialog3.isOpen())
			{
				yield return 0;
			}
			yield return StartCoroutine(limitOverDialog3.show(Constant.eMoney.Coin));
		}
		if (jewel_num > Constant.JewelMax)
		{
			DialogLimitOver limitOverDialog2 = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
			while (limitOverDialog2.isOpen())
			{
				yield return 0;
			}
			yield return StartCoroutine(limitOverDialog2.show(Constant.eMoney.Jewel));
		}
		if (item_num > Constant.ItemMax)
		{
			DialogLimitOver limitOverDialog = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
			while (limitOverDialog.isOpen())
			{
				yield return 0;
			}
			yield return StartCoroutine(limitOverDialog.show(Constant.eMoney.Ticket));
		}
	}

	private T FindChildComponent<T>(string child_path, Transform parent = null) where T : Component
	{
		if (!parent)
		{
			parent = base.transform;
		}
		Transform transform = parent.Find(child_path);
		if (!transform)
		{
			Debug.LogError(parent.name + " Don't Have Child named " + child_path, parent.gameObject);
			return (T)null;
		}
		T component = transform.GetComponent<T>();
		if (!(UnityEngine.Object)component)
		{
			Debug.LogError(transform.name + " Don't Have Component " + typeof(T).ToString(), transform.gameObject);
			return (T)null;
		}
		return component;
	}
}
