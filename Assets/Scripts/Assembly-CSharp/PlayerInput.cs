using System;
using DG.Tweening;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static int PlayerHelperID = -1;

    public CryptoInt3 Health = 100;

    public Team PlayerTeam;

    public CryptoFloat PlayerSpeed = 0.18f;

    public bool Dead = true;

    public bool isCursor = true;

    public bool NoDamage;

    public bool Move = true;

    public bool Look = true;

    public bool Zombie;

    public bool DamageSpeed;

    public CryptoInt DamageForce = 0;

    public bool MoveIce;

    public CryptoInt3 MaxHealth = 100;

    public bool Shift;

    public bool Climb;

    public bool Water;

    public bool MoveCrosshair;

    [Header("UFPS")]
    public vp_FPController FPController;

    public vp_FPCamera FPCamera;

    [Header("Player")]
    public Transform PlayerTransform;

    public Camera PlayerCamera;

    public Camera PlayerWeaponCamera;

    public PlayerWeapons PlayerWeapon;

    public ControllerManager Controller;

    public AudioClip[] PlayerFoosteps;

    [Disabled]
    public Vector2 MoveAxis;

    [Disabled]
    public Vector2 LookAxis;

    [Disabled]
    public float RotateCamera;

    [Header("Fall Damage")]
    public bool FallDamage;

    public CryptoFloat FallDamageThreshold = 10f;

    private bool FallingDamage;

    private float StartFallDamage;

    [Header("AFK")]
    public bool AfkEnabled;

    public float AfkDuration = 40f;

    private float AfkTimer = -1f;

    [Header("Bunny Hop")]
    public bool BunnyHopEnabled;

    public CryptoFloat BunnyHopSpeed = 0.4f;

    public CryptoFloat BunnyHopLerp = 5f;

    public CryptoFloat BunnyHopDefaultLerp = 0.5f;

    public CryptoFloat BunnyHopDefaultSpeed = 0.18f;

    private bool BunnyHopActive;

    private bool BunnyHopAutoJump;

    [Header("Surf")]
    public bool SurfEnabled;

    public CryptoFloat SurfAcceleration = 2f;

    public CryptoFloat SurfMaxSpeed = 120f;

    private CryptoFloat SurfSpeed;

    private bool Surf;

    private bool isStopSurf;

    [Header("Others")]
    public AudioSource m_AudioSource;

    public nTimer Timer;

    private bool isJump;

    private TimerData StartNoDamageInvokeData;

    private TimerData SettingsInvokeData;

    private byte GroundedDetect;

    public static PlayerInput instance;

    public bool Grounded
    {
        get
        {
            if (Water || Surf)
            {
                return false;
            }
            return mCharacterController.isGrounded;
        }
    }

    public CharacterController mCharacterController
    {
        get
        {
            return FPController.mCharacterController;
        }
    }

    private void Start()
    {
        instance = this;
        Controller = base.transform.root.GetComponent<ControllerManager>();
        SetHealth(Health);
        EventManager.AddListener("OnSettings", OnSettings);
        OnSettings();
        Timer.In(nValue.int2, true, CheckController);
        Timer.In(nValue.int1, true, CheckCamera);
        Timer.In(nValue.int2, true, UpdateValue);
    }

    [ContextMenu("Update Value")]
    private void UpdateValue()
    {
        Health.UpdateValue();
        PlayerSpeed.UpdateValue();
        DamageForce.UpdateValue();
        MaxHealth.UpdateValue();
        FallDamageThreshold.UpdateValue();
        BunnyHopSpeed.UpdateValue();
        BunnyHopLerp.UpdateValue();
        BunnyHopDefaultLerp.UpdateValue();
        BunnyHopDefaultSpeed.UpdateValue();
        SurfAcceleration.UpdateValue();
        SurfMaxSpeed.UpdateValue();
        SurfSpeed.UpdateValue();
    }

    private void OnEnable()
    {
        PlayerHelperID = -1;
        LODObject.Target = PlayerCamera.transform;
        vp_FPCamera fPCamera = FPCamera;
        fPCamera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Combine(fPCamera.BobStepCallback, new vp_FPCamera.BobStepDelegate(PlayFoosteps));
        InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
        InputManager.GetButtonUpEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonUpEvent, new InputManager.ButtonDelegate(GetButtonUp));
        InputManager.GetAxisEvent = (InputManager.AxisDelegate)Delegate.Combine(InputManager.GetAxisEvent, new InputManager.AxisDelegate(GetAxis));
        if ((bool)GameManager.startDamage)
        {
            StartNoDamage();
        }
        if (Climb)
        {
            SetClimb(false);
        }
        if (Water)
        {
            SetWater(false);
        }
        if (MoveIce)
        {
            SetMoveIce(false);
        }
        Dead = false;
        UIDeathScreen.ClearTakenDamage();
    }

    private void OnDisable()
    {
        vp_FPCamera fPCamera = FPCamera;
        fPCamera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Remove(fPCamera.BobStepCallback, new vp_FPCamera.BobStepDelegate(PlayFoosteps));
        InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
        InputManager.GetButtonUpEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonUpEvent, new InputManager.ButtonDelegate(GetButtonUp));
        InputManager.GetAxisEvent = (InputManager.AxisDelegate)Delegate.Remove(InputManager.GetAxisEvent, new InputManager.AxisDelegate(GetAxis));
        MoveAxis = Vector2.zero;
        LookAxis = Vector2.zero;
        BunnyHopAutoJump = false;
        SurfSpeed = nValue.float0;
        Dead = true;
        isJump = false;
    }

    private void GetButtonDown(string name)
    {
        switch (name)
        {
            case "Jump":
                isJump = true;
                break;
        }
    }

    private void GetButtonUp(string name)
    {
        switch (name)
        {
            case "Jump":
                isJump = false;
                break;
        }
    }

    private void GetAxis(string name, float value)
    {
        switch (name)
        {
            case "Horizontal":
                MoveAxis.x = value;
                break;
            case "Vertical":
                MoveAxis.y = value;
                break;
            case "Mouse X":
                LookAxis.x = value;
                break;
            case "Mouse Y":
                LookAxis.y = value;
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.isJump = true;
            TimerManager.In(0.1f, delegate {
                this.isJump = false;
            });
        }

        this.UpdateCursor();
        UpdateMove();
        UpdateLook();
        UpdateJump();
        UpdateBunnyHop();
        UpdateSurf();
        UpdateFallDamage();
        UpdateVelocity();
        UpdateAFK();
    }

    private void FixedUpdate()
    {
        this.UpdateSurf();
        this.UpdateFallDamage();
        this.UpdateVelocity();
    }

    private void UpdateCursor()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.P))
        {
            this.isCursor = !this.isCursor;
        }
        Screen.lockCursor = !this.isCursor;
    }

    // Token: 0x06001FCF RID: 8143 RVA: 0x000AB910 File Offset: 0x000A9B10
    private void UpdateMove()
    {
        if (this.Move)
        {
            this.MoveAxis = this.MoveAxis.normalized;
            if (this.PlayerWeapon.isScope && this.PlayerWeapon.GetSelectedWeaponData().Scope2)
            {
                this.MoveAxis /= (float)nValue.int2;
            }
            if (Application.platform != RuntimePlatform.Android)
            {
                Vector2 vec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                MoveAxis = vec.normalized;
            }
            this.FPController.OnValue_InputMoveVector = this.MoveAxis;
        }
    }

    // Token: 0x06001FD0 RID: 8144 RVA: 0x000AB98C File Offset: 0x000A9B8C
    private void UpdateLook()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            this.LookAxis.y = Input.GetAxis("Mouse Y");
            this.LookAxis.x = Input.GetAxis("Mouse X");
        }
        if (this.PlayerWeapon.isScope)
        {
            this.LookAxis *= ((!this.PlayerWeapon.GetSelectedWeaponData().Scope) ? this.PlayerWeapon.GetSelectedWeaponData().Scope2Sensitivity : this.PlayerWeapon.GetSelectedWeaponData().ScopeSensitivity);
        }
        this.FPCamera.UpdateLook(this.LookAxis);
        this.RotateCamera = -this.FPCamera.Pitch / nValue.float60;
    }

    private void UpdateJump()
    {
        if (BunnyHopAutoJump)
        {
            if (Grounded)
            {
                if (FPController.CanStartJump())
                {
                    FPController.OnStartJump();
                }
            }
            else
            {
                FPController.OnStopJump();
            }
            return;
        }
        if (MoveCrosshair && !FPController.CanStartJump())
        {
            UICrosshair.SetMove(1f);
        }
        if (isJump && !Climb && !Water)
        {
            if (FPController.CanStartJump())
            {
                FPController.OnStartJump();
                if (MoveCrosshair)
                {
                    UICrosshair.SetMove(10f);
                }
            }
        }
        else
        {
            FPController.OnStopJump();
        }
    }


    private void UpdateBunnyHop()
    {
        if (!BunnyHopEnabled)
        {
            return;
        }
        if (!Grounded && mCharacterController.velocity.sqrMagnitude > (float)nValue.int20)
        {
            if (!BunnyHopActive)
            {
                BunnyHopActive = true;
                DOTween.Kill("BunnyHop");
                DOTween.To(() => FPController.MotorAcceleration, delegate (float x)
                {
                    FPController.MotorAcceleration = x;
                }, BunnyHopSpeed, BunnyHopLerp).SetId("BunnyHop");
            }
        }
        else if (BunnyHopActive)
        {
            BunnyHopActive = false;
            DOTween.Kill("BunnyHop");
            DOTween.To(() => FPController.MotorAcceleration, delegate (float x)
            {
                FPController.MotorAcceleration = x;
            }, BunnyHopDefaultSpeed, BunnyHopDefaultLerp).SetId("BunnyHop");
        }
    }

    private void UpdateSurf()
    {
        if (!SurfEnabled)
        {
            return;
        }
        if (isStopSurf)
        {
            Surf = false;
            SurfSpeed = nValue.float0;
            FPController.Stop();
            isStopSurf = false;
            return;
        }
        if (FPController.GroundAngle > (float)nValue.int30 && mCharacterController.isGrounded)
        {
            if (!Surf)
            {
                SurfSpeed = (float)SurfSpeed + ((float)SurfAcceleration + mCharacterController.velocity.magnitude * (float)SurfAcceleration);
            }
            else
            {
                SurfSpeed = (float)SurfSpeed + (float)SurfAcceleration;
            }
            Surf = true;
        }
        else if (FPController.GroundAngle < (float)nValue.int30 && mCharacterController.isGrounded)
        {
            Surf = false;
            SurfSpeed = nValue.float0;
        }
        else if ((float)SurfSpeed > nValue.float0)
        {
            Surf = false;
            SurfSpeed = (float)SurfSpeed - (float)SurfAcceleration / (float)nValue.int3;
        }
        SurfSpeed = Mathf.Clamp(SurfSpeed, nValue.float0, SurfMaxSpeed);
        if ((float)SurfSpeed > nValue.float0)
        {
            FPController.AddForce(FPCamera.Forward * ((float)SurfSpeed * nValue.float00001 + MoveAxis.y / (float)nValue.int100));
        }
    }

    private void UpdateFallDamage()
    {
        if (!FallDamage)
        {
            return;
        }
        if (Grounded)
        {
            if (FallingDamage)
            {
                FallingDamage = false;
                if (PlayerTransform.position.y < StartFallDamage - (float)FallDamageThreshold)
                {
                    int damage = (int)(StartFallDamage - PlayerTransform.position.y);
                    DamageInfo damageInfo = DamageInfo.Get(damage, Vector3.zero, Team.None, nValue.int0, nValue.int0, -nValue.int1, false);
                    Damage(damageInfo);
                }
            }
        }
        else if (!FallingDamage)
        {
            FallingDamage = true;
            StartFallDamage = PlayerTransform.position.y;
        }
    }

    private void UpdateVelocity()
    {
        if (mCharacterController.velocity.y < (float)(-nValue.int100))
        {
            DamageInfo damageInfo = DamageInfo.Get(nValue.int1000, Vector3.zero, Team.None, nValue.int0, nValue.int0, -nValue.int1, false);
            Damage(damageInfo);
        }
    }

    public void SetBunnyHopAutoJump(bool active)
    {
        BunnyHopAutoJump = active;
    }

    public void Damage(DamageInfo damageInfo)
    {
        if (Dead || NoDamage)
        {
            return;
        }
        SetHealth((int)Health - damageInfo.damage);
        if (damageInfo.position != Vector3.zero)
        {
            UIDamage.Damage(damageInfo.position, FPCamera.Transform);
            if ((int)DamageForce > nValue.int0)
            {
                float num = (float)nValue.int1 - Vector3.Distance(PlayerTransform.position, damageInfo.position) / (float)nValue.int100;
                Vector3 force = (damageInfo.position - PlayerTransform.position).normalized * num * -(int)DamageForce / nValue.int100;
                force.y = nValue.float0;
                FPController.AddForce(force);
            }
        }
        if ((int)Health <= nValue.int0)
        {
            EventManager.Dispatch("DeadPlayer", damageInfo);
            PlayerWeapon.DeactiveScope();
            return;
        }
        PlayerHelperID = damageInfo.player;
        if (DamageSpeed)
        {
            FPController.MotorAcceleration = nValue.float013;
            if (DOTween.IsTweening("DamageSpeed"))
            {
                DOTween.Kill("DamageSpeed");
            }
            DOTween.To(() => FPController.MotorAcceleration, delegate (float x)
            {
                FPController.MotorAcceleration = x;
            }, nValue.float019, nValue.float15).SetId("DamageSpeed");
        }
        FPCamera.AddRollForce(UnityEngine.Random.Range(-nValue.int2, nValue.int2));
    }

    private void PlayFoosteps()
    {
        if (!Water && !Climb && Grounded)
        {
            UpdateFoosteps();
        }
    }

    public void UpdateFoosteps()
    {
        if (Settings.Sound)
        {
            AudioClip clip = PlayerFoosteps[UnityEngine.Random.Range(nValue.int0, PlayerFoosteps.Length)];
            m_AudioSource.pitch = UnityEngine.Random.Range(1f, 1.5f);
            m_AudioSource.clip = clip;
            m_AudioSource.Play();
            clip = null;
        }
    }

    public void StartNoDamage()
    {
        NoDamage = true;
        try
        {
            if (!((float)GameManager.startDamageTime > (float)(-nValue.int0)))
            {
                return;
            }
            if (!Timer.Contains("StartNoDamage"))
            {
                Timer.Create("StartNoDamage", GameManager.startDamageTime, delegate
                {
                    NoDamage = false;
                });
            }
            Timer.In("StartNoDamage", GameManager.startDamageTime);
        }
        catch
        {
            NoDamage = false;
        }
    }

    private void OnSettings()
    {
        if (!Timer.Contains("OnSettings"))
        {
            Timer.Create("OnSettings", nValue.float01, delegate
            {
                float num = Settings.Sensitivity * 16f;
                FPCamera.MouseSensitivity = new Vector2(num, num);
                PlayerWeaponCamera.enabled = Settings.ShowWeapon;
                Shift = Settings.ShiftButton;
            });
        }
        Timer.In("OnSettings");
    }

    public void SetHealth(int health)
    {
        Health = health;
        Health = Mathf.Clamp(Health, nValue.int0, MaxHealth);
        UIHealth.SetHealth(Health);
        Controller.SetHealth((byte)health);
    }

    public void SetMove(bool move)
    {
        Move = move;
        if (!move)
        {
            MoveAxis = Vector2.zero;
            FPController.OnValue_InputMoveVector = MoveAxis;
        }
    }

    public void SetMove(bool move, float duration)
    {
        Move = move;
        TimerManager.Cancel("MoveTime");
        TimerManager.In("MoveTime", duration, delegate
        {
            Move = !move;
        });
    }

    public void SetLook(bool look)
    {
        Look = look;
    }

    public void SetLook(bool look, float duration)
    {
        Look = look;
        TimerManager.Cancel("LookTime");
        TimerManager.In("LookTime", duration, delegate
        {
            Look = !look;
        });
    }

    public void SetMoveIce(bool active)
    {
        MoveIce = active;
        if (active)
        {
            DOTween.To(() => FPController.MotorDamping, delegate (float x)
            {
                FPController.MotorDamping = x;
            }, nValue.float002, nValue.float02);
        }
        else
        {
            DOTween.To(() => FPController.MotorDamping, delegate (float x)
            {
                FPController.MotorDamping = x;
            }, nValue.float017, nValue.float02);
        }
    }

    public void UpdatePlayerSpeed(float speed)
    {
        PlayerSpeed = speed;
        FPController.MotorAcceleration = (float)PlayerSpeed;
    }

    public void SetPlayerSpeed(float mass)
    {
        FPController.MotorAcceleration = (float)PlayerSpeed - mass;
    }

    public void SetClimb(bool active)
    {
        Climb = active;
        if (Climb)
        {
            FPController.Stop();
            FPController.PhysicsGravityModifier = nValue.int0;
            FPController.MotorFreeFly = true;
        }
        else
        {
            FPController.PhysicsGravityModifier = nValue.float02;
            FPController.MotorFreeFly = false;
        }
    }

    public void SetWater(bool active)
    {
        SetWater(active, false);
    }

    public void SetWater(bool active, bool freeGravity)
    {
        Water = active;
        if (Water)
        {
            if (freeGravity)
            {
                FPController.Stop();
            }
            FPController.PhysicsGravityModifier = nValue.float0;
            FPController.MotorFreeFly = true;
        }
        else
        {
            FPController.PhysicsGravityModifier = nValue.float02;
            FPController.MotorFreeFly = false;
        }
    }

    public void StopSurf()
    {
        if (Surf)
        {
            isStopSurf = true;
        }
    }

    private void CheckController()
    {
        if (Dead)
        {
            return;
        }
        if (mCharacterController.slopeLimit != nValue.float45)
        {
            CheckManager.Detected("Controller Error 1");
        }
        if (mCharacterController.stepOffset != nValue.float03)
        {
            CheckManager.Detected("Controller Error 2");
        }
        if (mCharacterController.center.x != (float)nValue.int0 && mCharacterController.center.y != nValue.float0745 && mCharacterController.center.z != (float)nValue.int0)
        {
            CheckManager.Detected("Controller Error 3");
        }
        if (mCharacterController.radius != nValue.float0375)
        {
            CheckManager.Detected("Controller Error 4");
        }
        if (mCharacterController.height != nValue.float149)
        {
            CheckManager.Detected("Controller Error 5");
        }
        if (mCharacterController.isGrounded)
        {
            if (Physics.CheckSphere(PlayerTransform.position, mCharacterController.radius, -1749041173))
            {
                if (GroundedDetect > 0)
                {
                    GroundedDetect--;
                }
            }
            else
            {
                GroundedDetect += (byte)nValue.int3;
            }
        }
        if (GroundedDetect >= nValue.int9)
        {
            CheckManager.Detected("Controller Error 6");
        }
    }

    private void CheckCamera()
    {
        if (Mathf.Abs(FPCamera.Transform.localPosition.x) > nValue.float15 || Mathf.Abs(FPCamera.Transform.localPosition.y) > nValue.float15 || Mathf.Abs(FPCamera.Transform.localPosition.z) > nValue.float15)
        {
            GroundedDetect++;
            if (GroundedDetect >= nValue.int3)
            {
                CheckManager.Detected("Camera Error");
            }
        }
        else if (GroundedDetect > 0)
        {
            GroundedDetect--;
        }
        if (PlayerCamera.nearClipPlane != nValue.float001)
        {
            CheckManager.Detected("Camera Error");
        }
    }

    private void UpdateAFK()
    {
        if (!AfkEnabled)
        {
            return;
        }
        if (Input.touchCount != 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                StopAFK();
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
            {
                StartAFK();
            }
        }
        if (AfkTimer != -1f && GameManager.roundState == RoundState.PlayRound)
        {
            AfkTimer -= Time.deltaTime;
            if (AfkTimer < 0f)
            {
                AFKDetection();
            }
        }
    }

    public void StartAFK()
    {
        if (AfkEnabled)
        {
            AfkTimer = AfkDuration;
        }
    }

    public void StopAFK()
    {
        AfkTimer = -1f;
    }

    private void AFKDetection()
    {
        GameManager.leaveRoomMessage = "AFK";
        PhotonNetwork.LeaveRoom();
    }
}
