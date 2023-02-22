using UnityEngine;

public class UIControllerList : MonoBehaviour
{
	public UISprite FireSprite;

	public UISprite JumpSprite;

	public UISprite ReloadSprite;

	public UISprite AimSprite;

	public UISprite StatsSprite;

	public UISprite ChatSprite;

	public UISprite SelectWeaponSprite;

	public UISprite UseSprite;

	public UILabel UseLabel;

	public UISprite PauseSprite;

	public UISprite StoreSprite;

	public UISprite BombSprite;

	public UIPanel BuyWeaponsPanel;

	public InputJoystick JoystickInput;

	public InputTouchLook TouchLookInput;

	private static UIControllerList instance;

	public static UISprite Fire
	{
		get
		{
			return instance.FireSprite;
		}
	}

	public static UISprite Jump
	{
		get
		{
			return instance.JumpSprite;
		}
	}

	public static UISprite Reload
	{
		get
		{
			return instance.ReloadSprite;
		}
	}

	public static UISprite Aim
	{
		get
		{
			return instance.AimSprite;
		}
	}

	public static UISprite Stats
	{
		get
		{
			return instance.StatsSprite;
		}
	}

	public static UISprite Chat
	{
		get
		{
			return instance.ChatSprite;
		}
	}

	public static UISprite SelectWeapon
	{
		get
		{
			return instance.SelectWeaponSprite;
		}
	}

	public static UISprite Use
	{
		get
		{
			return instance.UseSprite;
		}
	}

	public static UILabel UseText
	{
		get
		{
			return instance.UseLabel;
		}
	}

	public static UISprite Pause
	{
		get
		{
			return instance.PauseSprite;
		}
	}

	public static UISprite Store
	{
		get
		{
			return instance.StoreSprite;
		}
	}

	public static UISprite Bomb
	{
		get
		{
			return instance.BombSprite;
		}
	}

	public static InputJoystick Joystick
	{
		get
		{
			return instance.JoystickInput;
		}
	}

	public static InputTouchLook TouchLook
	{
		get
		{
			return instance.TouchLookInput;
		}
	}

	public static UIPanel BuyWeapons
	{
		get
		{
			return instance.BuyWeaponsPanel;
		}
	}

	private void Start()
	{
		instance = this;
	}
}
