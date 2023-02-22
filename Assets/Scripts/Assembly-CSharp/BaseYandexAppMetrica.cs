using System.Collections.Generic;

public abstract class BaseYandexAppMetrica : IYandexAppMetrica
{
	private YandexAppMetricaConfig? _metricaConfig;

	public YandexAppMetricaConfig? ActivationConfig
	{
		get
		{
			return _metricaConfig;
		}
	}

	public abstract bool CollectInstalledApps { get; set; }

	public abstract string LibraryVersion { get; }

	public abstract int LibraryApiLevel { get; }

	public event ConfigUpdateHandler OnActivation;

	public virtual void ActivateWithAPIKey(string apiKey)
	{
		UpdateConfiguration(new YandexAppMetricaConfig(apiKey));
	}

	public virtual void ActivateWithConfiguration(YandexAppMetricaConfig config)
	{
		UpdateConfiguration(config);
	}

	private void UpdateConfiguration(YandexAppMetricaConfig config)
	{
		_metricaConfig = config;
		ConfigUpdateHandler onActivation = this.OnActivation;
		if (onActivation != null)
		{
			onActivation(config);
		}
	}

	public abstract void OnResumeApplication();

	public abstract void OnPauseApplication();

	public abstract void ReportEvent(string message);

	public abstract void ReportEvent(string message, Dictionary<string, object> parameters);

	public abstract void ReportEvent(string message, string key, string value);

	public abstract void ReportError(string condition, string stackTrace);

	public abstract void SetTrackLocationEnabled(bool enabled);

	public abstract void SetLocation(YandexAppMetricaConfig.Coordinates coordinates);

	public abstract void SetSessionTimeout(uint sessionTimeoutSeconds);

	public abstract void SetReportCrashesEnabled(bool enabled);

	public abstract void SetCustomAppVersion(string appVersion);

	public abstract void SetLoggingEnabled();

	public abstract void SetEnvironmentValue(string key, string value);
}
