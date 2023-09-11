using System.Collections;

public class DebugMenuPayment : DebugMenuBase
{
	private enum eItem
	{
		ProductID = 0,
		Payment = 1,
		ItemDelivery = 2,
		Max = 3
	}

	private string productID_ = string.Empty;

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(3, "Payment"));
		SetDefaultText(0, productID_);
	}

	public override void OnDraw()
	{
		DrawItem(0, string.Empty, eItemType.TextField);
		DrawItem(1, "Payment", eItemType.CenterOnly);
		DrawItem(2, "ItemDelivery", eItemType.CenterOnly);
	}

	public override void OnExecute()
	{
		productID_ = VaryString(0, productID_);
		if (IsPressCenterButton(1))
		{
			payment();
		}
		if (IsPressCenterButton(2))
		{
			itemDelivery();
		}
	}

	private void payment()
	{
	}

	private void itemDelivery()
	{
	}
}
