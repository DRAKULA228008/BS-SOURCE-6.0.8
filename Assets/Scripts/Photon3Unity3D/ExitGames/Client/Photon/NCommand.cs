using System;

namespace ExitGames.Client.Photon
{
	internal class NCommand : IComparable<NCommand>
	{
		internal byte commandFlags;

		internal const int FLAG_RELIABLE = 1;

		internal const int FLAG_UNSEQUENCED = 2;

		internal const byte FV_UNRELIABLE = 0;

		internal const byte FV_RELIABLE = 1;

		internal const byte FV_UNRELIBALE_UNSEQUENCED = 2;

		internal byte commandType;

		internal const byte CT_NONE = 0;

		internal const byte CT_ACK = 1;

		internal const byte CT_CONNECT = 2;

		internal const byte CT_VERIFYCONNECT = 3;

		internal const byte CT_DISCONNECT = 4;

		internal const byte CT_PING = 5;

		internal const byte CT_SENDRELIABLE = 6;

		internal const byte CT_SENDUNRELIABLE = 7;

		internal const byte CT_SENDFRAGMENT = 8;

		internal const byte CT_EG_SERVERTIME = 12;

		internal byte commandChannelID;

		internal int reliableSequenceNumber;

		internal int unreliableSequenceNumber;

		internal int unsequencedGroupNumber;

		internal byte reservedByte = 4;

		internal int startSequenceNumber;

		internal int fragmentCount;

		internal int fragmentNumber;

		internal int totalLength;

		internal int fragmentOffset;

		internal int fragmentsRemaining;

		internal int commandSentTime;

		internal byte commandSentCount;

		internal int roundTripTimeout;

		internal int timeoutTime;

		internal int ackReceivedReliableSequenceNumber;

		internal int ackReceivedSentTime;

		internal const int HEADER_UDP_PACK_LENGTH = 12;

		internal const int CmdSizeMinimum = 12;

		internal const int CmdSizeAck = 20;

		internal const int CmdSizeConnect = 44;

		internal const int CmdSizeVerifyConnect = 44;

		internal const int CmdSizeDisconnect = 12;

		internal const int CmdSizePing = 12;

		internal const int CmdSizeReliableHeader = 12;

		internal const int CmdSizeUnreliableHeader = 16;

		internal const int CmdSizeFragmentHeader = 32;

		internal const int CmdSizeMaxHeader = 36;

		internal int Size;

		private byte[] commandHeader;

		internal int SizeOfHeader;

		internal byte[] Payload;

		protected internal int SizeOfPayload
		{
			get
			{
				return (Payload != null) ? Payload.Length : 0;
			}
		}

		internal NCommand(EnetPeer peer, byte commandType, byte[] payload, byte channel)
		{
			this.commandType = commandType;
			commandFlags = 1;
			commandChannelID = channel;
			Payload = payload;
			Size = 12;
			switch (this.commandType)
			{
			case 2:
			{
				Size = 44;
				Payload = new byte[32];
				Payload[0] = 0;
				Payload[1] = 0;
				int targetOffset = 2;
				Protocol.Serialize((short)peer.mtu, Payload, ref targetOffset);
				Payload[4] = 0;
				Payload[5] = 0;
				Payload[6] = 128;
				Payload[7] = 0;
				Payload[11] = peer.ChannelCount;
				Payload[15] = 0;
				Payload[19] = 0;
				Payload[22] = 19;
				Payload[23] = 136;
				Payload[27] = 2;
				Payload[31] = 2;
				break;
			}
			case 4:
				Size = 12;
				if (peer.peerConnectionState != PeerBase.ConnectionStateValue.Connected)
				{
					commandFlags = 2;
					if (peer.peerConnectionState == PeerBase.ConnectionStateValue.Zombie)
					{
						reservedByte = 2;
					}
				}
				break;
			case 6:
				Size = 12 + payload.Length;
				break;
			case 7:
				Size = 16 + payload.Length;
				commandFlags = 0;
				break;
			case 8:
				Size = 32 + payload.Length;
				break;
			case 3:
			case 5:
				break;
			}
		}

		internal static void CreateAck(byte[] buffer, int offset, NCommand commandToAck, int sentTime)
		{
			buffer[offset++] = 1;
			buffer[offset++] = commandToAck.commandChannelID;
			buffer[offset++] = 0;
			buffer[offset++] = commandToAck.reservedByte;
			Protocol.Serialize(20, buffer, ref offset);
			Protocol.Serialize(commandToAck.reliableSequenceNumber, buffer, ref offset);
			Protocol.Serialize(commandToAck.reliableSequenceNumber, buffer, ref offset);
			Protocol.Serialize(sentTime, buffer, ref offset);
		}

