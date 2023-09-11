using System.Collections;
using UnityEngine;

public class DialogLowMemory : DialogBase
{
	private void Awake()
	{
	}

	public override void OnCreate()
	{
		base.transform.localPosition = Vector3.back * 500f;
		Object.Destroy(base.transform.Find("window/Close_Button").gameObject);
		base.transform.Find("window/Dialog_Label").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(1479);
		base.transform.Find("window/ConfirmButton/Label").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(2401);
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		yield return dialogManager_.StartCoroutine(close());
	}
}
