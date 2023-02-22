using System.Collections.Generic;
using FreeJSON;
using UnityEngine;

public class PlayerRoundManager
{
	public class cFireStat
	{
		public CryptoInt weapon;

		public List<CryptoInt> skins = new List<CryptoInt>();

		public List<CryptoInt> counts = new List<CryptoInt>();
	}

	private static GameMode Mode;

	private static CryptoInt2 XP = 0;

	private static CryptoInt2 Money = 0;

	private static CryptoInt2 Kills = 0;

	private static CryptoInt2 Headshot = 0;

	private static CryptoInt2 Deaths = 0;

	private static List<cFireStat> FireStat = new List<cFireStat>();

	private static float StartTime;

	private static float FinishTime;

	public static bool hasValue
	{
		get
		{
			if (GetXP() != 0)
			{
				return true;
			}
			if (GetMoney() != 0)
			{
				return true;
			}
			if (GetKills() != 0)
			{
				return true;
			}
			if (GetHeadshot() != 0)
			{
				return true;
			}
			if (GetDeaths() != 0)
			{
				return true;
			}
			return false;
		}
	}

	public static GameMode GetMode()
	{
		return Mode;
	}

	public static int GetXP()
	{
		return XP;
	}

	public static int GetMoney()
	{
		return Money;
	}

	public static int GetKills()
	{
		return Kills;
	}

	public static int GetHeadshot()
	{
		return Headshot;
	}

	public static int GetDeaths()
	{
		return Deaths;
	}

	public static float GetTime()
	{
		return FinishTime - StartTime;
	}

	public static void SetMode(GameMode mode)
	{
		Mode = mode;
		if (StartTime == 0f)
		{
			StartTime = Time.time;
		}
	}

	public static void SetXP(int xp)
	{
		SetXP(xp, false);
	}

	public static void SetXP(int xp, bool simple)
	{
		if (!LevelManager.customScene)
		{
			if (simple)
			{
				XP = (int)XP + xp;
			}
			else
			{
				XP = (int)XP + Mathf.Clamp(Mathf.RoundToInt((float)PhotonNetwork.room.PlayerCount / (float)nValue.int12 * (float)xp), nValue.int0, xp);
			}
		}
	}

	public static void SetMoney(int money)
	{
		SetMoney(money, false);
	}

	public static void SetMoney(int money, bool simple)
	{
		if (!LevelManager.customScene)
		{
			if (simple)
			{
				Money = (int)Money + money;
			}
			else
			{
				Money = (int)Money + Mathf.Clamp(Mathf.RoundToInt((float)PhotonNetwork.room.PlayerCount / (float)nValue.int12 * (float)money), nValue.int0, money);
			}
		}
	}

	public static void SetKills1()
	{
		if (!LevelManager.customScene)
		{
			Kills = (int)Kills + nValue.int1;
		}
	}

	public static void SetHeadshot1()
	{
		if (!LevelManager.customScene)
		{
			Headshot = (int)Headshot + nValue.int1;
		}
	}

	public static void SetDeaths1()
	{
		if (!LevelManager.customScene)
		{
			Deaths = (int)Deaths + nValue.int1;
		}
	}

    public static void SetFireStat1(int weapon, int skin)
    {
        bool flag = false;
        for (int i = 0; i < PlayerRoundManager.FireStat.Count; i++)
        {
            if (PlayerRoundManager.FireStat[i].weapon == weapon)
            {
                if (PlayerRoundManager.FireStat[i].skins.Contains(skin))
                {
                    for (int j = 0; j < PlayerRoundManager.FireStat[i].skins.Count; j++)
                    {
                        if (PlayerRoundManager.FireStat[i].skins[j] == skin)
                        {
                            List<CryptoInt> counts;
                            List<CryptoInt> list = counts = PlayerRoundManager.FireStat[i].counts;
                            int index2;
                            int index = index2 = j;
                            CryptoInt value = counts[index2];
                            list[index] = ++value;
                        }
                    }
                }
                else
                {
                    PlayerRoundManager.FireStat[i].skins.Add(skin);
                    PlayerRoundManager.FireStat[i].counts.Add(nValue.int1);
                }
                flag = true;
                if (!flag)
                {
                    PlayerRoundManager.cFireStat cFireStat = new PlayerRoundManager.cFireStat();
                    cFireStat.weapon = weapon;
                    cFireStat.skins.Add(skin);
                    cFireStat.counts.Add(nValue.int1);
                    PlayerRoundManager.FireStat.Add(cFireStat);
                }
                return;
            }
        }
    }

    public static void Show()
	{
		if (PhotonNetwork.offlineMode)
		{
			return;
		}
		FinishTime = Time.time;
		TimerManager.In(0.5f, false, delegate
		{
			if (AccountManager.GetInAppPurchase().Count > 0 && AccountManager.GetGold() > -1 && AccountManager.GetMoney() > -1)
			{
				ShowPopup();
			}
			else
			{
				AdsManager.ShowAny("ExitServer");
				TimerManager.In(0.5f, ShowPopup);
			}
		});
	}

	private static void ShowPopup()
	{
		if (hasValue)
		{
			mPlayerRoundManager.Show();
			SetData();
		}
		else
		{
			Clear();
		}
	}

  //  jsonObject.Add("x", GetXP());
	//	jsonObject.Add("m", GetMoney());
	//	jsonObject.Add("k", GetKills());
		//jsonObject.Add("d", GetDeaths());
		//jsonObject.Add("h", GetHeadshot());
		//jsonObject.Add("t", Mathf.FloorToInt(GetTime()));

   private static void SetData()
    {
        AccountManager.SetXP1(PlayerRoundManager.GetXP());
        AccountManager.SetKills1(PlayerRoundManager.GetKills());
        AccountManager.SetDeaths1(PlayerRoundManager.GetDeaths());
        AccountManager.SetHeadshot1(PlayerRoundManager.GetHeadshot());
        AccountManager.SetTime1(Mathf.FloorToInt(GetTime()));
        TimerManager.In(0.03f, delegate ()
        {
            AccountManager.UpdatePlayerRoundData(AccountManager.GetXP(), AccountManager.GetKills(), AccountManager.GetDeaths(), AccountManager.GetHeadshot(), AccountManager.GetTime(), null, null);
        });
    }


    private static void Failed(string error)
	{
		TimerManager.In(0.1f, delegate
		{
			mPopUp.ShowPopup(Localization.Get("Error") + ": " + error, Localization.Get("Error"), Localization.Get("Retry"), delegate
			{
				mPopUp.HideAll();
				SetData();
			});
		});
	}

	public static void Clear()
	{
		XP = nValue.int0;
		Money = nValue.int0;
		Kills = nValue.int0;
		Headshot = nValue.int0;
		Deaths = nValue.int0;
		StartTime = nValue.int0;
		FinishTime = nValue.int0;
		FireStat.Clear();
	}
}
