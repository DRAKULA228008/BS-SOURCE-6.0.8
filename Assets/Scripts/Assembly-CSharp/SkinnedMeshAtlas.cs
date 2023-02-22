using UnityEngine;

[AddComponentMenu("NGUI/Atlas3D/SkinnedMeshAtlas")]
[ExecuteInEditMode]
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class SkinnedMeshAtlas : MonoBehaviour
{
	public static UIAtlas lastUsedAtlas;

	public Mesh originalMesh;

	private SkinnedMeshRenderer _mf;

	private Mesh mesh;

	public Material originalMaterial;

	public Material customMaterial;

	public UIAtlas mAtlas;

	public string mSpriteName;

	public UISpriteData mSprite;

	private bool mSpriteSet;

	public SkinnedMeshRenderer mf
	{
		get
		{
			if (_mf == null)
			{
				_mf = base.gameObject.GetComponent<SkinnedMeshRenderer>();
			}
			return _mf;
		}
	}

	[SerializeField]
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
			mSpriteSet = false;
			mSprite = null;
			if (!(mAtlas != null))
			{
				return;
			}
			lastUsedAtlas = value;
			customMaterial = mAtlas.spriteMaterial;
			mf.GetComponent<Renderer>().sharedMaterial = customMaterial;
			if (originalMaterial != null && !string.IsNullOrEmpty(originalMaterial.name))
			{
				for (int i = 0; i < mAtlas.spriteList.Count; i++)
				{
					if (mAtlas.spriteList[i].name == originalMaterial.name)
					{
						SetAtlasSprite(mAtlas.spriteList[i]);
						mSpriteName = mSprite.name;
						break;
					}
				}
			}
			if (string.IsNullOrEmpty(mSpriteName) && mAtlas != null && mAtlas.spriteList.Count > 0)
			{
				SetAtlasSprite(mAtlas.spriteList[0]);
				mSpriteName = mSprite.name;
			}
			if (!string.IsNullOrEmpty(mSpriteName))
			{
				string text = mSpriteName;
				mSpriteName = string.Empty;
				spriteName = text;
			}
		}
	}

	[SerializeField]
	public string spriteName
	{
		get
		{
			return mSpriteName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(mSpriteName))
				{
					mSpriteName = string.Empty;
					mSprite = null;
					mSpriteSet = false;
				}
			}
			else if (mSpriteName != value)
			{
				mSpriteName = value;
				mSprite = null;
				mSpriteSet = false;
				UpdateUVs();
			}
		}
	}

	public bool isValid
	{
		get
		{
			return GetAtlasSprite() != null;
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
			originalMesh = base.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
			originalMaterial = mf.GetComponent<Renderer>().sharedMaterial;
			if (atlas == null && lastUsedAtlas != null)
			{
				atlas = lastUsedAtlas;
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
		if (mesh == null)
		{
			mesh = (Mesh)Object.Instantiate(originalMesh);
			mesh.name = originalMesh.name + "_Atlas";
			UpdateUVs();
			if (customMaterial != null)
			{
				mf.GetComponent<Renderer>().sharedMaterial = customMaterial;
			}
			mf.sharedMesh = mesh;
		}
	}

	public void DisableMesh()
	{
		if (mesh != null)
		{
			customMaterial = mf.GetComponent<Renderer>().sharedMaterial;
			Object.DestroyImmediate(mesh);
			mesh = null;
		}
		if (originalMesh != null)
		{
			mf.sharedMesh = originalMesh;
		}
		if (originalMaterial != null)
		{
			mf.GetComponent<Renderer>().sharedMaterial = originalMaterial;
		}
	}

	public void UpdateUVs()
	{
		if (mesh == null || atlas == null || string.IsNullOrEmpty(spriteName))
		{
			return;
		}
		UISpriteData sprite = atlas.GetSprite(spriteName);
		if (sprite != null && !(atlas.texture == null))
		{
			mSprite = sprite;
			Rect rect = new Rect(mSprite.x, mSprite.y, mSprite.width, mSprite.height);
			Rect rect2 = NGUIMath.ConvertToTexCoords(rect, atlas.texture.width, atlas.texture.height);
			Vector2[] uv = originalMesh.uv;
			for (int i = 0; i < uv.Length; i++)
			{
				uv[i].x = uv[i].x * rect2.width + rect2.x;
				uv[i].y = uv[i].y * rect2.height + rect2.y;
			}
			mesh.uv = uv;
		}
	}

	public UISpriteData GetAtlasSprite()
	{
		if (!mSpriteSet)
		{
			mSprite = null;
		}
		if (mSprite == null && mAtlas != null)
		{
			if (!string.IsNullOrEmpty(mSpriteName))
			{
				UISpriteData sprite = mAtlas.GetSprite(mSpriteName);
				if (sprite == null)
				{
					return null;
				}
				SetAtlasSprite(sprite);
			}
			if (mSprite == null && mAtlas.spriteList.Count > 0)
			{
				UISpriteData uISpriteData = mAtlas.spriteList[0];
				if (uISpriteData == null)
				{
					return null;
				}
				SetAtlasSprite(uISpriteData);
				if (mSprite == null)
				{
					Debug.LogError(mAtlas.name + " seems to have a null sprite!");
					return null;
				}
				mSpriteName = mSprite.name;
			}
		}
		return mSprite;
	}

	private void SetAtlasSprite(UISpriteData sp)
	{
		mSpriteSet = true;
		if (sp != null)
		{
			mSprite = sp;
			mSpriteName = mSprite.name;
		}
		else
		{
			mSpriteName = ((mSprite == null) ? string.Empty : mSprite.name);
			mSprite = sp;
		}
	}

	public void UpdateAllMeshes()
	{
		MeshAtlas[] array = NGUITools.FindActive<MeshAtlas>();
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			MeshAtlas meshAtlas = array[i];
			if (meshAtlas.enabled && originalMesh == meshAtlas.originalMesh)
			{
				meshAtlas.UpdateMesh();
			}
		}
	}

	public void UpdateMeshTextures()
	{
	}
}
