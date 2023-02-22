using System.Collections.Generic;
using AppodealAds.Unity.Common;

namespace AppodealAds.Unity.Api
{
	public class Appodeal
	{
		public enum LogLevel
		{
			None,
			Debug,
			Verbose
		}

		public const int NONE = 0;

		public const int INTERSTITIAL = 3;

		public const int BANNER = 4;

		public const int BANNER_BOTTOM = 8;

		public const int BANNER_TOP = 16;

		public const int BANNER_VIEW = 64;

		public const int MREC = 512;

		public const int REWARDED_VIDEO = 128;

		public const int NON_SKIPPABLE_VIDEO = 128;

		public const int BANNER_HORIZONTAL_SMART = -1;

		public const int BANNER_HORIZONTAL_CENTER = -2;

		public const int BANNER_HORIZONTAL_RIGHT = -3;

		public const int BANNER_HORIZONTAL_LEFT = -4;

		public const string APPODEAL_PLUGIN_VERSION = "2.8.49";

		private static IAppodealAdsClient client;

		private static IAppodealAdsClient getInstance()
		{
			if (client == null)
			{
				client = AppodealAdsClientFactory.GetAppodealAdsClient();
			}
			return client;
		}

		public static void initialize(string appKey, int adTypes)
		{
			getInstance().initialize(appKey, adTypes);
		}

		public static void initialize(string appKey, int adTypes, bool hasConsent)
		{
			getInstance().initialize(appKey, adTypes, hasConsent);
		}

		public static bool show(int adTypes)
		{
			return getInstance().show(adTypes);
		}

		public static bool show(int adTypes, string placement)
		{
			return getInstance().show(adTypes, placement);
		}

		public static bool showBannerView(int YAxis, int XGravity, string placement)
		{
			return getInstance().showBannerView(YAxis, XGravity, placement);
		}

		public static bool showMrecView(int YAxis, int XGravity, string placement)
		{
			return getInstance().showMrecView(YAxis, XGravity, placement);
		}

		public static bool isLoaded(int adTypes)
		{
			return getInstance().isLoaded(adTypes);
		}

		public static void cache(int adTypes)
		{
			getInstance().cache(adTypes);
		}

		public static void hide(int adTypes)
		{
			getInstance().hide(adTypes);
		}

		public static void hideBannerView()
		{
			getInstance().hideBannerView();
		}

		public static void hideMrecView()
		{
			getInstance().hideMrecView();
		}

		public static void setAutoCache(int adTypes, bool autoCache)
		{
			getInstance().setAutoCache(adTypes, autoCache);
		}

		public static bool isPrecache(int adTypes)
		{
			return getInstance().isPrecache(adTypes);
		}

		public static void onResume()
		{
			getInstance().onResume();
		}

		public static void setSmartBanners(bool value)
		{
			getInstance().setSmartBanners(value);
		}

		public static void setBannerBackground(bool value)
		{
			getInstance().setBannerBackground(value);
		}

		public static void setBannerAnimation(bool value)
		{
			getInstance().setBannerAnimation(value);
		}

		public static void setTabletBanners(bool value)
		{
			getInstance().setTabletBanners(value);
		}

		public static void setTesting(bool test)
		{
			getInstance().setTesting(test);
		}

		public static void setLogLevel(LogLevel log)
		{
			getInstance().setLogLevel(log);
		}

		public static void setChildDirectedTreatment(bool value)
		{
			getInstance().setChildDirectedTreatment(value);
		}

		public static void disableNetwork(string network)
		{
			getInstance().disableNetwork(network);
		}

		public static void disableNetwork(string network, int adType)
		{
			getInstance().disableNetwork(network, adType);
		}

		public static void disableLocationPermissionCheck()
		{
			getInstance().disableLocationPermissionCheck();
		}

		public static void disableWriteExternalStoragePermissionCheck()
		{
			getInstance().disableWriteExternalStoragePermissionCheck();
		}

		public static void setTriggerOnLoadedOnPrecache(int adTypes, bool onLoadedTriggerBoth)
		{
			getInstance().setTriggerOnLoadedOnPrecache(adTypes, onLoadedTriggerBoth);
		}

		public static void muteVideosIfCallsMuted(bool value)
		{
			getInstance().muteVideosIfCallsMuted(value);
		}

		public static void showTestScreen()
		{
			getInstance().showTestScreen();
		}

		public static bool canShow(int adTypes)
		{
			return getInstance().canShow(adTypes);
		}

		public static bool canShow(int adTypes, string placement)
		{
			return getInstance().canShow(adTypes, placement);
		}

		public static void setSegmentFilter(string name, bool value)
		{
			getInstance().setSegmentFilter(name, value);
		}

		public static void setSegmentFilter(string name, int value)
		{
			getInstance().setSegmentFilter(name, value);
		}

		public static void setSegmentFilter(string name, double value)
		{
			getInstance().setSegmentFilter(name, value);
		}

		public static void setSegmentFilter(string name, string value)
		{
			getInstance().setSegmentFilter(name, value);
		}

		public static void setExtraData(string key, bool value)
		{
			getInstance().setExtraData(key, value);
		}

		public static void setExtraData(string key, int value)
		{
			getInstance().setExtraData(key, value);
		}

		public static void setExtraData(string key, double value)
		{
			getInstance().setExtraData(key, value);
		}

		public static void setExtraData(string key, string value)
		{
			getInstance().setExtraData(key, value);
		}

		public static void trackInAppPurchase(double amount, string currency)
		{
			getInstance().trackInAppPurchase(amount, currency);
		}

		public static string getNativeSDKVersion()
		{
			return getInstance().getVersion();
		}

		public static string getPluginVersion()
		{
			return "2.8.49";
		}

		public static void setInterstitialCallbacks(IInterstitialAdListener listener)
		{
			getInstance().setInterstitialCallbacks(listener);
		}

		public static void setNonSkippableVideoCallbacks(INonSkippableVideoAdListener listener)
		{
			getInstance().setNonSkippableVideoCallbacks(listener);
		}

		public static void setRewardedVideoCallbacks(IRewardedVideoAdListener listener)
		{
			getInstance().setRewardedVideoCallbacks(listener);
		}

		public static void setBannerCallbacks(IBannerAdListener listener)
		{
			getInstance().setBannerCallbacks(listener);
		}

		public static void setMrecCallbacks(IMrecAdListener listener)
		{
			getInstance().setMrecCallbacks(listener);
		}

		public static void requestAndroidMPermissions(IPermissionGrantedListener listener)
		{
			getInstance().requestAndroidMPermissions(listener);
		}

		public static KeyValuePair<string, double> getRewardParameters()
		{
			return new KeyValuePair<string, double>(getInstance().getRewardCurrency(), getInstance().getRewardAmount());
		}

		public static KeyValuePair<string, double> getRewardParameters(string placement)
		{
			return new KeyValuePair<string, double>(getInstance().getRewardCurrency(placement), getInstance().getRewardAmount(placement));
		}

		public static double getPredictedEcpm(int adType)
		{
			return getInstance().getPredictedEcpm(adType);
		}

		public static void destroy(int adTypes)
		{
			getInstance().destroy(adTypes);
		}
	}
}
