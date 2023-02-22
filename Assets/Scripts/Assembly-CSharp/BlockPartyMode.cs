using System;
using Photon;
using UnityEngine;

public class BlockPartyMode : Photon.MonoBehaviour
{
	public nMonoBehaviour[] blocks;

	public Color32[] colors;

	public int selectColor;

	public MeshFilter line;

	private System.Random rand = new System.Random(5);

	private int[] timers = new int[2];

	private float duration = 5f;

	private void Start()
	{
		base.photonView.AddMessage("OnCreatePlayer", OnCreatePlayer);
		base.photonView.AddMessage("PhotonStartRound", PhotonStartRound);
		base.photonView.AddMessage("OnFinishRound", OnFinishRound);
		base.photonView.AddMessage("CheckPlayers", CheckPlayers);
		base.photonView.AddMessage("OnWinPlayer", OnWinPlayer);
		GameOthers.pauseInterval = 200;
		UIScore.SetActiveScore(false);
		GameManager.startDamageTime = nValue.float02;
		UIPanelManager.ShowPanel("Display");
		CameraManager.SetType(CameraType.Static);
		UIPlayerStatistics.isOnlyBluePanel = true;
		GameManager.changeWeapons = false;
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		CameraManager.main.cameraTransform.GetComponent<Camera>().renderingPath = RenderingPath.Forward;
		TimerManager.In(nValue.float05, delegate
		{
			PlayerInput.instance.PlayerCamera.renderingPath = RenderingPath.Forward;
			PlayerInput.instance.AfkEnabled = true;
			PlayerInput.instance.AfkDuration = 15f;
			PlayerInput.instance.StartAFK();
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
	private void OnCreatePlayer(PhotonMessage message)
	{
		OnCreatePlayer();
	}

	private void OnCreatePlayer()
	{
		if (PhotonNetwork.player.GetTeam() != 0)
		{
			WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
			WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
			PlayerInput player = GameManager.player;
			player.SetHealth(nValue.int100);
			CameraManager.SetType(CameraType.None);
			SpawnPoint teamSpawn = SpawnManager.GetTeamSpawn(Team.Blue);
			GameManager.controller.ActivePlayer(teamSpawn.spawnPosition, new Vector3(nValue.int0, UnityEngine.Random.Range(nValue.int0, nValue.int360), nValue.int0));
			player.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
		}
	}

	private void OnStartRound()
	{
		if (PhotonNetwork.playerList.Length <= nValue.int1)
		{
			ActivationWaitPlayer();
		}
		else if (PhotonNetwork.isMasterClient)
		{
			GameManager.roundState = RoundState.PlayRound;
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(UnityEngine.Random.Range(0, 100000));
			base.photonView.RPC("PhotonStartRound", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void PhotonStartRound(PhotonMessage message)
	{
		int seed = message.ReadInt();
		rand = new System.Random(seed);
		duration = nValue.int5;
		OnCreatePlayer();
		SelectRandomColor();
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
	private void OnWinPlayer(PhotonMessage message)
	{
		OnWinPlayer();
	}

	private void OnWinPlayer()
	{
		PhotonNetwork.player.SetKills1();
		PlayerRoundManager.SetXP(nValue.int12);
		PlayerRoundManager.SetMoney(nValue.int10);
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessage message)
	{
		GameManager.roundState = RoundState.EndRound;
		UIDuration.StopDuration();
		TimerManager.Cancel(timers);
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

	private void SetColor()
	{
		Color32[] array = new Color32[blocks[nValue.int0].cachedMeshFilter.mesh.vertices.Length];
		for (int i = nValue.int0; i < blocks.Length; i++)
		{
			int num = rand.Next(nValue.int0, colors.Length);
			for (int j = nValue.int0; j < array.Length; j++)
			{
				array[j] = colors[num];
			}
			blocks[i].cachedMeshFilter.mesh.colors32 = array;
			blocks[i].cachedGameObject.name = num.ToString();
		}
	}

	private void SetLineColor()
	{
		Color32[] array = new Color32[line.mesh.vertices.Length];
		for (int i = nValue.int0; i < array.Length; i++)
		{
			array[i] = colors[selectColor];
		}
		line.mesh.colors32 = array;
	}

	private void SelectRandomColor()
	{
		ResetBlocks();
		SetColor();
		selectColor = rand.Next(nValue.int0, colors.Length);
		SetLineColor();
		duration -= nValue.float01;
		duration = Mathf.Clamp(duration, 2f, 5f);
		timers[nValue.int0] = TimerManager.In(duration, DeactiveBlocks);
		UIDuration.duration.color = new Color32((byte)(colors[selectColor].r - 40), (byte)(colors[selectColor].g - 40), (byte)(colors[selectColor].b - 40), byte.MaxValue);
		UIDuration.StartDuration(duration);
	}

	private void DeactiveBlocks()
	{
		for (byte b = 0; b < blocks.Length; b = (byte)(b + 1))
		{
			if (blocks[b].cachedGameObject.name != selectColor.ToString())
			{
				blocks[b].cachedGameObject.SetActive(false);
			}
		}
		timers[nValue.int1] = TimerManager.In(nValue.int2, SelectRandomColor);
	}
}
