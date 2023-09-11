using System.Collections;
using UnityEngine;

public class DialogItemHelp : DialogBase
{
	private ItemHelp[] items_;

	public override void OnCreate()
	{
		items_ = GetComponentsInChildren<ItemHelp>(true);
	}

	public void setup(StageInfo.CommonInfo info)
	{
		for (int i = 0; i < items_.Length; i++)
		{
			ItemHelp itemHelp = items_[i];
			if (i >= info.ItemNum)
			{
				itemHelp.gameObject.SetActive(false);
				continue;
			}
			itemHelp.setup((Constant.Item.eType)info.Items[i].Type, info.Items[i].Num);
			itemHelp.gameObject.SetActive(true);
		}
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}
}
