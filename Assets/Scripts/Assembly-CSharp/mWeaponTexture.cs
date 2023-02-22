using UnityEngine;

public class mWeaponTexture : MonoBehaviour
{
	[SelectedWeapon]
	public int Weapon;

	public float Size = 1f;

	public bool OnlyStart;

	public UILabel SkinNameLabel;

	private UISprite WeaponIcon;

	private void Start()
	{
		SetTexture();
	}

	private void OnEnable()
	{
		if (!(WeaponIcon == null) && !OnlyStart)
		{
			SetTexture();
		}
	}

	[ContextMenu("Set Texture")]
	private void SetTexture()
	{
		if (WeaponIcon == null)
		{
			WeaponIcon = GetComponent<UISprite>();
		}
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((int)GameSettings.instance.Weapons[i].ID == Weapon)
			{
				WeaponSkinData randomWeaponSkin = WeaponManager.GetRandomWeaponSkin(GameSettings.instance.Weapons[i].ID);
				WeaponIcon.spriteName = Weapon + "-" + randomWeaponSkin.ID;
				WeaponIcon.width = (int)(GameSettings.instance.WeaponsCaseSize[i].x * Size);
				WeaponIcon.height = (int)(GameSettings.instance.WeaponsCaseSize[i].y * Size);
				if (SkinNameLabel != null)
				{
					SetSkinName(randomWeaponSkin);
				}
				break;
			}
		}
	}

	private void SetSkinName(WeaponSkinData skinData)
	{
		switch (skinData.Quality)
		{
		case WeaponSkinQuality.Normal:
			SkinNameLabel.text = skinData.Name;
			break;
		case WeaponSkinQuality.Basic:
			SkinNameLabel.text = "[00aff0]" + skinData.Name;
			break;
		case WeaponSkinQuality.Professional:
			SkinNameLabel.text = "[ff0000]" + skinData.Name;
			break;
		case WeaponSkinQuality.Legendary:
			SkinNameLabel.text = "[E00061]" + skinData.Name;
			break;
		}
	}
}
