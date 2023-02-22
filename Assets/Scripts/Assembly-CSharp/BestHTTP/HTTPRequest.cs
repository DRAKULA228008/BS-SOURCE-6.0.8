using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BestHTTP.Authentication;
using BestHTTP.Extensions;
using BestHTTP.Forms;

namespace BestHTTP
{
	public sealed class HTTPRequest : IDisposable, IEnumerator, IEnumerator<HTTPRequest>
	{
		public static readonly byte[] EOL = new byte[2] { 13, 10 };

		public static readonly string[] MethodNames = new string[8]
		{
			HTTPMethods.Get.ToString().ToUpper(),
			HTTPMethods.Head.ToString().ToUpper(),
			HTTPMethods.Post.ToString().ToUpper(),
			HTTPMethods.Put.ToString().ToUpper(),
			HTTPMethods.Delete.ToString().ToUpper(),
			HTTPMethods.Patch.ToString().ToUpper(),
			HTTPMethods.Merge.ToString().ToUpper(),
			HTTPMethods.Options.ToString().ToUpper()
		};

		public static int UploadChunkSize = 2048;

		public OnUploadProgressDelegate OnUploadProgress;

		public OnDownloadProgressDelegate OnProgress;

		public OnRequestFinishedDelegate OnUpgraded;

		private OnBeforeRedirectionDelegate onBeforeRedirection;

		private OnBeforeHeaderSendDelegate _onBeforeHeaderSend;

		private bool isKeepAlive;

		private int streamFragmentSize;

		private bool useStreaming;

		private HTTPFormBase FieldCollector;

		private HTTPFormBase FormImpl;

		HTTPRequest IEnumerator<HTTPRequest>.Current
		{
			get
			{
				return this;
			}
		}

		public Uri Uri { get; set; }

		public HTTPMethods MethodType { get; set; }

		public byte[] RawData { get; set; }

		public Stream UploadStream { get; set; }

		public bool DisposeUploadStream { get; set; }

		public bool UseUploadStreamLength { get; set; }

		public bool IsKeepAlive
		{
			get
			{
				return isKeepAlive;
			}
			set
			{
				if (State == HTTPRequestStates.Processing)
				{
					throw new NotSupportedException("Changing the IsKeepAlive property while processing the request is not supported.");
				}
				isKeepAlive = value;
			}
		}

		public bool UseStreaming
		{
			get
			{
				return useStreaming;
			}
			set
			{
				if (State == HTTPRequestStates.Processing)
				{
					throw new NotSupportedException("Changing the UseStreaming property while processing the request is not supported.");
				}
				useStreaming = value;
			}
		}

		public int StreamFragmentSize
		{
			get
			{
				return streamFragmentSize;
			}
			set
			{
				if (State == HTTPRequestStates.Processing)
				{
					throw new NotSupportedException("Changing the StreamFragmentSize property while processing the request is not supported.");
				}
				if (value < 1)
				{
					throw new ArgumentException("StreamFragmentSize must be at least 1.");
				}
				streamFragmentSize = value;
			}
		}

		public int MaxFragmentQueueLength { get; set; }

		public OnRequestFinishedDelegate Callback { get; set; }

		public bool DisableRetry { get; set; }

		public bool IsRedirected { get; internal set; }

		public Uri RedirectUri { get; internal set; }

		public Uri CurrentUri
		{
			get
			{
				return (!IsRedirected) ? Uri : RedirectUri;
			}
		}

		public HTTPResponse Response { get; internal set; }

		public Exception Exception { get; internal set; }

		public object Tag { get; set; }

		public Credentials Credentials { get; set; }

		public int MaxRedirects { get; set; }

		public HTTPFormUsage FormUsage { get; set; }

		public HTTPRequestStates State { get; internal set; }

		public int RedirectCount { get; internal set; }

		public TimeSpan ConnectTimeout { get; set; }

		public TimeSpan Timeout { get; set; }

