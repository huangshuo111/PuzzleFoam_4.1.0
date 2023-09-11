using System.Collections;
using UnityEngine;

public class DialogConfirm : DialogBase
{
	private void Awake()
	{
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "ConfirmButton":
			Constant.SoundUtil.PlayDecideSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	public void setMessage(string message)
	{
		Transform transform = base.transform.Find("window/label");
		if (transform != null)
		{
			UILabel component = transform.GetComponent<UILabel>();
			component.text = message;
		}
	}

	public void setTitleEnable(bool enable)
	{
		Transform transform = base.transform.Find("window/title_plate");
		Transform transform2 = base.transform.Find("window/Title_boxget");
		if (transform != null)
		{
			transform.gameObject.SetActive(enable);
		}
		if (transform2 != null)
		{
			transform2.gameObject.SetActive(enable);
		}
	}
}
