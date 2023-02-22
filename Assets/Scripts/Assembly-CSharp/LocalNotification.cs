using System;
using UnityEngine;

internal class LocalNotification
{
	public enum NotificationExecuteMode
	{
		Inexact,
		Exact,
		ExactAndAllowWhileIdle
	}

	private static string fullClassName = "net.agasper.unitynotification.UnityNotificationManager";

	private static string mainActivityClassName = "com.prime31.UnityPlayerNativeActivity";

	public static void SendNotification(int id, TimeSpan delay, string title, string message)
	{
		SendNotification(id, (int)delay.TotalSeconds, title, message, Color.white, true, true, true, string.Empty);
	}

	public static void SendNotification(int id, long delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "", NotificationExecuteMode executeMode = NotificationExecuteMode.Inexact)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(fullClassName);
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("SetNotification", id, delay * 1000, title, message, message, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_icon_small", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, (int)executeMode, mainActivityClassName);
		}
	}

	public static void SendRepeatingNotification(int id, long delay, long timeout, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(fullClassName);
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("SetRepeatingNotification", id, delay * 1000, title, message, message, timeout * 1000, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_icon_small", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, mainActivityClassName);
		}
	}

	public static void CancelNotification(int id)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(fullClassName);
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("CancelNotification", id);
		}
	}
}