		public bool EnableTimoutForStreaming { get; set; }

		public bool EnableSafeReadOnUnknownContentLength { get; set; }

		public int Priority { get; set; }

		public SupportedProtocols ProtocolHandler { get; set; }

		public bool TryToMinimizeTCPLatency { get; set; }

		internal long Downloaded { get; set; }

		internal long DownloadLength { get; set; }

		internal bool DownloadProgressChanged { get; set; }

		internal long UploadStreamLength
		{
			get
			{
				//Discarded unreachable code: IL_002a, IL_0038
				if (UploadStream == null || !UseUploadStreamLength)
				{
					return -1L;
				}
				try
				{
					return UploadStream.Length;
				}
				catch
				{
					return -1L;
				}
			}
		}

		internal long Uploaded { get; set; }

		internal long UploadLength { get; set; }

		internal bool UploadProgressChanged { get; set; }

		private Dictionary<string, List<string>> Headers { get; set; }

		public object Current
		{
			get
			{
				return null;
			}
		}

		public event Func<HTTPRequest, X509Certificate, X509Chain, bool> CustomCertificationValidator;

		public event OnBeforeRedirectionDelegate OnBeforeRedirection
		{
			add
			{
				onBeforeRedirection = (OnBeforeRedirectionDelegate)Delegate.Combine(onBeforeRedirection, value);
			}
			remove
			{
				onBeforeRedirection = (OnBeforeRedirectionDelegate)Delegate.Remove(onBeforeRedirection, value);
			}
		}

		public event OnBeforeHeaderSendDelegate OnBeforeHeaderSend
		{
			add
			{
				_onBeforeHeaderSend = (OnBeforeHeaderSendDelegate)Delegate.Combine(_onBeforeHeaderSend, value);
			}
			remove
			{
				_onBeforeHeaderSend = (OnBeforeHeaderSendDelegate)Delegate.Remove(_onBeforeHeaderSend, value);
			}
		}

		public HTTPRequest(Uri uri)
			: this(uri, HTTPMethods.Get, HTTPManager.KeepAliveDefaultValue, true, null)
		{
		}

		public HTTPRequest(Uri uri, OnRequestFinishedDelegate callback)
			: this(uri, HTTPMethods.Get, HTTPManager.KeepAliveDefaultValue, true, callback)
		{
		}

		public HTTPRequest(Uri uri, bool isKeepAlive, OnRequestFinishedDelegate callback)
			: this(uri, HTTPMethods.Get, isKeepAlive, true, callback)
		{
		}

		public HTTPRequest(Uri uri, bool isKeepAlive, bool disableCache, OnRequestFinishedDelegate callback)
			: this(uri, HTTPMethods.Get, isKeepAlive, disableCache, callback)
		{
		}

		public HTTPRequest(Uri uri, HTTPMethods methodType)
			: this(uri, methodType, HTTPManager.KeepAliveDefaultValue, true, null)
		{
		}

		public HTTPRequest(Uri uri, HTTPMethods methodType, OnRequestFinishedDelegate callback)
			: this(uri, methodType, HTTPManager.KeepAliveDefaultValue, true, callback)
		{
		}

		public HTTPRequest(Uri uri, HTTPMethods methodType, bool isKeepAlive, OnRequestFinishedDelegate callback)
			: this(uri, methodType, isKeepAlive, true, callback)
		{
		}

