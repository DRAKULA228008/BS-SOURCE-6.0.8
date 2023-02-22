using UnityEngine;

[ExecuteInEditMode]
public class SkyboxManager : MonoBehaviour
{
	[Range(0f, 1f)]
	public float TimeDay;

	public bool Moon;

	public bool Stars;

	public bool Sun;

	public bool Clouds;

	public Material SkyboxMaterial;

	public Transform SkyboxCamera;

	public Transform SkyboxCameraParent;

	public GameObject MoonObject;

	public GameObject StarsObject;

	public GameObject SunObject;

	public GameObject CloudsObject;

	private static SkyboxManager instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (Application.isPlaying)
		{
			SkyboxMaterial.mainTextureOffset = new Vector2(TimeDay, 0f);
			MoonObject.SetActive(Moon);
			StarsObject.SetActive(Stars);
			SunObject.SetActive(Sun);
			CloudsObject.SetActive(Clouds);
			EventManager.AddListener("OnSettings", OnSettings);
			OnSettings();
		}
	}

	private void Reset()
	{
		SkyboxMaterial.mainTextureOffset = new Vector2(TimeDay, 0f);
	}

	public static Transform GetCamera()
	{
		return instance.SkyboxCamera;
	}

	public static Transform GetCameraParent()
	{
		return instance.SkyboxCameraParent;
	}

	private void OnSettings()
	{
		if (Settings.Clouds)
		{
			if (Clouds)
			{
				CloudsObject.SetActive(true);
			}
		}
		else
		{
			CloudsObject.SetActive(false);
		}
	}
}
