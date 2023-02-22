using System;
using System.Collections;
using System.IO;
using DG.Tweening;
using UnityEngine;

public class FPWeaponShooter : MonoBehaviour
{
	[Serializable]
	public class DataClass
	{
		public int weapon;

		public int skin;

		public int[] stickers;

		public int firestat;

		public Team team;

		public int handSkin;
	}

	[Serializable]
	public class MotionClass
	{
		public CryptoVector3 RecoilPosition = new Vector3(0f, 0f, -0.035f);

		public CryptoVector3 RecoilRotation = new Vector3(-10f, 0f, 0f);
	}

	[Serializable]
	public class FireReloadSettings
	{
		public CryptoBool Enabled;

		public CryptoFloat Delay = 0.2f;

		public CryptoFloat Duration = 0.5f;

		public CryptoInt Force = 60;

		public CryptoVector3 Position;

		public CryptoVector3 Rotation;
	}

	[Serializable]
	public class KnifeSettings
	{
		public CryptoBool Enabled;

		public CryptoFloat Delay = 0.1f;

		public CryptoInt DelayForce = 50;

		public CryptoVector3 DelayPosition;

		public CryptoVector3 DelayRotation;

		public CryptoFloat AttackDuration = 0.2f;

		public CryptoInt AttackForce = 50;

		public CryptoVector3 AttackPosition;

		public CryptoVector3 AttackRotation;
	}

	[Serializable]
	public class FireStatSettings
	{
		[Disabled]
		public bool Enabled;

		public GameObject Target;

		public MeshAtlas[] Counters;
	}

	[Serializable]
	public class InspectWeaponSettings
	{
		[Serializable]
		public class InspectWeaponList
		{
			public float Duration = 0.5f;

			public Vector3 Position;

			public Vector3 Rotation;
		}

		[Disabled]
		public bool Active;

		public Transform HidePrefab;

		public InspectWeaponList[] List;

		[Disabled]
		public float DefaultPosition;

		public Tweener Tween;
	}

	[Serializable]
	public class ShellSettings
	{
		public bool Enable;

		public bool DoubleWeapon;

		public ParticleSystem[] Prefabs;
	}

	[Serializable]
	public class DoubleWeaponClass
	{
		public CryptoBool Enabled;

		public FPWeaponTwoHanded RightWeapon;

		public FPWeaponTwoHanded LeftWeapon;

		public MuzzleSettings RightMuzzle;

		public MuzzleSettings LeftMuzzle;

		[Disabled]
		public bool Toogle;
	}

	[Serializable]
	public class ReloadWeaponClass
	{
		public CryptoVector3 ReloadPosition;

		public CryptoVector3 ReloadRotation;
	}

	[Serializable]
	public class BulletPrefab
	{
		public bool Enabled;

		public string Prefab;

		public Vector3 Position;

		public float Force = 150f;
	}

	[Serializable]
	public class MuzzleSettings
	{
		public bool enabled = true;

		public Transform transform;

		public MeshRenderer meshRenderer;
	}

	[Header("Data")]
	public DataClass Data;

	[Header("Motion Settings")]
	public MotionClass Motion;

	[Header("Fire Reload Settings")]
	public FireReloadSettings FireReload;

	[Header("Reload Settings")]
	public ReloadWeaponClass ReloadWeapon;

	[Header("Knife Settings")]
	public KnifeSettings Knife;

	[Header("Double Weapon Settings")]
	public DoubleWeaponClass DoubleWeapon;

	[Header("Inspect Weapon Settings")]
	public InspectWeaponSettings InspectWeapon;

	[Header("FireStat Settings")]
	public FireStatSettings FireStat;

	[Header("Stickers Settings")]
	public MeshAtlas[] Stickers;

	[Header("Shell Settings")]
	public ShellSettings Shell;

	[Header("Bullet Settings")]
	public BulletPrefab Bullet;

