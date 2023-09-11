using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Zip;
using LitJson;
using Network;
using UnityEngine;

public class StageDataTable : MonoBehaviour
{
	public enum eResource
	{
		EventInfo = 1,
		StageInfo = 2,
		EventStage = 4,
		PlacementData = 8,
		parkStageInfo = 0x10,
		parkStageData = 0x20,
		minilenThanksData = 0x40,
		buildingData = 0x80
	}

	public enum eStageDataType
	{
		NormalStage = 0,
		EventStage = 1,
		BossStage = 2,
		ParkInfo = 3,
		ParkStageDatas = 4
	}

	public delegate IEnumerator OnError();

	public const int EventStageNoBase = 10000;

	public const int EventRequireStageNo = 18;

	public const int ChallengeRequireStageNo = 30;

	public const int CollaborationRequireStageNo = 6;

	private StageInfo stageData_;

	private EventStageInfo eventData_;

	private BossStageInfo bossData_;

	private RankingStageInfo rankingData_;

	private ParkStageInfo parkStageData_;

	private WWW resourceURLData_;

	private Hashtable args_ = new Hashtable();

	private Dictionary<Constant.Item.eType, int> newItemDict_ = new Dictionary<Constant.Item.eType, int>();

	private List<int> addItemList_ = new List<int>(4);

	private GameObject dialogObj_;

	public bool bGameResourceDownload;

	public int getNewItemStageNo(Constant.Item.eType itemType)
	{
		if (!newItemDict_.ContainsKey(itemType))
		{
			return -1;
		}
		return newItemDict_[itemType];
	}

	public int getAddItemStageNo(int index)
	{
		if (index >= addItemList_.Count)
		{
			return -1;
		}
		return addItemList_[index];
	}

	private void setupNewItemDict()
	{
		newItemDict_.Clear();
		addItemList_.Clear();
		for (int i = 0; i < stageData_.Infos.Length; i++)
		{
			StageInfo.Info info = stageData_.Infos[i];
			int stageNo = info.Common.StageNo;
			int num = 0;
			StageInfo.Item[] items = info.Common.Items;
			foreach (StageInfo.Item item in items)
			{
				Constant.Item.eType type = (Constant.Item.eType)item.Type;
				if (!newItemDict_.ContainsKey(type))
				{
					newItemDict_[type] = stageNo - 1;
				}
				if (type != Constant.Item.eType.Invalid)
				{
					num++;
				}
			}
			if (addItemList_.Count < num)
			{
				addItemList_.Add(stageNo);
			}
		}
	}

	private void loadStageData(string text)
	{
		stageData_ = Xml.DeserializeObject<StageInfo>(text) as StageInfo;
		setupNewItemDict();
	}

	public void loadStageData()
	{
		byte[] bytes = Aes.DecryptFromFile(getStageDataFilePath());
		Encoding encoding = Encoding.GetEncoding("UTF-8");
		string @string = encoding.GetString(bytes);
		loadStageData(@string);
		setupNewItemDict();
	}

	public void loadEventData()
	{
		byte[] bytes = Aes.DecryptFromFile(getEventDataFilePath());
		Encoding encoding = Encoding.GetEncoding("UTF-8");
		string @string = encoding.GetString(bytes);
		eventData_ = Xml.DeserializeObject<EventStageInfo>(@string) as EventStageInfo;
	}

	public void loadEventData(string filename)
	{
		byte[] bytes = Aes.DecryptFromFile(getEventDataFilePath());
		Encoding encoding = Encoding.GetEncoding("UTF-8");
		string @string = encoding.GetString(bytes);
		eventData_ = Xml.DeserializeObject<EventStageInfo>(@string) as EventStageInfo;
	}

	public void loadBossData()
	{
		byte[] bytes = Aes.DecryptFromFile(getBossDataFilePath());
		Encoding encoding = Encoding.GetEncoding("UTF-8");
		string @string = encoding.GetString(bytes);
		bossData_ = Xml.DeserializeObject<BossStageInfo>(@string) as BossStageInfo;
	}

	public void loadRankingStageData()
	{
		byte[] bytes = Aes.DecryptFromFile(getRankingStageDataFilePath());
		Encoding encoding = Encoding.GetEncoding("UTF-8");
		string @string = encoding.GetString(bytes);
		rankingData_ = Xml.DeserializeObject<RankingStageInfo>(@string) as RankingStageInfo;
	}

	public void loadParkStageData()
	{
		byte[] bytes = Aes.DecryptFromFile(getParkStageInfoFilePath());
		Encoding encoding = Encoding.GetEncoding("UTF-8");
		string @string = encoding.GetString(bytes);
		parkStageData_ = Xml.DeserializeObject<ParkStageInfo>(@string) as ParkStageInfo;
		setupNewItemDict();
	}

	public EventStageInfo getEventData()
	{
		return eventData_;
	}

	public BossStageInfo getBossData()
	{
		return bossData_;
	}

	public StageInfo getStageData()
	{
		return stageData_;
	}

	public RankingStageInfo getRankingData()
	{
		return rankingData_;
	}

	public ParkStageInfo getParkStageData()
	{
		return parkStageData_;
	}

	public EventStageInfo.Info getEventInfo(int stageNo, int eventNo)
	{
		if (eventData_ == null)
		{
			return null;
		}
		if (!Constant.Event.isEventStage(stageNo))
		{
			return null;
		}
		int num = Constant.Event.convNoToLevel(stageNo, eventNo);
		if (num - 1 < 0 || num - 1 > eventData_.Infos.Length)
		{
			return null;
		}
		return eventData_.Infos[num - 1];
	}

	public ParkStageInfo.Info getParkInfo(int stageNo)
	{
		if (parkStageData_ == null)
		{
			return null;
		}
		if (!Constant.ParkStage.isParkStage(stageNo))
		{
			return null;
		}
		return Array.Find(parkStageData_.Infos, (ParkStageInfo.Info psd) => psd.Common.StageNo == stageNo);
	}

