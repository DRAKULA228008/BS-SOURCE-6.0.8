using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class DropperMode : Photon.MonoBehaviour
{
	[Serializable]
	public struct LevelData
	{
		public int level;

		public List<GameObject> Objects;
	}

	public GameObject[] Droppers;

	public GameObject Place;

	public int SelectLevel = -1;

	public int DestroyLevel = -3;

	public List<LevelData> Levels = new List<LevelData>();

	public int MaxLevel = 50;

	public int max;

	public int seed = 4545;

	private System.Random random;

	private Vector3 lastPos;

	private void Start()
	{
		GameManager.roundState = RoundState.PlayRound;
		UIScore.SetActiveScore(true, nValue.int0);
		GameManager.startDamageTime = nValue.float01;
		UIPanelManager.ShowPanel("Display");
		GameManager.changeWeapons = false;
		UIPlayerStatistics.isOnlyBluePanel = true;
		CameraManager.SetType(CameraType.Static);
		TimerManager.In(nValue.float05, delegate
		{
			GameManager.team = Team.Blue;
			GameManager.player.FPController.MotorAirSpeed = 1f;
			GameManager.player.FPController.MotorBackwardsSpeed = 1f;
			OnRevivalPlayer();
			UIScore.UpdateScore(MaxLevel, GameManager.blueScore, GameManager.redScore);
		});
		EventManager.AddListener<DamageInfo>("DeadPlayer", OnDeadPlayer);
		random = new System.Random(seed);
		for (int i = 0; i < max; i++)
		{
			GenerateLevel(i);
		}
	}

	private void OnSpawnPlayer()
	{
		GameManager.controller.SpawnPlayer(SpawnManager.GetTeamSpawn(Team.Blue).cachedTransform.position, SpawnManager.GetTeamSpawn(Team.Blue).cachedTransform.eulerAngles);
	}

	private void OnRevivalPlayer()
	{
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		PlayerInput player = GameManager.player;
		player.SetHealth(nValue.int100);
		CameraManager.SetType(CameraType.None);
		GameManager.controller.ActivePlayer(SpawnManager.GetTeamSpawn(Team.Blue).cachedTransform.position, SpawnManager.GetTeamSpawn(Team.Blue).cachedTransform.eulerAngles);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
		if (player.PlayerTeam != Team.Blue)
		{
			GameManager.team = Team.Blue;
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		++GameManager.redScore;
		OnSpawnPlayer();
	}

	public void NextLevel(Transform spawn)
	{
		SelectLevel++;
		DestroyLevel++;
		DeactiveLevel();
		SpawnManager.GetTeamSpawn(Team.Blue).cachedTransform.position = spawn.position;
		spawn.gameObject.SetActive(false);
	}

	private void GenerateLevel(int level)
	{
		GameObject gameObject = PoolManager.Spawn("Place", Place, lastPos + new Vector3(0f, -10f, 0f), Vector3.zero);
		gameObject.name = "Place";
		lastPos = GetStartPosition() + gameObject.transform.localPosition;
		LevelData item = default(LevelData);
		item.level = level;
		item.Objects = new List<GameObject>();
		item.Objects.Add(gameObject);
		GameObject gameObject2 = PoolManager.Spawn("Dropper_2", Droppers[2], lastPos, Vector3.zero);
		gameObject2.name = "Dropper_2";
		item.Objects.Add(gameObject2);
		int num = random.Next(3, 5) + level;
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			lastPos += new Vector3(random.Next(-3, 3), random.Next(-40, -30), random.Next(-3, 3));
			num2 = random.Next(0, Droppers.Length - 1);
			gameObject2 = PoolManager.Spawn("Dropper_" + num2, Droppers[num2], lastPos, Vector3.zero);
			gameObject2.name = "Dropper_" + num2;
			item.Objects.Add(gameObject2);
		}
		Levels.Add(item);
	}

	private void DeactiveLevel()
	{
		if (DestroyLevel < 0)
		{
			return;
		}
		for (int i = 0; i < Levels.Count; i++)
		{
			if (Levels[i].level == DestroyLevel)
			{
				for (int j = 0; j < Levels[i].Objects.Count; j++)
				{
					PoolManager.Despawn(Levels[i].Objects[j].name, Levels[i].Objects[j]);
				}
			}
		}
	}

	private Vector3 GetStartPosition()
	{
		Vector3 result = new Vector3(0f, -10f, 0f);
		if (random.Next(0, 100) > 50)
		{
			result.x = random.Next(9, 12) * ((random.Next(0, 100) > 50) ? 1 : (-1));
			result.z = random.Next(0, 12) * ((random.Next(0, 100) > 50) ? 1 : (-1));
		}
		else
		{
			result.x = random.Next(0, 12) * ((random.Next(0, 100) > 50) ? 1 : (-1));
			result.z = random.Next(9, 12) * ((random.Next(0, 100) > 50) ? 1 : (-1));
		}
		return result;
	}
}
