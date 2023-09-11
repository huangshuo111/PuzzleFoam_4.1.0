using UnityEngine;

public class Slave : MonoBehaviour
{
	private Transform me;

	public Transform target;

	private void Start()
	{
		me = base.transform;
	}

	private void LateUpdate()
	{
		forceUpdate();
	}

	public void forceUpdate()
	{
		if (!(target == null))
		{
			Vector3 position = me.position;
			position.x = target.position.x;
			position.y = target.position.y;
			me.position = position;
			me.localRotation = target.localRotation;
		}
	}
}
