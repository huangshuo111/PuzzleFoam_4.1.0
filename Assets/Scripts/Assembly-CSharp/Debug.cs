using UnityEngine;

public static class Debug
{
	public static bool isDebugBuild
	{
		get
		{
			return false;
		}
	}

	private static bool isValid()
	{
		return isDebugBuild;
	}

	public static void Break()
	{
		if (isValid())
		{
			UnityEngine.Debug.Break();
		}
	}

	public static void Log(object message, Object context)
	{
		if (isValid())
		{
			UnityEngine.Debug.Log(message, context);
		}
	}

	public static void Log(object message)
	{
		if (isValid())
		{
			UnityEngine.Debug.Log(message);
		}
	}

	public static void LogError(object message, Object context)
	{
		if (isValid())
		{
			UnityEngine.Debug.LogError(message, context);
		}
	}

	public static void LogError(object message)
	{
		if (isValid())
		{
			UnityEngine.Debug.LogError(message);
		}
	}

	public static void LogWarning(object message, Object context)
	{
		if (isValid())
		{
			UnityEngine.Debug.LogWarning(message, context);
		}
	}

	public static void LogWarning(object message)
	{
		if (isValid())
		{
			UnityEngine.Debug.LogWarning(message);
		}
	}

	public static void DrawLine(Vector3 start, Vector3 end)
	{
		if (isValid())
		{
			UnityEngine.Debug.DrawLine(start, end);
		}
	}

	public static void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		if (isValid())
		{
			UnityEngine.Debug.DrawLine(start, end, color);
		}
	}
}
