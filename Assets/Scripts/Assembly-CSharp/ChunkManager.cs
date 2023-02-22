using System;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
	public static List<Chunk> chunks = new List<Chunk>();

	public byte maxSizeX = 50;

	public byte maxSizeY = 50;

	public byte maxSizeZ = 50;

	public byte size = 5;

	public float scale = 1f;

	public byte texture;

	public Material material;

	public string save;

	private static ChunkManager instance;

	public static byte chunkSize
	{
		get
		{
			return instance.size;
		}
	}

	public static float chunkScale
	{
		get
		{
			return instance.scale;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	public static void CreatePlane(int x, int y, int z)
	{
		byte[,,] array = new byte[x, y, z];
		for (int i = 0; i < array.GetLength(0); i++)
		{
			for (int j = 0; j < array.GetLength(1); j++)
			{
				for (int k = 0; k < array.GetLength(2); k++)
				{
					array[i, j, k] = 1;
				}
			}
		}
		SetMap(array);
	}

	public static void SetMap(byte[,,] map)
	{
		for (int i = 0; i < map.GetLength(0); i++)
		{
			for (int j = 0; j < map.GetLength(1); j++)
			{
				for (int k = 0; k < map.GetLength(2); k++)
				{
					Chunk chunk = FindChunk(i, j, k);
					if (chunk == null)
					{
						chunk = CreateChunk(i, j, k);
					}
					chunk.map[i - chunk.minX, j - chunk.minY, k - chunk.minZ] = map[i, j, k];
				}
			}
		}
	}

	public static string SaveMap()
	{
		byte[] array = new byte[instance.maxSizeX * instance.maxSizeY * instance.maxSizeZ + 5];
		array[0] = instance.maxSizeX;
		array[1] = instance.maxSizeY;
		array[2] = instance.maxSizeZ;
		array[3] = instance.size;
		int num = 4;
		for (int i = 0; i < instance.maxSizeX; i++)
		{
			for (int j = 0; j < instance.maxSizeY; j++)
			{
				for (int k = 0; k < instance.maxSizeZ; k++)
				{
					Chunk chunk = FindChunk(i, j, k);
					if (chunk != null)
					{
						array[num] = chunk.map[i - chunk.minX, j - chunk.minY, k - chunk.minZ];
					}
					num++;
				}
			}
		}
		MonoBehaviour.print(array.Length);
		MonoBehaviour.print(lzip.compressBuffer(array, 9).Length);
		return Convert.ToBase64String(lzip.compressBuffer(array, 9));
	}

	public static void LoadMap(byte[] list)
	{
		list = lzip.decompressBuffer(list);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		instance.maxSizeX = list[0];
		instance.maxSizeY = list[1];
		instance.maxSizeZ = list[2];
		instance.size = list[3];
		for (int i = 4; i < list.Length; i++)
		{
			if (num3 > instance.maxSizeZ - 1)
			{
				num3 = 0;
				if (num2 >= instance.maxSizeY - 1)
				{
					num2 = 0;
					num++;
				}
				else
				{
					num2++;
				}
			}
			if (list[i] != 0)
			{
				Chunk chunk = FindChunk(num, num2, num3);
				if (chunk == null)
				{
					chunk = CreateChunk(num, num2, num3);
				}
				chunk.map[num - chunk.minX, num2 - chunk.minY, num3 - chunk.minZ] = list[i];
			}
			num3++;
		}
		UpdateMeshAll();
	}

	public static void AddCube(int x, int y, int z, byte tex)
	{
		if (x >= 0 && y >= 0 && z >= 0 && x < instance.maxSizeX && y < instance.maxSizeY && z < instance.maxSizeZ)
		{
			Chunk chunk = FindChunk(x, y, z);
			if (chunk == null)
			{
				chunk = CreateChunk(x, y, z);
			}
			chunk.AddCube(x, y, z, tex);
		}
	}

	public static void RemoveCube(int x, int y, int z, byte tex)
	{
		Chunk chunk = FindChunk(x, y, z);
		if (chunk != null)
		{
			chunk.RemoveCube(x, y, z, tex);
		}
	}

	public static Chunk FindChunk(int x, int y, int z)
	{
		for (int i = 0; i < chunks.Count; i++)
		{
			if (x >= chunks[i].minX && y >= chunks[i].minY && z >= chunks[i].minZ && x < chunks[i].minX + chunkSize && y < chunks[i].minY + chunkSize && z < chunks[i].minZ + chunkSize)
			{
				return chunks[i];
			}
		}
		return null;
	}

	private static Chunk CreateChunk(int x, int y, int z)
	{
		GameObject gameObject = new GameObject("Chunk [" + x / (int)chunkSize * chunkSize + "," + y / (int)chunkSize * chunkSize + "," + z / (int)chunkSize * chunkSize + "]");
		gameObject.transform.SetParent(instance.transform);
		gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = instance.material;
		byte minX = (byte)(x / (int)chunkSize * chunkSize);
		byte minY = (byte)(y / (int)chunkSize * chunkSize);
		byte minZ = (byte)(z / (int)chunkSize * chunkSize);
		gameObject.transform.localPosition = new Vector3((float)(x / (int)chunkSize * chunkSize) * chunkScale, (float)(y / (int)chunkSize * chunkSize) * chunkScale, (float)(z / (int)chunkSize * chunkSize) * chunkScale);
		gameObject.tag = "ChunkBlock";
		Chunk chunk = gameObject.AddComponent<Chunk>();
		chunk.Init();
		chunk.minX = minX;
		chunk.minY = minY;
		chunk.minZ = minZ;
		return chunk;
	}

	public static void UpdateMeshAll()
	{
		for (int i = 0; i < chunks.Count; i++)
		{
			chunks[i].UpdateMesh();
			chunks[i].UpdateColliders();
		}
	}
}
