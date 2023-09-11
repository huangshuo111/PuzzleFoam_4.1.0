using System.Collections.Generic;
using UnityEngine;

public class TreasureBoxParent : MonoBehaviour
{
	private Dictionary<int, TreasureBox> boxDict_ = new Dictionary<int, TreasureBox>();

	public void setup()
	{
		TreasureBox[] componentsInChildren = GetComponentsInChildren<TreasureBox>(true);
		TreasureBox[] array = componentsInChildren;
		foreach (TreasureBox treasureBox in array)
		{
			boxDict_[treasureBox.getID()] = treasureBox;
		}
	}

	public Dictionary<int, TreasureBox>.KeyCollection getKeys()
	{
		return boxDict_.Keys;
	}

	public TreasureBox getBox(int id)
	{
		if (!boxDict_.ContainsKey(id))
		{
			return null;
		}
		return boxDict_[id];
	}
}
