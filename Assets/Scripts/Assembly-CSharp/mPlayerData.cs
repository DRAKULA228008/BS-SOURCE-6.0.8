using System.Globalization;
using UnityEngine;

public class mPlayerData : MonoBehaviour
{
	public UILabel PlayerNameLabel;

	public UILabel PlayerLevelLabel;

	public UIProgressBar PlayerXP;

	public UILabel MoneyLabel;

	public UILabel GoldLabel;

	public UITexture AvatarTexture;

    public static mPlayerData instance;

	private void Start()
	{
		EventManager.AddListener("AccountUpdate", AccountUpdate);
		EventManager.AddListener("AvatarUpdate", AvatarUpdate);
		AccountUpdate();
		AvatarUpdate();
	}

	private void AccountUpdate()
	{
		if (string.IsNullOrEmpty(AccountManager.instance.Data.Clan.ToString()))
		{
			PlayerNameLabel.text = AccountManager.instance.Data.AccountName;
		}
		else
		{
			PlayerNameLabel.text = string.Concat(AccountManager.instance.Data.AccountName, " - ", AccountManager.instance.Data.Clan);
		}
		PlayerLevelLabel.text = Localization.Get("Level") + " - " + AccountManager.GetLevel();
		PlayerXP.value = (float)AccountManager.GetXP() / (float)AccountManager.GetMaxXP();
		GoldLabel.text = AccountManager.GetGold().ToString("N0", new CultureInfo("en-us"));
        MoneyLabel.text = AccountManager.GetMoney().ToString("N0", new CultureInfo("en-us"));
	}

	private void AvatarUpdate()
	{
        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            AvatarTexture.mainTexture = GameSettings.instance.DeveloperAvatar;
        } else
        //AvatarTexture.mainTexture = AccountManager.instance.Data.Avatar;
        AvatarTexture.mainTexture = GameSettings.instance.NoAvatarTexture;
    }
}
