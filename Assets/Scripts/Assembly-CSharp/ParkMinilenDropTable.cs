using UnityEngine;

public class ParkMinilenDropTable : MonoBehaviour
{
	public class MinilenDropInfo
	{
		public class Info
		{
			public int ID;

			public int GroupId;

			public int MinilenId;
		}

		public Info[] Infos;
	}

	private MinilenDropInfo _minilen_drop_info;

	private void Start()
	{
		TextAsset textAsset = Resources.Load("Parameter/StageData/park_minilen_drop", typeof(TextAsset)) as TextAsset;
		_minilen_drop_info = Xml.DeserializeObject<MinilenDropInfo>(textAsset.text) as MinilenDropInfo;
	}
}
