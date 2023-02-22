using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class PhotonDataWrite
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
	private struct FloatBytesUnion
	{
		[FieldOffset(0)]
		public float f;

		[FieldOffset(0)]
		public Byte4 b4;
	}

	public BetterList<byte> bytes;

	private FloatBytesUnion floatConverter = default(FloatBytesUnion);

	public PhotonDataWrite()
	{
		bytes = new BetterList<byte>();
	}

	public PhotonDataWrite(int size)
	{
		bytes = new BetterList<byte>();
		bytes.size = size;
	}

	public void Clear()
	{
		bytes.Clear();
	}

	public byte[] ToArray()
	{
		return bytes.ToArray();
	}

	public void Write(byte value)
	{
		bytes.Add(value);
	}

	public void Write(byte[] value)
	{
		Write((short)value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			bytes.Add(value[i]);
		}
	}

	public void Write(bool value)
	{
		bytes.Add((byte)(value ? 1u : 0u));
	}

	public void Write(int value)
	{
		bytes.Add((byte)(value >> 24));
		bytes.Add((byte)(value >> 16));
		bytes.Add((byte)(value >> 8));
		bytes.Add((byte)value);
	}

	public void Write(int[] value)
	{
		Write((short)value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			Write(value[i]);
		}
	}

	public void Write(float value)
	{
		floatConverter.f = value;
		bytes.Add(floatConverter.b4.b1);
		bytes.Add(floatConverter.b4.b2);
		bytes.Add(floatConverter.b4.b3);
		bytes.Add(floatConverter.b4.b4);
	}

	public void Write(short value)
	{
		bytes.Add((byte)(value >> 8));
		bytes.Add((byte)value);
	}

	public void Write(short[] value)
	{
		Write((short)value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			Write(value[i]);
		}
	}

	public void Write(string value)
	{
		byte[] value2 = Encoding.UTF8.GetBytes(value);
		Write(value2);
	}

	public void Write(Vector3 value)
	{
		Write(value.x);
		Write(value.y);
		Write(value.z);
	}

	public void Write(Vector2 value)
	{
		Write(value.x);
		Write(value.y);
	}

	public void Write(Quaternion value)
	{
		Write(value.x);
		Write(value.y);
		Write(value.z);
		Write(value.w);
	}

	public void Write(PhotonPlayer value)
	{
		Write(value.ID);
	}
}
