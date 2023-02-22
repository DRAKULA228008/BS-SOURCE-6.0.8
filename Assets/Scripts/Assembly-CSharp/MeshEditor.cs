using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class MeshEditor : MonoBehaviour
{
	public Mesh originalMesh;

	private Mesh editMesh;

	private MeshFilter mMeshFilter;

	public Vector3 mScale = Vector3.one;

	public Vector3 mPivot = Vector3.zero;

	public bool mMirrorX;

	public bool mMirrorY;

	public bool mMirrorZ;

	public bool mFlip;

	public Rect mUVRect = new Rect(0f, 0f, 1f, 1f);

	public Color mColor = Color.white;

	private GameObject mGameObject;

	private Transform mTransform;

	private MeshRenderer mMeshRenderer;

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

	public Rect uvRect
	{
		get
		{
			return mUVRect;
		}
		set
		{
			if (mUVRect != value)
			{
				mUVRect = value;
				UpdateUV();
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
		editMesh = new Mesh();
		editMesh.name = originalMesh.name + "_Editor";
		UpdateVertices();
		UpdateFlip();
		editMesh.uv = originalMesh.uv;
		editMesh.uv2 = originalMesh.uv2;
		editMesh.normals = originalMesh.normals;
		editMesh.tangents = originalMesh.tangents;
		UpdateUV();
		UpdateColor();
		meshFilter.mesh = editMesh;
	}

	public void DisableMesh()
	{
		if (editMesh != null)
		{
			Object.DestroyImmediate(editMesh);
			editMesh = null;
		}
		if (originalMesh != null)
		{
			meshFilter.mesh = originalMesh;
		}
	}

	public void UpdateSettings()
	{
		UpdateVertices();
		UpdateFlip();
		UpdateUV();
		UpdateColor();
	}

	private void UpdateFlip()
	{
		if (!(editMesh == null))
		{
			if (flip)
			{
				editMesh.triangles = originalMesh.triangles.Reverse().ToArray();
			}
			else
			{
				editMesh.triangles = originalMesh.triangles;
			}
		}
	}

	private void UpdateVertices()
	{
		if (!(editMesh == null))
		{
			Vector3[] vertices = originalMesh.vertices;
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i].x = (vertices[i].x + mPivot.x) * mScale.x * (float)((!mMirrorX) ? 1 : (-1));
				vertices[i].y = (vertices[i].y + mPivot.y) * mScale.y * (float)((!mMirrorY) ? 1 : (-1));
				vertices[i].z = (vertices[i].z + mPivot.z) * mScale.z * (float)((!mMirrorZ) ? 1 : (-1));
			}
			editMesh.vertices = vertices;
		}
	}

	private void UpdateUV()
	{
		if (!(editMesh == null))
		{
			Vector2[] uv = originalMesh.uv;
			for (int i = 0; i < uv.Length; i++)
			{
				uv[i].x = uv[i].x * uvRect.width + uvRect.x;
				uv[i].y = uv[i].y * uvRect.height + uvRect.y;
			}
			editMesh.uv = uv;
		}
	}

	private void UpdateColor()
	{
		if (!(editMesh == null))
		{
			Color[] array = new Color[originalMesh.uv.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = mColor;
			}
			editMesh.colors = array;
		}
	}
}
