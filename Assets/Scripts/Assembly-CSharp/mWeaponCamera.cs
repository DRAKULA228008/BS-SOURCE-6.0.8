using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class mWeaponCamera : MonoBehaviour
{
	[Serializable]
	public class WeaponData
	{
		[SelectedWeapon]
		public string Weapon;

		public GameObject Target;

		public GameObject FireStat;

		public MeshAtlas[] FireStatCounters;

		public MeshAtlas[] Stickers;
	}

	public Transform Point;

	public Transform PointAll;

	public float RotateSpeed = 200f;

	public Vector3 defaultPosition;

	public List<WeaponData> Weapons = new List<WeaponData>();

	private int SelectedWeapon = -1;

	private Camera mCamera;

	private Tweener mTween;

	private int SelectedWeaponID;

	private int SelectedWeaponSkin;

	private int PrevSticker = -1;

	private static mWeaponCamera instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		mCamera = GetComponent<Camera>();
		RotateSpeed = Mathf.Sqrt(RotateSpeed) / Mathf.Sqrt(Screen.dpi);
	}

	public static void SetCamera(bool down)
	{
		if (!down)
		{
			instance.PointAll.transform.localPosition = Vector3.zero;
		}
		else
		{
			instance.PointAll.transform.localPosition = new Vector3(0f, -0.4f, 0f);
		}
	}

	public static void Show(string weapon)
	{
		SetFieldOfView(60f);
		instance.mCamera.enabled = true;
		ResetRotateX(true);
		if (instance.SelectedWeapon != -1)
		{
			instance.Weapons[instance.SelectedWeapon].Target.SetActive(false);
			if (instance.Weapons[instance.SelectedWeapon].FireStat != null)
			{
				instance.Weapons[instance.SelectedWeapon].FireStat.SetActive(true);
			}
			instance.SelectedWeapon = -1;
		}
		if (weapon == "BS Gold")
		{
			instance.Weapons[44].Target.SetActive(true);
			instance.SelectedWeapon = 44;
			return;
		}
		if (weapon == "Sticker")
		{
			instance.Weapons[45].Target.SetActive(true);
			instance.SelectedWeapon = 45;
			return;
		}
		for (int i = 0; i < instance.Weapons.Count; i++)
		{
			if (instance.Weapons[i].Weapon == weapon)
			{
				instance.Weapons[i].Target.SetActive(true);
				instance.SelectedWeapon = i;
				break;
			}
		}
	}

	public static void Close()
	{
		instance.mCamera.enabled = false;
		if (instance.SelectedWeapon != -1)
		{
			instance.Weapons[instance.SelectedWeapon].Target.SetActive(false);
			instance.SelectedWeapon = -1;
		}
	}

	public static void SetViewportRect(Rect rect, float duration)
	{
		if (instance.mTween != null && instance.mTween.IsActive())
		{
			if (duration == 0f)
			{
				instance.mTween.Kill();
				instance.mCamera.rect = rect;
			}
			else
			{
				instance.mTween = instance.mTween.ChangeEndValue(rect, duration);
			}
		}
		else if (duration == 0f)
		{
			instance.mCamera.rect = rect;
		}
		else
		{
			instance.mTween = instance.mCamera.DORect(rect, duration);
		}
	}

	public static void SetSkin(int weaponID, int skin)
	{
		if (instance.SelectedWeapon == -1)
		{
			return;
		}
		if (instance.SelectedWeapon == 45)
		{
			instance.Weapons[instance.SelectedWeapon].Target.GetComponent<MeshAtlas>().spriteName = skin.ToString();
			return;
		}
		instance.SelectedWeaponID = weaponID;
		instance.SelectedWeaponSkin = skin;
		MeshAtlas[] componentsInChildren = instance.Weapons[instance.SelectedWeapon].Target.GetComponentsInChildren<MeshAtlas>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].name != "FireStat" && componentsInChildren[i].name != "1" && componentsInChildren[i].name != "2" && componentsInChildren[i].name != "3" && componentsInChildren[i].name != "4" && componentsInChildren[i].name != "5" && componentsInChildren[i].name != "6")
			{
				componentsInChildren[i].spriteName = weaponID + "-" + skin;
			}
		}
		if (AccountManager.GetFireStatCounter(weaponID, skin) > -1 && instance.Weapons[instance.SelectedWeapon].FireStat != null)
		{
			instance.Weapons[instance.SelectedWeapon].FireStat.SetActive(true);
			string text = AccountManager.GetFireStatCounter(weaponID, skin).ToString("D6");
			for (int j = 0; j < text.Length; j++)
			{
				instance.Weapons[instance.SelectedWeapon].FireStatCounters[j].spriteName = "f" + text[j];
			}
		}
		else if (instance.Weapons[instance.SelectedWeapon].FireStat != null)
		{
			instance.Weapons[instance.SelectedWeapon].FireStat.SetActive(false);
		}
	}

	public static void Rotate(Vector2 rotate, bool onlyY)
	{
		if (onlyY)
		{
			instance.Point.Rotate(new Vector2(0f, (0f - rotate.x) * instance.RotateSpeed), Space.Self);
		}
		else
		{
			instance.PointAll.Rotate(new Vector2(rotate.y * instance.RotateSpeed, (0f - rotate.x) * instance.RotateSpeed), Space.World);
		}
	}

	public static void SetRotation(Vector3 rotation)
	{
		instance.Point.localEulerAngles = rotation;
	}

	public static void ResetRotateX(bool isTween)
	{
		if (isTween)
		{
			instance.Point.DOLocalRotate(instance.defaultPosition, 0.5f);
			instance.PointAll.DOLocalRotate(instance.defaultPosition, 0.5f);
		}
		else
		{
			instance.Point.localEulerAngles = instance.defaultPosition;
			instance.PointAll.localEulerAngles = instance.defaultPosition;
		}
	}

	public static void SetFieldOfView(float value)
	{
		SetFieldOfView(-1f, value, -1f);
	}

	public static void SetFieldOfView(float from, float to, float duration)
	{
		if (from == -1f)
		{
			instance.mCamera.fieldOfView = to;
			return;
		}
		instance.mCamera.fieldOfView = from;
		instance.mCamera.DOFieldOfView(to, duration);
	}

	public static bool HasStickers()
	{
		return instance.Weapons[instance.SelectedWeapon].Stickers.Length != 0;
	}

	public static int GetStickersCount()
	{
		return instance.Weapons[instance.SelectedWeapon].Stickers.Length;
	}

    public static void SetStickers(AccountWeaponStickers stickers)
    {
        for (int i = 0; i < instance.Weapons[instance.SelectedWeapon].Stickers.Length; i++)
        {
            instance.Weapons[instance.SelectedWeapon].Stickers[i].gameObject.SetActive(false);
        }
        if (stickers != null)
        {
            //return;
            //}
            for (int j = 0; j < stickers.StickerData.Count; j++)
            {
                for (int k = 0; k < instance.Weapons[instance.SelectedWeapon].Stickers.Length; k++)
                {
                    if (instance.Weapons[instance.SelectedWeapon].Stickers[k].name == stickers.StickerData[j].Index.ToString())
                    {
                        instance.Weapons[instance.SelectedWeapon].Stickers[k].gameObject.SetActive(true);
                        instance.Weapons[instance.SelectedWeapon].Stickers[k].spriteName = stickers.StickerData[j].StickerID.ToString();
                        break;
                    }
                }
            }
        }
    }

	public static void ActivePrevSticker(int pos, int id)
	{
		pos--;
		MeshAtlas atlas = instance.Weapons[instance.SelectedWeapon].Stickers[pos];
		if (instance.PrevSticker != pos)
		{
			DeactivePrevSticker();
			atlas.gameObject.SetActive(true);
			instance.PrevSticker = pos;
			TimerManager.In("PrevSticker", 0.3f, -1, 0.3f, delegate
			{
				atlas.cachedGameObject.SetActive(!atlas.cachedGameObject.activeSelf);
			});
		}
		atlas.spriteName = id.ToString();
	}

	public static void DeactivePrevSticker()
	{
		if (instance.PrevSticker != -1)
		{
			TimerManager.Cancel("PrevSticker");
			if (AccountManager.HasWeaponSticker(instance.SelectedWeaponID, instance.SelectedWeaponSkin, instance.PrevSticker + 1))
			{
				instance.Weapons[instance.SelectedWeapon].Stickers[instance.PrevSticker].gameObject.SetActive(true);
				int weaponSticker = AccountManager.GetWeaponSticker(instance.SelectedWeaponID, instance.SelectedWeaponSkin, instance.PrevSticker + 1);
				instance.Weapons[instance.SelectedWeapon].Stickers[instance.PrevSticker].spriteName = weaponSticker.ToString();
			}
			else
			{
				instance.Weapons[instance.SelectedWeapon].Stickers[instance.PrevSticker].gameObject.SetActive(false);
			}
			instance.PrevSticker = -1;
		}
	}
}
