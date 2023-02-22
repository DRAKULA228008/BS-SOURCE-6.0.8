using System;
using Photon;
using UnityEngine;

public class TennisMode : Photon.MonoBehaviour
{
	public SpawnPoint[] Spawns;

	private void Start()
	{
		base.photonView.AddMessage("OnCreatePlayer", OnCreatePlayer);
		base.photonView.AddMessage("OnKilledPlayer", OnKilledPlayer);
		base.photonView.AddMessage("OnFinishRound", OnFinishRound);
		base.photonView.AddMessage("CheckPlayers", CheckPlayers);
		GameManager.maxScore = 0;
		UIScore.SetActiveScore(true, 0);
		GameManager.startDamageTime = nValue.int1;
		UIPanelManager.ShowPanel("Display");
		CameraManager.SetType(CameraType.Static);
		GameManager.changeWeapons = false;
		if (PhotonNetwork.isMasterClient)
		{
			TimerManager.In(nValue.float05, delegate
			{
				ActivationWaitPlayer();
			});
		}
		else
		{
			UISelectTeam.OnStart(OnSelectTeam);
		}
		EventManager.AddListener<DamageInfo>("DeadPlayer", OnDeadPlayer);
	}

	private void OnEnable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
		PhotonNetwork.onPhotonPlayerDisconnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerDisconnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerDisconnected));
	}

	private void OnDisable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
		PhotonNetwork.onPhotonPlayerDisconnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerDisconnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerDisconnected));
	}

	private void OnSelectTeam(Team team)
	{
		UIPanelManager.ShowPanel("Display");
		if (GameManager.player.Dead)
		{
			CameraManager.SetType(CameraType.Spectate);
		}
	}

	private void ActivationWaitPlayer()
	{
		EventManager.Dispatch("WaitPlayer");
		GameManager.roundState = RoundState.WaitPlayer;
		GameManager.team = Team.Blue;
		OnWaitPlayer();
		OnCreatePlayer();
	}

	private void OnWaitPlayer()
	{
		UIStatus.Add(Localization.Get("Waiting for other players"), true);
		TimerManager.In(nValue.int4, delegate
		{
			if (GameManager.roundState == RoundState.WaitPlayer)
			{
				if (PhotonNetwork.playerList.Length <= nValue.int1)
				{
					OnWaitPlayer();
				}
				else
				{
					TimerManager.In(nValue.int4, delegate
					{
						OnStartRound();
					});
				}
			}
		});
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			GameManager.SetScore(playerConnect);
			if (GameManager.roundState != 0)
			{
				CheckPlayers();
			}
		}
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer playerDisconnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			CheckPlayers();
		}
	}

	private void OnStartRound()
	{
		DecalsManager.ClearBulletHoles();
		if (PhotonNetwork.playerList.Length <= nValue.int1)
		{
			ActivationWaitPlayer();
		}
		else if (PhotonNetwork.isMasterClient)
		{
			GameManager.roundState = RoundState.PlayRound;
			base.photonView.RPC("OnCreatePlayer", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void OnCreatePlayer(PhotonMessage message)
	{
		OnCreatePlayer();
	}

	private void OnCreatePlayer()
	{
		if (PhotonNetwork.player.GetTeam() == Team.None)
		{
			return;
		}
		PlayerInput player = GameManager.player;
		player.SetHealth(nValue.int100);
		CameraManager.SetType(CameraType.None);
		if (PhotonNetwork.room.PlayerCount <= 4)
		{
			if (player.PlayerTeam == Team.Blue)
			{
				GameManager.controller.ActivePlayer(Spawns[0].spawnPosition, Spawns[0].spawnRotation);
			}
			else if (player.PlayerTeam == Team.Red)
			{
				GameManager.controller.ActivePlayer(Spawns[1].spawnPosition, Spawns[1].spawnRotation);
			}
		}
		else if (PhotonNetwork.room.PlayerCount <= 8)
		{
			if (player.PlayerTeam == Team.Blue)
			{
				GameManager.controller.ActivePlayer(Spawns[2].spawnPosition, Spawns[2].spawnRotation);
			}
			else if (player.PlayerTeam == Team.Red)
			{
				GameManager.controller.ActivePlayer(Spawns[3].spawnPosition, Spawns[3].spawnRotation);
			}
		}
		else if (PhotonNetwork.room.PlayerCount <= 12)
		{
			if (player.PlayerTeam == Team.Blue)
			{
				GameManager.controller.ActivePlayer(Spawns[4].spawnPosition, Spawns[4].spawnRotation);
			}
			else if (player.PlayerTeam == Team.Red)
			{
				GameManager.controller.ActivePlayer(Spawns[5].spawnPosition, Spawns[5].spawnRotation);
			}
		}
		WeaponManager.SetSelectWeapon(WeaponType.Knife, 46);
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		if (GameManager.roundState == RoundState.PlayRound)
		{
			PhotonNetwork.player.SetDeaths1();
			PlayerRoundManager.SetDeaths1();
			UIStatus.Add(damageInfo);
			UIDeathScreen.Show(damageInfo);
			Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.player.PlayerTransform.position, damageInfo.position);
			CameraManager.SetType(CameraType.Dead, GameManager.player.FPCamera.Transform.position, GameManager.player.FPCamera.Transform.eulerAngles, ragdollForce * nValue.int100);
			GameManager.controller.DeactivePlayer(ragdollForce, damageInfo.headshot);
			if (damageInfo.otherPlayer)
			{
				PhotonDataWrite data = base.photonView.GetData();
				data.Write(damageInfo.Deserialize());
				base.photonView.RPC("OnKilledPlayer", PhotonPlayer.Find(damageInfo.player), data);
			}
			base.photonView.RPC("CheckPlayers", PhotonTargets.MasterClient);
			GameManager.BalanceTeam(true);
			TimerManager.In(nValue.int3, delegate
			{
				if (GameManager.player.Dead)
				{
					CameraManager.SetType(CameraType.Spectate);
				}
			});
		}
		else
		{
			OnCreatePlayer();
		}
	}

	[PunRPC]
	private void OnKilledPlayer(PhotonMessage message)
	{
		DamageInfo e = DamageInfo.Serialize(message.ReadBytes());
		EventManager.Dispatch("KillPlayer", e);
		PhotonNetwork.player.SetKills1();
		PlayerRoundManager.SetKills1();
		UIDeathScreen.AddKill(message.sender.ID);
		PlayerRoundManager.SetXP(nValue.int7);
		PlayerRoundManager.SetMoney(nValue.int5);
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessage message)
	{
		GameManager.roundState = RoundState.EndRound;
		GameManager.BalanceTeam(true);
		float delay = (float)nValue.int8 - (float)(PhotonNetwork.time - message.timestamp);
		TimerManager.In(delay, delegate
		{
			OnStartRound();
		});
	}

	[PunRPC]
	private void CheckPlayers(PhotonMessage message)
	{
		CheckPlayers();
	}

	private void CheckPlayers()
	{
		if (!PhotonNetwork.isMasterClient || GameManager.roundState == RoundState.EndRound)
		{
			return;
		}
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		bool flag = false;
		bool flag2 = false;
		for (int i = nValue.int0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue && !playerList[i].GetDead())
			{
				flag = true;
				break;
			}
		}
		for (int j = nValue.int0; j < playerList.Length; j++)
		{
			if (playerList[j].GetTeam() == Team.Red && !playerList[j].GetDead())
			{
				flag2 = true;
				break;
			}
		}
		if (!flag)
		{
			++GameManager.redScore;
			GameManager.SetScore();
			UIMainStatus.Add("[@]", false, nValue.int5, "Red Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
		else if (!flag2)
		{
			++GameManager.blueScore;
			GameManager.SetScore();
			UIMainStatus.Add("[@]", false, nValue.int5, "Blue Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}
}
