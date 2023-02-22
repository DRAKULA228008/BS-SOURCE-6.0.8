using UnityEngine;

[ExecuteInEditMode]
public class nBoxCollider : MonoBehaviour
{
	public Vector3 center = Vector3.zero;

	public Vector3 size = Vector3.one;

	public PlayerSkinDamage playerDamage;

	[HideInInspector]
	public float distance = 1000f;

	private Transform mTransform;

	private Vector3[] coordinates = new Vector3[8]
	{
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, -0.5f, -0.5f)
	};

	private Vector3[] box = new Vector3[8];

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

	public bool Raycast(Ray ray, float maxDistance)
	{
		UpdateCoordinates();
		distance = nRaycast.Intersect(box[0], box[3], box[1], ray);
		if (distance < maxDistance)
		{
			return true;
		}
		distance = nRaycast.Intersect(box[0], box[2], box[3], ray);
		if (distance < maxDistance)
		{
			return true;
		}
		distance = nRaycast.Intersect(box[2], box[5], box[3], ray);
		if (distance < maxDistance)
		{
			return true;
		}
		distance = nRaycast.Intersect(box[2], box[4], box[5], ray);
		if (distance < maxDistance)
		{
			return true;
		}
		distance = nRaycast.Intersect(box[4], box[7], box[5], ray);
		if (distance < maxDistance)
		{
			return true;
		}
		distance = nRaycast.Intersect(box[4], box[6], box[7], ray);
		if (distance < maxDistance)
		{
			return true;
		}
		distance = nRaycast.Intersect(box[6], box[1], box[7], ray);
		if (distance < maxDistance)
		{
			return true;
		}
		distance = nRaycast.Intersect(box[6], box[0], box[1], ray);
		if (distance < maxDistance)
		{
			return true;
		}
		distance = nRaycast.Intersect(box[1], box[5], box[7], ray);
		if (distance < maxDistance)
		{
			return true;
		}
		distance = nRaycast.Intersect(box[1], box[3], box[5], ray);
		if (distance < maxDistance)
		{
			return true;
		}
		distance = nRaycast.Intersect(box[6], box[2], box[0], ray);
		if (distance < maxDistance)
		{
			return true;
		}
		distance = nRaycast.Intersect(box[6], box[4], box[2], ray);
		if (distance < maxDistance)
		{
			return true;
		}
		return false;
	}

	public void UpdateCoordinates()
	{
		for (int i = 0; i < 8; i++)
		{
			box[i].x = (coordinates[i].x + center.x) * size.x;
			box[i].y = (coordinates[i].y + center.y) * size.y;
			box[i].z = (coordinates[i].z + center.z) * size.z;
			box[i] = cachedTransform.TransformPoint(box[i]);
		}
	}
}
