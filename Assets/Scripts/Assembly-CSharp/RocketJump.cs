using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class RocketJump : Photon.MonoBehaviour
{
	[Serializable]
	public struct LevelData
	{
		public GameObject Map;

		public Transform Spawn;
	}

	public CryptoInt SelectLevel = 1;

	public List<LevelData> Levels;

	private float LastTime;

	private byte pIndex;

	private PhotonView pView;

	private void Start()
	{
		GameManager.roundState = RoundState.PlayRound;
		UIScore.SetActiveScore(true, nValue.int0);
		GameManager.startDamageTime = nValue.float01;
		UIPanelManager.ShowPanel("Display");
		GameManager.changeWeapons = false;
		UIPlayerStatistics.isOnlyBluePanel = true;
		CameraManager.SetType(CameraType.Static);
		TimerManager.In(nValue.float005, delegate
		{
			byte[] array = new byte[nValue.int100];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (byte)(i + 1);
			}
			PhotonNetwork.SetSendingEnabled(null, array);
			pView = GameManager.controller.photonView;
			UpdateGroup();
			InvokeRepeating("UpdateActiveGroup", nValue.int0, nValue.float1 / (float)PhotonNetwork.sendRateOnSerialize);
		});
		TimerManager.In(nValue.float05, delegate
		{
			GameManager.team = Team.Blue;
			GameManager.player.FPController.MotorDoubleJump = true;
			OnRevivalPlayer();
			UIScore.UpdateScore(Levels.Count, GameManager.blueScore, GameManager.redScore);
		});
		EventManager.AddListener<DamageInfo>("DeadPlayer", OnDeadPlayer);
	}

	private void OnSpawnPlayer()
	{
		GameManager.controller.SpawnPlayer(Levels[(int)SelectLevel - nValue.int1].Spawn.position, Levels[(int)SelectLevel - nValue.int1].Spawn.eulerAngles);
	}

	private void OnRevivalPlayer()
	{
		WeaponManager.SetSelectWeapon(WeaponType.Knife, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int2);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		PlayerInput player = GameManager.player;
		player.SetHealth(nValue.int100);
		CameraManager.SetType(CameraType.None);
		GameManager.controller.ActivePlayer(Levels[(int)SelectLevel - nValue.int1].Spawn.position, Levels[(int)SelectLevel - nValue.int1].Spawn.eulerAngles);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Pistol);
		if (player.PlayerTeam != Team.Blue)
		{
			GameManager.team = Team.Blue;
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		++GameManager.redScore;
		UIScore.UpdateScore(Levels.Count, GameManager.blueScore, GameManager.redScore);
		OnSpawnPlayer();
	}

	public void NextLevel()
	{
		if (LastTime + 2f > Time.time)
		{
			PhotonNetwork.LeaveRoom();
		}
		LastTime = Time.time;
		++SelectLevel;
		if (Levels.Count + nValue.int1 <= (int)SelectLevel)
		{
			SelectLevel = nValue.int1;
			UIMainStatus.Add(PhotonNetwork.player.UserId + " [@]", false, nValue.int5, "Finished map");
		}
		UpdateGroup();
		Levels[(int)SelectLevel - nValue.int1].Map.SetActive(true);
		OnSpawnPlayer();
		PhotonNetwork.player.SetKills(SelectLevel);
		GameManager.blueScore = SelectLevel;
		PlayerRoundManager.SetXP(nValue.int3);
		PlayerRoundManager.SetMoney(nValue.int5);
		UIScore.UpdateScore(Levels.Count, GameManager.blueScore, GameManager.redScore);
		if ((int)SelectLevel == nValue.int1)
		{
			Levels[Levels.Count - nValue.int1].Map.SetActive(false);
		}
		else
		{
			Levels[(int)SelectLevel - nValue.int2].Map.SetActive(false);
		}
	}

	private void UpdateGroup()
	{
		if (pView != null)
		{
			pView.group = (byte)(int)SelectLevel;
		}
		if ((int)SelectLevel == nValue.int1)
		{
			PhotonNetwork.SetInterestGroups(new byte[1] { (byte)Levels.Count }, new byte[1] { (byte)(int)SelectLevel });
		}
		else
		{
			PhotonNetwork.SetInterestGroups(new byte[1] { (byte)((int)SelectLevel - nValue.int1) }, new byte[1] { (byte)(int)SelectLevel });
		}
	}

	private void UpdateActiveGroup()
	{
		pIndex++;
		if (pIndex >= 5)
		{
			pIndex = 0;
		}
		switch (pIndex)
		{
		case 0:
			pView.group = 0;
			break;
		case 1:
			pView.group = (byte)(int)SelectLevel;
			break;
		}
	}
}
