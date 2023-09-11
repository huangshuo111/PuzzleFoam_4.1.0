using System.Collections;
using System.Collections.Generic;
using Bridge;
using UnityEngine;

public class DialogPlayerInfo : DialogBase
{
	private UILabel[] labels = new UILabel[5];

	private UIButton[] buttons = new UIButton[3];

	private Texture2D tex;

	private PlayerIcon icon;

	private int userCount;

	private int userPage;

	private List<UserData> sameStageUser = new List<UserData>();

	private int stageMax;

	private int treasureMax;

	private string latestName;

	private UISysFontLabel sysLabel_;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public override void OnCreate()
	{
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		StageIconDataTable component = @object.GetComponent<StageIconDataTable>();
		stageMax = component.getMaxStageIconsNum();
		treasureMax = component.getMaxBoxNum();
		Transform transform = base.transform.Find("window/PlayerInfoFont_Label");
		if (transform != null)
		{
			labels[0] = transform.GetComponent<UILabel>();
		}
		transform = base.transform.Find("window/User1/Name");
		if (transform != null)
		{
			labels[1] = transform.GetComponent<UILabel>();
		}
		transform = base.transform.Find("window/User1/Star_Label");
		if (transform != null)
		{
			labels[2] = transform.GetComponent<UILabel>();
		}
		transform = base.transform.Find("window/Stage/Stage_Label");
		if (transform != null)
		{
			labels[3] = transform.GetComponent<UILabel>();
		}
		transform = base.transform.Find("window/Treasure/Treasure_Label");
		if (transform != null)
		{
			labels[4] = transform.GetComponent<UILabel>();
		}
		transform = base.transform.Find("window/Close_Button");
		if (transform != null)
		{
			buttons[0] = transform.GetComponent<UIButton>();
		}
		transform = base.transform.Find("Arrow/arrow_00");
		if (transform != null)
		{
			buttons[1] = transform.GetComponent<UIButton>();
		}
		transform = base.transform.Find("Arrow/arrow_01");
		if (transform != null)
		{
			buttons[2] = transform.GetComponent<UIButton>();
		}
		transform = base.transform.Find("window/User1/UserIcon");
		if (transform != null)
		{
			icon = transform.GetComponent<PlayerIcon>();
		}
	}

	public void setup_friend(List<UserData> _data, int _stage, string _name)
	{
		userCount = 0;
		sameStageUser.Clear();
		int num = 0;
		int num2 = 0;
		num = ((_stage <= stageMax) ? _stage : stageMax);
		foreach (UserData _datum in _data)
		{
			if (_datum == null)
			{
				break;
			}
			num2 = ((_datum.StageNo <= stageMax) ? _datum.StageNo : stageMax);
			if (num == num2)
			{
				sameStageUser.Add(_datum);
				userCount++;
			}
		}
		if (userCount == 1)
		{
			buttons[1].gameObject.SetActive(false);
			buttons[2].gameObject.SetActive(false);
			DialogDataSet_friend(sameStageUser, 0);
			return;
		}
		buttons[1].gameObject.SetActive(true);
		buttons[2].gameObject.SetActive(true);
		for (int i = 0; i < userCount; i++)
		{
			if (sameStageUser[i].UserName.Equals(_name))
			{
				userPage = i;
				break;
			}
		}
		if (userPage == 0)
		{
			buttons[1].gameObject.SetActive(false);
		}
		else if (userPage == userCount - 1)
		{
			buttons[2].gameObject.SetActive(false);
		}
		DialogDataSet_friend(sameStageUser, userPage);
	}

	private void DialogDataSet_friend(List<UserData> _data, int _page)
	{
		if (_data[_page] != null)
		{
			labels[1].gameObject.SetActive(true);
			labels[1].text = string.Empty + _data[_page].UserName;
			latestName = Constant.UserName.ReplaceOverStr(labels[1]);
			labels[1].text = latestName;
			if (sysLabel_ != null)
			{
				Object.DestroyImmediate(sysLabel_.gameObject);
			}
			Utility.createSysLabel(labels[1], ref sysLabel_);
			sysLabel_.FontSize = (int)(labels[1].transform.localScale.y * 0.87f);
			labels[2].text = string.Empty + _data[_page].allStarSum;
			if (_data[_page].StageNo > stageMax)
			{
				labels[3].text = string.Empty + stageMax + "/" + stageMax;
			}
			else
			{
				labels[3].text = string.Empty + _data[_page].StageNo + "/" + stageMax;
			}
			labels[4].text = string.Empty;
			if (_data[_page].Texture != null)
			{
				tex = _data[_page].Texture;
			}
			else
			{
				tex = Resources.Load("materials/user_00", typeof(Texture2D)) as Texture2D;
			}
			icon.createIcon(tex);
		}
	}

	public void setup_player(UserData _data)
	{
		labels[1].text = string.Empty + _data.UserName;
		latestName = Constant.UserName.ReplaceOverStr(labels[1]);
		labels[1].text = latestName;
		Utility.createSysLabel(labels[1], ref sysLabel_);
		sysLabel_.FontSize = (int)(labels[1].transform.localScale.y * 0.87f);
		labels[2].text = string.Empty + Bridge.StageData.getTotalStar();
		if (PlayerData.getCurrentStage() >= stageMax)
		{
			labels[3].text = string.Empty + PlayerData.getCurrentStage() + "/" + stageMax;
		}
		else
		{
			labels[3].text = string.Empty + (PlayerData.getCurrentStage() + 1) + "/" + stageMax;
		}
		labels[4].text = string.Empty + PlayerData.getTreasureNum() + "/" + treasureMax;
		buttons[1].gameObject.SetActive(false);
		buttons[2].gameObject.SetActive(false);
		if (_data.Texture != null)
		{
			tex = _data.Texture;
		}
		else
		{
			tex = Resources.Load("materials/user_00", typeof(Texture2D)) as Texture2D;
		}
		icon.createIcon(tex);
	}

	private IEnumerator OnButton(GameObject trig)
	{
		switch (trig.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			userPage = 0;
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "arrow_00":
			userPage--;
			if (userPage == 0)
			{
				buttons[1].gameObject.SetActive(false);
			}
			if (userPage != userCount - 1)
			{
				buttons[2].gameObject.SetActive(true);
			}
			DialogDataSet_friend(sameStageUser, userPage);
			break;
		case "arrow_01":
			userPage++;
			if (userPage == userCount - 1)
			{
				buttons[2].gameObject.SetActive(false);
			}
			if (userPage != 0)
			{
				buttons[1].gameObject.SetActive(true);
			}
			DialogDataSet_friend(sameStageUser, userPage);
			break;
		}
	}
}
