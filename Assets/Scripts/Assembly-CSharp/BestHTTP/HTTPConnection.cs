using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using BestHTTP.Authentication;
using BestHTTP.Logger;
using BestHTTP.PlatformSupport.TcpClient.General;

namespace BestHTTP
{
	internal sealed class HTTPConnection : ConnectionBase
	{
		private TcpClient Client;

		private Stream Stream;

		private KeepAliveHeader KeepAlive;

		public override bool IsRemovable
		{
			get
			{
				if (base.IsRemovable)
				{
					return true;
				}
				if (base.IsFree && KeepAlive != null && DateTime.UtcNow - LastProcessTime >= KeepAlive.TimeOut)
				{
					return true;
				}
				return false;
			}
		}

		internal HTTPConnection(string serverAddress)
			: base(serverAddress)
		{
		}

		protected override void ThreadFunc(object param)
		{
			bool flag = false;
			bool flag2 = false;
			RetryCauses retryCauses = RetryCauses.None;
			try
			{
				if (Client != null && !Client.IsConnected())
				{
					Close();
				}
				do
				{
					if (retryCauses == RetryCauses.Reconnect)
					{
						Close();
						Thread.Sleep(100);
					}
					base.LastProcessedUri = base.CurrentRequest.CurrentUri;
					retryCauses = RetryCauses.None;
					Connect();
					if (base.State == HTTPConnectionStates.AbortRequested)
					{
						throw new Exception("AbortRequested");
					}
					bool flag3 = false;
					try
					{
						Client.NoDelay = base.CurrentRequest.TryToMinimizeTCPLatency;
						base.CurrentRequest.SendOutTo(Stream);
						flag3 = true;
					}
					catch (Exception ex)
					{
						Close();
						if (base.State == HTTPConnectionStates.TimedOut || base.State == HTTPConnectionStates.AbortRequested)
						{
							throw new Exception("AbortRequested");
						}
						if (flag || base.CurrentRequest.DisableRetry)
						{
							throw ex;
						}
						flag = true;
						retryCauses = RetryCauses.Reconnect;
					}
					if (!flag3)
					{
						continue;
					}
					bool flag4 = Receive();
					if (base.State == HTTPConnectionStates.TimedOut || base.State == HTTPConnectionStates.AbortRequested)
					{
						throw new Exception("AbortRequested");
					}
					if (!flag4 && !flag && !base.CurrentRequest.DisableRetry)
					{
						flag = true;
						retryCauses = RetryCauses.Reconnect;
					}
					if (base.CurrentRequest.Response == null)
					{
						continue;
					}
					switch (base.CurrentRequest.Response.StatusCode)
					{
					case 401:
					{
						string text = DigestStore.FindBest(base.CurrentRequest.Response.GetHeaderValues("www-authenticate"));
						if (!string.IsNullOrEmpty(text))
						{
							Digest orCreate = DigestStore.GetOrCreate(base.CurrentRequest.CurrentUri);
							orCreate.ParseChallange(text);
							if (base.CurrentRequest.Credentials != null && orCreate.IsUriProtected(base.CurrentRequest.CurrentUri) && (!base.CurrentRequest.HasHeader("Authorization") || orCreate.Stale))
							{
								retryCauses = RetryCauses.Authenticate;
							}
						}
						break;
					}
					case 301:
					case 302:
					case 307:
					case 308:
						if (base.CurrentRequest.RedirectCount < base.CurrentRequest.MaxRedirects)
						{
							base.CurrentRequest.RedirectCount++;
							string firstHeaderValue = base.CurrentRequest.Response.GetFirstHeaderValue("location");
							if (string.IsNullOrEmpty(firstHeaderValue))
							{
								throw new MissingFieldException(string.Format("Got redirect status({0}) without 'location' header!", base.CurrentRequest.Response.StatusCode.ToString()));
							}
							Uri redirectUri = GetRedirectUri(firstHeaderValue);
							if (HTTPManager.Logger.Level == Loglevels.All)
							{
								HTTPManager.Logger.Verbose("HTTPConnection", string.Format("{0} - Redirected to Location: '{1}' redirectUri: '{1}'", base.CurrentRequest.CurrentUri.ToString(), firstHeaderValue, redirectUri));
							}
							if (!base.CurrentRequest.CallOnBeforeRedirection(redirectUri))
							{
								HTTPManager.Logger.Information("HTTPConnection", "OnBeforeRedirection returned False");
								break;
							}
							base.CurrentRequest.RemoveHeader("Host");
							base.CurrentRequest.SetHeader("Referer", base.CurrentRequest.CurrentUri.ToString());
							base.CurrentRequest.RedirectUri = redirectUri;
							base.CurrentRequest.Response = null;
							bool flag5 = true;
							base.CurrentRequest.IsRedirected = flag5;
							flag2 = flag5;
						}
						break;
					}
					if (base.CurrentRequest.Response != null && base.CurrentRequest.Response.IsClosedManually)
					{
						continue;
					}
					bool flag6 = base.CurrentRequest.Response == null || base.CurrentRequest.Response.HasHeaderWithValue("connection", "close");
					bool flag7 = !base.CurrentRequest.IsKeepAlive;
					if (flag6 || flag7)
					{
						Close();
					}
					else
					{
						if (base.CurrentRequest.Response == null)
						{
							continue;
						}
						List<string> headerValues = base.CurrentRequest.Response.GetHeaderValues("keep-alive");
						if (headerValues != null && headerValues.Count > 0)
						{
							if (KeepAlive == null)
							{
								KeepAlive = new KeepAliveHeader();
							}
							KeepAlive.Parse(headerValues);
						}
					}
				}
				while (retryCauses != 0);
			}
			catch (TimeoutException exception)
			{
				base.CurrentRequest.Response = null;
				base.CurrentRequest.Exception = exception;
				base.CurrentRequest.State = HTTPRequestStates.ConnectionTimedOut;
				Close();
			}
			catch (Exception exception2)
			{
				if (base.CurrentRequest != null)
				{
					base.CurrentRequest.Response = null;
					switch (base.State)
					{
					case HTTPConnectionStates.AbortRequested:
					case HTTPConnectionStates.Closed:
						base.CurrentRequest.State = HTTPRequestStates.Aborted;
						break;
					case HTTPConnectionStates.TimedOut:
						base.CurrentRequest.State = HTTPRequestStates.TimedOut;
						break;
					default:
						base.CurrentRequest.Exception = exception2;
						base.CurrentRequest.State = HTTPRequestStates.Error;
						break;
					}
				}
				Close();
			}
			finally
			{
				if (base.CurrentRequest != null)
				{
					lock (HTTPManager.Locker)
					{
						if (base.CurrentRequest != null && base.CurrentRequest.Response != null && base.CurrentRequest.Response.IsUpgraded)
						{
							base.State = HTTPConnectionStates.Upgraded;
						}
						else
						{
							base.State = (flag2 ? HTTPConnectionStates.Redirected : ((Client != null) ? HTTPConnectionStates.WaitForRecycle : HTTPConnectionStates.Closed));
						}
						if (base.CurrentRequest.State == HTTPRequestStates.Processing && (base.State == HTTPConnectionStates.Closed || base.State == HTTPConnectionStates.WaitForRecycle))
						{
							if (base.CurrentRequest.Response != null)
							{
								base.CurrentRequest.State = HTTPRequestStates.Finished;
							}
							else
							{
								base.CurrentRequest.Exception = new Exception(string.Format("Remote server closed the connection before sending response header! Previous request state: {0}. Connection state: {1}", base.CurrentRequest.State.ToString(), base.State.ToString()));
								base.CurrentRequest.State = HTTPRequestStates.Error;
							}
						}
						if (base.CurrentRequest.State == HTTPRequestStates.ConnectionTimedOut)
						{
							base.State = HTTPConnectionStates.Closed;
						}
						LastProcessTime = DateTime.UtcNow;
						if (OnConnectionRecycled != null)
						{
							RecycleNow();
						}
					}
				}
			}
		}

