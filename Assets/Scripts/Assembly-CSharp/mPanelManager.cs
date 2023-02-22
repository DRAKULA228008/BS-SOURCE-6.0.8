using System.Collections.Generic;
using UnityEngine;

public class mPanelManager : MonoBehaviour
{
	public List<UIPanel> panels = new List<UIPanel>();

	public UIPanel playerData;

	public UIPanel ignoreClick;

	public bool tween;

	public float duration = 0.2f;

	public static float tweenDuration = 0.2f;

	private UIPanel selectPanel;

	private UIPanel lastPanel;

	private static mPanelManager instance;

	public static UIPanel last
	{
		get
		{
			return instance.lastPanel;
		}
	}

	public static UIPanel select
	{
		get
		{
			return instance.selectPanel;
		}
	}

	public static bool isPlayerData
	{
		get
		{
			if (instance.tween)
			{
				return (instance.playerData.alpha == 1f) ? true : false;
			}
			return instance.playerData.cachedGameObject.activeSelf;
		}
		set
		{
			if (instance.tween)
			{
				TweenAlpha.Begin(instance.playerData.cachedGameObject, tweenDuration, value ? 1 : 0);
			}
			else
			{
				instance.playerData.cachedGameObject.SetActive(value);
			}
		}
	}

	private void Awake()
	{
		instance = this;
		selectPanel = panels[0];
		tweenDuration = duration;
	}

	public static void Show(string name, bool playerData)
	{
		if (select != null && select.name == name)
		{
			return;
		}
		ShowIgnoreClick(tweenDuration);
		isPlayerData = playerData;
		if (select != null)
		{
			for (int i = 0; i < instance.panels.Count; i++)
			{
				if (instance.tween && select == instance.panels[i])
				{
					UIPanel uIPanel = instance.panels[i];
					TweenAlpha.Begin(uIPanel.cachedGameObject, tweenDuration, 0f);
					continue;
				}
				if (instance.tween)
				{
					instance.panels[i].alpha = 0f;
				}
				instance.panels[i].cachedGameObject.SetActive(false);
			}
		}
		for (int j = 0; j < instance.panels.Count; j++)
		{
			if (instance.panels[j].name == name)
			{
				if (instance.selectPanel != null)
				{
					instance.lastPanel = instance.selectPanel;
				}
				instance.selectPanel = instance.panels[j];
				if (instance.tween)
				{
					instance.panels[j].alpha = 0f;
					TweenAlpha.Begin(instance.panels[j].cachedGameObject, tweenDuration, 1f);
				}
				else
				{
					instance.panels[j].alpha = 1f;
				}
				instance.panels[j].cachedGameObject.SetActive(true);
			}
		}
	}

	public void Show(string panel)
	{
		Show(panel, true);
	}

	public void Show(GameObject panel)
	{
		Show(panel.name, true);
	}

	public static void ShowIgnoreClick(float duration)
	{
		if (instance.tween)
		{
			instance.ignoreClick.cachedGameObject.SetActive(true);
			TimerManager.In(duration, delegate
			{
				instance.ignoreClick.cachedGameObject.SetActive(false);
			});
		}
	}

	public static void ShowTween(GameObject go)
	{
		if (instance.tween)
		{
			go.GetComponent<UIPanel>().alpha = 0f;
			TweenAlpha.Begin(go, tweenDuration, 1f);
		}
		else
		{
			go.GetComponent<UIPanel>().alpha = 1f;
		}
		go.SetActive(true);
	}

	public static void HideTween(GameObject go)
	{
		if (instance.tween)
		{
			go.GetComponent<UIPanel>().alpha = 1f;
			TweenAlpha.Begin(go, tweenDuration, 0f);
		}
		else
		{
			go.SetActive(false);
		}
	}

	public void ShowAnim(GameObject go)
	{
		ShowTween(go);
	}

	public void HideAnim(GameObject go)
	{
		HideTween(go);
	}

	public static void Hide()
	{
		for (int i = 0; i < instance.panels.Count; i++)
		{
			if (instance.selectPanel != null && instance.selectPanel == instance.panels[i])
			{
				UIPanel panel = instance.panels[i];
				if (instance.tween)
				{
					HideTween(panel.cachedGameObject);
					TimerManager.In(tweenDuration, delegate
					{
						panel.alpha = 0f;
						panel.cachedGameObject.SetActive(false);
					});
				}
				else
				{
					panel.cachedGameObject.SetActive(false);
				}
			}
			else
			{
				if (instance.tween)
				{
					instance.panels[i].alpha = 0f;
				}
				instance.panels[i].cachedGameObject.SetActive(false);
			}
		}
		instance.selectPanel = null;
		isPlayerData = false;
	}

	public void SetPlayerData(bool active)
	{
		ShowIgnoreClick(tweenDuration);
		if (tween)
		{
			TweenAlpha.Begin(playerData.cachedGameObject, tweenDuration, active ? 1 : 0);
		}
		else
		{
			playerData.cachedGameObject.SetActive(active);
		}
	}
}
