using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class HotKnifeMode : Photon.MonoBehaviour
{
	private int KnifeID;

    public static int Enable;

    private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		base.photonView.AddMessage("StartTimer", StartTimer);
		base.photonView.AddMessage("HitPlayer", HitPlayer);
		base.photonView.AddMessage("OnFinishRound", OnFinishRound);
		base.photonView.AddMessage("CheckPlayers", CheckPlayers);
		UIScore.SetActiveScore(true);
		GameManager.startDamageTime = 1f;
		UIPanelManager.ShowPanel("Display");
		WeaponManager.MaxDamage = true;
		GameManager.changeWeapons = false;
		CameraManager.SetType(CameraType.Static);
		TimerManager.In(0.5f, delegate
		{
			GameManager.team = Team.Blue;
			if (PhotonNetwork.isMasterClient)
			{
				ActivationWaitPlayer();
			}
			else
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
		OnCreatePlayer(true);
		HitPlayer(-2);
	}

	private void OnWaitPlayer()
	{
		UIStatus.Add(Localization.Get("Waiting for other players"), true);
		TimerManager.In(4f, delegate
		{
			if (GameManager.roundState == RoundState.WaitPlayer)
			{
				if (PhotonNetwork.playerList.Length <= 1)
				{
					OnWaitPlayer();
				}
				else
				{
					TimerManager.In(4f, delegate
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
		if (PhotonNetwork.playerList.Length <= 1)
		{
			ActivationWaitPlayer();
		}
		else if (PhotonNetwork.isMasterClient)
		{
			GameManager.roundState = RoundState.PlayRound;
			int iD = PhotonNetwork.playerList[UnityEngine.Random.Range(0, PhotonNetwork.playerList.Length)].ID;
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(iD);
			data.Write(true);
			base.photonView.RPC("StartTimer", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void StartTimer(PhotonMessage message)
	{
		int knifeID = message.ReadInt();
		bool spawn = message.ReadBool();
		UIScore.timeData.active = false;
		KnifeID = knifeID;
		float num = 45f;
		num -= (float)(PhotonNetwork.time - message.timestamp);
		UIScore.StartTime(num, StopTimer);
		OnCreatePlayer(spawn);
	}

	private void StopTimer()
	{
		if (PhotonNetwork.player.ID == KnifeID)
		{
			PlayerInput player = GameManager.player;
			DamageInfo damageInfo = DamageInfo.Get(101, Vector3.zero, Team.Blue, player.PlayerWeapon.GetSelectedWeaponData().ID, 0, PhotonNetwork.player.ID, false);
			player.Damage(damageInfo);
		}
	}

	private void OnCreatePlayer(bool spawn)
	{
		if (PhotonNetwork.player.GetTeam() != 0)
		{
			PlayerInput player = GameManager.player;
			if (spawn)
			{
				player.SetHealth(nValue.int100);
				CameraManager.SetType(CameraType.None);
				SpawnPoint playerIDSpawn = SpawnManager.GetPlayerIDSpawn();
				GameManager.controller.ActivePlayer(playerIDSpawn.spawnPosition, playerIDSpawn.spawnRotation);
				HitPlayer(KnifeID);
			}
			else if (!PhotonNetwork.player.GetDead())
			{
				HitPlayer(KnifeID);
			}
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		if (damageInfo.player == PhotonNetwork.player.ID)
		{
			++GameManager.redScore;
			PhotonNetwork.player.SetDeaths1();
			PlayerRoundManager.SetDeaths1();
			UIStatus.Add(damageInfo);
			Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.player.PlayerTransform.position, damageInfo.position);
			CameraManager.SetType(CameraType.Dead, GameManager.player.FPCamera.Transform.position, GameManager.player.FPCamera.Transform.eulerAngles, ragdollForce * 100f);
			GameManager.controller.DeactivePlayer(ragdollForce, damageInfo.headshot);
			base.photonView.RPC("CheckPlayers", PhotonTargets.MasterClient);
			TimerManager.In(3f, delegate
			{
				if (GameManager.player.Dead)
				{
					CameraManager.SetType(CameraType.Spectate);
				}
			});
		}
		else
		{
			UIStatus.Add("[@]: " + UIStatus.GetTeamHexColor(PhotonNetwork.player), false, "HotKnife");
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(PhotonNetwork.player.ID);
			base.photonView.RPC("HitPlayer", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void HitPlayer(PhotonMessage message)
	{
		HitPlayer(message.ReadInt());
	}

	private void HitPlayer(int knife)
	{
		KnifeID = knife;
		if (PhotonNetwork.player.GetTeam() != 0 && !PhotonNetwork.player.GetDead())
		{
			PlayerInput playerInput = GameManager.player;
			playerInput.SetHealth(nValue.int100);
			if (PhotonNetwork.player.ID == knife)
			{
				GameManager.team = Team.Red;
				GameManager.controller.SetTeam(Team.Red);
				WeaponManager.SetSelectWeapon(WeaponType.Knife, 4);
				WeaponManager.SetSelectWeapon(WeaponType.Pistol, 0);
				WeaponManager.SetSelectWeapon(WeaponType.Rifle, 0);
				playerInput.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
				UIHealth.SetHealth(0);
			}
			else if (WeaponManager.HasSelectWeapon(WeaponType.Knife))
			{
				GameManager.team = Team.Blue;
				GameManager.controller.SetTeam(Team.Blue);
				WeaponManager.SetSelectWeapon(WeaponType.Knife, 0);
				WeaponManager.SetSelectWeapon(WeaponType.Pistol, 0);
				WeaponManager.SetSelectWeapon(WeaponType.Rifle, 47);
				TimerManager.In(0.1f, delegate
				{
					playerInput.PlayerWeapon.GetWeaponData(WeaponType.Rifle).AmmoMax = 0;
					playerInput.PlayerWeapon.GetWeaponData(WeaponType.Rifle).Ammo = 0;
					UIAmmo.SetAmmo(0, -1);
					UIHealth.SetHealth(0);
				});
				playerInput.PlayerWeapon.UpdateWeaponAll(WeaponType.Rifle);
			}
		}
		if (PhotonNetwork.isMasterClient && !UIScore.timeData.active && GameManager.roundState == RoundState.PlayRound)
		{
			CheckPlayers();
		}
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessage message)
	{
		int num = message.ReadInt();
		UIScore.timeData.active = false;
		GameManager.roundState = RoundState.EndRound;
		if (PhotonNetwork.player.ID == num)
		{
			PhotonNetwork.player.SetKills1();
			PlayerRoundManager.SetXP(nValue.int12);
			PlayerRoundManager.SetMoney(nValue.int7);
		}
		if (GameManager.checkScore)
		{
			GameManager.LoadNextLevel(GameMode.Classic);
			TimerManager.In(8f, delegate
			{
				PhotonNetwork.LeaveRoom();
			});
		}
		else
		{
			float delay = 8f - (float)(PhotonNetwork.time - message.timestamp);
			TimerManager.In(delay, delegate
			{
				OnStartRound();
			});
		}
	}

	private void SelectKnife()
	{
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		List<int> list = new List<int>();
		for (int i = 0; i < playerList.Length; i++)
		{
			if (!playerList[i].GetDead())
			{
				list.Add(i);
			}
		}
		int iD = playerList[list[UnityEngine.Random.Range(0, list.Count)]].ID;
		PhotonDataWrite data = base.photonView.GetData();
		data.Write(iD);
		data.Write(false);
		base.photonView.RPC("StartTimer", PhotonTargets.All, data);
		UIStatus.Add("[@]: " + UIStatus.GetTeamHexColor(PhotonPlayer.Find(iD)), false, "HotKnife");
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
		int num = -1;
		for (int i = 0; i < playerList.Length; i++)
		{
			if (!playerList[i].GetDead())
			{
				num = ((num != -1) ? (-2) : playerList[i].ID);
			}
		}
		if (num == -1)
		{
			UIMainStatus.Add("[@]", false, 5f, "Draw");
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(-2);
			base.photonView.RPC("OnFinishRound", PhotonTargets.All, data);
			return;
		}
		if (num >= 0)
		{
			++GameManager.blueScore;
			PhotonPlayer photonPlayer = PhotonPlayer.Find(num);
			if (photonPlayer.ID == PhotonNetwork.player.ID)
			{
				++GameManager.blueScore;
			}
			UIMainStatus.Add(photonPlayer.UserId + " [@]", false, 5f, "Win");
			PhotonDataWrite data2 = base.photonView.GetData();
			data2.Write(photonPlayer.ID);
			base.photonView.RPC("OnFinishRound", PhotonTargets.All, data2);
			return;
		}
		bool flag = false;
		for (int j = 0; j < playerList.Length; j++)
		{
			if (playerList[j].ID == KnifeID && !playerList[j].GetDead())
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			SelectKnife();
		}
	}
}
