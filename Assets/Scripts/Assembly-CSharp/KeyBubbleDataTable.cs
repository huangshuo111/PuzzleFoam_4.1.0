using UnityEngine;

public class KeyBubbleDataTable : MonoBehaviour
{
	private KeyBubbleInfo info_;

	private bool bLoaded_;

	public void load()
	{
		if (!bLoaded_)
		{
			TextAsset textAsset = Resources.Load("Parameter/key_bubble_info", typeof(TextAsset)) as TextAsset;
			info_ = Xml.DeserializeObject<KeyBubbleInfo>(textAsset.text) as KeyBubbleInfo;
			bLoaded_ = true;
		}
	}

	public KeyBubbleInfo getInfo()
	{
		return info_;
	}

	public float getPercentage(int index)
	{
		KeyBubbleInfo.PercentageInfo[] infos = info_.Infos;
		foreach (KeyBubbleInfo.PercentageInfo percentageInfo in infos)
		{
			if (percentageInfo.Index == index)
			{
				return percentageInfo.Percentage;
			}
		}
		return 0f;
	}

	public bool isLoaded()
	{
		return bLoaded_;
	}

	public void testLogging()
	{
		Debug.Log("KeyBubbleDataTable : testLogging");
		KeyBubbleInfo.PercentageInfo[] infos = info_.Infos;
		foreach (KeyBubbleInfo.PercentageInfo percentageInfo in infos)
		{
			Debug.Log("PercentageInfo.Index : " + percentageInfo.Index);
			Debug.Log("PercentageInfo.Percentage : " + percentageInfo.Percentage);
		}
	}
}
