using System;
using System.Collections.Generic;
using BestHTTP.Extensions;

namespace BestHTTP.ServerSentEvents
{
	public class EventSource : IHeartbeat
	{
		private States _state;

		private Dictionary<string, OnEventDelegate> EventTable;

		private byte RetryCount;

		private DateTime RetryCalled;

		public Uri Uri { get; private set; }

		public States State
		{
			get
			{
				return _state;
			}
			private set
			{
				States state = _state;
				_state = value;
				if (this.OnStateChanged != null)
				{
					try
					{
						this.OnStateChanged(this, state, _state);
					}
					catch (Exception ex)
					{
						HTTPManager.Logger.Exception("EventSource", "OnStateChanged", ex);
					}
				}
			}
		}

		public TimeSpan ReconnectionTime { get; set; }

		public string LastEventId { get; private set; }

		public HTTPRequest InternalRequest { get; private set; }

		public event OnGeneralEventDelegate OnOpen;

		public event OnMessageDelegate OnMessage;

		public event OnErrorDelegate OnError;

		public event OnRetryDelegate OnRetry;

		public event OnGeneralEventDelegate OnClosed;

		public event OnStateChangedDelegate OnStateChanged;

		public EventSource(Uri uri)
		{
			Uri = uri;
			ReconnectionTime = TimeSpan.FromMilliseconds(2000.0);
			InternalRequest = new HTTPRequest(Uri, HTTPMethods.Get, true, true, OnRequestFinished);
			InternalRequest.SetHeader("Accept", "text/event-stream");
			InternalRequest.SetHeader("Cache-Control", "no-cache");
			InternalRequest.SetHeader("Accept-Encoding", "identity");
			InternalRequest.ProtocolHandler = SupportedProtocols.ServerSentEvents;
			InternalRequest.OnUpgraded = OnUpgraded;
			InternalRequest.DisableRetry = true;
		}

		void IHeartbeat.OnHeartbeatUpdate(TimeSpan dif)
		{
			if (State != States.Retrying)
			{
				HTTPManager.Heartbeats.Unsubscribe(this);
			}
			else if (DateTime.UtcNow - RetryCalled >= ReconnectionTime)
			{
				Open();
				if (State != States.Connecting)
				{
					SetClosed("OnHeartbeatUpdate");
				}
				HTTPManager.Heartbeats.Unsubscribe(this);
			}
		}

		public void Open()
		{
			if (State == States.Initial || State == States.Retrying || State == States.Closed)
			{
				State = States.Connecting;
				if (!string.IsNullOrEmpty(LastEventId))
				{
					InternalRequest.SetHeader("Last-Event-ID", LastEventId);
				}
				InternalRequest.Send();
			}
		}

		public void Close()
		{
			if (State != States.Closing && State != States.Closed)
			{
				State = States.Closing;
				if (InternalRequest != null)
				{
					InternalRequest.Abort();
				}
				else
				{
					State = States.Closed;
				}
			}
		}

		public void On(string eventName, OnEventDelegate action)
		{
			if (EventTable == null)
			{
				EventTable = new Dictionary<string, OnEventDelegate>();
			}
			EventTable[eventName] = action;
		}

		public void Off(string eventName)
		{
			if (eventName != null && EventTable != null)
			{
				EventTable.Remove(eventName);
			}
		}

		private void CallOnError(string error, string msg)
		{
			if (this.OnError != null)
			{
				try
				{
					this.OnError(this, error);
				}
				catch (Exception ex)
				{
					HTTPManager.Logger.Exception("EventSource", msg + " - OnError", ex);
				}
			}
		}

		private bool CallOnRetry()
		{
			//Discarded unreachable code: IL_001d
			if (this.OnRetry != null)
			{
				try
				{
					return this.OnRetry(this);
				}
				catch (Exception ex)
				{
					HTTPManager.Logger.Exception("EventSource", "CallOnRetry", ex);
				}
			}
			return true;
		}

		private void SetClosed(string msg)
		{
			State = States.Closed;
			if (this.OnClosed != null)
			{
				try
				{
					this.OnClosed(this);
				}
				catch (Exception ex)
				{
					HTTPManager.Logger.Exception("EventSource", msg + " - OnClosed", ex);
				}
			}
		}

