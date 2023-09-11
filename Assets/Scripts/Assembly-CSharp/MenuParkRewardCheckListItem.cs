using UnityEngine;

public class MenuParkRewardCheckListItem : MonoBehaviour
{
	[SerializeField]
	private UISprite _reward_icon_left;

	[SerializeField]
	private UISprite _reward_icon_right;

	[SerializeField]
	private UILabel _reward_num_left;

	[SerializeField]
	private UILabel _reward_num_right;

	[SerializeField]
	private GameObject _line_left;

	[SerializeField]
	private GameObject _line_right;

	public void Setup(DialogParkRewardCheckList.RewardData left_data, DialogParkRewardCheckList.RewardData right_data)
	{
		if (left_data != null)
		{
			_reward_icon_left.gameObject.SetActive(true);
			_reward_num_left.gameObject.SetActive(true);
			_line_left.gameObject.SetActive(true);
			_reward_num_left.text = left_data.num.ToString();
			RewardIconType(left_data, _reward_icon_left);
		}
		else
		{
			_reward_icon_left.gameObject.SetActive(false);
			_reward_num_left.gameObject.SetActive(false);
			_line_left.gameObject.SetActive(false);
		}
		if (right_data != null)
		{
			_reward_icon_right.gameObject.SetActive(true);
			_reward_num_right.gameObject.SetActive(true);
			_line_right.gameObject.SetActive(true);
			_reward_num_right.text = right_data.num.ToString();
			RewardIconType(right_data, _reward_icon_right);
		}
		else
		{
			_reward_icon_right.gameObject.SetActive(false);
			_reward_num_right.gameObject.SetActive(false);
			_line_right.gameObject.SetActive(false);
		}
	}

	private void RewardIconType(DialogParkRewardCheckList.RewardData data, UISprite icon)
	{
		switch (data.type)
		{
		case 3:
			icon.spriteName = "UI_icon_coin_00";
			icon.MakePixelPerfect();
			break;
		case 4:
			icon.spriteName = "UI_icon_heart_00";
			icon.MakePixelPerfect();
			break;
		case 6:
		{
			string empty = string.Empty;
			empty = empty + "item_" + (data.id % 1000).ToString("000") + "_00";
			icon.spriteName = empty;
			icon.MakePixelPerfect();
			icon.transform.localScale *= 0.6f;
			break;
		}
		case 5:
			break;
		}
	}
}
