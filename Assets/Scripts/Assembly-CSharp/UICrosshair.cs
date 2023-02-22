using DG.Tweening;
using UnityEngine;

public class UICrosshair : MonoBehaviour
{
	public GameObject Crosshair;

	public UISprite LeftSprite;

	public UISprite RightSprite;

	public UISprite TopSprite;

	public UISprite BottomSprite;

	public UISprite PointSprite;

	public CryptoFloat MaxAccuracy;

	public CryptoFloat Accuracy;

	private Vector2 FireAccuracy;

	public CryptoInt AccuracyWidth = 1600;

	public CryptoInt AccuracyHeight = 960;

	private Tweener Tween;

	[Header("Hit Settings")]
	public UISprite HitLeftSprite;

	public UISprite HitRightSprite;

	public UISprite HitTopSprite;

	public UISprite HitBottomSprite;

	public float HitDuration;

	private Tweener HitTween;

	private float HitAlpha;

	private bool HitMarker = true;

	[Header("Scope")]
	public GameObject RifleScope;

	private bool isDynamic = true;

	public int Gap;

	private bool isActive;

	private static UICrosshair instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		EventManager.AddListener("OnSettings", UpdateSettings);
		UpdateSettings();
		Tween = DOTween.To(() => Accuracy, delegate(float x)
		{
			Accuracy = x;
		}, nValue.int5, nValue.float12).SetAutoKill(false).SetEase(Ease.OutQuart);
		Tween.OnUpdate(delegate
		{
			if (isDynamic)
			{
				LeftSprite.cachedTransform.localPosition = Vector3.left * ((float)Accuracy + (float)Gap);
				RightSprite.cachedTransform.localPosition = Vector3.right * ((float)Accuracy + (float)Gap);
				TopSprite.cachedTransform.localPosition = Vector3.up * ((float)Accuracy + (float)Gap);
				BottomSprite.cachedTransform.localPosition = Vector3.down * ((float)Accuracy + (float)Gap);
				LeftSprite.UpdateWidget();
				RightSprite.UpdateWidget();
				TopSprite.UpdateWidget();
				BottomSprite.UpdateWidget();
			}
		});
		HitTween = DOTween.To(() => HitAlpha, delegate(float x)
		{
			HitAlpha = x;
		}, nValue.int0, HitDuration).SetAutoKill(false);
		HitTween.OnUpdate(delegate
		{
			if (HitMarker)
			{
				HitLeftSprite.alpha = HitAlpha;
				HitRightSprite.alpha = HitAlpha;
				HitTopSprite.alpha = HitAlpha;
				HitBottomSprite.alpha = HitAlpha;
			}
		});
	}

	private void OnEnable()
	{
		isActive = true;
	}

	private void OnDisable()
	{
		isActive = false;
	}

	public static void SetAccuracy(float accuracy)
	{
		if (!(instance == null) && instance.isActive)
		{
			float num = accuracy * nValue.float15;
			if (instance.Tween != null)
			{
				instance.Tween.ChangeEndValue(num, true);
			}
			instance.Accuracy = num;
			instance.UpdateCrosshair();
			if (!instance.isDynamic)
			{
				instance.LeftSprite.cachedTransform.localPosition = Vector3.left * (num + (float)instance.Gap);
				instance.RightSprite.cachedTransform.localPosition = Vector3.right * (num + (float)instance.Gap);
				instance.TopSprite.cachedTransform.localPosition = Vector3.up * (num + (float)instance.Gap);
				instance.BottomSprite.cachedTransform.localPosition = Vector3.down * (num + (float)instance.Gap);
			}
		}
	}

	public static Vector2 Fire(float accuracy)
	{
		nProfiler.BeginSample("UICrosshair.Fire");
		if (instance == null || !instance.isActive)
		{
			return new Vector2(Random.Range(0f - accuracy, accuracy), Random.Range(0f - accuracy, accuracy));
		}
		instance.FireAccuracy = Vector3.zero;
		if (accuracy != (float)nValue.int0)
		{
			instance.FireAccuracy = new Vector2((float)instance.Accuracy / (float)(int)instance.AccuracyWidth, (float)instance.Accuracy / (float)(int)instance.AccuracyHeight);
			UICrosshair uICrosshair = instance;
			uICrosshair.Accuracy = (float)uICrosshair.Accuracy + accuracy * nValue.float15;
			instance.Accuracy = Mathf.Min(instance.Accuracy, instance.MaxAccuracy);
			instance.UpdateCrosshair();
		}
		nProfiler.EndSample();
		return instance.FireAccuracy;
	}

	public static void SetMove(float move)
	{
		if (move != (float)nValue.int0)
		{
			UICrosshair uICrosshair = instance;
			uICrosshair.Accuracy = (float)uICrosshair.Accuracy + move;
			instance.Accuracy = Mathf.Min(instance.Accuracy, instance.MaxAccuracy);
			instance.UpdateCrosshair();
		}
	}

	private void UpdateCrosshair()
	{
		nProfiler.BeginSample("UICrosshair.UpdateCrosshair");
		Tween.ChangeStartValue((float)Accuracy).Restart();
		nProfiler.EndSample();
	}

	public static void Hit()
	{
		if (instance.HitMarker)
		{
			instance.HitAlpha = nValue.int1;
			instance.HitTween.ChangeStartValue(instance.HitAlpha).Restart();
		}
	}

	public static void SetActiveScope(bool active)
	{
		instance.RifleScope.SetActive(active);
		SetActiveCrosshair(!active);
	}

	public static void SetActiveCrosshair(bool active)
	{
		try
		{
			instance.Crosshair.SetActive(active);
		}
		catch
		{
		}
	}

	private void UpdateSettings()
	{
		HitMarker = Settings.HitMarker;
		int num = Mathf.FloorToInt(nPlayerPrefs.GetFloat("CrosshairSize", 0.2f) * 40f + 4f);
		LeftSprite.width = num;
		RightSprite.width = num;
		TopSprite.height = num;
		BottomSprite.height = num;
		int num2 = Mathf.FloorToInt(nPlayerPrefs.GetFloat("CrosshairThickness", 0.1f) * 20f + 2f);
		LeftSprite.height = num2;
		RightSprite.height = num2;
		TopSprite.width = num2;
		BottomSprite.width = num2;
		PointSprite.width = num2;
		PointSprite.height = num2;
		Gap = Mathf.FloorToInt(nPlayerPrefs.GetFloat("CrosshairGap", 0f) * 20f);
		LeftSprite.cachedTransform.localPosition = Vector3.left * ((float)Accuracy + (float)Gap);
		RightSprite.cachedTransform.localPosition = Vector3.right * ((float)Accuracy + (float)Gap);
		TopSprite.cachedTransform.localPosition = Vector3.up * ((float)Accuracy + (float)Gap);
		BottomSprite.cachedTransform.localPosition = Vector3.down * ((float)Accuracy + (float)Gap);
		string[] array = nPlayerPrefs.GetString("CrosshairColor", "1|1|1|1").Split("|"[0]);
		Color color = new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
		LeftSprite.color = color;
		RightSprite.color = color;
		TopSprite.color = color;
		BottomSprite.color = color;
		PointSprite.color = color;
		float @float = nPlayerPrefs.GetFloat("CrosshairAlpha", 1f);
		LeftSprite.alpha = @float;
		RightSprite.alpha = @float;
		TopSprite.alpha = @float;
		BottomSprite.alpha = @float;
		PointSprite.alpha = @float;
		PointSprite.cachedGameObject.SetActive((nPlayerPrefs.GetInt("CrosshairPoint", 0) == 1) ? true : false);
		LeftSprite.cachedGameObject.SetActive((nPlayerPrefs.GetInt("CrosshairEnable_0", 1) == 1) ? true : false);
		RightSprite.cachedGameObject.SetActive((nPlayerPrefs.GetInt("CrosshairEnable_1", 1) == 1) ? true : false);
		TopSprite.cachedGameObject.SetActive((nPlayerPrefs.GetInt("CrosshairEnable_2", 1) == 1) ? true : false);
		BottomSprite.cachedGameObject.SetActive((nPlayerPrefs.GetInt("CrosshairEnable_3", 1) == 1) ? true : false);
		isDynamic = ((nPlayerPrefs.GetInt("CrosshairDynamics", 1) == 1) ? true : false);
	}
}
