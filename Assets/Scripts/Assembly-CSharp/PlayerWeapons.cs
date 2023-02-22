using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    [Serializable]
    public class PlayerWeaponData
    {
        public bool Enabled;

        public bool CustomData;

        public CryptoInt ID;

        public bool CanFire;

        public bool FireUp;

        public CryptoString Name;

        public CryptoInt FaceDamage;

        public CryptoInt BodyDamage;

        public CryptoInt HandDamage;

        public CryptoInt LegDamage;

        public CryptoFloat FireRate;

        public CryptoFloat Accuracy;

        public CryptoFloat FireAccuracy;

        public CryptoInt FireBullets;

        public CryptoFloat ReloadTime;

        public CryptoInt ReloadWarning;

        public CryptoFloat Distance;

        public CryptoFloat Mass;

        public bool Scope;

        public CryptoInt ScopeSize;

        public CryptoFloat ScopeSensitivity;

        public CryptoFloat ScopeRecoil;

        public CryptoFloat ScopeAccuracy;

        public bool Scope2;

        public CryptoInt Scope2Size;

        public CryptoFloat Scope2Sensitivity;

        public WeaponSound FireSound;

        public WeaponSound ReloadSound;

        public CryptoInt Ammo;

        public CryptoInt AmmoTotal;

        public CryptoInt AmmoMax;

        public CryptoInt Skin;

        public CryptoInt FireStat;

        public CryptoInt[] Stickers;

        public float LastFire;

        public FPWeaponShooter Script;
    }

    public WeaponType SelectedWeapon;

    public bool CanFire = true;

    public bool isDebug;

    [Disabled]
    public bool isFire;

    [Disabled]
    public bool isScope;

    [Disabled]
    public bool isReload;

    [Disabled]
    public bool Wielded;

    private float DropWeaponTime = -1f;

    public bool InfiniteAmmo;

    private bool isDryFire;

    private CryptoVector2 FireAccuracy;

    private Ray FireRay;

    private RaycastHit FireRaycastHit;

    private Transform FireRayTransform;

    private DamageInfo FireDamageInfo = default(DamageInfo);

    [Header("Weapons Data")]
    public PlayerWeaponData KnifeData = new PlayerWeaponData();

    public PlayerWeaponData PistolData = new PlayerWeaponData();

    public PlayerWeaponData RifleData = new PlayerWeaponData();

    private bool isUpdateWeaponData;

    [Header("RigidBody")]
    public bool PushRigidbody;

    public float PushRigidbodyForce = 1000f;

    [Header("Sounds")]
    public PlayerSounds Sounds;

    [Header("Others")]
    public LayerMask FireLayers;

    public Camera PlayerCamera;

    public Camera WeaponCamera;

    public nTimer Timer;

    private Dictionary<string, GameObject> WeaponObjects = new Dictionary<string, GameObject>();

    private void Start()
    {
        EventManager.AddListener<DamageInfo>("KillPlayer", KillPlayer);
        Timer.In(nValue.int2, true, UpdateValue);
    }

    [ContextMenu("UpdateValue")]
    private void UpdateValue()
    {
        FireAccuracy.UpdateValue();
        UpdateValueWeaponData(GetSelectedWeaponData());
    }

    private void UpdateValueWeaponData(PlayerWeaponData data)
    {
        data.ID.UpdateValue();
        data.FaceDamage.UpdateValue();
        data.BodyDamage.UpdateValue();
        data.HandDamage.UpdateValue();
        data.LegDamage.UpdateValue();
        data.FireRate.UpdateValue();
        data.Accuracy.UpdateValue();
        data.FireAccuracy.UpdateValue();
        data.FireBullets.UpdateValue();
        data.ReloadTime.UpdateValue();
        data.Distance.UpdateValue();
        data.Mass.UpdateValue();
        data.ScopeSize.UpdateValue();
        data.ScopeSensitivity.UpdateValue();
        data.ScopeRecoil.UpdateValue();
        data.ScopeAccuracy.UpdateValue();
        data.Scope2Size.UpdateValue();
        data.Scope2Sensitivity.UpdateValue();
        data.Ammo.UpdateValue();
        data.AmmoTotal.UpdateValue();
        data.AmmoMax.UpdateValue();
        data.Skin.UpdateValue();
        data.FireStat.UpdateValue();
    }

    private void OnEnable()
    {
        UICrosshair.SetActiveCrosshair(true);
        InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
        InputManager.GetButtonUpEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonUpEvent, new InputManager.ButtonDelegate(GetButtonUp));
    }

    private void OnDisable()
    {
        UICrosshair.SetActiveCrosshair(false);
        InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
        InputManager.GetButtonUpEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonUpEvent, new InputManager.ButtonDelegate(GetButtonUp));
        isReload = false;
        isFire = false;
        Timer.Cancel("ReloadTime");
        Timer.Cancel("WieldedTime");
    }

    private void GetButtonDown(string name)
    {
        switch (name)
        {
            case "Fire":
            case "FireSniper":
                isFire = true;
                break;
            case "Aim":
                ScopeWeapon(true);
                break;
            case "Reload":
                ReloadWeapon();
                break;
            case "Pause":
            case "Statistics":
                DeactiveScope();
                break;
            case "SelectWeapon":
                if (DropWeaponManager.enable)
                {
                    DropWeaponTime = 0f;
                }
                else
                {
                    UpdateSelectWeapon();
                }
                break;
        }
    }

    private void GetButtonUp(string name)
    {
        switch (name)
        {
            case "Fire":
            case "FireSniper":
                isFire = false;
                isDryFire = false;
                break;
            case "SelectWeapon":
                if (DropWeaponManager.enable && DropWeaponTime >= 0f)
                {
                    UpdateSelectWeapon();
                    DropWeaponTime = -1f;
                }
                break;
        }
    }

