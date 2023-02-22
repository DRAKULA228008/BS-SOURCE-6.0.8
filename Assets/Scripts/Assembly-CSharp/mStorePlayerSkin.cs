using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class mStorePlayerSkin : Photon.MonoBehaviour
{
    [Header("Player Skin")]
    public GameObject background;

    public BodyParts selectBodyParts;

    public UILabel selectSkinButtonLabel;

    public UILabel buySkinButtonLabel;

    public UITexture buySkinButtonTexture;

    public UISprite buySkinLine;

    public UITexture changeTeamButton;

    [Header("Others")]
    public Texture2D moneyTexture;

    public Texture2D goldTexture;

    public GameObject inAppPanel;

    public int headSkin;

    public int bodySkin;

    public int legsSkin;

    private Team team = Team.Blue;

    private PlayerStoreSkinData headData;

    private PlayerStoreSkinData bodyData;

    private PlayerStoreSkinData legsData;

    private List<int> headList = new List<int>();

    private List<int> bodyList = new List<int>();

    private List<int> legsList = new List<int>();

    private bool actived;

    private void Start()
    {
        UIEventListener uIEventListener = UIEventListener.Get(background);
        uIEventListener.onDrag = RotateWeapon;
    }

    private void OnEnable()
    {
        PhotonNetwork.onDisconnectedFromPhoton = (PhotonNetwork.VoidDelegate)Delegate.Combine(PhotonNetwork.onDisconnectedFromPhoton, new PhotonNetwork.VoidDelegate(OnDisconnectedFromPhoton));
    }

    private void OnDisable()
    {
        PhotonNetwork.onDisconnectedFromPhoton = (PhotonNetwork.VoidDelegate)Delegate.Remove(PhotonNetwork.onDisconnectedFromPhoton, new PhotonNetwork.VoidDelegate(OnDisconnectedFromPhoton));
    }

    private void OnDisconnectedFromPhoton()
    {
        Close();
    }

    private void RotateWeapon(GameObject go, Vector2 drag)
    {
        mPlayerCamera.Rotate(drag);
    }

    public void Show()
    {
        actived = true;
        mPanelManager.Show("PlayerSkin", true);
        GetSkinsList();
        headSkin = GetSkinIndex(AccountManager.GetPlayerSkinSelected(BodyParts.Head), BodyParts.Head);
        bodySkin = GetSkinIndex(AccountManager.GetPlayerSkinSelected(BodyParts.Body), BodyParts.Body);
        legsSkin = GetSkinIndex(AccountManager.GetPlayerSkinSelected(BodyParts.Legs), BodyParts.Legs);
        UpdateSkin();
    }

    public void Close()
    {
        if (actived)
        {
            mPanelManager.Show("Store", true);
            mPlayerCamera.Close();
            actived = false;
        }
    }

    private void UpdateSkin()
    {
        GetPlayerSkinData();
        mPlayerCamera.Show();
        mPlayerCamera.SetSkin(team, headList[headSkin].ToString(), bodyList[bodySkin].ToString(), legsList[legsSkin].ToString());
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        bool flag = AccountManager.GetPlayerSkin(GetSelectSkinData().ID, selectBodyParts);
        if ((int)GetSelectSkinData().Price == 0)
        {
            flag = true;
        }
        if (flag)
        {
            selectSkinButtonLabel.cachedGameObject.SetActive(true);
            buySkinButtonLabel.cachedGameObject.SetActive(false);
            if (AccountManager.GetPlayerSkinSelected(selectBodyParts) == (int)GetSelectSkinData().ID)
            {
                selectSkinButtonLabel.text = Localization.Get("Selected");
                selectSkinButtonLabel.alpha = 0.3f;
            }
            else
            {
                selectSkinButtonLabel.text = Localization.Get("Select");
                selectSkinButtonLabel.alpha = 1f;
            }
        }
        else
        {
            selectSkinButtonLabel.cachedGameObject.SetActive(false);
            buySkinButtonLabel.cachedGameObject.SetActive(true);
            buySkinButtonLabel.text = GetSelectSkinData().Price.ToString("n0");
            buySkinButtonTexture.mainTexture = ((GetSelectSkinData().Currency != 0) ? goldTexture : moneyTexture);
            buySkinLine.color = ((GetSelectSkinData().Currency != 0) ? new Color32(byte.MaxValue, 179, 0, byte.MaxValue) : new Color32(169, 174, 183, byte.MaxValue));
        }
    }

    public void NextSkin()
    {
        switch (selectBodyParts)
        {
            case BodyParts.Head:
                headSkin++;
                if (headSkin >= headList.Count)
                {
                    headSkin = 0;
                }
                break;
            case BodyParts.Body:
                bodySkin++;
                if (bodySkin >= bodyList.Count)
                {
                    bodySkin = 0;
                }
                break;
            case BodyParts.Legs:
                legsSkin++;
                if (legsSkin >= legsList.Count)
                {
                    legsSkin = 0;
                }
                break;
        }
        UpdateSkin();
    }

    public void LastSkin()
    {
        switch (selectBodyParts)
        {
            case BodyParts.Head:
                headSkin--;
                if (headSkin <= -1)
                {
                    headSkin = headList.Count - 1;
                }
                break;
            case BodyParts.Body:
                bodySkin--;
                if (bodySkin <= -1)
                {
                    bodySkin = bodyList.Count - 1;
                }
                break;
            case BodyParts.Legs:
                legsSkin--;
                if (legsSkin <= -1)
                {
                    legsSkin = legsList.Count - 1;
                }
                break;
        }
        UpdateSkin();
    }

    public void ChangeTeam()
    {
        if (team == Team.Blue)
        {
            team = Team.Red;
            changeTeamButton.color = new Color32(237, 44, 45, byte.MaxValue);
        }
        else
        {
            team = Team.Blue;
            changeTeamButton.color = new Color32(70, 136, 231, byte.MaxValue);
        }
        UpdateSkin();
    }

    public void SelectBodyParts(int select)
    {
        selectBodyParts = (BodyParts)select;
        headSkin = GetSkinIndex(AccountManager.GetPlayerSkinSelected(BodyParts.Head), BodyParts.Head);
        bodySkin = GetSkinIndex(AccountManager.GetPlayerSkinSelected(BodyParts.Body), BodyParts.Body);
        legsSkin = GetSkinIndex(AccountManager.GetPlayerSkinSelected(BodyParts.Legs), BodyParts.Legs);
        UpdateSkin();
    }

    public void SelectSkin()
    {
        if (AccountManager.GetPlayerSkinSelected(selectBodyParts) != (int)GetSelectSkinData().ID)
        {
            AccountManager.SetPlayerSkinSelected(GetSelectSkinData().ID, selectBodyParts);
            UpdateButtons();
        }
    }

    public void BuySkin()
    {
        if (!AccountManager.isConnect)
        {
            UIToast.Show(Localization.Get("Connection account"));
            return;
        }
        if (GetSelectSkinData().Price > AccountManager.GetMoney() && GetSelectSkinData().Price > AccountManager.GetGold())
        {
            inAppPanel.SetActive(true);
            UIToast.Show(Localization.Get("Not enough money"));
            return;
        }
        mPopUp.ShowPopup(Localization.Get("Do you really want to buy?"), Localization.Get("Player Skin"), Localization.Get("Yes"), delegate ()
        {
            mPopUp.HideAll();
            AccountManager.SetPlayerSkin(GetSelectSkinData().ID, selectBodyParts);
            AccountManager.BuyPlayerSkinFirebase(this.GetSelectSkinData().ID, selectBodyParts);
            if (this.GetSelectSkinData().Currency == GameCurrency.Gold)
            {
                AccountManager.UpdateGold(AccountManager.GetGold() - this.GetSelectSkinData().Price, null, null);
                AccountManager.SetGold(AccountManager.GetGold() - this.GetSelectSkinData().Price, false);
            }
            if (this.GetSelectSkinData().Currency == GameCurrency.Money)
            {
                AccountManager.UpdateMoney(AccountManager.GetMoney() - this.GetSelectSkinData().Price, null, null);
                AccountManager.SetMoney(AccountManager.GetMoney() - this.GetSelectSkinData().Price, false);
            }
            this.UpdateButtons();
            EventManager.Dispatch("AccountUpdate");
        }, Localization.Get("No"), delegate ()
        {
            mPopUp.HideAll();
        });
    }

    private void GetSkinsList()
    {
        headList.Clear();
        bodyList.Clear();
        legsList.Clear();
        for (int i = 0; i < GameSettings.instance.PlayerStoreHead.Count; i++)
        {
            headList.Add(GameSettings.instance.PlayerStoreHead[i].ID);
        }
        for (int j = 0; j < GameSettings.instance.PlayerStoreBody.Count; j++)
        {
            bodyList.Add(GameSettings.instance.PlayerStoreBody[j].ID);
        }
        for (int k = 0; k < GameSettings.instance.PlayerStoreLegs.Count; k++)
        {
            legsList.Add(GameSettings.instance.PlayerStoreLegs[k].ID);
        }
    }

    private void GetPlayerSkinData()
    {
        switch (selectBodyParts)
        {
            case BodyParts.Head:
                {
                    for (int j = 0; j < GameSettings.instance.PlayerStoreHead.Count; j++)
                    {
                        if (headList[headSkin] == (int)GameSettings.instance.PlayerStoreHead[j].ID)
                        {
                            headData = GameSettings.instance.PlayerStoreHead[j];
                            break;
                        }
                    }
                    break;
                }
            case BodyParts.Body:
                {
                    for (int k = 0; k < GameSettings.instance.PlayerStoreBody.Count; k++)
                    {
                        if (bodyList[bodySkin] == (int)GameSettings.instance.PlayerStoreBody[k].ID)
                        {
                            bodyData = GameSettings.instance.PlayerStoreBody[k];
                            break;
                        }
                    }
                    break;
                }
            case BodyParts.Legs:
                {
                    for (int i = 0; i < GameSettings.instance.PlayerStoreLegs.Count; i++)
                    {
                        if (legsList[legsSkin] == (int)GameSettings.instance.PlayerStoreLegs[i].ID)
                        {
                            legsData = GameSettings.instance.PlayerStoreLegs[i];
                            break;
                        }
                    }
                    break;
                }
        }
    }

    private PlayerStoreSkinData GetSelectSkinData()
    {
        switch (selectBodyParts)
        {
            case BodyParts.Head:
                return headData;
            case BodyParts.Body:
                return bodyData;
            case BodyParts.Legs:
                return legsData;
            default:
                return bodyData;
        }
    }

    private int GetSkinIndex(int skinID, BodyParts part)
    {
        switch (part)
        {
            case BodyParts.Head:
                {
                    for (int j = 0; j < headList.Count; j++)
                    {
                        if (skinID == headList[j])
                        {
                            return j;
                        }
                    }
                    break;
                }
            case BodyParts.Body:
                {
                    for (int k = 0; k < bodyList.Count; k++)
                    {
                        if (skinID == bodyList[k])
                        {
                            return k;
                        }
                    }
                    break;
                }
            case BodyParts.Legs:
                {
                    for (int i = 0; i < legsList.Count; i++)
                    {
                        if (skinID == legsList[i])
                        {
                            return i;
                        }
                    }
                    break;
                }
        }
        return 0;
    }
}
