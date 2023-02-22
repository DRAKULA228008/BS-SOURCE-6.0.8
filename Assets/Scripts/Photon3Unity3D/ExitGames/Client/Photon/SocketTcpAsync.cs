using System;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading;

namespace ExitGames.Client.Photon
{
	public class SocketTcpAsync : IPhotonSocket, IDisposable
	{
		private class ReceiveContext
		{
			public Socket workSocket;

			public int ReceivedHeaderBytes;

			public byte[] HeaderBuffer;

			public int ExpectedMessageBytes;

			public int ReceivedMessageBytes;

			public byte[] MessageBuffer;

			public bool ReadingHeader
			{
				get
				{
					return ExpectedMessageBytes == 0;
				}
			}

			public bool ReadingMessage
			{
				get
				{
					return ExpectedMessageBytes != 0;
				}
			}

			public byte[] CurrentBuffer
			{
				get
				{
					return ReadingHeader ? HeaderBuffer : MessageBuffer;
				}
			}

			public int CurrentOffset
			{
				get
				{
					return ReadingHeader ? ReceivedHeaderBytes : ReceivedMessageBytes;
				}
			}

			public int CurrentExpected
			{
				get
				{
					return ReadingHeader ? 9 : ExpectedMessageBytes;
				}
			}

			public ReceiveContext(Socket socket, byte[] headerBuffer, byte[] messageBuffer)
			{
				HeaderBuffer = headerBuffer;
				MessageBuffer = messageBuffer;
				workSocket = socket;
			}

			public void Reset()
			{
				ReceivedHeaderBytes = 0;
				ExpectedMessageBytes = 0;
				ReceivedMessageBytes = 0;
			}
		}

		private Socket sock;

		private readonly object syncer = new object();

		public SocketTcpAsync(PeerBase npeer)
			: base(npeer)
		{
			if (ReportDebugOfLevel(DebugLevel.ALL))
			{
				base.Listener.DebugReturn(DebugLevel.ALL, "SocketTcpAsync: TCP, DotNet, Unity.");
			}
			PollReceive = false;
		}

		public override bool Connect()
		{
			if (!base.Connect())
			{
				return false;
			}
			base.State = PhotonSocketState.Connecting;
			Thread thread = new Thread(DnsAndConnect);
			thread.Name = "photon dns thread";
			thread.IsBackground = true;
			thread.Start();
			return true;
		}

