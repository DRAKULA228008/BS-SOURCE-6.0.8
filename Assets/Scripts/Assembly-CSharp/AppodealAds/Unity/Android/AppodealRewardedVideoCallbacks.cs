using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android
{
	public class AppodealRewardedVideoCallbacks : AndroidJavaProxy
	{
		private IRewardedVideoAdListener listener;

		internal AppodealRewardedVideoCallbacks(IRewardedVideoAdListener listener)
			: base("com.appodeal.ads.RewardedVideoCallbacks")
		{
			this.listener = listener;
		}

		private void onRewardedVideoLoaded(bool precache)
		{
			listener.onRewardedVideoLoaded(precache);
		}

		private void onRewardedVideoFailedToLoad()
		{
			listener.onRewardedVideoFailedToLoad();
		}

		private void onRewardedVideoShown()
		{
			listener.onRewardedVideoShown();
		}

		private void onRewardedVideoFinished(double amount, AndroidJavaObject name)
		{
			listener.onRewardedVideoFinished(amount, null);
		}

		private void onRewardedVideoFinished(double amount, string name)
		{
			listener.onRewardedVideoFinished(amount, name);
		}

		private void onRewardedVideoClosed(bool finished)
		{
			listener.onRewardedVideoClosed(finished);
		}

		private void onRewardedVideoExpired()
		{
			listener.onRewardedVideoExpired();
		}
	}
}
