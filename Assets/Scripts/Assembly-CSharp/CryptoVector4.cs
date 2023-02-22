using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public struct CryptoVector4 : IEquatable<CryptoVector4>
{
	[Serializable]
	public struct EncryptVector4
	{
		public int x;

		public int y;

		public int z;

		public int w;
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
	private EncryptVector4 hiddenValue;

	[SerializeField]
	private Vector4 fakeValue;

	[SerializeField]
	private bool inited;

	public float x
	{
		get
		{
			float num = DecryptFloat(hiddenValue.x);
			if (fakeValue.x != num)
			{
				CryptoManager.CheatingDetected();
			}
			return num;
		}
	}

	public float y
	{
		get
		{
			float num = DecryptFloat(hiddenValue.y);
			if (fakeValue.y != num)
			{
				CryptoManager.CheatingDetected();
			}
			return num;
		}
	}

	public float z
	{
		get
		{
			float num = DecryptFloat(hiddenValue.z);
			if (fakeValue.z != num)
			{
				CryptoManager.CheatingDetected();
			}
			return num;
		}
	}

	public float w
	{
		get
		{
			float num = DecryptFloat(hiddenValue.w);
			if (fakeValue.z != num)
			{
				CryptoManager.CheatingDetected();
			}
			return num;
		}
	}

	private CryptoVector4(Vector4 value)
	{
		cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		EncryptVector4 encryptVector = default(EncryptVector4);
		encryptVector.x = new FloatIntUnion
		{
			f = value.x
		}.i ^ cryptoKey;
		encryptVector.y = new FloatIntUnion
		{
			f = value.y
		}.i ^ cryptoKey;
		encryptVector.z = new FloatIntUnion
		{
			f = value.z
		}.i ^ cryptoKey;
		encryptVector.w = new FloatIntUnion
		{
			f = value.w
		}.i ^ cryptoKey;
		hiddenValue = encryptVector;
		fakeValue = value;
		inited = true;
	}

	public void SetValue(Vector4 value)
	{
		cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		hiddenValue = Encrypt(value);
		fakeValue = value;
	}

	private Vector4 GetValue()
	{
		if (!inited)
		{
			cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
			hiddenValue = Encrypt(Vector4.zero);
			fakeValue = Vector4.zero;
			inited = true;
		}
		Vector4 vector = Decrypt(hiddenValue);
		if (fakeValue != vector)
		{
			CryptoManager.CheatingDetected();
		}
		return vector;
	}

	private EncryptVector4 Encrypt(Vector4 value)
	{
		EncryptVector4 result = default(EncryptVector4);
		result.x = EncryptFloat(value.x);
		result.y = EncryptFloat(value.y);
		result.z = EncryptFloat(value.z);
		result.w = EncryptFloat(value.w);
		return result;
	}

	private int EncryptFloat(float value)
	{
		FloatIntUnion floatIntUnion = default(FloatIntUnion);
		floatIntUnion.f = value;
		floatIntUnion.i ^= cryptoKey;
		return floatIntUnion.i;
	}

	private Vector4 Decrypt(EncryptVector4 value)
	{
		Vector4 result = default(Vector4);
		result.x = DecryptFloat(value.x);
		result.y = DecryptFloat(value.y);
		result.z = DecryptFloat(value.z);
		result.w = DecryptFloat(value.w);
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
		if (!(obj is CryptoVector4))
		{
			return false;
		}
		return Equals((CryptoVector4)obj);
	}

	public bool Equals(CryptoVector4 obj)
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

	public static implicit operator CryptoVector4(Vector4 value)
	{
		return new CryptoVector4(value);
	}

	public static implicit operator Vector4(CryptoVector4 value)
	{
		return value.GetValue();
	}

	public static Vector4 operator +(CryptoVector4 a, CryptoVector4 b)
	{
		return a.GetValue() + b.GetValue();
	}

	public static Vector4 operator +(Vector4 a, CryptoVector4 b)
	{
		return a + b.GetValue();
	}

	public static Vector4 operator +(CryptoVector4 a, Vector4 b)
	{
		return a.GetValue() + b;
	}

	public static Vector4 operator -(CryptoVector4 a, CryptoVector4 b)
	{
		return a.GetValue() - b.GetValue();
	}

	public static Vector4 operator -(Vector4 a, CryptoVector4 b)
	{
		return a - b.GetValue();
	}

	public static Vector4 operator -(CryptoVector4 a, Vector4 b)
	{
		return a.GetValue() - b;
	}

	public static Vector4 operator -(CryptoVector4 a)
	{
		return -a.GetValue();
	}

	public static Vector4 operator *(CryptoVector4 a, float d)
	{
		return a.GetValue() * d;
	}

	public static Vector4 operator *(float d, CryptoVector4 a)
	{
		return d * a.GetValue();
	}

	public static Vector4 operator /(CryptoVector4 a, float d)
	{
		return a.GetValue() / d;
	}

	public static bool operator ==(CryptoVector4 lhs, CryptoVector4 rhs)
	{
		return lhs.GetValue() == rhs.GetValue();
	}

	public static bool operator ==(Vector4 lhs, CryptoVector4 rhs)
	{
		return lhs == rhs.GetValue();
	}

	public static bool operator ==(CryptoVector4 lhs, Vector4 rhs)
	{
		return lhs.GetValue() == rhs;
	}

	public static bool operator !=(CryptoVector4 lhs, CryptoVector4 rhs)
	{
		return lhs.GetValue() != rhs.GetValue();
	}

	public static bool operator !=(Vector4 lhs, CryptoVector4 rhs)
	{
		return lhs != rhs.GetValue();
	}

	public static bool operator !=(CryptoVector4 lhs, Vector4 rhs)
	{
		return lhs.GetValue() != rhs;
	}
}
