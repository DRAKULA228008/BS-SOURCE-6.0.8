using System;
using System.Collections.Generic;
using System.IO;
using BestHTTP.Extensions;
using BestHTTP.ServerSentEvents;

namespace BestHTTP
{
	public static class HTTPProtocolFactory
	{
		public static HTTPResponse Get(SupportedProtocols protocol, HTTPRequest request, Stream stream, bool isStreamed, bool isFromCache)
		{
			if (protocol == SupportedProtocols.ServerSentEvents)
			{
				return new EventSourceResponse(request, stream, isStreamed, isFromCache);
			}
			return new HTTPResponse(request, new ReadOnlyBufferedStream(stream), isStreamed, isFromCache);
		}

		public static SupportedProtocols GetProtocolFromUri(Uri uri)
		{
			if (uri == null || uri.Scheme == null)
			{
				throw new Exception("Malformed URI in GetProtocolFromUri");
			}
			string text = uri.Scheme.ToLowerInvariant();
			string text2 = text;
			if (text2 != null)
			{
				//if (_003C_003Ef__switch_0024map6 == null)
				{
					//_003C_003Ef__switch_0024map6 = new Dictionary<string, int>(0);
				}
				int value;
				//if (!_003C_003Ef__switch_0024map6.TryGetValue(text2, out value))
				{
				}
			}
			return SupportedProtocols.HTTP;
		}

		public static bool IsSecureProtocol(Uri uri)
		{
			if (uri == null || uri.Scheme == null)
			{
				throw new Exception("Malformed URI in IsSecureProtocol");
			}
			switch (uri.Scheme.ToLowerInvariant())
			{
			case "https":
				return true;
			default:
				return false;
			}
		}
	}
}
