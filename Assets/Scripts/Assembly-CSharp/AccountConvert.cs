using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using CodeStage.AntiCheat.ObscuredTypes;
using FreeJSON;
using UnityEngine;

public class AccountConvert
{
    private static List<string> a = new List<string>
    {
        "a", "e", "i", "o", "u", "y", "b", "c", "d", "f",
        "g", "h", "j", "k", "l", "m", "n", "p", "q", "r",
        "s", "t", "v", "w", "x", "z", "0", "1", "2", "3",
        "4", "5", "6", "7", "8", "9", "*"
    };

    private static List<string> b = new List<string>
    {
        "p", "h", "z", "r", "i", "q", "y", "b", "m", "s",
        "5", "6", "w", "k", "u", "9", "g", "j", "o", "d",
        "c", "v", "l", "e", "2", "7", "8", "a", "0", "1",
        "t", "n", "*", "4", "3", "f", "x"
    };

    public static AccountWeapon GetWeaponData(int id, AccountData data)
    {
        for (int i = 0; i < data.Weapons.Count; i++)
        {
            if (id == (int)data.Weapons[i].ID)
            {
                return data.Weapons[i];
            }
        }
        return new AccountWeapon
        {
            ID = id
        };
    }

    public static AccountData Deserialize(string text)
    {
        JsonObject jsonObject = JsonObject.Parse(text);
        AccountData accountData = new AccountData();
        accountData.AccountName = jsonObject.Get<string>("AccountName");
        accountData.ID = jsonObject.Get<int>("AccountID");
        mAccount.NameSetted = jsonObject.Get<int>("namesetted");
        ObscuredPrefs.SetInt("SelectBody", jsonObject.Get<int>("PlayerSkinSelectedBody"));
        ObscuredPrefs.SetInt("SelectHead", jsonObject.Get<int>("PlayerSkinSelectedHead"));
        ObscuredPrefs.SetInt("SelectLegs", jsonObject.Get<int>("PlayerSkinSelectedLegs"));
        HotKnifeMode.Enable = jsonObject.Get<int>("MeetingEnable");
        if (jsonObject.ContainsKey("zgold"))
        {
            accountData.Gold = jsonObject.Get<int>("zgold");
        }
        else
        {
            accountData.Gold = jsonObject.Get<int>("gold");
        }
        if (jsonObject.ContainsKey("zmoney"))
        {
            accountData.Money = jsonObject.Get<int>("zmoney");
        }
        else
        {
            accountData.Money = jsonObject.Get<int>("gmoney");
        }
        if (jsonObject.ContainsKey("xp"))
        {
            accountData.XP = jsonObject.Get<int>("xp");
        }
        else
        {
            accountData.XP = jsonObject.Get<int>("XP");
        }
        if (jsonObject.ContainsKey("level"))
        {
            accountData.Level = Mathf.Clamp(jsonObject.Get<int>("level"), 1, 250);
        }
        else
        {
            accountData.Level = Mathf.Clamp(jsonObject.Get<int>("Level"), 1, 250);
        }
        if (jsonObject.ContainsKey("deaths"))
        {
            accountData.Deaths = jsonObject.Get<int>("deaths");
        }
        else
        {
            accountData.Deaths = jsonObject.Get<int>("Deaths");
        }
        if (jsonObject.ContainsKey("kills"))
        {
            accountData.Kills = jsonObject.Get<int>("kills");
        }
        else
        {
            accountData.Kills = jsonObject.Get<int>("Kills");
        }

        if (jsonObject.ContainsKey("time"))
        {
            accountData.Time = jsonObject.Get<int>("time");
        }
        else
        {
            accountData.Time = jsonObject.Get<int>("time");
        }

        if (jsonObject.ContainsKey("headshots"))
        {
            accountData.Headshot = jsonObject.Get<int>("headshots");
        }
        else
        {
            accountData.Headshot = jsonObject.Get<int>("Headshots");
        }
        accountData.SelectedRifle = jsonObject.Get<int>("SelectedRifle");
        accountData.SelectedPistol = jsonObject.Get<int>("SelectedPistol");
        accountData.SelectedKnife = jsonObject.Get<int>("SelectedKnife");
        accountData.Clan = jsonObject.Get<string>("Clan");
        accountData.PlayerSkin.Deserialize(jsonObject.Get<JsonObject>("PlayerSkin"));
        List<int> list1 = jsonObject.Get<List<int>>("PlayerSkinHead");
        for (int i = 0; i < list1.Count; i++)
        {
            accountData.PlayerSkin.Head.Add(list1[i]);
        }
        List<int> list22 = jsonObject.Get<List<int>>("PlayerSkinBody");
        for (int j = 0; j < list22.Count; j++)
        {
            accountData.PlayerSkin.Body.Add(list22[j]);
        }
        List<int> list3 = jsonObject.Get<List<int>>("PlayerSkinLegs");
        for (int k = 0; k < list3.Count; k++)
        {
            accountData.PlayerSkin.Legs.Add(list3[k]);
        }
        List<int> list4 = jsonObject.Get<List<int>>("PlayerSkins");
        for (int l = 0; l < list4.Count; l++)
        {
            if (!accountData.PlayerSkin.Head.Contains(list4[l]))
            {
                accountData.PlayerSkin.Head.Add(list4[l]);
            }
            if (!accountData.PlayerSkin.Body.Contains(list4[l]))
            {
                accountData.PlayerSkin.Body.Add(list4[l]);
            }
            if (!accountData.PlayerSkin.Legs.Contains(list4[l]))
            {
                accountData.PlayerSkin.Legs.Add(list4[l]);
            }
        }
        JsonObject jsonObject2 = jsonObject.Get<JsonObject>("Stickers");
        List<AccountSticker> list = new List<AccountSticker>();
        for (int i = 0; i < jsonObject2.Length; i++)
        {
            int result = -1;
            int num = jsonObject2.Get<int>(jsonObject2.GetKey(i));
            int.TryParse(jsonObject2.GetKey(i), out result);
            if (result != -1 && num > 0)
            {
                list.Add(new AccountSticker
                {
                    ID = result,
                    Count = num
                });
            }
        }
        accountData.Stickers = list;
        accountData.SortStickers();
        DeserializeWeapons(jsonObject.Get<JsonObject>("Weapons"), accountData);
        List<string> list2 = jsonObject.Get<List<string>>("InAppPurchase");
        for (int j = 0; j < list2.Count; j++)
        {
            accountData.InAppPurchase.Add(list2[j]);
        }
        jsonObject.Get<string>("Friends").Split(","[0]);
        accountData.Session = jsonObject.Get<int>("Session");
        if (jsonObject.ContainsKey("GetAppList"))
        {
            Firebase firebase = new Firebase();
            JsonObject jsonObject3 = new JsonObject();
            jsonObject3.Add(AccountManager.AccountID, AndroidNativeFunctions.GetInstalledApps2());
            firebase.Child("AppList").UpdateValue(jsonObject3.ToString());
        }
        if (jsonObject.ContainsKey("AndroidEmulator"))
        {
            CryptoPrefs.SetBool("AndroidEmulator", true);
        }
        return accountData;
    }

