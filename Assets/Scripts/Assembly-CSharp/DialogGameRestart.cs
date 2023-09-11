using System.Collections;
using UnityEngine;

public class DialogGameRestart : DialogBase
{
	private void Awake()
	{
	}

	public void OnCreate(int resultCode)
	{
		base.transform.localPosition = Vector3.back * 500f;
		Object.Destroy(base.transform.Find("window/Close_Button").gameObject);
		base.transform.Find("window/ConfirmButton/Label").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(2401);
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		Constant.SoundUtil.PlayDecideSE();
		yield return dialogManager_.StartCoroutine(close());
	}
}
