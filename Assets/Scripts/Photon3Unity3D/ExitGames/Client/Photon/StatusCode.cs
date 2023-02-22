using System;

namespace ExitGames.Client.Photon
{
	public enum StatusCode
	{
		Connect = 0x400,
		Disconnect = 1025,
		Exception = 1026,
		ExceptionOnConnect = 0x3FF,
		SecurityExceptionOnConnect = 1022,
		[Obsolete("Check QueuedOutgoingCommands and QueuedIncomingCommands on demand instead.")]
		QueueOutgoingReliableWarning = 1027,
		[Obsolete("Check QueuedOutgoingCommands and QueuedIncomingCommands on demand instead.")]
		QueueOutgoingUnreliableWarning = 1029,
		SendError = 1030,
		[Obsolete("Check QueuedOutgoingCommands and QueuedIncomingCommands on demand instead.")]
		QueueOutgoingAcksWarning = 1031,
		[Obsolete("Check QueuedOutgoingCommands and QueuedIncomingCommands on demand instead.")]
		QueueIncomingReliableWarning = 1033,
		[Obsolete("Check QueuedOutgoingCommands and QueuedIncomingCommands on demand instead.")]
		QueueIncomingUnreliableWarning = 1035,
		[Obsolete("Check QueuedOutgoingCommands and QueuedIncomingCommands on demand instead.")]
		QueueSentWarning = 1037,
		ExceptionOnReceive = 1039,
		TimeoutDisconnect = 1040,
		DisconnectByServer = 1041,
		DisconnectByServerUserLimit = 1042,
		DisconnectByServerLogic = 1043,
		EncryptionEstablished = 1048,
		EncryptionFailedToEstablish = 1049
	}
}
