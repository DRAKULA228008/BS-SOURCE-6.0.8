using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Photon;
using UnityEngine;

public class mStoreWeapons : Photon.MonoBehaviour
{
    public enum SelectPanel
    {
        Weapon,
        Skins,
        Stickers
    }

    [Serializable]
    public class WeaponInfo
    {
        public GameObject panel;

        public UILabel nameLabel;

        public UILabel damageLabel;

        public UIProgressBar damageProgressBar;

        public UILabel fireRateLabel;

        public UIProgressBar fireRateProgressBar;

        public UILabel accuracyLabel;

        public UIProgressBar accuracyProgressBar;

        public UILabel ammoLabel;

        public UIProgressBar ammoProgressBar;

        public UILabel maxAmmoLabel;

        public UIProgressBar maxAmmoProgressBar;

        public UILabel mobilityLabel;

        public UIProgressBar mobilityProgressBar;

        [Space(10f)]
        public float maxDamage = 200f;

        public float maxFireRate = 100f;

        public float maxAccuracy = 100f;

        public float maxAmmo = 100f;

        public float maxMaxAmmo = 500f;

        public float maxMobility = 100f;
    }

    [Header("Weapon Info")]
    public WeaponInfo weaponInfo;

    [Header("Weapon Panel")]
    public UIWidget background;

    public UIWidget[] weaponButtonsBackground;

    public UILabel selectWeaponButtonLabel;

    public UILabel weaponBuyButtonLabel;

    public UITexture weaponBuyIcon;

    public UISprite weaponBuyLine;

    public UILabel fireStatLabel;

    private List<string> weaponsList = new List<string>();

    private int Weapon;

    private int WeaponSkin;

    private WeaponType weaponType;

    private WeaponData weaponData;

    private WeaponStoreData weaponStoreData;

    [Header("Weapon Skin")]
    public Color normalColor = new Color(255f, 255f, 255f, 255f);

    public Color baseColor = new Color(33f, 153f, 255f, 255f);

    public Color professionalColor = new Color(255f, 57f, 57f, 255f);

    public Color legendaryColor = new Color(255f, 32f, 147f, 255f);

    public UILabel skinNameLabel;

    public UILabel skinRarityLabel;

    public UILabel selectSkinButtonLabel;

    public UILabel selectSkinCountLabel;

    public GameObject skinDropInCase;

    public UILabel skinTemporarySkin;

    [Header("Weapon Stickers")]
    public UITexture stickersButton;

    public BoxCollider stickersButtonCollider;

    public UIScrollView stickerScrollView;

    public UISprite stickerElement;

    public List<UISprite> stickerElementList = new List<UISprite>();

    public List<UISprite> stickerElementPool = new List<UISprite>();

    private int stickerPos = 1;

    public UILabel stickerName;

    public UIWidget selectStickerButton;

    public UIWidget deleteStickerButton;

    public UISprite previewStickerSprite;

    private int weaponStickersCount;

    private int[] stickers;

    private int selectSticker;

    [Header("Others")]
    public GameObject weaponPanel;

    public GameObject skinPanel;

    public GameObject stickersPanel;

    public UISprite lineSprite;

    public GameObject inAppPanel;

    public Texture2D moneyTexture;

    public Texture2D goldTexture;

    private SelectPanel selectPanel;

    private bool Active;

    private void Start()
    {
        UIEventListener.Get(background.cachedGameObject).onDrag = RotateWeapon;
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
        mWeaponCamera.Rotate(drag, selectPanel == SelectPanel.Weapon);
    }

    public void Show(int type)
    {
        Active = true;
        background.cachedGameObject.SetActive(true);
        Weapon = 0;
        weaponType = (WeaponType)type;
        if (type > 0)
        {
            type--;
        }
        for (int i = 0; i < weaponButtonsBackground.Length; i++)
        {
            weaponButtonsBackground[i].color = new Color(1f, 1f, 1f, 0f);
        }
        weaponButtonsBackground[type].color = Color.white;
        GetWeaponList();
        UpdateWeapon();
    }

    public void Close()
    {
        if (Active)
        {
            background.cachedGameObject.SetActive(false);
            mPanelManager.Show("Store", true);
            mWeaponCamera.ResetRotateX(false);
            mWeaponCamera.SetViewportRect(new Rect(0f, 0f, 1f, 1f), 0f);
            mWeaponCamera.Close();
            selectPanel = SelectPanel.Weapon;
            Active = false;
        }
    }