		public HTTPRequest(Uri uri, HTTPMethods methodType, bool isKeepAlive, bool disableCache, OnRequestFinishedDelegate callback)
		{
			Uri = uri;
			MethodType = methodType;
			IsKeepAlive = isKeepAlive;
			Callback = callback;
			StreamFragmentSize = 4096;
			MaxFragmentQueueLength = 10;
			DisableRetry = methodType != HTTPMethods.Get;
			MaxRedirects = int.MaxValue;
			RedirectCount = 0;
			Downloaded = (DownloadLength = 0L);
			DownloadProgressChanged = false;
			State = HTTPRequestStates.Initial;
			ConnectTimeout = HTTPManager.ConnectTimeout;
			Timeout = HTTPManager.RequestTimeout;
			EnableTimoutForStreaming = false;
			EnableSafeReadOnUnknownContentLength = true;
			UseUploadStreamLength = true;
			DisposeUploadStream = true;
			this.CustomCertificationValidator = (Func<HTTPRequest, X509Certificate, X509Chain, bool>)Delegate.Combine(this.CustomCertificationValidator, HTTPManager.DefaultCertificationValidator);
			TryToMinimizeTCPLatency = HTTPManager.TryToMinimizeTCPLatency;
		}

		public void AddField(string fieldName, string value)
		{
			AddField(fieldName, value, Encoding.UTF8);
		}

		public void AddField(string fieldName, string value, Encoding e)
		{
			if (FieldCollector == null)
			{
				FieldCollector = new HTTPFormBase();
			}
			FieldCollector.AddField(fieldName, value, e);
		}

		public void AddBinaryData(string fieldName, byte[] content)
		{
			AddBinaryData(fieldName, content, null, null);
		}

		public void AddBinaryData(string fieldName, byte[] content, string fileName)
		{
			AddBinaryData(fieldName, content, fileName, null);
		}

		public void AddBinaryData(string fieldName, byte[] content, string fileName, string mimeType)
		{
			if (FieldCollector == null)
			{
				FieldCollector = new HTTPFormBase();
			}
			FieldCollector.AddBinaryData(fieldName, content, fileName, mimeType);
		}

		public void SetForm(HTTPFormBase form)
		{
			FormImpl = form;
		}

		public List<HTTPFieldData> GetFormFields()
		{
			if (FieldCollector == null || FieldCollector.IsEmpty)
			{
				return null;
			}
			return new List<HTTPFieldData>(FieldCollector.Fields);
		}

		public void ClearForm()
		{
			FormImpl = null;
			FieldCollector = null;
		}

		private HTTPFormBase SelectFormImplementation()
		{
			if (FormImpl != null)
			{
				return FormImpl;
			}
			if (FieldCollector == null)
			{
				return null;
			}
			switch (FormUsage)
			{
			case HTTPFormUsage.Automatic:
				if (FieldCollector.HasBinary || FieldCollector.HasLongValue)
				{
					goto case HTTPFormUsage.Multipart;
				}
				goto case HTTPFormUsage.UrlEncoded;
			case HTTPFormUsage.UrlEncoded:
				FormImpl = new HTTPUrlEncodedForm();
				break;
			case HTTPFormUsage.Multipart:
				FormImpl = new HTTPMultiPartForm();
				break;
			case HTTPFormUsage.RawJSon:
				FormImpl = new RawJsonForm();
				break;
			}
			FormImpl.CopyFrom(FieldCollector);
			return FormImpl;
		}

