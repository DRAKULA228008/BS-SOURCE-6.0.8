using UnityEngine;

public class UISettings : MonoBehaviour
{
	[Header("General")]
	public UILabel fontLabel;

	private int selectFont;

	public UILabel languageLabel;

	private static int selectLanguage;

	[Header("Control")]
	public UISlider Sensitivity;

	public UILabel SensitivityLabel;

	public UISlider ButtonAlpha;

	public UILabel ButtonAlphaLabel;

	public UIToggle ShiftButton;

	[Header("Sound")]
	public UISlider Volume;

	public UILabel VolumeLabel;

	public UIToggle Sound;

	public UIToggle AmbientSound;

	[Header("Others")]
	public UIToggle FPSMeter;

	public UIToggle Chat;

	public UIToggle Console;

	public UIToggle ShowDamage;

	public UIToggle BulletHole;

	public UIToggle Blood;

	public UIToggle HitMarker;

	public UIToggle HUD;

	public UIToggle ShowWeapon;

	public UIToggle Shell;

	public UIToggle ProjectileEffect;

	public UIToggle FilterChat;

	public UIToggle ShowFirestat;

	public UIToggle ShowStickers;

	public UIToggle ShowAvatars;

	public UIToggle Clouds;

	public UITable table;

	private void Start()
	{
		Load();
	}

	private void Load()
	{
		Settings.Load();
		UpdateLanguage();
		selectFont = Settings.Font;
		fontLabel.text = Localization.Get("Font") + ": " + UIFontManager.GetFonts()[selectFont].name.Split("-"[0])[1];
		languageLabel.text = Localization.Get("Language") + ": " + Localization.Get("LanguageType");
		FPSMeter.value = Settings.FPSMeter;
		Chat.value = Settings.Chat;
		Console.value = Settings.Console;
		UpdateConsole();
		ShowDamage.value = Settings.ShowDamage;
		BulletHole.value = Settings.BulletHole;
		Blood.value = Settings.Blood;
		HitMarker.value = Settings.HitMarker;
		HUD.value = Settings.HUD;
		ShowWeapon.value = Settings.ShowWeapon;
		Shell.value = Settings.Shell;
		ProjectileEffect.value = Settings.ProjectileEffect;
		FilterChat.value = Settings.FilterChat;
		ShowFirestat.value = Settings.ShowFirestat;
		ShowStickers.value = Settings.ShowStickers;
		ShowAvatars.value = Settings.ShowAvatars;
		Clouds.value = Settings.Clouds;
		UpdateSensitivity();
		UpdateButtonAlpha();
		ShiftButton.value = Settings.ShiftButton;
		UpdateVolume();
		Sound.value = Settings.Sound;
		AmbientSound.value = Settings.AmbientSound;
	}

	public void Save()
	{
		Settings.Font = selectFont;
		Settings.FPSMeter = FPSMeter.value;
		Settings.Chat = Chat.value;
		Settings.Console = Console.value;
		UpdateConsole();
		Settings.ShowDamage = ShowDamage.value;
		Settings.BulletHole = BulletHole.value;
		Settings.Blood = Blood.value;
		Settings.HitMarker = HitMarker.value;
		Settings.HUD = HUD.value;
		Settings.ShowWeapon = ShowWeapon.value;
		Settings.Shell = Shell.value;
		Settings.ProjectileEffect = ProjectileEffect.value;
		Settings.FilterChat = FilterChat.value;
		Settings.ShowFirestat = ShowFirestat.value;
		Settings.ShowStickers = ShowStickers.value;
		Settings.ShowAvatars = ShowAvatars.value;
		Settings.Clouds = Clouds.value;
		Settings.ShiftButton = ShiftButton.value;
		Settings.Sound = Sound.value;
		Settings.AmbientSound = AmbientSound.value;
		Settings.Save();
		EventManager.Dispatch("OnSettings");
	}

	public void Default()
	{
		Settings.Font = 0;
		Settings.FPSMeter = false;
		Settings.Chat = true;
		Settings.Console = false;
		UpdateConsole();
		Settings.ShowDamage = false;
		Settings.BulletHole = true;
		Settings.Blood = true;
		Settings.HitMarker = true;
		Settings.HUD = true;
		Settings.ShowWeapon = true;
		Settings.FilterChat = true;
		Settings.Shell = true;
		Settings.ProjectileEffect = true;
		Settings.ShowFirestat = true;
		Settings.ShowStickers = true;
		Settings.ShowAvatars = true;
		Settings.Clouds = true;
		Settings.Sensitivity = 0.2f;
		Settings.ButtonAlpha = 1f;
		Settings.DynamicJoystick = true;
		Settings.ShiftButton = false;
		Settings.Volume = 0.8f;
		Settings.Sound = true;
		Settings.AmbientSound = true;
		Settings.Save();
		Load();
		EventManager.Dispatch("OnSettings");
	}

