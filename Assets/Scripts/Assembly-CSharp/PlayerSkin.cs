using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerSkin : MonoBehaviour
{
	public bool isPlayerActive;

	public byte Health;

	public Team PlayerTeam;

	private bool StartDamage;

	public ControllerManager Controller;

	public PlayerAnimator PlayerAnimator;

	public PlayerSkinRagdoll PlayerRagdoll;

	public Transform[] PlayerLimbs;

	public PlayerSkinAtlas PlayerAtlas;

	public Transform[] PlayerWeaponContainers;

	public Transform PlayerWeaponRoot;

	public Transform PlayerTwoWeaponRoot;

	public AudioClip[] PlayerFoosteps;

	public AudioSource cachedAudioSource;

	public Transform PlayerTransform;

	public Transform PlayerSpectatePoint;

	public GameObject PlayerRoot;

	public PlayerSkinDamage[] PlayerDamages;

	public nTimer Timer;

	public PlayerSounds Sounds;

	private bool Sound = true;

	private bool ShowDamage;

	public float PhotonSpeed = 10f;

	public Vector3 PhotonPosition = Vector3.zero;

	public Quaternion PhotonRotation = Quaternion.identity;

	public bool Dead;

	private float Move;

	public float Rotate;

	private Tweener RotateTween;

	private bool Foostep;

	private bool visible;

	public int HeadSkin = -1;

	public int BodySkin = -1;

	public int LegsSkin = -1;

	private bool activeRagdoll;

	public TPWeaponShooter SelectWeapon;

	private List<TPWeaponShooter> WeaponsList = new List<TPWeaponShooter>();

	public void Start()
	{
		if (Controller == null)
		{
			Controller = PlayerTransform.root.GetComponent<ControllerManager>();
		}
		Timer.In(0.15f, true, CheckPosition);
		Timer.In(0.12f, true, UpdateMove);
		EventManager.AddListener("OnSettings", UpdateSettings);
		UpdateSettings();
		if (RotateTween != null)
		{
			RotateTween.Kill();
		}
		RotateTween = DOTween.To(() => Rotate, delegate(float x)
		{
			Rotate = x;
		}, 0f, 0.15f).SetAutoKill(false).SetUpdate(UpdateType.Late)
			.OnUpdate(UpdateRotate)
			.SetEase(Ease.Linear);
	}

	public void OnDefault()
	{
		PlayerRoot.SetActive(false);
		PhotonPosition = Vector3.zero;
		PhotonRotation = Quaternion.identity;
		isPlayerActive = false;
		Health = 0;
		PlayerTeam = Team.None;
		Dead = false;
		Rotate = 0f;
		HeadSkin = -1;
		BodySkin = -1;
		LegsSkin = -1;
		StartDamage = false;
		Sound = true;
		ShowDamage = false;
		Move = 0f;
		Foostep = false;
		visible = false;
		if (activeRagdoll)
		{
			DeactiveRagdoll();
		}
		activeRagdoll = false;
		Sounds.OnDefault();
		PlayerAnimator.OnDefault();
	}

	private void UpdateSettings()
	{
		Sound = Settings.Sound;
		ShowDamage = Settings.ShowDamage;
	}

	public void OnEnableRoot()
	{
		Foostep = false;
		isPlayerActive = true;
		PlayerAnimator.SetDefault();
		RotateTween.Pause();
	}

	public void OnDisableRoot()
	{
		isPlayerActive = false;
		RotateTween.Play();
	}

	private void CheckPosition()
	{
		nProfiler.BeginSample("CheckPosition");
		if (visible && !Dead && (PlayerTransform.localPosition.x - PhotonPosition.x > 3f || PlayerTransform.localPosition.y - PhotonPosition.y > 3f || PlayerTransform.localPosition.z - PhotonPosition.z > 3f))
		{
			PlayerTransform.localPosition = PhotonPosition;
		}
		nProfiler.EndSample();
	}

	private void LateUpdate()
	{
		if (visible)
		{
			if (PlayerTransform.localPosition != PhotonPosition)
			{
				PlayerTransform.localPosition = Vector3Lerp(PlayerTransform.localPosition, PhotonPosition, Time.deltaTime * PhotonSpeed);
			}
			if (PlayerTransform.localRotation != PhotonRotation)
			{
				PlayerTransform.localRotation = Quaternion.Lerp(PlayerTransform.localRotation, PhotonRotation, Time.deltaTime * PhotonSpeed);
			}
		}
		else
		{
			if (PlayerTransform.localPosition != PhotonPosition)
			{
				PlayerTransform.localPosition = PhotonPosition;
			}
			if (PlayerTransform.localRotation != PhotonRotation)
			{
				PlayerTransform.localRotation = PhotonRotation;
			}
		}
	}

	private Vector3 Vector3Lerp(Vector3 from, Vector3 to, float t)
	{
		from.x += (to.x - from.x) * t;
		from.y += (to.y - from.y) * t;
		from.z += (to.z - from.z) * t;
		return from;
	}

	private void UpdateMove()
	{
		nProfiler.BeginSample("UpdateMove");
		if (isPlayerActive)
		{
			if (visible)
			{
				PlayerAnimator.move = Move;
			}
			if (!Foostep && Sound && Mathf.Abs(Move) >= 0.6f)
			{
				UpdateFoosteps();
			}
		}
		nProfiler.EndSample();
	}

	private void UpdateRotate()
	{
		PlayerAnimator.rotate = Rotate;
	}

	public void SetMove(float move)
	{
		Move = move;
	}

	public float GetMove()
	{
		return Move;
	}

	public void SetRotate(float rotate)
	{
		if (visible && !Dead)
		{
			if (RotateTween != null)
			{
				RotateTween.ChangeStartValue(Rotate);
				RotateTween.ChangeEndValue(rotate).Restart();
			}
		}
		else
		{
			Rotate = rotate;
			UpdateRotate();
		}
	}

	public void SetGrounded(bool grounded)
	{
		if (isPlayerActive)
		{
			PlayerAnimator.grounded = grounded;
		}
	}

	public void SetPlayerVisible(bool value)
	{
		visible = value;
		PlayerAnimator.visible = value;
		if (!visible)
		{
			PlayerAnimator.move = 0f;
			RotateTween.Pause();
		}
		else
		{
			RotateTween.Play();
		}
	}

	public void SetWeapon(WeaponData weaponType, int skinID, int fireStat, byte[] stickers)
	{
		PlayerAnimator.SetWeapon(weaponType.Animation);
		if (SelectWeapon != null && SelectWeapon.name == (string)weaponType.Name)
		{
			return;
		}
		if (SelectWeapon != null)
		{
			SelectWeapon.Deactive();
		}
		TPWeaponShooter tPWeaponShooter = ContainsWeapon(weaponType.Name);
		if (tPWeaponShooter == null)
		{
			GameObject gameObject = Utils.AddChild(weaponType.TpsPrefab, PlayerWeaponRoot, weaponType.TpsPrefab.transform.position, weaponType.TpsPrefab.transform.rotation);
			SelectWeapon = gameObject.GetComponent<TPWeaponShooter>();
			SelectWeapon.name = weaponType.Name;
			WeaponsList.Add(SelectWeapon);
			SelectWeapon.Init(weaponType.ID, skinID, this);
			if (fireStat > 0)
			{
				SelectWeapon.SetFireStat(fireStat);
			}
			SelectWeapon.SetStickers(stickers);
		}
		else
		{
			SelectWeapon = tPWeaponShooter;
			SelectWeapon.Data.skin = skinID;
			SelectWeapon.UpdateSkin();
			if (fireStat > 0)
			{
				SelectWeapon.UpdateFireStat(fireStat);
			}
			SelectWeapon.SetStickers(stickers);
		}
		SelectWeapon.Active();
		Sounds.Stop();
	}

	private TPWeaponShooter ContainsWeapon(string weaponName)
	{
		for (int i = 0; i < WeaponsList.Count; i++)
		{
			if (weaponName == WeaponsList[i].name)
			{
				return WeaponsList[i];
			}
		}
		return null;
	}

	public void Fire(DecalInfo decalInfo)
	{
		nProfiler.BeginSample("PlayerSkin.Fire");
		if (SelectWeapon != null)
		{
			SelectWeapon.Fire(visible, decalInfo);
		}
		DecalsManager.FireWeapon(decalInfo);
		nProfiler.EndSample();
	}

	public void Reload()
	{
		if (SelectWeapon != null)
		{
			PlayerAnimator.reload = true;
			SelectWeapon.Reload();
		}
	}

	public void Damage(DamageInfo damageInfo)
	{
		if (!Dead && (PlayerTeam != damageInfo.team || GameManager.friendDamage) && !StartDamage)
		{
			if (ShowDamage)
			{
				UIToast.Show(Localization.Get("Damage") + ": " + damageInfo.damage, 2f);
			}
			UICrosshair.Hit();
			Controller.Damage(damageInfo);
		}
	}

	public void ActiveRagdoll(Vector3 force, bool head)
	{
		if (CameraManager.type == CameraType.FirstPerson && UISpectator.GetActive() && CameraManager.selectPlayer == Controller.photonView.ownerId)
		{
			CameraManager.SetType(CameraType.Spectate, Controller.photonView.ownerId);
			visible = true;
		}
		if (visible)
		{
			if (force.magnitude < 0.5f)
			{
				force = new Vector3(0f, 0f, Random.Range(-1, 1));
			}
			Sounds.Stop();
			if (!activeRagdoll)
			{
				if (!Timer.Contains("DeactiveRagdoll"))
				{
					Timer.Create("DeactiveRagdoll", 2f, DeactiveRagdoll);
				}
				Timer.In("DeactiveRagdoll", 2f);
			}
			activeRagdoll = true;
			if (SelectWeapon != null && !DropWeaponManager.enable)
			{
				SelectWeapon.SetParent(PlayerRagdoll.playerRightWeaponRoot, PlayerRagdoll.playerLeftWeaponRoot);
			}
			PlayerRagdoll.Active(force, PlayerLimbs);
			PlayerRoot.SetActive(false);
		}
		else
		{
			if (activeRagdoll)
			{
				DeactiveRagdoll();
			}
			PlayerRoot.SetActive(false);
		}
	}

	public void DeactiveRagdoll()
	{
		if (SelectWeapon != null && !DropWeaponManager.enable)
		{
			SelectWeapon.SetParent(PlayerWeaponRoot, PlayerTwoWeaponRoot);
		}
		activeRagdoll = false;
		PlayerRagdoll.Deactive();
	}

	private void UpdateFoosteps()
	{
		Foostep = true;
		cachedAudioSource.pitch = Random.Range(1f, 1.5f);
		cachedAudioSource.clip = PlayerFoosteps[Random.Range(0, PlayerFoosteps.Length)];
		cachedAudioSource.Play();
		if (!Timer.Contains("UpdateFoosteps"))
		{
			Timer.Create("UpdateFoosteps", 0.3f, delegate
			{
				Foostep = false;
			});
		}
		Timer.In("UpdateFoosteps", 0.3f);
	}

	public void SetPosition(Vector3 pos)
	{
		PhotonPosition = pos;
		PlayerTransform.localPosition = PhotonPosition;
	}

	public void SetRotation(Vector3 rot)
	{
		PhotonRotation = Quaternion.Euler(rot);
		PlayerTransform.localRotation = PhotonRotation;
	}

	public void SetSkin(int head, int body, int legs)
	{
		HeadSkin = head;
		BodySkin = body;
		LegsSkin = legs;
	}

	public void UpdateSkin()
	{
		UIAtlas atlas = ((PlayerTeam != Team.Blue) ? GameSettings.instance.PlayerAtlasRed : GameSettings.instance.PlayerAtlasBlue);
		if (HeadSkin == 99)
		{
			atlas = GameSettings.instance.PlayerAtlasRed;
		}
		Timer.In(0.01f, delegate
		{
			if (PlayerAtlas != null)
			{
				PlayerAtlas.atlas = atlas;
				PlayerAtlas.SetSprite("0-" + HeadSkin, "1-" + BodySkin, "2-" + LegsSkin);
			}
		});
		Timer.In(Random.Range(0.1f, 0.12f), delegate
		{
			PlayerRagdoll.SetSkin(atlas, HeadSkin.ToString(), BodySkin.ToString(), LegsSkin.ToString());
		});
	}

	public void StartDamageTime()
	{
		if (!GameManager.startDamage)
		{
			return;
		}
		StartDamage = true;
		if (!Timer.Contains("StartDamageTime"))
		{
			Timer.Create("StartDamageTime", GameManager.startDamageTime, delegate
			{
				StartDamage = false;
			});
		}
		Timer.In("StartDamageTime", GameManager.startDamageTime);
	}
}