		public void AddHeader(string name, string value)
		{
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

		public void SetHeader(string name, string value)
		{
			if (Headers == null)
			{
				Headers = new Dictionary<string, List<string>>();
			}
			List<string> value2;
			if (!Headers.TryGetValue(name, out value2))
			{
				Headers.Add(name, value2 = new List<string>(1));
			}
			value2.Clear();
			value2.Add(value);
		}

		public bool RemoveHeader(string name)
		{
			if (Headers == null)
			{
				return false;
			}
			return Headers.Remove(name);
		}

		public bool HasHeader(string name)
		{
			return Headers != null && Headers.ContainsKey(name);
		}

		public string GetFirstHeaderValue(string name)
		{
			if (Headers == null)
			{
				return null;
			}
			List<string> value = null;
			if (Headers.TryGetValue(name, out value) && value.Count > 0)
			{
				return value[0];
			}
			return null;
		}

		public List<string> GetHeaderValues(string name)
		{
			if (Headers == null)
			{
				return null;
			}
			List<string> value = null;
			if (Headers.TryGetValue(name, out value) && value.Count > 0)
			{
				return value;
			}
			return null;
		}

		public void RemoveHeaders()
		{
			if (Headers != null)
			{
				Headers.Clear();
			}
		}

		public void SetRangeHeader(int firstBytePos)
		{
			SetHeader("Range", string.Format("bytes={0}-", firstBytePos));
		}

		public void SetRangeHeader(int firstBytePos, int lastBytePos)
		{
			SetHeader("Range", string.Format("bytes={0}-{1}", firstBytePos, lastBytePos));
		}

		public void EnumerateHeaders(OnHeaderEnumerationDelegate callback)
		{
			EnumerateHeaders(callback, false);
		}

		public void EnumerateHeaders(OnHeaderEnumerationDelegate callback, bool callBeforeSendCallback)
		{
			if (!HasHeader("Host"))
			{
				if (CurrentUri.Port == 80 || CurrentUri.Port == 443)
				{
					SetHeader("Host", CurrentUri.Host);
				}
				else
				{
					SetHeader("Host", CurrentUri.Authority);
				}
			}
			if (IsRedirected && !HasHeader("Referer"))
			{
				AddHeader("Referer", Uri.ToString());
			}
			if (!HasHeader("Accept-Encoding"))
			{
				AddHeader("Accept-Encoding", "gzip, identity");
			}
			if (!HasHeader("Connection"))
			{
				AddHeader("Connection", (!IsKeepAlive) ? "Close, TE" : "Keep-Alive, TE");
			}
			if (!HasHeader("TE"))
			{
				AddHeader("TE", "identity");
			}
			if (!HasHeader("User-Agent"))
			{
				AddHeader("User-Agent", "BestHTTP");
			}
			long num = -1L;
			if (UploadStream == null)
			{
				byte[] entityBody = GetEntityBody();
				num = ((entityBody != null) ? entityBody.Length : 0);
				if (RawData == null && (FormImpl != null || (FieldCollector != null && !FieldCollector.IsEmpty)))
				{
					SelectFormImplementation();
					if (FormImpl != null)
					{
						FormImpl.PrepareRequest(this);
					}
				}
			}
			else
			{
				num = UploadStreamLength;
				if (num == -1)
				{
					SetHeader("Transfer-Encoding", "Chunked");
				}
				if (!HasHeader("Content-Type"))
				{
					SetHeader("Content-Type", "application/octet-stream");
				}
			}
			if (num >= 0 && !HasHeader("Content-Length"))
			{
				SetHeader("Content-Length", num.ToString());
			}
			if (Credentials != null)
			{
				switch (Credentials.Type)
				{
				case AuthenticationTypes.Basic:
					SetHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(Credentials.UserName + ":" + Credentials.Password)));
					break;
				case AuthenticationTypes.Unknown:
				case AuthenticationTypes.Digest:
				{
					Digest digest = DigestStore.Get(CurrentUri);
					if (digest != null)
					{
						string value = digest.GenerateResponseHeader(this, Credentials);
						if (!string.IsNullOrEmpty(value))
						{
							SetHeader("Authorization", value);
						}
					}
					break;
				}
				}
			}
			if (callBeforeSendCallback && _onBeforeHeaderSend != null)
			{
				try
				{
					_onBeforeHeaderSend(this);
				}
				catch (Exception ex)
				{
					HTTPManager.Logger.Exception("HTTPRequest", "OnBeforeHeaderSend", ex);
				}
			}
			if (callback == null || Headers == null)
			{
				return;
			}
			foreach (KeyValuePair<string, List<string>> header in Headers)
			{
				callback(header.Key, header.Value);
			}
		}

