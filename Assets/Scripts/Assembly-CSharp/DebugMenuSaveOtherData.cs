using System.Collections;
using UnityEngine;

public class DebugMenuSaveOtherData : DebugMenuBase
{
	private enum eItem
	{
		Reset = 0,
		StageNo = 1,
		EventStageNo = 2,
		Max = 3
	}

	private SaveOtherData otherData_;

	private int eventNo_;

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(3, "OtherData"));
		if (!SaveData.IsInstance())
		{
			yield break;
		}
		GameObject dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		if (dataTbl == null)
		{
			yield break;
		}
		StageDataTable stageTbl = dataTbl.GetComponent<StageDataTable>();
		if (!(stageTbl == null))
		{
			if (stageTbl.getEventData() != null)
			{
				eventNo_ = stageTbl.getEventData().EventNo;
			}
			otherData_ = SaveData.Instance.getGameData().getOtherData();
		}
	}

	public override void OnDraw()
	{
		if (otherData_ != null)
		{
			DrawItem(0, "Reset", eItemType.CenterOnly);
			DrawItem(1, "StageNo : " + otherData_.getStageNo());
			DrawItem(2, "EventStageNo : " + otherData_.getEventStageNo());
		}
	}

	public override void OnExecute()
	{
		if (otherData_ != null)
		{
			if (IsPressCenterButton(0))
			{
				otherData_.reset();
			}
			int num = 0;
			num = (int)Vary(1, otherData_.getStageNo(), 1, 0, 104);
			if (num != otherData_.getStageNo())
			{
				otherData_.setStageNo(num);
				otherData_.save();
			}
			num = (int)Vary(2, otherData_.getEventStageNo(), 1, Constant.Event.getLeastLevelStageNo(eventNo_), Constant.Event.getHighestLevelStageNo(eventNo_));
			if (num != otherData_.getEventStageNo())
			{
				otherData_.setEventStageNo(num);
				otherData_.save();
			}
		}
	}
}