	public BossStageInfo.Info getBossInfo(int type)
	{
		if (bossData_ == null)
		{
			return null;
		}
		if (type <= -1 || 4 <= type)
		{
			return null;
		}
		return bossData_.Infos[type];
	}

	public BossStageInfo.LevelInfo getBossLevelData(int type, int level)
	{
		if (bossData_ == null)
		{
			return null;
		}
		if (type <= -1 || 4 <= type)
		{
			return null;
		}
		BossStageInfo.LevelInfo[] levelInfos = bossData_.Infos[type].BossInfo.LevelInfos;
		foreach (BossStageInfo.LevelInfo levelInfo in levelInfos)
		{
			if (levelInfo.Level == level)
			{
				return levelInfo;
			}
		}
		return null;
	}

	public StageInfo.Info getInfo(int stageNo)
	{
		if (stageNo < 0 || stageNo >= stageData_.Infos.Length)
		{
			if (Constant.ParkStage.isParkStage(stageNo))
			{
				ParkStageInfo.Info parkInfo = getParkInfo(stageNo);
				if (parkInfo != null)
				{
					return convertStageInfo(getParkInfo(stageNo));
				}
			}
			return null;
		}
		return stageData_.Infos[stageNo];
	}

	public StageData getPlacementData(int stageNo)
	{
		if (stageNo < 0 || stageNo >= stageData_.Infos.Length)
		{
			return null;
		}
		StageData result = null;
		using (ZipFile zipFile = ZipFile.Read(getPlacementDataFilePath()))
		{
			ZipEntry zipEntry = zipFile[(stageNo + 1).ToString("D5") + ".xml"];
			if (zipEntry == null)
			{
				zipEntry = zipFile["00001.xml"];
			}
			if (zipEntry != null)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					zipEntry.Extract(memoryStream);
					result = Xml.DeserializeObject<StageData>(Xml.UTF8ByteArrayToString(memoryStream.ToArray())) as StageData;
				}
			}
		}
		return result;
	}

	public StageData getParkPlacementData(int stageNo)
	{
		if (!Constant.ParkStage.isParkStage(stageNo))
		{
			return null;
		}
		StageData result = null;
		using (ZipFile zipFile = ZipFile.Read(getParkStageDatasPath()))
		{
			ZipEntry zipEntry = zipFile[stageNo + ".xml"];
			if (zipEntry == null)
			{
				zipEntry = zipFile["500001.xml"];
			}
			if (zipEntry != null)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					zipEntry.Extract(memoryStream);
					result = Xml.DeserializeObject<StageData>(Xml.UTF8ByteArrayToString(memoryStream.ToArray())) as StageData;
				}
			}
		}
		return result;
	}

	public ParkAreaInfo getParkAreaInfo(int area_id)
	{
		ParkStageInfo.Info[] array = Array.FindAll(parkStageData_.Infos, (ParkStageInfo.Info info) => info.Area == area_id);
		if (array == null || array.Length <= 0)
		{
			return null;
		}
		ParkAreaInfo area_info = new ParkAreaInfo(area_id);
		area_info.stage_infos = array;
		Array.ForEach(array, delegate(ParkStageInfo.Info stage)
		{
			if (area_info.first_info == null || stage.Common.StageNo < area_info.first_info.Common.StageNo)
			{
				area_info.first_info = stage;
			}
		});
		List<int> minilen_ids = new List<int>();
		Array.ForEach(array, delegate(ParkStageInfo.Info stage)
		{
			Array.ForEach(stage.Common.MinilenDrops, delegate(ParkStageInfo.MinilenDrop minilen)
			{
				if (!minilen_ids.Contains(minilen.MinilenDropId))
				{
					minilen_ids.Add(minilen.MinilenDropId);
				}
			});
		});
		area_info.gettable_minilens = minilen_ids.ToArray();
		return area_info;
	}

	public static string getEventDataFilePath()
	{
		return Constant.GetDocumentsPath() + "/event.xml";
	}

	public static string getBossDataFilePath()
	{
		return Constant.GetDocumentsPath() + "/boss.xml";
	}

	public static string getStageDataFilePath()
	{
		return Constant.GetDocumentsPath() + "/stage_Info.xml";
	}

	public static string getParkStageInfoFilePath()
	{
		return Constant.GetDocumentsPath() + "/park_stage.xml";
	}

	public static string getEventStageDataFilePath(int eventNo, EventStageInfo.eLevel level)
	{
		return Constant.GetDocumentsPath() + "/" + ((int)(eventNo * 10000 + level)).ToString("00000") + ".xml";
	}

	public static string getBossStageDataFilePath(int eventNo, BossStageInfo.eBossType type)
	{
		return Constant.GetDocumentsPath() + "/" + ((int)(30000 + type)).ToString("00000") + ".xml";
	}

	public static string getRankingStageDataFilePath()
	{
		return Constant.GetDocumentsPath() + "/rankingStage.xml";
	}

	public static string getPlacementDataFilePath()
	{
		string path = Constant.GetDocumentsPath() + "/stagedata";
		return Aes.EncryptPath(path);
	}

	public static string getParkStageDatasPath()
	{
		string path = Constant.GetDocumentsPath() + "/parkstagedata";
		return Aes.EncryptPath(path);
	}

	public static string getMinilenThanksDataPath()
	{
		return Constant.GetDocumentsPath() + "/minilen_thanks.xml";
	}

	public static string getBuildingDataPath()
	{
		return Constant.GetDocumentsPath() + "/building.xml";
	}

	public static string getParkDummyDataPath()
	{
		return Constant.GetDocumentsPath() + "/park_dummy_data.xml";
	}

	public IEnumerator download(ResponceHeaderData headerData, bool bCancel, bool bShowIcon)
	{
		int updateInfo = getUpdateInfo(headerData);
		if (updateInfo != 0 || getResourceURLData() == null)
		{
			yield return StartCoroutine(downloadResourceURLData(bCancel, bShowIcon));
			WWW www = getResourceURLData();
			if (www != null)
			{
				ResourceURLData resourceURLData = JsonMapper.ToObject<ResourceURLData>(www.text);
				yield return StartCoroutine(downloadResource(updateInfo, headerData, resourceURLData, bCancel, bShowIcon));
			}
		}
	}

	public int getUpdateInfo(ResponceHeaderData headerData)
	{
		int num = 0;
		if (headerData == null)
		{
			return num;
		}
		SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
		if (headerData.EventID != 0)
		{
		}
		if (headerData.EventID != 0)
		{
			num |= 4;
		}
		if (headerData.EventID == 0)
		{
			networkData.setEventStageNo(headerData.EventID);
			networkData.setEventNo(headerData.EventID);
			networkData.save();
		}
		if (networkData.getResourceVersion() != headerData.ResourceVersion || !Aes.Exists(getStageDataFilePath()))
		{
			num |= 2;
		}
		if (networkData.getPlacementDataVersion() != headerData.ResourceVersion || !File.Exists(getPlacementDataFilePath()))
		{
			num |= 8;
		}
		if (networkData.getParkInfoDataVersion() != headerData.ResourceVersion || !File.Exists(getParkStageInfoFilePath()))
		{
			num |= 0x10;
		}
		if (networkData.getParkStageDatasDataVersion() != headerData.ResourceVersion || !File.Exists(getParkStageDatasPath()))
		{
			num |= 0x20;
		}
		if (networkData.getMinilenThanksVersion() != headerData.ResourceVersion || !Aes.Exists(getMinilenThanksDataPath()))
		{
			num |= 0x40;
		}
		if (networkData.getBuildingVersion() != headerData.ResourceVersion || !Aes.Exists(getBuildingDataPath()))
		{
			num |= 0x80;
		}
		return num;
	}

	public bool isUpdateEvent(int info)
	{
		return true;
	}

	public bool isUpdateEventStageData(int info)
	{
		return true;
	}

	public bool isUpdateStageInfo(int info)
	{
		return (info & 2) != 0;
	}

	public bool isUpdateParkStageInfo(int info)
	{
		return (info & 0x10) != 0;
	}

	public bool isUpdateParkPlacementData(int info)
	{
		return (info & 0x20) != 0;
	}

	public bool isUpdateMinilenThanksData(int info)
	{
		return (info & 0x40) != 0;
	}

	public bool isUpdateBuildingData(int info)
	{
		return (info & 0x80) != 0;
	}

	public bool isUpdatePlacementData(int info)
	{
		return (info & 8) != 0;
	}

	public IEnumerator downloadResourceURLData(bool bCancel, bool bShowIcon)
	{
		args_.Clear();
		args_["uri"] = "resource/url/";
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, bCancel, bShowIcon));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			resourceURLData_ = NetworkMng.Instance.getWWW();
		}
	}

	public WWW getResourceURLData()
	{
		return resourceURLData_;
	}

	public IEnumerator downloadStageData(ResponceHeaderData headerData, ResourceURLData urlData, bool bCancel, bool bShowIcon)
	{
		Debug.Log(" downloadStageData url : " + urlData.eventResourceUrl);
		yield return StartCoroutine(_download(eStageDataType.NormalStage, urlData.stageInfoResourceUrl, "stageInfo.xml", headerData, bCancel, bShowIcon));
	}

	public IEnumerator downloadEventData(ResponceHeaderData headerData, ResourceURLData urlData, bool bCancel, bool bShowIcon)
	{
		Debug.Log(" downloadEventData url : " + urlData.eventResourceUrl);
		yield return StartCoroutine(_download(eStageDataType.EventStage, urlData.eventResourceUrl, "event.xml", headerData, bCancel, bShowIcon));
	}

	public IEnumerator downloadBossData(ResponceHeaderData headerData, ResourceURLData urlData, bool bCancel, bool bShowIcon)
	{
		yield return StartCoroutine(_download(eStageDataType.BossStage, urlData.bossResourceUrl, "boss.xml", headerData, bCancel, bShowIcon));
	}

	public IEnumerator downloadParkStageInfo(ResponceHeaderData headerData, ResourceURLData urlData, bool bCancel, bool bShowIcon)
	{
		yield return StartCoroutine(_download(eStageDataType.ParkInfo, urlData.parkResourceUrl, "park_stage.xml", headerData, bCancel, bShowIcon));
	}

	private WWW OnCreateWWW(Hashtable args)
	{
		if (args_.ContainsKey("uri"))
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			return WWWWrap.create((string)args_["uri"]);
		}
		if (args_.ContainsKey("url"))
		{
			return new WWW((string)args_["url"]);
		}
		return null;
	}

	public IEnumerator downloadEventStageData(ResponceHeaderData headerData, ResourceURLData urlData, bool bCancel, bool bShowIcon)
	{
		Debug.Log(" downloadEventStageData url : " + urlData.eventStageDataResourceUrl);
		args_.Clear();
		args_["url"] = urlData.eventStageDataResourceUrl;
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, bCancel, bShowIcon));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		Encoding enc = Encoding.GetEncoding("UTF-8");
		string dataText = enc.GetString(www.bytes);
		string[] datas = dataText.Split('\n');
		ulong freeDiskSpace = FreeDiskSpace.getFreeDiskSpace();
		ulong totalBytes = 0uL;
		for (int i = 0; i < datas.Length; i++)
		{
			string data = datas[i];
			if (data.Length > 1)
			{
				string filePath = getEventStageDataFilePath(headerData.EventID, (EventStageInfo.eLevel)(i + 1));
				totalBytes += (ulong)enc.GetBytes(data).Length;
				while (freeDiskSpace <= totalBytes)
				{
					yield return StartCoroutine(capacityShortage());
					freeDiskSpace = FreeDiskSpace.getFreeDiskSpace();
					Debug.Log("strage maz : free disk space:" + freeDiskSpace);
				}
				Aes.EncryptToFile(enc.GetBytes(data), filePath);
			}
		}
		SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
		networkData.setEventStageNo(headerData.EventID);
		networkData.save();
	}

	public IEnumerator downloadPlacementData(ResponceHeaderData headerData, ResourceURLData urlData, bool bCancel, bool bShowIcon)
	{
		args_.Clear();
		args_["url"] = urlData.stageDataResourceUrl;
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, bCancel, bShowIcon));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			ulong freeDiskSpace = FreeDiskSpace.getFreeDiskSpace();
			while (freeDiskSpace <= (ulong)www.bytes.Length)
			{
				yield return StartCoroutine(capacityShortage());
				freeDiskSpace = FreeDiskSpace.getFreeDiskSpace();
				Debug.Log("strage maz : free disk space:" + freeDiskSpace);
			}
			File.WriteAllBytes(getPlacementDataFilePath(), www.bytes);
			SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
			networkData.sePlacementDataVersion(headerData.ResourceVersion);
			networkData.save();
		}
	}

	public IEnumerator downloadParkPlacementData(ResponceHeaderData headerData, ResourceURLData urlData, bool bCancel, bool bShowIcon)
	{
		args_.Clear();
		args_["url"] = urlData.parkStageDataResourceUrl;
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, bCancel, bShowIcon));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			ulong freeDiskSpace = FreeDiskSpace.getFreeDiskSpace();
			while (freeDiskSpace <= (ulong)www.bytes.Length)
			{
				yield return StartCoroutine(capacityShortage());
				freeDiskSpace = FreeDiskSpace.getFreeDiskSpace();
				Debug.Log("strage maz : free disk space:" + freeDiskSpace);
			}
			string path = getParkStageDatasPath();
			File.WriteAllBytes(path, www.bytes);
			SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
			networkData.setParkStageDatasDataVersion(headerData.ResourceVersion);
			networkData.save();
		}
	}

	public IEnumerator downloadMinilenThanksData(ResponceHeaderData headerData, ResourceURLData urlData, bool bCancel, bool bShowIcon)
	{
		args_.Clear();
		args_["url"] = urlData.parkMinilenThanksDataResourceUrl;
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, bCancel, bShowIcon));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			ulong freeDiskSpace = FreeDiskSpace.getFreeDiskSpace();
			while (freeDiskSpace <= (ulong)www.bytes.Length)
			{
				yield return StartCoroutine(capacityShortage());
				freeDiskSpace = FreeDiskSpace.getFreeDiskSpace();
				Debug.Log("strage maz : free disk space:" + freeDiskSpace);
			}
			Aes.EncryptToFile(filepath: getMinilenThanksDataPath(), bytes: www.bytes);
			SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
			networkData.setMinilenThanksVersion(headerData.ResourceVersion);
			networkData.save();
		}
	}

	public IEnumerator downloadBuildingData(ResponceHeaderData headerData, ResourceURLData urlData, bool bCancel, bool bShowIcon)
	{
		args_.Clear();
		args_["url"] = urlData.parkBuildingDataResourceUrl;
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, bCancel, bShowIcon));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			ulong freeDiskSpace = FreeDiskSpace.getFreeDiskSpace();
			while (freeDiskSpace <= (ulong)www.bytes.Length)
			{
				yield return StartCoroutine(capacityShortage());
				freeDiskSpace = FreeDiskSpace.getFreeDiskSpace();
				Debug.Log("strage maz : free disk space:" + freeDiskSpace);
			}
			Aes.EncryptToFile(filepath: getBuildingDataPath(), bytes: www.bytes);
			SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
			networkData.setBuildingVersion(headerData.ResourceVersion);
			networkData.save();
		}
	}

	public IEnumerator downloadParkDummyData(ResourceURLData urlData, bool bCancel, bool bShowIcon, Action<string> ret)
	{
		args_.Clear();
		args_["url"] = urlData.parkDummyDataResourceUrl;
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, bCancel, bShowIcon));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			ulong freeDiskSpace = FreeDiskSpace.getFreeDiskSpace();
			while (freeDiskSpace <= (ulong)www.bytes.Length)
			{
				yield return StartCoroutine(capacityShortage());
				freeDiskSpace = FreeDiskSpace.getFreeDiskSpace();
				Debug.Log("strage maz : free disk space:" + freeDiskSpace);
			}
			ret(www.text);
		}
	}

	public IEnumerator downloadParkDummyData(bool bCancel, bool bShowIcon, Action<string> ret)
	{
		if (getResourceURLData() == null)
		{
			yield return StartCoroutine(downloadResourceURLData(bCancel, bShowIcon));
		}
		ResourceURLData resourceURL = JsonMapper.ToObject<ResourceURLData>(resourceURLData_.text);
		yield return StartCoroutine(downloadParkDummyData(resourceURL, bCancel, bShowIcon, ret));
	}

	private IEnumerator _download(eStageDataType dataType, string url, string fileName, ResponceHeaderData headerData, bool bCancel, bool bShowIcon)
	{
		args_.Clear();
		args_["url"] = url;
		yield return StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, bCancel, bShowIcon));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			if (!Directory.Exists(Constant.GetDocumentsPath()))
			{
				Directory.CreateDirectory(Constant.GetDocumentsPath());
			}
			string filePath2 = string.Empty;
			switch (dataType)
			{
			case eStageDataType.NormalStage:
				filePath2 = getStageDataFilePath();
				break;
			case eStageDataType.EventStage:
				filePath2 = getEventDataFilePath();
				break;
			case eStageDataType.BossStage:
				filePath2 = getBossDataFilePath();
				break;
			case eStageDataType.ParkInfo:
				filePath2 = getParkStageInfoFilePath();
				break;
			default:
				filePath2 = getStageDataFilePath();
				break;
			}
			ulong freeDiskSpace = FreeDiskSpace.getFreeDiskSpace();
			while (freeDiskSpace <= (ulong)www.bytes.Length)
			{
				yield return StartCoroutine(capacityShortage());
				freeDiskSpace = FreeDiskSpace.getFreeDiskSpace();
				Debug.Log("strage maz : free disk space:" + freeDiskSpace);
			}
			Aes.EncryptToFile(www.bytes, filePath2);
			Encoding enc = Encoding.GetEncoding("UTF-8");
			string data = enc.GetString(www.bytes);
			SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
			switch (dataType)
			{
			case eStageDataType.NormalStage:
				loadStageData(data);
				networkData.setResourceVersion(headerData.ResourceVersion);
				break;
			case eStageDataType.EventStage:
				eventData_ = Xml.DeserializeObject<EventStageInfo>(data) as EventStageInfo;
				networkData.setEventNo(headerData.EventID);
				break;
			case eStageDataType.BossStage:
				bossData_ = Xml.DeserializeObject<BossStageInfo>(data) as BossStageInfo;
				break;
			case eStageDataType.ParkInfo:
				parkStageData_ = Xml.DeserializeObject<ParkStageInfo>(data) as ParkStageInfo;
				networkData.setParkInfoDataVersion(headerData.ResourceVersion);
				break;
			default:
				loadStageData(data);
				networkData.setResourceVersion(headerData.ResourceVersion);
				break;
			}
			networkData.save();
		}
	}

	public IEnumerator downloadResource(int updateInfo, ResponceHeaderData headerData, ResourceURLData urlData, bool bCancel, bool bShowIcon)
	{
		NetworkMng netMng = NetworkMng.Instance;
		if (!string.IsNullOrEmpty(urlData.bossResourceUrl))
		{
			do
			{
				yield return StartCoroutine(downloadBossData(headerData, urlData, bCancel, bShowIcon));
				if (netMng.getStatus() == NetworkMng.eStatus.Cancel)
				{
					yield break;
				}
			}
			while (netMng.getStatus() != NetworkMng.eStatus.Success);
		}
		Debug.Log("isUpdateEvent");
		if (isUpdateEvent(updateInfo) && !string.IsNullOrEmpty(urlData.eventResourceUrl))
		{
			do
			{
				yield return StartCoroutine(downloadEventData(headerData, urlData, bCancel, bShowIcon));
				if (netMng.getStatus() == NetworkMng.eStatus.Cancel)
				{
					yield break;
				}
			}
			while (netMng.getStatus() != NetworkMng.eStatus.Success);
		}
		Debug.Log("isUpdateEventStageData");
		if (isUpdateEventStageData(updateInfo) && !string.IsNullOrEmpty(urlData.eventStageDataResourceUrl))
		{
			do
			{
				yield return StartCoroutine(downloadEventStageData(headerData, urlData, bCancel, bShowIcon));
				if (netMng.getStatus() == NetworkMng.eStatus.Cancel)
				{
					yield break;
				}
			}
			while (netMng.getStatus() != NetworkMng.eStatus.Success);
		}
		Debug.Log("isUpdateStageInfo");
		if (isUpdateStageInfo(updateInfo))
		{
			do
			{
				yield return StartCoroutine(downloadStageData(headerData, urlData, bCancel, bShowIcon));
				if (netMng.getStatus() == NetworkMng.eStatus.Cancel)
				{
					yield break;
				}
			}
			while (netMng.getStatus() != NetworkMng.eStatus.Success);
		}
		Debug.Log("isUpdatePlacementData");
		if (isUpdatePlacementData(updateInfo))
		{
			do
			{
				yield return StartCoroutine(downloadPlacementData(headerData, urlData, bCancel, bShowIcon));
				if (netMng.getStatus() == NetworkMng.eStatus.Cancel)
				{
					yield break;
				}
			}
			while (netMng.getStatus() != NetworkMng.eStatus.Success);
		}
		Debug.Log("isUpdateParkStageInfo");
		if (isUpdateParkStageInfo(updateInfo))
		{
			do
			{
				yield return StartCoroutine(downloadParkStageInfo(headerData, urlData, bCancel, bShowIcon));
				if (netMng.getStatus() == NetworkMng.eStatus.Cancel)
				{
					yield break;
				}
			}
			while (netMng.getStatus() != NetworkMng.eStatus.Success);
		}
		Debug.Log("isUpdateParkPlacementData");
		if (isUpdateParkPlacementData(updateInfo))
		{
			do
			{
				yield return StartCoroutine(downloadParkPlacementData(headerData, urlData, bCancel, bShowIcon));
				if (netMng.getStatus() == NetworkMng.eStatus.Cancel)
				{
					yield break;
				}
			}
			while (netMng.getStatus() != NetworkMng.eStatus.Success);
		}
		Debug.Log("isUpdateMinilenThanksData");
		if (isUpdateMinilenThanksData(updateInfo))
		{
			do
			{
				yield return StartCoroutine(downloadMinilenThanksData(headerData, urlData, bCancel, bShowIcon));
				if (netMng.getStatus() == NetworkMng.eStatus.Cancel)
				{
					yield break;
				}
			}
			while (netMng.getStatus() != NetworkMng.eStatus.Success);
		}
		Debug.Log("isUpdateBuildingData");
		if (isUpdateBuildingData(updateInfo))
		{
			do
			{
				yield return StartCoroutine(downloadBuildingData(headerData, urlData, bCancel, bShowIcon));
			}
			while (netMng.getStatus() != NetworkMng.eStatus.Cancel && netMng.getStatus() != NetworkMng.eStatus.Success);
		}
	}

	public IEnumerator downloadGameResource(Constant.eDownloadDataNo baseNo, int dataNo, int dlCount, int dlCountMax)
	{
		yield return StartCoroutine(downloadGameResource(baseNo, dataNo, true, dlCount, dlCountMax));
	}

	public IEnumerator downloadGameResource(Constant.eDownloadDataNo baseNo, int dataNo, bool bCloseDialog, int dlCount, int dlCountMax)
	{
		bGameResourceDownload = true;
		PartManager partManager = GameObject.Find("_PartManager").GetComponent<PartManager>();
		if (getResourceURLData() == null)
		{
			if (bCloseDialog && dialogObj_ != null)
			{
				DialogBase dialog4 = dialogObj_.GetComponent<DialogBase>();
				yield return StartCoroutine(dialog4.close());
				UnityEngine.Object.Destroy(dialogObj_);
				dialogObj_ = null;
				partManager.inhibitTips(false);
			}
			bGameResourceDownload = false;
			yield break;
		}
		int id = (int)(baseNo + dataNo);
		bool isExistLowData = true;
		if (baseNo == Constant.eDownloadDataNo.HowtoPlay_EN || baseNo == Constant.eDownloadDataNo.HowtoPlay_JP || baseNo == Constant.eDownloadDataNo.Park || baseNo == Constant.eDownloadDataNo.Minilen)
		{
			isExistLowData = false;
		}
		if (ResourceLoader.Instance.isUseLowResource() && isExistLowData)
		{
			id += 1000;
		}
		ResourceURLData resourceURLData = JsonMapper.ToObject<ResourceURLData>(getResourceURLData().text);
		if (resourceURLData.gameResourceUrl == null)
		{
			if (bCloseDialog && dialogObj_ != null)
			{
				DialogBase dialog5 = dialogObj_.GetComponent<DialogBase>();
				yield return StartCoroutine(dialog5.close());
				UnityEngine.Object.Destroy(dialogObj_);
				dialogObj_ = null;
				partManager.inhibitTips(false);
			}
			bGameResourceDownload = false;
			yield break;
		}
		GameResourceURLData gameResourceURLData = null;
		GameResourceURLData[] gameResourceUrl = resourceURLData.gameResourceUrl;
		foreach (GameResourceURLData data in gameResourceUrl)
		{
			if (int.Parse(data.id) == id)
			{
				gameResourceURLData = data;
			}
		}
		string localVersion = PlayerPrefs.GetString("GameResourceVersionCode" + id, "-1");
		if (localVersion == gameResourceURLData.version)
		{
			string[] files = Constant.sample;
			if (id < 1000)
			{
				switch (baseNo)
				{
				case Constant.eDownloadDataNo.Main:
					files = Constant.outsideFiles_main[dataNo];
					break;
				case Constant.eDownloadDataNo.BG:
					files = Constant.outsideFiles_BG[dataNo];
					break;
				case Constant.eDownloadDataNo.WorldMap:
					files = Constant.outsideFiles_worldmap[dataNo];
					break;
				case Constant.eDownloadDataNo.ThrowChara:
					files = Constant.outsideFiles_throw_chara[dataNo];
					break;
				case Constant.eDownloadDataNo.SupportChara:
					files = Constant.outsideFiles_support_chara[dataNo];
					break;
				case Constant.eDownloadDataNo.ScenarioChara:
					files = Constant.outsideFiles_scenario_chara[dataNo];
					break;
				case Constant.eDownloadDataNo.HowtoPlay_JP:
					files = Constant.outsideFiles_howtoplay_JP[dataNo];
					break;
				case Constant.eDownloadDataNo.HowtoPlay_EN:
					files = Constant.outsideFiles_howtoplay_EN[dataNo];
					break;
				case Constant.eDownloadDataNo.Park:
					files = Constant.outsideFiles_park[dataNo];
					break;
				case Constant.eDownloadDataNo.Minilen:
					files = Constant.outsideFiles_minilen[dataNo];
					break;
				}
			}
			else
			{
				switch (baseNo)
				{
				case Constant.eDownloadDataNo.Main:
					files = Constant.outsideFilesLow_main[dataNo];
					break;
				case Constant.eDownloadDataNo.BG:
					files = Constant.outsideFilesLow_BG[dataNo];
					break;
				case Constant.eDownloadDataNo.WorldMap:
					files = Constant.outsideFilesLow_worldmap[dataNo];
					break;
				case Constant.eDownloadDataNo.ThrowChara:
					files = Constant.outsideFilesLow_throw_chara[dataNo];
					break;
				case Constant.eDownloadDataNo.SupportChara:
					files = Constant.outsideFilesLow_support_chara[dataNo];
					break;
				case Constant.eDownloadDataNo.ScenarioChara:
					files = Constant.outsideFilesLow_scenario_chara[dataNo];
					break;
				}
			}
			bool bExist = true;
			if (files == Constant.sample)
			{
				yield return null;
			}
			string[] array = files;
			for (int k = 0; k < array.Length; k++)
			{
				if (!Aes.Exists(string.Concat(str2: Path.GetFileNameWithoutExtension(array[k]), str0: Constant.GetDocumentsPath(), str1: "/")))
				{
					bExist = false;
					break;
				}
			}
			if (bExist)
			{
				if (dialogObj_ != null)
				{
					setDownloadPercent((float)(dlCount + 1) / (float)dlCountMax);
					yield return null;
				}
				if (bCloseDialog && dialogObj_ != null)
				{
					DialogBase dialog3 = dialogObj_.GetComponent<DialogBase>();
					yield return StartCoroutine(dialog3.close());
					UnityEngine.Object.Destroy(dialogObj_);
					dialogObj_ = null;
					partManager.inhibitTips(false);
				}
				bGameResourceDownload = false;
				yield break;
			}
		}
		ulong freeDiskSpace;
		for (freeDiskSpace = FreeDiskSpace.getFreeDiskSpace(); freeDiskSpace <= 3145728; freeDiskSpace = FreeDiskSpace.getFreeDiskSpace())
		{
			yield return StartCoroutine(capacityShortage());
		}
		if (dialogObj_ == null)
		{
			dialogObj_ = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Download_Panel")) as GameObject;
			DialogBase dialog2 = dialogObj_.AddComponent<DialogBase>();
			Utility.setParent(dialogObj_, base.transform.parent.parent.parent.parent, false);
			dialog2.init(partManager.dialogManager, partManager, partManager.fade, DialogManager.eDialog.DataDownload);
			dialogObj_.transform.Find("window/Download_Label").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(1480);
			dialogObj_.transform.Find("window/Instructions_Label").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(1481);
			setDownloadPercent((float)dlCount / (float)dlCountMax);
			partManager.inhibitTips(true);
			yield return StartCoroutine(dialog2.open());
		}
		args_.Clear();
		args_["url"] = gameResourceURLData.url;
		NetworkMng.Instance.forceIconDisable(true);
		yield return StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, false, false));
		NetworkMng.Instance.forceIconDisable(false);
		WWW www2 = NetworkMng.Instance.getWWW();
		if (!Directory.Exists(Constant.GetDocumentsPath()))
		{
			Directory.CreateDirectory(Constant.GetDocumentsPath());
		}
		AssetBundle ab2 = www2.assetBundle;
		www2.Dispose();
		www2 = null;
		UnityEngine.Object[] objs = ab2.LoadAll();
		for (int i = 0; i < objs.Length; i++)
		{
			TextAsset asset = (TextAsset)objs[i];
			while (freeDiskSpace <= (ulong)asset.bytes.Length)
			{
				yield return StartCoroutine(capacityShortage());
				freeDiskSpace = FreeDiskSpace.getFreeDiskSpace();
				Debug.Log("strage maz : free disk space:" + freeDiskSpace);
			}
			freeDiskSpace -= (ulong)asset.bytes.Length;
			Debug.Log("free disk space:" + freeDiskSpace);
			File.WriteAllBytes(Aes.EncryptPath(Constant.GetDocumentsPath() + "/" + objs[i].name), asset.bytes);
			Resources.UnloadAsset(asset);
			yield return null;
			setDownloadPercent((float)dlCount / (float)dlCountMax + 1f / (float)dlCountMax / (float)objs.Length * (float)i);
		}
		ab2.Unload(true);
		ab2 = null;
		if (gameResourceURLData.versionCode != null)
		{
			PlayerPrefs.SetString("GameResourceVersionCode" + id, gameResourceURLData.versionCode);
		}
		PlayerPrefs.Save();
		setDownloadPercent((float)(dlCount + 1) / (float)dlCountMax);
		yield return null;
		if (bCloseDialog)
		{
			DialogBase dialog = dialogObj_.GetComponent<DialogBase>();
			yield return StartCoroutine(dialog.close());
			UnityEngine.Object.Destroy(dialogObj_);
			dialogObj_ = null;
			partManager.inhibitTips(false);
		}
		yield return Resources.UnloadUnusedAssets();
		bGameResourceDownload = false;
	}

	public bool isNewestGameResourceData(Constant.eDownloadDataNo baseNo, int dataNo, ResourceURLData resourceURLData)
	{
		int num = (int)(baseNo + dataNo);
		bool flag = true;
		if (baseNo == Constant.eDownloadDataNo.HowtoPlay_EN || baseNo == Constant.eDownloadDataNo.HowtoPlay_JP)
		{
			flag = false;
		}
		if (ResourceLoader.Instance.isUseLowResource() && flag)
		{
			num += 1000;
		}
		if (resourceURLData.gameResourceUrl == null)
		{
			return true;
		}
		GameResourceURLData gameResourceURLData = null;
		GameResourceURLData[] gameResourceUrl = resourceURLData.gameResourceUrl;
		foreach (GameResourceURLData gameResourceURLData2 in gameResourceUrl)
		{
			if (int.Parse(gameResourceURLData2.id) == num)
			{
				gameResourceURLData = gameResourceURLData2;
				break;
			}
		}
		string @string = PlayerPrefs.GetString("GameResourceVersionCode" + num, "-1");
		if (!(@string == gameResourceURLData.version))
		{
			return false;
		}
		string[] array = Constant.sample;
		if (num < 1000)
		{
			switch (baseNo)
			{
			case Constant.eDownloadDataNo.Main:
				array = Constant.outsideFiles_main[dataNo];
				break;
			case Constant.eDownloadDataNo.BG:
				array = Constant.outsideFiles_BG[dataNo];
				break;
			case Constant.eDownloadDataNo.WorldMap:
				array = Constant.outsideFiles_worldmap[dataNo];
				break;
			case Constant.eDownloadDataNo.ThrowChara:
				array = Constant.outsideFiles_throw_chara[dataNo];
				break;
			case Constant.eDownloadDataNo.SupportChara:
				array = Constant.outsideFiles_support_chara[dataNo];
				break;
			case Constant.eDownloadDataNo.ScenarioChara:
				array = Constant.outsideFiles_scenario_chara[dataNo];
				break;
			case Constant.eDownloadDataNo.HowtoPlay_JP:
				array = Constant.outsideFiles_howtoplay_JP[dataNo];
				break;
			case Constant.eDownloadDataNo.HowtoPlay_EN:
				array = Constant.outsideFiles_howtoplay_EN[dataNo];
				break;
			}
		}
		else
		{
			switch (baseNo)
			{
			case Constant.eDownloadDataNo.Main:
				array = Constant.outsideFilesLow_main[dataNo];
				break;
			case Constant.eDownloadDataNo.BG:
				array = Constant.outsideFilesLow_BG[dataNo];
				break;
			case Constant.eDownloadDataNo.WorldMap:
				array = Constant.outsideFilesLow_worldmap[dataNo];
				break;
			case Constant.eDownloadDataNo.ThrowChara:
				array = Constant.outsideFilesLow_throw_chara[dataNo];
				break;
			case Constant.eDownloadDataNo.SupportChara:
				array = Constant.outsideFilesLow_support_chara[dataNo];
				break;
			case Constant.eDownloadDataNo.ScenarioChara:
				array = Constant.outsideFilesLow_scenario_chara[dataNo];
				break;
			}
		}
		if (array == Constant.sample)
		{
			return true;
		}
		bool result = true;
		string[] array2 = array;
		foreach (string path in array2)
		{
			if (!Aes.Exists(Constant.GetDocumentsPath() + "/" + Path.GetFileNameWithoutExtension(path)))
			{
				result = false;
			}
		}
		return result;
	}

	public IEnumerator AssetBundleDownLoad()
	{
		bGameResourceDownload = true;
		float dlcount = 0f;
		float dlMaxcount = 5f;
		bool isError = false;
		PartManager partManager = GameObject.Find("_PartManager").GetComponent<PartManager>();
		dialogObj_ = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Download_Panel")) as GameObject;
		DialogBase dialog = dialogObj_.AddComponent<DialogBase>();
		Utility.setParent(dialogObj_, base.transform.parent.parent.parent.parent, false);
		dialog.init(partManager.dialogManager, partManager, partManager.fade, DialogManager.eDialog.DataDownload);
		dialogObj_.transform.Find("window/Download_Label").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(1480);
		dialogObj_.transform.Find("window/Instructions_Label").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(1481);
		setDownloadPercent(dlcount / dlMaxcount);
		partManager.inhibitTips(true);
		yield return StartCoroutine(dialog.open());
		AssetBundleLoader.Instance.StartLoadAssetBundle(delegate(float x, float y)
		{
			dlcount = x;
			dlMaxcount = y;
			setDownloadPercent(x / y);
		}, delegate
		{
			Debug.LogError("error");
			isError = true;
		});
		while (AssetBundleLoader.Instance.IsDownloading && !isError)
		{
			yield return null;
		}
		if (isError)
		{
			yield return StartCoroutine(dialog.close());
			UnityEngine.Object.Destroy(dialogObj_);
			dialogObj_ = null;
			yield return StartCoroutine(NetworkMng.Instance.openErrorDialog(false, eResultCode.ErrorUnknown));
		}
		else
		{
			setDownloadPercent(1f);
			yield return StartCoroutine(dialog.close());
			UnityEngine.Object.Destroy(dialogObj_);
			dialogObj_ = null;
			partManager.inhibitTips(false);
		}
	}

	private IEnumerator capacityShortage()
	{
		GameObject dialogObj = UnityEngine.Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "CommonDialog_Panel")) as GameObject;
		DialogCapacityShortage dialog = dialogObj.AddComponent<DialogCapacityShortage>();
		Utility.setParent(dialogObj, base.transform.parent.parent.parent.parent, true);
		PartManager partManager = GameObject.Find("_PartManager").GetComponent<PartManager>();
		dialog.init(partManager.dialogManager, partManager, partManager.fade, DialogManager.eDialog.CapacityShortage);
		dialog.OnCreate();
		NGUIUtility.setupButton(dialogObj, dialogObj, true);
		int count = Input.forceEnable();
		yield return StartCoroutine(dialog.open());
		while (dialog.isOpen())
		{
			yield return null;
		}
		Input.revertForceEnable(count);
		UnityEngine.Object.Destroy(dialogObj);
	}

	private void setDownloadPercent(float value)
	{
		dialogObj_.transform.Find("window/progress_Label").GetComponent<UILabel>().text = value.ToString("P0");
		dialogObj_.transform.Find("window/progress_gauge").GetComponent<UISlider>().sliderValue = value;
	}

	public static StageInfo.Info convertStageInfo(ParkStageInfo.Info park_info)
	{
		StageInfo.Info info = new StageInfo.Info();
		info.Area = park_info.Area;
		info.Common = new StageInfo.CommonInfo();
		info.Common.Coin = park_info.Common.Coin;
		info.Common.Bg = park_info.Common.Bg;
		info.Common.Exp = park_info.Common.Exp;
		info.Common.IsAllDelete = park_info.Common.IsAllDelete;
		info.Common.IsFriendDelete = park_info.Common.IsFriendDelete;
		info.Common.IsFulcrumDelete = park_info.Common.IsFulcrumDelete;
		info.Common.IsMinilenDelete = park_info.Common.IsMinilenDelete;
		info.Common.LoopLine = park_info.Common.LoopLine;
		info.Common.Move = park_info.Common.Move;
		info.Common.Score = park_info.Common.Score;
		info.Common.StageNo = park_info.Common.StageNo;
		info.Common.StarRewardNum = park_info.Common.StarRewardNum;
		info.Common.StarRewardType = park_info.Common.StarRewardType;
		info.Common.Time = park_info.Common.Time;
		info.Common.StarScores = park_info.Common.StarScores;
		info.Common.Continue = park_info.Common.Continue;
		info.Common.FreeItemNum = park_info.Common.FreeItemNum;
		info.Common.FreeItems = park_info.Common.FreeItems;
		info.Common.ItemNum = park_info.Common.ItemNum;
		info.Common.Items = park_info.Common.Items;
		info.Common.SpecialItemNum = park_info.Common.SpecialItemNum;
		info.Common.SpecialItems = park_info.Common.SpecialItems;
		info.Common.StageItemNum = park_info.Common.StageItemNum;
		info.Common.StageItems = park_info.Common.StageItems;
		return info;
	}
}
