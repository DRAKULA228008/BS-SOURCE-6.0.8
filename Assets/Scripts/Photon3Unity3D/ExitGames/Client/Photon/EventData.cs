using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public class EventData
	{
		public byte Code;

		public Dictionary<byte, object> Parameters;

		private static byte SenderKey = 254;

		private int sender = -1;

		private static byte CustomDataKey = 245;

		public int Sender
		{
			get
			{
				if (sender >= 0)
				{
					return sender;
				}
				sender = 0;
				object obj = this[SenderKey];
				if (obj is int)
				{
					sender = (int)obj;
				}
				return sender;
			}
		}

		public object CustomData
		{
			get
			{
				return this[CustomDataKey];
			}
		}

		public object this[byte key]
		{
			get
			{
				object value;
				Parameters.TryGetValue(key, out value);
				return value;
			}
			set
			{
				Parameters[key] = value;
			}
		}

		public override string ToString()
		{
			return string.Format("Event {0}.", Code.ToString());
		}

		public string ToStringFull()
		{
			return string.Format("Event {0}: {1}", Code, SupportClass.DictionaryToString(Parameters));
		}
	}
}
