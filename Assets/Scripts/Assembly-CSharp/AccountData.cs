using System;
using System.Collections.Generic;
using System.Globalization;
using FreeJSON;
using UnityEngine;

[Serializable]
public class AccountData
{
	public CryptoString GameVersion;

	public CryptoString AccountName;

	public CryptoInt2 ID;

    public Texture2D Avatar;

    public string AvatarUrl;

	public CryptoInt2 Money = 0;

	public CryptoInt2 Gold = 0;

    public CryptoInt2 XP = 0;

	public CryptoInt2 Level = 1;

	public long Time;

	public CryptoInt2 Deaths = 0;

	public CryptoInt2 Kills = 0;

	public CryptoInt2 Headshot = 0;

	public List<CryptoInt> SelectedWeapons = new List<CryptoInt>();

	public CryptoInt SelectedRifle = 12;

	public CryptoInt SelectedPistol = 3;

	public CryptoInt SelectedKnife = 4;

	public AccountPlayerSkinData PlayerSkin = new AccountPlayerSkinData();

	public CryptoString Clan;

	public CryptoInt Session;

	public List<AccountSticker> Stickers = new List<AccountSticker>();

	public List<AccountWeapon> Weapons = new List<AccountWeapon>();

	public List<int> Friends = new List<int>();

	public List<CryptoString> InAppPurchase = new List<CryptoString>();

	public bool UpdateSelectedWeapon;

	public bool UpdateSelectedPlayerSkin;

    public static AccountData instance;

    public void SortStickers()
	{
		Stickers.Sort(SortStickersComparer);
	}

	private int SortStickersComparer(AccountSticker a, AccountSticker b)
	{
		return ((int)a.ID).CompareTo(b.ID);
	}

	public void UpdateData(string data)
	{
		if (JsonObject.isJson(data))
		{
			UpdateData(JsonObject.Parse(data));
		}
    }


	public void UpdateData(JsonObject json)
	{
		if (json.ContainsKey("Gold"))
		{
            Gold = json.Get<int>("Gold");
        }
		if (json.ContainsKey("Gold1"))
		{
			Gold = (int)Gold + json.Get<int>("Gold1");
		}
		if (json.ContainsKey("Money"))
		{
			Money = json.Get<int>("Money");
		}
		if (json.ContainsKey("Money1"))
		{
			Money = (int)Money + json.Get<int>("Money1");
        }
		if (json.ContainsKey("XP"))
		{
			XP = json.Get<int>("XP");
		}
		if (json.ContainsKey("Level"))
		{
			Level = json.Get<int>("Level");
		}
		if (json.ContainsKey("Deaths"))
		{
			Deaths = json.Get<int>("Deaths");
		}
		if (json.ContainsKey("Kills"))
		{
			Kills = json.Get<int>("Kills");
		}
		if (json.ContainsKey("Head"))
		{
			Headshot = json.Get<int>("Head");
		}
		if (json.ContainsKey("time"))
		{
			Time = json.Get<long>("time");
		}
		if (json.ContainsKey("PlayerSkin"))
		{
			PlayerSkin.Deserialize(json.Get<JsonObject>("PlayerSkin"));
		}
		if (json.ContainsKey("Session"))
		{
			Session = json.Get<int>("Session");
		}
		if (json.ContainsKey("SetWeapon"))
		{
			int num = json.Get<int>("SetWeapon");
			for (int i = 0; i < Weapons.Count; i++)
			{
				if ((int)Weapons[i].ID == num)
				{
					Weapons[i].Buy = true;
					break;
				}
			}
		}
		if (json.ContainsKey("Clan"))
		{
			Clan = json.Get<string>("Clan");
		}
		if (json.ContainsKey("Weapon") && json.ContainsKey("Skin"))
		{
			int num2 = json.Get<int>("Weapon");
			int num3 = json.Get<int>("Skin");
			for (int j = 0; j < Weapons.Count; j++)
			{
				if ((int)Weapons[j].ID == num2)
				{
					if (!Weapons[j].Skins.Contains(num3))
					{
						Weapons[j].Skins.Add(num3);
					}
					break;
				}
			}
		}
		EventManager.Dispatch("AccountUpdate");
	}
}
