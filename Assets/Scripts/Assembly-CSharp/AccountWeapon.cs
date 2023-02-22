using System;
using System.Collections.Generic;

[Serializable]
public class AccountWeapon
{
	public CryptoInt ID = 0;

	public bool Buy;

	public CryptoInt Skin = 0;

	public CryptoInt LastSkin = 0;

	public List<CryptoInt> Skins = new List<CryptoInt>();

	public List<CryptoInt> FireStats = new List<CryptoInt>();

	public List<AccountWeaponStickers> Stickers = new List<AccountWeaponStickers>();

	public void SortWeaponStickers()
	{
		Stickers.Sort(SortWeaponStickersComparer);
		for (int i = 0; i < Stickers.Count; i++)
		{
			Stickers[i].SortWeaponStickerData();
		}
	}

	private int SortWeaponStickersComparer(AccountWeaponStickers a, AccountWeaponStickers b)
	{
		return ((int)a.SkinID).CompareTo(b.SkinID);
	}
}
