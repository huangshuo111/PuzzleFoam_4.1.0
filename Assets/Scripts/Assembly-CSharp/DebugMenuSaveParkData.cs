using System.Collections;

public class DebugMenuSaveParkData : DebugMenuBase
{
	private enum eItem
	{
		MinilenCount = 0,
		MinilenTotalCount = 1,
		ResetMinilenThanks = 2,
		AddBuildingAll = 3,
		AddBuilding = 4,
		DeletePlacedData = 5,
		Max = 6
	}

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(6, "ParkData"));
	}

	public override void OnDraw()
	{
	}

	public override void OnExecute()
	{
	}
}
