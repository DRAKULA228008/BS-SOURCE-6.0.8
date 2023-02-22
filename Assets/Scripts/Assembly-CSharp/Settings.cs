using UnityEngine;

public class Settings
{
	public static int Font;

	public static bool FPSMeter;

	public static bool Console;

	public static bool Chat = true;

	public static bool ShowDamage;

	public static bool BulletHole = true;

	public static bool Blood = true;

	public static bool HitMarker = true;

	public static int ColorCrosshair;

	public static float Sensitivity = 0.2f;

	public static float Volume = 0.8f;

	public static bool Sound = true;

	public static bool AmbientSound = true;

	public static float ButtonAlpha = 1f;

	public static bool HUD = true;

	public static bool ShowWeapon = true;

	public static bool Shell = true;

	public static bool ProjectileEffect = true;

	public static bool FilterChat = true;

	public static bool DynamicJoystick = true;

	public static bool ShiftButton;

	public static bool ShowFirestat = true;

	public static bool ShowStickers = true;

	public static bool ShowAvatars = true;

	public static bool Clouds = true;

	public static void Load()
	{
		Font = GetInt("Font", 0);
		FPSMeter = GetBool("FPSMeter", false);
		Console = GetBool("Console", false);
		Chat = GetBool("Chat", true);
		ShowDamage = GetBool("ShowDamage", false);
		BulletHole = GetBool("BulletHole", true);
		Blood = GetBool("Blood", true);
		HitMarker = GetBool("HitMarker", true);
		ColorCrosshair = GetInt("ColorCrosshair", 0);
		Sensitivity = GetFloat("Sensitivity", 0.2f);
		Volume = GetFloat("Volume", 0.8f);
		Sound = GetBool("Sound", true);
		AmbientSound = GetBool("AmbientSound", true);
		ButtonAlpha = Mathf.Clamp(GetFloat("ButtonAlpha", 1f), 0.01f, 1f);
		HUD = GetBool("HUD", true);
		ShowWeapon = GetBool("ShowWeapon", true);
		Shell = GetBool("Shell", true);
		ProjectileEffect = GetBool("ProjectileEffect", true);
		FilterChat = GetBool("FilterChat", true);
		DynamicJoystick = GetBool("DynamicJoystick", true);
		ShiftButton = GetBool("ShiftButton", false);
		ShowFirestat = GetBool("ShowFirestat", true);
		ShowStickers = GetBool("ShowStickers", true);
		ShowAvatars = GetBool("ShowAvatars", true);
		Clouds = GetBool("Clouds", true);
		AudioListener.volume = Volume;
	}

	public static void Save()
	{
		SetInt("Font", Font);
		SetBool("FPSMeter", FPSMeter);
		SetBool("Console", Console);
		SetBool("Chat", Chat);
		SetBool("ShowDamage", ShowDamage);
		SetBool("BulletHole", BulletHole);
		SetBool("Blood", Blood);
		SetBool("HitMarker", HitMarker);
		SetInt("ColorCrosshair", ColorCrosshair);
		SetFloat("Sensitivity", Sensitivity);
		SetFloat("Volume", Volume);
		SetBool("Sound", Sound);
		SetBool("AmbientSound", AmbientSound);
		SetFloat("ButtonAlpha", Mathf.Clamp(ButtonAlpha, 0.01f, 1f));
		SetBool("HUD", HUD);
		SetBool("ShowWeapon", ShowWeapon);
		SetBool("Shell", Shell);
		SetBool("ProjectileEffect", ProjectileEffect);
		SetBool("FilterChat", FilterChat);
		SetBool("DynamicJoystick", DynamicJoystick);
		SetBool("ShiftButton", ShiftButton);
		SetBool("ShowFirestat", ShowFirestat);
		SetBool("ShowStickers", ShowStickers);
		SetBool("ShowAvatars", ShowAvatars);
		SetBool("Clouds", Clouds);
		AudioListener.volume = Volume;
		//nPlayerPrefs.Save();
	}

	private static bool GetBool(string key, bool defaultValue)
	{
		return nPlayerPrefs.GetBool(key, defaultValue);
	}

	private static int GetInt(string key, int defaultValue)
	{
		return nPlayerPrefs.GetInt(key, defaultValue);
	}

	private static float GetFloat(string key, float defaultValue)
	{
		return nPlayerPrefs.GetFloat(key, defaultValue);
	}

	private static void SetBool(string key, bool value)
	{
		nPlayerPrefs.SetBool(key, value);
	}

	private static void SetInt(string key, int value)
	{
		nPlayerPrefs.SetInt(key, value);
	}

	private static void SetFloat(string key, float value)
	{
		nPlayerPrefs.SetFloat(key, value);
	}
}
