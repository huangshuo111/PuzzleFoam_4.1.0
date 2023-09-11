using UnityEngine;

public static class Input
{
	private static ReferenceCounter refCounter_ = new ReferenceCounter(true, true);

	public static bool enable
	{
		get
		{
			return refCounter_.Reference;
		}
		set
		{
			refCounter_.Reference = value;
		}
	}

	public static int enableCount
	{
		get
		{
			return refCounter_.Count;
		}
	}

	public static Vector3 acceleration
	{
		get
		{
			return UnityEngine.Input.acceleration;
		}
	}

	public static int accelerationEventCount
	{
		get
		{
			return UnityEngine.Input.accelerationEventCount;
		}
	}

	public static bool anyKey
	{
		get
		{
			if (!enable)
			{
				return false;
			}
			return UnityEngine.Input.anyKey;
		}
	}

	public static bool anyKeyDown
	{
		get
		{
			if (!enable)
			{
				return false;
			}
			return UnityEngine.Input.anyKeyDown;
		}
	}

	public static Compass compass
	{
		get
		{
			return UnityEngine.Input.compass;
		}
	}

	public static Vector2 compositionCursorPos
	{
		get
		{
			return UnityEngine.Input.compositionCursorPos;
		}
		set
		{
			UnityEngine.Input.compositionCursorPos = value;
		}
	}

	public static string compositionString
	{
		get
		{
			return UnityEngine.Input.compositionString;
		}
	}

	public static DeviceOrientation deviceOrientation
	{
		get
		{
			return UnityEngine.Input.deviceOrientation;
		}
	}

	public static Gyroscope gyro
	{
		get
		{
			return UnityEngine.Input.gyro;
		}
	}

	public static IMECompositionMode imeCompositionMode
	{
		get
		{
			return UnityEngine.Input.imeCompositionMode;
		}
		set
		{
			UnityEngine.Input.imeCompositionMode = value;
		}
	}

	public static bool imeIsSelected
	{
		get
		{
			return UnityEngine.Input.imeIsSelected;
		}
	}

	public static string inputString
	{
		get
		{
			return UnityEngine.Input.inputString;
		}
	}

	public static Vector3 mousePosition
	{
		get
		{
			return UnityEngine.Input.mousePosition;
		}
	}

	public static bool multiTouchEnabled
	{
		get
		{
			return UnityEngine.Input.multiTouchEnabled;
		}
		set
		{
			UnityEngine.Input.multiTouchEnabled = value;
		}
	}

	public static int touchCount
	{
		get
		{
			if (!enable)
			{
				return 0;
			}
			return UnityEngine.Input.touchCount;
		}
	}

	public static Touch[] touches
	{
		get
		{
			if (!enable)
			{
				return null;
			}
			return UnityEngine.Input.touches;
		}
	}

	public static void resetCount()
	{
		refCounter_.reset();
	}

	public static void releaseCB()
	{
		refCounter_.releaseCB();
	}

	public static void addCB(ReferenceCounter.CallBackData data)
	{
		refCounter_.addCB(data);
	}

	public static void removeCB(ReferenceCounter.CallBackData data)
	{
		refCounter_.removeCB(data);
	}

	public static void clearCB()
	{
		refCounter_.clearCB();
	}

	public static int forceEnable()
	{
		int num = 0;
		while (!enable)
		{
			enable = true;
			num++;
		}
		return num;
	}

	public static void revertForceEnable(int count)
	{
		while (count > 0)
		{
			enable = false;
			count--;
		}
	}

	public static int forceDisable()
	{
		int num = 0;
		while (enable)
		{
			enable = false;
			num++;
		}
		return num;
	}

	public static void revertForceDisable(int count)
	{
		while (count > 0)
		{
			enable = true;
			count--;
		}
	}

	public static AccelerationEvent GetAccelerationEvent(int index)
	{
		return UnityEngine.Input.GetAccelerationEvent(index);
	}

	public static float GetAxis(string axisName)
	{
		return UnityEngine.Input.GetAxis(axisName);
	}

	public static float GetAxisRaw(string axisName)
	{
		return UnityEngine.Input.GetAxisRaw(axisName);
	}

	public static bool GetButton(string buttonName)
	{
		if (!enable)
		{
			return false;
		}
		return UnityEngine.Input.GetButton(buttonName);
	}

	public static bool GetButtonDown(string buttonName)
	{
		if (!enable)
		{
			return false;
		}
		return UnityEngine.Input.GetButtonDown(buttonName);
	}

	public static bool GetButtonUp(string buttonName)
	{
		if (!enable)
		{
			return false;
		}
		return UnityEngine.Input.GetButtonUp(buttonName);
	}

	public static string[] GetJoystickNames()
	{
		return UnityEngine.Input.GetJoystickNames();
	}

	public static bool GetKey(string name)
	{
		if (!enable)
		{
			return false;
		}
		return UnityEngine.Input.GetKey(name);
	}

	public static bool GetKey(KeyCode code)
	{
		if (!enable)
		{
			return false;
		}
		return UnityEngine.Input.GetKey(code);
	}

	public static bool GetKeyDown(string name)
	{
		if (!enable)
		{
			return false;
		}
		return UnityEngine.Input.GetKeyDown(name);
	}

	public static bool GetKeyDown(KeyCode code)
	{
		if (!enable)
		{
			return false;
		}
		return UnityEngine.Input.GetKeyDown(code);
	}

	public static bool GetKeyUp(string name)
	{
		if (!enable)
		{
			return false;
		}
		return UnityEngine.Input.GetKeyUp(name);
	}

	public static bool GetKeyUp(KeyCode code)
	{
		if (!enable)
		{
			return false;
		}
		return UnityEngine.Input.GetKeyUp(code);
	}

	public static bool GetMouseButton(int button)
	{
		if (!enable)
		{
			return false;
		}
		return UnityEngine.Input.GetMouseButton(button);
	}

	public static bool GetMouseButtonDown(int button)
	{
		if (!enable)
		{
			return false;
		}
		return UnityEngine.Input.GetMouseButtonDown(button);
	}

	public static bool GetMouseButtonUp(int button)
	{
		if (!enable)
		{
			return false;
		}
		return UnityEngine.Input.GetMouseButtonUp(button);
	}

	public static Touch GetTouch(int index)
	{
		return UnityEngine.Input.GetTouch(index);
	}

	public static void ResetInputAxes()
	{
		UnityEngine.Input.ResetInputAxes();
	}
}