	[Header("Muzzle Settings")]
	public MuzzleSettings Muzzle;

	[Header("Others")]
	public vp_FPWeapon FPWeapon;

	public CryptoBool SparkEffect = true;

	public MeshAtlas[] WeaponAtlas;

	public MeshAtlas[] HandsAtlas;

	public nTimer Timer;

	private TimerData ReloadInvokeData;

	private TimerData InspectWeaponInvokeData;

	private TimerData MuzzleInvokeData;

	private TimerData FireReloadInvokeData;

	private TimerData FireReloadInvokeData2;

	private void OnDisable()
	{
		StopInspectWeapon();
	}

	public void Active()
	{
		if (!Knife.Enabled && (bool)SparkEffect)
		{
			SparkEffectManager.SetParent(Muzzle.transform.parent, Muzzle.transform.localPosition);
		}
		FPWeapon.Activate();
		FPWeapon.Wield();
		if ((bool)DoubleWeapon.Enabled)
		{
			DoubleWeapon.RightWeapon.Wield();
			DoubleWeapon.LeftWeapon.Wield();
		}
		if (!Knife.Enabled)
		{
			if ((bool)DoubleWeapon.Enabled)
			{
				DoubleWeapon.LeftMuzzle.meshRenderer.enabled = false;
				DoubleWeapon.RightMuzzle.meshRenderer.enabled = false;
			}
			else if (Muzzle.enabled)
			{
				Muzzle.meshRenderer.enabled = false;
			}
		}
	}

	public void Deactive()
	{
		if (!Knife.Enabled && (bool)SparkEffect)
		{
			SparkEffectManager.ClearParent();
		}
		FPWeapon.Deactivate();
	}

	public void Reload()
	{
		Reload(WeaponManager.GetWeaponData(Data.weapon).ReloadTime);
	}

	public void Reload(float delay)
	{
		if ((bool)Knife.Enabled)
		{
			return;
		}
		FPWeapon.PositionOffset = ReloadWeapon.ReloadPosition;
		FPWeapon.RotationOffset = ReloadWeapon.ReloadRotation;
		FPWeapon.Refresh();
		if (!Timer.Contains("Reload"))
		{
			Timer.Create("Reload", delay, delegate
			{
				FPWeapon.PositionOffset = FPWeapon.DefaultPosition;
				FPWeapon.RotationOffset = FPWeapon.DefaultRotation;
				FPWeapon.Refresh();
			});
		}
		Timer.In("Reload", delay);
	}

	public void StartInspectWeapon()
	{
		if (!InspectWeapon.Active && InspectWeapon.List.Length != nValue.int0)
		{
			InspectWeapon.Active = true;
			StartCoroutine(InspectWeaponCoroutine());
		}
	}

	private IEnumerator InspectWeaponCoroutine()
	{
		if (InspectWeapon.HidePrefab != null)
		{
			if (InspectWeapon.DefaultPosition == (float)nValue.int0)
			{
				InspectWeapon.DefaultPosition = InspectWeapon.HidePrefab.localPosition.y;
			}
			InspectWeapon.Tween = InspectWeapon.HidePrefab.DOLocalMoveY(-nValue.int1, nValue.float05);
			if (!Timer.Contains("InspectWeapon"))
			{
				Timer.Create("InspectWeapon", nValue.float05, delegate
				{
					if (InspectWeapon.Active)
					{
						InspectWeapon.HidePrefab.gameObject.SetActive(false);
					}
				});
			}
			Timer.In("InspectWeapon");
		}
		for (int i = 0; i < InspectWeapon.List.Length; i++)
		{
			if (InspectWeapon.Active)
			{
				FPWeapon.StopSprings();
				if ((bool)DoubleWeapon.Enabled)
				{
					DoubleWeapon.RightWeapon.StopSprings();
					DoubleWeapon.LeftWeapon.StopSprings();
				}
				FPWeapon.AddSoftForce(InspectWeapon.List[i].Position, InspectWeapon.List[i].Rotation, (int)(InspectWeapon.List[i].Duration * (float)nValue.int60));
				yield return new WaitForSeconds(InspectWeapon.List[i].Duration);
			}
		}
		if (InspectWeapon.HidePrefab != null)
		{
			if (!InspectWeapon.Active)
			{
				yield break;
			}
			yield return new WaitForSeconds(InspectWeapon.List[InspectWeapon.List.Length - nValue.int1].Duration * nValue.float015 + nValue.float02);
			if (InspectWeapon.Active)
			{
				InspectWeapon.HidePrefab.gameObject.SetActive(true);
				InspectWeapon.Tween = InspectWeapon.HidePrefab.DOLocalMoveY(InspectWeapon.DefaultPosition, nValue.float02);
				yield return new WaitForSeconds(nValue.float02);
				if (InspectWeapon.Active)
				{
					InspectWeapon.Active = false;
				}
			}
		}
		else
		{
			InspectWeapon.Active = false;
		}
	}

