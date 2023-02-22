using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class HungerGames : Photon.MonoBehaviour
{
	private List<byte> UsedBox = new List<byte>();

	private static HungerGames instance;

	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
		}
		else if (PhotonNetwork.room.GetGameMode() != GameMode.HungerGames)
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
		base.photonView.AddMessage("MasterPickupBox", MasterPickupBox);
		base.photonView.AddMessage("EventPickupBox", EventPickupBox);
		base.photonView.AddMessage("PhotonHideBoxes", PhotonHideBoxes);
		instance = this;
		UIScore.SetActiveScore(true, nValue.int20);
		GameManager.startDamageTime = nValue.int3;
		GameManager.friendDamage = true;
		UIPanelManager.ShowPanel("Display");
		GameManager.changeWeapons = false;
		GameManager.globalChat = false;
		CameraManager.SetType(CameraType.Static);
		UIPlayerStatistics.isOnlyBluePanel = true;
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
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		GameManager.SetScore(playerConnect);
		if (GameManager.roundState != 0)
		{
			CheckPlayers();
		}
		TimerManager.In(nValue.float15, delegate
		{
			if (UsedBox.Count != 0)
			{
				PhotonDataWrite data = base.photonView.GetData();
				data.Write(UsedBox.ToArray());
				base.photonView.RPC("PhotonHideBoxes", playerConnect, data);
			}
		});
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
		EventManager.Dispatch("StartRound");
		UsedBox.Clear();
		if (PhotonNetwork.player.GetTeam() != 0)
		{
			PlayerInput player = GameManager.player;
			player.SetHealth(nValue.int100);
			CameraManager.SetType(CameraType.None);
			player.FPCamera.GetComponent<Camera>().farClipPlane = nValue.int300;
			SpawnPoint playerIDSpawn = SpawnManager.GetPlayerIDSpawn();
			GameManager.controller.ActivePlayer(playerIDSpawn.spawnPosition, playerIDSpawn.spawnRotation);
			WeaponManager.SetSelectWeapon(WeaponType.Knife, nValue.int4);
			WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
			WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
			player.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
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
		if (GameManager.checkScore)
		{
			GameManager.LoadNextLevel(GameMode.HungerGames);
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
		int num = -nValue.int1;
		for (int i = 0; i < playerList.Length; i++)
		{
			if (!playerList[i].GetDead())
			{
				if (num != -nValue.int1)
				{
					flag = false;
					break;
				}
				num = playerList[i].ID;
				flag = true;
			}
		}
		if (flag)
		{
			++GameManager.blueScore;
			GameManager.SetScore();
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
			UIMainStatus.Add(PhotonPlayer.Find(num).UserId + " [@]", false, nValue.int5, "Win");
		}
	}

	public static void PickupBox(int id)
	{
		PhotonDataWrite data = instance.photonView.GetData();
		data.Write((byte)id);
		instance.photonView.RPC("MasterPickupBox", PhotonTargets.MasterClient, data);
	}

	[PunRPC]
	private void MasterPickupBox(PhotonMessage message)
	{
		byte b = message.ReadByte();
		if (!UsedBox.Contains(b))
		{
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(b);
			data.Write(message.sender.ID);
			base.photonView.RPC("EventPickupBox", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void EventPickupBox(PhotonMessage message)
	{
		byte b = message.ReadByte();
		int e = message.ReadInt();
		UsedBox.Add(b);
		EventManager.Dispatch("EventPickupBox", (int)b, e);
	}

	public static void HideBoxes(byte[] idBoxes)
	{
		PhotonDataWrite data = instance.photonView.GetData();
		data.Write(idBoxes);
		instance.photonView.RPC("PhotonHideBoxes", PhotonTargets.All, data);
	}

	[PunRPC]
	private void PhotonHideBoxes(PhotonMessage message)
	{
		byte[] ids = message.ReadBytes();
		TimerManager.In(nValue.float01, delegate
		{
			for (int i = nValue.int0; i < ids.Length; i++)
			{
				UsedBox.Add(ids[i]);
				EventManager.Dispatch("EventPickupBox", (int)ids[i], -nValue.int1);
			}
		});
	}

	public static void SetWeapon(int weaponID)
	{
		WeaponType type = WeaponManager.GetWeaponData(weaponID).Type;
		PlayerInput player = GameManager.player;
		WeaponManager.SetSelectWeapon(weaponID);
		WeaponCustomData weaponCustomData = new WeaponCustomData();
		weaponCustomData.AmmoMax = nValue.int0;
		player.PlayerWeapon.UpdateWeapon(type, true, weaponCustomData);
	}
}
