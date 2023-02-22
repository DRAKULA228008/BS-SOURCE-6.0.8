using System;
using UnityEngine;

public class TPWeaponShooter : MonoBehaviour
{
	[Serializable]
	public class DataClass
	{
		public int weapon;

		public int skin;

		public byte[] stickers;

		public int firestat;
	}

	[Serializable]
	public class FireStatSettings
	{
		[Disabled]
		public bool enabled;

		public GameObject target;

		public MeshAtlas[] counters;

		public int value;
	}

	[Serializable]
	public class MuzzleSettings
	{
		public bool enabled = true;

		public Transform transform;

		public MeshRenderer meshRenderer;
	}

	[Serializable]
	public class DoubleWeaponClass
	{
		public bool enabled;

		public GameObject leftWeapon;

		public MuzzleSettings rightMuzzle;

		public MuzzleSettings leftMuzzle;

		public Vector3 leftWeaponPosition;

		public Vector3 leftWeaponRotation;

		[Disabled]
		public bool toogle;
	}

	[Header("Data")]
	public DataClass Data;

	[Header("Weapon Settings")]
	public MeshAtlas[] WeaponAtlas;

	[Header("Muzzle Settings")]
	public MuzzleSettings Muzzle;

	[Header("Transform Settings")]
	public Vector3 DefaultPosition;

	public Quaternion DefaultRotation;

	[Header("FireStat Settings")]
	public FireStatSettings FireStat;

	[Header("Stickers")]
	public MeshAtlas[] Stickers;

	[Header("Double Weapon Settings")]
	public DoubleWeaponClass DoubleWeapon;

	[Header("Others")]
	public bool Sound = true;

	public nTimer Timer;

	private PlayerSkin PlayerSkin;

	private WeaponSound FireSound;

	private WeaponSound ReloadSound;

	private Transform cachedTransform;

	private GameObject cachedGameObject;

	private TimerData MuzzleInvokeData;

	private TimerData RightMuzzleInvokeData;

	private TimerData LeftMuzzleInvokeData;

	public void Init(int weaponID, int weaponSkin, PlayerSkin playerSkin)
	{
		cachedGameObject = base.gameObject;
		cachedTransform = base.transform;
		cachedTransform.localPosition = DefaultPosition;
		cachedTransform.localRotation = DefaultRotation;
		PlayerSkin = playerSkin;
		Data.weapon = weaponID;
		Data.skin = weaponSkin;
		FireSound = WeaponManager.GetWeaponData(Data.weapon).FireSound;
		ReloadSound = WeaponManager.GetWeaponData(Data.weapon).ReloadSound;
		UpdateSkin();
		UpdateDoubleWeapon();
	}

	public void Active()
	{
		cachedGameObject.SetActive(true);
		if (DoubleWeapon.enabled)
		{
			DoubleWeapon.leftWeapon.SetActive(true);
		}
		if (Muzzle.enabled)
		{
			if (DoubleWeapon.enabled)
			{
				DoubleWeapon.leftMuzzle.meshRenderer.enabled = false;
				DoubleWeapon.rightMuzzle.meshRenderer.enabled = false;
			}
			else
			{
				Muzzle.meshRenderer.enabled = false;
			}
		}
	}

	public void Deactive()
	{
		cachedGameObject.SetActive(false);
		if (DoubleWeapon.enabled)
		{
			DoubleWeapon.leftWeapon.SetActive(false);
		}
	}

	public void Fire(bool isVisible, DecalInfo decalInfo)
	{
		nProfiler.BeginSample("TPWeaponShooter.Fire");
		if (isVisible)
		{
			if (DoubleWeapon.enabled)
			{
				if (DoubleWeapon.toogle)
				{
					DoubleWeapon.rightMuzzle.meshRenderer.enabled = true;
					if (!Timer.Contains("RightMuzzle"))
					{
						Timer.Create("RightMuzzle", 0.05f, delegate
						{
							DoubleWeapon.rightMuzzle.meshRenderer.enabled = false;
						});
					}
					Timer.In("RightMuzzle");
				}
				else
				{
					DoubleWeapon.leftMuzzle.meshRenderer.enabled = true;
					if (!Timer.Contains("LeftMuzzle"))
					{
						Timer.Create("LeftMuzzle", 0.05f, delegate
						{
							DoubleWeapon.leftMuzzle.meshRenderer.enabled = false;
						});
					}
					Timer.In("LeftMuzzle");
				}
			}
			else if (Muzzle.enabled)
			{
				Muzzle.meshRenderer.enabled = true;
				if (!Timer.Contains("Muzzle"))
				{
					Timer.Create("Muzzle", 0.05f, delegate
					{
						Muzzle.meshRenderer.enabled = false;
					});
				}
				Timer.In("Muzzle");
			}
		}
		if (DoubleWeapon.enabled)
		{
			DoubleWeapon.toogle = !DoubleWeapon.toogle;
		}
		if (Sound)
		{
			PlayerSkin.Sounds.Play(FireSound);
		}
		nProfiler.EndSample();
	}

