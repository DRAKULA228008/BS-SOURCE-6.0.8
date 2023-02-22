using System;
using System.Collections.Generic;
using System.Text;
using FreeJSON;
using Photon;
using UnityEngine;

public class mCaseManager : Photon.MonoBehaviour
{
    [Serializable]
    public class Case
    {
        public string Name;

        public GameCurrency Currency;

        public CryptoInt Price;

        public UILabel NameLabel;

        public UILabel InfoLabel;

        public CryptoBool Money;

        public CryptoInt Normal;

        public CryptoInt Base;

        public CryptoInt Professional;

        public CryptoInt Legendary;

        public CryptoInt FireStat;

        public CryptoInt SecretWeapon;
    }

    [Header("Cases")]
    public Case[] SkinCases;

    public Case[] StickerCases;

    private bool isSkinCases = true;

    [Header("Case Wheel")]
    public bool caseRotate;

    private string selectCaseName;

    private float startRotate;

    public AnimationCurve caseWheelCurve;

    public float duration = 8f;

    public float caseWheelLerp = 1f;

    public Vector2 finishInterval;

    private float finishPosition;

    public AudioSource soundSource;

    public float soundInterval;

    private float lastsoundInterval;

    public mCaseItem[] caseItems;

    public mCaseItem finishItem;

    public Transform caseItemsRoot;

    [Header("Finish Panel")]
    public UISprite finishBackground;

    public GameObject finishPanel;

    public UILabel finishLabel;

    public UILabel finishQualityLabel;

    public UIGrid finishQualityTable;

    public UISprite finishLineSprite;

    public GameObject finishPanelGold;

    public GameObject finishAlreadyAvailable;

    public UITexture finishAlreadyAvailableTexture;

    public UILabel finishAlreadyAvailableLabel;

    public UITexture finishFireStatTexture;

    public GameObject finishSecretWeapon;

    private bool isFinishWeapon;

    private WeaponData finishWeapon;

    private WeaponSkinData finishWeaponSkin;

    private StickerData finishSticker;

    private bool finishWeaponFireStat;

    private bool finishWeaponAlready;

    private List<KeyValuePair<int, int>> normalSkins = new List<KeyValuePair<int, int>>();

    private List<KeyValuePair<int, int>> baseSkins = new List<KeyValuePair<int, int>>();

    private List<KeyValuePair<int, int>> professionalSkins = new List<KeyValuePair<int, int>>();

    private List<KeyValuePair<int, int>> legendarySkins = new List<KeyValuePair<int, int>>();

    private List<KeyValuePair<int, int>> secretWeaponSkins = new List<KeyValuePair<int, int>>();

    private List<int> baseStickers = new List<int>();

    private List<int> professionalStickers = new List<int>();

    private List<int> legendaryStickers = new List<int>();

    [Header("Others")]
    public GameObject inAppPanel;

    public GameObject shareButton;

    public GameObject backButton;

    private bool isShow;

