using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;

public class SpeedHackDetector2 : MonoBehaviour
{
	public static Action OnDetected;

	public byte interval = 1;

	public byte maxFalsePositives = 3;

	private int threshold = 3;

	private byte currentFalsePositives;

	private double timeOnStart = -1.0;

	private void Start()
	{
		Check();
	}

	private void Check()
	{
		Loom.RunAsync(delegate
		{
			while (true)
			{
				string[] array = File.ReadAllLines("proc/driver/rtc");
				array[0] = array[0].Replace(" ", string.Empty);
				array[0] = array[0].Replace("\t", string.Empty);
				array[1] = array[1].Replace("\t", string.Empty);
				array[1] = array[1].Replace(" ", string.Empty);
				array[1] = array[1].Replace(":", string.Empty);
				array[1] = array[1].Replace("rtc_date", string.Empty);
				string[] array2 = array[1].Split("-"[0]);
				string[] array3 = array[0].Split(":"[0]);
				DateTime dateTime = new DateTime(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array3[1]), int.Parse(array3[2]), int.Parse(array3[3]));
				if (timeOnStart <= -1.0)
				{
					timeOnStart = Math.Abs((dateTime - DateTime.UtcNow).TotalSeconds);
					currentFalsePositives = 0;
				}
				else if (Math.Abs(Math.Abs((dateTime - DateTime.UtcNow).TotalSeconds) - timeOnStart) > (double)threshold)
				{
					currentFalsePositives++;
					if (currentFalsePositives >= maxFalsePositives)
					{
						timeOnStart = -1.0;
						if (OnDetected != null)
						{
							OnDetected();
						}
					}
				}
				else if (currentFalsePositives > 0)
				{
					currentFalsePositives--;
				}
				Thread.Sleep(interval * 1000);
			}
		});
	}

	public static bool CheckStartTime(float difference)
	{
		return (DateTime.Now - Process.GetCurrentProcess().StartTime).TotalSeconds > (double)difference;
	}
}
