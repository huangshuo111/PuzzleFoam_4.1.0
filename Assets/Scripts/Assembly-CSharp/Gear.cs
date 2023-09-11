using UnityEngine;

public class Gear : MonoBehaviour
{
	public Transform arrow;

	private void Start()
	{
	}

	private void Update()
	{
		Vector3 localEulerAngles = arrow.localEulerAngles;
		if (base.name.Contains("_l"))
		{
			localEulerAngles.z = 360f - localEulerAngles.z;
		}
		base.transform.localEulerAngles = localEulerAngles;
	}
}