		private void SendHeaders(Stream stream)
		{
			EnumerateHeaders(delegate(string header, List<string> values)
			{
				if (!string.IsNullOrEmpty(header) && values != null)
				{
					byte[] aSCIIBytes = (header + ": ").GetASCIIBytes();
					for (int i = 0; i < values.Count; i++)
					{
						if (string.IsNullOrEmpty(values[i]))
						{
							HTTPManager.Logger.Warning("HTTPRequest", string.Format("Null/empty value for header: {0}", header));
						}
						else
						{
							if ((int)HTTPManager.Logger.Level <= 1)
							{
								VerboseLogging("Header - '" + header + "': '" + values[i] + "'");
							}
							byte[] aSCIIBytes2 = values[i].GetASCIIBytes();
							stream.WriteArray(aSCIIBytes);
							stream.WriteArray(aSCIIBytes2);
							stream.WriteArray(EOL);
							VariableSizedBufferPool.Release(aSCIIBytes2);
						}
					}
					VariableSizedBufferPool.Release(aSCIIBytes);
				}
			}, true);
		}

		public string DumpHeaders()
		{
			//Discarded unreachable code: IL_0023
			using (BufferPoolMemoryStream bufferPoolMemoryStream = new BufferPoolMemoryStream(5120))
			{
				SendHeaders(bufferPoolMemoryStream);
				return bufferPoolMemoryStream.ToArray().AsciiToString();
			}
		}

		public byte[] GetEntityBody()
		{
			if (RawData != null)
			{
				return RawData;
			}
			if (FormImpl != null || (FieldCollector != null && !FieldCollector.IsEmpty))
			{
				SelectFormImplementation();
				if (FormImpl != null)
				{
					return FormImpl.GetData();
				}
			}
			return null;
		}

		internal void SendOutTo(Stream stream)
		{
			try
			{
				string requestPathAndQueryURL = CurrentUri.GetRequestPathAndQueryURL();
				string text = string.Format("{0} {1} HTTP/1.1", MethodNames[(uint)MethodType], requestPathAndQueryURL);
				if ((int)HTTPManager.Logger.Level <= 1)
				{
					HTTPManager.Logger.Information("HTTPRequest", string.Format("Sending request: '{0}'", text));
				}
				using (WriteOnlyBufferedStream writeOnlyBufferedStream = new WriteOnlyBufferedStream(stream, (int)((float)UploadChunkSize * 1.5f)))
				{
					byte[] aSCIIBytes = text.GetASCIIBytes();
					writeOnlyBufferedStream.WriteArray(aSCIIBytes);
					writeOnlyBufferedStream.WriteArray(EOL);
					VariableSizedBufferPool.Release(aSCIIBytes);
					SendHeaders(writeOnlyBufferedStream);
					writeOnlyBufferedStream.WriteArray(EOL);
					writeOnlyBufferedStream.Flush();
					byte[] array = RawData;
					if (array == null && FormImpl != null)
					{
						array = FormImpl.GetData();
					}
					if (array != null || UploadStream != null)
					{
						Stream stream2 = UploadStream;
						if (stream2 == null)
						{
							stream2 = new MemoryStream(array, 0, array.Length);
							UploadLength = array.Length;
						}
						else
						{
							UploadLength = ((!UseUploadStreamLength) ? (-1) : UploadStreamLength);
						}
						Uploaded = 0L;
						byte[] array2 = VariableSizedBufferPool.Get(UploadChunkSize, true);
						int num = 0;
						while ((num = stream2.Read(array2, 0, array2.Length)) > 0)
						{
							if (!UseUploadStreamLength)
							{
								byte[] aSCIIBytes2 = num.ToString("X").GetASCIIBytes();
								writeOnlyBufferedStream.WriteArray(aSCIIBytes2);
								writeOnlyBufferedStream.WriteArray(EOL);
								VariableSizedBufferPool.Release(aSCIIBytes2);
							}
							writeOnlyBufferedStream.Write(array2, 0, num);
							if (!UseUploadStreamLength)
							{
								writeOnlyBufferedStream.WriteArray(EOL);
							}
							Uploaded += num;
							writeOnlyBufferedStream.Flush();
							UploadProgressChanged = true;
						}
						VariableSizedBufferPool.Release(array2);
						if (!UseUploadStreamLength)
						{
							byte[] array3 = VariableSizedBufferPool.Get(1L, true);
							array3[0] = 48;
							writeOnlyBufferedStream.Write(array3, 0, 1);
							writeOnlyBufferedStream.WriteArray(EOL);
							writeOnlyBufferedStream.WriteArray(EOL);
							VariableSizedBufferPool.Release(array3);
						}
						writeOnlyBufferedStream.Flush();
						if (UploadStream == null && stream2 != null)
						{
							stream2.Dispose();
						}
					}
					else
					{
						writeOnlyBufferedStream.Flush();
					}
				}
				HTTPManager.Logger.Information("HTTPRequest", "'" + text + "' sent out");
			}
			finally
			{
				if (UploadStream != null && DisposeUploadStream)
				{
					UploadStream.Dispose();
				}
			}
		}

