using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonPingManager
{
	private const string wssProtocolString = "wss://";

	public bool UseNative;

	public static int Attempts = 5;

	public static bool IgnoreInitialAttempt = true;

	public static int MaxMilliseconsPerPing = 800;

	private int PingsRunning;

	public Region BestRegion
	{
		get
		{
			Region result = null;
			int num = int.MaxValue;
			foreach (Region availableRegion in PhotonNetwork.networkingPeer.AvailableRegions)
			{
				if (availableRegion.Ping != 0 && availableRegion.Ping < num)
				{
					num = availableRegion.Ping;
					result = availableRegion;
				}
			}
			return result;
		}
	}

	public bool Done
	{
		get
		{
			return PingsRunning == 0;
		}
	}

	public IEnumerator PingSocket(Region region)
	{
		region.Ping = Attempts * MaxMilliseconsPerPing;
		PingsRunning++;
		PhotonPing ping;
		if (PhotonHandler.PingImplementation == typeof(SocketUdp))
		{
			UnityEngine.Debug.Log("Using constructor for new PingNativeDynamic()");
			//ping = new SocketUdp();
		}
		else if (PhotonHandler.PingImplementation != typeof(SocketUdp))
		{
			ping = ((PhotonHandler.PingImplementation != typeof(PingMono)) ? ((PhotonPing)Activator.CreateInstance(PhotonHandler.PingImplementation)) : new PingMono());
		}
		else
		{
			UnityEngine.Debug.Log("Using constructor for new PingNativeStatic()");
			//ping = new SocketUdp();
		}
		float rttSum = 0f;
		int replyCount = 0;
		string regionAddress = region.HostAndPort;
		int indexOfColon = regionAddress.LastIndexOf(':');
		if (indexOfColon > 1)
		{
			regionAddress = regionAddress.Substring(0, indexOfColon);
		}
		int indexOfProtocol = regionAddress.IndexOf("wss://");
		if (indexOfProtocol > -1)
		{
			regionAddress = regionAddress.Substring(indexOfProtocol + "wss://".Length);
		}
		regionAddress = ResolveHost(regionAddress);
		for (int i = 0; i < Attempts; i++)
		{
			bool overtime = false;
			Stopwatch sw = new Stopwatch();
			sw.Start();
			try
			{
				//ping.StartPing(regionAddress);
			}
			catch (Exception e)
			{
				UnityEngine.Debug.Log("catched: " + e);
				PingsRunning--;
				break;
			}
			//while (!ping.Done())
			{
				if (sw.ElapsedMilliseconds >= MaxMilliseconsPerPing)
				{
					overtime = true;
					break;
				}
				yield return 0;
			}
			int rtt = (int)sw.ElapsedMilliseconds;
			//if ((!IgnoreInitialAttempt || i != 0) && ping.Successful && !overtime)
			{
				rttSum += (float)rtt;
				replyCount++;
				region.Ping = (int)(rttSum / (float)replyCount);
			}
			yield return new WaitForSeconds(0.1f);
		}
		//ping.Dispose();
		PingsRunning--;
		yield return null;
	}

	public static string ResolveHost(string hostName)
	{
		string text = string.Empty;
		try
		{
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostName);
			if (hostAddresses.Length == 1)
			{
				return hostAddresses[0].ToString();
			}
			foreach (IPAddress iPAddress in hostAddresses)
			{
				if (iPAddress != null)
				{
					if (iPAddress.ToString().Contains(":"))
					{
						return iPAddress.ToString();
					}
					if (string.IsNullOrEmpty(text))
					{
						text = hostAddresses.ToString();
					}
				}
			}
			return text;
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.Log("Exception caught! " + ex.Source + " Message: " + ex.Message);
			return text;
		}
	}
}
