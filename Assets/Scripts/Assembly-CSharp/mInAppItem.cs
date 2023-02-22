using System;
using UnityEngine;

public class mInAppItem : MonoBehaviour
{
	public enum ItemList
	{
		Purchase,
		Consume
	}

	public ItemList Item;

	public string Sku;

	public UILabel PriceLabel;

	private GameObject mGameObject;

	private void Start()
	{
		PriceLabel.text = InAppManager.GetPrice(Sku);
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
		if (!(go != mGameObject))
		{
			if (!AccountManager.isConnect)
			{
				UIToast.Show(Localization.Get("Connection account"));
			}
			else if (Item == ItemList.Purchase)
			{
				InAppManager.Purchase(Sku);
			}
			else
			{
				InAppManager.Consume(Sku);
			}
		}
	}
}