    private void Start()
    {
        UIEventListener uIEventListener = UIEventListener.Get(finishBackground.cachedGameObject);
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

    private void RotateWeapon(GameObject go, Vector2 drag)
    {
        mWeaponCamera.Rotate(drag, true);
    }

    private void OnDisconnectedFromPhoton()
    {
        if (isShow)
        {
            Close();
        }
    }

    private void Update()
    {
        UpdateCaseWheel();
    }

    public void Show(bool skin)
    {
        isShow = true;
        isSkinCases = skin;
        if (isSkinCases)
        {
            for (int i = 0; i < SkinCases.Length; i++)
            {
                SkinCases[i].NameLabel.text = Localization.Get(SkinCases[i].Name);
                SkinCases[i].InfoLabel.text = GetCaseInfo(SkinCases[i]);
            }
            UpdateSkinsList();
        }
        else
        {
            for (int j = 0; j < StickerCases.Length; j++)
            {
                StickerCases[j].NameLabel.text = Localization.Get(StickerCases[j].Name);
                StickerCases[j].InfoLabel.text = GetCaseInfo(StickerCases[j]);
            }
            UpdateStickerList();
        }
    }

    public void Close()
    {
        isShow = false;
        caseRotate = false;
        mWeaponCamera.SetCamera(false);
        mWeaponCamera.Close();
        finishBackground.cachedGameObject.SetActive(false);
        mPanelManager.Show("Cases", true);
    }

    private string GetCaseInfo(Case selectCase)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(Localization.Get("ChanceOfDrop") + ":");
        stringBuilder.AppendLine(string.Empty);
        if ((bool)selectCase.Money)
        {
            stringBuilder.AppendLine("BS Coins -  50%");
        }
        if ((int)selectCase.Normal != 0)
        {
            stringBuilder.AppendLine(string.Concat(Localization.Get("Normal quality"), " - ", selectCase.Normal, "%"));
        }
        if ((int)selectCase.Base != 0)
        {
            stringBuilder.AppendLine(string.Concat("[2098ff]", Localization.Get("Basic quality"), "[-] - ", selectCase.Base, "%"));
        }
        if ((int)selectCase.Professional != 0)
        {
            stringBuilder.AppendLine(string.Concat("[ff3838]", Localization.Get("Professional quality"), "[-] - ", selectCase.Professional, "%"));
        }
        if ((int)selectCase.Legendary != 0)
        {
            stringBuilder.AppendLine(string.Concat("[ff2093]", Localization.Get("Legendary quality"), "[-] - ", selectCase.Legendary, "%"));
        }
        if ((int)selectCase.SecretWeapon != 0)
        {
            stringBuilder.AppendLine(string.Concat("[757575]", Localization.Get("Secret Weapon"), "[-] - ", selectCase.SecretWeapon, "%"));
        }
        return stringBuilder.ToString();
    }

    private Case GetCase(string caseName)
    {
        if (isSkinCases)
        {
            for (int i = 0; i < SkinCases.Length; i++)
            {
                if (SkinCases[i].Name == caseName)
                {
                    return SkinCases[i];
                }
            }
        }
        else
        {
            for (int j = 0; j < StickerCases.Length; j++)
            {
                if (StickerCases[j].Name == caseName)
                {
                    return StickerCases[j];
                }
            }
        }
        return null;
    }

    private int GetCaseIndex(string caseName)
    {
        if (isSkinCases)
        {
            for (int i = 0; i < SkinCases.Length; i++)
            {
                if (SkinCases[i].Name == caseName)
                {
                    return i;
                }
            }
        }
        else
        {
            for (int j = 0; j < StickerCases.Length; j++)
            {
                if (StickerCases[j].Name == caseName)
                {
                    return j;
                }
            }
        }
        return -1;
    }

