using UnityEngine;

public class mVersionManager : MonoBehaviour
{
	public UILabel VersionLabel;

	private static mVersionManager instance;

	private void Start()
	{
		instance = this;
		VersionLabel.text = VersionManager.bundleVersion;
		UpdateRegion();
	}

	private void OnLocalize()
	{
		UpdateRegion();
	}

	public static void UpdateRegion()
	{
		string region = mPhotonSettings.region;
		string bundleVersion = VersionManager.bundleVersion;
		switch (region)
		{
		case "ru":
		{
			string text = bundleVersion;
			bundleVersion = text + " | " + Localization.Get("Region") + ": " + Localization.Get("Russia");
			break;
		}
		case "eu":
		{
			string text = bundleVersion;
			bundleVersion = text + " | " + Localization.Get("Region") + ": " + Localization.Get("Europe");
			break;
		}
		case "us":
		{
			string text = bundleVersion;
			bundleVersion = text + " | " + Localization.Get("Region") + ": " + Localization.Get("USA");
			break;
		}
		case "kr":
		{
			string text = bundleVersion;
			bundleVersion = text + " | " + Localization.Get("Region") + ": " + Localization.Get("South Korea");
			break;
		}
		case "sa":
		{
			string text = bundleVersion;
			bundleVersion = text + " | " + Localization.Get("Region") + ": " + Localization.Get("Brazil");
			break;
		}
		case "jp":
		{
			string text = bundleVersion;
			bundleVersion = text + " | " + Localization.Get("Region") + ": " + Localization.Get("Japan");
			break;
		}
		case "in":
		{
			string text = bundleVersion;
			bundleVersion = text + " | " + Localization.Get("Region") + ": " + Localization.Get("India");
			break;
		}
		case "au":
		{
			string text = bundleVersion;
			bundleVersion = text + " | " + Localization.Get("Region") + ": " + Localization.Get("Australia");
			break;
		}
		case "asia":
		{
			string text = bundleVersion;
			bundleVersion = text + " | " + Localization.Get("Region") + ": " + Localization.Get("Asia");
			break;
		}
		default:
		{
			string text = bundleVersion;
			bundleVersion = text + " | " + Localization.Get("Region") + ": " + Localization.Get("Optimal");
			break;
		}
		}
		instance.VersionLabel.text = bundleVersion;
	}
}
