using System;
using UnityEngine;
using UnityEngine.Events;

public class UIEventClick : MonoBehaviour
{
	public UnityEvent onClick;

	private GameObject mGameObject;

	private void Start()
	{
		mGameObject = base.gameObject;
	}

	private void OnEnable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onClick, new UICamera.VoidDelegate(OnClick));
	}

	private void OnDisable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(OnClick));
	}

	private void OnClick(GameObject go)
	{
		if (!(go != mGameObject) && onClick != null)
		{
			onClick.Invoke();
		}
	}
}
