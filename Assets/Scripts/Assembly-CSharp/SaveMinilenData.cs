using System;
using Network;
using UnityEngine;

public class SaveMinilenData : MonoBehaviour
{
	private MinilenDataTable _minilen_data_table;

	private Network.MinilenData[] _minilen_list;

	private int _wear_minilen_id = 30000;

	public Network.MinilenData[] minilenList
	{
		get
		{
			return _minilen_list;
		}
	}

	public void setup()
	{
	}

	private Network.MinilenData Load(int id)
	{
		return null;
	}

	private Network.MinilenData getMinilenData(int id)
	{
		return null;
	}

	public void setLevel(int id, int level)
	{
	}

	public void Wear(int id)
	{
	}

	public Network.MinilenData getWearing()
	{
		Network.MinilenData minilenData = Array.Find(_minilen_list, (Network.MinilenData m) => m.index == _wear_minilen_id);
		if (minilenData == null)
		{
			minilenData = getMinilenData(30000);
			if (minilenData == null)
			{
				Debug.LogError("Couldn't Find Wearing Minilen !!");
			}
		}
		return minilenData;
	}
}
