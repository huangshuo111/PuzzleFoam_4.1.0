using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
	private static ResourceLoader instance_;

	private bool bLoaded_;

	private bool bUseLowResource_;

	private Dictionary<string, bool> infoDict_ = new Dictionary<string, bool>();

	private string language_ = "JP";

	public static ResourceLoader Instance
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
			Object.DontDestroyOnLoad(this);
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

	public void init(bool bUseLowResource)
	{
		bUseLowResource_ = bUseLowResource;
	}

	public void loadList()
	{
		if (bLoaded_)
		{
			return;
		}
		TextAsset textAsset = Resources.Load("Parameter/resource_info", typeof(TextAsset)) as TextAsset;
		string[] array = textAsset.text.Split('\n');
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.Length <= 0)
			{
				break;
			}
			string[] array3 = text.Split(',');
			string text2 = array3[1];
			bool value = ((array3[0] == "共通") ? true : false);
			text2 = text2.Replace("\r", string.Empty);
			infoDict_[text2] = value;
		}
		bLoaded_ = true;
	}

	public GameObject loadGameObject(string path, string prefabName)
	{
		if (!bLoaded_)
		{
			return null;
		}
		if (!infoDict_.ContainsKey(prefabName))
		{
			return null;
		}
		string text = "Common";
		if (!infoDict_[prefabName])
		{
			text = language_;
		}
		if (bUseLowResource_)
		{
			path = path.Replace("Prefabs/", "Prefabs_Low/");
		}
		string text2 = path + text + "/" + prefabName;
		if (AssetBundleLoader.Instance.CheckContain(text2))
		{
			return AssetBundleLoader.Instance.LoadAsset<GameObject>(text2, prefabName);
		}
		return Resources.Load(text2, typeof(GameObject)) as GameObject;
	}

	public Texture2D loadFromGameResource(string filename)
	{
		string path = Constant.GetDocumentsPath() + "/" + filename;
		Texture2D texture2D = new Texture2D(4, 4, TextureFormat.RGBA32, false);
		if (Aes.Exists(path))
		{
			texture2D.LoadImage(File.ReadAllBytes(Aes.EncryptPath(path)));
		}
		texture2D.wrapMode = TextureWrapMode.Clamp;
		return texture2D;
	}

	public bool isJapanResource()
	{
		return language_ == "JP";
	}

	public bool isUseLowResource()
	{
		return bUseLowResource_;
	}

	public GameObject instantiateGameObject(string path, string prefabName, Transform parent, bool bClearLocalTransform, bool bActive)
	{
		GameObject gameObject = Object.Instantiate(Instance.loadGameObject(path, prefabName)) as GameObject;
		if ((bool)gameObject)
		{
			Utility.setParent(gameObject, parent, bClearLocalTransform);
			gameObject.SetActive(bActive);
		}
		return gameObject;
	}
}