    private void UpdateSkinsList()
    {
        if (normalSkins.Count != 0)
        {
            return;
        }
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            if ((bool)GameSettings.instance.Weapons[i].Lock || (bool)GameSettings.instance.Weapons[i].Secret)
            {
                continue;
            }
            for (int j = 0; j < GameSettings.instance.WeaponsStore[i].Skins.Count; j++)
            {
                if ((int)GameSettings.instance.WeaponsStore[i].Skins[j].Price == 0 && string.IsNullOrEmpty(GameSettings.instance.WeaponsStore[i].Skins[j].Temporary))
                {
                    switch (GameSettings.instance.WeaponsStore[i].Skins[j].Quality)
                    {
                        case WeaponSkinQuality.Normal:
                            normalSkins.Add(new KeyValuePair<int, int>(GameSettings.instance.Weapons[i].ID, GameSettings.instance.WeaponsStore[i].Skins[j].ID));
                            break;
                        case WeaponSkinQuality.Basic:
                            baseSkins.Add(new KeyValuePair<int, int>(GameSettings.instance.Weapons[i].ID, GameSettings.instance.WeaponsStore[i].Skins[j].ID));
                            break;
                        case WeaponSkinQuality.Professional:
                            professionalSkins.Add(new KeyValuePair<int, int>(GameSettings.instance.Weapons[i].ID, GameSettings.instance.WeaponsStore[i].Skins[j].ID));
                            break;
                        case WeaponSkinQuality.Legendary:
                            legendarySkins.Add(new KeyValuePair<int, int>(GameSettings.instance.Weapons[i].ID, GameSettings.instance.WeaponsStore[i].Skins[j].ID));
                            break;
                    }
                }
            }
        }
        for (int k = 0; k < GameSettings.instance.Weapons.Count; k++)
        {
            if ((bool)GameSettings.instance.Weapons[k].Lock || !GameSettings.instance.Weapons[k].Secret)
            {
                continue;
            }
            for (int l = 0; l < GameSettings.instance.WeaponsStore[k].Skins.Count; l++)
            {
                if (GameSettings.instance.WeaponsStore[k].Skins[l].Quality != 0 && (int)GameSettings.instance.WeaponsStore[k].Skins[l].Price == 0)
                {
                    secretWeaponSkins.Add(new KeyValuePair<int, int>(GameSettings.instance.Weapons[k].ID, GameSettings.instance.WeaponsStore[k].Skins[l].ID));
                }
            }
        }
    }

    private void UpdateStickerList()
    {
        if (baseStickers.Count != 0)
        {
            return;
        }
        for (int i = 0; i < GameSettings.instance.Stickers.Count; i++)
        {
            switch (GameSettings.instance.Stickers[i].Quality)
            {
                case StickerQuality.Basic:
                    baseStickers.Add(GameSettings.instance.Stickers[i].ID);
                    break;
                case StickerQuality.Professional:
                    professionalStickers.Add(GameSettings.instance.Stickers[i].ID);
                    break;
                case StickerQuality.Legendary:
                    legendaryStickers.Add(GameSettings.instance.Stickers[i].ID);
                    break;
            }
        }
    }

    public void OnSpeed()
    {
        duration = 1f;
        caseWheelLerp = 10f;
    }

    public void StartCaseWheel(string caseName)
    {
        if (!AccountManager.isConnect)
        {
            UIToast.Show(Localization.Get("Connection account"));
            return;
        }
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UIToast.Show("Not Internet");
            return;
        }
        Case @case = GetCase(caseName);
        duration = 8f;
        caseWheelLerp = 1f;
        selectCaseName = caseName;
        Case case2 = GetCase(caseName);
        if ((int)case2.Price != 0)
        {
            int num = ((case2.Currency != GameCurrency.Gold) ? AccountManager.GetMoney() : AccountManager.GetGold());
            if ((int)case2.Price > num)
            {
                inAppPanel.SetActive(true);
                UIToast.Show(Localization.Get("Not enough money"));
                return;
            }
        }
        finishWeaponFireStat = false;
        finishWeaponAlready = false;
        UpdateOtherscaseItems();
        if (case2.Currency == GameCurrency.Gold)
        {
            AccountManager.UpdateGold(AccountManager.GetGold() - (int)case2.Price, null, null);
            AccountManager.instance.Data.Gold = AccountManager.GetGold() - (int)case2.Price;
        }
        else
        {
            AccountManager.UpdateMoney(AccountManager.GetMoney() - (int)case2.Price, null, null);
            AccountManager.instance.Data.Money = AccountManager.GetMoney() - (int)case2.Price;
        }
        mPanelManager.Show("CaseWheel", false);
        caseItemsRoot.localPosition = new Vector3(0f, caseItemsRoot.localPosition.y, 0f);
        startRotate = Time.time;
        caseRotate = true;
        lastsoundInterval = soundInterval / 2f;
        finishPosition = (int)UnityEngine.Random.Range(finishInterval.x, finishInterval.y);
        if ((bool)@case.Money)
        {
            SetFinishMoneyData();
        }
        else
        {
            SetFinishWeaponData(@case);
        }
    }

    private void UpdateOtherscaseItems()
    {
        Case @case = GetCase(selectCaseName);
        bool flag = @case.Money;
        int max = GameSettings.instance.Stickers.Count - 1;
        for (int i = 0; i < caseItems.Length; i++)
        {
            flag = @case.Money;
            if (flag && UnityEngine.Random.value < 0.4f)
            {
                flag = false;
            }
            if (flag)
            {
                SetCaseMoneyItem(caseItems[i]);
            }
            else if (isSkinCases)
            {
                int num = UnityEngine.Random.Range(0, 100);
                KeyValuePair<int, int> keyValuePair = default(KeyValuePair<int, int>);
                keyValuePair = ((num < 25) ? (((int)@case.Normal == 0) ? baseSkins[UnityEngine.Random.Range(0, baseSkins.Count)] : normalSkins[UnityEngine.Random.Range(0, normalSkins.Count)]) : ((num < 50) ? baseSkins[UnityEngine.Random.Range(0, baseSkins.Count)] : ((num < 75) ? professionalSkins[UnityEngine.Random.Range(0, professionalSkins.Count)] : ((i <= 10 || (int)@case.SecretWeapon == 0 || num <= 98) ? legendarySkins[UnityEngine.Random.Range(0, legendarySkins.Count)] : secretWeaponSkins[UnityEngine.Random.Range(0, secretWeaponSkins.Count)]))));
                SetCaseWeaponItem(caseItems[i], keyValuePair.Key, keyValuePair.Value);
            }
            else
            {
                int index = UnityEngine.Random.Range(0, max);
                SetCaseStickerItem(caseItems[i], GameSettings.instance.Stickers[index]);
            }
        }
    }

    private void UpdateCaseWheel()
    {
        if (caseRotate)
        {
            float time = (Time.time - startRotate) / duration;
            float num = caseWheelCurve.Evaluate(time);
            float x = num * finishPosition;
            float num2 = 1f - num + caseWheelLerp;
            caseItemsRoot.localPosition = Vector3.Lerp(caseItemsRoot.localPosition, new Vector3(x, caseItemsRoot.localPosition.y, 0f), Time.deltaTime * num2);
            if (0f - caseItemsRoot.localPosition.x > lastsoundInterval && duration != 1f)
            {
                soundSource.PlayOneShot(soundSource.clip);
                lastsoundInterval += soundInterval;
            }
            if (caseItemsRoot.localPosition.x <= finishPosition + 2f)
            {
                caseRotate = false;
                StartFinishPanel();
            }
        }
    }

    private void SetFinishWeaponData(Case selectCase)
    {
        int num = 3;
        if (isSkinCases)
        {
            isFinishWeapon = true;
            int randomQualty = this.GetRandomQualty(selectCase);
            bool num2 = (int)selectCase.SecretWeapon >= UnityEngine.Random.Range(nValue.int1, nValue.int100);
            KeyValuePair<int, int> keyValuePair = default(KeyValuePair<int, int>);
            if (num2)
            {
                keyValuePair = secretWeaponSkins[UnityEngine.Random.Range(nValue.int0, secretWeaponSkins.Count)];
                if (keyValuePair.Key == 40 && UnityEngine.Random.Range(nValue.int0, nValue.int150) != 40)
                {
                    do
                    {
                        keyValuePair = secretWeaponSkins[UnityEngine.Random.Range(nValue.int0, secretWeaponSkins.Count)];
                    }
                    while (keyValuePair.Key == 40);
                    UnityEngine.MonoBehaviour.print(keyValuePair.Key);
                }
                if (keyValuePair.Key == 45 && UnityEngine.Random.Range(nValue.int0, nValue.int150) != 45)
                {
                    do
                    {
                        keyValuePair = secretWeaponSkins[UnityEngine.Random.Range(nValue.int0, secretWeaponSkins.Count)];
                    }
                    while (keyValuePair.Key == 45);
                }
            }
            else
            {
                switch (randomQualty)
                {
                    case 1:
                        keyValuePair = normalSkins[UnityEngine.Random.Range(nValue.int0, normalSkins.Count)];
                        break;
                    case 2:
                        keyValuePair = baseSkins[UnityEngine.Random.Range(nValue.int0, baseSkins.Count)];
                        break;
                    case 3:
                        keyValuePair = professionalSkins[UnityEngine.Random.Range(nValue.int0, professionalSkins.Count)];
                        break;
                    case 4:
                        keyValuePair = legendarySkins[UnityEngine.Random.Range(nValue.int0, legendarySkins.Count)];
                        break;
                }
            }
            finishWeapon = WeaponManager.GetWeaponData(keyValuePair.Key);
            finishWeaponSkin = WeaponManager.GetWeaponSkin(keyValuePair.Key, keyValuePair.Value);
            if (finishWeaponSkin.Quality != 0 && finishWeaponSkin.Quality != WeaponSkinQuality.Normal && finishWeapon.Type != WeaponType.Knife && (int)GetCase(selectCaseName).FireStat >= UnityEngine.Random.Range(1, 100))
            {
                finishWeaponFireStat = true;
            }
            else
            {
                finishWeaponAlready = AccountManager.GetWeaponSkin(finishWeapon.ID, finishWeaponSkin.ID);
            }
            AccountManager.SetWeaponSkin(finishWeapon.ID, finishWeaponSkin.ID);
            AccountManager.SetWeaponSkin2(finishWeapon.Secret, finishWeapon.ID, finishWeaponSkin.ID, null, null);
            SetCaseWeaponItem(finishItem, keyValuePair.Key, keyValuePair.Value);
        }
        else
        {
            int index = -1;
            switch (num)
            {
                case 1:
                    index = baseStickers[UnityEngine.Random.Range(nValue.int0, baseStickers.Count)];
                    break;
                case 2:
                    index = baseStickers[UnityEngine.Random.Range(nValue.int0, baseStickers.Count)];
                    break;
                case 3:
                    index = legendaryStickers[UnityEngine.Random.Range(nValue.int0, legendaryStickers.Count)];
                    break;
            }
            finishSticker = GameSettings.instance.Stickers[index];
            AccountManager.SetStickers(finishSticker.ID);
            AccountManager.AddAccountStickerFirebase(finishSticker.ID);
            SetCaseStickerItem(finishItem, finishSticker);
        }
    }

    private int GetRandomQualty(mCaseManager.Case selectCase)
    {
        int num = UnityEngine.Random.Range(0, 100);
        if (this.isSkinCases)
        {
            int num2 = (!selectCase.Money) ? 1 : 2;
            if ((selectCase.Normal + selectCase.Base + selectCase.Professional) * num2 < num)
            {
                return 4;
            }
            if ((selectCase.Normal + selectCase.Base) * num2 < num)
            {
                return 3;
            }
            if (selectCase.Normal * num2 < num)
            {
                return 2;
            }
            return 1;
        }
        else
        {
            if (selectCase.Base + selectCase.Professional < num)
            {
                return 3;
            }
            if (selectCase.Base < num)
            {
                return 2;
            }
            return 1;
        }
    }

    private void SetFinishMoneyData()
    {
        isFinishWeapon = false;
        SetCaseMoneyItem(finishItem);
    }

    private void SetCaseWeaponItem(mCaseItem caseItem, int weaponID, int ID)
    {
        WeaponSkinData weaponSkin = WeaponManager.GetWeaponSkin(weaponID, ID);
        caseItem.weaponTexture.atlas = GameSettings.instance.WeaponIconAtlas;
        caseItem.weaponTexture.cachedGameObject.SetActive(true);
        caseItem.weaponShadow.atlas = GameSettings.instance.WeaponIconAtlas;
        caseItem.weaponShadow.cachedGameObject.SetActive(true);
        caseItem.goldTexture.cachedGameObject.SetActive(false);
        caseItem.weaponTexture.spriteName = weaponID + "-" + ID;
        caseItem.weaponShadow.spriteName = weaponID + "-" + ID;
        caseItem.weaponTexture.width = (int)GameSettings.instance.WeaponsCaseSize[weaponID - 1].x;
        caseItem.weaponTexture.height = (int)GameSettings.instance.WeaponsCaseSize[weaponID - 1].y;
        caseItem.weaponShadow.width = (int)GameSettings.instance.WeaponsCaseSize[weaponID - 1].x;
        caseItem.weaponShadow.height = (int)GameSettings.instance.WeaponsCaseSize[weaponID - 1].y;
        caseItem.nameLabel.text = weaponSkin.Name;
        caseItem.qualityLabel.text = Localization.Get(weaponSkin.Quality.ToString() + " quality");
        caseItem.qualityBackground.color = GetSkinQualityColor(weaponSkin.Quality);
    }

    private void SetCaseMoneyItem(mCaseItem caseItem)
    {
        caseItem.goldTexture.cachedGameObject.SetActive(true);
        caseItem.weaponTexture.cachedGameObject.SetActive(false);
        caseItem.weaponShadow.cachedGameObject.SetActive(false);
        caseItem.goldTexture.width = 80;
        caseItem.goldTexture.height = 80;
        caseItem.goldTexture.uvRect = new Rect(0f, 0f, 1f, 1f);
        caseItem.nameLabel.text = "BS GOLD";
        caseItem.qualityLabel.text = string.Empty;
        caseItem.qualityBackground.color = new Color(0.83f, 0.584f, 0f);
    }

    private void SetCaseStickerItem(mCaseItem caseItem, StickerData sticker)
    {
        caseItem.weaponTexture.atlas = GameSettings.instance.StickersAtlas;
        caseItem.weaponTexture.cachedGameObject.SetActive(true);
        caseItem.weaponShadow.cachedGameObject.SetActive(true);
        caseItem.goldTexture.cachedGameObject.SetActive(false);
        caseItem.weaponTexture.spriteName = sticker.ID.ToString();
        caseItem.weaponShadow.spriteName = sticker.ID.ToString();
        caseItem.weaponTexture.width = 64;
        caseItem.weaponTexture.height = 64;
        caseItem.weaponShadow.width = 64;
        caseItem.weaponShadow.height = 64;
        caseItem.nameLabel.text = sticker.Name;
        caseItem.qualityLabel.text = Localization.Get(sticker.Quality.ToString() + " quality");
        caseItem.qualityBackground.color = GetStickerQualityColor(sticker.Quality);
    }

    private Color GetSkinQualityColor(WeaponSkinQuality quality)
    {
        switch (quality)
        {
            case WeaponSkinQuality.Default:
            case WeaponSkinQuality.Normal:
                return new Color(0.63f, 0.63f, 0.63f, 1f);
            case WeaponSkinQuality.Basic:
                return new Color(0.07f, 0.65f, 0.87f, 1f);
            case WeaponSkinQuality.Professional:
                return new Color(0.9f, 0f, 0f, 1f);
            case WeaponSkinQuality.Legendary:
                return new Color(0.87f, 0f, 0.38f, 1f);
            default:
                return new Color(0.63f, 0.63f, 0.63f, 1f);
        }
    }

    private Color GetStickerQualityColor(StickerQuality quality)
    {
        switch (quality)
        {
            case StickerQuality.Basic:
                return new Color(0.07f, 0.65f, 0.87f, 1f);
            case StickerQuality.Professional:
                return new Color(0.9f, 0f, 0f, 1f);
            case StickerQuality.Legendary:
                return new Color(0.87f, 0f, 0.38f, 1f);
            default:
                return new Color(0.63f, 0.63f, 0.63f, 1f);
        }
    }

    public void StartFinishPanel()
    {
        UIFade.Fade(0f, 1f, 0.3f);
        TimerManager.In(0.5f, delegate
        {
            mPanelManager.Show("CaseFinish", false);
            UIFade.Fade(1f, 0f, 0.3f);
            if (isSkinCases)
            {
                if (isFinishWeapon)
                {
                    Color skinQualityColor = GetSkinQualityColor(finishWeaponSkin.Quality);
                    if (finishWeaponSkin.Quality != 0 && finishWeaponSkin.Quality != WeaponSkinQuality.Normal && finishWeapon.Type != WeaponType.Knife && (int)GetCase(selectCaseName).FireStat >= UnityEngine.Random.Range(nValue.int1, nValue.int100))
                    {
                        finishFireStatTexture.cachedGameObject.SetActive(true);
                        AccountManager.SetFireStat(finishWeapon.ID, finishWeaponSkin.ID);
                        AccountManager.SetFireStatFireBase(true, finishWeapon.ID, finishWeaponSkin.ID, 0, null, null);
                        Debug.Log("Firestat skin");
                    }
                    else
                    {
                        finishFireStatTexture.cachedGameObject.SetActive(false);
                    }
                    finishSecretWeapon.gameObject.SetActive(finishWeapon.Secret);
                    finishQualityTable.Reposition();
                    finishBackground.color = skinQualityColor;
                    finishBackground.alpha = 0.6f;
                    finishBackground.cachedGameObject.SetActive(true);
                    finishLabel.text = string.Concat(finishWeapon.Name, " | ", finishWeaponSkin.Name);
                    finishQualityLabel.text = Localization.Get(finishWeaponSkin.Quality.ToString() + " quality");
                    finishLineSprite.color = skinQualityColor;
                    finishPanel.SetActive(true);
                    finishPanelGold.SetActive(false);
                    mWeaponCamera.SetCamera(true);
                    mWeaponCamera.SetViewportRect(new Rect(0f, 0f, 1f, 1f), 0.5f);
                    mWeaponCamera.Show(finishWeapon.Name);
                    mWeaponCamera.SetSkin(finishWeapon.ID, finishWeaponSkin.ID);
                    finishAlreadyAvailable.SetActive(finishWeaponAlready);
                    if (finishWeaponAlready == true)
                    {
                        AccountManager.instance.Data.Gold = AccountManager.GetGold() + 2;
                    }
                }
                else
                {
                    Color skinQualityColor2 = GetSkinQualityColor(WeaponSkinQuality.Normal);
                    finishBackground.color = skinQualityColor2;
                    finishBackground.alpha = 0.6f;
                    finishBackground.cachedGameObject.SetActive(true);
                    finishPanel.SetActive(false);
                    finishPanelGold.SetActive(true);
                    finishLineSprite.color = skinQualityColor2;
                    mWeaponCamera.SetCamera(true);
                    mWeaponCamera.SetViewportRect(new Rect(0f, 0f, 1f, 1f), 0.5f);
                    mWeaponCamera.Show("BS Gold");
                    finishAlreadyAvailable.SetActive(false);
                    finishFireStatTexture.cachedGameObject.SetActive(false);
                    finishSecretWeapon.gameObject.SetActive(false);
                }
            }
            else
            {
                Color stickerQualityColor = GetStickerQualityColor(finishSticker.Quality);
                finishFireStatTexture.cachedGameObject.SetActive(false);
                finishQualityTable.Reposition();
                finishSecretWeapon.gameObject.SetActive(false);
                finishBackground.color = stickerQualityColor;
                finishBackground.alpha = 0.6f;
                finishBackground.cachedGameObject.SetActive(true);
                finishLabel.text = finishSticker.Name;
                finishQualityLabel.text = Localization.Get(finishSticker.Quality.ToString() + " quality");
                finishLineSprite.color = stickerQualityColor;
                finishPanel.SetActive(true);
                finishPanelGold.SetActive(false);
                mWeaponCamera.SetCamera(true);
                mWeaponCamera.SetViewportRect(new Rect(0f, 0f, 1f, 1f), 0.5f);
                mWeaponCamera.Show("Sticker");
                mWeaponCamera.SetSkin(0, finishSticker.ID);
                finishAlreadyAvailable.SetActive(false);
            }
            EventManager.Dispatch("AccountUpdate");
        });
    }

    public void StartRewardedCase()
    {
        if (!AccountManager.isConnect)
        {
            UIToast.Show(Localization.Get("Connection account"));
            return;
        }
        mPopUp.SetActiveWaitPanel(true, Localization.Get("Loading") + "...");
        TimerManager.In(0.5f, delegate
        {
            //AdsManager.ShowRewardedVideo(RewardedVideoComplete, RewardedVideoFailed, RewardedVideoAborted, "CaseVideo");
        });
    }

    private void RewardedVideoComplete(int amount, string name)
    {
        TimerManager.In(0.3f, delegate
        {
            mPopUp.SetActiveWaitPanel(false);
            StartCaseWheel("Free");
        });
    }

    private void RewardedVideoAborted()
    {
        TimerManager.In(0.3f, delegate
        {
            UIToast.Show(Localization.Get("Cancel"));
            mPopUp.SetActiveWaitPanel(false);
        });
    }

    private void RewardedVideoFailed()
    {
        TimerManager.In(0.3f, delegate
        {
            UIToast.Show(Localization.Get("Video not available"));
            mPopUp.SetActiveWaitPanel(false);
        });
    }

    public void ShareScreenshot()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return;
        }
        backButton.SetActive(false);
        shareButton.SetActive(false);
        TimerManager.In(0.5f, delegate
        {
            string text = string.Concat(finishWeapon.Name, " | ", finishWeaponSkin.Name, "_", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            AndroidNativeFunctions.TakeScreenshot(text, delegate (string path)
            {
                backButton.SetActive(true);
                shareButton.SetActive(true);
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("#BlockStrike #BS");
                stringBuilder.AppendLine(string.Concat("I just got ", finishWeapon.Name, " | ", finishWeaponSkin.Name, " in game Block Strike"));
                stringBuilder.AppendLine("http://bit.ly/blockstrike");
                AndroidNativeFunctions.ShareScreenshot(stringBuilder.ToString(), "Block Strike", Localization.Get("Share"), path);
            });
        });
    }
}
