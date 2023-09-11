using System.Collections;
using UnityEngine;

public class DebugMenuPark : DebugMenuBase
{
	private enum eItem
	{
		Building = 0,
		ReleaseArea = 1,
		ResetReleaseArea = 2,
		DetailedLayout = 3,
		Max = 4
	}

	private ParkObjectManager objectManager_;

	private SaveParkData saveDataPark_;

	private int buildingInfoIndex_;

	private ParkBuildingInfo.BuildingInfo[] buildingInfos_;

	private int releaseAreaIndex_;

	private ParkAreaReleaseDataTable areaReleaseDataTable_;

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(4, "Park"));
		if (ParkObjectManager.haveInstance)
		{
			GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
			ParkBuildingDataTable buildingDataTable = dataTable.GetComponent<ParkBuildingDataTable>();
			areaReleaseDataTable_ = dataTable.GetComponent<ParkAreaReleaseDataTable>();
			if (!(buildingDataTable == null) && !(areaReleaseDataTable_ == null))
			{
				buildingDataTable.load();
				areaReleaseDataTable_.load();
				buildingInfos_ = buildingDataTable.getAllBuildingInfo();
				objectManager_ = ParkObjectManager.Instance;
				saveDataPark_ = SaveData.Instance.getParkData();
			}
		}
	}

	public override void OnDraw()
	{
		if (!(objectManager_ == null))
		{
			string message = MessageResource.Instance.getMessage(buildingInfos_[buildingInfoIndex_].NameID);
			DrawItem(0, "Building:\n" + message + "-" + buildingInfos_[buildingInfoIndex_].ID, eItemType.Default);
			DrawItem(1, "Unlock ReleaseArea:\n" + areaReleaseDataTable_.getInfo(releaseAreaIndex_ + 1).AreaName, eItemType.Default);
			DrawItem(2, "Reset Release Area", eItemType.CenterOnly);
			DrawItem(3, "Detailed Layout:\n" + objectManager_.enableDetaildLayout, eItemType.CenterOnly);
		}
	}

	public override void OnExecute()
	{
		if (!(objectManager_ == null))
		{
			OnExecuteBuilding();
			OnExecuteReleaseArea();
			if (IsPressCenterButton(3))
			{
				objectManager_.enableDetaildLayout = !objectManager_.enableDetaildLayout;
			}
		}
	}

	private void OnExecuteBuilding()
	{
		if (IsPressCenterButton(0))
		{
			objectManager_.createBuildingOnScreenCenter(buildingInfos_[buildingInfoIndex_].ID, -1);
		}
		if (IsPressLeftButton(0))
		{
			buildingInfoIndex_--;
			if (buildingInfoIndex_ < 0)
			{
				buildingInfoIndex_ = buildingInfos_.Length - 1;
			}
		}
		if (IsPressRightButton(0))
		{
			buildingInfoIndex_++;
			if (buildingInfoIndex_ >= buildingInfos_.Length)
			{
				buildingInfoIndex_ = 0;
			}
		}
	}

	private void OnExecuteReleaseArea()
	{
		if (IsPressCenterButton(1))
		{
			objectManager_.StartCoroutine(objectManager_.ReleaseLockedArea(areaReleaseDataTable_.getInfo(releaseAreaIndex_ + 1).areaID));
		}
		if (IsPressLeftButton(1))
		{
			releaseAreaIndex_--;
			if (releaseAreaIndex_ < 0)
			{
				releaseAreaIndex_ = areaReleaseDataTable_.dataCount - 1;
			}
		}
		if (IsPressRightButton(1))
		{
			releaseAreaIndex_++;
			if (releaseAreaIndex_ >= areaReleaseDataTable_.dataCount)
			{
				releaseAreaIndex_ = 0;
			}
		}
		if (IsPressCenterButton(2))
		{
			saveDataPark_.areaReleasedCount = 0;
			saveDataPark_.save();
		}
	}
}
