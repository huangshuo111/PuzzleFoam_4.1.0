using System.Collections;
using UnityEngine;

public class TransitGate : Building
{
	public override IEnumerator setup(int id)
	{
		yield return StartCoroutine(base.setup(id));
		base.enableOnClick = true;
		base.enableOnPress = false;
		base.enableOnRelease = false;
		base.enableOnLongPress = false;
		base.enableOnDragStart = false;
		base.enableOnDrag = false;
		base.enableOnDragEnd = false;
	}

	public override void setupImmediate(int id)
	{
		base.setupImmediate(id);
		base.enableOnClick = true;
		base.enableOnPress = false;
		base.enableOnRelease = false;
		base.enableOnLongPress = false;
		base.enableOnDragStart = false;
		base.enableOnDrag = false;
		base.enableOnDragEnd = false;
	}

	public override void OnClick(Vector3 inputPosition)
	{
		if (ParkObjectManager.Instance.selectedObject == null)
		{
			Constant.SoundUtil.PlayDecideSE();
			ParkObjectManager.Instance.StartDialogFriendList();
		}
	}

	public override void OnLongPress(Vector3 inputPosition)
	{
	}

	public override void OnRelease(Vector3 inputPosition)
	{
	}

	public override void OnDragStart(Vector3 inputPosition)
	{
	}

	public override void OnDrag(Vector3 inputPosition, Vector3 delta)
	{
	}

	public override void OnDragEnd(Vector3 inputPosition)
	{
	}
}
