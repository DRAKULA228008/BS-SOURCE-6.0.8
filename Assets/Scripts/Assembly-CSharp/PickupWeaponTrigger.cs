using UnityEngine;

public class PickupWeaponTrigger : MonoBehaviour
{
	[SelectedWeapon(WeaponType.Rifle)]
	public int Weapon;

	private BoxCollider boxCollider;

	private void Start()
	{
		boxCollider = GetComponent<BoxCollider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player"))
		{
			return;
		}
		PlayerInput player = other.GetComponent<PlayerInput>();
		if (!(player == null) && !(boxCollider == null) && boxCollider.bounds.Intersects(player.mCharacterController.bounds) && !player.PlayerWeapon.GetWeaponData(WeaponType.Rifle).Enabled)
		{
			WeaponManager.SetSelectWeapon(WeaponType.Rifle, Weapon);
			player.PlayerWeapon.UpdateWeapon(WeaponType.Rifle, true);
			TimerManager.In(0.05f, delegate
			{
				player.PlayerWeapon.SetWeapon(WeaponType.Rifle, false);
			});
		}
	}
}
