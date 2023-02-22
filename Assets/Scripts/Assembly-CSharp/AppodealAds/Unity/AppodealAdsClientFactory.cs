using AppodealAds.Unity.Android;
using AppodealAds.Unity.Common;

namespace AppodealAds.Unity
{
	internal class AppodealAdsClientFactory
	{
		internal static IAppodealAdsClient GetAppodealAdsClient()
		{
			return new AndroidAppodealClient();
		}
	}
}
