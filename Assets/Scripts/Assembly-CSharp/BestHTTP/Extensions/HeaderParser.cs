using System;
using System.Collections.Generic;

namespace BestHTTP.Extensions
{
	public sealed class HeaderParser : KeyValuePairList
	{
		public HeaderParser(string headerStr)
		{
			base.Values = Parse(headerStr);
		}

		private List<HeaderValue> Parse(string headerStr)
		{
			List<HeaderValue> list = new List<HeaderValue>();
			int pos = 0;
			try
			{
				while (pos < headerStr.Length)
				{
					HeaderValue headerValue = new HeaderValue();
					headerValue.Parse(headerStr, ref pos);
					list.Add(headerValue);
				}
				return list;
			}
			catch (Exception ex)
			{
				HTTPManager.Logger.Exception("HeaderParser - Parse", headerStr, ex);
				return list;
			}
		}
	}
}
