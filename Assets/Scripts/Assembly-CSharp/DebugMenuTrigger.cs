using UnityEngine;

public class DebugMenuTrigger : MonoBehaviour
{
	private const float Length = 100f;

	private Rect beginTriggerArea;

	private Rect endTriggerArea;

	private bool bTapBeginArea_;

	private void OnCommandTrigger()
	{
		DebugMenuFactory.Instance.SetTrigger(isTriggerStandalone() || isTriggerAndroid() || isTriggerCommon());
	}

	private bool isTriggerStandalone()
	{
		return false;
	}

	private bool isTriggerAndroid()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Menu))
		{
			return true;
		}
		return false;
	}

	private bool isTriggerCommon()
	{
		if (UnityEngine.Input.GetMouseButtonUp(0))
		{
			updateArea();
			if (bTapBeginArea_ && endTriggerArea.Contains(UnityEngine.Input.mousePosition))
			{
				bTapBeginArea_ = false;
				return true;
			}
			if (beginTriggerArea.Contains(UnityEngine.Input.mousePosition))
			{
				bTapBeginArea_ = true;
				return false;
			}
			bTapBeginArea_ = false;
		}
		return false;
	}

	private void updateArea()
	{
		setArea(ref beginTriggerArea, 0f, (float)Screen.height - 100f, 100f, Screen.height);
		setArea(ref endTriggerArea, (float)Screen.width - 100f, (float)Screen.height - 100f, Screen.width, Screen.height);
	}

	private void setArea(ref Rect rect, float x, float y, float w, float h)
	{
		rect.x = x;
		rect.y = y;
		rect.width = w;
		rect.height = h;
	}

	private void Update()
	{
		if ((bool)DebugMenuFactory.Instance && DebugMenuFactory.Instance.IsCreateMenu() && UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			DebugMenu.Instance.PrevMenu();
		}
	}
}
