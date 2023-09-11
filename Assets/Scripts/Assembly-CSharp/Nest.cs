using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nest : GimmickBase
{
	public int row;

	public int column;

	public StagePause_Boss stagePause_;

	public Part_BossStage part_;

	private int spriteCount;

	public List<GameObject> childList;

	public IEnumerator NestInit()
	{
		spriteCount = base.transform.childCount;
		childList = new List<GameObject>();
		childList.Clear();
		for (int i = 0; i < spriteCount; i++)
		{
			childList.Add(base.transform.Find("bg" + i.ToString("00")).gameObject);
		}
		part_.nestList = childList;
		yield return stagePause_.sync();
	}
}
