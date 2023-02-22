using UnityEngine;

public class BadWordsManager
{
	private static string[] words = new string[0];

	public static void Init()
	{
		if (words.Length == 0)
		{
			TextAsset textAsset = (TextAsset)Resources.Load("Others/BadWords", typeof(TextAsset));
			words = textAsset.text.Split("\n"[0]);
		}
	}

	public static string Check(string text)
	{
		if (words.Length == 0)
		{
			Init();
		}
		bool flag = false;
		string text2 = text.ToLower();
		for (int i = 0; i < words.Length; i++)
		{
			if (text2.Contains(words[i]))
			{
				flag = true;
				text2 = text2.Replace(words[i], BlockWord(words[i].Length));
			}
		}
		return (!flag) ? text : text2;
	}

	public static bool Contains(string text)
	{
		if (words.Length == 0)
		{
			Init();
		}
		text = text.ToLower();
		for (int i = 0; i < words.Length; i++)
		{
			if (text.Contains(words[i]))
			{
				return true;
			}
		}
		return false;
	}

	private static string BlockWord(int length)
	{
		string text = string.Empty;
		for (int i = 0; i < length; i++)
		{
			text += "*";
		}
		return text;
	}
}
