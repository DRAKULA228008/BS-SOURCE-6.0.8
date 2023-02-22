using UnityEngine;
using System;

public class InputJoystick : MonoBehaviour
{
	public enum JoystickType
	{
		Dynamic,
		Static
	}

	public Camera uiCamera;

	public UISprite stick;

	public UISprite background;

	public float distance = 0.3f;

	public string xAxisName = "Horizontal";

	public string yAxisName = "Vertical";

	public JoystickType selectType;

	private int id = -1;

	private Rect touchZone;

	private Vector3 touchPos;

	public static bool shift;

	private float lastTouch;

	private float positionMultiplier;

	private float alpha = 1f;

	private void Start()
	{
		positionMultiplier = 1f / distance;
		EventManager.AddListener("OnSettings", OnSettings);
		OnSettings();
	}

	private void OnDisable()
	{
		if (selectType == JoystickType.Dynamic)
		{
			Hide();
		}
		id = -1;
		UpdateValue(Vector2.zero);
	}

	private void Update()
	{
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (touch.phase == TouchPhase.Began && touchZone.Contains(touch.position))
			{
				if (lastTouch + 0.25f > Time.time)
				{
					shift = true;
					lastTouch = 0f;
				}
				else
				{
					lastTouch = Time.time;
				}
				id = touch.fingerId;
				touchPos = touch.position;
				touchPos.x = Mathf.Clamp01(touchPos.x / (float)Screen.width);
				touchPos.y = Mathf.Clamp01(touchPos.y / (float)Screen.height);
				if (selectType == JoystickType.Dynamic)
				{
					background.cachedTransform.position = uiCamera.ViewportToWorldPoint(touchPos);
				}
				stick.cachedTransform.position = uiCamera.ViewportToWorldPoint(touchPos);
				stick.UpdateWidget();
				background.UpdateWidget();
				Show();
			}
			if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && id == touch.fingerId)
			{
				touchPos = touch.position;
				touchPos.x = Mathf.Clamp01(touchPos.x / (float)Screen.width);
				touchPos.y = Mathf.Clamp01(touchPos.y / (float)Screen.height);
				stick.cachedTransform.position = uiCamera.ViewportToWorldPoint(touchPos);
				stick.cachedTransform.position = Clamp(background.cachedTransform.position, stick.cachedTransform.position);
				UpdateValue((stick.cachedTransform.position - background.cachedTransform.position) * positionMultiplier);
				stick.UpdateWidget();
			}
			if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && id == touch.fingerId)
			{
				shift = false;
				id = -1;
				if (selectType == JoystickType.Dynamic)
				{
					Hide();
				}
				else
				{
					stick.cachedTransform.position = background.cachedTransform.position;
				}
				UpdateValue(Vector2.zero);
			}
		}
	}

	private Vector3 Clamp(Vector3 p, Vector3 y)
	{
		if (Vector3.Distance(p, y) > distance)
		{
			Vector3 vector = y - p;
			vector.Normalize();
			return vector * distance + p;
		}
		return y;
	}

	private void Show()
	{
		stick.alpha = alpha;
		background.alpha = alpha;
		stick.UpdateWidget();
		background.UpdateWidget();
	}

	private void Hide()
	{
		try
		{
			stick.alpha = 0f;
			background.alpha = 0f;
			stick.UpdateWidget();
			background.UpdateWidget();
		}
		catch
		{
		}
	}

	private void UpdateValue(Vector2 value)
	{
		InputManager.SetAxis(xAxisName, value.x);
		InputManager.SetAxis(yAxisName, value.y);
	}

	private void OnSettings()
	{
		selectType = ((!Settings.DynamicJoystick) ? JoystickType.Static : JoystickType.Dynamic);
		if (selectType == JoystickType.Dynamic)
		{
			touchZone = nPlayerPrefs.GetRect("Joystick_Rect", new Rect(0f, 0f, (float)Screen.width / 2.5f, Screen.height / 2));
            Hide();
		}
		else
		{
			if (nPlayerPrefs.HasKey("Joystick_Pos"))
			{
				background.cachedTransform.localPosition = nPlayerPrefs.GetVector3("Joystick_Pos");
			}
			Vector3 vector = uiCamera.WorldToViewportPoint(background.cachedTransform.position);
			touchZone = new Rect(vector.x * (float)Screen.width - (float)(background.width / 2), vector.y * (float)Screen.height - (float)(background.height / 2), background.width, background.height);
			Show();
			stick.cachedTransform.position = background.cachedTransform.position;
		}
		alpha = Settings.ButtonAlpha;
		stick.UpdateWidget();
		background.UpdateWidget();
	}
}
