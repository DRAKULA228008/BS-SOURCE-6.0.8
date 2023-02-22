using UnityEngine;

public class UIFontManager : MonoBehaviour
{
	public UILabel[] labels;

	public UIPopupList[] popupLists;

	public Font[] fonts;

	private static UIFontManager instance;

	private void Start()
	{
		instance = this;
		SetFont(Settings.Font);
	}

	public static void SetFont(int index)
	{
		for (int i = 0; i < instance.labels.Length; i++)
		{
			instance.labels[i].trueTypeFont = instance.fonts[index];
		}
		for (int j = 0; j < instance.popupLists.Length; j++)
		{
			instance.popupLists[j].trueTypeFont = instance.fonts[index];
		}
	}

	public static Font[] GetFonts()
	{
		return instance.fonts;
	}
}
