using System.Collections;
using UnityEngine;

public class DialogParkThankGet : DialogBase
{
	private UISprite _reward_icon_Main_5;

	private UISprite _reward_icon_Common;

	private UILabel _reward_label;

	private MinilenThanks.MinilenThanksInfo _info;

	public override void OnCreate()
	{
		base.OnCreate();
	}

	public override void OnOpen()
	{
		MenuParkMinilenDance.instance.Play();
		dialogManager_.addActiveDialogList(DialogManager.eDialog.ParkMinilenThanksGet);
	}

	public override void OnClose()
	{
		dialogManager_.removeActiveDialogList(DialogManager.eDialog.ParkMinilenThanksGet);
		MenuParkMinilenDance.instance.Stop();
		stopConfettiEff();
		if (_info.IncentiveType == 9)
		{
			ParkObjectManager.Instance.StartCoroutine(ParkObjectManager.Instance.ReleaseLockedArea(_info.IncentiveId));
		}
	}

	public void init()
	{
		_reward_icon_Main_5 = FindChildComponent<UISprite>("window/Avatar/reward_icon_Main_5");
		_reward_icon_Common = FindChildComponent<UISprite>("window/Avatar/reward_icon_Common");
		_reward_label = FindChildComponent<UILabel>("window/labels/label_reward");
	}

