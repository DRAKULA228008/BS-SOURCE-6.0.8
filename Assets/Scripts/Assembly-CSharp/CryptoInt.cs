using System;
using UnityEngine;

[Serializable]
public struct CryptoInt : IFormattable, IEquatable<CryptoInt>
{
	[SerializeField]
	private int cryptoKey;

	[SerializeField]
	private int hiddenValue;

	[SerializeField]
	private int fakeValue;

	[SerializeField]
	private bool inited;

	private CryptoInt(int value)
	{
		do
		{
			cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		}
		while (cryptoKey == 0);
		hiddenValue = value ^ cryptoKey;
		fakeValue = (CryptoManager.fakeValue ? value : 0);
		inited = true;
	}

	public void SetValue(int value)
	{
		do
		{
			cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		}
		while (cryptoKey == 0);
		fakeValue = (CryptoManager.fakeValue ? value : 0);
		hiddenValue = value ^ cryptoKey;
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
				cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
			}
			while (cryptoKey == 0);
			hiddenValue = cryptoKey;
			fakeValue = 0;
			inited = true;
		}
		int num = hiddenValue ^ cryptoKey;
		if ((CryptoManager.fakeValue && fakeValue != num) || cryptoKey == 0)
		{
			CryptoManager.CheatingDetected();
		}
		return num;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is CryptoInt))
		{
			return false;
		}
		return Equals((CryptoInt)obj);
	}

	public bool Equals(CryptoInt obj)
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

	public static implicit operator CryptoInt(int value)
	{
		return new CryptoInt(value);
	}

	public static implicit operator int(CryptoInt value)
	{
		return value.GetValue();
	}

	public static CryptoInt operator ++(CryptoInt value)
	{
		value.SetValue(value.GetValue() + 1);
		return value;
	}

	public static CryptoInt operator --(CryptoInt value)
	{
		value.SetValue(value.GetValue() - 1);
		return value;
	}
}
