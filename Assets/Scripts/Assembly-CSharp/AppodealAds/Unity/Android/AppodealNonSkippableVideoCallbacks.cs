using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android
{
	public class AppodealNonSkippableVideoCallbacks : AndroidJavaProxy
	{
		private INonSkippableVideoAdListener listener;

		internal AppodealNonSkippableVideoCallbacks(INonSkippableVideoAdListener listener)
			: base("com.appodeal.ads.NonSkippableVideoCallbacks")
		{
			this.listener = listener;
		}

		private void onNonSkippableVideoLoaded(bool isPrecache)
		{
			listener.onNonSkippableVideoLoaded(isPrecache);
		}

		private void onNonSkippableVideoFailedToLoad()
		{
			listener.onNonSkippableVideoFailedToLoad();
		}

		private void onNonSkippableVideoShown()
		{
			listener.onNonSkippableVideoShown();
		}

		private void onNonSkippableVideoFinished()
		{
			listener.onNonSkippableVideoFinished();
		}

		private void onNonSkippableVideoClosed(bool finished)
		{
			listener.onNonSkippableVideoClosed(finished);
		}

		private void onNonSkippableVideoExpired()
		{
			listener.onNonSkippableVideoExpired();
		}
	}
}
