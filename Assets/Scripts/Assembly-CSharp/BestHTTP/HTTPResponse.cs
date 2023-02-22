using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BestHTTP.Decompression.Zlib;
using BestHTTP.Extensions;
using BestHTTP.Logger;
using UnityEngine;

namespace BestHTTP
{
	public class HTTPResponse : IDisposable
	{
		internal const byte CR = 13;

		internal const byte LF = 10;

		public const int MinBufferSize = 4096;

		private const int MinLengthToDecompress = 256;

		protected string dataAsText;

		protected Texture2D texture;

		internal HTTPRequest baseRequest;

		protected Stream Stream;

		protected List<byte[]> streamedFragments;

		protected object SyncRoot = new object();

		protected byte[] fragmentBuffer;

		protected int fragmentBufferDataLength;

		protected int allFragmentSize;

		private BufferPoolMemoryStream decompressorInputStream;

		private BufferPoolMemoryStream decompressorOutputStream;

		private GZipStream decompressorGZipStream;

		private volatile AutoResetEvent fragmentWaitEvent;

		public int VersionMajor { get; protected set; }

		public int VersionMinor { get; protected set; }

		public int StatusCode { get; protected set; }

		public bool IsSuccess
		{
			get
			{
				return (StatusCode >= 200 && StatusCode < 300) || StatusCode == 304;
			}
		}

		public string Message { get; protected set; }

		public bool IsStreamed { get; protected set; }

		public bool IsStreamingFinished { get; internal set; }

		public Dictionary<string, List<string>> Headers { get; protected set; }

		public byte[] Data { get; internal set; }

		public bool IsUpgraded { get; protected set; }

		public string DataAsText
		{
			get
			{
				if (Data == null)
				{
					return string.Empty;
				}
				if (!string.IsNullOrEmpty(dataAsText))
				{
					return dataAsText;
				}
				return dataAsText = Encoding.UTF8.GetString(Data, 0, Data.Length);
			}
		}

		public Texture2D DataAsTexture2D
		{
			get
			{
				if (Data == null)
				{
					return null;
				}
				if (texture != null)
				{
					return texture;
				}
				texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
				texture.LoadImage(Data);
				return texture;
			}
		}

		public bool IsClosedManually { get; protected set; }

		public HTTPResponse(HTTPRequest request, Stream stream, bool isStreamed, bool isFromCache)
		{
			baseRequest = request;
			Stream = stream;
			IsStreamed = isStreamed;
			IsClosedManually = false;
		}

