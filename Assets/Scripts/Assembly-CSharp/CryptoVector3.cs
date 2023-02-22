using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public struct CryptoVector3 : IEquatable<CryptoVector3>
{
	[Serializable]
	public struct EncryptVector3
	{
		public int x;

		public int y;

		public int z;
	}

	[Serializable]
	private struct Byte4
	{
		public byte b1;

		public byte b2;

		public byte b3;

		public byte b4;
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
	private EncryptVector3 hiddenValue;

	[SerializeField]
	private Vector3 fakeValue;

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

	public float z
	{
		get
		{
			float num = DecryptFloat(hiddenValue.z);
			if (CryptoManager.fakeValue && fakeValue.z != num)
			{
				CryptoManager.CheatingDetected();
			}
			return num;
		}
		set
		{
			hiddenValue.z = EncryptFloat(value);
			if (CryptoManager.fakeValue)
			{
				fakeValue.z = value;
			}
		}
	}

	private CryptoVector3(Vector3 value)
	{
		cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		EncryptVector3 encryptVector = default(EncryptVector3);
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
		hiddenValue = encryptVector;
		fakeValue = ((!CryptoManager.fakeValue) ? Vector3.zero : value);
		inited = true;
	}

	public void UpdateValue()
	{
		SetValue(GetValue());
	}

	public void SetValue(Vector3 value)
	{
		cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		hiddenValue = Encrypt(value);
		if (CryptoManager.fakeValue)
		{
			fakeValue = value;
		}
	}

	private Vector3 GetValue()
	{
		if (!inited)
		{
			cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
			hiddenValue = Encrypt(Vector3.zero);
			fakeValue = Vector3.zero;
			inited = true;
		}
		Vector3 vector = Decrypt(hiddenValue);
		if (CryptoManager.fakeValue && fakeValue != vector)
		{
			CryptoManager.CheatingDetected();
		}
		return vector;
	}

	private EncryptVector3 Encrypt(Vector3 value)
	{
		EncryptVector3 result = default(EncryptVector3);
		result.x = EncryptFloat(value.x);
		result.y = EncryptFloat(value.y);
		result.z = EncryptFloat(value.z);
		return result;
	}

	private int EncryptFloat(float value)
	{
		FloatIntUnion floatIntUnion = default(FloatIntUnion);
		floatIntUnion.f = value;
		floatIntUnion.i ^= cryptoKey;
		return floatIntUnion.i;
	}

	private Vector3 Decrypt(EncryptVector3 value)
	{
		Vector3 result = default(Vector3);
		result.x = DecryptFloat(value.x);
		result.y = DecryptFloat(value.y);
		result.z = DecryptFloat(value.z);
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
		if (!(obj is CryptoVector3))
		{
			return false;
		}
		return Equals((CryptoVector3)obj);
	}

	public bool Equals(CryptoVector3 obj)
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

	public static implicit operator CryptoVector3(Vector3 value)
	{
		return new CryptoVector3(value);
	}

	public static implicit operator Vector3(CryptoVector3 value)
	{
		return value.GetValue();
	}

	public static CryptoVector3 operator +(CryptoVector3 a, CryptoVector3 b)
	{
		return a.GetValue() + b.GetValue();
	}

	public static CryptoVector3 operator +(Vector3 a, CryptoVector3 b)
	{
		return a + b.GetValue();
	}

	public static CryptoVector3 operator +(CryptoVector3 a, Vector3 b)
	{
		return a.GetValue() + b;
	}

	public static CryptoVector3 operator -(CryptoVector3 a, CryptoVector3 b)
	{
		return a.GetValue() - b.GetValue();
	}

	public static CryptoVector3 operator -(Vector3 a, CryptoVector3 b)
	{
		return a - b.GetValue();
	}

	public static CryptoVector3 operator -(CryptoVector3 a, Vector3 b)
	{
		return a.GetValue() - b;
	}

	public static CryptoVector3 operator -(CryptoVector3 a)
	{
		return -a.GetValue();
	}

	public static CryptoVector3 operator *(CryptoVector3 a, float d)
	{
		return a.GetValue() * d;
	}

	public static CryptoVector3 operator *(float d, CryptoVector3 a)
	{
		return d * a.GetValue();
	}

	public static CryptoVector3 operator /(CryptoVector3 a, float d)
	{
		return a.GetValue() / d;
	}

	public static bool operator ==(CryptoVector3 lhs, CryptoVector3 rhs)
	{
		return lhs.GetValue() == rhs.GetValue();
	}

	public static bool operator ==(Vector3 lhs, CryptoVector3 rhs)
	{
		return lhs == rhs.GetValue();
	}

	public static bool operator ==(CryptoVector3 lhs, Vector3 rhs)
	{
		return lhs.GetValue() == rhs;
	}

	public static bool operator !=(CryptoVector3 lhs, CryptoVector3 rhs)
	{
		return lhs.GetValue() != rhs.GetValue();
	}

	public static bool operator !=(Vector3 lhs, CryptoVector3 rhs)
	{
		return lhs != rhs.GetValue();
	}

	public static bool operator !=(CryptoVector3 lhs, Vector3 rhs)
	{
		return lhs.GetValue() != rhs;
	}
}
