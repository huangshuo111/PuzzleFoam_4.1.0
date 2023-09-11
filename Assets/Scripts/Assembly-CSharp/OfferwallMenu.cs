using UnityEngine;

public class OfferwallMenu : MonoBehaviour
{
	private Vector3 basePos_;

	private Vector3 prevMapPos_;

	private Transform map_;

	private Vector3 ButtonPosition;

	private int eventNum;

	private bool BBActive;

	private long nfreeAdAdViewTime;

	private int nfreeAdChargeMinute;

	public bool ADReady { get; private set; }

	private void Start()
	{
		basePos_ = base.transform.localPosition;
		ADReady = false;
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
			base.gameObject.transform.Find("OfferwallButton").gameObject.SetActive(false);
		}
		else
		{
			base.gameObject.transform.Find("OfferwallButton").gameObject.SetActive(true);
		}
	}

	public void SetCrossBBActive(bool _BBActive, bool _EventActive, PartManager.ePart part)
	{
		Debug.Log(" _BBActive : " + _BBActive);
		Debug.Log(" _EventActive : " + _EventActive);
		BBActive = _BBActive;
		ButtonPosition.x = -255f;
		ButtonPosition.z = 0f;
		if (BBActive && part == PartManager.ePart.Map)
		{
			Debug.Log(" BBActive : " + BBActive);
			ButtonPosition.y = 110f;
			if (!_EventActive)
			{
				ButtonPosition.y = 215f;
			}
		}
		else
		{
			Debug.Log(" BBActive : " + BBActive);
			ButtonPosition.y = 215f;
			if (!_EventActive)
			{
				ButtonPosition.y = 330f;
			}
		}
		base.gameObject.transform.Find("OfferwallButton").gameObject.transform.localPosition = ButtonPosition;
		basePos_ = base.transform.localPosition;
	}
}
