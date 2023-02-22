using System;
using UnityEngine;

[Serializable]
public struct CryptoInt3 : IFormattable, IEquatable<CryptoInt3>
{
	[SerializeField]
	private int cryptoKey;

	[SerializeField]
	private int hiddenValue;

	[SerializeField]
	private int fakeValue;

	[SerializeField]
	private bool inited;

	private CryptoInt3(int value)
	{
		do
		{
			cryptoKey = CryptoManager.Next();
		}
		while (cryptoKey == 0);
		hiddenValue = value + cryptoKey;
		fakeValue = value - cryptoKey;
		inited = true;
	}

	public void SetValue(int value)
	{
		do
		{
			cryptoKey = CryptoManager.Next();
		}
		while (cryptoKey == 0);
		hiddenValue = value + cryptoKey;
		fakeValue = value - cryptoKey;
	}

	public void UpdateValue()
	{
		SetValue(GetValue());
	}

	private int GetValue()
	{
		if (!inited)
		{
			do
			{
				cryptoKey = CryptoManager.Next();
			}
			while (cryptoKey == 0);
			hiddenValue = cryptoKey;
			fakeValue = -cryptoKey;
			inited = true;
		}
		if (hiddenValue - cryptoKey != fakeValue + cryptoKey)
		{
			CryptoManager.CheatingDetected();
			return 0;
		}
		return hiddenValue - cryptoKey;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is CryptoInt3))
		{
			return false;
		}
		return Equals((CryptoInt3)obj);
	}

	public bool Equals(CryptoInt3 obj)
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

	public string ToString(string format)
	{
		return GetValue().ToString(format);
	}

	public string ToString(IFormatProvider provider)
	{
		return GetValue().ToString(provider);
	}

	public string ToString(string format, IFormatProvider provider)
	{
		return GetValue().ToString(format, provider);
	}

	public static implicit operator CryptoInt3(int value)
	{
		return new CryptoInt3(value);
	}

	public static implicit operator CryptoInt3(CryptoInt value)
	{
		return new CryptoInt3(value);
	}

	public static implicit operator int(CryptoInt3 value)
	{
		return value.GetValue();
	}

	public static CryptoInt3 operator ++(CryptoInt3 value)
	{
		value.SetValue(value.GetValue() + 1);
		return value;
	}

	public static CryptoInt3 operator --(CryptoInt3 value)
	{
		value.SetValue(value.GetValue() - 1);
		return value;
	}
}
