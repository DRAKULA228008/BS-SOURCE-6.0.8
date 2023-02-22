using System;
using System.Globalization;
using UnityEngine;

public class mPlayerStats : MonoBehaviour
{
	public UITexture avatarTexture;

	public UILabel NameLabel;

	public UILabel IDLabel;

	public UILabel LevelLabel;

	public UILabel XPLabel;

	public UILabel DeathsLabel;

	public UILabel KillsLabel;

	public UILabel HeadshotKillsLabel;

	public UILabel TimeLabel;

	public UILabel TotalSkinLabel;

	public UILabel LegendarySkinLabel;

	public UILabel ProfessionalSkinLabel;

	public UILabel BasicSkinLabel;

	public UILabel NormalSkinLabel;

	public UILabel MoneyLabel;

	public UILabel GoldLabel;

	public GameObject Panel;

	public void Open()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		Panel.SetActive(true);
        #if UNITY_EDITOR
        avatarTexture.mainTexture = GameSettings.instance.DeveloperAvatar;
        #else
        avatarTexture.mainTexture = AccountManager.instance.Data.Avatar;
        #endif
        NameLabel.text = AccountManager.instance.Data.AccountName;
		IDLabel.text = "ID: " + AccountManager.instance.Data.ID.ToString();
		LevelLabel.text = AccountManager.GetLevel().ToString();
		XPLabel.text = AccountManager.GetXP() + "/" + AccountManager.GetMaxXP();
		DeathsLabel.text = AccountManager.GetDeaths().ToString();
		KillsLabel.text = AccountManager.GetKills().ToString();
		HeadshotKillsLabel.text = AccountManager.GetHeadshot().ToString();
		TimeLabel.text = Localization.Get("Time in the game") + ": " + ConvertTime(AccountManager.instance.Data.Time);
		TotalSkinLabel.text = GetOpenSkins(WeaponSkinQuality.Default) + "/" + GetTotalSkins(WeaponSkinQuality.Default);
		MoneyLabel.text = AccountManager.GetMoney().ToString("n0", new CultureInfo("en-us"));
		GoldLabel.text = AccountManager.GetGold().ToString("n0", new CultureInfo("en-us"));
        LegendarySkinLabel.text = GetOpenSkins(WeaponSkinQuality.Legendary) + "/" + GetTotalSkins(WeaponSkinQuality.Legendary);
		ProfessionalSkinLabel.text = GetOpenSkins(WeaponSkinQuality.Professional) + "/" + GetTotalSkins(WeaponSkinQuality.Professional);
		BasicSkinLabel.text = GetOpenSkins(WeaponSkinQuality.Basic) + "/" + GetTotalSkins(WeaponSkinQuality.Basic);
		NormalSkinLabel.text = GetOpenSkins(WeaponSkinQuality.Normal) + "/" + GetTotalSkins(WeaponSkinQuality.Normal);
	}

	private int GetOpenSkins(WeaponSkinQuality quality)
	{
		int num = 0;
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			for (int j = 1; j < GameSettings.instance.WeaponsStore[i].Skins.Count; j++)
			{
				if ((GameSettings.instance.WeaponsStore[i].Skins[j].Quality == quality || quality == WeaponSkinQuality.Default) && AccountManager.GetWeaponSkin(i + 1, j))
				{
					num++;
				}
			}
		}
		return num;
	}

	private int GetTotalSkins(WeaponSkinQuality quality)
	{
		int num = 0;
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			for (int j = 1; j < GameSettings.instance.WeaponsStore[i].Skins.Count; j++)
			{
				if (GameSettings.instance.WeaponsStore[i].Skins[j].Quality == quality || quality == WeaponSkinQuality.Default)
				{
					num++;
				}
			}
		}
		return num;
	}

	private string ConvertTime(long time)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(time);
		if (timeSpan.Days * 24 + timeSpan.Hours > 0)
		{
			return timeSpan.Days * 24 + timeSpan.Hours + " " + Localization.Get("h") + ".";
		}
		if (timeSpan.Minutes > 0)
		{
			return timeSpan.Minutes + " " + Localization.Get("m") + ".";
		}
		return timeSpan.Seconds + " " + Localization.Get("s") + ".";
	}
}