#if UNITY_EDITOR
    bool ePressed = false;
#endif

    // Token: 0x0600204F RID: 8271 RVA: 0x00015D1A File Offset: 0x00013F1A
    private void Update()
    {
#if UNITY_EDITOR
        if ((Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) && GameObject.Find("Display") != null)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (!ePressed)
                {
                    InputManager.SetButtonDown("Use");
                }
                ePressed = true;
            }
            else
            {
                ePressed = false;
                InputManager.SetButtonUp("Use");
            }
        }
        if (((Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)) && GameObject.Find("Display") != null)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                InputManager.SetButtonDown("Fire");
            }
            else
            {
                InputManager.SetButtonUp("Fire");
            }
        }
        if ((Input.GetKeyUp(KeyCode.Mouse1) && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)) && GameObject.Find("Display") != null)
        {
            this.ScopeWeapon(true);
        }
        if ((Input.GetKey(KeyCode.R) && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)) && GameObject.Find("Display") != null)
        {
            this.ReloadWeapon();
        }
        if ((Input.mouseScrollDelta.magnitude > 0.9f && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)) && GameObject.Find("Display") != null)
        {
            this.UpdateSelectWeapon();
        }
        if ((Input.GetKey(KeyCode.Alpha1) && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)) && GameObject.Find("Display") != null)
        {
            SetWeapon(WeaponType.Rifle, true);
        }
        if ((Input.GetKey(KeyCode.Alpha2) && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)) && GameObject.Find("Display") != null)
        {
            SetWeapon(WeaponType.Pistol, true);
        }
        if ((Input.GetKey(KeyCode.Alpha3) && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)) && GameObject.Find("Display") != null)
        {
            SetWeapon(WeaponType.Knife, true);
        }