		public virtual bool Receive(int forceReadRawContentLength = -1, bool readPayloadData = true)
		{
			//Discarded unreachable code: IL_00b1
			string empty = string.Empty;
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				VerboseLogging(string.Format("Receive. forceReadRawContentLength: '{0:N0}', readPayloadData: '{1:N0}'", forceReadRawContentLength, readPayloadData));
			}
			try
			{
				empty = ReadTo(Stream, 32);
			}
			catch
			{
				if (!baseRequest.DisableRetry)
				{
					HTTPManager.Logger.Warning("HTTPResponse", string.Format("{0} - Failed to read Status Line! Retry is enabled, returning with false.", baseRequest.CurrentUri.ToString()));
					return false;
				}
				HTTPManager.Logger.Warning("HTTPResponse", string.Format("{0} - Failed to read Status Line! Retry is disabled, re-throwing exception.", baseRequest.CurrentUri.ToString()));
				throw;
			}
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				VerboseLogging(string.Format("Status Line: '{0}'", empty));
			}
			if (string.IsNullOrEmpty(empty))
			{
				if (!baseRequest.DisableRetry)
				{
					return false;
				}
				throw new Exception("Remote server closed the connection before sending response header!");
			}
			string[] array = empty.Split('/', '.');
			VersionMajor = int.Parse(array[1]);
			VersionMinor = int.Parse(array[2]);
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				VerboseLogging(string.Format("HTTP Version: '{0}.{1}'", VersionMajor.ToString(), VersionMinor.ToString()));
			}
			string text = NoTrimReadTo(Stream, 32, 10);
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				VerboseLogging(string.Format("Status Code: '{0}'", text));
			}
			int result;
			if (baseRequest.DisableRetry)
			{
				result = int.Parse(text);
			}
			else if (!int.TryParse(text, out result))
			{
				return false;
			}
			StatusCode = result;
			if (text.Length > 0 && (byte)text[text.Length - 1] != 10 && (byte)text[text.Length - 1] != 13)
			{
				Message = ReadTo(Stream, 10);
				if (HTTPManager.Logger.Level == Loglevels.All)
				{
					VerboseLogging(string.Format("Status Message: '{0}'", Message));
				}
			}
			else
			{
				HTTPManager.Logger.Warning("HTTPResponse", string.Format("{0} - Skipping Status Message reading!", baseRequest.CurrentUri.ToString()));
				Message = string.Empty;
			}
			ReadHeaders(Stream);
			IsUpgraded = StatusCode == 101 && (HasHeaderWithValue("connection", "upgrade") || HasHeader("upgrade"));
			if (IsUpgraded && HTTPManager.Logger.Level == Loglevels.All)
			{
				VerboseLogging("Request Upgraded!");
			}
			if (!readPayloadData)
			{
				return true;
			}
			return ReadPayload(forceReadRawContentLength);
		}

		protected bool ReadPayload(int forceReadRawContentLength)
		{
			if (forceReadRawContentLength != -1)
			{
				ReadRaw(Stream, forceReadRawContentLength);
				if (HTTPManager.Logger.Level == Loglevels.All)
				{
					VerboseLogging("ReadPayload Finished!");
				}
				return true;
			}
			if ((StatusCode >= 100 && StatusCode < 200) || StatusCode == 204 || StatusCode == 304 || baseRequest.MethodType == HTTPMethods.Head)
			{
				return true;
			}
			if (HasHeaderWithValue("transfer-encoding", "chunked"))
			{
				ReadChunked(Stream);
			}
			else
			{
				List<string> headerValues = GetHeaderValues("content-length");
				List<string> headerValues2 = GetHeaderValues("content-range");
				if (headerValues != null && headerValues2 == null)
				{
					ReadRaw(Stream, long.Parse(headerValues[0]));
				}
				else if (headerValues2 != null)
				{
					if (headerValues != null)
					{
						ReadRaw(Stream, long.Parse(headerValues[0]));
					}
					else
					{
						HTTPRange range = GetRange();
						ReadRaw(Stream, range.LastBytePos - range.FirstBytePos + 1);
					}
				}
				else
				{
					ReadUnknownSize(Stream);
				}
			}
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				VerboseLogging("ReadPayload Finished!");
			}
			return true;
		}

		protected void ReadHeaders(Stream stream)
		{
			string text = ReadTo(stream, 58, 10);
			while (text != string.Empty)
			{
				string text2 = ReadTo(stream, 10);
				if (HTTPManager.Logger.Level == Loglevels.All)
				{
					VerboseLogging(string.Format("Header - '{0}': '{1}'", text, text2));
				}
				AddHeader(text, text2);
				text = ReadTo(stream, 58, 10);
			}
		}

		protected void AddHeader(string name, string value)
		{
			name = name.ToLower();
			if (Headers == null)
			{
				Headers = new Dictionary<string, List<string>>();
			}
			List<string> value2;
			if (!Headers.TryGetValue(name, out value2))
			{
				Headers.Add(name, value2 = new List<string>(1));
			}
			value2.Add(value);
		}

		public List<string> GetHeaderValues(string name)
		{
			if (Headers == null)
			{
				return null;
			}
			name = name.ToLower();
			List<string> value;
			if (!Headers.TryGetValue(name, out value) || value.Count == 0)
			{
				return null;
			}
			return value;
		}

		public string GetFirstHeaderValue(string name)
		{
			if (Headers == null)
			{
				return null;
			}
			name = name.ToLower();
			List<string> value;
			if (!Headers.TryGetValue(name, out value) || value.Count == 0)
			{
				return null;
			}
			return value[0];
		}

		public bool HasHeaderWithValue(string headerName, string value)
		{
			List<string> headerValues = GetHeaderValues(headerName);
			if (headerValues == null)
			{
				return false;
			}
			for (int i = 0; i < headerValues.Count; i++)
			{
				if (string.Compare(headerValues[i], value, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasHeader(string headerName)
		{
			List<string> headerValues = GetHeaderValues(headerName);
			if (headerValues == null)
			{
				return false;
			}
			return true;
		}

		public HTTPRange GetRange()
		{
			List<string> headerValues = GetHeaderValues("content-range");
			if (headerValues == null)
			{
				return null;
			}
			string[] array = headerValues[0].Split(new char[3] { ' ', '-', '/' }, StringSplitOptions.RemoveEmptyEntries);
			if (array[1] == "*")
			{
				return new HTTPRange(int.Parse(array[2]));
			}
			return new HTTPRange(int.Parse(array[1]), int.Parse(array[2]), (!(array[3] != "*")) ? (-1) : int.Parse(array[3]));
		}

		public static string ReadTo(Stream stream, byte blocker)
		{
			//Discarded unreachable code: IL_00a1
			byte[] buffer = VariableSizedBufferPool.Get(1024L, true);
			try
			{
				int num = 0;
				int num2 = stream.ReadByte();
				while (num2 != blocker && num2 != -1)
				{
					if (num2 > 127)
					{
						num2 = 63;
					}
					if (buffer.Length <= num)
					{
						VariableSizedBufferPool.Resize(ref buffer, buffer.Length * 2, true);
					}
					if (num > 0 || !char.IsWhiteSpace((char)num2))
					{
						buffer[num++] = (byte)num2;
					}
					num2 = stream.ReadByte();
				}
				while (num > 0 && char.IsWhiteSpace((char)buffer[num - 1]))
				{
					num--;
				}
				return Encoding.UTF8.GetString(buffer, 0, num);
			}
			finally
			{
				VariableSizedBufferPool.Release(buffer);
			}
		}

		public static string ReadTo(Stream stream, byte blocker1, byte blocker2)
		{
			//Discarded unreachable code: IL_00a8
			byte[] buffer = VariableSizedBufferPool.Get(1024L, true);
			try
			{
				int num = 0;
				int num2 = stream.ReadByte();
				while (num2 != blocker1 && num2 != blocker2 && num2 != -1)
				{
					if (num2 > 127)
					{
						num2 = 63;
					}
					if (buffer.Length <= num)
					{
						VariableSizedBufferPool.Resize(ref buffer, buffer.Length * 2, true);
					}
					if (num > 0 || !char.IsWhiteSpace((char)num2))
					{
						buffer[num++] = (byte)num2;
					}
					num2 = stream.ReadByte();
				}
				while (num > 0 && char.IsWhiteSpace((char)buffer[num - 1]))
				{
					num--;
				}
				return Encoding.UTF8.GetString(buffer, 0, num);
			}
			finally
			{
				VariableSizedBufferPool.Release(buffer);
			}
		}

		public static string NoTrimReadTo(Stream stream, byte blocker1, byte blocker2)
		{
			//Discarded unreachable code: IL_0088
			byte[] buffer = VariableSizedBufferPool.Get(1024L, true);
			try
			{
				int num = 0;
				int num2 = stream.ReadByte();
				while (num2 != blocker1 && num2 != blocker2 && num2 != -1)
				{
					if (num2 > 127)
					{
						num2 = 63;
					}
					if (buffer.Length <= num)
					{
						VariableSizedBufferPool.Resize(ref buffer, buffer.Length * 2, true);
					}
					if (num > 0 || !char.IsWhiteSpace((char)num2))
					{
						buffer[num++] = (byte)num2;
					}
					num2 = stream.ReadByte();
				}
				return Encoding.UTF8.GetString(buffer, 0, num);
			}
			finally
			{
				VariableSizedBufferPool.Release(buffer);
			}
		}

		protected int ReadChunkLength(Stream stream)
		{
			string text = ReadTo(stream, 10);
			string[] array = text.Split(';');
			string text2 = array[0];
			int result;
			if (int.TryParse(text2, NumberStyles.AllowHexSpecifier, null, out result))
			{
				return result;
			}
			throw new Exception(string.Format("Can't parse '{0}' as a hex number!", text2));
		}

		protected void ReadChunked(Stream stream)
		{
			BeginReceiveStreamFragments();
			string firstHeaderValue = GetFirstHeaderValue("Content-Length");
			bool flag = !string.IsNullOrEmpty(firstHeaderValue);
			int result = 0;
			if (flag)
			{
				flag = int.TryParse(firstHeaderValue, out result);
			}
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				VerboseLogging(string.Format("ReadChunked - hasContentLengthHeader: {0}, contentLengthHeader: {1} realLength: {2:N0}", flag.ToString(), firstHeaderValue, result));
			}
			using (BufferPoolMemoryStream bufferPoolMemoryStream = new BufferPoolMemoryStream())
			{
				int num = ReadChunkLength(stream);
				if (HTTPManager.Logger.Level == Loglevels.All)
				{
					VerboseLogging(string.Format("chunkLength: {0:N0}", num));
				}
				byte[] buffer = VariableSizedBufferPool.Get(Mathf.NextPowerOfTwo(num), true);
				int num2 = 0;
				baseRequest.DownloadLength = ((!flag) ? num : result);
				baseRequest.DownloadProgressChanged = IsSuccess;
				string firstHeaderValue2 = GetFirstHeaderValue("content-encoding");
				bool flag2 = !string.IsNullOrEmpty(firstHeaderValue2) && firstHeaderValue2 == "gzip";
				while (num != 0)
				{
					if (buffer.Length < num)
					{
						VariableSizedBufferPool.Resize(ref buffer, num, true);
					}
					int num3 = 0;
					do
					{
						int num4 = stream.Read(buffer, num3, num - num3);
						if (num4 <= 0)
						{
							throw ExceptionHelper.ServerClosedTCPStream();
						}
						num3 += num4;
						baseRequest.Downloaded += num4;
						baseRequest.DownloadProgressChanged = IsSuccess;
					}
					while (num3 < num);
					if (baseRequest.UseStreaming)
					{
						if (flag2)
						{
							byte[] array = Decompress(buffer, 0, num3);
							if (array != null)
							{
								FeedStreamFragment(array, 0, array.Length);
							}
						}
						else
						{
							FeedStreamFragment(buffer, 0, num3);
						}
					}
					else
					{
						bufferPoolMemoryStream.Write(buffer, 0, num3);
					}
					ReadTo(stream, 10);
					num2 += num3;
					num = ReadChunkLength(stream);
					if (HTTPManager.Logger.Level == Loglevels.All)
					{
						VerboseLogging(string.Format("chunkLength: {0:N0}", num));
					}
					if (!flag)
					{
						baseRequest.DownloadLength += num;
					}
					baseRequest.DownloadProgressChanged = IsSuccess;
				}
				VariableSizedBufferPool.Release(buffer);
				if (baseRequest.UseStreaming)
				{
					if (flag2)
					{
						byte[] array2 = Decompress(null, 0, 0, true);
						if (array2 != null)
						{
							FeedStreamFragment(array2, 0, array2.Length);
						}
					}
					FlushRemainingFragmentBuffer();
				}
				ReadHeaders(stream);
				if (!baseRequest.UseStreaming)
				{
					Data = DecodeStream(bufferPoolMemoryStream);
				}
			}
			CloseDecompressors();
		}

		internal void ReadRaw(Stream stream, long contentLength)
		{
			BeginReceiveStreamFragments();
			baseRequest.DownloadLength = contentLength;
			baseRequest.DownloadProgressChanged = IsSuccess;
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				VerboseLogging(string.Format("ReadRaw - contentLength: {0:N0}", contentLength));
			}
			string firstHeaderValue = GetFirstHeaderValue("content-encoding");
			bool flag = !string.IsNullOrEmpty(firstHeaderValue) && firstHeaderValue == "gzip";
			if (!baseRequest.UseStreaming && contentLength > 2147483646)
			{
				throw new OverflowException("You have to use STREAMING to download files bigger than 2GB!");
			}
			using (BufferPoolMemoryStream bufferPoolMemoryStream = new BufferPoolMemoryStream((int)((!baseRequest.UseStreaming) ? contentLength : 0)))
			{
				byte[] array = VariableSizedBufferPool.Get(Math.Max(baseRequest.StreamFragmentSize, 4096), true);
				int num = 0;
				while (contentLength > 0)
				{
					num = 0;
					do
					{
						int val = (int)Math.Min(2147483646u, (uint)contentLength);
						int num2 = stream.Read(array, num, Math.Min(val, array.Length - num));
						if (num2 <= 0)
						{
							throw ExceptionHelper.ServerClosedTCPStream();
						}
						num += num2;
						contentLength -= num2;
						baseRequest.Downloaded += num2;
						baseRequest.DownloadProgressChanged = IsSuccess;
					}
					while (num < array.Length && contentLength > 0);
					if (baseRequest.UseStreaming)
					{
						if (flag)
						{
							byte[] array2 = Decompress(array, 0, num);
							if (array2 != null)
							{
								FeedStreamFragment(array2, 0, array2.Length);
							}
						}
						else
						{
							FeedStreamFragment(array, 0, num);
						}
					}
					else
					{
						bufferPoolMemoryStream.Write(array, 0, num);
					}
				}
				VariableSizedBufferPool.Release(array);
				if (baseRequest.UseStreaming)
				{
					if (flag)
					{
						byte[] array3 = Decompress(null, 0, 0, true);
						if (array3 != null)
						{
							FeedStreamFragment(array3, 0, array3.Length);
						}
					}
					FlushRemainingFragmentBuffer();
				}
				if (!baseRequest.UseStreaming)
				{
					Data = DecodeStream(bufferPoolMemoryStream);
				}
			}
			CloseDecompressors();
		}

		protected void ReadUnknownSize(Stream stream)
		{
			string firstHeaderValue = GetFirstHeaderValue("content-encoding");
			bool flag = !string.IsNullOrEmpty(firstHeaderValue) && firstHeaderValue == "gzip";
			using (BufferPoolMemoryStream bufferPoolMemoryStream = new BufferPoolMemoryStream())
			{
				byte[] array = VariableSizedBufferPool.Get(Math.Max(baseRequest.StreamFragmentSize, 4096), false);
				if (HTTPManager.Logger.Level == Loglevels.All)
				{
					VerboseLogging(string.Format("ReadUnknownSize - buffer size: {0:N0}", array.Length));
				}
				int num = 0;
				int num2 = 0;
				do
				{
					num = 0;
					do
					{
						num2 = 0;
						NetworkStream networkStream = stream as NetworkStream;
						if (networkStream != null && baseRequest.EnableSafeReadOnUnknownContentLength)
						{
							for (int i = num; i < array.Length; i++)
							{
								if (!networkStream.DataAvailable)
								{
									break;
								}
								int num3 = stream.ReadByte();
								if (num3 >= 0)
								{
									array[i] = (byte)num3;
									num2++;
									continue;
								}
								break;
							}
						}
						else
						{
							num2 = stream.Read(array, num, array.Length - num);
						}
						num += num2;
						baseRequest.Downloaded += num2;
						baseRequest.DownloadLength = baseRequest.Downloaded;
						baseRequest.DownloadProgressChanged = IsSuccess;
					}
					while (num < array.Length && num2 > 0);
					if (baseRequest.UseStreaming)
					{
						if (flag)
						{
							byte[] array2 = Decompress(array, 0, num);
							if (array2 != null)
							{
								FeedStreamFragment(array2, 0, array2.Length);
							}
						}
						else
						{
							FeedStreamFragment(array, 0, num);
						}
					}
					else
					{
						bufferPoolMemoryStream.Write(array, 0, num);
					}
				}
				while (num2 > 0);
				VariableSizedBufferPool.Release(array);
				if (baseRequest.UseStreaming)
				{
					if (flag)
					{
						byte[] array3 = Decompress(null, 0, 0, true);
						if (array3 != null)
						{
							FeedStreamFragment(array3, 0, array3.Length);
						}
					}
					FlushRemainingFragmentBuffer();
				}
				if (!baseRequest.UseStreaming)
				{
					Data = DecodeStream(bufferPoolMemoryStream);
				}
			}
			CloseDecompressors();
		}

		protected byte[] DecodeStream(BufferPoolMemoryStream streamToDecode)
		{
			//Discarded unreachable code: IL_0109
			streamToDecode.Seek(0L, SeekOrigin.Begin);
			List<string> headerValues = GetHeaderValues("content-encoding");
			Stream stream = null;
			if (headerValues == null)
			{
				return streamToDecode.ToArray();
			}
			switch (headerValues[0])
			{
			default:
			{
				int num = 0;
				if (num == 1)
				{
					stream = new DeflateStream(streamToDecode, CompressionMode.Decompress);
					break;
				}
				return streamToDecode.ToArray();
			}
			case "gzip":
				stream = new GZipStream(streamToDecode, CompressionMode.Decompress);
				break;
			}
			using (BufferPoolMemoryStream bufferPoolMemoryStream = new BufferPoolMemoryStream((int)streamToDecode.Length))
			{
				byte[] array = VariableSizedBufferPool.Get(1024L, true);
				int num2 = 0;
				while ((num2 = stream.Read(array, 0, array.Length)) > 0)
				{
					bufferPoolMemoryStream.Write(array, 0, num2);
				}
				VariableSizedBufferPool.Release(array);
				stream.Dispose();
				return bufferPoolMemoryStream.ToArray();
			}
		}

		private void CloseDecompressors()
		{
			if (decompressorGZipStream != null)
			{
				decompressorGZipStream.Dispose();
			}
			decompressorGZipStream = null;
			if (decompressorInputStream != null)
			{
				decompressorInputStream.Dispose();
			}
			decompressorInputStream = null;
			if (decompressorOutputStream != null)
			{
				decompressorOutputStream.Dispose();
			}
			decompressorOutputStream = null;
		}

		private byte[] Decompress(byte[] data, int offset, int count, bool forceDecompress = false)
		{
			if (decompressorInputStream == null)
			{
				decompressorInputStream = new BufferPoolMemoryStream(count);
			}
			if (data != null)
			{
				decompressorInputStream.Write(data, offset, count);
			}
			if (!forceDecompress && decompressorInputStream.Length < 256)
			{
				return null;
			}
			decompressorInputStream.Position = 0L;
			if (decompressorGZipStream == null)
			{
				decompressorGZipStream = new GZipStream(decompressorInputStream, CompressionMode.Decompress, true);
				decompressorGZipStream.FlushMode = FlushType.Sync;
			}
			if (decompressorOutputStream == null)
			{
				decompressorOutputStream = new BufferPoolMemoryStream();
			}
			decompressorOutputStream.SetLength(0L);
			byte[] array = VariableSizedBufferPool.Get(1024L, true);
			int count2;
			while ((count2 = decompressorGZipStream.Read(array, 0, array.Length)) != 0)
			{
				decompressorOutputStream.Write(array, 0, count2);
			}
			VariableSizedBufferPool.Release(array);
			decompressorGZipStream.SetLength(0L);
			return decompressorOutputStream.ToArray();
		}

		protected void BeginReceiveStreamFragments()
		{
			allFragmentSize = 0;
		}

		protected void FeedStreamFragment(byte[] buffer, int pos, int length)
		{
			if (buffer == null || length == 0)
			{
				return;
			}
			WaitWhileFragmentQueueIsFull();
			if (fragmentBuffer == null)
			{
				fragmentBuffer = VariableSizedBufferPool.Get(baseRequest.StreamFragmentSize, false);
				fragmentBufferDataLength = 0;
			}
			if (fragmentBufferDataLength + length <= baseRequest.StreamFragmentSize)
			{
				Array.Copy(buffer, pos, fragmentBuffer, fragmentBufferDataLength, length);
				fragmentBufferDataLength += length;
				if (fragmentBufferDataLength == baseRequest.StreamFragmentSize)
				{
					AddStreamedFragment(fragmentBuffer);
					fragmentBuffer = null;
					fragmentBufferDataLength = 0;
				}
			}
			else
			{
				int num = baseRequest.StreamFragmentSize - fragmentBufferDataLength;
				FeedStreamFragment(buffer, pos, num);
				FeedStreamFragment(buffer, pos + num, length - num);
			}
		}

		protected void FlushRemainingFragmentBuffer()
		{
			if (fragmentBuffer != null)
			{
				VariableSizedBufferPool.Resize(ref fragmentBuffer, fragmentBufferDataLength, false);
				AddStreamedFragment(fragmentBuffer);
				fragmentBuffer = null;
				fragmentBufferDataLength = 0;
			}
			AutoResetEvent autoResetEvent = fragmentWaitEvent;
			fragmentWaitEvent = null;
			if (autoResetEvent != null)
			{
				((IDisposable)autoResetEvent).Dispose();
			}
		}

		protected void AddStreamedFragment(byte[] buffer)
		{
			lock (SyncRoot)
			{
				if (streamedFragments == null)
				{
					streamedFragments = new List<byte[]>();
				}
				streamedFragments.Add(buffer);
				if (HTTPManager.Logger.Level == Loglevels.All && buffer != null && streamedFragments != null)
				{
					VerboseLogging(string.Format("AddStreamedFragment buffer length: {0:N0} streamedFragments: {1:N0}", buffer.Length, streamedFragments.Count));
				}
			}
		}

		protected void WaitWhileFragmentQueueIsFull()
		{
			while (baseRequest.UseStreaming && FragmentQueueIsFull())
			{
				VerboseLogging("WaitWhileFragmentQueueIsFull");
				if (fragmentWaitEvent == null)
				{
					fragmentWaitEvent = new AutoResetEvent(false);
				}
				fragmentWaitEvent.WaitOne(16);
			}
		}

		private bool FragmentQueueIsFull()
		{
			//Discarded unreachable code: IL_0083
			lock (SyncRoot)
			{
				bool flag = streamedFragments != null && streamedFragments.Count >= baseRequest.MaxFragmentQueueLength;
				if (flag && HTTPManager.Logger.Level == Loglevels.All)
				{
					VerboseLogging(string.Format("HasFragmentsInQueue - {0} / {1}", streamedFragments.Count, baseRequest.MaxFragmentQueueLength));
				}
				return flag;
			}
		}

		public List<byte[]> GetStreamedFragments()
		{
			//Discarded unreachable code: IL_00af
			lock (SyncRoot)
			{
				if (streamedFragments == null || streamedFragments.Count == 0)
				{
					if (HTTPManager.Logger.Level == Loglevels.All)
					{
						VerboseLogging("GetStreamedFragments - no fragments, returning with null");
					}
					return null;
				}
				List<byte[]> list = new List<byte[]>(streamedFragments);
				streamedFragments.Clear();
				if (fragmentWaitEvent != null)
				{
					fragmentWaitEvent.Set();
				}
				if (HTTPManager.Logger.Level == Loglevels.All)
				{
					VerboseLogging(string.Format("GetStreamedFragments - returning with {0:N0} fragments", list.Count.ToString()));
				}
				return list;
			}
		}

		internal bool HasStreamedFragments()
		{
			//Discarded unreachable code: IL_003c
			lock (SyncRoot)
			{
				return streamedFragments != null && streamedFragments.Count >= baseRequest.MaxFragmentQueueLength;
			}
		}

		internal void FinishStreaming()
		{
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				VerboseLogging("FinishStreaming");
			}
			IsStreamingFinished = true;
			Dispose();
		}

		private void VerboseLogging(string str)
		{
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				HTTPManager.Logger.Verbose("HTTPResponse", "'" + baseRequest.CurrentUri.ToString() + "' - " + str);
			}
		}

		public void Dispose()
		{
			if (Stream != null && Stream is ReadOnlyBufferedStream)
			{
				((IDisposable)Stream).Dispose();
			}
			Stream = null;
			GC.SuppressFinalize(this);
		}
	}
}