    public void ShowHideWeaponInfoPanel()
    {
        weaponInfo.panel.SetActive(!weaponInfo.panel.activeSelf);
    }

    public void NextWeapon()
    {
        if (selectPanel == SelectPanel.Weapon)
        {
            Weapon++;
            if (Weapon >= weaponsList.Count)
            {
                Weapon = 0;
            }
            UpdateWeapon();
        }
    }

    public void LastWeapon()
    {
        if (selectPanel == SelectPanel.Weapon)
        {
            Weapon--;
            if (Weapon <= -1)
            {
                Weapon = weaponsList.Count - 1;
            }
            UpdateWeapon();
        }
    }

    private void UpdateWeapon()
    {
        GetWeaponData();
        GetWeaponStoreData();
        WeaponSkin = AccountManager.GetWeaponSkinSelected(weaponData.ID);
        mWeaponCamera.SetViewportRect(new Rect(0f, 0f, 1f, 1f), 0f);
        mWeaponCamera.Show(weaponsList[Weapon]);
        bool flag = AccountManager.GetWeapon(weaponData.ID);
        if ((int)weaponStoreData.Price == 0)
        {
            flag = true;
        }
        if (flag)
        {
            SelectedWeaponPanel();
        }
        else
        {
            BuyWeaponPanel();
        }
        stickersButtonCollider.enabled = mWeaponCamera.HasStickers();
        if (mWeaponCamera.HasStickers())
        {
            stickersButton.color = Color.white;
        }
        else
        {
            stickersButton.color = new Color(1f, 1f, 1f, 0.4f);
        }
        UpdateWeaponData();
        UpdateWeaponSkin();
    }

    private void SelectedWeaponPanel()
    {
        selectWeaponButtonLabel.cachedGameObject.SetActive(true);
        weaponBuyButtonLabel.cachedGameObject.SetActive(false);
        if (AccountManager.GetWeaponSelected(weaponType) == (int)weaponData.ID)
        {
            selectWeaponButtonLabel.text = Localization.Get("Selected");
            selectWeaponButtonLabel.alpha = 0.3f;
        }
        else
        {
            selectWeaponButtonLabel.text = Localization.Get("Select");
            selectWeaponButtonLabel.alpha = 1f;
        }
    }

    public void SelectWeapon()
    {
        if (AccountManager.GetWeaponSelected(weaponType) != (int)weaponData.ID && (!weaponData.Secret || AccountManager.GetGold() >= 0) && (!weaponData.Secret || AccountManager.GetMoney() >= 0))
        {
            AccountManager.SetWeaponSelected(weaponType, weaponData.ID);
            SelectedWeaponPanel();
            WeaponManager.UpdateData();
            UpdateWeaponSkin();
            AccountManager.SetFirebaseWeaponsSelected(null, null);
        }
    }

    private void BuyWeaponPanel()
    {
        selectWeaponButtonLabel.cachedGameObject.SetActive(false);
        selectWeaponButtonLabel.alpha = 1f;
        weaponBuyButtonLabel.cachedGameObject.SetActive(true);
        weaponBuyButtonLabel.text = weaponStoreData.Price.ToString("N0", new CultureInfo("en-us"));
        weaponBuyIcon.mainTexture = ((weaponStoreData.Currency != GameCurrency.Gold) ? moneyTexture : goldTexture);
        weaponBuyLine.color = ((weaponStoreData.Currency != GameCurrency.Gold) ? new Color32(169, 174, 183, byte.MaxValue) : new Color32(byte.MaxValue, 179, 0, byte.MaxValue));
    }

