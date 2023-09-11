using System.Collections;
using UnityEngine;

public class DebugMenuSaveNetworkData : DebugMenuBase
{
	private enum eItem
	{
		EventNo = 0,
		EventStageNo = 1,
		ResourceVer = 2,
		RankingDate = 3,
		RecovaryID = 4,
		SessionID = 5,
		Max = 6
	}

	private SaveNetworkData netData_;

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(6, "NetworkData"));
		if (SaveData.IsInstance())
		{
			GameObject dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
			if (!(dataTbl == null))
			{
				netData_ = SaveData.Instance.getGameData().getNetworkData();
			}
		}
	}

	public override void OnDraw()
	{
		if (netData_ != null)
		{
			DrawItem(0, "EventNo : " + netData_.getEventNo());
			DrawItem(1, "EventStageNo : " + netData_.getEventStageNo());
			DrawItem(2, "ResourceVer : " + netData_.getResourceVersion(), eItemType.TextField);
			DrawItem(3, "RankingDate : " + netData_.getRankingDateStr(), eItemType.CenterOnly);
			DrawItem(4, "RecovaryID : " + netData_.getRecovaryID().ToString(), eItemType.CenterOnly);
			DrawItem(5, "SessionID: " + netData_.getSessionID().ToString(), eItemType.CenterOnly);
		}
	}

	public override void OnExecute()
	{
		if (netData_ != null)
		{
			int num = 0;
			num = (int)Vary(0, netData_.getEventNo(), 1, 0, 100);
			if (num != netData_.getEventNo())
			{
				netData_.setEventNo(num);
				netData_.save();
			}
			num = (int)Vary(1, netData_.getEventStageNo(), 1, 0, 100);
			if (num != netData_.getEventStageNo())
			{
				netData_.setEventStageNo(num);
				netData_.save();
			}
			netData_.setResourceVersion(VaryString(2, netData_.getResourceVersion()));
			if (IsPressCenterButton(3))
			{
				netData_.resetRankingDate();
				netData_.save();
			}
			if (IsPressCenterButton(4))
			{
				netData_.resetRecoverySaveDate();
				netData_.save();
			}
			if (IsPressCenterButton(5))
			{
				netData_.setSessionID(string.Empty, true);
			}
		}
	}
}
