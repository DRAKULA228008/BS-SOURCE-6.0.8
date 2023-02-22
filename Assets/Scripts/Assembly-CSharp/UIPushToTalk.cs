using System.Collections.Generic;
using UnityEngine;

public class UIPushToTalk : MonoBehaviour
{
	public UILabel Element;

	public UIGrid Grid;

	private List<UILabel> ElementPool = new List<UILabel>();

	private static UIPushToTalk instance;

	private void Start()
	{
		instance = this;
	}

	public static void Add(string playerName, float duration)
	{
		UILabel label;
		if (instance.ElementPool.Count == 0)
		{
			label = instance.Grid.gameObject.AddChild(instance.Element.gameObject).GetComponent<UILabel>();
		}
		else
		{
			label = instance.ElementPool[0];
			instance.ElementPool.RemoveAt(0);
		}
		label.cachedGameObject.SetActive(true);
		label.text = playerName;
		instance.Grid.repositionNow = true;
		TimerManager.In(duration, delegate
		{
			label.cachedGameObject.SetActive(false);
			instance.Grid.repositionNow = true;
			instance.ElementPool.Add(label);
		});
	}
}
