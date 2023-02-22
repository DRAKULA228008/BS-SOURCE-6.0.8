using FreeJSON;
using UnityEngine;

public class mClanManager : MonoBehaviour
{
    public mClanCreate createClan;

    public mClanChat chatClan;

    public mClanPlayers playersClan;

    private bool loadData;

    public void Open()
    {
        if (!AccountManager.isConnect)
        {
            UIToast.Show(Localization.Get("Connection account"));
            return;
        }
        mPanelManager.Show("Clan", true);
        ClanManager.Load();
        if (string.IsNullOrEmpty(AccountManager.GetClan()))
        {
            createClan.panel.SetActive(true);
            chatClan.panel.SetActive(false);
            return;
        }
        chatClan.panel.SetActive(true);
        createClan.panel.SetActive(false);
        chatClan.Open();
        if (!loadData)
        {
            AccountManager.Clan.GetData(OnGetDataComplete, OnGetDataError);
        }
    }

    public void Close()
    {
        chatClan.Close();
    }

    public void OnSubmit()
    {
        createClan.OnSubmit();
        chatClan.OnSubmit();
    }

    public void OnCreateClan()
    {
        createClan.CreateClan(OnCreateClanComplete, OnCreateClanError);
    }

    public void OpenPlayers()
    {
        playersClan.panel.SetActive(true);
        chatClan.panel.SetActive(false);
        playersClan.Open();
    }

    public void SelectPlayer(mClanPlayerElement element)
    {
        playersClan.SelectPlayer(element);
    }

    private void OnCreateClanComplete()
    {
        mPopUp.SetActiveWaitPanel(false);
        Open();
    }

    private void OnCreateClanError(string error)
    {
        mPopUp.SetActiveWaitPanel(false);
        UIToast.Show(error);
    }

    private void OnGetDataComplete(string data)
    {
        JsonObject jsonObject = JsonObject.Parse(data);
        ClanManager.name = jsonObject.Get<string>("n");
        ClanManager.admin = jsonObject.Get<int>("a");
        JsonObject jsonObject2 = jsonObject.Get<JsonObject>("p");
        int[] array = new int[jsonObject2.Length];
        for (int i = 0; i < jsonObject2.Length; i++)
        {
            array[i] = int.Parse(jsonObject2.GetKey(i));
        }
        ClanManager.players = array;
        chatClan.nameLabel.text = ClanManager.name;
        ClanManager.Save();
        loadData = true;
    }

    private void OnGetDataError(string error)
    {
        UIToast.Show(error);
    }
}
