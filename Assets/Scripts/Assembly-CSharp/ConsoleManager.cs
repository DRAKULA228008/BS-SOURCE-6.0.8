using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ConsoleManager : MonoBehaviour
{
	private static bool autoErrorShow = false;

	public static bool isCreated;

	public static Action<string, string, LogType> LogCallback;

	public static List<string> list = new List<string>();

	private void Start()
	{
		autoErrorShow = GameConsole.Load("auto_error_show", false);
		Application.RegisterLogCallback(OnLogCallback);
	}

	private void OnEnable()
	{
		isCreated = true;
	}

	private void OnDisable()
	{
		isCreated = false;
	}

	public static void Log(string message)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("<color=grey>Log:</color> " + message);
		list.Add(stringBuilder.ToString());
	}

	public static void LogWarning(string message)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("<color=yellow>Warning:</color> " + message);
		list.Add(stringBuilder.ToString());
	}

	public static void LogError(string message)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("<color=red>Error:</color> " + message);
		list.Add(stringBuilder.ToString());
	}

	private void OnLogCallback(string message, string stackTrace, LogType type)
	{
		StringBuilder stringBuilder = new StringBuilder();
		switch (type)
		{
		case LogType.Assert:
			stringBuilder.AppendLine("<color=red>Assert:</color> " + message);
			if (autoErrorShow)
			{
				UIToast.Show(Localization.Get("Error"));
			}
			break;
		case LogType.Exception:
			stringBuilder.AppendLine("<color=red>Exception:</color> " + message);
			if (autoErrorShow)
			{
				UIToast.Show(Localization.Get("Error"));
			}
			break;
		case LogType.Error:
			stringBuilder.AppendLine("<color=red>Error:</color> " + message);
			if (autoErrorShow)
			{
				UIToast.Show(Localization.Get("Error"));
			}
			break;
		case LogType.Warning:
			stringBuilder.AppendLine("<color=yellow>Warning:</color> " + message);
			break;
		case LogType.Log:
			stringBuilder.AppendLine("<color=grey>Log:</color> " + message);
			break;
		}
		stringBuilder.Append("<color=grey>" + stackTrace + "</color>");
		list.Add(stringBuilder.ToString());
		if (LogCallback != null)
		{
			LogCallback(message, stackTrace, type);
		}
	}
}
