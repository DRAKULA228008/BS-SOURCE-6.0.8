using System;
using System.Collections.Generic;
using System.IO;

namespace ExitGames.Client.Photon
{
	internal class TPeer : PeerBase
	{
		internal const int TCP_HEADER_BYTES = 7;

		internal const int MSG_HEADER_BYTES = 2;

		public const int ALL_HEADER_BYTES = 9;

		private Queue<byte[]> incomingList = new Queue<byte[]>(32);

		internal List<byte[]> outgoingStream;

		private int lastPingResult;

		private byte[] pingRequest = new byte[5] { 240, 0, 0, 0, 0 };

		internal static readonly byte[] tcpFramedMessageHead = new byte[9] { 251, 0, 0, 0, 0, 0, 0, 243, 2 };

		internal static readonly byte[] tcpMsgHead = new byte[2] { 243, 2 };

		internal byte[] messageHeader;

		protected internal bool DoFraming = true;

		internal override int QueuedIncomingCommandsCount
		{
			get
			{
				return incomingList.Count;
			}
		}

		internal override int QueuedOutgoingCommandsCount
		{
			get
			{
				return outgoingCommandsInStream;
			}
		}

		internal TPeer()
		{
			PeerBase.peerCount++;
			InitOnce();
			TrafficPackageHeaderSize = 0;
		}

		internal override void InitPeerBase()
		{
			base.InitPeerBase();
			incomingList = new Queue<byte[]>(32);
			timestampOfLastReceive = SupportClass.GetTickCount();
		}

		internal override bool Connect(string serverAddress, string appID, object customData = null)
		{
			if (peerConnectionState != 0)
			{
				base.Listener.DebugReturn(DebugLevel.WARNING, "Connect() can't be called if peer is not Disconnected. Not connecting.");
				return false;
			}
			if ((int)base.debugOut >= 5)
			{
				base.Listener.DebugReturn(DebugLevel.ALL, "Connect()");
			}
			base.ServerAddress = serverAddress;
			InitPeerBase();
			outgoingStream = new List<byte[]>();
			if (usedProtocol == ConnectionProtocol.WebSocket || usedProtocol == ConnectionProtocol.WebSocketSecure)
			{
				serverAddress = PepareWebSocketUrl(serverAddress, appID, customData);
			}
			if (base.SocketImplementation != null)
			{
				rt = (IPhotonSocket)Activator.CreateInstance(base.SocketImplementation, this);
			}
			else
			{
				rt = new SocketTcp(this);
			}
			if (rt == null)
			{
				base.Listener.DebugReturn(DebugLevel.ERROR, "Connect() failed, because SocketImplementation or socket was null. Set PhotonPeer.SocketImplementation before Connect(). SocketImplementation: " + base.SocketImplementation);
				return false;
			}
			messageHeader = (DoFraming ? tcpFramedMessageHead : tcpMsgHead);
			if (rt.Connect())
			{
				peerConnectionState = ConnectionStateValue.Connecting;
				return true;
			}
			peerConnectionState = ConnectionStateValue.Disconnected;
			return false;
		}

		public override void OnConnect()
		{
			lastPingResult = SupportClass.GetTickCount();
			byte[] data = PrepareConnectData(base.ServerAddress, AppId, CustomInitData);
			EnqueueInit(data);
			SendOutgoingCommands();
		}

		internal override void Disconnect()
		{
			if (peerConnectionState != 0 && peerConnectionState != ConnectionStateValue.Disconnecting)
			{
				if ((int)base.debugOut >= 5)
				{
					base.Listener.DebugReturn(DebugLevel.ALL, "TPeer.Disconnect()");
				}
				StopConnection();
			}
		}

		internal override void StopConnection()
		{
			peerConnectionState = ConnectionStateValue.Disconnecting;
			if (rt != null)
			{
				rt.Disconnect();
			}
			lock (incomingList)
			{
				incomingList.Clear();
			}
			peerConnectionState = ConnectionStateValue.Disconnected;
			EnqueueStatusCallback(StatusCode.Disconnect);
		}

		internal override void FetchServerTimestamp()
		{
			if (peerConnectionState != ConnectionStateValue.Connected)
			{
				if ((int)base.debugOut >= 3)
				{
					base.Listener.DebugReturn(DebugLevel.INFO, "FetchServerTimestamp() was skipped, as the client is not connected. Current ConnectionState: " + peerConnectionState);
				}
				base.Listener.OnStatusChanged(StatusCode.SendError);
			}
			else
			{
				SendPing();
				serverTimeOffsetIsAvailable = false;
			}
		}

