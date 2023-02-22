using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class SpleefMode : Photon.MonoBehaviour
{
	public SpleefBlock[] blocks;

	private List<short> activeBlocks = new List<short>();

	private List<short> deactiveBlocks = new List<short>();

	public static SpleefMode instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		UIScore.SetActiveScore(false);
		GameManager.startDamageTime = nValue.float02;
		UIPanelManager.ShowPanel("Display");
		CameraManager.SetType(CameraType.Static);
		GameManager.changeWeapons = false;
		WeaponManager.SetSelectWeapon(WeaponType.Knife, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int3);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		TimerManager.In(nValue.float05, delegate
		{
			GameManager.team = Team.Blue;
			if (PhotonNetwork.isMasterClient)
			{
				ActivationWaitPlayer();
			}
			else if (GameManager.player.Dead)
			{
				CameraManager.SetType(CameraType.Spectate);
			}
		});
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

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(deactiveBlocks.ToArray());
			base.photonView.RPC("PhotonHideBlock", playerConnect, data);
		}
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer playerDisconnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			CheckPlayers();
		}
	}

	private void ActivationWaitPlayer()
	{
		EventManager.Dispatch("WaitPlayer");
		GameManager.roundState = RoundState.WaitPlayer;
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
			SpawnPoint teamSpawn = SpawnManager.GetTeamSpawn(Team.Blue);
			GameManager.controller.ActivePlayer(teamSpawn.spawnPosition, new Vector3(nValue.int0, UnityEngine.Random.Range(nValue.int0, nValue.int360), nValue.int0));
			player.PlayerWeapon.InfiniteAmmo = true;
			player.PlayerWeapon.UpdateWeaponAll(WeaponType.Pistol);
		}
		activeBlocks.Clear();
		deactiveBlocks.Clear();
		for (short num = 0; num < blocks.Length; num = (short)(num + 1))
		{
			activeBlocks.Add(num);
		}
	}

	private void OnStartRound()
	{
		ResetBlocks();
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

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		if (GameManager.roundState == RoundState.PlayRound)
		{
			PhotonNetwork.player.SetDeaths1();
			PlayerRoundManager.SetDeaths1();
			UIStatus.Add(damageInfo);
			Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.player.PlayerTransform.position, damageInfo.position);
			CameraManager.SetType(CameraType.Dead, GameManager.player.FPCamera.Transform.position, GameManager.player.FPCamera.Transform.eulerAngles, ragdollForce * nValue.int100);
			GameManager.controller.DeactivePlayer(ragdollForce, damageInfo.headshot);
			base.photonView.RPC("CheckPlayers", PhotonTargets.MasterClient);
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
	private void OnWinPlayer(PhotonMessage message)
	{
		PhotonNetwork.player.SetKills1();
		PlayerRoundManager.SetXP(nValue.int12);
		PlayerRoundManager.SetMoney(nValue.int10);
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessage message)
	{
		GameManager.roundState = RoundState.EndRound;
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
		PhotonPlayer photonPlayer = null;
		for (int i = nValue.int0; i < playerList.Length; i++)
		{
			if (!playerList[i].GetDead())
			{
				if (photonPlayer != null)
				{
					return;
				}
				photonPlayer = playerList[i];
			}
		}
		if (photonPlayer != null)
		{
			UIMainStatus.Add(photonPlayer.UserId + " [@]", false, nValue.int5, "Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
			base.photonView.RPC("OnWinPlayer", photonPlayer);
		}
	}

	private void ResetBlocks()
	{
		for (int i = nValue.int0; i < blocks.Length; i++)
		{
			blocks[i].cachedGameObject.SetActive(true);
		}
	}

	public void Damage(int id)
	{
		if (GameManager.roundState == RoundState.PlayRound)
		{
			PhotonDataWrite data = base.photonView.GetData();
			data.Write((short)id);
			base.photonView.RPC("PhotonDamage", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void PhotonDamage(PhotonMessage message)
	{
		short num = message.ReadShort();
		activeBlocks.Remove(num);
		deactiveBlocks.Add(num);
		SpleefBlock spleefBlock = blocks[num];
		spleefBlock.cachedGameObject.SetActive(false);
	}

	[PunRPC]
	private void PhotonHideBlock(PhotonMessage message)
	{
		short[] array = message.ReadShorts();
		for (int i = nValue.int0; i < array.Length; i++)
		{
			activeBlocks.Remove(array[i]);
			deactiveBlocks.Add(array[i]);
			SpleefBlock spleefBlock = blocks[array[i]];
			spleefBlock.cachedGameObject.SetActive(false);
		}
	}
}
