using System.Collections;

public class DebugMenuSaveUserData : DebugMenuBase
{
	private enum eItem
	{
		Reset = 0,
		Stage = 1,
		Level = 2,
		Exp = 3,
		Coin = 4,
		BuyJewel = 5,
		BonusJewel = 6,
		Heart = 7,
		Treasure = 8,
		AllClear = 9,
		Max = 10
	}

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(10, "UserData"));
	}

	public override void OnDraw()
	{
	}

	public override void OnExecute()
	{
	}
}
