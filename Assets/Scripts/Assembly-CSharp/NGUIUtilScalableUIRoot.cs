using UnityEngine;

public class NGUIUtilScalableUIRoot : MonoBehaviour
{
	public static int manualWidth = 640;

	public static int manualHeight = 960;

	private static int uiOffsetY;

	private static int uiOffsetX;

	private static UIRoot uiRoot_;

	private void Awake()
	{
		uiRoot_ = GetComponent<UIRoot>();
		uiRoot_.automatic = false;
		uiRoot_.minimumHeight = 1;
		uiRoot_.maximumHeight = int.MaxValue;
		uiOffsetY = (uiRoot_.manualHeight - manualHeight) / 2;
	}

	private void Update()
	{
		if ((bool)uiRoot_)
		{
			int num = manualHeight;
			int num2 = manualWidth;
			float num3 = (float)(Screen.height * manualWidth) / (float)(Screen.width * manualHeight);
			if (num3 > 1f)
			{
				num = (int)((float)num * num3);
				num2 = (int)((float)num2 * num3);
			}
			if (uiRoot_.manualHeight != num)
			{
				uiRoot_.manualHeight = num;
				uiOffsetY = (num - manualHeight) / 2;
				uiOffsetX = (num2 - manualWidth) / 2;
			}
		}
	}

	public static void OffsetUI(Transform ui, bool isTop)
	{
		if (isTop)
		{
			ui.localPosition += Vector3.up * uiOffsetY;
		}
		else
		{
			ui.localPosition += Vector3.down * uiOffsetY;
		}
	}

	public static Vector3 GetOffsetY(bool isTop)
	{
		if (isTop)
		{
			return Vector3.up * uiOffsetY;
		}
		return Vector3.down * uiOffsetY;
	}

	public static Vector3 GetOffsetX(bool isLeft)
	{
		if (isLeft)
		{
			return Vector3.left * uiOffsetX;
		}
		return Vector3.right * uiOffsetX;
	}

	public static void ScalingUI(Transform ui)
	{
		Vector3 localScale = ui.localScale;
		localScale.y *= (float)uiRoot_.manualHeight / (float)manualHeight;
		ui.localScale = localScale;
	}
}
