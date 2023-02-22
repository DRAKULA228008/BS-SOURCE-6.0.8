using System;
using UnityEngine;

public class nColliderContainer : MonoBehaviour
{
	[Serializable]
	public class Data
	{
		public nBoxColliderBounds mainCollider;

		public nBoxCollider[] otherColliders;
	}

	public static BetterList<nColliderContainer> list = new BetterList<nColliderContainer>();

	public static nColliderContainer container;

	private static nColliderContainer container2;

	public static nBoxCollider playerBoxCollider;

	public nBoxColliderBounds playerMain;

	public Data[] playerColliders;

	public nBoxColliderBounds nameCollider;

	public PlayerSkin playerSkin;

	private bool actived;

	public static void RaycastFire(Ray ray, float maxDistance)
	{
		container = null;
		container2 = null;
		playerBoxCollider = null;
		float num = 1000f;
		for (int i = 0; i < list.size; i++)
		{
			if (list[i].actived && list[i].playerMain.Raycast(ray, maxDistance) && num > list[i].playerMain.distance)
			{
				if (container != null)
				{
					container2 = container;
				}
				num = list[i].playerMain.distance;
				container = list[i];
			}
		}
		if (!(container == null) && !RaycastFire2(ray, maxDistance) && container2 != null)
		{
			container = container2;
			RaycastFire2(ray, maxDistance);
		}
	}

	private static bool RaycastFire2(Ray ray, float maxDistance)
	{
		float num = 1000f;
		Data data = null;
		Data data2 = null;
		for (int i = 0; i < container.playerColliders.Length; i++)
		{
			if (container.playerColliders[i].mainCollider.Raycast(ray, maxDistance) && num > container.playerColliders[i].mainCollider.distance)
			{
				num = container.playerColliders[i].mainCollider.distance;
				if (data != null)
				{
					data2 = data;
				}
				data = container.playerColliders[i];
			}
		}
		if (data == null)
		{
			return false;
		}
		for (int j = 0; j < data.otherColliders.Length; j++)
		{
			if (data.otherColliders[j].Raycast(ray, maxDistance))
			{
				playerBoxCollider = data.otherColliders[j];
				return true;
			}
		}
		if (data2 != null)
		{
			for (int k = 0; k < data2.otherColliders.Length; k++)
			{
				if (data2.otherColliders[k].Raycast(ray, maxDistance))
				{
					playerBoxCollider = data2.otherColliders[k];
					return true;
				}
			}
		}
		return false;
	}

	public static void RaycastName(Ray ray, float maxDistance)
	{
		container = null;
		float num = 1000f;
		for (int i = 0; i < list.size; i++)
		{
			if (list[i].actived && list[i].nameCollider.Raycast(ray, maxDistance) && num > list[i].nameCollider.distance)
			{
				num = list[i].nameCollider.distance;
				container = list[i];
			}
		}
	}

	private void Start()
	{
		list.Add(this);
	}

	private void OnEnable()
	{
		actived = true;
	}

	private void OnDisable()
	{
		actived = false;
	}

	private void OnDestroy()
	{
		list.Remove(this);
	}
}
