using System.Collections.Generic;
using UnityEngine;

public class GameLog : MonoBehaviour
{
	private bool is30FpsTargetFrameRate = true;

	public bool EnableShowMenuButton = true;

	private bool showMenuButtons;

	private int menuButtonBasePosX = Screen.width - 100;

	private int menuButtonBasePosY = 15;

	private int menuButtonButtonWidth = 85;

	private int menuButtonHeight = 50;

	private int menuButtonSpace = 60;

	private int logLineLimit = 40;

	public bool EnableUnityLogCallback = true;

	public GUIStyle LogShadowFont;

	public bool EnableDebugLog = true;

	private bool showDebugLog;

	private string debugLog = "[Debug Log]\n";

	public GUIStyle DebugLogFont;

	public bool EnableWarningLog = true;

	private bool showWarningLog;

	private string warningLog = "[Warning Log]\n";

	public GUIStyle WarningLogFont;

	public bool EnableErrorLog = true;

	private bool showErrorLog;

	private string errorLog = "[Error Log]\n";

	public GUIStyle ErrorLogFont;

	public bool EnableExceptionLog = true;

	private bool showExceptionLog;

	private string exceptionLog = "[Exception Log]\n";

	public GUIStyle ExceptionLogFont;

	private static GameLog staticInstance;

	public static GameLog Instance
	{
		get
		{
			return staticInstance;
		}
	}