		private void Retry()
		{
			if (RetryCount > 0 || !CallOnRetry())
			{
				SetClosed("Retry");
				return;
			}
			RetryCount++;
			RetryCalled = DateTime.UtcNow;
			HTTPManager.Heartbeats.Subscribe(this);
			State = States.Retrying;
		}

		private void OnUpgraded(HTTPRequest originalRequest, HTTPResponse response)
		{
			EventSourceResponse eventSourceResponse = response as EventSourceResponse;
			if (eventSourceResponse == null)
			{
				CallOnError("Not an EventSourceResponse!", "OnUpgraded");
				return;
			}
			if (this.OnOpen != null)
			{
				try
				{
					this.OnOpen(this);
				}
				catch (Exception ex)
				{
					HTTPManager.Logger.Exception("EventSource", "OnOpen", ex);
				}
			}
			eventSourceResponse.OnMessage = (Action<EventSourceResponse, Message>)Delegate.Combine(eventSourceResponse.OnMessage, new Action<EventSourceResponse, Message>(OnMessageReceived));
			eventSourceResponse.StartReceive();
			RetryCount = 0;
			State = States.Open;
		}

		private void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
		{
			if (State == States.Closed)
			{
				return;
			}
			if (State == States.Closing || req.State == HTTPRequestStates.Aborted)
			{
				SetClosed("OnRequestFinished");
				return;
			}
			string text = string.Empty;
			bool flag = true;
			switch (req.State)
			{
			case HTTPRequestStates.Processing:
				flag = !resp.HasHeader("content-length");
				break;
			case HTTPRequestStates.Finished:
				if (resp.StatusCode == 200 && !resp.HasHeaderWithValue("content-type", "text/event-stream"))
				{
					text = "No Content-Type header with value 'text/event-stream' present.";
					flag = false;
				}
				if (flag && resp.StatusCode != 500 && resp.StatusCode != 502 && resp.StatusCode != 503 && resp.StatusCode != 504)
				{
					flag = false;
					text = string.Format("Request Finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", resp.StatusCode, resp.Message, resp.DataAsText);
				}
				break;
			case HTTPRequestStates.Error:
				text = "Request Finished with Error! " + ((req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace));
				break;
			case HTTPRequestStates.Aborted:
				text = "OnRequestFinished - Aborted without request. EventSource's State: " + State;
				break;
			case HTTPRequestStates.ConnectionTimedOut:
				text = "Connection Timed Out!";
				break;
			case HTTPRequestStates.TimedOut:
				text = "Processing the request Timed Out!";
				break;
			}
			if (State < States.Closing)
			{
				if (!string.IsNullOrEmpty(text))
				{
					CallOnError(text, "OnRequestFinished");
				}
				if (flag)
				{
					Retry();
				}
				else
				{
					SetClosed("OnRequestFinished");
				}
			}
			else
			{
				SetClosed("OnRequestFinished");
			}
		}

		private void OnMessageReceived(EventSourceResponse resp, Message message)
		{
			if (State >= States.Closing)
			{
				return;
			}
			if (message.Id != null)
			{
				LastEventId = message.Id;
			}
			if (message.Retry.TotalMilliseconds > 0.0)
			{
				ReconnectionTime = message.Retry;
			}
			if (string.IsNullOrEmpty(message.Data))
			{
				return;
			}
			if (this.OnMessage != null)
			{
				try
				{
					this.OnMessage(this, message);
				}
				catch (Exception ex)
				{
					HTTPManager.Logger.Exception("EventSource", "OnMessageReceived - OnMessage", ex);
				}
			}
			OnEventDelegate value;
			if (EventTable != null && !string.IsNullOrEmpty(message.Event) && EventTable.TryGetValue(message.Event, out value) && value != null)
			{
				try
				{
					value(this, message);
				}
				catch (Exception ex2)
				{
					HTTPManager.Logger.Exception("EventSource", "OnMessageReceived - action", ex2);
				}
			}
		}
	}
}
