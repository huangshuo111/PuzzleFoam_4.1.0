using UnityEngine;

public class ClearCheckBox : MonoBehaviour
{
	[SerializeField]
	private GameObject[] CheckBoxs;

	[SerializeField]
	private float LabelCenter = -30f;

	[SerializeField]
	private float LabelTop;

	[SerializeField]
	private float Center;

	[SerializeField]
	private float Top;

	[SerializeField]
	private float Bottom;

	public void setup(StageInfo.CommonInfo info, int clearState, UILabel targetLabel)
	{
		int num = 0;
		if (info.Score > 0)
		{
			num++;
		}
		if (info.IsAllDelete || info.IsFriendDelete || info.IsFulcrumDelete || info.IsMinilenDelete)
		{
			num++;
		}
		if (num == 1)
		{
			setPos(targetLabel.gameObject, LabelCenter);
		}
		else
		{
			setPos(targetLabel.gameObject, LabelTop);
		}
		for (int i = 0; i < CheckBoxs.Length; i++)
		{
			GameObject gameObject = CheckBoxs[i];
			gameObject.SetActive(i < num);
			if (num == 1)
			{
				setCheck(gameObject, false);
				setPos(gameObject, Center);
			}
			else
			{
				setCheck(gameObject, (i != 0) ? isOtherClear(info, clearState) : isScoreClear(clearState));
				setPos(gameObject, (i != 0) ? Bottom : Top);
			}
		}
	}

	private bool isScoreClear(int clearState)
	{
		return (clearState & 1) != 0;
	}

	private bool isOtherClear(StageInfo.CommonInfo info, int clearState)
	{
		if (info.IsAllDelete)
		{
			return (clearState & 2) != 0;
		}
		if (info.IsFriendDelete)
		{
			return (clearState & 4) != 0;
		}
		if (info.IsFulcrumDelete)
		{
			return (clearState & 8) != 0;
		}
		return false;
	}

	private void setPos(GameObject obj, float offset)
	{
		Vector3 localPosition = obj.transform.localPosition;
		localPosition.y = offset;
		obj.transform.localPosition = localPosition;
	}

	private void setCheck(GameObject obj, bool bCheck)
	{
		obj.transform.Find("Checkmark").gameObject.SetActive(bCheck);
	}
}
