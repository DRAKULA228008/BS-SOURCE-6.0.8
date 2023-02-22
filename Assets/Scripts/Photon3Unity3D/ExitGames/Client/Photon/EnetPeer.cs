using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ExitGames.Client.Photon.EncryptorManaged;

namespace ExitGames.Client.Photon
{
	internal class EnetPeer : PeerBase
	{
		private const int CRC_LENGTH = 4;

		private static readonly int HMAC_SIZE = 32;

		private static readonly int BLOCK_SIZE = 16;

		private static readonly int IV_SIZE = 16;

		private const int EncryptedDataGramHeaderSize = 7;

		private const int EncryptedHeaderSize = 5;

		private List<NCommand> sentReliableCommands = new List<NCommand>();

		private StreamBuffer outgoingAcknowledgementsPool;

		internal readonly int windowSize = 128;

		private byte udpCommandCount;

		private byte[] udpBuffer;

		private int udpBufferIndex;

		private int udpBufferLength;

		private byte[] bufferForEncryption;

		internal int challenge;

		internal int reliableCommandsRepeated;

		internal int reliableCommandsSent;

		internal int serverSentTime;

		internal static readonly byte[] udpHeader0xF3 = new byte[2] { 243, 2 };

		internal static readonly byte[] messageHeader = udpHeader0xF3;

		protected bool datagramEncryptedConnection;

		private EnetChannel[] channelArray = new EnetChannel[0];

		private const byte ControlChannelNumber = byte.MaxValue;

		protected internal const short PeerIdForConnect = -1;

		protected internal const short PeerIdForConnectTrace = -2;

		private Queue<int> commandsToRemove = new Queue<int>();

		private Queue<NCommand> commandsToResend = new Queue<NCommand>();

		internal override int QueuedIncomingCommandsCount
		{
			get
			{
				int num = 0;
				lock (channelArray)
				{
					for (int i = 0; i < channelArray.Length; i++)
					{
						EnetChannel enetChannel = channelArray[i];
						num += enetChannel.incomingReliableCommandsList.Count;
						num += enetChannel.incomingUnreliableCommandsList.Count;
					}
				}
				return num;
			}
		}

		internal override int QueuedOutgoingCommandsCount
		{
			get
			{
				int num = 0;
				lock (channelArray)
				{
					for (int i = 0; i < channelArray.Length; i++)
					{
						EnetChannel enetChannel = channelArray[i];
						num += enetChannel.outgoingReliableCommandsList.Count;
						num += enetChannel.outgoingUnreliableCommandsList.Count;
					}
				}
				return num;
			}
		}

		private Encryptor encryptor
		{
			get
			{
				return ppeer.encryptor;
			}
		}

		private Decryptor decryptor
		{
			get
			{
				return ppeer.decryptor;
			}
		}

		internal EnetPeer()
		{
			PeerBase.peerCount++;
			InitOnce();
			TrafficPackageHeaderSize = 12;
		}

		internal override void InitPeerBase()
		{
			base.InitPeerBase();
			if (ppeer.PayloadEncryptionSecret != null && usedProtocol == ConnectionProtocol.Udp)
			{
				InitEncryption(ppeer.PayloadEncryptionSecret);
			}
			if (encryptor != null && decryptor != null)
			{
				isEncryptionAvailable = true;
			}
			peerID = (short)(ppeer.EnableServerTracing ? (-2) : (-1));
			challenge = SupportClass.ThreadSafeRandom.Next();
			if (udpBuffer == null || udpBuffer.Length != base.mtu)
			{
				udpBuffer = new byte[base.mtu];
			}
			reliableCommandsSent = 0;
			reliableCommandsRepeated = 0;
			lock (channelArray)
			{
				EnetChannel[] array = channelArray;
				if (array.Length != base.ChannelCount + 1)
				{
					array = new EnetChannel[base.ChannelCount + 1];
				}
				for (byte b = 0; b < base.ChannelCount; b = (byte)(b + 1))
				{
					array[b] = new EnetChannel(b, commandBufferSize);
				}
				array[base.ChannelCount] = new EnetChannel(byte.MaxValue, commandBufferSize);
				channelArray = array;
			}
			lock (sentReliableCommands)
			{
				sentReliableCommands = new List<NCommand>(commandBufferSize);
			}
			outgoingAcknowledgementsPool = new StreamBuffer();
			CommandLogInit();
		}

		internal override bool Connect(string ipport, string appID, object custom = null)
		{
			if (peerConnectionState != 0)
			{
				base.Listener.DebugReturn(DebugLevel.WARNING, "Connect() can't be called if peer is not Disconnected. Not connecting. peerConnectionState: " + peerConnectionState);
				return false;
			}
			if ((int)base.debugOut >= 5)
			{
				base.Listener.DebugReturn(DebugLevel.ALL, "Connect()");
			}
			base.ServerAddress = ipport;
			InitPeerBase();
			if (base.SocketImplementation != null)
			{
				rt = (IPhotonSocket)Activator.CreateInstance(base.SocketImplementation, this);
			}
			else
			{
				rt = new SocketUdp(this);
			}
			if (rt == null)
			{
				base.Listener.DebugReturn(DebugLevel.ERROR, "Connect() failed, because SocketImplementation or socket was null. Set PhotonPeer.SocketImplementation before Connect().");
				return false;
			}
			if (rt.Connect())
			{
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsOutgoing.ControlCommandBytes += 44;
					base.TrafficStatsOutgoing.ControlCommandCount++;
				}
				peerConnectionState = ConnectionStateValue.Connecting;
				return true;
			}
			return false;
		}

