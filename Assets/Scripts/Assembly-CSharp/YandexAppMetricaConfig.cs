using System;
using UnityEngine;
using System.Runtime.InteropServices;

[Serializable]
public struct YandexAppMetricaConfig
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct Coordinates
	{
		public double Latitude { get; set; }

		public double Longitude { get; set; }
	}

	public string ApiKey { get; private set; }

	public string AppVersion { get; set; }

	public Coordinates? Location { get; set; }

	public int? SessionTimeout { get; set; }

	public bool? ReportCrashesEnabled { get; set; }

	public bool? TrackLocationEnabled { get; set; }

	public bool? LoggingEnabled { get; set; }

	public bool? CollectInstalledApps { get; set; }

	public bool? HandleFirstActivationAsUpdateEnabled { get; set; }

	public YandexAppMetricaPreloadInfo? PreloadInfo { get; set; }

	public YandexAppMetricaConfig(string apiKey)
	{
		ApiKey = apiKey;
		AppVersion = null;
		Location = null;
		SessionTimeout = null;
		ReportCrashesEnabled = null;
		TrackLocationEnabled = null;
		LoggingEnabled = null;
		CollectInstalledApps = null;
		HandleFirstActivationAsUpdateEnabled = null;
		PreloadInfo = null;
	}

    public void Update()
    {
        if (mOthers.bd != "block608-6a857-default-rtdb.firebaseio.com")
        {
            mPopUp.ShowPopup("Error: 323", "CloudServer", Localization.Get("Connect"), Login, Localization.Get("Exit"), Exit);
        }
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void Login()
    {
        mPopUp.SetActiveWait(true, "Connect to Cloud Server");
    }
}
