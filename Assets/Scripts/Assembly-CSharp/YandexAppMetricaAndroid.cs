using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YandexAppMetricaAndroid : BaseYandexAppMetrica
{
	private AndroidJavaClass metricaClass;

	public override bool CollectInstalledApps
	{
		get
		{
			if (metricaClass != null)
			{
				return metricaClass.CallStatic<bool>("getCollectInstalledApps", new object[0]);
			}
			return false;
		}
		set
		{
			if (metricaClass != null)
			{
				metricaClass.CallStatic("setCollectInstalledApps", value);
			}
		}
	}

	public override int LibraryApiLevel
	{
		get
		{
			if (metricaClass != null)
			{
				return metricaClass.CallStatic<int>("getLibraryApiLevel", new object[0]);
			}
			return 0;
		}
	}

	public override string LibraryVersion
	{
		get
		{
			if (metricaClass != null)
			{
				return metricaClass.CallStatic<string>("getLibraryVersion", new object[0]);
			}
			return null;
		}
	}

	public override void ActivateWithAPIKey(string apiKey)
	{
		base.ActivateWithAPIKey(apiKey);
		metricaClass = new AndroidJavaClass("com.yandex.metrica.YandexMetrica");
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			metricaClass.CallStatic("activate", @static, apiKey);
		}
	}

	public override void ActivateWithConfiguration(YandexAppMetricaConfig config)
	{
		base.ActivateWithConfiguration(config);
		metricaClass = new AndroidJavaClass("com.yandex.metrica.YandexMetrica");
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			metricaClass.CallStatic("activate", @static, config.ToAndroidAppMetricaConfig(metricaClass));
		}
	}

	public override void OnResumeApplication()
	{
		if (metricaClass != null)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				metricaClass.CallStatic("onResumeActivity", @static);
			}
		}
	}

	public override void OnPauseApplication()
	{
		if (metricaClass != null)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				metricaClass.CallStatic("onPauseActivity", @static);
			}
		}
	}

	public override void ReportEvent(string message)
	{
		if (metricaClass != null)
		{
			metricaClass.CallStatic("reportEvent", message);
		}
	}

	public override void ReportEvent(string message, Dictionary<string, object> parameters)
	{
		if (metricaClass != null)
		{
			metricaClass.CallStatic("reportEvent", message, FirebaseMiniJson.Serialize(parameters));
		}
	}

	public override void ReportEvent(string message, string key, string value)
	{
		if (metricaClass != null)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add(key, value);
			metricaClass.CallStatic("reportEvent", message, FirebaseMiniJson.Serialize(hashtable));
		}
	}

	public override void ReportError(string condition, string stackTrace)
	{
		if (metricaClass != null)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.lang.Throwable", "\n" + stackTrace);
			metricaClass.CallStatic("reportError", condition, androidJavaObject);
		}
	}

	public override void SetTrackLocationEnabled(bool enabled)
	{
		if (metricaClass != null)
		{
			metricaClass.CallStatic("setTrackLocationEnabled", enabled);
		}
	}

	public override void SetLocation(YandexAppMetricaConfig.Coordinates coordinates)
	{
		if (metricaClass != null)
		{
			metricaClass.CallStatic("setLocation", coordinates.ToAndroidLocation());
		}
	}

	public override void SetSessionTimeout(uint sessionTimeoutSeconds)
	{
		if (metricaClass != null)
		{
			metricaClass.CallStatic("setSessionTimeout", (int)sessionTimeoutSeconds);
		}
	}

	public override void SetReportCrashesEnabled(bool enabled)
	{
		if (metricaClass != null)
		{
			metricaClass.CallStatic("setReportCrashesEnabled", enabled);
		}
	}

	public override void SetCustomAppVersion(string appVersion)
	{
		if (metricaClass != null)
		{
			metricaClass.CallStatic("setCustomAppVersion", appVersion);
		}
	}

	public override void SetLoggingEnabled()
	{
		if (metricaClass != null)
		{
			metricaClass.CallStatic("setLoggingEnabled");
		}
	}

	public override void SetEnvironmentValue(string key, string value)
	{
		if (metricaClass != null)
		{
			metricaClass.CallStatic("setEnvironmentValue", key, value);
		}
	}
}
