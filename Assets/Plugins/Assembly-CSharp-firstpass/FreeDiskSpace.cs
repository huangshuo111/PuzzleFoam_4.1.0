using UnityEngine;

public class FreeDiskSpace
{
	public static ulong getFreeDiskSpace()
	{
		ulong num = 0uL;
		ulong num2 = 0uL;
		using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.os.StatFs", Application.persistentDataPath))
		{
			if (androidJavaObject != null)
			{
				num = (ulong)androidJavaObject.Call<int>("getBlockSize", new object[0]);
				num2 = (ulong)androidJavaObject.Call<int>("getAvailableBlocks", new object[0]);
			}
		}
		return num * num2;
	}
}
