using System;
using System.Collections;
using System.Diagnostics;
using ExitGames.Client.Photon;
using UnityEngine;

internal class PhotonHandler : MonoBehaviour
{
	private const int SerializeRateFrameCorrection = 8;

	private const string PlayerPrefsKey = "Region";

	public static PhotonHandler SP;

	public int updateInterval;

	public int updateIntervalOnSerialize;

	private int nextSendTickCount;

	private int nextSendTickCountOnSerialize;

	private static bool sendThreadShouldRun;

	private static Stopwatch timerToStopConnectionInBackground;

	protected internal static bool AppQuits;

	protected internal static Type PingImplementation;

	private byte MaxDispathData = 1;

	public static int MaxDatagrams = 10;

	public static bool SendAsap;

	internal static CloudRegionCode BestRegionCodeInPreferences
	{
		get
		{
			string @string = nPlayerPrefs.GetString("Region", string.Empty);
			if (!string.IsNullOrEmpty(@string))
			{
				return Region.Parse(@string);
			}
			return CloudRegionCode.none;
		}
		set
		{
			if (value == CloudRegionCode.none)
			{
				nPlayerPrefs.DeleteKey("Region");
			}
			else
			{
				nPlayerPrefs.SetString("Region", value.ToString());
			}
		}
	}

	protected void Awake()
	{
		if (SP != null && SP != this && SP.gameObject != null)
		{
			UnityEngine.Object.DestroyImmediate(SP.gameObject);
		}
		SP = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		updateInterval = 1000 / PhotonNetwork.sendRate;
		updateIntervalOnSerialize = 1000 / PhotonNetwork.sendRateOnSerialize;
		StartFallbackSendAckThread();
	}

	protected void OnLevelWasLoaded(int level)
	{
		PhotonNetwork.networkingPeer.NewSceneLoaded();
		PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(SceneManagerHelper.ActiveSceneName, false);
		PhotonNetwork.networkingPeer.IsReloadingLevel = false;
		PhotonNetwork.networkingPeer.AsynchLevelLoadCall = false;
	}

	protected void OnApplicationQuit()
	{
		AppQuits = true;
		StopFallbackSendAckThread();
		PhotonNetwork.Disconnect();
	}

	protected void OnApplicationPause(bool pause)
	{
		if (PhotonNetwork.BackgroundTimeout > 0.1f)
		{
			if (timerToStopConnectionInBackground == null)
			{
				timerToStopConnectionInBackground = new Stopwatch();
			}
			timerToStopConnectionInBackground.Reset();
			if (pause)
			{
				timerToStopConnectionInBackground.Start();
			}
			else
			{
				timerToStopConnectionInBackground.Stop();
			}
		}
	}

	protected void OnDestroy()
	{
		StopFallbackSendAckThread();
	}

	protected void FixedUpdate()
	{
		if (PhotonNetwork.networkingPeer == null)
		{
			UnityEngine.Debug.LogError("NetworkPeer broke!");
			return;
		}
		bool flag = true;
		MaxDispathData = 1;
		while (PhotonNetwork.isMessageQueueRunning && flag && MaxDispathData <= 5)
		{
			flag = PhotonNetwork.networkingPeer.DispatchIncomingCommands();
			MaxDispathData++;
		}
	}

	protected void LateUpdate()
	{
		int num = (int)(Time.realtimeSinceStartup * 1000f);
		if (PhotonNetwork.isMessageQueueRunning && num > nextSendTickCountOnSerialize)
		{
			PhotonNetwork.networkingPeer.RunViewUpdate();
			nextSendTickCountOnSerialize = num + updateIntervalOnSerialize - 8;
			nextSendTickCount = 0;
		}
		num = (int)(Time.realtimeSinceStartup * 1000f);
		if (SendAsap || num > nextSendTickCount)
		{
			SendAsap = false;
			bool flag = true;
			int num2 = 0;
			while (PhotonNetwork.isMessageQueueRunning && flag && num2 < MaxDatagrams)
			{
				flag = PhotonNetwork.networkingPeer.SendOutgoingCommands();
				num2++;
			}
			nextSendTickCount = num + updateInterval;
		}
	}

	protected void OnJoinedRoom()
	{
		PhotonNetwork.networkingPeer.LoadLevelIfSynced();
	}

	protected void OnCreatedRoom()
	{
		PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(SceneManagerHelper.ActiveSceneName, false);
	}

	public static void StartFallbackSendAckThread()
	{
		if (!sendThreadShouldRun)
		{
			sendThreadShouldRun = true;
			SupportClass.StartBackgroundCalls(FallbackSendAckThread, 100, string.Empty);
		}
	}

	public static void StopFallbackSendAckThread()
	{
		sendThreadShouldRun = false;
	}

	public static bool FallbackSendAckThread()
	{
		if (sendThreadShouldRun && !PhotonNetwork.offlineMode && PhotonNetwork.networkingPeer != null)
		{
			if (timerToStopConnectionInBackground != null && PhotonNetwork.BackgroundTimeout > 0.1f && (float)timerToStopConnectionInBackground.ElapsedMilliseconds > PhotonNetwork.BackgroundTimeout * 1000f)
			{
				if (PhotonNetwork.connected)
				{
					PhotonNetwork.Disconnect();
				}
				timerToStopConnectionInBackground.Stop();
				timerToStopConnectionInBackground.Reset();
				return sendThreadShouldRun;
			}
			if (!PhotonNetwork.isMessageQueueRunning || PhotonNetwork.networkingPeer.ConnectionTime - PhotonNetwork.networkingPeer.LastSendOutgoingTime > 200)
			{
				PhotonNetwork.networkingPeer.SendAcksOnly();
			}
		}
		return sendThreadShouldRun;
	}

	protected internal static void PingAvailableRegionsAndConnectToBest()
	{
		SP.StartCoroutine(SP.PingAvailableRegionsCoroutine(true));
	}

	internal IEnumerator PingAvailableRegionsCoroutine(bool connectToBest)
	{
		while (PhotonNetwork.networkingPeer.AvailableRegions == null)
		{
			if (PhotonNetwork.connectionStateDetailed != ClientState.ConnectingToNameServer && PhotonNetwork.connectionStateDetailed != ClientState.ConnectedToNameServer)
			{
				UnityEngine.Debug.LogError("Call ConnectToNameServer to ping available regions.");
				yield break;
			}
			UnityEngine.Debug.Log(string.Concat("Waiting for AvailableRegions. State: ", PhotonNetwork.connectionStateDetailed, " Server: ", PhotonNetwork.Server, " PhotonNetwork.networkingPeer.AvailableRegions ", PhotonNetwork.networkingPeer.AvailableRegions != null));
			yield return new WaitForSeconds(0.25f);
		}
		if (PhotonNetwork.networkingPeer.AvailableRegions == null || PhotonNetwork.networkingPeer.AvailableRegions.Count == 0)
		{
			UnityEngine.Debug.LogError("No regions available. Are you sure your appid is valid and setup?");
			yield break;
		}
		PhotonPingManager pingManager = new PhotonPingManager();
		foreach (Region region in PhotonNetwork.networkingPeer.AvailableRegions)
		{
			SP.StartCoroutine(pingManager.PingSocket(region));
		}
		while (!pingManager.Done)
		{
			yield return new WaitForSeconds(0.1f);
		}
		Region best = pingManager.BestRegion;
		BestRegionCodeInPreferences = best.Code;
		if (connectToBest)
		{
			PhotonNetwork.networkingPeer.ConnectToRegionMaster(best.Code);
		}
	}
}