	public void StopInspectWeapon()
	{
		if (!InspectWeapon.Active)
		{
			return;
		}
		InspectWeapon.Active = false;
		if (InspectWeapon.HidePrefab != null)
		{
			InspectWeapon.HidePrefab.gameObject.SetActive(true);
			if (InspectWeapon.Tween != null && InspectWeapon.Tween.IsActive())
			{
				InspectWeapon.Tween.Kill();
			}
			InspectWeapon.HidePrefab.localPosition = new Vector3(InspectWeapon.HidePrefab.localPosition.x, InspectWeapon.DefaultPosition, InspectWeapon.HidePrefab.localPosition.z);
		}
		FPWeapon.StopSprings();
	}

	public void Fire()
	{
		nProfiler.BeginSample("FPWeaponShooter.Fire");
		if (InspectWeapon.Active)
		{
			InspectWeapon.Active = false;
			FPWeapon.StopSprings();
			if ((bool)DoubleWeapon.Enabled)
			{
				DoubleWeapon.RightWeapon.StopSprings();
				DoubleWeapon.LeftWeapon.StopSprings();
			}
			if (InspectWeapon.HidePrefab != null)
			{
				InspectWeapon.HidePrefab.gameObject.SetActive(true);
				if (InspectWeapon.Tween != null && InspectWeapon.Tween.IsActive())
				{
					InspectWeapon.Tween.Kill();
				}
				InspectWeapon.HidePrefab.localPosition = new Vector3(InspectWeapon.HidePrefab.localPosition.x, InspectWeapon.DefaultPosition, InspectWeapon.HidePrefab.localPosition.z);
			}
		}
		if ((bool)Knife.Enabled)
		{
			FireKnife();
		}
		else
		{
			FireWeapon();
		}
		nProfiler.EndSample();
	}

