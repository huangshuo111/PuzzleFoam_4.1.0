using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class AssetBundleLoader : MonoBehaviour
{
	public struct AssetInfo
	{
		public string serverFilePath;

		public string outPath;

		public string AssetBundleFileName;

		public string Version;
	}

	public struct ResourceInfo
	{
		public string AssetBundlePath;

		public string path;

		public string fileName;
	}

	private const string AssetBundleInfoFileName = "AssetBundleInfo.txt";

	private const string ResourceInfoFileName = "ResourcesInfo.txt";

	private const string SubPath = "Android";

	private const float TimeoutSeconds = 90f;

	private string VersionPath = string.Empty;

	private List<AssetInfo> assetInfos;

	private List<ResourceInfo> resourceInfos;

	private Dictionary<string, ResourceInfo> resourceInfoDic;

	private Action<float, float> onDownLoadCallBack;

	private Action<string> onDownloadFailCallBack;

	private int downloadedNum;

	private static AssetBundleLoader instance_;

	private string AssetBundleSavePath
	{
		get
		{
			return Application.persistentDataPath + "/AssetBundle";
		}
	}

	private string AssetBundleInfoURL
	{
		get
		{
			return ProjectSettings.Instance.AssetBundleServerIP + VersionPath + "Android/AssetBundleInfo.txt";
		}
	}

	private string ResourceInfoURL
	{
		get
		{
			return ProjectSettings.Instance.AssetBundleServerIP + VersionPath + "Android/ResourcesInfo.txt";
		}
	}

	public bool IsDownloading { get; private set; }

	public bool HasError { get; private set; }

	public static AssetBundleLoader Instance
	{
		get
		{
			return instance_;
		}
	}

	private void Awake()
	{
		if (instance_ == null)
		{
			instance_ = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		TextAsset textAsset = Resources.Load("Parameter/version") as TextAsset;
		VersionPath = textAsset.text + "/";
	}

	public void StartLoadAssetBundle(Action<float, float> downloadCallback, Action<string> onFailCallBack)
	{
		NetworkMng.Instance.statusChangeLoad();
		IsDownloading = true;
		HasError = false;
		downloadedNum = 0;
		onDownLoadCallBack = downloadCallback;
		onDownloadFailCallBack = onFailCallBack;
		StartCoroutine(coroutineLoadData());
	}

	private void onDownloadComplete1File(int fileNum)
	{
		downloadedNum = fileNum + 1;
		if (downloadedNum >= assetInfos.Count)
		{
			Resources.UnloadUnusedAssets();
			GC.Collect();
			IsDownloading = false;
		}
	}

	private void updateDownload(float percent)
	{
		onDownLoadCallBack((float)downloadedNum + percent, assetInfos.Count);
	}

	private void onDownloadFial(string error)
	{
		HasError = true;
		onDownloadFailCallBack(error);
	}

	public bool CheckContain(string path)
	{
		if (resourceInfoDic == null)
		{
			return false;
		}
		return resourceInfoDic.ContainsKey(path);
	}

	public TYPE LoadAsset<TYPE>(string fullPath, string fileName) where TYPE : UnityEngine.Object
	{
		try
		{
			ResourceInfo resourceInfo = resourceInfoDic[fullPath];
			AssetBundle assetBundle = AsstCache.LoadAssetBundleFromCache(AssetBundleSavePath + resourceInfo.AssetBundlePath);
			UnityEngine.Object @object = assetBundle.Load(fileName, typeof(TYPE));
			return @object as TYPE;
		}
		catch (Exception)
		{
		}
		return (TYPE)null;
	}

	private IEnumerator coroutineLoadData()
	{
		string assetInfoStr = string.Empty;
		yield return StartCoroutine(coroutineDownLoadParam(AssetBundleInfoURL, delegate(string res)
		{
			assetInfoStr = res;
		}));
		if (HasError)
		{
			yield break;
		}
		loadAssetInfo(assetInfoStr);
		if (!checkServerVersionIsNewALL(assetInfos))
		{
			string resoruceInfoStr = loadParamTextFile(AssetBundleSavePath + "/ResourcesInfo.txt");
			loadResourceInfo(resoruceInfoStr);
			initDic();
			onDownloadComplete1File(assetInfos.Count - 1);
			yield break;
		}
		string resourceInfoStr = string.Empty;
		yield return StartCoroutine(coroutineDownLoadParam(ResourceInfoURL, delegate(string res)
		{
			resourceInfoStr = res;
		}));
		if (!HasError)
		{
			loadResourceInfo(resourceInfoStr);
			createDirectoryIfNotExist(AssetBundleSavePath);
			saveParamTextFile(AssetBundleSavePath + "/ResourcesInfo.txt", resourceInfoStr);
			initDic();
			yield return StartCoroutine(coroutineDownLoadAssetBundle());
			if (!HasError)
			{
				saveServerVersion(assetInfos);
			}
		}
	}

	private void loadAssetInfo(string infoStr)
	{
		string[] array = infoStr.Split('\n');
		int num = array.Length;
		assetInfos = new List<AssetInfo>();
		for (int i = 0; i < num; i++)
		{
			string[] array2 = array[i].Split(',');
			if (array2.Length < 4)
			{
				break;
			}
			AssetInfo item = default(AssetInfo);
			item.serverFilePath = array2[0].Trim();
			item.outPath = array2[1].Trim();
			item.AssetBundleFileName = array2[2].Trim();
			item.Version = array2[3].Trim();
			assetInfos.Add(item);
		}
	}

	private void loadResourceInfo(string infoStr)
	{
		string[] array = infoStr.Split('\n');
		int num = array.Length;
		resourceInfos = new List<ResourceInfo>();
		for (int i = 0; i < num; i++)
		{
			string[] array2 = array[i].Split(',');
			if (array2.Length < 2)
			{
				break;
			}
			ResourceInfo item = default(ResourceInfo);
			item.AssetBundlePath = array2[0].Trim();
			item.path = array2[1].Trim();
			item.fileName = array2[2].Trim();
			resourceInfos.Add(item);
		}
	}

	private IEnumerator coroutineDownLoadAssetBundle()
	{
		yield return new WaitForEndOfFrame();
		for (int i = 0; i < assetInfos.Count; i++)
		{
			if (!checkServerVersionIsNew(assetInfos[i]))
			{
				onDownloadComplete1File(i);
				continue;
			}
			WWW www = new WWW(toDownloadURL(assetInfos[i].serverFilePath));
			float timeOutTick = 0f;
			float lastProgressNum = 0f;
			while (!www.isDone)
			{
				if (www.progress - lastProgressNum <= 0.001f)
				{
					timeOutTick += Time.deltaTime;
					if (timeOutTick >= 90f)
					{
						onDownloadFial("Download error " + toDownloadURL(assetInfos[i].serverFilePath) + " ::TimeOut");
						yield break;
					}
				}
				else
				{
					lastProgressNum = www.progress;
				}
				updateDownload(www.progress);
				yield return new WaitForEndOfFrame();
			}
			if (!string.IsNullOrEmpty(www.error))
			{
				onDownloadFial("Download error " + toDownloadURL(assetInfos[i].serverFilePath) + " ::" + www.error);
				break;
			}
			yield return new WaitForEndOfFrame();
			byte[] bytes = www.bytes;
			string saveDir = AssetBundleSavePath + assetInfos[i].outPath;
			string savePath = saveDir + "/" + assetInfos[i].AssetBundleFileName;
			createDirectoryIfNotExist(saveDir);
			saveAssetBundleFile(bytes, savePath);
			onDownloadComplete1File(i);
		}
	}

	private bool saveAssetBundleFile(byte[] bytes, string path)
	{
		try
		{
			File.WriteAllBytes(path, bytes);
			return true;
		}
		catch (Exception ex)
		{
			onDownloadFial("AssetBundle save failed " + ex.Message);
			return false;
		}
	}

	private void initDic()
	{
		resourceInfoDic = new Dictionary<string, ResourceInfo>();
		foreach (ResourceInfo resourceInfo in resourceInfos)
		{
			string key = resourceInfo.path + resourceInfo.fileName;
			resourceInfoDic.Add(key, resourceInfo);
		}
	}

	private bool checkServerVersionIsNew(AssetInfo newInfo)
	{
		string text = newInfo.outPath + "/" + newInfo.AssetBundleFileName;
		string @string = PlayerPrefs.GetString("ABKey" + text, string.Empty);
		if (newInfo.Version != @string)
		{
			return true;
		}
		return false;
	}

	private bool checkServerVersionIsNewALL(List<AssetInfo> newInfo)
	{
		foreach (AssetInfo item in newInfo)
		{
			if (checkServerVersionIsNew(item))
			{
				return true;
			}
		}
		return false;
	}

	private void saveServerVersion(List<AssetInfo> newInfo)
	{
		foreach (AssetInfo item in newInfo)
		{
			string text = item.outPath + "/" + item.AssetBundleFileName;
			PlayerPrefs.SetString("ABKey" + text, item.Version);
		}
	}

	private IEnumerator coroutineDownLoadParam(string url, Action<string> callback)
	{
		WWW www2 = new WWW(url);
		float timeOutTick = 0f;
		float lastProgressNum = 0f;
		while (!www2.isDone)
		{
			if (www2.progress - lastProgressNum <= 0.001f)
			{
				timeOutTick += Time.deltaTime;
				if (timeOutTick >= 90f)
				{
					onDownloadFial("Download error " + url + " ::TimeOut");
					yield break;
				}
			}
			else
			{
				lastProgressNum = www2.progress;
			}
			yield return new WaitForEndOfFrame();
		}
		if (!string.IsNullOrEmpty(www2.error))
		{
			onDownloadFial("Cannnot Download " + url + " ::" + www2.error);
			HasError = true;
		}
		else
		{
			callback(www2.text);
			www2 = null;
		}
	}

	private void saveParamTextFile(string path, string text)
	{
		try
		{
			StreamWriter streamWriter = new StreamWriter(path, false, new UTF8Encoding(false));
			streamWriter.NewLine = "\n";
			streamWriter.Write(text);
			streamWriter.Close();
		}
		catch (Exception ex)
		{
			onDownloadFial("Cannnot write file " + path + " ::" + ex.Message);
		}
	}

	private string loadParamTextFile(string path)
	{
		try
		{
			StreamReader streamReader = new StreamReader(path);
			string result = streamReader.ReadToEnd();
			streamReader.Close();
			return result;
		}
		catch (Exception ex)
		{
			onDownloadFial("Cannnot read file " + path + " ::" + ex.Message);
		}
		return string.Empty;
	}

	private static void createDirectoryIfNotExist(string path)
	{
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
	}

	private string toDownloadURL(string path)
	{
		return ProjectSettings.Instance.AssetBundleServerIP + VersionPath + "Android" + path;
	}
}
