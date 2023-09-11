using System.Collections;
using UnityEngine;

public class DialogCredit : DialogBase
{
	private UILabel mCreditLabel;

	private UIDraggablePanel mDragPanel;

	public override void OnCreate()
	{
		mDragPanel = base.gameObject.transform.GetComponentInChildren<UIDraggablePanel>();
		mCreditLabel = base.gameObject.transform.Find("credit").GetComponentInChildren<UILabel>();
		mCreditLabel.text = string.Empty;
	}

	public void setup()
	{
		mCreditLabel.text = MessageResource.Instance.getMessage(500001);
		BoxCollider componentInChildren = base.gameObject.transform.Find("credit").GetComponentInChildren<BoxCollider>();
		Vector3 vector2 = (componentInChildren.size = new Vector3(mCreditLabel.relativeSize.x, mCreditLabel.relativeSize.y, 0f));
		componentInChildren.center = new Vector3(0f, (0f - vector2.y) / 2f, 0f);
	}

	public IEnumerator show()
	{
		Sound.Instance.playSe(Sound.eSe.SE_220_count);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		setup();
		mDragPanel.ResetPosition();
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			mCreditLabel.text = string.Empty;
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}
}
