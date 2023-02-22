using System;
using System.Collections;
using UnityEngine;

public class NTPManager : MonoBehaviour
{
	private string Url = "http://chronic.herokuapp.com/utc/now";

	private Action<DateTime> Callback;

	public static int LastGetSeconds;

	private static NTPManager instance;

	private void Awake()
	{
		instance = this;
	}

	public static void GetTime(Action<DateTime> callback)
	{
		instance.Callback = callback;
		instance.StartCoroutine(instance.GetNTPTime());
	}

	public static int GetSeconds(DateTime time)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		return (int)(time.ToUniversalTime() - dateTime).TotalSeconds;
	}

	public static long GetMilliSeconds(DateTime time)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		return (long)(time.ToUniversalTime() - dateTime).TotalMilliseconds;
	}

	public static int GetHours(DateTime time)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		return (int)(time.ToUniversalTime() - dateTime).TotalHours;
	}

	private IEnumerator GetNTPTime()
	{
		WWW www = new WWW(Url);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			string[] data = www.text.Split(" "[0]);
			string[] date = data[0].Split("-"[0]);
			string[] time = data[1].Split(":"[0]);
			int year = int.Parse(date[0]);
			int month = int.Parse(date[1]);
			int day = int.Parse(date[2]);
			int hour = int.Parse(time[0]);
			int minute = int.Parse(time[1]);
			int second = int.Parse(time[2]);
			DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
			LastGetSeconds = GetSeconds(dateTime);
			if (Callback != null)
			{
				Callback(dateTime);
			}
		}
		else
		{
			Callback(new DateTime(1970L));
		}
	}
}
