using System;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

public class UIFramesPerSecond : MonoBehaviour
{
	public const int MEMORY_DIVIDER = 1048576;

	public UILabel label;

	private bool activated;

	private float accum;

	private float frames;

	private StringBuilder builder;

	private static bool allStats;

	private void Start()
	{
		EventManager.AddListener("OnSettings", OnSettings);
		OnSettings();
		allStats = GameConsole.Load("show_all_stats", false);
	}

	private void OnDisable()
	{
		TimerManager.Cancel("FPSMeter");
	}

	private void Update()
	{
		if (activated)
		{
			accum += Time.timeScale / Time.deltaTime;
			frames += 1f;
		}
	}

	private void OnSettings()
	{
		activated = Settings.FPSMeter;
		if (activated)
		{
			if (TimerManager.IsActive("FPSMeter"))
			{
				TimerManager.Cancel("FPSMeter");
			}
			TimerManager.In("FPSMeter", 1f, -1, 1f, UpdateLabel);
		}
		else
		{
			TimerManager.Cancel("FPSMeter");
		}
		label.gameObject.SetActive(activated);
	}

	private void UpdateLabel()
	{
		int number = Mathf.RoundToInt(accum / frames);
		accum = 0f;
		frames = 0f;
		if (allStats)
		{
			if (builder == null)
			{
				builder = new StringBuilder();
			}
			builder.Length = 0;
			builder.Capacity = 0;
			builder.Append("FPS: ").Append(StringCache.Get(number)).Append(" | ");
			builder.Append("PING: ").Append(StringCache.Get(PhotonNetwork.GetPing())).Append(" | ");
			builder.Append("MEM TOTAL: ").Append(StringCache.Get(Profiler.GetTotalReservedMemory() / 1048576u)).Append(" | ");
			builder.Append("MEM ALLOC: ").Append(StringCache.Get(Profiler.GetTotalAllocatedMemory() / 1048576u)).Append(" | ");
			builder.Append("MEM MONO: ").Append(StringCache.Get((int)(GC.GetTotalMemory(false) / 1048576))).Append(" | ");
			builder.Append("MEM UNSED: ").Append(StringCache.Get(UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemory() / 1048576u));
			label.text = builder.ToString();
		}
		else
		{
			label.text = "FPS: " + StringCache.Get(number);
		}
	}
}
