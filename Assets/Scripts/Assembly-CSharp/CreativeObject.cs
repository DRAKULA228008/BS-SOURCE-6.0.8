using UnityEngine;

public class CreativeObject : MonoBehaviour
{
	public byte id;

	public MeshAtlas[] meshAtlases;

	private Transform mTransform;

	private GameObject mGameObject;

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

	public void UpdateFaces(byte spriteID)
	{
		UpdateFaces(spriteID, true);
	}

	public void UpdateFaces(byte spriteID, bool checkFace)
	{
		id = spriteID;
		for (int i = 0; i < meshAtlases.Length; i++)
		{
			if (checkFace)
			{
				RaycastHit hitInfo;
				if (Physics.Raycast(cachedTransform.position, -meshAtlases[i].cachedTransform.forward, out hitInfo, 1.1f))
				{
					if (hitInfo.transform.CompareTag("CreativeObject"))
					{
						if (hitInfo.transform.name != "Plane")
						{
							hitInfo.transform.GetComponent<Renderer>().enabled = false;
						}
						meshAtlases[i].meshRenderer.enabled = false;
					}
					else
					{
						meshAtlases[i].meshRenderer.enabled = true;
					}
				}
				else
				{
					meshAtlases[i].meshRenderer.enabled = true;
				}
			}
			meshAtlases[i].spriteName = spriteID.ToString();
		}
	}

	public void Delete()
	{
		Delete(true);
	}

	public void Delete(bool check)
	{
		if (check)
		{
			for (int i = 0; i < meshAtlases.Length; i++)
			{
				RaycastHit hitInfo;
				if (Physics.Raycast(cachedTransform.position, -meshAtlases[i].cachedTransform.forward, out hitInfo, 1.1f) && hitInfo.transform.CompareTag("CreativeObject") && hitInfo.transform.name != "Plane")
				{
					hitInfo.transform.GetComponent<Renderer>().enabled = true;
				}
			}
		}
		PoolManager.Despawn("Block ", cachedGameObject);
	}
}
