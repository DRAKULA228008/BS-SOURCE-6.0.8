using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public struct CryptoVector2 : IEquatable<CryptoVector2>
{
	[Serializable]
	public struct EncryptVector2
	{
		public int x;

		public int y;
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct FloatIntUnion
	{
		[FieldOffset(0)]
		public float f;

		[FieldOffset(0)]
		public int i;
	}

	[SerializeField]
	private int cryptoKey;

	[SerializeField]
	private EncryptVector2 hiddenValue;

	[SerializeField]
	private Vector2 fakeValue;

	[SerializeField]
	private bool inited;

	public float x
	{
		get
		{
			float num = DecryptFloat(hiddenValue.x);
			if (CryptoManager.fakeValue && fakeValue.x != num)
			{
				CryptoManager.CheatingDetected();
			}
			return num;
		}
		set
		{
			hiddenValue.x = EncryptFloat(value);
			if (CryptoManager.fakeValue)
			{
				fakeValue.x = value;
			}
		}
	}

	public float y
	{
		get
		{
			float num = DecryptFloat(hiddenValue.y);
			if (CryptoManager.fakeValue && fakeValue.y != num)
			{
				CryptoManager.CheatingDetected();
			}
			return num;
		}
		set
		{
			hiddenValue.y = EncryptFloat(value);
			if (CryptoManager.fakeValue)
			{
				fakeValue.y = value;
			}
		}
	}

	private CryptoVector2(Vector2 value)
	{
		cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		EncryptVector2 encryptVector = default(EncryptVector2);
		encryptVector.x = new FloatIntUnion
		{
			f = value.x
		}.i ^ cryptoKey;
		encryptVector.y = new FloatIntUnion
		{
			f = value.y
		}.i ^ cryptoKey;
		hiddenValue = encryptVector;
		fakeValue = ((!CryptoManager.fakeValue) ? Vector2.zero : value);
		inited = true;
	}

	public void UpdateValue()
	{
		SetValue(GetValue());
	}

	public void SetValue(Vector2 value)
	{
		cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		hiddenValue = Encrypt(value);
		if (CryptoManager.fakeValue)
		{
			fakeValue = value;
		}
	}

	private Vector2 GetValue()
	{
		if (!inited)
		{
			cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
			hiddenValue = Encrypt(Vector2.zero);
			fakeValue = Vector2.zero;
			inited = true;
		}
		Vector2 vector = Decrypt(hiddenValue);
		if (CryptoManager.fakeValue && fakeValue != vector)
		{
			CryptoManager.CheatingDetected();
		}
		return vector;
	}

	private EncryptVector2 Encrypt(Vector2 value)
	{
		EncryptVector2 result = default(EncryptVector2);
		result.x = EncryptFloat(value.x);
		result.y = EncryptFloat(value.y);
		return result;
	}

	private int EncryptFloat(float value)
	{
		FloatIntUnion floatIntUnion = default(FloatIntUnion);
		floatIntUnion.f = value;
		floatIntUnion.i ^= cryptoKey;
		return floatIntUnion.i;
	}

	private Vector2 Decrypt(EncryptVector2 value)
	{
		Vector2 result = default(Vector2);
		result.x = DecryptFloat(value.x);
		result.y = DecryptFloat(value.y);
		return result;
	}

	private float DecryptFloat(int value)
	{
		FloatIntUnion floatIntUnion = default(FloatIntUnion);
		floatIntUnion.i = value ^ cryptoKey;
		return floatIntUnion.f;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is CryptoVector2))
		{
			return false;
		}
		return Equals((CryptoVector2)obj);
	}

	public bool Equals(CryptoVector2 obj)
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

	public static implicit operator CryptoVector2(Vector2 value)
	{
		return new CryptoVector2(value);
	}

	public static implicit operator Vector2(CryptoVector2 value)
	{
		return value.GetValue();
	}

	public static CryptoVector2 operator +(CryptoVector2 a, CryptoVector2 b)
	{
		return a.GetValue() + b.GetValue();
	}

	public static CryptoVector2 operator +(Vector2 a, CryptoVector2 b)
	{
		return a + b.GetValue();
	}

	public static CryptoVector2 operator +(CryptoVector2 a, Vector2 b)
	{
		return a.GetValue() + b;
	}

	public static CryptoVector2 operator -(CryptoVector2 a, CryptoVector2 b)
	{
		return a.GetValue() - b.GetValue();
	}

	public static CryptoVector2 operator -(Vector2 a, CryptoVector2 b)
	{
		return a - b.GetValue();
	}

	public static CryptoVector2 operator -(CryptoVector2 a, Vector2 b)
	{
		return a.GetValue() - b;
	}

	public static CryptoVector2 operator -(CryptoVector2 a)
	{
		return -a.GetValue();
	}

	public static CryptoVector2 operator *(CryptoVector2 a, float d)
	{
		return a.GetValue() * d;
	}

	public static CryptoVector2 operator *(float d, CryptoVector2 a)
	{
		return d * a.GetValue();
	}

	public static CryptoVector2 operator /(CryptoVector2 a, float d)
	{
		return a.GetValue() / d;
	}

	public static bool operator ==(CryptoVector2 lhs, CryptoVector2 rhs)
	{
		return lhs.GetValue() == rhs.GetValue();
	}

	public static bool operator ==(Vector2 lhs, CryptoVector2 rhs)
	{
		return lhs == rhs.GetValue();
	}

	public static bool operator ==(CryptoVector2 lhs, Vector2 rhs)
	{
		return lhs.GetValue() == rhs;
	}

	public static bool operator !=(CryptoVector2 lhs, CryptoVector2 rhs)
	{
		return lhs.GetValue() != rhs.GetValue();
	}

	public static bool operator !=(Vector2 lhs, CryptoVector2 rhs)
	{
		return lhs != rhs.GetValue();
	}

	public static bool operator !=(CryptoVector2 lhs, Vector2 rhs)
	{
		return lhs.GetValue() != rhs;
	}
}
