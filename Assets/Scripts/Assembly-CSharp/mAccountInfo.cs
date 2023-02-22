using System.Text;
using UnityEngine;

public class mAccountInfo : MonoBehaviour
{
	public static void Convert(AccountData data)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(Localization.Get("Name") + ": " + data.AccountName);
		stringBuilder.AppendLine("==========================================================");
		stringBuilder.AppendLine(Localization.Get("Money") + ": " + data.Money);
		stringBuilder.AppendLine(Localization.Get("Gold") + ": " + data.Gold);
		stringBuilder.AppendLine("==========================================================");
		stringBuilder.AppendLine(Localization.Get("XP") + ": " + data.XP);
		stringBuilder.AppendLine(Localization.Get("Level") + ": " + data.Level);
		stringBuilder.AppendLine("==========================================================");
		stringBuilder.AppendLine(Localization.Get("Time in the game") + ": " + data.Time);
		stringBuilder.AppendLine("==========================================================");
		stringBuilder.AppendLine(Localization.Get("Deaths") + ": " + data.Deaths);
		stringBuilder.AppendLine(Localization.Get("Kills") + ": " + data.Kills);
		stringBuilder.AppendLine(Localization.Get("Headshot") + ": " + data.Headshot);
		stringBuilder.AppendLine("==========================================================");
		stringBuilder.AppendLine(Localization.Get("Weapon"));
		stringBuilder.AppendLine(Localization.Get("Main Weapon") + ": " + WeaponManager.GetWeaponName(data.SelectedRifle));
		stringBuilder.AppendLine(Localization.Get("Secondary weapon") + ": " + WeaponManager.GetWeaponName(data.SelectedPistol));
		stringBuilder.AppendLine(Localization.Get("Melee Weapon") + ": " + WeaponManager.GetWeaponName(data.SelectedKnife));
		stringBuilder.AppendLine("==========================================================");
		stringBuilder.AppendLine(Localization.Get("Player Skin"));
		stringBuilder.Append(Localization.Get("Head") + ": " + ((int)data.PlayerSkin.Select[0] + 1));
		if (data.PlayerSkin.Head.Count > 0)
		{
			stringBuilder.Append(" [");
			for (int i = 0; i < data.PlayerSkin.Head.Count; i++)
			{
				stringBuilder.Append((int)data.PlayerSkin.Head[i] + 1 + ((i >= data.PlayerSkin.Head.Count - 1) ? string.Empty : ","));
			}
			stringBuilder.AppendLine("]");
		}
		stringBuilder.Append(Localization.Get("Body") + ": " + ((int)data.PlayerSkin.Select[1] + 1));
		if (data.PlayerSkin.Body.Count > 0)
		{
			stringBuilder.Append(" [");
			for (int j = 0; j < data.PlayerSkin.Body.Count; j++)
			{
				stringBuilder.Append((int)data.PlayerSkin.Body[j] + 1 + ((j >= data.PlayerSkin.Body.Count - 1) ? string.Empty : ","));
			}
			stringBuilder.AppendLine("]");
		}
		stringBuilder.Append(Localization.Get("Legs") + ": " + ((int)data.PlayerSkin.Select[2] + 1));
		if (data.PlayerSkin.Legs.Count > 0)
		{
			stringBuilder.Append(" [");
			for (int k = 0; k < data.PlayerSkin.Legs.Count; k++)
			{
				stringBuilder.Append((int)data.PlayerSkin.Legs[k] + 1 + ((k >= data.PlayerSkin.Legs.Count - 1) ? string.Empty : ","));
			}
			stringBuilder.AppendLine("]");
		}
		stringBuilder.AppendLine("==========================================================");
		if (data.Stickers.Count > 0)
		{
			stringBuilder.AppendLine(Localization.Get("Stickers"));
			for (int l = 0; l < data.Stickers.Count; l++)
			{
				stringBuilder.AppendLine(WeaponManager.GetStickerName(data.Stickers[l].ID) + ": " + data.Stickers[l].Count);
			}
			stringBuilder.AppendLine("==========================================================");
		}
		stringBuilder.AppendLine(Localization.Get("Weapons"));
		for (int m = 0; m < data.Weapons.Count; m++)
		{
			stringBuilder.AppendLine("////////////////");
			stringBuilder.AppendLine(WeaponManager.GetWeaponName(data.Weapons[m].ID) + ((!data.Weapons[m].Buy) ? " -" : " +"));
			for (int num = 0; num < data.Weapons[m].Skins.Count; m++)
			{
				WeaponSkinData weaponSkin = WeaponManager.GetWeaponSkin(data.Weapons[m].ID, data.Weapons[m].Skins[num]);
				stringBuilder.AppendLine(weaponSkin.Name + (((int)data.Weapons[m].FireStats[num] == -1) ? string.Empty : string.Concat(" [", data.Weapons[m].FireStats[num], "]")));
			}
			stringBuilder.AppendLine("////////////////");
		}
		MonoBehaviour.print(stringBuilder.ToString());
	}
}
