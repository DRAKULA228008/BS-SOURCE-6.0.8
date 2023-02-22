using DG.Tweening;
using UnityEngine;

public class UIDuration : MonoBehaviour
{
	public UISprite sprite;

	public UILabel label;

	private Tweener tween;

	private static UIDuration instance;

	public static UISprite duration
	{
		get
		{
			return instance.sprite;
		}
	}

	private void Start()
	{
		instance = this;
	}

	public static void StartDuration(float duration)
	{
		StartDuration(duration, false, null);
	}

	public static void StartDuration(float duration, bool time)
	{
		StartDuration(duration, time, null);
	}

	public static void StartDuration(float duration, bool time, TweenCallback callback)
	{
		StopDuration();
		instance.sprite.cachedGameObject.SetActive(true);
		if (callback != null)
		{
			instance.tween = DOTween.To(() => instance.sprite.width, delegate(int x)
			{
				instance.sprite.width = x;
			}, 155, duration).SetEase(Ease.Linear).OnComplete(callback);
			if (time)
			{
				instance.tween.OnUpdate(instance.UpdateDuration);
			}
		}
		else
		{
			instance.tween = DOTween.To(() => instance.sprite.width, delegate(int x)
			{
				instance.sprite.width = x;
			}, 155, duration).SetEase(Ease.Linear);
			if (time)
			{
				instance.tween.OnUpdate(instance.UpdateDuration);
			}
		}
	}

	private void UpdateDuration()
	{
		label.text = (tween.Duration() - tween.fullPosition).ToString("00:00");
	}

	public static void StopDuration()
	{
		if (instance.tween != null && instance.tween.IsActive())
		{
			instance.tween.Kill();
		}
		instance.sprite.cachedGameObject.SetActive(false);
		instance.sprite.width = 0;
		instance.label.text = string.Empty;
	}
}
