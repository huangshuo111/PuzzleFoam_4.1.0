using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class Utility
{
	private static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0);

	private static readonly string EncryptString = "gumikorea!@#$%^&";

	public static void clearLocalTransform(Transform t)
	{
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;
	}

	public static GameObject createObject(string name, Transform parent)
	{
		return createObject(name, parent, true);
	}

	public static GameObject createObject(string name, Transform parent, bool bClearForm)
	{
		GameObject gameObject = new GameObject(name);
		setParent(gameObject, parent, bClearForm);
		return gameObject;
	}

	public static void setParent(GameObject obj, Transform parent, bool bClearLocalTransform)
	{
		Vector3 localPosition = obj.transform.localPosition;
		Quaternion localRotation = obj.transform.localRotation;
		Vector3 localScale = obj.transform.localScale;
		obj.transform.parent = parent;
		if (bClearLocalTransform)
		{
			clearLocalTransform(obj.transform);
			return;
		}
		obj.transform.localPosition = localPosition;
		obj.transform.localRotation = localRotation;
		obj.transform.localScale = localScale;
	}

	public static int GetLenByte(string str)
	{
		int num = 0;
		foreach (char c in str)
		{
			num = ((!IsEn(c)) ? (num + 2) : (num + 1));
		}
		return num;
	}

	public static bool IsEn(char c)
	{
		if ('\0' <= c && c <= '\u007f')
		{
			return true;
		}
		return false;
	}

	public static string LeftB(string str, int byteSize)
	{
		int num = 0;
		for (int i = 0; i < str.Length; i++)
		{
			char c = str[i];
			num = ((!IsEn(c)) ? (num + 2) : (num + 1));
			if (num > byteSize)
			{
				return str.Substring(0, i);
			}
		}
		return str;
	}

	public static IEnumerator clickCoroutine()
	{
		while (!Input.GetMouseButtonDown(0))
		{
			yield return 0;
		}
		while (!Input.GetMouseButtonUp(0))
		{
			yield return 0;
		}
	}

	public static void setLayer(GameObject parent, string name)
	{
		if (!(parent == null))
		{
			int layer = (parent.layer = LayerMask.NameToLayer(name));
			Transform transform = parent.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.layer = layer;
			}
		}
	}

	public static long getUnixTime(DateTime dateTime)
	{
		return (long)(dateTime.ToUniversalTime() - UNIX_EPOCH).TotalSeconds;
	}

	public static void updateUIWidget(GameObject parent)
	{
		UIWidget[] componentsInChildren = parent.GetComponentsInChildren<UIWidget>();
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			componentsInChildren[i].Update();
		}
	}

	public static void createSysLabel(UILabel label, ref UISysFontLabel sysLabel)
	{
		if (label != null)
		{
			Transform transform = label.transform.parent.Find("sysLabel");
			GameObject gameObject = ((!(transform == null)) ? transform.gameObject : new GameObject("sysLabel", typeof(UISysFontLabel)));
			setParent(gameObject, label.transform.parent, true);
			gameObject.transform.localPosition = label.transform.localPosition;
			sysLabel = gameObject.GetComponent<UISysFontLabel>();
			sysLabel.FontSize = (int)(label.transform.localScale.y * 0.87f);
			Color color = label.color;
			sysLabel.color = new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f);
			sysLabel.pivot = label.pivot;
			sysLabel.IsBold = true;
			switch (label.pivot)
			{
			case UIWidget.Pivot.TopLeft:
			case UIWidget.Pivot.Left:
			case UIWidget.Pivot.BottomLeft:
				sysLabel.Alignment = SysFont.Alignment.Left;
				break;
			case UIWidget.Pivot.TopRight:
			case UIWidget.Pivot.Right:
			case UIWidget.Pivot.BottomRight:
				sysLabel.Alignment = SysFont.Alignment.Right;
				break;
			default:
				sysLabel.Alignment = SysFont.Alignment.Center;
				break;
			}
			sysLabel.Text = label.text;
			sysLabel.Update();
			sysLabel.MakePixelPerfect();
			label.gameObject.SetActive(false);
		}
	}

	public static int getStringLine(string str)
	{
		return 1 + (str.Length - str.Replace("\n", string.Empty).Length);
	}

	public static bool decideByProbability(float percentage)
	{
		if (percentage <= 0f)
		{
			return false;
		}
		if (percentage >= 100f)
		{
			return true;
		}
		int num = 0;
		string text = percentage.ToString();
		int num2 = text.IndexOf(".");
		if (num2 > 0)
		{
			num = text.Substring(num2 + 1).Length;
		}
		int num3 = Mathf.FloorToInt(Mathf.Pow(10f, num));
		int max = 100 * num3;
		int num4 = Mathf.FloorToInt(percentage * (float)num3);
		return UnityEngine.Random.Range(0, max) <= num4;
	}

	public static string StripInvalidUnicodeCharacters(string str)
	{
		str.Normalize();
		Regex regex = new Regex("([\ud800-\udbff](?![\udc00-\udfff]))|((?<![\ud800-\udbff])[\udc00-\udfff])");
		return regex.Replace(str, string.Empty);
	}

	public static int EncryptInt(int src)
	{
		return src ^ EncryptString.GetHashCode();
	}

	public static int DecryptInt(int Src)
	{
		return EncryptInt(Src);
	}

	public static int GetOSVersion()
	{
		int num = 0;
		string operatingSystem = SystemInfo.operatingSystem;
		int num2 = operatingSystem.IndexOf("API-");
		return int.Parse(operatingSystem.Substring(num2 + 4, 2).ToString());
	}
}
