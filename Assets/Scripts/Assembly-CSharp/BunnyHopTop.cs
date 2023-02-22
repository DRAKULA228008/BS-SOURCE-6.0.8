using System;
using System.Collections.Generic;
using System.Text;
using FreeJSON;
using UnityEngine;

public class BunnyHopTop : MonoBehaviour
{
	public class PlayerData
	{
		public string name;

		public float time;

		public int deaths;

		public PlayerData(string n, float t, int d)
		{
			name = n;
			time = t;
			deaths = d;
		}
	}

	public TextMesh Text;

	private List<PlayerData> list = new List<PlayerData>();

	private int Deaths;

	private int Timer;

	private static BunnyHopTop instance;

	private void Start()
	{
		instance = this;
		EventManager.AddListener<DamageInfo>("DeadPlayer", OnDeadPlayer);
	}

	public static void StartTimer()
	{
		if (!(instance == null))
		{
			TimerManager.Cancel(instance.Timer);
			instance.Timer = TimerManager.Start();
			instance.Deaths = 0;
		}
	}

	public static void RestartTimer()
	{
		if (!(instance == null))
		{
			BunnyHop.SetDataTopList(TimerManager.GetDuration(instance.Timer), instance.Deaths);
			StartTimer();
		}
	}

	public static void StopTimer()
	{
		if (!(instance == null))
		{
			TimerManager.Cancel(instance.Timer);
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		Deaths++;
	}

	public static void UpdateData(string playerName, float time, int deaths)
	{
		bool flag = false;
		for (int i = 0; i < instance.list.Count; i++)
		{
			if (instance.list[i].name == playerName)
			{
				if (instance.list[i].time > time)
				{
					instance.list[i].time = time;
					instance.list[i].deaths = deaths;
				}
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			instance.list.Add(new PlayerData(playerName, time, deaths));
		}
		instance.list.Sort(SortByTime);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Top List:");
		for (int j = 0; j < instance.list.Count; j++)
		{
			stringBuilder.AppendLine((j + 1).ToString() + ". " + instance.list[j].name + " " + ConvertTime(instance.list[j].time) + " / " + instance.list[j].deaths);
			if (j == 4)
			{
				break;
			}
		}
		instance.Text.text = stringBuilder.ToString();
	}

	public static int SortByTime(PlayerData a, PlayerData b)
	{
		if (a.time == b.time)
		{
			if (a.deaths == b.deaths)
			{
				return a.name.CompareTo(b.name);
			}
			return a.deaths.CompareTo(b.deaths);
		}
		return a.time.CompareTo(b.time);
	}

	private static string ConvertTime(float time)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(time);
		return string.Format("{0:0}:{1:00}:{2:00}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
	}

	public static string GetTopList()
	{
		if (instance == null)
		{
			return string.Empty;
		}
		JsonArray jsonArray = new JsonArray();
		for (int i = 0; i < instance.list.Count; i++)
		{
			JsonObject jsonObject = new JsonObject();
			jsonObject.Add("n", instance.list[i].name);
			jsonObject.Add("t", instance.list[i].time);
			jsonObject.Add("d", instance.list[i].deaths);
			jsonArray.Add(jsonObject);
			if (i == 4)
			{
				break;
			}
		}
		return jsonArray.ToString();
	}

	public static void SetTopList(string data)
	{
		if (instance == null)
		{
			return;
		}
		JsonArray jsonArray = JsonArray.Parse(data);
		for (int i = 0; i < jsonArray.Length; i++)
		{
			JsonObject jsonObject = jsonArray.Get<JsonObject>(i);
			PlayerData item = new PlayerData(jsonObject.Get<string>("n"), jsonObject.Get<float>("t"), jsonObject.Get<int>("n"));
			instance.list.Add(item);
		}
		instance.list.Sort(SortByTime);
		StringBuilder stringBuilder = new StringBuilder();
		if (instance.list.Count == 0)
		{
			return;
		}
		stringBuilder.AppendLine("Top List:");
		for (int j = 0; j < instance.list.Count; j++)
		{
			stringBuilder.AppendLine((j + 1).ToString() + ". " + instance.list[j].name + " " + ConvertTime(instance.list[j].time) + " / " + instance.list[j].deaths);
			if (j == 4)
			{
				break;
			}
		}
		instance.Text.text = stringBuilder.ToString();
	}
}
