using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public struct CryptoFloat : IFormattable, IEquatable<CryptoFloat>
{
	[Serializable]
	private struct Byte4
	{
		public byte b1;

		public byte b2;

		public byte b3;

		public byte b4;
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct FloatIntBytesUnion
	{
		[FieldOffset(0)]
		public float f;

		[FieldOffset(0)]
		public int i;

		[FieldOffset(0)]
		public Byte4 b4;
	}

	[SerializeField]
	private int cryptoKey;

	[SerializeField]
	private Byte4 hiddenValue;

	[SerializeField]
	private float fakeValue;

	[SerializeField]
	private bool inited;

	private CryptoFloat(float value)
	{
		cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		FloatIntBytesUnion floatIntBytesUnion = new FloatIntBytesUnion
		{
			f = value
		};
		floatIntBytesUnion.i ^= cryptoKey;
		hiddenValue = floatIntBytesUnion.b4;
		fakeValue = ((!CryptoManager.fakeValue) ? 0f : value);
		inited = true;
	}

	public void UpdateValue()
	{
		SetValue(GetValue());
	}

	public void SetValue(float value)
	{
		cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		hiddenValue = Encrypt(value);
		fakeValue = ((!CryptoManager.fakeValue) ? 0f : value);
	}

	private float GetValue()
	{
		if (!inited)
		{
			cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
			hiddenValue = Encrypt(0f);
			fakeValue = 0f;
			inited = true;
		}
		float num = Decrypt(hiddenValue);
		if (CryptoManager.fakeValue && fakeValue != num)
		{
			CryptoManager.CheatingDetected();
		}
		return num;
	}

	private Byte4 Encrypt(float value)
	{
		FloatIntBytesUnion floatIntBytesUnion = default(FloatIntBytesUnion);
		floatIntBytesUnion.f = value;
		floatIntBytesUnion.i ^= cryptoKey;
		return floatIntBytesUnion.b4;
	}

	private float Decrypt(Byte4 bytes)
	{
		FloatIntBytesUnion floatIntBytesUnion = default(FloatIntBytesUnion);
		floatIntBytesUnion.b4 = hiddenValue;
		floatIntBytesUnion.i ^= cryptoKey;
		return floatIntBytesUnion.f;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is CryptoFloat))
		{
			return false;
		}
		return Equals((CryptoFloat)obj);
	}

	public bool Equals(CryptoFloat obj)
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

	public static implicit operator CryptoFloat(float value)
	{
		return new CryptoFloat(value);
	}

	public static implicit operator float(CryptoFloat value)
	{
		return value.GetValue();
	}

	public static CryptoFloat operator ++(CryptoFloat value)
	{
		value.SetValue(value.GetValue() + 1f);
		return value;
	}

	public static CryptoFloat operator --(CryptoFloat value)
	{
		value.SetValue(value.GetValue() - 1f);
		return value;
	}
}
