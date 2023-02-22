using System;
using AppodealAds.Unity.Api;
using UnityEngine;
using FreeJSON;

public class mAccount : MonoBehaviour
{
    public CryptoString WebClientID;

    public static int NameSetted;

    public static string Name;

    public static bool isAdmin = true;

    public void Start()
    {
        if (!AccountManager.isConnect)
        {
            Start2();
        }
    }

    public static void Reconnect()
    {

    }

    private void Start2()
    {
        string playerID = string.Empty;
        string AccountID = AndroidNativeFunctions.GetAndroidID();
        playerID = AccountID;
        playerID = playerID.Replace(".", "*");
        //mPopUp.SetActiveWait(true, Localization.Get("Connect to the account") + ": " + playerID);
        mPopUp.SetActiveWait(true, Localization.Get("Connect to the account"));
        playerID = playerID.ToLower();
        TimerManager.In(0.5f, delegate
        {
            AccountManager.Login(playerID, Login, LoginError);
        });
        TimerManager.In(1.5f, delegate
        {
            AccountManager.isConnect = true;
        });
    }

    private void Login(bool isCreated)
    {
        {
            if (isCreated)
            {
                mPopUp.SetActiveWait(false);
                EventManager.Dispatch("AccountUpdate");
                WeaponManager.UpdateData();
                UserSettings userSettings = new UserSettings();
                userSettings.setUserId(AccountManager.AccountID);
                if (string.IsNullOrEmpty(AccountManager.instance.Data.AccountName) || AccountManager.instance.Data.AccountName.ToString() == " ")
                {
                    SetPlayerName(delegate (string playerName)
                    {
                        AccountManager.UpdateName(playerName, SetPlayerNameComplete, SetPlayerNameError);
                        mPopUp.ShowText(Localization.Get("Please wait") + "...");
                    });
                }
            }
            else
            {
                mPopUp.SetActiveWait(false);
                mPopUp.ShowText(Localization.Get("Please wait") + "...");
                SetPlayerName(delegate (string playerName)
                {
                    AccountManager.Register(playerName, RegisterComplete, RegisterError);
                    mPopUp.ShowText(Localization.Get("Please wait") + "...");
                });
            }
        }
    }

    private void LoginError(string error)
    {
        mPopUp.ShowPopup(Localization.Get("Error") + ": " + error, "Account", Localization.Get("Connect"), LoginErrorTry, Localization.Get("Exit"), Exit);
        mPopUp.SetActiveWait(false);
    }

    private void LoginErrorTry()
    {
        if (mPopUp.activePopup)
        {
            mPopUp.HideAll("Menu");
        }
        AccountManager.Login(Login, LoginError);
        string text = AccountManager.AccountID;
        text = text.Remove(text.LastIndexOf("@"));
        mPopUp.SetActiveWait(true, Localization.Get("Connect to the account") + ": " + text);
    }

    private void RegisterComplete()
    {
        EventManager.Dispatch("AccountUpdate");
        WeaponManager.UpdateData();
        mPopUp.HideAll("Menu");
    }

    private void RegisterError(string error)
    {
        mPopUp.ShowPopup(Localization.Get("Error") + ": " + error, "Account", Localization.Get("Connect"), RegisterErrorTry, Localization.Get("Exit"), Exit);
    }

    private void RegisterErrorTry()
    {
        if (mPopUp.activePopup)
        {
            mPopUp.HideAll("Menu");
        }
        SetPlayerName(delegate (string playerName)
        {
            AccountManager.Register(playerName, RegisterComplete, RegisterError);
            mPopUp.ShowText(Localization.Get("Please wait") + "...");
        });
    }

    private void SetPlayerName(Action<string> action)
    {
        mPopUp.SetActiveWait(false);
        mPopUp.ShowInput(string.Empty, Localization.Get("ChangeName"), 12, UIInput.KeyboardType.Default, new Action(this.SetPlayerNameSubmit), new Action(this.SetPlayerNameChange), Localization.Get("Back"), null, "Ok", delegate
        {
            this.SetPlayerNameSave(action);
        });
    }

    private void SetPlayerNameSave(Action<string> action)
    {
        string text = NGUIText.StripSymbols(mPopUp.GetInputText());
        string text2 = mChangeName.UpdateSymbols(text, true);
        if (text != text2)
        {
            mPopUp.SetInputText(text2);
            mAccount.Name = text;
            return;
        }
        if (text.Length > 3 && !(text == "Null") && !(text[0].ToString() == " ") && !(text[text.Length - 1].ToString() == " "))
        {
            if (action != null)
            {
                mAccount.Name = text;
                action(text);
            }
            return;
        }
        text = "Player " + UnityEngine.Random.Range(0, 99999);
        mPopUp.SetInputText(text);
        mAccount.Name = text;
    }

    private void SetPlayerNameSubmit()
    {
        string text = mPopUp.GetInputText();
        if (text.Length <= 3 || text == "Null" || text[0].ToString() == " " || text[text.Length - 1].ToString() == " ")
        {
            text = "Player " + UnityEngine.Random.Range(0, 99999);
        }
        text = NGUIText.StripSymbols(text);
        mPopUp.SetInputText(text);
    }

    private void SetPlayerNameChange()
    {
        string inputText = mPopUp.GetInputText();
        string text = mChangeName.UpdateSymbols(inputText, true);
        if (inputText != text)
        {
            mPopUp.SetInputText(text);
        }
    }

    private void SetPlayerNameComplete(string playerName)
    {
        EventManager.Dispatch("AccountUpdate");
        WeaponManager.UpdateData();
        mPopUp.HideAll("Menu");
    }

    private void SetPlayerNameError(string error)
    {
        SetPlayerName(delegate (string playerName)
        {
            AccountManager.UpdateName(playerName, SetPlayerNameComplete, SetPlayerNameError);
            mPopUp.ShowText(Localization.Get("Please wait") + "...");
        });
        string text = error;
        if (text == "Name already taken")
        {
            text = Localization.Get("Name already taken");
        }
        UIToast.Show(Localization.Get("Error") + ": " + text, 3f);
    }

    public static void Exit()
    {
        Application.Quit();
    }
}
