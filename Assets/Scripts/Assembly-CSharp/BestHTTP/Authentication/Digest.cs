using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP.Extensions;

namespace BestHTTP.Authentication
{
	public sealed class Digest
	{
		public Uri Uri { get; private set; }

		public AuthenticationTypes Type { get; private set; }

		public string Realm { get; private set; }

		public bool Stale { get; private set; }

		private string Nonce { get; set; }

		private string Opaque { get; set; }

		private string Algorithm { get; set; }

		public List<string> ProtectedUris { get; private set; }

		private string QualityOfProtections { get; set; }

		private int NonceCount { get; set; }

		private string HA1Sess { get; set; }

		internal Digest(Uri uri)
		{
			Uri = uri;
			Algorithm = "md5";
		}

		public void ParseChallange(string header)
		{
			Type = AuthenticationTypes.Unknown;
			Stale = false;
			Opaque = null;
			HA1Sess = null;
			NonceCount = 0;
			QualityOfProtections = null;
			if (ProtectedUris != null)
			{
				ProtectedUris.Clear();
			}
			WWWAuthenticateHeaderParser wWWAuthenticateHeaderParser = new WWWAuthenticateHeaderParser(header);
			foreach (HeaderValue value in wWWAuthenticateHeaderParser.Values)
			{
				switch (value.Key)
				{
				case "basic":
					Type = AuthenticationTypes.Basic;
					break;
				case "digest":
					Type = AuthenticationTypes.Digest;
					break;
				case "realm":
					Realm = value.Value;
					break;
				case "domain":
					if (!string.IsNullOrEmpty(value.Value) && value.Value.Length != 0)
					{
						if (ProtectedUris == null)
						{
							ProtectedUris = new List<string>();
						}
						int pos = 0;
						string item = value.Value.Read(ref pos, ' ');
						do
						{
							ProtectedUris.Add(item);
							item = value.Value.Read(ref pos, ' ');
						}
						while (pos < value.Value.Length);
					}
					break;
				case "nonce":
					Nonce = value.Value;
					break;
				case "qop":
					QualityOfProtections = value.Value;
					break;
				case "stale":
					Stale = bool.Parse(value.Value);
					break;
				case "opaque":
					Opaque = value.Value;
					break;
				case "algorithm":
					Algorithm = value.Value;
					break;
				}
			}
		}

		public string GenerateResponseHeader(HTTPRequest request, Credentials credentials, bool isProxy = false)
		{
			try
			{
				switch (Type)
				{
				case AuthenticationTypes.Basic:
					return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", credentials.UserName, credentials.Password)));
				case AuthenticationTypes.Digest:
				{
					NonceCount++;
					string empty = string.Empty;
					string text = new Random(request.GetHashCode()).Next(int.MinValue, int.MaxValue).ToString("X8");
					string text2 = NonceCount.ToString("X8");
					switch (Algorithm.TrimAndLower())
					{
					default:
					{
						int num = 0;
						if (num == 1)
						{
							if (string.IsNullOrEmpty(HA1Sess))
							{
								HA1Sess = string.Format("{0}:{1}:{2}:{3}:{4}", credentials.UserName, Realm, credentials.Password, Nonce, text2).CalculateMD5Hash();
							}
							empty = HA1Sess;
							break;
						}
						return string.Empty;
					}
					case "md5":
						empty = string.Format("{0}:{1}:{2}", credentials.UserName, Realm, credentials.Password).CalculateMD5Hash();
						break;
					}
					string empty2 = string.Empty;
					string text3 = ((QualityOfProtections == null) ? null : QualityOfProtections.TrimAndLower());
					string text4 = ((!isProxy) ? request.MethodType.ToString().ToUpper() : "CONNECT");
					string text5 = ((!isProxy) ? request.CurrentUri.GetRequestPathAndQueryURL() : (request.CurrentUri.Host + ":" + request.CurrentUri.Port));
					if (text3 == null)
					{
						string arg = (request.MethodType.ToString().ToUpper() + ":" + request.CurrentUri.GetRequestPathAndQueryURL()).CalculateMD5Hash();
						empty2 = string.Format("{0}:{1}:{2}", empty, Nonce, arg).CalculateMD5Hash();
					}
					else if (text3.Contains("auth-int"))
					{
						text3 = "auth-int";
						byte[] array = request.GetEntityBody();
						if (array == null)
						{
							array = VariableSizedBufferPool.NoData;
						}
						string text6 = string.Format("{0}:{1}:{2}", text4, text5, array.CalculateMD5Hash()).CalculateMD5Hash();
						empty2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", empty, Nonce, text2, text, text3, text6).CalculateMD5Hash();
					}
					else
					{
						if (!text3.Contains("auth"))
						{
							return string.Empty;
						}
						text3 = "auth";
						string text7 = (text4 + ":" + text5).CalculateMD5Hash();
						empty2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", empty, Nonce, text2, text, text3, text7).CalculateMD5Hash();
					}
					string text8 = string.Format("Digest username=\"{0}\", realm=\"{1}\", nonce=\"{2}\", uri=\"{3}\", cnonce=\"{4}\", response=\"{5}\"", credentials.UserName, Realm, Nonce, text5, text, empty2);
					if (text3 != null)
					{
						text8 = string.Concat(text8, ", qop=\"" + text3 + "\", nc=" + text2);
					}
					if (!string.IsNullOrEmpty(Opaque))
					{
						text8 = text8 + ", opaque=\"" + Opaque + "\"";
					}
					return text8;
				}
				}
			}
			catch
			{
			}
			return string.Empty;
		}

		public bool IsUriProtected(Uri uri)
		{
			if (string.CompareOrdinal(uri.Host, Uri.Host) != 0)
			{
				return false;
			}
			string text = uri.ToString();
			if (ProtectedUris != null && ProtectedUris.Count > 0)
			{
				for (int i = 0; i < ProtectedUris.Count; i++)
				{
					if (text.Contains(ProtectedUris[i]))
					{
						return true;
					}
				}
			}
			return true;
		}
	}
}
