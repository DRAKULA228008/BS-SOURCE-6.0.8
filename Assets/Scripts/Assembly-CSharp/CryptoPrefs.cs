using System;
using System.Linq;
using System.Text;
using UnityEngine;

public static class CryptoPrefs
{
	private static byte[] defaultKey;

	private static byte[] S;

	private static int x;

	private static int y;

	static CryptoPrefs()
	{
		defaultKey = new byte[9] { 97, 50, 51, 106, 54, 103, 57, 52, 104 };
		x = 0;
		y = 0;
		string text = SystemInfo.deviceUniqueIdentifier;
		if (string.IsNullOrEmpty(text))
		{
			text = "3h56gk58n3g5f0d2";
		}
		else
		{
			if (text.Length < 16)
			{
				text += "3h56gk58n3g5f0d2";
			}
			text = text.Remove(16);
		}
		defaultKey = Encoding.UTF8.GetBytes(text);
		SetCryptoKey();
	}

	public static bool HasKey(string key)
	{
		return PlayerPrefs.HasKey(Convert.ToBase64String(EncryptDencrypt(Encoding.UTF8.GetBytes(key))));
	}

	public static void DeleteKey(string key)
	{
		PlayerPrefs.DeleteKey(Convert.ToBase64String(EncryptDencrypt(Encoding.UTF8.GetBytes(key))));
	}

	public static void SetInt(string key, int value)
	{
		key = Convert.ToBase64String(EncryptDencrypt(Encoding.UTF8.GetBytes(key)));
		string value2 = Convert.ToBase64String(EncryptDencrypt(Encoding.UTF8.GetBytes(key + "|" + value)));
		PlayerPrefs.SetString(key, value2);
	}

	public static int GetInt(string key)
	{
		return GetInt(key, 0);
	}

	public static int GetInt(string key, int defaultValue)
	{
		//Discarded unreachable code: IL_007b, IL_0088
		try
		{
			key = Convert.ToBase64String(EncryptDencrypt(Encoding.UTF8.GetBytes(key)));
			string @string = PlayerPrefs.GetString(key, defaultValue.ToString());
			if (@string == defaultValue.ToString())
			{
				return defaultValue;
			}
			@string = Encoding.UTF8.GetString(EncryptDencrypt(Convert.FromBase64String(@string)));
			return int.Parse(@string.Split("|"[0])[1]);
		}
		catch
		{
			return defaultValue;
		}
	}

	public static void SetFloat(string key, float value)
	{
		key = Convert.ToBase64String(EncryptDencrypt(Encoding.UTF8.GetBytes(key)));
		string value2 = Convert.ToBase64String(EncryptDencrypt(Encoding.UTF8.GetBytes(key + "|" + value)));
		PlayerPrefs.SetString(key, value2);
	}

	public static float GetFloat(string key)
	{
		return GetFloat(key, 0f);
	}

	public static float GetFloat(string key, float defaultValue)
	{
		//Discarded unreachable code: IL_007b, IL_0088
		try
		{
			key = Convert.ToBase64String(EncryptDencrypt(Encoding.UTF8.GetBytes(key)));
			string @string = PlayerPrefs.GetString(key, defaultValue.ToString());
			if (@string == defaultValue.ToString())
			{
				return defaultValue;
			}
			@string = Encoding.UTF8.GetString(EncryptDencrypt(Convert.FromBase64String(@string)));
			return float.Parse(@string.Split("|"[0])[1]);
		}
		catch
		{
			return defaultValue;
		}
	}

	public static void SetBool(string key, bool value)
	{
		int num = UnityEngine.Random.Range(0, 1000);
		if (value && num % 2 == 0)
		{
			num++;
		}
		else if (!value && num % 2 == 1)
		{
			num++;
		}
		SetInt(key, num);
	}

	public static bool GetBool(string key)
	{
		return GetBool(key, false);
	}

	public static bool GetBool(string key, bool defaultValue)
	{
		int @int = GetInt(key, defaultValue ? 1 : 0);
		return (@int % 2 == 1) ? true : false;
	}

	public static void SetString(string key, string value)
	{
		key = Convert.ToBase64String(EncryptDencrypt(Encoding.UTF8.GetBytes(key)));
		string value2 = Convert.ToBase64String(EncryptDencrypt(Encoding.UTF8.GetBytes(key + "|" + value)));
		PlayerPrefs.SetString(key, value2);
	}

	public static string GetString(string key)
	{
		return GetString(key, string.Empty);
	}

	public static string GetString(string key, string defaultValue)
	{
		//Discarded unreachable code: IL_0074, IL_0081
		try
		{
			key = Convert.ToBase64String(EncryptDencrypt(Encoding.UTF8.GetBytes(key)));
			string @string = PlayerPrefs.GetString(key, defaultValue.ToString());
			if (@string == defaultValue.ToString())
			{
				return defaultValue;
			}
			@string = Encoding.UTF8.GetString(EncryptDencrypt(Convert.FromBase64String(@string)));
			return @string.Split("|"[0])[1];
		}
		catch
		{
			return defaultValue;
		}
	}

	private static void SetCryptoKey()
	{
		S = new byte[256];
		for (int i = 0; i < 256; i++)
		{
			S[i] = (byte)i;
		}
		int num = 0;
		for (int j = 0; j < 256; j++)
		{
			num = (num + S[j] + defaultKey[j % defaultKey.Length]) % 256;
			Swap(S, j, num);
		}
	}

	public static byte[] EncryptDencrypt(byte[] data)
	{
		x = 0;
		y = 0;
		SetCryptoKey();
		data = data.Take(data.Length).ToArray();
		byte[] array = new byte[data.Length];
		for (int i = 0; i < data.Length; i++)
		{
			array[i] = (byte)(data[i] ^ Kword());
		}
		return array;
	}

	private static void Swap(byte[] array, int ind1, int ind2)
	{
		byte b = array[ind1];
		array[ind1] = array[ind2];
		array[ind2] = b;
	}

	private static byte Kword()
	{
		x = (x + 1) % 256;
		y = (y + S[x]) % 256;
		Swap(S, x, y);
		return S[(S[x] + S[y]) % 256];
	}
}
