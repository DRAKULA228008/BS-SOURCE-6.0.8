using UnityEngine;

public class PhotonStream
{
	public PhotonDataWrite writeData;

	public PhotonDataRead readData;

	private bool write;

	public bool isWriting
	{
		get
		{
			return write;
		}
	}

	public bool isReading
	{
		get
		{
			return !write;
		}
	}

	public int Count
	{
		get
		{
			return (!isWriting) ? readData.bytes.Length : writeData.bytes.size;
		}
	}

	public PhotonStream(bool write)
	{
		this.write = write;
		if (write)
		{
			writeData = new PhotonDataWrite(32);
		}
		else
		{
			readData = new PhotonDataRead();
		}
	}

	public byte[] ToArray()
	{
		return writeData.ToArray();
	}

	public void SetData(byte[] bytes)
	{
		readData.Clear();
		readData.bytes = bytes;
	}

	public void Clear()
	{
		if (isWriting)
		{
			writeData.Clear();
		}
		else
		{
			readData.Clear();
		}
	}

	public void Write(byte value)
	{
		writeData.Write(value);
	}

	public void Write(byte[] value)
	{
		writeData.Write(value);
	}

	public void Write(bool value)
	{
		writeData.Write(value);
	}

	public void Write(int value)
	{
		writeData.Write(value);
	}

	public void Write(int[] value)
	{
		writeData.Write(value);
	}

	public void Write(float value)
	{
		writeData.Write(value);
	}

	public void Write(short value)
	{
		writeData.Write(value);
	}

	public void Write(string value)
	{
		writeData.Write(value);
	}

	public void Write(Vector3 value)
	{
		writeData.Write(value);
	}

	public void Write(Vector2 value)
	{
		writeData.Write(value);
	}

	public void Write(Quaternion value)
	{
		writeData.Write(value);
	}

	public byte ReadByte()
	{
		return readData.ReadByte();
	}

	public bool ReadBool()
	{
		return readData.ReadBool();
	}

	public byte[] ReadBytes()
	{
		return readData.ReadBytes();
	}

	public int ReadInt()
	{
		return readData.ReadInt();
	}

	public short ReadShort()
	{
		return readData.ReadShort();
	}

	public float ReadFloat()
	{
		return readData.ReadFloat();
	}

	public Vector3 ReadVector3()
	{
		return readData.ReadVector3();
	}

	public Quaternion ReadQuaternion()
	{
		return readData.ReadQuaternion();
	}
}
