using UnityEngine;

public class MenuParkStage : MonoBehaviour
{
	private Vector3 basePos_;

	private Transform map_;

	private Vector3 prevMapPos_;

	private void Start()
	{
		basePos_ = base.transform.localPosition;
		base.transform.localPosition = basePos_;
	}

	private void Update()
	{
		if (map_ == null)
		{
			Part_Park part_Park = Object.FindObjectOfType<Part_Park>();
			if (!part_Park)
			{
				return;
			}
			map_ = part_Park.transform.Find("ParkMapCamera(Clone)");
			if (map_ == null)
			{
				return;
			}
			prevMapPos_ = map_.localPosition;
		}
		base.transform.localPosition += (prevMapPos_ - map_.localPosition) * 0.05f;
		prevMapPos_ = map_.localPosition;
		Vector3 vector = basePos_ - base.transform.localPosition;
		vector.z = 0f;
		float num = Time.deltaTime;
		if (num > 1f)
		{
			num = 1f;
		}
		base.transform.localPosition += vector * num;
	}

	public void Reposition()
	{
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		base.transform.localPosition = localPosition;
	}
}
