using System.Collections.Generic;
using UnityEngine;

public class GlobalRoot : MonoBehaviour
{
	private static GlobalRoot instance_;

	private Dictionary<string, GameObject> appendObjDict_ = new Dictionary<string, GameObject>();

	public static GlobalRoot Instance
	{
		get
		{
			return instance_;
		}
	}

	public static bool IsInstance()
	{
		return (instance_ != null) ? true : false;
	}

	private void OnDestroy()
	{
		appendObjDict_.Clear();
		instance_ = null;
	}

	private void Awake()
	{
		if (instance_ == null)
		{
			instance_ = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public bool isAppend(string appendName)
	{
		return appendObjDict_.ContainsKey(appendName);
	}

	public bool isAppend(GlobalObjectParam.eObject objType)
	{
		string key = GlobalObjectParam.getName(objType);
		return appendObjDict_.ContainsKey(key);
	}

	public GameObject load(string path, bool bClearTransform)
	{
		string text = path.Substring(path.LastIndexOf("/") + 1);
		return load(path, text, text, bClearTransform);
	}

	public GameObject load(string path, GlobalObjectParam.eObject objType, bool bClearTransform)
	{
		string text = GlobalObjectParam.getName(objType);
		return load(path, text, text, bClearTransform);
	}

	public GameObject load(string path, string prefabName, GlobalObjectParam.eObject objType, bool bClearTransform)
	{
		return load(path, prefabName, GlobalObjectParam.getName(objType), bClearTransform);
	}

	public GameObject load(string path, string prefabName, string appendName, bool bClearTransform)
	{
		if (isAppend(appendName))
		{
			return null;
		}
		GameObject gameObject = Object.Instantiate(ResourceLoader.Instance.loadGameObject(path, prefabName)) as GameObject;
		Utility.setParent(gameObject, base.transform, bClearTransform);
		appendObjDict_[appendName] = gameObject;
		return gameObject;
	}

	public void unload(GlobalObjectParam.eObject objType)
	{
		unload(GlobalObjectParam.getName(objType));
	}

	public void unload(string appendName)
	{
		if (isAppend(appendName))
		{
			Object.DestroyImmediate(appendObjDict_[appendName]);
			appendObjDict_.Remove(appendName);
			Resources.UnloadUnusedAssets();
		}
	}

	public void appendObject(GameObject obj, GlobalObjectParam.eObject objType, bool bClearTransform)
	{
		appendObject(obj, GlobalObjectParam.getName(objType), bClearTransform);
	}

	public void appendObject(GameObject obj, string appendName, bool bClearTransform)
	{
		if (!isAppend(appendName))
		{
			Utility.setParent(obj, base.transform, bClearTransform);
			appendObjDict_[appendName] = obj;
		}
	}

	public GameObject getObject(string appendName)
	{
		if (!isAppend(appendName))
		{
			return null;
		}
		return appendObjDict_[appendName];
	}

	public GameObject getObject(GlobalObjectParam.eObject objType)
	{
		string text = GlobalObjectParam.getName(objType);
		if (!isAppend(text))
		{
			return null;
		}
		return appendObjDict_[text];
	}
}
