using System;
using UnityEngine;

public class InputButton : MonoBehaviour
{
	public string button;

	private bool press;

	private float alpha = 1f;

	private UISprite sprite;

	private void Start()
	{
		sprite = GetComponent<UISprite>();
		EventManager.AddListener("OnSettings", OnSettings);
		EventManager.AddListener("OnSaveButton", OnPosition);
		OnSettings();
		OnPosition();
	}

	private void OnEnable()
	{
		UICamera.onPress = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onPress, new UICamera.BoolDelegate(OnPress));
		if (sprite != null)
		{
			sprite.alpha = alpha;
		}
	}

	private void OnDisable()
	{
		UICamera.onPress = (UICamera.BoolDelegate)Delegate.Remove(UICamera.onPress, new UICamera.BoolDelegate(OnPress));
		if (press)
		{
			InputManager.SetButtonUp(button);
		}
	}

	private void OnPress(GameObject go, bool pressed)
	{
		if (!(sprite.cachedGameObject != go))
		{
			press = pressed;
			if (press)
			{
				sprite.alpha = alpha * 0.5f;
				InputManager.SetButtonDown(button);
			}
			else
			{
				sprite.alpha = alpha;
				InputManager.SetButtonUp(button);
			}
		}
	}

	private void OnPosition()
	{
		TimerManager.In(0.1f, delegate
		{
			if (nPlayerPrefs.HasKey("Button_Pos_" + button))
			{
				sprite.cachedTransform.localPosition = nPlayerPrefs.GetVector3("Button_Pos_" + button);
				if (nPlayerPrefs.HasKey("Button_Size_" + button))
				{
					Vector2 vector = nPlayerPrefs.GetVector2("Button_Size_" + button);
					if (vector != Vector2.zero)
					{
						sprite.width = (int)vector.x;
						sprite.height = (int)vector.y;
					}
				}
			}
			sprite.UpdateWidget();
		});
	}

	private void OnSettings()
	{
		alpha = Settings.ButtonAlpha;
		if (!Settings.HUD)
		{
			alpha = 1f;
		}
		sprite.alpha = alpha;
	}
}
