using System;
using UnityEngine;

public class mServerInfo : MonoBehaviour
{
	public UILabel ServerNameLabel;

	public UILabel ModeLabel;

	public UILabel PlayersLabel;

	public GameObject Password;

	public UISprite mapSprite;

	private RoomInfo room;

	private void OnEnable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onClick, new UICamera.VoidDelegate(OnClick));
	}

	private void OnDisable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(OnClick));
	}

	public void SetData(RoomInfo info)
	{
		room = info;
		ServerNameLabel.text = info.Name;
		if (info.GetGameMode() == GameMode.Only)
		{
			ModeLabel.text = Localization.Get(info.GetGameMode().ToString()) + " (" + WeaponManager.GetWeaponName(info.GetOnlyWeapon()) + ")";
		}
		else if (info.GetCustomMapHash() != 0)
		{
			ModeLabel.text = Localization.Get(info.GetGameMode().ToString()) + " | Custom Map | " + info.GetSceneName();
		}
		else
		{
			ModeLabel.text = Localization.Get(info.GetGameMode().ToString()) + " | " + info.GetSceneName();
		}
		UISpriteData sprite = mapSprite.GetSprite(info.GetSceneName());
		if (sprite == null)
		{
			mapSprite.spriteName = "CustomMap";
		}
		else
		{
			mapSprite.spriteName = sprite.name;
		}
		PlayersLabel.text = info.PlayerCount + "/" + info.MaxPlayers;
		if (string.IsNullOrEmpty(info.GetPassword()))
		{
			Password.SetActive(false);
		}
		else
		{
			Password.SetActive(true);
		}
	}

	private void OnClick(GameObject go)
	{
		if (!(go != base.gameObject))
		{
			mJoinServer.room = room;
			mJoinServer.onBack = OnBack;
			mJoinServer.Join();
		}
	}

	private void OnBack()
	{
		mPopUp.HideAll("ServerList", false);
	}
}