		internal void UpgradeCallback()
		{
			if (Response == null || !Response.IsUpgraded)
			{
				return;
			}
			try
			{
				if (OnUpgraded != null)
				{
					OnUpgraded(this, Response);
				}
			}
			catch (Exception ex)
			{
				HTTPManager.Logger.Exception("HTTPRequest", "UpgradeCallback", ex);
			}
		}

		internal void CallCallback()
		{
			try
			{
				if (Callback != null)
				{
					Callback(this, Response);
				}
			}
			catch (Exception ex)
			{
				HTTPManager.Logger.Exception("HTTPRequest", "CallCallback", ex);
			}
		}

		internal bool CallOnBeforeRedirection(Uri redirectUri)
		{
			if (onBeforeRedirection != null)
			{
				return onBeforeRedirection(this, Response, redirectUri);
			}
			return true;
		}

		internal void FinishStreaming()
		{
			if (Response != null && UseStreaming)
			{
				Response.FinishStreaming();
			}
		}

		internal void Prepare()
		{
		}

		internal bool CallCustomCertificationValidator(X509Certificate cert, X509Chain chain)
		{
			if (this.CustomCertificationValidator != null)
			{
				return this.CustomCertificationValidator(this, cert, chain);
			}
			return true;
		}

		public HTTPRequest Send()
		{
			return HTTPManager.SendRequest(this);
		}

		public void Abort()
		{
			//Discarded unreachable code: IL_00b6
			try
			{
				if (State >= HTTPRequestStates.Finished)
				{
					HTTPManager.Logger.Warning("HTTPRequest", string.Format("Abort - Already in a state({0}) where no Abort required!", State.ToString()));
					return;
				}
				ConnectionBase connectionWith = HTTPManager.GetConnectionWith(this);
				if (connectionWith == null)
				{
					if (!HTTPManager.RemoveFromQueue(this))
					{
						HTTPManager.Logger.Warning("HTTPRequest", "Abort - No active connection found with this request! (The request may already finished?)");
					}
					State = HTTPRequestStates.Aborted;
					CallCallback();
				}
				else
				{
					if (Response != null && Response.IsStreamed)
					{
						Response.Dispose();
					}
					connectionWith.Abort(HTTPConnectionStates.AbortRequested);
				}
			}
			catch
			{
				throw new Exception("Wasn't able to acquire a thread lock. Abort failed!");
			}
		}

		public void Clear()
		{
			ClearForm();
			RemoveHeaders();
			IsRedirected = false;
			RedirectCount = 0;
			long num3 = (Downloaded = (DownloadLength = 0L));
		}

		private void VerboseLogging(string str)
		{
			HTTPManager.Logger.Verbose("HTTPRequest", "'" + CurrentUri.ToString() + "' - " + str);
		}

		public bool MoveNext()
		{
			return State < HTTPRequestStates.Finished;
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			if (Response != null)
			{
				Response.Dispose();
			}
		}
	}
}
