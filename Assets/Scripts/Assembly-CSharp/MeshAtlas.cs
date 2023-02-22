using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Atlas3D/MeshAtlas")]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshAtlas : MonoBehaviour
{
	public static UIAtlas lastAtlas;

	public static string lastSpriteName;

	public Mesh originalMesh;

	private Mesh atlasMesh;

	private MeshFilter mMeshFilter;

	private MeshRenderer mMeshRenderer;

	public Vector3 mScale = Vector3.one;

	public Vector3 mPivot = Vector3.zero;

	public Rect mSpriteSize = new Rect(0f, 0f, 1f, 1f);

	public bool mMirrorX;

	public bool mMirrorY;

	public bool mMirrorZ;

	public bool mFlip;

	public Color mColor = Color.white;

	public Material originalMaterial;

	public Material atlasMaterial;

	public UIAtlas mAtlas;

	public string mSpriteName;

	private GameObject mGameObject;

	private Transform mTransform;

	public GameObject cachedGameObject
	{
		get
		{
			if (mGameObject == null)
			{
				mGameObject = base.gameObject;
			}
			return mGameObject;
		}
	}

	public Transform cachedTransform
	{
		get
		{
			if (mTransform == null)
			{
				mTransform = base.transform;
			}
			return mTransform;
		}
	}

	public Vector3 scale
	{
		get
		{
			return mScale;
		}
		set
		{
			if (mScale != value)
			{
				mScale = value;
				UpdateVertices();
			}
		}
	}

	public Vector3 pivot
	{
		get
		{
			return mPivot;
		}
		set
		{
			if (mPivot != value)
			{
				mPivot = value;
				UpdateVertices();
			}
		}
	}

	public Rect spriteSize
	{
		get
		{
			return mSpriteSize;
		}
		set
		{
			if (mSpriteSize != value)
			{
				mSpriteSize = value;
				UpdateUVs();
			}
		}
	}

	public bool mirrorX
	{
		get
		{
			return mMirrorX;
		}
		set
		{
			if (mMirrorX != value)
			{
				mMirrorX = value;
				UpdateVertices();
			}
		}
	}

	public bool mirrorY
	{
		get
		{
			return mMirrorY;
		}
		set
		{
			if (mMirrorY != value)
			{
				mMirrorY = value;
				UpdateVertices();
			}
		}
	}

	public bool mirrorZ
	{
		get
		{
			return mMirrorZ;
		}
		set
		{
			if (mMirrorZ != value)
			{
				mMirrorZ = value;
				UpdateVertices();
			}
		}
	}

	public bool flip
	{
		get
		{
			return mFlip;
		}
		set
		{
			if (mFlip != value)
			{
				mFlip = value;
				UpdateFlip();
			}
		}
	}

	public Color color
	{
		get
		{
			return mColor;
		}
		set
		{
			if (mColor != value)
			{
				mColor = value;
				UpdateColor();
			}
		}
	}

	public MeshFilter meshFilter
	{
		get
		{
			if (mMeshFilter == null)
			{
				mMeshFilter = GetComponent<MeshFilter>();
			}
			return mMeshFilter;
		}
	}

	public MeshRenderer meshRenderer
	{
		get
		{
			if (mMeshRenderer == null)
			{
				mMeshRenderer = GetComponent<MeshRenderer>();
			}
			return mMeshRenderer;
		}
	}

	public UIAtlas atlas
	{
		get
		{
			return mAtlas;
		}
		set
		{
			if (!(mAtlas != value))
			{
				return;
			}
			mAtlas = value;
			if (mAtlas != null)
			{
				lastAtlas = value;
				atlasMaterial = mAtlas.spriteMaterial;
				meshRenderer.sharedMaterial = atlasMaterial;
				if (string.IsNullOrEmpty(mSpriteName))
				{
					spriteName = lastSpriteName;
				}
				else
				{
					UpdateUVs();
				}
			}
		}
	}

	public string spriteName
	{
		get
		{
			return mSpriteName;
		}
		set
		{
			if (mSpriteName != value)
			{
				mSpriteName = value;
				lastSpriteName = value;
				UpdateUVs();
			}
		}
	}

	private void OnEnable()
	{
		UpdateMesh();
	}

	private void Reset()
	{
		DisableMesh();
		if (Application.isEditor)
		{
			UpdateMesh();
		}
	}

	public void UpdateMesh()
	{
		if (originalMesh == null)
		{
			originalMesh = meshFilter.mesh;
			originalMaterial = meshFilter.GetComponent<Renderer>().sharedMaterial;
			if (atlas == null && lastAtlas != null)
			{
				atlas = lastAtlas;
			}
		}
		if (base.enabled)
		{
			EnableMesh();
		}
		else
		{
			DisableMesh();
		}
	}

	public void EnableMesh()
	{
		if (!(atlas == null) && !(atlasMesh != null))
		{
			atlasMesh = new Mesh();
			atlasMesh.name = originalMesh.name + "_Atlas";
			UpdateVertices();
			UpdateFlip();
			atlasMesh.uv2 = originalMesh.uv2;
			atlasMesh.normals = originalMesh.normals;
			UpdateColor();
			atlasMesh.tangents = originalMesh.tangents;
			UpdateUVs();
			if (atlasMaterial == null)
			{
				atlasMaterial = atlas.spriteMaterial;
			}
			meshFilter.GetComponent<Renderer>().sharedMaterial = atlasMaterial;
			meshFilter.mesh = atlasMesh;
		}
	}

	public void DisableMesh()
	{
		if (atlasMesh != null)
		{
			atlasMaterial = meshFilter.GetComponent<Renderer>().sharedMaterial;
			Object.DestroyImmediate(atlasMesh);
			atlasMesh = null;
		}
		if (originalMesh != null)
		{
			meshFilter.mesh = originalMesh;
		}
		if (originalMaterial != null)
		{
			meshFilter.GetComponent<Renderer>().sharedMaterial = originalMaterial;
		}
	}

	public void UpdateSettings()
	{
		UpdateVertices();
		UpdateFlip();
		atlasMesh.uv2 = originalMesh.uv2;
		atlasMesh.normals = originalMesh.normals;
		UpdateColor();
		atlasMesh.tangents = originalMesh.tangents;
		UpdateUVs();
	}

	private void UpdateUVs()
	{
		if (atlasMesh == null || atlas == null || string.IsNullOrEmpty(spriteName))
		{
			return;
		}
		UISpriteData sprite = atlas.GetSprite(spriteName);
		if (sprite != null && !(atlas.texture == null))
		{
			Rect rect = new Rect(sprite.x, sprite.y, sprite.width, sprite.height);
			Rect rect2 = NGUIMath.ConvertToTexCoords(rect, atlas.texture.width, atlas.texture.height);
			Vector2[] uv = originalMesh.uv;
			for (int i = 0; i < uv.Length; i++)
			{
				uv[i].x = uv[i].x * rect2.width * spriteSize.width + rect2.x + spriteSize.x;
				uv[i].y = uv[i].y * rect2.height * spriteSize.height + rect2.y + spriteSize.y;
			}
			atlasMesh.uv = uv;
		}
	}

	private void UpdateFlip()
	{
		if (!(atlasMesh == null))
		{
			if (flip)
			{
				atlasMesh.triangles = originalMesh.triangles.Reverse().ToArray();
			}
			else
			{
				atlasMesh.triangles = originalMesh.triangles;
			}
		}
	}

	private void UpdateVertices()
	{
		if (!(atlasMesh == null))
		{
			Vector3[] vertices = originalMesh.vertices;
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i].x = (vertices[i].x + mPivot.x) * mScale.x * (float)((!mMirrorX) ? 1 : (-1));
				vertices[i].y = (vertices[i].y + mPivot.y) * mScale.y * (float)((!mMirrorY) ? 1 : (-1));
				vertices[i].z = (vertices[i].z + mPivot.z) * mScale.z * (float)((!mMirrorZ) ? 1 : (-1));
			}
			atlasMesh.vertices = vertices;
		}
	}

	private void UpdateColor()
	{
		if (!(atlasMesh == null))
		{
			Color[] array = new Color[atlasMesh.uv.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = mColor;
			}
			atlasMesh.colors = array;
		}
	}

	public bool HasSprite(string spriteName)
	{
		if (atlas != null)
		{
			return false;
		}
		return (atlas.GetSprite(spriteName) != null) ? true : false;
	}
}
