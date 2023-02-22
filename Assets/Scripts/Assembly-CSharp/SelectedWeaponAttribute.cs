using UnityEngine;

public class SelectedWeaponAttribute : PropertyAttribute
{
	public bool AllWeapons;

	public WeaponType weaponType;

	public int selected;

	public SelectedWeaponAttribute(WeaponType weapon)
	{
		weaponType = weapon;
	}

	public SelectedWeaponAttribute()
	{
		AllWeapons = true;
	}
}
