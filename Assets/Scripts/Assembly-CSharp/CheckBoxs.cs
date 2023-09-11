using UnityEngine;

public class CheckBoxs : MonoBehaviour
{
	[SerializeField]
	private CheckBox[] Targets;

	[SerializeField]
	private Vector2[] PosPatterns;

	private void Awake()
	{
	}

	public void setup(int num)
	{
		for (int i = 0; i < Targets.Length; i++)
		{
			Targets[i].gameObject.SetActive(i < num);
		}
		base.transform.localPosition = PosPatterns[num - 1];
	}

	public void setText(int index, string text)
	{
		if (!isIndexOver(index))
		{
			Targets[index].setLabel(text);
		}
	}

	public void setCheck(int index, bool bEnable)
	{
		if (!isIndexOver(index))
		{
			Targets[index].setCheck(bEnable);
		}
	}

	private bool isIndexOver(int index)
	{
		if (index < 0 || index >= Targets.Length)
		{
			return true;
		}
		return false;
	}
}
