using System;
using UnityEngine;

public class Friend : PlayerIcon
{
	public enum ePalce
	{
		Center = 0,
		LeftBottom = 1,
		LeftTop = 2,
		Max = 3
	}

	[Serializable]
	public class PlaceInfo
	{
		public Vector3 Pos;

		public Vector3 Scale;
	}

	private Part_Map map;

	private int friendsNum;

	public PlaceInfo[] Places = new PlaceInfo[3];

	private int stageNo_;

	public void setup(Vector3 pos, int stageNo, Part_Map _map, int _num)
	{
		stageNo_ = stageNo;
		setPlace(pos, ePalce.Center);
		map = _map;
		friendsNum = _num;
	}

	public int getStageNo()
	{
		return stageNo_;
	}

	public void setPlace(Vector3 pos, ePalce place)
	{
		if (Places.Length <= (int)place || pos == Vector3.zero)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		PlaceInfo placeInfo = Places[(int)place];
		Vector3 vector = pos + placeInfo.Pos;
		Vector3 scale = placeInfo.Scale;
		base.transform.localPosition = new Vector3(vector.x, vector.y, placeInfo.Pos.z);
		base.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
	}

	private void TapEvent()
	{
		StartCoroutine(map.showDialog_FriendData(friendsNum));
	}
}
