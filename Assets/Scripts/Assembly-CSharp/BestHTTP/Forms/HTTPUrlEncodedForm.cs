using System;
using System.Text;

namespace BestHTTP.Forms
{
	public sealed class HTTPUrlEncodedForm : HTTPFormBase
	{
		private const int EscapeTreshold = 256;

		private byte[] CachedData;

		public override void PrepareRequest(HTTPRequest request)
		{
			request.SetHeader("Content-Type", "application/x-www-form-urlencoded");
		}

		public override byte[] GetData()
		{
			if (CachedData != null && !base.IsChanged)
			{
				return CachedData;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < base.Fields.Count; i++)
			{
				HTTPFieldData hTTPFieldData = base.Fields[i];
				if (i > 0)
				{
					stringBuilder.Append("&");
				}
				stringBuilder.Append(EscapeString(hTTPFieldData.Name));
				stringBuilder.Append("=");
				if (!string.IsNullOrEmpty(hTTPFieldData.Text) || hTTPFieldData.Binary == null)
				{
					stringBuilder.Append(EscapeString(hTTPFieldData.Text));
				}
				else
				{
					stringBuilder.Append(EscapeString(Encoding.UTF8.GetString(hTTPFieldData.Binary, 0, hTTPFieldData.Binary.Length)));
				}
			}
			base.IsChanged = false;
			return CachedData = Encoding.UTF8.GetBytes(stringBuilder.ToString());
		}

		public static string EscapeString(string originalString)
		{
			if (originalString.Length < 256)
			{
				return Uri.EscapeDataString(originalString);
			}
			int num = originalString.Length / 256;
			StringBuilder stringBuilder = new StringBuilder(num);
			for (int i = 0; i <= num; i++)
			{
				stringBuilder.Append((i >= num) ? Uri.EscapeDataString(originalString.Substring(256 * i)) : Uri.EscapeDataString(originalString.Substring(256 * i, 256)));
			}
			return stringBuilder.ToString();
		}
	}
}
