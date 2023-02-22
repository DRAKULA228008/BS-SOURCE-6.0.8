using System;
using UnityEngine;

public class UISpectatorElement : MonoBehaviour
{
	public UISprite LineSprite;

	public UILabel NameLabel;

	public UISprite WeaponSprite;

	public UISprite DeadSprite;

	public UILabel HealthLabel;

	public UITexture AvatarTexture;

	public PhotonPlayer Player;

	private void OnEnable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onClick, new UICamera.VoidDelegate(OnClick));
	}

	private void OnDisable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(OnClick));
	}

	public void UpdateWidget()
	{
		if (LineSprite.cachedGameObject.activeSelf)
		{
			LineSprite.UpdateWidget();
			NameLabel.UpdateWidget();
			WeaponSprite.UpdateWidget();
			DeadSprite.UpdateWidget();
			HealthLabel.UpdateWidget();
		}
	}

	private void OnClick(GameObject go)
	{
		if (!(go != LineSprite.cachedGameObject) && Player != null && !Player.GetDead())
		{
			CameraManager.selectPlayer = Player.ID;
		}
	}

	public bool SetWeapon(int playerID, int weapon, int skin)
	{
		if (Player != null && Player.ID == playerID)
		{
			SetWeapon(weapon, skin);
			return true;
		}
		return false;
	}

	public void SetWeapon(int weapon, int skin)
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((int)GameSettings.instance.Weapons[i].ID == weapon)
			{
				WeaponSprite.spriteName = weapon + "-" + skin;
				WeaponSprite.width = (int)(GameSettings.instance.WeaponsCaseSize[i].x * 0.45f);
				WeaponSprite.height = (int)(GameSettings.instance.WeaponsCaseSize[i].y * 0.45f);
				break;
			}
		}
	}

	public void SetData(PhotonPlayer player)
	{
		Player = player;
		NameLabel.text = player.UserId;
		AvatarTexture.mainTexture = AvatarManager.Get(player.GetAvatarUrl());
		SetDead(player.GetDead());
	}

	public bool SetDead(int playerID, bool dead)
	{
		if (Player != null && Player.ID == playerID)
		{
			SetDead(dead);
			return true;
		}
		return false;
	}

	public void SetDead(bool dead)
	{
		DeadSprite.cachedGameObject.SetActive(dead);
		HealthLabel.cachedGameObject.SetActive(!dead);
		WeaponSprite.cachedGameObject.SetActive(!dead);
	}

	public bool SetHealth(int playerID, byte health)
	{
		if (Player != null && Player.ID == playerID)
		{
			SetHealth(health);
			return true;
		}
		return false;
	}

	public void SetHealth(byte health)
	{
		if (health == 0)
		{
			HealthLabel.cachedGameObject.SetActive(false);
		}
		else
		{
			HealthLabel.text = "+" + health;
		}
	}
}
