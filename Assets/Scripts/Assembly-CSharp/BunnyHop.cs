using System;
using Photon;
using UnityEngine;

public class BunnyHop : Photon.MonoBehaviour
{
	private Vector3 StartSpawnPosition;

	private Quaternion StartSpawnRotation;

	private static BunnyHop instance;

    public int time = 0;

	private void Awake()
	{
		if (PhotonNetwork.room.GetGameMode() != GameMode.BunnyHop)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		instance = this;
		base.photonView.AddMessage("StartTimer", StartTimer);
		base.photonView.AddMessage("UpdateTimer", UpdateTimer);
		base.photonView.AddMessage("OnFinishRound", OnFinishRound);
		base.photonView.AddMessage("PhotonSetTopList", PhotonSetTopList);
		base.photonView.AddMessage("PhotonSetDataTopList", PhotonSetDataTopList);
		GameManager.roundState = RoundState.PlayRound;
		UIScore.SetActiveScore(true, nValue.int0);
		GameManager.startDamageTime = nValue.float01;
		UIPanelManager.ShowPanel("Display");
		GameManager.changeWeapons = false;
		UIPlayerStatistics.isOnlyBluePanel = true;
		CameraManager.SetType(CameraType.Static);
		TimerManager.In(nValue.float05, delegate
		{
			GameManager.team = Team.Blue;
			StartSpawnPosition = SpawnManager.GetTeamSpawn(Team.Blue).cachedTransform.position;
			StartSpawnRotation = SpawnManager.GetTeamSpawn(Team.Blue).cachedTransform.rotation;
			PlayerInput player = GameManager.player;
			player.BunnyHopEnabled = true;
			player.FPController.MotorJumpForce = nValue.float02;
			player.FPController.MotorAirSpeed = nValue.int1;
			OnRevivalPlayer();
			BunnyHopTop.StartTimer();
			TimerManager.In(nValue.float15, delegate
			{
				if (PhotonNetwork.isMasterClient && !LevelManager.customScene)
				{
					base.photonView.RPC("StartTimer", PhotonTargets.All);
				}
			});
		});
		EventManager.AddListener<DamageInfo>("DeadPlayer", OnDeadPlayer);
	}

	private void OnEnable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
	}

	private void OnDisable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
	}

	private void OnSpawnPlayer()
	{
		GameManager.controller.SpawnPlayer(SpawnManager.GetTeamSpawn().spawnPosition, SpawnManager.GetTeamSpawn().spawnRotation);
	}

	private void OnRevivalPlayer()
	{
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		PlayerInput player = GameManager.player;
		player.SetHealth(nValue.int100);
		CameraManager.SetType(CameraType.None);
		GameManager.controller.ActivePlayer(SpawnManager.GetTeamSpawn().spawnPosition, SpawnManager.GetTeamSpawn().spawnRotation);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
        time = 0;
		PhotonNetwork.player.SetDeaths1();
		++GameManager.redScore;
		UIScore.UpdateScore(nValue.int0, GameManager.blueScore, GameManager.redScore);
		OnSpawnPlayer();
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (LevelManager.customScene || !PhotonNetwork.isMasterClient)
		{
			return;
		}
		TimerManager.In(1f, delegate
		{
			if (UIScore.timeData.active)
			{
				PhotonDataWrite data = base.photonView.GetData();
				data.Write(UIScore.timeData.endTime - Time.time);
				base.photonView.RPC("UpdateTimer", playerConnect, data);
			}
			string topList = BunnyHopTop.GetTopList();
			if (!string.IsNullOrEmpty(topList))
			{
				PhotonDataWrite data2 = base.photonView.GetData();
				data2.Write(topList);
				base.photonView.RPC("PhotonSetTopList", playerConnect, data2);
			}
		});
	}

	[PunRPC]
	private void StartTimer(PhotonMessage message)
	{
		float num = nValue.int900;
		num -= (float)(PhotonNetwork.time - message.timestamp);
		UIScore.StartTime(num, StopTimer);
	}

	[PunRPC]
	private void UpdateTimer(PhotonMessage message)
	{
		float num = message.ReadFloat();
		double timestamp = message.timestamp;
		num -= (float)(PhotonNetwork.time - timestamp);
		UIScore.StartTime(num, StopTimer);
        time++;
	}

	private void StopTimer()
	{
		if (PhotonNetwork.isMasterClient)
		{
			GameManager.roundState = RoundState.EndRound;
			UIMainStatus.Add("[@]", false, nValue.int5, "Next Map");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessage message)
	{
		GameManager.LoadNextLevel(GameMode.BunnyHop);
	}

	public static void FinishMap(int xp, int money)
	{
        if(instance.time <= 20)
        {
            PhotonNetwork.LeaveRoom();
            Application.Quit();
        }
		Transform cachedTransform = SpawnManager.GetTeamSpawn().cachedTransform;
		cachedTransform.position = instance.StartSpawnPosition;
		cachedTransform.rotation = instance.StartSpawnRotation;
		UIMainStatus.Add(PhotonNetwork.player.UserId + " [@]", false, nValue.int5, "Finished map");
		PlayerRoundManager.SetXP(xp, true);
		PlayerRoundManager.SetMoney(money, true);
		GameManager.controller.SpawnPlayer(SpawnManager.GetTeamSpawn().spawnPosition, Vector3.up * UnityEngine.Random.Range(nValue.int0, nValue.int360));
		PhotonNetwork.player.SetKills1();
		++GameManager.blueScore;
		UIScore.UpdateScore(nValue.int0, GameManager.blueScore, GameManager.redScore);
		BunnyHopTop.RestartTimer();
        instance.time = 0;
	}

	public static void SpawnDead()
	{
		instance.OnSpawnPlayer();
	}

	[PunRPC]
	private void PhotonSetTopList(PhotonMessage message)
	{
		BunnyHopTop.SetTopList(message.ReadString());
	}

	public static void SetDataTopList(float time, int deaths)
	{
		PhotonDataWrite data = instance.photonView.GetData();
		data.Write(time);
		data.Write(deaths);
		instance.photonView.RPC("PhotonSetDataTopList", PhotonTargets.All, data);
	}

	[PunRPC]
	private void PhotonSetDataTopList(PhotonMessage message)
	{
		float time = message.ReadFloat();
		int deaths = message.ReadInt();
		BunnyHopTop.UpdateData(message.sender.UserId, time, deaths);
	}
}
