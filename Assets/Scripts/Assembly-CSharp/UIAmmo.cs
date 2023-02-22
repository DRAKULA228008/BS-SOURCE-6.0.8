using UnityEngine;

public class UIAmmo : MonoBehaviour
{
	public UILabel label;

	public UISprite sprite;

	private Color normalColor = new Color(0f, 0f, 0f, 0.705f);

	private Color criticalColor = new Color(1f, 0f, 0f, 0.705f);

	private static UIAmmo instance;

	private void Start()
	{
		instance = this;
	}

	public static void SetAmmo(int ammo, int ammoMax)
	{
		SetAmmo(ammo, ammoMax, false, -1);
	}

	public static void SetAmmo(int ammo, int ammoMax, bool infinity, int warning)
	{
		nProfiler.BeginSample("SetAmmo");
		if (ammoMax == -1)
		{
			instance.label.text = " ";
			instance.sprite.cachedGameObject.SetActive(false);
		}
		else
		{
			if (!instance.sprite.cachedGameObject.activeSelf)
			{
				instance.sprite.cachedGameObject.SetActive(true);
			}
			instance.sprite.color = (((ammo > warning || ammoMax == 0) && ammoMax != 0) ? instance.normalColor : instance.criticalColor);
			if (infinity)
			{
				instance.label.text = StringCache.Get(ammo) + "/∞";
			}
			else
			{
				instance.label.text = StringCache.Get(ammo) + "/" + StringCache.Get(ammoMax);
			}
		}
		nProfiler.EndSample();
	}
}
