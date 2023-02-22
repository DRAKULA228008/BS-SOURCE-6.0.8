using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	public byte[,,] map;

	public byte minX;

	public byte minY;

	public byte minZ;

	private MeshFilter meshFilter;

	private Mesh mesh;

	private BoxCollider[,,] boxColliders;

	private bool init;

	private List<Vector3> vertices = new List<Vector3>();

	private List<int> triangles = new List<int>();

	private List<Vector2> uvs = new List<Vector2>();

	private void Start()
	{
		Init();
	}

	public void Init()
	{
		if (!init)
		{
			ChunkManager.chunks.Add(this);
			meshFilter = GetComponent<MeshFilter>();
			mesh = new Mesh();
			meshFilter.sharedMesh = mesh;
			byte chunkSize = ChunkManager.chunkSize;
			map = new byte[chunkSize, chunkSize, chunkSize];
			boxColliders = new BoxCollider[chunkSize, chunkSize, chunkSize];
			init = true;
		}
	}

	public void AddCube(int x, int y, int z, byte tex)
	{
		x -= minX;
		y -= minY;
		z -= minZ;
		if (map[x, y, z] != 0)
		{
			Debug.LogWarning("Chunk already exists");
			return;
		}
		map[x, y, z] = tex;
		if (boxColliders[x, y, z] != null)
		{
			boxColliders[x, y, z].enabled = true;
		}
		else
		{
			BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
			boxCollider.center = new Vector3((float)x + 0.5f, (float)y + 0.5f, (float)z + 0.5f);
			boxCollider.size = Vector3.one;
			boxColliders[x, y, z] = boxCollider;
		}
		UpdateMesh();
		UpdateNextCube(x, y, z);
	}

	public void RemoveCube(int x, int y, int z, byte tex)
	{
		x -= minX;
		y -= minY;
		z -= minZ;
		if (map[x, y, z] == 0)
		{
			Debug.LogWarning("Chunk no exists");
			return;
		}
		map[x, y, z] = 0;
		boxColliders[x, y, z].enabled = false;
		UpdateMesh();
		UpdateNextCube(x, y, z);
	}

	public bool IsTransparent(int x, int y, int z)
	{
		if (GetByte(x, y, z) == 0)
		{
			return true;
		}
		return false;
	}

	public byte GetByte(int x, int y, int z)
	{
		if (x < 0 || y < 0 || z < 0 || x >= ChunkManager.chunkSize || y >= ChunkManager.chunkSize || z >= ChunkManager.chunkSize)
		{
			Chunk chunk = ChunkManager.FindChunk(x + minX, y + minY, z + minZ);
			if (chunk == this)
			{
				return 0;
			}
			if (chunk == null)
			{
				return 0;
			}
			return chunk.GetByte(x + minX - chunk.minX, y + minY - chunk.minY, z + minZ - chunk.minZ);
		}
		return map[x, y, z];
	}

	private void UpdateNextCube(int x, int y, int z)
	{
		if (x == 0)
		{
			Chunk chunk = ChunkManager.FindChunk(x - 1 + minX, y + minY, z + minZ);
			if (chunk != null)
			{
				chunk.UpdateMesh();
			}
		}
		if (x == ChunkManager.chunkSize - 1)
		{
			Chunk chunk2 = ChunkManager.FindChunk(x + 1 + minX, y + minY, z + minZ);
			if (chunk2 != null)
			{
				chunk2.UpdateMesh();
			}
		}
		if (y == 0)
		{
			Chunk chunk3 = ChunkManager.FindChunk(x + minX, y - 1 + minY, z + minZ);
			if (chunk3 != null)
			{
				chunk3.UpdateMesh();
			}
		}
		if (y == ChunkManager.chunkSize - 1)
		{
			Chunk chunk4 = ChunkManager.FindChunk(x + minX, y + 1 + minY, z + minZ);
			if (chunk4 != null)
			{
				chunk4.UpdateMesh();
			}
		}
		if (z == 0)
		{
			Chunk chunk5 = ChunkManager.FindChunk(x + minX, y + minY, z - 1 + minZ);
			if (chunk5 != null)
			{
				chunk5.UpdateMesh();
			}
		}
		if (z == ChunkManager.chunkSize - 1)
		{
			Chunk chunk6 = ChunkManager.FindChunk(x + minX, y + minY, z + 1 + minZ);
			if (chunk6 != null)
			{
				chunk6.UpdateMesh();
			}
		}
	}

	public void UpdateMesh()
	{
		vertices.Clear();
		triangles.Clear();
		uvs.Clear();
		for (int i = 0; i < ChunkManager.chunkSize; i++)
		{
			for (int j = 0; j < ChunkManager.chunkSize; j++)
			{
				for (int k = 0; k < ChunkManager.chunkSize; k++)
				{
					if (map[i, j, k] != 0)
					{
						byte brick = map[i, j, k];
						if (IsTransparent(i + 1, j, k))
						{
							BuildFace(brick, new Vector3(i + 1, j, k), Vector3.up, Vector3.forward, true);
						}
						if (IsTransparent(i - 1, j, k))
						{
							BuildFace(brick, new Vector3(i, j, k), Vector3.up, Vector3.forward, false);
						}
						if (IsTransparent(i, j + 1, k))
						{
							BuildFace(brick, new Vector3(i, j + 1, k), Vector3.forward, Vector3.right, true);
						}
						if (IsTransparent(i, j - 1, k))
						{
							BuildFace(brick, new Vector3(i, j, k), Vector3.forward, Vector3.right, false);
						}
						if (IsTransparent(i, j, k - 1))
						{
							BuildFace(brick, new Vector3(i, j, k), Vector3.up, Vector3.right, true);
						}
						if (IsTransparent(i, j, k + 1))
						{
							BuildFace(brick, new Vector3(i, j, k + 1), Vector3.up, Vector3.right, false);
						}
					}
				}
			}
		}
		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}

	public void UpdateColliders()
	{
		for (int i = 0; i < ChunkManager.chunkSize; i++)
		{
			for (int j = 0; j < ChunkManager.chunkSize; j++)
			{
				for (int k = 0; k < ChunkManager.chunkSize; k++)
				{
					if (map[i, j, k] == 0)
					{
						if (boxColliders[i, j, k] != null)
						{
							boxColliders[i, j, k].enabled = false;
						}
					}
					else if (boxColliders[i, j, k] != null)
					{
						boxColliders[i, j, k].enabled = true;
					}
					else
					{
						BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
						boxCollider.center = new Vector3((float)i + 0.5f, (float)j + 0.5f, (float)k + 0.5f);
						boxCollider.size = Vector3.one;
						boxColliders[i, j, k] = boxCollider;
					}
				}
			}
		}
	}

	public void BuildFace(byte brick, Vector3 corner, Vector3 up, Vector3 right, bool reversed)
	{
		int count = vertices.Count;
		vertices.Add(corner * ChunkManager.chunkScale);
		vertices.Add((corner + up) * ChunkManager.chunkScale);
		vertices.Add((corner + up + right) * ChunkManager.chunkScale);
		vertices.Add((corner + right) * ChunkManager.chunkScale);
		uvs.Add(new Vector2(0f, 0f));
		uvs.Add(new Vector2(0f, 0.0625f));
		uvs.Add(new Vector2(0.0625f, 0.0625f));
		uvs.Add(new Vector2(0.0625f, 0f));
		if (reversed)
		{
			triangles.Add(count);
			triangles.Add(count + 1);
			triangles.Add(count + 2);
			triangles.Add(count + 2);
			triangles.Add(count + 3);
			triangles.Add(count);
		}
		else
		{
			triangles.Add(count + 1);
			triangles.Add(count);
			triangles.Add(count + 2);
			triangles.Add(count + 3);
			triangles.Add(count + 2);
			triangles.Add(count);
		}
	}
}
