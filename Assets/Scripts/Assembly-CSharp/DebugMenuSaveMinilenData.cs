using System.Collections;

public class DebugMenuSaveMinilenData : DebugMenuBase
{
	private enum eItem
	{
		MinilenID = 0,
		MinilenLevel = 1,
		DropRateUp = 2,
		Max = 3
	}

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(3, "MinilenData"));
	}

	public override void OnDraw()
	{
	}

	public override void OnExecute()
	{
	}
}
