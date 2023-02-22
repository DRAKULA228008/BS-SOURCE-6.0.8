using System;
using System.Collections.Generic;
using System.Linq;
using Photon;
using UnityEngine;

public class ZombieMode : Photon.MonoBehaviour
{
	public ZombieBlock[] Blocks;

	private bool isEscape;

	private int StartZombieTimerID;

	private static ZombieMode instance;

	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (PhotonNetwork.room.GetGameMode() != GameMode.ZombieSurvival)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		instance = this;
		if (LevelManager.GetSceneName() == "Escape")
		{
			isEscape = true;
		}
	}

	private void Start()
	{
		base.photonView.AddMessage("StartTimer", StartTimer);
		base.photonView.AddMessage("UpdateTimer", UpdateTimer);
		base.photonView.AddMessage("OnSendKillerInfo", OnSendKillerInfo);
		base.photonView.AddMessage("OnKilledPlayer", OnKilledPlayer);
		base.photonView.AddMessage("OnFinishRound", OnFinishRound);
		base.photonView.AddMessage("CheckPlayers", CheckPlayers);
		base.photonView.AddMessage("PhotonAddDamage", PhotonAddDamage);
		base.photonView.AddMessage("DeactiveBlock", DeactiveBlock);
		base.photonView.AddMessage("DeactiveBlocks", DeactiveBlocks);
		base.photonView.AddMessage("PhotonClickButton", PhotonClickButton);
		UIScore.SetActiveScore(true, nValue.int20);
		GameManager.startDamageTime = nValue.int1;
		UIPanelManager.ShowPanel("Display");
		GameManager.maxScore = nValue.int20;
		CameraManager.SetType(CameraType.Static);
		TimerManager.In(nValue.int1, delegate
		{
			if (PhotonNetwork.isMasterClient)
			{
				ActivationWaitPlayer();
			}
			else if (GameManager.roundState == RoundState.WaitPlayer || GameManager.roundState == RoundState.StartRound)
			{
				OnCreatePlayer();
			}
			else
			{
				OnCreateZombie();
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
					GameManager.roundState = RoundState.StartRound;
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
		TimerManager.In(nValue.float05, delegate
		{
			GameManager.SetScore(playerConnect);
		});
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
		if (Blocks == null || Blocks.Length <= nValue.int0)
		{
			return;
		}
		TimerManager.In(2f, delegate
		{
			List<byte> list = new List<byte>();
			for (int i = nValue.int0; i < Blocks.Length; i++)
			{
				if (!Blocks[i].actived)
				{
					list.Add((byte)Blocks[i].ID);
				}
			}
			PhotonDataWrite data2 = base.photonView.GetData();
			data2.Write(list.ToArray());
			base.photonView.RPC("DeactiveBlocks", playerConnect, data2);
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
			GameManager.roundState = RoundState.StartRound;
			base.photonView.RPC("StartTimer", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void StartTimer(PhotonMessage message)
	{
		EventManager.Dispatch("StartRound");
		OnCreatePlayer();
		UIToast.Show(Localization.Get("Infestation will start in 20 seconds"));
		float num = nValue.int20;
		num -= (float)(PhotonNetwork.time - message.timestamp);
		StartZombieTimerID = TimerManager.In(num, delegate
		{
			if (PhotonNetwork.isMasterClient)
			{
				List<PhotonPlayer> list = PhotonNetwork.playerList.ToList();
				int num2 = OnSelectMaxDeaths(list.Count);
				string text = string.Empty;
				for (int i = 0; i < num2; i++)
				{
					int index = UnityEngine.Random.Range(nValue.int0, list.Count);
					text = text + list[index].ID + "#";
					list.RemoveAt(index);
				}
				PhotonDataWrite data = base.photonView.GetData();
				data.Write(text);
				base.photonView.RPC("OnSendKillerInfo", PhotonTargets.All, data);
			}
			GameManager.roundState = RoundState.PlayRound;
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
			++GameManager.blueScore;
			GameManager.SetScore();
			UIMainStatus.Add("[@]", false, nValue.int5, "Survivors Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void OnSendKillerInfo(PhotonMessage message)
	{
		string[] array = message.ReadString().Split("#"[nValue.int0]);
		bool flag = false;
		for (int i = nValue.int0; i < array.Length - nValue.int1; i++)
		{
			if (PhotonNetwork.player.ID == int.Parse(array[i]))
			{
				flag = true;
				break;
			}
		}
		UIToast.Show(Localization.Get("Infestation started"));
		float num = nValue.int300;
		num -= (float)(PhotonNetwork.time - message.timestamp);
		UIScore.StartTime(num, StopTimer);
		if (flag)
		{
			OnCreateZombie();
		}
		TimerManager.In(nValue.int3, delegate
		{
			if (PhotonNetwork.isMasterClient)
			{
				CheckPlayers();
			}
		});
	}

	private void OnCreatePlayer()
	{
		PlayerInput player = GameManager.player;
		player.Zombie = false;
		player.DamageSpeed = false;
		GameManager.team = Team.Blue;
		if (!isEscape)
		{
			player.DamageForce = nValue.int0;
		}
		player.MaxHealth = nValue.int100;
		player.SetHealth(nValue.int100);
		CameraManager.SetType(CameraType.None);
		GameManager.controller.ActivePlayer(SpawnManager.GetTeamSpawn().spawnPosition, SpawnManager.GetTeamSpawn().spawnRotation);
		player.UpdatePlayerSpeed(nValue.float018);
		player.FPCamera.GetComponent<Camera>().fieldOfView = nValue.int60;
		WeaponManager.SetSelectWeapon(WeaponType.Knife, AccountManager.GetWeaponSelected(WeaponType.Knife));
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, AccountManager.GetWeaponSelected(WeaponType.Pistol));
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, AccountManager.GetWeaponSelected(WeaponType.Rifle));
		WeaponCustomData weaponCustomData = new WeaponCustomData();
		weaponCustomData.CustomData = false;
		weaponCustomData.AmmoMax = (int)WeaponManager.GetSelectWeaponData(WeaponType.Rifle).MaxAmmo * ((!isEscape) ? nValue.int5 : nValue.int10);
		WeaponCustomData weaponCustomData2 = new WeaponCustomData();
		weaponCustomData2.CustomData = false;
		weaponCustomData2.AmmoMax = (int)WeaponManager.GetSelectWeaponData(WeaponType.Pistol).MaxAmmo * ((!isEscape) ? nValue.int5 : nValue.int10);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Rifle, null, weaponCustomData2, weaponCustomData);
	}

	private void OnCreateZombie()
	{
		PlayerInput player = GameManager.player;
		player.Zombie = true;
		player.DamageSpeed = true;
		GameManager.team = Team.Red;
		if (isEscape)
		{
			player.DamageForce = nValue.int15;
		}
		player.MaxHealth = nValue.int1000;
		player.SetHealth(nValue.int1000);
		CameraManager.SetType(CameraType.None);
		GameManager.controller.ActivePlayer(SpawnManager.GetTeamSpawn().spawnPosition, SpawnManager.GetTeamSpawn().spawnRotation);
		player.UpdatePlayerSpeed(nValue.float019);
		player.FPCamera.GetComponent<Camera>().fieldOfView = nValue.int100;
		WeaponManager.SetSelectWeapon(WeaponType.Knife, nValue.int17);
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
		}
		if (GameManager.roundState == RoundState.PlayRound)
		{
			UIStatus.Add(damageInfo);
			if (damageInfo.team == Team.Blue)
			{
				UIDeathScreen.Show(damageInfo);
			}
		}
		if (GameManager.roundState == RoundState.PlayRound)
		{
			Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.player.PlayerTransform.position, damageInfo.position);
			CameraManager.SetType(CameraType.Dead, GameManager.player.FPCamera.Transform.position, GameManager.player.FPCamera.Transform.eulerAngles, ragdollForce * nValue.int100);
			GameManager.controller.DeactivePlayer(ragdollForce, damageInfo.headshot);
		}
		else
		{
			OnCreatePlayer();
		}
		if (damageInfo.otherPlayer)
		{
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(damageInfo.Deserialize());
			base.photonView.RPC("OnKilledPlayer", PhotonPlayer.Find(damageInfo.player), data);
		}
		if (GameManager.roundState != RoundState.PlayRound)
		{
			return;
		}
		base.photonView.RPC("CheckPlayers", PhotonTargets.MasterClient);
		TimerManager.In((!damageInfo.otherPlayer) ? nValue.int1 : nValue.int3, delegate
		{
			if (GameManager.player.Dead)
			{
				OnCreateZombie();
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
			if (e.team == Team.Red)
			{
				PlayerRoundManager.SetXP(nValue.int10);
				PlayerRoundManager.SetMoney(nValue.int5);
			}
			else
			{
				PlayerRoundManager.SetXP(nValue.int15);
				PlayerRoundManager.SetMoney(nValue.int10);
			}
			PlayerRoundManager.SetHeadshot1();
		}
		else if (e.team == Team.Red)
		{
			PlayerRoundManager.SetXP(nValue.int5);
			PlayerRoundManager.SetMoney(nValue.int4);
		}
		else
		{
			PlayerRoundManager.SetXP(nValue.int5);
			PlayerRoundManager.SetMoney(nValue.int8);
		}
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessage message)
	{
		TimerManager.Cancel(StartZombieTimerID);
		UIScore.timeData.active = false;
		GameManager.roundState = RoundState.EndRound;
		if (GameManager.checkScore)
		{
			GameManager.LoadNextLevel(GameMode.ZombieSurvival);
			return;
		}
		float delay = (float)nValue.int6 - (float)(PhotonNetwork.time - message.timestamp);
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
		if (!PhotonNetwork.isMasterClient || GameManager.roundState != RoundState.PlayRound)
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
			if (playerList[j].GetTeam() == Team.Red)
			{
				flag2 = true;
				break;
			}
		}
		if (!flag)
		{
			++GameManager.redScore;
			GameManager.SetScore();
			UIMainStatus.Add("[@]", false, nValue.int5, "Zombie Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
		else if (!flag2)
		{
			++GameManager.blueScore;
			GameManager.SetScore();
			UIMainStatus.Add("[@]", false, nValue.int5, "Survivors Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}

	private int OnSelectMaxDeaths(int maxPlayers)
	{
		if (maxPlayers >= nValue.int8)
		{
			return nValue.int2;
		}
		return nValue.int1;
	}

	public static void AddDamage(byte id)
	{
		PhotonDataWrite data = instance.photonView.GetData();
		data.Write(id);
		instance.photonView.RPC("PhotonAddDamage", PhotonTargets.All, data);
	}

	[PunRPC]
	private void PhotonAddDamage(PhotonMessage message)
	{
		byte b = message.ReadByte();
		for (int i = nValue.int0; i < Blocks.Length; i++)
		{
			if (Blocks[i].ID == b)
			{
				Blocks[i].Attack();
				if (PhotonNetwork.isMasterClient && Blocks[i].CountAttack == nValue.int0)
				{
					PhotonDataWrite data = instance.photonView.GetData();
					data.Write(b);
					base.photonView.RPC("DeactiveBlock", PhotonTargets.All, data);
				}
			}
		}
	}

	[PunRPC]
	private void DeactiveBlock(PhotonMessage message)
	{
		byte b = message.ReadByte();
		for (int i = nValue.int0; i < Blocks.Length; i++)
		{
			if (Blocks[i].ID == b)
			{
				Blocks[i].actived = false;
			}
		}
	}

	[PunRPC]
	private void DeactiveBlocks(PhotonMessage message)
	{
		byte[] array = message.ReadBytes();
		for (int i = nValue.int0; i < Blocks.Length; i++)
		{
			for (int j = nValue.int0; j < array.Length; j++)
			{
				if (Blocks[i].ID == array[j])
				{
					Blocks[i].actived = false;
				}
			}
		}
	}

	public static void ClickButton(byte button)
	{
		PhotonDataWrite data = instance.photonView.GetData();
		data.Write(button);
		instance.photonView.RPC("PhotonClickButton", PhotonTargets.All, data);
	}

	[PunRPC]
	private void PhotonClickButton(PhotonMessage message)
	{
		EventManager.Dispatch("ZombieClickButton", message.ReadByte());
	}
}
