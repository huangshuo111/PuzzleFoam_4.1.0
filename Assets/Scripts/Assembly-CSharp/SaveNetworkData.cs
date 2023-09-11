using System;
using UnityEngine;

public class SaveNetworkData : SaveDataBase
{
	private int eventNo_;

	private int eventStageDataNo_;

	private string resourceVersion_ = string.Empty;

	private string placementDataVersion_ = string.Empty;

	private string recoveryID_ = string.Empty;

	private string rankingDate_ = string.Empty;

	private string rankingUniqueID_ = string.Empty;

	private string sessionID_ = string.Empty;

	private string parkInfoDataVersion_ = string.Empty;

	private string parkStageDatasDataVersion_ = string.Empty;

	private string minilenThanksVersion_ = string.Empty;

	private string buildingVersion_ = string.Empty;

	protected override void OnSetup()
	{
		if (!PlayerPrefs.HasKey(SaveKeys.getNetworkEventNoKey()))
		{
			saveDefault();
		}
	}

	protected override void OnLoad()
	{
		eventNo_ = PlayerPrefs.GetInt(SaveKeys.getNetworkEventNoKey());
		eventStageDataNo_ = PlayerPrefs.GetInt(SaveKeys.getNetworkEventStageDataNoKey());
		resourceVersion_ = PlayerPrefs.GetString(SaveKeys.getNetworkResourceVersionKey());
		placementDataVersion_ = PlayerPrefs.GetString(SaveKeys.getNetworkPlacementDataVersionKey());
		recoveryID_ = PlayerPrefs.GetString(SaveKeys.getNetworkRecoverySaveDateKey());
		rankingDate_ = PlayerPrefs.GetString(SaveKeys.getNetworkRankingDateKey());
		sessionID_ = PlayerPrefs.GetString(SaveKeys.getNetworkSessionIDKey());
		rankingUniqueID_ = PlayerPrefs.GetString(SaveKeys.getNetworkRankingUniqueIDKey());
		parkInfoDataVersion_ = PlayerPrefs.GetString(SaveKeys.getNetworkParkStageDataVersionKey());
		parkStageDatasDataVersion_ = PlayerPrefs.GetString(SaveKeys.getNetworkParkPlacementVersionKey());
		minilenThanksVersion_ = PlayerPrefs.GetString(SaveKeys.getNetworkMinilenThanksVersionKey());
		buildingVersion_ = PlayerPrefs.GetString(SaveKeys.getNetworkBuildingVersionKey());
	}

	protected override void OnReset()
	{
		saveDefault();
	}

	protected override void OnSave()
	{
		PlayerPrefs.SetInt(SaveKeys.getNetworkEventNoKey(), eventNo_);
		PlayerPrefs.SetInt(SaveKeys.getNetworkEventStageDataNoKey(), eventStageDataNo_);
		PlayerPrefs.SetString(SaveKeys.getNetworkResourceVersionKey(), resourceVersion_);
		PlayerPrefs.SetString(SaveKeys.getNetworkPlacementDataVersionKey(), placementDataVersion_);
		PlayerPrefs.SetString(SaveKeys.getNetworkRecoverySaveDateKey(), recoveryID_);
		PlayerPrefs.SetString(SaveKeys.getNetworkRankingDateKey(), rankingDate_);
		PlayerPrefs.SetString(SaveKeys.getNetworkSessionIDKey(), sessionID_);
		PlayerPrefs.SetString(SaveKeys.getNetworkRankingUniqueIDKey(), rankingUniqueID_);
		PlayerPrefs.SetString(SaveKeys.getNetworkParkStageDataVersionKey(), parkInfoDataVersion_);
		PlayerPrefs.SetString(SaveKeys.getNetworkParkPlacementVersionKey(), parkStageDatasDataVersion_);
		PlayerPrefs.SetString(SaveKeys.getNetworkMinilenThanksVersionKey(), minilenThanksVersion_);
		PlayerPrefs.SetString(SaveKeys.getNetworkBuildingVersionKey(), buildingVersion_);
	}

	private void saveDefault()
	{
		eventNo_ = 0;
		eventStageDataNo_ = 0;
		resourceVersion_ = string.Empty;
		placementDataVersion_ = string.Empty;
		recoveryID_ = string.Empty;
		rankingUniqueID_ = string.Empty;
		rankingDate_ = string.Empty;
		sessionID_ = "0";
		parkInfoDataVersion_ = string.Empty;
		parkStageDatasDataVersion_ = string.Empty;
		minilenThanksVersion_ = string.Empty;
		buildingVersion_ = string.Empty;
		OnSave();
	}

	public int getEventNo()
	{
		return eventNo_;
	}

	public void setEventNo(int no)
	{
		eventNo_ = no;
	}

	public int getEventStageNo()
	{
		return eventStageDataNo_;
	}

	public void setEventStageNo(int no)
	{
		eventStageDataNo_ = no;
	}

	public string getResourceVersion()
	{
		return resourceVersion_;
	}

	public void setResourceVersion(string version)
	{
		resourceVersion_ = version;
	}

	public string getPlacementDataVersion()
	{
		return placementDataVersion_;
	}

	public void sePlacementDataVersion(string version)
	{
		placementDataVersion_ = version;
	}

	public string getRecovaryID()
	{
		return recoveryID_;
	}

	public void setRecoveryID(string id)
	{
		recoveryID_ = id;
	}

	public void resetRecoverySaveDate()
	{
		recoveryID_ = string.Empty;
	}

	public bool isRecovery()
	{
		return recoveryID_ != string.Empty;
	}

	public void resetRankingUniqueID()
	{
		rankingUniqueID_ = string.Empty;
	}

	public void setRankingUniqueID(string id)
	{
		rankingUniqueID_ = id;
	}

	public string getRankingUniqueID()
	{
		return rankingUniqueID_;
	}

	public void setParkInfoDataVersion(string version)
	{
		parkInfoDataVersion_ = version;
	}

	public string getParkInfoDataVersion()
	{
		return parkInfoDataVersion_;
	}

	public void setParkStageDatasDataVersion(string version)
	{
		parkStageDatasDataVersion_ = version;
	}

	public string getParkStageDatasDataVersion()
	{
		return parkStageDatasDataVersion_;
	}

	public void setMinilenThanksVersion(string version)
	{
		minilenThanksVersion_ = version;
	}

	public string getMinilenThanksVersion()
	{
		return minilenThanksVersion_;
	}

	public void setBuildingVersion(string version)
	{
		buildingVersion_ = version;
	}

	public string getBuildingVersion()
	{
		return buildingVersion_;
	}

	public void setRankingDate(DateTime date)
	{
		rankingDate_ = date.ToString();
	}

	public void resetRankingDate()
	{
		rankingDate_ = string.Empty;
	}

	public bool isShowRanking()
	{
		if (rankingDate_ == string.Empty)
		{
			return true;
		}
		if (getRankingDate() < DateTime.Now)
		{
			return true;
		}
		return false;
	}

	public DateTime getRankingDate()
	{
		return DateTime.Parse(rankingDate_);
	}

	public string getRankingDateStr()
	{
		return rankingDate_;
	}

	public string getSessionID()
	{
		return sessionID_;
	}

	public void setSessionID(string id, bool bSave)
	{
		sessionID_ = id;
		if (bSave)
		{
			PlayerPrefs.SetString(SaveKeys.getNetworkSessionIDKey(), sessionID_);
		}
	}
}