		private void EnqueueInit(byte[] data)
		{
			if (DoFraming)
			{
				StreamBuffer streamBuffer = new StreamBuffer(data.Length + 32);
				BinaryWriter binaryWriter = new BinaryWriter(streamBuffer);
				byte[] array = new byte[7] { 251, 0, 0, 0, 0, 0, 1 };
				int targetOffset = 1;
				Protocol.Serialize(data.Length + array.Length, array, ref targetOffset);
				binaryWriter.Write(array);
				binaryWriter.Write(data);
				byte[] array2 = streamBuffer.ToArray();
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsOutgoing.TotalPacketCount++;
					base.TrafficStatsOutgoing.TotalCommandsInPackets++;
					base.TrafficStatsOutgoing.CountControlCommand(array2.Length);
				}
				EnqueueMessageAsPayload(true, array2, 0);
			}
		}

		internal override bool DispatchIncomingCommands()
		{
			while (true)
			{
				MyAction myAction;
				lock (ActionQueue)
				{
					if (ActionQueue.Count <= 0)
					{
						break;
					}
					myAction = ActionQueue.Dequeue();
					goto IL_0042;
				}
				IL_0042:
				myAction();
			}
			byte[] array;
			lock (incomingList)
			{
				if (incomingList.Count <= 0)
				{
					return false;
				}
				array = incomingList.Dequeue();
			}
			ByteCountCurrentDispatch = array.Length + 3;
			return DeserializeMessageAndCallback(array);
		}

		internal override bool SendOutgoingCommands()
		{
			if (peerConnectionState == ConnectionStateValue.Disconnected)
			{
				return false;
			}
			if (!rt.Connected)
			{
				return false;
			}
			timeInt = SupportClass.GetTickCount() - timeBase;
			timeLastSendOutgoing = timeInt;
			if (peerConnectionState == ConnectionStateValue.Connected && Math.Abs(SupportClass.GetTickCount() - lastPingResult) > base.timePingInterval)
			{
				SendPing();
			}
			lock (outgoingStream)
			{
				foreach (byte[] item in outgoingStream)
				{
					SendData(item);
				}
				outgoingStream.Clear();
				outgoingCommandsInStream = 0;
			}
			return false;
		}

		internal override bool SendAcksOnly()
		{
			if (rt == null || !rt.Connected)
			{
				return false;
			}
			timeInt = SupportClass.GetTickCount() - timeBase;
			if (peerConnectionState == ConnectionStateValue.Connected && SupportClass.GetTickCount() - lastPingResult > base.timePingInterval)
			{
				SendPing();
			}
			return false;
		}

