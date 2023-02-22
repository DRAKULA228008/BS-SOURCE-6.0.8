using System.Collections.Generic;

namespace BestHTTP.Extensions
{
	public sealed class WWWAuthenticateHeaderParser : KeyValuePairList
	{
		public WWWAuthenticateHeaderParser(string headerValue)
		{
			base.Values = ParseQuotedHeader(headerValue);
		}

		private List<HeaderValue> ParseQuotedHeader(string str)
		{
			List<HeaderValue> list = new List<HeaderValue>();
			if (str != null)
			{
				int pos = 0;
				string key = str.Read(ref pos, (char ch) => !char.IsWhiteSpace(ch) && !char.IsControl(ch)).TrimAndLower();
				list.Add(new HeaderValue(key));
				while (pos < str.Length)
				{
					string key2 = str.Read(ref pos, '=').TrimAndLower();
					HeaderValue headerValue = new HeaderValue(key2);
					str.SkipWhiteSpace(ref pos);
					headerValue.Value = str.ReadPossibleQuotedText(ref pos);
					list.Add(headerValue);
				}
			}
			return list;
		}
	}
}
