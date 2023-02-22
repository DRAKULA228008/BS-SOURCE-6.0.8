using System;
using UnityEngine;

public class ZombieEscapeManager : MonoBehaviour
{
	[Serializable]
	public class DataClass
	{
		public Transform target;

		public float time;

		public bool active;
	}

	[Serializable]
	public class SpawnClass
	{
		public Vector3 pos;

		public Vector3 rot;

		public float time;

		public bool active;
	}

	public DataClass[] doors;

	public SpawnClass[] spawn;

	public SpawnClass defaultSpawn;

	public Color gizmosColor;

	public Vector3 gizmosSize;

	private Transform zombieSpawn;

	private void Start()
	{
		EventManager.AddListener("StartRound", StartRound);
		EventManager.AddListener("WaitPlayer", StartRound);
		zombieSpawn = SpawnManager.GetTeamSpawn(Team.Red).cachedTransform;
		InvokeRepeating("UpdateData", nValue.float01, nValue.int1);
	}

	private void UpdateData()
	{
		if (!UIScore.timeData.active)
		{
			return;
		}
		for (int i = nValue.int0; i < doors.Length; i++)
		{
			if (!doors[i].active && doors[i].time >= UIScore.timeData.endTime - Time.time)
			{
				doors[i].target.gameObject.SetActive(false);
				doors[i].active = true;
			}
		}
		for (int j = nValue.int0; j < spawn.Length; j++)
		{
			if (!spawn[j].active && spawn[j].time >= UIScore.timeData.endTime - Time.time)
			{
				spawn[j].active = true;
				zombieSpawn.localPosition = spawn[j].pos;
				zombieSpawn.localEulerAngles = spawn[j].rot;
			}
		}
	}

	private void StartRound()
	{
		for (int i = nValue.int0; i < doors.Length; i++)
		{
			doors[i].target.gameObject.SetActive(true);
			doors[i].active = false;
		}
		for (int j = nValue.int0; j < spawn.Length; j++)
		{
			spawn[j].active = false;
		}
		zombieSpawn.localPosition = defaultSpawn.pos;
		zombieSpawn.localEulerAngles = defaultSpawn.rot;
	}
}
