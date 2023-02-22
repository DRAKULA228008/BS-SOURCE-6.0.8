using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class PushMode : Photon.MonoBehaviour
{
	public GameObject[] blocks;

	private List<byte> activeBlocks = new List<byte>();

	private List<byte> deactiveBlocks = new List<byte>();

	public Material defaultBlock;

	public Material damageBlock;

	private int mainTimer;

	private int damageTimer;

	private void Start()
	{
		UIScore.SetActiveScore(false);
		GameManager.startDamageTime = nValue.float02;
		GameManager.friendDamage = true;
		UIPanelManager.ShowPanel("Display");
		CameraManager.SetType(CameraType.Static);
		GameManager.changeWeapons = false;
		WeaponManager.SetSelectWeapon(WeaponType.Knife, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int2);
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

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			GameManager.SetScore(playerConnect);
			base.photonView.RPC("PhotonHideBlock", playerConnect, deactiveBlocks.ToArray());
		}
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer playerDisconnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			CheckPlayers();
		}
	}

	[PunRPC]
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
			WeaponCustomData weaponCustomData = new WeaponCustomData();
			weaponCustomData.BodyDamage = nValue.int0;
			weaponCustomData.FaceDamage = nValue.int0;
			weaponCustomData.HandDamage = nValue.int0;
			weaponCustomData.LegDamage = nValue.int0;
			player.PlayerWeapon.UpdateWeaponAll(WeaponType.Pistol, null, weaponCustomData, null);
			player.DamageForce = nValue.int40;
			player.FPController.MotorDoubleJump = true;
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
			base.photonView.RPC("PhotonStartRound", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void PhotonStartRound()
	{
		StartTimer();
		OnCreatePlayer();
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		if (GameManager.roundState == RoundState.PlayRound)
		{
			if (damageInfo.player != -nValue.int1)
			{
				return;
			}
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
	private void OnWinPlayer()
	{
		PhotonNetwork.player.SetKills1();
		PlayerRoundManager.SetXP(nValue.int12);
		PlayerRoundManager.SetMoney(nValue.int10);
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessageInfo info)
	{
		TimerManager.Cancel(mainTimer);
		TimerManager.Cancel(damageTimer);
		GameManager.roundState = RoundState.EndRound;
		float delay = (float)nValue.int8 - (float)(PhotonNetwork.time - info.timestamp);
		TimerManager.In(delay, delegate
		{
			OnStartRound();
		});
	}

	[PunRPC]
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
			blocks[i].SetActive(true);
			blocks[i].GetComponent<Renderer>().material = defaultBlock;
		}
	}

	public void StartTimer()
	{
		activeBlocks.Clear();
		deactiveBlocks.Clear();
		for (byte b = 0; b < blocks.Length; b = (byte)(b + 1))
		{
			activeBlocks.Add(b);
		}
		mainTimer = TimerManager.In(nValue.int3, -nValue.int1, nValue.int3, delegate
		{
			if (PhotonNetwork.isMasterClient && activeBlocks.Count != nValue.int1)
			{
				byte[] array = ((activeBlocks.Count > nValue.int200) ? new byte[nValue.int5] : ((activeBlocks.Count > nValue.int150) ? new byte[nValue.int4] : ((activeBlocks.Count <= nValue.int100) ? new byte[nValue.int2] : new byte[nValue.int3])));
				if (activeBlocks.Count <= nValue.int2)
				{
					array = new byte[nValue.int1];
				}
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = activeBlocks[UnityEngine.Random.Range(nValue.int0, activeBlocks.Count)];
					activeBlocks.Remove(array[i]);
				}
				base.photonView.RPC("PhotonHideBlock", PhotonTargets.All, array);
			}
		});
	}

	[PunRPC]
	private void PhotonHideBlock(byte[] ids)
	{
		if (ids.Length > nValue.int5)
		{
			if (GameManager.roundState == RoundState.PlayRound)
			{
				for (int i = nValue.int0; i < ids.Length; i++)
				{
					blocks[ids[i]].SetActive(false);
				}
			}
			return;
		}
		for (int j = nValue.int0; j < ids.Length; j++)
		{
			byte id = ids[j];
			blocks[id].GetComponent<Renderer>().material = damageBlock;
			activeBlocks.Remove(id);
			deactiveBlocks.Add(id);
			damageTimer = TimerManager.In(nValue.float15, delegate
			{
				if (GameManager.roundState == RoundState.PlayRound)
				{
					blocks[id].SetActive(false);
				}
			});
		}
	}
}
