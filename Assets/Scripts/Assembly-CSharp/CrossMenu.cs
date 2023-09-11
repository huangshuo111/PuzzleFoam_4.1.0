using UnityEngine;

public class CrossMenu : MonoBehaviour
{
	private Vector3 basePos_;

	private Vector3 prevMapPos_;

	private Transform map_;

	private int eventNum;

	private bool BBActive;

	private void Start()
	{
		basePos_ = base.transform.localPosition;
	}

	private void Update()
	{
		if (map_ == null)
		{
			map_ = base.transform.parent.parent.Find("DragCamera(Clone)/DragObject/MapCamera");
			if (map_ != null)
			{
				prevMapPos_ = map_.localPosition;
			}
		}
		if (map_ != null)
		{
			base.transform.localPosition += (prevMapPos_ - map_.localPosition) * 0.5f;
			prevMapPos_ = map_.localPosition;
		}
		Vector3 vector = basePos_ - base.transform.localPosition;
		vector.z = 0f;
		float num = Time.deltaTime;
		if (num > 1f)
		{
			num = 1f;
		}
		base.transform.localPosition += vector * num;
	}

	public void updateEnable(PartManager.ePart part)
	{
		if (part != PartManager.ePart.Map)
		{
			base.gameObject.transform.Find("CrossButton").gameObject.SetActive(false);
		}
	}

	public void SetCrossBBActive(bool _BBActive, PartManager.ePart part)
	{
		BBActive = _BBActive;
		if (BBActive)
		{
			if (part == PartManager.ePart.Map)
			{
				Vector3 localPosition = default(Vector3);
				localPosition.x = -255f;
				localPosition.y = 345f;
				localPosition.z = 0f;
				base.gameObject.transform.Find("CrossButton").gameObject.transform.localPosition = localPosition;
				base.gameObject.transform.Find("CrossButton").gameObject.SetActive(true);
			}
		}
		else
		{
			base.gameObject.transform.Find("CrossButton").gameObject.SetActive(false);
		}
	}
}
