using System.Collections.Generic;
using UnityEngine;

public class UIPanelManager : MonoBehaviour
{
	public List<GameObject> list = new List<GameObject>();

	private static UIPanelManager instance;

	private void Awake()
	{
		instance = this;
	}

	public static void ShowPanel(string panel)
	{
		for (int i = 0; i < instance.list.Count; i++)
		{
			if (instance.list[i].name == panel)
			{
				instance.list[i].SetActive(true);
			}
			else
			{
				instance.list[i].SetActive(false);
			}
		}
	}

	public void Show(string panel)
	{
		ShowPanel(panel);
	}

	public static GameObject Get(string panel)
	{
		for (int i = 0; i < instance.list.Count; i++)
		{
			if (instance.list[i].name == panel)
			{
				return instance.list[i];
			}
		}
		return null;
	}

	public static void HideAll()
	{
		for (int i = 0; i < instance.list.Count; i++)
		{
			instance.list[i].SetActive(false);
		}
	}
}
