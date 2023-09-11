using System.Collections;
using UnityEngine;

public class DialogBonusRouletteCancel : DialogBase
{
	private UIButton[] buttons = new UIButton[2];

	private UILabel label;

	private int addCoins;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public override void OnCreate()
	{
		Transform transform = base.transform.Find("window/Close_Button");
		if (transform != null)
		{
			buttons[0] = transform.GetComponent<UIButton>();
		}
		transform = base.transform.Find("window/ConfirmButton");
		if (transform != null)
		{
			buttons[1] = transform.GetComponent<UIButton>();
		}
		transform = base.transform.Find("window/Dialog_Label");
		if (transform != null)
		{
			label = transform.GetComponent<UILabel>();
		}
	}

	public void setup(int _coin)
	{
		addCoins = _coin;
		label.text = "ルーレットを回さずに\n" + addCoins + "コインを受け取ります。";
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