    public void BuyWeapon()
    {
        if (!AccountManager.isConnect)
        {
            UIToast.Show(Localization.Get("Connection account"));
            return;
        }
        int num = ((weaponStoreData.Currency != GameCurrency.Gold) ? AccountManager.GetMoney() : AccountManager.GetGold());
        if ((int)weaponStoreData.Price > num)
        {
            inAppPanel.SetActive(true);
            UIToast.Show(Localization.Get("Not enough money"));
            return;
        }
        mPopUp.ShowPopup(Localization.Get("Do you really want to buy?"), Localization.Get("Weapon"), Localization.Get("Yes"), delegate
        {
            mPopUp.HideAll();
            mPopUp.SetActiveWaitPanel(true, Localization.Get("Please wait"));
            AccountManager.SetFirebaseWeaponsBuy(weaponData.ID, delegate
            {
            }, delegate (string e)
            {
                mPopUp.SetActiveWaitPanel(false);
                UIToast.Show(e);
            });
            mPopUp.SetActiveWaitPanel(false);
            AccountManager.SetWeapon(weaponData.ID);
            AccountManager.SetWeaponSelected(weaponType, weaponData.ID);
            if (weaponStoreData.Currency == GameCurrency.Gold)
            {
                AccountManager.SetGold(num - (int)weaponStoreData.Price);
            }
            else
            {
                AccountManager.SetMoney(num - (int)weaponStoreData.Price);
            }
            SelectedWeaponPanel();
            EventManager.Dispatch("AccountUpdate");
            WeaponManager.UpdateData();
        }, Localization.Get("No"), delegate
        {
            mPopUp.HideAll();
        });
    }

    private void UpdateWeaponData()
    {
        weaponInfo.nameLabel.text = weaponData.Name;
        SetDamage();
        SetFireRate();
        SetAccuracy();
        SetAmmo();
        SetMaxAmmo();
        SetMobility();
    }

    private void UpdateWeaponSkin()
    {
        skinNameLabel.text = string.Concat(weaponData.Name, " | ", weaponStoreData.Skins[WeaponSkin].Name);
        fireStatLabel.cachedTransform.parent.gameObject.SetActive(AccountManager.GetFireStat(weaponData.ID, weaponStoreData.Skins[WeaponSkin].ID) && weaponData.Type != WeaponType.Knife);
        if (fireStatLabel.cachedTransform.parent.gameObject.activeSelf)
        {
            fireStatLabel.text = Localization.Get("Kills") + ": " + AccountManager.GetFireStatCounter(weaponData.ID, weaponStoreData.Skins[WeaponSkin].ID);
        }
        mWeaponCamera.SetSkin(weaponData.ID, weaponStoreData.Skins[WeaponSkin].ID);
        if (AccountManager.GetWeaponSkin(weaponData.ID, weaponStoreData.Skins[WeaponSkin].ID))
        {
            selectSkinButtonLabel.cachedGameObject.SetActive(true);
            if (AccountManager.GetWeaponSkinSelected(weaponData.ID) == (int)weaponStoreData.Skins[WeaponSkin].ID)
            {
                selectSkinButtonLabel.text = Localization.Get("Selected");
                selectSkinButtonLabel.alpha = 0.5f;
            }
            else
            {
                selectSkinButtonLabel.text = Localization.Get("Select");
                selectSkinButtonLabel.alpha = 1f;
            }
            skinDropInCase.SetActive(false);
        }
        else
        {
            skinDropInCase.SetActive(true);
            selectSkinButtonLabel.cachedGameObject.SetActive(false);
        }
        if (string.IsNullOrEmpty(weaponStoreData.Skins[WeaponSkin].Temporary))
        {
            skinTemporarySkin.cachedGameObject.SetActive(false);
        }
        else
        {
            skinTemporarySkin.cachedGameObject.SetActive(true);
            skinTemporarySkin.text = Localization.Get("You can get to") + ": " + weaponStoreData.Skins[WeaponSkin].Temporary;
        }
        selectSkinCountLabel.text = (WeaponSkin + 1).ToString() + "/" + weaponStoreData.Skins.Count;
        switch (weaponStoreData.Skins[WeaponSkin].Quality)
        {
            case WeaponSkinQuality.Default:
                skinRarityLabel.text = Localization.Get("Normal quality");
                TweenColor.Begin(lineSprite.cachedGameObject, 0.5f, normalColor);
                break;
            case WeaponSkinQuality.Normal:
                skinRarityLabel.text = Localization.Get("Normal quality");
                TweenColor.Begin(lineSprite.cachedGameObject, 0.5f, normalColor);
                break;
            case WeaponSkinQuality.Basic:
                skinRarityLabel.text = Localization.Get("Basic quality");
                TweenColor.Begin(lineSprite.cachedGameObject, 0.5f, baseColor);
                break;
            case WeaponSkinQuality.Professional:
                skinRarityLabel.text = Localization.Get("Professional quality");
                TweenColor.Begin(lineSprite.cachedGameObject, 0.5f, professionalColor);
                break;
            case WeaponSkinQuality.Legendary:
                skinRarityLabel.text = Localization.Get("Legendary quality");
                TweenColor.Begin(lineSprite.cachedGameObject, 0.5f, legendaryColor);
                break;
        }
        UpdateWeaponStickers();
    }

