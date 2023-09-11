using System.Collections;

public class DebugMenuSaveInfoData : DebugMenuBase
{
	private enum eItem
	{
		Index = 0,
		Reset = 1,
		ID = 2,
		Date = 3,
		Max = 4
	}

	private SaveInformationData infoData_;

	private string date_ = string.Empty;

	private int index_ = 1;

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(4, "InfoData"));
		if (SaveData.IsInstance())
		{
			infoData_ = getInfoData(index_);
			date_ = infoData_.getDateStr();
			SetDefaultText(3, date_);
		}
	}

	public override void OnDraw()
	{
		if (infoData_ != null)
		{
			DrawItem(0, "Index" + index_);
			DrawItem(2, "ID" + infoData_.getID(), eItemType.CenterOnly);
			DrawItem(1, "Reset", eItemType.CenterOnly);
			DrawItem(3, string.Empty, eItemType.TextField);
		}
	}

	public override void OnExecute()
	{
		if (infoData_ != null)
		{
			if (IsPressCenterButton(1))
			{
				infoData_.reset();
				date_ = infoData_.getDateStr();
				SetDefaultText(3, date_);
			}
			int num = 0;
			num = (int)Vary(0, index_, 1, 1, Constant.InformationSaveMax + 1);
			if (num != index_)
			{
				index_ = num;
				infoData_ = getInfoData(index_);
				date_ = infoData_.getDateStr();
				SetDefaultText(3, date_);
			}
			else
			{
				date_ = VaryString(3, date_);
			}
		}
	}

	private SaveInformationData getInfoData(int index)
	{
		return SaveData.Instance.getGameData().getOtherData().getInfoData(index);
	}
}
