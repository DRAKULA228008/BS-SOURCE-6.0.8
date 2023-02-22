using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Photon.SocketServer.Security;

namespace ExitGames.Client.Photon
{
	public abstract class PeerBase
	{
		internal delegate void MyAction();

		public enum ConnectionStateValue : byte
		{
			Disconnected = 0,
			Connecting = 1,
			Connected = 3,
			Disconnecting = 4,
			AcknowledgingDisconnect = 5,
			Zombie = 6
		}

		internal enum EgMessageType : byte
		{
			Init = 0,
			InitResponse = 1,
			Operation = 2,
			OperationResponse = 3,
			Event = 4,
			InternalOperationRequest = 6,
			InternalOperationResponse = 7,
			Message = 8,
			RawMessage = 9
		}

		internal PhotonPeer ppeer;

		internal IProtocol protocol;

		internal ConnectionProtocol usedProtocol;

		internal IPhotonSocket rt;

		internal int ByteCountLastOperation;

		internal int ByteCountCurrentDispatch;

		internal NCommand CommandInCurrentDispatch;

		internal int TrafficPackageHeaderSize;

		internal int packetLossByCrc;

		internal int packetLossByChallenge;

		internal readonly Queue<MyAction> ActionQueue = new Queue<MyAction>();

		internal short peerID = -1;

		internal ConnectionStateValue peerConnectionState;

		internal int serverTimeOffset;

		internal bool serverTimeOffsetIsAvailable;

		internal int roundTripTime;

		internal int roundTripTimeVariance;

		internal int lastRoundTripTime;

		internal int lowestRoundTripTime;

		internal int lastRoundTripTimeVariance;

		internal int highestRoundTripTimeVariance;

		internal int timestampOfLastReceive;

		internal int packetThrottleInterval;

		internal static short peerCount;

		internal long bytesOut;

		internal long bytesIn;

		internal int commandBufferSize = 100;

		internal ICryptoProvider CryptoProvider;

		private readonly Random lagRandomizer = new Random();

		internal readonly LinkedList<SimulationItem> NetSimListOutgoing = new LinkedList<SimulationItem>();

		internal readonly LinkedList<SimulationItem> NetSimListIncoming = new LinkedList<SimulationItem>();

		private readonly NetworkSimulationSet networkSimulationSettings = new NetworkSimulationSet();

		internal Queue<CmdLogItem> CommandLog;

		internal Queue<CmdLogItem> InReliableLog;

		internal object CustomInitData;

		internal string AppId;

		internal int timeBase;

		internal int timeInt;

		internal int timeoutInt;

		internal int timeLastAckReceive;

		internal int timeLastSendAck;

		internal int timeLastSendOutgoing;

		internal const int ENET_PEER_PACKET_LOSS_SCALE = 65536;

		internal const int ENET_PEER_DEFAULT_ROUND_TRIP_TIME = 300;

		internal const int ENET_PEER_PACKET_THROTTLE_INTERVAL = 5000;

		internal bool ApplicationIsInitialized;

		internal bool isEncryptionAvailable;

		internal int outgoingCommandsInStream = 0;

		protected StreamBuffer SerializeMemStream = new StreamBuffer();

		internal string ClientVersion
		{
			get
			{
				return ppeer.ClientVersion;
			}
		}

		internal Type SocketImplementation
		{
			get
			{
				return ppeer.SocketImplementation;
			}
		}

		public string ServerAddress { get; internal set; }

		internal string HttpUrlParameters { get; set; }

		internal bool TrafficStatsEnabled
		{
			get
			{
				return ppeer.TrafficStatsEnabled;
			}
		}

		internal TrafficStats TrafficStatsIncoming
		{
			get
			{
				return ppeer.TrafficStatsIncoming;
			}
		}

		internal TrafficStats TrafficStatsOutgoing
		{
			get
			{
				return ppeer.TrafficStatsOutgoing;
			}
		}

		internal TrafficStatsGameLevel TrafficStatsGameLevel
		{
			get
			{
				return ppeer.TrafficStatsGameLevel;
			}
		}

