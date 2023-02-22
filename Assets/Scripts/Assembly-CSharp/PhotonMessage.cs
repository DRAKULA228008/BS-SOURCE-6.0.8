using UnityEngine;

public class PhotonMessage
{
	public PhotonDataRead readData = new PhotonDataRead();

	private int timeInt;

	public PhotonPlayer sender;

	public double timestamp
	{
		get
		{
			uint num = (uint)timeInt;
			double num2 = num;
			return num2 / 1000.0;
		}
	}

	public void SetData(byte[] bytes, int senderID, int sendTime)
	{
		nProfiler.BeginSample("PhotonMessage.SetData");
		readData.Clear();
		readData.bytes = bytes;
		sender = PhotonPlayer.Find(senderID);
		timeInt = sendTime;
		nProfiler.EndSample();
	}

	public void Clear()
	{
		readData.Clear();
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

	public short[] ReadShorts()
	{
		return readData.ReadShorts();
	}

	public float ReadFloat()
	{
		return readData.ReadFloat();
	}

	public string ReadString()
	{
		return readData.ReadString();
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
