using System;
using UnityEngine;

public class AppMetrica : MonoBehaviour
{
    [SerializeField]
    private string APIKey;

    [SerializeField]
    private bool ExceptionsReporting = true;

    [SerializeField]
    private uint SessionTimeoutSec = 10u;

    [SerializeField]
    private bool LoggingEnabled = true;

    [SerializeField]
    private bool HandleFirstActivationAsUpdate;

    private static bool _isInitialized = false;

    private bool _actualPauseStatus;

    private static IYandexAppMetrica _metrica = null;

    private static object syncRoot = new UnityEngine.Object();

    public static IYandexAppMetrica Instance
    {
        get
        {
            if (_metrica == null)
            {
                lock (syncRoot)
                {
                    if (_metrica == null && Application.platform == RuntimePlatform.Android)
                    {
                        _metrica = new YandexAppMetricaAndroid();
                    }
                    if (_metrica == null)
                    {
                        _metrica = new YandexAppMetricaDummy();
                    }
                }
            }
            return _metrica;
        }
    }

    private void SetupMetrica()
    {
        YandexAppMetricaConfig yandexAppMetricaConfig = new YandexAppMetricaConfig(APIKey);
        YandexAppMetricaConfig yandexAppMetricaConfig2 = yandexAppMetricaConfig;
        yandexAppMetricaConfig2.SessionTimeout = (int)SessionTimeoutSec;
        yandexAppMetricaConfig2.LoggingEnabled = LoggingEnabled;
        yandexAppMetricaConfig2.HandleFirstActivationAsUpdateEnabled = HandleFirstActivationAsUpdate;
        yandexAppMetricaConfig = yandexAppMetricaConfig2;
        yandexAppMetricaConfig.TrackLocationEnabled = false;
        Instance.ActivateWithConfiguration(yandexAppMetricaConfig);
        ProcessCrashReports();
    }

    private void Awake()
    {
        if (!_isInitialized)
        {
            _isInitialized = true;
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
            SetupMetrica();
        }
        else
        {
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }

    private void Start()
    {
        Instance.OnResumeApplication();
    }

    private void OnEnable()
    {
        if (ExceptionsReporting)
        {
            ConsoleManager.LogCallback = (Action<string, string, LogType>)Delegate.Combine(ConsoleManager.LogCallback, new Action<string, string, LogType>(HandleLog));
        }
    }

    private void OnDisable()
    {
        if (ExceptionsReporting)
        {
            ConsoleManager.LogCallback = (Action<string, string, LogType>)Delegate.Remove(ConsoleManager.LogCallback, new Action<string, string, LogType>(HandleLog));
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (_actualPauseStatus != pauseStatus)
        {
            _actualPauseStatus = pauseStatus;
            if (pauseStatus)
            {
                Instance.OnPauseApplication();
            }
            else
            {
                Instance.OnResumeApplication();
            }
        }
    }

    public void ProcessCrashReports()
    {
        if (ExceptionsReporting)
        {
            CrashReport[] reports = CrashReport.reports;
            CrashReport[] array = reports;
            foreach (CrashReport crashReport in array)
            {
                string stackTrace = string.Format("Time: {0}\nText: {1}", crashReport.time, crashReport.text);
                Instance.ReportError("Crash", stackTrace);
                crashReport.Remove();
            }
        }
    }

    private void HandleLog(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            if (condition.Contains("Invalid IL code in"))
            {
                Application.Quit();
            }
            Instance.ReportError(condition, stackTrace);
        }
    }
}
