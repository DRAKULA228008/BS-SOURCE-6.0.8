using System;
using System.Collections.Generic;
using System.Linq;
using Photon;
using UnityEngine;

public class HunterMode : Photon.MonoBehaviour
{
	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
		}
		else if (PhotonNetwork.room.GetGameMode() != GameMode.Hunter)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		base.photonView.AddMessage("OnSendKillerInfo", OnSendKillerInfo);
		base.photonView.AddMessage("UpdateTimer", UpdateTimer);
		base.photonView.AddMessage("OnKilledPlayer", OnKilledPlayer);
		base.photonView.AddMessage("OnFinishRound", OnFinishRound);
		base.photonView.AddMessage("CheckPlayers", CheckPlayers);
		UIScore.SetActiveScore(true, nValue.int20);
		GameManager.startDamageTime = nValue.int1;
		UIPanelManager.ShowPanel("Display");
		GameManager.maxScore = nValue.int20;
		GameManager.changeWeapons = false;
		GameManager.globalChat = false;
		CameraManager.SetType(CameraType.Static);
		TimerManager.In(nValue.float05, delegate
		{
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
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		GameManager.SetScore(playerConnect);
		if (GameManager.roundState != 0)
		{
			CheckPlayers();
			if (UIScore.timeData.active)
			{
				PhotonDataWrite data = base.photonView.GetData();
				data.Write(UIScore.timeData.endTime - Time.time);
				base.photonView.RPC("UpdateTimer", playerConnect, data);
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
		if (PhotonNetwork.playerList.Length <= nValue.int1)
		{
			ActivationWaitPlayer();
		}
		else if (PhotonNetwork.isMasterClient)
		{
			List<PhotonPlayer> list = PhotonNetwork.playerList.ToList();
			int num = OnSelectMaxDeaths(list.Count);
			string text = string.Empty;
			for (int i = 0; i < num; i++)
			{
				int index = UnityEngine.Random.Range(nValue.int0, list.Count);
				text = text + list[index].ID + "#";
				list.RemoveAt(index);
			}
			GameManager.roundState = RoundState.PlayRound;
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(text);
			base.photonView.RPC("OnSendKillerInfo", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void OnSendKillerInfo(PhotonMessage message)
	{
		string[] array = message.ReadString().Split("#"[nValue.int0]);
		bool flag = false;
		for (int i = 0; i < array.Length - nValue.int1; i++)
		{
			if (PhotonNetwork.player.ID == int.Parse(array[i]))
			{
				flag = true;
				break;
			}
		}
		float num = nValue.int120;
		num -= (float)(PhotonNetwork.time - message.timestamp);
		UIScore.StartTime(num, StopTimer);
		if (flag)
		{
			GameManager.team = Team.Red;
		}
		else
		{
			GameManager.team = Team.Blue;
		}
		EventManager.Dispatch("StartRound");
		OnCreatePlayer();
		TimerManager.In(nValue.int3, delegate
		{
			if (PhotonNetwork.isMasterClient)
			{
				CheckPlayers();
			}
		});
	}

	[PunRPC]
	private void UpdateTimer(PhotonMessage message)
	{
		float time = message.ReadFloat();
		double timestamp = message.timestamp;
		TimerManager.In(nValue.float15, delegate
		{
			time -= (float)(PhotonNetwork.time - timestamp);
			UIScore.StartTime(time, StopTimer);
		});
	}

	private void StopTimer()
	{
		if (PhotonNetwork.isMasterClient)
		{
			++GameManager.redScore;
			GameManager.SetScore();
			UIMainStatus.Add("[@]", false, nValue.int5, "Red Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}

	private void OnCreatePlayer()
	{
		PlayerInput player = GameManager.player;
		player.SetHealth(nValue.int100);
		CameraManager.SetType(CameraType.None);
		GameManager.controller.ActivePlayer(SpawnManager.GetTeamSpawn().spawnPosition, SpawnManager.GetTeamSpawn().spawnRotation);
		if (player.PlayerTeam == Team.Blue)
		{
			WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int6);
			WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
			WeaponCustomData weaponCustomData = new WeaponCustomData();
			weaponCustomData.Ammo = nValue.int1;
			weaponCustomData.AmmoMax = nValue.int0;
			player.PlayerWeapon.UpdateWeaponAll(WeaponType.Pistol, null, weaponCustomData, null);
		}
		else
		{
			WeaponManager.SetSelectWeapon(WeaponType.Pistol, AccountManager.GetWeaponSelected(WeaponType.Pistol));
			WeaponManager.SetSelectWeapon(WeaponType.Rifle, AccountManager.GetWeaponSelected(WeaponType.Rifle));
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
		UIScore.timeData.active = false;
		if (GameManager.checkScore)
		{
			GameManager.LoadNextLevel(GameMode.Hunter);
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
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue && !playerList[i].GetDead())
			{
				flag = true;
				break;
			}
		}
		for (int j = 0; j < playerList.Length; j++)
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

	private int OnSelectMaxDeaths(int maxPlayers)
	{
		if (maxPlayers >= nValue.int10)
		{
			return nValue.int4;
		}
		if (maxPlayers >= nValue.int7)
		{
			return nValue.int3;
		}
		if (maxPlayers >= nValue.int4)
		{
			return nValue.int2;
		}
		return nValue.int1;
	}
}
