using System;
using System.Security.Cryptography;
using System.Text;

public static class CryptoManager
{
	public static string cryptoKey;

	public static byte[] cryptoKeyBytes;

	public static Random random;

	public static bool fakeValue;

	private static int seed;

	private static int randValue;

	private static int _staticValue;

	private static Action DetectorAction;

	public static int staticValue
	{
		get
		{
			if (_staticValue == 0)
			{
				_staticValue = Next(1, 10);
			}
			return _staticValue;
		}
	}

	static CryptoManager()
	{
		cryptoKey = "^8b1v]x4";
		cryptoKeyBytes = new byte[5] { 1, 61, 42, 250, 125 };
		random = new Random();
		fakeValue = false;
		_staticValue = 0;
		seed = DateTime.Now.Millisecond;
		randValue = seed;
	}

	public static void StartDetection(Action callback)
	{
		DetectorAction = callback;
	}

	public static void CheatingDetected()
	{
		if (DetectorAction != null)
		{
			DetectorAction();
		}
	}

	public static string MD5(int value)
	{
		return MD5(BitConverter.GetBytes(value));
	}

	public static string MD5(string value)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		return MD5(uTF8Encoding.GetBytes(value));
	}

	public static string MD5(byte[] bytes)
	{
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text += Convert.ToString(array[i], 16).PadLeft(2, '0');
		}
		return text.PadLeft(32, '0');
	}

	public static int Next()
	{
		randValue ^= randValue << 21;
		randValue ^= randValue >> 3;
		randValue ^= randValue << 4;
		return randValue;
	}

	public static int Next(int min, int max)
	{
		randValue ^= randValue << 21;
		randValue ^= randValue >> 3;
		randValue ^= randValue << 4;
		return (int)(((float)randValue / 2.14748365E+09f + 1f) / 2f * (float)(max - min) + (float)min);
	}
}
