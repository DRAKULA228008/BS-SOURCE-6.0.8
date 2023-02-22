using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExitGames.Client.Photon.EncryptorManaged;

namespace ExitGames.Client.Photon
{
	public class PhotonPeer
	{
		public const bool NoSocket = false;

		public const bool NativeDatagramEncrypt = false;

		public const bool DebugBuild = true;

		protected internal byte ClientSdkId = 15;

		public static bool AsyncKeyExchange = false;

		private string clientVersion;

		public Dictionary<ConnectionProtocol, Type> SocketImplementationConfig;

		public DebugLevel DebugOut = DebugLevel.ERROR;

		private Stopwatch trafficStatsStopwatch;

		private bool trafficStatsEnabled = false;

		private int commandLogSize;

		private byte quickResendAttempts;

		public int RhttpMinConnections = 2;

		public int RhttpMaxConnections = 6;

		public byte ChannelCount = 2;

		private bool crcEnabled;

		[Obsolete("Check QueuedOutgoingCommands and QueuedIncomingCommands on demand instead.")]
		public int WarningSize;

		public int SentCountAllowance = 7;

		public int TimePingInterval = 1000;

		public int DisconnectTimeout = 10000;

		public static int OutgoingStreamBufferSize = 1200;

		private int mtu = 1200;

		internal PeerBase peerBase;

		private readonly object SendOutgoingLockObject = new object();

		private readonly object DispatchLockObject = new object();

		private readonly object EnqueueLock = new object();

		protected internal byte[] PayloadEncryptionSecret;

		internal Encryptor encryptor;

		internal Decryptor decryptor;

		protected internal byte ClientSdkIdShifted
		{
			get
			{
				byte b = 0;
				return (byte)((ClientSdkId << 1) | b);
			}
		}

		public string ClientVersion
		{
			get
			{
				if (string.IsNullOrEmpty(clientVersion))
				{
					clientVersion = string.Format("{0}.{1}.{2}.{3}", Version.clientVersion[0], Version.clientVersion[1], Version.clientVersion[2], Version.clientVersion[3]);
				}
				return clientVersion;
			}
		}

		public SerializationProtocol SerializationProtocolType { get; set; }

		public Type SocketImplementation { get; internal set; }

		public IPhotonPeerListener Listener { get; protected set; }

		public long BytesIn
		{
			get
			{
				return peerBase.BytesIn;
			}
		}

		public long BytesOut
		{
			get
			{
				return peerBase.BytesOut;
			}
		}

		public int ByteCountCurrentDispatch
		{
			get
			{
				return peerBase.ByteCountCurrentDispatch;
			}
		}

		public string CommandInfoCurrentDispatch
		{
			get
			{
				return (peerBase.CommandInCurrentDispatch != null) ? peerBase.CommandInCurrentDispatch.ToString() : string.Empty;
			}
		}

		public int ByteCountLastOperation
		{
			get
			{
				return peerBase.ByteCountLastOperation;
			}
		}

		public TrafficStats TrafficStatsIncoming { get; internal set; }

		public TrafficStats TrafficStatsOutgoing { get; internal set; }

		public TrafficStatsGameLevel TrafficStatsGameLevel { get; internal set; }

		public long TrafficStatsElapsedMs
		{
			get
			{
				return (trafficStatsStopwatch != null) ? trafficStatsStopwatch.ElapsedMilliseconds : 0;
			}
		}

		public bool TrafficStatsEnabled
		{
			get
			{
				return trafficStatsEnabled;
			}
			set
			{
				if (trafficStatsEnabled == value)
				{
					return;
				}
				trafficStatsEnabled = value;
				if (value)
				{
					if (trafficStatsStopwatch == null)
					{
						InitializeTrafficStats();
					}
					trafficStatsStopwatch.Start();
				}
				else if (trafficStatsStopwatch != null)
				{
					trafficStatsStopwatch.Stop();
				}
			}
		}

		public int CommandLogSize
		{
			get
			{
				return commandLogSize;
			}
			set
			{
				commandLogSize = value;
				peerBase.CommandLogResize();
			}
		}

		public bool EnableServerTracing { get; set; }

		public byte QuickResendAttempts
		{
			get
			{
				return quickResendAttempts;
			}
			set
			{
				quickResendAttempts = value;
				if (quickResendAttempts > 4)
				{
					quickResendAttempts = 4;
				}
			}
		}

