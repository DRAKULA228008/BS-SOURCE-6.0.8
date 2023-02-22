using UnityEngine;

public class mInAppManager : MonoBehaviour
{
	private GameCurrency Currency;

	private bool isRewardedVideo;

	public void OnRewardedVideo(int currency)
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
		}
		else
		{
			if (isRewardedVideo)
			{
				return;
			}
			Currency = (GameCurrency)currency;
			if (Currency != GameCurrency.Gold || AccountManager.GetGold() < 8000)
			{
				UIToast.Show(Localization.Get("Please wait") + "...");
				isRewardedVideo = true;
				TimerManager.In(0.5f, delegate
				{
					AdsManager.ShowRewardedVideo(RewardedVideoComplete, RewardedVideoFailed, RewardedVideoAborted, (Currency != GameCurrency.Gold) ? "RewardedMoney" : "RewardedGold");
				});
			}
		}
	}

	private void RewardedVideoComplete(int amount, string name)
	{
		TimerManager.In(0.3f, delegate
		{
			AccountManager.Rewarded(Currency, delegate
			{
				if (Currency == GameCurrency.Money)
				{
					UIToast.Show("+50 BS Silver");
				}
				else
				{
					UIToast.Show("+1 BS Coins");
				}
				EventManager.Dispatch("AccountUpdate");
				isRewardedVideo = false;
			}, delegate(string e)
			{
				UIToast.Show(e);
				isRewardedVideo = false;
			});
		});
	}

	private void RewardedVideoAborted()
	{
		UIToast.Show(Localization.Get("Cancel"));
		isRewardedVideo = false;
	}

	private void RewardedVideoFailed()
	{
		isRewardedVideo = false;
		UIToast.Show(Localization.Get("Video not available"), 3f);
	}
}
