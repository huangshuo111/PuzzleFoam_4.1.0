using System.Collections;
using LitJson;
using Network;
using UnityEngine;

public class ActionReward : MonoBehaviour
{
	public enum eType
	{
		Invalid = -1,
		BestScore = 1,
		LevelUp = 2,
		AreaClear = 3,
		TreasureBox = 4,
		HiscoreChange = 5,
		Review = 7,
		Key = 8,
		Boss = 9,
		Friend = 10
	}

	public static ActionRewardInfo info_;

	public static IEnumerator getActionRewardInfo(eType type)
	{
		info_ = null;
		NetworkMng.Instance.setup(null);
		yield return NetworkMng.Instance.StartCoroutine(NetworkMng.Instance.download(OnCreateGetWWW, true));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield break;
		}
		ActionRewardList infoList = JsonMapper.ToObject<ActionRewardList>(NetworkMng.Instance.getWWW().text);
		ActionRewardInfo[] actionRewardList = infoList.actionRewardList;
		foreach (ActionRewardInfo info in actionRewardList)
		{
			if (info.type == (int)type)
			{
				info_ = info;
				break;
			}
		}
	}

	public static IEnumerator addActionReward(DialogManager dialogManager)
	{
		if (info_ == null)
		{
			yield break;
		}
		Constant.Reward reward = new Constant.Reward
		{
			RewardType = rewardType(),
			Num = rewardNum()
		};
		bool bLimitOver = Constant.Reward.addReward(reward);
		Hashtable args = new Hashtable { { "type", info_.type } };
		NetworkMng.Instance.setup(args);
		yield return NetworkMng.Instance.StartCoroutine(NetworkMng.Instance.download(OnCreateAddWWW, true));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			JsonData json = JsonMapper.ToObject(www.text);
			GameData gameData = GlobalData.Instance.getGameData();
			gameData.heart = (int)json["heart"];
			gameData.coin = (int)json["coin"];
			gameData.bonusJewel = (int)json["bonusJewel"];
			gameData.buyJewel = (int)json["buyJewel"];
			MainMenu menu = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
			menu.update();
			DialogCommon getDialog = dialogManager.getDialog(DialogManager.eDialog.InviteSent) as DialogCommon;
			UISprite sprite = getDialog.transform.Find("window/chara/anm1").GetComponent<UISprite>();
			MessageResource msgRes = MessageResource.Instance;
			string msg4 = msgRes.getMessage(4000);
			if (info_.type == 7)
			{
				msg4 = msgRes.getMessage(4012);
			}
			switch (reward.RewardType)
			{
			case Constant.eMoney.Jewel:
				sprite.spriteName = "UI_chara_00_019";
				msg4 = msgRes.castCtrlCode(msg4, 1, msgRes.getMessage(2569));
				msg4 = msgRes.castCtrlCode(msg4, 2, msgRes.castCtrlCode(msgRes.getMessage(28), 1, reward.Num.ToString("N0")));
				break;
			case Constant.eMoney.Coin:
				sprite.spriteName = "UI_chara_00_008";
				msg4 = msgRes.castCtrlCode(msg4, 1, msgRes.getMessage(2568));
				msg4 = msgRes.castCtrlCode(msg4, 2, msgRes.castCtrlCode(msgRes.getMessage(31), 1, reward.Num.ToString("N0")));
				break;
			case Constant.eMoney.Heart:
				sprite.spriteName = "UI_chara_00_020";
				msg4 = msgRes.castCtrlCode(msg4, 1, msgRes.getMessage(2570));
				msg4 = msgRes.castCtrlCode(msg4, 2, msgRes.castCtrlCode(msgRes.getMessage(28), 1, reward.Num.ToString("N0")));
				break;
			}
			getDialog.setup(msg4, null, null, true);
			yield return dialogManager.StartCoroutine(dialogManager.openDialog(getDialog));
			if (bLimitOver)
			{
				DialogLimitOver limitOverDialog = dialogManager.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
				yield return dialogManager.StartCoroutine(limitOverDialog.show(reward.RewardType));
			}
			while (getDialog.isOpen())
			{
				yield return null;
			}
		}
	}

	private static WWW OnCreateGetWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		return WWWWrap.create("reward/info/");
	}

	private static WWW OnCreateAddWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("type", args["type"]);
		return WWWWrap.create("reward/add/");
	}

	public static void setupButton(Transform button, eType type)
	{
		bool flag = info_ != null && info_.type == (int)type;
		button.Find("icon_coin").gameObject.SetActive(flag);
		button.Find("Label_reward").gameObject.SetActive(flag);
		if (flag)
		{
			button.Find("Label").localPosition = new Vector3(-25f, 0f, -1f);
			UISprite component = button.Find("icon_coin").GetComponent<UISprite>();
			component.spriteName = iconName();
			component.transform.localScale = iconScale();
			UILabel component2 = button.Find("Label_reward").GetComponent<UILabel>();
			component2.text = numStr();
		}
		else
		{
			button.Find("Label").localPosition = Vector3.back;
		}
	}

	private static string iconName()
	{
		if (info_.coin > 0)
		{
			return "UI_icon_coin_00";
		}
		if (info_.heart > 0)
		{
			return "UI_icon_heart_00";
		}
		return "UI_icon_jewel_00";
	}

	private static Vector3 iconScale()
	{
		if (info_.coin > 0)
		{
			return new Vector3(39f, 44f, 1f);
		}
		if (info_.heart > 0)
		{
			return new Vector3(47.2f, 41.6f, 1f);
		}
		return new Vector3(40f, 46f, 1f);
	}

	private static string numStr()
	{
		MessageResource instance = MessageResource.Instance;
		string message = instance.getMessage(45);
		return instance.castCtrlCode(message, 1, rewardNum().ToString());
	}

	public static Constant.eMoney rewardType()
	{
		if (info_.coin > 0)
		{
			return Constant.eMoney.Coin;
		}
		if (info_.heart > 0)
		{
			return Constant.eMoney.Heart;
		}
		return Constant.eMoney.Jewel;
	}

	public static int rewardNum()
	{
		if (info_.coin > 0)
		{
			return info_.coin;
		}
		if (info_.heart > 0)
		{
			return info_.heart;
		}
		return info_.jewel;
	}
}
