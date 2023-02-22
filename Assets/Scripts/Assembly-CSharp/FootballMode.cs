using System;
using System.Collections.Generic;
using System.Linq;
using Photon;

public class FootballMode : MonoBehaviour
{
	private void Start()
	{
		base.photonView.AddMessage("OnCreatePlayer", OnCreatePlayer);
		base.photonView.AddMessage("AutoGoal", AutoGoal);
		base.photonView.AddMessage("GoalPlayer", GoalPlayer);
		base.photonView.AddMessage("OnFinishRound", OnFinishRound);
		GameManager.maxScore = nValue.int0;
		UIScore.SetActiveScore(true, nValue.int0);
		GameManager.startDamageTime = -nValue.int1;
		UIPanelManager.ShowPanel("Display");
		CameraManager.SetType(CameraType.Static);
		GameManager.player.PlayerWeapon.PushRigidbody = true;
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
		TimerManager.In(nValue.float05, delegate
		{
			PlayerInput player = GameManager.player;
			player.BunnyHopEnabled = true;
			player.BunnyHopSpeed = nValue.float025;
			player.FPController.MotorJumpForce = nValue.float02;
			player.FPController.MotorAirSpeed = nValue.int1;
		});
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
		FootballManager.StartRound();
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

	public void OnCreatePlayer()
	{
		OnCreatePlayer(new PhotonMessage());
	}

	[PunRPC]
	private void OnCreatePlayer(PhotonMessage message)
	{
		if (PhotonNetwork.player.GetTeam() != 0)
		{
			WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
			WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
			PlayerInput player = GameManager.player;
			player.SetHealth(nValue.int100);
			if (message.timestamp != (double)nValue.int0)
			{
				float duration = (float)(PhotonNetwork.time - message.timestamp + (double)nValue.int3);
				player.SetMove(false, duration);
			}
			CameraManager.SetType(CameraType.None);
			int num = UIPlayerStatistics.GetPlayerStatsPosition(PhotonNetwork.player) - nValue.int1;
			if (PhotonNetwork.player.GetTeam() == Team.Red)
			{
				num += nValue.int6;
			}
			GameManager.controller.ActivePlayer(SpawnManager.GetSpawn(num).spawnPosition, SpawnManager.GetSpawn(num).spawnRotation);
			player.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
		}
	}

	public void Goal(PhotonPlayer player, Team gateTeam)
	{
		bool flag = false;
		UIMainStatus.Add(player.UserId + " [@]", false, nValue.int5, "scored a goal");
		if (player.GetTeam() == Team.Red)
		{
			if (gateTeam == Team.Blue)
			{
				++GameManager.redScore;
				GameManager.SetScore();
			}
			else
			{
				++GameManager.blueScore;
				GameManager.SetScore();
				flag = true;
			}
		}
		else if (gateTeam == Team.Red)
		{
			++GameManager.blueScore;
			GameManager.SetScore();
		}
		else
		{
			++GameManager.redScore;
			GameManager.SetScore();
			flag = true;
		}
		base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		if (flag)
		{
			TimerManager.In(nValue.float15, delegate
			{
				UIMainStatus.Add("[@]", false, nValue.int3, "Autogoal");
			});
			base.photonView.RPC("AutoGoal", player);
		}
		else
		{
			base.photonView.RPC("GoalPlayer", player);
		}
	}

	[PunRPC]
	private void GoalPlayer(PhotonMessage message)
	{
		PhotonNetwork.player.SetKills1();
		PlayerRoundManager.SetXP(nValue.int17);
		PlayerRoundManager.SetMoney(nValue.int15);
	}

	[PunRPC]
	private void AutoGoal(PhotonMessage message)
	{
		PhotonNetwork.player.SetDeaths1();
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessage message)
	{
		GameManager.roundState = RoundState.EndRound;
		GameManager.BalanceTeam(true);
		float delay = (float)nValue.int5 - (float)(PhotonNetwork.time - message.timestamp);
		TimerManager.In(delay, delegate
		{
			OnStartRound();
		});
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
		if (!flag || !flag2)
		{
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}

	private void UpdateMasterServer()
	{
		if (PhotonNetwork.isMasterClient && GameManager.roundState == RoundState.PlayRound)
		{
			List<PhotonPlayer> list = PhotonNetwork.playerList.ToList();
			list.Sort(SortByPing);
			if (list[nValue.int0].ID != PhotonNetwork.player.ID)
			{
				PhotonNetwork.SetMasterClient(list[nValue.int0]);
			}
		}
	}

	public static int SortByPing(PhotonPlayer a, PhotonPlayer b)
	{
		return a.GetPing().CompareTo(b.GetPing());
	}
}