    public void NextSkin()
    {
        if (selectPanel != SelectPanel.Skins)
        {
            return;
        }
        if ((bool)weaponData.Secret)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < weaponStoreData.Skins.Count; i++)
            {
                if (AccountManager.GetWeaponSkin(weaponData.ID, weaponStoreData.Skins[i].ID))
                {
                    list.Add(i);
                }
            }
            for (int j = 0; j < list.Count; j++)
            {
                if (WeaponSkin == list[j])
                {
                    if (WeaponSkin == list[list.Count - 1])
                    {
                        WeaponSkin = list[0];
                    }
                    else
                    {
                        WeaponSkin = list[j + 1];
                    }
                    break;
                }
            }
        }
        else
        {
            WeaponSkin++;
            if (WeaponSkin >= weaponStoreData.Skins.Count)
            {
                WeaponSkin = 0;
            }
            if (!string.IsNullOrEmpty(weaponStoreData.Skins[WeaponSkin].Temporary) && !AccountManager.GetWeaponSkin(weaponData.ID, weaponStoreData.Skins[WeaponSkin].ID))
            {
                string[] array = weaponStoreData.Skins[WeaponSkin].Temporary.Split("."[0]);
                if (array.Length != 3)
                {
                    UnityEngine.MonoBehaviour.print("Error Temporary: " + weaponStoreData.Skins[WeaponSkin].Name + " " + weaponStoreData.Skins[WeaponSkin].Temporary);
                    NextSkin();
                    return;
                }
                if (new DateTime(2000 + int.Parse(array[2]), int.Parse(array[1]), int.Parse(array[0])) < DateTime.UtcNow)
                {
                    NextSkin();
                    return;
                }
            }
        }
        UpdateWeaponSkin();
    }

    public void LastSkin()
    {
        if (selectPanel != SelectPanel.Skins)
        {
            return;
        }
        if ((bool)weaponData.Secret)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < weaponStoreData.Skins.Count; i++)
            {
                if (AccountManager.GetWeaponSkin(weaponData.ID, weaponStoreData.Skins[i].ID))
                {
                    list.Add(i);
                }
            }
            for (int j = 0; j < list.Count; j++)
            {
                if (WeaponSkin == list[j])
                {
                    if (WeaponSkin == list[0])
                    {
                        WeaponSkin = list[list.Count - 1];
                    }
                    else
                    {
                        WeaponSkin = list[j - 1];
                    }
                    break;
                }
            }
        }
        else
        {
            WeaponSkin--;
            if (WeaponSkin < 0)
            {
                WeaponSkin = weaponStoreData.Skins.Count - 1;
            }
            if (!string.IsNullOrEmpty(weaponStoreData.Skins[WeaponSkin].Temporary) && !AccountManager.GetWeaponSkin(weaponData.ID, weaponStoreData.Skins[WeaponSkin].ID))
            {
                string[] array = weaponStoreData.Skins[WeaponSkin].Temporary.Split("."[0]);
                if (array.Length != 3)
                {
                    UnityEngine.MonoBehaviour.print("Error Temporary: " + weaponStoreData.Skins[WeaponSkin].Name + " " + weaponStoreData.Skins[WeaponSkin].Temporary);
                    LastSkin();
                    return;
                }
                if (new DateTime(2000 + int.Parse(array[2]), int.Parse(array[1]), int.Parse(array[0])) < DateTime.UtcNow)
                {
                    LastSkin();
                    return;
                }
            }
        }
        UpdateWeaponSkin();
    }

    public void SelectSkin()
    {
        if (selectPanel == SelectPanel.Skins && AccountManager.GetGold() >= 0 && AccountManager.GetMoney() >= 0)
        {
            AccountManager.SetWeaponSkinSelected(weaponData.ID, weaponStoreData.Skins[WeaponSkin].ID);
            UpdateWeaponSkin();
            AccountManager.SetWeaponSkinSelected2(weaponData.ID, weaponStoreData.Skins[WeaponSkin].ID, null, null);
        }
    }

    public void NextStickerPos()
    {
        stickerPos++;
        if (stickerPos >= weaponStickersCount)
        {
            stickerPos = 1;
        }
        UpdateSelectedSticker();
    }

    public void LastStickerPos()
    {
        stickerPos--;
        if (stickerPos <= 0)
        {
            stickerPos = weaponStickersCount;
        }
        UpdateSelectedSticker();
    }

    private void UpdateWeaponStickers()
    {
        mWeaponCamera.SetStickers(AccountManager.GetWeaponStickers(weaponData.ID, weaponStoreData.Skins[WeaponSkin].ID));
    }

    private void UpdateSelectedSticker()
    {
        int weaponSticker = AccountManager.GetWeaponSticker(weaponData.ID, weaponStoreData.Skins[WeaponSkin].ID, stickerPos);
        if (weaponSticker != -1)
        {
            StickerData stickerData = WeaponManager.GetStickerData(weaponSticker);
            mWeaponCamera.ActivePrevSticker(stickerPos, stickerData.ID);
            selectStickerButton.cachedGameObject.SetActive(false);
            deleteStickerButton.cachedGameObject.SetActive(true);
            previewStickerSprite.cachedGameObject.SetActive(true);
            previewStickerSprite.spriteName = stickerData.ID.ToString();
            stickerName.text = stickerData.Name;
        }
        else if (stickers.Length > 0)
        {
            StickerData stickerData2 = WeaponManager.GetStickerData(stickers[selectSticker]);
            mWeaponCamera.ActivePrevSticker(stickerPos, stickerData2.ID);
            selectStickerButton.cachedGameObject.SetActive(true);
            deleteStickerButton.cachedGameObject.SetActive(false);
            previewStickerSprite.cachedGameObject.SetActive(true);
            previewStickerSprite.spriteName = stickerData2.ID.ToString();
            stickerName.text = stickerData2.Name;
        }
        else
        {
            mWeaponCamera.DeactivePrevSticker();
            selectStickerButton.cachedGameObject.SetActive(false);
            deleteStickerButton.cachedGameObject.SetActive(false);
            stickerName.text = string.Empty;
            previewStickerSprite.cachedGameObject.SetActive(false);
        }
    }

    public void SetSticker()
    {
        if (selectPanel != SelectPanel.Stickers || AccountManager.GetGold() > 0 || AccountManager.GetMoney() > 0)
        {
            return;
        }
        mPopUp.ShowPopup(Localization.Get("Do you really want to stick a sticker?"), Localization.Get("Stickers"), Localization.Get("Yes"), delegate
        {
            mPopUp.HideAll();
            mPopUp.SetActiveWaitPanel(true, Localization.Get("Please wait"));
            AccountManager.DeleteSticker(this.stickers[this.selectSticker], false, null, null);
            AccountManager.DeleteAccountStickerFirebase(this.stickers[this.selectSticker]);
            AccountManager.AddWeaponStickerFirebase(weaponData.ID, weaponStoreData.Skins[WeaponSkin].ID, stickers[selectSticker], stickerPos);
            AccountManager.SetWeaponSticker(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID, stickerPos, this.stickers[this.selectSticker]);
        {
                mPopUp.SetActiveWaitPanel(false);
                stickers = AccountManager.GetStickers();
                selectSticker = 0;
                UpdateSelectedSticker();
                UpdateStickerScroll();
            }//, delegate (string e)
            {
                mPopUp.SetActiveWaitPanel(false);
                //UIToast.Show(e);
            }//);
        }, Localization.Get("No"), delegate
        {
            mPopUp.HideAll();
        });
    }

    public void DeleteSticker()
    {
        if (AccountManager.GetGold() >= 0 || AccountManager.GetMoney() >= 0)
        {
        mPopUp.ShowPopup(Localization.Get("Do you really want to remove the sticker? The sticker will be destroyed!"), Localization.Get("Delete"), Localization.Get("Yes"), delegate ()
        {
        mPopUp.HideAll();
        mPopUp.SetActiveWaitPanel(true, Localization.Get("Please wait"));
        AccountManager.DeleteWeaponSticker(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID, stickerPos);
        AccountManager.DeleteWeaponStickerFirebase(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID, stickerPos);
            mPopUp.SetActiveWaitPanel(false);
            UpdateSelectedSticker();
            UpdateStickerScroll();
        }, Localization.Get("No"), delegate ()
        {
            mPopUp.HideAll();
        });
        return;
       }
    }

    public void SelectSticker(GameObject go)
    {
        selectSticker = int.Parse(go.name);
        UpdateSelectedSticker();
    }

    private void UpdateStickerScroll()
    {
        stickerScrollView.ResetPosition();
        ClearStickerElements();
        int num = 0;
        for (int i = 0; i < stickers.Length; i++)
        {
            num = AccountManager.GetStickerCount(stickers[i]);
            if (num > 0)
            {
                UISprite uISprite = GetStickerElement();
                uISprite.spriteName = stickers[i].ToString();
                uISprite.cachedTransform.localPosition = new Vector3(50 * i, 0f, 0f);
                uISprite.cachedGameObject.name = i.ToString();
                UILabel componentInChildren = uISprite.GetComponentInChildren<UILabel>();
                componentInChildren.text = num.ToString();
                stickerElementList.Add(uISprite);
            }
        }
    }

    private UISprite GetStickerElement()
    {
        if (stickerElementPool.Count > 0)
        {
            UISprite uISprite = stickerElementPool[0];
            stickerElementPool.RemoveAt(0);
            uISprite.cachedGameObject.SetActive(true);
            return uISprite;
        }
        GameObject gameObject = stickerScrollView.gameObject.AddChild(stickerElement.cachedGameObject);
        gameObject.SetActive(true);
        return gameObject.GetComponent<UISprite>();
    }

    private void ClearStickerElements()
    {
        for (int i = 0; i < stickerElementList.Count; i++)
        {
            stickerElementPool.Add(stickerElementList[i]);
        }
        stickerElementList.Clear();
    }

    private void GetWeaponList()
    {
        weaponsList.Clear();
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            if (GameSettings.instance.Weapons[i].Type != weaponType)
            {
                continue;
            }
            int num = GameSettings.instance.Weapons[i].ID;
            if (num == 4 || num == 3 || num == 12)
            {
                weaponsList.Insert(0, GameSettings.instance.Weapons[i].Name);
            }
            else
            {
                if ((bool)GameSettings.instance.Weapons[i].Lock)
                {
                    continue;
                }
                if ((bool)GameSettings.instance.Weapons[i].Secret)
                {
                    if (AccountManager.GetWeapon(GameSettings.instance.Weapons[i].ID))
                    {
                        weaponsList.Add(GameSettings.instance.Weapons[i].Name);
                    }
                }
                else
                {
                    weaponsList.Add(GameSettings.instance.Weapons[i].Name);
                }
            }
        }
    }

    private void GetWeaponData()
    {
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            if ((string)GameSettings.instance.Weapons[i].Name == weaponsList[Weapon])
            {
                weaponData = GameSettings.instance.Weapons[i];
                break;
            }
        }
    }

    private void GetWeaponStoreData()
    {
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            if ((string)GameSettings.instance.Weapons[i].Name == weaponsList[Weapon])
            {
                weaponStoreData = GameSettings.instance.WeaponsStore[i];
                break;
            }
        }
    }

    private void SetDamage()
    {
        float num = ((int)weaponData.FaceDamage * (int)weaponData.FireBullets + (int)weaponData.BodyDamage * (int)weaponData.FireBullets + (int)weaponData.HandDamage * (int)weaponData.FireBullets + (int)weaponData.LegDamage * (int)weaponData.FireBullets) / 4;
        if ((bool)weaponData.CanFire)
        {
            weaponInfo.damageLabel.text = num.ToString();
            weaponInfo.damageProgressBar.value = num / weaponInfo.maxDamage;
        }
        else
        {
            weaponInfo.damageLabel.text = "-";
            weaponInfo.damageProgressBar.value = 0f;
        }
    }

    private void SetFireRate()
    {
        if (weaponData.Type == WeaponType.Knife || !weaponData.CanFire)
        {
            weaponInfo.fireRateLabel.text = "-";
            weaponInfo.fireRateProgressBar.value = 0f;
        }
        else
        {
            int num = Mathf.FloorToInt(100f - (float)weaponData.FireRate * 100f / 1.5f);
            weaponInfo.fireRateLabel.text = num.ToString();
            weaponInfo.fireRateProgressBar.value = (float)num / weaponInfo.maxFireRate;
        }
    }

    private void SetAccuracy()
    {
        if (weaponData.Type == WeaponType.Knife || !weaponData.CanFire)
        {
            weaponInfo.accuracyLabel.text = "-";
            weaponInfo.accuracyProgressBar.value = 0f;
        }
        else
        {
            int num = Mathf.FloorToInt(100f - (float)weaponData.FireAccuracy - (float)weaponData.Accuracy);
            weaponInfo.accuracyLabel.text = num.ToString();
            weaponInfo.accuracyProgressBar.value = (float)num / weaponInfo.maxAccuracy;
        }
    }

    private void SetAmmo()
    {
        if (weaponData.Type == WeaponType.Knife || !weaponData.CanFire)
        {
            weaponInfo.ammoLabel.text = "-";
            weaponInfo.ammoProgressBar.value = 0f;
        }
        else
        {
            int num = weaponData.Ammo;
            weaponInfo.ammoLabel.text = num.ToString();
            weaponInfo.ammoProgressBar.value = (float)num / weaponInfo.maxAmmo;
        }
    }

    private void SetMaxAmmo()
    {
        if (weaponData.Type == WeaponType.Knife || !weaponData.CanFire)
        {
            weaponInfo.maxAmmoLabel.text = "-";
            weaponInfo.maxAmmoProgressBar.value = 0f;
        }
        else
        {
            int num = weaponData.MaxAmmo;
            weaponInfo.maxAmmoLabel.text = num.ToString();
            weaponInfo.maxAmmoProgressBar.value = (float)num / weaponInfo.maxMaxAmmo;
        }
    }

    private void SetMobility()
    {
        int num = Mathf.FloorToInt(100f - (float)weaponData.Mass * 1000f);
        if (!weaponData.CanFire)
        {
            weaponInfo.mobilityLabel.text = "-";
            return;
        }
        weaponInfo.mobilityLabel.text = num.ToString();
        weaponInfo.mobilityProgressBar.value = (float)num / weaponInfo.maxMobility;
    }

    public void ShowStickersPanel()
    {
        if (selectPanel == SelectPanel.Weapon)
        {
            selectPanel = SelectPanel.Stickers;
            weaponPanel.SetActive(false);
            stickersPanel.SetActive(true);
            stickers = AccountManager.GetStickers();
            selectSticker = 0;
            weaponStickersCount = mWeaponCamera.GetStickersCount();
            UpdateSelectedSticker();
            UpdateStickerScroll();
        }
    }

    public void ShowSkinPanel()
    {
        if (selectPanel == SelectPanel.Weapon)
        {
            selectPanel = SelectPanel.Skins;
            weaponPanel.SetActive(false);
            skinPanel.SetActive(true);
        }
    }

    public void ShowWeaponPanel()
    {
        mWeaponCamera.ResetRotateX(true);
        if (selectPanel == SelectPanel.Skins)
        {
            skinPanel.SetActive(false);
        }
        else if (selectPanel == SelectPanel.Stickers)
        {
            stickersPanel.SetActive(false);
            previewStickerSprite.cachedGameObject.SetActive(false);
            mWeaponCamera.DeactivePrevSticker();
        }
        weaponPanel.SetActive(true);
        selectPanel = SelectPanel.Weapon;
        WeaponSkin = AccountManager.GetWeaponSkinSelected(weaponData.ID);
        UpdateWeaponSkin();
    }

    public void ShareScreenshot()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return;
        }
        skinPanel.SetActive(false);
        TimerManager.In(0.5f, delegate
        {
            AndroidNativeFunctions.TakeScreenshot(string.Concat(weaponData.Name, " | ", weaponStoreData.Skins[WeaponSkin].Name, "_", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")), delegate (string path)
            {
                skinPanel.SetActive(true);
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("#BlockStrike #BS");
                stringBuilder.AppendLine(string.Concat("My weapon ", weaponData.Name, " | ", weaponStoreData.Skins[WeaponSkin].Name, " in game Block Strike"));
                stringBuilder.AppendLine("http://bit.ly/blockstrike");
                AndroidNativeFunctions.ShareScreenshot(stringBuilder.ToString(), "Block Strike", Localization.Get("Share"), path);
            });
        });
    }

    public void MoneySet(bool isgoldprice)
    {
        if (isgoldprice)
        {
            AccountManager.UpdateGold(AccountManager.GetGold() - (int)weaponStoreData.Price, null, null);
            AccountManager.SetGold(AccountManager.GetGold() - (int)weaponStoreData.Price);
        }
        else
        {
            AccountManager.UpdateMoney(AccountManager.GetMoney() - (int)weaponStoreData.Price, null, null);
            AccountManager.SetMoney(AccountManager.GetMoney() - (int)weaponStoreData.Price);
        }
    }
}
