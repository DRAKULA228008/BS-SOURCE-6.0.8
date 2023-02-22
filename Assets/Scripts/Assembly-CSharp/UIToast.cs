using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIToast : MonoBehaviour
{
	public UILabel label;

	public UISprite background;

	private List<string> textList = new List<string>();

	private List<float> timeList = new List<float>();

	private bool isShow;

	private Tweener tween;

	private static UIToast instance;

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
		TimerManager.Cancel("Toast");
		instance.label.alpha = 0f;
		if (instance.tween == null)
		{
			instance.tween = DOTween.To(() => instance.label.alpha, delegate(float x)
			{
				instance.label.alpha = x;
			}, 1f, 0.2f).SetAutoKill(false).OnUpdate(instance.OnUpdate);
		}
		else
		{
			instance.tween.ChangeStartValue(0f).ChangeEndValue(1f).Restart();
		}
		instance.label.text = text;
		instance.isShow = true;
		instance.background.UpdateAnchors();
		TimerManager.In("Toast", duration, delegate
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
		label.UpdateWidget();
	}
}