		public PeerStateValue PeerState
		{
			get
			{
				if (peerBase.peerConnectionState == PeerBase.ConnectionStateValue.Connected && !peerBase.ApplicationIsInitialized)
				{
					return PeerStateValue.InitializingApplication;
				}
				return (PeerStateValue)peerBase.peerConnectionState;
			}
		}

		public string PeerID
		{
			get
			{
				return peerBase.PeerID;
			}
		}

		public int CommandBufferSize
		{
			get
			{
				return peerBase.commandBufferSize;
			}
		}

		public int LimitOfUnreliableCommands { get; set; }

		public int QueuedIncomingCommands
		{
			get
			{
				return peerBase.QueuedIncomingCommandsCount;
			}
		}

		public int QueuedOutgoingCommands
		{
			get
			{
				return peerBase.QueuedOutgoingCommandsCount;
			}
		}

		public bool CrcEnabled
		{
			get
			{
				return crcEnabled;
			}
			set
			{
				if (crcEnabled != value)
				{
					if (peerBase.peerConnectionState != 0)
					{
						throw new Exception("CrcEnabled can only be set while disconnected.");
					}
					crcEnabled = value;
				}
			}
		}

		public int PacketLossByCrc
		{
			get
			{
				return peerBase.packetLossByCrc;
			}
		}

		public int PacketLossByChallenge
		{
			get
			{
				return peerBase.packetLossByChallenge;
			}
		}

		public int ResentReliableCommands
		{
			get
			{
				return (UsedProtocol == ConnectionProtocol.Udp) ? ((EnetPeer)peerBase).reliableCommandsRepeated : 0;
			}
		}

		public int ServerTimeInMilliSeconds
		{
			get
			{
				return peerBase.serverTimeOffsetIsAvailable ? (peerBase.serverTimeOffset + SupportClass.GetTickCount()) : 0;
			}
		}

		public int ConnectionTime
		{
			get
			{
				return peerBase.timeInt;
			}
		}

		public int LastSendAckTime
		{
			get
			{
				return peerBase.timeLastSendAck;
			}
		}

		public int LastSendOutgoingTime
		{
			get
			{
				return peerBase.timeLastSendOutgoing;
			}
		}

		[Obsolete("Should be replaced by: SupportClass.GetTickCount(). Internally this is used, too.")]
		public int LocalTimeInMilliSeconds
		{
			get
			{
				return SupportClass.GetTickCount();
			}
		}

		public SupportClass.IntegerMillisecondsDelegate LocalMsTimestampDelegate
		{
			set
			{
				if (PeerState != 0)
				{
					throw new Exception("LocalMsTimestampDelegate only settable while disconnected. State: " + PeerState);
				}
				SupportClass.IntegerMilliseconds = value;
			}
		}

		public int RoundTripTime
		{
			get
			{
				return peerBase.roundTripTime;
			}
		}

		public int RoundTripTimeVariance
		{
			get
			{
				return peerBase.roundTripTimeVariance;
			}
		}

		public int TimestampOfLastSocketReceive
		{
			get
			{
				return peerBase.timestampOfLastReceive;
			}
		}

		public string ServerAddress
		{
			get
			{
				return peerBase.ServerAddress;
			}
			set
			{
				if ((int)DebugOut >= 1)
				{
					Listener.DebugReturn(DebugLevel.ERROR, "Failed to set ServerAddress. Can be set only when using HTTP.");
				}
			}
		}

		public ConnectionProtocol UsedProtocol
		{
			get
			{
				return peerBase.usedProtocol;
			}
		}

		public ConnectionProtocol TransportProtocol { get; set; }

		public virtual bool IsSimulationEnabled
		{
			get
			{
				return NetworkSimulationSettings.IsSimulationEnabled;
			}
			set
			{
				if (value != NetworkSimulationSettings.IsSimulationEnabled)
				{
					lock (SendOutgoingLockObject)
					{
						NetworkSimulationSettings.IsSimulationEnabled = value;
					}
				}
			}
		}

		public NetworkSimulationSet NetworkSimulationSettings
		{
			get
			{
				return peerBase.NetworkSimulationSettings;
			}
		}

		public int MaximumTransferUnit
		{
			get
			{
				return mtu;
			}
			set
			{
				if (PeerState != 0)
				{
					throw new Exception("MaximumTransferUnit is only settable while disconnected. State: " + PeerState);
				}
				if (value < 576)
				{
					value = 576;
				}
				mtu = value;
			}
		}

		public bool IsEncryptionAvailable
		{
			get
			{
				return peerBase.isEncryptionAvailable;
			}
		}

		public bool IsSendingOnlyAcks { get; set; }

