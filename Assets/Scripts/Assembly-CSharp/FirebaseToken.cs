using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public class FirebaseToken
{
	private static int TOKEN_VERSION;

	private string _firebaseSecret;

	public FirebaseToken(string firebaseSecret)
	{
		_firebaseSecret = firebaseSecret;
	}

	public string CreateToken(string uid)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("uid", uid);
		Dictionary<string, object> data = dictionary;
		return CreateToken(data);
	}

	public string CreateToken(Dictionary<string, object> data)
	{
		return CreateToken(data, new FirebaseTokenOptions());
	}

	public string CreateToken(Dictionary<string, object> data, FirebaseTokenOptions options)
	{
		bool flag = data == null || data.Count == 0;
		if (flag && (options == null || (!options.admin && !options.debug)))
		{
			throw new Exception("data is empty and no options are set.  This token will have no effect on Firebase.");
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["v"] = TOKEN_VERSION;
		dictionary["iat"] = secondsSinceEpoch(DateTime.Now);
		validateToken(data, false);
		if (!flag)
		{
			dictionary["d"] = data;
		}
		if (options != null)
		{
			if (options.expires.HasValue)
			{
				dictionary["exp"] = secondsSinceEpoch(options.expires.Value);
			}
			if (options.notBefore.HasValue)
			{
				dictionary["nbf"] = secondsSinceEpoch(options.notBefore.Value);
			}
			if (options.admin)
			{
				dictionary["admin"] = true;
			}
			if (options.debug)
			{
				dictionary["debug"] = true;
			}
		}
		string text = computeToken(dictionary);
		if (text.Length > 1024)
		{
			throw new Exception("Generated token is too long. The token cannot be longer than 1024 bytes.");
		}
		return text;
	}

	private string computeToken(Dictionary<string, object> claims)
	{
		return Encode(claims, _firebaseSecret);
	}

	private static long secondsSinceEpoch(DateTime dt)
	{
		return (long)(dt.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
	}

	private static void validateToken(Dictionary<string, object> data, bool isAdminToken)
	{
		bool flag = data != null && data.ContainsKey("uid");
		if ((!flag && !isAdminToken) || (flag && !(data["uid"] is string)))
		{
			throw new Exception("Data payload must contain a \"uid\" key that must not be a string.");
		}
		if (flag && data["uid"].ToString().Length > 256)
		{
			throw new Exception("Data payload must contain a \"uid\" key that must not be longer than 256 characters.");
		}
	}

	private static string Encode(Dictionary<string, object> payload, string key)
	{
		List<string> list = new List<string>();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("typ", "JWT");
		dictionary.Add("alg", "HS256");
		Dictionary<string, object> json = dictionary;
		byte[] bytes = Encoding.UTF8.GetBytes(FirebaseMiniJson.Serialize(json));
		byte[] bytes2 = Encoding.UTF8.GetBytes(FirebaseMiniJson.Serialize(payload));
		list.Add(Base64UrlEncode(bytes));
		list.Add(Base64UrlEncode(bytes2));
		string s = string.Join(".", list.ToArray());
		byte[] bytes3 = Encoding.UTF8.GetBytes(s);
		HMACSHA256 hMACSHA = new HMACSHA256(Encoding.UTF8.GetBytes(key));
		byte[] input = hMACSHA.ComputeHash(bytes3);
		list.Add(Base64UrlEncode(input));
		return string.Join(".", list.ToArray());
	}

	private static string Base64UrlEncode(byte[] input)
	{
		string text = Convert.ToBase64String(input);
		text = text.Split('=')[0];
		text = text.Replace('+', '-');
		return text.Replace('/', '_');
	}
}
