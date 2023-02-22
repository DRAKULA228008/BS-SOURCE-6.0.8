using System;
using UnityEngine;

public class DropWeaponStatic : MonoBehaviour
{
	[SelectedWeapon]
	public int weaponID;

	public bool useCustomData;

	public bool updatePlayerData = true;

	public WeaponCustomData customData;

	public MeshAtlas[] weaponMeshes;

	private bool isActive;

	private BoxCollider boxCollider;

	public event Action onDropWeaponEvent;

	private void Start()
	{
		EventManager.AddListener<DamageInfo>("DeadPlayer", OnDeadPlayer);
		if (useCustomData && (int)customData.Skin != -1)
		{
			for (int i = 0; i < weaponMeshes.Length; i++)
			{
				weaponMeshes[i].spriteName = weaponID + "-" + customData.Skin;
			}
		}
		boxCollider = GetComponent<BoxCollider>();
	}

	private void GetButtonDown(string name)
	{
		if (!isActive || !(name == "Use"))
		{
			return;
		}
		if (PlayerInput.instance.Dead)
		{
			Deactive();
			return;
		}
		if (updatePlayerData)
		{
			WeaponType type = WeaponManager.GetWeaponData(weaponID).Type;
			WeaponManager.SetSelectWeapon(type, weaponID);
			PlayerInput.instance.PlayerWeapon.UpdateWeapon(type, true, (!useCustomData) ? null : customData);
		}
		Deactive();
		if (this.onDropWeaponEvent != null)
		{
			this.onDropWeaponEvent();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (!(component == null) && !(boxCollider == null) && boxCollider.bounds.Intersects(component.mCharacterController.bounds))
			{
				isActive = true;
				InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
				UIControllerList.Use.cachedGameObject.SetActive(true);
				UIControllerList.UseText.text = Localization.Get("Pick up") + " " + WeaponManager.GetWeaponName(weaponID);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (isActive && other.CompareTag("Player"))
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
		Deactive();
	}

	private void Deactive()
	{
		isActive = false;
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
		UIControllerList.Use.cachedGameObject.SetActive(false);
		UIControllerList.UseText.text = string.Empty;
	}
}
