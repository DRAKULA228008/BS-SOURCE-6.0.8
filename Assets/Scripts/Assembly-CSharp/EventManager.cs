using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
	public struct EventClass
	{
		public string key;

		public int id;

		public Delegate mDelegate;

		public bool dontDestroy;
	}

	public delegate void Callback();

	public delegate void Callback<T>(T arg1);

	public delegate void Callback<T, T2>(T arg1, T2 arg2);

	public delegate void Callback<T, T2, T3>(T arg1, T2 arg2, T3 arg3);

	public delegate T Getter<T>();

	public List<EventClass> Events = new List<EventClass>();

	public Dictionary<string, Delegate> Values = new Dictionary<string, Delegate>();

	public Dictionary<string, List<Delegate>> ValuesList = new Dictionary<string, List<Delegate>>();

	private static EventManager instance;

	private int nextID;

	private static void Init()
	{
		if (instance == null)
		{
			GameObject gameObject = new GameObject("EventManager");
			instance = gameObject.AddComponent<EventManager>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
	}

	private void OnLevelWasLoaded(int level)
	{
		ClearEventsAll();
		ClearValuesAll();
	}

	public static int AddListener(string key, Callback e)
	{
		Init();
		return instance.OnListenerAdding(key, e, false);
	}

	public static int AddListener(string key, Callback e, bool dontDestroy)
	{
		Init();
		return instance.OnListenerAdding(key, e, dontDestroy);
	}

	public static int AddListener<T>(string key, Callback<T> e)
	{
		Init();
		return instance.OnListenerAdding(key, e, false);
	}

	public static int AddListener<T>(string key, Callback<T> e, bool dontDestroy)
	{
		Init();
		return instance.OnListenerAdding(key, e, dontDestroy);
	}

	public static int AddListener<T, T2>(string key, Callback<T, T2> e)
	{
		Init();
		return instance.OnListenerAdding(key, e, false);
	}

	public static int AddListener<T, T2>(string key, Callback<T, T2> e, bool dontDestroy)
	{
		Init();
		return instance.OnListenerAdding(key, e, dontDestroy);
	}

	public static int AddListener<T, T2, T3>(string key, Callback<T, T2, T3> e)
	{
		Init();
		return instance.OnListenerAdding(key, e, false);
	}

	public static int AddListener<T, T2, T3>(string key, Callback<T, T2, T3> e, bool dontDestroy)
	{
		Init();
		return instance.OnListenerAdding(key, e, dontDestroy);
	}

	private int OnListenerAdding(string key, Delegate del, bool dontDestroy)
	{
		EventClass item = default(EventClass);
		item.key = key;
		item.id = GetNextID();
		item.mDelegate = del;
		item.dontDestroy = dontDestroy;
		Events.Add(item);
		return item.id;
	}

	public static void Dispatch(string key)
	{
		Init();
		for (int i = 0; i < instance.Events.Count; i++)
		{
			if (instance.Events[i].key == key)
			{
				Callback callback = instance.Events[i].mDelegate as Callback;
				if (callback != null)
				{
					callback();
				}
			}
		}
	}

	public static void Dispatch(int id)
	{
		Init();
		for (int i = 0; i < instance.Events.Count; i++)
		{
			if (instance.Events[i].id == id)
			{
				Callback callback = instance.Events[i].mDelegate as Callback;
				if (callback != null)
				{
					callback();
				}
				break;
			}
		}
	}

	public static void Dispatch<T>(string key, T e)
	{
		Init();
		for (int i = 0; i < instance.Events.Count; i++)
		{
			if (instance.Events[i].key == key)
			{
				Callback<T> callback = instance.Events[i].mDelegate as Callback<T>;
				if (callback != null)
				{
					callback(e);
				}
			}
		}
	}

	public static void Dispatch<T>(int id, T e)
	{
		Init();
		for (int i = 0; i < instance.Events.Count; i++)
		{
			if (instance.Events[i].id == id)
			{
				Callback<T> callback = instance.Events[i].mDelegate as Callback<T>;
				if (callback != null)
				{
					callback(e);
				}
				break;
			}
		}
	}

	public static void Dispatch<T, T2>(string key, T e, T2 e2)
	{
		Init();
		for (int i = 0; i < instance.Events.Count; i++)
		{
			if (instance.Events[i].key == key)
			{
				Callback<T, T2> callback = instance.Events[i].mDelegate as Callback<T, T2>;
				if (callback != null)
				{
					callback(e, e2);
				}
			}
		}
	}

	public static void Dispatch<T, T2>(int id, T e, T2 e2)
	{
		Init();
		for (int i = 0; i < instance.Events.Count; i++)
		{
			if (instance.Events[i].id == id)
			{
				Callback<T, T2> callback = instance.Events[i].mDelegate as Callback<T, T2>;
				if (callback != null)
				{
					callback(e, e2);
				}
				break;
			}
		}
	}

	public static void Dispatch<T, T2, T3>(string key, T e, T2 e2, T3 e3)
	{
		Init();
		for (int i = 0; i < instance.Events.Count; i++)
		{
			if (instance.Events[i].key == key)
			{
				Callback<T, T2, T3> callback = instance.Events[i].mDelegate as Callback<T, T2, T3>;
				if (callback != null)
				{
					callback(e, e2, e3);
				}
			}
		}
	}

	public static void Dispatch<T, T2, T3>(int id, T e, T2 e2, T3 e3)
	{
		Init();
		for (int i = 0; i < instance.Events.Count; i++)
		{
			if (instance.Events[i].id == id)
			{
				Callback<T, T2, T3> callback = instance.Events[i].mDelegate as Callback<T, T2, T3>;
				if (callback != null)
				{
					callback(e, e2, e3);
				}
				break;
			}
		}
	}

	public static void ClearEvent(string key)
	{
		if (instance == null)
		{
			return;
		}
		for (int i = 0; i < instance.Events.Count; i++)
		{
			if (instance.Events[i].key == key)
			{
				instance.Events.RemoveAt(i);
			}
		}
	}

	public static void ClearEvent(int id)
	{
		if (instance == null)
		{
			return;
		}
		for (int i = 0; i < instance.Events.Count; i++)
		{
			if (instance.Events[i].id == id)
			{
				instance.Events.RemoveAt(i);
				break;
			}
		}
	}

	public static void ClearEventsAll()
	{
		if (instance == null)
		{
			return;
		}
		for (int i = 0; i < instance.Events.Count; i++)
		{
			if (!instance.Events[i].dontDestroy)
			{
				instance.Events.RemoveAt(i);
				i--;
			}
		}
	}

	private int GetNextID()
	{
		nextID++;
		return nextID;
	}

	public static void SetValue<T>(string key, Getter<T> e)
	{
		Init();
		instance.Values.Add(key, e);
	}

	public static T GetValue<T>(string key)
	{
		Init();
		if (instance.Values.ContainsKey(key))
		{
			Getter<T> getter = instance.Values[key] as Getter<T>;
			if (getter != null)
			{
				return getter();
			}
		}
		return default(T);
	}

	public static bool HasValue(string key)
	{
		Init();
		return instance.Values.ContainsKey(key);
	}

	public static void ClearValue(string key)
	{
		Init();
		instance.Values.Remove(key);
	}

	public static void ClearValuesAll()
	{
		Init();
		instance.Values.Clear();
	}

	public static void SetValueList<T>(string key, Getter<T> e)
	{
		Init();
		if (!instance.ValuesList.ContainsKey(key))
		{
			instance.ValuesList.Add(key, new List<Delegate>());
		}
		instance.ValuesList[key].Add(e);
	}

	public static T GetValueList<T>(string key)
	{
		Init();
		if (instance.ValuesList.ContainsKey(key))
		{
			Getter<T> getter = instance.Values[key] as Getter<T>;
			if (getter != null)
			{
				return getter();
			}
		}
		return default(T);
	}
}
