using System;
using System.Collections.Generic;
using BestHTTP.Extensions;

namespace BestHTTP
{
	internal sealed class KeepAliveHeader
	{
		public TimeSpan TimeOut { get; private set; }

		public int MaxRequests { get; private set; }

		public void Parse(List<string> headerValues)
		{
			HeaderParser headerParser = new HeaderParser(headerValues[0]);
			HeaderValue param;
			if (headerParser.TryGet("timeout", out param) && param.HasValue)
			{
				int result = 0;
				if (int.TryParse(param.Value, out result))
				{
					TimeOut = TimeSpan.FromSeconds(result);
				}
				else
				{
					TimeOut = TimeSpan.MaxValue;
				}
			}
			if (headerParser.TryGet("max", out param) && param.HasValue)
			{
				int result2 = 0;
				if (int.TryParse("max", out result2))
				{
					MaxRequests = result2;
				}
				else
				{
					MaxRequests = int.MaxValue;
				}
			}
		}
	}
}
