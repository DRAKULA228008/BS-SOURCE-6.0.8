using UnityEngine;

public class mCheckUpdateGame : MonoBehaviour
{
	private void Start()
	{
		EventManager.AddListener<string>("MinVersion", CheckGame);
	}

	private void Show()
	{
		AndroidNativeFunctions.ShowAlert(Localization.Get("Available new version of the game"), Localization.Get("New Version"), Localization.Get("Download"), string.Empty, string.Empty, Download);
		GameSettings.instance.PhotonID = string.Empty;
		TimerManager.In(0.1f, -1, 0.1f, delegate
		{
			AccountManager.isConnect = false;
		});
		TimerManager.In(20f, delegate
		{
			Application.Quit();
		});
	}

	private void CheckGame(string version)
	{
		if (Utils.CompareVersion(VersionManager.bundleVersion, version))
		{
			Show();
		}
	}

	private void Download(DialogInterface dialog)
	{
		AndroidNativeFunctions.OpenGooglePlay("com.rexetstudio.blockstrike");
		AndroidNativeFunctions.ShowAlert(Localization.Get("Available new version of the game"), Localization.Get("New Version"), Localization.Get("Download"), string.Empty, string.Empty, Download);
	}

	private bool CheckVersion(string data)
	{
		string text = data;
		int num = text.LastIndexOf("softwareVersion");
		if (num == -1)
		{
			num = text.LastIndexOf("Current Version");
			num += 46;
			text = text.Remove(0, num);
			num = text.IndexOf("</span>");
			return Utils.CompareVersion(VersionManager.bundleVersion, text.Remove(num));
		}
		num += 18;
		text = text.Remove(0, num);
		num = text.IndexOf("</div>") - 2;
		return Utils.CompareVersion(VersionManager.bundleVersion, text.Remove(num));
	}
}
