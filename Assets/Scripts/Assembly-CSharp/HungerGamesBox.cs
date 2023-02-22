using System;
using System.Collections.Generic;
using UnityEngine;

public class HungerGamesBox : MonoBehaviour
{
	[Serializable]
	public class WeaponData
	{
		[SelectedWeapon]
		public int Weapon;
	}

	[Range(1f, 50f)]
	public int ID = 1;

	public bool Used;

	public List<WeaponData> Weapons = new List<WeaponData>();

	public DropWeaponStatic dropWeapon;

	private GameObject cachedGameObject;

	private void Start()
	{
		cachedGameObject = base.gameObject;
		EventManager.AddListener("StartRound", StartRound);
		EventManager.AddListener<int, int>("EventPickupBox", Pickup);
		dropWeapon.onDropWeaponEvent += OnDropWeapon;
	}

	private void StartRound()
	{
		cachedGameObject.SetActive(true);
		dropWeapon.weaponID = Weapons[UnityEngine.Random.Range(0, Weapons.Count)].Weapon;
	}

	private void SelectWeapon()
	{
		Used = true;
		HungerGames.SetWeapon(dropWeapon.weaponID);
	}

	private void Pickup(int id, int pickupPlayer)
	{
		if (id == ID)
		{
			cachedGameObject.SetActive(false);
			if (pickupPlayer == PhotonNetwork.player.ID)
			{
				SelectWeapon();
			}
		}
	}

	public void OnDropWeapon()
	{
		HungerGames.PickupBox(ID);
	}
}
