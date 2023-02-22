using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
	public CryptoInt selectedKnife;

	public CryptoInt selectedPistol;

	public CryptoInt selectedRifle;

	public static WeaponType DefaultWeaponType = WeaponType.Rifle;

	public static CryptoBool MaxDamage = false;

	private PlayerWeapons CachedPlayerWeapons;

	private static WeaponManager instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public static void Init()
	{
		if (instance == null)
		{
			GameObject gameObject = new GameObject("WeaponManager");
			gameObject.AddComponent<WeaponManager>();
		}
		UpdateData();
	}

	public static void UpdateData()
	{
		MaxDamage = false;
		instance.selectedKnife = AccountManager.GetWeaponSelected(WeaponType.Knife);
		instance.selectedPistol = AccountManager.GetWeaponSelected(WeaponType.Pistol);
		instance.selectedRifle = AccountManager.GetWeaponSelected(WeaponType.Rifle);
	}

	public static int GetSelectWeapon(WeaponType type)
	{
		switch (type)
		{
		case WeaponType.Knife:
			return instance.selectedKnife;
		case WeaponType.Pistol:
			return instance.selectedPistol;
		case WeaponType.Rifle:
			return instance.selectedRifle;
		default:
			return 0;
		}
	}

	public static void SetSelectWeapon(int weaponID)
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((int)GameSettings.instance.Weapons[i].ID == weaponID)
			{
				SetSelectWeapon(GameSettings.instance.Weapons[i].Type, weaponID);
				break;
			}
		}
	}

	public static void SetSelectWeapon(WeaponType weapon, int weaponID)
	{
		switch (weapon)
		{
		case WeaponType.Knife:
			if ((AccountManager.GetGold() >= 0 && AccountManager.GetMoney() >= 0) || !GetWeaponData(weaponID).Secret)
			{
				instance.selectedKnife = weaponID;
			}
			break;
		case WeaponType.Pistol:
			instance.selectedPistol = weaponID;
			break;
		case WeaponType.Rifle:
			instance.selectedRifle = weaponID;
			break;
		}
	}

	public static bool HasSelectWeapon(WeaponType type)
	{
		switch (type)
		{
		case WeaponType.Knife:
			return ((int)instance.selectedKnife != 0) ? true : false;
		case WeaponType.Pistol:
			return ((int)instance.selectedPistol != 0) ? true : false;
		case WeaponType.Rifle:
			return ((int)instance.selectedRifle != 0) ? true : false;
		default:
			return false;
		}
	}

	public static WeaponData GetSelectWeaponData(WeaponType type)
	{
		return GameSettings.instance.Weapons[GetSelectWeapon(type) - 1];
	}

	public static int GetMemberDamage(PlayerSkinMember member, int weaponID)
	{
		if (instance.CachedPlayerWeapons == null)
		{
			instance.CachedPlayerWeapons = GameManager.player.PlayerWeapon;
		}
		PlayerWeapons.PlayerWeaponData weaponData = instance.CachedPlayerWeapons.GetWeaponData(weaponID);
		if (weaponData == null)
		{
			return GetMemberDamage(member, GetWeaponData(weaponID));
		}
		return GetMemberDamage(member, weaponData.FaceDamage, weaponData.BodyDamage, weaponData.HandDamage, weaponData.LegDamage);
	}

	public static int GetMemberDamage(PlayerSkinMember member, WeaponData weaponData)
	{
		return GetMemberDamage(member, weaponData.FaceDamage, weaponData.BodyDamage, weaponData.HandDamage, weaponData.LegDamage);
	}

	public static int GetMemberDamage(PlayerSkinMember member, int faceDamage, int bodyDamage, int handDamage, int legDamage)
	{
		if ((bool)MaxDamage)
		{
			return 100;
		}
		switch (member)
		{
		case PlayerSkinMember.Face:
			if (faceDamage == 100 || faceDamage == 0)
			{
				return faceDamage;
			}
			return faceDamage + Random.Range(-5, 5);
		case PlayerSkinMember.Body:
			if (bodyDamage == 100 || bodyDamage == 0)
			{
				return bodyDamage;
			}
			return bodyDamage + Random.Range(-4, 4);
		case PlayerSkinMember.Hands:
			if (handDamage == 100 || handDamage == 0)
			{
				return handDamage;
			}
			return handDamage + Random.Range(-3, 3);
		case PlayerSkinMember.Legs:
			if (legDamage == 100 || legDamage == 0)
			{
				return legDamage;
			}
			return legDamage + Random.Range(-2, 2);
		default:
			return bodyDamage;
		}
	}

	public static int GetRandomWeaponID()
	{
		List<WeaponData> list = new List<WeaponData>();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((bool)GameSettings.instance.Weapons[i].Lock)
			{
				continue;
			}
			if ((bool)GameSettings.instance.Weapons[i].Secret)
			{
				if (AccountManager.GetWeapon(GameSettings.instance.Weapons[i].ID))
				{
					list.Add(GameSettings.instance.Weapons[i]);
				}
			}
			else
			{
				list.Add(GameSettings.instance.Weapons[i]);
			}
		}
		return list[Random.Range(0, list.Count)].ID;
	}

	public static int GetRandomWeaponID(WeaponType type)
	{
		List<WeaponData> list = new List<WeaponData>();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].Type != type || (bool)GameSettings.instance.Weapons[i].Lock)
			{
				continue;
			}
			if ((bool)GameSettings.instance.Weapons[i].Secret)
			{
				if (AccountManager.GetWeapon(GameSettings.instance.Weapons[i].ID))
				{
					list.Add(GameSettings.instance.Weapons[i]);
				}
			}
			else
			{
				list.Add(GameSettings.instance.Weapons[i]);
			}
		}
		return list[Random.Range(0, list.Count)].ID;
	}

	public static int GetRandomWeaponID(bool rifle, bool pistol, bool knife, bool secret)
	{
		List<WeaponData> list = new List<WeaponData>();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((bool)GameSettings.instance.Weapons[i].Lock)
			{
				continue;
			}
			if ((bool)GameSettings.instance.Weapons[i].Secret)
			{
				if (secret && AccountManager.GetWeapon(GameSettings.instance.Weapons[i].ID))
				{
					list.Add(GameSettings.instance.Weapons[i]);
				}
				continue;
			}
			switch (GameSettings.instance.Weapons[i].Type)
			{
			case WeaponType.Rifle:
				if (rifle)
				{
					list.Add(GameSettings.instance.Weapons[i]);
				}
				break;
			case WeaponType.Pistol:
				if (pistol)
				{
					list.Add(GameSettings.instance.Weapons[i]);
				}
				break;
			case WeaponType.Knife:
				if (knife)
				{
					list.Add(GameSettings.instance.Weapons[i]);
				}
				break;
			}
		}
		return list[Random.Range(0, list.Count)].ID;
	}

	public static string GetWeaponName(int weaponID)
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((int)GameSettings.instance.Weapons[i].ID == weaponID)
			{
				return GameSettings.instance.Weapons[i].Name;
			}
		}
		return string.Empty;
	}

	public static int GetWeaponID(string weaponName)
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((string)GameSettings.instance.Weapons[i].Name == weaponName)
			{
				return GameSettings.instance.Weapons[i].ID;
			}
		}
		return -1;
	}

	public static WeaponData GetWeaponData(int weaponID)
	{
		if (weaponID <= 0)
		{
			weaponID = 3;
		}
		return GameSettings.instance.Weapons[weaponID - 1];
	}

	public static WeaponData GetWeaponData(string weaponName)
	{
		return GetWeaponData(GetWeaponID(weaponName));
	}

	public static WeaponStoreData GetWeaponStoreData(int weaponID)
	{
		if (weaponID <= 0)
		{
			weaponID = 3;
		}
		return GameSettings.instance.WeaponsStore[weaponID - 1];
	}

	public static WeaponSkinData GetWeaponSkin(int weaponID, int skinID)
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((int)GameSettings.instance.Weapons[i].ID != weaponID)
			{
				continue;
			}
			for (int j = 0; j < GameSettings.instance.WeaponsStore[i].Skins.Count; j++)
			{
				if ((int)GameSettings.instance.WeaponsStore[i].Skins[j].ID == skinID)
				{
					return GameSettings.instance.WeaponsStore[i].Skins[j];
				}
			}
		}
		return null;
	}

	public static bool HasWeaponLock(int weaponID)
	{
		return GetWeaponData(weaponID).Lock;
	}

	public static WeaponSkinData GetRandomWeaponSkin(int weaponID)
	{
		return GameSettings.instance.WeaponsStore[weaponID - 1].Skins[Random.Range(0, GameSettings.instance.WeaponsStore[weaponID - 1].Skins.Count)];
	}

	public static StickerData GetStickerData(int id)
	{
		for (int i = 0; i < GameSettings.instance.Stickers.Count; i++)
		{
			if ((int)GameSettings.instance.Stickers[i].ID == id)
			{
				return GameSettings.instance.Stickers[i];
			}
		}
		return null;
	}

	public static string GetStickerName(int id)
	{
		return GetStickerData(id).Name;
	}
}
