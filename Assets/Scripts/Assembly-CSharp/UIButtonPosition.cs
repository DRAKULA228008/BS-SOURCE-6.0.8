using System;
using UnityEngine;

public class UIButtonPosition : MonoBehaviour
{
	public string button;

	public bool changeSize = true;

	private Vector2 size = new Vector2(35f, 150f);

	private Vector3 defaultPosition;

	private Vector2 defaultSize;

	private UISprite sprite;

	private void Start()
	{
		sprite = GetComponent<UISprite>();
		defaultPosition = sprite.transform.localPosition;
		defaultSize = new Vector2(sprite.width, sprite.height);
		EventManager.AddListener("OnDefaultButton", OnDefaultButton);
		EventManager.AddListener("OnSaveButton", OnSaveButton);
		OnPosition();
	}

	private void OnPosition()
	{
		TimerManager.In(0.1f, delegate
		{
			if (nPlayerPrefs.HasKey("Button_Pos_" + button))
			{
				sprite.cachedTransform.localPosition = nPlayerPrefs.GetVector3("Button_Pos_" + button);
				if (changeSize && nPlayerPrefs.HasKey("Button_Size_" + button))
				{
					Vector2 vector = nPlayerPrefs.GetVector2("Button_Size_" + button);
					if (vector != Vector2.zero)
					{
						sprite.width = (int)vector.x;
						sprite.height = (int)vector.y;
					}
				}
			}
		});
	}

	private void OnEnable()
	{
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Combine(UICamera.onDrag, new UICamera.VectorDelegate(OnDrag));
		UICamera.onDoubleClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onDoubleClick, new UICamera.VoidDelegate(OnDoubleClick));
	}

	private void OnDisable()
	{
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Remove(UICamera.onDrag, new UICamera.VectorDelegate(OnDrag));
		UICamera.onDoubleClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onDoubleClick, new UICamera.VoidDelegate(OnDoubleClick));
	}

	private void OnDrag(GameObject go, Vector2 delta)
	{
		if (!(sprite.cachedGameObject != go))
		{
			sprite.cachedTransform.localPosition += new Vector3(delta.x, delta.y, 0f);
		}
	}

	private void OnDoubleClick(GameObject go)
	{
		if (!(sprite.cachedGameObject != go) && changeSize)
		{
			sprite.width += 5;
			sprite.height += 5;
			if ((float)sprite.width > size.y)
			{
				sprite.width = (int)size.x;
				sprite.height = (int)size.x;
			}
		}
	}

	private void OnDefaultButton()
	{
		sprite.transform.localPosition = defaultPosition;
		if (changeSize)
		{
			sprite.width = (int)defaultSize.x;
			sprite.height = (int)defaultSize.y;
		}
	}

	private void OnSaveButton()
	{
		nPlayerPrefs.SetVector3("Button_Pos_" + button, sprite.cachedTransform.localPosition);
		if (changeSize)
		{
			nPlayerPrefs.SetVector2("Button_Size_" + button, new Vector2(sprite.width, sprite.height));
		}
	}
}
