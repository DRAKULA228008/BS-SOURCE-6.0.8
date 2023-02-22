using System.Collections.Generic;
using UnityEngine;

public class PhotonPoolManager : MonoBehaviour, IPunPrefabPool
{
	public string[] prefabID;

	public GameObject[] prefabs;

	private List<string> listID = new List<string>();

	private List<GameObject> list = new List<GameObject>();

	private List<string> poolID = new List<string>();

	public List<GameObject> pool = new List<GameObject>();

	private static PhotonPoolManager instance;

	private void Start()
	{
		if (instance != null)
		{
			Destroy(base.gameObject);
			return;
		}
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject;
		Transform transform;
		for (int i = 0; i < pool.Count; i++)
		{
			if (poolID[i] == prefabId)
			{
				gameObject = pool[i];
				pool.RemoveAt(i);
				poolID.RemoveAt(i);
				listID.Add(prefabId);
				list.Add(gameObject);
				transform = gameObject.transform;
				transform.position = position;
				transform.rotation = rotation;
				return gameObject;
			}
		}
		for (int j = 0; j < prefabs.Length; j++)
		{
			if (prefabID[j] == prefabId)
			{
				gameObject = (GameObject)Object.Instantiate(prefabs[j]);
				gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
				listID.Add(prefabId);
				list.Add(gameObject);
				transform = gameObject.transform;
				transform.position = position;
				transform.rotation = rotation;
				return gameObject;
			}
		}
		Debug.LogWarning("No Photon Pool: " + prefabId);
		gameObject = (GameObject)Resources.Load(prefabId, typeof(GameObject));
		listID.Add(prefabId);
		list.Add(gameObject);
		transform = gameObject.transform;
		transform.position = position;
		transform.rotation = rotation;
		return gameObject;
	}

	public void Destroy(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		go.SetActive(false);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == go)
			{
				list.RemoveAt(i);
				string item = listID[i];
				listID.RemoveAt(i);
				pool.Add(go);
				poolID.Add(item);
				return;
			}
		}
		Destroy(go);
	}

	public static void ClearAll()
	{
		instance.listID.Clear();
		instance.list.Clear();
		instance.poolID.Clear();
		instance.pool.Clear();
	}

	private void OnLevelWasLoaded(int level)
	{
		ClearAll();
	}
}