		public void DnsAndConnect()
		{
			try
			{
				IPAddress ipAddress = IPhotonSocket.GetIpAddress(base.ServerAddress);
				if (ipAddress == null)
				{
					throw new ArgumentException("SocketTcpAsync. Invalid IPAddress. Address: " + base.ServerAddress);
				}
				sock = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				sock.NoDelay = true;
				sock.ReceiveTimeout = peerBase.DisconnectTimeout;
				sock.SendTimeout = peerBase.DisconnectTimeout;
				sock.Connect(ipAddress, base.ServerPort);
				base.AddressResolvedAsIpv6 = IsIpv6SimpleCheck(ipAddress);
				base.State = PhotonSocketState.Connected;
				peerBase.OnConnect();
			}
			catch (SecurityException ex)
			{
				if (ReportDebugOfLevel(DebugLevel.ERROR))
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "SocketTcpAsync SecurityException. Connect() to '" + base.ServerAddress + "' failed: " + ex.ToString());
				}
				HandleException(StatusCode.SecurityExceptionOnConnect);
				return;
			}
			catch (Exception ex2)
			{
				if (ReportDebugOfLevel(DebugLevel.ERROR))
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "SocketTcpAsync Exception. Connect() to '" + base.ServerAddress + "' failed: " + ex2.ToString());
				}
				HandleException(StatusCode.ExceptionOnConnect);
				return;
			}
			ReceiveAsync();
		}

		public override PhotonSocketError Send(byte[] data, int length)
		{
			if (sock == null || !sock.Connected)
			{
				return PhotonSocketError.Skipped;
			}
			try
			{
				sock.Send(data, 0, length, SocketFlags.None);
			}
			catch (Exception ex)
			{
				if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
				{
					if (ReportDebugOfLevel(DebugLevel.ERROR))
					{
						EnqueueDebugReturn(DebugLevel.ERROR, string.Format("SocketTcpAsync.Send Exception: Cannot send to: {0}. Uptime: {1} ms. {2} {3}", base.ServerAddress, SupportClass.GetTickCount() - peerBase.timeBase, base.AddressResolvedAsIpv6 ? " IPv6" : string.Empty, ex));
					}
					HandleException(StatusCode.Exception);
				}
				return PhotonSocketError.Exception;
			}
			return PhotonSocketError.Success;
		}

		public override PhotonSocketError Receive(out byte[] data)
		{
			data = null;
			return PhotonSocketError.NoData;
		}

		private void ReceiveAsync(ReceiveContext context = null)
		{
			if (context == null)
			{
				context = new ReceiveContext(sock, new byte[9], new byte[base.MTU]);
			}
			try
			{
				sock.BeginReceive(context.CurrentBuffer, context.CurrentOffset, context.CurrentExpected - context.CurrentOffset, SocketFlags.None, ReceiveAsync, context);
			}
			catch (Exception ex)
			{
				if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
				{
					if (ReportDebugOfLevel(DebugLevel.ERROR))
					{
						EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("SocketTcpAsync.ReceiveAsync Exception. State: ", base.State, ". Server: '", base.ServerAddress, "' Exception: ", ex));
					}
					HandleException(StatusCode.ExceptionOnReceive);
				}
			}
		}

		private void ReceiveAsync(IAsyncResult ar)
		{
			if (base.State == PhotonSocketState.Disconnecting || base.State == PhotonSocketState.Disconnected)
			{
				return;
			}
			int num = 0;
			try
			{
				num = sock.EndReceive(ar);
				if (num == 0)
				{
					throw new SocketException(10054);
				}
			}
			catch (SocketException ex)
			{
				if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
				{
					if (ReportDebugOfLevel(DebugLevel.ERROR))
					{
						EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("SocketTcpAsync.EndReceive SocketException. State: ", base.State, ". Server: '", base.ServerAddress, "' ErrorCode: ", ex.ErrorCode, " SocketErrorCode: ", ex.SocketErrorCode, " Message: ", ex.Message, " ", ex));
					}
					HandleException(StatusCode.ExceptionOnReceive);
					return;
				}
			}
			catch (Exception ex2)
			{
				if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
				{
					if (ReportDebugOfLevel(DebugLevel.ERROR))
					{
						EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("SocketTcpAsync.EndReceive Exception. State: ", base.State, ". Server: '", base.ServerAddress, "' Exception: ", ex2));
					}
					HandleException(StatusCode.ExceptionOnReceive);
					return;
				}
			}
			ReceiveContext receiveContext = (ReceiveContext)ar.AsyncState;
			if (num + receiveContext.CurrentOffset != receiveContext.CurrentExpected)
			{
				if (receiveContext.ReadingHeader)
				{
					receiveContext.ReceivedHeaderBytes += num;
				}
				else
				{
					receiveContext.ReceivedMessageBytes += num;
				}
				ReceiveAsync(receiveContext);
			}
			else if (receiveContext.ReadingHeader)
			{
				byte[] headerBuffer = receiveContext.HeaderBuffer;
				if (headerBuffer[0] == 240)
				{
					HandleReceivedDatagram(headerBuffer, headerBuffer.Length, true);
					receiveContext.Reset();
					ReceiveAsync(receiveContext);
					return;
				}
				int num2 = (headerBuffer[1] << 24) | (headerBuffer[2] << 16) | (headerBuffer[3] << 8) | headerBuffer[4];
				receiveContext.ExpectedMessageBytes = num2 - 7;
				if (receiveContext.ExpectedMessageBytes > receiveContext.MessageBuffer.Length)
				{
					receiveContext.MessageBuffer = new byte[receiveContext.ExpectedMessageBytes];
				}
				receiveContext.MessageBuffer[0] = headerBuffer[7];
				receiveContext.MessageBuffer[1] = headerBuffer[8];
				receiveContext.ReceivedMessageBytes = 2;
				ReceiveAsync(receiveContext);
			}
			else
			{
				HandleReceivedDatagram(receiveContext.MessageBuffer, receiveContext.ExpectedMessageBytes, true);
				receiveContext.Reset();
				ReceiveAsync(receiveContext);
			}
		}

		public override bool Disconnect()
		{
			if (ReportDebugOfLevel(DebugLevel.INFO))
			{
				EnqueueDebugReturn(DebugLevel.INFO, "SocketTcpAsync.Disconnect()");
			}
			base.State = PhotonSocketState.Disconnecting;
			lock (syncer)
			{
				if (sock != null)
				{
					try
					{
						sock.Close();
					}
					catch (Exception ex)
					{
						EnqueueDebugReturn(DebugLevel.INFO, "Exception in Disconnect(): " + ex);
					}
					sock = null;
				}
			}
			base.State = PhotonSocketState.Disconnected;
			return true;
		}

		public void Dispose()
		{
			base.State = PhotonSocketState.Disconnecting;
			if (sock != null)
			{
				try
				{
					if (sock.Connected)
					{
						sock.Close();
					}
				}
				catch (Exception ex)
				{
					EnqueueDebugReturn(DebugLevel.INFO, "Exception in Dispose(): " + ex);
				}
			}
			sock = null;
			base.State = PhotonSocketState.Disconnected;
		}
	}
}
