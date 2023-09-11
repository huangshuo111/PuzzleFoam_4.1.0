using System.Collections.Generic;
using UnityEngine;

public class MenuParkBuildingListItem : MonoBehaviour
{
	private enum eBuildingItemMessage
	{
		Grid = 9109,
		Reward = 9110,
		Possession = 9111,
		Arrangement = 9112,
		NoneReward = 9113
	}

	private const int NONE_REWARD = -1;

	[SerializeField]
	private UISprite _building_image;

	[SerializeField]
	private UILabel _building_name;

	[SerializeField]
	private UILabel _grid_ratio;

	[SerializeField]
	private UILabel _reward_number;

	[SerializeField]
	private UILabel _possession_number;

	[SerializeField]
	private UIButton _arrangement_button;

	[SerializeField]
	private UISprite _reward_icon_image;

	[SerializeField]
	private UILabel _grid_label;

	[SerializeField]
	private UILabel _reward_label;

	[SerializeField]
	private UILabel _possession_label;

	[SerializeField]
	private UILabel _arrangement_label;

	private int _building_id;

	private List<int> _uniqueIDList = new List<int>();

	public int buildingId
	{
		get
		{
			return _building_id;
		}
	}

	public int uniqueId
	{
		get
		{
			if (_uniqueIDList.Count == 0)
			{
				return -1;
			}
			return _uniqueIDList[0];
		}
	}

	public void Setup(DialogParkBuildingList.BuildingListData building_list_data, bool can_build, GameObject data_object)
	{
		ParkBuildingInfo.BuildingInfo info = building_list_data.info;
		_building_id = info.ID;
		_uniqueIDList.AddRange(building_list_data.placableUniqueIDList);
		MessageResource instance = MessageResource.Instance;
		_grid_label.text = instance.getMessage(9109);
		_reward_label.text = instance.getMessage(9110);
		_possession_label.text = instance.getMessage(9111);
		_arrangement_label.text = instance.getMessage(9112);
		_building_image.spriteName = building_list_data.iconSpriteName;
		_building_image.MakePixelPerfect();
		int nameID = info.NameID;
		string message = instance.getMessage(nameID);
		_building_name.text = message;
		string text = info.GridWidth.ToString();
		string text2 = info.GridHeight.ToString();
		_grid_ratio.text = text + "x" + text2;
		int rewardId = info.RewardId;
		int rewardType = info.RewardType;
		string text3 = info.RewardNum.ToString();
		switch (rewardType)
		{
		case 3:
			_reward_number.text = text3.ToString();
			_reward_icon_image.spriteName = "UI_icon_coin_00";
			break;
		case 4:
			_reward_number.text = text3.ToString();
			_reward_icon_image.spriteName = "UI_icon_heart_00";
			break;
		case 6:
		{
			_reward_number.text = text3.ToString();
			string empty = string.Empty;
			empty = empty + "item_" + (rewardId % 1000).ToString("000") + "_00";
			_reward_icon_image.spriteName = empty;
			break;
		}
		case -1:
			_reward_icon_image.gameObject.SetActive(false);
			_reward_number.text = instance.getMessage(9113);
			break;
		}
		_possession_number.text = string.Format("{0}/{1}", building_list_data.placableNum, building_list_data.havingNum);
		bool flag = building_list_data.placableNum > 0;
		_arrangement_button.setEnable(flag);
		if (!flag)
		{
			_arrangement_label.color = _arrangement_button.pressed;
		}
	}
}
