using System;
using Photon;
using UnityEngine;

public class GunGame : Photon.MonoBehaviour
{
	public CryptoInt MaxScore = 100;

	private int PlayerKills;

	private int[] Weapons = new int[35]
	{
		3, 27, 13, 49, 36, 6, 2, 42, 21, 37,
		9, 25, 26, 14, 24, 12, 7, 50, 18, 29,
		19, 28, 1, 5, 15, 8, 30, 41, 23, 38,
		11, 10, 16, 4, 22
	};

	private int SelectWeaponIndex;

	private WeaponData SelectWeapon;

	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
		}
		else if (PhotonNetwork.room.GetGameMode() != GameMode.GunGame)
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
		WeaponManager.SetSelectWeapon(WeaponType.Knife, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int3);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		GameManager.maxScore = MaxScore;
		GameManager.changeWeapons = false;
		GameManager.StartAutoBalance();
		SelectWeapon = WeaponManager.GetWeaponData(nValue.int3);
		UISelectTeam.OnStart(OnSelectTeam);
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
		playerInput.PlayerWeapon.UpdateWeaponAll(SelectWeapon.Type);
		if (SelectWeapon.Type != WeaponType.Knife)
		{
			TimerManager.In(nValue.float01, delegate
			{
				PlayerWeapons.PlayerWeaponData weaponData = playerInput.PlayerWeapon.GetWeaponData(SelectWeapon.Type);
				weaponData.AmmoMax = (int)weaponData.AmmoMax * nValue.int2;
				UIAmmo.SetAmmo(playerInput.PlayerWeapon.GetSelectedWeaponData().Ammo, playerInput.PlayerWeapon.GetSelectedWeaponData().AmmoMax);
			});
		}
	}

	private void OnUpdateWeapon()
	{
		if (SelectWeaponIndex >= Weapons.Length - nValue.int1)
		{
			SelectWeaponIndex = nValue.int0;
		}
		else
		{
			SelectWeaponIndex++;
		}
		WeaponManager.SetSelectWeapon(WeaponType.Knife, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		SelectWeapon = WeaponManager.GetWeaponData(Weapons[SelectWeaponIndex]);
		UIToast.Show(SelectWeapon.Name);
		SoundManager.Play2D("UpWeapon");
		switch (SelectWeapon.Type)
		{
		case WeaponType.Knife:
			WeaponManager.SetSelectWeapon(WeaponType.Knife, SelectWeapon.ID);
			break;
		case WeaponType.Pistol:
			WeaponManager.SetSelectWeapon(WeaponType.Pistol, SelectWeapon.ID);
			break;
		case WeaponType.Rifle:
			WeaponManager.SetSelectWeapon(WeaponType.Rifle, SelectWeapon.ID);
			break;
		}
		PlayerInput playerInput = GameManager.player;
		if (playerInput.Dead)
		{
			return;
		}
		playerInput.PlayerWeapon.CanFire = false;
		TimerManager.In(nValue.float02, delegate
		{
			if (!playerInput.Dead)
			{
				playerInput.PlayerWeapon.UpdateWeaponAll(SelectWeapon.Type);
				TimerManager.In(nValue.float01, delegate
				{
					playerInput.PlayerWeapon.CanFire = true;
					if (SelectWeapon.Type != WeaponType.Knife)
					{
						PlayerWeapons.PlayerWeaponData weaponData = playerInput.PlayerWeapon.GetWeaponData(SelectWeapon.Type);
						weaponData.AmmoMax = (int)weaponData.AmmoMax * nValue.int2;
						UIAmmo.SetAmmo(playerInput.PlayerWeapon.GetSelectedWeaponData().Ammo, playerInput.PlayerWeapon.GetSelectedWeaponData().AmmoMax);
					}
				});
			}
			else
			{
				playerInput.PlayerWeapon.CanFire = true;
			}
		});
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
			PlayerRoundManager.SetMoney(nValue.int6);
			PlayerRoundManager.SetHeadshot1();
		}
		else
		{
			PlayerRoundManager.SetXP(nValue.int5);
			PlayerRoundManager.SetMoney(nValue.int3);
		}
		if (PlayerKills >= nValue.int1 || (PlayerKills >= nValue.int0 && SelectWeapon.Type == WeaponType.Knife))
		{
			PlayerKills = nValue.int0;
			OnUpdateWeapon();
		}
		else
		{
			PlayerKills++;
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
		GameManager.LoadNextLevel(GameMode.GunGame);
	}
}
