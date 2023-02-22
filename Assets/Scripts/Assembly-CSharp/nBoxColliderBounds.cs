using UnityEngine;

public class nBoxColliderBounds : MonoBehaviour
{
	public Vector3 center = Vector3.zero;

	public Vector3 size = Vector3.one;

	public Renderer meshRenderer;

	[HideInInspector]
	public float distance = 1000f;

	public bool gizmo = true;

	private Bounds bounds = default(Bounds);

	private Transform mTransform;

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

	private void Start()
	{
		bounds.size = size;
	}

	public bool Raycast(Ray ray, float maxDistance)
	{
		if (!meshRenderer.isVisible)
		{
			distance = 1000f;
			return false;
		}
		bounds.center = center + cachedTransform.position;
		if (bounds.IntersectRay(ray, out distance) && distance <= maxDistance)
		{
			return true;
		}
		distance = 1000f;
		return false;
	}
}
