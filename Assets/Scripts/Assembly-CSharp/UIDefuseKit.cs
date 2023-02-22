using System;
using UnityEngine;

public class UIDefuseKit : MonoBehaviour
{
	private static bool DefuseKit;

	public UIWidget DefuseKitButton;

	private static UISprite DefuseKitIcon;

	public static bool defuseKit
	{
		get
		{
			return DefuseKit;
		}
		set
		{
			DefuseKit = value;
			if (DefuseKitIcon == null)
			{
				DefuseKitIcon = UIElements.Get<UISprite>("BombIcon");
			}
			DefuseKitIcon.cachedGameObject.SetActive(value);
			if (value)
			{
				DefuseKitIcon.spriteName = "Pilers";
			}
		}
	}

	private void OnEnable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onClick, new UICamera.VoidDelegate(OnClick));
		DefuseKitButton.cachedGameObject.SetActive(GameManager.team == Team.Blue);
		DefuseKitButton.alpha = ((!DefuseKit) ? 1f : 0.5f);
	}

	private void OnDisable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(OnClick));
		DefuseKit = false;
	}

	private void OnDestroy()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(OnClick));
		DefuseKit = false;
	}

	private void OnClick(GameObject go)
	{
		if (!(go == null) && !(go != base.gameObject) && !defuseKit)
		{
			if (UIBuyWeapon.Money < 400)
			{
				UIToast.Show(Localization.Get("Not enough money"));
				return;
			}
			UIBuyWeapon.Money -= 400;
			defuseKit = true;
			DefuseKitButton.alpha = 0.5f;
		}
	}
}
