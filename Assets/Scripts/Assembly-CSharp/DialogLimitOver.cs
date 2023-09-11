using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogLimitOver : DialogBase
{
	private Dictionary<Constant.eMoney, GameObject> labelDict_ = new Dictionary<Constant.eMoney, GameObject>();

	private UISysFontLabel sysLabel_;

	public override void OnCreate()
	{
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			switch (transform.name)
			{
			case "HeartDialog_Label":
				labelDict_[Constant.eMoney.Heart] = transform.gameObject;
				break;
			case "JewelDialog_Label":
				labelDict_[Constant.eMoney.Jewel] = transform.gameObject;
				break;
			case "CoinDialog_Label":
				labelDict_[Constant.eMoney.Coin] = transform.gameObject;
				break;
			case "ItemDialog_Label":
				labelDict_[Constant.eMoney.Ticket] = transform.gameObject;
				break;
			}
		}
	}

	public IEnumerator show(Constant.eMoney money)
	{
		foreach (GameObject value in labelDict_.Values)
		{
			value.SetActive(false);
		}
		labelDict_[money].SetActive(true);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
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
}
