using System;
using UnityEngine;

public class DropWeapon : MonoBehaviour
{
	private static DropWeapon selectWeapon;

	public int ID;

	[SelectedWeapon]
	public int weaponID;

	public GameObject Weapon;

	public MeshAtlas[] WeaponAtlas;

	public MeshAtlas[] Stickers;

	public GameObject FireStat;

	public MeshAtlas[] FireStatCounters;

	private bool AutoDrop;

	public bool DestroyDrop;

	public float DestroyTime = -1f;

	public bool CustomData;

	public WeaponCustomData Data = new WeaponCustomData();

	private bool isEnterTrigger;

	private int TimerID;

	private int EventID;

	private BoxCollider boxCollider;

	private void OnEnable()
	{
		if (boxCollider == null)
		{
			boxCollider = GetComponent<BoxCollider>();
		}
		EventID = EventManager.AddListener<DamageInfo>("DeadPlayer", OnDeadPlayer);
		if (DestroyTime > 0f)
		{
			TimerID = TimerManager.In(DestroyTime, DestroyWeapon);
		}
	}

	private void OnDisable()
	{
		if (!PhotonNetwork.leavingRoom)
		{
			Deactive();
			EventManager.ClearEvent(EventID);
			TimerManager.Cancel(TimerID);
		}
	}

	private void GetButtonDown(string name)
	{
		if (isEnterTrigger && !(name != "Use") && !(selectWeapon != this) && !GameManager.player.PlayerWeapon.Wielded && !GameManager.player.Dead)
		{
			DropWeaponManager.PickupWeapon(ID);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (DropWeaponManager.lastDropTime + 0.2f > Time.time || !other.CompareTag("Player"))
		{
			return;
		}
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (!(component == null) && !component.Dead && boxCollider.bounds.Intersects(component.mCharacterController.bounds))
		{
			if (AutoDrop && !component.PlayerWeapon.GetWeaponData(WeaponManager.GetWeaponData(weaponID).Type).Enabled)
			{
				DropWeaponManager.PickupWeapon(ID);
			}
			else if (!component.PlayerWeapon.GetWeaponData(WeaponManager.GetWeaponData(weaponID).Type).Enabled)
			{
				isEnterTrigger = true;
				InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
				UIControllerList.Use.cachedGameObject.SetActive(true);
				UIControllerList.UseText.text = Localization.Get("Pick up") + " " + WeaponManager.GetWeaponName(weaponID);
				selectWeapon = this;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (!(component == null))
			{
				Deactive();
			}
		}
	}

	private void OnDeadPlayer(DamageInfo info)
	{
		if (!PhotonNetwork.leavingRoom)
		{
			Deactive();
		}
	}

	private void Deactive()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
		if (isEnterTrigger && selectWeapon == this)
		{
			UIControllerList.Use.cachedGameObject.SetActive(false);
			UIControllerList.UseText.text = string.Empty;
		}
		isEnterTrigger = false;
	}

	public void PickupWeapon()
	{
		WeaponData weaponData = WeaponManager.GetWeaponData(weaponID);
		GameManager.player.PlayerWeapon.DropWeapon(weaponData.Type);
		GameManager.player.PlayerWeapon.UpdateWeapon(weaponData.ID, true, (!CustomData) ? null : Data);
		Deactive();
		if (DestroyDrop)
		{
			DestroyWeapon();
		}
	}

	public void DestroyWeapon()
	{
		PoolManager.Despawn(weaponID + "-Drop", Weapon);
	}

	public void UpdateWeapon()
	{
		if (CustomData)
		{
			string spriteName = weaponID + "-" + Data.Skin;
			for (int i = 0; i < WeaponAtlas.Length; i++)
			{
				WeaponAtlas[i].spriteName = spriteName;
			}
			if ((int)Data.FireStatCounter >= 0)
			{
				FireStat.SetActive(true);
				string text = Data.FireStatCounter.ToString("D6");
				for (int j = 0; j < text.Length; j++)
				{
					FireStatCounters[j].spriteName = "f" + text[j];
				}
			}
			else
			{
				FireStat.SetActive(false);
			}
			for (int k = 0; k < Stickers.Length; k++)
			{
				Stickers[k].cachedGameObject.SetActive(false);
			}
			for (int l = 0; l < Data.Stickers.Length; l++)
			{
				if ((int)Data.Stickers[l] != -1)
				{
					Stickers[l].cachedGameObject.SetActive(true);
					Stickers[l].spriteName = Data.Stickers[l].ToString();
				}
			}
		}
		else
		{
			string spriteName2 = weaponID + "-0";
			for (int m = 0; m < WeaponAtlas.Length; m++)
			{
				WeaponAtlas[m].spriteName = spriteName2;
			}
		}
	}
}