#endif
        if (this.isFire)
        {
            this.FireWeapon();
        }
    }

    private void UpdateSelectWeapon()
    {
        if (SelectedWeapon == WeaponType.Knife)
        {
            if (RifleData.Enabled)
            {
                SetWeapon(WeaponType.Rifle);
            }
            else if (PistolData.Enabled)
            {
                SetWeapon(WeaponType.Pistol);
            }
        }
        else if (SelectedWeapon == WeaponType.Pistol)
        {
            if (KnifeData.Enabled)
            {
                SetWeapon(WeaponType.Knife);
            }
            else if (RifleData.Enabled)
            {
                SetWeapon(WeaponType.Rifle);
            }
        }
        else if (SelectedWeapon == WeaponType.Rifle)
        {
            if (PistolData.Enabled)
            {
                SetWeapon(WeaponType.Pistol);
            }
            else if (KnifeData.Enabled)
            {
                SetWeapon(WeaponType.Knife);
            }
        }
    }

    public PlayerWeaponData GetSelectedWeaponData()
    {
        return GetWeaponData(SelectedWeapon);
    }

    public PlayerWeaponData GetWeaponData(WeaponType weapon)
    {
        switch (weapon)
        {
            case WeaponType.Knife:
                return KnifeData;
            case WeaponType.Pistol:
                return PistolData;
            case WeaponType.Rifle:
                return RifleData;
            default:
                return null;
        }
    }

    public PlayerWeaponData GetWeaponData(int weaponID)
    {
        if ((int)KnifeData.ID == weaponID)
        {
            return KnifeData;
        }
        if ((int)PistolData.ID == weaponID)
        {
            return PistolData;
        }
        if ((int)RifleData.ID == weaponID)
        {
            return RifleData;
        }
        return null;
    }

    public void UpdateWeaponAmmoAll()
    {
        UpdateWeaponAmmo(WeaponType.Pistol);
        UpdateWeaponAmmo(WeaponType.Rifle);
        PlayerWeaponData selectedWeaponData = GetSelectedWeaponData();
        UIAmmo.SetAmmo(selectedWeaponData.Ammo, selectedWeaponData.AmmoMax, InfiniteAmmo, selectedWeaponData.ReloadWarning);
    }

    public void UpdateWeaponAmmo(WeaponType type)
    {
        if (type != WeaponType.Knife)
        {
            PlayerWeaponData weaponData = GetWeaponData(type);
            if (weaponData.Enabled)
            {
                WeaponData weaponData2 = WeaponManager.GetWeaponData(weaponData.ID);
                weaponData.Ammo = (int)weaponData2.Ammo;
                weaponData.AmmoTotal = (int)weaponData2.Ammo;
                weaponData.AmmoMax = (int)weaponData2.MaxAmmo;
            }
        }
    }

    public void UpdateWeaponAll()
    {
        UpdateWeaponAll(WeaponManager.DefaultWeaponType);
    }

    public void UpdateWeaponAll(WeaponType selectWeapon)
    {
        UpdateWeaponAll(selectWeapon, null, null, null);
    }

    public void UpdateWeaponAll(WeaponType selectWeapon, WeaponCustomData dataKnife, WeaponCustomData dataPistol, WeaponCustomData dataRifle)
    {
        isUpdateWeaponData = true;
        WeaponCamera.cullingMask = 0;
        DeactiveScope();
        DeactiveAll();
        UpdateWeaponData(WeaponType.Knife, dataKnife);
        UpdateWeaponData(WeaponType.Pistol, dataPistol);
        UpdateWeaponData(WeaponType.Rifle, dataRifle);
        TimerManager.In(nValue.float005, delegate
        {
            SetWeapon(selectWeapon, false);
            isUpdateWeaponData = false;
            WeaponCamera.cullingMask = int.MinValue;
        });
        UpdateShowButtons();
    }

    public void UpdateWeaponAll(WeaponType selectWeapon, int knife, int pistol, int rifle, WeaponCustomData dataKnife, WeaponCustomData dataPistol, WeaponCustomData dataRifle)
    {
        isUpdateWeaponData = true;
        WeaponCamera.cullingMask = 0;
        DeactiveScope();
        DeactiveAll();
        UpdateWeaponData(knife, dataKnife);
        UpdateWeaponData(pistol, dataPistol);
        UpdateWeaponData(rifle, dataRifle);
        TimerManager.In(nValue.float005, delegate
        {
            SetWeapon(selectWeapon, false);
            isUpdateWeaponData = false;
            WeaponCamera.cullingMask = int.MinValue;
        });
        UpdateShowButtons();
    }

    public void UpdateWeapon(WeaponType type, bool isSelectWeapon)
    {
        UpdateWeapon(type, isSelectWeapon, null);
    }

    public void UpdateWeapon(WeaponType type, bool isSelectWeapon, WeaponCustomData customData)
    {
        int weaponID = WeaponManager.GetSelectWeapon(type);
        TimerManager.In(nValue.float005, delegate
        {
            isUpdateWeaponData = true;
            DeactiveAll();
            WeaponManager.SetSelectWeapon(type, weaponID);
            UpdateWeaponData(type, customData);
            TimerManager.In(nValue.float005, delegate
            {
                if (isSelectWeapon)
                {
                    SetWeapon(type, false);
                }
                else
                {
                    SetWeapon(SelectedWeapon, false);
                }
                isUpdateWeaponData = false;
            });
            UpdateShowButtons();
        });
    }

    public void UpdateWeapon(int weaponID, bool isSelectWeapon)
    {
        UpdateWeapon(weaponID, isSelectWeapon, null);
    }

    public void UpdateWeapon(int weaponID, bool isSelectWeapon, WeaponCustomData customData)
    {
        TimerManager.In(nValue.float005, delegate
        {
            isUpdateWeaponData = true;
            DeactiveAll();
            UpdateWeaponData(weaponID, customData);
            TimerManager.In(nValue.float005, delegate
            {
                if (isSelectWeapon)
                {
                    SetWeapon(WeaponManager.GetWeaponData(weaponID).Type, false);
                }
                else
                {
                    SetWeapon(SelectedWeapon, false);
                }
                isUpdateWeaponData = false;
            });
            UpdateShowButtons();
        });
    }

    private void UpdateShowButtons()
    {
        int num = nValue.int0;
        if (KnifeData.Enabled)
        {
            num++;
        }
        if (PistolData.Enabled)
        {
            num++;
        }
        if (RifleData.Enabled)
        {
            num++;
        }
        if (num >= nValue.int2)
        {
            UIControllerList.SelectWeapon.cachedGameObject.SetActive(true);
        }
        else
        {
            UIControllerList.SelectWeapon.cachedGameObject.SetActive(false);
        }
    }

    private void UpdateWeaponData(WeaponType weaponType)
    {
        UpdateWeaponData(weaponType, null);
    }

    private void UpdateWeaponData(int weaponID)
    {
        UpdateWeaponData(weaponID, null);
    }

    private void UpdateWeaponData(WeaponType weaponType, WeaponCustomData customData)
    {
        WeaponData data = null;
        switch (weaponType)
        {
            case WeaponType.Knife:
                if (WeaponManager.HasSelectWeapon(WeaponType.Knife))
                {
                    data = WeaponManager.GetSelectWeaponData(WeaponType.Knife);
                }
                break;
            case WeaponType.Pistol:
                if (WeaponManager.HasSelectWeapon(WeaponType.Pistol))
                {
                    data = WeaponManager.GetSelectWeaponData(WeaponType.Pistol);
                }
                break;
            case WeaponType.Rifle:
                if (WeaponManager.HasSelectWeapon(WeaponType.Rifle))
                {
                    data = WeaponManager.GetSelectWeaponData(WeaponType.Rifle);
                }
                break;
        }
        UpdateWeaponData(weaponType, data, true, customData);
    }

    private void UpdateWeaponData(int weaponID, WeaponCustomData customData)
    {
        WeaponData weaponData = WeaponManager.GetWeaponData(weaponID);
        UpdateWeaponData(weaponData.Type, weaponData, true, customData);
    }

    private void UpdateWeaponData(WeaponType weaponType, WeaponData data, bool isUpgrade, WeaponCustomData customData)
    {
        PlayerWeaponData playerWeaponData = new PlayerWeaponData();
        if (data != null && !WeaponObjects.ContainsKey(data.Name) && data.FpsPrefab != null)
        {
            GameObject fpsPrefab = data.FpsPrefab;
            fpsPrefab = Utils.AddChild(fpsPrefab, GameManager.player.FPCamera.transform);
            WeaponObjects.Add(data.Name, fpsPrefab);
            fpsPrefab.SetActive(true);
        }
        if (data != null)
        {
            playerWeaponData.Enabled = true;
            playerWeaponData.CustomData = false;
            playerWeaponData.ID = (int)data.ID;
            playerWeaponData.CanFire = data.CanFire;
            playerWeaponData.Name = (string)data.Name;
            playerWeaponData.FaceDamage = (int)data.FaceDamage;
            playerWeaponData.BodyDamage = (int)data.BodyDamage;
            playerWeaponData.HandDamage = (int)data.HandDamage;
            playerWeaponData.LegDamage = (int)data.LegDamage;
            playerWeaponData.FireRate = (float)data.FireRate;
            playerWeaponData.Accuracy = (float)data.Accuracy;
            playerWeaponData.FireAccuracy = (float)data.FireAccuracy;
            playerWeaponData.Ammo = (int)data.Ammo;
            playerWeaponData.AmmoTotal = (int)data.Ammo;
            playerWeaponData.AmmoMax = (int)data.MaxAmmo;
            playerWeaponData.Mass = (float)data.Mass;
            playerWeaponData.FireBullets = (int)data.FireBullets;
            playerWeaponData.ReloadTime = (float)data.ReloadTime;
            playerWeaponData.ReloadWarning = (int)data.ReloadWarning;
            playerWeaponData.Distance = (float)data.Distance;
            playerWeaponData.Scope = data.Scope;
            playerWeaponData.ScopeSize = (int)data.ScopeSize;
            playerWeaponData.ScopeSensitivity = (float)data.ScopeSensitivity;
            playerWeaponData.ScopeRecoil = (float)data.ScopeRecoil;
            playerWeaponData.ScopeAccuracy = (float)data.ScopeAccuracy;
            playerWeaponData.Scope2 = data.Scope2;
            playerWeaponData.Scope2Size = (int)data.Scope2Size;
            playerWeaponData.Scope2Sensitivity = (float)data.Scope2Sensitivity;
            playerWeaponData.FireSound = data.FireSound;
            playerWeaponData.ReloadSound = data.ReloadSound;
            playerWeaponData.Skin = AccountManager.GetWeaponSkinSelected(playerWeaponData.ID);
            playerWeaponData.FireStat = AccountManager.GetFireStatCounter(playerWeaponData.ID, playerWeaponData.Skin);
            playerWeaponData.Stickers = ConvertArray(AccountManager.GetWeaponStickers(playerWeaponData.ID, playerWeaponData.Skin).ToArray());
            playerWeaponData.LastFire = 0f;
            playerWeaponData.Script = WeaponObjects[data.Name].GetComponent<FPWeaponShooter>();
            if (customData != null)
            {
                playerWeaponData.CustomData = customData.CustomData;
                if ((int)customData.FaceDamage != -nValue.int1)
                {
                    playerWeaponData.FaceDamage = (int)customData.FaceDamage;
                }
                if ((int)customData.BodyDamage != -nValue.int1)
                {
                    playerWeaponData.BodyDamage = (int)customData.BodyDamage;
                }
                if ((int)customData.HandDamage != -nValue.int1)
                {
                    playerWeaponData.HandDamage = (int)customData.HandDamage;
                }
                if ((int)customData.LegDamage != -nValue.int1)
                {
                    playerWeaponData.LegDamage = (int)customData.LegDamage;
                }
                if ((int)customData.Ammo != -nValue.int1)
                {
                    playerWeaponData.Ammo = (int)customData.Ammo;
                }
                if ((int)customData.AmmoMax != -nValue.int1)
                {
                    playerWeaponData.AmmoMax = (int)customData.AmmoMax;
                }
                if ((int)customData.AmmoTotal != -nValue.int1)
                {
                    playerWeaponData.AmmoTotal = (int)customData.AmmoTotal;
                }
                if (customData.Mass != 0f - nValue.float1)
                {
                    playerWeaponData.Mass = customData.Mass;
                }
                if ((int)customData.Skin != -nValue.int1)
                {
                    playerWeaponData.Skin = (int)customData.Skin;
                }
                if ((int)customData.FireStatCounter >= 0)
                {
                    playerWeaponData.FireStat = (int)customData.FireStatCounter;
                }
                else if (playerWeaponData.CustomData)
                {
                    playerWeaponData.FireStat = -1;
                }
                if (customData.Stickers != null && customData.Stickers.Length > 0)
                {
                    playerWeaponData.Stickers = customData.Stickers;
                }
                else if (playerWeaponData.CustomData)
                {
                    playerWeaponData.Stickers = new CryptoInt[0];
                }
            }
            playerWeaponData.Script.UpdateWeaponData(playerWeaponData);
        }
        else
        {
            playerWeaponData.Enabled = false;
        }
        switch (weaponType)
        {
            case WeaponType.Knife:
                KnifeData = playerWeaponData;
                break;
            case WeaponType.Pistol:
                PistolData = playerWeaponData;
                break;
            case WeaponType.Rifle:
                RifleData = playerWeaponData;
                break;
        }
    }

    public void SetWeapon(WeaponType weapon)
    {
        SetWeapon(weapon, true);
    }

    public void SetWeapon(WeaponType weapon, bool checkSelectedWeapon)
    {
        if ((checkSelectedWeapon && SelectedWeapon == weapon) || !GetWeaponData(weapon).Enabled)
        {
            return;
        }
        DeactiveScope();
        DeactiveAll();
        SelectedWeapon = weapon;
        PlayerWeaponData selectedWeaponData = GetSelectedWeaponData();
        UICrosshair.SetAccuracy(selectedWeaponData.Accuracy);
        UIAmmo.SetAmmo(selectedWeaponData.Ammo, selectedWeaponData.AmmoMax, InfiniteAmmo, selectedWeaponData.ReloadWarning);
        UIControllerList.Aim.cachedGameObject.SetActive(selectedWeaponData.Scope || selectedWeaponData.Scope2);
        GameManager.player.SetPlayerSpeed(selectedWeaponData.Mass);
        Timer.Cancel("ReloadTime");
        isReload = false;
        Sounds.Stop();
        Timer.Cancel("WieldedTime");
        Wielded = true;
        if (!Timer.Contains("WieldedTime"))
        {
            Timer.Create("WieldedTime", nValue.float05, delegate
            {
                Wielded = false;
            });
        }
        Timer.In("WieldedTime");
        switch (weapon)
        {
            case WeaponType.Knife:
                KnifeData.Script.Active();
                GameManager.controller.SetWeapon(KnifeData);
                break;
            case WeaponType.Pistol:
                PistolData.Script.Active();
                GameManager.controller.SetWeapon(PistolData);
                break;
            case WeaponType.Rifle:
                RifleData.Script.Active();
                GameManager.controller.SetWeapon(RifleData);
                break;
        }
    }

    private void DeactiveAll()
    {
        if (KnifeData.Script != null)
        {
            KnifeData.Script.Deactive();
        }
        if (PistolData.Script != null)
        {
            PistolData.Script.Deactive();
        }
        if (RifleData.Script != null)
        {
            RifleData.Script.Deactive();
        }
    }

    public void FireWeapon()
    {
        nProfiler.BeginSample("PlayerWeapons.FireWeapon");
        if (CanFire && !isReload && GameManager.roundState != RoundState.EndRound)
        {
            switch (SelectedWeapon)
            {
                case WeaponType.Knife:
                    Fire(KnifeData);
                    break;
                case WeaponType.Pistol:
                    Fire(PistolData);
                    break;
                case WeaponType.Rifle:
                    Fire(RifleData);
                    break;
            }
            nProfiler.EndSample();
        }
    }

    private void Fire(PlayerWeaponData weapon)
    {
        nProfiler.BeginSample("PlayerWeapons.Fire");
        if (Wielded || (weapon.FireUp && isFire) || weapon.LastFire > Time.time || isUpdateWeaponData || !weapon.CanFire)
        {
            return;
        }
        if ((bool)weapon.Script.Knife.Enabled)
        {
            weapon.LastFire = Time.time + (float)weapon.FireRate;
            Sounds.Play(weapon.FireSound);
            weapon.Script.Fire();
            TimerManager.In(weapon.Script.Knife.Delay, delegate
            {
                FireData(weapon);
            });
        }
        else if ((int)weapon.Ammo > 0)
        {
            --weapon.Ammo;
            weapon.LastFire = Time.time + (float)weapon.FireRate;
            Sounds.Play(weapon.FireSound);
            weapon.Script.Fire();
            FireData(weapon);
            if (isScope && weapon.Scope && (float)weapon.ScopeRecoil != (float)nValue.int0)
            {
                GameManager.player.FPCamera.Pitch -= weapon.ScopeRecoil;
                float num = (float)weapon.ScopeRecoil / (float)nValue.int2;
                GameManager.player.FPCamera.Yaw += UnityEngine.Random.Range(0f - num, num);
            }
        }
        else if ((int)weapon.AmmoMax == 0)
        {
            DryFire(weapon);
        }
        else
        {
            Reload(weapon);
        }
        nProfiler.EndSample();
    }

    private void FireData(PlayerWeaponData weapon)
    {
        nProfiler.BeginSample("PlayerWeapons.FireData");
        UIAmmo.SetAmmo(weapon.Ammo, weapon.AmmoMax, InfiniteAmmo, weapon.ReloadWarning);
        DecalInfo decalInfo = DecalInfo.Get();
        if (SelectedWeapon == WeaponType.Knife)
        {
            decalInfo.isKnife = true;
        }
        for (int i = nValue.int0; i < (int)weapon.FireBullets; i++)
        {
            if (isScope && weapon.Scope)
            {
                FireAccuracy = UICrosshair.Fire(weapon.ScopeAccuracy);
            }
            else
            {
                FireAccuracy = UICrosshair.Fire(weapon.FireAccuracy);
            }
            FireAccuracy.x = UnityEngine.Random.Range(0f - FireAccuracy.x, FireAccuracy.x);
            FireAccuracy.y = UnityEngine.Random.Range(0f - FireAccuracy.y, FireAccuracy.y);
            FireRay = PlayerCamera.ViewportPointToRay(new Vector3(nValue.float05 + FireAccuracy.x, nValue.float05 + FireAccuracy.y, nValue.int0));
            if (nRaycast.RaycastFire(FireRay, weapon.Distance, FireLayers))
            {
                nProfiler.BeginSample("nRaycast.RaycastFire => True");
                nProfiler.BeginSample("1");
                if (decalInfo.BloodDecal == nValue.int200)
                {
                    decalInfo.BloodDecal = (byte)decalInfo.Points.size;
                    decalInfo.Points.Add(nRaycast.hitPoint);
                    decalInfo.Normals.Add(Vector3.zero);
                }
                nProfiler.EndSample();
                nProfiler.BeginSample("2");
                FireDamageInfo.Set(weapon.BodyDamage, GameManager.player.PlayerTransform.position, GameManager.player.PlayerTeam, weapon.ID, weapon.Skin, PhotonNetwork.player.ID, false);
                nProfiler.EndSample();
                nProfiler.BeginSample("3");
                nRaycast.hitBoxCollider.playerDamage.Damage(FireDamageInfo);
                nProfiler.EndSample();
                nProfiler.BeginSample("SparkEffectManager.Fire");
                if (!isScope)
                {
                    SparkEffectManager.Fire(nRaycast.hitPoint, nRaycast.hitDistance);
                }
                nProfiler.EndSample();
            }
            else
            {
                if (!Physics.Raycast(FireRay, out FireRaycastHit, weapon.Distance, FireLayers))
                {
                    continue;
                }
                FireRayTransform = FireRaycastHit.transform;
                if (FireRayTransform.CompareTag("IgnoreDecal") || FireRayTransform.CompareTag("PlayerSkin"))
                {
                    continue;
                }
                if (FireRayTransform.CompareTag("RigidbodyObject") && PushRigidbody)
                {
                    FireRaycastHit.transform.GetComponent<RigidbodyObject>().Force(PlayerCamera.transform.forward * PushRigidbodyForce);
                    continue;
                }
                if (FireRayTransform.CompareTag("DamageObject"))
                {
                    FireDamageInfo = DamageInfo.Get(weapon.BodyDamage, GameManager.player.PlayerTransform.position, GameManager.player.PlayerTeam, weapon.ID, weapon.Skin, PhotonNetwork.player.ID, false);
                    FireRaycastHit.transform.SendMessage("Damage", FireDamageInfo, SendMessageOptions.DontRequireReceiver);
                    continue;
                }
                decalInfo.Points.Add(FireRaycastHit.point);
                decalInfo.Normals.Add(FireRaycastHit.normal);
                if (!isScope)
                {
                    SparkEffectManager.Fire(FireRaycastHit.point, FireRaycastHit.distance);
                }
            }
        }
        GameManager.controller.FireWeapon(decalInfo);
        nProfiler.EndSample();
    }

    private void DryFire(PlayerWeaponData weapon)
    {
        if (!isDryFire)
        {
            isDryFire = true;
            weapon.Script.DryFire();
            Sounds.Play(WeaponSound.AmmoEmpty);
        }
    }

    public void ReloadWeapon()
    {
        if (isReload)
        {
            return;
        }
        switch (SelectedWeapon)
        {
            case WeaponType.Knife:
                if (KnifeData.Script != null)
                {
                    KnifeData.Script.StartInspectWeapon();
                }
                break;
            case WeaponType.Pistol:
                Reload(PistolData);
                break;
            case WeaponType.Rifle:
                Reload(RifleData);
                break;
        }
    }

    private void Reload(PlayerWeaponData weapon)
    {
        if (!weapon.Enabled || Wielded)
        {
            return;
        }
        if (isScope)
        {
            DeactiveScope();
        }
        if ((int)weapon.Ammo == (int)weapon.AmmoTotal || (int)weapon.AmmoMax == nValue.int0)
        {
            weapon.Script.StartInspectWeapon();
            return;
        }
        isReload = true;
        Sounds.Play(weapon.ReloadSound);
        weapon.Script.Reload(weapon.ReloadTime);
        GameManager.controller.ReloadWeapon();
        if (!Timer.Contains("ReloadTime"))
        {
            Timer.Create("ReloadTime", (float)weapon.ReloadTime + nValue.float05, delegate
            {
                PlayerWeaponData selectedWeaponData = GetSelectedWeaponData();
                isReload = false;
                if (InfiniteAmmo)
                {
                    selectedWeaponData.Ammo = selectedWeaponData.AmmoTotal;
                }
                else if ((int)selectedWeaponData.AmmoMax > (int)selectedWeaponData.AmmoTotal)
                {
                    selectedWeaponData.AmmoMax = (int)selectedWeaponData.AmmoMax - ((int)selectedWeaponData.AmmoTotal - (int)selectedWeaponData.Ammo);
                    selectedWeaponData.Ammo = selectedWeaponData.AmmoTotal;
                }
                else
                {
                    int num = selectedWeaponData.Ammo;
                    selectedWeaponData.Ammo = (int)selectedWeaponData.Ammo + (int)selectedWeaponData.AmmoMax;
                    selectedWeaponData.Ammo = Mathf.Min(selectedWeaponData.AmmoTotal, selectedWeaponData.Ammo);
                    selectedWeaponData.AmmoMax = (int)selectedWeaponData.AmmoMax - ((int)selectedWeaponData.Ammo - num);
                    selectedWeaponData.AmmoMax = Mathf.Max(nValue.int0, selectedWeaponData.AmmoMax);
                }
                UIAmmo.SetAmmo(selectedWeaponData.Ammo, selectedWeaponData.AmmoMax, InfiniteAmmo, selectedWeaponData.ReloadWarning);
            });
        }
        Timer.In("ReloadTime", (float)weapon.ReloadTime + nValue.float05);
    }

    private void ScopeWeapon(bool check)
    {
        if ((check && isReload) || (!GetSelectedWeaponData().Scope && !GetSelectedWeaponData().Scope2 && check))
        {
            return;
        }
        isScope = !isScope;
        LODObject.isScope = isScope;
        DOTween.Kill("Scope");
        DOTween.Kill("Scope2");
        if (isScope)
        {
            if (GetSelectedWeaponData().Scope)
            {
                PlayerCamera.DOFieldOfView((int)GetSelectedWeaponData().ScopeSize, nValue.float02).id = "Scope";
                WeaponCamera.fieldOfView = nValue.int1;
                UICrosshair.SetActiveScope(true);
            }
            else if (GetSelectedWeaponData().Scope2)
            {
                PlayerCamera.DOFieldOfView((int)GetSelectedWeaponData().Scope2Size, nValue.float02).id = "Scope";
                WeaponCamera.DOFieldOfView((int)GetSelectedWeaponData().Scope2Size, nValue.float02).id = "Scope2";
            }
        }
        else
        {
            DOTween.Kill("Scope");
            DOTween.Kill("Scope2");
            PlayerCamera.DOFieldOfView(nValue.float60, nValue.float02).id = "Scope";
            if (GetSelectedWeaponData().Scope2)
            {
                WeaponCamera.DOFieldOfView(nValue.float60, nValue.float02).id = "Scope2";
            }
            else
            {
                WeaponCamera.fieldOfView = nValue.float60;
            }
            UICrosshair.SetActiveScope(false);
        }
        GetSelectedWeaponData().Script.ScopeRifle();
    }

    public void DeactiveScope()
    {
        if (isScope)
        {
            ScopeWeapon(false);
        }
        if (GameManager.player.Dead)
        {
            UICrosshair.SetActiveCrosshair(false);
        }
    }

    private void KillPlayer(DamageInfo damageInfo)
    {
        if (!LevelManager.customScene)
        {
            PlayerWeaponData playerWeaponData = null;
            if (PistolData.Enabled && (int)PistolData.ID == damageInfo.weapon && PistolData.Script.FireStat.Enabled)
            {
                playerWeaponData = PistolData;
            }
            else if (RifleData.Enabled && (int)RifleData.ID == damageInfo.weapon && RifleData.Script.FireStat.Enabled)
            {
                playerWeaponData = RifleData;
            }
            if (playerWeaponData != null && !playerWeaponData.CustomData)
            {
                int num = AccountManager.GetFireStatCounter(damageInfo.weapon, damageInfo.weaponSkin) + nValue.int1;
                playerWeaponData.Script.UpdateFireStat(num);
                PlayerRoundManager.SetFireStat1(damageInfo.weapon, damageInfo.weaponSkin);
                AccountManager.SetFireStatCounter(damageInfo.weapon, damageInfo.weaponSkin, num);
                GameManager.controller.UpdateFireStatValue(playerWeaponData.ID);
            }
        }
    }

    public void DropWeapon()
    {
        DropWeapon(SelectedWeapon, false);
    }

    public void DropWeapon(bool updateWeapon)
    {
        DropWeapon(SelectedWeapon, updateWeapon);
    }

    public void DropWeapon(WeaponType type)
    {
        DropWeapon(type, false);
    }

    public void DropWeapon(WeaponType type, bool updateWeapon)
    {
        if (type == WeaponType.Knife)
        {
            return;
        }
        PlayerWeaponData weaponData = GetWeaponData(type);
        if (weaponData.Enabled)
        {
            bool flag = DropWeaponManager.CreateWeapon(false, (byte)(int)weaponData.ID, (byte)(int)weaponData.Skin, true, GameManager.player.PlayerTransform.position, GameManager.player.PlayerTransform.eulerAngles, StickersToByteArray(weaponData.Stickers), weaponData.FireStat, weaponData.Ammo, weaponData.AmmoMax);
            if (updateWeapon && flag)
            {
                weaponData.Enabled = false;
                UpdateSelectWeapon();
            }
        }
    }

    private byte[] StickersToByteArray(CryptoInt[] stickers)
    {
        byte[] array = new byte[stickers.Length];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = (byte)(int)stickers[i];
        }
        return array;
    }

    public byte[] SerializeWeapons()
    {
        byte[] array = new byte[6]
        {
            (byte)(int)GetSelectedWeaponData().ID,
            (byte)(int)GetSelectedWeaponData().Skin,
            0,
            0,
            0,
            0
        };
        byte b = 2;
        if (SelectedWeapon != WeaponType.Rifle)
        {
            array[b] = (byte)(int)RifleData.ID;
            array[b + 1] = (byte)(int)RifleData.Skin;
            b = (byte)(b + 2);
        }
        if (SelectedWeapon != WeaponType.Pistol)
        {
            array[b] = (byte)(int)PistolData.ID;
            array[b + 1] = (byte)(int)PistolData.Skin;
            b = (byte)(b + 2);
        }
        if (SelectedWeapon != WeaponType.Knife)
        {
            array[b] = (byte)(int)KnifeData.ID;
            array[b + 1] = (byte)(int)KnifeData.Skin;
        }
        for (int i = 0; i < array.Length; i++)
        {
            MonoBehaviour.print(array[i]);
        }
        return array;
    }

    public CryptoInt[] ConvertArray(int[] array)
    {
        if (array == null)
        {
            return new CryptoInt[0];
        }
        CryptoInt[] array2 = new CryptoInt[array.Length];
        for (int i = 0; i < array2.Length; i++)
        {
            array2[i] = array[i];
        }
        return array2;
    }

    public int[] ConvertArray(CryptoInt[] array)
    {
        if (array == null)
        {
            return new int[0];
        }
        int[] array2 = new int[array.Length];
        for (int i = 0; i < array2.Length; i++)
        {
            array2[i] = array[i];
        }
        return array2;
    }
}
