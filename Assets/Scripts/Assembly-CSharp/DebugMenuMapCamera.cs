using System.Collections;
using UnityEngine;

public class DebugMenuMapCamera : DebugMenuBase
{
	private enum eItem
	{
		PinchMin = 0,
		PinchMax = 1,
		PinchScale = 2,
		Max = 3
	}

	private MapCamera mapCamera_;

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(3, "MapCamera"));
		GameObject obj = GameObject.Find("MapCamera(Clone)");
		if (obj != null)
		{
			mapCamera_ = obj.GetComponent<MapCamera>();
		}
	}

	public override void OnDraw()
	{
		if (!(mapCamera_ == null))
		{
			DrawItem(0, "PinchMin : " + mapCamera_.DebugPinchScaleMin);
			DrawItem(1, "PinchMax : " + mapCamera_.DebugPinchScaleMax);
			DrawItem(2, "PinchScale : " + mapCamera_.DebugPinchScaleFactor);
		}
	}

	public override void OnExecute()
	{
		if (!(mapCamera_ == null))
		{
			mapCamera_.DebugPinchScaleMin = (float)Vary(0, mapCamera_.DebugPinchScaleMin, 0.1f, 0f, 10f);
			mapCamera_.DebugPinchScaleMax = (float)Vary(1, mapCamera_.DebugPinchScaleMax, 0.1f, 0f, 10f);
			mapCamera_.DebugPinchScaleFactor = (float)Vary(2, mapCamera_.DebugPinchScaleFactor, 0.001f, 0f, 10f);
		}
	}
}
