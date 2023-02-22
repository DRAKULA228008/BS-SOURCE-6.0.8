using System;
using UnityEngine;

public class mLanguage : MonoBehaviour
{
	[Serializable]
	public class LanguageClass
	{
		public string language;

		public Texture2D Texture;
	}

	public LanguageClass[] languages;

	public UITexture Button;

	private int selectLanguage;

	private void Awake()
	{
		if (PlayerPrefs.HasKey("Language"))
		{
			SetLanguage(PlayerPrefs.GetString("Language"));
		}
		else if (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian)
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

	private void SetLanguage(string language)
	{
		for (int i = 0; i < languages.Length; i++)
		{
			if (languages[i].language == language)
			{
				Localization.language = languages[i].language;
				Button.mainTexture = languages[i].Texture;
				selectLanguage = i;
				break;
			}
		}
	}

	public void SelectLanguage()
	{
		LanguageClass languageClass = null;
		if (selectLanguage < languages.Length - 1)
		{
			selectLanguage++;
		}
		else
		{
			selectLanguage = 0;
		}
		languageClass = languages[selectLanguage];
		Localization.language = languageClass.language;
		Button.mainTexture = languageClass.Texture;
	}
}
