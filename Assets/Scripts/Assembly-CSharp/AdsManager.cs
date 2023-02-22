using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200034D RID: 845
public class AdsManager : MonoBehaviour
{
    // Token: 0x06001D50 RID: 7504 RVA: 0x00013C84 File Offset: 0x00011E84
    private void Start()
    {

    }

    // Token: 0x06001D51 RID: 7505 RVA: 0x0009FE2C File Offset: 0x0009E02C
    private void Init()
    {

    }

    // Token: 0x06001D52 RID: 7506 RVA: 0x0009FECC File Offset: 0x0009E0CC
    private IEnumerator CheckHostsFile()
    {
        WWW www = (WWW)(object)new WWW("file:///etc/hosts");
        yield return www;
        if (www.error == string.Empty && www.size > 100)
        {
            isBlockAds = true;
        }
    }

    // Token: 0x06001D53 RID: 7507 RVA: 0x00013C97 File Offset: 0x00011E97
    public static void ShowAny()
    {
        AdsManager.ShowAny("default");
    }

    // Token: 0x06001D54 RID: 7508 RVA: 0x00013CA3 File Offset: 0x00011EA3
    public static void ShowAny(string placement)
    {

    }

    // Token: 0x06001D55 RID: 7509 RVA: 0x00013CDB File Offset: 0x00011EDB
    public static void ShowInterstitial()
    {
        AdsManager.ShowInterstitial("default");
    }

    // Token: 0x06001D56 RID: 7510 RVA: 0x00013CE7 File Offset: 0x00011EE7
    public static void ShowInterstitial(string placement)
    {

    }

    // Token: 0x06001D57 RID: 7511 RVA: 0x00013CFF File Offset: 0x00011EFF
    public static void ShowRewardedVideo(Action<int, string> complete, Action failed, Action aborted)
    {
        AdsManager.ShowRewardedVideo(complete, failed, aborted, "default");
    }

    // Token: 0x06001D58 RID: 7512 RVA: 0x00013D0E File Offset: 0x00011F0E
    public static void ShowRewardedVideo(Action<int, string> complete, Action failed, Action aborted, string placement)
    {

    }

    // Token: 0x06001D59 RID: 7513 RVA: 0x00013D4E File Offset: 0x00011F4E
    private static void RewardedVideoFinished(int isComplete)
    {
        AdsManager.RewardedVideoFinished(isComplete, 0, string.Empty);
    }

    // Token: 0x06001D5A RID: 7514 RVA: 0x0009FEE0 File Offset: 0x0009E0E0
    private static void RewardedVideoFinished(int isComplete, int amount, string name)
    {
        if (AdsManager.FinishRewardedVideo != 0)
        {
            AdsManager.FinishRewardedVideo = isComplete;
        }
        if (!TimerManager.IsActive(AdsManager.TimerID))
        {
            AdsManager.TimerID = TimerManager.In(0.2f, delegate ()
            {
                if (isComplete == 0)
                {
                    if (AdsManager.RewardedVideoComplete != null)
                    {
                        AdsManager.RewardedVideoComplete(amount, name);
                    }
                }
                else if (isComplete == 1)
                {
                    if (AdsManager.RewardedVideoAborted != null)
                    {
                        AdsManager.RewardedVideoAborted();
                    }
                }
                else if (AdsManager.RewardedVideoFailed != null)
                {
                    AdsManager.RewardedVideoFailed();
                }
                AdsManager.RewardedVideoComplete = null;
                AdsManager.RewardedVideoFailed = null;
                AdsManager.RewardedVideoAborted = null;
            });
        }
    }

    // Token: 0x06001D5B RID: 7515 RVA: 0x0009FF44 File Offset: 0x0009E144
    private void Print(string text)
    {
        if (Settings.Console)
        {
            TimerManager.In(0.1f, delegate ()
            {
                MonoBehaviour.print(text);
            });
        }
    }

    // Token: 0x06001D5C RID: 7516 RVA: 0x00013D5C File Offset: 0x00011F5C
    public void onRewardedVideoLoaded()
    {
        this.Print("Rewarded Video loaded");
    }

    // Token: 0x06001D5D RID: 7517 RVA: 0x00013D69 File Offset: 0x00011F69
    public void onRewardedVideoFailedToLoad()
    {
        AdsManager.RewardedVideoFinished(1);
        this.Print("Rewarded Video failed");
    }

    // Token: 0x06001D5E RID: 7518 RVA: 0x00013D7C File Offset: 0x00011F7C
    public void onRewardedVideoShown()
    {
        this.Print("Rewarded Video shown");
    }

    // Token: 0x06001D5F RID: 7519 RVA: 0x00013D89 File Offset: 0x00011F89
    public void onRewardedVideoClosed(bool isFinished)
    {
        this.Print("Rewarded Video closed");
        AdsManager.isShow = false;
    }

    // Token: 0x06001D60 RID: 7520 RVA: 0x00013D9C File Offset: 0x00011F9C
    public void onRewardedVideoFinished(int amount, string name)
    {
        AdsManager.RewardedVideoFinished(0);
        this.Print("Rewarded Video finished");
        AdsManager.isShow = false;
    }

    // Token: 0x06001D61 RID: 7521 RVA: 0x00013DB5 File Offset: 0x00011FB5
    public void onInterstitialLoaded(bool isPrecache)
    {
        this.Print("Interstitial loaded");
    }

    // Token: 0x06001D62 RID: 7522 RVA: 0x00013DC2 File Offset: 0x00011FC2
    public void onInterstitialFailedToLoad()
    {
        this.Print("Interstitial failed");
    }

    // Token: 0x06001D63 RID: 7523 RVA: 0x00013DCF File Offset: 0x00011FCF
    public void onInterstitialShown()
    {
        this.Print("Interstitial opened");
    }

    // Token: 0x06001D64 RID: 7524 RVA: 0x00013DDC File Offset: 0x00011FDC
    public void onInterstitialClosed()
    {
        this.Print("Interstitial closed");
        AdsManager.isShow = false;
    }

    // Token: 0x06001D65 RID: 7525 RVA: 0x00013DEF File Offset: 0x00011FEF
    public void onInterstitialClicked()
    {
        this.Print("Interstitial clicked");
        AdsManager.isShow = false;
    }

    // Token: 0x0400119F RID: 4511
    public static bool isBlockAds;

    // Token: 0x040011A0 RID: 4512
    public static bool isShow;

    // Token: 0x040011A1 RID: 4513
    private static Action<int, string> RewardedVideoComplete;

    // Token: 0x040011A2 RID: 4514
    private static Action RewardedVideoFailed;

    // Token: 0x040011A3 RID: 4515
    private static Action RewardedVideoAborted;

    // Token: 0x040011A4 RID: 4516
    private static int TimerID;

    // Token: 0x040011A5 RID: 4517
    private static int FinishRewardedVideo = -1;
}