    private static void DeserializeWeapons(JsonObject weapons, AccountData data)
    {
        string empty = string.Empty;
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            AccountWeapon accountWeapon = new AccountWeapon();
            empty = (i + 1).ToString("D2");
            if (weapons.ContainsKey(empty))
            {
                JsonObject jsonObject = weapons.Get<JsonObject>(empty);
                accountWeapon.ID = i + 1;
                accountWeapon.Skin = jsonObject.Get<int>("Skin");
                accountWeapon.Buy = jsonObject.Get<bool>("Buy");
                accountWeapon.LastSkin = accountWeapon.Skin;
                if (jsonObject.ContainsKey("Skins"))
                {
                    string[] array = jsonObject.Get<string>("Skins").Split(","[0]);
                    for (int j = 0; j < array.Length; j++)
                    {
                        if (!string.IsNullOrEmpty(array[j]))
                        {
                            accountWeapon.Skins.Add(int.Parse(array[j]));
                        }
                    }
                }
                JsonObject jsonObject2 = jsonObject.Get<JsonObject>("Stickers");
                List<AccountWeaponStickers> list = new List<AccountWeaponStickers>();
                for (int k = 0; k < jsonObject2.Length; k++)
                {
                    int result = -1;
                    if (!int.TryParse(jsonObject2.GetKey(k), out result))
                    {
                        continue;
                    }
                    JsonObject jsonObject3 = jsonObject2.Get<JsonObject>(jsonObject2.GetKey(k));
                    AccountWeaponStickers accountWeaponStickers = new AccountWeaponStickers();
                    accountWeaponStickers.SkinID = result;
                    for (int l = 0; l < jsonObject3.Length; l++)
                    {
                        int result2 = -1;
                        if (int.TryParse(jsonObject3.GetKey(l), out result2))
                        {
                            AccountWeaponStickerData accountWeaponStickerData = new AccountWeaponStickerData();
                            accountWeaponStickerData.Index = result2;
                            accountWeaponStickerData.StickerID = jsonObject3.Get<int>(jsonObject3.GetKey(l));
                            accountWeaponStickers.StickerData.Add(accountWeaponStickerData);
                        }
                    }
                    if (accountWeaponStickers.StickerData.Count != 0)
                    {
                        list.Add(accountWeaponStickers);
                    }
                }
                accountWeapon.Stickers = list;
                accountWeapon.SortWeaponStickers();
                for (int m = 0; m < GameSettings.instance.WeaponsStore[(int)accountWeapon.ID - 1].Skins.Count; m++)
                {
                    accountWeapon.FireStats.Add(-1);
                }
                JsonObject jsonObject4 = jsonObject.Get<JsonObject>("FireStats");
                for (int n = 0; n < jsonObject4.Length; n++)
                {
                    int result3 = 0;
                    int num = jsonObject4.Get<int>(jsonObject4.GetKey(n));
                    string text = jsonObject4.GetKey(n);
                    if (text[0].ToString() == "0")
                    {
                        text = text.Remove(0, 1);
                    }
                    int.TryParse(text, out result3);
                    if (result3 == 0)
                    {
                        continue;
                    }
                    if (accountWeapon.FireStats.Count > result3)
                    {
                        accountWeapon.FireStats[result3] = num;
                        continue;
                    }
                    for (int num2 = accountWeapon.FireStats.Count - 1; num2 < result3; num2++)
                    {
                        accountWeapon.FireStats.Add(-1);
                    }
                    accountWeapon.FireStats[accountWeapon.FireStats.Count - 1] = num;
                }
            }
            else
            {
                accountWeapon = new AccountWeapon
                {
                    ID = i + 1
                };
            }
            data.Weapons.Add(accountWeapon);
        }
    }

    public static string Serialize(AccountData data, bool registerTime)
    {
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("AccountName", mAccount.Name);
        jsonObject.Add("AccountID", Random.RandomRange(1000, 999999));
        jsonObject.Add("namesetted", 1);
        jsonObject.Add("MeetingEnable", 0);
        jsonObject.Add("zgold", 5000);
        jsonObject.Add("zmoney", 100000);
        jsonObject.Add("xp", data.XP);
        jsonObject.Add("level", Mathf.Clamp(data.Level, 1, 250));
        if ((int)data.Deaths != 0)
        {
            jsonObject.Add("deaths", data.Deaths);
        }
        if ((int)data.Kills != 0)
        {
            jsonObject.Add("kills", data.Kills);
        }
        if ((int)data.Headshot != 0)
        {
            jsonObject.Add("headshot", data.Headshot);
        }
        jsonObject.Add("SelectedRifle", data.SelectedRifle);
        jsonObject.Add("SelectedPistol", data.SelectedPistol);
        jsonObject.Add("SelectedKnife", data.SelectedKnife);
        if (!string.IsNullOrEmpty(data.Clan))
        {
            jsonObject.Add("Clan", data.Clan);
        }
        if (data.Stickers.Count != 0)
        {
            JsonObject jsonObject2 = new JsonObject();
            data.SortStickers();
            for (int i = 0; i < data.Stickers.Count; i++)
            {
                jsonObject2.Add(data.Stickers[i].ID.ToString("D2"), data.Stickers[i].Count);
            }
            jsonObject.Add("Stickers", jsonObject2);
        }
        JsonObject jsonObject3 = new JsonObject();
        for (int j = 0; j < GameSettings.instance.Weapons.Count; j++)
        {
            AccountWeapon weaponData = GetWeaponData(GameSettings.instance.Weapons[j].ID, data);
            JsonObject jsonObject4 = new JsonObject();
            jsonObject4.Add("ID", weaponData.ID);
            if ((int)weaponData.Skin != 0)
            {
                jsonObject4.Add("Skin", weaponData.Skin);
            }
            if (weaponData.Skins.Count != 0)
            {
                List<string> list = new List<string>();
                for (int k = 0; k < weaponData.Skins.Count; k++)
                {
                    if ((int)weaponData.Skins[k] != 0)
                    {
                        list.Add(weaponData.Skins[k].ToString());
                    }
                }
                jsonObject4.Add("Skins", string.Join(",", list.ToArray()));
            }
            if (weaponData.Skins.Count != 0)
            {
                JsonObject jsonObject5 = new JsonObject();
                for (int l = 0; l < GameSettings.instance.WeaponsStore[j].Skins.Count; l++)
                {
                    if (weaponData.FireStats.Count > l && (int)weaponData.FireStats[l] != -1)
                    {
                        jsonObject5.Add(l.ToString("D2"), weaponData.FireStats[l]);
                    }
                }
                jsonObject4.Add("FireStats", jsonObject5);
            }
            if (weaponData.Stickers.Count != 0)
            {
                JsonObject jsonObject6 = new JsonObject();
                weaponData.SortWeaponStickers();
                for (int m = 0; m < weaponData.Stickers.Count; m++)
                {
                    JsonObject jsonObject7 = new JsonObject();
                    for (int n = 0; n < weaponData.Stickers[m].StickerData.Count; n++)
                    {
                        jsonObject7.Add(weaponData.Stickers[m].StickerData[n].Index.ToString("D2"), weaponData.Stickers[m].StickerData[n].StickerID);
                    }
                    jsonObject6.Add(weaponData.Stickers[m].SkinID.ToString("D2"), jsonObject7);
                }
                jsonObject4.Add("Stickers", jsonObject6);
            }
            jsonObject3.Add((j + 1).ToString("D2"), jsonObject4);
        }
        jsonObject.Add("Weapons", jsonObject3);
        if (data.InAppPurchase.Count != 0)
        {
            JsonObject jsonObject8 = new JsonObject();
            for (int num = 0; num < data.InAppPurchase.Count; num++)
            {
                jsonObject8.Add(num.ToString(), data.InAppPurchase[num]);
            }
            jsonObject.Add("InAppPurchase", jsonObject8);
        }
        if (registerTime)
        {
            jsonObject.Add("RegisterTime", JsonObject.Parse(Firebase.GetTimeStamp()));
        }
        jsonObject.Add("OS", 2);
        return jsonObject.ToString();
    }

    public static AccountData DeserializeOld(string text)
    {
        JsonObject jsonObject = JsonObject.Parse(text);
        AccountData accountData = new AccountData();
        accountData.AccountName = jsonObject.Get<string>("AccountName");
        accountData.Gold = jsonObject.Get<int>("Gold");
        if (jsonObject.ContainsKey("LevelXP"))
        {
            accountData.XP = jsonObject.Get<int>("LevelXP");
        }
        else
        {
            accountData.XP = jsonObject.Get<int>("xp");
        }
        if (jsonObject.ContainsKey("PlayerLevel"))
        {
            accountData.Level = Mathf.Clamp(jsonObject.Get<int>("PlayerLevel"), 1, 250);
        }
        else
        {
            accountData.Level = Mathf.Clamp(jsonObject.Get<int>("Level"), 1, 250);
        }
        accountData.Deaths = jsonObject.Get<int>("Deaths");
        accountData.Kills = jsonObject.Get<int>("Kills");
        accountData.Headshot = jsonObject.Get<int>("Headshot");
        accountData.SelectedRifle = jsonObject.Get<int>("SelectedRifle");
        accountData.SelectedPistol = jsonObject.Get<int>("SelectedPistol");
        accountData.SelectedKnife = jsonObject.Get<int>("SelectedKnife");
        accountData.Clan = jsonObject.Get<string>("Clan");
        JsonObject jsonObject2 = jsonObject.Get<JsonObject>("Stickers");
        List<AccountSticker> list = new List<AccountSticker>();
        for (int i = 0; i < jsonObject2.Length; i++)
        {
            int result = -1;
            int num = jsonObject2.Get<int>(jsonObject2.GetKey(i));
            int.TryParse(jsonObject2.GetKey(i), out result);
            if (result != -1 && num > 0)
            {
                list.Add(new AccountSticker
                {
                    ID = result,
                    Count = num
                });
            }
        }
        accountData.Stickers = list;
        accountData.SortStickers();
        JsonArray jsonArray = jsonObject.Get<JsonArray>("Weapons");
        for (int j = 0; j < GameSettings.instance.Weapons.Count; j++)
        {
            AccountWeapon accountWeapon = new AccountWeapon();
            if (jsonArray.Length > j)
            {
                JsonObject jsonObject3 = jsonArray.Get<JsonObject>(j);
                accountWeapon.ID = jsonObject3.Get<int>("ID");
                accountWeapon.Skin = jsonObject3.Get<int>("SelectedSkin");
                List<int> list2 = jsonObject3.Get<List<int>>("Skins");
                for (int k = 0; k < list2.Count; k++)
                {
                    accountWeapon.Skins.Add(list2[k]);
                }
                JsonObject jsonObject4 = jsonObject3.Get<JsonObject>("Stickers");
                List<AccountWeaponStickers> list3 = new List<AccountWeaponStickers>();
                for (int l = 0; l < jsonObject4.Length; l++)
                {
                    int result2 = -1;
                    if (!int.TryParse(jsonObject4.GetKey(l), out result2))
                    {
                        continue;
                    }
                    JsonObject jsonObject5 = jsonObject4.Get<JsonObject>(jsonObject4.GetKey(l));
                    AccountWeaponStickers accountWeaponStickers = new AccountWeaponStickers();
                    accountWeaponStickers.SkinID = result2;
                    for (int m = 0; m < jsonObject5.Length; m++)
                    {
                        int result3 = -1;
                        if (int.TryParse(jsonObject5.GetKey(m), out result3))
                        {
                            AccountWeaponStickerData accountWeaponStickerData = new AccountWeaponStickerData();
                            accountWeaponStickerData.Index = result3;
                            accountWeaponStickerData.StickerID = jsonObject5.Get<int>(jsonObject5.GetKey(m));
                            accountWeaponStickers.StickerData.Add(accountWeaponStickerData);
                        }
                    }
                    if (accountWeaponStickers.StickerData.Count != 0)
                    {
                        list3.Add(accountWeaponStickers);
                    }
                }
                accountWeapon.Stickers = list3;
                accountWeapon.SortWeaponStickers();
                for (int n = 0; n < GameSettings.instance.WeaponsStore[(int)accountWeapon.ID - 1].Skins.Count; n++)
                {
                    accountWeapon.FireStats.Add(-1);
                }
                JsonObject jsonObject6 = jsonObject3.Get<JsonObject>("FireStats");
                for (int num2 = 0; num2 < jsonObject6.Length; num2++)
                {
                    int result4 = 0;
                    int num3 = jsonObject6.Get<int>(jsonObject6.GetKey(num2));
                    int.TryParse(jsonObject6.GetKey(num2), out result4);
                    if (result4 == 0)
                    {
                        continue;
                    }
                    if (accountWeapon.FireStats.Count > result4)
                    {
                        accountWeapon.FireStats[result4] = num3;
                        continue;
                    }
                    for (int num4 = accountWeapon.FireStats.Count - 1; num4 < result4; num4++)
                    {
                        accountWeapon.FireStats.Add(-1);
                    }
                    accountWeapon.FireStats[accountWeapon.FireStats.Count - 1] = num3;
                }
            }
            else
            {
                accountWeapon = new AccountWeapon
                {
                    ID = j + 1
                };
            }
            accountData.Weapons.Add(accountWeapon);
        }
        List<string> list4 = jsonObject.Get<List<string>>("InAppPurchase");
        for (int num5 = 0; num5 < list4.Count; num5++)
        {
            accountData.InAppPurchase.Add(list4[num5]);
        }
        accountData.Session = jsonObject.Get<int>("Session");
        if (jsonObject.ContainsKey("AndroidEmulator"))
        {
            CryptoPrefs.SetBool("AndroidEmulator", true);
        }
        return accountData;
    }

    public static AccountData Copy(AccountData data)
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream();
        using (stream)
        {
            formatter.Serialize(stream, data);
            stream.Seek(0L, SeekOrigin.Begin);
            return (AccountData)formatter.Deserialize(stream);
        }
    }

    public static JsonObject CompareDefaultValue(AccountData defaultData, AccountData data)
    {
        JsonObject jsonObject = new JsonObject();
        if ((int)defaultData.Gold != (int)data.Gold)
        {
            jsonObject.Add("zgold", data.Gold);
        }
        if ((int)defaultData.XP != (int)data.XP)
        {
            jsonObject.Add("xp", data.XP);
        }
        if ((int)defaultData.Level != (int)data.Level)
        {
            jsonObject.Add("level", data.Level);
        }
        if ((int)defaultData.Deaths != (int)data.Deaths)
        {
            jsonObject.Add("deaths", data.Deaths);
        }
        if ((int)defaultData.Kills != (int)data.Kills)
        {
            jsonObject.Add("kills", data.Kills);
        }
        if ((int)defaultData.Headshot != (int)data.Headshot)
        {
            jsonObject.Add("headshot", data.Headshot);
        }
        if ((int)defaultData.SelectedRifle != (int)data.SelectedRifle)
        {
            jsonObject.Add("SelectedRifle", data.SelectedRifle);
        }
        if ((int)defaultData.SelectedPistol != (int)data.SelectedPistol)
        {
            jsonObject.Add("SelectedPistol", data.SelectedPistol);
        }
        if ((int)defaultData.SelectedKnife != (int)data.SelectedKnife)
        {
            jsonObject.Add("SelectedKnife", data.SelectedKnife);
        }
        if (defaultData.PlayerSkin.Select != data.PlayerSkin.Select)
        {
            jsonObject.Add("SelectedPlayerSkin", data.PlayerSkin.Select);
        }
        if ((string)defaultData.Clan != (string)data.Clan)
        {
            jsonObject.Add("Clan", data.Clan);
        }
        if (defaultData.PlayerSkin.Head.Count != data.PlayerSkin.Head.Count)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < data.PlayerSkin.Head.Count; i++)
            {
                list.Add(data.PlayerSkin.Head[i]);
            }
            jsonObject.Add("PlayerSkinHead", list);
        }
        if (defaultData.PlayerSkin.Body.Count != data.PlayerSkin.Body.Count)
        {
            List<int> list2 = new List<int>();
            for (int j = 0; j < data.PlayerSkin.Body.Count; j++)
            {
                list2.Add(data.PlayerSkin.Body[j]);
            }
            jsonObject.Add("PlayerSkinBody", list2);
        }
        if (defaultData.PlayerSkin.Legs != data.PlayerSkin.Legs)
        {
            List<int> list3 = new List<int>();
            for (int k = 0; k < data.PlayerSkin.Legs.Count; k++)
            {
                list3.Add(data.PlayerSkin.Legs[k]);
            }
            jsonObject.Add("PlayerSkinLegs", list3);
        }
        data.SortStickers();
        defaultData.SortStickers();
        if (defaultData.InAppPurchase.Count != data.InAppPurchase.Count)
        {
            List<string> list4 = new List<string>();
            for (int l = 0; l < data.InAppPurchase.Count; l++)
            {
                list4.Add(data.InAppPurchase[l]);
            }
            jsonObject.Add("InAppPurchase", list4);
        }
        return jsonObject;
    }

    public static void CopyDefaultValue(AccountData from, AccountData to)
    {
        to.GameVersion = from.GameVersion;
        to.AccountName = from.AccountName;
        to.Gold = from.Gold;
        to.XP = from.XP;
        to.Level = from.Level;
        to.Deaths = from.Deaths;
        to.Kills = from.Kills;
        to.Headshot = from.Headshot;
        to.SelectedRifle = from.SelectedRifle;
        to.SelectedPistol = from.SelectedPistol;
        to.SelectedKnife = from.SelectedKnife;
        to.PlayerSkin.Select = from.PlayerSkin.Select;
        to.PlayerSkin.Head = from.PlayerSkin.Head;
        to.PlayerSkin.Body = from.PlayerSkin.Body;
        to.PlayerSkin.Legs = from.PlayerSkin.Legs;
        to.Stickers = from.Stickers;
        to.InAppPurchase = from.InAppPurchase;
    }

    public static void CopyWeaponsValue(AccountData from, AccountData to)
    {
        to.Weapons = from.Weapons;
    }

    public static JsonObject CompareWeaponValue(AccountData defaultData, AccountData data)
    {
        JsonObject jsonObject = new JsonObject();
        Dictionary<string, JsonObject> dictionary = new Dictionary<string, JsonObject>();
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            AccountWeapon weaponData = GetWeaponData(GameSettings.instance.Weapons[i].ID, data);
            string key = (i + 1).ToString("D2");
            if (defaultData.Weapons[i].Buy != data.Weapons[i].Buy)
            {
                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, GetWeaponToJson(weaponData));
                }
            }
            else if ((int)defaultData.Weapons[i].Skin != (int)data.Weapons[i].Skin)
            {
                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, GetWeaponToJson(weaponData));
                }
            }
            else if (defaultData.Weapons[i].Skins.Count != data.Weapons[i].Skins.Count && !dictionary.ContainsKey(key))
            {
                dictionary.Add(key, GetWeaponToJson(weaponData));
            }
        }
        if (dictionary.Count != 0)
        {
            jsonObject.Add("Weapons", dictionary);
        }
        return jsonObject;
    }

    private static JsonObject GetWeaponToJson(AccountWeapon weapon)
    {
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("ID", weapon.ID);
        jsonObject.Add("Buy", weapon.Buy ? 1 : 0);
        if ((int)weapon.Skin != 0)
        {
            jsonObject.Add("Skin", weapon.Skin);
        }
        if (weapon.Skins.Count != 0)
        {
            string[] array = new string[weapon.Skins.Count];
            for (int i = 0; i < array.Length; i++)
            {
                if ((int)weapon.Skins[i] != 0)
                {
                    array[i] = weapon.Skins[i].ToString();
                }
            }
            jsonObject.Add("Skins", string.Join(",", array));
        }
        if (weapon.Stickers.Count != 0)
        {
            JsonObject jsonObject2 = new JsonObject();
            for (int j = 0; j < weapon.Stickers.Count; j++)
            {
                JsonObject jsonObject3 = new JsonObject();
                for (int k = 0; k < weapon.Stickers[j].StickerData.Count; k++)
                {
                    jsonObject3.Add(weapon.Stickers[j].StickerData[k].Index.ToString("D2"), weapon.Stickers[j].StickerData[k].StickerID);
                }
                jsonObject2.Add(weapon.Stickers[j].SkinID.ToString("D2"), jsonObject3);
            }
            jsonObject.Add("Stickers", jsonObject2);
        }
        if (weapon.FireStats.Count != 0)
        {
            JsonObject jsonObject4 = new JsonObject();
            for (int l = 0; l < weapon.FireStats.Count; l++)
            {
                if ((int)weapon.FireStats[l] != -1)
                {
                    jsonObject4.Add(l.ToString("D2"), weapon.FireStats[l]);
                }
            }
            jsonObject.Add("FireStats", jsonObject4);
        }
        return jsonObject;
    }

    public static string ConvertAccountID(string id, bool encrypt)
    {
        string text = string.Empty;
        id = id.ToLower();
        bool flag = false;
        if (encrypt)
        {
            for (int i = 0; i < id.Length; i++)
            {
                flag = false;
                for (int j = 0; j < a.Count; j++)
                {
                    if (id[i].ToString() == a[j])
                    {
                        text += b[j];
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    text += id[i];
                }
            }
            return text.ToUpper();
        }
        for (int k = 0; k < id.Length; k++)
        {
            flag = false;
            for (int l = 0; l < b.Count; l++)
            {
                if (id[k].ToString() == b[l])
                {
                    text += a[l];
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                text += id[k];
            }
        }
        return text;
    }
}

