using System.Collections;

public class DebugMenuSaveEventStageData : DebugMenuBase
{
	private enum eItem
	{
		All = 0,
		Reset = 1,
		EventNo = 2,
		Level = 3,
		Star = 4,
		Clear = 5,
		FirstClear = 6,
		Score = 7,
		ShowIcon = 8,
		Max = 9
	}

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(9, "EventData"));
	}

	public override void OnDraw()
	{
	}

	public override void OnExecute()
	{
	}
}
