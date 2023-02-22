using UnityEngine;

public class mServerSettings : MonoBehaviour
{
	public GameObject settingsButton;

	[Header("Only")]
	public GameObject onlyPanel;

	public UIPopupList onlyWeaponPopupList;

	private static mServerSettings instance;

	private void Start()
	{
		instance = this;
	}

	public static void Check(GameMode mode, string map)
	{
		switch (mode)
		{
		case GameMode.Only:
			instance.settingsButton.SetActive(true);
			instance.gameObject.SendMessage("SetDefaultMaxPlayers");
			break;
		case GameMode.MiniGames:
			if (map == "50Traps")
			{
				instance.gameObject.SendMessage("SetMaxPlayers", new int[5] { 4, 8, 16, 24, 32 });
			}
			else
			{
				instance.gameObject.SendMessage("SetDefaultMaxPlayers");
			}
			break;
		default:
			instance.gameObject.SendMessage("SetDefaultMaxPlayers");
			instance.settingsButton.SetActive(false);
			break;
		}
	}

	public void Open()
	{
		GameMode mode = mCreateServer.mode;
		if (mode == GameMode.Only)
		{
			StartOnlyMode();
		}
	}

	public void Close()
	{
		GameMode mode = mCreateServer.mode;
		if (mode == GameMode.Only)
		{
			onlyPanel.gameObject.SetActive(false);
		}
	}

	private void StartOnlyMode()
	{
		onlyPanel.gameObject.SetActive(true);
		onlyWeaponPopupList.Clear();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (!GameSettings.instance.Weapons[i].Lock && !GameSettings.instance.Weapons[i].Secret)
			{
				onlyWeaponPopupList.AddItem(GameSettings.instance.Weapons[i].Name);
			}
		}
		onlyWeaponPopupList.value = onlyWeaponPopupList.items[0];
	}

	public static int GetOnlyWeapon()
	{
		int num = WeaponManager.GetWeaponID(instance.onlyWeaponPopupList.value);
		if (num <= 0)
		{
			num = 1;
		}
		return num;
	}
}
