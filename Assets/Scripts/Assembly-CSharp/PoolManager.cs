using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	public class PoolObject
	{
		public GameObject prefab;

		public List<GameObject> cache = new List<GameObject>();
	}

	private Dictionary<string, PoolObject> list = new Dictionary<string, PoolObject>();

	private Transform mTransform;

	private static PoolManager instance;

	private Transform cachedTransform
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

	public static void ClearAll()
	{
		if (instance == null)
		{
			return;
		}
		foreach (KeyValuePair<string, PoolObject> item in instance.list)
		{
			for (int i = 0; i < item.Value.cache.Count; i++)
			{
				Object.Destroy(item.Value.cache[i]);
			}
		}
		instance.list.Clear();
	}

	private static void Init()
	{
		if (instance == null)
		{
			GameObject gameObject = new GameObject("PoolManager");
			instance = gameObject.AddComponent<PoolManager>();
			Object.DontDestroyOnLoad(gameObject);
		}
	}

	public static bool isContains(string key)
	{
		return instance.list.ContainsKey(key);
	}

	public static GameObject Spawn(string key, GameObject prefab)
	{
		return Spawn(key, prefab, Vector3.zero, Vector3.zero, null);
	}

	public static GameObject Spawn(string key, GameObject prefab, Vector3 position, Vector3 rotation)
	{
		return Spawn(key, prefab, position, rotation, null);
	}

	public static GameObject Spawn(string key, GameObject prefab, Vector3 position, Vector3 rotation, Transform parent)
	{
		Init();
		if (prefab == null)
		{
			Debug.LogError("Prefab null");
			return null;
		}
		if (!instance.list.ContainsKey(key))
		{
			PoolObject poolObject = new PoolObject();
			poolObject.prefab = prefab;
			instance.list.Add(key, poolObject);
		}
		PoolObject poolObject2 = instance.list[key];
		GameObject gameObject;
		if (poolObject2.cache.Count > 0)
		{
			gameObject = poolObject2.cache[0];
			poolObject2.cache.RemoveAt(0);
			Transform transform = gameObject.transform;
			transform.SetParent(parent, false);
			transform.localPosition = position;
			transform.localEulerAngles = rotation;
			gameObject.SetActive(true);
		}
		else
		{
			gameObject = (GameObject)Object.Instantiate(prefab, position, Quaternion.Euler(rotation));
			gameObject.name = prefab.name + " [Pool]";
			gameObject.transform.SetParent(parent, false);
			gameObject.SetActive(true);
		}
		return gameObject;
	}

	public static void Despawn(string key, GameObject prefab)
	{
		Init();
		if (prefab == null)
		{
			Debug.LogError("Prefab null");
			return;
		}
		if (!instance.list.ContainsKey(key))
		{
			PoolObject poolObject = new PoolObject();
			poolObject.prefab = prefab;
			instance.list.Add(key, poolObject);
		}
		instance.list[key].cache.Add(prefab);
		prefab.SetActive(false);
		prefab.transform.SetParent(instance.cachedTransform);
	}

	public static int CacheCount(string key)
	{
		Init();
		return instance.list[key].cache.Count;
	}
}
