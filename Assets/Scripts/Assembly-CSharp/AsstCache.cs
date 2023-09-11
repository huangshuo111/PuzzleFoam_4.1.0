using System;
using System.Collections.Generic;
using UnityEngine;

public class AsstCache
{
	private static Dictionary<string, AssetBundle> assetBundleCacheDic = new Dictionary<string, AssetBundle>();

	private static int loadSteps = 0;

	public static AssetBundle LoadAssetBundleFromCache(string path)
	{
		if (++loadSteps % 50 == 0)
		{
			ClearAssetBundleCache();
		}
		if (assetBundleCacheDic.ContainsKey(path))
		{
			return assetBundleCacheDic[path];
		}
		AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
		assetBundleCacheDic.Add(path, assetBundle);
		return assetBundle;
	}

	public static void ClearAssetBundleCache()
	{
		List<string> list = new List<string>(assetBundleCacheDic.Keys);
		foreach (string item in list)
		{
			AssetBundle assetBundle = assetBundleCacheDic[item];
			assetBundle.Unload(false);
		}
		assetBundleCacheDic.Clear();
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}
}
