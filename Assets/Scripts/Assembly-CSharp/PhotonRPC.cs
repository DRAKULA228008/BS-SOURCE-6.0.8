using System.Collections.Generic;
using UnityEngine;

public static class PhotonRPC
{
	public delegate void MessageDelegate(PhotonMessage message);

	public static readonly byte eventCode = 42;

	private static PhotonHashtable rpcEvent = new PhotonHashtable();

	private static PhotonHashtable rpcData = new PhotonHashtable();

	private static byte[] rpcEventBytes;

	private static RaiseEventOptions optionsRpcEvent = new RaiseEventOptions();

	private static Dictionary<string, MessageDelegate> messages = new Dictionary<string, MessageDelegate>();

	private static PhotonDataWrite data = new PhotonDataWrite();

	private static PhotonMessage messageData = new PhotonMessage();

	public static void AddMessage(string methodName, MessageDelegate callback)
	{
		messages.Add(methodName, callback);
	}

	public static bool Contains(string methodName)
	{
		return messages.ContainsKey(methodName);
	}

	public static PhotonDataWrite GetData()
	{
		data.Clear();
		return data;
	}

	public static void Clear()
	{
		messages.Clear();
	}

	public static void OnEventCall(byte code, byte[] content, int senderID)
	{
		if (eventCode == code)
		{
			ExecuteRpc(content, senderID);
		}
	}

	private static void InvokeMessage(string methodName, byte[] data, int senderID, int sendTime)
	{
		nProfiler.BeginSample("PhotonRPC.InvokeMessage");
		nProfiler.BeginSample("1");
		if (!messages.ContainsKey(methodName))
		{
			Debug.Log("No Find Method: " + methodName);
			return;
		}
		nProfiler.EndSample();
		nProfiler.BeginSample("2");
		messageData.SetData(data, senderID, sendTime);
		nProfiler.EndSample();
		nProfiler.BeginSample("3");
		messages[methodName](messageData);
		nProfiler.EndSample();
		nProfiler.EndSample();
	}

	private static void ExecuteRpc(byte[] bytes, int senderID = 0)
	{
		nProfiler.BeginSample("PhotonRPC.ExecuteRpc");
		if (bytes.Length == 0)
		{
			Debug.LogError("Malformed RPC; this should never occur.");
			return;
		}
		rpcData.Clear();
		rpcData.SetData(bytes);
		string empty = string.Empty;
		if (rpcData.ContainsKey(5))
		{
			byte @byte = rpcData.GetByte(5);
			if (@byte > PhotonNetwork.PhotonServerSettings.RpcList.Count - 1)
			{
				Debug.LogError("Could not find RPC with index: " + @byte + ". Going to ignore! Check PhotonServerSettings.RpcList");
				return;
			}
			empty = PhotonNetwork.PhotonServerSettings.RpcList[@byte];
		}
		else
		{
			empty = rpcData.GetString(3);
		}
		nProfiler.BeginSample("4");
		byte[] array = null;
		array = ((!rpcData.ContainsKey(4)) ? new byte[0] : rpcData.GetBytes(4));
		nProfiler.EndSample();
		if (string.IsNullOrEmpty(empty))
		{
			Debug.LogError("Malformed RPC; this should never occur.");
			return;
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			Debug.Log("Received RPC: " + empty);
		}
		nProfiler.BeginSample("6");
		int @int = rpcData.GetInt(2);
		nProfiler.EndSample();
		InvokeMessage(empty, array, senderID, @int);
		nProfiler.EndSample();
	}

	public static void RPC(string methodName, PhotonTargets target)
	{
		RPC(methodName, target, null, null);
	}

	public static void RPC(string methodName, PhotonTargets target, byte[] data)
	{
		RPC(methodName, target, null, data);
	}

	public static void RPC(string methodName, PhotonTargets target, int value)
	{
		PhotonDataWrite photonDataWrite = GetData();
		photonDataWrite.Write(value);
		RPC(methodName, target, null, photonDataWrite.ToArray());
	}

	public static void RPC(string methodName, PhotonTargets target, PhotonDataWrite data)
	{
		RPC(methodName, target, null, data.ToArray());
	}

	public static void RPC(string methodName, PhotonPlayer targetPlayer, byte[] data)
	{
		RPC(methodName, PhotonTargets.Others, targetPlayer, data);
	}

	public static void RPC(string methodName, PhotonPlayer targetPlayer, PhotonDataWrite data)
	{
		RPC(methodName, PhotonTargets.Others, targetPlayer, data.ToArray());
	}

	public static void RPC(string methodName, PhotonPlayer targetPlayer)
	{
		RPC(methodName, PhotonTargets.Others, targetPlayer, null);
	}

