using DG.Tweening;
using UnityEngine;

public class UIFade : MonoBehaviour
{
	public UISprite fadeSprite;

	public Tweener tween;

	private static UIFade instance;

	private void Start()
	{
		instance = this;
	}

	public static void Fade(float from, float to, float duration)
	{
		Fade(from, to, duration, null);
	}

	public static void Fade(float from, float to, float duration, TweenCallback finish)
	{
		if (instance.tween == null)
		{
			instance.tween = DOTween.To(() => instance.fadeSprite.alpha, delegate(float x)
			{
				instance.fadeSprite.alpha = x;
			}, to, duration).SetAutoKill(false).OnUpdate(instance.OnUpdate);
			instance.tween.onComplete = finish;
		}
		else
		{
			instance.tween.ChangeValues(from, to, duration).Restart();
			instance.tween.onComplete = finish;
		}
	}

	private void OnUpdate()
	{
		fadeSprite.UpdateWidget();
	}
}
