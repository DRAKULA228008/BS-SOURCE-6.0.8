using System.Collections.Generic;
using Photon;
using UnityEngine;

public class DropWeaponManager : Photon.MonoBehaviour
{
	public bool actived;

	public static float lastDropTime;

	public static float lastPickupTime;

	private List<DropWeapon> list = new List<DropWeapon>();

	private static DropWeaponManager instance;

	public static bool enable
	{
		get
		{
			return instance.actived;
		}
		set
		{
			instance.actived = value;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		PhotonRPC.AddMessage("PhotonCreateWeapon", PhotonCreateWeapon);
		PhotonRPC.AddMessage("PhotonMasterPickupWeapon", PhotonMasterPickupWeapon);
		PhotonRPC.AddMessage("PhotonPickupWeapon", PhotonPickupWeapon);
	}

	public static void ClearScene()
	{
		if (instance == null)
		{
			return;
		}
		for (int i = 0; i < instance.list.Count; i++)
		{
			if (instance.list[i].Weapon.activeSelf)
			{
				instance.list[i].DestroyWeapon();
			}
		}
	}

	public static bool CreateWeapon(bool local, byte weaponID, byte weaponSkin, bool checkPos, Vector3 pos, Vector3 rot, byte[] stickers, int firestat, int ammo, int ammoMax)
	{
		if (checkPos)
		{
			RaycastHit hitInfo;
			if (!Physics.Raycast(pos, Vector3.down, out hitInfo, 50f))
			{
				return false;
			}
			pos = hitInfo.point + hitInfo.normal * 0.01f;
			rot = Quaternion.LookRotation(hitInfo.normal).eulerAngles;
		}
		lastDropTime = Time.time;
		PhotonDataWrite data = PhotonRPC.GetData();
		data.Write(weaponID);
		data.Write(weaponSkin);
		data.Write(pos);
		data.Write(rot);
		data.Write(stickers);
		data.Write(firestat);
		data.Write(ammo);
		data.Write(ammoMax);
		if (local)
		{
			PhotonRPC.RPC("PhotonCreateWeapon", PhotonNetwork.player, data);
		}
		else
		{
			PhotonRPC.RPC("PhotonCreateWeapon", PhotonTargets.All, data);
		}
		return true;
	}

	[PunRPC]
	private void PhotonCreateWeapon(PhotonMessage message)
	{
		byte b = message.ReadByte();
		byte b2 = message.ReadByte();
		Vector3 position = message.ReadVector3();
		Vector3 eulerAngles = message.ReadVector3();
		byte[] stickers = message.ReadBytes();
		int num = message.ReadInt();
		int num2 = message.ReadInt();
		int num3 = message.ReadInt();
		if (WeaponManager.GetWeaponData(b).Type != WeaponType.Knife)
		{
			GameObject gameObject = PoolManager.Spawn(b + "-Drop", GameSettings.instance.Weapons[b - 1].DropPrefab);
			DropWeapon component = gameObject.GetComponent<DropWeapon>();
			gameObject.transform.position = position;
			gameObject.transform.eulerAngles = eulerAngles;
			component.ID = (int)(message.timestamp * 100.0);
			component.Data.Ammo = num2;
			component.Data.AmmoMax = num3;
			component.Data.Skin = b2;
			component.Data.FireStatCounter = num;
			component.Data.Stickers = ConvertStickers(stickers);
			component.DestroyDrop = true;
			component.CustomData = true;
			component.UpdateWeapon();
			if (!list.Contains(component))
			{
				list.Add(component);
			}
			EventManager.Dispatch("DropWeapon", component);
		}
	}

	public static void PickupWeapon(int id)
	{
		lastPickupTime = Time.time;
		PhotonRPC.RPC("PhotonMasterPickupWeapon", PhotonTargets.MasterClient, id);
	}

	[PunRPC]
	private void PhotonMasterPickupWeapon(PhotonMessage message)
	{
		int num = message.ReadInt();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].ID == num && list[i].Weapon.activeSelf)
			{
				PhotonDataWrite data = PhotonRPC.GetData();
				data.Write(message.sender.ID);
				data.Write(num);
				PhotonRPC.RPC("PhotonPickupWeapon", PhotonTargets.All, data);
				break;
			}
		}
	}

	[PunRPC]
	private void PhotonPickupWeapon(PhotonMessage message)
	{
		int num = message.ReadInt();
		int id = message.ReadInt();
		DropWeapon weapon = GetWeapon(id);
		if (!(weapon == null))
		{
			if (PhotonNetwork.player.ID == num)
			{
				weapon.PickupWeapon();
			}
			else
			{
				weapon.DestroyWeapon();
			}
		}
	}

	private DropWeapon GetWeapon(int id)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].ID == id)
			{
				return list[i];
			}
		}
		return null;
	}

	private CryptoInt[] ConvertStickers(byte[] stickers)
	{
		if (stickers == null)
		{
			return new CryptoInt[0];
		}
		CryptoInt[] array = new CryptoInt[stickers.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (stickers[i] == byte.MaxValue)
			{
				array[i] = -1;
			}
			else
			{
				array[i] = stickers[i];
			}
		}
		return array;
	}
}