	private static void RPC(string methodName, PhotonTargets target, PhotonPlayer player, byte[] data)
	{
		nProfiler.BeginSample("PhotonPRC.RPC");
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			Debug.Log(string.Concat("Sending RPC \"", methodName, "\" to target: ", target, " or player:", player, "."));
		}
		rpcEvent.Clear();
		rpcEvent.Add(2, PhotonNetwork.ServerTimestamp);
		bool flag = false;
		for (int i = 0; i < PhotonNetwork.networkingPeer.rpcShortcuts.size; i++)
		{
			if (PhotonNetwork.networkingPeer.rpcShortcuts[i] == methodName)
			{
				rpcEvent.Add(5, (byte)i);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			rpcEvent.Add(3, methodName);
		}
		if (data != null && data.Length > 0)
		{
			rpcEvent.Add(4, data);
		}
		if (player != null)
		{
			if (PhotonNetwork.networkingPeer.LocalPlayer.ID == player.ID)
			{
				ExecuteRpc(rpcEvent.ToArray(), player.ID);
				return;
			}
			optionsRpcEvent.Reset();
			optionsRpcEvent.TargetActors = new int[1] { player.ID };
			PhotonNetwork.networkingPeer.OpRaiseEvent(eventCode, rpcEvent.ToArray(), true, optionsRpcEvent);
			return;
		}
		nProfiler.BeginSample("6");
		switch (target)
		{
		case PhotonTargets.All:
			optionsRpcEvent.Reset();
			rpcEventBytes = rpcEvent.ToArray();
			PhotonNetwork.networkingPeer.OpRaiseEvent(eventCode, rpcEventBytes, true, optionsRpcEvent);
			ExecuteRpc(rpcEventBytes, PhotonNetwork.networkingPeer.LocalPlayer.ID);
			break;
		case PhotonTargets.Others:
			optionsRpcEvent.Reset();
			PhotonNetwork.networkingPeer.OpRaiseEvent(eventCode, rpcEvent.ToArray(), true, optionsRpcEvent);
			break;
		case PhotonTargets.AllBuffered:
			optionsRpcEvent.Reset();
			optionsRpcEvent.CachingOption = EventCaching.AddToRoomCache;
			rpcEventBytes = rpcEvent.ToArray();
			PhotonNetwork.networkingPeer.OpRaiseEvent(eventCode, rpcEventBytes, true, optionsRpcEvent);
			ExecuteRpc(rpcEventBytes, PhotonNetwork.networkingPeer.LocalPlayer.ID);
			break;
		case PhotonTargets.OthersBuffered:
			optionsRpcEvent.Reset();
			optionsRpcEvent.CachingOption = EventCaching.AddToRoomCache;
			PhotonNetwork.networkingPeer.OpRaiseEvent(eventCode, rpcEvent.ToArray(), true, optionsRpcEvent);
			break;
		case PhotonTargets.MasterClient:
			if (PhotonNetwork.networkingPeer.mMasterClientId == PhotonNetwork.networkingPeer.LocalPlayer.ID)
			{
				ExecuteRpc(rpcEvent.ToArray(), PhotonNetwork.networkingPeer.LocalPlayer.ID);
				break;
			}
			optionsRpcEvent.Reset();
			optionsRpcEvent.Receivers = ReceiverGroup.MasterClient;
			PhotonNetwork.networkingPeer.OpRaiseEvent(eventCode, rpcEvent.ToArray(), true, optionsRpcEvent);
			break;
		case PhotonTargets.AllViaServer:
			optionsRpcEvent.Reset();
			optionsRpcEvent.Receivers = ReceiverGroup.All;
			PhotonNetwork.networkingPeer.OpRaiseEvent(eventCode, rpcEvent.ToArray(), true, optionsRpcEvent);
			if (PhotonNetwork.offlineMode)
			{
				ExecuteRpc(rpcEvent.ToArray(), PhotonNetwork.networkingPeer.LocalPlayer.ID);
			}
			break;
		case PhotonTargets.AllBufferedViaServer:
			optionsRpcEvent.Reset();
			optionsRpcEvent.Receivers = ReceiverGroup.All;
			optionsRpcEvent.CachingOption = EventCaching.AddToRoomCache;
			PhotonNetwork.networkingPeer.OpRaiseEvent(eventCode, rpcEvent.ToArray(), true, optionsRpcEvent);
			if (PhotonNetwork.offlineMode)
			{
				ExecuteRpc(rpcEvent.ToArray(), PhotonNetwork.networkingPeer.LocalPlayer.ID);
			}
			break;
		default:
			Debug.LogError("Unsupported target enum: " + target);
			break;
		}
		nProfiler.EndSample();
		nProfiler.EndSample();
	}
}