		public void TrafficStatsReset()
		{
			TrafficStatsEnabled = false;
			InitializeTrafficStats();
			TrafficStatsEnabled = true;
		}

		internal void InitializeTrafficStats()
		{
			TrafficStatsIncoming = new TrafficStats(peerBase.TrafficPackageHeaderSize);
			TrafficStatsOutgoing = new TrafficStats(peerBase.TrafficPackageHeaderSize);
			TrafficStatsGameLevel = new TrafficStatsGameLevel();
			trafficStatsStopwatch = new Stopwatch();
		}

		public string CommandLogToString()
		{
			return peerBase.CommandLogToString();
		}

		public PhotonPeer(ConnectionProtocol protocolType)
		{
			TransportProtocol = protocolType;
			CreatePeerBase();
		}

		public PhotonPeer(IPhotonPeerListener listener, ConnectionProtocol protocolType)
			: this(protocolType)
		{
			Listener = listener;
		}

		public virtual bool Connect(string serverAddress, string applicationName)
		{
			return Connect(serverAddress, applicationName, null);
		}

		public virtual bool Connect(string serverAddress, string applicationName, object custom)
		{
			lock (DispatchLockObject)
			{
				lock (SendOutgoingLockObject)
				{
					CreatePeerBase();
					if (peerBase == null)
					{
						return false;
					}
					if (peerBase.SocketImplementation == null)
					{
						peerBase.EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("Connect failed. SocketImplementationConfig is null for protocol ", TransportProtocol, ": ", SupportClass.DictionaryToString(SocketImplementationConfig)));
						return false;
					}
					peerBase.CustomInitData = custom;
					peerBase.AppId = applicationName;
					return peerBase.Connect(serverAddress, applicationName, custom);
				}
			}
		}

		private void CreatePeerBase()
		{
			if (SocketImplementationConfig == null)
			{
				SocketImplementationConfig = new Dictionary<ConnectionProtocol, Type>(5);
				SocketImplementationConfig.Add(ConnectionProtocol.Udp, typeof(SocketUdp));
				SocketImplementationConfig.Add(ConnectionProtocol.Tcp, typeof(SocketTcp));
			}
			switch (TransportProtocol)
			{
			case ConnectionProtocol.Tcp:
				peerBase = new TPeer();
				break;
			case ConnectionProtocol.WebSocket:
			case ConnectionProtocol.WebSocketSecure:
				peerBase = new TPeer
				{
					DoFraming = false
				};
				break;
			default:
				peerBase = new EnetPeer();
				break;
			}
			if (peerBase == null)
			{
				throw new Exception("No PeerBase");
			}
			peerBase.ppeer = this;
			peerBase.usedProtocol = TransportProtocol;
			Type value = null;
			SocketImplementationConfig.TryGetValue(TransportProtocol, out value);
			SocketImplementation = value;
		}

		public virtual void Disconnect()
		{
			lock (DispatchLockObject)
			{
				lock (SendOutgoingLockObject)
				{
					peerBase.Disconnect();
				}
			}
		}

		public virtual void StopThread()
		{
			lock (DispatchLockObject)
			{
				lock (SendOutgoingLockObject)
				{
					peerBase.StopConnection();
				}
			}
		}

		public virtual void FetchServerTimestamp()
		{
			peerBase.FetchServerTimestamp();
		}

		public bool EstablishEncryption()
		{
			if (AsyncKeyExchange)
			{
				SupportClass.CallInBackground(delegate
				{
					peerBase.ExchangeKeysForEncryption(SendOutgoingLockObject);
					return false;
				});
				return true;
			}
			return peerBase.ExchangeKeysForEncryption(SendOutgoingLockObject);
		}

