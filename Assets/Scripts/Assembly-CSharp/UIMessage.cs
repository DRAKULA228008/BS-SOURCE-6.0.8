using System;
using System.Collections.Generic;
using UnityEngine;

public class UIMessage : MonoBehaviour
{
	public class MessageClass
	{
		public string title;

		public string text;

		public Action<bool> callback;

		public Action<bool, object> callback2;

		public object data;
	}

	public UITexture messageSprite;

	public UILabel messageCountLabel;

	public UISprite pauseMessageSprite;

	public List<MessageClass> Messages = new List<MessageClass>();

	private static UIMessage instance;

	private void Start()
	{
		instance = this;
	}

	public static void Add(string text, string title, Action<bool> callback)
	{
		MessageClass messageClass = new MessageClass();
		messageClass.callback = callback;
		messageClass.title = title;
		messageClass.text = text;
		instance.Messages.Add(messageClass);
		instance.messageSprite.cachedGameObject.SetActive(true);
		instance.messageCountLabel.text = instance.Messages.Count.ToString();
		instance.pauseMessageSprite.cachedGameObject.SetActive(true);
	}

	public static void Add(string text, string title, object data, Action<bool, object> callback)
	{
		MessageClass messageClass = new MessageClass();
		messageClass.data = data;
		messageClass.callback2 = callback;
		messageClass.title = title;
		messageClass.text = text;
		instance.Messages.Add(messageClass);
		instance.messageSprite.cachedGameObject.SetActive(true);
		instance.messageCountLabel.text = instance.Messages.Count.ToString();
		instance.pauseMessageSprite.cachedGameObject.SetActive(true);
	}

	public void Click()
	{
		if (Messages.Count == 0)
		{
			messageSprite.cachedGameObject.SetActive(false);
		}
		else
		{
			UIPopUp.ShowPopUp(Messages[0].text, Messages[0].title, Localization.Get("No"), No, Localization.Get("Yes"), Yes);
		}
	}

	private void No()
	{
		UIPanelManager.ShowPanel("Pause");
		if (Messages[0].callback != null)
		{
			Messages[0].callback(false);
		}
		if (Messages[0].callback2 != null)
		{
			Messages[0].callback2(false, Messages[0].data);
		}
		Messages.RemoveAt(0);
		messageCountLabel.text = Messages.Count.ToString();
		if (Messages.Count == 0)
		{
			messageSprite.cachedGameObject.SetActive(false);
			pauseMessageSprite.cachedGameObject.SetActive(false);
		}
	}

	private void Yes()
	{
		UIPanelManager.ShowPanel("Pause");
		if (Messages[0].callback != null)
		{
			Messages[0].callback(true);
		}
		if (Messages[0].callback2 != null)
		{
			Messages[0].callback2(true, Messages[0].data);
		}
		Messages.RemoveAt(0);
		messageCountLabel.text = Messages.Count.ToString();
		if (Messages.Count == 0)
		{
			messageSprite.cachedGameObject.SetActive(false);
			pauseMessageSprite.cachedGameObject.SetActive(false);
		}
	}
}
