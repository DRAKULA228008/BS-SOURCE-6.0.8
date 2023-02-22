using System;
using Photon;
using UnityEngine;

public class AWPMode : Photon.MonoBehaviour
{
	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
		}
		else if (PhotonNetwork.room.GetGameMode() != GameMode.AWPMode)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		base.photonView.AddMessage("OnKilledPlayer", OnKilledPlayer);
		base.photonView.AddMessage("PhotonOnScore", PhotonOnScore);
		base.photonView.AddMessage("PhotonNextLevel", PhotonNextLevel);
		GameManager.roundState = RoundState.PlayRound;
		CameraManager.SetType(CameraType.Static);
		UIScore.SetActiveScore(true, nValue.int80);
        WeaponManager.SetSelectWeapon(WeaponType.Knife, AccountManager.GetWeaponSelected(WeaponType.Knife));
        WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int8);
		GameManager.changeWeapons = false;
		GameManager.maxScore = nValue.int80;
		GameManager.StartAutoBalance();
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
        WeaponManager.SetSelectWeapon(WeaponType.Knife, AccountManager.GetWeaponSelected(WeaponType.Knife));
        WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int8);
		PlayerInput player = GameManager.player;
		player.SetHealth(nValue.int100);
		CameraManager.SetType(CameraType.None);
		GameManager.controller.ActivePlayer(SpawnManager.GetTeamSpawn().spawnPosition, SpawnManager.GetTeamSpawn().spawnRotation);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Rifle);
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
			PlayerRoundManager.SetMoney(nValue.int9);
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
		GameManager.LoadNextLevel(GameMode.AWPMode);
	}
}
