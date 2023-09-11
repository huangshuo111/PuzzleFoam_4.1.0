using UnityEngine;

public class Editor_ObjectSelect : MonoBehaviour
{
	public static int selectIndex;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void button(GameObject obj)
	{
		for (int i = 0; i < Editor_Main.bossObjectNames.Length; i++)
		{
			if (obj.name == Editor_Main.bossObjectNames[i])
			{
				Debug.Log("find BossObject name : " + Editor_Main.bossObjectNames[i]);
				selectIndex = i;
			}
		}
		UILabel[] componentsInChildren = obj.transform.parent.GetComponentsInChildren<UILabel>();
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			uILabel.effectColor = ((!(uILabel.name == obj.name)) ? Color.black : Color.red);
		}
	}
}
