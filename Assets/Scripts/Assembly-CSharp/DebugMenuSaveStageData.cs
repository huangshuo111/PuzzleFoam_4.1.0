using System.Collections;

public class DebugMenuSaveStageData : DebugMenuBase
{
	private enum eItem
	{
		All = 0,
		Reset = 1,
		Stage = 2,
		Star = 3,
		Clear = 4,
		ClearCount = 5,
		PlayCount = 6,
		Score = 7,
		Max = 8
	}

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(8, "StageData"));
	}

	public override void OnDraw()
	{
	}

	public override void OnExecute()
	{
	}
}
