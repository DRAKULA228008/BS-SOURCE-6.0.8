using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
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

	public Vector3 spawnPosition
	{
		get
		{
			Vector3 position = cachedTransform.position;
			position.x += Random.Range((0f - spawnScale.x) / 2f, spawnScale.x / 2f);
			position.z += Random.Range((0f - spawnScale.z) / 2f, spawnScale.z / 2f);
			return position;
		}
		set
		{
			cachedTransform.position = value;
		}
	}

	public Vector3 spawnRotation
	{
		get
		{
			return cachedTransform.eulerAngles;
		}
		set
		{
			cachedTransform.eulerAngles = value;
		}
	}

	public Vector3 spawnScale
	{
		get
		{
			return cachedTransform.localScale;
		}
		set
		{
			cachedTransform.localScale = value;
		}
	}
}
