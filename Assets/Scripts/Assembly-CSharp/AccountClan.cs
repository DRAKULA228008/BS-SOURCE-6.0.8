using System;
using FreeJSON;
using UnityEngine;

public class AccountClan
{
    public void Create(string tag, string name, Action complete, Action<string> failed)
    {
    }

    public void AddPlayer(int id, Action complete, Action<string> failed)
    {
    }

    public void GetData(Action<string> complete, Action<string> failed)
    {
        GetData(AccountManager.instance.Data.Clan, complete, failed);
    }

    public void GetData(string tag, Action<string> complete, Action<string> failed)
    {
    }

    public void DeletePlayer(int playerID, string tag, Action complete, Action<string> failed)
    {
    }

    public void SendMessage(string message)
    {
        Firebase obj = new Firebase
        {
            Auth = AccountManager.AccountToken
        };
        new AccountData();
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("m", string.Concat(AccountManager.instance.Data.AccountName, ": ", message));
        jsonObject.Add("n", string.Empty);
        jsonObject.Add("t", JsonObject.Parse(Firebase.GetTimeStamp()));
        obj.Child("Clans").Child(AccountManager.GetClan()).Child("c")
            .Child((string)AccountManager.instance.Data.AccountName + UnityEngine.Random.Range(int.MinValue, int.MaxValue))
            .UpdateValue(jsonObject.ToString(), delegate
            {
            }, null);
    }
}
