using System.Collections.Generic;
using UnityEngine;

public class UIFriends : MonoBehaviour
{
	public const int maxFriends = 20;

	private static List<int> requestPlayers = new List<int>();

	private void Start()
	{
		PhotonRPC.AddMessage("PhotonAddFriend", PhotonAddFriend);
	}

	public void AddFriend()
	{
		if (requestPlayers.Contains(UIPlayerStatistics.SelectPlayer.GetPlayerID()))
		{
			UIToast.Show(Localization.Get("Request has already been sent"));
			return;
		}
		if (AccountManager.instance.Data.Friends.Contains(UIPlayerStatistics.SelectPlayer.GetPlayerID()))
		{
			UIToast.Show(Localization.Get("Player is already in your friends"));
			return;
		}
		if (AccountManager.instance.Data.Friends.Count >= 20)
		{
			UIToast.Show(Localization.Get("Reached the maximum number of friends"));
			return;
		}
		PhotonDataWrite data = PhotonRPC.GetData();
		data.Write((byte)1);
		PhotonRPC.RPC("PhotonAddFriend", UIPlayerStatistics.SelectPlayer, data);
		requestPlayers.Add(UIPlayerStatistics.SelectPlayer.GetPlayerID());
	}

	[PunRPC]
	private void PhotonAddFriend(PhotonMessage message)
	{
		switch (message.ReadByte())
		{
		case 1:
		{
			PhotonPlayer player = message.sender;
			if (player == null || AccountManager.instance.Data.Friends.Contains(player.GetPlayerID()) || AccountManager.instance.Data.Friends.Count >= 20)
			{
				break;
			}
			UIMessage.Add(Localization.Get("Add friend") + ": " + player.UserId, Localization.Get("Friends"), player.ID, delegate(bool result, object obj)
			{
				if (AccountManager.instance.Data.Friends.Count >= 20)
				{
					UIToast.Show(Localization.Get("Reached the maximum number of friends"));
				}
				else
				{
					player = PhotonPlayer.Find((int)obj);
					if (player != null)
					{
						if (result)
						{
							CryptoPrefs.SetString("Friend_#" + (int)obj, player.UserId);
							AccountManager.AddFriend(player.GetPlayerID(), delegate
							{
								UIToast.Show(Localization.Get("Added friend"));
							}, delegate(string error)
							{
								UIToast.Show(error);
							});
						}
						PhotonDataWrite data = PhotonRPC.GetData();
						data.Write((byte)2);
						data.Write(result);
						PhotonRPC.RPC("PhotonAddFriend", player, data);
						requestPlayers.Add(player.GetPlayerID());
					}
				}
			});
			break;
		}
		case 2:
			if (message.ReadBool())
			{
				if (AccountManager.instance.Data.Friends.Count < 20)
				{
					PhotonPlayer sender = message.sender;
					CryptoPrefs.SetString("Friend_#" + sender.GetPlayerID(), sender.UserId);
					AccountManager.AddFriend(sender.GetPlayerID(), delegate
					{
						UIToast.Show(Localization.Get("Added friend"));
					}, delegate(string error)
					{
						UIToast.Show(error);
					});
				}
			}
			else
			{
				UIToast.Show("declined the request");
			}
			break;
		}
	}
}
