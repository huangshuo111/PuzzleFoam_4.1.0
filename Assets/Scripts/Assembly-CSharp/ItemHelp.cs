using UnityEngine;

public class ItemHelp : BoostItemBase
{
	[SerializeField]
	private UILabel HelpLabel;

	[SerializeField]
	private UILabel NameLabel;

	protected override void OnAwake()
	{
	}

	public override void setup(Constant.Item.eType item, int num)
	{
		base.setup(item, num);
		NameLabel.text = msgRes_.getMessage((int)(item - 1 + 1000));
		string text = msgRes_.getMessage((int)(item - 1 + 1200));
		if (msgRes_.isCtrlCode(text, 1))
		{
			text = msgRes_.castCtrlCode(text, 1, num.ToString());
		}
		HelpLabel.text = text;
		if (Utility.getStringLine(text) >= 4)
		{
			HelpLabel.transform.localScale = new Vector3(24f, 24f, 1f);
		}
		else
		{
			HelpLabel.transform.localScale = new Vector3(30f, 30f, 1f);
		}
	}
}
