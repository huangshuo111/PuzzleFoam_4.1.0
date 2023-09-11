using System.Collections;
using UnityEngine;

public class DebugMenuTop : DebugMenuBase
{
	public string Name = "MenuTop";

	[SerializeField]
	private GameObject InputReject;

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(0, Name));
		GameObject go = GameObject.Find("_PartManager");
		if (go != null)
		{
			PartManager.ePart ep = go.GetComponent<PartManager>().currentPart;
			if (ep == PartManager.ePart.Stage || ep == PartManager.ePart.BossStage || ep == PartManager.ePart.BonusStage)
			{
				InputReject.SetActive(false);
			}
		}
	}

	public override void OnDraw()
	{
	}

	public override void OnExecute()
	{
	}
}
