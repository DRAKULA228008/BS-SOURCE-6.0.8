using System;
using UnityEngine;

public class TileAtlasManager : ScriptableObject
{
	[Serializable]
	public struct TextureData
	{
		public Material mat;

		public byte x;

		public byte y;

		public byte width;

		public byte height;
	}

	public Material mat;

	public TextureData[] data;

	private static TileAtlasManager Instance;

	public static TileAtlasManager instance
	{
		get
		{
			if (Instance == null)
			{
				Instance = Resources.Load("TileAtlasManager") as TileAtlasManager;
			}
			return Instance;
		}
	}

	public static TextureData GetData(Material material)
	{
		for (int i = 0; i < instance.data.Length; i++)
		{
			if (instance.data[i].mat == material)
			{
				return instance.data[i];
			}
		}
		return default(TextureData);
	}

	public static TextureData GetData(byte x, byte y)
	{
		for (int i = 0; i < instance.data.Length; i++)
		{
			if (instance.data[i].x == x && instance.data[i].y == y)
			{
				return instance.data[i];
			}
		}
		return default(TextureData);
	}

	public static Vector2 GetCordinates(byte index)
	{
		return new Vector2((int)instance.data[index].x, (int)instance.data[index].y);
	}

	public static bool Contains(Material material)
	{
		for (int i = 0; i < instance.data.Length; i++)
		{
			if (instance.data[i].mat == material)
			{
				return true;
			}
		}
		return false;
	}
}
