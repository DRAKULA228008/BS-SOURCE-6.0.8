using System;
using UnityEngine;

[Serializable]
public struct CryptoString : IEquatable<CryptoString>
{
	[SerializeField]
	private byte[] cryptoKey;

	[SerializeField]
	private byte[] hiddenValue;

	[SerializeField]
	private string fakeValue;

	[SerializeField]
	private bool inited;

	public int Length
	{
		get
		{
			return GetValue().Length;
		}
	}

	private CryptoString(string value)
	{
		cryptoKey = new byte[CryptoManager.random.Next(10, 30)];
		CryptoManager.random.NextBytes(cryptoKey);
		hiddenValue = GetBytes(value);
		for (int i = 0; i < hiddenValue.Length; i++)
		{
			hiddenValue[i] = (byte)(hiddenValue[i] ^ cryptoKey[i % cryptoKey.Length]);
		}
		fakeValue = ((!CryptoManager.fakeValue) ? string.Empty : value);
		inited = true;
	}

	public void SetValue(string value)
	{
		cryptoKey = new byte[CryptoManager.random.Next(10, 30)];
		CryptoManager.random.NextBytes(cryptoKey);
		hiddenValue = GetBytes(value);
		for (int i = 0; i < hiddenValue.Length; i++)
		{
			hiddenValue[i] = (byte)(hiddenValue[i] ^ cryptoKey[i % cryptoKey.Length]);
		}
		fakeValue = ((!CryptoManager.fakeValue) ? string.Empty : value);
	}

	private string GetValue()
	{
		if (!inited)
		{
			cryptoKey = new byte[CryptoManager.random.Next(10, 30)];
			CryptoManager.random.NextBytes(cryptoKey);
			hiddenValue = new byte[0];
			fakeValue = string.Empty;
			inited = true;
		}
		byte[] array = new byte[hiddenValue.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (byte)(hiddenValue[i] ^ cryptoKey[i % cryptoKey.Length]);
		}
		string @string = GetString(array);
		if (CryptoManager.fakeValue && !string.IsNullOrEmpty(fakeValue) && fakeValue != @string)
		{
			CryptoManager.CheatingDetected();
		}
		return @string;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is CryptoString))
		{
			return false;
		}
		return Equals((CryptoString)obj);
	}

	public bool Equals(CryptoString obj)
	{
		return GetValue() == obj.GetValue();
	}

	public override int GetHashCode()
	{
		return GetValue().GetHashCode();
	}

	public override string ToString()
	{
		return GetValue().ToString();
	}

	public bool Contains(string value)
	{
		return GetValue().Contains(value);
	}

	public static byte[] GetBytes(string str)
	{
		byte[] array = new byte[str.Length * 2];
		Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
		return array;
	}

	public static string GetString(byte[] bytes)
	{
		char[] array = new char[bytes.Length / 2];
		Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
		return new string(array);
	}

	public static implicit operator CryptoString(string value)
	{
		return new CryptoString(value);
	}

	public static implicit operator string(CryptoString value)
	{
		return value.GetValue();
	}
}