		private void Connect()
		{
			Uri currentUri = base.CurrentRequest.CurrentUri;
			if (Client == null)
			{
				Client = new TcpClient();
			}
			if (!Client.Connected)
			{
				Client.ConnectTimeout = base.CurrentRequest.ConnectTimeout;
				if (HTTPManager.Logger.Level == Loglevels.All)
				{
					HTTPManager.Logger.Verbose("HTTPConnection", string.Format("'{0}' - Connecting to {1}:{2}", base.CurrentRequest.CurrentUri.ToString(), currentUri.Host, currentUri.Port.ToString()));
				}
				Client.SendBufferSize = HTTPManager.SendBufferSize;
				Client.ReceiveBufferSize = HTTPManager.ReceiveBufferSize;
				if (HTTPManager.Logger.Level == Loglevels.All)
				{
					HTTPManager.Logger.Verbose("HTTPConnection", string.Format("'{0}' - Buffer sizes - Send: {1} Receive: {2} Blocking: {3}", base.CurrentRequest.CurrentUri.ToString(), Client.SendBufferSize.ToString(), Client.ReceiveBufferSize.ToString(), Client.Client.Blocking.ToString()));
				}
				Client.Connect(currentUri.Host, currentUri.Port);
				if ((int)HTTPManager.Logger.Level <= 1)
				{
					HTTPManager.Logger.Information("HTTPConnection", "Connected to " + currentUri.Host + ":" + currentUri.Port);
				}
			}
			else if ((int)HTTPManager.Logger.Level <= 1)
			{
				HTTPManager.Logger.Information("HTTPConnection", "Already connected to " + currentUri.Host + ":" + currentUri.Port);
			}
			base.StartTime = DateTime.UtcNow;
			if (Stream != null)
			{
				return;
			}
			bool flag = HTTPProtocolFactory.IsSecureProtocol(base.CurrentRequest.CurrentUri);
			Stream = Client.GetStream();
			if (flag)
			{
				SslStream sslStream = new SslStream(Client.GetStream(), false, (object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors) => base.CurrentRequest.CallCustomCertificationValidator(cert, chain));
				if (!sslStream.IsAuthenticated)
				{
					sslStream.AuthenticateAsClient(base.CurrentRequest.CurrentUri.Host);
				}
				Stream = sslStream;
			}
		}

