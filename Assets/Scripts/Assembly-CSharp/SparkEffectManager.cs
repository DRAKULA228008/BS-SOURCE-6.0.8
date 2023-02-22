using UnityEngine;

public class SparkEffectManager : MonoBehaviour
{
	public ParticleSystem Spark;

	private Transform mTransform;

	private bool Active;

	private static SparkEffectManager instance;

	private Transform cacheTransform
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
		instance = this;
	}

	public static void ClearParent()
	{
		instance.cacheTransform.SetParent(null);
		instance.Active = false;
	}

	public static void SetParent(Transform parent, Vector3 pos)
	{
		if (parent == null)
		{
			instance.cacheTransform.SetParent(null);
			instance.Active = false;
			return;
		}
		instance.cacheTransform.SetParent(parent);
		instance.cacheTransform.localPosition = pos;
		instance.cacheTransform.localEulerAngles = new Vector3(0f, -4f, 0f);
		instance.Active = true;
	}

	public static void Fire(Vector3 point, float distance)
	{
		if (Settings.ProjectileEffect && distance > 2.5f && instance.Active && Random.value > 0.2f)
		{
			instance.cacheTransform.LookAt(point);
			instance.Spark.Emit(1);
		}
	}
}
