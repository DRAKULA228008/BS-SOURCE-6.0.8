using System;
using System.Collections.Generic;

[Serializable]
public class AccountWeaponStickers
{
	public CryptoInt SkinID;

	public List<AccountWeaponStickerData> StickerData = new List<AccountWeaponStickerData>();

	public void SortWeaponStickerData()
	{
		StickerData.Sort(SortWeaponStickerDataComparer);
	}

	private int SortWeaponStickerDataComparer(AccountWeaponStickerData a, AccountWeaponStickerData b)
	{
		return ((int)a.Index).CompareTo(b.Index);
	}

	public int[] ToArray()
	{
		SortWeaponStickerData();
		if (StickerData.Count == 0)
		{
			return new int[0];
		}
		int[] array = new int[(int)StickerData[StickerData.Count - 1].Index];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = -1;
		}
		for (int j = 0; j < StickerData.Count; j++)
		{
			array[(int)StickerData[j].Index - 1] = StickerData[j].StickerID;
		}
		return array;
	}

	public static byte[] Serialize(int weapon, int skin)
	{
		AccountWeaponStickers weaponStickers = AccountManager.GetWeaponStickers(weapon, skin);
		if (weaponStickers == null)
		{
			return new byte[0];
		}
		byte[] array = new byte[weaponStickers.StickerData.Count * 2];
		int num = 0;
		for (int i = 0; i < weaponStickers.StickerData.Count; i++)
		{
			array[num] = (byte)(int)weaponStickers.StickerData[i].Index;
			num++;
			array[num] = (byte)(int)weaponStickers.StickerData[i].StickerID;
			num++;
		}
		return array;
	}

	public static AccountWeaponStickers Deserialize(byte[] bytes)
	{
		AccountWeaponStickers accountWeaponStickers = new AccountWeaponStickers();
		List<AccountWeaponStickerData> list = new List<AccountWeaponStickerData>();
		int num = 0;
		for (int i = 0; i < bytes.Length / 2; i++)
		{
			AccountWeaponStickerData accountWeaponStickerData = new AccountWeaponStickerData();
			accountWeaponStickerData.Index = bytes[num];
			num++;
			accountWeaponStickerData.StickerID = bytes[num];
			num++;
			list.Add(accountWeaponStickerData);
		}
		accountWeaponStickers.StickerData = list;
		return accountWeaponStickers;
	}
}
