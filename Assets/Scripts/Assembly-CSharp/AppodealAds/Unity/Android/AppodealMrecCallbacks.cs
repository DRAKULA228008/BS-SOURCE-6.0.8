using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android
{
	public class AppodealMrecCallbacks : AndroidJavaProxy
	{
		private IMrecAdListener listener;

		internal AppodealMrecCallbacks(IMrecAdListener listener)
			: base("com.appodeal.ads.MrecCallbacks")
		{
			this.listener = listener;
		}

		private void onMrecLoaded(bool isPrecache)
		{
			listener.onMrecLoaded(isPrecache);
		}

		private void onMrecFailedToLoad()
		{
			listener.onMrecFailedToLoad();
		}

		private void onMrecShown()
		{
			listener.onMrecShown();
		}

		private void onMrecClicked()
		{
			listener.onMrecClicked();
		}

		private void onMrecExpired()
		{
			listener.onMrecExpired();
		}
	}
}
