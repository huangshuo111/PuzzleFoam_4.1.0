using System.Collections;

public class DebugMenuSaveTreasureData : DebugMenuBase
{
	private enum eItem
	{
		All = 0,
		Reset = 1,
		ID = 2,
		Open = 3,
		Max = 4
	}

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(4, "TreasureData"));
	}

	public override void OnDraw()
	{
	}

	public override void OnExecute()
	{
	}
}