	private void FireWeapon()
	{
		nProfiler.BeginSample("FPWeaponShooter.FireWeapon");
		if (Muzzle.enabled && UnityEngine.Random.value > nValue.float02)
		{
			MuzzleSettings muzzleSettings = ((!DoubleWeapon.Enabled) ? Muzzle : ((!DoubleWeapon.Toogle) ? DoubleWeapon.RightMuzzle : DoubleWeapon.LeftMuzzle));
			muzzleSettings.transform.localEulerAngles = new Vector3(muzzleSettings.transform.localEulerAngles.x, muzzleSettings.transform.localEulerAngles.y, UnityEngine.Random.value * (float)nValue.int360);
			muzzleSettings.meshRenderer.enabled = true;
			if (!Timer.Contains("Muzzle"))
			{
				Timer.Create("Muzzle", nValue.float002, delegate
				{
					if ((bool)DoubleWeapon.Enabled)
					{
						if (!DoubleWeapon.Toogle)
						{
							DoubleWeapon.LeftMuzzle.meshRenderer.enabled = false;
						}
						else
						{
							DoubleWeapon.RightMuzzle.meshRenderer.enabled = false;
						}
					}
					else
					{
						Muzzle.meshRenderer.enabled = false;
					}
				});
			}
			Timer.In("Muzzle");
		}
		if (Shell.Enable && Settings.Shell)
		{
			if (Shell.DoubleWeapon)
			{
				Shell.Prefabs[DoubleWeapon.Toogle ? 1 : 0].Emit(nValue.int1);
			}
			else
			{
				Shell.Prefabs[0].Emit(nValue.int1);
			}
		}
		FPWeapon.ResetSprings(nValue.float05, nValue.float05, nValue.int1, nValue.int1);
		if (Motion.RecoilRotation.z == (float)nValue.int0)
		{
			if ((bool)DoubleWeapon.Enabled)
			{
				FPWeaponTwoHanded fPWeaponTwoHanded = ((!DoubleWeapon.Toogle) ? DoubleWeapon.RightWeapon : DoubleWeapon.LeftWeapon);
				fPWeaponTwoHanded.AddForce2(Motion.RecoilPosition, Motion.RecoilRotation);
				DoubleWeapon.Toogle = !DoubleWeapon.Toogle;
			}
			else
			{
				FPWeapon.AddForce2(Motion.RecoilPosition, Motion.RecoilRotation);
			}
		}
		else if ((bool)DoubleWeapon.Enabled)
		{
			FPWeaponTwoHanded fPWeaponTwoHanded2 = ((!DoubleWeapon.Toogle) ? DoubleWeapon.RightWeapon : DoubleWeapon.LeftWeapon);
			fPWeaponTwoHanded2.AddForce2(Motion.RecoilPosition, Vector3.Scale(Motion.RecoilRotation, Vector3.one + Vector3.back) + ((!(UnityEngine.Random.value < 0.5f)) ? Vector3.back : Vector3.forward) * UnityEngine.Random.Range(Motion.RecoilRotation.z * 0.5f, Motion.RecoilRotation.z));
			DoubleWeapon.Toogle = !DoubleWeapon.Toogle;
		}
		else
		{
			FPWeapon.AddForce2(Motion.RecoilPosition, Vector3.Scale(Motion.RecoilRotation, Vector3.one + Vector3.back) + ((!(UnityEngine.Random.value < 0.5f)) ? Vector3.back : Vector3.forward) * UnityEngine.Random.Range(Motion.RecoilRotation.z * 0.5f, Motion.RecoilRotation.z));
		}
		if ((bool)FireReload.Enabled)
		{
			if (!Timer.Contains("FireReload"))
			{
				Timer.Create("FireReload", FireReload.Delay, delegate
				{
					FPWeapon.AddSoftForce(FireReload.Position, FireReload.Rotation, FireReload.Force);
					if (!Timer.Contains("FireReload2"))
					{
						Timer.Create("FireReload2", FireReload.Duration, delegate
						{
							FPWeapon.StopSprings();
							if ((bool)DoubleWeapon.Enabled)
							{
								DoubleWeapon.RightWeapon.StopSprings();
								DoubleWeapon.LeftWeapon.StopSprings();
							}
						});
					}
					Timer.In("FireReload2");
				});
			}
			Timer.In("FireReload");
		}
		nProfiler.EndSample();
	}