		private bool Receive()
		{
			SupportedProtocols supportedProtocols = ((base.CurrentRequest.ProtocolHandler != 0) ? base.CurrentRequest.ProtocolHandler : HTTPProtocolFactory.GetProtocolFromUri(base.CurrentRequest.CurrentUri));
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				HTTPManager.Logger.Verbose("HTTPConnection", string.Format("{0} - Receive - protocol: {1}", base.CurrentRequest.CurrentUri.ToString(), supportedProtocols.ToString()));
			}
			base.CurrentRequest.Response = HTTPProtocolFactory.Get(supportedProtocols, base.CurrentRequest, Stream, base.CurrentRequest.UseStreaming, false);
			if (!base.CurrentRequest.Response.Receive())
			{
				if (HTTPManager.Logger.Level == Loglevels.All)
				{
					HTTPManager.Logger.Verbose("HTTPConnection", string.Format("{0} - Receive - Failed! Response will be null, returning with false.", base.CurrentRequest.CurrentUri.ToString()));
				}
				base.CurrentRequest.Response = null;
				return false;
			}
			if (base.CurrentRequest.Response.StatusCode == 304)
			{
				return false;
			}
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				HTTPManager.Logger.Verbose("HTTPConnection", string.Format("{0} - Receive - Finished Successfully!", base.CurrentRequest.CurrentUri.ToString()));
			}
			return true;
		}

		private Uri GetRedirectUri(string location)
		{
			Uri uri = null;
			try
			{
				uri = new Uri(location);
				if (uri.IsFile || uri.AbsolutePath == location)
				{
					uri = null;
				}
			}
			catch (UriFormatException)
			{
				uri = null;
			}
			if (uri == null)
			{
				Uri uri2 = base.CurrentRequest.Uri;
				UriBuilder uriBuilder = new UriBuilder(uri2.Scheme, uri2.Host, uri2.Port, location);
				uri = uriBuilder.Uri;
			}
			return uri;
		}

		internal override void Abort(HTTPConnectionStates newState)
		{
			base.State = newState;
			HTTPConnectionStates state = base.State;
			if (state == HTTPConnectionStates.TimedOut)
			{
				base.TimedOutStart = DateTime.UtcNow;
			}
			if (Stream != null)
			{
				try
				{
					Stream.Dispose();
				}
				catch
				{
				}
			}
		}

		private void Close()
		{
			KeepAlive = null;
			base.LastProcessedUri = null;
			if (Client == null)
			{
				return;
			}
			try
			{
				Client.Close();
			}
			catch
			{
			}
			finally
			{
				Stream = null;
				Client = null;
			}
		}

		protected override void Dispose(bool disposing)
		{
			Close();
			base.Dispose(disposing);
		}
	}
}