		internal override bool EnqueueOperation(Dictionary<byte, object> parameters, byte opCode, bool sendReliable, byte channelId, bool encrypt, EgMessageType messageType)
		{
			if (peerConnectionState != ConnectionStateValue.Connected)
			{
				if ((int)base.debugOut >= 1)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: " + opCode + "! Not connected. PeerState: " + peerConnectionState);
				}
				base.Listener.OnStatusChanged(StatusCode.SendError);
				return false;
			}
			if (channelId >= base.ChannelCount)
			{
				if ((int)base.debugOut >= 1)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: Selected channel (" + channelId + ")>= channelCount (" + base.ChannelCount + ").");
				}
				base.Listener.OnStatusChanged(StatusCode.SendError);
				return false;
			}
			byte[] opMessage = SerializeOperationToMessage(opCode, parameters, messageType, encrypt);
			return EnqueueMessageAsPayload(sendReliable, opMessage, channelId);
		}

		internal override byte[] SerializeOperationToMessage(byte opc, Dictionary<byte, object> parameters, EgMessageType messageType, bool encrypt)
		{
			byte[] array2;
			lock (SerializeMemStream)
			{
				SerializeMemStream.SetLength(0L);
				if (!encrypt)
				{
					SerializeMemStream.Write(messageHeader, 0, messageHeader.Length);
				}
				protocol.SerializeOperationRequest(SerializeMemStream, opc, parameters, false);
				if (encrypt)
				{
					byte[] array = CryptoProvider.Encrypt(SerializeMemStream.GetBuffer(), 0, (int)SerializeMemStream.Length);
					SerializeMemStream.SetLength(0L);
					SerializeMemStream.Write(messageHeader, 0, messageHeader.Length);
					SerializeMemStream.Write(array, 0, array.Length);
				}
				array2 = SerializeMemStream.ToArray();
			}
			if (messageType != EgMessageType.Operation)
			{
				array2[messageHeader.Length - 1] = (byte)messageType;
			}
			if (encrypt)
			{
				array2[messageHeader.Length - 1] = (byte)(array2[messageHeader.Length - 1] | 0x80u);
			}
			if (DoFraming)
			{
				int targetOffset = 1;
				Protocol.Serialize(array2.Length, array2, ref targetOffset);
			}
			return array2;
		}

		internal bool EnqueueMessageAsPayload(bool sendReliable, byte[] opMessage, byte channelId)
		{
			if (opMessage == null)
			{
				return false;
			}
			if (DoFraming)
			{
				opMessage[5] = channelId;
				opMessage[6] = (byte)(sendReliable ? 1 : 0);
			}
			lock (outgoingStream)
			{
				outgoingStream.Add(opMessage);
				outgoingCommandsInStream++;
			}
			int num = (ByteCountLastOperation = opMessage.Length);
			if (base.TrafficStatsEnabled)
			{
				if (sendReliable)
				{
					base.TrafficStatsOutgoing.CountReliableOpCommand(num);
				}
				else
				{
					base.TrafficStatsOutgoing.CountUnreliableOpCommand(num);
				}
				base.TrafficStatsGameLevel.CountOperation(num);
			}
			return true;
		}

		internal void SendPing()
		{
			lastPingResult = SupportClass.GetTickCount();
			if (!DoFraming)
			{
				int tickCount = SupportClass.GetTickCount();
				EnqueueOperation(new Dictionary<byte, object> { { 1, tickCount } }, PhotonCodes.Ping, true, 0, false, EgMessageType.InternalOperationRequest);
				byte[] data = outgoingStream[outgoingStream.Count - 1];
				outgoingStream.RemoveAt(outgoingStream.Count - 1);
				SendData(data);
				return;
			}
			int targetOffset = 1;
			Protocol.Serialize(SupportClass.GetTickCount(), pingRequest, ref targetOffset);
			if (base.TrafficStatsEnabled)
			{
				base.TrafficStatsOutgoing.CountControlCommand(pingRequest.Length);
			}
			if (base.NetworkSimulationSettings.IsSimulationEnabled)
			{
				byte[] array = new byte[pingRequest.Length];
				Buffer.BlockCopy(pingRequest, 0, array, 0, pingRequest.Length);
				SendData(array);
			}
			else
			{
				SendData(pingRequest);
			}
		}

		internal void SendData(byte[] data, int length = -1)
		{
			if (length < 0)
			{
				length = data.Length;
			}
			try
			{
				bytesOut += length;
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsOutgoing.TotalPacketCount++;
					base.TrafficStatsOutgoing.TotalCommandsInPackets += outgoingCommandsInStream;
				}
				if (base.NetworkSimulationSettings.IsSimulationEnabled)
				{
					SendNetworkSimulated(data);
				}
				else
				{
					rt.Send(data, length);
				}
			}
			catch (Exception ex)
			{
				if ((int)base.debugOut >= 1)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, ex.ToString());
				}
				SupportClass.WriteStackTrace(ex);
			}
		}

		internal override void ReceiveIncomingCommands(byte[] inbuff, int dataLength)
		{
			if (inbuff == null)
			{
				if ((int)base.debugOut >= 1)
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "checkAndQueueIncomingCommands() inBuff: null");
				}
				return;
			}
			timestampOfLastReceive = SupportClass.GetTickCount();
			timeInt = SupportClass.GetTickCount() - timeBase;
			bytesIn += dataLength + 7;
			if (base.TrafficStatsEnabled)
			{
				base.TrafficStatsIncoming.TotalPacketCount++;
				base.TrafficStatsIncoming.TotalCommandsInPackets++;
			}
			if (inbuff[0] == 243 || inbuff[0] == 244)
			{
				byte[] array = new byte[dataLength];
				Buffer.BlockCopy(inbuff, 0, array, 0, dataLength);
				lock (incomingList)
				{
					incomingList.Enqueue(array);
				}
			}
			else if (inbuff[0] == 240)
			{
				base.TrafficStatsIncoming.CountControlCommand(inbuff.Length);
				ReadPingResult(inbuff);
			}
			else if ((int)base.debugOut >= 1)
			{
				EnqueueDebugReturn(DebugLevel.ERROR, "receiveIncomingCommands() MagicNumber should be 0xF0, 0xF3 or 0xF4. Is: " + inbuff[0]);
			}
		}

		private void ReadPingResult(byte[] inbuff)
		{
			int value = 0;
			int value2 = 0;
			int offset = 1;
			Protocol.Deserialize(out value, inbuff, ref offset);
			Protocol.Deserialize(out value2, inbuff, ref offset);
			lastRoundTripTime = SupportClass.GetTickCount() - value2;
			if (!serverTimeOffsetIsAvailable)
			{
				roundTripTime = lastRoundTripTime;
			}
			UpdateRoundTripTimeAndVariance(lastRoundTripTime);
			if (!serverTimeOffsetIsAvailable)
			{
				serverTimeOffset = value + (lastRoundTripTime >> 1) - SupportClass.GetTickCount();
				serverTimeOffsetIsAvailable = true;
			}
		}

		protected internal void ReadPingResult(OperationResponse operationResponse)
		{
			int num = (int)operationResponse.Parameters[2];
			int num2 = (int)operationResponse.Parameters[1];
			lastRoundTripTime = SupportClass.GetTickCount() - num2;
			if (!serverTimeOffsetIsAvailable)
			{
				roundTripTime = lastRoundTripTime;
			}
			UpdateRoundTripTimeAndVariance(lastRoundTripTime);
			if (!serverTimeOffsetIsAvailable)
			{
				serverTimeOffset = num + (lastRoundTripTime >> 1) - SupportClass.GetTickCount();
				serverTimeOffsetIsAvailable = true;
			}
		}
	}
}
