using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIWarningToast : MonoBehaviour
{
	public UIWidget widget;

	public UILabel label;

	public UISprite background;

	private List<string> textList = new List<string>();

	private List<float> timeList = new List<float>();

	private bool isShow;

	private Tweener tween;

	private static UIWarningToast instance;

	private void Awake()
	{
		instance = this;
	}

	public static void Show(string text)
	{
		Show(text, 2f, false);
	}

	public static void Show(string text, float duration)
	{
		Show(text, duration, false);
	}

	public static void Show(string text, float duration, bool queue)
	{
		if (instance == null)
		{
			return;
		}
		if (queue && instance.isShow)
		{
			instance.textList.Add(text);
			instance.timeList.Add(duration);
			return;
		}
		TimerManager.Cancel("WarningToast");
		instance.widget.alpha = 0f;
		if (instance.tween == null)
		{
			instance.tween = DOTween.To(() => instance.widget.alpha, delegate(float x)
			{
				instance.widget.alpha = x;
			}, 1f, 0.2f).SetAutoKill(false).OnUpdate(instance.OnUpdate);
		}
		else
		{
			instance.tween.ChangeStartValue(0f).ChangeEndValue(1f).Restart();
		}
		instance.label.text = text;
		instance.isShow = true;
		instance.background.UpdateAnchors();
		if (instance.background.width <= 120)
		{
			instance.background.width = 120;
		}
		TimerManager.In("WarningToast", duration, delegate
		{
			if (instance.textList.Count != 0)
			{
				Show(instance.textList[0], instance.timeList[0]);
				instance.textList.RemoveAt(0);
				instance.timeList.RemoveAt(0);
			}
			else
			{
				instance.isShow = false;
				instance.tween.ChangeStartValue(1f).ChangeEndValue(0f).Restart();
			}
		});
	}

	private void OnUpdate()
	{
		widget.UpdateWidget();
	}
}
