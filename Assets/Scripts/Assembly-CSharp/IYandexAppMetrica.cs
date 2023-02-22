using System.Collections.Generic;

public interface IYandexAppMetrica
{
	YandexAppMetricaConfig? ActivationConfig { get; }

	bool CollectInstalledApps { get; set; }

	string LibraryVersion { get; }

	int LibraryApiLevel { get; }

	event ConfigUpdateHandler OnActivation;

	void ActivateWithAPIKey(string apiKey);

	void ActivateWithConfiguration(YandexAppMetricaConfig config);

	void OnResumeApplication();

	void OnPauseApplication();

	void ReportEvent(string message);

	void ReportEvent(string message, Dictionary<string, object> parameters);

	void ReportEvent(string message, string key, string value);

	void ReportError(string condition, string stackTrace);

	void SetTrackLocationEnabled(bool enabled);

	void SetLocation(YandexAppMetricaConfig.Coordinates coordinates);

	void SetSessionTimeout(uint sessionTimeoutSeconds);

	void SetReportCrashesEnabled(bool enabled);

	void SetCustomAppVersion(string appVersion);

	void SetLoggingEnabled();

	void SetEnvironmentValue(string key, string value);
}
