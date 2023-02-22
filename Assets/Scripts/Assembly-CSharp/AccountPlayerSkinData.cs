using System;
using System.Collections.Generic;
using FreeJSON;
using UnityEngine;

[Serializable]
public class AccountPlayerSkinData
{
	public List<CryptoInt> Head = new List<CryptoInt>();

	public List<CryptoInt> Body = new List<CryptoInt>();

	public List<CryptoInt> Legs = new List<CryptoInt>();

	public List<CryptoInt> Select = new List<CryptoInt> { 0, 0, 0 };

	[HideInInspector]
	public List<CryptoInt> LastSelect = new List<CryptoInt> { 0, 0, 0 };

	public void Deserialize(JsonObject json)
	{
		if (json == null)
		{
			return;
		}
		Head = JsonObjectToList(json.Get<JsonObject>("Head"));
		Body = JsonObjectToList(json.Get<JsonObject>("Body"));
		Legs = JsonObjectToList(json.Get<JsonObject>("Legs"));
		if (Head.Count == 0)
		{
			Head = JsonArrayToList(json.Get<JsonArray>("Head"));
		}
		if (Body.Count == 0)
		{
			Body = JsonArrayToList(json.Get<JsonArray>("Body"));
		}
		if (Legs.Count == 0)
		{
			Legs = JsonArrayToList(json.Get<JsonArray>("Legs"));
		}
		try
		{
			if (!json.ContainsKey("Select"))
			{
				return;
			}
			string text = json.Get<string>("Select");
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(","[0]);
				int result = 0;
				for (int i = 0; i < array.Length; i++)
				{
					int.TryParse(array[i], out result);
					Select[i] = result;
					LastSelect[i] = result;
				}
			}
		}
		catch
		{
			Select = new List<CryptoInt> { 0, 0, 0 };
			LastSelect = new List<CryptoInt> { 0, 0, 0 };
		}
	}

	private List<CryptoInt> JsonObjectToList(JsonObject json)
	{
		List<CryptoInt> list = new List<CryptoInt>();
		for (int i = 0; i < json.Length; i++)
		{
			list.Add(json.Get<int>(json.GetKey(i)));
		}
		return list;
	}

	private List<CryptoInt> JsonArrayToList(JsonArray json)
	{
		List<CryptoInt> list = new List<CryptoInt>();
		for (int i = 0; i < json.Length; i++)
		{
			list.Add(json.Get<int>(i));
		}
		return list;
	}
}
