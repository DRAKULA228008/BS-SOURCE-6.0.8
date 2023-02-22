using System.Collections.Generic;
using UnityEngine;

public class DecalInfo
{
	private static PhotonDataWrite writeData = new PhotonDataWrite();

	private static PhotonDataRead readData = new PhotonDataRead();

	public byte BloodDecal = 200;

	public bool isKnife;

	public BetterList<Vector3> Points = new BetterList<Vector3>();

	public BetterList<Vector3> Normals = new BetterList<Vector3>();

	private static List<DecalInfo> PoolList = new List<DecalInfo>();

	public static DecalInfo Get()
	{
		nProfiler.BeginSample("DecalInfo.Get");
		if (PoolList.Count != 0)
		{
			DecalInfo result = PoolList[0];
			PoolList.RemoveAt(0);
			nProfiler.EndSample();
			return result;
		}
		nProfiler.EndSample();
		return new DecalInfo();
	}

	public void Dispose()
	{
		BloodDecal = 200;
		isKnife = false;
		Points.Clear();
		Normals.Clear();
		PoolList.Add(this);
	}

	public byte[] ToArray()
	{
		if (BloodDecal == 200 && Points.size == 0)
		{
			return new byte[1] { 199 };
		}
		writeData.Clear();
		writeData.Write(BloodDecal);
		writeData.Write(isKnife);
		writeData.Write((byte)Points.size);
		for (int i = 0; i < Points.size; i++)
		{
			writeData.Write(Points[i]);
		}
		writeData.Write((byte)Normals.size);
		for (int j = 0; j < Normals.size; j++)
		{
			writeData.Write(Normals[j]);
		}
		return writeData.ToArray();
	}

	public static DecalInfo SetData(byte[] bytes)
	{
		nProfiler.BeginSample("DecalInfo.SetData");
		DecalInfo decalInfo = Get();
		if (bytes.Length > 1)
		{
			readData.Clear();
			readData.bytes = bytes;
			decalInfo.BloodDecal = readData.ReadByte();
			decalInfo.isKnife = readData.ReadBool();
			byte b = readData.ReadByte();
			for (int i = 0; i < b; i++)
			{
				decalInfo.Points.Add(readData.ReadVector3());
			}
			byte b2 = readData.ReadByte();
			for (int j = 0; j < b2; j++)
			{
				decalInfo.Normals.Add(readData.ReadVector3());
			}
		}
		nProfiler.EndSample();
		return decalInfo;
	}
}
