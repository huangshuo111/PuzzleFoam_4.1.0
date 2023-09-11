using UnityEngine;

public class ParkRoadPlacedDataTable : MonoBehaviour
{
	private ParkRoadPlacedInfo roadPlacedInfo_;

	public void load()
	{
		if (roadPlacedInfo_ != null)
		{
			return;
		}
		TextAsset textAsset = Resources.Load<TextAsset>("Parameter/road_list");
		if (textAsset == null)
		{
			return;
		}
		roadPlacedInfo_ = new ParkRoadPlacedInfo();
		string[] array = textAsset.text.Split('|');
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Replace("\t", string.Empty);
				text = text.Replace(" ", string.Empty);
				text = text.Replace("{", string.Empty);
				text = text.Replace("}", string.Empty);
				string[] array2 = text.Split(',');
				ParkRoadPlacedInfo.RoadPlacedInfo roadPlacedInfo = new ParkRoadPlacedInfo.RoadPlacedInfo();
				roadPlacedInfo.X = int.Parse(array2[0]);
				roadPlacedInfo.Y = int.Parse(array2[1]);
				string text2 = array2[2];
				int value = int.Parse(text2);
				roadPlacedInfo.SpriteID = Mathf.Abs(value);
				roadPlacedInfo.IsReverse = (text2.Contains("-") ? true : false);
				roadPlacedInfo_.RoadPlacedInfos.Add(roadPlacedInfo);
			}
		}
	}

	public ParkRoadPlacedInfo.RoadPlacedInfo[] getAllInfo()
	{
		if (roadPlacedInfo_ == null)
		{
			return null;
		}
		return roadPlacedInfo_.RoadPlacedInfos.ToArray();
	}
}
