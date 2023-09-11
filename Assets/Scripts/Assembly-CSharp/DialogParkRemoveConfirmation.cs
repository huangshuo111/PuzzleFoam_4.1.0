using System.Collections;
using UnityEngine;

public class DialogParkRemoveConfirmation : DialogBase
{
	private Building selectedObject_;

	private bool isRemove_;

	public void setSelectedObject(Building building)
	{
		selectedObject_ = building;
	}

	public override void OnOpen()
	{
		isRemove_ = false;
		dialogManager_.addActiveDialogList(DialogManager.eDialog.ParkRemoveConfirm);
	}

	public override void OnClose()
	{
		dialogManager_.removeActiveDialogList(DialogManager.eDialog.ParkRemoveConfirm);
		if (isRemove_)
		{
			dialogManager_.StartCoroutine(destroyObject());
		}
	}

	private IEnumerator destroyObject()
	{
		ParkObjectManager objectManager = ParkObjectManager.Instance;
		Input.enable = false;
		SaveData.Instance.getParkData().RemoveBuildingData(selectedObject_);
		SaveData.Instance.getParkData().save();
		yield return objectManager.StartCoroutine(objectManager.SendMapUpdateForRemove(selectedObject_));
		dialogManager_.StartCoroutine(objectManager.buildingEffects.PlaySmokeEffects(selectedObject_.index, selectedObject_.gridSize, selectedObject_.direction));
		float smokeTime = 0.475f;
		yield return dialogManager_.StartCoroutine(selectedObject_.fadeOutAlpha(smokeTime, smokeTime));
		objectManager.Remove(selectedObject_, true);
		selectedObject_.OnRemove();
		Object.Destroy(selectedObject_.gameObject);
		selectedObject_ = null;
		Input.enable = true;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		if (selectedObject_ == null)
		{
			yield return StartCoroutine(close());
			yield break;
		}
		switch (trigger.name)
		{
		case "ConfirmButton":
			Constant.SoundUtil.PlayButtonSE();
			isRemove_ = true;
			yield return dialogManager_.StartCoroutine(close());
			break;
		case "CloseButton":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(close());
			break;
		}
	}
}
