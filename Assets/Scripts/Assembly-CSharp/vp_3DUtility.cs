using UnityEngine;

public static class vp_3DUtility
{
	public static Vector3 HorizontalVector(Vector3 value)
	{
		value.y = 0f;
		return value;
	}

	public static bool OnScreen(Camera camera, Renderer renderer, Vector3 worldPosition, out Vector3 screenPosition)
	{
		screenPosition = Vector2.zero;
		if (camera == null || renderer == null || !renderer.isVisible)
		{
			return false;
		}
		screenPosition = camera.WorldToScreenPoint(worldPosition);
		if (screenPosition.z < 0f)
		{
			return false;
		}
		return true;
	}

	public static bool InLineOfSight(Vector3 from, Transform target, Vector3 targetOffset, int layerMask)
	{
		RaycastHit hitInfo;
		Physics.Linecast(from, target.position + targetOffset, out hitInfo, layerMask);
		if (hitInfo.collider == null || hitInfo.collider.transform.root == target)
		{
			return true;
		}
		return false;
	}

	public static bool WithinRange(Vector3 from, Vector3 to, float range, out float distance)
	{
		distance = Vector3.Distance(from, to);
		if (distance > range)
		{
			return false;
		}
		return true;
	}

	public static float DistanceToRay(Ray ray, Vector3 point)
	{
		return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
	}

	public static float LookAtAngle(Vector3 sourcePosition, Vector3 sourceDirection, Vector3 targetPosition)
	{
		return Mathf.Acos(Vector3.Dot((sourcePosition - targetPosition).normalized, -sourceDirection)) * 57.29578f;
	}
}
