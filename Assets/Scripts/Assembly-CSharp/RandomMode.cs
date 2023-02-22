using System;
using Photon;
using UnityEngine;

public class RandomMode : Photon.MonoBehaviour
{
	public CryptoInt MaxScore = 100;

	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
		}
		else if (PhotonNetwork.room.GetGameMode() != GameMode.RandomMode)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		base.photonView.AddMessage("PhotonOnScore", PhotonOnScore);
		base.photonView.AddMessage("OnKilledPlayer", OnKilledPlayer);
		base.photonView.AddMessage("PhotonNextLevel", PhotonNextLevel);
		GameManager.roundState = RoundState.PlayRound;
		CameraManager.SetType(CameraType.Static);
		UIScore.SetActiveScore(true, MaxScore);
		UISelectTeam.OnStart(OnSelectTeam);
		GameManager.maxScore = MaxScore;
		GameManager.changeWeapons = false;
		GameManager.StartAutoBalance();
		EventManager.AddListener<DamageInfo>("DeadPlayer", OnDeadPlayer);
		EventManager.AddListener<Team>("AutoBalance", OnAutoBalance);
	}

	private void OnEnable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
	}

	private void OnDisable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
	}

	private void OnSelectTeam(Team team)
	{
		UIPanelManager.ShowPanel("Display");
		OnRevivalPlayer();
	}

	private void OnAutoBalance(Team team)
	{
		GameManager.team = team;
		OnRevivalPlayer();
	}

	private void OnRevivalPlayer()
	{
		PlayerInput playerInput = GameManager.player;
		playerInput.SetHealth(nValue.int100);
		CameraManager.SetType(CameraType.None);
		GameManager.controller.ActivePlayer(SpawnManager.GetTeamSpawn().spawnPosition, SpawnManager.GetTeamSpawn().spawnRotation);
		WeaponType selectWeaponType = WeaponType.Rifle;
		int num = UnityEngine.Random.Range(nValue.int0, nValue.int100);
		if (num >= 45)
		{
			selectWeaponType = WeaponType.Rifle;
		}
		else if (num >= nValue.int5)
		{
			selectWeaponType = WeaponType.Pistol;
		}
		else
		{
			selectWeaponType = WeaponType.Knife;
		}
		int randomWeaponID = WeaponManager.GetRandomWeaponID(selectWeaponType);
		WeaponManager.SetSelectWeapon(WeaponType.Knife, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		WeaponManager.SetSelectWeapon(selectWeaponType, randomWeaponID);
		playerInput.PlayerWeapon.UpdateWeaponAll(selectWeaponType);
		if (selectWeaponType != WeaponType.Knife)
		{
			TimerManager.In(nValue.float01, delegate
			{
				PlayerWeapons.PlayerWeaponData weaponData = playerInput.PlayerWeapon.GetWeaponData(selectWeaponType);
				weaponData.AmmoMax = (int)weaponData.AmmoMax * nValue.int2;
				UIAmmo.SetAmmo(playerInput.PlayerWeapon.GetSelectedWeaponData().Ammo, playerInput.PlayerWeapon.GetSelectedWeaponData().AmmoMax);
			});
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		PlayerRoundManager.SetDeaths1();
		UIStatus.Add(damageInfo);
		UIDeathScreen.Show(damageInfo);
		if (damageInfo.otherPlayer)
		{
			OnScore(damageInfo.team);
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(damageInfo.Deserialize());
			base.photonView.RPC("OnKilledPlayer", PhotonPlayer.Find(damageInfo.player), data);
		}
		Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.player.PlayerTransform.position, damageInfo.position);
		CameraManager.SetType(CameraType.Dead, GameManager.player.FPCamera.Transform.position, GameManager.player.FPCamera.Transform.eulerAngles, ragdollForce * nValue.int100);
		GameManager.controller.DeactivePlayer(ragdollForce, damageInfo.headshot);
		TimerManager.In(nValue.int3, delegate
		{
			OnRevivalPlayer();
		});
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			GameManager.SetScore(playerConnect);
		}
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
			PlayerRoundManager.SetXP(nValue.int10);
			PlayerRoundManager.SetMoney(nValue.int7);
			PlayerRoundManager.SetHeadshot1();
		}
		else
		{
			PlayerRoundManager.SetXP(nValue.int5);
			PlayerRoundManager.SetMoney(nValue.int3);
		}
	}

	public void OnScore(Team team)
	{
		PhotonDataWrite data = base.photonView.GetData();
		data.Write((byte)team);
		base.photonView.RPC("PhotonOnScore", PhotonTargets.MasterClient, data);
	}

	[PunRPC]
	private void PhotonOnScore(PhotonMessage message)
	{
		switch (message.ReadByte())
		{
		case 1:
			++GameManager.blueScore;
			break;
		case 2:
			++GameManager.redScore;
			break;
		}
		GameManager.SetScore();
		if (GameManager.checkScore)
		{
			GameManager.roundState = RoundState.EndRound;
			if (GameManager.winTeam == Team.Blue)
			{
				UIMainStatus.Add("[@]", false, nValue.int5, "Blue Win");
			}
			else if (GameManager.winTeam == Team.Red)
			{
				UIMainStatus.Add("[@]", false, nValue.int5, "Red Win");
			}
			base.photonView.RPC("PhotonNextLevel", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void PhotonNextLevel(PhotonMessage message)
	{
		GameManager.LoadNextLevel(GameMode.RandomMode);
	}
}
