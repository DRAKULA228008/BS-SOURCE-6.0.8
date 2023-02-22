using System.Collections.Generic;
using UnityEngine;

public class UIDeathScreen : MonoBehaviour
{
	public UIPanel Panel;

	public UILabel PlayerLabel;

	public UILabel WeaponLabel;

	public UILabel DamageLabel;

	public UITexture HeadshotTexture;

	public UISprite WeaponSprite;

	public UITexture AvatarTexture;

	public float sizeWeapon = 1f;

	private Dictionary<int, int> kills = new Dictionary<int, int>();

	private Dictionary<int, int> deaths = new Dictionary<int, int>();

	private Dictionary<int, int> takenDamages = new Dictionary<int, int>();

	private Dictionary<int, int> takenHits = new Dictionary<int, int>();

	private Dictionary<int, int> givenDamages = new Dictionary<int, int>();

	private Dictionary<int, int> givenHits = new Dictionary<int, int>();

	private int Timer;

	private static UIDeathScreen instance;

	private void Start()
	{
		instance = this;
	}

	public static void Show(DamageInfo damageInfo)
	{
		if (!damageInfo.otherPlayer || damageInfo.weapon == 46)
		{
			return;
		}
		AddDeath(damageInfo.player);
		TimerManager.Cancel(instance.Timer);
		instance.Panel.cachedGameObject.SetActive(true);
		instance.Panel.alpha = 0f;
		TweenAlpha.Begin(instance.Panel.cachedGameObject, 0.2f, 1f);
		PhotonPlayer photonPlayer = PhotonPlayer.Find(damageInfo.player);
		instance.PlayerLabel.text = photonPlayer.UserId;
		instance.HeadshotTexture.cachedGameObject.SetActive(damageInfo.headshot);
		instance.SetWeaponData(damageInfo);
		int num = 0;
		int number = 0;
		int num2 = 0;
		int number2 = 0;
		if (instance.takenDamages.ContainsKey(damageInfo.player))
		{
			num = instance.takenDamages[damageInfo.player];
			number = instance.takenHits[damageInfo.player];
		}
		if (instance.givenDamages.ContainsKey(damageInfo.player))
		{
			num2 = instance.givenDamages[damageInfo.player];
			number2 = instance.givenHits[damageInfo.player];
		}
		instance.DamageLabel.text = num + " " + Localization.Get("Damage taken").ToLower() + " | " + StringCache.Get(number) + " " + Localization.Get("Hits").ToLower() + "\n" + num2 + " " + Localization.Get("Damage given").ToLower() + " | " + StringCache.Get(number2) + " " + Localization.Get("Hits").ToLower();
		if (Settings.ShowAvatars)
		{
			instance.AvatarTexture.mainTexture = AvatarManager.Get(photonPlayer.GetAvatarUrl());
		}
		else
		{
			instance.AvatarTexture.mainTexture = GameSettings.instance.NoAvatarTexture;
		}
		instance.Timer = TimerManager.In(3f, delegate
		{
			TweenAlpha.Begin(instance.Panel.cachedGameObject, 0.2f, 0f);
			TimerManager.In(0.2f, delegate
			{
				instance.Panel.cachedGameObject.SetActive(false);
			});
		});
	}

	public static void AddKill(int playerID)
	{
		if (instance.kills.ContainsKey(playerID))
		{
			Dictionary<int, int> dictionary;
			Dictionary<int, int> dictionary2 = (dictionary = instance.kills);
			int key;
			int key2 = (key = playerID);
			key = dictionary[key];
			dictionary2[key2] = key + 1;
		}
		else
		{
			instance.kills.Add(playerID, 1);
		}
	}

	public static void AddTakenDamage(int playerID, int damage)
	{
		if (instance.takenDamages.ContainsKey(playerID))
		{
			Dictionary<int, int> dictionary;
			Dictionary<int, int> dictionary2 = (dictionary = instance.takenDamages);
			int key;
			int key2 = (key = playerID);
			key = dictionary[key];
			dictionary2[key2] = key + damage;
			Dictionary<int, int> dictionary3;
			Dictionary<int, int> dictionary4 = (dictionary3 = instance.takenHits);
			int key3 = (key = playerID);
			key = dictionary3[key];
			dictionary4[key3] = key + 1;
		}
		else
		{
			instance.takenDamages.Add(playerID, damage);
			instance.takenHits.Add(playerID, 1);
		}
	}

	public static void AddGivenDamage(int playerID, int damage)
	{
		if (instance.givenDamages.ContainsKey(playerID))
		{
			Dictionary<int, int> dictionary;
			Dictionary<int, int> dictionary2 = (dictionary = instance.givenDamages);
			int key;
			int key2 = (key = playerID);
			key = dictionary[key];
			dictionary2[key2] = key + damage;
			Dictionary<int, int> dictionary3;
			Dictionary<int, int> dictionary4 = (dictionary3 = instance.givenHits);
			int key3 = (key = playerID);
			key = dictionary3[key];
			dictionary4[key3] = key + 1;
		}
		else
		{
			instance.givenDamages.Add(playerID, damage);
			instance.givenHits.Add(playerID, 1);
		}
	}

	public static void ClearTakenDamage()
	{
		instance.takenDamages.Clear();
		instance.takenHits.Clear();
	}

	public static void ClearGivenDamage()
	{
		instance.givenDamages.Clear();
		instance.givenHits.Clear();
	}

	public static void ClearGivenDamage(int playerID)
	{
		if (instance.givenDamages.ContainsKey(playerID))
		{
			instance.givenDamages[playerID] = 0;
			instance.givenHits[playerID] = 0;
		}
	}

	public static void ClearAll()
	{
		ClearTakenDamage();
		ClearGivenDamage();
	}

	private static void AddDeath(int playerID)
	{
		if (instance.deaths.ContainsKey(playerID))
		{
			Dictionary<int, int> dictionary;
			Dictionary<int, int> dictionary2 = (dictionary = instance.deaths);
			int key;
			int key2 = (key = playerID);
			key = dictionary[key];
			dictionary2[key2] = key + 1;
		}
		else
		{
			instance.deaths.Add(playerID, 1);
		}
	}

	private void SetWeaponData(DamageInfo damageInfo)
	{
		WeaponData weaponData = WeaponManager.GetWeaponData(damageInfo.weapon);
		WeaponSkinData weaponSkin = WeaponManager.GetWeaponSkin(damageInfo.weapon, damageInfo.weaponSkin);
		WeaponLabel.text = string.Concat(weaponData.Name, " | ", weaponSkin.Name);
		WeaponLabel.color = GetWeaponSkinQualityColor(weaponSkin.Quality);
		WeaponSprite.spriteName = damageInfo.weapon + "-" + damageInfo.weaponSkin;
		WeaponSprite.width = (int)(GameSettings.instance.WeaponsCaseSize[(int)weaponData.ID - 1].x * sizeWeapon);
		WeaponSprite.height = (int)(GameSettings.instance.WeaponsCaseSize[(int)weaponData.ID - 1].y * sizeWeapon);
	}

	private Color GetWeaponSkinQualityColor(WeaponSkinQuality quality)
	{
		switch (quality)
		{
		case WeaponSkinQuality.Default:
		case WeaponSkinQuality.Normal:
			return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		case WeaponSkinQuality.Basic:
			return new Color32(54, 189, byte.MaxValue, byte.MaxValue);
		case WeaponSkinQuality.Professional:
			return new Color32(byte.MaxValue, 0, 0, byte.MaxValue);
		case WeaponSkinQuality.Legendary:
			return new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);
		default:
			return Color.white;
		}
	}
}
