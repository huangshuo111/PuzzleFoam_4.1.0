using System;
using UnityEngine;

public class ADMenu : MonoBehaviour
{
	private Vector3 basePos_;

	private Vector3 prevMapPos_;

	private Transform map_;

	private Vector3 ADButtonPosition;

	private int eventNum;

	private bool BBActive;

	private UILabel TimerLabel;

	private long nfreeAdAdViewTime;

	private int nfreeAdChargeMinute;

	private static bool bServerOn;

	public bool ADReady { get; private set; }

	private void Start()
	{
		basePos_ = base.transform.localPosition;
		TimerLabel = base.gameObject.transform.Find("AdButton/energyLabel").GetComponent<UILabel>();
		ADReady = false;
		if (GlobalData.Instance.getGameData().monetization == null)
		{
			bServerOn = false;
			base.gameObject.transform.Find("AdButton").gameObject.SetActive(false);
			Debug.Log(" (GlobalData.Instance.getGameData().monetization == null)");
		}
		else
		{
			bServerOn = true;
			base.gameObject.transform.Find("AdButton").gameObject.SetActive(true);
		}
	}

	private void Update()
	{
		if (GlobalData.Instance.getGameData().monetization != null)
		{
			nfreeAdAdViewTime = GlobalData.Instance.getGameData().monetization.freeAdAdViewTime;
			nfreeAdChargeMinute = GlobalData.Instance.getGameData().monetization.freeAdChargeMinute;
		}
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
		if (GlobalData.Instance.getGameData() == null)
		{
			return;
		}
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		int num2 = (int)((double)nfreeAdAdViewTime - (DateTime.UtcNow - dateTime).TotalMilliseconds) / 1000;
		num2 = num2 + nfreeAdChargeMinute * 60 + 10;
		int num3 = num2 / 3600;
		int num4 = num2 % 3600 / 60;
		int num5 = num2 % 60;
		if (num2 < 0)
		{
			TimerLabel.transform.localScale = new Vector3(21f, 21f, 1f);
			TimerLabel.text = "선물받기";
			ADReady = true;
			return;
		}
		if (num2 < 60)
		{
			num4 = 1;
		}
		TimerLabel.transform.localScale = new Vector3(26f, 26f, 1f);
		TimerLabel.text = num3.ToString("00") + ":" + num4.ToString("00");
		ADReady = false;
	}

	public void updateEnable(PartManager.ePart part)
	{
		if (!bServerOn)
		{
			base.gameObject.transform.Find("AdButton").gameObject.SetActive(false);
			Debug.Log("if (bServerOn == false)");
			return;
		}
		base.gameObject.transform.Find("AdButton").gameObject.SetActive(true);
		if (part != PartManager.ePart.Map)
		{
			base.gameObject.transform.Find("AdButton").gameObject.SetActive(false);
			Debug.Log(" (part != PartManager.ePart.Map)");
		}
	}

	public void SetCrossBBActive(bool _BBActive, bool _EventActive, PartManager.ePart part)
	{
		if (GlobalData.Instance.getGameData().monetization == null)
		{
			return;
		}
		BBActive = _BBActive;
		ADButtonPosition.x = -255f;
		ADButtonPosition.z = 0f;
		if (BBActive && part == PartManager.ePart.Map)
		{
			Debug.Log(" BBActive : " + BBActive);
			ADButtonPosition.y = 130f;
			if (!_EventActive)
			{
				ADButtonPosition.y = 235f;
			}
		}
		else
		{
			Debug.Log(" BBActive : " + BBActive);
			ADButtonPosition.y = 235f;
			if (!_EventActive)
			{
				ADButtonPosition.y = 350f;
			}
		}
		base.gameObject.transform.Find("AdButton").gameObject.transform.localPosition = ADButtonPosition;
		basePos_ = base.transform.localPosition;
	}
}