	private void FireKnife()
	{
		if ((float)Knife.Delay != (float)nValue.int0)
		{
			if ((bool)DoubleWeapon.Enabled)
			{
				FPWeaponTwoHanded fPWeaponTwoHanded = ((!DoubleWeapon.Toogle) ? DoubleWeapon.RightWeapon : DoubleWeapon.LeftWeapon);
				fPWeaponTwoHanded.AddSoftForceKnifeDelay();
			}
			else
			{
				FPWeapon.AddSoftForce(Knife.DelayPosition, Knife.DelayRotation, Knife.DelayForce);
			}
		}
		if (!Timer.Contains("FireKnife"))
		{
			Timer.Create("FireKnife", Knife.Delay, delegate
			{
				if ((bool)DoubleWeapon.Enabled)
				{
					FPWeaponTwoHanded fPWeaponTwoHanded2 = ((!DoubleWeapon.Toogle) ? DoubleWeapon.RightWeapon : DoubleWeapon.LeftWeapon);
					fPWeaponTwoHanded2.StopSprings();
					fPWeaponTwoHanded2.AddSoftForceKnifeAttack();
					if (DoubleWeapon.Toogle)
					{
						if (!Timer.Contains("FireKnifeLeft"))
						{
							Timer.Create("FireKnifeLeft", Knife.AttackDuration, delegate
							{
								DoubleWeapon.LeftWeapon.StopSprings();
							});
						}
						Timer.In("FireKnifeLeft");
					}
					else
					{
						if (!Timer.Contains("FireKnifeRight"))
						{
							Timer.Create("FireKnifeRight", Knife.AttackDuration, delegate
							{
								DoubleWeapon.RightWeapon.StopSprings();
							});
						}
						Timer.In("FireKnifeRight");
					}
					DoubleWeapon.Toogle = !DoubleWeapon.Toogle;
				}
				else
				{
					FPWeapon.StopSprings();
					FPWeapon.AddSoftForce(Knife.AttackPosition, Knife.AttackRotation, Knife.AttackForce);
					if (!Timer.Contains("FireKnife2"))
					{
						Timer.Create("FireKnife2", Knife.AttackDuration, delegate
						{
							FPWeapon.StopSprings();
						});
					}
					Timer.In("FireKnife2");
					if (Bullet.Enabled)
					{
						GameObject gameObject = PhotonNetwork.Instantiate(Bullet.Prefab, PlayerInput.instance.FPCamera.Transform.position + Bullet.Position, Quaternion.identity, 0);
						Rigidbody component = gameObject.GetComponent<Rigidbody>();
						component.AddForce(PlayerInput.instance.FPCamera.Transform.forward * Bullet.Force);
					}
				}
			});
		}
		Timer.In("FireKnife");
	}

	public void DryFire()
	{
		if (!Knife.Enabled)
		{
			if ((bool)DoubleWeapon.Enabled)
			{
				FPWeaponTwoHanded fPWeaponTwoHanded = ((!DoubleWeapon.Toogle) ? DoubleWeapon.RightWeapon : DoubleWeapon.LeftWeapon);
				fPWeaponTwoHanded.AddForce2(Motion.RecoilPosition * (0f - nValue.float01), Motion.RecoilRotation * (0f - nValue.float01));
				DoubleWeapon.Toogle = !DoubleWeapon.Toogle;
			}
			else
			{
				FPWeapon.AddForce2(Motion.RecoilPosition * (0f - nValue.float01), Motion.RecoilRotation * (0f - nValue.float01));
			}
		}
	}

	public void ScopeRifle()
	{
		if (InspectWeapon.Active)
		{
			FPWeapon.StopSprings();
			StopInspectWeapon();
		}
	}

	public void UpdateWeaponData(PlayerWeapons.PlayerWeaponData weaponData)
	{
		int[] array = new int[weaponData.Stickers.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = weaponData.Stickers[i];
		}
		UpdateWeaponData(weaponData.ID, weaponData.Skin, array, weaponData.FireStat);
	}

	public void UpdateWeaponData(int weapon, int skin, int[] stickers, int firestat)
	{
		UpdateHandAtlas();
		UpdateWeaponAtlas(weapon, skin);
		UpdateStickers(stickers);
		if (!Knife.Enabled && firestat >= 0)
		{
			FireStat.Enabled = true;
			FireStat.Target.SetActive(true);
			UpdateFireStat(firestat);
		}
		else if (FireStat.Enabled)
		{
			FireStat.Enabled = false;
			FireStat.Target.SetActive(false);
		}
	}