	private void Awake()
	{
		base.gameObject.SetActive(false);
		RegisterLogCallback();
		UpdateTargetFrameRate();
		if (staticInstance == null)
		{
			staticInstance = this;
			Object.DontDestroyOnLoad(this);
		}
		else
		{
			Debug.Log("GameLog is Already Exist, the New one will be Destroyed.");
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		UpdateTargetFrameRate();
		UpdateLogLineLimit();
	}

	private void Update()
	{
		if (EnableUnityLogCallback)
		{
			UpdateLogLineLimit();
		}
	}

	private void RegisterLogCallback()
	{
		if (EnableUnityLogCallback)
		{
			Application.RegisterLogCallback(OnLogCallback);
			Application.RegisterLogCallbackThreaded(OnLogCallback);
		}
	}

	private void UpdateLogLineLimit()
	{
		logLineLimit = (Screen.height - 100) / 20 + 5;
	}

	private void UpdateTargetFrameRate()
	{
		if (is30FpsTargetFrameRate)
		{
			Application.targetFrameRate = 30;
			QualitySettings.vSyncCount = 2;
		}
		else
		{
			Application.targetFrameRate = 60;
			QualitySettings.vSyncCount = 1;
		}
	}

	private void OnGUI()
	{
		DisplayBasicInfo();
		DisplayDebugLog();
		DisplayWarningLog();
		DisplayErrorLog();
		DisplayExceptionLog();
		if (EnableShowMenuButton)
		{
			DisplayMenuButtons();
		}
	}

	private void DisplayBasicInfo()
	{
		string empty = string.Empty;
		int num = 0;
		if (Time.deltaTime > 0f)
		{
			num = (int)(1f / Time.deltaTime);
		}
		empty += num;
		empty += " FPS ( ";
		empty += Screen.width;
		empty += " x ";
		empty += Screen.height;
		empty += " ), Execute Time: ";
		empty += string.Format("{0:F2}", Time.realtimeSinceStartup);
		empty += " sec.";
		GUI.Label(new Rect(12f, 5f, Screen.width - 8, Screen.height - 10), empty, LogShadowFont);
		GUI.Label(new Rect(10f, 5f, Screen.width - 10, Screen.height - 10), empty, DebugLogFont);
	}

	private void DisplayDebugLog()
	{
		if (EnableDebugLog)
		{
			List<int> list = new List<int>();
			for (int num = debugLog.IndexOf('\n', 0); num != -1; num = debugLog.IndexOf('\n', num + 1))
			{
				list.Add(num);
			}
			if (list.Count > logLineLimit)
			{
				int num2 = list[list.Count - logLineLimit];
				debugLog = "\n[Debug Log]\n" + debugLog.Remove(0, num2 + 1);
			}
			if (showDebugLog)
			{
				GUI.Label(new Rect(12f, 25f, Screen.width - 8, Screen.height - 10), debugLog, LogShadowFont);
				GUI.Label(new Rect(10f, 25f, Screen.width - 10, Screen.height - 10), debugLog, DebugLogFont);
			}
		}
	}

	private void DisplayWarningLog()
	{
		if (EnableWarningLog)
		{
			List<int> list = new List<int>();
			for (int num = warningLog.IndexOf('\n', 0); num != -1; num = warningLog.IndexOf('\n', num + 1))
			{
				list.Add(num);
			}
			if (list.Count > logLineLimit)
			{
				int num2 = list[list.Count - logLineLimit];
				warningLog = "\n[Warning Log]\n" + warningLog.Remove(0, num2 + 1);
			}
			if (showWarningLog)
			{
				GUI.Label(new Rect(12f, 25f, Screen.width - 8, Screen.height - 10), warningLog, LogShadowFont);
				GUI.Label(new Rect(10f, 25f, Screen.width - 10, Screen.height - 10), warningLog, WarningLogFont);
			}
		}
	}

	private void DisplayErrorLog()
	{
		if (EnableErrorLog)
		{
			List<int> list = new List<int>();
			for (int num = errorLog.IndexOf('\n', 0); num != -1; num = errorLog.IndexOf('\n', num + 1))
			{
				list.Add(num);
			}
			if (list.Count > logLineLimit)
			{
				int num2 = list[list.Count - logLineLimit];
				errorLog = "\n[Error Log]\n" + errorLog.Remove(0, num2 + 1);
			}
			if (showErrorLog)
			{
				GUI.Label(new Rect(12f, 25f, Screen.width - 8, Screen.height - 10), errorLog, LogShadowFont);
				GUI.Label(new Rect(10f, 25f, Screen.width - 10, Screen.height - 10), errorLog, ErrorLogFont);
			}
		}
	}

	private void DisplayExceptionLog()
	{
		if (EnableExceptionLog)
		{
			List<int> list = new List<int>();
			for (int num = exceptionLog.IndexOf('\n', 0); num != -1; num = exceptionLog.IndexOf('\n', num + 1))
			{
				list.Add(num);
			}
			if (list.Count > logLineLimit)
			{
				int num2 = list[list.Count - logLineLimit];
				exceptionLog = "\n[Exception Log]\n" + exceptionLog.Remove(0, num2 + 1);
			}
			if (showExceptionLog)
			{
				GUI.Label(new Rect(12f, 25f, Screen.width - 8, Screen.height - 10), exceptionLog, LogShadowFont);
				GUI.Label(new Rect(10f, 25f, Screen.width - 10, Screen.height - 10), exceptionLog, ExceptionLogFont);
			}
		}
	}

	private Rect GetNextButtonRect(ref Rect buttonRect, ref int buttonPosition)
	{
		buttonRect.y = menuButtonBasePosY + menuButtonSpace * buttonPosition++;
		if (buttonRect.y + (float)menuButtonHeight > (float)Screen.height)
		{
			buttonPosition = 0;
			buttonRect.x -= (float)menuButtonButtonWidth + 10f;
			buttonRect.y = menuButtonBasePosY + menuButtonSpace * buttonPosition++;
		}
		return buttonRect;
	}

	private void DisplayMenuButtons()
	{
		int buttonPosition = 1;
		Rect buttonRect = new Rect(menuButtonBasePosX, menuButtonBasePosY, menuButtonButtonWidth, menuButtonHeight);
		if (GUI.Button(buttonRect, (!showMenuButtons) ? "Show Menu" : "Hide Menu"))
		{
			showMenuButtons = !showMenuButtons;
		}
		if (!showMenuButtons)
		{
			return;
		}
		if (GUI.Button(GetNextButtonRect(ref buttonRect, ref buttonPosition), (!showDebugLog) ? "Show Log\nDebug" : "Hide Log\nDebug"))
		{
			showDebugLog = !showDebugLog;
		}
		if (GUI.Button(GetNextButtonRect(ref buttonRect, ref buttonPosition), (!showWarningLog) ? "Show Log\nWarning" : "Hide Log\nWarning"))
		{
			showWarningLog = !showWarningLog;
		}
		if (GUI.Button(GetNextButtonRect(ref buttonRect, ref buttonPosition), (!showErrorLog) ? "Show Log\nError" : "Hide Log\nError"))
		{
			showErrorLog = !showErrorLog;
		}
		if (GUI.Button(GetNextButtonRect(ref buttonRect, ref buttonPosition), (!showExceptionLog) ? "Show Log\nException" : "Hide Log\nException"))
		{
			showExceptionLog = !showExceptionLog;
		}
		if (GUI.Button(GetNextButtonRect(ref buttonRect, ref buttonPosition), (!is30FpsTargetFrameRate) ? "30 FPS\nMode" : "60 FPS\nMode"))
		{
			is30FpsTargetFrameRate = !is30FpsTargetFrameRate;
			UpdateTargetFrameRate();
		}
		if (GUI.Button(GetNextButtonRect(ref buttonRect, ref buttonPosition), "Pause"))
		{
			if (Time.timeScale != 0f)
			{
				Time.timeScale = 0f;
			}
			else
			{
				Time.timeScale = 1f;
			}
		}
	}

	private void OnLogCallback(string logString, string stackTrace, LogType type)
	{
		switch (type)
		{
		case LogType.Log:
			if (EnableDebugLog)
			{
				debugLog += logString;
				debugLog += "\n";
			}
			break;
		case LogType.Warning:
			if (EnableWarningLog)
			{
				warningLog += logString;
				warningLog += "\n";
			}
			break;
		case LogType.Error:
			if (EnableErrorLog)
			{
				errorLog += logString;
				errorLog += "\n";
				errorLog += stackTrace;
				errorLog += "\n\n";
			}
			break;
		case LogType.Exception:
			if (EnableExceptionLog)
			{
				exceptionLog += logString;
				exceptionLog += "\n";
				exceptionLog += stackTrace;
				exceptionLog += "\n\n";
			}
			break;
		case LogType.Assert:
			break;
		}
	}
}
