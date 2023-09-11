using System.Collections;
using UnityEngine;

public class DialogDailyMission : DialogBase
{
	private UILabel label;

	private UILabel cautionLabel;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public override void OnCreate()
	{
		Transform transform = base.transform.Find("window/DailyMission_Label");
		if (transform != null)
		{
			label = transform.GetComponent<UILabel>();
		}
		transform = base.transform.Find("window/DailyMission_Label_2");
		if (transform != null)
		{
			cautionLabel = transform.GetComponent<UILabel>();
		}
	}

	public void setup(int missionNum, int missionTarget)
	{
		int messageID = 4600 + (missionNum - 1);
		int messageID2 = 4800 + (missionNum - 1);
		string message = MessageResource.Instance.getMessage(messageID);
		message = MessageResource.Instance.castCtrlCode(message, 1, missionTarget.ToString());
		label.text = string.Empty + message;
		cautionLabel.text = MessageResource.Instance.getMessage(messageID2);
		for (int i = 0; i < 9; i++)
		{
			GameObject gameObject = base.transform.Find("window/condition_icon/0" + i).gameObject;
			if (i == missionNum - 1)
			{
				gameObject.SetActive(true);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}

	private IEnumerator OnButton(GameObject trig)
	{
		switch (trig.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "ConfirmButton":
			Constant.SoundUtil.PlayDecideSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}
}
