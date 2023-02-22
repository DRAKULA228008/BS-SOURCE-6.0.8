using System;
using BestHTTP;
using BestHTTP.ServerSentEvents;
using FreeJSON;
using UnityEngine;

public class mChatManager : MonoBehaviour
{
	public UIInput input;

	public UITextList textList;

	private float time;

	private float maxTime;

	private string link;

	private HTTPRequest request;

	private EventSource sse;

	private JsonObject json = new JsonObject();

	private bool isOpen;

	public void Open()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		textList.Clear();
		mPanelManager.Show("GlobalChat", true);
		if (sse == null)
		{
			sse = new EventSource(new Uri("https://api-project-106199818523.firebaseio.com/GlobalChat.json"));
			sse.Open();
			sse.OnOpen += delegate
			{
				isOpen = true;
			};
			sse.OnError += delegate(EventSource eventSource, string error)
			{
				UIToast.Show("Global Chat Error: " + error);
			};
			sse.OnClosed += delegate
			{
				isOpen = false;
			};
			sse.OnMessage += OnMessage;
			sse.OnRetry += OnRetry;
		}
		else
		{
			sse.Open();
		}
	}

	private void OnMessage(EventSource source, Message message)
	{
		if (string.IsNullOrEmpty(message.Data) || message.Data == "null" || !JsonObject.isJson(message.Data))
		{
			return;
		}
		json = JsonObject.Parse(message.Data);
		string text = json.Get<string>("data");
		if (string.IsNullOrEmpty(text) || text == "null")
		{
			return;
		}
		if (JsonObject.isJson(text))
		{
			JsonObject jsonObject = json.Get<JsonObject>("data");
			for (int i = 0; i < jsonObject.Length; i++)
			{
				textList.Add(jsonObject.Get<string>(jsonObject.GetKey(i)));
			}
		}
		else
		{
			textList.Add(text);
		}
	}

	public void Close()
	{
		if (sse != null)
		{
			sse.Close();
			HTTPManager.AbortAll(true);
		}
	}

	private bool OnRetry(EventSource source)
	{
		return true;
	}

	public void OnSubmit()
	{
		if (isOpen)
		{
			if (time + maxTime > Time.time)
			{
				textList.Add("Message sending limit " + StringCache.Get(Mathf.CeilToInt(time + maxTime - Time.time)) + " sec");
			}
			else
			{
				Add(input.value);
			}
		}
	}

	private void Add(string text)
	{
		if (!string.IsNullOrEmpty(text) && !isOnlySpace(text))
		{
			if (text.Contains("\n"))
			{
				text = text.Replace("\n", string.Empty);
			}
			text = BadWordsManager.Check(text);
			if (time + maxTime + 2f > Time.time)
			{
				maxTime += 1f;
			}
			else
			{
				maxTime = 0f;
			}
			time = Time.time;
			text = string.Concat(AccountManager.instance.Data.AccountName, ": ", NGUIText.StripSymbols(text));
			input.value = string.Empty;
			link = string.Format(AccountManager.instance.Links[0], 8, text, 1);
			if (request == null)
			{
				request = new HTTPRequest(new Uri(link));
			}
			else
			{
				request.Uri = new Uri(link);
			}
			request.Send();
		}
	}

	private bool isOnlySpace(string text)
	{
		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] != ' ')
			{
				return false;
			}
		}
		return true;
	}
}
