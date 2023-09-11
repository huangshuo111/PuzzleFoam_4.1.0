using System.Collections;
using Network;
using UnityEngine;

public class DialogAttendCheck : DialogBase
{
	private GameObject ObjectDay_1;

	private GameObject ObjectDay_2;

	private GameObject ObjectDay_3;

	private GameObject ObjectDay_4;

	private GameObject ObjectDay_5;

	private GameObject ObjectDay_6;

	private GameObject ObjectDay_7;

	private GameObject ObjectAttendDay;

	private AttendanceConfig AttendanceInfo;

	private float AttendSize = 2000f;

	private float MinSize = 70f;

	private float MinSize7 = 145f;

	private void Start()
	{
	}

	private void Update()
	{
		if (!isOpen() || AttendanceInfo == null || !(ObjectAttendDay != null))
		{
			return;
		}
		AttendSize -= Time.deltaTime * 6000f;
		if (AttendanceInfo.attendDay == 7)
		{
			if (MinSize7 > AttendSize)
			{
				AttendSize = MinSize7;
			}
		}
		else if (MinSize > AttendSize)
		{
			AttendSize = MinSize;
		}
		ObjectAttendDay.transform.Find("bg_clamp").gameObject.transform.localScale = new Vector3(AttendSize, AttendSize, 1f);
	}

	public override void OnCreate()
	{
		ObjectDay_1 = base.transform.Find("window/Day1").gameObject;
		ObjectDay_2 = base.transform.Find("window/Day2").gameObject;
		ObjectDay_3 = base.transform.Find("window/Day3").gameObject;
		ObjectDay_4 = base.transform.Find("window/Day4").gameObject;
		ObjectDay_5 = base.transform.Find("window/Day5").gameObject;
		ObjectDay_6 = base.transform.Find("window/Day6").gameObject;
		ObjectDay_7 = base.transform.Find("window/Day7").gameObject;
	}

	public void setup(AttendanceConfig _AttendanceInfo)
	{
		AttendanceInfo = _AttendanceInfo;
		SetRewardInfo(ObjectDay_1, _AttendanceInfo.rewardList[0], 1, AttendanceInfo.attendDay);
		SetRewardInfo(ObjectDay_2, _AttendanceInfo.rewardList[1], 2, AttendanceInfo.attendDay);
		SetRewardInfo(ObjectDay_3, _AttendanceInfo.rewardList[2], 3, AttendanceInfo.attendDay);
		SetRewardInfo(ObjectDay_4, _AttendanceInfo.rewardList[3], 4, AttendanceInfo.attendDay);
		SetRewardInfo(ObjectDay_5, _AttendanceInfo.rewardList[4], 5, AttendanceInfo.attendDay);
		SetRewardInfo(ObjectDay_6, _AttendanceInfo.rewardList[5], 6, AttendanceInfo.attendDay);
		SetRewardInfo(ObjectDay_7, _AttendanceInfo.rewardList[6], 7, AttendanceInfo.attendDay);
	}

	private void SetRewardInfo(GameObject _ObjectDay, AttendanceReward _AttendanceReward, int _Day, int _AttendDay)
	{
		_ObjectDay.transform.Find("reward/reward" + _AttendanceReward.rewardImageId).gameObject.SetActive(true);
		UILabel component = _ObjectDay.transform.Find("Label").GetComponent<UILabel>();
		string message = MessageResource.Instance.getMessage(700000 + _AttendanceReward.rewardType);
		component.text = MessageResource.Instance.castCtrlCode(message, 1, _AttendanceReward.rewardValue.ToString());
		if (_Day <= _AttendDay)
		{
			_ObjectDay.transform.Find("bg_clamp").gameObject.SetActive(true);
		}
		if (_Day == _AttendDay)
		{
			ObjectAttendDay = _ObjectDay;
		}
	}

	private void SetClampInfo(int _AttendDay)
	{
	}

	private IEnumerator OnButton(GameObject trig)
	{
		switch (trig.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}
}
