using UnityEngine;

public class UIColorCrosshair : MonoBehaviour
{
	private int SelectColor;

	private UISprite mSprite;

	private void Start()
	{
		mSprite = GetComponent<UISprite>();
		SelectColor = Settings.ColorCrosshair;
		Color color = Utils.GetColor(SelectColor);
		if (color == Color.clear)
		{
			color = new Color(1f, 1f, 1f, 0.5f);
		}
		mSprite.color = color;
	}

	private void OnClick()
	{
		SelectColor++;
		if (9 < SelectColor)
		{
			SelectColor = 0;
		}
		Color color = Utils.GetColor(SelectColor);
		if (color.a == 0f)
		{
			color = new Color(1f, 1f, 1f, 0.05f);
		}
		mSprite.color = color;
		Settings.ColorCrosshair = SelectColor;
	}
}
