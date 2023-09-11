public class SaveGameData : SaveDataBase
{
	private SaveOtherData otherData_ = new SaveOtherData();

	private SaveNetworkData networkData_ = new SaveNetworkData();

	protected override void OnSetup()
	{
		networkData_.setup();
		otherData_.setup();
	}

	protected override void OnLoad()
	{
		networkData_.load();
		otherData_.load();
	}

	protected override void OnReset()
	{
		networkData_.reset();
		otherData_.reset();
	}

	protected override void OnSave()
	{
		networkData_.save();
		otherData_.save();
	}

	public SaveNetworkData getNetworkData()
	{
		return networkData_;
	}

	public SaveOtherData getOtherData()
	{
		return otherData_;
	}

	private void resetData(SaveDataBase[] saveDatas)
	{
		for (int i = 0; i < saveDatas.Length; i++)
		{
			if (saveDatas[i] != null)
			{
				saveDatas[i].reset();
			}
		}
	}

	private void loadData(SaveDataBase[] saveDatas)
	{
		for (int i = 0; i < saveDatas.Length; i++)
		{
			saveDatas[i].load();
		}
	}

	private void saveData(SaveDataBase[] saveDatas)
	{
		for (int i = 0; i < saveDatas.Length; i++)
		{
			if (saveDatas[i] != null)
			{
				saveDatas[i].save();
			}
		}
	}
}
