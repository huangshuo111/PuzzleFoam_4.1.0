using UnityEngine;

public class SaveData : MonoBehaviour
{
	private string AppVersion = string.Empty;

	private static SaveData instance_;

	private SaveGameData gameData_ = new SaveGameData();

	private SaveSystemData systemData_ = new SaveSystemData();

	private SaveParkData parkData_ = new SaveParkData();

	public string ServerVersion { get; set; }

	public bool PresentFlag { get; set; }

	public static SaveData Instance
	{
		get
		{
			return instance_;
		}
	}

	public static bool IsInstance()
	{
		return (!(instance_ == null)) ? true : false;
	}

	private void Awake()
	{
		if (instance_ == null)
		{
			instance_ = this;
			Object.DontDestroyOnLoad(this);
			TextAsset textAsset = Resources.Load("Parameter/version", typeof(TextAsset)) as TextAsset;
			instance_.AppVersion = textAsset.text;
			setup();
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		instance_ = null;
	}

	public void save()
	{
		gameData_.save();
		systemData_.save();
	}

	public void load()
	{
		gameData_.load();
		systemData_.load();
		parkData_.load();
	}

	public void reset()
	{
		gameData_.reset();
		systemData_.reset();
		parkData_.delete();
	}

	private void setup()
	{
		gameData_.setup();
		systemData_.setup();
	}

	public SaveGameData getGameData()
	{
		return gameData_;
	}

	public SaveSystemData getSystemData()
	{
		return systemData_;
	}

	public SaveParkData getParkData()
	{
		return parkData_;
	}

	public string getAppVersion()
	{
		return AppVersion;
	}
}
