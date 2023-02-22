using System.Collections.Generic;

public class YandexAppMetricaDummy : BaseYandexAppMetrica
{
	public override bool CollectInstalledApps
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public override string LibraryVersion
	{
		get
		{
			return null;
		}
	}

	public override int LibraryApiLevel
	{
		get
		{
			return 0;
		}
	}

	public override void ActivateWithAPIKey(string apiKey)
	{
	}

	public override void ActivateWithConfiguration(YandexAppMetricaConfig config)
	{
	}

	public override void OnResumeApplication()
	{
	}

	public override void OnPauseApplication()
	{
	}

	public override void ReportEvent(string message)
	{
	}

	public override void ReportEvent(string message, Dictionary<string, object> parameters)
	{
	}

	public override void ReportEvent(string message, string key, string value)
	{
	}

	public override void ReportError(string condition, string stackTrace)
	{
	}

	public override void SetTrackLocationEnabled(bool enabled)
	{
	}

	public override void SetLocation(YandexAppMetricaConfig.Coordinates coordinates)
	{
	}

	public override void SetSessionTimeout(uint sessionTimeoutSeconds)
	{
	}

	public override void SetReportCrashesEnabled(bool enabled)
	{
	}

	public override void SetCustomAppVersion(string appVersion)
	{
	}

	public override void SetLoggingEnabled()
	{
	}

	public override void SetEnvironmentValue(string key, string value)
	{
	}
}
