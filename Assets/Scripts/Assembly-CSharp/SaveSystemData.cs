public class SaveSystemData : SaveDataBase
{
	private SaveOptionData optionData_ = new SaveOptionData();

	protected override void OnSetup()
	{
		optionData_.setup();
	}

	protected override void OnLoad()
	{
		optionData_.load();
	}

	protected override void OnReset()
	{
		optionData_.reset();
		save();
	}

	protected override void OnSave()
	{
		optionData_.save();
	}

	public SaveOptionData getOptionData()
	{
		return optionData_;
	}
}
