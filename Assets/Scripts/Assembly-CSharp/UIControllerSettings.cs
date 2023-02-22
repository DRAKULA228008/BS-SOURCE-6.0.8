using System;
using UnityEngine;

public class UIControllerSettings : MonoBehaviour
{
	public Transform Anchor;

	public UISprite JoystickRect;

	public UISprite LookRect;

	public UISprite JoystickSprite;

	public static event Action onDefaultEvent;

	public static event Action onSaveEvent;

	private void OnEnable()
	{
		TimerManager.In(0.1f, delegate
		{
			if (Settings.DynamicJoystick)
			{
				JoystickRect.cachedGameObject.SetActive(true);
				RectToSprite(JoystickRect, nPlayerPrefs.GetRect("Joystick_Rect", new Rect(0f, 0f, (float)Screen.width / 2.5f, Screen.height / 2)));
				JoystickSprite.cachedGameObject.SetActive(false);
			}
			else
			{
				JoystickRect.cachedGameObject.SetActive(false);
				JoystickSprite.cachedGameObject.SetActive(true);
			}
			RectToSprite(LookRect, nPlayerPrefs.GetRect("Look_Rect", new Rect(Screen.width / 2, 0f, Screen.width / 2, Screen.height)));
		});
	}

	public void OnBack()
	{
		if (UIControllerSettings.onSaveEvent != null)
		{
			UIControllerSettings.onSaveEvent();
		}
		if (Settings.DynamicJoystick)
		{
			nPlayerPrefs.SetRect("Joystick_Rect", SpriteToRect(JoystickRect));
		}
		nPlayerPrefs.SetRect("Look_Rect", SpriteToRect(LookRect));
	}

	public void OnDefault()
	{
		if (UIControllerSettings.onDefaultEvent != null)
		{
			UIControllerSettings.onDefaultEvent();
		}
		nPlayerPrefs.SetRect("Joystick_Rect", new Rect(0f, 0f, (float)Screen.width / 2.5f, Screen.height / 2));
		nPlayerPrefs.SetRect("Look_Rect", new Rect(Screen.width / 2, 0f, Screen.width / 2, Screen.height));
		OnEnable();
	}

	private void RectToSprite(UISprite sp, Rect rect)
	{
		if (rect.x != 0f)
		{
			rect.x = rect.x / (float)Screen.width * Anchor.localPosition.x;
		}
		if (rect.y != 0f)
		{
			rect.y = rect.y / (float)Screen.height * Anchor.localPosition.y;
		}
		if (rect.width != 0f)
		{
			rect.width = rect.width / (float)Screen.width * Anchor.localPosition.x;
		}
		if (rect.height != 0f)
		{
			rect.height = rect.height / (float)Screen.height * Anchor.localPosition.y;
		}
		Vector3 localPosition = new Vector3(rect.x + rect.width / 2f, rect.y + rect.height / 2f, 0f);
		sp.cachedTransform.localPosition = localPosition;
		sp.width = (int)rect.width;
		sp.height = (int)rect.height;
	}

	private Rect SpriteToRect(UISprite sp)
	{
		Rect result = default(Rect);
		result.x = sp.cachedTransform.localPosition.x - (float)sp.width / 2f;
		if (result.x != 0f)
		{
			result.x = result.x / Anchor.localPosition.x * (float)Screen.width;
		}
		result.y = sp.cachedTransform.localPosition.y - (float)sp.height / 2f;
		if (result.y != 0f)
		{
			result.y = result.y / Anchor.localPosition.y * (float)Screen.height;
		}
		result.width = sp.width;
		if (result.width != 0f)
		{
			result.width = result.width / Anchor.localPosition.x * (float)Screen.width;
		}
		result.height = sp.height;
		if (result.height != 0f)
		{
			result.height = result.height / Anchor.localPosition.y * (float)Screen.height;
		}
		return result;
	}
}