	public static void UpdateLanguage()
	{
		if (PlayerPrefs.HasKey("Language"))
		{
			SetLanguage(PlayerPrefs.GetString("Language"));
		}
		else if (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian || Application.systemLanguage == SystemLanguage.Belarusian)
		{
			SetLanguage("Russia");
		}
		else if (Application.systemLanguage == SystemLanguage.English)
		{
			SetLanguage("English");
		}
		else if (Application.systemLanguage == SystemLanguage.Korean)
		{
			SetLanguage("Korean");
		}
		else if (Application.systemLanguage == SystemLanguage.Spanish)
		{
			SetLanguage("Spanish");
		}
		else if (Application.systemLanguage == SystemLanguage.Portuguese)
		{
			SetLanguage("Portuguese");
		}
		else if (Application.systemLanguage == SystemLanguage.French)
		{
			SetLanguage("French");
		}
		else if (Application.systemLanguage == SystemLanguage.Japanese)
		{
			SetLanguage("Japan");
		}
		else if (Application.systemLanguage == SystemLanguage.Polish)
		{
			SetLanguage("Polish");
		}
	}

	private static void SetLanguage(string language)
	{
		for (int i = 0; i < Localization.knownLanguages.Length; i++)
		{
			if (Localization.knownLanguages[i] == language)
			{
				Localization.language = Localization.knownLanguages[i];
				selectLanguage = i;
				break;
			}
		}
	}

	public void NextLanguage()
	{
		selectLanguage++;
		if (selectLanguage > Localization.knownLanguages.Length - 1)
		{
			selectLanguage = 0;
		}
		SetLanguage(Localization.knownLanguages[selectLanguage]);
		table.repositionNow = true;
		languageLabel.text = Localization.Get("Language") + ": " + Localization.Get("LanguageType");
	}

	public void LastLanguage()
	{
		selectLanguage--;
		if (selectLanguage < 0)
		{
			selectLanguage = Localization.knownLanguages.Length - 1;
		}
		SetLanguage(Localization.knownLanguages[selectLanguage]);
		table.repositionNow = true;
		languageLabel.text = Localization.Get("Language") + ": " + Localization.Get("LanguageType");
	}

	public void NextFont()
	{
		selectFont++;
		if (selectFont > UIFontManager.GetFonts().Length - 1)
		{
			selectFont = 0;
		}
		UIFontManager.SetFont(selectFont);
		fontLabel.text = Localization.Get("Font") + ": " + UIFontManager.GetFonts()[selectFont].name.Split("-"[0])[1];
	}

	public void LastFont()
	{
		selectFont--;
		if (selectFont < 0)
		{
			selectFont = UIFontManager.GetFonts().Length - 1;
		}
		UIFontManager.SetFont(selectFont);
		fontLabel.text = Localization.Get("Font") + ": " + UIFontManager.GetFonts()[selectFont].name.Split("-"[0])[1];
	}

	private void UpdateConsole()
	{
		GameConsole.actived = Settings.Console;
	}

	private void UpdateSensitivity()
	{
		Sensitivity.value = Settings.Sensitivity;
		SensitivityLabel.text = Localization.Get("Sensitivity") + ": " + Mathf.RoundToInt(Sensitivity.value * 100f) + "%";
	}

	public void SetSensitivity()
	{
		Settings.Sensitivity = Sensitivity.value;
		SensitivityLabel.text = Localization.Get("Sensitivity") + ": " + Mathf.RoundToInt(Sensitivity.value * 100f) + "%";
	}

	private void UpdateButtonAlpha()
	{
		ButtonAlpha.value = Settings.ButtonAlpha;
		ButtonAlphaLabel.text = Localization.Get("Button Alpha") + ": " + Mathf.RoundToInt(ButtonAlpha.value * 100f) + "%";
	}

	public void SetButtonAlpha()
	{
		Settings.ButtonAlpha = Mathf.Clamp(ButtonAlpha.value, 0.01f, 1f);
		ButtonAlphaLabel.text = Localization.Get("Button Alpha") + ": " + Mathf.RoundToInt(ButtonAlpha.value * 100f) + "%";
	}

	private void UpdateVolume()
	{
		Volume.value = Settings.Volume;
		VolumeLabel.text = Localization.Get("Volume") + ": " + Mathf.RoundToInt(Volume.value * 100f) + "%";
	}

	public void SetVolume()
	{
		Settings.Volume = Volume.value;
		VolumeLabel.text = Localization.Get("Volume") + ": " + Mathf.RoundToInt(Volume.value * 100f) + "%";
	}

	public void Optimization(UIToggle toggle)
	{
		if (toggle.value)
		{
			UIToast.Show(Localization.Get("Optimization") + " +");
		}
	}

	public void OnDefaultButton()
	{
		EventManager.Dispatch("OnDefaultButton");
	}

	public void OnSaveButton()
	{
		EventManager.Dispatch("OnSaveButton");
	}
}
