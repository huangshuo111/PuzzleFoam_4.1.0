using UnityEngine;

public class Ceiling : MonoBehaviour
{
	private Transform me;

	private Transform target;

	private void Start()
	{
		me = base.transform;
	}

	private void LateUpdate()
	{
		if (target != null)
		{
			Vector3 position = me.position;
			position.y = target.position.y;
			me.position = position;
			me.localPosition += Vector3.down * 19f;
		}
	}

	public void setTarget(Transform t)
	{
		me = base.transform;
		target = t;
		LateUpdate();
	}
}
