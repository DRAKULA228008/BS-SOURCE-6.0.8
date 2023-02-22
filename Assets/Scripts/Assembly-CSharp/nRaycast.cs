using UnityEngine;

public static class nRaycast
{
	public static nBoxCollider hitBoxCollider;

	public static float hitDistance;

	public static Vector3 hitPoint;

	public static nColliderContainer container;

	private static RaycastHit hitInfo;

	private static Vector3 e1;

	private static Vector3 e2;

	private static Vector3 p;

	private static Vector3 q;

	private static Vector3 t;

	private static float det;

	private static float invDet;

	private static float u;

	private static float v;

	private static float result;

	public static bool RaycastName(Ray ray, float maxDistance)
	{
		nProfiler.BeginSample("nRaycast.RaycastName");
		nColliderContainer.RaycastName(ray, maxDistance);
		if (nColliderContainer.container == null)
		{
			nProfiler.EndSample();
			return false;
		}
		if (Physics.Raycast(ray, out hitInfo, maxDistance) && hitInfo.distance < nColliderContainer.container.nameCollider.distance)
		{
			nProfiler.EndSample();
			return false;
		}
		container = nColliderContainer.container;
		nProfiler.EndSample();
		return true;
	}

	public static bool RaycastFire(Ray ray, float maxDistance, int layerMask)
	{
		nProfiler.BeginSample("nRaycast.RaycastFire");
		nColliderContainer.RaycastFire(ray, maxDistance);
		if (nColliderContainer.container == null || nColliderContainer.playerBoxCollider == null)
		{
			nProfiler.EndSample();
			return false;
		}
		if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask) && hitInfo.distance < nColliderContainer.playerBoxCollider.distance)
		{
			nProfiler.EndSample();
			return false;
		}
		container = nColliderContainer.container;
		hitDistance = nColliderContainer.playerBoxCollider.distance;
		hitPoint = ray.GetPoint(hitDistance);
		hitBoxCollider = nColliderContainer.playerBoxCollider;
		nProfiler.EndSample();
		return true;
	}

	public static float Intersect(Vector3 p1, Vector3 p2, Vector3 p3, Ray ray)
	{
		e1 = p2 - p1;
		e2 = p3 - p1;
		p = Vector3.Cross(ray.direction, e2);
		det = Vector3.Dot(e1, p);
		if (det > -1.401298E-45f && det < float.Epsilon)
		{
			return float.MaxValue;
		}
		invDet = 1f / det;
		t = ray.origin - p1;
		u = Vector3.Dot(t, p) * invDet;
		if (u < 0f || u > 1f)
		{
			return float.MaxValue;
		}
		q = Vector3.Cross(t, e1);
		v = Vector3.Dot(ray.direction, q) * invDet;
		if (v < 0f || u + v > 1f)
		{
			return float.MaxValue;
		}
		result = Vector3.Dot(e2, q) * invDet;
		if (result > float.Epsilon)
		{
			return result;
		}
		return float.MaxValue;
	}
}