		public bool InitDatagramEncryption(byte[] encryptionSecret, byte[] hmacSecret, bool randomizedSequenceNumbers = false)
		{
			try
			{
				encryptor = new Encryptor();
				encryptor.Init(encryptionSecret, hmacSecret);
				decryptor = new Decryptor();
				decryptor.Init(encryptionSecret, hmacSecret);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public void InitPayloadEncryption(byte[] secret)
		{
			PayloadEncryptionSecret = secret;
		}

		public virtual void Service()
		{
			while (DispatchIncomingCommands())
			{
			}
			while (SendOutgoingCommands())
			{
			}
		}

		public virtual bool SendOutgoingCommands()
		{
			if (TrafficStatsEnabled)
			{
				TrafficStatsGameLevel.SendOutgoingCommandsCalled();
			}
			lock (SendOutgoingLockObject)
			{
				return peerBase.SendOutgoingCommands();
			}
		}

		public virtual bool SendAcksOnly()
		{
			if (TrafficStatsEnabled)
			{
				TrafficStatsGameLevel.SendOutgoingCommandsCalled();
			}
			lock (SendOutgoingLockObject)
			{
				return peerBase.SendAcksOnly();
			}
		}

		public virtual bool DispatchIncomingCommands()
		{
			if (TrafficStatsEnabled)
			{
				TrafficStatsGameLevel.DispatchIncomingCommandsCalled();
			}
			lock (DispatchLockObject)
			{
				peerBase.ByteCountCurrentDispatch = 0;
				return peerBase.DispatchIncomingCommands();
			}
		}

		public string VitalStatsToString(bool all)
		{
			if (TrafficStatsGameLevel == null)
			{
				return "Stats not available. Use PhotonPeer.TrafficStatsEnabled.";
			}
			if (!all)
			{
				return string.Format("Rtt(variance): {0}({1}). Ms since last receive: {2}. Stats elapsed: {4}sec.\n{3}", RoundTripTime, RoundTripTimeVariance, SupportClass.GetTickCount() - TimestampOfLastSocketReceive, TrafficStatsGameLevel.ToStringVitalStats(), TrafficStatsElapsedMs / 1000);
			}
			return string.Format("Rtt(variance): {0}({1}). Ms since last receive: {2}. Stats elapsed: {6}sec.\n{3}\n{4}\n{5}", RoundTripTime, RoundTripTimeVariance, SupportClass.GetTickCount() - TimestampOfLastSocketReceive, TrafficStatsGameLevel.ToStringVitalStats(), TrafficStatsIncoming.ToString(), TrafficStatsOutgoing.ToString(), TrafficStatsElapsedMs / 1000);
		}

		public virtual bool SendOperation(byte operationCode, Dictionary<byte, object> operationParameters, SendOptions sendOptions)
		{
			if (peerBase.usedProtocol == ConnectionProtocol.WebSocketSecure)
			{
				sendOptions.Encrypt = false;
			}
			lock (EnqueueLock)
			{
				return peerBase.EnqueueOperation(operationParameters, operationCode, sendOptions.Reliability, sendOptions.Channel, sendOptions.Encrypt);
			}
		}

		public virtual bool OpCustom(byte customOpCode, Dictionary<byte, object> customOpParameters, bool sendReliable)
		{
			return OpCustom(customOpCode, customOpParameters, sendReliable, 0);
		}

		public virtual bool OpCustom(byte customOpCode, Dictionary<byte, object> customOpParameters, bool sendReliable, byte channelId)
		{
			lock (EnqueueLock)
			{
				return peerBase.EnqueueOperation(customOpParameters, customOpCode, sendReliable, channelId, false);
			}
		}

		public virtual bool OpCustom(byte customOpCode, Dictionary<byte, object> customOpParameters, bool sendReliable, byte channelId, bool encrypt)
		{
			if (peerBase.usedProtocol == ConnectionProtocol.WebSocketSecure)
			{
				encrypt = false;
			}
			if (encrypt && !IsEncryptionAvailable)
			{
				throw new ArgumentException("Can't use encryption yet. Exchange keys first.");
			}
			lock (EnqueueLock)
			{
				return peerBase.EnqueueOperation(customOpParameters, customOpCode, sendReliable, channelId, encrypt);
			}
		}

		public virtual bool OpCustom(OperationRequest operationRequest, bool sendReliable, byte channelId, bool encrypt)
		{
			if (peerBase.usedProtocol == ConnectionProtocol.WebSocketSecure)
			{
				encrypt = false;
			}
			if (encrypt && !IsEncryptionAvailable)
			{
				throw new ArgumentException("Can't use encryption yet. Exchange keys first.");
			}
			lock (EnqueueLock)
			{
				return peerBase.EnqueueOperation(operationRequest.Parameters, operationRequest.OperationCode, sendReliable, channelId, encrypt);
			}
		}

		public static bool RegisterType(Type customType, byte code, SerializeMethod serializeMethod, DeserializeMethod constructor)
		{
			return Protocol.TryRegisterType(customType, code, serializeMethod, constructor);
		}

		public static bool RegisterType(Type customType, byte code, SerializeStreamMethod serializeMethod, DeserializeStreamMethod constructor)
		{
			return Protocol.TryRegisterType(customType, code, serializeMethod, constructor);
		}
	}
}
