using UnityEngine;

public class UICrosshairSettings : MonoBehaviour
{
	public UISprite[] Crosshair;

	public UIToggle[] CrosshairToogle;

	public UISprite Point;

	public UIToggle DynamicsToogle;

	public UIToggle PointToogle;

	public UILabel SizeLabel;

	public UILabel ThicknessLabel;

	public UILabel GapLabel;

	public UILabel AlphaLabel;

	public UISlider SizeSlider;

	public UISlider ThicknessSlider;

	public UISlider GapSlider;

	public UISlider AlphaSlider;

	public UIColorPicker ColorPicker;

	private void Awake()
	{
		DynamicsToogle.value = ((nPlayerPrefs.GetInt("CrosshairDynamics", 1) == 1) ? true : false);
		PointToogle.value = ((nPlayerPrefs.GetInt("CrosshairPoint", 0) == 1) ? true : false);
		for (int i = 0; i < CrosshairToogle.Length; i++)
		{
			CrosshairToogle[i].value = ((nPlayerPrefs.GetInt("CrosshairEnable_" + i, 1) == 1) ? true : false);
		}
		SizeSlider.value = nPlayerPrefs.GetFloat("CrosshairSize", 0.2f);
		ThicknessSlider.value = nPlayerPrefs.GetFloat("CrosshairThickness", 0.1f);
		GapSlider.value = nPlayerPrefs.GetFloat("CrosshairGap", 0f);
		AlphaSlider.value = nPlayerPrefs.GetFloat("CrosshairAlpha", 1f);
		string[] array = nPlayerPrefs.GetString("CrosshairColor", "1|1|1|1").Split("|"[0]);
		ColorPicker.value = new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
		UpdateAll();
	}

	public void SetSize()
	{
		int num = Mathf.FloorToInt(SizeSlider.value * 40f + 4f);
		SizeLabel.text = Localization.Get("Size") + ": " + num;
		Crosshair[0].width = num;
		Crosshair[1].width = num;
		Crosshair[2].height = num;
		Crosshair[3].height = num;
		nPlayerPrefs.SetFloat("CrosshairSize", SizeSlider.value);
	}

	public void SetThickness()
	{
		int num = Mathf.FloorToInt(ThicknessSlider.value * 20f + 2f);
		ThicknessLabel.text = Localization.Get("Thickness") + ": " + num;
		Crosshair[0].height = num;
		Crosshair[1].height = num;
		Crosshair[2].width = num;
		Crosshair[3].width = num;
		Point.width = num;
		Point.height = num;
		nPlayerPrefs.SetFloat("CrosshairThickness", ThicknessSlider.value);
	}

	public void SetGap()
	{
		int num = Mathf.FloorToInt(GapSlider.value * 20f) + 10;
		GapLabel.text = Localization.Get("Gap") + ": " + (num - 10);
		Crosshair[0].cachedTransform.localPosition = Vector3.left * num;
		Crosshair[1].cachedTransform.localPosition = Vector3.right * num;
		Crosshair[2].cachedTransform.localPosition = Vector3.up * num;
		Crosshair[3].cachedTransform.localPosition = Vector3.down * num;
		nPlayerPrefs.SetFloat("CrosshairGap", GapSlider.value);
	}

	public void SetAlpha()
	{
		float alpha = Mathf.Clamp(AlphaSlider.value, 0.01f, 1f);
		AlphaLabel.text = Localization.Get("Alpha") + ": " + alpha.ToString("f2");
		Crosshair[0].alpha = alpha;
		Crosshair[1].alpha = alpha;
		Crosshair[2].alpha = alpha;
		Crosshair[3].alpha = alpha;
		Point.alpha = alpha;
		nPlayerPrefs.SetFloat("CrosshairAlpha", AlphaSlider.value);
	}

	public void SetColor()
	{
		Color value = ColorPicker.value;
		Crosshair[0].color = value;
		Crosshair[1].color = value;
		Crosshair[2].color = value;
		Crosshair[3].color = value;
		Point.color = value;
		nPlayerPrefs.SetString("CrosshairColor", value.r + "|" + value.g + "|" + value.b + "|" + value.a);
	}

	public void SetPoint()
	{
		Point.cachedGameObject.SetActive(PointToogle.value);
		nPlayerPrefs.SetInt("CrosshairPoint", PointToogle.value ? 1 : 0);
	}

	public void SetDynamics()
	{
		nPlayerPrefs.SetInt("CrosshairDynamics", DynamicsToogle.value ? 1 : 0);
	}

	public void SetCrosshair()
	{
		Crosshair[0].cachedGameObject.SetActive(CrosshairToogle[0].value);
		Crosshair[1].cachedGameObject.SetActive(CrosshairToogle[1].value);
		Crosshair[2].cachedGameObject.SetActive(CrosshairToogle[2].value);
		Crosshair[3].cachedGameObject.SetActive(CrosshairToogle[3].value);
		for (int i = 0; i < CrosshairToogle.Length; i++)
		{
			nPlayerPrefs.SetInt("CrosshairEnable_" + i, CrosshairToogle[i].value ? 1 : 0);
		}
	}

	public void SetDefault()
	{
		DynamicsToogle.value = true;
		PointToogle.value = false;
		for (int i = 0; i < CrosshairToogle.Length; i++)
		{
			CrosshairToogle[i].value = true;
		}
		SizeSlider.value = 0.2f;
		ThicknessSlider.value = 0.1f;
		GapSlider.value = 0f;
		AlphaSlider.value = 1f;
		ColorPicker.Select(Color.white);
		nPlayerPrefs.SetInt("CrosshairDynamics", 1);
		nPlayerPrefs.SetInt("CrosshairPoint", 0);
		for (int j = 0; j < CrosshairToogle.Length; j++)
		{
			nPlayerPrefs.SetInt("CrosshairEnable_" + j, 1);
		}
		nPlayerPrefs.SetFloat("CrosshairSize", 0.2f);
		nPlayerPrefs.SetFloat("CrosshairThickness", 0.1f);
		nPlayerPrefs.SetFloat("CrosshairGap", 0f);
		nPlayerPrefs.SetFloat("CrosshairAlpha", 1f);
		nPlayerPrefs.SetString("CrosshairColor", "1|1|1|1");
		UpdateAll();
	}

	private void UpdateAll()
	{
		SetSize();
		SetThickness();
		SetGap();
		SetAlpha();
		SetColor();
		SetPoint();
		SetDynamics();
		SetCrosshair();
	}
}
