using System;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
[ExecuteInEditMode]
public class PlayerSkinAtlas : MonoBehaviour
{
	[Serializable]
	public class SpriteData
	{
		public string sprite;

		public string lastSpriteName;
	}

	public static UIAtlas lastUsedAtlas;

	public Mesh originalMesh;

	private SkinnedMeshRenderer _skinnedMeshRenderer;

	private Mesh mesh;

	public Material originalMaterial;

	public Material atlasMaterial;

	public UIAtlas mAtlas;

	public string headSprite;

	public string headSpriteLast;

	public string bodySprite;

	public string bodySpriteLast;

	public string legsSprite;

	public string legsSpriteLast;

	public SkinnedMeshRenderer skinnedMeshRenderer
	{
		get
		{
			if (_skinnedMeshRenderer == null)
			{
				_skinnedMeshRenderer = base.gameObject.GetComponent<SkinnedMeshRenderer>();
			}
			return _skinnedMeshRenderer;
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
			if (!(mAtlas != null))
			{
				return;
			}
			lastUsedAtlas = value;
			atlasMaterial = mAtlas.spriteMaterial;
			skinnedMeshRenderer.sharedMaterial = atlasMaterial;
			if (string.IsNullOrEmpty(headSprite) || string.IsNullOrEmpty(bodySprite) || string.IsNullOrEmpty(legsSprite))
			{
				if (string.IsNullOrEmpty(headSprite))
				{
					SetSprite(PlayerSkinMember.Face, headSpriteLast);
				}
				if (string.IsNullOrEmpty(bodySprite))
				{
					SetSprite(PlayerSkinMember.Body, bodySpriteLast);
				}
				if (string.IsNullOrEmpty(legsSprite))
				{
					SetSprite(PlayerSkinMember.Legs, legsSpriteLast);
				}
			}
			else
			{
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
			originalMesh = base.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
			originalMaterial = skinnedMeshRenderer.sharedMaterial;
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
			mesh = (Mesh)UnityEngine.Object.Instantiate(originalMesh);
			mesh.name = originalMesh.name + "_Atlas";
			UpdateUVs();
			if (atlasMaterial != null)
			{
				skinnedMeshRenderer.sharedMaterial = atlasMaterial;
			}
			skinnedMeshRenderer.sharedMesh = mesh;
		}
	}

	public void DisableMesh()
	{
		if (mesh != null)
		{
			atlasMaterial = skinnedMeshRenderer.sharedMaterial;
			UnityEngine.Object.DestroyImmediate(mesh);
			mesh = null;
		}
		if (originalMesh != null)
		{
			skinnedMeshRenderer.sharedMesh = originalMesh;
		}
		if (originalMaterial != null)
		{
			skinnedMeshRenderer.sharedMaterial = originalMaterial;
		}
	}

	public void UpdateUVs()
	{
		if (!(mesh == null) && !(atlas == null))
		{
			Vector2[] uv = originalMesh.uv;
			uv = UpdatePlayerUV(PlayerSkinMember.Face, headSprite, uv);
			uv = UpdatePlayerUV(PlayerSkinMember.Body, bodySprite, uv);
			uv = UpdatePlayerUV(PlayerSkinMember.Legs, legsSprite, uv);
			mesh.uv = uv;
		}
	}

	public Vector2[] UpdatePlayerUV(PlayerSkinMember element, string sprite, Vector2[] playerUV)
	{
		if (string.IsNullOrEmpty(sprite))
		{
			return playerUV;
		}
		UISpriteData sprite2 = atlas.GetSprite(sprite);
		if (sprite2 == null)
		{
			return playerUV;
		}
		if (atlas.texture == null)
		{
			return playerUV;
		}
		Rect rect = new Rect(sprite2.x, sprite2.y, sprite2.width, sprite2.height);
		Rect rect2 = NGUIMath.ConvertToTexCoords(rect, atlas.texture.width, atlas.texture.height);
		for (int i = 0; i < playerUV.Length; i++)
		{
			switch (element)
			{
			case PlayerSkinMember.Face:
				if (i >= 428 && i <= 451)
				{
					playerUV[i].x = playerUV[i].x * rect2.width + rect2.x;
					playerUV[i].y = playerUV[i].y * rect2.height + rect2.y;
				}
				break;
			case PlayerSkinMember.Body:
				if (i >= 196 && i <= 427)
				{
					playerUV[i].x = playerUV[i].x * rect2.width + rect2.x;
					playerUV[i].y = playerUV[i].y * rect2.height + rect2.y;
				}
				break;
			case PlayerSkinMember.Legs:
				if ((i >= 0 && i <= 195) || (i >= 452 && i <= 527))
				{
					playerUV[i].x = playerUV[i].x * rect2.width + rect2.x;
					playerUV[i].y = playerUV[i].y * rect2.height + rect2.y;
				}
				break;
			}
		}
		return playerUV;
	}

	public string GetSprite(PlayerSkinMember member)
	{
		switch (member)
		{
		case PlayerSkinMember.Face:
			return headSprite;
		case PlayerSkinMember.Body:
			return bodySprite;
		case PlayerSkinMember.Legs:
			return legsSprite;
		default:
			return string.Empty;
		}
	}

	public void SetSprite(PlayerSkinMember member, string value)
	{
		string sprite = GetSprite(member);
		if (sprite != value)
		{
			switch (member)
			{
			case PlayerSkinMember.Face:
				headSprite = value;
				headSpriteLast = value;
				break;
			case PlayerSkinMember.Body:
				bodySprite = value;
				bodySpriteLast = value;
				break;
			case PlayerSkinMember.Legs:
				legsSprite = value;
				legsSpriteLast = value;
				break;
			}
			UpdateUVs();
		}
	}

	public void SetSprite(string head, string body, string legs)
	{
		if (headSprite != head)
		{
			headSprite = head;
			headSpriteLast = head;
		}
		if (bodySprite != body)
		{
			bodySprite = body;
			bodySpriteLast = body;
		}
		if (legsSprite != legs)
		{
			legsSprite = legs;
			legsSpriteLast = legs;
		}
		UpdateUVs();
	}

	public void UpdateAllMeshes()
	{
		PlayerSkinAtlas[] array = NGUITools.FindActive<PlayerSkinAtlas>();
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			PlayerSkinAtlas playerSkinAtlas = array[i];
			if (playerSkinAtlas.enabled && originalMesh == playerSkinAtlas.originalMesh)
			{
				playerSkinAtlas.UpdateMesh();
			}
		}
	}

	public void UpdateMeshTextures()
	{
	}
}
