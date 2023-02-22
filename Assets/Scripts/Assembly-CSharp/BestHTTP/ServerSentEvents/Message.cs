using System;

namespace BestHTTP.ServerSentEvents
{
	public sealed class Message
	{
		public string Id { get; internal set; }

		public string Event { get; internal set; }

		public string Data { get; internal set; }

		public TimeSpan Retry { get; internal set; }

		public override string ToString()
		{
			return string.Format("\"{0}\": \"{1}\"", Event, Data);
		}
	}
}
