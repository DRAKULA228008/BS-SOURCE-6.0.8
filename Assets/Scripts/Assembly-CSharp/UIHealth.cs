using UnityEngine;

public class UIHealth : MonoBehaviour
{
	public UILabel label;

	public UISprite sprite;

	private Color normalColor = new Color(0f, 0f, 0f, 0.705f);

	private Color criticalColor = new Color(1f, 0f, 0f, 0.705f);

	private static UIHealth instance;

	private void Start()
	{
		instance = this;
	}

	public static void SetHealth(int health)
	{
		if (health == 0)
		{
			instance.label.text = " ";
			instance.sprite.cachedGameObject.SetActive(false);
			UIAmmo.SetAmmo(0, -1);
			return;
		}
		instance.sprite.cachedGameObject.SetActive(true);
		instance.label.text = "+" + StringCache.Get(health);
		if (health <= 25)
		{
			instance.sprite.color = instance.criticalColor;
		}
		else
		{
			instance.sprite.color = instance.normalColor;
		}
	}
}