	public void UpdateHandAtlas()
	{
		UpdateHandAtlas(PhotonNetwork.player.GetTeam(), AccountManager.GetPlayerSkinSelected(BodyParts.Body));
	}

	public void UpdateHandAtlas(Team team, int id)
	{
		Data.team = team;
		Data.handSkin = id;
		UIAtlas atlas = ((team != Team.Blue) ? GameSettings.instance.PlayerAtlasRed : GameSettings.instance.PlayerAtlasBlue);
		string spriteName = "1-" + id;
		for (int i = 0; i < HandsAtlas.Length; i++)
		{
			HandsAtlas[i].atlas = atlas;
			HandsAtlas[i].spriteName = spriteName;
		}
	}

	public void UpdateWeaponAtlas(int weaponID, int weaponSkin)
	{
		Data.weapon = weaponID;
		Data.skin = weaponSkin;
		for (int i = 0; i < WeaponAtlas.Length; i++)
		{
			WeaponAtlas[i].spriteName = weaponID + "-" + weaponSkin;
		}
	}

	public void UpdateFireStat(int counter)
	{
		if (FireStat.Enabled)
		{
			Data.firestat = counter;
			counter = Mathf.Min(counter, 999999);
			string text = counter.ToString("D6");
			for (int i = 0; i < text.Length; i++)
			{
				FireStat.Counters[i].spriteName = "f" + text[i];
			}
		}
	}

	private void UpdateStickers(int[] stickers)
	{
		for (int i = 0; i < Stickers.Length; i++)
		{
			Stickers[i].cachedGameObject.SetActive(false);
		}
		for (int j = 0; j < stickers.Length; j++)
		{
			if (stickers[j] != -1)
			{
				Stickers[j].cachedGameObject.SetActive(true);
				Stickers[j].spriteName = stickers[j].ToString();
			}
		}
	}

	private void UpdateCustomSkin(object value)
	{
		if (!PhotonNetwork.offlineMode)
		{
			return;
		}
		string text = string.Empty;
		if (Application.isEditor)
		{
			text = Directory.GetParent(Application.dataPath).FullName + "/Others/Custom Weapon Skins";
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			string text2 = new AndroidJavaClass("android.os.Environment").CallStatic<AndroidJavaObject>("getExternalStorageDirectory", new object[0]).Call<string>("getAbsolutePath", new object[0]);
			text2 += "/Android/data/";
			text2 += new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<string>("getPackageName", new object[0]);
			if (Directory.Exists(text2))
			{
				if (Directory.Exists(text2 + "/files/Custom Weapon Skins"))
				{
					Directory.CreateDirectory(text2 + "/files/Custom Weapon Skins");
				}
				text = text2 + "/files/Custom Weapon Skins";
			}
			else
			{
				text = Application.dataPath;
			}
		}
		string path = text + "/" + (string)value;
		Texture2D texture2D = null;
		if (File.Exists(path))
		{
			byte[] data = File.ReadAllBytes(path);
			texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(data);
			texture2D.filterMode = FilterMode.Point;
			if (texture2D.width > 128 || texture2D.height > 128)
			{
				MonoBehaviour.print("Maximum texture size 128x128");
				return;
			}
			Material material = new Material(Shader.Find("MADFINGER/Diffuse/Simple"));
			material.mainTexture = texture2D;
			for (int i = 0; i < WeaponAtlas.Length; i++)
			{
				WeaponAtlas[i].meshFilter.sharedMesh = WeaponAtlas[i].originalMesh;
				WeaponAtlas[i].meshRenderer.material = material;
				UnityEngine.Object.Destroy(WeaponAtlas[i]);
			}
			WeaponAtlas = new MeshAtlas[0];
		}
		else
		{
			MonoBehaviour.print("No found file");
		}
	}
}
