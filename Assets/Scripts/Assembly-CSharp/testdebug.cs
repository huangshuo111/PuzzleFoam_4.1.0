using UnityEngine;

public class testdebug : MonoBehaviour
{
	public static UILabel label;

	private static testdebug inst;

	public static testdebug _instance
	{
		get
		{
			if (inst == null)
			{
				inst = Object.FindObjectOfType(typeof(testdebug)) as testdebug;
			}
			return inst;
		}
	}

	public static void Log(string text)
	{
	}

	public void Update()
	{
	}
}
