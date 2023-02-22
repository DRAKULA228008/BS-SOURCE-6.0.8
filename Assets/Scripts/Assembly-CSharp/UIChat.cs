using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UIChat : MonoBehaviour
{
	public UILabel label;

	public UIInput input;

	public UISprite background;

	private StringBuilder builder = new StringBuilder();

	private List<int> list = new List<int>();

	private static UIChat instance;

	private float time;

	private float maxTime;

	public static bool actived;

	private static float textShowDuration = 8f;

    public static bool isChat;

    private void Start()
	{
		instance = this;
		PhotonRPC.AddMessage("PhotonChatNewLine", PhotonChatNewLine);
		textShowDuration = Mathf.Clamp(GameConsole.Load("chat_show_duration", 8f), 1f, 20f);
	}

	private void OnEnable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
	}

	private void UpdateBackground()
	{
		if (string.IsNullOrEmpty(label.text))
		{
			background.alpha = 0f;
		}
		else
		{
			background.height = label.height + 6;
			background.alpha = 0.2f;
		}
		background.UpdateWidget();
	}

	private void GetButtonDown(string name)
	{
		if (name == "Chat")
		{
			Show();
		}
	}

	public static void Add(string text)
	{
		if (instance.time + instance.maxTime + 2f > Time.time)
		{
			instance.maxTime += 1f;
		}
		else
		{
			instance.maxTime = 0f;
		}
		instance.time = Time.time;
		bool value = text[0] == '.';
		PhotonDataWrite data = PhotonRPC.GetData();
		data.Write(text);
		data.Write(value);
		PhotonRPC.RPC("PhotonChatNewLine", PhotonTargets.All, data);
	}

	[PunRPC]
	private void PhotonChatNewLine(PhotonMessage message)
	{
		string text = message.ReadString();
		bool flag = message.ReadBool();
		PhotonPlayer sender = message.sender;
		text = ((!Settings.FilterChat) ? NGUIText.StripSymbols(text) : BadWordsManager.Check(NGUIText.StripSymbols(text)));
		if (flag)
		{
			text = text.Remove(0, 1);
			text = UIStatus.GetTeamHexColor("[Team] " + sender.UserId, sender.GetTeam()) + ": " + text;
		}
		else
		{
			text = UIStatus.GetTeamHexColor(sender) + ": " + text;
		}
		if (GameManager.globalChat)
		{
			if (flag)
			{
				if (PhotonNetwork.player.GetTeam() == sender.GetTeam())
				{
					NewLine(text);
				}
			}
			else
			{
				NewLine(text);
			}
		}
		else if (sender.GetDead())
		{
			text = text.Insert(text.IndexOf("]") + 1, "[" + Localization.Get("Dead") + "] ");
			if (!PhotonNetwork.player.GetDead())
			{
				return;
			}
			if (flag)
			{
				if (PhotonNetwork.player.GetTeam() == sender.GetTeam())
				{
					NewLine(text);
				}
			}
			else
			{
				NewLine(text);
			}
		}
		else if (flag)
		{
			if (PhotonNetwork.player.GetTeam() == sender.GetTeam())
			{
				NewLine(text);
			}
		}
		else
		{
			NewLine(text);
		}
	}

	private void Show()
	{
		if (PhotonNetwork.offlineMode || GameManager.loadingLevel)
		{
			return;
		}
		if (time + maxTime > Time.time)
		{
			NewLine("Message sending limit " + StringCache.Get(Mathf.CeilToInt(time + maxTime - Time.time)) + " sec");
		}
		else if (!UICamera.inputHasFocus)
		{
			input.value = string.Empty;
			TimerManager.In(0.1f, delegate
			{
				UICamera.selectedObject = input.gameObject;
				actived = true;
			});
		}
	}

	public void OnSubmit()
	{
		string value = input.value;
		value = value.Replace("\n", string.Empty);
		if (!string.IsNullOrEmpty(value))
		{
			input.value = string.Empty;
			input.isSelected = false;
			TimerManager.In(0.1f, delegate
			{
				actived = false;
			});
			Add(value);
		}
	}

	public static void NewLine(string text)
	{
		nProfiler.BeginSample("UIChat.NewLine");
		if (Settings.Chat)
		{
			if (instance.builder.Length > 0)
			{
				instance.builder.Insert(0, text + "\n");
			}
			else
			{
				instance.builder.Append(text);
			}
			instance.list.Add(text.Length);
			instance.UpdateLabel(true);
			nProfiler.EndSample();
		}
	}

	private void UpdateLabel(bool clear)
	{
		nProfiler.BeginSample("UIChat.UpdateLabel");
		label.text = builder.ToString();
		TimerManager.In(0.2f, UpdateBackground);
		if (clear)
		{
			TimerManager.In(textShowDuration, RemoveLabel);
		}
		nProfiler.EndSample();
	}

	private void RemoveLabel()
	{
		if (list.Count == 1)
		{
			builder.Length = 0;
			builder.Capacity = 0;
		}
		else
		{
			builder.Remove(builder.Length - (list[0] + 1), list[0] + 1);
		}
		list.RemoveAt(0);
		UpdateLabel(false);
	}
}