		internal bool crcEnabled
		{
			get
			{
				return ppeer.CrcEnabled;
			}
		}

		internal IPhotonPeerListener Listener
		{
			get
			{
				return ppeer.Listener;
			}
		}

		internal DebugLevel debugOut
		{
			get
			{
				return ppeer.DebugOut;
			}
		}

		internal int sentCountAllowance
		{
			get
			{
				return ppeer.SentCountAllowance;
			}
		}

		internal int DisconnectTimeout
		{
			get
			{
				return ppeer.DisconnectTimeout;
			}
		}

		internal int timePingInterval
		{
			get
			{
				return ppeer.TimePingInterval;
			}
		}

		internal byte ChannelCount
		{
			get
			{
				return ppeer.ChannelCount;
			}
		}

		internal int limitOfUnreliableCommands
		{
			get
			{
				return ppeer.LimitOfUnreliableCommands;
			}
		}

		public byte QuickResendAttempts
		{
			get
			{
				return ppeer.QuickResendAttempts;
			}
		}

		public NetworkSimulationSet NetworkSimulationSettings
		{
			get
			{
				return networkSimulationSettings;
			}
		}

		internal int CommandLogSize
		{
			get
			{
				return ppeer.CommandLogSize;
			}
		}

		internal long BytesOut
		{
			get
			{
				return bytesOut;
			}
		}

		internal long BytesIn
		{
			get
			{
				return bytesIn;
			}
		}

		internal abstract int QueuedIncomingCommandsCount { get; }

		internal abstract int QueuedOutgoingCommandsCount { get; }

		public virtual string PeerID
		{
			get
			{
				return ((ushort)peerID).ToString();
			}
		}

		protected internal byte[] TcpConnectionPrefix { get; set; }

		protected internal bool IsIpv6
		{
			get
			{
				return rt != null && rt.AddressResolvedAsIpv6;
			}
		}

		internal static int outgoingStreamBufferSize
		{
			get
			{
				return PhotonPeer.OutgoingStreamBufferSize;
			}
		}

		internal bool IsSendingOnlyAcks
		{
			get
			{
				return ppeer.IsSendingOnlyAcks;
			}
		}

		internal int mtu
		{
			get
			{
				return ppeer.MaximumTransferUnit;
			}
		}

		internal int rhttpMinConnections
		{
			get
			{
				return ppeer.RhttpMinConnections;
			}
		}

		internal int rhttpMaxConnections
		{
			get
			{
				return ppeer.RhttpMaxConnections;
			}
		}

		internal void CommandLogResize()
		{
			if (CommandLogSize <= 0)
			{
				CommandLog = null;
				InReliableLog = null;
				return;
			}
			if (CommandLog == null || InReliableLog == null)
			{
				CommandLogInit();
			}
			while (CommandLog.Count > 0 && CommandLog.Count > CommandLogSize)
			{
				CommandLog.Dequeue();
			}
			while (InReliableLog.Count > 0 && InReliableLog.Count > CommandLogSize)
			{
				InReliableLog.Dequeue();
			}
		}

		internal void CommandLogInit()
		{
			if (CommandLogSize <= 0)
			{
				CommandLog = null;
				InReliableLog = null;
			}
			else if (CommandLog == null || InReliableLog == null)
			{
				CommandLog = new Queue<CmdLogItem>(CommandLogSize);
				InReliableLog = new Queue<CmdLogItem>(CommandLogSize);
			}
			else
			{
				CommandLog.Clear();
				InReliableLog.Clear();
			}
		}

		public string CommandLogToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = ((usedProtocol == ConnectionProtocol.Udp) ? ((EnetPeer)this).reliableCommandsRepeated : 0);
			stringBuilder.AppendFormat("PeerId: {0} Now: {1} Server: {2} State: {3} Total Resends: {4} Received {5}ms ago.\n", PeerID, timeInt, ServerAddress, peerConnectionState, num, SupportClass.GetTickCount() - timestampOfLastReceive);
			if (CommandLog == null)
			{
				return stringBuilder.ToString();
			}
			foreach (CmdLogItem item in CommandLog)
			{
				stringBuilder.AppendLine(item.ToString());
			}
			stringBuilder.AppendLine("Received Reliable Log: ");
			foreach (CmdLogItem item2 in InReliableLog)
			{
				stringBuilder.AppendLine(item2.ToString());
			}
			return stringBuilder.ToString();
		}