		internal NCommand(EnetPeer peer, byte[] inBuff, ref int readingOffset)
		{
			commandType = inBuff[readingOffset++];
			commandChannelID = inBuff[readingOffset++];
			commandFlags = inBuff[readingOffset++];
			reservedByte = inBuff[readingOffset++];
			Protocol.Deserialize(out Size, inBuff, ref readingOffset);
			Protocol.Deserialize(out reliableSequenceNumber, inBuff, ref readingOffset);
			peer.bytesIn += Size;
			switch (commandType)
			{
			case 1:
				Protocol.Deserialize(out ackReceivedReliableSequenceNumber, inBuff, ref readingOffset);
				Protocol.Deserialize(out ackReceivedSentTime, inBuff, ref readingOffset);
				break;
			case 6:
				Payload = new byte[Size - 12];
				break;
			case 7:
				Protocol.Deserialize(out unreliableSequenceNumber, inBuff, ref readingOffset);
				Payload = new byte[Size - 16];
				break;
			case 8:
				Protocol.Deserialize(out startSequenceNumber, inBuff, ref readingOffset);
				Protocol.Deserialize(out fragmentCount, inBuff, ref readingOffset);
				Protocol.Deserialize(out fragmentNumber, inBuff, ref readingOffset);
				Protocol.Deserialize(out totalLength, inBuff, ref readingOffset);
				Protocol.Deserialize(out fragmentOffset, inBuff, ref readingOffset);
				Payload = new byte[Size - 32];
				fragmentsRemaining = fragmentCount;
				break;
			case 3:
			{
				short value;
				Protocol.Deserialize(out value, inBuff, ref readingOffset);
				readingOffset += 30;
				if (peer.peerID == -1 || peer.peerID == -2)
				{
					peer.peerID = value;
				}
				break;
			}
			}
			if (Payload != null)
			{
				Buffer.BlockCopy(inBuff, readingOffset, Payload, 0, Payload.Length);
				readingOffset += Payload.Length;
			}
		}

		internal void SerializeHeader(byte[] buffer, ref int bufferIndex)
		{
			if (commandHeader == null)
			{
				SizeOfHeader = 12;
				if (commandType == 7)
				{
					SizeOfHeader = 16;
				}
				else if (commandType == 8)
				{
					SizeOfHeader = 32;
				}
				buffer[bufferIndex++] = commandType;
				buffer[bufferIndex++] = commandChannelID;
				buffer[bufferIndex++] = commandFlags;
				buffer[bufferIndex++] = reservedByte;
				Protocol.Serialize(Size, buffer, ref bufferIndex);
				Protocol.Serialize(reliableSequenceNumber, buffer, ref bufferIndex);
				if (commandType == 7)
				{
					Protocol.Serialize(unreliableSequenceNumber, buffer, ref bufferIndex);
				}
				else if (commandType == 8)
				{
					Protocol.Serialize(startSequenceNumber, buffer, ref bufferIndex);
					Protocol.Serialize(fragmentCount, buffer, ref bufferIndex);
					Protocol.Serialize(fragmentNumber, buffer, ref bufferIndex);
					Protocol.Serialize(totalLength, buffer, ref bufferIndex);
					Protocol.Serialize(fragmentOffset, buffer, ref bufferIndex);
				}
			}
		}

		internal byte[] Serialize()
		{
			return Payload;
		}

		public int CompareTo(NCommand other)
		{
			if (((uint)commandFlags & (true ? 1u : 0u)) != 0)
			{
				return reliableSequenceNumber - other.reliableSequenceNumber;
			}
			return unreliableSequenceNumber - other.unreliableSequenceNumber;
		}

		public override string ToString()
		{
			if (commandType == 1)
			{
				return string.Format("CMD({1} ack for c#:{0} s#/time {2}/{3})", commandChannelID, commandType, ackReceivedReliableSequenceNumber, ackReceivedSentTime);
			}
			return string.Format("CMD({1} c#:{0} r/u: {2}/{3} st/r#/rt:{4}/{5}/{6})", commandChannelID, commandType, reliableSequenceNumber, unreliableSequenceNumber, commandSentTime, commandSentCount, timeoutTime);
		}
	}
}
