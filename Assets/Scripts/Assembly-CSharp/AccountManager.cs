using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using FreeJSON;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    public AccountData Data = new AccountData();

    public CryptoString[] Links = new CryptoString[0];

    public static bool isConnect;

    public static CryptoString AccountID;

    public static AccountManager instance;

    public static AccountClan Clan;

    public static CryptoString AccountToken;

    public AccountData DefaultData = new AccountData();

    public static long LastLogin;

    private string OldData;

    public static string AccountParent
    {
        get
        {
            return AccountID;
        }
    }

    public static string PlayerID
    {
        get
        {
            return AccountConvert.ConvertAccountID(AccountID, true);
        }
    }

    static AccountManager()
    {
        Clan = new AccountClan();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        }
        else
        {
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }

    public static void Init()
    {
        new GameObject("AccountManager").AddComponent<AccountManager>();
    }

    public static void Login(Action<bool> complete, Action<string> failed)
    {
        Login(AccountID, complete, failed);
    }

    public static void Login(string id, Action<bool> complete, Action<string> failed)
    {
        AccountID = id;

        Firebase firebase = new Firebase();
        firebase.Auth = AccountToken;
        firebase.Child("Players").Child("ServerCheck").GetValue(FirebaseParam.Default.OrderByKey().EqualTo(AccountID), delegate (string result)
        {
            JsonObject jsonObject = JsonObject.Parse(result);
            if (jsonObject.Length != 0)
            {
                mPopUp.SetActiveWait(false);
                mPopUp.ShowPopup(" ажетс€ сервер закрыт, скоро все наладитс€.", "server", Localization.Get("Exit"), mAccount.Exit, Localization.Get("Connect"), mAccount.Reconnect);
            }
            else
            {
                LoginNext(complete, failed);
            }
        }, delegate
        {
            Application.Quit();
        });
    }

    private static void SetAvatar(string url)
    {

    }

    public static bool CheckVersion()
    {
        return Utils.CompareVersion(VersionManager.bundleVersion, instance.Data.GameVersion);
    }

    public static bool CheckAndroidEmulator()
    {
        if (AndroidEmulatorDetector.isEmulator() && !CryptoPrefs.GetBool("AndroidEmulator", false))
        {
            TimerManager.In(0.2f, false, delegate
            {
                Application.Quit();
            });
            AndroidNativeFunctions.ShowToast("Android Emulator Detected");
            return true;
        }
        return false;
    }

    public static void UpdateName(string newName, Action<string> complete, Action<string> failed)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountToken;
        string oldName = instance.Data.AccountName;
        string parentNew = newName.ToUpper()[0].ToString();
        firebase.Child("Players").Child("NickNames").Child(parentNew)
            .Child(newName)
            .GetValue(delegate (string result)
            {
                bool flag = false;
                if (result != "null" && result != "{}")
                {
                    flag = true;
                }
                if (flag)
                {
                    failed("Name already taken");
                }
                else
                {
                    JsonObject json3 = new JsonObject();
                    json3.Add("AccountName", newName);
                    firebase = new Firebase();
                    firebase.Auth = AccountToken;
                    firebase.Child("Players").Child("Accounts").Child(AccountParent)
                        .Child(AccountID)
                        .UpdateValue(json3.ToString(), delegate
                        {
                            instance.Data.AccountName = newName;
                            complete(newName);
                            firebase = new Firebase();
                            firebase.Auth = AccountToken;
                            json3 = new JsonObject();
                            json3.Add(newName, AccountID);
                            firebase.Child("Players").Child("NickNames").Child(parentNew)
                                .UpdateValue(json3.ToString(), delegate
                                {
                                    if (!string.IsNullOrEmpty(oldName))
                                    {
                                        firebase = new Firebase();
                                        firebase.Auth = AccountToken;
                                        firebase.Child("Players").Child("NickNames").Child(oldName.ToUpper()[0].ToString())
                                            .Child(oldName)
                                            .Delete();
                                    }
                                }, delegate
                                {
                                    if (!string.IsNullOrEmpty(oldName))
                                    {
                                        firebase = new Firebase();
                                        firebase.Auth = AccountToken;
                                        firebase.Child("Players").Child("NickNames").Child(oldName.ToUpper()[0].ToString())
                                            .Child(oldName)
                                            .Delete();
                                    }
                                });
                        }, delegate (string error, string json2)
                        {
                            failed(error);
                        });
                }
            }, delegate (string error)
            {
                failed(error);
            });
    }

    public static void UpdateData(Action failed)
    {
        UpdateData(failed, null);
    }

    public static void UpdateRound(JsonObject data, Action<string> complete, Action<string> failed)
    {
    }

    public static void BuyPlayerSkin(int id, BodyParts part, Action complete, Action<string> failed)
    {
    }

    public static void OpenSkinCase(int id, Action<string> complete, Action<string> failed)
    {
    }

    public static void OpenStickerCase(int id, Action<string> complete, Action<string> failed)
    {
    }

    public static void InAppPurchase(JsonObject purchase, Action<string> complete, Action<string> failed)
    {
    }

    public static void Rewarded(GameCurrency currency, Action complete, Action<string> failed)
    {
    }

    public static void UpdateData(Action failed, Action complete)
    {
        AccountData oldData = AccountConvert.DeserializeOld(instance.OldData);
        Firebase firebase = new Firebase();
        firebase.Auth = AccountToken;
        firebase.Child("Players").Child("Accounts").Child(AccountParent)
            .Child(AccountID)
            .Child("LevelXP")
            .Delete();
        firebase.Child("Players").Child("Accounts").Child(AccountParent)
            .Child(AccountID)
            .Child("PlayerLevel")
            .Delete();
        TimerManager.In(1f, delegate
        {
            string json3 = AccountConvert.Serialize(oldData, false);
            firebase = new Firebase();
            firebase.Auth = AccountToken;
            firebase.Child("Players").Child("Accounts").Child(AccountParent)
                .Child(AccountID)
                .UpdateValue(json3, delegate (string result)
                {
                    instance.DefaultData = AccountConvert.Deserialize(result);
                    instance.Data = AccountConvert.Deserialize(result);
                    isConnect = true;
                    UpdateLastLogin();
                    UpdateSession();
                    mPopUp.HideAll("Menu");
                    mPopUp.SetActiveWait(false);
                    EventManager.Dispatch("AccountUpdate");
                    WeaponManager.UpdateData();
                }, delegate
                {
                    UpdateData(null);
                    isConnect = false;
                });
        });
    }

    public static void SetSticker(int weapon, int skin, int sticker, int pos, Action complete, Action<string> failed)
    {
    }

    public static void DeleteSticker(int weapon, int skin, int pos, Action complete, Action<string> failed)
    {
    }

    public static void BuyWeapon(int id, Action complete, Action<string> failed)
    {
    }

    public static void AddFriend(int id, Action complete, Action<string> failed)
    {
    }

    public static void DeleteFriend(int id, Action complete, Action<string> failed)
    {
    }

    public static void GetFriendsName(int[] ids, Action complete, Action<string> failed)
    {
    }

    public static void GetFriendsInfo(int id, Action<string> complete, Action<string> failed)
    {
    }

    public static int GetMoney()
    {
        return instance.Data.Money;
    }

    public static int GetGold()
    {
        return instance.Data.Gold;
    }

    public static long GetTime()
    {
        return instance.Data.Time;
    }

    public static int GetXP()
    {
        if (GetLevel() == 250)
        {
            return GetMaxXP();
        }
        return instance.Data.XP;
    }

    public static int GetMaxXP()
    {
        return 150 + 150 * GetLevel();
    }

    public static int GetLevel()
    {
        return instance.Data.Level;
    }

    public static List<string> GetInAppPurchase()
    {
        List<string> list = new List<string>();
        for (int i = 0; i < instance.Data.InAppPurchase.Count; i++)
        {
            list.Add(instance.Data.InAppPurchase[i]);
        }
        return list;
    }

    public static int GetDeaths()
    {
        return instance.Data.Deaths;
    }

    public static int GetKills()
    {
        return instance.Data.Kills;
    }

    public static int GetHeadshot()
    {
        return instance.Data.Headshot;
    }

    public static void SetWeaponSelected(WeaponType weaponType, int weaponID)
    {
        switch (weaponType)
        {
            case WeaponType.Knife:
                if ((GetGold() < 0 || GetMoney() < 0) && (bool)WeaponManager.GetWeaponData(weaponID).Secret)
                {
                    return;
                }
                instance.Data.SelectedKnife = weaponID;
                break;
            case WeaponType.Pistol:
                instance.Data.SelectedPistol = weaponID;
                break;
            case WeaponType.Rifle:
                instance.Data.SelectedRifle = weaponID;
                break;
        }
        instance.Data.UpdateSelectedWeapon = true;
    }

    public static int GetWeaponSelected(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Knife:
                return instance.Data.SelectedKnife;
            case WeaponType.Pistol:
                return instance.Data.SelectedPistol;
            case WeaponType.Rifle:
                return instance.Data.SelectedRifle;
            default:
                return 0;
        }
    }

    public static void SetPlayerSkinSelected(int id, BodyParts part)
    {
        switch (part)
        {
            case BodyParts.Head:
                ObscuredPrefs.SetInt("SelectHead", id);
                AccountManager.SetPlayerSkinSelected2(null, null);
                return;
            case BodyParts.Body:
                ObscuredPrefs.SetInt("SelectBody", id);
                AccountManager.SetPlayerSkinSelected2(null, null);
                return;
            case BodyParts.Legs:
                ObscuredPrefs.SetInt("SelectLegs", id);
                AccountManager.SetPlayerSkinSelected2(null, null);
                return;
            default:
                return;
        }
    }

    public static int GetPlayerSkinSelected(BodyParts part)
    {
            switch (part)
            {
                case BodyParts.Head:
                    return ObscuredPrefs.GetInt("SelectHead", 0);
                case BodyParts.Body:
                    return ObscuredPrefs.GetInt("SelectBody", 0);
                case BodyParts.Legs:
                    return ObscuredPrefs.GetInt("SelectLegs", 0);
                default:
                    return -1;
            }
        }

    public static bool GetPlayerSkin(int id, BodyParts part)
    {
        if (id == 0)
        {
            return true;
        }
        switch (part)
        {
            case BodyParts.Head:
                return instance.Data.PlayerSkin.Head.Contains(id);
            case BodyParts.Body:
                return instance.Data.PlayerSkin.Body.Contains(id);
            case BodyParts.Legs:
                return instance.Data.PlayerSkin.Legs.Contains(id);
            default:
                return false;
        }
    }

    public static bool GetWeapon(int id)
    {
        if (id == nValue.int12 || id == nValue.int3 || id == nValue.int4)
        {
            return true;
        }
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            if ((int)GameSettings.instance.Weapons[i].ID != id)
            {
                continue;
            }
            if ((bool)GameSettings.instance.Weapons[i].Secret)
            {
                for (int j = 0; j < instance.Data.Weapons.Count; j++)
                {
                    if ((int)instance.Data.Weapons[j].ID == id)
                    {
                        if (instance.Data.Weapons[j].Skins != null)
                        {
                            return instance.Data.Weapons[j].Skins.Count != 0;
                        }
                        return false;
                    }
                }
                continue;
            }
            for (int k = 0; k < instance.Data.Weapons.Count; k++)
            {
                if ((int)instance.Data.Weapons[k].ID == id)
                {
                    return instance.Data.Weapons[k].Buy;
                }
            }
        }
        return false;
    }

    public static void SetWeaponSkin(int id, int skin)
    {
        for (int i = 0; i < instance.Data.Weapons.Count; i++)
        {
            if ((int)instance.Data.Weapons[i].ID == id)
            {
                if (!instance.Data.Weapons[i].Skins.Contains(skin))
                {
                    instance.Data.Weapons[i].Skins.Add(skin);
                }
                break;
            }
        }
    }

    public static bool GetWeaponSkin(int id, int skin)
    {
        if (skin == 0)
        {
            return true;
        }
        for (int i = 0; i < instance.Data.Weapons.Count; i++)
        {
            if ((int)instance.Data.Weapons[i].ID == id)
            {
                return instance.Data.Weapons[i].Skins.Contains(skin);
            }
        }
        return false;
    }

    public static void SetWeaponSkinSelected(int id, int skin)
    {
        for (int i = 0; i < instance.Data.Weapons.Count; i++)
        {
            if ((int)instance.Data.Weapons[i].ID == id)
            {
                instance.Data.Weapons[i].LastSkin = instance.Data.Weapons[i].Skin;
                instance.Data.Weapons[i].Skin = skin;
                break;
            }
        }
    }

    public static int GetWeaponSkinSelected(int id)
    {
        if (id == 0)
        {
            return 0;
        }
        for (int i = 0; i < instance.Data.Weapons.Count; i++)
        {
            if ((int)instance.Data.Weapons[i].ID == id)
            {
                return instance.Data.Weapons[i].Skin;
            }
        }
        return 0;
    }

    public static void SetFireStat(int id, int skin)
    {
        if (skin == 0)
        {
            return;
        }
        int i = 0;
        while (i < AccountManager.instance.Data.Weapons.Count)
        {
            if (AccountManager.instance.Data.Weapons[i].ID == id)
            {
                if (AccountManager.instance.Data.Weapons[i].FireStats.Count <= skin)
                {
                    for (int j = AccountManager.instance.Data.Weapons[i].FireStats.Count - 1; j < skin; j++)
                    {
                        AccountManager.instance.Data.Weapons[i].FireStats.Add(-1);
                    }
                    AccountManager.instance.Data.Weapons[i].FireStats[AccountManager.instance.Data.Weapons[i].FireStats.Count - 1] = 0;
                    return;
                }
                if (AccountManager.instance.Data.Weapons[i].FireStats[skin] < 0)
                {
                    AccountManager.instance.Data.Weapons[i].FireStats[skin] = 0;
                    return;
                }
                return;
            }
            else
            {
                i++;
            }
        }
    }

    // Token: 0x06001EB4 RID: 7860 RVA: 0x000A5B10 File Offset: 0x000A3D10
    public static bool GetFireStat(int id, int skin)
    {
        if (skin == 0)
        {
            return false;
        }
        for (int i = 0; i < AccountManager.instance.Data.Weapons.Count; i++)
        {
            if (AccountManager.instance.Data.Weapons[i].ID == id)
            {
                return AccountManager.instance.Data.Weapons[i].FireStats.Count > skin && AccountManager.instance.Data.Weapons[i].FireStats[skin] != -1;
            }
        }
        return false;
    }

    // Token: 0x06001EB5 RID: 7861 RVA: 0x000A5BB8 File Offset: 0x000A3DB8
    public static void SetFireStatCounter(int id, int skin, int value)
    {
        if (skin == 0)
        {
            return;
        }
        int i = 0;
        while (i < AccountManager.instance.Data.Weapons.Count)
        {
            if (AccountManager.instance.Data.Weapons[i].ID == id)
            {
                if (AccountManager.instance.Data.Weapons[i].FireStats.Count > skin)
                {
                    AccountManager.instance.Data.Weapons[i].FireStats[skin] = value;
                    return;
                }
                for (int j = AccountManager.instance.Data.Weapons[i].FireStats.Count - 1; j < skin; j++)
                {
                    AccountManager.instance.Data.Weapons[i].FireStats.Add(-1);
                }
                AccountManager.instance.Data.Weapons[i].FireStats[AccountManager.instance.Data.Weapons[i].FireStats.Count - 1] = value;
                return;
            }
            else
            {
                i++;
            }
        }
    }

    // Token: 0x06001EB6 RID: 7862 RVA: 0x000A5CF0 File Offset: 0x000A3EF0
    public static int GetFireStatCounter(int id, int skin)
    {
        if (skin == 0)
        {
            return -1;
        }
        int i = 0;
        while (i < AccountManager.instance.Data.Weapons.Count)
        {
            if (AccountManager.instance.Data.Weapons[i].ID == id)
            {
                if (AccountManager.instance.Data.Weapons[i].FireStats.Count > skin)
                {
                    return AccountManager.instance.Data.Weapons[i].FireStats[skin];
                }
                return -1;
            }
            else
            {
                i++;
            }
        }
        return -1;
    }

    public static void SetFireStatFireBase(bool iscase, int weaponid, int skinid, int firestat, Action<string> complete, Action<string> failed)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        TimerManager.In(2f, delegate ()
        {
            if (iscase)
            {
                if (skinid > 9)
                {
                    jsonObject.Add(skinid.ToString(), 0);
                }
                else
                {
                    jsonObject.Add(0 + skinid.ToString(), 0);
                }
            }
            else if (skinid > 9)
            {
                jsonObject.Add(skinid.ToString(), firestat);
            }
            else
            {
                jsonObject.Add(0 + skinid.ToString(), firestat);
            }
            if (weaponid > 9)
            {
                firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(weaponid.ToString()).Child("FireStats").UpdateValue(jsonObject.ToString(), delegate (string result)
                {
                }, null);
                return;
            }
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(0 + weaponid.ToString()).Child("FireStats").UpdateValue(jsonObject.ToString(), delegate (string result)
            {
            }, null);
        });
    }

    public static string GetClan()
    {
        return instance.Data.Clan;
    }

    public static void SetStickers(int id)
    {
        if (GetStickers(id))
        {
            for (int i = 0; i < instance.Data.Stickers.Count; i++)
            {
                if ((int)instance.Data.Stickers[i].ID == id)
                {
                    AccountSticker accountSticker = instance.Data.Stickers[i];
                    accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = ++accountSticker.Count)))))))))))))))));
                    break;
                }
            }
        }
        else
        {
            AccountSticker accountSticker2 = new AccountSticker();
            accountSticker2.ID = id;
            accountSticker2.Count = 1;
            instance.Data.Stickers.Add(accountSticker2);
            instance.Data.SortStickers();
        }
    }

    public static void DeleteSticker(int id, bool update, Action<bool> complete, Action<string> failed)
    {
        if (GetStickers(id))
        {
            for (int i = 0; i < instance.Data.Stickers.Count; i++)
            {
                if ((int)instance.Data.Stickers[i].ID == id)
                {
                    AccountSticker accountSticker = instance.Data.Stickers[i];
                    accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = (accountSticker.Count = --accountSticker.Count))));
                    if ((int)instance.Data.Stickers[i].Count == 0)
                    {
                        instance.Data.Stickers.RemoveAt(i);
                        instance.Data.SortStickers();
                    }
                    break;
                }
            }
        }
        if (update)
        {
            UpdateDefaultData(null, null);
        }
    }

    public static bool GetStickers(int id)
    {
        for (int i = 0; i < instance.Data.Stickers.Count; i++)
        {
            if ((int)instance.Data.Stickers[i].ID == id)
            {
                return true;
            }
        }
        return false;
    }

    public static int[] GetStickers()
    {
        int[] array = new int[instance.Data.Stickers.Count];
        for (int i = 0; i < instance.Data.Stickers.Count; i++)
        {
            array[i] = instance.Data.Stickers[i].ID;
        }
        return array;
    }

    public static int GetStickerCount(int id)
    {
        for (int i = 0; i < instance.Data.Stickers.Count; i++)
        {
            if ((int)instance.Data.Stickers[i].ID == id)
            {
                return instance.Data.Stickers[i].Count;
            }
        }
        return 0;
    }

    public static void SetWeaponSticker(int weapon, int skin, int pos, int sticker)
    {
        AccountWeaponStickers weaponStickers = GetWeaponStickers(weapon, skin);
        if (HasWeaponSticker(weapon, skin, pos))
        {
            for (int i = 0; i < weaponStickers.StickerData.Count; i++)
            {
                if ((int)weaponStickers.StickerData[i].Index == pos)
                {
                    weaponStickers.StickerData[i].StickerID = sticker;
                    break;
                }
            }
        }
        else
        {
            AccountWeaponStickerData accountWeaponStickerData = new AccountWeaponStickerData();
            accountWeaponStickerData.Index = pos;
            accountWeaponStickerData.StickerID = sticker;
            weaponStickers.StickerData.Add(accountWeaponStickerData);
            weaponStickers.SortWeaponStickerData();
        }
    }

    public static void DeleteWeaponSticker(int weapon, int skin, int pos)
    {
        AccountWeaponStickers weaponStickers = GetWeaponStickers(weapon, skin);
        if (weaponStickers == null)
        {
            return;
        }
        for (int i = 0; i < weaponStickers.StickerData.Count; i++)
        {
            if ((int)weaponStickers.StickerData[i].Index == pos)
            {
                weaponStickers.StickerData.RemoveAt(i);
                weaponStickers.SortWeaponStickerData();
                break;
            }
        }
    }

    public static bool HasWeaponSticker(int weapon, int skin, int pos)
    {
        for (int i = 0; i < instance.Data.Weapons.Count; i++)
        {
            if ((int)instance.Data.Weapons[i].ID != weapon)
            {
                continue;
            }
            for (int j = 0; j < instance.Data.Weapons[i].Stickers.Count; j++)
            {
                if ((int)instance.Data.Weapons[i].Stickers[j].SkinID != skin)
                {
                    continue;
                }
                for (int k = 0; k < instance.Data.Weapons[i].Stickers[j].StickerData.Count; k++)
                {
                    if ((int)instance.Data.Weapons[i].Stickers[j].StickerData[k].Index == pos)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static int GetWeaponSticker(int weapon, int skin, int pos)
    {
        for (int i = 0; i < instance.Data.Weapons.Count; i++)
        {
            if ((int)instance.Data.Weapons[i].ID != weapon)
            {
                continue;
            }
            for (int j = 0; j < instance.Data.Weapons[i].Stickers.Count; j++)
            {
                if ((int)instance.Data.Weapons[i].Stickers[j].SkinID != skin)
                {
                    continue;
                }
                for (int k = 0; k < instance.Data.Weapons[i].Stickers[j].StickerData.Count; k++)
                {
                    if ((int)instance.Data.Weapons[i].Stickers[j].StickerData[k].Index == pos)
                    {
                        return instance.Data.Weapons[i].Stickers[j].StickerData[k].StickerID;
                    }
                }
            }
        }
        return -1;
    }

    public static AccountWeaponStickers GetWeaponStickers(int weapon, int skin)
    {
        for (int i = 0; i < instance.Data.Weapons.Count; i++)
        {
            if ((int)instance.Data.Weapons[i].ID != weapon)
            {
                continue;
            }
            for (int j = 0; j < instance.Data.Weapons[i].Stickers.Count; j++)
            {
                if ((int)instance.Data.Weapons[i].Stickers[j].SkinID == skin)
                {
                    return instance.Data.Weapons[i].Stickers[j];
                }
            }
            AccountWeaponStickers accountWeaponStickers = new AccountWeaponStickers();
            accountWeaponStickers.SkinID = skin;
            instance.Data.Weapons[i].Stickers.Add(accountWeaponStickers);
            return instance.Data.Weapons[i].Stickers[instance.Data.Weapons[i].Stickers.Count - 1];
        }
        return new AccountWeaponStickers();
    }

    private void OnApplicationQuit()
    {
        // nPlayerPrefs.Save();
    }

    public static bool isOldAccountVersion(string text)
    {
        return JsonObject.Parse(text).Get<int>("OS") != 2;
    }

    private static void LoginNext(Action<bool> complete, Action<string> failed)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountToken;
        firebase.Child("Players").Child("Accounts").Child(AccountParent)
            .Child(AccountID)
            .GetValue(delegate (string result)
            {
                if (result == "null")
                {
                    complete(false);
                    isConnect = false;
                }
                else if (isOldAccountVersion(result))
                {
                    instance.OldData = result;
                    mPopUp.SetActiveWait(false);
                    mPopUp.ShowText(Localization.Get("Please wait") + "...");
                }
                else
                {
                    instance.DefaultData = AccountConvert.Deserialize(result);
                    instance.Data = AccountConvert.Deserialize(result);
                    complete(true);
                    isConnect = true;
                    UpdateLastLogin();
                    UpdateSession();
                    if (!PlayerPrefs.HasKey("DELETEDATA2"))
                    {
                        firebase = new Firebase();
                        firebase.Auth = AccountToken;
                        firebase.Child("Players").Child("Accounts").Child(AccountParent)
                            .Child(AccountID)
                            .Child("Money")
                            .Delete();
                        firebase.Child("Players").Child("Accounts").Child(AccountParent)
                            .Child(AccountID)
                            .Child("Gold")
                            .Delete();
                        firebase.Child("Players").Child("Accounts").Child(AccountParent)
                            .Child(AccountID)
                            .Child("LevelXP")
                            .Delete();
                        firebase.Child("Players").Child("Accounts").Child(AccountParent)
                            .Child(AccountID)
                            .Child("PlayerLevel")
                            .Delete();
                        PlayerPrefs.SetInt("DELETEDATA1", 2);
                    }
                    EventManager.Dispatch("AccountConnected");
                }
            }, delegate (string error)
            {
            });
    }

    public static void UpdateLastLogin()
    {
        if (!isConnect)
        {
            return;
        }
        TimerManager.In(UnityEngine.Random.value, delegate
        {
            Firebase firebase = new Firebase();
            firebase.Auth = AccountToken;
            firebase.Child("Players").Child("Accounts").Child(AccountParent)
                .Child(AccountID)
                .Child("LastLogin")
                .SetValue(JsonObject.Parse(Firebase.GetTimeStamp()).ToString(), delegate (string result)
                {
                    long.TryParse(result, out LastLogin);
                }, null);
        });
    }

    public static void UpdateSession()
    {
        TimerManager.In(UnityEngine.Random.value, delegate
        {
            Firebase obj = new Firebase
            {
                Auth = AccountToken
            };
            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("Session", (int)instance.Data.Session + 1);
            obj.Child("Players").Child("Accounts").Child(AccountParent)
                .Child(AccountID)
                .UpdateValue(jsonObject.ToString(), delegate
                {
                    AccountData data = instance.Data;
                    data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = ++data.Session))))))))))))))))))))))))))))))))))));
                }, null);
        });
    }

    public static void SetGold(int gold)
    {
        SetGold(gold, false, null, null);
    }

    public static void CheckSession(Action<bool> action)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountToken;
        firebase.Child("Players").Child("Accounts").Child(AccountParent)
            .Child(AccountID)
            .Child("Session")
            .GetValue(delegate (string result)
            {
                if (result == instance.Data.Session.ToString())
                {
                    action(true);
                }
                else
                {
                    action(false);
                    if (isConnect)
                    {
                        UIToast.Show(Localization.Get("Session is already outdated"));
                        UIToast.Show(Localization.Get("Restart the game"));
                        isConnect = false;
                        PhotonNetwork.Disconnect();
                    }
                }
            }, delegate
            {
                action(false);
                if (isConnect)
                {
                    UIToast.Show(Localization.Get("Session is already outdated"));
                    UIToast.Show(Localization.Get("Restart the game"));
                    isConnect = false;
                    PhotonNetwork.Disconnect();
                }
            });
    }

    public static void UpdateDefaultData(Action<bool> complete, Action<string, string> failed)
    {
        JsonObject json3 = AccountConvert.CompareDefaultValue(instance.DefaultData, instance.Data);
        if (json3.Length == 0)
        {
            return;
        }
        if (complete != null)
        {
            complete(false);
        }
        CheckSession(delegate (bool session)
        {
            if (session && isConnect)
            {
                AccountData data = AccountConvert.Copy(instance.Data);
                Firebase firebase = new Firebase();
                firebase.Auth = AccountToken;
                firebase.Child("Players").Child("Accounts").Child(AccountParent)
                    .Child(AccountID)
                    .UpdateValue(json3.ToString(), delegate
                    {
                        AccountConvert.CopyDefaultValue(data, instance.DefaultData);
                        if (complete != null)
                        {
                            complete(true);
                        }
                    }, delegate (string error, string json2)
                    {
                        if (failed != null)
                        {
                            failed(error, json2);
                        }
                    });
            }
        });
    }

    public static void SetGold(int gold, bool update)
    {
        SetGold(gold, update, null, null);
    }

    public static void SetGold(int gold, bool update, Action<bool> complete, Action<string, string> failed)
    {
        instance.Data.Gold = Mathf.Min(gold, 100000000);
        if (update)
        {
            UpdateDefaultData(complete, failed);
        }
    }

    public static void SetGold1(int gold)
    {
        SetGold1(gold, false, null, null);
    }

    public static void SetGold1(int gold, bool update)
    {
        SetGold1(gold, update, null, null);
    }

    public static void SetGold1(int gold, bool update, Action<bool> complete, Action<string, string> failed)
    {
        SetGold(GetGold() + gold, update, complete, failed);
    }

    public static void Register(AccountData data, Action complete, Action<string> failed)
    {
        string json = AccountConvert.Serialize(data, true);
        Firebase firebase = new Firebase();
        firebase.Auth = AccountToken;
        firebase.Child("Players").Child("Accounts").Child(AccountParent)
            .Child(AccountID)
            .UpdateValue(json, delegate (string result)
            {
                instance.DefaultData = AccountConvert.Deserialize(result);
                instance.Data = AccountConvert.Deserialize(result);
                complete();
                isConnect = true;
                UpdateLastLogin();
                UpdateSession();
            }, delegate (string error, string sdrg)
            {
                failed(error);
                isConnect = false;
            });
    }

    public static void Register(string playerName, Action complete, Action<string> failed)
    {
        Register(new AccountData
        {
            Weapons =
            {
                new AccountWeapon
                {
                    ID = 12
                },
                new AccountWeapon
                {
                    ID = 4
                },
                new AccountWeapon
                {
                    ID = 3
                }
            }
        }, complete, failed);
    }

    public static void SetPlayerSkinSelected2(Action<string> complete, Action<string> failed)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("PlayerSkinSelectedBody", ObscuredPrefs.GetInt("SelectBody"));
        jsonObject.Add("PlayerSkinSelectedLegs", ObscuredPrefs.GetInt("SelectLegs"));
        jsonObject.Add("PlayerSkinSelectedHead", ObscuredPrefs.GetInt("SelectHead"));
        firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString(), delegate (string result)
        {

        }, null);
    }

    public static void SetXP(int xp)
    {
        SetXP(xp, false, null, null);
    }

    public static void SetXP(int xp, bool update)
    {
        SetXP(xp, update, null, null);
    }

    public static void SetXP(int xp, bool update, Action<bool> complete, Action<string, string> failed)
    {
        instance.Data.XP = xp;
        if (update)
        {
            UpdateDefaultData(complete, failed);
        }
    }

    public static void SetXP1(int xp)
    {
        int level = GetLevel();
        int num = GetXP() + xp;
        int maxXP = GetMaxXP();
        if (num >= maxXP)
        {
            if (level == 250)
            {
                num = maxXP;
            }
            else
            {
                level++;
                num -= maxXP;
                num = Mathf.Max(num, 0);
                maxXP = 150 + 150 * level;
                SetLevel(level);
                UpdateLevel(level, null, null);
                if (LevelManager.GetSceneName() == "Menu")
                {
                    UIToast.Show(Localization.Get("New Level") + " " + level);
                    SetGold1(10);
                }
            }
        }
        SetXP(num);
    }

    public static void SetLevel(int level)
    {
        SetLevel(level, false, null, null);
    }

    public static void SetLevel(int level, bool update)
    {
        SetLevel(level, update, null, null);
    }

    public static void SetLevel(int level, bool update, Action<bool> complete, Action<string, string> failed)
    {
        instance.Data.Level = level;
        if (update)
        {
            UpdateDefaultData(complete, failed);
        }
    }

    public static void SetKills(int kills)
    {
        SetKills(kills, false, null, null);
    }

    public static void SetKills(int kills, bool update)
    {
        SetKills(kills, update, null, null);
    }

    public static void SetKills(int kills, bool update, Action<bool> complete, Action<string, string> failed)
    {
        instance.Data.Kills = kills;
        if (update)
        {
            UpdateDefaultData(complete, failed);
        }
    }

    public static void SetKills1(int kills)
    {
        SetKills1(kills, false, null, null);
    }

    public static void SetKills1(int kills, bool update)
    {
        SetKills1(kills, update, null, null);
    }

    public static void SetKills1(int kills, bool update, Action<bool> complete, Action<string, string> failed)
    {
        SetKills(GetKills() + kills, update, complete, failed);
    }

    public static void SetDeaths(int deaths)
    {
        SetDeaths(deaths, false, null, null);
    }

    public static void SetDeaths(int deaths, bool update)
    {
        SetDeaths(deaths, update, null, null);
    }

    public static void SetDeaths(int deaths, bool update, Action<bool> complete, Action<string, string> failed)
    {
        instance.Data.Deaths = deaths;
        if (update)
        {
            UpdateDefaultData(complete, failed);
        }
    }

    public static void SetDeaths1(int deaths)
    {
        SetDeaths1(deaths, false, null, null);
    }

    public static void SetDeaths1(int deaths, bool update)
    {
        SetDeaths1(deaths, update, null, null);
    }

    public static void SetDeaths1(int deaths, bool update, Action<bool> complete, Action<string, string> failed)
    {
        SetDeaths(GetDeaths() + deaths, update, complete, failed);
    }

    public static void SetHeadshot(int headshot)
    {
        SetHeadshot(headshot, false, null, null);
    }

    public static void SetHeadshot(int headshot, bool update)
    {
        SetHeadshot(headshot, update, null, null);
    }

    public static void SetHeadshot(int headshot, bool update, Action<bool> complete, Action<string, string> failed)
    {
        instance.Data.Headshot = headshot;
        if (update)
        {
            UpdateDefaultData(complete, failed);
        }
    }

    public static void SetHeadshot1(int headshot)
    {
        SetHeadshot1(headshot, false, null, null);
    }

    public static void SetHeadshot1(int headshot, bool update)
    {
        SetHeadshot1(headshot, update, null, null);
    }

    public static void SetHeadshot1(int headshot, bool update, Action<bool> complete, Action<string, string> failed)
    {
        SetHeadshot(GetHeadshot() + headshot, update, complete, failed);
    }


    public static void SetTime(long time, bool update)
    {
        SetTime(time, update, null, null);
    }

    public static void SetTime(long time, bool update, Action<bool> complete, Action<string, string> failed)
    {
        instance.Data.Time = time;
        if (update)
        {
            UpdateDefaultData(complete, failed);
        }
    }

    public static void SetTime1(long time)
    {
        SetTime1(time, false, null, null);
    }

    public static void SetTime1(long time, bool update)
    {
        SetTime1(time, update, null, null);
    }

    public static void SetTime1(long time, bool update, Action<bool> complete, Action<string, string> failed)
    {
        SetTime(GetTime() + time, update, complete, failed);
    }

    public static void UpdateGold(int gold, Action<string> complete, Action<string> failed)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("zgold", gold);
        firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString(), delegate (string result)
        {

        }, null);
    }

    // Token: 0x06001EF1 RID: 7921 RVA: 0x000A6AB8 File Offset: 0x000A4CB8
    public static void UpdateXP(int XP, Action<string> complete, Action<string> failed)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("xp", XP);
        firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString(), delegate (string result)
        {

        }, null);
    }

    // Token: 0x06001EF2 RID: 7922 RVA: 0x000A6B4C File Offset: 0x000A4D4C
    public static void UpdateKills(int Kills, Action<string> complete, Action<string> failed)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("kills", Kills);
        firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString(), delegate (string result)
        {

        }, null);
    }

    // Token: 0x06001EF3 RID: 7923 RVA: 0x000A6BE0 File Offset: 0x000A4DE0
    public static void UpdateDeaths(int Deaths, Action<string> complete, Action<string> failed)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("deaths", Deaths);
        firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString(), delegate (string result)
        {

        }, null);
    }

    // Token: 0x06001EF4 RID: 7924 RVA: 0x000A6C74 File Offset: 0x000A4E74
    public static void UpdateHeadshots(int Headshots, Action<string> complete, Action<string> failed)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("headshot", Headshots);
        firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString(), delegate (string result)
        {

        }, null);
    }

    // Token: 0x06001EF5 RID: 7925 RVA: 0x000A6D08 File Offset: 0x000A4F08
    public static void UpdatePlayerRoundData(int xp, int kills, int deaths, int headshots, long time, Action<string> complete, Action<string> failed)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("xp", xp);
        jsonObject.Add("kills", kills);
        jsonObject.Add("deaths", deaths);
        jsonObject.Add("headshots", headshots);
        jsonObject.Add("time", time);
        firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString(), delegate (string result)
        {

        }, null);
    }

    // Token: 0x06001EF6 RID: 7926 RVA: 0x000A6DCC File Offset: 0x000A4FCC
    public static void SetFirebaseWeaponsSelected(Action<string> complete, Action<string> failed)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("SelectedKnife", AccountManager.GetWeaponSelected(WeaponType.Knife));
        jsonObject.Add("SelectedPistol", AccountManager.GetWeaponSelected(WeaponType.Pistol));
        jsonObject.Add("SelectedRifle", AccountManager.GetWeaponSelected(WeaponType.Rifle));
        firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString(), delegate (string result)
        {

        }, null);
    }

    public static void SetWeaponSkinSelected2(int weaponid, int id, Action<string> complete, Action<string> failed)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountToken;
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("Skin", id);
        if (weaponid > 9)
        {
            firebase.Child("Players").Child("Accounts").Child(AccountParent)
                .Child(AccountID)
                .Child("Weapons")
                .Child(weaponid.ToString())
                .UpdateValue(jsonObject.ToString(), delegate
                {
                    AccountData data2 = instance.Data;
                    data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = ++data2.Session))))))))))))))))))))));
                }, null);
        }
        else
        {
            firebase.Child("Players").Child("Accounts").Child(AccountParent)
                .Child(AccountID)
                .Child("Weapons")
                .Child("0" + weaponid)
                .UpdateValue(jsonObject.ToString(), delegate
                {
                    AccountData data = instance.Data;
                    data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = ++data.Session))))))))))))))))))))));
                }, null);
        }
    }

    public static void SetClanFirebase(string clan, Action<string> complete, Action<string> failed)
    {
        Firebase obj = new Firebase
        {
            Auth = AccountToken
        };
        JsonObject jsonObject = new JsonObject();
        Clan.SendMessage(string.Concat(instance.Data.AccountName, " joined to the the clan"));
        jsonObject.Add("Clan", clan);
        obj.Child("Players").Child("Accounts").Child(AccountParent)
            .Child(AccountID)
            .UpdateValue(jsonObject.ToString(), delegate
            {
                AccountData data = instance.Data;
                data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = ++data.Session))))))))))))))))))))))));
            }, null);
    }

    public static void SetFirebaseWeaponsBuy(int id, Action<string> complete, Action<string> failed)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountToken;
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("Buy", 1);
        if (id < 10)
        {
            firebase.Child("Players").Child("Accounts").Child(AccountParent)
                .Child(AccountID)
                .Child("Weapons")
                .Child("0" + id)
                .UpdateValue(jsonObject.ToString(), delegate
                {
                    AccountData data2 = instance.Data;
                    data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = (data2.Session = ++data2.Session)))))))))))))))))))))))));
                }, null);
        }
        else
        {
            firebase.Child("Players").Child("Accounts").Child(AccountParent)
                .Child(AccountID)
                .Child("Weapons")
                .Child(id.ToString())
                .UpdateValue(jsonObject.ToString(), delegate
                {
                    AccountData data = instance.Data;
                    data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = ++data.Session)))))))))))))))))))))))));
                }, null);
        }
        mPopUp.SetActiveWaitPanel(false);
    }

    public static string baseSkins;

    public static void DeserializeWeaponSkins(string text)
    {
        AccountManager.baseSkins = JsonObject.Parse(text).Get<string>("Skins");
    }

    // Token: 0x06001F76 RID: 8054 RVA: 0x000AC094 File Offset: 0x000AA294
    public static void SetWeaponSkin2(bool secretWeapon, int weaponid, int skinid, Action<string> complete, Action<string> failed)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        if (weaponid > 9)
        {
            new Firebase
            {
                Auth = AccountManager.AccountToken
            }.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(weaponid.ToString()).GetValue(delegate (string result)
            {
                if (result == "null")
                {
                    return;
                }
                AccountManager.DeserializeWeaponSkins(result);
            }, delegate (string error)
            {
            });
        }
        else
        {
            new Firebase
            {
                Auth = AccountManager.AccountToken
            }.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(0 + weaponid.ToString()).GetValue(delegate (string result)
            {
                if (result == "null")
                {
                    return;
                }
                AccountManager.DeserializeWeaponSkins(result);
            }, delegate (string error)
            {
            });
        }
        TimerManager.In(2f, delegate ()
        {
            if (secretWeapon)
            {
                jsonObject.Add("Buy", 1);
            }
            if (AccountManager.baseSkins != null && !(AccountManager.baseSkins == ""))
            {
                jsonObject.Add("Skins", AccountManager.baseSkins + "," + skinid);
            }
            else
            {
                jsonObject.Add("Skins", skinid);
            }
            if (weaponid > 9)
            {
                firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(weaponid.ToString()).UpdateValue(jsonObject.ToString(), delegate (string result)
                {
                }, null);
                return;
            }
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(0 + weaponid.ToString()).UpdateValue(jsonObject.ToString(), delegate (string result)
            {
            }, null);
        });
    }

    public static void UpdateAccount()
    {
        AccountData oldData = AccountConvert.DeserializeOld(instance.OldData);
        Firebase firebase = new Firebase();
        firebase.Auth = AccountToken;
        firebase.Child("Players").Child("Accounts").Child(AccountParent)
            .Child(AccountID)
            .Child("LevelXP")
            .Delete();
        firebase.Child("Players").Child("Accounts").Child(AccountParent)
            .Child(AccountID)
            .Child("PlayerLevel")
            .Delete();
        TimerManager.In(1f, delegate
        {
            string json3 = AccountConvert.Serialize(oldData, false);
            firebase = new Firebase();
            firebase.Auth = AccountToken;
            firebase.Child("Players").Child("Accounts").Child(AccountParent)
                .Child(AccountID)
                .UpdateValue(json3, delegate (string result)
                {
                    instance.DefaultData = AccountConvert.Deserialize(result);
                    instance.Data = AccountConvert.Deserialize(result);
                    isConnect = true;
                    UpdateLastLogin();
                    UpdateSession();
                    mPopUp.HideAll("Menu");
                    mPopUp.SetActiveWait(false);
                    EventManager.Dispatch("AccountUpdate");
                    WeaponManager.UpdateData();
                }, delegate
                {
                    UpdateAccount();
                    isConnect = false;
                });
        });
    }

    public static void UpdateWeaponsData(Action<bool> complete, Action<string, string> failed)
    {
        JsonObject json3 = AccountConvert.CompareWeaponValue(instance.DefaultData, instance.Data);
        if (json3.Length == 0)
        {
            return;
        }
        if (complete != null)
        {
            complete(false);
        }
        CheckSession(delegate (bool session)
        {
            if (session && isConnect)
            {
                AccountData data = AccountConvert.Copy(instance.Data);
                Firebase firebase = new Firebase();
                firebase.Auth = AccountToken;
                firebase.Child("Players").Child("Accounts").Child(AccountParent)
                    .Child(AccountID)
                    .Child("Weapons")
                    .UpdateValue(json3.Get<string>("Weapons"), delegate
                    {
                        AccountConvert.CopyWeaponsValue(data, instance.DefaultData);
                        if (complete != null)
                        {
                            complete(true);
                        }
                    }, delegate (string error, string json2)
                    {
                        if (failed != null)
                        {
                            failed(error, json2);
                        }
                    });
            }
        });
    }

    public static void SetClan(string text)
    {
        instance.Data.Clan = text;
    }

    public static void SetWeapon(int id)
    {
        SetWeapon(id, false, null, null);
    }

    public static void SetWeapon(int id, bool update)
    {
        SetWeapon(id, update, null, null);
    }

    public static void SetWeapon(int id, bool update, Action<bool> complete, Action<string, string> failed)
    {
        for (int i = 0; i < instance.Data.Weapons.Count; i++)
        {
            if ((int)instance.Data.Weapons[i].ID == id)
            {
                instance.Data.Weapons[i].Buy = true;
                if (update)
                {
                    UpdateWeaponsData(complete, failed);
                }
                break;
            }
        }
    }

    public static void UpdateMoney(int money, Action<string> complete, Action<string> failed)
    {
        Firebase obj = new Firebase
        {
            Auth = AccountToken
        };
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("zmoney", money);
        obj.Child("Players").Child("Accounts").Child(AccountParent)
            .Child(AccountID)
            .UpdateValue(jsonObject.ToString(), delegate
            {
                AccountData data = instance.Data;
                data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = ++data.Session))))))))))))))))))));
            }, null);
    }

    public static void SetMoney(int money)
    {
        SetMoney(money, false, null, null);
    }

    public static void SetMoney(int money, bool update)
    {
        SetMoney(money, update, null, null);
    }

    public static void SetMoney(int money, bool update, Action<bool> complete, Action<string, string> failed)
    {
        instance.Data.Money = Mathf.Min(money, 100000000);
        if (update)
        {
            UpdateDefaultData(complete, failed);
        }
    }

    public static void SetMoney1(int money)
    {
        SetMoney1(money, false, null, null);
    }

    public static void SetMoney1(int money, bool update)
    {
        SetMoney1(money, update, null, null);
    }

    public static void SetMoney1(int money, bool update, Action<bool> complete, Action<string, string> failed)
    {
        SetMoney(GetMoney() + money, update, complete, failed);
    }

    public static void UpdatePlayerRoundData(int xp, int kills, int money, int deaths, int headshots, Action<string> complete, Action<string> failed)
    {
        Firebase obj = new Firebase
        {
            Auth = AccountToken
        };
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("xp", xp);
        jsonObject.Add("kills", kills);
        jsonObject.Add("deaths", deaths);
        jsonObject.Add("headshots", headshots);
        jsonObject.Add("zmoney", money);
        obj.Child("Players").Child("Accounts").Child(AccountParent)
            .Child(AccountID)
            .UpdateValue(jsonObject.ToString(), delegate
            {
                AccountData data = instance.Data;
                data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = ++data.Session)))))))))))))))))))));
            }, null);
    }

    public static void UpdateLevel(int lvl, Action<string> complete, Action<string> failed)
    {
        Firebase obj = new Firebase
        {
            Auth = AccountToken
        };
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("level", lvl);
        obj.Child("Players").Child("Accounts").Child(AccountParent)
            .Child(AccountID)
            .UpdateValue(jsonObject.ToString(), delegate
            {
                AccountData data = instance.Data;
                data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = (data.Session = ++data.Session)))))));
            }, null);
    }
    public static void AddAccountStickerFirebase(int id)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add(id.ToString(), 1);
        firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Stickers").UpdateValue(jsonObject.ToString());
    }
    public static void AddWeaponStickerFirebase(int weaponid, int skinid, int stickerid, int position)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add(0 + position.ToString(), stickerid);
        if (weaponid > 9)
        {
            if (skinid > 9)
            {
                firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(weaponid.ToString()).Child("Stickers").Child(skinid.ToString()).UpdateValue(jsonObject.ToString());
                return;
            }
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(weaponid.ToString()).Child("Stickers").Child(0 + skinid.ToString()).UpdateValue(jsonObject.ToString());
            return;
        }
        else
        {
            if (skinid > 9)
            {
                firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(0 + weaponid.ToString()).Child("Stickers").Child(skinid.ToString()).UpdateValue(jsonObject.ToString());
                return;
            }
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(0 + weaponid.ToString()).Child("Stickers").Child(0 + skinid.ToString()).UpdateValue(jsonObject.ToString());
            return;
        }
    }
    public static void DeleteAccountStickerFirebase(int id)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        new JsonObject();
        firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Stickers").Child(id.ToString()).Delete();
    }

    public static void DeleteWeaponStickerFirebase(int weaponid, int skinid, int position)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        new JsonObject();
        if (weaponid > 9)
        {
            if (skinid > 9)
            {
                firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(weaponid.ToString()).Child("Stickers").Child(skinid.ToString()).Child(0 + position.ToString()).Delete();
                return;
            }
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(weaponid.ToString()).Child("Stickers").Child(0 + skinid.ToString()).Child(0 + position.ToString()).Delete();
            return;
        }
        else
        {
            if (skinid > 9)
            {
                firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(0 + weaponid.ToString()).Child("Stickers").Child(skinid.ToString()).Child(0 + position.ToString()).Delete();
                return;
            }
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(0 + weaponid.ToString()).Child("Stickers").Child(0 + skinid.ToString()).Child(0 + position.ToString()).Delete();
            return;
        }
    }

    public static void BuyPlayerSkinFirebase(int id, BodyParts part)
    {
        AccountManager.SetPlayerSkinSelected(id, part);
        AccountManager.SetPlayerSkinSelected2(null, null);
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        if (part == BodyParts.Head)
        {
            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("PlayerSkinHead", AccountManager.instance.Data.PlayerSkin.Head);
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString());
        }
        if (part == BodyParts.Body)
        {
            JsonObject jsonObject2 = new JsonObject();
            jsonObject2.Add("PlayerSkinBody", AccountManager.instance.Data.PlayerSkin.Body);
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject2.ToString());
        }
        if (part == BodyParts.Legs)
        {
            JsonObject jsonObject3 = new JsonObject();
            jsonObject3.Add("PlayerSkinLegs", AccountManager.instance.Data.PlayerSkin.Legs);
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject3.ToString());
        }
    }

    public static void SetPlayerSkin(int id, BodyParts part)
    {
        if (!AccountManager.GetPlayerSkin(id, part))
        {
            switch (part)
            {
                case BodyParts.Head:
                    AccountManager.instance.Data.PlayerSkin.Head.Add(id);
                    return;
                case BodyParts.Body:
                    AccountManager.instance.Data.PlayerSkin.Body.Add(id);
                    return;
                case BodyParts.Legs:
                    AccountManager.instance.Data.PlayerSkin.Legs.Add(id);
                    break;
                default:
                    return;
            }
        }
    }
}
