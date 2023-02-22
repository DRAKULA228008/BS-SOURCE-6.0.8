using System.Collections.Generic;
using UnityEngine;

public static class YandexAppMetricaExtensionsAndroid
{
	public static AndroidJavaObject ToAndroidAppMetricaConfig(this YandexAppMetricaConfig self, AndroidJavaClass metricaClass)
	{
		AndroidJavaObject androidJavaObject = null;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.yandex.metrica.YandexMetricaConfig"))
		{
			AndroidJavaObject androidJavaObject2 = androidJavaClass.CallStatic<AndroidJavaObject>("newConfigBuilder", new object[1] { self.ApiKey });
			if (self.Location.HasValue)
			{
				YandexAppMetricaConfig.Coordinates value = self.Location.Value;
				androidJavaObject2.Call<AndroidJavaObject>("setLocation", new object[1] { value.ToAndroidLocation() });
			}
			if (self.AppVersion != null)
			{
				androidJavaObject2.Call<AndroidJavaObject>("setAppVersion", new object[1] { self.AppVersion });
			}
			if (self.TrackLocationEnabled.HasValue)
			{
				androidJavaObject2.Call<AndroidJavaObject>("setTrackLocationEnabled", new object[1] { self.TrackLocationEnabled.Value });
			}
			if (self.SessionTimeout.HasValue)
			{
				androidJavaObject2.Call<AndroidJavaObject>("setSessionTimeout", new object[1] { self.SessionTimeout.Value });
			}
			if (self.ReportCrashesEnabled.HasValue)
			{
				androidJavaObject2.Call<AndroidJavaObject>("setReportCrashesEnabled", new object[1] { self.ReportCrashesEnabled.Value });
			}
			bool? loggingEnabled = self.LoggingEnabled;
			if (loggingEnabled.HasValue && loggingEnabled.Value)
			{
				androidJavaObject2.Call<AndroidJavaObject>("setLogEnabled", new object[0]);
			}
			if (self.CollectInstalledApps.HasValue)
			{
				androidJavaObject2.Call<AndroidJavaObject>("setCollectInstalledApps", new object[1] { self.CollectInstalledApps.Value });
			}
			if (self.HandleFirstActivationAsUpdateEnabled.HasValue)
			{
				androidJavaObject2.Call<AndroidJavaObject>("handleFirstActivationAsUpdate", new object[1] { self.HandleFirstActivationAsUpdateEnabled.Value });
			}
			if (self.PreloadInfo.HasValue)
			{
				YandexAppMetricaPreloadInfo value2 = self.PreloadInfo.Value;
				AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.yandex.metrica.PreloadInfo");
				AndroidJavaObject androidJavaObject3 = androidJavaClass2.CallStatic<AndroidJavaObject>("newBuilder", new object[1] { value2.TrackingId });
				foreach (KeyValuePair<string, string> item in value2.AdditionalInfo)
				{
					androidJavaObject3.Call<AndroidJavaObject>("setAdditionalParams", new object[2] { item.Key, item.Value });
				}
				androidJavaObject2.Call<AndroidJavaObject>("setPreloadInfo", new object[1] { androidJavaObject3.Call<AndroidJavaObject>("build", new object[0]) });
			}
			androidJavaObject2.Call<AndroidJavaObject>("setReportNativeCrashesEnabled", new object[1] { false });
			return androidJavaObject2.Call<AndroidJavaObject>("build", new object[0]);
		}
	}

	public static AndroidJavaObject ToAndroidLocation(this YandexAppMetricaConfig.Coordinates self)
	{
		AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.location.Location", string.Empty);
		androidJavaObject.Call("setLatitude", self.Latitude);
		androidJavaObject.Call("setLongitude", self.Longitude);
		return androidJavaObject;
	}
}
