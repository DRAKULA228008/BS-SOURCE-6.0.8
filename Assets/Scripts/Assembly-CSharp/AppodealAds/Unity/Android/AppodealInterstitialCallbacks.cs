using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android
{
	public class AppodealInterstitialCallbacks : AndroidJavaProxy
	{
		private IInterstitialAdListener listener;

		internal AppodealInterstitialCallbacks(IInterstitialAdListener listener)
			: base("com.appodeal.ads.InterstitialCallbacks")
		{
			this.listener = listener;
		}

		private void onInterstitialLoaded(bool isPrecache)
		{
			listener.onInterstitialLoaded(isPrecache);
		}

		private void onInterstitialFailedToLoad()
		{
			listener.onInterstitialFailedToLoad();
		}

		private void onInterstitialShown()
		{
			listener.onInterstitialShown();
		}

		private void onInterstitialClicked()
		{
			listener.onInterstitialClicked();
		}

		private void onInterstitialClosed()
		{
			listener.onInterstitialClosed();
		}

		private void onInterstitialExpired()
		{
			listener.onInterstitialExpired();
		}
	}
}
