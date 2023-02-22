using UnityEngine;

public class MeetingMode : MonoBehaviour
{
	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			Object.Destroy(this);
		}
		else if (PhotonNetwork.room.GetGameMode() != GameMode.Meeting)
		{
			Object.Destroy(this);
		}
	}

	private void Start()
	{
		GameManager.roundState = RoundState.PlayRound;
		GameManager.startDamageTime = nValue.int2;
		UIScore.SetActiveScore(false);
		UIPanelManager.ShowPanel("Display");
		UIPlayerStatistics.isOnlyBluePanel = true;
		CameraManager.SetType(CameraType.Static);
		TimerManager.In(nValue.float05, delegate
		{
			GameManager.team = Team.Blue;
			OnRevivalPlayer();
		});
		EventManager.AddListener<DamageInfo>("DeadPlayer", OnDeadPlayer);
	}

	private void OnRevivalPlayer()
	{
		PlayerInput player = GameManager.player;
		player.SetHealth(nValue.int100);
		CameraManager.SetType(CameraType.None);
		GameManager.controller.ActivePlayer(SpawnManager.GetTeamSpawn().spawnPosition, SpawnManager.GetTeamSpawn().spawnRotation);
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
		player.PlayerWeapon.CanFire = false;
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		UIDeathScreen.Show(damageInfo);
		UIStatus.Add(damageInfo);
		Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.player.PlayerTransform.position, damageInfo.position);
		CameraManager.SetType(CameraType.Dead, GameManager.player.FPCamera.Transform.position, GameManager.player.FPCamera.Transform.eulerAngles, ragdollForce * nValue.int100);
		GameManager.controller.DeactivePlayer(ragdollForce, damageInfo.headshot);
		TimerManager.In(nValue.int3, delegate
		{
			OnRevivalPlayer();
		});
	}

	public void OnClimb(bool active)
	{
		PlayerInput.instance.SetClimb(active);
	}

	public void OnIce(bool active)
	{
		PlayerInput.instance.SetMoveIce(active);
	}

	public void OnTrampoline(float force)
	{
		PlayerInput.instance.FPController.AddForce(new Vector3(0f, force, 0f));
	}

	public void OnSize(float size)
	{
		PlayerSkin[] array = Object.FindObjectsOfType<PlayerSkin>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Controller.transform.localScale = Vector3.one * size;
		}
	}

	public void GiveKnife(int weaponID)
	{
		WeaponManager.SetSelectWeapon(WeaponType.Knife, weaponID);
		PlayerInput player = GameManager.player;
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
	}
}