		public override void OnConnect()
		{
			QueueOutgoingReliableCommand(new NCommand(this, 2, null, byte.MaxValue));
		}

		internal override void Disconnect()
		{
			if (peerConnectionState == ConnectionStateValue.Disconnected || peerConnectionState == ConnectionStateValue.Disconnecting)
			{
				return;
			}
			if (sentReliableCommands != null)
			{
				lock (sentReliableCommands)
				{
					sentReliableCommands.Clear();
				}
			}
			lock (channelArray)
			{
				EnetChannel[] array = channelArray;
				foreach (EnetChannel enetChannel in array)
				{
					enetChannel.clearAll();
				}
			}
			bool isSimulationEnabled = base.NetworkSimulationSettings.IsSimulationEnabled;
			base.NetworkSimulationSettings.IsSimulationEnabled = false;
			NCommand nCommand = new NCommand(this, 4, null, byte.MaxValue);
			QueueOutgoingReliableCommand(nCommand);
			SendOutgoingCommands();
			if (base.TrafficStatsEnabled)
			{
				base.TrafficStatsOutgoing.CountControlCommand(nCommand.Size);
			}
			base.NetworkSimulationSettings.IsSimulationEnabled = isSimulationEnabled;
			rt.Disconnect();
			peerConnectionState = ConnectionStateValue.Disconnected;
			EnqueueStatusCallback(StatusCode.Disconnect);
			datagramEncryptedConnection = false;
		}

		internal override void StopConnection()
		{
			if (rt != null)
			{
				rt.Disconnect();
			}
			peerConnectionState = ConnectionStateValue.Disconnected;
			if (base.Listener != null)
			{
				base.Listener.OnStatusChanged(StatusCode.Disconnect);
			}
		}

