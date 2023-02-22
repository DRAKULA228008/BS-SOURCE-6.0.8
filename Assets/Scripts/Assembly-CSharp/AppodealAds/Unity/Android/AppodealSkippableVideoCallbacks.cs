using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android
{
	public class AppodealSkippableVideoCallbacks : AndroidJavaProxy
	{
		private ISkippableVideoAdListener listener;

		internal AppodealSkippableVideoCallbacks(ISkippableVideoAdListener listener)
			: base("com.appodeal.ads.SkippableVideoCallbacks")
		{
			this.listener = listener;
		}

		private void onSkippableVideoLoaded()
		{
			listener.onSkippableVideoLoaded();
		}

		private void onSkippableVideoFailedToLoad()
		{
			listener.onSkippableVideoFailedToLoad();
		}

		private void onSkippableVideoShown()
		{
			listener.onSkippableVideoShown();
		}

		private void onSkippableVideoFinished()
		{
			listener.onSkippableVideoFinished();
		}

		private void onSkippableVideoClosed(bool finished)
		{
			listener.onSkippableVideoClosed();
		}
	}
}
