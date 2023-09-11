using System.Collections;
using UnityEngine;

public class PlayerIcon : MonoBehaviour
{
	public enum eState
	{
		Normal = 0,
		Loading = 1,
		Error = 2
	}

	[SerializeField]
	private UITexture Icon;

	[SerializeField]
	private Material Material;

	private Material mat_;

	private Texture2D loadTexture_;

	private eState state_;

	public Material createMaterial()
	{
		if (mat_ == null)
		{
			mat_ = Object.Instantiate(Material) as Material;
			Icon.material = mat_;
		}
		return mat_;
	}

	public void setTexture(Texture texture)
	{
		if (Icon != null)
		{
			Icon.mainTexture = texture;
		}
	}

	public eState getState()
	{
		return state_;
	}

	public void createIcon(Texture texture)
	{
		Material material = createMaterial();
		material.mainTexture = texture;
		Icon.material = material;
	}

	public IEnumerator loadTexture(string url, bool bSet, UserData data)
	{
		state_ = eState.Loading;
		if (string.IsNullOrEmpty(url))
		{
			state_ = eState.Error;
			yield break;
		}
		int failedCount = 0;
		WWW www;
		while (true)
		{
			www = new WWW(url);
			while (!www.isDone && www.error == null)
			{
				yield return null;
			}
			if (www.error == null)
			{
				break;
			}
			www.Dispose();
			failedCount++;
			if (failedCount >= 3)
			{
				state_ = eState.Error;
				yield break;
			}
		}
		loadTexture_ = www.textureNonReadable;
		www.Dispose();
		state_ = eState.Normal;
		if (bSet)
		{
			setTexture(loadTexture_);
		}
		if (data != null)
		{
			data.Texture = loadTexture_;
		}
	}

	public IEnumerator loadTexture(string url, bool bSet, UserData[] data, int index)
	{
		state_ = eState.Loading;
		if (string.IsNullOrEmpty(url))
		{
			state_ = eState.Error;
			yield break;
		}
		int failedCount = 0;
		WWW www;
		while (true)
		{
			www = new WWW(url);
			while (!www.isDone && www.error == null)
			{
				yield return null;
			}
			if (www.error == null)
			{
				break;
			}
			www.Dispose();
			failedCount++;
			if (failedCount >= 3)
			{
				state_ = eState.Error;
				yield break;
			}
		}
		loadTexture_ = www.textureNonReadable;
		www.Dispose();
		state_ = eState.Normal;
		if (bSet)
		{
			setTexture(loadTexture_);
		}
		if (data != null && index != -1)
		{
			data[index].Texture = loadTexture_;
		}
	}

	public Texture2D getLoadTexture()
	{
		return loadTexture_;
	}

	private void OnDestroy()
	{
		mat_ = null;
	}
}
