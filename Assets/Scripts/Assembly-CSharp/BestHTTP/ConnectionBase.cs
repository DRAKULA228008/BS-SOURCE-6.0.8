using System;
using System.Threading;

namespace BestHTTP
{
	internal abstract class ConnectionBase : IDisposable
	{
		protected DateTime LastProcessTime;

		protected HTTPConnectionRecycledDelegate OnConnectionRecycled;

		private bool IsThreaded;

		public string ServerAddress { get; protected set; }

		public HTTPConnectionStates State { get; protected set; }

		public bool IsFree
		{
			get
			{
				return State == HTTPConnectionStates.Initial || State == HTTPConnectionStates.Free;
			}
		}

		public bool IsActive
		{
			get
			{
				return State > HTTPConnectionStates.Initial && State < HTTPConnectionStates.Free;
			}
		}

		public HTTPRequest CurrentRequest { get; protected set; }

		public virtual bool IsRemovable
		{
			get
			{
				return IsFree && DateTime.UtcNow - LastProcessTime > HTTPManager.MaxConnectionIdleTime;
			}
		}

		public DateTime StartTime { get; protected set; }

		public DateTime TimedOutStart { get; protected set; }

		public Uri LastProcessedUri { get; protected set; }

		protected bool IsDisposed { get; private set; }

		public ConnectionBase(string serverAddress)
			: this(serverAddress, true)
		{
		}

		public ConnectionBase(string serverAddress, bool threaded)
		{
			ServerAddress = serverAddress;
			State = HTTPConnectionStates.Initial;
			LastProcessTime = DateTime.UtcNow;
			IsThreaded = threaded;
		}

		internal abstract void Abort(HTTPConnectionStates hTTPConnectionStates);

		internal void Process(HTTPRequest request)
		{
			if (State == HTTPConnectionStates.Processing)
			{
				throw new Exception("Connection already processing a request!");
			}
			StartTime = DateTime.MaxValue;
			State = HTTPConnectionStates.Processing;
			CurrentRequest = request;
			if (IsThreaded)
			{
				ThreadPool.QueueUserWorkItem(ThreadFunc);
			}
			else
			{
				ThreadFunc(null);
			}
		}

		protected virtual void ThreadFunc(object param)
		{
		}

		internal void HandleProgressCallback()
		{
			if (CurrentRequest.OnProgress != null && CurrentRequest.DownloadProgressChanged)
			{
				try
				{
					CurrentRequest.OnProgress(CurrentRequest, CurrentRequest.Downloaded, CurrentRequest.DownloadLength);
				}
				catch (Exception ex)
				{
					HTTPManager.Logger.Exception("ConnectionBase", "HandleProgressCallback - OnProgress", ex);
				}
				CurrentRequest.DownloadProgressChanged = false;
			}
			if (CurrentRequest.OnUploadProgress != null && CurrentRequest.UploadProgressChanged)
			{
				try
				{
					CurrentRequest.OnUploadProgress(CurrentRequest, CurrentRequest.Uploaded, CurrentRequest.UploadLength);
				}
				catch (Exception ex2)
				{
					HTTPManager.Logger.Exception("ConnectionBase", "HandleProgressCallback - OnUploadProgress", ex2);
				}
				CurrentRequest.UploadProgressChanged = false;
			}
		}

		internal void HandleCallback()
		{
			try
			{
				HandleProgressCallback();
				if (State == HTTPConnectionStates.Upgraded)
				{
					if (CurrentRequest != null && CurrentRequest.Response != null && CurrentRequest.Response.IsUpgraded)
					{
						CurrentRequest.UpgradeCallback();
					}
					State = HTTPConnectionStates.WaitForProtocolShutdown;
				}
				else
				{
					CurrentRequest.CallCallback();
				}
			}
			catch (Exception ex)
			{
				HTTPManager.Logger.Exception("ConnectionBase", "HandleCallback", ex);
			}
		}

		internal void Recycle(HTTPConnectionRecycledDelegate onConnectionRecycled)
		{
			OnConnectionRecycled = onConnectionRecycled;
			if (State <= HTTPConnectionStates.Initial || State >= HTTPConnectionStates.WaitForProtocolShutdown || State == HTTPConnectionStates.Redirected)
			{
				RecycleNow();
			}
		}

		protected void RecycleNow()
		{
			if (State == HTTPConnectionStates.TimedOut || State == HTTPConnectionStates.Closed)
			{
				LastProcessTime = DateTime.MinValue;
			}
			State = HTTPConnectionStates.Free;
			if (CurrentRequest != null)
			{
				CurrentRequest.Dispose();
			}
			CurrentRequest = null;
			if (OnConnectionRecycled != null)
			{
				OnConnectionRecycled(this);
				OnConnectionRecycled = null;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			IsDisposed = true;
		}

		~ConnectionBase()
		{
			Dispose(false);
		}
	}
}
