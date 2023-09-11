using System;
using Bridge;
using Network;
using UnityEngine;

public class MenuMinilenThanks : MonoBehaviour
{
	[SerializeField]
	private UILabel labelCount_;

	[SerializeField]
	private GameObject _clear_notice_object;

	[SerializeField]
	private UILabel _clear_notice_label;

	private Vector3 basePos_;

	private Transform map_;

	private Vector3 prevMapPos_;

	private void Start()
	{
		basePos_ = base.transform.localPosition;
		base.transform.localPosition = basePos_;
	}

	public void UpdateCount()
	{
		if (!(labelCount_ == null))
		{
			int num = Bridge.PlayerData.getMinilenCount();
			if (num < 0)
			{
				num = 0;
			}
			labelCount_.text = num.ToString();
			Network.MinilenThanksData[] array = null;
			array = GlobalData.Instance.getGameData().thanksList;
			int num2 = Array.FindAll(array, (Network.MinilenThanksData thank) => thank.available).Length;
			if (num2 <= 0)
			{
				_clear_notice_object.SetActive(false);
				return;
			}
			_clear_notice_object.SetActive(true);
			_clear_notice_label.text = num2.ToString();
		}
	}

	private void Update()
	{
		if (map_ == null)
		{
			Part_Park part_Park = UnityEngine.Object.FindObjectOfType<Part_Park>();
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
