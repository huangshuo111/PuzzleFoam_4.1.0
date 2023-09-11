using System;
using System.Collections;
using UnityEngine;

public class MenuParkNiceHistoryItem : MonoBehaviour
{
	public enum eMessage
	{
		MinutesAgo = 9162,
		HoursAgo = 9163,
		DaysAgo = 9164,
		LongTimeAgo = 9165
	}

	[SerializeField]
	private UILabel _playerNameLabel;

	[SerializeField]
	private PlayerIcon _playerIcon;

	[SerializeField]
	private DummyFriendIcon _dummyFriendIcon_;

	[SerializeField]
	private UILabel _minilenCountLabel;

	[SerializeField]
	private UILabel _niceDateLabel;

	[SerializeField]
	private GameObject _goToParkButton;

	[SerializeField]
	private UISprite _minilen_sprite;

	[SerializeField]
	private GameObject _no_park_label_obj;

	public IEnumerator Setup(DialogParkNiceHistoryList.NiceHistoryListData data, bool use_nice_history)
	{
		string _minilen_sprite_name = "UI_picturebook_mini_";
		_minilen_sprite_name += (data.user.minilenId % 3000).ToString("000");
		if (_minilen_sprite.atlas.spriteList.Exists((UIAtlas.Sprite s) => s.name == _minilen_sprite_name))
		{
			_minilen_sprite.spriteName = _minilen_sprite_name;
			_minilen_sprite.MakePixelPerfect();
		}
		_playerIcon.createMaterial();
		if (data.user.Texture != null)
		{
			_playerIcon.setTexture(data.user.Texture);
		}
		else if (!data.user.IsDummy)
		{
			NetworkMng.Instance.StartCoroutine(_playerIcon.loadTexture(data.user.URL, true, data.user));
		}
		else
		{
			_playerIcon.gameObject.SetActive(false);
			_dummyFriendIcon_.gameObject.SetActive(true);
			DummyFriendDataTable dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<DummyFriendDataTable>();
			_dummyFriendIcon_.setFriendSprite(dataTable.getInfo((int)data.user.ID));
		}
		_playerNameLabel.text = data.user.UserName;
		string replace_userName = Constant.UserName.ReplaceOverStr(_playerNameLabel);
		_playerNameLabel.text = replace_userName;
		_minilenCountLabel.text = data.user.TotalMinilenNum.ToString();
		_goToParkButton.name = _goToParkButton.name + "_" + data.user.ID;
		_niceDateLabel.gameObject.SetActive(use_nice_history);
		if (use_nice_history)
		{
			int days = 0;
			int hours = 0;
			int minutes = 0;
			int seconds = 0;
			getElapsedTimes(data.GiveNiceElapsedTimes, out days, out hours, out minutes, out seconds);
			setDateString(days, hours, minutes, seconds);
		}
		if (!data.is_open_park)
		{
			_goToParkButton.SetActive(false);
			_no_park_label_obj.SetActive(true);
		}
		else
		{
			_no_park_label_obj.SetActive(false);
		}
		yield break;
	}

	public static void getElapsedTimes(int elapsedTimes, out int days, out int hours, out int minutes, out int seconds)
	{
		days = elapsedTimes / 86400;
		int num = elapsedTimes % 86400;
		hours = num / 3600;
		num %= 3600;
		minutes = num / 60;
		seconds = num % 60;
	}

	private void setDateString(long days, long hours, long minutes, long seconds)
	{
		MessageResource messageResource = MessageResource.Instance;
		string labelMessage = string.Empty;
		Action<int, long> action = delegate(int id, long time)
		{
			string message = messageResource.getMessage(id);
			if (message.Length > 0)
			{
				labelMessage = message.Replace("%CTRL01%", time.ToString());
			}
		};
		if (days >= 3)
		{
			action(9165, 3L);
		}
		else if (days > 0)
		{
			action(9164, days);
		}
		else if (hours > 0)
		{
			action(9163, hours);
		}
		else if (minutes > 0)
		{
			long num = minutes / 5 * 5;
			if (num == 0L)
			{
				num = 5L;
			}
			if (num >= 60)
			{
				action(9163, 1L);
			}
			else
			{
				action(9162, num);
			}
		}
		else
		{
			action(9162, 1L);
		}
		_niceDateLabel.text = labelMessage;
	}

	public void setEnableGoToParkButton(bool enable)
	{
		UIButton componentInChildren = _goToParkButton.GetComponentInChildren<UIButton>();
		if (!(componentInChildren != null))
		{
			return;
		}
		componentInChildren.setEnable(enable);
		if (!enable)
		{
			UILabel componentInChildren2 = _goToParkButton.transform.GetComponentInChildren<UILabel>();
			UISprite componentInChildren3 = _goToParkButton.transform.GetComponentInChildren<UISprite>();
			if ((bool)componentInChildren2 && (bool)componentInChildren3)
			{
				componentInChildren2.color = Color.gray;
				componentInChildren3.color = Color.gray;
			}
		}
	}
}
