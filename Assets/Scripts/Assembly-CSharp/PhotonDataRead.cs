using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class PhotonDataRead
{
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
		public Byte4 b;
	}

	public byte[] bytes;

	public int index;

	private FloatBytesUnion floatConverter = default(FloatBytesUnion);

	private Byte4 floatBytes = default(Byte4);

	public PhotonDataRead()
	{
	}

	public PhotonDataRead(byte[] value)
	{
		bytes = value;
	}

	public void Clear()
	{
		index = 0;
	}

	public bool ReadBool()
	{
		return ReadByte() != 0;
	}

	public byte ReadByte()
	{
		byte result = bytes[index];
		index++;
		return result;
	}

	public byte[] ReadBytes()
	{
		nProfiler.BeginSample("PhotonDataRead.ReadBytes");
		int num = ReadShort();
		byte[] array = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadByte();
		}
		nProfiler.EndSample();
		return array;
	}

	public int ReadInt()
	{
		return (ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte();
	}

	public short ReadShort()
	{
		return (short)((ReadByte() << 8) | ReadByte());
	}

	public short[] ReadShorts()
	{
		int num = ReadShort();
		short[] array = new short[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadShort();
		}
		return array;
	}

	public float ReadFloat()
	{
		floatBytes.b1 = ReadByte();
		floatBytes.b2 = ReadByte();
		floatBytes.b3 = ReadByte();
		floatBytes.b4 = ReadByte();
		floatConverter.b = floatBytes;
		return floatConverter.f;
	}

	public string ReadString()
	{
		byte[] array = ReadBytes();
		return Encoding.UTF8.GetString(array);
	}

	public Vector3 ReadVector3()
	{
		return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
	}

	public Quaternion ReadQuaternion()
	{
		return new Quaternion(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
	}
}
