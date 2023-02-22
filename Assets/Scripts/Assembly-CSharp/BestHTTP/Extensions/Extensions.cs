using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BestHTTP.Extensions
{
	public static class Extensions
	{
		private static readonly Regex validIpV4AddressRegex = new Regex("\\b(?:\\d{1,3}\\.){3}\\d{1,3}\\b", RegexOptions.IgnoreCase);

		public static string AsciiToString(this byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder(bytes.Length);
			foreach (byte b in bytes)
			{
				stringBuilder.Append((char)((b > 127) ? 63 : b));
			}
			return stringBuilder.ToString();
		}

		public static byte[] GetASCIIBytes(this string str)
		{
			byte[] array = VariableSizedBufferPool.Get(str.Length, false);
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];
				array[i] = (byte)((c >= '\u0080') ? '?' : c);
			}
			return array;
		}

		public static void SendAsASCII(this BinaryWriter stream, string str)
		{
			foreach (char c in str)
			{
				stream.Write((byte)((c >= '\u0080') ? '?' : c));
			}
		}

		public static void WriteLine(this Stream fs)
		{
			fs.Write(HTTPRequest.EOL, 0, 2);
		}

		public static void WriteLine(this Stream fs, string line)
		{
			byte[] aSCIIBytes = line.GetASCIIBytes();
			fs.Write(aSCIIBytes, 0, aSCIIBytes.Length);
			fs.WriteLine();
			VariableSizedBufferPool.Release(aSCIIBytes);
		}

		public static void WriteLine(this Stream fs, string format, params object[] values)
		{
			byte[] aSCIIBytes = string.Format(format, values).GetASCIIBytes();
			fs.Write(aSCIIBytes, 0, aSCIIBytes.Length);
			fs.WriteLine();
			VariableSizedBufferPool.Release(aSCIIBytes);
		}

		public static string GetRequestPathAndQueryURL(this Uri uri)
		{
			string text = uri.GetComponents(UriComponents.PathAndQuery, UriFormat.UriEscaped);
			if (string.IsNullOrEmpty(text))
			{
				text = "/";
			}
			return text;
		}

		public static string[] FindOption(this string str, string option)
		{
			string[] array = str.ToLower().Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			option = option.ToLower();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Contains(option))
				{
					return array[i].Split(new char[1] { '=' }, StringSplitOptions.RemoveEmptyEntries);
				}
			}
			return null;
		}

		public static void WriteArray(this Stream stream, byte[] array)
		{
			stream.Write(array, 0, array.Length);
		}

		public static bool IsHostIsAnIPAddress(this Uri uri)
		{
			if (uri == null)
			{
				return false;
			}
			return IsIpV4AddressValid(uri.Host) || IsIpV6AddressValid(uri.Host);
		}

		public static bool IsIpV4AddressValid(string address)
		{
			if (!string.IsNullOrEmpty(address))
			{
				return validIpV4AddressRegex.IsMatch(address.Trim());
			}
			return false;
		}

		public static bool IsIpV6AddressValid(string address)
		{
			IPAddress address2;
			if (!string.IsNullOrEmpty(address) && IPAddress.TryParse(address, out address2))
			{
				return address2.AddressFamily == AddressFamily.InterNetworkV6;
			}
			return false;
		}

		public static int ToInt32(this string str, int defaultValue = 0)
		{
			//Discarded unreachable code: IL_0014, IL_0021
			if (str == null)
			{
				return defaultValue;
			}
			try
			{
				return int.Parse(str);
			}
			catch
			{
				return defaultValue;
			}
		}

		public static long ToInt64(this string str, long defaultValue = 0L)
		{
			//Discarded unreachable code: IL_0014, IL_0021
			if (str == null)
			{
				return defaultValue;
			}
			try
			{
				return long.Parse(str);
			}
			catch
			{
				return defaultValue;
			}
		}

		public static DateTime ToDateTime(this string str, [Optional] DateTime defaultValue)
		{
			//Discarded unreachable code: IL_001e, IL_002b
			if (str == null)
			{
				return defaultValue;
			}
			try
			{
				DateTime.TryParse(str, out defaultValue);
				return defaultValue.ToUniversalTime();
			}
			catch
			{
				return defaultValue;
			}
		}

		public static string ToStrOrEmpty(this string str)
		{
			if (str == null)
			{
				return string.Empty;
			}
			return str;
		}

		public static string CalculateMD5Hash(this string input)
		{
			byte[] aSCIIBytes = input.GetASCIIBytes();
			string result = aSCIIBytes.CalculateMD5Hash();
			VariableSizedBufferPool.Release(aSCIIBytes);
			return result;
		}

		public static string CalculateMD5Hash(this byte[] input)
		{
			//Discarded unreachable code: IL_0056
			using (MD5 mD = MD5.Create())
			{
				byte[] array = mD.ComputeHash(input);
				StringBuilder stringBuilder = new StringBuilder(array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x2"));
				}
				VariableSizedBufferPool.Release(array);
				return stringBuilder.ToString();
			}
		}

		internal static string Read(this string str, ref int pos, char block, bool needResult = true)
		{
			return str.Read(ref pos, (char ch) => ch != block, needResult);
		}

		internal static string Read(this string str, ref int pos, Func<char, bool> block, bool needResult = true)
		{
			if (pos >= str.Length)
			{
				return string.Empty;
			}
			str.SkipWhiteSpace(ref pos);
			int num = pos;
			while (pos < str.Length && block(str[pos]))
			{
				pos++;
			}
			string result = ((!needResult) ? null : str.Substring(num, pos - num));
			pos++;
			return result;
		}

		internal static string ReadPossibleQuotedText(this string str, ref int pos)
		{
			string empty = string.Empty;
			if (str == null)
			{
				return empty;
			}
			if (str[pos] == '"')
			{
				str.Read(ref pos, '"', false);
				empty = str.Read(ref pos, '"');
				str.Read(ref pos, ',', false);
			}
			else
			{
				empty = str.Read(ref pos, (char ch) => ch != ',' && ch != ';');
			}
			return empty;
		}

		internal static void SkipWhiteSpace(this string str, ref int pos)
		{
			if (pos < str.Length)
			{
				while (pos < str.Length && char.IsWhiteSpace(str[pos]))
				{
					pos++;
				}
			}
		}

		internal static string TrimAndLower(this string str)
		{
			if (str == null)
			{
				return null;
			}
			char[] array = new char[str.Length];
			int length = 0;
			foreach (char c in str)
			{
				if (!char.IsWhiteSpace(c) && !char.IsControl(c))
				{
					array[length++] = char.ToLowerInvariant(c);
				}
			}
			return new string(array, 0, length);
		}

		internal static char? Peek(this string str, int pos)
		{
			if (pos < 0 || pos >= str.Length)
			{
				return null;
			}
			return str[pos];
		}

		internal static List<HeaderValue> ParseOptionalHeader(this string str)
		{
			List<HeaderValue> list = new List<HeaderValue>();
			if (str == null)
			{
				return list;
			}
			int pos = 0;
			while (pos < str.Length)
			{
				string key = str.Read(ref pos, (char ch) => ch != '=' && ch != ',').TrimAndLower();
				HeaderValue headerValue = new HeaderValue(key);
				if (str[pos - 1] == '=')
				{
					headerValue.Value = str.ReadPossibleQuotedText(ref pos);
				}
				list.Add(headerValue);
			}
			return list;
		}

		internal static List<HeaderValue> ParseQualityParams(this string str)
		{
			List<HeaderValue> list = new List<HeaderValue>();
			if (str == null)
			{
				return list;
			}
			int pos = 0;
			while (pos < str.Length)
			{
				string key = str.Read(ref pos, (char ch) => ch != ',' && ch != ';').TrimAndLower();
				HeaderValue headerValue = new HeaderValue(key);
				if (str[pos - 1] == ';')
				{
					str.Read(ref pos, '=', false);
					headerValue.Value = str.Read(ref pos, ',');
				}
				list.Add(headerValue);
			}
			return list;
		}

		public static void ReadBuffer(this Stream stream, byte[] buffer)
		{
			int num = 0;
			do
			{
				int num2 = stream.Read(buffer, num, buffer.Length - num);
				if (num2 <= 0)
				{
					throw ExceptionHelper.ServerClosedTCPStream();
				}
				num += num2;
			}
			while (num < buffer.Length);
		}

		public static void ReadBuffer(this Stream stream, byte[] buffer, int length)
		{
			int num = 0;
			do
			{
				int num2 = stream.Read(buffer, num, length - num);
				if (num2 <= 0)
				{
					throw ExceptionHelper.ServerClosedTCPStream();
				}
				num += num2;
			}
			while (num < length);
		}

		public static void WriteString(this BufferPoolMemoryStream ms, string str)
		{
			int byteCount = Encoding.UTF8.GetByteCount(str);
			byte[] array = VariableSizedBufferPool.Get(byteCount, true);
			Encoding.UTF8.GetBytes(str, 0, str.Length, array, 0);
			ms.Write(array, 0, byteCount);
			VariableSizedBufferPool.Release(array);
		}

		public static void WriteLine(this BufferPoolMemoryStream ms)
		{
			ms.Write(HTTPRequest.EOL, 0, HTTPRequest.EOL.Length);
		}

		public static void WriteLine(this BufferPoolMemoryStream ms, string str)
		{
			ms.WriteString(str);
			ms.Write(HTTPRequest.EOL, 0, HTTPRequest.EOL.Length);
		}
	}
}
