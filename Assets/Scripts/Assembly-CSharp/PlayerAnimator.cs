using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
	[Serializable]
	public class WeaponAnimData
	{
		public Key[] defaultValue;

		public Key[] maxRotateValue;

		public Key[] minRotateValue;

		public Keys[] reloadValue;

		public int reloadIndex;

		public int maxReloadIndex;

		public bool reloading;

		public bool active;

		public float lastRotate;

		private int defaultValueCount;

		private int maxRotateValueCount;

		private int minRotateValueCount;

		private int reloadValueCount;

		public void SetDefault()
		{
			if (defaultValueCount == 0)
			{
				defaultValueCount = defaultValue.Length;
			}
			for (int i = 0; i < defaultValueCount; i++)
			{
				defaultValue[i].target.localPosition = defaultValue[i].pos;
				defaultValue[i].target.localRotation = defaultValue[i].rot;
			}
		}

		public void SetRotate(float rotate, bool visible)
		{
			if (!visible || lastRotate == rotate)
			{
				return;
			}
			lastRotate = rotate;
			if (rotate >= 0f)
			{
				if (maxRotateValueCount == 0)
				{
					maxRotateValueCount = maxRotateValue.Length;
				}
				for (int i = 0; i < maxRotateValueCount; i++)
				{
					maxRotateValue[i].target.localPosition = Vector3.MoveTowards(defaultValue[i + 3].pos, maxRotateValue[i].pos, rotate);
					maxRotateValue[i].target.localRotation = Quaternion.Lerp(defaultValue[i + 3].rot, maxRotateValue[i].rot, rotate);
				}
				return;
			}
			rotate *= -1f;
			if (minRotateValueCount == 0)
			{
				minRotateValueCount = minRotateValue.Length;
			}
			for (int j = 0; j < minRotateValueCount; j++)
			{
				minRotateValue[j].target.localPosition = Vector3.MoveTowards(defaultValue[j + 3].pos, minRotateValue[j].pos, rotate);
				minRotateValue[j].target.localRotation = Quaternion.Lerp(defaultValue[j + 3].rot, minRotateValue[j].rot, rotate);
			}
		}

		public void StartReload()
		{
			reloadIndex = 0;
			reloading = true;
		}

		public void StopReload()
		{
			reloadIndex = 0;
			reloading = false;
			SetDefault();
		}

		public void UpdateReload(bool visible)
		{
			reloadIndex++;
			if (reloadIndex >= maxReloadIndex)
			{
				StopReload();
			}
			else if (visible)
			{
				if (reloadValueCount == 0)
				{
					reloadValueCount = reloadValue.Length;
				}
				for (int i = 0; i < reloadValueCount; i++)
				{
					reloadValue[i].target.localPosition = reloadValue[i].pos[reloadIndex];
					reloadValue[i].target.localRotation = reloadValue[i].rot[reloadIndex];
				}
			}
		}
	}

	[Serializable]
	public class LegsAnimData
	{
		public Key[] defaultValue;

		public Key[] groundedValue;

		public Keys[] moveValue;

		public int moveIndex;

		public bool active;

		private int defaultValueCount;

		private int groundedValueCount;

		private int moveValueCount;

		public void SetDefault()
		{
			if (defaultValueCount == 0)
			{
				defaultValueCount = defaultValue.Length;
			}
			for (int i = 0; i < defaultValueCount; i++)
			{
				if (defaultValue[i].target.localPosition != defaultValue[i].pos)
				{
					defaultValue[i].target.localPosition = defaultValue[i].pos;
				}
				if (defaultValue[i].target.localRotation != defaultValue[i].rot)
				{
					defaultValue[i].target.localRotation = defaultValue[i].rot;
				}
			}
		}

		public void SetGrounded(bool grounded)
		{
			if (!grounded)
			{
				if (groundedValueCount == 0)
				{
					groundedValueCount = groundedValue.Length;
				}
				for (int i = 0; i < groundedValueCount; i++)
				{
					if (defaultValue[i].target.localPosition != groundedValue[i].pos)
					{
						defaultValue[i].target.localPosition = groundedValue[i].pos;
					}
					if (defaultValue[i].target.localRotation != groundedValue[i].rot)
					{
						defaultValue[i].target.localRotation = groundedValue[i].rot;
					}
				}
			}
			else
			{
				SetDefault();
			}
		}

		public void UpdateMove(float speed, bool visible)
		{
			nProfiler.BeginSample("PlayerAnimator.UpdateMove");
			if (speed >= 0f)
			{
				moveIndex++;
				if (moveIndex >= moveValue[0].pos.Length)
				{
					moveIndex = 0;
				}
			}
			else
			{
				moveIndex--;
				if (moveIndex <= 0)
				{
					moveIndex = moveValue[0].pos.Length - 1;
				}
				if (speed < 0f)
				{
					speed *= -1f;
				}
			}
			if (speed != 0f && !visible)
			{
				nProfiler.EndSample();
				return;
			}
			if (!active)
			{
				nProfiler.EndSample();
				return;
			}
			if (PhotonNetwork.leavingRoom)
			{
				nProfiler.EndSample();
				return;
			}
			if (moveValueCount == 0)
			{
				moveValueCount = moveValue.Length;
			}
			for (int i = 0; i < moveValueCount; i++)
			{
				if (visible)
				{
					moveValue[i].target.localRotation = Quaternion.Lerp(defaultValue[i].rot, moveValue[i].rot[moveIndex], speed);
				}
				else
				{
					moveValue[i].target.localRotation = moveValue[i].rot[moveIndex];
				}
			}
			nProfiler.EndSample();
		}

		private Vector3 Vector3Lerp(Vector3 from, Vector3 to, float t)
		{
			from.x += (to.x - from.x) * t;
			from.y += (to.y - from.y) * t;
			from.z += (to.z - from.z) * t;
			return from;
		}
	}

	[Serializable]
	public class Key
	{
		public Transform target;

		public Vector3 pos;

		public Quaternion rot;
	}

	[Serializable]
	public class Keys
	{
		public Transform target;

		public Vector3[] pos;

		public Quaternion[] rot;
	}

	public WeaponType selectWeapon;

	public WeaponAnimData rifle;

	public WeaponAnimData pistol;

	public WeaponAnimData knife;

	public LegsAnimData legs;

	public Transform root;

	private float cachedRotate;

	private bool cachedGrounded;

	private float cachedMove;

	private bool cachedReload;

	private Vector3 rootPosition = Vector3.zero;

	private bool cacheVisible;

	public float rotate
	{
		get
		{
			return cachedRotate;
		}
		set
		{
			if (cachedRotate != value)
			{
				cachedRotate = value;
				if (!cachedReload)
				{
					GetSelectWeapon().SetRotate(value, visible);
				}
			}
		}
	}

	public bool grounded
	{
		get
		{
			return cachedGrounded;
		}
		set
		{
			if (cachedGrounded != value)
			{
				cachedGrounded = value;
				legs.SetGrounded(value);
			}
		}
	}

	public bool reload
	{
		get
		{
			return cachedReload;
		}
		set
		{
			if (selectWeapon == WeaponType.Knife)
			{
				return;
			}
			if (value)
			{
				if (cachedReload)
				{
					GetSelectWeapon().StopReload();
				}
				cachedReload = value;
				GetSelectWeapon().StartReload();
			}
			else
			{
				cachedReload = value;
				GetSelectWeapon().StopReload();
			}
		}
	}

	public float move
	{
		get
		{
			return cachedMove;
		}
		set
		{
			if (value == 0f)
			{
				if (cachedMove != 0f && cachedGrounded)
				{
					legs.UpdateMove(value, visible);
					cachedMove = value;
				}
			}
			else
			{
				cachedMove = value;
			}
		}
	}

	public bool visible
	{
		get
		{
			return cacheVisible;
		}
		set
		{
			cacheVisible = value;
			if (!cachedReload)
			{
				GetSelectWeapon().SetRotate(cachedRotate, value);
				if (grounded)
				{
					legs.UpdateMove(cachedMove, value);
				}
			}
		}
	}

	public Vector3 rootPos
	{
		get
		{
			return rootPosition;
		}
		set
		{
			rootPosition = value;
			root.localPosition = rootPosition;
		}
	}

	private void OnEnable()
	{
		rifle.active = true;
		pistol.active = true;
		knife.active = true;
		legs.active = true;
	}

	private void OnDisable()
	{
		rifle.active = false;
		pistol.active = false;
		knife.active = false;
		legs.active = false;
	}

	public void OnDefault()
	{
		selectWeapon = WeaponType.Knife;
		rifle.reloadIndex = 0;
		rifle.reloading = false;
		rifle.active = false;
		rifle.lastRotate = 0f;
		pistol.reloadIndex = 0;
		pistol.reloading = false;
		pistol.active = false;
		pistol.lastRotate = 0f;
		knife.reloadIndex = 0;
		knife.reloading = false;
		knife.active = false;
		knife.lastRotate = 0f;
		legs.moveIndex = 0;
		legs.active = false;
	}

	private void LateUpdate()
	{
		if (cachedMove != 0f && cachedGrounded)
		{
			legs.UpdateMove(cachedMove, visible);
		}
		if (cachedReload)
		{
			GetSelectWeapon().UpdateReload(visible);
			if (!GetSelectWeapon().reloading)
			{
				GetSelectWeapon().StopReload();
				GetSelectWeapon().SetRotate(cachedRotate, visible);
				cachedReload = false;
			}
		}
	}

	private WeaponAnimData GetSelectWeapon()
	{
		switch (selectWeapon)
		{
		case WeaponType.Rifle:
			return rifle;
		case WeaponType.Pistol:
			return pistol;
		case WeaponType.Knife:
			return knife;
		default:
			return rifle;
		}
	}

	public void SetWeapon(WeaponType type)
	{
		if (type != selectWeapon)
		{
			if (GetSelectWeapon().reloading)
			{
				GetSelectWeapon().StopReload();
			}
			selectWeapon = type;
			reload = false;
			switch (type)
			{
			case WeaponType.Rifle:
				rifle.SetDefault();
				break;
			case WeaponType.Pistol:
				pistol.SetDefault();
				break;
			case WeaponType.Knife:
				knife.SetDefault();
				break;
			}
		}
	}

	public void SetDefault()
	{
		root.localPosition = rootPos;
		root.localEulerAngles = Vector3.zero;
		SetWeapon(selectWeapon);
		cachedRotate = 0f;
		cachedReload = false;
		cachedMove = 0f;
		cachedGrounded = true;
		WeaponAnimData weaponAnimData = GetSelectWeapon();
		weaponAnimData.SetDefault();
		weaponAnimData.SetRotate(rotate, visible);
		legs.SetGrounded(grounded);
	}
}