		internal void InitOnce()
		{
			networkSimulationSettings.peerBase = this;
		}

		internal abstract bool Connect(string serverAddress, string appID, object customData = null);

		public abstract void OnConnect();

		private string GetHttpKeyValueString(Dictionary<string, string> dic)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> item in dic)
			{
				stringBuilder.Append(item.Key).Append("=").Append(item.Value)
					.Append("&");
			}
			return stringBuilder.ToString();
		}

		[Obsolete("A IPhotonSocket should set AddressResolvedAsIpv6 as soon as the server's address is resolved and before calling OnConnect.")]
		public virtual void SetInitIPV6Bit(bool isV6)
		{
			if (debugOut == DebugLevel.ALL)
			{
				Listener.DebugReturn(DebugLevel.ALL, "Setting IPv6 bit " + isV6);
			}
			rt.AddressResolvedAsIpv6 = isV6;
		}

		internal byte[] PrepareConnectData(string serverAddress, string appID, object custom)
		{
			if (rt == null || !rt.Connected)
			{
				EnqueueDebugReturn(DebugLevel.WARNING, "The peer attempts to prepare an Init-Request but the socket is not connected!?");
			}
			if (custom == null)
			{
				byte[] array = new byte[41];
				byte[] clientVersion = Version.clientVersion;
				array[0] = 243;
				array[1] = 0;
				array[2] = protocol.VersionBytes[0];
				array[3] = protocol.VersionBytes[1];
				array[4] = ppeer.ClientSdkIdShifted;
				array[5] = (byte)((byte)(clientVersion[0] << 4) | clientVersion[1]);
				array[6] = clientVersion[2];
				array[7] = clientVersion[3];
				array[8] = 0;
				if (string.IsNullOrEmpty(appID))
				{
					appID = "LoadBalancing";
				}
				for (int i = 0; i < 32; i++)
				{
					array[i + 9] = (byte)((i < appID.Length) ? ((byte)appID[i]) : 0);
				}
				if (IsIpv6)
				{
					array[5] |= 128;
				}
				else
				{
					array[5] &= 127;
				}
				return array;
			}
			if (custom != null)
			{
				byte[] array2 = null;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["init"] = null;
				dictionary["app"] = appID;
				dictionary["clientversion"] = ClientVersion;
				dictionary["protocol"] = protocol.protocolType;
				dictionary["sid"] = ppeer.ClientSdkIdShifted.ToString();
				byte[] array3 = null;
				int num = 0;
				if (custom != null)
				{
					array3 = protocol.Serialize(custom);
					num += array3.Length;
				}
				string text = GetHttpKeyValueString(dictionary);
				if (IsIpv6)
				{
					text += "&IPv6";
				}
				string text2 = string.Format("POST /?{0} HTTP/1.1\r\nHost: {1}\r\nContent-Length: {2}\r\n\r\n", text, serverAddress, num);
				array2 = new byte[text2.Length + num];
				if (array3 != null)
				{
					Buffer.BlockCopy(array3, 0, array2, text2.Length, array3.Length);
				}
				Buffer.BlockCopy(Encoding.UTF8.GetBytes(text2), 0, array2, 0, text2.Length);
				return array2;
			}
			return null;
		}

		internal string PepareWebSocketUrl(string serverAddress, string appId, object customData)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			string empty = string.Empty;
			if (customData != null)
			{
				byte[] array = protocol.Serialize(customData);
				if (array == null)
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "Can not deserialize custom data");
					return null;
				}
			}
			stringBuilder.AppendFormat("app={0}&clientver={1}&sid={2}&{3}&initobj={4}", appId, ClientVersion, ppeer.ClientSdkIdShifted, IsIpv6 ? "IPv6" : string.Empty, empty);
			return stringBuilder.ToString();
		}

		internal abstract void Disconnect();

		internal abstract void StopConnection();

		internal abstract void FetchServerTimestamp();

		internal bool EnqueueOperation(Dictionary<byte, object> parameters, byte opCode, bool sendReliable, byte channelId, bool encrypted)
		{
			return EnqueueOperation(parameters, opCode, sendReliable, channelId, encrypted, EgMessageType.Operation);
		}

		internal abstract bool EnqueueOperation(Dictionary<byte, object> parameters, byte opCode, bool sendReliable, byte channelId, bool encrypted, EgMessageType messageType);

		internal abstract bool DispatchIncomingCommands();

		internal abstract bool SendOutgoingCommands();

		internal virtual bool SendAcksOnly()
		{
			return false;
		}

		internal byte[] SerializeMessageToMessage(object message, bool encrypt, byte[] messageHeader, bool writeLength = true)
		{
			byte[] array3;
			lock (SerializeMemStream)
			{
				SerializeMemStream.SetLength(0L);
				if (!encrypt)
				{
					SerializeMemStream.Write(messageHeader, 0, messageHeader.Length);
				}
				if (message is byte[])
				{
					byte[] array = message as byte[];
					SerializeMemStream.Write(array, 0, array.Length);
				}
				else
				{
					protocol.SerializeMessage(SerializeMemStream, message);
				}
				if (encrypt)
				{
					byte[] array2 = CryptoProvider.Encrypt(SerializeMemStream.GetBuffer(), 0, (int)SerializeMemStream.Length);
					SerializeMemStream.SetLength(0L);
					SerializeMemStream.Write(messageHeader, 0, messageHeader.Length);
					SerializeMemStream.Write(array2, 0, array2.Length);
				}
				array3 = SerializeMemStream.ToArray();
			}
			array3[messageHeader.Length - 1] = (byte)((message is byte[]) ? 9 : 8);
			if (encrypt)
			{
				array3[messageHeader.Length - 1] = (byte)(array3[messageHeader.Length - 1] | 0x80u);
			}
			if (writeLength)
			{
				int targetOffset = 1;
				Protocol.Serialize(array3.Length, array3, ref targetOffset);
			}
			return array3;
		}

		internal abstract byte[] SerializeOperationToMessage(byte opCode, Dictionary<byte, object> parameters, EgMessageType messageType, bool encrypt);

		internal abstract void ReceiveIncomingCommands(byte[] inBuff, int dataLength);

		internal void InitCallback()
		{
			if (peerConnectionState == ConnectionStateValue.Connecting)
			{
				peerConnectionState = ConnectionStateValue.Connected;
			}
			ApplicationIsInitialized = true;
			FetchServerTimestamp();
			Listener.OnStatusChanged(StatusCode.Connect);
		}

		internal bool ExchangeKeysForEncryption(object lockObject)
		{
			isEncryptionAvailable = false;
			if (CryptoProvider != null)
			{
				CryptoProvider.Dispose();
				CryptoProvider = null;
			}
			if (CryptoProvider == null)
			{
				CryptoProvider = new DiffieHellmanCryptoProvider();
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>(1);
			dictionary[PhotonCodes.ClientKey] = CryptoProvider.PublicKey;
			if (lockObject != null)
			{
				lock (lockObject)
				{
					return EnqueueOperation(dictionary, PhotonCodes.InitEncryption, true, 0, false, EgMessageType.InternalOperationRequest);
				}
			}
			return EnqueueOperation(dictionary, PhotonCodes.InitEncryption, true, 0, false, EgMessageType.InternalOperationRequest);
		}

		internal void DeriveSharedKey(OperationResponse operationResponse)
		{
			if (operationResponse.ReturnCode != 0)
			{
				EnqueueDebugReturn(DebugLevel.ERROR, "Establishing encryption keys failed. " + operationResponse.ToStringFull());
				EnqueueStatusCallback(StatusCode.EncryptionFailedToEstablish);
				return;
			}
			byte[] array = (byte[])operationResponse[PhotonCodes.ServerKey];
			if (array == null || array.Length == 0)
			{
				EnqueueDebugReturn(DebugLevel.ERROR, "Establishing encryption keys failed. Server's public key is null or empty. " + operationResponse.ToStringFull());
				EnqueueStatusCallback(StatusCode.EncryptionFailedToEstablish);
			}
			else
			{
				CryptoProvider.DeriveSharedKey(array);
				isEncryptionAvailable = true;
				EnqueueStatusCallback(StatusCode.EncryptionEstablished);
			}
		}

		internal void EnqueueActionForDispatch(MyAction action)
		{
			lock (ActionQueue)
			{
				ActionQueue.Enqueue(action);
			}
		}

		internal void EnqueueDebugReturn(DebugLevel level, string debugReturn)
		{
			lock (ActionQueue)
			{
				ActionQueue.Enqueue(delegate
				{
					Listener.DebugReturn(level, debugReturn);
				});
			}
		}

		internal void EnqueueStatusCallback(StatusCode statusValue)
		{
			lock (ActionQueue)
			{
				ActionQueue.Enqueue(delegate
				{
					Listener.OnStatusChanged(statusValue);
				});
			}
		}

		internal virtual void InitPeerBase()
		{
			protocol = SerializationProtocolFactory.Create(ppeer.SerializationProtocolType);
			ppeer.InitializeTrafficStats();
			ByteCountLastOperation = 0;
			ByteCountCurrentDispatch = 0;
			bytesIn = 0L;
			bytesOut = 0L;
			packetLossByCrc = 0;
			packetLossByChallenge = 0;
			networkSimulationSettings.LostPackagesIn = 0;
			networkSimulationSettings.LostPackagesOut = 0;
			lock (NetSimListOutgoing)
			{
				NetSimListOutgoing.Clear();
			}
			lock (NetSimListIncoming)
			{
				NetSimListIncoming.Clear();
			}
			peerConnectionState = ConnectionStateValue.Disconnected;
			timeBase = SupportClass.GetTickCount();
			isEncryptionAvailable = false;
			ApplicationIsInitialized = false;
			roundTripTime = 300;
			roundTripTimeVariance = 0;
			packetThrottleInterval = 5000;
			serverTimeOffsetIsAvailable = false;
			serverTimeOffset = 0;
		}

		internal virtual bool DeserializeMessageAndCallback(byte[] inBuff)
		{
			if (inBuff.Length < 2)
			{
				if ((int)debugOut >= 1)
				{
					Listener.DebugReturn(DebugLevel.ERROR, "Incoming UDP data too short! " + inBuff.Length);
				}
				return false;
			}
			if (inBuff[0] != 243 && inBuff[0] != 253)
			{
				if ((int)debugOut >= 1)
				{
					Listener.DebugReturn(DebugLevel.ALL, "No regular operation UDP message: " + inBuff[0]);
				}
				return false;
			}
			byte b = (byte)(inBuff[1] & 0x7Fu);
			bool flag = (inBuff[1] & 0x80) > 0;
			StreamBuffer streamBuffer = null;
			if (b != 1)
			{
				try
				{
					if (flag)
					{
						inBuff = CryptoProvider.Decrypt(inBuff, 2, inBuff.Length - 2);
						streamBuffer = new StreamBuffer(inBuff);
					}
					else
					{
						streamBuffer = new StreamBuffer(inBuff);
						streamBuffer.Seek(2L, SeekOrigin.Begin);
					}
				}
				catch (Exception ex)
				{
					if ((int)debugOut >= 1)
					{
						Listener.DebugReturn(DebugLevel.ERROR, ex.ToString());
					}
					SupportClass.WriteStackTrace(ex);
					return false;
				}
			}
			int num = 0;
			switch (b)
			{
			case 3:
			{
				OperationResponse operationResponse = protocol.DeserializeOperationResponse(streamBuffer);
				if (TrafficStatsEnabled)
				{
					TrafficStatsGameLevel.CountResult(ByteCountCurrentDispatch);
					num = SupportClass.GetTickCount();
				}
				Listener.OnOperationResponse(operationResponse);
				if (TrafficStatsEnabled)
				{
					TrafficStatsGameLevel.TimeForResponseCallback(operationResponse.OperationCode, SupportClass.GetTickCount() - num);
				}
				break;
			}
			case 4:
			{
				EventData eventData = protocol.DeserializeEventData(streamBuffer);
				if (TrafficStatsEnabled)
				{
					TrafficStatsGameLevel.CountEvent(ByteCountCurrentDispatch);
					num = SupportClass.GetTickCount();
				}
				Listener.OnEvent(eventData);
				if (TrafficStatsEnabled)
				{
					TrafficStatsGameLevel.TimeForEventCallback(eventData.Code, SupportClass.GetTickCount() - num);
				}
				break;
			}
			case 1:
				InitCallback();
				break;
			case 7:
			{
				OperationResponse operationResponse = protocol.DeserializeOperationResponse(streamBuffer);
				if (TrafficStatsEnabled)
				{
					TrafficStatsGameLevel.CountResult(ByteCountCurrentDispatch);
					num = SupportClass.GetTickCount();
				}
				if (operationResponse.OperationCode == PhotonCodes.InitEncryption)
				{
					DeriveSharedKey(operationResponse);
				}
				else if (operationResponse.OperationCode == PhotonCodes.Ping)
				{
					TPeer tPeer = this as TPeer;
					if (tPeer != null)
					{
						tPeer.ReadPingResult(operationResponse);
					}
				}
				else
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "Received unknown internal operation. " + operationResponse.ToStringFull());
				}
				if (TrafficStatsEnabled)
				{
					TrafficStatsGameLevel.TimeForResponseCallback(operationResponse.OperationCode, SupportClass.GetTickCount() - num);
				}
				break;
			}
			default:
				EnqueueDebugReturn(DebugLevel.ERROR, "unexpected msgType " + b);
				break;
			}
			return true;
		}

		internal void SendNetworkSimulated(byte[] dataToSend)
		{
			if (!NetworkSimulationSettings.IsSimulationEnabled)
			{
				throw new NotImplementedException("SendNetworkSimulated was called, despite NetworkSimulationSettings.IsSimulationEnabled == false.");
			}
			if (usedProtocol == ConnectionProtocol.Udp && NetworkSimulationSettings.OutgoingLossPercentage > 0 && lagRandomizer.Next(101) < NetworkSimulationSettings.OutgoingLossPercentage)
			{
				networkSimulationSettings.LostPackagesOut++;
				return;
			}
			int num = ((networkSimulationSettings.OutgoingJitter > 0) ? (lagRandomizer.Next(networkSimulationSettings.OutgoingJitter * 2) - networkSimulationSettings.OutgoingJitter) : 0);
			int num2 = networkSimulationSettings.OutgoingLag + num;
			int num3 = SupportClass.GetTickCount() + num2;
			SimulationItem value = new SimulationItem
			{
				DelayedData = dataToSend,
				TimeToExecute = num3,
				Delay = num2
			};
			lock (NetSimListOutgoing)
			{
				if (NetSimListOutgoing.Count == 0 || usedProtocol == ConnectionProtocol.Tcp)
				{
					NetSimListOutgoing.AddLast(value);
					return;
				}
				LinkedListNode<SimulationItem> linkedListNode = NetSimListOutgoing.First;
				while (linkedListNode != null && linkedListNode.Value.TimeToExecute < num3)
				{
					linkedListNode = linkedListNode.Next;
				}
				if (linkedListNode == null)
				{
					NetSimListOutgoing.AddLast(value);
				}
				else
				{
					NetSimListOutgoing.AddBefore(linkedListNode, value);
				}
			}
		}

		internal void ReceiveNetworkSimulated(byte[] dataReceived)
		{
			if (!networkSimulationSettings.IsSimulationEnabled)
			{
				throw new NotImplementedException("ReceiveNetworkSimulated was called, despite NetworkSimulationSettings.IsSimulationEnabled == false.");
			}
			if (usedProtocol == ConnectionProtocol.Udp && networkSimulationSettings.IncomingLossPercentage > 0 && lagRandomizer.Next(101) < networkSimulationSettings.IncomingLossPercentage)
			{
				networkSimulationSettings.LostPackagesIn++;
				return;
			}
			int num = ((networkSimulationSettings.IncomingJitter > 0) ? (lagRandomizer.Next(networkSimulationSettings.IncomingJitter * 2) - networkSimulationSettings.IncomingJitter) : 0);
			int num2 = networkSimulationSettings.IncomingLag + num;
			int num3 = SupportClass.GetTickCount() + num2;
			SimulationItem value = new SimulationItem
			{
				DelayedData = dataReceived,
				TimeToExecute = num3,
				Delay = num2
			};
			lock (NetSimListIncoming)
			{
				if (NetSimListIncoming.Count == 0 || usedProtocol == ConnectionProtocol.Tcp)
				{
					NetSimListIncoming.AddLast(value);
					return;
				}
				LinkedListNode<SimulationItem> linkedListNode = NetSimListIncoming.First;
				while (linkedListNode != null && linkedListNode.Value.TimeToExecute < num3)
				{
					linkedListNode = linkedListNode.Next;
				}
				if (linkedListNode == null)
				{
					NetSimListIncoming.AddLast(value);
				}
				else
				{
					NetSimListIncoming.AddBefore(linkedListNode, value);
				}
			}
		}

		protected internal void NetworkSimRun()
		{
			while (true)
			{
				bool flag = false;
				lock (networkSimulationSettings.NetSimManualResetEvent)
				{
					flag = networkSimulationSettings.IsSimulationEnabled;
				}
				if (!flag)
				{
					networkSimulationSettings.NetSimManualResetEvent.WaitOne();
					continue;
				}
				lock (NetSimListIncoming)
				{
					SimulationItem simulationItem = null;
					while (NetSimListIncoming.First != null)
					{
						simulationItem = NetSimListIncoming.First.Value;
						if (simulationItem.stopw.ElapsedMilliseconds < simulationItem.Delay)
						{
							break;
						}
						if (rt != null && rt.Connected)
						{
							ReceiveIncomingCommands(simulationItem.DelayedData, simulationItem.DelayedData.Length);
						}
						NetSimListIncoming.RemoveFirst();
					}
				}
				lock (NetSimListOutgoing)
				{
					SimulationItem simulationItem2 = null;
					while (NetSimListOutgoing.First != null)
					{
						simulationItem2 = NetSimListOutgoing.First.Value;
						if (simulationItem2.stopw.ElapsedMilliseconds < simulationItem2.Delay)
						{
							break;
						}
						if (rt != null && rt.Connected)
						{
							rt.Send(simulationItem2.DelayedData, simulationItem2.DelayedData.Length);
						}
						NetSimListOutgoing.RemoveFirst();
					}
				}
				Thread.Sleep(0);
			}
		}

		internal void UpdateRoundTripTimeAndVariance(int lastRoundtripTime)
		{
			if (lastRoundtripTime >= 0)
			{
				roundTripTimeVariance -= roundTripTimeVariance / 4;
				if (lastRoundtripTime >= roundTripTime)
				{
					roundTripTime += (lastRoundtripTime - roundTripTime) / 8;
					roundTripTimeVariance += (lastRoundtripTime - roundTripTime) / 4;
				}
				else
				{
					roundTripTime += (lastRoundtripTime - roundTripTime) / 8;
					roundTripTimeVariance -= (lastRoundtripTime - roundTripTime) / 4;
				}
				if (roundTripTime < lowestRoundTripTime)
				{
					lowestRoundTripTime = roundTripTime;
				}
				if (roundTripTimeVariance > highestRoundTripTimeVariance)
				{
					highestRoundTripTimeVariance = roundTripTimeVariance;
				}
			}
		}

		internal virtual bool InitUdpEncryption(byte[] encryptionSecret, byte[] hmacSecret)
		{
			return false;
		}

		internal virtual void InitEncryption(byte[] secret)
		{
			CryptoProvider = new DiffieHellmanCryptoProvider(secret);
			isEncryptionAvailable = true;
		}
	}
}