	public IEnumerator show(MinilenThanks.MinilenThanksInfo info)
	{
		_info = info;
		MessageResource message = MessageResource.Instance;
		Input.enable = false;
		if (!_reward_label)
		{
			init();
		}
		switch (_info.IncentiveType)
		{
		case 1:
		{
			_reward_icon_Main_5.gameObject.SetActive(true);
			_reward_icon_Common.gameObject.SetActive(false);
			_reward_icon_Main_5.spriteName = "UI_picturebook_building_" + (_info.IncentiveId % 10000).ToString("0000");
			_reward_icon_Main_5.MakePixelPerfect();
			ParkBuildingDataTable building_data_table = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ParkBuildingDataTable>();
			ParkBuildingInfo.BuildingInfo building_info = building_data_table.getInfo(_info.IncentiveId);
			if (building_info == null)
			{
				Input.enable = true;
				base.gameObject.SetActive(false);
				yield break;
			}
			string building_reward_str2 = message.getMessage(9149);
			building_reward_str2 = message.castCtrlCode(building_reward_str2, 1, message.getMessage(building_info.NameID));
			_reward_label.text = building_reward_str2;
			break;
		}
		case 9:
			_reward_icon_Main_5.gameObject.SetActive(true);
			_reward_icon_Common.gameObject.SetActive(false);
			_reward_icon_Main_5.spriteName = "UI_parkitem_001";
			_reward_icon_Main_5.MakePixelPerfect();
			_reward_label.text = message.getMessage(9150);
			break;
		case 2:
		{
			_reward_icon_Main_5.gameObject.SetActive(true);
			_reward_icon_Main_5.spriteName = "UI_parkitem_002";
			_reward_icon_Main_5.MakePixelPerfect();
			_reward_icon_Common.gameObject.SetActive(false);
			StageDataTable stage_data_table = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
			if (stage_data_table.getParkAreaInfo(_info.IncentiveId) == null)
			{
				Input.enable = true;
				base.gameObject.SetActive(false);
				yield break;
			}
			string stage_reward_str2 = message.getMessage(9151);
			stage_reward_str2 = message.castCtrlCode(stage_reward_str2, 1, message.getMessage(9200 + _info.IncentiveId));
			_reward_label.text = stage_reward_str2;
			break;
		}
		case 8:
		{
			_reward_icon_Main_5.gameObject.SetActive(false);
			_reward_icon_Common.gameObject.SetActive(true);
			_reward_icon_Common.spriteName = "UI_park_icon_key";
			_reward_icon_Common.MakePixelPerfect();
			_reward_icon_Common.transform.localScale *= 1.5f;
			string boss_key_str = message.getMessage(9152);
			_reward_label.text = message.castCtrlCode(boss_key_str, 1, _info.IncentiveNum.ToString());
			break;
		}
		case 3:
		{
			_reward_icon_Main_5.gameObject.SetActive(false);
			_reward_icon_Common.gameObject.SetActive(true);
			_reward_icon_Common.spriteName = "UI_icon_coin_00";
			_reward_icon_Common.MakePixelPerfect();
			_reward_icon_Common.transform.localScale *= 1.5f;
			string coin_reward_str = message.getMessage(9153);
			coin_reward_str = message.castCtrlCode(coin_reward_str, 1, _info.IncentiveNum.ToString());
			_reward_label.text = coin_reward_str;
			break;
		}
		case 7:
		{
			_reward_icon_Main_5.gameObject.SetActive(false);
			_reward_icon_Common.gameObject.SetActive(true);
			_reward_icon_Common.spriteName = "gacha_ticket";
			_reward_icon_Common.MakePixelPerfect();
			_reward_icon_Common.transform.localScale *= 1.5f;
			string gacha_ticket_str = message.getMessage(9154);
			_reward_label.text = message.castCtrlCode(gacha_ticket_str, 1, _info.IncentiveNum.ToString());
			break;
		}
		case 4:
		{
			_reward_icon_Main_5.gameObject.SetActive(false);
			_reward_icon_Common.gameObject.SetActive(true);
			_reward_icon_Common.spriteName = "UI_icon_heart_00";
			_reward_icon_Common.MakePixelPerfect();
			_reward_icon_Common.transform.localScale *= 1.5f;
			string heart_str = message.getMessage(9155);
			_reward_label.text = message.castCtrlCode(heart_str, 1, _info.IncentiveNum.ToString());
			break;
		}
		case 5:
		{
			_reward_icon_Main_5.gameObject.SetActive(false);
			_reward_icon_Common.gameObject.SetActive(true);
			_reward_icon_Common.spriteName = "UI_icon_juwel_00";
			_reward_icon_Common.MakePixelPerfect();
			_reward_icon_Common.transform.localScale *= 1.5f;
			string juwel_str = message.getMessage(9156);
			_reward_label.text = message.castCtrlCode(juwel_str, 1, _info.IncentiveNum.ToString());
			break;
		}
		case 6:
		{
			_reward_icon_Main_5.gameObject.SetActive(false);
			_reward_icon_Common.gameObject.SetActive(true);
			_reward_icon_Common.spriteName = "item_" + (_info.IncentiveId % 1000).ToString("000") + "_00";
			_reward_icon_Common.MakePixelPerfect();
			string item_str = message.getMessage(9157);
			item_str = message.castCtrlCode(item_str, 1, message.getMessage(_info.IncentiveId - 1));
			_reward_label.text = message.castCtrlCode(item_str, 2, _info.IncentiveNum.ToString());
			break;
		}
		default:
			Input.enable = true;
			base.gameObject.SetActive(false);
			yield break;
		}
		startConfettiEff();
		Sound.Instance.playSe(Sound.eSe.SE_013_park_gift);
		yield return StartCoroutine(base.open());
		Input.enable = true;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		Constant.SoundUtil.PlayDecideSE();
		if (_info.IncentiveType == 9)
		{
			dialogManager_.StartCoroutine(Object.FindObjectOfType<MinilenThanksDialogManager>().Close());
		}
		else
		{
			DialogParkThanksList list_dialog = dialogManager_.getDialog(DialogManager.eDialog.ParkMinilenThanks) as DialogParkThanksList;
			MinilenThanksDialogManager thanks_manager = Object.FindObjectOfType<MinilenThanksDialogManager>();
			yield return StartCoroutine(thanks_manager.Close());
			yield return StartCoroutine(thanks_manager.Show());
		}
		StartCoroutine(base.close());
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
		if (!(Object)component)
		{
			Debug.LogError(transform.name + " Don't Have Component " + typeof(T).ToString(), transform.gameObject);
			return (T)null;
		}
		return component;
	}
}
