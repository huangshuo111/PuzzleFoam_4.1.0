using UnityEngine;

public class Snake : MonoBehaviour
{
	private Transform sprite_tran;

	private void Start()
	{
		sprite_tran = base.transform.Find("snake");
	}

	private void Update()
	{
	}

	private void setRotateY(float rotY)
	{
		Quaternion localRotation = sprite_tran.localRotation;
		localRotation.y = rotY;
		sprite_tran.localRotation = localRotation;
	}
}
