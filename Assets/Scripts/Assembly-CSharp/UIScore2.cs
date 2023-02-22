using System;
using UnityEngine;

public class UIScore2 : MonoBehaviour
{
	public class TimeData
	{
		public bool active;

		public bool show;

		public float endTime;

		public Action callback;
	}

	public GameObject score;

	public UILabel maxScoreLabel;

	public UILabel blueScoreLabel;

	public UILabel redScoreLabel;

	public UILabel bluePlayersLabel;

	public UILabel redPlayersLabel;

	public static TimeData timeData = new TimeData();

	private static UIScore2 instance;

	private void Awake()
	{
		instance = this;
	}

	private void OnDisable()
	{
		ControllerManager.SetDeadEvent -= OnUpdatePlayers;
		timeData = new TimeData();
	}

	private void OnUpdatePlayers()
	{
		OnUpdatePlayers(0, false);
	}

	private void OnUpdatePlayers(DamageInfo damageInfo)
	{
		OnUpdatePlayers(0, false);
	}

	private void OnUpdatePlayers(int player, bool dead)
	{
		int num = 0;
		int num2 = 0;
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue && !playerList[i].GetDead())
			{
				num2++;
			}
			else if (playerList[i].GetTeam() == Team.Red && !playerList[i].GetDead())
			{
				num++;
			}
		}
		bluePlayersLabel.text = StringCache.Get(num2);
		redPlayersLabel.text = StringCache.Get(num);
	}

	public static void SetActiveScore(bool active)
	{
		EventManager.AddListener<DamageInfo>("DeadPlayer", instance.OnUpdatePlayers);
		ControllerManager.SetDeadEvent += instance.OnUpdatePlayers;
		instance.score.SetActive(active);
		instance.OnUpdatePlayers();
	}

	public static void SetActiveScore(bool active, int maxScore)
	{
		EventManager.AddListener<DamageInfo>("DeadPlayer", instance.OnUpdatePlayers);
		ControllerManager.SetDeadEvent += instance.OnUpdatePlayers;
		instance.score.SetActive(active);
		if (maxScore <= nValue.int0)
		{
			instance.maxScoreLabel.text = "-";
		}
		else
		{
			instance.maxScoreLabel.text = StringCache.Get(maxScore);
		}
		instance.OnUpdatePlayers();
	}

	public static void UpdateScore()
	{
		if ((int)GameManager.maxScore <= nValue.int0)
		{
			instance.maxScoreLabel.text = "-";
		}
		else
		{
			instance.maxScoreLabel.text = StringCache.Get(GameManager.maxScore);
		}
		instance.blueScoreLabel.text = StringCache.Get(GameManager.blueScore);
		instance.redScoreLabel.text = StringCache.Get(GameManager.redScore);
		instance.OnUpdatePlayers();
	}

	public static void UpdateScore(int maxScore, int blueScore, int redScore)
	{
		if (maxScore <= nValue.int0)
		{
			instance.maxScoreLabel.text = "-";
		}
		else
		{
			instance.maxScoreLabel.text = StringCache.Get(maxScore);
		}
		instance.blueScoreLabel.text = StringCache.Get(blueScore);
		instance.redScoreLabel.text = StringCache.Get(redScore);
		instance.OnUpdatePlayers();
	}

	public static void StartTime(float time, Action callback)
	{
		StartTime(time, true, callback);
	}

	public static void StartTime(float time, bool show, Action callback)
	{
		timeData.active = true;
		TimerManager.In("UIScoreTimer", 0.25f, -1, 0.25f, instance.UpdateTimer);
		timeData.show = show;
		timeData.endTime = time + Time.time;
		timeData.callback = callback;
	}

	public static void StopTime(bool callback)
	{
		TimerManager.Cancel("UIScoreTimer");
		timeData.active = false;
		if (callback && timeData.callback != null)
		{
			timeData.callback();
		}
	}

	private void UpdateTimer()
	{
		if (timeData.active)
		{
			if (timeData.show)
			{
				maxScoreLabel.text = StringCache.GetTime(timeData.endTime - Time.time);
			}
			if (timeData.endTime <= Time.time)
			{
				TimerManager.Cancel("UIScoreTimer");
				timeData.active = false;
				if (timeData.callback != null)
				{
					timeData.callback();
				}
			}
		}
		else
		{
			TimerManager.Cancel("UIScoreTimer");
		}
	}
}
