using BestHTTP.Extensions;

namespace BestHTTP.Forms
{
	public sealed class HTTPMultiPartForm : HTTPFormBase
	{
		private string Boundary;

		private byte[] CachedData;

		public HTTPMultiPartForm()
		{
			Boundary = "BestHTTP_HTTPMultiPartForm_" + GetHashCode().ToString("X");
		}

		public override void PrepareRequest(HTTPRequest request)
		{
			request.SetHeader("Content-Type", "multipart/form-data; boundary=\"" + Boundary + "\"");
		}

		public override byte[] GetData()
		{
			//Discarded unreachable code: IL_014f
			if (CachedData != null)
			{
				return CachedData;
			}
			using (BufferPoolMemoryStream bufferPoolMemoryStream = new BufferPoolMemoryStream())
			{
				for (int i = 0; i < base.Fields.Count; i++)
				{
					HTTPFieldData hTTPFieldData = base.Fields[i];
					bufferPoolMemoryStream.WriteLine("--" + Boundary);
					bufferPoolMemoryStream.WriteLine("Content-Disposition: form-data; name=\"" + hTTPFieldData.Name + "\"" + (string.IsNullOrEmpty(hTTPFieldData.FileName) ? string.Empty : ("; filename=\"" + hTTPFieldData.FileName + "\"")));
					if (!string.IsNullOrEmpty(hTTPFieldData.MimeType))
					{
						bufferPoolMemoryStream.WriteLine("Content-Type: " + hTTPFieldData.MimeType);
					}
					bufferPoolMemoryStream.WriteLine("Content-Length: " + hTTPFieldData.Payload.Length);
					bufferPoolMemoryStream.WriteLine();
					bufferPoolMemoryStream.Write(hTTPFieldData.Payload, 0, hTTPFieldData.Payload.Length);
					bufferPoolMemoryStream.Write(HTTPRequest.EOL, 0, HTTPRequest.EOL.Length);
				}
				bufferPoolMemoryStream.WriteLine("--" + Boundary + "--");
				base.IsChanged = false;
				return CachedData = bufferPoolMemoryStream.ToArray();
			}
		}
	}
}
