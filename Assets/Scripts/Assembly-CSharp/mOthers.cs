using UnityEngine;
using System;
using System.Globalization;

public class mOthers : MonoBehaviour
{
	private bool isExit;

    public static string bd = "blockstrike-314ea-default-rtdb.firebaseio.com";

    public static string appid = "375c9fe5-a02b-4704-9a47-9a5db5602d8d";

    private void Start()
	{
		Application.targetFrameRate = 60;
        InputManager.Init();
		UICamera.clickUIInput = true;
		RoundPlayersData.Init();
		BadWordsManager.Init();
		WeaponManager.Init();
		UISelectWeapon.AllWeapons = false;
		UISelectWeapon.SelectedUpdateWeaponManager = false;
		TimerManager.In(0.8f, delegate
		{
			isExit = true;
		});
		LocalNotification.CancelNotification(1);
		LocalNotification.SendNotification(1, 259200L, "Block Strike", Localization.Get("NotificationText"), Color.red, true, false, false, "app_icon");
		PoolManager.ClearAll();
		DropWeaponManager.ClearScene();
		if (!AccountManager.isConnect)
		{
			EventManager.AddListener("AccountConnected", AccountConnected);
		}
    }

	private void AccountConnected()
	{
		CheckData();
		CheckCurrency();
	}

	private void CheckData()
	{
		int weaponSelected = AccountManager.GetWeaponSelected(WeaponType.Rifle);
		if (!AccountManager.GetWeapon(weaponSelected) || WeaponManager.GetWeaponData(weaponSelected).Type != WeaponType.Rifle)
		{
			AccountManager.SetWeaponSelected(WeaponType.Rifle, 12);
			WeaponManager.SetSelectWeapon(WeaponType.Rifle, 12);
		}
		weaponSelected = AccountManager.GetWeaponSelected(WeaponType.Pistol);
		if (!AccountManager.GetWeapon(weaponSelected) || WeaponManager.GetWeaponData(weaponSelected).Type != WeaponType.Pistol)
		{
			AccountManager.SetWeaponSelected(WeaponType.Pistol, 3);
			WeaponManager.SetSelectWeapon(WeaponType.Pistol, 3);
		}
		weaponSelected = AccountManager.GetWeaponSelected(WeaponType.Knife);
		if (!AccountManager.GetWeapon(weaponSelected) || WeaponManager.GetWeaponData(weaponSelected).Type != WeaponType.Knife)
		{
			AccountManager.SetWeaponSelected(WeaponType.Knife, 4);
			WeaponManager.SetSelectWeapon(WeaponType.Knife, 4);
		}
	}

	private void CheckCurrency()
	{
		if (AccountManager.GetGold() < 0 || AccountManager.GetMoney() < 0)
		{
			if ((bool)WeaponManager.GetWeaponData(AccountManager.GetWeaponSelected(WeaponType.Knife)).Secret)
			{
				AccountManager.SetWeaponSelected(WeaponType.Knife, 4);
			}
			for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
			{
				AccountManager.SetWeaponSkinSelected(GameSettings.instance.Weapons[i].ID, 0);
			}
		}
	}

	private void Update()
	{
		if (!isExit && Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	public void ExitGame()
	{
		if (isExit)
		{
			mPopUp.ShowPopup(Localization.Get("ExitText"), Localization.Get("Exit"), Localization.Get("Yes"), delegate
			{
				Application.Quit();
			}, Localization.Get("No"), delegate
			{
				mPopUp.HideAll("Menu");
			});
		}
	}

	public void ShowOthersGames()
	{
		Application.OpenURL("https://play.google.com/store/apps/dev?id=6363329851677974248");
	}

	public void ComingSoon()
	{
		UIToast.Show(Localization.Get("Coming soon"));
	}

	public void OpenStore()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
		}
		else
		{
			mPanelManager.Show("Store", true);
		}
	}

	public void AccountUpdate()
	{
		AccountManager.UpdateData(null);
	}

	public static void Stream(string url)
	{
		mPopUp.ShowPopup(Localization.Get("Stream Toast"), Localization.Get("Watch the video"), Localization.Get("Yes"), delegate
		{
			mPopUp.HideAll("Menu");
			Application.OpenURL(url);
		}, Localization.Get("No"), delegate
		{
			mPopUp.HideAll("Menu");
		});
	}

	public void ShowToast(string text)
	{
		UIToast.Show(text);
	}
}
