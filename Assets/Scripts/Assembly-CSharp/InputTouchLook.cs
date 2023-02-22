using UnityEngine;
using System;

public class InputTouchLook : MonoBehaviour
{
	private Rect touchZone = new Rect(50f, 0f, 50f, 100f);

	private bool move;

	private int id = -1;

	private float dpi;

	private Vector2 value;

	private Touch touch;

	private static bool fixTouch;

	private Vector2 fixPos;

	private void Start()
	{
		dpi = Screen.dpi / 100f;
		if (dpi == 0f)
		{
			dpi = 1.6f;
		}
		EventManager.AddListener("OnSettings", OnSettings);
		OnSettings();
		fixTouch = GameConsole.Load("fix_touch_look", false);
	}

	private void OnDisable()
	{
		UpdateValue(Vector2.zero);
		id = -1;
		move = false;
	}

	private void Update()
	{
		for (int i = 0; i < Input.touchCount; i++)
		{
			touch = Input.GetTouch(i);
			if (touch.phase == TouchPhase.Began && touchZone.Contains(new Vector3(touch.position.x, (float)Screen.height - touch.position.y, 0f)))
			{
				move = true;
				id = touch.fingerId;
				if (fixTouch)
				{
					fixPos = touch.position;
				}
			}
			if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && id == touch.fingerId && move)
			{
				if (fixTouch)
				{
					UpdateValue((touch.position - fixPos) / dpi);
					fixPos = touch.position;
				}
				else
				{
					UpdateValue(touch.deltaPosition / (dpi + 5f));
				}
			}
			if ((touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended) && id == touch.fingerId && move)
			{
				id = -1;
				move = false;
				UpdateValue(Vector2.zero);
				if (fixTouch)
				{
					fixPos = touch.position;
				}
			}
		}
	}

	private void UpdateValue(Vector2 v)
	{
		value = v;
		InputManager.SetAxis("Mouse X", value.x);
		InputManager.SetAxis("Mouse Y", value.y);
	}

	private void OnSettings()
	{
		touchZone = nPlayerPrefs.GetRect("Look_Rect", new Rect(Screen.width / 2, 0f, Screen.width / 2, Screen.height));
    }
}