	public void Reload()
	{
		if (Sound)
		{
			PlayerSkin.Sounds.Play(ReloadSound, 0.2f);
		}
	}

	public void UpdateSkin()
	{
		string text = Data.weapon + "-" + Data.skin;
		for (int i = 0; i < WeaponAtlas.Length; i++)
		{
			if (WeaponAtlas[i].spriteName != text)
			{
				MeshAtlas meshAtlas = WeaponAtlas[i];
				meshAtlas.spriteName = text;
			}
		}
	}

	public void SetFireStat(int firestat)
	{
		if (Settings.ShowFirestat && firestat >= -1)
		{
			FireStat.enabled = true;
			FireStat.target.SetActive(true);
			UpdateFireStat(firestat);
		}
	}

	public void UpdateFireStat1()
	{
		if (FireStat.enabled)
		{
			UpdateFireStat(FireStat.value + 1);
		}
	}

	public void UpdateFireStat(int counter)
	{
		if (FireStat.enabled && FireStat.value != counter)
		{
			FireStat.value = counter;
			string text = counter.ToString("D6");
			for (int i = 0; i < FireStat.counters.Length; i++)
			{
				FireStat.counters[i].spriteName = "f" + text[i];
			}
		}
	}

	public void SetParent(Transform p1, Transform p2)
	{
		if (cachedTransform.parent != p1)
		{
			cachedTransform.SetParent(p1);
			cachedTransform.localPosition = DefaultPosition;
			cachedTransform.localRotation = DefaultRotation;
		}
		if (DoubleWeapon.enabled && DoubleWeapon.leftWeapon.transform.parent != p2)
		{
			DoubleWeapon.leftWeapon.transform.SetParent(p2);
			DoubleWeapon.leftWeapon.transform.localPosition = DoubleWeapon.leftWeaponPosition;
			DoubleWeapon.leftWeapon.transform.localEulerAngles = DoubleWeapon.leftWeaponRotation;
		}
	}

	public void UpdateDoubleWeapon()
	{
		if (DoubleWeapon.enabled)
		{
			DoubleWeapon.leftWeapon.transform.SetParent(PlayerSkin.PlayerTwoWeaponRoot);
			DoubleWeapon.leftWeapon.transform.localPosition = DoubleWeapon.leftWeaponPosition;
			DoubleWeapon.leftWeapon.transform.localEulerAngles = DoubleWeapon.leftWeaponRotation;
		}
	}

	public void SetStickers(byte[] stickers)
	{
		if (!Settings.ShowStickers)
		{
			return;
		}
		if (stickers == null || stickers.Length == 0)
		{
			for (int i = 0; i < Stickers.Length; i++)
			{
				Stickers[i].cachedGameObject.SetActive(false);
			}
			return;
		}
		if (CheckStickers(Data.stickers, stickers))
		{
			for (int j = 0; j < Stickers.Length; j++)
			{
				Stickers[j].cachedGameObject.SetActive(false);
			}
		}
		int num = 0;
		for (int k = 0; k < stickers.Length / 2; k++)
		{
			Stickers[stickers[num] - 1].cachedGameObject.SetActive(true);
			Stickers[stickers[num] - 1].spriteName = stickers[num + 1].ToString();
			num += 2;
		}
		Data.stickers = stickers;
	}

	private bool CheckStickers(byte[] a1, byte[] a2)
	{
		if (a1 == null || a2 == null)
		{
			return false;
		}
		if (a1.Length != a2.Length)
		{
			return false;
		}
		for (int i = 0; i < a1.Length; i++)
		{
			if (a1[i] != a2[i])
			{
				return false;
			}
		}
		return true;
	}

	public int[] GetStickers()
	{
		int[] array = new int[Stickers.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (Stickers[i].cachedGameObject.activeSelf)
			{
				try
				{
					array[i] = int.Parse(Stickers[i].spriteName);
				}
				catch
				{
					array[i] = -1;
				}
			}
			else
			{
				array[i] = -1;
			}
		}
		return array;
	}

	public Transform GetCachedTransform()
	{
		if (cachedTransform == null)
		{
			cachedTransform = base.transform;
		}
		return cachedTransform;
	}
}