		internal override void FetchServerTimestamp()
		{
			if (peerConnectionState != ConnectionStateValue.Connected || !ApplicationIsInitialized)
			{
				if ((int)base.debugOut >= 3)
				{
					EnqueueDebugReturn(DebugLevel.INFO, "FetchServerTimestamp() was skipped, as the client is not connected. Current ConnectionState: " + peerConnectionState);
				}
			}
			else
			{
				CreateAndEnqueueCommand(12, new byte[0], byte.MaxValue);
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
			NCommand value = null;
			lock (channelArray)
			{
				for (int i = 0; i < channelArray.Length; i++)
				{
					EnetChannel enetChannel = channelArray[i];
					if (enetChannel.incomingUnreliableCommandsList.Count > 0)
					{
						int num = int.MaxValue;
						foreach (int key in enetChannel.incomingUnreliableCommandsList.Keys)
						{
							NCommand nCommand = enetChannel.incomingUnreliableCommandsList[key];
							if (key < enetChannel.incomingUnreliableSequenceNumber || nCommand.reliableSequenceNumber < enetChannel.incomingReliableSequenceNumber)
							{
								commandsToRemove.Enqueue(key);
							}
							else if (base.limitOfUnreliableCommands > 0 && enetChannel.incomingUnreliableCommandsList.Count > base.limitOfUnreliableCommands)
							{
								commandsToRemove.Enqueue(key);
							}
							else if (key < num && nCommand.reliableSequenceNumber <= enetChannel.incomingReliableSequenceNumber)
							{
								num = key;
							}
						}
						while (commandsToRemove.Count > 0)
						{
							enetChannel.incomingUnreliableCommandsList.Remove(commandsToRemove.Dequeue());
						}
						if (num < int.MaxValue)
						{
							value = enetChannel.incomingUnreliableCommandsList[num];
						}
						if (value != null)
						{
							enetChannel.incomingUnreliableCommandsList.Remove(value.unreliableSequenceNumber);
							enetChannel.incomingUnreliableSequenceNumber = value.unreliableSequenceNumber;
							break;
						}
					}
					if (value != null || enetChannel.incomingReliableCommandsList.Count <= 0)
					{
						continue;
					}
					enetChannel.incomingReliableCommandsList.TryGetValue(enetChannel.incomingReliableSequenceNumber + 1, out value);
					if (value == null)
					{
						continue;
					}
					if (value.commandType != 8)
					{
						enetChannel.incomingReliableSequenceNumber = value.reliableSequenceNumber;
						enetChannel.incomingReliableCommandsList.Remove(value.reliableSequenceNumber);
						break;
					}
					if (value.fragmentsRemaining > 0)
					{
						value = null;
						break;
					}
					byte[] array = new byte[value.totalLength];
					for (int j = value.startSequenceNumber; j < value.startSequenceNumber + value.fragmentCount; j++)
					{
						if (enetChannel.ContainsReliableSequenceNumber(j))
						{
							NCommand nCommand2 = enetChannel.FetchReliableSequenceNumber(j);
							Buffer.BlockCopy(nCommand2.Payload, 0, array, nCommand2.fragmentOffset, nCommand2.Payload.Length);
							enetChannel.incomingReliableCommandsList.Remove(nCommand2.reliableSequenceNumber);
							continue;
						}
						throw new Exception("command.fragmentsRemaining was 0, but not all fragments are found to be combined!");
					}
					if ((int)base.debugOut >= 5)
					{
						base.Listener.DebugReturn(DebugLevel.ALL, "assembled fragmented payload from " + value.fragmentCount + " parts. Dispatching now.");
					}
					value.Payload = array;
					value.Size = 12 * value.fragmentCount + value.totalLength;
					enetChannel.incomingReliableSequenceNumber = value.reliableSequenceNumber + value.fragmentCount - 1;
					break;
				}
			}
			if (value != null && value.Payload != null)
			{
				ByteCountCurrentDispatch = value.Size;
				CommandInCurrentDispatch = value;
				if (DeserializeMessageAndCallback(value.Payload))
				{
					CommandInCurrentDispatch = null;
					return true;
				}
				CommandInCurrentDispatch = null;
			}
			return false;
		}

		private int GetFragmentLength()
		{
			int num = base.mtu;
			if (datagramEncryptedConnection)
			{
				num = num - 7 - HMAC_SIZE - IV_SIZE;
				num = num / BLOCK_SIZE * BLOCK_SIZE;
				return num - 5 - 36;
			}
			return num - 12 - 36;
		}

		private int CalculateBufferLen()
		{
			int num = base.mtu;
			if (datagramEncryptedConnection)
			{
				num = num - 7 - HMAC_SIZE - IV_SIZE;
				num = num / BLOCK_SIZE * BLOCK_SIZE;
				return num - 1;
			}
			return num;
		}

		private int CalculateInitialOffset()
		{
			if (datagramEncryptedConnection)
			{
				return 5;
			}
			int num = 12;
			if (base.crcEnabled)
			{
				num += 4;
			}
			return num;
		}

		internal override bool SendAcksOnly()
		{
			if (peerConnectionState == ConnectionStateValue.Disconnected)
			{
				return false;
			}
			if (rt == null || !rt.Connected)
			{
				return false;
			}
			lock (udpBuffer)
			{
				int num = 0;
				udpBufferIndex = CalculateInitialOffset();
				udpBufferLength = CalculateBufferLen();
				udpCommandCount = 0;
				timeInt = SupportClass.GetTickCount() - timeBase;
				lock (outgoingAcknowledgementsPool)
				{
					num = SerializeAckToBuffer();
					timeLastSendAck = timeInt;
				}
				if (timeInt > timeoutInt && sentReliableCommands.Count > 0)
				{
					lock (sentReliableCommands)
					{
						foreach (NCommand sentReliableCommand in sentReliableCommands)
						{
							if (sentReliableCommand != null && sentReliableCommand.roundTripTimeout != 0 && timeInt - sentReliableCommand.commandSentTime > sentReliableCommand.roundTripTimeout)
							{
								sentReliableCommand.commandSentCount = 1;
								sentReliableCommand.roundTripTimeout = 0;
								sentReliableCommand.timeoutTime = int.MaxValue;
								sentReliableCommand.commandSentTime = timeInt;
							}
						}
					}
				}
				if (udpCommandCount <= 0)
				{
					return false;
				}
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsOutgoing.TotalPacketCount++;
					base.TrafficStatsOutgoing.TotalCommandsInPackets += udpCommandCount;
				}
				SendData(udpBuffer, udpBufferIndex);
				return num > 0;
			}
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
			lock (udpBuffer)
			{
				int num = 0;
				udpBufferIndex = CalculateInitialOffset();
				udpBufferLength = CalculateBufferLen();
				udpCommandCount = 0;
				timeInt = SupportClass.GetTickCount() - timeBase;
				timeLastSendOutgoing = timeInt;
				lock (outgoingAcknowledgementsPool)
				{
					if (outgoingAcknowledgementsPool.Length > 0)
					{
						num = SerializeAckToBuffer();
						timeLastSendAck = timeInt;
					}
				}
				if (!base.IsSendingOnlyAcks && timeInt > timeoutInt && sentReliableCommands.Count > 0)
				{
					lock (sentReliableCommands)
					{
						commandsToResend.Clear();
						foreach (NCommand sentReliableCommand in sentReliableCommands)
						{
							if (sentReliableCommand == null || timeInt - sentReliableCommand.commandSentTime <= sentReliableCommand.roundTripTimeout)
							{
								continue;
							}
							if (sentReliableCommand.commandSentCount > base.sentCountAllowance || timeInt > sentReliableCommand.timeoutTime)
							{
								if ((int)base.debugOut >= 2)
								{
									base.Listener.DebugReturn(DebugLevel.WARNING, string.Concat("Timeout-disconnect! Command: ", sentReliableCommand, " now: ", timeInt, " challenge: ", Convert.ToString(challenge, 16)));
								}
								if (CommandLog != null)
								{
									CommandLog.Enqueue(new CmdLogSentReliable(sentReliableCommand, timeInt, roundTripTime, roundTripTimeVariance, true));
									CommandLogResize();
								}
								peerConnectionState = ConnectionStateValue.Zombie;
								EnqueueStatusCallback(StatusCode.TimeoutDisconnect);
								Disconnect();
								return false;
							}
							commandsToResend.Enqueue(sentReliableCommand);
						}
						while (commandsToResend.Count > 0)
						{
							NCommand nCommand = commandsToResend.Dequeue();
							QueueOutgoingReliableCommand(nCommand);
							sentReliableCommands.Remove(nCommand);
							reliableCommandsRepeated++;
							if ((int)base.debugOut >= 3)
							{
								base.Listener.DebugReturn(DebugLevel.INFO, string.Format("Resending: {0}. times out after: {1} sent: {3} now: {2} rtt/var: {4}/{5} last recv: {6}", nCommand, nCommand.roundTripTimeout, timeInt, nCommand.commandSentTime, roundTripTime, roundTripTimeVariance, SupportClass.GetTickCount() - timestampOfLastReceive));
							}
						}
					}
				}
				if (!base.IsSendingOnlyAcks && peerConnectionState == ConnectionStateValue.Connected && base.timePingInterval > 0 && sentReliableCommands.Count == 0 && timeInt - timeLastAckReceive > base.timePingInterval && !AreReliableCommandsInTransit() && udpBufferIndex + 12 < udpBufferLength)
				{
					NCommand nCommand2 = new NCommand(this, 5, null, byte.MaxValue);
					QueueOutgoingReliableCommand(nCommand2);
					if (base.TrafficStatsEnabled)
					{
						base.TrafficStatsOutgoing.CountControlCommand(nCommand2.Size);
					}
				}
				if (!base.IsSendingOnlyAcks)
				{
					lock (channelArray)
					{
						for (int i = 0; i < channelArray.Length; i++)
						{
							EnetChannel enetChannel = channelArray[i];
							num += SerializeToBuffer(enetChannel.outgoingReliableCommandsList);
							num += SerializeToBuffer(enetChannel.outgoingUnreliableCommandsList);
						}
					}
				}
				if (udpCommandCount <= 0)
				{
					return false;
				}
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsOutgoing.TotalPacketCount++;
					base.TrafficStatsOutgoing.TotalCommandsInPackets += udpCommandCount;
				}
				SendData(udpBuffer, udpBufferIndex);
				return num > 0;
			}
		}

		private bool AreReliableCommandsInTransit()
		{
			lock (channelArray)
			{
				for (int i = 0; i < channelArray.Length; i++)
				{
					EnetChannel enetChannel = channelArray[i];
					if (enetChannel.outgoingReliableCommandsList.Count > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal override bool EnqueueOperation(Dictionary<byte, object> parameters, byte opCode, bool sendReliable, byte channelId, bool encrypt, EgMessageType messageType)
		{
			if (peerConnectionState != ConnectionStateValue.Connected)
			{
				if ((int)base.debugOut >= 1)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: " + opCode + " Not connected. PeerState: " + peerConnectionState);
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
			byte[] payload = SerializeOperationToMessage(opCode, parameters, messageType, encrypt);
			return CreateAndEnqueueCommand((byte)(sendReliable ? 6 : 7), payload, channelId);
		}

		private EnetChannel GetChannel(byte channelNumber)
		{
			return (channelNumber == byte.MaxValue) ? channelArray[channelArray.Length - 1] : channelArray[channelNumber];
		}

		internal bool CreateAndEnqueueCommand(byte commandType, byte[] payload, byte channelNumber)
		{
			if (payload == null)
			{
				return false;
			}
			EnetChannel channel = GetChannel(channelNumber);
			ByteCountLastOperation = 0;
			int num = GetFragmentLength();
			if (payload.Length > num)
			{
				int fragmentCount = (payload.Length + num - 1) / num;
				int startSequenceNumber = channel.outgoingReliableSequenceNumber + 1;
				int num2 = 0;
				for (int i = 0; i < payload.Length; i += num)
				{
					if (payload.Length - i < num)
					{
						num = payload.Length - i;
					}
					byte[] array = new byte[num];
					Buffer.BlockCopy(payload, i, array, 0, num);
					NCommand nCommand = new NCommand(this, 8, array, channel.ChannelNumber);
					nCommand.fragmentNumber = num2;
					nCommand.startSequenceNumber = startSequenceNumber;
					nCommand.fragmentCount = fragmentCount;
					nCommand.totalLength = payload.Length;
					nCommand.fragmentOffset = i;
					QueueOutgoingReliableCommand(nCommand);
					ByteCountLastOperation += nCommand.Size;
					if (base.TrafficStatsEnabled)
					{
						base.TrafficStatsOutgoing.CountFragmentOpCommand(nCommand.Size);
						base.TrafficStatsGameLevel.CountOperation(nCommand.Size);
					}
					num2++;
				}
			}
			else
			{
				NCommand nCommand2 = new NCommand(this, commandType, payload, channel.ChannelNumber);
				if (nCommand2.commandFlags == 1)
				{
					QueueOutgoingReliableCommand(nCommand2);
					ByteCountLastOperation = nCommand2.Size;
					if (base.TrafficStatsEnabled)
					{
						base.TrafficStatsOutgoing.CountReliableOpCommand(nCommand2.Size);
						base.TrafficStatsGameLevel.CountOperation(nCommand2.Size);
					}
				}
				else
				{
					QueueOutgoingUnreliableCommand(nCommand2);
					ByteCountLastOperation = nCommand2.Size;
					if (base.TrafficStatsEnabled)
					{
						base.TrafficStatsOutgoing.CountUnreliableOpCommand(nCommand2.Size);
						base.TrafficStatsGameLevel.CountOperation(nCommand2.Size);
					}
				}
			}
			return true;
		}

		internal override byte[] SerializeOperationToMessage(byte opCode, Dictionary<byte, object> parameters, EgMessageType messageType, bool encrypt)
		{
			encrypt = encrypt && !datagramEncryptedConnection;
			byte[] array2;
			lock (SerializeMemStream)
			{
				SerializeMemStream.SetLength(0L);
				if (!encrypt)
				{
					SerializeMemStream.Write(messageHeader, 0, messageHeader.Length);
				}
				protocol.SerializeOperationRequest(SerializeMemStream, opCode, parameters, false);
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
			return array2;
		}

		internal int SerializeAckToBuffer()
		{
			outgoingAcknowledgementsPool.Seek(0L, SeekOrigin.Begin);
			while (outgoingAcknowledgementsPool.Position + 20 <= outgoingAcknowledgementsPool.Length)
			{
				if (udpBufferIndex + 20 > udpBufferLength)
				{
					if ((int)base.debugOut >= 3)
					{
						base.Listener.DebugReturn(DebugLevel.INFO, "UDP package is full. Commands in Package: " + udpCommandCount + ". bytes left in queue: " + outgoingAcknowledgementsPool.Position);
					}
					break;
				}
				int offset;
				byte[] bufferAndAdvance = outgoingAcknowledgementsPool.GetBufferAndAdvance(20, out offset);
				Buffer.BlockCopy(bufferAndAdvance, offset, udpBuffer, udpBufferIndex, 20);
				udpBufferIndex += 20;
				udpCommandCount++;
			}
			outgoingAcknowledgementsPool.Compact();
			outgoingAcknowledgementsPool.Position = outgoingAcknowledgementsPool.Length;
			return (int)outgoingAcknowledgementsPool.Length / 20;
		}

		internal int SerializeToBuffer(Queue<NCommand> commandList)
		{
			while (commandList.Count > 0)
			{
				NCommand nCommand = commandList.Peek();
				if (nCommand == null)
				{
					commandList.Dequeue();
					continue;
				}
				if (udpBufferIndex + nCommand.Size > udpBufferLength)
				{
					if ((int)base.debugOut >= 3)
					{
						base.Listener.DebugReturn(DebugLevel.INFO, "UDP package is full. Commands in Package: " + udpCommandCount + ". Commands left in queue: " + commandList.Count);
					}
					break;
				}
				nCommand.SerializeHeader(udpBuffer, ref udpBufferIndex);
				if (nCommand.SizeOfPayload > 0)
				{
					Buffer.BlockCopy(nCommand.Serialize(), 0, udpBuffer, udpBufferIndex, nCommand.SizeOfPayload);
					udpBufferIndex += nCommand.SizeOfPayload;
				}
				udpCommandCount++;
				if ((nCommand.commandFlags & 1) > 0)
				{
					QueueSentCommand(nCommand);
					if (CommandLog != null)
					{
						CommandLog.Enqueue(new CmdLogSentReliable(nCommand, timeInt, roundTripTime, roundTripTimeVariance));
						CommandLogResize();
					}
				}
				commandList.Dequeue();
			}
			return commandList.Count;
		}

		internal void SendData(byte[] data, int length)
		{
			try
			{
				if (datagramEncryptedConnection)
				{
					SendDataEncrypted(data, length);
					return;
				}
				int targetOffset = 0;
				Protocol.Serialize(peerID, data, ref targetOffset);
				data[2] = (byte)(base.crcEnabled ? 204 : 0);
				data[3] = udpCommandCount;
				targetOffset = 4;
				Protocol.Serialize(timeInt, data, ref targetOffset);
				Protocol.Serialize(challenge, data, ref targetOffset);
				if (base.crcEnabled)
				{
					Protocol.Serialize(0, data, ref targetOffset);
					uint value = SupportClass.CalculateCrc(data, length);
					targetOffset -= 4;
					Protocol.Serialize((int)value, data, ref targetOffset);
				}
				bytesOut += length;
				SendToSocket(data, length);
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

		private void SendToSocket(byte[] data, int length)
		{
			if (base.NetworkSimulationSettings.IsSimulationEnabled)
			{
				byte[] array = new byte[length];
				Buffer.BlockCopy(data, 0, array, 0, length);
				SendNetworkSimulated(array);
			}
			else
			{
				rt.Send(data, length);
			}
		}

		private void SendDataEncrypted(byte[] data, int length)
		{
			if (bufferForEncryption == null || bufferForEncryption.Length != base.mtu)
			{
				bufferForEncryption = new byte[base.mtu];
			}
			byte[] array = bufferForEncryption;
			int targetOffset = 0;
			Protocol.Serialize(peerID, array, ref targetOffset);
			array[2] = 1;
			targetOffset++;
			Protocol.Serialize(challenge, array, ref targetOffset);
			data[0] = udpCommandCount;
			int targetOffset2 = 1;
			Protocol.Serialize(timeInt, data, ref targetOffset2);
			encryptor.Encrypt(data, length, array, ref targetOffset);
			Buffer.BlockCopy(encryptor.FinishHMAC(array, 0, targetOffset), 0, array, targetOffset, HMAC_SIZE);
			SendToSocket(array, targetOffset + HMAC_SIZE);
		}

		internal void QueueSentCommand(NCommand command)
		{
			command.commandSentTime = timeInt;
			command.commandSentCount++;
			if (command.roundTripTimeout == 0)
			{
				command.roundTripTimeout = roundTripTime + 4 * roundTripTimeVariance;
				command.timeoutTime = timeInt + base.DisconnectTimeout;
			}
			else if (command.commandSentCount > base.QuickResendAttempts + 1)
			{
				command.roundTripTimeout *= 2;
			}
			lock (sentReliableCommands)
			{
				if (sentReliableCommands.Count == 0)
				{
					int num = command.commandSentTime + command.roundTripTimeout;
					if (num < timeoutInt)
					{
						timeoutInt = num;
					}
				}
				reliableCommandsSent++;
				sentReliableCommands.Add(command);
			}
		}

		internal void QueueOutgoingReliableCommand(NCommand command)
		{
			EnetChannel channel = GetChannel(command.commandChannelID);
			lock (channel)
			{
				if (command.reliableSequenceNumber == 0)
				{
					command.reliableSequenceNumber = ++channel.outgoingReliableSequenceNumber;
				}
				channel.outgoingReliableCommandsList.Enqueue(command);
			}
		}

		internal void QueueOutgoingUnreliableCommand(NCommand command)
		{
			EnetChannel channel = GetChannel(command.commandChannelID);
			lock (channel)
			{
				command.reliableSequenceNumber = channel.outgoingReliableSequenceNumber;
				command.unreliableSequenceNumber = ++channel.outgoingUnreliableSequenceNumber;
				channel.outgoingUnreliableCommandsList.Enqueue(command);
			}
		}

		internal void QueueOutgoingAcknowledgement(NCommand readCommand, int sendTime)
		{
			lock (outgoingAcknowledgementsPool)
			{
				int offset;
				byte[] bufferAndAdvance = outgoingAcknowledgementsPool.GetBufferAndAdvance(20, out offset);
				NCommand.CreateAck(bufferAndAdvance, offset, readCommand, sendTime);
			}
		}

		internal override void ReceiveIncomingCommands(byte[] inBuff, int dataLength)
		{
			timestampOfLastReceive = SupportClass.GetTickCount();
			try
			{
				int offset = 0;
				short value;
				Protocol.Deserialize(out value, inBuff, ref offset);
				byte b = inBuff[offset++];
				int value2;
				byte b2;
				if (b == 1)
				{
					if (decryptor == null)
					{
						EnqueueDebugReturn(DebugLevel.ERROR, "Got encrypted packet, but encryption is not set up. Packet ignored");
						return;
					}
					datagramEncryptedConnection = true;
					if (!decryptor.CheckHMAC(inBuff, dataLength))
					{
						packetLossByCrc++;
						if (peerConnectionState != 0 && (int)base.debugOut >= 3)
						{
							EnqueueDebugReturn(DebugLevel.INFO, "Ignored package due to wrong HMAC.");
						}
						return;
					}
					Protocol.Deserialize(out value2, inBuff, ref offset);
					inBuff = decryptor.DecryptBufferWithIV(inBuff, offset, dataLength - offset - HMAC_SIZE, out dataLength);
					dataLength = inBuff.Length;
					offset = 0;
					b2 = inBuff[offset++];
					Protocol.Deserialize(out serverSentTime, inBuff, ref offset);
					bytesIn += 12 + IV_SIZE + HMAC_SIZE + dataLength + (BLOCK_SIZE - dataLength % BLOCK_SIZE);
					goto IL_0220;
				}
				if (datagramEncryptedConnection)
				{
					EnqueueDebugReturn(DebugLevel.WARNING, "Got not encrypted packet, but expected only encrypted. Packet ignored");
					return;
				}
				b2 = inBuff[offset++];
				Protocol.Deserialize(out serverSentTime, inBuff, ref offset);
				Protocol.Deserialize(out value2, inBuff, ref offset);
				if (b != 204)
				{
					goto IL_020f;
				}
				int value3;
				Protocol.Deserialize(out value3, inBuff, ref offset);
				bytesIn += 4L;
				offset -= 4;
				Protocol.Serialize(0, inBuff, ref offset);
				uint num = SupportClass.CalculateCrc(inBuff, dataLength);
				if (value3 == (int)num)
				{
					goto IL_020f;
				}
				packetLossByCrc++;
				if (peerConnectionState != 0 && (int)base.debugOut >= 3)
				{
					EnqueueDebugReturn(DebugLevel.INFO, string.Format("Ignored package due to wrong CRC. Incoming:  {0:X} Local: {1:X}", (uint)value3, num));
				}
				goto end_IL_000c;
				IL_0220:
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.TotalPacketCount++;
					base.TrafficStatsIncoming.TotalCommandsInPackets += b2;
				}
				if (b2 > commandBufferSize || b2 <= 0)
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "too many/few incoming commands in package: " + b2 + " > " + commandBufferSize);
				}
				if (value2 != challenge)
				{
					packetLossByChallenge++;
					if (peerConnectionState != 0 && (int)base.debugOut >= 5)
					{
						EnqueueDebugReturn(DebugLevel.ALL, "Info: Ignoring received package due to wrong challenge. Challenge in-package!=local:" + value2 + "!=" + challenge + " Commands in it: " + b2);
					}
					return;
				}
				timeInt = SupportClass.GetTickCount() - timeBase;
				for (int i = 0; i < b2; i++)
				{
					NCommand readCommand = new NCommand(this, inBuff, ref offset);
					if (readCommand.commandType != 1)
					{
						EnqueueActionForDispatch(delegate
						{
							ExecuteCommand(readCommand);
						});
					}
					else
					{
						ExecuteCommand(readCommand);
					}
					if ((readCommand.commandFlags & 1) > 0)
					{
						if (InReliableLog != null)
						{
							InReliableLog.Enqueue(new CmdLogReceivedReliable(readCommand, timeInt, roundTripTime, roundTripTimeVariance, timeInt - timeLastSendOutgoing, timeInt - timeLastSendAck));
							CommandLogResize();
						}
						QueueOutgoingAcknowledgement(readCommand, serverSentTime);
						if (base.TrafficStatsEnabled)
						{
							base.TrafficStatsIncoming.TimestampOfLastReliableCommand = SupportClass.GetTickCount();
							base.TrafficStatsOutgoing.CountControlCommand(20);
						}
					}
				}
				goto end_IL_000c;
				IL_020f:
				bytesIn += 12L;
				goto IL_0220;
				end_IL_000c:;
			}
			catch (Exception ex)
			{
				if ((int)base.debugOut >= 1)
				{
					EnqueueDebugReturn(DebugLevel.ERROR, string.Format("Exception while reading commands from incoming data: {0}", ex));
				}
				SupportClass.WriteStackTrace(ex);
			}
		}

		internal bool ExecuteCommand(NCommand command)
		{
			bool flag = true;
			switch (command.commandType)
			{
			case 2:
			case 5:
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.CountControlCommand(command.Size);
				}
				break;
			case 4:
			{
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.CountControlCommand(command.Size);
				}
				StatusCode statusValue = StatusCode.DisconnectByServer;
				if (command.reservedByte == 1)
				{
					statusValue = StatusCode.DisconnectByServerLogic;
				}
				else if (command.reservedByte == 3)
				{
					statusValue = StatusCode.DisconnectByServerUserLimit;
				}
				if ((int)base.debugOut >= 3)
				{
					base.Listener.DebugReturn(DebugLevel.INFO, "Server " + base.ServerAddress + " sent disconnect. PeerId: " + (ushort)peerID + " RTT/Variance:" + roundTripTime + "/" + roundTripTimeVariance + " reason byte: " + command.reservedByte);
				}
				EnqueueStatusCallback(statusValue);
				Disconnect();
				break;
			}
			case 1:
			{
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.TimestampOfLastAck = SupportClass.GetTickCount();
					base.TrafficStatsIncoming.CountControlCommand(command.Size);
				}
				timeLastAckReceive = timeInt;
				lastRoundTripTime = timeInt - command.ackReceivedSentTime;
				NCommand nCommand = RemoveSentReliableCommand(command.ackReceivedReliableSequenceNumber, command.commandChannelID);
				if (CommandLog != null)
				{
					CommandLog.Enqueue(new CmdLogReceivedAck(command, timeInt, roundTripTime, roundTripTimeVariance));
					CommandLogResize();
				}
				if (nCommand == null)
				{
					break;
				}
				if (nCommand.commandType == 12)
				{
					if (lastRoundTripTime <= roundTripTime)
					{
						serverTimeOffset = serverSentTime + (lastRoundTripTime >> 1) - SupportClass.GetTickCount();
						serverTimeOffsetIsAvailable = true;
					}
					else
					{
						FetchServerTimestamp();
					}
					break;
				}
				UpdateRoundTripTimeAndVariance(lastRoundTripTime);
				if (nCommand.commandType == 4 && peerConnectionState == ConnectionStateValue.Disconnecting)
				{
					if ((int)base.debugOut >= 3)
					{
						EnqueueDebugReturn(DebugLevel.INFO, "Received disconnect ACK by server");
					}
					EnqueueActionForDispatch(delegate
					{
						rt.Disconnect();
					});
				}
				else if (nCommand.commandType == 2)
				{
					roundTripTime = lastRoundTripTime;
				}
				break;
			}
			case 6:
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.CountReliableOpCommand(command.Size);
				}
				if (peerConnectionState == ConnectionStateValue.Connected)
				{
					flag = QueueIncomingCommand(command);
				}
				break;
			case 7:
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.CountUnreliableOpCommand(command.Size);
				}
				if (peerConnectionState == ConnectionStateValue.Connected)
				{
					flag = QueueIncomingCommand(command);
				}
				break;
			case 8:
			{
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.CountFragmentOpCommand(command.Size);
				}
				if (peerConnectionState != ConnectionStateValue.Connected)
				{
					break;
				}
				if (command.fragmentNumber > command.fragmentCount || command.fragmentOffset >= command.totalLength || command.fragmentOffset + command.Payload.Length > command.totalLength)
				{
					if ((int)base.debugOut >= 1)
					{
						base.Listener.DebugReturn(DebugLevel.ERROR, "Received fragment has bad size: " + command);
					}
					break;
				}
				flag = QueueIncomingCommand(command);
				if (!flag)
				{
					break;
				}
				EnetChannel channel = GetChannel(command.commandChannelID);
				if (command.reliableSequenceNumber == command.startSequenceNumber)
				{
					command.fragmentsRemaining--;
					int num = command.startSequenceNumber + 1;
					while (command.fragmentsRemaining > 0 && num < command.startSequenceNumber + command.fragmentCount)
					{
						if (channel.ContainsReliableSequenceNumber(num++))
						{
							command.fragmentsRemaining--;
						}
					}
				}
				else if (channel.ContainsReliableSequenceNumber(command.startSequenceNumber))
				{
					NCommand nCommand2 = channel.FetchReliableSequenceNumber(command.startSequenceNumber);
					nCommand2.fragmentsRemaining--;
				}
				break;
			}
			case 3:
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.CountControlCommand(command.Size);
				}
				if (peerConnectionState == ConnectionStateValue.Connecting)
				{
					byte[] payload = PrepareConnectData(base.ServerAddress, AppId, CustomInitData);
					CreateAndEnqueueCommand(6, payload, 0);
					peerConnectionState = ConnectionStateValue.Connected;
				}
				break;
			}
			return flag;
		}

		internal bool QueueIncomingCommand(NCommand command)
		{
			EnetChannel channel = GetChannel(command.commandChannelID);
			if (channel == null)
			{
				if ((int)base.debugOut >= 1)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Received command for non-existing channel: " + command.commandChannelID);
				}
				return false;
			}
			if ((int)base.debugOut >= 5)
			{
				base.Listener.DebugReturn(DebugLevel.ALL, string.Concat("queueIncomingCommand() ", command, " channel seq# r/u: ", channel.incomingReliableSequenceNumber, "/", channel.incomingUnreliableSequenceNumber));
			}
			if (command.commandFlags == 1)
			{
				if (command.reliableSequenceNumber <= channel.incomingReliableSequenceNumber)
				{
					if ((int)base.debugOut >= 3)
					{
						base.Listener.DebugReturn(DebugLevel.INFO, string.Concat("incoming command ", command, " is old (not saving it). Dispatched incomingReliableSequenceNumber: ", channel.incomingReliableSequenceNumber));
					}
					return false;
				}
				if (channel.ContainsReliableSequenceNumber(command.reliableSequenceNumber))
				{
					if ((int)base.debugOut >= 3)
					{
						base.Listener.DebugReturn(DebugLevel.INFO, string.Concat("Info: command was received before! Old/New: ", channel.FetchReliableSequenceNumber(command.reliableSequenceNumber), "/", command, " inReliableSeq#: ", channel.incomingReliableSequenceNumber));
					}
					return false;
				}
				channel.incomingReliableCommandsList.Add(command.reliableSequenceNumber, command);
				return true;
			}
			if (command.commandFlags == 0)
			{
				if (command.reliableSequenceNumber < channel.incomingReliableSequenceNumber)
				{
					if ((int)base.debugOut >= 3)
					{
						base.Listener.DebugReturn(DebugLevel.INFO, "incoming reliable-seq# < Dispatched-rel-seq#. not saved.");
					}
					return true;
				}
				if (command.unreliableSequenceNumber <= channel.incomingUnreliableSequenceNumber)
				{
					if ((int)base.debugOut >= 3)
					{
						base.Listener.DebugReturn(DebugLevel.INFO, "incoming unreliable-seq# < Dispatched-unrel-seq#. not saved.");
					}
					return true;
				}
				if (channel.ContainsUnreliableSequenceNumber(command.unreliableSequenceNumber))
				{
					if ((int)base.debugOut >= 3)
					{
						base.Listener.DebugReturn(DebugLevel.INFO, string.Concat("command was received before! Old/New: ", channel.incomingUnreliableCommandsList[command.unreliableSequenceNumber], "/", command));
					}
					return false;
				}
				channel.incomingUnreliableCommandsList.Add(command.unreliableSequenceNumber, command);
				return true;
			}
			return false;
		}

		internal NCommand RemoveSentReliableCommand(int ackReceivedReliableSequenceNumber, int ackReceivedChannel)
		{
			NCommand nCommand = null;
			lock (sentReliableCommands)
			{
				foreach (NCommand sentReliableCommand in sentReliableCommands)
				{
					if (sentReliableCommand != null && sentReliableCommand.reliableSequenceNumber == ackReceivedReliableSequenceNumber && sentReliableCommand.commandChannelID == ackReceivedChannel)
					{
						nCommand = sentReliableCommand;
						break;
					}
				}
				if (nCommand != null)
				{
					sentReliableCommands.Remove(nCommand);
					if (sentReliableCommands.Count > 0)
					{
						timeoutInt = timeInt + 25;
					}
				}
				else if ((int)base.debugOut >= 5 && peerConnectionState != ConnectionStateValue.Connected && peerConnectionState != ConnectionStateValue.Disconnecting)
				{
					EnqueueDebugReturn(DebugLevel.ALL, string.Format("No sent command for ACK (Ch: {0} Sq#: {1}). PeerState: {2}.", ackReceivedReliableSequenceNumber, ackReceivedChannel, peerConnectionState));
				}
			}
			return nCommand;
		}

		internal string CommandListToString(NCommand[] list)
		{
			if ((int)base.debugOut < 5)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < list.Length; i++)
			{
				stringBuilder.Append(i + "=");
				stringBuilder.Append(list[i]);
				stringBuilder.Append(" # ");
			}
			return stringBuilder.ToString();
		}
	}
}
