using System;
using Photon;
using UnityEngine;

public class SurfMode : Photon.MonoBehaviour
{
	private Vector3 StartSpawnPosition;

	private Quaternion StartSpawnRotation;

	private static SurfMode instance;

	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
		}
		else if (PhotonNetwork.room.GetGameMode() != GameMode.Surf)
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
		GameManager.roundState = RoundState.PlayRound;
		UIScore.SetActiveScore(true, nValue.int0);
		GameManager.startDamageTime = nValue.int1;
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
			player.SurfEnabled = true;
			player.FPCamera.GetComponent<Camera>().farClipPlane = nValue.int300;
			player.FPController.MotorAirSpeed = nValue.float013;
			player.FPController.PhysicsGravityModifier = nValue.float015;
			player.FPController.PhysicsForceDamping = 1.045f;
			player.FPController.PhysicsSlopeSlideLimit = nValue.int90;
			OnRevivalPlayer();
			TimerManager.In(nValue.float15, delegate
			{
				if (PhotonNetwork.isMasterClient)
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
		GameManager.player.StopSurf();
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
		player.StopSurf();
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		++GameManager.redScore;
		UIScore.UpdateScore(nValue.int0, GameManager.blueScore, GameManager.redScore);
		OnSpawnPlayer();
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient && UIScore.timeData.active)
		{
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(UIScore.timeData.endTime - Time.time);
			base.photonView.RPC("UpdateTimer", playerConnect, data);
		}
	}

	[PunRPC]
	private void StartTimer(PhotonMessage message)
	{
		float num = nValue.int360 * nValue.int10;
		num -= (float)(PhotonNetwork.time - message.timestamp);
		UIScore.StartTime(num, StopTimer);
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
			GameManager.roundState = RoundState.EndRound;
			UIMainStatus.Add("[@]", false, nValue.int5, "Next Map");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessage message)
	{
		GameManager.LoadNextLevel(GameMode.Surf);
	}

	public static void FinishMap(int xp, int money)
	{
		Transform cachedTransform = SpawnManager.GetTeamSpawn().cachedTransform;
		cachedTransform.position = instance.StartSpawnPosition;
		cachedTransform.rotation = instance.StartSpawnRotation;
		UIMainStatus.Add(PhotonNetwork.player.UserId + " [@]", false, nValue.int5, "Finished map");
		PlayerRoundManager.SetXP(xp);
		PlayerRoundManager.SetMoney(money);
		GameManager.controller.SpawnPlayer(SpawnManager.GetTeamSpawn().spawnPosition, Vector3.up * UnityEngine.Random.Range(nValue.int0, nValue.int360));
		GameManager.player.StopSurf();
		PhotonNetwork.player.SetKills1();
		++GameManager.blueScore;
		UIScore.UpdateScore(nValue.int0, GameManager.blueScore, GameManager.redScore);
	}

	public static void SpawnDead()
	{
		instance.OnSpawnPlayer();
	}
}
