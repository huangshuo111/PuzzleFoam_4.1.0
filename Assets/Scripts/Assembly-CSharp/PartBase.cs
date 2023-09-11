using System.Collections;
using UnityEngine;

public abstract class PartBase : MonoBehaviour
{
	protected PartManager partManager;

	public GameObject uiRoot;

	protected DialogManager dialogManager;

	public void init(PartManager manager, DialogManager dialog)
	{
		partManager = manager;
		dialogManager = dialog;
		base.transform.parent = partManager.transform;
		uiRoot = Utility.createObject(base.gameObject.name + "_UI", manager.uiParent);
		dialogManager.setCurrenUiRoot(uiRoot);
	}

	public abstract IEnumerator setup(Hashtable args);

	protected void setupButton(GameObject obj)
	{
		setupButton(obj, false);
	}

	protected void setupButton(GameObject obj, bool includeInactive)
	{
		UIButtonMessage[] componentsInChildren = obj.GetComponentsInChildren<UIButtonMessage>(includeInactive);
		UIButtonMessage[] array = componentsInChildren;
		foreach (UIButtonMessage uIButtonMessage in array)
		{
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "OnButton";
		}
	}

	private void OnDestroy()
	{
		Object.Destroy(uiRoot);
	}

	public virtual IEnumerator OnDestroyCB()
	{
		yield break;
	}
}
