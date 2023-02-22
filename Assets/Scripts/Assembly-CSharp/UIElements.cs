using System;
using UnityEngine;

public class UIElements : MonoBehaviour
{
	[Serializable]
	public class Element
	{
		public string key;

		public GameObject target;

		[HideInInspector]
		public UnityEngine.Object comp;
	}

	public Element[] list;

	private static UIElements instance;

	private void Awake()
	{
		instance = this;
	}

	public static T Get<T>(string key) where T : Component
	{
		for (int i = 0; i < instance.list.Length; i++)
		{
			if (!(instance.list[i].key == key))
			{
				continue;
			}
			if (instance.list[i].comp == null)
			{
				T component = instance.list[i].target.GetComponent<T>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					instance.list[i].comp = component;
					return component;
				}
				return (T)null;
			}
			return instance.list[i].comp as T;
		}
		return (T)null;
	}
}
