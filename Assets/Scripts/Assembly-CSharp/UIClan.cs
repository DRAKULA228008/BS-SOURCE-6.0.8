using System.Collections.Generic;
using UnityEngine;

public class UIClan : MonoBehaviour
{
	public const int maxPlayers = 20;

	private static List<int> requestPlayers = new List<int>();

	private void Start()
	{
		PhotonRPC.AddMessage("PhotonAddClan", PhotonAddClan);
	}

	public void AddPlayer()
	{
		if (string.IsNullOrEmpty(AccountManager.instance.Data.Clan))
		{
			MonoBehaviour.print(1);
			return;
		}
		if ((int)AccountManager.instance.Data.ID != ClanManager.admin)
		{
			MonoBehaviour.print(2);
			return;
		}
		if (requestPlayers.Contains(UIPlayerStatistics.SelectPlayer.GetPlayerID()))
		{
			MonoBehaviour.print(3);
			UIToast.Show(Localization.Get("Request has already been sent"));
			return;
		}
		if (ClanManager.players.Contains(UIPlayerStatistics.SelectPlayer.GetPlayerID()) || !string.IsNullOrEmpty(UIPlayerStatistics.SelectPlayer.GetClan()))
		{
			MonoBehaviour.print(4);
			UIToast.Show(Localization.Get("Player is already in clan"));
			return;
		}
		if (ClanManager.players.Length >= 20)
		{
			MonoBehaviour.print(5);
			UIToast.Show(Localization.Get("The maximum number of players in the clan"));
			return;
		}
		PhotonDataWrite data = PhotonRPC.GetData();
		data.Write((byte)1);
		data.Write(AccountManager.instance.Data.Clan.ToString());
		PhotonRPC.RPC("PhotonAddClan", UIPlayerStatistics.SelectPlayer, data);
		requestPlayers.Add(UIPlayerStatistics.SelectPlayer.GetPlayerID());
	}

	[PunRPC]
	private void PhotonAddClan(PhotonMessage message)
	{
		switch (message.ReadByte())
		{
		case 1:
		{
			string arg = message.ReadString();
			PhotonPlayer player = message.sender;
			if (player == null || string.IsNullOrEmpty(player.GetClan()) || !string.IsNullOrEmpty(AccountManager.instance.Data.Clan))
			{
				break;
			}
			UIMessage.Add(string.Format(Localization.Get("Your invite {0} to clan {1}. Want to join?"), player.NickName, arg), Localization.Get("Clan"), player.ID, delegate(bool result, object obj)
			{
				player = PhotonPlayer.Find((int)obj);
				if (player != null)
				{
					PhotonDataWrite data = PhotonRPC.GetData();
					data.Write((byte)2);
					data.Write(result);
					PhotonRPC.RPC("PhotonAddClan", player, data);
					requestPlayers.Add(player.GetPlayerID());
				}
			});
			break;
		}
		case 2:
			if (message.ReadBool())
			{
				PhotonPlayer player2 = message.sender;
				CryptoPrefs.SetString("Friend_#" + player2.GetPlayerID(), player2.UserId);
				AccountManager.Clan.AddPlayer(player2.GetPlayerID(), delegate
				{
					UIToast.Show(player2.NickName + " " + Localization.Get("joined the clan"));
					PhotonDataWrite data2 = PhotonRPC.GetData();
					data2.Write((byte)3);
					data2.Write(AccountManager.instance.Data.Clan.ToString());
					PhotonRPC.RPC("PhotonAddClan", player2, data2);
				}, delegate(string error)
				{
					UIToast.Show(error);
				});
			}
			else
			{
				UIToast.Show("declined the request");
			}
			break;
		case 3:
		{
			string text = message.ReadString();
			AccountManager.instance.Data.Clan = text;
			UIToast.Show(PhotonNetwork.player.NickName + " " + Localization.Get("joined the clan"));
			break;
		}
		}
	}
}
