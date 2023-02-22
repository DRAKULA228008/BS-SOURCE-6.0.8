using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UIStatus : MonoBehaviour
{
	public UILabel label;

	public UISprite background;

	private StringBuilder builder = new StringBuilder();

	private List<int> list = new List<int>();

	private static UIStatus instance;

	private void Start()
	{
		instance = this;
		PhotonRPC.AddMessage("PhotonStatusNewLine", PhotonStatusNewLine);
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

	public static void Add(string text)
	{
		Add(text, false, string.Empty);
	}

	public static void Add(string text, bool local)
	{
		Add(text, local, string.Empty);
	}

	public static void Add(string text, bool local, string localize)
	{
		if (local)
		{
			NewLine(text);
			return;
		}
		PhotonDataWrite data = PhotonRPC.GetData();
		if (string.IsNullOrEmpty(localize))
		{
			data.Write((byte)3);
			data.Write(text);
			PhotonRPC.RPC("PhotonStatusNewLine", PhotonTargets.All, data);
		}
		else
		{
			data.Write((byte)4);
			data.Write(text);
			data.Write(localize);
			PhotonRPC.RPC("PhotonStatusNewLine", PhotonTargets.All, data);
		}
	}

	public static void Add(DamageInfo damageInfo)
	{
		PhotonDataWrite data = PhotonRPC.GetData();
		if (damageInfo.otherPlayer)
		{
			data.Write((byte)2);
			data.Write(damageInfo.player);
			data.Write(PhotonNetwork.player.ID);
			data.Write((byte)damageInfo.weapon);
			data.Write(damageInfo.headshot);
			PhotonRPC.RPC("PhotonStatusNewLine", PhotonTargets.All, data);
		}
		else
		{
			data.Write((byte)1);
			data.Write(PhotonNetwork.player.ID);
			PhotonRPC.RPC("PhotonStatusNewLine", PhotonTargets.All, data);
		}
	}

	public static void DisconnectPlayer(PhotonPlayer player)
	{
		NewLine(GetTeamHexColor(player) + " " + Localization.Get("Disconnect"));
	}

	public static void ConnectPlayer(PhotonPlayer player)
	{
		NewLine(player.UserId + " " + Localization.Get("Connected"));
	}

	[PunRPC]
	private void PhotonStatusNewLine(PhotonMessage message)
	{
		byte b = message.ReadByte();
		string text = string.Empty;
		bool flag = false;
		switch (b)
		{
		case 1:
			text = GetTeamHexColor(PhotonPlayer.Find(message.ReadInt())) + " " + Localization.Get("died");
			break;
		case 2:
			text = KillerStatus(message.ReadInt(), message.ReadInt(), message.ReadByte(), message.ReadBool());
			break;
		case 3:
			text = message.ReadString();
			flag = text[0] == '.';
			if (flag)
			{
				text = text.Remove(0, 1);
			}
			break;
		case 4:
		{
			text = message.ReadString();
			string newValue = Localization.Get(message.ReadString());
			text = text.Replace("[@]", newValue);
			if (flag)
			{
				text = text.Remove(0, 1);
			}
			break;
		}
		}
		if (flag)
		{
			if (PhotonNetwork.player.GetTeam() == message.sender.GetTeam())
			{
				text = text.Remove(0, 1);
				NewLine(text);
			}
		}
		else
		{
			NewLine(text);
		}
	}

	public static void NewLine(string text)
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
	}

	private void UpdateLabel(bool clear)
	{
		label.text = builder.ToString();
		TimerManager.In(0.1f, UpdateBackground);
		if (clear)
		{
			TimerManager.In(5f, RemoveLabel);
		}
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

	private string KillerStatus(int killerID, int death, int weapon, bool headshot)
	{
		string empty = string.Empty;
		PhotonPlayer player = PhotonPlayer.Find(killerID);
		PhotonPlayer player2 = PhotonPlayer.Find(death);
		string teamHexColor = GetTeamHexColor(player, PhotonPlayer.Find(PlayerInput.PlayerHelperID));
		return teamHexColor + "   " + GetSpecialSymbol(weapon) + ((!headshot) ? string.Empty : ("   " + GetSpecialSymbol(99))) + "   " + GetTeamHexColor(player2);
	}

	public static string GetTeamHexColor(PhotonPlayer player)
	{
		return GetTeamHexColor(player.UserId, player.GetTeam());
	}

	public static string GetTeamHexColor(PhotonPlayer player, PhotonPlayer helper)
	{
		return GetTeamHexColor(player.UserId, player.GetTeam());
	}

	public static string GetTeamHexColor(string text, Team team)
	{
		switch (team)
		{
		case Team.Blue:
			text = "[4688E7]" + text + "[-]";
			break;
		case Team.Red:
			text = "[ED2C2D]" + text + "[-]";
			break;
		}
		return text;
	}

	public static string GetSpecialSymbol(int weapon)
	{
		switch (weapon)
		{
		case 1:
			return 'ࠀ'.ToString();
		case 2:
			return 'ࠁ'.ToString();
		case 3:
			return 'ࠂ'.ToString();
		case 4:
			return 'ࠃ'.ToString();
		case 5:
			return 'ࠄ'.ToString();
		case 6:
			return 'ࠅ'.ToString();
		case 7:
			return 'ࠆ'.ToString();
		case 8:
			return 'ࠇ'.ToString();
		case 9:
			return 'ࠈ'.ToString();
		case 10:
			return 'ࠉ'.ToString();
		case 11:
			return 'ࠊ'.ToString();
		case 12:
			return 'ࠋ'.ToString();
		case 13:
			return 'ࠌ'.ToString();
		case 14:
			return 'ࠍ'.ToString();
		case 15:
			return 'ࠎ'.ToString();
		case 16:
			return 'ࠏ'.ToString();
		case 18:
			return 'ࠐ'.ToString();
		case 19:
			return 'ࠑ'.ToString();
		case 21:
			return 'ࠒ'.ToString();
		case 22:
			return 'ࠓ'.ToString();
		case 23:
			return 'ࠔ'.ToString();
		case 24:
			return 'ࠕ'.ToString();
		case 25:
			return '\u0816'.ToString();
		case 26:
			return '\u0817'.ToString();
		case 27:
			return '\u0818'.ToString();
		case 28:
			return '\u0819'.ToString();
		case 29:
			return 'ࠚ'.ToString();
		case 30:
			return '\u081b'.ToString();
		case 31:
			return '\u081c'.ToString();
		case 32:
			return '\u081d'.ToString();
		case 33:
			return '\u081e'.ToString();
		case 34:
			return '\u081f'.ToString();
		case 35:
			return '\u0820'.ToString();
		case 36:
			return '\u0821'.ToString();
		case 37:
			return '\u0822'.ToString();
		case 38:
			return '\u0823'.ToString();
		case 39:
			return 'ࠤ'.ToString();
		case 40:
			return '\u0825'.ToString();
		case 41:
			return '\u0826'.ToString();
		case 42:
			return '\u0827'.ToString();
		case 43:
			return 'ࠨ'.ToString();
		case 44:
			return '\u0829'.ToString();
		case 45:
			return '\u082a'.ToString();
		case 46:
			return '\u082b'.ToString();
		case 48:
			return '\u082c'.ToString();
		case 49:
			return '\u082d'.ToString();
		case 50:
			return '\u082e'.ToString();
		case 51:
			return '࠰'.ToString();
		case 17:
			return '\u082f'.ToString();
		case 98:
			return '࡞'.ToString();
		case 99:
			return '\u085f'.ToString();
		default:
			return string.Empty;
		}
	}
}
