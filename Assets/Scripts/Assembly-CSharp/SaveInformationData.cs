using System;
using UnityEngine;

public class SaveInformationData : SaveDataBase
{
	private int uniqueID_ = -1;

	private int id_ = -1;

	private string date_ = string.Empty;

	public SaveInformationData(int uniqueID)
	{
		uniqueID_ = uniqueID;
	}

	protected override void OnSetup()
	{
		if (!PlayerPrefs.HasKey(SaveKeys.getInformationDataIDKey(uniqueID_)))
		{
			saveDefault();
		}
	}

	protected override void OnLoad()
	{
		id_ = PlayerPrefs.GetInt(SaveKeys.getInformationDataIDKey(uniqueID_));
		date_ = PlayerPrefs.GetString(SaveKeys.getInformationDataDateKey(uniqueID_));
	}

	protected override void OnReset()
	{
		saveDefault();
	}

	protected override void OnSave()
	{
		PlayerPrefs.SetInt(SaveKeys.getInformationDataIDKey(uniqueID_), id_);
		PlayerPrefs.SetString(SaveKeys.getInformationDataDateKey(uniqueID_), date_);
	}

	private void saveDefault()
	{
		id_ = -1;
		date_ = string.Empty;
		OnSave();
	}

	public void setDate(DateTime date)
	{
		date_ = date.ToString();
	}

	public void resetDate()
	{
		date_ = string.Empty;
	}

	public DateTime getDate()
	{
		return DateTime.Parse(date_);
	}

	public string getDateStr()
	{
		return date_;
	}

	public bool isShow()
	{
		if (getDateStr() == string.Empty)
		{
			return true;
		}
		DateTime date = getDate();
		DateTime now = DateTime.Now;
		if ((now - date).Days > 1)
		{
			return true;
		}
		if (date.Day != now.Day)
		{
			return true;
		}
		return false;
	}

	public int getID()
	{
		return id_;
	}

	public void setID(int id)
	{
		id_ = id;
	}

	public void resetID()
	{
		id_ = -1;
	}

	public bool isNone()
	{
		return id_ == -1;
	}
}
