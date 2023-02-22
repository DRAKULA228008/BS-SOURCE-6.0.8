using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class BombMode2 : Photon.MonoBehaviour
{
	public static BombMode2 instance;

	private bool changeTeam;

	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
		}
		else if (PhotonNetwork.room.GetGameMode() != GameMode.Bomb2)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			instance = this;
		}
	}

	private void Start()
	{
		base.photonView.AddMessage("PhotonStartRound", PhotonStartRound);
		base.photonView.AddMessage("UpdateTimer", UpdateTimer);
		base.photonView.AddMessage("OnKilledPlayer", OnKilledPlayer);
		base.photonView.AddMessage("OnFinishRound", OnFinishRound);
		base.photonView.AddMessage("CheckPlayers", CheckPlayers);
		UIScore2.SetActiveScore(true, nValue.int20);
		GameManager.startDamageTime = nValue.int1;
		GameManager.globalChat = false;
		WeaponManager.SetSelectWeapon(WeaponType.Knife, AccountManager.GetWeaponSelected(WeaponType.Knife));
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int3);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		UIPanelManager.ShowPanel("Display");
		CameraManager.SetType(CameraType.Static);
		CameraManager.Team = true;
		CameraManager.ChangeType = true;
		GameManager.changeWeapons = false;
		DropWeaponManager.enable = true;
		UIBuyWeapon.SetActive(true);
		UIBuyWeapon.Money = nValue.int500;
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
			UIBuyWeapon.SetActive(false);
			CameraManager.Team = false;
			CameraManager.SetType(CameraType.Spectate);
			UIControllerList.Chat.cachedGameObject.SetActive(false);
			UIControllerList.SelectWeapon.cachedGameObject.SetActive(false);
			UISpectator.SetActive(true);
		}
		else if (GameManager.player.Dead)
		{
			CameraManager.SetType(CameraType.Spectate);
			UISpectator.SetActive(true);
		}
	}

	private void ActivationWaitPlayer()
	{
		EventManager.Dispatch("WaitPlayer");
		GameManager.roundState = RoundState.WaitPlayer;
		GameManager.team = Team.Red;
		OnWaitPlayer();
		OnCreatePlayer();
		PlayerInput.instance.SetMove(true);
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
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		GameManager.SetScore(playerConnect);
		if (GameManager.roundState == RoundState.WaitPlayer)
		{
			return;
		}
		CheckPlayers();
		if (UIScore2.timeData.active && !BombManager.BombPlaced)
		{
			TimerManager.In(nValue.float05, delegate
			{
				PhotonDataWrite data = base.photonView.GetData();
				data.Write(UIScore2.timeData.endTime - Time.time);
				base.photonView.RPC("UpdateTimer", playerConnect, data);
			});
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
			base.photonView.RPC("PhotonStartRound", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void PhotonStartRound(PhotonMessage message)
	{
		DropWeaponManager.ClearScene();
		EventManager.Dispatch("StartRound");
		float num = nValue.int7;
		num -= (float)(PhotonNetwork.time - message.timestamp);
		UIScore2.StartTime(num, StartTimer);
		OnCreatePlayer(false);
		BombManager.BuyTime = true;
	}

	private void StartTimer()
	{
		if (GameManager.roundState != RoundState.EndRound)
		{
			float time = nValue.int120;
			UIScore2.StartTime(time, StopTimer);
			PlayerInput.instance.SetMove(true);
		}
		BombManager.BuyTime = false;
	}

	private void StopTimer()
	{
		if (PhotonNetwork.isMasterClient)
		{
			++GameManager.blueScore;
			GameManager.SetScore();
			UIMainStatus.Add("[@]", false, nValue.int5, "Blue Win");
			PhotonDataWrite data = base.photonView.GetData();
			data.Write((byte)1);
			data.Write(false);
			base.photonView.RPC("OnFinishRound", PhotonTargets.All, data);
		}
	}

	public void Boom()
	{
		if (PhotonNetwork.isMasterClient && GameManager.roundState == RoundState.PlayRound)
		{
			++GameManager.redScore;
			GameManager.SetScore();
			UIMainStatus.Add("[@]", false, nValue.int5, "Red Win");
			PhotonDataWrite data = base.photonView.GetData();
			data.Write((byte)2);
			data.Write(true);
			base.photonView.RPC("OnFinishRound", PhotonTargets.All, data);
		}
	}

	public void DeactiveBoom()
	{
		if (PhotonNetwork.isMasterClient && GameManager.roundState == RoundState.PlayRound)
		{
			++GameManager.blueScore;
			GameManager.SetScore();
			UIMainStatus.Add("[@]", false, nValue.int5, "Blue Win");
			PhotonDataWrite data = base.photonView.GetData();
			data.Write((byte)1);
			data.Write(true);
			base.photonView.RPC("OnFinishRound", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void UpdateTimer(PhotonMessage message)
	{
		float time = message.ReadFloat();
		double timestamp = message.timestamp;
		TimerManager.In(nValue.float15, delegate
		{
			time -= (float)(PhotonNetwork.time - timestamp);
			UIScore2.StartTime(time, StopTimer);
		});
	}

	private void OnCreatePlayer()
	{
		OnCreatePlayer(true);
	}

	private void OnCreatePlayer(bool move)
	{
		if (PhotonNetwork.player.GetTeam() != 0)
		{
			bool dead = GameManager.player.Dead;
			UISpectator.SetActive(false);
			PlayerInput playerInput = GameManager.player;
			playerInput.SetHealth(nValue.int100);
			CameraManager.SetType(CameraType.None);
			GameManager.controller.ActivePlayer(SpawnManager.GetTeamSpawn().spawnPosition, SpawnManager.GetTeamSpawn().spawnRotation);
			if (dead || changeTeam)
			{
				playerInput.PlayerWeapon.UpdateWeaponAll(WeaponType.Pistol);
			}
			else
			{
				playerInput.PlayerWeapon.UpdateWeaponAmmoAll();
			}
			changeTeam = false;
			TimerManager.In(0.1f, delegate
			{
				playerInput.SetMove(move);
			});
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		PlayerRoundManager.SetDeaths1();
		UIStatus.Add(damageInfo);
		UIDeathScreen.Show(damageInfo);
		PlayerInput player = GameManager.player;
		Vector3 ragdollForce = Utils.GetRagdollForce(player.PlayerTransform.position, damageInfo.position);
		CameraManager.SetType(CameraType.Dead, GameManager.player.FPCamera.Transform.position, player.FPCamera.Transform.eulerAngles, ragdollForce * nValue.int100);
		GameManager.controller.DeactivePlayer(ragdollForce, damageInfo.headshot);
		player.PlayerWeapon.DropWeapon();
		UIDefuseKit.defuseKit = false;
		if (damageInfo.otherPlayer)
		{
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(damageInfo.Deserialize());
			base.photonView.RPC("OnKilledPlayer", PhotonPlayer.Find(damageInfo.player), data);
		}
		base.photonView.RPC("CheckPlayers", PhotonTargets.MasterClient);
		GameManager.BalanceTeam(true);
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int3);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		TimerManager.In(nValue.int3, delegate
		{
			if (GameManager.player.Dead)
			{
				UISpectator.SetActive(true);
				CameraManager.SetType(CameraType.Spectate);
			}
		});
		BombManager.DeadPlayer();
	}

	[PunRPC]
	private void OnKilledPlayer(PhotonMessage message)
	{
		DamageInfo e = DamageInfo.Serialize(message.ReadBytes());
		EventManager.Dispatch("KillPlayer", e);
		PhotonNetwork.player.SetKills1();
		PlayerRoundManager.SetKills1();
		UIDeathScreen.AddKill(message.sender.ID);
		UIBuyWeapon.Money += nValue.int150;
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
		byte b = message.ReadByte();
		bool flag = message.ReadBool();
		UIScore2.timeData.active = false;
		GameManager.roundState = RoundState.EndRound;
		GameManager.BalanceTeam(true);
		if ((Team)b == PhotonNetwork.player.GetTeam())
		{
			UIBuyWeapon.Money += ((!flag) ? 2500 : 2750);
		}
		else
		{
			UIBuyWeapon.Money += ((!flag) ? 1750 : 2000);
		}
		if ((int)GameManager.blueScore + (int)GameManager.redScore == (int)GameManager.maxScore / nValue.int2)
		{
			UIDeathScreen.ClearAll();
			if (PhotonNetwork.isMasterClient)
			{
				int num = GameManager.blueScore;
				GameManager.blueScore = GameManager.redScore;
				GameManager.redScore = num;
				GameManager.SetScore();
			}
			if (PhotonNetwork.player.GetTeam() == Team.Blue)
			{
				GameManager.team = Team.Red;
			}
			else if (PhotonNetwork.player.GetTeam() == Team.Red)
			{
				GameManager.team = Team.Blue;
			}
			UIBuyWeapon.Money = nValue.int500;
			changeTeam = true;
			WeaponManager.SetSelectWeapon(WeaponType.Knife, AccountManager.GetWeaponSelected(WeaponType.Knife));
			WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int3);
			WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
			PlayerInput.instance.PlayerWeapon.UpdateWeaponAll(WeaponType.Pistol);
		}
		else
		{
			GameManager.BalanceTeam(true);
		}
		if (GameManager.checkScore)
		{
			GameManager.LoadNextLevel(GameMode.Bomb);
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
		if (!BombManager.BombPlaced)
		{
			for (int j = nValue.int0; j < playerList.Length; j++)
			{
				if (playerList[j].GetTeam() == Team.Red && !playerList[j].GetDead())
				{
					flag2 = true;
					break;
				}
			}
		}
		else
		{
			flag2 = true;
		}
		if (!flag)
		{
			++GameManager.redScore;
			GameManager.SetScore();
			UIMainStatus.Add("[@]", false, nValue.int5, "Red Win");
			PhotonDataWrite data = base.photonView.GetData();
			data.Write((byte)2);
			data.Write(false);
			base.photonView.RPC("OnFinishRound", PhotonTargets.All, data);
		}
		else if (!flag2)
		{
			++GameManager.blueScore;
			GameManager.SetScore();
			UIMainStatus.Add("[@]", false, nValue.int5, "Blue Win");
			PhotonDataWrite data2 = base.photonView.GetData();
			data2.Write((byte)1);
			data2.Write(false);
			base.photonView.RPC("OnFinishRound", PhotonTargets.All, data2);
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
