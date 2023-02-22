using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class ClassicMode : Photon.MonoBehaviour
{
	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
		}
		else if (PhotonNetwork.room.GetGameMode() != GameMode.Classic)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		base.photonView.AddMessage("OnCreatePlayer", OnCreatePlayer);
		base.photonView.AddMessage("OnKilledPlayer", OnKilledPlayer);
		base.photonView.AddMessage("OnFinishRound", OnFinishRound);
		base.photonView.AddMessage("CheckPlayers", CheckPlayers);
		UIScore.SetActiveScore(true, nValue.int20);
		GameManager.startDamageTime = nValue.int1;
		GameManager.globalChat = false;
		UIPanelManager.ShowPanel("Display");
		CameraManager.SetType(CameraType.Static);
		CameraManager.Team = true;
		CameraManager.ChangeType = true;
		UIChangeTeam.SetChangeTeam(true, true);
		if (PhotonNetwork.isMasterClient)
		{
			TimerManager.In(nValue.float05, delegate
			{
				ActivationWaitPlayer();
			});
		}
		else
		{
			UISelectTeam.OnSpectator();
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
		if (team == Team.None)
		{
			CameraManager.Team = false;
			CameraManager.SetType(CameraType.Spectate);
			UIControllerList.Chat.cachedGameObject.SetActive(false);
			UIControllerList.SelectWeapon.cachedGameObject.SetActive(false);
			UISpectator.SetActive(true);
		}
		else if (GameManager.player.Dead)
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
				if (GetPlayers().Length <= nValue.int1)
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
		UIDeathScreen.ClearAll();
		DecalsManager.ClearBulletHoles();
		if (GetPlayers().Length <= nValue.int1)
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
		if (PhotonNetwork.player.GetTeam() != 0)
		{
			PlayerInput player = GameManager.player;
			player.SetHealth(nValue.int100);
			CameraManager.SetType(CameraType.None);
			GameManager.controller.ActivePlayer(SpawnManager.GetTeamSpawn().spawnPosition, SpawnManager.GetTeamSpawn().spawnRotation);
			player.PlayerWeapon.UpdateWeaponAll(WeaponType.Rifle);
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
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

	[PunRPC]
	private void OnKilledPlayer(PhotonMessage message)
	{
		DamageInfo e = DamageInfo.Serialize(message.ReadBytes());
		EventManager.Dispatch("KillPlayer", e);
		PhotonNetwork.player.SetKills1();
		PlayerRoundManager.SetKills1();
		UIDeathScreen.AddKill(message.sender.ID);
		if (e.headshot)
		{
			PlayerRoundManager.SetXP(nValue.int12);
			PlayerRoundManager.SetMoney(nValue.int10);
			PlayerRoundManager.SetHeadshot1();
		}
		else
		{
			PlayerRoundManager.SetXP(nValue.int6);
			PlayerRoundManager.SetMoney(nValue.int5);
		}
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessage message)
	{
		GameManager.roundState = RoundState.EndRound;
		GameManager.BalanceTeam(true);
		if (GameManager.checkScore)
		{
			GameManager.LoadNextLevel(GameMode.Classic);
			return;
		}
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

	private PhotonPlayer[] GetPlayers()
	{
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() != 0)
			{
				list.Add(playerList[i]);
			}
		}
		return list.ToArray();
	}
}
